using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class UsageDailySnapshot
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public UsageDailySnapshot()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("date", null);
            this.attributes.Add("current_storage", null);
            this.attributes.Add("usage_by_top_level_dir", new object[0]);
        }

        public UsageDailySnapshot(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// ID of the usage record
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// The date of this usage record
        /// </summary>
        [JsonPropertyName("date")]
        public Nullable<DateTime> Date
        {
            get { return (Nullable<DateTime>) attributes["date"]; }
        }

        /// <summary>
        /// The quantity of storage held for this site
        /// </summary>
        [JsonPropertyName("current_storage")]
        public Nullable<Int64> CurrentStorage
        {
            get { return (Nullable<Int64>) attributes["current_storage"]; }
        }

        /// <summary>
        /// Usage broken down by each top-level folder
        /// </summary>
        [JsonPropertyName("usage_by_top_level_dir")]
        public object[] UsageByTopLevelDir
        {
            get { return (object[]) attributes["usage_by_top_level_dir"]; }
        }



        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        /// </summary>
        public static async Task<UsageDailySnapshot[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/usage_daily_snapshots", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<UsageDailySnapshot[]>(responseJson);
        }

        public static async Task<UsageDailySnapshot[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

