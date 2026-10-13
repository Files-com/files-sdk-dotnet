using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Counts a download's progress as it goes, and reports snapshots to the caller's handler: when the size becomes
    // known, as each part is written, at most every ReportInterval while bytes arrive, and at the end. Counting is
    // lock-free; reports are serialized, so a handler never runs twice at once.
    internal sealed class DownloadProgressTracker
    {
        private static readonly long ReportInterval = Stopwatch.Frequency / 4;

        private readonly IProgress<DownloadProgress> handler;
        private readonly string path;
        private readonly string localPath;
        private readonly object reportLock = new object();
        private readonly long startedAt = Stopwatch.GetTimestamp();
        private long totalBytes;
        private int totalBytesKnown;
        private int versionBound;
        private long resumedBytes;
        private long writtenBytes;
        private long receivedBytes;
        private long partsWritten;
        private int activeParts;
        private int retries;
        private int restarts;
        private long requestWaitTicks;
        private long bufferWaitTicks;
        private long lastReport;
        private Func<DownloadConcurrency> concurrency;

        internal DownloadProgressTracker(IProgress<DownloadProgress> handler, string path, string localPath)
        {
            this.handler = handler;
            this.path = path;
            this.localPath = localPath;
        }

        internal TimeSpan Elapsed => ToTimeSpan(Stopwatch.GetTimestamp() - startedAt);

        internal long PartsWritten => Interlocked.Read(ref partsWritten);

        internal void SizeKnown(long size, bool bound)
        {
            Interlocked.Exchange(ref totalBytes, size);
            Volatile.Write(ref totalBytesKnown, 1);
            Volatile.Write(ref versionBound, bound ? 1 : 0);
        }

        internal void Resumed(long bytes)
        {
            Interlocked.Exchange(ref resumedBytes, bytes);
            Interlocked.Exchange(ref writtenBytes, bytes);
        }

        // The download starts over: what it wrote no longer counts.
        internal void Restarted()
        {
            Interlocked.Increment(ref restarts);
            Interlocked.Exchange(ref writtenBytes, 0);
            Interlocked.Exchange(ref resumedBytes, 0);
            Interlocked.Exchange(ref partsWritten, 0);
        }

        internal void Received(int bytes)
        {
            Interlocked.Add(ref receivedBytes, bytes);
            ReportIfDue();
        }

        // Bytes written in order and counted as they are, for a file not fetched in parts.
        internal void Written(long bytes)
        {
            Interlocked.Add(ref writtenBytes, bytes);
            if (Volatile.Read(ref totalBytesKnown) == 0)
            {
                Interlocked.Add(ref totalBytes, bytes);
            }
        }

        internal void PartWritten(long bytes)
        {
            Interlocked.Add(ref writtenBytes, bytes);
            Interlocked.Increment(ref partsWritten);
            Report();
        }

        internal void PartStarted()
        {
            Interlocked.Increment(ref activeParts);
        }

        internal void PartEnded()
        {
            Interlocked.Decrement(ref activeParts);
        }

        // Reports include the concurrency snapshot, once the download fetches parts.
        internal void Observe(Func<DownloadConcurrency> snapshot)
        {
            Volatile.Write(ref concurrency, snapshot);
        }

        internal void Retried()
        {
            Interlocked.Increment(ref retries);
        }

        internal Task<T> TimeRequestWait<T>(Task<T> wait)
        {
            return Time(wait, ticks => Interlocked.Add(ref requestWaitTicks, ticks));
        }

        internal Task<T> TimeBufferWait<T>(Task<T> wait)
        {
            return Time(wait, ticks => Interlocked.Add(ref bufferWaitTicks, ticks));
        }

        internal DownloadProgress Snapshot()
        {
            return new DownloadProgress(
                path,
                localPath,
                Interlocked.Read(ref totalBytes),
                Volatile.Read(ref totalBytesKnown) == 1,
                Volatile.Read(ref versionBound) == 1,
                Interlocked.Read(ref resumedBytes),
                Interlocked.Read(ref writtenBytes),
                Interlocked.Read(ref receivedBytes),
                Interlocked.Read(ref partsWritten),
                Volatile.Read(ref activeParts),
                Volatile.Read(ref retries),
                Volatile.Read(ref restarts),
                ToTimeSpan(Interlocked.Read(ref requestWaitTicks)),
                ToTimeSpan(Interlocked.Read(ref bufferWaitTicks)),
                Elapsed,
                ProcessResourceSample.Current(),
                Volatile.Read(ref concurrency)?.Invoke());
        }

        internal void Report()
        {
            if (handler == null)
            {
                return;
            }
            lock (reportLock)
            {
                Volatile.Write(ref lastReport, Stopwatch.GetTimestamp());
                handler.Report(Snapshot());
            }
        }

        private void ReportIfDue()
        {
            if (handler != null && Stopwatch.GetTimestamp() - Volatile.Read(ref lastReport) >= ReportInterval)
            {
                Report();
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

        private static TimeSpan ToTimeSpan(long stopwatchTicks)
        {
            return TimeSpan.FromSeconds(stopwatchTicks / (double)Stopwatch.Frequency);
        }
    }
}