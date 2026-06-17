using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PartnerChannel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PartnerChannel() : this(null, null) { }

        public PartnerChannel(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("partner_id"))
            {
                this.attributes.Add("partner_id", null);
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
            if (!this.attributes.ContainsKey("effective_to_partner_folder_name"))
            {
                this.attributes.Add("effective_to_partner_folder_name", null);
            }
            if (!this.attributes.ContainsKey("effective_from_partner_folder_name"))
            {
                this.attributes.Add("effective_from_partner_folder_name", null);
            }
            if (!this.attributes.ContainsKey("channel_path"))
            {
                this.attributes.Add("channel_path", null);
            }
            if (!this.attributes.ContainsKey("to_partner_folder_path"))
            {
                this.attributes.Add("to_partner_folder_path", null);
            }
            if (!this.attributes.ContainsKey("from_partner_folder_path"))
            {
                this.attributes.Add("from_partner_folder_path", null);
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
        /// The unique ID of the Partner Channel.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// ID of the Workspace associated with this Partner Channel.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// ID of the Partner this Channel belongs to.
        /// </summary>
        [JsonPropertyName("partner_id")]
        public Nullable<Int64> PartnerId
        {
            get { return (Nullable<Int64>)attributes["partner_id"]; }
            set { attributes["partner_id"] = value; }
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
        /// Resolved to-Partner folder name after Channel override and default.
        /// </summary>
        [JsonPropertyName("effective_to_partner_folder_name")]
        public string EffectiveToPartnerFolderName
        {
            get { return (string)attributes["effective_to_partner_folder_name"]; }
            set { attributes["effective_to_partner_folder_name"] = value; }
        }

        /// <summary>
        /// Resolved from-Partner folder name after Channel override and default.
        /// </summary>
        [JsonPropertyName("effective_from_partner_folder_name")]
        public string EffectiveFromPartnerFolderName
        {
            get { return (string)attributes["effective_from_partner_folder_name"]; }
            set { attributes["effective_from_partner_folder_name"] = value; }
        }

        /// <summary>
        /// Resolved Channel folder path.
        /// </summary>
        [JsonPropertyName("channel_path")]
        public string ChannelPath
        {
            get { return (string)attributes["channel_path"]; }
            set { attributes["channel_path"] = value; }
        }

        /// <summary>
        /// Resolved to-Partner folder path.
        /// </summary>
        [JsonPropertyName("to_partner_folder_path")]
        public string ToPartnerFolderPath
        {
            get { return (string)attributes["to_partner_folder_path"]; }
            set { attributes["to_partner_folder_path"] = value; }
        }

        /// <summary>
        /// Resolved from-Partner folder path.
        /// </summary>
        [JsonPropertyName("from_partner_folder_path")]
        public string FromPartnerFolderPath
        {
            get { return (string)attributes["from_partner_folder_path"]; }
            set { attributes["from_partner_folder_path"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_route_path - string - Optional route path for files uploaded by the Partner.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_route_path - string - Optional route path for files delivered to the Partner.
        ///   path - string - Channel path relative to the Partner root folder.
        /// </summary>
        public async Task<PartnerChannel> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("from_partner_route_path") && !(parameters["from_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_route_path must be of type string", "parameters[\"from_partner_route_path\"]");
            }
            if (parameters.ContainsKey("to_partner_folder_name") && !(parameters["to_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_folder_name must be of type string", "parameters[\"to_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("to_partner_route_path") && !(parameters["to_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_route_path must be of type string", "parameters[\"to_partner_route_path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partner_channels/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<PartnerChannel>(responseJson, JsonUtil.Options);
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

            await FilesClient.SendRequest($"/partner_channels/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await PartnerChannel.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `path` or `partner_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `partner_id` and `workspace_id`. Valid field combinations are `[ workspace_id, partner_id ]`.
        /// </summary>
        public static FilesList<PartnerChannel> List(

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

            return new FilesList<PartnerChannel>($"/partner_channels", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<PartnerChannel> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner Channel ID.
        /// </summary>
        public static async Task<PartnerChannel> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/partner_channels/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<PartnerChannel>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<PartnerChannel> Get(
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
        ///   from_partner_route_path - string - Optional route path for files uploaded by the Partner.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_route_path - string - Optional route path for files delivered to the Partner.
        ///   partner_id (required) - int64 - ID of the Partner this Channel belongs to.
        ///   path (required) - string - Channel path relative to the Partner root folder.
        ///   workspace_id - int64 - ID of the Workspace associated with this Partner Channel.
        /// </summary>
        public static async Task<PartnerChannel> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("partner_id") || parameters["partner_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: partner_id", "parameters[\"partner_id\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("from_partner_folder_name") && !(parameters["from_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_folder_name must be of type string", "parameters[\"from_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("from_partner_route_path") && !(parameters["from_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_route_path must be of type string", "parameters[\"from_partner_route_path\"]");
            }
            if (parameters.ContainsKey("to_partner_folder_name") && !(parameters["to_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_folder_name must be of type string", "parameters[\"to_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("to_partner_route_path") && !(parameters["to_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_route_path must be of type string", "parameters[\"to_partner_route_path\"]");
            }
            if (parameters.ContainsKey("partner_id") && !(parameters["partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: partner_id must be of type Nullable<Int64>", "parameters[\"partner_id\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partner_channels", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<PartnerChannel>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_route_path - string - Optional route path for files uploaded by the Partner.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_route_path - string - Optional route path for files delivered to the Partner.
        ///   path - string - Channel path relative to the Partner root folder.
        /// </summary>
        public static async Task<PartnerChannel> Update(
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
            if (parameters.ContainsKey("from_partner_route_path") && !(parameters["from_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: from_partner_route_path must be of type string", "parameters[\"from_partner_route_path\"]");
            }
            if (parameters.ContainsKey("to_partner_folder_name") && !(parameters["to_partner_folder_name"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_folder_name must be of type string", "parameters[\"to_partner_folder_name\"]");
            }
            if (parameters.ContainsKey("to_partner_route_path") && !(parameters["to_partner_route_path"] is string))
            {
                throw new ArgumentException("Bad parameter: to_partner_route_path must be of type string", "parameters[\"to_partner_route_path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partner_channels/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<PartnerChannel>(responseJson, JsonUtil.Options);
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

            await FilesClient.SendRequest($"/partner_channels/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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