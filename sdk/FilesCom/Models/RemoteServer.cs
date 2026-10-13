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
    /// RemoteServerEntity model
    /// </summary>
    public class RemoteServer : IModel, IJsonOnDeserialized
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
        /// Makes a RemoteServer with no attributes set and no options.
        /// </summary>
        public RemoteServer() : this(null, null) { }

        /// <summary>
        /// Makes a RemoteServer with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public RemoteServer(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("disabled"))
            {
                this.attributes.Add("disabled", false);
            }
            if (!this.attributes.ContainsKey("authentication_method"))
            {
                this.attributes.Add("authentication_method", null);
            }
            if (!this.attributes.ContainsKey("hostname"))
            {
                this.attributes.Add("hostname", null);
            }
            if (!this.attributes.ContainsKey("remote_home_path"))
            {
                this.attributes.Add("remote_home_path", null);
            }
            if (!this.attributes.ContainsKey("upload_staging_path"))
            {
                this.attributes.Add("upload_staging_path", null);
            }
            if (!this.attributes.ContainsKey("allow_relative_paths"))
            {
                this.attributes.Add("allow_relative_paths", false);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("port"))
            {
                this.attributes.Add("port", null);
            }
            if (!this.attributes.ContainsKey("buffer_uploads"))
            {
                this.attributes.Add("buffer_uploads", null);
            }
            if (!this.attributes.ContainsKey("max_connections"))
            {
                this.attributes.Add("max_connections", null);
            }
            if (!this.attributes.ContainsKey("pin_to_site_region"))
            {
                this.attributes.Add("pin_to_site_region", false);
            }
            if (!this.attributes.ContainsKey("pinned_region"))
            {
                this.attributes.Add("pinned_region", null);
            }
            if (!this.attributes.ContainsKey("remote_server_credential_id"))
            {
                this.attributes.Add("remote_server_credential_id", null);
            }
            if (!this.attributes.ContainsKey("s3_bucket"))
            {
                this.attributes.Add("s3_bucket", null);
            }
            if (!this.attributes.ContainsKey("s3_region"))
            {
                this.attributes.Add("s3_region", null);
            }
            if (!this.attributes.ContainsKey("s3_kms_key_id"))
            {
                this.attributes.Add("s3_kms_key_id", null);
            }
            if (!this.attributes.ContainsKey("aws_access_key"))
            {
                this.attributes.Add("aws_access_key", null);
            }
            if (!this.attributes.ContainsKey("s3_assume_role_arn"))
            {
                this.attributes.Add("s3_assume_role_arn", null);
            }
            if (!this.attributes.ContainsKey("s3_assume_role_duration_seconds"))
            {
                this.attributes.Add("s3_assume_role_duration_seconds", null);
            }
            if (!this.attributes.ContainsKey("s3_assume_role_external_id"))
            {
                this.attributes.Add("s3_assume_role_external_id", null);
            }
            if (!this.attributes.ContainsKey("server_certificate"))
            {
                this.attributes.Add("server_certificate", null);
            }
            if (!this.attributes.ContainsKey("server_host_key"))
            {
                this.attributes.Add("server_host_key", null);
            }
            if (!this.attributes.ContainsKey("server_type"))
            {
                this.attributes.Add("server_type", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("ssl"))
            {
                this.attributes.Add("ssl", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("google_cloud_storage_bucket"))
            {
                this.attributes.Add("google_cloud_storage_bucket", null);
            }
            if (!this.attributes.ContainsKey("google_cloud_storage_authentication_method"))
            {
                this.attributes.Add("google_cloud_storage_authentication_method", null);
            }
            if (!this.attributes.ContainsKey("google_cloud_storage_oauth_scope"))
            {
                this.attributes.Add("google_cloud_storage_oauth_scope", null);
            }
            if (!this.attributes.ContainsKey("google_cloud_storage_project_id"))
            {
                this.attributes.Add("google_cloud_storage_project_id", null);
            }
            if (!this.attributes.ContainsKey("google_cloud_storage_s3_compatible_access_key"))
            {
                this.attributes.Add("google_cloud_storage_s3_compatible_access_key", null);
            }
            if (!this.attributes.ContainsKey("backblaze_b2_s3_endpoint"))
            {
                this.attributes.Add("backblaze_b2_s3_endpoint", null);
            }
            if (!this.attributes.ContainsKey("backblaze_b2_bucket"))
            {
                this.attributes.Add("backblaze_b2_bucket", null);
            }
            if (!this.attributes.ContainsKey("wasabi_bucket"))
            {
                this.attributes.Add("wasabi_bucket", null);
            }
            if (!this.attributes.ContainsKey("wasabi_region"))
            {
                this.attributes.Add("wasabi_region", null);
            }
            if (!this.attributes.ContainsKey("wasabi_access_key"))
            {
                this.attributes.Add("wasabi_access_key", null);
            }
            if (!this.attributes.ContainsKey("auth_status"))
            {
                this.attributes.Add("auth_status", null);
            }
            if (!this.attributes.ContainsKey("auth_account_name"))
            {
                this.attributes.Add("auth_account_name", null);
            }
            if (!this.attributes.ContainsKey("one_drive_account_type"))
            {
                this.attributes.Add("one_drive_account_type", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_tenant_id"))
            {
                this.attributes.Add("sharepoint_tenant_id", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_client_id"))
            {
                this.attributes.Add("sharepoint_client_id", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_app_authentication"))
            {
                this.attributes.Add("sharepoint_app_authentication", false);
            }
            if (!this.attributes.ContainsKey("sharepoint_app_credential_type"))
            {
                this.attributes.Add("sharepoint_app_credential_type", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_site_url"))
            {
                this.attributes.Add("sharepoint_site_url", null);
            }
            if (!this.attributes.ContainsKey("azure_blob_storage_account"))
            {
                this.attributes.Add("azure_blob_storage_account", null);
            }
            if (!this.attributes.ContainsKey("azure_blob_storage_container"))
            {
                this.attributes.Add("azure_blob_storage_container", null);
            }
            if (!this.attributes.ContainsKey("azure_blob_storage_hierarchical_namespace"))
            {
                this.attributes.Add("azure_blob_storage_hierarchical_namespace", false);
            }
            if (!this.attributes.ContainsKey("azure_blob_storage_dns_suffix"))
            {
                this.attributes.Add("azure_blob_storage_dns_suffix", null);
            }
            if (!this.attributes.ContainsKey("azure_files_storage_account"))
            {
                this.attributes.Add("azure_files_storage_account", null);
            }
            if (!this.attributes.ContainsKey("azure_files_storage_share_name"))
            {
                this.attributes.Add("azure_files_storage_share_name", null);
            }
            if (!this.attributes.ContainsKey("azure_files_storage_dns_suffix"))
            {
                this.attributes.Add("azure_files_storage_dns_suffix", null);
            }
            if (!this.attributes.ContainsKey("s3_compatible_bucket"))
            {
                this.attributes.Add("s3_compatible_bucket", null);
            }
            if (!this.attributes.ContainsKey("s3_compatible_endpoint"))
            {
                this.attributes.Add("s3_compatible_endpoint", null);
            }
            if (!this.attributes.ContainsKey("s3_compatible_region"))
            {
                this.attributes.Add("s3_compatible_region", null);
            }
            if (!this.attributes.ContainsKey("s3_compatible_virtual_hosted_style"))
            {
                this.attributes.Add("s3_compatible_virtual_hosted_style", false);
            }
            if (!this.attributes.ContainsKey("s3_compatible_access_key"))
            {
                this.attributes.Add("s3_compatible_access_key", null);
            }
            if (!this.attributes.ContainsKey("enable_dedicated_ips"))
            {
                this.attributes.Add("enable_dedicated_ips", false);
            }
            if (!this.attributes.ContainsKey("custom_domain_id"))
            {
                this.attributes.Add("custom_domain_id", null);
            }
            if (!this.attributes.ContainsKey("outbound_ip_addresses"))
            {
                this.attributes.Add("outbound_ip_addresses", new string[0]);
            }
            if (!this.attributes.ContainsKey("files_agent_permission_set"))
            {
                this.attributes.Add("files_agent_permission_set", null);
            }
            if (!this.attributes.ContainsKey("files_agent_root"))
            {
                this.attributes.Add("files_agent_root", null);
            }
            if (!this.attributes.ContainsKey("files_agent_version"))
            {
                this.attributes.Add("files_agent_version", null);
            }
            if (!this.attributes.ContainsKey("files_agent_up_to_date"))
            {
                this.attributes.Add("files_agent_up_to_date", false);
            }
            if (!this.attributes.ContainsKey("files_agent_latest_version"))
            {
                this.attributes.Add("files_agent_latest_version", null);
            }
            if (!this.attributes.ContainsKey("files_agent_supports_push_updates"))
            {
                this.attributes.Add("files_agent_supports_push_updates", false);
            }
            if (!this.attributes.ContainsKey("direct_transfer_available"))
            {
                this.attributes.Add("direct_transfer_available", false);
            }
            if (!this.attributes.ContainsKey("outbound_agent_id"))
            {
                this.attributes.Add("outbound_agent_id", null);
            }
            if (!this.attributes.ContainsKey("filebase_bucket"))
            {
                this.attributes.Add("filebase_bucket", null);
            }
            if (!this.attributes.ContainsKey("filebase_access_key"))
            {
                this.attributes.Add("filebase_access_key", null);
            }
            if (!this.attributes.ContainsKey("files_api_key_prefix"))
            {
                this.attributes.Add("files_api_key_prefix", null);
            }
            if (!this.attributes.ContainsKey("cloudflare_bucket"))
            {
                this.attributes.Add("cloudflare_bucket", null);
            }
            if (!this.attributes.ContainsKey("cloudflare_access_key"))
            {
                this.attributes.Add("cloudflare_access_key", null);
            }
            if (!this.attributes.ContainsKey("cloudflare_endpoint"))
            {
                this.attributes.Add("cloudflare_endpoint", null);
            }
            if (!this.attributes.ContainsKey("dropbox_teams"))
            {
                this.attributes.Add("dropbox_teams", false);
            }
            if (!this.attributes.ContainsKey("linode_bucket"))
            {
                this.attributes.Add("linode_bucket", null);
            }
            if (!this.attributes.ContainsKey("linode_access_key"))
            {
                this.attributes.Add("linode_access_key", null);
            }
            if (!this.attributes.ContainsKey("linode_region"))
            {
                this.attributes.Add("linode_region", null);
            }
            if (!this.attributes.ContainsKey("supports_versioning"))
            {
                this.attributes.Add("supports_versioning", false);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("password"))
            {
                this.attributes.Add("password", null);
            }
            if (!this.attributes.ContainsKey("private_key"))
            {
                this.attributes.Add("private_key", null);
            }
            if (!this.attributes.ContainsKey("private_key_passphrase"))
            {
                this.attributes.Add("private_key_passphrase", null);
            }
            if (!this.attributes.ContainsKey("reset_authentication"))
            {
                this.attributes.Add("reset_authentication", false);
            }
            if (!this.attributes.ContainsKey("sharepoint_client_certificate"))
            {
                this.attributes.Add("sharepoint_client_certificate", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_client_secret"))
            {
                this.attributes.Add("sharepoint_client_secret", null);
            }
            if (!this.attributes.ContainsKey("ssl_certificate"))
            {
                this.attributes.Add("ssl_certificate", null);
            }
            if (!this.attributes.ContainsKey("aws_secret_key"))
            {
                this.attributes.Add("aws_secret_key", null);
            }
            if (!this.attributes.ContainsKey("azure_blob_storage_access_key"))
            {
                this.attributes.Add("azure_blob_storage_access_key", null);
            }
            if (!this.attributes.ContainsKey("azure_blob_storage_sas_token"))
            {
                this.attributes.Add("azure_blob_storage_sas_token", null);
            }
            if (!this.attributes.ContainsKey("azure_files_storage_access_key"))
            {
                this.attributes.Add("azure_files_storage_access_key", null);
            }
            if (!this.attributes.ContainsKey("azure_files_storage_sas_token"))
            {
                this.attributes.Add("azure_files_storage_sas_token", null);
            }
            if (!this.attributes.ContainsKey("backblaze_b2_application_key"))
            {
                this.attributes.Add("backblaze_b2_application_key", null);
            }
            if (!this.attributes.ContainsKey("backblaze_b2_key_id"))
            {
                this.attributes.Add("backblaze_b2_key_id", null);
            }
            if (!this.attributes.ContainsKey("cloudflare_secret_key"))
            {
                this.attributes.Add("cloudflare_secret_key", null);
            }
            if (!this.attributes.ContainsKey("filebase_secret_key"))
            {
                this.attributes.Add("filebase_secret_key", null);
            }
            if (!this.attributes.ContainsKey("google_cloud_storage_credentials_json"))
            {
                this.attributes.Add("google_cloud_storage_credentials_json", null);
            }
            if (!this.attributes.ContainsKey("google_cloud_storage_s3_compatible_secret_key"))
            {
                this.attributes.Add("google_cloud_storage_s3_compatible_secret_key", null);
            }
            if (!this.attributes.ContainsKey("linode_secret_key"))
            {
                this.attributes.Add("linode_secret_key", null);
            }
            if (!this.attributes.ContainsKey("s3_compatible_secret_key"))
            {
                this.attributes.Add("s3_compatible_secret_key", null);
            }
            if (!this.attributes.ContainsKey("wasabi_secret_key"))
            {
                this.attributes.Add("wasabi_secret_key", null);
            }
            if (!this.attributes.ContainsKey("files_api_key"))
            {
                this.attributes.Add("files_api_key", null);
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
        /// Remote Server ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// If true, this Remote Server is disabled. Updating it clears this flag, except for retired Agent v1 records, which remain disabled.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; unsaved.Add("disabled"); }
        }

        /// <summary>
        /// Type of authentication method to use
        /// </summary>
        [JsonPropertyName("authentication_method")]
        public string AuthenticationMethod
        {
            get { return (string)attributes["authentication_method"]; }
            set { attributes["authentication_method"] = value; unsaved.Add("authentication_method"); }
        }

        /// <summary>
        /// Hostname or IP address. For Agent Remote Servers, the Agent reports the hostname it connects from at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        /// </summary>
        [JsonPropertyName("hostname")]
        public string Hostname
        {
            get { return (string)attributes["hostname"]; }
            set { attributes["hostname"] = value; unsaved.Add("hostname"); }
        }

        /// <summary>
        /// Initial home folder on remote server
        /// </summary>
        [JsonPropertyName("remote_home_path")]
        public string RemoteHomePath
        {
            get { return (string)attributes["remote_home_path"]; }
            set { attributes["remote_home_path"] = value; unsaved.Add("remote_home_path"); }
        }

        /// <summary>
        /// Upload staging path.  Applies to SFTP only.  If a path is provided here, files will first be uploaded to this path on the remote folder and the moved into the final correct path via an SFTP move command.  This is required by some remote MFT systems to emulate atomic uploads, which are otherwise not supoprted by SFTP.
        /// </summary>
        [JsonPropertyName("upload_staging_path")]
        public string UploadStagingPath
        {
            get { return (string)attributes["upload_staging_path"]; }
            set { attributes["upload_staging_path"] = value; unsaved.Add("upload_staging_path"); }
        }

        /// <summary>
        /// Allow relative paths in SFTP. If true, paths will not be forced to be absolute, allowing operations relative to the user's home directory.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_relative_paths")]
        public bool AllowRelativePaths
        {
            get { return attributes["allow_relative_paths"] == null ? false : (bool)attributes["allow_relative_paths"]; }
            set { attributes["allow_relative_paths"] = value; unsaved.Add("allow_relative_paths"); }
        }

        /// <summary>
        /// Internal name for your reference
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Internal description for your reference
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; unsaved.Add("description"); }
        }

        /// <summary>
        /// Port for remote server. For Agent Remote Servers, the Agent reports its downstream proxy port at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        /// </summary>
        [JsonPropertyName("port")]
        public Nullable<Int64> Port
        {
            get { return (Nullable<Int64>)attributes["port"]; }
            set { attributes["port"] = value; unsaved.Add("port"); }
        }

        /// <summary>
        /// If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
        /// </summary>
        [JsonPropertyName("buffer_uploads")]
        public string BufferUploads
        {
            get { return (string)attributes["buffer_uploads"]; }
            set { attributes["buffer_uploads"] = value; unsaved.Add("buffer_uploads"); }
        }

        /// <summary>
        /// Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        /// </summary>
        [JsonPropertyName("max_connections")]
        public Nullable<Int64> MaxConnections
        {
            get { return (Nullable<Int64>)attributes["max_connections"]; }
            set { attributes["max_connections"] = value; unsaved.Add("max_connections"); }
        }

        /// <summary>
        /// If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("pin_to_site_region")]
        public bool PinToSiteRegion
        {
            get { return attributes["pin_to_site_region"] == null ? false : (bool)attributes["pin_to_site_region"]; }
            set { attributes["pin_to_site_region"] = value; unsaved.Add("pin_to_site_region"); }
        }

        /// <summary>
        /// If set, all communications with this remote server are made through the provided region.
        /// </summary>
        [JsonPropertyName("pinned_region")]
        public string PinnedRegion
        {
            get { return (string)attributes["pinned_region"]; }
            set { attributes["pinned_region"] = value; unsaved.Add("pinned_region"); }
        }

        /// <summary>
        /// ID of Remote Server Credential, if applicable.
        /// </summary>
        [JsonPropertyName("remote_server_credential_id")]
        public Nullable<Int64> RemoteServerCredentialId
        {
            get { return (Nullable<Int64>)attributes["remote_server_credential_id"]; }
            set { attributes["remote_server_credential_id"] = value; unsaved.Add("remote_server_credential_id"); }
        }

        /// <summary>
        /// S3 bucket name
        /// </summary>
        [JsonPropertyName("s3_bucket")]
        public string S3Bucket
        {
            get { return (string)attributes["s3_bucket"]; }
            set { attributes["s3_bucket"] = value; unsaved.Add("s3_bucket"); }
        }

        /// <summary>
        /// S3 region
        /// </summary>
        [JsonPropertyName("s3_region")]
        public string S3Region
        {
            get { return (string)attributes["s3_region"]; }
            set { attributes["s3_region"] = value; unsaved.Add("s3_region"); }
        }

        /// <summary>
        /// ARN of the AWS KMS key that encrypts files written to the bucket (SSE-KMS). Leave blank unless your bucket requires a specific key.
        /// </summary>
        [JsonPropertyName("s3_kms_key_id")]
        public string S3KmsKeyId
        {
            get { return (string)attributes["s3_kms_key_id"]; }
            set { attributes["s3_kms_key_id"] = value; unsaved.Add("s3_kms_key_id"); }
        }

        /// <summary>
        /// AWS Access Key.
        /// </summary>
        [JsonPropertyName("aws_access_key")]
        public string AwsAccessKey
        {
            get { return (string)attributes["aws_access_key"]; }
            set { attributes["aws_access_key"] = value; unsaved.Add("aws_access_key"); }
        }

        /// <summary>
        /// AWS IAM Role ARN for AssumeRole authentication.
        /// </summary>
        [JsonPropertyName("s3_assume_role_arn")]
        public string S3AssumeRoleArn
        {
            get { return (string)attributes["s3_assume_role_arn"]; }
            set { attributes["s3_assume_role_arn"] = value; unsaved.Add("s3_assume_role_arn"); }
        }

        /// <summary>
        /// Session duration in seconds for AssumeRole authentication (900-43200).
        /// </summary>
        [JsonPropertyName("s3_assume_role_duration_seconds")]
        public Nullable<Int64> S3AssumeRoleDurationSeconds
        {
            get { return (Nullable<Int64>)attributes["s3_assume_role_duration_seconds"]; }
            set { attributes["s3_assume_role_duration_seconds"] = value; unsaved.Add("s3_assume_role_duration_seconds"); }
        }

        /// <summary>
        /// External ID for AssumeRole authentication.
        /// </summary>
        [JsonPropertyName("s3_assume_role_external_id")]
        public string S3AssumeRoleExternalId
        {
            get { return (string)attributes["s3_assume_role_external_id"]; }
            set { attributes["s3_assume_role_external_id"] = value; unsaved.Add("s3_assume_role_external_id"); }
        }

        /// <summary>
        /// Remote server certificate
        /// </summary>
        [JsonPropertyName("server_certificate")]
        public string ServerCertificate
        {
            get { return (string)attributes["server_certificate"]; }
            set { attributes["server_certificate"] = value; unsaved.Add("server_certificate"); }
        }

        /// <summary>
        /// Pinned SSH host key or OpenSSH host certificate for SFTP. If omitted, Files.com detects and stores a host key, preferring plain keys over certificates. With `server_certificate=require_match` (the default), the server must present the exact pinned key or certificate and prove it holds the matching private key. A pinned certificate is compared in full, so renewal can require updating `server_host_key` even when its underlying key is unchanged. Files.com does not check certificate CA signatures, principals, or validity periods. Certificate expiration alone does not end the pin. Update `server_host_key` to replace the pin.
        /// </summary>
        [JsonPropertyName("server_host_key")]
        public string ServerHostKey
        {
            get { return (string)attributes["server_host_key"]; }
            set { attributes["server_host_key"] = value; unsaved.Add("server_host_key"); }
        }

        /// <summary>
        /// Remote server type.
        /// </summary>
        [JsonPropertyName("server_type")]
        public string ServerType
        {
            get { return (string)attributes["server_type"]; }
            set { attributes["server_type"] = value; unsaved.Add("server_type"); }
        }

        /// <summary>
        /// Workspace ID (0 for default workspace)
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Should we require SSL?
        /// </summary>
        [JsonPropertyName("ssl")]
        public string Ssl
        {
            get { return (string)attributes["ssl"]; }
            set { attributes["ssl"] = value; unsaved.Add("ssl"); }
        }

        /// <summary>
        /// Remote server username.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; unsaved.Add("username"); }
        }

        /// <summary>
        /// Google Cloud Storage: Bucket Name
        /// </summary>
        [JsonPropertyName("google_cloud_storage_bucket")]
        public string GoogleCloudStorageBucket
        {
            get { return (string)attributes["google_cloud_storage_bucket"]; }
            set { attributes["google_cloud_storage_bucket"] = value; unsaved.Add("google_cloud_storage_bucket"); }
        }

        /// <summary>
        /// Google Cloud Storage: Authentication method. Can be json, hmac, or oauth.
        /// </summary>
        [JsonPropertyName("google_cloud_storage_authentication_method")]
        public string GoogleCloudStorageAuthenticationMethod
        {
            get { return (string)attributes["google_cloud_storage_authentication_method"]; }
            set { attributes["google_cloud_storage_authentication_method"] = value; unsaved.Add("google_cloud_storage_authentication_method"); }
        }

        /// <summary>
        /// Google Cloud Storage: OAuth scope. Can be https://www.googleapis.com/auth/devstorage.read_only or https://www.googleapis.com/auth/devstorage.read_write.
        /// </summary>
        [JsonPropertyName("google_cloud_storage_oauth_scope")]
        public string GoogleCloudStorageOauthScope
        {
            get { return (string)attributes["google_cloud_storage_oauth_scope"]; }
            set { attributes["google_cloud_storage_oauth_scope"] = value; unsaved.Add("google_cloud_storage_oauth_scope"); }
        }

        /// <summary>
        /// Google Cloud Storage: Project ID
        /// </summary>
        [JsonPropertyName("google_cloud_storage_project_id")]
        public string GoogleCloudStorageProjectId
        {
            get { return (string)attributes["google_cloud_storage_project_id"]; }
            set { attributes["google_cloud_storage_project_id"] = value; unsaved.Add("google_cloud_storage_project_id"); }
        }

        /// <summary>
        /// Google Cloud Storage: S3-compatible Access Key.
        /// </summary>
        [JsonPropertyName("google_cloud_storage_s3_compatible_access_key")]
        public string GoogleCloudStorageS3CompatibleAccessKey
        {
            get { return (string)attributes["google_cloud_storage_s3_compatible_access_key"]; }
            set { attributes["google_cloud_storage_s3_compatible_access_key"] = value; unsaved.Add("google_cloud_storage_s3_compatible_access_key"); }
        }

        /// <summary>
        /// Backblaze B2 Cloud Storage: S3 Endpoint
        /// </summary>
        [JsonPropertyName("backblaze_b2_s3_endpoint")]
        public string BackblazeB2S3Endpoint
        {
            get { return (string)attributes["backblaze_b2_s3_endpoint"]; }
            set { attributes["backblaze_b2_s3_endpoint"] = value; unsaved.Add("backblaze_b2_s3_endpoint"); }
        }

        /// <summary>
        /// Backblaze B2 Cloud Storage: Bucket name
        /// </summary>
        [JsonPropertyName("backblaze_b2_bucket")]
        public string BackblazeB2Bucket
        {
            get { return (string)attributes["backblaze_b2_bucket"]; }
            set { attributes["backblaze_b2_bucket"] = value; unsaved.Add("backblaze_b2_bucket"); }
        }

        /// <summary>
        /// Wasabi: Bucket name
        /// </summary>
        [JsonPropertyName("wasabi_bucket")]
        public string WasabiBucket
        {
            get { return (string)attributes["wasabi_bucket"]; }
            set { attributes["wasabi_bucket"] = value; unsaved.Add("wasabi_bucket"); }
        }

        /// <summary>
        /// Wasabi: Region
        /// </summary>
        [JsonPropertyName("wasabi_region")]
        public string WasabiRegion
        {
            get { return (string)attributes["wasabi_region"]; }
            set { attributes["wasabi_region"] = value; unsaved.Add("wasabi_region"); }
        }

        /// <summary>
        /// Wasabi: Access Key.
        /// </summary>
        [JsonPropertyName("wasabi_access_key")]
        public string WasabiAccessKey
        {
            get { return (string)attributes["wasabi_access_key"]; }
            set { attributes["wasabi_access_key"] = value; unsaved.Add("wasabi_access_key"); }
        }

        /// <summary>
        /// Either `in_setup` or `complete`
        /// </summary>
        [JsonPropertyName("auth_status")]
        public string AuthStatus
        {
            get { return (string)attributes["auth_status"]; }
            set { attributes["auth_status"] = value; unsaved.Add("auth_status"); }
        }

        /// <summary>
        /// Describes the authorized account
        /// </summary>
        [JsonPropertyName("auth_account_name")]
        public string AuthAccountName
        {
            get { return (string)attributes["auth_account_name"]; }
            set { attributes["auth_account_name"] = value; unsaved.Add("auth_account_name"); }
        }

        /// <summary>
        /// OneDrive: Either personal or business_other account types
        /// </summary>
        [JsonPropertyName("one_drive_account_type")]
        public string OneDriveAccountType
        {
            get { return (string)attributes["one_drive_account_type"]; }
            set { attributes["one_drive_account_type"] = value; unsaved.Add("one_drive_account_type"); }
        }

        /// <summary>
        /// SharePoint: Microsoft Entra tenant ID for app-only authentication.
        /// </summary>
        [JsonPropertyName("sharepoint_tenant_id")]
        public string SharepointTenantId
        {
            get { return (string)attributes["sharepoint_tenant_id"]; }
            set { attributes["sharepoint_tenant_id"] = value; unsaved.Add("sharepoint_tenant_id"); }
        }

        /// <summary>
        /// SharePoint: Microsoft Entra application client ID for app-only authentication.
        /// </summary>
        [JsonPropertyName("sharepoint_client_id")]
        public string SharepointClientId
        {
            get { return (string)attributes["sharepoint_client_id"]; }
            set { attributes["sharepoint_client_id"] = value; unsaved.Add("sharepoint_client_id"); }
        }

        /// <summary>
        /// SharePoint: If true, this remote server uses Microsoft Entra app-only authentication.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sharepoint_app_authentication")]
        public bool SharepointAppAuthentication
        {
            get { return attributes["sharepoint_app_authentication"] == null ? false : (bool)attributes["sharepoint_app_authentication"]; }
            set { attributes["sharepoint_app_authentication"] = value; unsaved.Add("sharepoint_app_authentication"); }
        }

        /// <summary>
        /// SharePoint: App-only credential type. Either secret or certificate.
        /// </summary>
        [JsonPropertyName("sharepoint_app_credential_type")]
        public string SharepointAppCredentialType
        {
            get { return (string)attributes["sharepoint_app_credential_type"]; }
            set { attributes["sharepoint_app_credential_type"] = value; unsaved.Add("sharepoint_app_credential_type"); }
        }

        /// <summary>
        /// SharePoint: Site URL to scope app-only authentication to a single site. Leave blank to browse all sites.
        /// </summary>
        [JsonPropertyName("sharepoint_site_url")]
        public string SharepointSiteUrl
        {
            get { return (string)attributes["sharepoint_site_url"]; }
            set { attributes["sharepoint_site_url"] = value; unsaved.Add("sharepoint_site_url"); }
        }

        /// <summary>
        /// Azure Blob Storage: Account name
        /// </summary>
        [JsonPropertyName("azure_blob_storage_account")]
        public string AzureBlobStorageAccount
        {
            get { return (string)attributes["azure_blob_storage_account"]; }
            set { attributes["azure_blob_storage_account"] = value; unsaved.Add("azure_blob_storage_account"); }
        }

        /// <summary>
        /// Azure Blob Storage: Container name
        /// </summary>
        [JsonPropertyName("azure_blob_storage_container")]
        public string AzureBlobStorageContainer
        {
            get { return (string)attributes["azure_blob_storage_container"]; }
            set { attributes["azure_blob_storage_container"] = value; unsaved.Add("azure_blob_storage_container"); }
        }

        /// <summary>
        /// Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("azure_blob_storage_hierarchical_namespace")]
        public bool AzureBlobStorageHierarchicalNamespace
        {
            get { return attributes["azure_blob_storage_hierarchical_namespace"] == null ? false : (bool)attributes["azure_blob_storage_hierarchical_namespace"]; }
            set { attributes["azure_blob_storage_hierarchical_namespace"] = value; unsaved.Add("azure_blob_storage_hierarchical_namespace"); }
        }

        /// <summary>
        /// Azure Blob Storage: Custom DNS suffix
        /// </summary>
        [JsonPropertyName("azure_blob_storage_dns_suffix")]
        public string AzureBlobStorageDnsSuffix
        {
            get { return (string)attributes["azure_blob_storage_dns_suffix"]; }
            set { attributes["azure_blob_storage_dns_suffix"] = value; unsaved.Add("azure_blob_storage_dns_suffix"); }
        }

        /// <summary>
        /// Azure Files: Storage Account name
        /// </summary>
        [JsonPropertyName("azure_files_storage_account")]
        public string AzureFilesStorageAccount
        {
            get { return (string)attributes["azure_files_storage_account"]; }
            set { attributes["azure_files_storage_account"] = value; unsaved.Add("azure_files_storage_account"); }
        }

        /// <summary>
        /// Azure Files:  Storage Share name
        /// </summary>
        [JsonPropertyName("azure_files_storage_share_name")]
        public string AzureFilesStorageShareName
        {
            get { return (string)attributes["azure_files_storage_share_name"]; }
            set { attributes["azure_files_storage_share_name"] = value; unsaved.Add("azure_files_storage_share_name"); }
        }

        /// <summary>
        /// Azure Files: Custom DNS suffix
        /// </summary>
        [JsonPropertyName("azure_files_storage_dns_suffix")]
        public string AzureFilesStorageDnsSuffix
        {
            get { return (string)attributes["azure_files_storage_dns_suffix"]; }
            set { attributes["azure_files_storage_dns_suffix"] = value; unsaved.Add("azure_files_storage_dns_suffix"); }
        }

        /// <summary>
        /// S3-compatible: Bucket name
        /// </summary>
        [JsonPropertyName("s3_compatible_bucket")]
        public string S3CompatibleBucket
        {
            get { return (string)attributes["s3_compatible_bucket"]; }
            set { attributes["s3_compatible_bucket"] = value; unsaved.Add("s3_compatible_bucket"); }
        }

        /// <summary>
        /// S3-compatible: endpoint
        /// </summary>
        [JsonPropertyName("s3_compatible_endpoint")]
        public string S3CompatibleEndpoint
        {
            get { return (string)attributes["s3_compatible_endpoint"]; }
            set { attributes["s3_compatible_endpoint"] = value; unsaved.Add("s3_compatible_endpoint"); }
        }

        /// <summary>
        /// S3-compatible: region
        /// </summary>
        [JsonPropertyName("s3_compatible_region")]
        public string S3CompatibleRegion
        {
            get { return (string)attributes["s3_compatible_region"]; }
            set { attributes["s3_compatible_region"] = value; unsaved.Add("s3_compatible_region"); }
        }

        /// <summary>
        /// S3-compatible: If true, use virtual-hosted-style URLs instead of path-style URLs
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("s3_compatible_virtual_hosted_style")]
        public bool S3CompatibleVirtualHostedStyle
        {
            get { return attributes["s3_compatible_virtual_hosted_style"] == null ? false : (bool)attributes["s3_compatible_virtual_hosted_style"]; }
            set { attributes["s3_compatible_virtual_hosted_style"] = value; unsaved.Add("s3_compatible_virtual_hosted_style"); }
        }

        /// <summary>
        /// S3-compatible: Access Key
        /// </summary>
        [JsonPropertyName("s3_compatible_access_key")]
        public string S3CompatibleAccessKey
        {
            get { return (string)attributes["s3_compatible_access_key"]; }
            set { attributes["s3_compatible_access_key"] = value; unsaved.Add("s3_compatible_access_key"); }
        }

        /// <summary>
        /// `true` if remote server only accepts connections from dedicated IPs
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("enable_dedicated_ips")]
        public bool EnableDedicatedIps
        {
            get { return attributes["enable_dedicated_ips"] == null ? false : (bool)attributes["enable_dedicated_ips"]; }
            set { attributes["enable_dedicated_ips"] = value; unsaved.Add("enable_dedicated_ips"); }
        }

        /// <summary>
        /// Custom Domain ID whose dedicated IP addresses are selected when this Remote Server uses dedicated IPs. Must be available to this Remote Server's workspace. Requires enable_dedicated_ips and cannot be combined with an outbound Agent. Set to null to use the site's default dedicated IPs.
        /// </summary>
        [JsonPropertyName("custom_domain_id")]
        public Nullable<Int64> CustomDomainId
        {
            get { return (Nullable<Int64>)attributes["custom_domain_id"]; }
            set { attributes["custom_domain_id"] = value; unsaved.Add("custom_domain_id"); }
        }

        /// <summary>
        /// Current eligible public IP addresses for the selected Custom Domain. Any address in this list may originate a connection. Empty when no domain is selected or its configuration is unavailable. Only included in responses for a single Remote Server.
        /// </summary>
        [JsonPropertyName("outbound_ip_addresses")]
        public string[] OutboundIpAddresses
        {
            get { return (string[])attributes["outbound_ip_addresses"]; }
            set { attributes["outbound_ip_addresses"] = value; unsaved.Add("outbound_ip_addresses"); }
        }

        /// <summary>
        /// Agent file permissions: `read_only`, `write_only`, or `read_write`. Files.com writes this value as `permission_set` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the permissions in its local configuration file, controlled by the host's system administrator, and reports them at registration, replacing the stored value.
        /// </summary>
        [JsonPropertyName("files_agent_permission_set")]
        public string FilesAgentPermissionSet
        {
            get { return (string)attributes["files_agent_permission_set"]; }
            set { attributes["files_agent_permission_set"] = value; unsaved.Add("files_agent_permission_set"); }
        }

        /// <summary>
        /// Agent local root path. Files.com writes this value as `root` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the root in its local configuration file, controlled by the host's system administrator, and reports it at registration, replacing the stored value.
        /// </summary>
        [JsonPropertyName("files_agent_root")]
        public string FilesAgentRoot
        {
            get { return (string)attributes["files_agent_root"]; }
            set { attributes["files_agent_root"] = value; unsaved.Add("files_agent_root"); }
        }

        /// <summary>
        /// Files.com Agent version reported by the Agent at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not update the running Agent.
        /// </summary>
        [JsonPropertyName("files_agent_version")]
        public string FilesAgentVersion
        {
            get { return (string)attributes["files_agent_version"]; }
            set { attributes["files_agent_version"] = value; unsaved.Add("files_agent_version"); }
        }

        /// <summary>
        /// If true, the Files Agent is up to date.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("files_agent_up_to_date")]
        public bool FilesAgentUpToDate
        {
            get { return attributes["files_agent_up_to_date"] == null ? false : (bool)attributes["files_agent_up_to_date"]; }
            set { attributes["files_agent_up_to_date"] = value; unsaved.Add("files_agent_up_to_date"); }
        }

        /// <summary>
        /// Latest available Files Agent version
        /// </summary>
        [JsonPropertyName("files_agent_latest_version")]
        public string FilesAgentLatestVersion
        {
            get { return (string)attributes["files_agent_latest_version"]; }
            set { attributes["files_agent_latest_version"] = value; unsaved.Add("files_agent_latest_version"); }
        }

        /// <summary>
        /// Files Agent supports receiving push updates
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("files_agent_supports_push_updates")]
        public bool FilesAgentSupportsPushUpdates
        {
            get { return attributes["files_agent_supports_push_updates"] == null ? false : (bool)attributes["files_agent_supports_push_updates"]; }
            set { attributes["files_agent_supports_push_updates"] = value; unsaved.Add("files_agent_supports_push_updates"); }
        }

        /// <summary>
        /// Whether the Files Agent Proxy recently validated a direct transfer connection. `true` means a direct connection was recently validated (actual availability can vary by client network), `false` means direct transfers are enabled but not currently available, and `null` means direct transfers are disabled or unsupported. Only provided for a connected Files Agent when showing a single Remote Server.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("direct_transfer_available")]
        public bool DirectTransferAvailable
        {
            get { return attributes["direct_transfer_available"] == null ? false : (bool)attributes["direct_transfer_available"]; }
            set { attributes["direct_transfer_available"] = value; unsaved.Add("direct_transfer_available"); }
        }

        /// <summary>
        /// Route traffic to outbound on a files-agent
        /// </summary>
        [JsonPropertyName("outbound_agent_id")]
        public Nullable<Int64> OutboundAgentId
        {
            get { return (Nullable<Int64>)attributes["outbound_agent_id"]; }
            set { attributes["outbound_agent_id"] = value; unsaved.Add("outbound_agent_id"); }
        }

        /// <summary>
        /// Filebase: Bucket name
        /// </summary>
        [JsonPropertyName("filebase_bucket")]
        public string FilebaseBucket
        {
            get { return (string)attributes["filebase_bucket"]; }
            set { attributes["filebase_bucket"] = value; unsaved.Add("filebase_bucket"); }
        }

        /// <summary>
        /// Filebase: Access Key.
        /// </summary>
        [JsonPropertyName("filebase_access_key")]
        public string FilebaseAccessKey
        {
            get { return (string)attributes["filebase_access_key"]; }
            set { attributes["filebase_access_key"] = value; unsaved.Add("filebase_access_key"); }
        }

        /// <summary>
        /// Files.com direct link: paired API key prefix.
        /// </summary>
        [JsonPropertyName("files_api_key_prefix")]
        public string FilesApiKeyPrefix
        {
            get { return (string)attributes["files_api_key_prefix"]; }
            set { attributes["files_api_key_prefix"] = value; unsaved.Add("files_api_key_prefix"); }
        }

        /// <summary>
        /// Cloudflare: Bucket name
        /// </summary>
        [JsonPropertyName("cloudflare_bucket")]
        public string CloudflareBucket
        {
            get { return (string)attributes["cloudflare_bucket"]; }
            set { attributes["cloudflare_bucket"] = value; unsaved.Add("cloudflare_bucket"); }
        }

        /// <summary>
        /// Cloudflare: Access Key.
        /// </summary>
        [JsonPropertyName("cloudflare_access_key")]
        public string CloudflareAccessKey
        {
            get { return (string)attributes["cloudflare_access_key"]; }
            set { attributes["cloudflare_access_key"] = value; unsaved.Add("cloudflare_access_key"); }
        }

        /// <summary>
        /// Cloudflare: endpoint
        /// </summary>
        [JsonPropertyName("cloudflare_endpoint")]
        public string CloudflareEndpoint
        {
            get { return (string)attributes["cloudflare_endpoint"]; }
            set { attributes["cloudflare_endpoint"] = value; unsaved.Add("cloudflare_endpoint"); }
        }

        /// <summary>
        /// Dropbox: If true, list Team folders in root?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dropbox_teams")]
        public bool DropboxTeams
        {
            get { return attributes["dropbox_teams"] == null ? false : (bool)attributes["dropbox_teams"]; }
            set { attributes["dropbox_teams"] = value; unsaved.Add("dropbox_teams"); }
        }

        /// <summary>
        /// Linode: Bucket name
        /// </summary>
        [JsonPropertyName("linode_bucket")]
        public string LinodeBucket
        {
            get { return (string)attributes["linode_bucket"]; }
            set { attributes["linode_bucket"] = value; unsaved.Add("linode_bucket"); }
        }

        /// <summary>
        /// Linode: Access Key
        /// </summary>
        [JsonPropertyName("linode_access_key")]
        public string LinodeAccessKey
        {
            get { return (string)attributes["linode_access_key"]; }
            set { attributes["linode_access_key"] = value; unsaved.Add("linode_access_key"); }
        }

        /// <summary>
        /// Linode: region
        /// </summary>
        [JsonPropertyName("linode_region")]
        public string LinodeRegion
        {
            get { return (string)attributes["linode_region"]; }
            set { attributes["linode_region"] = value; unsaved.Add("linode_region"); }
        }

        /// <summary>
        /// If true, this remote server supports file versioning. This value is determined automatically by Files.com.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("supports_versioning")]
        public bool SupportsVersioning
        {
            get { return attributes["supports_versioning"] == null ? false : (bool)attributes["supports_versioning"]; }
            set { attributes["supports_versioning"] = value; unsaved.Add("supports_versioning"); }
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
        /// Password, if needed.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string)attributes["password"]; }
            set { attributes["password"] = value; unsaved.Add("password"); }
        }

        /// <summary>
        /// Private key, if needed.
        /// </summary>
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string)attributes["private_key"]; }
            set { attributes["private_key"] = value; unsaved.Add("private_key"); }
        }

        /// <summary>
        /// Passphrase for private key if needed.
        /// </summary>
        [JsonPropertyName("private_key_passphrase")]
        public string PrivateKeyPassphrase
        {
            get { return (string)attributes["private_key_passphrase"]; }
            set { attributes["private_key_passphrase"] = value; unsaved.Add("private_key_passphrase"); }
        }

        /// <summary>
        /// Reset authenticated account?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("reset_authentication")]
        public bool ResetAuthentication
        {
            get { return attributes["reset_authentication"] == null ? false : (bool)attributes["reset_authentication"]; }
            set { attributes["reset_authentication"] = value; unsaved.Add("reset_authentication"); }
        }

        /// <summary>
        /// SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        /// </summary>
        [JsonPropertyName("sharepoint_client_certificate")]
        public string SharepointClientCertificate
        {
            get { return (string)attributes["sharepoint_client_certificate"]; }
            set { attributes["sharepoint_client_certificate"] = value; unsaved.Add("sharepoint_client_certificate"); }
        }

        /// <summary>
        /// SharePoint: Microsoft Entra application client secret for app-only authentication.
        /// </summary>
        [JsonPropertyName("sharepoint_client_secret")]
        public string SharepointClientSecret
        {
            get { return (string)attributes["sharepoint_client_secret"]; }
            set { attributes["sharepoint_client_secret"] = value; unsaved.Add("sharepoint_client_secret"); }
        }

        /// <summary>
        /// SSL client certificate.
        /// </summary>
        [JsonPropertyName("ssl_certificate")]
        public string SslCertificate
        {
            get { return (string)attributes["ssl_certificate"]; }
            set { attributes["ssl_certificate"] = value; unsaved.Add("ssl_certificate"); }
        }

        /// <summary>
        /// AWS: secret key.
        /// </summary>
        [JsonPropertyName("aws_secret_key")]
        public string AwsSecretKey
        {
            get { return (string)attributes["aws_secret_key"]; }
            set { attributes["aws_secret_key"] = value; unsaved.Add("aws_secret_key"); }
        }

        /// <summary>
        /// Azure Blob Storage: Access Key
        /// </summary>
        [JsonPropertyName("azure_blob_storage_access_key")]
        public string AzureBlobStorageAccessKey
        {
            get { return (string)attributes["azure_blob_storage_access_key"]; }
            set { attributes["azure_blob_storage_access_key"] = value; unsaved.Add("azure_blob_storage_access_key"); }
        }

        /// <summary>
        /// Azure Blob Storage: Shared Access Signature (SAS) token
        /// </summary>
        [JsonPropertyName("azure_blob_storage_sas_token")]
        public string AzureBlobStorageSasToken
        {
            get { return (string)attributes["azure_blob_storage_sas_token"]; }
            set { attributes["azure_blob_storage_sas_token"] = value; unsaved.Add("azure_blob_storage_sas_token"); }
        }

        /// <summary>
        /// Azure File Storage: Access Key
        /// </summary>
        [JsonPropertyName("azure_files_storage_access_key")]
        public string AzureFilesStorageAccessKey
        {
            get { return (string)attributes["azure_files_storage_access_key"]; }
            set { attributes["azure_files_storage_access_key"] = value; unsaved.Add("azure_files_storage_access_key"); }
        }

        /// <summary>
        /// Azure File Storage: Shared Access Signature (SAS) token
        /// </summary>
        [JsonPropertyName("azure_files_storage_sas_token")]
        public string AzureFilesStorageSasToken
        {
            get { return (string)attributes["azure_files_storage_sas_token"]; }
            set { attributes["azure_files_storage_sas_token"] = value; unsaved.Add("azure_files_storage_sas_token"); }
        }

        /// <summary>
        /// Backblaze B2 Cloud Storage: applicationKey
        /// </summary>
        [JsonPropertyName("backblaze_b2_application_key")]
        public string BackblazeB2ApplicationKey
        {
            get { return (string)attributes["backblaze_b2_application_key"]; }
            set { attributes["backblaze_b2_application_key"] = value; unsaved.Add("backblaze_b2_application_key"); }
        }

        /// <summary>
        /// Backblaze B2 Cloud Storage: keyID
        /// </summary>
        [JsonPropertyName("backblaze_b2_key_id")]
        public string BackblazeB2KeyId
        {
            get { return (string)attributes["backblaze_b2_key_id"]; }
            set { attributes["backblaze_b2_key_id"] = value; unsaved.Add("backblaze_b2_key_id"); }
        }

        /// <summary>
        /// Cloudflare: Secret Key
        /// </summary>
        [JsonPropertyName("cloudflare_secret_key")]
        public string CloudflareSecretKey
        {
            get { return (string)attributes["cloudflare_secret_key"]; }
            set { attributes["cloudflare_secret_key"] = value; unsaved.Add("cloudflare_secret_key"); }
        }

        /// <summary>
        /// Filebase: Secret Key
        /// </summary>
        [JsonPropertyName("filebase_secret_key")]
        public string FilebaseSecretKey
        {
            get { return (string)attributes["filebase_secret_key"]; }
            set { attributes["filebase_secret_key"] = value; unsaved.Add("filebase_secret_key"); }
        }

        /// <summary>
        /// Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
        /// </summary>
        [JsonPropertyName("google_cloud_storage_credentials_json")]
        public string GoogleCloudStorageCredentialsJson
        {
            get { return (string)attributes["google_cloud_storage_credentials_json"]; }
            set { attributes["google_cloud_storage_credentials_json"] = value; unsaved.Add("google_cloud_storage_credentials_json"); }
        }

        /// <summary>
        /// Google Cloud Storage: S3-compatible secret key
        /// </summary>
        [JsonPropertyName("google_cloud_storage_s3_compatible_secret_key")]
        public string GoogleCloudStorageS3CompatibleSecretKey
        {
            get { return (string)attributes["google_cloud_storage_s3_compatible_secret_key"]; }
            set { attributes["google_cloud_storage_s3_compatible_secret_key"] = value; unsaved.Add("google_cloud_storage_s3_compatible_secret_key"); }
        }

        /// <summary>
        /// Linode: Secret Key
        /// </summary>
        [JsonPropertyName("linode_secret_key")]
        public string LinodeSecretKey
        {
            get { return (string)attributes["linode_secret_key"]; }
            set { attributes["linode_secret_key"] = value; unsaved.Add("linode_secret_key"); }
        }

        /// <summary>
        /// S3-compatible: Secret Key
        /// </summary>
        [JsonPropertyName("s3_compatible_secret_key")]
        public string S3CompatibleSecretKey
        {
            get { return (string)attributes["s3_compatible_secret_key"]; }
            set { attributes["s3_compatible_secret_key"] = value; unsaved.Add("s3_compatible_secret_key"); }
        }

        /// <summary>
        /// Wasabi: Secret Key
        /// </summary>
        [JsonPropertyName("wasabi_secret_key")]
        public string WasabiSecretKey
        {
            get { return (string)attributes["wasabi_secret_key"]; }
            set { attributes["wasabi_secret_key"] = value; unsaved.Add("wasabi_secret_key"); }
        }

        /// <summary>
        /// Files.com direct link: API key used once to pair the remote server.
        /// </summary>
        [JsonPropertyName("files_api_key")]
        public string FilesApiKey
        {
            get { return (string)attributes["files_api_key"]; }
            set { attributes["files_api_key"] = value; unsaved.Add("files_api_key"); }
        }

        /// <summary>
        /// List Files.com Agent nodes
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task<AgentNode> AgentNodes(Dictionary<string, object> parameters)
        {
            return AgentNodesCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// List Files.com Agent nodes
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<AgentNode> AgentNodesAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return AgentNodesCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<AgentNode> AgentNodesCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteServer has no id, which AgentNodes needs", "id");
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
            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers/{System.Uri.EscapeDataString(attributes["id"].ToString())}/agent_nodes", System.Net.Http.HttpMethod.Get, parameters, requestOptions, cancellationToken);
            AgentNode result = JsonUtil.DeserializeModelResponse<AgentNode>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Push update to Files Agent
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task<AgentPushUpdate> AgentPushUpdate(Dictionary<string, object> parameters)
        {
            return AgentPushUpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Push update to Files Agent
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<AgentPushUpdate> AgentPushUpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return AgentPushUpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<AgentPushUpdate> AgentPushUpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteServer has no id, which AgentPushUpdate needs", "id");
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
            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers/{System.Uri.EscapeDataString(attributes["id"].ToString())}/agent_push_update", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            AgentPushUpdate result = JsonUtil.DeserializeModelResponse<AgentPushUpdate>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Parameters:
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
        ///   reset_authentication - boolean - Reset authenticated account?
        ///   sharepoint_client_certificate - string - SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        ///   sharepoint_client_secret - string - SharePoint: Microsoft Entra application client secret for app-only authentication.
        ///   ssl_certificate - string - SSL client certificate.
        ///   aws_secret_key - string - AWS: secret key.
        ///   azure_blob_storage_access_key - string - Azure Blob Storage: Access Key
        ///   azure_blob_storage_sas_token - string - Azure Blob Storage: Shared Access Signature (SAS) token
        ///   azure_files_storage_access_key - string - Azure File Storage: Access Key
        ///   azure_files_storage_sas_token - string - Azure File Storage: Shared Access Signature (SAS) token
        ///   backblaze_b2_application_key - string - Backblaze B2 Cloud Storage: applicationKey
        ///   backblaze_b2_key_id - string - Backblaze B2 Cloud Storage: keyID
        ///   cloudflare_secret_key - string - Cloudflare: Secret Key
        ///   filebase_secret_key - string - Filebase: Secret Key
        ///   google_cloud_storage_credentials_json - string - Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
        ///   google_cloud_storage_s3_compatible_secret_key - string - Google Cloud Storage: S3-compatible secret key
        ///   linode_secret_key - string - Linode: Secret Key
        ///   s3_compatible_secret_key - string - S3-compatible: Secret Key
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        ///   allow_relative_paths - boolean - Allow relative paths in SFTP. If true, paths will not be forced to be absolute, allowing operations relative to the user's home directory.
        ///   aws_access_key - string - AWS Access Key.
        ///   azure_blob_storage_account - string - Azure Blob Storage: Account name
        ///   azure_blob_storage_container - string - Azure Blob Storage: Container name
        ///   azure_blob_storage_dns_suffix - string - Azure Blob Storage: Custom DNS suffix
        ///   azure_blob_storage_hierarchical_namespace - boolean - Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
        ///   azure_files_storage_account - string - Azure Files: Storage Account name
        ///   azure_files_storage_dns_suffix - string - Azure Files: Custom DNS suffix
        ///   azure_files_storage_share_name - string - Azure Files:  Storage Share name
        ///   backblaze_b2_bucket - string - Backblaze B2 Cloud Storage: Bucket name
        ///   backblaze_b2_s3_endpoint - string - Backblaze B2 Cloud Storage: S3 Endpoint
        ///   buffer_uploads - string - If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   cloudflare_bucket - string - Cloudflare: Bucket name
        ///   cloudflare_endpoint - string - Cloudflare: endpoint
        ///   description - string - Internal description for your reference
        ///   dropbox_teams - boolean - Dropbox: If true, list Team folders in root?
        ///   enable_dedicated_ips - boolean - `true` if remote server only accepts connections from dedicated IPs
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   filebase_bucket - string - Filebase: Bucket name
        ///   files_api_key - string - Files.com direct link: API key used once to pair the remote server.
        ///   files_agent_permission_set - string - Agent file permissions: `read_only`, `write_only`, or `read_write`. Files.com writes this value as `permission_set` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the permissions in its local configuration file, controlled by the host's system administrator, and reports them at registration, replacing the stored value.
        ///   files_agent_root - string - Agent local root path. Files.com writes this value as `root` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the root in its local configuration file, controlled by the host's system administrator, and reports it at registration, replacing the stored value.
        ///   files_agent_version - string - Files.com Agent version reported by the Agent at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not update the running Agent.
        ///   outbound_agent_id - int64 - Route traffic to outbound on a files-agent
        ///   custom_domain_id - int64 - Custom Domain ID whose dedicated IP addresses are selected when this Remote Server uses dedicated IPs. Must be available to this Remote Server's workspace. Requires enable_dedicated_ips and cannot be combined with an outbound Agent. Set to null to use the site's default dedicated IPs.
        ///   google_cloud_storage_authentication_method - string - Google Cloud Storage: Authentication method. Can be json, hmac, or oauth.
        ///   google_cloud_storage_bucket - string - Google Cloud Storage: Bucket Name
        ///   google_cloud_storage_oauth_scope - string - Google Cloud Storage: OAuth scope. Can be https://www.googleapis.com/auth/devstorage.read_only or https://www.googleapis.com/auth/devstorage.read_write.
        ///   google_cloud_storage_project_id - string - Google Cloud Storage: Project ID
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   hostname - string - Hostname or IP address. For Agent Remote Servers, the Agent reports the hostname it connects from at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        ///   linode_access_key - string - Linode: Access Key
        ///   linode_bucket - string - Linode: Bucket name
        ///   linode_region - string - Linode: region
        ///   max_connections - int64 - Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        ///   name - string - Internal name for your reference
        ///   one_drive_account_type - string - OneDrive: Either personal or business_other account types
        ///   pin_to_site_region - boolean - If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        ///   port - int64 - Port for remote server. For Agent Remote Servers, the Agent reports its downstream proxy port at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        ///   upload_staging_path - string - Upload staging path.  Applies to SFTP only.  If a path is provided here, files will first be uploaded to this path on the remote folder and the moved into the final correct path via an SFTP move command.  This is required by some remote MFT systems to emulate atomic uploads, which are otherwise not supoprted by SFTP.
        ///   remote_server_credential_id - int64 - ID of Remote Server Credential, if applicable.
        ///   s3_assume_role_arn - string - AWS IAM Role ARN for AssumeRole authentication.
        ///   s3_assume_role_duration_seconds - int64 - Session duration in seconds for AssumeRole authentication (900-43200).
        ///   s3_bucket - string - S3 bucket name
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   s3_compatible_bucket - string - S3-compatible: Bucket name
        ///   s3_compatible_endpoint - string - S3-compatible: endpoint
        ///   s3_compatible_region - string - S3-compatible: region
        ///   s3_compatible_virtual_hosted_style - boolean - S3-compatible: If true, use virtual-hosted-style URLs instead of path-style URLs
        ///   s3_kms_key_id - string - ARN of the AWS KMS key that encrypts files written to the bucket (SSE-KMS). Leave blank unless your bucket requires a specific key.
        ///   s3_region - string - S3 region
        ///   server_certificate - string - Remote server certificate
        ///   server_host_key - string - Pinned SSH host key or OpenSSH host certificate for SFTP. If omitted, Files.com detects and stores a host key, preferring plain keys over certificates. With `server_certificate=require_match` (the default), the server must present the exact pinned key or certificate and prove it holds the matching private key. A pinned certificate is compared in full, so renewal can require updating `server_host_key` even when its underlying key is unchanged. Files.com does not check certificate CA signatures, principals, or validity periods. Certificate expiration alone does not end the pin. Update `server_host_key` to replace the pin.
        ///   server_type - string - Remote server type.
        ///   sharepoint_client_id - string - SharePoint: Microsoft Entra application client ID for app-only authentication.
        ///   sharepoint_site_url - string - SharePoint: Site URL to scope app-only authentication to a single site. Leave blank to browse all sites.
        ///   sharepoint_tenant_id - string - SharePoint: Microsoft Entra tenant ID for app-only authentication.
        ///   ssl - string - Should we require SSL?
        ///   username - string - Remote server username.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   wasabi_bucket - string - Wasabi: Bucket name
        ///   wasabi_region - string - Wasabi: Region
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The RemoteServer it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<RemoteServer> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
        ///   reset_authentication - boolean - Reset authenticated account?
        ///   sharepoint_client_certificate - string - SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        ///   sharepoint_client_secret - string - SharePoint: Microsoft Entra application client secret for app-only authentication.
        ///   ssl_certificate - string - SSL client certificate.
        ///   aws_secret_key - string - AWS: secret key.
        ///   azure_blob_storage_access_key - string - Azure Blob Storage: Access Key
        ///   azure_blob_storage_sas_token - string - Azure Blob Storage: Shared Access Signature (SAS) token
        ///   azure_files_storage_access_key - string - Azure File Storage: Access Key
        ///   azure_files_storage_sas_token - string - Azure File Storage: Shared Access Signature (SAS) token
        ///   backblaze_b2_application_key - string - Backblaze B2 Cloud Storage: applicationKey
        ///   backblaze_b2_key_id - string - Backblaze B2 Cloud Storage: keyID
        ///   cloudflare_secret_key - string - Cloudflare: Secret Key
        ///   filebase_secret_key - string - Filebase: Secret Key
        ///   google_cloud_storage_credentials_json - string - Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
        ///   google_cloud_storage_s3_compatible_secret_key - string - Google Cloud Storage: S3-compatible secret key
        ///   linode_secret_key - string - Linode: Secret Key
        ///   s3_compatible_secret_key - string - S3-compatible: Secret Key
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        ///   allow_relative_paths - boolean - Allow relative paths in SFTP. If true, paths will not be forced to be absolute, allowing operations relative to the user's home directory.
        ///   aws_access_key - string - AWS Access Key.
        ///   azure_blob_storage_account - string - Azure Blob Storage: Account name
        ///   azure_blob_storage_container - string - Azure Blob Storage: Container name
        ///   azure_blob_storage_dns_suffix - string - Azure Blob Storage: Custom DNS suffix
        ///   azure_blob_storage_hierarchical_namespace - boolean - Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
        ///   azure_files_storage_account - string - Azure Files: Storage Account name
        ///   azure_files_storage_dns_suffix - string - Azure Files: Custom DNS suffix
        ///   azure_files_storage_share_name - string - Azure Files:  Storage Share name
        ///   backblaze_b2_bucket - string - Backblaze B2 Cloud Storage: Bucket name
        ///   backblaze_b2_s3_endpoint - string - Backblaze B2 Cloud Storage: S3 Endpoint
        ///   buffer_uploads - string - If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   cloudflare_bucket - string - Cloudflare: Bucket name
        ///   cloudflare_endpoint - string - Cloudflare: endpoint
        ///   description - string - Internal description for your reference
        ///   dropbox_teams - boolean - Dropbox: If true, list Team folders in root?
        ///   enable_dedicated_ips - boolean - `true` if remote server only accepts connections from dedicated IPs
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   filebase_bucket - string - Filebase: Bucket name
        ///   files_api_key - string - Files.com direct link: API key used once to pair the remote server.
        ///   files_agent_permission_set - string - Agent file permissions: `read_only`, `write_only`, or `read_write`. Files.com writes this value as `permission_set` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the permissions in its local configuration file, controlled by the host's system administrator, and reports them at registration, replacing the stored value.
        ///   files_agent_root - string - Agent local root path. Files.com writes this value as `root` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the root in its local configuration file, controlled by the host's system administrator, and reports it at registration, replacing the stored value.
        ///   files_agent_version - string - Files.com Agent version reported by the Agent at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not update the running Agent.
        ///   outbound_agent_id - int64 - Route traffic to outbound on a files-agent
        ///   custom_domain_id - int64 - Custom Domain ID whose dedicated IP addresses are selected when this Remote Server uses dedicated IPs. Must be available to this Remote Server's workspace. Requires enable_dedicated_ips and cannot be combined with an outbound Agent. Set to null to use the site's default dedicated IPs.
        ///   google_cloud_storage_authentication_method - string - Google Cloud Storage: Authentication method. Can be json, hmac, or oauth.
        ///   google_cloud_storage_bucket - string - Google Cloud Storage: Bucket Name
        ///   google_cloud_storage_oauth_scope - string - Google Cloud Storage: OAuth scope. Can be https://www.googleapis.com/auth/devstorage.read_only or https://www.googleapis.com/auth/devstorage.read_write.
        ///   google_cloud_storage_project_id - string - Google Cloud Storage: Project ID
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   hostname - string - Hostname or IP address. For Agent Remote Servers, the Agent reports the hostname it connects from at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        ///   linode_access_key - string - Linode: Access Key
        ///   linode_bucket - string - Linode: Bucket name
        ///   linode_region - string - Linode: region
        ///   max_connections - int64 - Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        ///   name - string - Internal name for your reference
        ///   one_drive_account_type - string - OneDrive: Either personal or business_other account types
        ///   pin_to_site_region - boolean - If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        ///   port - int64 - Port for remote server. For Agent Remote Servers, the Agent reports its downstream proxy port at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        ///   upload_staging_path - string - Upload staging path.  Applies to SFTP only.  If a path is provided here, files will first be uploaded to this path on the remote folder and the moved into the final correct path via an SFTP move command.  This is required by some remote MFT systems to emulate atomic uploads, which are otherwise not supoprted by SFTP.
        ///   remote_server_credential_id - int64 - ID of Remote Server Credential, if applicable.
        ///   s3_assume_role_arn - string - AWS IAM Role ARN for AssumeRole authentication.
        ///   s3_assume_role_duration_seconds - int64 - Session duration in seconds for AssumeRole authentication (900-43200).
        ///   s3_bucket - string - S3 bucket name
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   s3_compatible_bucket - string - S3-compatible: Bucket name
        ///   s3_compatible_endpoint - string - S3-compatible: endpoint
        ///   s3_compatible_region - string - S3-compatible: region
        ///   s3_compatible_virtual_hosted_style - boolean - S3-compatible: If true, use virtual-hosted-style URLs instead of path-style URLs
        ///   s3_kms_key_id - string - ARN of the AWS KMS key that encrypts files written to the bucket (SSE-KMS). Leave blank unless your bucket requires a specific key.
        ///   s3_region - string - S3 region
        ///   server_certificate - string - Remote server certificate
        ///   server_host_key - string - Pinned SSH host key or OpenSSH host certificate for SFTP. If omitted, Files.com detects and stores a host key, preferring plain keys over certificates. With `server_certificate=require_match` (the default), the server must present the exact pinned key or certificate and prove it holds the matching private key. A pinned certificate is compared in full, so renewal can require updating `server_host_key` even when its underlying key is unchanged. Files.com does not check certificate CA signatures, principals, or validity periods. Certificate expiration alone does not end the pin. Update `server_host_key` to replace the pin.
        ///   server_type - string - Remote server type.
        ///   sharepoint_client_id - string - SharePoint: Microsoft Entra application client ID for app-only authentication.
        ///   sharepoint_site_url - string - SharePoint: Site URL to scope app-only authentication to a single site. Leave blank to browse all sites.
        ///   sharepoint_tenant_id - string - SharePoint: Microsoft Entra tenant ID for app-only authentication.
        ///   ssl - string - Should we require SSL?
        ///   username - string - Remote server username.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   wasabi_bucket - string - Wasabi: Bucket name
        ///   wasabi_region - string - Wasabi: Region
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The RemoteServer it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<RemoteServer> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<RemoteServer> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteServer has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_passphrase", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_passphrase is null, which cannot be sent" : "Bad parameter: private_key_passphrase must be of type string", "private_key_passphrase");
            }
            if (parameters.TryGetValue("reset_authentication", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: reset_authentication is null, which cannot be sent" : "Bad parameter: reset_authentication must be of type bool", "reset_authentication");
            }
            if (parameters.TryGetValue("sharepoint_client_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_certificate is null, which cannot be sent" : "Bad parameter: sharepoint_client_certificate must be of type string", "sharepoint_client_certificate");
            }
            if (parameters.TryGetValue("sharepoint_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_secret is null, which cannot be sent" : "Bad parameter: sharepoint_client_secret must be of type string", "sharepoint_client_secret");
            }
            if (parameters.TryGetValue("ssl_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl_certificate is null, which cannot be sent" : "Bad parameter: ssl_certificate must be of type string", "ssl_certificate");
            }
            if (parameters.TryGetValue("aws_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_secret_key is null, which cannot be sent" : "Bad parameter: aws_secret_key must be of type string", "aws_secret_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_access_key is null, which cannot be sent" : "Bad parameter: azure_blob_storage_access_key must be of type string", "azure_blob_storage_access_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_sas_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_sas_token is null, which cannot be sent" : "Bad parameter: azure_blob_storage_sas_token must be of type string", "azure_blob_storage_sas_token");
            }
            if (parameters.TryGetValue("azure_files_storage_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_access_key is null, which cannot be sent" : "Bad parameter: azure_files_storage_access_key must be of type string", "azure_files_storage_access_key");
            }
            if (parameters.TryGetValue("azure_files_storage_sas_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_sas_token is null, which cannot be sent" : "Bad parameter: azure_files_storage_sas_token must be of type string", "azure_files_storage_sas_token");
            }
            if (parameters.TryGetValue("backblaze_b2_application_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_application_key is null, which cannot be sent" : "Bad parameter: backblaze_b2_application_key must be of type string", "backblaze_b2_application_key");
            }
            if (parameters.TryGetValue("backblaze_b2_key_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_key_id is null, which cannot be sent" : "Bad parameter: backblaze_b2_key_id must be of type string", "backblaze_b2_key_id");
            }
            if (parameters.TryGetValue("cloudflare_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_secret_key is null, which cannot be sent" : "Bad parameter: cloudflare_secret_key must be of type string", "cloudflare_secret_key");
            }
            if (parameters.TryGetValue("filebase_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_secret_key is null, which cannot be sent" : "Bad parameter: filebase_secret_key must be of type string", "filebase_secret_key");
            }
            if (parameters.TryGetValue("google_cloud_storage_credentials_json", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_credentials_json is null, which cannot be sent" : "Bad parameter: google_cloud_storage_credentials_json must be of type string", "google_cloud_storage_credentials_json");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_secret_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_secret_key must be of type string", "google_cloud_storage_s3_compatible_secret_key");
            }
            if (parameters.TryGetValue("linode_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_secret_key is null, which cannot be sent" : "Bad parameter: linode_secret_key must be of type string", "linode_secret_key");
            }
            if (parameters.TryGetValue("s3_compatible_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_secret_key is null, which cannot be sent" : "Bad parameter: s3_compatible_secret_key must be of type string", "s3_compatible_secret_key");
            }
            if (parameters.TryGetValue("wasabi_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_secret_key is null, which cannot be sent" : "Bad parameter: wasabi_secret_key must be of type string", "wasabi_secret_key");
            }
            if (parameters.TryGetValue("allow_relative_paths", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_relative_paths is null, which cannot be sent" : "Bad parameter: allow_relative_paths must be of type bool", "allow_relative_paths");
            }
            if (parameters.TryGetValue("aws_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_access_key is null, which cannot be sent" : "Bad parameter: aws_access_key must be of type string", "aws_access_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_account", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_account is null, which cannot be sent" : "Bad parameter: azure_blob_storage_account must be of type string", "azure_blob_storage_account");
            }
            if (parameters.TryGetValue("azure_blob_storage_container", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_container is null, which cannot be sent" : "Bad parameter: azure_blob_storage_container must be of type string", "azure_blob_storage_container");
            }
            if (parameters.TryGetValue("azure_blob_storage_dns_suffix", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_dns_suffix is null, which cannot be sent" : "Bad parameter: azure_blob_storage_dns_suffix must be of type string", "azure_blob_storage_dns_suffix");
            }
            if (parameters.TryGetValue("azure_blob_storage_hierarchical_namespace", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_hierarchical_namespace is null, which cannot be sent" : "Bad parameter: azure_blob_storage_hierarchical_namespace must be of type bool", "azure_blob_storage_hierarchical_namespace");
            }
            if (parameters.TryGetValue("azure_files_storage_account", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_account is null, which cannot be sent" : "Bad parameter: azure_files_storage_account must be of type string", "azure_files_storage_account");
            }
            if (parameters.TryGetValue("azure_files_storage_dns_suffix", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_dns_suffix is null, which cannot be sent" : "Bad parameter: azure_files_storage_dns_suffix must be of type string", "azure_files_storage_dns_suffix");
            }
            if (parameters.TryGetValue("azure_files_storage_share_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_share_name is null, which cannot be sent" : "Bad parameter: azure_files_storage_share_name must be of type string", "azure_files_storage_share_name");
            }
            if (parameters.TryGetValue("backblaze_b2_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_bucket is null, which cannot be sent" : "Bad parameter: backblaze_b2_bucket must be of type string", "backblaze_b2_bucket");
            }
            if (parameters.TryGetValue("backblaze_b2_s3_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_s3_endpoint is null, which cannot be sent" : "Bad parameter: backblaze_b2_s3_endpoint must be of type string", "backblaze_b2_s3_endpoint");
            }
            if (parameters.TryGetValue("buffer_uploads", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: buffer_uploads is null, which cannot be sent" : "Bad parameter: buffer_uploads must be of type string", "buffer_uploads");
            }
            if (parameters.TryGetValue("cloudflare_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_access_key is null, which cannot be sent" : "Bad parameter: cloudflare_access_key must be of type string", "cloudflare_access_key");
            }
            if (parameters.TryGetValue("cloudflare_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_bucket is null, which cannot be sent" : "Bad parameter: cloudflare_bucket must be of type string", "cloudflare_bucket");
            }
            if (parameters.TryGetValue("cloudflare_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_endpoint is null, which cannot be sent" : "Bad parameter: cloudflare_endpoint must be of type string", "cloudflare_endpoint");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("dropbox_teams", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dropbox_teams is null, which cannot be sent" : "Bad parameter: dropbox_teams must be of type bool", "dropbox_teams");
            }
            if (parameters.TryGetValue("enable_dedicated_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enable_dedicated_ips is null, which cannot be sent" : "Bad parameter: enable_dedicated_ips must be of type bool", "enable_dedicated_ips");
            }
            if (parameters.TryGetValue("filebase_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_access_key is null, which cannot be sent" : "Bad parameter: filebase_access_key must be of type string", "filebase_access_key");
            }
            if (parameters.TryGetValue("filebase_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_bucket is null, which cannot be sent" : "Bad parameter: filebase_bucket must be of type string", "filebase_bucket");
            }
            if (parameters.TryGetValue("files_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_api_key is null, which cannot be sent" : "Bad parameter: files_api_key must be of type string", "files_api_key");
            }
            if (parameters.TryGetValue("files_agent_permission_set", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_permission_set is null, which cannot be sent" : "Bad parameter: files_agent_permission_set must be of type string", "files_agent_permission_set");
            }
            if (parameters.TryGetValue("files_agent_root", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_root is null, which cannot be sent" : "Bad parameter: files_agent_root must be of type string", "files_agent_root");
            }
            if (parameters.TryGetValue("files_agent_version", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_version is null, which cannot be sent" : "Bad parameter: files_agent_version must be of type string", "files_agent_version");
            }
            if (parameters.TryGetValue("outbound_agent_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: outbound_agent_id is null, which cannot be sent" : "Bad parameter: outbound_agent_id must be of type Nullable<Int64>", "outbound_agent_id");
            }
            if (parameters.TryGetValue("custom_domain_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: custom_domain_id is null, which cannot be sent" : "Bad parameter: custom_domain_id must be of type Nullable<Int64>", "custom_domain_id");
            }
            if (parameters.TryGetValue("google_cloud_storage_authentication_method", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_authentication_method is null, which cannot be sent" : "Bad parameter: google_cloud_storage_authentication_method must be of type string", "google_cloud_storage_authentication_method");
            }
            if (parameters.TryGetValue("google_cloud_storage_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_bucket is null, which cannot be sent" : "Bad parameter: google_cloud_storage_bucket must be of type string", "google_cloud_storage_bucket");
            }
            if (parameters.TryGetValue("google_cloud_storage_oauth_scope", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_oauth_scope is null, which cannot be sent" : "Bad parameter: google_cloud_storage_oauth_scope must be of type string", "google_cloud_storage_oauth_scope");
            }
            if (parameters.TryGetValue("google_cloud_storage_project_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_project_id is null, which cannot be sent" : "Bad parameter: google_cloud_storage_project_id must be of type string", "google_cloud_storage_project_id");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "google_cloud_storage_s3_compatible_access_key");
            }
            if (parameters.TryGetValue("hostname", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: hostname is null, which cannot be sent" : "Bad parameter: hostname must be of type string", "hostname");
            }
            if (parameters.TryGetValue("linode_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_access_key is null, which cannot be sent" : "Bad parameter: linode_access_key must be of type string", "linode_access_key");
            }
            if (parameters.TryGetValue("linode_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_bucket is null, which cannot be sent" : "Bad parameter: linode_bucket must be of type string", "linode_bucket");
            }
            if (parameters.TryGetValue("linode_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_region is null, which cannot be sent" : "Bad parameter: linode_region must be of type string", "linode_region");
            }
            if (parameters.TryGetValue("max_connections", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_connections is null, which cannot be sent" : "Bad parameter: max_connections must be of type Nullable<Int64>", "max_connections");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("one_drive_account_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: one_drive_account_type is null, which cannot be sent" : "Bad parameter: one_drive_account_type must be of type string", "one_drive_account_type");
            }
            if (parameters.TryGetValue("pin_to_site_region", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pin_to_site_region is null, which cannot be sent" : "Bad parameter: pin_to_site_region must be of type bool", "pin_to_site_region");
            }
            if (parameters.TryGetValue("port", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: port is null, which cannot be sent" : "Bad parameter: port must be of type Nullable<Int64>", "port");
            }
            if (parameters.TryGetValue("upload_staging_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: upload_staging_path is null, which cannot be sent" : "Bad parameter: upload_staging_path must be of type string", "upload_staging_path");
            }
            if (parameters.TryGetValue("remote_server_credential_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_server_credential_id is null, which cannot be sent" : "Bad parameter: remote_server_credential_id must be of type Nullable<Int64>", "remote_server_credential_id");
            }
            if (parameters.TryGetValue("s3_assume_role_arn", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_arn is null, which cannot be sent" : "Bad parameter: s3_assume_role_arn must be of type string", "s3_assume_role_arn");
            }
            if (parameters.TryGetValue("s3_assume_role_duration_seconds", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_duration_seconds is null, which cannot be sent" : "Bad parameter: s3_assume_role_duration_seconds must be of type Nullable<Int64>", "s3_assume_role_duration_seconds");
            }
            if (parameters.TryGetValue("s3_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_bucket is null, which cannot be sent" : "Bad parameter: s3_bucket must be of type string", "s3_bucket");
            }
            if (parameters.TryGetValue("s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: s3_compatible_access_key must be of type string", "s3_compatible_access_key");
            }
            if (parameters.TryGetValue("s3_compatible_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_bucket is null, which cannot be sent" : "Bad parameter: s3_compatible_bucket must be of type string", "s3_compatible_bucket");
            }
            if (parameters.TryGetValue("s3_compatible_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_endpoint is null, which cannot be sent" : "Bad parameter: s3_compatible_endpoint must be of type string", "s3_compatible_endpoint");
            }
            if (parameters.TryGetValue("s3_compatible_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_region is null, which cannot be sent" : "Bad parameter: s3_compatible_region must be of type string", "s3_compatible_region");
            }
            if (parameters.TryGetValue("s3_compatible_virtual_hosted_style", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_virtual_hosted_style is null, which cannot be sent" : "Bad parameter: s3_compatible_virtual_hosted_style must be of type bool", "s3_compatible_virtual_hosted_style");
            }
            if (parameters.TryGetValue("s3_kms_key_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_kms_key_id is null, which cannot be sent" : "Bad parameter: s3_kms_key_id must be of type string", "s3_kms_key_id");
            }
            if (parameters.TryGetValue("s3_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_region is null, which cannot be sent" : "Bad parameter: s3_region must be of type string", "s3_region");
            }
            if (parameters.TryGetValue("server_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_certificate is null, which cannot be sent" : "Bad parameter: server_certificate must be of type string", "server_certificate");
            }
            if (parameters.TryGetValue("server_host_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_host_key is null, which cannot be sent" : "Bad parameter: server_host_key must be of type string", "server_host_key");
            }
            if (parameters.TryGetValue("server_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_type is null, which cannot be sent" : "Bad parameter: server_type must be of type string", "server_type");
            }
            if (parameters.TryGetValue("sharepoint_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_id is null, which cannot be sent" : "Bad parameter: sharepoint_client_id must be of type string", "sharepoint_client_id");
            }
            if (parameters.TryGetValue("sharepoint_site_url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_site_url is null, which cannot be sent" : "Bad parameter: sharepoint_site_url must be of type string", "sharepoint_site_url");
            }
            if (parameters.TryGetValue("sharepoint_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_tenant_id is null, which cannot be sent" : "Bad parameter: sharepoint_tenant_id must be of type string", "sharepoint_tenant_id");
            }
            if (parameters.TryGetValue("ssl", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl is null, which cannot be sent" : "Bad parameter: ssl must be of type string", "ssl");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("wasabi_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_access_key is null, which cannot be sent" : "Bad parameter: wasabi_access_key must be of type string", "wasabi_access_key");
            }
            if (parameters.TryGetValue("wasabi_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_bucket is null, which cannot be sent" : "Bad parameter: wasabi_bucket must be of type string", "wasabi_bucket");
            }
            if (parameters.TryGetValue("wasabi_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_region is null, which cannot be sent" : "Bad parameter: wasabi_region must be of type string", "wasabi_region");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            RemoteServer updated = JsonUtil.DeserializeModelResponse<RemoteServer>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<RemoteServer>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /remote_servers/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /remote_servers/{id} to the Files.com API.
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
                throw new MissingParameterException("The RemoteServer has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/remote_servers/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "user_id", "password", "private_key", "private_key_passphrase", "reset_authentication", "sharepoint_client_certificate", "sharepoint_client_secret", "ssl_certificate", "aws_secret_key", "azure_blob_storage_access_key", "azure_blob_storage_sas_token", "azure_files_storage_access_key", "azure_files_storage_sas_token", "backblaze_b2_application_key", "backblaze_b2_key_id", "cloudflare_secret_key", "filebase_secret_key", "google_cloud_storage_credentials_json", "google_cloud_storage_s3_compatible_secret_key", "linode_secret_key", "s3_compatible_secret_key", "wasabi_secret_key", "allow_relative_paths", "aws_access_key", "azure_blob_storage_account", "azure_blob_storage_container", "azure_blob_storage_dns_suffix", "azure_blob_storage_hierarchical_namespace", "azure_files_storage_account", "azure_files_storage_dns_suffix", "azure_files_storage_share_name", "backblaze_b2_bucket", "backblaze_b2_s3_endpoint", "buffer_uploads", "cloudflare_access_key", "cloudflare_bucket", "cloudflare_endpoint", "description", "dropbox_teams", "enable_dedicated_ips", "filebase_access_key", "filebase_bucket", "files_api_key", "files_agent_permission_set", "files_agent_root", "files_agent_version", "outbound_agent_id", "custom_domain_id", "google_cloud_storage_authentication_method", "google_cloud_storage_bucket", "google_cloud_storage_oauth_scope", "google_cloud_storage_project_id", "google_cloud_storage_s3_compatible_access_key", "hostname", "linode_access_key", "linode_bucket", "linode_region", "max_connections", "name", "one_drive_account_type", "pin_to_site_region", "port", "upload_staging_path", "remote_server_credential_id", "s3_assume_role_arn", "s3_assume_role_duration_seconds", "s3_bucket", "s3_compatible_access_key", "s3_compatible_bucket", "s3_compatible_endpoint", "s3_compatible_region", "s3_compatible_virtual_hosted_style", "s3_kms_key_id", "s3_region", "server_certificate", "server_host_key", "server_type", "sharepoint_client_id", "sharepoint_site_url", "sharepoint_tenant_id", "ssl", "username", "wasabi_access_key", "wasabi_bucket", "wasabi_region", "workspace_id" };
        private static readonly string[] UpdateParameterNames = { "id", "password", "private_key", "private_key_passphrase", "reset_authentication", "sharepoint_client_certificate", "sharepoint_client_secret", "ssl_certificate", "aws_secret_key", "azure_blob_storage_access_key", "azure_blob_storage_sas_token", "azure_files_storage_access_key", "azure_files_storage_sas_token", "backblaze_b2_application_key", "backblaze_b2_key_id", "cloudflare_secret_key", "filebase_secret_key", "google_cloud_storage_credentials_json", "google_cloud_storage_s3_compatible_secret_key", "linode_secret_key", "s3_compatible_secret_key", "wasabi_secret_key", "allow_relative_paths", "aws_access_key", "azure_blob_storage_account", "azure_blob_storage_container", "azure_blob_storage_dns_suffix", "azure_blob_storage_hierarchical_namespace", "azure_files_storage_account", "azure_files_storage_dns_suffix", "azure_files_storage_share_name", "backblaze_b2_bucket", "backblaze_b2_s3_endpoint", "buffer_uploads", "cloudflare_access_key", "cloudflare_bucket", "cloudflare_endpoint", "description", "dropbox_teams", "enable_dedicated_ips", "filebase_access_key", "filebase_bucket", "files_api_key", "files_agent_permission_set", "files_agent_root", "files_agent_version", "outbound_agent_id", "custom_domain_id", "google_cloud_storage_authentication_method", "google_cloud_storage_bucket", "google_cloud_storage_oauth_scope", "google_cloud_storage_project_id", "google_cloud_storage_s3_compatible_access_key", "hostname", "linode_access_key", "linode_bucket", "linode_region", "max_connections", "name", "one_drive_account_type", "pin_to_site_region", "port", "upload_staging_path", "remote_server_credential_id", "s3_assume_role_arn", "s3_assume_role_duration_seconds", "s3_bucket", "s3_compatible_access_key", "s3_compatible_bucket", "s3_compatible_endpoint", "s3_compatible_region", "s3_compatible_virtual_hosted_style", "s3_kms_key_id", "s3_region", "server_certificate", "server_host_key", "server_type", "sharepoint_client_id", "sharepoint_site_url", "sharepoint_tenant_id", "ssl", "username", "wasabi_access_key", "wasabi_bucket", "wasabi_region" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this RemoteServer on Files.com, or updates it if it has an ID.
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
            RemoteServer created = await RemoteServer.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name`, `server_type`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `name`, `server_type`, `workspace_id`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`. Valid field combinations are `[ server_type, name ]`, `[ workspace_id, name ]`, `[ backblaze_b2_bucket, name ]`, `[ google_cloud_storage_bucket, name ]`, `[ wasabi_bucket, name ]`, `[ s3_bucket, name ]`, `[ azure_blob_storage_container, name ]`, `[ azure_files_storage_share_name, name ]`, `[ s3_compatible_bucket, name ]`, `[ filebase_bucket, name ]`, `[ cloudflare_bucket, name ]`, `[ linode_bucket, name ]`, `[ workspace_id, server_type ]` or `[ workspace_id, server_type, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`. Valid field combinations are `[ backblaze_b2_bucket, name ]`, `[ google_cloud_storage_bucket, name ]`, `[ wasabi_bucket, name ]`, `[ s3_bucket, name ]`, `[ azure_blob_storage_container, name ]`, `[ azure_files_storage_share_name, name ]`, `[ s3_compatible_bucket, name ]`, `[ filebase_bucket, name ]`, `[ cloudflare_bucket, name ]` or `[ linode_bucket, name ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<RemoteServer> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name`, `server_type`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `name`, `server_type`, `workspace_id`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`. Valid field combinations are `[ server_type, name ]`, `[ workspace_id, name ]`, `[ backblaze_b2_bucket, name ]`, `[ google_cloud_storage_bucket, name ]`, `[ wasabi_bucket, name ]`, `[ s3_bucket, name ]`, `[ azure_blob_storage_container, name ]`, `[ azure_files_storage_share_name, name ]`, `[ s3_compatible_bucket, name ]`, `[ filebase_bucket, name ]`, `[ cloudflare_bucket, name ]`, `[ linode_bucket, name ]`, `[ workspace_id, server_type ]` or `[ workspace_id, server_type, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`. Valid field combinations are `[ backblaze_b2_bucket, name ]`, `[ google_cloud_storage_bucket, name ]`, `[ wasabi_bucket, name ]`, `[ s3_bucket, name ]`, `[ azure_blob_storage_container, name ]`, `[ azure_files_storage_share_name, name ]`, `[ s3_compatible_bucket, name ]`, `[ filebase_bucket, name ]`, `[ cloudflare_bucket, name ]` or `[ linode_bucket, name ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<RemoteServer> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<RemoteServer> ListCore(
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

            return new FilesList<RemoteServer>(client, $"/remote_servers", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server ID.
        /// </summary>
        /// <param name="id">Remote Server ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServer> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server ID.
        /// </summary>
        /// <param name="id">Remote Server ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServer> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<RemoteServer> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteServer>(responseJson, context, options);
        }

        /// <summary>
        /// List Files.com Agent nodes
        /// </summary>
        /// <param name="id">Remote Server ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AgentNode> AgentNodes(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return AgentNodesCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<AgentNode> AgentNodesCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}/agent_nodes", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AgentNode>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server ID.
        /// </summary>
        /// <param name="id">Remote Server ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServerConfigurationFile> FindConfigurationFile(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindConfigurationFileCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteServerConfigurationFile> FindConfigurationFileCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}/configuration_file", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteServerConfigurationFile>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
        ///   reset_authentication - boolean - Reset authenticated account?
        ///   sharepoint_client_certificate - string - SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        ///   sharepoint_client_secret - string - SharePoint: Microsoft Entra application client secret for app-only authentication.
        ///   ssl_certificate - string - SSL client certificate.
        ///   aws_secret_key - string - AWS: secret key.
        ///   azure_blob_storage_access_key - string - Azure Blob Storage: Access Key
        ///   azure_blob_storage_sas_token - string - Azure Blob Storage: Shared Access Signature (SAS) token
        ///   azure_files_storage_access_key - string - Azure File Storage: Access Key
        ///   azure_files_storage_sas_token - string - Azure File Storage: Shared Access Signature (SAS) token
        ///   backblaze_b2_application_key - string - Backblaze B2 Cloud Storage: applicationKey
        ///   backblaze_b2_key_id - string - Backblaze B2 Cloud Storage: keyID
        ///   cloudflare_secret_key - string - Cloudflare: Secret Key
        ///   filebase_secret_key - string - Filebase: Secret Key
        ///   google_cloud_storage_credentials_json - string - Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
        ///   google_cloud_storage_s3_compatible_secret_key - string - Google Cloud Storage: S3-compatible secret key
        ///   linode_secret_key - string - Linode: Secret Key
        ///   s3_compatible_secret_key - string - S3-compatible: Secret Key
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        ///   allow_relative_paths - boolean - Allow relative paths in SFTP. If true, paths will not be forced to be absolute, allowing operations relative to the user's home directory.
        ///   aws_access_key - string - AWS Access Key.
        ///   azure_blob_storage_account - string - Azure Blob Storage: Account name
        ///   azure_blob_storage_container - string - Azure Blob Storage: Container name
        ///   azure_blob_storage_dns_suffix - string - Azure Blob Storage: Custom DNS suffix
        ///   azure_blob_storage_hierarchical_namespace - boolean - Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
        ///   azure_files_storage_account - string - Azure Files: Storage Account name
        ///   azure_files_storage_dns_suffix - string - Azure Files: Custom DNS suffix
        ///   azure_files_storage_share_name - string - Azure Files:  Storage Share name
        ///   backblaze_b2_bucket - string - Backblaze B2 Cloud Storage: Bucket name
        ///   backblaze_b2_s3_endpoint - string - Backblaze B2 Cloud Storage: S3 Endpoint
        ///   buffer_uploads - string - If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   cloudflare_bucket - string - Cloudflare: Bucket name
        ///   cloudflare_endpoint - string - Cloudflare: endpoint
        ///   description - string - Internal description for your reference
        ///   dropbox_teams - boolean - Dropbox: If true, list Team folders in root?
        ///   enable_dedicated_ips - boolean - `true` if remote server only accepts connections from dedicated IPs
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   filebase_bucket - string - Filebase: Bucket name
        ///   files_api_key - string - Files.com direct link: API key used once to pair the remote server.
        ///   files_agent_permission_set - string - Agent file permissions: `read_only`, `write_only`, or `read_write`. Files.com writes this value as `permission_set` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the permissions in its local configuration file, controlled by the host's system administrator, and reports them at registration, replacing the stored value.
        ///   files_agent_root - string - Agent local root path. Files.com writes this value as `root` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the root in its local configuration file, controlled by the host's system administrator, and reports it at registration, replacing the stored value.
        ///   files_agent_version - string - Files.com Agent version reported by the Agent at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not update the running Agent.
        ///   outbound_agent_id - int64 - Route traffic to outbound on a files-agent
        ///   custom_domain_id - int64 - Custom Domain ID whose dedicated IP addresses are selected when this Remote Server uses dedicated IPs. Must be available to this Remote Server's workspace. Requires enable_dedicated_ips and cannot be combined with an outbound Agent. Set to null to use the site's default dedicated IPs.
        ///   google_cloud_storage_authentication_method - string - Google Cloud Storage: Authentication method. Can be json, hmac, or oauth.
        ///   google_cloud_storage_bucket - string - Google Cloud Storage: Bucket Name
        ///   google_cloud_storage_oauth_scope - string - Google Cloud Storage: OAuth scope. Can be https://www.googleapis.com/auth/devstorage.read_only or https://www.googleapis.com/auth/devstorage.read_write.
        ///   google_cloud_storage_project_id - string - Google Cloud Storage: Project ID
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   hostname - string - Hostname or IP address. For Agent Remote Servers, the Agent reports the hostname it connects from at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        ///   linode_access_key - string - Linode: Access Key
        ///   linode_bucket - string - Linode: Bucket name
        ///   linode_region - string - Linode: region
        ///   max_connections - int64 - Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        ///   name - string - Internal name for your reference
        ///   one_drive_account_type - string - OneDrive: Either personal or business_other account types
        ///   pin_to_site_region - boolean - If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        ///   port - int64 - Port for remote server. For Agent Remote Servers, the Agent reports its downstream proxy port at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        ///   upload_staging_path - string - Upload staging path.  Applies to SFTP only.  If a path is provided here, files will first be uploaded to this path on the remote folder and the moved into the final correct path via an SFTP move command.  This is required by some remote MFT systems to emulate atomic uploads, which are otherwise not supoprted by SFTP.
        ///   remote_server_credential_id - int64 - ID of Remote Server Credential, if applicable.
        ///   s3_assume_role_arn - string - AWS IAM Role ARN for AssumeRole authentication.
        ///   s3_assume_role_duration_seconds - int64 - Session duration in seconds for AssumeRole authentication (900-43200).
        ///   s3_bucket - string - S3 bucket name
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   s3_compatible_bucket - string - S3-compatible: Bucket name
        ///   s3_compatible_endpoint - string - S3-compatible: endpoint
        ///   s3_compatible_region - string - S3-compatible: region
        ///   s3_compatible_virtual_hosted_style - boolean - S3-compatible: If true, use virtual-hosted-style URLs instead of path-style URLs
        ///   s3_kms_key_id - string - ARN of the AWS KMS key that encrypts files written to the bucket (SSE-KMS). Leave blank unless your bucket requires a specific key.
        ///   s3_region - string - S3 region
        ///   server_certificate - string - Remote server certificate
        ///   server_host_key - string - Pinned SSH host key or OpenSSH host certificate for SFTP. If omitted, Files.com detects and stores a host key, preferring plain keys over certificates. With `server_certificate=require_match` (the default), the server must present the exact pinned key or certificate and prove it holds the matching private key. A pinned certificate is compared in full, so renewal can require updating `server_host_key` even when its underlying key is unchanged. Files.com does not check certificate CA signatures, principals, or validity periods. Certificate expiration alone does not end the pin. Update `server_host_key` to replace the pin.
        ///   server_type - string - Remote server type.
        ///   sharepoint_client_id - string - SharePoint: Microsoft Entra application client ID for app-only authentication.
        ///   sharepoint_site_url - string - SharePoint: Site URL to scope app-only authentication to a single site. Leave blank to browse all sites.
        ///   sharepoint_tenant_id - string - SharePoint: Microsoft Entra tenant ID for app-only authentication.
        ///   ssl - string - Should we require SSL?
        ///   username - string - Remote server username.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   wasabi_bucket - string - Wasabi: Bucket name
        ///   wasabi_region - string - Wasabi: Region
        ///   workspace_id - int64 - Workspace ID (0 for default workspace)
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServer> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteServer> CreateCore(
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
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_passphrase", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_passphrase is null, which cannot be sent" : "Bad parameter: private_key_passphrase must be of type string", "private_key_passphrase");
            }
            if (parameters.TryGetValue("reset_authentication", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: reset_authentication is null, which cannot be sent" : "Bad parameter: reset_authentication must be of type bool", "reset_authentication");
            }
            if (parameters.TryGetValue("sharepoint_client_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_certificate is null, which cannot be sent" : "Bad parameter: sharepoint_client_certificate must be of type string", "sharepoint_client_certificate");
            }
            if (parameters.TryGetValue("sharepoint_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_secret is null, which cannot be sent" : "Bad parameter: sharepoint_client_secret must be of type string", "sharepoint_client_secret");
            }
            if (parameters.TryGetValue("ssl_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl_certificate is null, which cannot be sent" : "Bad parameter: ssl_certificate must be of type string", "ssl_certificate");
            }
            if (parameters.TryGetValue("aws_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_secret_key is null, which cannot be sent" : "Bad parameter: aws_secret_key must be of type string", "aws_secret_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_access_key is null, which cannot be sent" : "Bad parameter: azure_blob_storage_access_key must be of type string", "azure_blob_storage_access_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_sas_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_sas_token is null, which cannot be sent" : "Bad parameter: azure_blob_storage_sas_token must be of type string", "azure_blob_storage_sas_token");
            }
            if (parameters.TryGetValue("azure_files_storage_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_access_key is null, which cannot be sent" : "Bad parameter: azure_files_storage_access_key must be of type string", "azure_files_storage_access_key");
            }
            if (parameters.TryGetValue("azure_files_storage_sas_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_sas_token is null, which cannot be sent" : "Bad parameter: azure_files_storage_sas_token must be of type string", "azure_files_storage_sas_token");
            }
            if (parameters.TryGetValue("backblaze_b2_application_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_application_key is null, which cannot be sent" : "Bad parameter: backblaze_b2_application_key must be of type string", "backblaze_b2_application_key");
            }
            if (parameters.TryGetValue("backblaze_b2_key_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_key_id is null, which cannot be sent" : "Bad parameter: backblaze_b2_key_id must be of type string", "backblaze_b2_key_id");
            }
            if (parameters.TryGetValue("cloudflare_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_secret_key is null, which cannot be sent" : "Bad parameter: cloudflare_secret_key must be of type string", "cloudflare_secret_key");
            }
            if (parameters.TryGetValue("filebase_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_secret_key is null, which cannot be sent" : "Bad parameter: filebase_secret_key must be of type string", "filebase_secret_key");
            }
            if (parameters.TryGetValue("google_cloud_storage_credentials_json", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_credentials_json is null, which cannot be sent" : "Bad parameter: google_cloud_storage_credentials_json must be of type string", "google_cloud_storage_credentials_json");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_secret_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_secret_key must be of type string", "google_cloud_storage_s3_compatible_secret_key");
            }
            if (parameters.TryGetValue("linode_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_secret_key is null, which cannot be sent" : "Bad parameter: linode_secret_key must be of type string", "linode_secret_key");
            }
            if (parameters.TryGetValue("s3_compatible_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_secret_key is null, which cannot be sent" : "Bad parameter: s3_compatible_secret_key must be of type string", "s3_compatible_secret_key");
            }
            if (parameters.TryGetValue("wasabi_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_secret_key is null, which cannot be sent" : "Bad parameter: wasabi_secret_key must be of type string", "wasabi_secret_key");
            }
            if (parameters.TryGetValue("allow_relative_paths", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_relative_paths is null, which cannot be sent" : "Bad parameter: allow_relative_paths must be of type bool", "allow_relative_paths");
            }
            if (parameters.TryGetValue("aws_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_access_key is null, which cannot be sent" : "Bad parameter: aws_access_key must be of type string", "aws_access_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_account", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_account is null, which cannot be sent" : "Bad parameter: azure_blob_storage_account must be of type string", "azure_blob_storage_account");
            }
            if (parameters.TryGetValue("azure_blob_storage_container", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_container is null, which cannot be sent" : "Bad parameter: azure_blob_storage_container must be of type string", "azure_blob_storage_container");
            }
            if (parameters.TryGetValue("azure_blob_storage_dns_suffix", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_dns_suffix is null, which cannot be sent" : "Bad parameter: azure_blob_storage_dns_suffix must be of type string", "azure_blob_storage_dns_suffix");
            }
            if (parameters.TryGetValue("azure_blob_storage_hierarchical_namespace", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_hierarchical_namespace is null, which cannot be sent" : "Bad parameter: azure_blob_storage_hierarchical_namespace must be of type bool", "azure_blob_storage_hierarchical_namespace");
            }
            if (parameters.TryGetValue("azure_files_storage_account", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_account is null, which cannot be sent" : "Bad parameter: azure_files_storage_account must be of type string", "azure_files_storage_account");
            }
            if (parameters.TryGetValue("azure_files_storage_dns_suffix", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_dns_suffix is null, which cannot be sent" : "Bad parameter: azure_files_storage_dns_suffix must be of type string", "azure_files_storage_dns_suffix");
            }
            if (parameters.TryGetValue("azure_files_storage_share_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_share_name is null, which cannot be sent" : "Bad parameter: azure_files_storage_share_name must be of type string", "azure_files_storage_share_name");
            }
            if (parameters.TryGetValue("backblaze_b2_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_bucket is null, which cannot be sent" : "Bad parameter: backblaze_b2_bucket must be of type string", "backblaze_b2_bucket");
            }
            if (parameters.TryGetValue("backblaze_b2_s3_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_s3_endpoint is null, which cannot be sent" : "Bad parameter: backblaze_b2_s3_endpoint must be of type string", "backblaze_b2_s3_endpoint");
            }
            if (parameters.TryGetValue("buffer_uploads", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: buffer_uploads is null, which cannot be sent" : "Bad parameter: buffer_uploads must be of type string", "buffer_uploads");
            }
            if (parameters.TryGetValue("cloudflare_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_access_key is null, which cannot be sent" : "Bad parameter: cloudflare_access_key must be of type string", "cloudflare_access_key");
            }
            if (parameters.TryGetValue("cloudflare_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_bucket is null, which cannot be sent" : "Bad parameter: cloudflare_bucket must be of type string", "cloudflare_bucket");
            }
            if (parameters.TryGetValue("cloudflare_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_endpoint is null, which cannot be sent" : "Bad parameter: cloudflare_endpoint must be of type string", "cloudflare_endpoint");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("dropbox_teams", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dropbox_teams is null, which cannot be sent" : "Bad parameter: dropbox_teams must be of type bool", "dropbox_teams");
            }
            if (parameters.TryGetValue("enable_dedicated_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enable_dedicated_ips is null, which cannot be sent" : "Bad parameter: enable_dedicated_ips must be of type bool", "enable_dedicated_ips");
            }
            if (parameters.TryGetValue("filebase_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_access_key is null, which cannot be sent" : "Bad parameter: filebase_access_key must be of type string", "filebase_access_key");
            }
            if (parameters.TryGetValue("filebase_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_bucket is null, which cannot be sent" : "Bad parameter: filebase_bucket must be of type string", "filebase_bucket");
            }
            if (parameters.TryGetValue("files_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_api_key is null, which cannot be sent" : "Bad parameter: files_api_key must be of type string", "files_api_key");
            }
            if (parameters.TryGetValue("files_agent_permission_set", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_permission_set is null, which cannot be sent" : "Bad parameter: files_agent_permission_set must be of type string", "files_agent_permission_set");
            }
            if (parameters.TryGetValue("files_agent_root", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_root is null, which cannot be sent" : "Bad parameter: files_agent_root must be of type string", "files_agent_root");
            }
            if (parameters.TryGetValue("files_agent_version", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_version is null, which cannot be sent" : "Bad parameter: files_agent_version must be of type string", "files_agent_version");
            }
            if (parameters.TryGetValue("outbound_agent_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: outbound_agent_id is null, which cannot be sent" : "Bad parameter: outbound_agent_id must be of type Nullable<Int64>", "outbound_agent_id");
            }
            if (parameters.TryGetValue("custom_domain_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: custom_domain_id is null, which cannot be sent" : "Bad parameter: custom_domain_id must be of type Nullable<Int64>", "custom_domain_id");
            }
            if (parameters.TryGetValue("google_cloud_storage_authentication_method", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_authentication_method is null, which cannot be sent" : "Bad parameter: google_cloud_storage_authentication_method must be of type string", "google_cloud_storage_authentication_method");
            }
            if (parameters.TryGetValue("google_cloud_storage_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_bucket is null, which cannot be sent" : "Bad parameter: google_cloud_storage_bucket must be of type string", "google_cloud_storage_bucket");
            }
            if (parameters.TryGetValue("google_cloud_storage_oauth_scope", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_oauth_scope is null, which cannot be sent" : "Bad parameter: google_cloud_storage_oauth_scope must be of type string", "google_cloud_storage_oauth_scope");
            }
            if (parameters.TryGetValue("google_cloud_storage_project_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_project_id is null, which cannot be sent" : "Bad parameter: google_cloud_storage_project_id must be of type string", "google_cloud_storage_project_id");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "google_cloud_storage_s3_compatible_access_key");
            }
            if (parameters.TryGetValue("hostname", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: hostname is null, which cannot be sent" : "Bad parameter: hostname must be of type string", "hostname");
            }
            if (parameters.TryGetValue("linode_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_access_key is null, which cannot be sent" : "Bad parameter: linode_access_key must be of type string", "linode_access_key");
            }
            if (parameters.TryGetValue("linode_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_bucket is null, which cannot be sent" : "Bad parameter: linode_bucket must be of type string", "linode_bucket");
            }
            if (parameters.TryGetValue("linode_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_region is null, which cannot be sent" : "Bad parameter: linode_region must be of type string", "linode_region");
            }
            if (parameters.TryGetValue("max_connections", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_connections is null, which cannot be sent" : "Bad parameter: max_connections must be of type Nullable<Int64>", "max_connections");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("one_drive_account_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: one_drive_account_type is null, which cannot be sent" : "Bad parameter: one_drive_account_type must be of type string", "one_drive_account_type");
            }
            if (parameters.TryGetValue("pin_to_site_region", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pin_to_site_region is null, which cannot be sent" : "Bad parameter: pin_to_site_region must be of type bool", "pin_to_site_region");
            }
            if (parameters.TryGetValue("port", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: port is null, which cannot be sent" : "Bad parameter: port must be of type Nullable<Int64>", "port");
            }
            if (parameters.TryGetValue("upload_staging_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: upload_staging_path is null, which cannot be sent" : "Bad parameter: upload_staging_path must be of type string", "upload_staging_path");
            }
            if (parameters.TryGetValue("remote_server_credential_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_server_credential_id is null, which cannot be sent" : "Bad parameter: remote_server_credential_id must be of type Nullable<Int64>", "remote_server_credential_id");
            }
            if (parameters.TryGetValue("s3_assume_role_arn", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_arn is null, which cannot be sent" : "Bad parameter: s3_assume_role_arn must be of type string", "s3_assume_role_arn");
            }
            if (parameters.TryGetValue("s3_assume_role_duration_seconds", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_duration_seconds is null, which cannot be sent" : "Bad parameter: s3_assume_role_duration_seconds must be of type Nullable<Int64>", "s3_assume_role_duration_seconds");
            }
            if (parameters.TryGetValue("s3_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_bucket is null, which cannot be sent" : "Bad parameter: s3_bucket must be of type string", "s3_bucket");
            }
            if (parameters.TryGetValue("s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: s3_compatible_access_key must be of type string", "s3_compatible_access_key");
            }
            if (parameters.TryGetValue("s3_compatible_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_bucket is null, which cannot be sent" : "Bad parameter: s3_compatible_bucket must be of type string", "s3_compatible_bucket");
            }
            if (parameters.TryGetValue("s3_compatible_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_endpoint is null, which cannot be sent" : "Bad parameter: s3_compatible_endpoint must be of type string", "s3_compatible_endpoint");
            }
            if (parameters.TryGetValue("s3_compatible_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_region is null, which cannot be sent" : "Bad parameter: s3_compatible_region must be of type string", "s3_compatible_region");
            }
            if (parameters.TryGetValue("s3_compatible_virtual_hosted_style", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_virtual_hosted_style is null, which cannot be sent" : "Bad parameter: s3_compatible_virtual_hosted_style must be of type bool", "s3_compatible_virtual_hosted_style");
            }
            if (parameters.TryGetValue("s3_kms_key_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_kms_key_id is null, which cannot be sent" : "Bad parameter: s3_kms_key_id must be of type string", "s3_kms_key_id");
            }
            if (parameters.TryGetValue("s3_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_region is null, which cannot be sent" : "Bad parameter: s3_region must be of type string", "s3_region");
            }
            if (parameters.TryGetValue("server_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_certificate is null, which cannot be sent" : "Bad parameter: server_certificate must be of type string", "server_certificate");
            }
            if (parameters.TryGetValue("server_host_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_host_key is null, which cannot be sent" : "Bad parameter: server_host_key must be of type string", "server_host_key");
            }
            if (parameters.TryGetValue("server_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_type is null, which cannot be sent" : "Bad parameter: server_type must be of type string", "server_type");
            }
            if (parameters.TryGetValue("sharepoint_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_id is null, which cannot be sent" : "Bad parameter: sharepoint_client_id must be of type string", "sharepoint_client_id");
            }
            if (parameters.TryGetValue("sharepoint_site_url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_site_url is null, which cannot be sent" : "Bad parameter: sharepoint_site_url must be of type string", "sharepoint_site_url");
            }
            if (parameters.TryGetValue("sharepoint_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_tenant_id is null, which cannot be sent" : "Bad parameter: sharepoint_tenant_id must be of type string", "sharepoint_tenant_id");
            }
            if (parameters.TryGetValue("ssl", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl is null, which cannot be sent" : "Bad parameter: ssl must be of type string", "ssl");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("wasabi_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_access_key is null, which cannot be sent" : "Bad parameter: wasabi_access_key must be of type string", "wasabi_access_key");
            }
            if (parameters.TryGetValue("wasabi_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_bucket is null, which cannot be sent" : "Bad parameter: wasabi_bucket must be of type string", "wasabi_bucket");
            }
            if (parameters.TryGetValue("wasabi_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_region is null, which cannot be sent" : "Bad parameter: wasabi_region must be of type string", "wasabi_region");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteServer>(responseJson, context, options);
        }

        /// <summary>
        /// Push update to Files Agent
        /// </summary>
        /// <param name="id">Remote Server ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AgentPushUpdate> AgentPushUpdate(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return AgentPushUpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<AgentPushUpdate> AgentPushUpdateCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}/agent_push_update", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AgentPushUpdate>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
        ///   reset_authentication - boolean - Reset authenticated account?
        ///   sharepoint_client_certificate - string - SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        ///   sharepoint_client_secret - string - SharePoint: Microsoft Entra application client secret for app-only authentication.
        ///   ssl_certificate - string - SSL client certificate.
        ///   aws_secret_key - string - AWS: secret key.
        ///   azure_blob_storage_access_key - string - Azure Blob Storage: Access Key
        ///   azure_blob_storage_sas_token - string - Azure Blob Storage: Shared Access Signature (SAS) token
        ///   azure_files_storage_access_key - string - Azure File Storage: Access Key
        ///   azure_files_storage_sas_token - string - Azure File Storage: Shared Access Signature (SAS) token
        ///   backblaze_b2_application_key - string - Backblaze B2 Cloud Storage: applicationKey
        ///   backblaze_b2_key_id - string - Backblaze B2 Cloud Storage: keyID
        ///   cloudflare_secret_key - string - Cloudflare: Secret Key
        ///   filebase_secret_key - string - Filebase: Secret Key
        ///   google_cloud_storage_credentials_json - string - Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
        ///   google_cloud_storage_s3_compatible_secret_key - string - Google Cloud Storage: S3-compatible secret key
        ///   linode_secret_key - string - Linode: Secret Key
        ///   s3_compatible_secret_key - string - S3-compatible: Secret Key
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        ///   allow_relative_paths - boolean - Allow relative paths in SFTP. If true, paths will not be forced to be absolute, allowing operations relative to the user's home directory.
        ///   aws_access_key - string - AWS Access Key.
        ///   azure_blob_storage_account - string - Azure Blob Storage: Account name
        ///   azure_blob_storage_container - string - Azure Blob Storage: Container name
        ///   azure_blob_storage_dns_suffix - string - Azure Blob Storage: Custom DNS suffix
        ///   azure_blob_storage_hierarchical_namespace - boolean - Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
        ///   azure_files_storage_account - string - Azure Files: Storage Account name
        ///   azure_files_storage_dns_suffix - string - Azure Files: Custom DNS suffix
        ///   azure_files_storage_share_name - string - Azure Files:  Storage Share name
        ///   backblaze_b2_bucket - string - Backblaze B2 Cloud Storage: Bucket name
        ///   backblaze_b2_s3_endpoint - string - Backblaze B2 Cloud Storage: S3 Endpoint
        ///   buffer_uploads - string - If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   cloudflare_bucket - string - Cloudflare: Bucket name
        ///   cloudflare_endpoint - string - Cloudflare: endpoint
        ///   description - string - Internal description for your reference
        ///   dropbox_teams - boolean - Dropbox: If true, list Team folders in root?
        ///   enable_dedicated_ips - boolean - `true` if remote server only accepts connections from dedicated IPs
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   filebase_bucket - string - Filebase: Bucket name
        ///   files_api_key - string - Files.com direct link: API key used once to pair the remote server.
        ///   files_agent_permission_set - string - Agent file permissions: `read_only`, `write_only`, or `read_write`. Files.com writes this value as `permission_set` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the permissions in its local configuration file, controlled by the host's system administrator, and reports them at registration, replacing the stored value.
        ///   files_agent_root - string - Agent local root path. Files.com writes this value as `root` in the configuration file generated for this Remote Server. Setting it on Create or Update changes the generated file, not a running Agent. The Agent enforces the root in its local configuration file, controlled by the host's system administrator, and reports it at registration, replacing the stored value.
        ///   files_agent_version - string - Files.com Agent version reported by the Agent at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not update the running Agent.
        ///   outbound_agent_id - int64 - Route traffic to outbound on a files-agent
        ///   custom_domain_id - int64 - Custom Domain ID whose dedicated IP addresses are selected when this Remote Server uses dedicated IPs. Must be available to this Remote Server's workspace. Requires enable_dedicated_ips and cannot be combined with an outbound Agent. Set to null to use the site's default dedicated IPs.
        ///   google_cloud_storage_authentication_method - string - Google Cloud Storage: Authentication method. Can be json, hmac, or oauth.
        ///   google_cloud_storage_bucket - string - Google Cloud Storage: Bucket Name
        ///   google_cloud_storage_oauth_scope - string - Google Cloud Storage: OAuth scope. Can be https://www.googleapis.com/auth/devstorage.read_only or https://www.googleapis.com/auth/devstorage.read_write.
        ///   google_cloud_storage_project_id - string - Google Cloud Storage: Project ID
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   hostname - string - Hostname or IP address. For Agent Remote Servers, the Agent reports the hostname it connects from at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        ///   linode_access_key - string - Linode: Access Key
        ///   linode_bucket - string - Linode: Bucket name
        ///   linode_region - string - Linode: region
        ///   max_connections - int64 - Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        ///   name - string - Internal name for your reference
        ///   one_drive_account_type - string - OneDrive: Either personal or business_other account types
        ///   pin_to_site_region - boolean - If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        ///   port - int64 - Port for remote server. For Agent Remote Servers, the Agent reports its downstream proxy port at each registration, replacing the stored value. This field is not included in the generated Agent configuration file. Setting it on Create or Update does not change the running Agent.
        ///   upload_staging_path - string - Upload staging path.  Applies to SFTP only.  If a path is provided here, files will first be uploaded to this path on the remote folder and the moved into the final correct path via an SFTP move command.  This is required by some remote MFT systems to emulate atomic uploads, which are otherwise not supoprted by SFTP.
        ///   remote_server_credential_id - int64 - ID of Remote Server Credential, if applicable.
        ///   s3_assume_role_arn - string - AWS IAM Role ARN for AssumeRole authentication.
        ///   s3_assume_role_duration_seconds - int64 - Session duration in seconds for AssumeRole authentication (900-43200).
        ///   s3_bucket - string - S3 bucket name
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   s3_compatible_bucket - string - S3-compatible: Bucket name
        ///   s3_compatible_endpoint - string - S3-compatible: endpoint
        ///   s3_compatible_region - string - S3-compatible: region
        ///   s3_compatible_virtual_hosted_style - boolean - S3-compatible: If true, use virtual-hosted-style URLs instead of path-style URLs
        ///   s3_kms_key_id - string - ARN of the AWS KMS key that encrypts files written to the bucket (SSE-KMS). Leave blank unless your bucket requires a specific key.
        ///   s3_region - string - S3 region
        ///   server_certificate - string - Remote server certificate
        ///   server_host_key - string - Pinned SSH host key or OpenSSH host certificate for SFTP. If omitted, Files.com detects and stores a host key, preferring plain keys over certificates. With `server_certificate=require_match` (the default), the server must present the exact pinned key or certificate and prove it holds the matching private key. A pinned certificate is compared in full, so renewal can require updating `server_host_key` even when its underlying key is unchanged. Files.com does not check certificate CA signatures, principals, or validity periods. Certificate expiration alone does not end the pin. Update `server_host_key` to replace the pin.
        ///   server_type - string - Remote server type.
        ///   sharepoint_client_id - string - SharePoint: Microsoft Entra application client ID for app-only authentication.
        ///   sharepoint_site_url - string - SharePoint: Site URL to scope app-only authentication to a single site. Leave blank to browse all sites.
        ///   sharepoint_tenant_id - string - SharePoint: Microsoft Entra tenant ID for app-only authentication.
        ///   ssl - string - Should we require SSL?
        ///   username - string - Remote server username.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   wasabi_bucket - string - Wasabi: Bucket name
        ///   wasabi_region - string - Wasabi: Region
        /// </summary>
        /// <param name="id">Remote Server ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServer> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteServer> UpdateCore(
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
            if (parameters.TryGetValue("password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: password is null, which cannot be sent" : "Bad parameter: password must be of type string", "password");
            }
            if (parameters.TryGetValue("private_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key is null, which cannot be sent" : "Bad parameter: private_key must be of type string", "private_key");
            }
            if (parameters.TryGetValue("private_key_passphrase", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: private_key_passphrase is null, which cannot be sent" : "Bad parameter: private_key_passphrase must be of type string", "private_key_passphrase");
            }
            if (parameters.TryGetValue("reset_authentication", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: reset_authentication is null, which cannot be sent" : "Bad parameter: reset_authentication must be of type bool", "reset_authentication");
            }
            if (parameters.TryGetValue("sharepoint_client_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_certificate is null, which cannot be sent" : "Bad parameter: sharepoint_client_certificate must be of type string", "sharepoint_client_certificate");
            }
            if (parameters.TryGetValue("sharepoint_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_secret is null, which cannot be sent" : "Bad parameter: sharepoint_client_secret must be of type string", "sharepoint_client_secret");
            }
            if (parameters.TryGetValue("ssl_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl_certificate is null, which cannot be sent" : "Bad parameter: ssl_certificate must be of type string", "ssl_certificate");
            }
            if (parameters.TryGetValue("aws_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_secret_key is null, which cannot be sent" : "Bad parameter: aws_secret_key must be of type string", "aws_secret_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_access_key is null, which cannot be sent" : "Bad parameter: azure_blob_storage_access_key must be of type string", "azure_blob_storage_access_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_sas_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_sas_token is null, which cannot be sent" : "Bad parameter: azure_blob_storage_sas_token must be of type string", "azure_blob_storage_sas_token");
            }
            if (parameters.TryGetValue("azure_files_storage_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_access_key is null, which cannot be sent" : "Bad parameter: azure_files_storage_access_key must be of type string", "azure_files_storage_access_key");
            }
            if (parameters.TryGetValue("azure_files_storage_sas_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_sas_token is null, which cannot be sent" : "Bad parameter: azure_files_storage_sas_token must be of type string", "azure_files_storage_sas_token");
            }
            if (parameters.TryGetValue("backblaze_b2_application_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_application_key is null, which cannot be sent" : "Bad parameter: backblaze_b2_application_key must be of type string", "backblaze_b2_application_key");
            }
            if (parameters.TryGetValue("backblaze_b2_key_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_key_id is null, which cannot be sent" : "Bad parameter: backblaze_b2_key_id must be of type string", "backblaze_b2_key_id");
            }
            if (parameters.TryGetValue("cloudflare_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_secret_key is null, which cannot be sent" : "Bad parameter: cloudflare_secret_key must be of type string", "cloudflare_secret_key");
            }
            if (parameters.TryGetValue("filebase_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_secret_key is null, which cannot be sent" : "Bad parameter: filebase_secret_key must be of type string", "filebase_secret_key");
            }
            if (parameters.TryGetValue("google_cloud_storage_credentials_json", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_credentials_json is null, which cannot be sent" : "Bad parameter: google_cloud_storage_credentials_json must be of type string", "google_cloud_storage_credentials_json");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_secret_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_secret_key must be of type string", "google_cloud_storage_s3_compatible_secret_key");
            }
            if (parameters.TryGetValue("linode_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_secret_key is null, which cannot be sent" : "Bad parameter: linode_secret_key must be of type string", "linode_secret_key");
            }
            if (parameters.TryGetValue("s3_compatible_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_secret_key is null, which cannot be sent" : "Bad parameter: s3_compatible_secret_key must be of type string", "s3_compatible_secret_key");
            }
            if (parameters.TryGetValue("wasabi_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_secret_key is null, which cannot be sent" : "Bad parameter: wasabi_secret_key must be of type string", "wasabi_secret_key");
            }
            if (parameters.TryGetValue("allow_relative_paths", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_relative_paths is null, which cannot be sent" : "Bad parameter: allow_relative_paths must be of type bool", "allow_relative_paths");
            }
            if (parameters.TryGetValue("aws_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_access_key is null, which cannot be sent" : "Bad parameter: aws_access_key must be of type string", "aws_access_key");
            }
            if (parameters.TryGetValue("azure_blob_storage_account", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_account is null, which cannot be sent" : "Bad parameter: azure_blob_storage_account must be of type string", "azure_blob_storage_account");
            }
            if (parameters.TryGetValue("azure_blob_storage_container", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_container is null, which cannot be sent" : "Bad parameter: azure_blob_storage_container must be of type string", "azure_blob_storage_container");
            }
            if (parameters.TryGetValue("azure_blob_storage_dns_suffix", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_dns_suffix is null, which cannot be sent" : "Bad parameter: azure_blob_storage_dns_suffix must be of type string", "azure_blob_storage_dns_suffix");
            }
            if (parameters.TryGetValue("azure_blob_storage_hierarchical_namespace", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_blob_storage_hierarchical_namespace is null, which cannot be sent" : "Bad parameter: azure_blob_storage_hierarchical_namespace must be of type bool", "azure_blob_storage_hierarchical_namespace");
            }
            if (parameters.TryGetValue("azure_files_storage_account", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_account is null, which cannot be sent" : "Bad parameter: azure_files_storage_account must be of type string", "azure_files_storage_account");
            }
            if (parameters.TryGetValue("azure_files_storage_dns_suffix", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_dns_suffix is null, which cannot be sent" : "Bad parameter: azure_files_storage_dns_suffix must be of type string", "azure_files_storage_dns_suffix");
            }
            if (parameters.TryGetValue("azure_files_storage_share_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_files_storage_share_name is null, which cannot be sent" : "Bad parameter: azure_files_storage_share_name must be of type string", "azure_files_storage_share_name");
            }
            if (parameters.TryGetValue("backblaze_b2_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_bucket is null, which cannot be sent" : "Bad parameter: backblaze_b2_bucket must be of type string", "backblaze_b2_bucket");
            }
            if (parameters.TryGetValue("backblaze_b2_s3_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: backblaze_b2_s3_endpoint is null, which cannot be sent" : "Bad parameter: backblaze_b2_s3_endpoint must be of type string", "backblaze_b2_s3_endpoint");
            }
            if (parameters.TryGetValue("buffer_uploads", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: buffer_uploads is null, which cannot be sent" : "Bad parameter: buffer_uploads must be of type string", "buffer_uploads");
            }
            if (parameters.TryGetValue("cloudflare_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_access_key is null, which cannot be sent" : "Bad parameter: cloudflare_access_key must be of type string", "cloudflare_access_key");
            }
            if (parameters.TryGetValue("cloudflare_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_bucket is null, which cannot be sent" : "Bad parameter: cloudflare_bucket must be of type string", "cloudflare_bucket");
            }
            if (parameters.TryGetValue("cloudflare_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_endpoint is null, which cannot be sent" : "Bad parameter: cloudflare_endpoint must be of type string", "cloudflare_endpoint");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("dropbox_teams", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dropbox_teams is null, which cannot be sent" : "Bad parameter: dropbox_teams must be of type bool", "dropbox_teams");
            }
            if (parameters.TryGetValue("enable_dedicated_ips", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: enable_dedicated_ips is null, which cannot be sent" : "Bad parameter: enable_dedicated_ips must be of type bool", "enable_dedicated_ips");
            }
            if (parameters.TryGetValue("filebase_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_access_key is null, which cannot be sent" : "Bad parameter: filebase_access_key must be of type string", "filebase_access_key");
            }
            if (parameters.TryGetValue("filebase_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_bucket is null, which cannot be sent" : "Bad parameter: filebase_bucket must be of type string", "filebase_bucket");
            }
            if (parameters.TryGetValue("files_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_api_key is null, which cannot be sent" : "Bad parameter: files_api_key must be of type string", "files_api_key");
            }
            if (parameters.TryGetValue("files_agent_permission_set", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_permission_set is null, which cannot be sent" : "Bad parameter: files_agent_permission_set must be of type string", "files_agent_permission_set");
            }
            if (parameters.TryGetValue("files_agent_root", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_root is null, which cannot be sent" : "Bad parameter: files_agent_root must be of type string", "files_agent_root");
            }
            if (parameters.TryGetValue("files_agent_version", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: files_agent_version is null, which cannot be sent" : "Bad parameter: files_agent_version must be of type string", "files_agent_version");
            }
            if (parameters.TryGetValue("outbound_agent_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: outbound_agent_id is null, which cannot be sent" : "Bad parameter: outbound_agent_id must be of type Nullable<Int64>", "outbound_agent_id");
            }
            if (parameters.TryGetValue("custom_domain_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: custom_domain_id is null, which cannot be sent" : "Bad parameter: custom_domain_id must be of type Nullable<Int64>", "custom_domain_id");
            }
            if (parameters.TryGetValue("google_cloud_storage_authentication_method", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_authentication_method is null, which cannot be sent" : "Bad parameter: google_cloud_storage_authentication_method must be of type string", "google_cloud_storage_authentication_method");
            }
            if (parameters.TryGetValue("google_cloud_storage_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_bucket is null, which cannot be sent" : "Bad parameter: google_cloud_storage_bucket must be of type string", "google_cloud_storage_bucket");
            }
            if (parameters.TryGetValue("google_cloud_storage_oauth_scope", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_oauth_scope is null, which cannot be sent" : "Bad parameter: google_cloud_storage_oauth_scope must be of type string", "google_cloud_storage_oauth_scope");
            }
            if (parameters.TryGetValue("google_cloud_storage_project_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_project_id is null, which cannot be sent" : "Bad parameter: google_cloud_storage_project_id must be of type string", "google_cloud_storage_project_id");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "google_cloud_storage_s3_compatible_access_key");
            }
            if (parameters.TryGetValue("hostname", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: hostname is null, which cannot be sent" : "Bad parameter: hostname must be of type string", "hostname");
            }
            if (parameters.TryGetValue("linode_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_access_key is null, which cannot be sent" : "Bad parameter: linode_access_key must be of type string", "linode_access_key");
            }
            if (parameters.TryGetValue("linode_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_bucket is null, which cannot be sent" : "Bad parameter: linode_bucket must be of type string", "linode_bucket");
            }
            if (parameters.TryGetValue("linode_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_region is null, which cannot be sent" : "Bad parameter: linode_region must be of type string", "linode_region");
            }
            if (parameters.TryGetValue("max_connections", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_connections is null, which cannot be sent" : "Bad parameter: max_connections must be of type Nullable<Int64>", "max_connections");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("one_drive_account_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: one_drive_account_type is null, which cannot be sent" : "Bad parameter: one_drive_account_type must be of type string", "one_drive_account_type");
            }
            if (parameters.TryGetValue("pin_to_site_region", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: pin_to_site_region is null, which cannot be sent" : "Bad parameter: pin_to_site_region must be of type bool", "pin_to_site_region");
            }
            if (parameters.TryGetValue("port", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: port is null, which cannot be sent" : "Bad parameter: port must be of type Nullable<Int64>", "port");
            }
            if (parameters.TryGetValue("upload_staging_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: upload_staging_path is null, which cannot be sent" : "Bad parameter: upload_staging_path must be of type string", "upload_staging_path");
            }
            if (parameters.TryGetValue("remote_server_credential_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: remote_server_credential_id is null, which cannot be sent" : "Bad parameter: remote_server_credential_id must be of type Nullable<Int64>", "remote_server_credential_id");
            }
            if (parameters.TryGetValue("s3_assume_role_arn", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_arn is null, which cannot be sent" : "Bad parameter: s3_assume_role_arn must be of type string", "s3_assume_role_arn");
            }
            if (parameters.TryGetValue("s3_assume_role_duration_seconds", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_duration_seconds is null, which cannot be sent" : "Bad parameter: s3_assume_role_duration_seconds must be of type Nullable<Int64>", "s3_assume_role_duration_seconds");
            }
            if (parameters.TryGetValue("s3_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_bucket is null, which cannot be sent" : "Bad parameter: s3_bucket must be of type string", "s3_bucket");
            }
            if (parameters.TryGetValue("s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: s3_compatible_access_key must be of type string", "s3_compatible_access_key");
            }
            if (parameters.TryGetValue("s3_compatible_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_bucket is null, which cannot be sent" : "Bad parameter: s3_compatible_bucket must be of type string", "s3_compatible_bucket");
            }
            if (parameters.TryGetValue("s3_compatible_endpoint", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_endpoint is null, which cannot be sent" : "Bad parameter: s3_compatible_endpoint must be of type string", "s3_compatible_endpoint");
            }
            if (parameters.TryGetValue("s3_compatible_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_region is null, which cannot be sent" : "Bad parameter: s3_compatible_region must be of type string", "s3_compatible_region");
            }
            if (parameters.TryGetValue("s3_compatible_virtual_hosted_style", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_virtual_hosted_style is null, which cannot be sent" : "Bad parameter: s3_compatible_virtual_hosted_style must be of type bool", "s3_compatible_virtual_hosted_style");
            }
            if (parameters.TryGetValue("s3_kms_key_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_kms_key_id is null, which cannot be sent" : "Bad parameter: s3_kms_key_id must be of type string", "s3_kms_key_id");
            }
            if (parameters.TryGetValue("s3_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_region is null, which cannot be sent" : "Bad parameter: s3_region must be of type string", "s3_region");
            }
            if (parameters.TryGetValue("server_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_certificate is null, which cannot be sent" : "Bad parameter: server_certificate must be of type string", "server_certificate");
            }
            if (parameters.TryGetValue("server_host_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_host_key is null, which cannot be sent" : "Bad parameter: server_host_key must be of type string", "server_host_key");
            }
            if (parameters.TryGetValue("server_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_type is null, which cannot be sent" : "Bad parameter: server_type must be of type string", "server_type");
            }
            if (parameters.TryGetValue("sharepoint_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_id is null, which cannot be sent" : "Bad parameter: sharepoint_client_id must be of type string", "sharepoint_client_id");
            }
            if (parameters.TryGetValue("sharepoint_site_url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_site_url is null, which cannot be sent" : "Bad parameter: sharepoint_site_url must be of type string", "sharepoint_site_url");
            }
            if (parameters.TryGetValue("sharepoint_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_tenant_id is null, which cannot be sent" : "Bad parameter: sharepoint_tenant_id must be of type string", "sharepoint_tenant_id");
            }
            if (parameters.TryGetValue("ssl", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ssl is null, which cannot be sent" : "Bad parameter: ssl must be of type string", "ssl");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("wasabi_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_access_key is null, which cannot be sent" : "Bad parameter: wasabi_access_key must be of type string", "wasabi_access_key");
            }
            if (parameters.TryGetValue("wasabi_bucket", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_bucket is null, which cannot be sent" : "Bad parameter: wasabi_bucket must be of type string", "wasabi_bucket");
            }
            if (parameters.TryGetValue("wasabi_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_region is null, which cannot be sent" : "Bad parameter: wasabi_region must be of type string", "wasabi_region");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteServer>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /remote_servers/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Remote Server ID.</param>
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
        /// Sends DELETE /remote_servers/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Remote Server ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}