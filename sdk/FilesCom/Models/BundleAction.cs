using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class BundleAction
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public BundleAction() : this(null, null) { }

        public BundleAction(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("bundle_registration"))
            {
                this.attributes.Add("bundle_registration", null);
            }
            if (!this.attributes.ContainsKey("when"))
            {
                this.attributes.Add("when", null);
            }
            if (!this.attributes.ContainsKey("destination"))
            {
                this.attributes.Add("destination", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("source"))
            {
                this.attributes.Add("source", null);
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
        /// Type of action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            private set { attributes["action"] = value; }
        }

        /// <summary>
        /// Object that contains bundle registration information
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_registration")]
        public BundleRegistration BundleRegistration
        {
            get { return (BundleRegistration)attributes["bundle_registration"]; }
            private set { attributes["bundle_registration"] = value; }
        }

        /// <summary>
        /// Action occurrence date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("when")]
        public Nullable<DateTime> When
        {
            get { return (Nullable<DateTime>)attributes["when"]; }
            private set { attributes["when"] = value; }
        }

        /// <summary>
        /// The destination path for this bundle action, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string)attributes["destination"]; }
            private set { attributes["destination"] = value; }
        }

        /// <summary>
        /// Path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// The source path for this bundle action, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string)attributes["source"]; }
            private set { attributes["source"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[bundle_registration_id]=desc`). Valid fields are `bundle_registration_id` and `created_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        ///   bundle_id - int64 - Bundle ID
        ///   bundle_registration_id - int64 - BundleRegistration ID
        /// </summary>
        public static FilesList<BundleAction> List(

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
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
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
            if (parameters.ContainsKey("bundle_registration_id") && !(parameters["bundle_registration_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: bundle_registration_id must be of type Nullable<Int64>", "parameters[\"bundle_registration_id\"]");
            }

            return new FilesList<BundleAction>($"/bundle_actions", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<BundleAction> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}