using System;
using System.Diagnostics;

namespace FilesCom.Transfers
{
    /// <summary>
    /// The CPU and memory use of the whole process the SDK runs in, as part of a <see cref="TransferProgress"/>. It
    /// covers everything the process does, not just transfers, and is sampled at most once a second, however many
    /// transfers report it.
    /// </summary>
    public sealed class ProcessResourceSample
    {
        private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);
        private static readonly object sampling = new object();
        private static ProcessResourceSample latest;
        private static long latestTimestamp;
        private static bool unavailable;

        private ProcessResourceSample(DateTimeOffset sampledAt, TimeSpan totalProcessorTime, double? processorPercent, long workingSetBytes, long managedHeapBytes)
        {
            SampledAt = sampledAt;
            TotalProcessorTime = totalProcessorTime;
            ProcessorPercent = processorPercent;
            WorkingSetBytes = workingSetBytes;
            ManagedHeapBytes = managedHeapBytes;
        }

        /// <summary>When the sample was taken.</summary>
        public DateTimeOffset SampledAt { get; }

        /// <summary>The CPU time the process has used since it started, on all processors.</summary>
        public TimeSpan TotalProcessorTime { get; }

        /// <summary>
        /// The process's average CPU use since the previous sample, as a percentage of all the machine's processors
        /// (0 to 100), or null for the first sample.
        /// </summary>
        public double? ProcessorPercent { get; }

        /// <summary>The physical memory the process uses, in bytes.</summary>
        public long WorkingSetBytes { get; }

        /// <summary>The bytes the process's managed heap is thought to hold, without forcing a collection.</summary>
        public long ManagedHeapBytes { get; }

        // The latest sample, taken now if the latest is more than a second old; null where the platform does not
        // report process resources.
        internal static ProcessResourceSample Current()
        {
            lock (sampling)
            {
                long now = Stopwatch.GetTimestamp();
                if (unavailable || (latest != null && now - latestTimestamp < Interval.TotalSeconds * Stopwatch.Frequency))
                {
                    return latest;
                }
                try
                {
                    TimeSpan processorTime;
                    long workingSet;
                    using (Process process = Process.GetCurrentProcess())
                    {
                        processorTime = process.TotalProcessorTime;
                        workingSet = process.WorkingSet64;
                    }
                    double? percent = null;
                    if (latest != null)
                    {
                        double wall = (now - latestTimestamp) / (double)Stopwatch.Frequency;
                        double cpu = (processorTime - latest.TotalProcessorTime).TotalSeconds;
                        percent = Math.Max(0, Math.Min(100, cpu / (wall * Environment.ProcessorCount) * 100));
                    }
                    latest = new ProcessResourceSample(DateTimeOffset.UtcNow, processorTime, percent, workingSet, GC.GetTotalMemory(false));
                    latestTimestamp = now;
                }
                catch (Exception e) when (e is PlatformNotSupportedException || e is NotSupportedException || e is InvalidOperationException || e is System.ComponentModel.Win32Exception)
                {
                    unavailable = true;
                }
                return latest;
            }
        }
    }
}