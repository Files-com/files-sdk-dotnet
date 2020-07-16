using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class PublicIpAddress
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PublicIpAddress() : this(null, null) { }

        public PublicIpAddress(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("ip_address"))
            {
                this.attributes.Add("ip_address", null);
            }
            if (!this.attributes.ContainsKey("server_name"))
            {
                this.attributes.Add("server_name", null);
            }
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
        /// The public IP address.
        /// </summary>
        [JsonPropertyName("ip_address")]
        public string IpAddress
        {
            get { return (string) attributes["ip_address"]; }
        }

        /// <summary>
        /// The name of the frontend server.
        /// </summary>
        [JsonPropertyName("server_name")]
        public string ServerName
        {
            get { return (string) attributes["server_name"]; }
        }



    }
}

