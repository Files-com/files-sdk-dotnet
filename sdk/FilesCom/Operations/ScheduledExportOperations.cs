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
    /// ScheduledExport operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.ScheduledExports"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="ScheduledExport"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class ScheduledExportOperations
    {
        private readonly FilesClient client;

        internal ScheduledExportOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a ScheduledExport that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public ScheduledExport New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            ScheduledExport model = new ScheduledExport(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `export_type` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `export_type`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `export_type`.
        /// </summary>
        public FilesList<ScheduledExport> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ScheduledExport.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `export_type` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `export_type`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `export_type`.
        /// </summary>
        public FilesList<ScheduledExport> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ScheduledExport.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Scheduled Export ID.
        /// </summary>
        public Task<ScheduledExport> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ScheduledExport.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Scheduled Export ID.
        /// </summary>
        public Task<ScheduledExport> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ScheduledExport.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   name (required) - string - Name for this scheduled export.
        ///   export_type (required) - string - Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
        ///   export_options - object - Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
        ///   user_id - int64 - Site Admin user who receives the completed export e-mail.
        ///   disabled - boolean - If true, this scheduled export will not run.
        ///   trigger - string - Schedule trigger type: `daily` or `custom_schedule`.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the scheduled export.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the scheduled export's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the scheduled export schedule.
        ///   schedule_time_zone - string - Time zone used by the scheduled export schedule.
        ///   holiday_region - string - Optional holiday region used by the scheduled export schedule.
        /// </summary>
        public Task<ScheduledExport> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ScheduledExport.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this scheduled export.
        ///   export_type - string - Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
        ///   export_options - object - Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
        ///   user_id - int64 - Site Admin user who receives the completed export e-mail.
        ///   disabled - boolean - If true, this scheduled export will not run.
        ///   trigger - string - Schedule trigger type: `daily` or `custom_schedule`.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the scheduled export.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the scheduled export's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the scheduled export schedule.
        ///   schedule_time_zone - string - Time zone used by the scheduled export schedule.
        ///   holiday_region - string - Optional holiday region used by the scheduled export schedule.
        /// </summary>
        public Task<ScheduledExport> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ScheduledExport.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return ScheduledExport.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return ScheduledExport.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}