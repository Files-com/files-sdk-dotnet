using FilesCom.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FilesCom.Transfers
{
    // The concurrency settings one upload runs with, after UploadOptions.MaxConcurrency has been applied. The bounds
    // keep their order: Min <= Start <= Soft <= Max, and Min <= Plateau <= Max.
    internal sealed class ConcurrencySettings
    {
        // Static uploads measure windows of this many requests for their progress reports, and decide nothing.
        private const int StaticWindowAttempts = 4;

        private ConcurrencySettings(UploadConcurrencyMode mode, AutomaticConcurrencyOptions tuning, UploadPerformanceProfile profile, UploadProfileSource profileSource, int start, int min, int soft, int max, int plateau, int latencyFloor, int probeFloor = 0, int throughputFloor = 0, UploadStartup startup = UploadStartup.Default)
        {
            ProbeFloor = probeFloor;
            ThroughputFloor = throughputFloor;
            Startup = startup;
            Mode = mode;
            Tuning = tuning;
            Profile = profile;
            ProfileSource = profileSource;
            Start = start;
            Min = min;
            Soft = soft;
            Max = max;
            Plateau = plateau;
            LatencyFloor = latencyFloor;
        }

        internal UploadConcurrencyMode Mode { get; }

        internal bool Adaptive => Mode == UploadConcurrencyMode.Automatic;

        // The rest of an automatic upload's tuning; null for a static upload.
        internal AutomaticConcurrencyOptions Tuning { get; }

        // Where an automatic upload's tuning came from; for a static upload, only the destination its part URLs showed.
        internal UploadPerformanceProfile Profile { get; }

        internal UploadProfileSource ProfileSource { get; }

        internal int Start { get; }

        internal int Min { get; }

        internal int Soft { get; }

        internal int Max { get; }

        internal int Plateau { get; }

        // The lowest target a latency backoff lowers it to: Min, unless the tuning sets a floor of its own.
        internal int LatencyFloor { get; }

        // The fast-link probe floor (AutomaticConcurrencyOptions.ProbeFloorTarget, within Max), or 0 for none.
        internal int ProbeFloor { get; }

        // The lowest target a rejected probe's shrink lowers it to (ThroughputFloorTarget, or Min).
        internal int ThroughputFloor { get; }

        internal int WindowAttempts => Tuning?.WindowAttempts ?? StaticWindowAttempts;

        // How an upload whose policy the SDK chose starts (UploadOptions.Startup); Default for every other.
        internal UploadStartup Startup { get; }

        // Whether the SDK chose the scaled S3 policy itself (UploadOptions.UseDestination), with no tuning from the
        // caller, and its Startup is the default: an upload's own controller, or the scope a directory upload's files
        // share, then warm-starts (ConcurrencyController). Otherwise it starts at Start.
        internal bool WarmStarts => Adaptive && ProfileSource == UploadProfileSource.Server && Profile == UploadPerformanceProfile.S3 && Startup == UploadStartup.Default;

        internal static ConcurrencySettings Static(int concurrency)
        {
            return new ConcurrencySettings(UploadConcurrencyMode.Static, null, UploadPerformanceProfile.Generic, UploadProfileSource.Server, concurrency, concurrency, concurrency, concurrency, concurrency, concurrency);
        }

        // tuning must have been validated; cap is UploadOptions.MaxConcurrency.
        internal static ConcurrencySettings Automatic(AutomaticConcurrencyOptions tuning, int? cap, UploadProfileSource source, UploadStartup startup = UploadStartup.Default)
        {
            int max = Math.Min(tuning.MaxTarget, cap ?? int.MaxValue);
            int min = Math.Min(tuning.MinTarget, max);
            return new ConcurrencySettings(UploadConcurrencyMode.Automatic, tuning, tuning.Profile, source, Math.Min(tuning.StartTarget, max), min, Math.Min(tuning.SoftCeiling, max), max, Math.Min(tuning.PlateauTarget, max), Math.Max(min, Math.Min(tuning.LatencyFloorTarget ?? min, max)),
                tuning.ProbeFloorTarget.HasValue && tuning.ProbeFloorBytesPerSecond > 0 ? Math.Max(min, Math.Min(tuning.ProbeFloorTarget.Value, max)) : 0, Math.Max(min, Math.Min(tuning.ThroughputFloorTarget ?? min, max)), startup);
        }

        // A static upload's settings, recording the destination its part URLs showed, which changes nothing it does.
        internal ConcurrencySettings Described(UploadPerformanceProfile profile)
        {
            return new ConcurrencySettings(Mode, Tuning, profile, UploadProfileSource.Server, Start, Min, Soft, Max, Plateau, LatencyFloor, ProbeFloor, ThroughputFloor, Startup);
        }
    }

    // What one network attempt to send a part found out, as the controller uses it.
    internal sealed class AttemptSample
    {
        internal AttemptSample(bool accepted, bool throttled, bool transient, TimeSpan? retryAfter, long bytes, TimeSpan duration, int sizeEpoch = 0, PartSizeController sizer = null)
        {
            Sizer = sizer;
            Accepted = accepted;
            Throttled = throttled;
            Transient = transient;
            RetryAfter = retryAfter;
            Bytes = bytes;
            Duration = duration;
            SizeEpoch = sizeEpoch;
        }

        internal bool Accepted { get; }

        // HTTP 429 or 503, or Retry-After on a failure: the server asked the client to slow down.
        internal bool Throttled { get; }

        // A failure that trying again could fix: a connection failure, 5xx other than 501, 408, 429 or storage's
        // RequestTimeout (RetryPolicy.StorageTimedOut).
        internal bool Transient { get; }

        internal TimeSpan? RetryAfter { get; }

        // The part's bytes.
        internal long Bytes { get; }

        // From sending the request to receiving its answer.
        internal TimeSpan Duration { get; }

        // The size epoch the part was sized in, when the upload chooses its part sizes.
        internal int SizeEpoch { get; }

        // The size controller of the upload the part belongs to, when it chooses its part sizes: the controller's own
        // sizing only judges the parts it sized (ConcurrencyController.AttachSizing).
        internal PartSizeController Sizer { get; }
    }

    // Chooses an upload's concurrency target from what its network attempts measure, and keeps the counts that explain
    // it. It is not thread-safe: AttemptAdmission calls it under its lock with the time of each event, so every
    // decision sees one consistent state. It never waits and never calls out.
    //
    // Measuring. Successful attempts are grouped into windows at one target, each of at least WindowAttempts
    // completions, one for each request of the target, and lasting at least their mean duration. A window's throughput
    // is the bytes they delivered divided by the window's elapsed time, never by the sum of their overlapping
    // durations, and its latency is their total duration per byte, which puts parts of different sizes on one scale.
    // Comparing that latency with the lowest seen gives a Vegas-style estimate of how many requests were queued rather
    // than moving data. A window is judged only if the upload had its target in flight for most of it: time spent
    // waiting for the shared limiter, for a part to be read, or sending one part at a time shows nothing about what
    // more connections would do, so such a window neither grows the target nor counts against a probe. Attempts that
    // complete while new requests are paused belong to the time before the pause and are not measured.
    //
    // Deciding. After GrowthIntervalWindows judged windows at a target, the target rises by GrowthStep, up to the soft
    // ceiling until the upload has delivered SoftCeilingUnlockBytes and a window has measured
    // SoftCeilingUnlockBytesPerSecond, then up to Max. Each rise is a probe: within ProbeWindows judged windows its
    // throughput must beat the mean of the windows at the target it came from by the required gain, or the target goes
    // back there and growth holds for HoldWindows. A throttling answer pauses new attempts and lowers the target; other
    // temporary failures lower it too. Each lowering starts a new epoch, and only an attempt started in the current
    // epoch can lower it again, so a burst of failures from requests already in flight counts once.
    //
    // Local I/O. A download's attempt in flight that is writing what it received to the destination (Local) moves no
    // bytes over the network meanwhile: while the requests on the network are fewer than the target because of such
    // writes, the time is DestinationWrites, not time at target, so a window bound by the destination is not judged,
    // and it teaches the target neither a gain nor a loss. The count of attempts writing is integrated over time like
    // the count in flight, so overlapping writes are never added up.
    //
    // Scaled policies (the tuning's fast-link probe floor, loss tolerance and probe-rejection shrink, all off by default).
    // Once a judged window has reached ProbeFloorBytesPerSecond, the link is fast: raises up to ProbeFloorTarget need no
    // gain, and neither a rejected probe's shrink nor a latency backoff takes the target below it. A probe above the
    // unlocked soft ceiling, up to the plateau, that loses no more than ProbeLossTolerancePercent is kept. With
    // ProbeRejectionShrinkPercent, a rejected probe lowers the target by that much from the raised target, down to the
    // throughput floor, instead of going back to the level it came from.
    //
    // Warm start (an upload's own controller, or a directory upload's shared scope, once for all its files, when the SDK
    // chose the scaled S3 policy itself: ConcurrencySettings.WarmStarts; never a directory download's).
    // The scaled S3 policy starts at 150 parts, which can flood a slow link. An SDK-selected policy instead starts
    // at one (WarmStartTarget) and grows from acknowledged work, unless its UploadOptions.Startup is Aggressive, which
    // starts at 150 as given tuning does. The first window warms its connection; the second establishes the baseline.
    // A raise adds half the target, rounded up, or one part after a rest. Its first judged
    // window opens after the attempts that first filled the raised target have ended (SettledWindow); after a full
    // policy window of answers, it can judge without that cohort so an unanswered request does not hold growth forever.
    // A raise is kept only when the added parts deliver at least half of what each previous part delivered and their
    // estimated queue is less than half the number added. Otherwise it returns to the previous target and rests for
    // the policy's hold and growth interval before trying one more part. Failures and throttling use normal backoff,
    // with a minimum of one during startup. A window already showing a queue permits no new raise.
    // Small parts have variable response times even on a fast link. WarmWindowAnswers therefore uses at least two
    // answers, and more when their measured variation obscures the gain or queue being judged, up to the policy's
    // window size. This is a sampling heuristic, not a confidence guarantee. Comparisons use median seconds per byte
    // to reduce outlier influence and a reference from judged windows at the target being compared with. A lifetime
    // minimum would let one unusually fast or unsettled early window make every later target appear queued.
    // Any eligible window reaching ProbeFloorBytesPerSecond promotes directly to the policy's start, even before its
    // larger sample window is complete. Startup also ends on reaching that start by measured raises. From then on,
    // the ordinary policy and its minimum apply. A directory's files share this startup once, and add preparation
    // capacity with its raises so file setup and finalization do not starve a newly earned part target.
    // While resting, part-size probes retain their ordinary feedback. Their windows do not advance the rest, and a
    // fast-link promotion interrupts a size probe. Failed attempts retain normal backoff and Retry-After handling.
    //
    // Part sizes. When the upload chooses its part sizes, a PartSizeController changes the size of future parts, and
    // only one of the two changes at a time: a size probe is judged by the windows alone, and the size grows only when
    // the target cannot (it has reached its ceiling, growth holds after a raised target was withdrawn, or the warm start
    // rests). Windows then count only parts sized in the current size epoch until the first part of a probe's size has
    // been accepted, and a failure or throttling ends a size probe. A static upload judges its windows for the size alone.
    internal sealed class ConcurrencyController
    {
        // A window is judged only if the upload had its target in flight for at least this share of it.
        private const double JudgedWindowTimeAtTarget = 0.75;

        // The warm start (see above): its first target, and the share of what each part delivered that each part added
        // must deliver for the raise to be kept: 50% more throughput from 1 part to 2, 25% from 2 to 3, 33% from 3 to 5.
        internal const int WarmStartTarget = 1;
        private const double WarmGainPerPartPercent = 50;

        // In sizing a warm window (WarmWindowAnswers), how many spreads of its queue estimate the point half-way between
        // no queue and the queue that keeps no raise is to lie from either: a rule of thumb, not a confidence level.
        private const double WarmWindowMargin = 2;

        private static readonly double TicksPerSecond = Stopwatch.Frequency;

        private readonly ConcurrencySettings settings;
        private readonly AutomaticConcurrencyOptions tuning;
        // The size controller the windows judge part sizes for: an upload's own, or, for a scope that several uploads
        // share, the earliest attached that is still uploading (AttachSizing), so that one size probe at a time changes
        // one file's parts. That file's sizes are judged by its own parts alone, in its own windows (AddToSizeWindow).
        private PartSizeController sizing;
        private readonly List<PartSizeController> attachedSizing = new List<PartSizeController>();
        private readonly string path;
        private readonly long startedAt;

        // What AttemptAdmission has admitted: attempts holding a permit, and those of them that have a request slot
        // and are being sent.
        private int permitted;
        private int inFlight;
        // Attempts in flight that are writing to the local destination now (Local).
        private int local;
        private int peakInFlight;
        private UploadSerialReason serialReason;
        private bool measuring;
        private long lastEvent;

        private int target;
        private int epoch;
        // The lowest target failures and throttling lower it to: the settings' Min, or 1 from a warm start's beginning
        // until the warm start reaches the policy's start.
        private int minTarget;
        private long pausedUntil;

        // The warm start, while warming: whether it rests (in windows of the policy's size, counted by holdWindows and
        // windowsTowardGrowth), the windows judged at its start target (warmWindows), and the throughput of the window it
        // last rose after (warmLevel, 0 for none), at warmFrom.
        private bool warming;
        private bool resting;
        private int warmWindows;
        private double warmLevel;
        private int warmFrom;
        // Whether it has rested since it last kept a raise, so that its next raise adds one part (WarmStep).
        private bool restedSinceRaise;
        // The fastest window judged at the target since it was set (levelLowest) and at the target the raise under judgment
        // came from (fromLowest), as the median seconds per byte of its answers, 0 for none, and the queue estimate of the
        // window last judged against them (warmQueue).
        private double levelLowest;
        private double fromLowest;
        private double warmQueue;
        // The variation of answers' times, which sizes its windows (WarmWindowAnswers): over the windows it judged, the
        // squares of each answer's seconds per byte less its window's mean, relative to that mean (variationSquares), and
        // the answers they count, each window's less one (variationAnswers): its degrees of freedom.
        private double variationSquares;
        private int variationAnswers;
        // A raised warm target, until it is judged (settling): the number of the attempt that first filled it (settleLast,
        // 0 until one did), how many of the attempts in flight then are still in flight (settleLeft), when the last of
        // them ended (settledAt), and the answers since the raise (settleAnswers). Every attempt started has a number,
        // from 1 (started), which it ends with (Released).
        private bool settling;
        private long settleLast;
        private int settleLeft;
        private long settledAt;
        private int settleAnswers;
        private long started;
        private bool unlocked;
        private long acceptedBytes;
        private double lowestSecondsPerByte;

        // The target's level: the judged windows measured at it, and how many more it needs before it grows.
        private double levelThroughputSum;
        private int levelWindows;
        private int windowsTowardGrowth;
        private int holdWindows;
        // Whether growth holds because a raised target was withdrawn, a plateau at which larger parts may still help.
        private bool holdingAtPlateau;

        // The probe under way, if probeFrom is not 0: the target it came from and that target's throughput.
        private int probeFrom;
        private double probeBaseline;
        private int probeMisses;

        // The current window, from windowStart, and when the window last closed had opened, with that window's answers'
        // seconds per byte (closedPerByte, as for the current window below) and their median (lastMedianPerByte), which only
        // the warm start uses, kept only while it is under way.
        private long windowStart;
        private long closedWindowStart;
        private List<double> closedPerByte = new List<double>();
        private double lastMedianPerByte;
        private int windowAttempts;
        private long windowBytes;
        private double windowAttemptSeconds;
        // While warming, the seconds per byte of each of its answers of any bytes.
        private List<double> windowPerByte = new List<double>();
        private double windowInFlightTicks;
        private readonly long[] windowTicksBelowTarget = new long[5];

        // In a scope several uploads share, the window of the file whose sizes are judged, from sizeWindowStart, with the
        // time in it that the shared target was in flight, and the level of its current size, measured at sizeLevelTarget.
        private long sizeWindowStart;
        private int sizeWindowAttempts;
        private long sizeWindowBytes;
        private double sizeWindowAttemptSeconds;
        private long sizeWindowTicksAtTarget;
        private double sizeLevelSum;
        private int sizeLevelWindows;
        private int sizeLevelTarget;

        // Totals for snapshots.
        private readonly long[] ticksBelowTarget = new long[5];
        // The best throughput of a judged window: what makes the link fast for the probe floor.
        private double bestJudgedThroughput;
        private ConcurrencyDecision? lastDecision;
        private int grows;
        private int shrinks;
        private int probesAccepted;
        private int probesRejected;
        private int measuredWindows;
        private int unmeasuredWindows;
        private int throttles;
        private int transientFailures;
        private double lastThroughput;
        private double bestThroughput;
        private double lastSecondsPerByte;
        private double lastAverageInFlight;
        private double queueEstimate;

        // warmStart: whether to warm-start, for an upload's own controller, or the scope a directory upload's files
        // share, whose settings warm-start (ConcurrencySettings.WarmStarts); a directory download's scope never does.
        internal ConcurrencyController(ConcurrencySettings settings, string path, long startedAt, PartSizeController sizing = null, bool warmStart = false)
        {
            this.settings = settings;
            tuning = settings.Tuning;
            this.sizing = sizing;
            this.path = path;
            this.startedAt = startedAt;
            target = settings.Start;
            minTarget = settings.Min;
            if (warmStart && settings.Adaptive && settings.Start > WarmStartTarget && tuning.ProbeFloorBytesPerSecond > 0)
            {
                warming = true;
                WarmStarted = true;
                target = WarmStartTarget;
                minTarget = 1;
            }
        }

        internal int Target => target;

        // Whether the warm start is under way, and whether there was one.
        internal bool Warming => warming;

        internal bool WarmStarted { get; }

        internal int Permitted => permitted;

        // Attempts started and not yet ended.
        internal int InFlight => inFlight;

        // Attempts started now belong to this epoch.
        internal int Epoch => epoch;

        internal bool IsPaused(long now)
        {
            return pausedUntil > now;
        }

        internal long PausedUntil => pausedUntil;

        internal void Granted(long now)
        {
            if (!measuring)
            {
                // Measuring starts with the first attempt, not with negotiating and reading the first parts.
                measuring = true;
                lastEvent = now;
                OpenWindow(now);
            }
            Advance(now);
            permitted++;
        }

        // Returns the attempt's number, which it gives Released when it ends.
        internal long Sending(long now)
        {
            Advance(now);
            inFlight++;
            peakInFlight = Math.Max(peakInFlight, inFlight);
            started++;
            if (settling && settleLast == 0 && inFlight >= target)
            {
                settleLast = started;
                settleLeft = inFlight;
            }
            return started;
        }

        // attempt: the number Sending gave the attempt that was sending, or 0 if none is known.
        internal void Released(long now, bool sending, long attempt = 0)
        {
            Advance(now);
            permitted--;
            if (sending)
            {
                inFlight--;
                if (settling && attempt > 0 && attempt <= settleLast && settleLeft > 0 && --settleLeft == 0)
                {
                    settledAt = now;
                }
            }
        }

        // A file of a shared scope that chooses its own part sizes joins (AttachSizing) or leaves (DetachSizing) the files
        // whose sizes may be judged. The first to join has its sizes judged until it leaves; then the next. A probe of a
        // file that leaves is over, and that file's level starts again.
        internal void AttachSizing(long now, PartSizeController sizer)
        {
            Advance(now);
            attachedSizing.Add(sizer);
            if (sizing == null)
            {
                sizing = sizer;
                ResetSizeLevel(now);
            }
        }

        internal void DetachSizing(long now, PartSizeController sizer)
        {
            Advance(now);
            attachedSizing.Remove(sizer);
            if (sizing == sizer)
            {
                sizing = attachedSizing.Count > 0 ? attachedSizing[0] : null;
                ResetSizeLevel(now);
            }
        }

        // An attempt in flight starts (writing) or stops writing what it received to the local destination.
        internal void Local(long now, bool writing)
        {
            Advance(now);
            local += writing ? 1 : -1;
        }

        internal void UseSerial(long now, UploadSerialReason reason)
        {
            Advance(now);
            if (serialReason == UploadSerialReason.None)
            {
                serialReason = reason;
            }
        }

        // Takes one attempt's outcome into account. attemptEpoch is the epoch it started in. Decisions are added to
        // decisions when it is not null.
        internal void Record(long now, int attemptEpoch, AttemptSample sample, List<UploadTraceEvent> decisions)
        {
            Advance(now);
            if (sample.Throttled)
            {
                throttles++;
            }
            else if (!sample.Accepted && sample.Transient)
            {
                transientFailures++;
            }
            if (warming && !sample.Accepted && (sample.Throttled || sample.Transient))
            {
                // The warm start rests from where the failure leaves it, and the failure is handled as any other.
                Rest();
            }
            if (sample.Accepted)
            {
                acceptedBytes += sample.Bytes;
                if (settling)
                {
                    settleAnswers++;
                }
                AddToWindow(now, sample, decisions);
            }
            else if (settings.Adaptive && sample.Throttled)
            {
                Throttled(now, attemptEpoch, sample.RetryAfter, decisions);
            }
            else if (settings.Adaptive && sample.Transient && attemptEpoch == epoch)
            {
                int previous = target;
                if (Lower(tuning.ShrinkPercent))
                {
                    Restart(now);
                    Decide(now, ConcurrencyDecision.ShrankAfterFailure, previous, decisions);
                }
            }
            // A failure disturbs what a size probe measures: the size goes back.
            if (!sample.Accepted && (sample.Throttled || sample.Transient) && sizing != null && sizing.Interrupt(now, decisions))
            {
                ResetLevel(now);
                if (attachedSizing.Count > 0)
                {
                    ResetSizeLevel(now);
                }
            }
        }

        internal UploadConcurrency Snapshot(long now)
        {
            Advance(now);
            return new UploadConcurrency
            {
                Mode = settings.Mode,
                Profile = settings.Profile,
                ProfileSource = settings.ProfileSource,
                Target = target,
                Limit = serialReason != UploadSerialReason.None ? 1 : target,
                ActiveAttempts = inFlight,
                PeakActiveAttempts = peakInFlight,
                MinTarget = minTarget,
                SoftCeiling = settings.Soft,
                MaxTarget = settings.Max,
                SoftCeilingUnlocked = unlocked,
                SerialReason = serialReason,
                LastDecision = lastDecision,
                Grows = grows,
                Shrinks = shrinks,
                ProbesAccepted = probesAccepted,
                ProbesRejected = probesRejected,
                MeasuredWindows = measuredWindows,
                UnmeasuredWindows = unmeasuredWindows,
                Throttles = throttles,
                TransientFailures = transientFailures,
                CooldownRemaining = pausedUntil > now ? Duration(pausedUntil - now) : TimeSpan.Zero,
                LastThroughputBytesPerSecond = lastThroughput,
                BestThroughputBytesPerSecond = bestThroughput,
                LastLatencyPerMebibyte = TimeSpan.FromSeconds(lastSecondsPerByte * 1024 * 1024),
                QueueEstimate = queueEstimate,
                TimeAtTarget = Duration(ticksBelowTarget[(int)BelowTargetReason.None]),
                TimeWaitingForLimiter = Duration(ticksBelowTarget[(int)BelowTargetReason.SharedLimiter]),
                TimeWithoutReadyPart = Duration(ticksBelowTarget[(int)BelowTargetReason.NoPartReady]),
                TimeSendingSerially = Duration(ticksBelowTarget[(int)BelowTargetReason.SerialParts]),
                TimeWritingDestination = Duration(ticksBelowTarget[(int)BelowTargetReason.DestinationWrites]),
            };
        }

        // Adds the time since the last event to the reason the upload had for being where it was.
        private void Advance(long now)
        {
            if (!measuring || now <= lastEvent)
            {
                return;
            }
            long from = lastEvent;
            lastEvent = now;
            int reason = (int)CurrentReason();
            ticksBelowTarget[reason] += now - from;
            long windowFrom = Math.Max(from, windowStart);
            if (now > windowFrom)
            {
                windowTicksBelowTarget[reason] += now - windowFrom;
                windowInFlightTicks += (inFlight - local) * (double)(now - windowFrom);
            }
            long sizeWindowFrom = Math.Max(from, sizeWindowStart);
            if (attachedSizing.Count > 0 && reason == (int)BelowTargetReason.None && now > sizeWindowFrom)
            {
                sizeWindowTicksAtTarget += now - sizeWindowFrom;
            }
        }

        private BelowTargetReason CurrentReason()
        {
            if (serialReason != UploadSerialReason.None)
            {
                return BelowTargetReason.SerialParts;
            }
            if (inFlight - local >= target)
            {
                return BelowTargetReason.None;
            }
            if (inFlight >= target)
            {
                return BelowTargetReason.DestinationWrites;
            }
            return permitted > inFlight ? BelowTargetReason.SharedLimiter : BelowTargetReason.NoPartReady;
        }

        private void AddToWindow(long now, AttemptSample sample, List<UploadTraceEvent> decisions)
        {
            if (attachedSizing.Count > 0)
            {
                // A shared scope's windows measure every file's parts as they are; the parts of the file whose sizes are
                // judged also count in its own window.
                if (sample.Sizer == sizing)
                {
                    AddToSizeWindow(now, sample, decisions);
                }
            }
            else if (sizing != null && sample.SizeEpoch != sizing.Epoch)
            {
                // A part of an earlier size, prepared, sent or retried before the size changed. Its bytes count toward
                // the upload, but windows judge only parts of the current size: the window starts again after it, since
                // it shared the network with the window's parts.
                if (!sizing.AwaitingProbeParts)
                {
                    OpenWindow(now);
                }
                return;
            }
            else if (sizing != null && sizing.AwaitingProbeParts)
            {
                // The probe's first part: its windows start with it, since its own time was mostly spent before then.
                sizing.ProbePartsArrived();
                OpenWindow(now);
                return;
            }
            if (now <= windowStart)
            {
                return;
            }
            windowAttempts++;
            windowBytes += sample.Bytes;
            windowAttemptSeconds += sample.Duration.TotalSeconds;
            if (warming && sample.Bytes > 0)
            {
                windowPerByte.Add(sample.Duration.TotalSeconds / sample.Bytes);
            }
            // A window spans a full round of the target's requests: at least one completion for each of them, and at
            // least as long as they take on average. Requests started together finish together, so a shorter window
            // would measure how they bunch up, not how fast they deliver.
            // While warming, a window is one completion per request of the target, so that the first answers decide, except
            // while the warm start rests; a window it judges holds as many more as WarmWindowAnswers calls for, but still
            // closes on any answer from where it could first close that leaves it showing a fast link, so that the first
            // answers that show one still decide.
            double elapsed = (now - windowStart) / TicksPerSecond;
            if (windowAttempts >= (warming && !resting ? target : Math.Max(settings.WindowAttempts, target)) && elapsed >= windowAttemptSeconds / windowAttempts
                && (!warming || resting || windowAttempts >= WarmWindowAnswers() || windowBytes / elapsed >= tuning.ProbeFloorBytesPerSecond))
            {
                CloseWindow(now, decisions);
            }
        }

        private void CloseWindow(long now, List<UploadTraceEvent> decisions)
        {
            long ticks = now - windowStart;
            double throughput = windowBytes / (ticks / TicksPerSecond);
            double averageInFlight = windowInFlightTicks / ticks;
            if (windowBytes > 0 && windowAttemptSeconds > 0)
            {
                lastSecondsPerByte = windowAttemptSeconds / windowBytes;
                lowestSecondsPerByte = lowestSecondsPerByte > 0 ? Math.Min(lowestSecondsPerByte, lastSecondsPerByte) : lastSecondsPerByte;
                queueEstimate = averageInFlight * (1 - (lowestSecondsPerByte / lastSecondsPerByte));
                lastAverageInFlight = averageInFlight;
            }
            BelowTargetReason shortfall = Shortfall(ticks);
            bool anyBytes = windowBytes > 0;
            closedWindowStart = windowStart;
            if (warming)
            {
                List<double> closed = windowPerByte;
                windowPerByte = closedPerByte;
                closedPerByte = closed;
                lastMedianPerByte = Median(closedPerByte);
            }
            OpenWindow(now);
            if (!anyBytes)
            {
                return;
            }
            lastThroughput = throughput;
            bestThroughput = Math.Max(bestThroughput, throughput);
            if (!settings.Adaptive)
            {
                if (sizing != null && shortfall == BelowTargetReason.None)
                {
                    JudgeSize(now, throughput, decisions);
                }
                return;
            }
            if (shortfall != BelowTargetReason.None)
            {
                unmeasuredWindows++;
                Decide(now, ConcurrencyDecision.WindowNotMeasured, target, decisions, throughput, reason: shortfall);
                return;
            }
            measuredWindows++;
            Judge(now, throughput, decisions);
        }

        // Why the window cannot be judged, or None if the upload had its target in flight for enough of it.
        private BelowTargetReason Shortfall(long ticks)
        {
            if (windowTicksBelowTarget[(int)BelowTargetReason.None] >= JudgedWindowTimeAtTarget * ticks)
            {
                return BelowTargetReason.None;
            }
            BelowTargetReason longest = BelowTargetReason.SharedLimiter;
            foreach (BelowTargetReason reason in new[] { BelowTargetReason.NoPartReady, BelowTargetReason.SerialParts, BelowTargetReason.DestinationWrites })
            {
                if (windowTicksBelowTarget[(int)reason] > windowTicksBelowTarget[(int)longest])
                {
                    longest = reason;
                }
            }
            return longest;
        }

        private void Judge(long now, double throughput, List<UploadTraceEvent> decisions)
        {
            if (!unlocked && settings.Soft < settings.Max && acceptedBytes >= tuning.SoftCeilingUnlockBytes && throughput >= tuning.SoftCeilingUnlockBytesPerSecond)
            {
                unlocked = true;
                Decide(now, ConcurrencyDecision.SoftCeilingUnlocked, target, decisions, throughput);
            }
            bestJudgedThroughput = Math.Max(bestJudgedThroughput, throughput);
            if (warming)
            {
                JudgeWarm(now, throughput, decisions);
                return;
            }
            if (sizing != null && sizing.Probing)
            {
                // A shared scope's file judges its probe in its own windows (AddToSizeWindow); the target holds meanwhile.
                if (attachedSizing.Count == 0)
                {
                    JudgeSizeProbe(now, throughput, decisions);
                }
                return;
            }

            if (probeFrom > 0)
            {
                if (throughput >= probeBaseline * (1 + (RequiredGainPercent(target, probeFrom) / 100)))
                {
                    probesAccepted++;
                    Decide(now, ConcurrencyDecision.ProbeAccepted, target, decisions, throughput, probeBaseline);
                    probeFrom = 0;
                }
                else if (WithinLossTolerance(throughput))
                {
                    // Above the unlocked soft ceiling, a raise that costs no more than the tolerance is kept.
                    probesAccepted++;
                    Decide(now, ConcurrencyDecision.ProbeKeptWithinLossTolerance, target, decisions, throughput, probeBaseline);
                    probeFrom = 0;
                }
                else if (++probeMisses < tuning.ProbeWindows)
                {
                    return;
                }
                else
                {
                    // More connections did not help: go back to the level they were added to (or, with a rejection
                    // shrink, lower by that much, down to the floor), and wait before trying again.
                    int previous = target;
                    double baseline = probeBaseline;
                    target = RejectedProbeTarget();
                    probesRejected++;
                    shrinks++;
                    Restart(now);
                    holdingAtPlateau = true;
                    levelThroughputSum = baseline;
                    levelWindows = 1;
                    Decide(now, ConcurrencyDecision.ProbeRejected, previous, decisions, throughput, baseline);
                    return;
                }
            }

            double level = levelWindows > 0 ? levelThroughputSum / levelWindows : 0;
            if (tuning.LatencyBackoffQueueLimit > 0 && queueEstimate > tuning.LatencyBackoffQueueLimit && levelWindows > 0 && throughput < level * (1 - (Math.Max(tuning.ProbeGainPercent, 1) / 100))
                && !(FastLink && target <= settings.ProbeFloor))
            {
                int previous = target;
                if (Lower(tuning.LatencyShrinkPercent ?? tuning.ShrinkPercent, Math.Max(settings.LatencyFloor, FastLink ? settings.ProbeFloor : 0)))
                {
                    Restart(now);
                    Decide(now, ConcurrencyDecision.ShrankForLatency, previous, decisions, throughput, level);
                    return;
                }
            }

            levelThroughputSum += throughput;
            levelWindows++;
            if (attachedSizing.Count == 0)
            {
                sizing?.WindowJudged();
            }
            if (holdWindows > 0)
            {
                holdWindows--;
                if (holdingAtPlateau)
                {
                    TryGrowParts(now, throughput, decisions);
                }
                return;
            }
            holdingAtPlateau = false;
            if (tuning.LatencyGrowthQueueLimit > 0 && queueEstimate > tuning.LatencyGrowthQueueLimit)
            {
                windowsTowardGrowth = 0;
                Decide(now, ConcurrencyDecision.GrowthHeldForLatency, target, decisions, throughput, level);
                return;
            }
            int ceiling = unlocked ? settings.Max : settings.Soft;
            if (++windowsTowardGrowth < tuning.GrowthIntervalWindows)
            {
                return;
            }
            if (target >= ceiling)
            {
                TryGrowParts(now, throughput, decisions);
                return;
            }

            int from = target;
            probeBaseline = levelThroughputSum / levelWindows;
            probeMisses = 0;
            target = (int)Math.Min(ceiling, (long)from + tuning.GrowthStep);
            // Below the probe floor of a fast link, a raise needs no gain: it is not a probe.
            probeFrom = FastLink && target <= settings.ProbeFloor ? 0 : from;
            grows++;
            levelThroughputSum = 0;
            levelWindows = 0;
            windowsTowardGrowth = 0;
            OpenWindow(now);
            Decide(now, ConcurrencyDecision.Grew, from, decisions, throughput, probeBaseline);
        }

        // A static upload's judged window, for its part size alone.
        private void JudgeSize(long now, double throughput, List<UploadTraceEvent> decisions)
        {
            if (sizing.Probing)
            {
                JudgeSizeProbe(now, throughput, decisions);
                return;
            }
            levelThroughputSum += throughput;
            levelWindows++;
            sizing.WindowJudged();
            TryGrowParts(now, throughput, decisions);
        }

        // Starts a size probe from the current level, when the size is due to grow. In a shared scope the file's own
        // windows start it (AddToSizeWindow).
        private void TryGrowParts(long now, double throughput, List<UploadTraceEvent> decisions)
        {
            if (sizing != null && attachedSizing.Count == 0 && levelWindows > 0 && sizing.TryGrow(now, throughput, levelThroughputSum / levelWindows, decisions))
            {
                ResetLevel(now);
            }
        }

        // A part of the file whose sizes a shared scope judges. Its window holds only that file's parts of its current
        // size, as an upload's own windows do, so neither the other files' parts nor how they bunch with its own decide
        // its sizes: at least WindowAttempts of them, and as long as they take on average. It is measured as what those
        // parts' requests delivered per second of their own time, which the file's share of the target does not change.
        // As an upload's own windows
        // are, it is judged only if the shared target was in flight for enough of it: time waiting for the limiter or
        // for parts to be ready is no measure of a size. Its sizes are judged only while the shared target is not
        // changing, at its ceiling or held after a withdrawn raise, and only against a level measured at that same
        // target: when the target moves, or restarts after a failure or cooldown (Restart), the file's level starts
        // again. Every change of its size starts the shared target's level again too, since what all files deliver changed.
        private void AddToSizeWindow(long now, AttemptSample sample, List<UploadTraceEvent> decisions)
        {
            if (sample.SizeEpoch != sizing.Epoch)
            {
                if (!sizing.AwaitingProbeParts)
                {
                    OpenSizeWindow(now);
                }
                return;
            }
            if (sizing.AwaitingProbeParts)
            {
                sizing.ProbePartsArrived();
                OpenSizeWindow(now);
                return;
            }
            if (now <= sizeWindowStart)
            {
                return;
            }
            // A size probe holds the shared target until it is judged; outside one, the level belongs to one settled target.
            if (!sizing.Probing && (!TargetSettled || target != sizeLevelTarget))
            {
                ResetSizeLevel(now);
                sizeLevelTarget = target;
                return;
            }
            sizeWindowAttempts++;
            sizeWindowBytes += sample.Bytes;
            sizeWindowAttemptSeconds += sample.Duration.TotalSeconds;
            if (sizeWindowAttempts < settings.WindowAttempts || (now - sizeWindowStart) / TicksPerSecond < sizeWindowAttemptSeconds / sizeWindowAttempts)
            {
                return;
            }
            // What the file's own part requests delivered for each second they took: the files share the target's requests
            // unevenly and changingly, so the file's bytes over the window's time would move with its share, not its sizes.
            double throughput = sizeWindowBytes / sizeWindowAttemptSeconds;
            bool eligible = sizeWindowTicksAtTarget >= JudgedWindowTimeAtTarget * (now - sizeWindowStart);
            OpenSizeWindow(now);
            if (!eligible)
            {
                return;
            }
            if (sizing.Probing)
            {
                switch (sizing.JudgeProbe(now, throughput, decisions, out double baseline))
                {
                    case SizeProbeOutcome.Accepted:
                        sizeLevelSum = throughput;
                        sizeLevelWindows = 1;
                        ResetLevel(now);
                        break;
                    case SizeProbeOutcome.Rejected:
                        sizeLevelSum = baseline;
                        sizeLevelWindows = 1;
                        ResetLevel(now);
                        break;
                }
                return;
            }
            sizeLevelSum += throughput;
            sizeLevelWindows++;
            sizing.WindowJudged();
            if (sizing.TryGrow(now, throughput, sizeLevelSum / sizeLevelWindows, decisions))
            {
                sizeLevelSum = 0;
                sizeLevelWindows = 0;
                ResetLevel(now);
            }
        }

        // The file whose sizes a shared scope judges changed, or its size went back: its level starts again.
        private void ResetSizeLevel(long now)
        {
            sizeLevelSum = 0;
            sizeLevelWindows = 0;
            OpenSizeWindow(now);
        }

        private void OpenSizeWindow(long now)
        {
            sizeWindowStart = Math.Max(now, pausedUntil);
            sizeWindowAttempts = 0;
            sizeWindowBytes = 0;
            sizeWindowAttemptSeconds = 0;
            sizeWindowTicksAtTarget = 0;
        }

        // Whether the shared target is not changing: no raise is being probed, and it is at its ceiling or held after a
        // withdrawn raise.
        private bool TargetSettled => probeFrom == 0 && (holdingAtPlateau || target >= (unlocked ? settings.Max : settings.Soft));

        private void JudgeSizeProbe(long now, double throughput, List<UploadTraceEvent> decisions)
        {
            switch (sizing.JudgeProbe(now, throughput, decisions, out double baseline))
            {
                case SizeProbeOutcome.Accepted:
                    // The new size's level starts with the window that showed its gain.
                    ResetLevel(now);
                    levelThroughputSum = throughput;
                    levelWindows = 1;
                    break;
                case SizeProbeOutcome.Rejected:
                    // The old size's level stands.
                    ResetLevel(now);
                    levelThroughputSum = baseline;
                    levelWindows = 1;
                    break;
            }
        }

        // The size changed: measuring at the level starts over.
        private void ResetLevel(long now)
        {
            levelThroughputSum = 0;
            levelWindows = 0;
            windowsTowardGrowth = 0;
            OpenWindow(now);
        }

        // The link is fast once a judged window reached the tuning's ProbeFloorBytesPerSecond (with a probe floor set).
        private bool FastLink => settings.ProbeFloor > 0 && bestJudgedThroughput >= tuning.ProbeFloorBytesPerSecond;

        // A probe that missed its gain is kept when it lost no more than the tuning's tolerance, above an unlocked soft
        // ceiling and up to the plateau.
        private bool WithinLossTolerance(double throughput)
        {
            return tuning.ProbeLossTolerancePercent > 0 && unlocked && target > settings.Soft && target <= settings.Plateau
                && throughput >= probeBaseline * (1 - (tuning.ProbeLossTolerancePercent / 100));
        }

        // Where a rejected probe takes the target: back to the level it came from, or, with a rejection shrink, that much
        // lower than the raised target, down to the throughput floor (the probe floor on a fast link). A rejection only
        // ever lowers load: never above the level the probe came from, which was within the ceiling then in force, even
        // when the floor is above that level (the target started, or failures took it, below the floor).
        private int RejectedProbeTarget()
        {
            if (!tuning.ProbeRejectionShrinkPercent.HasValue)
            {
                return probeFrom;
            }
            int floor = Math.Min(settings.Max, Math.Max(settings.ThroughputFloor, FastLink ? settings.ProbeFloor : 0));
            int next = (int)Math.Floor(target * (100 - tuning.ProbeRejectionShrinkPercent.Value) / 100);
            return Math.Min(probeFrom, Math.Max(floor, Math.Min(next, target - 1)));
        }

        // The gain, in percent, a probe from `from` to `to` parts at once must show. Parts added above the plateau must
        // each earn their own gain.
        private double RequiredGainPercent(int to, int from)
        {
            int addedAbovePlateau = to - Math.Max(from, settings.Plateau);
            return addedAbovePlateau > 0 ? Math.Max(tuning.ProbeGainPercent, addedAbovePlateau * tuning.ProbeGainPerAddedPartPercent) : tuning.ProbeGainPercent;
        }

        private void Throttled(long now, int attemptEpoch, TimeSpan? retryAfter, List<UploadTraceEvent> decisions)
        {
            TimeSpan pause = tuning.BackpressurePause;
            if (retryAfter.HasValue && retryAfter.Value > pause)
            {
                // The whole wait the server asks for. A storage 429 or 503 asking for longer than the SDK can wait is
                // not retried (RetryPolicy.CanWait), and the pause is then the longest that can be scheduled, so that
                // no other attempt starts before it ends, and its end can neither overflow nor fail to be scheduled.
                pause = retryAfter.Value <= RetryPolicy.MaxWait ? retryAfter.Value : RetryPolicy.MaxWait;
            }
            long until = now + (long)(pause.TotalSeconds * TicksPerSecond);
            if (until > pausedUntil)
            {
                pausedUntil = until;
            }

            int previous = target;
            bool lowered = attemptEpoch == epoch && Lower(tuning.BackpressureShrinkPercent);
            // Measuring starts again when the pause ends.
            Restart(now);
            Decide(now, ConcurrencyDecision.CooldownStarted, previous, decisions, cooldown: pause);
            if (lowered)
            {
                Decide(now, ConcurrencyDecision.ShrankAfterThrottling, previous, decisions);
            }
        }

        // A judged window of the warm start: a fast link goes to the policy's start. While resting, the policy's hold and
        // growth interval pass first, and the last window is the one the target rises from. Otherwise the start target's
        // first window only warms the connection it opened, and its second judges it, as the baseline the target rises
        // from; a raised target is judged by its first settled window (SettledWindow), against the window before the raise
        // and the fastest window judged at the target it came from: kept, and the target raised again, when the parts
        // added delivered at least WarmGainPerPartPercent of what each part delivered before them and fewer than that share
        // of them were queued (Queued), and otherwise going back to the target before, to rest there. A window whose
        // requests were queued, against the fastest window judged at its own target, raises nothing: the parts the next
        // raise would add could only wait as well, so the warm start rests.
        private void JudgeWarm(long now, double throughput, List<UploadTraceEvent> decisions)
        {
            if (throughput >= tuning.ProbeFloorBytesPerSecond)
            {
                EndWarmStart(now, ConcurrencyDecision.WarmStartFastLink, decisions, throughput);
                return;
            }
            if (resting)
            {
                // The target holds while the warm start rests, as growth holds after a withdrawn raise, so the part size
                // may change meanwhile: a size probe's windows judge the size alone and leave the rest where it was, and
                // only the rest's hold windows may start one, never the window that ends the rest. A scope several
                // uploads share judges a file's sizes by its own parts only (AddToSizeWindow), so its windows, which
                // hold every file's parts, neither judge nor mature them.
                if (attachedSizing.Count == 0)
                {
                    if (sizing != null && sizing.Probing)
                    {
                        JudgeSizeProbe(now, throughput, decisions);
                        return;
                    }
                    levelThroughputSum += throughput;
                    levelWindows++;
                    sizing?.WindowJudged();
                }
                JudgedAtTarget();
                if (holdWindows > 0)
                {
                    holdWindows--;
                    TryGrowParts(now, throughput, decisions);
                    return;
                }
                if (++windowsTowardGrowth < tuning.GrowthIntervalWindows)
                {
                    return;
                }
                resting = false;
                if (Queued(levelLowest, WarmStep()))
                {
                    Rest();
                    return;
                }
            }
            else if (!settling)
            {
                // The start target's first window only warms the connection it opened: it is neither the baseline nor
                // the fastest window the next raise is compared with.
                if (++warmWindows < 2)
                {
                    return;
                }
                JudgedAtTarget();
                if (Queued(levelLowest, WarmStep()))
                {
                    Rest();
                    return;
                }
            }
            else if (!SettledWindow())
            {
                return;
            }
            else
            {
                CountVariation();
                int added = target - warmFrom;
                bool queued = Queued(fromLowest, added);
                if (throughput < warmLevel * (1 + (WarmGainPerPartPercent / 100 * added / warmFrom)) || queued)
                {
                    int previous = target;
                    double baseline = warmLevel;
                    target = warmFrom;
                    shrinks++;
                    epoch++;
                    Restart(now);
                    Rest();
                    Decide(now, ConcurrencyDecision.WarmStartSettled, previous, decisions, throughput, baseline);
                    return;
                }
                // Kept: the window is the level the next raise is judged against.
                restedSinceRaise = false;
                levelLowest = lastMedianPerByte;
            }
            warmLevel = throughput;
            warmFrom = target;
            WarmRaise(now, decisions, throughput);
        }

        // A window the warm start judged at its target: it may be the fastest there, which the target's next raise is
        // compared with, and its answers' times count toward their variation.
        private void JudgedAtTarget()
        {
            if (lastMedianPerByte > 0)
            {
                levelLowest = levelLowest > 0 ? Math.Min(levelLowest, lastMedianPerByte) : lastMedianPerByte;
            }
            CountVariation();
        }

        // Adds the variation of the answers' times of the window just closed, about their mean, to the warm start's.
        private void CountVariation()
        {
            if (closedPerByte.Count >= 2)
            {
                variationSquares += RelativeSquares(closedPerByte);
                variationAnswers += closedPerByte.Count - 1;
            }
        }

        // The squares of the values less their mean, relative to that mean; 0 where they are all the same.
        private static double RelativeSquares(List<double> values)
        {
            double mean = 0;
            foreach (double value in values)
            {
                mean += value / values.Count;
            }
            double squares = 0;
            foreach (double value in values)
            {
                squares += (value - mean) * (value - mean);
            }
            return squares / (mean * mean);
        }

        // Inflate the observed relative variance when a small sample could understate variation. The
        // Wilson-Hilferty approximation supplies the inflation factor; correlated, non-normal response times and
        // median comparisons make this a heuristic, not a confidence bound. A single degree of freedom is unbounded;
        // identical answers require no inflation. The private replay comparison supporting this is in the review notes.
        private static double SmallSampleVariation(double squares, int freedom)
        {
            if (squares <= 0)
            {
                return 0;
            }
            double spread = 2.0 / (9 * freedom);
            double root = 1 - spread - (WarmWindowMargin * Math.Sqrt(spread));
            return root <= 0 ? double.PositiveInfinity : squares / (freedom * root * root * root);
        }

        // The median of the values, sorted in place; zero for none.
        private static double Median(List<double> values)
        {
            if (values.Count == 0)
            {
                return 0;
            }
            values.Sort();
            int middle = values.Count / 2;
            return values.Count % 2 == 1 ? values[middle] : (values[middle - 1] + values[middle]) / 2;
        }

        // Whether the window's requests were queued, against reference, the latency of the fastest window judged at the
        // target compared with (0 for none, never queued), and records the queue estimate for the decision's trace
        // (warmQueue): the estimate, from how much longer each byte took than in that window, is the work of
        // WarmGainPerPartPercent of the given parts or more. That is the same share that each part added must deliver: on
        // a link that the parts added could not use, they and the parts already sent wait for each other, whether or not
        // the link delivered more than before for other reasons, such as a slow connection finishing or the link warming
        // up.
        private bool Queued(double reference, int parts)
        {
            warmQueue = reference > 0 && lastMedianPerByte > 0 ? lastAverageInFlight * (1 - (reference / lastMedianPerByte)) : 0;
            return warmQueue >= WarmGainPerPartPercent / 100 * parts;
        }

        // The parts the warm start's next raise adds: half again as many as the target, rounded up (so one from 1 and from
        // 2), or one after it rested.
        private int WarmStep()
        {
            return restedSinceRaise ? 1 : Math.Max(1, (target + 1) / 2);
        }

        // Extra answers needed to judge a warm window, beyond the target's full round. Cold and unsettled windows
        // close early to let a settled window open. Judged windows need at least two answers; the variance heuristic
        // asks for m >= 2 v (raised * margin / (0.5 * parts added))^2, bounded by the policy window. Under independent
        // mean comparisons this separates the queue threshold from sampling variation. Actual responses are correlated
        // and compared by medians, so it only adapts sample size; it promises no confidence level.
        private int WarmWindowAnswers()
        {
            bool judged = settling ? settleLast > 0 && settleLeft == 0 && windowStart >= settledAt : warmWindows > 0;
            if (!judged)
            {
                return 0;
            }
            double squares = variationSquares;
            int freedom = variationAnswers;
            if (windowPerByte.Count >= 2)
            {
                squares += RelativeSquares(windowPerByte);
                freedom += windowPerByte.Count - 1;
            }
            int parts = settling ? target - warmFrom : WarmStep();
            int raised = settling ? target : target + parts;
            double ratio = raised * WarmWindowMargin / (WarmGainPerPartPercent / 100 * parts);
            double needed = freedom == 0 ? 2 : Math.Max(2, Math.Ceiling(2 * SmallSampleVariation(squares, freedom) * ratio * ratio));
            return (int)Math.Min(settings.WindowAttempts, needed);
        }

        // Whether the window just closed may judge the raised target: it opened once every attempt in flight when the
        // target first filled had ended, or, whether or not they have, the target has had the answers of a window of the
        // policy's size since the raise.
        private bool SettledWindow()
        {
            return (settleLast > 0 && settleLeft == 0 && closedWindowStart >= settledAt) || settleAnswers >= Math.Max(settings.WindowAttempts, target);
        }

        // The warm start rests where its target is: windows of the policy's size, its HoldWindows and then its
        // GrowthIntervalWindows of them (Restart starts both counts over), before it tries one part more again. The level a
        // size probe of the rest starts from, and the fastest window its last window is compared with, are measured in the
        // rest alone.
        private void Rest()
        {
            resting = true;
            settling = false;
            restedSinceRaise = true;
            warmWindows = 0;
            warmLevel = 0;
            levelLowest = 0;
            holdWindows = tuning.HoldWindows;
            windowsTowardGrowth = 0;
            levelThroughputSum = 0;
            levelWindows = 0;
        }

        // Raises the warm start's target by WarmStep, up to the policy's start, where the warm start is over. The raised
        // target is judged against the fastest window judged at the target it rose from (fromLowest).
        private void WarmRaise(long now, List<UploadTraceEvent> decisions, double throughput)
        {
            int step = WarmStep();
            if (target + step >= settings.Start)
            {
                EndWarmStart(now, ConcurrencyDecision.WarmStartRaised, decisions, throughput);
                return;
            }
            int previous = target;
            target += step;
            fromLowest = levelLowest;
            levelLowest = 0;
            grows++;
            settling = true;
            settleLast = 0;
            settleLeft = 0;
            settleAnswers = 0;
            OpenWindow(now);
            Decide(now, ConcurrencyDecision.WarmStartRaised, previous, decisions, throughput);
        }

        // Ends the warm start at the policy's start, from which the policy carries on as it would have, and where failures
        // lower the target only as far as the policy's minimum. A size probe of a rest, measured at the rest's target, is
        // over, and the size goes back, as after a failure.
        private void EndWarmStart(long now, ConcurrencyDecision decision, List<UploadTraceEvent> decisions, double throughput)
        {
            warming = false;
            resting = false;
            settling = false;
            int previous = target;
            target = settings.Start;
            minTarget = settings.Min;
            grows++;
            Restart(now);
            Decide(now, decision, previous, decisions, throughput);
            sizing?.Interrupt(now, decisions);
        }

        // Lowers the target by percent, by at least one part, but not below floor (the lowest target unless given).
        private bool Lower(double percent, int floor = 0)
        {
            floor = Math.Max(floor, minTarget);
            if (percent <= 0 || target <= floor)
            {
                return false;
            }
            int next = (int)Math.Floor(target * (100 - percent) / 100);
            target = Math.Max(floor, Math.Min(next, target - 1));
            epoch++;
            shrinks++;
            return true;
        }

        // After the target was lowered or a probe withdrawn: the level starts over and growth holds.
        private void Restart(long now)
        {
            holdingAtPlateau = false;
            probeFrom = 0;
            levelThroughputSum = 0;
            levelWindows = 0;
            windowsTowardGrowth = 0;
            holdWindows = tuning.HoldWindows;
            OpenWindow(now);
            if (attachedSizing.Count > 0)
            {
                ResetSizeLevel(now);
            }
        }

        private void OpenWindow(long now)
        {
            windowStart = Math.Max(now, pausedUntil);
            windowAttempts = 0;
            windowBytes = 0;
            windowAttemptSeconds = 0;
            windowPerByte.Clear();
            windowInFlightTicks = 0;
            Array.Clear(windowTicksBelowTarget, 0, windowTicksBelowTarget.Length);
        }

        private void Decide(long now, ConcurrencyDecision decision, int previousTarget, List<UploadTraceEvent> decisions, double throughput = 0, double baseline = 0, BelowTargetReason reason = BelowTargetReason.None, TimeSpan cooldown = default(TimeSpan))
        {
            lastDecision = decision;
            decisions?.Add(new UploadDecisionTrace(path, Duration(now - startedAt))
            {
                Decision = decision,
                PreviousTarget = previousTarget,
                Target = target,
                BelowTargetReason = reason,
                ThroughputBytesPerSecond = throughput,
                BaselineBytesPerSecond = baseline,
                // A warm start's raise, or a raise withdrawn, by the queue estimate it was judged on (Queued).
                QueueEstimate = decision == ConcurrencyDecision.WarmStartRaised || decision == ConcurrencyDecision.WarmStartSettled ? warmQueue : queueEstimate,
                Cooldown = cooldown,
            });
        }

        private static TimeSpan Duration(long stopwatchTicks)
        {
            return TimeSpan.FromSeconds(stopwatchTicks / TicksPerSecond);
        }
    }
}