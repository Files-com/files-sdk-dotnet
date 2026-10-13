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
    public class Folder : IModel, IJsonOnDeserialized
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
        /// <summary>
        /// Makes a Folder with no attributes set and no options.
        /// </summary>
        public Folder() : this(null, null) { }

        /// <summary>
        /// Makes a Folder with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Folder(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("mkdir_parents"))
            {
                this.attributes.Add("mkdir_parents", false);
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
        /// Create parent directories if they do not exist?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mkdir_parents")]
        public bool MkdirParents
        {
            get { return attributes["mkdir_parents"] == null ? false : (bool)attributes["mkdir_parents"]; }
            set { attributes["mkdir_parents"] = value; unsaved.Add("mkdir_parents"); }
        }


        private static readonly string[] CreateParameterNames = { "path", "mkdir_parents", "provided_mtime" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Folder on Files.com.
        /// It sends the attributes set on this object, in its constructor or with its properties, or changed in place,
        /// such as an element of an array, since it was made or last came from Files.com, of those that operation
        /// takes; it sends no other attributes, and no defaults. Each goes in the form its parameter takes, such as a
        /// DateTime as ISO 8601 text. When it succeeds, this object takes the attributes Files.com returns.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            RemoteFile created = await Folder.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), Path, AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        /// Parameters:
        ///   cursor - string - Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   sort_by - object - Search by field and direction. Valid fields are `path`, `size`, `modified_at_datetime`, `provided_modified_at`.  Valid directions are `asc` and `desc`.  Defaults to `{"path":"asc"}`.
        ///   search - string - If specified, will search the folders/files list by name. Ignores text before last `/`. This is the same API used by the search bar in the web UI when running 'Search This Folder'.  Search results are a best effort, not real time, and not guaranteed to perfectly match the latest folder listing.  Results may be truncated if more than 1,000 possible matches exist.  This field should only be used for ad-hoc (human) searching, and not as part of an automated process.
        ///   search_custom_metadata_key - string - If provided, the search string in `search` will search for files where this custom metadata key matches the value sent in `search`.  Set this to `*` to allow any metadata key to match the value sent in `search`.
        ///   search_all - boolean - Search entire site?  If true, we will search the entire site.  Do not provide a path when using this parameter.  This is the same API used by the search bar in the web UI when running 'Search All Files'.  Search results are a best effort, not real time, and not guaranteed to match every file.  This field should only be used for ad-hoc (human) searching, and not as part of an automated process.
        ///   with_previews - boolean - Include file previews?
        ///   with_priority_color - boolean - Include file priority color information?
        ///   type - string - Type of objects to return.  Can be `folder` or `file`.
        ///   modified_at_datetime - string - If provided, will only return files/folders modified after this time. Can be used only in combination with `type` filter.
        /// </summary>
        /// <param name="path">Path to operate on.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<RemoteFile> ListFor(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListForCore(FilesClient.Instance, path, parameters, options);
        }

        internal static FilesList<RemoteFile> ListForCore(
            FilesClient client,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options
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
            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("preview_size", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: preview_size is null, which cannot be sent" : "Bad parameter: preview_size must be of type string", "preview_size");
            }
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("search", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: search is null, which cannot be sent" : "Bad parameter: search must be of type string", "search");
            }
            if (parameters.TryGetValue("search_custom_metadata_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: search_custom_metadata_key is null, which cannot be sent" : "Bad parameter: search_custom_metadata_key must be of type string", "search_custom_metadata_key");
            }
            if (parameters.TryGetValue("search_all", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: search_all is null, which cannot be sent" : "Bad parameter: search_all must be of type bool", "search_all");
            }
            if (parameters.TryGetValue("with_previews", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_previews is null, which cannot be sent" : "Bad parameter: with_previews must be of type bool", "with_previews");
            }
            if (parameters.TryGetValue("with_priority_color", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: with_priority_color is null, which cannot be sent" : "Bad parameter: with_priority_color must be of type bool", "with_priority_color");
            }
            if (parameters.TryGetValue("type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: type is null, which cannot be sent" : "Bad parameter: type must be of type string", "type");
            }
            if (parameters.TryGetValue("modified_at_datetime", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: modified_at_datetime is null, which cannot be sent" : "Bad parameter: modified_at_datetime must be of type string", "modified_at_datetime");
            }

            return new FilesList<RemoteFile>(client, $"/folders/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   provided_mtime - string - User provided modification time.
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
            if (parameters.TryGetValue("mkdir_parents", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mkdir_parents is null, which cannot be sent" : "Bad parameter: mkdir_parents must be of type bool", "mkdir_parents");
            }
            if (parameters.TryGetValue("provided_mtime", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: provided_mtime is null, which cannot be sent" : "Bad parameter: provided_mtime must be of type string", "provided_mtime");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/folders/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteFile>(responseJson, context, options);
        }

    }
}