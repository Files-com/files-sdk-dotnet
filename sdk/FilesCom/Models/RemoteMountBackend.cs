using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class RemoteMountBackend
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public RemoteMountBackend() : this(null, null) { }

        public RemoteMountBackend(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;

            if (this.attributes == null)
            {
                this.attributes = new Dictionary<string, object>();
            }

            if (this.options == null)
            {
                this.options = new Dictionary<string, object>();
            }

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

        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        public object GetOption(string name)
        {
            return (this.options.ContainsKey(name) ? this.options[name] : null);
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// Path to the canary file used for health checks.
        /// </summary>
        [JsonPropertyName("canary_file_path")]
        public string CanaryFilePath
        {
            get { return (string)attributes["canary_file_path"]; }
            set { attributes["canary_file_path"] = value; }
        }

        /// <summary>
        /// True if this backend is enabled.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("enabled")]
        public bool Enabled
        {
            get { return attributes["enabled"] == null ? false : (bool)attributes["enabled"]; }
            set { attributes["enabled"] = value; }
        }

        /// <summary>
        /// Number of consecutive failures before considering the backend unhealthy.
        /// </summary>
        [JsonPropertyName("fall")]
        public Nullable<Int64> Fall
        {
            get { return (Nullable<Int64>)attributes["fall"]; }
            set { attributes["fall"] = value; }
        }

        /// <summary>
        /// True if health checks are enabled for this backend.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("health_check_enabled")]
        public bool HealthCheckEnabled
        {
            get { return attributes["health_check_enabled"] == null ? false : (bool)attributes["health_check_enabled"]; }
            set { attributes["health_check_enabled"] = value; }
        }

        /// <summary>
        /// Array of recent health check results.
        /// </summary>
        [JsonPropertyName("health_check_results")]
        public object[] HealthCheckResults
        {
            get { return (object[])attributes["health_check_results"]; }
            set { attributes["health_check_results"] = value; }
        }

        /// <summary>
        /// Type of health check to perform.
        /// </summary>
        [JsonPropertyName("health_check_type")]
        public string HealthCheckType
        {
            get { return (string)attributes["health_check_type"]; }
            set { attributes["health_check_type"] = value; }
        }

        /// <summary>
        /// Unique identifier for this backend.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Interval in seconds between health checks.
        /// </summary>
        [JsonPropertyName("interval")]
        public Nullable<Int64> Interval
        {
            get { return (Nullable<Int64>)attributes["interval"]; }
            set { attributes["interval"] = value; }
        }

        /// <summary>
        /// Minimum free CPU percentage required for this backend to be considered healthy.
        /// </summary>
        [JsonPropertyName("min_free_cpu")]
        public double MinFreeCpu
        {
            get { return (double)attributes["min_free_cpu"]; }
            set { attributes["min_free_cpu"] = value; }
        }

        /// <summary>
        /// Minimum free memory percentage required for this backend to be considered healthy.
        /// </summary>
        [JsonPropertyName("min_free_mem")]
        public double MinFreeMem
        {
            get { return (double)attributes["min_free_mem"]; }
            set { attributes["min_free_mem"] = value; }
        }

        /// <summary>
        /// Priority of this backend.
        /// </summary>
        [JsonPropertyName("priority")]
        public Nullable<Int64> Priority
        {
            get { return (Nullable<Int64>)attributes["priority"]; }
            set { attributes["priority"] = value; }
        }

        /// <summary>
        /// Path on the remote server to treat as the root of this mount.
        /// </summary>
        [JsonPropertyName("remote_path")]
        public string RemotePath
        {
            get { return (string)attributes["remote_path"]; }
            set { attributes["remote_path"] = value; }
        }

        /// <summary>
        /// The remote server that this backend is associated with.
        /// </summary>
        [JsonPropertyName("remote_server_id")]
        public Nullable<Int64> RemoteServerId
        {
            get { return (Nullable<Int64>)attributes["remote_server_id"]; }
            set { attributes["remote_server_id"] = value; }
        }

        /// <summary>
        /// The mount ID of the Remote Server Mount that this backend is associated with.
        /// </summary>
        [JsonPropertyName("remote_server_mount_id")]
        public Nullable<Int64> RemoteServerMountId
        {
            get { return (Nullable<Int64>)attributes["remote_server_mount_id"]; }
            set { attributes["remote_server_mount_id"] = value; }
        }

        /// <summary>
        /// Number of consecutive successes before considering the backend healthy.
        /// </summary>
        [JsonPropertyName("rise")]
        public Nullable<Int64> Rise
        {
            get { return (Nullable<Int64>)attributes["rise"]; }
            set { attributes["rise"] = value; }
        }

        /// <summary>
        /// Status of this backend.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; }
        }

        /// <summary>
        /// True if this backend is undergoing maintenance.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("undergoing_maintenance")]
        public bool UndergoingMaintenance
        {
            get { return attributes["undergoing_maintenance"] == null ? false : (bool)attributes["undergoing_maintenance"]; }
            set { attributes["undergoing_maintenance"] = value; }
        }

        /// <summary>
        /// Reset backend status to healthy
        /// </summary>
        public async Task ResetStatus(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/remote_mount_backends/{System.Uri.EscapeDataString(attributes["id"].ToString())}/reset_status", System.Net.Http.HttpMethod.Post, parameters, options);
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
        public async Task<RemoteMountBackend> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("fall") && !(parameters["fall"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: fall must be of type Nullable<Int64>", "parameters[\"fall\"]");
            }
            if (parameters.ContainsKey("health_check_enabled") && !(parameters["health_check_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: health_check_enabled must be of type bool", "parameters[\"health_check_enabled\"]");
            }
            if (parameters.ContainsKey("health_check_type") && !(parameters["health_check_type"] is string))
            {
                throw new ArgumentException("Bad parameter: health_check_type must be of type string", "parameters[\"health_check_type\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: interval must be of type Nullable<Int64>", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("min_free_cpu") && !(parameters["min_free_cpu"] is double))
            {
                throw new ArgumentException("Bad parameter: min_free_cpu must be of type double", "parameters[\"min_free_cpu\"]");
            }
            if (parameters.ContainsKey("min_free_mem") && !(parameters["min_free_mem"] is double))
            {
                throw new ArgumentException("Bad parameter: min_free_mem must be of type double", "parameters[\"min_free_mem\"]");
            }
            if (parameters.ContainsKey("priority") && !(parameters["priority"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: priority must be of type Nullable<Int64>", "parameters[\"priority\"]");
            }
            if (parameters.ContainsKey("remote_path") && !(parameters["remote_path"] is string))
            {
                throw new ArgumentException("Bad parameter: remote_path must be of type string", "parameters[\"remote_path\"]");
            }
            if (parameters.ContainsKey("rise") && !(parameters["rise"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: rise must be of type Nullable<Int64>", "parameters[\"rise\"]");
            }
            if (parameters.ContainsKey("canary_file_path") && !(parameters["canary_file_path"] is string))
            {
                throw new ArgumentException("Bad parameter: canary_file_path must be of type string", "parameters[\"canary_file_path\"]");
            }
            if (parameters.ContainsKey("remote_server_id") && !(parameters["remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: remote_server_id must be of type Nullable<Int64>", "parameters[\"remote_server_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_mount_backends/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteMountBackend>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/remote_mount_backends/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                await this.Update(this.attributes);
            }
            else
            {
                var newObj = await RemoteMountBackend.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `remote_server_mount_id`.
        /// </summary>
        public static FilesList<RemoteMountBackend> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }

            return new FilesList<RemoteMountBackend>($"/remote_mount_backends", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<RemoteMountBackend> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Mount Backend ID.
        /// </summary>
        public static async Task<RemoteMountBackend> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_mount_backends/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteMountBackend>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<RemoteMountBackend> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
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
        public static async Task<RemoteMountBackend> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("canary_file_path") || parameters["canary_file_path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: canary_file_path", "parameters[\"canary_file_path\"]");
            }
            if (!parameters.ContainsKey("remote_server_mount_id") || parameters["remote_server_mount_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: remote_server_mount_id", "parameters[\"remote_server_mount_id\"]");
            }
            if (!parameters.ContainsKey("remote_server_id") || parameters["remote_server_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: remote_server_id", "parameters[\"remote_server_id\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("fall") && !(parameters["fall"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: fall must be of type Nullable<Int64>", "parameters[\"fall\"]");
            }
            if (parameters.ContainsKey("health_check_enabled") && !(parameters["health_check_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: health_check_enabled must be of type bool", "parameters[\"health_check_enabled\"]");
            }
            if (parameters.ContainsKey("health_check_type") && !(parameters["health_check_type"] is string))
            {
                throw new ArgumentException("Bad parameter: health_check_type must be of type string", "parameters[\"health_check_type\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: interval must be of type Nullable<Int64>", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("min_free_cpu") && !(parameters["min_free_cpu"] is double))
            {
                throw new ArgumentException("Bad parameter: min_free_cpu must be of type double", "parameters[\"min_free_cpu\"]");
            }
            if (parameters.ContainsKey("min_free_mem") && !(parameters["min_free_mem"] is double))
            {
                throw new ArgumentException("Bad parameter: min_free_mem must be of type double", "parameters[\"min_free_mem\"]");
            }
            if (parameters.ContainsKey("priority") && !(parameters["priority"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: priority must be of type Nullable<Int64>", "parameters[\"priority\"]");
            }
            if (parameters.ContainsKey("remote_path") && !(parameters["remote_path"] is string))
            {
                throw new ArgumentException("Bad parameter: remote_path must be of type string", "parameters[\"remote_path\"]");
            }
            if (parameters.ContainsKey("rise") && !(parameters["rise"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: rise must be of type Nullable<Int64>", "parameters[\"rise\"]");
            }
            if (parameters.ContainsKey("canary_file_path") && !(parameters["canary_file_path"] is string))
            {
                throw new ArgumentException("Bad parameter: canary_file_path must be of type string", "parameters[\"canary_file_path\"]");
            }
            if (parameters.ContainsKey("remote_server_mount_id") && !(parameters["remote_server_mount_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: remote_server_mount_id must be of type Nullable<Int64>", "parameters[\"remote_server_mount_id\"]");
            }
            if (parameters.ContainsKey("remote_server_id") && !(parameters["remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: remote_server_id must be of type Nullable<Int64>", "parameters[\"remote_server_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_mount_backends", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteMountBackend>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Reset backend status to healthy
        /// </summary>
        public static async Task ResetStatus(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/remote_mount_backends/{System.Uri.EscapeDataString(parameters["id"].ToString())}/reset_status", System.Net.Http.HttpMethod.Post, parameters, options);
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
        public static async Task<RemoteMountBackend> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("fall") && !(parameters["fall"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: fall must be of type Nullable<Int64>", "parameters[\"fall\"]");
            }
            if (parameters.ContainsKey("health_check_enabled") && !(parameters["health_check_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: health_check_enabled must be of type bool", "parameters[\"health_check_enabled\"]");
            }
            if (parameters.ContainsKey("health_check_type") && !(parameters["health_check_type"] is string))
            {
                throw new ArgumentException("Bad parameter: health_check_type must be of type string", "parameters[\"health_check_type\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: interval must be of type Nullable<Int64>", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("min_free_cpu") && !(parameters["min_free_cpu"] is double))
            {
                throw new ArgumentException("Bad parameter: min_free_cpu must be of type double", "parameters[\"min_free_cpu\"]");
            }
            if (parameters.ContainsKey("min_free_mem") && !(parameters["min_free_mem"] is double))
            {
                throw new ArgumentException("Bad parameter: min_free_mem must be of type double", "parameters[\"min_free_mem\"]");
            }
            if (parameters.ContainsKey("priority") && !(parameters["priority"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: priority must be of type Nullable<Int64>", "parameters[\"priority\"]");
            }
            if (parameters.ContainsKey("remote_path") && !(parameters["remote_path"] is string))
            {
                throw new ArgumentException("Bad parameter: remote_path must be of type string", "parameters[\"remote_path\"]");
            }
            if (parameters.ContainsKey("rise") && !(parameters["rise"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: rise must be of type Nullable<Int64>", "parameters[\"rise\"]");
            }
            if (parameters.ContainsKey("canary_file_path") && !(parameters["canary_file_path"] is string))
            {
                throw new ArgumentException("Bad parameter: canary_file_path must be of type string", "parameters[\"canary_file_path\"]");
            }
            if (parameters.ContainsKey("remote_server_id") && !(parameters["remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: remote_server_id must be of type Nullable<Int64>", "parameters[\"remote_server_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_mount_backends/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteMountBackend>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public static async Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/remote_mount_backends/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(id, parameters, options);
        }

    }
}