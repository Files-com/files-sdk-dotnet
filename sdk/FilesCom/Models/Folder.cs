using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Folder
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Folder() : this(null, null) { }

        public Folder(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
        /// Create parent directories if they do not exist?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mkdir_parents")]
        public bool MkdirParents
        {
            get { return attributes["mkdir_parents"] == null ? false : (bool)attributes["mkdir_parents"]; }
            set { attributes["mkdir_parents"] = value; }
        }


        public async Task Save()
        {
            var newObj = await Folder.Create(Path, this.attributes, this.options);
            this.attributes = newObj.getAttributes();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   sort_by - object - Search by field and direction. Valid fields are `path`, `size`, `modified_at_datetime`, `provided_modified_at`.  Valid directions are `asc` and `desc`.  Defaults to `{"path":"asc"}`.
        ///   search - string - If specified, will search the folders/files list by name. Ignores text before last `/`. This is the same API used by the search bar in the web UI when running 'Search This Folder'.  Search results are a best effort, not real time, and not guaranteed to perfectly match the latest folder listing.  Results may be truncated if more than 1,000 possible matches exist.  This field should only be used for ad-hoc (human) searching, and not as part of an automated process.
        ///   search_custom_metadata_key - string - If provided, the search string in `search` will search for files where this custom metadata key matches the value sent in `search`.  Set this to `*` to allow any metadata key to match the value sent in `search`.
        ///   search_all - boolean - Search entire site?  If set, we will ignore the folder path provided and search the entire site.  This is the same API used by the search bar in the web UI when running 'Search All Files'.  Search results are a best effort, not real time, and not guaranteed to match every file.  This field should only be used for ad-hoc (human) searching, and not as part of an automated process.
        ///   with_previews - boolean - Include file previews?
        ///   with_priority_color - boolean - Include file priority color information?
        ///   type - string - Type of objects to return.  Can be `folder` or `file`.
        ///   modified_at_datetime - string - If provided, will only return files/folders modified after this time. Can be used only in combination with `type` filter.
        /// </summary>
        public static FilesList<RemoteFile> ListFor(
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
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("preview_size") && !(parameters["preview_size"] is string))
            {
                throw new ArgumentException("Bad parameter: preview_size must be of type string", "parameters[\"preview_size\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("search") && !(parameters["search"] is string))
            {
                throw new ArgumentException("Bad parameter: search must be of type string", "parameters[\"search\"]");
            }
            if (parameters.ContainsKey("search_custom_metadata_key") && !(parameters["search_custom_metadata_key"] is string))
            {
                throw new ArgumentException("Bad parameter: search_custom_metadata_key must be of type string", "parameters[\"search_custom_metadata_key\"]");
            }
            if (parameters.ContainsKey("search_all") && !(parameters["search_all"] is bool))
            {
                throw new ArgumentException("Bad parameter: search_all must be of type bool", "parameters[\"search_all\"]");
            }
            if (parameters.ContainsKey("with_previews") && !(parameters["with_previews"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_previews must be of type bool", "parameters[\"with_previews\"]");
            }
            if (parameters.ContainsKey("with_priority_color") && !(parameters["with_priority_color"] is bool))
            {
                throw new ArgumentException("Bad parameter: with_priority_color must be of type bool", "parameters[\"with_priority_color\"]");
            }
            if (parameters.ContainsKey("type") && !(parameters["type"] is string))
            {
                throw new ArgumentException("Bad parameter: type must be of type string", "parameters[\"type\"]");
            }
            if (parameters.ContainsKey("modified_at_datetime") && !(parameters["modified_at_datetime"] is string))
            {
                throw new ArgumentException("Bad parameter: modified_at_datetime must be of type string", "parameters[\"modified_at_datetime\"]");
            }

            return new FilesList<RemoteFile>($"/folders/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   provided_mtime - string - User provided modification time.
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
            if (parameters.ContainsKey("mkdir_parents") && !(parameters["mkdir_parents"] is bool))
            {
                throw new ArgumentException("Bad parameter: mkdir_parents must be of type bool", "parameters[\"mkdir_parents\"]");
            }
            if (parameters.ContainsKey("provided_mtime") && !(parameters["provided_mtime"] is string))
            {
                throw new ArgumentException("Bad parameter: provided_mtime must be of type string", "parameters[\"provided_mtime\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/folders/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteFile>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}