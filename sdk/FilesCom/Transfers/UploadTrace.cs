using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A detailed record of an upload's work, for diagnostics: <see cref="UploadAttemptTrace"/> for each request that
    /// sent a part, <see cref="UploadDecisionTrace"/> for each concurrency decision and <see cref="UploadSizingTrace"/>
    /// for each part size decision. Set <see cref="UploadOptions.Trace"/> to receive them.
    /// </summary>
    /// <remarks>
    /// Records never include part addresses, headers or credentials.
    /// </remarks>
    public abstract class UploadTraceEvent
    {
        internal UploadTraceEvent(string path, TimeSpan elapsed)
        {
            Path = path;
            Elapsed = elapsed;
        }

        /// <summary>The remote path being uploaded.</summary>
        public string Path { get; }

        /// <summary>The time since the upload started.</summary>
        public TimeSpan Elapsed { get; }
    }

    /// <summary>
    /// One request that sent a part, recorded when its answer arrived and before any retry waits.
    /// </summary>
    /// <remarks>
    /// Requests that were cancelled, or that stopped because the file or stream could not be read, are not recorded:
    /// they say nothing about the network.
    /// </remarks>
    public sealed class UploadAttemptTrace : UploadTraceEvent
    {
        internal UploadAttemptTrace(string path, TimeSpan elapsed) : base(path, elapsed)
        {
        }

        /// <summary>The part number, from 1.</summary>
        public long PartNumber { get; internal set; }

        /// <summary>Which attempt to send the part this was, from 1.</summary>
        public int Attempt { get; internal set; }

        /// <summary>Whether the server accepted the part.</summary>
        public bool Accepted { get; internal set; }

        /// <summary>The storage server's HTTP status, or null if no answer was received.</summary>
        public int? StatusCode { get; internal set; }

        /// <summary>Whether the answer asked the upload to slow down: HTTP 429 or 503, or <c>Retry-After</c> on a failure.</summary>
        public bool Throttled { get; internal set; }

        /// <summary>The server's <c>Retry-After</c>, when it sent one.</summary>
        public TimeSpan? RetryAfter { get; internal set; }

        /// <summary>The time from sending the request to receiving its answer. Waits before it are not included.</summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>The time the request waited to be admitted, by the upload's own target and cooldown and by its limiter.</summary>
        public TimeSpan AdmissionWait { get; internal set; }

        /// <summary>The number of bytes in the part.</summary>
        public long PartBytes { get; internal set; }

        /// <summary>The bytes this request wrote to the network, including any it resent to follow a redirect.</summary>
        public long SentBytes { get; internal set; }

        /// <summary>The upload's target once this request had been taken into account.</summary>
        public int Target { get; internal set; }
    }

    /// <summary>
    /// A concurrency decision of an <see cref="UploadConcurrencyMode.Automatic"/> upload.
    /// </summary>
    public sealed class UploadDecisionTrace : UploadTraceEvent
    {
        internal UploadDecisionTrace(string path, TimeSpan elapsed) : base(path, elapsed)
        {
        }

        /// <summary>What the upload decided.</summary>
        public ConcurrencyDecision Decision { get; internal set; }

        /// <summary>The target before the decision.</summary>
        public int PreviousTarget { get; internal set; }

        /// <summary>The target after the decision.</summary>
        public int Target { get; internal set; }

        /// <summary>
        /// For <see cref="ConcurrencyDecision.WindowNotMeasured"/>, why the upload was below its target; otherwise
        /// <see cref="Transfers.BelowTargetReason.None"/>.
        /// </summary>
        public BelowTargetReason BelowTargetReason { get; internal set; }

        /// <summary>The throughput of the window behind the decision, in bytes per second, or 0 if none was.</summary>
        public double ThroughputBytesPerSecond { get; internal set; }

        /// <summary>The throughput the decision compared it with, in bytes per second, or 0.</summary>
        public double BaselineBytesPerSecond { get; internal set; }

        /// <summary>
        /// The queue estimate of the window behind the decision. For <see cref="ConcurrencyDecision.WarmStartRaised"/> and
        /// <see cref="ConcurrencyDecision.WarmStartSettled"/>, the one the warm start judged by: from the median time the
        /// window's answers took for each byte, against the fastest window judged at the target it was compared with;
        /// otherwise from its time per byte against the lowest the upload has seen (<see cref="UploadConcurrency.QueueEstimate"/>).
        /// </summary>
        public double QueueEstimate { get; internal set; }

        /// <summary>For <see cref="ConcurrencyDecision.CooldownStarted"/>, how long new part requests are paused.</summary>
        public TimeSpan Cooldown { get; internal set; }
    }

    /// <summary>
    /// A part size decision of an upload that chooses its part sizes.
    /// </summary>
    public sealed class UploadSizingTrace : UploadTraceEvent
    {
        internal UploadSizingTrace(string path, TimeSpan elapsed) : base(path, elapsed)
        {
        }

        /// <summary>What the upload decided.</summary>
        public PartSizeDecision Decision { get; internal set; }

        /// <summary>The size before the decision, in bytes, or 0 for the first.</summary>
        public long PreviousSize { get; internal set; }

        /// <summary>The size of the parts read from now on, in bytes, unless a part holds the rest of the file.</summary>
        public long Size { get; internal set; }

        /// <summary>For <see cref="PartSizeDecision.Chosen"/>, what most limited the size.</summary>
        public PartSizeConstraint Constraint { get; internal set; }

        /// <summary>
        /// The throughput of the window behind the decision, in bytes per second, or 0 if none was. For a file of a folder
        /// upload whose files share one target, it is what that file's own part requests delivered per second of their
        /// own time.
        /// </summary>
        public double ThroughputBytesPerSecond { get; internal set; }

        /// <summary>The throughput the decision compared it with, in bytes per second, or 0.</summary>
        public double BaselineBytesPerSecond { get; internal set; }
    }
}