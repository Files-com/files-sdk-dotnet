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
    /// UserEntity model
    /// </summary>
    public class User : IModel, IJsonOnDeserialized
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        // The names of the attributes set on this object, by its constructor or its properties, since it was made
        // or last came from Files.com. SaveAsync sends these, never the defaults of attributes nobody set.
        private readonly HashSet<string> unsaved = new HashSet<string>();
        /// <summary>
        /// Makes a User with no attributes set and no options.
        /// </summary>
        public User() : this(null, null) { }

        /// <summary>
        /// Makes a User with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public User(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

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
                this.attributes.Add("attachments_permission", false);
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
            if (!this.attributes.ContainsKey("billable"))
            {
                this.attributes.Add("billable", false);
            }
            if (!this.attributes.ContainsKey("billing_permission"))
            {
                this.attributes.Add("billing_permission", false);
            }
            if (!this.attributes.ContainsKey("bypass_site_allowed_ips"))
            {
                this.attributes.Add("bypass_site_allowed_ips", false);
            }
            if (!this.attributes.ContainsKey("bypass_user_lifecycle_rules"))
            {
                this.attributes.Add("bypass_user_lifecycle_rules", false);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("dav_permission"))
            {
                this.attributes.Add("dav_permission", false);
            }
            if (!this.attributes.ContainsKey("disabled"))
            {
                this.attributes.Add("disabled", false);
            }
            if (!this.attributes.ContainsKey("disabled_expired_or_inactive"))
            {
                this.attributes.Add("disabled_expired_or_inactive", false);
            }
            if (!this.attributes.ContainsKey("ai_assistant_personality_id"))
            {
                this.attributes.Add("ai_assistant_personality_id", null);
            }
            if (!this.attributes.ContainsKey("desktop_configuration_profile_id"))
            {
                this.attributes.Add("desktop_configuration_profile_id", null);
            }
            if (!this.attributes.ContainsKey("integration_centric_profile_id"))
            {
                this.attributes.Add("integration_centric_profile_id", null);
            }
            if (!this.attributes.ContainsKey("email"))
            {
                this.attributes.Add("email", "");
            }
            if (!this.attributes.ContainsKey("filesystem_layout"))
            {
                this.attributes.Add("filesystem_layout", null);
            }
            if (!this.attributes.ContainsKey("first_login_at"))
            {
                this.attributes.Add("first_login_at", null);
            }
            if (!this.attributes.ContainsKey("ftp_permission"))
            {
                this.attributes.Add("ftp_permission", false);
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
                this.attributes.Add("office_integration_enabled", false);
            }
            if (!this.attributes.ContainsKey("partner_admin"))
            {
                this.attributes.Add("partner_admin", false);
            }
            if (!this.attributes.ContainsKey("partner_id"))
            {
                this.attributes.Add("partner_id", null);
            }
            if (!this.attributes.ContainsKey("partner_name"))
            {
                this.attributes.Add("partner_name", null);
            }
            if (!this.attributes.ContainsKey("password_set_at"))
            {
                this.attributes.Add("password_set_at", null);
            }
            if (!this.attributes.ContainsKey("password_validity_days"))
            {
                this.attributes.Add("password_validity_days", null);
            }
            if (!this.attributes.ContainsKey("primary_group_id"))
            {
                this.attributes.Add("primary_group_id", null);
            }
            if (!this.attributes.ContainsKey("public_keys_count"))
            {
                this.attributes.Add("public_keys_count", null);
            }
            if (!this.attributes.ContainsKey("receive_admin_alerts"))
            {
                this.attributes.Add("receive_admin_alerts", false);
            }
            if (!this.attributes.ContainsKey("notify_on_all_site_warnings"))
            {
                this.attributes.Add("notify_on_all_site_warnings", false);
            }
            if (!this.attributes.ContainsKey("notify_on_all_sso_failures"))
            {
                this.attributes.Add("notify_on_all_sso_failures", false);
            }
            if (!this.attributes.ContainsKey("notify_on_all_user_security_events"))
            {
                this.attributes.Add("notify_on_all_user_security_events", false);
            }
            if (!this.attributes.ContainsKey("notify_on_all_pending_work_failures"))
            {
                this.attributes.Add("notify_on_all_pending_work_failures", false);
            }
            if (!this.attributes.ContainsKey("notify_on_all_siem_http_destination_failures"))
            {
                this.attributes.Add("notify_on_all_siem_http_destination_failures", false);
            }
            if (!this.attributes.ContainsKey("notify_on_all_sync_failures"))
            {
                this.attributes.Add("notify_on_all_sync_failures", false);
            }
            if (!this.attributes.ContainsKey("notify_on_all_automation_failures"))
            {
                this.attributes.Add("notify_on_all_automation_failures", false);
            }
            if (!this.attributes.ContainsKey("notify_on_all_expectation_failures"))
            {
                this.attributes.Add("notify_on_all_expectation_failures", false);
            }
            if (!this.attributes.ContainsKey("require_2fa"))
            {
                this.attributes.Add("require_2fa", null);
            }
            if (!this.attributes.ContainsKey("require_login_by"))
            {
                this.attributes.Add("require_login_by", null);
            }
            if (!this.attributes.ContainsKey("active_2fa"))
            {
                this.attributes.Add("active_2fa", false);
            }
            if (!this.attributes.ContainsKey("require_password_change"))
            {
                this.attributes.Add("require_password_change", false);
            }
            if (!this.attributes.ContainsKey("password_expired"))
            {
                this.attributes.Add("password_expired", false);
            }
            if (!this.attributes.ContainsKey("responsible_group_id"))
            {
                this.attributes.Add("responsible_group_id", null);
            }
            if (!this.attributes.ContainsKey("responsible_user_id"))
            {
                this.attributes.Add("responsible_user_id", null);
            }
            if (!this.attributes.ContainsKey("readonly_site_admin"))
            {
                this.attributes.Add("readonly_site_admin", false);
            }
            if (!this.attributes.ContainsKey("restapi_permission"))
            {
                this.attributes.Add("restapi_permission", false);
            }
            if (!this.attributes.ContainsKey("s3_compatible_endpoint_permission"))
            {
                this.attributes.Add("s3_compatible_endpoint_permission", false);
            }
            if (!this.attributes.ContainsKey("self_managed"))
            {
                this.attributes.Add("self_managed", false);
            }
            if (!this.attributes.ContainsKey("sftp_permission"))
            {
                this.attributes.Add("sftp_permission", false);
            }
            if (!this.attributes.ContainsKey("site_admin"))
            {
                this.attributes.Add("site_admin", false);
            }
            if (!this.attributes.ContainsKey("workspace_admin"))
            {
                this.attributes.Add("workspace_admin", false);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("default_workspace_id"))
            {
                this.attributes.Add("default_workspace_id", null);
            }
            if (!this.attributes.ContainsKey("skip_welcome_screen"))
            {
                this.attributes.Add("skip_welcome_screen", false);
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
                this.attributes.Add("subscribe_to_newsletter", false);
            }
            if (!this.attributes.ContainsKey("externally_managed"))
            {
                this.attributes.Add("externally_managed", false);
            }
            if (!this.attributes.ContainsKey("tags"))
            {
                this.attributes.Add("tags", null);
            }
            if (!this.attributes.ContainsKey("time_zone"))
            {
                this.attributes.Add("time_zone", null);
            }
            if (!this.attributes.ContainsKey("type_of_2fa"))
            {
                this.attributes.Add("type_of_2fa", null);
            }
            if (!this.attributes.ContainsKey("type_of_2fa_for_display"))
            {
                this.attributes.Add("type_of_2fa_for_display", null);
            }
            if (!this.attributes.ContainsKey("user_root"))
            {
                this.attributes.Add("user_root", null);
            }
            if (!this.attributes.ContainsKey("user_home"))
            {
                this.attributes.Add("user_home", null);
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
                this.attributes.Add("avatar_delete", false);
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
                this.attributes.Add("announcements_read", false);
            }
            if (!this.attributes.ContainsKey("clear_2fa"))
            {
                this.attributes.Add("clear_2fa", false);
            }
            if (!this.attributes.ContainsKey("convert_to_partner_user"))
            {
                this.attributes.Add("convert_to_partner_user", false);
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
            this.client = client;
            this.origin = origin;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => origin;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[0]; }
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

        // What is read from JSON, such as a response from Files.com, is not waiting to be saved.
        void IJsonOnDeserialized.OnDeserialized()
        {
            unsaved.Clear();
        }


        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// User's username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; unsaved.Add("username"); }
        }

        /// <summary>
        /// List of group IDs of which this user is an administrator
        /// </summary>
        [JsonPropertyName("admin_group_ids")]
        public Nullable<Int64>[] AdminGroupIds
        {
            get { return (Nullable<Int64>[])attributes["admin_group_ids"]; }
            set { attributes["admin_group_ids"] = value; unsaved.Add("admin_group_ids"); }
        }

        /// <summary>
        /// A list of allowed IPs if applicable.  Newline delimited
        /// </summary>
        [JsonPropertyName("allowed_ips")]
        public string AllowedIps
        {
            get { return (string)attributes["allowed_ips"]; }
            set { attributes["allowed_ips"] = value; unsaved.Add("allowed_ips"); }
        }

        /// <summary>
        /// If `true`, the user can user create Bundles (aka Share Links). Use the bundle permission instead.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("attachments_permission")]
        public bool AttachmentsPermission
        {
            get { return attributes["attachments_permission"] == null ? false : (bool)attributes["attachments_permission"]; }
            set { attributes["attachments_permission"] = value; unsaved.Add("attachments_permission"); }
        }

        /// <summary>
        /// Number of API keys associated with this user
        /// </summary>
        [JsonPropertyName("api_keys_count")]
        public Nullable<Int64> ApiKeysCount
        {
            get { return (Nullable<Int64>)attributes["api_keys_count"]; }
            set { attributes["api_keys_count"] = value; unsaved.Add("api_keys_count"); }
        }

        /// <summary>
        /// Scheduled Date/Time at which user will be deactivated
        /// </summary>
        [JsonPropertyName("authenticate_until")]
        public Nullable<DateTime> AuthenticateUntil
        {
            get { return (Nullable<DateTime>)attributes["authenticate_until"]; }
            set { attributes["authenticate_until"] = value; unsaved.Add("authenticate_until"); }
        }

        /// <summary>
        /// How is this user authenticated?
        /// </summary>
        [JsonPropertyName("authentication_method")]
        public string AuthenticationMethod
        {
            get { return (string)attributes["authentication_method"]; }
            set { attributes["authentication_method"] = value; unsaved.Add("authentication_method"); }
        }

        /// <summary>
        /// URL holding the user's avatar
        /// </summary>
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl
        {
            get { return (string)attributes["avatar_url"]; }
            set { attributes["avatar_url"] = value; unsaved.Add("avatar_url"); }
        }

        /// <summary>
        /// Is this a billable user record?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("billable")]
        public bool Billable
        {
            get { return attributes["billable"] == null ? false : (bool)attributes["billable"]; }
            set { attributes["billable"] = value; unsaved.Add("billable"); }
        }

        /// <summary>
        /// Allow this user to perform operations on the account, payments, and invoices?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("billing_permission")]
        public bool BillingPermission
        {
            get { return attributes["billing_permission"] == null ? false : (bool)attributes["billing_permission"]; }
            set { attributes["billing_permission"] = value; unsaved.Add("billing_permission"); }
        }

        /// <summary>
        /// Allow this user to skip site-wide IP blacklists?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bypass_site_allowed_ips")]
        public bool BypassSiteAllowedIps
        {
            get { return attributes["bypass_site_allowed_ips"] == null ? false : (bool)attributes["bypass_site_allowed_ips"]; }
            set { attributes["bypass_site_allowed_ips"] = value; unsaved.Add("bypass_site_allowed_ips"); }
        }

        /// <summary>
        /// Exempt this user from user lifecycle rules?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bypass_user_lifecycle_rules")]
        public bool BypassUserLifecycleRules
        {
            get { return attributes["bypass_user_lifecycle_rules"] == null ? false : (bool)attributes["bypass_user_lifecycle_rules"]; }
            set { attributes["bypass_user_lifecycle_rules"] = value; unsaved.Add("bypass_user_lifecycle_rules"); }
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
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dav_permission")]
        public bool DavPermission
        {
            get { return attributes["dav_permission"] == null ? false : (bool)attributes["dav_permission"]; }
            set { attributes["dav_permission"] = value; unsaved.Add("dav_permission"); }
        }

        /// <summary>
        /// Is user disabled? Disabled users cannot log in, and do not count for billing purposes. Users can be automatically disabled after an inactivity period via a Site setting or schedule to be deactivated after specific date.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; unsaved.Add("disabled"); }
        }

        /// <summary>
        /// Computed property that returns true if user disabled or expired or inactive.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled_expired_or_inactive")]
        public bool DisabledExpiredOrInactive
        {
            get { return attributes["disabled_expired_or_inactive"] == null ? false : (bool)attributes["disabled_expired_or_inactive"]; }
            set { attributes["disabled_expired_or_inactive"] = value; unsaved.Add("disabled_expired_or_inactive"); }
        }

        /// <summary>
        /// AI Assistant Personality ID assigned directly to this user, if any.
        /// </summary>
        [JsonPropertyName("ai_assistant_personality_id")]
        public Nullable<Int64> AiAssistantPersonalityId
        {
            get { return (Nullable<Int64>)attributes["ai_assistant_personality_id"]; }
            set { attributes["ai_assistant_personality_id"] = value; unsaved.Add("ai_assistant_personality_id"); }
        }

        /// <summary>
        /// Desktop Configuration Profile ID assigned directly to this user, if any.
        /// </summary>
        [JsonPropertyName("desktop_configuration_profile_id")]
        public Nullable<Int64> DesktopConfigurationProfileId
        {
            get { return (Nullable<Int64>)attributes["desktop_configuration_profile_id"]; }
            set { attributes["desktop_configuration_profile_id"] = value; unsaved.Add("desktop_configuration_profile_id"); }
        }

        /// <summary>
        /// Integration Centric Profile ID assigned directly to this user, if any.
        /// </summary>
        [JsonPropertyName("integration_centric_profile_id")]
        public Nullable<Int64> IntegrationCentricProfileId
        {
            get { return (Nullable<Int64>)attributes["integration_centric_profile_id"]; }
            set { attributes["integration_centric_profile_id"] = value; unsaved.Add("integration_centric_profile_id"); }
        }

        /// <summary>
        /// User email address
        /// </summary>
        [JsonPropertyName("email")]
        public string Email
        {
            get { return (string)attributes["email"]; }
            set { attributes["email"] = value; unsaved.Add("email"); }
        }

        /// <summary>
        /// File system layout
        /// </summary>
        [JsonPropertyName("filesystem_layout")]
        public string FilesystemLayout
        {
            get { return (string)attributes["filesystem_layout"]; }
            set { attributes["filesystem_layout"] = value; unsaved.Add("filesystem_layout"); }
        }

        /// <summary>
        /// User's first login time
        /// </summary>
        [JsonPropertyName("first_login_at")]
        public Nullable<DateTime> FirstLoginAt
        {
            get { return (Nullable<DateTime>)attributes["first_login_at"]; }
            set { attributes["first_login_at"] = value; unsaved.Add("first_login_at"); }
        }

        /// <summary>
        /// Can the user access with FTP/FTPS?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ftp_permission")]
        public bool FtpPermission
        {
            get { return attributes["ftp_permission"] == null ? false : (bool)attributes["ftp_permission"]; }
            set { attributes["ftp_permission"] = value; unsaved.Add("ftp_permission"); }
        }

        /// <summary>
        /// Comma-separated list of group IDs of which this user is a member
        /// </summary>
        [JsonPropertyName("group_ids")]
        public string GroupIds
        {
            get { return (string)attributes["group_ids"]; }
            set { attributes["group_ids"] = value; unsaved.Add("group_ids"); }
        }

        /// <summary>
        /// Text to display to the user in the header of the UI
        /// </summary>
        [JsonPropertyName("header_text")]
        public string HeaderText
        {
            get { return (string)attributes["header_text"]; }
            set { attributes["header_text"] = value; unsaved.Add("header_text"); }
        }

        /// <summary>
        /// Preferred language
        /// </summary>
        [JsonPropertyName("language")]
        public string Language
        {
            get { return (string)attributes["language"]; }
            set { attributes["language"] = value; unsaved.Add("language"); }
        }

        /// <summary>
        /// User's most recent login time via any protocol
        /// </summary>
        [JsonPropertyName("last_login_at")]
        public Nullable<DateTime> LastLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_login_at"]; }
            set { attributes["last_login_at"] = value; unsaved.Add("last_login_at"); }
        }

        /// <summary>
        /// User's most recent login time via web
        /// </summary>
        [JsonPropertyName("last_web_login_at")]
        public Nullable<DateTime> LastWebLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_web_login_at"]; }
            set { attributes["last_web_login_at"] = value; unsaved.Add("last_web_login_at"); }
        }

        /// <summary>
        /// User's most recent login time via FTP
        /// </summary>
        [JsonPropertyName("last_ftp_login_at")]
        public Nullable<DateTime> LastFtpLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_ftp_login_at"]; }
            set { attributes["last_ftp_login_at"] = value; unsaved.Add("last_ftp_login_at"); }
        }

        /// <summary>
        /// User's most recent login time via SFTP
        /// </summary>
        [JsonPropertyName("last_sftp_login_at")]
        public Nullable<DateTime> LastSftpLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_sftp_login_at"]; }
            set { attributes["last_sftp_login_at"] = value; unsaved.Add("last_sftp_login_at"); }
        }

        /// <summary>
        /// User's most recent login time via WebDAV
        /// </summary>
        [JsonPropertyName("last_dav_login_at")]
        public Nullable<DateTime> LastDavLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_dav_login_at"]; }
            set { attributes["last_dav_login_at"] = value; unsaved.Add("last_dav_login_at"); }
        }

        /// <summary>
        /// User's most recent login time via Desktop app
        /// </summary>
        [JsonPropertyName("last_desktop_login_at")]
        public Nullable<DateTime> LastDesktopLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_desktop_login_at"]; }
            set { attributes["last_desktop_login_at"] = value; unsaved.Add("last_desktop_login_at"); }
        }

        /// <summary>
        /// User's most recent login time via Rest API
        /// </summary>
        [JsonPropertyName("last_restapi_login_at")]
        public Nullable<DateTime> LastRestapiLoginAt
        {
            get { return (Nullable<DateTime>)attributes["last_restapi_login_at"]; }
            set { attributes["last_restapi_login_at"] = value; unsaved.Add("last_restapi_login_at"); }
        }

        /// <summary>
        /// User's most recent API use time
        /// </summary>
        [JsonPropertyName("last_api_use_at")]
        public Nullable<DateTime> LastApiUseAt
        {
            get { return (Nullable<DateTime>)attributes["last_api_use_at"]; }
            set { attributes["last_api_use_at"] = value; unsaved.Add("last_api_use_at"); }
        }

        /// <summary>
        /// User's most recent activity time, which is the latest of most recent login, most recent API use, enablement, or creation
        /// </summary>
        [JsonPropertyName("last_active_at")]
        public Nullable<DateTime> LastActiveAt
        {
            get { return (Nullable<DateTime>)attributes["last_active_at"]; }
            set { attributes["last_active_at"] = value; unsaved.Add("last_active_at"); }
        }

        /// <summary>
        /// The most recent protocol and cipher used
        /// </summary>
        [JsonPropertyName("last_protocol_cipher")]
        public string LastProtocolCipher
        {
            get { return (string)attributes["last_protocol_cipher"]; }
            set { attributes["last_protocol_cipher"] = value; unsaved.Add("last_protocol_cipher"); }
        }

        /// <summary>
        /// Time in the future that the user will no longer be locked out if applicable
        /// </summary>
        [JsonPropertyName("lockout_expires")]
        public Nullable<DateTime> LockoutExpires
        {
            get { return (Nullable<DateTime>)attributes["lockout_expires"]; }
            set { attributes["lockout_expires"] = value; unsaved.Add("lockout_expires"); }
        }

        /// <summary>
        /// User's full name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// User's company
        /// </summary>
        [JsonPropertyName("company")]
        public string Company
        {
            get { return (string)attributes["company"]; }
            set { attributes["company"] = value; unsaved.Add("company"); }
        }

        /// <summary>
        /// Any internal notes on the user
        /// </summary>
        [JsonPropertyName("notes")]
        public string Notes
        {
            get { return (string)attributes["notes"]; }
            set { attributes["notes"] = value; unsaved.Add("notes"); }
        }

        /// <summary>
        /// Hour of the day at which daily notifications should be sent. Can be in range 0 to 23
        /// </summary>
        [JsonPropertyName("notification_daily_send_time")]
        public Nullable<Int64> NotificationDailySendTime
        {
            get { return (Nullable<Int64>)attributes["notification_daily_send_time"]; }
            set { attributes["notification_daily_send_time"] = value; unsaved.Add("notification_daily_send_time"); }
        }

        /// <summary>
        /// Enable integration with Office for the web?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("office_integration_enabled")]
        public bool OfficeIntegrationEnabled
        {
            get { return attributes["office_integration_enabled"] == null ? false : (bool)attributes["office_integration_enabled"]; }
            set { attributes["office_integration_enabled"] = value; unsaved.Add("office_integration_enabled"); }
        }

        /// <summary>
        /// Is this user a Partner administrator?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("partner_admin")]
        public bool PartnerAdmin
        {
            get { return attributes["partner_admin"] == null ? false : (bool)attributes["partner_admin"]; }
            set { attributes["partner_admin"] = value; unsaved.Add("partner_admin"); }
        }

        /// <summary>
        /// Partner ID if this user belongs to a Partner
        /// </summary>
        [JsonPropertyName("partner_id")]
        public Nullable<Int64> PartnerId
        {
            get { return (Nullable<Int64>)attributes["partner_id"]; }
            set { attributes["partner_id"] = value; unsaved.Add("partner_id"); }
        }

        /// <summary>
        /// Name of the Partner if this user belongs to a Partner
        /// </summary>
        [JsonPropertyName("partner_name")]
        public string PartnerName
        {
            get { return (string)attributes["partner_name"]; }
            set { attributes["partner_name"] = value; unsaved.Add("partner_name"); }
        }

        /// <summary>
        /// Last time the user's password was set
        /// </summary>
        [JsonPropertyName("password_set_at")]
        public Nullable<DateTime> PasswordSetAt
        {
            get { return (Nullable<DateTime>)attributes["password_set_at"]; }
            set { attributes["password_set_at"] = value; unsaved.Add("password_set_at"); }
        }

        /// <summary>
        /// Number of days to allow user to use the same password
        /// </summary>
        [JsonPropertyName("password_validity_days")]
        public Nullable<Int64> PasswordValidityDays
        {
            get { return (Nullable<Int64>)attributes["password_validity_days"]; }
            set { attributes["password_validity_days"] = value; unsaved.Add("password_validity_days"); }
        }

        /// <summary>
        /// Primary group ID for Group Admin scoping
        /// </summary>
        [JsonPropertyName("primary_group_id")]
        public Nullable<Int64> PrimaryGroupId
        {
            get { return (Nullable<Int64>)attributes["primary_group_id"]; }
            set { attributes["primary_group_id"] = value; unsaved.Add("primary_group_id"); }
        }

        /// <summary>
        /// Number of public keys associated with this user
        /// </summary>
        [JsonPropertyName("public_keys_count")]
        public Nullable<Int64> PublicKeysCount
        {
            get { return (Nullable<Int64>)attributes["public_keys_count"]; }
            set { attributes["public_keys_count"] = value; unsaved.Add("public_keys_count"); }
        }

        /// <summary>
        /// Deprecated. Use notify_on_all_site_warnings and granular failure notification preferences instead.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("receive_admin_alerts")]
        public bool ReceiveAdminAlerts
        {
            get { return attributes["receive_admin_alerts"] == null ? false : (bool)attributes["receive_admin_alerts"]; }
            set { attributes["receive_admin_alerts"] = value; unsaved.Add("receive_admin_alerts"); }
        }

        /// <summary>
        /// Should the user receive site warnings via email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_all_site_warnings")]
        public bool NotifyOnAllSiteWarnings
        {
            get { return attributes["notify_on_all_site_warnings"] == null ? false : (bool)attributes["notify_on_all_site_warnings"]; }
            set { attributes["notify_on_all_site_warnings"] = value; unsaved.Add("notify_on_all_site_warnings"); }
        }

        /// <summary>
        /// Should the user receive sso/scim/ldap configuration/sync failures via email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_all_sso_failures")]
        public bool NotifyOnAllSsoFailures
        {
            get { return attributes["notify_on_all_sso_failures"] == null ? false : (bool)attributes["notify_on_all_sso_failures"]; }
            set { attributes["notify_on_all_sso_failures"] = value; unsaved.Add("notify_on_all_sso_failures"); }
        }

        /// <summary>
        /// Should the user receive user security events via email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_all_user_security_events")]
        public bool NotifyOnAllUserSecurityEvents
        {
            get { return attributes["notify_on_all_user_security_events"] == null ? false : (bool)attributes["notify_on_all_user_security_events"]; }
            set { attributes["notify_on_all_user_security_events"] = value; unsaved.Add("notify_on_all_user_security_events"); }
        }

        /// <summary>
        /// Should the user receive pending work failures via email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_all_pending_work_failures")]
        public bool NotifyOnAllPendingWorkFailures
        {
            get { return attributes["notify_on_all_pending_work_failures"] == null ? false : (bool)attributes["notify_on_all_pending_work_failures"]; }
            set { attributes["notify_on_all_pending_work_failures"] = value; unsaved.Add("notify_on_all_pending_work_failures"); }
        }

        /// <summary>
        /// Should the user receive siem failures via email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_all_siem_http_destination_failures")]
        public bool NotifyOnAllSiemHttpDestinationFailures
        {
            get { return attributes["notify_on_all_siem_http_destination_failures"] == null ? false : (bool)attributes["notify_on_all_siem_http_destination_failures"]; }
            set { attributes["notify_on_all_siem_http_destination_failures"] = value; unsaved.Add("notify_on_all_siem_http_destination_failures"); }
        }

        /// <summary>
        /// Should the user receive sync failures via email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_all_sync_failures")]
        public bool NotifyOnAllSyncFailures
        {
            get { return attributes["notify_on_all_sync_failures"] == null ? false : (bool)attributes["notify_on_all_sync_failures"]; }
            set { attributes["notify_on_all_sync_failures"] = value; unsaved.Add("notify_on_all_sync_failures"); }
        }

        /// <summary>
        /// Should the user receive automation failures via email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_all_automation_failures")]
        public bool NotifyOnAllAutomationFailures
        {
            get { return attributes["notify_on_all_automation_failures"] == null ? false : (bool)attributes["notify_on_all_automation_failures"]; }
            set { attributes["notify_on_all_automation_failures"] = value; unsaved.Add("notify_on_all_automation_failures"); }
        }

        /// <summary>
        /// Should the user receive expectation failures and misses via email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_all_expectation_failures")]
        public bool NotifyOnAllExpectationFailures
        {
            get { return attributes["notify_on_all_expectation_failures"] == null ? false : (bool)attributes["notify_on_all_expectation_failures"]; }
            set { attributes["notify_on_all_expectation_failures"] = value; unsaved.Add("notify_on_all_expectation_failures"); }
        }

        /// <summary>
        /// 2FA required setting. `use_system_setting` uses the site-wide setting, including SSO exemptions. `always_require` and `never_require` override the site-wide setting when user-level overrides are allowed.
        /// </summary>
        [JsonPropertyName("require_2fa")]
        public string Require2fa
        {
            get { return (string)attributes["require_2fa"]; }
            set { attributes["require_2fa"] = value; unsaved.Add("require_2fa"); }
        }

        /// <summary>
        /// Require user to login by specified date otherwise it will be disabled.
        /// </summary>
        [JsonPropertyName("require_login_by")]
        public Nullable<DateTime> RequireLoginBy
        {
            get { return (Nullable<DateTime>)attributes["require_login_by"]; }
            set { attributes["require_login_by"] = value; unsaved.Add("require_login_by"); }
        }

        /// <summary>
        /// Is 2fa active for the user?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("active_2fa")]
        public bool Active2fa
        {
            get { return attributes["active_2fa"] == null ? false : (bool)attributes["active_2fa"]; }
            set { attributes["active_2fa"] = value; unsaved.Add("active_2fa"); }
        }

        /// <summary>
        /// Is a password change required upon next user login?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("require_password_change")]
        public bool RequirePasswordChange
        {
            get { return attributes["require_password_change"] == null ? false : (bool)attributes["require_password_change"]; }
            set { attributes["require_password_change"] = value; unsaved.Add("require_password_change"); }
        }

        /// <summary>
        /// Is user's password expired?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("password_expired")]
        public bool PasswordExpired
        {
            get { return attributes["password_expired"] == null ? false : (bool)attributes["password_expired"]; }
            set { attributes["password_expired"] = value; unsaved.Add("password_expired"); }
        }

        /// <summary>
        /// ID of the internal Group responsible for this Partner User, overriding the Partner default.
        /// </summary>
        [JsonPropertyName("responsible_group_id")]
        public Nullable<Int64> ResponsibleGroupId
        {
            get { return (Nullable<Int64>)attributes["responsible_group_id"]; }
            set { attributes["responsible_group_id"] = value; unsaved.Add("responsible_group_id"); }
        }

        /// <summary>
        /// ID of the internal User responsible for this Partner User, overriding the Partner default.
        /// </summary>
        [JsonPropertyName("responsible_user_id")]
        public Nullable<Int64> ResponsibleUserId
        {
            get { return (Nullable<Int64>)attributes["responsible_user_id"]; }
            set { attributes["responsible_user_id"] = value; unsaved.Add("responsible_user_id"); }
        }

        /// <summary>
        /// Is the user an allowed to view all (non-billing) site configuration for this site?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("readonly_site_admin")]
        public bool ReadonlySiteAdmin
        {
            get { return attributes["readonly_site_admin"] == null ? false : (bool)attributes["readonly_site_admin"]; }
            set { attributes["readonly_site_admin"] = value; unsaved.Add("readonly_site_admin"); }
        }

        /// <summary>
        /// Can this user access the Web app, Desktop app, SDKs, or REST API?  (All of these tools use the API internally, so this is one unified permission set.)
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("restapi_permission")]
        public bool RestapiPermission
        {
            get { return attributes["restapi_permission"] == null ? false : (bool)attributes["restapi_permission"]; }
            set { attributes["restapi_permission"] = value; unsaved.Add("restapi_permission"); }
        }

        /// <summary>
        /// Can the user access the S3-compatible endpoint? Defaults to true.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("s3_compatible_endpoint_permission")]
        public bool S3CompatibleEndpointPermission
        {
            get { return attributes["s3_compatible_endpoint_permission"] == null ? false : (bool)attributes["s3_compatible_endpoint_permission"]; }
            set { attributes["s3_compatible_endpoint_permission"] = value; unsaved.Add("s3_compatible_endpoint_permission"); }
        }

        /// <summary>
        /// Does this user manage it's own credentials or is it a shared/bot user?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("self_managed")]
        public bool SelfManaged
        {
            get { return attributes["self_managed"] == null ? false : (bool)attributes["self_managed"]; }
            set { attributes["self_managed"] = value; unsaved.Add("self_managed"); }
        }

        /// <summary>
        /// Can the user access with SFTP?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_permission")]
        public bool SftpPermission
        {
            get { return attributes["sftp_permission"] == null ? false : (bool)attributes["sftp_permission"]; }
            set { attributes["sftp_permission"] = value; unsaved.Add("sftp_permission"); }
        }

        /// <summary>
        /// Is the user an administrator for this site?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("site_admin")]
        public bool SiteAdmin
        {
            get { return attributes["site_admin"] == null ? false : (bool)attributes["site_admin"]; }
            set { attributes["site_admin"] = value; unsaved.Add("site_admin"); }
        }

        /// <summary>
        /// Whether the user is an administrator of their own Custom Workspace. Does not reflect administration granted through Permissions.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("workspace_admin")]
        public bool WorkspaceAdmin
        {
            get { return attributes["workspace_admin"] == null ? false : (bool)attributes["workspace_admin"]; }
            set { attributes["workspace_admin"] = value; unsaved.Add("workspace_admin"); }
        }

        /// <summary>
        /// Site ID
        /// </summary>
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            set { attributes["site_id"] = value; unsaved.Add("site_id"); }
        }

        /// <summary>
        /// ID of the Workspace the user belongs to. 0 is the Default Workspace.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Workspace ID the user should land in by default when more than one Workspace is available.
        /// </summary>
        [JsonPropertyName("default_workspace_id")]
        public Nullable<Int64> DefaultWorkspaceId
        {
            get { return (Nullable<Int64>)attributes["default_workspace_id"]; }
            set { attributes["default_workspace_id"] = value; unsaved.Add("default_workspace_id"); }
        }

        /// <summary>
        /// Skip Welcome page in the UI?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_welcome_screen")]
        public bool SkipWelcomeScreen
        {
            get { return attributes["skip_welcome_screen"] == null ? false : (bool)attributes["skip_welcome_screen"]; }
            set { attributes["skip_welcome_screen"] = value; unsaved.Add("skip_welcome_screen"); }
        }

        /// <summary>
        /// SSL required setting
        /// </summary>
        [JsonPropertyName("ssl_required")]
        public string SslRequired
        {
            get { return (string)attributes["ssl_required"]; }
            set { attributes["ssl_required"] = value; unsaved.Add("ssl_required"); }
        }

        /// <summary>
        /// SSO (Single Sign On) strategy ID for the user, if applicable.
        /// </summary>
        [JsonPropertyName("sso_strategy_id")]
        public Nullable<Int64> SsoStrategyId
        {
            get { return (Nullable<Int64>)attributes["sso_strategy_id"]; }
            set { attributes["sso_strategy_id"] = value; unsaved.Add("sso_strategy_id"); }
        }

        /// <summary>
        /// Is the user subscribed to the newsletter?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("subscribe_to_newsletter")]
        public bool SubscribeToNewsletter
        {
            get { return attributes["subscribe_to_newsletter"] == null ? false : (bool)attributes["subscribe_to_newsletter"]; }
            set { attributes["subscribe_to_newsletter"] = value; unsaved.Add("subscribe_to_newsletter"); }
        }

        /// <summary>
        /// Is this user managed by a SsoStrategy?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("externally_managed")]
        public bool ExternallyManaged
        {
            get { return attributes["externally_managed"] == null ? false : (bool)attributes["externally_managed"]; }
            set { attributes["externally_managed"] = value; unsaved.Add("externally_managed"); }
        }

        /// <summary>
        /// Comma-separated list of Tags for this user. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        /// </summary>
        [JsonPropertyName("tags")]
        public string Tags
        {
            get { return (string)attributes["tags"]; }
            set { attributes["tags"] = value; unsaved.Add("tags"); }
        }

        /// <summary>
        /// User time zone
        /// </summary>
        [JsonPropertyName("time_zone")]
        public string TimeZone
        {
            get { return (string)attributes["time_zone"]; }
            set { attributes["time_zone"] = value; unsaved.Add("time_zone"); }
        }

        /// <summary>
        /// Type(s) of 2FA methods in use, for programmatic use.  Will be either `sms`, `totp`, `webauthn`, `yubi`, `email`, or multiple values sorted alphabetically and joined by an underscore.  Does not specify whether user has more than one of a given method.
        /// </summary>
        [JsonPropertyName("type_of_2fa")]
        public string TypeOf2fa
        {
            get { return (string)attributes["type_of_2fa"]; }
            set { attributes["type_of_2fa"] = value; unsaved.Add("type_of_2fa"); }
        }

        /// <summary>
        /// Type(s) of 2FA methods in use, formatted for displaying in the UI.  Unlike `type_of_2fa`, this value will make clear when a user has more than 1 of the same type of method.
        /// </summary>
        [JsonPropertyName("type_of_2fa_for_display")]
        public string TypeOf2faForDisplay
        {
            get { return (string)attributes["type_of_2fa_for_display"]; }
            set { attributes["type_of_2fa_for_display"] = value; unsaved.Add("type_of_2fa_for_display"); }
        }

        /// <summary>
        /// If filesystem layout is user_root, this path is the root path the user is fixed to for all interfaces. If the filesystem layout is site_root or partner_root, this acts as a root folder only for FTP and SFTP (SFTP applicability also requires a site-wide setting to be set). For partner_root layout, this path is relative to the Partner root folder for all callers and blank opts out of an additional protocol root. In this situation, this path is not applied to the API, Desktop, or Web interface.
        /// </summary>
        [JsonPropertyName("user_root")]
        public string UserRoot
        {
            get { return (string)attributes["user_root"]; }
            set { attributes["user_root"] = value; unsaved.Add("user_root"); }
        }

        /// <summary>
        /// Home folder for FTP/SFTP. For users with the partner_root filesystem layout, this path is relative to the Partner root folder. In all other cases, it is an absolute path. Only applies to FTP and SFTP, and not any other interface.
        /// </summary>
        [JsonPropertyName("user_home")]
        public string UserHome
        {
            get { return (string)attributes["user_home"]; }
            set { attributes["user_home"] = value; unsaved.Add("user_home"); }
        }

        /// <summary>
        /// Number of days remaining until password expires
        /// </summary>
        [JsonPropertyName("days_remaining_until_password_expire")]
        public Nullable<Int64> DaysRemainingUntilPasswordExpire
        {
            get { return (Nullable<Int64>)attributes["days_remaining_until_password_expire"]; }
            set { attributes["days_remaining_until_password_expire"] = value; unsaved.Add("days_remaining_until_password_expire"); }
        }

        /// <summary>
        /// Password expiration datetime
        /// </summary>
        [JsonPropertyName("password_expire_at")]
        public Nullable<DateTime> PasswordExpireAt
        {
            get { return (Nullable<DateTime>)attributes["password_expire_at"]; }
            set { attributes["password_expire_at"] = value; unsaved.Add("password_expire_at"); }
        }

        /// <summary>
        /// An image file for your user avatar.
        /// </summary>
        [JsonPropertyName("avatar_file")]
        public FileAttachment AvatarFile
        {
            get { return (FileAttachment)attributes["avatar_file"]; }
            set { attributes["avatar_file"] = value; unsaved.Add("avatar_file"); }
        }

        /// <summary>
        /// If true, the avatar will be deleted.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("avatar_delete")]
        public bool AvatarDelete
        {
            get { return attributes["avatar_delete"] == null ? false : (bool)attributes["avatar_delete"]; }
            set { attributes["avatar_delete"] = value; unsaved.Add("avatar_delete"); }
        }

        /// <summary>
        /// Used for changing a password on an existing user.
        /// </summary>
        [JsonPropertyName("change_password")]
        public string ChangePassword
        {
            get { return (string)attributes["change_password"]; }
            set { attributes["change_password"] = value; unsaved.Add("change_password"); }
        }

        /// <summary>
        /// Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        /// </summary>
        [JsonPropertyName("change_password_confirmation")]
        public string ChangePasswordConfirmation
        {
            get { return (string)attributes["change_password_confirmation"]; }
            set { attributes["change_password_confirmation"] = value; unsaved.Add("change_password_confirmation"); }
        }

        /// <summary>
        /// Permission to grant on the User Root upon creation, or on the Workspace root when the User Root is blank. For the Default Workspace, a blank User Root uses the site root. `admin` on a Custom Workspace's root grants Workspace Administrator access; on a subfolder it grants Folder Admin access. Can be blank or `admin`, `full`, `read`, `write`, `list`, `read+write`, `list+write`, or `history`.
        /// </summary>
        [JsonPropertyName("grant_permission")]
        public string GrantPermission
        {
            get { return (string)attributes["grant_permission"]; }
            set { attributes["grant_permission"] = value; unsaved.Add("grant_permission"); }
        }

        /// <summary>
        /// Group ID to associate this user with.
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>)attributes["group_id"]; }
            set { attributes["group_id"] = value; unsaved.Add("group_id"); }
        }

        /// <summary>
        /// Pre-calculated hash of the user's password. If supplied, this will be used to authenticate the user on first login. Supported hash methods are MD5, SHA1, and SHA256.
        /// </summary>
        [JsonPropertyName("imported_password_hash")]
        public string ImportedPasswordHash
        {
            get { return (string)attributes["imported_password_hash"]; }
            set { attributes["imported_password_hash"] = value; unsaved.Add("imported_password_hash"); }
        }

        /// <summary>
        /// User password.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string)attributes["password"]; }
            set { attributes["password"] = value; unsaved.Add("password"); }
        }

        /// <summary>
        /// Optional, but if provided, we will ensure that it matches the value sent in `password`.
        /// </summary>
        [JsonPropertyName("password_confirmation")]
        public string PasswordConfirmation
        {
            get { return (string)attributes["password_confirmation"]; }
            set { attributes["password_confirmation"] = value; unsaved.Add("password_confirmation"); }
        }

        /// <summary>
        /// Signifies that the user has read all the announcements in the UI.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("announcements_read")]
        public bool AnnouncementsRead
        {
            get { return attributes["announcements_read"] == null ? false : (bool)attributes["announcements_read"]; }
            set { attributes["announcements_read"] = value; unsaved.Add("announcements_read"); }
        }

        /// <summary>
        /// If true when changing authentication_method from `password` to `sso`, remove all two-factor methods. Ignored in all other cases.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("clear_2fa")]
        public bool Clear2fa
        {
            get { return attributes["clear_2fa"] == null ? false : (bool)attributes["clear_2fa"]; }
            set { attributes["clear_2fa"] = value; unsaved.Add("clear_2fa"); }
        }

        /// <summary>
        /// Required when assigning a Partner to an existing non-Partner user. If true, convert the user by assigning the partner_id provided.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("convert_to_partner_user")]
        public bool ConvertToPartnerUser
        {
            get { return attributes["convert_to_partner_user"] == null ? false : (bool)attributes["convert_to_partner_user"]; }
            set { attributes["convert_to_partner_user"] = value; unsaved.Add("convert_to_partner_user"); }
        }

        /// <summary>
        /// Unlock user who has been locked out due to failed logins
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Unlock(Dictionary<string, object> parameters)
        {
            return UnlockCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Unlock user who has been locked out due to failed logins
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task UnlockAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UnlockCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task UnlockCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The User has no id, which Unlock needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}/unlock", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Resend user welcome email
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task ResendWelcomeEmail(Dictionary<string, object> parameters)
        {
            return ResendWelcomeEmailCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Resend user welcome email
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task ResendWelcomeEmailAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ResendWelcomeEmailCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task ResendWelcomeEmailCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The User has no id, which ResendWelcomeEmail needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}/resend_welcome_email", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Trigger 2FA Reset process for user who has lost access to their existing 2FA methods
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task User2faReset(Dictionary<string, object> parameters)
        {
            return User2faResetCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Trigger 2FA Reset process for user who has lost access to their existing 2FA methods
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task User2faResetAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return User2faResetCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task User2faResetCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The User has no id, which User2faReset needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}/2fa/reset", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the User Root upon creation, or on the Workspace root when the User Root is blank. For the Default Workspace, a blank User Root uses the site root. `admin` on a Custom Workspace's root grants Workspace Administrator access; on a subfolder it grants Folder Admin access. Can be blank or `admin`, `full`, `read`, `write`, `list`, `read+write`, `list+write`, or `history`.
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
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The User it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<User> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the User Root upon creation, or on the Workspace root when the User Root is blank. For the Default Workspace, a blank User Root uses the site root. `admin` on a Custom Workspace's root grants Workspace Administrator access; on a subfolder it grants Folder Admin access. Can be blank or `admin`, `full`, `read`, `write`, `list`, `read+write`, `list+write`, or `history`.
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
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The User it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<User> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<User> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The User has no id, which Update needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("avatar_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: avatar_file is null, which cannot be sent" : "Bad parameter: avatar_file must be of type FileAttachment or IDictionary<string, object>", "avatar_file");
            }
            if (parameters.TryGetValue("avatar_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: avatar_delete is null, which cannot be sent" : "Bad parameter: avatar_delete must be of type bool", "avatar_delete");
            }
            if (parameters.TryGetValue("change_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: change_password is null, which cannot be sent" : "Bad parameter: change_password must be of type string", "change_password");
            }
            if (parameters.TryGetValue("change_password_confirmation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: change_password_confirmation is null, which cannot be sent" : "Bad parameter: change_password_confirmation must be of type string", "change_password_confirmation");
            }
            if (parameters.TryGetValue("email", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email is null, which cannot be sent" : "Bad parameter: email must be of type string", "email");
            }
            if (parameters.TryGetValue("grant_permission", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: grant_permission is null, which cannot be sent" : "Bad parameter: grant_permission must be of type string", "grant_permission");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type Nullable<Int64>", "group_id");
            }
            if (parameters.TryGetValue("group_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_ids is null, which cannot be sent" : "Bad parameter: group_ids must be of type string", "group_ids");
            }
            if (parameters.TryGetValue("imported_password_hash", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: imported_password_hash is null, which cannot be sent" : "Bad parameter: imported_password_hash must be of type string", "imported_password_hash");
            }
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("password_confirmation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_confirmation is null, which cannot be sent" : "Bad parameter: password_confirmation must be of type string", "password_confirmation");
            }
            if (parameters.TryGetValue("announcements_read", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: announcements_read is null, which cannot be sent" : "Bad parameter: announcements_read must be of type bool", "announcements_read");
            }
            if (parameters.TryGetValue("ai_assistant_personality_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ai_assistant_personality_id is null, which cannot be sent" : "Bad parameter: ai_assistant_personality_id must be of type Nullable<Int64>", "ai_assistant_personality_id");
            }
            if (parameters.TryGetValue("allowed_ips", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_ips is null, which cannot be sent" : "Bad parameter: allowed_ips must be of type string", "allowed_ips");
            }
            if (parameters.TryGetValue("attachments_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: attachments_permission is null, which cannot be sent" : "Bad parameter: attachments_permission must be of type bool", "attachments_permission");
            }
            if (parameters.TryGetValue("authenticate_until", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: authenticate_until is null, which cannot be sent" : "Bad parameter: authenticate_until must be of type string", "authenticate_until");
            }
            if (parameters.TryGetValue("authentication_method", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: authentication_method is null, which cannot be sent" : "Bad parameter: authentication_method must be of type string", "authentication_method");
            }
            if (parameters.TryGetValue("billing_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: billing_permission is null, which cannot be sent" : "Bad parameter: billing_permission must be of type bool", "billing_permission");
            }
            if (parameters.TryGetValue("bypass_user_lifecycle_rules", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypass_user_lifecycle_rules is null, which cannot be sent" : "Bad parameter: bypass_user_lifecycle_rules must be of type bool", "bypass_user_lifecycle_rules");
            }
            if (parameters.TryGetValue("bypass_site_allowed_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypass_site_allowed_ips is null, which cannot be sent" : "Bad parameter: bypass_site_allowed_ips must be of type bool", "bypass_site_allowed_ips");
            }
            if (parameters.TryGetValue("dav_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dav_permission is null, which cannot be sent" : "Bad parameter: dav_permission must be of type bool", "dav_permission");
            }
            if (parameters.TryGetValue("desktop_configuration_profile_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: desktop_configuration_profile_id is null, which cannot be sent" : "Bad parameter: desktop_configuration_profile_id must be of type Nullable<Int64>", "desktop_configuration_profile_id");
            }
            if (parameters.TryGetValue("default_workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: default_workspace_id is null, which cannot be sent" : "Bad parameter: default_workspace_id must be of type Nullable<Int64>", "default_workspace_id");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("filesystem_layout", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filesystem_layout is null, which cannot be sent" : "Bad parameter: filesystem_layout must be of type string", "filesystem_layout");
            }
            if (parameters.TryGetValue("ftp_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ftp_permission is null, which cannot be sent" : "Bad parameter: ftp_permission must be of type bool", "ftp_permission");
            }
            if (parameters.TryGetValue("header_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: header_text is null, which cannot be sent" : "Bad parameter: header_text must be of type string", "header_text");
            }
            if (parameters.TryGetValue("integration_centric_profile_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: integration_centric_profile_id is null, which cannot be sent" : "Bad parameter: integration_centric_profile_id must be of type Nullable<Int64>", "integration_centric_profile_id");
            }
            if (parameters.TryGetValue("language", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: language is null, which cannot be sent" : "Bad parameter: language must be of type string", "language");
            }
            if (parameters.TryGetValue("notification_daily_send_time", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notification_daily_send_time is null, which cannot be sent" : "Bad parameter: notification_daily_send_time must be of type Nullable<Int64>", "notification_daily_send_time");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("company", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: company is null, which cannot be sent" : "Bad parameter: company must be of type string", "company");
            }
            if (parameters.TryGetValue("notes", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notes is null, which cannot be sent" : "Bad parameter: notes must be of type string", "notes");
            }
            if (parameters.TryGetValue("office_integration_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: office_integration_enabled is null, which cannot be sent" : "Bad parameter: office_integration_enabled must be of type bool", "office_integration_enabled");
            }
            if (parameters.TryGetValue("partner_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_admin is null, which cannot be sent" : "Bad parameter: partner_admin must be of type bool", "partner_admin");
            }
            if (parameters.TryGetValue("partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_id is null, which cannot be sent" : "Bad parameter: partner_id must be of type Nullable<Int64>", "partner_id");
            }
            if (parameters.TryGetValue("password_validity_days", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_validity_days is null, which cannot be sent" : "Bad parameter: password_validity_days must be of type Nullable<Int64>", "password_validity_days");
            }
            if (parameters.TryGetValue("primary_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: primary_group_id is null, which cannot be sent" : "Bad parameter: primary_group_id must be of type Nullable<Int64>", "primary_group_id");
            }
            if (parameters.TryGetValue("readonly_site_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: readonly_site_admin is null, which cannot be sent" : "Bad parameter: readonly_site_admin must be of type bool", "readonly_site_admin");
            }
            if (parameters.TryGetValue("receive_admin_alerts", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: receive_admin_alerts is null, which cannot be sent" : "Bad parameter: receive_admin_alerts must be of type bool", "receive_admin_alerts");
            }
            if (parameters.TryGetValue("notify_on_all_site_warnings", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_site_warnings is null, which cannot be sent" : "Bad parameter: notify_on_all_site_warnings must be of type bool", "notify_on_all_site_warnings");
            }
            if (parameters.TryGetValue("notify_on_all_sso_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_sso_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_sso_failures must be of type bool", "notify_on_all_sso_failures");
            }
            if (parameters.TryGetValue("notify_on_all_user_security_events", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_user_security_events is null, which cannot be sent" : "Bad parameter: notify_on_all_user_security_events must be of type bool", "notify_on_all_user_security_events");
            }
            if (parameters.TryGetValue("notify_on_all_pending_work_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_pending_work_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_pending_work_failures must be of type bool", "notify_on_all_pending_work_failures");
            }
            if (parameters.TryGetValue("notify_on_all_siem_http_destination_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_siem_http_destination_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_siem_http_destination_failures must be of type bool", "notify_on_all_siem_http_destination_failures");
            }
            if (parameters.TryGetValue("notify_on_all_sync_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_sync_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_sync_failures must be of type bool", "notify_on_all_sync_failures");
            }
            if (parameters.TryGetValue("notify_on_all_automation_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_automation_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_automation_failures must be of type bool", "notify_on_all_automation_failures");
            }
            if (parameters.TryGetValue("notify_on_all_expectation_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_expectation_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_expectation_failures must be of type bool", "notify_on_all_expectation_failures");
            }
            if (parameters.TryGetValue("require_login_by", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_login_by is null, which cannot be sent" : "Bad parameter: require_login_by must be of type string", "require_login_by");
            }
            if (parameters.TryGetValue("require_password_change", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_password_change is null, which cannot be sent" : "Bad parameter: require_password_change must be of type bool", "require_password_change");
            }
            if (parameters.TryGetValue("responsible_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_group_id is null, which cannot be sent" : "Bad parameter: responsible_group_id must be of type Nullable<Int64>", "responsible_group_id");
            }
            if (parameters.TryGetValue("responsible_user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_user_id is null, which cannot be sent" : "Bad parameter: responsible_user_id must be of type Nullable<Int64>", "responsible_user_id");
            }
            if (parameters.TryGetValue("restapi_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restapi_permission is null, which cannot be sent" : "Bad parameter: restapi_permission must be of type bool", "restapi_permission");
            }
            if (parameters.TryGetValue("s3_compatible_endpoint_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_endpoint_permission is null, which cannot be sent" : "Bad parameter: s3_compatible_endpoint_permission must be of type bool", "s3_compatible_endpoint_permission");
            }
            if (parameters.TryGetValue("self_managed", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: self_managed is null, which cannot be sent" : "Bad parameter: self_managed must be of type bool", "self_managed");
            }
            if (parameters.TryGetValue("sftp_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_permission is null, which cannot be sent" : "Bad parameter: sftp_permission must be of type bool", "sftp_permission");
            }
            if (parameters.TryGetValue("site_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: site_admin is null, which cannot be sent" : "Bad parameter: site_admin must be of type bool", "site_admin");
            }
            if (parameters.TryGetValue("skip_welcome_screen", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_welcome_screen is null, which cannot be sent" : "Bad parameter: skip_welcome_screen must be of type bool", "skip_welcome_screen");
            }
            if (parameters.TryGetValue("ssl_required", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl_required is null, which cannot be sent" : "Bad parameter: ssl_required must be of type string", "ssl_required");
            }
            if (parameters.TryGetValue("sso_strategy_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sso_strategy_id is null, which cannot be sent" : "Bad parameter: sso_strategy_id must be of type Nullable<Int64>", "sso_strategy_id");
            }
            if (parameters.TryGetValue("subscribe_to_newsletter", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: subscribe_to_newsletter is null, which cannot be sent" : "Bad parameter: subscribe_to_newsletter must be of type bool", "subscribe_to_newsletter");
            }
            if (parameters.TryGetValue("require_2fa", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_2fa is null, which cannot be sent" : "Bad parameter: require_2fa must be of type string", "require_2fa");
            }
            if (parameters.TryGetValue("tags", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: tags is null, which cannot be sent" : "Bad parameter: tags must be of type string", "tags");
            }
            if (parameters.TryGetValue("time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: time_zone is null, which cannot be sent" : "Bad parameter: time_zone must be of type string", "time_zone");
            }
            if (parameters.TryGetValue("user_root", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_root is null, which cannot be sent" : "Bad parameter: user_root must be of type string", "user_root");
            }
            if (parameters.TryGetValue("user_home", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_home is null, which cannot be sent" : "Bad parameter: user_home must be of type string", "user_home");
            }
            if (parameters.TryGetValue("workspace_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_admin is null, which cannot be sent" : "Bad parameter: workspace_admin must be of type bool", "workspace_admin");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("clear_2fa", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: clear_2fa is null, which cannot be sent" : "Bad parameter: clear_2fa must be of type bool", "clear_2fa");
            }
            if (parameters.TryGetValue("convert_to_partner_user", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: convert_to_partner_user is null, which cannot be sent" : "Bad parameter: convert_to_partner_user must be of type bool", "convert_to_partner_user");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            User updated = JsonUtil.DeserializeModelResponse<User>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<User>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Parameters:
        ///   new_owner_id - int64 - Provide a User ID here to transfer ownership of certain resources such as Automations and Share Links (Bundles) to that new user.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   new_owner_id - int64 - Provide a User ID here to transfer ownership of certain resources such as Automations and Share Links (Bundles) to that new user.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>, but neither waits for the request nor reports its failure.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        [Obsolete("Destroy neither waits for the request nor reports its failure. Use DestroyAsync instead.")]
        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The User has no id, which Delete needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("new_owner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: new_owner_id is null, which cannot be sent" : "Bad parameter: new_owner_id must be of type Nullable<Int64>", "new_owner_id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/users/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "avatar_file", "avatar_delete", "change_password", "change_password_confirmation", "email", "grant_permission", "group_id", "group_ids", "imported_password_hash", "password", "password_confirmation", "announcements_read", "ai_assistant_personality_id", "allowed_ips", "attachments_permission", "authenticate_until", "authentication_method", "billing_permission", "bypass_user_lifecycle_rules", "bypass_site_allowed_ips", "dav_permission", "desktop_configuration_profile_id", "default_workspace_id", "disabled", "filesystem_layout", "ftp_permission", "header_text", "integration_centric_profile_id", "language", "notification_daily_send_time", "name", "company", "notes", "office_integration_enabled", "partner_admin", "partner_id", "password_validity_days", "primary_group_id", "readonly_site_admin", "receive_admin_alerts", "notify_on_all_site_warnings", "notify_on_all_sso_failures", "notify_on_all_user_security_events", "notify_on_all_pending_work_failures", "notify_on_all_siem_http_destination_failures", "notify_on_all_sync_failures", "notify_on_all_automation_failures", "notify_on_all_expectation_failures", "require_login_by", "require_password_change", "responsible_group_id", "responsible_user_id", "restapi_permission", "s3_compatible_endpoint_permission", "self_managed", "sftp_permission", "site_admin", "skip_welcome_screen", "ssl_required", "sso_strategy_id", "subscribe_to_newsletter", "require_2fa", "tags", "time_zone", "user_root", "user_home", "workspace_admin", "username", "workspace_id" };
        private static readonly string[] UpdateParameterNames = { "id", "avatar_file", "avatar_delete", "change_password", "change_password_confirmation", "email", "grant_permission", "group_id", "group_ids", "imported_password_hash", "password", "password_confirmation", "announcements_read", "ai_assistant_personality_id", "allowed_ips", "attachments_permission", "authenticate_until", "authentication_method", "billing_permission", "bypass_user_lifecycle_rules", "bypass_site_allowed_ips", "dav_permission", "desktop_configuration_profile_id", "default_workspace_id", "disabled", "filesystem_layout", "ftp_permission", "header_text", "integration_centric_profile_id", "language", "notification_daily_send_time", "name", "company", "notes", "office_integration_enabled", "partner_admin", "partner_id", "password_validity_days", "primary_group_id", "readonly_site_admin", "receive_admin_alerts", "notify_on_all_site_warnings", "notify_on_all_sso_failures", "notify_on_all_user_security_events", "notify_on_all_pending_work_failures", "notify_on_all_siem_http_destination_failures", "notify_on_all_sync_failures", "notify_on_all_automation_failures", "notify_on_all_expectation_failures", "require_login_by", "require_password_change", "responsible_group_id", "responsible_user_id", "restapi_permission", "s3_compatible_endpoint_permission", "self_managed", "sftp_permission", "site_admin", "skip_welcome_screen", "ssl_required", "sso_strategy_id", "subscribe_to_newsletter", "require_2fa", "tags", "time_zone", "user_root", "user_home", "workspace_admin", "username", "workspace_id", "clear_2fa", "convert_to_partner_user" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this User on Files.com, or updates it if it has an ID.
        /// It sends the attributes set on this object, in its constructor or with its properties, or changed in place,
        /// such as an element of an array, since it was made or last came from Files.com, of those that operation
        /// takes; it sends no other attributes, and no defaults. Each goes in the form its parameter takes, such as a
        /// DateTime as ISO 8601 text. When it succeeds, this object takes the attributes Files.com returns.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                await UpdateCore(AttributesToSave(UpdateParameterNames), cancellationToken);
                return;
            }
            User created = await User.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
            Refresh(created.getAttributes());
            origin = ((IModel)created).Origin;
        }

        // The attributes set or changed on this object since it was made or last came from Files.com, of those named
        // in parameterNames: the parameters of the operation that saves it.
        private Dictionary<string, object> AttributesToSave(string[] parameterNames)
        {
            Dictionary<string, object> toSave = new Dictionary<string, object>();
            foreach (string name in parameterNames)
            {
                if (unsaved.Contains(name))
                {
                    toSave[name] = ToParameter(name, attributes[name]);
                }
            }
            return toSave;
        }

        // The value SaveAsync sends for an attribute whose property type is not the one its parameter takes.
        private static object ToParameter(string name, object value)
        {
            switch (name)
            {
                case "authenticate_until":
                case "require_login_by":
                    return ParameterValue.FromDateTime(value);
                default:
                    return value;
            }
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
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
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<User> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
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
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<User> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<User> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }
            if (parameters.TryGetValue("filter_gt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gt is null, which cannot be sent" : "Bad parameter: filter_gt must be of type object", "filter_gt");
            }
            if (parameters.TryGetValue("filter_gteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gteq is null, which cannot be sent" : "Bad parameter: filter_gteq must be of type object", "filter_gteq");
            }
            if (parameters.TryGetValue("filter_prefix", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_prefix is null, which cannot be sent" : "Bad parameter: filter_prefix must be of type object", "filter_prefix");
            }
            if (parameters.TryGetValue("filter_lt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lt is null, which cannot be sent" : "Bad parameter: filter_lt must be of type object", "filter_lt");
            }
            if (parameters.TryGetValue("filter_lteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lteq is null, which cannot be sent" : "Bad parameter: filter_lteq must be of type object", "filter_lteq");
            }
            if (parameters.TryGetValue("ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ids is null, which cannot be sent" : "Bad parameter: ids must be of type string", "ids");
            }
            if (parameters.TryGetValue("include_parent_site_users", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: include_parent_site_users is null, which cannot be sent" : "Bad parameter: include_parent_site_users must be of type bool", "include_parent_site_users");
            }
            if (parameters.TryGetValue("search", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: search is null, which cannot be sent" : "Bad parameter: search must be of type string", "search");
            }

            return new FilesList<User>(client, $"/users", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - User ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<User> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - User ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<User> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<User> FindCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<User>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the User Root upon creation, or on the Workspace root when the User Root is blank. For the Default Workspace, a blank User Root uses the site root. `admin` on a Custom Workspace's root grants Workspace Administrator access; on a subfolder it grants Folder Admin access. Can be blank or `admin`, `full`, `read`, `write`, `list`, `read+write`, `list+write`, or `history`.
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
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<User> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<User> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("username", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: username", "username");
            }
            if (parameters.TryGetValue("avatar_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: avatar_file is null, which cannot be sent" : "Bad parameter: avatar_file must be of type FileAttachment or IDictionary<string, object>", "avatar_file");
            }
            if (parameters.TryGetValue("avatar_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: avatar_delete is null, which cannot be sent" : "Bad parameter: avatar_delete must be of type bool", "avatar_delete");
            }
            if (parameters.TryGetValue("change_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: change_password is null, which cannot be sent" : "Bad parameter: change_password must be of type string", "change_password");
            }
            if (parameters.TryGetValue("change_password_confirmation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: change_password_confirmation is null, which cannot be sent" : "Bad parameter: change_password_confirmation must be of type string", "change_password_confirmation");
            }
            if (parameters.TryGetValue("email", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email is null, which cannot be sent" : "Bad parameter: email must be of type string", "email");
            }
            if (parameters.TryGetValue("grant_permission", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: grant_permission is null, which cannot be sent" : "Bad parameter: grant_permission must be of type string", "grant_permission");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type Nullable<Int64>", "group_id");
            }
            if (parameters.TryGetValue("group_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_ids is null, which cannot be sent" : "Bad parameter: group_ids must be of type string", "group_ids");
            }
            if (parameters.TryGetValue("imported_password_hash", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: imported_password_hash is null, which cannot be sent" : "Bad parameter: imported_password_hash must be of type string", "imported_password_hash");
            }
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("password_confirmation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_confirmation is null, which cannot be sent" : "Bad parameter: password_confirmation must be of type string", "password_confirmation");
            }
            if (parameters.TryGetValue("announcements_read", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: announcements_read is null, which cannot be sent" : "Bad parameter: announcements_read must be of type bool", "announcements_read");
            }
            if (parameters.TryGetValue("ai_assistant_personality_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ai_assistant_personality_id is null, which cannot be sent" : "Bad parameter: ai_assistant_personality_id must be of type Nullable<Int64>", "ai_assistant_personality_id");
            }
            if (parameters.TryGetValue("allowed_ips", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_ips is null, which cannot be sent" : "Bad parameter: allowed_ips must be of type string", "allowed_ips");
            }
            if (parameters.TryGetValue("attachments_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: attachments_permission is null, which cannot be sent" : "Bad parameter: attachments_permission must be of type bool", "attachments_permission");
            }
            if (parameters.TryGetValue("authenticate_until", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: authenticate_until is null, which cannot be sent" : "Bad parameter: authenticate_until must be of type string", "authenticate_until");
            }
            if (parameters.TryGetValue("authentication_method", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: authentication_method is null, which cannot be sent" : "Bad parameter: authentication_method must be of type string", "authentication_method");
            }
            if (parameters.TryGetValue("billing_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: billing_permission is null, which cannot be sent" : "Bad parameter: billing_permission must be of type bool", "billing_permission");
            }
            if (parameters.TryGetValue("bypass_user_lifecycle_rules", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypass_user_lifecycle_rules is null, which cannot be sent" : "Bad parameter: bypass_user_lifecycle_rules must be of type bool", "bypass_user_lifecycle_rules");
            }
            if (parameters.TryGetValue("bypass_site_allowed_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypass_site_allowed_ips is null, which cannot be sent" : "Bad parameter: bypass_site_allowed_ips must be of type bool", "bypass_site_allowed_ips");
            }
            if (parameters.TryGetValue("dav_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dav_permission is null, which cannot be sent" : "Bad parameter: dav_permission must be of type bool", "dav_permission");
            }
            if (parameters.TryGetValue("desktop_configuration_profile_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: desktop_configuration_profile_id is null, which cannot be sent" : "Bad parameter: desktop_configuration_profile_id must be of type Nullable<Int64>", "desktop_configuration_profile_id");
            }
            if (parameters.TryGetValue("default_workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: default_workspace_id is null, which cannot be sent" : "Bad parameter: default_workspace_id must be of type Nullable<Int64>", "default_workspace_id");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("filesystem_layout", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filesystem_layout is null, which cannot be sent" : "Bad parameter: filesystem_layout must be of type string", "filesystem_layout");
            }
            if (parameters.TryGetValue("ftp_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ftp_permission is null, which cannot be sent" : "Bad parameter: ftp_permission must be of type bool", "ftp_permission");
            }
            if (parameters.TryGetValue("header_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: header_text is null, which cannot be sent" : "Bad parameter: header_text must be of type string", "header_text");
            }
            if (parameters.TryGetValue("integration_centric_profile_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: integration_centric_profile_id is null, which cannot be sent" : "Bad parameter: integration_centric_profile_id must be of type Nullable<Int64>", "integration_centric_profile_id");
            }
            if (parameters.TryGetValue("language", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: language is null, which cannot be sent" : "Bad parameter: language must be of type string", "language");
            }
            if (parameters.TryGetValue("notification_daily_send_time", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notification_daily_send_time is null, which cannot be sent" : "Bad parameter: notification_daily_send_time must be of type Nullable<Int64>", "notification_daily_send_time");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("company", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: company is null, which cannot be sent" : "Bad parameter: company must be of type string", "company");
            }
            if (parameters.TryGetValue("notes", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notes is null, which cannot be sent" : "Bad parameter: notes must be of type string", "notes");
            }
            if (parameters.TryGetValue("office_integration_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: office_integration_enabled is null, which cannot be sent" : "Bad parameter: office_integration_enabled must be of type bool", "office_integration_enabled");
            }
            if (parameters.TryGetValue("partner_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_admin is null, which cannot be sent" : "Bad parameter: partner_admin must be of type bool", "partner_admin");
            }
            if (parameters.TryGetValue("partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_id is null, which cannot be sent" : "Bad parameter: partner_id must be of type Nullable<Int64>", "partner_id");
            }
            if (parameters.TryGetValue("password_validity_days", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_validity_days is null, which cannot be sent" : "Bad parameter: password_validity_days must be of type Nullable<Int64>", "password_validity_days");
            }
            if (parameters.TryGetValue("primary_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: primary_group_id is null, which cannot be sent" : "Bad parameter: primary_group_id must be of type Nullable<Int64>", "primary_group_id");
            }
            if (parameters.TryGetValue("readonly_site_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: readonly_site_admin is null, which cannot be sent" : "Bad parameter: readonly_site_admin must be of type bool", "readonly_site_admin");
            }
            if (parameters.TryGetValue("receive_admin_alerts", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: receive_admin_alerts is null, which cannot be sent" : "Bad parameter: receive_admin_alerts must be of type bool", "receive_admin_alerts");
            }
            if (parameters.TryGetValue("notify_on_all_site_warnings", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_site_warnings is null, which cannot be sent" : "Bad parameter: notify_on_all_site_warnings must be of type bool", "notify_on_all_site_warnings");
            }
            if (parameters.TryGetValue("notify_on_all_sso_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_sso_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_sso_failures must be of type bool", "notify_on_all_sso_failures");
            }
            if (parameters.TryGetValue("notify_on_all_user_security_events", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_user_security_events is null, which cannot be sent" : "Bad parameter: notify_on_all_user_security_events must be of type bool", "notify_on_all_user_security_events");
            }
            if (parameters.TryGetValue("notify_on_all_pending_work_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_pending_work_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_pending_work_failures must be of type bool", "notify_on_all_pending_work_failures");
            }
            if (parameters.TryGetValue("notify_on_all_siem_http_destination_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_siem_http_destination_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_siem_http_destination_failures must be of type bool", "notify_on_all_siem_http_destination_failures");
            }
            if (parameters.TryGetValue("notify_on_all_sync_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_sync_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_sync_failures must be of type bool", "notify_on_all_sync_failures");
            }
            if (parameters.TryGetValue("notify_on_all_automation_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_automation_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_automation_failures must be of type bool", "notify_on_all_automation_failures");
            }
            if (parameters.TryGetValue("notify_on_all_expectation_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_expectation_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_expectation_failures must be of type bool", "notify_on_all_expectation_failures");
            }
            if (parameters.TryGetValue("require_login_by", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_login_by is null, which cannot be sent" : "Bad parameter: require_login_by must be of type string", "require_login_by");
            }
            if (parameters.TryGetValue("require_password_change", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_password_change is null, which cannot be sent" : "Bad parameter: require_password_change must be of type bool", "require_password_change");
            }
            if (parameters.TryGetValue("responsible_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_group_id is null, which cannot be sent" : "Bad parameter: responsible_group_id must be of type Nullable<Int64>", "responsible_group_id");
            }
            if (parameters.TryGetValue("responsible_user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_user_id is null, which cannot be sent" : "Bad parameter: responsible_user_id must be of type Nullable<Int64>", "responsible_user_id");
            }
            if (parameters.TryGetValue("restapi_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restapi_permission is null, which cannot be sent" : "Bad parameter: restapi_permission must be of type bool", "restapi_permission");
            }
            if (parameters.TryGetValue("s3_compatible_endpoint_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_endpoint_permission is null, which cannot be sent" : "Bad parameter: s3_compatible_endpoint_permission must be of type bool", "s3_compatible_endpoint_permission");
            }
            if (parameters.TryGetValue("self_managed", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: self_managed is null, which cannot be sent" : "Bad parameter: self_managed must be of type bool", "self_managed");
            }
            if (parameters.TryGetValue("sftp_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_permission is null, which cannot be sent" : "Bad parameter: sftp_permission must be of type bool", "sftp_permission");
            }
            if (parameters.TryGetValue("site_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: site_admin is null, which cannot be sent" : "Bad parameter: site_admin must be of type bool", "site_admin");
            }
            if (parameters.TryGetValue("skip_welcome_screen", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_welcome_screen is null, which cannot be sent" : "Bad parameter: skip_welcome_screen must be of type bool", "skip_welcome_screen");
            }
            if (parameters.TryGetValue("ssl_required", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl_required is null, which cannot be sent" : "Bad parameter: ssl_required must be of type string", "ssl_required");
            }
            if (parameters.TryGetValue("sso_strategy_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sso_strategy_id is null, which cannot be sent" : "Bad parameter: sso_strategy_id must be of type Nullable<Int64>", "sso_strategy_id");
            }
            if (parameters.TryGetValue("subscribe_to_newsletter", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: subscribe_to_newsletter is null, which cannot be sent" : "Bad parameter: subscribe_to_newsletter must be of type bool", "subscribe_to_newsletter");
            }
            if (parameters.TryGetValue("require_2fa", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_2fa is null, which cannot be sent" : "Bad parameter: require_2fa must be of type string", "require_2fa");
            }
            if (parameters.TryGetValue("tags", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: tags is null, which cannot be sent" : "Bad parameter: tags must be of type string", "tags");
            }
            if (parameters.TryGetValue("time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: time_zone is null, which cannot be sent" : "Bad parameter: time_zone must be of type string", "time_zone");
            }
            if (parameters.TryGetValue("user_root", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_root is null, which cannot be sent" : "Bad parameter: user_root must be of type string", "user_root");
            }
            if (parameters.TryGetValue("user_home", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_home is null, which cannot be sent" : "Bad parameter: user_home must be of type string", "user_home");
            }
            if (parameters.TryGetValue("workspace_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_admin is null, which cannot be sent" : "Bad parameter: workspace_admin must be of type bool", "workspace_admin");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/users", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<User>(responseJson, context, options);
        }

        /// <summary>
        /// Unlock user who has been locked out due to failed logins
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Unlock(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UnlockCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task UnlockCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}/unlock", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Resend user welcome email
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task ResendWelcomeEmail(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ResendWelcomeEmailCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task ResendWelcomeEmailCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}/resend_welcome_email", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Trigger 2FA Reset process for user who has lost access to their existing 2FA methods
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task User2faReset(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return User2faResetCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task User2faResetCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}/2fa/reset", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   avatar_file - file - An image file for your user avatar.
        ///   avatar_delete - boolean - If true, the avatar will be deleted.
        ///   change_password - string - Used for changing a password on an existing user.
        ///   change_password_confirmation - string - Optional, but if provided, we will ensure that it matches the value sent in `change_password`.
        ///   email - string - User's email.
        ///   grant_permission - string - Permission to grant on the User Root upon creation, or on the Workspace root when the User Root is blank. For the Default Workspace, a blank User Root uses the site root. `admin` on a Custom Workspace's root grants Workspace Administrator access; on a subfolder it grants Folder Admin access. Can be blank or `admin`, `full`, `read`, `write`, `list`, `read+write`, `list+write`, or `history`.
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
        /// <param name="id">User ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<User> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<User> UpdateCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("avatar_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: avatar_file is null, which cannot be sent" : "Bad parameter: avatar_file must be of type FileAttachment or IDictionary<string, object>", "avatar_file");
            }
            if (parameters.TryGetValue("avatar_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: avatar_delete is null, which cannot be sent" : "Bad parameter: avatar_delete must be of type bool", "avatar_delete");
            }
            if (parameters.TryGetValue("change_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: change_password is null, which cannot be sent" : "Bad parameter: change_password must be of type string", "change_password");
            }
            if (parameters.TryGetValue("change_password_confirmation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: change_password_confirmation is null, which cannot be sent" : "Bad parameter: change_password_confirmation must be of type string", "change_password_confirmation");
            }
            if (parameters.TryGetValue("email", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email is null, which cannot be sent" : "Bad parameter: email must be of type string", "email");
            }
            if (parameters.TryGetValue("grant_permission", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: grant_permission is null, which cannot be sent" : "Bad parameter: grant_permission must be of type string", "grant_permission");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type Nullable<Int64>", "group_id");
            }
            if (parameters.TryGetValue("group_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_ids is null, which cannot be sent" : "Bad parameter: group_ids must be of type string", "group_ids");
            }
            if (parameters.TryGetValue("imported_password_hash", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: imported_password_hash is null, which cannot be sent" : "Bad parameter: imported_password_hash must be of type string", "imported_password_hash");
            }
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("password_confirmation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_confirmation is null, which cannot be sent" : "Bad parameter: password_confirmation must be of type string", "password_confirmation");
            }
            if (parameters.TryGetValue("announcements_read", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: announcements_read is null, which cannot be sent" : "Bad parameter: announcements_read must be of type bool", "announcements_read");
            }
            if (parameters.TryGetValue("ai_assistant_personality_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ai_assistant_personality_id is null, which cannot be sent" : "Bad parameter: ai_assistant_personality_id must be of type Nullable<Int64>", "ai_assistant_personality_id");
            }
            if (parameters.TryGetValue("allowed_ips", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_ips is null, which cannot be sent" : "Bad parameter: allowed_ips must be of type string", "allowed_ips");
            }
            if (parameters.TryGetValue("attachments_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: attachments_permission is null, which cannot be sent" : "Bad parameter: attachments_permission must be of type bool", "attachments_permission");
            }
            if (parameters.TryGetValue("authenticate_until", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: authenticate_until is null, which cannot be sent" : "Bad parameter: authenticate_until must be of type string", "authenticate_until");
            }
            if (parameters.TryGetValue("authentication_method", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: authentication_method is null, which cannot be sent" : "Bad parameter: authentication_method must be of type string", "authentication_method");
            }
            if (parameters.TryGetValue("billing_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: billing_permission is null, which cannot be sent" : "Bad parameter: billing_permission must be of type bool", "billing_permission");
            }
            if (parameters.TryGetValue("bypass_user_lifecycle_rules", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypass_user_lifecycle_rules is null, which cannot be sent" : "Bad parameter: bypass_user_lifecycle_rules must be of type bool", "bypass_user_lifecycle_rules");
            }
            if (parameters.TryGetValue("bypass_site_allowed_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypass_site_allowed_ips is null, which cannot be sent" : "Bad parameter: bypass_site_allowed_ips must be of type bool", "bypass_site_allowed_ips");
            }
            if (parameters.TryGetValue("dav_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dav_permission is null, which cannot be sent" : "Bad parameter: dav_permission must be of type bool", "dav_permission");
            }
            if (parameters.TryGetValue("desktop_configuration_profile_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: desktop_configuration_profile_id is null, which cannot be sent" : "Bad parameter: desktop_configuration_profile_id must be of type Nullable<Int64>", "desktop_configuration_profile_id");
            }
            if (parameters.TryGetValue("default_workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: default_workspace_id is null, which cannot be sent" : "Bad parameter: default_workspace_id must be of type Nullable<Int64>", "default_workspace_id");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("filesystem_layout", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filesystem_layout is null, which cannot be sent" : "Bad parameter: filesystem_layout must be of type string", "filesystem_layout");
            }
            if (parameters.TryGetValue("ftp_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ftp_permission is null, which cannot be sent" : "Bad parameter: ftp_permission must be of type bool", "ftp_permission");
            }
            if (parameters.TryGetValue("header_text", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: header_text is null, which cannot be sent" : "Bad parameter: header_text must be of type string", "header_text");
            }
            if (parameters.TryGetValue("integration_centric_profile_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: integration_centric_profile_id is null, which cannot be sent" : "Bad parameter: integration_centric_profile_id must be of type Nullable<Int64>", "integration_centric_profile_id");
            }
            if (parameters.TryGetValue("language", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: language is null, which cannot be sent" : "Bad parameter: language must be of type string", "language");
            }
            if (parameters.TryGetValue("notification_daily_send_time", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notification_daily_send_time is null, which cannot be sent" : "Bad parameter: notification_daily_send_time must be of type Nullable<Int64>", "notification_daily_send_time");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("company", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: company is null, which cannot be sent" : "Bad parameter: company must be of type string", "company");
            }
            if (parameters.TryGetValue("notes", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notes is null, which cannot be sent" : "Bad parameter: notes must be of type string", "notes");
            }
            if (parameters.TryGetValue("office_integration_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: office_integration_enabled is null, which cannot be sent" : "Bad parameter: office_integration_enabled must be of type bool", "office_integration_enabled");
            }
            if (parameters.TryGetValue("partner_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_admin is null, which cannot be sent" : "Bad parameter: partner_admin must be of type bool", "partner_admin");
            }
            if (parameters.TryGetValue("partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_id is null, which cannot be sent" : "Bad parameter: partner_id must be of type Nullable<Int64>", "partner_id");
            }
            if (parameters.TryGetValue("password_validity_days", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password_validity_days is null, which cannot be sent" : "Bad parameter: password_validity_days must be of type Nullable<Int64>", "password_validity_days");
            }
            if (parameters.TryGetValue("primary_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: primary_group_id is null, which cannot be sent" : "Bad parameter: primary_group_id must be of type Nullable<Int64>", "primary_group_id");
            }
            if (parameters.TryGetValue("readonly_site_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: readonly_site_admin is null, which cannot be sent" : "Bad parameter: readonly_site_admin must be of type bool", "readonly_site_admin");
            }
            if (parameters.TryGetValue("receive_admin_alerts", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: receive_admin_alerts is null, which cannot be sent" : "Bad parameter: receive_admin_alerts must be of type bool", "receive_admin_alerts");
            }
            if (parameters.TryGetValue("notify_on_all_site_warnings", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_site_warnings is null, which cannot be sent" : "Bad parameter: notify_on_all_site_warnings must be of type bool", "notify_on_all_site_warnings");
            }
            if (parameters.TryGetValue("notify_on_all_sso_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_sso_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_sso_failures must be of type bool", "notify_on_all_sso_failures");
            }
            if (parameters.TryGetValue("notify_on_all_user_security_events", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_user_security_events is null, which cannot be sent" : "Bad parameter: notify_on_all_user_security_events must be of type bool", "notify_on_all_user_security_events");
            }
            if (parameters.TryGetValue("notify_on_all_pending_work_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_pending_work_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_pending_work_failures must be of type bool", "notify_on_all_pending_work_failures");
            }
            if (parameters.TryGetValue("notify_on_all_siem_http_destination_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_siem_http_destination_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_siem_http_destination_failures must be of type bool", "notify_on_all_siem_http_destination_failures");
            }
            if (parameters.TryGetValue("notify_on_all_sync_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_sync_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_sync_failures must be of type bool", "notify_on_all_sync_failures");
            }
            if (parameters.TryGetValue("notify_on_all_automation_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_automation_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_automation_failures must be of type bool", "notify_on_all_automation_failures");
            }
            if (parameters.TryGetValue("notify_on_all_expectation_failures", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_all_expectation_failures is null, which cannot be sent" : "Bad parameter: notify_on_all_expectation_failures must be of type bool", "notify_on_all_expectation_failures");
            }
            if (parameters.TryGetValue("require_login_by", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_login_by is null, which cannot be sent" : "Bad parameter: require_login_by must be of type string", "require_login_by");
            }
            if (parameters.TryGetValue("require_password_change", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_password_change is null, which cannot be sent" : "Bad parameter: require_password_change must be of type bool", "require_password_change");
            }
            if (parameters.TryGetValue("responsible_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_group_id is null, which cannot be sent" : "Bad parameter: responsible_group_id must be of type Nullable<Int64>", "responsible_group_id");
            }
            if (parameters.TryGetValue("responsible_user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_user_id is null, which cannot be sent" : "Bad parameter: responsible_user_id must be of type Nullable<Int64>", "responsible_user_id");
            }
            if (parameters.TryGetValue("restapi_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restapi_permission is null, which cannot be sent" : "Bad parameter: restapi_permission must be of type bool", "restapi_permission");
            }
            if (parameters.TryGetValue("s3_compatible_endpoint_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_endpoint_permission is null, which cannot be sent" : "Bad parameter: s3_compatible_endpoint_permission must be of type bool", "s3_compatible_endpoint_permission");
            }
            if (parameters.TryGetValue("self_managed", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: self_managed is null, which cannot be sent" : "Bad parameter: self_managed must be of type bool", "self_managed");
            }
            if (parameters.TryGetValue("sftp_permission", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_permission is null, which cannot be sent" : "Bad parameter: sftp_permission must be of type bool", "sftp_permission");
            }
            if (parameters.TryGetValue("site_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: site_admin is null, which cannot be sent" : "Bad parameter: site_admin must be of type bool", "site_admin");
            }
            if (parameters.TryGetValue("skip_welcome_screen", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_welcome_screen is null, which cannot be sent" : "Bad parameter: skip_welcome_screen must be of type bool", "skip_welcome_screen");
            }
            if (parameters.TryGetValue("ssl_required", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl_required is null, which cannot be sent" : "Bad parameter: ssl_required must be of type string", "ssl_required");
            }
            if (parameters.TryGetValue("sso_strategy_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sso_strategy_id is null, which cannot be sent" : "Bad parameter: sso_strategy_id must be of type Nullable<Int64>", "sso_strategy_id");
            }
            if (parameters.TryGetValue("subscribe_to_newsletter", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: subscribe_to_newsletter is null, which cannot be sent" : "Bad parameter: subscribe_to_newsletter must be of type bool", "subscribe_to_newsletter");
            }
            if (parameters.TryGetValue("require_2fa", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_2fa is null, which cannot be sent" : "Bad parameter: require_2fa must be of type string", "require_2fa");
            }
            if (parameters.TryGetValue("tags", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: tags is null, which cannot be sent" : "Bad parameter: tags must be of type string", "tags");
            }
            if (parameters.TryGetValue("time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: time_zone is null, which cannot be sent" : "Bad parameter: time_zone must be of type string", "time_zone");
            }
            if (parameters.TryGetValue("user_root", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_root is null, which cannot be sent" : "Bad parameter: user_root must be of type string", "user_root");
            }
            if (parameters.TryGetValue("user_home", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_home is null, which cannot be sent" : "Bad parameter: user_home must be of type string", "user_home");
            }
            if (parameters.TryGetValue("workspace_admin", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_admin is null, which cannot be sent" : "Bad parameter: workspace_admin must be of type bool", "workspace_admin");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("clear_2fa", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: clear_2fa is null, which cannot be sent" : "Bad parameter: clear_2fa must be of type bool", "clear_2fa");
            }
            if (parameters.TryGetValue("convert_to_partner_user", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: convert_to_partner_user is null, which cannot be sent" : "Bad parameter: convert_to_partner_user must be of type bool", "convert_to_partner_user");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<User>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   new_owner_id - int64 - Provide a User ID here to transfer ownership of certain resources such as Automations and Share Links (Bundles) to that new user.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   new_owner_id - int64 - Provide a User ID here to transfer ownership of certain resources such as Automations and Share Links (Bundles) to that new user.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(id, parameters, options);
        }

        internal static async Task DeleteCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("new_owner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: new_owner_id is null, which cannot be sent" : "Bad parameter: new_owner_id must be of type Nullable<Int64>", "new_owner_id");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/users/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}