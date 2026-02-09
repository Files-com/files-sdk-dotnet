# FilesCom.Models.RemoteServerCredential

## Example RemoteServerCredential Object

```
{
  "id": 1,
  "workspace_id": 1,
  "name": "My Credential",
  "description": "More information or notes about this credential.",
  "server_type": "s3",
  "aws_access_key": "example",
  "s3_assume_role_arn": "example",
  "s3_assume_role_duration_seconds": 1,
  "s3_assume_role_external_id": "example",
  "google_cloud_storage_s3_compatible_access_key": "example",
  "wasabi_access_key": "example",
  "s3_compatible_access_key": "example",
  "filebase_access_key": "example",
  "cloudflare_access_key": "example",
  "linode_access_key": "example",
  "username": "user"
}
```

* `id` / `Id`  (Nullable<Int64>): Remote Server Credential ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID (0 for default workspace)
* `name` / `Name`  (string): Internal name for your reference
* `description` / `Description`  (string): Internal description for your reference
* `server_type` / `ServerType`  (string): Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
* `aws_access_key` / `AwsAccessKey`  (string): AWS Access Key.
* `s3_assume_role_arn` / `S3AssumeRoleArn`  (string): AWS IAM Role ARN for AssumeRole authentication.
* `s3_assume_role_duration_seconds` / `S3AssumeRoleDurationSeconds`  (Nullable<Int64>): Session duration in seconds for AssumeRole authentication (900-43200).
* `s3_assume_role_external_id` / `S3AssumeRoleExternalId`  (string): External ID for AssumeRole authentication.
* `google_cloud_storage_s3_compatible_access_key` / `GoogleCloudStorageS3CompatibleAccessKey`  (string): Google Cloud Storage: S3-compatible Access Key.
* `wasabi_access_key` / `WasabiAccessKey`  (string): Wasabi: Access Key.
* `s3_compatible_access_key` / `S3CompatibleAccessKey`  (string): S3-compatible: Access Key
* `filebase_access_key` / `FilebaseAccessKey`  (string): Filebase: Access Key.
* `cloudflare_access_key` / `CloudflareAccessKey`  (string): Cloudflare: Access Key.
* `linode_access_key` / `LinodeAccessKey`  (string): Linode: Access Key
* `username` / `Username`  (string): Remote server username.
* `password` / `Password`  (string): Password, if needed.
* `private_key` / `PrivateKey`  (string): Private key, if needed.
* `private_key_passphrase` / `PrivateKeyPassphrase`  (string): Passphrase for private key if needed.
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

## List Remote Server Credentials

```
Task<FilesList<RemoteServerCredential>> RemoteServerCredential.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id` and `name`. Valid field combinations are `[ workspace_id, name ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.


---

## Show Remote Server Credential

```
Task<RemoteServerCredential> RemoteServerCredential.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server Credential ID.


---

## Create Remote Server Credential

```
Task<RemoteServerCredential> RemoteServerCredential.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Internal name for your reference
* `description` (string): Internal description for your reference
* `server_type` (string): Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
* `aws_access_key` (string): AWS Access Key.
* `s3_assume_role_arn` (string): AWS IAM Role ARN for AssumeRole authentication.
* `s3_assume_role_duration_seconds` (Nullable<Int64>): Session duration in seconds for AssumeRole authentication (900-43200).
* `cloudflare_access_key` (string): Cloudflare: Access Key.
* `filebase_access_key` (string): Filebase: Access Key.
* `google_cloud_storage_s3_compatible_access_key` (string): Google Cloud Storage: S3-compatible Access Key.
* `linode_access_key` (string): Linode: Access Key
* `s3_compatible_access_key` (string): S3-compatible: Access Key
* `username` (string): Remote server username.
* `wasabi_access_key` (string): Wasabi: Access Key.
* `password` (string): Password, if needed.
* `private_key` (string): Private key, if needed.
* `private_key_passphrase` (string): Passphrase for private key if needed.
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
* `workspace_id` (Nullable<Int64>): Workspace ID (0 for default workspace)


---

## Update Remote Server Credential

```
Task<RemoteServerCredential> RemoteServerCredential.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server Credential ID.
* `name` (string): Internal name for your reference
* `description` (string): Internal description for your reference
* `server_type` (string): Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
* `aws_access_key` (string): AWS Access Key.
* `s3_assume_role_arn` (string): AWS IAM Role ARN for AssumeRole authentication.
* `s3_assume_role_duration_seconds` (Nullable<Int64>): Session duration in seconds for AssumeRole authentication (900-43200).
* `cloudflare_access_key` (string): Cloudflare: Access Key.
* `filebase_access_key` (string): Filebase: Access Key.
* `google_cloud_storage_s3_compatible_access_key` (string): Google Cloud Storage: S3-compatible Access Key.
* `linode_access_key` (string): Linode: Access Key
* `s3_compatible_access_key` (string): S3-compatible: Access Key
* `username` (string): Remote server username.
* `wasabi_access_key` (string): Wasabi: Access Key.
* `password` (string): Password, if needed.
* `private_key` (string): Private key, if needed.
* `private_key_passphrase` (string): Passphrase for private key if needed.
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


---

## Delete Remote Server Credential

```
Task RemoteServerCredential.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server Credential ID.


---

## Update Remote Server Credential

```
var RemoteServerCredential = RemoteServerCredential.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "My Credential");
parameters.Add("description", "More information or notes about this credential.");
parameters.Add("server_type", "s3");
parameters.Add("aws_access_key", "example");
parameters.Add("s3_assume_role_arn", "example");
parameters.Add("s3_assume_role_duration_seconds", 1);
parameters.Add("cloudflare_access_key", "example");
parameters.Add("filebase_access_key", "example");
parameters.Add("google_cloud_storage_s3_compatible_access_key", "example");
parameters.Add("linode_access_key", "example");
parameters.Add("s3_compatible_access_key", "example");
parameters.Add("username", "user");
parameters.Add("wasabi_access_key", "example");

RemoteServerCredential.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server Credential ID.
* `name` (string): Internal name for your reference
* `description` (string): Internal description for your reference
* `server_type` (string): Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
* `aws_access_key` (string): AWS Access Key.
* `s3_assume_role_arn` (string): AWS IAM Role ARN for AssumeRole authentication.
* `s3_assume_role_duration_seconds` (Nullable<Int64>): Session duration in seconds for AssumeRole authentication (900-43200).
* `cloudflare_access_key` (string): Cloudflare: Access Key.
* `filebase_access_key` (string): Filebase: Access Key.
* `google_cloud_storage_s3_compatible_access_key` (string): Google Cloud Storage: S3-compatible Access Key.
* `linode_access_key` (string): Linode: Access Key
* `s3_compatible_access_key` (string): S3-compatible: Access Key
* `username` (string): Remote server username.
* `wasabi_access_key` (string): Wasabi: Access Key.
* `password` (string): Password, if needed.
* `private_key` (string): Private key, if needed.
* `private_key_passphrase` (string): Passphrase for private key if needed.
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


---

## Delete Remote Server Credential

```
var RemoteServerCredential = RemoteServerCredential.Find(1);

var parameters = new Dictionary<string, object>();


RemoteServerCredential.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Remote Server Credential ID.
