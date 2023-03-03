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
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("bundle_id"))
            {
                this.attributes.Add("bundle_id", null);
            }
            if (!this.attributes.ContainsKey("share_after_create"))
            {
                this.attributes.Add("share_after_create", null);
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
        /// User ID.  Provide a value of `0` to operate the current session's user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
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
        [JsonPropertyName("share_after_create")]
        public bool ShareAfterCreate
        {
            get { return (bool)attributes["share_after_create"]; }
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
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `has_registrations`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `has_registrations`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `has_registrations`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal to the supplied value. Valid fields are `has_registrations`.
        ///   filter_like - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `has_registrations`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `has_registrations`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal to the supplied value. Valid fields are `has_registrations`.
        ///   bundle_id (required) - int64 - List recipients for the bundle with this ID.
        /// </summary>
        public static async Task<BundleRecipient[]> List(

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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_like") && !(parameters["filter_like"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_like must be of type object", "parameters[\"filter_like\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }
            if (parameters.ContainsKey("bundle_id") && !(parameters["bundle_id"] is Nullable<Int64>))
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

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
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

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
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
            if (!parameters.ContainsKey("bundle_id") || parameters["bundle_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: bundle_id", "parameters[\"bundle_id\"]");
            }
            if (!parameters.ContainsKey("recipient") || parameters["recipient"] == null)
            {
                throw new ArgumentNullException("Parameter missing: recipient", "parameters[\"recipient\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundle_recipients", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<BundleRecipient>(responseJson);
        }


    }
}