using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
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
            if (!this.attributes.ContainsKey("ip"))
            {
                this.attributes.Add("ip", null);
            }
            if (!this.attributes.ContainsKey("inbox_code"))
            {
                this.attributes.Add("inbox_code", null);
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
            if (!this.attributes.ContainsKey("bundle_code"))
            {
                this.attributes.Add("bundle_code", null);
            }
            if (!this.attributes.ContainsKey("bundle_id"))
            {
                this.attributes.Add("bundle_id", null);
            }
            if (!this.attributes.ContainsKey("bundle_recipient_id"))
            {
                this.attributes.Add("bundle_recipient_id", null);
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
        /// Registrant IP Address
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string)attributes["ip"]; }
            private set { attributes["ip"] = value; }
        }

        /// <summary>
        /// InboxRegistration cookie code, if there is an associated InboxRegistration
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("inbox_code")]
        public string InboxCode
        {
            get { return (string)attributes["inbox_code"]; }
            private set { attributes["inbox_code"] = value; }
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
        /// Bundle URL code
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_code")]
        public string BundleCode
        {
            get { return (string)attributes["bundle_code"]; }
            private set { attributes["bundle_code"] = value; }
        }

        /// <summary>
        /// Id of associated bundle
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_id")]
        public Nullable<Int64> BundleId
        {
            get { return (Nullable<Int64>)attributes["bundle_id"]; }
            private set { attributes["bundle_id"] = value; }
        }

        /// <summary>
        /// Id of associated bundle recipient
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_recipient_id")]
        public Nullable<Int64> BundleRecipientId
        {
            get { return (Nullable<Int64>)attributes["bundle_recipient_id"]; }
            private set { attributes["bundle_recipient_id"] = value; }
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
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   bundle_id - int64 - ID of the associated Bundle
        /// </summary>
        public static async Task<BundleRegistration[]> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("bundle_id") && !(parameters["bundle_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: bundle_id must be of type Nullable<Int64>", "parameters[\"bundle_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundle_registrations", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<BundleRegistration[]>(responseJson);
        }

        public static async Task<BundleRegistration[]> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}