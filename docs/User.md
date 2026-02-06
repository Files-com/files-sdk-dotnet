# FilesCom.Models.User

## Example User Object

```
{
  "id": 1,
  "username": "user",
  "admin_group_ids": [
    1
  ],
  "allowed_ips": "10.0.0.0/8\n127.0.0.1",
  "attachments_permission": true,
  "api_keys_count": 1,
  "authenticate_until": "2000-01-01T01:00:00Z",
  "authentication_method": "password",
  "avatar_url": "example",
  "billable": true,
  "billing_permission": true,
  "bypass_site_allowed_ips": true,
  "bypass_user_lifecycle_rules": true,
  "created_at": "2000-01-01T01:00:00Z",
  "dav_permission": true,
  "disabled": true,
  "disabled_expired_or_inactive": true,
  "email": "john.doe@files.com",
  "filesystem_layout": "site_root",
  "first_login_at": "2000-01-01T01:00:00Z",
  "ftp_permission": true,
  "group_ids": "example",
  "header_text": "User-specific message.",
  "language": "en",
  "last_login_at": "2000-01-01T01:00:00Z",
  "last_web_login_at": "2000-01-01T01:00:00Z",
  "last_ftp_login_at": "2000-01-01T01:00:00Z",
  "last_sftp_login_at": "2000-01-01T01:00:00Z",
  "last_dav_login_at": "2000-01-01T01:00:00Z",
  "last_desktop_login_at": "2000-01-01T01:00:00Z",
  "last_restapi_login_at": "2000-01-01T01:00:00Z",
  "last_api_use_at": "2000-01-01T01:00:00Z",
  "last_active_at": "2000-01-01T01:00:00Z",
  "last_protocol_cipher": "example",
  "lockout_expires": "2000-01-01T01:00:00Z",
  "name": "John Doe",
  "company": "ACME Corp.",
  "notes": "Internal notes on this user.",
  "notification_daily_send_time": 18,
  "office_integration_enabled": true,
  "partner_admin": true,
  "partner_id": 1,
  "partner_name": "example",
  "password_set_at": "2000-01-01T01:00:00Z",
  "password_validity_days": 1,
  "public_keys_count": 1,
  "receive_admin_alerts": true,
  "require_2fa": "always_require",
  "require_login_by": "2000-01-01T01:00:00Z",
  "active_2fa": true,
  "require_password_change": true,
  "password_expired": true,
  "readonly_site_admin": true,
  "restapi_permission": true,
  "self_managed": true,
  "sftp_permission": true,
  "site_admin": true,
  "workspace_admin": true,
  "site_id": 1,
  "workspace_id": 1,
  "skip_welcome_screen": true,
  "ssl_required": "always_require",
  "sso_strategy_id": 1,
  "subscribe_to_newsletter": true,
  "externally_managed": true,
  "tags": "example",
  "time_zone": "Pacific Time (US & Canada)",
  "type_of_2fa": "yubi",
  "type_of_2fa_for_display": "yubi",
  "user_root": "example",
  "user_home": "example",
  "days_remaining_until_password_expire": 1,
  "password_expire_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): User ID
* `username` / `Username`  (string): User's username
* `admin_group_ids` / `AdminGroupIds`  (Nullable<Int64>[]): List of group IDs of which this user is an administrator
* `allowed_ips` / `AllowedIps`  (string): A list of allowed IPs if applicable.  Newline delimited
* `attachments_permission` / `AttachmentsPermission`  (bool): If `true`, the user can user create Bundles (aka Share Links). Use the bundle permission instead.
* `api_keys_count` / `ApiKeysCount`  (Nullable<Int64>): Number of API keys associated with this user
* `authenticate_until` / `AuthenticateUntil`  (Nullable<DateTime>): Scheduled Date/Time at which user will be deactivated
* `authentication_method` / `AuthenticationMethod`  (string): How is this user authenticated?
* `avatar_url` / `AvatarUrl`  (string): URL holding the user's avatar
* `billable` / `Billable`  (bool): Is this a billable user record?
* `billing_permission` / `BillingPermission`  (bool): Allow this user to perform operations on the account, payments, and invoices?
* `bypass_site_allowed_ips` / `BypassSiteAllowedIps`  (bool): Allow this user to skip site-wide IP blacklists?
* `bypass_user_lifecycle_rules` / `BypassUserLifecycleRules`  (bool): Exempt this user from user lifecycle rules?
* `created_at` / `CreatedAt`  (Nullable<DateTime>): When this user was created
* `dav_permission` / `DavPermission`  (bool): Can the user connect with WebDAV?
* `disabled` / `Disabled`  (bool): Is user disabled? Disabled users cannot log in, and do not count for billing purposes. Users can be automatically disabled after an inactivity period via a Site setting or schedule to be deactivated after specific date.
* `disabled_expired_or_inactive` / `DisabledExpiredOrInactive`  (bool): Computed property that returns true if user disabled or expired or inactive.
* `email` / `Email`  (string): User email address
* `filesystem_layout` / `FilesystemLayout`  (string): File system layout
* `first_login_at` / `FirstLoginAt`  (Nullable<DateTime>): User's first login time
* `ftp_permission` / `FtpPermission`  (bool): Can the user access with FTP/FTPS?
* `group_ids` / `GroupIds`  (string): Comma-separated list of group IDs of which this user is a member
* `header_text` / `HeaderText`  (string): Text to display to the user in the header of the UI
* `language` / `Language`  (string): Preferred language
* `last_login_at` / `LastLoginAt`  (Nullable<DateTime>): User's most recent login time via any protocol
* `last_web_login_at` / `LastWebLoginAt`  (Nullable<DateTime>): User's most recent login time via web
* `last_ftp_login_at` / `LastFtpLoginAt`  (Nullable<DateTime>): User's most recent login time via FTP
* `last_sftp_login_at` / `LastSftpLoginAt`  (Nullable<DateTime>): User's most recent login time via SFTP
* `last_dav_login_at` / `LastDavLoginAt`  (Nullable<DateTime>): User's most recent login time via WebDAV
* `last_desktop_login_at` / `LastDesktopLoginAt`  (Nullable<DateTime>): User's most recent login time via Desktop app
* `last_restapi_login_at` / `LastRestapiLoginAt`  (Nullable<DateTime>): User's most recent login time via Rest API
* `last_api_use_at` / `LastApiUseAt`  (Nullable<DateTime>): User's most recent API use time
* `last_active_at` / `LastActiveAt`  (Nullable<DateTime>): User's most recent activity time, which is the latest of most recent login, most recent API use, enablement, or creation
* `last_protocol_cipher` / `LastProtocolCipher`  (string): The most recent protocol and cipher used
* `lockout_expires` / `LockoutExpires`  (Nullable<DateTime>): Time in the future that the user will no longer be locked out if applicable
* `name` / `Name`  (string): User's full name
* `company` / `Company`  (string): User's company
* `notes` / `Notes`  (string): Any internal notes on the user
* `notification_daily_send_time` / `NotificationDailySendTime`  (Nullable<Int64>): Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
* `office_integration_enabled` / `OfficeIntegrationEnabled`  (bool): Enable integration with Office for the web?
* `partner_admin` / `PartnerAdmin`  (bool): Is this user a Partner administrator?
* `partner_id` / `PartnerId`  (Nullable<Int64>): Partner ID if this user belongs to a Partner
* `partner_name` / `PartnerName`  (string): Name of the Partner if this user belongs to a Partner
* `password_set_at` / `PasswordSetAt`  (Nullable<DateTime>): Last time the user's password was set
* `password_validity_days` / `PasswordValidityDays`  (Nullable<Int64>): Number of days to allow user to use the same password
* `public_keys_count` / `PublicKeysCount`  (Nullable<Int64>): Number of public keys associated with this user
* `receive_admin_alerts` / `ReceiveAdminAlerts`  (bool): Should the user receive admin alerts such a certificate expiration notifications and overages?
* `require_2fa` / `Require2fa`  (string): 2FA required setting
* `require_login_by` / `RequireLoginBy`  (Nullable<DateTime>): Require user to login by specified date otherwise it will be disabled.
* `active_2fa` / `Active2fa`  (bool): Is 2fa active for the user?
* `require_password_change` / `RequirePasswordChange`  (bool): Is a password change required upon next user login?
* `password_expired` / `PasswordExpired`  (bool): Is user's password expired?
* `readonly_site_admin` / `ReadonlySiteAdmin`  (bool): Is the user an allowed to view all (non-billing) site configuration for this site?
* `restapi_permission` / `RestapiPermission`  (bool): Can this user access the Web app, Desktop app, SDKs, or REST API?  (All of these tools use the API internally, so this is one unified permission set.)
* `self_managed` / `SelfManaged`  (bool): Does this user manage it's own credentials or is it a shared/bot user?
* `sftp_permission` / `SftpPermission`  (bool): Can the user access with SFTP?
* `site_admin` / `SiteAdmin`  (bool): Is the user an administrator for this site?
* `workspace_admin` / `WorkspaceAdmin`  (bool): Is the user a Workspace administrator?  Applicable only to the workspace ID related to this user, if one is set.
* `site_id` / `SiteId`  (Nullable<Int64>): Site ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID
* `skip_welcome_screen` / `SkipWelcomeScreen`  (bool): Skip Welcome page in the UI?
* `ssl_required` / `SslRequired`  (string): SSL required setting
* `sso_strategy_id` / `SsoStrategyId`  (Nullable<Int64>): SSO (Single Sign On) strategy ID for the user, if applicable.
* `subscribe_to_newsletter` / `SubscribeToNewsletter`  (bool): Is the user subscribed to the newsletter?
* `externally_managed` / `ExternallyManaged`  (bool): Is this user managed by a SsoStrategy?
* `tags` / `Tags`  (string): Comma-separated list of Tags for this user. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
* `time_zone` / `TimeZone`  (string): User time zone
* `type_of_2fa` / `TypeOf2fa`  (string): Type(s) of 2FA methods in use, for programmatic use.  Will be either `sms`, `totp`, `webauthn`, `yubi`, `email`, or multiple values sorted alphabetically and joined by an underscore.  Does not specify whether user has more than one of a given method.
* `type_of_2fa_for_display` / `TypeOf2faForDisplay`  (string): Type(s) of 2FA methods in use, formatted for displaying in the UI.  Unlike `type_of_2fa`, this value will make clear when a user has more than 1 of the same type of method.
* `user_root` / `UserRoot`  (string): Root folder for FTP (and optionally SFTP if the appropriate site-wide setting is set).  Note that this is not used for API, Desktop, or Web interface.
* `user_home` / `UserHome`  (string): Home folder for FTP/SFTP.  Note that this is not used for API, Desktop, or Web interface.
* `days_remaining_until_password_expire` / `DaysRemainingUntilPasswordExpire`  (Nullable<Int64>): Number of days remaining until password expires
* `password_expire_at` / `PasswordExpireAt`  (Nullable<DateTime>): Password expiration datetime
* `avatar_file` / `AvatarFile`  (System.Net.Http.ByteArrayContent): An image file for your user avatar.
* `avatar_delete` / `AvatarDelete`  (bool): If true, the avatar will be deleted.
* `change_password` / `ChangePassword`  (string): Used for changing a password on an existing user.
* `change_password_confirmation` / `ChangePasswordConfirmation`  (string): Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
* `grant_permission` / `GrantPermission`  (string): Permission to grant on the User Root upon user creation. Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
* `group_id` / `GroupId`  (Nullable<Int64>): Group ID to associate this user with.
* `imported_password_hash` / `ImportedPasswordHash`  (string): Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash methods are MD5, SHA1, and SHA256.
* `password` / `Password`  (string): User password.
* `password_confirmation` / `PasswordConfirmation`  (string): Optional, but if provided, we will ensure that it matches the value sent in `password`.
* `announcements_read` / `AnnouncementsRead`  (bool): Signifies that the user has read all the announcements in the UI.
* `clear_2fa` / `Clear2fa`  (bool): If true when changing authentication_method from `password` to `sso`, remove all two-factor methods. Ignored in all other cases.
* `convert_to_partner_user` / `ConvertToPartnerUser`  (bool): If true, convert this user to a partner user by assigning the partner_id provided.


---

## List Users

```
Task<FilesList<User>> User.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id`, `company`, `name`, `disabled`, `authenticate_until`, `username`, `email`, `last_desktop_login_at`, `last_login_at`, `site_admin`, `password_validity_days` or `ssl_required`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `username`, `name`, `email`, `company`, `site_admin`, `password_validity_days`, `ssl_required`, `last_login_at`, `authenticate_until`, `not_site_admin`, `disabled`, `partner_id` or `workspace_id`. Valid field combinations are `[ site_admin, username ]`, `[ not_site_admin, username ]`, `[ workspace_id, username ]`, `[ company, name ]`, `[ workspace_id, name ]`, `[ workspace_id, email ]`, `[ workspace_id, company ]`, `[ workspace_id, disabled ]`, `[ workspace_id, partner_id ]`, `[ workspace_id, disabled, username ]`, `[ workspace_id, partner_id, username ]` or `[ workspace_id, company, name ]`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `username`, `name`, `email` or `company`. Valid field combinations are `[ company, name ]`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
* `ids` (string): comma-separated list of User IDs
* `include_parent_site_users` (bool): Include users from the parent site.
* `search` (string): Searches for partial matches of name, username, or email.


---

## Show User

```
Task<User> User.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.


