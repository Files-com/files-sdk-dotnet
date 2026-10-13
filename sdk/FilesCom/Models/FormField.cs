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
    /// A FormField object of the Files.com API.
    /// </summary>
    public class FormField : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a FormField with no attributes set and no options.
        /// </summary>
        public FormField() : this(null, null) { }

        /// <summary>
        /// Makes a FormField with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FormField(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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
                this.attributes.Add("required", false);
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
        /// Form field id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Label to be displayed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("label")]
        public string Label
        {
            get { return (string)attributes["label"]; }
            private set { attributes["label"] = value; }
        }

        /// <summary>
        /// Is this a required field?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("required")]
        public bool Required
        {
            get { return attributes["required"] == null ? false : (bool)attributes["required"]; }
            private set { attributes["required"] = value; }
        }

        /// <summary>
        /// Help text to be displayed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("help_text")]
        public string HelpText
        {
            get { return (string)attributes["help_text"]; }
            private set { attributes["help_text"] = value; }
        }

        /// <summary>
        /// Type of Field
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("field_type")]
        public string FieldType
        {
            get { return (string)attributes["field_type"]; }
            private set { attributes["field_type"] = value; }
        }

        /// <summary>
        /// Options to display for radio and dropdown
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("options_for_select")]
        public string[] OptionsForSelect
        {
            get { return (string[])attributes["options_for_select"]; }
            private set { attributes["options_for_select"] = value; }
        }

        /// <summary>
        /// Default option for radio and dropdown
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("default_option")]
        public string DefaultOption
        {
            get { return (string)attributes["default_option"]; }
            private set { attributes["default_option"] = value; }
        }

        /// <summary>
        /// Form field set id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("form_field_set_id")]
        public Nullable<Int64> FormFieldSetId
        {
            get { return (Nullable<Int64>)attributes["form_field_set_id"]; }
            private set { attributes["form_field_set_id"] = value; }
        }



    }
}