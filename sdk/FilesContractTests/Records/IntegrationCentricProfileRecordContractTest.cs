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

    // The record contract of IntegrationCentricProfile, generated from the schema: each case sends one request through the
    // SDK's IntegrationCentricProfiles operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class IntegrationCentricProfileRecordContractTest : ContractCase
    {
        private const string ListPath = "/integration_centric_profiles";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("IntegrationCentricProfile", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["expected_remote_servers"] = new object[] { new Dictionary<string, object> { ["k"] = "v" } }, ["workspace_id"] = (Int64)7, ["use_for_all_users"] = true })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "IntegrationCentricProfile", ("id", "int64"), ("name", "string"), ("workspace_id", "int64"), ("use_for_all_users", "boolean"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the IntegrationCentricProfile the SDK returns gives it.
        private static void AssertModel(JsonObject record, IntegrationCentricProfile model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no IntegrationCentricProfile");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "use_for_all_users", model.UseForAllUsers, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("IntegrationCentricProfile", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["expected_remote_servers"] = new object[] { new Dictionary<string, object> { ["k"] = "v" } }, ["workspace_id"] = (Int64)7, ["use_for_all_users"] = true };
            IntegrationCentricProfile created = await Simulator.Sdk(operation).IntegrationCentricProfiles.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["expected_remote_servers"] = "array", ["workspace_id"] = "number", ["use_for_all_users"] = "boolean" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "IntegrationCentricProfile.create", "name", "expected_remote_servers", "workspace_id", "use_for_all_users");
            AssertModel(record, created, "IntegrationCentricProfile.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("IntegrationCentricProfile", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            IntegrationCentricProfile[] listed = (await Simulator.Sdk(operation).IntegrationCentricProfiles.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"IntegrationCentricProfile.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("IntegrationCentricProfile", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            IntegrationCentricProfile found = await Simulator.Sdk(operation).IntegrationCentricProfiles.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "IntegrationCentricProfile.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("IntegrationCentricProfile", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["workspace_id"] = (Int64)8;
            sent["use_for_all_users"] = false;
            IntegrationCentricProfile updated = await Simulator.Sdk(operation).IntegrationCentricProfiles.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["workspace_id"] = "number", ["use_for_all_users"] = "boolean" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "IntegrationCentricProfile.update", "name", "workspace_id", "use_for_all_users");
            AssertModel(now, updated, "IntegrationCentricProfile.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("IntegrationCentricProfile", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).IntegrationCentricProfiles.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"IntegrationCentricProfile.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}