# FilesCom.Models.RemoteServer

## Example RemoteServer Object

```
{
  "id": 1,
  "authentication_method": "password",
  "hostname": "remote-server.com",
  "remote_home_path": "/home/user1",
  "name": "My Remote server",
  "port": 1,
  "max_connections": 1,
  "s3_bucket": "my-bucket",
  "s3_region": "us-east-1",
  "server_certificate": "require_match",
  "server_host_key": "[public key]",
  "server_type": "s3",
  "ssl": "if_available",
  "username": "user",
  "google_cloud_storage_bucket": "my-bucket",
  "google_cloud_storage_project_id": "my-project",
  "backblaze_b2_s3_endpoint": "s3.us-west-001.backblazeb2.com",
  "backblaze_b2_bucket": "my-bucket",
  "wasabi_bucket": "my-bucket",
  "wasabi_region": "us-west-1",
  "rackspace_username": "rackspaceuser",
  "rackspace_region": "dfw",
  "rackspace_container": "my-container",
  "auth_setup_link": "auth/:provider",
  "auth_status": "in_setup",
  "auth_account_name": "me@example.com",
  "one_drive_account_type": "personal",
  "azure_blob_storage_account": "storage-account-name",
  "azure_blob_storage_container": "container-name",
  "s3_compatible_bucket": "my-bucket",
  "s3_compatible_region": "us-east-1",
  "s3_compatible_endpoint": "mys3platform.com"
}
```

* `id` / `Id`  (Nullable<Int64>): Remote server ID
* `authentication_method` / `AuthenticationMethod`  (string): Type of authentication method
* `hostname` / `Hostname`  (string): Hostname or IP address
* `remote_home_path` / `RemoteHomePath`  (string): Initial home folder on remote server
* `name` / `Name`  (string): Internal name for your reference
* `port` / `Port`  (Nullable<Int64>): Port for remote server.  Not needed for S3.
* `max_connections` / `MaxConnections`  (Nullable<Int64>): Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
* `s3_bucket` / `S3Bucket`  (string): S3 bucket name
* `s3_region` / `S3Region`  (string): S3 region
* `server_certificate` / `ServerCertificate`  (string): Remote server certificate
* `server_host_key` / `ServerHostKey`  (string): Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
* `server_type` / `ServerType`  (string): Remote server type.
* `ssl` / `Ssl`  (string): Should we require SSL?
* `username` / `Username`  (string): Remote server username.  Not needed for S3 buckets.
* `google_cloud_storage_bucket` / `GoogleCloudStorageBucket`  (string): Google Cloud Storage bucket name
* `google_cloud_storage_project_id` / `GoogleCloudStorageProjectId`  (string): Google Cloud Project ID
* `backblaze_b2_s3_endpoint` / `BackblazeB2S3Endpoint`  (string): Backblaze B2 Cloud Storage S3 Endpoint
* `backblaze_b2_bucket` / `BackblazeB2Bucket`  (string): Backblaze B2 Cloud Storage Bucket name
* `wasabi_bucket` / `WasabiBucket`  (string): Wasabi Bucket name
* `wasabi_region` / `WasabiRegion`  (string): Wasabi region
* `rackspace_username` / `RackspaceUsername`  (string): Rackspace username used to login to the Rackspace Cloud Control Panel.
* `rackspace_region` / `RackspaceRegion`  (string): Three letter airport code for Rackspace region. See https://support.rackspace.com/how-to/about-regions/
* `rackspace_container` / `RackspaceContainer`  (string): The name of the container (top level directory) where files will sync.
* `auth_setup_link` / `AuthSetupLink`  (string): Returns link to login with an Oauth provider
* `auth_status` / `AuthStatus`  (string): Either `in_setup` or `complete`
* `auth_account_name` / `AuthAccountName`  (string): Describes the authorized account
* `one_drive_account_type` / `OneDriveAccountType`  (string): Either personal or business_other account types
* `azure_blob_storage_account` / `AzureBlobStorageAccount`  (string): Azure Blob Storage Account name
* `azure_blob_storage_container` / `AzureBlobStorageContainer`  (string): Azure Blob Storage Container name
* `s3_compatible_bucket` / `S3CompatibleBucket`  (string): S3-compatible Bucket name
* `s3_compatible_region` / `S3CompatibleRegion`  (string): S3-compatible Bucket name
* `s3_compatible_endpoint` / `S3CompatibleEndpoint`  (string): S3-compatible endpoint
* `aws_access_key` / `AwsAccessKey`  (string): AWS Access Key.
* `aws_secret_key` / `AwsSecretKey`  (string): AWS secret key.
* `password` / `Password`  (string): Password if needed.
* `private_key` / `PrivateKey`  (string): Private key if needed.
* `ssl_certificate` / `SslCertificate`  (string): SSL client certificate.
* `google_cloud_storage_credentials_json` / `GoogleCloudStorageCredentialsJson`  (string): A JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
* `wasabi_access_key` / `WasabiAccessKey`  (string): Wasabi access key.
* `wasabi_secret_key` / `WasabiSecretKey`  (string): Wasabi secret key.
* `backblaze_b2_key_id` / `BackblazeB2KeyId`  (string): Backblaze B2 Cloud Storage keyID.
* `backblaze_b2_application_key` / `BackblazeB2ApplicationKey`  (string): Backblaze B2 Cloud Storage applicationKey.
* `rackspace_api_key` / `RackspaceApiKey`  (string): Rackspace API key from the Rackspace Cloud Control Panel.
* `reset_authentication` / `ResetAuthentication`  (bool): Reset authenticated account
* `azure_blob_storage_access_key` / `AzureBlobStorageAccessKey`  (string): Azure Blob Storage secret key.
* `s3_compatible_access_key` / `S3CompatibleAccessKey`  (string): S3-compatible access key
* `s3_compatible_secret_key` / `S3CompatibleSecretKey`  (string): S3-compatible secret key


