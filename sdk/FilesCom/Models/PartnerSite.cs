using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PartnerSite
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PartnerSite() : this(null, null) { }

        public PartnerSite(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("partner_id"))
            {
                this.attributes.Add("partner_id", null);
            }
            if (!this.attributes.ContainsKey("partner_name"))
            {
                this.attributes.Add("partner_name", null);
            }
            if (!this.attributes.ContainsKey("linked_site_id"))
            {
                this.attributes.Add("linked_site_id", null);
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
        /// Partner ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("partner_id")]
        public Nullable<Int64> PartnerId
        {
            get { return (Nullable<Int64>)attributes["partner_id"]; }
            private set { attributes["partner_id"] = value; }
        }

        /// <summary>
        /// Partner Name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("partner_name")]
        public string PartnerName
        {
            get { return (string)attributes["partner_name"]; }
            private set { attributes["partner_name"] = value; }
        }

        /// <summary>
        /// Linked Site ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("linked_site_id")]
        public Nullable<Int64> LinkedSiteId
        {
            get { return (Nullable<Int64>)attributes["linked_site_id"]; }
            private set { attributes["linked_site_id"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<PartnerSite> List(

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

            return new FilesList<PartnerSite>($"/partner_sites", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<PartnerSite> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}