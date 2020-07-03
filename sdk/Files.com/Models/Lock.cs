using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Lock
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;

        public static Lock Create(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            return Lock.Create((string)attributes["path"], attributes, options);
        }

        public Lock()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("path", null);
            this.attributes.Add("timeout", null);
            this.attributes.Add("depth", null);
            this.attributes.Add("owner", null);
            this.attributes.Add("scope", null);
            this.attributes.Add("token", null);
            this.attributes.Add("type", null);
            this.attributes.Add("user_id", null);
            this.attributes.Add("username", null);
        }

        public Lock(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Lock timeout
        /// </summary>
        [JsonPropertyName("timeout")]
        public Nullable<Int64> Timeout
        {
            get { return (Nullable<Int64>) attributes["timeout"]; }
            set { attributes["timeout"] = value; }
        }

        /// <summary>
        /// Lock depth (0 or infinity)
        /// </summary>
        [JsonPropertyName("depth")]
        public string Depth
        {
            get { return (string) attributes["depth"]; }
            set { attributes["depth"] = value; }
        }

        /// <summary>
        /// Owner of lock.  This can be any arbitrary string.
        /// </summary>
        [JsonPropertyName("owner")]
        public string Owner
        {
            get { return (string) attributes["owner"]; }
            set { attributes["owner"] = value; }
        }

        /// <summary>
        /// Lock scope(shared or exclusive)
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope
        {
            get { return (string) attributes["scope"]; }
            set { attributes["scope"] = value; }
        }

        /// <summary>
        /// Lock token.  Use to release lock.
        /// </summary>
        [JsonPropertyName("token")]
        public string Token
        {
            get { return (string) attributes["token"]; }
            set { attributes["token"] = value; }
        }

        /// <summary>
        /// Lock type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string) attributes["type"]; }
            set { attributes["type"] = value; }
        }

        /// <summary>
        /// Lock creator user ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Lock creator username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string) attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   token (required) - string - Lock token
        /// </summary>
        public async Task<Lock> Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path")) {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("token") && !(parameters["token"] is string ))
            {
                throw new ArgumentException("Bad parameter: token must be of type string", "parameters[\"token\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("token") || parameters["token"] == null)
            {
                throw new ArgumentNullException("Parameter missing: token", "parameters[\"token\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/locks/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Lock>(responseJson);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["path"] != null)
            {
                throw new NotImplementedException("The Lock object doesn't support updates.");
            }
            else
            {
                // TODO: Remove this cast after return types are implemented
                var newObj = (Lock) await Lock.Create(this.attributes, this.options);
                this.attributes = newObj.attributes;
            }
        }

        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        ///   path (required) - string - Path to operate on.
        ///   include_children - boolean - Include locks from children objects?
        /// </summary>
        public static async Task<Lock[]> ListFor(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
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
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("include_children") && !(parameters["include_children"] is bool ))
            {
                throw new ArgumentException("Bad parameter: include_children must be of type bool", "parameters[\"include_children\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/locks/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Lock[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path
        ///   timeout - int64 - Lock timeout length
        /// </summary>
        public static async Task<Lock> Create(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("timeout") && !(parameters["timeout"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: timeout must be of type Nullable<Int64>", "parameters[\"timeout\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/locks/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Lock>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   token (required) - string - Lock token
        /// </summary>
        public static async Task<Lock> Delete(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("token") && !(parameters["token"] is string ))
            {
                throw new ArgumentException("Bad parameter: token must be of type string", "parameters[\"token\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("token") || parameters["token"] == null)
            {
                throw new ArgumentNullException("Parameter missing: token", "parameters[\"token\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/locks/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Lock>(responseJson);
        }

        public static async Task<Lock> Destroy(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(path, parameters, options);
        }

    }
}