---

## List Remote Servers

```
Task<RemoteServer[]> RemoteServer.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


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

## Create Remote Server

```
Task<RemoteServer> RemoteServer.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `aws_access_key` (string): AWS Access Key.
* `aws_secret_key` (string): AWS secret key.
* `password` (string): Password if needed.
* `private_key` (string): Private key if needed.
* `ssl_certificate` (string): SSL client certificate.
* `google_cloud_storage_credentials_json` (string): A JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
* `wasabi_access_key` (string): Wasabi access key.
* `wasabi_secret_key` (string): Wasabi secret key.
* `backblaze_b2_key_id` (string): Backblaze B2 Cloud Storage keyID.
* `backblaze_b2_application_key` (string): Backblaze B2 Cloud Storage applicationKey.
* `rackspace_api_key` (string): Rackspace API key from the Rackspace Cloud Control Panel.
* `reset_authentication` (bool): Reset authenticated account
* `azure_blob_storage_access_key` (string): Azure Blob Storage secret key.
* `hostname` (string): Hostname or IP address
* `name` (string): Internal name for your reference
* `max_connections` (Nullable<Int64>): Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
* `port` (Nullable<Int64>): Port for remote server.  Not needed for S3.
* `s3_bucket` (string): S3 bucket name
* `s3_region` (string): S3 region
* `server_certificate` (string): Remote server certificate
* `server_host_key` (string): Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
* `server_type` (string): Remote server type.
* `ssl` (string): Should we require SSL?
* `username` (string): Remote server username.  Not needed for S3 buckets.
* `google_cloud_storage_bucket` (string): Google Cloud Storage bucket name
* `google_cloud_storage_project_id` (string): Google Cloud Project ID
* `backblaze_b2_bucket` (string): Backblaze B2 Cloud Storage Bucket name
* `backblaze_b2_s3_endpoint` (string): Backblaze B2 Cloud Storage S3 Endpoint
* `wasabi_bucket` (string): Wasabi Bucket name
* `wasabi_region` (string): Wasabi region
* `rackspace_username` (string): Rackspace username used to login to the Rackspace Cloud Control Panel.
* `rackspace_region` (string): Three letter airport code for Rackspace region. See https://support.rackspace.com/how-to/about-regions/
* `rackspace_container` (string): The name of the container (top level directory) where files will sync.
* `one_drive_account_type` (string): Either personal or business_other account types
* `azure_blob_storage_account` (string): Azure Blob Storage Account name
* `azure_blob_storage_container` (string): Azure Blob Storage Container name
* `s3_compatible_bucket` (string): S3-compatible Bucket name
* `s3_compatible_region` (string): S3-compatible Bucket name
* `s3_compatible_endpoint` (string): S3-compatible endpoint
* `s3_compatible_access_key` (string): S3-compatible access key
* `s3_compatible_secret_key` (string): S3-compatible secret key


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
* `aws_access_key` (string): AWS Access Key.
* `aws_secret_key` (string): AWS secret key.
* `password` (string): Password if needed.
* `private_key` (string): Private key if needed.
* `ssl_certificate` (string): SSL client certificate.
* `google_cloud_storage_credentials_json` (string): A JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
* `wasabi_access_key` (string): Wasabi access key.
* `wasabi_secret_key` (string): Wasabi secret key.
* `backblaze_b2_key_id` (string): Backblaze B2 Cloud Storage keyID.
* `backblaze_b2_application_key` (string): Backblaze B2 Cloud Storage applicationKey.
* `rackspace_api_key` (string): Rackspace API key from the Rackspace Cloud Control Panel.
* `reset_authentication` (bool): Reset authenticated account
* `azure_blob_storage_access_key` (string): Azure Blob Storage secret key.
* `hostname` (string): Hostname or IP address
* `name` (string): Internal name for your reference
* `max_connections` (Nullable<Int64>): Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
* `port` (Nullable<Int64>): Port for remote server.  Not needed for S3.
* `s3_bucket` (string): S3 bucket name
* `s3_region` (string): S3 region
* `server_certificate` (string): Remote server certificate
* `server_host_key` (string): Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
* `server_type` (string): Remote server type.
* `ssl` (string): Should we require SSL?
* `username` (string): Remote server username.  Not needed for S3 buckets.
* `google_cloud_storage_bucket` (string): Google Cloud Storage bucket name
* `google_cloud_storage_project_id` (string): Google Cloud Project ID
* `backblaze_b2_bucket` (string): Backblaze B2 Cloud Storage Bucket name
* `backblaze_b2_s3_endpoint` (string): Backblaze B2 Cloud Storage S3 Endpoint
* `wasabi_bucket` (string): Wasabi Bucket name
* `wasabi_region` (string): Wasabi region
* `rackspace_username` (string): Rackspace username used to login to the Rackspace Cloud Control Panel.
* `rackspace_region` (string): Three letter airport code for Rackspace region. See https://support.rackspace.com/how-to/about-regions/
* `rackspace_container` (string): The name of the container (top level directory) where files will sync.
* `one_drive_account_type` (string): Either personal or business_other account types
* `azure_blob_storage_account` (string): Azure Blob Storage Account name
* `azure_blob_storage_container` (string): Azure Blob Storage Container name
* `s3_compatible_bucket` (string): S3-compatible Bucket name
* `s3_compatible_region` (string): S3-compatible Bucket name
* `s3_compatible_endpoint` (string): S3-compatible endpoint
* `s3_compatible_access_key` (string): S3-compatible access key
* `s3_compatible_secret_key` (string): S3-compatible secret key


