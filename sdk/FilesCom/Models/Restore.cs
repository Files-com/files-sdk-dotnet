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
    /// RestoreEntity model
    /// </summary>
    public class Restore : IModel, IJsonOnDeserialized
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
        /// Makes a Restore with no attributes set and no options.
        /// </summary>
        public Restore() : this(null, null) { }

        /// <summary>
        /// Makes a Restore with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Restore(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("earliest_date"))
            {
                this.attributes.Add("earliest_date", null);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("dirs_restored"))
            {
                this.attributes.Add("dirs_restored", null);
            }
            if (!this.attributes.ContainsKey("dirs_errored"))
            {
                this.attributes.Add("dirs_errored", null);
            }
            if (!this.attributes.ContainsKey("dirs_total"))
            {
                this.attributes.Add("dirs_total", null);
            }
            if (!this.attributes.ContainsKey("files_restored"))
            {
                this.attributes.Add("files_restored", null);
            }
            if (!this.attributes.ContainsKey("files_errored"))
            {
                this.attributes.Add("files_errored", null);
            }
            if (!this.attributes.ContainsKey("files_total"))
            {
                this.attributes.Add("files_total", null);
            }
            if (!this.attributes.ContainsKey("prefix"))
            {
                this.attributes.Add("prefix", null);
            }
            if (!this.attributes.ContainsKey("restoration_type"))
            {
                this.attributes.Add("restoration_type", null);
            }
            if (!this.attributes.ContainsKey("restore_in_place"))
            {
                this.attributes.Add("restore_in_place", false);
            }
            if (!this.attributes.ContainsKey("restore_deleted_permissions"))
            {
                this.attributes.Add("restore_deleted_permissions", false);
            }
            if (!this.attributes.ContainsKey("users_restored"))
            {
                this.attributes.Add("users_restored", null);
            }
            if (!this.attributes.ContainsKey("users_errored"))
            {
                this.attributes.Add("users_errored", null);
            }
            if (!this.attributes.ContainsKey("users_total"))
            {
                this.attributes.Add("users_total", null);
            }
            if (!this.attributes.ContainsKey("api_keys_restored"))
            {
                this.attributes.Add("api_keys_restored", null);
            }
            if (!this.attributes.ContainsKey("public_keys_restored"))
            {
                this.attributes.Add("public_keys_restored", null);
            }
            if (!this.attributes.ContainsKey("two_factor_authentication_methods_restored"))
            {
                this.attributes.Add("two_factor_authentication_methods_restored", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("update_timestamps"))
            {
                this.attributes.Add("update_timestamps", false);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("error_messages"))
            {
                this.attributes.Add("error_messages", new string[0]);
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
        /// Restore files or users deleted on or after this date/time. Don't set this earlier than you need. Can not be greater than 365 days prior to the restore request.
        /// </summary>
        [JsonPropertyName("earliest_date")]
        public Nullable<DateTime> EarliestDate
        {
            get { return (Nullable<DateTime>)attributes["earliest_date"]; }
            set { attributes["earliest_date"] = value; unsaved.Add("earliest_date"); }
        }

        /// <summary>
        /// Restore Record ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Number of directories that were successfully restored.
        /// </summary>
        [JsonPropertyName("dirs_restored")]
        public Nullable<Int64> DirsRestored
        {
            get { return (Nullable<Int64>)attributes["dirs_restored"]; }
            set { attributes["dirs_restored"] = value; unsaved.Add("dirs_restored"); }
        }

        /// <summary>
        /// Number of directories that were not able to be restored.
        /// </summary>
        [JsonPropertyName("dirs_errored")]
        public Nullable<Int64> DirsErrored
        {
            get { return (Nullable<Int64>)attributes["dirs_errored"]; }
            set { attributes["dirs_errored"] = value; unsaved.Add("dirs_errored"); }
        }

        /// <summary>
        /// Total number of directories processed.
        /// </summary>
        [JsonPropertyName("dirs_total")]
        public Nullable<Int64> DirsTotal
        {
            get { return (Nullable<Int64>)attributes["dirs_total"]; }
            set { attributes["dirs_total"] = value; unsaved.Add("dirs_total"); }
        }

        /// <summary>
        /// Number of files successfully restored.
        /// </summary>
        [JsonPropertyName("files_restored")]
        public Nullable<Int64> FilesRestored
        {
            get { return (Nullable<Int64>)attributes["files_restored"]; }
            set { attributes["files_restored"] = value; unsaved.Add("files_restored"); }
        }

        /// <summary>
        /// Number of files that were not able to be restored.
        /// </summary>
        [JsonPropertyName("files_errored")]
        public Nullable<Int64> FilesErrored
        {
            get { return (Nullable<Int64>)attributes["files_errored"]; }
            set { attributes["files_errored"] = value; unsaved.Add("files_errored"); }
        }

        /// <summary>
        /// Total number of files processed.
        /// </summary>
        [JsonPropertyName("files_total")]
        public Nullable<Int64> FilesTotal
        {
            get { return (Nullable<Int64>)attributes["files_total"]; }
            set { attributes["files_total"] = value; unsaved.Add("files_total"); }
        }

        /// <summary>
        /// Prefix of the files/folders to restore, or a case-insensitive username prefix for a user restore. A trailing slash selects that folder and its contents only; without it, the prefix matches any path beginning with that text. Do not use a leading slash. To restore all deleted items of the selected restoration type within the selected site or workspace scope, specify an empty string (`''`) or omit the field.
        /// </summary>
        [JsonPropertyName("prefix")]
        public string Prefix
        {
            get { return (string)attributes["prefix"]; }
            set { attributes["prefix"] = value; unsaved.Add("prefix"); }
        }

        /// <summary>
        /// Type of restoration to perform. `files` restores deleted filesystem items. `users` restores deleted users and associated authentication records, plus permissions when `restore_deleted_permissions=true`. A 5-minute grace period allows for batch deletion processes: associated records deleted within the 5 minutes before the user are included, even if removed independently. Records deleted earlier, or before `earliest_date`, remain deleted.
        /// </summary>
        [JsonPropertyName("restoration_type")]
        public string RestorationType
        {
            get { return (string)attributes["restoration_type"]; }
            set { attributes["restoration_type"] = value; unsaved.Add("restoration_type"); }
        }

        /// <summary>
        /// If true, we will restore the files in place (into their original paths). If false, we will create a new restoration folder in the root and restore files there.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("restore_in_place")]
        public bool RestoreInPlace
        {
            get { return attributes["restore_in_place"] == null ? false : (bool)attributes["restore_in_place"]; }
            set { attributes["restore_in_place"] = value; unsaved.Add("restore_in_place"); }
        }

        /// <summary>
        /// If true, a user restore restores permissions deleted along with the selected users, including permissions removed independently within the 5 minutes before user deletion. Permissions deleted earlier, or before `earliest_date`, remain deleted. An in-place file restore restores user and group permissions on returned folders and their subfolders that were deleted from the time of the folder's deletion through 24 hours afterward, including permissions removed independently within that window. File restores into a new folder do not restore permissions.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("restore_deleted_permissions")]
        public bool RestoreDeletedPermissions
        {
            get { return attributes["restore_deleted_permissions"] == null ? false : (bool)attributes["restore_deleted_permissions"]; }
            set { attributes["restore_deleted_permissions"] = value; unsaved.Add("restore_deleted_permissions"); }
        }

        /// <summary>
        /// Number of users successfully restored (only present for `restoration_type=users`).
        /// </summary>
        [JsonPropertyName("users_restored")]
        public Nullable<Int64> UsersRestored
        {
            get { return (Nullable<Int64>)attributes["users_restored"]; }
            set { attributes["users_restored"] = value; unsaved.Add("users_restored"); }
        }

        /// <summary>
        /// Number of users that failed to restore (only present for `restoration_type=users`).
        /// </summary>
        [JsonPropertyName("users_errored")]
        public Nullable<Int64> UsersErrored
        {
            get { return (Nullable<Int64>)attributes["users_errored"]; }
            set { attributes["users_errored"] = value; unsaved.Add("users_errored"); }
        }

        /// <summary>
        /// Total number of users processed (only present for `restoration_type=users`).
        /// </summary>
        [JsonPropertyName("users_total")]
        public Nullable<Int64> UsersTotal
        {
            get { return (Nullable<Int64>)attributes["users_total"]; }
            set { attributes["users_total"] = value; unsaved.Add("users_total"); }
        }

        /// <summary>
        /// Number of API keys restored (only present for `restoration_type=users`).
        /// </summary>
        [JsonPropertyName("api_keys_restored")]
        public Nullable<Int64> ApiKeysRestored
        {
            get { return (Nullable<Int64>)attributes["api_keys_restored"]; }
            set { attributes["api_keys_restored"] = value; unsaved.Add("api_keys_restored"); }
        }

        /// <summary>
        /// Number of public keys restored (only present for `restoration_type=users`).
        /// </summary>
        [JsonPropertyName("public_keys_restored")]
        public Nullable<Int64> PublicKeysRestored
        {
            get { return (Nullable<Int64>)attributes["public_keys_restored"]; }
            set { attributes["public_keys_restored"] = value; unsaved.Add("public_keys_restored"); }
        }

        /// <summary>
        /// Number of two factor authentication methods restored (only present for `restoration_type=users`).
        /// </summary>
        [JsonPropertyName("two_factor_authentication_methods_restored")]
        public Nullable<Int64> TwoFactorAuthenticationMethodsRestored
        {
            get { return (Nullable<Int64>)attributes["two_factor_authentication_methods_restored"]; }
            set { attributes["two_factor_authentication_methods_restored"] = value; unsaved.Add("two_factor_authentication_methods_restored"); }
        }

        /// <summary>
        /// Status of the restoration process.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; unsaved.Add("status"); }
        }

        /// <summary>
        /// If true, we will update the last modified timestamp of restored files to today's date. If false, we might trigger File Expiration to delete the file again.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("update_timestamps")]
        public bool UpdateTimestamps
        {
            get { return attributes["update_timestamps"] == null ? false : (bool)attributes["update_timestamps"]; }
            set { attributes["update_timestamps"] = value; unsaved.Add("update_timestamps"); }
        }

        /// <summary>
        /// Workspace ID for a workspace-scoped restore. `0` means the default site-wide scope.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Error messages received while restoring files and/or directories. Only present if there were errors.
        /// </summary>
        [JsonPropertyName("error_messages")]
        public string[] ErrorMessages
        {
            get { return (string[])attributes["error_messages"]; }
            set { attributes["error_messages"] = value; unsaved.Add("error_messages"); }
        }


        private static readonly string[] CreateParameterNames = { "earliest_date", "prefix", "restoration_type", "restore_deleted_permissions", "restore_in_place", "update_timestamps", "workspace_id" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Restore on Files.com.
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
                throw new NotImplementedException("The Restore object doesn't support updates.");
            }
            Restore created = await Restore.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
                case "earliest_date":
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
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `restoration_type`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Restore> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `restoration_type`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Restore> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Restore> ListCore(
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

            return new FilesList<Restore>(client, $"/restores", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   earliest_date (required) - string - Restore files or users deleted on or after this date/time. Don't set this earlier than you need. Can not be greater than 365 days prior to the restore request.
        ///   prefix - string - Prefix of the files/folders to restore, or a case-insensitive username prefix for a user restore. A trailing slash selects that folder and its contents only; without it, the prefix matches any path beginning with that text. Do not use a leading slash. To restore all deleted items of the selected restoration type within the selected site or workspace scope, specify an empty string (`''`) or omit the field.
        ///   restoration_type - string - Type of restoration to perform. `files` restores deleted filesystem items. `users` restores deleted users and associated authentication records, plus permissions when `restore_deleted_permissions=true`. A 5-minute grace period allows for batch deletion processes: associated records deleted within the 5 minutes before the user are included, even if removed independently. Records deleted earlier, or before `earliest_date`, remain deleted.
        ///   restore_deleted_permissions - boolean - If true, a user restore restores permissions deleted along with the selected users, including permissions removed independently within the 5 minutes before user deletion. Permissions deleted earlier, or before `earliest_date`, remain deleted. An in-place file restore restores user and group permissions on returned folders and their subfolders that were deleted from the time of the folder's deletion through 24 hours afterward, including permissions removed independently within that window. File restores into a new folder do not restore permissions.
        ///   restore_in_place - boolean - If true, we will restore the files in place (into their original paths). If false, we will create a new restoration folder in the root and restore files there.
        ///   update_timestamps - boolean - If true, we will update the last modified timestamp of restored files to today's date. If false, we might trigger File Expiration to delete the file again.
        ///   workspace_id - int64 - Workspace ID for a workspace-scoped restore. `0` means the default site-wide scope.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Restore> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Restore> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("earliest_date", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: earliest_date", "earliest_date");
            }
            if (parameters.TryGetValue("earliest_date", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: earliest_date is null, which cannot be sent" : "Bad parameter: earliest_date must be of type string", "earliest_date");
            }
            if (parameters.TryGetValue("prefix", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: prefix is null, which cannot be sent" : "Bad parameter: prefix must be of type string", "prefix");
            }
            if (parameters.TryGetValue("restoration_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restoration_type is null, which cannot be sent" : "Bad parameter: restoration_type must be of type string", "restoration_type");
            }
            if (parameters.TryGetValue("restore_deleted_permissions", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restore_deleted_permissions is null, which cannot be sent" : "Bad parameter: restore_deleted_permissions must be of type bool", "restore_deleted_permissions");
            }
            if (parameters.TryGetValue("restore_in_place", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: restore_in_place is null, which cannot be sent" : "Bad parameter: restore_in_place must be of type bool", "restore_in_place");
            }
            if (parameters.TryGetValue("update_timestamps", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: update_timestamps is null, which cannot be sent" : "Bad parameter: update_timestamps must be of type bool", "update_timestamps");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/restores", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Restore>(responseJson, context, options);
        }

    }
}