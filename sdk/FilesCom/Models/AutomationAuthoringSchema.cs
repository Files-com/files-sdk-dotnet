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
            if (!this.attributes.ContainsKey("nodes"))
            {
                this.attributes.Add("nodes", new object[0]);
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
        /// Active Automation v2 node authoring metadata.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("nodes")]
        public object[] Nodes
        {
            get { return (object[])attributes["nodes"]; }
            private set { attributes["nodes"] = value; }
        }



    }
}