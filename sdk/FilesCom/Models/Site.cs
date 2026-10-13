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
    /// SiteEntity model
    /// </summary>
    public class Site : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a Site with no attributes set and no options.
        /// </summary>
        public Site() : this(null, null) { }

        /// <summary>
        /// Makes a Site with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Site(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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
            if (!this.attributes.ContainsKey("ai_feature_availability"))
            {
                this.attributes.Add("ai_feature_availability", null);
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
            if (!this.attributes.ContainsKey("allow_user_level_2fa_override"))
            {
                this.attributes.Add("allow_user_level_2fa_override", false);
            }
            if (!this.attributes.ContainsKey("allow_user_level_allowed_ip_override"))
            {
                this.attributes.Add("allow_user_level_allowed_ip_override", false);
            }
            if (!this.attributes.ContainsKey("allow_user_level_ssl_override"))
            {
                this.attributes.Add("allow_user_level_ssl_override", false);
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
            if (!this.attributes.ContainsKey("as2_message_retention_days"))
            {
                this.attributes.Add("as2_message_retention_days", null);
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
            if (!this.attributes.ContainsKey("bundles_default_owned_by_primary_group"))
            {
                this.attributes.Add("bundles_default_owned_by_primary_group", false);
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
            if (!this.attributes.ContainsKey("bundle_send_one_time_password_to_recipient_at_registration"))
            {
                this.attributes.Add("bundle_send_one_time_password_to_recipient_at_registration", false);
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
            if (!this.attributes.ContainsKey("child_site_folder_admins_can_see_parent_site_directory"))
            {
                this.attributes.Add("child_site_folder_admins_can_see_parent_site_directory", false);
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
            if (!this.attributes.ContainsKey("migrate_remote_server_sync_to_sync"))
            {
                this.attributes.Add("migrate_remote_server_sync_to_sync", false);
            }
            if (!this.attributes.ContainsKey("mcp_dcr_enabled"))
            {
                this.attributes.Add("mcp_dcr_enabled", false);
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
            if (!this.attributes.ContainsKey("disable_all_ai_features"))
            {
                this.attributes.Add("disable_all_ai_features", false);
            }
            if (!this.attributes.ContainsKey("disable_files_certificate_generation"))
            {
                this.attributes.Add("disable_files_certificate_generation", false);
            }
            if (!this.attributes.ContainsKey("disable_notifications"))
            {
                this.attributes.Add("disable_notifications", false);
            }
            if (!this.attributes.ContainsKey("file_activity_events_enabled"))
            {
                this.attributes.Add("file_activity_events_enabled", false);
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
            if (!this.attributes.ContainsKey("fedramp"))
            {
                this.attributes.Add("fedramp", false);
            }
            if (!this.attributes.ContainsKey("files_com_remote_server_enabled"))
            {
                this.attributes.Add("files_com_remote_server_enabled", false);
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
            if (!this.attributes.ContainsKey("group_admins_can_add_users"))
            {
                this.attributes.Add("group_admins_can_add_users", false);
            }
            if (!this.attributes.ContainsKey("group_admins_can_manage_group_memberships"))
            {
                this.attributes.Add("group_admins_can_manage_group_memberships", false);
            }
            if (!this.attributes.ContainsKey("group_admins_can_delete_users"))
            {
                this.attributes.Add("group_admins_can_delete_users", false);
            }
            if (!this.attributes.ContainsKey("group_admins_can_enable_disable_users"))
            {
                this.attributes.Add("group_admins_can_enable_disable_users", false);
            }
            if (!this.attributes.ContainsKey("group_admins_can_modify_users"))
            {
                this.attributes.Add("group_admins_can_modify_users", false);
            }
            if (!this.attributes.ContainsKey("group_admins_can_bypass_user_lifecycle_rules"))
            {
                this.attributes.Add("group_admins_can_bypass_user_lifecycle_rules", false);
            }
            if (!this.attributes.ContainsKey("group_admins_can_reset_passwords"))
            {
                this.attributes.Add("group_admins_can_reset_passwords", false);
            }
            if (!this.attributes.ContainsKey("group_admins_can_set_user_password"))
            {
                this.attributes.Add("group_admins_can_set_user_password", false);
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
            if (!this.attributes.ContainsKey("ldap_server_certificate"))
            {
                this.attributes.Add("ldap_server_certificate", null);
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
            if (!this.attributes.ContainsKey("managed_site_settings"))
            {
                this.attributes.Add("managed_site_settings", null);
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
            if (!this.attributes.ContainsKey("restrict_root_folder_behaviors_to_site_admins"))
            {
                this.attributes.Add("restrict_root_folder_behaviors_to_site_admins", false);
            }
            if (!this.attributes.ContainsKey("root_folder_behaviors_apply_to_workspaces"))
            {
                this.attributes.Add("root_folder_behaviors_apply_to_workspaces", false);
            }
            if (!this.attributes.ContainsKey("require_2fa_exempt_all_sso_users"))
            {
                this.attributes.Add("require_2fa_exempt_all_sso_users", false);
            }
            if (!this.attributes.ContainsKey("require_2fa_stop_time"))
            {
                this.attributes.Add("require_2fa_stop_time", null);
            }
            if (!this.attributes.ContainsKey("revoke_bundle_access_on_disable_or_delete"))
            {
                this.attributes.Add("revoke_bundle_access_on_disable_or_delete", false);
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
            if (!this.attributes.ContainsKey("s3_compatible_endpoint_enabled"))
            {
                this.attributes.Add("s3_compatible_endpoint_enabled", false);
            }
            if (!this.attributes.ContainsKey("sftp_enabled"))
            {
                this.attributes.Add("sftp_enabled", false);
            }
            if (!this.attributes.ContainsKey("sftp_finalize_partial_uploads"))
            {
                this.attributes.Add("sftp_finalize_partial_uploads", false);
            }
            if (!this.attributes.ContainsKey("sftp_host_key_type"))
            {
                this.attributes.Add("sftp_host_key_type", null);
            }
            if (!this.attributes.ContainsKey("active_sftp_host_key_id"))
            {
                this.attributes.Add("active_sftp_host_key_id", null);
            }
            if (!this.attributes.ContainsKey("active_sftp_host_key_ids"))
            {
                this.attributes.Add("active_sftp_host_key_ids", new Nullable<Int64>[0]);
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
            if (!this.attributes.ContainsKey("site_public_footer"))
            {
                this.attributes.Add("site_public_footer", null);
            }
            if (!this.attributes.ContainsKey("site_public_header"))
            {
                this.attributes.Add("site_public_header", null);
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
            if (!this.attributes.ContainsKey("smtp_ssl"))
            {
                this.attributes.Add("smtp_ssl", null);
            }
            if (!this.attributes.ContainsKey("smtp_username"))
            {
                this.attributes.Add("smtp_username", null);
            }
            if (!this.attributes.ContainsKey("session_expiry_minutes"))
            {
                this.attributes.Add("session_expiry_minutes", null);
            }
            if (!this.attributes.ContainsKey("snapshot_sharing_enabled"))
            {
                this.attributes.Add("snapshot_sharing_enabled", false);
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
            if (!this.attributes.ContainsKey("username_display"))
            {
                this.attributes.Add("username_display", null);
            }
            if (!this.attributes.ContainsKey("welcome_custom_text"))
            {
                this.attributes.Add("welcome_custom_text", null);
            }
            if (!this.attributes.ContainsKey("email_footer_custom_text"))
            {
                this.attributes.Add("email_footer_custom_text", null);
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
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => null;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[] { BundleWatermarkAttachment, Icon128, Icon16, Icon32, Icon48, Logo, LoginPageBackgroundImage, Session, User }; }
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
        /// Availability settings for AI features. Each feature requires the site_admins, workspace_admins, folder_admins, and all_users keys. Optional selected_group_members defaults to false; when true, members of at least one group in group_ids get access regardless of the other options. All availability options are additive: any enabled option matching the user grants access. Optional group_ids is an array of integer IDs of active groups on this site, from any workspace. Omitted or empty group_ids grants no access through selected_group_members and does not affect other options. Disabling all AI features overrides these settings.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ai_feature_availability")]
        public object AiFeatureAvailability
        {
            get { return (object)attributes["ai_feature_availability"]; }
            private set { attributes["ai_feature_availability"] = value; }
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
        /// Allow the site-wide two-factor authentication requirement to be overriden on a per-user-basis?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_user_level_2fa_override")]
        public bool AllowUserLevel2faOverride
        {
            get { return attributes["allow_user_level_2fa_override"] == null ? false : (bool)attributes["allow_user_level_2fa_override"]; }
            private set { attributes["allow_user_level_2fa_override"] = value; }
        }

        /// <summary>
        /// Allow the site-wide allowed IP restriction to be overriden on a per-user-basis?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_user_level_allowed_ip_override")]
        public bool AllowUserLevelAllowedIpOverride
        {
            get { return attributes["allow_user_level_allowed_ip_override"] == null ? false : (bool)attributes["allow_user_level_allowed_ip_override"]; }
            private set { attributes["allow_user_level_allowed_ip_override"] = value; }
        }

        /// <summary>
        /// Allow the site-wide FTP SSL requirement to be overriden on a per-user-basis?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_user_level_ssl_override")]
        public bool AllowUserLevelSslOverride
        {
            get { return attributes["allow_user_level_ssl_override"] == null ? false : (bool)attributes["allow_user_level_ssl_override"]; }
            private set { attributes["allow_user_level_ssl_override"] = value; }
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
        /// Number of days to retain AS2 messages (incoming and outgoing).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_message_retention_days")]
        public Nullable<Int64> As2MessageRetentionDays
        {
            get { return (Nullable<Int64>)attributes["as2_message_retention_days"]; }
            private set { attributes["as2_message_retention_days"] = value; }
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
        /// If true, new Share Links created by a user with a primary group will default to that group as owner.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bundles_default_owned_by_primary_group")]
        public bool BundlesDefaultOwnedByPrimaryGroup
        {
            get { return attributes["bundles_default_owned_by_primary_group"] == null ? false : (bool)attributes["bundles_default_owned_by_primary_group"]; }
            private set { attributes["bundles_default_owned_by_primary_group"] = value; }
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
        /// If true, new Share Links must send a one-time password to the recipient when they register. Requires bundle_require_share_recipient and cannot be enabled with bundle_password_required.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bundle_send_one_time_password_to_recipient_at_registration")]
        public bool BundleSendOneTimePasswordToRecipientAtRegistration
        {
            get { return attributes["bundle_send_one_time_password_to_recipient_at_registration"] == null ? false : (bool)attributes["bundle_send_one_time_password_to_recipient_at_registration"]; }
            private set { attributes["bundle_send_one_time_password_to_recipient_at_registration"] = value; }
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
        /// If true, administrators on this site's Child Sites, including Folder Admins, can list this site's Default Workspace users and groups and grant them folder permissions and notifications on Child Site paths.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("child_site_folder_admins_can_see_parent_site_directory")]
        public bool ChildSiteFolderAdminsCanSeeParentSiteDirectory
        {
            get { return attributes["child_site_folder_admins_can_see_parent_site_directory"] == null ? false : (bool)attributes["child_site_folder_admins_can_see_parent_site_directory"]; }
            private set { attributes["child_site_folder_admins_can_see_parent_site_directory"] = value; }
        }

        /// <summary>
        /// Require email authentication, virus, and spam checks for incoming emails to Inboxes and Incoming Email Automations in every Workspace on this site?
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
        /// If true, we will migrate all remote server syncs to the new Sync model.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("migrate_remote_server_sync_to_sync")]
        public bool MigrateRemoteServerSyncToSync
        {
            get { return attributes["migrate_remote_server_sync_to_sync"] == null ? false : (bool)attributes["migrate_remote_server_sync_to_sync"]; }
            private set { attributes["migrate_remote_server_sync_to_sync"] = value; }
        }

        /// <summary>
        /// Is OAuth DCR (dynamic client registration) for MCP enabled?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mcp_dcr_enabled")]
        public bool McpDcrEnabled
        {
            get { return attributes["mcp_dcr_enabled"] == null ? false : (bool)attributes["mcp_dcr_enabled"]; }
            private set { attributes["mcp_dcr_enabled"] = value; }
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
        /// If true, all AI features are disabled for this site.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disable_all_ai_features")]
        public bool DisableAllAiFeatures
        {
            get { return attributes["disable_all_ai_features"] == null ? false : (bool)attributes["disable_all_ai_features"]; }
            private set { attributes["disable_all_ai_features"] = value; }
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
        /// If true, file operations produce file_action.activity.v1 EventChannel events in all workspaces. Defaults to false; subscription filters and workspace access still control delivery.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("file_activity_events_enabled")]
        public bool FileActivityEventsEnabled
        {
            get { return attributes["file_activity_events_enabled"] == null ? false : (bool)attributes["file_activity_events_enabled"]; }
            private set { attributes["file_activity_events_enabled"] = value; }
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
        /// Letsencrypt chain to use when registering SSL Certificate for domain. No longer used as of 2026.
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
        /// Are FedRAMP security restrictions enabled for this site?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("fedramp")]
        public bool Fedramp
        {
            get { return attributes["fedramp"] == null ? false : (bool)attributes["fedramp"]; }
            private set { attributes["fedramp"] = value; }
        }

        /// <summary>
        /// Allow other Files.com sites to use this site's API keys for native Files.com Remote Server connections? Defaults to true and applies to all keys and workspaces on this site. When false, new pairings and access through existing connections are rejected without revoking keys or deleting connections. Re-enabling permits access again with usable keys. Does not disable this site's connections to other sites, ordinary API access, or Connected Sites.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("files_com_remote_server_enabled")]
        public bool FilesComRemoteServerEnabled
        {
            get { return attributes["files_com_remote_server_enabled"] == null ? false : (bool)attributes["files_com_remote_server_enabled"]; }
            private set { attributes["files_com_remote_server_enabled"] = value; }
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
        /// If true, permissions for this site must be bound to a group (not a user).
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
        /// Allow group admins to create users in their groups
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("group_admins_can_add_users")]
        public bool GroupAdminsCanAddUsers
        {
            get { return attributes["group_admins_can_add_users"] == null ? false : (bool)attributes["group_admins_can_add_users"]; }
            private set { attributes["group_admins_can_add_users"] = value; }
        }

        /// <summary>
        /// Allow group admins to add or remove existing users in their groups
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("group_admins_can_manage_group_memberships")]
        public bool GroupAdminsCanManageGroupMemberships
        {
            get { return attributes["group_admins_can_manage_group_memberships"] == null ? false : (bool)attributes["group_admins_can_manage_group_memberships"]; }
            private set { attributes["group_admins_can_manage_group_memberships"] = value; }
        }

        /// <summary>
        /// Allow group admins to delete users in their groups
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("group_admins_can_delete_users")]
        public bool GroupAdminsCanDeleteUsers
        {
            get { return attributes["group_admins_can_delete_users"] == null ? false : (bool)attributes["group_admins_can_delete_users"]; }
            private set { attributes["group_admins_can_delete_users"] = value; }
        }

        /// <summary>
        /// Allow group admins to enable or disable users in their groups
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("group_admins_can_enable_disable_users")]
        public bool GroupAdminsCanEnableDisableUsers
        {
            get { return attributes["group_admins_can_enable_disable_users"] == null ? false : (bool)attributes["group_admins_can_enable_disable_users"]; }
            private set { attributes["group_admins_can_enable_disable_users"] = value; }
        }

        /// <summary>
        /// Allow group admins to modify users in their groups
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("group_admins_can_modify_users")]
        public bool GroupAdminsCanModifyUsers
        {
            get { return attributes["group_admins_can_modify_users"] == null ? false : (bool)attributes["group_admins_can_modify_users"]; }
            private set { attributes["group_admins_can_modify_users"] = value; }
        }

        /// <summary>
        /// Allow group admins to exempt users in their groups from lifecycle rules
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("group_admins_can_bypass_user_lifecycle_rules")]
        public bool GroupAdminsCanBypassUserLifecycleRules
        {
            get { return attributes["group_admins_can_bypass_user_lifecycle_rules"] == null ? false : (bool)attributes["group_admins_can_bypass_user_lifecycle_rules"]; }
            private set { attributes["group_admins_can_bypass_user_lifecycle_rules"] = value; }
        }

        /// <summary>
        /// Allow group admins to reset passwords for users in their groups
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("group_admins_can_reset_passwords")]
        public bool GroupAdminsCanResetPasswords
        {
            get { return attributes["group_admins_can_reset_passwords"] == null ? false : (bool)attributes["group_admins_can_reset_passwords"]; }
            private set { attributes["group_admins_can_reset_passwords"] = value; }
        }

        /// <summary>
        /// Allow group admins to set password authentication method
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
        /// How to validate the LDAP server certificate. `require_match` validates the certificate chain and hostname; `allow_any` disables certificate validation.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ldap_server_certificate")]
        public string LdapServerCertificate
        {
            get { return (string)attributes["ldap_server_certificate"]; }
            private set { attributes["ldap_server_certificate"] = value; }
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
        /// Site settings managed by the parent site. Each key identifies a managed setting, with a null value. Non-secret effective values are returned in their own Site settings fields.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("managed_site_settings")]
        public object ManagedSiteSettings
        {
            get { return (object)attributes["managed_site_settings"]; }
            private set { attributes["managed_site_settings"] = value; }
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
        public Nullable<decimal> NextBillingAmount
        {
            get { return (Nullable<decimal>)attributes["next_billing_amount"]; }
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
        /// If true, only site admins may create, modify, or delete any behavior at the site root, or a skip that would disable one.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("restrict_root_folder_behaviors_to_site_admins")]
        public bool RestrictRootFolderBehaviorsToSiteAdmins
        {
            get { return attributes["restrict_root_folder_behaviors_to_site_admins"] == null ? false : (bool)attributes["restrict_root_folder_behaviors_to_site_admins"]; }
            private set { attributes["restrict_root_folder_behaviors_to_site_admins"] = value; }
        }

        /// <summary>
        /// If true, supported protective behaviors at the site root also apply within named workspaces. Requires restrict_root_folder_behaviors_to_site_admins to be enabled.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("root_folder_behaviors_apply_to_workspaces")]
        public bool RootFolderBehaviorsApplyToWorkspaces
        {
            get { return attributes["root_folder_behaviors_apply_to_workspaces"] == null ? false : (bool)attributes["root_folder_behaviors_apply_to_workspaces"]; }
            private set { attributes["root_folder_behaviors_apply_to_workspaces"] = value; }
        }

        /// <summary>
        /// If true, SSO users using the default user-level two-factor authentication setting are exempt from the site-wide two-factor authentication requirement.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("require_2fa_exempt_all_sso_users")]
        public bool Require2faExemptAllSsoUsers
        {
            get { return attributes["require_2fa_exempt_all_sso_users"] == null ? false : (bool)attributes["require_2fa_exempt_all_sso_users"]; }
            private set { attributes["require_2fa_exempt_all_sso_users"] = value; }
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
        /// Auto-removes bundles for disabled/deleted users and enforces bundle expiry within user access period.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("revoke_bundle_access_on_disable_or_delete")]
        public bool RevokeBundleAccessOnDisableOrDelete
        {
            get { return attributes["revoke_bundle_access_on_disable_or_delete"] == null ? false : (bool)attributes["revoke_bundle_access_on_disable_or_delete"]; }
            private set { attributes["revoke_bundle_access_on_disable_or_delete"] = value; }
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
        /// Is the S3-compatible endpoint enabled for all users and workspaces on this site? Defaults to true. When false, user and group S3 permissions do not allow access.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("s3_compatible_endpoint_enabled")]
        public bool S3CompatibleEndpointEnabled
        {
            get { return attributes["s3_compatible_endpoint_enabled"] == null ? false : (bool)attributes["s3_compatible_endpoint_enabled"]; }
            private set { attributes["s3_compatible_endpoint_enabled"] = value; }
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
        /// Finalize partial SFTP uploads from interrupted connections? Default: true.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_finalize_partial_uploads")]
        public bool SftpFinalizePartialUploads
        {
            get { return attributes["sftp_finalize_partial_uploads"] == null ? false : (bool)attributes["sftp_finalize_partial_uploads"]; }
            private set { attributes["sftp_finalize_partial_uploads"] = value; }
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
        /// Ids of the selected custom SFTP Host Keys
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("active_sftp_host_key_ids")]
        public Nullable<Int64>[] ActiveSftpHostKeyIds
        {
            get { return (Nullable<Int64>[])attributes["active_sftp_host_key_ids"]; }
            private set { attributes["active_sftp_host_key_ids"] = value; }
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
        /// Custom site footer text for authenticated pages
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_footer")]
        public string SiteFooter
        {
            get { return (string)attributes["site_footer"]; }
            private set { attributes["site_footer"] = value; }
        }

        /// <summary>
        /// Custom site header text for authenticated pages
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_header")]
        public string SiteHeader
        {
            get { return (string)attributes["site_header"]; }
            private set { attributes["site_header"] = value; }
        }

        /// <summary>
        /// Custom site footer text for public pages
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_public_footer")]
        public string SitePublicFooter
        {
            get { return (string)attributes["site_public_footer"]; }
            private set { attributes["site_public_footer"] = value; }
        }

        /// <summary>
        /// Custom site header text for public pages
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_public_header")]
        public string SitePublicHeader
        {
            get { return (string)attributes["site_public_header"]; }
            private set { attributes["site_public_header"] = value; }
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
        /// Custom SMTP encryption mode: if_available (default) uses STARTTLS when offered and otherwise sends credentials and messages unencrypted. A certificate-verified STARTTLS connection automatically changes if_available to require unless smtp_ssl is managed by a parent policy. require requires STARTTLS before authentication; require_implicit uses TLS from connection start; never disables TLS. TLS verifies the server certificate against smtp_address. Encryption is never automatically downgraded.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smtp_ssl")]
        public string SmtpSsl
        {
            get { return (string)attributes["smtp_ssl"]; }
            private set { attributes["smtp_ssl"] = value; }
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
        /// Allow snapshot share links creation
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("snapshot_sharing_enabled")]
        public bool SnapshotSharingEnabled
        {
            get { return attributes["snapshot_sharing_enabled"] == null ? false : (bool)attributes["snapshot_sharing_enabled"]; }
            private set { attributes["snapshot_sharing_enabled"] = value; }
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
        /// How usernames are displayed in the web UI. Can be `username_only`, `full_name_only`, `full_name_username`, `full_name_company`, or `full_name_username_company`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("username_display")]
        public string UsernameDisplay
        {
            get { return (string)attributes["username_display"]; }
            private set { attributes["username_display"] = value; }
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
        /// Custom footer text for system-generated emails (as Markdown). Supports standard strftime date/time patterns like %Y (4-digit year), %m (month), %d (day).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("email_footer_custom_text")]
        public string EmailFooterCustomText
        {
            get { return (string)attributes["email_footer_custom_text"]; }
            private set { attributes["email_footer_custom_text"] = value; }
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
        /// Sends GET /site to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Site> Get(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return GetCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Site> GetCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendStringRequest(context, $"/site", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Site>(responseJson, context, options);
        }

        /// <summary>
        /// Sends GET /site/usage to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<UsageSnapshot> GetUsage(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return GetUsageCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<UsageSnapshot> GetUsageCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendStringRequest(context, $"/site/usage", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<UsageSnapshot>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Site name
        ///   subdomain - string - Site subdomain
        ///   domain - string - Custom domain
        ///   domain_hsts_header - boolean - Send HSTS (HTTP Strict Transport Security) header when visitors access the site via a custom domain?
        ///   domain_letsencrypt_chain - string - Letsencrypt chain to use when registering SSL Certificate for domain. No longer used as of 2026.
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
        ///   folder_permissions_groups_only - boolean - If true, permissions for this site must be bound to a group (not a user).
        ///   welcome_screen - string - Does the welcome screen appear?
        ///   office_integration_available - boolean - If true, allows users to use a document editing integration.
        ///   office_integration_type - string - Which document editing integration to support. Files.com Editor or Microsoft Office for the Web.
        ///   pin_all_remote_servers_to_site_region - boolean - If true, we will ensure that all internal communications with any remote server are made through the primary region of the site. This setting overrides individual remote server settings.
        ///   motd_text - string - A message to show users when they connect via FTP or SFTP.
        ///   motd_use_for_ftp - boolean - Show message to users connecting via FTP
        ///   motd_use_for_sftp - boolean - Show message to users connecting via SFTP
        ///   left_navigation_visibility - object - Visibility settings for account navigation
        ///   disable_all_ai_features - boolean - If true, all AI features are disabled for this site.
        ///   ai_feature_availability - object - Availability settings for AI features. Each feature requires the site_admins, workspace_admins, folder_admins, and all_users keys. Optional selected_group_members defaults to false; when true, members of at least one group in group_ids get access regardless of the other options. All availability options are additive: any enabled option matching the user grants access. Optional group_ids is an array of integer IDs of active groups on this site, from any workspace. Omitted or empty group_ids grants no access through selected_group_members and does not affect other options. Disabling all AI features overrides these settings.
        ///   mcp_dcr_enabled - boolean - Is OAuth DCR (dynamic client registration) for MCP enabled?
        ///   additional_text_file_types - array(string) - Additional extensions that are considered text files
        ///   bundle_require_note - boolean - Do Bundles require internal notes?
        ///   bundle_send_shared_receipts - boolean - Do Bundle creators receive receipts of invitations?
        ///   bundles_default_owned_by_primary_group - boolean - If true, new Share Links created by a user with a primary group will default to that group as owner.
        ///   calculate_file_checksums_crc32 - boolean - Calculate CRC32 checksums for files?
        ///   calculate_file_checksums_md5 - boolean - Calculate MD5 checksums for files?
        ///   calculate_file_checksums_sha1 - boolean - Calculate SHA1 checksums for files?
        ///   calculate_file_checksums_sha256 - boolean - Calculate SHA256 checksums for files?
        ///   legacy_checksums_mode - boolean - Use legacy checksums mode?
        ///   migrate_remote_server_sync_to_sync - boolean - If true, we will migrate all remote server syncs to the new Sync model.
        ///   as2_message_retention_days - int64 - Number of days to retain AS2 messages (incoming and outgoing).
        ///   file_activity_events_enabled - boolean - If true, file operations produce file_action.activity.v1 EventChannel events in all workspaces. Defaults to false; subscription filters and workspace access still control delivery.
        ///   username_display - string - How usernames are displayed in the web UI. Can be `username_only`, `full_name_only`, `full_name_username`, `full_name_company`, or `full_name_username_company`.
        ///   session_expiry_minutes - int64 - Session expiry in minutes
        ///   ssl_required - boolean - Is SSL required?  Disabling this is insecure.
        ///   sftp_insecure_ciphers - boolean - If true, we will allow weak and known insecure ciphers to be used for SFTP connections.  Enabling this setting severely weakens the security of your site and it is not recommend, except as a last resort for compatibility.
        ///   sftp_insecure_diffie_hellman - boolean - If true, we will allow weak Diffie Hellman parameters to be used within ciphers for SFTP that are otherwise on our secure list.  This has the effect of making the cipher weaker than our normal threshold for security, but is required to support certain legacy or broken SSH and MFT clients.  Enabling this weakens security, but not nearly as much as enabling the full `sftp_insecure_ciphers` option.
        ///   disable_files_certificate_generation - boolean - If set, Files.com will not set the CAA records required to generate future SSL certificates for this domain.
        ///   fedramp - boolean - Are FedRAMP security restrictions enabled for this site?
        ///   user_lockout - boolean - Will users be locked out after incorrect login attempts?
        ///   user_lockout_tries - int64 - Number of login tries within `user_lockout_within` hours before users are locked out
        ///   user_lockout_within - int64 - Number of hours for user lockout window
        ///   user_lockout_lock_period - int64 - How many hours to lock user out for failed password?
        ///   include_password_in_welcome_email - boolean - Include password in emails to new users?
        ///   allowed_countries - string - Comma separated list of allowed Country codes
        ///   allowed_ips - string - List of allowed IP addresses
        ///   allow_user_level_2fa_override - boolean - Allow the site-wide two-factor authentication requirement to be overriden on a per-user-basis?
        ///   allow_user_level_allowed_ip_override - boolean - Allow the site-wide allowed IP restriction to be overriden on a per-user-basis?
        ///   allow_user_level_ssl_override - boolean - Allow the site-wide FTP SSL requirement to be overriden on a per-user-basis?
        ///   disallowed_countries - string - Comma separated list of disallowed Country codes
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
        ///   bundle_send_one_time_password_to_recipient_at_registration - boolean - If true, new Share Links must send a one-time password to the recipient when they register. Requires bundle_require_share_recipient and cannot be enabled with bundle_password_required.
        ///   bundle_registration_notifications - string - Do Bundle owners receive registration notification?
        ///   bundle_activity_notifications - string - Do Bundle owners receive activity notifications?
        ///   bundle_upload_receipt_notifications - string - Do Bundle uploaders receive upload confirmation notifications?
        ///   document_edits_in_bundle_allowed - boolean - If true, allow public viewers of Bundles with full permissions to use document editing integrations.
        ///   password_requirements_apply_to_bundles - boolean - Require bundles' passwords, and passwords for other items (inboxes, public shares, etc.) to conform to the same requirements as users' passwords?
        ///   prevent_root_permissions_for_non_site_admins - boolean - If true, we will prevent non-administrators from receiving any permissions directly on the root folder.  This is commonly used to prevent the accidental application of permissions.
        ///   restrict_root_folder_behaviors_to_site_admins - boolean - If true, only site admins may create, modify, or delete any behavior at the site root, or a skip that would disable one.
        ///   root_folder_behaviors_apply_to_workspaces - boolean - If true, supported protective behaviors at the site root also apply within named workspaces. Requires restrict_root_folder_behaviors_to_site_admins to be enabled.
        ///   opt_out_global - boolean - Use servers in the USA only?
        ///   use_provided_modified_at - boolean - Allow uploaders to set `provided_modified_at` for uploaded files?
        ///   custom_namespace - boolean - Is this site using a custom namespace for users?
        ///   non_sso_groups_allowed - boolean - If true, groups can be manually created / modified / deleted by Site Admins. Otherwise, groups can only be managed via your SSO provider.
        ///   non_sso_users_allowed - boolean - If true, users can be manually created / modified / deleted by Site Admins. Otherwise, users can only be managed via your SSO provider.
        ///   sharing_enabled - boolean - Allow bundle creation
        ///   snapshot_sharing_enabled - boolean - Allow snapshot share links creation
        ///   user_requests_enabled - boolean - Enable User Requests feature
        ///   user_requests_notify_admins - boolean - Send email to site admins when a user request is received?
        ///   dav_enabled - boolean - Is WebDAV enabled?
        ///   files_com_remote_server_enabled - boolean - Allow other Files.com sites to use this site's API keys for native Files.com Remote Server connections? Defaults to true and applies to all keys and workspaces on this site. When false, new pairings and access through existing connections are rejected without revoking keys or deleting connections. Re-enabling permits access again with usable keys. Does not disable this site's connections to other sites, ordinary API access, or Connected Sites.
        ///   ftp_enabled - boolean - Is FTP enabled?
        ///   s3_compatible_endpoint_enabled - boolean - Is the S3-compatible endpoint enabled for all users and workspaces on this site? Defaults to true. When false, user and group S3 permissions do not allow access.
        ///   sftp_enabled - boolean - Is SFTP enabled?
        ///   sftp_finalize_partial_uploads - boolean - Finalize partial SFTP uploads from interrupted connections? Default: true.
        ///   users_can_create_api_keys - boolean - Allow users to create their own API keys?
        ///   users_can_create_ssh_keys - boolean - Allow users to create their own SSH keys?
        ///   show_user_notifications_log_in_link - boolean - Show log in link in user notifications?
        ///   sftp_host_key_type - string - Sftp Host Key Type
        ///   active_sftp_host_key_id - int64 - Id of the currently selected custom SFTP Host Key
        ///   active_sftp_host_key_ids - array(int64) - Ids of the selected custom SFTP Host Keys
        ///   protocol_access_groups_only - boolean - If true, protocol access permissions on users will be ignored, and only protocol access permissions set on Groups will be honored.  Make sure that your current user is a member of a group with API permission when changing this value to avoid locking yourself out of your site.
        ///   revoke_bundle_access_on_disable_or_delete - boolean - Auto-removes bundles for disabled/deleted users and enforces bundle expiry within user access period.
        ///   group_admins_can_add_users - boolean - Allow group admins to create users in their groups
        ///   group_admins_can_manage_group_memberships - boolean - Allow group admins to add or remove existing users in their groups
        ///   group_admins_can_delete_users - boolean - Allow group admins to delete users in their groups
        ///   group_admins_can_enable_disable_users - boolean - Allow group admins to enable or disable users in their groups
        ///   group_admins_can_modify_users - boolean - Allow group admins to modify users in their groups
        ///   group_admins_can_bypass_user_lifecycle_rules - boolean - Allow group admins to exempt users in their groups from lifecycle rules
        ///   group_admins_can_reset_passwords - boolean - Allow group admins to reset passwords for users in their groups
        ///   group_admins_can_set_user_password - boolean - Allow group admins to set password authentication method
        ///   child_site_folder_admins_can_see_parent_site_directory - boolean - If true, administrators on this site's Child Sites, including Folder Admins, can list this site's Default Workspace users and groups and grant them folder permissions and notifications on Child Site paths.
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
        ///   require_2fa_exempt_all_sso_users - boolean - If true, SSO users using the default user-level two-factor authentication setting are exempt from the site-wide two-factor authentication requirement.
        ///   require_2fa_user_type - string - What type of user is required to use two-factor authentication (when require_2fa is set to `true` for this site)?
        ///   color2_top - string - Top bar background color
        ///   color2_left - string - Page link and button color
        ///   color2_link - string - Top bar link color
        ///   color2_text - string - Page link and button color
        ///   color2_top_text - string - Top bar text color
        ///   site_header - string - Custom site header text for authenticated pages
        ///   site_footer - string - Custom site footer text for authenticated pages
        ///   site_public_header - string - Custom site header text for public pages
        ///   site_public_footer - string - Custom site footer text for public pages
        ///   login_help_text - string - Login help text
        ///   use_dedicated_ips_for_smtp - boolean - If using custom SMTP, should we use dedicated IPs to deliver emails?
        ///   email_footer_custom_text - string - Custom footer text for system-generated emails (as Markdown). Supports standard strftime date/time patterns like %Y (4-digit year), %m (month), %d (day).
        ///   smtp_address - string - SMTP server hostname or IP
        ///   smtp_authentication - string - SMTP server authentication type
        ///   smtp_from - string - From address to use when mailing through custom SMTP
        ///   smtp_username - string - SMTP server username
        ///   smtp_port - int64 - SMTP server port
        ///   smtp_ssl - string - Custom SMTP encryption mode: if_available (default) uses STARTTLS when offered and otherwise sends credentials and messages unencrypted. A certificate-verified STARTTLS connection automatically changes if_available to require unless smtp_ssl is managed by a parent policy. require requires STARTTLS before authentication; require_implicit uses TLS from connection start; never disables TLS. TLS verifies the server certificate against smtp_address. Encryption is never automatically downgraded.
        ///   ldap_enabled - boolean - Main LDAP setting: is LDAP enabled?
        ///   ldap_type - string - LDAP type
        ///   ldap_host - string - LDAP host
        ///   ldap_host_2 - string - LDAP backup host
        ///   ldap_host_3 - string - LDAP backup host
        ///   ldap_port - int64 - LDAP port
        ///   ldap_secure - boolean - Use secure LDAP?
        ///   ldap_server_certificate - string - How to validate the LDAP server certificate. `require_match` validates the certificate chain and hostname; `allow_any` disables certificate validation.
        ///   ldap_username - string - Username for signing in to LDAP server.
        ///   ldap_username_field - string - LDAP username field
        ///   ldap_domain - string - Domain name that will be appended to usernames
        ///   ldap_user_action - string - Should we sync users from LDAP server?
        ///   ldap_group_action - string - Should we sync groups from LDAP server?
        ///   ldap_user_include_groups - string - Comma or newline separated list of group names (with optional wildcards) - if provided, only users in these groups will be added or synced.
        ///   ldap_group_exclusion - string - Comma or newline separated list of group names (with optional wildcards) to exclude when syncing.
        ///   ldap_group_inclusion - string - Comma or newline separated list of group names (with optional wildcards) to include when syncing.
        ///   ldap_base_dn - string - Base DN for looking up users in LDAP server
        ///   uploads_via_email_authentication - boolean - Require email authentication, virus, and spam checks for incoming emails to Inboxes and Incoming Email Automations in every Workspace on this site?
        ///   bundle_watermark_value - object - Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
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
        ///   redirect_old_subdomain - boolean - If true, and if changing the site subdomain, then create a redirect from the previous Files.com subdomain to the new Files.com subdomain.
        ///   smtp_password - string - Password for SMTP server.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Site> Update(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Site> UpdateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("subdomain", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: subdomain is null, which cannot be sent" : "Bad parameter: subdomain must be of type string", "subdomain");
            }
            if (parameters.TryGetValue("domain", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: domain is null, which cannot be sent" : "Bad parameter: domain must be of type string", "domain");
            }
            if (parameters.TryGetValue("domain_hsts_header", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: domain_hsts_header is null, which cannot be sent" : "Bad parameter: domain_hsts_header must be of type bool", "domain_hsts_header");
            }
            if (parameters.TryGetValue("domain_letsencrypt_chain", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: domain_letsencrypt_chain is null, which cannot be sent" : "Bad parameter: domain_letsencrypt_chain must be of type string", "domain_letsencrypt_chain");
            }
            if (parameters.TryGetValue("email", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email is null, which cannot be sent" : "Bad parameter: email must be of type string", "email");
            }
            if (parameters.TryGetValue("reply_to_email", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: reply_to_email is null, which cannot be sent" : "Bad parameter: reply_to_email must be of type string", "reply_to_email");
            }
            if (parameters.TryGetValue("allow_bundle_names", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_bundle_names is null, which cannot be sent" : "Bad parameter: allow_bundle_names must be of type bool", "allow_bundle_names");
            }
            if (parameters.TryGetValue("bundle_expiration", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_expiration is null, which cannot be sent" : "Bad parameter: bundle_expiration must be of type Nullable<Int64>", "bundle_expiration");
            }
            if (parameters.TryGetValue("welcome_email_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: welcome_email_enabled is null, which cannot be sent" : "Bad parameter: welcome_email_enabled must be of type bool", "welcome_email_enabled");
            }
            if (parameters.TryGetValue("ask_about_overwrites", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ask_about_overwrites is null, which cannot be sent" : "Bad parameter: ask_about_overwrites must be of type bool", "ask_about_overwrites");
            }
            if (parameters.TryGetValue("show_request_access_link", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: show_request_access_link is null, which cannot be sent" : "Bad parameter: show_request_access_link must be of type bool", "show_request_access_link");
            }
            if (parameters.TryGetValue("always_mkdir_parents", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_mkdir_parents is null, which cannot be sent" : "Bad parameter: always_mkdir_parents must be of type bool", "always_mkdir_parents");
            }
            if (parameters.TryGetValue("welcome_email_cc", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: welcome_email_cc is null, which cannot be sent" : "Bad parameter: welcome_email_cc must be of type string", "welcome_email_cc");
            }
            if (parameters.TryGetValue("welcome_email_subject", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: welcome_email_subject is null, which cannot be sent" : "Bad parameter: welcome_email_subject must be of type string", "welcome_email_subject");
            }
            if (parameters.TryGetValue("welcome_custom_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: welcome_custom_text is null, which cannot be sent" : "Bad parameter: welcome_custom_text must be of type string", "welcome_custom_text");
            }
            if (parameters.TryGetValue("language", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: language is null, which cannot be sent" : "Bad parameter: language must be of type string", "language");
            }
            if (parameters.TryGetValue("windows_mode_ftp", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: windows_mode_ftp is null, which cannot be sent" : "Bad parameter: windows_mode_ftp must be of type bool", "windows_mode_ftp");
            }
            if (parameters.TryGetValue("default_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: default_time_zone is null, which cannot be sent" : "Bad parameter: default_time_zone must be of type string", "default_time_zone");
            }
            if (parameters.TryGetValue("desktop_app", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: desktop_app is null, which cannot be sent" : "Bad parameter: desktop_app must be of type bool", "desktop_app");
            }
            if (parameters.TryGetValue("desktop_app_session_ip_pinning", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: desktop_app_session_ip_pinning is null, which cannot be sent" : "Bad parameter: desktop_app_session_ip_pinning must be of type bool", "desktop_app_session_ip_pinning");
            }
            if (parameters.TryGetValue("desktop_app_session_lifetime", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: desktop_app_session_lifetime is null, which cannot be sent" : "Bad parameter: desktop_app_session_lifetime must be of type Nullable<Int64>", "desktop_app_session_lifetime");
            }
            if (parameters.TryGetValue("mobile_app", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mobile_app is null, which cannot be sent" : "Bad parameter: mobile_app must be of type bool", "mobile_app");
            }
            if (parameters.TryGetValue("mobile_app_session_ip_pinning", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mobile_app_session_ip_pinning is null, which cannot be sent" : "Bad parameter: mobile_app_session_ip_pinning must be of type bool", "mobile_app_session_ip_pinning");
            }
            if (parameters.TryGetValue("mobile_app_session_lifetime", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mobile_app_session_lifetime is null, which cannot be sent" : "Bad parameter: mobile_app_session_lifetime must be of type Nullable<Int64>", "mobile_app_session_lifetime");
            }
            if (parameters.TryGetValue("folder_permissions_groups_only", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: folder_permissions_groups_only is null, which cannot be sent" : "Bad parameter: folder_permissions_groups_only must be of type bool", "folder_permissions_groups_only");
            }
            if (parameters.TryGetValue("welcome_screen", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: welcome_screen is null, which cannot be sent" : "Bad parameter: welcome_screen must be of type string", "welcome_screen");
            }
            if (parameters.TryGetValue("office_integration_available", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: office_integration_available is null, which cannot be sent" : "Bad parameter: office_integration_available must be of type bool", "office_integration_available");
            }
            if (parameters.TryGetValue("office_integration_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: office_integration_type is null, which cannot be sent" : "Bad parameter: office_integration_type must be of type string", "office_integration_type");
            }
            if (parameters.TryGetValue("pin_all_remote_servers_to_site_region", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pin_all_remote_servers_to_site_region is null, which cannot be sent" : "Bad parameter: pin_all_remote_servers_to_site_region must be of type bool", "pin_all_remote_servers_to_site_region");
            }
            if (parameters.TryGetValue("motd_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: motd_text is null, which cannot be sent" : "Bad parameter: motd_text must be of type string", "motd_text");
            }
            if (parameters.TryGetValue("motd_use_for_ftp", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: motd_use_for_ftp is null, which cannot be sent" : "Bad parameter: motd_use_for_ftp must be of type bool", "motd_use_for_ftp");
            }
            if (parameters.TryGetValue("motd_use_for_sftp", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: motd_use_for_sftp is null, which cannot be sent" : "Bad parameter: motd_use_for_sftp must be of type bool", "motd_use_for_sftp");
            }
            if (parameters.TryGetValue("left_navigation_visibility", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: left_navigation_visibility is null, which cannot be sent" : "Bad parameter: left_navigation_visibility must be of type object", "left_navigation_visibility");
            }
            if (parameters.TryGetValue("disable_all_ai_features", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disable_all_ai_features is null, which cannot be sent" : "Bad parameter: disable_all_ai_features must be of type bool", "disable_all_ai_features");
            }
            if (parameters.TryGetValue("ai_feature_availability", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ai_feature_availability is null, which cannot be sent" : "Bad parameter: ai_feature_availability must be of type object", "ai_feature_availability");
            }
            if (parameters.TryGetValue("mcp_dcr_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: mcp_dcr_enabled is null, which cannot be sent" : "Bad parameter: mcp_dcr_enabled must be of type bool", "mcp_dcr_enabled");
            }
            if (parameters.TryGetValue("additional_text_file_types", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: additional_text_file_types is null, which cannot be sent" : "Bad parameter: additional_text_file_types must be of type string[]", "additional_text_file_types");
            }
            if (parameters.TryGetValue("bundle_require_note", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_require_note is null, which cannot be sent" : "Bad parameter: bundle_require_note must be of type bool", "bundle_require_note");
            }
            if (parameters.TryGetValue("bundle_send_shared_receipts", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_send_shared_receipts is null, which cannot be sent" : "Bad parameter: bundle_send_shared_receipts must be of type bool", "bundle_send_shared_receipts");
            }
            if (parameters.TryGetValue("bundles_default_owned_by_primary_group", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundles_default_owned_by_primary_group is null, which cannot be sent" : "Bad parameter: bundles_default_owned_by_primary_group must be of type bool", "bundles_default_owned_by_primary_group");
            }
            if (parameters.TryGetValue("calculate_file_checksums_crc32", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: calculate_file_checksums_crc32 is null, which cannot be sent" : "Bad parameter: calculate_file_checksums_crc32 must be of type bool", "calculate_file_checksums_crc32");
            }
            if (parameters.TryGetValue("calculate_file_checksums_md5", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: calculate_file_checksums_md5 is null, which cannot be sent" : "Bad parameter: calculate_file_checksums_md5 must be of type bool", "calculate_file_checksums_md5");
            }
            if (parameters.TryGetValue("calculate_file_checksums_sha1", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: calculate_file_checksums_sha1 is null, which cannot be sent" : "Bad parameter: calculate_file_checksums_sha1 must be of type bool", "calculate_file_checksums_sha1");
            }
            if (parameters.TryGetValue("calculate_file_checksums_sha256", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: calculate_file_checksums_sha256 is null, which cannot be sent" : "Bad parameter: calculate_file_checksums_sha256 must be of type bool", "calculate_file_checksums_sha256");
            }
            if (parameters.TryGetValue("legacy_checksums_mode", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: legacy_checksums_mode is null, which cannot be sent" : "Bad parameter: legacy_checksums_mode must be of type bool", "legacy_checksums_mode");
            }
            if (parameters.TryGetValue("migrate_remote_server_sync_to_sync", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: migrate_remote_server_sync_to_sync is null, which cannot be sent" : "Bad parameter: migrate_remote_server_sync_to_sync must be of type bool", "migrate_remote_server_sync_to_sync");
            }
            if (parameters.TryGetValue("as2_message_retention_days", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: as2_message_retention_days is null, which cannot be sent" : "Bad parameter: as2_message_retention_days must be of type Nullable<Int64>", "as2_message_retention_days");
            }
            if (parameters.TryGetValue("file_activity_events_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_activity_events_enabled is null, which cannot be sent" : "Bad parameter: file_activity_events_enabled must be of type bool", "file_activity_events_enabled");
            }
            if (parameters.TryGetValue("username_display", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username_display is null, which cannot be sent" : "Bad parameter: username_display must be of type string", "username_display");
            }
            if (parameters.TryGetValue("session_expiry_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: session_expiry_minutes is null, which cannot be sent" : "Bad parameter: session_expiry_minutes must be of type Nullable<Int64>", "session_expiry_minutes");
            }
            if (parameters.TryGetValue("ssl_required", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl_required is null, which cannot be sent" : "Bad parameter: ssl_required must be of type bool", "ssl_required");
            }
            if (parameters.TryGetValue("sftp_insecure_ciphers", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_insecure_ciphers is null, which cannot be sent" : "Bad parameter: sftp_insecure_ciphers must be of type bool", "sftp_insecure_ciphers");
            }
            if (parameters.TryGetValue("sftp_insecure_diffie_hellman", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_insecure_diffie_hellman is null, which cannot be sent" : "Bad parameter: sftp_insecure_diffie_hellman must be of type bool", "sftp_insecure_diffie_hellman");
            }
            if (parameters.TryGetValue("disable_files_certificate_generation", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disable_files_certificate_generation is null, which cannot be sent" : "Bad parameter: disable_files_certificate_generation must be of type bool", "disable_files_certificate_generation");
            }
            if (parameters.TryGetValue("fedramp", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: fedramp is null, which cannot be sent" : "Bad parameter: fedramp must be of type bool", "fedramp");
            }
            if (parameters.TryGetValue("user_lockout", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_lockout is null, which cannot be sent" : "Bad parameter: user_lockout must be of type bool", "user_lockout");
            }
            if (parameters.TryGetValue("user_lockout_tries", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_lockout_tries is null, which cannot be sent" : "Bad parameter: user_lockout_tries must be of type Nullable<Int64>", "user_lockout_tries");
            }
            if (parameters.TryGetValue("user_lockout_within", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_lockout_within is null, which cannot be sent" : "Bad parameter: user_lockout_within must be of type Nullable<Int64>", "user_lockout_within");
            }
            if (parameters.TryGetValue("user_lockout_lock_period", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_lockout_lock_period is null, which cannot be sent" : "Bad parameter: user_lockout_lock_period must be of type Nullable<Int64>", "user_lockout_lock_period");
            }
            if (parameters.TryGetValue("include_password_in_welcome_email", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: include_password_in_welcome_email is null, which cannot be sent" : "Bad parameter: include_password_in_welcome_email must be of type bool", "include_password_in_welcome_email");
            }
            if (parameters.TryGetValue("allowed_countries", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_countries is null, which cannot be sent" : "Bad parameter: allowed_countries must be of type string", "allowed_countries");
            }
            if (parameters.TryGetValue("allowed_ips", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_ips is null, which cannot be sent" : "Bad parameter: allowed_ips must be of type string", "allowed_ips");
            }
            if (parameters.TryGetValue("allow_user_level_2fa_override", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_user_level_2fa_override is null, which cannot be sent" : "Bad parameter: allow_user_level_2fa_override must be of type bool", "allow_user_level_2fa_override");
            }
            if (parameters.TryGetValue("allow_user_level_allowed_ip_override", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_user_level_allowed_ip_override is null, which cannot be sent" : "Bad parameter: allow_user_level_allowed_ip_override must be of type bool", "allow_user_level_allowed_ip_override");
            }
            if (parameters.TryGetValue("allow_user_level_ssl_override", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_user_level_ssl_override is null, which cannot be sent" : "Bad parameter: allow_user_level_ssl_override must be of type bool", "allow_user_level_ssl_override");
            }
            if (parameters.TryGetValue("disallowed_countries", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disallowed_countries is null, which cannot be sent" : "Bad parameter: disallowed_countries must be of type string", "disallowed_countries");
            }
            if (parameters.TryGetValue("days_to_retain_backups", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: days_to_retain_backups is null, which cannot be sent" : "Bad parameter: days_to_retain_backups must be of type Nullable<Int64>", "days_to_retain_backups");
            }
            if (parameters.TryGetValue("max_prior_passwords", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_prior_passwords is null, which cannot be sent" : "Bad parameter: max_prior_passwords must be of type Nullable<Int64>", "max_prior_passwords");
            }
            if (parameters.TryGetValue("password_validity_days", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_validity_days is null, which cannot be sent" : "Bad parameter: password_validity_days must be of type Nullable<Int64>", "password_validity_days");
            }
            if (parameters.TryGetValue("password_min_length", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_min_length is null, which cannot be sent" : "Bad parameter: password_min_length must be of type Nullable<Int64>", "password_min_length");
            }
            if (parameters.TryGetValue("password_require_letter", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_require_letter is null, which cannot be sent" : "Bad parameter: password_require_letter must be of type bool", "password_require_letter");
            }
            if (parameters.TryGetValue("password_require_mixed", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_require_mixed is null, which cannot be sent" : "Bad parameter: password_require_mixed must be of type bool", "password_require_mixed");
            }
            if (parameters.TryGetValue("password_require_special", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_require_special is null, which cannot be sent" : "Bad parameter: password_require_special must be of type bool", "password_require_special");
            }
            if (parameters.TryGetValue("password_require_number", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_require_number is null, which cannot be sent" : "Bad parameter: password_require_number must be of type bool", "password_require_number");
            }
            if (parameters.TryGetValue("password_require_unbreached", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_require_unbreached is null, which cannot be sent" : "Bad parameter: password_require_unbreached must be of type bool", "password_require_unbreached");
            }
            if (parameters.TryGetValue("require_logout_from_bundles_and_inboxes", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_logout_from_bundles_and_inboxes is null, which cannot be sent" : "Bad parameter: require_logout_from_bundles_and_inboxes must be of type bool", "require_logout_from_bundles_and_inboxes");
            }
            if (parameters.TryGetValue("dav_user_root_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dav_user_root_enabled is null, which cannot be sent" : "Bad parameter: dav_user_root_enabled must be of type bool", "dav_user_root_enabled");
            }
            if (parameters.TryGetValue("sftp_user_root_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_user_root_enabled is null, which cannot be sent" : "Bad parameter: sftp_user_root_enabled must be of type bool", "sftp_user_root_enabled");
            }
            if (parameters.TryGetValue("disable_password_reset", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disable_password_reset is null, which cannot be sent" : "Bad parameter: disable_password_reset must be of type bool", "disable_password_reset");
            }
            if (parameters.TryGetValue("immutable_files", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: immutable_files is null, which cannot be sent" : "Bad parameter: immutable_files must be of type bool", "immutable_files");
            }
            if (parameters.TryGetValue("bundle_not_found_message", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_not_found_message is null, which cannot be sent" : "Bad parameter: bundle_not_found_message must be of type string", "bundle_not_found_message");
            }
            if (parameters.TryGetValue("bundle_password_required", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_password_required is null, which cannot be sent" : "Bad parameter: bundle_password_required must be of type bool", "bundle_password_required");
            }
            if (parameters.TryGetValue("bundle_require_registration", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_require_registration is null, which cannot be sent" : "Bad parameter: bundle_require_registration must be of type bool", "bundle_require_registration");
            }
            if (parameters.TryGetValue("bundle_require_share_recipient", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_require_share_recipient is null, which cannot be sent" : "Bad parameter: bundle_require_share_recipient must be of type bool", "bundle_require_share_recipient");
            }
            if (parameters.TryGetValue("bundle_send_one_time_password_to_recipient_at_registration", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_send_one_time_password_to_recipient_at_registration is null, which cannot be sent" : "Bad parameter: bundle_send_one_time_password_to_recipient_at_registration must be of type bool", "bundle_send_one_time_password_to_recipient_at_registration");
            }
            if (parameters.TryGetValue("bundle_registration_notifications", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_registration_notifications is null, which cannot be sent" : "Bad parameter: bundle_registration_notifications must be of type string", "bundle_registration_notifications");
            }
            if (parameters.TryGetValue("bundle_activity_notifications", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_activity_notifications is null, which cannot be sent" : "Bad parameter: bundle_activity_notifications must be of type string", "bundle_activity_notifications");
            }
            if (parameters.TryGetValue("bundle_upload_receipt_notifications", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_upload_receipt_notifications is null, which cannot be sent" : "Bad parameter: bundle_upload_receipt_notifications must be of type string", "bundle_upload_receipt_notifications");
            }
            if (parameters.TryGetValue("document_edits_in_bundle_allowed", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: document_edits_in_bundle_allowed is null, which cannot be sent" : "Bad parameter: document_edits_in_bundle_allowed must be of type bool", "document_edits_in_bundle_allowed");
            }
            if (parameters.TryGetValue("password_requirements_apply_to_bundles", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_requirements_apply_to_bundles is null, which cannot be sent" : "Bad parameter: password_requirements_apply_to_bundles must be of type bool", "password_requirements_apply_to_bundles");
            }
            if (parameters.TryGetValue("prevent_root_permissions_for_non_site_admins", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: prevent_root_permissions_for_non_site_admins is null, which cannot be sent" : "Bad parameter: prevent_root_permissions_for_non_site_admins must be of type bool", "prevent_root_permissions_for_non_site_admins");
            }
            if (parameters.TryGetValue("restrict_root_folder_behaviors_to_site_admins", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restrict_root_folder_behaviors_to_site_admins is null, which cannot be sent" : "Bad parameter: restrict_root_folder_behaviors_to_site_admins must be of type bool", "restrict_root_folder_behaviors_to_site_admins");
            }
            if (parameters.TryGetValue("root_folder_behaviors_apply_to_workspaces", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: root_folder_behaviors_apply_to_workspaces is null, which cannot be sent" : "Bad parameter: root_folder_behaviors_apply_to_workspaces must be of type bool", "root_folder_behaviors_apply_to_workspaces");
            }
            if (parameters.TryGetValue("opt_out_global", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: opt_out_global is null, which cannot be sent" : "Bad parameter: opt_out_global must be of type bool", "opt_out_global");
            }
            if (parameters.TryGetValue("use_provided_modified_at", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: use_provided_modified_at is null, which cannot be sent" : "Bad parameter: use_provided_modified_at must be of type bool", "use_provided_modified_at");
            }
            if (parameters.TryGetValue("custom_namespace", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: custom_namespace is null, which cannot be sent" : "Bad parameter: custom_namespace must be of type bool", "custom_namespace");
            }
            if (parameters.TryGetValue("non_sso_groups_allowed", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: non_sso_groups_allowed is null, which cannot be sent" : "Bad parameter: non_sso_groups_allowed must be of type bool", "non_sso_groups_allowed");
            }
            if (parameters.TryGetValue("non_sso_users_allowed", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: non_sso_users_allowed is null, which cannot be sent" : "Bad parameter: non_sso_users_allowed must be of type bool", "non_sso_users_allowed");
            }
            if (parameters.TryGetValue("sharing_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharing_enabled is null, which cannot be sent" : "Bad parameter: sharing_enabled must be of type bool", "sharing_enabled");
            }
            if (parameters.TryGetValue("snapshot_sharing_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: snapshot_sharing_enabled is null, which cannot be sent" : "Bad parameter: snapshot_sharing_enabled must be of type bool", "snapshot_sharing_enabled");
            }
            if (parameters.TryGetValue("user_requests_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_requests_enabled is null, which cannot be sent" : "Bad parameter: user_requests_enabled must be of type bool", "user_requests_enabled");
            }
            if (parameters.TryGetValue("user_requests_notify_admins", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_requests_notify_admins is null, which cannot be sent" : "Bad parameter: user_requests_notify_admins must be of type bool", "user_requests_notify_admins");
            }
            if (parameters.TryGetValue("dav_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dav_enabled is null, which cannot be sent" : "Bad parameter: dav_enabled must be of type bool", "dav_enabled");
            }
            if (parameters.TryGetValue("files_com_remote_server_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_com_remote_server_enabled is null, which cannot be sent" : "Bad parameter: files_com_remote_server_enabled must be of type bool", "files_com_remote_server_enabled");
            }
            if (parameters.TryGetValue("ftp_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ftp_enabled is null, which cannot be sent" : "Bad parameter: ftp_enabled must be of type bool", "ftp_enabled");
            }
            if (parameters.TryGetValue("s3_compatible_endpoint_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_endpoint_enabled is null, which cannot be sent" : "Bad parameter: s3_compatible_endpoint_enabled must be of type bool", "s3_compatible_endpoint_enabled");
            }
            if (parameters.TryGetValue("sftp_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_enabled is null, which cannot be sent" : "Bad parameter: sftp_enabled must be of type bool", "sftp_enabled");
            }
            if (parameters.TryGetValue("sftp_finalize_partial_uploads", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_finalize_partial_uploads is null, which cannot be sent" : "Bad parameter: sftp_finalize_partial_uploads must be of type bool", "sftp_finalize_partial_uploads");
            }
            if (parameters.TryGetValue("users_can_create_api_keys", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: users_can_create_api_keys is null, which cannot be sent" : "Bad parameter: users_can_create_api_keys must be of type bool", "users_can_create_api_keys");
            }
            if (parameters.TryGetValue("users_can_create_ssh_keys", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: users_can_create_ssh_keys is null, which cannot be sent" : "Bad parameter: users_can_create_ssh_keys must be of type bool", "users_can_create_ssh_keys");
            }
            if (parameters.TryGetValue("show_user_notifications_log_in_link", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: show_user_notifications_log_in_link is null, which cannot be sent" : "Bad parameter: show_user_notifications_log_in_link must be of type bool", "show_user_notifications_log_in_link");
            }
            if (parameters.TryGetValue("sftp_host_key_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_host_key_type is null, which cannot be sent" : "Bad parameter: sftp_host_key_type must be of type string", "sftp_host_key_type");
            }
            if (parameters.TryGetValue("active_sftp_host_key_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: active_sftp_host_key_id is null, which cannot be sent" : "Bad parameter: active_sftp_host_key_id must be of type Nullable<Int64>", "active_sftp_host_key_id");
            }
            if (parameters.TryGetValue("active_sftp_host_key_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: active_sftp_host_key_ids is null, which cannot be sent" : "Bad parameter: active_sftp_host_key_ids must be of type Nullable<Int64>[]", "active_sftp_host_key_ids");
            }
            if (parameters.TryGetValue("protocol_access_groups_only", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: protocol_access_groups_only is null, which cannot be sent" : "Bad parameter: protocol_access_groups_only must be of type bool", "protocol_access_groups_only");
            }
            if (parameters.TryGetValue("revoke_bundle_access_on_disable_or_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: revoke_bundle_access_on_disable_or_delete is null, which cannot be sent" : "Bad parameter: revoke_bundle_access_on_disable_or_delete must be of type bool", "revoke_bundle_access_on_disable_or_delete");
            }
            if (parameters.TryGetValue("group_admins_can_add_users", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_admins_can_add_users is null, which cannot be sent" : "Bad parameter: group_admins_can_add_users must be of type bool", "group_admins_can_add_users");
            }
            if (parameters.TryGetValue("group_admins_can_manage_group_memberships", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_admins_can_manage_group_memberships is null, which cannot be sent" : "Bad parameter: group_admins_can_manage_group_memberships must be of type bool", "group_admins_can_manage_group_memberships");
            }
            if (parameters.TryGetValue("group_admins_can_delete_users", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_admins_can_delete_users is null, which cannot be sent" : "Bad parameter: group_admins_can_delete_users must be of type bool", "group_admins_can_delete_users");
            }
            if (parameters.TryGetValue("group_admins_can_enable_disable_users", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_admins_can_enable_disable_users is null, which cannot be sent" : "Bad parameter: group_admins_can_enable_disable_users must be of type bool", "group_admins_can_enable_disable_users");
            }
            if (parameters.TryGetValue("group_admins_can_modify_users", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_admins_can_modify_users is null, which cannot be sent" : "Bad parameter: group_admins_can_modify_users must be of type bool", "group_admins_can_modify_users");
            }
            if (parameters.TryGetValue("group_admins_can_bypass_user_lifecycle_rules", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_admins_can_bypass_user_lifecycle_rules is null, which cannot be sent" : "Bad parameter: group_admins_can_bypass_user_lifecycle_rules must be of type bool", "group_admins_can_bypass_user_lifecycle_rules");
            }
            if (parameters.TryGetValue("group_admins_can_reset_passwords", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_admins_can_reset_passwords is null, which cannot be sent" : "Bad parameter: group_admins_can_reset_passwords must be of type bool", "group_admins_can_reset_passwords");
            }
            if (parameters.TryGetValue("group_admins_can_set_user_password", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_admins_can_set_user_password is null, which cannot be sent" : "Bad parameter: group_admins_can_set_user_password must be of type bool", "group_admins_can_set_user_password");
            }
            if (parameters.TryGetValue("child_site_folder_admins_can_see_parent_site_directory", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: child_site_folder_admins_can_see_parent_site_directory is null, which cannot be sent" : "Bad parameter: child_site_folder_admins_can_see_parent_site_directory must be of type bool", "child_site_folder_admins_can_see_parent_site_directory");
            }
            if (parameters.TryGetValue("bundle_recipient_blacklist_free_email_domains", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_recipient_blacklist_free_email_domains is null, which cannot be sent" : "Bad parameter: bundle_recipient_blacklist_free_email_domains must be of type bool", "bundle_recipient_blacklist_free_email_domains");
            }
            if (parameters.TryGetValue("bundle_recipient_blacklist_domains", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_recipient_blacklist_domains is null, which cannot be sent" : "Bad parameter: bundle_recipient_blacklist_domains must be of type string[]", "bundle_recipient_blacklist_domains");
            }
            if (parameters.TryGetValue("admins_bypass_locked_subfolders", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: admins_bypass_locked_subfolders is null, which cannot be sent" : "Bad parameter: admins_bypass_locked_subfolders must be of type bool", "admins_bypass_locked_subfolders");
            }
            if (parameters.TryGetValue("allowed_2fa_method_sms", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_2fa_method_sms is null, which cannot be sent" : "Bad parameter: allowed_2fa_method_sms must be of type bool", "allowed_2fa_method_sms");
            }
            if (parameters.TryGetValue("allowed_2fa_method_totp", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_2fa_method_totp is null, which cannot be sent" : "Bad parameter: allowed_2fa_method_totp must be of type bool", "allowed_2fa_method_totp");
            }
            if (parameters.TryGetValue("allowed_2fa_method_webauthn", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_2fa_method_webauthn is null, which cannot be sent" : "Bad parameter: allowed_2fa_method_webauthn must be of type bool", "allowed_2fa_method_webauthn");
            }
            if (parameters.TryGetValue("allowed_2fa_method_yubi", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_2fa_method_yubi is null, which cannot be sent" : "Bad parameter: allowed_2fa_method_yubi must be of type bool", "allowed_2fa_method_yubi");
            }
            if (parameters.TryGetValue("allowed_2fa_method_email", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_2fa_method_email is null, which cannot be sent" : "Bad parameter: allowed_2fa_method_email must be of type bool", "allowed_2fa_method_email");
            }
            if (parameters.TryGetValue("allowed_2fa_method_static", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_2fa_method_static is null, which cannot be sent" : "Bad parameter: allowed_2fa_method_static must be of type bool", "allowed_2fa_method_static");
            }
            if (parameters.TryGetValue("allowed_2fa_method_bypass_for_ftp_sftp_dav", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_2fa_method_bypass_for_ftp_sftp_dav is null, which cannot be sent" : "Bad parameter: allowed_2fa_method_bypass_for_ftp_sftp_dav must be of type bool", "allowed_2fa_method_bypass_for_ftp_sftp_dav");
            }
            if (parameters.TryGetValue("require_2fa", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_2fa is null, which cannot be sent" : "Bad parameter: require_2fa must be of type bool", "require_2fa");
            }
            if (parameters.TryGetValue("require_2fa_exempt_all_sso_users", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_2fa_exempt_all_sso_users is null, which cannot be sent" : "Bad parameter: require_2fa_exempt_all_sso_users must be of type bool", "require_2fa_exempt_all_sso_users");
            }
            if (parameters.TryGetValue("require_2fa_user_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_2fa_user_type is null, which cannot be sent" : "Bad parameter: require_2fa_user_type must be of type string", "require_2fa_user_type");
            }
            if (parameters.TryGetValue("color2_top", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: color2_top is null, which cannot be sent" : "Bad parameter: color2_top must be of type string", "color2_top");
            }
            if (parameters.TryGetValue("color2_left", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: color2_left is null, which cannot be sent" : "Bad parameter: color2_left must be of type string", "color2_left");
            }
            if (parameters.TryGetValue("color2_link", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: color2_link is null, which cannot be sent" : "Bad parameter: color2_link must be of type string", "color2_link");
            }
            if (parameters.TryGetValue("color2_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: color2_text is null, which cannot be sent" : "Bad parameter: color2_text must be of type string", "color2_text");
            }
            if (parameters.TryGetValue("color2_top_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: color2_top_text is null, which cannot be sent" : "Bad parameter: color2_top_text must be of type string", "color2_top_text");
            }
            if (parameters.TryGetValue("site_header", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: site_header is null, which cannot be sent" : "Bad parameter: site_header must be of type string", "site_header");
            }
            if (parameters.TryGetValue("site_footer", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: site_footer is null, which cannot be sent" : "Bad parameter: site_footer must be of type string", "site_footer");
            }
            if (parameters.TryGetValue("site_public_header", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: site_public_header is null, which cannot be sent" : "Bad parameter: site_public_header must be of type string", "site_public_header");
            }
            if (parameters.TryGetValue("site_public_footer", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: site_public_footer is null, which cannot be sent" : "Bad parameter: site_public_footer must be of type string", "site_public_footer");
            }
            if (parameters.TryGetValue("login_help_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: login_help_text is null, which cannot be sent" : "Bad parameter: login_help_text must be of type string", "login_help_text");
            }
            if (parameters.TryGetValue("use_dedicated_ips_for_smtp", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: use_dedicated_ips_for_smtp is null, which cannot be sent" : "Bad parameter: use_dedicated_ips_for_smtp must be of type bool", "use_dedicated_ips_for_smtp");
            }
            if (parameters.TryGetValue("email_footer_custom_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email_footer_custom_text is null, which cannot be sent" : "Bad parameter: email_footer_custom_text must be of type string", "email_footer_custom_text");
            }
            if (parameters.TryGetValue("smtp_address", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: smtp_address is null, which cannot be sent" : "Bad parameter: smtp_address must be of type string", "smtp_address");
            }
            if (parameters.TryGetValue("smtp_authentication", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: smtp_authentication is null, which cannot be sent" : "Bad parameter: smtp_authentication must be of type string", "smtp_authentication");
            }
            if (parameters.TryGetValue("smtp_from", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: smtp_from is null, which cannot be sent" : "Bad parameter: smtp_from must be of type string", "smtp_from");
            }
            if (parameters.TryGetValue("smtp_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: smtp_username is null, which cannot be sent" : "Bad parameter: smtp_username must be of type string", "smtp_username");
            }
            if (parameters.TryGetValue("smtp_port", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: smtp_port is null, which cannot be sent" : "Bad parameter: smtp_port must be of type Nullable<Int64>", "smtp_port");
            }
            if (parameters.TryGetValue("smtp_ssl", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: smtp_ssl is null, which cannot be sent" : "Bad parameter: smtp_ssl must be of type string", "smtp_ssl");
            }
            if (parameters.TryGetValue("ldap_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_enabled is null, which cannot be sent" : "Bad parameter: ldap_enabled must be of type bool", "ldap_enabled");
            }
            if (parameters.TryGetValue("ldap_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_type is null, which cannot be sent" : "Bad parameter: ldap_type must be of type string", "ldap_type");
            }
            if (parameters.TryGetValue("ldap_host", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_host is null, which cannot be sent" : "Bad parameter: ldap_host must be of type string", "ldap_host");
            }
            if (parameters.TryGetValue("ldap_host_2", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_host_2 is null, which cannot be sent" : "Bad parameter: ldap_host_2 must be of type string", "ldap_host_2");
            }
            if (parameters.TryGetValue("ldap_host_3", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_host_3 is null, which cannot be sent" : "Bad parameter: ldap_host_3 must be of type string", "ldap_host_3");
            }
            if (parameters.TryGetValue("ldap_port", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_port is null, which cannot be sent" : "Bad parameter: ldap_port must be of type Nullable<Int64>", "ldap_port");
            }
            if (parameters.TryGetValue("ldap_secure", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_secure is null, which cannot be sent" : "Bad parameter: ldap_secure must be of type bool", "ldap_secure");
            }
            if (parameters.TryGetValue("ldap_server_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_server_certificate is null, which cannot be sent" : "Bad parameter: ldap_server_certificate must be of type string", "ldap_server_certificate");
            }
            if (parameters.TryGetValue("ldap_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_username is null, which cannot be sent" : "Bad parameter: ldap_username must be of type string", "ldap_username");
            }
            if (parameters.TryGetValue("ldap_username_field", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_username_field is null, which cannot be sent" : "Bad parameter: ldap_username_field must be of type string", "ldap_username_field");
            }
            if (parameters.TryGetValue("ldap_domain", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_domain is null, which cannot be sent" : "Bad parameter: ldap_domain must be of type string", "ldap_domain");
            }
            if (parameters.TryGetValue("ldap_user_action", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_user_action is null, which cannot be sent" : "Bad parameter: ldap_user_action must be of type string", "ldap_user_action");
            }
            if (parameters.TryGetValue("ldap_group_action", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_group_action is null, which cannot be sent" : "Bad parameter: ldap_group_action must be of type string", "ldap_group_action");
            }
            if (parameters.TryGetValue("ldap_user_include_groups", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_user_include_groups is null, which cannot be sent" : "Bad parameter: ldap_user_include_groups must be of type string", "ldap_user_include_groups");
            }
            if (parameters.TryGetValue("ldap_group_exclusion", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_group_exclusion is null, which cannot be sent" : "Bad parameter: ldap_group_exclusion must be of type string", "ldap_group_exclusion");
            }
            if (parameters.TryGetValue("ldap_group_inclusion", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_group_inclusion is null, which cannot be sent" : "Bad parameter: ldap_group_inclusion must be of type string", "ldap_group_inclusion");
            }
            if (parameters.TryGetValue("ldap_base_dn", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_base_dn is null, which cannot be sent" : "Bad parameter: ldap_base_dn must be of type string", "ldap_base_dn");
            }
            if (parameters.TryGetValue("uploads_via_email_authentication", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: uploads_via_email_authentication is null, which cannot be sent" : "Bad parameter: uploads_via_email_authentication must be of type bool", "uploads_via_email_authentication");
            }
            if (parameters.TryGetValue("bundle_watermark_value", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_watermark_value is null, which cannot be sent" : "Bad parameter: bundle_watermark_value must be of type object", "bundle_watermark_value");
            }
            if (parameters.TryGetValue("icon16_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: icon16_file is null, which cannot be sent" : "Bad parameter: icon16_file must be of type FileAttachment or IDictionary<string, object>", "icon16_file");
            }
            if (parameters.TryGetValue("icon16_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: icon16_delete is null, which cannot be sent" : "Bad parameter: icon16_delete must be of type bool", "icon16_delete");
            }
            if (parameters.TryGetValue("icon32_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: icon32_file is null, which cannot be sent" : "Bad parameter: icon32_file must be of type FileAttachment or IDictionary<string, object>", "icon32_file");
            }
            if (parameters.TryGetValue("icon32_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: icon32_delete is null, which cannot be sent" : "Bad parameter: icon32_delete must be of type bool", "icon32_delete");
            }
            if (parameters.TryGetValue("icon48_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: icon48_file is null, which cannot be sent" : "Bad parameter: icon48_file must be of type FileAttachment or IDictionary<string, object>", "icon48_file");
            }
            if (parameters.TryGetValue("icon48_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: icon48_delete is null, which cannot be sent" : "Bad parameter: icon48_delete must be of type bool", "icon48_delete");
            }
            if (parameters.TryGetValue("icon128_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: icon128_file is null, which cannot be sent" : "Bad parameter: icon128_file must be of type FileAttachment or IDictionary<string, object>", "icon128_file");
            }
            if (parameters.TryGetValue("icon128_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: icon128_delete is null, which cannot be sent" : "Bad parameter: icon128_delete must be of type bool", "icon128_delete");
            }
            if (parameters.TryGetValue("logo_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: logo_file is null, which cannot be sent" : "Bad parameter: logo_file must be of type FileAttachment or IDictionary<string, object>", "logo_file");
            }
            if (parameters.TryGetValue("logo_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: logo_delete is null, which cannot be sent" : "Bad parameter: logo_delete must be of type bool", "logo_delete");
            }
            if (parameters.TryGetValue("bundle_watermark_attachment_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_watermark_attachment_file is null, which cannot be sent" : "Bad parameter: bundle_watermark_attachment_file must be of type FileAttachment or IDictionary<string, object>", "bundle_watermark_attachment_file");
            }
            if (parameters.TryGetValue("bundle_watermark_attachment_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bundle_watermark_attachment_delete is null, which cannot be sent" : "Bad parameter: bundle_watermark_attachment_delete must be of type bool", "bundle_watermark_attachment_delete");
            }
            if (parameters.TryGetValue("login_page_background_image_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: login_page_background_image_file is null, which cannot be sent" : "Bad parameter: login_page_background_image_file must be of type FileAttachment or IDictionary<string, object>", "login_page_background_image_file");
            }
            if (parameters.TryGetValue("login_page_background_image_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: login_page_background_image_delete is null, which cannot be sent" : "Bad parameter: login_page_background_image_delete must be of type bool", "login_page_background_image_delete");
            }
            if (parameters.TryGetValue("disable_2fa_with_delay", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disable_2fa_with_delay is null, which cannot be sent" : "Bad parameter: disable_2fa_with_delay must be of type bool", "disable_2fa_with_delay");
            }
            if (parameters.TryGetValue("ldap_password_change", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_password_change is null, which cannot be sent" : "Bad parameter: ldap_password_change must be of type string", "ldap_password_change");
            }
            if (parameters.TryGetValue("ldap_password_change_confirmation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ldap_password_change_confirmation is null, which cannot be sent" : "Bad parameter: ldap_password_change_confirmation must be of type string", "ldap_password_change_confirmation");
            }
            if (parameters.TryGetValue("redirect_old_subdomain", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: redirect_old_subdomain is null, which cannot be sent" : "Bad parameter: redirect_old_subdomain must be of type bool", "redirect_old_subdomain");
            }
            if (parameters.TryGetValue("smtp_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: smtp_password is null, which cannot be sent" : "Bad parameter: smtp_password must be of type string", "smtp_password");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/site", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Site>(responseJson, context, options);
        }

    }
}