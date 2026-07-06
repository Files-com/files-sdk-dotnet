using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class CustomDomain : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public CustomDomain() : this(null, null) { }

        public CustomDomain(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("domain"))
            {
                this.attributes.Add("domain", null);
            }
            if (!this.attributes.ContainsKey("destination"))
            {
                this.attributes.Add("destination", null);
            }
            if (!this.attributes.ContainsKey("dns_status"))
            {
                this.attributes.Add("dns_status", null);
            }
            if (!this.attributes.ContainsKey("ssl_certificate_id"))
            {
                this.attributes.Add("ssl_certificate_id", null);
            }
            if (!this.attributes.ContainsKey("brick_managed"))
            {
                this.attributes.Add("brick_managed", false);
            }
            if (!this.attributes.ContainsKey("folder_behavior_id"))
            {
                this.attributes.Add("folder_behavior_id", null);
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

        void IModel.SetOptions(Dictionary<string, object> options)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// Custom Domain ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Customer-owned domain name.
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain
        {
            get { return (string)attributes["domain"]; }
            set { attributes["domain"] = value; }
        }

        /// <summary>
        /// Where this custom domain routes. Can be `site_alias`, `public_hosting`, or `s3_endpoint`.
        /// </summary>
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string)attributes["destination"]; }
            set { attributes["destination"] = value; }
        }

        /// <summary>
        /// Current DNS verification status.
        /// </summary>
        [JsonPropertyName("dns_status")]
        public string DnsStatus
        {
            get { return (string)attributes["dns_status"]; }
            set { attributes["dns_status"] = value; }
        }

        /// <summary>
        /// Current SSL certificate ID.
        /// </summary>
        [JsonPropertyName("ssl_certificate_id")]
        public Nullable<Int64> SslCertificateId
        {
            get { return (Nullable<Int64>)attributes["ssl_certificate_id"]; }
            set { attributes["ssl_certificate_id"] = value; }
        }

        /// <summary>
        /// Is this domain's SSL certificate automatically managed and renewed by Files.com?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("brick_managed")]
        public bool BrickManaged
        {
            get { return attributes["brick_managed"] == null ? false : (bool)attributes["brick_managed"]; }
            set { attributes["brick_managed"] = value; }
        }

        /// <summary>
        /// Public Hosting behavior ID when this domain routes to a specific Public Hosting behavior.
        /// </summary>
        [JsonPropertyName("folder_behavior_id")]
        public Nullable<Int64> FolderBehaviorId
        {
            get { return (Nullable<Int64>)attributes["folder_behavior_id"]; }
            set { attributes["folder_behavior_id"] = value; }
        }

        /// <summary>
        /// When this Custom Domain was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// When this Custom Domain was last updated.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>)attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   destination - string - Where this custom domain routes. Can be `site_alias`, `public_hosting`, or `s3_endpoint`.
        ///   folder_behavior_id - int64 - Public Hosting behavior ID when this domain routes to a specific Public Hosting behavior.
        ///   ssl_certificate_id - int64 - Current SSL certificate ID.
        ///   domain - string - Customer-owned domain name.
        /// </summary>
        public async Task<CustomDomain> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("folder_behavior_id") && !(parameters["folder_behavior_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: folder_behavior_id must be of type Nullable<Int64>", "parameters[\"folder_behavior_id\"]");
            }
            if (parameters.ContainsKey("ssl_certificate_id") && !(parameters["ssl_certificate_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: ssl_certificate_id must be of type Nullable<Int64>", "parameters[\"ssl_certificate_id\"]");
            }
            if (parameters.ContainsKey("domain") && !(parameters["domain"] is string))
            {
                throw new ArgumentException("Bad parameter: domain must be of type string", "parameters[\"domain\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/custom_domains/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<CustomDomain>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            await FilesClient.SendRequest($"/custom_domains/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                await this.Update(this.attributes);
            }
            else
            {
                var newObj = await CustomDomain.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `id`.
        /// </summary>
        public static FilesList<CustomDomain> List(

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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }

            return new FilesList<CustomDomain>($"/custom_domains", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<CustomDomain> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Custom Domain ID.
        /// </summary>
        public static async Task<CustomDomain> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/custom_domains/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<CustomDomain>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<CustomDomain> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   destination - string - Where this custom domain routes. Can be `site_alias`, `public_hosting`, or `s3_endpoint`.
        ///   folder_behavior_id - int64 - Public Hosting behavior ID when this domain routes to a specific Public Hosting behavior.
        ///   ssl_certificate_id - int64 - Current SSL certificate ID.
        ///   domain (required) - string - Customer-owned domain name.
        /// </summary>
        public static async Task<CustomDomain> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("domain") || parameters["domain"] == null)
            {
                throw new ArgumentNullException("Parameter missing: domain", "parameters[\"domain\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("folder_behavior_id") && !(parameters["folder_behavior_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: folder_behavior_id must be of type Nullable<Int64>", "parameters[\"folder_behavior_id\"]");
            }
            if (parameters.ContainsKey("ssl_certificate_id") && !(parameters["ssl_certificate_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: ssl_certificate_id must be of type Nullable<Int64>", "parameters[\"ssl_certificate_id\"]");
            }
            if (parameters.ContainsKey("domain") && !(parameters["domain"] is string))
            {
                throw new ArgumentException("Bad parameter: domain must be of type string", "parameters[\"domain\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/custom_domains", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<CustomDomain>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   destination - string - Where this custom domain routes. Can be `site_alias`, `public_hosting`, or `s3_endpoint`.
        ///   folder_behavior_id - int64 - Public Hosting behavior ID when this domain routes to a specific Public Hosting behavior.
        ///   ssl_certificate_id - int64 - Current SSL certificate ID.
        ///   domain - string - Customer-owned domain name.
        /// </summary>
        public static async Task<CustomDomain> Update(
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
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("folder_behavior_id") && !(parameters["folder_behavior_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: folder_behavior_id must be of type Nullable<Int64>", "parameters[\"folder_behavior_id\"]");
            }
            if (parameters.ContainsKey("ssl_certificate_id") && !(parameters["ssl_certificate_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: ssl_certificate_id must be of type Nullable<Int64>", "parameters[\"ssl_certificate_id\"]");
            }
            if (parameters.ContainsKey("domain") && !(parameters["domain"] is string))
            {
                throw new ArgumentException("Bad parameter: domain must be of type string", "parameters[\"domain\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/custom_domains/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<CustomDomain>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            await FilesClient.SendRequest($"/custom_domains/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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