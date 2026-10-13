using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// A DirectConnectionInfo object of the Files.com API.
    /// </summary>
    public class DirectConnectionInfo : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a DirectConnectionInfo with no attributes set and no options.
        /// </summary>
        public DirectConnectionInfo() : this(null, null) { }

        /// <summary>
        /// Makes a DirectConnectionInfo with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public DirectConnectionInfo(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => null;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[0]; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
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