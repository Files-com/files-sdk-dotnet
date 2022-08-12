using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class AccountLineItem
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public AccountLineItem() : this(null, null) { }

        public AccountLineItem(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("amount"))
            {
                this.attributes.Add("amount", null);
            }
            if (!this.attributes.ContainsKey("balance"))
            {
                this.attributes.Add("balance", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("currency"))
            {
                this.attributes.Add("currency", null);
            }
            if (!this.attributes.ContainsKey("download_uri"))
            {
                this.attributes.Add("download_uri", null);
            }
            if (!this.attributes.ContainsKey("invoice_line_items"))
            {
                this.attributes.Add("invoice_line_items", new string[0]);
            }
            if (!this.attributes.ContainsKey("method"))
            {
                this.attributes.Add("method", null);
            }
            if (!this.attributes.ContainsKey("payment_line_items"))
            {
                this.attributes.Add("payment_line_items", new string[0]);
            }
            if (!this.attributes.ContainsKey("payment_reversed_at"))
            {
                this.attributes.Add("payment_reversed_at", null);
            }
            if (!this.attributes.ContainsKey("payment_type"))
            {
                this.attributes.Add("payment_type", null);
            }
            if (!this.attributes.ContainsKey("site_name"))
            {
                this.attributes.Add("site_name", null);
            }
            if (!this.attributes.ContainsKey("type"))
            {
                this.attributes.Add("type", null);
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
        /// Line item Id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Line item amount
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("amount")]
        public double Amount
        {
            get { return (double) attributes["amount"]; }
            private set { attributes["amount"] = value; }
        }

        /// <summary>
        /// Line item balance
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("balance")]
        public double Balance
        {
            get { return (double) attributes["balance"]; }
            private set { attributes["balance"] = value; }
        }

        /// <summary>
        /// Line item created at
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Line item currency
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("currency")]
        public string Currency
        {
            get { return (string) attributes["currency"]; }
            private set { attributes["currency"] = value; }
        }

        /// <summary>
        /// Line item download uri
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("download_uri")]
        public string DownloadUri
        {
            get { return (string) attributes["download_uri"]; }
            private set { attributes["download_uri"] = value; }
        }

        /// <summary>
        /// Associated invoice line items
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("invoice_line_items")]
        public string[] InvoiceLineItems
        {
            get { return (string[]) attributes["invoice_line_items"]; }
            private set { attributes["invoice_line_items"] = value; }
        }

        /// <summary>
        /// Line item payment method
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("method")]
        public string Method
        {
            get { return (string) attributes["method"]; }
            private set { attributes["method"] = value; }
        }

        /// <summary>
        /// Associated payment line items
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("payment_line_items")]
        public string[] PaymentLineItems
        {
            get { return (string[]) attributes["payment_line_items"]; }
            private set { attributes["payment_line_items"] = value; }
        }

        /// <summary>
        /// Date/time payment was reversed if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("payment_reversed_at")]
        public Nullable<DateTime> PaymentReversedAt
        {
            get { return (Nullable<DateTime>) attributes["payment_reversed_at"]; }
            private set { attributes["payment_reversed_at"] = value; }
        }

        /// <summary>
        /// Type of payment if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("payment_type")]
        public string PaymentType
        {
            get { return (string) attributes["payment_type"]; }
            private set { attributes["payment_type"] = value; }
        }

        /// <summary>
        /// Site name this line item is for
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_name")]
        public string SiteName
        {
            get { return (string) attributes["site_name"]; }
            private set { attributes["site_name"] = value; }
        }

        /// <summary>
        /// Type of line item, either payment or invoice
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string) attributes["type"]; }
            private set { attributes["type"] = value; }
        }

        /// <summary>
        /// Line item updated at
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>) attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }



    }
}

