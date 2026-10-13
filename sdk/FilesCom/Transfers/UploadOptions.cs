using FilesCom.Util;
using System;
using System.Collections.Generic;

namespace FilesCom.Transfers
{
    /// <summary>
    /// Where a stream upload keeps each part until the server has accepted it, so that a retry can resend the same
    /// bytes.
    /// </summary>
    public enum StreamPartStorage
    {
        /// <summary>
        /// In memory. The parts held at any time stay within the limiter's <see cref="TransferLimiter.MaxBufferedBytes"/>,
        /// which also limits how large one part can be. The default.
        /// </summary>
        Memory,

        /// <summary>
        /// In a temporary file of its own, written as the part is read, read back a copy buffer at a time for each attempt,
        /// and deleted once the part has been accepted or the upload ends. Memory then no longer limits how large parts can
        /// be, so parts can be as large as the limits allow; the files use disk space for the parts read ahead and being sent.
        /// </summary>
        TemporaryFiles,
    }

    /// <summary>
    /// How an automatic upload starts when the SDK chooses the scaled S3 policy for it
    /// (<see cref="AutomaticConcurrencyOptions.ScaledS3"/>): an ordinary multipart upload straight to Amazon S3 without
    /// <see cref="UploadOptions.AutomaticConcurrency"/> of its own. Other uploads start as their settings say, whichever is
    /// chosen (<see cref="UploadOptions.Startup"/>).
    /// </summary>
    public enum UploadStartup
    {
        /// <summary>
        /// Probes the link first: starts at one part at a time and adds parts only as answered parts show they help, up to
        /// the policy's start, or straight to it once they show a fast link, as <see cref="UploadOptions.AutomaticConcurrency"/>
        /// describes. Can reach the same throughput and concurrency ceiling as Aggressive; only the ramp-up takes
        /// longer. Suits any link, including one that more parts at once would overload. The default.
        /// </summary>
        Default,

        /// <summary>
        /// Starts at the policy's start, 150 parts at once, within <see cref="UploadOptions.MaxConcurrency"/> and the
        /// limiter. Use on a known high-throughput network to skip the initial concurrency probes. The upload still
        /// adapts from there: failures, throttling and
        /// rising latency lower the target, and it grows only as measured windows allow.
        /// </summary>
        Aggressive,
    }

    /// <summary>
    /// Settings for an upload made with <see cref="TransferOperations"/>.
    /// </summary>
    /// <remarks>
    /// An upload copies these settings when it starts, and checks them before it sends anything, so changing them
    /// afterward affects only uploads started later. The same object can be used for several uploads.
    /// </remarks>
    public sealed class UploadOptions
    {
        /// <summary>The number of parts a <see cref="UploadConcurrencyMode.Static"/> upload sends at once unless <see cref="MaxConcurrency"/> is set.</summary>
        public const int DefaultStaticConcurrency = 4;

        /// <summary>The value of <see cref="MaxReadyParts"/> unless another is set.</summary>
        public const int DefaultMaxReadyParts = 2;

        /// <summary>The value of <see cref="MaxReadyBytes"/> unless another is set: 64 MiB.</summary>
        public const long DefaultMaxReadyBytes = 64L * 1024 * 1024;

        /// <summary>
        /// Whether the upload adapts how many parts it sends at once (<see cref="UploadConcurrencyMode.Automatic"/>,
        /// the default) or keeps that number fixed (<see cref="UploadConcurrencyMode.Static"/>). Either way, when the
        /// server does not allow parallel parts, the upload sends one part at a time, in order.
        /// </summary>
        public UploadConcurrencyMode ConcurrencyMode { get; set; } = UploadConcurrencyMode.Automatic;

        /// <summary>
        /// The most parts of this upload to send at the same time, from 1 to 1024. An automatic upload stays within
        /// it, and within its <see cref="AutomaticConcurrencyOptions.MaxTarget"/>; if null, only the latter applies. A
        /// static upload sends this many at once; if null, <see cref="DefaultStaticConcurrency"/>. The limiter's
        /// <see cref="TransferLimiter.MaxConcurrentRequests"/> also applies.
        /// </summary>
        public int? MaxConcurrency { get; set; }

