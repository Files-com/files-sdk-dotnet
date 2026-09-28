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
    /// Notification operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Notifications"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="Notification"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class NotificationOperations
    {
        private readonly FilesClient client;

        internal NotificationOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a Notification that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public Notification New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            Notification model = new Notification(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `path`, `user_id` or `group_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `user_id`, `workspace_id` or `group_id`. Valid field combinations are `[ workspace_id, path ]`, `[ workspace_id, user_id ]`, `[ workspace_id, group_id ]` or `[ workspace_id, user_id, path ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        ///   path - string - Show notifications for this Path.
        ///   include_ancestors - boolean - If `include_ancestors` is `true` and `path` is specified, include notifications for any parent paths. Ignored if `path` is not specified.
        ///   group_id - string
        /// </summary>
        public FilesList<Notification> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Notification.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `path`, `user_id` or `group_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `user_id`, `workspace_id` or `group_id`. Valid field combinations are `[ workspace_id, path ]`, `[ workspace_id, user_id ]`, `[ workspace_id, group_id ]` or `[ workspace_id, user_id, path ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        ///   path - string - Show notifications for this Path.
        ///   include_ancestors - boolean - If `include_ancestors` is `true` and `path` is specified, include notifications for any parent paths. Ignored if `path` is not specified.
        ///   group_id - string
        /// </summary>
        public FilesList<Notification> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Notification.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Notification ID.
        /// </summary>
        public Task<Notification> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Notification.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Notification ID.
        /// </summary>
        public Task<Notification> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Notification.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - The id of the user to notify. Provide `user_id`, `username` or `group_id`.
        ///   notify_on_copy - boolean - If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
        ///   notify_on_delete - boolean - Trigger on files deleted in this path?
        ///   notify_on_download - boolean - Trigger on files downloaded in this path?
        ///   notify_on_move - boolean - Trigger on files moved to this path?
        ///   notify_on_upload - boolean - Trigger on files created/uploaded/updated/changed in this path?
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
        ///   subject - string - Custom subject line to use for notification emails
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will only notify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        ///   group_id - int64 - The ID of the group to notify.  Provide `user_id`, `username` or `group_id`.
        ///   group_ids - string - Group IDs when the notification requires multiple groups. If sent as a string, it should be comma-delimited.
        ///   path - string - Path
        ///   username - string - The username of the user to notify.  Provide `user_id`, `username` or `group_id`.
        /// </summary>
        public Task<Notification> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Notification.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   notify_on_copy - boolean - If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
        ///   notify_on_delete - boolean - Trigger on files deleted in this path?
        ///   notify_on_download - boolean - Trigger on files downloaded in this path?
        ///   notify_on_move - boolean - Trigger on files moved to this path?
        ///   notify_on_upload - boolean - Trigger on files created/uploaded/updated/changed in this path?
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
        ///   subject - string - Custom subject line to use for notification emails
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will only notify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public Task<Notification> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Notification.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return Notification.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return Notification.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}