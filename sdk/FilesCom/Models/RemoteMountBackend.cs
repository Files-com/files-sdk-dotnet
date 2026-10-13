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
    /// RemoteMountBackendEntity model
    /// </summary>
    public class RemoteMountBackend : IModel, IJsonOnDeserialized
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
        /// Makes a RemoteMountBackend with no attributes set and no options.
        /// </summary>
        public RemoteMountBackend() : this(null, null) { }

        /// <summary>
        /// Makes a RemoteMountBackend with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public RemoteMountBackend(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("canary_file_path"))
            {
                this.attributes.Add("canary_file_path", null);
            }
            if (!this.attributes.ContainsKey("enabled"))
            {
                this.attributes.Add("enabled", false);
            }
            if (!this.attributes.ContainsKey("fall"))
            {
                this.attributes.Add("fall", null);
            }
            if (!this.attributes.ContainsKey("health_check_enabled"))
            {
                this.attributes.Add("health_check_enabled", false);
            }
            if (!this.attributes.ContainsKey("health_check_results"))
            {
                this.attributes.Add("health_check_results", new object[0]);
            }
            if (!this.attributes.ContainsKey("health_check_type"))
            {
                this.attributes.Add("health_check_type", null);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("interval"))
            {
                this.attributes.Add("interval", null);
            }
            if (!this.attributes.ContainsKey("min_free_cpu"))
            {
                this.attributes.Add("min_free_cpu", null);
            }
            if (!this.attributes.ContainsKey("min_free_mem"))
            {
                this.attributes.Add("min_free_mem", null);
            }
            if (!this.attributes.ContainsKey("priority"))
            {
                this.attributes.Add("priority", null);
            }
            if (!this.attributes.ContainsKey("remote_path"))
            {
                this.attributes.Add("remote_path", null);
            }
            if (!this.attributes.ContainsKey("remote_server_id"))
            {
                this.attributes.Add("remote_server_id", null);
            }
            if (!this.attributes.ContainsKey("remote_server_mount_id"))
            {
                this.attributes.Add("remote_server_mount_id", null);
            }
            if (!this.attributes.ContainsKey("rise"))
            {
                this.attributes.Add("rise", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("undergoing_maintenance"))
            {
                this.attributes.Add("undergoing_maintenance", false);
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
        /// Path to the canary file used for health checks.
        /// </summary>
        [JsonPropertyName("canary_file_path")]
        public string CanaryFilePath
        {
            get { return (string)attributes["canary_file_path"]; }
            set { attributes["canary_file_path"] = value; unsaved.Add("canary_file_path"); }
        }

        /// <summary>
        /// True if this backend is enabled.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("enabled")]
        public bool Enabled
        {
            get { return attributes["enabled"] == null ? false : (bool)attributes["enabled"]; }
            set { attributes["enabled"] = value; unsaved.Add("enabled"); }
        }

        /// <summary>
        /// Number of consecutive failures before considering the backend unhealthy.
        /// </summary>
        [JsonPropertyName("fall")]
        public Nullable<Int64> Fall
        {
            get { return (Nullable<Int64>)attributes["fall"]; }
            set { attributes["fall"] = value; unsaved.Add("fall"); }
        }

        /// <summary>
        /// True if health checks are enabled for this backend.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("health_check_enabled")]
        public bool HealthCheckEnabled
        {
            get { return attributes["health_check_enabled"] == null ? false : (bool)attributes["health_check_enabled"]; }
            set { attributes["health_check_enabled"] = value; unsaved.Add("health_check_enabled"); }
        }

        /// <summary>
        /// Array of recent health check results.
        /// </summary>
        [JsonPropertyName("health_check_results")]
        public object[] HealthCheckResults
        {
            get { return (object[])attributes["health_check_results"]; }
            set { attributes["health_check_results"] = value; unsaved.Add("health_check_results"); }
        }

        /// <summary>
        /// Type of health check to perform.
        /// </summary>
        [JsonPropertyName("health_check_type")]
        public string HealthCheckType
        {
            get { return (string)attributes["health_check_type"]; }
            set { attributes["health_check_type"] = value; unsaved.Add("health_check_type"); }
        }

        /// <summary>
        /// Unique identifier for this backend.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Interval in seconds between health checks.
        /// </summary>
        [JsonPropertyName("interval")]
        public Nullable<Int64> Interval
        {
            get { return (Nullable<Int64>)attributes["interval"]; }
            set { attributes["interval"] = value; unsaved.Add("interval"); }
        }

        /// <summary>
        /// Minimum free CPU percentage required for this backend to be considered healthy.
        /// </summary>
        [JsonPropertyName("min_free_cpu")]
        public Nullable<decimal> MinFreeCpu
        {
            get { return (Nullable<decimal>)attributes["min_free_cpu"]; }
            set { attributes["min_free_cpu"] = value; unsaved.Add("min_free_cpu"); }
        }

        /// <summary>
        /// Minimum free memory percentage required for this backend to be considered healthy.
        /// </summary>
        [JsonPropertyName("min_free_mem")]
        public Nullable<decimal> MinFreeMem
        {
            get { return (Nullable<decimal>)attributes["min_free_mem"]; }
            set { attributes["min_free_mem"] = value; unsaved.Add("min_free_mem"); }
        }

        /// <summary>
        /// Priority of this backend.
        /// </summary>
        [JsonPropertyName("priority")]
        public Nullable<Int64> Priority
        {
            get { return (Nullable<Int64>)attributes["priority"]; }
            set { attributes["priority"] = value; unsaved.Add("priority"); }
        }

        /// <summary>
        /// Path on the remote server to treat as the root of this mount.
        /// </summary>
        [JsonPropertyName("remote_path")]
        public string RemotePath
        {
            get { return (string)attributes["remote_path"]; }
            set { attributes["remote_path"] = value; unsaved.Add("remote_path"); }
        }

        /// <summary>
        /// The remote server that this backend is associated with.
        /// </summary>
        [JsonPropertyName("remote_server_id")]
        public Nullable<Int64> RemoteServerId
        {
            get { return (Nullable<Int64>)attributes["remote_server_id"]; }
            set { attributes["remote_server_id"] = value; unsaved.Add("remote_server_id"); }
        }

        /// <summary>
        /// The mount ID of the Remote Server Mount that this backend is associated with.
        /// </summary>
        [JsonPropertyName("remote_server_mount_id")]
        public Nullable<Int64> RemoteServerMountId
        {
            get { return (Nullable<Int64>)attributes["remote_server_mount_id"]; }
            set { attributes["remote_server_mount_id"] = value; unsaved.Add("remote_server_mount_id"); }
        }

        /// <summary>
        /// Number of consecutive successes before considering the backend healthy.
        /// </summary>
        [JsonPropertyName("rise")]
        public Nullable<Int64> Rise
        {
            get { return (Nullable<Int64>)attributes["rise"]; }
            set { attributes["rise"] = value; unsaved.Add("rise"); }
        }

        /// <summary>
        /// Status of this backend.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; unsaved.Add("status"); }
        }

        /// <summary>
        /// True if this backend is undergoing maintenance.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("undergoing_maintenance")]
        public bool UndergoingMaintenance
        {
            get { return attributes["undergoing_maintenance"] == null ? false : (bool)attributes["undergoing_maintenance"]; }
            set { attributes["undergoing_maintenance"] = value; unsaved.Add("undergoing_maintenance"); }
        }

        /// <summary>
        /// Reset backend status to healthy
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task ResetStatus(Dictionary<string, object> parameters)
        {
            return ResetStatusCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Reset backend status to healthy
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task ResetStatusAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ResetStatusCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task ResetStatusCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteMountBackend has no id, which ResetStatus needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/remote_mount_backends/{System.Uri.EscapeDataString(attributes["id"].ToString())}/reset_status", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Parameters:
        ///   enabled - boolean - True if this backend is enabled.
        ///   fall - int64 - Number of consecutive failures before considering the backend unhealthy.
        ///   health_check_enabled - boolean - True if health checks are enabled for this backend.
        ///   health_check_type - string - Type of health check to perform.
        ///   interval - int64 - Interval in seconds between health checks.
        ///   min_free_cpu - double - Minimum free CPU percentage required for this backend to be considered healthy.
        ///   min_free_mem - double - Minimum free memory percentage required for this backend to be considered healthy.
        ///   priority - int64 - Priority of this backend.
        ///   remote_path - string - Path on the remote server to treat as the root of this mount.
        ///   rise - int64 - Number of consecutive successes before considering the backend healthy.
        ///   canary_file_path - string - Path to the canary file used for health checks.
        ///   remote_server_id - int64 - The remote server that this backend is associated with.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The RemoteMountBackend it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<RemoteMountBackend> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   enabled - boolean - True if this backend is enabled.
        ///   fall - int64 - Number of consecutive failures before considering the backend unhealthy.
        ///   health_check_enabled - boolean - True if health checks are enabled for this backend.
        ///   health_check_type - string - Type of health check to perform.
        ///   interval - int64 - Interval in seconds between health checks.
        ///   min_free_cpu - double - Minimum free CPU percentage required for this backend to be considered healthy.
        ///   min_free_mem - double - Minimum free memory percentage required for this backend to be considered healthy.
        ///   priority - int64 - Priority of this backend.
        ///   remote_path - string - Path on the remote server to treat as the root of this mount.
        ///   rise - int64 - Number of consecutive successes before considering the backend healthy.
        ///   canary_file_path - string - Path to the canary file used for health checks.
        ///   remote_server_id - int64 - The remote server that this backend is associated with.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The RemoteMountBackend it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<RemoteMountBackend> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<RemoteMountBackend> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteMountBackend has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enabled is null, which cannot be sent" : "Bad parameter: enabled must be of type bool", "enabled");
            }
            if (parameters.TryGetValue("fall", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: fall is null, which cannot be sent" : "Bad parameter: fall must be of type Nullable<Int64>", "fall");
            }
            if (parameters.TryGetValue("health_check_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: health_check_enabled is null, which cannot be sent" : "Bad parameter: health_check_enabled must be of type bool", "health_check_enabled");
            }
            if (parameters.TryGetValue("health_check_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: health_check_type is null, which cannot be sent" : "Bad parameter: health_check_type must be of type string", "health_check_type");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type Nullable<Int64>", "interval");
            }
            if (parameters.TryGetValue("min_free_cpu", out parameterValue) && !(parameterValue is decimal || parameterValue is Nullable<double>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: min_free_cpu is null, which cannot be sent" : "Bad parameter: min_free_cpu must be of type decimal or Nullable<double>", "min_free_cpu");
            }
            if (parameters.TryGetValue("min_free_mem", out parameterValue) && !(parameterValue is decimal || parameterValue is Nullable<double>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: min_free_mem is null, which cannot be sent" : "Bad parameter: min_free_mem must be of type decimal or Nullable<double>", "min_free_mem");
            }
            if (parameters.TryGetValue("priority", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: priority is null, which cannot be sent" : "Bad parameter: priority must be of type Nullable<Int64>", "priority");
            }
            if (parameters.TryGetValue("remote_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_path is null, which cannot be sent" : "Bad parameter: remote_path must be of type string", "remote_path");
            }
            if (parameters.TryGetValue("rise", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: rise is null, which cannot be sent" : "Bad parameter: rise must be of type Nullable<Int64>", "rise");
            }
            if (parameters.TryGetValue("canary_file_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: canary_file_path is null, which cannot be sent" : "Bad parameter: canary_file_path must be of type string", "canary_file_path");
            }
            if (parameters.TryGetValue("remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_server_id is null, which cannot be sent" : "Bad parameter: remote_server_id must be of type Nullable<Int64>", "remote_server_id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_mount_backends/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            RemoteMountBackend updated = JsonUtil.DeserializeModelResponse<RemoteMountBackend>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<RemoteMountBackend>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /remote_mount_backends/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /remote_mount_backends/{id} to the Files.com API.
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
                throw new MissingParameterException("The RemoteMountBackend has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/remote_mount_backends/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "enabled", "fall", "health_check_enabled", "health_check_type", "interval", "min_free_cpu", "min_free_mem", "priority", "remote_path", "rise", "canary_file_path", "remote_server_mount_id", "remote_server_id" };
        private static readonly string[] UpdateParameterNames = { "id", "enabled", "fall", "health_check_enabled", "health_check_type", "interval", "min_free_cpu", "min_free_mem", "priority", "remote_path", "rise", "canary_file_path", "remote_server_id" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this RemoteMountBackend on Files.com, or updates it if it has an ID.
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
            RemoteMountBackend created = await RemoteMountBackend.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `remote_server_mount_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<RemoteMountBackend> List(

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
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `remote_server_mount_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<RemoteMountBackend> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<RemoteMountBackend> ListCore(
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
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }

            return new FilesList<RemoteMountBackend>(client, $"/remote_mount_backends", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Mount Backend ID.
        /// </summary>
        /// <param name="id">Remote Mount Backend ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteMountBackend> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Mount Backend ID.
        /// </summary>
        /// <param name="id">Remote Mount Backend ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteMountBackend> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<RemoteMountBackend> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_mount_backends/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteMountBackend>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   enabled - boolean - True if this backend is enabled.
        ///   fall - int64 - Number of consecutive failures before considering the backend unhealthy.
        ///   health_check_enabled - boolean - True if health checks are enabled for this backend.
        ///   health_check_type - string - Type of health check to perform.
        ///   interval - int64 - Interval in seconds between health checks.
        ///   min_free_cpu - double - Minimum free CPU percentage required for this backend to be considered healthy.
        ///   min_free_mem - double - Minimum free memory percentage required for this backend to be considered healthy.
        ///   priority - int64 - Priority of this backend.
        ///   remote_path - string - Path on the remote server to treat as the root of this mount.
        ///   rise - int64 - Number of consecutive successes before considering the backend healthy.
        ///   canary_file_path (required) - string - Path to the canary file used for health checks.
        ///   remote_server_mount_id (required) - int64 - The mount ID of the Remote Server Mount that this backend is associated with.
        ///   remote_server_id (required) - int64 - The remote server that this backend is associated with.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteMountBackend> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteMountBackend> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("canary_file_path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: canary_file_path", "canary_file_path");
            }
            if (!parameters.TryGetValue("remote_server_mount_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: remote_server_mount_id", "remote_server_mount_id");
            }
            if (!parameters.TryGetValue("remote_server_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: remote_server_id", "remote_server_id");
            }
            if (parameters.TryGetValue("enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enabled is null, which cannot be sent" : "Bad parameter: enabled must be of type bool", "enabled");
            }
            if (parameters.TryGetValue("fall", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: fall is null, which cannot be sent" : "Bad parameter: fall must be of type Nullable<Int64>", "fall");
            }
            if (parameters.TryGetValue("health_check_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: health_check_enabled is null, which cannot be sent" : "Bad parameter: health_check_enabled must be of type bool", "health_check_enabled");
            }
            if (parameters.TryGetValue("health_check_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: health_check_type is null, which cannot be sent" : "Bad parameter: health_check_type must be of type string", "health_check_type");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type Nullable<Int64>", "interval");
            }
            if (parameters.TryGetValue("min_free_cpu", out parameterValue) && !(parameterValue is decimal || parameterValue is Nullable<double>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: min_free_cpu is null, which cannot be sent" : "Bad parameter: min_free_cpu must be of type decimal or Nullable<double>", "min_free_cpu");
            }
            if (parameters.TryGetValue("min_free_mem", out parameterValue) && !(parameterValue is decimal || parameterValue is Nullable<double>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: min_free_mem is null, which cannot be sent" : "Bad parameter: min_free_mem must be of type decimal or Nullable<double>", "min_free_mem");
            }
            if (parameters.TryGetValue("priority", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: priority is null, which cannot be sent" : "Bad parameter: priority must be of type Nullable<Int64>", "priority");
            }
            if (parameters.TryGetValue("remote_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_path is null, which cannot be sent" : "Bad parameter: remote_path must be of type string", "remote_path");
            }
            if (parameters.TryGetValue("rise", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: rise is null, which cannot be sent" : "Bad parameter: rise must be of type Nullable<Int64>", "rise");
            }
            if (parameters.TryGetValue("canary_file_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: canary_file_path is null, which cannot be sent" : "Bad parameter: canary_file_path must be of type string", "canary_file_path");
            }
            if (parameters.TryGetValue("remote_server_mount_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_server_mount_id is null, which cannot be sent" : "Bad parameter: remote_server_mount_id must be of type Nullable<Int64>", "remote_server_mount_id");
            }
            if (parameters.TryGetValue("remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_server_id is null, which cannot be sent" : "Bad parameter: remote_server_id must be of type Nullable<Int64>", "remote_server_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_mount_backends", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteMountBackend>(responseJson, context, options);
        }

        /// <summary>
        /// Reset backend status to healthy
        /// </summary>
        /// <param name="id">Remote Mount Backend ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task ResetStatus(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ResetStatusCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task ResetStatusCore(
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

            await FilesClient.SendRequestAndDiscard(context, $"/remote_mount_backends/{System.Uri.EscapeDataString(parameters["id"].ToString())}/reset_status", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   enabled - boolean - True if this backend is enabled.
        ///   fall - int64 - Number of consecutive failures before considering the backend unhealthy.
        ///   health_check_enabled - boolean - True if health checks are enabled for this backend.
        ///   health_check_type - string - Type of health check to perform.
        ///   interval - int64 - Interval in seconds between health checks.
        ///   min_free_cpu - double - Minimum free CPU percentage required for this backend to be considered healthy.
        ///   min_free_mem - double - Minimum free memory percentage required for this backend to be considered healthy.
        ///   priority - int64 - Priority of this backend.
        ///   remote_path - string - Path on the remote server to treat as the root of this mount.
        ///   rise - int64 - Number of consecutive successes before considering the backend healthy.
        ///   canary_file_path - string - Path to the canary file used for health checks.
        ///   remote_server_id - int64 - The remote server that this backend is associated with.
        /// </summary>
        /// <param name="id">Remote Mount Backend ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteMountBackend> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteMountBackend> UpdateCore(
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
            if (parameters.TryGetValue("enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enabled is null, which cannot be sent" : "Bad parameter: enabled must be of type bool", "enabled");
            }
            if (parameters.TryGetValue("fall", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: fall is null, which cannot be sent" : "Bad parameter: fall must be of type Nullable<Int64>", "fall");
            }
            if (parameters.TryGetValue("health_check_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: health_check_enabled is null, which cannot be sent" : "Bad parameter: health_check_enabled must be of type bool", "health_check_enabled");
            }
            if (parameters.TryGetValue("health_check_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: health_check_type is null, which cannot be sent" : "Bad parameter: health_check_type must be of type string", "health_check_type");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type Nullable<Int64>", "interval");
            }
            if (parameters.TryGetValue("min_free_cpu", out parameterValue) && !(parameterValue is decimal || parameterValue is Nullable<double>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: min_free_cpu is null, which cannot be sent" : "Bad parameter: min_free_cpu must be of type decimal or Nullable<double>", "min_free_cpu");
            }
            if (parameters.TryGetValue("min_free_mem", out parameterValue) && !(parameterValue is decimal || parameterValue is Nullable<double>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: min_free_mem is null, which cannot be sent" : "Bad parameter: min_free_mem must be of type decimal or Nullable<double>", "min_free_mem");
            }
            if (parameters.TryGetValue("priority", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: priority is null, which cannot be sent" : "Bad parameter: priority must be of type Nullable<Int64>", "priority");
            }
            if (parameters.TryGetValue("remote_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_path is null, which cannot be sent" : "Bad parameter: remote_path must be of type string", "remote_path");
            }
            if (parameters.TryGetValue("rise", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: rise is null, which cannot be sent" : "Bad parameter: rise must be of type Nullable<Int64>", "rise");
            }
            if (parameters.TryGetValue("canary_file_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: canary_file_path is null, which cannot be sent" : "Bad parameter: canary_file_path must be of type string", "canary_file_path");
            }
            if (parameters.TryGetValue("remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_server_id is null, which cannot be sent" : "Bad parameter: remote_server_id must be of type Nullable<Int64>", "remote_server_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_mount_backends/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteMountBackend>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /remote_mount_backends/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Remote Mount Backend ID.</param>
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
        /// Sends DELETE /remote_mount_backends/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Remote Mount Backend ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/remote_mount_backends/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}