        /// <summary>
        /// Tuning for an automatic upload. If null, the tuning for the destination the upload's negotiated parts show
        /// when it starts: the defaults of <see cref="UploadPerformanceProfile.Agent"/> when Files.com supplies direct
        /// connection information for the upload (a Files.com Agent or Integration Worker destination); the scaled S3
        /// policy, <see cref="AutomaticConcurrencyOptions.ScaledS3"/>, for an ordinary multipart upload straight to
        /// Amazon S3; the defaults of <see cref="UploadPerformanceProfile.Generic"/> otherwise. Tuning given here is
        /// used as it is, whatever the destination, for the whole upload: for example
        /// <see cref="AutomaticConcurrencyOptions.For"/> with <see cref="UploadPerformanceProfile.S3"/> for the
        /// conservative S3 preset.
        /// Settings given here are used as they are, whatever the destination. Ignored by static uploads.
        /// </summary>
        /// <remarks>
        /// An upload using the SDK-selected scaled S3 policy starts with one part, unless <see cref="Startup"/> is
        /// <see cref="UploadStartup.Aggressive"/>, which starts at the policy's start instead. A directory's files share that
        /// startup once (<see cref="DirectoryUploadOptions.FileOptions"/>). A window delivering 96 MiB/s promotes to the
        /// policy's starting target, within <see cref="MaxConcurrency"/>; ordinary adaptive behavior continues there.
        /// Until then, the first window warms the connection and the second establishes a baseline. Judged windows
        /// contain at least two answers and more when response times vary, up to the policy's 32-answer window. Median
        /// time per byte and a reference from judged windows at the compared target reduce misleading latency signals.
        /// The sample-size rule is a heuristic, not a statistical confidence guarantee.
        /// Each successful probe adds half the target, rounded up, or one part after a rest. Its settled window must
        /// show that each added part delivers at least half of what a previous part delivered, and a queue estimate below
        /// half the number added. For example, a raise from 1 to 2 requires 50% more throughput; 2 to 3 requires 25%.
        /// The raised target is judged after its initial in-flight attempts have ended, or after a full policy window
        /// of answers if they have not. A failed probe returns to the previous target and rests for the policy's hold
        /// and growth interval. Queued requests prevent another raise. Failures and throttling keep normal backoff;
        /// during startup the minimum is one, then it becomes the policy's minimum.
        /// A fast-link window still promotes immediately while larger samples are being gathered or during a rest.
        /// Part-size probes during a rest follow <see cref="AutomaticPartSizing"/>; their windows do not advance the
        /// rest, and promotion interrupts a size probe. Directory files begin size probes once their shared startup ends.
        /// <see cref="ConcurrencyDecision.WarmStartRaised"/>, <see cref="ConcurrencyDecision.WarmStartFastLink"/> and
        /// <see cref="ConcurrencyDecision.WarmStartSettled"/> trace startup decisions. Their queue estimates are described
        /// by <see cref="UploadDecisionTrace.QueueEstimate"/>. Directory downloads do not use this startup. Explicit
        /// tuning, including <see cref="AutomaticConcurrencyOptions.ScaledS3"/>, starts at its configured
        /// <see cref="AutomaticConcurrencyOptions.StartTarget"/>.
        /// </remarks>
        public AutomaticConcurrencyOptions AutomaticConcurrency { get; set; }

        /// <summary>
        /// How an automatic upload starts when the SDK chooses the scaled S3 policy for it: <see cref="UploadStartup.Default"/>,
        /// the default, probes the link from one part at a time; <see cref="UploadStartup.Aggressive"/> starts at the policy's
        /// start to skip the initial concurrency probes on a known high-throughput network. Default can reach the same
        /// throughput and concurrency ceiling; only its ramp-up takes longer. Either way the upload then adapts with
        /// the same policy, within <see cref="MaxConcurrency"/> and the
        /// <see cref="Limiter"/>, keeps its retries, backoff and cancellation, and sends one part at a time when the server
        /// requires it; a directory upload's files share one startup and one target (<see cref="DirectoryUploadOptions.FileOptions"/>),
        /// and nothing is kept for later uploads.
        /// </summary>
        /// <remarks>
        /// It chooses nothing else: <see cref="AutomaticConcurrency"/> given here starts at its own
        /// <see cref="AutomaticConcurrencyOptions.StartTarget"/>, a <see cref="UploadConcurrencyMode.Static"/> upload sends its
        /// fixed number, and an upload to an Agent or other destination starts from that profile's defaults, whichever
        /// startup is chosen. The part sizes an upload plans before it sends anything do not depend on it.
        /// </remarks>
        public UploadStartup Startup { get; set; } = UploadStartup.Default;

