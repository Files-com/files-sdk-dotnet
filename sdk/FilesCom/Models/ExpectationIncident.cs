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
    /// ExpectationIncidentEntity model
    /// </summary>
    public class ExpectationIncident : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        /// <summary>
        /// Makes a ExpectationIncident with no attributes set and no options.
        /// </summary>
        public ExpectationIncident() : this(null, null) { }

        /// <summary>
        /// Makes a ExpectationIncident with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public ExpectationIncident(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("expectation_id"))
            {
                this.attributes.Add("expectation_id", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("opened_at"))
            {
                this.attributes.Add("opened_at", null);
            }
            if (!this.attributes.ContainsKey("last_failed_at"))
            {
                this.attributes.Add("last_failed_at", null);
            }
            if (!this.attributes.ContainsKey("acknowledged_at"))
            {
                this.attributes.Add("acknowledged_at", null);
            }
            if (!this.attributes.ContainsKey("snoozed_until"))
            {
                this.attributes.Add("snoozed_until", null);
            }
            if (!this.attributes.ContainsKey("resolved_at"))
            {
                this.attributes.Add("resolved_at", null);
            }
            if (!this.attributes.ContainsKey("opened_by_evaluation_id"))
            {
                this.attributes.Add("opened_by_evaluation_id", null);
            }
            if (!this.attributes.ContainsKey("last_evaluation_id"))
            {
                this.attributes.Add("last_evaluation_id", null);
            }
            if (!this.attributes.ContainsKey("resolved_by_evaluation_id"))
            {
                this.attributes.Add("resolved_by_evaluation_id", null);
            }
            if (!this.attributes.ContainsKey("summary"))
            {
                this.attributes.Add("summary", null);
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
            get { return new object[0]; }
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
        /// Expectation Incident ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Workspace ID. `0` means the default workspace.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            private set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// Expectation ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("expectation_id")]
        public Nullable<Int64> ExpectationId
        {
            get { return (Nullable<Int64>)attributes["expectation_id"]; }
            private set { attributes["expectation_id"] = value; }
        }

        /// <summary>
        /// Incident status.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// When the incident was opened.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("opened_at")]
        public Nullable<DateTime> OpenedAt
        {
            get { return (Nullable<DateTime>)attributes["opened_at"]; }
            private set { attributes["opened_at"] = value; }
        }

        /// <summary>
        /// When the most recent failing evaluation contributing to the incident occurred.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_failed_at")]
        public Nullable<DateTime> LastFailedAt
        {
            get { return (Nullable<DateTime>)attributes["last_failed_at"]; }
            private set { attributes["last_failed_at"] = value; }
        }

        /// <summary>
        /// When the incident was acknowledged.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("acknowledged_at")]
        public Nullable<DateTime> AcknowledgedAt
        {
            get { return (Nullable<DateTime>)attributes["acknowledged_at"]; }
            private set { attributes["acknowledged_at"] = value; }
        }

        /// <summary>
        /// When the current snooze expires.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("snoozed_until")]
        public Nullable<DateTime> SnoozedUntil
        {
            get { return (Nullable<DateTime>)attributes["snoozed_until"]; }
            private set { attributes["snoozed_until"] = value; }
        }

        /// <summary>
        /// When the incident was resolved.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("resolved_at")]
        public Nullable<DateTime> ResolvedAt
        {
            get { return (Nullable<DateTime>)attributes["resolved_at"]; }
            private set { attributes["resolved_at"] = value; }
        }

        /// <summary>
        /// Evaluation that first opened the incident.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("opened_by_evaluation_id")]
        public Nullable<Int64> OpenedByEvaluationId
        {
            get { return (Nullable<Int64>)attributes["opened_by_evaluation_id"]; }
            private set { attributes["opened_by_evaluation_id"] = value; }
        }

        /// <summary>
        /// Most recent evaluation linked to the incident.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_evaluation_id")]
        public Nullable<Int64> LastEvaluationId
        {
            get { return (Nullable<Int64>)attributes["last_evaluation_id"]; }
            private set { attributes["last_evaluation_id"] = value; }
        }

        /// <summary>
        /// Evaluation that resolved the incident.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("resolved_by_evaluation_id")]
        public Nullable<Int64> ResolvedByEvaluationId
        {
            get { return (Nullable<Int64>)attributes["resolved_by_evaluation_id"]; }
            private set { attributes["resolved_by_evaluation_id"] = value; }
        }

        /// <summary>
        /// Compact incident summary payload.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("summary")]
        public object Summary
        {
            get { return (object)attributes["summary"]; }
            private set { attributes["summary"] = value; }
        }

        /// <summary>
        /// Creation time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Last update time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>)attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }

        /// <summary>
        /// Resolve an expectation incident
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task<ExpectationIncident> Resolve(Dictionary<string, object> parameters)
        {
            return ResolveCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Resolve an expectation incident
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<ExpectationIncident> ResolveAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ResolveCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ExpectationIncident> ResolveCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The ExpectationIncident has no id, which Resolve needs", "id");
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
            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(attributes["id"].ToString())}/resolve", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            ExpectationIncident result = JsonUtil.DeserializeModelResponse<ExpectationIncident>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Snooze an expectation incident until a specified time
        ///
        /// Parameters:
        ///   snoozed_until (required) - string - Time until which the incident should remain snoozed.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<ExpectationIncident> Snooze(Dictionary<string, object> parameters)
        {
            return SnoozeCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Snooze an expectation incident until a specified time
        ///
        /// Parameters:
        ///   snoozed_until (required) - string - Time until which the incident should remain snoozed.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<ExpectationIncident> SnoozeAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return SnoozeCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ExpectationIncident> SnoozeCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The ExpectationIncident has no id, which Snooze needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (!parameters.TryGetValue("snoozed_until", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: snoozed_until", "snoozed_until");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("snoozed_until", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: snoozed_until is null, which cannot be sent" : "Bad parameter: snoozed_until must be of type string", "snoozed_until");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(attributes["id"].ToString())}/snooze", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            ExpectationIncident result = JsonUtil.DeserializeModelResponse<ExpectationIncident>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Acknowledge an expectation incident
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task<ExpectationIncident> Acknowledge(Dictionary<string, object> parameters)
        {
            return AcknowledgeCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Acknowledge an expectation incident
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<ExpectationIncident> AcknowledgeAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return AcknowledgeCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ExpectationIncident> AcknowledgeCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The ExpectationIncident has no id, which Acknowledge needs", "id");
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
            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(attributes["id"].ToString())}/acknowledge", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            ExpectationIncident result = JsonUtil.DeserializeModelResponse<ExpectationIncident>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `expectation_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `expectation_id` and `workspace_id`. Valid field combinations are `[ workspace_id, expectation_id ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<ExpectationIncident> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `expectation_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `expectation_id` and `workspace_id`. Valid field combinations are `[ workspace_id, expectation_id ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<ExpectationIncident> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<ExpectationIncident> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

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

            return new FilesList<ExpectationIncident>(client, $"/expectation_incidents", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation Incident ID.
        /// </summary>
        /// <param name="id">Expectation Incident ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ExpectationIncident> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation Incident ID.
        /// </summary>
        /// <param name="id">Expectation Incident ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ExpectationIncident> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<ExpectationIncident> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ExpectationIncident>(responseJson, context, options);
        }

        /// <summary>
        /// Resolve an expectation incident
        /// </summary>
        /// <param name="id">Expectation Incident ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ExpectationIncident> Resolve(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ResolveCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<ExpectationIncident> ResolveCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(parameters["id"].ToString())}/resolve", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ExpectationIncident>(responseJson, context, options);
        }

        /// <summary>
        /// Snooze an expectation incident until a specified time
        ///
        /// Parameters:
        ///   snoozed_until (required) - string - Time until which the incident should remain snoozed.
        /// </summary>
        /// <param name="id">Expectation Incident ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ExpectationIncident> Snooze(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return SnoozeCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<ExpectationIncident> SnoozeCore(
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
            if (!parameters.TryGetValue("snoozed_until", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: snoozed_until", "snoozed_until");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("snoozed_until", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: snoozed_until is null, which cannot be sent" : "Bad parameter: snoozed_until must be of type string", "snoozed_until");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(parameters["id"].ToString())}/snooze", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ExpectationIncident>(responseJson, context, options);
        }

        /// <summary>
        /// Acknowledge an expectation incident
        /// </summary>
        /// <param name="id">Expectation Incident ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ExpectationIncident> Acknowledge(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return AcknowledgeCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<ExpectationIncident> AcknowledgeCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(parameters["id"].ToString())}/acknowledge", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ExpectationIncident>(responseJson, context, options);
        }

    }
}