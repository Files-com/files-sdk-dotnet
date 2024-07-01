using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
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
            if (!this.attributes.ContainsKey("bundle_id"))
            {
                this.attributes.Add("bundle_id", null);
            }
            if (!this.attributes.ContainsKey("share_after_create"))
            {
                this.attributes.Add("share_after_create", false);
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
            get { return (string)attributes["company"]; }
            set { attributes["company"] = value; }
        }

        /// <summary>
        /// The recipient's name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// A note sent to the recipient with the bundle.
        /// </summary>
        [JsonPropertyName("note")]
        public string Note
        {
            get { return (string)attributes["note"]; }
            set { attributes["note"] = value; }
        }

        /// <summary>
        /// The recipient's email address.
        /// </summary>
        [JsonPropertyName("recipient")]
        public string Recipient
        {
            get { return (string)attributes["recipient"]; }
            set { attributes["recipient"] = value; }
        }

        /// <summary>
        /// When the Bundle was shared with this recipient.
        /// </summary>
        [JsonPropertyName("sent_at")]
        public Nullable<DateTime> SentAt
        {
            get { return (Nullable<DateTime>)attributes["sent_at"]; }
            set { attributes["sent_at"] = value; }
        }

        /// <summary>
        /// Bundle to share.
        /// </summary>
        [JsonPropertyName("bundle_id")]
        public Nullable<Int64> BundleId
        {
            get { return (Nullable<Int64>)attributes["bundle_id"]; }
            set { attributes["bundle_id"] = value; }
        }

        /// <summary>
        /// Set to true to share the link with the recipient upon creation.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("share_after_create")]
        public bool ShareAfterCreate
        {
            get { return attributes["share_after_create"] == null ? false : (bool)attributes["share_after_create"]; }
            set { attributes["share_after_create"] = value; }
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The BundleRecipient object doesn't support updates.");
            }
            else
            {
                var newObj = await BundleRecipient.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[has_registrations]=desc`). Valid fields are `has_registrations`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `has_registrations`.
        ///   bundle_id (required) - int64 - List recipients for the bundle with this ID.
        /// </summary>
        public static FilesList<BundleRecipient> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("bundle_id") || parameters["bundle_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: bundle_id", "parameters[\"bundle_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("bundle_id") && !(parameters["bundle_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: bundle_id must be of type Nullable<Int64>", "parameters[\"bundle_id\"]");
            }

            return new FilesList<BundleRecipient>($"/bundle_recipients", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<BundleRecipient> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   bundle_id (required) - int64 - Bundle to share.
        ///   recipient (required) - string - Email addresses to share this bundle with.
        ///   name - string - Name of recipient.
        ///   company - string - Company of recipient.
        ///   note - string - Note to include in email.
        ///   share_after_create - boolean - Set to true to share the link with the recipient upon creation.
        /// </summary>
        public static async Task<BundleRecipient> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("bundle_id") || parameters["bundle_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: bundle_id", "parameters[\"bundle_id\"]");
            }
            if (!parameters.ContainsKey("recipient") || parameters["recipient"] == null)
            {
                throw new ArgumentNullException("Parameter missing: recipient", "parameters[\"recipient\"]");
            }
            if (parameters.ContainsKey("bundle_id") && !(parameters["bundle_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: bundle_id must be of type Nullable<Int64>", "parameters[\"bundle_id\"]");
            }
            if (parameters.ContainsKey("recipient") && !(parameters["recipient"] is string))
            {
                throw new ArgumentException("Bad parameter: recipient must be of type string", "parameters[\"recipient\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("company") && !(parameters["company"] is string))
            {
                throw new ArgumentException("Bad parameter: company must be of type string", "parameters[\"company\"]");
            }
            if (parameters.ContainsKey("note") && !(parameters["note"] is string))
            {
                throw new ArgumentException("Bad parameter: note must be of type string", "parameters[\"note\"]");
            }
            if (parameters.ContainsKey("share_after_create") && !(parameters["share_after_create"] is bool))
            {
                throw new ArgumentException("Bad parameter: share_after_create must be of type bool", "parameters[\"share_after_create\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/bundle_recipients", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<BundleRecipient>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}