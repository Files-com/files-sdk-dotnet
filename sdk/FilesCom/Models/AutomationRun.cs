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

            if (!this.attributes.ContainsKey("automation_id"))
            {
                this.attributes.Add("automation_id", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
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
        /// ID of the associated Automation.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("automation_id")]
        public Nullable<Int64> AutomationId
        {
            get { return (Nullable<Int64>) attributes["automation_id"]; }
            private set { attributes["automation_id"] = value; }
        }

        /// <summary>
        /// The success status of the AutomationRun. One of `running`, `success`, `partial_failure`, or `failure`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string) attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Link to status messages log file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status_messages_url")]
        public string StatusMessagesUrl
        {
            get { return (string) attributes["status_messages_url"]; }
            private set { attributes["status_messages_url"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   automation_id (required) - int64 - ID of the associated Automation.
        /// </summary>
        public static async Task<AutomationRun[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("automation_id") && !(parameters["automation_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: automation_id must be of type Nullable<Int64>", "parameters[\"automation_id\"]");
            }
            if (!parameters.ContainsKey("automation_id") || parameters["automation_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: automation_id", "parameters[\"automation_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/automation_runs", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<AutomationRun[]>(responseJson);
        }

        public static async Task<AutomationRun[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

