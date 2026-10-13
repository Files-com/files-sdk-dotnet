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
    /// As2PartnerEntity model
    /// </summary>
    public class As2Partner : IModel, IJsonOnDeserialized
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
        /// Makes a As2Partner with no attributes set and no options.
        /// </summary>
        public As2Partner() : this(null, null) { }

        /// <summary>
        /// Makes a As2Partner with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public As2Partner(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("as2_station_id"))
            {
                this.attributes.Add("as2_station_id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("uri"))
            {
                this.attributes.Add("uri", null);
            }
            if (!this.attributes.ContainsKey("server_certificate"))
            {
                this.attributes.Add("server_certificate", null);
            }
            if (!this.attributes.ContainsKey("http_auth_username"))
            {
                this.attributes.Add("http_auth_username", null);
            }
            if (!this.attributes.ContainsKey("additional_http_headers"))
            {
                this.attributes.Add("additional_http_headers", null);
            }
            if (!this.attributes.ContainsKey("default_mime_type"))
            {
                this.attributes.Add("default_mime_type", null);
            }
            if (!this.attributes.ContainsKey("mdn_validation_level"))
            {
                this.attributes.Add("mdn_validation_level", null);
            }
            if (!this.attributes.ContainsKey("signature_validation_level"))
            {
                this.attributes.Add("signature_validation_level", null);
            }
            if (!this.attributes.ContainsKey("enable_dedicated_ips"))
            {
                this.attributes.Add("enable_dedicated_ips", false);
            }
            if (!this.attributes.ContainsKey("hex_public_certificate_serial"))
            {
                this.attributes.Add("hex_public_certificate_serial", null);
            }
            if (!this.attributes.ContainsKey("public_certificate"))
            {
                this.attributes.Add("public_certificate", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_md5"))
            {
                this.attributes.Add("public_certificate_md5", null);
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
            if (!this.attributes.ContainsKey("http_auth_password"))
            {
                this.attributes.Add("http_auth_password", null);
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
        /// ID of the AS2 Partner.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// ID of the Workspace associated with this AS2 Partner.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// ID of the AS2 Station associated with this partner.
        /// </summary>
        [JsonPropertyName("as2_station_id")]
        public Nullable<Int64> As2StationId
        {
            get { return (Nullable<Int64>)attributes["as2_station_id"]; }
            set { attributes["as2_station_id"] = value; unsaved.Add("as2_station_id"); }
        }

        /// <summary>
        /// The partner's formal AS2 name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Public URI where we will send the AS2 messages (via HTTP/HTTPS).
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri
        {
            get { return (string)attributes["uri"]; }
            set { attributes["uri"] = value; unsaved.Add("uri"); }
        }

        /// <summary>
        /// Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS? (This only applies to Outgoing AS2 message from Files.com to a Partner.)
        /// </summary>
        [JsonPropertyName("server_certificate")]
        public string ServerCertificate
        {
            get { return (string)attributes["server_certificate"]; }
            set { attributes["server_certificate"] = value; unsaved.Add("server_certificate"); }
        }

        /// <summary>
        /// Username to send to server for HTTP Authentication.
        /// </summary>
        [JsonPropertyName("http_auth_username")]
        public string HttpAuthUsername
        {
            get { return (string)attributes["http_auth_username"]; }
            set { attributes["http_auth_username"] = value; unsaved.Add("http_auth_username"); }
        }

        /// <summary>
        /// Additional HTTP Headers for outgoing message sent to this partner.
        /// </summary>
        [JsonPropertyName("additional_http_headers")]
        public object AdditionalHttpHeaders
        {
            get { return (object)attributes["additional_http_headers"]; }
            set { attributes["additional_http_headers"] = value; unsaved.Add("additional_http_headers"); }
        }

        /// <summary>
        /// Default mime type of the file attached to the encrypted message
        /// </summary>
        [JsonPropertyName("default_mime_type")]
        public string DefaultMimeType
        {
            get { return (string)attributes["default_mime_type"]; }
            set { attributes["default_mime_type"] = value; unsaved.Add("default_mime_type"); }
        }

        /// <summary>
        /// How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates. `auto`: Automatically set the correct value for this setting based on next mdn received.
        /// </summary>
        [JsonPropertyName("mdn_validation_level")]
        public string MdnValidationLevel
        {
            get { return (string)attributes["mdn_validation_level"]; }
            set { attributes["mdn_validation_level"] = value; unsaved.Add("mdn_validation_level"); }
        }

        /// <summary>
        /// Should Files.com require signatures on incoming AS2 messages?  `normal`: require that incoming messages are signed with a valid matching signature. `none`: Unsigned incoming messages are allowed. `auto`: A one-time starting state. The next message received is always accepted and switches this to `normal` if it carries a valid signature, or `none` if it does not.
        /// </summary>
        [JsonPropertyName("signature_validation_level")]
        public string SignatureValidationLevel
        {
            get { return (string)attributes["signature_validation_level"]; }
            set { attributes["signature_validation_level"] = value; unsaved.Add("signature_validation_level"); }
        }

        /// <summary>
        /// If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("enable_dedicated_ips")]
        public bool EnableDedicatedIps
        {
            get { return attributes["enable_dedicated_ips"] == null ? false : (bool)attributes["enable_dedicated_ips"]; }
            set { attributes["enable_dedicated_ips"] = value; unsaved.Add("enable_dedicated_ips"); }
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
        /// Public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate")]
        public string PublicCertificate
        {
            get { return (string)attributes["public_certificate"]; }
            set { attributes["public_certificate"] = value; unsaved.Add("public_certificate"); }
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
        /// Password to send to server for HTTP Authentication.
        /// </summary>
        [JsonPropertyName("http_auth_password")]
        public string HttpAuthPassword
        {
            get { return (string)attributes["http_auth_password"]; }
            set { attributes["http_auth_password"] = value; unsaved.Add("http_auth_password"); }
        }

        /// <summary>
        /// Base64-encoded PKCS#12 (.pfx or .p12) file containing the partner's public certificate. Provide this instead of public_certificate. Any private key in the file is discarded. A public-only file must contain exactly one certificate.
        /// </summary>
        [JsonPropertyName("pkcs12")]
        public string Pkcs12
        {
            get { return (string)attributes["pkcs12"]; }
            set { attributes["pkcs12"] = value; unsaved.Add("pkcs12"); }
        }

        /// <summary>
        /// Password for pkcs12. The file and password are used only for import; the extracted public certificate is stored as PEM.
        /// </summary>
        [JsonPropertyName("pkcs12_password")]
        public string Pkcs12Password
        {
            get { return (string)attributes["pkcs12_password"]; }
            set { attributes["pkcs12_password"] = value; unsaved.Add("pkcs12_password"); }
        }

        /// <summary>
        /// Parameters:
        ///   enable_dedicated_ips - boolean - If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
        ///   http_auth_username - string - Username to send to server for HTTP Authentication.
        ///   http_auth_password - string - Password to send to server for HTTP Authentication.
        ///   mdn_validation_level - string - How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates. `auto`: Automatically set the correct value for this setting based on next mdn received.
        ///   signature_validation_level - string - Should Files.com require signatures on incoming AS2 messages?  `normal`: require that incoming messages are signed with a valid matching signature. `none`: Unsigned incoming messages are allowed. `auto`: A one-time starting state. The next message received is always accepted and switches this to `normal` if it carries a valid signature, or `none` if it does not.
        ///   server_certificate - string - Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS? (This only applies to Outgoing AS2 message from Files.com to a Partner.)
        ///   default_mime_type - string - Default mime type of the file attached to the encrypted message
        ///   additional_http_headers - object - Additional HTTP Headers for outgoing message sent to this partner.
        ///   name - string - The partner's formal AS2 name.
        ///   uri - string - Public URI where we will send the AS2 messages (via HTTP/HTTPS).
        ///   public_certificate - string - Public certificate for AS2 Partner.  Note: This is the certificate for AS2 message security, not a certificate used for HTTPS authentication.
        ///   pkcs12 - string - Base64-encoded PKCS#12 (.pfx or .p12) file containing the partner's public certificate. Provide this instead of public_certificate. Any private key in the file is discarded. A public-only file must contain exactly one certificate.
        ///   pkcs12_password - string - Password for pkcs12. The file and password are used only for import; the extracted public certificate is stored as PEM.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The As2Partner it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<As2Partner> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   enable_dedicated_ips - boolean - If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
        ///   http_auth_username - string - Username to send to server for HTTP Authentication.
        ///   http_auth_password - string - Password to send to server for HTTP Authentication.
        ///   mdn_validation_level - string - How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates. `auto`: Automatically set the correct value for this setting based on next mdn received.
        ///   signature_validation_level - string - Should Files.com require signatures on incoming AS2 messages?  `normal`: require that incoming messages are signed with a valid matching signature. `none`: Unsigned incoming messages are allowed. `auto`: A one-time starting state. The next message received is always accepted and switches this to `normal` if it carries a valid signature, or `none` if it does not.
        ///   server_certificate - string - Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS? (This only applies to Outgoing AS2 message from Files.com to a Partner.)
        ///   default_mime_type - string - Default mime type of the file attached to the encrypted message
        ///   additional_http_headers - object - Additional HTTP Headers for outgoing message sent to this partner.
        ///   name - string - The partner's formal AS2 name.
        ///   uri - string - Public URI where we will send the AS2 messages (via HTTP/HTTPS).
        ///   public_certificate - string - Public certificate for AS2 Partner.  Note: This is the certificate for AS2 message security, not a certificate used for HTTPS authentication.
        ///   pkcs12 - string - Base64-encoded PKCS#12 (.pfx or .p12) file containing the partner's public certificate. Provide this instead of public_certificate. Any private key in the file is discarded. A public-only file must contain exactly one certificate.
        ///   pkcs12_password - string - Password for pkcs12. The file and password are used only for import; the extracted public certificate is stored as PEM.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The As2Partner it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<As2Partner> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<As2Partner> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The As2Partner has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("enable_dedicated_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enable_dedicated_ips is null, which cannot be sent" : "Bad parameter: enable_dedicated_ips must be of type bool", "enable_dedicated_ips");
            }
            if (parameters.TryGetValue("http_auth_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: http_auth_username is null, which cannot be sent" : "Bad parameter: http_auth_username must be of type string", "http_auth_username");
            }
            if (parameters.TryGetValue("http_auth_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: http_auth_password is null, which cannot be sent" : "Bad parameter: http_auth_password must be of type string", "http_auth_password");
            }
            if (parameters.TryGetValue("mdn_validation_level", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mdn_validation_level is null, which cannot be sent" : "Bad parameter: mdn_validation_level must be of type string", "mdn_validation_level");
            }
            if (parameters.TryGetValue("signature_validation_level", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: signature_validation_level is null, which cannot be sent" : "Bad parameter: signature_validation_level must be of type string", "signature_validation_level");
            }
            if (parameters.TryGetValue("server_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_certificate is null, which cannot be sent" : "Bad parameter: server_certificate must be of type string", "server_certificate");
            }
            if (parameters.TryGetValue("default_mime_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: default_mime_type is null, which cannot be sent" : "Bad parameter: default_mime_type must be of type string", "default_mime_type");
            }
            if (parameters.TryGetValue("additional_http_headers", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: additional_http_headers is null, which cannot be sent" : "Bad parameter: additional_http_headers must be of type object", "additional_http_headers");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("uri", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: uri is null, which cannot be sent" : "Bad parameter: uri must be of type string", "uri");
            }
            if (parameters.TryGetValue("public_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_certificate is null, which cannot be sent" : "Bad parameter: public_certificate must be of type string", "public_certificate");
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
            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_partners/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            As2Partner updated = JsonUtil.DeserializeModelResponse<As2Partner>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<As2Partner>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /as2_partners/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /as2_partners/{id} to the Files.com API.
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
                throw new MissingParameterException("The As2Partner has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/as2_partners/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "enable_dedicated_ips", "http_auth_username", "http_auth_password", "mdn_validation_level", "signature_validation_level", "server_certificate", "default_mime_type", "additional_http_headers", "as2_station_id", "name", "uri", "public_certificate", "pkcs12", "pkcs12_password" };
        private static readonly string[] UpdateParameterNames = { "id", "enable_dedicated_ips", "http_auth_username", "http_auth_password", "mdn_validation_level", "signature_validation_level", "server_certificate", "default_mime_type", "additional_http_headers", "name", "uri", "public_certificate", "pkcs12", "pkcs12_password" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this As2Partner on Files.com, or updates it if it has an ID.
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
            As2Partner created = await As2Partner.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `as2_station_id` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `as2_station_id` and `workspace_id`. Valid field combinations are `[ workspace_id, as2_station_id ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<As2Partner> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `as2_station_id` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `as2_station_id` and `workspace_id`. Valid field combinations are `[ workspace_id, as2_station_id ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<As2Partner> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<As2Partner> ListCore(
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

            return new FilesList<As2Partner>(client, $"/as2_partners", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - As2 Partner ID.
        /// </summary>
        /// <param name="id">As2 Partner ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2Partner> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - As2 Partner ID.
        /// </summary>
        /// <param name="id">As2 Partner ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2Partner> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<As2Partner> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<As2Partner>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   enable_dedicated_ips - boolean - If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
        ///   http_auth_username - string - Username to send to server for HTTP Authentication.
        ///   http_auth_password - string - Password to send to server for HTTP Authentication.
        ///   mdn_validation_level - string - How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates. `auto`: Automatically set the correct value for this setting based on next mdn received.
        ///   signature_validation_level - string - Should Files.com require signatures on incoming AS2 messages?  `normal`: require that incoming messages are signed with a valid matching signature. `none`: Unsigned incoming messages are allowed. `auto`: A one-time starting state. The next message received is always accepted and switches this to `normal` if it carries a valid signature, or `none` if it does not.
        ///   server_certificate - string - Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS? (This only applies to Outgoing AS2 message from Files.com to a Partner.)
        ///   default_mime_type - string - Default mime type of the file attached to the encrypted message
        ///   additional_http_headers - object - Additional HTTP Headers for outgoing message sent to this partner.
        ///   as2_station_id (required) - int64 - ID of the AS2 Station associated with this partner.
        ///   name (required) - string - The partner's formal AS2 name.
        ///   uri (required) - string - Public URI where we will send the AS2 messages (via HTTP/HTTPS).
        ///   public_certificate - string - Public certificate for AS2 Partner.  Note: This is the certificate for AS2 message security, not a certificate used for HTTPS authentication.
        ///   pkcs12 - string - Base64-encoded PKCS#12 (.pfx or .p12) file containing the partner's public certificate. Provide this instead of public_certificate. Any private key in the file is discarded. A public-only file must contain exactly one certificate.
        ///   pkcs12_password - string - Password for pkcs12. The file and password are used only for import; the extracted public certificate is stored as PEM.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2Partner> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<As2Partner> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("as2_station_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: as2_station_id", "as2_station_id");
            }
            if (!parameters.TryGetValue("name", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: name", "name");
            }
            if (!parameters.TryGetValue("uri", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: uri", "uri");
            }
            if (parameters.TryGetValue("enable_dedicated_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enable_dedicated_ips is null, which cannot be sent" : "Bad parameter: enable_dedicated_ips must be of type bool", "enable_dedicated_ips");
            }
            if (parameters.TryGetValue("http_auth_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: http_auth_username is null, which cannot be sent" : "Bad parameter: http_auth_username must be of type string", "http_auth_username");
            }
            if (parameters.TryGetValue("http_auth_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: http_auth_password is null, which cannot be sent" : "Bad parameter: http_auth_password must be of type string", "http_auth_password");
            }
            if (parameters.TryGetValue("mdn_validation_level", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mdn_validation_level is null, which cannot be sent" : "Bad parameter: mdn_validation_level must be of type string", "mdn_validation_level");
            }
            if (parameters.TryGetValue("signature_validation_level", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: signature_validation_level is null, which cannot be sent" : "Bad parameter: signature_validation_level must be of type string", "signature_validation_level");
            }
            if (parameters.TryGetValue("server_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_certificate is null, which cannot be sent" : "Bad parameter: server_certificate must be of type string", "server_certificate");
            }
            if (parameters.TryGetValue("default_mime_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: default_mime_type is null, which cannot be sent" : "Bad parameter: default_mime_type must be of type string", "default_mime_type");
            }
            if (parameters.TryGetValue("additional_http_headers", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: additional_http_headers is null, which cannot be sent" : "Bad parameter: additional_http_headers must be of type object", "additional_http_headers");
            }
            if (parameters.TryGetValue("as2_station_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: as2_station_id is null, which cannot be sent" : "Bad parameter: as2_station_id must be of type Nullable<Int64>", "as2_station_id");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("uri", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: uri is null, which cannot be sent" : "Bad parameter: uri must be of type string", "uri");
            }
            if (parameters.TryGetValue("public_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_certificate is null, which cannot be sent" : "Bad parameter: public_certificate must be of type string", "public_certificate");
            }
            if (parameters.TryGetValue("pkcs12", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12 is null, which cannot be sent" : "Bad parameter: pkcs12 must be of type string", "pkcs12");
            }
            if (parameters.TryGetValue("pkcs12_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12_password is null, which cannot be sent" : "Bad parameter: pkcs12_password must be of type string", "pkcs12_password");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_partners", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<As2Partner>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   enable_dedicated_ips - boolean - If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
        ///   http_auth_username - string - Username to send to server for HTTP Authentication.
        ///   http_auth_password - string - Password to send to server for HTTP Authentication.
        ///   mdn_validation_level - string - How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates. `auto`: Automatically set the correct value for this setting based on next mdn received.
        ///   signature_validation_level - string - Should Files.com require signatures on incoming AS2 messages?  `normal`: require that incoming messages are signed with a valid matching signature. `none`: Unsigned incoming messages are allowed. `auto`: A one-time starting state. The next message received is always accepted and switches this to `normal` if it carries a valid signature, or `none` if it does not.
        ///   server_certificate - string - Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS? (This only applies to Outgoing AS2 message from Files.com to a Partner.)
        ///   default_mime_type - string - Default mime type of the file attached to the encrypted message
        ///   additional_http_headers - object - Additional HTTP Headers for outgoing message sent to this partner.
        ///   name - string - The partner's formal AS2 name.
        ///   uri - string - Public URI where we will send the AS2 messages (via HTTP/HTTPS).
        ///   public_certificate - string - Public certificate for AS2 Partner.  Note: This is the certificate for AS2 message security, not a certificate used for HTTPS authentication.
        ///   pkcs12 - string - Base64-encoded PKCS#12 (.pfx or .p12) file containing the partner's public certificate. Provide this instead of public_certificate. Any private key in the file is discarded. A public-only file must contain exactly one certificate.
        ///   pkcs12_password - string - Password for pkcs12. The file and password are used only for import; the extracted public certificate is stored as PEM.
        /// </summary>
        /// <param name="id">As2 Partner ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2Partner> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<As2Partner> UpdateCore(
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
            if (parameters.TryGetValue("enable_dedicated_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enable_dedicated_ips is null, which cannot be sent" : "Bad parameter: enable_dedicated_ips must be of type bool", "enable_dedicated_ips");
            }
            if (parameters.TryGetValue("http_auth_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: http_auth_username is null, which cannot be sent" : "Bad parameter: http_auth_username must be of type string", "http_auth_username");
            }
            if (parameters.TryGetValue("http_auth_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: http_auth_password is null, which cannot be sent" : "Bad parameter: http_auth_password must be of type string", "http_auth_password");
            }
            if (parameters.TryGetValue("mdn_validation_level", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mdn_validation_level is null, which cannot be sent" : "Bad parameter: mdn_validation_level must be of type string", "mdn_validation_level");
            }
            if (parameters.TryGetValue("signature_validation_level", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: signature_validation_level is null, which cannot be sent" : "Bad parameter: signature_validation_level must be of type string", "signature_validation_level");
            }
            if (parameters.TryGetValue("server_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_certificate is null, which cannot be sent" : "Bad parameter: server_certificate must be of type string", "server_certificate");
            }
            if (parameters.TryGetValue("default_mime_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: default_mime_type is null, which cannot be sent" : "Bad parameter: default_mime_type must be of type string", "default_mime_type");
            }
            if (parameters.TryGetValue("additional_http_headers", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: additional_http_headers is null, which cannot be sent" : "Bad parameter: additional_http_headers must be of type object", "additional_http_headers");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("uri", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: uri is null, which cannot be sent" : "Bad parameter: uri must be of type string", "uri");
            }
            if (parameters.TryGetValue("public_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_certificate is null, which cannot be sent" : "Bad parameter: public_certificate must be of type string", "public_certificate");
            }
            if (parameters.TryGetValue("pkcs12", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12 is null, which cannot be sent" : "Bad parameter: pkcs12 must be of type string", "pkcs12");
            }
            if (parameters.TryGetValue("pkcs12_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pkcs12_password is null, which cannot be sent" : "Bad parameter: pkcs12_password must be of type string", "pkcs12_password");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<As2Partner>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /as2_partners/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">As2 Partner ID.</param>
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
        /// Sends DELETE /as2_partners/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">As2 Partner ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/as2_partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}