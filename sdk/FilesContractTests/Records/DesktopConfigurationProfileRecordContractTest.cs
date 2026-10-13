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

    // The record contract of DesktopConfigurationProfile, generated from the schema: each case sends one request through the
    // SDK's DesktopConfigurationProfiles operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class DesktopConfigurationProfileRecordContractTest : ContractCase
    {
        private const string ListPath = "/desktop_configuration_profiles";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("DesktopConfigurationProfile", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["mount_mappings"] = new Dictionary<string, object> { ["k"] = "v" }, ["workspace_id"] = (Int64)7, ["use_for_all_users"] = true, ["disable_drive_mounting"] = true })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "DesktopConfigurationProfile", ("id", "int64"), ("name", "string"), ("workspace_id", "int64"), ("use_for_all_users", "boolean"), ("disable_drive_mounting", "boolean"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the DesktopConfigurationProfile the SDK returns gives it.
        private static void AssertModel(JsonObject record, DesktopConfigurationProfile model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no DesktopConfigurationProfile");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "use_for_all_users", model.UseForAllUsers, what);
            Simulator.Same(record, "disable_drive_mounting", model.DisableDriveMounting, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("DesktopConfigurationProfile", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["mount_mappings"] = new Dictionary<string, object> { ["k"] = "v" }, ["workspace_id"] = (Int64)7, ["use_for_all_users"] = true, ["disable_drive_mounting"] = true };
            DesktopConfigurationProfile created = await Simulator.Sdk(operation).DesktopConfigurationProfiles.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["mount_mappings"] = "object", ["workspace_id"] = "number", ["use_for_all_users"] = "boolean", ["disable_drive_mounting"] = "boolean" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "DesktopConfigurationProfile.create", "name", "mount_mappings", "workspace_id", "use_for_all_users", "disable_drive_mounting");
            AssertModel(record, created, "DesktopConfigurationProfile.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("DesktopConfigurationProfile", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            DesktopConfigurationProfile[] listed = (await Simulator.Sdk(operation).DesktopConfigurationProfiles.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"DesktopConfigurationProfile.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("DesktopConfigurationProfile", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            DesktopConfigurationProfile found = await Simulator.Sdk(operation).DesktopConfigurationProfiles.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "DesktopConfigurationProfile.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("DesktopConfigurationProfile", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["workspace_id"] = (Int64)8;
            sent["use_for_all_users"] = false;
            sent["disable_drive_mounting"] = false;
            DesktopConfigurationProfile updated = await Simulator.Sdk(operation).DesktopConfigurationProfiles.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["workspace_id"] = "number", ["use_for_all_users"] = "boolean", ["disable_drive_mounting"] = "boolean" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "DesktopConfigurationProfile.update", "name", "workspace_id", "use_for_all_users", "disable_drive_mounting");
            AssertModel(now, updated, "DesktopConfigurationProfile.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("DesktopConfigurationProfile", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).DesktopConfigurationProfiles.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"DesktopConfigurationProfile.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}