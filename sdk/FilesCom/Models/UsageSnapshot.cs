using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class UsageSnapshot
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public UsageSnapshot() : this(null, null) { }

        public UsageSnapshot(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("start_at"))
            {
                this.attributes.Add("start_at", null);
            }
            if (!this.attributes.ContainsKey("end_at"))
            {
                this.attributes.Add("end_at", null);
            }
            if (!this.attributes.ContainsKey("high_water_user_count"))
            {
                this.attributes.Add("high_water_user_count", null);
            }
            if (!this.attributes.ContainsKey("current_storage"))
            {
                this.attributes.Add("current_storage", null);
            }
            if (!this.attributes.ContainsKey("high_water_storage"))
            {
                this.attributes.Add("high_water_storage", null);
            }
            if (!this.attributes.ContainsKey("root_storage"))
            {
                this.attributes.Add("root_storage", null);
            }
            if (!this.attributes.ContainsKey("deleted_files_counted_in_minimum"))
            {
                this.attributes.Add("deleted_files_counted_in_minimum", null);
            }
            if (!this.attributes.ContainsKey("deleted_files_storage"))
            {
                this.attributes.Add("deleted_files_storage", null);
            }
            if (!this.attributes.ContainsKey("total_billable_usage"))
            {
                this.attributes.Add("total_billable_usage", null);
            }
            if (!this.attributes.ContainsKey("total_billable_transfer_usage"))
            {
                this.attributes.Add("total_billable_transfer_usage", null);
            }
            if (!this.attributes.ContainsKey("bytes_sent"))
            {
                this.attributes.Add("bytes_sent", null);
            }
            if (!this.attributes.ContainsKey("sync_bytes_received"))
            {
                this.attributes.Add("sync_bytes_received", null);
            }
            if (!this.attributes.ContainsKey("sync_bytes_sent"))
            {
                this.attributes.Add("sync_bytes_sent", null);
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
        /// Usage snapshot ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Usage snapshot start date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("start_at")]
        public Nullable<DateTime> StartAt
        {
            get { return (Nullable<DateTime>)attributes["start_at"]; }
            private set { attributes["start_at"] = value; }
        }

        /// <summary>
        /// Usage snapshot end date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("end_at")]
        public Nullable<DateTime> EndAt
        {
            get { return (Nullable<DateTime>)attributes["end_at"]; }
            private set { attributes["end_at"] = value; }
        }

        /// <summary>
        /// Highest user count number in time period
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("high_water_user_count")]
        public Nullable<Int64> HighWaterUserCount
        {
            get { return (Nullable<Int64>)attributes["high_water_user_count"]; }
            private set { attributes["high_water_user_count"] = value; }
        }

        /// <summary>
        /// Current total Storage Usage GB as of end date (not necessarily high water mark, which is used for billing)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("current_storage")]
        public double CurrentStorage
        {
            get { return (double)attributes["current_storage"]; }
            private set { attributes["current_storage"] = value; }
        }

        /// <summary>
        /// Highest Storage Usage GB recorded in time period (used for billing)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("high_water_storage")]
        public double HighWaterStorage
        {
            get { return (double)attributes["high_water_storage"]; }
            private set { attributes["high_water_storage"] = value; }
        }

        /// <summary>
        /// Storage Usage for root folder as of end date (not necessarily high water mark, which is used for billing)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("root_storage")]
        public double RootStorage
        {
            get { return (double)attributes["root_storage"]; }
            private set { attributes["root_storage"] = value; }
        }

        /// <summary>
        /// Storage Usage for files that are deleted but uploaded within last 30 days as of end date (not necessarily high water mark, which is used for billing)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("deleted_files_counted_in_minimum")]
        public double DeletedFilesCountedInMinimum
        {
            get { return (double)attributes["deleted_files_counted_in_minimum"]; }
            private set { attributes["deleted_files_counted_in_minimum"] = value; }
        }

        /// <summary>
        /// Storage Usage for files that are deleted but retained as backups as of end date (not necessarily high water mark, which is used for billing)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("deleted_files_storage")]
        public double DeletedFilesStorage
        {
            get { return (double)attributes["deleted_files_storage"]; }
            private set { attributes["deleted_files_storage"] = value; }
        }

        /// <summary>
        /// Storage + Transfer Usage - Total Billable amount
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("total_billable_usage")]
        public double TotalBillableUsage
        {
            get { return (double)attributes["total_billable_usage"]; }
            private set { attributes["total_billable_usage"] = value; }
        }

        /// <summary>
        /// Transfer usage for period - Total Billable amount
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("total_billable_transfer_usage")]
        public double TotalBillableTransferUsage
        {
            get { return (double)attributes["total_billable_transfer_usage"]; }
            private set { attributes["total_billable_transfer_usage"] = value; }
        }

        /// <summary>
        /// Transfer Usage for period - Outbound GB from Files Native Storage
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes_sent")]
        public double BytesSent
        {
            get { return (double)attributes["bytes_sent"]; }
            private set { attributes["bytes_sent"] = value; }
        }

        /// <summary>
        /// Transfer Usage for period - Inbound GB to Remote Servers (Sync/Mount)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_bytes_received")]
        public double SyncBytesReceived
        {
            get { return (double)attributes["sync_bytes_received"]; }
            private set { attributes["sync_bytes_received"] = value; }
        }

        /// <summary>
        /// Transfer Usage for period - Outbound GB from Remote Servers (Sync/Mount)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_bytes_sent")]
        public double SyncBytesSent
        {
            get { return (double)attributes["sync_bytes_sent"]; }
            private set { attributes["sync_bytes_sent"] = value; }
        }

        /// <summary>
        /// Storage Usage - map of root folders to their usage as of end date (not necessarily high water mark, which is used for billing)
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
        /// </summary>
        public static FilesList<UsageSnapshot> List(

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

            return new FilesList<UsageSnapshot>($"/usage_snapshots", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<UsageSnapshot> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}