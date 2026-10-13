using FilesCom.Models;
using System;
using System.Collections.Generic;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A completed upload.
    /// </summary>
    public sealed class UploadResult
    {
        internal UploadResult(string path, long size, RemoteFile file, IReadOnlyList<UploadedPart> parts, bool parallelParts, TransferProgress finalProgress, int resumedParts = 0, long resumedBytes = 0)
        {
            ResumedParts = resumedParts;
            ResumedBytes = resumedBytes;
            Path = path;
            Size = size;
            File = file;
            Parts = parts;
            ParallelParts = parallelParts;
            SentBytes = finalProgress.SentBytes;
            Elapsed = finalProgress.Elapsed;
            FinalProgress = finalProgress;
        }

        /// <summary>The remote path the upload was made to.</summary>
        public string Path { get; }

        /// <summary>The number of bytes uploaded.</summary>
        public long Size { get; }

        /// <summary>The file, as Files.com described it in response to completing the upload.</summary>
        public RemoteFile File { get; }

        /// <summary>The parts the file was sent in, by part number.</summary>
        public IReadOnlyList<UploadedPart> Parts { get; }

        /// <summary>
        /// Whether the server allowed sending parts in parallel for the whole upload: false if any of its instructions
        /// for a part, including a renewed one, said parts must be sent one at a time.
        /// </summary>
        public bool ParallelParts { get; }

        /// <summary>The bytes written to the network, including parts sent more than once.</summary>
        public long SentBytes { get; }

        /// <summary>The time the upload took.</summary>
        public TimeSpan Elapsed { get; }

        /// <summary>
        /// The upload's progress once it was completed, as a progress handler would receive it: its waits, retries,
        /// and how it chose how many parts to send at once.
        /// </summary>
        public TransferProgress FinalProgress { get; }

        /// <summary>
        /// The number of parts an earlier run of a continued upload had sent, which this run did not send again (their
        /// <see cref="UploadedPart.Attempts"/> are 0); 0 for an upload that was not continued.
        /// </summary>
        public int ResumedParts { get; }

        /// <summary>The bytes of <see cref="ResumedParts"/>.</summary>
        public long ResumedBytes { get; }
    }

    /// <summary>
    /// One part of a completed upload.
    /// </summary>
    public sealed class UploadedPart
    {
        internal UploadedPart(long number, long offset, long length, string etag, int attempts, string sha256 = null)
        {
            Number = number;
            Offset = offset;
            Length = length;
            ETag = etag;
            Attempts = attempts;
            Sha256 = sha256;
        }

        /// <summary>The part number, from 1.</summary>
        public long Number { get; }

        /// <summary>The position of the part's first byte in the file.</summary>
        public long Offset { get; }

        /// <summary>The number of bytes in the part.</summary>
        public long Length { get; }

        /// <summary>
        /// The ETag the server returned when it accepted the part, exactly as sent, or null if it returned none.
        /// Some destinations legitimately return none for parts sent one at a time.
        /// </summary>
        public string ETag { get; }

        /// <summary>
        /// The number of times the part was sent by this run: 1 unless it was retried, and 0 for a part an earlier run of
        /// a continued upload sent.
        /// </summary>
        public int Attempts { get; }

        // The SHA-256 of the bytes sent, in lower-case hexadecimal, when the upload has a checkpoint; null otherwise.
        internal string Sha256 { get; }
    }
}