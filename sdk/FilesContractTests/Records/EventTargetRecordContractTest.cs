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

    // The record contract of EventTarget, generated from the schema: each case sends one request through the
    // SDK's EventTargets operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class EventTargetRecordContractTest : ContractCase
    {
        private const string ListPath = "/event_targets";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("EventTarget", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["workspace_id"] = (Int64)7, ["apply_to_all_workspaces"] = true, ["enabled"] = true, ["config"] = new Dictionary<string, object> { ["k"] = "v" }, ["target_type"] = "email" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "EventTarget", ("id", "int64"), ("name", "string"), ("target_type", "string"), ("workspace_id", "int64"), ("apply_to_all_workspaces", "boolean"), ("enabled", "boolean"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the EventTarget the SDK returns gives it.
        private static void AssertModel(JsonObject record, EventTarget model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no EventTarget");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "target_type", model.TargetType, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "apply_to_all_workspaces", model.ApplyToAllWorkspaces, what);
            Simulator.Same(record, "enabled", model.Enabled, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("EventTarget", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["workspace_id"] = (Int64)7, ["apply_to_all_workspaces"] = true, ["enabled"] = true, ["config"] = new Dictionary<string, object> { ["k"] = "v" }, ["target_type"] = "email" };
            EventTarget created = await Simulator.Sdk(operation).EventTargets.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["workspace_id"] = "number", ["apply_to_all_workspaces"] = "boolean", ["enabled"] = "boolean", ["config"] = "object", ["target_type"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "EventTarget.create", "name", "workspace_id", "apply_to_all_workspaces", "enabled", "config", "target_type");
            AssertModel(record, created, "EventTarget.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("EventTarget", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EventTarget[] listed = (await Simulator.Sdk(operation).EventTargets.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"EventTarget.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("EventTarget", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EventTarget found = await Simulator.Sdk(operation).EventTargets.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "EventTarget.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("EventTarget", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["workspace_id"] = (Int64)8;
            sent["apply_to_all_workspaces"] = false;
            sent["enabled"] = false;
            EventTarget updated = await Simulator.Sdk(operation).EventTargets.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["workspace_id"] = "number", ["apply_to_all_workspaces"] = "boolean", ["enabled"] = "boolean" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "EventTarget.update", "name", "workspace_id", "apply_to_all_workspaces", "enabled");
            AssertModel(now, updated, "EventTarget.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("EventTarget", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).EventTargets.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"EventTarget.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}