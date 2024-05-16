using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class RemoteFile
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RemoteFile));

        public static async Task<RemoteFile> Create(Dictionary<string, object> parameters = null, Dictionary<string, object> options = null)
        {
            return (RemoteFile)await RemoteFile.Create((string)parameters["path"], parameters, options);
        }

        public static async Task<RemoteFile> DownloadFile(string path, string localPath = null, Dictionary<string, object> options = null)
        {
            localPath = localPath != null ? localPath : (System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + path.Substring(path.LastIndexOf('/') + 1));
            RemoteFile f = new RemoteFile(null, options);
            f.Path = path;
            await f.DownloadFile(localPath);
            return f;
        }

        public static async Task<RemoteFile> DownloadFile(string path, System.IO.Stream stream, Dictionary<string, object> options = null)
        {
            RemoteFile f = new RemoteFile(null, options);
            f.Path = path;
            await f.DownloadFile(stream);
            return f;
        }

        private static async Task<Tuple<Int64, string>> UploadChunk(string path, System.IO.Stream readStream, string fileRef, Int64 partNumber, Int64 offset, Int64 fileLength, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
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

            FileUploadPart[] uploadActions = await BeginUpload(path, parameters, options);
            FileUploadPart uploadAction = uploadActions[0];
            Int64 chunkLength = Math.Min(fileLength - offset, (Int64)uploadAction.Partsize);
            System.Net.Http.HttpMethod httpMethod = new System.Net.Http.HttpMethod(uploadAction.HttpMethod);

            await FilesClient.ChunkUpload(httpMethod, uploadAction.UploadUri, readStream, chunkLength);

            return Tuple.Create(chunkLength, uploadAction.Ref);
        }

        public static async Task<bool> UploadFile(string localPath, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(localPath);

            if (destinationPath == null)
            {
                destinationPath = localPath.Substring(localPath.LastIndexOf('/') + 1);
            }

            DateTime mTime = fileInfo.LastWriteTimeUtc;
            Int64 fileLength = fileInfo.Length;

            System.IO.Stream readStream = System.IO.File.OpenRead(localPath);
            return await UploadFile(destinationPath, readStream, fileLength, mTime, options, parameters);
        }

        public static async Task<bool> UploadFile(string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null)
        {
            bool success = false;

            using (readStream)
            {
                Int64 parts = 0;
                Int64 bytesWritten = 0;
                string fileRef = null;

                // TODO: Set up multiple parallel streams instead of looping serial uploads here.
                while (bytesWritten < fileLength || parts == 0)
                {
                    parts++;
                    Tuple<Int64, string> result = await UploadChunk(destinationPath, readStream, fileRef, parts, bytesWritten, fileLength, options, parameters);
                    bytesWritten += result.Item1;
                    fileRef = result.Item2;
                }

                Dictionary<string, object> createParams = new Dictionary<string, object>();
                createParams["action"] = "end";
                createParams["provided_mtime"] = mTime.ToString("u");
                createParams["ref"] = fileRef;
                createParams["size"] = fileLength;

                await RemoteFile.Create(destinationPath, createParams, options);
                success = true;
            }
            return success;
        }

        public async Task<string> GetDownloadUriWithLoad()
        {
            if (DownloadUri == null)
            {
                RemoteFile f = (RemoteFile)await RemoteFile.Download(Path, null, options);
                attributes = f.attributes;
            }
            return DownloadUri;
        }

        public async Task DownloadFile(string outputFile)
        {
            System.IO.FileStream fileStream = new System.IO.FileStream(outputFile, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);
            await DownloadFile(fileStream);
        }

        public async Task DownloadFile(System.IO.Stream writeStream)
        {
            try
            {
                string uri = await GetDownloadUriWithLoad();
                await FilesClient.StreamDownload(uri, writeStream);
            }
            catch (Exception e)
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
            if (!this.attributes.ContainsKey("created_by_remote_server_sync_id"))
            {
                this.attributes.Add("created_by_remote_server_sync_id", null);
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
            if (!this.attributes.ContainsKey("last_modified_by_remote_server_sync_id"))
            {
                this.attributes.Add("last_modified_by_remote_server_sync_id", null);
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
            if (!this.attributes.ContainsKey("structure"))
            {
                this.attributes.Add("structure", null);
            }
            if (!this.attributes.ContainsKey("with_rename"))
            {
                this.attributes.Add("with_rename", false);
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

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// File/Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
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
        /// ID of the Remote Server Sync that created the file/folder
        /// </summary>
        [JsonPropertyName("created_by_remote_server_sync_id")]
        public Nullable<Int64> CreatedByRemoteServerSyncId
        {
            get { return (Nullable<Int64>)attributes["created_by_remote_server_sync_id"]; }
            set { attributes["created_by_remote_server_sync_id"] = value; }
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
        /// ID of the Remote Server Sync that last modified the file/folder
        /// </summary>
        [JsonPropertyName("last_modified_by_remote_server_sync_id")]
        public Nullable<Int64> LastModifiedByRemoteServerSyncId
        {
            get { return (Nullable<Int64>)attributes["last_modified_by_remote_server_sync_id"]; }
            set { attributes["last_modified_by_remote_server_sync_id"] = value; }
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
        /// Download file
        ///
        /// Parameters:
        ///   action - string - Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        public async Task<RemoteFile> Download(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendStringRequest($"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteFile>(responseJson);
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
        public async Task<RemoteFile> Update(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendStringRequest($"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteFile>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folers.  Otherwise, will error on non-empty folders.
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/files/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Copy file/folder
        ///
        /// Parameters:
        ///   destination (required) - string - Copy destination path.
        ///   structure - boolean - Copy structure only?
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        public async Task<FileAction> Copy(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("structure") && !(parameters["structure"] is bool))
            {
                throw new ArgumentException("Bad parameter: structure must be of type bool", "parameters[\"structure\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/file_actions/copy/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FileAction>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Move file/folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        public async Task<FileAction> Move(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendStringRequest($"/file_actions/move/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FileAction>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Begin file upload
        ///
        /// Parameters:
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Total bytes of file being uploaded (include bytes being retained if appending/restarting).
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        /// </summary>
        public async Task<FileUploadPart[]> BeginUpload(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendStringRequest($"/file_actions/begin_upload/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FileUploadPart[]>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }



        public async Task Save()
        {
            var newObj = await RemoteFile.Create(Path, this.attributes, this.options);
            this.attributes = newObj.getAttributes();
        }

        /// <summary>
        /// Download file
        ///
        /// Parameters:
        ///   action - string - Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        public static async Task<RemoteFile> Download(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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

            string responseJson = await FilesClient.SendStringRequest($"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteFile>(responseJson);
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
        ///   structure - string - If copying folder, copy just the structure?
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        /// </summary>
        public static async Task<RemoteFile> Create(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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
            if (parameters.ContainsKey("structure") && !(parameters["structure"] is string))
            {
                throw new ArgumentException("Bad parameter: structure must be of type string", "parameters[\"structure\"]");
            }
            if (parameters.ContainsKey("with_rename") && !(parameters["with_rename"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_rename must be of type bool", "parameters[\"with_rename\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteFile>(responseJson);
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
        public static async Task<RemoteFile> Update(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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

            string responseJson = await FilesClient.SendStringRequest($"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteFile>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folers.  Otherwise, will error on non-empty folders.
        /// </summary>
        public static async Task Delete(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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

            await FilesClient.SendRequest($"/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(path, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        public static async Task<RemoteFile> Find(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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

            string responseJson = await FilesClient.SendStringRequest($"/file_actions/metadata/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteFile>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<RemoteFile> Get(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(path, parameters, options);
        }

        /// <summary>
        /// Copy file/folder
        ///
        /// Parameters:
        ///   destination (required) - string - Copy destination path.
        ///   structure - boolean - Copy structure only?
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        public static async Task<FileAction> Copy(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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
            if (parameters.ContainsKey("structure") && !(parameters["structure"] is bool))
            {
                throw new ArgumentException("Bad parameter: structure must be of type bool", "parameters[\"structure\"]");
            }
            if (parameters.ContainsKey("overwrite") && !(parameters["overwrite"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite must be of type bool", "parameters[\"overwrite\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/file_actions/copy/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FileAction>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Move file/folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        public static async Task<FileAction> Move(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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

            string responseJson = await FilesClient.SendStringRequest($"/file_actions/move/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FileAction>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Begin file upload
        ///
        /// Parameters:
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Total bytes of file being uploaded (include bytes being retained if appending/restarting).
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        /// </summary>
        public static async Task<FileUploadPart[]> BeginUpload(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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

            string responseJson = await FilesClient.SendStringRequest($"/file_actions/begin_upload/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FileUploadPart[]>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}