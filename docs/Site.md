# Files.Models.Site

## Example Site Object

```
{
  "name": "My Site",
  "allowed_2fa_method_sms": true,
  "allowed_2fa_method_totp": true,
  "allowed_2fa_method_u2f": true,
  "allowed_2fa_method_yubi": true,
  "admin_user_id": 1,
  "allow_bundle_names": true,
  "allowed_countries": "US,DE",
  "allowed_ips": "",
  "ask_about_overwrites": true,
  "bundle_expiration": 1,
  "bundle_password_required": true,
  "color2_left": "#0066a7",
  "color2_link": "#d34f5d",
  "color2_text": "#0066a7",
  "color2_top": "#000000",
  "color2_top_text": "#ffffff",
  "created_at": "2000-01-01T01:00:00Z",
  "currency": "USD",
  "custom_namespace": true,
  "days_to_retain_backups": 30,
  "default_time_zone": "Pacific Time (US & Canada)",
  "desktop_app": true,
  "desktop_app_session_ip_pinning": true,
  "desktop_app_session_lifetime": 1,
  "disallowed_countries": "US,DE",
  "disable_notifications": true,
  "disable_password_reset": true,
  "domain": "my-custom-domain.com",
  "email": "john.doe@files.com",
  "reply_to_email": "jane.doe@files.com",
  "non_sso_groups_allowed": true,
  "non_sso_users_allowed": true,
  "folder_permissions_groups_only": true,
  "hipaa": true,
  "icon128": {
    "name": "My logo",
    "uri": "https://mysite.files.com/.../my_image.png"
  },
  "icon16": {
    "name": "My logo",
    "uri": "https://mysite.files.com/.../my_image.png"
  },
  "icon32": {
    "name": "My logo",
    "uri": "https://mysite.files.com/.../my_image.png"
  },
  "icon48": {
    "name": "My logo",
    "uri": "https://mysite.files.com/.../my_image.png"
  },
  "immutable_files_set_at": "2000-01-01T01:00:00Z",
  "include_password_in_welcome_email": true,
  "language": "en",
  "ldap_base_dn": "",
  "ldap_domain": "mysite.com",
  "ldap_enabled": true,
  "ldap_group_action": "disabled",
  "ldap_group_exclusion": "",
  "ldap_group_inclusion": "",
  "ldap_host": "ldap.site.com",
  "ldap_host_2": "ldap2.site.com",
  "ldap_host_3": "ldap3.site.com",
  "ldap_port": 1,
  "ldap_secure": true,
  "ldap_type": "open_ldap",
  "ldap_user_action": "disabled",
  "ldap_user_include_groups": "",
  "ldap_username": "[ldap username]",
  "ldap_username_field": "sAMAccountName",
  "login_help_text": "Login page help text.",
  "logo": {
    "name": "My logo",
    "uri": "https://mysite.files.com/.../my_image.png"
  },
  "max_prior_passwords": 1,
  "next_billing_amount": 1.0,
  "next_billing_date": "Apr 20",
  "office_integration_available": true,
  "opt_out_global": true,
  "overage_notified_at": "2000-01-01T01:00:00Z",
  "overage_notify": true,
  "overdue": true,
  "password_min_length": 1,
  "password_require_letter": true,
  "password_require_mixed": true,
  "password_require_number": true,
  "password_require_special": true,
  "password_require_unbreached": true,
  "password_requirements_apply_to_bundles": true,
  "password_validity_days": 1,
  "phone": "555-555-5555",
  "require_2fa": true,
  "require_2fa_stop_time": "2000-01-01T01:00:00Z",
  "require_2fa_user_type": "`site_admins`",
  "session": {
    "id": "60525f92e859c4c3d74cb02fd176b1525901b525",
    "language": "en",
    "login_token": "@tok-randomcode",
    "login_token_domain": "https://mysite.files.com",
    "max_dir_listing_size": false,
    "multiple_regions": true,
    "read_only": true,
    "root_path": "",
    "site_id": 1,
    "ssl_required": true,
    "tls_disabled": false,
    "two_factor_setup_needed": false,
    "allowed_2fa_method_sms": true,
    "allowed_2fa_method_totp": true,
    "allowed_2fa_method_u2f": true,
    "allowed_2fa_method_yubi": true,
    "use_provided_modified_at": true,
    "windows_mode_ftp": false
  },
  "session_pinned_by_ip": true,
  "sftp_user_root_enabled": true,
  "sharing_enabled": true,
  "show_request_access_link": true,
  "site_footer": "",
  "site_header": "",
  "smtp_address": "smtp.my-mail-server.com",
  "smtp_authentication": "plain",
  "smtp_from": "me@my-mail-server.com",
  "smtp_port": 25,
  "smtp_username": "mail",
  "session_expiry": 6.0,
  "ssl_required": true,
  "subdomain": "mysite",
  "switch_to_plan_date": "2000-01-01T01:00:00Z",
  "tls_disabled": true,
  "trial_days_left": 1,
  "trial_until": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z",
  "use_provided_modified_at": true,
  "user": {
    "id": 1,
    "username": "user",
    "admin_group_ids": [
      1
    ],
    "allowed_ips": "127.0.0.1",
    "attachments_permission": true,
    "api_keys_count": 1,
    "authenticate_until": "2000-01-01T01:00:00Z",
    "authentication_method": "password",
    "avatar_url": "",
    "billing_permission": false,
    "bypass_site_allowed_ips": false,
    "bypass_inactive_disable": false,
    "created_at": "2000-01-01T01:00:00Z",
    "dav_permission": true,
    "disabled": true,
    "email": "john.doe@files.com",
    "ftp_permission": true,
    "group_ids": "",
    "header_text": "User-specific message.",
    "language": "en",
    "last_login_at": "2000-01-01T01:00:00Z",
    "last_protocol_cipher": "",
    "lockout_expires": "2000-01-01T01:00:00Z",
    "name": "John Doe",
    "company": "ACME Corp.",
    "notes": "Internal notes on this user.",
    "notification_daily_send_time": 18,
    "office_integration_enabled": true,
    "password_set_at": "2000-01-01T01:00:00Z",
    "password_validity_days": 1,
    "public_keys_count": 1,
    "receive_admin_alerts": true,
    "require_2fa": "always_require",
    "active_2fa": true,
    "require_password_change": true,
    "restapi_permission": true,
    "self_managed": true,
    "sftp_permission": true,
    "site_admin": true,
    "skip_welcome_screen": true,
    "ssl_required": "always_require",
    "sso_strategy_id": 1,
    "subscribe_to_newsletter": true,
    "externally_managed": true,
    "time_zone": "Pacific Time (US & Canada)",
    "type_of_2fa": "",
    "user_root": ""
  },
  "user_lockout": true,
  "user_lockout_lock_period": 1,
  "user_lockout_tries": 1,
  "user_lockout_within": 6,
  "user_requests_enabled": true,
  "welcome_custom_text": "Welcome to my site!",
  "welcome_email_cc": "",
  "welcome_email_enabled": true,
  "welcome_screen": "user_controlled",
  "windows_mode_ftp": true,
  "disable_users_from_inactivity_period_days": 1
}
```

