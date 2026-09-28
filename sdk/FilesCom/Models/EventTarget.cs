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
    public class EventTarget : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        public EventTarget() : this(null, null) { }

        public EventTarget(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("target_type"))
            {
                this.attributes.Add("target_type", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("apply_to_all_workspaces"))
            {
                this.attributes.Add("apply_to_all_workspaces", false);
            }
            if (!this.attributes.ContainsKey("enabled"))
            {
                this.attributes.Add("enabled", false);
            }
            if (!this.attributes.ContainsKey("config"))
            {
                this.attributes.Add("config", null);
            }
            if (!this.attributes.ContainsKey("delivery_policy"))
            {
                this.attributes.Add("delivery_policy", null);
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
        /// Event Target ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Event Target name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Event Target type.
        /// </summary>
        [JsonPropertyName("target_type")]
        public string TargetType
        {
            get { return (string)attributes["target_type"]; }
            set { attributes["target_type"] = value; }
        }

        /// <summary>
        /// Workspace ID. 0 means the default workspace or site-wide.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// If true, this default-workspace target can receive events from all workspaces.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("apply_to_all_workspaces")]
        public bool ApplyToAllWorkspaces
        {
            get { return attributes["apply_to_all_workspaces"] == null ? false : (bool)attributes["apply_to_all_workspaces"]; }
            set { attributes["apply_to_all_workspaces"] = value; }
        }

        /// <summary>
        /// Whether this Event Target can receive events.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("enabled")]
        public bool Enabled
        {
            get { return attributes["enabled"] == null ? false : (bool)attributes["enabled"]; }
            set { attributes["enabled"] = value; }
        }

        /// <summary>
        /// Event Target configuration. Folder targets accept path and format (json or csv).
        /// </summary>
        [JsonPropertyName("config")]
        public object Config
        {
            get { return (object)attributes["config"]; }
            set { attributes["config"] = value; }
        }

        /// <summary>
        /// Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400.
        /// </summary>
        [JsonPropertyName("delivery_policy")]
        public object DeliveryPolicy
        {
            get { return (object)attributes["delivery_policy"]; }
            set { attributes["delivery_policy"] = value; }
        }

        /// <summary>
        /// Event Target create date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Event Target update date/time.
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
        ///   name - string - Event Target name.
        ///   workspace_id - int64 - Workspace ID. 0 means the default workspace or site-wide.
        ///   apply_to_all_workspaces - boolean - If true, this default-workspace target can receive events from all workspaces.
        ///   enabled - boolean - Whether this Event Target can receive events.
        ///   config - object - Event Target configuration. Folder targets accept path and format (json or csv).
        ///   delivery_policy - object - Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400.
        /// </summary>
        public Task<EventTarget> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Event Target name.
        ///   workspace_id - int64 - Workspace ID. 0 means the default workspace or site-wide.
        ///   apply_to_all_workspaces - boolean - If true, this default-workspace target can receive events from all workspaces.
        ///   enabled - boolean - Whether this Event Target can receive events.
        ///   config - object - Event Target configuration. Folder targets accept path and format (json or csv).
        ///   delivery_policy - object - Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400.
        /// </summary>
        public Task<EventTarget> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<EventTarget> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("config") && !(parameters["config"] is object))
            {
                throw new ArgumentException("Bad parameter: config must be of type object", "parameters[\"config\"]");
            }
            if (parameters.ContainsKey("delivery_policy") && !(parameters["delivery_policy"] is object))
            {
                throw new ArgumentException("Bad parameter: delivery_policy must be of type object", "parameters[\"delivery_policy\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/event_targets/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<EventTarget>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// </summary>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
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
            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/event_targets/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            response.Dispose();
        }


        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                await UpdateCore(this.attributes, cancellationToken);
            }
            else
            {
                var newObj = await EventTarget.CreateCore(new OperationContext(FilesClient.Bind(ref client)), this.attributes, DictionaryUtil.Copy(this.options), cancellationToken);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `enabled` or `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `enabled`, `target_type` or `workspace_id`. Valid field combinations are `[ enabled, target_type ]`, `[ workspace_id, enabled ]` or `[ workspace_id, enabled, target_type ]`.
        /// </summary>
        public static FilesList<EventTarget> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        public static FilesList<EventTarget> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<EventTarget> ListCore(
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

            return new FilesList<EventTarget>(client, $"/event_targets", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Event Target ID.
        /// </summary>
        public static Task<EventTarget> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        public static Task<EventTarget> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<EventTarget> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/event_targets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<EventTarget>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   name (required) - string - Event Target name.
        ///   workspace_id - int64 - Workspace ID. 0 means the default workspace or site-wide.
        ///   apply_to_all_workspaces - boolean - If true, this default-workspace target can receive events from all workspaces.
        ///   enabled - boolean - Whether this Event Target can receive events.
        ///   config (required) - object - Event Target configuration. Folder targets accept path and format (json or csv).
        ///   delivery_policy - object - Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400.
        ///   target_type (required) - string - Event Target type.
        /// </summary>
        public static Task<EventTarget> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<EventTarget> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("name") || parameters["name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: name", "parameters[\"name\"]");
            }
            if (!parameters.ContainsKey("config") || parameters["config"] == null)
            {
                throw new ArgumentNullException("Parameter missing: config", "parameters[\"config\"]");
            }
            if (!parameters.ContainsKey("target_type") || parameters["target_type"] == null)
            {
                throw new ArgumentNullException("Parameter missing: target_type", "parameters[\"target_type\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("config") && !(parameters["config"] is object))
            {
                throw new ArgumentException("Bad parameter: config must be of type object", "parameters[\"config\"]");
            }
            if (parameters.ContainsKey("delivery_policy") && !(parameters["delivery_policy"] is object))
            {
                throw new ArgumentException("Bad parameter: delivery_policy must be of type object", "parameters[\"delivery_policy\"]");
            }
            if (parameters.ContainsKey("target_type") && !(parameters["target_type"] is string))
            {
                throw new ArgumentException("Bad parameter: target_type must be of type string", "parameters[\"target_type\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/event_targets", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<EventTarget>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Event Target name.
        ///   workspace_id - int64 - Workspace ID. 0 means the default workspace or site-wide.
        ///   apply_to_all_workspaces - boolean - If true, this default-workspace target can receive events from all workspaces.
        ///   enabled - boolean - Whether this Event Target can receive events.
        ///   config - object - Event Target configuration. Folder targets accept path and format (json or csv).
        ///   delivery_policy - object - Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400.
        /// </summary>
        public static Task<EventTarget> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<EventTarget> UpdateCore(
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("config") && !(parameters["config"] is object))
            {
                throw new ArgumentException("Bad parameter: config must be of type object", "parameters[\"config\"]");
            }
            if (parameters.ContainsKey("delivery_policy") && !(parameters["delivery_policy"] is object))
            {
                throw new ArgumentException("Bad parameter: delivery_policy must be of type object", "parameters[\"delivery_policy\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/event_targets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<EventTarget>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// </summary>
        public static Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        public static Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(id, parameters, options);
        }

        internal static async Task DeleteCore(
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

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/event_targets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
            response.Dispose();
        }

    }
}