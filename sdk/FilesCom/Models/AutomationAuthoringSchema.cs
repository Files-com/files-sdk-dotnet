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
    /// AutomationAuthoringSchemaEntity model
    /// </summary>
    public class AutomationAuthoringSchema : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a AutomationAuthoringSchema with no attributes set and no options.
        /// </summary>
        public AutomationAuthoringSchema() : this(null, null) { }

        /// <summary>
        /// Makes a AutomationAuthoringSchema with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public AutomationAuthoringSchema(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("definition_schema"))
            {
                this.attributes.Add("definition_schema", null);
            }
            if (!this.attributes.ContainsKey("error_families"))
            {
                this.attributes.Add("error_families", new object[0]);
            }
            if (!this.attributes.ContainsKey("nodes"))
            {
                this.attributes.Add("nodes", new object[0]);
            }
            if (!this.attributes.ContainsKey("schema_url"))
            {
                this.attributes.Add("schema_url", null);
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
        /// JSON Schema for active Automation v2 graph definitions.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("definition_schema")]
        public object DefinitionSchema
        {
            get { return (object)attributes["definition_schema"]; }
            private set { attributes["definition_schema"] = value; }
        }

        /// <summary>
        /// Typed error families accepted by Automation v2 on_error rules.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error_families")]
        public object[] ErrorFamilies
        {
            get { return (object[])attributes["error_families"]; }
            private set { attributes["error_families"] = value; }
        }

        /// <summary>
        /// Active Automation v2 node authoring metadata.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("nodes")]
        public object[] Nodes
        {
            get { return (object[])attributes["nodes"]; }
            private set { attributes["nodes"] = value; }
        }

        /// <summary>
        /// Stable public URL for the Automation v2 graph definition JSON Schema.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("schema_url")]
        public string SchemaUrl
        {
            get { return (string)attributes["schema_url"]; }
            private set { attributes["schema_url"] = value; }
        }



    }
}