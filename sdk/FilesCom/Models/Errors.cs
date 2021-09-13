using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Errors
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Errors() : this(null, null) { }

        public Errors(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("fields"))
            {
                this.attributes.Add("fields", new string[0]);
            }
            if (!this.attributes.ContainsKey("messages"))
            {
                this.attributes.Add("messages", new string[0]);
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
        /// A list of fields where errors occur
        /// </summary>
        [JsonPropertyName("fields")]
        public string[] Fields
        {
            get { return (string[]) attributes["fields"]; }
        }

        /// <summary>
        /// A list of error messages
        /// </summary>
        [JsonPropertyName("messages")]
        public string[] Messages
        {
            get { return (string[]) attributes["messages"]; }
        }



    }
}

