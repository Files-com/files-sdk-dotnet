using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ActionNotificationExport
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ActionNotificationExport() : this(null, null) { }

        public ActionNotificationExport(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("export_version"))
            {
                this.attributes.Add("export_version", null);
            }
            if (!this.attributes.ContainsKey("start_at"))
            {
                this.attributes.Add("start_at", null);
            }
            if (!this.attributes.ContainsKey("end_at"))
            {
                this.attributes.Add("end_at", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("query_path"))
            {
                this.attributes.Add("query_path", null);
            }
            if (!this.attributes.ContainsKey("query_folder"))
            {
                this.attributes.Add("query_folder", null);
            }
            if (!this.attributes.ContainsKey("query_message"))
            {
                this.attributes.Add("query_message", null);
            }
            if (!this.attributes.ContainsKey("query_request_method"))
            {
                this.attributes.Add("query_request_method", null);
            }
            if (!this.attributes.ContainsKey("query_request_url"))
            {
                this.attributes.Add("query_request_url", null);
            }
            if (!this.attributes.ContainsKey("query_status"))
            {
                this.attributes.Add("query_status", null);
            }
            if (!this.attributes.ContainsKey("query_success"))
            {
                this.attributes.Add("query_success", false);
            }
            if (!this.attributes.ContainsKey("results_url"))
            {
                this.attributes.Add("results_url", null);
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
        /// History Export ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Version of the underlying records for the export.
        /// </summary>
        [JsonPropertyName("export_version")]
        public string ExportVersion
        {
            get { return (string)attributes["export_version"]; }
            set { attributes["export_version"] = value; }
        }

        /// <summary>
        /// Start date/time of export range.
        /// </summary>
        [JsonPropertyName("start_at")]
        public Nullable<DateTime> StartAt
        {
            get { return (Nullable<DateTime>)attributes["start_at"]; }
            set { attributes["start_at"] = value; }
        }

        /// <summary>
        /// End date/time of export range.
        /// </summary>
        [JsonPropertyName("end_at")]
        public Nullable<DateTime> EndAt
        {
            get { return (Nullable<DateTime>)attributes["end_at"]; }
            set { attributes["end_at"] = value; }
        }

        /// <summary>
        /// Status of export.  Valid values: `building`, `ready`, or `failed`
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; }
        }

        /// <summary>
        /// Return notifications that were triggered by actions on this specific path.
        /// </summary>
        [JsonPropertyName("query_path")]
        public string QueryPath
        {
            get { return (string)attributes["query_path"]; }
            set { attributes["query_path"] = value; }
        }

        /// <summary>
        /// Return notifications that were triggered by actions in this folder.
        /// </summary>
        [JsonPropertyName("query_folder")]
        public string QueryFolder
        {
            get { return (string)attributes["query_folder"]; }
            set { attributes["query_folder"] = value; }
        }

        /// <summary>
        /// Error message associated with the request, if any.
        /// </summary>
        [JsonPropertyName("query_message")]
        public string QueryMessage
        {
            get { return (string)attributes["query_message"]; }
            set { attributes["query_message"] = value; }
        }

        /// <summary>
        /// The HTTP request method used by the webhook.
        /// </summary>
        [JsonPropertyName("query_request_method")]
        public string QueryRequestMethod
        {
            get { return (string)attributes["query_request_method"]; }
            set { attributes["query_request_method"] = value; }
        }

        /// <summary>
        /// The target webhook URL.
        /// </summary>
        [JsonPropertyName("query_request_url")]
        public string QueryRequestUrl
        {
            get { return (string)attributes["query_request_url"]; }
            set { attributes["query_request_url"] = value; }
        }

        /// <summary>
        /// The HTTP status returned from the server in response to the webhook request.
        /// </summary>
        [JsonPropertyName("query_status")]
        public string QueryStatus
        {
            get { return (string)attributes["query_status"]; }
            set { attributes["query_status"] = value; }
        }

        /// <summary>
        /// true if the webhook request succeeded (i.e. returned a 200 or 204 response status). false otherwise.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("query_success")]
        public bool QuerySuccess
        {
            get { return attributes["query_success"] == null ? false : (bool)attributes["query_success"]; }
            set { attributes["query_success"] = value; }
        }

        /// <summary>
        /// If `status` is `ready`, this will be a URL where all the results can be downloaded at once as a CSV.
        /// </summary>
        [JsonPropertyName("results_url")]
        public string ResultsUrl
        {
            get { return (string)attributes["results_url"]; }
            set { attributes["results_url"] = value; }
        }

        /// <summary>
        /// User ID.  Provide a value of `0` to operate the current session's user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The ActionNotificationExport object doesn't support updates.");
            }
            else
            {
                var newObj = await ActionNotificationExport.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Action Notification Export ID.
        /// </summary>
        public static async Task<ActionNotificationExport> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/action_notification_exports/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ActionNotificationExport>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<ActionNotificationExport> Get(
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
        ///   start_at - string - Start date/time of export range.
        ///   end_at - string - End date/time of export range.
        ///   query_message - string - Error message associated with the request, if any.
        ///   query_request_method - string - The HTTP request method used by the webhook.
        ///   query_request_url - string - The target webhook URL.
        ///   query_status - string - The HTTP status returned from the server in response to the webhook request.
        ///   query_success - boolean - true if the webhook request succeeded (i.e. returned a 200 or 204 response status). false otherwise.
        ///   query_path - string - Return notifications that were triggered by actions on this specific path.
        ///   query_folder - string - Return notifications that were triggered by actions in this folder.
        /// </summary>
        public static async Task<ActionNotificationExport> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("query_message") && !(parameters["query_message"] is string))
            {
                throw new ArgumentException("Bad parameter: query_message must be of type string", "parameters[\"query_message\"]");
            }
            if (parameters.ContainsKey("query_request_method") && !(parameters["query_request_method"] is string))
            {
                throw new ArgumentException("Bad parameter: query_request_method must be of type string", "parameters[\"query_request_method\"]");
            }
            if (parameters.ContainsKey("query_request_url") && !(parameters["query_request_url"] is string))
            {
                throw new ArgumentException("Bad parameter: query_request_url must be of type string", "parameters[\"query_request_url\"]");
            }
            if (parameters.ContainsKey("query_status") && !(parameters["query_status"] is string))
            {
                throw new ArgumentException("Bad parameter: query_status must be of type string", "parameters[\"query_status\"]");
            }
            if (parameters.ContainsKey("query_success") && !(parameters["query_success"] is bool))
            {
                throw new ArgumentException("Bad parameter: query_success must be of type bool", "parameters[\"query_success\"]");
            }
            if (parameters.ContainsKey("query_path") && !(parameters["query_path"] is string))
            {
                throw new ArgumentException("Bad parameter: query_path must be of type string", "parameters[\"query_path\"]");
            }
            if (parameters.ContainsKey("query_folder") && !(parameters["query_folder"] is string))
            {
                throw new ArgumentException("Bad parameter: query_folder must be of type string", "parameters[\"query_folder\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/action_notification_exports", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ActionNotificationExport>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}