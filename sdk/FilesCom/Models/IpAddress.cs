using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class IpAddress
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public IpAddress() : this(null, null) { }

        public IpAddress(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("associated_with"))
            {
                this.attributes.Add("associated_with", null);
            }
            if (!this.attributes.ContainsKey("group_id"))
            {
                this.attributes.Add("group_id", null);
            }
            if (!this.attributes.ContainsKey("ip_addresses"))
            {
                this.attributes.Add("ip_addresses", new string[0]);
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
        /// Unique label for list; used by Zapier and other integrations.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id
        {
            get { return (string) attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// The object that this public IP address list is associated with.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("associated_with")]
        public string AssociatedWith
        {
            get { return (string) attributes["associated_with"]; }
            private set { attributes["associated_with"] = value; }
        }

        /// <summary>
        /// Group ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>) attributes["group_id"]; }
            private set { attributes["group_id"] = value; }
        }

        /// <summary>
        /// A list of IP addresses.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ip_addresses")]
        public string[] IpAddresses
        {
            get { return (string[]) attributes["ip_addresses"]; }
            private set { attributes["ip_addresses"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<IpAddress[]> List(
            
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

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<PublicIpAddress[]> GetReserved(
            
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

            string responseJson = await FilesClient.SendRequest($"/ip_addresses/reserved", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<PublicIpAddress[]>(responseJson);
        }


    }
}

