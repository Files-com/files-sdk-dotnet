using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class PaymentLineItem
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PaymentLineItem()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("amount", null);
            this.attributes.Add("created_at", null);
            this.attributes.Add("invoice_id", null);
            this.attributes.Add("payment_id", null);
            this.attributes.Add("updated_at", null);
        }

        public PaymentLineItem(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Payment line item amount
        /// </summary>
        [JsonPropertyName("amount")]
        public double Amount
        {
            get { return (double) attributes["amount"]; }
        }

        /// <summary>
        /// Payment line item created at date/time
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
        }

        /// <summary>
        /// Invoice ID
        /// </summary>
        [JsonPropertyName("invoice_id")]
        public Nullable<Int64> InvoiceId
        {
            get { return (Nullable<Int64>) attributes["invoice_id"]; }
        }

        /// <summary>
        /// Payment ID
        /// </summary>
        [JsonPropertyName("payment_id")]
        public Nullable<Int64> PaymentId
        {
            get { return (Nullable<Int64>) attributes["payment_id"]; }
        }

        /// <summary>
        /// Payment line item updated at date/time
        /// </summary>
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>) attributes["updated_at"]; }
        }



    }
}

