using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A change an <see cref="UploadConcurrencyMode.Automatic"/> upload made to its target, or a measurement it chose
    /// not to act on.
    /// </summary>
    public enum ConcurrencyDecision
    {
        /// <summary>The target rose by <see cref="AutomaticConcurrencyOptions.GrowthStep"/>, starting a probe.</summary>
        Grew,

        /// <summary>A raised target showed the throughput gain it needed and was kept.</summary>
        ProbeAccepted,

        /// <summary>
        /// A raised target did not show the gain it needed and went back to the target it came from, or, with
        /// <see cref="AutomaticConcurrencyOptions.ProbeRejectionShrinkPercent"/>, below it.
        /// </summary>
        ProbeRejected,

        /// <summary>A temporary failure lowered the target by <see cref="AutomaticConcurrencyOptions.ShrinkPercent"/>.</summary>
        ShrankAfterFailure,

        /// <summary>A throttling answer lowered the target by <see cref="AutomaticConcurrencyOptions.BackpressureShrinkPercent"/>.</summary>
        ShrankAfterThrottling,

        /// <summary>Rising latency and falling throughput lowered the target by <see cref="AutomaticConcurrencyOptions.ShrinkPercent"/>.</summary>
        ShrankForLatency,

        /// <summary>A throttling answer paused new part requests, including retries.</summary>
        CooldownStarted,

        /// <summary>Growth waited because latency showed requests queuing.</summary>
        GrowthHeldForLatency,

        /// <summary>The upload moved enough data fast enough for its target to grow past its soft ceiling.</summary>
        SoftCeilingUnlocked,

        /// <summary>
        /// A window was not used to judge the target, because the upload was below its target for most of it; see
        /// <see cref="UploadDecisionTrace.BelowTargetReason"/>.
        /// </summary>
        WindowNotMeasured,

        /// <summary>
        /// A raised target was kept although it did not show the required gain: its throughput lost no more than
        /// <see cref="AutomaticConcurrencyOptions.ProbeLossTolerancePercent"/>, above an unlocked soft ceiling and up to
        /// the plateau.
        /// </summary>
        ProbeKeptWithinLossTolerance,

        /// <summary>
        /// An upload warm-starting with the scaled S3 policy the SDK chose for it (see
        /// <see cref="UploadOptions.AutomaticConcurrency"/> and <see cref="UploadOptions.Startup"/>) raised its target by half again (one part from 1 and from 2),
        /// or by one part after it rested: after the baseline its start target measured once its connections were warm,
        /// after the parts last added delivered at least half of what each part delivered before them, or after it rested;
        /// and only from a window whose requests were not queued (its <see cref="UploadDecisionTrace.QueueEstimate"/>,
        /// against the fastest window judged at the target compared with, under half the parts added or to add). On
        /// reaching the policy's start target, the warm start is over.
        /// </summary>
        WarmStartRaised,

        /// <summary>
        /// A window of the warm start's answered parts showed a fast link, and the upload went to the policy's start
        /// target, from which the policy carries on as it would have.
        /// </summary>
        WarmStartFastLink,

        /// <summary>
        /// Once the requests under way when they were added had been answered (or the target had had a window of the
        /// policy's size without them), the parts the warm start last added did not deliver half of what each part
        /// delivered before them, or half of them or more were queued (the decision's
        /// <see cref="UploadDecisionTrace.QueueEstimate"/>, against the fastest window judged at the target before the
        /// raise) whatever they delivered, so the target went back to where it was before that raise. The warm start goes
        /// on: it rests there for windows of the policy's size before it tries one part more again.
        /// </summary>
        WarmStartSettled,
    }

    /// <summary>
    /// Why an upload had fewer part requests in flight than its target.
    /// </summary>
    public enum BelowTargetReason
    {
        /// <summary>It was not below its target.</summary>
        None,

        /// <summary>Its requests were waiting for request slots of its <see cref="TransferLimiter"/>, which other transfers were using.</summary>
        SharedLimiter,

        /// <summary>
        /// No part was ready to send: the next part was still being read or waiting for buffer space, parts were
        /// waiting to retry, or none were left.
        /// </summary>
        NoPartReady,

        /// <summary>The server requires the upload's parts to be sent one at a time.</summary>
        SerialParts,

        /// <summary>
        /// A download had its target of requests in flight, but some of them were writing what they had received to
        /// the local destination, so fewer were moving bytes over the network. Uploads never report it.
        /// </summary>
        DestinationWrites,
    }

    /// <summary>
    /// Why an upload sends its parts one at a time, whatever its concurrency settings.
    /// </summary>
    public enum UploadSerialReason
    {
        /// <summary>It does not: the server has allowed parallel parts so far.</summary>
        None,

        /// <summary>The server did not allow parallel parts for the first part.</summary>
        ServerRequiresSerialParts,

        /// <summary>The server withdrew permission to send parts in parallel for a later part or a renewed address.</summary>
        ServerWithdrewParallelParts,

        /// <summary>
        /// The length of the stream is not known, so each part is read, and sent, only once the one before it has been:
        /// only then is it known where the part starts, and whether the stream has ended.
        /// </summary>
        LengthUnknown,
    }

    /// <summary>
    /// How an upload is choosing how many parts to send at once, as part of a <see cref="TransferProgress"/>.
    /// </summary>
    /// <remarks>
    /// Throughput here is goodput: bytes of parts the server accepted, divided by elapsed time, so retried bytes do
    /// not inflate it. The times below target add up how long the upload had fewer requests in flight than its target,
    /// by reason; <see cref="TimeAtTarget"/> is the time it had its target in flight.
    /// </remarks>
    public sealed class UploadConcurrency
    {
        internal UploadConcurrency()
        {
        }

        /// <summary>Whether the upload adapts its concurrency or keeps it fixed.</summary>
        public UploadConcurrencyMode Mode { get; internal set; }

        /// <summary>
        /// The profile whose defaults an automatic upload's tuning came from, as <see cref="ProfileSource"/> says. For a
        /// static upload, the destination its part URLs showed, which changes nothing it does.
        /// </summary>
        public UploadPerformanceProfile Profile { get; internal set; }

        /// <summary>
        /// Whether <see cref="Profile"/> came from the destination the upload's part URLs showed or from
        /// <see cref="UploadOptions.AutomaticConcurrency"/>.
        /// </summary>
        public UploadProfileSource ProfileSource { get; internal set; }

        /// <summary>The number of part requests the upload aims to have in flight at once.</summary>
        public int Target { get; internal set; }

        /// <summary>The number the upload allows in flight now: its target, or 1 while its parts must be sent one at a time.</summary>
        public int Limit { get; internal set; }

        /// <summary>
        /// Whether the target is shared with the other files of a directory upload: its target, attempts, windows and
        /// decisions are then those of all of them together. <see cref="Limit"/> and <see cref="SerialReason"/> are still
        /// this upload's own: 1, with its reason, while it must send its parts one at a time.
        /// </summary>
        public bool Shared { get; internal set; }

        /// <summary>The part requests being sent now.</summary>
        public int ActiveAttempts { get; internal set; }

        /// <summary>The most part requests the upload has had in flight at once.</summary>
        public int PeakActiveAttempts { get; internal set; }

        /// <summary>
        /// The lowest target failures and throttling may lower it to, after <see cref="UploadOptions.MaxConcurrency"/> is
        /// applied. For a warm-starting upload (see <see cref="UploadOptions.AutomaticConcurrency"/>) it is 1 until the warm
        /// start reaches the policy's start target, and the policy's minimum from then on.
        /// </summary>
        public int MinTarget { get; internal set; }

        /// <summary>The soft ceiling, after <see cref="UploadOptions.MaxConcurrency"/> is applied.</summary>
        public int SoftCeiling { get; internal set; }

        /// <summary>The highest target, after <see cref="UploadOptions.MaxConcurrency"/> is applied.</summary>
        public int MaxTarget { get; internal set; }

        /// <summary>Whether the target may grow past <see cref="SoftCeiling"/>.</summary>
        public bool SoftCeilingUnlocked { get; internal set; }

        /// <summary>Why the upload sends parts one at a time, or <see cref="UploadSerialReason.None"/>.</summary>
        public UploadSerialReason SerialReason { get; internal set; }

        /// <summary>The upload's most recent decision, or null if it has made none.</summary>
        public ConcurrencyDecision? LastDecision { get; internal set; }

        /// <summary>The number of times the target grew.</summary>
        public int Grows { get; internal set; }

        /// <summary>The number of times the target was lowered, including withdrawn probes.</summary>
        public int Shrinks { get; internal set; }

        /// <summary>The number of raised targets kept.</summary>
        public int ProbesAccepted { get; internal set; }

        /// <summary>The number of raised targets withdrawn for lack of gain.</summary>
        public int ProbesRejected { get; internal set; }

        /// <summary>The number of windows used to judge the target.</summary>
        public int MeasuredWindows { get; internal set; }

        /// <summary>The number of windows not used because the upload was below its target for most of them.</summary>
        public int UnmeasuredWindows { get; internal set; }

        /// <summary>The number of throttling answers: HTTP 429 or 503, or a failed part answered with <c>Retry-After</c>.</summary>
        public int Throttles { get; internal set; }

        /// <summary>
        /// The number of other temporary failures: connection failures and timeouts, 5xx answers other than 501 and 503,
        /// and 408.
        /// </summary>
        public int TransientFailures { get; internal set; }

        /// <summary>How long new part requests remain paused after throttling; zero when they are not.</summary>
        public TimeSpan CooldownRemaining { get; internal set; }

        /// <summary>The throughput of the most recent window, in bytes per second.</summary>
        public double LastThroughputBytesPerSecond { get; internal set; }

        /// <summary>The highest throughput of any window, in bytes per second.</summary>
        public double BestThroughputBytesPerSecond { get; internal set; }

        /// <summary>The most recent window's latency: the time its part requests took per MiB sent.</summary>
        public TimeSpan LastLatencyPerMebibyte { get; internal set; }

        /// <summary>
        /// The estimated number of part requests queued rather than moving data in the most recent window, from its
        /// latency compared with the lowest the upload has seen.
        /// </summary>
        public double QueueEstimate { get; internal set; }

        /// <summary>The time the upload had its target in flight.</summary>
        public TimeSpan TimeAtTarget { get; internal set; }

        /// <summary>The time the upload was below its target waiting for its <see cref="TransferLimiter"/>'s request slots.</summary>
        public TimeSpan TimeWaitingForLimiter { get; internal set; }

        /// <summary>The time the upload was below its target because no part was ready to send.</summary>
        public TimeSpan TimeWithoutReadyPart { get; internal set; }

        /// <summary>The time the upload was below its target because its parts had to be sent one at a time.</summary>
        public TimeSpan TimeSendingSerially { get; internal set; }

        // A download's time below its target on the network because requests in flight were writing to the destination
        // (BelowTargetReason.DestinationWrites); reported by DownloadConcurrency, always zero for an upload.
        internal TimeSpan TimeWritingDestination { get; set; }
    }
}