        /// <summary>
        /// How the upload sizes its parts where it chooses them (see <see cref="UploadPartSizingMode"/>): <see cref="UploadPartSizingMode.Automatic"/>
        /// (the default), <see cref="UploadPartSizingMode.Fixed"/> or <see cref="UploadPartSizingMode.ServerPlanned"/>.
        /// Independent of <see cref="ConcurrencyMode"/>.
        /// </summary>
        public UploadPartSizingMode PartSizingMode { get; set; } = UploadPartSizingMode.Automatic;

        /// <summary>
        /// The size, in bytes, of every part but the last of a <see cref="UploadPartSizingMode.Fixed"/> upload, which
        /// requires it. Must be null in the other modes.
        /// </summary>
        public long? PartSize { get; set; }

        /// <summary>
        /// Tuning for <see cref="UploadPartSizingMode.Automatic"/> part sizes. If null, the defaults of
        /// <see cref="AutomaticPartSizingOptions"/>. Ignored in the other modes.
        /// </summary>
        public AutomaticPartSizingOptions AutomaticPartSizing { get; set; }

        /// <summary>
        /// For a stream uploaded without its length, the most bytes it may give, at least 0. A longer stream fails the
        /// upload, which is not completed. Where the upload chooses part sizes, so does one longer than the largest file
        /// the limits allow (<see cref="PartSizeLimits.MaxFileBytes"/>), and parts are sized so that the smaller of the
        /// two fits in the parts allowed. Ignored by uploads whose length is known.
        /// </summary>
        public long? MaxStreamLength { get; set; }

        /// <summary>
        /// Where a stream upload keeps each part until it is accepted: <see cref="Transfers.StreamPartStorage.Memory"/>,
        /// the default, or <see cref="Transfers.StreamPartStorage.TemporaryFiles"/>. Ignored by file uploads.
        /// </summary>
        public StreamPartStorage StreamPartStorage { get; set; } = StreamPartStorage.Memory;

        /// <summary>
        /// For a stream uploaded without its length, copies the whole stream first to a temporary file of at most this
        /// many bytes, at least 1, and then uploads that file with its length known, like a local file. Nothing is sent to
        /// Files.com until the copy is complete: a longer stream, one that cannot be read, or cancellation meanwhile ends
        /// the upload before it starts. The copy's buffer, of at most 128 KiB, comes from the <see cref="Limiter"/>'s buffer
        /// space when a limiter is given, so the copy waits for its turn there like a part. The file is deleted when the
        /// upload ends. If null, the default, the stream is uploaded as it is read. Ignored by uploads whose length is
        /// known.
        /// </summary>
        public long? SpoolMaxBytes { get; set; }

        /// <summary>
        /// The existing folder for the upload's temporary files: the spool file of <see cref="SpoolMaxBytes"/>, and the
        /// part files of <see cref="Transfers.StreamPartStorage.TemporaryFiles"/>. If null, the user's temporary folder
        /// (<see cref="System.IO.Path.GetTempPath"/>). Each file has a new random name, is created for the upload alone,
        /// and is deleted when the upload no longer needs it. On macOS and Linux it is readable only by its owner from
        /// the moment it exists, whatever the umask; the netstandard2.0 build on a runtime that cannot call the C library
        /// (such as .NET Core 2.x on Linux) cannot make it so, and fails the upload with <see cref="System.IO.IOException"/>
        /// before anything is copied. On Windows it has the folder's permissions.
        /// </summary>
        public string TemporaryDirectory { get; set; }

        /// <summary>
        /// The most parts read ahead and waiting to be sent, at least 1. However high an automatic upload's target
        /// grows, no more than this many parts wait ready.
        /// </summary>
        public int MaxReadyParts { get; set; } = DefaultMaxReadyParts;

