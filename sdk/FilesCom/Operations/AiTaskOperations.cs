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
    /// AiTask operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.AiTasks"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="AiTask"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class AiTaskOperations
    {
        private readonly FilesClient client;

        internal AiTaskOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a AiTask that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public AiTask New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            AiTask model = new AiTask(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `id`, `disabled` or `updated_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled`, `trigger` or `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.
        /// </summary>
        public FilesList<AiTask> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return AiTask.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `id`, `disabled` or `updated_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled`, `trigger` or `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.
        /// </summary>
        public FilesList<AiTask> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return AiTask.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Ai Task ID.
        /// </summary>
        public Task<AiTask> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return AiTask.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Ai Task ID.
        /// </summary>
        public Task<AiTask> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return AiTask.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   description - string - AI Task description.
        ///   disabled - boolean - If true, this AI Task will not run.
        ///   holiday_region - string - Optional holiday region used by the AI Task schedule.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the AI Task.
        ///   name (required) - string - AI Task name.
        ///   path - string - Path scope used for action-triggered AI Tasks.
        ///   permission_set - string - Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
        ///   prompt (required) - string - Prompt sent when this AI Task is invoked.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the AI Task's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_time_zone - string - Time zone used by the AI Task schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the AI Task schedule.
        ///   source - string - Source glob used with `path` for action-triggered AI Tasks.
        ///   trigger - string - How this AI Task is triggered.
        ///   trigger_actions - array(string) - If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public Task<AiTask> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return AiTask.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Manually Run AI Task
        /// </summary>
        public Task ManualRunAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return AiTask.ManualRunCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   description - string - AI Task description.
        ///   disabled - boolean - If true, this AI Task will not run.
        ///   holiday_region - string - Optional holiday region used by the AI Task schedule.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the AI Task.
        ///   name - string - AI Task name.
        ///   path - string - Path scope used for action-triggered AI Tasks.
        ///   permission_set - string - Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
        ///   prompt - string - Prompt sent when this AI Task is invoked.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the AI Task's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_time_zone - string - Time zone used by the AI Task schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the AI Task schedule.
        ///   source - string - Source glob used with `path` for action-triggered AI Tasks.
        ///   trigger - string - How this AI Task is triggered.
        ///   trigger_actions - array(string) - If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public Task<AiTask> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return AiTask.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return AiTask.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return AiTask.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}