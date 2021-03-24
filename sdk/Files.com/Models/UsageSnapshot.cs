using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
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
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
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
            if (!this.attributes.ContainsKey("total_downloads"))
            {
                this.attributes.Add("total_downloads", null);
            }
            if (!this.attributes.ContainsKey("total_uploads"))
            {
                this.attributes.Add("total_uploads", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
            }
            if (!this.attributes.ContainsKey("usage_by_top_level_dir"))
            {
                this.attributes.Add("usage_by_top_level_dir", null);
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
        /// Site usage ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// Site usage report start date/time
        /// </summary>
        [JsonPropertyName("start_at")]
        public Nullable<DateTime> StartAt
        {
            get { return (Nullable<DateTime>) attributes["start_at"]; }
        }

        /// <summary>
        /// Site usage report end date/time
        /// </summary>
        [JsonPropertyName("end_at")]
        public Nullable<DateTime> EndAt
        {
            get { return (Nullable<DateTime>) attributes["end_at"]; }
        }

        /// <summary>
        /// Site usage report created at date/time
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
        }

        /// <summary>
        /// Site usage report highest usage in time period
        /// </summary>
        [JsonPropertyName("high_water_user_count")]
        public double HighWaterUserCount
        {
            get { return (double) attributes["high_water_user_count"]; }
        }

        /// <summary>
        /// Current site usage as of report
        /// </summary>
        [JsonPropertyName("current_storage")]
        public double CurrentStorage
        {
            get { return (double) attributes["current_storage"]; }
        }

        /// <summary>
        /// Site usage report highest usage in time period
        /// </summary>
        [JsonPropertyName("high_water_storage")]
        public double HighWaterStorage
        {
            get { return (double) attributes["high_water_storage"]; }
        }

        /// <summary>
        /// Number of downloads in report time period
        /// </summary>
        [JsonPropertyName("total_downloads")]
        public Nullable<Int64> TotalDownloads
        {
            get { return (Nullable<Int64>) attributes["total_downloads"]; }
        }

        /// <summary>
        /// Number of uploads in time period
        /// </summary>
        [JsonPropertyName("total_uploads")]
        public Nullable<Int64> TotalUploads
        {
            get { return (Nullable<Int64>) attributes["total_uploads"]; }
        }

        /// <summary>
        /// The last time this site usage report was updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>) attributes["updated_at"]; }
        }

        /// <summary>
        /// A map of root folders to their total usage
        /// </summary>
        [JsonPropertyName("usage_by_top_level_dir")]
        public object UsageByTopLevelDir
        {
            get { return (object) attributes["usage_by_top_level_dir"]; }
        }

        /// <summary>
        /// Usage for root folder
        /// </summary>
        [JsonPropertyName("root_storage")]
        public double RootStorage
        {
            get { return (double) attributes["root_storage"]; }
        }

        /// <summary>
        /// Usage for files that are deleted but uploaded within last 30 days
        /// </summary>
        [JsonPropertyName("deleted_files_counted_in_minimum")]
        public double DeletedFilesCountedInMinimum
        {
            get { return (double) attributes["deleted_files_counted_in_minimum"]; }
        }

        /// <summary>
        /// Usage for files that are deleted but retained as backups
        /// </summary>
        [JsonPropertyName("deleted_files_storage")]
        public double DeletedFilesStorage
        {
            get { return (double) attributes["deleted_files_storage"]; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<UsageSnapshot[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/usage_snapshots", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<UsageSnapshot[]>(responseJson);
        }

        public static async Task<UsageSnapshot[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

