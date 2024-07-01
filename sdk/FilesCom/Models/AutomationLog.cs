using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class AutomationLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public AutomationLog() : this(null, null) { }

        public AutomationLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("automation_id"))
            {
                this.attributes.Add("automation_id", null);
            }
            if (!this.attributes.ContainsKey("automation_run_id"))
            {
                this.attributes.Add("automation_run_id", null);
            }
            if (!this.attributes.ContainsKey("dest_path"))
            {
                this.attributes.Add("dest_path", null);
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
        /// Automation ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("automation_id")]
        public Nullable<Int64> AutomationId
        {
            get { return (Nullable<Int64>)attributes["automation_id"]; }
            private set { attributes["automation_id"] = value; }
        }

        /// <summary>
        /// Automation Run ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("automation_run_id")]
        public Nullable<Int64> AutomationRunId
        {
            get { return (Nullable<Int64>)attributes["automation_run_id"]; }
            private set { attributes["automation_run_id"] = value; }
        }

        /// <summary>
        /// Destination path, for moves and copies
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dest_path")]
        public string DestPath
        {
            get { return (string)attributes["dest_path"]; }
            private set { attributes["dest_path"] = value; }
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
        /// File path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
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
        ///   action - string
        ///   page - int64
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `automation_id`, `automation_run_id`, `operation`, `path` or `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ automation_id ]`, `[ automation_run_id ]`, `[ operation ]`, `[ path ]`, `[ status ]`, `[ start_date, end_date ]`, `[ start_date, automation_id ]`, `[ start_date, automation_run_id ]`, `[ start_date, operation ]`, `[ start_date, path ]`, `[ start_date, status ]`, `[ end_date, automation_id ]`, `[ end_date, automation_run_id ]`, `[ end_date, operation ]`, `[ end_date, path ]`, `[ end_date, status ]`, `[ automation_id, automation_run_id ]`, `[ automation_id, operation ]`, `[ automation_id, path ]`, `[ automation_id, status ]`, `[ automation_run_id, operation ]`, `[ automation_run_id, path ]`, `[ automation_run_id, status ]`, `[ operation, path ]`, `[ operation, status ]`, `[ path, status ]`, `[ start_date, end_date, automation_id ]`, `[ start_date, end_date, automation_run_id ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, status ]`, `[ start_date, automation_id, automation_run_id ]`, `[ start_date, automation_id, operation ]`, `[ start_date, automation_id, path ]`, `[ start_date, automation_id, status ]`, `[ start_date, automation_run_id, operation ]`, `[ start_date, automation_run_id, path ]`, `[ start_date, automation_run_id, status ]`, `[ start_date, operation, path ]`, `[ start_date, operation, status ]`, `[ start_date, path, status ]`, `[ end_date, automation_id, automation_run_id ]`, `[ end_date, automation_id, operation ]`, `[ end_date, automation_id, path ]`, `[ end_date, automation_id, status ]`, `[ end_date, automation_run_id, operation ]`, `[ end_date, automation_run_id, path ]`, `[ end_date, automation_run_id, status ]`, `[ end_date, operation, path ]`, `[ end_date, operation, status ]`, `[ end_date, path, status ]`, `[ automation_id, automation_run_id, operation ]`, `[ automation_id, automation_run_id, path ]`, `[ automation_id, automation_run_id, status ]`, `[ automation_id, operation, path ]`, `[ automation_id, operation, status ]`, `[ automation_id, path, status ]`, `[ automation_run_id, operation, path ]`, `[ automation_run_id, operation, status ]`, `[ automation_run_id, path, status ]`, `[ operation, path, status ]`, `[ start_date, end_date, automation_id, automation_run_id ]`, `[ start_date, end_date, automation_id, operation ]`, `[ start_date, end_date, automation_id, path ]`, `[ start_date, end_date, automation_id, status ]`, `[ start_date, end_date, automation_run_id, operation ]`, `[ start_date, end_date, automation_run_id, path ]`, `[ start_date, end_date, automation_run_id, status ]`, `[ start_date, end_date, operation, path ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, path, status ]`, `[ start_date, automation_id, automation_run_id, operation ]`, `[ start_date, automation_id, automation_run_id, path ]`, `[ start_date, automation_id, automation_run_id, status ]`, `[ start_date, automation_id, operation, path ]`, `[ start_date, automation_id, operation, status ]`, `[ start_date, automation_id, path, status ]`, `[ start_date, automation_run_id, operation, path ]`, `[ start_date, automation_run_id, operation, status ]`, `[ start_date, automation_run_id, path, status ]`, `[ start_date, operation, path, status ]`, `[ end_date, automation_id, automation_run_id, operation ]`, `[ end_date, automation_id, automation_run_id, path ]`, `[ end_date, automation_id, automation_run_id, status ]`, `[ end_date, automation_id, operation, path ]`, `[ end_date, automation_id, operation, status ]`, `[ end_date, automation_id, path, status ]`, `[ end_date, automation_run_id, operation, path ]`, `[ end_date, automation_run_id, operation, status ]`, `[ end_date, automation_run_id, path, status ]`, `[ end_date, operation, path, status ]`, `[ automation_id, automation_run_id, operation, path ]`, `[ automation_id, automation_run_id, operation, status ]`, `[ automation_id, automation_run_id, path, status ]`, `[ automation_id, operation, path, status ]`, `[ automation_run_id, operation, path, status ]`, `[ start_date, end_date, automation_id, automation_run_id, operation ]`, `[ start_date, end_date, automation_id, automation_run_id, path ]`, `[ start_date, end_date, automation_id, automation_run_id, status ]`, `[ start_date, end_date, automation_id, operation, path ]`, `[ start_date, end_date, automation_id, operation, status ]`, `[ start_date, end_date, automation_id, path, status ]`, `[ start_date, end_date, automation_run_id, operation, path ]`, `[ start_date, end_date, automation_run_id, operation, status ]`, `[ start_date, end_date, automation_run_id, path, status ]`, `[ start_date, end_date, operation, path, status ]`, `[ start_date, automation_id, automation_run_id, operation, path ]`, `[ start_date, automation_id, automation_run_id, operation, status ]`, `[ start_date, automation_id, automation_run_id, path, status ]`, `[ start_date, automation_id, operation, path, status ]`, `[ start_date, automation_run_id, operation, path, status ]`, `[ end_date, automation_id, automation_run_id, operation, path ]`, `[ end_date, automation_id, automation_run_id, operation, status ]`, `[ end_date, automation_id, automation_run_id, path, status ]`, `[ end_date, automation_id, operation, path, status ]`, `[ end_date, automation_run_id, operation, path, status ]`, `[ automation_id, automation_run_id, operation, path, status ]`, `[ start_date, end_date, automation_id, automation_run_id, operation, path ]`, `[ start_date, end_date, automation_id, automation_run_id, operation, status ]`, `[ start_date, end_date, automation_id, automation_run_id, path, status ]`, `[ start_date, end_date, automation_id, operation, path, status ]`, `[ start_date, end_date, automation_run_id, operation, path, status ]`, `[ start_date, automation_id, automation_run_id, operation, path, status ]` or `[ end_date, automation_id, automation_run_id, operation, path, status ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `operation`, `path` or `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ automation_id ]`, `[ automation_run_id ]`, `[ operation ]`, `[ path ]`, `[ status ]`, `[ start_date, end_date ]`, `[ start_date, automation_id ]`, `[ start_date, automation_run_id ]`, `[ start_date, operation ]`, `[ start_date, path ]`, `[ start_date, status ]`, `[ end_date, automation_id ]`, `[ end_date, automation_run_id ]`, `[ end_date, operation ]`, `[ end_date, path ]`, `[ end_date, status ]`, `[ automation_id, automation_run_id ]`, `[ automation_id, operation ]`, `[ automation_id, path ]`, `[ automation_id, status ]`, `[ automation_run_id, operation ]`, `[ automation_run_id, path ]`, `[ automation_run_id, status ]`, `[ operation, path ]`, `[ operation, status ]`, `[ path, status ]`, `[ start_date, end_date, automation_id ]`, `[ start_date, end_date, automation_run_id ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, status ]`, `[ start_date, automation_id, automation_run_id ]`, `[ start_date, automation_id, operation ]`, `[ start_date, automation_id, path ]`, `[ start_date, automation_id, status ]`, `[ start_date, automation_run_id, operation ]`, `[ start_date, automation_run_id, path ]`, `[ start_date, automation_run_id, status ]`, `[ start_date, operation, path ]`, `[ start_date, operation, status ]`, `[ start_date, path, status ]`, `[ end_date, automation_id, automation_run_id ]`, `[ end_date, automation_id, operation ]`, `[ end_date, automation_id, path ]`, `[ end_date, automation_id, status ]`, `[ end_date, automation_run_id, operation ]`, `[ end_date, automation_run_id, path ]`, `[ end_date, automation_run_id, status ]`, `[ end_date, operation, path ]`, `[ end_date, operation, status ]`, `[ end_date, path, status ]`, `[ automation_id, automation_run_id, operation ]`, `[ automation_id, automation_run_id, path ]`, `[ automation_id, automation_run_id, status ]`, `[ automation_id, operation, path ]`, `[ automation_id, operation, status ]`, `[ automation_id, path, status ]`, `[ automation_run_id, operation, path ]`, `[ automation_run_id, operation, status ]`, `[ automation_run_id, path, status ]`, `[ operation, path, status ]`, `[ start_date, end_date, automation_id, automation_run_id ]`, `[ start_date, end_date, automation_id, operation ]`, `[ start_date, end_date, automation_id, path ]`, `[ start_date, end_date, automation_id, status ]`, `[ start_date, end_date, automation_run_id, operation ]`, `[ start_date, end_date, automation_run_id, path ]`, `[ start_date, end_date, automation_run_id, status ]`, `[ start_date, end_date, operation, path ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, path, status ]`, `[ start_date, automation_id, automation_run_id, operation ]`, `[ start_date, automation_id, automation_run_id, path ]`, `[ start_date, automation_id, automation_run_id, status ]`, `[ start_date, automation_id, operation, path ]`, `[ start_date, automation_id, operation, status ]`, `[ start_date, automation_id, path, status ]`, `[ start_date, automation_run_id, operation, path ]`, `[ start_date, automation_run_id, operation, status ]`, `[ start_date, automation_run_id, path, status ]`, `[ start_date, operation, path, status ]`, `[ end_date, automation_id, automation_run_id, operation ]`, `[ end_date, automation_id, automation_run_id, path ]`, `[ end_date, automation_id, automation_run_id, status ]`, `[ end_date, automation_id, operation, path ]`, `[ end_date, automation_id, operation, status ]`, `[ end_date, automation_id, path, status ]`, `[ end_date, automation_run_id, operation, path ]`, `[ end_date, automation_run_id, operation, status ]`, `[ end_date, automation_run_id, path, status ]`, `[ end_date, operation, path, status ]`, `[ automation_id, automation_run_id, operation, path ]`, `[ automation_id, automation_run_id, operation, status ]`, `[ automation_id, automation_run_id, path, status ]`, `[ automation_id, operation, path, status ]`, `[ automation_run_id, operation, path, status ]`, `[ start_date, end_date, automation_id, automation_run_id, operation ]`, `[ start_date, end_date, automation_id, automation_run_id, path ]`, `[ start_date, end_date, automation_id, automation_run_id, status ]`, `[ start_date, end_date, automation_id, operation, path ]`, `[ start_date, end_date, automation_id, operation, status ]`, `[ start_date, end_date, automation_id, path, status ]`, `[ start_date, end_date, automation_run_id, operation, path ]`, `[ start_date, end_date, automation_run_id, operation, status ]`, `[ start_date, end_date, automation_run_id, path, status ]`, `[ start_date, end_date, operation, path, status ]`, `[ start_date, automation_id, automation_run_id, operation, path ]`, `[ start_date, automation_id, automation_run_id, operation, status ]`, `[ start_date, automation_id, automation_run_id, path, status ]`, `[ start_date, automation_id, operation, path, status ]`, `[ start_date, automation_run_id, operation, path, status ]`, `[ end_date, automation_id, automation_run_id, operation, path ]`, `[ end_date, automation_id, automation_run_id, operation, status ]`, `[ end_date, automation_id, automation_run_id, path, status ]`, `[ end_date, automation_id, operation, path, status ]`, `[ end_date, automation_run_id, operation, path, status ]`, `[ automation_id, automation_run_id, operation, path, status ]`, `[ start_date, end_date, automation_id, automation_run_id, operation, path ]`, `[ start_date, end_date, automation_id, automation_run_id, operation, status ]`, `[ start_date, end_date, automation_id, automation_run_id, path, status ]`, `[ start_date, end_date, automation_id, operation, path, status ]`, `[ start_date, end_date, automation_run_id, operation, path, status ]`, `[ start_date, automation_id, automation_run_id, operation, path, status ]` or `[ end_date, automation_id, automation_run_id, operation, path, status ]`.
        /// </summary>
        public static FilesList<AutomationLog> List(

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

            return new FilesList<AutomationLog>($"/automation_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<AutomationLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}