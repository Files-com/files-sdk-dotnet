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
    /// InboxRecipient operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.InboxRecipients"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="InboxRecipient"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class InboxRecipientOperations
    {
        private readonly FilesClient client;

        internal InboxRecipientOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a InboxRecipient that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public InboxRecipient New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            InboxRecipient model = new InboxRecipient(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `has_registrations`.
        ///   inbox_id (required) - int64 - List recipients for the inbox with this ID.
        /// </summary>
        public FilesList<InboxRecipient> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return InboxRecipient.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `has_registrations`.
        ///   inbox_id (required) - int64 - List recipients for the inbox with this ID.
        /// </summary>
        public FilesList<InboxRecipient> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return InboxRecipient.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   inbox_id (required) - int64 - Inbox to share.
        ///   recipient (required) - string - Email address to share this inbox with.
        ///   name - string - Name of recipient.
        ///   company - string - Company of recipient.
        ///   note - string - Note to include in email.
        ///   share_after_create - boolean - Set to true to share the link with the recipient upon creation.
        /// </summary>
        public Task<InboxRecipient> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return InboxRecipient.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}