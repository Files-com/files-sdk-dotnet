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
    /// Expectation operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Expectations"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="Expectation"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class ExpectationOperations
    {
        private readonly FilesClient client;

        internal ExpectationOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a Expectation that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public Expectation New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            Expectation model = new Expectation(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.
        /// </summary>
        public FilesList<Expectation> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Expectation.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.
        /// </summary>
        public FilesList<Expectation> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Expectation.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation ID.
        /// </summary>
        public Task<Expectation> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Expectation.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation ID.
        /// </summary>
        public Task<Expectation> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Expectation.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Expectation name.
        ///   description - string - Expectation description.
        ///   path - string - Path scope for the expectation. Supports workspace-relative presentation.
        ///   source - string - Source glob used to select candidate files.
        ///   exclude_pattern - string - Optional source exclusion glob.
        ///   disabled - boolean - If true, the expectation is disabled.
        ///   trigger - string - How this expectation opens windows.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the expectation.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the Expectation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the Expectation schedule.
        ///   schedule_time_zone - string - Time zone used by the Expectation schedule.
        ///   holiday_region - string - Optional holiday region used by the Expectation schedule.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Versioned success criteria definition for the expectation, including optional Files Transform Script content validation in criteria v2.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public Task<Expectation> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Expectation.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Manually open an Expectation window
        /// </summary>
        public Task<ExpectationEvaluation> TriggerEvaluationAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Expectation.TriggerEvaluationCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Expectation name.
        ///   description - string - Expectation description.
        ///   path - string - Path scope for the expectation. Supports workspace-relative presentation.
        ///   source - string - Source glob used to select candidate files.
        ///   exclude_pattern - string - Optional source exclusion glob.
        ///   disabled - boolean - If true, the expectation is disabled.
        ///   trigger - string - How this expectation opens windows.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the expectation.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the Expectation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the Expectation schedule.
        ///   schedule_time_zone - string - Time zone used by the Expectation schedule.
        ///   holiday_region - string - Optional holiday region used by the Expectation schedule.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Versioned success criteria definition for the expectation, including optional Files Transform Script content validation in criteria v2.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public Task<Expectation> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Expectation.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return Expectation.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return Expectation.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}