using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Counts an upload's progress with cheap atomic updates, and reports snapshots and trace records to the caller's
    // handlers one at a time, so reports never overlap and never go backward.
    internal sealed class UploadProgressTracker
    {
        private readonly IProgress<TransferProgress> handler;
        private readonly IProgress<UploadTraceEvent> trace;
        private readonly string path;
        private readonly UploadConcurrency pendingConcurrency;
        private readonly object reportLock = new object();
        private long totalBytes;
        private int totalBytesKnown;
        private long spooledBytes;
        private long acknowledgedBytes;
        private long sentBytes;
        private int partsAcknowledged;
        private int activeParts;
        private int partRetries;
        private long admissionWaitTicks;
        private long requestWaitTicks;
        private long bufferWaitTicks;
        private int readyParts;
        private int readyLimitWaits;
        private Func<UploadConcurrency> concurrency;
        private Func<UploadPartSizing> partSizing;

        // mode and callerTuning describe the upload's concurrency until the controller exists, once Files.com has
        // answered its first request.
        internal UploadProgressTracker(IProgress<TransferProgress> handler, IProgress<UploadTraceEvent> trace, string path, UploadConcurrencyMode mode, bool callerTuning)
        {
            this.handler = handler;
            this.trace = trace;
            this.path = path;
            pendingConcurrency = new UploadConcurrency { Mode = mode, ProfileSource = callerTuning ? UploadProfileSource.Options : UploadProfileSource.Server };
            StartedAt = Stopwatch.GetTimestamp();
        }

        // When the upload started, on the Stopwatch clock.
        internal long StartedAt { get; }

        internal TimeSpan Elapsed => ToTimeSpan(Stopwatch.GetTimestamp() - StartedAt);

        internal long SentBytes => Interlocked.Read(ref sentBytes);

        internal bool Tracing => trace != null;

        // Where snapshots get the upload's concurrency and part sizing from.
        internal void Observe(Func<UploadConcurrency> concurrencySource, Func<UploadPartSizing> sizingSource)
        {
            concurrency = concurrencySource;
            partSizing = sizingSource;
        }

        // The upload's size, once the source knows it; null for a stream of unknown length.
        internal void LengthKnown(long? length)
        {
            if (length.HasValue)
            {
                Interlocked.Exchange(ref totalBytes, length.Value);
                Volatile.Write(ref totalBytesKnown, 1);
            }
        }

        // A part has been read. For a stream of unknown length, the total grows with it, and is known once the stream
        // has ended.
        internal void PartRead(PartDescriptor part, bool lengthKnown)
        {
            if (!lengthKnown)
            {
                Interlocked.Add(ref totalBytes, part.Length);
                if (part.IsLast)
                {
                    Volatile.Write(ref totalBytesKnown, 1);
                }
            }
        }

        // A stream of unknown length ended where its last part read did: the total is what its parts hold.
        internal void StreamEnded()
        {
            Volatile.Write(ref totalBytesKnown, 1);
        }

        // A stream is being copied to its spool file: reported as it starts.
        internal void Spooling()
        {
            Report();
        }

        internal void Spooled(int count)
        {
            Interlocked.Add(ref spooledBytes, count);
        }

        internal void AddSentBytes(int count)
        {
            Interlocked.Add(ref sentBytes, count);
        }

        internal void PartStarted()
        {
            Interlocked.Increment(ref activeParts);
        }

        internal void PartEnded()
        {
            Interlocked.Decrement(ref activeParts);
        }

        internal void PartAccepted(long length)
        {
            Interlocked.Add(ref acknowledgedBytes, length);
            Interlocked.Increment(ref partsAcknowledged);
        }

        // The upload starts over as a new one: the parts accepted before no longer count.
        internal void Restart()
        {
            Interlocked.Exchange(ref acknowledgedBytes, 0);
            Interlocked.Exchange(ref partsAcknowledged, 0);
        }

        internal void PartRetried()
        {
            Interlocked.Increment(ref partRetries);
        }

        internal void ReadyChanged(int change)
        {
            Interlocked.Add(ref readyParts, change);
        }

        internal void ReadyLimitReached()
        {
            Interlocked.Increment(ref readyLimitWaits);
        }

        internal Task<T> TimeAdmissionWait<T>(Task<T> wait)
        {
            return Time(wait, ticks => Interlocked.Add(ref admissionWaitTicks, ticks));
        }

        internal Task<T> TimeRequestWait<T>(Task<T> wait)
        {
            return Time(wait, ticks => Interlocked.Add(ref requestWaitTicks, ticks));
        }

        internal Task<T> TimeBufferWait<T>(Task<T> wait)
        {
            return Time(wait, ticks => Interlocked.Add(ref bufferWaitTicks, ticks));
        }

        internal TransferProgress Snapshot()
        {
            return new TransferProgress(
                path,
                Interlocked.Read(ref totalBytes),
                Volatile.Read(ref totalBytesKnown) == 1,
                Interlocked.Read(ref spooledBytes),
                Interlocked.Read(ref acknowledgedBytes),
                Interlocked.Read(ref sentBytes),
                Volatile.Read(ref partsAcknowledged),
                Volatile.Read(ref activeParts),
                Volatile.Read(ref partRetries),
                ToTimeSpan(Interlocked.Read(ref admissionWaitTicks)),
                ToTimeSpan(Interlocked.Read(ref requestWaitTicks)),
                ToTimeSpan(Interlocked.Read(ref bufferWaitTicks)),
                Volatile.Read(ref readyParts),
                Volatile.Read(ref readyLimitWaits),
                Elapsed,
                concurrency != null ? concurrency() : pendingConcurrency,
                partSizing?.Invoke(),
                ProcessResourceSample.Current());
        }

        internal void Report()
        {
            if (handler == null)
            {
                return;
            }
            lock (reportLock)
            {
                handler.Report(Snapshot());
            }
        }

        // Reports trace records, when the caller asked for them.
        internal void Trace(UploadTraceEvent record, IReadOnlyList<UploadTraceEvent> more = null)
        {
            if (trace == null)
            {
                return;
            }
            lock (reportLock)
            {
                trace.Report(record);
                if (more != null)
                {
                    foreach (UploadTraceEvent next in more)
                    {
                        trace.Report(next);
                    }
                }
            }
        }

        private static async Task<T> Time<T>(Task<T> wait, Action<long> add)
        {
            if (wait.IsCompleted)
            {
                return await wait;
            }
            long start = Stopwatch.GetTimestamp();
            try
            {
                return await wait;
            }
            finally
            {
                add(Stopwatch.GetTimestamp() - start);
            }
        }

        internal static TimeSpan ToTimeSpan(long stopwatchTicks)
        {
            return TimeSpan.FromSeconds(stopwatchTicks / (double)Stopwatch.Frequency);
        }
    }
}