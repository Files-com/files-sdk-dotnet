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
    /// StatusEntity model
    /// </summary>
    public class Status : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a Status with no attributes set and no options.
        /// </summary>
        public Status() : this(null, null) { }

        /// <summary>
        /// Makes a Status with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Status(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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
                this.attributes.Add("errors", new Errors[0]);
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
            get { return new object[] { Data, Errors }; }
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
        public Errors[] Errors
        {
            get { return (Errors[])attributes["errors"]; }
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