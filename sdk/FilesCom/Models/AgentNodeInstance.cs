using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class AgentNodeInstance : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public AgentNodeInstance() : this(null, null) { }

        public AgentNodeInstance(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("instance_id"))
            {
                this.attributes.Add("instance_id", null);
            }
            if (!this.attributes.ContainsKey("process_state"))
            {
                this.attributes.Add("process_state", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("is_default"))
            {
                this.attributes.Add("is_default", false);
            }
            if (!this.attributes.ContainsKey("agent_version"))
            {
                this.attributes.Add("agent_version", null);
            }
            if (!this.attributes.ContainsKey("last_seen_at"))
            {
                this.attributes.Add("last_seen_at", null);
            }
            if (!this.attributes.ContainsKey("connections"))
            {
                this.attributes.Add("connections", new AgentNodeConnection[0]);
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
        /// Ephemeral ID for this running Agent process
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("instance_id")]
        public string InstanceId
        {
            get { return (string)attributes["instance_id"]; }
            private set { attributes["instance_id"] = value; }
        }

        /// <summary>
        /// Role of this process during an Agent update
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("process_state")]
        public string ProcessState
        {
            get { return (string)attributes["process_state"]; }
            private set { attributes["process_state"] = value; }
        }

        /// <summary>
        /// Whether this process has an available proxy connection
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Whether this process receives new unscoped work for its node
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
        /// Agent version reported by this process
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("agent_version")]
        public string AgentVersion
        {
            get { return (string)attributes["agent_version"]; }
            private set { attributes["agent_version"] = value; }
        }

        /// <summary>
        /// Most recent successful observation for this process
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_seen_at")]
        public Nullable<DateTime> LastSeenAt
        {
            get { return (Nullable<DateTime>)attributes["last_seen_at"]; }
            private set { attributes["last_seen_at"] = value; }
        }

        /// <summary>
        /// Proxy connections observed for this process
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("connections")]
        public AgentNodeConnection[] Connections
        {
            get { return (AgentNodeConnection[])attributes["connections"]; }
            private set { attributes["connections"] = value; }
        }



    }
}