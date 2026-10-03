using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Operations
{
    // Imported inside the namespace so that model names, such as Action, take precedence over System's.
    using FilesCom.Models;

    /// <summary>
    /// RemoteServer operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.RemoteServers"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="RemoteServer"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class RemoteServerOperations
    {
        private readonly FilesClient client;

        internal RemoteServerOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a RemoteServer that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public RemoteServer New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            RemoteServer model = new RemoteServer(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
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
        public FilesList<RemoteServer> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return RemoteServer.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
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
        public FilesList<RemoteServer> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return RemoteServer.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server ID.
        /// </summary>
        public Task<RemoteServer> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server ID.
        /// </summary>
        public Task<RemoteServer> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// List Files.com Agent nodes
        /// </summary>
        public Task<AgentNode> AgentNodesAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.AgentNodesCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server ID.
        /// </summary>
        public Task<RemoteServerConfigurationFile> FindConfigurationFileAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.FindConfigurationFileCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
        public Task<RemoteServer> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Push update to Files Agent
        /// </summary>
        public Task<AgentPushUpdate> AgentPushUpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.AgentPushUpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
        public Task<RemoteServer> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// </summary>
        public Task DeleteAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// </summary>
        public Task DestroyAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteServer.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}