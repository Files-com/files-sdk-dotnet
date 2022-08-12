using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class FormField
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FormField() : this(null, null) { }

        public FormField(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("label"))
            {
                this.attributes.Add("label", null);
            }
            if (!this.attributes.ContainsKey("required"))
            {
                this.attributes.Add("required", null);
            }
            if (!this.attributes.ContainsKey("help_text"))
            {
                this.attributes.Add("help_text", null);
            }
            if (!this.attributes.ContainsKey("field_type"))
            {
                this.attributes.Add("field_type", null);
            }
            if (!this.attributes.ContainsKey("options_for_select"))
            {
                this.attributes.Add("options_for_select", new string[0]);
            }
            if (!this.attributes.ContainsKey("default_option"))
            {
                this.attributes.Add("default_option", null);
            }
            if (!this.attributes.ContainsKey("form_field_set_id"))
            {
                this.attributes.Add("form_field_set_id", null);
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
        /// Form field id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Label to be displayed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("label")]
        public string Label
        {
            get { return (string) attributes["label"]; }
            private set { attributes["label"] = value; }
        }

        /// <summary>
        /// Is this a required field?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("required")]
        public bool Required
        {
            get { return (bool) attributes["required"]; }
            private set { attributes["required"] = value; }
        }

        /// <summary>
        /// Help text to be displayed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("help_text")]
        public string HelpText
        {
            get { return (string) attributes["help_text"]; }
            private set { attributes["help_text"] = value; }
        }

        /// <summary>
        /// Type of Field
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("field_type")]
        public string FieldType
        {
            get { return (string) attributes["field_type"]; }
            private set { attributes["field_type"] = value; }
        }

        /// <summary>
        /// Options to display for radio and dropdown
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("options_for_select")]
        public string[] OptionsForSelect
        {
            get { return (string[]) attributes["options_for_select"]; }
            private set { attributes["options_for_select"] = value; }
        }

        /// <summary>
        /// Default option for radio and dropdown
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("default_option")]
        public string DefaultOption
        {
            get { return (string) attributes["default_option"]; }
            private set { attributes["default_option"] = value; }
        }

        /// <summary>
        /// Form field set id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("form_field_set_id")]
        public Nullable<Int64> FormFieldSetId
        {
            get { return (Nullable<Int64>) attributes["form_field_set_id"]; }
            private set { attributes["form_field_set_id"] = value; }
        }



    }
}

