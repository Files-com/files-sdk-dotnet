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
    public class ChildSiteManagementPolicy : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        public ChildSiteManagementPolicy() : this(null, null) { }

        public ChildSiteManagementPolicy(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("policy_type"))
            {
                this.attributes.Add("policy_type", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("value"))
            {
                this.attributes.Add("value", null);
            }
            if (!this.attributes.ContainsKey("applied_child_site_ids"))
            {
                this.attributes.Add("applied_child_site_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("skip_child_site_ids"))
            {
                this.attributes.Add("skip_child_site_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("child_site_ids"))
            {
                this.attributes.Add("child_site_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("default_policy"))
            {
                this.attributes.Add("default_policy", false);
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
        /// Policy ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Type of policy.  Valid values: `settings`.
        /// </summary>
        [JsonPropertyName("policy_type")]
        public string PolicyType
        {
            get { return (string)attributes["policy_type"]; }
            set { attributes["policy_type"] = value; }
        }

        /// <summary>
        /// Name for this policy.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Description for this policy.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; }
        }

        /// <summary>
        /// Policy configuration data. Settings policies accept site settings plus an optional `folder_behaviors` array for parent-managed root behaviors on child sites. For more information, refer to the Value Hash section of the developer documentation.
        /// </summary>
        [JsonPropertyName("value")]
        public object Value
        {
            get { return (object)attributes["value"]; }
            set { attributes["value"] = value; }
        }

        /// <summary>
        /// IDs of child sites that this policy has been applied to. This field is read-only.
        /// </summary>
        [JsonPropertyName("applied_child_site_ids")]
        public Nullable<Int64>[] AppliedChildSiteIds
        {
            get { return (Nullable<Int64>[])attributes["applied_child_site_ids"]; }
            set { attributes["applied_child_site_ids"] = value; }
        }

        /// <summary>
        /// IDs of child sites excluded from this default policy.
        /// </summary>
        [JsonPropertyName("skip_child_site_ids")]
        public Nullable<Int64>[] SkipChildSiteIds
        {
            get { return (Nullable<Int64>[])attributes["skip_child_site_ids"]; }
            set { attributes["skip_child_site_ids"] = value; }
        }

        /// <summary>
        /// IDs of child sites explicitly assigned to this non-default policy.
        /// </summary>
        [JsonPropertyName("child_site_ids")]
        public Nullable<Int64>[] ChildSiteIds
        {
            get { return (Nullable<Int64>[])attributes["child_site_ids"]; }
            set { attributes["child_site_ids"] = value; }
        }

        /// <summary>
        /// Whether this policy applies to child sites not explicitly assigned to another policy.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("default_policy")]
        public bool DefaultPolicy
        {
            get { return attributes["default_policy"] == null ? false : (bool)attributes["default_policy"]; }
            set { attributes["default_policy"] = value; }
        }

        /// <summary>
        /// When this policy was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// When this policy was last updated.
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
        ///   value - object - Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
        ///   skip_child_site_ids - array(int64) - IDs of child sites excluded from this default policy.
        ///   child_site_ids - array(int64) - IDs of child sites explicitly assigned to this non-default policy.
        ///   default_policy - boolean - Whether this policy applies to child sites not explicitly assigned to another policy.
        ///   policy_type - string - Type of policy.  Valid values: `settings`.
        ///   name - string - Name for this policy.
        ///   description - string - Description for this policy.
        /// </summary>
        public Task<ChildSiteManagementPolicy> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   value - object - Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
        ///   skip_child_site_ids - array(int64) - IDs of child sites excluded from this default policy.
        ///   child_site_ids - array(int64) - IDs of child sites explicitly assigned to this non-default policy.
        ///   default_policy - boolean - Whether this policy applies to child sites not explicitly assigned to another policy.
        ///   policy_type - string - Type of policy.  Valid values: `settings`.
        ///   name - string - Name for this policy.
        ///   description - string - Description for this policy.
        /// </summary>
        public Task<ChildSiteManagementPolicy> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ChildSiteManagementPolicy> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
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
            if (parameters.ContainsKey("value") && !(parameters["value"] is object))
            {
                throw new ArgumentException("Bad parameter: value must be of type object", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("skip_child_site_ids") && !(parameters["skip_child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: skip_child_site_ids must be of type Nullable<Int64>[]", "parameters[\"skip_child_site_ids\"]");
            }
            if (parameters.ContainsKey("child_site_ids") && !(parameters["child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: child_site_ids must be of type Nullable<Int64>[]", "parameters[\"child_site_ids\"]");
            }
            if (parameters.ContainsKey("default_policy") && !(parameters["default_policy"] is bool))
            {
                throw new ArgumentException("Bad parameter: default_policy must be of type bool", "parameters[\"default_policy\"]");
            }
            if (parameters.ContainsKey("policy_type") && !(parameters["policy_type"] is string))
            {
                throw new ArgumentException("Bad parameter: policy_type must be of type string", "parameters[\"policy_type\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/child_site_management_policies/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ChildSiteManagementPolicy>(responseJson, context.Client, requestOptions);
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
            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/child_site_management_policies/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
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
                var newObj = await ChildSiteManagementPolicy.CreateCore(new OperationContext(FilesClient.Bind(ref client)), this.attributes, DictionaryUtil.Copy(this.options), cancellationToken);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<ChildSiteManagementPolicy> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        public static FilesList<ChildSiteManagementPolicy> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<ChildSiteManagementPolicy> ListCore(
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

            return new FilesList<ChildSiteManagementPolicy>(client, $"/child_site_management_policies", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Child Site Management Policy ID.
        /// </summary>
        public static Task<ChildSiteManagementPolicy> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        public static Task<ChildSiteManagementPolicy> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<ChildSiteManagementPolicy> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/child_site_management_policies/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ChildSiteManagementPolicy>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   value - object - Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
        ///   skip_child_site_ids - array(int64) - IDs of child sites excluded from this default policy.
        ///   child_site_ids - array(int64) - IDs of child sites explicitly assigned to this non-default policy.
        ///   default_policy - boolean - Whether this policy applies to child sites not explicitly assigned to another policy.
        ///   policy_type (required) - string - Type of policy.  Valid values: `settings`.
        ///   name - string - Name for this policy.
        ///   description - string - Description for this policy.
        /// </summary>
        public static Task<ChildSiteManagementPolicy> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<ChildSiteManagementPolicy> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("policy_type") || parameters["policy_type"] == null)
            {
                throw new ArgumentNullException("Parameter missing: policy_type", "parameters[\"policy_type\"]");
            }
            if (parameters.ContainsKey("value") && !(parameters["value"] is object))
            {
                throw new ArgumentException("Bad parameter: value must be of type object", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("skip_child_site_ids") && !(parameters["skip_child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: skip_child_site_ids must be of type Nullable<Int64>[]", "parameters[\"skip_child_site_ids\"]");
            }
            if (parameters.ContainsKey("child_site_ids") && !(parameters["child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: child_site_ids must be of type Nullable<Int64>[]", "parameters[\"child_site_ids\"]");
            }
            if (parameters.ContainsKey("default_policy") && !(parameters["default_policy"] is bool))
            {
                throw new ArgumentException("Bad parameter: default_policy must be of type bool", "parameters[\"default_policy\"]");
            }
            if (parameters.ContainsKey("policy_type") && !(parameters["policy_type"] is string))
            {
                throw new ArgumentException("Bad parameter: policy_type must be of type string", "parameters[\"policy_type\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/child_site_management_policies", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ChildSiteManagementPolicy>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   value - object - Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
        ///   skip_child_site_ids - array(int64) - IDs of child sites excluded from this default policy.
        ///   child_site_ids - array(int64) - IDs of child sites explicitly assigned to this non-default policy.
        ///   default_policy - boolean - Whether this policy applies to child sites not explicitly assigned to another policy.
        ///   policy_type - string - Type of policy.  Valid values: `settings`.
        ///   name - string - Name for this policy.
        ///   description - string - Description for this policy.
        /// </summary>
        public static Task<ChildSiteManagementPolicy> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<ChildSiteManagementPolicy> UpdateCore(
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
            if (parameters.ContainsKey("value") && !(parameters["value"] is object))
            {
                throw new ArgumentException("Bad parameter: value must be of type object", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("skip_child_site_ids") && !(parameters["skip_child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: skip_child_site_ids must be of type Nullable<Int64>[]", "parameters[\"skip_child_site_ids\"]");
            }
            if (parameters.ContainsKey("child_site_ids") && !(parameters["child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: child_site_ids must be of type Nullable<Int64>[]", "parameters[\"child_site_ids\"]");
            }
            if (parameters.ContainsKey("default_policy") && !(parameters["default_policy"] is bool))
            {
                throw new ArgumentException("Bad parameter: default_policy must be of type bool", "parameters[\"default_policy\"]");
            }
            if (parameters.ContainsKey("policy_type") && !(parameters["policy_type"] is string))
            {
                throw new ArgumentException("Bad parameter: policy_type must be of type string", "parameters[\"policy_type\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/child_site_management_policies/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<ChildSiteManagementPolicy>(responseJson, context.Client, options);
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

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/child_site_management_policies/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
            response.Dispose();
        }

    }
}