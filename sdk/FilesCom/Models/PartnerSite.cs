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

            if (!this.attributes.ContainsKey("host_partner_id"))
            {
                this.attributes.Add("host_partner_id", null);
            }
            if (!this.attributes.ContainsKey("host_partner_name"))
            {
                this.attributes.Add("host_partner_name", null);
            }
            if (!this.attributes.ContainsKey("guest_partner_id"))
            {
                this.attributes.Add("guest_partner_id", null);
            }
            if (!this.attributes.ContainsKey("guest_partner_name"))
            {
                this.attributes.Add("guest_partner_name", null);
            }
            if (!this.attributes.ContainsKey("host_site_id"))
            {
                this.attributes.Add("host_site_id", null);
            }
            if (!this.attributes.ContainsKey("host_site_name"))
            {
                this.attributes.Add("host_site_name", null);
            }
            if (!this.attributes.ContainsKey("guest_site_id"))
            {
                this.attributes.Add("guest_site_id", null);
            }
            if (!this.attributes.ContainsKey("guest_site_name"))
            {
                this.attributes.Add("guest_site_name", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
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
        /// Host Partner ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("host_partner_id")]
        public Nullable<Int64> HostPartnerId
        {
            get { return (Nullable<Int64>)attributes["host_partner_id"]; }
            private set { attributes["host_partner_id"] = value; }
        }

        /// <summary>
        /// Host Partner Name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("host_partner_name")]
        public string HostPartnerName
        {
            get { return (string)attributes["host_partner_name"]; }
            private set { attributes["host_partner_name"] = value; }
        }

        /// <summary>
        /// Guest Partner ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("guest_partner_id")]
        public Nullable<Int64> GuestPartnerId
        {
            get { return (Nullable<Int64>)attributes["guest_partner_id"]; }
            private set { attributes["guest_partner_id"] = value; }
        }

        /// <summary>
        /// Guest Partner Name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("guest_partner_name")]
        public string GuestPartnerName
        {
            get { return (string)attributes["guest_partner_name"]; }
            private set { attributes["guest_partner_name"] = value; }
        }

        /// <summary>
        /// Host Site ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("host_site_id")]
        public Nullable<Int64> HostSiteId
        {
            get { return (Nullable<Int64>)attributes["host_site_id"]; }
            private set { attributes["host_site_id"] = value; }
        }

        /// <summary>
        /// Host Site Name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("host_site_name")]
        public string HostSiteName
        {
            get { return (string)attributes["host_site_name"]; }
            private set { attributes["host_site_name"] = value; }
        }

        /// <summary>
        /// Guest Site ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("guest_site_id")]
        public Nullable<Int64> GuestSiteId
        {
            get { return (Nullable<Int64>)attributes["guest_site_id"]; }
            private set { attributes["guest_site_id"] = value; }
        }

        /// <summary>
        /// Guest Site Name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("guest_site_name")]
        public string GuestSiteName
        {
            get { return (string)attributes["guest_site_name"]; }
            private set { attributes["guest_site_name"] = value; }
        }

        /// <summary>
        /// Workspace ID for the Host Partner
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            private set { attributes["workspace_id"] = value; }
        }



        /// <summary>
        /// </summary>
        public static async Task<PartnerSite[]> Linkeds(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendStringRequest($"/partner_sites/linked_partner_sites", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<PartnerSite[]>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
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