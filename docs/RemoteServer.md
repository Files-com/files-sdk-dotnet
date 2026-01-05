# FilesCom.Models.RemoteServer

## Example RemoteServer Object

```
{
  "id": 1,
  "disabled": true,
  "authentication_method": "password",
  "hostname": "remote-server.com",
  "remote_home_path": "/home/user1",
  "name": "My Remote server",
  "description": "More information or notes about my server",
  "port": 1,
  "buffer_uploads": "example",
  "max_connections": 1,
  "pin_to_site_region": true,
  "pinned_region": "us-east-1",
  "remote_server_credential_id": 1,
  "s3_bucket": "my-bucket",
  "s3_region": "us-east-1",
  "aws_access_key": "example",
  "server_certificate": "require_match",
  "server_host_key": "[public key]",
  "server_type": "s3",
  "workspace_id": 1,
  "ssl": "if_available",
  "username": "user",
  "google_cloud_storage_bucket": "my-bucket",
  "google_cloud_storage_project_id": "my-project",
  "google_cloud_storage_s3_compatible_access_key": "example",
  "backblaze_b2_s3_endpoint": "s3.us-west-001.backblazeb2.com",
  "backblaze_b2_bucket": "my-bucket",
  "wasabi_bucket": "my-bucket",
  "wasabi_region": "us-west-1",
  "wasabi_access_key": "example",
  "auth_status": "in_setup",
  "auth_account_name": "me@example.com",
  "one_drive_account_type": "personal",
  "azure_blob_storage_account": "storage-account-name",
  "azure_blob_storage_container": "container-name",
  "azure_blob_storage_hierarchical_namespace": true,
  "azure_blob_storage_dns_suffix": "usgovcloudapi.net",
  "azure_files_storage_account": "storage-account-name",
  "azure_files_storage_share_name": "share-name",
  "azure_files_storage_dns_suffix": "file.core.windows.net",
  "s3_compatible_bucket": "my-bucket",
  "s3_compatible_endpoint": "mys3platform.com",
  "s3_compatible_region": "us-east-1",
  "s3_compatible_access_key": "example",
  "enable_dedicated_ips": true,
  "files_agent_permission_set": "read_write",
  "files_agent_root": "example",
  "files_agent_api_token": "example",
  "files_agent_version": "example",
  "files_agent_up_to_date": true,
  "files_agent_latest_version": "example",
  "files_agent_supports_push_updates": true,
  "outbound_agent_id": 1,
  "filebase_bucket": "my-bucket",
  "filebase_access_key": "example",
  "cloudflare_bucket": "my-bucket",
  "cloudflare_access_key": "example",
  "cloudflare_endpoint": "https://<ACCOUNT_ID>.r2.cloudflarestorage.com",
  "dropbox_teams": true,
  "linode_bucket": "my-bucket",
  "linode_access_key": "example",
  "linode_region": "us-east-1",
  "supports_versioning": true
}
```

