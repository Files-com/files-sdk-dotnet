using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ActionNotificationExportResult
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ActionNotificationExportResult() : this(null, null) { }

        public ActionNotificationExportResult(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
                this.attributes.Add("success", null);
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
        /// Notification ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            
            get;
            set;
        }

        /// <summary>
        /// When the notification was sent.
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<Int64> CreatedAt
        {
            
            get;
            set;
        }

        /// <summary>
        /// HTTP status code returned in the webhook response.
        /// </summary>
        [JsonPropertyName("status")]
        public Nullable<Int64> Status
        {
            
            get;
            set;
        }

        /// <summary>
        /// A message indicating the overall status of the webhook notification.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message
        {
            
            get;
            set;
        }

        /// <summary>
        /// `true` if the webhook succeeded by receiving a 200 or 204 response.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success
        {
            
            get;
            set;
        }

        /// <summary>
        /// A JSON-encoded string with headers that were sent with the webhook.
        /// </summary>
        [JsonPropertyName("request_headers")]
        public string RequestHeaders
        {
            
            get;
            set;
        }

        /// <summary>
        /// The HTTP verb used to perform the webhook.
        /// </summary>
        [JsonPropertyName("request_method")]
        public string RequestMethod
        {
            
            get;
            set;
        }

        /// <summary>
        /// The webhook request URL.
        /// </summary>
        [JsonPropertyName("request_url")]
        public string RequestUrl
        {
            
            get;
            set;
        }

        /// <summary>
        /// The path to the actual file that triggered this notification. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            
            get;
            set;
        }

        /// <summary>
        /// The folder associated with the triggering action for this notification.
        /// </summary>
        [JsonPropertyName("folder")]
        public string Folder
        {
            
            get;
            set;
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action_notification_export_id (required) - int64 - ID of the associated action notification export.
        /// </summary>
        public static async Task<ActionNotificationExportResult[]> List(
            
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
            if (parameters.ContainsKey("action_notification_export_id") && !(parameters["action_notification_export_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: action_notification_export_id must be of type Nullable<Int64>", "parameters[\"action_notification_export_id\"]");
            }
            if (!parameters.ContainsKey("action_notification_export_id") || parameters["action_notification_export_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: action_notification_export_id", "parameters[\"action_notification_export_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/action_notification_export_results", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<ActionNotificationExportResult[]>(responseJson);
        }

        public static async Task<ActionNotificationExportResult[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

