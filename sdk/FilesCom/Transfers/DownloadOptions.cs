using System;
using System.Collections.Generic;

namespace FilesCom.Transfers
{
    /// <summary>
    /// Settings for a download with <see cref="TransferOperations"/>. Every property has a default; the object is read
    /// once, when the download starts, so changing it afterwards does not affect a download already running.
    /// </summary>
    public sealed class DownloadOptions
    {
        /// <summary>
        /// The number of parts a <see cref="DownloadConcurrencyMode.Static"/> download fetches at once unless
        /// <see cref="MaxConcurrency"/> is set.
        /// </summary>
        public const int DefaultConcurrency = 4;

        /// <summary>The size of the parts a download fetches unless <see cref="PartSize"/> is set: 8 MiB.</summary>
        public const long DefaultPartSize = 8L * 1024 * 1024;

        /// <summary>The most parts <see cref="MaxConcurrency"/> allows at once.</summary>
        public const int MaxAllowedConcurrency = AutomaticConcurrencyOptions.MaxParts;

        // The request slots of the limiter an automatic download without tuning of its own gets, when it has none: the
        // highest target either profile it may use (S3 or Generic) can reach.
        // The request slots of an automatic download's own limiter when neither MaxConcurrency nor tuning says: enough for
        // the most the storage's tuning may target, the scaled S3 policy's MaxTarget; the target, not the slots, decides how
        // many are used, and the limiter's buffer space bounds the parts in flight.
        private const int AutomaticDefaultRequests = AutomaticConcurrencyOptions.MaxParts;

        /// <summary>
        /// Whether the download adapts how many parts it fetches at once (<see cref="DownloadConcurrencyMode.Automatic"/>,
        /// the default) or keeps that number fixed (<see cref="DownloadConcurrencyMode.Static"/>).
        /// </summary>
        public DownloadConcurrencyMode ConcurrencyMode { get; set; } = DownloadConcurrencyMode.Automatic;

        /// <summary>
        /// The most parts fetched at the same time, from 1 to <see cref="MaxAllowedConcurrency"/>. A static download
        /// fetches this many, or <see cref="DefaultConcurrency"/> if null; an automatic one never targets more, and if
        /// null stays within its <see cref="AutomaticConcurrencyOptions.MaxTarget"/>. The <see cref="Limiter"/>'s
        /// <see cref="TransferLimiter.MaxConcurrentRequests"/> also applies. Parts are fetched at once only when the
        /// download is bound to a version of the file; otherwise the file comes in one request.
        /// </summary>
        public int? MaxConcurrency { get; set; }

        /// <summary>
        /// Tuning for an automatic download. If null, the tuning for the storage the download is bound to: the scaled S3
        /// policy, <see cref="AutomaticConcurrencyOptions.ScaledS3"/>, for Amazon S3, the only storage whose downloads
        /// are fetched in parts, and the defaults of <see cref="UploadPerformanceProfile.Generic"/> otherwise. Tuning
        /// given here is used as it is, whatever the storage, for the whole download:
        /// <see cref="AutomaticConcurrencyOptions.For"/> with <see cref="UploadPerformanceProfile.S3"/> for the
        /// conservative S3 preset. For more parts at once than the limiter allows, give a <see cref="Limiter"/> with
        /// the request slots and buffer space it needs. Only for <see cref="DownloadConcurrencyMode.Automatic"/>.
        /// </summary>
        public AutomaticConcurrencyOptions AutomaticConcurrency { get; set; }

        /// <summary>
        /// The size in bytes of each part (Range) fetched, 1 or more; null for <see cref="DefaultPartSize"/>. The last
        /// part may be smaller. A download into a stream keeps each part in memory until the parts before it have been
        /// written, so its parts are no larger than the <see cref="Limiter"/>'s <see cref="TransferLimiter.MaxBufferedBytes"/>.
        /// </summary>
        public long? PartSize { get; set; }

        /// <summary>
        /// Limits request slots and buffer space, and shares them with the other transfers given the same limiter; null
        /// for one of this download's own, with <see cref="TransferLimiter.DefaultMaxBufferedBytes"/> bytes and as many
        /// request slots as the download may target: <see cref="MaxConcurrency"/>, or for an automatic download without
        /// it, its tuning's <see cref="AutomaticConcurrencyOptions.MaxTarget"/>, or without tuning the most the
        /// storage's own tuning may target, <see cref="MaxAllowedConcurrency"/>; the buffer space bounds the parts in
        /// flight all the same.
        /// </summary>
        public TransferLimiter Limiter { get; set; }

        /// <summary>
        /// Receives the download's progress: once it knows the file's size, as each part is written, at most every
        /// quarter second while a part arrives, and when it ends. Null for none.
        /// </summary>
        public IProgress<DownloadProgress> Progress { get; set; }

        /// <summary>
        /// A file in which the download records its progress, so that when it is stopped, by cancellation, a failure or
        /// the process ending, running it again with the same arguments continues it; null for none. Only a download to
        /// a local file can be continued. See <see cref="TransferOperations.DownloadFileAsync"/>.
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
        /// How the finished file is put in place under its name: <see cref="DownloadPublicationPolicy.ReplaceExisting"/>,
        /// the default, <see cref="DownloadPublicationPolicy.CreateNew"/> or
        /// <see cref="DownloadPublicationPolicy.ReplaceRegularFile"/>. It applies only to a download to a local file: a
        /// download into a stream with another policy fails with <see cref="InvalidParameterException"/>. It is the policy of this
        /// run: a download continued from its checkpoint is put in place as this run's policy says, whatever the run that
        /// wrote the checkpoint was given. A destination the policy refuses is refused before anything is fetched when it
        /// is there already, and again when the file is put in place.
        /// </summary>
        public DownloadPublicationPolicy PublicationPolicy { get; set; }

