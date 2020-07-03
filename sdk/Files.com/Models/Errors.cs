using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Errors
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Errors()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("fields", new object[0]);
            this.attributes.Add("messages", new object[0]);
        }

        public Errors(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// A list of fields where errors occur
        /// </summary>
        [JsonPropertyName("fields")]
        public object[] Fields
        {
            get { return (object[]) attributes["fields"]; }
        }

        /// <summary>
        /// A list of error messages
        /// </summary>
        [JsonPropertyName("messages")]
        public object[] Messages
        {
            get { return (object[]) attributes["messages"]; }
        }



    }
}

