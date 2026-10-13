using FilesCom.Models;
using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Runs one directory upload or download. First it plans: it lists the source tree, top down, in ordinal order of
    // names, into the files to transfer and the empty folders to create. Then it transfers the files, at most
    // MaxConcurrentFiles at once, starting them in the order planned; every file is an ordinary file upload or download
    // (UploadCoordinator, DownloadCoordinator) with the transfer's limiter, which admits the requests of every file, and
    // of every transfer sharing it, in turn. The transfer's settings are its options as they were when it started
    // (DirectorySettings), and every request of it, the listing included, belongs to its one operation context, so it
    // keeps the client settings, origin and lifetime it started with.
    //
    // A directory transfer, upload or download, whose files are automatic (DirectorySettings.LearnsFiles) gives them one
    // scope for each profile (TransferScopes), so that the parts of every file, many small files of one part each as
    // much as a few large ones, teach one target, which they share rather than each having all of it. It starts files as
    // that target allows, with room for it to grow (FileLimit), and fewer once it is lowered; a file in progress is never
    // stopped for it.
    //
    // Local entries are reached one name at a time from the directory the caller named, through LocalDirectory, without
    // following symbolic links: an upload reads only regular files inside the tree, and a download creates folders and
    // publishes files only inside it, whatever another program renames meanwhile (where LocalDirectory can guarantee
    // that). A remote name that cannot be a local name here fails the download before anything is written.
    //
    // The first failure stops the files in progress, and no other file starts; once every file has ended, the transfer
    // ends with that failure, unchanged, or with the caller's cancellation. With a CheckpointDirectory, the transfer
    // records each file it completes, with what identifies its source: for a download, the version written (its
    // storage's entity tag and resource) and the local file's identity, length and modification time; for an upload, the
    // SHA-256 of every part sent. Each file in progress has a checkpoint of its own there. Running the transfer again
    // skips a completed file only once its source is shown to be unchanged: its storage confirms the remote version is
    // still the current one, or the local file still has the bytes that were sent. Metadata alone, such as an equal size and modification time, never does. A file
    // without such a record is transferred again. A download reports a local file, put in place now or completed earlier,
    // only where the path it reports still leads to the folder the file is in (LocalDirectory.StillAtPath), as looked at
    // after anything it waited for; the root, opened before the listing, is looked at again once the listing is read.
    //
    // File tasks are not kept: each gives back its slot as the last thing it does, so taking back every slot waits for
    // all of them. The plan itself, every file's path and size, is held until the transfer ends: the number of files at
    // once bounds the transfers in progress, not the memory of a very large tree's plan.
    internal sealed class DirectoryTransfer : IDisposable
    {
        private const string RecordName = "transfer.json";

        // An automatic directory transfer's target before any of its files has sent or fetched parts, and the fewest
        // files it limits itself to (FileLimit).
        private const int InitialFileTarget = 4;
        private const int FileLimitFloor = 4;

        // How often a transfer waiting to start a file looks again at the target its files learn, which can grow without
        // a file ending.
        private static readonly TimeSpan FileLimitRecheck = TimeSpan.FromMilliseconds(100);

        // For the SDK's own tests: a failure to throw, within the flow of a test that sets it, in place of recording the
        // file at a relative path as completed, or null to record it. Nothing else sets it.
        internal static readonly AsyncLocal<Func<string, Exception>> RecordFaults = new AsyncLocal<Func<string, Exception>>();

        private static readonly SdkLog log = SdkLog.For(typeof(DirectoryTransfer));

        private readonly OperationContext context;
        private readonly string localDirectory;
        private readonly string remoteDirectory;
        private readonly DirectorySettings settings;
        private readonly CancellationToken cancellationToken;
        private readonly CancellationTokenSource stop;
        private readonly object gate = new object();
        private readonly string checkpointDirectory;
        private readonly Dictionary<string, CompletedFile> completed = new Dictionary<string, CompletedFile>(StringComparer.Ordinal);
        private DirectoryProgressTracker progress;
        private DirectoryCheckpoint record;
        private ExceptionDispatchInfo failure;
        // The scopes an automatic directory transfer's files share; null otherwise.
        private readonly TransferScopes scopes;
        private int filesInProgress;
        // Completed, and replaced, as each file ends.
        private TaskCompletionSource<bool> fileEnded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        // The files an upload has grown to while too few of its parts were ready to send (FileLimit), when that last
        // changed, the shared target they fed when it last looked (grownTarget, 0 before any was known, and then the warm
        // start's first target if the target began with one), and whether a warm start under way set that target, read
        // with it (grownWarm, TransferScopes.WarmTarget).
        private int grownFileLimit;
        private long grownChangedAt;
        private int grownTarget;
        private bool grownWarm;

        private DirectoryTransfer(OperationContext context, string localDirectory, string remoteDirectory, DirectorySettings settings, CancellationToken cancellationToken)
        {
            this.context = context;
            this.localDirectory = localDirectory;
            this.remoteDirectory = remoteDirectory;
            this.settings = settings;
            this.cancellationToken = cancellationToken;
            stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            checkpointDirectory = settings.CheckpointDirectory;
            scopes = settings.LearnsFiles ? new TransferScopes(this.remoteDirectory) : null;
        }

        public void Dispose()
        {
            scopes?.Dispose();
            stop.Dispose();
        }

        internal static async Task<DirectoryTransferResult> UploadAsync(OperationContext context, string localDirectory, string remoteDirectory, DirectorySettings settings, CancellationToken cancellationToken)
        {
            using (var transfer = new DirectoryTransfer(context, localDirectory, RemoteFolder(remoteDirectory), settings, cancellationToken))
            using (LocalDirectory root = LocalDirectory.Open(Path.GetFullPath(localDirectory)))
            {
                transfer.CheckCheckpointDirectory(root);
                var files = new List<PlannedFile>();
                var empty = new List<string>();
                var excluded = new List<string>();
                WalkLocal(Path.GetFullPath(localDirectory), "", files, empty, excluded);
                transfer.OpenRecord(DirectoryCheckpoint.UploadKind);
                transfer.progress = new DirectoryProgressTracker(settings.Progress, localDirectory, transfer.remoteDirectory, files);

                foreach (string folder in empty)
                {
                    await transfer.CreateRemoteFolderAsync(folder);
                }
                await transfer.RunFilesAsync(files, (file, token) => transfer.UploadFileAsync(root, file, token));
                return transfer.Complete(files, empty, excluded);
            }
        }

        internal static async Task<DirectoryTransferResult> DownloadAsync(OperationContext context, string remoteDirectory, string localDirectory, DirectorySettings settings, CancellationToken cancellationToken)
        {
            using (var transfer = new DirectoryTransfer(context, localDirectory, RemoteFolder(remoteDirectory), settings, cancellationToken))
            {
                Directory.CreateDirectory(localDirectory);
                // Opened, and on Windows held when the policy needs it to be, before anything is listed.
                using (LocalDirectory root = LocalDirectory.Open(Path.GetFullPath(localDirectory), settings.DownloadFile.PublicationPolicy))
                {
                    transfer.CheckCheckpointDirectory(root);
                    var files = new List<PlannedFile>();
                    var empty = new List<string>();
                    await transfer.WalkRemoteAsync(files, empty);
                    // The listing takes as long as Files.com does, and the root was opened before it: nothing is done in
                    // a root no longer at its path, where no file would be where the result says.
                    if (!root.StillAtPath())
                    {
                        string local = Path.GetFullPath(localDirectory);
                        throw new DownloadPublicationException(DownloadPublicationProblem.DestinationMoved, local, $"The folder {local} was renamed, or replaced by another, while the remote folder was listed, so nothing was downloaded: in the folder the transfer opened, no file would be at the path it reports. Nothing there was changed, and the transfer's checkpoint, if any, is kept.");
                    }
                    transfer.OpenRecord(DirectoryCheckpoint.DownloadKind);
                    transfer.progress = new DirectoryProgressTracker(settings.Progress, localDirectory, transfer.remoteDirectory, files);

                    foreach (string folder in empty)
                    {
                        OpenLocalFolder(root, folder.Split('/'), folder.Split('/').Length, create: true).Dispose();
                    }
                    await transfer.RunFilesAsync(files, (file, token) => transfer.DownloadFileAsync(root, file, token));
                    return transfer.Complete(files, empty, new string[0]);
                }
            }
        }

        // Transfers every file, at most MaxConcurrentFiles at once, and for an automatic directory transfer no more than
        // FileLimit allows, and returns once every file started has ended, or throws the first failure, or the caller's
        // cancellation.
        private async Task RunFilesAsync(List<PlannedFile> files, Func<PlannedFile, CancellationToken, Task> transfer)
        {
            progress.Observe(FileLimit);
            progress.Report(force: true);
            grownChangedAt = Stopwatch.GetTimestamp();
            int most = settings.MaxConcurrentFiles;
            using (var slots = new SemaphoreSlim(most, most))
            {
                foreach (PlannedFile file in files)
                {
                    try
                    {
                        await WaitForFileLimitAsync();
                        await slots.WaitAsync(stop.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    // A wait being cancelled can still be granted the slot a file gives back as it stops the transfer, the
                    // cancellation and the slot arriving together: no other file starts once the transfer has stopped.
                    if (stop.IsCancellationRequested)
                    {
                        slots.Release();
                        break;
                    }
                    Interlocked.Increment(ref filesInProgress);
                    _ = RunFileAsync(file, transfer, slots);
                }
                for (int slot = 0; slot < most; slot++)
                {
                    // RunFileAsync never throws, and always gives its slot back.
                    await slots.WaitAsync();
                }
            }
            failure?.Throw();
            cancellationToken.ThrowIfCancellationRequested();
        }

        // How many files may be in progress at once: MaxConcurrentFiles, or for a directory transfer with automatic files the
        // target they learn together (TransferScopes.Target), InitialFileTarget until one of them sends or fetches parts, with room
        // for it to grow: target + target / 4 + 1, at least FileLimitFloor, within MaxConcurrentFiles. Files of one part
        // each then still offer the target more parts than it allows, which is what lets it show that more would help.
        private int TargetFileLimit()
        {
            int most = settings.MaxConcurrentFiles;
            if (scopes == null)
            {
                return most;
            }
            int target = scopes.Target() ?? InitialFileTarget;
            return Math.Min(most, Math.Max(FileLimitFloor, target + (target / 4) + 1));
        }

        // TargetFileLimit, or for an upload the files it has grown to (grownFileLimit) while fewer of its parts waited,
        // ready to send, than that room (target / 4 + 1), within MaxConcurrentFiles. An upload's file of one part spends
        // time beginning, before its part is ready, and finishing, after it was sent, and where that takes longer than
        // sending the part, the files the target's limit allows cannot feed it: more begin (WaitForFileLimitAsync), until
        // enough parts are ready, however long beginning or finishing takes, so that the target is fed and the files'
        // requests to Files.com grow gradually rather than at once.
        private int FileLimit()
        {
            int limit = TargetFileLimit();
            if (scopes == null || settings.UploadFile == null)
            {
                return limit;
            }
            return Math.Max(limit, Volatile.Read(ref grownFileLimit));
        }

        // Waits until another file may start, looking again as each file ends, and when the target may have grown
        // (FileLimitRecheck): at once within TargetFileLimit. Past it, an upload starts a file in place of each that ended,
        // up to the files it has grown to, which change by at most one file each FileLimitRecheck, however often files
        // end meanwhile: one more while its parts are too few to feed the target, one fewer, down to the target's limit,
        // while enough are ready. So its files' requests to Files.com rise and fall a file at a time; except that the
        // raises of the shared target's warm start (TransferScopes.WarmTarget), which add half again as many parts at once,
        // its last raise to the policy's start included, take the files it has grown to up in proportion to the target,
        // from the target it last looked at, as many for each part added as fed each part before: a file takes as long to
        // begin and to finish whatever the target, so each part in flight needs as many files, and a raised target fed a
        // file at a time would go unfed, its windows not measured or judged short of what it delivers, until they came.
        // Rises once the warm start is over, the policy's own, come a file at a time again. Never past
        // MaxConcurrentFiles: at the scaled policy's start of 150 parts, its own limit already allows all of them.
        private async Task WaitForFileLimitAsync()
        {
            while (scopes != null)
            {
                Task ended = Volatile.Read(ref fileEnded).Task;
                int files = Volatile.Read(ref filesInProgress);
                int limit = TargetFileLimit();
                if (files < limit)
                {
                    return;
                }
                TimeSpan wait = FileLimitRecheck;
                if (settings.UploadFile != null)
                {
                    (int Target, bool Warming, bool WarmStarted)? known = scopes.WarmTarget();
                    int target = known?.Target ?? InitialFileTarget;
                    long now = Stopwatch.GetTimestamp();
                    if (known.HasValue)
                    {
                        // Files grown before any file sent a part, while the first parts were being prepared, fed the
                        // warm start's first target, however far it has risen, or ended, by the time this looks. A rise
                        // from a target the warm start set while under way is its own, up to the raise that ends it.
                        if (grownTarget == 0 && known.Value.WarmStarted)
                        {
                            grownTarget = ConcurrencyController.WarmStartTarget;
                            grownWarm = true;
                        }
                        if (grownTarget > 0 && target > grownTarget && (grownWarm || known.Value.Warming))
                        {
                            Volatile.Write(ref grownFileLimit, (int)Math.Min(settings.MaxConcurrentFiles, Math.Ceiling(grownFileLimit * (double)target / grownTarget)));
                        }
                        grownTarget = target;
                        grownWarm = known.Value.Warming;
                    }
                    bool tooFew = scopes.Waiting() < (target / 4) + 1;
                    if (Since(grownChangedAt, now) >= FileLimitRecheck && (tooFew ? files >= grownFileLimit : grownFileLimit > limit))
                    {
                        Volatile.Write(ref grownFileLimit, tooFew ? Math.Min(settings.MaxConcurrentFiles, files + 1) : grownFileLimit - 1);
                        grownChangedAt = now;
                    }
                    if (files < grownFileLimit)
                    {
                        return;
                    }
                    TimeSpan left = FileLimitRecheck - Since(grownChangedAt, now);
                    if (tooFew && left > TimeSpan.Zero)
                    {
                        wait = left;
                    }
                }
                await Task.WhenAny(ended, Task.Delay(wait, stop.Token));
                stop.Token.ThrowIfCancellationRequested();
            }
        }

        private static TimeSpan Since(long from, long now)
        {
            return TimeSpan.FromSeconds((now - from) / (double)Stopwatch.Frequency);
        }

        // Transfers file, then reports that it ended. It never throws, and it gives its slot back whatever the caller's
        // progress handler or a callback on the transfer's token throws: a handler's failure, as the file starts, ends or
        // is counted, is the transfer's failure, unless it has one already.
        private async Task RunFileAsync(PlannedFile file, Func<PlannedFile, CancellationToken, Task> transfer, SemaphoreSlim slots)
        {
            try
            {
                try
                {
                    progress.Started(file);
                    await transfer(file, stop.Token);
                    if (!file.CompletedEarlier && !file.Acknowledged)
                    {
                        Completed(file);
                    }
                }
                catch (Exception e)
                {
                    Fail(e);
                }
                try
                {
                    progress.Ended(file);
                }
                catch (Exception e)
                {
                    Fail(e);
                }
            }
            finally
            {
                Interlocked.Decrement(ref filesInProgress);
                Interlocked.Exchange(ref fileEnded, new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)).TrySetResult(true);
                slots.Release();
            }
        }

        // Records the transfer's first failure and stops every file in progress. A file stopped by the transfer's own
        // stop, after the first failure or the caller's cancellation, has not failed.
        private void Fail(Exception e)
        {
            lock (gate)
            {
                if (failure == null && !(e is OperationCanceledException && stop.IsCancellationRequested))
                {
                    failure = ExceptionDispatchInfo.Capture(e);
                }
            }
            CancellationUtil.Stop(stop, log);
        }

        private async Task UploadFileAsync(LocalDirectory root, PlannedFile file, CancellationToken token)
        {
            string[] names = file.RelativePath.Split('/');
            using (LocalDirectory parent = OpenLocalFolder(root, names, names.Length - 1, create: false))
            using (FileUploadSource source = FileUploadSource.OfLocal(parent.OpenFileToRead(names[names.Length - 1])))
            {
                if (completed.TryGetValue(file.RelativePath, out CompletedFile done) && await StillUploadedAsync(source, done, token))
                {
                    file.Size = done.Size;
                    file.Parts = done.Parts;
                    Skip(file);
                    return;
                }
                // What the completed upload is recorded with: the file as opened, and the SHA-256 of each part as sent.
                file.Size = source.Length.Value;
                file.SourceModified = source.LastWriteTimeUtc.Ticks;
                UploadOptions options = settings.UploadFile.Snapshot();
                options.Limiter = settings.Limiter;
                options.SharedScopes = scopes;
                options.MkdirParents = options.MkdirParents ?? true;
                options.CheckpointPath = FileCheckpointPath(file, "upload");
                options.Progress = new FileProgress<TransferProgress>(settings.UploadFile.Progress, report => progress.Update(file, report.AcknowledgedBytes, report.SentBytes));
                UploadResult result = await UploadCoordinator.RunFileAsync(context, RemotePath(file.RelativePath), source, LocalPath(file.RelativePath), options, token);
                file.Parts = result.Parts.Select(part => new CompletedPart { Offset = part.Offset, Length = part.Length, Sha256 = part.Sha256 }).ToList();
            }
        }

        // Whether the local file still has the bytes an earlier run uploaded, read again and compared with the SHA-256 of
        // every part that was sent. Its length and modification time prove nothing: a file can be changed without either.
        // It is read through one buffer from the transfer's limiter, held until it has been read.
        private async Task<bool> StillUploadedAsync(FileUploadSource source, CompletedFile done, CancellationToken token)
        {
            if (done.Parts == null || source.Length != done.Size || done.Parts.Sum(part => part.Length) != done.Size || done.Parts.Any(part => string.IsNullOrEmpty(part.Sha256)))
            {
                return false;
            }
            long bufferBytes = UploadCoordinator.ReadBufferBytes(settings.Limiter, done.Size);
            using (await settings.Limiter.ReserveBufferAsync(bufferBytes, token))
            {
                byte[] buffer = new byte[bufferBytes];
                foreach (CompletedPart part in done.Parts)
                {
                    if (await UploadCoordinator.HasBytesAsync(source, part.Offset, part.Length, part.Sha256, buffer, token) != true)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Downloads file, with its options' publication policy and restart policy, unless an earlier run completed it and
        // it is still what that run wrote. With a checkpoint directory, the file's own checkpoint records it complete
        // before it is put in place, and is removed only once the transfer's record says the file is completed, with
        // the identity of the file it wrote (Acknowledge), so a run that ends between the two finds one or the other: a
        // later run recognizes the file its checkpoint records as this transfer's, finishes what that run left undone,
        // and then treats it as any completed file: it is recorded, as completed earlier, only once its storage confirms
        // its version is still the file's current one; otherwise it is downloaded again as the publication policy says,
        // CreateNew refusing to replace it. A file that is someone else's, even of the same length and time, is not
        // taken for this transfer's.
        private async Task DownloadFileAsync(LocalDirectory root, PlannedFile file, CancellationToken token)
        {
            long listedSize = file.Size;
            if (completed.TryGetValue(file.RelativePath, out CompletedFile done) && await StillCurrentAsync(root, file, listedSize, done, token))
            {
                file.Size = done.Size;
                file.Version = VersionOf(done);
                file.LocalModified = done.LocalModified;
                file.LocalIdentity = done.LocalIdentity;
                Skip(file);
                return;
            }
            string[] names = file.RelativePath.Split('/');
            using (LocalDirectory parent = OpenLocalFolder(root, names, names.Length - 1, create: true))
            {
                DownloadOptions template = settings.DownloadFile;
                DownloadOptions options = new DownloadOptions
                {
                    ConcurrencyMode = template.ConcurrencyMode,
                    MaxConcurrency = template.MaxConcurrency,
                    AutomaticConcurrency = template.AutomaticConcurrency,
                    SharedScopes = scopes,
                    PartSize = template.PartSize,
                    Limiter = settings.Limiter,
                    Progress = new FileProgress<DownloadProgress>(template.Progress, report =>
                    {
                        progress.SizeKnown(file, report.TotalBytesKnown ? report.TotalBytes : (long?)null);
                        progress.Update(file, report.WrittenBytes, report.ReceivedBytes);
                    }),
                    CheckpointPath = FileCheckpointPath(file, "download"),
                    PublicationPolicy = template.PublicationPolicy,
                    CheckpointRestartPolicy = template.CheckpointRestartPolicy,
                    AcknowledgedByCaller = record != null,
                    RequestOptions = template.RequestOptions,
                }.Snapshot();
                DownloadResult result = await DownloadCoordinator.ToFileInAsync(context, RemotePath(file.RelativePath), parent, names[names.Length - 1], options, token);
                if (result.CompletedEarlier && record != null)
                {
                    var placed = new CompletedFile { RelativePath = file.RelativePath, Size = result.Size, ETag = result.Version?.ETag, Source = result.Version?.Source, LocalIdentity = result.LocalIdentity };
                    if (!await StillCurrentAsync(root, file, listedSize, placed, token))
                    {
                        // The file an earlier run put in place is no longer Files.com's current one, or cannot be shown to
                        // be: that run is done with, and the file is downloaded again as the policy says.
                        log.Info("Downloading a file of a directory again: the file an earlier run put in place is not shown to be the current one");
                        CheckpointFile.Delete(options.CheckpointPath);
                        result = await DownloadCoordinator.ToFileInAsync(context, RemotePath(file.RelativePath), parent, names[names.Length - 1], options, token);
                    }
                }
                file.Size = result.Size;
                file.Version = result.Version;
                file.LocalIdentity = result.LocalIdentity;
                file.LocalModified = File.GetLastWriteTimeUtc(LocalPath(file.RelativePath)).Ticks;
                if (record != null)
                {
                    Acknowledge(file, options.CheckpointPath, recovered: result.CompletedEarlier);
                }
            }
        }

        // Records file as completed, with the file it wrote, then removes the file's own checkpoint. A file its checkpoint
        // showed an earlier run put in place is reported as completed earlier. A failure to record it leaves the file,
        // which is in place, and its checkpoint, which a later run finishes it from.
        private void Acknowledge(PlannedFile file, string checkpointPath, bool recovered)
        {
            try
            {
                Record(file);
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException)
            {
                throw new DownloadPublicationException(DownloadPublicationProblem.PlacedNotRecorded, LocalPath(file.RelativePath), $"The file {LocalPath(file.RelativePath)} was put in place, but the transfer's record of it could not be written: {e.Message}", e) { CheckpointPath = checkpointPath };
            }
            CheckpointFile.Delete(checkpointPath);
            file.Acknowledged = true;
            if (recovered)
            {
                Skip(file);
            }
            else
            {
                progress.Done(file);
            }
        }

        // Whether a file an earlier run downloaded is still what it wrote, and still the current version of it:
        // listed with the size it was written with, when the listing gives one; the local file is the one it wrote, the
        // same file on storage (its identity) of the same length, or, in a record an earlier SDK version wrote, with the
        // default publication policy only, of the same length and modification time; and its storage, through the URL
        // Files.com gives for it now, which must name the same resource, still serves the version written, by its entity
        // tag and size (DownloadCoordinator.IsCurrentAsync). A file downloaded without a version to name has no such
        // proof, so it is downloaded again. Nor has a file an earlier SDK version recorded without its identity,
        // with CreateNew or ReplaceRegularFile, which put in place only files shown to be this transfer's own: its length
        // and time do not show whose it is, so the policy decides, as for any file already there. The local file is
        // looked at before Files.com is asked, and again once it has answered, which takes as long as it does: it is
        // counted as completed only as it is then, where its path says (LocalFileIs).
        private async Task<bool> StillCurrentAsync(LocalDirectory root, PlannedFile file, long listedSize, CompletedFile done, CancellationToken token)
        {
            if (file.SizeKnown && listedSize != done.Size)
            {
                return false;
            }
            if (done.LocalIdentity == null && settings.DownloadFile.PublicationPolicy != DownloadPublicationPolicy.ReplaceExisting)
            {
                return false;
            }
            DownloadVersion written = VersionOf(done);
            if (written == null || !LocalFileStill(root, file.RelativePath, done))
            {
                return false;
            }
            bool current = await DownloadCoordinator.IsCurrentAsync(context, RemotePath(file.RelativePath), written, settings.Limiter, DictionaryUtil.Copy(settings.RequestOptions), token);
            return current && LocalFileStill(root, file.RelativePath, done);
        }

        // The version a completed download wrote, or null when it was not bound to one.
        private static DownloadVersion VersionOf(CompletedFile done)
        {
            return string.IsNullOrEmpty(done.ETag) || string.IsNullOrEmpty(done.Source) ? null : new DownloadVersion(done.ETag, done.Size, done.Source);
        }

        // Whether the local file of a completed download is still the one it wrote: by its identity, or, in a record an
        // earlier SDK version wrote, by the length and time of the file at its path.
        private bool LocalFileStill(LocalDirectory root, string relativePath, CompletedFile done)
        {
            return done.LocalIdentity != null ? LocalFileIs(root, relativePath, done) : LocalFileIs(LocalPath(relativePath), done);
        }

        // An earlier run completed file, and its source is shown to be unchanged: it is not transferred again.
        private void Skip(PlannedFile file)
        {
            file.CompletedEarlier = true;
            progress.Skipped(file);
        }

        private async Task CreateRemoteFolderAsync(string relativePath)
        {
            stop.Token.ThrowIfCancellationRequested();
            var parameters = new Dictionary<string, object> { { "mkdir_parents", true } };
            try
            {
                // Sent again after a lost answer: a folder created by the first request is only there already.
                await Folder.CreateCore(context.ForReplaySafeRequests(), RemotePath(relativePath), parameters, DictionaryUtil.Copy(settings.RequestOptions), stop.Token);
            }
            catch (DestinationExistsException)
            {
                // Already there.
            }
        }

        // Records file as completed, in the record if the transfer keeps one.
        private void Completed(PlannedFile file)
        {
            progress.Done(file);
            Record(file);
        }

        private void Record(PlannedFile file)
        {
            if (record == null)
            {
                return;
            }
            Exception fault = RecordFaults.Value?.Invoke(file.RelativePath);
            if (fault != null)
            {
                throw fault;
            }
            lock (gate)
            {
                completed[file.RelativePath] = new CompletedFile
                {
                    RelativePath = file.RelativePath,
                    Size = file.Size,
                    SourceModified = file.SourceModified,
                    LocalModified = file.LocalModified,
                    ETag = file.Version?.ETag,
                    Source = file.Version?.Source,
                    LocalIdentity = file.LocalIdentity,
                    Parts = file.Parts,
                };
                record.Completed = completed.Values.OrderBy(done => done.RelativePath, StringComparer.Ordinal).ToList();
                CheckpointFile.Write(RecordPath, record);
            }
        }

        private DirectoryTransferResult Complete(List<PlannedFile> files, List<string> empty, IReadOnlyList<string> excluded)
        {
            // Reported before the record goes: a handler that throws here fails a transfer whose record still says what
            // it completed.
            DirectoryTransferProgress final = progress.Report(force: true);
            if (record != null)
            {
                CheckpointFile.Delete(RecordPath);
            }
            var entries = files.Select(file => new DirectoryTransferEntry(file.RelativePath, LocalPath(file.RelativePath), RemotePath(file.RelativePath), file.Size, file.CompletedEarlier)).ToList();
            return new DirectoryTransferResult(entries, empty, excluded, final);
        }

        // Reads the record an earlier run of this transfer left, if there is one, refusing one of another transfer.
        private void OpenRecord(string kind)
        {
            if (checkpointDirectory == null)
            {
                return;
            }
            string site = CheckpointFile.SiteOf(context);
            string workspace = CheckpointFile.WorkspaceOf(context, settings.RequestOptions);
            string local = Path.GetFullPath(localDirectory);
            DirectoryCheckpoint saved = CheckpointFile.Read<DirectoryCheckpoint>(RecordPath, kind);
            if (saved != null)
            {
                string mismatch = saved.ScopeMismatch(site, workspace, remoteDirectory, local);
                if (mismatch != null)
                {
                    throw new TransferCheckpointException(RecordPath, TransferCheckpointProblem.OtherTransfer, mismatch);
                }
                string problem = saved.Problem();
                if (problem != null)
                {
                    throw new TransferCheckpointException(RecordPath, TransferCheckpointProblem.Unreadable, problem);
                }
                foreach (CompletedFile done in saved.Completed)
                {
                    completed[done.RelativePath] = done;
                }
            }
            record = new DirectoryCheckpoint { Kind = kind, Site = site, Workspace = workspace, RemotePath = remoteDirectory, LocalPath = local, Completed = completed.Values.ToList() };
            CheckpointFile.Write(RecordPath, record);
        }

        private string RecordPath => Path.Combine(checkpointDirectory, RecordName);

        // Where the file's own checkpoint is kept: named for its relative path, which may not be a valid local name.
        private string FileCheckpointPath(PlannedFile file, string kind)
        {
            if (checkpointDirectory == null)
            {
                return null;
            }
            using (SHA256 sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(file.RelativePath));
                return Path.Combine(checkpointDirectory, BitConverter.ToString(hash, 0, 16).Replace("-", "").ToLowerInvariant() + "." + kind + ".json");
            }
        }

        // Refuses a checkpoint directory inside the local directory, which the transfer would then also transfer, by its
        // name, and, where directories are held by descriptor, as it is on storage, whatever links its name goes through;
        // then creates it, if missing, as a directory only this user can use (CheckpointStorage). Nothing is created in
        // the local directory.
        private void CheckCheckpointDirectory(LocalDirectory root)
        {
            if (checkpointDirectory == null)
            {
                return;
            }
            string inside = Path.GetFullPath(root.Path).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if ((checkpointDirectory + Path.DirectorySeparatorChar).StartsWith(inside, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)
                || CheckpointStorage.IsWithin(checkpointDirectory, root))
            {
                throw InvalidParameterException.For(nameof(DirectoryTransferOptions.CheckpointDirectory), $"The checkpoint directory {checkpointDirectory} is inside the local directory {localDirectory}, which the transfer would then also transfer");
            }
            CheckpointStorage.CreateDirectory(checkpointDirectory);
        }

        private string RemotePath(string relativePath)
        {
            return remoteDirectory.Length == 0 ? relativePath : remoteDirectory + "/" + relativePath;
        }

        private string LocalPath(string relativePath)
        {
            return Path.Combine(new[] { Path.GetFullPath(localDirectory) }.Concat(relativePath.Split('/')).ToArray());
        }

        private static string RemoteFolder(string remoteDirectory)
        {
            return (remoteDirectory ?? "").Trim('/');
        }

        // The folder at the first count names under root, entered one name at a time, as a handle of its own.
        private static LocalDirectory OpenLocalFolder(LocalDirectory root, string[] names, int count, bool create)
        {
            LocalDirectory current = root.Duplicate();
            for (int i = 0; i < count; i++)
            {
                LocalDirectory next;
                try
                {
                    next = current.OpenChild(names[i], create);
                }
                finally
                {
                    current.Dispose();
                }
                current = next;
            }
            return current;
        }

        // Lists the local tree under directory into files, empty folders and excluded entries. Returns whether anything
        // under it is transferred.
        private static bool WalkLocal(string directory, string relative, List<PlannedFile> files, List<string> empty, List<string> excluded)
        {
            bool any = false;
            foreach (FileSystemInfo entry in new DirectoryInfo(directory).GetFileSystemInfos().OrderBy(entry => entry.Name, StringComparer.Ordinal))
            {
                string child = relative.Length == 0 ? entry.Name : relative + "/" + entry.Name;
                if ((entry.Attributes & (FileAttributes.ReparsePoint | FileAttributes.Device)) != 0)
                {
                    excluded.Add(child);
                }
                else if (entry is DirectoryInfo folder)
                {
                    if (!WalkLocal(folder.FullName, child, files, empty, excluded))
                    {
                        empty.Add(child);
                    }
                    any = true;
                }
                else if (entry is FileInfo file)
                {
                    files.Add(new PlannedFile(child, file.Length, file.LastWriteTimeUtc.Ticks));
                    any = true;
                }
            }
            return any;
        }

        // Lists the remote tree, a folder at a time, into files and empty folders, every page within the transfer's own
        // operation context.
        private async Task WalkRemoteAsync(List<PlannedFile> files, List<string> empty)
        {
            var folders = new Queue<string>();
            folders.Enqueue("");
            while (folders.Count > 0)
            {
                string relative = folders.Dequeue();
                string folder = relative.Length == 0 ? remoteDirectory : RemotePath(relative);
                var entries = new List<RemoteFile>();
                FilesList<RemoteFile> list = Folder.ListForCore(context.Client, folder, new Dictionary<string, object>(), null);
                await foreach (RemoteFile entry in list.ListWithinAsync(context, settings.RequestOptions, stop.Token).ConfigureAwait(false))
                {
                    entries.Add(entry);
                }
                if (entries.Count == 0 && relative.Length > 0)
                {
                    empty.Add(relative);
                }
                foreach (RemoteFile entry in entries.OrderBy(entry => NameOf(folder, entry), StringComparer.Ordinal))
                {
                    string name = NameOf(folder, entry);
                    string child = relative.Length == 0 ? name : relative + "/" + name;
                    if (entry.Type == "directory")
                    {
                        folders.Enqueue(child);
                    }
                    else
                    {
                        files.Add(new PlannedFile(child, entry.Size ?? 0, entry.Mtime?.ToUniversalTime().Ticks) { SizeKnown = entry.Size.HasValue });
                    }
                }
            }
            files.Sort((a, b) => string.CompareOrdinal(a.RelativePath, b.RelativePath));
            empty.Sort(StringComparer.Ordinal);
        }

        // The name of a listed entry of folder, checked to be one this system's files can have.
        private static string NameOf(string folder, RemoteFile entry)
        {
            string path = entry.Path ?? "";
            int slash = path.LastIndexOf('/');
            string name = path.Substring(slash + 1);
            string parent = slash < 0 ? "" : path.Substring(0, slash);
            if (!string.Equals(parent, folder, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidResponseException($"Files.com listed {path} as inside {folder}, which it is not.");
            }
            if (!IsLocalName(name))
            {
                throw new IOException($"{path} cannot be downloaded: \"{name}\" cannot be the name of a local file here, so nothing was downloaded.");
            }
            return name;
        }

        private static bool IsLocalName(string name)
        {
            if (string.IsNullOrEmpty(name) || name == "." || name == ".." || name.IndexOf('/') >= 0 || name.IndexOf('\0') >= 0)
            {
                return false;
            }
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return true;
            }
            // The reserved device names, with any extension: the serial and printer ports are COM0 to COM9 and LPT0 to
            // LPT9, and the superscript digits ¹ ² ³, which Windows reads as those ports too.
            string stem = name.Split('.')[0].TrimEnd(' ').ToUpperInvariant();
            bool device = stem == "CON" || stem == "PRN" || stem == "AUX" || stem == "NUL"
                || (stem.Length == 4 && (stem.StartsWith("COM", StringComparison.Ordinal) || stem.StartsWith("LPT", StringComparison.Ordinal))
                    && ((stem[3] >= '0' && stem[3] <= '9') || stem[3] == '¹' || stem[3] == '²' || stem[3] == '³'));
            return !device && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && !name.EndsWith(".", StringComparison.Ordinal) && !name.EndsWith(" ", StringComparison.Ordinal);
        }

        // Whether the local file at relativePath under root is still the one a download wrote: the same file on storage,
        // looked at without following links, of the same length. Found so in a folder the path this transfer reports for it
        // no longer leads to, as when the root, opened before the listing, was renamed or replaced since, it is in place,
        // but not there: that fails the transfer (DestinationMoved, placed), leaving the file and every checkpoint of it
        // as they are.
        private bool LocalFileIs(LocalDirectory root, string relativePath, CompletedFile done)
        {
            string[] names = relativePath.Split('/');
            LocalDirectory parent;
            try
            {
                parent = OpenLocalFolder(root, names, names.Length - 1, create: false);
            }
            catch (IOException)
            {
                return false;
            }
            using (parent)
            {
                LocalEntry entry;
                try
                {
                    entry = parent.Examine(names[names.Length - 1]);
                }
                catch (IOException)
                {
                    return false;
                }
                if (entry == null || !entry.Regular || !FileIdentity.Same(done.LocalIdentity, entry.Identity) || entry.Size != done.Size)
                {
                    return false;
                }
                if (!parent.StillAtPath())
                {
                    string path = LocalPath(relativePath);
                    throw new DownloadPublicationException(DownloadPublicationProblem.DestinationMoved, path, $"The file an earlier run of this transfer downloaded to {path} is in place in the folder the transfer opened, but that folder is no longer at {parent.Path}: it was renamed, or replaced by another, so the file is not at {path}. It is not removed, and the transfer's checkpoint is kept.", placed: true);
                }
                return true;
            }
        }

        // In a record an earlier SDK version wrote: whether the local file at path still has the length and modification
        // time a download wrote it with.
        private static bool LocalFileIs(string path, CompletedFile done)
        {
            var file = new FileInfo(path);
            return file.Exists && (file.Attributes & FileAttributes.ReparsePoint) == 0 && file.Length == done.Size && file.LastWriteTimeUtc.Ticks == done.LocalModified;
        }

        // One file of the transfer, as planned.
        internal sealed class PlannedFile
        {
            internal PlannedFile(string relativePath, long size, long? sourceModified)
            {
                RelativePath = relativePath;
                Size = size;
                SourceModified = sourceModified;
            }

            internal string RelativePath { get; }

            // The file's size: as listed, then as transferred.
            internal long Size { get; set; }

            internal bool SizeKnown { get; set; } = true;

            internal long? SourceModified { get; set; }

            internal long? LocalModified { get; set; }

            // The version a download wrote, or null when it was not bound to one.
            internal DownloadVersion Version { get; set; }

            // An upload's parts, with the SHA-256 of their bytes as sent.
            internal List<CompletedPart> Parts { get; set; }

            // A download's local file, by its identity on storage (FileIdentity).
            internal string LocalIdentity { get; set; }

            internal bool CompletedEarlier { get; set; }

            // Recorded as completed already, by the file's own transfer (Acknowledge).
            internal bool Acknowledged { get; set; }
        }

        // Forwards a file's progress to the transfer and then to the caller's handler for files, if any.
        private sealed class FileProgress<T> : IProgress<T>
        {
            private readonly IProgress<T> caller;
            private readonly Action<T> transfer;

            internal FileProgress(IProgress<T> caller, Action<T> transfer)
            {
                this.caller = caller;
                this.transfer = transfer;
            }

            public void Report(T value)
            {
                transfer(value);
                caller?.Report(value);
            }
        }
    }
}