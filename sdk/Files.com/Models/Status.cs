using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Status
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Status() : this(null, null) { }

        public Status(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("code"))
            {
                this.attributes.Add("code", null);
            }
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("data"))
            {
                this.attributes.Add("data", null);
            }
            if (!this.attributes.ContainsKey("errors"))
            {
                this.attributes.Add("errors", new string[0]);
            }
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
        /// Status http code
        /// </summary>
        [JsonPropertyName("code")]
        public Nullable<Int64> Code
        {
            get { return (Nullable<Int64>) attributes["code"]; }
        }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string) attributes["message"]; }
        }

        /// <summary>
        /// Status message
        /// </summary>
        [JsonPropertyName("status")]
        public string StatusType
        {
            get { return (string) attributes["status"]; }
        }

        /// <summary>
        /// Additional data
        /// </summary>
        [JsonPropertyName("data")]
        public object Data
        {
            get { return (object) attributes["data"]; }
        }

        /// <summary>
        /// A list of api errors
        /// </summary>
        [JsonPropertyName("errors")]
        public string[] Errors
        {
            get { return (string[]) attributes["errors"]; }
        }



    }
}

