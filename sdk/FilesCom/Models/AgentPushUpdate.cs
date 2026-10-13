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
    /// AgentPushUpdateEntity model
    /// </summary>
    public class AgentPushUpdate : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a AgentPushUpdate with no attributes set and no options.
        /// </summary>
        public AgentPushUpdate() : this(null, null) { }

        /// <summary>
        /// Makes a AgentPushUpdate with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public AgentPushUpdate(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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