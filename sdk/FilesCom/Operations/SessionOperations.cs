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
    /// Session operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Sessions"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="global::FilesCom.Models.Session"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class SessionOperations
    {
        private readonly FilesClient client;

        internal SessionOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a Session that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public global::FilesCom.Models.Session New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            global::FilesCom.Models.Session model = new global::FilesCom.Models.Session(attributes, null);
            ((IModel)model).SetContext(client, options, null);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   username - string - Username to sign in as
        ///   password - string - Password for sign in
        ///   otp - string - If this user has a 2FA device, provide its OTP or code here.
        ///   partial_session_id - string - Identifier for a partially-completed login
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.Session> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.Session.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Sends DELETE /sessions to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.Session.DeleteCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Sends DELETE /sessions to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.Session.DeleteCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}