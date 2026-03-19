using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class DesktopConfigurationProfile
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public DesktopConfigurationProfile() : this(null, null) { }

        public DesktopConfigurationProfile(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("use_for_all_users"))
            {
                this.attributes.Add("use_for_all_users", false);
            }
            if (!this.attributes.ContainsKey("mount_mappings"))
            {
                this.attributes.Add("mount_mappings", null);
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
        /// Desktop Configuration Profile ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Profile name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Workspace ID
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// Whether this profile applies to all users in the Workspace by default
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("use_for_all_users")]
        public bool UseForAllUsers
        {
            get { return attributes["use_for_all_users"] == null ? false : (bool)attributes["use_for_all_users"]; }
            set { attributes["use_for_all_users"] = value; }
        }

        /// <summary>
        /// Mount point mappings for the desktop app. Keys are mount points (e.g. drive letters) and values are paths in Files.com that the mount points map to.
        /// </summary>
        [JsonPropertyName("mount_mappings")]
        public object MountMappings
        {
            get { return (object)attributes["mount_mappings"]; }
            set { attributes["mount_mappings"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Profile name
        ///   workspace_id - int64 - Workspace ID
        ///   mount_mappings - object - Mount point mappings for the desktop app. Keys are mount points (e.g. drive letters) and values are paths in Files.com that the mount points map to.
        ///   use_for_all_users - boolean - Whether this profile applies to all users in the Workspace by default
        /// </summary>
        public async Task<DesktopConfigurationProfile> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("mount_mappings") && !(parameters["mount_mappings"] is object))
            {
                throw new ArgumentException("Bad parameter: mount_mappings must be of type object", "parameters[\"mount_mappings\"]");
            }
            if (parameters.ContainsKey("use_for_all_users") && !(parameters["use_for_all_users"] is bool))
            {
                throw new ArgumentException("Bad parameter: use_for_all_users must be of type bool", "parameters[\"use_for_all_users\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/desktop_configuration_profiles/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<DesktopConfigurationProfile>(responseJson, JsonUtil.Options);
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

            await FilesClient.SendRequest($"/desktop_configuration_profiles/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await DesktopConfigurationProfile.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        public static FilesList<DesktopConfigurationProfile> List(

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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }

            return new FilesList<DesktopConfigurationProfile>($"/desktop_configuration_profiles", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<DesktopConfigurationProfile> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Desktop Configuration Profile ID.
        /// </summary>
        public static async Task<DesktopConfigurationProfile> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/desktop_configuration_profiles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<DesktopConfigurationProfile>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<DesktopConfigurationProfile> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name (required) - string - Profile name
        ///   mount_mappings (required) - object - Mount point mappings for the desktop app. Keys are mount points (e.g. drive letters) and values are paths in Files.com that the mount points map to.
        ///   workspace_id - int64 - Workspace ID
        ///   use_for_all_users - boolean - Whether this profile applies to all users in the Workspace by default
        /// </summary>
        public static async Task<DesktopConfigurationProfile> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("name") || parameters["name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: name", "parameters[\"name\"]");
            }
            if (!parameters.ContainsKey("mount_mappings") || parameters["mount_mappings"] == null)
            {
                throw new ArgumentNullException("Parameter missing: mount_mappings", "parameters[\"mount_mappings\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("mount_mappings") && !(parameters["mount_mappings"] is object))
            {
                throw new ArgumentException("Bad parameter: mount_mappings must be of type object", "parameters[\"mount_mappings\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("use_for_all_users") && !(parameters["use_for_all_users"] is bool))
            {
                throw new ArgumentException("Bad parameter: use_for_all_users must be of type bool", "parameters[\"use_for_all_users\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/desktop_configuration_profiles", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<DesktopConfigurationProfile>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Profile name
        ///   workspace_id - int64 - Workspace ID
        ///   mount_mappings - object - Mount point mappings for the desktop app. Keys are mount points (e.g. drive letters) and values are paths in Files.com that the mount points map to.
        ///   use_for_all_users - boolean - Whether this profile applies to all users in the Workspace by default
        /// </summary>
        public static async Task<DesktopConfigurationProfile> Update(
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("mount_mappings") && !(parameters["mount_mappings"] is object))
            {
                throw new ArgumentException("Bad parameter: mount_mappings must be of type object", "parameters[\"mount_mappings\"]");
            }
            if (parameters.ContainsKey("use_for_all_users") && !(parameters["use_for_all_users"] is bool))
            {
                throw new ArgumentException("Bad parameter: use_for_all_users must be of type bool", "parameters[\"use_for_all_users\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/desktop_configuration_profiles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<DesktopConfigurationProfile>(responseJson, JsonUtil.Options);
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

            await FilesClient.SendRequest($"/desktop_configuration_profiles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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