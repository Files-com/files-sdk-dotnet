using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class EmailLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public EmailLog() : this(null, null) { }

        public EmailLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("timestamp"))
            {
                this.attributes.Add("timestamp", null);
            }
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("subject"))
            {
                this.attributes.Add("subject", null);
            }
            if (!this.attributes.ContainsKey("to"))
            {
                this.attributes.Add("to", null);
            }
            if (!this.attributes.ContainsKey("cc"))
            {
                this.attributes.Add("cc", null);
            }
            if (!this.attributes.ContainsKey("delivery_method"))
            {
                this.attributes.Add("delivery_method", null);
            }
            if (!this.attributes.ContainsKey("smtp_hostname"))
            {
                this.attributes.Add("smtp_hostname", null);
            }
            if (!this.attributes.ContainsKey("smtp_ip"))
            {
                this.attributes.Add("smtp_ip", null);
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
        /// Start Time of Action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("timestamp")]
        public Nullable<DateTime> Timestamp
        {
            get { return (Nullable<DateTime>)attributes["timestamp"]; }
            private set { attributes["timestamp"] = value; }
        }

        /// <summary>
        /// Log Message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            private set { attributes["message"] = value; }
        }

        /// <summary>
        /// Status of E-Mail delivery
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Subject line of E-Mail
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("subject")]
        public string Subject
        {
            get { return (string)attributes["subject"]; }
            private set { attributes["subject"] = value; }
        }

        /// <summary>
        /// To field of E-Mail
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("to")]
        public string To
        {
            get { return (string)attributes["to"]; }
            private set { attributes["to"] = value; }
        }

        /// <summary>
        /// CC field of E-Mail
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cc")]
        public string Cc
        {
            get { return (string)attributes["cc"]; }
            private set { attributes["cc"] = value; }
        }

        /// <summary>
        /// How was the email delivered?  `customer_smtp` or `files.com`
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("delivery_method")]
        public string DeliveryMethod
        {
            get { return (string)attributes["delivery_method"]; }
            private set { attributes["delivery_method"] = value; }
        }

        /// <summary>
        /// Customer SMTP Hostname used.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smtp_hostname")]
        public string SmtpHostname
        {
            get { return (string)attributes["smtp_hostname"]; }
            private set { attributes["smtp_hostname"] = value; }
        }

        /// <summary>
        /// Customer SMTP IP address as resolved for use (useful for troubleshooting DNS issues with customer SMTP).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smtp_ip")]
        public string SmtpIp
        {
            get { return (string)attributes["smtp_ip"]; }
            private set { attributes["smtp_ip"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date` or `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ status ]`, `[ start_date, end_date ]`, `[ start_date, status ]` or `[ end_date, status ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ status ]`, `[ start_date, end_date ]`, `[ start_date, status ]` or `[ end_date, status ]`.
        /// </summary>
        public static FilesList<EmailLog> List(

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
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }

            return new FilesList<EmailLog>($"/email_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<EmailLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}