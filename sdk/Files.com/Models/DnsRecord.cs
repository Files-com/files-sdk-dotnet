using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class DnsRecord
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public DnsRecord() : this(null, null) { }

        public DnsRecord(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("domain"))
            {
                this.attributes.Add("domain", null);
            }
            if (!this.attributes.ContainsKey("rrtype"))
            {
                this.attributes.Add("rrtype", null);
            }
            if (!this.attributes.ContainsKey("value"))
            {
                this.attributes.Add("value", null);
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
        /// Unique label for DNS record; used by Zapier and other integrations.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id
        {
            get { return (string) attributes["id"]; }
        }

        /// <summary>
        /// DNS record domain name
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain
        {
            get { return (string) attributes["domain"]; }
        }

        /// <summary>
        /// DNS record type
        /// </summary>
        [JsonPropertyName("rrtype")]
        public string Rrtype
        {
            get { return (string) attributes["rrtype"]; }
        }

        /// <summary>
        /// DNS record value
        /// </summary>
        [JsonPropertyName("value")]
        public string Value
        {
            get { return (string) attributes["value"]; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<DnsRecord[]> List(
            
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

            string responseJson = await FilesClient.SendRequest($"/dns_records", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<DnsRecord[]>(responseJson);
        }

        public static async Task<DnsRecord[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

