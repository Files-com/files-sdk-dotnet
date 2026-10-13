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
    /// PartnerChannelTemplateEntity model
    /// </summary>
    public class PartnerChannelTemplate : IModel, IJsonOnDeserialized
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
        private static readonly string[] ArrayAttributeNames = { "to_partner_managed_folder_paths", "from_partner_managed_folder_paths" };
        /// <summary>
        /// Makes a PartnerChannelTemplate with no attributes set and no options.
        /// </summary>
        public PartnerChannelTemplate() : this(null, null) { }

        /// <summary>
        /// Makes a PartnerChannelTemplate with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public PartnerChannelTemplate(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("direction"))
            {
                this.attributes.Add("direction", null);
            }
            if (!this.attributes.ContainsKey("use_channel_root"))
            {
                this.attributes.Add("use_channel_root", false);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("to_partner_folder_name"))
            {
                this.attributes.Add("to_partner_folder_name", null);
            }
            if (!this.attributes.ContainsKey("from_partner_folder_name"))
            {
                this.attributes.Add("from_partner_folder_name", null);
            }
            if (!this.attributes.ContainsKey("from_partner_route_path_pattern"))
            {
                this.attributes.Add("from_partner_route_path_pattern", null);
            }
            if (!this.attributes.ContainsKey("to_partner_route_path_pattern"))
            {
                this.attributes.Add("to_partner_route_path_pattern", null);
            }
            if (!this.attributes.ContainsKey("to_partner_managed_folder_paths"))
            {
                this.attributes.Add("to_partner_managed_folder_paths", new string[0]);
            }
            if (!this.attributes.ContainsKey("from_partner_managed_folder_paths"))
            {
                this.attributes.Add("from_partner_managed_folder_paths", new string[0]);
            }
            if (!this.attributes.ContainsKey("effective_to_partner_folder_name"))
            {
                this.attributes.Add("effective_to_partner_folder_name", null);
            }
            if (!this.attributes.ContainsKey("effective_from_partner_folder_name"))
            {
                this.attributes.Add("effective_from_partner_folder_name", null);
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
            handedOut.Clear();
        }


        /// <summary>
        /// The unique ID of the Partner Channel Template.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// ID of the Workspace associated with this Partner Channel Template.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Channel directions. `two_way` enables both directions, `to_partner` enables outgoing downloads, and `from_partner` enables incoming uploads.
        /// </summary>
        [JsonPropertyName("direction")]
        public string Direction
        {
            get { return (string)attributes["direction"]; }
            set { attributes["direction"] = value; unsaved.Add("direction"); }
        }

        /// <summary>
        /// Use the Channel folder directly for a one-way exchange. Defaults to false. Cannot be changed after creation. Folder name overrides must be blank when enabled, and the Template must remain one-way.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("use_channel_root")]
        public bool UseChannelRoot
        {
            get { return attributes["use_channel_root"] == null ? false : (bool)attributes["use_channel_root"]; }
            set { attributes["use_channel_root"] = value; unsaved.Add("use_channel_root"); }
        }

        /// <summary>
        /// The name of the Partner Channel Template.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Channel path relative to the Partner root folder. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; unsaved.Add("path"); }
        }

        /// <summary>
        /// Optional Channel-level to-Partner folder name override.
        /// </summary>
        [JsonPropertyName("to_partner_folder_name")]
        public string ToPartnerFolderName
        {
            get { return (string)attributes["to_partner_folder_name"]; }
            set { attributes["to_partner_folder_name"] = value; unsaved.Add("to_partner_folder_name"); }
        }

        /// <summary>
        /// Optional Channel-level from-Partner folder name override.
        /// </summary>
        [JsonPropertyName("from_partner_folder_name")]
        public string FromPartnerFolderName
        {
            get { return (string)attributes["from_partner_folder_name"]; }
            set { attributes["from_partner_folder_name"] = value; unsaved.Add("from_partner_folder_name"); }
        }

        /// <summary>
        /// Optional route path pattern for files uploaded by the Partner. Supports {{partner_name}}.
        /// </summary>
        [JsonPropertyName("from_partner_route_path_pattern")]
        public string FromPartnerRoutePathPattern
        {
            get { return (string)attributes["from_partner_route_path_pattern"]; }
            set { attributes["from_partner_route_path_pattern"] = value; unsaved.Add("from_partner_route_path_pattern"); }
        }

        /// <summary>
        /// Optional route path pattern for files delivered to the Partner. Supports {{partner_name}}.
        /// </summary>
        [JsonPropertyName("to_partner_route_path_pattern")]
        public string ToPartnerRoutePathPattern
        {
            get { return (string)attributes["to_partner_route_path_pattern"]; }
            set { attributes["to_partner_route_path_pattern"] = value; unsaved.Add("to_partner_route_path_pattern"); }
        }

        /// <summary>
        /// Managed folder paths inside the to-Partner folder.
        /// </summary>
        [JsonPropertyName("to_partner_managed_folder_paths")]
        public string[] ToPartnerManagedFolderPaths
        {
            get { return (string[])HandedOut("to_partner_managed_folder_paths"); }
            set { attributes["to_partner_managed_folder_paths"] = value; unsaved.Add("to_partner_managed_folder_paths"); }
        }

        /// <summary>
        /// Managed folder paths inside the from-Partner folder.
        /// </summary>
        [JsonPropertyName("from_partner_managed_folder_paths")]
        public string[] FromPartnerManagedFolderPaths
        {
            get { return (string[])HandedOut("from_partner_managed_folder_paths"); }
            set { attributes["from_partner_managed_folder_paths"] = value; unsaved.Add("from_partner_managed_folder_paths"); }
        }

        /// <summary>
        /// Resolved to-Partner subfolder name. Null when the direction is disabled or uses the Channel folder directly.
        /// </summary>
        [JsonPropertyName("effective_to_partner_folder_name")]
        public string EffectiveToPartnerFolderName
        {
            get { return (string)attributes["effective_to_partner_folder_name"]; }
            set { attributes["effective_to_partner_folder_name"] = value; unsaved.Add("effective_to_partner_folder_name"); }
        }

        /// <summary>
        /// Resolved from-Partner subfolder name. Null when the direction is disabled or uses the Channel folder directly.
        /// </summary>
        [JsonPropertyName("effective_from_partner_folder_name")]
        public string EffectiveFromPartnerFolderName
        {
            get { return (string)attributes["effective_from_partner_folder_name"]; }
            set { attributes["effective_from_partner_folder_name"] = value; unsaved.Add("effective_from_partner_folder_name"); }
        }

        /// <summary>
        /// Parameters:
        ///   direction - string - Channel directions. `two_way` enables both directions, `to_partner` enables outgoing downloads, and `from_partner` enables incoming uploads.
        ///   use_channel_root - boolean - Use the Channel folder directly for a one-way exchange. Defaults to false. Cannot be changed after creation. Folder name overrides must be blank when enabled, and the Template must remain one-way.
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path_pattern - string - Optional route path pattern for files uploaded by the Partner. Supports {{partner_name}}.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path_pattern - string - Optional route path pattern for files delivered to the Partner. Supports {{partner_name}}.
        ///   name - string - The name of the Partner Channel Template.
        ///   path - string - Channel path relative to the Partner root folder.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The PartnerChannelTemplate it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<PartnerChannelTemplate> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   direction - string - Channel directions. `two_way` enables both directions, `to_partner` enables outgoing downloads, and `from_partner` enables incoming uploads.
        ///   use_channel_root - boolean - Use the Channel folder directly for a one-way exchange. Defaults to false. Cannot be changed after creation. Folder name overrides must be blank when enabled, and the Template must remain one-way.
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path_pattern - string - Optional route path pattern for files uploaded by the Partner. Supports {{partner_name}}.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path_pattern - string - Optional route path pattern for files delivered to the Partner. Supports {{partner_name}}.
        ///   name - string - The name of the Partner Channel Template.
        ///   path - string - Channel path relative to the Partner root folder.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The PartnerChannelTemplate it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<PartnerChannelTemplate> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<PartnerChannelTemplate> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The PartnerChannelTemplate has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("direction", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: direction is null, which cannot be sent" : "Bad parameter: direction must be of type string", "direction");
            }
            if (parameters.TryGetValue("use_channel_root", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: use_channel_root is null, which cannot be sent" : "Bad parameter: use_channel_root must be of type bool", "use_channel_root");
            }
            if (parameters.TryGetValue("from_partner_folder_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_folder_name is null, which cannot be sent" : "Bad parameter: from_partner_folder_name must be of type string", "from_partner_folder_name");
            }
            if (parameters.TryGetValue("from_partner_managed_folder_paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_managed_folder_paths is null, which cannot be sent" : "Bad parameter: from_partner_managed_folder_paths must be of type string[]", "from_partner_managed_folder_paths");
            }
            if (parameters.TryGetValue("from_partner_route_path_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_route_path_pattern is null, which cannot be sent" : "Bad parameter: from_partner_route_path_pattern must be of type string", "from_partner_route_path_pattern");
            }
            if (parameters.TryGetValue("to_partner_folder_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_folder_name is null, which cannot be sent" : "Bad parameter: to_partner_folder_name must be of type string", "to_partner_folder_name");
            }
            if (parameters.TryGetValue("to_partner_managed_folder_paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_managed_folder_paths is null, which cannot be sent" : "Bad parameter: to_partner_managed_folder_paths must be of type string[]", "to_partner_managed_folder_paths");
            }
            if (parameters.TryGetValue("to_partner_route_path_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_route_path_pattern is null, which cannot be sent" : "Bad parameter: to_partner_route_path_pattern must be of type string", "to_partner_route_path_pattern");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/partner_channel_templates/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            PartnerChannelTemplate updated = JsonUtil.DeserializeModelResponse<PartnerChannelTemplate>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<PartnerChannelTemplate>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /partner_channel_templates/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /partner_channel_templates/{id} to the Files.com API.
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
                throw new MissingParameterException("The PartnerChannelTemplate has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/partner_channel_templates/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "direction", "use_channel_root", "from_partner_folder_name", "from_partner_managed_folder_paths", "from_partner_route_path_pattern", "to_partner_folder_name", "to_partner_managed_folder_paths", "to_partner_route_path_pattern", "name", "path", "workspace_id" };
        private static readonly string[] UpdateParameterNames = { "id", "direction", "use_channel_root", "from_partner_folder_name", "from_partner_managed_folder_paths", "from_partner_route_path_pattern", "to_partner_folder_name", "to_partner_managed_folder_paths", "to_partner_route_path_pattern", "name", "path" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this PartnerChannelTemplate on Files.com, or updates it if it has an ID.
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
            PartnerChannelTemplate created = await PartnerChannelTemplate.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<PartnerChannelTemplate> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<PartnerChannelTemplate> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<PartnerChannelTemplate> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }

            return new FilesList<PartnerChannelTemplate>(client, $"/partner_channel_templates", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner Channel Template ID.
        /// </summary>
        /// <param name="id">Partner Channel Template ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<PartnerChannelTemplate> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner Channel Template ID.
        /// </summary>
        /// <param name="id">Partner Channel Template ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<PartnerChannelTemplate> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<PartnerChannelTemplate> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/partner_channel_templates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<PartnerChannelTemplate>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   direction - string - Channel directions. `two_way` enables both directions, `to_partner` enables outgoing downloads, and `from_partner` enables incoming uploads.
        ///   use_channel_root - boolean - Use the Channel folder directly for a one-way exchange. Defaults to false. Cannot be changed after creation. Folder name overrides must be blank when enabled, and the Template must remain one-way.
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path_pattern - string - Optional route path pattern for files uploaded by the Partner. Supports {{partner_name}}.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path_pattern - string - Optional route path pattern for files delivered to the Partner. Supports {{partner_name}}.
        ///   name (required) - string - The name of the Partner Channel Template.
        ///   path (required) - string - Channel path relative to the Partner root folder.
        ///   workspace_id - int64 - ID of the Workspace associated with this Partner Channel Template.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<PartnerChannelTemplate> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<PartnerChannelTemplate> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("name", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: name", "name");
            }
            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("direction", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: direction is null, which cannot be sent" : "Bad parameter: direction must be of type string", "direction");
            }
            if (parameters.TryGetValue("use_channel_root", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: use_channel_root is null, which cannot be sent" : "Bad parameter: use_channel_root must be of type bool", "use_channel_root");
            }
            if (parameters.TryGetValue("from_partner_folder_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_folder_name is null, which cannot be sent" : "Bad parameter: from_partner_folder_name must be of type string", "from_partner_folder_name");
            }
            if (parameters.TryGetValue("from_partner_managed_folder_paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_managed_folder_paths is null, which cannot be sent" : "Bad parameter: from_partner_managed_folder_paths must be of type string[]", "from_partner_managed_folder_paths");
            }
            if (parameters.TryGetValue("from_partner_route_path_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_route_path_pattern is null, which cannot be sent" : "Bad parameter: from_partner_route_path_pattern must be of type string", "from_partner_route_path_pattern");
            }
            if (parameters.TryGetValue("to_partner_folder_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_folder_name is null, which cannot be sent" : "Bad parameter: to_partner_folder_name must be of type string", "to_partner_folder_name");
            }
            if (parameters.TryGetValue("to_partner_managed_folder_paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_managed_folder_paths is null, which cannot be sent" : "Bad parameter: to_partner_managed_folder_paths must be of type string[]", "to_partner_managed_folder_paths");
            }
            if (parameters.TryGetValue("to_partner_route_path_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_route_path_pattern is null, which cannot be sent" : "Bad parameter: to_partner_route_path_pattern must be of type string", "to_partner_route_path_pattern");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/partner_channel_templates", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<PartnerChannelTemplate>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   direction - string - Channel directions. `two_way` enables both directions, `to_partner` enables outgoing downloads, and `from_partner` enables incoming uploads.
        ///   use_channel_root - boolean - Use the Channel folder directly for a one-way exchange. Defaults to false. Cannot be changed after creation. Folder name overrides must be blank when enabled, and the Template must remain one-way.
        ///   from_partner_folder_name - string - Optional Channel-level from-Partner folder name override.
        ///   from_partner_managed_folder_paths - array(string) - Managed folder paths inside the from-Partner folder.
        ///   from_partner_route_path_pattern - string - Optional route path pattern for files uploaded by the Partner. Supports {{partner_name}}.
        ///   to_partner_folder_name - string - Optional Channel-level to-Partner folder name override.
        ///   to_partner_managed_folder_paths - array(string) - Managed folder paths inside the to-Partner folder.
        ///   to_partner_route_path_pattern - string - Optional route path pattern for files delivered to the Partner. Supports {{partner_name}}.
        ///   name - string - The name of the Partner Channel Template.
        ///   path - string - Channel path relative to the Partner root folder.
        /// </summary>
        /// <param name="id">Partner Channel Template ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<PartnerChannelTemplate> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<PartnerChannelTemplate> UpdateCore(
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
            if (parameters.TryGetValue("direction", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: direction is null, which cannot be sent" : "Bad parameter: direction must be of type string", "direction");
            }
            if (parameters.TryGetValue("use_channel_root", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: use_channel_root is null, which cannot be sent" : "Bad parameter: use_channel_root must be of type bool", "use_channel_root");
            }
            if (parameters.TryGetValue("from_partner_folder_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_folder_name is null, which cannot be sent" : "Bad parameter: from_partner_folder_name must be of type string", "from_partner_folder_name");
            }
            if (parameters.TryGetValue("from_partner_managed_folder_paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_managed_folder_paths is null, which cannot be sent" : "Bad parameter: from_partner_managed_folder_paths must be of type string[]", "from_partner_managed_folder_paths");
            }
            if (parameters.TryGetValue("from_partner_route_path_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: from_partner_route_path_pattern is null, which cannot be sent" : "Bad parameter: from_partner_route_path_pattern must be of type string", "from_partner_route_path_pattern");
            }
            if (parameters.TryGetValue("to_partner_folder_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_folder_name is null, which cannot be sent" : "Bad parameter: to_partner_folder_name must be of type string", "to_partner_folder_name");
            }
            if (parameters.TryGetValue("to_partner_managed_folder_paths", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_managed_folder_paths is null, which cannot be sent" : "Bad parameter: to_partner_managed_folder_paths must be of type string[]", "to_partner_managed_folder_paths");
            }
            if (parameters.TryGetValue("to_partner_route_path_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: to_partner_route_path_pattern is null, which cannot be sent" : "Bad parameter: to_partner_route_path_pattern must be of type string", "to_partner_route_path_pattern");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/partner_channel_templates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<PartnerChannelTemplate>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /partner_channel_templates/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Partner Channel Template ID.</param>
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
        /// Sends DELETE /partner_channel_templates/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Partner Channel Template ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/partner_channel_templates/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}