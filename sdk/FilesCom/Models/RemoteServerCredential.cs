using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class RemoteServerCredential
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public RemoteServerCredential() : this(null, null) { }

        public RemoteServerCredential(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("google_cloud_storage_s3_compatible_access_key"))
            {
                this.attributes.Add("google_cloud_storage_s3_compatible_access_key", null);
            }
            if (!this.attributes.ContainsKey("wasabi_access_key"))
            {
                this.attributes.Add("wasabi_access_key", null);
            }
            if (!this.attributes.ContainsKey("azure_blob_storage_account"))
            {
                this.attributes.Add("azure_blob_storage_account", null);
            }
            if (!this.attributes.ContainsKey("azure_files_storage_account"))
            {
                this.attributes.Add("azure_files_storage_account", null);
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
        /// Remote Server Credential ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Workspace ID (0 for default workspace)
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; }
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
        /// Internal description for your reference
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; }
        }

        /// <summary>
        /// Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        /// </summary>
        [JsonPropertyName("server_type")]
        public string ServerType
        {
            get { return (string)attributes["server_type"]; }
            set { attributes["server_type"] = value; }
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
        /// Google Cloud Storage: S3-compatible Access Key.
        /// </summary>
        [JsonPropertyName("google_cloud_storage_s3_compatible_access_key")]
        public string GoogleCloudStorageS3CompatibleAccessKey
        {
            get { return (string)attributes["google_cloud_storage_s3_compatible_access_key"]; }
            set { attributes["google_cloud_storage_s3_compatible_access_key"] = value; }
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
        /// Azure Blob Storage: Account name
        /// </summary>
        [JsonPropertyName("azure_blob_storage_account")]
        public string AzureBlobStorageAccount
        {
            get { return (string)attributes["azure_blob_storage_account"]; }
            set { attributes["azure_blob_storage_account"] = value; }
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
        /// S3-compatible: Access Key
        /// </summary>
        [JsonPropertyName("s3_compatible_access_key")]
        public string S3CompatibleAccessKey
        {
            get { return (string)attributes["s3_compatible_access_key"]; }
            set { attributes["s3_compatible_access_key"] = value; }
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
        /// Cloudflare: Access Key.
        /// </summary>
        [JsonPropertyName("cloudflare_access_key")]
        public string CloudflareAccessKey
        {
            get { return (string)attributes["cloudflare_access_key"]; }
            set { attributes["cloudflare_access_key"] = value; }
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
        /// Remote server username.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; }
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
        /// Parameters:
        ///   workspace_id - int64 - Workspace ID (0 for default workspace)
        ///   name - string - Internal name for your reference
        ///   description - string - Internal description for your reference
        ///   server_type - string - Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        ///   aws_access_key - string - AWS Access Key.
        ///   azure_blob_storage_account - string - Azure Blob Storage: Account name
        ///   azure_files_storage_account - string - Azure Files: Storage Account name
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   linode_access_key - string - Linode: Access Key
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
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
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        /// </summary>
        public async Task<RemoteServerCredential> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("server_type") && !(parameters["server_type"] is string))
            {
                throw new ArgumentException("Bad parameter: server_type must be of type string", "parameters[\"server_type\"]");
            }
            if (parameters.ContainsKey("aws_access_key") && !(parameters["aws_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_access_key must be of type string", "parameters[\"aws_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_account") && !(parameters["azure_blob_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_account must be of type string", "parameters[\"azure_blob_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_account") && !(parameters["azure_files_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_account must be of type string", "parameters[\"azure_files_storage_account\"]");
            }
            if (parameters.ContainsKey("cloudflare_access_key") && !(parameters["cloudflare_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_access_key must be of type string", "parameters[\"cloudflare_access_key\"]");
            }
            if (parameters.ContainsKey("filebase_access_key") && !(parameters["filebase_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_access_key must be of type string", "parameters[\"filebase_access_key\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_access_key") && !(parameters["google_cloud_storage_s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("linode_access_key") && !(parameters["linode_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_access_key must be of type string", "parameters[\"linode_access_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_access_key") && !(parameters["s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_access_key must be of type string", "parameters[\"s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (parameters.ContainsKey("wasabi_access_key") && !(parameters["wasabi_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_access_key must be of type string", "parameters[\"wasabi_access_key\"]");
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

            string responseJson = await FilesClient.SendStringRequest($"/remote_server_credentials/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServerCredential>(responseJson);
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

            await FilesClient.SendRequest($"/remote_server_credentials/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await RemoteServerCredential.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id` and `name`. Valid field combinations are `[ workspace_id, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
        /// </summary>
        public static FilesList<RemoteServerCredential> List(

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

            return new FilesList<RemoteServerCredential>($"/remote_server_credentials", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<RemoteServerCredential> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Remote Server Credential ID.
        /// </summary>
        public static async Task<RemoteServerCredential> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/remote_server_credentials/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServerCredential>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<RemoteServerCredential> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   workspace_id - int64 - Workspace ID (0 for default workspace)
        ///   name - string - Internal name for your reference
        ///   description - string - Internal description for your reference
        ///   server_type - string - Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        ///   aws_access_key - string - AWS Access Key.
        ///   azure_blob_storage_account - string - Azure Blob Storage: Account name
        ///   azure_files_storage_account - string - Azure Files: Storage Account name
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   linode_access_key - string - Linode: Access Key
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
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
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        /// </summary>
        public static async Task<RemoteServerCredential> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("server_type") && !(parameters["server_type"] is string))
            {
                throw new ArgumentException("Bad parameter: server_type must be of type string", "parameters[\"server_type\"]");
            }
            if (parameters.ContainsKey("aws_access_key") && !(parameters["aws_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_access_key must be of type string", "parameters[\"aws_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_account") && !(parameters["azure_blob_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_account must be of type string", "parameters[\"azure_blob_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_account") && !(parameters["azure_files_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_account must be of type string", "parameters[\"azure_files_storage_account\"]");
            }
            if (parameters.ContainsKey("cloudflare_access_key") && !(parameters["cloudflare_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_access_key must be of type string", "parameters[\"cloudflare_access_key\"]");
            }
            if (parameters.ContainsKey("filebase_access_key") && !(parameters["filebase_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_access_key must be of type string", "parameters[\"filebase_access_key\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_access_key") && !(parameters["google_cloud_storage_s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("linode_access_key") && !(parameters["linode_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_access_key must be of type string", "parameters[\"linode_access_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_access_key") && !(parameters["s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_access_key must be of type string", "parameters[\"s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (parameters.ContainsKey("wasabi_access_key") && !(parameters["wasabi_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_access_key must be of type string", "parameters[\"wasabi_access_key\"]");
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

            string responseJson = await FilesClient.SendStringRequest($"/remote_server_credentials", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServerCredential>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   workspace_id - int64 - Workspace ID (0 for default workspace)
        ///   name - string - Internal name for your reference
        ///   description - string - Internal description for your reference
        ///   server_type - string - Remote server type.  Remote Server Credentials are only valid for a single type of Remote Server.
        ///   aws_access_key - string - AWS Access Key.
        ///   azure_blob_storage_account - string - Azure Blob Storage: Account name
        ///   azure_files_storage_account - string - Azure Files: Storage Account name
        ///   cloudflare_access_key - string - Cloudflare: Access Key.
        ///   filebase_access_key - string - Filebase: Access Key.
        ///   google_cloud_storage_s3_compatible_access_key - string - Google Cloud Storage: S3-compatible Access Key.
        ///   linode_access_key - string - Linode: Access Key
        ///   s3_compatible_access_key - string - S3-compatible: Access Key
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
        ///   wasabi_secret_key - string - Wasabi: Secret Key
        /// </summary>
        public static async Task<RemoteServerCredential> Update(
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
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("server_type") && !(parameters["server_type"] is string))
            {
                throw new ArgumentException("Bad parameter: server_type must be of type string", "parameters[\"server_type\"]");
            }
            if (parameters.ContainsKey("aws_access_key") && !(parameters["aws_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: aws_access_key must be of type string", "parameters[\"aws_access_key\"]");
            }
            if (parameters.ContainsKey("azure_blob_storage_account") && !(parameters["azure_blob_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_blob_storage_account must be of type string", "parameters[\"azure_blob_storage_account\"]");
            }
            if (parameters.ContainsKey("azure_files_storage_account") && !(parameters["azure_files_storage_account"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_files_storage_account must be of type string", "parameters[\"azure_files_storage_account\"]");
            }
            if (parameters.ContainsKey("cloudflare_access_key") && !(parameters["cloudflare_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: cloudflare_access_key must be of type string", "parameters[\"cloudflare_access_key\"]");
            }
            if (parameters.ContainsKey("filebase_access_key") && !(parameters["filebase_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: filebase_access_key must be of type string", "parameters[\"filebase_access_key\"]");
            }
            if (parameters.ContainsKey("google_cloud_storage_s3_compatible_access_key") && !(parameters["google_cloud_storage_s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: google_cloud_storage_s3_compatible_access_key must be of type string", "parameters[\"google_cloud_storage_s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("linode_access_key") && !(parameters["linode_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: linode_access_key must be of type string", "parameters[\"linode_access_key\"]");
            }
            if (parameters.ContainsKey("s3_compatible_access_key") && !(parameters["s3_compatible_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: s3_compatible_access_key must be of type string", "parameters[\"s3_compatible_access_key\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }
            if (parameters.ContainsKey("wasabi_access_key") && !(parameters["wasabi_access_key"] is string))
            {
                throw new ArgumentException("Bad parameter: wasabi_access_key must be of type string", "parameters[\"wasabi_access_key\"]");
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

            string responseJson = await FilesClient.SendStringRequest($"/remote_server_credentials/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<RemoteServerCredential>(responseJson);
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

            await FilesClient.SendRequest($"/remote_server_credentials/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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