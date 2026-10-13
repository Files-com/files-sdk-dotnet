using FilesCom.Models;
using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    /// <summary>
    /// Remote files and folders used the way local ones are with <see cref="File"/>, <see cref="Directory"/> and
    /// <see cref="Stream"/>, through one client. Get it from <see cref="FilesClient.FileSystem"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It offers exactly these operations: opening a file to read (<see cref="OpenReadAsync"/>) or to write
    /// (<see cref="OpenWriteAsync"/>) as a <see cref="Stream"/>; reading or writing a whole file
    /// (<see cref="ReadAllBytesAsync"/>, <see cref="WriteAllBytesAsync"/>); telling whether a file or folder exists
    /// (<see cref="FileExistsAsync"/>, <see cref="DirectoryExistsAsync"/>); listing a folder, alone or with everything
    /// below it (<see cref="EnumerateAsync"/>); creating a folder with its parents (<see cref="CreateDirectoryAsync"/>);
    /// and deleting, copying or moving a file or folder (<see cref="DeleteAsync"/>, <see cref="CopyAsync"/>,
    /// <see cref="MoveAsync"/>), each returning once Files.com has finished it.
    /// </para>
    /// <para>
    /// Remote files are read and written whole, forward: the streams do not seek, have no length or position, and
    /// cannot append to or change part of a file; those members throw <see cref="NotSupportedException"/>, as they do
    /// for other forward-only streams. Attributes, permissions, locks and times other than the modification time sent
    /// with an upload have no counterpart here; use the client's other operations for what Files.com offers instead.
    /// </para>
    /// </remarks>
    public sealed class RemoteFileSystem
    {
        // The bytes a stream opened to read or write holds between the transfer and its caller.
        private const int PipeCapacity = 1024 * 1024;

        private readonly FilesClient client;

        internal RemoteFileSystem(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Opens the remote file at <paramref name="path"/> to read, as a forward-only stream.
        /// </summary>
        /// <param name="path">The remote file.</param>
        /// <param name="options">Concurrency, part size, limits, progress and request options, as for <see cref="TransferOperations.DownloadStreamAsync"/>. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops opening, and then the download behind the stream.</param>
        /// <returns>The stream, once its first bytes can be read, or the file is known to be empty.</returns>
        /// <remarks>
        /// The file is downloaded as <see cref="TransferOperations.DownloadStreamAsync"/> downloads it, behind the stream:
        /// bound to the version there when the file is opened, when Files.com allows that, with every part checked, and
        /// read ahead of the caller by at most the parts in progress and 1 MiB. A failure reaches the caller as it
        /// reads, once it has read everything before it; a failure before the first byte is thrown here. Disposing the
        /// stream before its end stops the download, and returns once it has stopped.
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="path"/> is null.</exception>
        /// <exception cref="InvalidParameterException"><see cref="DownloadOptions.CheckpointPath"/> or a <see cref="DownloadOptions.PublicationPolicy"/> other than the default is set, a setting of <paramref name="options"/> is out of range, or settings of <paramref name="options"/> contradict each other.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public async Task<Stream> OpenReadAsync(string path, DownloadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (path == null)
            {
                throw MissingParameterException.For(nameof(path));
            }
            if (options?.CheckpointPath != null)
            {
                throw InvalidParameterException.For(nameof(DownloadOptions.CheckpointPath), "A remote file opened to read cannot be continued later, so it takes no checkpoint");
            }
            if (options != null && options.PublicationPolicy != DownloadPublicationPolicy.ReplaceExisting)
            {
                throw InvalidParameterException.For(nameof(DownloadOptions.PublicationPolicy), "A download into a stream puts no local file in place, so it takes no PublicationPolicy: download to a local file to use one");
            }
            DownloadOptions settings = (options ?? new DownloadOptions()).Snapshot();
            var context = new OperationContext(client);
            var pipe = new TransferPipe(PipeCapacity);
            var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task download = Task.Run(async () =>
            {
                try
                {
                    using (OperationContext.OperationScope scope = context.Begin(stop.Token))
                    {
                        await DownloadCoordinator.ToStreamAsync(context, path, new PipeEndStream(pipe, reads: false), settings, scope.Token);
                    }
                    pipe.Complete();
                }
                catch (Exception e)
                {
                    pipe.Complete(e);
                }
            });
            var stream = new RemoteReadStream(pipe, download, stop);
            try
            {
                await pipe.WaitUntilReadableAsync(cancellationToken);
                pipe.ThrowIfFailedEmpty();
            }
            catch
            {
                stream.Dispose();
                throw;
            }
            return stream;
        }

        /// <summary>
        /// Opens the remote file at <paramref name="path"/> to write, as a forward-only stream: what is written becomes
        /// the file once the stream is disposed.
        /// </summary>
        /// <param name="path">The remote file, created or replaced.</param>
        /// <param name="options">Concurrency, sizing, limits, progress and request options, as for <see cref="TransferOperations.UploadStreamAsync(Stream, string, UploadOptions, CancellationToken)"/>. If null, the defaults.</param>
        /// <param name="cancellationToken">
        /// Abandons the upload: nothing more is sent, the file is not completed, and disposing the stream then returns
        /// without an error.
        /// </param>
        /// <returns>The stream.</returns>
        /// <remarks>
        /// <para>
        /// What is written is uploaded as a stream of unknown length (see
        /// <see cref="TransferOperations.UploadStreamAsync(Stream, string, UploadOptions, CancellationToken)"/>), while it is
        /// written: a write waits while the upload is 1 MiB behind. Disposing the stream ends the file, waits until the
        /// upload is completed, and throws its failure, if it failed, such as
        /// <see cref="UploadCompletionUnknownException"/>; a write after the upload failed throws that failure.
        /// </para>
        /// <para>
        /// Disposing completes the file with what was written, including when the code writing it failed: to leave the
        /// file uncompleted, cancel <paramref name="cancellationToken"/> before disposing.
        /// </para>
        /// </remarks>
        /// <exception cref="MissingParameterException"><paramref name="path"/> is null.</exception>
        /// <exception cref="InvalidParameterException"><see cref="UploadOptions.CheckpointPath"/> is set, a setting of <paramref name="options"/> is out of range, or settings of <paramref name="options"/> contradict each other.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public Task<Stream> OpenWriteAsync(string path, UploadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (path == null)
            {
                throw MissingParameterException.For(nameof(path));
            }
            if (options?.CheckpointPath != null)
            {
                throw InvalidParameterException.For(nameof(UploadOptions.CheckpointPath), "A remote file opened to write cannot be continued later, so it takes no checkpoint");
            }
            UploadOptions settings = (options ?? new UploadOptions()).Snapshot();
            var context = new OperationContext(client);
            var pipe = new TransferPipe(PipeCapacity);
            // Admitted to the client before this returns. The client's logging is made current in the upload's task,
            // not here, where the caller's own code goes on.
            OperationContext.OperationScope scope = context.BeginElsewhere(cancellationToken);
            Task upload = Task.Run(async () =>
            {
                try
                {
                    using (SdkLog.Use(context.Logging))
                    {
                        var reader = new PipeEndStream(pipe, reads: true);
                        using (UploadSource source = settings.SpoolMaxBytes.HasValue
                            ? new SpooledStreamSource(reader, Math.Min(settings.SpoolMaxBytes.Value, settings.MaxStreamLength ?? long.MaxValue), settings.TemporaryDirectory)
                            : (UploadSource)new StreamUploadSource(reader, null, settings.StreamPartStorage, settings.TemporaryDirectory))
                        {
                            await UploadCoordinator.RunAsync(context, path, source, settings, scope.Token);
                        }
                    }
                }
                finally
                {
                    // Nothing more is read: a write now fails with the upload's failure.
                    pipe.Leave();
                    scope.Dispose();
                }
            });
            return Task.FromResult<Stream>(new RemoteWriteStream(pipe, upload, cancellationToken));
        }

        /// <summary>
        /// Reads the whole remote file at <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The remote file.</param>
        /// <param name="options">Download settings, as for <see cref="TransferOperations.DownloadStreamAsync"/>. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops the download.</param>
        /// <returns>The file's bytes.</returns>
        /// <exception cref="MissingParameterException"><paramref name="path"/> is null.</exception>
        public async Task<byte[]> ReadAllBytesAsync(string path, DownloadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (path == null)
            {
                throw MissingParameterException.For(nameof(path));
            }
            var bytes = new MemoryStream();
            await client.Transfers.DownloadStreamAsync(path, bytes, options, cancellationToken);
            return bytes.ToArray();
        }

        /// <summary>
        /// Writes <paramref name="bytes"/> as the remote file at <paramref name="path"/>, creating or replacing it.
        /// </summary>
        /// <param name="path">The remote file.</param>
        /// <param name="bytes">The file's bytes.</param>
        /// <param name="options">Upload settings, as for <see cref="TransferOperations.UploadStreamAsync(Stream, long, string, UploadOptions, CancellationToken)"/>. If null, the defaults.</param>
        /// <param name="cancellationToken">Stops the upload.</param>
        /// <exception cref="MissingParameterException"><paramref name="path"/> or <paramref name="bytes"/> is null.</exception>
        public async Task WriteAllBytesAsync(string path, byte[] bytes, UploadOptions options = null, CancellationToken cancellationToken = default)
        {
            if (path == null)
            {
                throw MissingParameterException.For(nameof(path));
            }
            if (bytes == null)
            {
                throw MissingParameterException.For(nameof(bytes));
            }
            await client.Transfers.UploadStreamAsync(new MemoryStream(bytes, writable: false), bytes.Length, path, options, cancellationToken);
        }

        /// <summary>Whether a file is at <paramref name="path"/>: false for a folder, or nothing.</summary>
        /// <param name="path">The remote path.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task<bool> FileExistsAsync(string path, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            return await RunAsync(options, cancellationToken, (context, requestOptions, token) => TypeAtAsync(context, path, requestOptions, token)) == "file";
        }

        /// <summary>Whether a folder is at <paramref name="path"/>: false for a file, or nothing.</summary>
        /// <param name="path">The remote path.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task<bool> DirectoryExistsAsync(string path, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            return await RunAsync(options, cancellationToken, (context, requestOptions, token) => TypeAtAsync(context, path, requestOptions, token)) == "directory";
        }

        /// <summary>
        /// Lists the files and folders in the folder at <paramref name="path"/>, and with <paramref name="recursive"/>,
        /// everything below it.
        /// </summary>
        /// <param name="path">The remote folder; "" for the site's root.</param>
        /// <param name="recursive">Whether to list the folders inside too, and theirs.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops listing.</param>
        /// <returns>
        /// The entries, a folder's in the order Files.com lists them, then those of the folders it holds, in that order.
        /// Each folder is listed, a page at a time, only when the enumeration reaches it.
        /// </returns>
        public async IAsyncEnumerable<RemoteFile> EnumerateAsync(string path, bool recursive = false, Dictionary<string, object> options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (path == null)
            {
                throw MissingParameterException.For(nameof(path));
            }
            // One operation: every folder and page is listed with the client settings, origin and request options it
            // started with.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            var context = new OperationContext(client);
            var folders = new Queue<string>();
            folders.Enqueue(path.Trim('/'));
            while (folders.Count > 0)
            {
                FilesList<RemoteFile> list = Folder.ListForCore(client, folders.Dequeue(), new Dictionary<string, object>(), null);
                await foreach (RemoteFile entry in list.ListWithinAsync(context, requestOptions, cancellationToken).ConfigureAwait(false))
                {
                    yield return entry;
                    if (recursive && entry.Type == "directory")
                    {
                        folders.Enqueue(entry.Path);
                    }
                }
            }
        }

        /// <summary>
        /// Creates the folder at <paramref name="path"/>, and any missing folders above it; nothing happens when it exists.
        /// </summary>
        /// <param name="path">The remote folder.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <exception cref="IOException">A file is at <paramref name="path"/>.</exception>
        public Task CreateDirectoryAsync(string path, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            return RunAsync(options, cancellationToken, async (context, requestOptions, token) =>
            {
                try
                {
                    // Sent again after a lost answer: a folder created by the first request is only there already.
                    await Folder.CreateCore(context.ForReplaySafeRequests(), path, new Dictionary<string, object> { { "mkdir_parents", true } }, DictionaryUtil.Copy(requestOptions), token);
                }
                catch (DestinationExistsException)
                {
                    if (await TypeAtAsync(context, path, requestOptions, token) != "directory")
                    {
                        throw new IOException($"{path} cannot be created as a folder: a file is there.");
                    }
                }
                return true;
            });
        }

        /// <summary>
        /// Deletes the file or folder at <paramref name="path"/>, and returns once Files.com has nothing there.
        /// </summary>
        /// <param name="path">The remote file or folder.</param>
        /// <param name="recursive">Whether to delete a folder that is not empty, with everything in it.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops waiting; a deletion Files.com has begun goes on.</param>
        /// <remarks>Files.com may finish deleting a folder with its contents after it answers; this waits until it has (<see cref="Operations.RemoteFileOperations.WaitUntilGoneAsync"/>).</remarks>
        public Task DeleteAsync(string path, bool recursive = false, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            return RunAsync(options, cancellationToken, async (context, requestOptions, token) =>
            {
                var parameters = new Dictionary<string, object>();
                if (recursive)
                {
                    parameters["recursive"] = true;
                }
                await RemoteFile.DeleteCore(context, path, parameters, DictionaryUtil.Copy(requestOptions), token);
                await RemoteFile.WaitUntilGoneCore(context, path, FileAction.CheckedPollInterval(null), requestOptions, token);
                return true;
            });
        }

        /// <summary>
        /// Copies the file or folder at <paramref name="source"/> to <paramref name="destination"/>, and returns once
        /// Files.com has finished.
        /// </summary>
        /// <param name="source">The remote file or folder to copy.</param>
        /// <param name="destination">The remote path of the copy.</param>
        /// <param name="overwrite">Whether to replace what is at <paramref name="destination"/>.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops waiting; a copy Files.com has begun goes on.</param>
        /// <exception cref="FileMigrationFailedException">The copy failed or was cancelled.</exception>
        public Task CopyAsync(string source, string destination, bool overwrite = false, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            return RunAsync(options, cancellationToken, async (context, requestOptions, token) =>
            {
                FileAction action = await RemoteFile.CopyCore(context, source, Destination(destination, overwrite), DictionaryUtil.Copy(requestOptions), token);
                return await action.WaitCoreAsync(context, FileAction.CheckedPollInterval(null), null, requestOptions, token);
            });
        }

        /// <summary>
        /// Moves the file or folder at <paramref name="source"/> to <paramref name="destination"/>, and returns once
        /// Files.com has finished.
        /// </summary>
        /// <param name="source">The remote file or folder to move.</param>
        /// <param name="destination">Its new remote path.</param>
        /// <param name="overwrite">Whether to replace what is at <paramref name="destination"/>.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops waiting; a move Files.com has begun goes on.</param>
        /// <exception cref="FileMigrationFailedException">The move failed or was cancelled.</exception>
        public Task MoveAsync(string source, string destination, bool overwrite = false, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            return RunAsync(options, cancellationToken, async (context, requestOptions, token) =>
            {
                FileAction action = await RemoteFile.MoveCore(context, source, Destination(destination, overwrite), DictionaryUtil.Copy(requestOptions), token);
                return await action.WaitCoreAsync(context, FileAction.CheckedPollInterval(null), null, requestOptions, token);
            });
        }

        private static Dictionary<string, object> Destination(string destination, bool overwrite)
        {
            var parameters = new Dictionary<string, object> { { "destination", destination } };
            if (overwrite)
            {
                parameters["overwrite"] = true;
            }
            return parameters;
        }

        // Runs one compound operation: every request of it has this client's settings as they are now, the origin its
        // first request reaches, and a snapshot of the caller's request options, and it holds the client open until done.
        private async Task<T> RunAsync<T>(Dictionary<string, object> options, CancellationToken cancellationToken, Func<OperationContext, Dictionary<string, object>, CancellationToken, Task<T>> operation)
        {
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            var context = new OperationContext(client);
            using (OperationContext.OperationScope scope = context.Begin(cancellationToken))
            {
                return await operation(context, requestOptions, scope.Token);
            }
        }

        // The type of what is at path, "file" or "directory", or null for nothing.
        private static async Task<string> TypeAtAsync(OperationContext context, string path, Dictionary<string, object> requestOptions, CancellationToken cancellationToken)
        {
            try
            {
                RemoteFile found = await RemoteFile.FindCore(context, path, null, DictionaryUtil.Copy(requestOptions), cancellationToken);
                return found.Type;
            }
            catch (NotFoundException)
            {
                return null;
            }
        }
    }
}