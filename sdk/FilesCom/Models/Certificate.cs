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
    /// CertificateEntity model
    /// </summary>
    public class Certificate : IModel, IJsonOnDeserialized
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
        // The JSON, when handed out since then, of the arrays among them that callers can change in place, directly or
        // through a model in them. SaveAsync compares each with the array's JSON now, and sends the ones that differ.
        private readonly Dictionary<string, string> handedOut = new Dictionary<string, string>();
        private static readonly string[] ArrayAttributeNames = { "certificate_extra_domains" };
        /// <summary>
        /// Makes a Certificate with no attributes set and no options.
        /// </summary>
        public Certificate() : this(null, null) { }

        /// <summary>
        /// Makes a Certificate with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Certificate(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("custom_domain_id"))
            {
                this.attributes.Add("custom_domain_id", null);
            }
            if (!this.attributes.ContainsKey("certificate"))
            {
                this.attributes.Add("certificate", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("display_status"))
            {
                this.attributes.Add("display_status", null);
            }
            if (!this.attributes.ContainsKey("domains"))
            {
                this.attributes.Add("domains", new string[0]);
            }
            if (!this.attributes.ContainsKey("expires_at"))
            {
                this.attributes.Add("expires_at", null);
            }
            if (!this.attributes.ContainsKey("brick_managed"))
            {
                this.attributes.Add("brick_managed", false);
            }
            if (!this.attributes.ContainsKey("intermediates"))
            {
                this.attributes.Add("intermediates", null);
            }
            if (!this.attributes.ContainsKey("ip_addresses"))
            {
                this.attributes.Add("ip_addresses", new string[0]);
            }
            if (!this.attributes.ContainsKey("issuer"))
            {
                this.attributes.Add("issuer", null);
            }
            if (!this.attributes.ContainsKey("key_type"))
            {
                this.attributes.Add("key_type", null);
            }
            if (!this.attributes.ContainsKey("request"))
            {
                this.attributes.Add("request", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("subject"))
            {
                this.attributes.Add("subject", null);
            }
            if (!this.attributes.ContainsKey("certificate_domain"))
            {
                this.attributes.Add("certificate_domain", null);
            }
            if (!this.attributes.ContainsKey("certificate_extra_domains"))
            {
                this.attributes.Add("certificate_extra_domains", new string[0]);
            }
            if (!this.attributes.ContainsKey("certificate_country"))
            {
                this.attributes.Add("certificate_country", null);
            }
            if (!this.attributes.ContainsKey("certificate_state_or_province"))
            {
                this.attributes.Add("certificate_state_or_province", null);
            }
            if (!this.attributes.ContainsKey("certificate_city_or_locale"))
            {
                this.attributes.Add("certificate_city_or_locale", null);
            }
            if (!this.attributes.ContainsKey("certificate_company_name"))
            {
                this.attributes.Add("certificate_company_name", null);
            }
            if (!this.attributes.ContainsKey("csr_ou1"))
            {
                this.attributes.Add("csr_ou1", null);
            }
            if (!this.attributes.ContainsKey("csr_ou2"))
            {
                this.attributes.Add("csr_ou2", null);
            }
            if (!this.attributes.ContainsKey("csr_ou3"))
            {
                this.attributes.Add("csr_ou3", null);
            }
            if (!this.attributes.ContainsKey("certificate_email_address"))
            {
                this.attributes.Add("certificate_email_address", null);
            }
            if (!this.attributes.ContainsKey("private_key"))
            {
                this.attributes.Add("private_key", null);
            }
            if (!this.attributes.ContainsKey("password"))
            {
                this.attributes.Add("password", null);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            // The copy holds this object's own arrays, which the caller can change in place as through its properties.
            foreach (string name in ArrayAttributeNames)
            {
                HandedOut(name);
            }
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
            handedOut.Clear();
        }


        /// <summary>
        /// Certificate ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Descriptive name of certificate
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Custom Domain that owns this certificate, including while the certificate is inactive.
        /// </summary>
        [JsonPropertyName("custom_domain_id")]
        public Nullable<Int64> CustomDomainId
        {
            get { return (Nullable<Int64>)attributes["custom_domain_id"]; }
            set { attributes["custom_domain_id"] = value; unsaved.Add("custom_domain_id"); }
        }

        /// <summary>
        /// Full text of SSL certificate
        /// </summary>
        [JsonPropertyName("certificate")]
        public string CertificateType
        {
            get { return (string)attributes["certificate"]; }
            set { attributes["certificate"] = value; unsaved.Add("certificate"); }
        }

        /// <summary>
        /// Certificate created at
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Certificate status. (One of `Request Generated`, `New`, `Active`, `Active/Expired`, `Expired`, or `Available`)
        /// </summary>
        [JsonPropertyName("display_status")]
        public string DisplayStatus
        {
            get { return (string)attributes["display_status"]; }
            set { attributes["display_status"] = value; unsaved.Add("display_status"); }
        }

        /// <summary>
        /// Domains on this certificate other than files.com domains
        /// </summary>
        [JsonPropertyName("domains")]
        public string[] Domains
        {
            get { return (string[])attributes["domains"]; }
            set { attributes["domains"] = value; unsaved.Add("domains"); }
        }

        /// <summary>
        /// Certificate expiration date/time
        /// </summary>
        [JsonPropertyName("expires_at")]
        public Nullable<DateTime> ExpiresAt
        {
            get { return (Nullable<DateTime>)attributes["expires_at"]; }
            set { attributes["expires_at"] = value; unsaved.Add("expires_at"); }
        }

        /// <summary>
        /// Is this certificate automatically managed and renewed by Files.com?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("brick_managed")]
        public bool BrickManaged
        {
            get { return attributes["brick_managed"] == null ? false : (bool)attributes["brick_managed"]; }
            set { attributes["brick_managed"] = value; unsaved.Add("brick_managed"); }
        }

        /// <summary>
        /// Intermediate certificates
        /// </summary>
        [JsonPropertyName("intermediates")]
        public string Intermediates
        {
            get { return (string)attributes["intermediates"]; }
            set { attributes["intermediates"] = value; unsaved.Add("intermediates"); }
        }

        /// <summary>
        /// A list of IP addresses associated with this SSL Certificate
        /// </summary>
        [JsonPropertyName("ip_addresses")]
        public string[] IpAddresses
        {
            get { return (string[])attributes["ip_addresses"]; }
            set { attributes["ip_addresses"] = value; unsaved.Add("ip_addresses"); }
        }

        /// <summary>
        /// X509 issuer
        /// </summary>
        [JsonPropertyName("issuer")]
        public string Issuer
        {
            get { return (string)attributes["issuer"]; }
            set { attributes["issuer"] = value; unsaved.Add("issuer"); }
        }

        /// <summary>
        /// Type of key
        /// </summary>
        [JsonPropertyName("key_type")]
        public string KeyType
        {
            get { return (string)attributes["key_type"]; }
            set { attributes["key_type"] = value; unsaved.Add("key_type"); }
        }

        /// <summary>
        /// Certificate signing request text
        /// </summary>
        [JsonPropertyName("request")]
        public string Request
        {
            get { return (string)attributes["request"]; }
            set { attributes["request"] = value; unsaved.Add("request"); }
        }

        /// <summary>
        /// Certificate status (Request Generated, New, Active, Active/Expired, Expired, or Available)
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; unsaved.Add("status"); }
        }

        /// <summary>
        /// X509 Subject name
        /// </summary>
        [JsonPropertyName("subject")]
        public string Subject
        {
            get { return (string)attributes["subject"]; }
            set { attributes["subject"] = value; unsaved.Add("subject"); }
        }

        /// <summary>
        /// Domain for certificate.
        /// </summary>
        [JsonPropertyName("certificate_domain")]
        public string CertificateDomain
        {
            get { return (string)attributes["certificate_domain"]; }
            set { attributes["certificate_domain"] = value; unsaved.Add("certificate_domain"); }
        }

        /// <summary>
        /// Additional domains for the certificate CSR.
        /// </summary>
        [JsonPropertyName("certificate_extra_domains")]
        public string[] CertificateExtraDomains
        {
            get { return (string[])HandedOut("certificate_extra_domains"); }
            set { attributes["certificate_extra_domains"] = value; unsaved.Add("certificate_extra_domains"); }
        }

        /// <summary>
        /// Country.
        /// </summary>
        [JsonPropertyName("certificate_country")]
        public string CertificateCountry
        {
            get { return (string)attributes["certificate_country"]; }
            set { attributes["certificate_country"] = value; unsaved.Add("certificate_country"); }
        }

        /// <summary>
        /// State or province.
        /// </summary>
        [JsonPropertyName("certificate_state_or_province")]
        public string CertificateStateOrProvince
        {
            get { return (string)attributes["certificate_state_or_province"]; }
            set { attributes["certificate_state_or_province"] = value; unsaved.Add("certificate_state_or_province"); }
        }

        /// <summary>
        /// City or locale.
        /// </summary>
        [JsonPropertyName("certificate_city_or_locale")]
        public string CertificateCityOrLocale
        {
            get { return (string)attributes["certificate_city_or_locale"]; }
            set { attributes["certificate_city_or_locale"] = value; unsaved.Add("certificate_city_or_locale"); }
        }

        /// <summary>
        /// Company name.
        /// </summary>
        [JsonPropertyName("certificate_company_name")]
        public string CertificateCompanyName
        {
            get { return (string)attributes["certificate_company_name"]; }
            set { attributes["certificate_company_name"] = value; unsaved.Add("certificate_company_name"); }
        }

        /// <summary>
        /// Department name or organization unit 1
        /// </summary>
        [JsonPropertyName("csr_ou1")]
        public string CsrOu1
        {
            get { return (string)attributes["csr_ou1"]; }
            set { attributes["csr_ou1"] = value; unsaved.Add("csr_ou1"); }
        }

        /// <summary>
        /// Department name or organization unit 2
        /// </summary>
        [JsonPropertyName("csr_ou2")]
        public string CsrOu2
        {
            get { return (string)attributes["csr_ou2"]; }
            set { attributes["csr_ou2"] = value; unsaved.Add("csr_ou2"); }
        }

        /// <summary>
        /// Department name or organization unit 3
        /// </summary>
        [JsonPropertyName("csr_ou3")]
        public string CsrOu3
        {
            get { return (string)attributes["csr_ou3"]; }
            set { attributes["csr_ou3"] = value; unsaved.Add("csr_ou3"); }
        }

        /// <summary>
        /// Email address for the certificate owner.
        /// </summary>
        [JsonPropertyName("certificate_email_address")]
        public string CertificateEmailAddress
        {
            get { return (string)attributes["certificate_email_address"]; }
            set { attributes["certificate_email_address"] = value; unsaved.Add("certificate_email_address"); }
        }

        /// <summary>
        /// Certificate private key.
        /// </summary>
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string)attributes["private_key"]; }
            set { attributes["private_key"] = value; unsaved.Add("private_key"); }
        }

        /// <summary>
        /// Certificate password.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string)attributes["password"]; }
            set { attributes["password"] = value; unsaved.Add("password"); }
        }

        /// <summary>
        /// Activate SSL Certificate
        ///
        /// Parameters:
        ///   replace_cert - string - Leave blank or set to `any` to replace any currently active certificate, if applicable.  Set to `new_ips` to activate this certificate as an additional certificate on your Site by allocating a new set of Dedicated IPs (may require a Plan upgrade).  Set to the ID of a currently active certificate to replace that certificate on its set of dedicated IPs.  A certificate belonging to a Custom Domain activates only for that domain: leave blank, use `any`, or specify that domain's current certificate ID.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task Activate(Dictionary<string, object> parameters)
        {
            return ActivateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Activate SSL Certificate
        ///
        /// Parameters:
        ///   replace_cert - string - Leave blank or set to `any` to replace any currently active certificate, if applicable.  Set to `new_ips` to activate this certificate as an additional certificate on your Site by allocating a new set of Dedicated IPs (may require a Plan upgrade).  Set to the ID of a currently active certificate to replace that certificate on its set of dedicated IPs.  A certificate belonging to a Custom Domain activates only for that domain: leave blank, use `any`, or specify that domain's current certificate ID.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task ActivateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ActivateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task ActivateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Certificate has no id, which Activate needs", "id");
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
            if (parameters.TryGetValue("replace_cert", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: replace_cert is null, which cannot be sent" : "Bad parameter: replace_cert must be of type string", "replace_cert");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/certificates/{System.Uri.EscapeDataString(attributes["id"].ToString())}/activate", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Deactivate SSL Certificate
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Deactivate(Dictionary<string, object> parameters)
        {
            return DeactivateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Deactivate SSL Certificate
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeactivateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeactivateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task DeactivateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Certificate has no id, which Deactivate needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/certificates/{System.Uri.EscapeDataString(attributes["id"].ToString())}/deactivate", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Replace this active custom SSL Certificate for the Primary Custom Domain with a Files.com SSL Certificate.  The Primary Custom Domain's CNAME record must point at the Site's subdomain.  The Files.com certificate is issued and activated in the background
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task ReplaceWithFiles(Dictionary<string, object> parameters)
        {
            return ReplaceWithFilesCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Replace this active custom SSL Certificate for the Primary Custom Domain with a Files.com SSL Certificate.  The Primary Custom Domain's CNAME record must point at the Site's subdomain.  The Files.com certificate is issued and activated in the background
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task ReplaceWithFilesAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ReplaceWithFilesCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task ReplaceWithFilesCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Certificate has no id, which ReplaceWithFiles needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/certificates/{System.Uri.EscapeDataString(attributes["id"].ToString())}/replace_with_files_certificate", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Internal certificate name.
        ///   intermediates - string - Any intermediate certificates.  PEM or PKCS7 formats accepted.
        ///   certificate - string - The certificate.  PEM or PKCS7 formats accepted.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Certificate it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Certificate> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Internal certificate name.
        ///   intermediates - string - Any intermediate certificates.  PEM or PKCS7 formats accepted.
        ///   certificate - string - The certificate.  PEM or PKCS7 formats accepted.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Certificate it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Certificate> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Certificate> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Certificate has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("intermediates", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: intermediates is null, which cannot be sent" : "Bad parameter: intermediates must be of type string", "intermediates");
            }
            if (parameters.TryGetValue("certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate is null, which cannot be sent" : "Bad parameter: certificate must be of type string", "certificate");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/certificates/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            Certificate updated = JsonUtil.DeserializeModelResponse<Certificate>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<Certificate>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /certificates/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /certificates/{id} to the Files.com API.
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
                throw new MissingParameterException("The Certificate has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/certificates/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "name", "certificate_domain", "certificate_extra_domains", "certificate_country", "certificate_state_or_province", "certificate_city_or_locale", "certificate_company_name", "csr_ou1", "csr_ou2", "csr_ou3", "certificate_email_address", "key_type", "certificate", "private_key", "password", "intermediates" };
        private static readonly string[] UpdateParameterNames = { "id", "name", "intermediates", "certificate" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Certificate on Files.com, or updates it if it has an ID.
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
            Certificate created = await Certificate.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
                if (unsaved.Contains(name) || ChangedInPlace(name))
                {
                    toSave[name] = attributes[name];
                }
            }
            return toSave;
        }

        // An array attribute, for a property or getAttributes to hand out. Its JSON is kept the first time, as the caller
        // can change the array, or an array in a model in it, without setting anything.
        private object HandedOut(string name)
        {
            object value = attributes[name];
            if (value != null && !handedOut.ContainsKey(name))
            {
                handedOut[name] = JsonSerializer.Serialize(value, JsonUtil.Options);
            }
            return value;
        }

        // Whether an array handed out has been changed in place since, at any depth.
        private bool ChangedInPlace(string name)
        {
            return handedOut.TryGetValue(name, out string handedOutJson) && JsonSerializer.Serialize(attributes[name], JsonUtil.Options) != handedOutJson;
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
            handedOut.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Certificate> List(

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
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Certificate> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Certificate> ListCore(
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

            return new FilesList<Certificate>(client, $"/certificates", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Certificate ID.
        /// </summary>
        /// <param name="id">Certificate ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Certificate> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Certificate ID.
        /// </summary>
        /// <param name="id">Certificate ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Certificate> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<Certificate> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/certificates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Certificate>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name (required) - string - Internal name of the SSL certificate.
        ///   certificate_domain - string - Domain for certificate.
        ///   certificate_extra_domains - array(string) - Additional domains for the certificate CSR.
        ///   certificate_country - string - Country.
        ///   certificate_state_or_province - string - State or province.
        ///   certificate_city_or_locale - string - City or locale.
        ///   certificate_company_name - string - Company name.
        ///   csr_ou1 - string - Department name or organization unit 1
        ///   csr_ou2 - string - Department name or organization unit 2
        ///   csr_ou3 - string - Department name or organization unit 3
        ///   certificate_email_address - string - Email address for the certificate owner.
        ///   key_type - string - Any supported key type.  Defaults to `RSA-4096`.
        ///   certificate - string - The certificate.  PEM or PKCS7 formats accepted.
        ///   private_key - string - Certificate private key.
        ///   password - string - Certificate password.
        ///   intermediates - string - Intermediate certificates.  PEM or PKCS7 formats accepted.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Certificate> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Certificate> CreateCore(
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
            if (parameters.TryGetValue("certificate_domain", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate_domain is null, which cannot be sent" : "Bad parameter: certificate_domain must be of type string", "certificate_domain");
            }
            if (parameters.TryGetValue("certificate_extra_domains", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate_extra_domains is null, which cannot be sent" : "Bad parameter: certificate_extra_domains must be of type string[]", "certificate_extra_domains");
            }
            if (parameters.TryGetValue("certificate_country", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate_country is null, which cannot be sent" : "Bad parameter: certificate_country must be of type string", "certificate_country");
            }
            if (parameters.TryGetValue("certificate_state_or_province", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate_state_or_province is null, which cannot be sent" : "Bad parameter: certificate_state_or_province must be of type string", "certificate_state_or_province");
            }
            if (parameters.TryGetValue("certificate_city_or_locale", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate_city_or_locale is null, which cannot be sent" : "Bad parameter: certificate_city_or_locale must be of type string", "certificate_city_or_locale");
            }
            if (parameters.TryGetValue("certificate_company_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate_company_name is null, which cannot be sent" : "Bad parameter: certificate_company_name must be of type string", "certificate_company_name");
            }
            if (parameters.TryGetValue("csr_ou1", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: csr_ou1 is null, which cannot be sent" : "Bad parameter: csr_ou1 must be of type string", "csr_ou1");
            }
            if (parameters.TryGetValue("csr_ou2", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: csr_ou2 is null, which cannot be sent" : "Bad parameter: csr_ou2 must be of type string", "csr_ou2");
            }
            if (parameters.TryGetValue("csr_ou3", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: csr_ou3 is null, which cannot be sent" : "Bad parameter: csr_ou3 must be of type string", "csr_ou3");
            }
            if (parameters.TryGetValue("certificate_email_address", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate_email_address is null, which cannot be sent" : "Bad parameter: certificate_email_address must be of type string", "certificate_email_address");
            }
            if (parameters.TryGetValue("key_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: key_type is null, which cannot be sent" : "Bad parameter: key_type must be of type string", "key_type");
            }
            if (parameters.TryGetValue("certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate is null, which cannot be sent" : "Bad parameter: certificate must be of type string", "certificate");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("intermediates", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: intermediates is null, which cannot be sent" : "Bad parameter: intermediates must be of type string", "intermediates");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/certificates", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Certificate>(responseJson, context, options);
        }

        /// <summary>
        /// Activate SSL Certificate
        ///
        /// Parameters:
        ///   replace_cert - string - Leave blank or set to `any` to replace any currently active certificate, if applicable.  Set to `new_ips` to activate this certificate as an additional certificate on your Site by allocating a new set of Dedicated IPs (may require a Plan upgrade).  Set to the ID of a currently active certificate to replace that certificate on its set of dedicated IPs.  A certificate belonging to a Custom Domain activates only for that domain: leave blank, use `any`, or specify that domain's current certificate ID.
        /// </summary>
        /// <param name="id">Certificate ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Activate(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ActivateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task ActivateCore(
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
            if (parameters.TryGetValue("replace_cert", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: replace_cert is null, which cannot be sent" : "Bad parameter: replace_cert must be of type string", "replace_cert");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/certificates/{System.Uri.EscapeDataString(parameters["id"].ToString())}/activate", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Deactivate SSL Certificate
        /// </summary>
        /// <param name="id">Certificate ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Deactivate(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeactivateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task DeactivateCore(
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

            await FilesClient.SendRequestAndDiscard(context, $"/certificates/{System.Uri.EscapeDataString(parameters["id"].ToString())}/deactivate", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Replace this active custom SSL Certificate for the Primary Custom Domain with a Files.com SSL Certificate.  The Primary Custom Domain's CNAME record must point at the Site's subdomain.  The Files.com certificate is issued and activated in the background
        /// </summary>
        /// <param name="id">Certificate ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task ReplaceWithFiles(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ReplaceWithFilesCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task ReplaceWithFilesCore(
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

            await FilesClient.SendRequestAndDiscard(context, $"/certificates/{System.Uri.EscapeDataString(parameters["id"].ToString())}/replace_with_files_certificate", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Internal certificate name.
        ///   intermediates - string - Any intermediate certificates.  PEM or PKCS7 formats accepted.
        ///   certificate - string - The certificate.  PEM or PKCS7 formats accepted.
        /// </summary>
        /// <param name="id">Certificate ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Certificate> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<Certificate> UpdateCore(
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
            if (parameters.TryGetValue("intermediates", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: intermediates is null, which cannot be sent" : "Bad parameter: intermediates must be of type string", "intermediates");
            }
            if (parameters.TryGetValue("certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: certificate is null, which cannot be sent" : "Bad parameter: certificate must be of type string", "certificate");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/certificates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Certificate>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /certificates/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Certificate ID.</param>
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
        /// Sends DELETE /certificates/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Certificate ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/certificates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}