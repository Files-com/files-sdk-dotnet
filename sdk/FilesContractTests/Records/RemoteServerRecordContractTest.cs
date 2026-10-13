using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FilesContractTests.Records
{
    // Imported inside the namespace so that model names, such as Lock, take precedence over those of the namespaces
    // above, such as System.Threading.Lock.
    using FilesCom.Models;

    // The record contract of RemoteServer, generated from the schema: each case sends one request through the
    // SDK's RemoteServers operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class RemoteServerRecordContractTest : ContractCase
    {
        private const string ListPath = "/remote_servers";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("RemoteServer", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["allow_relative_paths"] = true, ["aws_access_key"] = "contract-aws_access_key", ["azure_blob_storage_account"] = "contract-azure_blob_storage_account", ["azure_blob_storage_container"] = "contract-azure_blob_storage_container", ["azure_blob_storage_dns_suffix"] = "contract-azure_blob_storage_dns_suffix", ["azure_blob_storage_hierarchical_namespace"] = true, ["azure_files_storage_account"] = "contract-azure_files_storage_account", ["azure_files_storage_dns_suffix"] = "contract-azure_files_storage_dns_suffix", ["azure_files_storage_share_name"] = "contract-azure_files_storage_share_name", ["backblaze_b2_bucket"] = "contract-backblaze_b2_bucket", ["backblaze_b2_s3_endpoint"] = "contract-backblaze_b2_s3_endpoint", ["buffer_uploads"] = "auto", ["cloudflare_access_key"] = "contract-cloudflare_access_key", ["cloudflare_bucket"] = "contract-cloudflare_bucket", ["cloudflare_endpoint"] = "contract-cloudflare_endpoint", ["description"] = "contract-description", ["dropbox_teams"] = true, ["enable_dedicated_ips"] = true, ["filebase_access_key"] = "contract-filebase_access_key", ["filebase_bucket"] = "contract-filebase_bucket", ["files_agent_permission_set"] = "read_write", ["files_agent_root"] = "contract-files_agent_root", ["files_agent_version"] = "contract-files_agent_version", ["outbound_agent_id"] = (Int64)7, ["custom_domain_id"] = (Int64)7, ["google_cloud_storage_authentication_method"] = "json", ["google_cloud_storage_bucket"] = "contract-google_cloud_storage_bucket", ["google_cloud_storage_oauth_scope"] = "contract-google_cloud_storage_oauth_scope", ["google_cloud_storage_project_id"] = "contract-google_cloud_storage_project_id", ["google_cloud_storage_s3_compatible_access_key"] = "contract-google_cloud_storage_s3_compatible_access_key", ["hostname"] = "contract-hostname", ["linode_access_key"] = "contract-linode_access_key", ["linode_bucket"] = "contract-linode_bucket", ["linode_region"] = "contract-linode_region", ["max_connections"] = (Int64)7, ["name"] = "contract-name", ["one_drive_account_type"] = "personal", ["pin_to_site_region"] = true, ["port"] = (Int64)7, ["upload_staging_path"] = "contract-upload_staging_path", ["remote_server_credential_id"] = (Int64)7, ["s3_assume_role_arn"] = "contract-s3_assume_role_arn", ["s3_assume_role_duration_seconds"] = (Int64)7, ["s3_bucket"] = "contract-s3_bucket", ["s3_compatible_access_key"] = "contract-s3_compatible_access_key", ["s3_compatible_bucket"] = "contract-s3_compatible_bucket", ["s3_compatible_endpoint"] = "contract-s3_compatible_endpoint", ["s3_compatible_region"] = "contract-s3_compatible_region", ["s3_compatible_virtual_hosted_style"] = true, ["s3_kms_key_id"] = "contract-s3_kms_key_id", ["s3_region"] = "contract-s3_region", ["server_certificate"] = "require_match", ["server_host_key"] = "contract-server_host_key", ["server_type"] = "ftp", ["sharepoint_client_id"] = "contract-sharepoint_client_id", ["sharepoint_site_url"] = "contract-sharepoint_site_url", ["sharepoint_tenant_id"] = "contract-sharepoint_tenant_id", ["ssl"] = "if_available", ["username"] = "contract-username", ["wasabi_access_key"] = "contract-wasabi_access_key", ["wasabi_bucket"] = "contract-wasabi_bucket", ["wasabi_region"] = "contract-wasabi_region", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "RemoteServer", ("id", "int64"), ("disabled", "boolean"), ("authentication_method", "string"), ("hostname", "string"), ("remote_home_path", "string"), ("upload_staging_path", "string"), ("allow_relative_paths", "boolean"), ("name", "string"), ("description", "string"), ("port", "int64"), ("buffer_uploads", "string"), ("max_connections", "int64"), ("pin_to_site_region", "boolean"), ("pinned_region", "string"), ("remote_server_credential_id", "int64"), ("s3_bucket", "string"), ("s3_region", "string"), ("s3_kms_key_id", "string"), ("aws_access_key", "string"), ("s3_assume_role_arn", "string"), ("s3_assume_role_duration_seconds", "int64"), ("s3_assume_role_external_id", "string"), ("server_certificate", "string"), ("server_host_key", "string"), ("server_type", "string"), ("workspace_id", "int64"), ("ssl", "string"), ("username", "string"), ("google_cloud_storage_bucket", "string"), ("google_cloud_storage_authentication_method", "string"), ("google_cloud_storage_oauth_scope", "string"), ("google_cloud_storage_project_id", "string"), ("google_cloud_storage_s3_compatible_access_key", "string"), ("backblaze_b2_s3_endpoint", "string"), ("backblaze_b2_bucket", "string"), ("wasabi_bucket", "string"), ("wasabi_region", "string"), ("wasabi_access_key", "string"), ("auth_status", "string"), ("auth_account_name", "string"), ("one_drive_account_type", "string"), ("sharepoint_tenant_id", "string"), ("sharepoint_client_id", "string"), ("sharepoint_app_authentication", "boolean"), ("sharepoint_app_credential_type", "string"), ("sharepoint_site_url", "string"), ("azure_blob_storage_account", "string"), ("azure_blob_storage_container", "string"), ("azure_blob_storage_hierarchical_namespace", "boolean"), ("azure_blob_storage_dns_suffix", "string"), ("azure_files_storage_account", "string"), ("azure_files_storage_share_name", "string"), ("azure_files_storage_dns_suffix", "string"), ("s3_compatible_bucket", "string"), ("s3_compatible_endpoint", "string"), ("s3_compatible_region", "string"), ("s3_compatible_virtual_hosted_style", "boolean"), ("s3_compatible_access_key", "string"), ("enable_dedicated_ips", "boolean"), ("custom_domain_id", "int64"), ("outbound_ip_addresses", "array(string)"), ("files_agent_permission_set", "string"), ("files_agent_root", "string"), ("files_agent_version", "string"), ("files_agent_up_to_date", "boolean"), ("files_agent_latest_version", "string"), ("files_agent_supports_push_updates", "boolean"), ("direct_transfer_available", "boolean"), ("outbound_agent_id", "int64"), ("filebase_bucket", "string"), ("filebase_access_key", "string"), ("files_api_key_prefix", "string"), ("cloudflare_bucket", "string"), ("cloudflare_access_key", "string"), ("cloudflare_endpoint", "string"), ("dropbox_teams", "boolean"), ("linode_bucket", "string"), ("linode_access_key", "string"), ("linode_region", "string"), ("supports_versioning", "boolean"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the RemoteServer the SDK returns gives it.
        private static void AssertModel(JsonObject record, RemoteServer model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no RemoteServer");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "disabled", model.Disabled, what);
            Simulator.Same(record, "authentication_method", model.AuthenticationMethod, what);
            Simulator.Same(record, "hostname", model.Hostname, what);
            Simulator.Same(record, "remote_home_path", model.RemoteHomePath, what);
            Simulator.Same(record, "upload_staging_path", model.UploadStagingPath, what);
            Simulator.Same(record, "allow_relative_paths", model.AllowRelativePaths, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "port", model.Port, what);
            Simulator.Same(record, "buffer_uploads", model.BufferUploads, what);
            Simulator.Same(record, "max_connections", model.MaxConnections, what);
            Simulator.Same(record, "pin_to_site_region", model.PinToSiteRegion, what);
            Simulator.Same(record, "pinned_region", model.PinnedRegion, what);
            Simulator.Same(record, "remote_server_credential_id", model.RemoteServerCredentialId, what);
            Simulator.Same(record, "s3_bucket", model.S3Bucket, what);
            Simulator.Same(record, "s3_region", model.S3Region, what);
            Simulator.Same(record, "s3_kms_key_id", model.S3KmsKeyId, what);
            Simulator.Same(record, "aws_access_key", model.AwsAccessKey, what);
            Simulator.Same(record, "s3_assume_role_arn", model.S3AssumeRoleArn, what);
            Simulator.Same(record, "s3_assume_role_duration_seconds", model.S3AssumeRoleDurationSeconds, what);
            Simulator.Same(record, "s3_assume_role_external_id", model.S3AssumeRoleExternalId, what);
            Simulator.Same(record, "server_certificate", model.ServerCertificate, what);
            Simulator.Same(record, "server_host_key", model.ServerHostKey, what);
            Simulator.Same(record, "server_type", model.ServerType, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "ssl", model.Ssl, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "google_cloud_storage_bucket", model.GoogleCloudStorageBucket, what);
            Simulator.Same(record, "google_cloud_storage_authentication_method", model.GoogleCloudStorageAuthenticationMethod, what);
            Simulator.Same(record, "google_cloud_storage_oauth_scope", model.GoogleCloudStorageOauthScope, what);
            Simulator.Same(record, "google_cloud_storage_project_id", model.GoogleCloudStorageProjectId, what);
            Simulator.Same(record, "google_cloud_storage_s3_compatible_access_key", model.GoogleCloudStorageS3CompatibleAccessKey, what);
            Simulator.Same(record, "backblaze_b2_s3_endpoint", model.BackblazeB2S3Endpoint, what);
            Simulator.Same(record, "backblaze_b2_bucket", model.BackblazeB2Bucket, what);
            Simulator.Same(record, "wasabi_bucket", model.WasabiBucket, what);
            Simulator.Same(record, "wasabi_region", model.WasabiRegion, what);
            Simulator.Same(record, "wasabi_access_key", model.WasabiAccessKey, what);
            Simulator.Same(record, "auth_status", model.AuthStatus, what);
            Simulator.Same(record, "auth_account_name", model.AuthAccountName, what);
            Simulator.Same(record, "one_drive_account_type", model.OneDriveAccountType, what);
            Simulator.Same(record, "sharepoint_tenant_id", model.SharepointTenantId, what);
            Simulator.Same(record, "sharepoint_client_id", model.SharepointClientId, what);
            Simulator.Same(record, "sharepoint_app_authentication", model.SharepointAppAuthentication, what);
            Simulator.Same(record, "sharepoint_app_credential_type", model.SharepointAppCredentialType, what);
            Simulator.Same(record, "sharepoint_site_url", model.SharepointSiteUrl, what);
            Simulator.Same(record, "azure_blob_storage_account", model.AzureBlobStorageAccount, what);
            Simulator.Same(record, "azure_blob_storage_container", model.AzureBlobStorageContainer, what);
            Simulator.Same(record, "azure_blob_storage_hierarchical_namespace", model.AzureBlobStorageHierarchicalNamespace, what);
            Simulator.Same(record, "azure_blob_storage_dns_suffix", model.AzureBlobStorageDnsSuffix, what);
            Simulator.Same(record, "azure_files_storage_account", model.AzureFilesStorageAccount, what);
            Simulator.Same(record, "azure_files_storage_share_name", model.AzureFilesStorageShareName, what);
            Simulator.Same(record, "azure_files_storage_dns_suffix", model.AzureFilesStorageDnsSuffix, what);
            Simulator.Same(record, "s3_compatible_bucket", model.S3CompatibleBucket, what);
            Simulator.Same(record, "s3_compatible_endpoint", model.S3CompatibleEndpoint, what);
            Simulator.Same(record, "s3_compatible_region", model.S3CompatibleRegion, what);
            Simulator.Same(record, "s3_compatible_virtual_hosted_style", model.S3CompatibleVirtualHostedStyle, what);
            Simulator.Same(record, "s3_compatible_access_key", model.S3CompatibleAccessKey, what);
            Simulator.Same(record, "enable_dedicated_ips", model.EnableDedicatedIps, what);
            Simulator.Same(record, "custom_domain_id", model.CustomDomainId, what);
            Simulator.Same(record, "outbound_ip_addresses", model.OutboundIpAddresses, what);
            Simulator.Same(record, "files_agent_permission_set", model.FilesAgentPermissionSet, what);
            Simulator.Same(record, "files_agent_root", model.FilesAgentRoot, what);
            Simulator.Same(record, "files_agent_version", model.FilesAgentVersion, what);
            Simulator.Same(record, "files_agent_up_to_date", model.FilesAgentUpToDate, what);
            Simulator.Same(record, "files_agent_latest_version", model.FilesAgentLatestVersion, what);
            Simulator.Same(record, "files_agent_supports_push_updates", model.FilesAgentSupportsPushUpdates, what);
            Simulator.Same(record, "direct_transfer_available", model.DirectTransferAvailable, what);
            Simulator.Same(record, "outbound_agent_id", model.OutboundAgentId, what);
            Simulator.Same(record, "filebase_bucket", model.FilebaseBucket, what);
            Simulator.Same(record, "filebase_access_key", model.FilebaseAccessKey, what);
            Simulator.Same(record, "files_api_key_prefix", model.FilesApiKeyPrefix, what);
            Simulator.Same(record, "cloudflare_bucket", model.CloudflareBucket, what);
            Simulator.Same(record, "cloudflare_access_key", model.CloudflareAccessKey, what);
            Simulator.Same(record, "cloudflare_endpoint", model.CloudflareEndpoint, what);
            Simulator.Same(record, "dropbox_teams", model.DropboxTeams, what);
            Simulator.Same(record, "linode_bucket", model.LinodeBucket, what);
            Simulator.Same(record, "linode_access_key", model.LinodeAccessKey, what);
            Simulator.Same(record, "linode_region", model.LinodeRegion, what);
            Simulator.Same(record, "supports_versioning", model.SupportsVersioning, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServer", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["allow_relative_paths"] = true, ["aws_access_key"] = "contract-aws_access_key", ["azure_blob_storage_account"] = "contract-azure_blob_storage_account", ["azure_blob_storage_container"] = "contract-azure_blob_storage_container", ["azure_blob_storage_dns_suffix"] = "contract-azure_blob_storage_dns_suffix", ["azure_blob_storage_hierarchical_namespace"] = true, ["azure_files_storage_account"] = "contract-azure_files_storage_account", ["azure_files_storage_dns_suffix"] = "contract-azure_files_storage_dns_suffix", ["azure_files_storage_share_name"] = "contract-azure_files_storage_share_name", ["backblaze_b2_bucket"] = "contract-backblaze_b2_bucket", ["backblaze_b2_s3_endpoint"] = "contract-backblaze_b2_s3_endpoint", ["buffer_uploads"] = "auto", ["cloudflare_access_key"] = "contract-cloudflare_access_key", ["cloudflare_bucket"] = "contract-cloudflare_bucket", ["cloudflare_endpoint"] = "contract-cloudflare_endpoint", ["description"] = "contract-description", ["dropbox_teams"] = true, ["enable_dedicated_ips"] = true, ["filebase_access_key"] = "contract-filebase_access_key", ["filebase_bucket"] = "contract-filebase_bucket", ["files_agent_permission_set"] = "read_write", ["files_agent_root"] = "contract-files_agent_root", ["files_agent_version"] = "contract-files_agent_version", ["outbound_agent_id"] = (Int64)7, ["custom_domain_id"] = (Int64)7, ["google_cloud_storage_authentication_method"] = "json", ["google_cloud_storage_bucket"] = "contract-google_cloud_storage_bucket", ["google_cloud_storage_oauth_scope"] = "contract-google_cloud_storage_oauth_scope", ["google_cloud_storage_project_id"] = "contract-google_cloud_storage_project_id", ["google_cloud_storage_s3_compatible_access_key"] = "contract-google_cloud_storage_s3_compatible_access_key", ["hostname"] = "contract-hostname", ["linode_access_key"] = "contract-linode_access_key", ["linode_bucket"] = "contract-linode_bucket", ["linode_region"] = "contract-linode_region", ["max_connections"] = (Int64)7, ["name"] = "contract-name", ["one_drive_account_type"] = "personal", ["pin_to_site_region"] = true, ["port"] = (Int64)7, ["upload_staging_path"] = "contract-upload_staging_path", ["remote_server_credential_id"] = (Int64)7, ["s3_assume_role_arn"] = "contract-s3_assume_role_arn", ["s3_assume_role_duration_seconds"] = (Int64)7, ["s3_bucket"] = "contract-s3_bucket", ["s3_compatible_access_key"] = "contract-s3_compatible_access_key", ["s3_compatible_bucket"] = "contract-s3_compatible_bucket", ["s3_compatible_endpoint"] = "contract-s3_compatible_endpoint", ["s3_compatible_region"] = "contract-s3_compatible_region", ["s3_compatible_virtual_hosted_style"] = true, ["s3_kms_key_id"] = "contract-s3_kms_key_id", ["s3_region"] = "contract-s3_region", ["server_certificate"] = "require_match", ["server_host_key"] = "contract-server_host_key", ["server_type"] = "ftp", ["sharepoint_client_id"] = "contract-sharepoint_client_id", ["sharepoint_site_url"] = "contract-sharepoint_site_url", ["sharepoint_tenant_id"] = "contract-sharepoint_tenant_id", ["ssl"] = "if_available", ["username"] = "contract-username", ["wasabi_access_key"] = "contract-wasabi_access_key", ["wasabi_bucket"] = "contract-wasabi_bucket", ["wasabi_region"] = "contract-wasabi_region", ["workspace_id"] = (Int64)7 };
            RemoteServer created = await Simulator.Sdk(operation).RemoteServers.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["allow_relative_paths"] = "boolean", ["aws_access_key"] = "string", ["azure_blob_storage_account"] = "string", ["azure_blob_storage_container"] = "string", ["azure_blob_storage_dns_suffix"] = "string", ["azure_blob_storage_hierarchical_namespace"] = "boolean", ["azure_files_storage_account"] = "string", ["azure_files_storage_dns_suffix"] = "string", ["azure_files_storage_share_name"] = "string", ["backblaze_b2_bucket"] = "string", ["backblaze_b2_s3_endpoint"] = "string", ["buffer_uploads"] = "string", ["cloudflare_access_key"] = "string", ["cloudflare_bucket"] = "string", ["cloudflare_endpoint"] = "string", ["description"] = "string", ["dropbox_teams"] = "boolean", ["enable_dedicated_ips"] = "boolean", ["filebase_access_key"] = "string", ["filebase_bucket"] = "string", ["files_agent_permission_set"] = "string", ["files_agent_root"] = "string", ["files_agent_version"] = "string", ["outbound_agent_id"] = "number", ["custom_domain_id"] = "number", ["google_cloud_storage_authentication_method"] = "string", ["google_cloud_storage_bucket"] = "string", ["google_cloud_storage_oauth_scope"] = "string", ["google_cloud_storage_project_id"] = "string", ["google_cloud_storage_s3_compatible_access_key"] = "string", ["hostname"] = "string", ["linode_access_key"] = "string", ["linode_bucket"] = "string", ["linode_region"] = "string", ["max_connections"] = "number", ["name"] = "string", ["one_drive_account_type"] = "string", ["pin_to_site_region"] = "boolean", ["port"] = "number", ["upload_staging_path"] = "string", ["remote_server_credential_id"] = "number", ["s3_assume_role_arn"] = "string", ["s3_assume_role_duration_seconds"] = "number", ["s3_bucket"] = "string", ["s3_compatible_access_key"] = "string", ["s3_compatible_bucket"] = "string", ["s3_compatible_endpoint"] = "string", ["s3_compatible_region"] = "string", ["s3_compatible_virtual_hosted_style"] = "boolean", ["s3_kms_key_id"] = "string", ["s3_region"] = "string", ["server_certificate"] = "string", ["server_host_key"] = "string", ["server_type"] = "string", ["sharepoint_client_id"] = "string", ["sharepoint_site_url"] = "string", ["sharepoint_tenant_id"] = "string", ["ssl"] = "string", ["username"] = "string", ["wasabi_access_key"] = "string", ["wasabi_bucket"] = "string", ["wasabi_region"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "RemoteServer.create", "allow_relative_paths", "aws_access_key", "azure_blob_storage_account", "azure_blob_storage_container", "azure_blob_storage_dns_suffix", "azure_blob_storage_hierarchical_namespace", "azure_files_storage_account", "azure_files_storage_dns_suffix", "azure_files_storage_share_name", "backblaze_b2_bucket", "backblaze_b2_s3_endpoint", "buffer_uploads", "cloudflare_access_key", "cloudflare_bucket", "cloudflare_endpoint", "description", "dropbox_teams", "enable_dedicated_ips", "filebase_access_key", "filebase_bucket", "files_agent_permission_set", "files_agent_root", "files_agent_version", "outbound_agent_id", "custom_domain_id", "google_cloud_storage_authentication_method", "google_cloud_storage_bucket", "google_cloud_storage_oauth_scope", "google_cloud_storage_project_id", "google_cloud_storage_s3_compatible_access_key", "hostname", "linode_access_key", "linode_bucket", "linode_region", "max_connections", "name", "one_drive_account_type", "pin_to_site_region", "port", "upload_staging_path", "remote_server_credential_id", "s3_assume_role_arn", "s3_assume_role_duration_seconds", "s3_bucket", "s3_compatible_access_key", "s3_compatible_bucket", "s3_compatible_endpoint", "s3_compatible_region", "s3_compatible_virtual_hosted_style", "s3_kms_key_id", "s3_region", "server_certificate", "server_host_key", "server_type", "sharepoint_client_id", "sharepoint_site_url", "sharepoint_tenant_id", "ssl", "username", "wasabi_access_key", "wasabi_bucket", "wasabi_region", "workspace_id");
            AssertModel(record, created, "RemoteServer.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServer", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            RemoteServer[] listed = (await Simulator.Sdk(operation).RemoteServers.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"RemoteServer.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServer", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            RemoteServer found = await Simulator.Sdk(operation).RemoteServers.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "RemoteServer.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServer", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["allow_relative_paths"] = false;
            sent["aws_access_key"] = "contract-aws_access_key-2";
            sent["azure_blob_storage_account"] = "contract-azure_blob_storage_account-2";
            sent["azure_blob_storage_container"] = "contract-azure_blob_storage_container-2";
            sent["azure_blob_storage_dns_suffix"] = "contract-azure_blob_storage_dns_suffix-2";
            sent["azure_blob_storage_hierarchical_namespace"] = false;
            sent["azure_files_storage_account"] = "contract-azure_files_storage_account-2";
            sent["azure_files_storage_dns_suffix"] = "contract-azure_files_storage_dns_suffix-2";
            sent["azure_files_storage_share_name"] = "contract-azure_files_storage_share_name-2";
            sent["backblaze_b2_bucket"] = "contract-backblaze_b2_bucket-2";
            sent["backblaze_b2_s3_endpoint"] = "contract-backblaze_b2_s3_endpoint-2";
            sent["buffer_uploads"] = "always";
            sent["cloudflare_access_key"] = "contract-cloudflare_access_key-2";
            sent["cloudflare_bucket"] = "contract-cloudflare_bucket-2";
            sent["cloudflare_endpoint"] = "contract-cloudflare_endpoint-2";
            sent["description"] = "contract-description-2";
            sent["dropbox_teams"] = false;
            sent["enable_dedicated_ips"] = false;
            sent["filebase_access_key"] = "contract-filebase_access_key-2";
            sent["filebase_bucket"] = "contract-filebase_bucket-2";
            sent["files_agent_permission_set"] = "read_only";
            sent["files_agent_root"] = "contract-files_agent_root-2";
            sent["files_agent_version"] = "contract-files_agent_version-2";
            sent["outbound_agent_id"] = (Int64)8;
            sent["custom_domain_id"] = (Int64)8;
            sent["google_cloud_storage_authentication_method"] = "hmac";
            sent["google_cloud_storage_bucket"] = "contract-google_cloud_storage_bucket-2";
            sent["google_cloud_storage_oauth_scope"] = "contract-google_cloud_storage_oauth_scope-2";
            sent["google_cloud_storage_project_id"] = "contract-google_cloud_storage_project_id-2";
            sent["google_cloud_storage_s3_compatible_access_key"] = "contract-google_cloud_storage_s3_compatible_access_key-2";
            sent["hostname"] = "contract-hostname-2";
            sent["linode_access_key"] = "contract-linode_access_key-2";
            sent["linode_bucket"] = "contract-linode_bucket-2";
            sent["linode_region"] = "contract-linode_region-2";
            sent["max_connections"] = (Int64)8;
            sent["name"] = "contract-name-2";
            sent["one_drive_account_type"] = "business_other";
            sent["pin_to_site_region"] = false;
            sent["port"] = (Int64)8;
            sent["upload_staging_path"] = "contract-upload_staging_path-2";
            sent["remote_server_credential_id"] = (Int64)8;
            sent["s3_assume_role_arn"] = "contract-s3_assume_role_arn-2";
            sent["s3_assume_role_duration_seconds"] = (Int64)8;
            sent["s3_bucket"] = "contract-s3_bucket-2";
            sent["s3_compatible_access_key"] = "contract-s3_compatible_access_key-2";
            sent["s3_compatible_bucket"] = "contract-s3_compatible_bucket-2";
            sent["s3_compatible_endpoint"] = "contract-s3_compatible_endpoint-2";
            sent["s3_compatible_region"] = "contract-s3_compatible_region-2";
            sent["s3_compatible_virtual_hosted_style"] = false;
            sent["s3_kms_key_id"] = "contract-s3_kms_key_id-2";
            sent["s3_region"] = "contract-s3_region-2";
            sent["server_certificate"] = "allow_any";
            sent["server_host_key"] = "contract-server_host_key-2";
            sent["server_type"] = "sftp";
            sent["sharepoint_client_id"] = "contract-sharepoint_client_id-2";
            sent["sharepoint_site_url"] = "contract-sharepoint_site_url-2";
            sent["sharepoint_tenant_id"] = "contract-sharepoint_tenant_id-2";
            sent["ssl"] = "require";
            sent["username"] = "contract-username-2";
            sent["wasabi_access_key"] = "contract-wasabi_access_key-2";
            sent["wasabi_bucket"] = "contract-wasabi_bucket-2";
            sent["wasabi_region"] = "contract-wasabi_region-2";
            RemoteServer updated = await Simulator.Sdk(operation).RemoteServers.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["allow_relative_paths"] = "boolean", ["aws_access_key"] = "string", ["azure_blob_storage_account"] = "string", ["azure_blob_storage_container"] = "string", ["azure_blob_storage_dns_suffix"] = "string", ["azure_blob_storage_hierarchical_namespace"] = "boolean", ["azure_files_storage_account"] = "string", ["azure_files_storage_dns_suffix"] = "string", ["azure_files_storage_share_name"] = "string", ["backblaze_b2_bucket"] = "string", ["backblaze_b2_s3_endpoint"] = "string", ["buffer_uploads"] = "string", ["cloudflare_access_key"] = "string", ["cloudflare_bucket"] = "string", ["cloudflare_endpoint"] = "string", ["description"] = "string", ["dropbox_teams"] = "boolean", ["enable_dedicated_ips"] = "boolean", ["filebase_access_key"] = "string", ["filebase_bucket"] = "string", ["files_agent_permission_set"] = "string", ["files_agent_root"] = "string", ["files_agent_version"] = "string", ["outbound_agent_id"] = "number", ["custom_domain_id"] = "number", ["google_cloud_storage_authentication_method"] = "string", ["google_cloud_storage_bucket"] = "string", ["google_cloud_storage_oauth_scope"] = "string", ["google_cloud_storage_project_id"] = "string", ["google_cloud_storage_s3_compatible_access_key"] = "string", ["hostname"] = "string", ["linode_access_key"] = "string", ["linode_bucket"] = "string", ["linode_region"] = "string", ["max_connections"] = "number", ["name"] = "string", ["one_drive_account_type"] = "string", ["pin_to_site_region"] = "boolean", ["port"] = "number", ["upload_staging_path"] = "string", ["remote_server_credential_id"] = "number", ["s3_assume_role_arn"] = "string", ["s3_assume_role_duration_seconds"] = "number", ["s3_bucket"] = "string", ["s3_compatible_access_key"] = "string", ["s3_compatible_bucket"] = "string", ["s3_compatible_endpoint"] = "string", ["s3_compatible_region"] = "string", ["s3_compatible_virtual_hosted_style"] = "boolean", ["s3_kms_key_id"] = "string", ["s3_region"] = "string", ["server_certificate"] = "string", ["server_host_key"] = "string", ["server_type"] = "string", ["sharepoint_client_id"] = "string", ["sharepoint_site_url"] = "string", ["sharepoint_tenant_id"] = "string", ["ssl"] = "string", ["username"] = "string", ["wasabi_access_key"] = "string", ["wasabi_bucket"] = "string", ["wasabi_region"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "RemoteServer.update", "allow_relative_paths", "aws_access_key", "azure_blob_storage_account", "azure_blob_storage_container", "azure_blob_storage_dns_suffix", "azure_blob_storage_hierarchical_namespace", "azure_files_storage_account", "azure_files_storage_dns_suffix", "azure_files_storage_share_name", "backblaze_b2_bucket", "backblaze_b2_s3_endpoint", "buffer_uploads", "cloudflare_access_key", "cloudflare_bucket", "cloudflare_endpoint", "description", "dropbox_teams", "enable_dedicated_ips", "filebase_access_key", "filebase_bucket", "files_agent_permission_set", "files_agent_root", "files_agent_version", "outbound_agent_id", "custom_domain_id", "google_cloud_storage_authentication_method", "google_cloud_storage_bucket", "google_cloud_storage_oauth_scope", "google_cloud_storage_project_id", "google_cloud_storage_s3_compatible_access_key", "hostname", "linode_access_key", "linode_bucket", "linode_region", "max_connections", "name", "one_drive_account_type", "pin_to_site_region", "port", "upload_staging_path", "remote_server_credential_id", "s3_assume_role_arn", "s3_assume_role_duration_seconds", "s3_bucket", "s3_compatible_access_key", "s3_compatible_bucket", "s3_compatible_endpoint", "s3_compatible_region", "s3_compatible_virtual_hosted_style", "s3_kms_key_id", "s3_region", "server_certificate", "server_host_key", "server_type", "sharepoint_client_id", "sharepoint_site_url", "sharepoint_tenant_id", "ssl", "username", "wasabi_access_key", "wasabi_bucket", "wasabi_region");
            AssertModel(now, updated, "RemoteServer.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServer", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).RemoteServers.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"RemoteServer.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}