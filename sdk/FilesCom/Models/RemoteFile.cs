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
    public class RemoteFile : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RemoteFile));

        public static async Task<RemoteFile> Create(Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            return (RemoteFile)await RemoteFile.Create((string)parameters["path"], parameters, options);
        }

        public static Task<RemoteFile> DownloadFile(string path, string localPath = null, Dictionary<string, object> options = null)
        {
            return DownloadFileCore(FilesClient.Instance, path, localPath, options, CancellationToken.None);
        }

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
        // directory or onto another file.
        private static string DefaultLocalPath(string path)
        {
            string normalizedPath = PathUtil.normalize(path);
            string name = normalizedPath.Substring(normalizedPath.LastIndexOf('/') + 1);
            if (name.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException($"Bad parameter: localPath is required because \"{name}\" is not a valid local file name", "localPath");
            }
            return System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + name;
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
            ((IModel)f).SetContext(client, options);
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

            FileUploadPart[] uploadActions = await BeginUploadCore(context, path, parameters, options, cancellationToken);
            FileUploadPart uploadAction = uploadActions[0];
            Int64 chunkLength = Math.Min(fileLength - offset, (Int64)uploadAction.Partsize);
            System.Net.Http.HttpMethod httpMethod = new System.Net.Http.HttpMethod(uploadAction.HttpMethod);

            await FilesClient.ChunkUpload(context, httpMethod, uploadAction.UploadUri, readStream, chunkLength, cancellationToken);

            return Tuple.Create(chunkLength, uploadAction.Ref);
        }

        public static Task<bool> UploadFile(string localPath, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return UploadFileCore(OperationContext.OfDefaultClient(), localPath, destinationPath, options, parameters, CancellationToken.None);
        }

        internal static async Task<bool> UploadFileCore(OperationContext context, string localPath, string destinationPath, Dictionary<string, object> options, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(localPath);

            if (destinationPath == null)
            {
                destinationPath = localPath.Substring(localPath.LastIndexOf('/') + 1);
            }

            DateTime mTime = fileInfo.LastWriteTimeUtc;
            Int64 fileLength = fileInfo.Length;

            System.IO.Stream readStream = System.IO.File.OpenRead(localPath);
            return await UploadFileCore(context, destinationPath, readStream, fileLength, mTime, options, parameters, cancellationToken);
        }

        public static async Task<bool> UploadToRemoteServer(string localPath, long remoteServerId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(localPath, UnderscoreUploadDestinationPath("RemoteServers", remoteServerId, localPath, destinationPath), options, parameters);
        }

        public static async Task<bool> UploadToRemoteServer(long remoteServerId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath), readStream, fileLength, mTime, options, parameters);
        }

        public static async Task<FileAction> CopyToRemoteServer(string path, long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return await Copy(path, parameters, options);
        }

        public static async Task<FileAction> MoveToRemoteServer(string path, long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return await Move(path, parameters, options);
        }

        public Task<FileAction> CopyToRemoteServer(long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return CopyToRemoteServerAsync(remoteServerId, destinationPath, parameters, CancellationToken.None);
        }

        public Task<FileAction> CopyToRemoteServerAsync(long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return CopyAsync(parameters, cancellationToken);
        }

        public Task<FileAction> MoveToRemoteServer(long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return MoveToRemoteServerAsync(remoteServerId, destinationPath, parameters, CancellationToken.None);
        }

        public Task<FileAction> MoveToRemoteServerAsync(long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return MoveAsync(parameters, cancellationToken);
        }

        public static async Task<bool> UploadToSnapshot(string localPath, long snapshotId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(localPath, UnderscoreUploadDestinationPath("Snapshots", snapshotId, localPath, destinationPath), options, parameters);
        }

        public static async Task<bool> UploadToSnapshot(long snapshotId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath), readStream, fileLength, mTime, options, parameters);
        }

        public static async Task<FileAction> CopyToSnapshot(string path, long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return await Copy(path, parameters, options);
        }

        public static async Task<FileAction> MoveToSnapshot(string path, long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return await Move(path, parameters, options);
        }

        public Task<FileAction> CopyToSnapshot(long snapshotId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return CopyToSnapshotAsync(snapshotId, destinationPath, parameters, CancellationToken.None);
        }

        public Task<FileAction> CopyToSnapshotAsync(long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return CopyAsync(parameters, cancellationToken);
        }

        public Task<FileAction> MoveToSnapshot(long snapshotId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return MoveToSnapshotAsync(snapshotId, destinationPath, parameters, CancellationToken.None);
        }

        public Task<FileAction> MoveToSnapshotAsync(long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return MoveAsync(parameters, cancellationToken);
        }

        public static async Task<bool> UploadToChildSite(string localPath, long siteId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(localPath, UnderscoreUploadDestinationPath("Sites", siteId, localPath, destinationPath), options, parameters);
        }

        public static async Task<bool> UploadToChildSite(long siteId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return await UploadFile(UnderscoreDestinationPath("Sites", siteId, destinationPath), readStream, fileLength, mTime, options, parameters);
        }

        public static async Task<FileAction> CopyToChildSite(string path, long siteId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return await Copy(path, parameters, options);
        }

        public static async Task<FileAction> MoveToChildSite(string path, long siteId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return await Move(path, parameters, options);
        }

        public Task<FileAction> CopyToChildSite(long siteId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return CopyToChildSiteAsync(siteId, destinationPath, parameters, CancellationToken.None);
        }

        public Task<FileAction> CopyToChildSiteAsync(long siteId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return CopyAsync(parameters, cancellationToken);
        }

        public Task<FileAction> MoveToChildSite(long siteId, string destinationPath, Dictionary<string, object> parameters = null)
        {
            return MoveToChildSiteAsync(siteId, destinationPath, parameters, CancellationToken.None);
        }

        public Task<FileAction> MoveToChildSiteAsync(long siteId, string destinationPath, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return MoveAsync(parameters, cancellationToken);
        }


        public static Task<bool> UploadFile(string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            return UploadFileCore(OperationContext.OfDefaultClient(), destinationPath, readStream, fileLength, mTime, options, parameters, CancellationToken.None);
        }

        // Uploads the stream part by part, then finalizes the upload. Every stage runs with context and one copy of
        // options. The upload owns readStream and disposes it when it ends, however it ends.
        internal static async Task<bool> UploadFileCore(OperationContext context, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            bool success = false;

            using (readStream)
            {
                options = DictionaryUtil.Copy(options);
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

                await RemoteFile.CreateCore(context, destinationPath, createParams, options, cancellationToken);
                success = true;
            }
            return success;
        }

        public Task<string> GetDownloadUriWithLoad()
        {
            return GetDownloadUriWithLoadAsync(CancellationToken.None);
        }

        public async Task<string> GetDownloadUriWithLoadAsync(CancellationToken cancellationToken = default)
        {
            if (DownloadUri != null)
            {
                return DownloadUri;
            }
            return await LoadDownloadUri(new OperationContext(FilesClient.Bind(ref client)), DictionaryUtil.Copy(options), cancellationToken);
        }

        // Asks the API for a download URI, unless this file already has one.
        private async Task<string> LoadDownloadUri(OperationContext context, Dictionary<string, object> requestOptions, CancellationToken cancellationToken)
        {
            if (DownloadUri == null)
            {
                RemoteFile f = await RemoteFile.DownloadCore(context, Path, null, requestOptions, cancellationToken);
                attributes = f.attributes;
            }
            return DownloadUri;
        }

        public Task DownloadFile(string outputFile)
        {
            return DownloadFileAsync(outputFile, CancellationToken.None);
        }

        /// <summary>
        /// Downloads the file to <paramref name="outputFile"/>, creating or replacing it.
        /// </summary>
        /// <remarks>
        /// The local file is closed when the task ends, whether the download succeeds, fails or is cancelled. After a
        /// failure or cancellation it may be left partly written.
        /// </remarks>
        public async Task DownloadFileAsync(string outputFile, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            System.IO.FileStream fileStream = new System.IO.FileStream(outputFile, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
            try
            {
                await DownloadFileAsync(fileStream, cancellationToken);
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

        public Task DownloadFile(System.IO.Stream writeStream)
        {
            return DownloadFileAsync(writeStream, CancellationToken.None);
        }

        /// <summary>
        /// Downloads the file into <paramref name="writeStream"/>, from the stream's current position.
        /// </summary>
        /// <remarks>
        /// The stream is borrowed: it is left open, including when the download fails or is cancelled. The file's
        /// details, if a download URI must be requested, and its contents come from this file's client.
        /// </remarks>
        public async Task DownloadFileAsync(System.IO.Stream writeStream, CancellationToken cancellationToken = default)
        {
            try
            {
                OperationContext context = new OperationContext(FilesClient.Bind(ref client));
                string uri = await LoadDownloadUri(context, DictionaryUtil.Copy(options), cancellationToken);
                await FilesClient.StreamDownload(context, uri, writeStream, cancellationToken);
            }
            catch (Exception e) when (!FilesClient.IsRequestedCancellation(e, cancellationToken))
            {
                log.Error($"DownloadFile failed for {Path}: {e.ToString()}");
                throw;
            }
        }
        public RemoteFile() : this(null, null) { }

        public RemoteFile(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;

            if (this.attributes == null)
            {
                this.attributes = new Dictionary<string, object>();
            }

            if (this.options == null)
            {
                this.options = new Dictionary<string, object>();
            }

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

        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        public object GetOption(string name)
        {
            return (this.options.ContainsKey(name) ? this.options[name] : null);
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options)
        {
            this.client = client;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[] { DirectConnectionInfo, Preview }; }
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// File/Folder path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// User ID of the User who created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_id")]
        public Nullable<Int64> CreatedById
        {
            get { return (Nullable<Int64>)attributes["created_by_id"]; }
            set { attributes["created_by_id"] = value; }
        }

        /// <summary>
        /// ID of the API key that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_api_key_id")]
        public Nullable<Int64> CreatedByApiKeyId
        {
            get { return (Nullable<Int64>)attributes["created_by_api_key_id"]; }
            set { attributes["created_by_api_key_id"] = value; }
        }

        /// <summary>
        /// ID of the AS2 Incoming Message that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_as2_incoming_message_id")]
        public Nullable<Int64> CreatedByAs2IncomingMessageId
        {
            get { return (Nullable<Int64>)attributes["created_by_as2_incoming_message_id"]; }
            set { attributes["created_by_as2_incoming_message_id"] = value; }
        }

        /// <summary>
        /// ID of the Automation that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_automation_id")]
        public Nullable<Int64> CreatedByAutomationId
        {
            get { return (Nullable<Int64>)attributes["created_by_automation_id"]; }
            set { attributes["created_by_automation_id"] = value; }
        }

        /// <summary>
        /// ID of the Bundle Registration that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_bundle_registration_id")]
        public Nullable<Int64> CreatedByBundleRegistrationId
        {
            get { return (Nullable<Int64>)attributes["created_by_bundle_registration_id"]; }
            set { attributes["created_by_bundle_registration_id"] = value; }
        }

        /// <summary>
        /// ID of the Inbox that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_inbox_id")]
        public Nullable<Int64> CreatedByInboxId
        {
            get { return (Nullable<Int64>)attributes["created_by_inbox_id"]; }
            set { attributes["created_by_inbox_id"] = value; }
        }

        /// <summary>
        /// ID of the Remote Server that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_remote_server_id")]
        public Nullable<Int64> CreatedByRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["created_by_remote_server_id"]; }
            set { attributes["created_by_remote_server_id"] = value; }
        }

        /// <summary>
        /// ID of the Sync that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_sync_id")]
        public Nullable<Int64> CreatedBySyncId
        {
            get { return (Nullable<Int64>)attributes["created_by_sync_id"]; }
            set { attributes["created_by_sync_id"] = value; }
        }

        /// <summary>
        /// Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
        /// </summary>
        [JsonPropertyName("custom_metadata")]
        public object CustomMetadata
        {
            get { return (object)attributes["custom_metadata"]; }
            set { attributes["custom_metadata"] = value; }
        }

        /// <summary>
        /// File/Folder display name
        /// </summary>
        [JsonPropertyName("display_name")]
        public string DisplayName
        {
            get { return (string)attributes["display_name"]; }
            set { attributes["display_name"] = value; }
        }

        /// <summary>
        /// Type: `directory` or `file`.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string)attributes["type"]; }
            set { attributes["type"] = value; }
        }

        /// <summary>
        /// File/Folder size
        /// </summary>
        [JsonPropertyName("size")]
        public Nullable<Int64> Size
        {
            get { return (Nullable<Int64>)attributes["size"]; }
            set { attributes["size"] = value; }
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
            set { attributes["last_modified_by_id"] = value; }
        }

        /// <summary>
        /// ID of the API key that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_api_key_id")]
        public Nullable<Int64> LastModifiedByApiKeyId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_api_key_id"]; }
            set { attributes["last_modified_by_api_key_id"] = value; }
        }

        /// <summary>
        /// ID of the Automation that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_automation_id")]
        public Nullable<Int64> LastModifiedByAutomationId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_automation_id"]; }
            set { attributes["last_modified_by_automation_id"] = value; }
        }

        /// <summary>
        /// ID of the Bundle Registration that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_bundle_registration_id")]
        public Nullable<Int64> LastModifiedByBundleRegistrationId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_bundle_registration_id"]; }
            set { attributes["last_modified_by_bundle_registration_id"] = value; }
        }

        /// <summary>
        /// ID of the Remote Server that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_remote_server_id")]
        public Nullable<Int64> LastModifiedByRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_remote_server_id"]; }
            set { attributes["last_modified_by_remote_server_id"] = value; }
        }

        /// <summary>
        /// ID of the Sync that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_sync_id")]
        public Nullable<Int64> LastModifiedBySyncId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_sync_id"]; }
            set { attributes["last_modified_by_sync_id"] = value; }
        }

        /// <summary>
        /// File last modified date/time, according to the server.  This is the timestamp of the last Files.com operation of the file, regardless of what modified timestamp was sent.
        /// </summary>
        [JsonPropertyName("mtime")]
        public Nullable<DateTime> Mtime
        {
            get { return (Nullable<DateTime>)attributes["mtime"]; }
            set { attributes["mtime"] = value; }
        }

        /// <summary>
        /// File last modified date/time, according to the client who set it.  Files.com allows desktop, FTP, SFTP, and WebDAV clients to set modified at times.  This allows Desktop<->Cloud syncing to preserve modified at times.
        /// </summary>
        [JsonPropertyName("provided_mtime")]
        public Nullable<DateTime> ProvidedMtime
        {
            get { return (Nullable<DateTime>)attributes["provided_mtime"]; }
            set { attributes["provided_mtime"] = value; }
        }

        /// <summary>
        /// File CRC32 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("crc32")]
        public string Crc32
        {
            get { return (string)attributes["crc32"]; }
            set { attributes["crc32"] = value; }
        }

        /// <summary>
        /// File MD5 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("md5")]
        public string Md5
        {
            get { return (string)attributes["md5"]; }
            set { attributes["md5"] = value; }
        }

        /// <summary>
        /// File SHA1 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("sha1")]
        public string Sha1
        {
            get { return (string)attributes["sha1"]; }
            set { attributes["sha1"] = value; }
        }

        /// <summary>
        /// File SHA256 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("sha256")]
        public string Sha256
        {
            get { return (string)attributes["sha256"]; }
            set { attributes["sha256"] = value; }
        }

        /// <summary>
        /// MIME Type.  This is determined by the filename extension and is not stored separately internally.
        /// </summary>
        [JsonPropertyName("mime_type")]
        public string MimeType
        {
            get { return (string)attributes["mime_type"]; }
            set { attributes["mime_type"] = value; }
        }

        /// <summary>
        /// Region location
        /// </summary>
        [JsonPropertyName("region")]
        public string Region
        {
            get { return (string)attributes["region"]; }
            set { attributes["region"] = value; }
        }

        /// <summary>
        /// A short string representing the current user's permissions.  Can be `r` (Read),`w` (Write),`d` (Delete), `l` (List) or any combination
        /// </summary>
        [JsonPropertyName("permissions")]
        public string Permissions
        {
            get { return (string)attributes["permissions"]; }
            set { attributes["permissions"] = value; }
        }

        /// <summary>
        /// Are subfolders locked and unable to be modified?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("subfolders_locked?")]
        public bool SubfoldersLocked
        {
            get { return attributes["subfolders_locked?"] == null ? false : (bool)attributes["subfolders_locked?"]; }
            set { attributes["subfolders_locked?"] = value; }
        }

        /// <summary>
        /// Is this folder locked and unable to be modified?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("is_locked")]
        public bool IsLocked
        {
            get { return attributes["is_locked"] == null ? false : (bool)attributes["is_locked"]; }
            set { attributes["is_locked"] = value; }
        }

        /// <summary>
        /// Link to download file. Provided only in response to a download request.
        /// </summary>
        [JsonPropertyName("download_uri")]
        public string DownloadUri
        {
            get { return (string)attributes["download_uri"]; }
            set { attributes["download_uri"] = value; }
        }

        /// <summary>
        /// Optional direct connection information for direct Agent transfer attempts
        /// </summary>
        [JsonPropertyName("direct_connection_info")]
        public DirectConnectionInfo DirectConnectionInfo
        {
            get { return (DirectConnectionInfo)attributes["direct_connection_info"]; }
            set { attributes["direct_connection_info"] = value; }
        }

        /// <summary>
        /// Bookmark/priority color of file/folder
        /// </summary>
        [JsonPropertyName("priority_color")]
        public string PriorityColor
        {
            get { return (string)attributes["priority_color"]; }
            set { attributes["priority_color"] = value; }
        }

        /// <summary>
        /// File preview ID
        /// </summary>
        [JsonPropertyName("preview_id")]
        public Nullable<Int64> PreviewId
        {
            get { return (Nullable<Int64>)attributes["preview_id"]; }
            set { attributes["preview_id"] = value; }
        }

        /// <summary>
        /// File preview
        /// </summary>
        [JsonPropertyName("preview")]
        public Preview Preview
        {
            get { return (Preview)attributes["preview"]; }
            set { attributes["preview"] = value; }
        }

        /// <summary>
        /// The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            set { attributes["action"] = value; }
        }

        /// <summary>
        /// Length of file.
        /// </summary>
        [JsonPropertyName("length")]
        public Nullable<Int64> Length
        {
            get { return (Nullable<Int64>)attributes["length"]; }
            set { attributes["length"] = value; }
        }

        /// <summary>
        /// Create parent directories if they do not exist?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mkdir_parents")]
        public bool MkdirParents
        {
            get { return attributes["mkdir_parents"] == null ? false : (bool)attributes["mkdir_parents"]; }
            set { attributes["mkdir_parents"] = value; }
        }

        /// <summary>
        /// Part if uploading a part.
        /// </summary>
        [JsonPropertyName("part")]
        public Nullable<Int64> Part
        {
            get { return (Nullable<Int64>)attributes["part"]; }
            set { attributes["part"] = value; }
        }

        /// <summary>
        /// How many parts to fetch?
        /// </summary>
        [JsonPropertyName("parts")]
        public Nullable<Int64> Parts
        {
            get { return (Nullable<Int64>)attributes["parts"]; }
            set { attributes["parts"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonPropertyName("ref")]
        public string Ref
        {
            get { return (string)attributes["ref"]; }
            set { attributes["ref"] = value; }
        }

        /// <summary>
        /// File byte offset to restart from.
        /// </summary>
        [JsonPropertyName("restart")]
        public Nullable<Int64> Restart
        {
            get { return (Nullable<Int64>)attributes["restart"]; }
            set { attributes["restart"] = value; }
        }

        /// <summary>
        /// If copying a folder, also copy supported behaviors, email notification subscriptions, and per-folder branding to the destination folder tree?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("copy_behaviors")]
        public bool CopyBehaviors
        {
            get { return attributes["copy_behaviors"] == null ? false : (bool)attributes["copy_behaviors"]; }
            set { attributes["copy_behaviors"] = value; }
        }

        /// <summary>
        /// If copying folder, copy just the structure?
        /// </summary>
        [JsonPropertyName("structure")]
        public string Structure
        {
            get { return (string)attributes["structure"]; }
            set { attributes["structure"] = value; }
        }

        /// <summary>
        /// Allow file rename instead of overwrite?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("with_rename")]
        public bool WithRename
        {
            get { return attributes["with_rename"] == null ? false : (bool)attributes["with_rename"]; }
            set { attributes["with_rename"] = value; }
        }

        /// <summary>
        /// If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("buffered_upload")]
        public bool BufferedUpload
        {
            get { return attributes["buffered_upload"] == null ? false : (bool)attributes["buffered_upload"]; }
            set { attributes["buffered_upload"] = value; }
        }

        /// <summary>
        /// Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("with_direct_connection_info")]
        public bool WithDirectConnectionInfo
        {
            get { return attributes["with_direct_connection_info"] == null ? false : (bool)attributes["with_direct_connection_info"]; }
            set { attributes["with_direct_connection_info"] = value; }
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
        public Task<RemoteFile> DownloadAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DownloadCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<RemoteFile> DownloadCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("preview_size") && !(parameters["preview_size"] is string))
            {
                throw new ArgumentException("Bad parameter: preview_size must be of type string", "parameters[\"preview_size\"]");
            }
            if (parameters.ContainsKey("with_previews") && !(parameters["with_previews"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_previews must be of type bool", "parameters[\"with_previews\"]");
            }
            if (parameters.ContainsKey("with_priority_color") && !(parameters["with_priority_color"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_priority_color must be of type bool", "parameters[\"with_priority_color\"]");
            }
            if (parameters.ContainsKey("with_direct_connection_info") && !(parameters["with_direct_connection_info"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_direct_connection_info must be of type bool", "parameters[\"with_direct_connection_info\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<RemoteFile>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   custom_metadata - object - Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
        ///   provided_mtime - string - Modified time of file.
        ///   priority_color - string - Priority/Bookmark color of file.
        /// </summary>
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
        public Task<RemoteFile> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<RemoteFile> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("custom_metadata") && !(parameters["custom_metadata"] is object))
            {
                throw new ArgumentException("Bad parameter: custom_metadata must be of type object", "parameters[\"custom_metadata\"]");
            }
            if (parameters.ContainsKey("provided_mtime") && !(parameters["provided_mtime"] is string))
            {
                throw new ArgumentException("Bad parameter: provided_mtime must be of type string", "parameters[\"provided_mtime\"]");
            }
            if (parameters.ContainsKey("priority_color") && !(parameters["priority_color"] is string))
            {
                throw new ArgumentException("Bad parameter: priority_color must be of type string", "parameters[\"priority_color\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<RemoteFile>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            response.Dispose();
        }

        /// <summary>
        /// List the contents of a ZIP file
        /// </summary>
        public Task<ZipListEntry[]> ZipListContents(Dictionary<string, object> parameters)
        {
            return ZipListContentsCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// List the contents of a ZIP file
        /// </summary>
        public Task<ZipListEntry[]> ZipListContentsAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ZipListContentsCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ZipListEntry[]> ZipListContentsCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/zip_list/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ZipListEntry[]>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
        public Task<FileAction> CopyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return CopyCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> CopyCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("copy_behaviors") && !(parameters["copy_behaviors"] is bool))
            {
                throw new ArgumentException("Bad parameter: copy_behaviors must be of type bool", "parameters[\"copy_behaviors\"]");
            }
            if (parameters.ContainsKey("structure") && !(parameters["structure"] is bool))
            {
                throw new ArgumentException("Bad parameter: structure must be of type bool", "parameters[\"structure\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/copy/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Move File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
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
        public Task<FileAction> MoveAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return MoveCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> MoveCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/move/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
        public Task<FileAction> TransformAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return TransformCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> TransformCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (!parameters.ContainsKey("transform_type") || parameters["transform_type"] == null)
            {
                throw new ArgumentNullException("Parameter missing: transform_type", "parameters[\"transform_type\"]");
            }
            if (!parameters.ContainsKey("target_format") || parameters["target_format"] == null)
            {
                throw new ArgumentNullException("Parameter missing: target_format", "parameters[\"target_format\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("transform_type") && !(parameters["transform_type"] is string))
            {
                throw new ArgumentException("Bad parameter: transform_type must be of type string", "parameters[\"transform_type\"]");
            }
            if (parameters.ContainsKey("target_format") && !(parameters["target_format"] is string))
            {
                throw new ArgumentException("Bad parameter: target_format must be of type string", "parameters[\"target_format\"]");
            }
            if (parameters.ContainsKey("script") && !(parameters["script"] is string))
            {
                throw new ArgumentException("Bad parameter: script must be of type string", "parameters[\"script\"]");
            }
            if (parameters.ContainsKey("width") && !(parameters["width"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: width must be of type Nullable<Int64>", "parameters[\"width\"]");
            }
            if (parameters.ContainsKey("height") && !(parameters["height"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: height must be of type Nullable<Int64>", "parameters[\"height\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/transform/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
        public Task<FileAction> GpgDecryptAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return GpgDecryptCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> GpgDecryptCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("gpg_key_ids") && !(parameters["gpg_key_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: gpg_key_ids must be of type Nullable<Int64>[]", "parameters[\"gpg_key_ids\"]");
            }
            if (parameters.ContainsKey("gpg_key_partner_id") && !(parameters["gpg_key_partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: gpg_key_partner_id must be of type Nullable<Int64>", "parameters[\"gpg_key_partner_id\"]");
            }
            if (parameters.ContainsKey("use_all_private_keys") && !(parameters["use_all_private_keys"] is bool))
            {
                throw new ArgumentException("Bad parameter: use_all_private_keys must be of type bool", "parameters[\"use_all_private_keys\"]");
            }
            if (parameters.ContainsKey("ignore_mdc_error") && !(parameters["ignore_mdc_error"] is bool))
            {
                throw new ArgumentException("Bad parameter: ignore_mdc_error must be of type bool", "parameters[\"ignore_mdc_error\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/gpg_decrypt/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
        public Task<FileAction> GpgEncryptAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return GpgEncryptCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> GpgEncryptCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("gpg_key_ids") && !(parameters["gpg_key_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: gpg_key_ids must be of type Nullable<Int64>[]", "parameters[\"gpg_key_ids\"]");
            }
            if (parameters.ContainsKey("gpg_key_partner_id") && !(parameters["gpg_key_partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: gpg_key_partner_id must be of type Nullable<Int64>", "parameters[\"gpg_key_partner_id\"]");
            }
            if (parameters.ContainsKey("signing_key_id") && !(parameters["signing_key_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: signing_key_id must be of type Nullable<Int64>", "parameters[\"signing_key_id\"]");
            }
            if (parameters.ContainsKey("armor") && !(parameters["armor"] is bool))
            {
                throw new ArgumentException("Bad parameter: armor must be of type bool", "parameters[\"armor\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/gpg_encrypt/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Extract a ZIP file to a destination folder
        ///
        /// Parameters:
        ///   destination (required) - string - Destination folder path for extracted files.
        ///   filename - string - Optional single entry filename to extract.
        ///   overwrite - boolean - Overwrite existing files in the destination?
        /// </summary>
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
        public Task<FileAction> UnzipAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UnzipCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileAction> UnzipCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("filename") && !(parameters["filename"] is string))
            {
                throw new ArgumentException("Bad parameter: filename must be of type string", "parameters[\"filename\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/unzip", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
        public Task<FileUploadPart[]> BeginUploadAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return BeginUploadCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FileUploadPart[]> BeginUploadCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("mkdir_parents") && !(parameters["mkdir_parents"] is bool))
            {
                throw new ArgumentException("Bad parameter: mkdir_parents must be of type bool", "parameters[\"mkdir_parents\"]");
            }
            if (parameters.ContainsKey("part") && !(parameters["part"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: part must be of type Nullable<Int64>", "parameters[\"part\"]");
            }
            if (parameters.ContainsKey("parts") && !(parameters["parts"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: parts must be of type Nullable<Int64>", "parameters[\"parts\"]");
            }
            if (parameters.ContainsKey("ref") && !(parameters["ref"] is string))
            {
                throw new ArgumentException("Bad parameter: ref must be of type string", "parameters[\"ref\"]");
            }
            if (parameters.ContainsKey("restart") && !(parameters["restart"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: restart must be of type Nullable<Int64>", "parameters[\"restart\"]");
            }
            if (parameters.ContainsKey("size") && !(parameters["size"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: size must be of type Nullable<Int64>", "parameters[\"size\"]");
            }
            if (parameters.ContainsKey("with_rename") && !(parameters["with_rename"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_rename must be of type bool", "parameters[\"with_rename\"]");
            }
            if (parameters.ContainsKey("buffered_upload") && !(parameters["buffered_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: buffered_upload must be of type bool", "parameters[\"buffered_upload\"]");
            }
            if (parameters.ContainsKey("with_direct_connection_info") && !(parameters["with_direct_connection_info"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_direct_connection_info must be of type bool", "parameters[\"with_direct_connection_info\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/begin_upload/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileUploadPart[]>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            var newObj = await RemoteFile.CreateCore(new OperationContext(FilesClient.Bind(ref client)), Path, this.attributes, DictionaryUtil.Copy(this.options), cancellationToken);
            this.attributes = newObj.getAttributes();
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("preview_size") && !(parameters["preview_size"] is string))
            {
                throw new ArgumentException("Bad parameter: preview_size must be of type string", "parameters[\"preview_size\"]");
            }
            if (parameters.ContainsKey("with_previews") && !(parameters["with_previews"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_previews must be of type bool", "parameters[\"with_previews\"]");
            }
            if (parameters.ContainsKey("with_priority_color") && !(parameters["with_priority_color"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_priority_color must be of type bool", "parameters[\"with_priority_color\"]");
            }
            if (parameters.ContainsKey("with_direct_connection_info") && !(parameters["with_direct_connection_info"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_direct_connection_info must be of type bool", "parameters[\"with_direct_connection_info\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<RemoteFile>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("custom_metadata") && !(parameters["custom_metadata"] is object))
            {
                throw new ArgumentException("Bad parameter: custom_metadata must be of type object", "parameters[\"custom_metadata\"]");
            }
            if (parameters.ContainsKey("length") && !(parameters["length"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: length must be of type Nullable<Int64>", "parameters[\"length\"]");
            }
            if (parameters.ContainsKey("mkdir_parents") && !(parameters["mkdir_parents"] is bool))
            {
                throw new ArgumentException("Bad parameter: mkdir_parents must be of type bool", "parameters[\"mkdir_parents\"]");
            }
            if (parameters.ContainsKey("part") && !(parameters["part"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: part must be of type Nullable<Int64>", "parameters[\"part\"]");
            }
            if (parameters.ContainsKey("parts") && !(parameters["parts"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: parts must be of type Nullable<Int64>", "parameters[\"parts\"]");
            }
            if (parameters.ContainsKey("provided_mtime") && !(parameters["provided_mtime"] is string))
            {
                throw new ArgumentException("Bad parameter: provided_mtime must be of type string", "parameters[\"provided_mtime\"]");
            }
            if (parameters.ContainsKey("ref") && !(parameters["ref"] is string))
            {
                throw new ArgumentException("Bad parameter: ref must be of type string", "parameters[\"ref\"]");
            }
            if (parameters.ContainsKey("restart") && !(parameters["restart"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: restart must be of type Nullable<Int64>", "parameters[\"restart\"]");
            }
            if (parameters.ContainsKey("size") && !(parameters["size"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: size must be of type Nullable<Int64>", "parameters[\"size\"]");
            }
            if (parameters.ContainsKey("copy_behaviors") && !(parameters["copy_behaviors"] is bool))
            {
                throw new ArgumentException("Bad parameter: copy_behaviors must be of type bool", "parameters[\"copy_behaviors\"]");
            }
            if (parameters.ContainsKey("structure") && !(parameters["structure"] is string))
            {
                throw new ArgumentException("Bad parameter: structure must be of type string", "parameters[\"structure\"]");
            }
            if (parameters.ContainsKey("with_rename") && !(parameters["with_rename"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_rename must be of type bool", "parameters[\"with_rename\"]");
            }
            if (parameters.ContainsKey("buffered_upload") && !(parameters["buffered_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: buffered_upload must be of type bool", "parameters[\"buffered_upload\"]");
            }
            if (parameters.ContainsKey("with_direct_connection_info") && !(parameters["with_direct_connection_info"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_direct_connection_info must be of type bool", "parameters[\"with_direct_connection_info\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<RemoteFile>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   custom_metadata - object - Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
        ///   provided_mtime - string - Modified time of file.
        ///   priority_color - string - Priority/Bookmark color of file.
        /// </summary>
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("custom_metadata") && !(parameters["custom_metadata"] is object))
            {
                throw new ArgumentException("Bad parameter: custom_metadata must be of type object", "parameters[\"custom_metadata\"]");
            }
            if (parameters.ContainsKey("provided_mtime") && !(parameters["provided_mtime"] is string))
            {
                throw new ArgumentException("Bad parameter: provided_mtime must be of type string", "parameters[\"provided_mtime\"]");
            }
            if (parameters.ContainsKey("priority_color") && !(parameters["priority_color"] is string))
            {
                throw new ArgumentException("Bad parameter: priority_color must be of type string", "parameters[\"priority_color\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<RemoteFile>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        public static Task Delete(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
            response.Dispose();
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        public static Task<RemoteFile> Find(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("preview_size") && !(parameters["preview_size"] is string))
            {
                throw new ArgumentException("Bad parameter: preview_size must be of type string", "parameters[\"preview_size\"]");
            }
            if (parameters.ContainsKey("with_previews") && !(parameters["with_previews"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_previews must be of type bool", "parameters[\"with_previews\"]");
            }
            if (parameters.ContainsKey("with_priority_color") && !(parameters["with_priority_color"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_priority_color must be of type bool", "parameters[\"with_priority_color\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/metadata/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<RemoteFile>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// List the contents of a ZIP file
        /// </summary>
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/zip_list/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ZipListEntry[]>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("copy_behaviors") && !(parameters["copy_behaviors"] is bool))
            {
                throw new ArgumentException("Bad parameter: copy_behaviors must be of type bool", "parameters[\"copy_behaviors\"]");
            }
            if (parameters.ContainsKey("structure") && !(parameters["structure"] is bool))
            {
                throw new ArgumentException("Bad parameter: structure must be of type bool", "parameters[\"structure\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/copy/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Move File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/move/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (!parameters.ContainsKey("transform_type") || parameters["transform_type"] == null)
            {
                throw new ArgumentNullException("Parameter missing: transform_type", "parameters[\"transform_type\"]");
            }
            if (!parameters.ContainsKey("target_format") || parameters["target_format"] == null)
            {
                throw new ArgumentNullException("Parameter missing: target_format", "parameters[\"target_format\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("transform_type") && !(parameters["transform_type"] is string))
            {
                throw new ArgumentException("Bad parameter: transform_type must be of type string", "parameters[\"transform_type\"]");
            }
            if (parameters.ContainsKey("target_format") && !(parameters["target_format"] is string))
            {
                throw new ArgumentException("Bad parameter: target_format must be of type string", "parameters[\"target_format\"]");
            }
            if (parameters.ContainsKey("script") && !(parameters["script"] is string))
            {
                throw new ArgumentException("Bad parameter: script must be of type string", "parameters[\"script\"]");
            }
            if (parameters.ContainsKey("width") && !(parameters["width"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: width must be of type Nullable<Int64>", "parameters[\"width\"]");
            }
            if (parameters.ContainsKey("height") && !(parameters["height"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: height must be of type Nullable<Int64>", "parameters[\"height\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/transform/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("gpg_key_ids") && !(parameters["gpg_key_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: gpg_key_ids must be of type Nullable<Int64>[]", "parameters[\"gpg_key_ids\"]");
            }
            if (parameters.ContainsKey("gpg_key_partner_id") && !(parameters["gpg_key_partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: gpg_key_partner_id must be of type Nullable<Int64>", "parameters[\"gpg_key_partner_id\"]");
            }
            if (parameters.ContainsKey("use_all_private_keys") && !(parameters["use_all_private_keys"] is bool))
            {
                throw new ArgumentException("Bad parameter: use_all_private_keys must be of type bool", "parameters[\"use_all_private_keys\"]");
            }
            if (parameters.ContainsKey("ignore_mdc_error") && !(parameters["ignore_mdc_error"] is bool))
            {
                throw new ArgumentException("Bad parameter: ignore_mdc_error must be of type bool", "parameters[\"ignore_mdc_error\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/gpg_decrypt/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("gpg_key_ids") && !(parameters["gpg_key_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: gpg_key_ids must be of type Nullable<Int64>[]", "parameters[\"gpg_key_ids\"]");
            }
            if (parameters.ContainsKey("gpg_key_partner_id") && !(parameters["gpg_key_partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: gpg_key_partner_id must be of type Nullable<Int64>", "parameters[\"gpg_key_partner_id\"]");
            }
            if (parameters.ContainsKey("signing_key_id") && !(parameters["signing_key_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: signing_key_id must be of type Nullable<Int64>", "parameters[\"signing_key_id\"]");
            }
            if (parameters.ContainsKey("armor") && !(parameters["armor"] is bool))
            {
                throw new ArgumentException("Bad parameter: armor must be of type bool", "parameters[\"armor\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/gpg_encrypt/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Extract a ZIP file to a destination folder
        ///
        /// Parameters:
        ///   destination (required) - string - Destination folder path for extracted files.
        ///   filename - string - Optional single entry filename to extract.
        ///   overwrite - boolean - Overwrite existing files in the destination?
        /// </summary>
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("filename") && !(parameters["filename"] is string))
            {
                throw new ArgumentException("Bad parameter: filename must be of type string", "parameters[\"filename\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/unzip", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   paths (required) - array(string) - Paths to include in the ZIP.
        ///   destination (required) - string - Destination file path for the ZIP.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
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

            if (!parameters.ContainsKey("paths") || parameters["paths"] == null)
            {
                throw new ArgumentNullException("Parameter missing: paths", "parameters[\"paths\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("paths") && !(parameters["paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: paths must be of type string[]", "parameters[\"paths\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/zip", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileAction>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("mkdir_parents") && !(parameters["mkdir_parents"] is bool))
            {
                throw new ArgumentException("Bad parameter: mkdir_parents must be of type bool", "parameters[\"mkdir_parents\"]");
            }
            if (parameters.ContainsKey("part") && !(parameters["part"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: part must be of type Nullable<Int64>", "parameters[\"part\"]");
            }
            if (parameters.ContainsKey("parts") && !(parameters["parts"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: parts must be of type Nullable<Int64>", "parameters[\"parts\"]");
            }
            if (parameters.ContainsKey("ref") && !(parameters["ref"] is string))
            {
                throw new ArgumentException("Bad parameter: ref must be of type string", "parameters[\"ref\"]");
            }
            if (parameters.ContainsKey("restart") && !(parameters["restart"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: restart must be of type Nullable<Int64>", "parameters[\"restart\"]");
            }
            if (parameters.ContainsKey("size") && !(parameters["size"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: size must be of type Nullable<Int64>", "parameters[\"size\"]");
            }
            if (parameters.ContainsKey("with_rename") && !(parameters["with_rename"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_rename must be of type bool", "parameters[\"with_rename\"]");
            }
            if (parameters.ContainsKey("buffered_upload") && !(parameters["buffered_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: buffered_upload must be of type bool", "parameters[\"buffered_upload\"]");
            }
            if (parameters.ContainsKey("with_direct_connection_info") && !(parameters["with_direct_connection_info"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_direct_connection_info must be of type bool", "parameters[\"with_direct_connection_info\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/file_actions/begin_upload/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<FileUploadPart[]>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

    }
}