* `id` / `Id`  (Nullable<Int64>): Remote server ID
* `disabled` / `Disabled`  (bool): If true, this server has been disabled due to failures.  Make any change or set disabled to false to clear this flag.
* `authentication_method` / `AuthenticationMethod`  (string): Type of authentication method
* `hostname` / `Hostname`  (string): Hostname or IP address
* `remote_home_path` / `RemoteHomePath`  (string): Initial home folder on remote server
* `name` / `Name`  (string): Internal name for your reference
* `description` / `Description`  (string): Internal description for your reference
* `port` / `Port`  (Nullable<Int64>): Port for remote server.
* `buffer_uploads` / `BufferUploads`  (string): If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
* `max_connections` / `MaxConnections`  (Nullable<Int64>): Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
* `pin_to_site_region` / `PinToSiteRegion`  (bool): If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
* `pinned_region` / `PinnedRegion`  (string): If set, all communications with this remote server are made through the provided region.
* `remote_server_credential_id` / `RemoteServerCredentialId`  (Nullable<Int64>): ID of Remote Server Credential, if applicable.
* `s3_bucket` / `S3Bucket`  (string): S3 bucket name
* `s3_region` / `S3Region`  (string): S3 region
* `aws_access_key` / `AwsAccessKey`  (string): AWS Access Key.
* `server_certificate` / `ServerCertificate`  (string): Remote server certificate
* `server_host_key` / `ServerHostKey`  (string): Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
* `server_type` / `ServerType`  (string): Remote server type.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID (0 for default workspace)
* `ssl` / `Ssl`  (string): Should we require SSL?
* `username` / `Username`  (string): Remote server username.
* `google_cloud_storage_bucket` / `GoogleCloudStorageBucket`  (string): Google Cloud Storage: Bucket Name
* `google_cloud_storage_project_id` / `GoogleCloudStorageProjectId`  (string): Google Cloud Storage: Project ID
* `google_cloud_storage_s3_compatible_access_key` / `GoogleCloudStorageS3CompatibleAccessKey`  (string): Google Cloud Storage: S3-compatible Access Key.
* `backblaze_b2_s3_endpoint` / `BackblazeB2S3Endpoint`  (string): Backblaze B2 Cloud Storage: S3 Endpoint
* `backblaze_b2_bucket` / `BackblazeB2Bucket`  (string): Backblaze B2 Cloud Storage: Bucket name
* `wasabi_bucket` / `WasabiBucket`  (string): Wasabi: Bucket name
* `wasabi_region` / `WasabiRegion`  (string): Wasabi: Region
* `wasabi_access_key` / `WasabiAccessKey`  (string): Wasabi: Access Key.
* `auth_status` / `AuthStatus`  (string): Either `in_setup` or `complete`
* `auth_account_name` / `AuthAccountName`  (string): Describes the authorized account
* `one_drive_account_type` / `OneDriveAccountType`  (string): OneDrive: Either personal or business_other account types
* `azure_blob_storage_account` / `AzureBlobStorageAccount`  (string): Azure Blob Storage: Account name
* `azure_blob_storage_container` / `AzureBlobStorageContainer`  (string): Azure Blob Storage: Container name
* `azure_blob_storage_hierarchical_namespace` / `AzureBlobStorageHierarchicalNamespace`  (bool): Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
* `azure_blob_storage_dns_suffix` / `AzureBlobStorageDnsSuffix`  (string): Azure Blob Storage: Custom DNS suffix
* `azure_files_storage_account` / `AzureFilesStorageAccount`  (string): Azure Files: Storage Account name
* `azure_files_storage_share_name` / `AzureFilesStorageShareName`  (string): Azure Files:  Storage Share name
* `azure_files_storage_dns_suffix` / `AzureFilesStorageDnsSuffix`  (string): Azure Files: Custom DNS suffix
* `s3_compatible_bucket` / `S3CompatibleBucket`  (string): S3-compatible: Bucket name
* `s3_compatible_endpoint` / `S3CompatibleEndpoint`  (string): S3-compatible: endpoint
* `s3_compatible_region` / `S3CompatibleRegion`  (string): S3-compatible: region
* `s3_compatible_access_key` / `S3CompatibleAccessKey`  (string): S3-compatible: Access Key
* `enable_dedicated_ips` / `EnableDedicatedIps`  (bool): `true` if remote server only accepts connections from dedicated IPs
* `files_agent_permission_set` / `FilesAgentPermissionSet`  (string): Local permissions for files agent. read_only, write_only, or read_write
* `files_agent_root` / `FilesAgentRoot`  (string): Agent local root path
* `files_agent_api_token` / `FilesAgentApiToken`  (string): Files Agent API Token
* `files_agent_version` / `FilesAgentVersion`  (string): Files Agent version
* `files_agent_up_to_date` / `FilesAgentUpToDate`  (bool): If true, the Files Agent is up to date.
* `files_agent_latest_version` / `FilesAgentLatestVersion`  (string): Latest available Files Agent version
* `files_agent_supports_push_updates` / `FilesAgentSupportsPushUpdates`  (bool): Files Agent supports receiving push updates
* `outbound_agent_id` / `OutboundAgentId`  (Nullable<Int64>): Route traffic to outbound on a files-agent
* `filebase_bucket` / `FilebaseBucket`  (string): Filebase: Bucket name
* `filebase_access_key` / `FilebaseAccessKey`  (string): Filebase: Access Key.
* `cloudflare_bucket` / `CloudflareBucket`  (string): Cloudflare: Bucket name
* `cloudflare_access_key` / `CloudflareAccessKey`  (string): Cloudflare: Access Key.
* `cloudflare_endpoint` / `CloudflareEndpoint`  (string): Cloudflare: endpoint
* `dropbox_teams` / `DropboxTeams`  (bool): Dropbox: If true, list Team folders in root?
* `linode_bucket` / `LinodeBucket`  (string): Linode: Bucket name
* `linode_access_key` / `LinodeAccessKey`  (string): Linode: Access Key
* `linode_region` / `LinodeRegion`  (string): Linode: region
* `supports_versioning` / `SupportsVersioning`  (bool): If true, this remote server supports file versioning. This value is determined automatically by Files.com.
* `password` / `Password`  (string): Password, if needed.
* `private_key` / `PrivateKey`  (string): Private key, if needed.
* `private_key_passphrase` / `PrivateKeyPassphrase`  (string): Passphrase for private key if needed.
* `reset_authentication` / `ResetAuthentication`  (bool): Reset authenticated account?
* `ssl_certificate` / `SslCertificate`  (string): SSL client certificate.
* `aws_secret_key` / `AwsSecretKey`  (string): AWS: secret key.
* `azure_blob_storage_access_key` / `AzureBlobStorageAccessKey`  (string): Azure Blob Storage: Access Key
* `azure_blob_storage_sas_token` / `AzureBlobStorageSasToken`  (string): Azure Blob Storage: Shared Access Signature (SAS) token
* `azure_files_storage_access_key` / `AzureFilesStorageAccessKey`  (string): Azure File Storage: Access Key
* `azure_files_storage_sas_token` / `AzureFilesStorageSasToken`  (string): Azure File Storage: Shared Access Signature (SAS) token
* `backblaze_b2_application_key` / `BackblazeB2ApplicationKey`  (string): Backblaze B2 Cloud Storage: applicationKey
* `backblaze_b2_key_id` / `BackblazeB2KeyId`  (string): Backblaze B2 Cloud Storage: keyID
* `cloudflare_secret_key` / `CloudflareSecretKey`  (string): Cloudflare: Secret Key
* `filebase_secret_key` / `FilebaseSecretKey`  (string): Filebase: Secret Key
* `google_cloud_storage_credentials_json` / `GoogleCloudStorageCredentialsJson`  (string): Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
* `google_cloud_storage_s3_compatible_secret_key` / `GoogleCloudStorageS3CompatibleSecretKey`  (string): Google Cloud Storage: S3-compatible secret key
* `linode_secret_key` / `LinodeSecretKey`  (string): Linode: Secret Key
* `s3_compatible_secret_key` / `S3CompatibleSecretKey`  (string): S3-compatible: Secret Key
* `wasabi_secret_key` / `WasabiSecretKey`  (string): Wasabi: Secret Key


