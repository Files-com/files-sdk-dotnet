using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// Where an upload's part sizes come from.
    /// </summary>
    public enum PartSizeSource
    {
        /// <summary>From Files.com's plan: see <see cref="UploadPartSizing.ServerPlannedReason"/> for why.</summary>
        ServerPlanned,

        /// <summary>Chosen by the upload, as <see cref="UploadPartSizingMode.Automatic"/> describes.</summary>
        Automatic,

        /// <summary><see cref="UploadOptions.PartSize"/>, as <see cref="UploadPartSizingMode.Fixed"/> describes.</summary>
        Fixed,
    }

    /// <summary>
    /// Why an upload uses the part sizes Files.com plans.
    /// </summary>
    public enum ServerPlannedReason
    {
        /// <summary>It does not: the upload chooses its sizes.</summary>
        None,

        /// <summary><see cref="UploadPartSizingMode.ServerPlanned"/> was chosen.</summary>
        Requested,

        /// <summary>
        /// The upload's parts do not go straight to Amazon S3 as an ordinary multipart upload, the only destination the
        /// SDK chooses part sizes for (see <see cref="UploadPartSizingMode"/>), or the file is larger than
        /// <see cref="PartSizeLimits.MaxFileBytes"/> or cannot fit in the parts the limits allow, so it sends the parts
        /// Files.com sizes.
        /// </summary>
        NotAllowed,

        /// <summary>
        /// The length of the stream is not known and its parts do not go where the SDK chooses part sizes, so they are
        /// sent as Files.com plans them, one at a time.
        /// </summary>
        LengthUnknown,

        /// <summary>
        /// The parts of the stream of unknown length go where the SDK chooses part sizes, but parts held in memory could
        /// not be large enough for the most it may hold to fit in the parts the limits allow. Its parts go one at a time,
        /// as Files.com sizes them, within <see cref="UploadOptions.MaxStreamLength"/> and the parts Files.com makes
        /// available.
        /// Declare a smaller <see cref="UploadOptions.MaxStreamLength"/>, or keep parts in
        /// <see cref="StreamPartStorage.TemporaryFiles"/>.
        /// </summary>
        MemoryLimit,
    }

    /// <summary>
    /// What most limited the size an upload chose for its parts.
    /// </summary>
    public enum PartSizeConstraint
    {
        /// <summary>Nothing: the size is the one preferred for the file's size or the workload's.</summary>
        None,

        /// <summary><see cref="AutomaticPartSizingOptions.MinPartSize"/>.</summary>
        MinPartSize,

        /// <summary><see cref="AutomaticPartSizingOptions.MaxPartSize"/>.</summary>
        MaxPartSize,

        /// <summary>The smallest part other than the last that the limits allow (<see cref="PartSizeLimits.MinNonfinalBytes"/>).</summary>
        ServerMinimum,

        /// <summary>The largest part the limits allow (<see cref="PartSizeLimits.MaxPartBytes"/>).</summary>
        ServerMaximum,

        /// <summary>The number of parts allowed: smaller parts would need more.</summary>
        PartLimit,

        /// <summary>The buffer space of the limiter, which holds whole parts of a stream until they are accepted.</summary>
        BufferLimit,
    }

    /// <summary>
    /// A change an upload made to the size of its parts.
    /// </summary>
    public enum PartSizeDecision
    {
        /// <summary>The size the upload started with.</summary>
        Chosen,

        /// <summary>The parts not yet read grew by <see cref="AutomaticPartSizingOptions.GrowthFactor"/>, starting a probe.</summary>
        Grew,

        /// <summary>Larger parts showed the throughput gain they needed and were kept.</summary>
        ProbeAccepted,

        /// <summary>Larger parts did not show the gain they needed, or were interrupted, and the size went back.</summary>
        ProbeRejected,
    }

    /// <summary>
    /// The limits within which an upload chooses the sizes of its parts. For an ordinary multipart upload straight to
    /// Amazon S3 they are the SDK's planning limits: parts other than the last of at least 5 MiB, parts of at most 1 GiB,
    /// at most 10,000 parts (or fewer, when Files.com's <c>available_parts</c> says so) and files of at most 5 TiB. They
    /// keep every upload within S3's own rules; they are not a promise that Files.com, or the destination, accepts any
    /// file within them.
    /// </summary>
    public sealed class PartSizeLimits
    {
        internal PartSizeLimits(long minNonfinalBytes, long maxPartBytes, long maxParts, long maxFileBytes)
        {
            MinNonfinalBytes = minNonfinalBytes;
            MaxPartBytes = maxPartBytes;
            MaxParts = maxParts;
            MaxFileBytes = maxFileBytes;
        }

        /// <summary>The smallest size in bytes of every part except the last.</summary>
        public long MinNonfinalBytes { get; }

        /// <summary>The largest size in bytes of any part.</summary>
        public long MaxPartBytes { get; }

        /// <summary>The most parts.</summary>
        public long MaxParts { get; }

        /// <summary>The largest file, in bytes.</summary>
        public long MaxFileBytes { get; }

        // The most parts an upload may have within these limits: MaxParts, or fewer when Files.com's available_parts says
        // so.
        internal long PartsAllowed(long? availableParts)
        {
            return availableParts.HasValue && availableParts.Value >= 1 && availableParts.Value < MaxParts ? availableParts.Value : MaxParts;
        }

        internal bool SameAs(PartSizeLimits other)
        {
            return other != null && other.MinNonfinalBytes == MinNonfinalBytes && other.MaxPartBytes == MaxPartBytes && other.MaxParts == MaxParts && other.MaxFileBytes == MaxFileBytes;
        }
    }

    /// <summary>
    /// How an upload sizes its parts, as part of a <see cref="TransferProgress"/>.
    /// </summary>
    public sealed class UploadPartSizing
    {
        internal UploadPartSizing()
        {
        }

        /// <summary>The mode the upload was asked to size its parts with.</summary>
        public UploadPartSizingMode Mode { get; internal set; }

        /// <summary>Where the sizes come from.</summary>
        public PartSizeSource Source { get; internal set; }

        /// <summary>Why the sizes are Files.com's, or <see cref="Transfers.ServerPlannedReason.None"/>.</summary>
        public ServerPlannedReason ServerPlannedReason { get; internal set; }

        /// <summary>
        /// The limits sizes are chosen within, or null where the upload's parts do not go where the SDK chooses them.
        /// They bind only the sizes the upload chooses: parts Files.com sizes are sent as it plans them.
        /// </summary>
        public PartSizeLimits Limits { get; internal set; }

        /// <summary>
        /// For a stream of unknown length, the most bytes it may hold: <see cref="UploadOptions.MaxStreamLength"/>, and
        /// where the upload chooses its part sizes, no more than the largest file the limits allow. Null for a file, or
        /// a stream nothing bounds.
        /// </summary>
        public long? MaxStreamLength { get; internal set; }

        /// <summary>
        /// Whether every part request carries the part's offset in the file. Always false: this version of the SDK sends
        /// every part to the URL exactly as Files.com gives it, and the destination places parts by their numbers.
        /// </summary>
        public bool SendsPartOffsets { get; internal set; }

        /// <summary>
        /// The size the next part to be read will have, unless it holds the rest of the file, or 0 when the sizes are
        /// Files.com's or every part has been read: for a stream of unknown length, once it has ended.
        /// </summary>
        public long NextPartSize { get; internal set; }

        /// <summary>The size of the largest part read so far.</summary>
        public long LargestPartSize { get; internal set; }

        /// <summary>What most limited the size the upload started with.</summary>
        public PartSizeConstraint StartConstraint { get; internal set; }

        /// <summary>The number of times the parts grew.</summary>
        public int Grows { get; internal set; }

        /// <summary>The number of times larger parts were kept.</summary>
        public int ProbesAccepted { get; internal set; }

        /// <summary>The number of times larger parts were given up.</summary>
        public int ProbesRejected { get; internal set; }

        /// <summary>The upload's most recent sizing decision, or null if it has made none.</summary>
        public PartSizeDecision? LastDecision { get; internal set; }
    }

    /// <summary>
    /// Where the performance profile of an automatic upload came from.
    /// </summary>
    public enum UploadProfileSource
    {
        /// <summary>
        /// From the destination the upload's parts go to, as the negotiated parts show when the upload starts:
        /// <see cref="UploadPerformanceProfile.Agent"/> when Files.com supplies direct connection information for the
        /// upload, <see cref="UploadPerformanceProfile.S3"/> with the scaled S3 policy
        /// (<see cref="AutomaticConcurrencyOptions.ScaledS3"/>) for an ordinary multipart upload straight to Amazon S3,
        /// and <see cref="UploadPerformanceProfile.Generic"/> otherwise.
        /// </summary>
        Server,

        /// <summary>From <see cref="UploadOptions.AutomaticConcurrency"/>, which the caller set.</summary>
        Options,
    }
}