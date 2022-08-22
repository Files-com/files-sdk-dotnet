using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ApiKey
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ApiKey() : this(null, null) { }

        public ApiKey(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("descriptive_label"))
            {
                this.attributes.Add("descriptive_label", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("expires_at"))
            {
                this.attributes.Add("expires_at", null);
            }
            if (!this.attributes.ContainsKey("key"))
            {
                this.attributes.Add("key", null);
            }
            if (!this.attributes.ContainsKey("last_use_at"))
            {
                this.attributes.Add("last_use_at", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("permission_set"))
            {
                this.attributes.Add("permission_set", null);
            }
            if (!this.attributes.ContainsKey("platform"))
            {
                this.attributes.Add("platform", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
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
        /// API Key ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Unique label that describes this API key.  Useful for external systems where you may have API keys from multiple accounts and want a human-readable label for each key.
        /// </summary>
        [JsonPropertyName("descriptive_label")]
        public string DescriptiveLabel
        {
            get { return (string) attributes["descriptive_label"]; }
            set { attributes["descriptive_label"] = value; }
        }

        /// <summary>
        /// Time which API Key was created
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// API Key expiration date
        /// </summary>
        [JsonPropertyName("expires_at")]
        public Nullable<DateTime> ExpiresAt
        {
            get { return (Nullable<DateTime>) attributes["expires_at"]; }
            set { attributes["expires_at"] = value; }
        }

        /// <summary>
        /// API Key actual key string
        /// </summary>
        [JsonPropertyName("key")]
        public string Key
        {
            get { return (string) attributes["key"]; }
            set { attributes["key"] = value; }
        }

        /// <summary>
        /// API Key last used - note this value is only updated once per 3 hour period, so the 'actual' time of last use may be up to 3 hours later than this timestamp.
        /// </summary>
        [JsonPropertyName("last_use_at")]
        public Nullable<DateTime> LastUseAt
        {
            get { return (Nullable<DateTime>) attributes["last_use_at"]; }
            set { attributes["last_use_at"] = value; }
        }

        /// <summary>
        /// Internal name for the API Key.  For your use.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string) attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Folder path restriction for this api key. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Permissions for this API Key.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations).  Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
        /// </summary>
        [JsonPropertyName("permission_set")]
        public string PermissionSet
        {
            get { return (string) attributes["permission_set"]; }
            set { attributes["permission_set"] = value; }
        }

        /// <summary>
        /// If this API key represents a Desktop app, what platform was it created on?
        /// </summary>
        [JsonPropertyName("platform")]
        public string Platform
        {
            get { return (string) attributes["platform"]; }
            set { attributes["platform"] = value; }
        }

        /// <summary>
        /// User ID for the owner of this API Key.  May be blank for Site-wide API Keys.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Internal name for the API Key.  For your use.
        ///   expires_at - string - API Key expiration date
        ///   permission_set - string - Permissions for this API Key.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations).  Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
        /// </summary>
        public async Task<ApiKey> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id")) {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("expires_at") && !(parameters["expires_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: expires_at must be of type string", "parameters[\"expires_at\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string ))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/api_keys/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<ApiKey> Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id")) {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/api_keys/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
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
                var newObj = await ApiKey.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `expires_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `expires_at`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `expires_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal to the supplied value. Valid fields are `expires_at`.
        ///   filter_like - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `expires_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `expires_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal to the supplied value. Valid fields are `expires_at`.
        /// </summary>
        public static async Task<ApiKey[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object ))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_like") && !(parameters["filter_like"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_like must be of type object", "parameters[\"filter_like\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/api_keys", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<ApiKey[]>(responseJson);
        }

        public static async Task<ApiKey[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// </summary>
        public static async Task<ApiKey> FindCurrent(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendRequest($"/api_key", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Api Key ID.
        /// </summary>
        public static async Task<ApiKey> Find(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/api_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
        }

        public static async Task<ApiKey> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   name - string - Internal name for the API Key.  For your use.
        ///   expires_at - string - API Key expiration date
        ///   permission_set - string - Permissions for this API Key.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations).  Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
        ///   path - string - Folder path restriction for this api key.
        /// </summary>
        public static async Task<ApiKey> Create(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("expires_at") && !(parameters["expires_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: expires_at must be of type string", "parameters[\"expires_at\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string ))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/api_keys", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   expires_at - string - API Key expiration date
        ///   name - string - Internal name for the API Key.  For your use.
        ///   permission_set - string - Permissions for this API Key.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations).  Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
        /// </summary>
        public static async Task<ApiKey> UpdateCurrent(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("expires_at") && !(parameters["expires_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: expires_at must be of type string", "parameters[\"expires_at\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string ))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/api_key", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Internal name for the API Key.  For your use.
        ///   expires_at - string - API Key expiration date
        ///   permission_set - string - Permissions for this API Key.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations).  Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
        /// </summary>
        public static async Task<ApiKey> Update(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("expires_at") && !(parameters["expires_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: expires_at must be of type string", "parameters[\"expires_at\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string ))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/api_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<ApiKey> DeleteCurrent(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendRequest($"/api_key", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<ApiKey> Delete(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/api_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<ApiKey>(responseJson);
        }

        public static async Task<ApiKey> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

