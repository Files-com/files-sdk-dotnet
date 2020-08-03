using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class InvoiceLineItem
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public InvoiceLineItem() : this(null, null) { }

        public InvoiceLineItem(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("amount"))
            {
                this.attributes.Add("amount", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("type"))
            {
                this.attributes.Add("type", null);
            }
            if (!this.attributes.ContainsKey("service_end_at"))
            {
                this.attributes.Add("service_end_at", null);
            }
            if (!this.attributes.ContainsKey("service_start_at"))
            {
                this.attributes.Add("service_start_at", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
            }
            if (!this.attributes.ContainsKey("plan"))
            {
                this.attributes.Add("plan", null);
            }
            if (!this.attributes.ContainsKey("site"))
            {
                this.attributes.Add("site", null);
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
        /// Invoice line item amount
        /// </summary>
        [JsonPropertyName("amount")]
        public double Amount
        {
            get { return (double) attributes["amount"]; }
        }

        /// <summary>
        /// Invoice line item created at date/time
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
        }

        /// <summary>
        /// Invoice line item description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string) attributes["description"]; }
        }

        /// <summary>
        /// Invoice line item type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string) attributes["type"]; }
        }

        /// <summary>
        /// Invoice line item service end date/time
        /// </summary>
        [JsonPropertyName("service_end_at")]
        public Nullable<DateTime> ServiceEndAt
        {
            get { return (Nullable<DateTime>) attributes["service_end_at"]; }
        }

        /// <summary>
        /// Invoice line item service start date/time
        /// </summary>
        [JsonPropertyName("service_start_at")]
        public Nullable<DateTime> ServiceStartAt
        {
            get { return (Nullable<DateTime>) attributes["service_start_at"]; }
        }

        /// <summary>
        /// Invoice line item updated date/time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>) attributes["updated_at"]; }
        }

        /// <summary>
        /// Plan name
        /// </summary>
        [JsonPropertyName("plan")]
        public string Plan
        {
            get { return (string) attributes["plan"]; }
        }

        /// <summary>
        /// Site name
        /// </summary>
        [JsonPropertyName("site")]
        public string Site
        {
            get { return (string) attributes["site"]; }
        }



    }
}

