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
    /// PartnerChannel operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.PartnerChannels"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="PartnerChannel"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class PartnerChannelOperations
    {
        private readonly FilesClient client;

        internal PartnerChannelOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a PartnerChannel that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public PartnerChannel New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            PartnerChannel model = new PartnerChannel(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `path` or `partner_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `partner_id` and `workspace_id`. Valid field combinations are `[ workspace_id, partner_id ]`.
        /// </summary>
        public FilesList<PartnerChannel> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return PartnerChannel.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `path` or `partner_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `partner_id` and `workspace_id`. Valid field combinations are `[ workspace_id, partner_id ]`.
        /// </summary>
        public FilesList<PartnerChannel> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return PartnerChannel.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner Channel ID.
        /// </summary>
        public Task<PartnerChannel> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return PartnerChannel.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner Channel ID.
        /// </summary>
        public Task<PartnerChannel> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return PartnerChannel.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   direction - string - Channel directions. `two_way` enables both directions, `to_partner` enables outgoing downloads, and `from_partner` enables incoming uploads.
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path - string - Optional route path for files uploaded by the Partner.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path - string - Optional route path for files delivered to the Partner.
        ///   partner_id (required) - int64 - ID of the Partner this Channel belongs to.
        ///   path (required) - string - Channel path relative to the Partner root folder.
        ///   workspace_id - int64 - ID of the Workspace associated with this Partner Channel.
        /// </summary>
        public Task<PartnerChannel> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return PartnerChannel.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   direction - string - Channel directions. `two_way` enables both directions, `to_partner` enables outgoing downloads, and `from_partner` enables incoming uploads.
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path - string - Optional route path for files uploaded by the Partner.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path - string - Optional route path for files delivered to the Partner.
        ///   path - string - Channel path relative to the Partner root folder.
        /// </summary>
        public Task<PartnerChannel> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return PartnerChannel.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return PartnerChannel.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return PartnerChannel.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}