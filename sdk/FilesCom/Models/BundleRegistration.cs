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
            if (!this.attributes.ContainsKey("bundle_id"))
            {
                this.attributes.Add("bundle_id", null);
            }
            if (!this.attributes.ContainsKey("bundle_recipient_id"))
            {
                this.attributes.Add("bundle_recipient_id", null);
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
        [JsonPropertyName("code")]
        public string Code
        {
            
            get;
            set;
        }

        /// <summary>
        /// Registrant name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            
            get;
            set;
        }

        /// <summary>
        /// Registrant company name
        /// </summary>
        [JsonPropertyName("company")]
        public string Company
        {
            
            get;
            set;
        }

        /// <summary>
        /// Registrant email address
        /// </summary>
        [JsonPropertyName("email")]
        public string Email
        {
            
            get;
            set;
        }

        /// <summary>
        /// Registrant IP Address
        /// </summary>
        [JsonPropertyName("ip")]
        public string Ip
        {
            
            get;
            set;
        }

        /// <summary>
        /// InboxRegistration cookie code, if there is an associated InboxRegistration
        /// </summary>
        [JsonPropertyName("inbox_code")]
        public string InboxCode
        {
            
            get;
            set;
        }

        /// <summary>
        /// Clickwrap text that was shown to the registrant
        /// </summary>
        [JsonPropertyName("clickwrap_body")]
        public string ClickwrapBody
        {
            
            get;
            set;
        }

        /// <summary>
        /// Id of associated form field set
        /// </summary>
        [JsonPropertyName("form_field_set_id")]
        public Nullable<Int64> FormFieldSetId
        {
            
            get;
            set;
        }

        /// <summary>
        /// Data for form field set with form field ids as keys and user data as values
        /// </summary>
        [JsonPropertyName("form_field_data")]
        public string FormFieldData
        {
            
            get;
            set;
        }

        /// <summary>
        /// Id of associated bundle
        /// </summary>
        [JsonPropertyName("bundle_id")]
        public Nullable<Int64> BundleId
        {
            
            get;
            set;
        }

        /// <summary>
        /// Id of associated bundle recipient
        /// </summary>
        [JsonPropertyName("bundle_recipient_id")]
        public Nullable<Int64> BundleRecipientId
        {
            
            get;
            set;
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
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

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("bundle_id") && !(parameters["bundle_id"] is Nullable<Int64> ))
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