        /// <summary>
        /// The most bytes of buffers the parts read ahead may hold, at least 1. A part needing more than this is still
        /// read ahead, once no other part is waiting. Parts keep the buffer space they were given, from the limiter's
        /// <see cref="TransferLimiter.MaxBufferedBytes"/>, until they have been sent for the last time.
        /// </summary>
        public long MaxReadyBytes { get; set; } = DefaultMaxReadyBytes;

        /// <summary>
        /// The limiter whose request slots and buffer space this upload shares with other transfers. If null, the
        /// upload has a limiter of its own that allows as many requests as the upload may send at once and
        /// <see cref="TransferLimiter.DefaultMaxBufferedBytes"/> bytes.
        /// </summary>
        public TransferLimiter Limiter { get; set; }

        /// <summary>
        /// Receives <see cref="TransferProgress"/> reports: when the upload starts sending, and each time the server
        /// accepts a part. Retries are counted in the next report. A stream being copied to its spool file
        /// (<see cref="SpoolMaxBytes"/>) is also reported when the copy starts, after each 16 MiB and when it ends.
        /// </summary>
        /// <remarks>
        /// <see cref="IProgress{T}.Report"/> is called on the upload's own threads, one call at a time. A report
        /// for an accepted part is made after that part has released its limiter resources; other parts of the upload
        /// may still hold theirs, so keep <see cref="IProgress{T}.Report"/> quick. If <see cref="IProgress{T}.Report"/>
        /// itself throws, the upload stops, cancels its parts in flight and fails with that exception.
        /// <see cref="Progress{T}"/> instead posts each report to the context it was created on, such as a UI
        /// thread, and returns at once; an exception from its handler is raised in that context, not in the upload.
        /// No report follows the final request, so completing the upload does not depend on the handler.
        /// </remarks>
        public IProgress<TransferProgress> Progress { get; set; }

        /// <summary>
        /// Receives an <see cref="UploadAttemptTrace"/> for each request that sent a part, an
        /// <see cref="UploadDecisionTrace"/> for each concurrency decision and an <see cref="UploadSizingTrace"/> for
        /// each part size decision, for detailed diagnostics. If null, the default, no records are made.
        /// </summary>
        /// <remarks>
        /// Records are reported like <see cref="Progress"/> reports: on the upload's threads, one call at a time, and
        /// an exception from <see cref="IProgress{T}.Report"/> stops the upload. A request's record is reported before
        /// its part is sent again, so keep the handler quick. Records never include part addresses, headers or
        /// credentials.
        /// </remarks>
        public IProgress<UploadTraceEvent> Trace { get; set; }

        /// <summary>
        /// The modification time to record for the file. If null, a file upload uses the local file's last write
        /// time and a stream upload lets Files.com use the time the upload completes.
        /// </summary>
        public DateTime? ModifiedTime { get; set; }

        /// <summary>
        /// Whether to ask Files.com to create missing parent folders of the destination (the <c>mkdir_parents</c>
        /// parameter). If null, it is not sent, and the site's own setting decides. A site set to always create parent
        /// folders creates them even when this is false.
        /// </summary>
        public bool? MkdirParents { get; set; }

        /// <summary>
        /// Request options for the upload's Files.com API requests, such as <c>api_key</c>, <c>session_id</c> or
        /// <c>workspace_id</c>, as the other client methods take them. The part requests sent to the storage URLs
        /// that Files.com provides never carry Files.com credentials.
        /// </summary>
        public Dictionary<string, object> RequestOptions { get; set; }

