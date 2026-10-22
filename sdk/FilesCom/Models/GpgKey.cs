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
    /// GpgKeyEntity model
    /// </summary>
    public class GpgKey : IModel, IJsonOnDeserialized
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
        /// Makes a GpgKey with no attributes set and no options.
        /// </summary>
        public GpgKey() : this(null, null) { }

        /// <summary>
        /// Makes a GpgKey with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public GpgKey(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("expires_at"))
            {
                this.attributes.Add("expires_at", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("partner_id"))
            {
                this.attributes.Add("partner_id", null);
            }
            if (!this.attributes.ContainsKey("partner_name"))
            {
                this.attributes.Add("partner_name", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("public_key_md5"))
            {
                this.attributes.Add("public_key_md5", null);
            }
            if (!this.attributes.ContainsKey("private_key_md5"))
            {
                this.attributes.Add("private_key_md5", null);
            }
            if (!this.attributes.ContainsKey("generated_public_key"))
            {
                this.attributes.Add("generated_public_key", null);
            }
            if (!this.attributes.ContainsKey("generated_private_key"))
            {
                this.attributes.Add("generated_private_key", null);
            }
            if (!this.attributes.ContainsKey("private_key_password_md5"))
            {
                this.attributes.Add("private_key_password_md5", null);
            }
            if (!this.attributes.ContainsKey("public_key"))
            {
                this.attributes.Add("public_key", null);
            }
            if (!this.attributes.ContainsKey("private_key"))
            {
                this.attributes.Add("private_key", null);
            }
            if (!this.attributes.ContainsKey("private_key_password"))
            {
                this.attributes.Add("private_key_password", null);
            }
            if (!this.attributes.ContainsKey("generate_expires_at"))
            {
                this.attributes.Add("generate_expires_at", null);
            }
            if (!this.attributes.ContainsKey("generate_keypair"))
            {
                this.attributes.Add("generate_keypair", false);
            }
            if (!this.attributes.ContainsKey("generate_full_name"))
            {
                this.attributes.Add("generate_full_name", null);
            }
            if (!this.attributes.ContainsKey("generate_email"))
            {
                this.attributes.Add("generate_email", null);
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
        /// GPG key ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Workspace ID (0 for default workspace).
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// GPG key expiration date.
        /// </summary>
        [JsonPropertyName("expires_at")]
        public Nullable<DateTime> ExpiresAt
        {
            get { return (Nullable<DateTime>)attributes["expires_at"]; }
            set { attributes["expires_at"] = value; unsaved.Add("expires_at"); }
        }

        /// <summary>
        /// GPG key name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Partner ID who owns this GPG Key, if applicable.
        /// </summary>
        [JsonPropertyName("partner_id")]
        public Nullable<Int64> PartnerId
        {
            get { return (Nullable<Int64>)attributes["partner_id"]; }
            set { attributes["partner_id"] = value; unsaved.Add("partner_id"); }
        }

        /// <summary>
        /// Name of the Partner who owns this GPG Key, if applicable.
        /// </summary>
        [JsonPropertyName("partner_name")]
        public string PartnerName
        {
            get { return (string)attributes["partner_name"]; }
            set { attributes["partner_name"] = value; unsaved.Add("partner_name"); }
        }

        /// <summary>
        /// User ID who owns this GPG Key, if applicable.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; unsaved.Add("user_id"); }
        }

        /// <summary>
        /// MD5 hash of the GPG public key
        /// </summary>
        [JsonPropertyName("public_key_md5")]
        public string PublicKeyMd5
        {
            get { return (string)attributes["public_key_md5"]; }
            set { attributes["public_key_md5"] = value; unsaved.Add("public_key_md5"); }
        }

        /// <summary>
        /// MD5 hash of the GPG private key.
        /// </summary>
        [JsonPropertyName("private_key_md5")]
        public string PrivateKeyMd5
        {
            get { return (string)attributes["private_key_md5"]; }
            set { attributes["private_key_md5"] = value; unsaved.Add("private_key_md5"); }
        }

        /// <summary>
        /// GPG public key
        /// </summary>
        [JsonPropertyName("generated_public_key")]
        public string GeneratedPublicKey
        {
            get { return (string)attributes["generated_public_key"]; }
            set { attributes["generated_public_key"] = value; unsaved.Add("generated_public_key"); }
        }

        /// <summary>
        /// GPG private key.
        /// </summary>
        [JsonPropertyName("generated_private_key")]
        public string GeneratedPrivateKey
        {
            get { return (string)attributes["generated_private_key"]; }
            set { attributes["generated_private_key"] = value; unsaved.Add("generated_private_key"); }
        }

        /// <summary>
        /// GPG private key password. Only required for password protected keys.
        /// </summary>
        [JsonPropertyName("private_key_password_md5")]
        public string PrivateKeyPasswordMd5
        {
            get { return (string)attributes["private_key_password_md5"]; }
            set { attributes["private_key_password_md5"] = value; unsaved.Add("private_key_password_md5"); }
        }

        /// <summary>
        /// The GPG public key
        /// </summary>
        [JsonPropertyName("public_key")]
        public string PublicKey
        {
            get { return (string)attributes["public_key"]; }
            set { attributes["public_key"] = value; unsaved.Add("public_key"); }
        }

        /// <summary>
        /// The GPG private key
        /// </summary>
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string)attributes["private_key"]; }
            set { attributes["private_key"] = value; unsaved.Add("private_key"); }
        }

        /// <summary>
        /// The GPG private key password
        /// </summary>
        [JsonPropertyName("private_key_password")]
        public string PrivateKeyPassword
        {
            get { return (string)attributes["private_key_password"]; }
            set { attributes["private_key_password"] = value; unsaved.Add("private_key_password"); }
        }

        /// <summary>
        /// Expiration date of the key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        [JsonPropertyName("generate_expires_at")]
        public string GenerateExpiresAt
        {
            get { return (string)attributes["generate_expires_at"]; }
            set { attributes["generate_expires_at"] = value; unsaved.Add("generate_expires_at"); }
        }

        /// <summary>
        /// If true, generate a new GPG key pair. Can not be used with `public_key`/`private_key`
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("generate_keypair")]
        public bool GenerateKeypair
        {
            get { return attributes["generate_keypair"] == null ? false : (bool)attributes["generate_keypair"]; }
            set { attributes["generate_keypair"] = value; unsaved.Add("generate_keypair"); }
        }

        /// <summary>
        /// Full name of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        [JsonPropertyName("generate_full_name")]
        public string GenerateFullName
        {
            get { return (string)attributes["generate_full_name"]; }
            set { attributes["generate_full_name"] = value; unsaved.Add("generate_full_name"); }
        }

        /// <summary>
        /// Email address of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        [JsonPropertyName("generate_email")]
        public string GenerateEmail
        {
            get { return (string)attributes["generate_email"]; }
            set { attributes["generate_email"] = value; unsaved.Add("generate_email"); }
        }

        /// <summary>
        /// Parameters:
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name - string - GPG key name.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The GpgKey it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<GpgKey> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name - string - GPG key name.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The GpgKey it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<GpgKey> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<GpgKey> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The GpgKey has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_id is null, which cannot be sent" : "Bad parameter: partner_id must be of type Nullable<Int64>", "partner_id");
            }
            if (parameters.TryGetValue("public_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_key is null, which cannot be sent" : "Bad parameter: public_key must be of type string", "public_key");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_password is null, which cannot be sent" : "Bad parameter: private_key_password must be of type string", "private_key_password");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/gpg_keys/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            GpgKey updated = JsonUtil.DeserializeModelResponse<GpgKey>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<GpgKey>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /gpg_keys/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /gpg_keys/{id} to the Files.com API.
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
                throw new MissingParameterException("The GpgKey has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/gpg_keys/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "user_id", "partner_id", "public_key", "private_key", "private_key_password", "name", "workspace_id", "generate_expires_at", "generate_keypair", "generate_full_name", "generate_email" };
        private static readonly string[] UpdateParameterNames = { "id", "partner_id", "public_key", "private_key", "private_key_password", "name" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this GpgKey on Files.com, or updates it if it has an ID.
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
            GpgKey created = await GpgKey.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `expires_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `partner_id` or `expires_at`. Valid field combinations are `[ workspace_id, partner_id ]`, `[ workspace_id, expires_at ]`, `[ partner_id, expires_at ]` or `[ workspace_id, partner_id, expires_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `expires_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `expires_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `expires_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `expires_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<GpgKey> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `expires_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `partner_id` or `expires_at`. Valid field combinations are `[ workspace_id, partner_id ]`, `[ workspace_id, expires_at ]`, `[ partner_id, expires_at ]` or `[ workspace_id, partner_id, expires_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `expires_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `expires_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `expires_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `expires_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<GpgKey> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<GpgKey> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
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
            if (parameters.TryGetValue("filter_gt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gt is null, which cannot be sent" : "Bad parameter: filter_gt must be of type object", "filter_gt");
            }
            if (parameters.TryGetValue("filter_gteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gteq is null, which cannot be sent" : "Bad parameter: filter_gteq must be of type object", "filter_gteq");
            }
            if (parameters.TryGetValue("filter_lt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lt is null, which cannot be sent" : "Bad parameter: filter_lt must be of type object", "filter_lt");
            }
            if (parameters.TryGetValue("filter_lteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lteq is null, which cannot be sent" : "Bad parameter: filter_lteq must be of type object", "filter_lteq");
            }

            return new FilesList<GpgKey>(client, $"/gpg_keys", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Gpg Key ID.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<GpgKey> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Gpg Key ID.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<GpgKey> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<GpgKey> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/gpg_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<GpgKey>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name (required) - string - GPG key name.
        ///   workspace_id - int64 - Workspace ID (0 for default workspace).
        ///   generate_expires_at - string - Expiration date of the key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        ///   generate_keypair - boolean - If true, generate a new GPG key pair. Can not be used with `public_key`/`private_key`
        ///   generate_full_name - string - Full name of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        ///   generate_email - string - Email address of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<GpgKey> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<GpgKey> CreateCore(
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
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_id is null, which cannot be sent" : "Bad parameter: partner_id must be of type Nullable<Int64>", "partner_id");
            }
            if (parameters.TryGetValue("public_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_key is null, which cannot be sent" : "Bad parameter: public_key must be of type string", "public_key");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_password is null, which cannot be sent" : "Bad parameter: private_key_password must be of type string", "private_key_password");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("generate_expires_at", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: generate_expires_at is null, which cannot be sent" : "Bad parameter: generate_expires_at must be of type string", "generate_expires_at");
            }
            if (parameters.TryGetValue("generate_keypair", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: generate_keypair is null, which cannot be sent" : "Bad parameter: generate_keypair must be of type bool", "generate_keypair");
            }
            if (parameters.TryGetValue("generate_full_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: generate_full_name is null, which cannot be sent" : "Bad parameter: generate_full_name must be of type string", "generate_full_name");
            }
            if (parameters.TryGetValue("generate_email", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: generate_email is null, which cannot be sent" : "Bad parameter: generate_email must be of type string", "generate_email");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/gpg_keys", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<GpgKey>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name - string - GPG key name.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<GpgKey> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<GpgKey> UpdateCore(
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
            if (parameters.TryGetValue("partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_id is null, which cannot be sent" : "Bad parameter: partner_id must be of type Nullable<Int64>", "partner_id");
            }
            if (parameters.TryGetValue("public_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_key is null, which cannot be sent" : "Bad parameter: public_key must be of type string", "public_key");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_password is null, which cannot be sent" : "Bad parameter: private_key_password must be of type string", "private_key_password");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/gpg_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<GpgKey>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /gpg_keys/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
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
        /// Sends DELETE /gpg_keys/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/gpg_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}