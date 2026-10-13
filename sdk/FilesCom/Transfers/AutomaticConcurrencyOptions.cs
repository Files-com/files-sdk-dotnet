using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// How an upload decides how many parts to send at the same time.
    /// </summary>
    public enum UploadConcurrencyMode
    {
        /// <summary>
        /// Starts at <see cref="AutomaticConcurrencyOptions.StartTarget"/> parts at once (after a warm start for an upload,
        /// or a directory upload's files together, straight to Amazon S3 without tuning of its own, unless its
        /// <see cref="UploadOptions.Startup"/> is <see cref="UploadStartup.Aggressive"/>; see
        /// <see cref="UploadOptions.AutomaticConcurrency"/>) and
        /// adjusts that number to what the upload measures: it adds parts while they raise throughput, and sends fewer
        /// after failures, throttling or rising latency. It stays within <see cref="UploadOptions.MaxConcurrency"/>. The
        /// default.
        /// </summary>
        Automatic,

        /// <summary>
        /// Sends <see cref="UploadOptions.MaxConcurrency"/> parts at once whenever the server allows parallel parts,
        /// whatever the upload measures. Use it when an integration needs the same behavior every time.
        /// </summary>
        Static,
    }

    /// <summary>
    /// The kind of destination whose performance the <see cref="AutomaticConcurrencyOptions"/> defaults are tuned for.
    /// </summary>
    /// <remarks>
    /// A profile is only a performance preference. It changes how many parts are sent at once, never the part sizes,
    /// addresses or other instructions Files.com gives for sending them, and it never lets parts be sent in parallel
    /// when Files.com says they must be sent one at a time. An automatic upload without
    /// <see cref="UploadOptions.AutomaticConcurrency"/> of its own uses <see cref="S3"/>'s defaults when its parts go
    /// straight to Amazon S3 as an ordinary multipart upload, as their URLs show when it starts, and <see cref="Generic"/>'s
    /// otherwise. <see cref="Agent"/>'s defaults are used only when chosen, since nothing Files.com sends identifies an
    /// Agent destination.
    /// </remarks>
    public enum UploadPerformanceProfile
    {
        /// <summary>Any destination: favors stability.</summary>
        Generic,

        /// <summary>
        /// Amazon S3 and S3-compatible storage: starts with more parts at once and can go well beyond its soft
        /// ceiling once the upload has moved enough data fast enough to justify more connections.
        /// </summary>
        S3,

        /// <summary>
        /// Files.com Agent and Integration Worker destinations, which have their own capacity limits and throttling:
        /// starts with fewer parts at once and stays lower. Chosen when Files.com supplies direct connection information
        /// for the upload, before any other destination.
        /// </summary>
        Agent,
    }

    /// <summary>
    /// Tuning for <see cref="UploadConcurrencyMode.Automatic"/> uploads and <see cref="DownloadConcurrencyMode.Automatic"/>
    /// downloads, for rollout and diagnostics. The defaults suit most transfers; start from
    /// <see cref="For(UploadPerformanceProfile)"/> and change only what you need.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <em>target</em> is the number of part requests the upload may have in flight at once. The upload measures
    /// its successful part requests in windows at one target: each has at least <see cref="WindowAttempts"/>
    /// completions and at least one per request of the target, and lasts at least as long as its requests took on
    /// average. A window's throughput is the bytes those requests delivered divided by the window's elapsed time,
    /// and its latency is the time the requests took per byte sent, which makes parts of different sizes comparable. A window counts only
    /// when the upload actually kept its target in flight for most of it: time spent waiting for its
    /// <see cref="TransferLimiter"/>, waiting for the next part to be read, or sending parts one at a time says
    /// nothing about what more connections would achieve.
    /// </para>
    /// <para>
    /// After <see cref="GrowthIntervalWindows"/> such windows the target rises by <see cref="GrowthStep"/>, up to
    /// <see cref="SoftCeiling"/> and, once the upload has delivered <see cref="SoftCeilingUnlockBytes"/> and a window
    /// has measured <see cref="SoftCeilingUnlockBytesPerSecond"/>, up to <see cref="MaxTarget"/>. Each rise is a
    /// probe: within <see cref="ProbeWindows"/> windows it must beat the throughput of the target it came from by
    /// <see cref="ProbeGainPercent"/>, and above <see cref="PlateauTarget"/> by
    /// <see cref="ProbeGainPerAddedPartPercent"/> for each part it added past the plateau. Otherwise the target goes
    /// back and growth waits <see cref="HoldWindows"/> windows before probing again. Growth also waits while the
    /// latency shows more than <see cref="LatencyGrowthQueueLimit"/> requests queued, and the target falls by
    /// <see cref="ShrinkPercent"/> when more than <see cref="LatencyBackoffQueueLimit"/> are queued and throughput has
    /// dropped.
    /// </para>
    /// <para>
    /// A throttling answer (HTTP 429 or 503, or any failed part answered with <c>Retry-After</c>) stops the upload
    /// from starting part requests, including retries, for the whole <c>Retry-After</c> time or
    /// <see cref="BackpressurePause"/>, whichever is longer, and lowers the target by
    /// <see cref="BackpressureShrinkPercent"/>. Other temporary failures lower it by <see cref="ShrinkPercent"/>.
    /// Requests already in flight when the target was lowered do not lower it again, so one burst of failures counts
    /// once, and later windows let it grow again. The target never goes below <see cref="MinTarget"/>.
    /// </para>
    /// <para>
    /// The defaults are provisional: they have been checked for safe behavior, not yet tuned against real S3 or
    /// Agent destinations, and may change. An upload copies these settings when it starts.
    /// </para>
    /// </remarks>
    public sealed class AutomaticConcurrencyOptions
    {
        // The longest BackpressurePause allowed. A server's Retry-After can ask for longer, and is honored in full.
        internal static readonly TimeSpan MaxBackpressurePause = TimeSpan.FromMinutes(1);

        // The most parts any upload may send at once.
        internal const int MaxParts = 1024;

        /// <summary>Makes a set of <see cref="UploadPerformanceProfile.Generic"/> defaults.</summary>
        public AutomaticConcurrencyOptions() : this(UploadPerformanceProfile.Generic)
        {
        }

        private AutomaticConcurrencyOptions(UploadPerformanceProfile profile)
        {
            Profile = profile;
            switch (profile)
            {
                case UploadPerformanceProfile.S3:
                    StartTarget = 8;
                    MinTarget = 2;
                    SoftCeiling = 16;
                    MaxTarget = 64;
                    SoftCeilingUnlockBytes = 1L << 30;
                    SoftCeilingUnlockBytesPerSecond = 32 * 1024 * 1024;
                    WindowAttempts = 8;
                    GrowthIntervalWindows = 1;
                    GrowthStep = 2;
                    ProbeGainPercent = 5;
                    PlateauTarget = 16;
                    ProbeGainPerAddedPartPercent = 2;
                    ShrinkPercent = 25;
                    HoldWindows = 1;
                    LatencyGrowthQueueLimit = 4;
                    LatencyBackoffQueueLimit = 8;
                    BackpressureShrinkPercent = 25;
                    break;
                case UploadPerformanceProfile.Agent:
                    StartTarget = 2;
                    MinTarget = 1;
                    SoftCeiling = 4;
                    MaxTarget = 8;
                    SoftCeilingUnlockBytes = 1L << 30;
                    SoftCeilingUnlockBytesPerSecond = 16 * 1024 * 1024;
                    WindowAttempts = 4;
                    GrowthIntervalWindows = 2;
                    GrowthStep = 1;
                    ProbeGainPercent = 10;
                    PlateauTarget = 4;
                    ProbeGainPerAddedPartPercent = 5;
                    ShrinkPercent = 50;
                    HoldWindows = 3;
                    LatencyGrowthQueueLimit = 1;
                    LatencyBackoffQueueLimit = 2;
                    BackpressureShrinkPercent = 50;
                    BackpressurePause = TimeSpan.FromSeconds(2);
                    break;
                default:
                    StartTarget = 4;
                    MinTarget = 1;
                    SoftCeiling = 8;
                    MaxTarget = 8;
                    SoftCeilingUnlockBytes = 1L << 30;
                    SoftCeilingUnlockBytesPerSecond = 0;
                    WindowAttempts = 4;
                    GrowthIntervalWindows = 2;
                    GrowthStep = 1;
                    ProbeGainPercent = 10;
                    PlateauTarget = 8;
                    ProbeGainPerAddedPartPercent = 5;
                    ShrinkPercent = 50;
                    HoldWindows = 2;
                    LatencyGrowthQueueLimit = 2;
                    LatencyBackoffQueueLimit = 4;
                    BackpressureShrinkPercent = 50;
                    break;
            }
            ProbeWindows = 2;
            if (BackpressurePause == TimeSpan.Zero)
            {
                BackpressurePause = TimeSpan.FromSeconds(1);
            }
        }

        /// <summary>
        /// Makes a set of defaults for <paramref name="profile"/>, which you can change before using it.
        /// </summary>
        /// <param name="profile">The kind of destination to tune for.</param>
        /// <returns>A new set of settings.</returns>
        /// <remarks>
        /// <list type="table">
        /// <listheader><term>Setting</term><description>Generic / S3 / Agent</description></listheader>
        /// <item><term>StartTarget, MinTarget</term><description>4, 1 / 8, 2 / 2, 1</description></item>
        /// <item><term>SoftCeiling, MaxTarget</term><description>8, 8 / 16, 64 / 4, 8</description></item>
        /// <item><term>SoftCeilingUnlockBytes, SoftCeilingUnlockBytesPerSecond</term><description>1 GiB, 0 / 1 GiB, 32 MiB/s / 1 GiB, 16 MiB/s</description></item>
        /// <item><term>WindowAttempts, GrowthIntervalWindows, GrowthStep</term><description>4, 2, 1 / 8, 1, 2 / 4, 2, 1</description></item>
        /// <item><term>ProbeGainPercent, PlateauTarget, ProbeGainPerAddedPartPercent, ProbeWindows</term><description>10, 8, 5, 2 / 5, 16, 2, 2 / 10, 4, 5, 2</description></item>
        /// <item><term>ShrinkPercent, HoldWindows</term><description>50, 2 / 25, 1 / 50, 3</description></item>
        /// <item><term>LatencyGrowthQueueLimit, LatencyBackoffQueueLimit</term><description>2, 4 / 4, 8 / 1, 2</description></item>
        /// <item><term>BackpressureShrinkPercent, BackpressurePause</term><description>50, 1 s / 25, 1 s / 50, 2 s</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidParameterException"><paramref name="profile"/> is not a defined profile.</exception>
        public static AutomaticConcurrencyOptions For(UploadPerformanceProfile profile)
        {
            if (profile != UploadPerformanceProfile.Generic && profile != UploadPerformanceProfile.S3 && profile != UploadPerformanceProfile.Agent)
            {
                throw InvalidParameterException.OutOfRange(nameof(profile), profile, "Unknown performance profile");
            }
            return new AutomaticConcurrencyOptions(profile);
        }

        /// <summary>
        /// Makes a scaled preset for Amazon S3 that starts high and probes far: the policy the Files.com Go SDK uses for
        /// S3 uploads and downloads, as these settings express it. An automatic upload whose parts go straight to Amazon
        /// S3 as an ordinary multipart upload, and an automatic download bound to Amazon S3, use it unless
        /// <see cref="UploadOptions.AutomaticConcurrency"/> or <see cref="DownloadOptions.AutomaticConcurrency"/> is
        /// set. Give it there to use it elsewhere, and <see cref="For(UploadPerformanceProfile)"/> with
        /// <see cref="UploadPerformanceProfile.S3"/> for the conservative S3 preset instead.
        /// </summary>
        /// <returns>A new set of settings, which you can change before using it.</returns>
        /// <remarks>
        /// <para>
        /// It starts at 150 parts at once, which is also its soft ceiling, and goes up to 1024 only after 64 GiB at 96 MiB/s.
        /// An upload, or a directory upload's files together, that takes it by default, without
        /// <see cref="UploadOptions.AutomaticConcurrency"/>, first warm-starts at 1 part at a time and adds half again as many parts
        /// when a probe pays, or one after resting, until its answered parts show a fast link or reach
        /// the start target, as <see cref="UploadOptions.AutomaticConcurrency"/> describes, including when failures may
        /// lower it below 8; what follows applies from then on. With <see cref="UploadOptions.Startup"/>
        /// <see cref="UploadStartup.Aggressive"/>, it starts at 150 as when it is given.
        /// It measures windows of at least 32 parts and grows by 4 when a window allows, keeping a rise that gains 1%, or
        /// 0.15% for each part added above 200. A temporary failure lowers the target by 35%, down to 8; latency lowers it
        /// by 8%, down to 50, once 160 parts are estimated queued (96 stop it growing); throttling lowers it by 10%, pausing
        /// for the server's <c>Retry-After</c> alone.
        /// </para>
        /// <para>
        /// Its probing is the Go SDK's. A rise that does not gain lowers the target by 8% from the raised target, down
        /// to 50, but never leaves it above where the rise began. Once a measured window has reached 96 MiB/s the link is
        /// fast: rises up to 150 need no gain, a latency backoff does not take the target below 150, and that lowering goes
        /// below 150 only back to where a rise from below it began. Above an unlocked soft ceiling, up to 200, a rise that
        /// loses no more than 2% is kept.
        /// </para>
        /// <para>
        /// It needs as many request slots as it targets: a <see cref="TransferLimiter"/> with fewer slots, or less buffer
        /// space for the parts in flight, is the limit instead, as <see cref="DownloadConcurrency.Limit"/> reports.
        /// </para>
        /// </remarks>
        public static AutomaticConcurrencyOptions ScaledS3()
        {
            return new AutomaticConcurrencyOptions(UploadPerformanceProfile.S3)
            {
                StartTarget = 150,
                MinTarget = 8,
                SoftCeiling = 150,
                MaxTarget = MaxParts,
                SoftCeilingUnlockBytes = 64L << 30,
                SoftCeilingUnlockBytesPerSecond = 96 * 1024 * 1024,
                WindowAttempts = 32,
                GrowthIntervalWindows = 1,
                GrowthStep = 4,
                ProbeGainPercent = 1,
                PlateauTarget = 200,
                ProbeGainPerAddedPartPercent = 0.15,
                ProbeWindows = 2,
                ShrinkPercent = 35,
                HoldWindows = 1,
                LatencyGrowthQueueLimit = 96,
                LatencyBackoffQueueLimit = 160,
                LatencyShrinkPercent = 8,
                LatencyFloorTarget = 50,
                ProbeFloorTarget = 150,
                ProbeFloorBytesPerSecond = 96 * 1024 * 1024,
                ProbeLossTolerancePercent = 2,
                ProbeRejectionShrinkPercent = 8,
                ThroughputFloorTarget = 50,
                BackpressureShrinkPercent = 10,
                BackpressurePause = TimeSpan.Zero,
            };
        }

        /// <summary>The profile whose defaults these settings started from. Reported in progress.</summary>
        public UploadPerformanceProfile Profile { get; }

        /// <summary>The target an upload starts with, in parts at once.</summary>
        public int StartTarget { get; set; }

        /// <summary>The lowest target failures, throttling or latency can lower it to, in parts at once. At least 1.</summary>
        public int MinTarget { get; set; }

        /// <summary>
        /// The highest target growth reaches until the upload has delivered <see cref="SoftCeilingUnlockBytes"/> and a
        /// window has measured <see cref="SoftCeilingUnlockBytesPerSecond"/>, in parts at once. Equal to
        /// <see cref="MaxTarget"/> for no soft ceiling.
        /// </summary>
        public int SoftCeiling { get; set; }

        /// <summary>
        /// The highest target, in parts at once: from 1 to 1024, and at least <see cref="SoftCeiling"/>.
        /// <see cref="UploadOptions.MaxConcurrency"/>, when set, is also a limit.
        /// </summary>
        public int MaxTarget { get; set; }

        /// <summary>
        /// The bytes of accepted parts an upload must have delivered before its target may grow past
        /// <see cref="SoftCeiling"/>. At least 1. Time alone never unlocks the soft ceiling.
        /// </summary>
        public long SoftCeilingUnlockBytes { get; set; }

        /// <summary>
        /// The throughput, in bytes per second, that a measured window must reach before the target may grow past
        /// <see cref="SoftCeiling"/>. 0 still requires a measured window. Both this and
        /// <see cref="SoftCeilingUnlockBytes"/> are required.
        /// </summary>
        public double SoftCeilingUnlockBytesPerSecond { get; set; }

        /// <summary>
        /// The fewest successful part requests in a measurement window, at least 1. A window also has at least one per
        /// request of the target, and lasts at least their mean duration.
        /// </summary>
        public int WindowAttempts { get; set; }

        /// <summary>The number of measured windows at a target before it grows. At least 1.</summary>
        public int GrowthIntervalWindows { get; set; }

        /// <summary>The number of parts at once each growth adds. At least 1.</summary>
        public int GrowthStep { get; set; }

        /// <summary>
        /// The throughput gain, in percent, a raised target must show over the target it came from to be kept.
        /// </summary>
        public double ProbeGainPercent { get; set; }

        /// <summary>
        /// The target above which each added part at once must earn <see cref="ProbeGainPerAddedPartPercent"/>, in
        /// parts at once: from <see cref="MinTarget"/> to <see cref="MaxTarget"/>.
        /// </summary>
        public int PlateauTarget { get; set; }

        /// <summary>
        /// The throughput gain, in percent, each part at once added above <see cref="PlateauTarget"/> must earn. A
        /// probe from 16 to 18 parts above a plateau of 16 must gain twice this, or <see cref="ProbeGainPercent"/> if
        /// that is more.
        /// </summary>
        public double ProbeGainPerAddedPartPercent { get; set; }

        /// <summary>
        /// The number of measured windows a raised target gets to show its gain before it is withdrawn. At least 1.
        /// </summary>
        public int ProbeWindows { get; set; }

        /// <summary>
        /// How much, in percent, a temporary failure (a connection failure or timeout, 5xx other than 501, or 408) or a
        /// latency backoff lowers the target: from 0 to 100. The target falls by at least one part when this is above 0.
        /// </summary>
        public double ShrinkPercent { get; set; }

        /// <summary>The number of measured windows growth waits after the target was lowered or a probe withdrawn.</summary>
        public int HoldWindows { get; set; }

        /// <summary>
        /// The estimated number of part requests queued, rather than moving data, above which growth waits. The
        /// estimate compares each window's latency per byte with the lowest the upload has seen. 0 turns this off.
        /// </summary>
        public double LatencyGrowthQueueLimit { get; set; }

        /// <summary>
        /// The estimated number of part requests queued above which, when throughput has also fallen, the target is
        /// lowered by <see cref="ShrinkPercent"/>. At least <see cref="LatencyGrowthQueueLimit"/>; 0 turns this off.
        /// </summary>
        public double LatencyBackoffQueueLimit { get; set; }

        /// <summary>
        /// How much, in percent, a latency backoff lowers the target: from 0 to 100. If null, <see cref="ShrinkPercent"/>,
        /// which temporary failures use.
        /// </summary>
        public double? LatencyShrinkPercent { get; set; }

        /// <summary>
        /// The lowest target a latency backoff can lower it to: from <see cref="MinTarget"/> to <see cref="MaxTarget"/>.
        /// Failures and throttling can still lower it to <see cref="MinTarget"/>. If null, <see cref="MinTarget"/>.
        /// </summary>
        public int? LatencyFloorTarget { get; set; }

        /// <summary>
        /// The fast-link probe floor, from <see cref="MinTarget"/> to <see cref="MaxTarget"/>, or null for none. Once a
        /// measured window has reached <see cref="ProbeFloorBytesPerSecond"/>, rises up to this target need no gain, and a
        /// latency backoff does not lower the target below it; a rejected probe lowers it below only back to where that
        /// probe began. Failures and throttling still can.
        /// </summary>
        public int? ProbeFloorTarget { get; set; }

        /// <summary>
        /// The throughput, in bytes per second, a measured window must reach for <see cref="ProbeFloorTarget"/> to apply;
        /// above 0 when that is set.
        /// </summary>
        public double ProbeFloorBytesPerSecond { get; set; }

        /// <summary>
        /// The loss, in percent from 0 to 100, a rise above an unlocked <see cref="SoftCeiling"/>, up to
        /// <see cref="PlateauTarget"/>, may show and still be kept (<see cref="ConcurrencyDecision.ProbeKeptWithinLossTolerance"/>);
        /// 0 keeps only rises that gain.
        /// </summary>
        public double ProbeLossTolerancePercent { get; set; }

        /// <summary>
        /// How much, in percent from 0 to 100, a rise that did not gain lowers the target from the raised target, down to
        /// <see cref="ThroughputFloorTarget"/>, but never leaving it above where it was before the rise; if null, the target
        /// goes back to where it was before the rise.
        /// </summary>
        public double? ProbeRejectionShrinkPercent { get; set; }

        /// <summary>
        /// The lowest target <see cref="ProbeRejectionShrinkPercent"/> lowers it to, from <see cref="MinTarget"/> to
        /// <see cref="MaxTarget"/>. If null, <see cref="MinTarget"/>.
        /// </summary>
        public int? ThroughputFloorTarget { get; set; }

        /// <summary>How much, in percent, a throttling answer lowers the target: from 0 to 100.</summary>
        public double BackpressureShrinkPercent { get; set; }

        /// <summary>
        /// The shortest time a throttling answer stops the upload from starting part requests, including retries:
        /// from zero to one minute. A longer <c>Retry-After</c> is honored instead, in full; a storage 429 or 503
        /// whose <c>Retry-After</c> is longer than the SDK can wait on the platform fails its part rather than being
        /// retried early.
        /// </summary>
        public TimeSpan BackpressurePause { get; set; }

        // A checked copy, so later changes to this object do not affect an upload that has started.
        internal AutomaticConcurrencyOptions Validated()
        {
            AtLeast(nameof(MinTarget), MinTarget, 1);
            AtMost(nameof(MaxTarget), MaxTarget, MaxParts);
            Ordered(nameof(MinTarget), MinTarget, nameof(StartTarget), StartTarget);
            Ordered(nameof(StartTarget), StartTarget, nameof(SoftCeiling), SoftCeiling);
            Ordered(nameof(SoftCeiling), SoftCeiling, nameof(MaxTarget), MaxTarget);
            Ordered(nameof(MinTarget), MinTarget, nameof(PlateauTarget), PlateauTarget);
            Ordered(nameof(PlateauTarget), PlateauTarget, nameof(MaxTarget), MaxTarget);
            if (SoftCeilingUnlockBytes < 1)
            {
                throw InvalidParameterException.OutOfRange(Name(nameof(SoftCeilingUnlockBytes)), SoftCeilingUnlockBytes, "Must be at least 1");
            }
            NonNegative(nameof(SoftCeilingUnlockBytesPerSecond), SoftCeilingUnlockBytesPerSecond, double.MaxValue);
            AtLeast(nameof(WindowAttempts), WindowAttempts, 1);
            AtLeast(nameof(GrowthIntervalWindows), GrowthIntervalWindows, 1);
            AtLeast(nameof(GrowthStep), GrowthStep, 1);
            AtLeast(nameof(ProbeWindows), ProbeWindows, 1);
            AtLeast(nameof(HoldWindows), HoldWindows, 0);
            NonNegative(nameof(ProbeGainPercent), ProbeGainPercent, double.MaxValue);
            NonNegative(nameof(ProbeGainPerAddedPartPercent), ProbeGainPerAddedPartPercent, double.MaxValue);
            NonNegative(nameof(ShrinkPercent), ShrinkPercent, 100);
            NonNegative(nameof(BackpressureShrinkPercent), BackpressureShrinkPercent, 100);
            if (LatencyShrinkPercent.HasValue)
            {
                NonNegative(nameof(LatencyShrinkPercent), LatencyShrinkPercent.Value, 100);
            }
            if (LatencyFloorTarget.HasValue)
            {
                Ordered(nameof(MinTarget), MinTarget, nameof(LatencyFloorTarget), LatencyFloorTarget.Value);
                Ordered(nameof(LatencyFloorTarget), LatencyFloorTarget.Value, nameof(MaxTarget), MaxTarget);
            }
            if (ProbeFloorTarget.HasValue)
            {
                Ordered(nameof(MinTarget), MinTarget, nameof(ProbeFloorTarget), ProbeFloorTarget.Value);
                Ordered(nameof(ProbeFloorTarget), ProbeFloorTarget.Value, nameof(MaxTarget), MaxTarget);
                if (!(ProbeFloorBytesPerSecond > 0) || double.IsInfinity(ProbeFloorBytesPerSecond))
                {
                    throw InvalidParameterException.OutOfRange(Name(nameof(ProbeFloorBytesPerSecond)), ProbeFloorBytesPerSecond, "Must be above 0 when ProbeFloorTarget is set");
                }
            }
            else
            {
                NonNegative(nameof(ProbeFloorBytesPerSecond), ProbeFloorBytesPerSecond, double.MaxValue);
            }
            NonNegative(nameof(ProbeLossTolerancePercent), ProbeLossTolerancePercent, 100);
            if (ProbeRejectionShrinkPercent.HasValue)
            {
                NonNegative(nameof(ProbeRejectionShrinkPercent), ProbeRejectionShrinkPercent.Value, 100);
            }
            if (ThroughputFloorTarget.HasValue)
            {
                Ordered(nameof(MinTarget), MinTarget, nameof(ThroughputFloorTarget), ThroughputFloorTarget.Value);
                Ordered(nameof(ThroughputFloorTarget), ThroughputFloorTarget.Value, nameof(MaxTarget), MaxTarget);
            }
            NonNegative(nameof(LatencyGrowthQueueLimit), LatencyGrowthQueueLimit, double.MaxValue);
            NonNegative(nameof(LatencyBackoffQueueLimit), LatencyBackoffQueueLimit, double.MaxValue);
            if (LatencyGrowthQueueLimit > 0 && LatencyBackoffQueueLimit > 0 && LatencyBackoffQueueLimit < LatencyGrowthQueueLimit)
            {
                throw InvalidParameterException.For(Name(nameof(LatencyBackoffQueueLimit)), $"{Name(nameof(LatencyBackoffQueueLimit))} ({LatencyBackoffQueueLimit}) cannot be below {Name(nameof(LatencyGrowthQueueLimit))} ({LatencyGrowthQueueLimit}): the target would be lowered at a latency that does not yet stop it growing");
            }
            if (BackpressurePause < TimeSpan.Zero || BackpressurePause > MaxBackpressurePause)
            {
                throw InvalidParameterException.OutOfRange(Name(nameof(BackpressurePause)), BackpressurePause, "Must be from zero to one minute");
            }
            return (AutomaticConcurrencyOptions)MemberwiseClone();
        }

        private static string Name(string setting)
        {
            return "AutomaticConcurrency." + setting;
        }

        private static void AtLeast(string setting, int value, int minimum)
        {
            if (value < minimum)
            {
                throw InvalidParameterException.OutOfRange(Name(setting), value, $"Must be at least {minimum}");
            }
        }

        private static void AtMost(string setting, int value, int maximum)
        {
            if (value > maximum)
            {
                throw InvalidParameterException.OutOfRange(Name(setting), value, $"Must be at most {maximum}");
            }
        }

        private static void Ordered(string lowSetting, int low, string highSetting, int high)
        {
            if (low > high)
            {
                throw InvalidParameterException.For(Name(lowSetting), $"{Name(lowSetting)} ({low}) cannot be above {Name(highSetting)} ({high})");
            }
        }

        private static void NonNegative(string setting, double value, double maximum)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > maximum)
            {
                throw InvalidParameterException.OutOfRange(Name(setting), value, maximum == double.MaxValue ? "Must be a finite number, at least 0" : $"Must be from 0 to {maximum}");
            }
        }
    }
}