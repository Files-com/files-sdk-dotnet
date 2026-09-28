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
    public class SftpHostKey : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        public SftpHostKey() : this(null, null) { }

        public SftpHostKey(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("active"))
            {
                this.attributes.Add("active", false);
            }
            if (!this.attributes.ContainsKey("custom_domain_id"))
            {
                this.attributes.Add("custom_domain_id", null);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("key_type"))
            {
                this.attributes.Add("key_type", null);
            }
            if (!this.attributes.ContainsKey("fingerprint_md5"))
            {
                this.attributes.Add("fingerprint_md5", null);
            }
            if (!this.attributes.ContainsKey("fingerprint_sha256"))
            {
                this.attributes.Add("fingerprint_sha256", null);
            }
            if (!this.attributes.ContainsKey("private_key"))
            {
                this.attributes.Add("private_key", null);
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
        /// If true, use this SFTP Host Key.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("active")]
        public bool Active
        {
            get { return attributes["active"] == null ? false : (bool)attributes["active"]; }
            set { attributes["active"] = value; }
        }

        /// <summary>
        /// Custom Domain ID. If set, this key is used only for that Custom Domain.
        /// </summary>
        [JsonPropertyName("custom_domain_id")]
        public Nullable<Int64> CustomDomainId
        {
            get { return (Nullable<Int64>)attributes["custom_domain_id"]; }
            set { attributes["custom_domain_id"] = value; }
        }

        /// <summary>
        /// SFTP Host Key ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// The friendly name of this SFTP Host Key.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// SSH key type
        /// </summary>
        [JsonPropertyName("key_type")]
        public string KeyType
        {
            get { return (string)attributes["key_type"]; }
            set { attributes["key_type"] = value; }
        }

        /// <summary>
        /// MD5 Fingerprint of the public key
        /// </summary>
        [JsonPropertyName("fingerprint_md5")]
        public string FingerprintMd5
        {
            get { return (string)attributes["fingerprint_md5"]; }
            set { attributes["fingerprint_md5"] = value; }
        }

        /// <summary>
        /// SHA256 Fingerprint of the public key
        /// </summary>
        [JsonPropertyName("fingerprint_sha256")]
        public string FingerprintSha256
        {
            get { return (string)attributes["fingerprint_sha256"]; }
            set { attributes["fingerprint_sha256"] = value; }
        }

        /// <summary>
        /// The private key data.
        /// </summary>
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string)attributes["private_key"]; }
            set { attributes["private_key"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   active - boolean - If true, use this SFTP Host Key.
        ///   custom_domain_id - int64 - Custom Domain ID. If set, this key is used only for that Custom Domain.
        ///   name - string - The friendly name of this SFTP Host Key.
        ///   private_key - string - The private key data.
        /// </summary>
        public Task<SftpHostKey> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   active - boolean - If true, use this SFTP Host Key.
        ///   custom_domain_id - int64 - Custom Domain ID. If set, this key is used only for that Custom Domain.
        ///   name - string - The friendly name of this SFTP Host Key.
        ///   private_key - string - The private key data.
        /// </summary>
        public Task<SftpHostKey> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<SftpHostKey> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
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
            if (parameters.ContainsKey("active") && !(parameters["active"] is bool))
            {
                throw new ArgumentException("Bad parameter: active must be of type bool", "parameters[\"active\"]");
            }
            if (parameters.ContainsKey("custom_domain_id") && !(parameters["custom_domain_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: custom_domain_id must be of type Nullable<Int64>", "parameters[\"custom_domain_id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/sftp_host_keys/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<SftpHostKey>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/sftp_host_keys/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
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
                await UpdateCore(this.attributes, cancellationToken);
            }
            else
            {
                var newObj = await SftpHostKey.CreateCore(new OperationContext(FilesClient.Bind(ref client)), this.attributes, DictionaryUtil.Copy(this.options), cancellationToken);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<SftpHostKey> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        public static FilesList<SftpHostKey> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<SftpHostKey> ListCore(
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

            return new FilesList<SftpHostKey>(client, $"/sftp_host_keys", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sftp Host Key ID.
        /// </summary>
        public static Task<SftpHostKey> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        public static Task<SftpHostKey> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<SftpHostKey> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/sftp_host_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<SftpHostKey>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   active - boolean - If true, use this SFTP Host Key.
        ///   custom_domain_id - int64 - Custom Domain ID. If set, this key is used only for that Custom Domain.
        ///   name - string - The friendly name of this SFTP Host Key.
        ///   private_key - string - The private key data.
        /// </summary>
        public static Task<SftpHostKey> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<SftpHostKey> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("active") && !(parameters["active"] is bool))
            {
                throw new ArgumentException("Bad parameter: active must be of type bool", "parameters[\"active\"]");
            }
            if (parameters.ContainsKey("custom_domain_id") && !(parameters["custom_domain_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: custom_domain_id must be of type Nullable<Int64>", "parameters[\"custom_domain_id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/sftp_host_keys", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<SftpHostKey>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   active - boolean - If true, use this SFTP Host Key.
        ///   custom_domain_id - int64 - Custom Domain ID. If set, this key is used only for that Custom Domain.
        ///   name - string - The friendly name of this SFTP Host Key.
        ///   private_key - string - The private key data.
        /// </summary>
        public static Task<SftpHostKey> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<SftpHostKey> UpdateCore(
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
            if (parameters.ContainsKey("active") && !(parameters["active"] is bool))
            {
                throw new ArgumentException("Bad parameter: active must be of type bool", "parameters[\"active\"]");
            }
            if (parameters.ContainsKey("custom_domain_id") && !(parameters["custom_domain_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: custom_domain_id must be of type Nullable<Int64>", "parameters[\"custom_domain_id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/sftp_host_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<SftpHostKey>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/sftp_host_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
            response.Dispose();
        }

    }
}