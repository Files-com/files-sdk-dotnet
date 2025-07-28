using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class RemoteServer
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public RemoteServer() : this(null, null) { }

        public RemoteServer(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("port"))
            {
                this.attributes.Add("port", null);
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
            if (!this.attributes.ContainsKey("s3_bucket"))
            {
                this.attributes.Add("s3_bucket", null);
            }
            if (!this.attributes.ContainsKey("s3_region"))
            {
                this.attributes.Add("s3_region", null);
            }
            if (!this.attributes.ContainsKey("aws_access_key"))
            {
                this.attributes.Add("aws_access_key", null);
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
            if (!this.attributes.ContainsKey("s3_compatible_access_key"))
            {
                this.attributes.Add("s3_compatible_access_key", null);
            }
            if (!this.attributes.ContainsKey("enable_dedicated_ips"))
            {
                this.attributes.Add("enable_dedicated_ips", false);
            }
            if (!this.attributes.ContainsKey("files_agent_permission_set"))
            {
                this.attributes.Add("files_agent_permission_set", null);
            }
            if (!this.attributes.ContainsKey("files_agent_root"))
            {
                this.attributes.Add("files_agent_root", null);
            }
            if (!this.attributes.ContainsKey("files_agent_api_token"))
            {
                this.attributes.Add("files_agent_api_token", null);
            }
            if (!this.attributes.ContainsKey("files_agent_version"))
            {
                this.attributes.Add("files_agent_version", null);
            }
            if (!this.attributes.ContainsKey("filebase_bucket"))
            {
                this.attributes.Add("filebase_bucket", null);
            }
            if (!this.attributes.ContainsKey("filebase_access_key"))
            {
                this.attributes.Add("filebase_access_key", null);
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
        /// Remote server ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// If true, this server has been disabled due to failures.  Make any change or set disabled to false to clear this flag.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; }
        }

        /// <summary>
        /// Type of authentication method
        /// </summary>
        [JsonPropertyName("authentication_method")]
        public string AuthenticationMethod
        {
            get { return (string)attributes["authentication_method"]; }
            set { attributes["authentication_method"] = value; }
        }

        /// <summary>
        /// Hostname or IP address
        /// </summary>
        [JsonPropertyName("hostname")]
        public string Hostname
        {
            get { return (string)attributes["hostname"]; }
            set { attributes["hostname"] = value; }
        }

        /// <summary>
        /// Initial home folder on remote server
        /// </summary>
        [JsonPropertyName("remote_home_path")]
        public string RemoteHomePath
        {
            get { return (string)attributes["remote_home_path"]; }
            set { attributes["remote_home_path"] = value; }
        }

        /// <summary>
        /// Internal name for your reference
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Port for remote server.  Not needed for S3.
        /// </summary>
        [JsonPropertyName("port")]
        public Nullable<Int64> Port
        {
            get { return (Nullable<Int64>)attributes["port"]; }
            set { attributes["port"] = value; }
        }

        /// <summary>
        /// Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        /// </summary>
        [JsonPropertyName("max_connections")]
        public Nullable<Int64> MaxConnections
        {
            get { return (Nullable<Int64>)attributes["max_connections"]; }
            set { attributes["max_connections"] = value; }
        }

        /// <summary>
        /// If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("pin_to_site_region")]
        public bool PinToSiteRegion
        {
            get { return attributes["pin_to_site_region"] == null ? false : (bool)attributes["pin_to_site_region"]; }
            set { attributes["pin_to_site_region"] = value; }
        }

        /// <summary>
        /// If set, all communications with this remote server are made through the provided region.
        /// </summary>
        [JsonPropertyName("pinned_region")]
        public string PinnedRegion
        {
            get { return (string)attributes["pinned_region"]; }
            set { attributes["pinned_region"] = value; }
        }

        /// <summary>
        /// S3 bucket name
        /// </summary>
        [JsonPropertyName("s3_bucket")]
        public string S3Bucket
        {
            get { return (string)attributes["s3_bucket"]; }
            set { attributes["s3_bucket"] = value; }
        }

        /// <summary>
        /// S3 region
        /// </summary>
        [JsonPropertyName("s3_region")]
        public string S3Region
        {
            get { return (string)attributes["s3_region"]; }
            set { attributes["s3_region"] = value; }
        }

        /// <summary>
        /// AWS Access Key.
        /// </summary>
        [JsonPropertyName("aws_access_key")]
        public string AwsAccessKey
        {
            get { return (string)attributes["aws_access_key"]; }
            set { attributes["aws_access_key"] = value; }
        }

        /// <summary>
        /// Remote server certificate
        /// </summary>
        [JsonPropertyName("server_certificate")]
        public string ServerCertificate
        {
            get { return (string)attributes["server_certificate"]; }
            set { attributes["server_certificate"] = value; }
        }

        /// <summary>
        /// Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
        /// </summary>
        [JsonPropertyName("server_host_key")]
        public string ServerHostKey
        {
            get { return (string)attributes["server_host_key"]; }
            set { attributes["server_host_key"] = value; }
        }

        /// <summary>
        /// Remote server type.
        /// </summary>
        [JsonPropertyName("server_type")]
        public string ServerType
        {
            get { return (string)attributes["server_type"]; }
            set { attributes["server_type"] = value; }
        }

        /// <summary>
        /// Should we require SSL?
        /// </summary>
        [JsonPropertyName("ssl")]
        public string Ssl
        {
            get { return (string)attributes["ssl"]; }
            set { attributes["ssl"] = value; }
        }

        /// <summary>
        /// Remote server username.  Not needed for S3 buckets.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// Google Cloud Storage: Bucket Name
        /// </summary>
        [JsonPropertyName("google_cloud_storage_bucket")]
        public string GoogleCloudStorageBucket
        {
            get { return (string)attributes["google_cloud_storage_bucket"]; }
            set { attributes["google_cloud_storage_bucket"] = value; }
        }

        /// <summary>
        /// Google Cloud Storage: Project ID
        /// </summary>
        [JsonPropertyName("google_cloud_storage_project_id")]
        public string GoogleCloudStorageProjectId
        {
            get { return (string)attributes["google_cloud_storage_project_id"]; }
            set { attributes["google_cloud_storage_project_id"] = value; }
        }

        /// <summary>
        /// Google Cloud Storage: S3-compatible Access Key.
        /// </summary>
        [JsonPropertyName("google_cloud_storage_s3_compatible_access_key")]
        public string GoogleCloudStorageS3CompatibleAccessKey
        {
            get { return (string)attributes["google_cloud_storage_s3_compatible_access_key"]; }
            set { attributes["google_cloud_storage_s3_compatible_access_key"] = value; }
        }

        /// <summary>
        /// Backblaze B2 Cloud Storage: S3 Endpoint
        /// </summary>
        [JsonPropertyName("backblaze_b2_s3_endpoint")]
        public string BackblazeB2S3Endpoint
        {
            get { return (string)attributes["backblaze_b2_s3_endpoint"]; }
            set { attributes["backblaze_b2_s3_endpoint"] = value; }
        }

        /// <summary>
        /// Backblaze B2 Cloud Storage: Bucket name
        /// </summary>
        [JsonPropertyName("backblaze_b2_bucket")]
        public string BackblazeB2Bucket
        {
            get { return (string)attributes["backblaze_b2_bucket"]; }
            set { attributes["backblaze_b2_bucket"] = value; }
        }

        /// <summary>
        /// Wasabi: Bucket name
        /// </summary>
        [JsonPropertyName("wasabi_bucket")]
        public string WasabiBucket
        {
            get { return (string)attributes["wasabi_bucket"]; }
            set { attributes["wasabi_bucket"] = value; }
        }

        /// <summary>
        /// Wasabi: Region
        /// </summary>
        [JsonPropertyName("wasabi_region")]
        public string WasabiRegion
        {
            get { return (string)attributes["wasabi_region"]; }
            set { attributes["wasabi_region"] = value; }
        }

        /// <summary>
        /// Wasabi: Access Key.
        /// </summary>
        [JsonPropertyName("wasabi_access_key")]
        public string WasabiAccessKey
        {
            get { return (string)attributes["wasabi_access_key"]; }
            set { attributes["wasabi_access_key"] = value; }
        }

        /// <summary>
        /// Either `in_setup` or `complete`
        /// </summary>
        [JsonPropertyName("auth_status")]
        public string AuthStatus
        {
            get { return (string)attributes["auth_status"]; }
            set { attributes["auth_status"] = value; }
        }

        /// <summary>
        /// Describes the authorized account
        /// </summary>
        [JsonPropertyName("auth_account_name")]
        public string AuthAccountName
        {
            get { return (string)attributes["auth_account_name"]; }
            set { attributes["auth_account_name"] = value; }
        }

        /// <summary>
        /// OneDrive: Either personal or business_other account types
        /// </summary>
        [JsonPropertyName("one_drive_account_type")]
        public string OneDriveAccountType
        {
            get { return (string)attributes["one_drive_account_type"]; }
            set { attributes["one_drive_account_type"] = value; }
        }

        /// <summary>
        /// Azure Blob Storage: Account name
        /// </summary>
        [JsonPropertyName("azure_blob_storage_account")]
        public string AzureBlobStorageAccount
        {
            get { return (string)attributes["azure_blob_storage_account"]; }
            set { attributes["azure_blob_storage_account"] = value; }
        }

        /// <summary>
        /// Azure Blob Storage: Container name
        /// </summary>
        [JsonPropertyName("azure_blob_storage_container")]
        public string AzureBlobStorageContainer
        {
            get { return (string)attributes["azure_blob_storage_container"]; }
            set { attributes["azure_blob_storage_container"] = value; }
        }

        /// <summary>
        /// Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("azure_blob_storage_hierarchical_namespace")]
        public bool AzureBlobStorageHierarchicalNamespace
        {
            get { return attributes["azure_blob_storage_hierarchical_namespace"] == null ? false : (bool)attributes["azure_blob_storage_hierarchical_namespace"]; }
            set { attributes["azure_blob_storage_hierarchical_namespace"] = value; }
        }

        /// <summary>
        /// Azure Blob Storage: Custom DNS suffix
        /// </summary>
        [JsonPropertyName("azure_blob_storage_dns_suffix")]
        public string AzureBlobStorageDnsSuffix
        {
            get { return (string)attributes["azure_blob_storage_dns_suffix"]; }
            set { attributes["azure_blob_storage_dns_suffix"] = value; }
        }

        /// <summary>
        /// Azure Files: Storage Account name
        /// </summary>
        [JsonPropertyName("azure_files_storage_account")]
        public string AzureFilesStorageAccount
        {
            get { return (string)attributes["azure_files_storage_account"]; }
            set { attributes["azure_files_storage_account"] = value; }
        }

        /// <summary>
        /// Azure Files:  Storage Share name
        /// </summary>
        [JsonPropertyName("azure_files_storage_share_name")]
        public string AzureFilesStorageShareName
        {
            get { return (string)attributes["azure_files_storage_share_name"]; }
            set { attributes["azure_files_storage_share_name"] = value; }
        }

        /// <summary>
        /// Azure Files: Custom DNS suffix
        /// </summary>
        [JsonPropertyName("azure_files_storage_dns_suffix")]
        public string AzureFilesStorageDnsSuffix
        {
            get { return (string)attributes["azure_files_storage_dns_suffix"]; }
            set { attributes["azure_files_storage_dns_suffix"] = value; }
        }

        /// <summary>
        /// S3-compatible: Bucket name
        /// </summary>
        [JsonPropertyName("s3_compatible_bucket")]
        public string S3CompatibleBucket
        {
            get { return (string)attributes["s3_compatible_bucket"]; }
            set { attributes["s3_compatible_bucket"] = value; }
        }

        /// <summary>
        /// S3-compatible: endpoint
        /// </summary>
        [JsonPropertyName("s3_compatible_endpoint")]
        public string S3CompatibleEndpoint
        {
            get { return (string)attributes["s3_compatible_endpoint"]; }
            set { attributes["s3_compatible_endpoint"] = value; }
        }

        /// <summary>
        /// S3-compatible: region
        /// </summary>
        [JsonPropertyName("s3_compatible_region")]
        public string S3CompatibleRegion
        {
            get { return (string)attributes["s3_compatible_region"]; }
            set { attributes["s3_compatible_region"] = value; }
        }

        /// <summary>
        /// S3-compatible: Access Key
        /// </summary>
        [JsonPropertyName("s3_compatible_access_key")]
        public string S3CompatibleAccessKey
        {
            get { return (string)attributes["s3_compatible_access_key"]; }
            set { attributes["s3_compatible_access_key"] = value; }
        }

        /// <summary>
        /// `true` if remote server only accepts connections from dedicated IPs
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("enable_dedicated_ips")]
        public bool EnableDedicatedIps
        {
            get { return attributes["enable_dedicated_ips"] == null ? false : (bool)attributes["enable_dedicated_ips"]; }
            set { attributes["enable_dedicated_ips"] = value; }
        }

        /// <summary>
        /// Local permissions for files agent. read_only, write_only, or read_write
        /// </summary>
        [JsonPropertyName("files_agent_permission_set")]
        public string FilesAgentPermissionSet
        {
            get { return (string)attributes["files_agent_permission_set"]; }
            set { attributes["files_agent_permission_set"] = value; }
        }

        /// <summary>
        /// Agent local root path
        /// </summary>
        [JsonPropertyName("files_agent_root")]
        public string FilesAgentRoot
        {
            get { return (string)attributes["files_agent_root"]; }
            set { attributes["files_agent_root"] = value; }
        }

        /// <summary>
        /// Files Agent API Token
        /// </summary>
        [JsonPropertyName("files_agent_api_token")]
        public string FilesAgentApiToken
        {
            get { return (string)attributes["files_agent_api_token"]; }
            set { attributes["files_agent_api_token"] = value; }
        }

        /// <summary>
        /// Files Agent version
        /// </summary>
        [JsonPropertyName("files_agent_version")]
        public string FilesAgentVersion
        {
            get { return (string)attributes["files_agent_version"]; }
            set { attributes["files_agent_version"] = value; }
        }

        /// <summary>
        /// Filebase: Bucket name
        /// </summary>
        [JsonPropertyName("filebase_bucket")]
        public string FilebaseBucket
        {
            get { return (string)attributes["filebase_bucket"]; }
            set { attributes["filebase_bucket"] = value; }
        }

        /// <summary>
        /// Filebase: Access Key.
        /// </summary>
        [JsonPropertyName("filebase_access_key")]
        public string FilebaseAccessKey
        {
            get { return (string)attributes["filebase_access_key"]; }
            set { attributes["filebase_access_key"] = value; }
        }

        /// <summary>
        /// Cloudflare: Bucket name
        /// </summary>
        [JsonPropertyName("cloudflare_bucket")]
        public string CloudflareBucket
        {
            get { return (string)attributes["cloudflare_bucket"]; }
            set { attributes["cloudflare_bucket"] = value; }
        }

        /// <summary>
        /// Cloudflare: Access Key.
        /// </summary>
        [JsonPropertyName("cloudflare_access_key")]
        public string CloudflareAccessKey
        {
            get { return (string)attributes["cloudflare_access_key"]; }
            set { attributes["cloudflare_access_key"] = value; }
        }

        /// <summary>
        /// Cloudflare: endpoint
        /// </summary>
        [JsonPropertyName("cloudflare_endpoint")]
        public string CloudflareEndpoint
        {
            get { return (string)attributes["cloudflare_endpoint"]; }
            set { attributes["cloudflare_endpoint"] = value; }
        }

        /// <summary>
        /// Dropbox: If true, list Team folders in root?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("dropbox_teams")]
        public bool DropboxTeams
        {
            get { return attributes["dropbox_teams"] == null ? false : (bool)attributes["dropbox_teams"]; }
            set { attributes["dropbox_teams"] = value; }
        }

        /// <summary>
        /// Linode: Bucket name
        /// </summary>
        [JsonPropertyName("linode_bucket")]
        public string LinodeBucket
        {
            get { return (string)attributes["linode_bucket"]; }
            set { attributes["linode_bucket"] = value; }
        }

        /// <summary>
        /// Linode: Access Key
        /// </summary>
        [JsonPropertyName("linode_access_key")]
        public string LinodeAccessKey
        {
            get { return (string)attributes["linode_access_key"]; }
            set { attributes["linode_access_key"] = value; }
        }

        /// <summary>
        /// Linode: region
        /// </summary>
        [JsonPropertyName("linode_region")]
        public string LinodeRegion
        {
            get { return (string)attributes["linode_region"]; }
            set { attributes["linode_region"] = value; }
        }

        /// <summary>
        /// If true, this remote server supports file versioning. This value is determined automatically by Files.com.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("supports_versioning")]
        public bool SupportsVersioning
        {
            get { return attributes["supports_versioning"] == null ? false : (bool)attributes["supports_versioning"]; }
            set { attributes["supports_versioning"] = value; }
        }

        /// <summary>
        /// Password, if needed.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password
        {
            get { return (string)attributes["password"]; }
            set { attributes["password"] = value; }
        }

        /// <summary>
        /// Private key, if needed.
        /// </summary>
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string)attributes["private_key"]; }
            set { attributes["private_key"] = value; }
        }

        /// <summary>
        /// Passphrase for private key if needed.
        /// </summary>
        [JsonPropertyName("private_key_passphrase")]
        public string PrivateKeyPassphrase
        {
            get { return (string)attributes["private_key_passphrase"]; }
            set { attributes["private_key_passphrase"] = value; }
        }

        /// <summary>
        /// Reset authenticated account?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("reset_authentication")]
        public bool ResetAuthentication
        {
            get { return attributes["reset_authentication"] == null ? false : (bool)attributes["reset_authentication"]; }
            set { attributes["reset_authentication"] = value; }
        }

        /// <summary>
        /// SSL client certificate.
        /// </summary>
        [JsonPropertyName("ssl_certificate")]
        public string SslCertificate
        {
            get { return (string)attributes["ssl_certificate"]; }
            set { attributes["ssl_certificate"] = value; }
        }

        /// <summary>
        /// AWS: secret key.
        /// </summary>
        [JsonPropertyName("aws_secret_key")]
        public string AwsSecretKey
        {
            get { return (string)attributes["aws_secret_key"]; }
            set { attributes["aws_secret_key"] = value; }
        }

        /// <summary>
        /// Azure Blob Storage: Access Key
        /// </summary>
        [JsonPropertyName("azure_blob_storage_access_key")]
        public string AzureBlobStorageAccessKey
        {
            get { return (string)attributes["azure_blob_storage_access_key"]; }
            set { attributes["azure_blob_storage_access_key"] = value; }
        }

        /// <summary>
        /// Azure Blob Storage: Shared Access Signature (SAS) token
        /// </summary>
        [JsonPropertyName("azure_blob_storage_sas_token")]
        public string AzureBlobStorageSasToken
        {
            get { return (string)attributes["azure_blob_storage_sas_token"]; }
            set { attributes["azure_blob_storage_sas_token"] = value; }
        }

        /// <summary>
        /// Azure File Storage: Access Key
        /// </summary>
        [JsonPropertyName("azure_files_storage_access_key")]
        public string AzureFilesStorageAccessKey
        {
            get { return (string)attributes["azure_files_storage_access_key"]; }
            set { attributes["azure_files_storage_access_key"] = value; }
        }

        /// <summary>
        /// Azure File Storage: Shared Access Signature (SAS) token
        /// </summary>
        [JsonPropertyName("azure_files_storage_sas_token")]
        public string AzureFilesStorageSasToken
        {
            get { return (string)attributes["azure_files_storage_sas_token"]; }
            set { attributes["azure_files_storage_sas_token"] = value; }
        }

        /// <summary>
        /// Backblaze B2 Cloud Storage: applicationKey
        /// </summary>
        [JsonPropertyName("backblaze_b2_application_key")]
        public string BackblazeB2ApplicationKey
        {
            get { return (string)attributes["backblaze_b2_application_key"]; }
            set { attributes["backblaze_b2_application_key"] = value; }
        }

        /// <summary>
        /// Backblaze B2 Cloud Storage: keyID
        /// </summary>
        [JsonPropertyName("backblaze_b2_key_id")]
        public string BackblazeB2KeyId
        {
            get { return (string)attributes["backblaze_b2_key_id"]; }
            set { attributes["backblaze_b2_key_id"] = value; }
        }

        /// <summary>
        /// Cloudflare: Secret Key
        /// </summary>
        [JsonPropertyName("cloudflare_secret_key")]
        public string CloudflareSecretKey
        {
            get { return (string)attributes["cloudflare_secret_key"]; }
            set { attributes["cloudflare_secret_key"] = value; }
        }

        /// <summary>
        /// Filebase: Secret Key
        /// </summary>
        [JsonPropertyName("filebase_secret_key")]
        public string FilebaseSecretKey
        {
            get { return (string)attributes["filebase_secret_key"]; }
            set { attributes["filebase_secret_key"] = value; }
        }

        /// <summary>
        /// Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
        /// </summary>
        [JsonPropertyName("google_cloud_storage_credentials_json")]
        public string GoogleCloudStorageCredentialsJson
        {
            get { return (string)attributes["google_cloud_storage_credentials_json"]; }
            set { attributes["google_cloud_storage_credentials_json"] = value; }
        }

        /// <summary>
        /// Google Cloud Storage: S3-compatible secret key
        /// </summary>
        [JsonPropertyName("google_cloud_storage_s3_compatible_secret_key")]
        public string GoogleCloudStorageS3CompatibleSecretKey
        {
            get { return (string)attributes["google_cloud_storage_s3_compatible_secret_key"]; }
            set { attributes["google_cloud_storage_s3_compatible_secret_key"] = value; }
        }

        /// <summary>
        /// Linode: Secret Key
        /// </summary>
        [JsonPropertyName("linode_secret_key")]
        public string LinodeSecretKey
        {
            get { return (string)attributes["linode_secret_key"]; }
            set { attributes["linode_secret_key"] = value; }
        }

        /// <summary>
        /// S3-compatible: Secret Key
        /// </summary>
        [JsonPropertyName("s3_compatible_secret_key")]
        public string S3CompatibleSecretKey
        {
            get { return (string)attributes["s3_compatible_secret_key"]; }
            set { attributes["s3_compatible_secret_key"] = value; }
        }

        /// <summary>
        /// Wasabi: Secret Key
        /// </summary>
        [JsonPropertyName("wasabi_secret_key")]
        public string WasabiSecretKey
        {
            get { return (string)attributes["wasabi_secret_key"]; }
            set { attributes["wasabi_secret_key"] = value; }
        }

        /// <summary>
        /// Post local changes, check in, and download configuration file (used by some Remote Server integrations, such as the Files.com Agent)
        ///
        /// Parameters:
        ///   api_token - string - Files Agent API Token
        ///   permission_set - string - The permission set for the agent ['read_write', 'read_only', 'write_only']
        ///   root - string - The root directory for the agent
        ///   hostname - string
        ///   port - int64 - Incoming port for files agent connections
        ///   status - string - either running or shutdown
        ///   config_version - string - agent config version
        ///   private_key - string - The private key for the agent
        ///   public_key - string - public key
        ///   server_host_key - string
        ///   subdomain - string - Files.com subdomain site name
        /// </summary>
        public async Task<RemoteServerConfigurationFile> ConfigurationFile(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("api_token") && !(parameters["api_token"] is string))
            {
                throw new ArgumentException("Bad parameter: api_token must be of type string", "parameters[\"api_token\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }
            if (parameters.ContainsKey("root") && !(parameters["root"] is string))
            {
                throw new ArgumentException("Bad parameter: root must be of type string", "parameters[\"root\"]");
            }
            if (parameters.ContainsKey("hostname") && !(parameters["hostname"] is string))
            {
                throw new ArgumentException("Bad parameter: hostname must be of type string", "parameters[\"hostname\"]");
            }
            if (parameters.ContainsKey("port") && !(parameters["port"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: port must be of type Nullable<Int64>", "parameters[\"port\"]");
            }
            if (parameters.ContainsKey("status") && !(parameters["status"] is string))
            {
                throw new ArgumentException("Bad parameter: status must be of type string", "parameters[\"status\"]");
            }
            if (parameters.ContainsKey("config_version") && !(parameters["config_version"] is string))
            {
                throw new ArgumentException("Bad parameter: config_version must be of type string", "parameters[\"config_version\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("public_key") && !(parameters["public_key"] is string))
            {
                throw new ArgumentException("Bad parameter: public_key must be of type string", "parameters[\"public_key\"]");
            }
            if (parameters.ContainsKey("server_host_key") && !(parameters["server_host_key"] is string))
            {
                throw new ArgumentException("Bad parameter: server_host_key must be of type string", "parameters[\"server_host_key\"]");
            }
            if (parameters.ContainsKey("subdomain") && !(parameters["subdomain"] is string))
            {
                throw new ArgumentException("Bad parameter: subdomain must be of type string", "parameters[\"subdomain\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_servers/{System.Uri.EscapeDataString(attributes["id"].ToString())}/configuration_file", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServerConfigurationFile>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
        ///   reset_authentication - boolean - Reset authenticated account?
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
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   cloudflare_bucket - string - Cloudflare: Bucket name
        ///   cloudflare_endpoint - string - Cloudflare: endpoint
        ///   dropbox_teams - boolean - Dropbox: If true, list Team folders in root?
        ///   enable_dedicated_ips - boolean - `true` if remote server only accepts connections from dedicated IPs
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   filebase_bucket - string - Filebase: Bucket name
        ///   files_agent_permission_set - string - Local permissions for files agent. read_only, write_only, or read_write
        ///   files_agent_root - string - Agent local root path
        ///   files_agent_version - string - Files Agent version
        ///   google_cloud_storage_bucket - string - Google Cloud Storage: Bucket Name
        ///   google_cloud_storage_project_id - string - Google Cloud Storage: Project ID
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   hostname - string - Hostname or IP address
        ///   linode_access_key - string - Linode: Access Key
        ///   linode_bucket - string - Linode: Bucket name
        ///   linode_region - string - Linode: region
        ///   max_connections - int64 - Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        ///   name - string - Internal name for your reference
        ///   one_drive_account_type - string - OneDrive: Either personal or business_other account types
        ///   pin_to_site_region - boolean - If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        ///   port - int64 - Port for remote server.  Not needed for S3.
        ///   s3_bucket - string - S3 bucket name
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   s3_compatible_bucket - string - S3-compatible: Bucket name
        ///   s3_compatible_endpoint - string - S3-compatible: endpoint
        ///   s3_compatible_region - string - S3-compatible: region
        ///   s3_region - string - S3 region
        ///   server_certificate - string - Remote server certificate
        ///   server_host_key - string - Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
        ///   server_type - string - Remote server type.
        ///   ssl - string - Should we require SSL?
        ///   username - string - Remote server username.  Not needed for S3 buckets.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   wasabi_bucket - string - Wasabi: Bucket name
        ///   wasabi_region - string - Wasabi: Region
        /// </summary>
        public async Task<RemoteServer> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_passphrase") && !(parameters["private_key_passphrase"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key_passphrase must be of type string", "parameters[\"private_key_passphrase\"]");
            }
            if (parameters.ContainsKey("reset_authentication") && !(parameters["reset_authentication"] is bool))
            {
                throw new ArgumentException("Bad parameter: reset_authentication must be of type bool", "parameters[\"reset_authentication\"]");
            }
            if (parameters.ContainsKey("ssl_certificate") && !(parameters["ssl_certificate"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl_certificate must be of type string", "parameters[\"ssl_certificate\"]");
            }
            if (parameters.ContainsKey("aws_secret_key") && !(parameters["aws_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_secret_key must be of type string", "parameters[\"aws_secret_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_access_key") && !(parameters["azure_blob_storage_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_access_key must be of type string", "parameters[\"azure_blob_storage_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_sas_token") && !(parameters["azure_blob_storage_sas_token"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_sas_token must be of type string", "parameters[\"azure_blob_storage_sas_token\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_access_key") && !(parameters["azure_files_storage_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_access_key must be of type string", "parameters[\"azure_files_storage_access_key\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_sas_token") && !(parameters["azure_files_storage_sas_token"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_sas_token must be of type string", "parameters[\"azure_files_storage_sas_token\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_application_key") && !(parameters["backblaze_b2_application_key"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_application_key must be of type string", "parameters[\"backblaze_b2_application_key\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_key_id") && !(parameters["backblaze_b2_key_id"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_key_id must be of type string", "parameters[\"backblaze_b2_key_id\"]");
            }
            if (parameters.ContainsKey("cloudflare_secret_key") && !(parameters["cloudflare_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_secret_key must be of type string", "parameters[\"cloudflare_secret_key\"]");
            }
            if (parameters.ContainsKey("filebase_secret_key") && !(parameters["filebase_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_secret_key must be of type string", "parameters[\"filebase_secret_key\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_credentials_json") && !(parameters["google_cloud_storage_credentials_json"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_credentials_json must be of type string", "parameters[\"google_cloud_storage_credentials_json\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_secret_key") && !(parameters["google_cloud_storage_s3_compatible_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_secret_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_secret_key\"]");
            }
            if (parameters.ContainsKey("linode_secret_key") && !(parameters["linode_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_secret_key must be of type string", "parameters[\"linode_secret_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_secret_key") && !(parameters["s3_compatible_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_secret_key must be of type string", "parameters[\"s3_compatible_secret_key\"]");
            }
            if (parameters.ContainsKey("wasabi_secret_key") && !(parameters["wasabi_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_secret_key must be of type string", "parameters[\"wasabi_secret_key\"]");
            }
            if (parameters.ContainsKey("aws_access_key") && !(parameters["aws_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_access_key must be of type string", "parameters[\"aws_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_account") && !(parameters["azure_blob_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_account must be of type string", "parameters[\"azure_blob_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_container") && !(parameters["azure_blob_storage_container"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_container must be of type string", "parameters[\"azure_blob_storage_container\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_dns_suffix") && !(parameters["azure_blob_storage_dns_suffix"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_dns_suffix must be of type string", "parameters[\"azure_blob_storage_dns_suffix\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_hierarchical_namespace") && !(parameters["azure_blob_storage_hierarchical_namespace"] is bool))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_hierarchical_namespace must be of type bool", "parameters[\"azure_blob_storage_hierarchical_namespace\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_account") && !(parameters["azure_files_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_account must be of type string", "parameters[\"azure_files_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_dns_suffix") && !(parameters["azure_files_storage_dns_suffix"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_dns_suffix must be of type string", "parameters[\"azure_files_storage_dns_suffix\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_share_name") && !(parameters["azure_files_storage_share_name"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_share_name must be of type string", "parameters[\"azure_files_storage_share_name\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_bucket") && !(parameters["backblaze_b2_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_bucket must be of type string", "parameters[\"backblaze_b2_bucket\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_s3_endpoint") && !(parameters["backblaze_b2_s3_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_s3_endpoint must be of type string", "parameters[\"backblaze_b2_s3_endpoint\"]");
            }
            if (parameters.ContainsKey("cloudflare_access_key") && !(parameters["cloudflare_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_access_key must be of type string", "parameters[\"cloudflare_access_key\"]");
            }
            if (parameters.ContainsKey("cloudflare_bucket") && !(parameters["cloudflare_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_bucket must be of type string", "parameters[\"cloudflare_bucket\"]");
            }
            if (parameters.ContainsKey("cloudflare_endpoint") && !(parameters["cloudflare_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_endpoint must be of type string", "parameters[\"cloudflare_endpoint\"]");
            }
            if (parameters.ContainsKey("dropbox_teams") && !(parameters["dropbox_teams"] is bool))
            {
                throw new ArgumentException("Bad parameter: dropbox_teams must be of type bool", "parameters[\"dropbox_teams\"]");
            }
            if (parameters.ContainsKey("enable_dedicated_ips") && !(parameters["enable_dedicated_ips"] is bool))
            {
                throw new ArgumentException("Bad parameter: enable_dedicated_ips must be of type bool", "parameters[\"enable_dedicated_ips\"]");
            }
            if (parameters.ContainsKey("filebase_access_key") && !(parameters["filebase_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_access_key must be of type string", "parameters[\"filebase_access_key\"]");
            }
            if (parameters.ContainsKey("filebase_bucket") && !(parameters["filebase_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_bucket must be of type string", "parameters[\"filebase_bucket\"]");
            }
            if (parameters.ContainsKey("files_agent_permission_set") && !(parameters["files_agent_permission_set"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_permission_set must be of type string", "parameters[\"files_agent_permission_set\"]");
            }
            if (parameters.ContainsKey("files_agent_root") && !(parameters["files_agent_root"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_root must be of type string", "parameters[\"files_agent_root\"]");
            }
            if (parameters.ContainsKey("files_agent_version") && !(parameters["files_agent_version"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_version must be of type string", "parameters[\"files_agent_version\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_bucket") && !(parameters["google_cloud_storage_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_bucket must be of type string", "parameters[\"google_cloud_storage_bucket\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_project_id") && !(parameters["google_cloud_storage_project_id"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_project_id must be of type string", "parameters[\"google_cloud_storage_project_id\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_access_key") && !(parameters["google_cloud_storage_s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("hostname") && !(parameters["hostname"] is string))
            {
                throw new ArgumentException("Bad parameter: hostname must be of type string", "parameters[\"hostname\"]");
            }
            if (parameters.ContainsKey("linode_access_key") && !(parameters["linode_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_access_key must be of type string", "parameters[\"linode_access_key\"]");
            }
            if (parameters.ContainsKey("linode_bucket") && !(parameters["linode_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_bucket must be of type string", "parameters[\"linode_bucket\"]");
            }
            if (parameters.ContainsKey("linode_region") && !(parameters["linode_region"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_region must be of type string", "parameters[\"linode_region\"]");
            }
            if (parameters.ContainsKey("max_connections") && !(parameters["max_connections"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_connections must be of type Nullable<Int64>", "parameters[\"max_connections\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("one_drive_account_type") && !(parameters["one_drive_account_type"] is string))
            {
                throw new ArgumentException("Bad parameter: one_drive_account_type must be of type string", "parameters[\"one_drive_account_type\"]");
            }
            if (parameters.ContainsKey("pin_to_site_region") && !(parameters["pin_to_site_region"] is bool))
            {
                throw new ArgumentException("Bad parameter: pin_to_site_region must be of type bool", "parameters[\"pin_to_site_region\"]");
            }
            if (parameters.ContainsKey("port") && !(parameters["port"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: port must be of type Nullable<Int64>", "parameters[\"port\"]");
            }
            if (parameters.ContainsKey("s3_bucket") && !(parameters["s3_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_bucket must be of type string", "parameters[\"s3_bucket\"]");
            }
            if (parameters.ContainsKey("s3_compatible_access_key") && !(parameters["s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_access_key must be of type string", "parameters[\"s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_bucket") && !(parameters["s3_compatible_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_bucket must be of type string", "parameters[\"s3_compatible_bucket\"]");
            }
            if (parameters.ContainsKey("s3_compatible_endpoint") && !(parameters["s3_compatible_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_endpoint must be of type string", "parameters[\"s3_compatible_endpoint\"]");
            }
            if (parameters.ContainsKey("s3_compatible_region") && !(parameters["s3_compatible_region"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_region must be of type string", "parameters[\"s3_compatible_region\"]");
            }
            if (parameters.ContainsKey("s3_region") && !(parameters["s3_region"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_region must be of type string", "parameters[\"s3_region\"]");
            }
            if (parameters.ContainsKey("server_certificate") && !(parameters["server_certificate"] is string))
            {
                throw new ArgumentException("Bad parameter: server_certificate must be of type string", "parameters[\"server_certificate\"]");
            }
            if (parameters.ContainsKey("server_host_key") && !(parameters["server_host_key"] is string))
            {
                throw new ArgumentException("Bad parameter: server_host_key must be of type string", "parameters[\"server_host_key\"]");
            }
            if (parameters.ContainsKey("server_type") && !(parameters["server_type"] is string))
            {
                throw new ArgumentException("Bad parameter: server_type must be of type string", "parameters[\"server_type\"]");
            }
            if (parameters.ContainsKey("ssl") && !(parameters["ssl"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl must be of type string", "parameters[\"ssl\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (parameters.ContainsKey("wasabi_access_key") && !(parameters["wasabi_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_access_key must be of type string", "parameters[\"wasabi_access_key\"]");
            }
            if (parameters.ContainsKey("wasabi_bucket") && !(parameters["wasabi_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_bucket must be of type string", "parameters[\"wasabi_bucket\"]");
            }
            if (parameters.ContainsKey("wasabi_region") && !(parameters["wasabi_region"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_region must be of type string", "parameters[\"wasabi_region\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_servers/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServer>(responseJson);
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

            await FilesClient.SendRequest($"/remote_servers/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await RemoteServer.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `server_type`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `name`, `server_type`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`. Valid field combinations are `[ server_type, name ]`, `[ backblaze_b2_bucket, name ]`, `[ google_cloud_storage_bucket, name ]`, `[ wasabi_bucket, name ]`, `[ s3_bucket, name ]`, `[ azure_blob_storage_container, name ]`, `[ azure_files_storage_share_name, name ]`, `[ s3_compatible_bucket, name ]`, `[ filebase_bucket, name ]`, `[ cloudflare_bucket, name ]` or `[ linode_bucket, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`. Valid field combinations are `[ backblaze_b2_bucket, name ]`, `[ google_cloud_storage_bucket, name ]`, `[ wasabi_bucket, name ]`, `[ s3_bucket, name ]`, `[ azure_blob_storage_container, name ]`, `[ azure_files_storage_share_name, name ]`, `[ s3_compatible_bucket, name ]`, `[ filebase_bucket, name ]`, `[ cloudflare_bucket, name ]` or `[ linode_bucket, name ]`.
        /// </summary>
        public static FilesList<RemoteServer> List(

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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }

            return new FilesList<RemoteServer>($"/remote_servers", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<RemoteServer> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server ID.
        /// </summary>
        public static async Task<RemoteServer> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServer>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<RemoteServer> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server ID.
        /// </summary>
        public static async Task<RemoteServerConfigurationFile> FindConfigurationFile(
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

            string responseJson = await FilesClient.SendStringRequest($"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}/configuration_file", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServerConfigurationFile>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
        ///   reset_authentication - boolean - Reset authenticated account?
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
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   cloudflare_bucket - string - Cloudflare: Bucket name
        ///   cloudflare_endpoint - string - Cloudflare: endpoint
        ///   dropbox_teams - boolean - Dropbox: If true, list Team folders in root?
        ///   enable_dedicated_ips - boolean - `true` if remote server only accepts connections from dedicated IPs
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   filebase_bucket - string - Filebase: Bucket name
        ///   files_agent_permission_set - string - Local permissions for files agent. read_only, write_only, or read_write
        ///   files_agent_root - string - Agent local root path
        ///   files_agent_version - string - Files Agent version
        ///   google_cloud_storage_bucket - string - Google Cloud Storage: Bucket Name
        ///   google_cloud_storage_project_id - string - Google Cloud Storage: Project ID
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   hostname - string - Hostname or IP address
        ///   linode_access_key - string - Linode: Access Key
        ///   linode_bucket - string - Linode: Bucket name
        ///   linode_region - string - Linode: region
        ///   max_connections - int64 - Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        ///   name - string - Internal name for your reference
        ///   one_drive_account_type - string - OneDrive: Either personal or business_other account types
        ///   pin_to_site_region - boolean - If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        ///   port - int64 - Port for remote server.  Not needed for S3.
        ///   s3_bucket - string - S3 bucket name
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   s3_compatible_bucket - string - S3-compatible: Bucket name
        ///   s3_compatible_endpoint - string - S3-compatible: endpoint
        ///   s3_compatible_region - string - S3-compatible: region
        ///   s3_region - string - S3 region
        ///   server_certificate - string - Remote server certificate
        ///   server_host_key - string - Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
        ///   server_type - string - Remote server type.
        ///   ssl - string - Should we require SSL?
        ///   username - string - Remote server username.  Not needed for S3 buckets.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   wasabi_bucket - string - Wasabi: Bucket name
        ///   wasabi_region - string - Wasabi: Region
        /// </summary>
        public static async Task<RemoteServer> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_passphrase") && !(parameters["private_key_passphrase"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key_passphrase must be of type string", "parameters[\"private_key_passphrase\"]");
            }
            if (parameters.ContainsKey("reset_authentication") && !(parameters["reset_authentication"] is bool))
            {
                throw new ArgumentException("Bad parameter: reset_authentication must be of type bool", "parameters[\"reset_authentication\"]");
            }
            if (parameters.ContainsKey("ssl_certificate") && !(parameters["ssl_certificate"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl_certificate must be of type string", "parameters[\"ssl_certificate\"]");
            }
            if (parameters.ContainsKey("aws_secret_key") && !(parameters["aws_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_secret_key must be of type string", "parameters[\"aws_secret_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_access_key") && !(parameters["azure_blob_storage_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_access_key must be of type string", "parameters[\"azure_blob_storage_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_sas_token") && !(parameters["azure_blob_storage_sas_token"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_sas_token must be of type string", "parameters[\"azure_blob_storage_sas_token\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_access_key") && !(parameters["azure_files_storage_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_access_key must be of type string", "parameters[\"azure_files_storage_access_key\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_sas_token") && !(parameters["azure_files_storage_sas_token"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_sas_token must be of type string", "parameters[\"azure_files_storage_sas_token\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_application_key") && !(parameters["backblaze_b2_application_key"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_application_key must be of type string", "parameters[\"backblaze_b2_application_key\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_key_id") && !(parameters["backblaze_b2_key_id"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_key_id must be of type string", "parameters[\"backblaze_b2_key_id\"]");
            }
            if (parameters.ContainsKey("cloudflare_secret_key") && !(parameters["cloudflare_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_secret_key must be of type string", "parameters[\"cloudflare_secret_key\"]");
            }
            if (parameters.ContainsKey("filebase_secret_key") && !(parameters["filebase_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_secret_key must be of type string", "parameters[\"filebase_secret_key\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_credentials_json") && !(parameters["google_cloud_storage_credentials_json"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_credentials_json must be of type string", "parameters[\"google_cloud_storage_credentials_json\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_secret_key") && !(parameters["google_cloud_storage_s3_compatible_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_secret_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_secret_key\"]");
            }
            if (parameters.ContainsKey("linode_secret_key") && !(parameters["linode_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_secret_key must be of type string", "parameters[\"linode_secret_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_secret_key") && !(parameters["s3_compatible_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_secret_key must be of type string", "parameters[\"s3_compatible_secret_key\"]");
            }
            if (parameters.ContainsKey("wasabi_secret_key") && !(parameters["wasabi_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_secret_key must be of type string", "parameters[\"wasabi_secret_key\"]");
            }
            if (parameters.ContainsKey("aws_access_key") && !(parameters["aws_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_access_key must be of type string", "parameters[\"aws_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_account") && !(parameters["azure_blob_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_account must be of type string", "parameters[\"azure_blob_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_container") && !(parameters["azure_blob_storage_container"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_container must be of type string", "parameters[\"azure_blob_storage_container\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_dns_suffix") && !(parameters["azure_blob_storage_dns_suffix"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_dns_suffix must be of type string", "parameters[\"azure_blob_storage_dns_suffix\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_hierarchical_namespace") && !(parameters["azure_blob_storage_hierarchical_namespace"] is bool))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_hierarchical_namespace must be of type bool", "parameters[\"azure_blob_storage_hierarchical_namespace\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_account") && !(parameters["azure_files_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_account must be of type string", "parameters[\"azure_files_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_dns_suffix") && !(parameters["azure_files_storage_dns_suffix"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_dns_suffix must be of type string", "parameters[\"azure_files_storage_dns_suffix\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_share_name") && !(parameters["azure_files_storage_share_name"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_share_name must be of type string", "parameters[\"azure_files_storage_share_name\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_bucket") && !(parameters["backblaze_b2_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_bucket must be of type string", "parameters[\"backblaze_b2_bucket\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_s3_endpoint") && !(parameters["backblaze_b2_s3_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_s3_endpoint must be of type string", "parameters[\"backblaze_b2_s3_endpoint\"]");
            }
            if (parameters.ContainsKey("cloudflare_access_key") && !(parameters["cloudflare_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_access_key must be of type string", "parameters[\"cloudflare_access_key\"]");
            }
            if (parameters.ContainsKey("cloudflare_bucket") && !(parameters["cloudflare_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_bucket must be of type string", "parameters[\"cloudflare_bucket\"]");
            }
            if (parameters.ContainsKey("cloudflare_endpoint") && !(parameters["cloudflare_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_endpoint must be of type string", "parameters[\"cloudflare_endpoint\"]");
            }
            if (parameters.ContainsKey("dropbox_teams") && !(parameters["dropbox_teams"] is bool))
            {
                throw new ArgumentException("Bad parameter: dropbox_teams must be of type bool", "parameters[\"dropbox_teams\"]");
            }
            if (parameters.ContainsKey("enable_dedicated_ips") && !(parameters["enable_dedicated_ips"] is bool))
            {
                throw new ArgumentException("Bad parameter: enable_dedicated_ips must be of type bool", "parameters[\"enable_dedicated_ips\"]");
            }
            if (parameters.ContainsKey("filebase_access_key") && !(parameters["filebase_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_access_key must be of type string", "parameters[\"filebase_access_key\"]");
            }
            if (parameters.ContainsKey("filebase_bucket") && !(parameters["filebase_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_bucket must be of type string", "parameters[\"filebase_bucket\"]");
            }
            if (parameters.ContainsKey("files_agent_permission_set") && !(parameters["files_agent_permission_set"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_permission_set must be of type string", "parameters[\"files_agent_permission_set\"]");
            }
            if (parameters.ContainsKey("files_agent_root") && !(parameters["files_agent_root"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_root must be of type string", "parameters[\"files_agent_root\"]");
            }
            if (parameters.ContainsKey("files_agent_version") && !(parameters["files_agent_version"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_version must be of type string", "parameters[\"files_agent_version\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_bucket") && !(parameters["google_cloud_storage_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_bucket must be of type string", "parameters[\"google_cloud_storage_bucket\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_project_id") && !(parameters["google_cloud_storage_project_id"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_project_id must be of type string", "parameters[\"google_cloud_storage_project_id\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_access_key") && !(parameters["google_cloud_storage_s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("hostname") && !(parameters["hostname"] is string))
            {
                throw new ArgumentException("Bad parameter: hostname must be of type string", "parameters[\"hostname\"]");
            }
            if (parameters.ContainsKey("linode_access_key") && !(parameters["linode_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_access_key must be of type string", "parameters[\"linode_access_key\"]");
            }
            if (parameters.ContainsKey("linode_bucket") && !(parameters["linode_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_bucket must be of type string", "parameters[\"linode_bucket\"]");
            }
            if (parameters.ContainsKey("linode_region") && !(parameters["linode_region"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_region must be of type string", "parameters[\"linode_region\"]");
            }
            if (parameters.ContainsKey("max_connections") && !(parameters["max_connections"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_connections must be of type Nullable<Int64>", "parameters[\"max_connections\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("one_drive_account_type") && !(parameters["one_drive_account_type"] is string))
            {
                throw new ArgumentException("Bad parameter: one_drive_account_type must be of type string", "parameters[\"one_drive_account_type\"]");
            }
            if (parameters.ContainsKey("pin_to_site_region") && !(parameters["pin_to_site_region"] is bool))
            {
                throw new ArgumentException("Bad parameter: pin_to_site_region must be of type bool", "parameters[\"pin_to_site_region\"]");
            }
            if (parameters.ContainsKey("port") && !(parameters["port"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: port must be of type Nullable<Int64>", "parameters[\"port\"]");
            }
            if (parameters.ContainsKey("s3_bucket") && !(parameters["s3_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_bucket must be of type string", "parameters[\"s3_bucket\"]");
            }
            if (parameters.ContainsKey("s3_compatible_access_key") && !(parameters["s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_access_key must be of type string", "parameters[\"s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_bucket") && !(parameters["s3_compatible_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_bucket must be of type string", "parameters[\"s3_compatible_bucket\"]");
            }
            if (parameters.ContainsKey("s3_compatible_endpoint") && !(parameters["s3_compatible_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_endpoint must be of type string", "parameters[\"s3_compatible_endpoint\"]");
            }
            if (parameters.ContainsKey("s3_compatible_region") && !(parameters["s3_compatible_region"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_region must be of type string", "parameters[\"s3_compatible_region\"]");
            }
            if (parameters.ContainsKey("s3_region") && !(parameters["s3_region"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_region must be of type string", "parameters[\"s3_region\"]");
            }
            if (parameters.ContainsKey("server_certificate") && !(parameters["server_certificate"] is string))
            {
                throw new ArgumentException("Bad parameter: server_certificate must be of type string", "parameters[\"server_certificate\"]");
            }
            if (parameters.ContainsKey("server_host_key") && !(parameters["server_host_key"] is string))
            {
                throw new ArgumentException("Bad parameter: server_host_key must be of type string", "parameters[\"server_host_key\"]");
            }
            if (parameters.ContainsKey("server_type") && !(parameters["server_type"] is string))
            {
                throw new ArgumentException("Bad parameter: server_type must be of type string", "parameters[\"server_type\"]");
            }
            if (parameters.ContainsKey("ssl") && !(parameters["ssl"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl must be of type string", "parameters[\"ssl\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (parameters.ContainsKey("wasabi_access_key") && !(parameters["wasabi_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_access_key must be of type string", "parameters[\"wasabi_access_key\"]");
            }
            if (parameters.ContainsKey("wasabi_bucket") && !(parameters["wasabi_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_bucket must be of type string", "parameters[\"wasabi_bucket\"]");
            }
            if (parameters.ContainsKey("wasabi_region") && !(parameters["wasabi_region"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_region must be of type string", "parameters[\"wasabi_region\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_servers", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServer>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Post local changes, check in, and download configuration file (used by some Remote Server integrations, such as the Files.com Agent)
        ///
        /// Parameters:
        ///   api_token - string - Files Agent API Token
        ///   permission_set - string - The permission set for the agent ['read_write', 'read_only', 'write_only']
        ///   root - string - The root directory for the agent
        ///   hostname - string
        ///   port - int64 - Incoming port for files agent connections
        ///   status - string - either running or shutdown
        ///   config_version - string - agent config version
        ///   private_key - string - The private key for the agent
        ///   public_key - string - public key
        ///   server_host_key - string
        ///   subdomain - string - Files.com subdomain site name
        /// </summary>
        public static async Task<RemoteServerConfigurationFile> ConfigurationFile(
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
            if (parameters.ContainsKey("api_token") && !(parameters["api_token"] is string))
            {
                throw new ArgumentException("Bad parameter: api_token must be of type string", "parameters[\"api_token\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }
            if (parameters.ContainsKey("root") && !(parameters["root"] is string))
            {
                throw new ArgumentException("Bad parameter: root must be of type string", "parameters[\"root\"]");
            }
            if (parameters.ContainsKey("hostname") && !(parameters["hostname"] is string))
            {
                throw new ArgumentException("Bad parameter: hostname must be of type string", "parameters[\"hostname\"]");
            }
            if (parameters.ContainsKey("port") && !(parameters["port"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: port must be of type Nullable<Int64>", "parameters[\"port\"]");
            }
            if (parameters.ContainsKey("status") && !(parameters["status"] is string))
            {
                throw new ArgumentException("Bad parameter: status must be of type string", "parameters[\"status\"]");
            }
            if (parameters.ContainsKey("config_version") && !(parameters["config_version"] is string))
            {
                throw new ArgumentException("Bad parameter: config_version must be of type string", "parameters[\"config_version\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("public_key") && !(parameters["public_key"] is string))
            {
                throw new ArgumentException("Bad parameter: public_key must be of type string", "parameters[\"public_key\"]");
            }
            if (parameters.ContainsKey("server_host_key") && !(parameters["server_host_key"] is string))
            {
                throw new ArgumentException("Bad parameter: server_host_key must be of type string", "parameters[\"server_host_key\"]");
            }
            if (parameters.ContainsKey("subdomain") && !(parameters["subdomain"] is string))
            {
                throw new ArgumentException("Bad parameter: subdomain must be of type string", "parameters[\"subdomain\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}/configuration_file", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServerConfigurationFile>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
        ///   reset_authentication - boolean - Reset authenticated account?
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
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   cloudflare_bucket - string - Cloudflare: Bucket name
        ///   cloudflare_endpoint - string - Cloudflare: endpoint
        ///   dropbox_teams - boolean - Dropbox: If true, list Team folders in root?
        ///   enable_dedicated_ips - boolean - `true` if remote server only accepts connections from dedicated IPs
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   filebase_bucket - string - Filebase: Bucket name
        ///   files_agent_permission_set - string - Local permissions for files agent. read_only, write_only, or read_write
        ///   files_agent_root - string - Agent local root path
        ///   files_agent_version - string - Files Agent version
        ///   google_cloud_storage_bucket - string - Google Cloud Storage: Bucket Name
        ///   google_cloud_storage_project_id - string - Google Cloud Storage: Project ID
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   hostname - string - Hostname or IP address
        ///   linode_access_key - string - Linode: Access Key
        ///   linode_bucket - string - Linode: Bucket name
        ///   linode_region - string - Linode: region
        ///   max_connections - int64 - Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
        ///   name - string - Internal name for your reference
        ///   one_drive_account_type - string - OneDrive: Either personal or business_other account types
        ///   pin_to_site_region - boolean - If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
        ///   port - int64 - Port for remote server.  Not needed for S3.
        ///   s3_bucket - string - S3 bucket name
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   s3_compatible_bucket - string - S3-compatible: Bucket name
        ///   s3_compatible_endpoint - string - S3-compatible: endpoint
        ///   s3_compatible_region - string - S3-compatible: region
        ///   s3_region - string - S3 region
        ///   server_certificate - string - Remote server certificate
        ///   server_host_key - string - Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
        ///   server_type - string - Remote server type.
        ///   ssl - string - Should we require SSL?
        ///   username - string - Remote server username.  Not needed for S3 buckets.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   wasabi_bucket - string - Wasabi: Bucket name
        ///   wasabi_region - string - Wasabi: Region
        /// </summary>
        public static async Task<RemoteServer> Update(
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
            if (parameters.ContainsKey("password") && !(parameters["password"] is string))
            {
                throw new ArgumentException("Bad parameter: password must be of type string", "parameters[\"password\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_passphrase") && !(parameters["private_key_passphrase"] is string))
            {
                throw new ArgumentException("Bad parameter: private_key_passphrase must be of type string", "parameters[\"private_key_passphrase\"]");
            }
            if (parameters.ContainsKey("reset_authentication") && !(parameters["reset_authentication"] is bool))
            {
                throw new ArgumentException("Bad parameter: reset_authentication must be of type bool", "parameters[\"reset_authentication\"]");
            }
            if (parameters.ContainsKey("ssl_certificate") && !(parameters["ssl_certificate"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl_certificate must be of type string", "parameters[\"ssl_certificate\"]");
            }
            if (parameters.ContainsKey("aws_secret_key") && !(parameters["aws_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_secret_key must be of type string", "parameters[\"aws_secret_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_access_key") && !(parameters["azure_blob_storage_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_access_key must be of type string", "parameters[\"azure_blob_storage_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_sas_token") && !(parameters["azure_blob_storage_sas_token"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_sas_token must be of type string", "parameters[\"azure_blob_storage_sas_token\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_access_key") && !(parameters["azure_files_storage_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_access_key must be of type string", "parameters[\"azure_files_storage_access_key\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_sas_token") && !(parameters["azure_files_storage_sas_token"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_sas_token must be of type string", "parameters[\"azure_files_storage_sas_token\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_application_key") && !(parameters["backblaze_b2_application_key"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_application_key must be of type string", "parameters[\"backblaze_b2_application_key\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_key_id") && !(parameters["backblaze_b2_key_id"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_key_id must be of type string", "parameters[\"backblaze_b2_key_id\"]");
            }
            if (parameters.ContainsKey("cloudflare_secret_key") && !(parameters["cloudflare_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_secret_key must be of type string", "parameters[\"cloudflare_secret_key\"]");
            }
            if (parameters.ContainsKey("filebase_secret_key") && !(parameters["filebase_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_secret_key must be of type string", "parameters[\"filebase_secret_key\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_credentials_json") && !(parameters["google_cloud_storage_credentials_json"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_credentials_json must be of type string", "parameters[\"google_cloud_storage_credentials_json\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_secret_key") && !(parameters["google_cloud_storage_s3_compatible_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_secret_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_secret_key\"]");
            }
            if (parameters.ContainsKey("linode_secret_key") && !(parameters["linode_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_secret_key must be of type string", "parameters[\"linode_secret_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_secret_key") && !(parameters["s3_compatible_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_secret_key must be of type string", "parameters[\"s3_compatible_secret_key\"]");
            }
            if (parameters.ContainsKey("wasabi_secret_key") && !(parameters["wasabi_secret_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_secret_key must be of type string", "parameters[\"wasabi_secret_key\"]");
            }
            if (parameters.ContainsKey("aws_access_key") && !(parameters["aws_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_access_key must be of type string", "parameters[\"aws_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_account") && !(parameters["azure_blob_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_account must be of type string", "parameters[\"azure_blob_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_container") && !(parameters["azure_blob_storage_container"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_container must be of type string", "parameters[\"azure_blob_storage_container\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_dns_suffix") && !(parameters["azure_blob_storage_dns_suffix"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_dns_suffix must be of type string", "parameters[\"azure_blob_storage_dns_suffix\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_hierarchical_namespace") && !(parameters["azure_blob_storage_hierarchical_namespace"] is bool))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_hierarchical_namespace must be of type bool", "parameters[\"azure_blob_storage_hierarchical_namespace\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_account") && !(parameters["azure_files_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_account must be of type string", "parameters[\"azure_files_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_dns_suffix") && !(parameters["azure_files_storage_dns_suffix"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_dns_suffix must be of type string", "parameters[\"azure_files_storage_dns_suffix\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_share_name") && !(parameters["azure_files_storage_share_name"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_share_name must be of type string", "parameters[\"azure_files_storage_share_name\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_bucket") && !(parameters["backblaze_b2_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_bucket must be of type string", "parameters[\"backblaze_b2_bucket\"]");
            }
            if (parameters.ContainsKey("backblaze_b2_s3_endpoint") && !(parameters["backblaze_b2_s3_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: backblaze_b2_s3_endpoint must be of type string", "parameters[\"backblaze_b2_s3_endpoint\"]");
            }
            if (parameters.ContainsKey("cloudflare_access_key") && !(parameters["cloudflare_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_access_key must be of type string", "parameters[\"cloudflare_access_key\"]");
            }
            if (parameters.ContainsKey("cloudflare_bucket") && !(parameters["cloudflare_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_bucket must be of type string", "parameters[\"cloudflare_bucket\"]");
            }
            if (parameters.ContainsKey("cloudflare_endpoint") && !(parameters["cloudflare_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_endpoint must be of type string", "parameters[\"cloudflare_endpoint\"]");
            }
            if (parameters.ContainsKey("dropbox_teams") && !(parameters["dropbox_teams"] is bool))
            {
                throw new ArgumentException("Bad parameter: dropbox_teams must be of type bool", "parameters[\"dropbox_teams\"]");
            }
            if (parameters.ContainsKey("enable_dedicated_ips") && !(parameters["enable_dedicated_ips"] is bool))
            {
                throw new ArgumentException("Bad parameter: enable_dedicated_ips must be of type bool", "parameters[\"enable_dedicated_ips\"]");
            }
            if (parameters.ContainsKey("filebase_access_key") && !(parameters["filebase_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_access_key must be of type string", "parameters[\"filebase_access_key\"]");
            }
            if (parameters.ContainsKey("filebase_bucket") && !(parameters["filebase_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_bucket must be of type string", "parameters[\"filebase_bucket\"]");
            }
            if (parameters.ContainsKey("files_agent_permission_set") && !(parameters["files_agent_permission_set"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_permission_set must be of type string", "parameters[\"files_agent_permission_set\"]");
            }
            if (parameters.ContainsKey("files_agent_root") && !(parameters["files_agent_root"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_root must be of type string", "parameters[\"files_agent_root\"]");
            }
            if (parameters.ContainsKey("files_agent_version") && !(parameters["files_agent_version"] is string))
            {
                throw new ArgumentException("Bad parameter: files_agent_version must be of type string", "parameters[\"files_agent_version\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_bucket") && !(parameters["google_cloud_storage_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_bucket must be of type string", "parameters[\"google_cloud_storage_bucket\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_project_id") && !(parameters["google_cloud_storage_project_id"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_project_id must be of type string", "parameters[\"google_cloud_storage_project_id\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_access_key") && !(parameters["google_cloud_storage_s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("hostname") && !(parameters["hostname"] is string))
            {
                throw new ArgumentException("Bad parameter: hostname must be of type string", "parameters[\"hostname\"]");
            }
            if (parameters.ContainsKey("linode_access_key") && !(parameters["linode_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_access_key must be of type string", "parameters[\"linode_access_key\"]");
            }
            if (parameters.ContainsKey("linode_bucket") && !(parameters["linode_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_bucket must be of type string", "parameters[\"linode_bucket\"]");
            }
            if (parameters.ContainsKey("linode_region") && !(parameters["linode_region"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_region must be of type string", "parameters[\"linode_region\"]");
            }
            if (parameters.ContainsKey("max_connections") && !(parameters["max_connections"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_connections must be of type Nullable<Int64>", "parameters[\"max_connections\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("one_drive_account_type") && !(parameters["one_drive_account_type"] is string))
            {
                throw new ArgumentException("Bad parameter: one_drive_account_type must be of type string", "parameters[\"one_drive_account_type\"]");
            }
            if (parameters.ContainsKey("pin_to_site_region") && !(parameters["pin_to_site_region"] is bool))
            {
                throw new ArgumentException("Bad parameter: pin_to_site_region must be of type bool", "parameters[\"pin_to_site_region\"]");
            }
            if (parameters.ContainsKey("port") && !(parameters["port"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: port must be of type Nullable<Int64>", "parameters[\"port\"]");
            }
            if (parameters.ContainsKey("s3_bucket") && !(parameters["s3_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_bucket must be of type string", "parameters[\"s3_bucket\"]");
            }
            if (parameters.ContainsKey("s3_compatible_access_key") && !(parameters["s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_access_key must be of type string", "parameters[\"s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_bucket") && !(parameters["s3_compatible_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_bucket must be of type string", "parameters[\"s3_compatible_bucket\"]");
            }
            if (parameters.ContainsKey("s3_compatible_endpoint") && !(parameters["s3_compatible_endpoint"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_endpoint must be of type string", "parameters[\"s3_compatible_endpoint\"]");
            }
            if (parameters.ContainsKey("s3_compatible_region") && !(parameters["s3_compatible_region"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_region must be of type string", "parameters[\"s3_compatible_region\"]");
            }
            if (parameters.ContainsKey("s3_region") && !(parameters["s3_region"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_region must be of type string", "parameters[\"s3_region\"]");
            }
            if (parameters.ContainsKey("server_certificate") && !(parameters["server_certificate"] is string))
            {
                throw new ArgumentException("Bad parameter: server_certificate must be of type string", "parameters[\"server_certificate\"]");
            }
            if (parameters.ContainsKey("server_host_key") && !(parameters["server_host_key"] is string))
            {
                throw new ArgumentException("Bad parameter: server_host_key must be of type string", "parameters[\"server_host_key\"]");
            }
            if (parameters.ContainsKey("server_type") && !(parameters["server_type"] is string))
            {
                throw new ArgumentException("Bad parameter: server_type must be of type string", "parameters[\"server_type\"]");
            }
            if (parameters.ContainsKey("ssl") && !(parameters["ssl"] is string))
            {
                throw new ArgumentException("Bad parameter: ssl must be of type string", "parameters[\"ssl\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (parameters.ContainsKey("wasabi_access_key") && !(parameters["wasabi_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_access_key must be of type string", "parameters[\"wasabi_access_key\"]");
            }
            if (parameters.ContainsKey("wasabi_bucket") && !(parameters["wasabi_bucket"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_bucket must be of type string", "parameters[\"wasabi_bucket\"]");
            }
            if (parameters.ContainsKey("wasabi_region") && !(parameters["wasabi_region"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_region must be of type string", "parameters[\"wasabi_region\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServer>(responseJson);
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

            await FilesClient.SendRequest($"/remote_servers/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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