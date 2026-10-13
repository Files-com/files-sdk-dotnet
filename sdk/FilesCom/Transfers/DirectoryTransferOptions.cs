using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// What directory uploads and downloads share: how many files move at once, the limiter every file's requests and
    /// buffers share, a checkpoint directory and progress. See <see cref="DirectoryUploadOptions"/> and
    /// <see cref="DirectoryDownloadOptions"/>.
    /// </summary>
    public abstract class DirectoryTransferOptions
    {
        /// <summary>
        /// The value of <see cref="MaxConcurrentFiles"/> until it is set: the number of files a transfer with static file
        /// options transfers at once.
        /// </summary>
        public const int DefaultMaxConcurrentFiles = 4;

        /// <summary>The most files one directory transfer may transfer at once.</summary>
        public const int MaxAllowedConcurrentFiles = 128;

        // The request slots of the limiter a directory upload with automatic file options gets when it has none: the most
        // any tuning may target (AutomaticConcurrencyOptions.MaxParts), which the scaled S3 policy chosen for files
        // uploaded straight to Amazon S3 reaches. A single file sizes its own limiter once its destination has settled its
        // tuning (UploadOptions.UseDestination); a directory's limiter is made before any of its files negotiates, so it
        // takes the most, as a download's does (DownloadOptions.Requests). The buffer space bounds the parts in flight all
        // the same, and the shared target decides how many of the slots are used.
        private const int AutomaticUploadRequests = AutomaticConcurrencyOptions.MaxParts;

        private int maxConcurrentFiles = DefaultMaxConcurrentFiles;
        private bool maxConcurrentFilesSet;

        private protected DirectoryTransferOptions()
        {
        }

        /// <summary>
        /// How many files are transferred at once, from 1 to <see cref="MaxAllowedConcurrentFiles"/>;
        /// <see cref="DefaultMaxConcurrentFiles"/> until it is set.
        /// </summary>
        /// <remarks>
        /// With static file options (<see cref="UploadConcurrencyMode.Static"/> or <see cref="DownloadConcurrencyMode.Static"/>)
        /// the transfer transfers exactly this many files at once. With automatic file options, the default for both
        /// directions, it starts as many files as the target its files learn together allows, with room for that target
        /// to grow, and an upload more while fewer of its parts are ready to send than that room: a file for each that
        /// ends, and one more each 100 ms, so that files beginning or finishing do not leave the target unfed, and with each
        /// raise of its files' shared warm start (<see cref="UploadOptions.AutomaticConcurrency"/>), its last included, more
        /// at once, as many for each part added as it took for each part before; once this is set never more than this;
        /// until it is set, up to <see cref="MaxAllowedConcurrentFiles"/>.
        /// <see cref="DirectoryTransferProgress.FileLimit"/> reports how many it may have in progress. Files start in the
        /// order of their paths; each sends its own parts within the <see cref="Limiter"/>.
        /// </remarks>
        public int MaxConcurrentFiles
        {
            get => maxConcurrentFiles;
            set
            {
                maxConcurrentFiles = value;
                maxConcurrentFilesSet = true;
            }
        }

        /// <summary>
        /// The limiter every file's part requests and buffers share, which other transfers, of this client or others, may
        /// share too; null for one of this transfer's own, with <see cref="TransferLimiter.DefaultMaxBufferedBytes"/>
        /// bytes and, with automatic file options, as many request slots as one of its files may target (its
        /// <c>MaxConcurrency</c>, or its tuning's <see cref="AutomaticConcurrencyOptions.MaxTarget"/>, or without either
        /// the most any tuning may target, 1024, which the scaled S3 policy chosen for files uploaded straight to Amazon S3
        /// reaches) but never fewer than <see cref="TransferLimiter.DefaultMaxConcurrentRequests"/>, or the default limits
        /// with static file options. The limiter admits waiting requests in turn, so
        /// files, and transfers, sharing it take turns rather than one of them taking it all.
        /// </summary>
        public TransferLimiter Limiter { get; set; }

        /// <summary>
        /// A local directory in which the transfer records its progress, so that when it is stopped, by cancellation, a
        /// failure or the process ending, running it again with the same arguments continues it; null for none. It holds
        /// a record of the files the transfer completed and a checkpoint for each file in progress, and no credentials or
        /// signed URLs. It is created if missing, usable by this user only, and must not be inside the local directory
        /// transferred, by its name or through a link. It is relied on only where no other user can change it, as
        /// <see cref="DownloadOptions.CheckpointPath"/> describes: a transfer refuses it before reading its record, though a
        /// directory download lists the remote folder first, and directories created for it, or for the local directory,
        /// stay.
        /// </summary>
        public string CheckpointDirectory { get; set; }

        /// <summary>
        /// Receives the whole transfer's progress: files and bytes done, bytes on the network and speed. It is called one
        /// report at a time.
        /// </summary>
        public IProgress<DirectoryTransferProgress> Progress { get; set; }

        // The shared settings as they are now, checked, for one transfer: changing these options later does not reach it.
        private protected DirectorySettings SnapshotShared(UploadOptions uploadFile, DownloadOptions downloadFile, System.Collections.Generic.Dictionary<string, object> requestOptions)
        {
            bool learnsFiles = downloadFile != null
                ? downloadFile.ConcurrencyMode == DownloadConcurrencyMode.Automatic
                : uploadFile.ConcurrencyMode == UploadConcurrencyMode.Automatic;
            if (maxConcurrentFiles < 1 || maxConcurrentFiles > MaxAllowedConcurrentFiles)
            {
                throw InvalidParameterException.OutOfRange(nameof(MaxConcurrentFiles), maxConcurrentFiles, $"A directory transfer transfers from 1 to {MaxAllowedConcurrentFiles} files at once");
            }
            // Automatic files are capped by MaxConcurrentFiles once the caller set it, otherwise by the supported most.
            int most = learnsFiles && !maxConcurrentFilesSet ? MaxAllowedConcurrentFiles : maxConcurrentFiles;
            // The limiter is every file's, for requests outside the scope too (a version's confirmation, a whole file): never
            // fewer slots than the default one has, and as many as one file may target when that is more.
            int requests = Math.Max(TransferLimiter.DefaultMaxConcurrentRequests,
                downloadFile != null ? downloadFile.Requests : uploadFile.MaxConcurrency ?? uploadFile.AutomaticConcurrency?.MaxTarget ?? AutomaticUploadRequests);
            TransferLimiter limiter = Limiter ?? (learnsFiles ? new TransferLimiter(requests, TransferLimiter.DefaultMaxBufferedBytes) : new TransferLimiter());
            return new DirectorySettings(most, learnsFiles, limiter, CheckpointDirectory == null ? null : System.IO.Path.GetFullPath(CheckpointDirectory), Progress, uploadFile, downloadFile, requestOptions);
        }
    }

    // One directory transfer's settings, taken from its options once, when it starts.
    internal sealed class DirectorySettings
    {
        internal DirectorySettings(int maxConcurrentFiles, bool learnsFiles, TransferLimiter limiter, string checkpointDirectory, IProgress<DirectoryTransferProgress> progress, UploadOptions uploadFile, DownloadOptions downloadFile, System.Collections.Generic.Dictionary<string, object> requestOptions)
        {
            MaxConcurrentFiles = maxConcurrentFiles;
            LearnsFiles = learnsFiles;
            Limiter = limiter;
            CheckpointDirectory = checkpointDirectory;
            Progress = progress;
            UploadFile = uploadFile;
            DownloadFile = downloadFile;
            RequestOptions = requestOptions;
        }

        internal int MaxConcurrentFiles { get; }

        // Whether files are admitted as the target their transfers learn together allows, within MaxConcurrentFiles: a
        // directory transfer, either way, whose files are automatic.
        internal bool LearnsFiles { get; }

        internal TransferLimiter Limiter { get; }

        // The full path, or null for none.
        internal string CheckpointDirectory { get; }

        internal IProgress<DirectoryTransferProgress> Progress { get; }

        // The checked settings each file's upload, or download, starts from.
        internal UploadOptions UploadFile { get; }

        internal DownloadOptions DownloadFile { get; }

        // The request options of the transfer's own API requests, such as its listing: the files' request options.
        internal System.Collections.Generic.Dictionary<string, object> RequestOptions { get; }
    }
}