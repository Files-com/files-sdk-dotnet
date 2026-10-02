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
    /// UserLifecycleRule operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.UserLifecycleRules"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="UserLifecycleRule"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class UserLifecycleRuleOperations
    {
        private readonly FilesClient client;

        internal UserLifecycleRuleOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a UserLifecycleRule that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public UserLifecycleRule New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            UserLifecycleRule model = new UserLifecycleRule(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id` and `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        public FilesList<UserLifecycleRule> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UserLifecycleRule.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id` and `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        public FilesList<UserLifecycleRule> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UserLifecycleRule.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - User Lifecycle Rule ID.
        /// </summary>
        public Task<UserLifecycleRule> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return UserLifecycleRule.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - User Lifecycle Rule ID.
        /// </summary>
        public Task<UserLifecycleRule> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return UserLifecycleRule.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   action - string - Action to take on inactive users (disable or delete)
        ///   apply_to_all_workspaces - boolean - If true, a Default Workspace rule also applies to users in all Custom Workspaces. Can only be enabled when `workspace_id` is `0`.
        ///   authentication_method - string - User authentication method for which the rule will apply. Use `all_non_sso` to target every non-SSO authentication method with one rule.
        ///   group_ids - array(int64) - Array of Group IDs to which the rule applies. If empty or not set, the rule applies to all users.
        ///   inactivity_days - int64 - Number of days of inactivity before the rule applies
        ///   include_site_admins - boolean - If true, the rule includes Site Administrators, who always belong to the Default Workspace. Can only be enabled when `workspace_id` is `0`.
        ///   include_folder_admins - boolean - If true, the rule will apply to folder admins.
        ///   name - string - User Lifecycle Rule name
        ///   notify_users - boolean - If true, users will be emailed before the rule disables or deletes them.
        ///   partner_tag - string - If provided, only users belonging to Partners with this tag at the Partner level will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.
        ///   user_state - string - State of the users to apply the rule to (inactive or disabled)
        ///   user_tag - string - If provided, only users with this tag will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.
        ///   workspace_id - int64 - Workspace whose users the rule applies to. `0` means the Default Workspace. A Custom Workspace rule applies only to users who belong to that Workspace, regardless of access granted to other users.
        /// </summary>
        public Task<UserLifecycleRule> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return UserLifecycleRule.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   action - string - Action to take on inactive users (disable or delete)
        ///   apply_to_all_workspaces - boolean - If true, a Default Workspace rule also applies to users in all Custom Workspaces. Can only be enabled when `workspace_id` is `0`.
        ///   authentication_method - string - User authentication method for which the rule will apply. Use `all_non_sso` to target every non-SSO authentication method with one rule.
        ///   group_ids - array(int64) - Array of Group IDs to which the rule applies. If empty or not set, the rule applies to all users.
        ///   inactivity_days - int64 - Number of days of inactivity before the rule applies
        ///   include_site_admins - boolean - If true, the rule includes Site Administrators, who always belong to the Default Workspace. Can only be enabled when `workspace_id` is `0`.
        ///   include_folder_admins - boolean - If true, the rule will apply to folder admins.
        ///   name - string - User Lifecycle Rule name
        ///   notify_users - boolean - If true, users will be emailed before the rule disables or deletes them.
        ///   partner_tag - string - If provided, only users belonging to Partners with this tag at the Partner level will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.
        ///   user_state - string - State of the users to apply the rule to (inactive or disabled)
        ///   user_tag - string - If provided, only users with this tag will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.
        ///   workspace_id - int64 - Workspace whose users the rule applies to. `0` means the Default Workspace. A Custom Workspace rule applies only to users who belong to that Workspace, regardless of access granted to other users.
        /// </summary>
        public Task<UserLifecycleRule> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return UserLifecycleRule.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return UserLifecycleRule.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return UserLifecycleRule.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}