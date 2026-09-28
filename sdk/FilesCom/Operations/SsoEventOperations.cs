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
    /// SsoEvent operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.SsoEvents"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="SsoEvent"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class SsoEventOperations
    {
        private readonly FilesClient client;

        internal SsoEventOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at`, `event_type`, `status` or `user_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `event_type`, `idp_uid`, `ip`, `provider`, `status`, `user_id` or `username`. Valid field combinations are `[ event_type, created_at ]`, `[ idp_uid, created_at ]`, `[ ip, created_at ]`, `[ provider, created_at ]`, `[ status, created_at ]`, `[ user_id, created_at ]`, `[ username, created_at ]`, `[ event_type, status ]`, `[ idp_uid, status ]`, `[ ip, status ]`, `[ provider, status ]`, `[ user_id, status ]`, `[ username, status ]`, `[ event_type, status, created_at ]`, `[ idp_uid, status, created_at ]`, `[ ip, status, created_at ]`, `[ provider, status, created_at ]`, `[ user_id, status, created_at ]` or `[ username, status, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `idp_uid`, `ip`, `provider` or `username`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        public FilesList<SsoEvent> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return SsoEvent.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at`, `event_type`, `status` or `user_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `event_type`, `idp_uid`, `ip`, `provider`, `status`, `user_id` or `username`. Valid field combinations are `[ event_type, created_at ]`, `[ idp_uid, created_at ]`, `[ ip, created_at ]`, `[ provider, created_at ]`, `[ status, created_at ]`, `[ user_id, created_at ]`, `[ username, created_at ]`, `[ event_type, status ]`, `[ idp_uid, status ]`, `[ ip, status ]`, `[ provider, status ]`, `[ user_id, status ]`, `[ username, status ]`, `[ event_type, status, created_at ]`, `[ idp_uid, status, created_at ]`, `[ ip, status, created_at ]`, `[ provider, status, created_at ]`, `[ user_id, status, created_at ]` or `[ username, status, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `idp_uid`, `ip`, `provider` or `username`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        public FilesList<SsoEvent> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return SsoEvent.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sso Event ID.
        /// </summary>
        public Task<SsoEvent> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return SsoEvent.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sso Event ID.
        /// </summary>
        public Task<SsoEvent> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return SsoEvent.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}