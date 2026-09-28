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
    public class KeyLifecycleRule : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        public KeyLifecycleRule() : this(null, null) { }

        public KeyLifecycleRule(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("key_type"))
            {
                this.attributes.Add("key_type", null);
            }
            if (!this.attributes.ContainsKey("inactivity_days"))
            {
                this.attributes.Add("inactivity_days", null);
            }
            if (!this.attributes.ContainsKey("expiration_days"))
            {
                this.attributes.Add("expiration_days", null);
            }
            if (!this.attributes.ContainsKey("apply_to_all_workspaces"))
            {
                this.attributes.Add("apply_to_all_workspaces", false);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
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
        /// Key Lifecycle Rule ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Key type for which the rule will apply (gpg, ssh, or api).
        /// </summary>
        [JsonPropertyName("key_type")]
        public string KeyType
        {
            get { return (string)attributes["key_type"]; }
            set { attributes["key_type"] = value; }
        }

        /// <summary>
        /// Number of days of inactivity before the rule applies.
        /// </summary>
        [JsonPropertyName("inactivity_days")]
        public Nullable<Int64> InactivityDays
        {
            get { return (Nullable<Int64>)attributes["inactivity_days"]; }
            set { attributes["inactivity_days"] = value; }
        }

        /// <summary>
        /// Number of days after creation before an SSH key expires. Applies only to SSH keys.
        /// </summary>
        [JsonPropertyName("expiration_days")]
        public Nullable<Int64> ExpirationDays
        {
            get { return (Nullable<Int64>)attributes["expiration_days"]; }
            set { attributes["expiration_days"] = value; }
        }

        /// <summary>
        /// If true, a default-workspace rule also applies to keys in all workspaces.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("apply_to_all_workspaces")]
        public bool ApplyToAllWorkspaces
        {
            get { return attributes["apply_to_all_workspaces"] == null ? false : (bool)attributes["apply_to_all_workspaces"]; }
            set { attributes["apply_to_all_workspaces"] = value; }
        }

        /// <summary>
        /// Key Lifecycle Rule name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Workspace ID. `0` means the default workspace.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   apply_to_all_workspaces - boolean - If true, a default-workspace rule also applies to keys in all workspaces.
        ///   expiration_days - int64 - Number of days after creation before an SSH key expires. Applies only to SSH keys.
        ///   key_type - string - Key type for which the rule will apply (gpg, ssh, or api).
        ///   inactivity_days - int64 - Number of days of inactivity before the rule applies.
        ///   name - string - Key Lifecycle Rule name
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public Task<KeyLifecycleRule> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   apply_to_all_workspaces - boolean - If true, a default-workspace rule also applies to keys in all workspaces.
        ///   expiration_days - int64 - Number of days after creation before an SSH key expires. Applies only to SSH keys.
        ///   key_type - string - Key type for which the rule will apply (gpg, ssh, or api).
        ///   inactivity_days - int64 - Number of days of inactivity before the rule applies.
        ///   name - string - Key Lifecycle Rule name
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public Task<KeyLifecycleRule> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<KeyLifecycleRule> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
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
            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("expiration_days") && !(parameters["expiration_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: expiration_days must be of type Nullable<Int64>", "parameters[\"expiration_days\"]");
            }
            if (parameters.ContainsKey("key_type") && !(parameters["key_type"] is string))
            {
                throw new ArgumentException("Bad parameter: key_type must be of type string", "parameters[\"key_type\"]");
            }
            if (parameters.ContainsKey("inactivity_days") && !(parameters["inactivity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_days must be of type Nullable<Int64>", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/key_lifecycle_rules/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<KeyLifecycleRule>(responseJson, context.Client, requestOptions);
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
            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/key_lifecycle_rules/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
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
                var newObj = await KeyLifecycleRule.CreateCore(new OperationContext(FilesClient.Bind(ref client)), this.attributes, DictionaryUtil.Copy(this.options), cancellationToken);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `key_type`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `key_type` and `workspace_id`. Valid field combinations are `[ workspace_id, key_type ]`.
        /// </summary>
        public static FilesList<KeyLifecycleRule> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        public static FilesList<KeyLifecycleRule> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<KeyLifecycleRule> ListCore(
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

            return new FilesList<KeyLifecycleRule>(client, $"/key_lifecycle_rules", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Key Lifecycle Rule ID.
        /// </summary>
        public static Task<KeyLifecycleRule> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        public static Task<KeyLifecycleRule> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<KeyLifecycleRule> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/key_lifecycle_rules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<KeyLifecycleRule>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   apply_to_all_workspaces - boolean - If true, a default-workspace rule also applies to keys in all workspaces.
        ///   expiration_days - int64 - Number of days after creation before an SSH key expires. Applies only to SSH keys.
        ///   key_type - string - Key type for which the rule will apply (gpg, ssh, or api).
        ///   inactivity_days - int64 - Number of days of inactivity before the rule applies.
        ///   name - string - Key Lifecycle Rule name
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public static Task<KeyLifecycleRule> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<KeyLifecycleRule> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("expiration_days") && !(parameters["expiration_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: expiration_days must be of type Nullable<Int64>", "parameters[\"expiration_days\"]");
            }
            if (parameters.ContainsKey("key_type") && !(parameters["key_type"] is string))
            {
                throw new ArgumentException("Bad parameter: key_type must be of type string", "parameters[\"key_type\"]");
            }
            if (parameters.ContainsKey("inactivity_days") && !(parameters["inactivity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_days must be of type Nullable<Int64>", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/key_lifecycle_rules", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<KeyLifecycleRule>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   apply_to_all_workspaces - boolean - If true, a default-workspace rule also applies to keys in all workspaces.
        ///   expiration_days - int64 - Number of days after creation before an SSH key expires. Applies only to SSH keys.
        ///   key_type - string - Key type for which the rule will apply (gpg, ssh, or api).
        ///   inactivity_days - int64 - Number of days of inactivity before the rule applies.
        ///   name - string - Key Lifecycle Rule name
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public static Task<KeyLifecycleRule> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<KeyLifecycleRule> UpdateCore(
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
            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("expiration_days") && !(parameters["expiration_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: expiration_days must be of type Nullable<Int64>", "parameters[\"expiration_days\"]");
            }
            if (parameters.ContainsKey("key_type") && !(parameters["key_type"] is string))
            {
                throw new ArgumentException("Bad parameter: key_type must be of type string", "parameters[\"key_type\"]");
            }
            if (parameters.ContainsKey("inactivity_days") && !(parameters["inactivity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_days must be of type Nullable<Int64>", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/key_lifecycle_rules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<KeyLifecycleRule>(responseJson, context.Client, options);
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

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/key_lifecycle_rules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
            response.Dispose();
        }

    }
}