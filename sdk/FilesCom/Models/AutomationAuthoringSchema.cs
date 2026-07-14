using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class AutomationAuthoringSchema : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public AutomationAuthoringSchema() : this(null, null) { }

        public AutomationAuthoringSchema(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        public object GetOption(string name)
        {
            return (this.options.ContainsKey(name) ? this.options[name] : null);
        }

        void IModel.SetOptions(Dictionary<string, object> options)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

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