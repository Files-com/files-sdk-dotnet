using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace FilesCom.Transfers
{
    // Counts a directory transfer's progress from its files' own progress, and reports snapshots to the caller's handler:
    // as files start and end, at most every ReportInterval while bytes move, and at the end. Reports are serialized, so a
    // handler never runs twice at once.
    internal sealed class DirectoryProgressTracker
    {
        private static readonly long ReportInterval = Stopwatch.Frequency / 4;

        private readonly IProgress<DirectoryTransferProgress> handler;
        private readonly string localDirectory;
        private readonly string remoteDirectory;
        private readonly object gate = new object();
        private readonly object reportLock = new object();
        private readonly long startedAt = Stopwatch.GetTimestamp();
        private readonly Dictionary<DirectoryTransfer.PlannedFile, FileState> active = new Dictionary<DirectoryTransfer.PlannedFile, FileState>();
        private readonly int filesTotal;
        private int filesCompletedEarlier;
        private long bytesTotal;
        private int filesDone;
        private long bytesDone;
        private long bytesBefore;
        private long networkBytes;
        private long lastReport;
        private Func<int> fileLimit;

        internal DirectoryProgressTracker(IProgress<DirectoryTransferProgress> handler, string localDirectory, string remoteDirectory, IReadOnlyCollection<DirectoryTransfer.PlannedFile> files)
        {
            this.handler = handler;
            this.localDirectory = localDirectory;
            this.remoteDirectory = remoteDirectory;
            filesTotal = files.Count;
            bytesTotal = files.Sum(file => file.Size);
        }

        internal void Started(DirectoryTransfer.PlannedFile file)
        {
            lock (gate)
            {
                active[file] = new FileState();
            }
            Report(force: true);
        }

        // A download's size, once Files.com gave it, where the listing did not.
        internal void SizeKnown(DirectoryTransfer.PlannedFile file, long? size)
        {
            lock (gate)
            {
                if (size.HasValue && !file.SizeKnown)
                {
                    bytesTotal += size.Value - file.Size;
                    file.Size = size.Value;
                    file.SizeKnown = true;
                }
            }
        }

        // A file's own progress: the bytes it has done, including what an earlier run did, and those it sent or received.
        internal void Update(DirectoryTransfer.PlannedFile file, long done, long network)
        {
            lock (gate)
            {
                if (!active.TryGetValue(file, out FileState state))
                {
                    return;
                }
                if (state.Reports++ == 0)
                {
                    // What a continued file had before this run.
                    state.Before = done;
                    bytesBefore += done;
                }
                state.Done = done;
                networkBytes += network - state.Network;
                state.Network = network;
            }
            Report(force: false);
        }

        // An earlier run completed file, and its source is unchanged: it counts as done, and not as this run's.
        internal void Skipped(DirectoryTransfer.PlannedFile file)
        {
            lock (gate)
            {
                filesCompletedEarlier++;
                bytesBefore += file.Size;
            }
            Done(file);
        }

        internal void Done(DirectoryTransfer.PlannedFile file)
        {
            lock (gate)
            {
                filesDone++;
                bytesDone += file.Size;
                if (active.TryGetValue(file, out FileState state))
                {
                    // Counted as done from now on.
                    state.Done = 0;
                    state.Completed = true;
                }
            }
        }

        internal void Ended(DirectoryTransfer.PlannedFile file)
        {
            lock (gate)
            {
                if (active.TryGetValue(file, out FileState state) && !state.Completed)
                {
                    // Stopped: what it had before this run no longer counts as this run's.
                    bytesBefore -= state.Before;
                }
                active.Remove(file);
            }
            Report(force: true);
        }

        // Reports include the transfer's file limit.
        internal void Observe(Func<int> limit)
        {
            System.Threading.Volatile.Write(ref fileLimit, limit);
        }

        internal DirectoryTransferProgress Report(bool force)
        {
            long now = Stopwatch.GetTimestamp();
            if (!force && now - System.Threading.Interlocked.Read(ref lastReport) < ReportInterval)
            {
                return null;
            }
            lock (reportLock)
            {
                DirectoryTransferProgress snapshot = Snapshot();
                System.Threading.Interlocked.Exchange(ref lastReport, now);
                handler?.Report(snapshot);
                return snapshot;
            }
        }

        private DirectoryTransferProgress Snapshot()
        {
            lock (gate)
            {
                long done = bytesDone + active.Values.Sum(state => state.Done);
                TimeSpan elapsed = TimeSpan.FromSeconds((Stopwatch.GetTimestamp() - startedAt) / (double)Stopwatch.Frequency);
                double perSecond = elapsed.TotalSeconds > 0 ? Math.Max(0, done - bytesBefore) / elapsed.TotalSeconds : 0;
                return new DirectoryTransferProgress(localDirectory, remoteDirectory, filesTotal, filesDone, filesCompletedEarlier, active.Count(entry => !entry.Value.Completed), bytesTotal, done, networkBytes, perSecond, elapsed, System.Threading.Volatile.Read(ref fileLimit)?.Invoke() ?? 0);
            }
        }

        private sealed class FileState
        {
            internal int Reports;
            internal long Before;
            internal long Done;
            internal long Network;
            internal bool Completed;
        }
    }
}