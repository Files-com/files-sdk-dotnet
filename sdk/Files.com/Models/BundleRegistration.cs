using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class BundleRegistration
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public BundleRegistration() : this(null, null) { }

        public BundleRegistration(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("code"))
            {
                this.attributes.Add("code", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("company"))
            {
                this.attributes.Add("company", null);
            }
            if (!this.attributes.ContainsKey("email"))
            {
                this.attributes.Add("email", null);
            }
            if (!this.attributes.ContainsKey("inbox_code"))
            {
                this.attributes.Add("inbox_code", null);
            }
            if (!this.attributes.ContainsKey("form_field_set_id"))
            {
                this.attributes.Add("form_field_set_id", null);
            }
            if (!this.attributes.ContainsKey("form_field_data"))
            {
                this.attributes.Add("form_field_data", null);
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
        /// Registration cookie code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code
        {
            get { return (string) attributes["code"]; }
        }

        /// <summary>
        /// Registrant name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string) attributes["name"]; }
        }

        /// <summary>
        /// Registrant company name
        /// </summary>
        [JsonPropertyName("company")]
        public string Company
        {
            get { return (string) attributes["company"]; }
        }

        /// <summary>
        /// Registrant email address
        /// </summary>
        [JsonPropertyName("email")]
        public string Email
        {
            get { return (string) attributes["email"]; }
        }

        /// <summary>
        /// InboxRegistration cookie code, if there is an associated InboxRegistration
        /// </summary>
        [JsonPropertyName("inbox_code")]
        public string InboxCode
        {
            get { return (string) attributes["inbox_code"]; }
        }

        /// <summary>
        /// Id of associated form field set
        /// </summary>
        [JsonPropertyName("form_field_set_id")]
        public Nullable<Int64> FormFieldSetId
        {
            get { return (Nullable<Int64>) attributes["form_field_set_id"]; }
        }

        /// <summary>
        /// Data for form field set with form field ids as keys and user data as values
        /// </summary>
        [JsonPropertyName("form_field_data")]
        public string FormFieldData
        {
            get { return (string) attributes["form_field_data"]; }
        }



    }
}

