using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class FileMigrationLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FileMigrationLog() : this(null, null) { }

        public FileMigrationLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("file_migration_id"))
            {
                this.attributes.Add("file_migration_id", null);
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
            if (!this.attributes.ContainsKey("type"))
            {
                this.attributes.Add("type", null);
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
        /// File Migration ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_migration_id")]
        public Nullable<Int64> FileMigrationId
        {
            get { return (Nullable<Int64>)attributes["file_migration_id"]; }
            private set { attributes["file_migration_id"] = value; }
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
        /// Operation type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string)attributes["type"]; }
            private set { attributes["type"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `file_migration_id`, `operation`, `status` or `type`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ file_migration_id ]`, `[ operation ]`, `[ status ]`, `[ type ]`, `[ start_date, end_date ]`, `[ start_date, file_migration_id ]`, `[ start_date, operation ]`, `[ start_date, status ]`, `[ start_date, type ]`, `[ end_date, file_migration_id ]`, `[ end_date, operation ]`, `[ end_date, status ]`, `[ end_date, type ]`, `[ file_migration_id, operation ]`, `[ file_migration_id, status ]`, `[ file_migration_id, type ]`, `[ operation, status ]`, `[ operation, type ]`, `[ status, type ]`, `[ start_date, end_date, file_migration_id ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, status ]`, `[ start_date, end_date, type ]`, `[ start_date, file_migration_id, operation ]`, `[ start_date, file_migration_id, status ]`, `[ start_date, file_migration_id, type ]`, `[ start_date, operation, status ]`, `[ start_date, operation, type ]`, `[ start_date, status, type ]`, `[ end_date, file_migration_id, operation ]`, `[ end_date, file_migration_id, status ]`, `[ end_date, file_migration_id, type ]`, `[ end_date, operation, status ]`, `[ end_date, operation, type ]`, `[ end_date, status, type ]`, `[ file_migration_id, operation, status ]`, `[ file_migration_id, operation, type ]`, `[ file_migration_id, status, type ]`, `[ operation, status, type ]`, `[ start_date, end_date, file_migration_id, operation ]`, `[ start_date, end_date, file_migration_id, status ]`, `[ start_date, end_date, file_migration_id, type ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, operation, type ]`, `[ start_date, end_date, status, type ]`, `[ start_date, file_migration_id, operation, status ]`, `[ start_date, file_migration_id, operation, type ]`, `[ start_date, file_migration_id, status, type ]`, `[ start_date, operation, status, type ]`, `[ end_date, file_migration_id, operation, status ]`, `[ end_date, file_migration_id, operation, type ]`, `[ end_date, file_migration_id, status, type ]`, `[ end_date, operation, status, type ]`, `[ file_migration_id, operation, status, type ]`, `[ start_date, end_date, file_migration_id, operation, status ]`, `[ start_date, end_date, file_migration_id, operation, type ]`, `[ start_date, end_date, file_migration_id, status, type ]`, `[ start_date, end_date, operation, status, type ]`, `[ start_date, file_migration_id, operation, status, type ]` or `[ end_date, file_migration_id, operation, status, type ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `operation`, `status` or `type`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ file_migration_id ]`, `[ operation ]`, `[ status ]`, `[ type ]`, `[ start_date, end_date ]`, `[ start_date, file_migration_id ]`, `[ start_date, operation ]`, `[ start_date, status ]`, `[ start_date, type ]`, `[ end_date, file_migration_id ]`, `[ end_date, operation ]`, `[ end_date, status ]`, `[ end_date, type ]`, `[ file_migration_id, operation ]`, `[ file_migration_id, status ]`, `[ file_migration_id, type ]`, `[ operation, status ]`, `[ operation, type ]`, `[ status, type ]`, `[ start_date, end_date, file_migration_id ]`, `[ start_date, end_date, operation ]`, `[ start_date, end_date, status ]`, `[ start_date, end_date, type ]`, `[ start_date, file_migration_id, operation ]`, `[ start_date, file_migration_id, status ]`, `[ start_date, file_migration_id, type ]`, `[ start_date, operation, status ]`, `[ start_date, operation, type ]`, `[ start_date, status, type ]`, `[ end_date, file_migration_id, operation ]`, `[ end_date, file_migration_id, status ]`, `[ end_date, file_migration_id, type ]`, `[ end_date, operation, status ]`, `[ end_date, operation, type ]`, `[ end_date, status, type ]`, `[ file_migration_id, operation, status ]`, `[ file_migration_id, operation, type ]`, `[ file_migration_id, status, type ]`, `[ operation, status, type ]`, `[ start_date, end_date, file_migration_id, operation ]`, `[ start_date, end_date, file_migration_id, status ]`, `[ start_date, end_date, file_migration_id, type ]`, `[ start_date, end_date, operation, status ]`, `[ start_date, end_date, operation, type ]`, `[ start_date, end_date, status, type ]`, `[ start_date, file_migration_id, operation, status ]`, `[ start_date, file_migration_id, operation, type ]`, `[ start_date, file_migration_id, status, type ]`, `[ start_date, operation, status, type ]`, `[ end_date, file_migration_id, operation, status ]`, `[ end_date, file_migration_id, operation, type ]`, `[ end_date, file_migration_id, status, type ]`, `[ end_date, operation, status, type ]`, `[ file_migration_id, operation, status, type ]`, `[ start_date, end_date, file_migration_id, operation, status ]`, `[ start_date, end_date, file_migration_id, operation, type ]`, `[ start_date, end_date, file_migration_id, status, type ]`, `[ start_date, end_date, operation, status, type ]`, `[ start_date, file_migration_id, operation, status, type ]` or `[ end_date, file_migration_id, operation, status, type ]`.
        /// </summary>
        public static FilesList<FileMigrationLog> List(

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

            return new FilesList<FileMigrationLog>($"/file_migration_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<FileMigrationLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}