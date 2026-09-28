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
    /// ChildSiteManagementPolicy operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.ChildSiteManagementPolicies"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="ChildSiteManagementPolicy"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class ChildSiteManagementPolicyOperations
    {
        private readonly FilesClient client;

        internal ChildSiteManagementPolicyOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a ChildSiteManagementPolicy that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public ChildSiteManagementPolicy New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            ChildSiteManagementPolicy model = new ChildSiteManagementPolicy(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        public FilesList<ChildSiteManagementPolicy> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ChildSiteManagementPolicy.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        public FilesList<ChildSiteManagementPolicy> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ChildSiteManagementPolicy.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Child Site Management Policy ID.
        /// </summary>
        public Task<ChildSiteManagementPolicy> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ChildSiteManagementPolicy.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Child Site Management Policy ID.
        /// </summary>
        public Task<ChildSiteManagementPolicy> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ChildSiteManagementPolicy.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   value - object - Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
        ///   skip_child_site_ids - array(int64) - IDs of child sites excluded from this default policy.
        ///   child_site_ids - array(int64) - IDs of child sites explicitly assigned to this non-default policy.
        ///   default_policy - boolean - Whether this policy applies to child sites not explicitly assigned to another policy.
        ///   policy_type (required) - string - Type of policy.  Valid values: `settings`.
        ///   name - string - Name for this policy.
        ///   description - string - Description for this policy.
        /// </summary>
        public Task<ChildSiteManagementPolicy> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ChildSiteManagementPolicy.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   value - object - Policy configuration data. Attributes differ by policy type. For more information, refer to the Value Hash section of the developer documentation.
        ///   skip_child_site_ids - array(int64) - IDs of child sites excluded from this default policy.
        ///   child_site_ids - array(int64) - IDs of child sites explicitly assigned to this non-default policy.
        ///   default_policy - boolean - Whether this policy applies to child sites not explicitly assigned to another policy.
        ///   policy_type - string - Type of policy.  Valid values: `settings`.
        ///   name - string - Name for this policy.
        ///   description - string - Description for this policy.
        /// </summary>
        public Task<ChildSiteManagementPolicy> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return ChildSiteManagementPolicy.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return ChildSiteManagementPolicy.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return ChildSiteManagementPolicy.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}