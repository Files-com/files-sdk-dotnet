using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// How a download decides how many of its parts to fetch at the same time.
    /// </summary>
    public enum DownloadConcurrencyMode
    {
        /// <summary>
        /// Starts at <see cref="AutomaticConcurrencyOptions.StartTarget"/> parts at once and adjusts that number to what
        /// the download measures, as an automatic upload does: it adds parts while they raise throughput, and fetches fewer
        /// after failures, throttling or rising latency. It stays within <see cref="DownloadOptions.MaxConcurrency"/>. The
        /// default.
        /// </summary>
        Automatic,

        /// <summary>
        /// Fetches <see cref="DownloadOptions.MaxConcurrency"/> parts at once, 4 unless set, whatever the download
        /// measures.
        /// </summary>
        Static,
    }

    /// <summary>
    /// How many parts a download fetches at once, and why, as its <see cref="DownloadProgress"/> reports it.
    /// </summary>
    /// <remarks>
    /// The files of a directory download share one target, learned from all of their parts, so each file's report shows
    /// that shared target and its requests (<see cref="Shared"/>).
    /// </remarks>
    public sealed class DownloadConcurrency
    {
        internal DownloadConcurrency(UploadConcurrency measured, DownloadConcurrencyMode mode, int requestLimit, bool shared)
        {
            Mode = mode;
            Profile = measured.Profile;
            Target = measured.Target;
            MaxTarget = measured.MaxTarget;
            Limit = Math.Min(measured.Target, requestLimit);
            RequestLimit = requestLimit;
            ActiveRequests = measured.ActiveAttempts;
            PeakActiveRequests = measured.PeakActiveAttempts;
            Shared = shared;
            LastDecision = measured.LastDecision;
            Grows = measured.Grows;
            Shrinks = measured.Shrinks;
            Throttles = measured.Throttles;
            TransientFailures = measured.TransientFailures;
            CooldownRemaining = measured.CooldownRemaining;
            LastThroughputBytesPerSecond = measured.LastThroughputBytesPerSecond;
            BestThroughputBytesPerSecond = measured.BestThroughputBytesPerSecond;
            TimeAtTarget = measured.TimeAtTarget;
            TimeWaitingForLimiter = measured.TimeWaitingForLimiter;
            TimeWithoutReadyPart = measured.TimeWithoutReadyPart;
            TimeWritingDestination = measured.TimeWritingDestination;
        }

        /// <summary>Whether the target adapts (<see cref="DownloadConcurrencyMode.Automatic"/>) or is fixed.</summary>
        public DownloadConcurrencyMode Mode { get; }

        /// <summary>The profile whose tuning an automatic download uses.</summary>
        public UploadPerformanceProfile Profile { get; }

        /// <summary>The number of part requests the download may have in flight at once.</summary>
        public int Target { get; }

        /// <summary>The highest the target may reach: <see cref="AutomaticConcurrencyOptions.MaxTarget"/> within
        /// <see cref="DownloadOptions.MaxConcurrency"/>.</summary>
        public int MaxTarget { get; }

        /// <summary>
        /// The most part requests the download can actually have in flight now: the target, within the request slots
        /// of its <see cref="TransferLimiter"/>. Buffer space can lower it further: see <see cref="TimeWithoutReadyPart"/>.
        /// </summary>
        public int Limit { get; }

        /// <summary>The <see cref="TransferLimiter.MaxConcurrentRequests"/> of the download's limiter.</summary>
        public int RequestLimit { get; }

        /// <summary>Part requests in flight.</summary>
        public int ActiveRequests { get; }

        /// <summary>The most part requests that have been in flight at once.</summary>
        public int PeakActiveRequests { get; }

        /// <summary>Whether the target is shared with the other files of a directory download.</summary>
        public bool Shared { get; }

        /// <summary>The last change the download made to its target, or null.</summary>
        public ConcurrencyDecision? LastDecision { get; }

        /// <summary>The number of times the target grew.</summary>
        public int Grows { get; }

        /// <summary>The number of times the target was lowered.</summary>
        public int Shrinks { get; }

        /// <summary>The number of throttling answers (HTTP 429 or 503, or Retry-After on a failure).</summary>
        public int Throttles { get; }

        /// <summary>The number of other temporary failures.</summary>
        public int TransientFailures { get; }

        /// <summary>How long new part requests, retries included, still wait after throttling.</summary>
        public TimeSpan CooldownRemaining { get; }

        /// <summary>The throughput of the last measured window, in bytes per second.</summary>
        public double LastThroughputBytesPerSecond { get; }

        /// <summary>The best throughput of a measured window, in bytes per second.</summary>
        public double BestThroughputBytesPerSecond { get; }

        /// <summary>The time the download had its target in flight.</summary>
        public TimeSpan TimeAtTarget { get; }

        /// <summary>The time admitted parts waited for request slots of the shared <see cref="TransferLimiter"/>.</summary>
        public TimeSpan TimeWaitingForLimiter { get; }

        /// <summary>
        /// The time the download had fewer requests in flight than its target because no part was ready: waiting for
        /// buffer space, for a stream to take parts already fetched, or because no parts remained.
        /// </summary>
        public TimeSpan TimeWithoutReadyPart { get; }

        /// <summary>
        /// The time the download had its target of requests in flight, but fewer of them on the network, because some
        /// were writing what they had received to the local file. It is not time at target: the download learns nothing
        /// about the network from it, neither a gain nor a loss. Wall-clock progress (<see cref="DownloadProgress.Elapsed"/>)
        /// still includes it.
        /// </summary>
        public TimeSpan TimeWritingDestination { get; }
    }
}