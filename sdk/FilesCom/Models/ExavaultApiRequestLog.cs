using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ExavaultApiRequestLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ExavaultApiRequestLog() : this(null, null) { }

        public ExavaultApiRequestLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("timestamp"))
            {
                this.attributes.Add("timestamp", null);
            }
            if (!this.attributes.ContainsKey("endpoint"))
            {
                this.attributes.Add("endpoint", null);
            }
            if (!this.attributes.ContainsKey("version"))
            {
                this.attributes.Add("version", null);
            }
            if (!this.attributes.ContainsKey("request_ip"))
            {
                this.attributes.Add("request_ip", null);
            }
            if (!this.attributes.ContainsKey("request_method"))
            {
                this.attributes.Add("request_method", null);
            }
            if (!this.attributes.ContainsKey("error_type"))
            {
                this.attributes.Add("error_type", null);
            }
            if (!this.attributes.ContainsKey("error_message"))
            {
                this.attributes.Add("error_message", null);
            }
            if (!this.attributes.ContainsKey("user_agent"))
            {
                this.attributes.Add("user_agent", null);
            }
            if (!this.attributes.ContainsKey("response_code"))
            {
                this.attributes.Add("response_code", null);
            }
            if (!this.attributes.ContainsKey("success"))
            {
                this.attributes.Add("success", false);
            }
            if (!this.attributes.ContainsKey("duration_ms"))
            {
                this.attributes.Add("duration_ms", null);
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
        /// Start Time of Action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("timestamp")]
        public Nullable<DateTime> Timestamp
        {
            get { return (Nullable<DateTime>)attributes["timestamp"]; }
            private set { attributes["timestamp"] = value; }
        }

        /// <summary>
        /// Name of API Endpoint
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("endpoint")]
        public string Endpoint
        {
            get { return (string)attributes["endpoint"]; }
            private set { attributes["endpoint"] = value; }
        }

        /// <summary>
        /// Exavault API Version
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("version")]
        public string Version
        {
            get { return (string)attributes["version"]; }
            private set { attributes["version"] = value; }
        }

        /// <summary>
        /// IP of requesting client
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_ip")]
        public string RequestIp
        {
            get { return (string)attributes["request_ip"]; }
            private set { attributes["request_ip"] = value; }
        }

        /// <summary>
        /// HTTP Method
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_method")]
        public string RequestMethod
        {
            get { return (string)attributes["request_method"]; }
            private set { attributes["request_method"] = value; }
        }

        /// <summary>
        /// Error type, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error_type")]
        public string ErrorType
        {
            get { return (string)attributes["error_type"]; }
            private set { attributes["error_type"] = value; }
        }

        /// <summary>
        /// Error message, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error_message")]
        public string ErrorMessage
        {
            get { return (string)attributes["error_message"]; }
            private set { attributes["error_message"] = value; }
        }

        /// <summary>
        /// User-Agent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_agent")]
        public string UserAgent
        {
            get { return (string)attributes["user_agent"]; }
            private set { attributes["user_agent"] = value; }
        }

        /// <summary>
        /// HTTP Response Code
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("response_code")]
        public Nullable<Int64> ResponseCode
        {
            get { return (Nullable<Int64>)attributes["response_code"]; }
            private set { attributes["response_code"] = value; }
        }

        /// <summary>
        /// `false` if HTTP Response Code is 4xx or 5xx
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
        /// Duration (in milliseconds)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("duration_ms")]
        public Nullable<Int64> DurationMs
        {
            get { return (Nullable<Int64>)attributes["duration_ms"]; }
            private set { attributes["duration_ms"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `request_ip`, `request_method` or `success`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ request_ip ]`, `[ request_method ]`, `[ success ]`, `[ start_date, end_date ]`, `[ start_date, request_ip ]`, `[ start_date, request_method ]`, `[ start_date, success ]`, `[ end_date, request_ip ]`, `[ end_date, request_method ]`, `[ end_date, success ]`, `[ request_ip, request_method ]`, `[ request_ip, success ]`, `[ request_method, success ]`, `[ start_date, end_date, request_ip ]`, `[ start_date, end_date, request_method ]`, `[ start_date, end_date, success ]`, `[ start_date, request_ip, request_method ]`, `[ start_date, request_ip, success ]`, `[ start_date, request_method, success ]`, `[ end_date, request_ip, request_method ]`, `[ end_date, request_ip, success ]`, `[ end_date, request_method, success ]`, `[ request_ip, request_method, success ]`, `[ start_date, end_date, request_ip, request_method ]`, `[ start_date, end_date, request_ip, success ]`, `[ start_date, end_date, request_method, success ]`, `[ start_date, request_ip, request_method, success ]` or `[ end_date, request_ip, request_method, success ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `request_ip` and `request_method`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ request_ip ]`, `[ request_method ]`, `[ success ]`, `[ start_date, end_date ]`, `[ start_date, request_ip ]`, `[ start_date, request_method ]`, `[ start_date, success ]`, `[ end_date, request_ip ]`, `[ end_date, request_method ]`, `[ end_date, success ]`, `[ request_ip, request_method ]`, `[ request_ip, success ]`, `[ request_method, success ]`, `[ start_date, end_date, request_ip ]`, `[ start_date, end_date, request_method ]`, `[ start_date, end_date, success ]`, `[ start_date, request_ip, request_method ]`, `[ start_date, request_ip, success ]`, `[ start_date, request_method, success ]`, `[ end_date, request_ip, request_method ]`, `[ end_date, request_ip, success ]`, `[ end_date, request_method, success ]`, `[ request_ip, request_method, success ]`, `[ start_date, end_date, request_ip, request_method ]`, `[ start_date, end_date, request_ip, success ]`, `[ start_date, end_date, request_method, success ]`, `[ start_date, request_ip, request_method, success ]` or `[ end_date, request_ip, request_method, success ]`.
        /// </summary>
        public static FilesList<ExavaultApiRequestLog> List(

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
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }

            return new FilesList<ExavaultApiRequestLog>($"/exavault_api_request_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<ExavaultApiRequestLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}