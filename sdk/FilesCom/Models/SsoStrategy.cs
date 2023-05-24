using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class SsoStrategy
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public SsoStrategy() : this(null, null) { }

        public SsoStrategy(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("protocol"))
            {
                this.attributes.Add("protocol", null);
            }
            if (!this.attributes.ContainsKey("provider"))
            {
                this.attributes.Add("provider", null);
            }
            if (!this.attributes.ContainsKey("label"))
            {
                this.attributes.Add("label", null);
            }
            if (!this.attributes.ContainsKey("logo_url"))
            {
                this.attributes.Add("logo_url", null);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("saml_provider_cert_fingerprint"))
            {
                this.attributes.Add("saml_provider_cert_fingerprint", null);
            }
            if (!this.attributes.ContainsKey("saml_provider_issuer_url"))
            {
                this.attributes.Add("saml_provider_issuer_url", null);
            }
            if (!this.attributes.ContainsKey("saml_provider_metadata_content"))
            {
                this.attributes.Add("saml_provider_metadata_content", null);
            }
            if (!this.attributes.ContainsKey("saml_provider_metadata_url"))
            {
                this.attributes.Add("saml_provider_metadata_url", null);
            }
            if (!this.attributes.ContainsKey("saml_provider_slo_target_url"))
            {
                this.attributes.Add("saml_provider_slo_target_url", null);
            }
            if (!this.attributes.ContainsKey("saml_provider_sso_target_url"))
            {
                this.attributes.Add("saml_provider_sso_target_url", null);
            }
            if (!this.attributes.ContainsKey("scim_authentication_method"))
            {
                this.attributes.Add("scim_authentication_method", null);
            }
            if (!this.attributes.ContainsKey("scim_username"))
            {
                this.attributes.Add("scim_username", null);
            }
            if (!this.attributes.ContainsKey("scim_oauth_access_token"))
            {
                this.attributes.Add("scim_oauth_access_token", null);
            }
            if (!this.attributes.ContainsKey("scim_oauth_access_token_expires_at"))
            {
                this.attributes.Add("scim_oauth_access_token_expires_at", null);
            }
            if (!this.attributes.ContainsKey("subdomain"))
            {
                this.attributes.Add("subdomain", null);
            }
            if (!this.attributes.ContainsKey("provision_users"))
            {
                this.attributes.Add("provision_users", null);
            }
            if (!this.attributes.ContainsKey("provision_groups"))
            {
                this.attributes.Add("provision_groups", null);
            }
            if (!this.attributes.ContainsKey("deprovision_users"))
            {
                this.attributes.Add("deprovision_users", null);
            }
            if (!this.attributes.ContainsKey("deprovision_groups"))
            {
                this.attributes.Add("deprovision_groups", null);
            }
            if (!this.attributes.ContainsKey("deprovision_behavior"))
            {
                this.attributes.Add("deprovision_behavior", null);
            }
            if (!this.attributes.ContainsKey("provision_group_default"))
            {
                this.attributes.Add("provision_group_default", null);
            }
            if (!this.attributes.ContainsKey("provision_group_exclusion"))
            {
                this.attributes.Add("provision_group_exclusion", null);
            }
            if (!this.attributes.ContainsKey("provision_group_inclusion"))
            {
                this.attributes.Add("provision_group_inclusion", null);
            }
            if (!this.attributes.ContainsKey("provision_group_required"))
            {
                this.attributes.Add("provision_group_required", null);
            }
            if (!this.attributes.ContainsKey("provision_email_signup_groups"))
            {
                this.attributes.Add("provision_email_signup_groups", null);
            }
            if (!this.attributes.ContainsKey("provision_site_admin_groups"))
            {
                this.attributes.Add("provision_site_admin_groups", null);
            }
            if (!this.attributes.ContainsKey("provision_group_admin_groups"))
            {
                this.attributes.Add("provision_group_admin_groups", null);
            }
            if (!this.attributes.ContainsKey("provision_attachments_permission"))
            {
                this.attributes.Add("provision_attachments_permission", null);
            }
            if (!this.attributes.ContainsKey("provision_dav_permission"))
            {
                this.attributes.Add("provision_dav_permission", null);
            }
            if (!this.attributes.ContainsKey("provision_ftp_permission"))
            {
                this.attributes.Add("provision_ftp_permission", null);
            }
            if (!this.attributes.ContainsKey("provision_sftp_permission"))
            {
                this.attributes.Add("provision_sftp_permission", null);
            }
            if (!this.attributes.ContainsKey("provision_time_zone"))
            {
                this.attributes.Add("provision_time_zone", null);
            }
            if (!this.attributes.ContainsKey("provision_company"))
            {
                this.attributes.Add("provision_company", null);
            }
            if (!this.attributes.ContainsKey("ldap_base_dn"))
            {
                this.attributes.Add("ldap_base_dn", null);
            }
            if (!this.attributes.ContainsKey("ldap_domain"))
            {
                this.attributes.Add("ldap_domain", null);
            }
            if (!this.attributes.ContainsKey("enabled"))
            {
                this.attributes.Add("enabled", null);
            }
            if (!this.attributes.ContainsKey("ldap_host"))
            {
                this.attributes.Add("ldap_host", null);
            }
            if (!this.attributes.ContainsKey("ldap_host_2"))
            {
                this.attributes.Add("ldap_host_2", null);
            }
            if (!this.attributes.ContainsKey("ldap_host_3"))
            {
                this.attributes.Add("ldap_host_3", null);
            }
            if (!this.attributes.ContainsKey("ldap_port"))
            {
                this.attributes.Add("ldap_port", null);
            }
            if (!this.attributes.ContainsKey("ldap_secure"))
            {
                this.attributes.Add("ldap_secure", null);
            }
            if (!this.attributes.ContainsKey("ldap_username"))
            {
                this.attributes.Add("ldap_username", null);
            }
            if (!this.attributes.ContainsKey("ldap_username_field"))
            {
                this.attributes.Add("ldap_username_field", null);
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
        /// SSO Protocol
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("protocol")]
        public string Protocol
        {
            get { return (string)attributes["protocol"]; }
            private set { attributes["protocol"] = value; }
        }

        /// <summary>
        /// Provider name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provider")]
        public string Provider
        {
            get { return (string)attributes["provider"]; }
            private set { attributes["provider"] = value; }
        }

        /// <summary>
        /// Custom label for the SSO provider on the login page.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("label")]
        public string Label
        {
            get { return (string)attributes["label"]; }
            private set { attributes["label"] = value; }
        }

        /// <summary>
        /// URL holding a custom logo for the SSO provider on the login page.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logo_url")]
        public string LogoUrl
        {
            get { return (string)attributes["logo_url"]; }
            private set { attributes["logo_url"] = value; }
        }

        /// <summary>
        /// ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Identity provider sha256 cert fingerprint if saml_provider_metadata_url is not available.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("saml_provider_cert_fingerprint")]
        public string SamlProviderCertFingerprint
        {
            get { return (string)attributes["saml_provider_cert_fingerprint"]; }
            private set { attributes["saml_provider_cert_fingerprint"] = value; }
        }

        /// <summary>
        /// Identity provider issuer url
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("saml_provider_issuer_url")]
        public string SamlProviderIssuerUrl
        {
            get { return (string)attributes["saml_provider_issuer_url"]; }
            private set { attributes["saml_provider_issuer_url"] = value; }
        }

        /// <summary>
        /// Custom identity provider metadata
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("saml_provider_metadata_content")]
        public string SamlProviderMetadataContent
        {
            get { return (string)attributes["saml_provider_metadata_content"]; }
            private set { attributes["saml_provider_metadata_content"] = value; }
        }

        /// <summary>
        /// Metadata URL for the SAML identity provider
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("saml_provider_metadata_url")]
        public string SamlProviderMetadataUrl
        {
            get { return (string)attributes["saml_provider_metadata_url"]; }
            private set { attributes["saml_provider_metadata_url"] = value; }
        }

        /// <summary>
        /// Identity provider SLO endpoint
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("saml_provider_slo_target_url")]
        public string SamlProviderSloTargetUrl
        {
            get { return (string)attributes["saml_provider_slo_target_url"]; }
            private set { attributes["saml_provider_slo_target_url"] = value; }
        }

        /// <summary>
        /// Identity provider SSO endpoint if saml_provider_metadata_url is not available.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("saml_provider_sso_target_url")]
        public string SamlProviderSsoTargetUrl
        {
            get { return (string)attributes["saml_provider_sso_target_url"]; }
            private set { attributes["saml_provider_sso_target_url"] = value; }
        }

        /// <summary>
        /// SCIM authentication type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("scim_authentication_method")]
        public string ScimAuthenticationMethod
        {
            get { return (string)attributes["scim_authentication_method"]; }
            private set { attributes["scim_authentication_method"] = value; }
        }

        /// <summary>
        /// SCIM username.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("scim_username")]
        public string ScimUsername
        {
            get { return (string)attributes["scim_username"]; }
            private set { attributes["scim_username"] = value; }
        }

        /// <summary>
        /// SCIM OAuth Access Token.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("scim_oauth_access_token")]
        public string ScimOauthAccessToken
        {
            get { return (string)attributes["scim_oauth_access_token"]; }
            private set { attributes["scim_oauth_access_token"] = value; }
        }

        /// <summary>
        /// SCIM OAuth Access Token Expiration Time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("scim_oauth_access_token_expires_at")]
        public string ScimOauthAccessTokenExpiresAt
        {
            get { return (string)attributes["scim_oauth_access_token_expires_at"]; }
            private set { attributes["scim_oauth_access_token_expires_at"] = value; }
        }

        /// <summary>
        /// Subdomain
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("subdomain")]
        public string Subdomain
        {
            get { return (string)attributes["subdomain"]; }
            private set { attributes["subdomain"] = value; }
        }

        /// <summary>
        /// Auto-provision users?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_users")]
        public bool ProvisionUsers
        {
            get { return (bool)attributes["provision_users"]; }
            private set { attributes["provision_users"] = value; }
        }

        /// <summary>
        /// Auto-provision group membership based on group memberships on the SSO side?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_groups")]
        public bool ProvisionGroups
        {
            get { return (bool)attributes["provision_groups"]; }
            private set { attributes["provision_groups"] = value; }
        }

        /// <summary>
        /// Auto-deprovision users?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("deprovision_users")]
        public bool DeprovisionUsers
        {
            get { return (bool)attributes["deprovision_users"]; }
            private set { attributes["deprovision_users"] = value; }
        }

        /// <summary>
        /// Auto-deprovision group membership based on group memberships on the SSO side?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("deprovision_groups")]
        public bool DeprovisionGroups
        {
            get { return (bool)attributes["deprovision_groups"]; }
            private set { attributes["deprovision_groups"] = value; }
        }

        /// <summary>
        /// Method used for deprovisioning users.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("deprovision_behavior")]
        public string DeprovisionBehavior
        {
            get { return (string)attributes["deprovision_behavior"]; }
            private set { attributes["deprovision_behavior"] = value; }
        }

        /// <summary>
        /// Comma-separated list of group names for groups to automatically add all auto-provisioned users to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_group_default")]
        public string ProvisionGroupDefault
        {
            get { return (string)attributes["provision_group_default"]; }
            private set { attributes["provision_group_default"] = value; }
        }

        /// <summary>
        /// Comma-separated list of group names for groups (with optional wildcards) that will be excluded from auto-provisioning.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_group_exclusion")]
        public string ProvisionGroupExclusion
        {
            get { return (string)attributes["provision_group_exclusion"]; }
            private set { attributes["provision_group_exclusion"] = value; }
        }

        /// <summary>
        /// Comma-separated list of group names for groups (with optional wildcards) that will be auto-provisioned.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_group_inclusion")]
        public string ProvisionGroupInclusion
        {
            get { return (string)attributes["provision_group_inclusion"]; }
            private set { attributes["provision_group_inclusion"] = value; }
        }

        /// <summary>
        /// Comma or newline separated list of group names (with optional wildcards) to require membership for user provisioning.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_group_required")]
        public string ProvisionGroupRequired
        {
            get { return (string)attributes["provision_group_required"]; }
            private set { attributes["provision_group_required"] = value; }
        }

        /// <summary>
        /// Comma-separated list of group names whose members will be created with email_signup authentication.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_email_signup_groups")]
        public string ProvisionEmailSignupGroups
        {
            get { return (string)attributes["provision_email_signup_groups"]; }
            private set { attributes["provision_email_signup_groups"] = value; }
        }

        /// <summary>
        /// Comma-separated list of group names whose members will be created as Site Admins.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_site_admin_groups")]
        public string ProvisionSiteAdminGroups
        {
            get { return (string)attributes["provision_site_admin_groups"]; }
            private set { attributes["provision_site_admin_groups"] = value; }
        }

        /// <summary>
        /// Comma-separated list of group names whose members will be provisioned as Group Admins.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_group_admin_groups")]
        public string ProvisionGroupAdminGroups
        {
            get { return (string)attributes["provision_group_admin_groups"]; }
            private set { attributes["provision_group_admin_groups"] = value; }
        }

        /// <summary>
        /// DEPRECATED: Auto-provisioned users get Sharing permission. Use a Group with the Bundle permission instead.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_attachments_permission")]
        public bool ProvisionAttachmentsPermission
        {
            get { return (bool)attributes["provision_attachments_permission"]; }
            private set { attributes["provision_attachments_permission"] = value; }
        }

        /// <summary>
        /// Auto-provisioned users get WebDAV permission?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_dav_permission")]
        public bool ProvisionDavPermission
        {
            get { return (bool)attributes["provision_dav_permission"]; }
            private set { attributes["provision_dav_permission"] = value; }
        }

        /// <summary>
        /// Auto-provisioned users get FTP permission?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_ftp_permission")]
        public bool ProvisionFtpPermission
        {
            get { return (bool)attributes["provision_ftp_permission"]; }
            private set { attributes["provision_ftp_permission"] = value; }
        }

        /// <summary>
        /// Auto-provisioned users get SFTP permission?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_sftp_permission")]
        public bool ProvisionSftpPermission
        {
            get { return (bool)attributes["provision_sftp_permission"]; }
            private set { attributes["provision_sftp_permission"] = value; }
        }

        /// <summary>
        /// Default time zone for auto provisioned users.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_time_zone")]
        public string ProvisionTimeZone
        {
            get { return (string)attributes["provision_time_zone"]; }
            private set { attributes["provision_time_zone"] = value; }
        }

        /// <summary>
        /// Default company for auto provisioned users.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("provision_company")]
        public string ProvisionCompany
        {
            get { return (string)attributes["provision_company"]; }
            private set { attributes["provision_company"] = value; }
        }

        /// <summary>
        /// Base DN for looking up users in LDAP server
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_base_dn")]
        public string LdapBaseDn
        {
            get { return (string)attributes["ldap_base_dn"]; }
            private set { attributes["ldap_base_dn"] = value; }
        }

        /// <summary>
        /// Domain name that will be appended to LDAP usernames
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_domain")]
        public string LdapDomain
        {
            get { return (string)attributes["ldap_domain"]; }
            private set { attributes["ldap_domain"] = value; }
        }

        /// <summary>
        /// Is strategy enabled?  This may become automatically set to `false` after a high number and duration of failures.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("enabled")]
        public bool Enabled
        {
            get { return (bool)attributes["enabled"]; }
            private set { attributes["enabled"] = value; }
        }

        /// <summary>
        /// LDAP host
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_host")]
        public string LdapHost
        {
            get { return (string)attributes["ldap_host"]; }
            private set { attributes["ldap_host"] = value; }
        }

        /// <summary>
        /// LDAP backup host
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_host_2")]
        public string LdapHost2
        {
            get { return (string)attributes["ldap_host_2"]; }
            private set { attributes["ldap_host_2"] = value; }
        }

        /// <summary>
        /// LDAP backup host
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_host_3")]
        public string LdapHost3
        {
            get { return (string)attributes["ldap_host_3"]; }
            private set { attributes["ldap_host_3"] = value; }
        }

        /// <summary>
        /// LDAP port
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_port")]
        public Nullable<Int64> LdapPort
        {
            get { return (Nullable<Int64>)attributes["ldap_port"]; }
            private set { attributes["ldap_port"] = value; }
        }

        /// <summary>
        /// Use secure LDAP?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_secure")]
        public bool LdapSecure
        {
            get { return (bool)attributes["ldap_secure"]; }
            private set { attributes["ldap_secure"] = value; }
        }

        /// <summary>
        /// Username for signing in to LDAP server.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_username")]
        public string LdapUsername
        {
            get { return (string)attributes["ldap_username"]; }
            private set { attributes["ldap_username"] = value; }
        }

        /// <summary>
        /// LDAP username field
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_username_field")]
        public string LdapUsernameField
        {
            get { return (string)attributes["ldap_username_field"]; }
            private set { attributes["ldap_username_field"] = value; }
        }

        /// <summary>
        /// Synchronize provisioning data with the SSO remote server
        /// </summary>
        public async Task<SsoStrategy> Sync(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/sso_strategies/{System.Uri.EscapeDataString(attributes["id"].ToString())}/sync", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<SsoStrategy>(responseJson);
        }




        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<SsoStrategy[]> List(

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

            string responseJson = await FilesClient.SendRequest($"/sso_strategies", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<SsoStrategy[]>(responseJson);
        }

        public static async Task<SsoStrategy[]> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sso Strategy ID.
        /// </summary>
        public static async Task<SsoStrategy> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/sso_strategies/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<SsoStrategy>(responseJson);
        }

        public static async Task<SsoStrategy> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Synchronize provisioning data with the SSO remote server
        /// </summary>
        public static async Task<SsoStrategy> Sync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/sso_strategies/{System.Uri.EscapeDataString(parameters["id"].ToString())}/sync", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<SsoStrategy>(responseJson);
        }


    }
}