using FilesCom.Util;
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
            if (!this.attributes.ContainsKey("private_key"))
            {
                this.attributes.Add("private_key", null);
            }
            if (!this.attributes.ContainsKey("subdomain"))
            {
                this.attributes.Add("subdomain", null);
            }
            if (!this.attributes.ContainsKey("root"))
            {
                this.attributes.Add("root", null);
            }
            if (!this.attributes.ContainsKey("follow_links"))
            {
                this.attributes.Add("follow_links", false);
            }
            if (!this.attributes.ContainsKey("prefer_protocol"))
            {
                this.attributes.Add("prefer_protocol", null);
            }
            if (!this.attributes.ContainsKey("dns"))
            {
                this.attributes.Add("dns", null);
            }
            if (!this.attributes.ContainsKey("proxy_all_outbound"))
            {
                this.attributes.Add("proxy_all_outbound", false);
            }
            if (!this.attributes.ContainsKey("endpoint_override"))
            {
                this.attributes.Add("endpoint_override", null);
            }
            if (!this.attributes.ContainsKey("log_file"))
            {
                this.attributes.Add("log_file", null);
            }
            if (!this.attributes.ContainsKey("log_level"))
            {
                this.attributes.Add("log_level", null);
            }
            if (!this.attributes.ContainsKey("log_rotate_num"))
            {
                this.attributes.Add("log_rotate_num", null);
            }
            if (!this.attributes.ContainsKey("log_rotate_size"))
            {
                this.attributes.Add("log_rotate_size", null);
            }
            if (!this.attributes.ContainsKey("override_max_concurrent_jobs"))
            {
                this.attributes.Add("override_max_concurrent_jobs", null);
            }
            if (!this.attributes.ContainsKey("graceful_shutdown_timeout"))
            {
                this.attributes.Add("graceful_shutdown_timeout", null);
            }
            if (!this.attributes.ContainsKey("transfer_rate_limit"))
            {
                this.attributes.Add("transfer_rate_limit", null);
            }
            if (!this.attributes.ContainsKey("api_token"))
            {
                this.attributes.Add("api_token", null);
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
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("server_host_key"))
            {
                this.attributes.Add("server_host_key", null);
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
        /// The remote server ID of the agent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// The permission set for the agent ['read_write', 'read_only', 'write_only']
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("permission_set")]
        public string PermissionSet
        {
            get { return (string)attributes["permission_set"]; }
            private set { attributes["permission_set"] = value; }
        }

        /// <summary>
        /// The private key for the agent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string)attributes["private_key"]; }
            private set { attributes["private_key"] = value; }
        }

        /// <summary>
        /// Files.com subdomain site name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("subdomain")]
        public string Subdomain
        {
            get { return (string)attributes["subdomain"]; }
            private set { attributes["subdomain"] = value; }
        }

        /// <summary>
        /// The root directory for the agent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("root")]
        public string Root
        {
            get { return (string)attributes["root"]; }
            private set { attributes["root"] = value; }
        }

        /// <summary>
        /// Follow symlinks when traversing directories
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("follow_links")]
        public bool FollowLinks
        {
            get { return attributes["follow_links"] == null ? false : (bool)attributes["follow_links"]; }
            private set { attributes["follow_links"] = value; }
        }

        /// <summary>
        /// Preferred network protocol ['udp', 'tcp'] (default udp)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("prefer_protocol")]
        public string PreferProtocol
        {
            get { return (string)attributes["prefer_protocol"]; }
            private set { attributes["prefer_protocol"] = value; }
        }

        /// <summary>
        /// DNS lookup method ['auto','doh','system'] (default auto)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dns")]
        public string Dns
        {
            get { return (string)attributes["dns"]; }
            private set { attributes["dns"] = value; }
        }

        /// <summary>
        /// Proxy all outbound traffic through files.com proxy server
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("proxy_all_outbound")]
        public bool ProxyAllOutbound
        {
            get { return attributes["proxy_all_outbound"] == null ? false : (bool)attributes["proxy_all_outbound"]; }
            private set { attributes["proxy_all_outbound"] = value; }
        }

        /// <summary>
        /// Custom site endpoint URL
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("endpoint_override")]
        public string EndpointOverride
        {
            get { return (string)attributes["endpoint_override"]; }
            private set { attributes["endpoint_override"] = value; }
        }

        /// <summary>
        /// Log file name and location
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("log_file")]
        public string LogFile
        {
            get { return (string)attributes["log_file"]; }
            private set { attributes["log_file"] = value; }
        }

        /// <summary>
        /// Log level for the agent logs ['debug', 'info', 'warn', 'error', 'fatal'] (default info)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("log_level")]
        public string LogLevel
        {
            get { return (string)attributes["log_level"]; }
            private set { attributes["log_level"] = value; }
        }

        /// <summary>
        /// Log route for agent logs. (default 5)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("log_rotate_num")]
        public Nullable<Int64> LogRotateNum
        {
            get { return (Nullable<Int64>)attributes["log_rotate_num"]; }
            private set { attributes["log_rotate_num"] = value; }
        }

        /// <summary>
        /// Log route size in MB for agent logs. (default 20)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("log_rotate_size")]
        public Nullable<Int64> LogRotateSize
        {
            get { return (Nullable<Int64>)attributes["log_rotate_size"]; }
            private set { attributes["log_rotate_size"] = value; }
        }

        /// <summary>
        /// Maximum number of concurrent jobs (default 500)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("override_max_concurrent_jobs")]
        public Nullable<Int64> OverrideMaxConcurrentJobs
        {
            get { return (Nullable<Int64>)attributes["override_max_concurrent_jobs"]; }
            private set { attributes["override_max_concurrent_jobs"] = value; }
        }

        /// <summary>
        /// Graceful shutdown timeout in seconds (default 15)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("graceful_shutdown_timeout")]
        public Nullable<Int64> GracefulShutdownTimeout
        {
            get { return (Nullable<Int64>)attributes["graceful_shutdown_timeout"]; }
            private set { attributes["graceful_shutdown_timeout"] = value; }
        }

        /// <summary>
        /// File transfer (upload/download) rate limit
        ///  `<limit>-<period>`, with the given periods:
        /// * 'S': second
        /// * 'M': minute
        /// * 'H': hour
        /// * 'D': day
        /// Examples:
        /// * 5 requests/second: '5-S'
        /// * 10 requests/minute: '10-M'
        /// * 1000 requests/hour: '1000-H'
        /// * 2000 requests/day: '2000-D'
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("transfer_rate_limit")]
        public string TransferRateLimit
        {
            get { return (string)attributes["transfer_rate_limit"]; }
            private set { attributes["transfer_rate_limit"] = value; }
        }

        /// <summary>
        /// Files Agent API Token
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("api_token")]
        public string ApiToken
        {
            get { return (string)attributes["api_token"]; }
            private set { attributes["api_token"] = value; }
        }

        /// <summary>
        /// Incoming port for files agent connections
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("port")]
        public Nullable<Int64> Port
        {
            get { return (Nullable<Int64>)attributes["port"]; }
            private set { attributes["port"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("hostname")]
        public string Hostname
        {
            get { return (string)attributes["hostname"]; }
            private set { attributes["hostname"] = value; }
        }

        /// <summary>
        /// public key
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("public_key")]
        public string PublicKey
        {
            get { return (string)attributes["public_key"]; }
            private set { attributes["public_key"] = value; }
        }

        /// <summary>
        /// either running or shutdown
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_host_key")]
        public string ServerHostKey
        {
            get { return (string)attributes["server_host_key"]; }
            private set { attributes["server_host_key"] = value; }
        }

        /// <summary>
        /// agent config version
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("config_version")]
        public string ConfigVersion
        {
            get { return (string)attributes["config_version"]; }
            private set { attributes["config_version"] = value; }
        }



    }
}