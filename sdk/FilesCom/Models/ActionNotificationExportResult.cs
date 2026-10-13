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
    /// ActionNotificationExportResultEntity model
    /// </summary>
    public class ActionNotificationExportResult : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a ActionNotificationExportResult with no attributes set and no options.
        /// </summary>
        public ActionNotificationExportResult() : this(null, null) { }

        /// <summary>
        /// Makes a ActionNotificationExportResult with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public ActionNotificationExportResult(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("success"))
            {
                this.attributes.Add("success", false);
            }
            if (!this.attributes.ContainsKey("request_headers"))
            {
                this.attributes.Add("request_headers", null);
            }
            if (!this.attributes.ContainsKey("request_method"))
            {
                this.attributes.Add("request_method", null);
            }
            if (!this.attributes.ContainsKey("request_url"))
            {
                this.attributes.Add("request_url", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("folder"))
            {
                this.attributes.Add("folder", null);
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
        /// Notification ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// When the notification was sent.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<Int64> CreatedAt
        {
            get { return (Nullable<Int64>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// HTTP status code returned in the webhook response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public Nullable<Int64> Status
        {
            get { return (Nullable<Int64>)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// A message indicating the overall status of the webhook notification.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            private set { attributes["message"] = value; }
        }

        /// <summary>
        /// `true` if the webhook succeeded by receiving a 200 or 204 response.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("success")]
        public bool Success
        {
            get { return attributes["success"] == null ? false : (bool)attributes["success"]; }
            private set { attributes["success"] = value; }
        }

        /// <summary>
        /// A JSON-encoded string with headers that were sent with the webhook.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_headers")]
        public string RequestHeaders
        {
            get { return (string)attributes["request_headers"]; }
            private set { attributes["request_headers"] = value; }
        }

        /// <summary>
        /// The HTTP verb used to perform the webhook.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_method")]
        public string RequestMethod
        {
            get { return (string)attributes["request_method"]; }
            private set { attributes["request_method"] = value; }
        }

        /// <summary>
        /// The webhook request URL.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_url")]
        public string RequestUrl
        {
            get { return (string)attributes["request_url"]; }
            private set { attributes["request_url"] = value; }
        }

        /// <summary>
        /// The path to the actual file that triggered this notification. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// The folder associated with the triggering action for this notification.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("folder")]
        public string Folder
        {
            get { return (string)attributes["folder"]; }
            private set { attributes["folder"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   action_notification_export_id (required) - int64 - ID of the associated action notification export.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<ActionNotificationExportResult> List(

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
        ///   action_notification_export_id (required) - int64 - ID of the associated action notification export.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<ActionNotificationExportResult> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<ActionNotificationExportResult> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("action_notification_export_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: action_notification_export_id", "action_notification_export_id");
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
            if (parameters.TryGetValue("action_notification_export_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action_notification_export_id is null, which cannot be sent" : "Bad parameter: action_notification_export_id must be of type Nullable<Int64>", "action_notification_export_id");
            }

            return new FilesList<ActionNotificationExportResult>(client, $"/action_notification_export_results", System.Net.Http.HttpMethod.Get, parameters, options);
        }

    }
}