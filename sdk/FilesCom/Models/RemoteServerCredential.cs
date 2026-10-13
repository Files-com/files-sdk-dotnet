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
    /// RemoteServerCredentialEntity model
    /// </summary>
    public class RemoteServerCredential : IModel, IJsonOnDeserialized
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
        /// Makes a RemoteServerCredential with no attributes set and no options.
        /// </summary>
        public RemoteServerCredential() : this(null, null) { }

        /// <summary>
        /// Makes a RemoteServerCredential with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public RemoteServerCredential(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("server_type"))
            {
                this.attributes.Add("server_type", null);
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
            if (!this.attributes.ContainsKey("google_cloud_storage_s3_compatible_access_key"))
            {
                this.attributes.Add("google_cloud_storage_s3_compatible_access_key", null);
            }
            if (!this.attributes.ContainsKey("wasabi_access_key"))
            {
                this.attributes.Add("wasabi_access_key", null);
            }
            if (!this.attributes.ContainsKey("s3_compatible_access_key"))
            {
                this.attributes.Add("s3_compatible_access_key", null);
            }
            if (!this.attributes.ContainsKey("filebase_access_key"))
            {
                this.attributes.Add("filebase_access_key", null);
            }
            if (!this.attributes.ContainsKey("cloudflare_access_key"))
            {
                this.attributes.Add("cloudflare_access_key", null);
            }
            if (!this.attributes.ContainsKey("linode_access_key"))
            {
                this.attributes.Add("linode_access_key", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_tenant_id"))
            {
                this.attributes.Add("sharepoint_tenant_id", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_client_id"))
            {
                this.attributes.Add("sharepoint_client_id", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_app_credential_type"))
            {
                this.attributes.Add("sharepoint_app_credential_type", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
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
            if (!this.attributes.ContainsKey("sharepoint_client_certificate"))
            {
                this.attributes.Add("sharepoint_client_certificate", null);
            }
            if (!this.attributes.ContainsKey("sharepoint_client_secret"))
            {
                this.attributes.Add("sharepoint_client_secret", null);
            }
            if (!this.attributes.ContainsKey("wasabi_secret_key"))
            {
                this.attributes.Add("wasabi_secret_key", null);
            }
            if (!this.attributes.ContainsKey("copy_values_from_credential_id"))
            {
                this.attributes.Add("copy_values_from_credential_id", null);
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
        /// Remote Server Credential ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
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
        /// Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        /// </summary>
        [JsonPropertyName("server_type")]
        public string ServerType
        {
            get { return (string)attributes["server_type"]; }
            set { attributes["server_type"] = value; unsaved.Add("server_type"); }
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
        /// Google Cloud Storage: S3-compatible Access Key.
        /// </summary>
        [JsonPropertyName("google_cloud_storage_s3_compatible_access_key")]
        public string GoogleCloudStorageS3CompatibleAccessKey
        {
            get { return (string)attributes["google_cloud_storage_s3_compatible_access_key"]; }
            set { attributes["google_cloud_storage_s3_compatible_access_key"] = value; unsaved.Add("google_cloud_storage_s3_compatible_access_key"); }
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
        /// S3-compatible: Access Key
        /// </summary>
        [JsonPropertyName("s3_compatible_access_key")]
        public string S3CompatibleAccessKey
        {
            get { return (string)attributes["s3_compatible_access_key"]; }
            set { attributes["s3_compatible_access_key"] = value; unsaved.Add("s3_compatible_access_key"); }
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
        /// Cloudflare: Access Key.
        /// </summary>
        [JsonPropertyName("cloudflare_access_key")]
        public string CloudflareAccessKey
        {
            get { return (string)attributes["cloudflare_access_key"]; }
            set { attributes["cloudflare_access_key"] = value; unsaved.Add("cloudflare_access_key"); }
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
        /// SharePoint: App-only credential type. Either secret or certificate.
        /// </summary>
        [JsonPropertyName("sharepoint_app_credential_type")]
        public string SharepointAppCredentialType
        {
            get { return (string)attributes["sharepoint_app_credential_type"]; }
            set { attributes["sharepoint_app_credential_type"] = value; unsaved.Add("sharepoint_app_credential_type"); }
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
        /// Wasabi: Secret Key
        /// </summary>
        [JsonPropertyName("wasabi_secret_key")]
        public string WasabiSecretKey
        {
            get { return (string)attributes["wasabi_secret_key"]; }
            set { attributes["wasabi_secret_key"] = value; unsaved.Add("wasabi_secret_key"); }
        }

        /// <summary>
        /// ID of Remote Server Credential to copy omitted values from.
        /// </summary>
        [JsonPropertyName("copy_values_from_credential_id")]
        public Nullable<Int64> CopyValuesFromCredentialId
        {
            get { return (Nullable<Int64>)attributes["copy_values_from_credential_id"]; }
            set { attributes["copy_values_from_credential_id"] = value; unsaved.Add("copy_values_from_credential_id"); }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Internal name for your reference
        ///   description - string - Internal description for your reference
        ///   server_type - string - Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        ///   aws_access_key - string - AWS Access Key.
        ///   s3_assume_role_arn - string - AWS IAM Role ARN for AssumeRole authentication.
        ///   s3_assume_role_duration_seconds - int64 - Session duration in seconds for AssumeRole authentication (900-43200).
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   linode_access_key - string - Linode: Access Key
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   sharepoint_client_id - string - SharePoint: Microsoft Entra application client ID for app-only authentication.
        ///   sharepoint_tenant_id - string - SharePoint: Microsoft Entra tenant ID for app-only authentication.
        ///   username - string - Remote server username.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
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
        ///   sharepoint_client_certificate - string - SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        ///   sharepoint_client_secret - string - SharePoint: Microsoft Entra application client secret for app-only authentication.
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The RemoteServerCredential it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<RemoteServerCredential> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Internal name for your reference
        ///   description - string - Internal description for your reference
        ///   server_type - string - Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        ///   aws_access_key - string - AWS Access Key.
        ///   s3_assume_role_arn - string - AWS IAM Role ARN for AssumeRole authentication.
        ///   s3_assume_role_duration_seconds - int64 - Session duration in seconds for AssumeRole authentication (900-43200).
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   linode_access_key - string - Linode: Access Key
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   sharepoint_client_id - string - SharePoint: Microsoft Entra application client ID for app-only authentication.
        ///   sharepoint_tenant_id - string - SharePoint: Microsoft Entra tenant ID for app-only authentication.
        ///   username - string - Remote server username.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
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
        ///   sharepoint_client_certificate - string - SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        ///   sharepoint_client_secret - string - SharePoint: Microsoft Entra application client secret for app-only authentication.
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The RemoteServerCredential it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<RemoteServerCredential> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<RemoteServerCredential> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The RemoteServerCredential has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("server_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_type is null, which cannot be sent" : "Bad parameter: server_type must be of type string", "server_type");
            }
            if (parameters.TryGetValue("aws_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_access_key is null, which cannot be sent" : "Bad parameter: aws_access_key must be of type string", "aws_access_key");
            }
            if (parameters.TryGetValue("s3_assume_role_arn", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_arn is null, which cannot be sent" : "Bad parameter: s3_assume_role_arn must be of type string", "s3_assume_role_arn");
            }
            if (parameters.TryGetValue("s3_assume_role_duration_seconds", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_duration_seconds is null, which cannot be sent" : "Bad parameter: s3_assume_role_duration_seconds must be of type Nullable<Int64>", "s3_assume_role_duration_seconds");
            }
            if (parameters.TryGetValue("cloudflare_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_access_key is null, which cannot be sent" : "Bad parameter: cloudflare_access_key must be of type string", "cloudflare_access_key");
            }
            if (parameters.TryGetValue("filebase_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_access_key is null, which cannot be sent" : "Bad parameter: filebase_access_key must be of type string", "filebase_access_key");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "google_cloud_storage_s3_compatible_access_key");
            }
            if (parameters.TryGetValue("linode_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_access_key is null, which cannot be sent" : "Bad parameter: linode_access_key must be of type string", "linode_access_key");
            }
            if (parameters.TryGetValue("s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: s3_compatible_access_key must be of type string", "s3_compatible_access_key");
            }
            if (parameters.TryGetValue("sharepoint_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_id is null, which cannot be sent" : "Bad parameter: sharepoint_client_id must be of type string", "sharepoint_client_id");
            }
            if (parameters.TryGetValue("sharepoint_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_tenant_id is null, which cannot be sent" : "Bad parameter: sharepoint_tenant_id must be of type string", "sharepoint_tenant_id");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("wasabi_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_access_key is null, which cannot be sent" : "Bad parameter: wasabi_access_key must be of type string", "wasabi_access_key");
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
            if (parameters.TryGetValue("sharepoint_client_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_certificate is null, which cannot be sent" : "Bad parameter: sharepoint_client_certificate must be of type string", "sharepoint_client_certificate");
            }
            if (parameters.TryGetValue("sharepoint_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_secret is null, which cannot be sent" : "Bad parameter: sharepoint_client_secret must be of type string", "sharepoint_client_secret");
            }
            if (parameters.TryGetValue("wasabi_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_secret_key is null, which cannot be sent" : "Bad parameter: wasabi_secret_key must be of type string", "wasabi_secret_key");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_server_credentials/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            RemoteServerCredential updated = JsonUtil.DeserializeModelResponse<RemoteServerCredential>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<RemoteServerCredential>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /remote_server_credentials/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /remote_server_credentials/{id} to the Files.com API.
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
                throw new MissingParameterException("The RemoteServerCredential has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/remote_server_credentials/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "name", "description", "server_type", "aws_access_key", "s3_assume_role_arn", "s3_assume_role_duration_seconds", "cloudflare_access_key", "filebase_access_key", "google_cloud_storage_s3_compatible_access_key", "linode_access_key", "s3_compatible_access_key", "sharepoint_client_id", "sharepoint_tenant_id", "username", "wasabi_access_key", "password", "private_key", "private_key_passphrase", "aws_secret_key", "azure_blob_storage_access_key", "azure_blob_storage_sas_token", "azure_files_storage_access_key", "azure_files_storage_sas_token", "backblaze_b2_application_key", "backblaze_b2_key_id", "cloudflare_secret_key", "filebase_secret_key", "google_cloud_storage_credentials_json", "google_cloud_storage_s3_compatible_secret_key", "linode_secret_key", "s3_compatible_secret_key", "sharepoint_client_certificate", "sharepoint_client_secret", "wasabi_secret_key", "workspace_id", "copy_values_from_credential_id" };
        private static readonly string[] UpdateParameterNames = { "id", "name", "description", "server_type", "aws_access_key", "s3_assume_role_arn", "s3_assume_role_duration_seconds", "cloudflare_access_key", "filebase_access_key", "google_cloud_storage_s3_compatible_access_key", "linode_access_key", "s3_compatible_access_key", "sharepoint_client_id", "sharepoint_tenant_id", "username", "wasabi_access_key", "password", "private_key", "private_key_passphrase", "aws_secret_key", "azure_blob_storage_access_key", "azure_blob_storage_sas_token", "azure_files_storage_access_key", "azure_files_storage_sas_token", "backblaze_b2_application_key", "backblaze_b2_key_id", "cloudflare_secret_key", "filebase_secret_key", "google_cloud_storage_credentials_json", "google_cloud_storage_s3_compatible_secret_key", "linode_secret_key", "s3_compatible_secret_key", "sharepoint_client_certificate", "sharepoint_client_secret", "wasabi_secret_key" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this RemoteServerCredential on Files.com, or updates it if it has an ID.
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
            RemoteServerCredential created = await RemoteServerCredential.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id` and `name`. Valid field combinations are `[ workspace_id, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<RemoteServerCredential> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id` and `name`. Valid field combinations are `[ workspace_id, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<RemoteServerCredential> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<RemoteServerCredential> ListCore(
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
            if (parameters.TryGetValue("filter_prefix", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_prefix is null, which cannot be sent" : "Bad parameter: filter_prefix must be of type object", "filter_prefix");
            }

            return new FilesList<RemoteServerCredential>(client, $"/remote_server_credentials", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server Credential ID.
        /// </summary>
        /// <param name="id">Remote Server Credential ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServerCredential> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server Credential ID.
        /// </summary>
        /// <param name="id">Remote Server Credential ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServerCredential> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<RemoteServerCredential> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_server_credentials/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteServerCredential>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Internal name for your reference
        ///   description - string - Internal description for your reference
        ///   server_type - string - Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        ///   aws_access_key - string - AWS Access Key.
        ///   s3_assume_role_arn - string - AWS IAM Role ARN for AssumeRole authentication.
        ///   s3_assume_role_duration_seconds - int64 - Session duration in seconds for AssumeRole authentication (900-43200).
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   linode_access_key - string - Linode: Access Key
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   sharepoint_client_id - string - SharePoint: Microsoft Entra application client ID for app-only authentication.
        ///   sharepoint_tenant_id - string - SharePoint: Microsoft Entra tenant ID for app-only authentication.
        ///   username - string - Remote server username.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
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
        ///   sharepoint_client_certificate - string - SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        ///   sharepoint_client_secret - string - SharePoint: Microsoft Entra application client secret for app-only authentication.
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        ///   workspace_id - int64 - Workspace ID (0 for default workspace)
        ///   copy_values_from_credential_id - int64 - ID of Remote Server Credential to copy omitted values from.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServerCredential> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteServerCredential> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("server_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_type is null, which cannot be sent" : "Bad parameter: server_type must be of type string", "server_type");
            }
            if (parameters.TryGetValue("aws_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_access_key is null, which cannot be sent" : "Bad parameter: aws_access_key must be of type string", "aws_access_key");
            }
            if (parameters.TryGetValue("s3_assume_role_arn", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_arn is null, which cannot be sent" : "Bad parameter: s3_assume_role_arn must be of type string", "s3_assume_role_arn");
            }
            if (parameters.TryGetValue("s3_assume_role_duration_seconds", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_duration_seconds is null, which cannot be sent" : "Bad parameter: s3_assume_role_duration_seconds must be of type Nullable<Int64>", "s3_assume_role_duration_seconds");
            }
            if (parameters.TryGetValue("cloudflare_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_access_key is null, which cannot be sent" : "Bad parameter: cloudflare_access_key must be of type string", "cloudflare_access_key");
            }
            if (parameters.TryGetValue("filebase_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_access_key is null, which cannot be sent" : "Bad parameter: filebase_access_key must be of type string", "filebase_access_key");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "google_cloud_storage_s3_compatible_access_key");
            }
            if (parameters.TryGetValue("linode_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_access_key is null, which cannot be sent" : "Bad parameter: linode_access_key must be of type string", "linode_access_key");
            }
            if (parameters.TryGetValue("s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: s3_compatible_access_key must be of type string", "s3_compatible_access_key");
            }
            if (parameters.TryGetValue("sharepoint_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_id is null, which cannot be sent" : "Bad parameter: sharepoint_client_id must be of type string", "sharepoint_client_id");
            }
            if (parameters.TryGetValue("sharepoint_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_tenant_id is null, which cannot be sent" : "Bad parameter: sharepoint_tenant_id must be of type string", "sharepoint_tenant_id");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("wasabi_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_access_key is null, which cannot be sent" : "Bad parameter: wasabi_access_key must be of type string", "wasabi_access_key");
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
            if (parameters.TryGetValue("sharepoint_client_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_certificate is null, which cannot be sent" : "Bad parameter: sharepoint_client_certificate must be of type string", "sharepoint_client_certificate");
            }
            if (parameters.TryGetValue("sharepoint_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_secret is null, which cannot be sent" : "Bad parameter: sharepoint_client_secret must be of type string", "sharepoint_client_secret");
            }
            if (parameters.TryGetValue("wasabi_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_secret_key is null, which cannot be sent" : "Bad parameter: wasabi_secret_key must be of type string", "wasabi_secret_key");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("copy_values_from_credential_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: copy_values_from_credential_id is null, which cannot be sent" : "Bad parameter: copy_values_from_credential_id must be of type Nullable<Int64>", "copy_values_from_credential_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_server_credentials", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteServerCredential>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Internal name for your reference
        ///   description - string - Internal description for your reference
        ///   server_type - string - Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        ///   aws_access_key - string - AWS Access Key.
        ///   s3_assume_role_arn - string - AWS IAM Role ARN for AssumeRole authentication.
        ///   s3_assume_role_duration_seconds - int64 - Session duration in seconds for AssumeRole authentication (900-43200).
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   linode_access_key - string - Linode: Access Key
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
        ///   sharepoint_client_id - string - SharePoint: Microsoft Entra application client ID for app-only authentication.
        ///   sharepoint_tenant_id - string - SharePoint: Microsoft Entra tenant ID for app-only authentication.
        ///   username - string - Remote server username.
        ///   wasabi_access_key - string - Wasabi: Access Key.
        ///   password - string - Password, if needed.
        ///   private_key - string - Private key, if needed.
        ///   private_key_passphrase - string - Passphrase for private key if needed.
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
        ///   sharepoint_client_certificate - string - SharePoint: PEM-encoded certificate and unencrypted private key for app-only authentication.
        ///   sharepoint_client_secret - string - SharePoint: Microsoft Entra application client secret for app-only authentication.
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        /// </summary>
        /// <param name="id">Remote Server Credential ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<RemoteServerCredential> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<RemoteServerCredential> UpdateCore(
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
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("server_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: server_type is null, which cannot be sent" : "Bad parameter: server_type must be of type string", "server_type");
            }
            if (parameters.TryGetValue("aws_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: aws_access_key is null, which cannot be sent" : "Bad parameter: aws_access_key must be of type string", "aws_access_key");
            }
            if (parameters.TryGetValue("s3_assume_role_arn", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_arn is null, which cannot be sent" : "Bad parameter: s3_assume_role_arn must be of type string", "s3_assume_role_arn");
            }
            if (parameters.TryGetValue("s3_assume_role_duration_seconds", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_assume_role_duration_seconds is null, which cannot be sent" : "Bad parameter: s3_assume_role_duration_seconds must be of type Nullable<Int64>", "s3_assume_role_duration_seconds");
            }
            if (parameters.TryGetValue("cloudflare_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cloudflare_access_key is null, which cannot be sent" : "Bad parameter: cloudflare_access_key must be of type string", "cloudflare_access_key");
            }
            if (parameters.TryGetValue("filebase_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filebase_access_key is null, which cannot be sent" : "Bad parameter: filebase_access_key must be of type string", "filebase_access_key");
            }
            if (parameters.TryGetValue("google_cloud_storage_s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: google_cloud_storage_s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "google_cloud_storage_s3_compatible_access_key");
            }
            if (parameters.TryGetValue("linode_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: linode_access_key is null, which cannot be sent" : "Bad parameter: linode_access_key must be of type string", "linode_access_key");
            }
            if (parameters.TryGetValue("s3_compatible_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: s3_compatible_access_key is null, which cannot be sent" : "Bad parameter: s3_compatible_access_key must be of type string", "s3_compatible_access_key");
            }
            if (parameters.TryGetValue("sharepoint_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_id is null, which cannot be sent" : "Bad parameter: sharepoint_client_id must be of type string", "sharepoint_client_id");
            }
            if (parameters.TryGetValue("sharepoint_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_tenant_id is null, which cannot be sent" : "Bad parameter: sharepoint_tenant_id must be of type string", "sharepoint_tenant_id");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }
            if (parameters.TryGetValue("wasabi_access_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_access_key is null, which cannot be sent" : "Bad parameter: wasabi_access_key must be of type string", "wasabi_access_key");
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
            if (parameters.TryGetValue("sharepoint_client_certificate", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_certificate is null, which cannot be sent" : "Bad parameter: sharepoint_client_certificate must be of type string", "sharepoint_client_certificate");
            }
            if (parameters.TryGetValue("sharepoint_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sharepoint_client_secret is null, which cannot be sent" : "Bad parameter: sharepoint_client_secret must be of type string", "sharepoint_client_secret");
            }
            if (parameters.TryGetValue("wasabi_secret_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: wasabi_secret_key is null, which cannot be sent" : "Bad parameter: wasabi_secret_key must be of type string", "wasabi_secret_key");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/remote_server_credentials/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<RemoteServerCredential>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /remote_server_credentials/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Remote Server Credential ID.</param>
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
        /// Sends DELETE /remote_server_credentials/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Remote Server Credential ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/remote_server_credentials/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}