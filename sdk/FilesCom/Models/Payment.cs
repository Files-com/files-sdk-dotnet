using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Payment
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Payment() : this(null, null) { }

        public Payment(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
                this.attributes.Add("invoice_line_items", new object[0]);
            }
            if (!this.attributes.ContainsKey("method"))
            {
                this.attributes.Add("method", null);
            }
            if (!this.attributes.ContainsKey("payment_line_items"))
            {
                this.attributes.Add("payment_line_items", new object[0]);
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
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Line item amount
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("amount")]
        public double Amount
        {
            get { return (double)attributes["amount"]; }
            private set { attributes["amount"] = value; }
        }

        /// <summary>
        /// Line item balance
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("balance")]
        public double Balance
        {
            get { return (double)attributes["balance"]; }
            private set { attributes["balance"] = value; }
        }

        /// <summary>
        /// Line item created at
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Line item currency
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("currency")]
        public string Currency
        {
            get { return (string)attributes["currency"]; }
            private set { attributes["currency"] = value; }
        }

        /// <summary>
        /// Line item download uri
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("download_uri")]
        public string DownloadUri
        {
            get { return (string)attributes["download_uri"]; }
            private set { attributes["download_uri"] = value; }
        }

        /// <summary>
        /// Associated invoice line items
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("invoice_line_items")]
        public object[] InvoiceLineItems
        {
            get { return (object[])attributes["invoice_line_items"]; }
            private set { attributes["invoice_line_items"] = value; }
        }

        /// <summary>
        /// Line item payment method
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("method")]
        public string Method
        {
            get { return (string)attributes["method"]; }
            private set { attributes["method"] = value; }
        }

        /// <summary>
        /// Associated payment line items
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("payment_line_items")]
        public object[] PaymentLineItems
        {
            get { return (object[])attributes["payment_line_items"]; }
            private set { attributes["payment_line_items"] = value; }
        }

        /// <summary>
        /// Date/time payment was reversed if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("payment_reversed_at")]
        public Nullable<DateTime> PaymentReversedAt
        {
            get { return (Nullable<DateTime>)attributes["payment_reversed_at"]; }
            private set { attributes["payment_reversed_at"] = value; }
        }

        /// <summary>
        /// Type of payment if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("payment_type")]
        public string PaymentType
        {
            get { return (string)attributes["payment_type"]; }
            private set { attributes["payment_type"] = value; }
        }

        /// <summary>
        /// Site name this line item is for
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_name")]
        public string SiteName
        {
            get { return (string)attributes["site_name"]; }
            private set { attributes["site_name"] = value; }
        }

        /// <summary>
        /// Type of line item, either payment or invoice
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string)attributes["type"]; }
            private set { attributes["type"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<AccountLineItem> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }

            return new FilesList<AccountLineItem>($"/payments", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<AccountLineItem> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
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

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/payments/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<AccountLineItem>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<AccountLineItem> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// </summary>
        public static async Task<Export> CreateExport(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendStringRequest($"/payments/create_export", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Export>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}