using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
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
            if (!this.attributes.ContainsKey("ftp_enabled"))
            {
                this.attributes.Add("ftp_enabled", false);
            }
            if (!this.attributes.ContainsKey("sftp_enabled"))
            {
                this.attributes.Add("sftp_enabled", false);
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