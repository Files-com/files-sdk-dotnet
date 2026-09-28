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
    /// Sync operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Syncs"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="Sync"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class SyncOperations
    {
        private readonly FilesClient client;

        internal SyncOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a Sync that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public Sync New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            Sync model = new Sync(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `disabled`, `src_remote_server_id` or `dest_remote_server_id`. Valid field combinations are `[ workspace_id, disabled ]`, `[ workspace_id, src_remote_server_id ]`, `[ workspace_id, dest_remote_server_id ]`, `[ disabled, src_remote_server_id ]`, `[ disabled, dest_remote_server_id ]`, `[ workspace_id, disabled, src_remote_server_id ]` or `[ workspace_id, disabled, dest_remote_server_id ]`.
        /// </summary>
        public FilesList<Sync> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Sync.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `disabled`, `src_remote_server_id` or `dest_remote_server_id`. Valid field combinations are `[ workspace_id, disabled ]`, `[ workspace_id, src_remote_server_id ]`, `[ workspace_id, dest_remote_server_id ]`, `[ disabled, src_remote_server_id ]`, `[ disabled, dest_remote_server_id ]`, `[ workspace_id, disabled, src_remote_server_id ]` or `[ workspace_id, disabled, dest_remote_server_id ]`.
        /// </summary>
        public FilesList<Sync> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Sync.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sync ID.
        /// </summary>
        public Task<Sync> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Sync.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sync ID.
        /// </summary>
        public Task<Sync> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Sync.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   description - string - Description for this sync job
        ///   dest_path - string - Absolute destination path for the sync
        ///   dest_remote_server_id - int64 - Remote server ID for the destination (if remote)
        ///   disabled - boolean - Is this sync disabled?
        ///   exclude_patterns - array(string) - Array of glob patterns to exclude
        ///   holiday_region - string - Skip the sync if there is a formal, observed holiday for this region.
        ///   include_patterns - array(string) - Array of glob patterns to include
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   name - string - Name for this sync job
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the sync's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_time_zone - string - Time zone for the schedule. If not set, times are interpreted as UTC.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format. For `custom_schedule`, run at these times on specified days of week. For `daily`, run at these times on the scheduled interval date.
        ///   src_path - string - Absolute source path for the sync
        ///   src_remote_server_id - int64 - Remote server ID for the source (if remote)
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   always_write_trigger_file - boolean - If true, the trigger file will be sent at the end of a successful sync even when no files were transferred.
        ///   workspace_id - int64 - Workspace ID this sync belongs to
        /// </summary>
        public Task<Sync> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Sync.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Dry Run Sync
        /// </summary>
        public Task DryRunAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Sync.DryRunCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Manually Run Sync
        /// </summary>
        public Task ManualRunAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Sync.ManualRunCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   description - string - Description for this sync job
        ///   dest_path - string - Absolute destination path for the sync
        ///   dest_remote_server_id - int64 - Remote server ID for the destination (if remote)
        ///   disabled - boolean - Is this sync disabled?
        ///   exclude_patterns - array(string) - Array of glob patterns to exclude
        ///   holiday_region - string - Skip the sync if there is a formal, observed holiday for this region.
        ///   include_patterns - array(string) - Array of glob patterns to include
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   name - string - Name for this sync job
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the sync's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_time_zone - string - Time zone for the schedule. If not set, times are interpreted as UTC.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format. For `custom_schedule`, run at these times on specified days of week. For `daily`, run at these times on the scheduled interval date.
        ///   src_path - string - Absolute source path for the sync
        ///   src_remote_server_id - int64 - Remote server ID for the source (if remote)
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   always_write_trigger_file - boolean - If true, the trigger file will be sent at the end of a successful sync even when no files were transferred.
        /// </summary>
        public Task<Sync> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Sync.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return Sync.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return Sync.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}