namespace FilesCom.Transfers
{
    /// <summary>
    /// One file of a completed directory transfer.
    /// </summary>
    public sealed class DirectoryTransferEntry
    {
        internal DirectoryTransferEntry(string relativePath, string localPath, string remotePath, long size, bool completedEarlier)
        {
            RelativePath = relativePath;
            LocalPath = localPath;
            RemotePath = remotePath;
            Size = size;
            CompletedEarlier = completedEarlier;
        }

        /// <summary>The file's path inside the transferred directory, with <c>/</c> between names.</summary>
        public string RelativePath { get; }

        /// <summary>The local file.</summary>
        public string LocalPath { get; }

        /// <summary>The remote file.</summary>
        public string RemotePath { get; }

        /// <summary>The file's bytes.</summary>
        public long Size { get; }

        /// <summary>
        /// Whether an earlier run of the transfer completed the file, which this run did not transfer again, having shown
        /// its source unchanged: for a download, Files.com confirmed the version written is still current, and the local
        /// file it wrote is as it was written; for an upload, the local file still has the bytes that were sent.
        /// </summary>
        public bool CompletedEarlier { get; }
    }
}