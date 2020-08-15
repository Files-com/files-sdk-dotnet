using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class HistoryExportResult
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public HistoryExportResult() : this(null, null) { }

        public HistoryExportResult(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("file_id"))
            {
                this.attributes.Add("file_id", null);
            }
            if (!this.attributes.ContainsKey("parent_id"))
            {
                this.attributes.Add("parent_id", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("folder"))
            {
                this.attributes.Add("folder", null);
            }
            if (!this.attributes.ContainsKey("src"))
            {
                this.attributes.Add("src", null);
            }
            if (!this.attributes.ContainsKey("destination"))
            {
                this.attributes.Add("destination", null);
            }
            if (!this.attributes.ContainsKey("ip"))
            {
                this.attributes.Add("ip", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("failure_type"))
            {
                this.attributes.Add("failure_type", null);
            }
            if (!this.attributes.ContainsKey("interface"))
            {
                this.attributes.Add("interface", null);
            }
            if (!this.attributes.ContainsKey("target_id"))
            {
                this.attributes.Add("target_id", null);
            }
            if (!this.attributes.ContainsKey("target_name"))
            {
                this.attributes.Add("target_name", null);
            }
            if (!this.attributes.ContainsKey("target_permission"))
            {
                this.attributes.Add("target_permission", null);
            }
            if (!this.attributes.ContainsKey("target_recursive"))
            {
                this.attributes.Add("target_recursive", null);
            }
            if (!this.attributes.ContainsKey("target_expires_at"))
            {
                this.attributes.Add("target_expires_at", null);
            }
            if (!this.attributes.ContainsKey("target_permission_set"))
            {
                this.attributes.Add("target_permission_set", null);
            }
            if (!this.attributes.ContainsKey("target_platform"))
            {
                this.attributes.Add("target_platform", null);
            }
            if (!this.attributes.ContainsKey("target_username"))
            {
                this.attributes.Add("target_username", null);
            }
            if (!this.attributes.ContainsKey("target_user_id"))
            {
                this.attributes.Add("target_user_id", null);
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
        /// Action ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// When the action happened
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<Int64> CreatedAt
        {
            get { return (Nullable<Int64>) attributes["created_at"]; }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
        }

        /// <summary>
        /// File ID related to the action
        /// </summary>
        [JsonPropertyName("file_id")]
        public Nullable<Int64> FileId
        {
            get { return (Nullable<Int64>) attributes["file_id"]; }
        }

        /// <summary>
        /// ID of the parent folder
        /// </summary>
        [JsonPropertyName("parent_id")]
        public Nullable<Int64> ParentId
        {
            get { return (Nullable<Int64>) attributes["parent_id"]; }
        }

        /// <summary>
        /// Path of the related action This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
        }

        /// <summary>
        /// Folder in which the action occurred
        /// </summary>
        [JsonPropertyName("folder")]
        public string Folder
        {
            get { return (string) attributes["folder"]; }
        }

        /// <summary>
        /// File move originated from this path
        /// </summary>
        [JsonPropertyName("src")]
        public string Src
        {
            get { return (string) attributes["src"]; }
        }

        /// <summary>
        /// File moved to this destination folder
        /// </summary>
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string) attributes["destination"]; }
        }

        /// <summary>
        /// Client IP that performed the action
        /// </summary>
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string) attributes["ip"]; }
        }

        /// <summary>
        /// Username of the user that performed the action
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string) attributes["username"]; }
        }

        /// <summary>
        /// What action was taken. Valid values: `create`, `read`, `update`, `destroy`, `move`, `login`, `failedlogin`, `copy`, `user_create`, `user_update`, `user_destroy`, `group_create`, `group_update`, `group_destroy`, `permission_create`, `permission_destroy`, `api_key_create`, `api_key_update`, `api_key_destroy`
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string) attributes["action"]; }
        }

        /// <summary>
        /// The type of login failure, if applicable.  Valid values: `expired_trial`, `account_overdue`, `locked_out`, `ip_mismatch`, `password_mismatch`, `site_mismatch`, `username_not_found`, `none`, `no_ftp_permission`, `no_web_permission`, `no_directory`, `errno_enoent`, `no_sftp_permission`, `no_dav_permission`, `no_restapi_permission`, `key_mismatch`, `region_mismatch`, `expired_access`, `desktop_ip_mismatch`, `desktop_api_key_not_used_quickly_enough`, `disabled`
        /// </summary>
        [JsonPropertyName("failure_type")]
        public string FailureType
        {
            get { return (string) attributes["failure_type"]; }
        }

        /// <summary>
        /// Inteface through which the action was taken. Valid values: `web`, `ftp`, `robot`, `jsapi`, `webdesktopapi`, `sftp`, `dav`, `desktop`, `restapi`, `scim`
        /// </summary>
        [JsonPropertyName("interface")]
        public string Interface
        {
            get { return (string) attributes["interface"]; }
        }

        /// <summary>
        /// ID of the object (such as Users, or API Keys) on which the action was taken
        /// </summary>
        [JsonPropertyName("target_id")]
        public Nullable<Int64> TargetId
        {
            get { return (Nullable<Int64>) attributes["target_id"]; }
        }

        /// <summary>
        /// Name of the User, Group or other object with a name related to this action
        /// </summary>
        [JsonPropertyName("target_name")]
        public string TargetName
        {
            get { return (string) attributes["target_name"]; }
        }

        /// <summary>
        /// Permission level of the action
        /// </summary>
        [JsonPropertyName("target_permission")]
        public string TargetPermission
        {
            get { return (string) attributes["target_permission"]; }
        }

        /// <summary>
        /// Whether or not the action was recursive
        /// </summary>
        [JsonPropertyName("target_recursive")]
        public bool TargetRecursive
        {
            get { return (bool) attributes["target_recursive"]; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is when the API key will expire
        /// </summary>
        [JsonPropertyName("target_expires_at")]
        public Nullable<Int64> TargetExpiresAt
        {
            get { return (Nullable<Int64>) attributes["target_expires_at"]; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this represents the permission set of the associated  API key
        /// </summary>
        [JsonPropertyName("target_permission_set")]
        public string TargetPermissionSet
        {
            get { return (string) attributes["target_permission_set"]; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is the platform on which the action was taken
        /// </summary>
        [JsonPropertyName("target_platform")]
        public string TargetPlatform
        {
            get { return (string) attributes["target_platform"]; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is the username on which the action was taken
        /// </summary>
        [JsonPropertyName("target_username")]
        public string TargetUsername
        {
            get { return (string) attributes["target_username"]; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is the User ID on which the action was taken
        /// </summary>
        [JsonPropertyName("target_user_id")]
        public Nullable<Int64> TargetUserId
        {
            get { return (Nullable<Int64>) attributes["target_user_id"]; }
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        ///   cursor - string - Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   history_export_id (required) - int64 - ID of the associated history export.
        /// </summary>
        public static async Task<HistoryExportResult[]> List(
            
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
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("history_export_id") && !(parameters["history_export_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: history_export_id must be of type Nullable<Int64>", "parameters[\"history_export_id\"]");
            }
            if (!parameters.ContainsKey("history_export_id") || parameters["history_export_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: history_export_id", "parameters[\"history_export_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/history_export_results", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<HistoryExportResult[]>(responseJson);
        }

        public static async Task<HistoryExportResult[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

