using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class User
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public User() : this(null, null) { }

        public User(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("admin_group_ids"))
            {
                this.attributes.Add("admin_group_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("allowed_ips"))
            {
                this.attributes.Add("allowed_ips", null);
            }
            if (!this.attributes.ContainsKey("attachments_permission"))
            {
                this.attributes.Add("attachments_permission", null);
            }
            if (!this.attributes.ContainsKey("api_keys_count"))
            {
                this.attributes.Add("api_keys_count", null);
            }
            if (!this.attributes.ContainsKey("authenticate_until"))
            {
                this.attributes.Add("authenticate_until", null);
            }
            if (!this.attributes.ContainsKey("authentication_method"))
            {
                this.attributes.Add("authentication_method", null);
            }
            if (!this.attributes.ContainsKey("avatar_url"))
            {
                this.attributes.Add("avatar_url", null);
            }
            if (!this.attributes.ContainsKey("billing_permission"))
            {
                this.attributes.Add("billing_permission", null);
            }
            if (!this.attributes.ContainsKey("bypass_site_allowed_ips"))
            {
                this.attributes.Add("bypass_site_allowed_ips", null);
            }
            if (!this.attributes.ContainsKey("bypass_inactive_disable"))
            {
                this.attributes.Add("bypass_inactive_disable", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("dav_permission"))
            {
                this.attributes.Add("dav_permission", null);
            }
            if (!this.attributes.ContainsKey("disabled"))
            {
                this.attributes.Add("disabled", null);
            }
            if (!this.attributes.ContainsKey("email"))
            {
                this.attributes.Add("email", null);
            }
            if (!this.attributes.ContainsKey("first_login_at"))
            {
                this.attributes.Add("first_login_at", null);
            }
            if (!this.attributes.ContainsKey("ftp_permission"))
            {
                this.attributes.Add("ftp_permission", null);
            }
            if (!this.attributes.ContainsKey("group_ids"))
            {
                this.attributes.Add("group_ids", null);
            }
            if (!this.attributes.ContainsKey("header_text"))
            {
                this.attributes.Add("header_text", null);
            }
            if (!this.attributes.ContainsKey("language"))
            {
                this.attributes.Add("language", null);
            }
            if (!this.attributes.ContainsKey("last_login_at"))
            {
                this.attributes.Add("last_login_at", null);
            }
            if (!this.attributes.ContainsKey("last_web_login_at"))
            {
                this.attributes.Add("last_web_login_at", null);
            }
            if (!this.attributes.ContainsKey("last_ftp_login_at"))
            {
                this.attributes.Add("last_ftp_login_at", null);
            }
            if (!this.attributes.ContainsKey("last_sftp_login_at"))
            {
                this.attributes.Add("last_sftp_login_at", null);
            }
            if (!this.attributes.ContainsKey("last_dav_login_at"))
            {
                this.attributes.Add("last_dav_login_at", null);
            }
            if (!this.attributes.ContainsKey("last_desktop_login_at"))
            {
                this.attributes.Add("last_desktop_login_at", null);
            }
            if (!this.attributes.ContainsKey("last_restapi_login_at"))
            {
                this.attributes.Add("last_restapi_login_at", null);
            }
            if (!this.attributes.ContainsKey("last_api_use_at"))
            {
                this.attributes.Add("last_api_use_at", null);
            }
            if (!this.attributes.ContainsKey("last_active_at"))
            {
                this.attributes.Add("last_active_at", null);
            }
            if (!this.attributes.ContainsKey("last_protocol_cipher"))
            {
                this.attributes.Add("last_protocol_cipher", null);
            }
            if (!this.attributes.ContainsKey("lockout_expires"))
            {
                this.attributes.Add("lockout_expires", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("company"))
            {
                this.attributes.Add("company", null);
            }
            if (!this.attributes.ContainsKey("notes"))
            {
                this.attributes.Add("notes", null);
            }
            if (!this.attributes.ContainsKey("notification_daily_send_time"))
            {
                this.attributes.Add("notification_daily_send_time", null);
            }
            if (!this.attributes.ContainsKey("office_integration_enabled"))
            {
                this.attributes.Add("office_integration_enabled", null);
            }
            if (!this.attributes.ContainsKey("password_set_at"))
            {
                this.attributes.Add("password_set_at", null);
            }
            if (!this.attributes.ContainsKey("password_validity_days"))
            {
                this.attributes.Add("password_validity_days", null);
            }
            if (!this.attributes.ContainsKey("public_keys_count"))
            {
                this.attributes.Add("public_keys_count", null);
            }
            if (!this.attributes.ContainsKey("receive_admin_alerts"))
            {
                this.attributes.Add("receive_admin_alerts", null);
            }
            if (!this.attributes.ContainsKey("require_2fa"))
            {
                this.attributes.Add("require_2fa", null);
            }
            if (!this.attributes.ContainsKey("active_2fa"))
            {
                this.attributes.Add("active_2fa", null);
            }
            if (!this.attributes.ContainsKey("require_password_change"))
            {
                this.attributes.Add("require_password_change", null);
            }
            if (!this.attributes.ContainsKey("password_expired"))
            {
                this.attributes.Add("password_expired", null);
            }
            if (!this.attributes.ContainsKey("restapi_permission"))
            {
                this.attributes.Add("restapi_permission", null);
            }
            if (!this.attributes.ContainsKey("self_managed"))
            {
                this.attributes.Add("self_managed", null);
            }
            if (!this.attributes.ContainsKey("sftp_permission"))
            {
                this.attributes.Add("sftp_permission", null);
            }
            if (!this.attributes.ContainsKey("site_admin"))
            {
                this.attributes.Add("site_admin", null);
            }
            if (!this.attributes.ContainsKey("skip_welcome_screen"))
            {
                this.attributes.Add("skip_welcome_screen", null);
            }
            if (!this.attributes.ContainsKey("ssl_required"))
            {
                this.attributes.Add("ssl_required", null);
            }
            if (!this.attributes.ContainsKey("sso_strategy_id"))
            {
                this.attributes.Add("sso_strategy_id", null);
            }
            if (!this.attributes.ContainsKey("subscribe_to_newsletter"))
            {
                this.attributes.Add("subscribe_to_newsletter", null);
            }
            if (!this.attributes.ContainsKey("externally_managed"))
            {
                this.attributes.Add("externally_managed", null);
            }
            if (!this.attributes.ContainsKey("time_zone"))
            {
                this.attributes.Add("time_zone", null);
            }
            if (!this.attributes.ContainsKey("type_of_2fa"))
            {
                this.attributes.Add("type_of_2fa", null);
            }
            if (!this.attributes.ContainsKey("user_root"))
            {
                this.attributes.Add("user_root", null);
            }
            if (!this.attributes.ContainsKey("days_remaining_until_password_expire"))
            {
                this.attributes.Add("days_remaining_until_password_expire", null);
            }
            if (!this.attributes.ContainsKey("password_expire_at"))
            {
                this.attributes.Add("password_expire_at", null);
            }
            if (!this.attributes.ContainsKey("avatar_file"))
            {
                this.attributes.Add("avatar_file", null);
            }
            if (!this.attributes.ContainsKey("avatar_delete"))
            {
                this.attributes.Add("avatar_delete", null);
            }
            if (!this.attributes.ContainsKey("change_password"))
            {
                this.attributes.Add("change_password", null);
            }
            if (!this.attributes.ContainsKey("change_password_confirmation"))
            {
                this.attributes.Add("change_password_confirmation", null);
            }
            if (!this.attributes.ContainsKey("grant_permission"))
            {
                this.attributes.Add("grant_permission", null);
            }
            if (!this.attributes.ContainsKey("group_id"))
            {
                this.attributes.Add("group_id", null);
            }
            if (!this.attributes.ContainsKey("imported_password_hash"))
            {
                this.attributes.Add("imported_password_hash", null);
            }
            if (!this.attributes.ContainsKey("password"))
            {
                this.attributes.Add("password", null);
            }
            if (!this.attributes.ContainsKey("password_confirmation"))
            {
                this.attributes.Add("password_confirmation", null);
            }
            if (!this.attributes.ContainsKey("announcements_read"))
            {
                this.attributes.Add("announcements_read", null);
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
        /// User ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// User's username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// List of group IDs of which this user is an administrator
        /// </summary>
        [JsonPropertyName("admin_group_ids")]
        public Nullable<Int64>[] AdminGroupIds
        {
            get { return (Nullable<Int64>[])attributes["admin_group_ids"]; }
            set { attributes["admin_group_ids"] = value; }
        }

        /// <summary>
        /// A list of allowed IPs if applicable.  Newline delimited
        /// </summary>
        [JsonPropertyName("allowed_ips")]
        public string AllowedIps
        {
            get { return (string)attributes["allowed_ips"]; }
            set { attributes["allowed_ips"] = value; }
        }

        /// <summary>
        /// DEPRECATED: Can the user create Bundles (aka Share Links)? Use the bundle permission instead.
        /// </summary>
        [JsonPropertyName("attachments_permission")]
        public bool AttachmentsPermission
        {
            get { return (bool)attributes["attachments_permission"]; }
            set { attributes["attachments_permission"] = value; }
        }

        /// <summary>
        /// Number of api keys associated with this user
        /// </summary>
        [JsonPropertyName("api_keys_count")]
        public Nullable<Int64> ApiKeysCount
        {
            get { return (Nullable<Int64>)attributes["api_keys_count"]; }
            set { attributes["api_keys_count"] = value; }
        }

        /// <summary>
        /// Scheduled Date/Time at which user will be deactivated
        /// </summary>
        [JsonPropertyName("authenticate_until")]
        public Nullable<DateTime> AuthenticateUntil
        {
            get { return (Nullable<DateTime>)attributes["authenticate_until"]; }
            set { attributes["authenticate_until"] = value; }
        }

        /// <summary>
        /// How is this user authenticated?
        /// </summary>
        [JsonPropertyName("authentication_method")]
        public string AuthenticationMethod
        {
            get { return (string)attributes["authentication_method"]; }
            set { attributes["authentication_method"] = value; }
        }

        /// <summary>
        /// URL holding the user's avatar
        /// </summary>
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl
        {
            get { return (string)attributes["avatar_url"]; }
            set { attributes["avatar_url"] = value; }
        }

        /// <summary>
        /// Allow this user to perform operations on the account, payments, and invoices?
        /// </summary>
        [JsonPropertyName("billing_permission")]
        public bool BillingPermission
        {
            get { return (bool)attributes["billing_permission"]; }
            set { attributes["billing_permission"] = value; }
        }

        /// <summary>
        /// Allow this user to skip site-wide IP blacklists?
        /// </summary>
        [JsonPropertyName("bypass_site_allowed_ips")]
        public bool BypassSiteAllowedIps
        {
            get { return (bool)attributes["bypass_site_allowed_ips"]; }
            set { attributes["bypass_site_allowed_ips"] = value; }
        }

        /// <summary>
        /// Exempt this user from being disabled based on inactivity?
        /// </summary>
        [JsonPropertyName("bypass_inactive_disable")]
        public bool BypassInactiveDisable
        {
            get { return (bool)attributes["bypass_inactive_disable"]; }
            set { attributes["bypass_inactive_disable"] = value; }
        }

        /// <summary>
        /// When this user was created
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Can the user connect with WebDAV?
        /// </summary>
        [JsonPropertyName("dav_permission")]
        public bool DavPermission
        {
            get { return (bool)attributes["dav_permission"]; }
            set { attributes["dav_permission"] = value; }
        }

        /// <summary>
        /// Is user disabled? Disabled users cannot log in, and do not count for billing purposes.  Users can be automatically disabled after an inactivity period via a Site setting.
        /// </summary>
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; }
        }

        /// <summary>
        /// User email address
        /// </summary>
        [JsonPropertyName("email")]
        public string Email
        {
            get { return (string)attributes["email"]; }
            set { attributes["email"] = value; }
        }

        /// <summary>
        /// User's first login time
        /// </summary>
        [JsonPropertyName("first_login_at")]
        public Nullable<DateTime> FirstLoginAt
        {
            get { return (Nullable<DateTime>)attributes["first_login_at"]; }
            set { attributes["first_login_at"] = value; }
        }

        /// <summary>
        /// Can the user access with FTP/FTPS?
        /// </summary>
        [JsonPropertyName("ftp_permission")]
        public bool FtpPermission
        {
            get { return (bool)attributes["ftp_permission"]; }
            set { attributes["ftp_permission"] = value; }
        }

        /// <summary>
        /// Comma-separated list of group IDs of which this user is a member
        /// </summary>
        [JsonPropertyName("group_ids")]
        public string GroupIds
        {
            get { return (string)attributes["group_ids"]; }
            set { attributes["group_ids"] = value; }
        }

        /// <summary>
        /// Text to display to the user in the header of the UI
        /// </summary>
        [JsonPropertyName("header_text")]
        public string HeaderText
        {
            get { return (string)attributes["header_text"]; }
            set { attributes["header_text"] = value; }
        }

        /// <summary>
        /// Preferred language
        /// </summary>
        [JsonPropertyName("language")]
        public string Language
        {
            get { return (string)attributes["language"]; }
            set { attributes["language"] = value; }
        }

        /// <summary>
        /// User's most recent login time via any protocol
        /// </summary>
        [JsonPropertyName("last_login_at")]
        public Nullable<DateTime> LastLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_login_at"]; }
            set { attributes["last_login_at"] = value; }
        }

        /// <summary>
        /// User's most recent login time via web
        /// </summary>
        [JsonPropertyName("last_web_login_at")]
        public Nullable<DateTime> LastWebLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_web_login_at"]; }
            set { attributes["last_web_login_at"] = value; }
        }

        /// <summary>
        /// User's most recent login time via FTP
        /// </summary>
        [JsonPropertyName("last_ftp_login_at")]
        public Nullable<DateTime> LastFtpLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_ftp_login_at"]; }
            set { attributes["last_ftp_login_at"] = value; }
        }

        /// <summary>
        /// User's most recent login time via SFTP
        /// </summary>
        [JsonPropertyName("last_sftp_login_at")]
        public Nullable<DateTime> LastSftpLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_sftp_login_at"]; }
            set { attributes["last_sftp_login_at"] = value; }
        }

        /// <summary>
        /// User's most recent login time via WebDAV
        /// </summary>
        [JsonPropertyName("last_dav_login_at")]
        public Nullable<DateTime> LastDavLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_dav_login_at"]; }
            set { attributes["last_dav_login_at"] = value; }
        }

        /// <summary>
        /// User's most recent login time via Desktop app
        /// </summary>
        [JsonPropertyName("last_desktop_login_at")]
        public Nullable<DateTime> LastDesktopLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_desktop_login_at"]; }
            set { attributes["last_desktop_login_at"] = value; }
        }

        /// <summary>
        /// User's most recent login time via Rest API
        /// </summary>
        [JsonPropertyName("last_restapi_login_at")]
        public Nullable<DateTime> LastRestapiLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_restapi_login_at"]; }
            set { attributes["last_restapi_login_at"] = value; }
        }

        /// <summary>
        /// User's most recent API use time
        /// </summary>
        [JsonPropertyName("last_api_use_at")]
        public Nullable<DateTime> LastApiUseAt
        {
            get { return (Nullable<DateTime>)attributes["last_api_use_at"]; }
            set { attributes["last_api_use_at"] = value; }
        }

        /// <summary>
        /// User's most recent activity time, which is the latest of most recent login, most recent API use, enablement, or creation
        /// </summary>
        [JsonPropertyName("last_active_at")]
        public Nullable<DateTime> LastActiveAt
        {
            get { return (Nullable<DateTime>)attributes["last_active_at"]; }
            set { attributes["last_active_at"] = value; }
        }

        /// <summary>
        /// The most recent protocol and cipher used
        /// </summary>
        [JsonPropertyName("last_protocol_cipher")]
        public string LastProtocolCipher
        {
            get { return (string)attributes["last_protocol_cipher"]; }
            set { attributes["last_protocol_cipher"] = value; }
        }

        /// <summary>
        /// Time in the future that the user will no longer be locked out if applicable
        /// </summary>
        [JsonPropertyName("lockout_expires")]
        public Nullable<DateTime> LockoutExpires
        {
            get { return (Nullable<DateTime>)attributes["lockout_expires"]; }
            set { attributes["lockout_expires"] = value; }
        }

        /// <summary>
        /// User's full name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// User's company
        /// </summary>
        [JsonPropertyName("company")]
        public string Company
        {
            get { return (string)attributes["company"]; }
            set { attributes["company"] = value; }
        }

        /// <summary>
        /// Any internal notes on the user
        /// </summary>
        [JsonPropertyName("notes")]
        public string Notes
        {
            get { return (string)attributes["notes"]; }
            set { attributes["notes"] = value; }
        }

        /// <summary>
        /// Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
        /// </summary>
        [JsonPropertyName("notification_daily_send_time")]
        public Nullable<Int64> NotificationDailySendTime
        {
            get { return (Nullable<Int64>)attributes["notification_daily_send_time"]; }
            set { attributes["notification_daily_send_time"] = value; }
        }

        /// <summary>
        /// Enable integration with Office for the web?
        /// </summary>
        [JsonPropertyName("office_integration_enabled")]
        public bool OfficeIntegrationEnabled
        {
            get { return (bool)attributes["office_integration_enabled"]; }
            set { attributes["office_integration_enabled"] = value; }
        }

        /// <summary>
        /// Last time the user's password was set
        /// </summary>
        [JsonPropertyName("password_set_at")]
        public Nullable<DateTime> PasswordSetAt
        {
            get { return (Nullable<DateTime>)attributes["password_set_at"]; }
            set { attributes["password_set_at"] = value; }
        }

        /// <summary>
        /// Number of days to allow user to use the same password
        /// </summary>
        [JsonPropertyName("password_validity_days")]
        public Nullable<Int64> PasswordValidityDays
        {
            get { return (Nullable<Int64>)attributes["password_validity_days"]; }
            set { attributes["password_validity_days"] = value; }
        }

        /// <summary>
        /// Number of public keys associated with this user
        /// </summary>
        [JsonPropertyName("public_keys_count")]
        public Nullable<Int64> PublicKeysCount
        {
            get { return (Nullable<Int64>)attributes["public_keys_count"]; }
            set { attributes["public_keys_count"] = value; }
        }

        /// <summary>
        /// Should the user receive admin alerts such a certificate expiration notifications and overages?
        /// </summary>
        [JsonPropertyName("receive_admin_alerts")]
        public bool ReceiveAdminAlerts
        {
            get { return (bool)attributes["receive_admin_alerts"]; }
            set { attributes["receive_admin_alerts"] = value; }
        }

        /// <summary>
        /// 2FA required setting
        /// </summary>
        [JsonPropertyName("require_2fa")]
        public string Require2fa
        {
            get { return (string)attributes["require_2fa"]; }
            set { attributes["require_2fa"] = value; }
        }

        /// <summary>
        /// Is 2fa active for the user?
        /// </summary>
        [JsonPropertyName("active_2fa")]
        public bool Active2fa
        {
            get { return (bool)attributes["active_2fa"]; }
            set { attributes["active_2fa"] = value; }
        }

        /// <summary>
        /// Is a password change required upon next user login?
        /// </summary>
        [JsonPropertyName("require_password_change")]
        public bool RequirePasswordChange
        {
            get { return (bool)attributes["require_password_change"]; }
            set { attributes["require_password_change"] = value; }
        }

        /// <summary>
        /// Is user's password expired?
        /// </summary>
        [JsonPropertyName("password_expired")]
        public bool PasswordExpired
        {
            get { return (bool)attributes["password_expired"]; }
            set { attributes["password_expired"] = value; }
        }

        /// <summary>
        /// Can this user access the REST API?
        /// </summary>
        [JsonPropertyName("restapi_permission")]
        public bool RestapiPermission
        {
            get { return (bool)attributes["restapi_permission"]; }
            set { attributes["restapi_permission"] = value; }
        }

        /// <summary>
        /// Does this user manage it's own credentials or is it a shared/bot user?
        /// </summary>
        [JsonPropertyName("self_managed")]
        public bool SelfManaged
        {
            get { return (bool)attributes["self_managed"]; }
            set { attributes["self_managed"] = value; }
        }

        /// <summary>
        /// Can the user access with SFTP?
        /// </summary>
        [JsonPropertyName("sftp_permission")]
        public bool SftpPermission
        {
            get { return (bool)attributes["sftp_permission"]; }
            set { attributes["sftp_permission"] = value; }
        }

        /// <summary>
        /// Is the user an administrator for this site?
        /// </summary>
        [JsonPropertyName("site_admin")]
        public bool SiteAdmin
        {
            get { return (bool)attributes["site_admin"]; }
            set { attributes["site_admin"] = value; }
        }

        /// <summary>
        /// Skip Welcome page in the UI?
        /// </summary>
        [JsonPropertyName("skip_welcome_screen")]
        public bool SkipWelcomeScreen
        {
            get { return (bool)attributes["skip_welcome_screen"]; }
            set { attributes["skip_welcome_screen"] = value; }
        }

        /// <summary>
        /// SSL required setting
        /// </summary>
        [JsonPropertyName("ssl_required")]
        public string SslRequired
        {
            get { return (string)attributes["ssl_required"]; }
            set { attributes["ssl_required"] = value; }
        }

        /// <summary>
        /// SSO (Single Sign On) strategy ID for the user, if applicable.
        /// </summary>
        [JsonPropertyName("sso_strategy_id")]
        public Nullable<Int64> SsoStrategyId
        {
            get { return (Nullable<Int64>)attributes["sso_strategy_id"]; }
            set { attributes["sso_strategy_id"] = value; }
        }

        /// <summary>
        /// Is the user subscribed to the newsletter?
        /// </summary>
        [JsonPropertyName("subscribe_to_newsletter")]
        public bool SubscribeToNewsletter
        {
            get { return (bool)attributes["subscribe_to_newsletter"]; }
            set { attributes["subscribe_to_newsletter"] = value; }
        }

        /// <summary>
        /// Is this user managed by a SsoStrategy?
        /// </summary>
        [JsonPropertyName("externally_managed")]
        public bool ExternallyManaged
        {
            get { return (bool)attributes["externally_managed"]; }
            set { attributes["externally_managed"] = value; }
        }

        /// <summary>
        /// User time zone
        /// </summary>
        [JsonPropertyName("time_zone")]
        public string TimeZone
        {
            get { return (string)attributes["time_zone"]; }
            set { attributes["time_zone"] = value; }
        }

        /// <summary>
        /// Type(s) of 2FA methods in use.  Will be either `sms`, `totp`, `u2f`, `yubi`, or multiple values sorted alphabetically and joined by an underscore.
        /// </summary>
        [JsonPropertyName("type_of_2fa")]
        public string TypeOf2fa
        {
            get { return (string)attributes["type_of_2fa"]; }
            set { attributes["type_of_2fa"] = value; }
        }

        /// <summary>
        /// Root folder for FTP (and optionally SFTP if the appropriate site-wide setting is set.)  Note that this is not used for API, Desktop, or Web interface.
        /// </summary>
        [JsonPropertyName("user_root")]
        public string UserRoot
        {
            get { return (string)attributes["user_root"]; }
            set { attributes["user_root"] = value; }
        }

        /// <summary>
        /// Number of days remaining until password expires
        /// </summary>
        [JsonPropertyName("days_remaining_until_password_expire")]
        public Nullable<Int64> DaysRemainingUntilPasswordExpire
        {
            get { return (Nullable<Int64>)attributes["days_remaining_until_password_expire"]; }
            set { attributes["days_remaining_until_password_expire"] = value; }
        }

        /// <summary>
        /// Password expiration datetime
        /// </summary>
        [JsonPropertyName("password_expire_at")]
        public Nullable<DateTime> PasswordExpireAt
        {
            get { return (Nullable<DateTime>)attributes["password_expire_at"]; }
            set { attributes["password_expire_at"] = value; }
        }

        /// <summary>
        /// An image file for your user avatar.
        /// </summary>
        [JsonPropertyName("avatar_file")]
        public System.Net.Http.ByteArrayContent AvatarFile
        {
            get { return (System.Net.Http.ByteArrayContent)attributes["avatar_file"]; }
            set { attributes["avatar_file"] = value; }
        }

        /// <summary>
        /// If true, the avatar will be deleted.
        /// </summary>
        [JsonPropertyName("avatar_delete")]
        public bool AvatarDelete
        {
            get { return (bool)attributes["avatar_delete"]; }
            set { attributes["avatar_delete"] = value; }
        }

        /// <summary>
        /// Used for changing a password on an existing user.
        /// </summary>
        [JsonPropertyName("change_password")]
        public string ChangePassword
        {
            get { return (string)attributes["change_password"]; }
            set { attributes["change_password"] = value; }
        }

        /// <summary>
        /// Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        /// </summary>
        [JsonPropertyName("change_password_confirmation")]
        public string ChangePasswordConfirmation
        {
            get { return (string)attributes["change_password_confirmation"]; }
            set { attributes["change_password_confirmation"] = value; }
        }

        /// <summary>
        /// Permission to grant on the user root.  Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
        /// </summary>
        [JsonPropertyName("grant_permission")]
        public string GrantPermission
        {
            get { return (string)attributes["grant_permission"]; }
            set { attributes["grant_permission"] = value; }
        }

        /// <summary>
        /// Group ID to associate this user with.
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>)attributes["group_id"]; }
            set { attributes["group_id"] = value; }
        }

        /// <summary>
        /// Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash menthods are MD5, SHA1, and SHA256.
        /// </summary>
        [JsonPropertyName("imported_password_hash")]
        public string ImportedPasswordHash
        {
            get { return (string)attributes["imported_password_hash"]; }
            set { attributes["imported_password_hash"] = value; }
        }

        /// <summary>
        /// User password.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string)attributes["password"]; }
            set { attributes["password"] = value; }
        }

        /// <summary>
        /// Optional, but if provided, we will ensure that it matches the value sent in `password`.
        /// </summary>
        [JsonPropertyName("password_confirmation")]
        public string PasswordConfirmation
        {
            get { return (string)attributes["password_confirmation"]; }
            set { attributes["password_confirmation"] = value; }
        }

        /// <summary>
        /// Signifies that the user has read all the announcements in the UI.
        /// </summary>
        [JsonPropertyName("announcements_read")]
        public bool AnnouncementsRead
        {
            get { return (bool)attributes["announcements_read"]; }
            set { attributes["announcements_read"] = value; }
        }

        /// <summary>
        /// Unlock user who has been locked out due to failed logins
        /// </summary>
        public async Task Unlock(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}/unlock", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Resend user welcome email
        /// </summary>
        public async Task ResendWelcomeEmail(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}/resend_welcome_email", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Trigger 2FA Reset process for user who has lost access to their existing 2FA methods
        /// </summary>
        public async Task User2faReset(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}/2fa/reset", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the user root.  Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
        ///   group_id - int64 - Group ID to associate this user with.
        ///   group_ids - string - A list of group ids to associate this user with.  Comma delimited.
        ///   imported_password_hash - string - Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash menthods are MD5, SHA1, and SHA256.
        ///   password - string - User password.
        ///   password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `password`.
        ///   announcements_read - boolean - Signifies that the user has read all the announcements in the UI.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   attachments_permission - boolean - DEPRECATED: Can the user create Bundles (aka Share Links)? Use the bundle permission instead.
        ///   authenticate_until - string - Scheduled Date/Time at which user will be deactivated
        ///   authentication_method - string - How is this user authenticated?
        ///   billing_permission - boolean - Allow this user to perform operations on the account, payments, and invoices?
        ///   bypass_inactive_disable - boolean - Exempt this user from being disabled based on inactivity?
        ///   bypass_site_allowed_ips - boolean - Allow this user to skip site-wide IP blacklists?
        ///   dav_permission - boolean - Can the user connect with WebDAV?
        ///   disabled - boolean - Is user disabled? Disabled users cannot log in, and do not count for billing purposes.  Users can be automatically disabled after an inactivity period via a Site setting.
        ///   ftp_permission - boolean - Can the user access with FTP/FTPS?
        ///   header_text - string - Text to display to the user in the header of the UI
        ///   language - string - Preferred language
        ///   notification_daily_send_time - int64 - Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
        ///   name - string - User's full name
        ///   company - string - User's company
        ///   notes - string - Any internal notes on the user
        ///   office_integration_enabled - boolean - Enable integration with Office for the web?
        ///   password_validity_days - int64 - Number of days to allow user to use the same password
        ///   receive_admin_alerts - boolean - Should the user receive admin alerts such a certificate expiration notifications and overages?
        ///   require_password_change - boolean - Is a password change required upon next user login?
        ///   restapi_permission - boolean - Can this user access the REST API?
        ///   self_managed - boolean - Does this user manage it's own credentials or is it a shared/bot user?
        ///   sftp_permission - boolean - Can the user access with SFTP?
        ///   site_admin - boolean - Is the user an administrator for this site?
        ///   skip_welcome_screen - boolean - Skip Welcome page in the UI?
        ///   ssl_required - string - SSL required setting
        ///   sso_strategy_id - int64 - SSO (Single Sign On) strategy ID for the user, if applicable.
        ///   subscribe_to_newsletter - boolean - Is the user subscribed to the newsletter?
        ///   require_2fa - string - 2FA required setting
        ///   time_zone - string - User time zone
        ///   user_root - string - Root folder for FTP (and optionally SFTP if the appropriate site-wide setting is set.)  Note that this is not used for API, Desktop, or Web interface.
        ///   username - string - User's username
        /// </summary>
        public async Task<User> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("avatar_file") && !(parameters["avatar_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: avatar_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"avatar_file\"]");
            }
            if (parameters.ContainsKey("avatar_delete") && !(parameters["avatar_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: avatar_delete must be of type bool", "parameters[\"avatar_delete\"]");
            }
            if (parameters.ContainsKey("change_password") && !(parameters["change_password"] is string))
            {
                throw new ArgumentException("Bad parameter: change_password must be of type string", "parameters[\"change_password\"]");
            }
            if (parameters.ContainsKey("change_password_confirmation") && !(parameters["change_password_confirmation"] is string))
            {
                throw new ArgumentException("Bad parameter: change_password_confirmation must be of type string", "parameters[\"change_password_confirmation\"]");
            }
            if (parameters.ContainsKey("email") && !(parameters["email"] is string))
            {
                throw new ArgumentException("Bad parameter: email must be of type string", "parameters[\"email\"]");
            }
            if (parameters.ContainsKey("grant_permission") && !(parameters["grant_permission"] is string))
            {
                throw new ArgumentException("Bad parameter: grant_permission must be of type string", "parameters[\"grant_permission\"]");
            }
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
            }
            if (parameters.ContainsKey("imported_password_hash") && !(parameters["imported_password_hash"] is string))
            {
                throw new ArgumentException("Bad parameter: imported_password_hash must be of type string", "parameters[\"imported_password_hash\"]");
            }
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("password_confirmation") && !(parameters["password_confirmation"] is string))
            {
                throw new ArgumentException("Bad parameter: password_confirmation must be of type string", "parameters[\"password_confirmation\"]");
            }
            if (parameters.ContainsKey("announcements_read") && !(parameters["announcements_read"] is bool))
            {
                throw new ArgumentException("Bad parameter: announcements_read must be of type bool", "parameters[\"announcements_read\"]");
            }
            if (parameters.ContainsKey("allowed_ips") && !(parameters["allowed_ips"] is string))
            {
                throw new ArgumentException("Bad parameter: allowed_ips must be of type string", "parameters[\"allowed_ips\"]");
            }
            if (parameters.ContainsKey("attachments_permission") && !(parameters["attachments_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: attachments_permission must be of type bool", "parameters[\"attachments_permission\"]");
            }
            if (parameters.ContainsKey("authenticate_until") && !(parameters["authenticate_until"] is string))
            {
                throw new ArgumentException("Bad parameter: authenticate_until must be of type string", "parameters[\"authenticate_until\"]");
            }
            if (parameters.ContainsKey("authentication_method") && !(parameters["authentication_method"] is string))
            {
                throw new ArgumentException("Bad parameter: authentication_method must be of type string", "parameters[\"authentication_method\"]");
            }
            if (parameters.ContainsKey("billing_permission") && !(parameters["billing_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: billing_permission must be of type bool", "parameters[\"billing_permission\"]");
            }
            if (parameters.ContainsKey("bypass_inactive_disable") && !(parameters["bypass_inactive_disable"] is bool))
            {
                throw new ArgumentException("Bad parameter: bypass_inactive_disable must be of type bool", "parameters[\"bypass_inactive_disable\"]");
            }
            if (parameters.ContainsKey("bypass_site_allowed_ips") && !(parameters["bypass_site_allowed_ips"] is bool))
            {
                throw new ArgumentException("Bad parameter: bypass_site_allowed_ips must be of type bool", "parameters[\"bypass_site_allowed_ips\"]");
            }
            if (parameters.ContainsKey("dav_permission") && !(parameters["dav_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: dav_permission must be of type bool", "parameters[\"dav_permission\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("ftp_permission") && !(parameters["ftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_permission must be of type bool", "parameters[\"ftp_permission\"]");
            }
            if (parameters.ContainsKey("header_text") && !(parameters["header_text"] is string))
            {
                throw new ArgumentException("Bad parameter: header_text must be of type string", "parameters[\"header_text\"]");
            }
            if (parameters.ContainsKey("language") && !(parameters["language"] is string))
            {
                throw new ArgumentException("Bad parameter: language must be of type string", "parameters[\"language\"]");
            }
            if (parameters.ContainsKey("notification_daily_send_time") && !(parameters["notification_daily_send_time"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: notification_daily_send_time must be of type Nullable<Int64>", "parameters[\"notification_daily_send_time\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("company") && !(parameters["company"] is string))
            {
                throw new ArgumentException("Bad parameter: company must be of type string", "parameters[\"company\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("office_integration_enabled") && !(parameters["office_integration_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: office_integration_enabled must be of type bool", "parameters[\"office_integration_enabled\"]");
            }
            if (parameters.ContainsKey("password_validity_days") && !(parameters["password_validity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: password_validity_days must be of type Nullable<Int64>", "parameters[\"password_validity_days\"]");
            }
            if (parameters.ContainsKey("receive_admin_alerts") && !(parameters["receive_admin_alerts"] is bool))
            {
                throw new ArgumentException("Bad parameter: receive_admin_alerts must be of type bool", "parameters[\"receive_admin_alerts\"]");
            }
            if (parameters.ContainsKey("require_password_change") && !(parameters["require_password_change"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_password_change must be of type bool", "parameters[\"require_password_change\"]");
            }
            if (parameters.ContainsKey("restapi_permission") && !(parameters["restapi_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: restapi_permission must be of type bool", "parameters[\"restapi_permission\"]");
            }
            if (parameters.ContainsKey("self_managed") && !(parameters["self_managed"] is bool))
            {
                throw new ArgumentException("Bad parameter: self_managed must be of type bool", "parameters[\"self_managed\"]");
            }
            if (parameters.ContainsKey("sftp_permission") && !(parameters["sftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_permission must be of type bool", "parameters[\"sftp_permission\"]");
            }
            if (parameters.ContainsKey("site_admin") && !(parameters["site_admin"] is bool))
            {
                throw new ArgumentException("Bad parameter: site_admin must be of type bool", "parameters[\"site_admin\"]");
            }
            if (parameters.ContainsKey("skip_welcome_screen") && !(parameters["skip_welcome_screen"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_welcome_screen must be of type bool", "parameters[\"skip_welcome_screen\"]");
            }
            if (parameters.ContainsKey("ssl_required") && !(parameters["ssl_required"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl_required must be of type string", "parameters[\"ssl_required\"]");
            }
            if (parameters.ContainsKey("sso_strategy_id") && !(parameters["sso_strategy_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: sso_strategy_id must be of type Nullable<Int64>", "parameters[\"sso_strategy_id\"]");
            }
            if (parameters.ContainsKey("subscribe_to_newsletter") && !(parameters["subscribe_to_newsletter"] is bool))
            {
                throw new ArgumentException("Bad parameter: subscribe_to_newsletter must be of type bool", "parameters[\"subscribe_to_newsletter\"]");
            }
            if (parameters.ContainsKey("require_2fa") && !(parameters["require_2fa"] is string))
            {
                throw new ArgumentException("Bad parameter: require_2fa must be of type string", "parameters[\"require_2fa\"]");
            }
            if (parameters.ContainsKey("time_zone") && !(parameters["time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: time_zone must be of type string", "parameters[\"time_zone\"]");
            }
            if (parameters.ContainsKey("user_root") && !(parameters["user_root"] is string))
            {
                throw new ArgumentException("Bad parameter: user_root must be of type string", "parameters[\"user_root\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<User>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                await this.Update(this.attributes);
            }
            else
            {
                var newObj = await User.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[authenticate_until]=desc`). Valid fields are `authenticate_until`, `email`, `last_desktop_login_at`, `last_login_at`, `username`, `company`, `name`, `site_admin`, `receive_admin_alerts`, `password_validity_days`, `ssl_required` or `not_site_admin`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `username`, `email`, `company`, `site_admin`, `password_validity_days`, `ssl_required`, `last_login_at`, `authenticate_until` or `not_site_admin`. Valid field combinations are `[ not_site_admin, username ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `username`, `email` or `company`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `password_validity_days`, `last_login_at` or `authenticate_until`.
        ///   ids - string - comma-separated list of User IDs
        ///   q[username] - string - List users matching username.
        ///   q[email] - string - List users matching email.
        ///   q[notes] - string - List users matching notes field.
        ///   q[admin] - string - If `true`, list only admin users.
        ///   q[allowed_ips] - string - If set, list only users with overridden allowed IP setting.
        ///   q[password_validity_days] - string - If set, list only users with overridden password validity days setting.
        ///   q[ssl_required] - string - If set, list only users with overridden SSL required setting.
        ///   search - string - Searches for partial matches of name, username, or email.
        /// </summary>
        public static async Task<User[]> List(

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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }
            if (parameters.ContainsKey("ids") && !(parameters["ids"] is string))
            {
                throw new ArgumentException("Bad parameter: ids must be of type string", "parameters[\"ids\"]");
            }
            if (parameters.ContainsKey("search") && !(parameters["search"] is string))
            {
                throw new ArgumentException("Bad parameter: search must be of type string", "parameters[\"search\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/users", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<User[]>(responseJson);
        }

        public static async Task<User[]> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - User ID.
        /// </summary>
        public static async Task<User> Find(
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

            string responseJson = await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<User>(responseJson);
        }

        public static async Task<User> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the user root.  Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
        ///   group_id - int64 - Group ID to associate this user with.
        ///   group_ids - string - A list of group ids to associate this user with.  Comma delimited.
        ///   imported_password_hash - string - Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash menthods are MD5, SHA1, and SHA256.
        ///   password - string - User password.
        ///   password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `password`.
        ///   announcements_read - boolean - Signifies that the user has read all the announcements in the UI.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   attachments_permission - boolean - DEPRECATED: Can the user create Bundles (aka Share Links)? Use the bundle permission instead.
        ///   authenticate_until - string - Scheduled Date/Time at which user will be deactivated
        ///   authentication_method - string - How is this user authenticated?
        ///   billing_permission - boolean - Allow this user to perform operations on the account, payments, and invoices?
        ///   bypass_inactive_disable - boolean - Exempt this user from being disabled based on inactivity?
        ///   bypass_site_allowed_ips - boolean - Allow this user to skip site-wide IP blacklists?
        ///   dav_permission - boolean - Can the user connect with WebDAV?
        ///   disabled - boolean - Is user disabled? Disabled users cannot log in, and do not count for billing purposes.  Users can be automatically disabled after an inactivity period via a Site setting.
        ///   ftp_permission - boolean - Can the user access with FTP/FTPS?
        ///   header_text - string - Text to display to the user in the header of the UI
        ///   language - string - Preferred language
        ///   notification_daily_send_time - int64 - Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
        ///   name - string - User's full name
        ///   company - string - User's company
        ///   notes - string - Any internal notes on the user
        ///   office_integration_enabled - boolean - Enable integration with Office for the web?
        ///   password_validity_days - int64 - Number of days to allow user to use the same password
        ///   receive_admin_alerts - boolean - Should the user receive admin alerts such a certificate expiration notifications and overages?
        ///   require_password_change - boolean - Is a password change required upon next user login?
        ///   restapi_permission - boolean - Can this user access the REST API?
        ///   self_managed - boolean - Does this user manage it's own credentials or is it a shared/bot user?
        ///   sftp_permission - boolean - Can the user access with SFTP?
        ///   site_admin - boolean - Is the user an administrator for this site?
        ///   skip_welcome_screen - boolean - Skip Welcome page in the UI?
        ///   ssl_required - string - SSL required setting
        ///   sso_strategy_id - int64 - SSO (Single Sign On) strategy ID for the user, if applicable.
        ///   subscribe_to_newsletter - boolean - Is the user subscribed to the newsletter?
        ///   require_2fa - string - 2FA required setting
        ///   time_zone - string - User time zone
        ///   user_root - string - Root folder for FTP (and optionally SFTP if the appropriate site-wide setting is set.)  Note that this is not used for API, Desktop, or Web interface.
        ///   username - string - User's username
        /// </summary>
        public static async Task<User> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("avatar_file") && !(parameters["avatar_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: avatar_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"avatar_file\"]");
            }
            if (parameters.ContainsKey("avatar_delete") && !(parameters["avatar_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: avatar_delete must be of type bool", "parameters[\"avatar_delete\"]");
            }
            if (parameters.ContainsKey("change_password") && !(parameters["change_password"] is string))
            {
                throw new ArgumentException("Bad parameter: change_password must be of type string", "parameters[\"change_password\"]");
            }
            if (parameters.ContainsKey("change_password_confirmation") && !(parameters["change_password_confirmation"] is string))
            {
                throw new ArgumentException("Bad parameter: change_password_confirmation must be of type string", "parameters[\"change_password_confirmation\"]");
            }
            if (parameters.ContainsKey("email") && !(parameters["email"] is string))
            {
                throw new ArgumentException("Bad parameter: email must be of type string", "parameters[\"email\"]");
            }
            if (parameters.ContainsKey("grant_permission") && !(parameters["grant_permission"] is string))
            {
                throw new ArgumentException("Bad parameter: grant_permission must be of type string", "parameters[\"grant_permission\"]");
            }
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
            }
            if (parameters.ContainsKey("imported_password_hash") && !(parameters["imported_password_hash"] is string))
            {
                throw new ArgumentException("Bad parameter: imported_password_hash must be of type string", "parameters[\"imported_password_hash\"]");
            }
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("password_confirmation") && !(parameters["password_confirmation"] is string))
            {
                throw new ArgumentException("Bad parameter: password_confirmation must be of type string", "parameters[\"password_confirmation\"]");
            }
            if (parameters.ContainsKey("announcements_read") && !(parameters["announcements_read"] is bool))
            {
                throw new ArgumentException("Bad parameter: announcements_read must be of type bool", "parameters[\"announcements_read\"]");
            }
            if (parameters.ContainsKey("allowed_ips") && !(parameters["allowed_ips"] is string))
            {
                throw new ArgumentException("Bad parameter: allowed_ips must be of type string", "parameters[\"allowed_ips\"]");
            }
            if (parameters.ContainsKey("attachments_permission") && !(parameters["attachments_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: attachments_permission must be of type bool", "parameters[\"attachments_permission\"]");
            }
            if (parameters.ContainsKey("authenticate_until") && !(parameters["authenticate_until"] is string))
            {
                throw new ArgumentException("Bad parameter: authenticate_until must be of type string", "parameters[\"authenticate_until\"]");
            }
            if (parameters.ContainsKey("authentication_method") && !(parameters["authentication_method"] is string))
            {
                throw new ArgumentException("Bad parameter: authentication_method must be of type string", "parameters[\"authentication_method\"]");
            }
            if (parameters.ContainsKey("billing_permission") && !(parameters["billing_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: billing_permission must be of type bool", "parameters[\"billing_permission\"]");
            }
            if (parameters.ContainsKey("bypass_inactive_disable") && !(parameters["bypass_inactive_disable"] is bool))
            {
                throw new ArgumentException("Bad parameter: bypass_inactive_disable must be of type bool", "parameters[\"bypass_inactive_disable\"]");
            }
            if (parameters.ContainsKey("bypass_site_allowed_ips") && !(parameters["bypass_site_allowed_ips"] is bool))
            {
                throw new ArgumentException("Bad parameter: bypass_site_allowed_ips must be of type bool", "parameters[\"bypass_site_allowed_ips\"]");
            }
            if (parameters.ContainsKey("dav_permission") && !(parameters["dav_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: dav_permission must be of type bool", "parameters[\"dav_permission\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("ftp_permission") && !(parameters["ftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_permission must be of type bool", "parameters[\"ftp_permission\"]");
            }
            if (parameters.ContainsKey("header_text") && !(parameters["header_text"] is string))
            {
                throw new ArgumentException("Bad parameter: header_text must be of type string", "parameters[\"header_text\"]");
            }
            if (parameters.ContainsKey("language") && !(parameters["language"] is string))
            {
                throw new ArgumentException("Bad parameter: language must be of type string", "parameters[\"language\"]");
            }
            if (parameters.ContainsKey("notification_daily_send_time") && !(parameters["notification_daily_send_time"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: notification_daily_send_time must be of type Nullable<Int64>", "parameters[\"notification_daily_send_time\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("company") && !(parameters["company"] is string))
            {
                throw new ArgumentException("Bad parameter: company must be of type string", "parameters[\"company\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("office_integration_enabled") && !(parameters["office_integration_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: office_integration_enabled must be of type bool", "parameters[\"office_integration_enabled\"]");
            }
            if (parameters.ContainsKey("password_validity_days") && !(parameters["password_validity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: password_validity_days must be of type Nullable<Int64>", "parameters[\"password_validity_days\"]");
            }
            if (parameters.ContainsKey("receive_admin_alerts") && !(parameters["receive_admin_alerts"] is bool))
            {
                throw new ArgumentException("Bad parameter: receive_admin_alerts must be of type bool", "parameters[\"receive_admin_alerts\"]");
            }
            if (parameters.ContainsKey("require_password_change") && !(parameters["require_password_change"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_password_change must be of type bool", "parameters[\"require_password_change\"]");
            }
            if (parameters.ContainsKey("restapi_permission") && !(parameters["restapi_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: restapi_permission must be of type bool", "parameters[\"restapi_permission\"]");
            }
            if (parameters.ContainsKey("self_managed") && !(parameters["self_managed"] is bool))
            {
                throw new ArgumentException("Bad parameter: self_managed must be of type bool", "parameters[\"self_managed\"]");
            }
            if (parameters.ContainsKey("sftp_permission") && !(parameters["sftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_permission must be of type bool", "parameters[\"sftp_permission\"]");
            }
            if (parameters.ContainsKey("site_admin") && !(parameters["site_admin"] is bool))
            {
                throw new ArgumentException("Bad parameter: site_admin must be of type bool", "parameters[\"site_admin\"]");
            }
            if (parameters.ContainsKey("skip_welcome_screen") && !(parameters["skip_welcome_screen"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_welcome_screen must be of type bool", "parameters[\"skip_welcome_screen\"]");
            }
            if (parameters.ContainsKey("ssl_required") && !(parameters["ssl_required"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl_required must be of type string", "parameters[\"ssl_required\"]");
            }
            if (parameters.ContainsKey("sso_strategy_id") && !(parameters["sso_strategy_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: sso_strategy_id must be of type Nullable<Int64>", "parameters[\"sso_strategy_id\"]");
            }
            if (parameters.ContainsKey("subscribe_to_newsletter") && !(parameters["subscribe_to_newsletter"] is bool))
            {
                throw new ArgumentException("Bad parameter: subscribe_to_newsletter must be of type bool", "parameters[\"subscribe_to_newsletter\"]");
            }
            if (parameters.ContainsKey("require_2fa") && !(parameters["require_2fa"] is string))
            {
                throw new ArgumentException("Bad parameter: require_2fa must be of type string", "parameters[\"require_2fa\"]");
            }
            if (parameters.ContainsKey("time_zone") && !(parameters["time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: time_zone must be of type string", "parameters[\"time_zone\"]");
            }
            if (parameters.ContainsKey("user_root") && !(parameters["user_root"] is string))
            {
                throw new ArgumentException("Bad parameter: user_root must be of type string", "parameters[\"user_root\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/users", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<User>(responseJson);
        }


        /// <summary>
        /// Unlock user who has been locked out due to failed logins
        /// </summary>
        public static async Task Unlock(
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

            await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}/unlock", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Resend user welcome email
        /// </summary>
        public static async Task ResendWelcomeEmail(
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

            await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}/resend_welcome_email", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Trigger 2FA Reset process for user who has lost access to their existing 2FA methods
        /// </summary>
        public static async Task User2faReset(
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

            await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}/2fa/reset", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the user root.  Can be blank or `full`, `read`, `write`, `list`, `read+write`, or `list+write`
        ///   group_id - int64 - Group ID to associate this user with.
        ///   group_ids - string - A list of group ids to associate this user with.  Comma delimited.
        ///   imported_password_hash - string - Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash menthods are MD5, SHA1, and SHA256.
        ///   password - string - User password.
        ///   password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `password`.
        ///   announcements_read - boolean - Signifies that the user has read all the announcements in the UI.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   attachments_permission - boolean - DEPRECATED: Can the user create Bundles (aka Share Links)? Use the bundle permission instead.
        ///   authenticate_until - string - Scheduled Date/Time at which user will be deactivated
        ///   authentication_method - string - How is this user authenticated?
        ///   billing_permission - boolean - Allow this user to perform operations on the account, payments, and invoices?
        ///   bypass_inactive_disable - boolean - Exempt this user from being disabled based on inactivity?
        ///   bypass_site_allowed_ips - boolean - Allow this user to skip site-wide IP blacklists?
        ///   dav_permission - boolean - Can the user connect with WebDAV?
        ///   disabled - boolean - Is user disabled? Disabled users cannot log in, and do not count for billing purposes.  Users can be automatically disabled after an inactivity period via a Site setting.
        ///   ftp_permission - boolean - Can the user access with FTP/FTPS?
        ///   header_text - string - Text to display to the user in the header of the UI
        ///   language - string - Preferred language
        ///   notification_daily_send_time - int64 - Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
        ///   name - string - User's full name
        ///   company - string - User's company
        ///   notes - string - Any internal notes on the user
        ///   office_integration_enabled - boolean - Enable integration with Office for the web?
        ///   password_validity_days - int64 - Number of days to allow user to use the same password
        ///   receive_admin_alerts - boolean - Should the user receive admin alerts such a certificate expiration notifications and overages?
        ///   require_password_change - boolean - Is a password change required upon next user login?
        ///   restapi_permission - boolean - Can this user access the REST API?
        ///   self_managed - boolean - Does this user manage it's own credentials or is it a shared/bot user?
        ///   sftp_permission - boolean - Can the user access with SFTP?
        ///   site_admin - boolean - Is the user an administrator for this site?
        ///   skip_welcome_screen - boolean - Skip Welcome page in the UI?
        ///   ssl_required - string - SSL required setting
        ///   sso_strategy_id - int64 - SSO (Single Sign On) strategy ID for the user, if applicable.
        ///   subscribe_to_newsletter - boolean - Is the user subscribed to the newsletter?
        ///   require_2fa - string - 2FA required setting
        ///   time_zone - string - User time zone
        ///   user_root - string - Root folder for FTP (and optionally SFTP if the appropriate site-wide setting is set.)  Note that this is not used for API, Desktop, or Web interface.
        ///   username - string - User's username
        /// </summary>
        public static async Task<User> Update(
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
            if (parameters.ContainsKey("avatar_file") && !(parameters["avatar_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: avatar_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"avatar_file\"]");
            }
            if (parameters.ContainsKey("avatar_delete") && !(parameters["avatar_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: avatar_delete must be of type bool", "parameters[\"avatar_delete\"]");
            }
            if (parameters.ContainsKey("change_password") && !(parameters["change_password"] is string))
            {
                throw new ArgumentException("Bad parameter: change_password must be of type string", "parameters[\"change_password\"]");
            }
            if (parameters.ContainsKey("change_password_confirmation") && !(parameters["change_password_confirmation"] is string))
            {
                throw new ArgumentException("Bad parameter: change_password_confirmation must be of type string", "parameters[\"change_password_confirmation\"]");
            }
            if (parameters.ContainsKey("email") && !(parameters["email"] is string))
            {
                throw new ArgumentException("Bad parameter: email must be of type string", "parameters[\"email\"]");
            }
            if (parameters.ContainsKey("grant_permission") && !(parameters["grant_permission"] is string))
            {
                throw new ArgumentException("Bad parameter: grant_permission must be of type string", "parameters[\"grant_permission\"]");
            }
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
            }
            if (parameters.ContainsKey("imported_password_hash") && !(parameters["imported_password_hash"] is string))
            {
                throw new ArgumentException("Bad parameter: imported_password_hash must be of type string", "parameters[\"imported_password_hash\"]");
            }
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("password_confirmation") && !(parameters["password_confirmation"] is string))
            {
                throw new ArgumentException("Bad parameter: password_confirmation must be of type string", "parameters[\"password_confirmation\"]");
            }
            if (parameters.ContainsKey("announcements_read") && !(parameters["announcements_read"] is bool))
            {
                throw new ArgumentException("Bad parameter: announcements_read must be of type bool", "parameters[\"announcements_read\"]");
            }
            if (parameters.ContainsKey("allowed_ips") && !(parameters["allowed_ips"] is string))
            {
                throw new ArgumentException("Bad parameter: allowed_ips must be of type string", "parameters[\"allowed_ips\"]");
            }
            if (parameters.ContainsKey("attachments_permission") && !(parameters["attachments_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: attachments_permission must be of type bool", "parameters[\"attachments_permission\"]");
            }
            if (parameters.ContainsKey("authenticate_until") && !(parameters["authenticate_until"] is string))
            {
                throw new ArgumentException("Bad parameter: authenticate_until must be of type string", "parameters[\"authenticate_until\"]");
            }
            if (parameters.ContainsKey("authentication_method") && !(parameters["authentication_method"] is string))
            {
                throw new ArgumentException("Bad parameter: authentication_method must be of type string", "parameters[\"authentication_method\"]");
            }
            if (parameters.ContainsKey("billing_permission") && !(parameters["billing_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: billing_permission must be of type bool", "parameters[\"billing_permission\"]");
            }
            if (parameters.ContainsKey("bypass_inactive_disable") && !(parameters["bypass_inactive_disable"] is bool))
            {
                throw new ArgumentException("Bad parameter: bypass_inactive_disable must be of type bool", "parameters[\"bypass_inactive_disable\"]");
            }
            if (parameters.ContainsKey("bypass_site_allowed_ips") && !(parameters["bypass_site_allowed_ips"] is bool))
            {
                throw new ArgumentException("Bad parameter: bypass_site_allowed_ips must be of type bool", "parameters[\"bypass_site_allowed_ips\"]");
            }
            if (parameters.ContainsKey("dav_permission") && !(parameters["dav_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: dav_permission must be of type bool", "parameters[\"dav_permission\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("ftp_permission") && !(parameters["ftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_permission must be of type bool", "parameters[\"ftp_permission\"]");
            }
            if (parameters.ContainsKey("header_text") && !(parameters["header_text"] is string))
            {
                throw new ArgumentException("Bad parameter: header_text must be of type string", "parameters[\"header_text\"]");
            }
            if (parameters.ContainsKey("language") && !(parameters["language"] is string))
            {
                throw new ArgumentException("Bad parameter: language must be of type string", "parameters[\"language\"]");
            }
            if (parameters.ContainsKey("notification_daily_send_time") && !(parameters["notification_daily_send_time"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: notification_daily_send_time must be of type Nullable<Int64>", "parameters[\"notification_daily_send_time\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("company") && !(parameters["company"] is string))
            {
                throw new ArgumentException("Bad parameter: company must be of type string", "parameters[\"company\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("office_integration_enabled") && !(parameters["office_integration_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: office_integration_enabled must be of type bool", "parameters[\"office_integration_enabled\"]");
            }
            if (parameters.ContainsKey("password_validity_days") && !(parameters["password_validity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: password_validity_days must be of type Nullable<Int64>", "parameters[\"password_validity_days\"]");
            }
            if (parameters.ContainsKey("receive_admin_alerts") && !(parameters["receive_admin_alerts"] is bool))
            {
                throw new ArgumentException("Bad parameter: receive_admin_alerts must be of type bool", "parameters[\"receive_admin_alerts\"]");
            }
            if (parameters.ContainsKey("require_password_change") && !(parameters["require_password_change"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_password_change must be of type bool", "parameters[\"require_password_change\"]");
            }
            if (parameters.ContainsKey("restapi_permission") && !(parameters["restapi_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: restapi_permission must be of type bool", "parameters[\"restapi_permission\"]");
            }
            if (parameters.ContainsKey("self_managed") && !(parameters["self_managed"] is bool))
            {
                throw new ArgumentException("Bad parameter: self_managed must be of type bool", "parameters[\"self_managed\"]");
            }
            if (parameters.ContainsKey("sftp_permission") && !(parameters["sftp_permission"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_permission must be of type bool", "parameters[\"sftp_permission\"]");
            }
            if (parameters.ContainsKey("site_admin") && !(parameters["site_admin"] is bool))
            {
                throw new ArgumentException("Bad parameter: site_admin must be of type bool", "parameters[\"site_admin\"]");
            }
            if (parameters.ContainsKey("skip_welcome_screen") && !(parameters["skip_welcome_screen"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_welcome_screen must be of type bool", "parameters[\"skip_welcome_screen\"]");
            }
            if (parameters.ContainsKey("ssl_required") && !(parameters["ssl_required"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl_required must be of type string", "parameters[\"ssl_required\"]");
            }
            if (parameters.ContainsKey("sso_strategy_id") && !(parameters["sso_strategy_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: sso_strategy_id must be of type Nullable<Int64>", "parameters[\"sso_strategy_id\"]");
            }
            if (parameters.ContainsKey("subscribe_to_newsletter") && !(parameters["subscribe_to_newsletter"] is bool))
            {
                throw new ArgumentException("Bad parameter: subscribe_to_newsletter must be of type bool", "parameters[\"subscribe_to_newsletter\"]");
            }
            if (parameters.ContainsKey("require_2fa") && !(parameters["require_2fa"] is string))
            {
                throw new ArgumentException("Bad parameter: require_2fa must be of type string", "parameters[\"require_2fa\"]");
            }
            if (parameters.ContainsKey("time_zone") && !(parameters["time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: time_zone must be of type string", "parameters[\"time_zone\"]");
            }
            if (parameters.ContainsKey("user_root") && !(parameters["user_root"] is string))
            {
                throw new ArgumentException("Bad parameter: user_root must be of type string", "parameters[\"user_root\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<User>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task Delete(
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

            await FilesClient.SendRequest($"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(id, parameters, options);
        }

    }
}