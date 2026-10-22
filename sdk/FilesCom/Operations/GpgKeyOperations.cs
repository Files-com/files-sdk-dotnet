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
    /// GpgKey operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.GpgKeys"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="global::FilesCom.Models.GpgKey"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class GpgKeyOperations
    {
        private readonly FilesClient client;

        internal GpgKeyOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a GpgKey that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public global::FilesCom.Models.GpgKey New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            global::FilesCom.Models.GpgKey model = new global::FilesCom.Models.GpgKey(attributes, null);
            ((IModel)model).SetContext(client, options, null);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `expires_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `partner_id` or `expires_at`. Valid field combinations are `[ workspace_id, partner_id ]`, `[ workspace_id, expires_at ]`, `[ partner_id, expires_at ]` or `[ workspace_id, partner_id, expires_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `expires_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `expires_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `expires_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `expires_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList<global::FilesCom.Models.GpgKey> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return global::FilesCom.Models.GpgKey.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `expires_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `partner_id` or `expires_at`. Valid field combinations are `[ workspace_id, partner_id ]`, `[ workspace_id, expires_at ]`, `[ partner_id, expires_at ]` or `[ workspace_id, partner_id, expires_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `expires_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `expires_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `expires_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `expires_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList<global::FilesCom.Models.GpgKey> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return global::FilesCom.Models.GpgKey.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Gpg Key ID.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.GpgKey> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GpgKey.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Gpg Key ID.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.GpgKey> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GpgKey.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name (required) - string - GPG key name.
        ///   workspace_id - int64 - Workspace ID (0 for default workspace).
        ///   generate_expires_at - string - Expiration date of the key. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        ///   generate_keypair - boolean - If true, generate a new GPG key pair. Can not be used with `public_key`/`private_key`
        ///   generate_full_name - string - Full name of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        ///   generate_email - string - Email address of the key owner. Used for the generation of the key. Will be ignored if `generate_keypair` is false.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.GpgKey> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GpgKey.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   partner_id - int64 - Partner ID who owns this GPG Key, if applicable.
        ///   public_key - string - The GPG public key
        ///   private_key - string - The GPG private key
        ///   private_key_password - string - The GPG private key password
        ///   name - string - GPG key name.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.GpgKey> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GpgKey.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Sends DELETE /gpg_keys/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GpgKey.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Sends DELETE /gpg_keys/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Gpg Key ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.GpgKey.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}