using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class RemoteBandwidthSnapshot
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public RemoteBandwidthSnapshot() : this(null, null) { }

        public RemoteBandwidthSnapshot(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("sync_bytes_received"))
            {
                this.attributes.Add("sync_bytes_received", null);
            }
            if (!this.attributes.ContainsKey("sync_bytes_sent"))
            {
                this.attributes.Add("sync_bytes_sent", null);
            }
            if (!this.attributes.ContainsKey("logged_at"))
            {
                this.attributes.Add("logged_at", null);
            }
            if (!this.attributes.ContainsKey("remote_server_id"))
            {
                this.attributes.Add("remote_server_id", null);
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
        /// Site bandwidth ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Site sync bandwidth report bytes received
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_bytes_received")]
        public double SyncBytesReceived
        {
            get { return (double) attributes["sync_bytes_received"]; }
            private set { attributes["sync_bytes_received"] = value; }
        }

        /// <summary>
        /// Site sync bandwidth report bytes sent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_bytes_sent")]
        public double SyncBytesSent
        {
            get { return (double) attributes["sync_bytes_sent"]; }
            private set { attributes["sync_bytes_sent"] = value; }
        }

        /// <summary>
        /// Time the site bandwidth report was logged
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logged_at")]
        public Nullable<DateTime> LoggedAt
        {
            get { return (Nullable<DateTime>) attributes["logged_at"]; }
            private set { attributes["logged_at"] = value; }
        }

        /// <summary>
        /// ID of related Remote Server
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("remote_server_id")]
        public Nullable<Int64> RemoteServerId
        {
            get { return (Nullable<Int64>) attributes["remote_server_id"]; }
            private set { attributes["remote_server_id"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `logged_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `logged_at`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `logged_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal to the supplied value. Valid fields are `logged_at`.
        ///   filter_like - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `logged_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `logged_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal to the supplied value. Valid fields are `logged_at`.
        /// </summary>
        public static async Task<RemoteBandwidthSnapshot[]> List(
            
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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object ))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_like") && !(parameters["filter_like"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_like must be of type object", "parameters[\"filter_like\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/remote_bandwidth_snapshots", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<RemoteBandwidthSnapshot[]>(responseJson);
        }

        public static async Task<RemoteBandwidthSnapshot[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

