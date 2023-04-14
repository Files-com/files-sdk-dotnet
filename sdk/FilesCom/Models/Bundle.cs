using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Bundle
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Bundle() : this(null, null) { }

        public Bundle(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("code"))
            {
                this.attributes.Add("code", null);
            }
            if (!this.attributes.ContainsKey("url"))
            {
                this.attributes.Add("url", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("password_protected"))
            {
                this.attributes.Add("password_protected", null);
            }
            if (!this.attributes.ContainsKey("permissions"))
            {
                this.attributes.Add("permissions", null);
            }
            if (!this.attributes.ContainsKey("preview_only"))
            {
                this.attributes.Add("preview_only", null);
            }
            if (!this.attributes.ContainsKey("require_registration"))
            {
                this.attributes.Add("require_registration", null);
            }
            if (!this.attributes.ContainsKey("require_share_recipient"))
            {
                this.attributes.Add("require_share_recipient", null);
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
                this.attributes.Add("skip_name", null);
            }
            if (!this.attributes.ContainsKey("skip_email"))
            {
                this.attributes.Add("skip_email", null);
            }
            if (!this.attributes.ContainsKey("skip_company"))
            {
                this.attributes.Add("skip_company", null);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("dont_separate_submissions_by_folder"))
            {
                this.attributes.Add("dont_separate_submissions_by_folder", null);
            }
            if (!this.attributes.ContainsKey("expires_at"))
            {
                this.attributes.Add("expires_at", null);
            }
            if (!this.attributes.ContainsKey("max_uses"))
            {
                this.attributes.Add("max_uses", null);
            }
            if (!this.attributes.ContainsKey("note"))
            {
                this.attributes.Add("note", null);
            }
            if (!this.attributes.ContainsKey("path_template"))
            {
                this.attributes.Add("path_template", null);
            }
            if (!this.attributes.ContainsKey("send_email_receipt_to_uploader"))
            {
                this.attributes.Add("send_email_receipt_to_uploader", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
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
            if (!this.attributes.ContainsKey("has_inbox"))
            {
                this.attributes.Add("has_inbox", null);
            }
            if (!this.attributes.ContainsKey("paths"))
            {
                this.attributes.Add("paths", new string[0]);
            }
            if (!this.attributes.ContainsKey("password"))
            {
                this.attributes.Add("password", null);
            }
            if (!this.attributes.ContainsKey("form_field_set_id"))
            {
                this.attributes.Add("form_field_set_id", null);
            }
            if (!this.attributes.ContainsKey("watermark_attachment_file"))
            {
                this.attributes.Add("watermark_attachment_file", null);
            }
            if (!this.attributes.ContainsKey("watermark_attachment_delete"))
            {
                this.attributes.Add("watermark_attachment_delete", null);
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
        /// Bundle code.  This code forms the end part of the Public URL.
        /// </summary>
        [JsonPropertyName("code")]
        public string Code
        {
            get { return (string)attributes["code"]; }
            set { attributes["code"] = value; }
        }

        /// <summary>
        /// Public URL of Share Link
        /// </summary>
        [JsonPropertyName("url")]
        public string Url
        {
            get { return (string)attributes["url"]; }
            set { attributes["url"] = value; }
        }

        /// <summary>
        /// Public description
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; }
        }

        /// <summary>
        /// Is this bundle password protected?
        /// </summary>
        [JsonPropertyName("password_protected")]
        public bool PasswordProtected
        {
            get { return (bool)attributes["password_protected"]; }
            set { attributes["password_protected"] = value; }
        }

        /// <summary>
        /// Permissions that apply to Folders in this Share Link.
        /// </summary>
        [JsonPropertyName("permissions")]
        public string Permissions
        {
            get { return (string)attributes["permissions"]; }
            set { attributes["permissions"] = value; }
        }

        /// <summary>
        /// Restrict users to previewing files only?
        /// </summary>
        [JsonPropertyName("preview_only")]
        public bool PreviewOnly
        {
            get { return (bool)attributes["preview_only"]; }
            set { attributes["preview_only"] = value; }
        }

        /// <summary>
        /// Show a registration page that captures the downloader's name and email address?
        /// </summary>
        [JsonPropertyName("require_registration")]
        public bool RequireRegistration
        {
            get { return (bool)attributes["require_registration"]; }
            set { attributes["require_registration"] = value; }
        }

        /// <summary>
        /// Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        /// </summary>
        [JsonPropertyName("require_share_recipient")]
        public bool RequireShareRecipient
        {
            get { return (bool)attributes["require_share_recipient"]; }
            set { attributes["require_share_recipient"] = value; }
        }

        /// <summary>
        /// Legal text that must be agreed to prior to accessing Bundle.
        /// </summary>
        [JsonPropertyName("clickwrap_body")]
        public string ClickwrapBody
        {
            get { return (string)attributes["clickwrap_body"]; }
            set { attributes["clickwrap_body"] = value; }
        }

        /// <summary>
        /// Custom Form to use
        /// </summary>
        [JsonPropertyName("form_field_set")]
        public FormFieldSet FormFieldSet
        {
            get { return (FormFieldSet)attributes["form_field_set"]; }
            set { attributes["form_field_set"] = value; }
        }

        /// <summary>
        /// BundleRegistrations can be saved without providing name?
        /// </summary>
        [JsonPropertyName("skip_name")]
        public bool SkipName
        {
            get { return (bool)attributes["skip_name"]; }
            set { attributes["skip_name"] = value; }
        }

        /// <summary>
        /// BundleRegistrations can be saved without providing email?
        /// </summary>
        [JsonPropertyName("skip_email")]
        public bool SkipEmail
        {
            get { return (bool)attributes["skip_email"]; }
            set { attributes["skip_email"] = value; }
        }

        /// <summary>
        /// BundleRegistrations can be saved without providing company?
        /// </summary>
        [JsonPropertyName("skip_company")]
        public bool SkipCompany
        {
            get { return (bool)attributes["skip_company"]; }
            set { attributes["skip_company"] = value; }
        }

        /// <summary>
        /// Bundle ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
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
        /// Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        /// </summary>
        [JsonPropertyName("dont_separate_submissions_by_folder")]
        public bool DontSeparateSubmissionsByFolder
        {
            get { return (bool)attributes["dont_separate_submissions_by_folder"]; }
            set { attributes["dont_separate_submissions_by_folder"] = value; }
        }

        /// <summary>
        /// Bundle expiration date/time
        /// </summary>
        [JsonPropertyName("expires_at")]
        public Nullable<DateTime> ExpiresAt
        {
            get { return (Nullable<DateTime>)attributes["expires_at"]; }
            set { attributes["expires_at"] = value; }
        }

        /// <summary>
        /// Maximum number of times bundle can be accessed
        /// </summary>
        [JsonPropertyName("max_uses")]
        public Nullable<Int64> MaxUses
        {
            get { return (Nullable<Int64>)attributes["max_uses"]; }
            set { attributes["max_uses"] = value; }
        }

        /// <summary>
        /// Bundle internal note
        /// </summary>
        [JsonPropertyName("note")]
        public string Note
        {
            get { return (string)attributes["note"]; }
            set { attributes["note"] = value; }
        }

        /// <summary>
        /// Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, and any custom form data.
        /// </summary>
        [JsonPropertyName("path_template")]
        public string PathTemplate
        {
            get { return (string)attributes["path_template"]; }
            set { attributes["path_template"] = value; }
        }

        /// <summary>
        /// Send delivery receipt to the uploader. Note: For writable share only
        /// </summary>
        [JsonPropertyName("send_email_receipt_to_uploader")]
        public bool SendEmailReceiptToUploader
        {
            get { return (bool)attributes["send_email_receipt_to_uploader"]; }
            set { attributes["send_email_receipt_to_uploader"] = value; }
        }

        /// <summary>
        /// Bundle creator user ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Bundle creator username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// ID of the clickwrap to use with this bundle.
        /// </summary>
        [JsonPropertyName("clickwrap_id")]
        public Nullable<Int64> ClickwrapId
        {
            get { return (Nullable<Int64>)attributes["clickwrap_id"]; }
            set { attributes["clickwrap_id"] = value; }
        }

        /// <summary>
        /// ID of the associated inbox, if available.
        /// </summary>
        [JsonPropertyName("inbox_id")]
        public Nullable<Int64> InboxId
        {
            get { return (Nullable<Int64>)attributes["inbox_id"]; }
            set { attributes["inbox_id"] = value; }
        }

        /// <summary>
        /// Preview watermark image applied to all bundle items.
        /// </summary>
        [JsonPropertyName("watermark_attachment")]
        public Image WatermarkAttachment
        {
            get { return (Image)attributes["watermark_attachment"]; }
            set { attributes["watermark_attachment"] = value; }
        }

        /// <summary>
        /// Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
        /// </summary>
        [JsonPropertyName("watermark_value")]
        public object WatermarkValue
        {
            get { return (object)attributes["watermark_value"]; }
            set { attributes["watermark_value"] = value; }
        }

        /// <summary>
        /// Does this bundle have an associated inbox?
        /// </summary>
        [JsonPropertyName("has_inbox")]
        public bool HasInbox
        {
            get { return (bool)attributes["has_inbox"]; }
            set { attributes["has_inbox"] = value; }
        }

        /// <summary>
        /// A list of paths in this bundle.  For performance reasons, this is not provided when listing bundles.
        /// </summary>
        [JsonPropertyName("paths")]
        public string[] Paths
        {
            get { return (string[])attributes["paths"]; }
            set { attributes["paths"] = value; }
        }

        /// <summary>
        /// Password for this bundle.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string)attributes["password"]; }
            set { attributes["password"] = value; }
        }

        /// <summary>
        /// Id of Form Field Set to use with this bundle
        /// </summary>
        [JsonPropertyName("form_field_set_id")]
        public Nullable<Int64> FormFieldSetId
        {
            get { return (Nullable<Int64>)attributes["form_field_set_id"]; }
            set { attributes["form_field_set_id"] = value; }
        }

        /// <summary>
        /// Preview watermark image applied to all bundle items.
        /// </summary>
        [JsonPropertyName("watermark_attachment_file")]
        public System.Net.Http.ByteArrayContent WatermarkAttachmentFile
        {
            get { return (System.Net.Http.ByteArrayContent)attributes["watermark_attachment_file"]; }
            set { attributes["watermark_attachment_file"] = value; }
        }

        /// <summary>
        /// If true, will delete the file stored in watermark_attachment
        /// </summary>
        [JsonPropertyName("watermark_attachment_delete")]
        public bool WatermarkAttachmentDelete
        {
            get { return (bool)attributes["watermark_attachment_delete"]; }
            set { attributes["watermark_attachment_delete"] = value; }
        }

        /// <summary>
        /// Send email(s) with a link to bundle
        ///
        /// Parameters:
        ///   to - array(string) - A list of email addresses to share this bundle with. Required unless `recipients` is used.
        ///   note - string - Note to include in email.
        ///   recipients - array(object) - A list of recipients to share this bundle with. Required unless `to` is used.
        /// </summary>
        public async Task<Bundle> Share(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("to") && !(parameters["to"] is string[]))
            {
                throw new ArgumentException("Bad parameter: to must be of type string[]", "parameters[\"to\"]");
            }
            if (parameters.ContainsKey("note") && !(parameters["note"] is string))
            {
                throw new ArgumentException("Bad parameter: note must be of type string", "parameters[\"note\"]");
            }
            if (parameters.ContainsKey("recipients") && !(parameters["recipients"] is object[]))
            {
                throw new ArgumentException("Bad parameter: recipients must be of type object[]", "parameters[\"recipients\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundles/{System.Uri.EscapeDataString(attributes["id"].ToString())}/share", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Bundle>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   paths - array(string) - A list of paths to include in this bundle.
        ///   password - string - Password for this bundle.
        ///   form_field_set_id - int64 - Id of Form Field Set to use with this bundle
        ///   clickwrap_id - int64 - ID of the clickwrap to use with this bundle.
        ///   code - string - Bundle code.  This code forms the end part of the Public URL.
        ///   description - string - Public description
        ///   dont_separate_submissions_by_folder - boolean - Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        ///   expires_at - string - Bundle expiration date/time
        ///   inbox_id - int64 - ID of the associated inbox, if available.
        ///   max_uses - int64 - Maximum number of times bundle can be accessed
        ///   note - string - Bundle internal note
        ///   path_template - string - Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, and any custom form data.
        ///   permissions - string - Permissions that apply to Folders in this Share Link.
        ///   preview_only - boolean - Restrict users to previewing files only?
        ///   require_registration - boolean - Show a registration page that captures the downloader's name and email address?
        ///   require_share_recipient - boolean - Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        ///   send_email_receipt_to_uploader - boolean - Send delivery receipt to the uploader. Note: For writable share only
        ///   skip_company - boolean - BundleRegistrations can be saved without providing company?
        ///   skip_email - boolean - BundleRegistrations can be saved without providing email?
        ///   skip_name - boolean - BundleRegistrations can be saved without providing name?
        ///   watermark_attachment_delete - boolean - If true, will delete the file stored in watermark_attachment
        ///   watermark_attachment_file - file - Preview watermark image applied to all bundle items.
        /// </summary>
        public async Task<Bundle> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("paths") && !(parameters["paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: paths must be of type string[]", "parameters[\"paths\"]");
            }
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("form_field_set_id") && !(parameters["form_field_set_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: form_field_set_id must be of type Nullable<Int64>", "parameters[\"form_field_set_id\"]");
            }
            if (parameters.ContainsKey("clickwrap_id") && !(parameters["clickwrap_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: clickwrap_id must be of type Nullable<Int64>", "parameters[\"clickwrap_id\"]");
            }
            if (parameters.ContainsKey("code") && !(parameters["code"] is string))
            {
                throw new ArgumentException("Bad parameter: code must be of type string", "parameters[\"code\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("dont_separate_submissions_by_folder") && !(parameters["dont_separate_submissions_by_folder"] is bool))
            {
                throw new ArgumentException("Bad parameter: dont_separate_submissions_by_folder must be of type bool", "parameters[\"dont_separate_submissions_by_folder\"]");
            }
            if (parameters.ContainsKey("expires_at") && !(parameters["expires_at"] is string))
            {
                throw new ArgumentException("Bad parameter: expires_at must be of type string", "parameters[\"expires_at\"]");
            }
            if (parameters.ContainsKey("inbox_id") && !(parameters["inbox_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inbox_id must be of type Nullable<Int64>", "parameters[\"inbox_id\"]");
            }
            if (parameters.ContainsKey("max_uses") && !(parameters["max_uses"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_uses must be of type Nullable<Int64>", "parameters[\"max_uses\"]");
            }
            if (parameters.ContainsKey("note") && !(parameters["note"] is string))
            {
                throw new ArgumentException("Bad parameter: note must be of type string", "parameters[\"note\"]");
            }
            if (parameters.ContainsKey("path_template") && !(parameters["path_template"] is string))
            {
                throw new ArgumentException("Bad parameter: path_template must be of type string", "parameters[\"path_template\"]");
            }
            if (parameters.ContainsKey("permissions") && !(parameters["permissions"] is string))
            {
                throw new ArgumentException("Bad parameter: permissions must be of type string", "parameters[\"permissions\"]");
            }
            if (parameters.ContainsKey("preview_only") && !(parameters["preview_only"] is bool))
            {
                throw new ArgumentException("Bad parameter: preview_only must be of type bool", "parameters[\"preview_only\"]");
            }
            if (parameters.ContainsKey("require_registration") && !(parameters["require_registration"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_registration must be of type bool", "parameters[\"require_registration\"]");
            }
            if (parameters.ContainsKey("require_share_recipient") && !(parameters["require_share_recipient"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_share_recipient must be of type bool", "parameters[\"require_share_recipient\"]");
            }
            if (parameters.ContainsKey("send_email_receipt_to_uploader") && !(parameters["send_email_receipt_to_uploader"] is bool))
            {
                throw new ArgumentException("Bad parameter: send_email_receipt_to_uploader must be of type bool", "parameters[\"send_email_receipt_to_uploader\"]");
            }
            if (parameters.ContainsKey("skip_company") && !(parameters["skip_company"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_company must be of type bool", "parameters[\"skip_company\"]");
            }
            if (parameters.ContainsKey("skip_email") && !(parameters["skip_email"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_email must be of type bool", "parameters[\"skip_email\"]");
            }
            if (parameters.ContainsKey("skip_name") && !(parameters["skip_name"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_name must be of type bool", "parameters[\"skip_name\"]");
            }
            if (parameters.ContainsKey("watermark_attachment_delete") && !(parameters["watermark_attachment_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: watermark_attachment_delete must be of type bool", "parameters[\"watermark_attachment_delete\"]");
            }
            if (parameters.ContainsKey("watermark_attachment_file") && !(parameters["watermark_attachment_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: watermark_attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"watermark_attachment_file\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundles/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Bundle>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Bundle> Delete(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendRequest($"/bundles/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Bundle>(responseJson);
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
                var newObj = await Bundle.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[created_at]=desc`). Valid fields are `created_at` and `code`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        public static async Task<Bundle[]> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
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
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundles", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Bundle[]>(responseJson);
        }

        public static async Task<Bundle[]> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Bundle ID.
        /// </summary>
        public static async Task<Bundle> Find(
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

            string responseJson = await FilesClient.SendRequest($"/bundles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Bundle>(responseJson);
        }

        public static async Task<Bundle> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   paths (required) - array(string) - A list of paths to include in this bundle.
        ///   password - string - Password for this bundle.
        ///   form_field_set_id - int64 - Id of Form Field Set to use with this bundle
        ///   dont_separate_submissions_by_folder - boolean - Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        ///   expires_at - string - Bundle expiration date/time
        ///   max_uses - int64 - Maximum number of times bundle can be accessed
        ///   description - string - Public description
        ///   note - string - Bundle internal note
        ///   code - string - Bundle code.  This code forms the end part of the Public URL.
        ///   path_template - string - Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, and any custom form data.
        ///   permissions - string - Permissions that apply to Folders in this Share Link.
        ///   preview_only - boolean - Restrict users to previewing files only?
        ///   require_registration - boolean - Show a registration page that captures the downloader's name and email address?
        ///   clickwrap_id - int64 - ID of the clickwrap to use with this bundle.
        ///   inbox_id - int64 - ID of the associated inbox, if available.
        ///   require_share_recipient - boolean - Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        ///   send_email_receipt_to_uploader - boolean - Send delivery receipt to the uploader. Note: For writable share only
        ///   skip_email - boolean - BundleRegistrations can be saved without providing email?
        ///   skip_name - boolean - BundleRegistrations can be saved without providing name?
        ///   skip_company - boolean - BundleRegistrations can be saved without providing company?
        ///   watermark_attachment_file - file - Preview watermark image applied to all bundle items.
        /// </summary>
        public static async Task<Bundle> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("paths") && !(parameters["paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: paths must be of type string[]", "parameters[\"paths\"]");
            }
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("form_field_set_id") && !(parameters["form_field_set_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: form_field_set_id must be of type Nullable<Int64>", "parameters[\"form_field_set_id\"]");
            }
            if (parameters.ContainsKey("dont_separate_submissions_by_folder") && !(parameters["dont_separate_submissions_by_folder"] is bool))
            {
                throw new ArgumentException("Bad parameter: dont_separate_submissions_by_folder must be of type bool", "parameters[\"dont_separate_submissions_by_folder\"]");
            }
            if (parameters.ContainsKey("expires_at") && !(parameters["expires_at"] is string))
            {
                throw new ArgumentException("Bad parameter: expires_at must be of type string", "parameters[\"expires_at\"]");
            }
            if (parameters.ContainsKey("max_uses") && !(parameters["max_uses"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_uses must be of type Nullable<Int64>", "parameters[\"max_uses\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("note") && !(parameters["note"] is string))
            {
                throw new ArgumentException("Bad parameter: note must be of type string", "parameters[\"note\"]");
            }
            if (parameters.ContainsKey("code") && !(parameters["code"] is string))
            {
                throw new ArgumentException("Bad parameter: code must be of type string", "parameters[\"code\"]");
            }
            if (parameters.ContainsKey("path_template") && !(parameters["path_template"] is string))
            {
                throw new ArgumentException("Bad parameter: path_template must be of type string", "parameters[\"path_template\"]");
            }
            if (parameters.ContainsKey("permissions") && !(parameters["permissions"] is string))
            {
                throw new ArgumentException("Bad parameter: permissions must be of type string", "parameters[\"permissions\"]");
            }
            if (parameters.ContainsKey("preview_only") && !(parameters["preview_only"] is bool))
            {
                throw new ArgumentException("Bad parameter: preview_only must be of type bool", "parameters[\"preview_only\"]");
            }
            if (parameters.ContainsKey("require_registration") && !(parameters["require_registration"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_registration must be of type bool", "parameters[\"require_registration\"]");
            }
            if (parameters.ContainsKey("clickwrap_id") && !(parameters["clickwrap_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: clickwrap_id must be of type Nullable<Int64>", "parameters[\"clickwrap_id\"]");
            }
            if (parameters.ContainsKey("inbox_id") && !(parameters["inbox_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inbox_id must be of type Nullable<Int64>", "parameters[\"inbox_id\"]");
            }
            if (parameters.ContainsKey("require_share_recipient") && !(parameters["require_share_recipient"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_share_recipient must be of type bool", "parameters[\"require_share_recipient\"]");
            }
            if (parameters.ContainsKey("send_email_receipt_to_uploader") && !(parameters["send_email_receipt_to_uploader"] is bool))
            {
                throw new ArgumentException("Bad parameter: send_email_receipt_to_uploader must be of type bool", "parameters[\"send_email_receipt_to_uploader\"]");
            }
            if (parameters.ContainsKey("skip_email") && !(parameters["skip_email"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_email must be of type bool", "parameters[\"skip_email\"]");
            }
            if (parameters.ContainsKey("skip_name") && !(parameters["skip_name"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_name must be of type bool", "parameters[\"skip_name\"]");
            }
            if (parameters.ContainsKey("skip_company") && !(parameters["skip_company"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_company must be of type bool", "parameters[\"skip_company\"]");
            }
            if (parameters.ContainsKey("watermark_attachment_file") && !(parameters["watermark_attachment_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: watermark_attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"watermark_attachment_file\"]");
            }
            if (!parameters.ContainsKey("paths") || parameters["paths"] == null)
            {
                throw new ArgumentNullException("Parameter missing: paths", "parameters[\"paths\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundles", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Bundle>(responseJson);
        }


        /// <summary>
        /// Send email(s) with a link to bundle
        ///
        /// Parameters:
        ///   to - array(string) - A list of email addresses to share this bundle with. Required unless `recipients` is used.
        ///   note - string - Note to include in email.
        ///   recipients - array(object) - A list of recipients to share this bundle with. Required unless `to` is used.
        /// </summary>
        public static async Task<Bundle> Share(
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
            if (parameters.ContainsKey("to") && !(parameters["to"] is string[]))
            {
                throw new ArgumentException("Bad parameter: to must be of type string[]", "parameters[\"to\"]");
            }
            if (parameters.ContainsKey("note") && !(parameters["note"] is string))
            {
                throw new ArgumentException("Bad parameter: note must be of type string", "parameters[\"note\"]");
            }
            if (parameters.ContainsKey("recipients") && !(parameters["recipients"] is object[]))
            {
                throw new ArgumentException("Bad parameter: recipients must be of type object[]", "parameters[\"recipients\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundles/{System.Uri.EscapeDataString(parameters["id"].ToString())}/share", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Bundle>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   paths - array(string) - A list of paths to include in this bundle.
        ///   password - string - Password for this bundle.
        ///   form_field_set_id - int64 - Id of Form Field Set to use with this bundle
        ///   clickwrap_id - int64 - ID of the clickwrap to use with this bundle.
        ///   code - string - Bundle code.  This code forms the end part of the Public URL.
        ///   description - string - Public description
        ///   dont_separate_submissions_by_folder - boolean - Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
        ///   expires_at - string - Bundle expiration date/time
        ///   inbox_id - int64 - ID of the associated inbox, if available.
        ///   max_uses - int64 - Maximum number of times bundle can be accessed
        ///   note - string - Bundle internal note
        ///   path_template - string - Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, and any custom form data.
        ///   permissions - string - Permissions that apply to Folders in this Share Link.
        ///   preview_only - boolean - Restrict users to previewing files only?
        ///   require_registration - boolean - Show a registration page that captures the downloader's name and email address?
        ///   require_share_recipient - boolean - Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
        ///   send_email_receipt_to_uploader - boolean - Send delivery receipt to the uploader. Note: For writable share only
        ///   skip_company - boolean - BundleRegistrations can be saved without providing company?
        ///   skip_email - boolean - BundleRegistrations can be saved without providing email?
        ///   skip_name - boolean - BundleRegistrations can be saved without providing name?
        ///   watermark_attachment_delete - boolean - If true, will delete the file stored in watermark_attachment
        ///   watermark_attachment_file - file - Preview watermark image applied to all bundle items.
        /// </summary>
        public static async Task<Bundle> Update(
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
            if (parameters.ContainsKey("paths") && !(parameters["paths"] is string[]))
            {
                throw new ArgumentException("Bad parameter: paths must be of type string[]", "parameters[\"paths\"]");
            }
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("form_field_set_id") && !(parameters["form_field_set_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: form_field_set_id must be of type Nullable<Int64>", "parameters[\"form_field_set_id\"]");
            }
            if (parameters.ContainsKey("clickwrap_id") && !(parameters["clickwrap_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: clickwrap_id must be of type Nullable<Int64>", "parameters[\"clickwrap_id\"]");
            }
            if (parameters.ContainsKey("code") && !(parameters["code"] is string))
            {
                throw new ArgumentException("Bad parameter: code must be of type string", "parameters[\"code\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("dont_separate_submissions_by_folder") && !(parameters["dont_separate_submissions_by_folder"] is bool))
            {
                throw new ArgumentException("Bad parameter: dont_separate_submissions_by_folder must be of type bool", "parameters[\"dont_separate_submissions_by_folder\"]");
            }
            if (parameters.ContainsKey("expires_at") && !(parameters["expires_at"] is string))
            {
                throw new ArgumentException("Bad parameter: expires_at must be of type string", "parameters[\"expires_at\"]");
            }
            if (parameters.ContainsKey("inbox_id") && !(parameters["inbox_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inbox_id must be of type Nullable<Int64>", "parameters[\"inbox_id\"]");
            }
            if (parameters.ContainsKey("max_uses") && !(parameters["max_uses"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_uses must be of type Nullable<Int64>", "parameters[\"max_uses\"]");
            }
            if (parameters.ContainsKey("note") && !(parameters["note"] is string))
            {
                throw new ArgumentException("Bad parameter: note must be of type string", "parameters[\"note\"]");
            }
            if (parameters.ContainsKey("path_template") && !(parameters["path_template"] is string))
            {
                throw new ArgumentException("Bad parameter: path_template must be of type string", "parameters[\"path_template\"]");
            }
            if (parameters.ContainsKey("permissions") && !(parameters["permissions"] is string))
            {
                throw new ArgumentException("Bad parameter: permissions must be of type string", "parameters[\"permissions\"]");
            }
            if (parameters.ContainsKey("preview_only") && !(parameters["preview_only"] is bool))
            {
                throw new ArgumentException("Bad parameter: preview_only must be of type bool", "parameters[\"preview_only\"]");
            }
            if (parameters.ContainsKey("require_registration") && !(parameters["require_registration"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_registration must be of type bool", "parameters[\"require_registration\"]");
            }
            if (parameters.ContainsKey("require_share_recipient") && !(parameters["require_share_recipient"] is bool))
            {
                throw new ArgumentException("Bad parameter: require_share_recipient must be of type bool", "parameters[\"require_share_recipient\"]");
            }
            if (parameters.ContainsKey("send_email_receipt_to_uploader") && !(parameters["send_email_receipt_to_uploader"] is bool))
            {
                throw new ArgumentException("Bad parameter: send_email_receipt_to_uploader must be of type bool", "parameters[\"send_email_receipt_to_uploader\"]");
            }
            if (parameters.ContainsKey("skip_company") && !(parameters["skip_company"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_company must be of type bool", "parameters[\"skip_company\"]");
            }
            if (parameters.ContainsKey("skip_email") && !(parameters["skip_email"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_email must be of type bool", "parameters[\"skip_email\"]");
            }
            if (parameters.ContainsKey("skip_name") && !(parameters["skip_name"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_name must be of type bool", "parameters[\"skip_name\"]");
            }
            if (parameters.ContainsKey("watermark_attachment_delete") && !(parameters["watermark_attachment_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: watermark_attachment_delete must be of type bool", "parameters[\"watermark_attachment_delete\"]");
            }
            if (parameters.ContainsKey("watermark_attachment_file") && !(parameters["watermark_attachment_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: watermark_attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"watermark_attachment_file\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Bundle>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Bundle> Delete(
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

            string responseJson = await FilesClient.SendRequest($"/bundles/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Bundle>(responseJson);
        }

        public static async Task<Bundle> Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}