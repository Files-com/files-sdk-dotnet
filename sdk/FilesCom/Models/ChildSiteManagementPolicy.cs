using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ChildSiteManagementPolicy
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
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
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
            }
            if (!this.attributes.ContainsKey("site_setting_name"))
            {
                this.attributes.Add("site_setting_name", null);
            }
            if (!this.attributes.ContainsKey("managed_value"))
            {
                this.attributes.Add("managed_value", null);
            }
            if (!this.attributes.ContainsKey("skip_child_site_ids"))
            {
                this.attributes.Add("skip_child_site_ids", new Nullable<Int64>[0]);
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
        /// ChildSiteManagementPolicy ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// ID of the Site managing the policy
        /// </summary>
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            set { attributes["site_id"] = value; }
        }

        /// <summary>
        /// The name of the setting that is managed by the policy
        /// </summary>
        [JsonPropertyName("site_setting_name")]
        public string SiteSettingName
        {
            get { return (string)attributes["site_setting_name"]; }
            set { attributes["site_setting_name"] = value; }
        }

        /// <summary>
        /// The value for the setting that will be enforced for all child sites that are not exempt
        /// </summary>
        [JsonPropertyName("managed_value")]
        public string ManagedValue
        {
            get { return (string)attributes["managed_value"]; }
            set { attributes["managed_value"] = value; }
        }

        /// <summary>
        /// The list of child site IDs that are exempt from this policy
        /// </summary>
        [JsonPropertyName("skip_child_site_ids")]
        public Nullable<Int64>[] SkipChildSiteIds
        {
            get { return (Nullable<Int64>[])attributes["skip_child_site_ids"]; }
            set { attributes["skip_child_site_ids"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   site_setting_name (required) - string - The name of the setting that is managed by the policy
        ///   managed_value (required) - string - The value for the setting that will be enforced for all child sites that are not exempt
        ///   skip_child_site_ids - array(int64) - The list of child site IDs that are exempt from this policy
        /// </summary>
        public async Task<ChildSiteManagementPolicy> Update(Dictionary<string, object> parameters)
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
            if (!parameters.ContainsKey("site_setting_name") || parameters["site_setting_name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: site_setting_name", "parameters[\"site_setting_name\"]");
            }
            if (!parameters.ContainsKey("managed_value") || parameters["managed_value"] == null)
            {
                throw new ArgumentNullException("Parameter missing: managed_value", "parameters[\"managed_value\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("site_setting_name") && !(parameters["site_setting_name"] is string))
            {
                throw new ArgumentException("Bad parameter: site_setting_name must be of type string", "parameters[\"site_setting_name\"]");
            }
            if (parameters.ContainsKey("managed_value") && !(parameters["managed_value"] is string))
            {
                throw new ArgumentException("Bad parameter: managed_value must be of type string", "parameters[\"managed_value\"]");
            }
            if (parameters.ContainsKey("skip_child_site_ids") && !(parameters["skip_child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: skip_child_site_ids must be of type Nullable<Int64>[]", "parameters[\"skip_child_site_ids\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/child_site_management_policies/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ChildSiteManagementPolicy>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/child_site_management_policies/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                await this.Update(this.attributes);
            }
            else
            {
                var newObj = await ChildSiteManagementPolicy.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<ChildSiteManagementPolicy> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
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

            return new FilesList<ChildSiteManagementPolicy>($"/child_site_management_policies", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<ChildSiteManagementPolicy> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Child Site Management Policy ID.
        /// </summary>
        public static async Task<ChildSiteManagementPolicy> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/child_site_management_policies/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ChildSiteManagementPolicy>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<ChildSiteManagementPolicy> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   site_setting_name (required) - string - The name of the setting that is managed by the policy
        ///   managed_value (required) - string - The value for the setting that will be enforced for all child sites that are not exempt
        ///   skip_child_site_ids - array(int64) - The list of child site IDs that are exempt from this policy
        /// </summary>
        public static async Task<ChildSiteManagementPolicy> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("site_setting_name") || parameters["site_setting_name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: site_setting_name", "parameters[\"site_setting_name\"]");
            }
            if (!parameters.ContainsKey("managed_value") || parameters["managed_value"] == null)
            {
                throw new ArgumentNullException("Parameter missing: managed_value", "parameters[\"managed_value\"]");
            }
            if (parameters.ContainsKey("site_setting_name") && !(parameters["site_setting_name"] is string))
            {
                throw new ArgumentException("Bad parameter: site_setting_name must be of type string", "parameters[\"site_setting_name\"]");
            }
            if (parameters.ContainsKey("managed_value") && !(parameters["managed_value"] is string))
            {
                throw new ArgumentException("Bad parameter: managed_value must be of type string", "parameters[\"managed_value\"]");
            }
            if (parameters.ContainsKey("skip_child_site_ids") && !(parameters["skip_child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: skip_child_site_ids must be of type Nullable<Int64>[]", "parameters[\"skip_child_site_ids\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/child_site_management_policies", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ChildSiteManagementPolicy>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   site_setting_name (required) - string - The name of the setting that is managed by the policy
        ///   managed_value (required) - string - The value for the setting that will be enforced for all child sites that are not exempt
        ///   skip_child_site_ids - array(int64) - The list of child site IDs that are exempt from this policy
        /// </summary>
        public static async Task<ChildSiteManagementPolicy> Update(
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
            if (!parameters.ContainsKey("site_setting_name") || parameters["site_setting_name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: site_setting_name", "parameters[\"site_setting_name\"]");
            }
            if (!parameters.ContainsKey("managed_value") || parameters["managed_value"] == null)
            {
                throw new ArgumentNullException("Parameter missing: managed_value", "parameters[\"managed_value\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("site_setting_name") && !(parameters["site_setting_name"] is string))
            {
                throw new ArgumentException("Bad parameter: site_setting_name must be of type string", "parameters[\"site_setting_name\"]");
            }
            if (parameters.ContainsKey("managed_value") && !(parameters["managed_value"] is string))
            {
                throw new ArgumentException("Bad parameter: managed_value must be of type string", "parameters[\"managed_value\"]");
            }
            if (parameters.ContainsKey("skip_child_site_ids") && !(parameters["skip_child_site_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: skip_child_site_ids must be of type Nullable<Int64>[]", "parameters[\"skip_child_site_ids\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/child_site_management_policies/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ChildSiteManagementPolicy>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public static async Task Delete(
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

            await FilesClient.SendRequest($"/child_site_management_policies/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(id, parameters, options);
        }

    }
}