        /// <summary>
        /// A file in which a file upload records its progress, so that when it is stopped, by cancellation, a failure or
        /// the process ending, running it again with the same arguments continues it; null for none. Only an upload of
        /// a local file can be continued. See <see cref="TransferOperations.UploadFileAsync"/>.
        /// </summary>
        /// <remarks>
        /// A checkpoint says what the transfer has done, so it is relied on only where no other user can have changed it.
        /// On macOS, and on Linux (x64, Arm64, x86 or 32-bit Arm; glibc or musl; .NET 8 or 10, or the netstandard2.0
        /// build on .NET Core 3.0 or later or on Mono), the checkpoint's directory and every directory above it must belong to this user or to root and be
        /// writable by no one else, unless the directory is sticky and what is below it on the path is this user's or
        /// root's (as <c>/tmp</c>); on macOS, none of them may have an access control list that allows access. On Windows,
        /// the checkpoint's directory must belong to this user, to Administrators or to SYSTEM, and its access control list
        /// must let no one else add, remove or rename what is in it, remove it, or change its access; the directories on
        /// its resolved path are held in place while the checkpoint is used, and a link or junction among them is refused;
        /// this user is the one the calling thread acts as, the impersonated user when it impersonates. Either way, the
        /// checkpoint must be a file of this user's (or, on Windows, of SYSTEM's or Administrators'), not a link, with one
        /// name, that no one else can write; the SDK writes it for this user only (on Windows, and for SYSTEM and
        /// Administrators). Anything else fails with <see cref="TransferCheckpointException"/>
        /// (<see cref="TransferCheckpointProblem.Untrusted"/>) before the checkpoint is read and before anything is sent,
        /// and leaves the checkpoint and what it records as they are. A runtime that cannot make these checks at all (one
        /// that cannot call the C library, such as .NET Core 2.x on Linux; Linux on another processor, such as ppc64le or
        /// s390x; or a system other than Windows, macOS and Linux) refuses every checkpoint that way: run the transfer
        /// there without one. These checks are tested on macOS; on Windows and Linux they follow those systems'
        /// documented interfaces, and have not yet been tested on them.
        /// </remarks>
        public string CheckpointPath { get; set; }

        /// <summary>
        /// What a file upload with a checkpoint does when the upload the checkpoint records cannot be continued: the file
        /// changed, Files.com no longer knows the upload, or it now allows it something else. <see cref="Transfers.CheckpointRestartPolicy.Fail"/>,
        /// the default, fails with <see cref="TransferCheckpointException"/> before any part is sent;
        /// <see cref="Transfers.CheckpointRestartPolicy.StartOver"/> uploads the file again as a new upload. An upload whose
        /// final request the checkpoint records as sent is never started over (<see cref="UploadCompletionUnknownException"/>).
        /// </summary>
        public CheckpointRestartPolicy CheckpointRestartPolicy { get; set; }

        // The concurrency settings the upload runs with. Set on snapshots only; null until Files.com has described the
        // destination, when an automatic upload takes its tuning from that.
        internal ConcurrencySettings Concurrency { get; private set; }

        // The part sizing the upload runs with. Set on snapshots only.
        internal PartSizingSettings Sizing { get; private set; }

        // Set by a directory upload on each file's snapshot: the scopes its automatic files share, so that they learn one
        // target together (TransferScopes).
        internal TransferScopes SharedScopes { get; set; }

