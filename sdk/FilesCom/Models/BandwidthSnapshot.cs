using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class BandwidthSnapshot
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public BandwidthSnapshot() : this(null, null) { }

        public BandwidthSnapshot(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("bytes_received"))
            {
                this.attributes.Add("bytes_received", null);
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
            if (!this.attributes.ContainsKey("requests_get"))
            {
                this.attributes.Add("requests_get", null);
            }
            if (!this.attributes.ContainsKey("requests_put"))
            {
                this.attributes.Add("requests_put", null);
            }
            if (!this.attributes.ContainsKey("requests_other"))
            {
                this.attributes.Add("requests_other", null);
            }
            if (!this.attributes.ContainsKey("logged_at"))
            {
                this.attributes.Add("logged_at", null);
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
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Site bandwidth report bytes received
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes_received")]
        public double BytesReceived
        {
            get { return (double)attributes["bytes_received"]; }
            private set { attributes["bytes_received"] = value; }
        }

        /// <summary>
        /// Site bandwidth report bytes sent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes_sent")]
        public double BytesSent
        {
            get { return (double)attributes["bytes_sent"]; }
            private set { attributes["bytes_sent"] = value; }
        }

        /// <summary>
        /// Site sync bandwidth report bytes received
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_bytes_received")]
        public double SyncBytesReceived
        {
            get { return (double)attributes["sync_bytes_received"]; }
            private set { attributes["sync_bytes_received"] = value; }
        }

        /// <summary>
        /// Site sync bandwidth report bytes sent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sync_bytes_sent")]
        public double SyncBytesSent
        {
            get { return (double)attributes["sync_bytes_sent"]; }
            private set { attributes["sync_bytes_sent"] = value; }
        }

        /// <summary>
        /// Site bandwidth report get requests
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("requests_get")]
        public double RequestsGet
        {
            get { return (double)attributes["requests_get"]; }
            private set { attributes["requests_get"] = value; }
        }

        /// <summary>
        /// Site bandwidth report put requests
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("requests_put")]
        public double RequestsPut
        {
            get { return (double)attributes["requests_put"]; }
            private set { attributes["requests_put"] = value; }
        }

        /// <summary>
        /// Site bandwidth report other requests
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("requests_other")]
        public double RequestsOther
        {
            get { return (double)attributes["requests_other"]; }
            private set { attributes["requests_other"] = value; }
        }

        /// <summary>
        /// Time the site bandwidth report was logged
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logged_at")]
        public Nullable<DateTime> LoggedAt
        {
            get { return (Nullable<DateTime>)attributes["logged_at"]; }
            private set { attributes["logged_at"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `logged_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `logged_at`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `logged_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `logged_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `logged_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `logged_at`.
        /// </summary>
        public static FilesList<BandwidthSnapshot> List(

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

            return new FilesList<BandwidthSnapshot>($"/bandwidth_snapshots", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<BandwidthSnapshot> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}