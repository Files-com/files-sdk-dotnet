namespace FilesCom.Transfers
{
    /// <summary>
    /// What a file transfer does when its checkpoint records a transfer that cannot be continued
    /// (<see cref="UploadOptions.CheckpointRestartPolicy"/>, <see cref="DownloadOptions.CheckpointRestartPolicy"/>).
    /// </summary>
    public enum CheckpointRestartPolicy
    {
        /// <summary>
        /// Fail with <see cref="TransferCheckpointException"/>, whose <see cref="TransferCheckpointException.Problem"/>
        /// says why, before anything is sent or fetched. The default: starting over replaces the upload the checkpoint
        /// records, or discards what a download has written.
        /// </summary>
        Fail = 0,

        /// <summary>
        /// Discard what the checkpoint records and transfer the file again from its first byte: an upload as a new upload,
        /// whose parts are never completed with parts of the old one; a download into its own stage again, emptied, or a
        /// new one.
        /// </summary>
        StartOver = 1,
    }
}