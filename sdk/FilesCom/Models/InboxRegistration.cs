using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class InboxRegistration
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public InboxRegistration() : this(null, null) { }

        public InboxRegistration(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("clickwrap_body"))
            {
                this.attributes.Add("clickwrap_body", null);
            }
            if (!this.attributes.ContainsKey("form_field_set_id"))
            {
                this.attributes.Add("form_field_set_id", null);
            }
            if (!this.attributes.ContainsKey("form_field_data"))
            {
                this.attributes.Add("form_field_data", null);
            }
            if (!this.attributes.ContainsKey("inbox_id"))
            {
                this.attributes.Add("inbox_id", null);
            }
            if (!this.attributes.ContainsKey("inbox_recipient_id"))
            {
                this.attributes.Add("inbox_recipient_id", null);
            }
            if (!this.attributes.ContainsKey("inbox_title"))
            {
                this.attributes.Add("inbox_title", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
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
        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code
        {
            get { return (string)attributes["code"]; }
            private set { attributes["code"] = value; }
        }

        /// <summary>
        /// Registrant name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            private set { attributes["name"] = value; }
        }

        /// <summary>
        /// Registrant company name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("company")]
        public string Company
        {
            get { return (string)attributes["company"]; }
            private set { attributes["company"] = value; }
        }

        /// <summary>
        /// Registrant email address
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("email")]
        public string Email
        {
            get { return (string)attributes["email"]; }
            private set { attributes["email"] = value; }
        }

        /// <summary>
        /// Clickwrap text that was shown to the registrant
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("clickwrap_body")]
        public string ClickwrapBody
        {
            get { return (string)attributes["clickwrap_body"]; }
            private set { attributes["clickwrap_body"] = value; }
        }

        /// <summary>
        /// Id of associated form field set
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("form_field_set_id")]
        public Nullable<Int64> FormFieldSetId
        {
            get { return (Nullable<Int64>)attributes["form_field_set_id"]; }
            private set { attributes["form_field_set_id"] = value; }
        }

        /// <summary>
        /// Data for form field set with form field ids as keys and user data as values
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("form_field_data")]
        public object FormFieldData
        {
            get { return (object)attributes["form_field_data"]; }
            private set { attributes["form_field_data"] = value; }
        }

        /// <summary>
        /// Id of associated inbox
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("inbox_id")]
        public Nullable<Int64> InboxId
        {
            get { return (Nullable<Int64>)attributes["inbox_id"]; }
            private set { attributes["inbox_id"] = value; }
        }

        /// <summary>
        /// Id of associated inbox recipient
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("inbox_recipient_id")]
        public Nullable<Int64> InboxRecipientId
        {
            get { return (Nullable<Int64>)attributes["inbox_recipient_id"]; }
            private set { attributes["inbox_recipient_id"] = value; }
        }

        /// <summary>
        /// Title of associated inbox
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("inbox_title")]
        public string InboxTitle
        {
            get { return (string)attributes["inbox_title"]; }
            private set { attributes["inbox_title"] = value; }
        }

        /// <summary>
        /// Registration creation date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   folder_behavior_id - int64 - ID of the associated Inbox.
        /// </summary>
        public static async Task<InboxRegistration[]> List(

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
            if (parameters.ContainsKey("folder_behavior_id") && !(parameters["folder_behavior_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: folder_behavior_id must be of type Nullable<Int64>", "parameters[\"folder_behavior_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/inbox_registrations", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<InboxRegistration[]>(responseJson);
        }

        public static async Task<InboxRegistration[]> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}