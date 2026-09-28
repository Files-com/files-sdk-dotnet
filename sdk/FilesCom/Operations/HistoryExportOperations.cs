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
    /// HistoryExport operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.HistoryExports"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="HistoryExport"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class HistoryExportOperations
    {
        private readonly FilesClient client;

        internal HistoryExportOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a HistoryExport that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public HistoryExport New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            HistoryExport model = new HistoryExport(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - History Export ID.
        /// </summary>
        public Task<HistoryExport> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return HistoryExport.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - History Export ID.
        /// </summary>
        public Task<HistoryExport> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return HistoryExport.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   start_at - string - Start date/time of export range.
        ///   end_at - string - End date/time of export range.
        ///   query_action - string - Filter results by this this action type. Valid values: `create`, `read`, `update`, `destroy`, `move`, `login`, `failedlogin`, `copy`, `user_create`, `user_update`, `user_destroy`, `group_create`, `group_update`, `group_destroy`, `permission_create`, `permission_destroy`, `api_key_create`, `api_key_update`, `api_key_destroy`, `archived_delete`
        ///   query_interface - string - Filter results by this this interface type. Valid values: `web`, `ftp`, `robot`, `jsapi`, `webdesktopapi`, `sftp`, `dav`, `desktop`, `restapi`, `scim`, `office`, `mobile`, `as2`, `inbound_email`, `remote`, `inbound_s3`
        ///   query_user_id - string - Return results that are actions performed by the user indicated by this User ID
        ///   query_file_id - string - Return results that are file actions related to the file indicated by this File ID
        ///   query_parent_id - string - Return results that are file actions inside the parent folder specified by this folder ID
        ///   query_path - string - Return results that are file actions related to paths matching this pattern.
        ///   query_folder - string - Return results that are file actions related to files or folders inside folder paths matching this pattern.
        ///   query_src - string - Return results that are file moves originating from paths matching this pattern.
        ///   query_destination - string - Return results that are file moves with paths matching this pattern as destination.
        ///   query_ip - string - Filter results by this IP address.
        ///   query_username - string - Filter results by this username.
        ///   query_failure_type - string - If searching for Histories about login failures, this parameter restricts results to failures of this specific type.  Valid values: `expired_trial`, `account_overdue`, `locked_out`, `ip_mismatch`, `password_mismatch`, `site_mismatch`, `username_not_found`, `none`, `no_ftp_permission`, `no_web_permission`, `no_directory`, `errno_enoent`, `no_sftp_permission`, `no_dav_permission`, `no_restapi_permission`, `key_mismatch`, `region_mismatch`, `expired_access`, `desktop_ip_mismatch`, `desktop_api_key_not_used_quickly_enough`, `disabled`, `country_mismatch`, `insecure_ftp`, `insecure_cipher`, `rate_limited`, `no_s3_compatible_endpoint_permission`, `mobile_ip_mismatch`, `mobile_api_key_not_used_quickly_enough`, `desktop_app_disabled`, `mobile_app_disabled`
        ///   query_target_id - string - If searching for Histories about specific objects (such as Users, or API Keys), this parameter restricts results to objects that match this ID.
        ///   query_target_name - string - If searching for Histories about Users, Groups or other objects with names, this parameter restricts results to objects with this name/username.
        ///   query_target_permission - string - If searching for Histories about Permissions, this parameter restricts results to permissions of this level.
        ///   query_target_user_id - string - If searching for Histories about API keys, this parameter restricts results to API keys created by/for this user ID.
        ///   query_target_username - string - If searching for Histories about API keys, this parameter restricts results to API keys created by/for this username.
        ///   query_target_platform - string - If searching for Histories about API keys, this parameter restricts results to API keys associated with this platform.
        ///   query_target_permission_set - string - If searching for Histories about API keys, this parameter restricts results to API keys with this permission set.
        /// </summary>
        public Task<HistoryExport> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return HistoryExport.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}