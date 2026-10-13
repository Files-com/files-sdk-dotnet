using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Operations
{
    // Imported inside the namespace so that model names, such as Lock, take precedence over those of the namespaces
    // above, such as System.Threading.Lock. A type of this namespace still takes precedence over a model of the same
    // name (a model UserOperations beside the operations class of User), so code here names models in their own
    // namespace (model_type, qualified: true).
    using FilesCom.Models;

    /// <summary>
    /// GroupUser operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.GroupUsers"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="global::FilesCom.Models.GroupUser"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class GroupUserOperations
    {
        private readonly FilesClient client;

        internal GroupUserOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a GroupUser that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public global::FilesCom.Models.GroupUser New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            global::FilesCom.Models.GroupUser model = new global::FilesCom.Models.GroupUser(attributes, null);
            ((IModel)model).SetContext(client, options, null);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   group_id - int64 - Group ID. If provided, returns memberships of this group. Requires a Site Administrator, a Read-only Administrator, a Workspace Administrator for the group's workspace, or a Group Administrator of this group.
        ///   user_id - int64 - User ID.  If provided, will return group_users of this user.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList<global::FilesCom.Models.GroupUser> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return global::FilesCom.Models.GroupUser.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   group_id - int64 - Group ID. If provided, returns memberships of this group. Requires a Site Administrator, a Read-only Administrator, a Workspace Administrator for the group's workspace, or a Group Administrator of this group.
        ///   user_id - int64 - User ID.  If provided, will return group_users of this user.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList<global::FilesCom.Models.GroupUser> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return global::FilesCom.Models.GroupUser.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   group_id (required) - int64 - Group ID to add user to.
        ///   user_id (required) - int64 - User ID to add to group.
        ///   admin - boolean - Is the user a group administrator?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.GroupUser> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GroupUser.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   group_id (required) - int64 - Group ID to add user to.
        ///   user_id (required) - int64 - User ID to add to group.
        ///   admin - boolean - Is the user a group administrator?
        /// </summary>
        /// <param name="id">Group User ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.GroupUser> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GroupUser.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   group_id (required) - int64 - Group ID from which to remove user.
        ///   user_id (required) - int64 - User ID to remove from group.
        /// </summary>
        /// <param name="id">Group User ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GroupUser.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   group_id (required) - int64 - Group ID from which to remove user.
        ///   user_id (required) - int64 - User ID to remove from group.
        /// </summary>
        /// <param name="id">Group User ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GroupUser.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}