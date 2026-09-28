using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PartnerSiteRequest : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
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
            if (!this.attributes.ContainsKey("host_partner_id"))
            {
                this.attributes.Add("host_partner_id", null);
            }
            if (!this.attributes.ContainsKey("guest_site_url"))
            {
                this.attributes.Add("guest_site_url", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("host_site_name"))
            {
                this.attributes.Add("host_site_name", null);
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
        }

        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        public object GetOption(string name)
        {
            return (this.options.ContainsKey(name) ? this.options[name] : null);
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options)
        {
            this.client = client;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[0]; }
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
        /// Host Partner ID
        /// </summary>
        [JsonPropertyName("host_partner_id")]
        public Nullable<Int64> HostPartnerId
        {
            get { return (Nullable<Int64>)attributes["host_partner_id"]; }
            set { attributes["host_partner_id"] = value; }
        }

        /// <summary>
        /// Guest Site URL
        /// </summary>
        [JsonPropertyName("guest_site_url")]
        public string GuestSiteUrl
        {
            get { return (string)attributes["guest_site_url"]; }
            set { attributes["guest_site_url"] = value; }
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
        /// Host Site Name
        /// </summary>
        [JsonPropertyName("host_site_name")]
        public string HostSiteName
        {
            get { return (string)attributes["host_site_name"]; }
            set { attributes["host_site_name"] = value; }
        }

        /// <summary>
        /// Pairing key used to approve this request on the Guest Site
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
        /// </summary>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// </summary>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
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

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/partner_site_requests/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            response.Dispose();
        }


        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The PartnerSiteRequest object doesn't support updates.");
            }
            else
            {
                var newObj = await PartnerSiteRequest.CreateCore(new OperationContext(FilesClient.Bind(ref client)), this.attributes, DictionaryUtil.Copy(this.options), cancellationToken);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `host_partner_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `host_partner_id`.
        /// </summary>
        public static FilesList<PartnerSiteRequest> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        public static FilesList<PartnerSiteRequest> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<PartnerSiteRequest> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }

            return new FilesList<PartnerSiteRequest>(client, $"/partner_site_requests", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   pairing_key (required) - string - Pairing key for the partner site request
        /// </summary>
        public static Task FindByPairingKey(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindByPairingKeyCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task FindByPairingKeyCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
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

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/partner_site_requests/find_by_pairing_key", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);
            response.Dispose();
        }

        /// <summary>
        /// Parameters:
        ///   host_partner_id (required) - int64 - Host Partner ID to link with
        ///   guest_site_url (required) - string - Guest Site URL to link to
        /// </summary>
        public static Task<PartnerSiteRequest> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<PartnerSiteRequest> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("host_partner_id") || parameters["host_partner_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: host_partner_id", "parameters[\"host_partner_id\"]");
            }
            if (!parameters.ContainsKey("guest_site_url") || parameters["guest_site_url"] == null)
            {
                throw new ArgumentNullException("Parameter missing: guest_site_url", "parameters[\"guest_site_url\"]");
            }
            if (parameters.ContainsKey("host_partner_id") && !(parameters["host_partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: host_partner_id must be of type Nullable<Int64>", "parameters[\"host_partner_id\"]");
            }
            if (parameters.ContainsKey("guest_site_url") && !(parameters["guest_site_url"] is string))
            {
                throw new ArgumentException("Bad parameter: guest_site_url must be of type string", "parameters[\"guest_site_url\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/partner_site_requests", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<PartnerSiteRequest>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   pairing_key (required) - string - Pairing key for the partner site request
        /// </summary>
        public static Task Reject(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return RejectCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task RejectCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
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

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/partner_site_requests/reject", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
            response.Dispose();
        }

        /// <summary>
        /// Parameters:
        ///   pairing_key (required) - string - Pairing key for the partner site request
        ///   partner_id - int64 - ID of an existing Partner on this site, with the host role, that represents the requesting organization. The connection binds to that Partner and makes it host_and_guest. When omitted, a guest Partner named after the host site is created.
        /// </summary>
        public static Task Approve(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ApproveCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task ApproveCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
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
            if (parameters.ContainsKey("partner_id") && !(parameters["partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: partner_id must be of type Nullable<Int64>", "parameters[\"partner_id\"]");
            }

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/partner_site_requests/approve", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
            response.Dispose();
        }

        /// <summary>
        /// </summary>
        public static Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        public static Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(id, parameters, options);
        }

        internal static async Task DeleteCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
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

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/partner_site_requests/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
            response.Dispose();
        }

    }
}