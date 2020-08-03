using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class BundleRecipient
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public BundleRecipient() : this(null, null) { }

        public BundleRecipient(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("company"))
            {
                this.attributes.Add("company", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("note"))
            {
                this.attributes.Add("note", null);
            }
            if (!this.attributes.ContainsKey("recipient"))
            {
                this.attributes.Add("recipient", null);
            }
            if (!this.attributes.ContainsKey("sent_at"))
            {
                this.attributes.Add("sent_at", null);
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
        /// The recipient's company.
        /// </summary>
        [JsonPropertyName("company")]
        public string Company
        {
            get { return (string) attributes["company"]; }
        }

        /// <summary>
        /// The recipient's name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string) attributes["name"]; }
        }

        /// <summary>
        /// A note sent to the recipient with the bundle.
        /// </summary>
        [JsonPropertyName("note")]
        public string Note
        {
            get { return (string) attributes["note"]; }
        }

        /// <summary>
        /// The recipient's email address.
        /// </summary>
        [JsonPropertyName("recipient")]
        public string Recipient
        {
            get { return (string) attributes["recipient"]; }
        }

        /// <summary>
        /// When the Bundle was shared with this recipient.
        /// </summary>
        [JsonPropertyName("sent_at")]
        public Nullable<DateTime> SentAt
        {
            get { return (Nullable<DateTime>) attributes["sent_at"]; }
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        ///   bundle_id (required) - int64 - List recipients for the bundle with this ID.
        /// </summary>
        public static async Task<BundleRecipient[]> List(
            
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
            if (parameters.ContainsKey("bundle_id") && !(parameters["bundle_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: bundle_id must be of type Nullable<Int64>", "parameters[\"bundle_id\"]");
            }
            if (!parameters.ContainsKey("bundle_id") || parameters["bundle_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: bundle_id", "parameters[\"bundle_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundle_recipients", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<BundleRecipient[]>(responseJson);
        }

        public static async Task<BundleRecipient[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

