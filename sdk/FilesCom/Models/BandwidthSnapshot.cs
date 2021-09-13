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
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
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
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            
            get;
            set;
        }

        /// <summary>
        /// Site bandwidth report bytes received
        /// </summary>
        [JsonPropertyName("bytes_received")]
        public double BytesReceived
        {
            
            get;
            set;
        }

        /// <summary>
        /// Site bandwidth report bytes sent
        /// </summary>
        [JsonPropertyName("bytes_sent")]
        public double BytesSent
        {
            
            get;
            set;
        }

        /// <summary>
        /// Site sync bandwidth report bytes received
        /// </summary>
        [JsonPropertyName("sync_bytes_received")]
        public double SyncBytesReceived
        {
            
            get;
            set;
        }

        /// <summary>
        /// Site sync bandwidth report bytes sent
        /// </summary>
        [JsonPropertyName("sync_bytes_sent")]
        public double SyncBytesSent
        {
            
            get;
            set;
        }

        /// <summary>
        /// Site bandwidth report get requests
        /// </summary>
        [JsonPropertyName("requests_get")]
        public double RequestsGet
        {
            
            get;
            set;
        }

        /// <summary>
        /// Site bandwidth report put requests
        /// </summary>
        [JsonPropertyName("requests_put")]
        public double RequestsPut
        {
            
            get;
            set;
        }

        /// <summary>
        /// Site bandwidth report other requests
        /// </summary>
        [JsonPropertyName("requests_other")]
        public double RequestsOther
        {
            
            get;
            set;
        }

        /// <summary>
        /// Time the site bandwidth report was logged
        /// </summary>
        [JsonPropertyName("logged_at")]
        public Nullable<DateTime> LoggedAt
        {
            
            get;
            set;
        }

        /// <summary>
        /// Site bandwidth report created at date/time
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            
            get;
            set;
        }

        /// <summary>
        /// The last time this site bandwidth report was updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            
            get;
            set;
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `logged_at`.
        ///   filter - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `logged_at`.
        ///   filter_gt - object - If set, return records where the specifiied field is greater than the supplied value. Valid fields are `logged_at`.
        ///   filter_gteq - object - If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `logged_at`.
        ///   filter_like - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `logged_at`.
        ///   filter_lt - object - If set, return records where the specifiied field is less than the supplied value. Valid fields are `logged_at`.
        ///   filter_lteq - object - If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `logged_at`.
        /// </summary>
        public static async Task<BandwidthSnapshot[]> List(
            
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

            string responseJson = await FilesClient.SendRequest($"/bandwidth_snapshots", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<BandwidthSnapshot[]>(responseJson);
        }

        public static async Task<BandwidthSnapshot[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

