using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class GpgKey
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public GpgKey() : this(null, null) { }

        public GpgKey(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
        /// GPG key ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// GPG key expiration date.
        /// </summary>
        [JsonPropertyName("expires_at")]
        public Nullable<DateTime> ExpiresAt
        {
            get { return (Nullable<DateTime>)attributes["expires_at"]; }
            set { attributes["expires_at"] = value; }
        }

        /// <summary>
        /// GPG key name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Partner ID who owns this GPG Key, if applicable.
        /// </summary>
        [JsonPropertyName("partner_id")]
        public Nullable<Int64> PartnerId
        {
            get { return (Nullable<Int64>)attributes["partner_id"]; }
            set { attributes["partner_id"] = value; }
        }

        /// <summary>
        /// Name of the Partner who owns this GPG Key, if applicable.
        /// </summary>
        [JsonPropertyName("partner_name")]
        public string PartnerName
        {
            get { return (string)attributes["partner_name"]; }
            set { attributes["partner_name"] = value; }
        }

        /// <summary>
        /// User ID who owns this GPG Key, if applicable.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// MD5 hash of the GPG public key
        /// </summary>
        [JsonPropertyName("public_key_md5")]
        public string PublicKeyMd5
        {
            get { return (string)attributes["public_key_md5"]; }
            set { attributes["public_key_md5"] = value; }
        }

        /// <summary>
        /// MD5 hash of the GPG private key.
        /// </summary>
        [JsonPropertyName("private_key_md5")]
        public string PrivateKeyMd5
        {
            get { return (string)attributes["private_key_md5"]; }
            set { attributes["private_key_md5"] = value; }
        }

        /// <summary>
        /// GPG public key
        /// </summary>
        [JsonPropertyName("generated_public_key")]
        public string GeneratedPublicKey
        {
            get { return (string)attributes["generated_public_key"]; }
            set { attributes["generated_public_key"] = value; }
        }

        /// <summary>
        /// GPG private key.
        /// </summary>
        [JsonPropertyName("generated_private_key")]
        public string GeneratedPrivateKey
        {
            get { return (string)attributes["generated_private_key"]; }
            set { attributes["generated_private_key"] = value; }
        }

        /// <summary>
        /// GPG private key password. Only required for password protected keys.
        /// </summary>
        [JsonPropertyName("private_key_password_md5")]
        public string PrivateKeyPasswordMd5
        {
            get { return (string)attributes["private_key_password_md5"]; }
            set { attributes["private_key_password_md5"] = value; }
        }

        /// <summary>
        /// The GPG public key
        /// </summary>
        [JsonPropertyName("public_key")]
        public string PublicKey
        {
            get { return (string)attributes["public_key"]; }
            set { attributes["public_key"] = value; }
        }

        /// <summary>
        /// The GPG private key
        /// </summary>
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string)attributes["private_key"]; }
            set { attributes["private_key"] = value; }
        }

        /// <summary>
        /// The GPG private key password
        /// </summary>
        [JsonPropertyName("private_key_password")]
        public string PrivateKeyPassword
        {
            get { return (string)attributes["private_key_password"]; }
            set { attributes["private_key_password"] = value; }
        }

        /// <summary>
        /// Expiration date of the key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        [JsonPropertyName("generate_expires_at")]
        public string GenerateExpiresAt
        {
            get { return (string)attributes["generate_expires_at"]; }
            set { attributes["generate_expires_at"] = value; }
        }

        /// <summary>
        /// If true, generate a new GPG key pair. Can not be used with `public_key`/`private_key`
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("generate_keypair")]
        public bool GenerateKeypair
        {
            get { return attributes["generate_keypair"] == null ? false : (bool)attributes["generate_keypair"]; }
            set { attributes["generate_keypair"] = value; }
        }

        /// <summary>
        /// Full name of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        [JsonPropertyName("generate_full_name")]
        public string GenerateFullName
        {
            get { return (string)attributes["generate_full_name"]; }
            set { attributes["generate_full_name"] = value; }
        }

        /// <summary>
        /// Email address of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        [JsonPropertyName("generate_email")]
        public string GenerateEmail
        {
            get { return (string)attributes["generate_email"]; }
            set { attributes["generate_email"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name - string - GPG key name.
        /// </summary>
        public async Task<GpgKey> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("partner_id") && !(parameters["partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: partner_id must be of type Nullable<Int64>", "parameters[\"partner_id\"]");
            }
            if (parameters.ContainsKey("public_key") && !(parameters["public_key"] is string))
            {
                throw new ArgumentException("Bad parameter: public_key must be of type string", "parameters[\"public_key\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_password") && !(parameters["private_key_password"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key_password must be of type string", "parameters[\"private_key_password\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/gpg_keys/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<GpgKey>(responseJson);
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

            await FilesClient.SendRequest($"/gpg_keys/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await GpgKey.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name` and `expires_at`.
        /// </summary>
        public static FilesList<GpgKey> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
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

            return new FilesList<GpgKey>($"/gpg_keys", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<GpgKey> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Gpg Key ID.
        /// </summary>
        public static async Task<GpgKey> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/gpg_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<GpgKey>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<GpgKey> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name (required) - string - GPG key name.
        ///   generate_expires_at - string - Expiration date of the key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        ///   generate_keypair - boolean - If true, generate a new GPG key pair. Can not be used with `public_key`/`private_key`
        ///   generate_full_name - string - Full name of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        ///   generate_email - string - Email address of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        public static async Task<GpgKey> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("name") || parameters["name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: name", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("partner_id") && !(parameters["partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: partner_id must be of type Nullable<Int64>", "parameters[\"partner_id\"]");
            }
            if (parameters.ContainsKey("public_key") && !(parameters["public_key"] is string))
            {
                throw new ArgumentException("Bad parameter: public_key must be of type string", "parameters[\"public_key\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_password") && !(parameters["private_key_password"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key_password must be of type string", "parameters[\"private_key_password\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("generate_expires_at") && !(parameters["generate_expires_at"] is string))
            {
                throw new ArgumentException("Bad parameter: generate_expires_at must be of type string", "parameters[\"generate_expires_at\"]");
            }
            if (parameters.ContainsKey("generate_keypair") && !(parameters["generate_keypair"] is bool))
            {
                throw new ArgumentException("Bad parameter: generate_keypair must be of type bool", "parameters[\"generate_keypair\"]");
            }
            if (parameters.ContainsKey("generate_full_name") && !(parameters["generate_full_name"] is string))
            {
                throw new ArgumentException("Bad parameter: generate_full_name must be of type string", "parameters[\"generate_full_name\"]");
            }
            if (parameters.ContainsKey("generate_email") && !(parameters["generate_email"] is string))
            {
                throw new ArgumentException("Bad parameter: generate_email must be of type string", "parameters[\"generate_email\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/gpg_keys", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<GpgKey>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name - string - GPG key name.
        /// </summary>
        public static async Task<GpgKey> Update(
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
            if (parameters.ContainsKey("partner_id") && !(parameters["partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: partner_id must be of type Nullable<Int64>", "parameters[\"partner_id\"]");
            }
            if (parameters.ContainsKey("public_key") && !(parameters["public_key"] is string))
            {
                throw new ArgumentException("Bad parameter: public_key must be of type string", "parameters[\"public_key\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_password") && !(parameters["private_key_password"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key_password must be of type string", "parameters[\"private_key_password\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/gpg_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<GpgKey>(responseJson);
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

            await FilesClient.SendRequest($"/gpg_keys/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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