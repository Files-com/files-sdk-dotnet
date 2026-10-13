using System.Collections.Generic;
using System.Linq;

namespace FilesCom.Transfers
{
    /// <summary>
    /// How an upload decides the sizes of its parts.
    /// </summary>
    /// <remarks>
    /// The upload chooses its part sizes, for files and for streams of unknown length alike, only where its parts go
    /// straight to Amazon S3 as an ordinary multipart upload: every part URL Files.com gives is a signed S3 UploadPart URL
    /// for one upload, on a general-purpose S3 endpoint, that signs neither the part's length nor its bytes and needs no
    /// other header. S3 lets those numbered parts differ in size, so Files.com's sizes are then recommendations, and every
    /// part is sent with a Content-Length of exactly its bytes. Sizes are chosen within <see cref="PartSizeLimits"/>: S3's
    /// rules and the SDK's planning limits for them. Elsewhere, such as remote servers Files.com reaches itself, Agents,
    /// S3-compatible storage or appends, every mode uses the sizes Files.com plans, as does a stream of unknown length
    /// whose parts could not be large enough in memory (<see cref="ServerPlannedReason.MemoryLimit"/>). The limits bind
    /// only the sizes the upload chooses.
    /// How many parts are sent at once is chosen separately, by <see cref="UploadOptions.ConcurrencyMode"/>.
    /// </remarks>
    public enum UploadPartSizingMode
    {
        /// <summary>
        /// Chooses a size from the file's size and <see cref="AutomaticPartSizingOptions"/>, and tries larger parts for
        /// the parts not yet read when adding parts at once no longer raises throughput. The default.
        /// </summary>
        Automatic,

        /// <summary>
        /// Sends every part but the last with <see cref="UploadOptions.PartSize"/> bytes, where the upload chooses its
        /// sizes; the upload fails before sending anything if the limits do not allow that size for the file. Use it,
        /// with <see cref="UploadConcurrencyMode.Static"/>, when an upload must behave the same way every time.
        /// </summary>
        Fixed,

        /// <summary>Uses the sizes Files.com plans, even where the upload could choose them.</summary>
        ServerPlanned,
    }

    /// <summary>
    /// A preferred part size for files up to a given size, for <see cref="AutomaticPartSizingOptions.KnownSizeTiers"/>.
    /// </summary>
    public sealed class PartSizeTier
    {
        /// <summary>Makes a tier: files of at most <paramref name="maxFileBytes"/> bytes start with parts of <paramref name="partSize"/> bytes.</summary>
        /// <param name="maxFileBytes">The largest file the tier applies to, in bytes.</param>
        /// <param name="partSize">The part size to start with, in bytes.</param>
        public PartSizeTier(long maxFileBytes, long partSize)
        {
            MaxFileBytes = maxFileBytes;
            PartSize = partSize;
        }

        /// <summary>The largest file the tier applies to, in bytes. Files larger than every tier use the last one.</summary>
        public long MaxFileBytes { get; }

        /// <summary>The part size, in bytes, an upload of such a file starts with.</summary>
        public long PartSize { get; }
    }

