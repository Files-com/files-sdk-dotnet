using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Site
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Site() : this(null, null) { }

        public Site(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
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
            if (!this.attributes.ContainsKey("admin_user_id"))
            {
                this.attributes.Add("admin_user_id", null);
            }
            if (!this.attributes.ContainsKey("allow_bundle_names"))
            {
                this.attributes.Add("allow_bundle_names", null);
            }
            if (!this.attributes.ContainsKey("allowed_countries"))
            {
                this.attributes.Add("allowed_countries", null);
            }
            if (!this.attributes.ContainsKey("allowed_ips"))
            {
                this.attributes.Add("allowed_ips", null);
            }
            if (!this.attributes.ContainsKey("ask_about_overwrites"))
            {
                this.attributes.Add("ask_about_overwrites", null);
            }
            if (!this.attributes.ContainsKey("bundle_expiration"))
            {
                this.attributes.Add("bundle_expiration", null);
            }
            if (!this.attributes.ContainsKey("bundle_password_required"))
            {
                this.attributes.Add("bundle_password_required", null);
            }
            if (!this.attributes.ContainsKey("bundle_require_share_recipient"))
            {
                this.attributes.Add("bundle_require_share_recipient", null);
            }
            if (!this.attributes.ContainsKey("color2_left"))
            {
                this.attributes.Add("color2_left", null);
            }
            if (!this.attributes.ContainsKey("color2_link"))
            {
                this.attributes.Add("color2_link", null);
            }
            if (!this.attributes.ContainsKey("color2_text"))
            {
                this.attributes.Add("color2_text", null);
            }
            if (!this.attributes.ContainsKey("color2_top"))
            {
                this.attributes.Add("color2_top", null);
            }
            if (!this.attributes.ContainsKey("color2_top_text"))
            {
                this.attributes.Add("color2_top_text", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("currency"))
            {
                this.attributes.Add("currency", null);
            }
            if (!this.attributes.ContainsKey("custom_namespace"))
            {
                this.attributes.Add("custom_namespace", null);
            }
            if (!this.attributes.ContainsKey("days_to_retain_backups"))
            {
                this.attributes.Add("days_to_retain_backups", null);
            }
            if (!this.attributes.ContainsKey("default_time_zone"))
            {
                this.attributes.Add("default_time_zone", null);
            }
            if (!this.attributes.ContainsKey("desktop_app"))
            {
                this.attributes.Add("desktop_app", null);
            }
            if (!this.attributes.ContainsKey("desktop_app_session_ip_pinning"))
            {
                this.attributes.Add("desktop_app_session_ip_pinning", null);
            }
            if (!this.attributes.ContainsKey("desktop_app_session_lifetime"))
            {
                this.attributes.Add("desktop_app_session_lifetime", null);
            }
            if (!this.attributes.ContainsKey("disallowed_countries"))
            {
                this.attributes.Add("disallowed_countries", null);
            }
            if (!this.attributes.ContainsKey("disable_notifications"))
            {
                this.attributes.Add("disable_notifications", null);
            }
            if (!this.attributes.ContainsKey("disable_password_reset"))
            {
                this.attributes.Add("disable_password_reset", null);
            }
            if (!this.attributes.ContainsKey("domain"))
            {
                this.attributes.Add("domain", null);
            }
            if (!this.attributes.ContainsKey("email"))
            {
                this.attributes.Add("email", null);
            }
            if (!this.attributes.ContainsKey("reply_to_email"))
            {
                this.attributes.Add("reply_to_email", null);
            }
            if (!this.attributes.ContainsKey("non_sso_groups_allowed"))
            {
                this.attributes.Add("non_sso_groups_allowed", null);
            }
            if (!this.attributes.ContainsKey("non_sso_users_allowed"))
            {
                this.attributes.Add("non_sso_users_allowed", null);
            }
            if (!this.attributes.ContainsKey("folder_permissions_groups_only"))
            {
                this.attributes.Add("folder_permissions_groups_only", null);
            }
            if (!this.attributes.ContainsKey("hipaa"))
            {
                this.attributes.Add("hipaa", null);
            }
            if (!this.attributes.ContainsKey("icon128"))
            {
                this.attributes.Add("icon128", null);
            }
            if (!this.attributes.ContainsKey("icon16"))
            {
                this.attributes.Add("icon16", null);
            }
            if (!this.attributes.ContainsKey("icon32"))
            {
                this.attributes.Add("icon32", null);
            }
            if (!this.attributes.ContainsKey("icon48"))
            {
                this.attributes.Add("icon48", null);
            }
            if (!this.attributes.ContainsKey("immutable_files_set_at"))
            {
                this.attributes.Add("immutable_files_set_at", null);
            }
            if (!this.attributes.ContainsKey("include_password_in_welcome_email"))
            {
                this.attributes.Add("include_password_in_welcome_email", null);
            }
            if (!this.attributes.ContainsKey("language"))
            {
                this.attributes.Add("language", null);
            }
            if (!this.attributes.ContainsKey("ldap_base_dn"))
            {
                this.attributes.Add("ldap_base_dn", null);
            }
            if (!this.attributes.ContainsKey("ldap_domain"))
            {
                this.attributes.Add("ldap_domain", null);
            }
            if (!this.attributes.ContainsKey("ldap_enabled"))
            {
                this.attributes.Add("ldap_enabled", null);
            }
            if (!this.attributes.ContainsKey("ldap_group_action"))
            {
                this.attributes.Add("ldap_group_action", null);
            }
            if (!this.attributes.ContainsKey("ldap_group_exclusion"))
            {
                this.attributes.Add("ldap_group_exclusion", null);
            }
            if (!this.attributes.ContainsKey("ldap_group_inclusion"))
            {
                this.attributes.Add("ldap_group_inclusion", null);
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
            if (!this.attributes.ContainsKey("ldap_type"))
            {
                this.attributes.Add("ldap_type", null);
            }
            if (!this.attributes.ContainsKey("ldap_user_action"))
            {
                this.attributes.Add("ldap_user_action", null);
            }
            if (!this.attributes.ContainsKey("ldap_user_include_groups"))
            {
                this.attributes.Add("ldap_user_include_groups", null);
            }
            if (!this.attributes.ContainsKey("ldap_username"))
            {
                this.attributes.Add("ldap_username", null);
            }
            if (!this.attributes.ContainsKey("ldap_username_field"))
            {
                this.attributes.Add("ldap_username_field", null);
            }
            if (!this.attributes.ContainsKey("login_help_text"))
            {
                this.attributes.Add("login_help_text", null);
            }
            if (!this.attributes.ContainsKey("logo"))
            {
                this.attributes.Add("logo", null);
            }
            if (!this.attributes.ContainsKey("max_prior_passwords"))
            {
                this.attributes.Add("max_prior_passwords", null);
            }
            if (!this.attributes.ContainsKey("next_billing_amount"))
            {
                this.attributes.Add("next_billing_amount", null);
            }
            if (!this.attributes.ContainsKey("next_billing_date"))
            {
                this.attributes.Add("next_billing_date", null);
            }
            if (!this.attributes.ContainsKey("office_integration_available"))
            {
                this.attributes.Add("office_integration_available", null);
            }
            if (!this.attributes.ContainsKey("oncehub_link"))
            {
                this.attributes.Add("oncehub_link", null);
            }
            if (!this.attributes.ContainsKey("opt_out_global"))
            {
                this.attributes.Add("opt_out_global", null);
            }
            if (!this.attributes.ContainsKey("overage_notified_at"))
            {
                this.attributes.Add("overage_notified_at", null);
            }
            if (!this.attributes.ContainsKey("overage_notify"))
            {
                this.attributes.Add("overage_notify", null);
            }
            if (!this.attributes.ContainsKey("overdue"))
            {
                this.attributes.Add("overdue", null);
            }
            if (!this.attributes.ContainsKey("password_min_length"))
            {
                this.attributes.Add("password_min_length", null);
            }
            if (!this.attributes.ContainsKey("password_require_letter"))
            {
                this.attributes.Add("password_require_letter", null);
            }
            if (!this.attributes.ContainsKey("password_require_mixed"))
            {
                this.attributes.Add("password_require_mixed", null);
            }
            if (!this.attributes.ContainsKey("password_require_number"))
            {
                this.attributes.Add("password_require_number", null);
            }
            if (!this.attributes.ContainsKey("password_require_special"))
            {
                this.attributes.Add("password_require_special", null);
            }
            if (!this.attributes.ContainsKey("password_require_unbreached"))
            {
                this.attributes.Add("password_require_unbreached", null);
            }
            if (!this.attributes.ContainsKey("password_requirements_apply_to_bundles"))
            {
                this.attributes.Add("password_requirements_apply_to_bundles", null);
            }
            if (!this.attributes.ContainsKey("password_validity_days"))
            {
                this.attributes.Add("password_validity_days", null);
            }
            if (!this.attributes.ContainsKey("phone"))
            {
                this.attributes.Add("phone", null);
            }
            if (!this.attributes.ContainsKey("require_2fa"))
            {
                this.attributes.Add("require_2fa", null);
            }
            if (!this.attributes.ContainsKey("require_2fa_stop_time"))
            {
                this.attributes.Add("require_2fa_stop_time", null);
            }
            if (!this.attributes.ContainsKey("require_2fa_user_type"))
            {
                this.attributes.Add("require_2fa_user_type", null);
            }
            if (!this.attributes.ContainsKey("session"))
            {
                this.attributes.Add("session", null);
            }
            if (!this.attributes.ContainsKey("session_pinned_by_ip"))
            {
                this.attributes.Add("session_pinned_by_ip", null);
            }
            if (!this.attributes.ContainsKey("sftp_user_root_enabled"))
            {
                this.attributes.Add("sftp_user_root_enabled", null);
            }
            if (!this.attributes.ContainsKey("sharing_enabled"))
            {
                this.attributes.Add("sharing_enabled", null);
            }
            if (!this.attributes.ContainsKey("show_request_access_link"))
            {
                this.attributes.Add("show_request_access_link", null);
            }
            if (!this.attributes.ContainsKey("site_footer"))
            {
                this.attributes.Add("site_footer", null);
            }
            if (!this.attributes.ContainsKey("site_header"))
            {
                this.attributes.Add("site_header", null);
            }
            if (!this.attributes.ContainsKey("smtp_address"))
            {
                this.attributes.Add("smtp_address", null);
            }
            if (!this.attributes.ContainsKey("smtp_authentication"))
            {
                this.attributes.Add("smtp_authentication", null);
            }
            if (!this.attributes.ContainsKey("smtp_from"))
            {
                this.attributes.Add("smtp_from", null);
            }
            if (!this.attributes.ContainsKey("smtp_port"))
            {
                this.attributes.Add("smtp_port", null);
            }
            if (!this.attributes.ContainsKey("smtp_username"))
            {
                this.attributes.Add("smtp_username", null);
            }
            if (!this.attributes.ContainsKey("session_expiry"))
            {
                this.attributes.Add("session_expiry", null);
            }
            if (!this.attributes.ContainsKey("ssl_required"))
            {
                this.attributes.Add("ssl_required", null);
            }
            if (!this.attributes.ContainsKey("subdomain"))
            {
                this.attributes.Add("subdomain", null);
            }
            if (!this.attributes.ContainsKey("switch_to_plan_date"))
            {
                this.attributes.Add("switch_to_plan_date", null);
            }
            if (!this.attributes.ContainsKey("tls_disabled"))
            {
                this.attributes.Add("tls_disabled", null);
            }
            if (!this.attributes.ContainsKey("trial_days_left"))
            {
                this.attributes.Add("trial_days_left", null);
            }
            if (!this.attributes.ContainsKey("trial_until"))
            {
                this.attributes.Add("trial_until", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
            }
            if (!this.attributes.ContainsKey("use_provided_modified_at"))
            {
                this.attributes.Add("use_provided_modified_at", null);
            }
            if (!this.attributes.ContainsKey("user"))
            {
                this.attributes.Add("user", null);
            }
            if (!this.attributes.ContainsKey("user_lockout"))
            {
                this.attributes.Add("user_lockout", null);
            }
            if (!this.attributes.ContainsKey("user_lockout_lock_period"))
            {
                this.attributes.Add("user_lockout_lock_period", null);
            }
            if (!this.attributes.ContainsKey("user_lockout_tries"))
            {
                this.attributes.Add("user_lockout_tries", null);
            }
            if (!this.attributes.ContainsKey("user_lockout_within"))
            {
                this.attributes.Add("user_lockout_within", null);
            }
            if (!this.attributes.ContainsKey("user_requests_enabled"))
            {
                this.attributes.Add("user_requests_enabled", null);
            }
            if (!this.attributes.ContainsKey("welcome_custom_text"))
            {
                this.attributes.Add("welcome_custom_text", null);
            }
            if (!this.attributes.ContainsKey("welcome_email_cc"))
            {
                this.attributes.Add("welcome_email_cc", null);
            }
            if (!this.attributes.ContainsKey("welcome_email_enabled"))
            {
                this.attributes.Add("welcome_email_enabled", null);
            }
            if (!this.attributes.ContainsKey("welcome_screen"))
            {
                this.attributes.Add("welcome_screen", null);
            }
            if (!this.attributes.ContainsKey("windows_mode_ftp"))
            {
                this.attributes.Add("windows_mode_ftp", null);
            }
            if (!this.attributes.ContainsKey("disable_users_from_inactivity_period_days"))
            {
                this.attributes.Add("disable_users_from_inactivity_period_days", null);
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
        /// Site name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string) attributes["name"]; }
        }

        /// <summary>
        /// Is SMS two factor authentication allowed?
        /// </summary>
        [JsonPropertyName("allowed_2fa_method_sms")]
        public bool Allowed2faMethodSms
        {
            get { return (bool) attributes["allowed_2fa_method_sms"]; }
        }

        /// <summary>
        /// Is TOTP two factor authentication allowed?
        /// </summary>
        [JsonPropertyName("allowed_2fa_method_totp")]
        public bool Allowed2faMethodTotp
        {
            get { return (bool) attributes["allowed_2fa_method_totp"]; }
        }

        /// <summary>
        /// Is U2F two factor authentication allowed?
        /// </summary>
        [JsonPropertyName("allowed_2fa_method_u2f")]
        public bool Allowed2faMethodU2f
        {
            get { return (bool) attributes["allowed_2fa_method_u2f"]; }
        }

        /// <summary>
        /// Is yubikey two factor authentication allowed?
        /// </summary>
        [JsonPropertyName("allowed_2fa_method_yubi")]
        public bool Allowed2faMethodYubi
        {
            get { return (bool) attributes["allowed_2fa_method_yubi"]; }
        }

        /// <summary>
        /// User ID for the main site administrator
        /// </summary>
        [JsonPropertyName("admin_user_id")]
        public Nullable<Int64> AdminUserId
        {
            get { return (Nullable<Int64>) attributes["admin_user_id"]; }
        }

        /// <summary>
        /// Are manual Bundle names allowed?
        /// </summary>
        [JsonPropertyName("allow_bundle_names")]
        public bool AllowBundleNames
        {
            get { return (bool) attributes["allow_bundle_names"]; }
        }

        /// <summary>
        /// Comma seperated list of allowed Country codes
        /// </summary>
        [JsonPropertyName("allowed_countries")]
        public string AllowedCountries
        {
            get { return (string) attributes["allowed_countries"]; }
        }

        /// <summary>
        /// List of allowed IP addresses
        /// </summary>
        [JsonPropertyName("allowed_ips")]
        public string AllowedIps
        {
            get { return (string) attributes["allowed_ips"]; }
        }

        /// <summary>
        /// If false, rename conflicting files instead of asking for overwrite confirmation.  Only applies to web interface.
        /// </summary>
        [JsonPropertyName("ask_about_overwrites")]
        public bool AskAboutOverwrites
        {
            get { return (bool) attributes["ask_about_overwrites"]; }
        }

        /// <summary>
        /// Site-wide Bundle expiration in days
        /// </summary>
        [JsonPropertyName("bundle_expiration")]
        public Nullable<Int64> BundleExpiration
        {
            get { return (Nullable<Int64>) attributes["bundle_expiration"]; }
        }

        /// <summary>
        /// Do Bundles require password protection?
        /// </summary>
        [JsonPropertyName("bundle_password_required")]
        public bool BundlePasswordRequired
        {
            get { return (bool) attributes["bundle_password_required"]; }
        }

        /// <summary>
        /// Do Bundles require recipients for sharing?
        /// </summary>
        [JsonPropertyName("bundle_require_share_recipient")]
        public bool BundleRequireShareRecipient
        {
            get { return (bool) attributes["bundle_require_share_recipient"]; }
        }

        /// <summary>
        /// Page link and button color
        /// </summary>
        [JsonPropertyName("color2_left")]
        public string Color2Left
        {
            get { return (string) attributes["color2_left"]; }
        }

        /// <summary>
        /// Top bar link color
        /// </summary>
        [JsonPropertyName("color2_link")]
        public string Color2Link
        {
            get { return (string) attributes["color2_link"]; }
        }

        /// <summary>
        /// Page link and button color
        /// </summary>
        [JsonPropertyName("color2_text")]
        public string Color2Text
        {
            get { return (string) attributes["color2_text"]; }
        }

        /// <summary>
        /// Top bar background color
        /// </summary>
        [JsonPropertyName("color2_top")]
        public string Color2Top
        {
            get { return (string) attributes["color2_top"]; }
        }

        /// <summary>
        /// Top bar text color
        /// </summary>
        [JsonPropertyName("color2_top_text")]
        public string Color2TopText
        {
            get { return (string) attributes["color2_top_text"]; }
        }

        /// <summary>
        /// Time this site was created
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
        }

        /// <summary>
        /// Preferred currency
        /// </summary>
        [JsonPropertyName("currency")]
        public string Currency
        {
            get { return (string) attributes["currency"]; }
        }

        /// <summary>
        /// Is this site using a custom namespace for users?
        /// </summary>
        [JsonPropertyName("custom_namespace")]
        public bool CustomNamespace
        {
            get { return (bool) attributes["custom_namespace"]; }
        }

        /// <summary>
        /// Number of days to keep deleted files
        /// </summary>
        [JsonPropertyName("days_to_retain_backups")]
        public Nullable<Int64> DaysToRetainBackups
        {
            get { return (Nullable<Int64>) attributes["days_to_retain_backups"]; }
        }

        /// <summary>
        /// Site default time zone
        /// </summary>
        [JsonPropertyName("default_time_zone")]
        public string DefaultTimeZone
        {
            get { return (string) attributes["default_time_zone"]; }
        }

        /// <summary>
        /// Is the desktop app enabled?
        /// </summary>
        [JsonPropertyName("desktop_app")]
        public bool DesktopApp
        {
            get { return (bool) attributes["desktop_app"]; }
        }

        /// <summary>
        /// Is desktop app session IP pinning enabled?
        /// </summary>
        [JsonPropertyName("desktop_app_session_ip_pinning")]
        public bool DesktopAppSessionIpPinning
        {
            get { return (bool) attributes["desktop_app_session_ip_pinning"]; }
        }

        /// <summary>
        /// Desktop app session lifetime (in hours)
        /// </summary>
        [JsonPropertyName("desktop_app_session_lifetime")]
        public Nullable<Int64> DesktopAppSessionLifetime
        {
            get { return (Nullable<Int64>) attributes["desktop_app_session_lifetime"]; }
        }

        /// <summary>
        /// Comma seperated list of disallowed Country codes
        /// </summary>
        [JsonPropertyName("disallowed_countries")]
        public string DisallowedCountries
        {
            get { return (string) attributes["disallowed_countries"]; }
        }

        /// <summary>
        /// Are notifications disabled?
        /// </summary>
        [JsonPropertyName("disable_notifications")]
        public bool DisableNotifications
        {
            get { return (bool) attributes["disable_notifications"]; }
        }

        /// <summary>
        /// Is password reset disabled?
        /// </summary>
        [JsonPropertyName("disable_password_reset")]
        public bool DisablePasswordReset
        {
            get { return (bool) attributes["disable_password_reset"]; }
        }

        /// <summary>
        /// Custom domain
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain
        {
            get { return (string) attributes["domain"]; }
        }

        /// <summary>
        /// Main email for this site
        /// </summary>
        [JsonPropertyName("email")]
        public string Email
        {
            get { return (string) attributes["email"]; }
        }

        /// <summary>
        /// Reply-to email for this site
        /// </summary>
        [JsonPropertyName("reply_to_email")]
        public string ReplyToEmail
        {
            get { return (string) attributes["reply_to_email"]; }
        }

        /// <summary>
        /// If true, groups can be manually created / modified / deleted by Site Admins. Otherwise, groups can only be managed via your SSO provider.
        /// </summary>
        [JsonPropertyName("non_sso_groups_allowed")]
        public bool NonSsoGroupsAllowed
        {
            get { return (bool) attributes["non_sso_groups_allowed"]; }
        }

        /// <summary>
        /// If true, users can be manually created / modified / deleted by Site Admins. Otherwise, users can only be managed via your SSO provider.
        /// </summary>
        [JsonPropertyName("non_sso_users_allowed")]
        public bool NonSsoUsersAllowed
        {
            get { return (bool) attributes["non_sso_users_allowed"]; }
        }

        /// <summary>
        /// If true, permissions for this site must be bound to a group (not a user). Otherwise, permissions must be bound to a user.
        /// </summary>
        [JsonPropertyName("folder_permissions_groups_only")]
        public bool FolderPermissionsGroupsOnly
        {
            get { return (bool) attributes["folder_permissions_groups_only"]; }
        }

        /// <summary>
        /// Is there a signed HIPAA BAA between Files.com and this site?
        /// </summary>
        [JsonPropertyName("hipaa")]
        public bool Hipaa
        {
            get { return (bool) attributes["hipaa"]; }
        }

        /// <summary>
        /// Branded icon 128x128
        /// </summary>
        [JsonPropertyName("icon128")]
        public object Icon128
        {
            get { return (object) attributes["icon128"]; }
        }

        /// <summary>
        /// Branded icon 16x16
        /// </summary>
        [JsonPropertyName("icon16")]
        public object Icon16
        {
            get { return (object) attributes["icon16"]; }
        }

        /// <summary>
        /// Branded icon 32x32
        /// </summary>
        [JsonPropertyName("icon32")]
        public object Icon32
        {
            get { return (object) attributes["icon32"]; }
        }

        /// <summary>
        /// Branded icon 48x48
        /// </summary>
        [JsonPropertyName("icon48")]
        public object Icon48
        {
            get { return (object) attributes["icon48"]; }
        }

        /// <summary>
        /// Can files be modified?
        /// </summary>
        [JsonPropertyName("immutable_files_set_at")]
        public Nullable<DateTime> ImmutableFilesSetAt
        {
            get { return (Nullable<DateTime>) attributes["immutable_files_set_at"]; }
        }

        /// <summary>
        /// Include password in emails to new users?
        /// </summary>
        [JsonPropertyName("include_password_in_welcome_email")]
        public bool IncludePasswordInWelcomeEmail
        {
            get { return (bool) attributes["include_password_in_welcome_email"]; }
        }

        /// <summary>
        /// Site default language
        /// </summary>
        [JsonPropertyName("language")]
        public string Language
        {
            get { return (string) attributes["language"]; }
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
        /// Domain name that will be appended to usernames
        /// </summary>
        [JsonPropertyName("ldap_domain")]
        public string LdapDomain
        {
            get { return (string) attributes["ldap_domain"]; }
        }

        /// <summary>
        /// Main LDAP setting: is LDAP enabled?
        /// </summary>
        [JsonPropertyName("ldap_enabled")]
        public bool LdapEnabled
        {
            get { return (bool) attributes["ldap_enabled"]; }
        }

        /// <summary>
        /// Should we sync groups from LDAP server?
        /// </summary>
        [JsonPropertyName("ldap_group_action")]
        public string LdapGroupAction
        {
            get { return (string) attributes["ldap_group_action"]; }
        }

        /// <summary>
        /// Comma or newline separated list of group names (with optional wildcards) to exclude when syncing.
        /// </summary>
        [JsonPropertyName("ldap_group_exclusion")]
        public string LdapGroupExclusion
        {
            get { return (string) attributes["ldap_group_exclusion"]; }
        }

        /// <summary>
        /// Comma or newline separated list of group names (with optional wildcards) to include when syncing.
        /// </summary>
        [JsonPropertyName("ldap_group_inclusion")]
        public string LdapGroupInclusion
        {
            get { return (string) attributes["ldap_group_inclusion"]; }
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
        /// LDAP type
        /// </summary>
        [JsonPropertyName("ldap_type")]
        public string LdapType
        {
            get { return (string) attributes["ldap_type"]; }
        }

        /// <summary>
        /// Should we sync users from LDAP server?
        /// </summary>
        [JsonPropertyName("ldap_user_action")]
        public string LdapUserAction
        {
            get { return (string) attributes["ldap_user_action"]; }
        }

        /// <summary>
        /// Comma or newline separated list of group names (with optional wildcards) - if provided, only users in these groups will be added or synced.
        /// </summary>
        [JsonPropertyName("ldap_user_include_groups")]
        public string LdapUserIncludeGroups
        {
            get { return (string) attributes["ldap_user_include_groups"]; }
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
        /// Login help text
        /// </summary>
        [JsonPropertyName("login_help_text")]
        public string LoginHelpText
        {
            get { return (string) attributes["login_help_text"]; }
        }

        /// <summary>
        /// Branded logo
        /// </summary>
        [JsonPropertyName("logo")]
        public object Logo
        {
            get { return (object) attributes["logo"]; }
        }

        /// <summary>
        /// Number of prior passwords to disallow
        /// </summary>
        [JsonPropertyName("max_prior_passwords")]
        public Nullable<Int64> MaxPriorPasswords
        {
            get { return (Nullable<Int64>) attributes["max_prior_passwords"]; }
        }

        /// <summary>
        /// Next billing amount
        /// </summary>
        [JsonPropertyName("next_billing_amount")]
        public double NextBillingAmount
        {
            get { return (double) attributes["next_billing_amount"]; }
        }

        /// <summary>
        /// Next billing date
        /// </summary>
        [JsonPropertyName("next_billing_date")]
        public string NextBillingDate
        {
            get { return (string) attributes["next_billing_date"]; }
        }

        /// <summary>
        /// Allow users to use Office for the web?
        /// </summary>
        [JsonPropertyName("office_integration_available")]
        public bool OfficeIntegrationAvailable
        {
            get { return (bool) attributes["office_integration_available"]; }
        }

        /// <summary>
        /// Link to scheduling a meeting with our Sales team
        /// </summary>
        [JsonPropertyName("oncehub_link")]
        public string OncehubLink
        {
            get { return (string) attributes["oncehub_link"]; }
        }

        /// <summary>
        /// Use servers in the USA only?
        /// </summary>
        [JsonPropertyName("opt_out_global")]
        public bool OptOutGlobal
        {
            get { return (bool) attributes["opt_out_global"]; }
        }

        /// <summary>
        /// Last time the site was notified about an overage
        /// </summary>
        [JsonPropertyName("overage_notified_at")]
        public Nullable<DateTime> OverageNotifiedAt
        {
            get { return (Nullable<DateTime>) attributes["overage_notified_at"]; }
        }

        /// <summary>
        /// Notify site email of overages?
        /// </summary>
        [JsonPropertyName("overage_notify")]
        public bool OverageNotify
        {
            get { return (bool) attributes["overage_notify"]; }
        }

        /// <summary>
        /// Is this site's billing overdue?
        /// </summary>
        [JsonPropertyName("overdue")]
        public bool Overdue
        {
            get { return (bool) attributes["overdue"]; }
        }

        /// <summary>
        /// Shortest password length for users
        /// </summary>
        [JsonPropertyName("password_min_length")]
        public Nullable<Int64> PasswordMinLength
        {
            get { return (Nullable<Int64>) attributes["password_min_length"]; }
        }

        /// <summary>
        /// Require a letter in passwords?
        /// </summary>
        [JsonPropertyName("password_require_letter")]
        public bool PasswordRequireLetter
        {
            get { return (bool) attributes["password_require_letter"]; }
        }

        /// <summary>
        /// Require lower and upper case letters in passwords?
        /// </summary>
        [JsonPropertyName("password_require_mixed")]
        public bool PasswordRequireMixed
        {
            get { return (bool) attributes["password_require_mixed"]; }
        }

        /// <summary>
        /// Require a number in passwords?
        /// </summary>
        [JsonPropertyName("password_require_number")]
        public bool PasswordRequireNumber
        {
            get { return (bool) attributes["password_require_number"]; }
        }

        /// <summary>
        /// Require special characters in password?
        /// </summary>
        [JsonPropertyName("password_require_special")]
        public bool PasswordRequireSpecial
        {
            get { return (bool) attributes["password_require_special"]; }
        }

        /// <summary>
        /// Require passwords that have not been previously breached? (see https://haveibeenpwned.com/)
        /// </summary>
        [JsonPropertyName("password_require_unbreached")]
        public bool PasswordRequireUnbreached
        {
            get { return (bool) attributes["password_require_unbreached"]; }
        }

        /// <summary>
        /// Require bundles' passwords, and passwords for other items (inboxes, public shares, etc.) to conform to the same requirements as users' passwords?
        /// </summary>
        [JsonPropertyName("password_requirements_apply_to_bundles")]
        public bool PasswordRequirementsApplyToBundles
        {
            get { return (bool) attributes["password_requirements_apply_to_bundles"]; }
        }

        /// <summary>
        /// Number of days password is valid
        /// </summary>
        [JsonPropertyName("password_validity_days")]
        public Nullable<Int64> PasswordValidityDays
        {
            get { return (Nullable<Int64>) attributes["password_validity_days"]; }
        }

        /// <summary>
        /// Site phone number
        /// </summary>
        [JsonPropertyName("phone")]
        public string Phone
        {
            get { return (string) attributes["phone"]; }
        }

        /// <summary>
        /// Require two-factor authentication for all users?
        /// </summary>
        [JsonPropertyName("require_2fa")]
        public bool Require2fa
        {
            get { return (bool) attributes["require_2fa"]; }
        }

        /// <summary>
        /// If set, requirement for two-factor authentication has been scheduled to end on this date-time.
        /// </summary>
        [JsonPropertyName("require_2fa_stop_time")]
        public Nullable<DateTime> Require2faStopTime
        {
            get { return (Nullable<DateTime>) attributes["require_2fa_stop_time"]; }
        }

        /// <summary>
        /// What type of user is required to use two-factor authentication (when require_2fa is set to `true` for this site)?
        /// </summary>
        [JsonPropertyName("require_2fa_user_type")]
        public string Require2faUserType
        {
            get { return (string) attributes["require_2fa_user_type"]; }
        }

        /// <summary>
        /// Current session
        /// </summary>
        [JsonPropertyName("session")]
        public object Session
        {
            get { return (object) attributes["session"]; }
        }

        /// <summary>
        /// Are sessions locked to the same IP? (i.e. do users need to log in again if they change IPs?)
        /// </summary>
        [JsonPropertyName("session_pinned_by_ip")]
        public bool SessionPinnedByIp
        {
            get { return (bool) attributes["session_pinned_by_ip"]; }
        }

        /// <summary>
        /// Use user FTP roots also for SFTP?
        /// </summary>
        [JsonPropertyName("sftp_user_root_enabled")]
        public bool SftpUserRootEnabled
        {
            get { return (bool) attributes["sftp_user_root_enabled"]; }
        }

        /// <summary>
        /// Allow bundle creation
        /// </summary>
        [JsonPropertyName("sharing_enabled")]
        public bool SharingEnabled
        {
            get { return (bool) attributes["sharing_enabled"]; }
        }

        /// <summary>
        /// Show request access link for users without access?  Currently unused.
        /// </summary>
        [JsonPropertyName("show_request_access_link")]
        public bool ShowRequestAccessLink
        {
            get { return (bool) attributes["show_request_access_link"]; }
        }

        /// <summary>
        /// Custom site footer text
        /// </summary>
        [JsonPropertyName("site_footer")]
        public string SiteFooter
        {
            get { return (string) attributes["site_footer"]; }
        }

        /// <summary>
        /// Custom site header text
        /// </summary>
        [JsonPropertyName("site_header")]
        public string SiteHeader
        {
            get { return (string) attributes["site_header"]; }
        }

        /// <summary>
        /// SMTP server hostname or IP
        /// </summary>
        [JsonPropertyName("smtp_address")]
        public string SmtpAddress
        {
            get { return (string) attributes["smtp_address"]; }
        }

        /// <summary>
        /// SMTP server authentication type
        /// </summary>
        [JsonPropertyName("smtp_authentication")]
        public string SmtpAuthentication
        {
            get { return (string) attributes["smtp_authentication"]; }
        }

        /// <summary>
        /// From address to use when mailing through custom SMTP
        /// </summary>
        [JsonPropertyName("smtp_from")]
        public string SmtpFrom
        {
            get { return (string) attributes["smtp_from"]; }
        }

        /// <summary>
        /// SMTP server port
        /// </summary>
        [JsonPropertyName("smtp_port")]
        public Nullable<Int64> SmtpPort
        {
            get { return (Nullable<Int64>) attributes["smtp_port"]; }
        }

        /// <summary>
        /// SMTP server username
        /// </summary>
        [JsonPropertyName("smtp_username")]
        public string SmtpUsername
        {
            get { return (string) attributes["smtp_username"]; }
        }

        /// <summary>
        /// Session expiry in hours
        /// </summary>
        [JsonPropertyName("session_expiry")]
        public double SessionExpiry
        {
            get { return (double) attributes["session_expiry"]; }
        }

        /// <summary>
        /// Is SSL required?  Disabling this is insecure.
        /// </summary>
        [JsonPropertyName("ssl_required")]
        public bool SslRequired
        {
            get { return (bool) attributes["ssl_required"]; }
        }

        /// <summary>
        /// Site subdomain
        /// </summary>
        [JsonPropertyName("subdomain")]
        public string Subdomain
        {
            get { return (string) attributes["subdomain"]; }
        }

        /// <summary>
        /// If switching plans, when does the new plan take effect?
        /// </summary>
        [JsonPropertyName("switch_to_plan_date")]
        public Nullable<DateTime> SwitchToPlanDate
        {
            get { return (Nullable<DateTime>) attributes["switch_to_plan_date"]; }
        }

        /// <summary>
        /// Is TLS disabled(site setting)?
        /// </summary>
        [JsonPropertyName("tls_disabled")]
        public bool TlsDisabled
        {
            get { return (bool) attributes["tls_disabled"]; }
        }

        /// <summary>
        /// Number of days left in trial
        /// </summary>
        [JsonPropertyName("trial_days_left")]
        public Nullable<Int64> TrialDaysLeft
        {
            get { return (Nullable<Int64>) attributes["trial_days_left"]; }
        }

        /// <summary>
        /// When does this Site trial expire?
        /// </summary>
        [JsonPropertyName("trial_until")]
        public Nullable<DateTime> TrialUntil
        {
            get { return (Nullable<DateTime>) attributes["trial_until"]; }
        }

        /// <summary>
        /// Last time this Site was updated
        /// </summary>
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>) attributes["updated_at"]; }
        }

        /// <summary>
        /// Allow uploaders to set `provided_modified_at` for uploaded files?
        /// </summary>
        [JsonPropertyName("use_provided_modified_at")]
        public bool UseProvidedModifiedAt
        {
            get { return (bool) attributes["use_provided_modified_at"]; }
        }

        /// <summary>
        /// User of current session
        /// </summary>
        [JsonPropertyName("user")]
        public object User
        {
            get { return (object) attributes["user"]; }
        }

        /// <summary>
        /// Will users be locked out after incorrect login attempts?
        /// </summary>
        [JsonPropertyName("user_lockout")]
        public bool UserLockout
        {
            get { return (bool) attributes["user_lockout"]; }
        }

        /// <summary>
        /// How many hours to lock user out for failed password?
        /// </summary>
        [JsonPropertyName("user_lockout_lock_period")]
        public Nullable<Int64> UserLockoutLockPeriod
        {
            get { return (Nullable<Int64>) attributes["user_lockout_lock_period"]; }
        }

        /// <summary>
        /// Number of login tries within `user_lockout_within` hours before users are locked out
        /// </summary>
        [JsonPropertyName("user_lockout_tries")]
        public Nullable<Int64> UserLockoutTries
        {
            get { return (Nullable<Int64>) attributes["user_lockout_tries"]; }
        }

        /// <summary>
        /// Number of hours for user lockout window
        /// </summary>
        [JsonPropertyName("user_lockout_within")]
        public Nullable<Int64> UserLockoutWithin
        {
            get { return (Nullable<Int64>) attributes["user_lockout_within"]; }
        }

        /// <summary>
        /// Enable User Requests feature
        /// </summary>
        [JsonPropertyName("user_requests_enabled")]
        public bool UserRequestsEnabled
        {
            get { return (bool) attributes["user_requests_enabled"]; }
        }

        /// <summary>
        /// Custom text send in user welcome email
        /// </summary>
        [JsonPropertyName("welcome_custom_text")]
        public string WelcomeCustomText
        {
            get { return (string) attributes["welcome_custom_text"]; }
        }

        /// <summary>
        /// Include this email in welcome emails if enabled
        /// </summary>
        [JsonPropertyName("welcome_email_cc")]
        public string WelcomeEmailCc
        {
            get { return (string) attributes["welcome_email_cc"]; }
        }

        /// <summary>
        /// Will the welcome email be sent to new users?
        /// </summary>
        [JsonPropertyName("welcome_email_enabled")]
        public bool WelcomeEmailEnabled
        {
            get { return (bool) attributes["welcome_email_enabled"]; }
        }

        /// <summary>
        /// Does the welcome screen appear?
        /// </summary>
        [JsonPropertyName("welcome_screen")]
        public string WelcomeScreen
        {
            get { return (string) attributes["welcome_screen"]; }
        }

        /// <summary>
        /// Does FTP user Windows emulation mode?
        /// </summary>
        [JsonPropertyName("windows_mode_ftp")]
        public bool WindowsModeFtp
        {
            get { return (bool) attributes["windows_mode_ftp"]; }
        }

        /// <summary>
        /// If greater than zero, users will unable to login if they do not show activity within this number of days.
        /// </summary>
        [JsonPropertyName("disable_users_from_inactivity_period_days")]
        public Nullable<Int64> DisableUsersFromInactivityPeriodDays
        {
            get { return (Nullable<Int64>) attributes["disable_users_from_inactivity_period_days"]; }
        }



        /// <summary>
        /// </summary>
        public static async Task<Site> Get(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendRequest($"/site", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Site>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<UsageSnapshot> GetUsage(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendRequest($"/site/usage", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<UsageSnapshot>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Site name
        ///   subdomain - string - Site subdomain
        ///   domain - string - Custom domain
        ///   email - string - Main email for this site
        ///   reply_to_email - string - Reply-to email for this site
        ///   allow_bundle_names - boolean - Are manual Bundle names allowed?
        ///   bundle_expiration - int64 - Site-wide Bundle expiration in days
        ///   overage_notify - boolean - Notify site email of overages?
        ///   welcome_email_enabled - boolean - Will the welcome email be sent to new users?
        ///   ask_about_overwrites - boolean - If false, rename conflicting files instead of asking for overwrite confirmation.  Only applies to web interface.
        ///   show_request_access_link - boolean - Show request access link for users without access?  Currently unused.
        ///   welcome_email_cc - string - Include this email in welcome emails if enabled
        ///   welcome_custom_text - string - Custom text send in user welcome email
        ///   language - string - Site default language
        ///   windows_mode_ftp - boolean - Does FTP user Windows emulation mode?
        ///   default_time_zone - string - Site default time zone
        ///   desktop_app - boolean - Is the desktop app enabled?
        ///   desktop_app_session_ip_pinning - boolean - Is desktop app session IP pinning enabled?
        ///   desktop_app_session_lifetime - int64 - Desktop app session lifetime (in hours)
        ///   folder_permissions_groups_only - boolean - If true, permissions for this site must be bound to a group (not a user). Otherwise, permissions must be bound to a user.
        ///   welcome_screen - string - Does the welcome screen appear?
        ///   office_integration_available - boolean - Allow users to use Office for the web?
        ///   session_expiry - double - Session expiry in hours
        ///   ssl_required - boolean - Is SSL required?  Disabling this is insecure.
        ///   tls_disabled - boolean - Is TLS disabled(site setting)?
        ///   user_lockout - boolean - Will users be locked out after incorrect login attempts?
        ///   user_lockout_tries - int64 - Number of login tries within `user_lockout_within` hours before users are locked out
        ///   user_lockout_within - int64 - Number of hours for user lockout window
        ///   user_lockout_lock_period - int64 - How many hours to lock user out for failed password?
        ///   include_password_in_welcome_email - boolean - Include password in emails to new users?
        ///   allowed_countries - string - Comma seperated list of allowed Country codes
        ///   allowed_ips - string - List of allowed IP addresses
        ///   disallowed_countries - string - Comma seperated list of disallowed Country codes
        ///   days_to_retain_backups - int64 - Number of days to keep deleted files
        ///   max_prior_passwords - int64 - Number of prior passwords to disallow
        ///   password_validity_days - int64 - Number of days password is valid
        ///   password_min_length - int64 - Shortest password length for users
        ///   password_require_letter - boolean - Require a letter in passwords?
        ///   password_require_mixed - boolean - Require lower and upper case letters in passwords?
        ///   password_require_special - boolean - Require special characters in password?
        ///   password_require_number - boolean - Require a number in passwords?
        ///   password_require_unbreached - boolean - Require passwords that have not been previously breached? (see https://haveibeenpwned.com/)
        ///   sftp_user_root_enabled - boolean - Use user FTP roots also for SFTP?
        ///   disable_password_reset - boolean - Is password reset disabled?
        ///   immutable_files - boolean - Are files protected from modification?
        ///   session_pinned_by_ip - boolean - Are sessions locked to the same IP? (i.e. do users need to log in again if they change IPs?)
        ///   bundle_password_required - boolean - Do Bundles require password protection?
        ///   bundle_require_share_recipient - boolean - Do Bundles require recipients for sharing?
        ///   password_requirements_apply_to_bundles - boolean - Require bundles' passwords, and passwords for other items (inboxes, public shares, etc.) to conform to the same requirements as users' passwords?
        ///   opt_out_global - boolean - Use servers in the USA only?
        ///   use_provided_modified_at - boolean - Allow uploaders to set `provided_modified_at` for uploaded files?
        ///   custom_namespace - boolean - Is this site using a custom namespace for users?
        ///   disable_users_from_inactivity_period_days - int64 - If greater than zero, users will unable to login if they do not show activity within this number of days.
        ///   non_sso_groups_allowed - boolean - If true, groups can be manually created / modified / deleted by Site Admins. Otherwise, groups can only be managed via your SSO provider.
        ///   non_sso_users_allowed - boolean - If true, users can be manually created / modified / deleted by Site Admins. Otherwise, users can only be managed via your SSO provider.
        ///   sharing_enabled - boolean - Allow bundle creation
        ///   user_requests_enabled - boolean - Enable User Requests feature
        ///   allowed_2fa_method_sms - boolean - Is SMS two factor authentication allowed?
        ///   allowed_2fa_method_u2f - boolean - Is U2F two factor authentication allowed?
        ///   allowed_2fa_method_totp - boolean - Is TOTP two factor authentication allowed?
        ///   allowed_2fa_method_yubi - boolean - Is yubikey two factor authentication allowed?
        ///   require_2fa - boolean - Require two-factor authentication for all users?
        ///   require_2fa_user_type - string - What type of user is required to use two-factor authentication (when require_2fa is set to `true` for this site)?
        ///   color2_top - string - Top bar background color
        ///   color2_left - string - Page link and button color
        ///   color2_link - string - Top bar link color
        ///   color2_text - string - Page link and button color
        ///   color2_top_text - string - Top bar text color
        ///   site_header - string - Custom site header text
        ///   site_footer - string - Custom site footer text
        ///   login_help_text - string - Login help text
        ///   smtp_address - string - SMTP server hostname or IP
        ///   smtp_authentication - string - SMTP server authentication type
        ///   smtp_from - string - From address to use when mailing through custom SMTP
        ///   smtp_username - string - SMTP server username
        ///   smtp_port - int64 - SMTP server port
        ///   ldap_enabled - boolean - Main LDAP setting: is LDAP enabled?
        ///   ldap_type - string - LDAP type
        ///   ldap_host - string - LDAP host
        ///   ldap_host_2 - string - LDAP backup host
        ///   ldap_host_3 - string - LDAP backup host
        ///   ldap_port - int64 - LDAP port
        ///   ldap_secure - boolean - Use secure LDAP?
        ///   ldap_username - string - Username for signing in to LDAP server.
        ///   ldap_username_field - string - LDAP username field
        ///   ldap_domain - string - Domain name that will be appended to usernames
        ///   ldap_user_action - string - Should we sync users from LDAP server?
        ///   ldap_group_action - string - Should we sync groups from LDAP server?
        ///   ldap_user_include_groups - string - Comma or newline separated list of group names (with optional wildcards) - if provided, only users in these groups will be added or synced.
        ///   ldap_group_exclusion - string - Comma or newline separated list of group names (with optional wildcards) to exclude when syncing.
        ///   ldap_group_inclusion - string - Comma or newline separated list of group names (with optional wildcards) to include when syncing.
        ///   ldap_base_dn - string - Base DN for looking up users in LDAP server
        ///   icon16_file - file
        ///   icon16_delete - boolean - If true, will delete the file stored in icon16
        ///   icon32_file - file
        ///   icon32_delete - boolean - If true, will delete the file stored in icon32
        ///   icon48_file - file
        ///   icon48_delete - boolean - If true, will delete the file stored in icon48
        ///   icon128_file - file
        ///   icon128_delete - boolean - If true, will delete the file stored in icon128
        ///   logo_file - file
        ///   logo_delete - boolean - If true, will delete the file stored in logo
        ///   disable_2fa_with_delay - boolean - If set to true, we will begin the process of disabling 2FA on this site.
        ///   ldap_password_change - string - New LDAP password.
        ///   ldap_password_change_confirmation - string - Confirm new LDAP password.
        ///   smtp_password - string - Password for SMTP server.
        /// </summary>
        public static async Task<Site> Update(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("subdomain") && !(parameters["subdomain"] is string ))
            {
                throw new ArgumentException("Bad parameter: subdomain must be of type string", "parameters[\"subdomain\"]");
            }
            if (parameters.ContainsKey("domain") && !(parameters["domain"] is string ))
            {
                throw new ArgumentException("Bad parameter: domain must be of type string", "parameters[\"domain\"]");
            }
            if (parameters.ContainsKey("email") && !(parameters["email"] is string ))
            {
                throw new ArgumentException("Bad parameter: email must be of type string", "parameters[\"email\"]");
            }
            if (parameters.ContainsKey("reply_to_email") && !(parameters["reply_to_email"] is string ))
            {
                throw new ArgumentException("Bad parameter: reply_to_email must be of type string", "parameters[\"reply_to_email\"]");
            }
            if (parameters.ContainsKey("allow_bundle_names") && !(parameters["allow_bundle_names"] is bool ))
            {
                throw new ArgumentException("Bad parameter: allow_bundle_names must be of type bool", "parameters[\"allow_bundle_names\"]");
            }
            if (parameters.ContainsKey("bundle_expiration") && !(parameters["bundle_expiration"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: bundle_expiration must be of type Nullable<Int64>", "parameters[\"bundle_expiration\"]");
            }
            if (parameters.ContainsKey("overage_notify") && !(parameters["overage_notify"] is bool ))
            {
                throw new ArgumentException("Bad parameter: overage_notify must be of type bool", "parameters[\"overage_notify\"]");
            }
            if (parameters.ContainsKey("welcome_email_enabled") && !(parameters["welcome_email_enabled"] is bool ))
            {
                throw new ArgumentException("Bad parameter: welcome_email_enabled must be of type bool", "parameters[\"welcome_email_enabled\"]");
            }
            if (parameters.ContainsKey("ask_about_overwrites") && !(parameters["ask_about_overwrites"] is bool ))
            {
                throw new ArgumentException("Bad parameter: ask_about_overwrites must be of type bool", "parameters[\"ask_about_overwrites\"]");
            }
            if (parameters.ContainsKey("show_request_access_link") && !(parameters["show_request_access_link"] is bool ))
            {
                throw new ArgumentException("Bad parameter: show_request_access_link must be of type bool", "parameters[\"show_request_access_link\"]");
            }
            if (parameters.ContainsKey("welcome_email_cc") && !(parameters["welcome_email_cc"] is string ))
            {
                throw new ArgumentException("Bad parameter: welcome_email_cc must be of type string", "parameters[\"welcome_email_cc\"]");
            }
            if (parameters.ContainsKey("welcome_custom_text") && !(parameters["welcome_custom_text"] is string ))
            {
                throw new ArgumentException("Bad parameter: welcome_custom_text must be of type string", "parameters[\"welcome_custom_text\"]");
            }
            if (parameters.ContainsKey("language") && !(parameters["language"] is string ))
            {
                throw new ArgumentException("Bad parameter: language must be of type string", "parameters[\"language\"]");
            }
            if (parameters.ContainsKey("windows_mode_ftp") && !(parameters["windows_mode_ftp"] is bool ))
            {
                throw new ArgumentException("Bad parameter: windows_mode_ftp must be of type bool", "parameters[\"windows_mode_ftp\"]");
            }
            if (parameters.ContainsKey("default_time_zone") && !(parameters["default_time_zone"] is string ))
            {
                throw new ArgumentException("Bad parameter: default_time_zone must be of type string", "parameters[\"default_time_zone\"]");
            }
            if (parameters.ContainsKey("desktop_app") && !(parameters["desktop_app"] is bool ))
            {
                throw new ArgumentException("Bad parameter: desktop_app must be of type bool", "parameters[\"desktop_app\"]");
            }
            if (parameters.ContainsKey("desktop_app_session_ip_pinning") && !(parameters["desktop_app_session_ip_pinning"] is bool ))
            {
                throw new ArgumentException("Bad parameter: desktop_app_session_ip_pinning must be of type bool", "parameters[\"desktop_app_session_ip_pinning\"]");
            }
            if (parameters.ContainsKey("desktop_app_session_lifetime") && !(parameters["desktop_app_session_lifetime"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: desktop_app_session_lifetime must be of type Nullable<Int64>", "parameters[\"desktop_app_session_lifetime\"]");
            }
            if (parameters.ContainsKey("folder_permissions_groups_only") && !(parameters["folder_permissions_groups_only"] is bool ))
            {
                throw new ArgumentException("Bad parameter: folder_permissions_groups_only must be of type bool", "parameters[\"folder_permissions_groups_only\"]");
            }
            if (parameters.ContainsKey("welcome_screen") && !(parameters["welcome_screen"] is string ))
            {
                throw new ArgumentException("Bad parameter: welcome_screen must be of type string", "parameters[\"welcome_screen\"]");
            }
            if (parameters.ContainsKey("office_integration_available") && !(parameters["office_integration_available"] is bool ))
            {
                throw new ArgumentException("Bad parameter: office_integration_available must be of type bool", "parameters[\"office_integration_available\"]");
            }
            if (parameters.ContainsKey("session_expiry") && !(parameters["session_expiry"] is double ))
            {
                throw new ArgumentException("Bad parameter: session_expiry must be of type double", "parameters[\"session_expiry\"]");
            }
            if (parameters.ContainsKey("ssl_required") && !(parameters["ssl_required"] is bool ))
            {
                throw new ArgumentException("Bad parameter: ssl_required must be of type bool", "parameters[\"ssl_required\"]");
            }
            if (parameters.ContainsKey("tls_disabled") && !(parameters["tls_disabled"] is bool ))
            {
                throw new ArgumentException("Bad parameter: tls_disabled must be of type bool", "parameters[\"tls_disabled\"]");
            }
            if (parameters.ContainsKey("user_lockout") && !(parameters["user_lockout"] is bool ))
            {
                throw new ArgumentException("Bad parameter: user_lockout must be of type bool", "parameters[\"user_lockout\"]");
            }
            if (parameters.ContainsKey("user_lockout_tries") && !(parameters["user_lockout_tries"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_lockout_tries must be of type Nullable<Int64>", "parameters[\"user_lockout_tries\"]");
            }
            if (parameters.ContainsKey("user_lockout_within") && !(parameters["user_lockout_within"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_lockout_within must be of type Nullable<Int64>", "parameters[\"user_lockout_within\"]");
            }
            if (parameters.ContainsKey("user_lockout_lock_period") && !(parameters["user_lockout_lock_period"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_lockout_lock_period must be of type Nullable<Int64>", "parameters[\"user_lockout_lock_period\"]");
            }
            if (parameters.ContainsKey("include_password_in_welcome_email") && !(parameters["include_password_in_welcome_email"] is bool ))
            {
                throw new ArgumentException("Bad parameter: include_password_in_welcome_email must be of type bool", "parameters[\"include_password_in_welcome_email\"]");
            }
            if (parameters.ContainsKey("allowed_countries") && !(parameters["allowed_countries"] is string ))
            {
                throw new ArgumentException("Bad parameter: allowed_countries must be of type string", "parameters[\"allowed_countries\"]");
            }
            if (parameters.ContainsKey("allowed_ips") && !(parameters["allowed_ips"] is string ))
            {
                throw new ArgumentException("Bad parameter: allowed_ips must be of type string", "parameters[\"allowed_ips\"]");
            }
            if (parameters.ContainsKey("disallowed_countries") && !(parameters["disallowed_countries"] is string ))
            {
                throw new ArgumentException("Bad parameter: disallowed_countries must be of type string", "parameters[\"disallowed_countries\"]");
            }
            if (parameters.ContainsKey("days_to_retain_backups") && !(parameters["days_to_retain_backups"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: days_to_retain_backups must be of type Nullable<Int64>", "parameters[\"days_to_retain_backups\"]");
            }
            if (parameters.ContainsKey("max_prior_passwords") && !(parameters["max_prior_passwords"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: max_prior_passwords must be of type Nullable<Int64>", "parameters[\"max_prior_passwords\"]");
            }
            if (parameters.ContainsKey("password_validity_days") && !(parameters["password_validity_days"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: password_validity_days must be of type Nullable<Int64>", "parameters[\"password_validity_days\"]");
            }
            if (parameters.ContainsKey("password_min_length") && !(parameters["password_min_length"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: password_min_length must be of type Nullable<Int64>", "parameters[\"password_min_length\"]");
            }
            if (parameters.ContainsKey("password_require_letter") && !(parameters["password_require_letter"] is bool ))
            {
                throw new ArgumentException("Bad parameter: password_require_letter must be of type bool", "parameters[\"password_require_letter\"]");
            }
            if (parameters.ContainsKey("password_require_mixed") && !(parameters["password_require_mixed"] is bool ))
            {
                throw new ArgumentException("Bad parameter: password_require_mixed must be of type bool", "parameters[\"password_require_mixed\"]");
            }
            if (parameters.ContainsKey("password_require_special") && !(parameters["password_require_special"] is bool ))
            {
                throw new ArgumentException("Bad parameter: password_require_special must be of type bool", "parameters[\"password_require_special\"]");
            }
            if (parameters.ContainsKey("password_require_number") && !(parameters["password_require_number"] is bool ))
            {
                throw new ArgumentException("Bad parameter: password_require_number must be of type bool", "parameters[\"password_require_number\"]");
            }
            if (parameters.ContainsKey("password_require_unbreached") && !(parameters["password_require_unbreached"] is bool ))
            {
                throw new ArgumentException("Bad parameter: password_require_unbreached must be of type bool", "parameters[\"password_require_unbreached\"]");
            }
            if (parameters.ContainsKey("sftp_user_root_enabled") && !(parameters["sftp_user_root_enabled"] is bool ))
            {
                throw new ArgumentException("Bad parameter: sftp_user_root_enabled must be of type bool", "parameters[\"sftp_user_root_enabled\"]");
            }
            if (parameters.ContainsKey("disable_password_reset") && !(parameters["disable_password_reset"] is bool ))
            {
                throw new ArgumentException("Bad parameter: disable_password_reset must be of type bool", "parameters[\"disable_password_reset\"]");
            }
            if (parameters.ContainsKey("immutable_files") && !(parameters["immutable_files"] is bool ))
            {
                throw new ArgumentException("Bad parameter: immutable_files must be of type bool", "parameters[\"immutable_files\"]");
            }
            if (parameters.ContainsKey("session_pinned_by_ip") && !(parameters["session_pinned_by_ip"] is bool ))
            {
                throw new ArgumentException("Bad parameter: session_pinned_by_ip must be of type bool", "parameters[\"session_pinned_by_ip\"]");
            }
            if (parameters.ContainsKey("bundle_password_required") && !(parameters["bundle_password_required"] is bool ))
            {
                throw new ArgumentException("Bad parameter: bundle_password_required must be of type bool", "parameters[\"bundle_password_required\"]");
            }
            if (parameters.ContainsKey("bundle_require_share_recipient") && !(parameters["bundle_require_share_recipient"] is bool ))
            {
                throw new ArgumentException("Bad parameter: bundle_require_share_recipient must be of type bool", "parameters[\"bundle_require_share_recipient\"]");
            }
            if (parameters.ContainsKey("password_requirements_apply_to_bundles") && !(parameters["password_requirements_apply_to_bundles"] is bool ))
            {
                throw new ArgumentException("Bad parameter: password_requirements_apply_to_bundles must be of type bool", "parameters[\"password_requirements_apply_to_bundles\"]");
            }
            if (parameters.ContainsKey("opt_out_global") && !(parameters["opt_out_global"] is bool ))
            {
                throw new ArgumentException("Bad parameter: opt_out_global must be of type bool", "parameters[\"opt_out_global\"]");
            }
            if (parameters.ContainsKey("use_provided_modified_at") && !(parameters["use_provided_modified_at"] is bool ))
            {
                throw new ArgumentException("Bad parameter: use_provided_modified_at must be of type bool", "parameters[\"use_provided_modified_at\"]");
            }
            if (parameters.ContainsKey("custom_namespace") && !(parameters["custom_namespace"] is bool ))
            {
                throw new ArgumentException("Bad parameter: custom_namespace must be of type bool", "parameters[\"custom_namespace\"]");
            }
            if (parameters.ContainsKey("disable_users_from_inactivity_period_days") && !(parameters["disable_users_from_inactivity_period_days"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: disable_users_from_inactivity_period_days must be of type Nullable<Int64>", "parameters[\"disable_users_from_inactivity_period_days\"]");
            }
            if (parameters.ContainsKey("non_sso_groups_allowed") && !(parameters["non_sso_groups_allowed"] is bool ))
            {
                throw new ArgumentException("Bad parameter: non_sso_groups_allowed must be of type bool", "parameters[\"non_sso_groups_allowed\"]");
            }
            if (parameters.ContainsKey("non_sso_users_allowed") && !(parameters["non_sso_users_allowed"] is bool ))
            {
                throw new ArgumentException("Bad parameter: non_sso_users_allowed must be of type bool", "parameters[\"non_sso_users_allowed\"]");
            }
            if (parameters.ContainsKey("sharing_enabled") && !(parameters["sharing_enabled"] is bool ))
            {
                throw new ArgumentException("Bad parameter: sharing_enabled must be of type bool", "parameters[\"sharing_enabled\"]");
            }
            if (parameters.ContainsKey("user_requests_enabled") && !(parameters["user_requests_enabled"] is bool ))
            {
                throw new ArgumentException("Bad parameter: user_requests_enabled must be of type bool", "parameters[\"user_requests_enabled\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_sms") && !(parameters["allowed_2fa_method_sms"] is bool ))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_sms must be of type bool", "parameters[\"allowed_2fa_method_sms\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_u2f") && !(parameters["allowed_2fa_method_u2f"] is bool ))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_u2f must be of type bool", "parameters[\"allowed_2fa_method_u2f\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_totp") && !(parameters["allowed_2fa_method_totp"] is bool ))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_totp must be of type bool", "parameters[\"allowed_2fa_method_totp\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_yubi") && !(parameters["allowed_2fa_method_yubi"] is bool ))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_yubi must be of type bool", "parameters[\"allowed_2fa_method_yubi\"]");
            }
            if (parameters.ContainsKey("require_2fa") && !(parameters["require_2fa"] is bool ))
            {
                throw new ArgumentException("Bad parameter: require_2fa must be of type bool", "parameters[\"require_2fa\"]");
            }
            if (parameters.ContainsKey("require_2fa_user_type") && !(parameters["require_2fa_user_type"] is string ))
            {
                throw new ArgumentException("Bad parameter: require_2fa_user_type must be of type string", "parameters[\"require_2fa_user_type\"]");
            }
            if (parameters.ContainsKey("color2_top") && !(parameters["color2_top"] is string ))
            {
                throw new ArgumentException("Bad parameter: color2_top must be of type string", "parameters[\"color2_top\"]");
            }
            if (parameters.ContainsKey("color2_left") && !(parameters["color2_left"] is string ))
            {
                throw new ArgumentException("Bad parameter: color2_left must be of type string", "parameters[\"color2_left\"]");
            }
            if (parameters.ContainsKey("color2_link") && !(parameters["color2_link"] is string ))
            {
                throw new ArgumentException("Bad parameter: color2_link must be of type string", "parameters[\"color2_link\"]");
            }
            if (parameters.ContainsKey("color2_text") && !(parameters["color2_text"] is string ))
            {
                throw new ArgumentException("Bad parameter: color2_text must be of type string", "parameters[\"color2_text\"]");
            }
            if (parameters.ContainsKey("color2_top_text") && !(parameters["color2_top_text"] is string ))
            {
                throw new ArgumentException("Bad parameter: color2_top_text must be of type string", "parameters[\"color2_top_text\"]");
            }
            if (parameters.ContainsKey("site_header") && !(parameters["site_header"] is string ))
            {
                throw new ArgumentException("Bad parameter: site_header must be of type string", "parameters[\"site_header\"]");
            }
            if (parameters.ContainsKey("site_footer") && !(parameters["site_footer"] is string ))
            {
                throw new ArgumentException("Bad parameter: site_footer must be of type string", "parameters[\"site_footer\"]");
            }
            if (parameters.ContainsKey("login_help_text") && !(parameters["login_help_text"] is string ))
            {
                throw new ArgumentException("Bad parameter: login_help_text must be of type string", "parameters[\"login_help_text\"]");
            }
            if (parameters.ContainsKey("smtp_address") && !(parameters["smtp_address"] is string ))
            {
                throw new ArgumentException("Bad parameter: smtp_address must be of type string", "parameters[\"smtp_address\"]");
            }
            if (parameters.ContainsKey("smtp_authentication") && !(parameters["smtp_authentication"] is string ))
            {
                throw new ArgumentException("Bad parameter: smtp_authentication must be of type string", "parameters[\"smtp_authentication\"]");
            }
            if (parameters.ContainsKey("smtp_from") && !(parameters["smtp_from"] is string ))
            {
                throw new ArgumentException("Bad parameter: smtp_from must be of type string", "parameters[\"smtp_from\"]");
            }
            if (parameters.ContainsKey("smtp_username") && !(parameters["smtp_username"] is string ))
            {
                throw new ArgumentException("Bad parameter: smtp_username must be of type string", "parameters[\"smtp_username\"]");
            }
            if (parameters.ContainsKey("smtp_port") && !(parameters["smtp_port"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: smtp_port must be of type Nullable<Int64>", "parameters[\"smtp_port\"]");
            }
            if (parameters.ContainsKey("ldap_enabled") && !(parameters["ldap_enabled"] is bool ))
            {
                throw new ArgumentException("Bad parameter: ldap_enabled must be of type bool", "parameters[\"ldap_enabled\"]");
            }
            if (parameters.ContainsKey("ldap_type") && !(parameters["ldap_type"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_type must be of type string", "parameters[\"ldap_type\"]");
            }
            if (parameters.ContainsKey("ldap_host") && !(parameters["ldap_host"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_host must be of type string", "parameters[\"ldap_host\"]");
            }
            if (parameters.ContainsKey("ldap_host_2") && !(parameters["ldap_host_2"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_host_2 must be of type string", "parameters[\"ldap_host_2\"]");
            }
            if (parameters.ContainsKey("ldap_host_3") && !(parameters["ldap_host_3"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_host_3 must be of type string", "parameters[\"ldap_host_3\"]");
            }
            if (parameters.ContainsKey("ldap_port") && !(parameters["ldap_port"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: ldap_port must be of type Nullable<Int64>", "parameters[\"ldap_port\"]");
            }
            if (parameters.ContainsKey("ldap_secure") && !(parameters["ldap_secure"] is bool ))
            {
                throw new ArgumentException("Bad parameter: ldap_secure must be of type bool", "parameters[\"ldap_secure\"]");
            }
            if (parameters.ContainsKey("ldap_username") && !(parameters["ldap_username"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_username must be of type string", "parameters[\"ldap_username\"]");
            }
            if (parameters.ContainsKey("ldap_username_field") && !(parameters["ldap_username_field"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_username_field must be of type string", "parameters[\"ldap_username_field\"]");
            }
            if (parameters.ContainsKey("ldap_domain") && !(parameters["ldap_domain"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_domain must be of type string", "parameters[\"ldap_domain\"]");
            }
            if (parameters.ContainsKey("ldap_user_action") && !(parameters["ldap_user_action"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_user_action must be of type string", "parameters[\"ldap_user_action\"]");
            }
            if (parameters.ContainsKey("ldap_group_action") && !(parameters["ldap_group_action"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_group_action must be of type string", "parameters[\"ldap_group_action\"]");
            }
            if (parameters.ContainsKey("ldap_user_include_groups") && !(parameters["ldap_user_include_groups"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_user_include_groups must be of type string", "parameters[\"ldap_user_include_groups\"]");
            }
            if (parameters.ContainsKey("ldap_group_exclusion") && !(parameters["ldap_group_exclusion"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_group_exclusion must be of type string", "parameters[\"ldap_group_exclusion\"]");
            }
            if (parameters.ContainsKey("ldap_group_inclusion") && !(parameters["ldap_group_inclusion"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_group_inclusion must be of type string", "parameters[\"ldap_group_inclusion\"]");
            }
            if (parameters.ContainsKey("ldap_base_dn") && !(parameters["ldap_base_dn"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_base_dn must be of type string", "parameters[\"ldap_base_dn\"]");
            }
            if (parameters.ContainsKey("icon16_file") && !(parameters["icon16_file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: icon16_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"icon16_file\"]");
            }
            if (parameters.ContainsKey("icon16_delete") && !(parameters["icon16_delete"] is bool ))
            {
                throw new ArgumentException("Bad parameter: icon16_delete must be of type bool", "parameters[\"icon16_delete\"]");
            }
            if (parameters.ContainsKey("icon32_file") && !(parameters["icon32_file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: icon32_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"icon32_file\"]");
            }
            if (parameters.ContainsKey("icon32_delete") && !(parameters["icon32_delete"] is bool ))
            {
                throw new ArgumentException("Bad parameter: icon32_delete must be of type bool", "parameters[\"icon32_delete\"]");
            }
            if (parameters.ContainsKey("icon48_file") && !(parameters["icon48_file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: icon48_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"icon48_file\"]");
            }
            if (parameters.ContainsKey("icon48_delete") && !(parameters["icon48_delete"] is bool ))
            {
                throw new ArgumentException("Bad parameter: icon48_delete must be of type bool", "parameters[\"icon48_delete\"]");
            }
            if (parameters.ContainsKey("icon128_file") && !(parameters["icon128_file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: icon128_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"icon128_file\"]");
            }
            if (parameters.ContainsKey("icon128_delete") && !(parameters["icon128_delete"] is bool ))
            {
                throw new ArgumentException("Bad parameter: icon128_delete must be of type bool", "parameters[\"icon128_delete\"]");
            }
            if (parameters.ContainsKey("logo_file") && !(parameters["logo_file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: logo_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"logo_file\"]");
            }
            if (parameters.ContainsKey("logo_delete") && !(parameters["logo_delete"] is bool ))
            {
                throw new ArgumentException("Bad parameter: logo_delete must be of type bool", "parameters[\"logo_delete\"]");
            }
            if (parameters.ContainsKey("disable_2fa_with_delay") && !(parameters["disable_2fa_with_delay"] is bool ))
            {
                throw new ArgumentException("Bad parameter: disable_2fa_with_delay must be of type bool", "parameters[\"disable_2fa_with_delay\"]");
            }
            if (parameters.ContainsKey("ldap_password_change") && !(parameters["ldap_password_change"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_password_change must be of type string", "parameters[\"ldap_password_change\"]");
            }
            if (parameters.ContainsKey("ldap_password_change_confirmation") && !(parameters["ldap_password_change_confirmation"] is string ))
            {
                throw new ArgumentException("Bad parameter: ldap_password_change_confirmation must be of type string", "parameters[\"ldap_password_change_confirmation\"]");
            }
            if (parameters.ContainsKey("smtp_password") && !(parameters["smtp_password"] is string ))
            {
                throw new ArgumentException("Bad parameter: smtp_password must be of type string", "parameters[\"smtp_password\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/site", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Site>(responseJson);
        }


    }
}

