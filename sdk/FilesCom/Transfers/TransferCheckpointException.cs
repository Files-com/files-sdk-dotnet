using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A transfer's checkpoint file cannot be used: it cannot be read, records a different transfer, records an upload
    /// or download that cannot be continued, was written by a later version of this SDK, or is kept where another user
    /// could have changed it (<see cref="Problem"/> says which). Nothing was transferred.
    /// </summary>
    /// <remarks>
    /// A checkpoint is used only by the transfer it records, so one never continues another's. Give each transfer a
    /// checkpoint path of its own, or delete a checkpoint whose transfer you do not mean to continue. A transfer that
    /// cannot be continued is started over only when <see cref="UploadOptions.CheckpointRestartPolicy"/> or
    /// <see cref="DownloadOptions.CheckpointRestartPolicy"/> says so. A checkpoint is never removed because it cannot be
    /// used: a later version of this SDK's checkpoint stays for that version to continue.
    /// <see cref="Exception.InnerException"/>, when not null, is the failure reading it.
    /// </remarks>
    public sealed class TransferCheckpointException : SdkException
    {
        internal TransferCheckpointException(string checkpointPath, TransferCheckpointProblem problem, string description, Exception failure = null)
            : base($"The checkpoint {checkpointPath} cannot be used: {description}. Nothing was transferred.", failure)
        {
            CheckpointPath = checkpointPath;
            Problem = problem;
        }

        /// <summary>The checkpoint file.</summary>
        public string CheckpointPath { get; }

        /// <summary>Why the checkpoint cannot be used.</summary>
        public TransferCheckpointProblem Problem { get; }
    }
}