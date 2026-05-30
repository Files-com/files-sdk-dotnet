using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class EventDeliveryAttempt
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public EventDeliveryAttempt() : this(null, null) { }

        public EventDeliveryAttempt(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("event_record_id"))
            {
                this.attributes.Add("event_record_id", null);
            }
            if (!this.attributes.ContainsKey("event_subscription_id"))
            {
                this.attributes.Add("event_subscription_id", null);
            }
            if (!this.attributes.ContainsKey("event_target_id"))
            {
                this.attributes.Add("event_target_id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("attempt_number"))
            {
                this.attributes.Add("attempt_number", null);
            }
            if (!this.attributes.ContainsKey("response_code"))
            {
                this.attributes.Add("response_code", null);
            }
            if (!this.attributes.ContainsKey("error_message"))
            {
                this.attributes.Add("error_message", null);
            }
            if (!this.attributes.ContainsKey("response_body"))
            {
                this.attributes.Add("response_body", null);
            }
            if (!this.attributes.ContainsKey("latency_ms"))
            {
                this.attributes.Add("latency_ms", null);
            }
            if (!this.attributes.ContainsKey("delivered_at"))
            {
                this.attributes.Add("delivered_at", null);
            }
            if (!this.attributes.ContainsKey("last_attempted_at"))
            {
                this.attributes.Add("last_attempted_at", null);
            }
            if (!this.attributes.ContainsKey("next_attempt_at"))
            {
                this.attributes.Add("next_attempt_at", null);
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
        /// Event Delivery Attempt ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Event Record ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("event_record_id")]
        public Nullable<Int64> EventRecordId
        {
            get { return (Nullable<Int64>)attributes["event_record_id"]; }
            private set { attributes["event_record_id"] = value; }
        }

        /// <summary>
        /// Event Subscription ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("event_subscription_id")]
        public Nullable<Int64> EventSubscriptionId
        {
            get { return (Nullable<Int64>)attributes["event_subscription_id"]; }
            private set { attributes["event_subscription_id"] = value; }
        }

        /// <summary>
        /// Event Target ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("event_target_id")]
        public Nullable<Int64> EventTargetId
        {
            get { return (Nullable<Int64>)attributes["event_target_id"]; }
            private set { attributes["event_target_id"] = value; }
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
        /// Delivery status.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Number of delivery attempts made.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("attempt_number")]
        public Nullable<Int64> AttemptNumber
        {
            get { return (Nullable<Int64>)attributes["attempt_number"]; }
            private set { attributes["attempt_number"] = value; }
        }

        /// <summary>
        /// HTTP response code, if applicable.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("response_code")]
        public Nullable<Int64> ResponseCode
        {
            get { return (Nullable<Int64>)attributes["response_code"]; }
            private set { attributes["response_code"] = value; }
        }

        /// <summary>
        /// Delivery error message, if applicable.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error_message")]
        public string ErrorMessage
        {
            get { return (string)attributes["error_message"]; }
            private set { attributes["error_message"] = value; }
        }

        /// <summary>
        /// Delivery response body, if applicable.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("response_body")]
        public string ResponseBody
        {
            get { return (string)attributes["response_body"]; }
            private set { attributes["response_body"] = value; }
        }

        /// <summary>
        /// Delivery latency in milliseconds.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("latency_ms")]
        public Nullable<Int64> LatencyMs
        {
            get { return (Nullable<Int64>)attributes["latency_ms"]; }
            private set { attributes["latency_ms"] = value; }
        }

        /// <summary>
        /// Successful delivery date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("delivered_at")]
        public Nullable<DateTime> DeliveredAt
        {
            get { return (Nullable<DateTime>)attributes["delivered_at"]; }
            private set { attributes["delivered_at"] = value; }
        }

        /// <summary>
        /// Most recent attempt date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_attempted_at")]
        public Nullable<DateTime> LastAttemptedAt
        {
            get { return (Nullable<DateTime>)attributes["last_attempted_at"]; }
            private set { attributes["last_attempted_at"] = value; }
        }

        /// <summary>
        /// Next scheduled attempt date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("next_attempt_at")]
        public Nullable<DateTime> NextAttemptAt
        {
            get { return (Nullable<DateTime>)attributes["next_attempt_at"]; }
            private set { attributes["next_attempt_at"] = value; }
        }

        /// <summary>
        /// Delivery Attempt create date/time.
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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at`, `status`, `event_record_id`, `event_target_id` or `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `status`, `workspace_id`, `event_record_id` or `event_target_id`. Valid field combinations are `[ workspace_id, status ]`, `[ workspace_id, event_record_id ]` or `[ workspace_id, event_target_id ]`.
        /// </summary>
        public static FilesList<EventDeliveryAttempt> List(

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

            return new FilesList<EventDeliveryAttempt>($"/event_delivery_attempts", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<EventDeliveryAttempt> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Event Delivery Attempt ID.
        /// </summary>
        public static async Task<EventDeliveryAttempt> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/event_delivery_attempts/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<EventDeliveryAttempt>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<EventDeliveryAttempt> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

    }
}