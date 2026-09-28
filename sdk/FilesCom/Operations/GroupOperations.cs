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
    /// Group operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Groups"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="Group"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class GroupOperations
    {
        private readonly FilesClient client;

        internal GroupOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a Group that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public Group New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            Group model = new Group(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `name` and `workspace_id`. Valid field combinations are `[ workspace_id, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
        ///   ids - string - Comma-separated list of group ids to include in results.
        ///   include_parent_site_groups - boolean - Include groups from the parent site.
        /// </summary>
        public FilesList<Group> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Group.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `name` and `workspace_id`. Valid field combinations are `[ workspace_id, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
        ///   ids - string - Comma-separated list of group ids to include in results.
        ///   include_parent_site_groups - boolean - Include groups from the parent site.
        /// </summary>
        public FilesList<Group> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Group.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Group ID.
        /// </summary>
        public Task<Group> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Group.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Group ID.
        /// </summary>
        public Task<Group> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Group.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   notes - string - Group notes.
        ///   user_ids - string - A list of user ids. If sent as a string, should be comma-delimited.
        ///   admin_ids - string - A list of group admin user ids. If sent as a string, should be comma-delimited.
        ///   ai_assistant_personality_id - int64 - AI Assistant Personality ID assigned to this Group, if any. Users in the Group inherit it unless a direct per-user or Partner assignment overrides it.
        ///   ftp_permission - boolean - If true, users in this group can use FTP to login.  This will override a false value of `ftp_permission` on the user level.
        ///   sftp_permission - boolean - If true, users in this group can use SFTP to login.  This will override a false value of `sftp_permission` on the user level.
        ///   dav_permission - boolean - If true, users in this group can use WebDAV to login.  This will override a false value of `dav_permission` on the user level.
        ///   restapi_permission - boolean - If true, users in this group can use the REST API to login.  This will override a false value of `restapi_permission` on the user level.
        ///   s3_compatible_endpoint_permission - boolean - If true, users in this group can access the S3-compatible endpoint. This will override a false value of `s3_compatible_endpoint_permission` on the user level. Defaults to false.
        ///   desktop_configuration_profile_id - int64 - Desktop Configuration Profile ID assigned to this Group, if any. Users in the Group inherit it unless a direct per-user assignment overrides it.
        ///   integration_centric_profile_id - int64 - Integration Centric Profile ID assigned to this Group, if any. Users in the Group inherit it unless a direct per-user assignment overrides it.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   name (required) - string - Group name.
        ///   workspace_id - int64 - Workspace ID
        /// </summary>
        public Task<Group> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Group.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   notes - string - Group notes.
        ///   user_ids - string - A list of user ids. If sent as a string, should be comma-delimited.
        ///   admin_ids - string - A list of group admin user ids. If sent as a string, should be comma-delimited.
        ///   ai_assistant_personality_id - int64 - AI Assistant Personality ID assigned to this Group, if any. Users in the Group inherit it unless a direct per-user or Partner assignment overrides it.
        ///   ftp_permission - boolean - If true, users in this group can use FTP to login.  This will override a false value of `ftp_permission` on the user level.
        ///   sftp_permission - boolean - If true, users in this group can use SFTP to login.  This will override a false value of `sftp_permission` on the user level.
        ///   dav_permission - boolean - If true, users in this group can use WebDAV to login.  This will override a false value of `dav_permission` on the user level.
        ///   restapi_permission - boolean - If true, users in this group can use the REST API to login.  This will override a false value of `restapi_permission` on the user level.
        ///   s3_compatible_endpoint_permission - boolean - If true, users in this group can access the S3-compatible endpoint. This will override a false value of `s3_compatible_endpoint_permission` on the user level. Defaults to false.
        ///   desktop_configuration_profile_id - int64 - Desktop Configuration Profile ID assigned to this Group, if any. Users in the Group inherit it unless a direct per-user assignment overrides it.
        ///   integration_centric_profile_id - int64 - Integration Centric Profile ID assigned to this Group, if any. Users in the Group inherit it unless a direct per-user assignment overrides it.
        ///   allowed_ips - string - A list of allowed IPs if applicable.  Newline delimited
        ///   name - string - Group name.
        /// </summary>
        public Task<Group> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Group.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return Group.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
            return Group.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}