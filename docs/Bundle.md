# FilesCom.Models.Bundle

## Example Bundle Object

```
{
  "code": "abc123",
  "color_left": "#0066a7",
  "color_link": "#d34f5d",
  "color_text": "#0066a7",
  "color_top": "#000000",
  "color_top_text": "#ffffff",
  "url": "https://subdomain.files.com/f/12345678",
  "description": "The public description of the bundle.",
  "expires_at": "2000-01-01T01:00:00Z",
  "password_protected": true,
  "permissions": "read",
  "preview_only": true,
  "require_registration": true,
  "require_share_recipient": true,
  "require_logout": true,
  "clickwrap_body": "[Legal text]",
  "form_field_set": {
    "id": 1,
    "title": "Sample Form Title",
    "form_layout": [
      1,
      2,
      3,
      4
    ],
    "form_fields": [
      {
        "id": 1,
        "label": "Sample Label",
        "required": true,
        "help_text": "Help Text",
        "field_type": "text",
        "options_for_select": [
          "red",
          "green",
          "blue"
        ],
        "default_option": "red",
        "form_field_set_id": 1
      }
    ],
    "skip_name": true,
    "skip_email": true,
    "skip_company": true
  },
  "skip_name": true,
  "skip_email": true,
  "start_access_on_date": "2000-01-01T01:00:00Z",
  "skip_company": true,
  "id": 1,
  "created_at": "2000-01-01T01:00:00Z",
  "dont_separate_submissions_by_folder": true,
  "max_uses": 1,
  "note": "The internal note on the bundle.",
  "path_template": "{{name}}_{{ip}}",
  "path_template_time_zone": "Eastern Time (US & Canada)",
  "send_email_receipt_to_uploader": true,
  "snapshot_id": 1,
  "user_id": 1,
  "username": "user",
  "clickwrap_id": 1,
  "inbox_id": 1,
  "watermark_attachment": {
    "name": "My logo",
    "uri": "https://mysite.files.com/.../my_image.png"
  },
  "watermark_value": {
    "key": "example value"
  },
  "has_inbox": true,
  "dont_allow_folders_in_uploads": true,
  "paths": [
    "file.txt"
  ],
  "bundlepaths": [
    {
      "recursive": true,
      "path": "example"
    }
  ]
}
```

* `code` / `Code`  (string): Bundle code.  This code forms the end part of the Public URL.
* `color_left` / `ColorLeft`  (string): Page link and button color
* `color_link` / `ColorLink`  (string): Top bar link color
* `color_text` / `ColorText`  (string): Page link and button color
* `color_top` / `ColorTop`  (string): Top bar background color
* `color_top_text` / `ColorTopText`  (string): Top bar text color
* `url` / `Url`  (string): Public URL of Share Link
* `description` / `Description`  (string): Public description
* `expires_at` / `ExpiresAt`  (Nullable<DateTime>): Bundle expiration date/time
* `password_protected` / `PasswordProtected`  (bool): Is this bundle password protected?
* `permissions` / `Permissions`  (string): Permissions that apply to Folders in this Share Link.
* `preview_only` / `PreviewOnly`  (bool): 
* `require_registration` / `RequireRegistration`  (bool): Show a registration page that captures the downloader's name and email address?
* `require_share_recipient` / `RequireShareRecipient`  (bool): Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
* `require_logout` / `RequireLogout`  (bool): If true, we will hide the 'Remember Me' box on the Bundle registration page, requiring that the user logout and log back in every time they visit the page.
* `clickwrap_body` / `ClickwrapBody`  (string): Legal text that must be agreed to prior to accessing Bundle.
* `form_field_set` / `FormFieldSet`  (FormFieldSet): Custom Form to use
* `skip_name` / `SkipName`  (bool): BundleRegistrations can be saved without providing name?
* `skip_email` / `SkipEmail`  (bool): BundleRegistrations can be saved without providing email?
* `start_access_on_date` / `StartAccessOnDate`  (Nullable<DateTime>): Date when share will start to be accessible. If `nil` access granted right after create.
* `skip_company` / `SkipCompany`  (bool): BundleRegistrations can be saved without providing company?
* `id` / `Id`  (Nullable<Int64>): Bundle ID
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Bundle created at date/time
* `dont_separate_submissions_by_folder` / `DontSeparateSubmissionsByFolder`  (bool): Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
* `max_uses` / `MaxUses`  (Nullable<Int64>): Maximum number of times bundle can be accessed
* `note` / `Note`  (string): Bundle internal note
* `path_template` / `PathTemplate`  (string): Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
* `path_template_time_zone` / `PathTemplateTimeZone`  (string): Timezone to use when rendering timestamps in path templates.
* `send_email_receipt_to_uploader` / `SendEmailReceiptToUploader`  (bool): Send delivery receipt to the uploader. Note: For writable share only
* `snapshot_id` / `SnapshotId`  (Nullable<Int64>): ID of the snapshot containing this bundle's contents.
* `user_id` / `UserId`  (Nullable<Int64>): Bundle creator user ID
* `username` / `Username`  (string): Bundle creator username
* `clickwrap_id` / `ClickwrapId`  (Nullable<Int64>): ID of the clickwrap to use with this bundle.
* `inbox_id` / `InboxId`  (Nullable<Int64>): ID of the associated inbox, if available.
* `watermark_attachment` / `WatermarkAttachment`  (Image): Preview watermark image applied to all bundle items.
* `watermark_value` / `WatermarkValue`  (object): Preview watermark settings applied to all bundle items. Uses the same keys as Behavior.value
* `has_inbox` / `HasInbox`  (bool): Does this bundle have an associated inbox?
* `dont_allow_folders_in_uploads` / `DontAllowFoldersInUploads`  (bool): Should folder uploads be prevented?
* `paths` / `Paths`  (string[]): A list of paths in this bundle.  For performance reasons, this is not provided when listing bundles.
* `bundlepaths` / `Bundlepaths`  (object[]): A list of bundlepaths in this bundle.  For performance reasons, this is not provided when listing bundles.
* `password` / `Password`  (string): Password for this bundle.
* `form_field_set_id` / `FormFieldSetId`  (Nullable<Int64>): Id of Form Field Set to use with this bundle
* `create_snapshot` / `CreateSnapshot`  (bool): If true, create a snapshot of this bundle's contents.
* `finalize_snapshot` / `FinalizeSnapshot`  (bool): If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
* `watermark_attachment_file` / `WatermarkAttachmentFile`  (System.Net.Http.ByteArrayContent): Preview watermark image applied to all bundle items.
* `watermark_attachment_delete` / `WatermarkAttachmentDelete`  (bool): If true, will delete the file stored in watermark_attachment