        /// <summary>
        /// What a download with a checkpoint does when the download the checkpoint records cannot be continued: its stage
        /// holds fewer bytes than it records, or it records what this SDK cannot use.
        /// <see cref="Transfers.CheckpointRestartPolicy.Fail"/>, the default, fails with
        /// <see cref="TransferCheckpointException"/> before anything is fetched, leaving the checkpoint and its stage as they
        /// are; <see cref="Transfers.CheckpointRestartPolicy.StartOver"/> discards them and downloads the file again from its
        /// first byte. Only this download's own state is discarded: a stage shown to be the one it records, and its
        /// checkpoint; never the destination, and never a checkpoint of another transfer, of a later version of this SDK,
        /// or one kept where another user could have changed it, which fail whatever this says. A download whose recorded
        /// version is gone, or whose stage is gone, starts over whatever this says, as it always has.
        /// </summary>
        public CheckpointRestartPolicy CheckpointRestartPolicy { get; set; }

        /// <summary>
        /// Options for the download's requests to the Files.com API, such as <c>workspace_id</c>, as the generated
        /// operations take them. Null for none.
        /// </summary>
        public Dictionary<string, object> RequestOptions { get; set; }

        // A static download's number of parts at once.
        internal int Concurrency { get; private set; }

        // An automatic download's checked tuning, or null for the defaults of the storage it is bound to.
        internal AutomaticConcurrencyOptions Tuning { get; private set; }

        // The request slots of the limiter the download gets when it has none of its own.
        internal int Requests { get; private set; }

        // Set by a directory download: the adaptive scopes its files share, so they learn one target together.
        internal TransferScopes SharedScopes { get; set; }

        internal long PartBytes { get; private set; }

        // Set by a directory download, which records each file it completed, and only then removes the file's checkpoint.
        internal bool AcknowledgedByCaller { get; set; }

        // A copy with defaults filled in and every setting checked, before anything is sent.
        internal DownloadOptions Snapshot()
        {
            int concurrency = MaxConcurrency ?? DefaultConcurrency;
            if (concurrency < 1 || concurrency > MaxAllowedConcurrency)
            {
                throw InvalidParameterException.OutOfRange(nameof(MaxConcurrency), concurrency, $"A download fetches from 1 to {MaxAllowedConcurrency} parts at once");
            }
            AutomaticConcurrencyOptions tuning;
            int requests;
            switch (ConcurrencyMode)
            {
                case DownloadConcurrencyMode.Automatic:
                    tuning = AutomaticConcurrency?.Validated();
                    requests = MaxConcurrency ?? tuning?.MaxTarget ?? AutomaticDefaultRequests;
                    break;
                case DownloadConcurrencyMode.Static:
                    if (AutomaticConcurrency != null)
                    {
                        throw InvalidParameterException.For(nameof(AutomaticConcurrency), "AutomaticConcurrency is for an automatic download; a static download fetches MaxConcurrency parts at once");
                    }
                    tuning = null;
                    requests = concurrency;
                    break;
                default:
                    throw InvalidParameterException.OutOfRange(nameof(ConcurrencyMode), ConcurrencyMode, "Unknown concurrency mode");
            }
            long partSize = PartSize ?? DefaultPartSize;
            if (partSize < 1)
            {
                throw InvalidParameterException.OutOfRange(nameof(PartSize), partSize, "A download's parts are at least 1 byte");
            }
            if (!Enum.IsDefined(typeof(DownloadPublicationPolicy), PublicationPolicy))
            {
                throw InvalidParameterException.OutOfRange(nameof(PublicationPolicy), PublicationPolicy, "A download's publication policy is ReplaceExisting, CreateNew or ReplaceRegularFile");
            }
            if (!Enum.IsDefined(typeof(CheckpointRestartPolicy), CheckpointRestartPolicy))
            {
                throw InvalidParameterException.OutOfRange(nameof(CheckpointRestartPolicy), CheckpointRestartPolicy, "A download's checkpoint restart policy is Fail or StartOver");
            }
            if (CheckpointPath != null && string.IsNullOrEmpty(System.IO.Path.GetFileName(CheckpointPath)))
            {
                throw InvalidParameterException.For(nameof(CheckpointPath), $"The checkpoint path {CheckpointPath} names no file");
            }
            return new DownloadOptions
            {
                ConcurrencyMode = ConcurrencyMode,
                MaxConcurrency = MaxConcurrency,
                AutomaticConcurrency = tuning,
                Tuning = tuning,
                Requests = requests,
                SharedScopes = SharedScopes,
                PartSize = PartSize,
                Limiter = Limiter ?? new TransferLimiter(requests, TransferLimiter.DefaultMaxBufferedBytes),
                Progress = Progress,
                CheckpointPath = CheckpointPath,
                PublicationPolicy = PublicationPolicy,
                CheckpointRestartPolicy = CheckpointRestartPolicy,
                AcknowledgedByCaller = AcknowledgedByCaller,
                RequestOptions = RequestOptions != null ? new Dictionary<string, object>(RequestOptions) : null,
                Concurrency = concurrency,
                PartBytes = partSize,
            };
        }
    }
}