using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class SyncRun
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public SyncRun() : this(null, null) { }

        public SyncRun(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("sync_id"))
            {
                this.attributes.Add("sync_id", null);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("src_remote_server_type"))
            {
                this.attributes.Add("src_remote_server_type", null);
            }
            if (!this.attributes.ContainsKey("dest_remote_server_type"))
            {
                this.attributes.Add("dest_remote_server_type", null);
            }
            if (!this.attributes.ContainsKey("body"))
            {
                this.attributes.Add("body", null);
            }
            if (!this.attributes.ContainsKey("event_errors"))
            {
                this.attributes.Add("event_errors", new string[0]);
            }
            if (!this.attributes.ContainsKey("compared_files"))
            {
                this.attributes.Add("compared_files", null);
            }
            if (!this.attributes.ContainsKey("compared_folders"))
            {
                this.attributes.Add("compared_folders", null);
            }
            if (!this.attributes.ContainsKey("errored_files"))
            {
                this.attributes.Add("errored_files", null);
            }
            if (!this.attributes.ContainsKey("successful_files"))
            {
                this.attributes.Add("successful_files", null);
            }
            if (!this.attributes.ContainsKey("runtime"))
            {
                this.attributes.Add("runtime", null);
            }
            if (!this.attributes.ContainsKey("log_url"))
            {
                this.attributes.Add("log_url", null);
            }
            if (!this.attributes.ContainsKey("completed_at"))
            {
                this.attributes.Add("completed_at", null);
            }
            if (!this.attributes.ContainsKey("dry_run"))
            {
                this.attributes.Add("dry_run", false);
            }
            if (!this.attributes.ContainsKey("bytes_synced"))
            {
                this.attributes.Add("bytes_synced", null);
            }
            if (!this.attributes.ContainsKey("estimated_bytes_count"))
            {
                this.attributes.Add("estimated_bytes_count", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
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
        /// SyncRun ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// ID of the Sync this run belongs to
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_id")]
        public Nullable<Int64> SyncId
        {
            get { return (Nullable<Int64>)attributes["sync_id"]; }
            private set { attributes["sync_id"] = value; }
        }

        /// <summary>
        /// Site ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            private set { attributes["site_id"] = value; }
        }

        /// <summary>
        /// Status of the sync run (success, failure, partial_failure, in_progress, skipped)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Source remote server type, if any
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("src_remote_server_type")]
        public string SrcRemoteServerType
        {
            get { return (string)attributes["src_remote_server_type"]; }
            private set { attributes["src_remote_server_type"] = value; }
        }

        /// <summary>
        /// Destination remote server type, if any
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dest_remote_server_type")]
        public string DestRemoteServerType
        {
            get { return (string)attributes["dest_remote_server_type"]; }
            private set { attributes["dest_remote_server_type"] = value; }
        }

        /// <summary>
        /// Log or summary body for this run
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("body")]
        public string Body
        {
            get { return (string)attributes["body"]; }
            private set { attributes["body"] = value; }
        }

        /// <summary>
        /// Array of errors encountered during the run
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("event_errors")]
        public string[] EventErrors
        {
            get { return (string[])attributes["event_errors"]; }
            private set { attributes["event_errors"] = value; }
        }

        /// <summary>
        /// Number of files compared
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("compared_files")]
        public Nullable<Int64> ComparedFiles
        {
            get { return (Nullable<Int64>)attributes["compared_files"]; }
            private set { attributes["compared_files"] = value; }
        }

        /// <summary>
        /// Number of folders compared
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("compared_folders")]
        public Nullable<Int64> ComparedFolders
        {
            get { return (Nullable<Int64>)attributes["compared_folders"]; }
            private set { attributes["compared_folders"] = value; }
        }

        /// <summary>
        /// Number of files that errored
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("errored_files")]
        public Nullable<Int64> ErroredFiles
        {
            get { return (Nullable<Int64>)attributes["errored_files"]; }
            private set { attributes["errored_files"] = value; }
        }

        /// <summary>
        /// Number of files successfully synced
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("successful_files")]
        public Nullable<Int64> SuccessfulFiles
        {
            get { return (Nullable<Int64>)attributes["successful_files"]; }
            private set { attributes["successful_files"] = value; }
        }

        /// <summary>
        /// Total runtime in seconds
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("runtime")]
        public double Runtime
        {
            get { return (double)attributes["runtime"]; }
            private set { attributes["runtime"] = value; }
        }

        /// <summary>
        /// Link to external log file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("log_url")]
        public string LogUrl
        {
            get { return (string)attributes["log_url"]; }
            private set { attributes["log_url"] = value; }
        }

        /// <summary>
        /// When this run was completed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed_at")]
        public Nullable<DateTime> CompletedAt
        {
            get { return (Nullable<DateTime>)attributes["completed_at"]; }
            private set { attributes["completed_at"] = value; }
        }

        /// <summary>
        /// Whether this run was a dry run (no actual changes made)
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dry_run")]
        public bool DryRun
        {
            get { return attributes["dry_run"] == null ? false : (bool)attributes["dry_run"]; }
            private set { attributes["dry_run"] = value; }
        }

        /// <summary>
        /// Total bytes synced in this run
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes_synced")]
        public Nullable<Int64> BytesSynced
        {
            get { return (Nullable<Int64>)attributes["bytes_synced"]; }
            private set { attributes["bytes_synced"] = value; }
        }

        /// <summary>
        /// Estimated bytes count for this run
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("estimated_bytes_count")]
        public Nullable<Int64> EstimatedBytesCount
        {
            get { return (Nullable<Int64>)attributes["estimated_bytes_count"]; }
            private set { attributes["estimated_bytes_count"] = value; }
        }

        /// <summary>
        /// When this run was created
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// When this run was last updated
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>)attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `sync_id`, `created_at` or `status`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `status`, `dry_run`, `src_remote_server_type`, `dest_remote_server_type` or `sync_id`. Valid field combinations are `[ status, created_at ]`, `[ src_remote_server_type, created_at ]`, `[ dest_remote_server_type, created_at ]`, `[ sync_id, created_at ]`, `[ src_remote_server_type, status ]`, `[ dest_remote_server_type, status ]`, `[ sync_id, status ]`, `[ src_remote_server_type, status, created_at ]`, `[ dest_remote_server_type, status, created_at ]` or `[ sync_id, status, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        public static FilesList<SyncRun> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            return new FilesList<SyncRun>($"/sync_runs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<SyncRun> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sync Run ID.
        /// </summary>
        public static async Task<SyncRun> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/sync_runs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<SyncRun>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<SyncRun> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

    }
}