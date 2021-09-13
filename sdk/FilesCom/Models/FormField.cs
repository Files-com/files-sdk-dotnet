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
                this.attributes.Add("options_for_select", null);
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
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            
            get;
            set;
        }

        /// <summary>
        /// Label to be displayed
        /// </summary>
        [JsonPropertyName("label")]
        public string Label
        {
            
            get;
            set;
        }

        /// <summary>
        /// Is this a required field?
        /// </summary>
        [JsonPropertyName("required")]
        public bool Required
        {
            
            get;
            set;
        }

        /// <summary>
        /// Help text to be displayed
        /// </summary>
        [JsonPropertyName("help_text")]
        public string HelpText
        {
            
            get;
            set;
        }

        /// <summary>
        /// Type of Field
        /// </summary>
        [JsonPropertyName("field_type")]
        public string FieldType
        {
            
            get;
            set;
        }

        /// <summary>
        /// Options to display for radio and dropdown
        /// </summary>
        [JsonPropertyName("options_for_select")]
        public string OptionsForSelect
        {
            
            get;
            set;
        }

        /// <summary>
        /// Default option for radio and dropdown
        /// </summary>
        [JsonPropertyName("default_option")]
        public string DefaultOption
        {
            
            get;
            set;
        }

        /// <summary>
        /// Form field set id
        /// </summary>
        [JsonPropertyName("form_field_set_id")]
        public Nullable<Int64> FormFieldSetId
        {
            
            get;
            set;
        }



    }
}

