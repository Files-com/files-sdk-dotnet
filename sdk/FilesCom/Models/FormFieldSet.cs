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
    /// FormFieldSetEntity model
    /// </summary>
    public class FormFieldSet : IModel, IJsonOnDeserialized
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
        // The JSON, when handed out since then, of the arrays among them that callers can change in place, directly or
        // through a model in them. SaveAsync compares each with the array's JSON now, and sends the ones that differ.
        private readonly Dictionary<string, string> handedOut = new Dictionary<string, string>();
        private static readonly string[] ArrayAttributeNames = { "form_fields" };
        /// <summary>
        /// Makes a FormFieldSet with no attributes set and no options.
        /// </summary>
        public FormFieldSet() : this(null, null) { }

        /// <summary>
        /// Makes a FormFieldSet with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FormFieldSet(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("title"))
            {
                this.attributes.Add("title", null);
            }
            if (!this.attributes.ContainsKey("require_complete_metadata_capture"))
            {
                this.attributes.Add("require_complete_metadata_capture", false);
            }
            if (!this.attributes.ContainsKey("form_layout"))
            {
                this.attributes.Add("form_layout", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("form_fields"))
            {
                this.attributes.Add("form_fields", new FormField[0]);
            }
            if (!this.attributes.ContainsKey("skip_name"))
            {
                this.attributes.Add("skip_name", false);
            }
            if (!this.attributes.ContainsKey("skip_email"))
            {
                this.attributes.Add("skip_email", false);
            }
            if (!this.attributes.ContainsKey("skip_company"))
            {
                this.attributes.Add("skip_company", false);
            }
            if (!this.attributes.ContainsKey("in_use"))
            {
                this.attributes.Add("in_use", false);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            // The copy holds this object's own arrays, which the caller can change in place as through its properties.
            foreach (string name in ArrayAttributeNames)
            {
                HandedOut(name);
            }
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
            get { return new object[] { FormFields }; }
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
            handedOut.Clear();
        }


        /// <summary>
        /// Form field set id
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Title to be displayed
        /// </summary>
        [JsonPropertyName("title")]
        public string Title
        {
            get { return (string)attributes["title"]; }
            set { attributes["title"] = value; unsaved.Add("title"); }
        }

        /// <summary>
        /// Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("require_complete_metadata_capture")]
        public bool RequireCompleteMetadataCapture
        {
            get { return attributes["require_complete_metadata_capture"] == null ? false : (bool)attributes["require_complete_metadata_capture"]; }
            set { attributes["require_complete_metadata_capture"] = value; unsaved.Add("require_complete_metadata_capture"); }
        }

        /// <summary>
        /// Layout of the form
        /// </summary>
        [JsonPropertyName("form_layout")]
        public Nullable<Int64>[] FormLayout
        {
            get { return (Nullable<Int64>[])attributes["form_layout"]; }
            set { attributes["form_layout"] = value; unsaved.Add("form_layout"); }
        }

        /// <summary>
        /// Associated form field definitions; authenticated form field set responses include historical definitions, while form_layout identifies current fields
        /// </summary>
        [JsonPropertyName("form_fields")]
        public FormField[] FormFields
        {
            get { return (FormField[])HandedOut("form_fields"); }
            set { attributes["form_fields"] = value; unsaved.Add("form_fields"); }
        }

        /// <summary>
        /// Any associated InboxRegistrations or BundleRegistrations can be saved without providing name
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_name")]
        public bool SkipName
        {
            get { return attributes["skip_name"] == null ? false : (bool)attributes["skip_name"]; }
            set { attributes["skip_name"] = value; unsaved.Add("skip_name"); }
        }

        /// <summary>
        /// Any associated InboxRegistrations or BundleRegistrations can be saved without providing email
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_email")]
        public bool SkipEmail
        {
            get { return attributes["skip_email"] == null ? false : (bool)attributes["skip_email"]; }
            set { attributes["skip_email"] = value; unsaved.Add("skip_email"); }
        }

        /// <summary>
        /// Any associated InboxRegistrations or BundleRegistrations can be saved without providing company
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_company")]
        public bool SkipCompany
        {
            get { return attributes["skip_company"] == null ? false : (bool)attributes["skip_company"]; }
            set { attributes["skip_company"] = value; unsaved.Add("skip_company"); }
        }

        /// <summary>
        /// Form Field Set is in use by an active Inbox / Bundle / Inbox Registration / Bundle Registration
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("in_use")]
        public bool InUse
        {
            get { return attributes["in_use"] == null ? false : (bool)attributes["in_use"]; }
            set { attributes["in_use"] = value; unsaved.Add("in_use"); }
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

        /// <summary>
        /// Workspace ID
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Parameters:
        ///   title - string - Title to be displayed
        ///   require_complete_metadata_capture - boolean - Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
        ///   workspace_id - int64 - Workspace ID
        ///   skip_email - boolean - Skip validating form email
        ///   skip_name - boolean - Skip validating form name
        ///   skip_company - boolean - Skip validating company
        ///   form_fields - array(object)
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The FormFieldSet it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<FormFieldSet> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   title - string - Title to be displayed
        ///   require_complete_metadata_capture - boolean - Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
        ///   workspace_id - int64 - Workspace ID
        ///   skip_email - boolean - Skip validating form email
        ///   skip_name - boolean - Skip validating form name
        ///   skip_company - boolean - Skip validating company
        ///   form_fields - array(object)
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The FormFieldSet it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<FormFieldSet> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<FormFieldSet> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The FormFieldSet has no id, which Update needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("title", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: title is null, which cannot be sent" : "Bad parameter: title must be of type string", "title");
            }
            if (parameters.TryGetValue("require_complete_metadata_capture", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_complete_metadata_capture is null, which cannot be sent" : "Bad parameter: require_complete_metadata_capture must be of type bool", "require_complete_metadata_capture");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("skip_email", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_email is null, which cannot be sent" : "Bad parameter: skip_email must be of type bool", "skip_email");
            }
            if (parameters.TryGetValue("skip_name", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_name is null, which cannot be sent" : "Bad parameter: skip_name must be of type bool", "skip_name");
            }
            if (parameters.TryGetValue("skip_company", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_company is null, which cannot be sent" : "Bad parameter: skip_company must be of type bool", "skip_company");
            }
            if (parameters.TryGetValue("form_fields", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: form_fields is null, which cannot be sent" : "Bad parameter: form_fields must be of type object[]", "form_fields");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/form_field_sets/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            FormFieldSet updated = JsonUtil.DeserializeModelResponse<FormFieldSet>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<FormFieldSet>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /form_field_sets/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /form_field_sets/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>, but neither waits for the request nor reports its failure.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        [Obsolete("Destroy neither waits for the request nor reports its failure. Use DestroyAsync instead.")]
        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The FormFieldSet has no id, which Delete needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/form_field_sets/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "user_id", "title", "require_complete_metadata_capture", "workspace_id", "skip_email", "skip_name", "skip_company", "form_fields" };
        private static readonly string[] UpdateParameterNames = { "id", "title", "require_complete_metadata_capture", "workspace_id", "skip_email", "skip_name", "skip_company", "form_fields" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this FormFieldSet on Files.com, or updates it if it has an ID.
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
                await UpdateCore(AttributesToSave(UpdateParameterNames), cancellationToken);
                return;
            }
            FormFieldSet created = await FormFieldSet.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
                if (unsaved.Contains(name) || ChangedInPlace(name))
                {
                    toSave[name] = attributes[name];
                }
            }
            return toSave;
        }

        // An array attribute, for a property or getAttributes to hand out. Its JSON is kept the first time, as the caller
        // can change the array, or an array in a model in it, without setting anything.
        private object HandedOut(string name)
        {
            object value = attributes[name];
            if (value != null && !handedOut.ContainsKey(name))
            {
                handedOut[name] = JsonSerializer.Serialize(value, JsonUtil.Options);
            }
            return value;
        }

        // Whether an array handed out has been changed in place since, at any depth.
        private bool ChangedInPlace(string name)
        {
            return handedOut.TryGetValue(name, out string handedOutJson) && JsonSerializer.Serialize(attributes[name], JsonUtil.Options) != handedOutJson;
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
            handedOut.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<FormFieldSet> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<FormFieldSet> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<FormFieldSet> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }

            return new FilesList<FormFieldSet>(client, $"/form_field_sets", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Form Field Set ID.
        /// </summary>
        /// <param name="id">Form Field Set ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FormFieldSet> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Form Field Set ID.
        /// </summary>
        /// <param name="id">Form Field Set ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FormFieldSet> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<FormFieldSet> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/form_field_sets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FormFieldSet>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   title - string - Title to be displayed
        ///   require_complete_metadata_capture - boolean - Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
        ///   workspace_id - int64 - Workspace ID
        ///   skip_email - boolean - Skip validating form email
        ///   skip_name - boolean - Skip validating form name
        ///   skip_company - boolean - Skip validating company
        ///   form_fields - array(object)
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FormFieldSet> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<FormFieldSet> CreateCore(
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
            if (parameters.TryGetValue("title", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: title is null, which cannot be sent" : "Bad parameter: title must be of type string", "title");
            }
            if (parameters.TryGetValue("require_complete_metadata_capture", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_complete_metadata_capture is null, which cannot be sent" : "Bad parameter: require_complete_metadata_capture must be of type bool", "require_complete_metadata_capture");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("skip_email", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_email is null, which cannot be sent" : "Bad parameter: skip_email must be of type bool", "skip_email");
            }
            if (parameters.TryGetValue("skip_name", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_name is null, which cannot be sent" : "Bad parameter: skip_name must be of type bool", "skip_name");
            }
            if (parameters.TryGetValue("skip_company", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_company is null, which cannot be sent" : "Bad parameter: skip_company must be of type bool", "skip_company");
            }
            if (parameters.TryGetValue("form_fields", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: form_fields is null, which cannot be sent" : "Bad parameter: form_fields must be of type object[]", "form_fields");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/form_field_sets", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FormFieldSet>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   title - string - Title to be displayed
        ///   require_complete_metadata_capture - boolean - Reject answers over 1024 characters or more than 32 answered custom fields to ensure complete file metadata capture
        ///   workspace_id - int64 - Workspace ID
        ///   skip_email - boolean - Skip validating form email
        ///   skip_name - boolean - Skip validating form name
        ///   skip_company - boolean - Skip validating company
        ///   form_fields - array(object)
        /// </summary>
        /// <param name="id">Form Field Set ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<FormFieldSet> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<FormFieldSet> UpdateCore(
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
            if (parameters.TryGetValue("title", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: title is null, which cannot be sent" : "Bad parameter: title must be of type string", "title");
            }
            if (parameters.TryGetValue("require_complete_metadata_capture", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: require_complete_metadata_capture is null, which cannot be sent" : "Bad parameter: require_complete_metadata_capture must be of type bool", "require_complete_metadata_capture");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("skip_email", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_email is null, which cannot be sent" : "Bad parameter: skip_email must be of type bool", "skip_email");
            }
            if (parameters.TryGetValue("skip_name", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_name is null, which cannot be sent" : "Bad parameter: skip_name must be of type bool", "skip_name");
            }
            if (parameters.TryGetValue("skip_company", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: skip_company is null, which cannot be sent" : "Bad parameter: skip_company must be of type bool", "skip_company");
            }
            if (parameters.TryGetValue("form_fields", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: form_fields is null, which cannot be sent" : "Bad parameter: form_fields must be of type object[]", "form_fields");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/form_field_sets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<FormFieldSet>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /form_field_sets/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Form Field Set ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /form_field_sets/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Form Field Set ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(id, parameters, options);
        }

        internal static async Task DeleteCore(
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

            await FilesClient.SendRequestAndDiscard(context, $"/form_field_sets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}