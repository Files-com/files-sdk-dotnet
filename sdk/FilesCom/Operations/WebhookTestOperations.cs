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
    /// WebhookTest operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.WebhookTests"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="WebhookTest"/> method of the same name. Methods that send a request
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
        public WebhookTest New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            WebhookTest model = new WebhookTest(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
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
        public Task<WebhookTest> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return WebhookTest.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}