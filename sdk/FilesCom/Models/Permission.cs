using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Permission
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Permission() : this(null, null) { }

        public Permission(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("group_id"))
            {
                this.attributes.Add("group_id", null);
            }
            if (!this.attributes.ContainsKey("group_name"))
            {
                this.attributes.Add("group_name", null);
            }
            if (!this.attributes.ContainsKey("permission"))
            {
                this.attributes.Add("permission", null);
            }
            if (!this.attributes.ContainsKey("recursive"))
            {
                this.attributes.Add("recursive", false);
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
        /// Permission ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// User's username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// Group ID
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>)attributes["group_id"]; }
            set { attributes["group_id"] = value; }
        }

        /// <summary>
        /// Group name if applicable
        /// </summary>
        [JsonPropertyName("group_name")]
        public string GroupName
        {
            get { return (string)attributes["group_name"]; }
            set { attributes["group_name"] = value; }
        }

        /// <summary>
        /// Permission type
        /// </summary>
        [JsonPropertyName("permission")]
        public string PermissionType
        {
            get { return (string)attributes["permission"]; }
            set { attributes["permission"] = value; }
        }

        /// <summary>
        /// Does this permission apply to subfolders?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("recursive")]
        public bool Recursive
        {
            get { return attributes["recursive"] == null ? false : (bool)attributes["recursive"]; }
            set { attributes["recursive"] = value; }
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
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/permissions/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The Permission object doesn't support updates.");
            }
            else
            {
                var newObj = await Permission.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[group_id]=desc`). Valid fields are `group_id`, `path`, `user_id` or `permission`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `group_id` or `user_id`. Valid field combinations are `[ group_id, path ]`, `[ user_id, path ]` or `[ user_id, group_id, path ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        ///   path - string - Permission path.  If provided, will scope all permissions(including upward) to this path.
        ///   include_groups - boolean - If searching by user or group, also include user's permissions that are inherited from its groups?
        ///   group_id - string
        ///   user_id - string
        /// </summary>
        public static FilesList<Permission> List(

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
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("include_groups") && !(parameters["include_groups"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_groups must be of type bool", "parameters[\"include_groups\"]");
            }
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is string))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type string", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is string))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type string", "parameters[\"user_id\"]");
            }

            return new FilesList<Permission>($"/permissions", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Permission> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   group_id - int64 - Group ID
        ///   path - string - Folder path
        ///   permission - string -  Permission type.  Can be `admin`, `full`, `readonly`, `writeonly`, `list`, or `history`
        ///   recursive - boolean - Apply to subfolders recursively?
        ///   user_id - int64 - User ID.  Provide `username` or `user_id`
        ///   username - string - User username.  Provide `username` or `user_id`
        /// </summary>
        public static async Task<Permission> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("permission") && !(parameters["permission"] is string))
            {
                throw new ArgumentException("Bad parameter: permission must be of type string", "parameters[\"permission\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/permissions", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Permission>(responseJson);
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

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/permissions/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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