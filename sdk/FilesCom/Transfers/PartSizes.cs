using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FilesCom.Transfers
{
    // The part sizing one upload runs with: the mode and, for its mode, the fixed size or a validated copy of the
    // automatic tuning. Checked before anything is sent.
    internal sealed class PartSizingSettings
    {
        private PartSizingSettings(UploadPartSizingMode mode, long? fixedSize, AutomaticPartSizingOptions tuning)
        {
            Mode = mode;
            FixedSize = fixedSize;
            Tuning = tuning;
        }

        internal UploadPartSizingMode Mode { get; }

        internal long? FixedSize { get; }

        internal AutomaticPartSizingOptions Tuning { get; }

        internal static PartSizingSettings Of(UploadPartSizingMode mode, long? partSize, AutomaticPartSizingOptions tuning)
        {
            switch (mode)
            {
                case UploadPartSizingMode.Automatic:
                case UploadPartSizingMode.ServerPlanned:
                    if (partSize.HasValue)
                    {
                        throw InvalidParameterException.For(nameof(UploadOptions.PartSize), "PartSize applies only to UploadPartSizingMode.Fixed");
                    }
                    return new PartSizingSettings(mode, null, mode == UploadPartSizingMode.Automatic ? (tuning ?? new AutomaticPartSizingOptions()).Validated() : null);
                case UploadPartSizingMode.Fixed:
                    if (!partSize.HasValue)
                    {
                        throw InvalidParameterException.For(nameof(UploadOptions.PartSize), "A fixed-size upload needs a PartSize of at least 1");
                    }
                    if (partSize.Value < 1)
                    {
                        throw InvalidParameterException.OutOfRange(nameof(UploadOptions.PartSize), partSize.Value, "A fixed-size upload needs a PartSize of at least 1");
                    }
                    return new PartSizingSettings(mode, partSize, null);
                default:
                    throw InvalidParameterException.OutOfRange(nameof(UploadOptions.PartSizingMode), mode, "Unknown part sizing mode");
            }
        }
    }

    // The sizes of the parts of an upload that chooses them within limits (UploadGrant). Every part but the last is
    // from floor to largest bytes; the last holds what remains, however little. Each part is sized as it is planned, from
    // the size the upload prefers at that moment, and made larger if a smaller one would leave the rest more parts than
    // allowed. For a file, the rest is what remains of it. For a stream of unknown length it is what remains of
    // the most the stream may hold, its bound: a part is sized as the most it may hold, ends early where the stream does,
    // and never goes past the bound. Plan proves before the first part is sent that the file, or the bound, fits in the
    // parts allowed (ceil(bytes / largest) <= parts), and that check, repeated for every part with Int64 arithmetic that
    // cannot overflow (bytes left - length <= (parts left - 1) * largest), keeps it so to the last part.
    internal sealed class PartSizes
    {
        private readonly object gate = new object();
        private readonly long bytes;
        private readonly bool lengthKnown;
        private readonly long maxParts;
        private readonly long floor;
        private readonly long largest;
        private readonly long? fixedSize;
        private long largestPlanned;
        private bool planned;

        private PartSizes(PartSizeLimits limits, long bytes, bool lengthKnown, long maxParts, long floor, long largest, long? fixedSize, PartSizeController controller, long startSize, PartSizeConstraint constraint)
        {
            Limits = limits;
            this.bytes = bytes;
            this.lengthKnown = lengthKnown;
            this.maxParts = maxParts;
            this.floor = floor;
            this.largest = largest;
            this.fixedSize = fixedSize;
            Controller = controller;
            StartSize = startSize;
            StartConstraint = constraint;
        }

        internal PartSizeLimits Limits { get; }

        // The size the upload started with, and what most limited it.
        internal long StartSize { get; }

        internal PartSizeConstraint StartConstraint { get; }

        // Changes the size of future parts; null for fixed sizes.
        internal PartSizeController Controller { get; }

        internal PartSizeSource Source => fixedSize.HasValue ? PartSizeSource.Fixed : PartSizeSource.Automatic;

        // For a stream of unknown length, the most it may hold; null for a file of known length.
        internal long? StreamBound => lengthKnown ? (long?)null : bytes;

        // The most parts the upload may have.
        internal long MaxParts => maxParts;

        // The size the next part would get, unless it holds the rest of the file.
        internal long PreferredSize => fixedSize ?? Controller.Current.Size;

        // Plans an upload of total bytes, or, when total is null, of a stream of unknown length that may hold at most
        // bound bytes (MaxStreamLength) and the largest file the limits allow. availableParts is the part count
        // Files.com advertised, which narrows the limit when it is lower; bufferLimit is the limiter's buffer space when
        // every part is held whole. When no plan within these bounds holds the file, returns null, with fallback saying
        // why, and the upload uses the sizes Files.com plans, before anything is sent: the limits are the SDK's own, and
        // Files.com may plan larger parts than it chooses. It fails instead where Files.com's sizes could not help
        // either: a fixed size the caller asked for, or parts held in memory, whose buffer space would hold none of the
        // larger parts Files.com would need for a file of known length.
        internal static PartSizes Plan(PartSizeLimits limits, long? total, long? bound, long? availableParts, PartSizingSettings settings, long? bufferLimit, string path, long startedAt, out ServerPlannedReason fallback)
        {
            fallback = ServerPlannedReason.None;
            bool lengthKnown = total.HasValue;
            if (lengthKnown && total.Value > limits.MaxFileBytes)
            {
                // The caller uses the server's sizes for such a file: a defect, not a limit.
                throw new InvalidOperationException($"The {total}-byte upload to {path} is larger than the {limits.MaxFileBytes} bytes its part sizes can be chosen for.");
            }
            long bytes = lengthKnown ? total.Value : Math.Min(limits.MaxFileBytes, bound ?? long.MaxValue);
            long maxParts = limits.PartsAllowed(availableParts);
            long largest = bufferLimit.HasValue && bufferLimit.Value < limits.MaxPartBytes ? bufferLimit.Value : limits.MaxPartBytes;
            bool bufferBound = largest < limits.MaxPartBytes;
            string what = lengthKnown ? $"the {bytes}-byte upload to {path}" : $"the upload to {path}, of a stream that may hold {bytes} bytes,";
            long fewestParts = CeilingDivide(bytes, largest);
            bool fits = !(bytes > largest && largest < limits.MinNonfinalBytes) && fewestParts <= maxParts;
            if (!fits && !lengthKnown && bufferBound)
            {
                // Parts held in memory cannot be large enough: the stream's bound is not lowered to hide that.
                fallback = ServerPlannedReason.MemoryLimit;
                return null;
            }
            if (bytes > largest && largest < limits.MinNonfinalBytes)
            {
                throw new UploadLimitException(UploadLimit.BufferSpace, $"The parts of {what} must be at least {limits.MinNonfinalBytes} bytes, more than the {bufferLimit} bytes its transfer limiter may buffer. {BufferAdvice}");
            }
            if (fewestParts > maxParts)
            {
                if (!bufferBound && settings.Mode != UploadPartSizingMode.Fixed)
                {
                    fallback = ServerPlannedReason.NotAllowed;
                    return null;
                }
                throw new UploadLimitException(UploadLimit.PartCount, $"{Capitalized(what)} needs at least {fewestParts} parts of at most {largest} bytes, more than the {maxParts} parts the upload may have." + (bufferBound ? " " + BufferAdvice : ""));
            }

            if (settings.Mode == UploadPartSizingMode.Fixed)
            {
                long size = settings.FixedSize.Value;
                // The parts the plan sends: of size bytes, all but the last, when the file or bound is larger; otherwise
                // all of it as one last part, which may be smaller than the smallest part allowed but not larger than the
                // largest. Every one of them must be within the limits, the last included.
                long largestPart = Math.Min(size, bytes);
                if (bytes > size && size < limits.MinNonfinalBytes)
                {
                    throw new UploadLimitException(UploadLimit.FixedPartSize, $"The fixed part size of {size} bytes is outside the part sizes the limits allow for the upload to {path}: from {limits.MinNonfinalBytes} to {limits.MaxPartBytes} bytes.");
                }
                if (largestPart > limits.MaxPartBytes)
                {
                    throw new UploadLimitException(UploadLimit.FixedPartSize, bytes > size
                        ? $"The fixed part size of {size} bytes is outside the part sizes the limits allow for the upload to {path}: from {limits.MinNonfinalBytes} to {limits.MaxPartBytes} bytes."
                        : $"With a fixed part size of {size} bytes, {what} would be sent as one part of {bytes} bytes, more than the {limits.MaxPartBytes} bytes the limits allow a part.");
                }
                if (largestPart > largest)
                {
                    throw new UploadLimitException(UploadLimit.FixedPartSize, $"With a fixed part size of {size} bytes, {what} needs parts of {largestPart} bytes, more than the {largest} bytes its transfer limiter may buffer. {BufferAdvice}");
                }
                if (bytes > size && CeilingDivide(bytes, size) > maxParts)
                {
                    throw new UploadLimitException(UploadLimit.FixedPartSize, $"With fixed parts of {size} bytes, {what} needs {CeilingDivide(bytes, size)} parts, more than the {maxParts} the upload may have.");
                }
                return new PartSizes(limits, bytes, lengthKnown, maxParts, limits.MinNonfinalBytes, largest, size, null, size, PartSizeConstraint.None);
            }

            AutomaticPartSizingOptions tuning = settings.Tuning;
            // The caller's bounds are preferences within the limits: where they disagree, the limits win.
            long floor = Math.Min(Math.Max(limits.MinNonfinalBytes, tuning.MinPartSize), largest);
            long ceiling = Math.Min(Math.Min(limits.MaxPartBytes, Math.Max(tuning.MaxPartSize, floor)), largest);
            long start;
            if (lengthKnown)
            {
                start = tuning.PreferredSizeFor(bytes);
                long workload = tuning.WorkloadBytes ?? bytes;
                if (workload / start < tuning.WorkloadParts)
                {
                    start = Math.Min(start, workload / tuning.WorkloadParts);
                }
            }
            else
            {
                // Parts are raised, as they are planned, whenever the bound needs larger ones.
                start = tuning.UnknownLengthStartSize;
            }
            PartSizeConstraint constraint = PartSizeConstraint.None;
            if (start < floor)
            {
                start = floor;
                constraint = tuning.MinPartSize > limits.MinNonfinalBytes && floor == tuning.MinPartSize ? PartSizeConstraint.MinPartSize : PartSizeConstraint.ServerMinimum;
            }
            if (start > ceiling)
            {
                start = ceiling;
                constraint = ceiling == largest && bufferBound ? PartSizeConstraint.BufferLimit : ceiling == limits.MaxPartBytes ? PartSizeConstraint.ServerMaximum : PartSizeConstraint.MaxPartSize;
            }
            long fit = lengthKnown ? CeilingDivide(bytes, maxParts) : 0;
            if (start < fit)
            {
                start = fit;
                constraint = PartSizeConstraint.PartLimit;
            }
            var controller = new PartSizeController(tuning, start, Math.Max(start, ceiling), path, startedAt);
            return new PartSizes(limits, bytes, lengthKnown, maxParts, floor, largest, null, controller, start, constraint);
        }

        private static string Capitalized(string text)
        {
            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }

        // The length of part number, starting at offset, and the size epoch it was sized in: for a stream of unknown
        // length, the most the part may hold. Called once per part, in order, as the part is planned.
        internal long LengthFor(long number, long offset, out int sizeEpoch)
        {
            long remaining = bytes - offset;
            long partsLeft = maxParts - (number - 1);
            if (partsLeft < 1)
            {
                // Plan proved this cannot happen: a defect, not a limit, so not an UploadLimitException.
                throw new InvalidOperationException($"Part {number} is more than the {maxParts} parts the upload may have.");
            }
            long length;
            if (fixedSize.HasValue)
            {
                length = fixedSize.Value;
                sizeEpoch = 0;
            }
            else
            {
                PartSizeController.SizeChoice current = Controller.Current;
                length = Math.Max(current.Size, floor);
                sizeEpoch = current.Epoch;
            }
            // Leave at most partsLeft - 1 parts of the largest size for the rest of the file.
            long restCapacity = partsLeft - 1 > long.MaxValue / largest ? long.MaxValue : (partsLeft - 1) * largest;
            if (remaining - length > restCapacity)
            {
                length = remaining - restCapacity;
            }
            if (lengthKnown ? length >= remaining : length > remaining)
            {
                length = remaining;
            }
            if (length > largest)
            {
                // Plan proved this cannot happen, for the last part as for any other: a defect, not a limit, so not an
                // UploadLimitException.
                throw new InvalidOperationException($"Part {number} cannot be planned within the limits: {remaining} bytes remain for {partsLeft} parts of at most {largest} bytes.");
            }
            lock (gate)
            {
                planned = true;
                largestPlanned = Math.Max(largestPlanned, length);
            }
            return length;
        }

        internal void Describe(UploadPartSizing sizing, bool morePartsToPlan)
        {
            sizing.Source = Source;
            sizing.Limits = Limits;
            sizing.MaxStreamLength = StreamBound;
            sizing.StartConstraint = StartConstraint;
            lock (gate)
            {
                sizing.LargestPartSize = largestPlanned;
                sizing.NextPartSize = morePartsToPlan || !planned ? PreferredSize : 0;
            }
            Controller?.Describe(sizing);
        }

        internal UploadSizingTrace StartTrace(string path, TimeSpan elapsed)
        {
            return new UploadSizingTrace(path, elapsed) { Decision = PartSizeDecision.Chosen, Size = StartSize, Constraint = StartConstraint };
        }

        // Advice for a stream upload whose parts do not fit the limiter's buffer space.
        private const string BufferAdvice = "Allow more with TransferLimiter.MaxBufferedBytes, or upload from a file, which does not hold whole parts in memory.";

        // ceil(dividend / divisor) for dividend >= 0 and divisor > 0, without the overflow of (dividend + divisor - 1).
        internal static long CeilingDivide(long dividend, long divisor)
        {
            return dividend / divisor + (dividend % divisor == 0 ? 0 : 1);
        }
    }

    internal enum SizeProbeOutcome
    {
        Continuing,
        Accepted,
        Rejected,
    }

    // Chooses the size of the parts an upload has not planned yet, from the windows its concurrency controller judges,
    // which calls it under its own lock and with the time of each event. It changes the size only when the controller is
    // not changing its target: after the target has reached its ceiling or a raised target was withdrawn, and after the
    // size has been kept for GrowthIntervalWindows judged windows. Growth is a probe: the parts planned from then on get
    // GrowthFactor times the size, and once the first of them has been accepted, each judged window compares its
    // throughput with the level before the probe. If none beats it by ProbeGainPercent within ProbeWindows windows, or a
    // failure or throttling interrupts the probe, the size goes back and growth holds for HoldWindows windows. Every
    // change starts a new size epoch, and windows judge only parts of the current epoch, starting again after each part
    // of an earlier one that arrives: neither a probe nor the level after it went back is measured with parts of another
    // size still being sent. The planner reads the current size from its own thread.
    internal sealed class PartSizeController
    {
        private static readonly double TicksPerSecond = Stopwatch.Frequency;

        private readonly object gate = new object();
        private readonly AutomaticPartSizingOptions tuning;
        private readonly long ceiling;
        private readonly string path;
        private readonly long startedAt;
        private SizeChoice current;

        private int windowsAtSize;
        private int holdWindows;
        private long probeFrom;
        private double probeBaseline;
        private int probeMisses;
        private bool awaitingProbeParts;

        private int grows;
        private int probesAccepted;
        private int probesRejected;
        private PartSizeDecision? lastDecision;

        internal PartSizeController(AutomaticPartSizingOptions tuning, long start, long ceiling, string path, long startedAt)
        {
            this.tuning = tuning;
            this.ceiling = ceiling;
            this.path = path;
            this.startedAt = startedAt;
            current = new SizeChoice(start, 0);
        }

        internal SizeChoice Current
        {
            get
            {
                lock (gate)
                {
                    return current;
                }
            }
        }

        // Whether a probe is under way; the controller then judges windows by it alone.
        internal bool Probing => probeFrom > 0;

        // Whether the probe's first part has yet to be accepted, until when windows are not measured.
        internal bool AwaitingProbeParts => awaitingProbeParts;

        internal int Epoch => current.Epoch;

        // The first part of the probe's size has been accepted: its windows can start.
        internal void ProbePartsArrived()
        {
            awaitingProbeParts = false;
        }

        // A judged window at the current size, outside a probe.
        internal void WindowJudged()
        {
            windowsAtSize++;
            if (holdWindows > 0)
            {
                holdWindows--;
            }
        }

        // Starts a probe with larger parts if the size is due to grow, from a level measured at baseline throughput.
        internal bool TryGrow(long now, double throughput, double baseline, List<UploadTraceEvent> decisions)
        {
            if (Probing || holdWindows > 0 || windowsAtSize < tuning.GrowthIntervalWindows || current.Size >= ceiling || baseline <= 0)
            {
                return false;
            }
            long next = (long)Math.Min(ceiling, Math.Floor(current.Size * tuning.GrowthFactor));
            if (next <= current.Size)
            {
                return false;
            }
            long previous = current.Size;
            probeFrom = previous;
            probeBaseline = baseline;
            probeMisses = 0;
            awaitingProbeParts = true;
            windowsAtSize = 0;
            grows++;
            Change(next);
            Decide(now, PartSizeDecision.Grew, previous, decisions, throughput, baseline);
            return true;
        }

        // Judges a window of the probe; baseline is the throughput it was compared with.
        internal SizeProbeOutcome JudgeProbe(long now, double throughput, List<UploadTraceEvent> decisions, out double baseline)
        {
            baseline = probeBaseline;
            if (throughput >= probeBaseline * (1 + (tuning.ProbeGainPercent / 100)))
            {
                probesAccepted++;
                probeFrom = 0;
                windowsAtSize = 0;
                Decide(now, PartSizeDecision.ProbeAccepted, current.Size, decisions, throughput, probeBaseline);
                return SizeProbeOutcome.Accepted;
            }
            if (++probeMisses < tuning.ProbeWindows)
            {
                return SizeProbeOutcome.Continuing;
            }
            Reject(now, throughput, decisions);
            return SizeProbeOutcome.Rejected;
        }

        // A failure or throttling interrupted the probe, if one was under way: the size goes back.
        internal bool Interrupt(long now, List<UploadTraceEvent> decisions)
        {
            if (!Probing)
            {
                return false;
            }
            Reject(now, 0, decisions);
            return true;
        }

        internal void Describe(UploadPartSizing sizing)
        {
            lock (gate)
            {
                sizing.Grows = grows;
                sizing.ProbesAccepted = probesAccepted;
                sizing.ProbesRejected = probesRejected;
                sizing.LastDecision = lastDecision;
            }
        }

        private void Reject(long now, double throughput, List<UploadTraceEvent> decisions)
        {
            long previous = current.Size;
            probesRejected++;
            Change(probeFrom);
            probeFrom = 0;
            awaitingProbeParts = false;
            windowsAtSize = 0;
            holdWindows = tuning.HoldWindows;
            Decide(now, PartSizeDecision.ProbeRejected, previous, decisions, throughput, probeBaseline);
        }

        private void Change(long size)
        {
            lock (gate)
            {
                current = new SizeChoice(size, current.Epoch + 1);
            }
        }

        private void Decide(long now, PartSizeDecision decision, long previous, List<UploadTraceEvent> decisions, double throughput, double baseline)
        {
            lock (gate)
            {
                lastDecision = decision;
            }
            decisions?.Add(new UploadSizingTrace(path, TimeSpan.FromSeconds((now - startedAt) / TicksPerSecond))
            {
                Decision = decision,
                PreviousSize = previous,
                Size = current.Size,
                ThroughputBytesPerSecond = throughput,
                BaselineBytesPerSecond = baseline,
            });
        }

        // A part size and the epoch it belongs to, read together.
        internal sealed class SizeChoice
        {
            internal SizeChoice(long size, int epoch)
            {
                Size = size;
                Epoch = epoch;
            }

            internal long Size { get; }

            internal int Epoch { get; }
        }
    }
}