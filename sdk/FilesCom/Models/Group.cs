using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Group
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Group() : this(null, null) { }

        public Group(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("allowed_ips"))
            {
                this.attributes.Add("allowed_ips", null);
            }
            if (!this.attributes.ContainsKey("admin_ids"))
            {
                this.attributes.Add("admin_ids", null);
            }
            if (!this.attributes.ContainsKey("notes"))
            {
                this.attributes.Add("notes", null);
            }
            if (!this.attributes.ContainsKey("user_ids"))
            {
                this.attributes.Add("user_ids", null);
            }
            if (!this.attributes.ContainsKey("usernames"))
            {
                this.attributes.Add("usernames", null);
            }
            if (!this.attributes.ContainsKey("ftp_permission"))
            {
                this.attributes.Add("ftp_permission", false);
            }
            if (!this.attributes.ContainsKey("sftp_permission"))
            {
                this.attributes.Add("sftp_permission", false);
            }
            if (!this.attributes.ContainsKey("dav_permission"))
            {
                this.attributes.Add("dav_permission", false);
            }
            if (!this.attributes.ContainsKey("restapi_permission"))
            {
                this.attributes.Add("restapi_permission", false);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
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
        /// Group ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Group name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// A list of allowed IPs if applicable.  Newline delimited
        /// </summary>
        [JsonPropertyName("allowed_ips")]
        public string AllowedIps
        {
            get { return (string)attributes["allowed_ips"]; }
            set { attributes["allowed_ips"] = value; }
        }

        /// <summary>
        /// Comma-delimited list of user IDs who are group administrators (separated by commas)
        /// </summary>
        [JsonPropertyName("admin_ids")]
        public string AdminIds
        {
            get { return (string)attributes["admin_ids"]; }
            set { attributes["admin_ids"] = value; }
        }

        /// <summary>
        /// Notes about this group
        /// </summary>
        [JsonPropertyName("notes")]
        public string Notes
        {
            get { return (string)attributes["notes"]; }
            set { attributes["notes"] = value; }
        }

        /// <summary>
        /// Comma-delimited list of user IDs who belong to this group (separated by commas)
        /// </summary>
        [JsonPropertyName("user_ids")]
        public string UserIds
        {
            get { return (string)attributes["user_ids"]; }
            set { attributes["user_ids"] = value; }
        }

        /// <summary>
        /// Comma-delimited list of usernames who belong to this group (separated by commas)
        /// </summary>
        [JsonPropertyName("usernames")]
        public string Usernames
        {
            get { return (string)attributes["usernames"]; }
            set { attributes["usernames"] = value; }
        }

        /// <summary>
        /// If true, users in this group can use FTP to login.  This will override a false value of `ftp_permission` on the user level.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ftp_permission")]
        public bool FtpPermission
        {
            get { return attributes["ftp_permission"] == null ? false : (bool)attributes["ftp_permission"]; }
            set { attributes["ftp_permission"] = value; }
        }

        /// <summary>
        /// If true, users in this group can use SFTP to login.  This will override a false value of `sftp_permission` on the user level.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_permission")]
        public bool SftpPermission
        {
            get { return attributes["sftp_permission"] == null ? false : (bool)attributes["sftp_permission"]; }
            set { attributes["sftp_permission"] = value; }
        }

        /// <summary>
        /// If true, users in this group can use WebDAV to login.  This will override a false value of `dav_permission` on the user level.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dav_permission")]
        public bool DavPermission
        {
            get { return attributes["dav_permission"] == null ? false : (bool)attributes["dav_permission"]; }
            set { attributes["dav_permission"] = value; }
        }

        /// <summary>
        /// If true, users in this group can use the REST API to login.  This will override a false value of `restapi_permission` on the user level.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("restapi_permission")]
        public bool RestapiPermission
        {
            get { return attributes["restapi_permission"] == null ? false : (bool)attributes["restapi_permission"]; }
            set { attributes["restapi_permission"] = value; }
        }

        /// <summary>
        /// Site ID
        /// </summary>
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            set { attributes["site_id"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   notes - string - Group notes.
        ///   user_ids - string - A list of user ids. If sent as a string, should be comma-delimited.
        ///   admin_ids - string - A list of group admin user ids. If sent as a string, should be comma-delimited.
        ///   ftp_permission - boolean - If true, users in this group can use FTP to login.  This will override a false value of `ftp_permission` on the user level.
        ///   sftp_permission - boolean - If true, users in this group can use SFTP to login.  This will override a false value of `sftp_permission` on the user level.
        ///   dav_permission - boolean - If true, users in this group can use WebDAV to login.  This will override a false value of `dav_permission` on the user level.
        ///   restapi_permission - boolean - If true, users in this group can use the REST API to login.  This will override a false value of `restapi_permission` on the user level.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   name - string - Group name.
        /// </summary>
        public async Task<Group> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }
            if (parameters.ContainsKey("ftp_permission") && !(parameters["ftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_permission must be of type bool", "parameters[\"ftp_permission\"]");
            }
            if (parameters.ContainsKey("sftp_permission") && !(parameters["sftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_permission must be of type bool", "parameters[\"sftp_permission\"]");
            }
            if (parameters.ContainsKey("dav_permission") && !(parameters["dav_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: dav_permission must be of type bool", "parameters[\"dav_permission\"]");
            }
            if (parameters.ContainsKey("restapi_permission") && !(parameters["restapi_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: restapi_permission must be of type bool", "parameters[\"restapi_permission\"]");
            }
            if (parameters.ContainsKey("allowed_ips") && !(parameters["allowed_ips"] is string))
            {
                throw new ArgumentException("Bad parameter: allowed_ips must be of type string", "parameters[\"allowed_ips\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/groups/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Group>(responseJson);
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

            await FilesClient.SendRequest($"/groups/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await Group.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `name`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
        ///   ids - string - Comma-separated list of group ids to include in results.
        ///   include_parent_site_groups - boolean - Include groups from the parent site.
        /// </summary>
        public static FilesList<Group> List(

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
            if (parameters.ContainsKey("ids") && !(parameters["ids"] is string))
            {
                throw new ArgumentException("Bad parameter: ids must be of type string", "parameters[\"ids\"]");
            }
            if (parameters.ContainsKey("include_parent_site_groups") && !(parameters["include_parent_site_groups"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_parent_site_groups must be of type bool", "parameters[\"include_parent_site_groups\"]");
            }

            return new FilesList<Group>($"/groups", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Group> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Group ID.
        /// </summary>
        public static async Task<Group> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/groups/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Group>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<Group> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   notes - string - Group notes.
        ///   user_ids - string - A list of user ids. If sent as a string, should be comma-delimited.
        ///   admin_ids - string - A list of group admin user ids. If sent as a string, should be comma-delimited.
        ///   ftp_permission - boolean - If true, users in this group can use FTP to login.  This will override a false value of `ftp_permission` on the user level.
        ///   sftp_permission - boolean - If true, users in this group can use SFTP to login.  This will override a false value of `sftp_permission` on the user level.
        ///   dav_permission - boolean - If true, users in this group can use WebDAV to login.  This will override a false value of `dav_permission` on the user level.
        ///   restapi_permission - boolean - If true, users in this group can use the REST API to login.  This will override a false value of `restapi_permission` on the user level.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   name (required) - string - Group name.
        /// </summary>
        public static async Task<Group> Create(

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
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }
            if (parameters.ContainsKey("ftp_permission") && !(parameters["ftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_permission must be of type bool", "parameters[\"ftp_permission\"]");
            }
            if (parameters.ContainsKey("sftp_permission") && !(parameters["sftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_permission must be of type bool", "parameters[\"sftp_permission\"]");
            }
            if (parameters.ContainsKey("dav_permission") && !(parameters["dav_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: dav_permission must be of type bool", "parameters[\"dav_permission\"]");
            }
            if (parameters.ContainsKey("restapi_permission") && !(parameters["restapi_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: restapi_permission must be of type bool", "parameters[\"restapi_permission\"]");
            }
            if (parameters.ContainsKey("allowed_ips") && !(parameters["allowed_ips"] is string))
            {
                throw new ArgumentException("Bad parameter: allowed_ips must be of type string", "parameters[\"allowed_ips\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/groups", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Group>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   notes - string - Group notes.
        ///   user_ids - string - A list of user ids. If sent as a string, should be comma-delimited.
        ///   admin_ids - string - A list of group admin user ids. If sent as a string, should be comma-delimited.
        ///   ftp_permission - boolean - If true, users in this group can use FTP to login.  This will override a false value of `ftp_permission` on the user level.
        ///   sftp_permission - boolean - If true, users in this group can use SFTP to login.  This will override a false value of `sftp_permission` on the user level.
        ///   dav_permission - boolean - If true, users in this group can use WebDAV to login.  This will override a false value of `dav_permission` on the user level.
        ///   restapi_permission - boolean - If true, users in this group can use the REST API to login.  This will override a false value of `restapi_permission` on the user level.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   name - string - Group name.
        /// </summary>
        public static async Task<Group> Update(
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
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }
            if (parameters.ContainsKey("ftp_permission") && !(parameters["ftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_permission must be of type bool", "parameters[\"ftp_permission\"]");
            }
            if (parameters.ContainsKey("sftp_permission") && !(parameters["sftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_permission must be of type bool", "parameters[\"sftp_permission\"]");
            }
            if (parameters.ContainsKey("dav_permission") && !(parameters["dav_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: dav_permission must be of type bool", "parameters[\"dav_permission\"]");
            }
            if (parameters.ContainsKey("restapi_permission") && !(parameters["restapi_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: restapi_permission must be of type bool", "parameters[\"restapi_permission\"]");
            }
            if (parameters.ContainsKey("allowed_ips") && !(parameters["allowed_ips"] is string))
            {
                throw new ArgumentException("Bad parameter: allowed_ips must be of type string", "parameters[\"allowed_ips\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/groups/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Group>(responseJson);
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

            await FilesClient.SendRequest($"/groups/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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