    /// <summary>
    /// Tuning for <see cref="UploadPartSizingMode.Automatic"/> part sizes, for rollout and diagnostics. The defaults
    /// suit most uploads; change only what you need.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The upload starts with the size of the first of <see cref="KnownSizeTiers"/> that covers its file. If that would
    /// give the workload fewer than <see cref="WorkloadParts"/> parts, it starts smaller, so that the workload has parts
    /// enough to send several at once: <see cref="WorkloadBytes"/> is the workload's size, or the file's when null. The
    /// size then stays from <see cref="MinPartSize"/> to <see cref="MaxPartSize"/>, within the limits. A part is made
    /// larger when that is needed to stay within the number of parts allowed, even beyond
    /// <see cref="MaxPartSize"/>; the file's last part holds what remains, however little.
    /// </para>
    /// <para>
    /// Later sizes follow what the upload measures, in the windows its concurrency is judged by (see
    /// <see cref="AutomaticConcurrencyOptions"/>). Only one of the two changes at a time. When adding parts at once
    /// stops helping, because the concurrency target has reached its ceiling, a raised target was withdrawn or a warm
    /// start rests (see <see cref="UploadOptions.AutomaticConcurrency"/>), and the size has been kept for
    /// <see cref="GrowthIntervalWindows"/> measured windows, the parts not yet read get <see cref="GrowthFactor"/> times
    /// the size, up to <see cref="MaxPartSize"/>. That is a probe: within <see cref="ProbeWindows"/> windows of parts of
    /// the new size it must beat the throughput before it by <see cref="ProbeGainPercent"/>. Otherwise, or if a failure,
    /// throttling or the end of a warm start interrupts it, the size goes back and growth waits
    /// <see cref="HoldWindows"/> windows. Parts already read, being sent or waiting to retry keep their
    /// size and bytes; once the size changes, whether it grew or went back, only parts of the new size are measured,
    /// and windows start again after each part of an earlier size that arrives, though all of them count as sent.
    /// </para>
    /// <para>
    /// A stream of unknown length starts with <see cref="UnknownLengthStartSize"/>, and grows the same way. Its parts
    /// are also made larger, before they are read, when smaller ones could leave too few parts for the rest of the
    /// most it may hold: <see cref="UploadOptions.MaxStreamLength"/>, or the largest file the limits allow.
    /// </para>
    /// <para>
    /// A stream upload keeping its parts in memory (<see cref="StreamPartStorage.Memory"/>) keeps them within its
    /// limiter's <see cref="TransferLimiter.MaxBufferedBytes"/> too. The defaults are provisional: they have been
    /// checked for safe behavior, not yet tuned against real destinations, and may change. An upload copies these
    /// settings when it starts.
    /// </para>
    /// </remarks>
    public sealed class AutomaticPartSizingOptions
    {
        private const long MiB = 1024 * 1024;

        // The largest factor a probe can grow parts by.
        internal const double MaxGrowthFactor = 16;

        /// <summary>Makes a set of defaults.</summary>
        public AutomaticPartSizingOptions()
        {
            KnownSizeTiers = new List<PartSizeTier>
            {
                new PartSizeTier(256 * MiB, 8 * MiB),
                new PartSizeTier(4096 * MiB, 16 * MiB),
                new PartSizeTier(64 * 1024 * MiB, 32 * MiB),
                new PartSizeTier(long.MaxValue, 64 * MiB),
            };
            WorkloadParts = 16;
            UnknownLengthStartSize = 8 * MiB;
            MaxPartSize = 256 * MiB;
            GrowthFactor = 2;
            GrowthIntervalWindows = 2;
            ProbeGainPercent = 10;
            ProbeWindows = 2;
            HoldWindows = 4;
        }

        /// <summary>
        /// The part size to start with, by the file's size, in order of <see cref="PartSizeTier.MaxFileBytes"/>. At
        /// least one. The defaults are 8 MiB for files up to 256 MiB, 16 MiB up to 4 GiB, 32 MiB up to 64 GiB, and
        /// 64 MiB for larger files.
        /// </summary>
        public IList<PartSizeTier> KnownSizeTiers { get; set; }

        /// <summary>
        /// The size of the whole workload this upload belongs to, such as a batch of files uploaded together, in bytes;
        /// if null, the size of this upload's file. Only the starting size uses it.
        /// </summary>
        public long? WorkloadBytes { get; set; }

        /// <summary>
        /// The fewest parts the workload should be divided into, at least 1: a smaller workload gets smaller parts, so
        /// that it can send several at once. 16 by default.
        /// </summary>
        public int WorkloadParts { get; set; }

        /// <summary>
        /// The size, in bytes, the parts of a stream of unknown length start with: 8 MiB by default. At least 1; the
        /// other bounds apply to it too.
        /// </summary>
        public long UnknownLengthStartSize { get; set; }

        /// <summary>The smallest size, in bytes, to choose for a part other than the last. At least 0; Files.com's minimum applies too.</summary>
        public long MinPartSize { get; set; }

        /// <summary>
        /// The largest size, in bytes, to choose or grow parts to, at least <see cref="MinPartSize"/>: 256 MiB by
        /// default. Files.com's maximum applies too, and parts can be larger when the file needs them to be.
        /// </summary>
        public long MaxPartSize { get; set; }

        /// <summary>How many times larger each growth makes the parts, more than 1 and at most 16. 2 by default.</summary>
        public double GrowthFactor { get; set; }

        /// <summary>The number of measured windows at a size before parts grow. At least 1.</summary>
        public int GrowthIntervalWindows { get; set; }

