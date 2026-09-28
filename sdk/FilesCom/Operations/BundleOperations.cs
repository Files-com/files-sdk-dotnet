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
    /// Bundle operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Bundles"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="Bundle"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class BundleOperations
    {
        private readonly FilesClient client;

        internal BundleOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a Bundle that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public Bundle New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            Bundle model = new Bundle(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
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
        public FilesList<Bundle> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Bundle.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
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
        public FilesList<Bundle> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Bundle.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Bundle ID.
        ///   deleted - boolean - If true, show a deleted Share Link.
        /// </summary>
        public Task<Bundle> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Bundle.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Bundle ID.
        ///   deleted - boolean - If true, show a deleted Share Link.
        /// </summary>
        public Task<Bundle> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Bundle.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
        public Task<Bundle> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Bundle.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Send email(s) with a link to bundle
        ///
        /// Parameters:
        ///   to - array(string) - A list of email addresses to share this bundle with. Required unless `recipients` is used.
        ///   note - string - Note to include in email.
        ///   recipients - array(object) - A list of recipients to share this bundle with. Required unless `to` is used.
        /// </summary>
        public Task ShareAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Bundle.ShareCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
        public Task<Bundle> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Bundle.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// </summary>
        public Task DeleteAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Bundle.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// </summary>
        public Task DestroyAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Bundle.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}