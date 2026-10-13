using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    using FilesCom.Http;
    using FilesCom.Models;
    using FilesCom.Util;

    // Runs one upload. First the source starts (a stream to spool is copied), and the first begin_upload answer says
    // what the upload may do: it settles the performance profile, and with it the concurrency controller and the private
    // limiter, and how parts are sized, all before any part is read. Then parts flow through three stages, each waiting
    // for one resource at a time:
    //
    //   PreparePartsAsync   waits for room among the parts read ahead (MaxReadyParts), plans the next part, so that
    //                       its size follows what the upload has measured until then, waits for room for its buffer
    //                       (MaxReadyBytes), then for buffer space from the limiter, reads the part and queues it;
    //   DispatchPartsAsync  takes queued parts in order, waits for PartAdmission (the server's parallel_parts and the
    //                       upload's maximum), then admits the part's first attempt, and starts the part;
    //   SendPartAsync       one task per part: sends it, retrying when allowed, and releases what it held.
    //
    // Every network attempt, first or retry, is admitted by AttemptAdmission, which follows the concurrency
    // controller's target and cooldown, and then takes a request slot from the limiter for as long as its request
    // lasts. It starts only if admission still allows it once it has the slot (StartAttemptAsync). Each attempt's
    // outcome is measured once, as its request ends and before any retry delay.
    //
    // When every part has been accepted, the upload is completed with a single final request, with the size of what was
    // sent. The first failure (or the caller's cancellation) stops both loops and cancels the parts in flight; every part
    // task has ended and released its resources before RunAsync returns, and nothing more is sent.
    //
    // A stream of unknown length is read one part at a time, in order, and only into buffers the limiter has been
    // charged for. Where the upload chooses its part sizes (UploadGrant), its parts are sent as a file's are, several at
    // once where allowed, and sized within its bound; otherwise they are also sent one at a time, in order, whatever the
    // server allows, as Files.com sizes them, within MaxStreamLength and available_parts.
    //
    // A file upload with a checkpoint (UploadCheckpointing) records its reference, its grant, every part as it is
    // planned and, for each part Files.com accepts, its ETag and the SHA-256 of the bytes sent, at most every
    // CheckpointInterval and whenever it stops. A later run of the same upload continues it: it first reads the parts
    // Files.com accepted from the file again, and continues only if every one still has the bytes that were sent; then
    // the parts planned and not accepted are sent again exactly as planned, the rest is planned as usual, and accepted
    // parts are never sent again. When the upload cannot be continued (the file changed, Files.com no longer knows the
    // upload, or its grant is not the one the upload started with) the run fails with TransferCheckpointException, or,
    // only when the caller chose CheckpointRestartPolicy.StartOver, starts over with a new upload: parts of two uploads
    // are never completed together. That the final request is being sent is recorded, durably, before it is sent; a run
    // that finds it recorded does not send it again (UploadCompletionUnknownException), whatever the policy.
    internal sealed class UploadCoordinator : IDisposable
    {
        // How often, at most, an upload records its progress in its checkpoint while it runs.
        private static readonly TimeSpan CheckpointInterval = TimeSpan.FromSeconds(2);

        // A part whose URL expires sooner than this gets a new one before it is sent.
        private static readonly TimeSpan RenewUrlsExpiringWithin = SignedUrl.RenewalMargin;

        private static readonly SdkLog log = SdkLog.For(typeof(UploadCoordinator));

        private readonly OperationContext context;
        private readonly UploadSource source;
        private readonly UploadOptions options;
        private readonly UploadSession session;
        private readonly UploadProgressTracker progress;
        private readonly ReadyParts ready;
        private PartPlanner planner;
        private AttemptAdmission attempts;
        // Whether attempts is the scope a directory upload's automatic files share (TransferScopes), which this upload
        // neither disposes nor makes serial for the others.
        private bool sharedAttempts;
        // This upload's own size controller, when it chooses its part sizes: its parts' samples carry it, and a shared
        // scope judges sizes for it while it is the scope's first attached file (AttemptAdmission.AttachSizing).
        private PartSizeController sizer;
        // In a shared scope, why this upload itself must send one part at a time (UploadSerialReason), or None.
        private int ownSerial;
        private ServerPlannedReason sizingFallback;
        private readonly CancellationToken cancellationToken;
        private readonly CancellationTokenSource stop;
        private readonly List<Task> partTasks = new List<Task>();
        private readonly SortedDictionary<long, UploadedPart> acceptedParts = new SortedDictionary<long, UploadedPart>();
        private ExceptionDispatchInfo failure;
        private PartAdmission admission;
        private readonly UploadCheckpointing checkpointing;
        private readonly SortedDictionary<long, PartDescriptor> plannedParts = new SortedDictionary<long, PartDescriptor>();
        private bool sizesChosen;
        private int resumedParts;
        private long resumedBytes;
        private DateTimeOffset lastCheckpoint;
        // The caller's file as the upload started, when source is one (FileUploadSource.IsLocalFile): its length and
        // modification time, which CheckSourceUnchanged compares before the upload is completed.
        private readonly long? sourceLength;
        private readonly DateTime? sourceLastWrite;

        private UploadCoordinator(OperationContext context, string path, UploadSource source, UploadOptions options, UploadCheckpointing checkpointing, CancellationToken cancellationToken)
        {
            this.context = context;
            this.source = source;
            this.options = options;
            if (source is FileUploadSource file && file.IsLocalFile)
            {
                sourceLength = file.Length;
                sourceLastWrite = file.LastWriteTimeUtc;
            }
            this.checkpointing = checkpointing;
            this.cancellationToken = cancellationToken;
            stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            session = new UploadSession(context, path, options.RequestOptions, options.MkdirParents);
            progress = new UploadProgressTracker(options.Progress, options.Trace, path, options.ConcurrencyMode, options.AutomaticConcurrency != null);
            ready = new ReadyParts(options.MaxReadyParts, options.MaxReadyBytes, progress);
        }

        private TransferLimiter Limiter => options.Limiter;

        // Uploads source to path. options must be a snapshot with its limiter resolved. checkpointing, for a file upload
        // with a checkpoint, is where it records its progress and what an earlier run of it recorded.
        internal static async Task<UploadResult> RunAsync(OperationContext context, string path, UploadSource source, UploadOptions options, CancellationToken cancellationToken, UploadCheckpointing checkpointing = null)
        {
            using (var coordinator = new UploadCoordinator(context, path, source, options, checkpointing, cancellationToken))
            {
                return await coordinator.RunAsync();
            }
        }

        // Uploads the opened local file source, localPath, to path: sent with the file's modification time unless the
        // options give one, and continuing the upload its checkpoint records, if it has one.
        internal static Task<UploadResult> RunFileAsync(OperationContext context, string path, FileUploadSource source, string localPath, UploadOptions options, CancellationToken cancellationToken)
        {
            DateTime lastWrite = source.LastWriteTimeUtc;
            options.ModifiedTime = options.ModifiedTime ?? lastWrite;
            UploadCheckpointing checkpointing = options.CheckpointPath == null ? null
                : UploadCheckpointing.Open(options.CheckpointPath, context, options.RequestOptions, path, localPath, source.Length.Value, lastWrite);
            return RunAsync(context, path, source, options, cancellationToken, checkpointing);
        }

        // Fails the upload of a caller's file that is no longer the file the upload started with: longer or shorter, or
        // written since, as a program appending to a log or rewriting an export while it uploads makes it. The parts
        // sent hold the file as it was read, part by part, so completing the upload would put a file on Files.com that
        // is neither the old one nor the new one. Nothing is sent again: the file has to be uploaded again once nothing
        // writes it. On Windows the upload's handle keeps other programs from writing the file, so this never fails
        // there. A write within the file system's time resolution that keeps the length is not seen.
        private void CheckSourceUnchanged()
        {
            if (!(source is FileUploadSource file) || !file.IsLocalFile)
            {
                return;
            }
            long length = file.CurrentLength;
            DateTime written = file.LastWriteTimeUtc;
            if (length != sourceLength || written != sourceLastWrite)
            {
                throw new System.IO.IOException($"The file changed while it was being uploaded: it was {sourceLength} bytes, last written {sourceLastWrite:u}, when the upload started, and is {length} bytes, last written {written:u}, now. The upload to {session.Path} was not completed; upload the file again once nothing is writing it.");
            }
        }

        public void Dispose()
        {
            if (!sharedAttempts)
            {
                attempts?.Dispose();
            }
            else if (sizer != null)
            {
                attempts.DetachSizing(sizer);
            }
            stop.Dispose();
        }

        // A shared scope's state as this upload reports it: the scope's target, attempts and decisions (Shared), with this
        // upload's own limit and reason while it must send one part at a time.
        private UploadConcurrency SharedSnapshot()
        {
            UploadConcurrency snapshot = attempts.Snapshot();
            snapshot.Shared = true;
            var serial = (UploadSerialReason)Volatile.Read(ref ownSerial);
            if (serial != UploadSerialReason.None)
            {
                snapshot.Limit = 1;
                snapshot.SerialReason = serial;
            }
            return snapshot;
        }

        private async Task<UploadResult> RunAsync()
        {
            await source.StartAsync(progress, Limiter, stop.Token);
            long? length = source.Length;
            progress.LengthKnown(length);
            planner = new PartPlanner(session, length, options.MaxStreamLength);
            UploadGrant grant = checkpointing?.Saved != null ? await ResumeAsync(length.Value) : null;
            PartSizes sizes = null;
            if (grant != null)
            {
                options.UseDestination(grant.Profile);
                sizes = ChooseSizes(grant, length);
                if ((sizes != null) != checkpointing.Saved.SizesChosen)
                {
                    grant = null;
                    CannotContinue(TransferCheckpointProblem.GrantChanged, "Files.com no longer lets its part sizes be chosen as they were");
                }
            }
            if (grant == null)
            {
                grant = await planner.StartAsync(stop.Token);
                options.UseDestination(grant.Profile);
                sizes = ChooseSizes(grant, length);
            }
            sizesChosen = sizes != null;
            planner.Use(sizes, options.Concurrency.Max);
            SaveCheckpoint(force: true);
            // A directory upload's automatic files share one scope: its target is learned from all of their parts, so a
            // file of one part still teaches the next, and they share that target rather than each having all of it.
            // A file that chooses its part sizes keeps that feedback: it attaches its size controller, and the scope
            // judges one attached file's sizes at a time, in order, by that file's own parts, so that no two files probe
            // at once. Decisions the scope makes are traced once, by the file whose attempt made them, under the scope's
            // path. A file the server makes send its parts one at a time is held to that by its own PartAdmission, and
            // reports it itself (SharedSnapshot), without making the other files serial.
            sizer = sizes?.Controller;
            // An upload warm-starts when the SDK chose the scaled S3 policy for it and its Startup is the default
            // (ConcurrencySettings.WarmStarts): with its own controller, or, for a directory upload's files, once, in the
            // scope they share.
            TransferScope shared = options.SharedScopes != null && options.Concurrency.Adaptive ? options.SharedScopes.For(options.Concurrency, progress.Tracing, options.Concurrency.WarmStarts) : null;
            sharedAttempts = shared != null;
            attempts = shared?.Admission ?? new AttemptAdmission(new ConcurrencyController(options.Concurrency, session.Path, progress.StartedAt, sizer, options.Concurrency.WarmStarts), progress.Tracing);
            if (sharedAttempts && sizer != null)
            {
                attempts.AttachSizing(sizer);
            }
            progress.Observe(sharedAttempts ? (Func<UploadConcurrency>)SharedSnapshot : attempts.Snapshot, Sizing);
            admission = new PartAdmission(options.Concurrency.Max, (length.HasValue || sizes != null) && planner.FirstPartParallel);
            if (!length.HasValue && sizes == null)
            {
                UseSerial(UploadSerialReason.LengthUnknown);
            }
            else if (!admission.Parallel)
            {
                UseSerial(UploadSerialReason.ServerRequiresSerialParts);
            }
            if (sizes != null)
            {
                progress.Trace(sizes.StartTrace(session.Path, progress.Elapsed));
            }
            progress.Report();

            try
            {
                await Task.WhenAll(StopOnFailure(PreparePartsAsync(stop.Token)), StopOnFailure(DispatchPartsAsync(stop.Token)));
                // Only DispatchPartsAsync adds part tasks, and it has ended. Part tasks never throw.
                await Task.WhenAll(partTasks);
            }
            finally
            {
                foreach (PreparedPart unsent in ready.TakeRemaining())
                {
                    unsent.Dispose();
                }
            }
            if (failure != null || cancellationToken.IsCancellationRequested)
            {
                // Every part has ended: what the checkpoint records is settled. The run's own failure or cancellation is
                // what the caller sees.
                SaveCheckpointAfterStop();
            }
            failure?.Throw();
            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<UploadedPart> parts = AcceptedParts();
            long size = planner.PlannedBytes;
            // A caller's file that was written while its parts were sent is not completed: what was sent is not the file.
            CheckSourceUnchanged();
            // Recorded, durably, before the request is sent: a run that ends while it is in flight must not complete the
            // upload again as if it had never been sent. Not recorded durably, it is not sent.
            try
            {
                SaveCheckpoint(force: true, finalRequestSent: true);
            }
            catch (System.IO.IOException e)
            {
                throw new System.IO.IOException($"The upload to {session.Path} was not completed: its final request was not sent, because the checkpoint could not record durably that it was being sent. If the checkpoint now records it as sent, a later run will not send it; as it was not sent, deleting the checkpoint and uploading again is safe. {e.Message}", e);
            }
            RemoteFile file;
            try
            {
                file = await session.FinishAsync(size, parts, options.ModifiedTime, cancellationToken);
            }
            catch (Exception e) when (!(e is UploadCompletionUnknownException) && !(e is OperationCanceledException))
            {
                // Refused: Files.com handled the request and did not complete the upload. The refusal is what the caller
                // sees.
                SaveCheckpointAfterStop();
                throw;
            }
            checkpointing?.Delete();
            // From the planner, which also sees URLs renewed after their parts were admitted.
            return new UploadResult(session.Path, size, file, parts, planner.EveryPartParallel, progress.Snapshot(), resumedParts, resumedBytes);
        }

        // Continues the upload the checkpoint records, and returns this run's grant; or, when it cannot be continued and
        // the caller chose to start over, returns null, having made a new planner. The file is checked first, before
        // anything is sent: its length, and the bytes of every part Files.com accepted, read again and compared with the
        // SHA-256 of the bytes that were sent. Its modification time proves nothing either way.
        private async Task<UploadGrant> ResumeAsync(long length)
        {
            UploadCheckpoint saved = checkpointing.Saved;
            List<PartDescriptor> planned = saved.PlannedParts();
            string changed = saved.SourceLength != length ? $"the file now has {length} bytes, not {saved.SourceLength}" : await ChangedPartAsync(saved, planned);
            if (changed != null)
            {
                CannotContinue(TransferCheckpointProblem.SourceChanged, changed);
                return null;
            }
            var accepted = new HashSet<long>(saved.Accepted.Select(part => part.Part));
            UploadGrant grant;
            try
            {
                grant = await planner.ResumeAsync(saved.Ref, planned, accepted, stop.Token);
            }
            catch (FileUploadNotFoundException e)
            {
                log.Debug($"Files.com no longer knows the upload to {session.Path} its checkpoint records", e);
                CannotContinue(TransferCheckpointProblem.UploadExpired, "Files.com no longer knows the upload it records, which expired or was cancelled; it was not completed");
                return null;
            }
            PartSizeLimits limits = grant.Limits;
            // An upload an earlier SDK sent with part offsets cannot be continued without them. One that chose its sizes
            // must still have the same limits and go to the same S3 upload, which a checkpoint of an earlier SDK does not
            // record; one sent with Files.com's sizes goes on with them, each part's size checked again as it is sent.
            bool sameLimits = !saved.SizesChosen || (saved.Limits != null && limits != null && limits.MinNonfinalBytes == saved.Limits[0]
                && limits.MaxPartBytes == saved.Limits[1] && limits.MaxParts == saved.Limits[2] && limits.MaxFileBytes == saved.Limits[3]);
            bool sameRoute = !saved.SizesChosen || (saved.Route != null && saved.Route == grant.Route?.Digest);
            if (!sameLimits || saved.PartOffsets || !sameRoute || planner.AvailableParts != saved.AvailableParts)
            {
                CannotContinue(TransferCheckpointProblem.GrantChanged, "Files.com now gives the upload other part limits, part counts or storage than it started with");
                return null;
            }
            // The same limits do not show that the parts were planned within them: before any part is prepared, each one
            // recorded, which is sent again as it is, must be.
            string outside = saved.SizesChosen ? saved.PartsOutside(limits, limits.PartsAllowed(planner.AvailableParts)) : null;
            if (outside != null)
            {
                CannotContinue(TransferCheckpointProblem.GrantChanged, outside);
                return null;
            }
            foreach (PartDescriptor part in planned)
            {
                plannedParts[part.Number] = part;
            }
            foreach (AcceptedPart part in saved.Accepted)
            {
                PartDescriptor descriptor = plannedParts[part.Part];
                acceptedParts[part.Part] = new UploadedPart(part.Part, descriptor.Offset, descriptor.Length, part.ETag, 0, part.Sha256);
                progress.PartAccepted(descriptor.Length);
                resumedParts++;
                resumedBytes += descriptor.Length;
            }
            return grant;
        }

        // Why the file is no longer what the upload was sending, or null when every accepted part still has the bytes
        // that were sent. A part recorded without their SHA-256 cannot be checked, which counts as changed. The parts
        // are read through one buffer, held until they have been read, from the caller's limiter's buffer space when the
        // caller gave one. Without one, the upload's own limiter is made only once Files.com says how many requests it may
        // send; nothing else can use its space, and the buffer is well within its default space.
        private async Task<string> ChangedPartAsync(UploadCheckpoint saved, List<PartDescriptor> planned)
        {
            if (saved.Accepted.Count == 0)
            {
                return null;
            }
            var file = (FileUploadSource)source;
            Dictionary<long, PartDescriptor> byNumber = planned.ToDictionary(part => part.Number);
            long bufferBytes = ReadBufferBytes(Limiter, saved.SourceLength);
            using (Limiter == null ? null : await progress.TimeBufferWait(Limiter.ReserveBufferAsync(bufferBytes, stop.Token)))
            {
                byte[] buffer = new byte[bufferBytes];
                foreach (AcceptedPart part in saved.Accepted)
                {
                    PartDescriptor descriptor = byNumber[part.Part];
                    if (string.IsNullOrEmpty(part.Sha256))
                    {
                        return $"part {part.Part} was recorded without the SHA-256 of its bytes, so the file cannot be shown to be unchanged";
                    }
                    bool? same = await HasBytesAsync(file, descriptor.Offset, descriptor.Length, part.Sha256, buffer, stop.Token);
                    if (same == null)
                    {
                        return $"the file ended within part {part.Part}";
                    }
                    if (same == false)
                    {
                        return $"the bytes of part {part.Part}, which Files.com accepted, are not the bytes that were sent";
                    }
                }
            }
            return null;
        }

        // The buffer a file's parts are read again through, to compare them with what was sent: no larger than the
        // limiter's buffer space, if there is a limiter, or than the file.
        internal static long ReadBufferBytes(TransferLimiter limiter, long fileBytes)
        {
            return Math.Min(Math.Min(FileUploadSource.CopyBufferSize, limiter?.MaxBufferedBytes ?? long.MaxValue), Math.Max(1, fileBytes));
        }

        // Whether the length bytes of file from offset have the SHA-256 sha256, read through buffer; null when the file
        // ends before them.
        internal static async Task<bool?> HasBytesAsync(FileUploadSource file, long offset, long length, string sha256, byte[] buffer, CancellationToken token)
        {
            using (IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            {
                for (long read = 0; read < length;)
                {
                    int count = await file.ReadAsync(offset + read, buffer, (int)Math.Min(buffer.Length, length - read), token);
                    if (count == 0)
                    {
                        return null;
                    }
                    hash.AppendData(buffer, 0, count);
                    read += count;
                }
                return string.Equals(Hex(hash.GetHashAndReset()), sha256, StringComparison.OrdinalIgnoreCase);
            }
        }

        // The upload the checkpoint records cannot be continued: fails, or starts over when the caller chose that.
        private void CannotContinue(TransferCheckpointProblem problem, string why)
        {
            if (options.CheckpointRestartPolicy != CheckpointRestartPolicy.StartOver)
            {
                throw new TransferCheckpointException(checkpointing.CheckpointPath, problem,
                    $"{why}. To upload the file again as a new upload, delete the checkpoint, or set UploadOptions.CheckpointRestartPolicy to StartOver");
            }
            StartOver(why);
        }

        internal static string Hex(byte[] bytes)
        {
            return bytes == null ? null : BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        // Records the settled state after the run stopped with a failure, cancellation or refusal of its own, which is
        // what the caller sees: a failure to record it too is logged, and the checkpoint keeps what it last recorded.
        private void SaveCheckpointAfterStop()
        {
            try
            {
                SaveCheckpoint(force: true);
            }
            catch (Exception e)
            {
                DiagnosticText.WarnCleanupFailed(log, "Recording the checkpoint of a stopped upload failed; the checkpoint keeps what it last recorded", e);
            }
        }

        // Discards what the checkpoint records and what this run did with it, so the upload starts over as a new one.
        private void StartOver(string why)
        {
            log.Info("Starting an upload over: the upload its checkpoint records cannot be continued");
            log.Debug($"The upload to {session.Path} its checkpoint records cannot be continued: {why}");
            planner = new PartPlanner(session, source.Length, options.MaxStreamLength);
            plannedParts.Clear();
            acceptedParts.Clear();
            resumedParts = 0;
            resumedBytes = 0;
            progress.Restart();
        }

        // Records the upload's state in its checkpoint, if it has one, at most every CheckpointInterval unless forced.
        private void SaveCheckpoint(bool force, bool finalRequestSent = false)
        {
            if (checkpointing == null || session.Ref == null || planner?.Grant == null)
            {
                return;
            }
            lock (plannedParts)
            {
                if (!force && DateTimeOffset.UtcNow - lastCheckpoint < CheckpointInterval)
                {
                    return;
                }
                PartSizeLimits limits = planner.Grant.Limits;
                List<AcceptedPart> accepted;
                lock (acceptedParts)
                {
                    accepted = acceptedParts.Values.Select(part => new AcceptedPart { Part = part.Number, ETag = part.ETag, Sha256 = part.Sha256 }).ToList();
                }
                checkpointing.Write(new UploadCheckpoint
                {
                    Ref = session.Ref,
                    Limits = limits == null ? null : new[] { limits.MinNonfinalBytes, limits.MaxPartBytes, limits.MaxParts, limits.MaxFileBytes },
                    AvailableParts = planner.AvailableParts,
                    Route = sizesChosen ? planner.Grant.Route?.Digest : null,
                    SizesChosen = sizesChosen,
                    Planned = plannedParts.Values.Select(part => new[] { part.Number, part.Offset, part.Length, part.IsLast ? 1L : 0L }).ToList(),
                    Accepted = accepted,
                    FinalRequestSent = finalRequestSent,
                });
                lastCheckpoint = DateTimeOffset.UtcNow;
            }
        }

        // How the upload sizes its parts: null for the sizes the server plans. Sizes are chosen only where the parts go
        // straight to Amazon S3 (UploadGrant) and the caller has not asked for the server's; the plan is proved possible
        // here, before anything is read or sent. Parts held in memory are kept within the limiter's buffer space; a
        // stream of unknown length whose bound cannot fit in parts that small uses the server's sizes instead. So does a
        // file larger than the largest the limits plan for, or one they cannot fit in the parts available (PartSizes.Plan):
        // the SDK's own limits do not refuse a file Files.com might accept, unless the caller fixed the part size.
        private PartSizes ChooseSizes(UploadGrant grant, long? length)
        {
            if (grant.Limits == null || options.Sizing.Mode == UploadPartSizingMode.ServerPlanned)
            {
                return null;
            }
            if (length.HasValue && length.Value > grant.Limits.MaxFileBytes)
            {
                sizingFallback = ServerPlannedReason.NotAllowed;
                return null;
            }
            long? bufferLimit = source.HoldsWholeParts ? Limiter.MaxBufferedBytes : (long?)null;
            return PartSizes.Plan(grant.Limits, length, options.MaxStreamLength, planner.AvailableParts, options.Sizing, bufferLimit, session.Path, progress.StartedAt, out sizingFallback);
        }

        private UploadPartSizing Sizing()
        {
            var sizing = new UploadPartSizing
            {
                Mode = options.Sizing.Mode,
                Source = PartSizeSource.ServerPlanned,
                Limits = planner.Grant?.Limits,
            };
            if (planner.Sizes != null)
            {
                planner.Sizes.Describe(sizing, !planner.AllPlanned);
            }
            else
            {
                sizing.MaxStreamLength = planner.StreamBound;
                sizing.ServerPlannedReason = sizingFallback != ServerPlannedReason.None ? sizingFallback
                    : options.Sizing.Mode == UploadPartSizingMode.ServerPlanned && sizing.Limits != null ? ServerPlannedReason.Requested
                    : !source.Length.HasValue ? ServerPlannedReason.LengthUnknown
                    : ServerPlannedReason.NotAllowed;
                sizing.LargestPartSize = planner.LargestPartSize;
            }
            return sizing;
        }

        private async Task PreparePartsAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    IDisposable partRoom = await ready.WaitForPartRoomAsync(token);
                    PreparedPart part;
                    try
                    {
                        part = await PrepareNextAsync(partRoom, token);
                    }
                    catch
                    {
                        partRoom.Dispose();
                        throw;
                    }
                    if (part == null)
                    {
                        partRoom.Dispose();
                        break;
                    }
                    ready.Add(part);
                }
            }
            finally
            {
                ready.Finish();
            }
        }

        // Plans, and reads, the next part, or returns null when every part has been. partRoom is its room among the
        // parts read ahead, which the part takes over.
        private async Task<PreparedPart> PrepareNextAsync(IDisposable partRoom, CancellationToken token)
        {
            PartToRead next = await planner.NextAsync(token);
            if (next == null)
            {
                return null;
            }
            long bytes = source.BufferSizeFor(next.Part);
            if (bytes > Limiter.MaxBufferedBytes)
            {
                throw new UploadLimitException(
                    UploadLimit.BufferSpace,
                    $"Part {next.Part.Number} of the upload to {session.Path} is {(source.Length.HasValue ? "" : "up to ")}{next.Part.Length} bytes, more than the {Limiter.MaxBufferedBytes} bytes its transfer limiter may buffer. " +
                    "Allow more with TransferLimiter.MaxBufferedBytes, or upload from a file, which does not hold whole parts in memory.");
            }
            IDisposable byteRoom = await ready.WaitForByteRoomAsync(bytes, token);
            IDisposable reservation = null;
            PreparedBody body = null;
            try
            {
                reservation = await progress.TimeBufferWait(Limiter.ReserveBufferAsync(bytes, token));
                body = await source.PrepareAsync(next.Part, token);
                PlannedPart planned = planner.Freeze(next, body.Length, body.EndOfSource);
                if (planned == null)
                {
                    // The stream ended where the part before did: nothing is left to send.
                    progress.StreamEnded();
                    body.Body.Dispose();
                    reservation.Dispose();
                    byteRoom.Dispose();
                    return null;
                }
                progress.PartRead(planned.Part, source.Length.HasValue);
                lock (plannedParts)
                {
                    plannedParts[planned.Part.Number] = planned.Part;
                }
                return new PreparedPart(planned, body.Body, new Room(partRoom, byteRoom), reservation);
            }
            catch
            {
                body?.Body.Dispose();
                reservation?.Dispose();
                byteRoom.Dispose();
                throw;
            }
        }

        private async Task DispatchPartsAsync(CancellationToken token)
        {
            PreparedPart part;
            while ((part = await ready.TakeAsync(token)) != null)
            {
                PartAdmission.Admitted admitted = null;
                AttemptAdmission.Attempt attempt = null;
                try
                {
                    // Renewed here, before admission, a URL comes with the server's current permission to send the part
                    // alongside others, and admission follows it in part order.
                    await RenewIfExpiringAsync(part, token);
                    admitted = await admission.AdmitAsync(part.Target, token);
                    if (!admission.Parallel)
                    {
                        UseSerial(UploadSerialReason.ServerWithdrewParallelParts);
                    }
                    // Waiting here, rather than in the part's own task, keeps later parts behind this one and leaves
                    // the upload one request in the limiter's queue, so uploads sharing it take turns.
                    attempt = await AdmitAttemptAsync(retry: false, progress.Elapsed, token);
                    ready.PartStarted(part);
                    partTasks.Add(SendPartAsync(part, admitted, attempt));
                }
                catch
                {
                    attempt?.Dispose();
                    admitted?.Dispose();
                    part.Dispose();
                    throw;
                }
            }
        }

        // Waits for the upload's own admission, then for a request slot from the limiter. The attempt is not started.
        // This upload must send its parts one at a time: its own scope sends one at a time; in a shared scope, the upload
        // records its own reason (SharedSnapshot) and PartAdmission holds it to one part at a time, while the other files
        // keep their parallel parts.
        private void UseSerial(UploadSerialReason reason)
        {
            if (!sharedAttempts)
            {
                attempts.UseSerial(reason);
            }
            else
            {
                Interlocked.CompareExchange(ref ownSerial, (int)reason, (int)UploadSerialReason.None);
            }
        }

        private async Task<AttemptAdmission.Attempt> AdmitAttemptAsync(bool retry, TimeSpan askedAt, CancellationToken token)
        {
            AttemptAdmission.Attempt attempt = await progress.TimeAdmissionWait(attempts.AdmitAsync(retry, token));
            try
            {
                attempt.Hold(await progress.TimeRequestWait(Limiter.AcquireRequestAsync(token)), askedAt);
                return attempt;
            }
            catch
            {
                attempt.Dispose();
                throw;
            }
        }

        private async Task SendPartAsync(PreparedPart part, PartAdmission.Admitted admitted, AttemptAdmission.Attempt firstAttempt)
        {
            bool accepted = false;
            progress.PartStarted();
            try
            {
                UploadedPart uploaded = await SendWithRetriesAsync(part, admitted, firstAttempt, stop.Token);
                lock (acceptedParts)
                {
                    acceptedParts.Add(uploaded.Number, uploaded);
                }
                progress.PartAccepted(uploaded.Length);
                accepted = true;
            }
            catch (Exception e)
            {
                Fail(e);
            }
            finally
            {
                progress.PartEnded();
                part.Dispose();
                admitted.Dispose();
            }

            if (accepted)
            {
                try
                {
                    SaveCheckpoint(force: false);
                    progress.Report();
                }
                catch (Exception e)
                {
                    Fail(e);
                }
            }
        }

        // Sends the part until the server accepts it. A failed attempt is retried, with the same bytes and part number,
        // only when it may have been temporary, the server allows retrying the part and the client's retry limit has
        // not been reached. The wait before a retry is the storage wait of RetryPolicy: the server's whole Retry-After
        // after 429 or 503, or none without one, and 100 ms after other failures, and a Retry-After longer than the SDK
        // can wait makes the failure the part's instead; the retry is then admitted like any attempt, ahead of new
        // parts, so a cooldown after throttling holds it too, and a URL that expired meanwhile is renewed. A failure to
        // read the source is never retried. An attempt ends when the network stalls for longer than the client's
        // WriteTimeout or ReadTimeout, which is a failure that may be temporary, or when the caller's token is
        // cancelled.
        //
        // Each attempt that gets an answer, or fails on the network, is measured as it ends, before any retry delay:
        // its status, the network time SendUploadPart reports and the bytes it wrote. Cancellation and failures to read
        // the source are not measured.
        //
        // A part that storage refuses saying its URL has expired, even though the URL's own expiry is still to come,
        // gets a new URL for the same part of the same upload, with the same number, offset and length, and is sent
        // again at once, with the same bytes: at most SignedUrl.MaxExpiredRenewals times, only when the server allows
        // retrying parts, and without using any of the client's retries, since nothing was sent that storage kept. Any
        // other refusal, such as an ordinary 403, is the part's failure.
        //
        // firstAttempt, from the dispatcher, has been admitted but not started.
        private async Task<UploadedPart> SendWithRetriesAsync(PreparedPart part, PartAdmission.Admitted admitted, AttemptAdmission.Attempt firstAttempt, CancellationToken token)
        {
            RetryPolicy retries = RetryPolicy.ForStorage(context.Network);
            AttemptAdmission.Attempt attempt = firstAttempt;
            int renewedForExpiry = 0;
            bool renew = false;
            for (int number = 1; ; number++)
            {
                attempt = await StartAttemptAsync(part, admitted, attempt, token, renew);
                renew = false;

                UploadPartAttempt outcome;
                byte[] sentSha256 = null;
                try
                {
                    PartContent content = part.Body.CreateContent(progress.AddSentBytes, token);
                    content.HashBody = checkpointing != null;
                    try
                    {
                        outcome = await FilesClient.SendUploadPart(context, part.Target.Method, part.Target.Uri, part.Target.Headers, content, token);
                    }
                    catch (Exception) when (content.SourceFailure != null)
                    {
                        // Reported below as the source's failure, not the transport's.
                        outcome = null;
                    }
                    if (content.SourceFailure != null)
                    {
                        ExceptionDispatchInfo.Capture(content.SourceFailure).Throw();
                    }
                    // A cancelled request can surface as a transport failure; it says nothing about the network.
                    if (!outcome.Succeeded)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    Measure(part, number, outcome, content.BytesWritten, attempt);
                    sentSha256 = content.Sha256;
                }
                finally
                {
                    // Ends an attempt that was not measured, without a sample.
                    attempt.Dispose();
                    attempt = null;
                }

                if (outcome.Succeeded)
                {
                    return new UploadedPart(part.Part.Number, part.Part.Offset, part.Part.Length, outcome.ETag, number, Hex(sentSha256));
                }
                if (outcome.UrlExpired && part.Target.RetryParts && renewedForExpiry < SignedUrl.MaxExpiredRenewals)
                {
                    renewedForExpiry++;
                    renew = true;
                    log.Info($"Renewing the URL of part {part.Part.Number} of an upload: the storage server says it has expired");
                    progress.PartRetried();
                    continue;
                }
                int retry = number - renewedForExpiry;
                TimeSpan delay = retries.DelayBefore(retry, outcome.StatusCode, outcome.RetryAfter);
                string operation = $"part {part.Part.Number} of the upload to {session.Path}";
                if (!outcome.IsTransient || !part.Target.RetryParts || retry > retries.MaxRetries || !retries.CanWait(operation, delay, outcome.StatusCode, outcome.RetryAfter))
                {
                    ExceptionDispatchInfo.Capture(outcome.Error).Throw();
                }
                retries.LogRetry(operation, retry, delay, outcome.StatusCode, outcome.RetryAfter, outcome.Error);
                progress.PartRetried();
                await Task.Delay(delay, token);
            }
        }

        // Starts the part's next attempt: admitted is the part's own admission, and attempt, if not null, an attempt
        // already admitted for it that holds a limiter request slot. The part's URL is renewed first if it is about to
        // expire, or, with renew, because storage refused it as expired. Then the attempt starts only if the upload
        // still allows it: waiting for the slot or the renewal, it may have been overtaken by a throttle's cooldown or a
        // lowered target. If so it gives its slot back and asks again, ahead of new parts, and its wait continues. A
        // renewal that fails gives the slot back before its failure is thrown.
        //
        // A URL renewed here, after admission, may withdraw permission to send the part alongside others. Unless the
        // part was admitted alone, other parts, possibly later ones, are already being sent with it, so the instruction
        // can no longer be followed; the upload stops instead of sending the part against it.
        private async Task<AttemptAdmission.Attempt> StartAttemptAsync(PreparedPart part, PartAdmission.Admitted admitted, AttemptAdmission.Attempt attempt, CancellationToken token, bool renew = false)
        {
            TimeSpan askedAt = attempt?.AskedAt ?? progress.Elapsed;
            while (true)
            {
                attempt = attempt ?? await AdmitAttemptAsync(retry: true, askedAt, token);
                try
                {
                    bool renewed = await RenewIfExpiringAsync(part, token, renew);
                    renew = false;
                    if (renewed && !part.Target.ParallelParts)
                    {
                        if (!admitted.Alone)
                        {
                            throw new InvalidResponseException($"Renewing the URL of part {part.Part.Number} of the upload to {session.Path}, Files.com withdrew permission to send parts in parallel after the part was already being sent alongside others, so the upload was stopped.");
                        }
                        UseSerial(UploadSerialReason.ServerWithdrewParallelParts);
                    }
                }
                catch
                {
                    attempt.Dispose();
                    throw;
                }
                if (attempt.TryStart(progress.Elapsed - askedAt))
                {
                    return attempt;
                }
                attempt = null;
            }
        }

        // Ends the attempt with its measurement, which the concurrency controller learns from, and reports it.
        private void Measure(PreparedPart part, int number, UploadPartAttempt outcome, long bytesWritten, AttemptAdmission.Attempt attempt)
        {
            var sample = new AttemptSample(outcome.Succeeded, outcome.IsThrottled, outcome.IsTransient, outcome.RetryAfter, part.Part.Length, outcome.Duration, part.Part.SizeEpoch, sizer);
            List<UploadTraceEvent> decisions = attempt.Complete(sample);
            if (outcome.IsThrottled && options.Concurrency.Adaptive)
            {
                log.Info($"Throttled (HTTP {outcome.StatusCode}) sending part {part.Part.Number} of an upload; pausing its part requests");
                log.Debug($"The throttled upload is to {session.Path}");
            }
            if (!progress.Tracing)
            {
                return;
            }
            var record = new UploadAttemptTrace(session.Path, progress.Elapsed)
            {
                PartNumber = part.Part.Number,
                Attempt = number,
                Accepted = outcome.Succeeded,
                StatusCode = outcome.StatusCode,
                Throttled = outcome.IsThrottled,
                RetryAfter = outcome.RetryAfter,
                Duration = outcome.Duration,
                AdmissionWait = attempt.Waited,
                PartBytes = part.Part.Length,
                SentBytes = bytesWritten,
                Target = attempts.Target,
            };
            progress.Trace(record, decisions);
        }

        // Gives the part a new URL, with the same number, offset and length, if its URL is about to expire, or always
        // with force. Returns whether it did.
        private async Task<bool> RenewIfExpiringAsync(PreparedPart part, CancellationToken token, bool force = false)
        {
            if (!force && !part.Target.ExpiresWithin(RenewUrlsExpiringWithin))
            {
                return false;
            }
            part.Target = await planner.RenewAsync(part.Part, token);
            return true;
        }

        // Records the upload's first failure and stops everything else. A part cancelled because of that, or because
        // the caller cancelled, has not failed on its own.
        private void Fail(Exception exception)
        {
            if (exception is OperationCanceledException && stop.IsCancellationRequested)
            {
                return;
            }
            if (Interlocked.CompareExchange(ref failure, ExceptionDispatchInfo.Capture(exception), null) == null)
            {
                log.Debug($"Stopping the upload to {session.Path}", exception);
            }
            CancellationUtil.Stop(stop, log);
        }

        private async Task StopOnFailure(Task stage)
        {
            try
            {
                await stage;
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        // Every planned part, in order, each accepted exactly once: the only list the upload is completed with. A gap is
        // a defect of the upload's own bookkeeping, never a limit, so it is not an UploadLimitException.
        private IReadOnlyList<UploadedPart> AcceptedParts()
        {
            lock (acceptedParts)
            {
                List<UploadedPart> parts = acceptedParts.Values.ToList();
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].Number != i + 1)
                    {
                        throw new InvalidOperationException($"Part {i + 1} of the upload to {session.Path} was not accepted.");
                    }
                }
                if (parts.Count != planner.PartCount)
                {
                    throw new InvalidOperationException($"Only {parts.Count} of the {planner.PartCount} parts of the upload to {session.Path} were accepted.");
                }
                return parts.AsReadOnly();
            }
        }

        // A part that has been read. It holds its room among the parts read ahead until it starts, and its buffer
        // reservation until it has been sent for the last time.
        private sealed class PreparedPart : IDisposable
        {
            private readonly IDisposable room;
            private readonly IDisposable reservation;

            internal PreparedPart(PlannedPart planned, PartBody body, IDisposable room, IDisposable reservation)
            {
                Part = planned.Part;
                Target = planned.Target;
                Body = body;
                this.room = room;
                this.reservation = reservation;
            }

            internal PartDescriptor Part { get; }

            // Replaced when the part's URL is renewed.
            internal PartTarget Target { get; set; }

            internal PartBody Body { get; }

            internal void Started()
            {
                room.Dispose();
            }

            public void Dispose()
            {
                room.Dispose();
                Body.Dispose();
                reservation.Dispose();
            }
        }

        // Prepared parts waiting to be sent, in part order. Holds at most maxParts parts, and buffers of at most
        // maxBytes, counting the one the dispatcher has taken but not started yet. Neither limit depends on how many
        // parts the upload sends at once, so a growing target never makes it read further ahead. A part needing more
        // than maxBytes takes all of it, so it is read once no other part waits.
        private sealed class ReadyParts
        {
            private readonly Queue<PreparedPart> parts = new Queue<PreparedPart>();
            private readonly SemaphoreSlim queued = new SemaphoreSlim(0);
            private readonly FifoBudget partRoom;
            private readonly FifoBudget byteRoom;
            private readonly UploadProgressTracker progress;

            internal ReadyParts(int maxParts, long maxBytes, UploadProgressTracker progress)
            {
                partRoom = new FifoBudget(maxParts);
                byteRoom = new FifoBudget(maxBytes);
                this.progress = progress;
            }

            // Room for one more part. Taken before the part is planned, so that its size is chosen as late as possible.
            internal Task<IDisposable> WaitForPartRoomAsync(CancellationToken token)
            {
                return Counted(partRoom.AcquireAsync(1, token));
            }

            // Room for the planned part's bufferBytes. Both rooms are disposed when the part starts, or will not be sent.
            internal Task<IDisposable> WaitForByteRoomAsync(long bufferBytes, CancellationToken token)
            {
                return Counted(byteRoom.AcquireAsync(Math.Min(bufferBytes, byteRoom.Capacity), token));
            }

            internal void Add(PreparedPart part)
            {
                lock (parts)
                {
                    parts.Enqueue(part);
                }
                progress.ReadyChanged(1);
                queued.Release();
            }

            // No more parts will be added.
            internal void Finish()
            {
                queued.Release();
            }

            // The next part, or null after Finish when every part has been taken.
            internal async Task<PreparedPart> TakeAsync(CancellationToken token)
            {
                await queued.WaitAsync(token);
                lock (parts)
                {
                    if (parts.Count == 0)
                    {
                        return null;
                    }
                    progress.ReadyChanged(-1);
                    return parts.Dequeue();
                }
            }

            internal void PartStarted(PreparedPart part)
            {
                part.Started();
            }

            internal List<PreparedPart> TakeRemaining()
            {
                lock (parts)
                {
                    var remaining = parts.ToList();
                    progress.ReadyChanged(-parts.Count);
                    parts.Clear();
                    return remaining;
                }
            }

            private async Task<IDisposable> Counted(Task<IDisposable> wait)
            {
                if (!wait.IsCompleted)
                {
                    progress.ReadyLimitReached();
                }
                return await wait;
            }
        }

        // A prepared part's room among the parts read ahead: one of MaxReadyParts and its share of MaxReadyBytes.
        private sealed class Room : IDisposable
        {
            private readonly IDisposable part;
            private readonly IDisposable bytes;

            internal Room(IDisposable part, IDisposable bytes)
            {
                this.part = part;
                this.bytes = bytes;
            }

            public void Dispose()
            {
                part.Dispose();
                bytes.Dispose();
            }
        }
    }
}