---

## Delete Remote Server

```
Task<RemoteServer> RemoteServer.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.


---

## Update Remote Server

```
var RemoteServer = RemoteServer.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("reset_authentication", true);
parameters.Add("hostname", "remote-server.com");
parameters.Add("name", "My Remote server");
parameters.Add("max_connections", 1);
parameters.Add("port", 1);
parameters.Add("s3_bucket", "my-bucket");
parameters.Add("s3_region", "us-east-1");
parameters.Add("server_certificate", "require_match");
parameters.Add("server_host_key", "[public key]");
parameters.Add("server_type", "s3");
parameters.Add("ssl", "if_available");
parameters.Add("username", "user");
parameters.Add("google_cloud_storage_bucket", "my-bucket");
parameters.Add("google_cloud_storage_project_id", "my-project");
parameters.Add("backblaze_b2_bucket", "my-bucket");
parameters.Add("backblaze_b2_s3_endpoint", "s3.us-west-001.backblazeb2.com");
parameters.Add("wasabi_bucket", "my-bucket");
parameters.Add("wasabi_region", "us-west-1");
parameters.Add("rackspace_username", "rackspaceuser");
parameters.Add("rackspace_region", "dfw");
parameters.Add("rackspace_container", "my-container");
parameters.Add("one_drive_account_type", "personal");
parameters.Add("azure_blob_storage_account", "storage-account-name");
parameters.Add("azure_blob_storage_container", "container-name");
parameters.Add("s3_compatible_bucket", "my-bucket");
parameters.Add("s3_compatible_region", "us-east-1");
parameters.Add("s3_compatible_endpoint", "mys3platform.com");

