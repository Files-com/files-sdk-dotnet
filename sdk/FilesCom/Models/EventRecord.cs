using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class EventRecord
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public EventRecord() : this(null, null) { }

        public EventRecord(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("event_uuid"))
            {
                this.attributes.Add("event_uuid", null);
            }
            if (!this.attributes.ContainsKey("event_type"))
            {
                this.attributes.Add("event_type", null);
            }
            if (!this.attributes.ContainsKey("severity"))
            {
                this.attributes.Add("severity", null);
            }
            if (!this.attributes.ContainsKey("source_type"))
            {
                this.attributes.Add("source_type", null);
            }
            if (!this.attributes.ContainsKey("source_id"))
            {
                this.attributes.Add("source_id", null);
            }
            if (!this.attributes.ContainsKey("occurred_at"))
            {
                this.attributes.Add("occurred_at", null);
            }
            if (!this.attributes.ContainsKey("human_title"))
            {
                this.attributes.Add("human_title", null);
            }
            if (!this.attributes.ContainsKey("human_summary"))
            {
                this.attributes.Add("human_summary", null);
            }
            if (!this.attributes.ContainsKey("human_fields"))
            {
                this.attributes.Add("human_fields", new object[0]);
            }
            if (!this.attributes.ContainsKey("actor"))
            {
                this.attributes.Add("actor", null);
            }
            if (!this.attributes.ContainsKey("resources"))
            {
                this.attributes.Add("resources", new object[0]);
            }
            if (!this.attributes.ContainsKey("payload"))
            {
                this.attributes.Add("payload", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
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
        /// Event Record ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Workspace ID. 0 means the default workspace or site-wide.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            private set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// Stable event UUID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("event_uuid")]
        public string EventUuid
        {
            get { return (string)attributes["event_uuid"]; }
            private set { attributes["event_uuid"] = value; }
        }

        /// <summary>
        /// Versioned event type string.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("event_type")]
        public string EventType
        {
            get { return (string)attributes["event_type"]; }
            private set { attributes["event_type"] = value; }
        }

        /// <summary>
        /// Event severity.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("severity")]
        public string Severity
        {
            get { return (string)attributes["severity"]; }
            private set { attributes["severity"] = value; }
        }

        /// <summary>
        /// Source record type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("source_type")]
        public string SourceType
        {
            get { return (string)attributes["source_type"]; }
            private set { attributes["source_type"] = value; }
        }

        /// <summary>
        /// Source record ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("source_id")]
        public Nullable<Int64> SourceId
        {
            get { return (Nullable<Int64>)attributes["source_id"]; }
            private set { attributes["source_id"] = value; }
        }

        /// <summary>
        /// Event occurrence date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("occurred_at")]
        public Nullable<DateTime> OccurredAt
        {
            get { return (Nullable<DateTime>)attributes["occurred_at"]; }
            private set { attributes["occurred_at"] = value; }
        }

        /// <summary>
        /// Human-readable event title.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("human_title")]
        public string HumanTitle
        {
            get { return (string)attributes["human_title"]; }
            private set { attributes["human_title"] = value; }
        }

        /// <summary>
        /// Human-readable event summary.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("human_summary")]
        public string HumanSummary
        {
            get { return (string)attributes["human_summary"]; }
            private set { attributes["human_summary"] = value; }
        }

        /// <summary>
        /// Human-readable event detail fields.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("human_fields")]
        public object[] HumanFields
        {
            get { return (object[])attributes["human_fields"]; }
            private set { attributes["human_fields"] = value; }
        }

        /// <summary>
        /// Actor associated with the event.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("actor")]
        public object Actor
        {
            get { return (object)attributes["actor"]; }
            private set { attributes["actor"] = value; }
        }

        /// <summary>
        /// Resources associated with the event.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("resources")]
        public object[] Resources
        {
            get { return (object[])attributes["resources"]; }
            private set { attributes["resources"] = value; }
        }

        /// <summary>
        /// Event payload.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("payload")]
        public object Payload
        {
            get { return (object)attributes["payload"]; }
            private set { attributes["payload"] = value; }
        }

        /// <summary>
        /// Event Record create date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `event_type`, `created_at` or `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `event_type` or `workspace_id`. Valid field combinations are `[ event_type, created_at ]`, `[ workspace_id, created_at ]`, `[ workspace_id, event_type ]` or `[ workspace_id, event_type, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `event_type`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        public static FilesList<EventRecord> List(

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
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            return new FilesList<EventRecord>($"/event_records", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<EventRecord> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Event Record ID.
        /// </summary>
        public static async Task<EventRecord> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/event_records/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<EventRecord>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<EventRecord> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

    }
}