        /// <summary>The throughput gain, in percent, larger parts must show to be kept. At least 0.</summary>
        public double ProbeGainPercent { get; set; }

        /// <summary>The number of measured windows larger parts get to show their gain. At least 1.</summary>
        public int ProbeWindows { get; set; }

        /// <summary>The number of measured windows growth waits after larger parts were given up. At least 0.</summary>
        public int HoldWindows { get; set; }

        // A checked copy, so later changes to this object do not affect an upload that has started.
        internal AutomaticPartSizingOptions Validated()
        {
            if (KnownSizeTiers == null || KnownSizeTiers.Count == 0)
            {
                throw InvalidParameterException.For(Name(nameof(KnownSizeTiers)), "Must have at least one tier");
            }
            List<PartSizeTier> tiers = KnownSizeTiers.ToList();
            for (int i = 0; i < tiers.Count; i++)
            {
                if (tiers[i] == null)
                {
                    throw InvalidParameterException.For(Name(nameof(KnownSizeTiers)), "Cannot hold null");
                }
                if (tiers[i].PartSize < 1 || tiers[i].MaxFileBytes < 0)
                {
                    throw InvalidParameterException.For(Name(nameof(KnownSizeTiers)), $"Tier {i + 1} must have a part size of at least 1 and a file size of at least 0");
                }
                if (i > 0 && tiers[i].MaxFileBytes <= tiers[i - 1].MaxFileBytes)
                {
                    throw InvalidParameterException.For(Name(nameof(KnownSizeTiers)), "Must be in order of increasing file size");
                }
            }
            if (WorkloadBytes.HasValue && WorkloadBytes.Value < 0)
            {
                throw InvalidParameterException.OutOfRange(Name(nameof(WorkloadBytes)), WorkloadBytes, "Must be at least 0");
            }
            AtLeast(nameof(WorkloadParts), WorkloadParts, 1);
            if (UnknownLengthStartSize < 1)
            {
                throw InvalidParameterException.OutOfRange(Name(nameof(UnknownLengthStartSize)), UnknownLengthStartSize, "Must be at least 1");
            }
            if (MinPartSize < 0)
            {
                throw InvalidParameterException.OutOfRange(Name(nameof(MinPartSize)), MinPartSize, "Must be at least 0");
            }
            if (MaxPartSize < 1 || MaxPartSize < MinPartSize)
            {
                throw InvalidParameterException.OutOfRange(Name(nameof(MaxPartSize)), MaxPartSize, $"Must be at least 1 and at least {Name(nameof(MinPartSize))} ({MinPartSize})");
            }
            if (double.IsNaN(GrowthFactor) || GrowthFactor <= 1 || GrowthFactor > MaxGrowthFactor)
            {
                throw InvalidParameterException.OutOfRange(Name(nameof(GrowthFactor)), GrowthFactor, $"Must be more than 1 and at most {MaxGrowthFactor}");
            }
            AtLeast(nameof(GrowthIntervalWindows), GrowthIntervalWindows, 1);
            if (double.IsNaN(ProbeGainPercent) || double.IsInfinity(ProbeGainPercent) || ProbeGainPercent < 0)
            {
                throw InvalidParameterException.OutOfRange(Name(nameof(ProbeGainPercent)), ProbeGainPercent, "Must be a finite number, at least 0");
            }
            AtLeast(nameof(ProbeWindows), ProbeWindows, 1);
            AtLeast(nameof(HoldWindows), HoldWindows, 0);
            var copy = (AutomaticPartSizingOptions)MemberwiseClone();
            copy.KnownSizeTiers = tiers.AsReadOnly();
            return copy;
        }

        // The size a file of totalBytes starts with, before the workload and the limits apply.
        internal long PreferredSizeFor(long totalBytes)
        {
            foreach (PartSizeTier tier in KnownSizeTiers)
            {
                if (totalBytes <= tier.MaxFileBytes)
                {
                    return tier.PartSize;
                }
            }
            return KnownSizeTiers[KnownSizeTiers.Count - 1].PartSize;
        }

        private static string Name(string setting)
        {
            return "AutomaticPartSizing." + setting;
        }

        private static void AtLeast(string setting, int value, int minimum)
        {
            if (value < minimum)
            {
                throw InvalidParameterException.OutOfRange(Name(setting), value, $"Must be at least {minimum}");
            }
        }
    }
}