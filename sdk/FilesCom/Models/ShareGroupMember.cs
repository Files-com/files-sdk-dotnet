using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ShareGroupMember
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ShareGroupMember() : this(null, null) { }

        public ShareGroupMember(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("company"))
            {
                this.attributes.Add("company", null);
            }
            if (!this.attributes.ContainsKey("email"))
            {
                this.attributes.Add("email", null);
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
        /// Name of the share group member
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            private set { attributes["name"] = value; }
        }

        /// <summary>
        /// Company of the share group member
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("company")]
        public string Company
        {
            get { return (string)attributes["company"]; }
            private set { attributes["company"] = value; }
        }

        /// <summary>
        /// Email of the share group member
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("email")]
        public string Email
        {
            get { return (string)attributes["email"]; }
            private set { attributes["email"] = value; }
        }



    }
}