        // The settings one upload runs with: a copy, checked before anything is sent.
        internal UploadOptions Snapshot()
        {
            if (MaxConcurrency.HasValue && (MaxConcurrency.Value < 1 || MaxConcurrency.Value > AutomaticConcurrencyOptions.MaxParts))
            {
                throw InvalidParameterException.OutOfRange(nameof(MaxConcurrency), MaxConcurrency, $"An upload must be allowed to send from 1 to {AutomaticConcurrencyOptions.MaxParts} parts at a time");
            }
            if (MaxReadyParts < 1)
            {
                throw InvalidParameterException.OutOfRange(nameof(MaxReadyParts), MaxReadyParts, "An upload must be allowed to read at least one part ahead");
            }
            if (MaxReadyBytes < 1)
            {
                throw InvalidParameterException.OutOfRange(nameof(MaxReadyBytes), MaxReadyBytes, "An upload must be allowed at least one byte for parts read ahead");
            }
            if (MaxStreamLength.HasValue && MaxStreamLength.Value < 0)
            {
                throw InvalidParameterException.OutOfRange(nameof(MaxStreamLength), MaxStreamLength, "Must be at least 0");
            }
            if (SpoolMaxBytes.HasValue && SpoolMaxBytes.Value < 1)
            {
                throw InvalidParameterException.OutOfRange(nameof(SpoolMaxBytes), SpoolMaxBytes, "Must be at least 1");
            }
            if (StreamPartStorage != StreamPartStorage.Memory && StreamPartStorage != StreamPartStorage.TemporaryFiles)
            {
                throw InvalidParameterException.OutOfRange(nameof(StreamPartStorage), StreamPartStorage, "Unknown part storage");
            }
            if (Startup != UploadStartup.Default && Startup != UploadStartup.Aggressive)
            {
                throw InvalidParameterException.OutOfRange(nameof(Startup), Startup, "Unknown startup");
            }
            if (CheckpointPath != null && string.IsNullOrEmpty(System.IO.Path.GetFileName(CheckpointPath)))
            {
                throw InvalidParameterException.For(nameof(CheckpointPath), $"The checkpoint path {CheckpointPath} names no file");
            }
            ConcurrencySettings concurrency;
            switch (ConcurrencyMode)
            {
                case UploadConcurrencyMode.Automatic:
                    // Without tuning of its own, the upload takes the defaults for the destination once Files.com
                    // describes it.
                    concurrency = AutomaticConcurrency != null ? ConcurrencySettings.Automatic(AutomaticConcurrency.Validated(), MaxConcurrency, UploadProfileSource.Options) : null;
                    break;
                case UploadConcurrencyMode.Static:
                    concurrency = ConcurrencySettings.Static(MaxConcurrency ?? DefaultStaticConcurrency);
                    break;
                default:
                    throw InvalidParameterException.OutOfRange(nameof(ConcurrencyMode), ConcurrencyMode, "Unknown concurrency mode");
            }
            PartSizingSettings sizing = PartSizingSettings.Of(PartSizingMode, PartSize, AutomaticPartSizing);
            return new UploadOptions
            {
                ConcurrencyMode = ConcurrencyMode,
                MaxConcurrency = concurrency?.Max ?? MaxConcurrency,
                AutomaticConcurrency = concurrency?.Tuning,
                Startup = Startup,
                PartSizingMode = PartSizingMode,
                PartSize = PartSize,
                // A directory snapshots each file's options again from these: the tuning carries over.
                AutomaticPartSizing = sizing.Tuning,
                MaxReadyParts = MaxReadyParts,
                MaxReadyBytes = MaxReadyBytes,
                Limiter = Limiter,
                Progress = Progress,
                Trace = Trace,
                ModifiedTime = ModifiedTime,
                MkdirParents = MkdirParents,
                RequestOptions = DictionaryUtil.Copy(RequestOptions) ?? new Dictionary<string, object>(),
                MaxStreamLength = MaxStreamLength,
                StreamPartStorage = StreamPartStorage,
                SpoolMaxBytes = SpoolMaxBytes,
                TemporaryDirectory = TemporaryDirectory,
                CheckpointPath = CheckpointPath,
                CheckpointRestartPolicy = CheckpointRestartPolicy,
                Concurrency = concurrency,
                Sizing = sizing,
            };
        }

        // Settles, on a snapshot, what waited for Files.com's description of the destination: an automatic upload
        // without tuning of its own takes the tuning for that destination, with its Startup, and an upload without a
        // limiter gets one that allows as many requests as it may send at once. Settings already made, from the caller's
        // tuning or an earlier description, are kept: the destination is described once, and every later negotiation
        // keeps the tuning the upload started with.
        internal void UseDestination(UploadPerformanceProfile profile)
        {
            if (Concurrency == null)
            {
                Concurrency = ConcurrencySettings.Automatic(DefaultTuningFor(profile).Validated(), MaxConcurrency, UploadProfileSource.Server, Startup);
                AutomaticConcurrency = Concurrency.Tuning;
                MaxConcurrency = Concurrency.Max;
            }
            else if (!Concurrency.Adaptive)
            {
                Concurrency = Concurrency.Described(profile);
            }
            Limiter = Limiter ?? new TransferLimiter(Concurrency.Max, TransferLimiter.DefaultMaxBufferedBytes);
        }

        // The tuning an automatic upload uses for a destination when the caller gives none: for an ordinary multipart
        // upload straight to Amazon S3, the scaled S3 policy (AutomaticConcurrencyOptions.ScaledS3), so that an upload to
        // S3 is fast without the caller naming it; for an Agent or Generic destination, that profile's defaults. The
        // conservative S3 preset stays available as an explicit choice, AutomaticConcurrencyOptions.For(S3).
        internal static AutomaticConcurrencyOptions DefaultTuningFor(UploadPerformanceProfile profile)
        {
            return profile == UploadPerformanceProfile.S3 ? AutomaticConcurrencyOptions.ScaledS3() : AutomaticConcurrencyOptions.For(profile);
        }
    }
}