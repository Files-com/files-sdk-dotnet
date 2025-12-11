using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class InboundS3Log
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public InboundS3Log() : this(null, null) { }

        public InboundS3Log(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("client_ip"))
            {
                this.attributes.Add("client_ip", null);
            }
            if (!this.attributes.ContainsKey("operation"))
            {
                this.attributes.Add("operation", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("aws_access_key"))
            {
                this.attributes.Add("aws_access_key", null);
            }
            if (!this.attributes.ContainsKey("error_message"))
            {
                this.attributes.Add("error_message", null);
            }
            if (!this.attributes.ContainsKey("error_type"))
            {
                this.attributes.Add("error_type", null);
            }
            if (!this.attributes.ContainsKey("duration_ms"))
            {
                this.attributes.Add("duration_ms", null);
            }
            if (!this.attributes.ContainsKey("request_id"))
            {
                this.attributes.Add("request_id", null);
            }
            if (!this.attributes.ContainsKey("user_agent"))
            {
                this.attributes.Add("user_agent", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
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
        /// Request Path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// Client IP
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("client_ip")]
        public string ClientIp
        {
            get { return (string)attributes["client_ip"]; }
            private set { attributes["client_ip"] = value; }
        }

        /// <summary>
        /// S3 Operation Type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("operation")]
        public string Operation
        {
            get { return (string)attributes["operation"]; }
            private set { attributes["operation"] = value; }
        }

        /// <summary>
        /// HTTP Status Code
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// AWS Access Key ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("aws_access_key")]
        public string AwsAccessKey
        {
            get { return (string)attributes["aws_access_key"]; }
            private set { attributes["aws_access_key"] = value; }
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
        /// Request ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_id")]
        public string RequestId
        {
            get { return (string)attributes["request_id"]; }
            private set { attributes["request_id"] = value; }
        }

        /// <summary>
        /// User Agent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_agent")]
        public string UserAgent
        {
            get { return (string)attributes["user_agent"]; }
            private set { attributes["user_agent"] = value; }
        }

        /// <summary>
        /// Start Time of Request
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `operation`, `status`, `path`, `client_ip` or `created_at`. Valid field combinations are `[ operation ]`, `[ status ]`, `[ path ]`, `[ client_ip ]`, `[ created_at ]`, `[ operation, status ]`, `[ operation, path ]`, `[ operation, client_ip ]`, `[ operation, created_at ]`, `[ status, path ]`, `[ status, client_ip ]`, `[ status, created_at ]`, `[ path, client_ip ]`, `[ path, created_at ]`, `[ client_ip, created_at ]`, `[ operation, status, path ]`, `[ operation, status, client_ip ]`, `[ operation, status, created_at ]`, `[ operation, path, client_ip ]`, `[ operation, path, created_at ]`, `[ operation, client_ip, created_at ]`, `[ status, path, client_ip ]`, `[ status, path, created_at ]`, `[ status, client_ip, created_at ]`, `[ path, client_ip, created_at ]`, `[ operation, status, path, client_ip ]`, `[ operation, status, path, created_at ]`, `[ operation, status, client_ip, created_at ]`, `[ operation, path, client_ip, created_at ]` or `[ status, path, client_ip, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`. Valid field combinations are `[ operation ]`, `[ status ]`, `[ path ]`, `[ client_ip ]`, `[ created_at ]`, `[ operation, status ]`, `[ operation, path ]`, `[ operation, client_ip ]`, `[ operation, created_at ]`, `[ status, path ]`, `[ status, client_ip ]`, `[ status, created_at ]`, `[ path, client_ip ]`, `[ path, created_at ]`, `[ client_ip, created_at ]`, `[ operation, status, path ]`, `[ operation, status, client_ip ]`, `[ operation, status, created_at ]`, `[ operation, path, client_ip ]`, `[ operation, path, created_at ]`, `[ operation, client_ip, created_at ]`, `[ status, path, client_ip ]`, `[ status, path, created_at ]`, `[ status, client_ip, created_at ]`, `[ path, client_ip, created_at ]`, `[ operation, status, path, client_ip ]`, `[ operation, status, path, created_at ]`, `[ operation, status, client_ip, created_at ]`, `[ operation, path, client_ip, created_at ]` or `[ status, path, client_ip, created_at ]`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`. Valid field combinations are `[ operation ]`, `[ status ]`, `[ path ]`, `[ client_ip ]`, `[ created_at ]`, `[ operation, status ]`, `[ operation, path ]`, `[ operation, client_ip ]`, `[ operation, created_at ]`, `[ status, path ]`, `[ status, client_ip ]`, `[ status, created_at ]`, `[ path, client_ip ]`, `[ path, created_at ]`, `[ client_ip, created_at ]`, `[ operation, status, path ]`, `[ operation, status, client_ip ]`, `[ operation, status, created_at ]`, `[ operation, path, client_ip ]`, `[ operation, path, created_at ]`, `[ operation, client_ip, created_at ]`, `[ status, path, client_ip ]`, `[ status, path, created_at ]`, `[ status, client_ip, created_at ]`, `[ path, client_ip, created_at ]`, `[ operation, status, path, client_ip ]`, `[ operation, status, path, created_at ]`, `[ operation, status, client_ip, created_at ]`, `[ operation, path, client_ip, created_at ]` or `[ status, path, client_ip, created_at ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `operation`, `status`, `path` or `client_ip`. Valid field combinations are `[ operation ]`, `[ status ]`, `[ path ]`, `[ client_ip ]`, `[ created_at ]`, `[ operation, status ]`, `[ operation, path ]`, `[ operation, client_ip ]`, `[ operation, created_at ]`, `[ status, path ]`, `[ status, client_ip ]`, `[ status, created_at ]`, `[ path, client_ip ]`, `[ path, created_at ]`, `[ client_ip, created_at ]`, `[ operation, status, path ]`, `[ operation, status, client_ip ]`, `[ operation, status, created_at ]`, `[ operation, path, client_ip ]`, `[ operation, path, created_at ]`, `[ operation, client_ip, created_at ]`, `[ status, path, client_ip ]`, `[ status, path, created_at ]`, `[ status, client_ip, created_at ]`, `[ path, client_ip, created_at ]`, `[ operation, status, path, client_ip ]`, `[ operation, status, path, created_at ]`, `[ operation, status, client_ip, created_at ]`, `[ operation, path, client_ip, created_at ]` or `[ status, path, client_ip, created_at ]`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`. Valid field combinations are `[ operation ]`, `[ status ]`, `[ path ]`, `[ client_ip ]`, `[ created_at ]`, `[ operation, status ]`, `[ operation, path ]`, `[ operation, client_ip ]`, `[ operation, created_at ]`, `[ status, path ]`, `[ status, client_ip ]`, `[ status, created_at ]`, `[ path, client_ip ]`, `[ path, created_at ]`, `[ client_ip, created_at ]`, `[ operation, status, path ]`, `[ operation, status, client_ip ]`, `[ operation, status, created_at ]`, `[ operation, path, client_ip ]`, `[ operation, path, created_at ]`, `[ operation, client_ip, created_at ]`, `[ status, path, client_ip ]`, `[ status, path, created_at ]`, `[ status, client_ip, created_at ]`, `[ path, client_ip, created_at ]`, `[ operation, status, path, client_ip ]`, `[ operation, status, path, created_at ]`, `[ operation, status, client_ip, created_at ]`, `[ operation, path, client_ip, created_at ]` or `[ status, path, client_ip, created_at ]`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`. Valid field combinations are `[ operation ]`, `[ status ]`, `[ path ]`, `[ client_ip ]`, `[ created_at ]`, `[ operation, status ]`, `[ operation, path ]`, `[ operation, client_ip ]`, `[ operation, created_at ]`, `[ status, path ]`, `[ status, client_ip ]`, `[ status, created_at ]`, `[ path, client_ip ]`, `[ path, created_at ]`, `[ client_ip, created_at ]`, `[ operation, status, path ]`, `[ operation, status, client_ip ]`, `[ operation, status, created_at ]`, `[ operation, path, client_ip ]`, `[ operation, path, created_at ]`, `[ operation, client_ip, created_at ]`, `[ status, path, client_ip ]`, `[ status, path, created_at ]`, `[ status, client_ip, created_at ]`, `[ path, client_ip, created_at ]`, `[ operation, status, path, client_ip ]`, `[ operation, status, path, created_at ]`, `[ operation, status, client_ip, created_at ]`, `[ operation, path, client_ip, created_at ]` or `[ status, path, client_ip, created_at ]`.
        /// </summary>
        public static FilesList<InboundS3Log> List(

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
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            return new FilesList<InboundS3Log>($"/inbound_s3_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<InboundS3Log> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}