using FilesCom.Util;
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

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("additional_text_file_types"))
            {
                this.attributes.Add("additional_text_file_types", new string[0]);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_sms"))
            {
                this.attributes.Add("allowed_2fa_method_sms", false);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_totp"))
            {
                this.attributes.Add("allowed_2fa_method_totp", false);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_webauthn"))
            {
                this.attributes.Add("allowed_2fa_method_webauthn", false);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_yubi"))
            {
                this.attributes.Add("allowed_2fa_method_yubi", false);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_email"))
            {
                this.attributes.Add("allowed_2fa_method_email", false);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_static"))
            {
                this.attributes.Add("allowed_2fa_method_static", false);
            }
            if (!this.attributes.ContainsKey("allowed_2fa_method_bypass_for_ftp_sftp_dav"))
            {
                this.attributes.Add("allowed_2fa_method_bypass_for_ftp_sftp_dav", false);
            }
            if (!this.attributes.ContainsKey("admin_user_id"))
            {
                this.attributes.Add("admin_user_id", null);
            }
            if (!this.attributes.ContainsKey("admins_bypass_locked_subfolders"))
            {
                this.attributes.Add("admins_bypass_locked_subfolders", false);
            }
            if (!this.attributes.ContainsKey("allow_bundle_names"))
            {
                this.attributes.Add("allow_bundle_names", false);
            }
            if (!this.attributes.ContainsKey("allowed_countries"))
            {
                this.attributes.Add("allowed_countries", null);
            }
            if (!this.attributes.ContainsKey("allowed_ips"))
            {
                this.attributes.Add("allowed_ips", null);
            }
            if (!this.attributes.ContainsKey("always_mkdir_parents"))
            {
                this.attributes.Add("always_mkdir_parents", false);
            }
            if (!this.attributes.ContainsKey("ask_about_overwrites"))
            {
                this.attributes.Add("ask_about_overwrites", false);
            }
            if (!this.attributes.ContainsKey("bundle_activity_notifications"))
            {
                this.attributes.Add("bundle_activity_notifications", null);
            }
            if (!this.attributes.ContainsKey("bundle_expiration"))
            {
                this.attributes.Add("bundle_expiration", null);
            }
            if (!this.attributes.ContainsKey("bundle_not_found_message"))
            {
                this.attributes.Add("bundle_not_found_message", null);
            }
            if (!this.attributes.ContainsKey("bundle_password_required"))
            {
                this.attributes.Add("bundle_password_required", false);
            }
            if (!this.attributes.ContainsKey("bundle_recipient_blacklist_domains"))
            {
                this.attributes.Add("bundle_recipient_blacklist_domains", new string[0]);
            }
            if (!this.attributes.ContainsKey("bundle_recipient_blacklist_free_email_domains"))
            {
                this.attributes.Add("bundle_recipient_blacklist_free_email_domains", false);
            }
            if (!this.attributes.ContainsKey("bundle_registration_notifications"))
            {
                this.attributes.Add("bundle_registration_notifications", null);
            }
            if (!this.attributes.ContainsKey("bundle_require_registration"))
            {
                this.attributes.Add("bundle_require_registration", false);
            }
            if (!this.attributes.ContainsKey("bundle_require_share_recipient"))
            {
                this.attributes.Add("bundle_require_share_recipient", false);
            }
            if (!this.attributes.ContainsKey("bundle_require_note"))
            {
                this.attributes.Add("bundle_require_note", false);
            }
            if (!this.attributes.ContainsKey("bundle_send_shared_receipts"))
            {
                this.attributes.Add("bundle_send_shared_receipts", false);
            }
            if (!this.attributes.ContainsKey("bundle_upload_receipt_notifications"))
            {
                this.attributes.Add("bundle_upload_receipt_notifications", null);
            }
            if (!this.attributes.ContainsKey("bundle_watermark_attachment"))
            {
                this.attributes.Add("bundle_watermark_attachment", null);
            }
            if (!this.attributes.ContainsKey("bundle_watermark_value"))
            {
                this.attributes.Add("bundle_watermark_value", null);
            }
            if (!this.attributes.ContainsKey("calculate_file_checksums_crc32"))
            {
                this.attributes.Add("calculate_file_checksums_crc32", false);
            }
            if (!this.attributes.ContainsKey("calculate_file_checksums_md5"))
            {
                this.attributes.Add("calculate_file_checksums_md5", false);
            }
            if (!this.attributes.ContainsKey("calculate_file_checksums_sha1"))
            {
                this.attributes.Add("calculate_file_checksums_sha1", false);
            }
            if (!this.attributes.ContainsKey("calculate_file_checksums_sha256"))
            {
                this.attributes.Add("calculate_file_checksums_sha256", false);
            }
            if (!this.attributes.ContainsKey("uploads_via_email_authentication"))
            {
                this.attributes.Add("uploads_via_email_authentication", false);
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
            if (!this.attributes.ContainsKey("contact_name"))
            {
                this.attributes.Add("contact_name", null);
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
                this.attributes.Add("custom_namespace", false);
            }
            if (!this.attributes.ContainsKey("dav_enabled"))
            {
                this.attributes.Add("dav_enabled", false);
            }
            if (!this.attributes.ContainsKey("dav_user_root_enabled"))
            {
                this.attributes.Add("dav_user_root_enabled", false);
            }
            if (!this.attributes.ContainsKey("days_before_deleting_disabled_users"))
            {
                this.attributes.Add("days_before_deleting_disabled_users", null);
            }
            if (!this.attributes.ContainsKey("days_to_retain_backups"))
            {
                this.attributes.Add("days_to_retain_backups", null);
            }
            if (!this.attributes.ContainsKey("document_edits_in_bundle_allowed"))
            {
                this.attributes.Add("document_edits_in_bundle_allowed", false);
            }
            if (!this.attributes.ContainsKey("default_time_zone"))
            {
                this.attributes.Add("default_time_zone", null);
            }
            if (!this.attributes.ContainsKey("desktop_app"))
            {
                this.attributes.Add("desktop_app", false);
            }
            if (!this.attributes.ContainsKey("desktop_app_session_ip_pinning"))
            {
                this.attributes.Add("desktop_app_session_ip_pinning", false);
            }
            if (!this.attributes.ContainsKey("desktop_app_session_lifetime"))
            {
                this.attributes.Add("desktop_app_session_lifetime", null);
            }
            if (!this.attributes.ContainsKey("legacy_checksums_mode"))
            {
                this.attributes.Add("legacy_checksums_mode", false);
            }
            if (!this.attributes.ContainsKey("mobile_app"))
            {
                this.attributes.Add("mobile_app", false);
            }
            if (!this.attributes.ContainsKey("mobile_app_session_ip_pinning"))
            {
                this.attributes.Add("mobile_app_session_ip_pinning", false);
            }
            if (!this.attributes.ContainsKey("mobile_app_session_lifetime"))
            {
                this.attributes.Add("mobile_app_session_lifetime", null);
            }
            if (!this.attributes.ContainsKey("disallowed_countries"))
            {
                this.attributes.Add("disallowed_countries", null);
            }
            if (!this.attributes.ContainsKey("disable_files_certificate_generation"))
            {
                this.attributes.Add("disable_files_certificate_generation", false);
            }
            if (!this.attributes.ContainsKey("disable_notifications"))
            {
                this.attributes.Add("disable_notifications", false);
            }
            if (!this.attributes.ContainsKey("disable_password_reset"))
            {
                this.attributes.Add("disable_password_reset", false);
            }
            if (!this.attributes.ContainsKey("domain"))
            {
                this.attributes.Add("domain", null);
            }
            if (!this.attributes.ContainsKey("domain_hsts_header"))
            {
                this.attributes.Add("domain_hsts_header", false);
            }
            if (!this.attributes.ContainsKey("domain_letsencrypt_chain"))
            {
                this.attributes.Add("domain_letsencrypt_chain", null);
            }
            if (!this.attributes.ContainsKey("email"))
            {
                this.attributes.Add("email", "");
            }
            if (!this.attributes.ContainsKey("ftp_enabled"))
            {
                this.attributes.Add("ftp_enabled", false);
            }
            if (!this.attributes.ContainsKey("reply_to_email"))
            {
                this.attributes.Add("reply_to_email", "");
            }
            if (!this.attributes.ContainsKey("non_sso_groups_allowed"))
            {
                this.attributes.Add("non_sso_groups_allowed", false);
            }
            if (!this.attributes.ContainsKey("non_sso_users_allowed"))
            {
                this.attributes.Add("non_sso_users_allowed", false);
            }
            if (!this.attributes.ContainsKey("folder_permissions_groups_only"))
            {
                this.attributes.Add("folder_permissions_groups_only", false);
            }
            if (!this.attributes.ContainsKey("hipaa"))
            {
                this.attributes.Add("hipaa", false);
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
                this.attributes.Add("include_password_in_welcome_email", false);
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
                this.attributes.Add("ldap_enabled", false);
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
                this.attributes.Add("ldap_secure", false);
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
            if (!this.attributes.ContainsKey("login_page_background_image"))
            {
                this.attributes.Add("login_page_background_image", null);
            }
            if (!this.attributes.ContainsKey("max_prior_passwords"))
            {
                this.attributes.Add("max_prior_passwords", null);
            }
            if (!this.attributes.ContainsKey("motd_text"))
            {
                this.attributes.Add("motd_text", null);
            }
            if (!this.attributes.ContainsKey("motd_use_for_ftp"))
            {
                this.attributes.Add("motd_use_for_ftp", false);
            }
            if (!this.attributes.ContainsKey("motd_use_for_sftp"))
            {
                this.attributes.Add("motd_use_for_sftp", false);
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
                this.attributes.Add("office_integration_available", false);
            }
            if (!this.attributes.ContainsKey("office_integration_type"))
            {
                this.attributes.Add("office_integration_type", null);
            }
            if (!this.attributes.ContainsKey("oncehub_link"))
            {
                this.attributes.Add("oncehub_link", null);
            }
            if (!this.attributes.ContainsKey("opt_out_global"))
            {
                this.attributes.Add("opt_out_global", false);
            }
            if (!this.attributes.ContainsKey("overdue"))
            {
                this.attributes.Add("overdue", false);
            }
            if (!this.attributes.ContainsKey("password_min_length"))
            {
                this.attributes.Add("password_min_length", null);
            }
            if (!this.attributes.ContainsKey("password_require_letter"))
            {
                this.attributes.Add("password_require_letter", false);
            }
            if (!this.attributes.ContainsKey("password_require_mixed"))
            {
                this.attributes.Add("password_require_mixed", false);
            }
            if (!this.attributes.ContainsKey("password_require_number"))
            {
                this.attributes.Add("password_require_number", false);
            }
            if (!this.attributes.ContainsKey("password_require_special"))
            {
                this.attributes.Add("password_require_special", false);
            }
            if (!this.attributes.ContainsKey("password_require_unbreached"))
            {
                this.attributes.Add("password_require_unbreached", false);
            }
            if (!this.attributes.ContainsKey("password_requirements_apply_to_bundles"))
            {
                this.attributes.Add("password_requirements_apply_to_bundles", false);
            }
            if (!this.attributes.ContainsKey("password_validity_days"))
            {
                this.attributes.Add("password_validity_days", null);
            }
            if (!this.attributes.ContainsKey("phone"))
            {
                this.attributes.Add("phone", null);
            }
            if (!this.attributes.ContainsKey("pin_all_remote_servers_to_site_region"))
            {
                this.attributes.Add("pin_all_remote_servers_to_site_region", false);
            }
            if (!this.attributes.ContainsKey("prevent_root_permissions_for_non_site_admins"))
            {
                this.attributes.Add("prevent_root_permissions_for_non_site_admins", false);
            }
            if (!this.attributes.ContainsKey("protocol_access_groups_only"))
            {
                this.attributes.Add("protocol_access_groups_only", false);
            }
            if (!this.attributes.ContainsKey("require_2fa"))
            {
                this.attributes.Add("require_2fa", false);
            }
            if (!this.attributes.ContainsKey("require_2fa_stop_time"))
            {
                this.attributes.Add("require_2fa_stop_time", null);
            }
            if (!this.attributes.ContainsKey("require_2fa_user_type"))
            {
                this.attributes.Add("require_2fa_user_type", null);
            }
            if (!this.attributes.ContainsKey("require_logout_from_bundles_and_inboxes"))
            {
                this.attributes.Add("require_logout_from_bundles_and_inboxes", false);
            }
            if (!this.attributes.ContainsKey("session"))
            {
                this.attributes.Add("session", null);
            }
            if (!this.attributes.ContainsKey("sftp_enabled"))
            {
                this.attributes.Add("sftp_enabled", false);
            }
            if (!this.attributes.ContainsKey("sftp_host_key_type"))
            {
                this.attributes.Add("sftp_host_key_type", null);
            }
            if (!this.attributes.ContainsKey("active_sftp_host_key_id"))
            {
                this.attributes.Add("active_sftp_host_key_id", null);
            }
            if (!this.attributes.ContainsKey("sftp_insecure_ciphers"))
            {
                this.attributes.Add("sftp_insecure_ciphers", false);
            }
            if (!this.attributes.ContainsKey("sftp_insecure_diffie_hellman"))
            {
                this.attributes.Add("sftp_insecure_diffie_hellman", false);
            }
            if (!this.attributes.ContainsKey("sftp_user_root_enabled"))
            {
                this.attributes.Add("sftp_user_root_enabled", false);
            }
            if (!this.attributes.ContainsKey("sharing_enabled"))
            {
                this.attributes.Add("sharing_enabled", false);
            }
            if (!this.attributes.ContainsKey("show_user_notifications_log_in_link"))
            {
                this.attributes.Add("show_user_notifications_log_in_link", false);
            }
            if (!this.attributes.ContainsKey("show_request_access_link"))
            {
                this.attributes.Add("show_request_access_link", false);
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
            if (!this.attributes.ContainsKey("session_expiry_minutes"))
            {
                this.attributes.Add("session_expiry_minutes", null);
            }
            if (!this.attributes.ContainsKey("ssl_required"))
            {
                this.attributes.Add("ssl_required", false);
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
                this.attributes.Add("tls_disabled", false);
            }
            if (!this.attributes.ContainsKey("trial_days_left"))
            {
                this.attributes.Add("trial_days_left", null);
            }
            if (!this.attributes.ContainsKey("trial_until"))
            {
                this.attributes.Add("trial_until", null);
            }
            if (!this.attributes.ContainsKey("use_dedicated_ips_for_smtp"))
            {
                this.attributes.Add("use_dedicated_ips_for_smtp", false);
            }
            if (!this.attributes.ContainsKey("use_provided_modified_at"))
            {
                this.attributes.Add("use_provided_modified_at", false);
            }
            if (!this.attributes.ContainsKey("user"))
            {
                this.attributes.Add("user", null);
            }
            if (!this.attributes.ContainsKey("user_lockout"))
            {
                this.attributes.Add("user_lockout", false);
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
                this.attributes.Add("user_requests_enabled", false);
            }
            if (!this.attributes.ContainsKey("user_requests_notify_admins"))
            {
                this.attributes.Add("user_requests_notify_admins", false);
            }
            if (!this.attributes.ContainsKey("users_can_create_api_keys"))
            {
                this.attributes.Add("users_can_create_api_keys", false);
            }
            if (!this.attributes.ContainsKey("users_can_create_ssh_keys"))
            {
                this.attributes.Add("users_can_create_ssh_keys", false);
            }
            if (!this.attributes.ContainsKey("welcome_custom_text"))
            {
                this.attributes.Add("welcome_custom_text", null);
            }
            if (!this.attributes.ContainsKey("welcome_email_cc"))
            {
                this.attributes.Add("welcome_email_cc", "");
            }
            if (!this.attributes.ContainsKey("welcome_email_subject"))
            {
                this.attributes.Add("welcome_email_subject", null);
            }
            if (!this.attributes.ContainsKey("welcome_email_enabled"))
            {
                this.attributes.Add("welcome_email_enabled", false);
            }
            if (!this.attributes.ContainsKey("welcome_screen"))
            {
                this.attributes.Add("welcome_screen", null);
            }
            if (!this.attributes.ContainsKey("windows_mode_ftp"))
            {
                this.attributes.Add("windows_mode_ftp", false);
            }
            if (!this.attributes.ContainsKey("disable_users_from_inactivity_period_days"))
            {
                this.attributes.Add("disable_users_from_inactivity_period_days", null);
            }
            if (!this.attributes.ContainsKey("group_admins_can_set_user_password"))
            {
                this.attributes.Add("group_admins_can_set_user_password", false);
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
        /// Site Id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Site name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            private set { attributes["name"] = value; }
        }

        /// <summary>
        /// Additional extensions that are considered text files
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("additional_text_file_types")]
        public string[] AdditionalTextFileTypes
        {
            get { return (string[])attributes["additional_text_file_types"]; }
            private set { attributes["additional_text_file_types"] = value; }
        }

        /// <summary>
        /// Is SMS two factor authentication allowed?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allowed_2fa_method_sms")]
        public bool Allowed2faMethodSms
        {
            get { return attributes["allowed_2fa_method_sms"] == null ? false : (bool)attributes["allowed_2fa_method_sms"]; }
            private set { attributes["allowed_2fa_method_sms"] = value; }
        }

        /// <summary>
        /// Is TOTP two factor authentication allowed?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allowed_2fa_method_totp")]
        public bool Allowed2faMethodTotp
        {
            get { return attributes["allowed_2fa_method_totp"] == null ? false : (bool)attributes["allowed_2fa_method_totp"]; }
            private set { attributes["allowed_2fa_method_totp"] = value; }
        }

        /// <summary>
        /// Is WebAuthn two factor authentication allowed?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allowed_2fa_method_webauthn")]
        public bool Allowed2faMethodWebauthn
        {
            get { return attributes["allowed_2fa_method_webauthn"] == null ? false : (bool)attributes["allowed_2fa_method_webauthn"]; }
            private set { attributes["allowed_2fa_method_webauthn"] = value; }
        }

        /// <summary>
        /// Is yubikey two factor authentication allowed?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allowed_2fa_method_yubi")]
        public bool Allowed2faMethodYubi
        {
            get { return attributes["allowed_2fa_method_yubi"] == null ? false : (bool)attributes["allowed_2fa_method_yubi"]; }
            private set { attributes["allowed_2fa_method_yubi"] = value; }
        }

        /// <summary>
        /// Is OTP via email two factor authentication allowed?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allowed_2fa_method_email")]
        public bool Allowed2faMethodEmail
        {
            get { return attributes["allowed_2fa_method_email"] == null ? false : (bool)attributes["allowed_2fa_method_email"]; }
            private set { attributes["allowed_2fa_method_email"] = value; }
        }

        /// <summary>
        /// Is OTP via static codes for two factor authentication allowed?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allowed_2fa_method_static")]
        public bool Allowed2faMethodStatic
        {
            get { return attributes["allowed_2fa_method_static"] == null ? false : (bool)attributes["allowed_2fa_method_static"]; }
            private set { attributes["allowed_2fa_method_static"] = value; }
        }

        /// <summary>
        /// Are users allowed to configure their two factor authentication to be bypassed for FTP/SFTP/WebDAV?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allowed_2fa_method_bypass_for_ftp_sftp_dav")]
        public bool Allowed2faMethodBypassForFtpSftpDav
        {
            get { return attributes["allowed_2fa_method_bypass_for_ftp_sftp_dav"] == null ? false : (bool)attributes["allowed_2fa_method_bypass_for_ftp_sftp_dav"]; }
            private set { attributes["allowed_2fa_method_bypass_for_ftp_sftp_dav"] = value; }
        }

        /// <summary>
        /// User ID for the main site administrator
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("admin_user_id")]
        public Nullable<Int64> AdminUserId
        {
            get { return (Nullable<Int64>)attributes["admin_user_id"]; }
            private set { attributes["admin_user_id"] = value; }
        }

        /// <summary>
        /// Allow admins to bypass the locked subfolders setting.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("admins_bypass_locked_subfolders")]
        public bool AdminsBypassLockedSubfolders
        {
            get { return attributes["admins_bypass_locked_subfolders"] == null ? false : (bool)attributes["admins_bypass_locked_subfolders"]; }
            private set { attributes["admins_bypass_locked_subfolders"] = value; }
        }

        /// <summary>
        /// Are manual Bundle names allowed?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_bundle_names")]
        public bool AllowBundleNames
        {
            get { return attributes["allow_bundle_names"] == null ? false : (bool)attributes["allow_bundle_names"]; }
            private set { attributes["allow_bundle_names"] = value; }
        }

        /// <summary>
        /// Comma separated list of allowed Country codes
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("allowed_countries")]
        public string AllowedCountries
        {
            get { return (string)attributes["allowed_countries"]; }
            private set { attributes["allowed_countries"] = value; }
        }

        /// <summary>
        /// List of allowed IP addresses
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("allowed_ips")]
        public string AllowedIps
        {
            get { return (string)attributes["allowed_ips"]; }
            private set { attributes["allowed_ips"] = value; }
        }

        /// <summary>
        /// Create parent directories if they do not exist during uploads?  This is primarily used to work around broken upload clients that assume servers will perform this step.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("always_mkdir_parents")]
        public bool AlwaysMkdirParents
        {
            get { return attributes["always_mkdir_parents"] == null ? false : (bool)attributes["always_mkdir_parents"]; }
            private set { attributes["always_mkdir_parents"] = value; }
        }

        /// <summary>
        /// If false, rename conflicting files instead of asking for overwrite confirmation.  Only applies to web interface.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ask_about_overwrites")]
        public bool AskAboutOverwrites
        {
            get { return attributes["ask_about_overwrites"] == null ? false : (bool)attributes["ask_about_overwrites"]; }
            private set { attributes["ask_about_overwrites"] = value; }
        }

        /// <summary>
        /// Do Bundle owners receive activity notifications?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_activity_notifications")]
        public string BundleActivityNotifications
        {
            get { return (string)attributes["bundle_activity_notifications"]; }
            private set { attributes["bundle_activity_notifications"] = value; }
        }

        /// <summary>
        /// Site-wide Bundle expiration in days
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_expiration")]
        public Nullable<Int64> BundleExpiration
        {
            get { return (Nullable<Int64>)attributes["bundle_expiration"]; }
            private set { attributes["bundle_expiration"] = value; }
        }

        /// <summary>
        /// Custom error message to show when bundle is not found.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_not_found_message")]
        public string BundleNotFoundMessage
        {
            get { return (string)attributes["bundle_not_found_message"]; }
            private set { attributes["bundle_not_found_message"] = value; }
        }

        /// <summary>
        /// Do Bundles require password protection?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bundle_password_required")]
        public bool BundlePasswordRequired
        {
            get { return attributes["bundle_password_required"] == null ? false : (bool)attributes["bundle_password_required"]; }
            private set { attributes["bundle_password_required"] = value; }
        }

        /// <summary>
        /// List of email domains to disallow when entering a Bundle/Inbox recipients
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_recipient_blacklist_domains")]
        public string[] BundleRecipientBlacklistDomains
        {
            get { return (string[])attributes["bundle_recipient_blacklist_domains"]; }
            private set { attributes["bundle_recipient_blacklist_domains"] = value; }
        }

        /// <summary>
        /// Disallow free email domains for Bundle/Inbox recipients?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bundle_recipient_blacklist_free_email_domains")]
        public bool BundleRecipientBlacklistFreeEmailDomains
        {
            get { return attributes["bundle_recipient_blacklist_free_email_domains"] == null ? false : (bool)attributes["bundle_recipient_blacklist_free_email_domains"]; }
            private set { attributes["bundle_recipient_blacklist_free_email_domains"] = value; }
        }

        /// <summary>
        /// Do Bundle owners receive registration notification?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_registration_notifications")]
        public string BundleRegistrationNotifications
        {
            get { return (string)attributes["bundle_registration_notifications"]; }
            private set { attributes["bundle_registration_notifications"] = value; }
        }

        /// <summary>
        /// Do Bundles require registration?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bundle_require_registration")]
        public bool BundleRequireRegistration
        {
            get { return attributes["bundle_require_registration"] == null ? false : (bool)attributes["bundle_require_registration"]; }
            private set { attributes["bundle_require_registration"] = value; }
        }

        /// <summary>
        /// Do Bundles require recipients for sharing?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bundle_require_share_recipient")]
        public bool BundleRequireShareRecipient
        {
            get { return attributes["bundle_require_share_recipient"] == null ? false : (bool)attributes["bundle_require_share_recipient"]; }
            private set { attributes["bundle_require_share_recipient"] = value; }
        }

        /// <summary>
        /// Do Bundles require internal notes?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bundle_require_note")]
        public bool BundleRequireNote
        {
            get { return attributes["bundle_require_note"] == null ? false : (bool)attributes["bundle_require_note"]; }
            private set { attributes["bundle_require_note"] = value; }
        }

        /// <summary>
        /// Do Bundle creators receive receipts of invitations?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bundle_send_shared_receipts")]
        public bool BundleSendSharedReceipts
        {
            get { return attributes["bundle_send_shared_receipts"] == null ? false : (bool)attributes["bundle_send_shared_receipts"]; }
            private set { attributes["bundle_send_shared_receipts"] = value; }
        }

        /// <summary>
        /// Do Bundle uploaders receive upload confirmation notifications?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_upload_receipt_notifications")]
        public string BundleUploadReceiptNotifications
        {
            get { return (string)attributes["bundle_upload_receipt_notifications"]; }
            private set { attributes["bundle_upload_receipt_notifications"] = value; }
        }

        /// <summary>
        /// Preview watermark image applied to all bundle items.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_watermark_attachment")]
        public Image BundleWatermarkAttachment
        {
            get { return (Image)attributes["bundle_watermark_attachment"]; }
            private set { attributes["bundle_watermark_attachment"] = value; }
        }

        /// <summary>
        /// Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bundle_watermark_value")]
        public object BundleWatermarkValue
        {
            get { return (object)attributes["bundle_watermark_value"]; }
            private set { attributes["bundle_watermark_value"] = value; }
        }

        /// <summary>
        /// Calculate CRC32 checksums for files?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("calculate_file_checksums_crc32")]
        public bool CalculateFileChecksumsCrc32
        {
            get { return attributes["calculate_file_checksums_crc32"] == null ? false : (bool)attributes["calculate_file_checksums_crc32"]; }
            private set { attributes["calculate_file_checksums_crc32"] = value; }
        }

        /// <summary>
        /// Calculate MD5 checksums for files?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("calculate_file_checksums_md5")]
        public bool CalculateFileChecksumsMd5
        {
            get { return attributes["calculate_file_checksums_md5"] == null ? false : (bool)attributes["calculate_file_checksums_md5"]; }
            private set { attributes["calculate_file_checksums_md5"] = value; }
        }

        /// <summary>
        /// Calculate SHA1 checksums for files?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("calculate_file_checksums_sha1")]
        public bool CalculateFileChecksumsSha1
        {
            get { return attributes["calculate_file_checksums_sha1"] == null ? false : (bool)attributes["calculate_file_checksums_sha1"]; }
            private set { attributes["calculate_file_checksums_sha1"] = value; }
        }

        /// <summary>
        /// Calculate SHA256 checksums for files?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("calculate_file_checksums_sha256")]
        public bool CalculateFileChecksumsSha256
        {
            get { return attributes["calculate_file_checksums_sha256"] == null ? false : (bool)attributes["calculate_file_checksums_sha256"]; }
            private set { attributes["calculate_file_checksums_sha256"] = value; }
        }

        /// <summary>
        /// Do incoming emails in the Inboxes require checking for SPF/DKIM/DMARC?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("uploads_via_email_authentication")]
        public bool UploadsViaEmailAuthentication
        {
            get { return attributes["uploads_via_email_authentication"] == null ? false : (bool)attributes["uploads_via_email_authentication"]; }
            private set { attributes["uploads_via_email_authentication"] = value; }
        }

        /// <summary>
        /// Page link and button color
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("color2_left")]
        public string Color2Left
        {
            get { return (string)attributes["color2_left"]; }
            private set { attributes["color2_left"] = value; }
        }

        /// <summary>
        /// Top bar link color
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("color2_link")]
        public string Color2Link
        {
            get { return (string)attributes["color2_link"]; }
            private set { attributes["color2_link"] = value; }
        }

        /// <summary>
        /// Page link and button color
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("color2_text")]
        public string Color2Text
        {
            get { return (string)attributes["color2_text"]; }
            private set { attributes["color2_text"] = value; }
        }

        /// <summary>
        /// Top bar background color
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("color2_top")]
        public string Color2Top
        {
            get { return (string)attributes["color2_top"]; }
            private set { attributes["color2_top"] = value; }
        }

        /// <summary>
        /// Top bar text color
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("color2_top_text")]
        public string Color2TopText
        {
            get { return (string)attributes["color2_top_text"]; }
            private set { attributes["color2_top_text"] = value; }
        }

        /// <summary>
        /// Site main contact name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("contact_name")]
        public string ContactName
        {
            get { return (string)attributes["contact_name"]; }
            private set { attributes["contact_name"] = value; }
        }

        /// <summary>
        /// Time this site was created
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Preferred currency
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("currency")]
        public string Currency
        {
            get { return (string)attributes["currency"]; }
            private set { attributes["currency"] = value; }
        }

        /// <summary>
        /// Is this site using a custom namespace for users?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("custom_namespace")]
        public bool CustomNamespace
        {
            get { return attributes["custom_namespace"] == null ? false : (bool)attributes["custom_namespace"]; }
            private set { attributes["custom_namespace"] = value; }
        }

        /// <summary>
        /// Is WebDAV enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dav_enabled")]
        public bool DavEnabled
        {
            get { return attributes["dav_enabled"] == null ? false : (bool)attributes["dav_enabled"]; }
            private set { attributes["dav_enabled"] = value; }
        }

        /// <summary>
        /// Use user FTP roots also for WebDAV?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dav_user_root_enabled")]
        public bool DavUserRootEnabled
        {
            get { return attributes["dav_user_root_enabled"] == null ? false : (bool)attributes["dav_user_root_enabled"]; }
            private set { attributes["dav_user_root_enabled"] = value; }
        }

        /// <summary>
        /// Number of days to keep disabled users before deleting them. If set to 0, disabled users will not be deleted.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("days_before_deleting_disabled_users")]
        public Nullable<Int64> DaysBeforeDeletingDisabledUsers
        {
            get { return (Nullable<Int64>)attributes["days_before_deleting_disabled_users"]; }
            private set { attributes["days_before_deleting_disabled_users"] = value; }
        }

        /// <summary>
        /// Number of days to keep deleted files
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("days_to_retain_backups")]
        public Nullable<Int64> DaysToRetainBackups
        {
            get { return (Nullable<Int64>)attributes["days_to_retain_backups"]; }
            private set { attributes["days_to_retain_backups"] = value; }
        }

        /// <summary>
        /// If true, allow public viewers of Bundles with full permissions to use document editing integrations.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("document_edits_in_bundle_allowed")]
        public bool DocumentEditsInBundleAllowed
        {
            get { return attributes["document_edits_in_bundle_allowed"] == null ? false : (bool)attributes["document_edits_in_bundle_allowed"]; }
            private set { attributes["document_edits_in_bundle_allowed"] = value; }
        }

        /// <summary>
        /// Site default time zone
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("default_time_zone")]
        public string DefaultTimeZone
        {
            get { return (string)attributes["default_time_zone"]; }
            private set { attributes["default_time_zone"] = value; }
        }

        /// <summary>
        /// Is the desktop app enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("desktop_app")]
        public bool DesktopApp
        {
            get { return attributes["desktop_app"] == null ? false : (bool)attributes["desktop_app"]; }
            private set { attributes["desktop_app"] = value; }
        }

        /// <summary>
        /// Is desktop app session IP pinning enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("desktop_app_session_ip_pinning")]
        public bool DesktopAppSessionIpPinning
        {
            get { return attributes["desktop_app_session_ip_pinning"] == null ? false : (bool)attributes["desktop_app_session_ip_pinning"]; }
            private set { attributes["desktop_app_session_ip_pinning"] = value; }
        }

        /// <summary>
        /// Desktop app session lifetime (in hours)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("desktop_app_session_lifetime")]
        public Nullable<Int64> DesktopAppSessionLifetime
        {
            get { return (Nullable<Int64>)attributes["desktop_app_session_lifetime"]; }
            private set { attributes["desktop_app_session_lifetime"] = value; }
        }

        /// <summary>
        /// Use legacy checksums mode?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("legacy_checksums_mode")]
        public bool LegacyChecksumsMode
        {
            get { return attributes["legacy_checksums_mode"] == null ? false : (bool)attributes["legacy_checksums_mode"]; }
            private set { attributes["legacy_checksums_mode"] = value; }
        }

        /// <summary>
        /// Is the mobile app enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mobile_app")]
        public bool MobileApp
        {
            get { return attributes["mobile_app"] == null ? false : (bool)attributes["mobile_app"]; }
            private set { attributes["mobile_app"] = value; }
        }

        /// <summary>
        /// Is mobile app session IP pinning enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mobile_app_session_ip_pinning")]
        public bool MobileAppSessionIpPinning
        {
            get { return attributes["mobile_app_session_ip_pinning"] == null ? false : (bool)attributes["mobile_app_session_ip_pinning"]; }
            private set { attributes["mobile_app_session_ip_pinning"] = value; }
        }

        /// <summary>
        /// Mobile app session lifetime (in hours)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("mobile_app_session_lifetime")]
        public Nullable<Int64> MobileAppSessionLifetime
        {
            get { return (Nullable<Int64>)attributes["mobile_app_session_lifetime"]; }
            private set { attributes["mobile_app_session_lifetime"] = value; }
        }

        /// <summary>
        /// Comma separated list of disallowed Country codes
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("disallowed_countries")]
        public string DisallowedCountries
        {
            get { return (string)attributes["disallowed_countries"]; }
            private set { attributes["disallowed_countries"] = value; }
        }

        /// <summary>
        /// If set, Files.com will not set the CAA records required to generate future SSL certificates for this domain.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disable_files_certificate_generation")]
        public bool DisableFilesCertificateGeneration
        {
            get { return attributes["disable_files_certificate_generation"] == null ? false : (bool)attributes["disable_files_certificate_generation"]; }
            private set { attributes["disable_files_certificate_generation"] = value; }
        }

        /// <summary>
        /// Are notifications disabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disable_notifications")]
        public bool DisableNotifications
        {
            get { return attributes["disable_notifications"] == null ? false : (bool)attributes["disable_notifications"]; }
            private set { attributes["disable_notifications"] = value; }
        }

        /// <summary>
        /// Is password reset disabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disable_password_reset")]
        public bool DisablePasswordReset
        {
            get { return attributes["disable_password_reset"] == null ? false : (bool)attributes["disable_password_reset"]; }
            private set { attributes["disable_password_reset"] = value; }
        }

        /// <summary>
        /// Custom domain
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("domain")]
        public string Domain
        {
            get { return (string)attributes["domain"]; }
            private set { attributes["domain"] = value; }
        }

        /// <summary>
        /// Send HSTS (HTTP Strict Transport Security) header when visitors access the site via a custom domain?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("domain_hsts_header")]
        public bool DomainHstsHeader
        {
            get { return attributes["domain_hsts_header"] == null ? false : (bool)attributes["domain_hsts_header"]; }
            private set { attributes["domain_hsts_header"] = value; }
        }

        /// <summary>
        /// Letsencrypt chain to use when registering SSL Certificate for domain.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("domain_letsencrypt_chain")]
        public string DomainLetsencryptChain
        {
            get { return (string)attributes["domain_letsencrypt_chain"]; }
            private set { attributes["domain_letsencrypt_chain"] = value; }
        }

        /// <summary>
        /// Main email for this site
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("email")]
        public string Email
        {
            get { return (string)attributes["email"]; }
            private set { attributes["email"] = value; }
        }

        /// <summary>
        /// Is FTP enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ftp_enabled")]
        public bool FtpEnabled
        {
            get { return attributes["ftp_enabled"] == null ? false : (bool)attributes["ftp_enabled"]; }
            private set { attributes["ftp_enabled"] = value; }
        }

        /// <summary>
        /// Reply-to email for this site
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("reply_to_email")]
        public string ReplyToEmail
        {
            get { return (string)attributes["reply_to_email"]; }
            private set { attributes["reply_to_email"] = value; }
        }

        /// <summary>
        /// If true, groups can be manually created / modified / deleted by Site Admins. Otherwise, groups can only be managed via your SSO provider.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("non_sso_groups_allowed")]
        public bool NonSsoGroupsAllowed
        {
            get { return attributes["non_sso_groups_allowed"] == null ? false : (bool)attributes["non_sso_groups_allowed"]; }
            private set { attributes["non_sso_groups_allowed"] = value; }
        }

        /// <summary>
        /// If true, users can be manually created / modified / deleted by Site Admins. Otherwise, users can only be managed via your SSO provider.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("non_sso_users_allowed")]
        public bool NonSsoUsersAllowed
        {
            get { return attributes["non_sso_users_allowed"] == null ? false : (bool)attributes["non_sso_users_allowed"]; }
            private set { attributes["non_sso_users_allowed"] = value; }
        }

        /// <summary>
        /// If true, permissions for this site must be bound to a group (not a user). Otherwise, permissions must be bound to a user.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("folder_permissions_groups_only")]
        public bool FolderPermissionsGroupsOnly
        {
            get { return attributes["folder_permissions_groups_only"] == null ? false : (bool)attributes["folder_permissions_groups_only"]; }
            private set { attributes["folder_permissions_groups_only"] = value; }
        }

        /// <summary>
        /// Is there a signed HIPAA BAA between Files.com and this site?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("hipaa")]
        public bool Hipaa
        {
            get { return attributes["hipaa"] == null ? false : (bool)attributes["hipaa"]; }
            private set { attributes["hipaa"] = value; }
        }

        /// <summary>
        /// Branded icon 128x128
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("icon128")]
        public Image Icon128
        {
            get { return (Image)attributes["icon128"]; }
            private set { attributes["icon128"] = value; }
        }

        /// <summary>
        /// Branded icon 16x16
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("icon16")]
        public Image Icon16
        {
            get { return (Image)attributes["icon16"]; }
            private set { attributes["icon16"] = value; }
        }

        /// <summary>
        /// Branded icon 32x32
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("icon32")]
        public Image Icon32
        {
            get { return (Image)attributes["icon32"]; }
            private set { attributes["icon32"] = value; }
        }

        /// <summary>
        /// Branded icon 48x48
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("icon48")]
        public Image Icon48
        {
            get { return (Image)attributes["icon48"]; }
            private set { attributes["icon48"] = value; }
        }

        /// <summary>
        /// Can files be modified?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("immutable_files_set_at")]
        public Nullable<DateTime> ImmutableFilesSetAt
        {
            get { return (Nullable<DateTime>)attributes["immutable_files_set_at"]; }
            private set { attributes["immutable_files_set_at"] = value; }
        }

        /// <summary>
        /// Include password in emails to new users?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("include_password_in_welcome_email")]
        public bool IncludePasswordInWelcomeEmail
        {
            get { return attributes["include_password_in_welcome_email"] == null ? false : (bool)attributes["include_password_in_welcome_email"]; }
            private set { attributes["include_password_in_welcome_email"] = value; }
        }

        /// <summary>
        /// Site default language
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("language")]
        public string Language
        {
            get { return (string)attributes["language"]; }
            private set { attributes["language"] = value; }
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
        /// Domain name that will be appended to usernames
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_domain")]
        public string LdapDomain
        {
            get { return (string)attributes["ldap_domain"]; }
            private set { attributes["ldap_domain"] = value; }
        }

        /// <summary>
        /// Main LDAP setting: is LDAP enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ldap_enabled")]
        public bool LdapEnabled
        {
            get { return attributes["ldap_enabled"] == null ? false : (bool)attributes["ldap_enabled"]; }
            private set { attributes["ldap_enabled"] = value; }
        }

        /// <summary>
        /// Should we sync groups from LDAP server?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_group_action")]
        public string LdapGroupAction
        {
            get { return (string)attributes["ldap_group_action"]; }
            private set { attributes["ldap_group_action"] = value; }
        }

        /// <summary>
        /// Comma or newline separated list of group names (with optional wildcards) to exclude when syncing.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_group_exclusion")]
        public string LdapGroupExclusion
        {
            get { return (string)attributes["ldap_group_exclusion"]; }
            private set { attributes["ldap_group_exclusion"] = value; }
        }

        /// <summary>
        /// Comma or newline separated list of group names (with optional wildcards) to include when syncing.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_group_inclusion")]
        public string LdapGroupInclusion
        {
            get { return (string)attributes["ldap_group_inclusion"]; }
            private set { attributes["ldap_group_inclusion"] = value; }
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
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ldap_secure")]
        public bool LdapSecure
        {
            get { return attributes["ldap_secure"] == null ? false : (bool)attributes["ldap_secure"]; }
            private set { attributes["ldap_secure"] = value; }
        }

        /// <summary>
        /// LDAP type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_type")]
        public string LdapType
        {
            get { return (string)attributes["ldap_type"]; }
            private set { attributes["ldap_type"] = value; }
        }

        /// <summary>
        /// Should we sync users from LDAP server?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_user_action")]
        public string LdapUserAction
        {
            get { return (string)attributes["ldap_user_action"]; }
            private set { attributes["ldap_user_action"] = value; }
        }

        /// <summary>
        /// Comma or newline separated list of group names (with optional wildcards) - if provided, only users in these groups will be added or synced.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_user_include_groups")]
        public string LdapUserIncludeGroups
        {
            get { return (string)attributes["ldap_user_include_groups"]; }
            private set { attributes["ldap_user_include_groups"] = value; }
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
        /// Login help text
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("login_help_text")]
        public string LoginHelpText
        {
            get { return (string)attributes["login_help_text"]; }
            private set { attributes["login_help_text"] = value; }
        }

        /// <summary>
        /// Branded logo
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logo")]
        public Image Logo
        {
            get { return (Image)attributes["logo"]; }
            private set { attributes["logo"] = value; }
        }

        /// <summary>
        /// Branded login page background
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("login_page_background_image")]
        public Image LoginPageBackgroundImage
        {
            get { return (Image)attributes["login_page_background_image"]; }
            private set { attributes["login_page_background_image"] = value; }
        }

        /// <summary>
        /// Number of prior passwords to disallow
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("max_prior_passwords")]
        public Nullable<Int64> MaxPriorPasswords
        {
            get { return (Nullable<Int64>)attributes["max_prior_passwords"]; }
            private set { attributes["max_prior_passwords"] = value; }
        }

        /// <summary>
        /// A message to show users when they connect via FTP or SFTP.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("motd_text")]
        public string MotdText
        {
            get { return (string)attributes["motd_text"]; }
            private set { attributes["motd_text"] = value; }
        }

        /// <summary>
        /// Show message to users connecting via FTP
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("motd_use_for_ftp")]
        public bool MotdUseForFtp
        {
            get { return attributes["motd_use_for_ftp"] == null ? false : (bool)attributes["motd_use_for_ftp"]; }
            private set { attributes["motd_use_for_ftp"] = value; }
        }

        /// <summary>
        /// Show message to users connecting via SFTP
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("motd_use_for_sftp")]
        public bool MotdUseForSftp
        {
            get { return attributes["motd_use_for_sftp"] == null ? false : (bool)attributes["motd_use_for_sftp"]; }
            private set { attributes["motd_use_for_sftp"] = value; }
        }

        /// <summary>
        /// Next billing amount
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("next_billing_amount")]
        public double NextBillingAmount
        {
            get { return (double)attributes["next_billing_amount"]; }
            private set { attributes["next_billing_amount"] = value; }
        }

        /// <summary>
        /// Next billing date
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("next_billing_date")]
        public string NextBillingDate
        {
            get { return (string)attributes["next_billing_date"]; }
            private set { attributes["next_billing_date"] = value; }
        }

        /// <summary>
        /// If true, allows users to use a document editing integration.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("office_integration_available")]
        public bool OfficeIntegrationAvailable
        {
            get { return attributes["office_integration_available"] == null ? false : (bool)attributes["office_integration_available"]; }
            private set { attributes["office_integration_available"] = value; }
        }

        /// <summary>
        /// Which document editing integration to support. Files.com Editor or Microsoft Office for the Web.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("office_integration_type")]
        public string OfficeIntegrationType
        {
            get { return (string)attributes["office_integration_type"]; }
            private set { attributes["office_integration_type"] = value; }
        }

        /// <summary>
        /// Link to scheduling a meeting with our Sales team
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("oncehub_link")]
        public string OncehubLink
        {
            get { return (string)attributes["oncehub_link"]; }
            private set { attributes["oncehub_link"] = value; }
        }

        /// <summary>
        /// Use servers in the USA only?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("opt_out_global")]
        public bool OptOutGlobal
        {
            get { return attributes["opt_out_global"] == null ? false : (bool)attributes["opt_out_global"]; }
            private set { attributes["opt_out_global"] = value; }
        }

        /// <summary>
        /// Is this site's billing overdue?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("overdue")]
        public bool Overdue
        {
            get { return attributes["overdue"] == null ? false : (bool)attributes["overdue"]; }
            private set { attributes["overdue"] = value; }
        }

        /// <summary>
        /// Shortest password length for users
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("password_min_length")]
        public Nullable<Int64> PasswordMinLength
        {
            get { return (Nullable<Int64>)attributes["password_min_length"]; }
            private set { attributes["password_min_length"] = value; }
        }

        /// <summary>
        /// Require a letter in passwords?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("password_require_letter")]
        public bool PasswordRequireLetter
        {
            get { return attributes["password_require_letter"] == null ? false : (bool)attributes["password_require_letter"]; }
            private set { attributes["password_require_letter"] = value; }
        }

        /// <summary>
        /// Require lower and upper case letters in passwords?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("password_require_mixed")]
        public bool PasswordRequireMixed
        {
            get { return attributes["password_require_mixed"] == null ? false : (bool)attributes["password_require_mixed"]; }
            private set { attributes["password_require_mixed"] = value; }
        }

        /// <summary>
        /// Require a number in passwords?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("password_require_number")]
        public bool PasswordRequireNumber
        {
            get { return attributes["password_require_number"] == null ? false : (bool)attributes["password_require_number"]; }
            private set { attributes["password_require_number"] = value; }
        }

        /// <summary>
        /// Require special characters in password?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("password_require_special")]
        public bool PasswordRequireSpecial
        {
            get { return attributes["password_require_special"] == null ? false : (bool)attributes["password_require_special"]; }
            private set { attributes["password_require_special"] = value; }
        }

        /// <summary>
        /// Require passwords that have not been previously breached? (see https://haveibeenpwned.com/)
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("password_require_unbreached")]
        public bool PasswordRequireUnbreached
        {
            get { return attributes["password_require_unbreached"] == null ? false : (bool)attributes["password_require_unbreached"]; }
            private set { attributes["password_require_unbreached"] = value; }
        }

        /// <summary>
        /// Require bundles' passwords, and passwords for other items (inboxes, public shares, etc.) to conform to the same requirements as users' passwords?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("password_requirements_apply_to_bundles")]
        public bool PasswordRequirementsApplyToBundles
        {
            get { return attributes["password_requirements_apply_to_bundles"] == null ? false : (bool)attributes["password_requirements_apply_to_bundles"]; }
            private set { attributes["password_requirements_apply_to_bundles"] = value; }
        }

        /// <summary>
        /// Number of days password is valid
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("password_validity_days")]
        public Nullable<Int64> PasswordValidityDays
        {
            get { return (Nullable<Int64>)attributes["password_validity_days"]; }
            private set { attributes["password_validity_days"] = value; }
        }

        /// <summary>
        /// Site phone number
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("phone")]
        public string Phone
        {
            get { return (string)attributes["phone"]; }
            private set { attributes["phone"] = value; }
        }

        /// <summary>
        /// If true, we will ensure that all internal communications with any remote server are made through the primary region of the site. This setting overrides individual remote server settings.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("pin_all_remote_servers_to_site_region")]
        public bool PinAllRemoteServersToSiteRegion
        {
            get { return attributes["pin_all_remote_servers_to_site_region"] == null ? false : (bool)attributes["pin_all_remote_servers_to_site_region"]; }
            private set { attributes["pin_all_remote_servers_to_site_region"] = value; }
        }

        /// <summary>
        /// If true, we will prevent non-administrators from receiving any permissions directly on the root folder.  This is commonly used to prevent the accidental application of permissions.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("prevent_root_permissions_for_non_site_admins")]
        public bool PreventRootPermissionsForNonSiteAdmins
        {
            get { return attributes["prevent_root_permissions_for_non_site_admins"] == null ? false : (bool)attributes["prevent_root_permissions_for_non_site_admins"]; }
            private set { attributes["prevent_root_permissions_for_non_site_admins"] = value; }
        }

        /// <summary>
        /// If true, protocol access permissions on users will be ignored, and only protocol access permissions set on Groups will be honored.  Make sure that your current user is a member of a group with API permission when changing this value to avoid locking yourself out of your site.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("protocol_access_groups_only")]
        public bool ProtocolAccessGroupsOnly
        {
            get { return attributes["protocol_access_groups_only"] == null ? false : (bool)attributes["protocol_access_groups_only"]; }
            private set { attributes["protocol_access_groups_only"] = value; }
        }

        /// <summary>
        /// Require two-factor authentication for all users?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("require_2fa")]
        public bool Require2fa
        {
            get { return attributes["require_2fa"] == null ? false : (bool)attributes["require_2fa"]; }
            private set { attributes["require_2fa"] = value; }
        }

        /// <summary>
        /// If set, requirement for two-factor authentication has been scheduled to end on this date-time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("require_2fa_stop_time")]
        public Nullable<DateTime> Require2faStopTime
        {
            get { return (Nullable<DateTime>)attributes["require_2fa_stop_time"]; }
            private set { attributes["require_2fa_stop_time"] = value; }
        }

        /// <summary>
        /// What type of user is required to use two-factor authentication (when require_2fa is set to `true` for this site)?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("require_2fa_user_type")]
        public string Require2faUserType
        {
            get { return (string)attributes["require_2fa_user_type"]; }
            private set { attributes["require_2fa_user_type"] = value; }
        }

        /// <summary>
        /// If true, we will hide the 'Remember Me' box on Inbox and Bundle registration pages, requiring that the user logout and log back in every time they visit the page.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("require_logout_from_bundles_and_inboxes")]
        public bool RequireLogoutFromBundlesAndInboxes
        {
            get { return attributes["require_logout_from_bundles_and_inboxes"] == null ? false : (bool)attributes["require_logout_from_bundles_and_inboxes"]; }
            private set { attributes["require_logout_from_bundles_and_inboxes"] = value; }
        }

        /// <summary>
        /// Current session
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("session")]
        public Session Session
        {
            get { return (Session)attributes["session"]; }
            private set { attributes["session"] = value; }
        }

        /// <summary>
        /// Is SFTP enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_enabled")]
        public bool SftpEnabled
        {
            get { return attributes["sftp_enabled"] == null ? false : (bool)attributes["sftp_enabled"]; }
            private set { attributes["sftp_enabled"] = value; }
        }

        /// <summary>
        /// Sftp Host Key Type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sftp_host_key_type")]
        public string SftpHostKeyType
        {
            get { return (string)attributes["sftp_host_key_type"]; }
            private set { attributes["sftp_host_key_type"] = value; }
        }

        /// <summary>
        /// Id of the currently selected custom SFTP Host Key
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("active_sftp_host_key_id")]
        public Nullable<Int64> ActiveSftpHostKeyId
        {
            get { return (Nullable<Int64>)attributes["active_sftp_host_key_id"]; }
            private set { attributes["active_sftp_host_key_id"] = value; }
        }

        /// <summary>
        /// If true, we will allow weak and known insecure ciphers to be used for SFTP connections.  Enabling this setting severely weakens the security of your site and it is not recommend, except as a last resort for compatibility.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_insecure_ciphers")]
        public bool SftpInsecureCiphers
        {
            get { return attributes["sftp_insecure_ciphers"] == null ? false : (bool)attributes["sftp_insecure_ciphers"]; }
            private set { attributes["sftp_insecure_ciphers"] = value; }
        }

        /// <summary>
        /// If true, we will allow weak Diffie Hellman parameters to be used within ciphers for SFTP that are otherwise on our secure list.  This has the effect of making the cipher weaker than our normal threshold for security, but is required to support certain legacy or broken SSH and MFT clients.  Enabling this weakens security, but not nearly as much as enabling the full `sftp_insecure_ciphers` option.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_insecure_diffie_hellman")]
        public bool SftpInsecureDiffieHellman
        {
            get { return attributes["sftp_insecure_diffie_hellman"] == null ? false : (bool)attributes["sftp_insecure_diffie_hellman"]; }
            private set { attributes["sftp_insecure_diffie_hellman"] = value; }
        }

        /// <summary>
        /// Use user FTP roots also for SFTP?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_user_root_enabled")]
        public bool SftpUserRootEnabled
        {
            get { return attributes["sftp_user_root_enabled"] == null ? false : (bool)attributes["sftp_user_root_enabled"]; }
            private set { attributes["sftp_user_root_enabled"] = value; }
        }

        /// <summary>
        /// Allow bundle creation
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sharing_enabled")]
        public bool SharingEnabled
        {
            get { return attributes["sharing_enabled"] == null ? false : (bool)attributes["sharing_enabled"]; }
            private set { attributes["sharing_enabled"] = value; }
        }

        /// <summary>
        /// Show log in link in user notifications?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("show_user_notifications_log_in_link")]
        public bool ShowUserNotificationsLogInLink
        {
            get { return attributes["show_user_notifications_log_in_link"] == null ? false : (bool)attributes["show_user_notifications_log_in_link"]; }
            private set { attributes["show_user_notifications_log_in_link"] = value; }
        }

        /// <summary>
        /// Show request access link for users without access?  Currently unused.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("show_request_access_link")]
        public bool ShowRequestAccessLink
        {
            get { return attributes["show_request_access_link"] == null ? false : (bool)attributes["show_request_access_link"]; }
            private set { attributes["show_request_access_link"] = value; }
        }

        /// <summary>
        /// Custom site footer text
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_footer")]
        public string SiteFooter
        {
            get { return (string)attributes["site_footer"]; }
            private set { attributes["site_footer"] = value; }
        }

        /// <summary>
        /// Custom site header text
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_header")]
        public string SiteHeader
        {
            get { return (string)attributes["site_header"]; }
            private set { attributes["site_header"] = value; }
        }

        /// <summary>
        /// SMTP server hostname or IP
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smtp_address")]
        public string SmtpAddress
        {
            get { return (string)attributes["smtp_address"]; }
            private set { attributes["smtp_address"] = value; }
        }

        /// <summary>
        /// SMTP server authentication type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smtp_authentication")]
        public string SmtpAuthentication
        {
            get { return (string)attributes["smtp_authentication"]; }
            private set { attributes["smtp_authentication"] = value; }
        }

        /// <summary>
        /// From address to use when mailing through custom SMTP
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smtp_from")]
        public string SmtpFrom
        {
            get { return (string)attributes["smtp_from"]; }
            private set { attributes["smtp_from"] = value; }
        }

        /// <summary>
        /// SMTP server port
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smtp_port")]
        public Nullable<Int64> SmtpPort
        {
            get { return (Nullable<Int64>)attributes["smtp_port"]; }
            private set { attributes["smtp_port"] = value; }
        }

        /// <summary>
        /// SMTP server username
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smtp_username")]
        public string SmtpUsername
        {
            get { return (string)attributes["smtp_username"]; }
            private set { attributes["smtp_username"] = value; }
        }

        /// <summary>
        /// Session expiry in hours
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("session_expiry")]
        public double SessionExpiry
        {
            get { return (double)attributes["session_expiry"]; }
            private set { attributes["session_expiry"] = value; }
        }

        /// <summary>
        /// Session expiry in minutes
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("session_expiry_minutes")]
        public Nullable<Int64> SessionExpiryMinutes
        {
            get { return (Nullable<Int64>)attributes["session_expiry_minutes"]; }
            private set { attributes["session_expiry_minutes"] = value; }
        }

        /// <summary>
        /// Is SSL required?  Disabling this is insecure.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ssl_required")]
        public bool SslRequired
        {
            get { return attributes["ssl_required"] == null ? false : (bool)attributes["ssl_required"]; }
            private set { attributes["ssl_required"] = value; }
        }

        /// <summary>
        /// Site subdomain
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("subdomain")]
        public string Subdomain
        {
            get { return (string)attributes["subdomain"]; }
            private set { attributes["subdomain"] = value; }
        }

        /// <summary>
        /// If switching plans, when does the new plan take effect?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("switch_to_plan_date")]
        public Nullable<DateTime> SwitchToPlanDate
        {
            get { return (Nullable<DateTime>)attributes["switch_to_plan_date"]; }
            private set { attributes["switch_to_plan_date"] = value; }
        }

        /// <summary>
        /// DO NOT ENABLE. This setting allows TLSv1.0 and TLSv1.1 to be used on your site.  We intend to remove this capability entirely in early 2024.  If set, the `sftp_insecure_ciphers` flag will be automatically set to true.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("tls_disabled")]
        public bool TlsDisabled
        {
            get { return attributes["tls_disabled"] == null ? false : (bool)attributes["tls_disabled"]; }
            private set { attributes["tls_disabled"] = value; }
        }

        /// <summary>
        /// Number of days left in trial
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("trial_days_left")]
        public Nullable<Int64> TrialDaysLeft
        {
            get { return (Nullable<Int64>)attributes["trial_days_left"]; }
            private set { attributes["trial_days_left"] = value; }
        }

        /// <summary>
        /// When does this Site trial expire?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("trial_until")]
        public Nullable<DateTime> TrialUntil
        {
            get { return (Nullable<DateTime>)attributes["trial_until"]; }
            private set { attributes["trial_until"] = value; }
        }

        /// <summary>
        /// If using custom SMTP, should we use dedicated IPs to deliver emails?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("use_dedicated_ips_for_smtp")]
        public bool UseDedicatedIpsForSmtp
        {
            get { return attributes["use_dedicated_ips_for_smtp"] == null ? false : (bool)attributes["use_dedicated_ips_for_smtp"]; }
            private set { attributes["use_dedicated_ips_for_smtp"] = value; }
        }

        /// <summary>
        /// Allow uploaders to set `provided_modified_at` for uploaded files?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("use_provided_modified_at")]
        public bool UseProvidedModifiedAt
        {
            get { return attributes["use_provided_modified_at"] == null ? false : (bool)attributes["use_provided_modified_at"]; }
            private set { attributes["use_provided_modified_at"] = value; }
        }

        /// <summary>
        /// User of current session
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user")]
        public User User
        {
            get { return (User)attributes["user"]; }
            private set { attributes["user"] = value; }
        }

        /// <summary>
        /// Will users be locked out after incorrect login attempts?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("user_lockout")]
        public bool UserLockout
        {
            get { return attributes["user_lockout"] == null ? false : (bool)attributes["user_lockout"]; }
            private set { attributes["user_lockout"] = value; }
        }

        /// <summary>
        /// How many hours to lock user out for failed password?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_lockout_lock_period")]
        public Nullable<Int64> UserLockoutLockPeriod
        {
            get { return (Nullable<Int64>)attributes["user_lockout_lock_period"]; }
            private set { attributes["user_lockout_lock_period"] = value; }
        }

        /// <summary>
        /// Number of login tries within `user_lockout_within` hours before users are locked out
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_lockout_tries")]
        public Nullable<Int64> UserLockoutTries
        {
            get { return (Nullable<Int64>)attributes["user_lockout_tries"]; }
            private set { attributes["user_lockout_tries"] = value; }
        }

        /// <summary>
        /// Number of hours for user lockout window
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_lockout_within")]
        public Nullable<Int64> UserLockoutWithin
        {
            get { return (Nullable<Int64>)attributes["user_lockout_within"]; }
            private set { attributes["user_lockout_within"] = value; }
        }

        /// <summary>
        /// Enable User Requests feature
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("user_requests_enabled")]
        public bool UserRequestsEnabled
        {
            get { return attributes["user_requests_enabled"] == null ? false : (bool)attributes["user_requests_enabled"]; }
            private set { attributes["user_requests_enabled"] = value; }
        }

        /// <summary>
        /// Send email to site admins when a user request is received?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("user_requests_notify_admins")]
        public bool UserRequestsNotifyAdmins
        {
            get { return attributes["user_requests_notify_admins"] == null ? false : (bool)attributes["user_requests_notify_admins"]; }
            private set { attributes["user_requests_notify_admins"] = value; }
        }

        /// <summary>
        /// Allow users to create their own API keys?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("users_can_create_api_keys")]
        public bool UsersCanCreateApiKeys
        {
            get { return attributes["users_can_create_api_keys"] == null ? false : (bool)attributes["users_can_create_api_keys"]; }
            private set { attributes["users_can_create_api_keys"] = value; }
        }

        /// <summary>
        /// Allow users to create their own SSH keys?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("users_can_create_ssh_keys")]
        public bool UsersCanCreateSshKeys
        {
            get { return attributes["users_can_create_ssh_keys"] == null ? false : (bool)attributes["users_can_create_ssh_keys"]; }
            private set { attributes["users_can_create_ssh_keys"] = value; }
        }

        /// <summary>
        /// Custom text send in user welcome email
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("welcome_custom_text")]
        public string WelcomeCustomText
        {
            get { return (string)attributes["welcome_custom_text"]; }
            private set { attributes["welcome_custom_text"] = value; }
        }

        /// <summary>
        /// Include this email in welcome emails if enabled
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("welcome_email_cc")]
        public string WelcomeEmailCc
        {
            get { return (string)attributes["welcome_email_cc"]; }
            private set { attributes["welcome_email_cc"] = value; }
        }

        /// <summary>
        /// Include this email subject in welcome emails if enabled
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("welcome_email_subject")]
        public string WelcomeEmailSubject
        {
            get { return (string)attributes["welcome_email_subject"]; }
            private set { attributes["welcome_email_subject"] = value; }
        }

        /// <summary>
        /// Will the welcome email be sent to new users?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("welcome_email_enabled")]
        public bool WelcomeEmailEnabled
        {
            get { return attributes["welcome_email_enabled"] == null ? false : (bool)attributes["welcome_email_enabled"]; }
            private set { attributes["welcome_email_enabled"] = value; }
        }

        /// <summary>
        /// Does the welcome screen appear?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("welcome_screen")]
        public string WelcomeScreen
        {
            get { return (string)attributes["welcome_screen"]; }
            private set { attributes["welcome_screen"] = value; }
        }

        /// <summary>
        /// Does FTP user Windows emulation mode?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("windows_mode_ftp")]
        public bool WindowsModeFtp
        {
            get { return attributes["windows_mode_ftp"] == null ? false : (bool)attributes["windows_mode_ftp"]; }
            private set { attributes["windows_mode_ftp"] = value; }
        }

        /// <summary>
        /// If greater than zero, users will unable to login if they do not show activity within this number of days.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("disable_users_from_inactivity_period_days")]
        public Nullable<Int64> DisableUsersFromInactivityPeriodDays
        {
            get { return (Nullable<Int64>)attributes["disable_users_from_inactivity_period_days"]; }
            private set { attributes["disable_users_from_inactivity_period_days"] = value; }
        }

        /// <summary>
        /// Allow group admins set password authentication method
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("group_admins_can_set_user_password")]
        public bool GroupAdminsCanSetUserPassword
        {
            get { return attributes["group_admins_can_set_user_password"] == null ? false : (bool)attributes["group_admins_can_set_user_password"]; }
            private set { attributes["group_admins_can_set_user_password"] = value; }
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


            string responseJson = await FilesClient.SendStringRequest($"/site", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Site>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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


            string responseJson = await FilesClient.SendStringRequest($"/site/usage", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<UsageSnapshot>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Site name
        ///   subdomain - string - Site subdomain
        ///   domain - string - Custom domain
        ///   domain_hsts_header - boolean - Send HSTS (HTTP Strict Transport Security) header when visitors access the site via a custom domain?
        ///   domain_letsencrypt_chain - string - Letsencrypt chain to use when registering SSL Certificate for domain.
        ///   email - string - Main email for this site
        ///   reply_to_email - string - Reply-to email for this site
        ///   allow_bundle_names - boolean - Are manual Bundle names allowed?
        ///   bundle_expiration - int64 - Site-wide Bundle expiration in days
        ///   welcome_email_enabled - boolean - Will the welcome email be sent to new users?
        ///   ask_about_overwrites - boolean - If false, rename conflicting files instead of asking for overwrite confirmation.  Only applies to web interface.
        ///   show_request_access_link - boolean - Show request access link for users without access?  Currently unused.
        ///   always_mkdir_parents - boolean - Create parent directories if they do not exist during uploads?  This is primarily used to work around broken upload clients that assume servers will perform this step.
        ///   welcome_email_cc - string - Include this email in welcome emails if enabled
        ///   welcome_email_subject - string - Include this email subject in welcome emails if enabled
        ///   welcome_custom_text - string - Custom text send in user welcome email
        ///   language - string - Site default language
        ///   windows_mode_ftp - boolean - Does FTP user Windows emulation mode?
        ///   default_time_zone - string - Site default time zone
        ///   desktop_app - boolean - Is the desktop app enabled?
        ///   desktop_app_session_ip_pinning - boolean - Is desktop app session IP pinning enabled?
        ///   desktop_app_session_lifetime - int64 - Desktop app session lifetime (in hours)
        ///   mobile_app - boolean - Is the mobile app enabled?
        ///   mobile_app_session_ip_pinning - boolean - Is mobile app session IP pinning enabled?
        ///   mobile_app_session_lifetime - int64 - Mobile app session lifetime (in hours)
        ///   folder_permissions_groups_only - boolean - If true, permissions for this site must be bound to a group (not a user). Otherwise, permissions must be bound to a user.
        ///   welcome_screen - string - Does the welcome screen appear?
        ///   office_integration_available - boolean - If true, allows users to use a document editing integration.
        ///   office_integration_type - string - Which document editing integration to support. Files.com Editor or Microsoft Office for the Web.
        ///   pin_all_remote_servers_to_site_region - boolean - If true, we will ensure that all internal communications with any remote server are made through the primary region of the site. This setting overrides individual remote server settings.
        ///   motd_text - string - A message to show users when they connect via FTP or SFTP.
        ///   motd_use_for_ftp - boolean - Show message to users connecting via FTP
        ///   motd_use_for_sftp - boolean - Show message to users connecting via SFTP
        ///   left_navigation_visibility - object - Visibility settings for account navigation
        ///   additional_text_file_types - array(string) - Additional extensions that are considered text files
        ///   bundle_require_note - boolean - Do Bundles require internal notes?
        ///   bundle_send_shared_receipts - boolean - Do Bundle creators receive receipts of invitations?
        ///   calculate_file_checksums_crc32 - boolean - Calculate CRC32 checksums for files?
        ///   calculate_file_checksums_md5 - boolean - Calculate MD5 checksums for files?
        ///   calculate_file_checksums_sha1 - boolean - Calculate SHA1 checksums for files?
        ///   calculate_file_checksums_sha256 - boolean - Calculate SHA256 checksums for files?
        ///   legacy_checksums_mode - boolean - Use legacy checksums mode?
        ///   session_expiry - double - Session expiry in hours
        ///   ssl_required - boolean - Is SSL required?  Disabling this is insecure.
        ///   tls_disabled - boolean - DO NOT ENABLE. This setting allows TLSv1.0 and TLSv1.1 to be used on your site.  We intend to remove this capability entirely in early 2024.  If set, the `sftp_insecure_ciphers` flag will be automatically set to true.
        ///   sftp_insecure_ciphers - boolean - If true, we will allow weak and known insecure ciphers to be used for SFTP connections.  Enabling this setting severely weakens the security of your site and it is not recommend, except as a last resort for compatibility.
        ///   sftp_insecure_diffie_hellman - boolean - If true, we will allow weak Diffie Hellman parameters to be used within ciphers for SFTP that are otherwise on our secure list.  This has the effect of making the cipher weaker than our normal threshold for security, but is required to support certain legacy or broken SSH and MFT clients.  Enabling this weakens security, but not nearly as much as enabling the full `sftp_insecure_ciphers` option.
        ///   disable_files_certificate_generation - boolean - If set, Files.com will not set the CAA records required to generate future SSL certificates for this domain.
        ///   user_lockout - boolean - Will users be locked out after incorrect login attempts?
        ///   user_lockout_tries - int64 - Number of login tries within `user_lockout_within` hours before users are locked out
        ///   user_lockout_within - int64 - Number of hours for user lockout window
        ///   user_lockout_lock_period - int64 - How many hours to lock user out for failed password?
        ///   include_password_in_welcome_email - boolean - Include password in emails to new users?
        ///   allowed_countries - string - Comma separated list of allowed Country codes
        ///   allowed_ips - string - List of allowed IP addresses
        ///   disallowed_countries - string - Comma separated list of disallowed Country codes
        ///   days_before_deleting_disabled_users - int64 - Number of days to keep disabled users before deleting them. If set to 0, disabled users will not be deleted.
        ///   days_to_retain_backups - int64 - Number of days to keep deleted files
        ///   max_prior_passwords - int64 - Number of prior passwords to disallow
        ///   password_validity_days - int64 - Number of days password is valid
        ///   password_min_length - int64 - Shortest password length for users
        ///   password_require_letter - boolean - Require a letter in passwords?
        ///   password_require_mixed - boolean - Require lower and upper case letters in passwords?
        ///   password_require_special - boolean - Require special characters in password?
        ///   password_require_number - boolean - Require a number in passwords?
        ///   password_require_unbreached - boolean - Require passwords that have not been previously breached? (see https://haveibeenpwned.com/)
        ///   require_logout_from_bundles_and_inboxes - boolean - If true, we will hide the 'Remember Me' box on Inbox and Bundle registration pages, requiring that the user logout and log back in every time they visit the page.
        ///   dav_user_root_enabled - boolean - Use user FTP roots also for WebDAV?
        ///   sftp_user_root_enabled - boolean - Use user FTP roots also for SFTP?
        ///   disable_password_reset - boolean - Is password reset disabled?
        ///   immutable_files - boolean - Are files protected from modification?
        ///   bundle_not_found_message - string - Custom error message to show when bundle is not found.
        ///   bundle_password_required - boolean - Do Bundles require password protection?
        ///   bundle_require_registration - boolean - Do Bundles require registration?
        ///   bundle_require_share_recipient - boolean - Do Bundles require recipients for sharing?
        ///   bundle_registration_notifications - string - Do Bundle owners receive registration notification?
        ///   bundle_activity_notifications - string - Do Bundle owners receive activity notifications?
        ///   bundle_upload_receipt_notifications - string - Do Bundle uploaders receive upload confirmation notifications?
        ///   document_edits_in_bundle_allowed - boolean - If true, allow public viewers of Bundles with full permissions to use document editing integrations.
        ///   password_requirements_apply_to_bundles - boolean - Require bundles' passwords, and passwords for other items (inboxes, public shares, etc.) to conform to the same requirements as users' passwords?
        ///   prevent_root_permissions_for_non_site_admins - boolean - If true, we will prevent non-administrators from receiving any permissions directly on the root folder.  This is commonly used to prevent the accidental application of permissions.
        ///   opt_out_global - boolean - Use servers in the USA only?
        ///   use_provided_modified_at - boolean - Allow uploaders to set `provided_modified_at` for uploaded files?
        ///   custom_namespace - boolean - Is this site using a custom namespace for users?
        ///   disable_users_from_inactivity_period_days - int64 - If greater than zero, users will unable to login if they do not show activity within this number of days.
        ///   non_sso_groups_allowed - boolean - If true, groups can be manually created / modified / deleted by Site Admins. Otherwise, groups can only be managed via your SSO provider.
        ///   non_sso_users_allowed - boolean - If true, users can be manually created / modified / deleted by Site Admins. Otherwise, users can only be managed via your SSO provider.
        ///   sharing_enabled - boolean - Allow bundle creation
        ///   user_requests_enabled - boolean - Enable User Requests feature
        ///   user_requests_notify_admins - boolean - Send email to site admins when a user request is received?
        ///   dav_enabled - boolean - Is WebDAV enabled?
        ///   ftp_enabled - boolean - Is FTP enabled?
        ///   sftp_enabled - boolean - Is SFTP enabled?
        ///   users_can_create_api_keys - boolean - Allow users to create their own API keys?
        ///   users_can_create_ssh_keys - boolean - Allow users to create their own SSH keys?
        ///   show_user_notifications_log_in_link - boolean - Show log in link in user notifications?
        ///   sftp_host_key_type - string - Sftp Host Key Type
        ///   active_sftp_host_key_id - int64 - Id of the currently selected custom SFTP Host Key
        ///   protocol_access_groups_only - boolean - If true, protocol access permissions on users will be ignored, and only protocol access permissions set on Groups will be honored.  Make sure that your current user is a member of a group with API permission when changing this value to avoid locking yourself out of your site.
        ///   bundle_watermark_value - object - Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
        ///   group_admins_can_set_user_password - boolean - Allow group admins set password authentication method
        ///   bundle_recipient_blacklist_free_email_domains - boolean - Disallow free email domains for Bundle/Inbox recipients?
        ///   bundle_recipient_blacklist_domains - array(string) - List of email domains to disallow when entering a Bundle/Inbox recipients
        ///   admins_bypass_locked_subfolders - boolean - Allow admins to bypass the locked subfolders setting.
        ///   allowed_2fa_method_sms - boolean - Is SMS two factor authentication allowed?
        ///   allowed_2fa_method_totp - boolean - Is TOTP two factor authentication allowed?
        ///   allowed_2fa_method_webauthn - boolean - Is WebAuthn two factor authentication allowed?
        ///   allowed_2fa_method_yubi - boolean - Is yubikey two factor authentication allowed?
        ///   allowed_2fa_method_email - boolean - Is OTP via email two factor authentication allowed?
        ///   allowed_2fa_method_static - boolean - Is OTP via static codes for two factor authentication allowed?
        ///   allowed_2fa_method_bypass_for_ftp_sftp_dav - boolean - Are users allowed to configure their two factor authentication to be bypassed for FTP/SFTP/WebDAV?
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
        ///   use_dedicated_ips_for_smtp - boolean - If using custom SMTP, should we use dedicated IPs to deliver emails?
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
        ///   uploads_via_email_authentication - boolean - Do incoming emails in the Inboxes require checking for SPF/DKIM/DMARC?
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
        ///   bundle_watermark_attachment_file - file
        ///   bundle_watermark_attachment_delete - boolean - If true, will delete the file stored in bundle_watermark_attachment
        ///   login_page_background_image_file - file
        ///   login_page_background_image_delete - boolean - If true, will delete the file stored in login_page_background_image
        ///   disable_2fa_with_delay - boolean - If set to true, we will begin the process of disabling 2FA on this site.
        ///   ldap_password_change - string - New LDAP password.
        ///   ldap_password_change_confirmation - string - Confirm new LDAP password.
        ///   smtp_password - string - Password for SMTP server.
        ///   session_expiry_minutes - int64 - Session expiry in minutes
        /// </summary>
        public static async Task<Site> Update(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("subdomain") && !(parameters["subdomain"] is string))
            {
                throw new ArgumentException("Bad parameter: subdomain must be of type string", "parameters[\"subdomain\"]");
            }
            if (parameters.ContainsKey("domain") && !(parameters["domain"] is string))
            {
                throw new ArgumentException("Bad parameter: domain must be of type string", "parameters[\"domain\"]");
            }
            if (parameters.ContainsKey("domain_hsts_header") && !(parameters["domain_hsts_header"] is bool))
            {
                throw new ArgumentException("Bad parameter: domain_hsts_header must be of type bool", "parameters[\"domain_hsts_header\"]");
            }
            if (parameters.ContainsKey("domain_letsencrypt_chain") && !(parameters["domain_letsencrypt_chain"] is string))
            {
                throw new ArgumentException("Bad parameter: domain_letsencrypt_chain must be of type string", "parameters[\"domain_letsencrypt_chain\"]");
            }
            if (parameters.ContainsKey("email") && !(parameters["email"] is string))
            {
                throw new ArgumentException("Bad parameter: email must be of type string", "parameters[\"email\"]");
            }
            if (parameters.ContainsKey("reply_to_email") && !(parameters["reply_to_email"] is string))
            {
                throw new ArgumentException("Bad parameter: reply_to_email must be of type string", "parameters[\"reply_to_email\"]");
            }
            if (parameters.ContainsKey("allow_bundle_names") && !(parameters["allow_bundle_names"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_bundle_names must be of type bool", "parameters[\"allow_bundle_names\"]");
            }
            if (parameters.ContainsKey("bundle_expiration") && !(parameters["bundle_expiration"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: bundle_expiration must be of type Nullable<Int64>", "parameters[\"bundle_expiration\"]");
            }
            if (parameters.ContainsKey("welcome_email_enabled") && !(parameters["welcome_email_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: welcome_email_enabled must be of type bool", "parameters[\"welcome_email_enabled\"]");
            }
            if (parameters.ContainsKey("ask_about_overwrites") && !(parameters["ask_about_overwrites"] is bool))
            {
                throw new ArgumentException("Bad parameter: ask_about_overwrites must be of type bool", "parameters[\"ask_about_overwrites\"]");
            }
            if (parameters.ContainsKey("show_request_access_link") && !(parameters["show_request_access_link"] is bool))
            {
                throw new ArgumentException("Bad parameter: show_request_access_link must be of type bool", "parameters[\"show_request_access_link\"]");
            }
            if (parameters.ContainsKey("always_mkdir_parents") && !(parameters["always_mkdir_parents"] is bool))
            {
                throw new ArgumentException("Bad parameter: always_mkdir_parents must be of type bool", "parameters[\"always_mkdir_parents\"]");
            }
            if (parameters.ContainsKey("welcome_email_cc") && !(parameters["welcome_email_cc"] is string))
            {
                throw new ArgumentException("Bad parameter: welcome_email_cc must be of type string", "parameters[\"welcome_email_cc\"]");
            }
            if (parameters.ContainsKey("welcome_email_subject") && !(parameters["welcome_email_subject"] is string))
            {
                throw new ArgumentException("Bad parameter: welcome_email_subject must be of type string", "parameters[\"welcome_email_subject\"]");
            }
            if (parameters.ContainsKey("welcome_custom_text") && !(parameters["welcome_custom_text"] is string))
            {
                throw new ArgumentException("Bad parameter: welcome_custom_text must be of type string", "parameters[\"welcome_custom_text\"]");
            }
            if (parameters.ContainsKey("language") && !(parameters["language"] is string))
            {
                throw new ArgumentException("Bad parameter: language must be of type string", "parameters[\"language\"]");
            }
            if (parameters.ContainsKey("windows_mode_ftp") && !(parameters["windows_mode_ftp"] is bool))
            {
                throw new ArgumentException("Bad parameter: windows_mode_ftp must be of type bool", "parameters[\"windows_mode_ftp\"]");
            }
            if (parameters.ContainsKey("default_time_zone") && !(parameters["default_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: default_time_zone must be of type string", "parameters[\"default_time_zone\"]");
            }
            if (parameters.ContainsKey("desktop_app") && !(parameters["desktop_app"] is bool))
            {
                throw new ArgumentException("Bad parameter: desktop_app must be of type bool", "parameters[\"desktop_app\"]");
            }
            if (parameters.ContainsKey("desktop_app_session_ip_pinning") && !(parameters["desktop_app_session_ip_pinning"] is bool))
            {
                throw new ArgumentException("Bad parameter: desktop_app_session_ip_pinning must be of type bool", "parameters[\"desktop_app_session_ip_pinning\"]");
            }
            if (parameters.ContainsKey("desktop_app_session_lifetime") && !(parameters["desktop_app_session_lifetime"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: desktop_app_session_lifetime must be of type Nullable<Int64>", "parameters[\"desktop_app_session_lifetime\"]");
            }
            if (parameters.ContainsKey("mobile_app") && !(parameters["mobile_app"] is bool))
            {
                throw new ArgumentException("Bad parameter: mobile_app must be of type bool", "parameters[\"mobile_app\"]");
            }
            if (parameters.ContainsKey("mobile_app_session_ip_pinning") && !(parameters["mobile_app_session_ip_pinning"] is bool))
            {
                throw new ArgumentException("Bad parameter: mobile_app_session_ip_pinning must be of type bool", "parameters[\"mobile_app_session_ip_pinning\"]");
            }
            if (parameters.ContainsKey("mobile_app_session_lifetime") && !(parameters["mobile_app_session_lifetime"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: mobile_app_session_lifetime must be of type Nullable<Int64>", "parameters[\"mobile_app_session_lifetime\"]");
            }
            if (parameters.ContainsKey("folder_permissions_groups_only") && !(parameters["folder_permissions_groups_only"] is bool))
            {
                throw new ArgumentException("Bad parameter: folder_permissions_groups_only must be of type bool", "parameters[\"folder_permissions_groups_only\"]");
            }
            if (parameters.ContainsKey("welcome_screen") && !(parameters["welcome_screen"] is string))
            {
                throw new ArgumentException("Bad parameter: welcome_screen must be of type string", "parameters[\"welcome_screen\"]");
            }
            if (parameters.ContainsKey("office_integration_available") && !(parameters["office_integration_available"] is bool))
            {
                throw new ArgumentException("Bad parameter: office_integration_available must be of type bool", "parameters[\"office_integration_available\"]");
            }
            if (parameters.ContainsKey("office_integration_type") && !(parameters["office_integration_type"] is string))
            {
                throw new ArgumentException("Bad parameter: office_integration_type must be of type string", "parameters[\"office_integration_type\"]");
            }
            if (parameters.ContainsKey("pin_all_remote_servers_to_site_region") && !(parameters["pin_all_remote_servers_to_site_region"] is bool))
            {
                throw new ArgumentException("Bad parameter: pin_all_remote_servers_to_site_region must be of type bool", "parameters[\"pin_all_remote_servers_to_site_region\"]");
            }
            if (parameters.ContainsKey("motd_text") && !(parameters["motd_text"] is string))
            {
                throw new ArgumentException("Bad parameter: motd_text must be of type string", "parameters[\"motd_text\"]");
            }
            if (parameters.ContainsKey("motd_use_for_ftp") && !(parameters["motd_use_for_ftp"] is bool))
            {
                throw new ArgumentException("Bad parameter: motd_use_for_ftp must be of type bool", "parameters[\"motd_use_for_ftp\"]");
            }
            if (parameters.ContainsKey("motd_use_for_sftp") && !(parameters["motd_use_for_sftp"] is bool))
            {
                throw new ArgumentException("Bad parameter: motd_use_for_sftp must be of type bool", "parameters[\"motd_use_for_sftp\"]");
            }
            if (parameters.ContainsKey("left_navigation_visibility") && !(parameters["left_navigation_visibility"] is object))
            {
                throw new ArgumentException("Bad parameter: left_navigation_visibility must be of type object", "parameters[\"left_navigation_visibility\"]");
            }
            if (parameters.ContainsKey("additional_text_file_types") && !(parameters["additional_text_file_types"] is string[]))
            {
                throw new ArgumentException("Bad parameter: additional_text_file_types must be of type string[]", "parameters[\"additional_text_file_types\"]");
            }
            if (parameters.ContainsKey("bundle_require_note") && !(parameters["bundle_require_note"] is bool))
            {
                throw new ArgumentException("Bad parameter: bundle_require_note must be of type bool", "parameters[\"bundle_require_note\"]");
            }
            if (parameters.ContainsKey("bundle_send_shared_receipts") && !(parameters["bundle_send_shared_receipts"] is bool))
            {
                throw new ArgumentException("Bad parameter: bundle_send_shared_receipts must be of type bool", "parameters[\"bundle_send_shared_receipts\"]");
            }
            if (parameters.ContainsKey("calculate_file_checksums_crc32") && !(parameters["calculate_file_checksums_crc32"] is bool))
            {
                throw new ArgumentException("Bad parameter: calculate_file_checksums_crc32 must be of type bool", "parameters[\"calculate_file_checksums_crc32\"]");
            }
            if (parameters.ContainsKey("calculate_file_checksums_md5") && !(parameters["calculate_file_checksums_md5"] is bool))
            {
                throw new ArgumentException("Bad parameter: calculate_file_checksums_md5 must be of type bool", "parameters[\"calculate_file_checksums_md5\"]");
            }
            if (parameters.ContainsKey("calculate_file_checksums_sha1") && !(parameters["calculate_file_checksums_sha1"] is bool))
            {
                throw new ArgumentException("Bad parameter: calculate_file_checksums_sha1 must be of type bool", "parameters[\"calculate_file_checksums_sha1\"]");
            }
            if (parameters.ContainsKey("calculate_file_checksums_sha256") && !(parameters["calculate_file_checksums_sha256"] is bool))
            {
                throw new ArgumentException("Bad parameter: calculate_file_checksums_sha256 must be of type bool", "parameters[\"calculate_file_checksums_sha256\"]");
            }
            if (parameters.ContainsKey("legacy_checksums_mode") && !(parameters["legacy_checksums_mode"] is bool))
            {
                throw new ArgumentException("Bad parameter: legacy_checksums_mode must be of type bool", "parameters[\"legacy_checksums_mode\"]");
            }
            if (parameters.ContainsKey("session_expiry") && !(parameters["session_expiry"] is double))
            {
                throw new ArgumentException("Bad parameter: session_expiry must be of type double", "parameters[\"session_expiry\"]");
            }
            if (parameters.ContainsKey("ssl_required") && !(parameters["ssl_required"] is bool))
            {
                throw new ArgumentException("Bad parameter: ssl_required must be of type bool", "parameters[\"ssl_required\"]");
            }
            if (parameters.ContainsKey("tls_disabled") && !(parameters["tls_disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: tls_disabled must be of type bool", "parameters[\"tls_disabled\"]");
            }
            if (parameters.ContainsKey("sftp_insecure_ciphers") && !(parameters["sftp_insecure_ciphers"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_insecure_ciphers must be of type bool", "parameters[\"sftp_insecure_ciphers\"]");
            }
            if (parameters.ContainsKey("sftp_insecure_diffie_hellman") && !(parameters["sftp_insecure_diffie_hellman"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_insecure_diffie_hellman must be of type bool", "parameters[\"sftp_insecure_diffie_hellman\"]");
            }
            if (parameters.ContainsKey("disable_files_certificate_generation") && !(parameters["disable_files_certificate_generation"] is bool))
            {
                throw new ArgumentException("Bad parameter: disable_files_certificate_generation must be of type bool", "parameters[\"disable_files_certificate_generation\"]");
            }
            if (parameters.ContainsKey("user_lockout") && !(parameters["user_lockout"] is bool))
            {
                throw new ArgumentException("Bad parameter: user_lockout must be of type bool", "parameters[\"user_lockout\"]");
            }
            if (parameters.ContainsKey("user_lockout_tries") && !(parameters["user_lockout_tries"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_lockout_tries must be of type Nullable<Int64>", "parameters[\"user_lockout_tries\"]");
            }
            if (parameters.ContainsKey("user_lockout_within") && !(parameters["user_lockout_within"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_lockout_within must be of type Nullable<Int64>", "parameters[\"user_lockout_within\"]");
            }
            if (parameters.ContainsKey("user_lockout_lock_period") && !(parameters["user_lockout_lock_period"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_lockout_lock_period must be of type Nullable<Int64>", "parameters[\"user_lockout_lock_period\"]");
            }
            if (parameters.ContainsKey("include_password_in_welcome_email") && !(parameters["include_password_in_welcome_email"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_password_in_welcome_email must be of type bool", "parameters[\"include_password_in_welcome_email\"]");
            }
            if (parameters.ContainsKey("allowed_countries") && !(parameters["allowed_countries"] is string))
            {
                throw new ArgumentException("Bad parameter: allowed_countries must be of type string", "parameters[\"allowed_countries\"]");
            }
            if (parameters.ContainsKey("allowed_ips") && !(parameters["allowed_ips"] is string))
            {
                throw new ArgumentException("Bad parameter: allowed_ips must be of type string", "parameters[\"allowed_ips\"]");
            }
            if (parameters.ContainsKey("disallowed_countries") && !(parameters["disallowed_countries"] is string))
            {
                throw new ArgumentException("Bad parameter: disallowed_countries must be of type string", "parameters[\"disallowed_countries\"]");
            }
            if (parameters.ContainsKey("days_before_deleting_disabled_users") && !(parameters["days_before_deleting_disabled_users"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: days_before_deleting_disabled_users must be of type Nullable<Int64>", "parameters[\"days_before_deleting_disabled_users\"]");
            }
            if (parameters.ContainsKey("days_to_retain_backups") && !(parameters["days_to_retain_backups"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: days_to_retain_backups must be of type Nullable<Int64>", "parameters[\"days_to_retain_backups\"]");
            }
            if (parameters.ContainsKey("max_prior_passwords") && !(parameters["max_prior_passwords"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_prior_passwords must be of type Nullable<Int64>", "parameters[\"max_prior_passwords\"]");
            }
            if (parameters.ContainsKey("password_validity_days") && !(parameters["password_validity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: password_validity_days must be of type Nullable<Int64>", "parameters[\"password_validity_days\"]");
            }
            if (parameters.ContainsKey("password_min_length") && !(parameters["password_min_length"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: password_min_length must be of type Nullable<Int64>", "parameters[\"password_min_length\"]");
            }
            if (parameters.ContainsKey("password_require_letter") && !(parameters["password_require_letter"] is bool))
            {
                throw new ArgumentException("Bad parameter: password_require_letter must be of type bool", "parameters[\"password_require_letter\"]");
            }
            if (parameters.ContainsKey("password_require_mixed") && !(parameters["password_require_mixed"] is bool))
            {
                throw new ArgumentException("Bad parameter: password_require_mixed must be of type bool", "parameters[\"password_require_mixed\"]");
            }
            if (parameters.ContainsKey("password_require_special") && !(parameters["password_require_special"] is bool))
            {
                throw new ArgumentException("Bad parameter: password_require_special must be of type bool", "parameters[\"password_require_special\"]");
            }
            if (parameters.ContainsKey("password_require_number") && !(parameters["password_require_number"] is bool))
            {
                throw new ArgumentException("Bad parameter: password_require_number must be of type bool", "parameters[\"password_require_number\"]");
            }
            if (parameters.ContainsKey("password_require_unbreached") && !(parameters["password_require_unbreached"] is bool))
            {
                throw new ArgumentException("Bad parameter: password_require_unbreached must be of type bool", "parameters[\"password_require_unbreached\"]");
            }
            if (parameters.ContainsKey("require_logout_from_bundles_and_inboxes") && !(parameters["require_logout_from_bundles_and_inboxes"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_logout_from_bundles_and_inboxes must be of type bool", "parameters[\"require_logout_from_bundles_and_inboxes\"]");
            }
            if (parameters.ContainsKey("dav_user_root_enabled") && !(parameters["dav_user_root_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: dav_user_root_enabled must be of type bool", "parameters[\"dav_user_root_enabled\"]");
            }
            if (parameters.ContainsKey("sftp_user_root_enabled") && !(parameters["sftp_user_root_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_user_root_enabled must be of type bool", "parameters[\"sftp_user_root_enabled\"]");
            }
            if (parameters.ContainsKey("disable_password_reset") && !(parameters["disable_password_reset"] is bool))
            {
                throw new ArgumentException("Bad parameter: disable_password_reset must be of type bool", "parameters[\"disable_password_reset\"]");
            }
            if (parameters.ContainsKey("immutable_files") && !(parameters["immutable_files"] is bool))
            {
                throw new ArgumentException("Bad parameter: immutable_files must be of type bool", "parameters[\"immutable_files\"]");
            }
            if (parameters.ContainsKey("bundle_not_found_message") && !(parameters["bundle_not_found_message"] is string))
            {
                throw new ArgumentException("Bad parameter: bundle_not_found_message must be of type string", "parameters[\"bundle_not_found_message\"]");
            }
            if (parameters.ContainsKey("bundle_password_required") && !(parameters["bundle_password_required"] is bool))
            {
                throw new ArgumentException("Bad parameter: bundle_password_required must be of type bool", "parameters[\"bundle_password_required\"]");
            }
            if (parameters.ContainsKey("bundle_require_registration") && !(parameters["bundle_require_registration"] is bool))
            {
                throw new ArgumentException("Bad parameter: bundle_require_registration must be of type bool", "parameters[\"bundle_require_registration\"]");
            }
            if (parameters.ContainsKey("bundle_require_share_recipient") && !(parameters["bundle_require_share_recipient"] is bool))
            {
                throw new ArgumentException("Bad parameter: bundle_require_share_recipient must be of type bool", "parameters[\"bundle_require_share_recipient\"]");
            }
            if (parameters.ContainsKey("bundle_registration_notifications") && !(parameters["bundle_registration_notifications"] is string))
            {
                throw new ArgumentException("Bad parameter: bundle_registration_notifications must be of type string", "parameters[\"bundle_registration_notifications\"]");
            }
            if (parameters.ContainsKey("bundle_activity_notifications") && !(parameters["bundle_activity_notifications"] is string))
            {
                throw new ArgumentException("Bad parameter: bundle_activity_notifications must be of type string", "parameters[\"bundle_activity_notifications\"]");
            }
            if (parameters.ContainsKey("bundle_upload_receipt_notifications") && !(parameters["bundle_upload_receipt_notifications"] is string))
            {
                throw new ArgumentException("Bad parameter: bundle_upload_receipt_notifications must be of type string", "parameters[\"bundle_upload_receipt_notifications\"]");
            }
            if (parameters.ContainsKey("document_edits_in_bundle_allowed") && !(parameters["document_edits_in_bundle_allowed"] is bool))
            {
                throw new ArgumentException("Bad parameter: document_edits_in_bundle_allowed must be of type bool", "parameters[\"document_edits_in_bundle_allowed\"]");
            }
            if (parameters.ContainsKey("password_requirements_apply_to_bundles") && !(parameters["password_requirements_apply_to_bundles"] is bool))
            {
                throw new ArgumentException("Bad parameter: password_requirements_apply_to_bundles must be of type bool", "parameters[\"password_requirements_apply_to_bundles\"]");
            }
            if (parameters.ContainsKey("prevent_root_permissions_for_non_site_admins") && !(parameters["prevent_root_permissions_for_non_site_admins"] is bool))
            {
                throw new ArgumentException("Bad parameter: prevent_root_permissions_for_non_site_admins must be of type bool", "parameters[\"prevent_root_permissions_for_non_site_admins\"]");
            }
            if (parameters.ContainsKey("opt_out_global") && !(parameters["opt_out_global"] is bool))
            {
                throw new ArgumentException("Bad parameter: opt_out_global must be of type bool", "parameters[\"opt_out_global\"]");
            }
            if (parameters.ContainsKey("use_provided_modified_at") && !(parameters["use_provided_modified_at"] is bool))
            {
                throw new ArgumentException("Bad parameter: use_provided_modified_at must be of type bool", "parameters[\"use_provided_modified_at\"]");
            }
            if (parameters.ContainsKey("custom_namespace") && !(parameters["custom_namespace"] is bool))
            {
                throw new ArgumentException("Bad parameter: custom_namespace must be of type bool", "parameters[\"custom_namespace\"]");
            }
            if (parameters.ContainsKey("disable_users_from_inactivity_period_days") && !(parameters["disable_users_from_inactivity_period_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: disable_users_from_inactivity_period_days must be of type Nullable<Int64>", "parameters[\"disable_users_from_inactivity_period_days\"]");
            }
            if (parameters.ContainsKey("non_sso_groups_allowed") && !(parameters["non_sso_groups_allowed"] is bool))
            {
                throw new ArgumentException("Bad parameter: non_sso_groups_allowed must be of type bool", "parameters[\"non_sso_groups_allowed\"]");
            }
            if (parameters.ContainsKey("non_sso_users_allowed") && !(parameters["non_sso_users_allowed"] is bool))
            {
                throw new ArgumentException("Bad parameter: non_sso_users_allowed must be of type bool", "parameters[\"non_sso_users_allowed\"]");
            }
            if (parameters.ContainsKey("sharing_enabled") && !(parameters["sharing_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sharing_enabled must be of type bool", "parameters[\"sharing_enabled\"]");
            }
            if (parameters.ContainsKey("user_requests_enabled") && !(parameters["user_requests_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: user_requests_enabled must be of type bool", "parameters[\"user_requests_enabled\"]");
            }
            if (parameters.ContainsKey("user_requests_notify_admins") && !(parameters["user_requests_notify_admins"] is bool))
            {
                throw new ArgumentException("Bad parameter: user_requests_notify_admins must be of type bool", "parameters[\"user_requests_notify_admins\"]");
            }
            if (parameters.ContainsKey("dav_enabled") && !(parameters["dav_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: dav_enabled must be of type bool", "parameters[\"dav_enabled\"]");
            }
            if (parameters.ContainsKey("ftp_enabled") && !(parameters["ftp_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_enabled must be of type bool", "parameters[\"ftp_enabled\"]");
            }
            if (parameters.ContainsKey("sftp_enabled") && !(parameters["sftp_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_enabled must be of type bool", "parameters[\"sftp_enabled\"]");
            }
            if (parameters.ContainsKey("users_can_create_api_keys") && !(parameters["users_can_create_api_keys"] is bool))
            {
                throw new ArgumentException("Bad parameter: users_can_create_api_keys must be of type bool", "parameters[\"users_can_create_api_keys\"]");
            }
            if (parameters.ContainsKey("users_can_create_ssh_keys") && !(parameters["users_can_create_ssh_keys"] is bool))
            {
                throw new ArgumentException("Bad parameter: users_can_create_ssh_keys must be of type bool", "parameters[\"users_can_create_ssh_keys\"]");
            }
            if (parameters.ContainsKey("show_user_notifications_log_in_link") && !(parameters["show_user_notifications_log_in_link"] is bool))
            {
                throw new ArgumentException("Bad parameter: show_user_notifications_log_in_link must be of type bool", "parameters[\"show_user_notifications_log_in_link\"]");
            }
            if (parameters.ContainsKey("sftp_host_key_type") && !(parameters["sftp_host_key_type"] is string))
            {
                throw new ArgumentException("Bad parameter: sftp_host_key_type must be of type string", "parameters[\"sftp_host_key_type\"]");
            }
            if (parameters.ContainsKey("active_sftp_host_key_id") && !(parameters["active_sftp_host_key_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: active_sftp_host_key_id must be of type Nullable<Int64>", "parameters[\"active_sftp_host_key_id\"]");
            }
            if (parameters.ContainsKey("protocol_access_groups_only") && !(parameters["protocol_access_groups_only"] is bool))
            {
                throw new ArgumentException("Bad parameter: protocol_access_groups_only must be of type bool", "parameters[\"protocol_access_groups_only\"]");
            }
            if (parameters.ContainsKey("bundle_watermark_value") && !(parameters["bundle_watermark_value"] is object))
            {
                throw new ArgumentException("Bad parameter: bundle_watermark_value must be of type object", "parameters[\"bundle_watermark_value\"]");
            }
            if (parameters.ContainsKey("group_admins_can_set_user_password") && !(parameters["group_admins_can_set_user_password"] is bool))
            {
                throw new ArgumentException("Bad parameter: group_admins_can_set_user_password must be of type bool", "parameters[\"group_admins_can_set_user_password\"]");
            }
            if (parameters.ContainsKey("bundle_recipient_blacklist_free_email_domains") && !(parameters["bundle_recipient_blacklist_free_email_domains"] is bool))
            {
                throw new ArgumentException("Bad parameter: bundle_recipient_blacklist_free_email_domains must be of type bool", "parameters[\"bundle_recipient_blacklist_free_email_domains\"]");
            }
            if (parameters.ContainsKey("bundle_recipient_blacklist_domains") && !(parameters["bundle_recipient_blacklist_domains"] is string[]))
            {
                throw new ArgumentException("Bad parameter: bundle_recipient_blacklist_domains must be of type string[]", "parameters[\"bundle_recipient_blacklist_domains\"]");
            }
            if (parameters.ContainsKey("admins_bypass_locked_subfolders") && !(parameters["admins_bypass_locked_subfolders"] is bool))
            {
                throw new ArgumentException("Bad parameter: admins_bypass_locked_subfolders must be of type bool", "parameters[\"admins_bypass_locked_subfolders\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_sms") && !(parameters["allowed_2fa_method_sms"] is bool))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_sms must be of type bool", "parameters[\"allowed_2fa_method_sms\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_totp") && !(parameters["allowed_2fa_method_totp"] is bool))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_totp must be of type bool", "parameters[\"allowed_2fa_method_totp\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_webauthn") && !(parameters["allowed_2fa_method_webauthn"] is bool))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_webauthn must be of type bool", "parameters[\"allowed_2fa_method_webauthn\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_yubi") && !(parameters["allowed_2fa_method_yubi"] is bool))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_yubi must be of type bool", "parameters[\"allowed_2fa_method_yubi\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_email") && !(parameters["allowed_2fa_method_email"] is bool))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_email must be of type bool", "parameters[\"allowed_2fa_method_email\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_static") && !(parameters["allowed_2fa_method_static"] is bool))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_static must be of type bool", "parameters[\"allowed_2fa_method_static\"]");
            }
            if (parameters.ContainsKey("allowed_2fa_method_bypass_for_ftp_sftp_dav") && !(parameters["allowed_2fa_method_bypass_for_ftp_sftp_dav"] is bool))
            {
                throw new ArgumentException("Bad parameter: allowed_2fa_method_bypass_for_ftp_sftp_dav must be of type bool", "parameters[\"allowed_2fa_method_bypass_for_ftp_sftp_dav\"]");
            }
            if (parameters.ContainsKey("require_2fa") && !(parameters["require_2fa"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_2fa must be of type bool", "parameters[\"require_2fa\"]");
            }
            if (parameters.ContainsKey("require_2fa_user_type") && !(parameters["require_2fa_user_type"] is string))
            {
                throw new ArgumentException("Bad parameter: require_2fa_user_type must be of type string", "parameters[\"require_2fa_user_type\"]");
            }
            if (parameters.ContainsKey("color2_top") && !(parameters["color2_top"] is string))
            {
                throw new ArgumentException("Bad parameter: color2_top must be of type string", "parameters[\"color2_top\"]");
            }
            if (parameters.ContainsKey("color2_left") && !(parameters["color2_left"] is string))
            {
                throw new ArgumentException("Bad parameter: color2_left must be of type string", "parameters[\"color2_left\"]");
            }
            if (parameters.ContainsKey("color2_link") && !(parameters["color2_link"] is string))
            {
                throw new ArgumentException("Bad parameter: color2_link must be of type string", "parameters[\"color2_link\"]");
            }
            if (parameters.ContainsKey("color2_text") && !(parameters["color2_text"] is string))
            {
                throw new ArgumentException("Bad parameter: color2_text must be of type string", "parameters[\"color2_text\"]");
            }
            if (parameters.ContainsKey("color2_top_text") && !(parameters["color2_top_text"] is string))
            {
                throw new ArgumentException("Bad parameter: color2_top_text must be of type string", "parameters[\"color2_top_text\"]");
            }
            if (parameters.ContainsKey("site_header") && !(parameters["site_header"] is string))
            {
                throw new ArgumentException("Bad parameter: site_header must be of type string", "parameters[\"site_header\"]");
            }
            if (parameters.ContainsKey("site_footer") && !(parameters["site_footer"] is string))
            {
                throw new ArgumentException("Bad parameter: site_footer must be of type string", "parameters[\"site_footer\"]");
            }
            if (parameters.ContainsKey("login_help_text") && !(parameters["login_help_text"] is string))
            {
                throw new ArgumentException("Bad parameter: login_help_text must be of type string", "parameters[\"login_help_text\"]");
            }
            if (parameters.ContainsKey("use_dedicated_ips_for_smtp") && !(parameters["use_dedicated_ips_for_smtp"] is bool))
            {
                throw new ArgumentException("Bad parameter: use_dedicated_ips_for_smtp must be of type bool", "parameters[\"use_dedicated_ips_for_smtp\"]");
            }
            if (parameters.ContainsKey("smtp_address") && !(parameters["smtp_address"] is string))
            {
                throw new ArgumentException("Bad parameter: smtp_address must be of type string", "parameters[\"smtp_address\"]");
            }
            if (parameters.ContainsKey("smtp_authentication") && !(parameters["smtp_authentication"] is string))
            {
                throw new ArgumentException("Bad parameter: smtp_authentication must be of type string", "parameters[\"smtp_authentication\"]");
            }
            if (parameters.ContainsKey("smtp_from") && !(parameters["smtp_from"] is string))
            {
                throw new ArgumentException("Bad parameter: smtp_from must be of type string", "parameters[\"smtp_from\"]");
            }
            if (parameters.ContainsKey("smtp_username") && !(parameters["smtp_username"] is string))
            {
                throw new ArgumentException("Bad parameter: smtp_username must be of type string", "parameters[\"smtp_username\"]");
            }
            if (parameters.ContainsKey("smtp_port") && !(parameters["smtp_port"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: smtp_port must be of type Nullable<Int64>", "parameters[\"smtp_port\"]");
            }
            if (parameters.ContainsKey("ldap_enabled") && !(parameters["ldap_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: ldap_enabled must be of type bool", "parameters[\"ldap_enabled\"]");
            }
            if (parameters.ContainsKey("ldap_type") && !(parameters["ldap_type"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_type must be of type string", "parameters[\"ldap_type\"]");
            }
            if (parameters.ContainsKey("ldap_host") && !(parameters["ldap_host"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_host must be of type string", "parameters[\"ldap_host\"]");
            }
            if (parameters.ContainsKey("ldap_host_2") && !(parameters["ldap_host_2"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_host_2 must be of type string", "parameters[\"ldap_host_2\"]");
            }
            if (parameters.ContainsKey("ldap_host_3") && !(parameters["ldap_host_3"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_host_3 must be of type string", "parameters[\"ldap_host_3\"]");
            }
            if (parameters.ContainsKey("ldap_port") && !(parameters["ldap_port"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: ldap_port must be of type Nullable<Int64>", "parameters[\"ldap_port\"]");
            }
            if (parameters.ContainsKey("ldap_secure") && !(parameters["ldap_secure"] is bool))
            {
                throw new ArgumentException("Bad parameter: ldap_secure must be of type bool", "parameters[\"ldap_secure\"]");
            }
            if (parameters.ContainsKey("ldap_username") && !(parameters["ldap_username"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_username must be of type string", "parameters[\"ldap_username\"]");
            }
            if (parameters.ContainsKey("ldap_username_field") && !(parameters["ldap_username_field"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_username_field must be of type string", "parameters[\"ldap_username_field\"]");
            }
            if (parameters.ContainsKey("ldap_domain") && !(parameters["ldap_domain"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_domain must be of type string", "parameters[\"ldap_domain\"]");
            }
            if (parameters.ContainsKey("ldap_user_action") && !(parameters["ldap_user_action"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_user_action must be of type string", "parameters[\"ldap_user_action\"]");
            }
            if (parameters.ContainsKey("ldap_group_action") && !(parameters["ldap_group_action"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_group_action must be of type string", "parameters[\"ldap_group_action\"]");
            }
            if (parameters.ContainsKey("ldap_user_include_groups") && !(parameters["ldap_user_include_groups"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_user_include_groups must be of type string", "parameters[\"ldap_user_include_groups\"]");
            }
            if (parameters.ContainsKey("ldap_group_exclusion") && !(parameters["ldap_group_exclusion"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_group_exclusion must be of type string", "parameters[\"ldap_group_exclusion\"]");
            }
            if (parameters.ContainsKey("ldap_group_inclusion") && !(parameters["ldap_group_inclusion"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_group_inclusion must be of type string", "parameters[\"ldap_group_inclusion\"]");
            }
            if (parameters.ContainsKey("ldap_base_dn") && !(parameters["ldap_base_dn"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_base_dn must be of type string", "parameters[\"ldap_base_dn\"]");
            }
            if (parameters.ContainsKey("uploads_via_email_authentication") && !(parameters["uploads_via_email_authentication"] is bool))
            {
                throw new ArgumentException("Bad parameter: uploads_via_email_authentication must be of type bool", "parameters[\"uploads_via_email_authentication\"]");
            }
            if (parameters.ContainsKey("icon16_file") && !(parameters["icon16_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: icon16_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"icon16_file\"]");
            }
            if (parameters.ContainsKey("icon16_delete") && !(parameters["icon16_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: icon16_delete must be of type bool", "parameters[\"icon16_delete\"]");
            }
            if (parameters.ContainsKey("icon32_file") && !(parameters["icon32_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: icon32_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"icon32_file\"]");
            }
            if (parameters.ContainsKey("icon32_delete") && !(parameters["icon32_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: icon32_delete must be of type bool", "parameters[\"icon32_delete\"]");
            }
            if (parameters.ContainsKey("icon48_file") && !(parameters["icon48_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: icon48_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"icon48_file\"]");
            }
            if (parameters.ContainsKey("icon48_delete") && !(parameters["icon48_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: icon48_delete must be of type bool", "parameters[\"icon48_delete\"]");
            }
            if (parameters.ContainsKey("icon128_file") && !(parameters["icon128_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: icon128_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"icon128_file\"]");
            }
            if (parameters.ContainsKey("icon128_delete") && !(parameters["icon128_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: icon128_delete must be of type bool", "parameters[\"icon128_delete\"]");
            }
            if (parameters.ContainsKey("logo_file") && !(parameters["logo_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: logo_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"logo_file\"]");
            }
            if (parameters.ContainsKey("logo_delete") && !(parameters["logo_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: logo_delete must be of type bool", "parameters[\"logo_delete\"]");
            }
            if (parameters.ContainsKey("bundle_watermark_attachment_file") && !(parameters["bundle_watermark_attachment_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: bundle_watermark_attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"bundle_watermark_attachment_file\"]");
            }
            if (parameters.ContainsKey("bundle_watermark_attachment_delete") && !(parameters["bundle_watermark_attachment_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: bundle_watermark_attachment_delete must be of type bool", "parameters[\"bundle_watermark_attachment_delete\"]");
            }
            if (parameters.ContainsKey("login_page_background_image_file") && !(parameters["login_page_background_image_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: login_page_background_image_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"login_page_background_image_file\"]");
            }
            if (parameters.ContainsKey("login_page_background_image_delete") && !(parameters["login_page_background_image_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: login_page_background_image_delete must be of type bool", "parameters[\"login_page_background_image_delete\"]");
            }
            if (parameters.ContainsKey("disable_2fa_with_delay") && !(parameters["disable_2fa_with_delay"] is bool))
            {
                throw new ArgumentException("Bad parameter: disable_2fa_with_delay must be of type bool", "parameters[\"disable_2fa_with_delay\"]");
            }
            if (parameters.ContainsKey("ldap_password_change") && !(parameters["ldap_password_change"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_password_change must be of type string", "parameters[\"ldap_password_change\"]");
            }
            if (parameters.ContainsKey("ldap_password_change_confirmation") && !(parameters["ldap_password_change_confirmation"] is string))
            {
                throw new ArgumentException("Bad parameter: ldap_password_change_confirmation must be of type string", "parameters[\"ldap_password_change_confirmation\"]");
            }
            if (parameters.ContainsKey("smtp_password") && !(parameters["smtp_password"] is string))
            {
                throw new ArgumentException("Bad parameter: smtp_password must be of type string", "parameters[\"smtp_password\"]");
            }
            if (parameters.ContainsKey("session_expiry_minutes") && !(parameters["session_expiry_minutes"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: session_expiry_minutes must be of type Nullable<Int64>", "parameters[\"session_expiry_minutes\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/site", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Site>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}