RemoteServer.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.
* `aws_access_key` (string): AWS Access Key.
* `aws_secret_key` (string): AWS secret key.
* `password` (string): Password if needed.
* `private_key` (string): Private key if needed.
* `ssl_certificate` (string): SSL client certificate.
* `google_cloud_storage_credentials_json` (string): A JSON file that contains the private key. To generate see https://cloud.google.com/storage/docs/json_api/v1/how-tos/authorizing#APIKey
* `wasabi_access_key` (string): Wasabi access key.
* `wasabi_secret_key` (string): Wasabi secret key.
* `backblaze_b2_key_id` (string): Backblaze B2 Cloud Storage keyID.
* `backblaze_b2_application_key` (string): Backblaze B2 Cloud Storage applicationKey.
* `rackspace_api_key` (string): Rackspace API key from the Rackspace Cloud Control Panel.
* `reset_authentication` (bool): Reset authenticated account
* `azure_blob_storage_access_key` (string): Azure Blob Storage secret key.
* `hostname` (string): Hostname or IP address
* `name` (string): Internal name for your reference
* `max_connections` (Nullable<Int64>): Max number of parallel connections.  Ignored for S3 connections (we will parallelize these as much as possible).
* `port` (Nullable<Int64>): Port for remote server.  Not needed for S3.
* `s3_bucket` (string): S3 bucket name
* `s3_region` (string): S3 region
* `server_certificate` (string): Remote server certificate
* `server_host_key` (string): Remote server SSH Host Key. If provided, we will require that the server host key matches the provided key. Uses OpenSSH format similar to what would go into ~/.ssh/known_hosts
* `server_type` (string): Remote server type.
* `ssl` (string): Should we require SSL?
* `username` (string): Remote server username.  Not needed for S3 buckets.
* `google_cloud_storage_bucket` (string): Google Cloud Storage bucket name
* `google_cloud_storage_project_id` (string): Google Cloud Project ID
* `backblaze_b2_bucket` (string): Backblaze B2 Cloud Storage Bucket name
* `backblaze_b2_s3_endpoint` (string): Backblaze B2 Cloud Storage S3 Endpoint
* `wasabi_bucket` (string): Wasabi Bucket name
* `wasabi_region` (string): Wasabi region
* `rackspace_username` (string): Rackspace username used to login to the Rackspace Cloud Control Panel.
* `rackspace_region` (string): Three letter airport code for Rackspace region. See https://support.rackspace.com/how-to/about-regions/
* `rackspace_container` (string): The name of the container (top level directory) where files will sync.
* `one_drive_account_type` (string): Either personal or business_other account types
* `azure_blob_storage_account` (string): Azure Blob Storage Account name
* `azure_blob_storage_container` (string): Azure Blob Storage Container name
* `s3_compatible_bucket` (string): S3-compatible Bucket name
* `s3_compatible_region` (string): S3-compatible Bucket name
* `s3_compatible_endpoint` (string): S3-compatible endpoint
* `s3_compatible_access_key` (string): S3-compatible access key
* `s3_compatible_secret_key` (string): S3-compatible secret key


---

## Delete Remote Server

```
var RemoteServer = RemoteServer.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


RemoteServer.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server ID.
