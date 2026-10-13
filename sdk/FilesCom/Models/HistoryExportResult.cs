using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// HistoryExportResultEntity model
    /// </summary>
    public class HistoryExportResult : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a HistoryExportResult with no attributes set and no options.
        /// </summary>
        public HistoryExportResult() : this(null, null) { }

        /// <summary>
        /// Makes a HistoryExportResult with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public HistoryExportResult(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("created_at_iso8601"))
            {
                this.attributes.Add("created_at_iso8601", null);
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
            if (!this.attributes.ContainsKey("user_is_from_parent_site"))
            {
                this.attributes.Add("user_is_from_parent_site", false);
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
                this.attributes.Add("target_recursive", false);
            }
            if (!this.attributes.ContainsKey("target_expires_at"))
            {
                this.attributes.Add("target_expires_at", null);
            }
            if (!this.attributes.ContainsKey("target_expires_at_iso8601"))
            {
                this.attributes.Add("target_expires_at_iso8601", null);
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

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => null;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[0]; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// Action ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// When the action happened
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<Int64> CreatedAt
        {
            get { return (Nullable<Int64>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// When the action happened, in ISO8601 format.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at_iso8601")]
        public string CreatedAtIso8601
        {
            get { return (string)attributes["created_at_iso8601"]; }
            private set { attributes["created_at_iso8601"] = value; }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            private set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// File ID related to the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_id")]
        public Nullable<Int64> FileId
        {
            get { return (Nullable<Int64>)attributes["file_id"]; }
            private set { attributes["file_id"] = value; }
        }

        /// <summary>
        /// ID of the parent folder
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("parent_id")]
        public Nullable<Int64> ParentId
        {
            get { return (Nullable<Int64>)attributes["parent_id"]; }
            private set { attributes["parent_id"] = value; }
        }

        /// <summary>
        /// Path of the related action. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// Folder in which the action occurred
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("folder")]
        public string Folder
        {
            get { return (string)attributes["folder"]; }
            private set { attributes["folder"] = value; }
        }

        /// <summary>
        /// File move originated from this path
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("src")]
        public string Src
        {
            get { return (string)attributes["src"]; }
            private set { attributes["src"] = value; }
        }

        /// <summary>
        /// File moved to this destination folder
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string)attributes["destination"]; }
            private set { attributes["destination"] = value; }
        }

        /// <summary>
        /// Client IP that performed the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string)attributes["ip"]; }
            private set { attributes["ip"] = value; }
        }

        /// <summary>
        /// Username of the user that performed the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            private set { attributes["username"] = value; }
        }

        /// <summary>
        /// true if this change was performed by a user on a parent site.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("user_is_from_parent_site")]
        public bool UserIsFromParentSite
        {
            get { return attributes["user_is_from_parent_site"] == null ? false : (bool)attributes["user_is_from_parent_site"]; }
            private set { attributes["user_is_from_parent_site"] = value; }
        }

        /// <summary>
        /// What action was taken. Valid values: `create`, `read`, `update`, `destroy`, `move`, `login`, `failedlogin`, `copy`, `user_create`, `user_update`, `user_destroy`, `group_create`, `group_update`, `group_destroy`, `permission_create`, `permission_destroy`, `api_key_create`, `api_key_update`, `api_key_destroy`, `archived_delete`
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            private set { attributes["action"] = value; }
        }

        /// <summary>
        /// The type of login failure, if applicable.  Valid values: `expired_trial`, `account_overdue`, `locked_out`, `ip_mismatch`, `password_mismatch`, `site_mismatch`, `username_not_found`, `none`, `no_ftp_permission`, `no_web_permission`, `no_directory`, `errno_enoent`, `no_sftp_permission`, `no_dav_permission`, `no_restapi_permission`, `key_mismatch`, `region_mismatch`, `expired_access`, `desktop_ip_mismatch`, `desktop_api_key_not_used_quickly_enough`, `disabled`, `country_mismatch`, `insecure_ftp`, `insecure_cipher`, `rate_limited`, `no_s3_compatible_endpoint_permission`, `mobile_ip_mismatch`, `mobile_api_key_not_used_quickly_enough`, `desktop_app_disabled`, `mobile_app_disabled`
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failure_type")]
        public string FailureType
        {
            get { return (string)attributes["failure_type"]; }
            private set { attributes["failure_type"] = value; }
        }

        /// <summary>
        /// Interface through which the action was taken. Valid values: `web`, `ftp`, `robot`, `jsapi`, `webdesktopapi`, `sftp`, `dav`, `desktop`, `restapi`, `scim`, `office`, `mobile`, `as2`, `inbound_email`, `remote`, `inbound_s3`
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("interface")]
        public string Interface
        {
            get { return (string)attributes["interface"]; }
            private set { attributes["interface"] = value; }
        }

        /// <summary>
        /// ID of the object (such as Users, or API Keys) on which the action was taken
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_id")]
        public Nullable<Int64> TargetId
        {
            get { return (Nullable<Int64>)attributes["target_id"]; }
            private set { attributes["target_id"] = value; }
        }

        /// <summary>
        /// Name of the User, Group or other object with a name related to this action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_name")]
        public string TargetName
        {
            get { return (string)attributes["target_name"]; }
            private set { attributes["target_name"] = value; }
        }

        /// <summary>
        /// Permission level of the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_permission")]
        public string TargetPermission
        {
            get { return (string)attributes["target_permission"]; }
            private set { attributes["target_permission"] = value; }
        }

        /// <summary>
        /// Whether or not the action was recursive
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("target_recursive")]
        public bool TargetRecursive
        {
            get { return attributes["target_recursive"] == null ? false : (bool)attributes["target_recursive"]; }
            private set { attributes["target_recursive"] = value; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is when the API key will expire. Represented as a Unix timestamp.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_expires_at")]
        public Nullable<Int64> TargetExpiresAt
        {
            get { return (Nullable<Int64>)attributes["target_expires_at"]; }
            private set { attributes["target_expires_at"] = value; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is when the API key will expire. Represented in ISO8601 format.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_expires_at_iso8601")]
        public string TargetExpiresAtIso8601
        {
            get { return (string)attributes["target_expires_at_iso8601"]; }
            private set { attributes["target_expires_at_iso8601"] = value; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this represents the permission set of the associated  API key
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_permission_set")]
        public string TargetPermissionSet
        {
            get { return (string)attributes["target_permission_set"]; }
            private set { attributes["target_permission_set"] = value; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is the platform on which the action was taken
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_platform")]
        public string TargetPlatform
        {
            get { return (string)attributes["target_platform"]; }
            private set { attributes["target_platform"] = value; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is the username on which the action was taken
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_username")]
        public string TargetUsername
        {
            get { return (string)attributes["target_username"]; }
            private set { attributes["target_username"] = value; }
        }

        /// <summary>
        /// If searching for Histories about API keys, this is the User ID on which the action was taken
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("target_user_id")]
        public Nullable<Int64> TargetUserId
        {
            get { return (Nullable<Int64>)attributes["target_user_id"]; }
            private set { attributes["target_user_id"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   history_export_id (required) - int64 - ID of the associated history export.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<HistoryExportResult> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   history_export_id (required) - int64 - ID of the associated history export.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<HistoryExportResult> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<HistoryExportResult> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("history_export_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: history_export_id", "history_export_id");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("history_export_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: history_export_id is null, which cannot be sent" : "Bad parameter: history_export_id must be of type Nullable<Int64>", "history_export_id");
            }

            return new FilesList<HistoryExportResult>(client, $"/history_export_results", System.Net.Http.HttpMethod.Get, parameters, options);
        }

    }
}