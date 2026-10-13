using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// WebhookTestEntity model
    /// </summary>
    public class WebhookTest : IModel, IJsonOnDeserialized
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        // The names of the attributes set on this object, by its constructor or its properties, since it was made
        // or last came from Files.com. SaveAsync sends these, never the defaults of attributes nobody set.
        private readonly HashSet<string> unsaved = new HashSet<string>();
        /// <summary>
        /// Makes a WebhookTest with no attributes set and no options.
        /// </summary>
        public WebhookTest() : this(null, null) { }

        /// <summary>
        /// Makes a WebhookTest with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public WebhookTest(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("code"))
            {
                this.attributes.Add("code", null);
            }
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("data"))
            {
                this.attributes.Add("data", null);
            }
            if (!this.attributes.ContainsKey("success"))
            {
                this.attributes.Add("success", false);
            }
            if (!this.attributes.ContainsKey("url"))
            {
                this.attributes.Add("url", null);
            }
            if (!this.attributes.ContainsKey("method"))
            {
                this.attributes.Add("method", null);
            }
            if (!this.attributes.ContainsKey("encoding"))
            {
                this.attributes.Add("encoding", null);
            }
            if (!this.attributes.ContainsKey("headers"))
            {
                this.attributes.Add("headers", null);
            }
            if (!this.attributes.ContainsKey("body"))
            {
                this.attributes.Add("body", null);
            }
            if (!this.attributes.ContainsKey("raw_body"))
            {
                this.attributes.Add("raw_body", null);
            }
            if (!this.attributes.ContainsKey("file_as_body"))
            {
                this.attributes.Add("file_as_body", false);
            }
            if (!this.attributes.ContainsKey("file_form_field"))
            {
                this.attributes.Add("file_form_field", null);
            }
            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("use_dedicated_ips"))
            {
                this.attributes.Add("use_dedicated_ips", false);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.client = client;
            this.origin = origin;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => origin;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[] { Data }; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }

        // What is read from JSON, such as a response from Files.com, is not waiting to be saved.
        void IJsonOnDeserialized.OnDeserialized()
        {
            unsaved.Clear();
        }


        /// <summary>
        /// Status HTTP code
        /// </summary>
        [JsonPropertyName("code")]
        public Nullable<Int64> Code
        {
            get { return (Nullable<Int64>)attributes["code"]; }
            set { attributes["code"] = value; unsaved.Add("code"); }
        }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            set { attributes["message"] = value; unsaved.Add("message"); }
        }

        /// <summary>
        /// Status message
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; unsaved.Add("status"); }
        }

        /// <summary>
        /// Additional data
        /// </summary>
        [JsonPropertyName("data")]
        public Auto Data
        {
            get { return (Auto)attributes["data"]; }
            set { attributes["data"] = value; unsaved.Add("data"); }
        }

        /// <summary>
        /// The success status of the webhook test
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("success")]
        public bool Success
        {
            get { return attributes["success"] == null ? false : (bool)attributes["success"]; }
            set { attributes["success"] = value; unsaved.Add("success"); }
        }

        /// <summary>
        /// URL for testing the webhook.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url
        {
            get { return (string)attributes["url"]; }
            set { attributes["url"] = value; unsaved.Add("url"); }
        }

        /// <summary>
        /// HTTP method(GET or POST).
        /// </summary>
        [JsonPropertyName("method")]
        public string Method
        {
            get { return (string)attributes["method"]; }
            set { attributes["method"] = value; unsaved.Add("method"); }
        }

        /// <summary>
        /// HTTP encoding method.  Can be JSON, XML, or RAW (form data).
        /// </summary>
        [JsonPropertyName("encoding")]
        public string Encoding
        {
            get { return (string)attributes["encoding"]; }
            set { attributes["encoding"] = value; unsaved.Add("encoding"); }
        }

        /// <summary>
        /// Additional request headers.
        /// </summary>
        [JsonPropertyName("headers")]
        public object Headers
        {
            get { return (object)attributes["headers"]; }
            set { attributes["headers"] = value; unsaved.Add("headers"); }
        }

        /// <summary>
        /// Additional body parameters.
        /// </summary>
        [JsonPropertyName("body")]
        public object Body
        {
            get { return (object)attributes["body"]; }
            set { attributes["body"] = value; unsaved.Add("body"); }
        }

        /// <summary>
        /// raw body text
        /// </summary>
        [JsonPropertyName("raw_body")]
        public string RawBody
        {
            get { return (string)attributes["raw_body"]; }
            set { attributes["raw_body"] = value; unsaved.Add("raw_body"); }
        }

        /// <summary>
        /// Send the file data as the request body?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("file_as_body")]
        public bool FileAsBody
        {
            get { return attributes["file_as_body"] == null ? false : (bool)attributes["file_as_body"]; }
            set { attributes["file_as_body"] = value; unsaved.Add("file_as_body"); }
        }

        /// <summary>
        /// Send the file data as a named parameter in the request POST body
        /// </summary>
        [JsonPropertyName("file_form_field")]
        public string FileFormField
        {
            get { return (string)attributes["file_form_field"]; }
            set { attributes["file_form_field"] = value; unsaved.Add("file_form_field"); }
        }

        /// <summary>
        /// action for test body
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            set { attributes["action"] = value; unsaved.Add("action"); }
        }

        /// <summary>
        /// Use dedicated IPs for sending the webhook?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("use_dedicated_ips")]
        public bool UseDedicatedIps
        {
            get { return attributes["use_dedicated_ips"] == null ? false : (bool)attributes["use_dedicated_ips"]; }
            set { attributes["use_dedicated_ips"] = value; unsaved.Add("use_dedicated_ips"); }
        }


        private static readonly string[] CreateParameterNames = { "url", "method", "encoding", "headers", "body", "raw_body", "file_as_body", "file_form_field", "action", "use_dedicated_ips" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this WebhookTest on Files.com.
        /// It sends the attributes set on this object, in its constructor or with its properties, or changed in place,
        /// such as an element of an array, since it was made or last came from Files.com, of those that operation
        /// takes; it sends no other attributes, and no defaults. Each goes in the form its parameter takes, such as a
        /// DateTime as ISO 8601 text. When it succeeds, this object takes the attributes Files.com returns.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The WebhookTest object doesn't support updates.");
            }
            WebhookTest created = await WebhookTest.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
            Refresh(created.getAttributes());
            origin = ((IModel)created).Origin;
        }

        // The attributes set or changed on this object since it was made or last came from Files.com, of those named
        // in parameterNames: the parameters of the operation that saves it.
        private Dictionary<string, object> AttributesToSave(string[] parameterNames)
        {
            Dictionary<string, object> toSave = new Dictionary<string, object>();
            foreach (string name in parameterNames)
            {
                if (unsaved.Contains(name))
                {
                    toSave[name] = attributes[name];
                }
            }
            return toSave;
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
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
        public static Task<WebhookTest> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<WebhookTest> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("url", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: url", "url");
            }
            if (parameters.TryGetValue("url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: url is null, which cannot be sent" : "Bad parameter: url must be of type string", "url");
            }
            if (parameters.TryGetValue("method", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: method is null, which cannot be sent" : "Bad parameter: method must be of type string", "method");
            }
            if (parameters.TryGetValue("encoding", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: encoding is null, which cannot be sent" : "Bad parameter: encoding must be of type string", "encoding");
            }
            if (parameters.TryGetValue("headers", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: headers is null, which cannot be sent" : "Bad parameter: headers must be of type object", "headers");
            }
            if (parameters.TryGetValue("body", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: body is null, which cannot be sent" : "Bad parameter: body must be of type object", "body");
            }
            if (parameters.TryGetValue("raw_body", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: raw_body is null, which cannot be sent" : "Bad parameter: raw_body must be of type string", "raw_body");
            }
            if (parameters.TryGetValue("file_as_body", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_as_body is null, which cannot be sent" : "Bad parameter: file_as_body must be of type bool", "file_as_body");
            }
            if (parameters.TryGetValue("file_form_field", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_form_field is null, which cannot be sent" : "Bad parameter: file_form_field must be of type string", "file_form_field");
            }
            if (parameters.TryGetValue("action", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action is null, which cannot be sent" : "Bad parameter: action must be of type string", "action");
            }
            if (parameters.TryGetValue("use_dedicated_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: use_dedicated_ips is null, which cannot be sent" : "Bad parameter: use_dedicated_ips must be of type bool", "use_dedicated_ips");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/webhook_tests", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<WebhookTest>(responseJson, context, options);
        }

    }
}