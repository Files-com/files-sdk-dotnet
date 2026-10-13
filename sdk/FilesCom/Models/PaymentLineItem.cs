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
    /// A PaymentLineItem object of the Files.com API.
    /// </summary>
    public class PaymentLineItem : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a PaymentLineItem with no attributes set and no options.
        /// </summary>
        public PaymentLineItem() : this(null, null) { }

        /// <summary>
        /// Makes a PaymentLineItem with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public PaymentLineItem(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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
        /// Payment line item amount
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("amount")]
        public Nullable<decimal> Amount
        {
            get { return (Nullable<decimal>)attributes["amount"]; }
            private set { attributes["amount"] = value; }
        }

        /// <summary>
        /// Payment line item created at date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Invoice ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("invoice_id")]
        public Nullable<Int64> InvoiceId
        {
            get { return (Nullable<Int64>)attributes["invoice_id"]; }
            private set { attributes["invoice_id"] = value; }
        }

        /// <summary>
        /// Payment ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("payment_id")]
        public Nullable<Int64> PaymentId
        {
            get { return (Nullable<Int64>)attributes["payment_id"]; }
            private set { attributes["payment_id"] = value; }
        }



    }
}