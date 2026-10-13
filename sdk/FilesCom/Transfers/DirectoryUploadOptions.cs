namespace FilesCom.Transfers
{
    /// <summary>
    /// Settings for <see cref="TransferOperations.UploadDirectoryAsync"/>.
    /// </summary>
    public sealed class DirectoryUploadOptions : DirectoryTransferOptions
    {
        /// <summary>
        /// The settings each file's upload runs with, such as its concurrency, part sizing and request options; null for
        /// the defaults. Its <see cref="UploadOptions.Limiter"/>, <see cref="UploadOptions.CheckpointPath"/> and
        /// <see cref="UploadOptions.ModifiedTime"/> must not be set: the transfer's <see cref="DirectoryTransferOptions.Limiter"/>
        /// and <see cref="DirectoryTransferOptions.CheckpointDirectory"/> apply, and each file is sent with its own
        /// modification time. Missing folders are created unless <see cref="UploadOptions.MkdirParents"/> is false. Its
        /// <see cref="UploadOptions.Progress"/>, if set, receives the progress of every file, each report naming its file.
        /// With <see cref="UploadConcurrencyMode.Automatic"/>, the default, the files' parts share one target, learned from
        /// all of them, and the files start as that target allows, and more, one at a time, while too few of their parts
        /// are ready to send (<see cref="DirectoryTransferOptions.MaxConcurrentFiles"/>). Where the SDK chooses the scaled
        /// S3 policy, that target starts once for all the files as their <see cref="UploadOptions.Startup"/> says: by
        /// default it warm-starts, as <see cref="UploadOptions.AutomaticConcurrency"/> describes; with
        /// <see cref="UploadStartup.Aggressive"/> it starts at the policy's start. A target above the number of files that
        /// may be in progress, such as the scaled S3 policy's 150 with at most 128 files, cannot be filled by files of one
        /// part each, so their parts alone then neither raise it nor lower it for latency; failures and throttling still
        /// lower it. Files that choose their part sizes keep
        /// adjusting them, once the warm start is over, one file's size probe at a time, judged by that file's
        /// own parts; a file that must send its parts one at a time does, and reports it, while the others keep the shared
        /// target; and the shared target's decisions reach <see cref="UploadOptions.Trace"/> once each, under the
        /// directory's path.
        /// </summary>
        public UploadOptions FileOptions { get; set; }

        // These options, and each file's, checked and copied for one transfer.
        internal DirectorySettings Snapshot()
        {
            UploadOptions template = FileOptions ?? new UploadOptions();
            if (template.Limiter != null || template.CheckpointPath != null || template.ModifiedTime != null)
            {
                throw InvalidParameterException.For(nameof(FileOptions), "A directory upload's FileOptions cannot have a Limiter, CheckpointPath or ModifiedTime: set DirectoryTransferOptions.Limiter and CheckpointDirectory instead; each file is sent with its own modification time");
            }
            UploadOptions file = template.Snapshot();
            return SnapshotShared(file, null, file.RequestOptions);
        }
    }
}