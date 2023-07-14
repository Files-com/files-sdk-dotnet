using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Lock
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Lock() : this(null, null) { }

        public Lock(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("timeout"))
            {
                this.attributes.Add("timeout", null);
            }
            if (!this.attributes.ContainsKey("depth"))
            {
                this.attributes.Add("depth", null);
            }
            if (!this.attributes.ContainsKey("recursive"))
            {
                this.attributes.Add("recursive", false);
            }
            if (!this.attributes.ContainsKey("owner"))
            {
                this.attributes.Add("owner", null);
            }
            if (!this.attributes.ContainsKey("scope"))
            {
                this.attributes.Add("scope", null);
            }
            if (!this.attributes.ContainsKey("exclusive"))
            {
                this.attributes.Add("exclusive", false);
            }
            if (!this.attributes.ContainsKey("token"))
            {
                this.attributes.Add("token", null);
            }
            if (!this.attributes.ContainsKey("type"))
            {
                this.attributes.Add("type", null);
            }
            if (!this.attributes.ContainsKey("allow_access_by_any_user"))
            {
                this.attributes.Add("allow_access_by_any_user", false);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
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
        /// Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Lock timeout in seconds
        /// </summary>
        [JsonPropertyName("timeout")]
        public Nullable<Int64> Timeout
        {
            get { return (Nullable<Int64>)attributes["timeout"]; }
            set { attributes["timeout"] = value; }
        }

        /// <summary>
        /// DEPRECATED: Lock depth
        /// </summary>
        [JsonPropertyName("depth")]
        public string Depth
        {
            get { return (string)attributes["depth"]; }
            set { attributes["depth"] = value; }
        }

        /// <summary>
        /// Does lock apply to subfolders?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("recursive")]
        public bool Recursive
        {
            get { return attributes["recursive"] == null ? false : (bool)attributes["recursive"]; }
            set { attributes["recursive"] = value; }
        }

        /// <summary>
        /// Owner of the lock.  This can be any arbitrary string.
        /// </summary>
        [JsonPropertyName("owner")]
        public string Owner
        {
            get { return (string)attributes["owner"]; }
            set { attributes["owner"] = value; }
        }

        /// <summary>
        /// DEPRECATED: Lock scope
        /// </summary>
        [JsonPropertyName("scope")]
        public string Scope
        {
            get { return (string)attributes["scope"]; }
            set { attributes["scope"] = value; }
        }

        /// <summary>
        /// Is lock exclusive?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("exclusive")]
        public bool Exclusive
        {
            get { return attributes["exclusive"] == null ? false : (bool)attributes["exclusive"]; }
            set { attributes["exclusive"] = value; }
        }

        /// <summary>
        /// Lock token.  Use to release lock.
        /// </summary>
        [JsonPropertyName("token")]
        public string Token
        {
            get { return (string)attributes["token"]; }
            set { attributes["token"] = value; }
        }

        /// <summary>
        /// DEPRECATED: Lock type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string)attributes["type"]; }
            set { attributes["type"] = value; }
        }

        /// <summary>
        /// Can lock be modified by users other than its creator?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_access_by_any_user")]
        public bool AllowAccessByAnyUser
        {
            get { return attributes["allow_access_by_any_user"] == null ? false : (bool)attributes["allow_access_by_any_user"]; }
            set { attributes["allow_access_by_any_user"] = value; }
        }

        /// <summary>
        /// Lock creator user ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Lock creator username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   token (required) - string - Lock token
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("token") && !(parameters["token"] is string))
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

            await FilesClient.SendRequest($"/locks/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            var newObj = await Lock.Create(Path, this.attributes, this.options);
            this.attributes = newObj.getAttributes();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
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
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("include_children") && !(parameters["include_children"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_children must be of type bool", "parameters[\"include_children\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/locks/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Lock[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path
        ///   allow_access_by_any_user - boolean - Allow lock to be updated by any user?
        ///   exclusive - boolean - Is lock exclusive?
        ///   recursive - string - Does lock apply to subfolders?
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
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("allow_access_by_any_user") && !(parameters["allow_access_by_any_user"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_access_by_any_user must be of type bool", "parameters[\"allow_access_by_any_user\"]");
            }
            if (parameters.ContainsKey("exclusive") && !(parameters["exclusive"] is bool))
            {
                throw new ArgumentException("Bad parameter: exclusive must be of type bool", "parameters[\"exclusive\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is string))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type string", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("timeout") && !(parameters["timeout"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: timeout must be of type Nullable<Int64>", "parameters[\"timeout\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/locks/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Lock>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   token (required) - string - Lock token
        /// </summary>
        public static async Task Delete(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("token") && !(parameters["token"] is string))
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

            await FilesClient.SendRequest($"/locks/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(path, parameters, options);
        }

    }
}