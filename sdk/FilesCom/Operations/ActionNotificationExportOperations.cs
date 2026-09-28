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
    /// ActionNotificationExport operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.ActionNotificationExports"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="ActionNotificationExport"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class ActionNotificationExportOperations
    {
        private readonly FilesClient client;

        internal ActionNotificationExportOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a ActionNotificationExport that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public ActionNotificationExport New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            ActionNotificationExport model = new ActionNotificationExport(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Action Notification Export ID.
        /// </summary>
        public Task<ActionNotificationExport> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ActionNotificationExport.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Action Notification Export ID.
        /// </summary>
        public Task<ActionNotificationExport> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ActionNotificationExport.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   workspace_id - int64 - Workspace whose logs are exported. Set to `0` for the default workspace. A null value means a site-wide export.
        ///   start_at - string - Start date/time of export range.
        ///   end_at - string - End date/time of export range.
        ///   query_message - string - Error message associated with the request, if any.
        ///   query_request_method - string - The HTTP request method used by the webhook.
        ///   query_request_url - string - The target webhook URL.
        ///   query_status - string - The HTTP status returned from the server in response to the webhook request.
        ///   query_success - boolean - true if the webhook request succeeded (i.e. returned a 200 or 204 response status). false otherwise.
        ///   query_path - string - Return notifications that were triggered by actions on this specific path.
        ///   query_folder - string - Return notifications that were triggered by actions in this folder.
        /// </summary>
        public Task<ActionNotificationExport> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ActionNotificationExport.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}