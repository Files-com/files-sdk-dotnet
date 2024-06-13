using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PublicHostingRequestLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PublicHostingRequestLog() : this(null, null) { }

        public PublicHostingRequestLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("remote_ip"))
            {
                this.attributes.Add("remote_ip", null);
            }
            if (!this.attributes.ContainsKey("server_ip"))
            {
                this.attributes.Add("server_ip", null);
            }
            if (!this.attributes.ContainsKey("hostname"))
            {
                this.attributes.Add("hostname", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("responseCode"))
            {
                this.attributes.Add("responseCode", null);
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
        /// IP Address of Public Hosting Client
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("remote_ip")]
        public string RemoteIp
        {
            get { return (string)attributes["remote_ip"]; }
            private set { attributes["remote_ip"] = value; }
        }

        /// <summary>
        /// IP Address of Public Hosting Server
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_ip")]
        public string ServerIp
        {
            get { return (string)attributes["server_ip"]; }
            private set { attributes["server_ip"] = value; }
        }

        /// <summary>
        /// HTTP Request Hostname
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("hostname")]
        public string Hostname
        {
            get { return (string)attributes["hostname"]; }
            private set { attributes["hostname"] = value; }
        }

        /// <summary>
        /// HTTP Request Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// HTTP Response Code
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("responseCode")]
        public Nullable<Int64> ResponseCode
        {
            get { return (Nullable<Int64>)attributes["responseCode"]; }
            private set { attributes["responseCode"] = value; }
        }

        /// <summary>
        /// Whether SFTP Action was successful.
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
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `path`, `remote_ip` or `success`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ path ]`, `[ remote_ip ]`, `[ success ]`, `[ start_date, end_date ]`, `[ start_date, path ]`, `[ start_date, remote_ip ]`, `[ start_date, success ]`, `[ end_date, path ]`, `[ end_date, remote_ip ]`, `[ end_date, success ]`, `[ path, remote_ip ]`, `[ path, success ]`, `[ remote_ip, success ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, remote_ip ]`, `[ start_date, end_date, success ]`, `[ start_date, path, remote_ip ]`, `[ start_date, path, success ]`, `[ start_date, remote_ip, success ]`, `[ end_date, path, remote_ip ]`, `[ end_date, path, success ]`, `[ end_date, remote_ip, success ]`, `[ path, remote_ip, success ]`, `[ start_date, end_date, path, remote_ip ]`, `[ start_date, end_date, path, success ]`, `[ start_date, end_date, remote_ip, success ]`, `[ start_date, path, remote_ip, success ]` or `[ end_date, path, remote_ip, success ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ path ]`, `[ remote_ip ]`, `[ success ]`, `[ start_date, end_date ]`, `[ start_date, path ]`, `[ start_date, remote_ip ]`, `[ start_date, success ]`, `[ end_date, path ]`, `[ end_date, remote_ip ]`, `[ end_date, success ]`, `[ path, remote_ip ]`, `[ path, success ]`, `[ remote_ip, success ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, remote_ip ]`, `[ start_date, end_date, success ]`, `[ start_date, path, remote_ip ]`, `[ start_date, path, success ]`, `[ start_date, remote_ip, success ]`, `[ end_date, path, remote_ip ]`, `[ end_date, path, success ]`, `[ end_date, remote_ip, success ]`, `[ path, remote_ip, success ]`, `[ start_date, end_date, path, remote_ip ]`, `[ start_date, end_date, path, success ]`, `[ start_date, end_date, remote_ip, success ]`, `[ start_date, path, remote_ip, success ]` or `[ end_date, path, remote_ip, success ]`.
        /// </summary>
        public static FilesList<PublicHostingRequestLog> List(

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
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }

            return new FilesList<PublicHostingRequestLog>($"/public_hosting_request_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<PublicHostingRequestLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}