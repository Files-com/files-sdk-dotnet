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
    /// BundleEntity model
    /// </summary>
    public class Bundle : IModel, IJsonOnDeserialized
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
        // The JSON, when handed out since then, of the arrays among them that callers can change in place, directly or
        // through a model in them. SaveAsync compares each with the array's JSON now, and sends the ones that differ.
        private readonly Dictionary<string, string> handedOut = new Dictionary<string, string>();
        private static readonly string[] ArrayAttributeNames = { "paths" };
        /// <summary>
        /// Makes a Bundle with no attributes set and no options.
        /// </summary>
        public Bundle() : this(null, null) { }

        /// <summary>
        /// Makes a Bundle with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Bundle(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("code"))
            {
                this.attributes.Add("code", null);
            }
            if (!this.attributes.ContainsKey("color_left"))
            {
                this.attributes.Add("color_left", null);
            }
            if (!this.attributes.ContainsKey("color_link"))
            {
                this.attributes.Add("color_link", null);
            }
            if (!this.attributes.ContainsKey("color_text"))
            {
                this.attributes.Add("color_text", null);
            }
            if (!this.attributes.ContainsKey("color_top"))
            {
                this.attributes.Add("color_top", null);
            }
            if (!this.attributes.ContainsKey("color_top_text"))
            {
                this.attributes.Add("color_top_text", null);
            }
            if (!this.attributes.ContainsKey("url"))
            {
                this.attributes.Add("url", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("expires_at"))
            {
                this.attributes.Add("expires_at", null);
            }
            if (!this.attributes.ContainsKey("password_protected"))
            {
                this.attributes.Add("password_protected", false);
            }
            if (!this.attributes.ContainsKey("permissions"))
            {
                this.attributes.Add("permissions", null);
            }
            if (!this.attributes.ContainsKey("preview_only"))
            {
                this.attributes.Add("preview_only", false);
            }
            if (!this.attributes.ContainsKey("require_registration"))
            {
                this.attributes.Add("require_registration", false);
            }
            if (!this.attributes.ContainsKey("require_share_recipient"))
            {
                this.attributes.Add("require_share_recipient", false);
            }
            if (!this.attributes.ContainsKey("require_logout"))
            {
                this.attributes.Add("require_logout", false);
            }
            if (!this.attributes.ContainsKey("clickwrap_body"))
            {
                this.attributes.Add("clickwrap_body", null);
            }
            if (!this.attributes.ContainsKey("form_field_set"))
            {
                this.attributes.Add("form_field_set", null);
            }
            if (!this.attributes.ContainsKey("skip_name"))
            {
                this.attributes.Add("skip_name", false);
            }
            if (!this.attributes.ContainsKey("skip_email"))
            {
                this.attributes.Add("skip_email", false);
            }
            if (!this.attributes.ContainsKey("start_access_on_date"))
            {
                this.attributes.Add("start_access_on_date", null);
            }
            if (!this.attributes.ContainsKey("skip_company"))
            {
                this.attributes.Add("skip_company", false);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("bypasses_site_expiration_rules"))
            {
                this.attributes.Add("bypasses_site_expiration_rules", false);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("deleted"))
            {
                this.attributes.Add("deleted", false);
            }
            if (!this.attributes.ContainsKey("deleted_at"))
            {
                this.attributes.Add("deleted_at", null);
            }
            if (!this.attributes.ContainsKey("dont_separate_submissions_by_folder"))
            {
                this.attributes.Add("dont_separate_submissions_by_folder", false);
            }
            if (!this.attributes.ContainsKey("effective_expires_at"))
            {
                this.attributes.Add("effective_expires_at", null);
            }
            if (!this.attributes.ContainsKey("max_uses"))
            {
                this.attributes.Add("max_uses", null);
            }
            if (!this.attributes.ContainsKey("internal_name"))
            {
                this.attributes.Add("internal_name", null);
            }
            if (!this.attributes.ContainsKey("note"))
            {
                this.attributes.Add("note", null);
            }
            if (!this.attributes.ContainsKey("path_template"))
            {
                this.attributes.Add("path_template", null);
            }
            if (!this.attributes.ContainsKey("path_template_time_zone"))
            {
                this.attributes.Add("path_template_time_zone", null);
            }
            if (!this.attributes.ContainsKey("send_email_receipt_to_uploader"))
            {
                this.attributes.Add("send_email_receipt_to_uploader", false);
            }
            if (!this.attributes.ContainsKey("snapshot_id"))
            {
                this.attributes.Add("snapshot_id", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("group_id"))
            {
                this.attributes.Add("group_id", null);
            }
            if (!this.attributes.ContainsKey("clickwrap_id"))
            {
                this.attributes.Add("clickwrap_id", null);
            }
            if (!this.attributes.ContainsKey("inbox_id"))
            {
                this.attributes.Add("inbox_id", null);
            }
            if (!this.attributes.ContainsKey("watermark_attachment"))
            {
                this.attributes.Add("watermark_attachment", null);
            }
            if (!this.attributes.ContainsKey("watermark_value"))
            {
                this.attributes.Add("watermark_value", null);
            }
            if (!this.attributes.ContainsKey("send_one_time_password_to_recipient_at_registration"))
            {
                this.attributes.Add("send_one_time_password_to_recipient_at_registration", false);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("has_inbox"))
            {
                this.attributes.Add("has_inbox", false);
            }
            if (!this.attributes.ContainsKey("dont_allow_folders_in_uploads"))
            {
                this.attributes.Add("dont_allow_folders_in_uploads", false);
            }
            if (!this.attributes.ContainsKey("requested_upload_slots"))
            {
                this.attributes.Add("requested_upload_slots", new object[0]);
            }
            if (!this.attributes.ContainsKey("paths"))
            {
                this.attributes.Add("paths", new string[0]);
            }
            if (!this.attributes.ContainsKey("bundlepaths"))
            {
                this.attributes.Add("bundlepaths", new BundlePath[0]);
            }
            if (!this.attributes.ContainsKey("password"))
            {
                this.attributes.Add("password", null);
            }
            if (!this.attributes.ContainsKey("form_field_set_id"))
            {
                this.attributes.Add("form_field_set_id", null);
            }
            if (!this.attributes.ContainsKey("create_snapshot"))
            {
                this.attributes.Add("create_snapshot", false);
            }
            if (!this.attributes.ContainsKey("finalize_snapshot"))
            {
                this.attributes.Add("finalize_snapshot", false);
            }
            if (!this.attributes.ContainsKey("watermark_attachment_file"))
            {
                this.attributes.Add("watermark_attachment_file", null);
            }
            if (!this.attributes.ContainsKey("watermark_attachment_delete"))
            {
                this.attributes.Add("watermark_attachment_delete", false);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            // The copy holds this object's own arrays, which the caller can change in place as through its properties.
            foreach (string name in ArrayAttributeNames)
            {
                HandedOut(name);
            }
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
            get { return new object[] { FormFieldSet, WatermarkAttachment, Bundlepaths }; }
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
            handedOut.Clear();
        }


        /// <summary>
        /// Bundle code.  This code forms the end part of the Public URL.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code
        {
            get { return (string)attributes["code"]; }
            set { attributes["code"] = value; unsaved.Add("code"); }
        }

        /// <summary>
        /// Page link and button color
        /// </summary>
        [JsonPropertyName("color_left")]
        public string ColorLeft
        {
            get { return (string)attributes["color_left"]; }
            set { attributes["color_left"] = value; unsaved.Add("color_left"); }
        }

        /// <summary>
        /// Top bar link color
        /// </summary>
        [JsonPropertyName("color_link")]
        public string ColorLink
        {
            get { return (string)attributes["color_link"]; }
            set { attributes["color_link"] = value; unsaved.Add("color_link"); }
        }

        /// <summary>
        /// Page link and button color
        /// </summary>
        [JsonPropertyName("color_text")]
        public string ColorText
        {
            get { return (string)attributes["color_text"]; }
            set { attributes["color_text"] = value; unsaved.Add("color_text"); }
        }

        /// <summary>
        /// Top bar background color
        /// </summary>
        [JsonPropertyName("color_top")]
        public string ColorTop
        {
            get { return (string)attributes["color_top"]; }
            set { attributes["color_top"] = value; unsaved.Add("color_top"); }
        }

        /// <summary>
        /// Top bar text color
        /// </summary>
        [JsonPropertyName("color_top_text")]
        public string ColorTopText
        {
            get { return (string)attributes["color_top_text"]; }
            set { attributes["color_top_text"] = value; unsaved.Add("color_top_text"); }
        }

        /// <summary>
        /// Public URL of Share Link
        /// </summary>
        [JsonPropertyName("url")]
        public string Url
        {
            get { return (string)attributes["url"]; }
            set { attributes["url"] = value; unsaved.Add("url"); }
        }

        /// <summary>
        /// Public description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; unsaved.Add("description"); }
        }

        /// <summary>
        /// Explicit Bundle expiration date/time. If not set, the site-wide expiration setting may apply.
        /// </summary>
        [JsonPropertyName("expires_at")]
        public Nullable<DateTime> ExpiresAt
        {
            get { return (Nullable<DateTime>)attributes["expires_at"]; }
            set { attributes["expires_at"] = value; unsaved.Add("expires_at"); }
        }

        /// <summary>
        /// Is this bundle password protected?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("password_protected")]
        public bool PasswordProtected
        {
            get { return attributes["password_protected"] == null ? false : (bool)attributes["password_protected"]; }
            set { attributes["password_protected"] = value; unsaved.Add("password_protected"); }
        }

        /// <summary>
        /// Permissions that apply to Folders in this Share Link.
        /// </summary>
        [JsonPropertyName("permissions")]
        public string Permissions
        {
            get { return (string)attributes["permissions"]; }
            set { attributes["permissions"] = value; unsaved.Add("permissions"); }
        }

        /// <summary>
        /// The preview_only field of a Bundle object in the Files.com API.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("preview_only")]
        public bool PreviewOnly
        {
            get { return attributes["preview_only"] == null ? false : (bool)attributes["preview_only"]; }
            set { attributes["preview_only"] = value; unsaved.Add("preview_only"); }
        }

        /// <summary>
        /// Show a registration page that captures the downloader's name and email address?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("require_registration")]
        public bool RequireRegistration
        {
            get { return attributes["require_registration"] == null ? false : (bool)attributes["require_registration"]; }
            set { attributes["require_registration"] = value; unsaved.Add("require_registration"); }
        }

        /// <summary>
        /// Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("require_share_recipient")]
        public bool RequireShareRecipient
        {
            get { return attributes["require_share_recipient"] == null ? false : (bool)attributes["require_share_recipient"]; }
            set { attributes["require_share_recipient"] = value; unsaved.Add("require_share_recipient"); }
        }

        /// <summary>
        /// If true, we will hide the 'Remember Me' box on the Bundle registration page, requiring that the user logout and log back in every time they visit the page.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("require_logout")]
        public bool RequireLogout
        {
            get { return attributes["require_logout"] == null ? false : (bool)attributes["require_logout"]; }
            set { attributes["require_logout"] = value; unsaved.Add("require_logout"); }
        }

        /// <summary>
        /// Legal text that must be agreed to prior to accessing Bundle.
        /// </summary>
        [JsonPropertyName("clickwrap_body")]
        public string ClickwrapBody
        {
            get { return (string)attributes["clickwrap_body"]; }
            set { attributes["clickwrap_body"] = value; unsaved.Add("clickwrap_body"); }
        }

        /// <summary>
        /// Custom Form to use
        /// </summary>
        [JsonPropertyName("form_field_set")]
        public FormFieldSet FormFieldSet
        {
            get { return (FormFieldSet)attributes["form_field_set"]; }
            set { attributes["form_field_set"] = value; unsaved.Add("form_field_set"); }
        }

        /// <summary>
        /// BundleRegistrations can be saved without providing name?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_name")]
        public bool SkipName
        {
            get { return attributes["skip_name"] == null ? false : (bool)attributes["skip_name"]; }
            set { attributes["skip_name"] = value; unsaved.Add("skip_name"); }
        }

        /// <summary>
        /// BundleRegistrations can be saved without providing email?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_email")]
        public bool SkipEmail
        {
            get { return attributes["skip_email"] == null ? false : (bool)attributes["skip_email"]; }
            set { attributes["skip_email"] = value; unsaved.Add("skip_email"); }
        }

        /// <summary>
        /// Date when share will start to be accessible. If `nil` access granted right after create.
        /// </summary>
        [JsonPropertyName("start_access_on_date")]
        public Nullable<DateTime> StartAccessOnDate
        {
            get { return (Nullable<DateTime>)attributes["start_access_on_date"]; }
            set { attributes["start_access_on_date"] = value; unsaved.Add("start_access_on_date"); }
        }

        /// <summary>
        /// BundleRegistrations can be saved without providing company?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_company")]
        public bool SkipCompany
        {
            get { return attributes["skip_company"] == null ? false : (bool)attributes["skip_company"]; }
            set { attributes["skip_company"] = value; unsaved.Add("skip_company"); }
        }

        /// <summary>
        /// Bundle ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// If true, this Share Link bypasses site-wide expiration rules. Only site admins may set this.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("bypasses_site_expiration_rules")]
        public bool BypassesSiteExpirationRules
        {
            get { return attributes["bypasses_site_expiration_rules"] == null ? false : (bool)attributes["bypasses_site_expiration_rules"]; }
            set { attributes["bypasses_site_expiration_rules"] = value; unsaved.Add("bypasses_site_expiration_rules"); }
        }

        /// <summary>
        /// Bundle created at date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Indicates if the bundle has been deleted.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("deleted")]
        public bool Deleted
        {
            get { return attributes["deleted"] == null ? false : (bool)attributes["deleted"]; }
            set { attributes["deleted"] = value; unsaved.Add("deleted"); }
        }

        /// <summary>
        /// Bundle deleted at date/time
        /// </summary>
        [JsonPropertyName("deleted_at")]
        public Nullable<DateTime> DeletedAt
        {
            get { return (Nullable<DateTime>)attributes["deleted_at"]; }
            set { attributes["deleted_at"] = value; unsaved.Add("deleted_at"); }
        }

        /// <summary>
        /// Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dont_separate_submissions_by_folder")]
        public bool DontSeparateSubmissionsByFolder
        {
            get { return attributes["dont_separate_submissions_by_folder"] == null ? false : (bool)attributes["dont_separate_submissions_by_folder"]; }
            set { attributes["dont_separate_submissions_by_folder"] = value; unsaved.Add("dont_separate_submissions_by_folder"); }
        }

        /// <summary>
        /// Read-only expiration date/time, using the explicit expiration or the site-wide setting when applicable. Null when the Share Link does not expire.
        /// </summary>
        [JsonPropertyName("effective_expires_at")]
        public Nullable<DateTime> EffectiveExpiresAt
        {
            get { return (Nullable<DateTime>)attributes["effective_expires_at"]; }
            set { attributes["effective_expires_at"] = value; unsaved.Add("effective_expires_at"); }
        }

        /// <summary>
        /// Maximum number of times bundle can be accessed
        /// </summary>
        [JsonPropertyName("max_uses")]
        public Nullable<Int64> MaxUses
        {
            get { return (Nullable<Int64>)attributes["max_uses"]; }
            set { attributes["max_uses"] = value; unsaved.Add("max_uses"); }
        }

        /// <summary>
        /// Internal name for identifying this Share Link.
        /// </summary>
        [JsonPropertyName("internal_name")]
        public string InternalName
        {
            get { return (string)attributes["internal_name"]; }
            set { attributes["internal_name"] = value; unsaved.Add("internal_name"); }
        }

        /// <summary>
        /// Bundle internal note
        /// </summary>
        [JsonPropertyName("note")]
        public string Note
        {
            get { return (string)attributes["note"]; }
            set { attributes["note"] = value; unsaved.Add("note"); }
        }

        /// <summary>
        /// Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
        /// </summary>
        [JsonPropertyName("path_template")]
        public string PathTemplate
        {
            get { return (string)attributes["path_template"]; }
            set { attributes["path_template"] = value; unsaved.Add("path_template"); }
        }

        /// <summary>
        /// Timezone to use when rendering timestamps in path templates.
        /// </summary>
        [JsonPropertyName("path_template_time_zone")]
        public string PathTemplateTimeZone
        {
            get { return (string)attributes["path_template_time_zone"]; }
            set { attributes["path_template_time_zone"] = value; unsaved.Add("path_template_time_zone"); }
        }

        /// <summary>
        /// Send delivery receipt to the uploader. Note: For writable share only
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("send_email_receipt_to_uploader")]
        public bool SendEmailReceiptToUploader
        {
            get { return attributes["send_email_receipt_to_uploader"] == null ? false : (bool)attributes["send_email_receipt_to_uploader"]; }
            set { attributes["send_email_receipt_to_uploader"] = value; unsaved.Add("send_email_receipt_to_uploader"); }
        }

        /// <summary>
        /// ID of the snapshot containing this bundle's contents.
        /// </summary>
        [JsonPropertyName("snapshot_id")]
        public Nullable<Int64> SnapshotId
        {
            get { return (Nullable<Int64>)attributes["snapshot_id"]; }
            set { attributes["snapshot_id"] = value; unsaved.Add("snapshot_id"); }
        }

        /// <summary>
        /// Bundle creator user ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; unsaved.Add("user_id"); }
        }

        /// <summary>
        /// Bundle creator username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; unsaved.Add("username"); }
        }

        /// <summary>
        /// Owning group ID. If set, members of this group can view, edit, and share this Share Link.
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>)attributes["group_id"]; }
            set { attributes["group_id"] = value; unsaved.Add("group_id"); }
        }

        /// <summary>
        /// ID of the clickwrap to use with this bundle.
        /// </summary>
        [JsonPropertyName("clickwrap_id")]
        public Nullable<Int64> ClickwrapId
        {
            get { return (Nullable<Int64>)attributes["clickwrap_id"]; }
            set { attributes["clickwrap_id"] = value; unsaved.Add("clickwrap_id"); }
        }

        /// <summary>
        /// ID of the associated inbox, if available.
        /// </summary>
        [JsonPropertyName("inbox_id")]
        public Nullable<Int64> InboxId
        {
            get { return (Nullable<Int64>)attributes["inbox_id"]; }
            set { attributes["inbox_id"] = value; unsaved.Add("inbox_id"); }
        }

        /// <summary>
        /// Preview watermark image applied to all bundle items.
        /// </summary>
        [JsonPropertyName("watermark_attachment")]
        public Image WatermarkAttachment
        {
            get { return (Image)attributes["watermark_attachment"]; }
            set { attributes["watermark_attachment"] = value; unsaved.Add("watermark_attachment"); }
        }

        /// <summary>
        /// Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
        /// </summary>
        [JsonPropertyName("watermark_value")]
        public object WatermarkValue
        {
            get { return (object)attributes["watermark_value"]; }
            set { attributes["watermark_value"] = value; unsaved.Add("watermark_value"); }
        }

        /// <summary>
        /// If true, require_share_recipient bundles will send a one-time password to the recipient when they register. Cannot be enabled if the bundle has a password set.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("send_one_time_password_to_recipient_at_registration")]
        public bool SendOneTimePasswordToRecipientAtRegistration
        {
            get { return attributes["send_one_time_password_to_recipient_at_registration"] == null ? false : (bool)attributes["send_one_time_password_to_recipient_at_registration"]; }
            set { attributes["send_one_time_password_to_recipient_at_registration"] = value; unsaved.Add("send_one_time_password_to_recipient_at_registration"); }
        }

        /// <summary>
        /// Workspace ID. `0` means the default workspace.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Does this bundle have an associated inbox?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("has_inbox")]
        public bool HasInbox
        {
            get { return attributes["has_inbox"] == null ? false : (bool)attributes["has_inbox"]; }
            set { attributes["has_inbox"] = value; unsaved.Add("has_inbox"); }
        }

        /// <summary>
        /// Should folder uploads be prevented?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dont_allow_folders_in_uploads")]
        public bool DontAllowFoldersInUploads
        {
            get { return attributes["dont_allow_folders_in_uploads"] == null ? false : (bool)attributes["dont_allow_folders_in_uploads"]; }
            set { attributes["dont_allow_folders_in_uploads"] = value; unsaved.Add("dont_allow_folders_in_uploads"); }
        }

        /// <summary>
        /// Upload slots requested by the associated Inbox. Each slot contains a name used as its label and destination subfolder name.
        /// </summary>
        [JsonPropertyName("requested_upload_slots")]
        public object[] RequestedUploadSlots
        {
            get { return (object[])attributes["requested_upload_slots"]; }
            set { attributes["requested_upload_slots"] = value; unsaved.Add("requested_upload_slots"); }
        }

        /// <summary>
        /// A list of paths in this bundle.  For performance reasons, this is not provided when listing bundles.
        /// </summary>
        [JsonPropertyName("paths")]
        public string[] Paths
        {
            get { return (string[])HandedOut("paths"); }
            set { attributes["paths"] = value; unsaved.Add("paths"); }
        }

        /// <summary>
        /// A list of bundlepaths in this bundle.  For performance reasons, this is not provided when listing bundles.
        /// </summary>
        [JsonPropertyName("bundlepaths")]
        public BundlePath[] Bundlepaths
        {
            get { return (BundlePath[])attributes["bundlepaths"]; }
            set { attributes["bundlepaths"] = value; unsaved.Add("bundlepaths"); }
        }

        /// <summary>
        /// Password for this bundle.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string)attributes["password"]; }
            set { attributes["password"] = value; unsaved.Add("password"); }
        }

        /// <summary>
        /// Id of Form Field Set to use with this bundle
        /// </summary>
        [JsonPropertyName("form_field_set_id")]
        public Nullable<Int64> FormFieldSetId
        {
            get { return (Nullable<Int64>)attributes["form_field_set_id"]; }
            set { attributes["form_field_set_id"] = value; unsaved.Add("form_field_set_id"); }
        }

        /// <summary>
        /// If true, create a snapshot of this bundle's contents.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("create_snapshot")]
        public bool CreateSnapshot
        {
            get { return attributes["create_snapshot"] == null ? false : (bool)attributes["create_snapshot"]; }
            set { attributes["create_snapshot"] = value; unsaved.Add("create_snapshot"); }
        }

        /// <summary>
        /// If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("finalize_snapshot")]
        public bool FinalizeSnapshot
        {
            get { return attributes["finalize_snapshot"] == null ? false : (bool)attributes["finalize_snapshot"]; }
            set { attributes["finalize_snapshot"] = value; unsaved.Add("finalize_snapshot"); }
        }

        /// <summary>
        /// Preview watermark image applied to all bundle items.
        /// </summary>
        [JsonPropertyName("watermark_attachment_file")]
        public FileAttachment WatermarkAttachmentFile
        {
            get { return (FileAttachment)attributes["watermark_attachment_file"]; }
            set { attributes["watermark_attachment_file"] = value; unsaved.Add("watermark_attachment_file"); }
        }

        /// <summary>
        /// If true, will delete the file stored in watermark_attachment
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("watermark_attachment_delete")]
        public bool WatermarkAttachmentDelete
        {
            get { return attributes["watermark_attachment_delete"] == null ? false : (bool)attributes["watermark_attachment_delete"]; }
            set { attributes["watermark_attachment_delete"] = value; unsaved.Add("watermark_attachment_delete"); }
        }

        /// <summary>
        /// Send email(s) with a link to bundle
        ///
        /// Parameters:
        ///   to - array(string) - A list of email addresses to share this bundle with. Required unless `recipients` is used.
        ///   note - string - Note to include in email.
        ///   recipients - array(object) - A list of recipients to share this bundle with. Required unless `to` is used.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task Share(Dictionary<string, object> parameters)
        {
            return ShareCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Send email(s) with a link to bundle
        ///
        /// Parameters:
        ///   to - array(string) - A list of email addresses to share this bundle with. Required unless `recipients` is used.
        ///   note - string - Note to include in email.
        ///   recipients - array(object) - A list of recipients to share this bundle with. Required unless `to` is used.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task ShareAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ShareCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task ShareCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Bundle has no id, which Share needs", "id");
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
            if (parameters.TryGetValue("to", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to is null, which cannot be sent" : "Bad parameter: to must be of type string[]", "to");
            }
            if (parameters.TryGetValue("note", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: note is null, which cannot be sent" : "Bad parameter: note must be of type string", "note");
            }
            if (parameters.TryGetValue("recipients", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recipients is null, which cannot be sent" : "Bad parameter: recipients must be of type object[]", "recipients");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/bundles/{System.Uri.EscapeDataString(attributes["id"].ToString())}/share", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Parameters:
        ///   paths - array(string) - A list of paths to include in this bundle.
        ///   password - string - Password for this bundle.
        ///   bypasses_site_expiration_rules - boolean - If true, this Share Link bypasses site-wide expiration rules. Only site admins may set this.
        ///   form_field_set_id - int64 - Id of Form Field Set to use with this bundle
        ///   clickwrap_id - int64 - ID of the clickwrap to use with this bundle.
        ///   code - string - Bundle code.  This code forms the end part of the Public URL.
        ///   create_snapshot - boolean - If true, create a snapshot of this bundle's contents.
        ///   description - string - Public description
        ///   dont_separate_submissions_by_folder - boolean - Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        ///   expires_at - string - Explicit Bundle expiration date/time. If not set, the site-wide expiration setting may apply.
        ///   finalize_snapshot - boolean - If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
        ///   inbox_id - int64 - ID of the associated inbox, if available.
        ///   max_uses - int64 - Maximum number of times bundle can be accessed
        ///   group_id - int64 - Owning group ID. If set, members of this group can view, edit, and share this Share Link.
        ///   internal_name - string - Internal name for identifying this Share Link.
        ///   note - string - Bundle internal note
        ///   path_template - string - Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
        ///   path_template_time_zone - string - Timezone to use when rendering timestamps in path templates.
        ///   permissions - string - Permissions that apply to Folders in this Share Link.
        ///   require_registration - boolean - Show a registration page that captures the downloader's name and email address?
        ///   require_share_recipient - boolean - Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        ///   send_one_time_password_to_recipient_at_registration - boolean - If true, require_share_recipient bundles will send a one-time password to the recipient when they register. Cannot be enabled if the bundle has a password set.
        ///   send_email_receipt_to_uploader - boolean - Send delivery receipt to the uploader. Note: For writable share only
        ///   skip_company - boolean - BundleRegistrations can be saved without providing company?
        ///   start_access_on_date - string - Date when share will start to be accessible. If `nil` access granted right after create.
        ///   skip_email - boolean - BundleRegistrations can be saved without providing email?
        ///   skip_name - boolean - BundleRegistrations can be saved without providing name?
        ///   user_id - int64 - The owning user id. Only site admins can set this.
        ///   watermark_attachment_delete - boolean - If true, will delete the file stored in watermark_attachment
        ///   watermark_attachment_file - file - Preview watermark image applied to all bundle items.
        ///   watermark_value - object - Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Bundle it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Bundle> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   paths - array(string) - A list of paths to include in this bundle.
        ///   password - string - Password for this bundle.
        ///   bypasses_site_expiration_rules - boolean - If true, this Share Link bypasses site-wide expiration rules. Only site admins may set this.
        ///   form_field_set_id - int64 - Id of Form Field Set to use with this bundle
        ///   clickwrap_id - int64 - ID of the clickwrap to use with this bundle.
        ///   code - string - Bundle code.  This code forms the end part of the Public URL.
        ///   create_snapshot - boolean - If true, create a snapshot of this bundle's contents.
        ///   description - string - Public description
        ///   dont_separate_submissions_by_folder - boolean - Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        ///   expires_at - string - Explicit Bundle expiration date/time. If not set, the site-wide expiration setting may apply.
        ///   finalize_snapshot - boolean - If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
        ///   inbox_id - int64 - ID of the associated inbox, if available.
        ///   max_uses - int64 - Maximum number of times bundle can be accessed
        ///   group_id - int64 - Owning group ID. If set, members of this group can view, edit, and share this Share Link.
        ///   internal_name - string - Internal name for identifying this Share Link.
        ///   note - string - Bundle internal note
        ///   path_template - string - Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
        ///   path_template_time_zone - string - Timezone to use when rendering timestamps in path templates.
        ///   permissions - string - Permissions that apply to Folders in this Share Link.
        ///   require_registration - boolean - Show a registration page that captures the downloader's name and email address?
        ///   require_share_recipient - boolean - Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        ///   send_one_time_password_to_recipient_at_registration - boolean - If true, require_share_recipient bundles will send a one-time password to the recipient when they register. Cannot be enabled if the bundle has a password set.
        ///   send_email_receipt_to_uploader - boolean - Send delivery receipt to the uploader. Note: For writable share only
        ///   skip_company - boolean - BundleRegistrations can be saved without providing company?
        ///   start_access_on_date - string - Date when share will start to be accessible. If `nil` access granted right after create.
        ///   skip_email - boolean - BundleRegistrations can be saved without providing email?
        ///   skip_name - boolean - BundleRegistrations can be saved without providing name?
        ///   user_id - int64 - The owning user id. Only site admins can set this.
        ///   watermark_attachment_delete - boolean - If true, will delete the file stored in watermark_attachment
        ///   watermark_attachment_file - file - Preview watermark image applied to all bundle items.
        ///   watermark_value - object - Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Bundle it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Bundle> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Bundle> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Bundle has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: paths is null, which cannot be sent" : "Bad parameter: paths must be of type string[]", "paths");
            }
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("bypasses_site_expiration_rules", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypasses_site_expiration_rules is null, which cannot be sent" : "Bad parameter: bypasses_site_expiration_rules must be of type bool", "bypasses_site_expiration_rules");
            }
            if (parameters.TryGetValue("form_field_set_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: form_field_set_id is null, which cannot be sent" : "Bad parameter: form_field_set_id must be of type Nullable<Int64>", "form_field_set_id");
            }
            if (parameters.TryGetValue("clickwrap_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: clickwrap_id is null, which cannot be sent" : "Bad parameter: clickwrap_id must be of type Nullable<Int64>", "clickwrap_id");
            }
            if (parameters.TryGetValue("code", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: code is null, which cannot be sent" : "Bad parameter: code must be of type string", "code");
            }
            if (parameters.TryGetValue("create_snapshot", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: create_snapshot is null, which cannot be sent" : "Bad parameter: create_snapshot must be of type bool", "create_snapshot");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("dont_separate_submissions_by_folder", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dont_separate_submissions_by_folder is null, which cannot be sent" : "Bad parameter: dont_separate_submissions_by_folder must be of type bool", "dont_separate_submissions_by_folder");
            }
            if (parameters.TryGetValue("expires_at", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: expires_at is null, which cannot be sent" : "Bad parameter: expires_at must be of type string", "expires_at");
            }
            if (parameters.TryGetValue("finalize_snapshot", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: finalize_snapshot is null, which cannot be sent" : "Bad parameter: finalize_snapshot must be of type bool", "finalize_snapshot");
            }
            if (parameters.TryGetValue("inbox_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: inbox_id is null, which cannot be sent" : "Bad parameter: inbox_id must be of type Nullable<Int64>", "inbox_id");
            }
            if (parameters.TryGetValue("max_uses", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_uses is null, which cannot be sent" : "Bad parameter: max_uses must be of type Nullable<Int64>", "max_uses");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type Nullable<Int64>", "group_id");
            }
            if (parameters.TryGetValue("internal_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: internal_name is null, which cannot be sent" : "Bad parameter: internal_name must be of type string", "internal_name");
            }
            if (parameters.TryGetValue("note", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: note is null, which cannot be sent" : "Bad parameter: note must be of type string", "note");
            }
            if (parameters.TryGetValue("path_template", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_template is null, which cannot be sent" : "Bad parameter: path_template must be of type string", "path_template");
            }
            if (parameters.TryGetValue("path_template_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_template_time_zone is null, which cannot be sent" : "Bad parameter: path_template_time_zone must be of type string", "path_template_time_zone");
            }
            if (parameters.TryGetValue("permissions", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: permissions is null, which cannot be sent" : "Bad parameter: permissions must be of type string", "permissions");
            }
            if (parameters.TryGetValue("require_registration", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_registration is null, which cannot be sent" : "Bad parameter: require_registration must be of type bool", "require_registration");
            }
            if (parameters.TryGetValue("require_share_recipient", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_share_recipient is null, which cannot be sent" : "Bad parameter: require_share_recipient must be of type bool", "require_share_recipient");
            }
            if (parameters.TryGetValue("send_one_time_password_to_recipient_at_registration", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_one_time_password_to_recipient_at_registration is null, which cannot be sent" : "Bad parameter: send_one_time_password_to_recipient_at_registration must be of type bool", "send_one_time_password_to_recipient_at_registration");
            }
            if (parameters.TryGetValue("send_email_receipt_to_uploader", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_email_receipt_to_uploader is null, which cannot be sent" : "Bad parameter: send_email_receipt_to_uploader must be of type bool", "send_email_receipt_to_uploader");
            }
            if (parameters.TryGetValue("skip_company", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_company is null, which cannot be sent" : "Bad parameter: skip_company must be of type bool", "skip_company");
            }
            if (parameters.TryGetValue("start_access_on_date", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: start_access_on_date is null, which cannot be sent" : "Bad parameter: start_access_on_date must be of type string", "start_access_on_date");
            }
            if (parameters.TryGetValue("skip_email", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_email is null, which cannot be sent" : "Bad parameter: skip_email must be of type bool", "skip_email");
            }
            if (parameters.TryGetValue("skip_name", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_name is null, which cannot be sent" : "Bad parameter: skip_name must be of type bool", "skip_name");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("watermark_attachment_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: watermark_attachment_delete is null, which cannot be sent" : "Bad parameter: watermark_attachment_delete must be of type bool", "watermark_attachment_delete");
            }
            if (parameters.TryGetValue("watermark_attachment_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: watermark_attachment_file is null, which cannot be sent" : "Bad parameter: watermark_attachment_file must be of type FileAttachment or IDictionary<string, object>", "watermark_attachment_file");
            }
            if (parameters.TryGetValue("watermark_value", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: watermark_value is null, which cannot be sent" : "Bad parameter: watermark_value must be of type object", "watermark_value");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/bundles/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            Bundle updated = JsonUtil.DeserializeModelResponse<Bundle>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<Bundle>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /bundles/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /bundles/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>, but neither waits for the request nor reports its failure.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        [Obsolete("Destroy neither waits for the request nor reports its failure. Use DestroyAsync instead.")]
        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
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
                throw new MissingParameterException("The Bundle has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/bundles/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "user_id", "paths", "password", "bypasses_site_expiration_rules", "form_field_set_id", "create_snapshot", "dont_separate_submissions_by_folder", "expires_at", "finalize_snapshot", "max_uses", "group_id", "internal_name", "description", "note", "code", "path_template", "path_template_time_zone", "permissions", "require_registration", "clickwrap_id", "inbox_id", "require_share_recipient", "send_one_time_password_to_recipient_at_registration", "send_email_receipt_to_uploader", "skip_email", "skip_name", "skip_company", "start_access_on_date", "snapshot_id", "workspace_id", "watermark_attachment_file", "watermark_value" };
        private static readonly string[] UpdateParameterNames = { "id", "paths", "password", "bypasses_site_expiration_rules", "form_field_set_id", "clickwrap_id", "code", "create_snapshot", "description", "dont_separate_submissions_by_folder", "expires_at", "finalize_snapshot", "inbox_id", "max_uses", "group_id", "internal_name", "note", "path_template", "path_template_time_zone", "permissions", "require_registration", "require_share_recipient", "send_one_time_password_to_recipient_at_registration", "send_email_receipt_to_uploader", "skip_company", "start_access_on_date", "skip_email", "skip_name", "user_id", "watermark_attachment_delete", "watermark_attachment_file", "watermark_value", "workspace_id" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Bundle on Files.com, or updates it if it has an ID.
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
            Bundle created = await Bundle.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
                if (unsaved.Contains(name) || ChangedInPlace(name))
                {
                    toSave[name] = ToParameter(name, attributes[name]);
                }
            }
            return toSave;
        }

        // An array attribute, for a property or getAttributes to hand out. Its JSON is kept the first time, as the caller
        // can change the array, or an array in a model in it, without setting anything.
        private object HandedOut(string name)
        {
            object value = attributes[name];
            if (value != null && !handedOut.ContainsKey(name))
            {
                handedOut[name] = JsonSerializer.Serialize(value, JsonUtil.Options);
            }
            return value;
        }

        // Whether an array handed out has been changed in place since, at any depth.
        private bool ChangedInPlace(string name)
        {
            return handedOut.TryGetValue(name, out string handedOutJson) && JsonSerializer.Serialize(attributes[name], JsonUtil.Options) != handedOutJson;
        }

        // The value SaveAsync sends for an attribute whose property type is not the one its parameter takes.
        private static object ToParameter(string name, object value)
        {
            switch (name)
            {
                case "expires_at":
                case "start_access_on_date":
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
            handedOut.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `expires_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `expires_at`, `code`, `group_id`, `user_id` or `bypasses_site_expiration_rules`. Valid field combinations are `[ group_id, expires_at ]` and `[ user_id, expires_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at` and `expires_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at` and `expires_at`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `code`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at` and `expires_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at` and `expires_at`.
        ///   deleted - boolean - If true, only list deleted Share Links.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Bundle> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `expires_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `expires_at`, `code`, `group_id`, `user_id` or `bypasses_site_expiration_rules`. Valid field combinations are `[ group_id, expires_at ]` and `[ user_id, expires_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at` and `expires_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at` and `expires_at`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `code`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at` and `expires_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at` and `expires_at`.
        ///   deleted - boolean - If true, only list deleted Share Links.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Bundle> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Bundle> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
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
            if (parameters.TryGetValue("deleted", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: deleted is null, which cannot be sent" : "Bad parameter: deleted must be of type bool", "deleted");
            }

            return new FilesList<Bundle>(client, $"/bundles", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Bundle ID.
        ///   deleted - boolean - If true, show a deleted Share Link.
        /// </summary>
        /// <param name="id">Bundle ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Bundle> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Bundle ID.
        ///   deleted - boolean - If true, show a deleted Share Link.
        /// </summary>
        /// <param name="id">Bundle ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Bundle> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<Bundle> FindCore(
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
            if (parameters.TryGetValue("deleted", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: deleted is null, which cannot be sent" : "Bad parameter: deleted must be of type bool", "deleted");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/bundles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Bundle>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   paths (required) - array(string) - A list of paths to include in this bundle.
        ///   password - string - Password for this bundle.
        ///   bypasses_site_expiration_rules - boolean - If true, this Share Link bypasses site-wide expiration rules. Only site admins may set this.
        ///   form_field_set_id - int64 - Id of Form Field Set to use with this bundle
        ///   create_snapshot - boolean - If true, create a snapshot of this bundle's contents.
        ///   dont_separate_submissions_by_folder - boolean - Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        ///   expires_at - string - Explicit Bundle expiration date/time. If not set, the site-wide expiration setting may apply.
        ///   finalize_snapshot - boolean - If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
        ///   max_uses - int64 - Maximum number of times bundle can be accessed
        ///   group_id - int64 - Owning group ID. If set, members of this group can view, edit, and share this Share Link.
        ///   internal_name - string - Internal name for identifying this Share Link.
        ///   description - string - Public description
        ///   note - string - Bundle internal note
        ///   code - string - Bundle code.  This code forms the end part of the Public URL.
        ///   path_template - string - Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
        ///   path_template_time_zone - string - Timezone to use when rendering timestamps in path templates.
        ///   permissions - string - Permissions that apply to Folders in this Share Link.
        ///   require_registration - boolean - Show a registration page that captures the downloader's name and email address?
        ///   clickwrap_id - int64 - ID of the clickwrap to use with this bundle.
        ///   inbox_id - int64 - ID of the associated inbox, if available.
        ///   require_share_recipient - boolean - Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        ///   send_one_time_password_to_recipient_at_registration - boolean - If true, require_share_recipient bundles will send a one-time password to the recipient when they register. Cannot be enabled if the bundle has a password set.
        ///   send_email_receipt_to_uploader - boolean - Send delivery receipt to the uploader. Note: For writable share only
        ///   skip_email - boolean - BundleRegistrations can be saved without providing email?
        ///   skip_name - boolean - BundleRegistrations can be saved without providing name?
        ///   skip_company - boolean - BundleRegistrations can be saved without providing company?
        ///   start_access_on_date - string - Date when share will start to be accessible. If `nil` access granted right after create.
        ///   snapshot_id - int64 - ID of the snapshot containing this bundle's contents.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        ///   watermark_attachment_file - file - Preview watermark image applied to all bundle items.
        ///   watermark_value - object - Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Bundle> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Bundle> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("paths", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: paths", "paths");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: paths is null, which cannot be sent" : "Bad parameter: paths must be of type string[]", "paths");
            }
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("bypasses_site_expiration_rules", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypasses_site_expiration_rules is null, which cannot be sent" : "Bad parameter: bypasses_site_expiration_rules must be of type bool", "bypasses_site_expiration_rules");
            }
            if (parameters.TryGetValue("form_field_set_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: form_field_set_id is null, which cannot be sent" : "Bad parameter: form_field_set_id must be of type Nullable<Int64>", "form_field_set_id");
            }
            if (parameters.TryGetValue("create_snapshot", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: create_snapshot is null, which cannot be sent" : "Bad parameter: create_snapshot must be of type bool", "create_snapshot");
            }
            if (parameters.TryGetValue("dont_separate_submissions_by_folder", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dont_separate_submissions_by_folder is null, which cannot be sent" : "Bad parameter: dont_separate_submissions_by_folder must be of type bool", "dont_separate_submissions_by_folder");
            }
            if (parameters.TryGetValue("expires_at", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: expires_at is null, which cannot be sent" : "Bad parameter: expires_at must be of type string", "expires_at");
            }
            if (parameters.TryGetValue("finalize_snapshot", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: finalize_snapshot is null, which cannot be sent" : "Bad parameter: finalize_snapshot must be of type bool", "finalize_snapshot");
            }
            if (parameters.TryGetValue("max_uses", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_uses is null, which cannot be sent" : "Bad parameter: max_uses must be of type Nullable<Int64>", "max_uses");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type Nullable<Int64>", "group_id");
            }
            if (parameters.TryGetValue("internal_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: internal_name is null, which cannot be sent" : "Bad parameter: internal_name must be of type string", "internal_name");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("note", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: note is null, which cannot be sent" : "Bad parameter: note must be of type string", "note");
            }
            if (parameters.TryGetValue("code", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: code is null, which cannot be sent" : "Bad parameter: code must be of type string", "code");
            }
            if (parameters.TryGetValue("path_template", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_template is null, which cannot be sent" : "Bad parameter: path_template must be of type string", "path_template");
            }
            if (parameters.TryGetValue("path_template_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_template_time_zone is null, which cannot be sent" : "Bad parameter: path_template_time_zone must be of type string", "path_template_time_zone");
            }
            if (parameters.TryGetValue("permissions", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: permissions is null, which cannot be sent" : "Bad parameter: permissions must be of type string", "permissions");
            }
            if (parameters.TryGetValue("require_registration", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_registration is null, which cannot be sent" : "Bad parameter: require_registration must be of type bool", "require_registration");
            }
            if (parameters.TryGetValue("clickwrap_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: clickwrap_id is null, which cannot be sent" : "Bad parameter: clickwrap_id must be of type Nullable<Int64>", "clickwrap_id");
            }
            if (parameters.TryGetValue("inbox_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: inbox_id is null, which cannot be sent" : "Bad parameter: inbox_id must be of type Nullable<Int64>", "inbox_id");
            }
            if (parameters.TryGetValue("require_share_recipient", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_share_recipient is null, which cannot be sent" : "Bad parameter: require_share_recipient must be of type bool", "require_share_recipient");
            }
            if (parameters.TryGetValue("send_one_time_password_to_recipient_at_registration", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_one_time_password_to_recipient_at_registration is null, which cannot be sent" : "Bad parameter: send_one_time_password_to_recipient_at_registration must be of type bool", "send_one_time_password_to_recipient_at_registration");
            }
            if (parameters.TryGetValue("send_email_receipt_to_uploader", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_email_receipt_to_uploader is null, which cannot be sent" : "Bad parameter: send_email_receipt_to_uploader must be of type bool", "send_email_receipt_to_uploader");
            }
            if (parameters.TryGetValue("skip_email", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_email is null, which cannot be sent" : "Bad parameter: skip_email must be of type bool", "skip_email");
            }
            if (parameters.TryGetValue("skip_name", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_name is null, which cannot be sent" : "Bad parameter: skip_name must be of type bool", "skip_name");
            }
            if (parameters.TryGetValue("skip_company", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_company is null, which cannot be sent" : "Bad parameter: skip_company must be of type bool", "skip_company");
            }
            if (parameters.TryGetValue("start_access_on_date", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: start_access_on_date is null, which cannot be sent" : "Bad parameter: start_access_on_date must be of type string", "start_access_on_date");
            }
            if (parameters.TryGetValue("snapshot_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: snapshot_id is null, which cannot be sent" : "Bad parameter: snapshot_id must be of type Nullable<Int64>", "snapshot_id");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("watermark_attachment_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: watermark_attachment_file is null, which cannot be sent" : "Bad parameter: watermark_attachment_file must be of type FileAttachment or IDictionary<string, object>", "watermark_attachment_file");
            }
            if (parameters.TryGetValue("watermark_value", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: watermark_value is null, which cannot be sent" : "Bad parameter: watermark_value must be of type object", "watermark_value");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/bundles", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Bundle>(responseJson, context, options);
        }

        /// <summary>
        /// Send email(s) with a link to bundle
        ///
        /// Parameters:
        ///   to - array(string) - A list of email addresses to share this bundle with. Required unless `recipients` is used.
        ///   note - string - Note to include in email.
        ///   recipients - array(object) - A list of recipients to share this bundle with. Required unless `to` is used.
        /// </summary>
        /// <param name="id">Bundle ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Share(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ShareCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task ShareCore(
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
            if (parameters.TryGetValue("to", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to is null, which cannot be sent" : "Bad parameter: to must be of type string[]", "to");
            }
            if (parameters.TryGetValue("note", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: note is null, which cannot be sent" : "Bad parameter: note must be of type string", "note");
            }
            if (parameters.TryGetValue("recipients", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recipients is null, which cannot be sent" : "Bad parameter: recipients must be of type object[]", "recipients");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/bundles/{System.Uri.EscapeDataString(parameters["id"].ToString())}/share", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   paths - array(string) - A list of paths to include in this bundle.
        ///   password - string - Password for this bundle.
        ///   bypasses_site_expiration_rules - boolean - If true, this Share Link bypasses site-wide expiration rules. Only site admins may set this.
        ///   form_field_set_id - int64 - Id of Form Field Set to use with this bundle
        ///   clickwrap_id - int64 - ID of the clickwrap to use with this bundle.
        ///   code - string - Bundle code.  This code forms the end part of the Public URL.
        ///   create_snapshot - boolean - If true, create a snapshot of this bundle's contents.
        ///   description - string - Public description
        ///   dont_separate_submissions_by_folder - boolean - Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        ///   expires_at - string - Explicit Bundle expiration date/time. If not set, the site-wide expiration setting may apply.
        ///   finalize_snapshot - boolean - If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
        ///   inbox_id - int64 - ID of the associated inbox, if available.
        ///   max_uses - int64 - Maximum number of times bundle can be accessed
        ///   group_id - int64 - Owning group ID. If set, members of this group can view, edit, and share this Share Link.
        ///   internal_name - string - Internal name for identifying this Share Link.
        ///   note - string - Bundle internal note
        ///   path_template - string - Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
        ///   path_template_time_zone - string - Timezone to use when rendering timestamps in path templates.
        ///   permissions - string - Permissions that apply to Folders in this Share Link.
        ///   require_registration - boolean - Show a registration page that captures the downloader's name and email address?
        ///   require_share_recipient - boolean - Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        ///   send_one_time_password_to_recipient_at_registration - boolean - If true, require_share_recipient bundles will send a one-time password to the recipient when they register. Cannot be enabled if the bundle has a password set.
        ///   send_email_receipt_to_uploader - boolean - Send delivery receipt to the uploader. Note: For writable share only
        ///   skip_company - boolean - BundleRegistrations can be saved without providing company?
        ///   start_access_on_date - string - Date when share will start to be accessible. If `nil` access granted right after create.
        ///   skip_email - boolean - BundleRegistrations can be saved without providing email?
        ///   skip_name - boolean - BundleRegistrations can be saved without providing name?
        ///   user_id - int64 - The owning user id. Only site admins can set this.
        ///   watermark_attachment_delete - boolean - If true, will delete the file stored in watermark_attachment
        ///   watermark_attachment_file - file - Preview watermark image applied to all bundle items.
        ///   watermark_value - object - Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="id">Bundle ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Bundle> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<Bundle> UpdateCore(
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
            if (parameters.TryGetValue("paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: paths is null, which cannot be sent" : "Bad parameter: paths must be of type string[]", "paths");
            }
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("bypasses_site_expiration_rules", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: bypasses_site_expiration_rules is null, which cannot be sent" : "Bad parameter: bypasses_site_expiration_rules must be of type bool", "bypasses_site_expiration_rules");
            }
            if (parameters.TryGetValue("form_field_set_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: form_field_set_id is null, which cannot be sent" : "Bad parameter: form_field_set_id must be of type Nullable<Int64>", "form_field_set_id");
            }
            if (parameters.TryGetValue("clickwrap_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: clickwrap_id is null, which cannot be sent" : "Bad parameter: clickwrap_id must be of type Nullable<Int64>", "clickwrap_id");
            }
            if (parameters.TryGetValue("code", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: code is null, which cannot be sent" : "Bad parameter: code must be of type string", "code");
            }
            if (parameters.TryGetValue("create_snapshot", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: create_snapshot is null, which cannot be sent" : "Bad parameter: create_snapshot must be of type bool", "create_snapshot");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("dont_separate_submissions_by_folder", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dont_separate_submissions_by_folder is null, which cannot be sent" : "Bad parameter: dont_separate_submissions_by_folder must be of type bool", "dont_separate_submissions_by_folder");
            }
            if (parameters.TryGetValue("expires_at", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: expires_at is null, which cannot be sent" : "Bad parameter: expires_at must be of type string", "expires_at");
            }
            if (parameters.TryGetValue("finalize_snapshot", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: finalize_snapshot is null, which cannot be sent" : "Bad parameter: finalize_snapshot must be of type bool", "finalize_snapshot");
            }
            if (parameters.TryGetValue("inbox_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: inbox_id is null, which cannot be sent" : "Bad parameter: inbox_id must be of type Nullable<Int64>", "inbox_id");
            }
            if (parameters.TryGetValue("max_uses", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_uses is null, which cannot be sent" : "Bad parameter: max_uses must be of type Nullable<Int64>", "max_uses");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type Nullable<Int64>", "group_id");
            }
            if (parameters.TryGetValue("internal_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: internal_name is null, which cannot be sent" : "Bad parameter: internal_name must be of type string", "internal_name");
            }
            if (parameters.TryGetValue("note", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: note is null, which cannot be sent" : "Bad parameter: note must be of type string", "note");
            }
            if (parameters.TryGetValue("path_template", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_template is null, which cannot be sent" : "Bad parameter: path_template must be of type string", "path_template");
            }
            if (parameters.TryGetValue("path_template_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_template_time_zone is null, which cannot be sent" : "Bad parameter: path_template_time_zone must be of type string", "path_template_time_zone");
            }
            if (parameters.TryGetValue("permissions", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: permissions is null, which cannot be sent" : "Bad parameter: permissions must be of type string", "permissions");
            }
            if (parameters.TryGetValue("require_registration", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_registration is null, which cannot be sent" : "Bad parameter: require_registration must be of type bool", "require_registration");
            }
            if (parameters.TryGetValue("require_share_recipient", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_share_recipient is null, which cannot be sent" : "Bad parameter: require_share_recipient must be of type bool", "require_share_recipient");
            }
            if (parameters.TryGetValue("send_one_time_password_to_recipient_at_registration", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_one_time_password_to_recipient_at_registration is null, which cannot be sent" : "Bad parameter: send_one_time_password_to_recipient_at_registration must be of type bool", "send_one_time_password_to_recipient_at_registration");
            }
            if (parameters.TryGetValue("send_email_receipt_to_uploader", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_email_receipt_to_uploader is null, which cannot be sent" : "Bad parameter: send_email_receipt_to_uploader must be of type bool", "send_email_receipt_to_uploader");
            }
            if (parameters.TryGetValue("skip_company", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_company is null, which cannot be sent" : "Bad parameter: skip_company must be of type bool", "skip_company");
            }
            if (parameters.TryGetValue("start_access_on_date", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: start_access_on_date is null, which cannot be sent" : "Bad parameter: start_access_on_date must be of type string", "start_access_on_date");
            }
            if (parameters.TryGetValue("skip_email", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_email is null, which cannot be sent" : "Bad parameter: skip_email must be of type bool", "skip_email");
            }
            if (parameters.TryGetValue("skip_name", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_name is null, which cannot be sent" : "Bad parameter: skip_name must be of type bool", "skip_name");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("watermark_attachment_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: watermark_attachment_delete is null, which cannot be sent" : "Bad parameter: watermark_attachment_delete must be of type bool", "watermark_attachment_delete");
            }
            if (parameters.TryGetValue("watermark_attachment_file", out parameterValue) && !(parameterValue is FileAttachment || parameterValue is IDictionary<string, object>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: watermark_attachment_file is null, which cannot be sent" : "Bad parameter: watermark_attachment_file must be of type FileAttachment or IDictionary<string, object>", "watermark_attachment_file");
            }
            if (parameters.TryGetValue("watermark_value", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: watermark_value is null, which cannot be sent" : "Bad parameter: watermark_value must be of type object", "watermark_value");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/bundles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Bundle>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /bundles/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Bundle ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
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
        /// Sends DELETE /bundles/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Bundle ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/bundles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}