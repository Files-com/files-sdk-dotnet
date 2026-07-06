using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ChatMessage : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ChatMessage() : this(null, null) { }

        public ChatMessage(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("role"))
            {
                this.attributes.Add("role", null);
            }
            if (!this.attributes.ContainsKey("content"))
            {
                this.attributes.Add("content", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
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
        /// Chat Message ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Message role.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("role")]
        public string Role
        {
            get { return (string)attributes["role"]; }
            private set { attributes["role"] = value; }
        }

        /// <summary>
        /// Message content.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content")]
        public string Content
        {
            get { return (string)attributes["content"]; }
            private set { attributes["content"] = value; }
        }

        /// <summary>
        /// Message creation date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }



    }
}