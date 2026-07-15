using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class DirectConnectionInfo : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public DirectConnectionInfo() : this(null, null) { }

        public DirectConnectionInfo(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("version"))
            {
                this.attributes.Add("version", null);
            }
            if (!this.attributes.ContainsKey("server_name"))
            {
                this.attributes.Add("server_name", null);
            }
            if (!this.attributes.ContainsKey("addresses"))
            {
                this.attributes.Add("addresses", new string[0]);
            }
            if (!this.attributes.ContainsKey("direct_uri"))
            {
                this.attributes.Add("direct_uri", null);
            }
            if (!this.attributes.ContainsKey("ca_pem"))
            {
                this.attributes.Add("ca_pem", null);
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

        void IModel.SetOptions(Dictionary<string, object> options)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// Direct connection information schema version.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("version")]
        public Nullable<Int64> Version
        {
            get { return (Nullable<Int64>)attributes["version"]; }
            private set { attributes["version"] = value; }
        }

        /// <summary>
        /// TLS server name (SNI and Host header) for the Agent's direct transfer listener.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_name")]
        public string ServerName
        {
            get { return (string)attributes["server_name"]; }
            private set { attributes["server_name"] = value; }
        }

        /// <summary>
        /// Validated ip:port candidates that may be dialed over TCP+TLS for this transfer.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("addresses")]
        public string[] Addresses
        {
            get { return (string[])attributes["addresses"]; }
            private set { attributes["addresses"] = value; }
        }

        /// <summary>
        /// Signed HTTPS URI for direct Agent transfer traffic.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("direct_uri")]
        public string DirectUri
        {
            get { return (string)attributes["direct_uri"]; }
            private set { attributes["direct_uri"] = value; }
        }

        /// <summary>
        /// PEM-encoded CA certificate used to verify the Agent's direct transfer TLS certificate.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ca_pem")]
        public string CaPem
        {
            get { return (string)attributes["ca_pem"]; }
            private set { attributes["ca_pem"] = value; }
        }



    }
}