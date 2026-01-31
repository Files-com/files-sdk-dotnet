using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class UsageDailySnapshot
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public UsageDailySnapshot() : this(null, null) { }

        public UsageDailySnapshot(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("date"))
            {
                this.attributes.Add("date", null);
            }
            if (!this.attributes.ContainsKey("api_usage_available"))
            {
                this.attributes.Add("api_usage_available", false);
            }
            if (!this.attributes.ContainsKey("read_api_usage"))
            {
                this.attributes.Add("read_api_usage", null);
            }
            if (!this.attributes.ContainsKey("write_api_usage"))
            {
                this.attributes.Add("write_api_usage", null);
            }
            if (!this.attributes.ContainsKey("user_count"))
            {
                this.attributes.Add("user_count", null);
            }
            if (!this.attributes.ContainsKey("current_storage"))
            {
                this.attributes.Add("current_storage", null);
            }
            if (!this.attributes.ContainsKey("deleted_files_storage"))
            {
                this.attributes.Add("deleted_files_storage", null);
            }
            if (!this.attributes.ContainsKey("deleted_files_counted_in_minimum"))
            {
                this.attributes.Add("deleted_files_counted_in_minimum", null);
            }
            if (!this.attributes.ContainsKey("root_storage"))
            {
                this.attributes.Add("root_storage", null);
            }
            if (!this.attributes.ContainsKey("usage_by_top_level_dir"))
            {
                this.attributes.Add("usage_by_top_level_dir", new object[0]);
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
        /// ID of the usage record
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// The date of this usage record
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("date")]
        public Nullable<DateTime> Date
        {
            get { return (Nullable<DateTime>)attributes["date"]; }
            private set { attributes["date"] = value; }
        }

        /// <summary>
        /// True if the API usage fields `read_api_usage` and `write_api_usage` can be relied upon.  If this is false, we suggest hiding that value from any UI.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("api_usage_available")]
        public bool ApiUsageAvailable
        {
            get { return attributes["api_usage_available"] == null ? false : (bool)attributes["api_usage_available"]; }
            private set { attributes["api_usage_available"] = value; }
        }

        /// <summary>
        /// Read API Calls used on this day. Note: only updated for days before the current day.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("read_api_usage")]
        public Nullable<Int64> ReadApiUsage
        {
            get { return (Nullable<Int64>)attributes["read_api_usage"]; }
            private set { attributes["read_api_usage"] = value; }
        }

        /// <summary>
        /// Write API Calls used on this day. Note: only updated for days before the current day.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("write_api_usage")]
        public Nullable<Int64> WriteApiUsage
        {
            get { return (Nullable<Int64>)attributes["write_api_usage"]; }
            private set { attributes["write_api_usage"] = value; }
        }

        /// <summary>
        /// Number of billable users as of this day.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_count")]
        public Nullable<Int64> UserCount
        {
            get { return (Nullable<Int64>)attributes["user_count"]; }
            private set { attributes["user_count"] = value; }
        }

        /// <summary>
        /// GB of Files Native Storage used on this day.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("current_storage")]
        public Nullable<Int64> CurrentStorage
        {
            get { return (Nullable<Int64>)attributes["current_storage"]; }
            private set { attributes["current_storage"] = value; }
        }

        /// <summary>
        /// GB of Files Native Storage used on this day for files that have been deleted and are stored as backups.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("deleted_files_storage")]
        public Nullable<Int64> DeletedFilesStorage
        {
            get { return (Nullable<Int64>)attributes["deleted_files_storage"]; }
            private set { attributes["deleted_files_storage"] = value; }
        }

        /// <summary>
        /// GB of Files Native Storage used on this day for files that have been permanently deleted but were uploaded less than 30 days ago, and are still billable.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("deleted_files_counted_in_minimum")]
        public Nullable<Int64> DeletedFilesCountedInMinimum
        {
            get { return (Nullable<Int64>)attributes["deleted_files_counted_in_minimum"]; }
            private set { attributes["deleted_files_counted_in_minimum"] = value; }
        }

        /// <summary>
        /// GB of Files Native Storage used for the root folder.  Included here because this value will not be part of `usage_by_top_level_dir`
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("root_storage")]
        public Nullable<Int64> RootStorage
        {
            get { return (Nullable<Int64>)attributes["root_storage"]; }
            private set { attributes["root_storage"] = value; }
        }

        /// <summary>
        /// Usage broken down by each top-level folder
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("usage_by_top_level_dir")]
        public object[] UsageByTopLevelDir
        {
            get { return (object[])attributes["usage_by_top_level_dir"]; }
            private set { attributes["usage_by_top_level_dir"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `date`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `date` and `usage_snapshot_id`. Valid field combinations are `[ usage_snapshot_id, date ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `date`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `date`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `date`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `date`.
        /// </summary>
        public static FilesList<UsageDailySnapshot> List(

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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
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
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            return new FilesList<UsageDailySnapshot>($"/usage_daily_snapshots", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<UsageDailySnapshot> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}