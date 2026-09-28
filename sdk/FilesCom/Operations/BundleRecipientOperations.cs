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
    /// BundleRecipient operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.BundleRecipients"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="BundleRecipient"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class BundleRecipientOperations
    {
        private readonly FilesClient client;

        internal BundleRecipientOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a BundleRecipient that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public BundleRecipient New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            BundleRecipient model = new BundleRecipient(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `has_registrations`.
        ///   bundle_id (required) - int64 - List recipients for the bundle with this ID.
        /// </summary>
        public FilesList<BundleRecipient> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return BundleRecipient.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `has_registrations`.
        ///   bundle_id (required) - int64 - List recipients for the bundle with this ID.
        /// </summary>
        public FilesList<BundleRecipient> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return BundleRecipient.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   bundle_id (required) - int64 - Bundle to share.
        ///   recipient (required) - string - Email addresses to share this bundle with.
        ///   name - string - Name of recipient.
        ///   company - string - Company of recipient.
        ///   note - string - Note to include in email.
        ///   share_after_create - boolean - Set to true to share the link with the recipient upon creation.
        /// </summary>
        public Task<BundleRecipient> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return BundleRecipient.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}