using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class SyncLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public SyncLog() : this(null, null) { }

        public SyncLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("sync_id"))
            {
                this.attributes.Add("sync_id", null);
            }
            if (!this.attributes.ContainsKey("external_event_id"))
            {
                this.attributes.Add("external_event_id", null);
            }
            if (!this.attributes.ContainsKey("error_type"))
            {
                this.attributes.Add("error_type", null);
            }
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("operation"))
            {
                this.attributes.Add("operation", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("size"))
            {
                this.attributes.Add("size", null);
            }
            if (!this.attributes.ContainsKey("file_type"))
            {
                this.attributes.Add("file_type", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
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
        /// Sync ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_id")]
        public Nullable<Int64> SyncId
        {
            get { return (Nullable<Int64>)attributes["sync_id"]; }
            private set { attributes["sync_id"] = value; }
        }

        /// <summary>
        /// External Event ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("external_event_id")]
        public Nullable<Int64> ExternalEventId
        {
            get { return (Nullable<Int64>)attributes["external_event_id"]; }
            private set { attributes["external_event_id"] = value; }
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
        /// Message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            private set { attributes["message"] = value; }
        }

        /// <summary>
        /// Operation type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("operation")]
        public string Operation
        {
            get { return (string)attributes["operation"]; }
            private set { attributes["operation"] = value; }
        }

        /// <summary>
        /// File path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// File size
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("size")]
        public string Size
        {
            get { return (string)attributes["size"]; }
            private set { attributes["size"] = value; }
        }

        /// <summary>
        /// File type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_type")]
        public string FileType
        {
            get { return (string)attributes["file_type"]; }
            private set { attributes["file_type"] = value; }
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
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `external_event_id`, `operation`, `status` or `sync_id`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ start_date, end_date ]`, `[ start_date, external_event_id ]`, `[ start_date, operation ]`, `[ start_date, status ]`, `[ start_date, sync_id ]`, `[ end_date, external_event_id ]`, `[ end_date, operation ]`, `[ end_date, status ]`, `[ end_date, sync_id ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ status, sync_id ]`, `[ start_date, end_date, external_event_id ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, status ]`, `[ start_date, end_date, sync_id ]`, `[ start_date, external_event_id, operation ]`, `[ start_date, external_event_id, status ]`, `[ start_date, external_event_id, sync_id ]`, `[ start_date, operation, status ]`, `[ start_date, operation, sync_id ]`, `[ start_date, status, sync_id ]`, `[ end_date, external_event_id, operation ]`, `[ end_date, external_event_id, status ]`, `[ end_date, external_event_id, sync_id ]`, `[ end_date, operation, status ]`, `[ end_date, operation, sync_id ]`, `[ end_date, status, sync_id ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, status, sync_id ]`, `[ operation, status, sync_id ]`, `[ start_date, end_date, external_event_id, operation ]`, `[ start_date, end_date, external_event_id, status ]`, `[ start_date, end_date, external_event_id, sync_id ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, operation, sync_id ]`, `[ start_date, end_date, status, sync_id ]`, `[ start_date, external_event_id, operation, status ]`, `[ start_date, external_event_id, operation, sync_id ]`, `[ start_date, external_event_id, status, sync_id ]`, `[ start_date, operation, status, sync_id ]`, `[ end_date, external_event_id, operation, status ]`, `[ end_date, external_event_id, operation, sync_id ]`, `[ end_date, external_event_id, status, sync_id ]`, `[ end_date, operation, status, sync_id ]`, `[ external_event_id, operation, status, sync_id ]`, `[ start_date, end_date, external_event_id, operation, status ]`, `[ start_date, end_date, external_event_id, operation, sync_id ]`, `[ start_date, end_date, external_event_id, status, sync_id ]`, `[ start_date, end_date, operation, status, sync_id ]`, `[ start_date, external_event_id, operation, status, sync_id ]` or `[ end_date, external_event_id, operation, status, sync_id ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `operation` and `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ start_date, end_date ]`, `[ start_date, external_event_id ]`, `[ start_date, operation ]`, `[ start_date, status ]`, `[ start_date, sync_id ]`, `[ end_date, external_event_id ]`, `[ end_date, operation ]`, `[ end_date, status ]`, `[ end_date, sync_id ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ status, sync_id ]`, `[ start_date, end_date, external_event_id ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, status ]`, `[ start_date, end_date, sync_id ]`, `[ start_date, external_event_id, operation ]`, `[ start_date, external_event_id, status ]`, `[ start_date, external_event_id, sync_id ]`, `[ start_date, operation, status ]`, `[ start_date, operation, sync_id ]`, `[ start_date, status, sync_id ]`, `[ end_date, external_event_id, operation ]`, `[ end_date, external_event_id, status ]`, `[ end_date, external_event_id, sync_id ]`, `[ end_date, operation, status ]`, `[ end_date, operation, sync_id ]`, `[ end_date, status, sync_id ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, status, sync_id ]`, `[ operation, status, sync_id ]`, `[ start_date, end_date, external_event_id, operation ]`, `[ start_date, end_date, external_event_id, status ]`, `[ start_date, end_date, external_event_id, sync_id ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, operation, sync_id ]`, `[ start_date, end_date, status, sync_id ]`, `[ start_date, external_event_id, operation, status ]`, `[ start_date, external_event_id, operation, sync_id ]`, `[ start_date, external_event_id, status, sync_id ]`, `[ start_date, operation, status, sync_id ]`, `[ end_date, external_event_id, operation, status ]`, `[ end_date, external_event_id, operation, sync_id ]`, `[ end_date, external_event_id, status, sync_id ]`, `[ end_date, operation, status, sync_id ]`, `[ external_event_id, operation, status, sync_id ]`, `[ start_date, end_date, external_event_id, operation, status ]`, `[ start_date, end_date, external_event_id, operation, sync_id ]`, `[ start_date, end_date, external_event_id, status, sync_id ]`, `[ start_date, end_date, operation, status, sync_id ]`, `[ start_date, external_event_id, operation, status, sync_id ]` or `[ end_date, external_event_id, operation, status, sync_id ]`.
        /// </summary>
        public static FilesList<SyncLog> List(

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

            return new FilesList<SyncLog>($"/sync_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<SyncLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `external_event_id`, `operation`, `status` or `sync_id`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ start_date, end_date ]`, `[ start_date, external_event_id ]`, `[ start_date, operation ]`, `[ start_date, status ]`, `[ start_date, sync_id ]`, `[ end_date, external_event_id ]`, `[ end_date, operation ]`, `[ end_date, status ]`, `[ end_date, sync_id ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ status, sync_id ]`, `[ start_date, end_date, external_event_id ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, status ]`, `[ start_date, end_date, sync_id ]`, `[ start_date, external_event_id, operation ]`, `[ start_date, external_event_id, status ]`, `[ start_date, external_event_id, sync_id ]`, `[ start_date, operation, status ]`, `[ start_date, operation, sync_id ]`, `[ start_date, status, sync_id ]`, `[ end_date, external_event_id, operation ]`, `[ end_date, external_event_id, status ]`, `[ end_date, external_event_id, sync_id ]`, `[ end_date, operation, status ]`, `[ end_date, operation, sync_id ]`, `[ end_date, status, sync_id ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, status, sync_id ]`, `[ operation, status, sync_id ]`, `[ start_date, end_date, external_event_id, operation ]`, `[ start_date, end_date, external_event_id, status ]`, `[ start_date, end_date, external_event_id, sync_id ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, operation, sync_id ]`, `[ start_date, end_date, status, sync_id ]`, `[ start_date, external_event_id, operation, status ]`, `[ start_date, external_event_id, operation, sync_id ]`, `[ start_date, external_event_id, status, sync_id ]`, `[ start_date, operation, status, sync_id ]`, `[ end_date, external_event_id, operation, status ]`, `[ end_date, external_event_id, operation, sync_id ]`, `[ end_date, external_event_id, status, sync_id ]`, `[ end_date, operation, status, sync_id ]`, `[ external_event_id, operation, status, sync_id ]`, `[ start_date, end_date, external_event_id, operation, status ]`, `[ start_date, end_date, external_event_id, operation, sync_id ]`, `[ start_date, end_date, external_event_id, status, sync_id ]`, `[ start_date, end_date, operation, status, sync_id ]`, `[ start_date, external_event_id, operation, status, sync_id ]` or `[ end_date, external_event_id, operation, status, sync_id ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `operation` and `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ start_date, end_date ]`, `[ start_date, external_event_id ]`, `[ start_date, operation ]`, `[ start_date, status ]`, `[ start_date, sync_id ]`, `[ end_date, external_event_id ]`, `[ end_date, operation ]`, `[ end_date, status ]`, `[ end_date, sync_id ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ status, sync_id ]`, `[ start_date, end_date, external_event_id ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, status ]`, `[ start_date, end_date, sync_id ]`, `[ start_date, external_event_id, operation ]`, `[ start_date, external_event_id, status ]`, `[ start_date, external_event_id, sync_id ]`, `[ start_date, operation, status ]`, `[ start_date, operation, sync_id ]`, `[ start_date, status, sync_id ]`, `[ end_date, external_event_id, operation ]`, `[ end_date, external_event_id, status ]`, `[ end_date, external_event_id, sync_id ]`, `[ end_date, operation, status ]`, `[ end_date, operation, sync_id ]`, `[ end_date, status, sync_id ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, status, sync_id ]`, `[ operation, status, sync_id ]`, `[ start_date, end_date, external_event_id, operation ]`, `[ start_date, end_date, external_event_id, status ]`, `[ start_date, end_date, external_event_id, sync_id ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, operation, sync_id ]`, `[ start_date, end_date, status, sync_id ]`, `[ start_date, external_event_id, operation, status ]`, `[ start_date, external_event_id, operation, sync_id ]`, `[ start_date, external_event_id, status, sync_id ]`, `[ start_date, operation, status, sync_id ]`, `[ end_date, external_event_id, operation, status ]`, `[ end_date, external_event_id, operation, sync_id ]`, `[ end_date, external_event_id, status, sync_id ]`, `[ end_date, operation, status, sync_id ]`, `[ external_event_id, operation, status, sync_id ]`, `[ start_date, end_date, external_event_id, operation, status ]`, `[ start_date, end_date, external_event_id, operation, sync_id ]`, `[ start_date, end_date, external_event_id, status, sync_id ]`, `[ start_date, end_date, operation, status, sync_id ]`, `[ start_date, external_event_id, operation, status, sync_id ]` or `[ end_date, external_event_id, operation, status, sync_id ]`.
        /// </summary>
        public static async Task<Export> CreateExport(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/sync_logs/create_export", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Export>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}