---

## List Remote Servers

```
Task<FilesList<RemoteServer>> RemoteServer.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name`, `server_type`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `name`, `server_type`, `workspace_id`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`. Valid field combinations are `[ server_type, name ]`, `[ workspace_id, name ]`, `[ backblaze_b2_bucket, name ]`, `[ google_cloud_storage_bucket, name ]`, `[ wasabi_bucket, name ]`, `[ s3_bucket, name ]`, `[ azure_blob_storage_container, name ]`, `[ azure_files_storage_share_name, name ]`, `[ s3_compatible_bucket, name ]`, `[ filebase_bucket, name ]`, `[ cloudflare_bucket, name ]`, `[ linode_bucket, name ]`, `[ workspace_id, server_type ]` or `[ workspace_id, server_type, name ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`, `backblaze_b2_bucket`, `google_cloud_storage_bucket`, `wasabi_bucket`, `s3_bucket`, `azure_blob_storage_container`, `azure_files_storage_share_name`, `s3_compatible_bucket`, `filebase_bucket`, `cloudflare_bucket` or `linode_bucket`. Valid field combinations are `[ backblaze_b2_bucket, name ]`, `[ google_cloud_storage_bucket, name ]`, `[ wasabi_bucket, name ]`, `[ s3_bucket, name ]`, `[ azure_blob_storage_container, name ]`, `[ azure_files_storage_share_name, name ]`, `[ s3_compatible_bucket, name ]`, `[ filebase_bucket, name ]`, `[ cloudflare_bucket, name ]` or `[ linode_bucket, name ]`.


---

## Show Remote Server

```
Task<RemoteServer> RemoteServer.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.


---

## Download configuration file (required for some Remote Server integrations, such as the Files.com Agent)

```
Task<RemoteServerConfigurationFile> RemoteServer.FindConfigurationFile(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.


---

## Create Remote Server

```
Task<RemoteServer> RemoteServer.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `password` (string): Password, if needed.
* `private_key` (string): Private key, if needed.
* `private_key_passphrase` (string): Passphrase for private key if needed.
* `reset_authentication` (bool): Reset authenticated account?
* `ssl_certificate` (string): SSL client certificate.
* `aws_secret_key` (string): AWS: secret key.
* `azure_blob_storage_access_key` (string): Azure Blob Storage: Access Key
* `azure_blob_storage_sas_token` (string): Azure Blob Storage: Shared Access Signature (SAS) token
* `azure_files_storage_access_key` (string): Azure File Storage: Access Key
* `azure_files_storage_sas_token` (string): Azure File Storage: Shared Access Signature (SAS) token
* `backblaze_b2_application_key` (string): Backblaze B2 Cloud Storage: applicationKey
* `backblaze_b2_key_id` (string): Backblaze B2 Cloud Storage: keyID
* `cloudflare_secret_key` (string): Cloudflare: Secret Key
* `filebase_secret_key` (string): Filebase: Secret Key
* `google_cloud_storage_credentials_json` (string): Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
* `google_cloud_storage_s3_compatible_secret_key` (string): Google Cloud Storage: S3-compatible secret key
* `linode_secret_key` (string): Linode: Secret Key
* `s3_compatible_secret_key` (string): S3-compatible: Secret Key
* `wasabi_secret_key` (string): Wasabi: Secret Key
* `aws_access_key` (string): AWS Access Key.
* `azure_blob_storage_account` (string): Azure Blob Storage: Account name
* `azure_blob_storage_container` (string): Azure Blob Storage: Container name
* `azure_blob_storage_dns_suffix` (string): Azure Blob Storage: Custom DNS suffix
* `azure_blob_storage_hierarchical_namespace` (bool): Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
* `azure_files_storage_account` (string): Azure Files: Storage Account name
* `azure_files_storage_dns_suffix` (string): Azure Files: Custom DNS suffix
* `azure_files_storage_share_name` (string): Azure Files:  Storage Share name
* `backblaze_b2_bucket` (string): Backblaze B2 Cloud Storage: Bucket name
* `backblaze_b2_s3_endpoint` (string): Backblaze B2 Cloud Storage: S3 Endpoint
* `buffer_uploads` (string): If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
* `cloudflare_access_key` (string): Cloudflare: Access Key.
* `cloudflare_bucket` (string): Cloudflare: Bucket name
* `cloudflare_endpoint` (string): Cloudflare: endpoint
* `description` (string): Internal description for your reference
* `dropbox_teams` (bool): Dropbox: If true, list Team folders in root?
* `enable_dedicated_ips` (bool): `true` if remote server only accepts connections from dedicated IPs
* `filebase_access_key` (string): Filebase: Access Key.
* `filebase_bucket` (string): Filebase: Bucket name
* `files_agent_permission_set` (string): Local permissions for files agent. read_only, write_only, or read_write
* `files_agent_root` (string): Agent local root path
* `files_agent_version` (string): Files Agent version
* `outbound_agent_id` (Nullable<Int64>): Route traffic to outbound on a files-agent
* `google_cloud_storage_bucket` (string): Google Cloud Storage: Bucket Name
* `google_cloud_storage_project_id` (string): Google Cloud Storage: Project ID
* `google_cloud_storage_s3_compatible_access_key` (string): Google Cloud Storage: S3-compatible Access Key.
* `hostname` (string): Hostname or IP address
* `linode_access_key` (string): Linode: Access Key
* `linode_bucket` (string): Linode: Bucket name
* `linode_region` (string): Linode: region
* `max_connections` (Nullable<Int64>): Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
* `name` (string): Internal name for your reference
* `one_drive_account_type` (string): OneDrive: Either personal or business_other account types
* `pin_to_site_region` (bool): If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
* `port` (Nullable<Int64>): Port for remote server.
* `remote_server_credential_id` (Nullable<Int64>): ID of Remote Server Credential, if applicable.
* `s3_bucket` (string): S3 bucket name
* `s3_compatible_access_key` (string): S3-compatible: Access Key
* `s3_compatible_bucket` (string): S3-compatible: Bucket name
* `s3_compatible_endpoint` (string): S3-compatible: endpoint
* `s3_compatible_region` (string): S3-compatible: region
* `s3_region` (string): S3 region
* `server_certificate` (string): Remote server certificate
* `server_host_key` (string): Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
* `server_type` (string): Remote server type.
* `ssl` (string): Should we require SSL?
* `username` (string): Remote server username.
* `wasabi_access_key` (string): Wasabi: Access Key.
* `wasabi_bucket` (string): Wasabi: Bucket name
* `wasabi_region` (string): Wasabi: Region
* `workspace_id` (Nullable<Int64>): Workspace ID (0 for default workspace)


---

## Push update to Files Agent

```
Task<AgentPushUpdate> RemoteServer.AgentPushUpdate(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.


---

## Post local changes, check in, and download configuration file (used by some Remote Server integrations, such as the Files.com Agent)

```
Task<RemoteServerConfigurationFile> RemoteServer.ConfigurationFile(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.
* `api_token` (string): Files Agent API Token
* `permission_set` (string): The permission set for the agent ['read_write', 'read_only', 'write_only']
* `root` (string): The root directory for the agent
* `hostname` (string): 
* `port` (Nullable<Int64>): Incoming port for files agent connections
* `status` (string): either running or shutdown
* `config_version` (string): agent config version
* `private_key` (string): The private key for the agent
* `public_key` (string): public key
* `server_host_key` (string): 
* `subdomain` (string): Files.com subdomain site name


---

## Update Remote Server

```
Task<RemoteServer> RemoteServer.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.
* `password` (string): Password, if needed.
* `private_key` (string): Private key, if needed.
* `private_key_passphrase` (string): Passphrase for private key if needed.
* `reset_authentication` (bool): Reset authenticated account?
* `ssl_certificate` (string): SSL client certificate.
* `aws_secret_key` (string): AWS: secret key.
* `azure_blob_storage_access_key` (string): Azure Blob Storage: Access Key
* `azure_blob_storage_sas_token` (string): Azure Blob Storage: Shared Access Signature (SAS) token
* `azure_files_storage_access_key` (string): Azure File Storage: Access Key
* `azure_files_storage_sas_token` (string): Azure File Storage: Shared Access Signature (SAS) token
* `backblaze_b2_application_key` (string): Backblaze B2 Cloud Storage: applicationKey
* `backblaze_b2_key_id` (string): Backblaze B2 Cloud Storage: keyID
* `cloudflare_secret_key` (string): Cloudflare: Secret Key
* `filebase_secret_key` (string): Filebase: Secret Key
* `google_cloud_storage_credentials_json` (string): Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
* `google_cloud_storage_s3_compatible_secret_key` (string): Google Cloud Storage: S3-compatible secret key
* `linode_secret_key` (string): Linode: Secret Key
* `s3_compatible_secret_key` (string): S3-compatible: Secret Key
* `wasabi_secret_key` (string): Wasabi: Secret Key
* `aws_access_key` (string): AWS Access Key.
* `azure_blob_storage_account` (string): Azure Blob Storage: Account name
* `azure_blob_storage_container` (string): Azure Blob Storage: Container name
* `azure_blob_storage_dns_suffix` (string): Azure Blob Storage: Custom DNS suffix
* `azure_blob_storage_hierarchical_namespace` (bool): Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
* `azure_files_storage_account` (string): Azure Files: Storage Account name
* `azure_files_storage_dns_suffix` (string): Azure Files: Custom DNS suffix
* `azure_files_storage_share_name` (string): Azure Files:  Storage Share name
* `backblaze_b2_bucket` (string): Backblaze B2 Cloud Storage: Bucket name
* `backblaze_b2_s3_endpoint` (string): Backblaze B2 Cloud Storage: S3 Endpoint
* `buffer_uploads` (string): If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
* `cloudflare_access_key` (string): Cloudflare: Access Key.
* `cloudflare_bucket` (string): Cloudflare: Bucket name
* `cloudflare_endpoint` (string): Cloudflare: endpoint
* `description` (string): Internal description for your reference
* `dropbox_teams` (bool): Dropbox: If true, list Team folders in root?
* `enable_dedicated_ips` (bool): `true` if remote server only accepts connections from dedicated IPs
* `filebase_access_key` (string): Filebase: Access Key.
* `filebase_bucket` (string): Filebase: Bucket name
* `files_agent_permission_set` (string): Local permissions for files agent. read_only, write_only, or read_write
* `files_agent_root` (string): Agent local root path
* `files_agent_version` (string): Files Agent version
* `outbound_agent_id` (Nullable<Int64>): Route traffic to outbound on a files-agent
* `google_cloud_storage_bucket` (string): Google Cloud Storage: Bucket Name
* `google_cloud_storage_project_id` (string): Google Cloud Storage: Project ID
* `google_cloud_storage_s3_compatible_access_key` (string): Google Cloud Storage: S3-compatible Access Key.
* `hostname` (string): Hostname or IP address
* `linode_access_key` (string): Linode: Access Key
* `linode_bucket` (string): Linode: Bucket name
* `linode_region` (string): Linode: region
* `max_connections` (Nullable<Int64>): Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
* `name` (string): Internal name for your reference
* `one_drive_account_type` (string): OneDrive: Either personal or business_other account types
* `pin_to_site_region` (bool): If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
* `port` (Nullable<Int64>): Port for remote server.
* `remote_server_credential_id` (Nullable<Int64>): ID of Remote Server Credential, if applicable.
* `s3_bucket` (string): S3 bucket name
* `s3_compatible_access_key` (string): S3-compatible: Access Key
* `s3_compatible_bucket` (string): S3-compatible: Bucket name
* `s3_compatible_endpoint` (string): S3-compatible: endpoint
* `s3_compatible_region` (string): S3-compatible: region
* `s3_region` (string): S3 region
* `server_certificate` (string): Remote server certificate
* `server_host_key` (string): Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
* `server_type` (string): Remote server type.
* `ssl` (string): Should we require SSL?
* `username` (string): Remote server username.
* `wasabi_access_key` (string): Wasabi: Access Key.
* `wasabi_bucket` (string): Wasabi: Bucket name
* `wasabi_region` (string): Wasabi: Region


---

## Delete Remote Server

```
Task RemoteServer.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.


---

## Push update to Files Agent

```
var RemoteServer = RemoteServer.Find(1);

var parameters = new Dictionary<string, object>();


RemoteServer.AgentPushUpdate
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.


---

## Post local changes, check in, and download configuration file (used by some Remote Server integrations, such as the Files.com Agent)

```
var RemoteServer = RemoteServer.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("api_token", "example");
parameters.Add("permission_set", "example");
parameters.Add("root", "C:\\Users\\");
parameters.Add("hostname", "example");
parameters.Add("port", 1);
parameters.Add("status", "example");
parameters.Add("config_version", "example");
parameters.Add("private_key", "example");
parameters.Add("public_key", "example");
parameters.Add("server_host_key", "example");
parameters.Add("subdomain", "example");

RemoteServer.ConfigurationFile(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.
* `api_token` (string): Files Agent API Token
* `permission_set` (string): The permission set for the agent ['read_write', 'read_only', 'write_only']
* `root` (string): The root directory for the agent
* `hostname` (string): 
* `port` (Nullable<Int64>): Incoming port for files agent connections
* `status` (string): either running or shutdown
* `config_version` (string): agent config version
* `private_key` (string): The private key for the agent
* `public_key` (string): public key
* `server_host_key` (string): 
* `subdomain` (string): Files.com subdomain site name


---

## Update Remote Server

```
var RemoteServer = RemoteServer.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("reset_authentication", false);
parameters.Add("aws_access_key", "example");
parameters.Add("azure_blob_storage_account", "storage-account-name");
parameters.Add("azure_blob_storage_container", "container-name");
parameters.Add("azure_blob_storage_dns_suffix", "usgovcloudapi.net");
parameters.Add("azure_blob_storage_hierarchical_namespace", true);
parameters.Add("azure_files_storage_account", "storage-account-name");
parameters.Add("azure_files_storage_dns_suffix", "file.core.windows.net");
parameters.Add("azure_files_storage_share_name", "share-name");
parameters.Add("backblaze_b2_bucket", "my-bucket");
parameters.Add("backblaze_b2_s3_endpoint", "s3.us-west-001.backblazeb2.com");
parameters.Add("buffer_uploads", "example");
parameters.Add("cloudflare_access_key", "example");
parameters.Add("cloudflare_bucket", "my-bucket");
parameters.Add("cloudflare_endpoint", "https://<ACCOUNT_ID>.r2.cloudflarestorage.com");
parameters.Add("description", "More information or notes about my server");
parameters.Add("dropbox_teams", true);
parameters.Add("enable_dedicated_ips", true);
parameters.Add("filebase_access_key", "example");
parameters.Add("filebase_bucket", "my-bucket");
parameters.Add("files_agent_permission_set", "read_write");
parameters.Add("files_agent_root", "example");
parameters.Add("files_agent_version", "example");
parameters.Add("outbound_agent_id", 1);
parameters.Add("google_cloud_storage_bucket", "my-bucket");
parameters.Add("google_cloud_storage_project_id", "my-project");
parameters.Add("google_cloud_storage_s3_compatible_access_key", "example");
parameters.Add("hostname", "remote-server.com");
parameters.Add("linode_access_key", "example");
parameters.Add("linode_bucket", "my-bucket");
parameters.Add("linode_region", "us-east-1");
parameters.Add("max_connections", 1);
parameters.Add("name", "My Remote server");
parameters.Add("one_drive_account_type", "personal");
parameters.Add("pin_to_site_region", true);
parameters.Add("port", 1);
parameters.Add("remote_server_credential_id", 1);
parameters.Add("s3_bucket", "my-bucket");
parameters.Add("s3_compatible_access_key", "example");
parameters.Add("s3_compatible_bucket", "my-bucket");
parameters.Add("s3_compatible_endpoint", "mys3platform.com");
parameters.Add("s3_compatible_region", "us-east-1");
parameters.Add("s3_region", "us-east-1");
parameters.Add("server_certificate", "require_match");
parameters.Add("server_host_key", "[public key]");
parameters.Add("server_type", "s3");
parameters.Add("ssl", "if_available");
parameters.Add("username", "user");
parameters.Add("wasabi_access_key", "example");
parameters.Add("wasabi_bucket", "my-bucket");
parameters.Add("wasabi_region", "us-west-1");

RemoteServer.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.
* `password` (string): Password, if needed.
* `private_key` (string): Private key, if needed.
* `private_key_passphrase` (string): Passphrase for private key if needed.
* `reset_authentication` (bool): Reset authenticated account?
* `ssl_certificate` (string): SSL client certificate.
* `aws_secret_key` (string): AWS: secret key.
* `azure_blob_storage_access_key` (string): Azure Blob Storage: Access Key
* `azure_blob_storage_sas_token` (string): Azure Blob Storage: Shared Access Signature (SAS) token
* `azure_files_storage_access_key` (string): Azure File Storage: Access Key
* `azure_files_storage_sas_token` (string): Azure File Storage: Shared Access Signature (SAS) token
* `backblaze_b2_application_key` (string): Backblaze B2 Cloud Storage: applicationKey
* `backblaze_b2_key_id` (string): Backblaze B2 Cloud Storage: keyID
* `cloudflare_secret_key` (string): Cloudflare: Secret Key
* `filebase_secret_key` (string): Filebase: Secret Key
* `google_cloud_storage_credentials_json` (string): Google Cloud Storage: JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
* `google_cloud_storage_s3_compatible_secret_key` (string): Google Cloud Storage: S3-compatible secret key
* `linode_secret_key` (string): Linode: Secret Key
* `s3_compatible_secret_key` (string): S3-compatible: Secret Key
* `wasabi_secret_key` (string): Wasabi: Secret Key
* `aws_access_key` (string): AWS Access Key.
* `azure_blob_storage_account` (string): Azure Blob Storage: Account name
* `azure_blob_storage_container` (string): Azure Blob Storage: Container name
* `azure_blob_storage_dns_suffix` (string): Azure Blob Storage: Custom DNS suffix
* `azure_blob_storage_hierarchical_namespace` (bool): Azure Blob Storage: Does the storage account has hierarchical namespace feature enabled?
* `azure_files_storage_account` (string): Azure Files: Storage Account name
* `azure_files_storage_dns_suffix` (string): Azure Files: Custom DNS suffix
* `azure_files_storage_share_name` (string): Azure Files:  Storage Share name
* `backblaze_b2_bucket` (string): Backblaze B2 Cloud Storage: Bucket name
* `backblaze_b2_s3_endpoint` (string): Backblaze B2 Cloud Storage: S3 Endpoint
* `buffer_uploads` (string): If set to always, uploads to this server will be uploaded first to Files.com before being sent to the remote server. This can improve performance in certain access patterns, such as high-latency connections.  It will cause data to be temporarily stored in Files.com. If set to auto, we will perform this optimization if we believe it to be a benefit in a given situation.
* `cloudflare_access_key` (string): Cloudflare: Access Key.
* `cloudflare_bucket` (string): Cloudflare: Bucket name
* `cloudflare_endpoint` (string): Cloudflare: endpoint
* `description` (string): Internal description for your reference
* `dropbox_teams` (bool): Dropbox: If true, list Team folders in root?
* `enable_dedicated_ips` (bool): `true` if remote server only accepts connections from dedicated IPs
* `filebase_access_key` (string): Filebase: Access Key.
* `filebase_bucket` (string): Filebase: Bucket name
* `files_agent_permission_set` (string): Local permissions for files agent. read_only, write_only, or read_write
* `files_agent_root` (string): Agent local root path
* `files_agent_version` (string): Files Agent version
* `outbound_agent_id` (Nullable<Int64>): Route traffic to outbound on a files-agent
* `google_cloud_storage_bucket` (string): Google Cloud Storage: Bucket Name
* `google_cloud_storage_project_id` (string): Google Cloud Storage: Project ID
* `google_cloud_storage_s3_compatible_access_key` (string): Google Cloud Storage: S3-compatible Access Key.
* `hostname` (string): Hostname or IP address
* `linode_access_key` (string): Linode: Access Key
* `linode_bucket` (string): Linode: Bucket name
* `linode_region` (string): Linode: region
* `max_connections` (Nullable<Int64>): Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
* `name` (string): Internal name for your reference
* `one_drive_account_type` (string): OneDrive: Either personal or business_other account types
* `pin_to_site_region` (bool): If true, we will ensure that all communications with this remote server are made through the primary region of the site.  This setting can also be overridden by a site-wide setting which will force it to true.
* `port` (Nullable<Int64>): Port for remote server.
* `remote_server_credential_id` (Nullable<Int64>): ID of Remote Server Credential, if applicable.
* `s3_bucket` (string): S3 bucket name
* `s3_compatible_access_key` (string): S3-compatible: Access Key
* `s3_compatible_bucket` (string): S3-compatible: Bucket name
* `s3_compatible_endpoint` (string): S3-compatible: endpoint
* `s3_compatible_region` (string): S3-compatible: region
* `s3_region` (string): S3 region
* `server_certificate` (string): Remote server certificate
* `server_host_key` (string): Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
* `server_type` (string): Remote server type.
* `ssl` (string): Should we require SSL?
* `username` (string): Remote server username.
* `wasabi_access_key` (string): Wasabi: Access Key.
* `wasabi_bucket` (string): Wasabi: Bucket name
* `wasabi_region` (string): Wasabi: Region


---

## Delete Remote Server

```
var RemoteServer = RemoteServer.Find(1);

var parameters = new Dictionary<string, object>();


RemoteServer.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.
