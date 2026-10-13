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

    // The record contract of RemoteServerCredential, generated from the schema: each case sends one request through the
    // SDK's RemoteServerCredentials operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class RemoteServerCredentialRecordContractTest : ContractCase
    {
        private const string ListPath = "/remote_server_credentials";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("RemoteServerCredential", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["description"] = "contract-description", ["server_type"] = "ftp", ["aws_access_key"] = "contract-aws_access_key", ["s3_assume_role_arn"] = "contract-s3_assume_role_arn", ["s3_assume_role_duration_seconds"] = (Int64)7, ["cloudflare_access_key"] = "contract-cloudflare_access_key", ["filebase_access_key"] = "contract-filebase_access_key", ["google_cloud_storage_s3_compatible_access_key"] = "contract-google_cloud_storage_s3_compatible_access_key", ["linode_access_key"] = "contract-linode_access_key", ["s3_compatible_access_key"] = "contract-s3_compatible_access_key", ["sharepoint_client_id"] = "contract-sharepoint_client_id", ["sharepoint_tenant_id"] = "contract-sharepoint_tenant_id", ["username"] = "contract-username", ["wasabi_access_key"] = "contract-wasabi_access_key", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "RemoteServerCredential", ("id", "int64"), ("workspace_id", "int64"), ("name", "string"), ("description", "string"), ("server_type", "string"), ("aws_access_key", "string"), ("s3_assume_role_arn", "string"), ("s3_assume_role_duration_seconds", "int64"), ("s3_assume_role_external_id", "string"), ("google_cloud_storage_s3_compatible_access_key", "string"), ("wasabi_access_key", "string"), ("s3_compatible_access_key", "string"), ("filebase_access_key", "string"), ("cloudflare_access_key", "string"), ("linode_access_key", "string"), ("sharepoint_tenant_id", "string"), ("sharepoint_client_id", "string"), ("sharepoint_app_credential_type", "string"), ("username", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the RemoteServerCredential the SDK returns gives it.
        private static void AssertModel(JsonObject record, RemoteServerCredential model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no RemoteServerCredential");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "server_type", model.ServerType, what);
            Simulator.Same(record, "aws_access_key", model.AwsAccessKey, what);
            Simulator.Same(record, "s3_assume_role_arn", model.S3AssumeRoleArn, what);
            Simulator.Same(record, "s3_assume_role_duration_seconds", model.S3AssumeRoleDurationSeconds, what);
            Simulator.Same(record, "s3_assume_role_external_id", model.S3AssumeRoleExternalId, what);
            Simulator.Same(record, "google_cloud_storage_s3_compatible_access_key", model.GoogleCloudStorageS3CompatibleAccessKey, what);
            Simulator.Same(record, "wasabi_access_key", model.WasabiAccessKey, what);
            Simulator.Same(record, "s3_compatible_access_key", model.S3CompatibleAccessKey, what);
            Simulator.Same(record, "filebase_access_key", model.FilebaseAccessKey, what);
            Simulator.Same(record, "cloudflare_access_key", model.CloudflareAccessKey, what);
            Simulator.Same(record, "linode_access_key", model.LinodeAccessKey, what);
            Simulator.Same(record, "sharepoint_tenant_id", model.SharepointTenantId, what);
            Simulator.Same(record, "sharepoint_client_id", model.SharepointClientId, what);
            Simulator.Same(record, "sharepoint_app_credential_type", model.SharepointAppCredentialType, what);
            Simulator.Same(record, "username", model.Username, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServerCredential", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["description"] = "contract-description", ["server_type"] = "ftp", ["aws_access_key"] = "contract-aws_access_key", ["s3_assume_role_arn"] = "contract-s3_assume_role_arn", ["s3_assume_role_duration_seconds"] = (Int64)7, ["cloudflare_access_key"] = "contract-cloudflare_access_key", ["filebase_access_key"] = "contract-filebase_access_key", ["google_cloud_storage_s3_compatible_access_key"] = "contract-google_cloud_storage_s3_compatible_access_key", ["linode_access_key"] = "contract-linode_access_key", ["s3_compatible_access_key"] = "contract-s3_compatible_access_key", ["sharepoint_client_id"] = "contract-sharepoint_client_id", ["sharepoint_tenant_id"] = "contract-sharepoint_tenant_id", ["username"] = "contract-username", ["wasabi_access_key"] = "contract-wasabi_access_key", ["workspace_id"] = (Int64)7 };
            RemoteServerCredential created = await Simulator.Sdk(operation).RemoteServerCredentials.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["description"] = "string", ["server_type"] = "string", ["aws_access_key"] = "string", ["s3_assume_role_arn"] = "string", ["s3_assume_role_duration_seconds"] = "number", ["cloudflare_access_key"] = "string", ["filebase_access_key"] = "string", ["google_cloud_storage_s3_compatible_access_key"] = "string", ["linode_access_key"] = "string", ["s3_compatible_access_key"] = "string", ["sharepoint_client_id"] = "string", ["sharepoint_tenant_id"] = "string", ["username"] = "string", ["wasabi_access_key"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "RemoteServerCredential.create", "name", "description", "server_type", "aws_access_key", "s3_assume_role_arn", "s3_assume_role_duration_seconds", "cloudflare_access_key", "filebase_access_key", "google_cloud_storage_s3_compatible_access_key", "linode_access_key", "s3_compatible_access_key", "sharepoint_client_id", "sharepoint_tenant_id", "username", "wasabi_access_key", "workspace_id");
            AssertModel(record, created, "RemoteServerCredential.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServerCredential", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            RemoteServerCredential[] listed = (await Simulator.Sdk(operation).RemoteServerCredentials.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"RemoteServerCredential.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServerCredential", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            RemoteServerCredential found = await Simulator.Sdk(operation).RemoteServerCredentials.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "RemoteServerCredential.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServerCredential", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["description"] = "contract-description-2";
            sent["server_type"] = "sftp";
            sent["aws_access_key"] = "contract-aws_access_key-2";
            sent["s3_assume_role_arn"] = "contract-s3_assume_role_arn-2";
            sent["s3_assume_role_duration_seconds"] = (Int64)8;
            sent["cloudflare_access_key"] = "contract-cloudflare_access_key-2";
            sent["filebase_access_key"] = "contract-filebase_access_key-2";
            sent["google_cloud_storage_s3_compatible_access_key"] = "contract-google_cloud_storage_s3_compatible_access_key-2";
            sent["linode_access_key"] = "contract-linode_access_key-2";
            sent["s3_compatible_access_key"] = "contract-s3_compatible_access_key-2";
            sent["sharepoint_client_id"] = "contract-sharepoint_client_id-2";
            sent["sharepoint_tenant_id"] = "contract-sharepoint_tenant_id-2";
            sent["username"] = "contract-username-2";
            sent["wasabi_access_key"] = "contract-wasabi_access_key-2";
            RemoteServerCredential updated = await Simulator.Sdk(operation).RemoteServerCredentials.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["description"] = "string", ["server_type"] = "string", ["aws_access_key"] = "string", ["s3_assume_role_arn"] = "string", ["s3_assume_role_duration_seconds"] = "number", ["cloudflare_access_key"] = "string", ["filebase_access_key"] = "string", ["google_cloud_storage_s3_compatible_access_key"] = "string", ["linode_access_key"] = "string", ["s3_compatible_access_key"] = "string", ["sharepoint_client_id"] = "string", ["sharepoint_tenant_id"] = "string", ["username"] = "string", ["wasabi_access_key"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "RemoteServerCredential.update", "name", "description", "server_type", "aws_access_key", "s3_assume_role_arn", "s3_assume_role_duration_seconds", "cloudflare_access_key", "filebase_access_key", "google_cloud_storage_s3_compatible_access_key", "linode_access_key", "s3_compatible_access_key", "sharepoint_client_id", "sharepoint_tenant_id", "username", "wasabi_access_key");
            AssertModel(now, updated, "RemoteServerCredential.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteServerCredential", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).RemoteServerCredentials.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"RemoteServerCredential.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}