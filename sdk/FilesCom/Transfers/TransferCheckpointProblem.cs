namespace FilesCom.Transfers
{
    /// <summary>
    /// Why a checkpoint cannot be used (<see cref="TransferCheckpointException.Problem"/>).
    /// </summary>
    public enum TransferCheckpointProblem
    {
        /// <summary>It cannot be read, or is not a checkpoint this SDK, in this format, wrote.</summary>
        Unreadable = 0,

        /// <summary>It records another transfer: another site, workspace, remote path, local file, or kind of transfer.</summary>
        OtherTransfer = 1,

        /// <summary>
        /// The local file is not the one the upload it records was sending: its length changed, or the bytes of a part
        /// Files.com accepted are no longer the bytes that were sent.
        /// </summary>
        SourceChanged = 2,

        /// <summary>
        /// Files.com no longer knows the upload it records, which expired or was cancelled. It was not completed: its
        /// final request had not been sent.
        /// </summary>
        UploadExpired = 3,

        /// <summary>
        /// Files.com now allows the upload other part limits, part counts or offsets than it started with, or the parts the
        /// checkpoint records, whose sizes the upload chose, are not within the limits it has, as an earlier version of this
        /// SDK could record.
        /// </summary>
        GrantChanged = 4,

        /// <summary>
        /// It is kept where another user could have changed it, so what it says is not relied on: its directory (or, on
        /// macOS and Linux, one above it) belongs to another user or lets one change what is in it, a directory on its path
        /// was replaced by a link, or it is another user's file, a link, a file with another name, or a file other users
        /// can write; or this runtime cannot check who else can change it. See <see cref="DownloadOptions.CheckpointPath"/>.
        /// </summary>
        Untrusted = 5,
    }
}