using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class AgentNode : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public AgentNode() : this(null, null) { }

        public AgentNode(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("node_id"))
            {
                this.attributes.Add("node_id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("hostname"))
            {
                this.attributes.Add("hostname", null);
            }
            if (!this.attributes.ContainsKey("availability_role"))
            {
                this.attributes.Add("availability_role", null);
            }
            if (!this.attributes.ContainsKey("connection_status"))
            {
                this.attributes.Add("connection_status", null);
            }
            if (!this.attributes.ContainsKey("is_default"))
            {
                this.attributes.Add("is_default", false);
            }
            if (!this.attributes.ContainsKey("agent_version"))
            {
                this.attributes.Add("agent_version", null);
            }
            if (!this.attributes.ContainsKey("direct_transfer_available"))
            {
                this.attributes.Add("direct_transfer_available", false);
            }
            if (!this.attributes.ContainsKey("last_seen_at"))
            {
                this.attributes.Add("last_seen_at", null);
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
        /// Stable Agent installation ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("node_id")]
        public string NodeId
        {
            get { return (string)attributes["node_id"]; }
            private set { attributes["node_id"] = value; }
        }

        /// <summary>
        /// Customer-configured Agent node name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            private set { attributes["name"] = value; }
        }

        /// <summary>
        /// Hostname reported by the Agent
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("hostname")]
        public string Hostname
        {
            get { return (string)attributes["hostname"]; }
            private set { attributes["hostname"] = value; }
        }

        /// <summary>
        /// Configured traffic preference
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("availability_role")]
        public string AvailabilityRole
        {
            get { return (string)attributes["availability_role"]; }
            private set { attributes["availability_role"] = value; }
        }

        /// <summary>
        /// Whether this node is currently available for traffic
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("connection_status")]
        public string ConnectionStatus
        {
            get { return (string)attributes["connection_status"]; }
            private set { attributes["connection_status"] = value; }
        }

        /// <summary>
        /// Whether this node is the current default route for new unscoped work
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("is_default")]
        public bool IsDefault
        {
            get { return attributes["is_default"] == null ? false : (bool)attributes["is_default"]; }
            private set { attributes["is_default"] = value; }
        }

        /// <summary>
        /// Agent version reported by this node
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("agent_version")]
        public string AgentVersion
        {
            get { return (string)attributes["agent_version"]; }
            private set { attributes["agent_version"] = value; }
        }

        /// <summary>
        /// Whether the proxy recently validated a direct connection to this Agent node. False means direct transfers are enabled but not currently available; null means disabled or unsupported.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("direct_transfer_available")]
        public bool DirectTransferAvailable
        {
            get { return attributes["direct_transfer_available"] == null ? false : (bool)attributes["direct_transfer_available"]; }
            private set { attributes["direct_transfer_available"] = value; }
        }

        /// <summary>
        /// Most recent successful node observation
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_seen_at")]
        public Nullable<DateTime> LastSeenAt
        {
            get { return (Nullable<DateTime>)attributes["last_seen_at"]; }
            private set { attributes["last_seen_at"] = value; }
        }



    }
}