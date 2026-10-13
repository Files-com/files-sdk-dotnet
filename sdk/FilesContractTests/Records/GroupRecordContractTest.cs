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

    // The record contract of Group, generated from the schema: each case sends one request through the
    // SDK's Groups operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class GroupRecordContractTest : ContractCase
    {
        private const string ListPath = "/groups";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Group", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["notes"] = "contract-notes", ["user_ids"] = "contract-user_ids", ["admin_ids"] = "contract-admin_ids", ["ai_assistant_personality_id"] = (Int64)7, ["ftp_permission"] = true, ["sftp_permission"] = true, ["dav_permission"] = true, ["restapi_permission"] = true, ["s3_compatible_endpoint_permission"] = true, ["desktop_configuration_profile_id"] = (Int64)7, ["integration_centric_profile_id"] = (Int64)7, ["allowed_ips"] = "contract-allowed_ips", ["name"] = "contract-name", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Group", ("id", "int64"), ("name", "string"), ("allowed_ips", "string"), ("admin_ids", "string"), ("notes", "string"), ("user_ids", "string"), ("usernames", "string"), ("ai_assistant_personality_id", "int64"), ("ftp_permission", "boolean"), ("sftp_permission", "boolean"), ("dav_permission", "boolean"), ("restapi_permission", "boolean"), ("s3_compatible_endpoint_permission", "boolean"), ("desktop_configuration_profile_id", "int64"), ("integration_centric_profile_id", "int64"), ("site_id", "int64"), ("workspace_id", "int64"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Group the SDK returns gives it.
        private static void AssertModel(JsonObject record, Group model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Group");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "allowed_ips", model.AllowedIps, what);
            Simulator.Same(record, "admin_ids", model.AdminIds, what);
            Simulator.Same(record, "notes", model.Notes, what);
            Simulator.Same(record, "user_ids", model.UserIds, what);
            Simulator.Same(record, "usernames", model.Usernames, what);
            Simulator.Same(record, "ai_assistant_personality_id", model.AiAssistantPersonalityId, what);
            Simulator.Same(record, "ftp_permission", model.FtpPermission, what);
            Simulator.Same(record, "sftp_permission", model.SftpPermission, what);
            Simulator.Same(record, "dav_permission", model.DavPermission, what);
            Simulator.Same(record, "restapi_permission", model.RestapiPermission, what);
            Simulator.Same(record, "s3_compatible_endpoint_permission", model.S3CompatibleEndpointPermission, what);
            Simulator.Same(record, "desktop_configuration_profile_id", model.DesktopConfigurationProfileId, what);
            Simulator.Same(record, "integration_centric_profile_id", model.IntegrationCentricProfileId, what);
            Simulator.Same(record, "site_id", model.SiteId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Group", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["notes"] = "contract-notes", ["user_ids"] = "contract-user_ids", ["admin_ids"] = "contract-admin_ids", ["ai_assistant_personality_id"] = (Int64)7, ["ftp_permission"] = true, ["sftp_permission"] = true, ["dav_permission"] = true, ["restapi_permission"] = true, ["s3_compatible_endpoint_permission"] = true, ["desktop_configuration_profile_id"] = (Int64)7, ["integration_centric_profile_id"] = (Int64)7, ["allowed_ips"] = "contract-allowed_ips", ["name"] = "contract-name", ["workspace_id"] = (Int64)7 };
            Group created = await Simulator.Sdk(operation).Groups.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["notes"] = "string", ["user_ids"] = "string", ["admin_ids"] = "string", ["ai_assistant_personality_id"] = "number", ["ftp_permission"] = "boolean", ["sftp_permission"] = "boolean", ["dav_permission"] = "boolean", ["restapi_permission"] = "boolean", ["s3_compatible_endpoint_permission"] = "boolean", ["desktop_configuration_profile_id"] = "number", ["integration_centric_profile_id"] = "number", ["allowed_ips"] = "string", ["name"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Group.create", "notes", "user_ids", "admin_ids", "ai_assistant_personality_id", "ftp_permission", "sftp_permission", "dav_permission", "restapi_permission", "s3_compatible_endpoint_permission", "desktop_configuration_profile_id", "integration_centric_profile_id", "allowed_ips", "name", "workspace_id");
            AssertModel(record, created, "Group.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Group", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Group[] listed = (await Simulator.Sdk(operation).Groups.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Group.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Group", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Group found = await Simulator.Sdk(operation).Groups.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Group.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Group", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["notes"] = "contract-notes-2";
            sent["user_ids"] = "contract-user_ids-2";
            sent["admin_ids"] = "contract-admin_ids-2";
            sent["ai_assistant_personality_id"] = (Int64)8;
            sent["ftp_permission"] = false;
            sent["sftp_permission"] = false;
            sent["dav_permission"] = false;
            sent["restapi_permission"] = false;
            sent["s3_compatible_endpoint_permission"] = false;
            sent["desktop_configuration_profile_id"] = (Int64)8;
            sent["integration_centric_profile_id"] = (Int64)8;
            sent["allowed_ips"] = "contract-allowed_ips-2";
            sent["name"] = "contract-name-2";
            Group updated = await Simulator.Sdk(operation).Groups.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["notes"] = "string", ["user_ids"] = "string", ["admin_ids"] = "string", ["ai_assistant_personality_id"] = "number", ["ftp_permission"] = "boolean", ["sftp_permission"] = "boolean", ["dav_permission"] = "boolean", ["restapi_permission"] = "boolean", ["s3_compatible_endpoint_permission"] = "boolean", ["desktop_configuration_profile_id"] = "number", ["integration_centric_profile_id"] = "number", ["allowed_ips"] = "string", ["name"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Group.update", "notes", "user_ids", "admin_ids", "ai_assistant_personality_id", "ftp_permission", "sftp_permission", "dav_permission", "restapi_permission", "s3_compatible_endpoint_permission", "desktop_configuration_profile_id", "integration_centric_profile_id", "allowed_ips", "name");
            AssertModel(now, updated, "Group.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Group", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Groups.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Group.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}