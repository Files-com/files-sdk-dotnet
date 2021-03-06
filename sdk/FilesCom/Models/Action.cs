using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Action
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Action() : this(null, null) { }

        public Action(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("when"))
            {
                this.attributes.Add("when", null);
            }
            if (!this.attributes.ContainsKey("destination"))
            {
                this.attributes.Add("destination", null);
            }
            if (!this.attributes.ContainsKey("display"))
            {
                this.attributes.Add("display", null);
            }
            if (!this.attributes.ContainsKey("ip"))
            {
                this.attributes.Add("ip", null);
            }
            if (!this.attributes.ContainsKey("source"))
            {
                this.attributes.Add("source", null);
            }
            if (!this.attributes.ContainsKey("targets"))
            {
                this.attributes.Add("targets", new string[0]);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("failure_type"))
            {
                this.attributes.Add("failure_type", null);
            }
            if (!this.attributes.ContainsKey("interface"))
            {
                this.attributes.Add("interface", null);
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
        /// Action ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
        }

        /// <summary>
        /// Action occurrence date/time
        /// </summary>
        [JsonPropertyName("when")]
        public Nullable<DateTime> When
        {
            get { return (Nullable<DateTime>) attributes["when"]; }
        }

        /// <summary>
        /// The destination path for this action, if applicable
        /// </summary>
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string) attributes["destination"]; }
        }

        /// <summary>
        /// Friendly displayed output
        /// </summary>
        [JsonPropertyName("display")]
        public string Display
        {
            get { return (string) attributes["display"]; }
        }

        /// <summary>
        /// IP Address that performed this action
        /// </summary>
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string) attributes["ip"]; }
        }

        /// <summary>
        /// The source path for this action, if applicable
        /// </summary>
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string) attributes["source"]; }
        }

        /// <summary>
        /// Targets
        /// </summary>
        [JsonPropertyName("targets")]
        public string[] Targets
        {
            get { return (string[]) attributes["targets"]; }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
        }

        /// <summary>
        /// Username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string) attributes["username"]; }
        }

        /// <summary>
        /// Type of action
        /// </summary>
        [JsonPropertyName("action")]
        public string ActionType
        {
            get { return (string) attributes["action"]; }
        }

        /// <summary>
        /// Failure type.  If action was a user login or session failure, why did it fail?
        /// </summary>
        [JsonPropertyName("failure_type")]
        public string FailureType
        {
            get { return (string) attributes["failure_type"]; }
        }

        /// <summary>
        /// Interface on which this action occurred.
        /// </summary>
        [JsonPropertyName("interface")]
        public string Interface
        {
            get { return (string) attributes["interface"]; }
        }



    }
}

