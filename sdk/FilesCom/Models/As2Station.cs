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
    /// <summary>
    /// As2StationEntity model
    /// </summary>
    public class As2Station : IModel, IJsonOnDeserialized
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        // The names of the attributes set on this object, by its constructor or its properties, since it was made
        // or last came from Files.com. SaveAsync sends these, never the defaults of attributes nobody set.
        private readonly HashSet<string> unsaved = new HashSet<string>();
        /// <summary>
        /// Makes a As2Station with no attributes set and no options.
        /// </summary>
        public As2Station() : this(null, null) { }

        /// <summary>
        /// Makes a As2Station with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public As2Station(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("uri"))
            {
                this.attributes.Add("uri", null);
            }
            if (!this.attributes.ContainsKey("domain"))
            {
                this.attributes.Add("domain", null);
            }
            if (!this.attributes.ContainsKey("hex_public_certificate_serial"))
            {
                this.attributes.Add("hex_public_certificate_serial", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_md5"))
            {
                this.attributes.Add("public_certificate_md5", null);
            }
            if (!this.attributes.ContainsKey("public_certificate"))
            {
                this.attributes.Add("public_certificate", null);
            }
            if (!this.attributes.ContainsKey("private_key_md5"))
            {
                this.attributes.Add("private_key_md5", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_subject"))
            {
                this.attributes.Add("public_certificate_subject", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_issuer"))
            {
                this.attributes.Add("public_certificate_issuer", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_serial"))
            {
                this.attributes.Add("public_certificate_serial", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_not_before"))
            {
                this.attributes.Add("public_certificate_not_before", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_not_after"))
            {
                this.attributes.Add("public_certificate_not_after", null);
            }
            if (!this.attributes.ContainsKey("private_key_password_md5"))
            {
                this.attributes.Add("private_key_password_md5", null);
            }
            if (!this.attributes.ContainsKey("private_key"))
            {
                this.attributes.Add("private_key", null);
            }
            if (!this.attributes.ContainsKey("private_key_password"))
            {
                this.attributes.Add("private_key_password", null);
            }
            if (!this.attributes.ContainsKey("pkcs12"))
            {
                this.attributes.Add("pkcs12", null);
            }
            if (!this.attributes.ContainsKey("pkcs12_password"))
            {
                this.attributes.Add("pkcs12_password", null);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.client = client;
            this.origin = origin;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => origin;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[0]; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }

        // What is read from JSON, such as a response from Files.com, is not waiting to be saved.
        void IJsonOnDeserialized.OnDeserialized()
        {
            unsaved.Clear();
        }


        /// <summary>
        /// Id of the AS2 Station.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// ID of the Workspace associated with this AS2 Station.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// The station's formal AS2 name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Public URI for sending AS2 message to.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri
        {
            get { return (string)attributes["uri"]; }
            set { attributes["uri"] = value; unsaved.Add("uri"); }
        }

        /// <summary>
        /// The station's AS2 domain name.
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain
        {
            get { return (string)attributes["domain"]; }
            set { attributes["domain"] = value; unsaved.Add("domain"); }
        }

        /// <summary>
        /// Serial of public certificate used for message security in hex format.
        /// </summary>
        [JsonPropertyName("hex_public_certificate_serial")]
        public string HexPublicCertificateSerial
        {
            get { return (string)attributes["hex_public_certificate_serial"]; }
            set { attributes["hex_public_certificate_serial"] = value; unsaved.Add("hex_public_certificate_serial"); }
        }

        /// <summary>
        /// MD5 hash of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_md5")]
        public string PublicCertificateMd5
        {
            get { return (string)attributes["public_certificate_md5"]; }
            set { attributes["public_certificate_md5"] = value; unsaved.Add("public_certificate_md5"); }
        }

        /// <summary>
        /// Public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate")]
        public string PublicCertificate
        {
            get { return (string)attributes["public_certificate"]; }
            set { attributes["public_certificate"] = value; unsaved.Add("public_certificate"); }
        }

        /// <summary>
        /// MD5 hash of private key used for message security.
        /// </summary>
        [JsonPropertyName("private_key_md5")]
        public string PrivateKeyMd5
        {
            get { return (string)attributes["private_key_md5"]; }
            set { attributes["private_key_md5"] = value; unsaved.Add("private_key_md5"); }
        }

        /// <summary>
        /// Subject of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_subject")]
        public string PublicCertificateSubject
        {
            get { return (string)attributes["public_certificate_subject"]; }
            set { attributes["public_certificate_subject"] = value; unsaved.Add("public_certificate_subject"); }
        }

        /// <summary>
        /// Issuer of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_issuer")]
        public string PublicCertificateIssuer
        {
            get { return (string)attributes["public_certificate_issuer"]; }
            set { attributes["public_certificate_issuer"] = value; unsaved.Add("public_certificate_issuer"); }
        }

        /// <summary>
        /// Serial of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_serial")]
        public string PublicCertificateSerial
        {
            get { return (string)attributes["public_certificate_serial"]; }
            set { attributes["public_certificate_serial"] = value; unsaved.Add("public_certificate_serial"); }
        }

        /// <summary>
        /// Not before value of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_not_before")]
        public string PublicCertificateNotBefore
        {
            get { return (string)attributes["public_certificate_not_before"]; }
            set { attributes["public_certificate_not_before"] = value; unsaved.Add("public_certificate_not_before"); }
        }

        /// <summary>
        /// Not after value of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_not_after")]
        public string PublicCertificateNotAfter
        {
            get { return (string)attributes["public_certificate_not_after"]; }
            set { attributes["public_certificate_not_after"] = value; unsaved.Add("public_certificate_not_after"); }
        }

        /// <summary>
        /// MD5 hash of private key password used for message security.
        /// </summary>
        [JsonPropertyName("private_key_password_md5")]
        public string PrivateKeyPasswordMd5
        {
            get { return (string)attributes["private_key_password_md5"]; }
            set { attributes["private_key_password_md5"] = value; unsaved.Add("private_key_password_md5"); }
        }

        /// <summary>
        /// PEM-encoded private key matching public_certificate.
        /// </summary>
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string)attributes["private_key"]; }
            set { attributes["private_key"] = value; unsaved.Add("private_key"); }
        }

        /// <summary>
        /// Password for the PEM-encoded private key.
        /// </summary>
        [JsonPropertyName("private_key_password")]
        public string PrivateKeyPassword
        {
            get { return (string)attributes["private_key_password"]; }
            set { attributes["private_key_password"] = value; unsaved.Add("private_key_password"); }
        }

        /// <summary>
        /// Base64-encoded PKCS#12 (.pfx or .p12) file containing the identity's certificate and private key. Provide this instead of public_certificate and private_key.
        /// </summary>
        [JsonPropertyName("pkcs12")]
        public string Pkcs12
        {
            get { return (string)attributes["pkcs12"]; }
            set { attributes["pkcs12"] = value; unsaved.Add("pkcs12"); }
        }

        /// <summary>
        /// Password for pkcs12. The file and password are used only for import; the extracted certificate and private key are stored as PEM.
        /// </summary>
        [JsonPropertyName("pkcs12_password")]
        public string Pkcs12Password
        {
            get { return (string)attributes["pkcs12_password"]; }
            set { attributes["pkcs12_password"] = value; unsaved.Add("pkcs12_password"); }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - The station's formal AS2 name.
        ///   public_certificate - string - Public certificate used for message security.
        ///   private_key - string - PEM-encoded private key matching public_certificate.
        ///   private_key_password - string - Password for the PEM-encoded private key.
        ///   pkcs12 - string - Base64-encoded PKCS#12 (.pfx or .p12) file containing the identity's replacement certificate and private key. Provide this instead of public_certificate and private_key.
        ///   pkcs12_password - string - Password for pkcs12. The file and password are used only for import; the extracted certificate and private key are stored as PEM.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The As2Station it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<As2Station> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - The station's formal AS2 name.
        ///   public_certificate - string - Public certificate used for message security.
        ///   private_key - string - PEM-encoded private key matching public_certificate.
        ///   private_key_password - string - Password for the PEM-encoded private key.
        ///   pkcs12 - string - Base64-encoded PKCS#12 (.pfx or .p12) file containing the identity's replacement certificate and private key. Provide this instead of public_certificate and private_key.
        ///   pkcs12_password - string - Password for pkcs12. The file and password are used only for import; the extracted certificate and private key are stored as PEM.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The As2Station it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<As2Station> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<As2Station> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The As2Station has no id, which Update needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("public_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_certificate is null, which cannot be sent" : "Bad parameter: public_certificate must be of type string", "public_certificate");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_password is null, which cannot be sent" : "Bad parameter: private_key_password must be of type string", "private_key_password");
            }
            if (parameters.TryGetValue("pkcs12", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12 is null, which cannot be sent" : "Bad parameter: pkcs12 must be of type string", "pkcs12");
            }
            if (parameters.TryGetValue("pkcs12_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12_password is null, which cannot be sent" : "Bad parameter: pkcs12_password must be of type string", "pkcs12_password");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_stations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            As2Station updated = JsonUtil.DeserializeModelResponse<As2Station>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<As2Station>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /as2_stations/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /as2_stations/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>, but neither waits for the request nor reports its failure.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        [Obsolete("Destroy neither waits for the request nor reports its failure. Use DestroyAsync instead.")]
        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The As2Station has no id, which Delete needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/as2_stations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "name", "workspace_id", "public_certificate", "private_key", "private_key_password", "pkcs12", "pkcs12_password" };
        private static readonly string[] UpdateParameterNames = { "id", "name", "public_certificate", "private_key", "private_key_password", "pkcs12", "pkcs12_password" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this As2Station on Files.com, or updates it if it has an ID.
        /// It sends the attributes set on this object, in its constructor or with its properties, or changed in place,
        /// such as an element of an array, since it was made or last came from Files.com, of those that operation
        /// takes; it sends no other attributes, and no defaults. Each goes in the form its parameter takes, such as a
        /// DateTime as ISO 8601 text. When it succeeds, this object takes the attributes Files.com returns.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                await UpdateCore(AttributesToSave(UpdateParameterNames), cancellationToken);
                return;
            }
            As2Station created = await As2Station.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
            Refresh(created.getAttributes());
            origin = ((IModel)created).Origin;
        }

        // The attributes set or changed on this object since it was made or last came from Files.com, of those named
        // in parameterNames: the parameters of the operation that saves it.
        private Dictionary<string, object> AttributesToSave(string[] parameterNames)
        {
            Dictionary<string, object> toSave = new Dictionary<string, object>();
            foreach (string name in parameterNames)
            {
                if (unsaved.Contains(name))
                {
                    toSave[name] = attributes[name];
                }
            }
            return toSave;
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<As2Station> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<As2Station> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<As2Station> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }

            return new FilesList<As2Station>(client, $"/as2_stations", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - As2 Station ID.
        /// </summary>
        /// <param name="id">As2 Station ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2Station> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - As2 Station ID.
        /// </summary>
        /// <param name="id">As2 Station ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2Station> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<As2Station> FindCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_stations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<As2Station>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name (required) - string - The station's formal AS2 name.
        ///   workspace_id - int64 - ID of the Workspace associated with this AS2 Station.
        ///   public_certificate - string - Public certificate used for message security.
        ///   private_key - string - PEM-encoded private key matching public_certificate.
        ///   private_key_password - string - Password for the PEM-encoded private key.
        ///   pkcs12 - string - Base64-encoded PKCS#12 (.pfx or .p12) file containing the identity's certificate and private key. Provide this instead of public_certificate and private_key.
        ///   pkcs12_password - string - Password for pkcs12. The file and password are used only for import; the extracted certificate and private key are stored as PEM.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2Station> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<As2Station> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("name", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: name", "name");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("public_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_certificate is null, which cannot be sent" : "Bad parameter: public_certificate must be of type string", "public_certificate");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_password is null, which cannot be sent" : "Bad parameter: private_key_password must be of type string", "private_key_password");
            }
            if (parameters.TryGetValue("pkcs12", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12 is null, which cannot be sent" : "Bad parameter: pkcs12 must be of type string", "pkcs12");
            }
            if (parameters.TryGetValue("pkcs12_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12_password is null, which cannot be sent" : "Bad parameter: pkcs12_password must be of type string", "pkcs12_password");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_stations", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<As2Station>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - The station's formal AS2 name.
        ///   public_certificate - string - Public certificate used for message security.
        ///   private_key - string - PEM-encoded private key matching public_certificate.
        ///   private_key_password - string - Password for the PEM-encoded private key.
        ///   pkcs12 - string - Base64-encoded PKCS#12 (.pfx or .p12) file containing the identity's replacement certificate and private key. Provide this instead of public_certificate and private_key.
        ///   pkcs12_password - string - Password for pkcs12. The file and password are used only for import; the extracted certificate and private key are stored as PEM.
        /// </summary>
        /// <param name="id">As2 Station ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2Station> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<As2Station> UpdateCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("public_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_certificate is null, which cannot be sent" : "Bad parameter: public_certificate must be of type string", "public_certificate");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_password is null, which cannot be sent" : "Bad parameter: private_key_password must be of type string", "private_key_password");
            }
            if (parameters.TryGetValue("pkcs12", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12 is null, which cannot be sent" : "Bad parameter: pkcs12 must be of type string", "pkcs12");
            }
            if (parameters.TryGetValue("pkcs12_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12_password is null, which cannot be sent" : "Bad parameter: pkcs12_password must be of type string", "pkcs12_password");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_stations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<As2Station>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /as2_stations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">As2 Station ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /as2_stations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">As2 Station ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
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
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/as2_stations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}