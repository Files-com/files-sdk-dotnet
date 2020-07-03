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
        public Status()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("code", null);
            this.attributes.Add("message", null);
            this.attributes.Add("status", null);
            this.attributes.Add("data", null);
            this.attributes.Add("errors", new object[0]);
        }

        public Status(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
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
        public object[] Errors
        {
            get { return (object[]) attributes["errors"]; }
        }



    }
}

