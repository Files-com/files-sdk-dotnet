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
    public class Session : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        public Session() : this(null, null) { }

        public Session(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("language"))
            {
                this.attributes.Add("language", null);
            }
            if (!this.attributes.ContainsKey("read_only"))
            {
                this.attributes.Add("read_only", false);
            }
            if (!this.attributes.ContainsKey("sftp_insecure_ciphers"))
            {
                this.attributes.Add("sftp_insecure_ciphers", false);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("password"))
            {
                this.attributes.Add("password", null);
            }
            if (!this.attributes.ContainsKey("otp"))
            {
                this.attributes.Add("otp", null);
            }
            if (!this.attributes.ContainsKey("partial_session_id"))
            {
                this.attributes.Add("partial_session_id", null);
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
        /// Session ID
        /// </summary>
        [JsonPropertyName("id")]
        public string Id
        {
            get { return (string)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Session language
        /// </summary>
        [JsonPropertyName("language")]
        public string Language
        {
            get { return (string)attributes["language"]; }
            set { attributes["language"] = value; }
        }

        /// <summary>
        /// Is this session read only?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("read_only")]
        public bool ReadOnly
        {
            get { return attributes["read_only"] == null ? false : (bool)attributes["read_only"]; }
            set { attributes["read_only"] = value; }
        }

        /// <summary>
        /// Are insecure SFTP ciphers allowed for this user? (If this is set to true, the site administrator has signaled that it is ok to use less secure SSH ciphers for this user.)
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_insecure_ciphers")]
        public bool SftpInsecureCiphers
        {
            get { return attributes["sftp_insecure_ciphers"] == null ? false : (bool)attributes["sftp_insecure_ciphers"]; }
            set { attributes["sftp_insecure_ciphers"] = value; }
        }

        /// <summary>
        /// Username to sign in as
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// Password for sign in
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string)attributes["password"]; }
            set { attributes["password"] = value; }
        }

        /// <summary>
        /// If this user has a 2FA device, provide its OTP or code here.
        /// </summary>
        [JsonPropertyName("otp")]
        public string Otp
        {
            get { return (string)attributes["otp"]; }
            set { attributes["otp"] = value; }
        }

        /// <summary>
        /// Identifier for a partially-completed login
        /// </summary>
        [JsonPropertyName("partial_session_id")]
        public string PartialSessionId
        {
            get { return (string)attributes["partial_session_id"]; }
            set { attributes["partial_session_id"] = value; }
        }


        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The Session object doesn't support updates.");
            }
            else
            {
                var newObj = await Session.CreateCore(new OperationContext(FilesClient.Bind(ref client)), this.attributes, DictionaryUtil.Copy(this.options), cancellationToken);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   username - string - Username to sign in as
        ///   password - string - Password for sign in
        ///   otp - string - If this user has a 2FA device, provide its OTP or code here.
        ///   partial_session_id - string - Identifier for a partially-completed login
        /// </summary>
        public static Task<Session> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Session> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("otp") && !(parameters["otp"] is string))
            {
                throw new ArgumentException("Bad parameter: otp must be of type string", "parameters[\"otp\"]");
            }
            if (parameters.ContainsKey("partial_session_id") && !(parameters["partial_session_id"] is string))
            {
                throw new ArgumentException("Bad parameter: partial_session_id must be of type string", "parameters[\"partial_session_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/sessions", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<Session>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// </summary>
        public static Task Delete(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        public static Task Destroy(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(parameters, options);
        }

        internal static async Task DeleteCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/sessions", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
            response.Dispose();
        }

    }
}