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
    /// User operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Users"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="User"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class UserOperations
    {
        private readonly FilesClient client;

        internal UserOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a User that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public User New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            User model = new User(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id`, `company`, `name`, `disabled`, `authenticate_until`, `username`, `email`, `site_admin`, `last_desktop_login_at`, `last_login_at`, `password_validity_days` or `ssl_required`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `username`, `name`, `email`, `company`, `site_admin`, `password_validity_days`, `ssl_required`, `last_login_at`, `authenticate_until`, `not_site_admin`, `disabled`, `partner_id`, `primary_group_id` or `workspace_id`. Valid field combinations are `[ site_admin, username ]`, `[ not_site_admin, username ]`, `[ workspace_id, username ]`, `[ company, name ]`, `[ workspace_id, name ]`, `[ workspace_id, email ]`, `[ workspace_id, company ]`, `[ workspace_id, site_admin ]`, `[ workspace_id, not_site_admin ]`, `[ workspace_id, disabled ]`, `[ workspace_id, partner_id ]`, `[ workspace_id, site_admin, username ]`, `[ workspace_id, not_site_admin, username ]`, `[ workspace_id, disabled, username ]`, `[ workspace_id, partner_id, username ]` or `[ workspace_id, company, name ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `username`, `name`, `email` or `company`. Valid field combinations are `[ company, name ]`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   ids - string - comma-separated list of User IDs
        ///   include_parent_site_users - boolean - Include users from the parent site.
        ///   search - string - Searches for partial matches of name, username, or email.
        /// </summary>
        public FilesList<User> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return User.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id`, `company`, `name`, `disabled`, `authenticate_until`, `username`, `email`, `site_admin`, `last_desktop_login_at`, `last_login_at`, `password_validity_days` or `ssl_required`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `username`, `name`, `email`, `company`, `site_admin`, `password_validity_days`, `ssl_required`, `last_login_at`, `authenticate_until`, `not_site_admin`, `disabled`, `partner_id`, `primary_group_id` or `workspace_id`. Valid field combinations are `[ site_admin, username ]`, `[ not_site_admin, username ]`, `[ workspace_id, username ]`, `[ company, name ]`, `[ workspace_id, name ]`, `[ workspace_id, email ]`, `[ workspace_id, company ]`, `[ workspace_id, site_admin ]`, `[ workspace_id, not_site_admin ]`, `[ workspace_id, disabled ]`, `[ workspace_id, partner_id ]`, `[ workspace_id, site_admin, username ]`, `[ workspace_id, not_site_admin, username ]`, `[ workspace_id, disabled, username ]`, `[ workspace_id, partner_id, username ]` or `[ workspace_id, company, name ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `username`, `name`, `email` or `company`. Valid field combinations are `[ company, name ]`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   ids - string - comma-separated list of User IDs
        ///   include_parent_site_users - boolean - Include users from the parent site.
        ///   search - string - Searches for partial matches of name, username, or email.
        /// </summary>
        public FilesList<User> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return User.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - User ID.
        /// </summary>
        public Task<User> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - User ID.
        /// </summary>
        public Task<User> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the User Root upon user creation. Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
        ///   group_id - int64 - Group ID to associate this user with.
        ///   group_ids - string - A list of group ids to associate this user with.  Comma delimited.
        ///   imported_password_hash - string - Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash methods are MD5, SHA1, and SHA256.
        ///   password - string - User password.
        ///   password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `password`.
        ///   announcements_read - boolean - Signifies that the user has read all the announcements in the UI.
        ///   ai_assistant_personality_id - int64 - AI Assistant Personality ID assigned directly to this user, if any.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   attachments_permission - boolean - DEPRECATED: If `true`, the user can user create Bundles (aka Share Links). Use the bundle permission instead.
        ///   authenticate_until - string - Scheduled Date/Time at which user will be deactivated
        ///   authentication_method - string - How is this user authenticated?
        ///   billing_permission - boolean - Allow this user to perform operations on the account, payments, and invoices?
        ///   bypass_user_lifecycle_rules - boolean - Exempt this user from user lifecycle rules?
        ///   bypass_site_allowed_ips - boolean - Allow this user to skip site-wide IP blacklists?
        ///   dav_permission - boolean - Can the user connect with WebDAV?
        ///   desktop_configuration_profile_id - int64 - Desktop Configuration Profile ID assigned directly to this user, if any.
        ///   default_workspace_id - int64 - Workspace ID the user should land in by default when more than one Workspace is available.
        ///   disabled - boolean - Is user disabled? Disabled users cannot log in, and do not count for billing purposes. Users can be automatically disabled after an inactivity period via a Site setting or schedule to be deactivated after specific date.
        ///   filesystem_layout - string - File system layout
        ///   ftp_permission - boolean - Can the user access with FTP/FTPS?
        ///   header_text - string - Text to display to the user in the header of the UI
        ///   integration_centric_profile_id - int64 - Integration Centric Profile ID assigned directly to this user, if any.
        ///   language - string - Preferred language
        ///   notification_daily_send_time - int64 - Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
        ///   name - string - User's full name
        ///   company - string - User's company
        ///   notes - string - Any internal notes on the user
        ///   office_integration_enabled - boolean - Enable integration with Office for the web?
        ///   partner_admin - boolean - Is this user a Partner administrator?
        ///   partner_id - int64 - Partner ID if this user belongs to a Partner
        ///   password_validity_days - int64 - Number of days to allow user to use the same password
        ///   primary_group_id - int64 - Primary group ID for Group Admin scoping
        ///   readonly_site_admin - boolean - Is the user an allowed to view all (non-billing) site configuration for this site?
        ///   receive_admin_alerts - boolean - Deprecated. Use notify_on_all_site_warnings and granular failure notification preferences instead.
        ///   notify_on_all_site_warnings - boolean - Should the user receive site warnings via email?
        ///   notify_on_all_sso_failures - boolean - Should the user receive sso/scim/ldap configuration/sync failures via email?
        ///   notify_on_all_user_security_events - boolean - Should the user receive user security events via email?
        ///   notify_on_all_pending_work_failures - boolean - Should the user receive pending work failures via email?
        ///   notify_on_all_siem_http_destination_failures - boolean - Should the user receive siem failures via email?
        ///   notify_on_all_sync_failures - boolean - Should the user receive sync failures via email?
        ///   notify_on_all_automation_failures - boolean - Should the user receive automation failures via email?
        ///   notify_on_all_expectation_failures - boolean - Should the user receive expectation failures and misses via email?
        ///   require_login_by - string - Require user to login by specified date otherwise it will be disabled.
        ///   require_password_change - boolean - Is a password change required upon next user login?
        ///   responsible_group_id - int64 - ID of the internal Group responsible for this Partner User, overriding the Partner default.
        ///   responsible_user_id - int64 - ID of the internal User responsible for this Partner User, overriding the Partner default.
        ///   restapi_permission - boolean - Can this user access the Web app, Desktop app, SDKs, or REST API?  (All of these tools use the API internally, so this is one unified permission set.)
        ///   s3_compatible_endpoint_permission - boolean - Can the user access the S3-compatible endpoint? Defaults to true.
        ///   self_managed - boolean - Does this user manage it's own credentials or is it a shared/bot user?
        ///   sftp_permission - boolean - Can the user access with SFTP?
        ///   site_admin - boolean - Is the user an administrator for this site?
        ///   skip_welcome_screen - boolean - Skip Welcome page in the UI?
        ///   ssl_required - string - SSL required setting
        ///   sso_strategy_id - int64 - SSO (Single Sign On) strategy ID for the user, if applicable.
        ///   subscribe_to_newsletter - boolean - Is the user subscribed to the newsletter?
        ///   require_2fa - string - 2FA required setting. `use_system_setting` uses the site-wide setting, including SSO exemptions. `always_require` and `never_require` override the site-wide setting when user-level overrides are allowed.
        ///   tags - string - Comma-separated list of Tags for this user. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        ///   time_zone - string - User time zone
        ///   user_root - string - If filesystem layout is user_root, this path is the root path the user is fixed to for all interfaces. If the filesystem layout is site_root or partner_root, this acts as a root folder only for FTP and SFTP (SFTP applicability also requires a site-wide setting to be set). For partner_root layout, this path is relative to the Partner root folder for all callers and blank opts out of an additional protocol root. In this situation, this path is not applied to the API, Desktop, or Web interface.
        ///   user_home - string - Home folder for FTP/SFTP. For users with the partner_root filesystem layout, this path is relative to the Partner root folder. In all other cases, it is an absolute path. Only applies to FTP and SFTP, and not any other interface.
        ///   workspace_admin - boolean - Whether the user is an administrator of their own Custom Workspace. Does not reflect administration granted through Permissions.
        ///   username (required) - string - User's username
        ///   workspace_id - int64 - ID of the Workspace the user belongs to. 0 is the Default Workspace.
        /// </summary>
        public Task<User> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Unlock user who has been locked out due to failed logins
        /// </summary>
        public Task UnlockAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.UnlockCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Resend user welcome email
        /// </summary>
        public Task ResendWelcomeEmailAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.ResendWelcomeEmailCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Trigger 2FA Reset process for user who has lost access to their existing 2FA methods
        /// </summary>
        public Task User2faResetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.User2faResetCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the User Root upon user creation. Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
        ///   group_id - int64 - Group ID to associate this user with.
        ///   group_ids - string - A list of group ids to associate this user with.  Comma delimited.
        ///   imported_password_hash - string - Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash methods are MD5, SHA1, and SHA256.
        ///   password - string - User password.
        ///   password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `password`.
        ///   announcements_read - boolean - Signifies that the user has read all the announcements in the UI.
        ///   ai_assistant_personality_id - int64 - AI Assistant Personality ID assigned directly to this user, if any.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   attachments_permission - boolean - DEPRECATED: If `true`, the user can user create Bundles (aka Share Links). Use the bundle permission instead.
        ///   authenticate_until - string - Scheduled Date/Time at which user will be deactivated
        ///   authentication_method - string - How is this user authenticated?
        ///   billing_permission - boolean - Allow this user to perform operations on the account, payments, and invoices?
        ///   bypass_user_lifecycle_rules - boolean - Exempt this user from user lifecycle rules?
        ///   bypass_site_allowed_ips - boolean - Allow this user to skip site-wide IP blacklists?
        ///   dav_permission - boolean - Can the user connect with WebDAV?
        ///   desktop_configuration_profile_id - int64 - Desktop Configuration Profile ID assigned directly to this user, if any.
        ///   default_workspace_id - int64 - Workspace ID the user should land in by default when more than one Workspace is available.
        ///   disabled - boolean - Is user disabled? Disabled users cannot log in, and do not count for billing purposes. Users can be automatically disabled after an inactivity period via a Site setting or schedule to be deactivated after specific date.
        ///   filesystem_layout - string - File system layout
        ///   ftp_permission - boolean - Can the user access with FTP/FTPS?
        ///   header_text - string - Text to display to the user in the header of the UI
        ///   integration_centric_profile_id - int64 - Integration Centric Profile ID assigned directly to this user, if any.
        ///   language - string - Preferred language
        ///   notification_daily_send_time - int64 - Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
        ///   name - string - User's full name
        ///   company - string - User's company
        ///   notes - string - Any internal notes on the user
        ///   office_integration_enabled - boolean - Enable integration with Office for the web?
        ///   partner_admin - boolean - Is this user a Partner administrator?
        ///   partner_id - int64 - Partner ID if this user belongs to a Partner
        ///   password_validity_days - int64 - Number of days to allow user to use the same password
        ///   primary_group_id - int64 - Primary group ID for Group Admin scoping
        ///   readonly_site_admin - boolean - Is the user an allowed to view all (non-billing) site configuration for this site?
        ///   receive_admin_alerts - boolean - Deprecated. Use notify_on_all_site_warnings and granular failure notification preferences instead.
        ///   notify_on_all_site_warnings - boolean - Should the user receive site warnings via email?
        ///   notify_on_all_sso_failures - boolean - Should the user receive sso/scim/ldap configuration/sync failures via email?
        ///   notify_on_all_user_security_events - boolean - Should the user receive user security events via email?
        ///   notify_on_all_pending_work_failures - boolean - Should the user receive pending work failures via email?
        ///   notify_on_all_siem_http_destination_failures - boolean - Should the user receive siem failures via email?
        ///   notify_on_all_sync_failures - boolean - Should the user receive sync failures via email?
        ///   notify_on_all_automation_failures - boolean - Should the user receive automation failures via email?
        ///   notify_on_all_expectation_failures - boolean - Should the user receive expectation failures and misses via email?
        ///   require_login_by - string - Require user to login by specified date otherwise it will be disabled.
        ///   require_password_change - boolean - Is a password change required upon next user login?
        ///   responsible_group_id - int64 - ID of the internal Group responsible for this Partner User, overriding the Partner default.
        ///   responsible_user_id - int64 - ID of the internal User responsible for this Partner User, overriding the Partner default.
        ///   restapi_permission - boolean - Can this user access the Web app, Desktop app, SDKs, or REST API?  (All of these tools use the API internally, so this is one unified permission set.)
        ///   s3_compatible_endpoint_permission - boolean - Can the user access the S3-compatible endpoint? Defaults to true.
        ///   self_managed - boolean - Does this user manage it's own credentials or is it a shared/bot user?
        ///   sftp_permission - boolean - Can the user access with SFTP?
        ///   site_admin - boolean - Is the user an administrator for this site?
        ///   skip_welcome_screen - boolean - Skip Welcome page in the UI?
        ///   ssl_required - string - SSL required setting
        ///   sso_strategy_id - int64 - SSO (Single Sign On) strategy ID for the user, if applicable.
        ///   subscribe_to_newsletter - boolean - Is the user subscribed to the newsletter?
        ///   require_2fa - string - 2FA required setting. `use_system_setting` uses the site-wide setting, including SSO exemptions. `always_require` and `never_require` override the site-wide setting when user-level overrides are allowed.
        ///   tags - string - Comma-separated list of Tags for this user. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        ///   time_zone - string - User time zone
        ///   user_root - string - If filesystem layout is user_root, this path is the root path the user is fixed to for all interfaces. If the filesystem layout is site_root or partner_root, this acts as a root folder only for FTP and SFTP (SFTP applicability also requires a site-wide setting to be set). For partner_root layout, this path is relative to the Partner root folder for all callers and blank opts out of an additional protocol root. In this situation, this path is not applied to the API, Desktop, or Web interface.
        ///   user_home - string - Home folder for FTP/SFTP. For users with the partner_root filesystem layout, this path is relative to the Partner root folder. In all other cases, it is an absolute path. Only applies to FTP and SFTP, and not any other interface.
        ///   workspace_admin - boolean - Whether the user is an administrator of their own Custom Workspace. Does not reflect administration granted through Permissions.
        ///   username - string - User's username
        ///   workspace_id - int64 - Workspace ID. Only Site Administrators can change this field. Values supplied by Workspace Administrators, Group Administrators, or other non-Site Administrators using `/user` are ignored.
        ///   clear_2fa - boolean - If true when changing authentication_method from `password` to `sso`, remove all two-factor methods. Ignored in all other cases.
        ///   convert_to_partner_user - boolean - Required when assigning a Partner to an existing non-Partner user. If true, convert the user by assigning the partner_id provided.
        /// </summary>
        public Task<User> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   new_owner_id - int64 - Provide a User ID here to transfer ownership of certain resources such as Automations and Share Links (Bundles) to that new user.
        /// </summary>
        public Task DeleteAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   new_owner_id - int64 - Provide a User ID here to transfer ownership of certain resources such as Automations and Share Links (Bundles) to that new user.
        /// </summary>
        public Task DestroyAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return User.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}