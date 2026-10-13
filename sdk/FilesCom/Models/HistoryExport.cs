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
    /// HistoryExportEntity model
    /// </summary>
    public class HistoryExport : IModel, IJsonOnDeserialized
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
        /// Makes a HistoryExport with no attributes set and no options.
        /// </summary>
        public HistoryExport() : this(null, null) { }

        /// <summary>
        /// Makes a HistoryExport with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public HistoryExport(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("history_version"))
            {
                this.attributes.Add("history_version", null);
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
            if (!this.attributes.ContainsKey("query_action"))
            {
                this.attributes.Add("query_action", null);
            }
            if (!this.attributes.ContainsKey("query_interface"))
            {
                this.attributes.Add("query_interface", null);
            }
            if (!this.attributes.ContainsKey("query_user_id"))
            {
                this.attributes.Add("query_user_id", null);
            }
            if (!this.attributes.ContainsKey("query_file_id"))
            {
                this.attributes.Add("query_file_id", null);
            }
            if (!this.attributes.ContainsKey("query_parent_id"))
            {
                this.attributes.Add("query_parent_id", null);
            }
            if (!this.attributes.ContainsKey("query_path"))
            {
                this.attributes.Add("query_path", null);
            }
            if (!this.attributes.ContainsKey("query_folder"))
            {
                this.attributes.Add("query_folder", null);
            }
            if (!this.attributes.ContainsKey("query_src"))
            {
                this.attributes.Add("query_src", null);
            }
            if (!this.attributes.ContainsKey("query_destination"))
            {
                this.attributes.Add("query_destination", null);
            }
            if (!this.attributes.ContainsKey("query_ip"))
            {
                this.attributes.Add("query_ip", null);
            }
            if (!this.attributes.ContainsKey("query_username"))
            {
                this.attributes.Add("query_username", null);
            }
            if (!this.attributes.ContainsKey("query_failure_type"))
            {
                this.attributes.Add("query_failure_type", null);
            }
            if (!this.attributes.ContainsKey("query_target_id"))
            {
                this.attributes.Add("query_target_id", null);
            }
            if (!this.attributes.ContainsKey("query_target_name"))
            {
                this.attributes.Add("query_target_name", null);
            }
            if (!this.attributes.ContainsKey("query_target_permission"))
            {
                this.attributes.Add("query_target_permission", null);
            }
            if (!this.attributes.ContainsKey("query_target_user_id"))
            {
                this.attributes.Add("query_target_user_id", null);
            }
            if (!this.attributes.ContainsKey("query_target_username"))
            {
                this.attributes.Add("query_target_username", null);
            }
            if (!this.attributes.ContainsKey("query_target_platform"))
            {
                this.attributes.Add("query_target_platform", null);
            }
            if (!this.attributes.ContainsKey("query_target_permission_set"))
            {
                this.attributes.Add("query_target_permission_set", null);
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
        /// Workspace of the export. 0 represents the default workspace. A null value means a site-wide export.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
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
        /// Version of the history for the export.
        /// </summary>
        [JsonPropertyName("history_version")]
        public string HistoryVersion
        {
            get { return (string)attributes["history_version"]; }
            set { attributes["history_version"] = value; unsaved.Add("history_version"); }
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
        /// Status of export.  Will be: `building`, `ready`, or `failed`
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; unsaved.Add("status"); }
        }

        /// <summary>
        /// Filter results by this this action type. Valid values: `create`, `read`, `update`, `destroy`, `move`, `login`, `failedlogin`, `copy`, `user_create`, `user_update`, `user_destroy`, `group_create`, `group_update`, `group_destroy`, `permission_create`, `permission_destroy`, `api_key_create`, `api_key_update`, `api_key_destroy`, `archived_delete`
        /// </summary>
        [JsonPropertyName("query_action")]
        public string QueryAction
        {
            get { return (string)attributes["query_action"]; }
            set { attributes["query_action"] = value; unsaved.Add("query_action"); }
        }

        /// <summary>
        /// Filter results by this this interface type. Valid values: `web`, `ftp`, `robot`, `jsapi`, `webdesktopapi`, `sftp`, `dav`, `desktop`, `restapi`, `scim`, `office`, `mobile`, `as2`, `inbound_email`, `remote`, `inbound_s3`
        /// </summary>
        [JsonPropertyName("query_interface")]
        public string QueryInterface
        {
            get { return (string)attributes["query_interface"]; }
            set { attributes["query_interface"] = value; unsaved.Add("query_interface"); }
        }

        /// <summary>
        /// Return results that are actions performed by the user indicated by this User ID
        /// </summary>
        [JsonPropertyName("query_user_id")]
        public string QueryUserId
        {
            get { return (string)attributes["query_user_id"]; }
            set { attributes["query_user_id"] = value; unsaved.Add("query_user_id"); }
        }

        /// <summary>
        /// Return results that are file actions related to the file indicated by this File ID
        /// </summary>
        [JsonPropertyName("query_file_id")]
        public string QueryFileId
        {
            get { return (string)attributes["query_file_id"]; }
            set { attributes["query_file_id"] = value; unsaved.Add("query_file_id"); }
        }

        /// <summary>
        /// Return results that are file actions inside the parent folder specified by this folder ID
        /// </summary>
        [JsonPropertyName("query_parent_id")]
        public string QueryParentId
        {
            get { return (string)attributes["query_parent_id"]; }
            set { attributes["query_parent_id"] = value; unsaved.Add("query_parent_id"); }
        }

        /// <summary>
        /// Return results that are file actions related to paths matching this pattern.
        /// </summary>
        [JsonPropertyName("query_path")]
        public string QueryPath
        {
            get { return (string)attributes["query_path"]; }
            set { attributes["query_path"] = value; unsaved.Add("query_path"); }
        }

        /// <summary>
        /// Return results that are file actions related to files or folders inside folder paths matching this pattern.
        /// </summary>
        [JsonPropertyName("query_folder")]
        public string QueryFolder
        {
            get { return (string)attributes["query_folder"]; }
            set { attributes["query_folder"] = value; unsaved.Add("query_folder"); }
        }

        /// <summary>
        /// Return results that are file moves originating from paths matching this pattern.
        /// </summary>
        [JsonPropertyName("query_src")]
        public string QuerySrc
        {
            get { return (string)attributes["query_src"]; }
            set { attributes["query_src"] = value; unsaved.Add("query_src"); }
        }

        /// <summary>
        /// Return results that are file moves with paths matching this pattern as destination.
        /// </summary>
        [JsonPropertyName("query_destination")]
        public string QueryDestination
        {
            get { return (string)attributes["query_destination"]; }
            set { attributes["query_destination"] = value; unsaved.Add("query_destination"); }
        }

        /// <summary>
        /// Filter results by this IP address.
        /// </summary>
        [JsonPropertyName("query_ip")]
        public string QueryIp
        {
            get { return (string)attributes["query_ip"]; }
            set { attributes["query_ip"] = value; unsaved.Add("query_ip"); }
        }

        /// <summary>
        /// Filter results by this username.
        /// </summary>
        [JsonPropertyName("query_username")]
        public string QueryUsername
        {
            get { return (string)attributes["query_username"]; }
            set { attributes["query_username"] = value; unsaved.Add("query_username"); }
        }

        /// <summary>
        /// If searching for Histories about login failures, this parameter restricts results to failures of this specific type.  Valid values: `expired_trial`, `account_overdue`, `locked_out`, `ip_mismatch`, `password_mismatch`, `site_mismatch`, `username_not_found`, `none`, `no_ftp_permission`, `no_web_permission`, `no_directory`, `errno_enoent`, `no_sftp_permission`, `no_dav_permission`, `no_restapi_permission`, `key_mismatch`, `region_mismatch`, `expired_access`, `desktop_ip_mismatch`, `desktop_api_key_not_used_quickly_enough`, `disabled`, `country_mismatch`, `insecure_ftp`, `insecure_cipher`, `rate_limited`, `no_s3_compatible_endpoint_permission`, `mobile_ip_mismatch`, `mobile_api_key_not_used_quickly_enough`, `desktop_app_disabled`, `mobile_app_disabled`
        /// </summary>
        [JsonPropertyName("query_failure_type")]
        public string QueryFailureType
        {
            get { return (string)attributes["query_failure_type"]; }
            set { attributes["query_failure_type"] = value; unsaved.Add("query_failure_type"); }
        }

        /// <summary>
        /// If searching for Histories about specific objects (such as Users, or API Keys), this parameter restricts results to objects that match this ID.
        /// </summary>
        [JsonPropertyName("query_target_id")]
        public string QueryTargetId
        {
            get { return (string)attributes["query_target_id"]; }
            set { attributes["query_target_id"] = value; unsaved.Add("query_target_id"); }
        }

        /// <summary>
        /// If searching for Histories about Users, Groups or other objects with names, this parameter restricts results to objects with this name/username.
        /// </summary>
        [JsonPropertyName("query_target_name")]
        public string QueryTargetName
        {
            get { return (string)attributes["query_target_name"]; }
            set { attributes["query_target_name"] = value; unsaved.Add("query_target_name"); }
        }

        /// <summary>
        /// If searching for Histories about Permissions, this parameter restricts results to permissions of this level.
        /// </summary>
        [JsonPropertyName("query_target_permission")]
        public string QueryTargetPermission
        {
            get { return (string)attributes["query_target_permission"]; }
            set { attributes["query_target_permission"] = value; unsaved.Add("query_target_permission"); }
        }

        /// <summary>
        /// If searching for Histories about API keys, this parameter restricts results to API keys created by/for this user ID.
        /// </summary>
        [JsonPropertyName("query_target_user_id")]
        public string QueryTargetUserId
        {
            get { return (string)attributes["query_target_user_id"]; }
            set { attributes["query_target_user_id"] = value; unsaved.Add("query_target_user_id"); }
        }

        /// <summary>
        /// If searching for Histories about API keys, this parameter restricts results to API keys created by/for this username.
        /// </summary>
        [JsonPropertyName("query_target_username")]
        public string QueryTargetUsername
        {
            get { return (string)attributes["query_target_username"]; }
            set { attributes["query_target_username"] = value; unsaved.Add("query_target_username"); }
        }

        /// <summary>
        /// If searching for Histories about API keys, this parameter restricts results to API keys associated with this platform.
        /// </summary>
        [JsonPropertyName("query_target_platform")]
        public string QueryTargetPlatform
        {
            get { return (string)attributes["query_target_platform"]; }
            set { attributes["query_target_platform"] = value; unsaved.Add("query_target_platform"); }
        }

        /// <summary>
        /// If searching for Histories about API keys, this parameter restricts results to API keys with this permission set.
        /// </summary>
        [JsonPropertyName("query_target_permission_set")]
        public string QueryTargetPermissionSet
        {
            get { return (string)attributes["query_target_permission_set"]; }
            set { attributes["query_target_permission_set"] = value; unsaved.Add("query_target_permission_set"); }
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


        private static readonly string[] CreateParameterNames = { "user_id", "start_at", "end_at", "query_action", "query_interface", "query_user_id", "query_file_id", "query_parent_id", "query_path", "query_folder", "query_src", "query_destination", "query_ip", "query_username", "query_failure_type", "query_target_id", "query_target_name", "query_target_permission", "query_target_user_id", "query_target_username", "query_target_platform", "query_target_permission_set" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this HistoryExport on Files.com.
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
                throw new NotImplementedException("The HistoryExport object doesn't support updates.");
            }
            HistoryExport created = await HistoryExport.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   id (required) - int64 - History Export ID.
        /// </summary>
        /// <param name="id">History Export ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<HistoryExport> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - History Export ID.
        /// </summary>
        /// <param name="id">History Export ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<HistoryExport> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<HistoryExport> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/history_exports/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<HistoryExport>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   start_at - string - Start date/time of export range.
        ///   end_at - string - End date/time of export range.
        ///   query_action - string - Filter results by this this action type. Valid values: `create`, `read`, `update`, `destroy`, `move`, `login`, `failedlogin`, `copy`, `user_create`, `user_update`, `user_destroy`, `group_create`, `group_update`, `group_destroy`, `permission_create`, `permission_destroy`, `api_key_create`, `api_key_update`, `api_key_destroy`, `archived_delete`
        ///   query_interface - string - Filter results by this this interface type. Valid values: `web`, `ftp`, `robot`, `jsapi`, `webdesktopapi`, `sftp`, `dav`, `desktop`, `restapi`, `scim`, `office`, `mobile`, `as2`, `inbound_email`, `remote`, `inbound_s3`
        ///   query_user_id - string - Return results that are actions performed by the user indicated by this User ID
        ///   query_file_id - string - Return results that are file actions related to the file indicated by this File ID
        ///   query_parent_id - string - Return results that are file actions inside the parent folder specified by this folder ID
        ///   query_path - string - Return results that are file actions related to paths matching this pattern.
        ///   query_folder - string - Return results that are file actions related to files or folders inside folder paths matching this pattern.
        ///   query_src - string - Return results that are file moves originating from paths matching this pattern.
        ///   query_destination - string - Return results that are file moves with paths matching this pattern as destination.
        ///   query_ip - string - Filter results by this IP address.
        ///   query_username - string - Filter results by this username.
        ///   query_failure_type - string - If searching for Histories about login failures, this parameter restricts results to failures of this specific type.  Valid values: `expired_trial`, `account_overdue`, `locked_out`, `ip_mismatch`, `password_mismatch`, `site_mismatch`, `username_not_found`, `none`, `no_ftp_permission`, `no_web_permission`, `no_directory`, `errno_enoent`, `no_sftp_permission`, `no_dav_permission`, `no_restapi_permission`, `key_mismatch`, `region_mismatch`, `expired_access`, `desktop_ip_mismatch`, `desktop_api_key_not_used_quickly_enough`, `disabled`, `country_mismatch`, `insecure_ftp`, `insecure_cipher`, `rate_limited`, `no_s3_compatible_endpoint_permission`, `mobile_ip_mismatch`, `mobile_api_key_not_used_quickly_enough`, `desktop_app_disabled`, `mobile_app_disabled`
        ///   query_target_id - string - If searching for Histories about specific objects (such as Users, or API Keys), this parameter restricts results to objects that match this ID.
        ///   query_target_name - string - If searching for Histories about Users, Groups or other objects with names, this parameter restricts results to objects with this name/username.
        ///   query_target_permission - string - If searching for Histories about Permissions, this parameter restricts results to permissions of this level.
        ///   query_target_user_id - string - If searching for Histories about API keys, this parameter restricts results to API keys created by/for this user ID.
        ///   query_target_username - string - If searching for Histories about API keys, this parameter restricts results to API keys created by/for this username.
        ///   query_target_platform - string - If searching for Histories about API keys, this parameter restricts results to API keys associated with this platform.
        ///   query_target_permission_set - string - If searching for Histories about API keys, this parameter restricts results to API keys with this permission set.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<HistoryExport> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<HistoryExport> CreateCore(
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
            if (parameters.TryGetValue("start_at", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: start_at is null, which cannot be sent" : "Bad parameter: start_at must be of type string", "start_at");
            }
            if (parameters.TryGetValue("end_at", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: end_at is null, which cannot be sent" : "Bad parameter: end_at must be of type string", "end_at");
            }
            if (parameters.TryGetValue("query_action", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_action is null, which cannot be sent" : "Bad parameter: query_action must be of type string", "query_action");
            }
            if (parameters.TryGetValue("query_interface", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_interface is null, which cannot be sent" : "Bad parameter: query_interface must be of type string", "query_interface");
            }
            if (parameters.TryGetValue("query_user_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_user_id is null, which cannot be sent" : "Bad parameter: query_user_id must be of type string", "query_user_id");
            }
            if (parameters.TryGetValue("query_file_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_file_id is null, which cannot be sent" : "Bad parameter: query_file_id must be of type string", "query_file_id");
            }
            if (parameters.TryGetValue("query_parent_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_parent_id is null, which cannot be sent" : "Bad parameter: query_parent_id must be of type string", "query_parent_id");
            }
            if (parameters.TryGetValue("query_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_path is null, which cannot be sent" : "Bad parameter: query_path must be of type string", "query_path");
            }
            if (parameters.TryGetValue("query_folder", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_folder is null, which cannot be sent" : "Bad parameter: query_folder must be of type string", "query_folder");
            }
            if (parameters.TryGetValue("query_src", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_src is null, which cannot be sent" : "Bad parameter: query_src must be of type string", "query_src");
            }
            if (parameters.TryGetValue("query_destination", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_destination is null, which cannot be sent" : "Bad parameter: query_destination must be of type string", "query_destination");
            }
            if (parameters.TryGetValue("query_ip", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_ip is null, which cannot be sent" : "Bad parameter: query_ip must be of type string", "query_ip");
            }
            if (parameters.TryGetValue("query_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_username is null, which cannot be sent" : "Bad parameter: query_username must be of type string", "query_username");
            }
            if (parameters.TryGetValue("query_failure_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_failure_type is null, which cannot be sent" : "Bad parameter: query_failure_type must be of type string", "query_failure_type");
            }
            if (parameters.TryGetValue("query_target_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_target_id is null, which cannot be sent" : "Bad parameter: query_target_id must be of type string", "query_target_id");
            }
            if (parameters.TryGetValue("query_target_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_target_name is null, which cannot be sent" : "Bad parameter: query_target_name must be of type string", "query_target_name");
            }
            if (parameters.TryGetValue("query_target_permission", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_target_permission is null, which cannot be sent" : "Bad parameter: query_target_permission must be of type string", "query_target_permission");
            }
            if (parameters.TryGetValue("query_target_user_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_target_user_id is null, which cannot be sent" : "Bad parameter: query_target_user_id must be of type string", "query_target_user_id");
            }
            if (parameters.TryGetValue("query_target_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_target_username is null, which cannot be sent" : "Bad parameter: query_target_username must be of type string", "query_target_username");
            }
            if (parameters.TryGetValue("query_target_platform", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_target_platform is null, which cannot be sent" : "Bad parameter: query_target_platform must be of type string", "query_target_platform");
            }
            if (parameters.TryGetValue("query_target_permission_set", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: query_target_permission_set is null, which cannot be sent" : "Bad parameter: query_target_permission_set must be of type string", "query_target_permission_set");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/history_exports", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<HistoryExport>(responseJson, context, options);
        }

    }
}