---

## List Bundles

```
Task<FilesList<Bundle>> Bundle.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at`, `code` or `note`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `code`, `note` or `created_at`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `code` and `note`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.


---

## Show Bundle

```
Task<Bundle> Bundle.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.


---

## Create Bundle

```
Task<Bundle> Bundle.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `paths` (string[]): Required - A list of paths to include in this bundle.
* `password` (string): Password for this bundle.
* `form_field_set_id` (Nullable<Int64>): Id of Form Field Set to use with this bundle
* `create_snapshot` (bool): If true, create a snapshot of this bundle's contents.
* `dont_separate_submissions_by_folder` (bool): Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
* `expires_at` (string): Bundle expiration date/time
* `finalize_snapshot` (bool): If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
* `max_uses` (Nullable<Int64>): Maximum number of times bundle can be accessed
* `description` (string): Public description
* `note` (string): Bundle internal note
* `code` (string): Bundle code.  This code forms the end part of the Public URL.
* `path_template` (string): Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
* `path_template_time_zone` (string): Timezone to use when rendering timestamps in path templates.
* `permissions` (string): Permissions that apply to Folders in this Share Link.
* `require_registration` (bool): Show a registration page that captures the downloader's name and email address?
* `clickwrap_id` (Nullable<Int64>): ID of the clickwrap to use with this bundle.
* `inbox_id` (Nullable<Int64>): ID of the associated inbox, if available.
* `require_share_recipient` (bool): Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
* `send_email_receipt_to_uploader` (bool): Send delivery receipt to the uploader. Note: For writable share only
* `skip_email` (bool): BundleRegistrations can be saved without providing email?
* `skip_name` (bool): BundleRegistrations can be saved without providing name?
* `skip_company` (bool): BundleRegistrations can be saved without providing company?
* `start_access_on_date` (string): Date when share will start to be accessible. If `nil` access granted right after create.
* `snapshot_id` (Nullable<Int64>): ID of the snapshot containing this bundle's contents.
* `watermark_attachment_file` (System.Net.Http.ByteArrayContent): Preview watermark image applied to all bundle items.


---

## Create an export CSV of Bundle resources

```
Task<Export> Bundle.CreateExport(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at`, `code` or `note`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `code`, `note` or `created_at`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `code` and `note`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.


---

## Send email(s) with a link to bundle

```
Task Bundle.Share(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
* `to` (string[]): A list of email addresses to share this bundle with. Required unless `recipients` is used.
* `note` (string): Note to include in email.
* `recipients` (object[]): A list of recipients to share this bundle with. Required unless `to` is used.


---

## Update Bundle

```
Task<Bundle> Bundle.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
* `paths` (string[]): A list of paths to include in this bundle.
* `password` (string): Password for this bundle.
* `form_field_set_id` (Nullable<Int64>): Id of Form Field Set to use with this bundle
* `clickwrap_id` (Nullable<Int64>): ID of the clickwrap to use with this bundle.
* `code` (string): Bundle code.  This code forms the end part of the Public URL.
* `create_snapshot` (bool): If true, create a snapshot of this bundle's contents.
* `description` (string): Public description
* `dont_separate_submissions_by_folder` (bool): Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
* `expires_at` (string): Bundle expiration date/time
* `finalize_snapshot` (bool): If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
* `inbox_id` (Nullable<Int64>): ID of the associated inbox, if available.
* `max_uses` (Nullable<Int64>): Maximum number of times bundle can be accessed
* `note` (string): Bundle internal note
* `path_template` (string): Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
* `path_template_time_zone` (string): Timezone to use when rendering timestamps in path templates.
* `permissions` (string): Permissions that apply to Folders in this Share Link.
* `require_registration` (bool): Show a registration page that captures the downloader's name and email address?
* `require_share_recipient` (bool): Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
* `send_email_receipt_to_uploader` (bool): Send delivery receipt to the uploader. Note: For writable share only
* `skip_company` (bool): BundleRegistrations can be saved without providing company?
* `start_access_on_date` (string): Date when share will start to be accessible. If `nil` access granted right after create.
* `skip_email` (bool): BundleRegistrations can be saved without providing email?
* `skip_name` (bool): BundleRegistrations can be saved without providing name?
* `watermark_attachment_delete` (bool): If true, will delete the file stored in watermark_attachment
* `watermark_attachment_file` (System.Net.Http.ByteArrayContent): Preview watermark image applied to all bundle items.


