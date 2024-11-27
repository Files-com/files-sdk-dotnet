using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class AutomationRun
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public AutomationRun() : this(null, null) { }

        public AutomationRun(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("automation_id"))
            {
                this.attributes.Add("automation_id", null);
            }
            if (!this.attributes.ContainsKey("completed_at"))
            {
                this.attributes.Add("completed_at", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("runtime"))
            {
                this.attributes.Add("runtime", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("successful_operations"))
            {
                this.attributes.Add("successful_operations", null);
            }
            if (!this.attributes.ContainsKey("failed_operations"))
            {
                this.attributes.Add("failed_operations", null);
            }
            if (!this.attributes.ContainsKey("status_messages_url"))
            {
                this.attributes.Add("status_messages_url", null);
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
        /// ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// ID of the associated Automation.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("automation_id")]
        public Nullable<Int64> AutomationId
        {
            get { return (Nullable<Int64>)attributes["automation_id"]; }
            private set { attributes["automation_id"] = value; }
        }

        /// <summary>
        /// Automation run completion/failure date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed_at")]
        public Nullable<DateTime> CompletedAt
        {
            get { return (Nullable<DateTime>)attributes["completed_at"]; }
            private set { attributes["completed_at"] = value; }
        }

        /// <summary>
        /// Automation run start date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Automation run runtime.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("runtime")]
        public double Runtime
        {
            get { return (double)attributes["runtime"]; }
            private set { attributes["runtime"] = value; }
        }

        /// <summary>
        /// The success status of the AutomationRun. One of `running`, `success`, `partial_failure`, or `failure`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Count of successful operations.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("successful_operations")]
        public Nullable<Int64> SuccessfulOperations
        {
            get { return (Nullable<Int64>)attributes["successful_operations"]; }
            private set { attributes["successful_operations"] = value; }
        }

        /// <summary>
        /// Count of failed operations.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failed_operations")]
        public Nullable<Int64> FailedOperations
        {
            get { return (Nullable<Int64>)attributes["failed_operations"]; }
            private set { attributes["failed_operations"] = value; }
        }

        /// <summary>
        /// Link to status messages log file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status_messages_url")]
        public string StatusMessagesUrl
        {
            get { return (string)attributes["status_messages_url"]; }
            private set { attributes["status_messages_url"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `automation_id`, `created_at` or `status`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `status` and `automation_id`. Valid field combinations are `[ automation_id, status ]`.
        ///   automation_id (required) - int64 - ID of the associated Automation.
        /// </summary>
        public static FilesList<AutomationRun> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("automation_id") || parameters["automation_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: automation_id", "parameters[\"automation_id\"]");
            }
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
            if (parameters.ContainsKey("automation_id") && !(parameters["automation_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: automation_id must be of type Nullable<Int64>", "parameters[\"automation_id\"]");
            }

            return new FilesList<AutomationRun>($"/automation_runs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<AutomationRun> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Automation Run ID.
        /// </summary>
        public static async Task<AutomationRun> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/automation_runs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<AutomationRun>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<AutomationRun> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

    }
}