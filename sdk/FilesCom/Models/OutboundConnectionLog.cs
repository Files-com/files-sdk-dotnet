using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class OutboundConnectionLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public OutboundConnectionLog() : this(null, null) { }

        public OutboundConnectionLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("client_ip"))
            {
                this.attributes.Add("client_ip", null);
            }
            if (!this.attributes.ContainsKey("src_remote_server_id"))
            {
                this.attributes.Add("src_remote_server_id", null);
            }
            if (!this.attributes.ContainsKey("dest_remote_server_id"))
            {
                this.attributes.Add("dest_remote_server_id", null);
            }
            if (!this.attributes.ContainsKey("operation"))
            {
                this.attributes.Add("operation", null);
            }
            if (!this.attributes.ContainsKey("error_message"))
            {
                this.attributes.Add("error_message", null);
            }
            if (!this.attributes.ContainsKey("error_operation"))
            {
                this.attributes.Add("error_operation", null);
            }
            if (!this.attributes.ContainsKey("error_type"))
            {
                this.attributes.Add("error_type", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("duration_ms"))
            {
                this.attributes.Add("duration_ms", null);
            }
            if (!this.attributes.ContainsKey("bytes_uploaded"))
            {
                this.attributes.Add("bytes_uploaded", null);
            }
            if (!this.attributes.ContainsKey("bytes_downloaded"))
            {
                this.attributes.Add("bytes_downloaded", null);
            }
            if (!this.attributes.ContainsKey("list_count"))
            {
                this.attributes.Add("list_count", null);
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
        /// Remote Path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// End User IP
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("client_ip")]
        public string ClientIp
        {
            get { return (string)attributes["client_ip"]; }
            private set { attributes["client_ip"] = value; }
        }

        /// <summary>
        /// Source Remote Server ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("src_remote_server_id")]
        public Nullable<Int64> SrcRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["src_remote_server_id"]; }
            private set { attributes["src_remote_server_id"] = value; }
        }

        /// <summary>
        /// Destination Remote Server ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dest_remote_server_id")]
        public Nullable<Int64> DestRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["dest_remote_server_id"]; }
            private set { attributes["dest_remote_server_id"] = value; }
        }

        /// <summary>
        /// Operation Type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("operation")]
        public string Operation
        {
            get { return (string)attributes["operation"]; }
            private set { attributes["operation"] = value; }
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
        /// Error operation, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error_operation")]
        public string ErrorOperation
        {
            get { return (string)attributes["error_operation"]; }
            private set { attributes["error_operation"] = value; }
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
        /// Status
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
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
        /// Data Length in Bytes. Present for upload actions that transfer data.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes_uploaded")]
        public Nullable<Int64> BytesUploaded
        {
            get { return (Nullable<Int64>)attributes["bytes_uploaded"]; }
            private set { attributes["bytes_uploaded"] = value; }
        }

        /// <summary>
        /// Data Length in Bytes. Present for download actions that transfer data.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes_downloaded")]
        public Nullable<Int64> BytesDownloaded
        {
            get { return (Nullable<Int64>)attributes["bytes_downloaded"]; }
            private set { attributes["bytes_downloaded"] = value; }
        }

        /// <summary>
        /// Number of entries returned for a folder list action.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("list_count")]
        public Nullable<Int64> ListCount
        {
            get { return (Nullable<Int64>)attributes["list_count"]; }
            private set { attributes["list_count"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `operation`, `status`, `src_remote_server_id`, `dest_remote_server_id`, `path` or `client_ip`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ operation ]`, `[ status ]`, `[ src_remote_server_id ]`, `[ dest_remote_server_id ]`, `[ path ]`, `[ client_ip ]`, `[ start_date, end_date ]`, `[ start_date, operation ]`, `[ start_date, status ]`, `[ start_date, src_remote_server_id ]`, `[ start_date, dest_remote_server_id ]`, `[ start_date, path ]`, `[ start_date, client_ip ]`, `[ end_date, operation ]`, `[ end_date, status ]`, `[ end_date, src_remote_server_id ]`, `[ end_date, dest_remote_server_id ]`, `[ end_date, path ]`, `[ end_date, client_ip ]`, `[ operation, status ]`, `[ operation, src_remote_server_id ]`, `[ operation, dest_remote_server_id ]`, `[ operation, path ]`, `[ operation, client_ip ]`, `[ status, src_remote_server_id ]`, `[ status, dest_remote_server_id ]`, `[ status, path ]`, `[ status, client_ip ]`, `[ src_remote_server_id, dest_remote_server_id ]`, `[ src_remote_server_id, path ]`, `[ src_remote_server_id, client_ip ]`, `[ dest_remote_server_id, path ]`, `[ dest_remote_server_id, client_ip ]`, `[ path, client_ip ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, status ]`, `[ start_date, end_date, src_remote_server_id ]`, `[ start_date, end_date, dest_remote_server_id ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, client_ip ]`, `[ start_date, operation, status ]`, `[ start_date, operation, src_remote_server_id ]`, `[ start_date, operation, dest_remote_server_id ]`, `[ start_date, operation, path ]`, `[ start_date, operation, client_ip ]`, `[ start_date, status, src_remote_server_id ]`, `[ start_date, status, dest_remote_server_id ]`, `[ start_date, status, path ]`, `[ start_date, status, client_ip ]`, `[ start_date, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, src_remote_server_id, path ]`, `[ start_date, src_remote_server_id, client_ip ]`, `[ start_date, dest_remote_server_id, path ]`, `[ start_date, dest_remote_server_id, client_ip ]`, `[ start_date, path, client_ip ]`, `[ end_date, operation, status ]`, `[ end_date, operation, src_remote_server_id ]`, `[ end_date, operation, dest_remote_server_id ]`, `[ end_date, operation, path ]`, `[ end_date, operation, client_ip ]`, `[ end_date, status, src_remote_server_id ]`, `[ end_date, status, dest_remote_server_id ]`, `[ end_date, status, path ]`, `[ end_date, status, client_ip ]`, `[ end_date, src_remote_server_id, dest_remote_server_id ]`, `[ end_date, src_remote_server_id, path ]`, `[ end_date, src_remote_server_id, client_ip ]`, `[ end_date, dest_remote_server_id, path ]`, `[ end_date, dest_remote_server_id, client_ip ]`, `[ end_date, path, client_ip ]`, `[ operation, status, src_remote_server_id ]`, `[ operation, status, dest_remote_server_id ]`, `[ operation, status, path ]`, `[ operation, status, client_ip ]`, `[ operation, src_remote_server_id, dest_remote_server_id ]`, `[ operation, src_remote_server_id, path ]`, `[ operation, src_remote_server_id, client_ip ]`, `[ operation, dest_remote_server_id, path ]`, `[ operation, dest_remote_server_id, client_ip ]`, `[ operation, path, client_ip ]`, `[ status, src_remote_server_id, dest_remote_server_id ]`, `[ status, src_remote_server_id, path ]`, `[ status, src_remote_server_id, client_ip ]`, `[ status, dest_remote_server_id, path ]`, `[ status, dest_remote_server_id, client_ip ]`, `[ status, path, client_ip ]`, `[ src_remote_server_id, dest_remote_server_id, path ]`, `[ src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ src_remote_server_id, path, client_ip ]`, `[ dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, operation, src_remote_server_id ]`, `[ start_date, end_date, operation, dest_remote_server_id ]`, `[ start_date, end_date, operation, path ]`, `[ start_date, end_date, operation, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id ]`, `[ start_date, end_date, status, dest_remote_server_id ]`, `[ start_date, end_date, status, path ]`, `[ start_date, end_date, status, client_ip ]`, `[ start_date, end_date, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, end_date, src_remote_server_id, path ]`, `[ start_date, end_date, src_remote_server_id, client_ip ]`, `[ start_date, end_date, dest_remote_server_id, path ]`, `[ start_date, end_date, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, path, client_ip ]`, `[ start_date, operation, status, src_remote_server_id ]`, `[ start_date, operation, status, dest_remote_server_id ]`, `[ start_date, operation, status, path ]`, `[ start_date, operation, status, client_ip ]`, `[ start_date, operation, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, operation, src_remote_server_id, path ]`, `[ start_date, operation, src_remote_server_id, client_ip ]`, `[ start_date, operation, dest_remote_server_id, path ]`, `[ start_date, operation, dest_remote_server_id, client_ip ]`, `[ start_date, operation, path, client_ip ]`, `[ start_date, status, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, status, src_remote_server_id, path ]`, `[ start_date, status, src_remote_server_id, client_ip ]`, `[ start_date, status, dest_remote_server_id, path ]`, `[ start_date, status, dest_remote_server_id, client_ip ]`, `[ start_date, status, path, client_ip ]`, `[ start_date, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, src_remote_server_id, path, client_ip ]`, `[ start_date, dest_remote_server_id, path, client_ip ]`, `[ end_date, operation, status, src_remote_server_id ]`, `[ end_date, operation, status, dest_remote_server_id ]`, `[ end_date, operation, status, path ]`, `[ end_date, operation, status, client_ip ]`, `[ end_date, operation, src_remote_server_id, dest_remote_server_id ]`, `[ end_date, operation, src_remote_server_id, path ]`, `[ end_date, operation, src_remote_server_id, client_ip ]`, `[ end_date, operation, dest_remote_server_id, path ]`, `[ end_date, operation, dest_remote_server_id, client_ip ]`, `[ end_date, operation, path, client_ip ]`, `[ end_date, status, src_remote_server_id, dest_remote_server_id ]`, `[ end_date, status, src_remote_server_id, path ]`, `[ end_date, status, src_remote_server_id, client_ip ]`, `[ end_date, status, dest_remote_server_id, path ]`, `[ end_date, status, dest_remote_server_id, client_ip ]`, `[ end_date, status, path, client_ip ]`, `[ end_date, src_remote_server_id, dest_remote_server_id, path ]`, `[ end_date, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ end_date, src_remote_server_id, path, client_ip ]`, `[ end_date, dest_remote_server_id, path, client_ip ]`, `[ operation, status, src_remote_server_id, dest_remote_server_id ]`, `[ operation, status, src_remote_server_id, path ]`, `[ operation, status, src_remote_server_id, client_ip ]`, `[ operation, status, dest_remote_server_id, path ]`, `[ operation, status, dest_remote_server_id, client_ip ]`, `[ operation, status, path, client_ip ]`, `[ operation, src_remote_server_id, dest_remote_server_id, path ]`, `[ operation, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ operation, src_remote_server_id, path, client_ip ]`, `[ operation, dest_remote_server_id, path, client_ip ]`, `[ status, src_remote_server_id, dest_remote_server_id, path ]`, `[ status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ status, src_remote_server_id, path, client_ip ]`, `[ status, dest_remote_server_id, path, client_ip ]`, `[ src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status, src_remote_server_id ]`, `[ start_date, end_date, operation, status, dest_remote_server_id ]`, `[ start_date, end_date, operation, status, path ]`, `[ start_date, end_date, operation, status, client_ip ]`, `[ start_date, end_date, operation, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, end_date, operation, src_remote_server_id, path ]`, `[ start_date, end_date, operation, src_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, dest_remote_server_id, path ]`, `[ start_date, end_date, operation, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, path, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, end_date, status, src_remote_server_id, path ]`, `[ start_date, end_date, status, src_remote_server_id, client_ip ]`, `[ start_date, end_date, status, dest_remote_server_id, path ]`, `[ start_date, end_date, status, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, status, path, client_ip ]`, `[ start_date, end_date, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, end_date, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, src_remote_server_id, path, client_ip ]`, `[ start_date, end_date, dest_remote_server_id, path, client_ip ]`, `[ start_date, operation, status, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, operation, status, src_remote_server_id, path ]`, `[ start_date, operation, status, src_remote_server_id, client_ip ]`, `[ start_date, operation, status, dest_remote_server_id, path ]`, `[ start_date, operation, status, dest_remote_server_id, client_ip ]`, `[ start_date, operation, status, path, client_ip ]`, `[ start_date, operation, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, operation, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, operation, src_remote_server_id, path, client_ip ]`, `[ start_date, operation, dest_remote_server_id, path, client_ip ]`, `[ start_date, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, status, src_remote_server_id, path, client_ip ]`, `[ start_date, status, dest_remote_server_id, path, client_ip ]`, `[ start_date, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ end_date, operation, status, src_remote_server_id, dest_remote_server_id ]`, `[ end_date, operation, status, src_remote_server_id, path ]`, `[ end_date, operation, status, src_remote_server_id, client_ip ]`, `[ end_date, operation, status, dest_remote_server_id, path ]`, `[ end_date, operation, status, dest_remote_server_id, client_ip ]`, `[ end_date, operation, status, path, client_ip ]`, `[ end_date, operation, src_remote_server_id, dest_remote_server_id, path ]`, `[ end_date, operation, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ end_date, operation, src_remote_server_id, path, client_ip ]`, `[ end_date, operation, dest_remote_server_id, path, client_ip ]`, `[ end_date, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ end_date, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ end_date, status, src_remote_server_id, path, client_ip ]`, `[ end_date, status, dest_remote_server_id, path, client_ip ]`, `[ end_date, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ operation, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ operation, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ operation, status, src_remote_server_id, path, client_ip ]`, `[ operation, status, dest_remote_server_id, path, client_ip ]`, `[ operation, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, end_date, operation, status, src_remote_server_id, path ]`, `[ start_date, end_date, operation, status, src_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, status, dest_remote_server_id, path ]`, `[ start_date, end_date, operation, status, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, status, path, client_ip ]`, `[ start_date, end_date, operation, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, end_date, operation, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, src_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, end_date, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id, path, client_ip ]`, `[ start_date, end_date, status, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, operation, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, operation, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, operation, status, src_remote_server_id, path, client_ip ]`, `[ start_date, operation, status, dest_remote_server_id, path, client_ip ]`, `[ start_date, operation, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ end_date, operation, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ end_date, operation, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ end_date, operation, status, src_remote_server_id, path, client_ip ]`, `[ end_date, operation, status, dest_remote_server_id, path, client_ip ]`, `[ end_date, operation, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ end_date, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ operation, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, end_date, operation, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, status, src_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, operation, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]` or `[ end_date, operation, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `operation`, `status` or `path`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ operation ]`, `[ status ]`, `[ src_remote_server_id ]`, `[ dest_remote_server_id ]`, `[ path ]`, `[ client_ip ]`, `[ start_date, end_date ]`, `[ start_date, operation ]`, `[ start_date, status ]`, `[ start_date, src_remote_server_id ]`, `[ start_date, dest_remote_server_id ]`, `[ start_date, path ]`, `[ start_date, client_ip ]`, `[ end_date, operation ]`, `[ end_date, status ]`, `[ end_date, src_remote_server_id ]`, `[ end_date, dest_remote_server_id ]`, `[ end_date, path ]`, `[ end_date, client_ip ]`, `[ operation, status ]`, `[ operation, src_remote_server_id ]`, `[ operation, dest_remote_server_id ]`, `[ operation, path ]`, `[ operation, client_ip ]`, `[ status, src_remote_server_id ]`, `[ status, dest_remote_server_id ]`, `[ status, path ]`, `[ status, client_ip ]`, `[ src_remote_server_id, dest_remote_server_id ]`, `[ src_remote_server_id, path ]`, `[ src_remote_server_id, client_ip ]`, `[ dest_remote_server_id, path ]`, `[ dest_remote_server_id, client_ip ]`, `[ path, client_ip ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, status ]`, `[ start_date, end_date, src_remote_server_id ]`, `[ start_date, end_date, dest_remote_server_id ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, client_ip ]`, `[ start_date, operation, status ]`, `[ start_date, operation, src_remote_server_id ]`, `[ start_date, operation, dest_remote_server_id ]`, `[ start_date, operation, path ]`, `[ start_date, operation, client_ip ]`, `[ start_date, status, src_remote_server_id ]`, `[ start_date, status, dest_remote_server_id ]`, `[ start_date, status, path ]`, `[ start_date, status, client_ip ]`, `[ start_date, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, src_remote_server_id, path ]`, `[ start_date, src_remote_server_id, client_ip ]`, `[ start_date, dest_remote_server_id, path ]`, `[ start_date, dest_remote_server_id, client_ip ]`, `[ start_date, path, client_ip ]`, `[ end_date, operation, status ]`, `[ end_date, operation, src_remote_server_id ]`, `[ end_date, operation, dest_remote_server_id ]`, `[ end_date, operation, path ]`, `[ end_date, operation, client_ip ]`, `[ end_date, status, src_remote_server_id ]`, `[ end_date, status, dest_remote_server_id ]`, `[ end_date, status, path ]`, `[ end_date, status, client_ip ]`, `[ end_date, src_remote_server_id, dest_remote_server_id ]`, `[ end_date, src_remote_server_id, path ]`, `[ end_date, src_remote_server_id, client_ip ]`, `[ end_date, dest_remote_server_id, path ]`, `[ end_date, dest_remote_server_id, client_ip ]`, `[ end_date, path, client_ip ]`, `[ operation, status, src_remote_server_id ]`, `[ operation, status, dest_remote_server_id ]`, `[ operation, status, path ]`, `[ operation, status, client_ip ]`, `[ operation, src_remote_server_id, dest_remote_server_id ]`, `[ operation, src_remote_server_id, path ]`, `[ operation, src_remote_server_id, client_ip ]`, `[ operation, dest_remote_server_id, path ]`, `[ operation, dest_remote_server_id, client_ip ]`, `[ operation, path, client_ip ]`, `[ status, src_remote_server_id, dest_remote_server_id ]`, `[ status, src_remote_server_id, path ]`, `[ status, src_remote_server_id, client_ip ]`, `[ status, dest_remote_server_id, path ]`, `[ status, dest_remote_server_id, client_ip ]`, `[ status, path, client_ip ]`, `[ src_remote_server_id, dest_remote_server_id, path ]`, `[ src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ src_remote_server_id, path, client_ip ]`, `[ dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, operation, src_remote_server_id ]`, `[ start_date, end_date, operation, dest_remote_server_id ]`, `[ start_date, end_date, operation, path ]`, `[ start_date, end_date, operation, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id ]`, `[ start_date, end_date, status, dest_remote_server_id ]`, `[ start_date, end_date, status, path ]`, `[ start_date, end_date, status, client_ip ]`, `[ start_date, end_date, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, end_date, src_remote_server_id, path ]`, `[ start_date, end_date, src_remote_server_id, client_ip ]`, `[ start_date, end_date, dest_remote_server_id, path ]`, `[ start_date, end_date, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, path, client_ip ]`, `[ start_date, operation, status, src_remote_server_id ]`, `[ start_date, operation, status, dest_remote_server_id ]`, `[ start_date, operation, status, path ]`, `[ start_date, operation, status, client_ip ]`, `[ start_date, operation, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, operation, src_remote_server_id, path ]`, `[ start_date, operation, src_remote_server_id, client_ip ]`, `[ start_date, operation, dest_remote_server_id, path ]`, `[ start_date, operation, dest_remote_server_id, client_ip ]`, `[ start_date, operation, path, client_ip ]`, `[ start_date, status, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, status, src_remote_server_id, path ]`, `[ start_date, status, src_remote_server_id, client_ip ]`, `[ start_date, status, dest_remote_server_id, path ]`, `[ start_date, status, dest_remote_server_id, client_ip ]`, `[ start_date, status, path, client_ip ]`, `[ start_date, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, src_remote_server_id, path, client_ip ]`, `[ start_date, dest_remote_server_id, path, client_ip ]`, `[ end_date, operation, status, src_remote_server_id ]`, `[ end_date, operation, status, dest_remote_server_id ]`, `[ end_date, operation, status, path ]`, `[ end_date, operation, status, client_ip ]`, `[ end_date, operation, src_remote_server_id, dest_remote_server_id ]`, `[ end_date, operation, src_remote_server_id, path ]`, `[ end_date, operation, src_remote_server_id, client_ip ]`, `[ end_date, operation, dest_remote_server_id, path ]`, `[ end_date, operation, dest_remote_server_id, client_ip ]`, `[ end_date, operation, path, client_ip ]`, `[ end_date, status, src_remote_server_id, dest_remote_server_id ]`, `[ end_date, status, src_remote_server_id, path ]`, `[ end_date, status, src_remote_server_id, client_ip ]`, `[ end_date, status, dest_remote_server_id, path ]`, `[ end_date, status, dest_remote_server_id, client_ip ]`, `[ end_date, status, path, client_ip ]`, `[ end_date, src_remote_server_id, dest_remote_server_id, path ]`, `[ end_date, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ end_date, src_remote_server_id, path, client_ip ]`, `[ end_date, dest_remote_server_id, path, client_ip ]`, `[ operation, status, src_remote_server_id, dest_remote_server_id ]`, `[ operation, status, src_remote_server_id, path ]`, `[ operation, status, src_remote_server_id, client_ip ]`, `[ operation, status, dest_remote_server_id, path ]`, `[ operation, status, dest_remote_server_id, client_ip ]`, `[ operation, status, path, client_ip ]`, `[ operation, src_remote_server_id, dest_remote_server_id, path ]`, `[ operation, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ operation, src_remote_server_id, path, client_ip ]`, `[ operation, dest_remote_server_id, path, client_ip ]`, `[ status, src_remote_server_id, dest_remote_server_id, path ]`, `[ status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ status, src_remote_server_id, path, client_ip ]`, `[ status, dest_remote_server_id, path, client_ip ]`, `[ src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status, src_remote_server_id ]`, `[ start_date, end_date, operation, status, dest_remote_server_id ]`, `[ start_date, end_date, operation, status, path ]`, `[ start_date, end_date, operation, status, client_ip ]`, `[ start_date, end_date, operation, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, end_date, operation, src_remote_server_id, path ]`, `[ start_date, end_date, operation, src_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, dest_remote_server_id, path ]`, `[ start_date, end_date, operation, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, path, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, end_date, status, src_remote_server_id, path ]`, `[ start_date, end_date, status, src_remote_server_id, client_ip ]`, `[ start_date, end_date, status, dest_remote_server_id, path ]`, `[ start_date, end_date, status, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, status, path, client_ip ]`, `[ start_date, end_date, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, end_date, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, src_remote_server_id, path, client_ip ]`, `[ start_date, end_date, dest_remote_server_id, path, client_ip ]`, `[ start_date, operation, status, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, operation, status, src_remote_server_id, path ]`, `[ start_date, operation, status, src_remote_server_id, client_ip ]`, `[ start_date, operation, status, dest_remote_server_id, path ]`, `[ start_date, operation, status, dest_remote_server_id, client_ip ]`, `[ start_date, operation, status, path, client_ip ]`, `[ start_date, operation, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, operation, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, operation, src_remote_server_id, path, client_ip ]`, `[ start_date, operation, dest_remote_server_id, path, client_ip ]`, `[ start_date, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, status, src_remote_server_id, path, client_ip ]`, `[ start_date, status, dest_remote_server_id, path, client_ip ]`, `[ start_date, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ end_date, operation, status, src_remote_server_id, dest_remote_server_id ]`, `[ end_date, operation, status, src_remote_server_id, path ]`, `[ end_date, operation, status, src_remote_server_id, client_ip ]`, `[ end_date, operation, status, dest_remote_server_id, path ]`, `[ end_date, operation, status, dest_remote_server_id, client_ip ]`, `[ end_date, operation, status, path, client_ip ]`, `[ end_date, operation, src_remote_server_id, dest_remote_server_id, path ]`, `[ end_date, operation, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ end_date, operation, src_remote_server_id, path, client_ip ]`, `[ end_date, operation, dest_remote_server_id, path, client_ip ]`, `[ end_date, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ end_date, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ end_date, status, src_remote_server_id, path, client_ip ]`, `[ end_date, status, dest_remote_server_id, path, client_ip ]`, `[ end_date, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ operation, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ operation, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ operation, status, src_remote_server_id, path, client_ip ]`, `[ operation, status, dest_remote_server_id, path, client_ip ]`, `[ operation, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status, src_remote_server_id, dest_remote_server_id ]`, `[ start_date, end_date, operation, status, src_remote_server_id, path ]`, `[ start_date, end_date, operation, status, src_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, status, dest_remote_server_id, path ]`, `[ start_date, end_date, operation, status, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, status, path, client_ip ]`, `[ start_date, end_date, operation, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, end_date, operation, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, src_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, end_date, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id, path, client_ip ]`, `[ start_date, end_date, status, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, operation, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, operation, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, operation, status, src_remote_server_id, path, client_ip ]`, `[ start_date, operation, status, dest_remote_server_id, path, client_ip ]`, `[ start_date, operation, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ end_date, operation, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ end_date, operation, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ end_date, operation, status, src_remote_server_id, path, client_ip ]`, `[ end_date, operation, status, dest_remote_server_id, path, client_ip ]`, `[ end_date, operation, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ end_date, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ operation, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status, src_remote_server_id, dest_remote_server_id, path ]`, `[ start_date, end_date, operation, status, src_remote_server_id, dest_remote_server_id, client_ip ]`, `[ start_date, end_date, operation, status, src_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, status, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, operation, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, end_date, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`, `[ start_date, operation, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]` or `[ end_date, operation, status, src_remote_server_id, dest_remote_server_id, path, client_ip ]`.
        /// </summary>
        public static FilesList<OutboundConnectionLog> List(

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

            return new FilesList<OutboundConnectionLog>($"/outbound_connection_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<OutboundConnectionLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}