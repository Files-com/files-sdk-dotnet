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
    /// SharedWorkflow operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.SharedWorkflows"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="global::FilesCom.Models.SharedWorkflow"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class SharedWorkflowOperations
    {
        private readonly FilesClient client;

        internal SharedWorkflowOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a SharedWorkflow that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public global::FilesCom.Models.SharedWorkflow New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            global::FilesCom.Models.SharedWorkflow model = new global::FilesCom.Models.SharedWorkflow(attributes, null);
            ((IModel)model).SetContext(client, options, null);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList<global::FilesCom.Models.SharedWorkflow> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return global::FilesCom.Models.SharedWorkflow.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList<global::FilesCom.Models.SharedWorkflow> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return global::FilesCom.Models.SharedWorkflow.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Shared Workflow ID.
        /// </summary>
        /// <param name="id">Shared Workflow ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.SharedWorkflow> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SharedWorkflow.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Shared Workflow ID.
        /// </summary>
        /// <param name="id">Shared Workflow ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.SharedWorkflow> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SharedWorkflow.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Shared Workflow ID.
        ///   run_id (required) - int64 - ID of the run returned by the Shared Workflow submission.
        /// </summary>
        /// <param name="id">Shared Workflow ID.</param>
        /// <param name="run_id">ID of the run returned by the Shared Workflow submission.</param>
        /// <param name="parameters">The operation takes no parameters besides id and run_id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.AutomationRun> FindRunAsync(
            Nullable<Int64> id, Nullable<Int64> run_id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SharedWorkflow.FindRunCore(new OperationContext(client), id, run_id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Run a Shared Workflow
        ///
        /// Parameters:
        ///   submission_revision (required) - string - Revision returned when opening the Shared Workflow.
        ///   submission_id (required) - string - Client-generated UUID used to retry the same submission without starting another run.
        ///   file_path (required) - string - Path of a file the recipient can read, including a file uploaded to a Snapshot created for this workflow.
        /// </summary>
        /// <param name="id">Shared Workflow ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.AutomationRun> RunWorkflowAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SharedWorkflow.RunWorkflowCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}