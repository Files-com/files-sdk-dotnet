using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class SyncJob
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public SyncJob() : this(null, null) { }

        public SyncJob(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("queued_at"))
            {
                this.attributes.Add("queued_at", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("regional_worker_status"))
            {
                this.attributes.Add("regional_worker_status", null);
            }
            if (!this.attributes.ContainsKey("uuid"))
            {
                this.attributes.Add("uuid", null);
            }
            if (!this.attributes.ContainsKey("folder_behavior_id"))
            {
                this.attributes.Add("folder_behavior_id", null);
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
        /// Job enqueued at
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("queued_at")]
        public Nullable<DateTime> QueuedAt
        {
            get { return (Nullable<DateTime>) attributes["queued_at"]; }
            private set { attributes["queued_at"] = value; }
        }

        /// <summary>
        /// Job updated at
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>) attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }

        /// <summary>
        /// Status of the job
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string) attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Most recent reported status of sync worker
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("regional_worker_status")]
        public string RegionalWorkerStatus
        {
            get { return (string) attributes["regional_worker_status"]; }
            private set { attributes["regional_worker_status"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("uuid")]
        public string Uuid
        {
            get { return (string) attributes["uuid"]; }
            private set { attributes["uuid"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("folder_behavior_id")]
        public Nullable<Int64> FolderBehaviorId
        {
            get { return (Nullable<Int64>) attributes["folder_behavior_id"]; }
            private set { attributes["folder_behavior_id"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<SyncJob[]> List(
            
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

            string responseJson = await FilesClient.SendRequest($"/sync_jobs", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<SyncJob[]>(responseJson);
        }

        public static async Task<SyncJob[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

