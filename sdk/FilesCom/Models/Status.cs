using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
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
            if (!this.attributes.ContainsKey("clickwrap_id"))
            {
                this.attributes.Add("clickwrap_id", null);
            }
            if (!this.attributes.ContainsKey("clickwrap_body"))
            {
                this.attributes.Add("clickwrap_body", null);
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
        /// Status HTTP code
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code")]
        public Nullable<Int64> Code
        {
            get { return (Nullable<Int64>)attributes["code"]; }
            private set { attributes["code"] = value; }
        }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            private set { attributes["message"] = value; }
        }

        /// <summary>
        /// Status message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string StatusType
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Additional data
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("data")]
        public Auto Data
        {
            get { return (Auto)attributes["data"]; }
            private set { attributes["data"] = value; }
        }

        /// <summary>
        /// A list of api errors
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("errors")]
        public string[] Errors
        {
            get { return (string[])attributes["errors"]; }
            private set { attributes["errors"] = value; }
        }

        /// <summary>
        /// Required Clickwrap id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("clickwrap_id")]
        public Nullable<Int64> ClickwrapId
        {
            get { return (Nullable<Int64>)attributes["clickwrap_id"]; }
            private set { attributes["clickwrap_id"] = value; }
        }

        /// <summary>
        /// Required Clickwrap body
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("clickwrap_body")]
        public string ClickwrapBody
        {
            get { return (string)attributes["clickwrap_body"]; }
            private set { attributes["clickwrap_body"] = value; }
        }



    }
}