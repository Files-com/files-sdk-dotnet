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
    /// ActionEntity model
    /// </summary>
    public class HistoryAction : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a HistoryAction with no attributes set and no options.
        /// </summary>
        public HistoryAction() : this(null, null) { }

        /// <summary>
        /// Makes a HistoryAction with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public HistoryAction(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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
                this.attributes.Add("targets", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("user_is_from_parent_site"))
            {
                this.attributes.Add("user_is_from_parent_site", false);
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
            get { return new object[0]; }
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
        /// Action ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// Action occurrence date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("when")]
        public Nullable<DateTime> When
        {
            get { return (Nullable<DateTime>)attributes["when"]; }
            private set { attributes["when"] = value; }
        }

        /// <summary>
        /// The destination path for this action, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string)attributes["destination"]; }
            private set { attributes["destination"] = value; }
        }

        /// <summary>
        /// Friendly displayed output
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("display")]
        public string Display
        {
            get { return (string)attributes["display"]; }
            private set { attributes["display"] = value; }
        }

        /// <summary>
        /// IP Address that performed this action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string)attributes["ip"]; }
            private set { attributes["ip"] = value; }
        }

        /// <summary>
        /// The source path for this action, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string)attributes["source"]; }
            private set { attributes["source"] = value; }
        }

        /// <summary>
        /// Targets
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("targets")]
        public object Targets
        {
            get { return (object)attributes["targets"]; }
            private set { attributes["targets"] = value; }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            private set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Username
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            private set { attributes["username"] = value; }
        }

        /// <summary>
        /// true if this change was performed by a user on a parent site.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("user_is_from_parent_site")]
        public bool UserIsFromParentSite
        {
            get { return attributes["user_is_from_parent_site"] == null ? false : (bool)attributes["user_is_from_parent_site"]; }
            private set { attributes["user_is_from_parent_site"] = value; }
        }

        /// <summary>
        /// Type of action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("action")]
        public string ActionType
        {
            get { return (string)attributes["action"]; }
            private set { attributes["action"] = value; }
        }

        /// <summary>
        /// Failure type.  If action was a user login or session failure, why did it fail?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failure_type")]
        public string FailureType
        {
            get { return (string)attributes["failure_type"]; }
            private set { attributes["failure_type"] = value; }
        }

        /// <summary>
        /// Interface on which this action occurred.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("interface")]
        public string Interface
        {
            get { return (string)attributes["interface"]; }
            private set { attributes["interface"] = value; }
        }



    }
}