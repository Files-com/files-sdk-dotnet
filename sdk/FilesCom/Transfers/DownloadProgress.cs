using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A snapshot of a download's progress.
    /// </summary>
    /// <remarks>
    /// <see cref="WrittenBytes"/> counts each byte of the file once, when the part it belongs to has been checked and
    /// written. <see cref="ReceivedBytes"/> counts every byte received, so a part fetched again after a failed attempt
    /// counts again. Divide <see cref="WrittenBytes"/> by <see cref="Elapsed"/> for the rate. Snapshots are cheap: every
    /// value is counted as the download goes, and <see cref="Process"/> is sampled at most once a second.
    /// </remarks>
    public sealed class DownloadProgress
    {
        internal DownloadProgress(
            string path,
            string localPath,
            long totalBytes,
            bool totalBytesKnown,
            bool versionBound,
            long resumedBytes,
            long writtenBytes,
            long receivedBytes,
            long partsWritten,
            int activeParts,
            int retries,
            int restarts,
            TimeSpan requestWait,
            TimeSpan bufferWait,
            TimeSpan elapsed,
            ProcessResourceSample process,
            DownloadConcurrency concurrency = null)
        {
            Path = path;
            LocalPath = localPath;
            TotalBytes = totalBytes;
            TotalBytesKnown = totalBytesKnown;
            VersionBound = versionBound;
            ResumedBytes = resumedBytes;
            WrittenBytes = writtenBytes;
            ReceivedBytes = receivedBytes;
            PartsWritten = partsWritten;
            ActiveParts = activeParts;
            Retries = retries;
            Restarts = restarts;
            RequestWait = requestWait;
            BufferWait = bufferWait;
            Elapsed = elapsed;
            Process = process;
            Concurrency = concurrency;
        }

        /// <summary>The remote path being downloaded.</summary>
        public string Path { get; }

        /// <summary>The local file being written, or null for a download into a stream.</summary>
        public string LocalPath { get; }

        /// <summary>
        /// The size of the file in bytes, once known: as the first answer of a download bound to a version states it, or
        /// from the length the storage server declares. While <see cref="TotalBytesKnown"/> is false, the bytes written so far.
        /// </summary>
        public long TotalBytes { get; }

        /// <summary>
        /// Whether <see cref="TotalBytes"/> is the file's size: false until the download knows it, which, for a file sent
        /// without a length, is once all of it has arrived.
        /// </summary>
        public bool TotalBytesKnown { get; }

        /// <summary>
        /// Whether the download is bound to one version of the file, which lets it fetch parts at once, fetch again only
        /// what failed, and continue later; false when the file comes in one request.
        /// </summary>
        public bool VersionBound { get; }

        /// <summary>The bytes a continued download already had from its checkpoint, included in <see cref="WrittenBytes"/>.</summary>
        public long ResumedBytes { get; }

        /// <summary>The bytes of the file checked and written so far, each counted once.</summary>
        public long WrittenBytes { get; }

        /// <summary>The bytes received from the network, including parts fetched more than once.</summary>
        public long ReceivedBytes { get; }

        /// <summary>The number of parts checked and written.</summary>
        public long PartsWritten { get; }

        /// <summary>The number of parts being fetched now, including parts waiting to be fetched again.</summary>
        public int ActiveParts { get; }

        /// <summary>The number of times a part, or the whole file, was fetched again after a failed attempt.</summary>
        public int Retries { get; }

        /// <summary>
        /// The number of times the download started over from its first byte: a download into a local file without a
        /// bound version, after its request failed partway, or one continued from a checkpoint whose version was gone.
        /// </summary>
        public int Restarts { get; }

        /// <summary>The total time requests spent waiting for a request slot from the limiter.</summary>
        public TimeSpan RequestWait { get; }

        /// <summary>The total time spent waiting for buffer space from the limiter.</summary>
        public TimeSpan BufferWait { get; }

        /// <summary>The time since the download started.</summary>
        public TimeSpan Elapsed { get; }

        /// <summary>
        /// The CPU and memory use of the whole process, which includes much besides this download; null where the
        /// platform does not report it.
        /// </summary>
        public ProcessResourceSample Process { get; }

        /// <summary>
        /// How many parts the download fetches at once, and why; null until it starts fetching parts, and for a file
        /// that comes in one request.
        /// </summary>
        public DownloadConcurrency Concurrency { get; }
    }
}