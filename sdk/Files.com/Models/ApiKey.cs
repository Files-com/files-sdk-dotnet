using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class ApiKey
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ApiKey()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("descriptive_label", null);
            this.attributes.Add("created_at", null);
            this.attributes.Add("expires_at", null);
            this.attributes.Add("key", null);
            this.attributes.Add("last_use_at", null);
            this.attributes.Add("name", null);
            this.attributes.Add("permission_set", null);
            this.attributes.Add("platform", null);
            this.attributes.Add("user_id", null);
        }

        public ApiKey(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
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
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
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

            string responseJson = await FilesClient.SendRequest($"/api_keys/{attributes["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

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

            string responseJson = await FilesClient.SendRequest($"/api_keys/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

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
                // TODO: Remove this cast after return types are implemented
                var newObj = (ApiKey) await ApiKey.Create(this.attributes, this.options);
                this.attributes = newObj.attributes;
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
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
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
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

            string responseJson = await FilesClient.SendRequest($"/api_keys/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

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

            string responseJson = await FilesClient.SendRequest($"/api_key", System.Net.Http.HttpMethod.Patch, parameters, options);

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

            string responseJson = await FilesClient.SendRequest($"/api_keys/{parameters["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

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

            string responseJson = await FilesClient.SendRequest($"/api_keys/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

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

