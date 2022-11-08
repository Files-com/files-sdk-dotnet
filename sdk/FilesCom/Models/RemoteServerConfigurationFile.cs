using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class RemoteServerConfigurationFile
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public RemoteServerConfigurationFile() : this(null, null) { }

        public RemoteServerConfigurationFile(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("permission_set"))
            {
                this.attributes.Add("permission_set", null);
            }
            if (!this.attributes.ContainsKey("api_token"))
            {
                this.attributes.Add("api_token", null);
            }
            if (!this.attributes.ContainsKey("root"))
            {
                this.attributes.Add("root", null);
            }
            if (!this.attributes.ContainsKey("port"))
            {
                this.attributes.Add("port", null);
            }
            if (!this.attributes.ContainsKey("hostname"))
            {
                this.attributes.Add("hostname", null);
            }
            if (!this.attributes.ContainsKey("public_key"))
            {
                this.attributes.Add("public_key", null);
            }
            if (!this.attributes.ContainsKey("private_key"))
            {
                this.attributes.Add("private_key", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("config_version"))
            {
                this.attributes.Add("config_version", null);
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
        /// Agent ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("permission_set")]
        public string PermissionSet
        {
            get { return (string) attributes["permission_set"]; }
            private set { attributes["permission_set"] = value; }
        }

        /// <summary>
        /// Files Agent API Token
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("api_token")]
        public string ApiToken
        {
            get { return (string) attributes["api_token"]; }
            private set { attributes["api_token"] = value; }
        }

        /// <summary>
        /// Agent local root path
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("root")]
        public string Root
        {
            get { return (string) attributes["root"]; }
            private set { attributes["root"] = value; }
        }

        /// <summary>
        /// Incoming port for files agent connections
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("port")]
        public Nullable<Int64> Port
        {
            get { return (Nullable<Int64>) attributes["port"]; }
            private set { attributes["port"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("hostname")]
        public string Hostname
        {
            get { return (string) attributes["hostname"]; }
            private set { attributes["hostname"] = value; }
        }

        /// <summary>
        /// public key
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("public_key")]
        public string PublicKey
        {
            get { return (string) attributes["public_key"]; }
            private set { attributes["public_key"] = value; }
        }

        /// <summary>
        /// private key
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string) attributes["private_key"]; }
            private set { attributes["private_key"] = value; }
        }

        /// <summary>
        /// either running or shutdown
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string) attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// agent config version
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("config_version")]
        public string ConfigVersion
        {
            get { return (string) attributes["config_version"]; }
            private set { attributes["config_version"] = value; }
        }



    }
}

