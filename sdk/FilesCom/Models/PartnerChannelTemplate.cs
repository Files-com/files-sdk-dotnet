using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PartnerChannelTemplate : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PartnerChannelTemplate() : this(null, null) { }

        public PartnerChannelTemplate(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("to_partner_folder_name"))
            {
                this.attributes.Add("to_partner_folder_name", null);
            }
            if (!this.attributes.ContainsKey("from_partner_folder_name"))
            {
                this.attributes.Add("from_partner_folder_name", null);
            }
            if (!this.attributes.ContainsKey("from_partner_route_path"))
            {
                this.attributes.Add("from_partner_route_path", null);
            }
            if (!this.attributes.ContainsKey("to_partner_route_path"))
            {
                this.attributes.Add("to_partner_route_path", null);
            }
            if (!this.attributes.ContainsKey("to_partner_managed_folder_paths"))
            {
                this.attributes.Add("to_partner_managed_folder_paths", new string[0]);
            }
            if (!this.attributes.ContainsKey("from_partner_managed_folder_paths"))
            {
                this.attributes.Add("from_partner_managed_folder_paths", new string[0]);
            }
            if (!this.attributes.ContainsKey("effective_to_partner_folder_name"))
            {
                this.attributes.Add("effective_to_partner_folder_name", null);
            }
            if (!this.attributes.ContainsKey("effective_from_partner_folder_name"))
            {
                this.attributes.Add("effective_from_partner_folder_name", null);
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

        void IModel.SetOptions(Dictionary<string, object> options)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// The unique ID of the Partner Channel Template.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// ID of the Workspace associated with this Partner Channel Template.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// The name of the Partner Channel Template.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Channel path relative to the Partner root folder. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Optional Channel-level to-Partner folder name override.
        /// </summary>
        [JsonPropertyName("to_partner_folder_name")]
        public string ToPartnerFolderName
        {
            get { return (string)attributes["to_partner_folder_name"]; }
            set { attributes["to_partner_folder_name"] = value; }
        }

        /// <summary>
        /// Optional Channel-level from-Partner folder name override.
        /// </summary>
        [JsonPropertyName("from_partner_folder_name")]
        public string FromPartnerFolderName
        {
            get { return (string)attributes["from_partner_folder_name"]; }
            set { attributes["from_partner_folder_name"] = value; }
        }

        /// <summary>
        /// Optional route path for files uploaded by the Partner.
        /// </summary>
        [JsonPropertyName("from_partner_route_path")]
        public string FromPartnerRoutePath
        {
            get { return (string)attributes["from_partner_route_path"]; }
            set { attributes["from_partner_route_path"] = value; }
        }

        /// <summary>
        /// Optional route path for files delivered to the Partner.
        /// </summary>
        [JsonPropertyName("to_partner_route_path")]
        public string ToPartnerRoutePath
        {
            get { return (string)attributes["to_partner_route_path"]; }
            set { attributes["to_partner_route_path"] = value; }
        }

        /// <summary>
        /// Managed folder paths inside the to-Partner folder.
        /// </summary>
        [JsonPropertyName("to_partner_managed_folder_paths")]
        public string[] ToPartnerManagedFolderPaths
        {
            get { return (string[])attributes["to_partner_managed_folder_paths"]; }
            set { attributes["to_partner_managed_folder_paths"] = value; }
        }

        /// <summary>
        /// Managed folder paths inside the from-Partner folder.
        /// </summary>
        [JsonPropertyName("from_partner_managed_folder_paths")]
        public string[] FromPartnerManagedFolderPaths
        {
            get { return (string[])attributes["from_partner_managed_folder_paths"]; }
            set { attributes["from_partner_managed_folder_paths"] = value; }
        }

        /// <summary>
        /// Resolved to-Partner folder name after Template override and default.
        /// </summary>
        [JsonPropertyName("effective_to_partner_folder_name")]
        public string EffectiveToPartnerFolderName
        {
            get { return (string)attributes["effective_to_partner_folder_name"]; }
            set { attributes["effective_to_partner_folder_name"] = value; }
        }

        /// <summary>
        /// Resolved from-Partner folder name after Template override and default.
        /// </summary>
        [JsonPropertyName("effective_from_partner_folder_name")]
        public string EffectiveFromPartnerFolderName
        {
            get { return (string)attributes["effective_from_partner_folder_name"]; }
            set { attributes["effective_from_partner_folder_name"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path - string - Optional route path for files uploaded by the Partner.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path - string - Optional route path for files delivered to the Partner.
        ///   name - string - The name of the Partner Channel Template.
        ///   path - string - Channel path relative to the Partner root folder.
        /// </summary>
        public async Task<PartnerChannelTemplate> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("from_partner_folder_name") && !(parameters["from_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_folder_name must be of type string", "parameters[\"from_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("from_partner_managed_folder_paths") && !(parameters["from_partner_managed_folder_paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: from_partner_managed_folder_paths must be of type string[]", "parameters[\"from_partner_managed_folder_paths\"]");
            }
            if (parameters.ContainsKey("from_partner_route_path") && !(parameters["from_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_route_path must be of type string", "parameters[\"from_partner_route_path\"]");
            }
            if (parameters.ContainsKey("to_partner_folder_name") && !(parameters["to_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_folder_name must be of type string", "parameters[\"to_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("to_partner_managed_folder_paths") && !(parameters["to_partner_managed_folder_paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: to_partner_managed_folder_paths must be of type string[]", "parameters[\"to_partner_managed_folder_paths\"]");
            }
            if (parameters.ContainsKey("to_partner_route_path") && !(parameters["to_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_route_path must be of type string", "parameters[\"to_partner_route_path\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partner_channel_templates/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<PartnerChannelTemplate>(responseJson, options);
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

            await FilesClient.SendRequest($"/partner_channel_templates/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await PartnerChannelTemplate.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        public static FilesList<PartnerChannelTemplate> List(

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

            return new FilesList<PartnerChannelTemplate>($"/partner_channel_templates", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<PartnerChannelTemplate> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner Channel Template ID.
        /// </summary>
        public static async Task<PartnerChannelTemplate> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/partner_channel_templates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<PartnerChannelTemplate>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<PartnerChannelTemplate> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path - string - Optional route path for files uploaded by the Partner.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path - string - Optional route path for files delivered to the Partner.
        ///   name (required) - string - The name of the Partner Channel Template.
        ///   path (required) - string - Channel path relative to the Partner root folder.
        ///   workspace_id - int64 - ID of the Workspace associated with this Partner Channel Template.
        /// </summary>
        public static async Task<PartnerChannelTemplate> Create(

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
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("from_partner_folder_name") && !(parameters["from_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_folder_name must be of type string", "parameters[\"from_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("from_partner_managed_folder_paths") && !(parameters["from_partner_managed_folder_paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: from_partner_managed_folder_paths must be of type string[]", "parameters[\"from_partner_managed_folder_paths\"]");
            }
            if (parameters.ContainsKey("from_partner_route_path") && !(parameters["from_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_route_path must be of type string", "parameters[\"from_partner_route_path\"]");
            }
            if (parameters.ContainsKey("to_partner_folder_name") && !(parameters["to_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_folder_name must be of type string", "parameters[\"to_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("to_partner_managed_folder_paths") && !(parameters["to_partner_managed_folder_paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: to_partner_managed_folder_paths must be of type string[]", "parameters[\"to_partner_managed_folder_paths\"]");
            }
            if (parameters.ContainsKey("to_partner_route_path") && !(parameters["to_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_route_path must be of type string", "parameters[\"to_partner_route_path\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partner_channel_templates", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<PartnerChannelTemplate>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path - string - Optional route path for files uploaded by the Partner.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path - string - Optional route path for files delivered to the Partner.
        ///   name - string - The name of the Partner Channel Template.
        ///   path - string - Channel path relative to the Partner root folder.
        /// </summary>
        public static async Task<PartnerChannelTemplate> Update(
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
            if (parameters.ContainsKey("from_partner_folder_name") && !(parameters["from_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_folder_name must be of type string", "parameters[\"from_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("from_partner_managed_folder_paths") && !(parameters["from_partner_managed_folder_paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: from_partner_managed_folder_paths must be of type string[]", "parameters[\"from_partner_managed_folder_paths\"]");
            }
            if (parameters.ContainsKey("from_partner_route_path") && !(parameters["from_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_route_path must be of type string", "parameters[\"from_partner_route_path\"]");
            }
            if (parameters.ContainsKey("to_partner_folder_name") && !(parameters["to_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_folder_name must be of type string", "parameters[\"to_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("to_partner_managed_folder_paths") && !(parameters["to_partner_managed_folder_paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: to_partner_managed_folder_paths must be of type string[]", "parameters[\"to_partner_managed_folder_paths\"]");
            }
            if (parameters.ContainsKey("to_partner_route_path") && !(parameters["to_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_route_path must be of type string", "parameters[\"to_partner_route_path\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partner_channel_templates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<PartnerChannelTemplate>(responseJson, options);
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

            await FilesClient.SendRequest($"/partner_channel_templates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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