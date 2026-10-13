using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A snapshot of a transfer's progress.
    /// </summary>
    /// <remarks>
    /// <see cref="AcknowledgedBytes"/> counts each byte once, when the server has accepted the part it belongs to.
    /// <see cref="SentBytes"/> counts every byte written to the network, so a part that is sent again after a
    /// failed attempt counts again. Divide <see cref="AcknowledgedBytes"/> by <see cref="Elapsed"/> for the rate.
    /// Snapshots are cheap: every value is counted as the transfer goes, and <see cref="Process"/> is sampled at most
    /// once a second.
    /// </remarks>
    public sealed class TransferProgress
    {
        internal TransferProgress(
            string path,
            long totalBytes,
            bool totalBytesKnown,
            long spooledBytes,
            long acknowledgedBytes,
            long sentBytes,
            int partsAcknowledged,
            int activeParts,
            int partRetries,
            TimeSpan admissionWait,
            TimeSpan requestWait,
            TimeSpan bufferWait,
            int readyParts,
            int readyLimitWaits,
            TimeSpan elapsed,
            UploadConcurrency concurrency,
            UploadPartSizing partSizing,
            ProcessResourceSample process)
        {
            Path = path;
            TotalBytes = totalBytes;
            TotalBytesKnown = totalBytesKnown;
            SpooledBytes = spooledBytes;
            AcknowledgedBytes = acknowledgedBytes;
            SentBytes = sentBytes;
            PartsAcknowledged = partsAcknowledged;
            ActiveParts = activeParts;
            ConcurrencyLimit = concurrency.Limit;
            PartRetries = partRetries;
            AdmissionWait = admissionWait;
            RequestWait = requestWait;
            BufferWait = bufferWait;
            ReadyParts = readyParts;
            ReadyLimitWaits = readyLimitWaits;
            Elapsed = elapsed;
            Concurrency = concurrency;
            PartSizing = partSizing;
            Process = process;
        }

        /// <summary>The remote path being transferred.</summary>
        public string Path { get; }

        /// <summary>
        /// The size of the transfer in bytes. While <see cref="TotalBytesKnown"/> is false, for a stream of unknown
        /// length, the bytes read from it so far.
        /// </summary>
        public long TotalBytes { get; }

        /// <summary>
        /// Whether <see cref="TotalBytes"/> is the transfer's size: false for a stream of unknown length until it has
        /// ended, and while a stream is being copied to its spool file (<see cref="UploadOptions.SpoolMaxBytes"/>).
        /// </summary>
        public bool TotalBytesKnown { get; }

        /// <summary>
        /// The bytes of a stream copied to its spool file (<see cref="UploadOptions.SpoolMaxBytes"/>) so far, before
        /// the upload is sent; 0 when the stream is not spooled.
        /// </summary>
        public long SpooledBytes { get; }

        /// <summary>The bytes of the parts the server has accepted.</summary>
        public long AcknowledgedBytes { get; }

        /// <summary>The bytes written to the network, including parts sent more than once.</summary>
        public long SentBytes { get; }

        /// <summary>The number of parts the server has accepted.</summary>
        public int PartsAcknowledged { get; }

        /// <summary>
        /// The number of parts started and not yet finished, including parts waiting to be sent again. See
        /// <see cref="UploadConcurrency.ActiveAttempts"/> for the requests being sent now.
        /// </summary>
        public int ActiveParts { get; }

        /// <summary>
        /// The most part requests this transfer allows in flight now: its concurrency target, or 1 when the server
        /// requires parts one at a time. Its limiter may allow fewer at a time. The same as
        /// <see cref="UploadConcurrency.Limit"/>.
        /// </summary>
        public int ConcurrencyLimit { get; }

        /// <summary>The number of times a part was sent again after a failed attempt.</summary>
        public int PartRetries { get; }

        /// <summary>
        /// The total time part requests spent waiting for the transfer's own admission: for its concurrency target
        /// and, after throttling, for its cooldown.
        /// </summary>
        public TimeSpan AdmissionWait { get; }

        /// <summary>The total time part requests spent waiting for a request slot from the limiter.</summary>
        public TimeSpan RequestWait { get; }

        /// <summary>The total time spent waiting for buffer space from the limiter before reading parts.</summary>
        public TimeSpan BufferWait { get; }

        /// <summary>The number of parts read ahead and waiting to be sent.</summary>
        public int ReadyParts { get; }

        /// <summary>
        /// The number of times reading ahead stopped because <see cref="UploadOptions.MaxReadyParts"/> or
        /// <see cref="UploadOptions.MaxReadyBytes"/> was reached.
        /// </summary>
        public int ReadyLimitWaits { get; }

        /// <summary>The time since the transfer started.</summary>
        public TimeSpan Elapsed { get; }

        /// <summary>How the transfer is choosing how many parts to send at once, and why.</summary>
        public UploadConcurrency Concurrency { get; }

        /// <summary>
        /// How the transfer sizes its parts, and within which limits; null until Files.com has answered its first
        /// request, such as while a stream is being copied to its spool file.
        /// </summary>
        public UploadPartSizing PartSizing { get; }

        /// <summary>
        /// The CPU and memory use of the whole process, which includes much besides this transfer; null where the
        /// platform does not report it.
        /// </summary>
        public ProcessResourceSample Process { get; }
    }
}