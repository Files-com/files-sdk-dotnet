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
    /// RemoteMountBackend operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.RemoteMountBackends"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="RemoteMountBackend"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class RemoteMountBackendOperations
    {
        private readonly FilesClient client;

        internal RemoteMountBackendOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a RemoteMountBackend that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public RemoteMountBackend New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            RemoteMountBackend model = new RemoteMountBackend(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `remote_server_mount_id`.
        /// </summary>
        public FilesList<RemoteMountBackend> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return RemoteMountBackend.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `remote_server_mount_id`.
        /// </summary>
        public FilesList<RemoteMountBackend> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return RemoteMountBackend.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Mount Backend ID.
        /// </summary>
        public Task<RemoteMountBackend> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteMountBackend.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Mount Backend ID.
        /// </summary>
        public Task<RemoteMountBackend> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteMountBackend.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   enabled - boolean - True if this backend is enabled.
        ///   fall - int64 - Number of consecutive failures before considering the backend unhealthy.
        ///   health_check_enabled - boolean - True if health checks are enabled for this backend.
        ///   health_check_type - string - Type of health check to perform.
        ///   interval - int64 - Interval in seconds between health checks.
        ///   min_free_cpu - double - Minimum free CPU percentage required for this backend to be considered healthy.
        ///   min_free_mem - double - Minimum free memory percentage required for this backend to be considered healthy.
        ///   priority - int64 - Priority of this backend.
        ///   remote_path - string - Path on the remote server to treat as the root of this mount.
        ///   rise - int64 - Number of consecutive successes before considering the backend healthy.
        ///   canary_file_path (required) - string - Path to the canary file used for health checks.
        ///   remote_server_mount_id (required) - int64 - The mount ID of the Remote Server Mount that this backend is associated with.
        ///   remote_server_id (required) - int64 - The remote server that this backend is associated with.
        /// </summary>
        public Task<RemoteMountBackend> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteMountBackend.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Reset backend status to healthy
        /// </summary>
        public Task ResetStatusAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteMountBackend.ResetStatusCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   enabled - boolean - True if this backend is enabled.
        ///   fall - int64 - Number of consecutive failures before considering the backend unhealthy.
        ///   health_check_enabled - boolean - True if health checks are enabled for this backend.
        ///   health_check_type - string - Type of health check to perform.
        ///   interval - int64 - Interval in seconds between health checks.
        ///   min_free_cpu - double - Minimum free CPU percentage required for this backend to be considered healthy.
        ///   min_free_mem - double - Minimum free memory percentage required for this backend to be considered healthy.
        ///   priority - int64 - Priority of this backend.
        ///   remote_path - string - Path on the remote server to treat as the root of this mount.
        ///   rise - int64 - Number of consecutive successes before considering the backend healthy.
        ///   canary_file_path - string - Path to the canary file used for health checks.
        ///   remote_server_id - int64 - The remote server that this backend is associated with.
        /// </summary>
        public Task<RemoteMountBackend> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteMountBackend.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return RemoteMountBackend.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return RemoteMountBackend.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}