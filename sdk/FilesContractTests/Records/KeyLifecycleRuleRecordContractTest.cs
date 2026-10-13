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

    // The record contract of KeyLifecycleRule, generated from the schema: each case sends one request through the
    // SDK's KeyLifecycleRules operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class KeyLifecycleRuleRecordContractTest : ContractCase
    {
        private const string ListPath = "/key_lifecycle_rules";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("KeyLifecycleRule", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["apply_to_all_workspaces"] = true, ["expiration_days"] = (Int64)7, ["key_type"] = "gpg", ["inactivity_days"] = (Int64)7, ["name"] = "contract-name", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "KeyLifecycleRule", ("id", "int64"), ("key_type", "string"), ("inactivity_days", "int64"), ("expiration_days", "int64"), ("apply_to_all_workspaces", "boolean"), ("name", "string"), ("workspace_id", "int64"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the KeyLifecycleRule the SDK returns gives it.
        private static void AssertModel(JsonObject record, KeyLifecycleRule model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no KeyLifecycleRule");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "key_type", model.KeyType, what);
            Simulator.Same(record, "inactivity_days", model.InactivityDays, what);
            Simulator.Same(record, "expiration_days", model.ExpirationDays, what);
            Simulator.Same(record, "apply_to_all_workspaces", model.ApplyToAllWorkspaces, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("KeyLifecycleRule", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["apply_to_all_workspaces"] = true, ["expiration_days"] = (Int64)7, ["key_type"] = "gpg", ["inactivity_days"] = (Int64)7, ["name"] = "contract-name", ["workspace_id"] = (Int64)7 };
            KeyLifecycleRule created = await Simulator.Sdk(operation).KeyLifecycleRules.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["apply_to_all_workspaces"] = "boolean", ["expiration_days"] = "number", ["key_type"] = "string", ["inactivity_days"] = "number", ["name"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "KeyLifecycleRule.create", "apply_to_all_workspaces", "expiration_days", "key_type", "inactivity_days", "name", "workspace_id");
            AssertModel(record, created, "KeyLifecycleRule.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("KeyLifecycleRule", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            KeyLifecycleRule[] listed = (await Simulator.Sdk(operation).KeyLifecycleRules.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"KeyLifecycleRule.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("KeyLifecycleRule", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            KeyLifecycleRule found = await Simulator.Sdk(operation).KeyLifecycleRules.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "KeyLifecycleRule.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("KeyLifecycleRule", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["apply_to_all_workspaces"] = false;
            sent["expiration_days"] = (Int64)8;
            sent["key_type"] = "ssh";
            sent["inactivity_days"] = (Int64)8;
            sent["name"] = "contract-name-2";
            sent["workspace_id"] = (Int64)8;
            KeyLifecycleRule updated = await Simulator.Sdk(operation).KeyLifecycleRules.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["apply_to_all_workspaces"] = "boolean", ["expiration_days"] = "number", ["key_type"] = "string", ["inactivity_days"] = "number", ["name"] = "string", ["workspace_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "KeyLifecycleRule.update", "apply_to_all_workspaces", "expiration_days", "key_type", "inactivity_days", "name", "workspace_id");
            AssertModel(now, updated, "KeyLifecycleRule.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("KeyLifecycleRule", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).KeyLifecycleRules.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"KeyLifecycleRule.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}