---

## Delete Bundle

```
Task Bundle.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.


---

## Send email(s) with a link to bundle

```
var Bundle = Bundle.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("to", ["johndoe@gmail.com"]);
parameters.Add("note", "Just a note.");
parameters.Add("recipients", [{"name":"John Doe","company":"Acme Ltd","recipient":"johndoe@gmail.com"}]);

Bundle.Share(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
* `to` (string[]): A list of email addresses to share this bundle with. Required unless `recipients` is used.
* `note` (string): Note to include in email.
* `recipients` (object[]): A list of recipients to share this bundle with. Required unless `to` is used.


---

## Update Bundle

```
var Bundle = Bundle.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("paths", ["file.txt"]);
parameters.Add("password", "Password");
parameters.Add("form_field_set_id", 1);
parameters.Add("clickwrap_id", 1);
parameters.Add("code", "abc123");
parameters.Add("create_snapshot", true);
parameters.Add("description", "The public description of the bundle.");
parameters.Add("dont_separate_submissions_by_folder", true);
parameters.Add("expires_at", "2000-01-01T01:00:00Z");
parameters.Add("finalize_snapshot", true);
parameters.Add("inbox_id", 1);
parameters.Add("max_uses", 1);
parameters.Add("note", "The internal note on the bundle.");
parameters.Add("path_template", "{{name}}_{{ip}}");
parameters.Add("path_template_time_zone", "Eastern Time (US & Canada)");
parameters.Add("permissions", "read");
parameters.Add("require_registration", true);
parameters.Add("require_share_recipient", true);
parameters.Add("send_email_receipt_to_uploader", true);
parameters.Add("skip_company", true);
parameters.Add("start_access_on_date", "2000-01-01T01:00:00Z");
parameters.Add("skip_email", true);
parameters.Add("skip_name", true);
parameters.Add("watermark_attachment_delete", true);

Bundle.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
* `paths` (string[]): A list of paths to include in this bundle.
* `password` (string): Password for this bundle.
* `form_field_set_id` (Nullable<Int64>): Id of Form Field Set to use with this bundle
* `clickwrap_id` (Nullable<Int64>): ID of the clickwrap to use with this bundle.
* `code` (string): Bundle code.  This code forms the end part of the Public URL.
* `create_snapshot` (bool): If true, create a snapshot of this bundle's contents.
* `description` (string): Public description
* `dont_separate_submissions_by_folder` (bool): Do not create subfolders for files uploaded to this share. Note: there are subtle security pitfalls with allowing anonymous uploads from multiple users to live in the same folder. We strongly discourage use of this option unless absolutely required.
* `expires_at` (string): Bundle expiration date/time
* `finalize_snapshot` (bool): If true, finalize the snapshot of this bundle's contents. Note that `create_snapshot` must also be true.
* `inbox_id` (Nullable<Int64>): ID of the associated inbox, if available.
* `max_uses` (Nullable<Int64>): Maximum number of times bundle can be accessed
* `note` (string): Bundle internal note
* `path_template` (string): Template for creating submission subfolders. Can use the uploader's name, email address, ip, company, `strftime` directives, and any custom form data.
* `path_template_time_zone` (string): Timezone to use when rendering timestamps in path templates.
* `permissions` (string): Permissions that apply to Folders in this Share Link.
* `require_registration` (bool): Show a registration page that captures the downloader's name and email address?
* `require_share_recipient` (bool): Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
* `send_email_receipt_to_uploader` (bool): Send delivery receipt to the uploader. Note: For writable share only
* `skip_company` (bool): BundleRegistrations can be saved without providing company?
* `start_access_on_date` (string): Date when share will start to be accessible. If `nil` access granted right after create.
* `skip_email` (bool): BundleRegistrations can be saved without providing email?
* `skip_name` (bool): BundleRegistrations can be saved without providing name?
* `watermark_attachment_delete` (bool): If true, will delete the file stored in watermark_attachment
* `watermark_attachment_file` (System.Net.Http.ByteArrayContent): Preview watermark image applied to all bundle items.


---

## Delete Bundle

```
var Bundle = Bundle.Find(1);

var parameters = new Dictionary<string, object>();


Bundle.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
