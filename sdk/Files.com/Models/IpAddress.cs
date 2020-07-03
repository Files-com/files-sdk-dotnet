using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class IpAddress
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public IpAddress()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("associated_with", null);
            this.attributes.Add("group_id", null);
            this.attributes.Add("ip_addresses", new object[0]);
        }

        public IpAddress(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Unique label for list; used by Zapier and other integrations.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id
        {
            get { return (string) attributes["id"]; }
        }

        /// <summary>
        /// The object that this public IP address list is associated with.
        /// </summary>
        [JsonPropertyName("associated_with")]
        public string AssociatedWith
        {
            get { return (string) attributes["associated_with"]; }
        }

        /// <summary>
        /// Group ID
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>) attributes["group_id"]; }
        }

        /// <summary>
        /// A list of IP addresses.
        /// </summary>
        [JsonPropertyName("ip_addresses")]
        public object[] IpAddresses
        {
            get { return (object[]) attributes["ip_addresses"]; }
        }



        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        /// </summary>
        public static async Task<IpAddress[]> List(
            
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

            string responseJson = await FilesClient.SendRequest($"/ip_addresses", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<IpAddress[]>(responseJson);
        }

        public static async Task<IpAddress[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

