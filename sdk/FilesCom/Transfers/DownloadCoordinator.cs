using FilesCom.Http;
using FilesCom.Models;
using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Runs one download. It asks Files.com for the file's ordinary download URL, and the storage the URL names, and its
    // first answer, decide how the bytes are fetched:
    //
    //   bound to a version  (storage that checks If-Match against the object the URL names, DownloadVersion.CanBind)
    //                       the first part is asked for alone, without a condition; its answer binds the download
    //                       (Establish): 206 from byte 0, a Content-Range stating the file's size, and a strong entity
    //                       tag, which with that size and the URL's resource is the version. The URL that answer comes
    //                       from, as renewed, must itself be one CanBind accepts. Every other request carries If-Match
    //                       with that tag, and the parts are fetched in parts of PartSize, several at once, each a Range
    //                       request that must be answered 206 with exactly those bytes of exactly that version:
    //                       Content-Range naming the part and the version's size, a matching Content-Length if any, the
    //                       version's entity tag, and a body of exactly the part's length. A part that fails in a way
    //                       that may be temporary is fetched again, alone, as exactly the bytes the version has of it.
    //                       Its URL is renewed when it is about to expire; the new URL must name the same resource, and
    //                       the parts it serves are still checked against the version.
    //   not bound           (other storage, or a first answer without a strong entity tag, for a file larger than the
    //                       first part) the file comes in a single request, never split or continued, and must be
    //                       answered 200 with all of it: the length the answer declares, or, for an answer that declares
    //                       none, such as one that ends by closing its connection, the size Files.com stated for the
    //                       file with the URL. A body of another size than the stated one is inconsistent with it, which
    //                       shows neither a body cut short nor a changed file, and fails the download, which is not
    //                       tried again; without a stated size, what arrives before the body ends is taken as the file.
    //                       A local file starts again from its first byte if that request fails partway; a stream only
    //                       if nothing has been written to it. A file no larger than the first part, answered without a
    //                       strong entity tag, is that one answer, whole, once all of it has arrived: cut short, it
    //                       decides nothing, and the part's next answer may bind a version. Without a Content-Length,
    //                       that answer too must have the size Files.com stated with the URL, which its Content-Range
    //                       does not stand in for: another size fails the download, which is not tried again.
    //
    // A first answer of 200, 416 or a redirect cannot bind the download, which then comes in one request. A bound part
    // answered 200 means the server ignores Range: the file is then fetched whole, as the version, if what was written
    // may be discarded. 412, 416, another size or another entity tag mean the version changed
    // (DownloadSourceChangedException). Requests that carry a condition or a range follow no redirect, so every answer
    // comes from the resource the version belongs to. An answer from the Files.com web app is refused before any byte
    // is written.
    //
    // Parts are admitted in order: each first takes buffer space from the limiter (a copy buffer for a file, the whole
    // part for a stream, which holds it until the parts before it are written), then admission to the download's scope
    // (TransferScope), which allows its fixed number, or an automatic download's learned target, of requests at once, then
    // a request slot, which its attempt holds; a retry gives both back while it waits, then asks again.
    // The first failure, or the caller's cancellation, stops everything; every part has ended, and released what it
    // held, before RunAsync ends. A local file is published only once every part has been written; with a checkpoint,
    // what was written is recorded instead so that running the download again continues it. Nothing is put in place,
    // recorded complete or reported done unless every byte of the file has been written (CheckWritten).
    internal sealed class DownloadCoordinator : IDisposable
    {
        // How often, at most, a download records its progress in its checkpoint while it runs.
        private static readonly TimeSpan CheckpointInterval = TimeSpan.FromSeconds(2);

        // For the SDK's own tests: what is to happen once a file an earlier run put in place has been recognized in its
        // directory, before that is relied on (RecoverPlaced), for the downloads within the flow of a test that sets it, so
        // that a test can move the directory at the moment another program could. Nothing else sets it.
        internal static readonly AsyncLocal<Action> BeforeRecoveringFaults = new AsyncLocal<Action>();

        // For the SDK's own tests: called with each extent's end as a salvage finishes copying it (SalvageAsync), so that a
        // test can fail or stop the copy partway. Nothing else sets it.
        internal static readonly AsyncLocal<Action<long>> SalvageFaults = new AsyncLocal<Action<long>>();

        // For the SDK's own tests: awaited before each write of a part's received bytes to a local file, as part of that
        // write, so that a test can hold or slow the destination. Nothing else sets it.
        internal static readonly AsyncLocal<Func<Task>> DestinationWriteFaults = new AsyncLocal<Func<Task>>();

        private static readonly SdkLog log = SdkLog.For(typeof(DownloadCoordinator));

        private readonly OperationContext context;
        private readonly string path;
        private readonly DownloadOptions options;
        private readonly DownloadProgressTracker progress;
        private readonly CancellationToken cancellationToken;
        private CancellationTokenSource stop;
        private readonly HttpClient storage;
        private readonly object renewLock = new object();
        private readonly SemaphoreSlim checkpointing = new SemaphoreSlim(1, 1);
        private DownloadSink sink;
        private SignedUrl url;
        // The size Files.com stated for the file in the answer that gave url, or null when that answer stated none that
        // can be used. It is neither a version nor progress: only an unbound answer to url that declares no Content-Length
        // is checked against it, a whole answer (FetchWholeOnceAsync) or a first part that is the whole file
        // (FetchPartOnceAsync).
        private long? statedSize;
        private DownloadVersion version;
        private Task renewal;
        // What the download's part requests are admitted to, once it fetches parts (ScopeFor).
        private TransferScope scope;
        // Part tasks not yet ended, and one more while parts are being dispatched; partsEnded completes when none remain.
        private int running;
        private TaskCompletionSource<bool> partsEnded;
        // While a download not yet bound fetches its first part (Establish): the index of the last part to fetch once
        // that part's answer is known, 0 when that answer is the whole file, or -1 when the file is to come in one
        // request instead.
        private TaskCompletionSource<long> established;
        // The size of a file no larger than its first part, once all of the one answer without a strong entity tag that
        // is the whole file has arrived; null otherwise.
        private long? wholeFirstPart;
        private long partsFetched;
        private ExceptionDispatchInfo failure;
        private DateTimeOffset lastCheckpoint;
        private long? partSize;
        private long wholeWritten;
        // Whether the stage holds the whole file, flushed, and the checkpoint is to record it complete.
        private bool complete;
        // Whether the download continues a stage an earlier run kept, whose version storage has not yet confirmed
        // (ContinueAsync): until it has, nothing is fetched into the stage or put in place, and a stop leaves the stage
        // and its checkpoint as they were (KeepForResume).
        private bool confirming;

        private DownloadCoordinator(OperationContext context, string path, DownloadOptions options, DownloadProgressTracker progress, CancellationToken cancellationToken)
        {
            this.context = context;
            this.path = path;
            this.options = options;
            this.progress = progress;
            this.cancellationToken = cancellationToken;
            stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            storage = FilesClient.StorageClient(context);
        }

        private TransferLimiter Limiter => options.Limiter;

        private FileDownloadSink FileSink => sink as FileDownloadSink;

        public void Dispose()
        {
            if (scope != null && !scope.Shared)
            {
                scope.Dispose();
            }
            checkpointing.Dispose();
            stop.Dispose();
        }

        // Downloads path to the local file localPath: into a stage beside it, published once complete. With a
        // checkpoint, an earlier run's stage and progress are continued when they are still usable.
        internal static async Task<DownloadResult> ToFileAsync(OperationContext context, string path, string localPath, DownloadOptions options, CancellationToken cancellationToken)
        {
            string full = Path.GetFullPath(localPath);
            string name = Path.GetFileName(full);
            if (string.IsNullOrEmpty(name))
            {
                throw InvalidParameterException.For(nameof(localPath), $"{localPath} names no file");
            }
            var progress = new DownloadProgressTracker(options.Progress, path, full);
            using (var coordinator = new DownloadCoordinator(context, path, options, progress, cancellationToken))
            {
                LocalDirectory directory = LocalDirectory.Open(Path.GetDirectoryName(full), options.PublicationPolicy);
                return await coordinator.RunToFileAsync(directory, ownsDirectory: true, name, full);
            }
        }

        // Downloads path to the file name in directory, which the caller has opened and keeps open.
        internal static async Task<DownloadResult> ToFileInAsync(OperationContext context, string path, LocalDirectory directory, string name, DownloadOptions options, CancellationToken cancellationToken)
        {
            string full = Path.Combine(directory.Path, name);
            var progress = new DownloadProgressTracker(options.Progress, path, full);
            using (var coordinator = new DownloadCoordinator(context, path, options, progress, cancellationToken))
            {
                return await coordinator.RunToFileAsync(directory, ownsDirectory: false, name, full);
            }
        }

        // Downloads path into destination, from its position.
        internal static async Task<DownloadResult> ToStreamAsync(OperationContext context, string path, Stream destination, DownloadOptions options, CancellationToken cancellationToken)
        {
            var progress = new DownloadProgressTracker(options.Progress, path, null);
            using (var coordinator = new DownloadCoordinator(context, path, options, progress, cancellationToken))
            using (var stream = new StreamDownloadSink(destination, progress))
            {
                coordinator.sink = stream;
                await coordinator.NegotiateAsync();
                long parts = await coordinator.FetchAsync(stream);
                // Cancelled at the last moment, even by a progress handler: the download does not succeed.
                cancellationToken.ThrowIfCancellationRequested();
                return new DownloadResult(progress.Snapshot(), parts);
            }
        }

        // Whether version, which a download of path wrote, is still the file's: Files.com's URL for it names the storage
        // resource the version came from, and storage still serves the version (ConfirmAsync). A file changed, or moved to
        // another resource, is not; a failure that shows neither, such as a connection's, is thrown.
        internal static async Task<bool> IsCurrentAsync(OperationContext context, string path, DownloadVersion version, TransferLimiter limiter, Dictionary<string, object> requestOptions, CancellationToken cancellationToken)
        {
            DownloadOptions options = new DownloadOptions { Limiter = limiter, RequestOptions = requestOptions }.Snapshot();
            using (var coordinator = new DownloadCoordinator(context, path, options, new DownloadProgressTracker(null, path, null), cancellationToken))
            {
                coordinator.version = version;
                try
                {
                    await coordinator.NegotiateAsync();
                    if (!DownloadVersion.CanBind(coordinator.url))
                    {
                        // Recorded by an earlier version of the SDK, for storage this one does not bind downloads to: nothing
                        // it answers shows the version is still the file's.
                        log.Debug($"The version of {path} a download wrote is of storage that cannot confirm it");
                        return false;
                    }
                    await coordinator.ConfirmAsync();
                    return true;
                }
                catch (DownloadSourceChangedException e)
                {
                    log.Debug($"The version of {path} a download wrote is no longer the file's", e);
                    return false;
                }
            }
        }

        // Publication and recovery, with a checkpoint: the stage is flushed and the checkpoint records it complete, with
        // the stage's identity, before it is put in place (RecordCompleteAsync), so that whatever happens next, a later
        // run can tell the stage still waiting, the file already in place under the destination's name, and neither. A
        // later run that finds the file in place, and not the stage, finishes what this one did not (the stage's second
        // name, the directory's flush, the checkpoint's removal) and reports the file as completed earlier, without
        // fetching it again (RecoverPlacedAsync). One that finds the stage continues it as any other, after storage
        // confirms its version (ContinueAsync), and puts it in place as its own policy says; stopped before that is known,
        // it leaves the stage and checkpoint as they were. A stage not bound to a version cannot be confirmed, so
        // it is discarded and the file is fetched again.
        private async Task<DownloadResult> RunToFileAsync(LocalDirectory directory, bool ownsDirectory, string name, string full)
        {
            DownloadCheckpoint saved;
            LocalStage stage = null;
            bool continuing = false;
            try
            {
                if (!directory.Supports(options.PublicationPolicy))
                {
                    throw new DownloadPublicationException(DownloadPublicationProblem.NotSupported, full, $"On this platform the SDK cannot put {full} in place as {options.PublicationPolicy} says, so nothing was downloaded.");
                }
                saved = ReadCheckpoint(full);
                if (saved != null && saved.Complete)
                {
                    LocalEntry placed = directory.Examine(name);
                    if (placed != null && placed.Regular && FileIdentity.Same(saved.StageIdentity, placed.Identity) && placed.Size == saved.Size)
                    {
                        try
                        {
                            BeforeRecoveringFaults.Value?.Invoke();
                            return RecoverPlaced(directory, saved, full);
                        }
                        finally
                        {
                            if (ownsDirectory)
                            {
                                directory.Dispose();
                            }
                        }
                    }
                }
                // A checkpoint that records digests because its stage or staging directory cannot be shown to be this
                // download's: that stage is only ever read (SalvageAsync), never reopened to write or made private first.
                bool readOnly = saved?.Extents != null && !(FileIdentity.Strong(saved.StageIdentity) && FileIdentity.Strong(saved.StagingIdentity));
                if (saved != null && !readOnly)
                {
                    stage = directory.ReopenStage(new StageLocation(saved.StageDirectory, saved.StageFile, saved.StageMode), name, saved.StageIdentity, saved.StagingIdentity);
                    continuing = stage != null;
                    if (continuing && saved.Complete && saved.ETag == null)
                    {
                        // Complete, but of no version storage can confirm: fetched again, into a new stage.
                        log.Info("Starting a download over: its checkpoint records a complete download not bound to a version, which cannot be confirmed");
                        stage.Dispose();
                        stage = null;
                        continuing = false;
                    }
                    // The checkpoint can be relied on (CheckpointFile); still, bytes it records written must be in the
                    // stage, or it is not the checkpoint of this stage.
                    if (continuing && stage.Length < saved.WrittenEnd)
                    {
                        string why = $"it records {saved.WrittenEnd} bytes written, but its stage holds {stage.Length}";
                        if (options.CheckpointRestartPolicy != CheckpointRestartPolicy.StartOver)
                        {
                            stage.Keep();
                            throw new TransferCheckpointException(options.CheckpointPath, TransferCheckpointProblem.Unreadable, $"{why}. To download the file again from its first byte, delete the checkpoint, or set DownloadOptions.CheckpointRestartPolicy to StartOver");
                        }
                        // This download's own stage, shown to be the one recorded: emptied, and used again.
                        log.Info("Starting a download over: its stage does not hold what its checkpoint records");
                        stage.SetLength(0);
                        DiscardStaleCheckpoint();
                        continuing = false;
                    }
                }
                List<DigestExtent> verified = continuing ? saved.Extents : null;
                if (saved != null && stage == null && saved.Extents != null && saved.ETag != null)
                {
                    // Its stage cannot be shown to be this download's, but its checkpoint records digests of what it wrote:
                    // the bytes it still holds as recorded are copied into a stage of this run's own (SalvageAsync).
                    (stage, verified) = await SalvageAsync(directory, saved, name);
                    continuing = stage != null;
                }
                CheckDestination(directory, name, full, retained: continuing);
                stage = stage ?? directory.CreateStage(name);
                sink = new FileDownloadSink(ownsDirectory ? directory : null, stage, continuing ? Covered(verified) ?? new IntervalSet(saved.Written) : null, continuing ? verified : null);
            }
            catch (Exception e)
            {
                if (e is DownloadPublicationException refused)
                {
                    refused.CheckpointPath = options.CheckpointPath;
                }
                if (continuing)
                {
                    stage.Keep();
                }
                stage?.Dispose();
                if (ownsDirectory)
                {
                    directory.Dispose();
                }
                throw;
            }

            using (sink)
            {
                try
                {
                    if (continuing)
                    {
                        await ContinueAsync(saved);
                    }
                    else
                    {
                        await NegotiateAsync();
                    }
                    await SaveCheckpointAsync(force: true);
                    long parts = await FetchAsync(sink);
                    // Cancelled at the last moment, even by a progress handler reporting the end: the file is not put in
                    // place, and the file already there stays.
                    cancellationToken.ThrowIfCancellationRequested();
                    await RecordCompleteAsync();
                    FileSink.Stage.Publish(options.PublicationPolicy);
                    if (!options.AcknowledgedByCaller)
                    {
                        DeleteCheckpoint();
                    }
                    return new DownloadResult(progress.Snapshot(), parts, version, completedEarlier: false, localIdentity: FileSink.Stage.Identity);
                }
                catch (Exception e)
                {
                    bool retained = KeepForResume();
                    if (e is DownloadPublicationException refused)
                    {
                        refused.Retained = retained;
                        refused.CheckpointPath = options.CheckpointPath;
                    }
                    throw;
                }
            }
        }

        // Refuses, before anything is fetched, a destination the policy would refuse when the file is put in place: an
        // entry of any kind for CreateNew, or one that is not a regular file for ReplaceRegularFile. The check is made
        // again as the file is put in place, and only that one decides. retained says whether an earlier run's stage is
        // kept, as it is, for a later run.
        private void CheckDestination(LocalDirectory directory, string name, string full, bool retained)
        {
            if (options.PublicationPolicy == DownloadPublicationPolicy.ReplaceExisting)
            {
                return;
            }
            LocalEntry existing = directory.Examine(name);
            if (existing == null)
            {
                return;
            }
            if (options.PublicationPolicy == DownloadPublicationPolicy.CreateNew)
            {
                throw new DownloadPublicationException(DownloadPublicationProblem.DestinationExists, full, $"{full} exists, and is left as it is: nothing was downloaded.") { Retained = retained };
            }
            if (!existing.Regular)
            {
                throw new DownloadPublicationException(DownloadPublicationProblem.DestinationNotRegularFile, full, $"{full} is not a regular file, so it is not replaced: nothing was downloaded.") { Retained = retained };
            }
        }

        // The file an earlier run put in place, recognized by its identity and length: what that run did not finish is
        // finished, and the download is reported as completed earlier. It counts only while the path the download was given
        // still leads to the directory it was found in, which may have been opened long before, as a folder download opens
        // its folders: otherwise it is in place, but not at full, and nothing is finished, removed or forgotten, so that a
        // run that finds that directory at the path again recognizes it (DestinationMoved, placed).
        private DownloadResult RecoverPlaced(LocalDirectory directory, DownloadCheckpoint saved, string full)
        {
            if (!directory.StillAtPath())
            {
                throw new DownloadPublicationException(DownloadPublicationProblem.DestinationMoved, full, $"The file an earlier run downloaded to {full} is in place in the folder this download opened, but that folder is no longer at {directory.Path}: it was renamed, or replaced by another, so the file is not at {full}. It is not removed, and its checkpoint is kept.", placed: true) { CheckpointPath = options.CheckpointPath };
            }
            log.Info("Completing a download an earlier run put in place");
            directory.FinishPlaced(new StageLocation(saved.StageDirectory, saved.StageFile, saved.StageMode), saved.StageIdentity, saved.StagingIdentity);
            try
            {
                directory.FlushEntries();
            }
            catch (IOException e)
            {
                throw new DownloadPublicationException(DownloadPublicationProblem.PlacedNotDurable, full, $"The file downloaded to {full} was put in place, but may not survive a system crash: {e.Message}", e) { CheckpointPath = options.CheckpointPath };
            }
            if (!options.AcknowledgedByCaller)
            {
                DeleteCheckpoint();
            }
            DownloadVersion placed = saved.ETag != null ? new DownloadVersion(saved.ETag, saved.Size, saved.Source) : null;
            progress.SizeKnown(saved.Size, bound: placed != null);
            progress.Resumed(saved.Size);
            progress.Report();
            return new DownloadResult(progress.Snapshot(), 0, placed, completedEarlier: true, localIdentity: saved.StageIdentity);
        }

        // Makes the complete stage durable and records it complete, with its identity, before it is put in place. A
        // download bound to a version records every byte written; one that is not records no entity tag.
        private async Task RecordCompleteAsync()
        {
            if (options.CheckpointPath == null || FileSink == null)
            {
                return;
            }
            complete = true;
            if (version != null && version.Size > 0)
            {
                FileSink.RecordWhole(version.Size);
            }
            await SaveCheckpointAsync(force: true);
        }

        // Continues the download a checkpoint records: gets a new URL, which must name the storage resource the version
        // came from, and has storage confirm the version (ConfirmAsync); if that version is gone, starts over as a new
        // download of the file as it is now, in the same stage. Until storage has answered for the version, the stage is
        // pending (confirming): stopped by cancellation, or by a failure that does not show the version gone, such as a
        // failed request or a refusal of access, it is kept with its checkpoint as they were, for a later run to confirm.
        private async Task ContinueAsync(DownloadCheckpoint saved)
        {
            confirming = true;
            version = new DownloadVersion(saved.ETag, saved.Size, saved.Source);
            try
            {
                await NegotiateAsync();
                if (!DownloadVersion.CanBind(url))
                {
                    // Recorded by an earlier version of the SDK, for storage this one does not bind downloads to: nothing it
                    // answers can confirm the version, so the stage's bytes are not used.
                    confirming = false;
                    log.Info("Starting a download over: its checkpoint records a version of storage that cannot confirm it");
                    version = null;
                    FileSink.Restart();
                    DiscardStaleCheckpoint();
                    progress.Restarted();
                    return;
                }
                await ConfirmAsync();
            }
            catch (DownloadSourceChangedException e)
            {
                confirming = false;
                log.Info("Starting a download over: the version its checkpoint records is gone");
                log.Debug($"The version of {path} the checkpoint records is gone", e);
                version = null;
                FileSink.Restart();
                DiscardStaleCheckpoint();
                progress.Restarted();
                await NegotiateAsync();
                return;
            }
            confirming = false;
            partSize = saved.PartSize;
            progress.Resumed(FileSink.Written.Total);
        }

        // Removes the checkpoint of a download that starts over in its stage: it records the version that is gone and the
        // ranges of the stage that held it, which the new download now writes over. Until the download binds a new version
        // and records it (SaveCheckpointAsync, which records nothing before then), a run that found that checkpoint after
        // a crash would take the stage's bytes for the old version's. Removal that fails leaves it, as CheckpointFile.Delete
        // says; the next checkpoint written replaces it.
        private void DiscardStaleCheckpoint()
        {
            if (options.CheckpointPath != null)
            {
                CheckpointFile.Delete(options.CheckpointPath);
            }
        }

        // Asks Files.com for the file's download URL, and the size it states for the file, as any download does. A download
        // bound to a version needs the new URL to name the same storage resource as the version's (DownloadVersion.Source):
        // its signature and expiry change, but a URL for another resource is not shown to be that version, whatever entity
        // tag it serves, so the version is then gone (DownloadSourceChangedException). The stated size is kept with the
        // URL it came with, replacing the one an earlier URL came with, or clearing it when this answer states none that
        // can be used (no size, or a negative one); 0 is a size.
        private async Task NegotiateAsync()
        {
            RemoteFile file = await RemoteFile.DownloadCore(context, path, null, options.RequestOptions != null ? new Dictionary<string, object>(options.RequestOptions) : null, stop.Token);
            if (string.IsNullOrEmpty(file.DownloadUri))
            {
                throw new InvalidResponseException($"Files.com gave no download URL for {path}.");
            }
            SignedUrl answered = SignedUrl.Of(file.DownloadUri);
            if (version != null && !version.IsSourceOf(answered))
            {
                throw new DownloadSourceChangedException(path, "Files.com renewed its URL for another storage resource than the one the version came from");
            }
            long? stated = file.Size >= 0 ? file.Size : null;
            url = answered;
            statedSize = stated;
        }

        // Has storage confirm that the version can still be fetched, before anything written for it is used: the
        // version's first byte is asked for with its entity tag in If-Match, and must be answered 206 for exactly that
        // byte, of the version's size, with the version's entity tag; or 200, the whole file, from storage that now
        // ignores Range, with the version's entity tag and size, whose body is not read. 412, 416, another size or another
        // entity tag show the version gone (DownloadSourceChangedException); any other failure, such as a connection's or
        // a refusal of access, shows nothing about it, and is thrown as it is once the client's retries are spent. A URL
        // storage refuses as expired is renewed, as a part's is.
        private async Task ConfirmAsync()
        {
            var first = new ByteRange(0, 0, 1);
            RetryPolicy retries = RetryPolicy.ForStorage(context.Network);
            SignedUrl refused = null;
            int renewedForExpiry = 0;
            for (int attempt = 1; ; attempt++)
            {
                Exception failed = null;
                bool expired = false;
                IDisposable slot = await progress.TimeRequestWait(Limiter.AcquireRequestAsync(stop.Token));
                try
                {
                    SignedUrl current = await CurrentUrlAsync(stop.Token, refused);
                    refused = null;
                    try
                    {
                        await ConfirmOnceAsync(first, current, stop.Token);
                        return;
                    }
                    catch (InvalidResponseException e) when (e.StorageSaysExpired && renewedForExpiry < SignedUrl.MaxExpiredRenewals && !stop.IsCancellationRequested)
                    {
                        renewedForExpiry++;
                        refused = current;
                        expired = true;
                    }
                }
                catch (Exception e) when (RetryPolicy.IsRetryable(e) && !stop.IsCancellationRequested)
                {
                    failed = e;
                }
                catch (HttpRequestException e) when (!stop.IsCancellationRequested)
                {
                    // A connection failure no retry can help, such as an untrusted certificate.
                    ExceptionDispatchInfo.Capture(ForCaller(e)).Throw();
                }
                finally
                {
                    slot.Dispose();
                }
                if (expired)
                {
                    log.Info("Renewing the URL of a download: the storage server says it has expired");
                    continue;
                }
                int retry = attempt - renewedForExpiry;
                TimeSpan delay = retries.DelayAfterFailure(failed, retry);
                const string operation = "the confirmation of a download's version";
                if (retry > retries.MaxRetries || !retries.CanWait(operation, delay, failed))
                {
                    ExceptionDispatchInfo.Capture(ForCaller(failed)).Throw();
                }
                retries.LogRetry(operation, retry, delay, failed);
                await Task.Delay(delay, stop.Token);
            }
        }

        private async Task ConfirmOnceAsync(ByteRange first, SignedUrl current, CancellationToken token)
        {
            using (var watch = new NetworkWatch(context.Network, token))
            using (HttpRequestMessage request = Request(current, first))
            {
                watch.Begin(NetworkWait.ResponseHeaders);
                using (HttpResponseMessage response = await FilesApiService.SendStorageRequestAsync(storage, request, watch))
                {
                    await CheckAnswerAsync(response, watch);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // The parts will find that Range is ignored, and fetch the file whole, as the version.
                        long? length = DownloadResponse.ContentLength(response);
                        if (length.HasValue && length.Value != version.Size)
                        {
                            throw new DownloadSourceChangedException(path, $"the storage server has {length.Value} bytes where the version has {version.Size}");
                        }
                        return;
                    }
                    if (response.StatusCode != HttpStatusCode.PartialContent)
                    {
                        throw new InvalidResponseException($"The storage server answered the request for the first byte of {path} with HTTP {(int)response.StatusCode}, not with that byte, so the version being continued could not be confirmed.");
                    }
                    CheckPart(response, first);
                    (long received, bool overrun) = await ResponseBody.ReadIntoAsync(response, watch, new byte[1], count => progress.Received(count));
                    if (overrun)
                    {
                        throw new InvalidResponseException(Overrun(first));
                    }
                    if (received < 1)
                    {
                        // A body that ended early: the connection's failure, which may be temporary.
                        throw new HttpRequestException($"The first byte of {path} did not arrive.");
                    }
                }
            }
        }

        // Fetches the file into target: in parts when it is bound to a version, or when its storage can bind it, from its
        // first answer (Establish); whole otherwise. Returns the number of parts it was fetched in.
        private async Task<long> FetchAsync(DownloadSink target)
        {
            if (version == null && !DownloadVersion.CanBind(url))
            {
                // Storage not known to check If-Match against the object its URL names: nothing would show that parts
                // of the file are of one version.
                await FetchWholeAsync(bound: false);
                return 1;
            }
            if (version != null)
            {
                progress.SizeKnown(version.Size, bound: true);
                progress.Report();
            }
            long each = PartSizeFor(target);
            bool rangesIgnored = await FetchPartsAsync(each, target);
            if (rangesIgnored)
            {
                if (!target.CanRestart)
                {
                    throw new InvalidResponseException($"The storage server answered part of {path} with the whole file after other parts had been written to the stream, so the download was stopped.");
                }
                target.Restart();
                if (version != null)
                {
                    log.Info("The storage server ignores Range: downloading the file in one request");
                    progress.Restarted();
                }
                else
                {
                    log.Info("The storage server's first answer cannot bind the download to a version: downloading the file in one request");
                }
                // Every part has ended, and the one request the file now comes in is no scope's: its progress reports no
                // part concurrency (null), as for any file that comes in one request. A scope this download shares with
                // other files stays theirs.
                progress.Observe(null);
                // The whole file is fetched under a stop of its own.
                failure = null;
                stop.Dispose();
                stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                await FetchWholeAsync(bound: version != null);
                return 1;
            }
            CheckWritten(target);
            return partsFetched;
        }

        // Every byte of the file must have been written once its parts have: the version's, or, for a file that was its
        // first part's one answer, that answer's. Anything less is a defect of the download itself, which then fails,
        // with nothing put in place, recorded complete or reported done.
        private void CheckWritten(DownloadSink target)
        {
            long size = version?.Size ?? wholeFirstPart ?? -1;
            bool whole = size >= 0 && (target is FileDownloadSink file ? file.Written.Covers(0, size) && file.Written.Total == size
                : target is StreamDownloadSink stream && stream.Delivered == size);
            if (!whole)
            {
                throw new InvalidOperationException($"The download of {path} ended before every byte of the file was written, so nothing was put in place.");
            }
        }

        // The size of every part but the last: a continued download keeps the size it started with. A part held in
        // memory until it is written must fit the limiter's buffer space and an array.
        private long PartSizeFor(DownloadSink target)
        {
            long each = partSize ?? options.PartBytes;
            if (target is StreamDownloadSink)
            {
                each = Math.Min(each, Math.Min(Limiter.MaxBufferedBytes, 1L << 30));
            }
            partSize = each;
            return each;
        }

        // The parts of a file of size bytes, each bytes long but the last, in order, leaving out those written, when the
        // download continues one. They are made as the download reaches them, so nothing is held for parts not yet
        // admitted, and a run of written parts is passed over at once. Positions never pass the size: each part is at
        // most what remains of the file, so no sum can overflow, whatever the size and part size.
        internal static IEnumerable<ByteRange> PlanRanges(long size, long each, IntervalSet written, CancellationToken token)
        {
            long start = 0;
            while (start < size)
            {
                token.ThrowIfCancellationRequested();
                long length = Math.Min(each, size - start);
                long end = start + length;
                if (written == null || !written.Covers(start, end))
                {
                    yield return new ByteRange(start / each, start, length);
                    start = end;
                    continue;
                }
                // Written parts are whole parts: the run ends on a part boundary, or at a part not wholly written.
                long covered = written.CoveredEnd(start);
                start = Math.Max(end, covered >= size ? size : covered - covered % each);
            }
        }

        // Fetches parts, several at once. Returns true when the file is to come in one request instead, every other part
        // having been stopped: the server answered a part with the whole file, ignoring Range, or the first answer of a
        // download not yet bound could not bind it. Part tasks are not kept: each counts itself out as the very last thing
        // it does, so the count reaching zero once every part has been dispatched means that all of them have ended.
        private async Task<bool> FetchPartsAsync(long each, DownloadSink target)
        {
            if (scope == null)
            {
                scope = ScopeFor(url);
                TransferScope observed = scope;
                progress.Observe(() => observed.Snapshot(Limiter));
            }
            // One for the dispatch itself, counted out once it has dispatched every part.
            running = 1;
            partsEnded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            established = version == null ? new TaskCompletionSource<long>(TaskCreationOptions.RunContinuationsAsynchronously) : null;
            Task delivery = Task.CompletedTask;
            if (target is StreamDownloadSink stream)
            {
                delivery = StopOnFailure(DeliverAsync(stream, each));
            }
            await StopOnFailure(DispatchAsync(each, target));
            CountOut();
            // Part tasks never throw, and always count themselves out.
            await partsEnded.Task;
            await delivery;
            if (failure != null && failure.SourceException is RangesIgnored)
            {
                return true;
            }
            failure?.Throw();
            cancellationToken.ThrowIfCancellationRequested();
            return false;
        }

        private void CountOut()
        {
            if (Interlocked.Decrement(ref running) == 0)
            {
                partsEnded.TrySetResult(true);
            }
        }

        // What the download's part requests are admitted to (TransferScope): a static download's fixed number, or an
        // automatic download's target, learned with its tuning, or with the tuning for the storage url names, as the Go
        // SDK chooses it by the storage's host. Only storage that can bind a download, Amazon S3, has its downloads fetched
        // in parts, so those take the scaled S3 policy (AutomaticConcurrencyOptions.ScaledS3) unless the caller gave
        // tuning, which is used as it is; any other storage takes the Generic defaults. A directory download's files share
        // the scope of their profile, so they learn one target; a static download, and an automatic one on its own, has
        // its own.
        private TransferScope ScopeFor(SignedUrl current)
        {
            if (options.ConcurrencyMode == DownloadConcurrencyMode.Static)
            {
                return new TransferScope(ConcurrencySettings.Static(options.Concurrency), options.ConcurrencyMode, path, shared: false);
            }
            AutomaticConcurrencyOptions tuning = options.Tuning ?? (DownloadVersion.CanBind(current) ? AutomaticConcurrencyOptions.ScaledS3() : AutomaticConcurrencyOptions.For(UploadPerformanceProfile.Generic));
            var settings = ConcurrencySettings.Automatic(tuning, options.MaxConcurrency, options.Tuning != null ? UploadProfileSource.Options : UploadProfileSource.Server);
            return options.SharedScopes?.For(settings) ?? new TransferScope(settings, options.ConcurrencyMode, path, shared: false);
        }

        // Admits parts in order: each first takes buffer space for exactly what it will allocate, then admission to the
        // download's scope, then a request slot, the order every transfer on a limiter takes them in; each is released at
        // once if a later one cannot be had. A download not yet bound admits its first part alone, and the others only
        // once that part's answer has bound the download (Establish), which says how many there are, and what each must
        // be. A part admitted is one more request in flight: as the scope's target grows, more are admitted, and as it is
        // lowered, none are until enough of those in flight have ended.
        private async Task DispatchAsync(long each, DownloadSink target)
        {
            IEnumerable<ByteRange> parts;
            if (version == null)
            {
                await DispatchPartAsync(new ByteRange(0, 0, each), target);
                if (await EstablishedAsync() < 1)
                {
                    return;
                }
                parts = PlanRanges(version.Size, each, null, stop.Token).Skip(1);
            }
            else
            {
                parts = PlanRanges(version.Size, each, FileSink?.Written, stop.Token);
            }
            foreach (ByteRange part in parts)
            {
                await DispatchPartAsync(part, target);
            }
        }

        private async Task DispatchPartAsync(ByteRange part, DownloadSink target)
        {
            long bufferBytes = target.BufferFor(part, Limiter.MaxBufferedBytes);
            IDisposable reservation = await progress.TimeBufferWait(Limiter.ReserveBufferAsync(bufferBytes, stop.Token));
            AttemptAdmission.Attempt attempt;
            try
            {
                // Waiting here, rather than in the part's own task, keeps later parts behind this one and leaves the
                // download one request in the limiter's queue, so transfers sharing it take turns.
                attempt = await AdmitAttemptAsync(retry: false, progress.Elapsed);
            }
            catch
            {
                reservation.Dispose();
                throw;
            }
            partsFetched++;
            Interlocked.Increment(ref running);
            _ = FetchPartAsync(part, target, bufferBytes, reservation, attempt);
        }

        // Waits for admission to the download's scope, then for a request slot from the limiter. The attempt is not
        // started.
        private async Task<AttemptAdmission.Attempt> AdmitAttemptAsync(bool retry, TimeSpan askedAt)
        {
            AttemptAdmission.Attempt attempt = await scope.Admission.AdmitAsync(retry, stop.Token);
            try
            {
                attempt.Hold(await progress.TimeRequestWait(Limiter.AcquireRequestAsync(stop.Token)), askedAt);
                return attempt;
            }
            catch
            {
                attempt.Dispose();
                throw;
            }
        }

        // Writes a stream's parts in order, once their number is known: at once for a download already bound, otherwise
        // once the first part's answer has bound the download, or shown it whole. Nothing is written when the file is to
        // come in one request instead.
        private async Task DeliverAsync(StreamDownloadSink stream, long each)
        {
            long last = version != null ? (version.Size - 1) / each : await EstablishedAsync();
            if (last >= 0)
            {
                await stream.DeliverAsync(0, last, stop.Token);
            }
        }

        // The index of the last part, once the first part's answer has bound the download, 0 when that answer is the whole
        // file, or -1 when the file is to come in one request.
        private async Task<long> EstablishedAsync()
        {
            await WithCancellation(established.Task, stop.Token);
            return established.Task.Result;
        }

        // Fetches part until it has arrived whole, then commits it to target. firstAttempt, from the dispatcher, has been
        // admitted and holds a request slot, but has not started. Each later attempt is admitted like any, ahead of new
        // parts, after the retry's wait, during which the part holds neither admission nor a request slot; a cooldown
        // after throttling holds it too. An attempt starts only if the scope still allows it once it holds its slot and
        // its URL (AttemptAdmission.Attempt.TryStart); otherwise it asks again. A part that storage refuses saying its URL
        // has expired, even though the URL's own expiry is still to come, is fetched again at once with the version's URL
        // renewed (CurrentUrlAsync), at most SignedUrl.MaxExpiredRenewals times, without using any of the client's
        // retries; any other refusal is the part's failure. The first part of a download not yet bound is asked for
        // without a condition, and is known only from its answer (Establish): fetched again before an answer bound the
        // download, a local file first starts again from its first byte, since another version may answer.
        //
        // Each attempt that gets its part whole, or fails in a way that may be temporary, is measured: from sending the
        // request until the last byte of the body arrived, less the time spent writing it to a local file, which says
        // nothing of the network; with its part's bytes, its status and the server's Retry-After. A failed attempt ends
        // there, before any retry's wait. A whole part's attempt keeps its admission until the part is committed and
        // recorded in the checkpoint, local work the scope does not count as the network's (Writing), so one part at a
        // time still means the next is asked for only once this one is recorded. An expired URL, an answer that is the
        // whole file or binds nothing, another failure and cancellation are not measured.
        private async Task FetchPartAsync(ByteRange part, DownloadSink target, long bufferBytes, IDisposable reservation, AttemptAdmission.Attempt firstAttempt)
        {
            progress.PartStarted();
            AttemptAdmission.Attempt attempt = firstAttempt;
            try
            {
                RetryPolicy retries = RetryPolicy.ForStorage(context.Network);
                SignedUrl refused = null;
                int renewedForExpiry = 0;
                for (int number = 1; ; number++)
                {
                    TimeSpan askedAt = attempt?.AskedAt ?? progress.Elapsed;
                    ByteRange answered = null;
                    PartReceiver receiver = null;
                    Exception failed = null;
                    bool expired = false;
                    // The attempt that got its part whole, and what its request measured: it keeps its admission until the
                    // part is committed and recorded.
                    AttemptAdmission.Attempt fetched = null;
                    AttemptSample measured = null;
                    try
                    {
                        if (number > 1 && version == null)
                        {
                            target.Restart();
                        }
                        SignedUrl current;
                        while (true)
                        {
                            attempt = attempt ?? await AdmitAttemptAsync(retry: true, askedAt);
                            current = await CurrentUrlAsync(stop.Token, refused);
                            if (attempt.TryStart(progress.Elapsed - askedAt))
                            {
                                break;
                            }
                            attempt = null;
                        }
                        // The size stated with current, taken with it while the download is not yet bound: this first part
                        // is then the only one, so no other renews the URL meanwhile.
                        long? stated = version == null ? statedSize : null;
                        refused = null;
                        // Once the version is known, the first part, planned before the file's size was, is exactly the
                        // bytes the version has of it, on every attempt.
                        ByteRange asked = version != null && part.End > version.Size ? new ByteRange(part.Index, part.Start, version.Size - part.Start) : part;
                        long sentAt = Stopwatch.GetTimestamp();
                        long writing = 0;
                        try
                        {
                            (answered, receiver) = await FetchPartOnceAsync(asked, target, bufferBytes, current, stated, stop.Token, attempt, ticks => writing += ticks);
                            if (receiver == null)
                            {
                                throw new RangesIgnored();
                            }
                            measured = new AttemptSample(true, false, false, null, answered.Length, NetworkTime(sentAt, writing));
                            fetched = attempt;
                            attempt = null;
                        }
                        catch (InvalidResponseException e) when (e.StorageSaysExpired && renewedForExpiry < SignedUrl.MaxExpiredRenewals && !stop.IsCancellationRequested)
                        {
                            renewedForExpiry++;
                            refused = current;
                            expired = true;
                        }
                        catch (Exception e) when (RetryPolicy.IsRetryable(e) && !stop.IsCancellationRequested)
                        {
                            int? status = RetryPolicy.StatusOf(e);
                            TimeSpan? retryAfter = RetryPolicy.RetryAfterOf(e);
                            attempt.Complete(new AttemptSample(false, status == 429 || status == 503 || retryAfter.HasValue, true, retryAfter, asked.Length, NetworkTime(sentAt, writing)));
                            throw;
                        }
                    }
                    catch (Exception e) when (RetryPolicy.IsRetryable(e) && !stop.IsCancellationRequested)
                    {
                        failed = e;
                    }
                    catch (HttpRequestException e) when (!stop.IsCancellationRequested)
                    {
                        // A connection failure no retry can help, such as an untrusted certificate.
                        ExceptionDispatchInfo.Capture(ForCaller(e)).Throw();
                    }
                    finally
                    {
                        // Ends an attempt that was not measured, without a sample, giving back its admission and slot.
                        attempt?.Dispose();
                        attempt = null;
                    }
                    if (expired)
                    {
                        log.Info("Renewing the URL of a download: the storage server says it has expired");
                        progress.Retried();
                        continue;
                    }
                    if (failed == null)
                    {
                        // The part is one of the download's parts at once until it is committed and recorded, as a
                        // part always was: with one part at a time, the next is not asked for before this one is in the
                        // checkpoint. That local work is not the network's (Writing), and the measurement is the request's.
                        fetched.Writing(true);
                        try
                        {
                            try
                            {
                                target.Commit(answered, receiver, reservation);
                            }
                            finally
                            {
                                receiver.Dispose();
                            }
                            reservation = null;
                            if (FileSink != null)
                            {
                                progress.PartWritten(answered.Length);
                                await SaveCheckpointAsync(force: false);
                            }
                        }
                        finally
                        {
                            fetched.Complete(measured);
                        }
                        return;
                    }
                    int retry = number - renewedForExpiry;
                    TimeSpan delay = retries.DelayAfterFailure(failed, retry);
                    string operation = $"part {part.Index + 1} of a download";
                    if (retry > retries.MaxRetries || !retries.CanWait(operation, delay, failed))
                    {
                        ExceptionDispatchInfo.Capture(ForCaller(failed)).Throw();
                    }
                    retries.LogRetry(operation, retry, delay, failed);
                    if (scope.Mode == DownloadConcurrencyMode.Automatic && RetryPolicy.StatusOf(failed) is int throttle && (throttle == 429 || throttle == 503))
                    {
                        log.Info($"Throttled (HTTP {throttle}) fetching part {part.Index + 1} of a download; pausing its part requests");
                    }
                    progress.Retried();
                    await Task.Delay(delay, stop.Token);
                }
            }
            catch (Exception e)
            {
                Fail(e);
            }
            finally
            {
                // A first part that ended before its answer bound the download, or showed it whole, has nothing for the
                // rest of the download to wait for.
                established?.TrySetResult(-1);
                attempt?.Dispose();
                reservation?.Dispose();
                progress.PartEnded();
                // Last: FetchPartsAsync waits for every part to have counted itself out.
                CountOut();
            }
        }

        // The time from sentAt until now, less writing, the ticks spent writing to the destination meanwhile.
        private static TimeSpan NetworkTime(long sentAt, long writing)
        {
            long ticks = Math.Max(0, Stopwatch.GetTimestamp() - sentAt - writing);
            return TimeSpan.FromSeconds(ticks / (double)Stopwatch.Frequency);
        }

        // One attempt at part, sent to current. Returns the part as answered, and the receiver that holds all of it, which
        // the caller then owns (PartReceiver); or no receiver when the file is to come in one request instead: the server
        // ignored Range and answered with the whole file, whose body is not read, or, for the first part of a download not
        // yet bound, its answer cannot bind it. An attempt that fails disposes of the receiver it made.
        // Anything else that is not exactly the part fails. stated is the size Files.com stated with current, for a
        // download not yet bound: a first answer that is the whole file, unbound and without a Content-Length, must have it.
        private async Task<(ByteRange Part, PartReceiver Receiver)> FetchPartOnceAsync(ByteRange part, DownloadSink target, long bufferBytes, SignedUrl current, long? stated, CancellationToken token, AttemptAdmission.Attempt attempt, Action<long> writing)
        {
            bool establishing = version == null;
            if (establishing && !DownloadVersion.CanBind(current))
            {
                // The URL, renewed since the download began, names storage that cannot bind it: whatever it would answer
                // binds nothing, so the file comes in one request, from that URL.
                log.Info("A download's renewed URL names storage that cannot bind it to a version: downloading the file in one request");
                established.TrySetResult(-1);
                return (part, null);
            }
            using (var watch = new NetworkWatch(context.Network, token))
            using (HttpRequestMessage request = Request(current, part))
            {
                watch.Begin(NetworkWait.ResponseHeaders);
                using (HttpResponseMessage response = await FilesApiService.SendStorageRequestAsync(storage, request, watch))
                {
                    if (establishing)
                    {
                        DownloadResponse.CheckNotFrontendApp(response);
                        int status = (int)response.StatusCode;
                        if (status == 200 || status == 416 || (status >= 300 && status < 400))
                        {
                            // The whole file, a range that cannot be satisfied, such as of an empty file, or a redirect,
                            // which a request for a range does not follow: nothing that binds the download.
                            established.TrySetResult(-1);
                            return (part, null);
                        }
                    }
                    await CheckAnswerAsync(response, watch);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return (part, null);
                    }
                    if (response.StatusCode != HttpStatusCode.PartialContent)
                    {
                        throw new InvalidResponseException($"The storage server answered part {part.Index + 1} of {path} with HTTP {(int)response.StatusCode}, not with the part.");
                    }
                    if (establishing)
                    {
                        part = Establish(response, part, current);
                        if (part == null)
                        {
                            return (null, null);
                        }
                    }
                    else
                    {
                        CheckPart(response, part);
                    }
                    PartReceiver receiver = target.Receive(part, bufferBytes);
                    try
                    {
                        long received = 0;
                        bool overrun = false;
                        if (receiver.ReadsInPlace)
                        {
                            (received, overrun) = await ResponseBody.ReadIntoAsync(response, watch, receiver.ReadBuffer, count => progress.Received(count));
                        }
                        else
                        {
                            // The copy buffer is filled before each write, so the destination takes a write for each buffer,
                            // not for each read; each read is counted as it arrives.
                            long written = 0;
                            await ResponseBody.CopyFilledToAsync(response, watch, receiver.ReadBuffer, count =>
                            {
                                if (received + count > part.Length)
                                {
                                    overrun = true;
                                    throw new InvalidResponseException(Overrun(part));
                                }
                                received += count;
                                progress.Received(count);
                            }, async (buffer, count) =>
                            {
                                // While it writes, the attempt moves nothing over the network: the scope does not count it
                                // as on the network (AttemptAdmission.Attempt.Writing), and its own duration leaves it out.
                                long writeStart = Stopwatch.GetTimestamp();
                                attempt.Writing(true);
                                try
                                {
                                    if (DestinationWriteFaults.Value is Func<Task> fault)
                                    {
                                        await fault();
                                    }
                                    await receiver.WriteAsync(written, buffer, count, token);
                                }
                                finally
                                {
                                    attempt.Writing(false);
                                }
                                writing(Stopwatch.GetTimestamp() - writeStart);
                                written += count;
                            });
                        }
                        if (overrun)
                        {
                            throw new InvalidResponseException(Overrun(part));
                        }
                        if (received < part.Length)
                        {
                            // A body that ended early: the connection's failure, which may be temporary.
                            throw new HttpRequestException($"Part {part.Index + 1} of the download ended after {received} of its {part.Length} bytes.");
                        }
                        if (establishing && version == null)
                        {
                            // The one answer, without a strong entity tag, that is the whole file: only now, with all of it
                            // here, does it decide the download, which has no other part. Without a Content-Length, it must
                            // also have the size Files.com stated, as a whole answer must (FetchWholeOnceAsync): its own
                            // Content-Range does not stand in for that. Another size is only inconsistent with it, which is
                            // not tried again; nothing of the part has been committed or given to a stream.
                            if (stated.HasValue && !DownloadResponse.ContentLength(response).HasValue && stated.Value != part.Length)
                            {
                                throw new InvalidResponseException($"The storage server's first answer for {path} held the {part.Length} bytes its Content-Range named as the whole file, without a Content-Length, not the {stated.Value} bytes Files.com stated for the file, so the download was stopped.");
                            }
                            wholeFirstPart = part.Length;
                            progress.SizeKnown(part.Length, bound: false);
                            progress.Report();
                            established.TrySetResult(0);
                        }
                        return (part, receiver);
                    }
                    catch
                    {
                        // An attempt that ended without its part whole: what it holds goes with it.
                        receiver.Dispose();
                        throw;
                    }
                }
            }
        }

        // Binds a download not yet bound from the answer to its first part: a 206 from byte 0 that states the file's size,
        // unencoded, naming exactly the bytes it holds, which are the part asked for or, for a smaller file, all of it.
        // Its strong entity tag, that size and the resource current names are the version (DownloadVersion): every later
        // request, the part's own next attempts included, carries the tag in If-Match, and must be answered as that
        // version, so the other parts may start at once. Without a strong entity tag, the answer is still the whole file
        // when the file is no larger than the part, unbound, but only once all of it has arrived (FetchPartOnceAsync); a
        // larger file then comes in one request (null), since nothing would show that its other parts are of the same
        // version. Returns the part as answered.
        private ByteRange Establish(HttpResponseMessage response, ByteRange requested, SignedUrl current)
        {
            ContentRangeHeaderValue range = response.Content?.Headers.ContentRange;
            if (range == null || !string.Equals(range.Unit, "bytes", StringComparison.OrdinalIgnoreCase) || !range.HasRange || !range.HasLength || range.Length.Value < 1)
            {
                throw new InvalidResponseException($"The storage server answered the first part of {path} without a Content-Range naming its bytes and the file's size.");
            }
            long size = range.Length.Value;
            var first = new ByteRange(0, 0, Math.Min(requested.Length, size));
            if (range.From.Value != 0 || range.To.Value != first.End - 1)
            {
                throw new InvalidResponseException($"The storage server answered the first part of {path}, bytes 0-{first.End - 1}, with bytes {range.From}-{range.To}.");
            }
            long? length = DownloadResponse.ContentLength(response);
            if (length.HasValue && length.Value != first.Length)
            {
                throw new InvalidResponseException($"The storage server answered the first part of {path} with a Content-Length of {length.Value}, not its {first.Length} bytes.");
            }
            DownloadResponse.CheckIdentityEncoding(response);
            string etag = DownloadVersion.EntityTagOf(response);
            bool bound = DownloadVersion.IsStrongEntityTag(etag);
            if (!bound && size > first.Length)
            {
                established.TrySetResult(-1);
                return null;
            }
            if (bound)
            {
                version = new DownloadVersion(etag, size, DownloadVersion.SourceOf(current));
                progress.SizeKnown(size, bound: true);
                progress.Report();
                established.TrySetResult((size - 1) / requested.Length);
            }
            return first;
        }

        // Fetches the whole file in one request: bound, as the version, after the server ignored Range; otherwise as the
        // storage server has it now. A local file starts again from its first byte after a failure partway, within the
        // client's retries; a stream is retried only while nothing has been written to it. A refusal saying the URL has
        // expired renews it and fetches again at once, as a part's does.
        private async Task FetchWholeAsync(bool bound)
        {
            RetryPolicy retries = RetryPolicy.ForStorage(context.Network);
            progress.PartStarted();
            try
            {
                // The copy buffer the body is read through: no larger than the limiter's buffer space, or than the file
                // when its size is known.
                long bufferBytes = Math.Min(FileDownloadSink.CopyBufferSize, Limiter.MaxBufferedBytes);
                if (bound)
                {
                    bufferBytes = Math.Min(bufferBytes, Math.Max(1, version.Size));
                }
                SignedUrl refused = null;
                int renewedForExpiry = 0;
                for (int attempt = 1; ; attempt++)
                {
                    Exception failed = null;
                    bool expired = false;
                    IDisposable buffer = null;
                    IDisposable slot = null;
                    try
                    {
                        // Buffer space, then a request slot, as parts take them; each is owned as soon as it is had.
                        buffer = await progress.TimeBufferWait(Limiter.ReserveBufferAsync(bufferBytes, stop.Token));
                        slot = await progress.TimeRequestWait(Limiter.AcquireRequestAsync(stop.Token));
                        if (wholeWritten > 0)
                        {
                            sink.Restart();
                            progress.Restarted();
                            wholeWritten = 0;
                        }
                        SignedUrl current;
                        long? stated = null;
                        if (bound)
                        {
                            current = await CurrentUrlAsync(stop.Token, refused);
                        }
                        else
                        {
                            if (refused != null && refused == url)
                            {
                                await NegotiateAsync();
                            }
                            current = url;
                            stated = statedSize;
                        }
                        refused = null;
                        try
                        {
                            await FetchWholeOnceAsync(bound, current, stated, new byte[bufferBytes], stop.Token);
                        }
                        catch (InvalidResponseException e) when (e.StorageSaysExpired && renewedForExpiry < SignedUrl.MaxExpiredRenewals && !stop.IsCancellationRequested)
                        {
                            renewedForExpiry++;
                            refused = current;
                            expired = true;
                        }
                    }
                    catch (Exception e) when (RetryPolicy.IsRetryable(e) && !stop.IsCancellationRequested && !(e is DestinationFailure))
                    {
                        failed = e;
                    }
                    catch (DestinationFailure e)
                    {
                        ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                    }
                    catch (HttpRequestException e) when (!stop.IsCancellationRequested)
                    {
                        // A connection failure no retry can help, such as an untrusted certificate.
                        ExceptionDispatchInfo.Capture(ForCaller(e)).Throw();
                    }
                    finally
                    {
                        slot?.Dispose();
                        buffer?.Dispose();
                    }
                    if (expired)
                    {
                        log.Info("Renewing the URL of a download: the storage server says it has expired");
                        progress.Retried();
                        continue;
                    }
                    if (failed == null)
                    {
                        progress.PartWritten(0);
                        return;
                    }
                    int retry = attempt - renewedForExpiry;
                    TimeSpan delay = retries.DelayAfterFailure(failed, retry);
                    // Not again once any of it is in the caller's stream, which cannot be taken back.
                    if (retry > retries.MaxRetries || !sink.CanRestart || !retries.CanWait("a download", delay, failed))
                    {
                        ExceptionDispatchInfo.Capture(ForCaller(failed)).Throw();
                    }
                    retries.LogRetry("a download", retry, delay, failed);
                    progress.Retried();
                    await Task.Delay(delay, stop.Token);
                    if (!bound)
                    {
                        await RenewWholeIfExpiringAsync();
                    }
                }
            }
            finally
            {
                progress.PartEnded();
            }
        }

        // One attempt at the whole file, from current. Its body must be exactly as long as expected: the version's size, for
        // a bound download; otherwise the length the answer declares; otherwise, for an answer that declares none, stated,
        // the size Files.com stated with current. A body that ends before a declared length is the connection's failure; a
        // body of another size than stated is only inconsistent with it, an InvalidResponseException that is not tried
        // again. A body longer than expected fails before the chunk that would pass it is written. With no size expected,
        // what arrives before the body ends is taken as the file.
        private async Task FetchWholeOnceAsync(bool bound, SignedUrl current, long? stated, byte[] copyBuffer, CancellationToken token)
        {
            using (var watch = new NetworkWatch(context.Network, token))
            using (HttpRequestMessage request = Request(current, null))
            {
                watch.Begin(NetworkWait.ResponseHeaders);
                using (HttpResponseMessage response = await FilesApiService.SendStorageRequestAsync(storage, request, watch))
                {
                    await CheckAnswerAsync(response, watch);
                    DownloadResponse.CheckWhole(response);
                    long? length = DownloadResponse.ContentLength(response);
                    if (bound && length.HasValue && length.Value != version.Size)
                    {
                        throw new DownloadSourceChangedException(path, $"the storage server sent {length.Value} bytes where the version has {version.Size}");
                    }
                    if (!bound && length.HasValue)
                    {
                        progress.SizeKnown(length.Value, bound: false);
                        progress.Report();
                    }
                    // The version's size and a declared length come first: only an unbound answer without a length is
                    // checked against the stated size.
                    bool againstStated = !bound && !length.HasValue && stated.HasValue;
                    long? expected = bound ? version.Size : length ?? stated;
                    long received = 0;
                    await ResponseBody.CopyToAsync(response, watch, copyBuffer, async (buffer, count) =>
                    {
                        // received never passes expected, so what remains of it is compared, which no sum can overflow.
                        if (expected.HasValue && count > expected.Value - received)
                        {
                            throw new InvalidResponseException(againstStated
                                ? $"The storage server's answer for {path}, which declared no length, held more than the {expected.Value} bytes Files.com stated for the file, so the download was stopped."
                                : $"The storage server sent more than the {expected.Value} bytes of {path}, so the download was stopped.");
                        }
                        try
                        {
                            await sink.WriteWholeAsync(received, buffer, count, token);
                        }
                        catch (Exception e) when (!FilesClient.IsRequestedCancellation(e, token))
                        {
                            throw new DestinationFailure(e);
                        }
                        received += count;
                        wholeWritten += count;
                        progress.Received(count);
                        progress.Written(count);
                    });
                    if (expected.HasValue && received < expected.Value)
                    {
                        if (againstStated)
                        {
                            throw new InvalidResponseException($"The storage server's answer for {path}, which declared no length, ended after {received} of the {expected.Value} bytes Files.com stated for the file, so the download was stopped.");
                        }
                        throw new HttpRequestException($"The download ended after {received} of its {expected.Value} bytes.");
                    }
                    if (!bound && !length.HasValue)
                    {
                        progress.SizeKnown(received, bound: false);
                    }
                }
            }
        }

        // What every answer must pass before anything else: not the web app, a success, and for a bound download, the
        // version's entity tag, and not a sign that the version changed.
        private async Task CheckAnswerAsync(HttpResponseMessage response, NetworkWatch watch)
        {
            DownloadResponse.CheckNotFrontendApp(response);
            int status = (int)response.StatusCode;
            if (version != null)
            {
                if (status == 412)
                {
                    throw new DownloadSourceChangedException(path, "the storage server no longer has the version (HTTP 412)");
                }
                if (status == 416)
                {
                    throw new DownloadSourceChangedException(path, "the storage server has fewer bytes than the version (HTTP 416)");
                }
            }
            if (status >= 300 && status < 400)
            {
                throw new InvalidResponseException($"The storage server redirected the download of {path} (HTTP {status}) where it is not followed: a request bound to a version follows no redirect, so that its answer comes from the version's own resource, and no request leaves HTTPS.");
            }
            if (!response.IsSuccessStatusCode)
            {
                throw await FilesApiService.StorageFailureAsync(response, watch);
            }
            if (version != null)
            {
                string etag = DownloadVersion.EntityTagOf(response);
                if (etag == null)
                {
                    throw new InvalidResponseException($"The storage server answered a request for {path} without the entity tag it was asked to match, so what it sent cannot be shown to be of the version being downloaded.");
                }
                if (!version.Matches(etag))
                {
                    throw new DownloadSourceChangedException(path, "the storage server answered with another version's entity tag");
                }
            }
        }

        private string Overrun(ByteRange part)
        {
            return $"The storage server sent more than the {part.Length} bytes of part {part.Index + 1} of {path}, which its Content-Range named, so the download was stopped.";
        }

        // A 206 must name exactly part, of the version's size, and declare no other length.
        private void CheckPart(HttpResponseMessage response, ByteRange part)
        {
            ContentRangeHeaderValue range = response.Content?.Headers.ContentRange;
            if (range == null || !string.Equals(range.Unit, "bytes", StringComparison.OrdinalIgnoreCase) || !range.HasRange || !range.HasLength)
            {
                throw new InvalidResponseException($"The storage server answered part {part.Index + 1} of {path} without a Content-Range naming its bytes and the file's size.");
            }
            if (range.Length.Value != version.Size)
            {
                throw new DownloadSourceChangedException(path, $"the storage server has {range.Length.Value} bytes where the version has {version.Size}");
            }
            if (range.From.Value != part.Start || range.To.Value != part.End - 1)
            {
                throw new InvalidResponseException($"The storage server answered part {part.Index + 1} of {path}, bytes {part.Start}-{part.End - 1}, with bytes {range.From}-{range.To}.");
            }
            long? length = DownloadResponse.ContentLength(response);
            if (length.HasValue && length.Value != part.Length)
            {
                throw new InvalidResponseException($"The storage server answered part {part.Index + 1} of {path} with a Content-Length of {length.Value}, not its {part.Length} bytes.");
            }
            DownloadResponse.CheckIdentityEncoding(response);
        }

        // The request for part, or for the whole file when part is null, for the bytes as stored, unencoded, at the URL
        // exactly as Files.com gave it. A bound download's If-Match goes with every request. A request bound to a version,
        // or for a range, follows no redirect (StaysAtResource), so that its answer comes from the resource the version
        // belongs to, or that binds it.
        private HttpRequestMessage Request(SignedUrl current, ByteRange part)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(current.Text));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("identity"));
            if (part != null)
            {
                request.Headers.Range = new RangeHeaderValue(part.Start, part.End - 1);
            }
            if (version != null)
            {
                foreach (KeyValuePair<string, string> header in version.Headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            if (part != null || version != null)
            {
                RequestMarks.Set(request, RequestMarks.StaysAtResource);
            }
            return request;
        }

        // The URL to send to, renewed first, once for everyone who needs it, when it is about to expire or when it is still
        // refused, the URL storage answered had expired. A URL already renewed by another part is used as it is.
        private async Task<SignedUrl> CurrentUrlAsync(CancellationToken token, SignedUrl refused = null)
        {
            SignedUrl current = url;
            if (current != refused && !current.ExpiresWithin(SignedUrl.RenewalMargin))
            {
                return current;
            }
            Task renewing;
            lock (renewLock)
            {
                if (url != current)
                {
                    return url;
                }
                if (renewal == null || renewal.IsCompleted)
                {
                    renewal = Renew(current);
                }
                renewing = renewal;
            }
            await WithCancellation(renewing, token);
            if (url.ExpiresWithin(TimeSpan.Zero))
            {
                throw new InvalidResponseException($"Files.com renewed the download URL of {path} with one that has already expired.");
            }
            return url;

            async Task Renew(SignedUrl expiring)
            {
                await NegotiateAsync();
            }
        }

        // A download not bound to a version gets a new URL when its URL is about to expire; the file then starts over,
        // as it does after any failure partway.
        private async Task RenewWholeIfExpiringAsync()
        {
            if (url.ExpiresWithin(SignedUrl.RenewalMargin))
            {
                await NegotiateAsync();
            }
        }

        // After a failure, cancellation or refusal: keeps what a later run can continue, and says whether it did. With a
        // checkpoint, a download bound to a version keeps its stage, made private again if its access had been changed to
        // the destination's, and records it; a stage that cannot be made private again is not kept. A file put in place
        // keeps its checkpoint, which then records it complete, so a later run recognizes it (RecoverPlaced); nothing is
        // left to continue. A stage lost to another program, whose name leads to that program's file, is made private
        // again where it can be, through its own descriptor, but neither kept nor removed, and its checkpoint, which would
        // lead a later run to that file, is removed. A stage an earlier run kept, stopped while its version was being
        // confirmed (confirming), is kept as it was, with its checkpoint unwritten, since nothing has shown that version
        // gone; a later run confirms it before fetching anything. Anything else keeps nothing: without a checkpoint, or
        // not bound to a version, whose bytes could not be confirmed later, the stage is removed with its checkpoint.
        private bool KeepForResume()
        {
            if (FileSink == null)
            {
                return false;
            }
            if (FileSink.Stage.Lost)
            {
                FileSink.Stage.RestorePrivacy();
                DeleteCheckpoint();
                return false;
            }
            if (FileSink.Stage.Placed)
            {
                if (!complete)
                {
                    DeleteCheckpoint();
                }
                return false;
            }
            if (confirming)
            {
                // Reopened as the stage its checkpoint records, holding what it records, and nothing written to it since.
                if (!FileSink.Stage.RestorePrivacy())
                {
                    log.Info("A stopped download's stage could not be made private again, so it is not kept for a later run");
                    DeleteCheckpoint();
                    return false;
                }
                FileSink.Stage.Keep();
                return true;
            }
            if (options.CheckpointPath == null || version == null)
            {
                DeleteCheckpoint();
                return false;
            }
            if (!FileSink.Stage.RestorePrivacy())
            {
                log.Info("A stopped download's stage could not be made private again, so it is not kept for a later run");
                DeleteCheckpoint();
                return false;
            }
            try
            {
                SaveCheckpointAsync(force: true).GetAwaiter().GetResult();
                FileSink.Stage.Keep();
                return true;
            }
            catch (Exception e)
            {
                // The download's own failure is what the caller sees; the checkpoint keeps what it last recorded.
                DiagnosticText.WarnCleanupFailed(log, "Recording the checkpoint of a stopped download failed; the checkpoint keeps what it last recorded", e);
                return false;
            }
        }

        // For a checkpoint with digests (DownloadCheckpoint.Extents) whose stage cannot be shown to be this download's:
        // copies the extents the earlier stage still holds as recorded into a new stage of this download's own, and records
        // that stage in the checkpoint. The earlier stage is read only through a handle that cannot change it
        // (LocalDirectory.OpenSalvage), one extent at a time through one buffer held in the limiter's buffer space, each
        // byte hashed as it is copied; an extent is kept only when all of it was read and its digest matches. Bytes outside
        // the extents, and extents not held whole or not as recorded, are not used. The new stage's bytes and names are
        // flushed to storage, then the checkpoint replaced, so a run stopped or failing before that, a flush that cannot be
        // made included, leaves the earlier stage and checkpoint as they were, and its new stage removed; one stopped by
        // the checkpoint's replacement keeps its new stage, which either checkpoint may then name. The
        // earlier stage and its staging directory are never written to, changed or removed, by this run or any later one.
        // Copying costs reading and writing what is kept, and room for both stages until the earlier one is removed by
        // whoever can tell it is theirs. Returns the new stage and its extents, or (null, null) when the earlier stage
        // cannot be read safely or holds none of them, and the download starts over.
        private async Task<(LocalStage, List<DigestExtent>)> SalvageAsync(LocalDirectory directory, DownloadCheckpoint saved, string name)
        {
            var location = new StageLocation(saved.StageDirectory, saved.StageFile, saved.StageMode);
            using (SalvageSource source = directory.OpenSalvage(location, saved.StageIdentity, saved.StagingIdentity))
            {
                if (source == null)
                {
                    log.Info("Starting a download over: its stage, which cannot be shown to be this download's, cannot be read as this user's alone");
                    return (null, null);
                }
                // The copy buffer: no larger than the limiter's buffer space, and held in it, as a part's buffer is, from
                // before the new stage is made until the copy has ended. Copying takes no request slot.
                long bufferBytes = Math.Min(FileDownloadSink.CopyBufferSize, Limiter.MaxBufferedBytes);
                LocalStage stage;
                var verified = new List<DigestExtent>();
                using (await Limiter.ReserveBufferAsync(bufferBytes, stop.Token))
                {
                    stage = directory.CreateStage(name);
                    try
                    {
                        byte[] buffer = new byte[bufferBytes];
                        foreach (DigestExtent extent in saved.Extents)
                        {
                            if (extent.End > source.Length)
                            {
                                continue;
                            }
                            using (IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
                            {
                                long position = extent.Start;
                                while (position < extent.End)
                                {
                                    stop.Token.ThrowIfCancellationRequested();
                                    int count = source.Read(position, buffer, (int)Math.Min(buffer.Length, extent.End - position));
                                    if (count <= 0)
                                    {
                                        break;
                                    }
                                    hash.AppendData(buffer, 0, count);
                                    await stage.WriteAsync(position, buffer, count, stop.Token);
                                    position += count;
                                }
                                if (position == extent.End && FileDownloadSink.Hex(hash.GetHashAndReset()) == extent.Sha256)
                                {
                                    verified.Add(new DigestExtent { Start = extent.Start, End = extent.End, Sha256 = extent.Sha256 });
                                }
                            }
                            SalvageFaults.Value?.Invoke(extent.End);
                        }
                        if (verified.Count == 0)
                        {
                            log.Info("Starting a download over: its stage, which cannot be shown to be this download's, holds none of what its checkpoint records as recorded");
                            stage.Dispose();
                            return (null, null);
                        }
                        // Its bytes, then its names: until both are on storage, the checkpoint keeps naming the earlier stage.
                        stage.Flush();
                        stage.FlushNames();
                    }
                    catch
                    {
                        stage.Dispose();
                        throw;
                    }
                }
                IntervalSet covered = Covered(verified);
                StageLocation moved = stage.Location;
                try
                {
                    CheckpointFile.Write(options.CheckpointPath, new DownloadCheckpoint
                    {
                        Kind = DownloadCheckpoint.DownloadKind,
                        Site = saved.Site,
                        Workspace = saved.Workspace,
                        RemotePath = saved.RemotePath,
                        LocalPath = saved.LocalPath,
                        Phase = DownloadCheckpoint.PartialPhase,
                        ETag = saved.ETag,
                        Source = saved.Source,
                        Size = saved.Size,
                        PartSize = saved.PartSize,
                        StageDirectory = moved.Directory,
                        StageFile = moved.File,
                        StageMode = moved.Mode,
                        StageIdentity = stage.Identity,
                        StagingIdentity = stage.StagingIdentity,
                        Written = covered.ToList(),
                        Extents = verified,
                    });
                }
                catch
                {
                    stage.Keep();
                    stage.Dispose();
                    throw;
                }
                log.Info($"Continuing a download in a new stage: {covered.Total} bytes its earlier stage still held as recorded were copied, and the earlier stage, {source.Path}, is left as it is");
                return (stage, verified);
            }
        }

        // The ranges extents cover together, or null for none.
        private static IntervalSet Covered(List<DigestExtent> extents)
        {
            if (extents == null)
            {
                return null;
            }
            var covered = new IntervalSet();
            foreach (DigestExtent extent in extents)
            {
                covered.Add(extent.Start, extent.End);
            }
            return covered;
        }

        // Records what the stage holds, flushed first, at most every CheckpointInterval unless forced: the ranges written
        // of a download bound to a version, and once it is complete, that it is, with the stage's identity (a download not
        // bound to a version is recorded only then).
        private async Task SaveCheckpointAsync(bool force)
        {
            if (options.CheckpointPath == null || FileSink == null || (version == null && !complete))
            {
                return;
            }
            if (!force && DateTimeOffset.UtcNow - lastCheckpoint < CheckpointInterval)
            {
                return;
            }
            await checkpointing.WaitAsync();
            try
            {
                if (!force && DateTimeOffset.UtcNow - lastCheckpoint < CheckpointInterval)
                {
                    return;
                }
                (List<long[]> written, List<DigestExtent> extents) = FileSink.Recorded();
                FileSink.Stage.Flush();
                StageLocation location = FileSink.Stage.Location;
                CheckpointFile.Write(options.CheckpointPath, new DownloadCheckpoint
                {
                    Kind = DownloadCheckpoint.DownloadKind,
                    Site = CheckpointFile.SiteOf(context),
                    Workspace = CheckpointFile.WorkspaceOf(context, options.RequestOptions),
                    RemotePath = path,
                    LocalPath = progress.Snapshot().LocalPath,
                    Phase = complete ? DownloadCheckpoint.CompletePhase : DownloadCheckpoint.PartialPhase,
                    ETag = version?.ETag,
                    Source = version?.Source,
                    Size = version?.Size ?? FileSink.Stage.Length,
                    PartSize = partSize ?? options.PartBytes,
                    StageDirectory = location.Directory,
                    StageFile = location.File,
                    StageMode = location.Mode,
                    StageIdentity = FileSink.Stage.Identity,
                    StagingIdentity = FileSink.Stage.StagingIdentity,
                    Written = version == null ? new List<long[]>() : written,
                    Extents = version == null ? null : extents,
                });
                lastCheckpoint = DateTimeOffset.UtcNow;
            }
            finally
            {
                checkpointing.Release();
            }
        }

        // The checkpoint of this download, or null when there is none. One for another transfer fails.
        private DownloadCheckpoint ReadCheckpoint(string full)
        {
            if (options.CheckpointPath == null)
            {
                return null;
            }
            DownloadCheckpoint saved = CheckpointFile.Read<DownloadCheckpoint>(options.CheckpointPath, DownloadCheckpoint.DownloadKind);
            if (saved == null)
            {
                return null;
            }
            string mismatch = saved.ScopeMismatch(CheckpointFile.SiteOf(context), CheckpointFile.WorkspaceOf(context, options.RequestOptions), path, full);
            if (mismatch != null)
            {
                throw new TransferCheckpointException(options.CheckpointPath, TransferCheckpointProblem.OtherTransfer, mismatch);
            }
            string problem = saved.Problem();
            if (problem != null)
            {
                if (options.CheckpointRestartPolicy != CheckpointRestartPolicy.StartOver)
                {
                    throw new TransferCheckpointException(options.CheckpointPath, TransferCheckpointProblem.Unreadable, $"{problem}. To download the file again from its first byte, delete the checkpoint, or set DownloadOptions.CheckpointRestartPolicy to StartOver");
                }
                // Nothing it records is relied on, or removed: the download starts over in a stage of its own, and its
                // first checkpoint replaces this one.
                log.Info("Starting a download over: its checkpoint records what this SDK cannot use");
                return null;
            }
            return saved;
        }

        private void DeleteCheckpoint()
        {
            if (options.CheckpointPath != null)
            {
                CheckpointFile.Delete(options.CheckpointPath);
            }
        }

        // Records the download's first failure and stops everything else. A part cancelled because of that, or because
        // the caller cancelled, has not failed on its own.
        private void Fail(Exception exception)
        {
            if (exception is OperationCanceledException && stop.IsCancellationRequested)
            {
                return;
            }
            if (Interlocked.CompareExchange(ref failure, ExceptionDispatchInfo.Capture(exception), null) == null && !(exception is RangesIgnored))
            {
                log.Debug($"Stopping the download of {path}", exception);
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

        private static async Task WithCancellation(Task task, CancellationToken token)
        {
            if (task.IsCompleted)
            {
                await task;
                return;
            }
            var cancelled = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using (token.Register(() => cancelled.TrySetCanceled(token)))
            {
                await await Task.WhenAny(task, cancelled.Task);
            }
        }

        // The failure an attempt at the storage server is thrown as once it is not tried again, as every other request of
        // the SDK throws it: an AnsweredFailure is its own inner failure, and a failure of the connection
        // (HttpRequestException), while sending or while the answer arrives, is an ApiConnectionException, which never
        // may have taken effect, since a download only reads. A timeout is a NetworkTimeoutException already. As for the
        // SDK's other requests, the connection failure itself is logged at DEBUG, not kept as an inner exception.
        private static Exception ForCaller(Exception failure)
        {
            Exception unwrapped = RetryPolicy.Unwrap(failure);
            if (unwrapped is HttpRequestException transport)
            {
                log.Debug("Download transport error", transport);
                return new ApiConnectionException(DiagnosticText.WithoutUrlCredentials(transport.Message), requestMayHaveBeenProcessed: false);
            }
            return unwrapped;
        }

        // A part answered with the whole file: the server ignores Range.
        private sealed class RangesIgnored : Exception
        {
        }

        // The destination's own failure, thrown as it is and never retried.
        private sealed class DestinationFailure : Exception
        {
            internal DestinationFailure(Exception failure) : base(failure.Message, failure)
            {
            }
        }
    }
}