---

## Create User

```
Task<User> User.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `avatar_file` (System.Net.Http.ByteArrayContent): An image file for your user avatar.
* `avatar_delete` (bool): If true, the avatar will be deleted.
* `change_password` (string): Used for changing a password on an existing user.
* `change_password_confirmation` (string): Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
* `email` (string): User's email.
* `grant_permission` (string): Permission to grant on the User Root upon user creation. Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
* `group_id` (Nullable<Int64>): Group ID to associate this user with.
* `group_ids` (string): A list of group ids to associate this user with.  Comma delimited.
* `imported_password_hash` (string): Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash methods are MD5, SHA1, and SHA256.
* `password` (string): User password.
* `password_confirmation` (string): Optional, but if provided, we will ensure that it matches the value sent in `password`.
* `announcements_read` (bool): Signifies that the user has read all the announcements in the UI.
* `allowed_ips` (string): A list of allowed IPs if applicable.  Newline delimited
* `attachments_permission` (bool): DEPRECATED: If `true`, the user can user create Bundles (aka Share Links). Use the bundle permission instead.
* `authenticate_until` (string): Scheduled Date/Time at which user will be deactivated
* `authentication_method` (string): How is this user authenticated?
* `billing_permission` (bool): Allow this user to perform operations on the account, payments, and invoices?
* `bypass_user_lifecycle_rules` (bool): Exempt this user from user lifecycle rules?
* `bypass_site_allowed_ips` (bool): Allow this user to skip site-wide IP blacklists?
* `dav_permission` (bool): Can the user connect with WebDAV?
* `disabled` (bool): Is user disabled? Disabled users cannot log in, and do not count for billing purposes. Users can be automatically disabled after an inactivity period via a Site setting or schedule to be deactivated after specific date.
* `filesystem_layout` (string): File system layout
* `ftp_permission` (bool): Can the user access with FTP/FTPS?
* `header_text` (string): Text to display to the user in the header of the UI
* `language` (string): Preferred language
* `notification_daily_send_time` (Nullable<Int64>): Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
* `name` (string): User's full name
* `company` (string): User's company
* `notes` (string): Any internal notes on the user
* `office_integration_enabled` (bool): Enable integration with Office for the web?
* `partner_admin` (bool): Is this user a Partner administrator?
* `partner_id` (Nullable<Int64>): Partner ID if this user belongs to a Partner
* `password_validity_days` (Nullable<Int64>): Number of days to allow user to use the same password
* `readonly_site_admin` (bool): Is the user an allowed to view all (non-billing) site configuration for this site?
* `receive_admin_alerts` (bool): Should the user receive admin alerts such a certificate expiration notifications and overages?
* `require_login_by` (string): Require user to login by specified date otherwise it will be disabled.
* `require_password_change` (bool): Is a password change required upon next user login?
* `restapi_permission` (bool): Can this user access the Web app, Desktop app, SDKs, or REST API?  (All of these tools use the API internally, so this is one unified permission set.)
* `self_managed` (bool): Does this user manage it's own credentials or is it a shared/bot user?
* `sftp_permission` (bool): Can the user access with SFTP?
* `site_admin` (bool): Is the user an administrator for this site?
* `skip_welcome_screen` (bool): Skip Welcome page in the UI?
* `ssl_required` (string): SSL required setting
* `sso_strategy_id` (Nullable<Int64>): SSO (Single Sign On) strategy ID for the user, if applicable.
* `subscribe_to_newsletter` (bool): Is the user subscribed to the newsletter?
* `require_2fa` (string): 2FA required setting
* `tags` (string): Comma-separated list of Tags for this user. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
* `time_zone` (string): User time zone
* `user_root` (string): Root folder for FTP (and optionally SFTP if the appropriate site-wide setting is set).  Note that this is not used for API, Desktop, or Web interface.
* `user_home` (string): Home folder for FTP/SFTP.  Note that this is not used for API, Desktop, or Web interface.
* `workspace_admin` (bool): Is the user a Workspace administrator?  Applicable only to the workspace ID related to this user, if one is set.
* `username` (string): Required - User's username
* `workspace_id` (Nullable<Int64>): Workspace ID


---

## Unlock user who has been locked out due to failed logins

```
Task User.Unlock(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.


---

## Resend user welcome email

```
Task User.ResendWelcomeEmail(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.


---

## Trigger 2FA Reset process for user who has lost access to their existing 2FA methods

```
Task User.User2faReset(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.


---

## Update User

```
Task<User> User.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.
* `avatar_file` (System.Net.Http.ByteArrayContent): An image file for your user avatar.
* `avatar_delete` (bool): If true, the avatar will be deleted.
* `change_password` (string): Used for changing a password on an existing user.
* `change_password_confirmation` (string): Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
* `email` (string): User's email.
* `grant_permission` (string): Permission to grant on the User Root upon user creation. Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
* `group_id` (Nullable<Int64>): Group ID to associate this user with.
* `group_ids` (string): A list of group ids to associate this user with.  Comma delimited.
* `imported_password_hash` (string): Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash methods are MD5, SHA1, and SHA256.
* `password` (string): User password.
* `password_confirmation` (string): Optional, but if provided, we will ensure that it matches the value sent in `password`.
* `announcements_read` (bool): Signifies that the user has read all the announcements in the UI.
* `allowed_ips` (string): A list of allowed IPs if applicable.  Newline delimited
* `attachments_permission` (bool): DEPRECATED: If `true`, the user can user create Bundles (aka Share Links). Use the bundle permission instead.
* `authenticate_until` (string): Scheduled Date/Time at which user will be deactivated
* `authentication_method` (string): How is this user authenticated?
* `billing_permission` (bool): Allow this user to perform operations on the account, payments, and invoices?
* `bypass_user_lifecycle_rules` (bool): Exempt this user from user lifecycle rules?
* `bypass_site_allowed_ips` (bool): Allow this user to skip site-wide IP blacklists?
* `dav_permission` (bool): Can the user connect with WebDAV?
* `disabled` (bool): Is user disabled? Disabled users cannot log in, and do not count for billing purposes. Users can be automatically disabled after an inactivity period via a Site setting or schedule to be deactivated after specific date.
* `filesystem_layout` (string): File system layout
* `ftp_permission` (bool): Can the user access with FTP/FTPS?
* `header_text` (string): Text to display to the user in the header of the UI
* `language` (string): Preferred language
* `notification_daily_send_time` (Nullable<Int64>): Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
* `name` (string): User's full name
* `company` (string): User's company
* `notes` (string): Any internal notes on the user
* `office_integration_enabled` (bool): Enable integration with Office for the web?
* `partner_admin` (bool): Is this user a Partner administrator?
* `partner_id` (Nullable<Int64>): Partner ID if this user belongs to a Partner
* `password_validity_days` (Nullable<Int64>): Number of days to allow user to use the same password
* `readonly_site_admin` (bool): Is the user an allowed to view all (non-billing) site configuration for this site?
* `receive_admin_alerts` (bool): Should the user receive admin alerts such a certificate expiration notifications and overages?
* `require_login_by` (string): Require user to login by specified date otherwise it will be disabled.
* `require_password_change` (bool): Is a password change required upon next user login?
* `restapi_permission` (bool): Can this user access the Web app, Desktop app, SDKs, or REST API?  (All of these tools use the API internally, so this is one unified permission set.)
* `self_managed` (bool): Does this user manage it's own credentials or is it a shared/bot user?
* `sftp_permission` (bool): Can the user access with SFTP?
* `site_admin` (bool): Is the user an administrator for this site?
* `skip_welcome_screen` (bool): Skip Welcome page in the UI?
* `ssl_required` (string): SSL required setting
* `sso_strategy_id` (Nullable<Int64>): SSO (Single Sign On) strategy ID for the user, if applicable.
* `subscribe_to_newsletter` (bool): Is the user subscribed to the newsletter?
* `require_2fa` (string): 2FA required setting
* `tags` (string): Comma-separated list of Tags for this user. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
* `time_zone` (string): User time zone
* `user_root` (string): Root folder for FTP (and optionally SFTP if the appropriate site-wide setting is set).  Note that this is not used for API, Desktop, or Web interface.
* `user_home` (string): Home folder for FTP/SFTP.  Note that this is not used for API, Desktop, or Web interface.
* `workspace_admin` (bool): Is the user a Workspace administrator?  Applicable only to the workspace ID related to this user, if one is set.
* `username` (string): User's username
* `clear_2fa` (bool): If true when changing authentication_method from `password` to `sso`, remove all two-factor methods. Ignored in all other cases.
* `convert_to_partner_user` (bool): If true, convert this user to a partner user by assigning the partner_id provided.


---

## Delete User

```
Task User.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.
* `new_owner_id` (Nullable<Int64>): Provide a User ID here to transfer ownership of certain resources such as Automations and Share Links (Bundles) to that new user.


---

## Unlock user who has been locked out due to failed logins

```
var User = User.Find(1);

var parameters = new Dictionary<string, object>();


User.Unlock
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.


---

## Resend user welcome email

```
var User = User.Find(1);

var parameters = new Dictionary<string, object>();


User.ResendWelcomeEmail
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.


---

## Trigger 2FA Reset process for user who has lost access to their existing 2FA methods

```
var User = User.Find(1);

var parameters = new Dictionary<string, object>();


User.User2faReset
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.


---

## Update User

```
var User = User.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("avatar_delete", false);
parameters.Add("email", "john.doe@files.com");
parameters.Add("group_id", 1);
parameters.Add("group_ids", "example");
parameters.Add("announcements_read", false);
parameters.Add("allowed_ips", "10.0.0.0/8\n127.0.0.1");
parameters.Add("attachments_permission", true);
parameters.Add("authenticate_until", "2000-01-01T01:00:00Z");
parameters.Add("authentication_method", "password");
parameters.Add("billing_permission", false);
parameters.Add("bypass_user_lifecycle_rules", false);
parameters.Add("bypass_site_allowed_ips", false);
parameters.Add("dav_permission", true);
parameters.Add("disabled", true);
parameters.Add("filesystem_layout", "site_root");
parameters.Add("ftp_permission", true);
parameters.Add("header_text", "User-specific message.");
parameters.Add("language", "en");
parameters.Add("notification_daily_send_time", 18);
parameters.Add("name", "John Doe");
parameters.Add("company", "ACME Corp.");
parameters.Add("notes", "Internal notes on this user.");
parameters.Add("office_integration_enabled", true);
parameters.Add("partner_admin", true);
parameters.Add("partner_id", 1);
parameters.Add("password_validity_days", 1);
parameters.Add("readonly_site_admin", true);
parameters.Add("receive_admin_alerts", true);
parameters.Add("require_login_by", "2000-01-01T01:00:00Z");
parameters.Add("require_password_change", true);
parameters.Add("restapi_permission", true);
parameters.Add("self_managed", true);
parameters.Add("sftp_permission", true);
parameters.Add("site_admin", true);
parameters.Add("skip_welcome_screen", true);
parameters.Add("ssl_required", "always_require");
parameters.Add("sso_strategy_id", 1);
parameters.Add("subscribe_to_newsletter", true);
parameters.Add("require_2fa", "always_require");
parameters.Add("tags", "example");
parameters.Add("time_zone", "Pacific Time (US & Canada)");
parameters.Add("user_root", "example");
parameters.Add("user_home", "example");
parameters.Add("workspace_admin", true);
parameters.Add("username", "user");
parameters.Add("clear_2fa", false);
parameters.Add("convert_to_partner_user", false);

User.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.
* `avatar_file` (System.Net.Http.ByteArrayContent): An image file for your user avatar.
* `avatar_delete` (bool): If true, the avatar will be deleted.
* `change_password` (string): Used for changing a password on an existing user.
* `change_password_confirmation` (string): Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
* `email` (string): User's email.
* `grant_permission` (string): Permission to grant on the User Root upon user creation. Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
* `group_id` (Nullable<Int64>): Group ID to associate this user with.
* `group_ids` (string): A list of group ids to associate this user with.  Comma delimited.
* `imported_password_hash` (string): Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash methods are MD5, SHA1, and SHA256.
* `password` (string): User password.
* `password_confirmation` (string): Optional, but if provided, we will ensure that it matches the value sent in `password`.
* `announcements_read` (bool): Signifies that the user has read all the announcements in the UI.
* `allowed_ips` (string): A list of allowed IPs if applicable.  Newline delimited
* `attachments_permission` (bool): DEPRECATED: If `true`, the user can user create Bundles (aka Share Links). Use the bundle permission instead.
* `authenticate_until` (string): Scheduled Date/Time at which user will be deactivated
* `authentication_method` (string): How is this user authenticated?
* `billing_permission` (bool): Allow this user to perform operations on the account, payments, and invoices?
* `bypass_user_lifecycle_rules` (bool): Exempt this user from user lifecycle rules?
* `bypass_site_allowed_ips` (bool): Allow this user to skip site-wide IP blacklists?
* `dav_permission` (bool): Can the user connect with WebDAV?
* `disabled` (bool): Is user disabled? Disabled users cannot log in, and do not count for billing purposes. Users can be automatically disabled after an inactivity period via a Site setting or schedule to be deactivated after specific date.
* `filesystem_layout` (string): File system layout
* `ftp_permission` (bool): Can the user access with FTP/FTPS?
* `header_text` (string): Text to display to the user in the header of the UI
* `language` (string): Preferred language
* `notification_daily_send_time` (Nullable<Int64>): Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
* `name` (string): User's full name
* `company` (string): User's company
* `notes` (string): Any internal notes on the user
* `office_integration_enabled` (bool): Enable integration with Office for the web?
* `partner_admin` (bool): Is this user a Partner administrator?
* `partner_id` (Nullable<Int64>): Partner ID if this user belongs to a Partner
* `password_validity_days` (Nullable<Int64>): Number of days to allow user to use the same password
* `readonly_site_admin` (bool): Is the user an allowed to view all (non-billing) site configuration for this site?
* `receive_admin_alerts` (bool): Should the user receive admin alerts such a certificate expiration notifications and overages?
* `require_login_by` (string): Require user to login by specified date otherwise it will be disabled.
* `require_password_change` (bool): Is a password change required upon next user login?
* `restapi_permission` (bool): Can this user access the Web app, Desktop app, SDKs, or REST API?  (All of these tools use the API internally, so this is one unified permission set.)
* `self_managed` (bool): Does this user manage it's own credentials or is it a shared/bot user?
* `sftp_permission` (bool): Can the user access with SFTP?
* `site_admin` (bool): Is the user an administrator for this site?
* `skip_welcome_screen` (bool): Skip Welcome page in the UI?
* `ssl_required` (string): SSL required setting
* `sso_strategy_id` (Nullable<Int64>): SSO (Single Sign On) strategy ID for the user, if applicable.
* `subscribe_to_newsletter` (bool): Is the user subscribed to the newsletter?
* `require_2fa` (string): 2FA required setting
* `tags` (string): Comma-separated list of Tags for this user. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
* `time_zone` (string): User time zone
* `user_root` (string): Root folder for FTP (and optionally SFTP if the appropriate site-wide setting is set).  Note that this is not used for API, Desktop, or Web interface.
* `user_home` (string): Home folder for FTP/SFTP.  Note that this is not used for API, Desktop, or Web interface.
* `workspace_admin` (bool): Is the user a Workspace administrator?  Applicable only to the workspace ID related to this user, if one is set.
* `username` (string): User's username
* `clear_2fa` (bool): If true when changing authentication_method from `password` to `sso`, remove all two-factor methods. Ignored in all other cases.
* `convert_to_partner_user` (bool): If true, convert this user to a partner user by assigning the partner_id provided.


---

## Delete User

```
var User = User.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("new_owner_id", 1);

User.Delete(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - User ID.
* `new_owner_id` (Nullable<Int64>): Provide a User ID here to transfer ownership of certain resources such as Automations and Share Links (Bundles) to that new user.
