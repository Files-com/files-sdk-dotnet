using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// FileEntity model
    /// </summary>
    public class RemoteFile : IModel, IJsonOnDeserialized
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        // The names of the attributes set on this object, by its constructor or its properties, since it was made
        // or last came from Files.com. SaveAsync sends these, never the defaults of attributes nobody set.
        private readonly HashSet<string> unsaved = new HashSet<string>();

        private static readonly SdkLog log = SdkLog.For(typeof(RemoteFile));

        private static readonly HashSet<string> WindowsDeviceNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CON", "PRN", "AUX", "NUL", "CONIN$", "CONOUT$",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM\u00b9", "COM\u00b2", "COM\u00b3",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9", "LPT\u00b9", "LPT\u00b2", "LPT\u00b3",
        };

        /// <summary>
        /// Same as <see cref="Create(string, Dictionary{string, object}, Dictionary{string, object})"/>, for the path
        /// in <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">The operation's parameters, keyed by their API names, including path.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static async Task<RemoteFile> Create(Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            return (RemoteFile)await RemoteFile.Create((string)parameters["path"], parameters, options);
        }

        /// <summary>
        /// Downloads the file at <paramref name="path"/> to <paramref name="localPath"/>, creating or replacing it, with
        /// the default client (<see cref="FilesClient.Instance"/>).
        /// </summary>
        /// <param name="path">The remote path of the file.</param>
        /// <param name="localPath">The local file to write. If null, a file with the remote file's name in the current directory.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteFile> DownloadFile(string path, string localPath = null, Dictionary<string, object> options = null)
        {
            return DownloadFileCore(FilesClient.Instance, path, localPath, options, CancellationToken.None);
        }

        /// <summary>
        /// Downloads the file at <paramref name="path"/> into <paramref name="stream"/>, from the stream's current
        /// position, with the default client (<see cref="FilesClient.Instance"/>).
        /// </summary>
        /// <param name="path">The remote path of the file.</param>
        /// <param name="stream">The stream to write to. It is borrowed: it is left open, including when the download fails or is cancelled.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteFile> DownloadFile(string path, System.IO.Stream stream, Dictionary<string, object> options = null)
        {
            return DownloadFileCore(FilesClient.Instance, path, stream, options, CancellationToken.None);
        }

        internal static async Task<RemoteFile> DownloadFileCore(FilesClient client, string path, string localPath, Dictionary<string, object> options, CancellationToken cancellationToken)
        {
            localPath = localPath != null ? localPath : DefaultLocalPath(path);
            RemoteFile f = ForPath(client, path, options);
            await f.DownloadFileAsync(localPath, cancellationToken);
            return f;
        }

        // The remote file's name in the working directory. A name that is not a valid local file name is refused: on
        // Windows a colon would make it a drive-relative path or an NTFS stream, which can write outside the working
        // directory or onto another file, and a device name such as NUL would write to the device instead of a file.
        private static string DefaultLocalPath(string path)
        {
            string normalizedPath = PathUtil.normalize(path);
            string name = normalizedPath.Substring(normalizedPath.LastIndexOf('/') + 1);
            if (name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0 || IsWindowsDeviceName(name))
            {
                throw InvalidParameterException.For("localPath", $"required because \"{name}\" is not a valid local file name");
            }
            return System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + name;
        }

        // Whether Windows opens name as a device rather than a file. A bare device name is always a device. Anything
        // after it, such as the extension in NUL.txt, makes it a device on some Windows versions and a file on others,
        // so Windows decides those names: it resolves a device to a \\.\ path.
        private static bool IsWindowsDeviceName(string name)
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return false;
            }
            string baseName = name.Split('.')[0].TrimEnd(' ');
            if (!WindowsDeviceNames.Contains(baseName))
            {
                return false;
            }
            return baseName == name || System.IO.Path.GetFullPath(name).StartsWith(@"\\.\", StringComparison.Ordinal);
        }

        internal static async Task<RemoteFile> DownloadFileCore(FilesClient client, string path, System.IO.Stream stream, Dictionary<string, object> options, CancellationToken cancellationToken)
        {
            RemoteFile f = ForPath(client, path, options);
            await f.DownloadFileAsync(stream, cancellationToken);
            return f;
        }

        // A RemoteFile for path that belongs to client, or, when client is null, to the default client from its first request.
        private static RemoteFile ForPath(FilesClient client, string path, Dictionary<string, object> options)
        {
            RemoteFile f = new RemoteFile(null, null);
            ((IModel)f).SetContext(client, options, null);
            f.Path = path;
            return f;
        }

        internal static string UnderscoreDestinationPath(string root, long id, string relativePath = null)
        {
            return PathUtil.normalize("_", root, id.ToString(), relativePath ?? "");
        }

        internal static string UnderscoreUploadDestinationPath(string root, long id, string localPath, string destinationPath = null)
        {
            return UnderscoreDestinationPath(root, id, destinationPath ?? System.IO.Path.GetFileName(localPath));
        }

        private static async Task<Tuple<Int64, string>> UploadChunk(OperationContext context, string path, System.IO.Stream readStream, string fileRef, Int64 partNumber, Int64 offset, Int64 fileLength, Dictionary<string, object> options, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }
            if (fileRef != null)
            {
                parameters["ref"] = fileRef;
            }
            parameters["part"] = partNumber;

            // Negotiations are sent again after a lost answer: an upload never heard of is never completed.
            FileUploadPart[] uploadActions = await BeginUploadCore(context.ForReplaySafeRequests(), path, parameters, options, cancellationToken);
            FileUploadPart uploadAction = uploadActions[0];
            Int64 chunkLength = Math.Min(fileLength - offset, (Int64)uploadAction.Partsize);
            System.Net.Http.HttpMethod httpMethod = new System.Net.Http.HttpMethod(uploadAction.HttpMethod);

            // A URL that is about to expire before the part is sent is replaced by negotiating the same part of the same
            // upload again. The part has been read by then, and is sent unchanged.
            Dictionary<string, object> renewal = new Dictionary<string, object>(parameters);
            renewal["ref"] = uploadAction.Ref ?? fileRef;
            Func<CancellationToken, Task<FilesCom.Http.SignedUrl>> renewUrl = async token =>
            {
                FileUploadPart renewed = (await BeginUploadCore(context.ForReplaySafeRequests(), path, renewal, options, token))[0];
                if (Math.Min(fileLength - offset, renewed.Partsize ?? 0) != chunkLength)
                {
                    throw new InvalidResponseException($"The server changed the size of part {partNumber} from {chunkLength} to {renewed.Partsize} bytes when renewing its URL.");
                }
                return PartUrl(renewed);
            };
            await FilesClient.ChunkUpload(context, httpMethod, PartUrl(uploadAction), readStream, chunkLength, renewUrl, cancellationToken);

            return Tuple.Create(chunkLength, uploadAction.Ref);
        }

        // A part's URL, which expires when Files.com said it does or its signature says it does, whichever is sooner.
        private static FilesCom.Http.SignedUrl PartUrl(FileUploadPart part)
        {
            DateTimeOffset expires;
            bool negotiated = DateTimeOffset.TryParse(part.Expires, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out expires);
            return FilesCom.Http.SignedUrl.Of(part.UploadUri, negotiated ? expires : (DateTimeOffset?)null);
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to <paramref name="destinationPath"/>, with the default
        /// client (<see cref="FilesClient.Instance"/>).
        /// </summary>
        /// <param name="localPath">The local file to upload. It is opened, and closed when the task ends.</param>
        /// <param name="destinationPath">The remote path to upload to. If null, the local file's name.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        /// <exception cref="MissingParameterException"><paramref name="localPath"/> is null or empty.</exception>
        public static Task<bool> UploadFile(string localPath, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return UploadFileCore(OperationContext.OfDefaultClient(), localPath, destinationPath, options, parameters, CancellationToken.None);
        }

        internal static async Task<bool> UploadFileCore(OperationContext context, string localPath, string destinationPath, Dictionary<string, object> options, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            // Checked for every entry point that takes a local path, before anything is opened or sent.
            if (string.IsNullOrEmpty(localPath))
            {
                throw MissingParameterException.For(nameof(localPath));
            }
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(localPath);

            // The local file's name by this platform's rules: on Windows after its last \ or /, elsewhere after its last /.
            if (destinationPath == null)
            {
                destinationPath = System.IO.Path.GetFileName(localPath);
            }

            DateTime mTime = fileInfo.LastWriteTimeUtc;
            Int64 fileLength = fileInfo.Length;

            System.IO.Stream readStream = System.IO.File.OpenRead(localPath);
            return await UploadFileCore(context, destinationPath, readStream, fileLength, mTime, options, parameters, cancellationToken);
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to the Remote Server with ID <paramref name="remoteServerId"/>,
        /// as <see cref="UploadFile(string, string, Dictionary{string, object}, Dictionary{string, object})"/> does.
        /// </summary>
        /// <param name="localPath">The local file to upload. It is opened, and closed when the task ends.</param>
        /// <param name="remoteServerId">The ID of the Remote Server.</param>
        /// <param name="destinationPath">The path to upload to, within the Remote Server. If null, the local file's name.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        public static async Task<bool> UploadToRemoteServer(string localPath, long remoteServerId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(localPath, UnderscoreUploadDestinationPath("RemoteServers", remoteServerId, localPath, destinationPath), options, parameters);
        }

        /// <summary>
        /// Uploads <paramref name="fileLength"/> bytes from <paramref name="readStream"/> to the Remote Server with ID <paramref name="remoteServerId"/>,
        /// as <see cref="UploadFile(string, System.IO.Stream, long, DateTime, Dictionary{string, object}, Dictionary{string, object})"/> does.
        /// </summary>
        /// <param name="remoteServerId">The ID of the Remote Server.</param>
        /// <param name="destinationPath">The path to upload to, within the Remote Server.</param>
        /// <param name="readStream">
        /// The stream to read from its current position. The upload takes ownership of it and disposes it when the task
        /// ends, whether the upload succeeds, fails or is cancelled.
        /// </param>
        /// <param name="fileLength">The number of bytes to upload. If the stream ends sooner, the upload fails with <see cref="System.IO.EndOfStreamException"/> and is not completed.</param>
        /// <param name="mTime">The file's modification time.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        public static async Task<bool> UploadToRemoteServer(long remoteServerId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath), readStream, fileLength, mTime, options, parameters);
        }

        /// <summary>
        /// Copies the file or folder at <paramref name="path"/> to the Remote Server with ID <paramref name="remoteServerId"/>.
        /// </summary>
        /// <param name="path">The remote path of the file or folder.</param>
        /// <param name="remoteServerId">The ID of the Remote Server.</param>
        /// <param name="destinationPath">The path to copy to, within the Remote Server.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static async Task<FileAction> CopyToRemoteServer(string path, long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return await Copy(path, parameters, options);
        }

        /// <summary>
        /// Moves the file or folder at <paramref name="path"/> to the Remote Server with ID <paramref name="remoteServerId"/>.
        /// </summary>
        /// <param name="path">The remote path of the file or folder.</param>
        /// <param name="remoteServerId">The ID of the Remote Server.</param>
        /// <param name="destinationPath">The path to move to, within the Remote Server.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static async Task<FileAction> MoveToRemoteServer(string path, long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return await Move(path, parameters, options);
        }

        /// <summary>
        /// Same as <see cref="CopyToRemoteServerAsync"/>, without a cancellation token.
        /// </summary>
        /// <param name="remoteServerId">The ID of the Remote Server.</param>
        /// <param name="destinationPath">The path to copy to, within the Remote Server.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        public Task<FileAction> CopyToRemoteServer(long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return CopyToRemoteServerAsync(remoteServerId, destinationPath, parameters, CancellationToken.None);
        }

        /// <summary>
        /// Copies this file or folder to the Remote Server with ID <paramref name="remoteServerId"/>.
        /// </summary>
        /// <param name="remoteServerId">The ID of the Remote Server.</param>
        /// <param name="destinationPath">The path to copy to, within the Remote Server.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> CopyToRemoteServerAsync(long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return CopyAsync(parameters, cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="MoveToRemoteServerAsync"/>, without a cancellation token.
        /// </summary>
        /// <param name="remoteServerId">The ID of the Remote Server.</param>
        /// <param name="destinationPath">The path to move to, within the Remote Server.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        public Task<FileAction> MoveToRemoteServer(long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return MoveToRemoteServerAsync(remoteServerId, destinationPath, parameters, CancellationToken.None);
        }

        /// <summary>
        /// Moves this file or folder to the Remote Server with ID <paramref name="remoteServerId"/>.
        /// </summary>
        /// <param name="remoteServerId">The ID of the Remote Server.</param>
        /// <param name="destinationPath">The path to move to, within the Remote Server.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> MoveToRemoteServerAsync(long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return MoveAsync(parameters, cancellationToken);
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to the Snapshot with ID <paramref name="snapshotId"/>,
        /// as <see cref="UploadFile(string, string, Dictionary{string, object}, Dictionary{string, object})"/> does.
        /// </summary>
        /// <param name="localPath">The local file to upload. It is opened, and closed when the task ends.</param>
        /// <param name="snapshotId">The ID of the Snapshot.</param>
        /// <param name="destinationPath">The path to upload to, within the Snapshot. If null, the local file's name.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        public static async Task<bool> UploadToSnapshot(string localPath, long snapshotId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(localPath, UnderscoreUploadDestinationPath("Snapshots", snapshotId, localPath, destinationPath), options, parameters);
        }

        /// <summary>
        /// Uploads <paramref name="fileLength"/> bytes from <paramref name="readStream"/> to the Snapshot with ID <paramref name="snapshotId"/>,
        /// as <see cref="UploadFile(string, System.IO.Stream, long, DateTime, Dictionary{string, object}, Dictionary{string, object})"/> does.
        /// </summary>
        /// <param name="snapshotId">The ID of the Snapshot.</param>
        /// <param name="destinationPath">The path to upload to, within the Snapshot.</param>
        /// <param name="readStream">
        /// The stream to read from its current position. The upload takes ownership of it and disposes it when the task
        /// ends, whether the upload succeeds, fails or is cancelled.
        /// </param>
        /// <param name="fileLength">The number of bytes to upload. If the stream ends sooner, the upload fails with <see cref="System.IO.EndOfStreamException"/> and is not completed.</param>
        /// <param name="mTime">The file's modification time.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        public static async Task<bool> UploadToSnapshot(long snapshotId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath), readStream, fileLength, mTime, options, parameters);
        }

        /// <summary>
        /// Copies the file or folder at <paramref name="path"/> to the Snapshot with ID <paramref name="snapshotId"/>.
        /// </summary>
        /// <param name="path">The remote path of the file or folder.</param>
        /// <param name="snapshotId">The ID of the Snapshot.</param>
        /// <param name="destinationPath">The path to copy to, within the Snapshot.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static async Task<FileAction> CopyToSnapshot(string path, long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return await Copy(path, parameters, options);
        }

        /// <summary>
        /// Moves the file or folder at <paramref name="path"/> to the Snapshot with ID <paramref name="snapshotId"/>.
        /// </summary>
        /// <param name="path">The remote path of the file or folder.</param>
        /// <param name="snapshotId">The ID of the Snapshot.</param>
        /// <param name="destinationPath">The path to move to, within the Snapshot.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static async Task<FileAction> MoveToSnapshot(string path, long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return await Move(path, parameters, options);
        }

        /// <summary>
        /// Same as <see cref="CopyToSnapshotAsync"/>, without a cancellation token.
        /// </summary>
        /// <param name="snapshotId">The ID of the Snapshot.</param>
        /// <param name="destinationPath">The path to copy to, within the Snapshot.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        public Task<FileAction> CopyToSnapshot(long snapshotId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return CopyToSnapshotAsync(snapshotId, destinationPath, parameters, CancellationToken.None);
        }

        /// <summary>
        /// Copies this file or folder to the Snapshot with ID <paramref name="snapshotId"/>.
        /// </summary>
        /// <param name="snapshotId">The ID of the Snapshot.</param>
        /// <param name="destinationPath">The path to copy to, within the Snapshot.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> CopyToSnapshotAsync(long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return CopyAsync(parameters, cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="MoveToSnapshotAsync"/>, without a cancellation token.
        /// </summary>
        /// <param name="snapshotId">The ID of the Snapshot.</param>
        /// <param name="destinationPath">The path to move to, within the Snapshot.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        public Task<FileAction> MoveToSnapshot(long snapshotId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return MoveToSnapshotAsync(snapshotId, destinationPath, parameters, CancellationToken.None);
        }

        /// <summary>
        /// Moves this file or folder to the Snapshot with ID <paramref name="snapshotId"/>.
        /// </summary>
        /// <param name="snapshotId">The ID of the Snapshot.</param>
        /// <param name="destinationPath">The path to move to, within the Snapshot.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> MoveToSnapshotAsync(long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return MoveAsync(parameters, cancellationToken);
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to the Child Site with ID <paramref name="siteId"/>,
        /// as <see cref="UploadFile(string, string, Dictionary{string, object}, Dictionary{string, object})"/> does.
        /// </summary>
        /// <param name="localPath">The local file to upload. It is opened, and closed when the task ends.</param>
        /// <param name="siteId">The ID of the Child Site.</param>
        /// <param name="destinationPath">The path to upload to, within the Child Site. If null, the local file's name.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        public static async Task<bool> UploadToChildSite(string localPath, long siteId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(localPath, UnderscoreUploadDestinationPath("Sites", siteId, localPath, destinationPath), options, parameters);
        }

        /// <summary>
        /// Uploads <paramref name="fileLength"/> bytes from <paramref name="readStream"/> to the Child Site with ID <paramref name="siteId"/>,
        /// as <see cref="UploadFile(string, System.IO.Stream, long, DateTime, Dictionary{string, object}, Dictionary{string, object})"/> does.
        /// </summary>
        /// <param name="siteId">The ID of the Child Site.</param>
        /// <param name="destinationPath">The path to upload to, within the Child Site.</param>
        /// <param name="readStream">
        /// The stream to read from its current position. The upload takes ownership of it and disposes it when the task
        /// ends, whether the upload succeeds, fails or is cancelled.
        /// </param>
        /// <param name="fileLength">The number of bytes to upload. If the stream ends sooner, the upload fails with <see cref="System.IO.EndOfStreamException"/> and is not completed.</param>
        /// <param name="mTime">The file's modification time.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        public static async Task<bool> UploadToChildSite(long siteId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(UnderscoreDestinationPath("Sites", siteId, destinationPath), readStream, fileLength, mTime, options, parameters);
        }

        /// <summary>
        /// Copies the file or folder at <paramref name="path"/> to the Child Site with ID <paramref name="siteId"/>.
        /// </summary>
        /// <param name="path">The remote path of the file or folder.</param>
        /// <param name="siteId">The ID of the Child Site.</param>
        /// <param name="destinationPath">The path to copy to, within the Child Site.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static async Task<FileAction> CopyToChildSite(string path, long siteId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return await Copy(path, parameters, options);
        }

        /// <summary>
        /// Moves the file or folder at <paramref name="path"/> to the Child Site with ID <paramref name="siteId"/>.
        /// </summary>
        /// <param name="path">The remote path of the file or folder.</param>
        /// <param name="siteId">The ID of the Child Site.</param>
        /// <param name="destinationPath">The path to move to, within the Child Site.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static async Task<FileAction> MoveToChildSite(string path, long siteId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return await Move(path, parameters, options);
        }

        /// <summary>
        /// Same as <see cref="CopyToChildSiteAsync"/>, without a cancellation token.
        /// </summary>
        /// <param name="siteId">The ID of the Child Site.</param>
        /// <param name="destinationPath">The path to copy to, within the Child Site.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        public Task<FileAction> CopyToChildSite(long siteId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return CopyToChildSiteAsync(siteId, destinationPath, parameters, CancellationToken.None);
        }

        /// <summary>
        /// Copies this file or folder to the Child Site with ID <paramref name="siteId"/>.
        /// </summary>
        /// <param name="siteId">The ID of the Child Site.</param>
        /// <param name="destinationPath">The path to copy to, within the Child Site.</param>
        /// <param name="parameters">Copy parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> CopyToChildSiteAsync(long siteId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return CopyAsync(parameters, cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="MoveToChildSiteAsync"/>, without a cancellation token.
        /// </summary>
        /// <param name="siteId">The ID of the Child Site.</param>
        /// <param name="destinationPath">The path to move to, within the Child Site.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        public Task<FileAction> MoveToChildSite(long siteId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return MoveToChildSiteAsync(siteId, destinationPath, parameters, CancellationToken.None);
        }

        /// <summary>
        /// Moves this file or folder to the Child Site with ID <paramref name="siteId"/>.
        /// </summary>
        /// <param name="siteId">The ID of the Child Site.</param>
        /// <param name="destinationPath">The path to move to, within the Child Site.</param>
        /// <param name="parameters">Move parameters, such as overwrite. The destination is the one <paramref name="destinationPath"/> gives.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> MoveToChildSiteAsync(long siteId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return MoveAsync(parameters, cancellationToken);
        }


        /// <summary>
        /// Uploads <paramref name="fileLength"/> bytes from <paramref name="readStream"/> to <paramref name="destinationPath"/>,
        /// with the default client (<see cref="FilesClient.Instance"/>).
        /// </summary>
        /// <param name="destinationPath">The remote path to upload to.</param>
        /// <param name="readStream">
        /// The stream to read from its current position. The upload takes ownership of it and disposes it when the task
        /// ends, whether the upload succeeds, fails or is cancelled.
        /// </param>
        /// <param name="fileLength">The number of bytes to upload. If the stream ends sooner, the upload fails with <see cref="System.IO.EndOfStreamException"/> and is not completed.</param>
        /// <param name="mTime">The file's modification time.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        public static Task<bool> UploadFile(string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return UploadFileCore(OperationContext.OfDefaultClient(), destinationPath, readStream, fileLength, mTime, options, parameters, CancellationToken.None);
        }

        // Uploads the stream part by part, then finalizes the upload. Every stage runs with context and one copy of
        // options, and holds the client open until the upload ends. The upload owns readStream and disposes it when it
        // ends, however it ends: within the time it holds the client open, so that closing the client waits for that
        // disposal too; or at once, if the upload never starts (there is no client, or it has been disposed).
        internal static async Task<bool> UploadFileCore(OperationContext context, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            OperationContext.OperationScope upload;
            try
            {
                if (context == null)
                {
                    // Its first request, begin_upload, sends the credentials, which only its options could carry.
                    throw FilesClient.NoDefaultClient(options, sendsCredentials: true);
                }
                upload = context.Begin(cancellationToken);
            }
            catch
            {
                readStream?.Dispose();
                throw;
            }
            using (upload)
            {
                using (readStream)
                {
                    await UploadPartsAndFinish(context, destinationPath, readStream, fileLength, mTime, DictionaryUtil.Copy(options), parameters, upload.Token);
                }
            }
            return true;
        }

        private static async Task UploadPartsAndFinish(OperationContext context, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            Int64 parts = 0;
            Int64 bytesWritten = 0;
            string fileRef = null;

            // TODO: Set up multiple parallel streams instead of looping serial uploads here.
            // Each request checks the token before it is sent, so cancellation starts no further part or the final request.
            while (bytesWritten < fileLength || parts == 0)
            {
                parts++;
                Tuple<Int64, string> result = await UploadChunk(context, destinationPath, readStream, fileRef, parts, bytesWritten, fileLength, options, parameters, cancellationToken);
                bytesWritten += result.Item1;
                fileRef = result.Item2;
            }

            Dictionary<string, object> createParams = new Dictionary<string, object>();
            createParams["action"] = "end";
            createParams["provided_mtime"] = mTime.ToString("u");
            createParams["ref"] = fileRef;
            createParams["size"] = fileLength;

            // Sent without the client's automatic retries (a redirect it is answered with is still followed, within the
            // attempt): Files.com forgets an upload's reference when it
            // completes it, so a repeat after a lost answer would report a completed upload as not found.
            await RemoteFile.CreateCore(context.WithoutRetries(), destinationPath, createParams, options, cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="GetDownloadUriWithLoadAsync"/>, without a cancellation token.
        /// </summary>
        public Task<string> GetDownloadUriWithLoad()
        {
            return GetDownloadUriWithLoadAsync(CancellationToken.None);
        }

        /// <summary>
        /// This file's download URI, asking Files.com for one when the file has none, or has one whose signature has
        /// expired.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task<string> GetDownloadUriWithLoadAsync(CancellationToken cancellationToken = default)
        {
            if (DownloadUri != null && !FilesCom.Http.SignedUrl.Of(DownloadUri).ExpiresWithin(TimeSpan.Zero))
            {
                return DownloadUri;
            }
            return await LoadDownloadUri(new OperationContext(FilesClient.Bind(ref client), origin), DictionaryUtil.Copy(options), cancellationToken);
        }

        // Asks the API for a download URI, unless this file already has one that has not expired.
        private async Task<string> LoadDownloadUri(OperationContext context, Dictionary<string, object> requestOptions, CancellationToken cancellationToken)
        {
            if (DownloadUri == null || FilesCom.Http.SignedUrl.Of(DownloadUri).ExpiresWithin(TimeSpan.Zero))
            {
                await RenewDownloadUri(context, requestOptions, cancellationToken);
            }
            return DownloadUri;
        }

        // Asks the API for a new download URI for this file, which then has the attributes Files.com returns, and
        // sends its later requests where this operation's went, as its other operations do.
        private async Task<FilesCom.Http.SignedUrl> RenewDownloadUri(OperationContext context, Dictionary<string, object> requestOptions, CancellationToken cancellationToken)
        {
            RemoteFile f = await RemoteFile.DownloadCore(context, Path, null, requestOptions, cancellationToken);
            attributes = f.attributes;
            origin = context.RelocatedOrigin;
            return FilesCom.Http.SignedUrl.Of(DownloadUri);
        }

        /// <summary>
        /// Same as <see cref="DownloadFileAsync(string, CancellationToken)"/>, without a cancellation token.
        /// </summary>
        /// <param name="outputFile">The local file to write.</param>
        public Task DownloadFile(string outputFile)
        {
            return DownloadFileAsync(outputFile, CancellationToken.None);
        }

        /// <summary>
        /// Downloads the file to <paramref name="outputFile"/>, creating or replacing it.
        /// </summary>
        /// <param name="outputFile">The local file to write.</param>
        /// <param name="cancellationToken">Stops the download.</param>
        /// <remarks>
        /// The local file is closed when the task ends, whether the download succeeds, fails or is cancelled. After a
        /// failure or cancellation it may be left partly written.
        /// </remarks>
        public async Task DownloadFileAsync(string outputFile, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrEmpty(outputFile))
            {
                throw MissingParameterException.For(nameof(outputFile));
            }
            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            using (OperationContext.OperationScope download = context.Begin(cancellationToken))
            {
                System.IO.FileStream fileStream = new System.IO.FileStream(outputFile, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
                try
                {
                    await DownloadAsync(context, fileStream, download.Token);
                }
                catch
                {
                    try
                    {
                        fileStream.Dispose();
                    }
                    catch (Exception)
                    {
                        // Dispose releases the file even when flushing it fails; report the download failure instead.
                    }
                    throw;
                }
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// Same as <see cref="DownloadFileAsync(System.IO.Stream, CancellationToken)"/>, without a cancellation token.
        /// </summary>
        /// <param name="writeStream">The stream to write to. It is borrowed: it is left open, including when the download fails or is cancelled.</param>
        public Task DownloadFile(System.IO.Stream writeStream)
        {
            return DownloadFileAsync(writeStream, CancellationToken.None);
        }

        /// <summary>
        /// Downloads the file into <paramref name="writeStream"/>, from the stream's current position.
        /// </summary>
        /// <param name="writeStream">The stream to write to.</param>
        /// <param name="cancellationToken">Stops the download.</param>
        /// <remarks>
        /// The stream is borrowed: it is left open, including when the download fails or is cancelled. The file's
        /// details, if a download URI must be requested, and its contents come from this file's client. A new
        /// download URI is requested when the file has none, or when its URI is about to expire before a request, as
        /// long as nothing has been written yet: once bytes are written, a failure is not retried.
        /// </remarks>
        public async Task DownloadFileAsync(System.IO.Stream writeStream, CancellationToken cancellationToken = default)
        {
            if (writeStream == null)
            {
                throw MissingParameterException.For(nameof(writeStream));
            }
            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            using (OperationContext.OperationScope download = context.Begin(cancellationToken))
            {
                await DownloadAsync(context, writeStream, download.Token);
            }
        }

        private async Task DownloadAsync(OperationContext context, System.IO.Stream writeStream, CancellationToken cancellationToken)
        {
            try
            {
                Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
                string uri = await LoadDownloadUri(context, requestOptions, cancellationToken);
                await FilesClient.StreamDownload(context, uri, writeStream, token => RenewDownloadUri(context, requestOptions, token), cancellationToken);
            }
            // The failure is thrown as it is, and may be the caller's own stream's, whose message can quote URLs and
            // credentials: only that the download failed is logged below DEBUG.
            catch (Exception e) when (!FilesClient.IsRequestedCancellation(e, cancellationToken))
            {
                log.Error("DownloadFile failed");
                log.Debug($"DownloadFile failed for {Path}", e);
                throw;
            }
        }

        // Waits, as part of the operation of context, until Files.com has nothing at path, checking every interval with
        // options, the operation's own snapshot of the caller's request options.
        internal static async Task WaitUntilGoneCore(OperationContext context, string path, TimeSpan interval, Dictionary<string, object> options, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await FindCore(context, path, null, DictionaryUtil.Copy(options), cancellationToken);
                }
                catch (NotFoundException)
                {
                    return;
                }
                await Task.Delay(interval, cancellationToken);
            }
        }
        /// <summary>
        /// Makes a RemoteFile with no attributes set and no options.
        /// </summary>
        public RemoteFile() : this(null, null) { }

        /// <summary>
        /// Makes a RemoteFile with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public RemoteFile(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("created_by_id"))
            {
                this.attributes.Add("created_by_id", null);
            }
            if (!this.attributes.ContainsKey("created_by_api_key_id"))
            {
                this.attributes.Add("created_by_api_key_id", null);
            }
            if (!this.attributes.ContainsKey("created_by_as2_incoming_message_id"))
            {
                this.attributes.Add("created_by_as2_incoming_message_id", null);
            }
            if (!this.attributes.ContainsKey("created_by_automation_id"))
            {
                this.attributes.Add("created_by_automation_id", null);
            }
            if (!this.attributes.ContainsKey("created_by_bundle_registration_id"))
            {
                this.attributes.Add("created_by_bundle_registration_id", null);
            }
            if (!this.attributes.ContainsKey("created_by_inbox_id"))
            {
                this.attributes.Add("created_by_inbox_id", null);
            }
            if (!this.attributes.ContainsKey("created_by_remote_server_id"))
            {
                this.attributes.Add("created_by_remote_server_id", null);
            }
            if (!this.attributes.ContainsKey("created_by_sync_id"))
            {
                this.attributes.Add("created_by_sync_id", null);
            }
            if (!this.attributes.ContainsKey("custom_metadata"))
            {
                this.attributes.Add("custom_metadata", null);
            }
            if (!this.attributes.ContainsKey("display_name"))
            {
                this.attributes.Add("display_name", null);
            }
            if (!this.attributes.ContainsKey("type"))
            {
                this.attributes.Add("type", null);
            }
            if (!this.attributes.ContainsKey("size"))
            {
                this.attributes.Add("size", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("last_modified_by_id"))
            {
                this.attributes.Add("last_modified_by_id", null);
            }
            if (!this.attributes.ContainsKey("last_modified_by_api_key_id"))
            {
                this.attributes.Add("last_modified_by_api_key_id", null);
            }
            if (!this.attributes.ContainsKey("last_modified_by_automation_id"))
            {
                this.attributes.Add("last_modified_by_automation_id", null);
            }
            if (!this.attributes.ContainsKey("last_modified_by_bundle_registration_id"))
            {
                this.attributes.Add("last_modified_by_bundle_registration_id", null);
            }
            if (!this.attributes.ContainsKey("last_modified_by_remote_server_id"))
            {
                this.attributes.Add("last_modified_by_remote_server_id", null);
            }
            if (!this.attributes.ContainsKey("last_modified_by_sync_id"))
            {
                this.attributes.Add("last_modified_by_sync_id", null);
            }
            if (!this.attributes.ContainsKey("mtime"))
            {
                this.attributes.Add("mtime", null);
            }
            if (!this.attributes.ContainsKey("provided_mtime"))
            {
                this.attributes.Add("provided_mtime", null);
            }
            if (!this.attributes.ContainsKey("crc32"))
            {
                this.attributes.Add("crc32", null);
            }
            if (!this.attributes.ContainsKey("md5"))
            {
                this.attributes.Add("md5", null);
            }
            if (!this.attributes.ContainsKey("sha1"))
            {
                this.attributes.Add("sha1", null);
            }
            if (!this.attributes.ContainsKey("sha256"))
            {
                this.attributes.Add("sha256", null);
            }
            if (!this.attributes.ContainsKey("mime_type"))
            {
                this.attributes.Add("mime_type", null);
            }
            if (!this.attributes.ContainsKey("region"))
            {
                this.attributes.Add("region", null);
            }
            if (!this.attributes.ContainsKey("permissions"))
            {
                this.attributes.Add("permissions", null);
            }
            if (!this.attributes.ContainsKey("subfolders_locked?"))
            {
                this.attributes.Add("subfolders_locked?", false);
            }
            if (!this.attributes.ContainsKey("is_locked"))
            {
                this.attributes.Add("is_locked", false);
            }
            if (!this.attributes.ContainsKey("download_uri"))
            {
                this.attributes.Add("download_uri", null);
            }
            if (!this.attributes.ContainsKey("direct_connection_info"))
            {
                this.attributes.Add("direct_connection_info", null);
            }
            if (!this.attributes.ContainsKey("priority_color"))
            {
                this.attributes.Add("priority_color", null);
            }
            if (!this.attributes.ContainsKey("preview_id"))
            {
                this.attributes.Add("preview_id", null);
            }
            if (!this.attributes.ContainsKey("preview"))
            {
                this.attributes.Add("preview", null);
            }
            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("length"))
            {
                this.attributes.Add("length", null);
            }
            if (!this.attributes.ContainsKey("mkdir_parents"))
            {
                this.attributes.Add("mkdir_parents", false);
            }
            if (!this.attributes.ContainsKey("part"))
            {
                this.attributes.Add("part", null);
            }
            if (!this.attributes.ContainsKey("parts"))
            {
                this.attributes.Add("parts", null);
            }
            if (!this.attributes.ContainsKey("ref"))
            {
                this.attributes.Add("ref", null);
            }
            if (!this.attributes.ContainsKey("restart"))
            {
                this.attributes.Add("restart", null);
            }
            if (!this.attributes.ContainsKey("copy_behaviors"))
            {
                this.attributes.Add("copy_behaviors", false);
            }
            if (!this.attributes.ContainsKey("structure"))
            {
                this.attributes.Add("structure", null);
            }
            if (!this.attributes.ContainsKey("with_rename"))
            {
                this.attributes.Add("with_rename", false);
            }
            if (!this.attributes.ContainsKey("buffered_upload"))
            {
                this.attributes.Add("buffered_upload", false);
            }
            if (!this.attributes.ContainsKey("with_direct_connection_info"))
            {
                this.attributes.Add("with_direct_connection_info", false);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.client = client;
            this.origin = origin;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => origin;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[] { DirectConnectionInfo, Preview }; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }

        // What is read from JSON, such as a response from Files.com, is not waiting to be saved.
        void IJsonOnDeserialized.OnDeserialized()
        {
            unsaved.Clear();
        }


        /// <summary>
        /// File/Folder path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; unsaved.Add("path"); }
        }

        /// <summary>
        /// User ID of the User who created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_id")]
        public Nullable<Int64> CreatedById
        {
            get { return (Nullable<Int64>)attributes["created_by_id"]; }
            set { attributes["created_by_id"] = value; unsaved.Add("created_by_id"); }
        }

        /// <summary>
        /// ID of the API key that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_api_key_id")]
        public Nullable<Int64> CreatedByApiKeyId
        {
            get { return (Nullable<Int64>)attributes["created_by_api_key_id"]; }
            set { attributes["created_by_api_key_id"] = value; unsaved.Add("created_by_api_key_id"); }
        }

        /// <summary>
        /// ID of the AS2 Incoming Message that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_as2_incoming_message_id")]
        public Nullable<Int64> CreatedByAs2IncomingMessageId
        {
            get { return (Nullable<Int64>)attributes["created_by_as2_incoming_message_id"]; }
            set { attributes["created_by_as2_incoming_message_id"] = value; unsaved.Add("created_by_as2_incoming_message_id"); }
        }

        /// <summary>
        /// ID of the Automation that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_automation_id")]
        public Nullable<Int64> CreatedByAutomationId
        {
            get { return (Nullable<Int64>)attributes["created_by_automation_id"]; }
            set { attributes["created_by_automation_id"] = value; unsaved.Add("created_by_automation_id"); }
        }

        /// <summary>
        /// ID of the Bundle Registration that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_bundle_registration_id")]
        public Nullable<Int64> CreatedByBundleRegistrationId
        {
            get { return (Nullable<Int64>)attributes["created_by_bundle_registration_id"]; }
            set { attributes["created_by_bundle_registration_id"] = value; unsaved.Add("created_by_bundle_registration_id"); }
        }

        /// <summary>
        /// ID of the Inbox that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_inbox_id")]
        public Nullable<Int64> CreatedByInboxId
        {
            get { return (Nullable<Int64>)attributes["created_by_inbox_id"]; }
            set { attributes["created_by_inbox_id"] = value; unsaved.Add("created_by_inbox_id"); }
        }

        /// <summary>
        /// ID of the Remote Server that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_remote_server_id")]
        public Nullable<Int64> CreatedByRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["created_by_remote_server_id"]; }
            set { attributes["created_by_remote_server_id"] = value; unsaved.Add("created_by_remote_server_id"); }
        }

        /// <summary>
        /// ID of the Sync that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_sync_id")]
        public Nullable<Int64> CreatedBySyncId
        {
            get { return (Nullable<Int64>)attributes["created_by_sync_id"]; }
            set { attributes["created_by_sync_id"] = value; unsaved.Add("created_by_sync_id"); }
        }

        /// <summary>
        /// Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
        /// </summary>
        [JsonPropertyName("custom_metadata")]
        public object CustomMetadata
        {
            get { return (object)attributes["custom_metadata"]; }
            set { attributes["custom_metadata"] = value; unsaved.Add("custom_metadata"); }
        }

        /// <summary>
        /// File/Folder display name
        /// </summary>
        [JsonPropertyName("display_name")]
        public string DisplayName
        {
            get { return (string)attributes["display_name"]; }
            set { attributes["display_name"] = value; unsaved.Add("display_name"); }
        }

        /// <summary>
        /// Type: `directory` or `file`.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string)attributes["type"]; }
            set { attributes["type"] = value; unsaved.Add("type"); }
        }

        /// <summary>
        /// File/Folder size
        /// </summary>
        [JsonPropertyName("size")]
        public Nullable<Int64> Size
        {
            get { return (Nullable<Int64>)attributes["size"]; }
            set { attributes["size"] = value; unsaved.Add("size"); }
        }

        /// <summary>
        /// File created date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// User ID of the User who last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_id")]
        public Nullable<Int64> LastModifiedById
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_id"]; }
            set { attributes["last_modified_by_id"] = value; unsaved.Add("last_modified_by_id"); }
        }

        /// <summary>
        /// ID of the API key that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_api_key_id")]
        public Nullable<Int64> LastModifiedByApiKeyId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_api_key_id"]; }
            set { attributes["last_modified_by_api_key_id"] = value; unsaved.Add("last_modified_by_api_key_id"); }
        }

        /// <summary>
        /// ID of the Automation that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_automation_id")]
        public Nullable<Int64> LastModifiedByAutomationId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_automation_id"]; }
            set { attributes["last_modified_by_automation_id"] = value; unsaved.Add("last_modified_by_automation_id"); }
        }

        /// <summary>
        /// ID of the Bundle Registration that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_bundle_registration_id")]
        public Nullable<Int64> LastModifiedByBundleRegistrationId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_bundle_registration_id"]; }
            set { attributes["last_modified_by_bundle_registration_id"] = value; unsaved.Add("last_modified_by_bundle_registration_id"); }
        }

        /// <summary>
        /// ID of the Remote Server that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_remote_server_id")]
        public Nullable<Int64> LastModifiedByRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_remote_server_id"]; }
            set { attributes["last_modified_by_remote_server_id"] = value; unsaved.Add("last_modified_by_remote_server_id"); }
        }

        /// <summary>
        /// ID of the Sync that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_sync_id")]
        public Nullable<Int64> LastModifiedBySyncId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_sync_id"]; }
            set { attributes["last_modified_by_sync_id"] = value; unsaved.Add("last_modified_by_sync_id"); }
        }

        /// <summary>
        /// File last modified date/time, according to the server.  This is the timestamp of the last Files.com operation of the file, regardless of what modified timestamp was sent.
        /// </summary>
        [JsonPropertyName("mtime")]
        public Nullable<DateTime> Mtime
        {
            get { return (Nullable<DateTime>)attributes["mtime"]; }
            set { attributes["mtime"] = value; unsaved.Add("mtime"); }
        }

        /// <summary>
        /// File last modified date/time, according to the client who set it.  Files.com allows desktop, FTP, SFTP, and WebDAV clients to set modified at times.  This allows Desktop&lt;-&gt;Cloud syncing to preserve modified at times.
        /// </summary>
        [JsonPropertyName("provided_mtime")]
        public Nullable<DateTime> ProvidedMtime
        {
            get { return (Nullable<DateTime>)attributes["provided_mtime"]; }
            set { attributes["provided_mtime"] = value; unsaved.Add("provided_mtime"); }
        }

        /// <summary>
        /// File CRC32 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("crc32")]
        public string Crc32
        {
            get { return (string)attributes["crc32"]; }
            set { attributes["crc32"] = value; unsaved.Add("crc32"); }
        }

        /// <summary>
        /// File MD5 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("md5")]
        public string Md5
        {
            get { return (string)attributes["md5"]; }
            set { attributes["md5"] = value; unsaved.Add("md5"); }
        }

        /// <summary>
        /// File SHA1 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("sha1")]
        public string Sha1
        {
            get { return (string)attributes["sha1"]; }
            set { attributes["sha1"] = value; unsaved.Add("sha1"); }
        }

        /// <summary>
        /// File SHA256 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("sha256")]
        public string Sha256
        {
            get { return (string)attributes["sha256"]; }
            set { attributes["sha256"] = value; unsaved.Add("sha256"); }
        }

        /// <summary>
        /// MIME Type.  This is determined by the filename extension and is not stored separately internally.
        /// </summary>
        [JsonPropertyName("mime_type")]
        public string MimeType
        {
            get { return (string)attributes["mime_type"]; }
            set { attributes["mime_type"] = value; unsaved.Add("mime_type"); }
        }

        /// <summary>
        /// Region location
        /// </summary>
        [JsonPropertyName("region")]
        public string Region
        {
            get { return (string)attributes["region"]; }
            set { attributes["region"] = value; unsaved.Add("region"); }
        }

        /// <summary>
        /// A short string representing the current user's permissions.  Can be `r` (Read),`w` (Write),`d` (Delete), `l` (List) or any combination
        /// </summary>
        [JsonPropertyName("permissions")]
        public string Permissions
        {
            get { return (string)attributes["permissions"]; }
            set { attributes["permissions"] = value; unsaved.Add("permissions"); }
        }

        /// <summary>
        /// Are subfolders locked and unable to be modified?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("subfolders_locked?")]
        public bool SubfoldersLocked
        {
            get { return attributes["subfolders_locked?"] == null ? false : (bool)attributes["subfolders_locked?"]; }
            set { attributes["subfolders_locked?"] = value; unsaved.Add("subfolders_locked?"); }
        }

        /// <summary>
        /// Is this folder locked and unable to be modified?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("is_locked")]
        public bool IsLocked
        {
            get { return attributes["is_locked"] == null ? false : (bool)attributes["is_locked"]; }
            set { attributes["is_locked"] = value; unsaved.Add("is_locked"); }
        }

        /// <summary>
        /// Link to download file. Provided only in response to a download request.
        /// </summary>
        [JsonPropertyName("download_uri")]
        public string DownloadUri
        {
            get { return (string)attributes["download_uri"]; }
            set { attributes["download_uri"] = value; unsaved.Add("download_uri"); }
        }

        /// <summary>
        /// Optional direct connection information for direct Agent transfer attempts
        /// </summary>
        [JsonPropertyName("direct_connection_info")]
        public DirectConnectionInfo DirectConnectionInfo
        {
            get { return (DirectConnectionInfo)attributes["direct_connection_info"]; }
            set { attributes["direct_connection_info"] = value; unsaved.Add("direct_connection_info"); }
        }

        /// <summary>
        /// Bookmark/priority color of file/folder
        /// </summary>
        [JsonPropertyName("priority_color")]
        public string PriorityColor
        {
            get { return (string)attributes["priority_color"]; }
            set { attributes["priority_color"] = value; unsaved.Add("priority_color"); }
        }

        /// <summary>
        /// File preview ID
        /// </summary>
        [JsonPropertyName("preview_id")]
        public Nullable<Int64> PreviewId
        {
            get { return (Nullable<Int64>)attributes["preview_id"]; }
            set { attributes["preview_id"] = value; unsaved.Add("preview_id"); }
        }

        /// <summary>
        /// File preview
        /// </summary>
        [JsonPropertyName("preview")]
        public Preview Preview
        {
            get { return (Preview)attributes["preview"]; }
            set { attributes["preview"] = value; unsaved.Add("preview"); }
        }

        /// <summary>
        /// The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            set { attributes["action"] = value; unsaved.Add("action"); }
        }

        /// <summary>
        /// Length of file.
        /// </summary>
        [JsonPropertyName("length")]
        public Nullable<Int64> Length
        {
            get { return (Nullable<Int64>)attributes["length"]; }
            set { attributes["length"] = value; unsaved.Add("length"); }
        }

        /// <summary>
        /// Create parent directories if they do not exist?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mkdir_parents")]
        public bool MkdirParents
        {
            get { return attributes["mkdir_parents"] == null ? false : (bool)attributes["mkdir_parents"]; }
            set { attributes["mkdir_parents"] = value; unsaved.Add("mkdir_parents"); }
        }

        /// <summary>
        /// Part if uploading a part.
        /// </summary>
        [JsonPropertyName("part")]
        public Nullable<Int64> Part
        {
            get { return (Nullable<Int64>)attributes["part"]; }
            set { attributes["part"] = value; unsaved.Add("part"); }
        }

        /// <summary>
        /// How many parts to fetch?
        /// </summary>
        [JsonPropertyName("parts")]
        public Nullable<Int64> Parts
        {
            get { return (Nullable<Int64>)attributes["parts"]; }
            set { attributes["parts"] = value; unsaved.Add("parts"); }
        }

        /// <summary>
        /// The ref parameter of the Files.com API request POST /files/{path}.
        /// </summary>
        [JsonPropertyName("ref")]
        public string Ref
        {
            get { return (string)attributes["ref"]; }
            set { attributes["ref"] = value; unsaved.Add("ref"); }
        }

        /// <summary>
        /// File byte offset to restart from.
        /// </summary>
        [JsonPropertyName("restart")]
        public Nullable<Int64> Restart
        {
            get { return (Nullable<Int64>)attributes["restart"]; }
            set { attributes["restart"] = value; unsaved.Add("restart"); }
        }

        /// <summary>
        /// If copying a folder, also copy supported behaviors, email notification subscriptions, and per-folder branding to the destination folder tree?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("copy_behaviors")]
        public bool CopyBehaviors
        {
            get { return attributes["copy_behaviors"] == null ? false : (bool)attributes["copy_behaviors"]; }
            set { attributes["copy_behaviors"] = value; unsaved.Add("copy_behaviors"); }
        }

        /// <summary>
        /// If copying folder, copy just the structure?
        /// </summary>
        [JsonPropertyName("structure")]
        public string Structure
        {
            get { return (string)attributes["structure"]; }
            set { attributes["structure"] = value; unsaved.Add("structure"); }
        }

        /// <summary>
        /// Allow file rename instead of overwrite?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("with_rename")]
        public bool WithRename
        {
            get { return attributes["with_rename"] == null ? false : (bool)attributes["with_rename"]; }
            set { attributes["with_rename"] = value; unsaved.Add("with_rename"); }
        }

        /// <summary>
        /// If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("buffered_upload")]
        public bool BufferedUpload
        {
            get { return attributes["buffered_upload"] == null ? false : (bool)attributes["buffered_upload"]; }
            set { attributes["buffered_upload"] = value; unsaved.Add("buffered_upload"); }
        }

        /// <summary>
        /// Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("with_direct_connection_info")]
        public bool WithDirectConnectionInfo
        {
            get { return attributes["with_direct_connection_info"] == null ? false : (bool)attributes["with_direct_connection_info"]; }
            set { attributes["with_direct_connection_info"] = value; unsaved.Add("with_direct_connection_info"); }
        }

        /// <summary>
        /// Download File
        ///
        /// Parameters:
        ///   action - string - Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<RemoteFile> Download(Dictionary<string, object> parameters)
        {
            return DownloadCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Download File
        ///
        /// Parameters:
        ///   action - string - Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<RemoteFile> DownloadAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DownloadCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<RemoteFile> DownloadCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which Download needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("action", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action is null, which cannot be sent" : "Bad parameter: action must be of type string", "action");
            }
            if (parameters.TryGetValue("preview_size", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: preview_size is null, which cannot be sent" : "Bad parameter: preview_size must be of type string", "preview_size");
            }
            if (parameters.TryGetValue("with_previews", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_previews is null, which cannot be sent" : "Bad parameter: with_previews must be of type bool", "with_previews");
            }
            if (parameters.TryGetValue("with_priority_color", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_priority_color is null, which cannot be sent" : "Bad parameter: with_priority_color must be of type bool", "with_priority_color");
            }
            if (parameters.TryGetValue("with_direct_connection_info", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_direct_connection_info is null, which cannot be sent" : "Bad parameter: with_direct_connection_info must be of type bool", "with_direct_connection_info");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, requestOptions, cancellationToken);
            RemoteFile result = JsonUtil.DeserializeModelResponse<RemoteFile>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Parameters:
        ///   custom_metadata - object - Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
        ///   provided_mtime - string - Modified time of file.
        ///   priority_color - string - Priority/Bookmark color of file.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The RemoteFile it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<RemoteFile> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   custom_metadata - object - Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
        ///   provided_mtime - string - Modified time of file.
        ///   priority_color - string - Priority/Bookmark color of file.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The RemoteFile it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<RemoteFile> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<RemoteFile> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which Update needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("custom_metadata", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: custom_metadata is null, which cannot be sent" : "Bad parameter: custom_metadata must be of type object", "custom_metadata");
            }
            if (parameters.TryGetValue("provided_mtime", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: provided_mtime is null, which cannot be sent" : "Bad parameter: provided_mtime must be of type string", "provided_mtime");
            }
            if (parameters.TryGetValue("priority_color", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: priority_color is null, which cannot be sent" : "Bad parameter: priority_color must be of type string", "priority_color");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            RemoteFile updated = JsonUtil.DeserializeModelResponse<RemoteFile>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<RemoteFile>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>, but neither waits for the request nor reports its failure.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        [Obsolete("Destroy neither waits for the request nor reports its failure. Use DestroyAsync instead.")]
        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which Delete needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("recursive", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recursive is null, which cannot be sent" : "Bad parameter: recursive must be of type bool", "recursive");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// List the contents of a ZIP file
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides path, which this object supplies.</param>
        public Task<ZipListEntry[]> ZipListContents(Dictionary<string, object> parameters)
        {
            return ZipListContentsCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// List the contents of a ZIP file
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides path, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<ZipListEntry[]> ZipListContentsAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ZipListContentsCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ZipListEntry[]> ZipListContentsCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which ZipListContents needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/zip_list/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, requestOptions, cancellationToken);
            ZipListEntry[] result = JsonUtil.DeserializeModelResponse<ZipListEntry[]>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Delete File/Folder and return its File Action.  A pending or processing deletion continues in the background after this returns; check its File Migration for completion.  Waiting for the File Migration does not cancel the deletion
        ///
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<FileAction> BeginDelete(Dictionary<string, object> parameters)
        {
            return BeginDeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Delete File/Folder and return its File Action.  A pending or processing deletion continues in the background after this returns; check its File Migration for completion.  Waiting for the File Migration does not cancel the deletion
        ///
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> BeginDeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return BeginDeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> BeginDeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which BeginDelete needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("recursive", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recursive is null, which cannot be sent" : "Bad parameter: recursive must be of type bool", "recursive");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/delete/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            FileAction result = JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Copy File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Copy destination path.
        ///   copy_behaviors - boolean - If copying a folder, also copy supported behaviors, email notification subscriptions, and per-folder branding to the destination folder tree?
        ///   structure - boolean - Copy structure only?
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<FileAction> Copy(Dictionary<string, object> parameters)
        {
            return CopyCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Copy File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Copy destination path.
        ///   copy_behaviors - boolean - If copying a folder, also copy supported behaviors, email notification subscriptions, and per-folder branding to the destination folder tree?
        ///   structure - boolean - Copy structure only?
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> CopyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return CopyCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> CopyCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which Copy needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("copy_behaviors", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: copy_behaviors is null, which cannot be sent" : "Bad parameter: copy_behaviors must be of type bool", "copy_behaviors");
            }
            if (parameters.TryGetValue("structure", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: structure is null, which cannot be sent" : "Bad parameter: structure must be of type bool", "structure");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/copy/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            FileAction result = JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Move File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<FileAction> Move(Dictionary<string, object> parameters)
        {
            return MoveCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Move File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> MoveAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return MoveCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> MoveCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which Move needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/move/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            FileAction result = JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Transform a file and save the output to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the transformed output.
        ///   transform_type (required) - string - Transform type. Supported values are `image_convert`, `document_convert`, and `files_transform_script_execute`.
        ///   target_format (required) - string - Destination format to create.
        ///   script - string - Files TransformScript source. Required when transform_type is `files_transform_script_execute`.
        ///   width - int64 - Maximum output width for image_convert.
        ///   height - int64 - Maximum output height for image_convert.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<FileAction> Transform(Dictionary<string, object> parameters)
        {
            return TransformCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Transform a file and save the output to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the transformed output.
        ///   transform_type (required) - string - Transform type. Supported values are `image_convert`, `document_convert`, and `files_transform_script_execute`.
        ///   target_format (required) - string - Destination format to create.
        ///   script - string - Files TransformScript source. Required when transform_type is `files_transform_script_execute`.
        ///   width - int64 - Maximum output width for image_convert.
        ///   height - int64 - Maximum output height for image_convert.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> TransformAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return TransformCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> TransformCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which Transform needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (!parameters.TryGetValue("transform_type", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: transform_type", "transform_type");
            }
            if (!parameters.TryGetValue("target_format", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: target_format", "target_format");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("transform_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: transform_type is null, which cannot be sent" : "Bad parameter: transform_type must be of type string", "transform_type");
            }
            if (parameters.TryGetValue("target_format", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: target_format is null, which cannot be sent" : "Bad parameter: target_format must be of type string", "target_format");
            }
            if (parameters.TryGetValue("script", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: script is null, which cannot be sent" : "Bad parameter: script must be of type string", "script");
            }
            if (parameters.TryGetValue("width", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: width is null, which cannot be sent" : "Bad parameter: width must be of type Nullable<Int64>", "width");
            }
            if (parameters.TryGetValue("height", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: height is null, which cannot be sent" : "Bad parameter: height must be of type Nullable<Int64>", "height");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/transform/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            FileAction result = JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Decrypt a GPG-encrypted file and save it to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the decrypted file.
        ///   gpg_key_ids - array(int64) - GPG Key IDs to decrypt with. If omitted, every accessible private GPG key in the source workspace is used.
        ///   gpg_key_partner_id - int64 - Partner ID whose GPG keys should be used for decryption.
        ///   use_all_private_keys - boolean - Use every accessible private GPG key in the source workspace for decryption.
        ///   ignore_mdc_error - boolean - Ignore errors from the MDC (modification detection code) check.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<FileAction> GpgDecrypt(Dictionary<string, object> parameters)
        {
            return GpgDecryptCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Decrypt a GPG-encrypted file and save it to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the decrypted file.
        ///   gpg_key_ids - array(int64) - GPG Key IDs to decrypt with. If omitted, every accessible private GPG key in the source workspace is used.
        ///   gpg_key_partner_id - int64 - Partner ID whose GPG keys should be used for decryption.
        ///   use_all_private_keys - boolean - Use every accessible private GPG key in the source workspace for decryption.
        ///   ignore_mdc_error - boolean - Ignore errors from the MDC (modification detection code) check.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> GpgDecryptAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return GpgDecryptCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> GpgDecryptCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which GpgDecrypt needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("gpg_key_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: gpg_key_ids is null, which cannot be sent" : "Bad parameter: gpg_key_ids must be of type Nullable<Int64>[]", "gpg_key_ids");
            }
            if (parameters.TryGetValue("gpg_key_partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: gpg_key_partner_id is null, which cannot be sent" : "Bad parameter: gpg_key_partner_id must be of type Nullable<Int64>", "gpg_key_partner_id");
            }
            if (parameters.TryGetValue("use_all_private_keys", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: use_all_private_keys is null, which cannot be sent" : "Bad parameter: use_all_private_keys must be of type bool", "use_all_private_keys");
            }
            if (parameters.TryGetValue("ignore_mdc_error", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ignore_mdc_error is null, which cannot be sent" : "Bad parameter: ignore_mdc_error must be of type bool", "ignore_mdc_error");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/gpg_decrypt/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            FileAction result = JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Encrypt a file with GPG and save it to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the encrypted file.
        ///   gpg_key_ids - array(int64) - GPG Key IDs to encrypt with.
        ///   gpg_key_partner_id - int64 - Partner ID whose GPG keys should be used for encryption.
        ///   signing_key_id - int64 - Optional GPG Key ID to sign with.
        ///   armor - boolean - Output ASCII-armored encrypted data.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<FileAction> GpgEncrypt(Dictionary<string, object> parameters)
        {
            return GpgEncryptCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Encrypt a file with GPG and save it to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the encrypted file.
        ///   gpg_key_ids - array(int64) - GPG Key IDs to encrypt with.
        ///   gpg_key_partner_id - int64 - Partner ID whose GPG keys should be used for encryption.
        ///   signing_key_id - int64 - Optional GPG Key ID to sign with.
        ///   armor - boolean - Output ASCII-armored encrypted data.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> GpgEncryptAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return GpgEncryptCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> GpgEncryptCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which GpgEncrypt needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("gpg_key_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: gpg_key_ids is null, which cannot be sent" : "Bad parameter: gpg_key_ids must be of type Nullable<Int64>[]", "gpg_key_ids");
            }
            if (parameters.TryGetValue("gpg_key_partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: gpg_key_partner_id is null, which cannot be sent" : "Bad parameter: gpg_key_partner_id must be of type Nullable<Int64>", "gpg_key_partner_id");
            }
            if (parameters.TryGetValue("signing_key_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: signing_key_id is null, which cannot be sent" : "Bad parameter: signing_key_id must be of type Nullable<Int64>", "signing_key_id");
            }
            if (parameters.TryGetValue("armor", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: armor is null, which cannot be sent" : "Bad parameter: armor must be of type bool", "armor");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/gpg_encrypt/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            FileAction result = JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Extract a ZIP file to a destination folder
        ///
        /// Parameters:
        ///   destination (required) - string - Destination folder path for extracted files.
        ///   filename - string - Optional single entry filename to extract.
        ///   overwrite - boolean - Overwrite existing files in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<FileAction> Unzip(Dictionary<string, object> parameters)
        {
            return UnzipCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Extract a ZIP file to a destination folder
        ///
        /// Parameters:
        ///   destination (required) - string - Destination folder path for extracted files.
        ///   filename - string - Optional single entry filename to extract.
        ///   overwrite - boolean - Overwrite existing files in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileAction> UnzipAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UnzipCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> UnzipCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which Unzip needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("filename", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filename is null, which cannot be sent" : "Bad parameter: filename must be of type string", "filename");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/unzip", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            FileAction result = JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Begin File Upload
        ///
        /// Parameters:
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Total bytes of file being uploaded (include bytes being retained if appending/restarting).
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        ///   buffered_upload - boolean - If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<FileUploadPart[]> BeginUpload(Dictionary<string, object> parameters)
        {
            return BeginUploadCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Begin File Upload
        ///
        /// Parameters:
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Total bytes of file being uploaded (include bytes being retained if appending/restarting).
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        ///   buffered_upload - boolean - If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<FileUploadPart[]> BeginUploadAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return BeginUploadCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileUploadPart[]> BeginUploadCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteFile has no path, which BeginUpload needs", "path");
            }
            parameters["path"] = parameterValue;
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("mkdir_parents", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mkdir_parents is null, which cannot be sent" : "Bad parameter: mkdir_parents must be of type bool", "mkdir_parents");
            }
            if (parameters.TryGetValue("part", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: part is null, which cannot be sent" : "Bad parameter: part must be of type Nullable<Int64>", "part");
            }
            if (parameters.TryGetValue("parts", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: parts is null, which cannot be sent" : "Bad parameter: parts must be of type Nullable<Int64>", "parts");
            }
            if (parameters.TryGetValue("ref", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ref is null, which cannot be sent" : "Bad parameter: ref must be of type string", "ref");
            }
            if (parameters.TryGetValue("restart", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restart is null, which cannot be sent" : "Bad parameter: restart must be of type Nullable<Int64>", "restart");
            }
            if (parameters.TryGetValue("size", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: size is null, which cannot be sent" : "Bad parameter: size must be of type Nullable<Int64>", "size");
            }
            if (parameters.TryGetValue("with_rename", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_rename is null, which cannot be sent" : "Bad parameter: with_rename must be of type bool", "with_rename");
            }
            if (parameters.TryGetValue("buffered_upload", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: buffered_upload is null, which cannot be sent" : "Bad parameter: buffered_upload must be of type bool", "buffered_upload");
            }
            if (parameters.TryGetValue("with_direct_connection_info", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_direct_connection_info is null, which cannot be sent" : "Bad parameter: with_direct_connection_info must be of type bool", "with_direct_connection_info");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/begin_upload/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            FileUploadPart[] result = JsonUtil.DeserializeModelResponse<FileUploadPart[]>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }


        private static readonly string[] CreateParameterNames = { "path", "action", "custom_metadata", "length", "mkdir_parents", "part", "parts", "provided_mtime", "ref", "restart", "size", "copy_behaviors", "structure", "with_rename", "buffered_upload", "with_direct_connection_info" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this RemoteFile on Files.com.
        /// It sends the attributes set on this object, in its constructor or with its properties, or changed in place,
        /// such as an element of an array, since it was made or last came from Files.com, of those that operation
        /// takes; it sends no other attributes, and no defaults. Each goes in the form its parameter takes, such as a
        /// DateTime as ISO 8601 text. When it succeeds, this object takes the attributes Files.com returns.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            RemoteFile created = await RemoteFile.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), Path, AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
            Refresh(created.getAttributes());
            origin = ((IModel)created).Origin;
        }

        // The attributes set or changed on this object since it was made or last came from Files.com, of those named
        // in parameterNames: the parameters of the operation that saves it.
        private Dictionary<string, object> AttributesToSave(string[] parameterNames)
        {
            Dictionary<string, object> toSave = new Dictionary<string, object>();
            foreach (string name in parameterNames)
            {
                if (unsaved.Contains(name))
                {
                    toSave[name] = ToParameter(name, attributes[name]);
                }
            }
            return toSave;
        }

        // The value SaveAsync sends for an attribute whose property type is not the one its parameter takes.
        private static object ToParameter(string name, object value)
        {
            switch (name)
            {
                case "provided_mtime":
                    return ParameterValue.FromDateTime(value);
                default:
                    return value;
            }
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
        }

        /// <summary>
        /// Download File
        ///
        /// Parameters:
        ///   action - string - Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteFile> Download(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DownloadCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteFile> DownloadCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("action", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action is null, which cannot be sent" : "Bad parameter: action must be of type string", "action");
            }
            if (parameters.TryGetValue("preview_size", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: preview_size is null, which cannot be sent" : "Bad parameter: preview_size must be of type string", "preview_size");
            }
            if (parameters.TryGetValue("with_previews", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_previews is null, which cannot be sent" : "Bad parameter: with_previews must be of type bool", "with_previews");
            }
            if (parameters.TryGetValue("with_priority_color", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_priority_color is null, which cannot be sent" : "Bad parameter: with_priority_color must be of type bool", "with_priority_color");
            }
            if (parameters.TryGetValue("with_direct_connection_info", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_direct_connection_info is null, which cannot be sent" : "Bad parameter: with_direct_connection_info must be of type bool", "with_direct_connection_info");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteFile>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   action - string - The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
        ///   custom_metadata - object - Custom metadata map to save when `action=end` completes the upload.  Replaces existing metadata; an empty map clears it.  No separate metadata-edit permission is required.  Supported on native files and configured remote mounts, excluding remote server automount paths.  Limited to 32 keys, 256 characters per key and 1024 characters per value.
        ///   etags[etag] (required) - array(string) - etag identifier.
        ///   etags[part] (required) - array(int64) - Part number.
        ///   length - int64 - Length of file.
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   provided_mtime - string - User provided modification time.
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Size of file.
        ///   copy_behaviors - boolean - If copying a folder, also copy supported behaviors, email notification subscriptions, and per-folder branding to the destination folder tree?
        ///   structure - string - If copying folder, copy just the structure?
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        ///   buffered_upload - boolean - If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteFile> Create(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteFile> CreateCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("action", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action is null, which cannot be sent" : "Bad parameter: action must be of type string", "action");
            }
            if (parameters.TryGetValue("custom_metadata", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: custom_metadata is null, which cannot be sent" : "Bad parameter: custom_metadata must be of type object", "custom_metadata");
            }
            if (parameters.TryGetValue("length", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: length is null, which cannot be sent" : "Bad parameter: length must be of type Nullable<Int64>", "length");
            }
            if (parameters.TryGetValue("mkdir_parents", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mkdir_parents is null, which cannot be sent" : "Bad parameter: mkdir_parents must be of type bool", "mkdir_parents");
            }
            if (parameters.TryGetValue("part", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: part is null, which cannot be sent" : "Bad parameter: part must be of type Nullable<Int64>", "part");
            }
            if (parameters.TryGetValue("parts", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: parts is null, which cannot be sent" : "Bad parameter: parts must be of type Nullable<Int64>", "parts");
            }
            if (parameters.TryGetValue("provided_mtime", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: provided_mtime is null, which cannot be sent" : "Bad parameter: provided_mtime must be of type string", "provided_mtime");
            }
            if (parameters.TryGetValue("ref", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ref is null, which cannot be sent" : "Bad parameter: ref must be of type string", "ref");
            }
            if (parameters.TryGetValue("restart", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restart is null, which cannot be sent" : "Bad parameter: restart must be of type Nullable<Int64>", "restart");
            }
            if (parameters.TryGetValue("size", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: size is null, which cannot be sent" : "Bad parameter: size must be of type Nullable<Int64>", "size");
            }
            if (parameters.TryGetValue("copy_behaviors", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: copy_behaviors is null, which cannot be sent" : "Bad parameter: copy_behaviors must be of type bool", "copy_behaviors");
            }
            if (parameters.TryGetValue("structure", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: structure is null, which cannot be sent" : "Bad parameter: structure must be of type string", "structure");
            }
            if (parameters.TryGetValue("with_rename", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_rename is null, which cannot be sent" : "Bad parameter: with_rename must be of type bool", "with_rename");
            }
            if (parameters.TryGetValue("buffered_upload", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: buffered_upload is null, which cannot be sent" : "Bad parameter: buffered_upload must be of type bool", "buffered_upload");
            }
            if (parameters.TryGetValue("with_direct_connection_info", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_direct_connection_info is null, which cannot be sent" : "Bad parameter: with_direct_connection_info must be of type bool", "with_direct_connection_info");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteFile>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   custom_metadata - object - Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
        ///   provided_mtime - string - Modified time of file.
        ///   priority_color - string - Priority/Bookmark color of file.
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteFile> Update(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteFile> UpdateCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("custom_metadata", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: custom_metadata is null, which cannot be sent" : "Bad parameter: custom_metadata must be of type object", "custom_metadata");
            }
            if (parameters.TryGetValue("provided_mtime", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: provided_mtime is null, which cannot be sent" : "Bad parameter: provided_mtime must be of type string", "provided_mtime");
            }
            if (parameters.TryGetValue("priority_color", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: priority_color is null, which cannot be sent" : "Bad parameter: priority_color must be of type string", "priority_color");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteFile>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Delete(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Destroy(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(path, parameters, options);
        }

        internal static async Task DeleteCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("recursive", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recursive is null, which cannot be sent" : "Bad parameter: recursive must be of type bool", "recursive");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteFile> Find(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteFile> Get(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(path, parameters, options);
        }

        internal static async Task<RemoteFile> FindCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("preview_size", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: preview_size is null, which cannot be sent" : "Bad parameter: preview_size must be of type string", "preview_size");
            }
            if (parameters.TryGetValue("with_previews", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_previews is null, which cannot be sent" : "Bad parameter: with_previews must be of type bool", "with_previews");
            }
            if (parameters.TryGetValue("with_priority_color", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_priority_color is null, which cannot be sent" : "Bad parameter: with_priority_color must be of type bool", "with_priority_color");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/metadata/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteFile>(responseJson, context, options);
        }

        /// <summary>
        /// List the contents of a ZIP file
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation takes no parameters besides path.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ZipListEntry[]> ZipListContents(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ZipListContentsCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<ZipListEntry[]> ZipListContentsCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/zip_list/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ZipListEntry[]>(responseJson, context, options);
        }

        /// <summary>
        /// Delete File/Folder and return its File Action.  A pending or processing deletion continues in the background after this returns; check its File Migration for completion.  Waiting for the File Migration does not cancel the deletion
        ///
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileAction> BeginDelete(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return BeginDeleteCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<FileAction> BeginDeleteCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("recursive", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recursive is null, which cannot be sent" : "Bad parameter: recursive must be of type bool", "recursive");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/delete/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, options);
        }

        /// <summary>
        /// Copy File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Copy destination path.
        ///   copy_behaviors - boolean - If copying a folder, also copy supported behaviors, email notification subscriptions, and per-folder branding to the destination folder tree?
        ///   structure - boolean - Copy structure only?
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileAction> Copy(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CopyCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<FileAction> CopyCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("copy_behaviors", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: copy_behaviors is null, which cannot be sent" : "Bad parameter: copy_behaviors must be of type bool", "copy_behaviors");
            }
            if (parameters.TryGetValue("structure", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: structure is null, which cannot be sent" : "Bad parameter: structure must be of type bool", "structure");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/copy/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, options);
        }

        /// <summary>
        /// Move File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileAction> Move(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return MoveCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<FileAction> MoveCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/move/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, options);
        }

        /// <summary>
        /// Transform a file and save the output to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the transformed output.
        ///   transform_type (required) - string - Transform type. Supported values are `image_convert`, `document_convert`, and `files_transform_script_execute`.
        ///   target_format (required) - string - Destination format to create.
        ///   script - string - Files TransformScript source. Required when transform_type is `files_transform_script_execute`.
        ///   width - int64 - Maximum output width for image_convert.
        ///   height - int64 - Maximum output height for image_convert.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileAction> Transform(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return TransformCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<FileAction> TransformCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (!parameters.TryGetValue("transform_type", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: transform_type", "transform_type");
            }
            if (!parameters.TryGetValue("target_format", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: target_format", "target_format");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("transform_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: transform_type is null, which cannot be sent" : "Bad parameter: transform_type must be of type string", "transform_type");
            }
            if (parameters.TryGetValue("target_format", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: target_format is null, which cannot be sent" : "Bad parameter: target_format must be of type string", "target_format");
            }
            if (parameters.TryGetValue("script", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: script is null, which cannot be sent" : "Bad parameter: script must be of type string", "script");
            }
            if (parameters.TryGetValue("width", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: width is null, which cannot be sent" : "Bad parameter: width must be of type Nullable<Int64>", "width");
            }
            if (parameters.TryGetValue("height", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: height is null, which cannot be sent" : "Bad parameter: height must be of type Nullable<Int64>", "height");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/transform/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, options);
        }

        /// <summary>
        /// Decrypt a GPG-encrypted file and save it to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the decrypted file.
        ///   gpg_key_ids - array(int64) - GPG Key IDs to decrypt with. If omitted, every accessible private GPG key in the source workspace is used.
        ///   gpg_key_partner_id - int64 - Partner ID whose GPG keys should be used for decryption.
        ///   use_all_private_keys - boolean - Use every accessible private GPG key in the source workspace for decryption.
        ///   ignore_mdc_error - boolean - Ignore errors from the MDC (modification detection code) check.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileAction> GpgDecrypt(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return GpgDecryptCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<FileAction> GpgDecryptCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("gpg_key_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: gpg_key_ids is null, which cannot be sent" : "Bad parameter: gpg_key_ids must be of type Nullable<Int64>[]", "gpg_key_ids");
            }
            if (parameters.TryGetValue("gpg_key_partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: gpg_key_partner_id is null, which cannot be sent" : "Bad parameter: gpg_key_partner_id must be of type Nullable<Int64>", "gpg_key_partner_id");
            }
            if (parameters.TryGetValue("use_all_private_keys", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: use_all_private_keys is null, which cannot be sent" : "Bad parameter: use_all_private_keys must be of type bool", "use_all_private_keys");
            }
            if (parameters.TryGetValue("ignore_mdc_error", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ignore_mdc_error is null, which cannot be sent" : "Bad parameter: ignore_mdc_error must be of type bool", "ignore_mdc_error");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/gpg_decrypt/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, options);
        }

        /// <summary>
        /// Encrypt a file with GPG and save it to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the encrypted file.
        ///   gpg_key_ids - array(int64) - GPG Key IDs to encrypt with.
        ///   gpg_key_partner_id - int64 - Partner ID whose GPG keys should be used for encryption.
        ///   signing_key_id - int64 - Optional GPG Key ID to sign with.
        ///   armor - boolean - Output ASCII-armored encrypted data.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileAction> GpgEncrypt(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return GpgEncryptCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<FileAction> GpgEncryptCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("gpg_key_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: gpg_key_ids is null, which cannot be sent" : "Bad parameter: gpg_key_ids must be of type Nullable<Int64>[]", "gpg_key_ids");
            }
            if (parameters.TryGetValue("gpg_key_partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: gpg_key_partner_id is null, which cannot be sent" : "Bad parameter: gpg_key_partner_id must be of type Nullable<Int64>", "gpg_key_partner_id");
            }
            if (parameters.TryGetValue("signing_key_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: signing_key_id is null, which cannot be sent" : "Bad parameter: signing_key_id must be of type Nullable<Int64>", "signing_key_id");
            }
            if (parameters.TryGetValue("armor", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: armor is null, which cannot be sent" : "Bad parameter: armor must be of type bool", "armor");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/gpg_encrypt/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, options);
        }

        /// <summary>
        /// Extract a ZIP file to a destination folder
        ///
        /// Parameters:
        ///   destination (required) - string - Destination folder path for extracted files.
        ///   filename - string - Optional single entry filename to extract.
        ///   overwrite - boolean - Overwrite existing files in the destination?
        /// </summary>
        /// <param name="path">ZIP file path to extract.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileAction> Unzip(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UnzipCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<FileAction> UnzipCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("filename", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filename is null, which cannot be sent" : "Bad parameter: filename must be of type string", "filename");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/unzip", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   paths (required) - array(string) - Paths to include in the ZIP.
        ///   destination (required) - string - Destination file path for the ZIP.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileAction> Zip(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ZipCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<FileAction> ZipCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("paths", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: paths", "paths");
            }
            if (!parameters.TryGetValue("destination", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination", "destination");
            }
            if (parameters.TryGetValue("paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: paths is null, which cannot be sent" : "Bad parameter: paths must be of type string[]", "paths");
            }
            if (parameters.TryGetValue("destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination is null, which cannot be sent" : "Bad parameter: destination must be of type string", "destination");
            }
            if (parameters.TryGetValue("overwrite", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite is null, which cannot be sent" : "Bad parameter: overwrite must be of type bool", "overwrite");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/zip", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileAction>(responseJson, context, options);
        }

        /// <summary>
        /// Begin File Upload
        ///
        /// Parameters:
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Total bytes of file being uploaded (include bytes being retained if appending/restarting).
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        ///   buffered_upload - boolean - If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FileUploadPart[]> BeginUpload(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return BeginUploadCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<FileUploadPart[]> BeginUploadCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("mkdir_parents", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mkdir_parents is null, which cannot be sent" : "Bad parameter: mkdir_parents must be of type bool", "mkdir_parents");
            }
            if (parameters.TryGetValue("part", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: part is null, which cannot be sent" : "Bad parameter: part must be of type Nullable<Int64>", "part");
            }
            if (parameters.TryGetValue("parts", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: parts is null, which cannot be sent" : "Bad parameter: parts must be of type Nullable<Int64>", "parts");
            }
            if (parameters.TryGetValue("ref", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ref is null, which cannot be sent" : "Bad parameter: ref must be of type string", "ref");
            }
            if (parameters.TryGetValue("restart", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restart is null, which cannot be sent" : "Bad parameter: restart must be of type Nullable<Int64>", "restart");
            }
            if (parameters.TryGetValue("size", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: size is null, which cannot be sent" : "Bad parameter: size must be of type Nullable<Int64>", "size");
            }
            if (parameters.TryGetValue("with_rename", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_rename is null, which cannot be sent" : "Bad parameter: with_rename must be of type bool", "with_rename");
            }
            if (parameters.TryGetValue("buffered_upload", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: buffered_upload is null, which cannot be sent" : "Bad parameter: buffered_upload must be of type bool", "buffered_upload");
            }
            if (parameters.TryGetValue("with_direct_connection_info", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_direct_connection_info is null, which cannot be sent" : "Bad parameter: with_direct_connection_info must be of type bool", "with_direct_connection_info");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/begin_upload/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FileUploadPart[]>(responseJson, context, options);
        }

    }
}