using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PartnerSiteRequest
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PartnerSiteRequest() : this(null, null) { }

        public PartnerSiteRequest(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("partner_id"))
            {
                this.attributes.Add("partner_id", null);
            }
            if (!this.attributes.ContainsKey("linked_site_id"))
            {
                this.attributes.Add("linked_site_id", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("pairing_key"))
            {
                this.attributes.Add("pairing_key", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
            }
            if (!this.attributes.ContainsKey("site_url"))
            {
                this.attributes.Add("site_url", null);
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
        /// Partner Site Request ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Partner ID
        /// </summary>
        [JsonPropertyName("partner_id")]
        public Nullable<Int64> PartnerId
        {
            get { return (Nullable<Int64>)attributes["partner_id"]; }
            set { attributes["partner_id"] = value; }
        }

        /// <summary>
        /// Linked Site ID
        /// </summary>
        [JsonPropertyName("linked_site_id")]
        public Nullable<Int64> LinkedSiteId
        {
            get { return (Nullable<Int64>)attributes["linked_site_id"]; }
            set { attributes["linked_site_id"] = value; }
        }

        /// <summary>
        /// Request status (pending, approved, rejected)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; }
        }

        /// <summary>
        /// Pairing key used to approve this request on the target site
        /// </summary>
        [JsonPropertyName("pairing_key")]
        public string PairingKey
        {
            get { return (string)attributes["pairing_key"]; }
            set { attributes["pairing_key"] = value; }
        }

        /// <summary>
        /// Request creation date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Request last updated date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>)attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }

        /// <summary>
        /// Site URL to link to
        /// </summary>
        [JsonPropertyName("site_url")]
        public string SiteUrl
        {
            get { return (string)attributes["site_url"]; }
            set { attributes["site_url"] = value; }
        }

        /// <summary>
        /// Reject partner site request
        /// </summary>
        public async Task Reject(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/partner_site_requests/{System.Uri.EscapeDataString(attributes["id"].ToString())}/reject", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Approve partner site request
        /// </summary>
        public async Task Approve(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/partner_site_requests/{System.Uri.EscapeDataString(attributes["id"].ToString())}/approve", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/partner_site_requests/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The PartnerSiteRequest object doesn't support updates.");
            }
            else
            {
                var newObj = await PartnerSiteRequest.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<PartnerSiteRequest> List(

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

            return new FilesList<PartnerSiteRequest>($"/partner_site_requests", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<PartnerSiteRequest> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   pairing_key (required) - string - Pairing key for the partner site request
        /// </summary>
        public static async Task FindByPairingKey(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("pairing_key") || parameters["pairing_key"] == null)
            {
                throw new ArgumentNullException("Parameter missing: pairing_key", "parameters[\"pairing_key\"]");
            }
            if (parameters.ContainsKey("pairing_key") && !(parameters["pairing_key"] is string))
            {
                throw new ArgumentException("Bad parameter: pairing_key must be of type string", "parameters[\"pairing_key\"]");
            }

            await FilesClient.SendRequest($"/partner_site_requests/find_by_pairing_key", System.Net.Http.HttpMethod.Get, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   partner_id (required) - int64 - Partner ID to link with
        ///   site_url (required) - string - Site URL to link to
        /// </summary>
        public static async Task<PartnerSiteRequest> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("partner_id") || parameters["partner_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: partner_id", "parameters[\"partner_id\"]");
            }
            if (!parameters.ContainsKey("site_url") || parameters["site_url"] == null)
            {
                throw new ArgumentNullException("Parameter missing: site_url", "parameters[\"site_url\"]");
            }
            if (parameters.ContainsKey("partner_id") && !(parameters["partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: partner_id must be of type Nullable<Int64>", "parameters[\"partner_id\"]");
            }
            if (parameters.ContainsKey("site_url") && !(parameters["site_url"] is string))
            {
                throw new ArgumentException("Bad parameter: site_url must be of type string", "parameters[\"site_url\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partner_site_requests", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<PartnerSiteRequest>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Reject partner site request
        /// </summary>
        public static async Task Reject(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/partner_site_requests/{System.Uri.EscapeDataString(parameters["id"].ToString())}/reject", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Approve partner site request
        /// </summary>
        public static async Task Approve(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/partner_site_requests/{System.Uri.EscapeDataString(parameters["id"].ToString())}/approve", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// </summary>
        public static async Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/partner_site_requests/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(id, parameters, options);
        }

    }
}