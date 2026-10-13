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
    /// ActionNotificationExportEntity model
    /// </summary>
    public class ActionNotificationExport : IModel, IJsonOnDeserialized
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
        /// Makes a ActionNotificationExport with no attributes set and no options.
        /// </summary>
        public ActionNotificationExport() : this(null, null) { }

        /// <summary>
        /// Makes a ActionNotificationExport with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public ActionNotificationExport(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("export_version"))
            {
                this.attributes.Add("export_version", null);
            }
            if (!this.attributes.ContainsKey("start_at"))
            {
                this.attributes.Add("start_at", null);
            }
            if (!this.attributes.ContainsKey("end_at"))
            {
                this.attributes.Add("end_at", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("query_path"))
            {
                this.attributes.Add("query_path", null);
            }
            if (!this.attributes.ContainsKey("query_folder"))
            {
                this.attributes.Add("query_folder", null);
            }
            if (!this.attributes.ContainsKey("query_message"))
            {
                this.attributes.Add("query_message", null);
            }
            if (!this.attributes.ContainsKey("query_request_method"))
            {
                this.attributes.Add("query_request_method", null);
            }
            if (!this.attributes.ContainsKey("query_request_url"))
            {
                this.attributes.Add("query_request_url", null);
            }
            if (!this.attributes.ContainsKey("query_status"))
            {
                this.attributes.Add("query_status", null);
            }
            if (!this.attributes.ContainsKey("query_success"))
            {
                this.attributes.Add("query_success", false);
            }
            if (!this.attributes.ContainsKey("results_url"))
            {
                this.attributes.Add("results_url", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
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
            get { return new object[0]; }
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
        /// History Export ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Workspace whose logs are exported. Set to `0` for the default workspace. A null value means a site-wide export.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Version of the underlying records for the export.
        /// </summary>
        [JsonPropertyName("export_version")]
        public string ExportVersion
        {
            get { return (string)attributes["export_version"]; }
            set { attributes["export_version"] = value; unsaved.Add("export_version"); }
        }

        /// <summary>
        /// Start date/time of export range.
        /// </summary>
        [JsonPropertyName("start_at")]
        public Nullable<DateTime> StartAt
        {
            get { return (Nullable<DateTime>)attributes["start_at"]; }
            set { attributes["start_at"] = value; unsaved.Add("start_at"); }
        }

        /// <summary>
        /// End date/time of export range.
        /// </summary>
        [JsonPropertyName("end_at")]
        public Nullable<DateTime> EndAt
        {
            get { return (Nullable<DateTime>)attributes["end_at"]; }
            set { attributes["end_at"] = value; unsaved.Add("end_at"); }
        }

        /// <summary>
        /// Status of export.  Valid values: `building`, `ready`, or `failed`
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; unsaved.Add("status"); }
        }

        /// <summary>
        /// Return notifications that were triggered by actions on this specific path.
        /// </summary>
        [JsonPropertyName("query_path")]
        public string QueryPath
        {
            get { return (string)attributes["query_path"]; }
            set { attributes["query_path"] = value; unsaved.Add("query_path"); }
        }

        /// <summary>
        /// Return notifications that were triggered by actions in this folder.
        /// </summary>
        [JsonPropertyName("query_folder")]
        public string QueryFolder
        {
            get { return (string)attributes["query_folder"]; }
            set { attributes["query_folder"] = value; unsaved.Add("query_folder"); }
        }

        /// <summary>
        /// Error message associated with the request, if any.
        /// </summary>
        [JsonPropertyName("query_message")]
        public string QueryMessage
        {
            get { return (string)attributes["query_message"]; }
            set { attributes["query_message"] = value; unsaved.Add("query_message"); }
        }

        /// <summary>
        /// The HTTP request method used by the webhook.
        /// </summary>
        [JsonPropertyName("query_request_method")]
        public string QueryRequestMethod
        {
            get { return (string)attributes["query_request_method"]; }
            set { attributes["query_request_method"] = value; unsaved.Add("query_request_method"); }
        }

        /// <summary>
        /// The target webhook URL.
        /// </summary>
        [JsonPropertyName("query_request_url")]
        public string QueryRequestUrl
        {
            get { return (string)attributes["query_request_url"]; }
            set { attributes["query_request_url"] = value; unsaved.Add("query_request_url"); }
        }

        /// <summary>
        /// The HTTP status returned from the server in response to the webhook request.
        /// </summary>
        [JsonPropertyName("query_status")]
        public string QueryStatus
        {
            get { return (string)attributes["query_status"]; }
            set { attributes["query_status"] = value; unsaved.Add("query_status"); }
        }

        /// <summary>
        /// true if the webhook request succeeded (i.e. returned a 200 or 204 response status). false otherwise.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("query_success")]
        public bool QuerySuccess
        {
            get { return attributes["query_success"] == null ? false : (bool)attributes["query_success"]; }
            set { attributes["query_success"] = value; unsaved.Add("query_success"); }
        }

        /// <summary>
        /// If `status` is `ready`, this will be a URL where all the results can be downloaded at once as a CSV.
        /// </summary>
        [JsonPropertyName("results_url")]
        public string ResultsUrl
        {
            get { return (string)attributes["results_url"]; }
            set { attributes["results_url"] = value; unsaved.Add("results_url"); }
        }

        /// <summary>
        /// User ID.  Provide a value of `0` to operate the current session's user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; unsaved.Add("user_id"); }
        }


        private static readonly string[] CreateParameterNames = { "user_id", "workspace_id", "start_at", "end_at", "query_message", "query_request_method", "query_request_url", "query_status", "query_success", "query_path", "query_folder" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this ActionNotificationExport on Files.com.
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
                throw new NotImplementedException("The ActionNotificationExport object doesn't support updates.");
            }
            ActionNotificationExport created = await ActionNotificationExport.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
                    toSave[name] = ToParameter(name, attributes[name]);
                }
            }
            return toSave;
        }

        // The value SaveAsync sends for an attribute whose property type is not the one its parameter takes.
        private static object ToParameter(string name, object value)
        {
            switch (name)
            {
                case "start_at":
                case "end_at":
                    return ParameterValue.FromDateTime(value);
                default:
                    return value;
            }
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
        ///   id (required) - int64 - Action Notification Export ID.
        /// </summary>
        /// <param name="id">Action Notification Export ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ActionNotificationExport> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Action Notification Export ID.
        /// </summary>
        /// <param name="id">Action Notification Export ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ActionNotificationExport> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<ActionNotificationExport> FindCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/action_notification_exports/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ActionNotificationExport>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   workspace_id - int64 - Workspace whose logs are exported. Set to `0` for the default workspace. A null value means a site-wide export.
        ///   start_at - string - Start date/time of export range.
        ///   end_at - string - End date/time of export range.
        ///   query_message - string - Error message associated with the request, if any.
        ///   query_request_method - string - The HTTP request method used by the webhook.
        ///   query_request_url - string - The target webhook URL.
        ///   query_status - string - The HTTP status returned from the server in response to the webhook request.
        ///   query_success - boolean - true if the webhook request succeeded (i.e. returned a 200 or 204 response status). false otherwise.
        ///   query_path - string - Return notifications that were triggered by actions on this specific path.
        ///   query_folder - string - Return notifications that were triggered by actions in this folder.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ActionNotificationExport> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<ActionNotificationExport> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("start_at", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: start_at is null, which cannot be sent" : "Bad parameter: start_at must be of type string", "start_at");
            }
            if (parameters.TryGetValue("end_at", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: end_at is null, which cannot be sent" : "Bad parameter: end_at must be of type string", "end_at");
            }
            if (parameters.TryGetValue("query_message", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_message is null, which cannot be sent" : "Bad parameter: query_message must be of type string", "query_message");
            }
            if (parameters.TryGetValue("query_request_method", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_request_method is null, which cannot be sent" : "Bad parameter: query_request_method must be of type string", "query_request_method");
            }
            if (parameters.TryGetValue("query_request_url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_request_url is null, which cannot be sent" : "Bad parameter: query_request_url must be of type string", "query_request_url");
            }
            if (parameters.TryGetValue("query_status", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_status is null, which cannot be sent" : "Bad parameter: query_status must be of type string", "query_status");
            }
            if (parameters.TryGetValue("query_success", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_success is null, which cannot be sent" : "Bad parameter: query_success must be of type bool", "query_success");
            }
            if (parameters.TryGetValue("query_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_path is null, which cannot be sent" : "Bad parameter: query_path must be of type string", "query_path");
            }
            if (parameters.TryGetValue("query_folder", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_folder is null, which cannot be sent" : "Bad parameter: query_folder must be of type string", "query_folder");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/action_notification_exports", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ActionNotificationExport>(responseJson, context, options);
        }

    }
}