using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ExternalEvent
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ExternalEvent() : this(null, null) { }

        public ExternalEvent(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("event_type"))
            {
                this.attributes.Add("event_type", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("body"))
            {
                this.attributes.Add("body", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("body_url"))
            {
                this.attributes.Add("body_url", null);
            }
            if (!this.attributes.ContainsKey("folder_behavior_id"))
            {
                this.attributes.Add("folder_behavior_id", null);
            }
            if (!this.attributes.ContainsKey("siem_http_destination_id"))
            {
                this.attributes.Add("siem_http_destination_id", null);
            }
            if (!this.attributes.ContainsKey("successful_files"))
            {
                this.attributes.Add("successful_files", null);
            }
            if (!this.attributes.ContainsKey("errored_files"))
            {
                this.attributes.Add("errored_files", null);
            }
            if (!this.attributes.ContainsKey("bytes_synced"))
            {
                this.attributes.Add("bytes_synced", null);
            }
            if (!this.attributes.ContainsKey("compared_files"))
            {
                this.attributes.Add("compared_files", null);
            }
            if (!this.attributes.ContainsKey("compared_folders"))
            {
                this.attributes.Add("compared_folders", null);
            }
            if (!this.attributes.ContainsKey("remote_server_type"))
            {
                this.attributes.Add("remote_server_type", null);
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
        /// Event ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Type of event being recorded.
        /// </summary>
        [JsonPropertyName("event_type")]
        public string EventType
        {
            get { return (string)attributes["event_type"]; }
            set { attributes["event_type"] = value; }
        }

        /// <summary>
        /// Status of event.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; }
        }

        /// <summary>
        /// Event body
        /// </summary>
        [JsonPropertyName("body")]
        public string Body
        {
            get { return (string)attributes["body"]; }
            set { attributes["body"] = value; }
        }

        /// <summary>
        /// External event create date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Link to log file.
        /// </summary>
        [JsonPropertyName("body_url")]
        public string BodyUrl
        {
            get { return (string)attributes["body_url"]; }
            set { attributes["body_url"] = value; }
        }

        /// <summary>
        /// Folder Behavior ID
        /// </summary>
        [JsonPropertyName("folder_behavior_id")]
        public Nullable<Int64> FolderBehaviorId
        {
            get { return (Nullable<Int64>)attributes["folder_behavior_id"]; }
            set { attributes["folder_behavior_id"] = value; }
        }

        /// <summary>
        /// SIEM HTTP Destination ID.
        /// </summary>
        [JsonPropertyName("siem_http_destination_id")]
        public Nullable<Int64> SiemHttpDestinationId
        {
            get { return (Nullable<Int64>)attributes["siem_http_destination_id"]; }
            set { attributes["siem_http_destination_id"] = value; }
        }

        /// <summary>
        /// For sync events, the number of files handled successfully.
        /// </summary>
        [JsonPropertyName("successful_files")]
        public Nullable<Int64> SuccessfulFiles
        {
            get { return (Nullable<Int64>)attributes["successful_files"]; }
            set { attributes["successful_files"] = value; }
        }

        /// <summary>
        /// For sync events, the number of files that encountered errors.
        /// </summary>
        [JsonPropertyName("errored_files")]
        public Nullable<Int64> ErroredFiles
        {
            get { return (Nullable<Int64>)attributes["errored_files"]; }
            set { attributes["errored_files"] = value; }
        }

        /// <summary>
        /// For sync events, the total number of bytes synced.
        /// </summary>
        [JsonPropertyName("bytes_synced")]
        public Nullable<Int64> BytesSynced
        {
            get { return (Nullable<Int64>)attributes["bytes_synced"]; }
            set { attributes["bytes_synced"] = value; }
        }

        /// <summary>
        /// For sync events, the number of files considered for the sync.
        /// </summary>
        [JsonPropertyName("compared_files")]
        public Nullable<Int64> ComparedFiles
        {
            get { return (Nullable<Int64>)attributes["compared_files"]; }
            set { attributes["compared_files"] = value; }
        }

        /// <summary>
        /// For sync events, the number of folders listed and considered for the sync.
        /// </summary>
        [JsonPropertyName("compared_folders")]
        public Nullable<Int64> ComparedFolders
        {
            get { return (Nullable<Int64>)attributes["compared_folders"]; }
            set { attributes["compared_folders"] = value; }
        }

        /// <summary>
        /// Associated Remote Server type, if any
        /// </summary>
        [JsonPropertyName("remote_server_type")]
        public string RemoteServerType
        {
            get { return (string)attributes["remote_server_type"]; }
            set { attributes["remote_server_type"] = value; }
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The ExternalEvent object doesn't support updates.");
            }
            else
            {
                var newObj = await ExternalEvent.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `siem_http_destination_id`, `created_at`, `event_type`, `status` or `folder_behavior_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `event_type`, `remote_server_type`, `status`, `folder_behavior_id` or `siem_http_destination_id`. Valid field combinations are `[ event_type, created_at ]`, `[ remote_server_type, created_at ]`, `[ status, created_at ]`, `[ folder_behavior_id, created_at ]`, `[ event_type, status ]`, `[ remote_server_type, status ]`, `[ folder_behavior_id, status ]`, `[ event_type, status, created_at ]`, `[ remote_server_type, status, created_at ]` or `[ folder_behavior_id, status, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        public static FilesList<ExternalEvent> List(

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

            return new FilesList<ExternalEvent>($"/external_events", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<ExternalEvent> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - External Event ID.
        /// </summary>
        public static async Task<ExternalEvent> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/external_events/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ExternalEvent>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<ExternalEvent> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   status (required) - string - Status of event.
        ///   body (required) - string - Event body
        /// </summary>
        public static async Task<ExternalEvent> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("status") || parameters["status"] == null)
            {
                throw new ArgumentNullException("Parameter missing: status", "parameters[\"status\"]");
            }
            if (!parameters.ContainsKey("body") || parameters["body"] == null)
            {
                throw new ArgumentNullException("Parameter missing: body", "parameters[\"body\"]");
            }
            if (parameters.ContainsKey("status") && !(parameters["status"] is string))
            {
                throw new ArgumentException("Bad parameter: status must be of type string", "parameters[\"status\"]");
            }
            if (parameters.ContainsKey("body") && !(parameters["body"] is string))
            {
                throw new ArgumentException("Bad parameter: body must be of type string", "parameters[\"body\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/external_events", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ExternalEvent>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}