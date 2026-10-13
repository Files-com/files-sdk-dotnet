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
    /// PublicIpAddressEntity model
    /// </summary>
    public class PublicIpAddress : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a PublicIpAddress with no attributes set and no options.
        /// </summary>
        public PublicIpAddress() : this(null, null) { }

        /// <summary>
        /// Makes a PublicIpAddress with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public PublicIpAddress(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("ip_address"))
            {
                this.attributes.Add("ip_address", null);
            }
            if (!this.attributes.ContainsKey("server_name"))
            {
                this.attributes.Add("server_name", null);
            }
            if (!this.attributes.ContainsKey("ftp_enabled"))
            {
                this.attributes.Add("ftp_enabled", false);
            }
            if (!this.attributes.ContainsKey("sftp_enabled"))
            {
                this.attributes.Add("sftp_enabled", false);
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
        /// The public IP address.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ip_address")]
        public string IpAddress
        {
            get { return (string)attributes["ip_address"]; }
            private set { attributes["ip_address"] = value; }
        }

        /// <summary>
        /// The name of the frontend server.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_name")]
        public string ServerName
        {
            get { return (string)attributes["server_name"]; }
            private set { attributes["server_name"] = value; }
        }

        /// <summary>
        /// The ftp_enabled field of a PublicIpAddress object in the Files.com API.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ftp_enabled")]
        public bool FtpEnabled
        {
            get { return attributes["ftp_enabled"] == null ? false : (bool)attributes["ftp_enabled"]; }
            private set { attributes["ftp_enabled"] = value; }
        }

        /// <summary>
        /// The sftp_enabled field of a PublicIpAddress object in the Files.com API.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_enabled")]
        public bool SftpEnabled
        {
            get { return attributes["sftp_enabled"] == null ? false : (bool)attributes["sftp_enabled"]; }
            private set { attributes["sftp_enabled"] = value; }
        }



    }
}