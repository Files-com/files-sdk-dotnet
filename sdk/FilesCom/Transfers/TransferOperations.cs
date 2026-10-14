using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    /// <summary>
    /// File transfers that run with one client: <c>client.Transfers</c>.
    /// </summary>
    /// <remarks>
    /// An upload is sent in parts. Their sizes are the ones Files.com plans, unless the parts go straight to Amazon S3
    /// as an ordinary multipart upload: then, by default, the upload chooses a size for the file and makes later parts
    /// larger while that raises throughput (<see cref="UploadPartSizingMode.Automatic"/>, tuned by
    /// <see cref="AutomaticPartSizingOptions"/>), within <see cref="PartSizeLimits"/>. When the server allows
    /// it, several parts are sent at the same time, within <see cref="UploadOptions.MaxConcurrency"/> and the request
    /// slots and buffer space of the upload's <see cref="TransferLimiter"/>; otherwise parts are sent one at a time,
    /// in order. By default the
    /// upload chooses how many at once from what it measures (<see cref="UploadConcurrencyMode.Automatic"/>, tuned by
    /// <see cref="AutomaticConcurrencyOptions"/>): it adds parts while they raise throughput, and after failures,
    /// throttling or rising latency sends fewer; after throttling it also pauses new part requests, retries included.
    /// <see cref="UploadConcurrencyMode.Static"/> keeps the number fixed. A part that fails with a
    /// connection failure, a timeout, a server error (5xx other than 501), a request timeout (408) or throttling (429)
    /// is sent again, with the same bytes, when the server allows retrying parts, up to the client's
    /// <c>MaxNetworkRetries</c>. After 429 or 503 it first waits what the server's <c>Retry-After</c> asks, however
    /// long, or not at all without one; when the wait is longer than the platform can schedule, the part is not sent
    /// again and the throttling error is thrown. After other failures it waits 100 milliseconds. A part whose URL is
    /// about to expire gets a new one, for the same part, before it is sent. Parts already accepted are never sent
    /// again. The upload is completed with a single final request once every part has been accepted.
    /// <para>
    /// Every request uses the client's settings as they were when the upload started. The part requests go to the
    /// storage URLs Files.com provides and carry no Files.com credentials. A part follows a redirect only if it can go
    /// on unchanged (the same method and bytes, never from HTTPS to HTTP); headers negotiated for a part go with it
    /// within the same origin, and its credential headers do not leave that origin.
    /// </para>
    /// <para>
    /// A part request fails when it stalls: when it can send nothing for the client's <c>WriteTimeout</c>, or waits
    /// longer than its <c>ReadTimeout</c> for the answer. How long a part or the upload takes is not limited; to limit
    /// that, use a token that cancels itself, for example from <see cref="CancellationTokenSource(TimeSpan)"/>.
    /// Cancelling the token, or disposing the client, stops the upload wherever it is: waiting for the limiter or a
    /// cooldown, reading, sending, waiting for an answer or waiting to retry. The
    /// upload then ends with an <see cref="OperationCanceledException"/> once every part in flight has stopped. A
    /// failure also stops every other part, and the upload ends with the first failure. In either case the upload is
    /// not completed, unless its final request had already been sent: if that request fails in a way that does not
    /// show whether Files.com completed the file, the upload ends with <see cref="UploadCompletionUnknownException"/>;
    /// if it is cancelled, whether the file was completed is also unknown.
    /// </para>
    /// </remarks>
    public sealed class TransferOperations
    {
        private readonly FilesClient client;

        internal TransferOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="localPath">The local file to upload.</param>
        /// <param name="destinationPath">The remote path to upload to. If null, the local file's name.</param>
        /// <param name="options">Concurrency, shared limits, progress and other settings. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops the upload.</param>
        /// <returns>The completed upload.</returns>
        /// <remarks>
        /// <para>
        /// The file is opened once and every part, on every attempt, is read through that handle, so renaming or
        /// replacing the file during the upload does not change what is uploaded. The handle fixes the length uploaded
        /// and, on Windows, prevents other programs from changing or deleting the file meanwhile; elsewhere, a program
        /// that writes into the file itself changes what later reads return, so leave the file unchanged until the
        /// upload ends. Parts are read a copy buffer at a time, so the file's contents are never held in memory. The
        /// handle is closed when the task ends, however it ends.
        /// </para>
        /// <para>
        /// With <see cref="UploadOptions.CheckpointPath"/>, the upload records in that file its Files.com upload, its
        /// limits, every part as it is planned, and the ETag and SHA-256 of each part Files.com accepts: at most every two
        /// seconds while it runs, and whenever it stops, each time replacing the file whole and flushing it to storage.
        /// When it is cancelled, fails or is interrupted, running it again with the same client site, workspace, paths
        /// and checkpoint continues it: it first reads the accepted parts from the file again, and continues only if each
        /// still has the bytes that were sent; then parts Files.com accepted are not sent again, parts planned and not
        /// accepted are sent again exactly as planned, and <see cref="UploadResult.ResumedParts"/> counts what was
        /// reused. When the upload cannot be continued (the file changed, Files.com no longer knows the upload, it no
        /// longer allows it what it did, or a part the checkpoint records, whose size the upload chose, is outside the
        /// limits) it fails with <see cref="TransferCheckpointException"/> before any part is sent,
        /// unless <see cref="UploadOptions.CheckpointRestartPolicy"/> is <see cref="CheckpointRestartPolicy.StartOver"/>:
        /// then it starts over, as a new upload. Parts of two uploads are never completed together. The checkpoint holds
        /// no credentials or signed URLs, and is deleted once the upload is completed. That the final request is being
        /// sent is recorded, and flushed, before it is sent: a run that finds that recorded does not send anything, and
        /// fails with <see cref="UploadCompletionUnknownException"/>, since only the file at the destination can show
        /// whether the upload was completed. On macOS and Linux, a checkpoint whose directory cannot be flushed to storage
        /// fails the upload, saying so, and nothing that depends on it is sent: the final request among them.
        /// </para>
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="localPath"/> is null.</exception>
        /// <exception cref="InvalidParameterException">A setting of <paramref name="options"/> is out of range, or settings of <paramref name="options"/> contradict each other, such as a <see cref="AutomaticConcurrencyOptions.StartTarget"/> above its <see cref="AutomaticConcurrencyOptions.SoftCeiling"/>.</exception>
        /// <exception cref="TransferCheckpointException">The checkpoint belongs to another transfer, cannot be read as one, is kept where another user could have changed it, or records an upload that cannot be continued (<see cref="TransferCheckpointException.Problem"/>). Nothing was sent.</exception>
        /// <exception cref="EndOfStreamException">The file became shorter during the upload. It was not completed.</exception>
        /// <exception cref="UploadLimitException">
        /// With a fixed part size, the file needs more parts than its limits allow (<see cref="UploadLimit.PartCount"/>)
        /// or the size cannot be used (<see cref="UploadLimit.FixedPartSize"/>); or a part's copy buffer is more than the
        /// limiter may buffer (<see cref="UploadLimit.BufferSpace"/>). No part was sent.
        /// </exception>
        /// <exception cref="UploadCompletionUnknownException">The final request failed after it may have reached Files.com, or the checkpoint records that an earlier run sent it.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public Task<UploadResult> UploadFileAsync(string localPath, string destinationPath = null, UploadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (localPath == null)
            {
                throw MissingParameterException.For(nameof(localPath));
            }
            string path = destinationPath ?? Path.GetFileName(localPath);
            UploadOptions settings = (options ?? new UploadOptions()).Snapshot();
            var context = new OperationContext(client);

            // On the thread pool, so the upload's own tasks never wait for the caller's synchronization context.
            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (OperationContext.OperationScope upload = context.Begin(cancellationToken))
                using (FileUploadSource source = FileUploadSource.Open(localPath))
                {
                    return await UploadCoordinator.RunFileAsync(context, path, source, localPath, settings, upload.Token);
                }
            });
        }

        /// <summary>
        /// Uploads <paramref name="length"/> bytes read from <paramref name="source"/> to <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="source">
        /// The stream to read, from its current position. It is read forward only, one part after another, and is
        /// never seeked, so it may be forward-only. It is borrowed: the upload does not dispose it. After a failure or
        /// cancellation its position is undefined.
        /// </param>
        /// <param name="length">The number of bytes to upload.</param>
        /// <param name="destinationPath">The remote path to upload to.</param>
        /// <param name="options">Concurrency, shared limits, progress and other settings. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops the upload.</param>
        /// <returns>The completed upload.</returns>
        /// <remarks>
        /// Each part is kept until the server accepts it, so a retry resends exactly the same bytes: by default in
        /// memory, where the parts held at any time are limited by the limiter's <see cref="TransferLimiter.MaxBufferedBytes"/>
        /// and a part larger than that fails the upload before the part is read; with
        /// <see cref="StreamPartStorage.TemporaryFiles"/>, in a temporary file of its own. A file upload holds neither.
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="source"/> or <paramref name="destinationPath"/> is null.</exception>
        /// <exception cref="InvalidParameterException"><paramref name="source"/> cannot be read, <paramref name="length"/> is negative, a setting of <paramref name="options"/> is out of range, settings of <paramref name="options"/> contradict each other, or <see cref="UploadOptions.CheckpointPath"/> is set: only a file upload can be continued.</exception>
        /// <exception cref="EndOfStreamException">The stream ended before <paramref name="length"/> bytes were read. The upload was not completed.</exception>
        /// <exception cref="UploadCompletionUnknownException">The final request failed after it may have reached Files.com.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        /// <exception cref="UploadLimitException">
        /// The stream needs more parts than its limits allow, with parts the limiter can hold in memory or of a fixed
        /// size (<see cref="UploadLimit.PartCount"/>); the fixed part size cannot be used (<see cref="UploadLimit.FixedPartSize"/>);
        /// or a part is more than the limiter may buffer (<see cref="UploadLimit.BufferSpace"/>) or an array can hold
        /// (<see cref="UploadLimit.MemoryPart"/>). No part was sent, and the part was not read.
        /// </exception>
        public Task<UploadResult> UploadStreamAsync(Stream source, long length, string destinationPath, UploadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                throw MissingParameterException.For(nameof(source));
            }
            if (!source.CanRead)
            {
                throw InvalidParameterException.For(nameof(source), "The stream cannot be read");
            }
            if (length < 0)
            {
                throw InvalidParameterException.OutOfRange(nameof(length), length, "The length to upload cannot be negative");
            }
            if (destinationPath == null)
            {
                throw MissingParameterException.For(nameof(destinationPath));
            }
            if (options?.CheckpointPath != null)
            {
                throw InvalidParameterException.For(nameof(UploadOptions.CheckpointPath), "Only an upload of a local file can be continued: a stream upload cannot have a CheckpointPath");
            }
            UploadOptions settings = (options ?? new UploadOptions()).Snapshot();
            var context = new OperationContext(client);

            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (OperationContext.OperationScope upload = context.Begin(cancellationToken))
                using (var stream = new StreamUploadSource(source, length, settings.StreamPartStorage, settings.TemporaryDirectory))
                {
                    return await UploadCoordinator.RunAsync(context, destinationPath, stream, settings, upload.Token);
                }
            });
        }

        /// <summary>
        /// Uploads everything <paramref name="source"/> gives, until it ends, to <paramref name="destinationPath"/>,
        /// for a stream whose length is not known in advance.
        /// </summary>
        /// <param name="source">
        /// The stream to read, from its current position, until a read returns no bytes. Its length is never asked for
        /// and it is never seeked, so it may be forward-only. It is borrowed: the upload does not dispose it. After a
        /// failure or cancellation its position is undefined.
        /// </param>
        /// <param name="destinationPath">The remote path to upload to.</param>
        /// <param name="options">Concurrency, shared limits, spooling, progress and other settings. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops the upload.</param>
        /// <returns>The completed upload, whose <see cref="UploadResult.Size"/> is the number of bytes the stream gave.</returns>
        /// <remarks>
        /// <para>
        /// The upload starts without a size. The stream is read one part at a time, in order, short reads accumulating
        /// until a part is full or the stream ends, and never further than the part being read. When the stream ends
        /// exactly where a part does, the next part's read returns nothing and that part is not sent, though Files.com
        /// may already have given its URL. Each part is kept until the server accepts it, as
        /// <see cref="UploadOptions.StreamPartStorage"/> says, so a retry resends exactly the same bytes.
        /// </para>
        /// <para>
        /// Where the upload chooses part sizes, the parts are sent as a file's are: several at once where Files.com
        /// allows, starting at <see cref="AutomaticPartSizingOptions.UnknownLengthStartSize"/> and growing as
        /// <see cref="UploadPartSizingMode.Automatic"/> describes. Every part is sized, before it is read, so that the
        /// most the stream may hold, <see cref="UploadOptions.MaxStreamLength"/> or the largest file the limits allow,
        /// still fits in the parts that remain. Parts in memory are limited by the limiter's buffer space: when that
        /// makes the bound impossible to fit, the upload uses the sizes Files.com plans instead, as
        /// <see cref="ServerPlannedReason.MemoryLimit"/> reports; declare a smaller <see cref="UploadOptions.MaxStreamLength"/>,
        /// or keep parts in <see cref="StreamPartStorage.TemporaryFiles"/>. Otherwise, Files.com plans each part in turn,
        /// and the parts are sent one at a time, in order, whatever the server allows, as Files.com sizes them, within
        /// <see cref="UploadOptions.MaxStreamLength"/> and the parts Files.com makes available. A destination that needs
        /// to know the size in advance refuses the upload before anything is sent.
        /// </para>
        /// <para>
        /// A stream that gives more than its bound, or than its parts can hold, fails the upload once that is found,
        /// without sending the excess, and the upload is not completed; parts sent before then are not undone. With
        /// <see cref="UploadOptions.SpoolMaxBytes"/>, the stream is instead copied first to a temporary file, and the
        /// upload starts only once the copy is complete: nothing is sent if the stream is longer, cannot be read, or the
        /// upload is cancelled meanwhile. The copy's buffer is taken from the <see cref="UploadOptions.Limiter"/>'s buffer
        /// space, if one was given, before the stream is read. The file is then uploaded with its size known, like a
        /// local file, and deleted when the upload ends, however it ends.
        /// </para>
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="source"/> or <paramref name="destinationPath"/> is null.</exception>
        /// <exception cref="InvalidParameterException"><paramref name="source"/> cannot be read, a setting of <paramref name="options"/> is out of range, settings of <paramref name="options"/> contradict each other, or <see cref="UploadOptions.CheckpointPath"/> is set: only a file upload can be continued.</exception>
        /// <exception cref="UploadLimitException">
        /// The stream is longer than <see cref="UploadOptions.MaxStreamLength"/> or the largest file its limits allow
        /// (<see cref="UploadLimit.StreamLength"/>), than the parts it allows can hold (<see cref="UploadLimit.PartCount"/>),
        /// or than <see cref="UploadOptions.SpoolMaxBytes"/> (<see cref="UploadLimit.SpoolLength"/>); the fixed part size
        /// cannot be used (<see cref="UploadLimit.FixedPartSize"/>); or a part is more than the limiter may buffer
        /// (<see cref="UploadLimit.BufferSpace"/>) or an array can hold (<see cref="UploadLimit.MemoryPart"/>). The upload
        /// was not completed.
        /// </exception>
        /// <exception cref="UploadCompletionUnknownException">The final request failed after it may have reached Files.com.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public Task<UploadResult> UploadStreamAsync(Stream source, string destinationPath, UploadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                throw MissingParameterException.For(nameof(source));
            }
            if (!source.CanRead)
            {
                throw InvalidParameterException.For(nameof(source), "The stream cannot be read");
            }
            if (destinationPath == null)
            {
                throw MissingParameterException.For(nameof(destinationPath));
            }
            if (options?.CheckpointPath != null)
            {
                throw InvalidParameterException.For(nameof(UploadOptions.CheckpointPath), "Only an upload of a local file can be continued: a stream upload cannot have a CheckpointPath");
            }
            UploadOptions settings = (options ?? new UploadOptions()).Snapshot();
            var context = new OperationContext(client);

            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (OperationContext.OperationScope upload = context.Begin(cancellationToken))
                using (UploadSource stream = settings.SpoolMaxBytes.HasValue
                    ? new SpooledStreamSource(source, Math.Min(settings.SpoolMaxBytes.Value, settings.MaxStreamLength ?? long.MaxValue), settings.TemporaryDirectory)
                    : (UploadSource)new StreamUploadSource(source, null, settings.StreamPartStorage, settings.TemporaryDirectory))
                {
                    return await UploadCoordinator.RunAsync(context, destinationPath, stream, settings, upload.Token);
                }
            });
        }

        /// <summary>
        /// Downloads the file at <paramref name="remotePath"/> to the local file <paramref name="localPath"/>, replacing
        /// any file there once the download is complete.
        /// </summary>
        /// <param name="remotePath">The remote path to download.</param>
        /// <param name="localPath">The local file to write. Its directory must exist.</param>
        /// <param name="options">Concurrency, part size, shared limits, progress, a checkpoint and other settings. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops the download.</param>
        /// <returns>The completed download.</returns>
        /// <remarks>
        /// <para>
        /// The download is bound to the version of the file its storage has now, where the storage checks a request's
        /// <c>If-Match</c> against the object its URL names, as Amazon S3, where Files.com stores files, does, at an
        /// HTTPS URL of an S3 general-purpose endpoint; the URL the first part's answer comes from, renewed or not,
        /// must be one. The first part is asked for alone, and its answer's strong entity tag, the file's size it
        /// states and the storage object are the version. The other parts are then fetched in parts of
        /// <see cref="DownloadOptions.PartSize"/>, several at once, each conditioned on that entity tag, and every part
        /// must come back as exactly those bytes of that version, or the download stops: parts of two versions are
        /// never put together. A part that fails in a way that may be temporary is fetched again alone, up to the
        /// client's <c>MaxNetworkRetries</c>, and a download URL about to expire is renewed, for the same storage
        /// object. Otherwise, such as for a file Files.com serves from other storage itself, the file comes in one
        /// request, which starts again from the first byte if it fails partway. When its answer declares no length,
        /// the file must have exactly the size Files.com stated for it with the download URL, or the download fails
        /// with <see cref="InvalidResponseException"/>, without another attempt, even when the file only changed
        /// meanwhile; the same size does not show that the bytes are of the file Files.com described. Without a stated
        /// size, what arrives before such an answer ends is taken as the whole file.
        /// </para>
        /// <para>
        /// The file is written into a stage, a temporary file in a private directory beside <paramref name="localPath"/>,
        /// and put in place under its name only once every byte has been written and checked, as
        /// <see cref="DownloadOptions.PublicationPolicy"/> says: by default replacing a file of that name, or a symbolic
        /// link, without following it; with <see cref="DownloadPublicationPolicy.CreateNew"/> only where nothing is there;
        /// with <see cref="DownloadPublicationPolicy.ReplaceRegularFile"/> replacing only a regular file, as PowerShell's
        /// <c>-Force</c> does. Until then the name is untouched: a download that fails or is cancelled, even from its last
        /// progress report, leaves no partial file there and does not succeed; a progress report of the last byte written
        /// does not mean the file is in place. On macOS and
        /// Linux the directory is used as it was opened; the staging directory and the stage are owner-only, without an
        /// access control list, and cannot be replaced by another user who can write into the directory; a file replaced
        /// keeps its access control list, owner, group and permissions, read from that very file; a file whose owner or
        /// group this user cannot give the new file, or, on macOS, that this user can neither read nor write, is not
        /// replaced, and the download fails before anything is replaced; a new file gets a new file's permissions; and
        /// the file and then its directory are flushed to storage as it is put in place. On Windows the stage has an
        /// access control list of its own that allows only this user, SYSTEM and Administrators until it is put in place,
        /// when it takes the replaced file's list, for <see cref="DownloadPublicationPolicy.ReplaceRegularFile"/> and the
        /// default, or the one a new file in the directory gets; its staging directory is removed only while it is the
        /// one the download made; and only the file is flushed. Whatever the policy, the directory, and every directory
        /// above it, is held from the moment the download starts, so that none of them can be renamed or replaced
        /// meanwhile, and the stage is renamed through its own handle; a directory that cannot be held from the root of
        /// its volume or share is refused (<see cref="DownloadPublicationProblem.NotSupported"/>). On Linux where the runtime cannot call the C
        /// library (such as .NET Core 2.x, with the netstandard2.0 build), or on a processor other than x64, x86, Arm64 and
        /// 32-bit Arm (such as ppc64le or s390x), it is used by path too, an existing file is not replaced, and the other
        /// policies fail (<see cref="DownloadPublicationProblem.NotSupported"/>). A destination a policy refuses fails
        /// with <see cref="DownloadPublicationException"/>, which says why, and whether the file was put in place.
        /// </para>
        /// <para>
        /// With <see cref="DownloadOptions.CheckpointPath"/>, a download that is cancelled, fails or is interrupted
        /// records its bound version and the parts it wrote, and keeps its stage. Running the download again with the
        /// same arguments and checkpoint continues it, fetching only the parts not yet written, once its storage
        /// confirms, for the file's first byte, that the version is still the file's; if it is not, or if Files.com now
        /// gives the file's URL for another storage object, or for storage this SDK does not bind downloads to (such as
        /// one an earlier version recorded), the download starts over in its own stage, from the file's first byte, as
        /// a download of the file as it is now: nothing of the old version was put in place. Cancelling is therefore
        /// also pausing. A download not bound to a version always starts over. Once every byte is written, the
        /// checkpoint records the stage complete, and which file on storage it is, before it is put in place: a
        /// download the policy refuses then keeps it (<see cref="DownloadPublicationException.Retained"/>), so running
        /// it again puts it in place, once the version is confirmed, without fetching it again; and a run that finds
        /// the file already in place, the very file an earlier run put there, completes without fetching it
        /// (<see cref="DownloadResult.CompletedEarlier"/>). The checkpoint is deleted once the download is complete. A
        /// checkpoint that records a download that cannot be continued fails, or is started over, as
        /// <see cref="DownloadOptions.CheckpointRestartPolicy"/> says.
        /// </para>
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="remotePath"/> or <paramref name="localPath"/> is null.</exception>
        /// <exception cref="InvalidParameterException"><paramref name="localPath"/> names no file, a setting of <paramref name="options"/> is out of range, or settings of <paramref name="options"/> contradict each other.</exception>
        /// <exception cref="DownloadSourceChangedException">The file changed during the download, so its version can no longer be downloaded. Nothing was put in place.</exception>
        /// <exception cref="TransferCheckpointException">The checkpoint cannot be used, is kept where another user could have changed it, or records another transfer. Nothing was downloaded.</exception>
        /// <exception cref="DownloadPublicationException">The file was not put in place as the publication policy says, or was put in place without certainty that it will survive a crash or was recorded.</exception>
        /// <exception cref="InvalidResponseException">The storage server's answer was not the file's bytes, such as the Files.com web app's page, or declared no Content-Length and held another number of bytes than Files.com stated for the file.</exception>
        /// <exception cref="ApiConnectionException">The connection failed, while the file was being asked for or while it arrived, and still failed when retried; a <see cref="NetworkTimeoutException"/> when it stalled. Nothing was put in place.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public Task<DownloadResult> DownloadFileAsync(string remotePath, string localPath, DownloadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (remotePath == null)
            {
                throw MissingParameterException.For(nameof(remotePath));
            }
            if (localPath == null)
            {
                throw MissingParameterException.For(nameof(localPath));
            }
            DownloadOptions settings = (options ?? new DownloadOptions()).Snapshot();
            var context = new OperationContext(client);

            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (OperationContext.OperationScope download = context.Begin(cancellationToken))
                {
                    return await DownloadCoordinator.ToFileAsync(context, remotePath, localPath, settings, download.Token);
                }
            });
        }

        /// <summary>
        /// Downloads the file at <paramref name="remotePath"/> into <paramref name="destination"/>, from its current position.
        /// </summary>
        /// <param name="remotePath">The remote path to download.</param>
        /// <param name="destination">
        /// The stream to write. It is borrowed: the download never seeks, flushes or disposes it, and its failures are
        /// thrown as it threw them.
        /// </param>
        /// <param name="options">Concurrency, part size, shared limits, progress and other settings. If null, the defaults. A checkpoint cannot be used.</param>
        /// <param name="cancellationToken">Stops the download.</param>
        /// <returns>The completed download.</returns>
        /// <remarks>
        /// When the download is bound to a version, as for a local file, parts are fetched several at once, and
        /// written to the stream in order: each is kept in memory, within the limiter's buffer space, until the parts
        /// before it have been written. Only whole, checked parts are written, so after a failure the stream holds the
        /// file's first parts and nothing else. Otherwise the file comes in one request and is written as it arrives; that
        /// request is retried only while nothing has been written, since the stream cannot be taken back. An answer to it
        /// that declares no length is checked against the size Files.com stated for the file, as for a local file: the
        /// stream then holds what was written before the difference showed, which for a shorter body is all of it.
        /// <see cref="DownloadResult.Size"/> and <see cref="DownloadProgress.TotalBytes"/> give the file's length, known
        /// before its first byte is written whenever the storage server states it.
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="remotePath"/> or <paramref name="destination"/> is null.</exception>
        /// <exception cref="InvalidParameterException"><paramref name="destination"/> cannot be written, <see cref="DownloadOptions.CheckpointPath"/> or a <see cref="DownloadOptions.PublicationPolicy"/> other than the default is set, a setting of <paramref name="options"/> is out of range, or settings of <paramref name="options"/> contradict each other.</exception>
        /// <exception cref="DownloadSourceChangedException">The file changed during the download, so its version can no longer be downloaded.</exception>
        /// <exception cref="InvalidResponseException">The storage server's answer was not the file's bytes, such as the Files.com web app's page, or declared no Content-Length and held another number of bytes than Files.com stated for the file.</exception>
        /// <exception cref="ApiConnectionException">The connection failed, while the file was being asked for or while it arrived, and still failed when retried, or once any of it had been written to <paramref name="destination"/>, which is not written again; a <see cref="NetworkTimeoutException"/> when it stalled. The stream holds what was written before. A failure of <paramref name="destination"/> itself is thrown as it threw it.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public Task<DownloadResult> DownloadStreamAsync(string remotePath, Stream destination, DownloadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (remotePath == null)
            {
                throw MissingParameterException.For(nameof(remotePath));
            }
            if (destination == null)
            {
                throw MissingParameterException.For(nameof(destination));
            }
            if (!destination.CanWrite)
            {
                throw InvalidParameterException.For(nameof(destination), "The stream cannot be written");
            }
            if (options?.CheckpointPath != null)
            {
                throw InvalidParameterException.For(nameof(DownloadOptions.CheckpointPath), "A download into a stream cannot be continued later, so it takes no checkpoint: download to a local file to use one");
            }
            if (options != null && options.PublicationPolicy != DownloadPublicationPolicy.ReplaceExisting)
            {
                throw InvalidParameterException.For(nameof(DownloadOptions.PublicationPolicy), "A download into a stream puts no local file in place, so it takes no PublicationPolicy: download to a local file to use one");
            }
            DownloadOptions settings = (options ?? new DownloadOptions()).Snapshot();
            var context = new OperationContext(client);

            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (OperationContext.OperationScope download = context.Begin(cancellationToken))
                {
                    return await DownloadCoordinator.ToStreamAsync(context, remotePath, destination, settings, download.Token);
                }
            });
        }

        /// <summary>
        /// Uploads the local directory <paramref name="localDirectory"/>, and everything in it, into the remote folder
        /// <paramref name="remoteDirectory"/>.
        /// </summary>
        /// <param name="localDirectory">The local directory to upload.</param>
        /// <param name="remoteDirectory">The remote folder to upload into, created if missing; "" for the site's root.</param>
        /// <param name="options">How many files at once, the shared limiter, a checkpoint directory, progress and each file's settings. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops the upload.</param>
        /// <returns>The completed upload.</returns>
        /// <remarks>
        /// <para>
        /// Every regular file under the directory is uploaded to the same relative path under the remote folder, as
        /// <see cref="UploadFileAsync"/> uploads a file, and every folder with nothing inside to upload is created. The
        /// directory is listed first, and its files are uploaded at most <see cref="DirectoryTransferOptions.MaxConcurrentFiles"/>
        /// at once, in the order of their paths, sharing <see cref="DirectoryTransferOptions.Limiter"/>. Symbolic links
        /// are not followed: they, and anything that is neither a regular file nor a directory, are not uploaded, and
        /// <see cref="DirectoryTransferResult.Excluded"/> lists them. On macOS and Linux, each file is opened through the
        /// directories the upload has already entered, never through a symbolic link, so a program renaming entries
        /// meanwhile cannot make it read a file outside the directory.
        /// </para>
        /// <para>
        /// The first failure stops the files in progress, and no other file starts; the upload then ends with that
        /// failure, unchanged, once every file has stopped. Files completed before then stay completed. With
        /// <see cref="DirectoryTransferOptions.CheckpointDirectory"/>, running the upload again with the same arguments
        /// skips a file it completed only while the file still has the bytes it sent (the SHA-256 of every part, read
        /// again), uploads it again otherwise, and continues each file in progress as <see cref="UploadFileAsync"/>
        /// continues one with a checkpoint. The options are taken when the upload starts: changing them meanwhile changes
        /// nothing.
        /// </para>
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="localDirectory"/> or <paramref name="remoteDirectory"/> is null.</exception>
        /// <exception cref="InvalidParameterException">A setting of <paramref name="options"/> is out of range or not allowed, such as a checkpoint directory inside <paramref name="localDirectory"/>.</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="localDirectory"/> does not exist.</exception>
        /// <exception cref="TransferCheckpointException">The checkpoint directory holds the record of another transfer, or another user could have changed it. Nothing was sent.</exception>
        /// <exception cref="UploadCompletionUnknownException">A file's final request failed after it may have reached Files.com.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public Task<DirectoryTransferResult> UploadDirectoryAsync(string localDirectory, string remoteDirectory, DirectoryUploadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (localDirectory == null)
            {
                throw MissingParameterException.For(nameof(localDirectory));
            }
            if (remoteDirectory == null)
            {
                throw MissingParameterException.For(nameof(remoteDirectory));
            }
            DirectorySettings settings = (options ?? new DirectoryUploadOptions()).Snapshot();
            var context = new OperationContext(client);

            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (OperationContext.OperationScope upload = context.Begin(cancellationToken))
                {
                    return await DirectoryTransfer.UploadAsync(context, localDirectory, remoteDirectory, settings, upload.Token);
                }
            });
        }

        /// <summary>
        /// Downloads the remote folder <paramref name="remoteDirectory"/>, and everything in it, into the local directory
        /// <paramref name="localDirectory"/>.
        /// </summary>
        /// <param name="remoteDirectory">The remote folder to download; "" for the site's root.</param>
        /// <param name="localDirectory">The local directory to download into, created if missing.</param>
        /// <param name="options">How many files at once, the shared limiter, a checkpoint directory, progress and each file's settings. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops the download.</param>
        /// <returns>The completed download.</returns>
        /// <remarks>
        /// <para>
        /// Every file under the folder is downloaded to the same relative path under the local directory, as
        /// <see cref="DownloadFileAsync"/> downloads a file, put in place once it is complete as the
        /// <see cref="DownloadOptions.PublicationPolicy"/> of <see cref="DirectoryDownloadOptions.FileOptions"/> says (by
        /// default replacing a file there), and every empty folder is created, or used as it is when it is there already.
        /// Each file is put in place on its own: the tree is not replaced as a whole. The folder is listed first, a folder at a time, and its files are downloaded at most
        /// <see cref="DirectoryTransferOptions.MaxConcurrentFiles"/> at once, in the order of their paths, sharing
        /// <see cref="DirectoryTransferOptions.Limiter"/>. A remote name that cannot be a local file's name on this
        /// system, such as one with <c>:</c> on Windows, fails the download before anything is written.
        /// </para>
        /// <para>
        /// Local folders are entered, and created, one name at a time without following symbolic links: a symbolic link
        /// where a folder belongs fails the download of the files below it. On macOS and Linux this goes through the
        /// directories already entered, so a program renaming entries meanwhile cannot make the download write outside
        /// the directory. On Windows, whatever the policy, the local directory, every directory above it and every folder
        /// entered are held from before the remote folder is listed until the download ends, so that none of them can be
        /// renamed or replaced meanwhile.
        /// </para>
        /// <para>
        /// The first failure stops the files in progress, and no other file starts; the download then ends with that
        /// failure, unchanged, once every file has stopped. Files completed before then stay in place, and so does
        /// everything else in the local directory: nothing is removed or rolled back. A file the policy refuses, such as
        /// one already there with <see cref="DownloadPublicationPolicy.CreateNew"/>, is such a failure
        /// (<see cref="DownloadPublicationException"/>). With <see cref="DirectoryTransferOptions.CheckpointDirectory"/>,
        /// the record of each completed file is written, with the identity on storage of the file it wrote, before the
        /// file's own checkpoint is removed; running the download again with the same arguments skips a file it completed
        /// only once its storage confirms the version it wrote is still the file's current version, and the local file is
        /// still that same file, of the same length; a file downloaded without a version to name is downloaded again,
        /// and, like any file whose remote version changed or whose local file is not that file, is then put in place as
        /// the policy says. A file an earlier run had put in place but not yet recorded is recognized by its checkpoint,
        /// and then treated as any completed file: recorded, as completed earlier, once its storage confirms its version is
        /// still the file's current one, and otherwise downloaded again as the policy says, which for
        /// <see cref="DownloadPublicationPolicy.CreateNew"/> means it is left as it is and refused. A record an earlier
        /// version of this SDK wrote names completed files by their length and modification time, not their identity:
        /// with the default policy such a file is skipped as before, once its storage confirms its version; with
        /// <see cref="DownloadPublicationPolicy.CreateNew"/> or <see cref="DownloadPublicationPolicy.ReplaceRegularFile"/>,
        /// which put in place only files shown to be this download's, it is not shown to be, so the policy decides, as
        /// for any file already there. It continues each file in progress as <see cref="DownloadFileAsync"/>
        /// continues one with a checkpoint. The options are taken when the download starts: changing them meanwhile
        /// changes nothing.
        /// </para>
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="remoteDirectory"/> or <paramref name="localDirectory"/> is null.</exception>
        /// <exception cref="InvalidParameterException">A setting of <paramref name="options"/> is out of range or not allowed, such as a checkpoint directory inside <paramref name="localDirectory"/>.</exception>
        /// <exception cref="IOException">A remote name cannot be a local name here, or a local folder cannot be entered or created.</exception>
        /// <exception cref="TransferCheckpointException">The checkpoint directory holds the record of another transfer, or another user could have changed it. Nothing was downloaded.</exception>
        /// <exception cref="DownloadSourceChangedException">A file changed while it was downloaded, and could not be downloaded again as it is now.</exception>
        /// <exception cref="DownloadPublicationException">A file was not put in place as the publication policy says, or was put in place without certainty that it will survive a crash or was recorded.</exception>
        /// <exception cref="ApiConnectionException">A connection failed and still failed when retried; a <see cref="NetworkTimeoutException"/> when it stalled.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public Task<DirectoryTransferResult> DownloadDirectoryAsync(string remoteDirectory, string localDirectory, DirectoryDownloadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (remoteDirectory == null)
            {
                throw MissingParameterException.For(nameof(remoteDirectory));
            }
            if (localDirectory == null)
            {
                throw MissingParameterException.For(nameof(localDirectory));
            }
            DirectorySettings settings = (options ?? new DirectoryDownloadOptions()).Snapshot();
            var context = new OperationContext(client);

            return Task.Run(async () =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (OperationContext.OperationScope download = context.Begin(cancellationToken))
                {
                    return await DirectoryTransfer.DownloadAsync(context, remoteDirectory, localDirectory, settings, download.Token);
                }
            });
        }
    }
}