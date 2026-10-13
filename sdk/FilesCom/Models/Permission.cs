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
    /// PermissionEntity model
    /// </summary>
    public class Permission : IModel, IJsonOnDeserialized
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
        private static readonly string[] ArrayAttributeNames = { "group_ids" };
        /// <summary>
        /// Makes a Permission with no attributes set and no options.
        /// </summary>
        public Permission() : this(null, null) { }

        /// <summary>
        /// Makes a Permission with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Permission(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("group_id"))
            {
                this.attributes.Add("group_id", null);
            }
            if (!this.attributes.ContainsKey("group_name"))
            {
                this.attributes.Add("group_name", null);
            }
            if (!this.attributes.ContainsKey("group_ids"))
            {
                this.attributes.Add("group_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("group_names"))
            {
                this.attributes.Add("group_names", new string[0]);
            }
            if (!this.attributes.ContainsKey("partner_id"))
            {
                this.attributes.Add("partner_id", null);
            }
            if (!this.attributes.ContainsKey("partner_name"))
            {
                this.attributes.Add("partner_name", null);
            }
            if (!this.attributes.ContainsKey("permission"))
            {
                this.attributes.Add("permission", null);
            }
            if (!this.attributes.ContainsKey("recursive"))
            {
                this.attributes.Add("recursive", false);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
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
        /// Permission ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Folder path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; unsaved.Add("path"); }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; unsaved.Add("user_id"); }
        }

        /// <summary>
        /// Username (if applicable)
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; unsaved.Add("username"); }
        }

        /// <summary>
        /// Group ID
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>)attributes["group_id"]; }
            set { attributes["group_id"] = value; unsaved.Add("group_id"); }
        }

        /// <summary>
        /// Group name (if applicable)
        /// </summary>
        [JsonPropertyName("group_name")]
        public string GroupName
        {
            get { return (string)attributes["group_name"]; }
            set { attributes["group_name"] = value; unsaved.Add("group_name"); }
        }

        /// <summary>
        /// Group IDs when this Permission requires membership in every listed group.
        /// </summary>
        [JsonPropertyName("group_ids")]
        public Nullable<Int64>[] GroupIds
        {
            get { return (Nullable<Int64>[])HandedOut("group_ids"); }
            set { attributes["group_ids"] = value; unsaved.Add("group_ids"); }
        }

        /// <summary>
        /// Group names when this permission requires multiple groups
        /// </summary>
        [JsonPropertyName("group_names")]
        public string[] GroupNames
        {
            get { return (string[])attributes["group_names"]; }
            set { attributes["group_names"] = value; unsaved.Add("group_names"); }
        }

        /// <summary>
        /// Partner ID (if applicable)
        /// </summary>
        [JsonPropertyName("partner_id")]
        public Nullable<Int64> PartnerId
        {
            get { return (Nullable<Int64>)attributes["partner_id"]; }
            set { attributes["partner_id"] = value; unsaved.Add("partner_id"); }
        }

        /// <summary>
        /// Partner name (if applicable)
        /// </summary>
        [JsonPropertyName("partner_name")]
        public string PartnerName
        {
            get { return (string)attributes["partner_name"]; }
            set { attributes["partner_name"] = value; unsaved.Add("partner_name"); }
        }

        /// <summary>
        /// Permission type.  See the table referenced in the documentation for an explanation of each permission.
        /// </summary>
        [JsonPropertyName("permission")]
        public string PermissionType
        {
            get { return (string)attributes["permission"]; }
            set { attributes["permission"] = value; unsaved.Add("permission"); }
        }

        /// <summary>
        /// Whether this Permission applies to subfolders. Must be true for `admin` Permissions.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("recursive")]
        public bool Recursive
        {
            get { return attributes["recursive"] == null ? false : (bool)attributes["recursive"]; }
            set { attributes["recursive"] = value; unsaved.Add("recursive"); }
        }

        /// <summary>
        /// Site ID
        /// </summary>
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            set { attributes["site_id"] = value; unsaved.Add("site_id"); }
        }

        /// <summary>
        /// Sends DELETE /permissions/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /permissions/{id} to the Files.com API.
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
                throw new MissingParameterException("The Permission has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/permissions/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "path", "group_id", "group_ids", "permission", "recursive", "partner_id", "user_id", "username", "group_name", "site_id" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Permission on Files.com.
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
                throw new NotImplementedException("The Permission object doesn't support updates.");
            }
            Permission created = await Permission.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
                    toSave[name] = ToParameter(name, attributes[name]);
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

        // The value SaveAsync sends for an attribute whose property type is not the one its parameter takes.
        private static object ToParameter(string name, object value)
        {
            switch (name)
            {
                case "group_ids":
                    return ParameterValue.FromIds(value);
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
            handedOut.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `group_id`, `path`, `user_id`, `partner_id` or `id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `group_id`, `partner_id` or `user_id`. Valid field combinations are `[ group_id, path ]`, `[ partner_id, path ]`, `[ user_id, path ]`, `[ user_id, group_id ]`, `[ user_id, group_id, path ]`, `[ user_id, group_id, partner_id ]` or `[ user_id, group_id, partner_id, path ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        ///   path - string - Permission path.  If provided, will scope all permissions(including upward) to this path.
        ///   include_groups - boolean - If searching by user or group, also include user's permissions that are inherited from its groups?
        ///   group_id - string
        ///   partner_id - string
        ///   user_id - string
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Permission> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `group_id`, `path`, `user_id`, `partner_id` or `id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `group_id`, `partner_id` or `user_id`. Valid field combinations are `[ group_id, path ]`, `[ partner_id, path ]`, `[ user_id, path ]`, `[ user_id, group_id ]`, `[ user_id, group_id, path ]`, `[ user_id, group_id, partner_id ]` or `[ user_id, group_id, partner_id, path ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        ///   path - string - Permission path.  If provided, will scope all permissions(including upward) to this path.
        ///   include_groups - boolean - If searching by user or group, also include user's permissions that are inherited from its groups?
        ///   group_id - string
        ///   partner_id - string
        ///   user_id - string
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Permission> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Permission> ListCore(
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
            if (parameters.TryGetValue("filter_prefix", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_prefix is null, which cannot be sent" : "Bad parameter: filter_prefix must be of type object", "filter_prefix");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("include_groups", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: include_groups is null, which cannot be sent" : "Bad parameter: include_groups must be of type bool", "include_groups");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type string", "group_id");
            }
            if (parameters.TryGetValue("partner_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_id is null, which cannot be sent" : "Bad parameter: partner_id must be of type string", "partner_id");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type string", "user_id");
            }

            return new FilesList<Permission>(client, $"/permissions", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Folder path
        ///   group_id - int64 - Group ID. Provide `group_name` or `group_id`
        ///   group_ids - string - Group IDs when the Permission requires membership in every listed group. If sent as a string, it should be comma-delimited.
        ///   permission - string - Permission type.  Can be `admin`, `full`, `readonly`, `writeonly`, `list`, or `history`
        ///   recursive - boolean - Apply to subfolders recursively? Must be true for `admin` Permissions.
        ///   partner_id - int64 - Partner ID if this Permission belongs to a partner.
        ///   user_id - int64 - User ID.  Provide `username` or `user_id`
        ///   username - string - User username.  Provide `username` or `user_id`
        ///   group_name - string - Group name.  Provide `group_name` or `group_id`
        ///   site_id - int64 - Site ID. If not provided, will default to current site. Used when creating a permission for a child site.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Permission> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Permission> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: path", "path");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type Nullable<Int64>", "group_id");
            }
            if (parameters.TryGetValue("group_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_ids is null, which cannot be sent" : "Bad parameter: group_ids must be of type string", "group_ids");
            }
            if (parameters.TryGetValue("permission", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: permission is null, which cannot be sent" : "Bad parameter: permission must be of type string", "permission");
            }
            if (parameters.TryGetValue("recursive", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recursive is null, which cannot be sent" : "Bad parameter: recursive must be of type bool", "recursive");
            }
            if (parameters.TryGetValue("partner_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_id is null, which cannot be sent" : "Bad parameter: partner_id must be of type Nullable<Int64>", "partner_id");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("group_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_name is null, which cannot be sent" : "Bad parameter: group_name must be of type string", "group_name");
            }
            if (parameters.TryGetValue("site_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: site_id is null, which cannot be sent" : "Bad parameter: site_id must be of type Nullable<Int64>", "site_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/permissions", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Permission>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /permissions/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Permission ID.</param>
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
        /// Sends DELETE /permissions/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Permission ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/permissions/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}