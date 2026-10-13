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
    /// WebhookTest operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.WebhookTests"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="global::FilesCom.Models.WebhookTest"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class WebhookTestOperations
    {
        private readonly FilesClient client;

        internal WebhookTestOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a WebhookTest that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public global::FilesCom.Models.WebhookTest New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            global::FilesCom.Models.WebhookTest model = new global::FilesCom.Models.WebhookTest(attributes, null);
            ((IModel)model).SetContext(client, options, null);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   url (required) - string - URL for testing the webhook.
        ///   method - string - HTTP method(GET or POST).
        ///   encoding - string - HTTP encoding method.  Can be JSON, XML, or RAW (form data).
        ///   headers - object - Additional request headers.
        ///   body - object - Additional body parameters.
        ///   raw_body - string - raw body text
        ///   file_as_body - boolean - Send the file data as the request body?
        ///   file_form_field - string - Send the file data as a named parameter in the request POST body
        ///   action - string - action for test body
        ///   use_dedicated_ips - boolean - Use dedicated IPs for sending the webhook?
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.WebhookTest> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.WebhookTest.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}