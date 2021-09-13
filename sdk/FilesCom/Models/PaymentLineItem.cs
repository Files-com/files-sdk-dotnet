using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PaymentLineItem
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PaymentLineItem() : this(null, null) { }

        public PaymentLineItem(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("invoice_id"))
            {
                this.attributes.Add("invoice_id", null);
            }
            if (!this.attributes.ContainsKey("payment_id"))
            {
                this.attributes.Add("payment_id", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
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
        /// Payment line item amount
        /// </summary>
        [JsonPropertyName("amount")]
        public double Amount
        {
            
            get;
            set;
        }

        /// <summary>
        /// Payment line item created at date/time
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            
            get;
            set;
        }

        /// <summary>
        /// Invoice ID
        /// </summary>
        [JsonPropertyName("invoice_id")]
        public Nullable<Int64> InvoiceId
        {
            
            get;
            set;
        }

        /// <summary>
        /// Payment ID
        /// </summary>
        [JsonPropertyName("payment_id")]
        public Nullable<Int64> PaymentId
        {
            
            get;
            set;
        }

        /// <summary>
        /// Payment line item updated at date/time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            
            get;
            set;
        }



    }
}

