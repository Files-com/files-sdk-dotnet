using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Payment
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Payment()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("amount", null);
            this.attributes.Add("balance", null);
            this.attributes.Add("created_at", null);
            this.attributes.Add("currency", null);
            this.attributes.Add("download_uri", null);
            this.attributes.Add("invoice_line_items", new object[0]);
            this.attributes.Add("method", null);
            this.attributes.Add("payment_line_items", new object[0]);
            this.attributes.Add("payment_reversed_at", null);
            this.attributes.Add("payment_type", null);
            this.attributes.Add("site_name", null);
            this.attributes.Add("type", null);
            this.attributes.Add("updated_at", null);
        }

        public Payment(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Line item Id
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// Line item amount
        /// </summary>
        [JsonPropertyName("amount")]
        public double Amount
        {
            get { return (double) attributes["amount"]; }
        }

        /// <summary>
        /// Line item balance
        /// </summary>
        [JsonPropertyName("balance")]
        public double Balance
        {
            get { return (double) attributes["balance"]; }
        }

        /// <summary>
        /// Line item created at
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
        }

        /// <summary>
        /// Line item currency
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency
        {
            get { return (string) attributes["currency"]; }
        }

        /// <summary>
        /// Line item download uri
        /// </summary>
        [JsonPropertyName("download_uri")]
        public string DownloadUri
        {
            get { return (string) attributes["download_uri"]; }
        }

        /// <summary>
        /// Associated invoice line items
        /// </summary>
        [JsonPropertyName("invoice_line_items")]
        public object[] InvoiceLineItems
        {
            get { return (object[]) attributes["invoice_line_items"]; }
        }

        /// <summary>
        /// Line item payment method
        /// </summary>
        [JsonPropertyName("method")]
        public string Method
        {
            get { return (string) attributes["method"]; }
        }

        /// <summary>
        /// Associated payment line items
        /// </summary>
        [JsonPropertyName("payment_line_items")]
        public object[] PaymentLineItems
        {
            get { return (object[]) attributes["payment_line_items"]; }
        }

        /// <summary>
        /// Date/time payment was reversed if applicable
        /// </summary>
        [JsonPropertyName("payment_reversed_at")]
        public Nullable<DateTime> PaymentReversedAt
        {
            get { return (Nullable<DateTime>) attributes["payment_reversed_at"]; }
        }

        /// <summary>
        /// Type of payment if applicable
        /// </summary>
        [JsonPropertyName("payment_type")]
        public string PaymentType
        {
            get { return (string) attributes["payment_type"]; }
        }

        /// <summary>
        /// Site name this line item is for
        /// </summary>
        [JsonPropertyName("site_name")]
        public string SiteName
        {
            get { return (string) attributes["site_name"]; }
        }

        /// <summary>
        /// Type of line item, either payment or invoice
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string) attributes["type"]; }
        }

        /// <summary>
        /// Line item updated at
        /// </summary>
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>) attributes["updated_at"]; }
        }



        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        /// </summary>
        public static async Task<Payment[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/payments", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Payment[]>(responseJson);
        }

        public static async Task<Payment[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Payment ID.
        /// </summary>
        public static async Task<AccountLineItem> Find(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/payments/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<AccountLineItem>(responseJson);
        }

        public static async Task<AccountLineItem> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

    }
}

