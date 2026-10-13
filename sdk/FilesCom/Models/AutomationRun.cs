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
    /// AutomationRunEntity model
    /// </summary>
    public class AutomationRun : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        /// <summary>
        /// Makes a AutomationRun with no attributes set and no options.
        /// </summary>
        public AutomationRun() : this(null, null) { }

        /// <summary>
        /// Makes a AutomationRun with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public AutomationRun(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("automation_id"))
            {
                this.attributes.Add("automation_id", null);
            }
            if (!this.attributes.ContainsKey("automation_version_id"))
            {
                this.attributes.Add("automation_version_id", null);
            }
            if (!this.attributes.ContainsKey("version"))
            {
                this.attributes.Add("version", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("cancel_requested_at"))
            {
                this.attributes.Add("cancel_requested_at", null);
            }
            if (!this.attributes.ContainsKey("completed_at"))
            {
                this.attributes.Add("completed_at", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("retry_at"))
            {
                this.attributes.Add("retry_at", null);
            }
            if (!this.attributes.ContainsKey("retried_at"))
            {
                this.attributes.Add("retried_at", null);
            }
            if (!this.attributes.ContainsKey("retried_in_run_id"))
            {
                this.attributes.Add("retried_in_run_id", null);
            }
            if (!this.attributes.ContainsKey("retry_of_run_id"))
            {
                this.attributes.Add("retry_of_run_id", null);
            }
            if (!this.attributes.ContainsKey("rerun_of_run_id"))
            {
                this.attributes.Add("rerun_of_run_id", null);
            }
            if (!this.attributes.ContainsKey("rerun_from_node_id"))
            {
                this.attributes.Add("rerun_from_node_id", null);
            }
            if (!this.attributes.ContainsKey("runtime"))
            {
                this.attributes.Add("runtime", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("successful_operations"))
            {
                this.attributes.Add("successful_operations", null);
            }
            if (!this.attributes.ContainsKey("failed_operations"))
            {
                this.attributes.Add("failed_operations", null);
            }
            if (!this.attributes.ContainsKey("definition"))
            {
                this.attributes.Add("definition", null);
            }
            if (!this.attributes.ContainsKey("node_states"))
            {
                this.attributes.Add("node_states", null);
            }
            if (!this.attributes.ContainsKey("execution_nodes"))
            {
                this.attributes.Add("execution_nodes", new AutomationExecutionNode[0]);
            }
            if (!this.attributes.ContainsKey("journal_url"))
            {
                this.attributes.Add("journal_url", null);
            }
            if (!this.attributes.ContainsKey("status_messages_url"))
            {
                this.attributes.Add("status_messages_url", null);
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
            get { return new object[] { ExecutionNodes }; }
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
        /// ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// ID of the associated Automation.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("automation_id")]
        public Nullable<Int64> AutomationId
        {
            get { return (Nullable<Int64>)attributes["automation_id"]; }
            private set { attributes["automation_id"] = value; }
        }

        /// <summary>
        /// ID of the immutable Automation version pinned by this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("automation_version_id")]
        public Nullable<Int64> AutomationVersionId
        {
            get { return (Nullable<Int64>)attributes["automation_version_id"]; }
            private set { attributes["automation_version_id"] = value; }
        }

        /// <summary>
        /// Pinned Automation v2 definition version.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("version")]
        public Nullable<Int64> Version
        {
            get { return (Nullable<Int64>)attributes["version"]; }
            private set { attributes["version"] = value; }
        }

        /// <summary>
        /// Workspace ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            private set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// Date/time at which cancellation was requested.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cancel_requested_at")]
        public Nullable<DateTime> CancelRequestedAt
        {
            get { return (Nullable<DateTime>)attributes["cancel_requested_at"]; }
            private set { attributes["cancel_requested_at"] = value; }
        }

        /// <summary>
        /// Automation run completion/failure date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed_at")]
        public Nullable<DateTime> CompletedAt
        {
            get { return (Nullable<DateTime>)attributes["completed_at"]; }
            private set { attributes["completed_at"] = value; }
        }

        /// <summary>
        /// Automation run start date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// If set, this automation will be retried at this date/time due to `failure` or `partial_failure`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("retry_at")]
        public Nullable<DateTime> RetryAt
        {
            get { return (Nullable<DateTime>)attributes["retry_at"]; }
            private set { attributes["retry_at"] = value; }
        }

        /// <summary>
        /// If set, this Automation run was retried due to `failure` or `partial_failure`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("retried_at")]
        public Nullable<DateTime> RetriedAt
        {
            get { return (Nullable<DateTime>)attributes["retried_at"]; }
            private set { attributes["retried_at"] = value; }
        }

        /// <summary>
        /// ID of the run that is or will be retrying this run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("retried_in_run_id")]
        public Nullable<Int64> RetriedInRunId
        {
            get { return (Nullable<Int64>)attributes["retried_in_run_id"]; }
            private set { attributes["retried_in_run_id"] = value; }
        }

        /// <summary>
        /// ID of the original run that this run is retrying.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("retry_of_run_id")]
        public Nullable<Int64> RetryOfRunId
        {
            get { return (Nullable<Int64>)attributes["retry_of_run_id"]; }
            private set { attributes["retry_of_run_id"] = value; }
        }

        /// <summary>
        /// ID of the run whose persisted node outputs this run reused.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("rerun_of_run_id")]
        public Nullable<Int64> RerunOfRunId
        {
            get { return (Nullable<Int64>)attributes["rerun_of_run_id"]; }
            private set { attributes["rerun_of_run_id"] = value; }
        }

        /// <summary>
        /// Node at which this run resumed execution.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("rerun_from_node_id")]
        public string RerunFromNodeId
        {
            get { return (string)attributes["rerun_from_node_id"]; }
            private set { attributes["rerun_from_node_id"] = value; }
        }

        /// <summary>
        /// Automation run runtime.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("runtime")]
        public Nullable<double> Runtime
        {
            get { return (Nullable<double>)attributes["runtime"]; }
            private set { attributes["runtime"] = value; }
        }

        /// <summary>
        /// The status of the AutomationRun. One of `queued`, `running`, `success`, `partial_failure`, `failure`, `skipped`, or `canceled`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Count of successful operations.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("successful_operations")]
        public Nullable<Int64> SuccessfulOperations
        {
            get { return (Nullable<Int64>)attributes["successful_operations"]; }
            private set { attributes["successful_operations"] = value; }
        }

        /// <summary>
        /// Count of failed operations.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failed_operations")]
        public Nullable<Int64> FailedOperations
        {
            get { return (Nullable<Int64>)attributes["failed_operations"]; }
            private set { attributes["failed_operations"] = value; }
        }

        /// <summary>
        /// Automation definition snapshot pinned by this run. For performance reasons, this is not provided when listing Automation runs.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("definition")]
        public object Definition
        {
            get { return (object)attributes["definition"]; }
            private set { attributes["definition"] = value; }
        }

        /// <summary>
        /// Status and execution stage for each node in this run. For performance reasons, this is not provided when listing Automation runs.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("node_states")]
        public object NodeStates
        {
            get { return (object)attributes["node_states"]; }
            private set { attributes["node_states"] = value; }
        }

        /// <summary>
        /// Execution status, timing, and bounded output summaries for each node. For performance reasons, this is not provided when listing Automation runs.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("execution_nodes")]
        public AutomationExecutionNode[] ExecutionNodes
        {
            get { return (AutomationExecutionNode[])attributes["execution_nodes"]; }
            private set { attributes["execution_nodes"] = value; }
        }

        /// <summary>
        /// Link to the run journal artifact.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("journal_url")]
        public string JournalUrl
        {
            get { return (string)attributes["journal_url"]; }
            private set { attributes["journal_url"] = value; }
        }

        /// <summary>
        /// Link to status messages log file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status_messages_url")]
        public string StatusMessagesUrl
        {
            get { return (string)attributes["status_messages_url"]; }
            private set { attributes["status_messages_url"] = value; }
        }

        /// <summary>
        /// Cancel Automation Run
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task<AutomationRun> Cancel(Dictionary<string, object> parameters)
        {
            return CancelCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Cancel Automation Run
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<AutomationRun> CancelAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return CancelCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<AutomationRun> CancelCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The AutomationRun has no id, which Cancel needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/automation_runs/{System.Uri.EscapeDataString(attributes["id"].ToString())}/cancel", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            AutomationRun result = JsonUtil.DeserializeModelResponse<AutomationRun>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Re-run Automation from Node
        ///
        /// Parameters:
        ///   node_id (required) - string - Node ID at which execution should resume.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<AutomationRun> Rerun(Dictionary<string, object> parameters)
        {
            return RerunCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Re-run Automation from Node
        ///
        /// Parameters:
        ///   node_id (required) - string - Node ID at which execution should resume.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<AutomationRun> RerunAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return RerunCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<AutomationRun> RerunCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The AutomationRun has no id, which Rerun needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (!parameters.TryGetValue("node_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: node_id", "node_id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("node_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: node_id is null, which cannot be sent" : "Bad parameter: node_id must be of type string", "node_id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/automation_runs/{System.Uri.EscapeDataString(attributes["id"].ToString())}/rerun", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            AutomationRun result = JsonUtil.DeserializeModelResponse<AutomationRun>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `automation_id`, `created_at` or `status`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `status`, `workspace_id` or `automation_id`. Valid field combinations are `[ workspace_id, status ]`, `[ automation_id, status ]`, `[ workspace_id, automation_id ]` or `[ workspace_id, automation_id, status ]`.
        ///   automation_id (required) - int64 - ID of the associated Automation.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<AutomationRun> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `automation_id`, `created_at` or `status`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `status`, `workspace_id` or `automation_id`. Valid field combinations are `[ workspace_id, status ]`, `[ automation_id, status ]`, `[ workspace_id, automation_id ]` or `[ workspace_id, automation_id, status ]`.
        ///   automation_id (required) - int64 - ID of the associated Automation.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<AutomationRun> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<AutomationRun> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("automation_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: automation_id", "automation_id");
            }
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
            if (parameters.TryGetValue("automation_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: automation_id is null, which cannot be sent" : "Bad parameter: automation_id must be of type Nullable<Int64>", "automation_id");
            }

            return new FilesList<AutomationRun>(client, $"/automation_runs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Automation Run ID.
        /// </summary>
        /// <param name="id">Automation Run ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AutomationRun> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Automation Run ID.
        /// </summary>
        /// <param name="id">Automation Run ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AutomationRun> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<AutomationRun> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/automation_runs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AutomationRun>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Automation Run ID.
        ///   node_id (required) - string - Node ID from the pinned Automation definition.
        /// </summary>
        /// <param name="id">Automation Run ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AutomationExecutionNode> FindNode(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindNodeCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<AutomationExecutionNode> FindNodeCore(
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
            if (!parameters.TryGetValue("node_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: node_id", "node_id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("node_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: node_id is null, which cannot be sent" : "Bad parameter: node_id must be of type string", "node_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/automation_runs/{System.Uri.EscapeDataString(parameters["id"].ToString())}/node", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AutomationExecutionNode>(responseJson, context, options);
        }

        /// <summary>
        /// Cancel Automation Run
        /// </summary>
        /// <param name="id">Automation Run ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AutomationRun> Cancel(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CancelCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<AutomationRun> CancelCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/automation_runs/{System.Uri.EscapeDataString(parameters["id"].ToString())}/cancel", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AutomationRun>(responseJson, context, options);
        }

        /// <summary>
        /// Re-run Automation from Node
        ///
        /// Parameters:
        ///   node_id (required) - string - Node ID at which execution should resume.
        /// </summary>
        /// <param name="id">Automation Run ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AutomationRun> Rerun(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return RerunCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<AutomationRun> RerunCore(
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
            if (!parameters.TryGetValue("node_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: node_id", "node_id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("node_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: node_id is null, which cannot be sent" : "Bad parameter: node_id must be of type string", "node_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/automation_runs/{System.Uri.EscapeDataString(parameters["id"].ToString())}/rerun", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AutomationRun>(responseJson, context, options);
        }

    }
}