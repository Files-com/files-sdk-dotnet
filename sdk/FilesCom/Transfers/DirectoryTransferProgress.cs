using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// The progress of a directory upload or download, as a whole.
    /// </summary>
    /// <remarks>
    /// Bytes done count each file's bytes once, however many times a part of it was sent or received; bytes on the
    /// network count every byte sent or received, including parts sent or fetched again. The speed is of bytes done by
    /// this run, so neither retries nor what an earlier run did inflate it.
    /// </remarks>
    public sealed class DirectoryTransferProgress
    {
        internal DirectoryTransferProgress(string localDirectory, string remoteDirectory, int filesTotal, int filesDone, int filesCompletedEarlier, int filesActive, long bytesTotal, long bytesDone, long networkBytes, double bytesPerSecond, TimeSpan elapsed, int fileLimit = 0)
        {
            LocalDirectory = localDirectory;
            RemoteDirectory = remoteDirectory;
            FilesTotal = filesTotal;
            FilesDone = filesDone;
            FilesCompletedEarlier = filesCompletedEarlier;
            FilesActive = filesActive;
            BytesTotal = bytesTotal;
            BytesDone = bytesDone;
            NetworkBytes = networkBytes;
            BytesPerSecond = bytesPerSecond;
            Elapsed = elapsed;
            FileLimit = fileLimit;
        }

        /// <summary>The local directory, as the caller named it.</summary>
        public string LocalDirectory { get; }

        /// <summary>The remote folder.</summary>
        public string RemoteDirectory { get; }

        /// <summary>The number of files to transfer, including those an earlier run completed.</summary>
        public int FilesTotal { get; }

        /// <summary>The number of files done: completed by this run or an earlier one.</summary>
        public int FilesDone { get; }

        /// <summary>
        /// The number of files an earlier run of the transfer completed, which this run did not transfer again, having shown
        /// their sources unchanged. A file counts once it has been checked, as the transfer reaches it.
        /// </summary>
        public int FilesCompletedEarlier { get; }

        /// <summary>The number of files being transferred now.</summary>
        public int FilesActive { get; }

        /// <summary>The bytes of every file to transfer. A remote file whose size Files.com did not give counts once it is known.</summary>
        public long BytesTotal { get; }

        /// <summary>The bytes done: of files done, and what files in progress have had accepted or written.</summary>
        public long BytesDone { get; }

        /// <summary>The bytes this run sent or received on the network, including parts sent or fetched again.</summary>
        public long NetworkBytes { get; }

        /// <summary>The bytes this run has done per second, since it started.</summary>
        public double BytesPerSecond { get; }

        /// <summary>The time since this run started.</summary>
        public TimeSpan Elapsed { get; }

        /// <summary>
        /// The most files the transfer may have in progress now: <see cref="DirectoryTransferOptions.MaxConcurrentFiles"/>,
        /// or, with automatic file options (uploads or downloads), as many as the target its files learn together allows,
        /// with room for it to grow, and for an upload as many more as it has added while too few of its parts were ready
        /// to send, and as many more again as each raise of the target's warm start, its last included, needs, within that.
        /// </summary>
        public int FileLimit { get; }
    }
}