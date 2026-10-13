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
    /// SyncRunEntity model
    /// </summary>
    public class SyncRun : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a SyncRun with no attributes set and no options.
        /// </summary>
        public SyncRun() : this(null, null) { }

        /// <summary>
        /// Makes a SyncRun with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public SyncRun(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("body"))
            {
                this.attributes.Add("body", null);
            }
            if (!this.attributes.ContainsKey("bytes_synced"))
            {
                this.attributes.Add("bytes_synced", null);
            }
            if (!this.attributes.ContainsKey("compared_files"))
            {
                this.attributes.Add("compared_files", null);
            }
            if (!this.attributes.ContainsKey("compared_folders"))
            {
                this.attributes.Add("compared_folders", null);
            }
            if (!this.attributes.ContainsKey("completed_at"))
            {
                this.attributes.Add("completed_at", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("dest_remote_server_type"))
            {
                this.attributes.Add("dest_remote_server_type", null);
            }
            if (!this.attributes.ContainsKey("dry_run"))
            {
                this.attributes.Add("dry_run", false);
            }
            if (!this.attributes.ContainsKey("errored_files"))
            {
                this.attributes.Add("errored_files", null);
            }
            if (!this.attributes.ContainsKey("estimated_bytes_count"))
            {
                this.attributes.Add("estimated_bytes_count", null);
            }
            if (!this.attributes.ContainsKey("event_errors"))
            {
                this.attributes.Add("event_errors", new string[0]);
            }
            if (!this.attributes.ContainsKey("log_url"))
            {
                this.attributes.Add("log_url", null);
            }
            if (!this.attributes.ContainsKey("runtime"))
            {
                this.attributes.Add("runtime", null);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("src_remote_server_type"))
            {
                this.attributes.Add("src_remote_server_type", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("successful_files"))
            {
                this.attributes.Add("successful_files", null);
            }
            if (!this.attributes.ContainsKey("sync_id"))
            {
                this.attributes.Add("sync_id", null);
            }
            if (!this.attributes.ContainsKey("sync_name"))
            {
                this.attributes.Add("sync_name", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
            }
            if (!this.attributes.ContainsKey("live_transfers"))
            {
                this.attributes.Add("live_transfers", new SyncRunLiveTransfer[0]);
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
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => null;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[] { LiveTransfers }; }
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
        /// Total runtime in seconds
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("runtime")]
        public Nullable<double> Runtime
        {
            get { return (Nullable<double>)attributes["runtime"]; }
            private set { attributes["runtime"] = value; }
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
        /// Workspace ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            private set { attributes["workspace_id"] = value; }
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
        /// Name of the Sync this run belongs to
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_name")]
        public string SyncName
        {
            get { return (string)attributes["sync_name"]; }
            private set { attributes["sync_name"] = value; }
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
        /// Array of in-progress file transfers with progress data. Only present when the sync run status is in_progress.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("live_transfers")]
        public SyncRunLiveTransfer[] LiveTransfers
        {
            get { return (SyncRunLiveTransfer[])attributes["live_transfers"]; }
            private set { attributes["live_transfers"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id`, `sync_id`, `created_at` or `status`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `status`, `dry_run`, `workspace_id`, `src_remote_server_type`, `dest_remote_server_type` or `sync_id`. Valid field combinations are `[ status, created_at ]`, `[ workspace_id, created_at ]`, `[ src_remote_server_type, created_at ]`, `[ dest_remote_server_type, created_at ]`, `[ sync_id, created_at ]`, `[ workspace_id, status ]`, `[ src_remote_server_type, status ]`, `[ dest_remote_server_type, status ]`, `[ sync_id, status ]`, `[ workspace_id, src_remote_server_type ]`, `[ workspace_id, dest_remote_server_type ]`, `[ workspace_id, sync_id ]`, `[ workspace_id, status, created_at ]`, `[ src_remote_server_type, status, created_at ]`, `[ dest_remote_server_type, status, created_at ]`, `[ sync_id, status, created_at ]`, `[ workspace_id, src_remote_server_type, created_at ]`, `[ workspace_id, dest_remote_server_type, created_at ]`, `[ workspace_id, sync_id, created_at ]`, `[ workspace_id, src_remote_server_type, status ]`, `[ workspace_id, dest_remote_server_type, status ]`, `[ workspace_id, sync_id, status ]`, `[ workspace_id, src_remote_server_type, status, created_at ]`, `[ workspace_id, dest_remote_server_type, status, created_at ]` or `[ workspace_id, sync_id, status, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<SyncRun> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id`, `sync_id`, `created_at` or `status`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `status`, `dry_run`, `workspace_id`, `src_remote_server_type`, `dest_remote_server_type` or `sync_id`. Valid field combinations are `[ status, created_at ]`, `[ workspace_id, created_at ]`, `[ src_remote_server_type, created_at ]`, `[ dest_remote_server_type, created_at ]`, `[ sync_id, created_at ]`, `[ workspace_id, status ]`, `[ src_remote_server_type, status ]`, `[ dest_remote_server_type, status ]`, `[ sync_id, status ]`, `[ workspace_id, src_remote_server_type ]`, `[ workspace_id, dest_remote_server_type ]`, `[ workspace_id, sync_id ]`, `[ workspace_id, status, created_at ]`, `[ src_remote_server_type, status, created_at ]`, `[ dest_remote_server_type, status, created_at ]`, `[ sync_id, status, created_at ]`, `[ workspace_id, src_remote_server_type, created_at ]`, `[ workspace_id, dest_remote_server_type, created_at ]`, `[ workspace_id, sync_id, created_at ]`, `[ workspace_id, src_remote_server_type, status ]`, `[ workspace_id, dest_remote_server_type, status ]`, `[ workspace_id, sync_id, status ]`, `[ workspace_id, src_remote_server_type, status, created_at ]`, `[ workspace_id, dest_remote_server_type, status, created_at ]` or `[ workspace_id, sync_id, status, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<SyncRun> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<SyncRun> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }
            if (parameters.TryGetValue("filter_gt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gt is null, which cannot be sent" : "Bad parameter: filter_gt must be of type object", "filter_gt");
            }
            if (parameters.TryGetValue("filter_gteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gteq is null, which cannot be sent" : "Bad parameter: filter_gteq must be of type object", "filter_gteq");
            }
            if (parameters.TryGetValue("filter_lt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lt is null, which cannot be sent" : "Bad parameter: filter_lt must be of type object", "filter_lt");
            }
            if (parameters.TryGetValue("filter_lteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lteq is null, which cannot be sent" : "Bad parameter: filter_lteq must be of type object", "filter_lteq");
            }

            return new FilesList<SyncRun>(client, $"/sync_runs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sync Run ID.
        /// </summary>
        /// <param name="id">Sync Run ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<SyncRun> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sync Run ID.
        /// </summary>
        /// <param name="id">Sync Run ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<SyncRun> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<SyncRun> FindCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/sync_runs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<SyncRun>(responseJson, context, options);
        }

    }
}