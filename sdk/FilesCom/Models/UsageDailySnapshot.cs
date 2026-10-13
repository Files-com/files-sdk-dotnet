using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// UsageDailySnapshotEntity model
    /// </summary>
    public class UsageDailySnapshot : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a UsageDailySnapshot with no attributes set and no options.
        /// </summary>
        public UsageDailySnapshot() : this(null, null) { }

        /// <summary>
        /// Makes a UsageDailySnapshot with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public UsageDailySnapshot(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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
            if (!this.attributes.ContainsKey("transformation_credits_usage"))
            {
                this.attributes.Add("transformation_credits_usage", null);
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
                this.attributes.Add("usage_by_top_level_dir", new UsageByTopLevelDir[0]);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => null;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[] { UsageByTopLevelDir }; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
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
        /// Transformation and AI credits used on this day. Note: only updated for days before the current day.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("transformation_credits_usage")]
        public Nullable<Int64> TransformationCreditsUsage
        {
            get { return (Nullable<Int64>)attributes["transformation_credits_usage"]; }
            private set { attributes["transformation_credits_usage"] = value; }
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
        public UsageByTopLevelDir[] UsageByTopLevelDir
        {
            get { return (UsageByTopLevelDir[])attributes["usage_by_top_level_dir"]; }
            private set { attributes["usage_by_top_level_dir"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `date`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `date` and `usage_snapshot_id`. Valid field combinations are `[ usage_snapshot_id, date ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `date`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `date`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `date`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `date`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<UsageDailySnapshot> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `date`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `date` and `usage_snapshot_id`. Valid field combinations are `[ usage_snapshot_id, date ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `date`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `date`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `date`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `date`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<UsageDailySnapshot> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<UsageDailySnapshot> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }
            if (parameters.TryGetValue("filter_gt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gt is null, which cannot be sent" : "Bad parameter: filter_gt must be of type object", "filter_gt");
            }
            if (parameters.TryGetValue("filter_gteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gteq is null, which cannot be sent" : "Bad parameter: filter_gteq must be of type object", "filter_gteq");
            }
            if (parameters.TryGetValue("filter_lt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lt is null, which cannot be sent" : "Bad parameter: filter_lt must be of type object", "filter_lt");
            }
            if (parameters.TryGetValue("filter_lteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lteq is null, which cannot be sent" : "Bad parameter: filter_lteq must be of type object", "filter_lteq");
            }

            return new FilesList<UsageDailySnapshot>(client, $"/usage_daily_snapshots", System.Net.Http.HttpMethod.Get, parameters, options);
        }

    }
}