namespace FilesCom.Transfers
{
    /// <summary>
    /// Settings for <see cref="TransferOperations.DownloadDirectoryAsync"/>.
    /// </summary>
    public sealed class DirectoryDownloadOptions : DirectoryTransferOptions
    {
        /// <summary>
        /// The settings each file's download runs with, such as its concurrency, part size and request options; null for
        /// the defaults. Its <see cref="DownloadOptions.Limiter"/> and <see cref="DownloadOptions.CheckpointPath"/> must
        /// not be set: the transfer's <see cref="DirectoryTransferOptions.Limiter"/> and
        /// <see cref="DirectoryTransferOptions.CheckpointDirectory"/> apply. Its <see cref="DownloadOptions.Progress"/>, if
        /// set, receives the progress of every file, each report naming its file. Its
        /// <see cref="DownloadOptions.PublicationPolicy"/> and <see cref="DownloadOptions.CheckpointRestartPolicy"/> apply
        /// to every file.
        /// </summary>
        public DownloadOptions FileOptions { get; set; }

        // These options, and each file's, checked and copied for one transfer.
        internal DirectorySettings Snapshot()
        {
            DownloadOptions template = FileOptions ?? new DownloadOptions();
            if (template.Limiter != null || template.CheckpointPath != null)
            {
                throw InvalidParameterException.For(nameof(FileOptions), "A directory download's FileOptions cannot have a Limiter or CheckpointPath: set DirectoryTransferOptions.Limiter and CheckpointDirectory instead");
            }
            DownloadOptions file = template.Snapshot();
            return SnapshotShared(null, file, file.RequestOptions);
        }
    }
}