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
    public class ExpectationIncident : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        public ExpectationIncident() : this(null, null) { }

        public ExpectationIncident(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            get { return new object[0]; }
        }

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
        public Task<ExpectationIncident> Resolve(Dictionary<string, object> parameters)
        {
            return ResolveCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Resolve an expectation incident
        /// </summary>
        public Task<ExpectationIncident> ResolveAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ResolveCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ExpectationIncident> ResolveCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(attributes["id"].ToString())}/resolve", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ExpectationIncident>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Snooze an expectation incident until a specified time
        ///
        /// Parameters:
        ///   snoozed_until (required) - string - Time until which the incident should remain snoozed.
        /// </summary>
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
        public Task<ExpectationIncident> SnoozeAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return SnoozeCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ExpectationIncident> SnoozeCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("snoozed_until") || parameters["snoozed_until"] == null)
            {
                throw new ArgumentNullException("Parameter missing: snoozed_until", "parameters[\"snoozed_until\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("snoozed_until") && !(parameters["snoozed_until"] is string))
            {
                throw new ArgumentException("Bad parameter: snoozed_until must be of type string", "parameters[\"snoozed_until\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(attributes["id"].ToString())}/snooze", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ExpectationIncident>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Acknowledge an expectation incident
        /// </summary>
        public Task<ExpectationIncident> Acknowledge(Dictionary<string, object> parameters)
        {
            return AcknowledgeCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Acknowledge an expectation incident
        /// </summary>
        public Task<ExpectationIncident> AcknowledgeAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return AcknowledgeCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ExpectationIncident> AcknowledgeCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(attributes["id"].ToString())}/acknowledge", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ExpectationIncident>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `expectation_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `expectation_id` and `workspace_id`. Valid field combinations are `[ workspace_id, expectation_id ]`.
        /// </summary>
        public static FilesList<ExpectationIncident> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

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

            return new FilesList<ExpectationIncident>(client, $"/expectation_incidents", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation Incident ID.
        /// </summary>
        public static Task<ExpectationIncident> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

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

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ExpectationIncident>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Resolve an expectation incident
        /// </summary>
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(parameters["id"].ToString())}/resolve", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ExpectationIncident>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Snooze an expectation incident until a specified time
        ///
        /// Parameters:
        ///   snoozed_until (required) - string - Time until which the incident should remain snoozed.
        /// </summary>
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
            if (!parameters.ContainsKey("snoozed_until") || parameters["snoozed_until"] == null)
            {
                throw new ArgumentNullException("Parameter missing: snoozed_until", "parameters[\"snoozed_until\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("snoozed_until") && !(parameters["snoozed_until"] is string))
            {
                throw new ArgumentException("Bad parameter: snoozed_until must be of type string", "parameters[\"snoozed_until\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(parameters["id"].ToString())}/snooze", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ExpectationIncident>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Acknowledge an expectation incident
        /// </summary>
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_incidents/{System.Uri.EscapeDataString(parameters["id"].ToString())}/acknowledge", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ExpectationIncident>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

    }
}