using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Operations
{
    // Imported inside the namespace so that model names, such as Action, take precedence over System's.
    using FilesCom.Models;

    /// <summary>
    /// Site operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Sites"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="Site"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class SiteOperations
    {
        private readonly FilesClient client;

        internal SiteOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// </summary>
        public Task<Site> GetAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Site.GetCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// </summary>
        public Task<UsageSnapshot> GetUsageAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Site.GetUsageCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
        ///   ai_feature_availability - object - Availability settings for AI features by user class
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
        ///   ftp_enabled - boolean - Is FTP enabled?
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
        ///   uploads_via_email_authentication - boolean - Do incoming emails in the Inboxes require checking for SPF/DKIM/DMARC?
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
        public Task<Site> UpdateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Site.UpdateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}