* `name` / `Name`  (string): Site name
* `allowed_2fa_method_sms` / `Allowed2faMethodSms`  (bool): Is SMS two factor authentication allowed?
* `allowed_2fa_method_totp` / `Allowed2faMethodTotp`  (bool): Is TOTP two factor authentication allowed?
* `allowed_2fa_method_u2f` / `Allowed2faMethodU2f`  (bool): Is U2F two factor authentication allowed?
* `allowed_2fa_method_yubi` / `Allowed2faMethodYubi`  (bool): Is yubikey two factor authentication allowed?
* `admin_user_id` / `AdminUserId`  (Nullable<Int64>): User ID for the main site administrator
* `allow_bundle_names` / `AllowBundleNames`  (bool): Are manual Bundle names allowed?
* `allowed_countries` / `AllowedCountries`  (string): Comma seperated list of allowed Country codes
* `allowed_ips` / `AllowedIps`  (string): List of allowed IP addresses
* `ask_about_overwrites` / `AskAboutOverwrites`  (bool): If false, rename conflicting files instead of asking for overwrite confirmation.  Only applies to web interface.
* `bundle_expiration` / `BundleExpiration`  (Nullable<Int64>): Site-wide Bundle expiration in days
* `bundle_password_required` / `BundlePasswordRequired`  (bool): Do Bundles require password protection?
* `color2_left` / `Color2Left`  (string): Page link and button color
* `color2_link` / `Color2Link`  (string): Top bar link color
* `color2_text` / `Color2Text`  (string): Page link and button color
* `color2_top` / `Color2Top`  (string): Top bar background color
* `color2_top_text` / `Color2TopText`  (string): Top bar text color
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Time this site was created
* `currency` / `Currency`  (string): Preferred currency
* `custom_namespace` / `CustomNamespace`  (bool): Is this site using a custom namespace for users?
* `days_to_retain_backups` / `DaysToRetainBackups`  (Nullable<Int64>): Number of days to keep deleted files
* `default_time_zone` / `DefaultTimeZone`  (string): Site default time zone
* `desktop_app` / `DesktopApp`  (bool): Is the desktop app enabled?
* `desktop_app_session_ip_pinning` / `DesktopAppSessionIpPinning`  (bool): Is desktop app session IP pinning enabled?
* `desktop_app_session_lifetime` / `DesktopAppSessionLifetime`  (Nullable<Int64>): Desktop app session lifetime (in hours)
* `disallowed_countries` / `DisallowedCountries`  (string): Comma seperated list of disallowed Country codes
* `disable_notifications` / `DisableNotifications`  (bool): Are notifications disabled?
* `disable_password_reset` / `DisablePasswordReset`  (bool): Is password reset disabled?
* `domain` / `Domain`  (string): Custom domain
* `email` / `Email`  (string): Main email for this site
* `reply_to_email` / `ReplyToEmail`  (string): Reply-to email for this site
* `non_sso_groups_allowed` / `NonSsoGroupsAllowed`  (bool): If true, groups can be manually created / modified / deleted by Site Admins. Otherwise, groups can only be managed via your SSO provider.
* `non_sso_users_allowed` / `NonSsoUsersAllowed`  (bool): If true, users can be manually created / modified / deleted by Site Admins. Otherwise, users can only be managed via your SSO provider.
* `folder_permissions_groups_only` / `FolderPermissionsGroupsOnly`  (bool): If true, permissions for this site must be bound to a group (not a user). Otherwise, permissions must be bound to a user.
* `hipaa` / `Hipaa`  (bool): Is there a signed HIPAA BAA between Files.com and this site?
* `icon128` / `Icon128`  (object): Branded icon 128x128
* `icon16` / `Icon16`  (object): Branded icon 16x16
* `icon32` / `Icon32`  (object): Branded icon 32x32
* `icon48` / `Icon48`  (object): Branded icon 48x48
* `immutable_files_set_at` / `ImmutableFilesSetAt`  (Nullable<DateTime>): Can files be modified?
* `include_password_in_welcome_email` / `IncludePasswordInWelcomeEmail`  (bool): Include password in emails to new users?
* `language` / `Language`  (string): Site default language
* `ldap_base_dn` / `LdapBaseDn`  (string): Base DN for looking up users in LDAP server
* `ldap_domain` / `LdapDomain`  (string): Domain name that will be appended to usernames
* `ldap_enabled` / `LdapEnabled`  (bool): Main LDAP setting: is LDAP enabled?
* `ldap_group_action` / `LdapGroupAction`  (string): Should we sync groups from LDAP server?
* `ldap_group_exclusion` / `LdapGroupExclusion`  (string): Comma or newline separated list of group names (with optional wildcards) to exclude when syncing.
* `ldap_group_inclusion` / `LdapGroupInclusion`  (string): Comma or newline separated list of group names (with optional wildcards) to include when syncing.
* `ldap_host` / `LdapHost`  (string): LDAP host
* `ldap_host_2` / `LdapHost2`  (string): LDAP backup host
* `ldap_host_3` / `LdapHost3`  (string): LDAP backup host
* `ldap_port` / `LdapPort`  (Nullable<Int64>): LDAP port
* `ldap_secure` / `LdapSecure`  (bool): Use secure LDAP?
* `ldap_type` / `LdapType`  (string): LDAP type
* `ldap_user_action` / `LdapUserAction`  (string): Should we sync users from LDAP server?
* `ldap_user_include_groups` / `LdapUserIncludeGroups`  (string): Comma or newline separated list of group names (with optional wildcards) - if provided, only users in these groups will be added or synced.
* `ldap_username` / `LdapUsername`  (string): Username for signing in to LDAP server.
* `ldap_username_field` / `LdapUsernameField`  (string): LDAP username field
* `login_help_text` / `LoginHelpText`  (string): Login help text
* `logo` / `Logo`  (object): Branded logo
* `max_prior_passwords` / `MaxPriorPasswords`  (Nullable<Int64>): Number of prior passwords to disallow
* `next_billing_amount` / `NextBillingAmount`  (double): Next billing amount
* `next_billing_date` / `NextBillingDate`  (string): Next billing date
* `office_integration_available` / `OfficeIntegrationAvailable`  (bool): Allow users to use Office for the web?
* `opt_out_global` / `OptOutGlobal`  (bool): Use servers in the USA only?
* `overage_notified_at` / `OverageNotifiedAt`  (Nullable<DateTime>): Last time the site was notified about an overage
* `overage_notify` / `OverageNotify`  (bool): Notify site email of overages?
* `overdue` / `Overdue`  (bool): Is this site's billing overdue?
* `password_min_length` / `PasswordMinLength`  (Nullable<Int64>): Shortest password length for users
* `password_require_letter` / `PasswordRequireLetter`  (bool): Require a letter in passwords?
* `password_require_mixed` / `PasswordRequireMixed`  (bool): Require lower and upper case letters in passwords?
* `password_require_number` / `PasswordRequireNumber`  (bool): Require a number in passwords?
* `password_require_special` / `PasswordRequireSpecial`  (bool): Require special characters in password?
* `password_require_unbreached` / `PasswordRequireUnbreached`  (bool): Require passwords that have not been previously breached? (see https://haveibeenpwned.com/)
* `password_requirements_apply_to_bundles` / `PasswordRequirementsApplyToBundles`  (bool): Require bundles' passwords, and passwords for other items (inboxes, public shares, etc.) to conform to the same requirements as users' passwords?
* `password_validity_days` / `PasswordValidityDays`  (Nullable<Int64>): Number of days password is valid
* `phone` / `Phone`  (string): Site phone number
* `require_2fa` / `Require2fa`  (bool): Require two-factor authentication for all users?
* `require_2fa_stop_time` / `Require2faStopTime`  (Nullable<DateTime>): If set, requirement for two-factor authentication has been scheduled to end on this date-time.
* `require_2fa_user_type` / `Require2faUserType`  (string): What type of user is required to use two-factor authentication (when require_2fa is set to `true` for this site)?
* `session` / `Session`  (object): Current session
* `session_pinned_by_ip` / `SessionPinnedByIp`  (bool): Are sessions locked to the same IP? (i.e. do users need to log in again if they change IPs?)
* `sftp_user_root_enabled` / `SftpUserRootEnabled`  (bool): Use user FTP roots also for SFTP?
* `sharing_enabled` / `SharingEnabled`  (bool): Allow bundle creation
* `show_request_access_link` / `ShowRequestAccessLink`  (bool): Show request access link for users without access?  Currently unused.
* `site_footer` / `SiteFooter`  (string): Custom site footer text
* `site_header` / `SiteHeader`  (string): Custom site header text
* `smtp_address` / `SmtpAddress`  (string): SMTP server hostname or IP
* `smtp_authentication` / `SmtpAuthentication`  (string): SMTP server authentication type
* `smtp_from` / `SmtpFrom`  (string): From address to use when mailing through custom SMTP
* `smtp_port` / `SmtpPort`  (Nullable<Int64>): SMTP server port
* `smtp_username` / `SmtpUsername`  (string): SMTP server username
* `session_expiry` / `SessionExpiry`  (double): Session expiry in hours
* `ssl_required` / `SslRequired`  (bool): Is SSL required?  Disabling this is insecure.
* `subdomain` / `Subdomain`  (string): Site subdomain
* `switch_to_plan_date` / `SwitchToPlanDate`  (Nullable<DateTime>): If switching plans, when does the new plan take effect?
* `tls_disabled` / `TlsDisabled`  (bool): Is TLS disabled(site setting)?
* `trial_days_left` / `TrialDaysLeft`  (Nullable<Int64>): Number of days left in trial
* `trial_until` / `TrialUntil`  (Nullable<DateTime>): When does this Site trial expire?
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last time this Site was updated
* `use_provided_modified_at` / `UseProvidedModifiedAt`  (bool): Allow uploaders to set `provided_modified_at` for uploaded files?
* `user` / `User`  (object): User of current session
* `user_lockout` / `UserLockout`  (bool): Will users be locked out after incorrect login attempts?
* `user_lockout_lock_period` / `UserLockoutLockPeriod`  (Nullable<Int64>): How many hours to lock user out for failed password?
* `user_lockout_tries` / `UserLockoutTries`  (Nullable<Int64>): Number of login tries within `user_lockout_within` hours before users are locked out
* `user_lockout_within` / `UserLockoutWithin`  (Nullable<Int64>): Number of hours for user lockout window
* `user_requests_enabled` / `UserRequestsEnabled`  (bool): Enable User Requests feature
* `welcome_custom_text` / `WelcomeCustomText`  (string): Custom text send in user welcome email
* `welcome_email_cc` / `WelcomeEmailCc`  (string): Include this email in welcome emails if enabled
* `welcome_email_enabled` / `WelcomeEmailEnabled`  (bool): Will the welcome email be sent to new users?
* `welcome_screen` / `WelcomeScreen`  (string): Does the welcome screen appear?
* `windows_mode_ftp` / `WindowsModeFtp`  (bool): Does FTP user Windows emulation mode?
* `disable_users_from_inactivity_period_days` / `DisableUsersFromInactivityPeriodDays`  (Nullable<Int64>): If greater than zero, users will unable to login if they do not show activity within this number of days.


---

## Show site settings

```
Task<Site> Site.Get(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `format` (string): 
* `site` (object): 


---

## Get the most recent usage snapshot (usage data for billing purposes) for a Site

```
Task<UsageSnapshot> Site.GetUsage(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `format` (string): 
* `site` (object): 


---

## Update site settings

```
Task<Site> Site.Update(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Site name
* `subdomain` (string): Site subdomain
* `domain` (string): Custom domain
* `email` (string): Main email for this site
* `reply_to_email` (string): Reply-to email for this site
* `allow_bundle_names` (bool): Are manual Bundle names allowed?
* `bundle_expiration` (Nullable<Int64>): Site-wide Bundle expiration in days
* `overage_notify` (bool): Notify site email of overages?
* `welcome_email_enabled` (bool): Will the welcome email be sent to new users?
* `ask_about_overwrites` (bool): If false, rename conflicting files instead of asking for overwrite confirmation.  Only applies to web interface.
* `show_request_access_link` (bool): Show request access link for users without access?  Currently unused.
* `welcome_email_cc` (string): Include this email in welcome emails if enabled
* `welcome_custom_text` (string): Custom text send in user welcome email
* `language` (string): Site default language
* `windows_mode_ftp` (bool): Does FTP user Windows emulation mode?
* `default_time_zone` (string): Site default time zone
* `desktop_app` (bool): Is the desktop app enabled?
* `desktop_app_session_ip_pinning` (bool): Is desktop app session IP pinning enabled?
* `desktop_app_session_lifetime` (Nullable<Int64>): Desktop app session lifetime (in hours)
* `folder_permissions_groups_only` (bool): If true, permissions for this site must be bound to a group (not a user). Otherwise, permissions must be bound to a user.
* `welcome_screen` (string): Does the welcome screen appear?
* `office_integration_available` (bool): Allow users to use Office for the web?
* `session_expiry` (double): Session expiry in hours
* `ssl_required` (bool): Is SSL required?  Disabling this is insecure.
* `tls_disabled` (bool): Is TLS disabled(site setting)?
* `user_lockout` (bool): Will users be locked out after incorrect login attempts?
* `user_lockout_tries` (Nullable<Int64>): Number of login tries within `user_lockout_within` hours before users are locked out
* `user_lockout_within` (Nullable<Int64>): Number of hours for user lockout window
* `user_lockout_lock_period` (Nullable<Int64>): How many hours to lock user out for failed password?
* `include_password_in_welcome_email` (bool): Include password in emails to new users?
* `allowed_countries` (string): Comma seperated list of allowed Country codes
* `allowed_ips` (string): List of allowed IP addresses
* `disallowed_countries` (string): Comma seperated list of disallowed Country codes
* `days_to_retain_backups` (Nullable<Int64>): Number of days to keep deleted files
* `max_prior_passwords` (Nullable<Int64>): Number of prior passwords to disallow
* `password_validity_days` (Nullable<Int64>): Number of days password is valid
* `password_min_length` (Nullable<Int64>): Shortest password length for users
* `password_require_letter` (bool): Require a letter in passwords?
* `password_require_mixed` (bool): Require lower and upper case letters in passwords?
* `password_require_special` (bool): Require special characters in password?
* `password_require_number` (bool): Require a number in passwords?
* `password_require_unbreached` (bool): Require passwords that have not been previously breached? (see https://haveibeenpwned.com/)
* `sftp_user_root_enabled` (bool): Use user FTP roots also for SFTP?
* `disable_password_reset` (bool): Is password reset disabled?
* `immutable_files` (bool): Are files protected from modification?
* `session_pinned_by_ip` (bool): Are sessions locked to the same IP? (i.e. do users need to log in again if they change IPs?)
* `bundle_password_required` (bool): Do Bundles require password protection?
* `password_requirements_apply_to_bundles` (bool): Require bundles' passwords, and passwords for other items (inboxes, public shares, etc.) to conform to the same requirements as users' passwords?
* `opt_out_global` (bool): Use servers in the USA only?
* `use_provided_modified_at` (bool): Allow uploaders to set `provided_modified_at` for uploaded files?
* `custom_namespace` (bool): Is this site using a custom namespace for users?
* `disable_users_from_inactivity_period_days` (Nullable<Int64>): If greater than zero, users will unable to login if they do not show activity within this number of days.
* `non_sso_groups_allowed` (bool): If true, groups can be manually created / modified / deleted by Site Admins. Otherwise, groups can only be managed via your SSO provider.
* `non_sso_users_allowed` (bool): If true, users can be manually created / modified / deleted by Site Admins. Otherwise, users can only be managed via your SSO provider.
* `sharing_enabled` (bool): Allow bundle creation
* `user_requests_enabled` (bool): Enable User Requests feature
* `allowed_2fa_method_sms` (bool): Is SMS two factor authentication allowed?
* `allowed_2fa_method_u2f` (bool): Is U2F two factor authentication allowed?
* `allowed_2fa_method_totp` (bool): Is TOTP two factor authentication allowed?
* `allowed_2fa_method_yubi` (bool): Is yubikey two factor authentication allowed?
* `require_2fa` (bool): Require two-factor authentication for all users?
* `require_2fa_user_type` (string): What type of user is required to use two-factor authentication (when require_2fa is set to `true` for this site)?
* `color2_top` (string): Top bar background color
* `color2_left` (string): Page link and button color
* `color2_link` (string): Top bar link color
* `color2_text` (string): Page link and button color
* `color2_top_text` (string): Top bar text color
* `site_header` (string): Custom site header text
* `site_footer` (string): Custom site footer text
* `login_help_text` (string): Login help text
* `smtp_address` (string): SMTP server hostname or IP
* `smtp_authentication` (string): SMTP server authentication type
* `smtp_from` (string): From address to use when mailing through custom SMTP
* `smtp_username` (string): SMTP server username
* `smtp_port` (Nullable<Int64>): SMTP server port
* `ldap_enabled` (bool): Main LDAP setting: is LDAP enabled?
* `ldap_type` (string): LDAP type
* `ldap_host` (string): LDAP host
* `ldap_host_2` (string): LDAP backup host
* `ldap_host_3` (string): LDAP backup host
* `ldap_port` (Nullable<Int64>): LDAP port
* `ldap_secure` (bool): Use secure LDAP?
* `ldap_username` (string): Username for signing in to LDAP server.
* `ldap_username_field` (string): LDAP username field
* `ldap_domain` (string): Domain name that will be appended to usernames
* `ldap_user_action` (string): Should we sync users from LDAP server?
* `ldap_group_action` (string): Should we sync groups from LDAP server?
* `ldap_user_include_groups` (string): Comma or newline separated list of group names (with optional wildcards) - if provided, only users in these groups will be added or synced.
* `ldap_group_exclusion` (string): Comma or newline separated list of group names (with optional wildcards) to exclude when syncing.
* `ldap_group_inclusion` (string): Comma or newline separated list of group names (with optional wildcards) to include when syncing.
* `ldap_base_dn` (string): Base DN for looking up users in LDAP server
* `icon16_file` (System.Net.Http.ByteArrayContent): 
* `icon16_delete` (bool): If true, will delete the file stored in icon16
* `icon32_file` (System.Net.Http.ByteArrayContent): 
* `icon32_delete` (bool): If true, will delete the file stored in icon32
* `icon48_file` (System.Net.Http.ByteArrayContent): 
* `icon48_delete` (bool): If true, will delete the file stored in icon48
* `icon128_file` (System.Net.Http.ByteArrayContent): 
* `icon128_delete` (bool): If true, will delete the file stored in icon128
* `logo_file` (System.Net.Http.ByteArrayContent): 
* `logo_delete` (bool): If true, will delete the file stored in logo
* `disable_2fa_with_delay` (bool): If set to true, we will begin the process of disabling 2FA on this site.
* `ldap_password_change` (string): New LDAP password.
* `ldap_password_change_confirmation` (string): Confirm new LDAP password.
* `smtp_password` (string): Password for SMTP server.
