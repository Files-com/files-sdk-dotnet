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
        public DnsRecord()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("domain", null);
            this.attributes.Add("rrtype", null);
            this.attributes.Add("value", null);
        }

        public DnsRecord(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
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
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        /// </summary>
        public static async Task<DnsRecord[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
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

