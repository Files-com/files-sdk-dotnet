using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class AgentPushUpdate
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public AgentPushUpdate() : this(null, null) { }

        public AgentPushUpdate(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("current_version"))
            {
                this.attributes.Add("current_version", null);
            }
            if (!this.attributes.ContainsKey("pending_version"))
            {
                this.attributes.Add("pending_version", null);
            }
            if (!this.attributes.ContainsKey("last_error"))
            {
                this.attributes.Add("last_error", null);
            }
            if (!this.attributes.ContainsKey("error"))
            {
                this.attributes.Add("error", null);
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
        /// Pushed agent version
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("version")]
        public string Version
        {
            get { return (string)attributes["version"]; }
            private set { attributes["version"] = value; }
        }

        /// <summary>
        /// Update accepted or reason
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            private set { attributes["message"] = value; }
        }

        /// <summary>
        /// Installed agent version
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("current_version")]
        public string CurrentVersion
        {
            get { return (string)attributes["current_version"]; }
            private set { attributes["current_version"] = value; }
        }

        /// <summary>
        /// Pending agent version or null
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("pending_version")]
        public string PendingVersion
        {
            get { return (string)attributes["pending_version"]; }
            private set { attributes["pending_version"] = value; }
        }

        /// <summary>
        /// Last error message or null
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_error")]
        public string LastError
        {
            get { return (string)attributes["last_error"]; }
            private set { attributes["last_error"] = value; }
        }

        /// <summary>
        /// Error code
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error")]
        public string Error
        {
            get { return (string)attributes["error"]; }
            private set { attributes["error"] = value; }
        }



    }
}