using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// An upload needs more than a limit allows: one of yours, such as <see cref="UploadOptions.MaxStreamLength"/> or
    /// the limiter's <see cref="TransferLimiter.MaxBufferedBytes"/>, or one of Files.com's, such as the most parts an
    /// upload may have. <see cref="Limit"/> says which; the message says by how much and what to change.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is an <see cref="InvalidOperationException"/>, as these failures always were, so code that catches that
    /// type still catches them. Nothing else an upload throws is an <see cref="UploadLimitException"/>: neither a
    /// failure of your stream, which is thrown as it is, whatever its type, nor any other failure of the SDK.
    /// </para>
    /// <para>
    /// A limit known before the upload starts fails it before anything is sent to storage. One found later, such as a
    /// stream of unknown length that proves too long, or a part Files.com plans for it that is too large to buffer,
    /// stops the upload without sending that part or the excess; parts sent before then are not undone. Either way the
    /// upload is not completed.
    /// </para>
    /// </remarks>
    public sealed class UploadLimitException : InvalidOperationException
    {
        internal UploadLimitException(UploadLimit limit, string message) : base(message)
        {
            Limit = limit;
        }

        /// <summary>The limit the upload needed more than.</summary>
        public UploadLimit Limit { get; }
    }

    /// <summary>The limits an <see cref="UploadLimitException"/> reports.</summary>
    public enum UploadLimit
    {
        /// <summary>
        /// A stream of unknown length gave more than it may hold: <see cref="UploadOptions.MaxStreamLength"/>, or, where
        /// the upload chooses part sizes, the largest file the limits allow. Declare a larger
        /// <see cref="UploadOptions.MaxStreamLength"/>, if the limits allow it, use
        /// <see cref="UploadPartSizingMode.ServerPlanned"/>, or upload the data as a file.
        /// </summary>
        StreamLength,

        /// <summary>
        /// The file or stream needs more parts than the upload may have: with a fixed part size, or with parts held in
        /// memory no larger than the limiter's buffer space allows; or a stream sent as Files.com sizes it gave more than
        /// the parts Files.com makes available hold. The message says which applies.
        /// </summary>
        PartCount,

        /// <summary>
        /// A part must be larger than the limiter may buffer (<see cref="TransferLimiter.MaxBufferedBytes"/>). Allow
        /// more, keep a stream's parts in <see cref="StreamPartStorage.TemporaryFiles"/>, or upload from a file, which
        /// holds no whole parts.
        /// </summary>
        BufferSpace,

        /// <summary>
        /// A part of a stream kept in memory would be larger than a single array can hold. Keep its parts in
        /// <see cref="StreamPartStorage.TemporaryFiles"/>, or upload from a file.
        /// </summary>
        MemoryPart,

        /// <summary>
        /// The stream is longer than <see cref="UploadOptions.SpoolMaxBytes"/>, the most it may copy to its spool file.
        /// Nothing was uploaded.
        /// </summary>
        SpoolLength,

        /// <summary>
        /// The fixed part size (<see cref="UploadPartSizingMode.Fixed"/>, <see cref="UploadOptions.PartSize"/>) cannot
        /// be used: it is outside the part sizes the limits allow, more than the limiter may buffer, would need more
        /// parts than they allow, or would send a file, or a stream's bound, no larger than it as one part larger than
        /// they allow. Choose another size, or automatic sizing.
        /// </summary>
        FixedPartSize,
    }
}