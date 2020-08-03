using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
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
                this.attributes.Add("protocol", new string[0]);
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
        [JsonPropertyName("protocol")]
        public string[] Protocol
        {
            get { return (string[]) attributes["protocol"]; }
        }

        /// <summary>
        /// Provider name
        /// </summary>
        [JsonPropertyName("provider")]
        public string Provider
        {
            get { return (string) attributes["provider"]; }
        }

        /// <summary>
        /// Custom label for the SSO provider on the login page.
        /// </summary>
        [JsonPropertyName("label")]
        public string Label
        {
            get { return (string) attributes["label"]; }
        }

        /// <summary>
        /// URL holding a custom logo for the SSO provider on the login page.
        /// </summary>
        [JsonPropertyName("logo_url")]
        public string LogoUrl
        {
            get { return (string) attributes["logo_url"]; }
        }

        /// <summary>
        /// ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// Identity provider sha256 cert fingerprint if saml_provider_metadata_url is not available.
        /// </summary>
        [JsonPropertyName("saml_provider_cert_fingerprint")]
        public string SamlProviderCertFingerprint
        {
            get { return (string) attributes["saml_provider_cert_fingerprint"]; }
        }

        /// <summary>
        /// Identity provider issuer url
        /// </summary>
        [JsonPropertyName("saml_provider_issuer_url")]
        public string SamlProviderIssuerUrl
        {
            get { return (string) attributes["saml_provider_issuer_url"]; }
        }

        /// <summary>
        /// Metadata URL for the SAML identity provider
        /// </summary>
        [JsonPropertyName("saml_provider_metadata_url")]
        public string SamlProviderMetadataUrl
        {
            get { return (string) attributes["saml_provider_metadata_url"]; }
        }

        /// <summary>
        /// Identity provider SLO endpoint
        /// </summary>
        [JsonPropertyName("saml_provider_slo_target_url")]
        public string SamlProviderSloTargetUrl
        {
            get { return (string) attributes["saml_provider_slo_target_url"]; }
        }

        /// <summary>
        /// Identity provider SSO endpoint if saml_provider_metadata_url is not available.
        /// </summary>
        [JsonPropertyName("saml_provider_sso_target_url")]
        public string SamlProviderSsoTargetUrl
        {
            get { return (string) attributes["saml_provider_sso_target_url"]; }
        }

        /// <summary>
        /// SCIM authentication type.
        /// </summary>
        [JsonPropertyName("scim_authentication_method")]
        public string ScimAuthenticationMethod
        {
            get { return (string) attributes["scim_authentication_method"]; }
        }

        /// <summary>
        /// SCIM username.
        /// </summary>
        [JsonPropertyName("scim_username")]
        public string ScimUsername
        {
            get { return (string) attributes["scim_username"]; }
        }

        /// <summary>
        /// Subdomain
        /// </summary>
        [JsonPropertyName("subdomain")]
        public string Subdomain
        {
            get { return (string) attributes["subdomain"]; }
        }

        /// <summary>
        /// Auto-provision users?
        /// </summary>
        [JsonPropertyName("provision_users")]
        public bool ProvisionUsers
        {
            get { return (bool) attributes["provision_users"]; }
        }

        /// <summary>
        /// Auto-provision group membership based on group memberships on the SSO side?
        /// </summary>
        [JsonPropertyName("provision_groups")]
        public bool ProvisionGroups
        {
            get { return (bool) attributes["provision_groups"]; }
        }

        /// <summary>
        /// Auto-deprovision users?
        /// </summary>
        [JsonPropertyName("deprovision_users")]
        public bool DeprovisionUsers
        {
            get { return (bool) attributes["deprovision_users"]; }
        }

        /// <summary>
        /// Auto-deprovision group membership based on group memberships on the SSO side?
        /// </summary>
        [JsonPropertyName("deprovision_groups")]
        public bool DeprovisionGroups
        {
            get { return (bool) attributes["deprovision_groups"]; }
        }

        /// <summary>
        /// Comma-separated list of group names for groups to automatically add all auto-provisioned users to.
        /// </summary>
        [JsonPropertyName("provision_group_default")]
        public string ProvisionGroupDefault
        {
            get { return (string) attributes["provision_group_default"]; }
        }

        /// <summary>
        /// Comma-separated list of group names for groups (with optional wildcards) that will be excluded from auto-provisioning.
        /// </summary>
        [JsonPropertyName("provision_group_exclusion")]
        public string ProvisionGroupExclusion
        {
            get { return (string) attributes["provision_group_exclusion"]; }
        }

        /// <summary>
        /// Comma-separated list of group names for groups (with optional wildcards) that will be auto-provisioned.
        /// </summary>
        [JsonPropertyName("provision_group_inclusion")]
        public string ProvisionGroupInclusion
        {
            get { return (string) attributes["provision_group_inclusion"]; }
        }

        /// <summary>
        /// Comma or newline separated list of group names (with optional wildcards) to require membership for user provisioning.
        /// </summary>
        [JsonPropertyName("provision_group_required")]
        public string ProvisionGroupRequired
        {
            get { return (string) attributes["provision_group_required"]; }
        }

        /// <summary>
        /// Auto-provisioned users get Sharing permission?
        /// </summary>
        [JsonPropertyName("provision_attachments_permission")]
        public bool ProvisionAttachmentsPermission
        {
            get { return (bool) attributes["provision_attachments_permission"]; }
        }

        /// <summary>
        /// Auto-provisioned users get WebDAV permission?
        /// </summary>
        [JsonPropertyName("provision_dav_permission")]
        public bool ProvisionDavPermission
        {
            get { return (bool) attributes["provision_dav_permission"]; }
        }

        /// <summary>
        /// Auto-provisioned users get FTP permission?
        /// </summary>
        [JsonPropertyName("provision_ftp_permission")]
        public bool ProvisionFtpPermission
        {
            get { return (bool) attributes["provision_ftp_permission"]; }
        }

        /// <summary>
        /// Auto-provisioned users get SFTP permission?
        /// </summary>
        [JsonPropertyName("provision_sftp_permission")]
        public bool ProvisionSftpPermission
        {
            get { return (bool) attributes["provision_sftp_permission"]; }
        }

        /// <summary>
        /// Default time zone for auto provisioned users.
        /// </summary>
        [JsonPropertyName("provision_time_zone")]
        public string ProvisionTimeZone
        {
            get { return (string) attributes["provision_time_zone"]; }
        }

        /// <summary>
        /// Base DN for looking up users in LDAP server
        /// </summary>
        [JsonPropertyName("ldap_base_dn")]
        public string LdapBaseDn
        {
            get { return (string) attributes["ldap_base_dn"]; }
        }

        /// <summary>
        /// Domain name that will be appended to LDAP usernames
        /// </summary>
        [JsonPropertyName("ldap_domain")]
        public string LdapDomain
        {
            get { return (string) attributes["ldap_domain"]; }
        }

        /// <summary>
        /// Is strategy enabled?
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool Enabled
        {
            get { return (bool) attributes["enabled"]; }
        }

        /// <summary>
        /// LDAP host
        /// </summary>
        [JsonPropertyName("ldap_host")]
        public string LdapHost
        {
            get { return (string) attributes["ldap_host"]; }
        }

        /// <summary>
        /// LDAP backup host
        /// </summary>
        [JsonPropertyName("ldap_host_2")]
        public string LdapHost2
        {
            get { return (string) attributes["ldap_host_2"]; }
        }

        /// <summary>
        /// LDAP backup host
        /// </summary>
        [JsonPropertyName("ldap_host_3")]
        public string LdapHost3
        {
            get { return (string) attributes["ldap_host_3"]; }
        }

        /// <summary>
        /// LDAP port
        /// </summary>
        [JsonPropertyName("ldap_port")]
        public Nullable<Int64> LdapPort
        {
            get { return (Nullable<Int64>) attributes["ldap_port"]; }
        }

        /// <summary>
        /// Use secure LDAP?
        /// </summary>
        [JsonPropertyName("ldap_secure")]
        public bool LdapSecure
        {
            get { return (bool) attributes["ldap_secure"]; }
        }

        /// <summary>
        /// Username for signing in to LDAP server.
        /// </summary>
        [JsonPropertyName("ldap_username")]
        public string LdapUsername
        {
            get { return (string) attributes["ldap_username"]; }
        }

        /// <summary>
        /// LDAP username field
        /// </summary>
        [JsonPropertyName("ldap_username_field")]
        public string LdapUsernameField
        {
            get { return (string) attributes["ldap_username_field"]; }
        }



        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        /// </summary>
        public static async Task<SsoStrategy[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
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
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/sso_strategies/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

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

    }
}

