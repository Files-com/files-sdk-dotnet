using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Session
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
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
            if (!this.attributes.ContainsKey("login_token"))
            {
                this.attributes.Add("login_token", null);
            }
            if (!this.attributes.ContainsKey("login_token_domain"))
            {
                this.attributes.Add("login_token_domain", null);
            }
            if (!this.attributes.ContainsKey("max_dir_listing_size"))
            {
                this.attributes.Add("max_dir_listing_size", null);
            }
            if (!this.attributes.ContainsKey("multiple_regions"))
            {
                this.attributes.Add("multiple_regions", null);
            }
            if (!this.attributes.ContainsKey("read_only"))
            {
                this.attributes.Add("read_only", null);
            }
            if (!this.attributes.ContainsKey("root_path"))
            {
                this.attributes.Add("root_path", null);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
            }
            if (!this.attributes.ContainsKey("ssl_required"))
            {
                this.attributes.Add("ssl_required", null);
            }
            if (!this.attributes.ContainsKey("tls_disabled"))
            {
                this.attributes.Add("tls_disabled", null);
            }
            if (!this.attributes.ContainsKey("two_factor_setup_needed"))
            {
                this.attributes.Add("two_factor_setup_needed", null);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_sms"))
            {
                this.attributes.Add("allowed_2fa_method_sms", null);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_totp"))
            {
                this.attributes.Add("allowed_2fa_method_totp", null);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_u2f"))
            {
                this.attributes.Add("allowed_2fa_method_u2f", null);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_yubi"))
            {
                this.attributes.Add("allowed_2fa_method_yubi", null);
            }
            if (!this.attributes.ContainsKey("use_provided_modified_at"))
            {
                this.attributes.Add("use_provided_modified_at", null);
            }
            if (!this.attributes.ContainsKey("windows_mode_ftp"))
            {
                this.attributes.Add("windows_mode_ftp", null);
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

        public object GetOption(string name)
        {
            return (this.options.ContainsKey(name) ? this.options[name] : null);
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
            get { return (string) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Session language
        /// </summary>
        [JsonPropertyName("language")]
        public string Language
        {
            get { return (string) attributes["language"]; }
            set { attributes["language"] = value; }
        }

        /// <summary>
        /// Login token. If set, this token will allow your user to log in via browser at the domain in `login_token_domain`.
        /// </summary>
        [JsonPropertyName("login_token")]
        public string LoginToken
        {
            get { return (string) attributes["login_token"]; }
            set { attributes["login_token"] = value; }
        }

        /// <summary>
        /// Domain to use with `login_token`.
        /// </summary>
        [JsonPropertyName("login_token_domain")]
        public string LoginTokenDomain
        {
            get { return (string) attributes["login_token_domain"]; }
            set { attributes["login_token_domain"] = value; }
        }

        /// <summary>
        /// Maximum number of files to retrieve per folder for a directory listing.  This is based on the user's plan.
        /// </summary>
        [JsonPropertyName("max_dir_listing_size")]
        public Nullable<Int64> MaxDirListingSize
        {
            get { return (Nullable<Int64>) attributes["max_dir_listing_size"]; }
            set { attributes["max_dir_listing_size"] = value; }
        }

        /// <summary>
        /// Can access multiple regions?
        /// </summary>
        [JsonPropertyName("multiple_regions")]
        public bool MultipleRegions
        {
            get { return (bool) attributes["multiple_regions"]; }
            set { attributes["multiple_regions"] = value; }
        }

        /// <summary>
        /// Is this session read only?
        /// </summary>
        [JsonPropertyName("read_only")]
        public bool ReadOnly
        {
            get { return (bool) attributes["read_only"]; }
            set { attributes["read_only"] = value; }
        }

        /// <summary>
        /// Initial root path to start the user's session in.
        /// </summary>
        [JsonPropertyName("root_path")]
        public string RootPath
        {
            get { return (string) attributes["root_path"]; }
            set { attributes["root_path"] = value; }
        }

        /// <summary>
        /// Site ID
        /// </summary>
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>) attributes["site_id"]; }
            set { attributes["site_id"] = value; }
        }

        /// <summary>
        /// Is SSL required for this user?  (If so, ensure all your communications with this user use SSL.)
        /// </summary>
        [JsonPropertyName("ssl_required")]
        public bool SslRequired
        {
            get { return (bool) attributes["ssl_required"]; }
            set { attributes["ssl_required"] = value; }
        }

        /// <summary>
        /// Is strong TLS disabled for this user? (If this is set to true, the site administrator has signaled that it is ok to use less secure TLS versions for this user.)
        /// </summary>
        [JsonPropertyName("tls_disabled")]
        public bool TlsDisabled
        {
            get { return (bool) attributes["tls_disabled"]; }
            set { attributes["tls_disabled"] = value; }
        }

        /// <summary>
        /// If true, this user needs to add a Two Factor Authentication method before performing any further actions.
        /// </summary>
        [JsonPropertyName("two_factor_setup_needed")]
        public bool TwoFactorSetupNeeded
        {
            get { return (bool) attributes["two_factor_setup_needed"]; }
            set { attributes["two_factor_setup_needed"] = value; }
        }

        /// <summary>
        /// Sent only if 2FA setup is needed. Is SMS two factor authentication allowed?
        /// </summary>
        [JsonPropertyName("allowed_2fa_method_sms")]
        public bool Allowed2faMethodSms
        {
            get { return (bool) attributes["allowed_2fa_method_sms"]; }
            set { attributes["allowed_2fa_method_sms"] = value; }
        }

        /// <summary>
        /// Sent only if 2FA setup is needed. Is TOTP two factor authentication allowed?
        /// </summary>
        [JsonPropertyName("allowed_2fa_method_totp")]
        public bool Allowed2faMethodTotp
        {
            get { return (bool) attributes["allowed_2fa_method_totp"]; }
            set { attributes["allowed_2fa_method_totp"] = value; }
        }

        /// <summary>
        /// Sent only if 2FA setup is needed. Is U2F two factor authentication allowed?
        /// </summary>
        [JsonPropertyName("allowed_2fa_method_u2f")]
        public bool Allowed2faMethodU2f
        {
            get { return (bool) attributes["allowed_2fa_method_u2f"]; }
            set { attributes["allowed_2fa_method_u2f"] = value; }
        }

        /// <summary>
        /// Sent only if 2FA setup is needed. Is Yubikey two factor authentication allowed?
        /// </summary>
        [JsonPropertyName("allowed_2fa_method_yubi")]
        public bool Allowed2faMethodYubi
        {
            get { return (bool) attributes["allowed_2fa_method_yubi"]; }
            set { attributes["allowed_2fa_method_yubi"] = value; }
        }

        /// <summary>
        /// Allow the user to provide file/folder modified at dates?  If false, the server will always use the current date/time.
        /// </summary>
        [JsonPropertyName("use_provided_modified_at")]
        public bool UseProvidedModifiedAt
        {
            get { return (bool) attributes["use_provided_modified_at"]; }
            set { attributes["use_provided_modified_at"] = value; }
        }

        /// <summary>
        /// Does this user want to use Windows line-ending emulation?  (CR vs CRLF)
        /// </summary>
        [JsonPropertyName("windows_mode_ftp")]
        public bool WindowsModeFtp
        {
            get { return (bool) attributes["windows_mode_ftp"]; }
            set { attributes["windows_mode_ftp"] = value; }
        }

        /// <summary>
        /// Username to sign in as
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string) attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// Password for sign in
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string) attributes["password"]; }
            set { attributes["password"] = value; }
        }

        /// <summary>
        /// If this user has a 2FA device, provide its OTP or code here.
        /// </summary>
        [JsonPropertyName("otp")]
        public string Otp
        {
            get { return (string) attributes["otp"]; }
            set { attributes["otp"] = value; }
        }

        /// <summary>
        /// Identifier for a partially-completed login
        /// </summary>
        [JsonPropertyName("partial_session_id")]
        public string PartialSessionId
        {
            get { return (string) attributes["partial_session_id"]; }
            set { attributes["partial_session_id"] = value; }
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The Session object doesn't support updates.");
            }
            else
            {
                // TODO: Remove this cast after return types are implemented
                var newObj = (Session) await Session.Create(this.attributes, this.options);
                this.attributes = newObj.attributes;
            }
        }

        /// <summary>
        /// Parameters:
        ///   username - string - Username to sign in as
        ///   password - string - Password for sign in
        ///   otp - string - If this user has a 2FA device, provide its OTP or code here.
        ///   partial_session_id - string - Identifier for a partially-completed login
        /// </summary>
        public static async Task<Session> Create(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("username") && !(parameters["username"] is string ))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (parameters.ContainsKey("password") && !(parameters["password"] is string ))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("otp") && !(parameters["otp"] is string ))
            {
                throw new ArgumentException("Bad parameter: otp must be of type string", "parameters[\"otp\"]");
            }
            if (parameters.ContainsKey("partial_session_id") && !(parameters["partial_session_id"] is string ))
            {
                throw new ArgumentException("Bad parameter: partial_session_id must be of type string", "parameters[\"partial_session_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/sessions", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Session>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Session> Delete(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendRequest($"/sessions", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Session>(responseJson);
        }

        public static async Task<Session> Destroy(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(parameters, options);
        }

    }
}

