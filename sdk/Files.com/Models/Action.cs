using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Action
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Action()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("path", null);
            this.attributes.Add("when", null);
            this.attributes.Add("destination", null);
            this.attributes.Add("display", null);
            this.attributes.Add("ip", null);
            this.attributes.Add("source", null);
            this.attributes.Add("targets", new object[0]);
            this.attributes.Add("user_id", null);
            this.attributes.Add("username", null);
            this.attributes.Add("action", null);
            this.attributes.Add("failure_type", null);
            this.attributes.Add("interface", null);
        }

        public Action(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
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
        public object[] Targets
        {
            get { return (object[]) attributes["targets"]; }
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

