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

    // The record contract of EventSubscription, generated from the schema: each case sends one request through the
    // SDK's EventSubscriptions operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class EventSubscriptionRecordContractTest : ContractCase
    {
        private const string ListPath = "/event_subscriptions";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("EventSubscription", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["event_channel_id"] = (Int64)7, ["workspace_id"] = (Int64)7, ["apply_to_all_workspaces"] = true, ["name"] = "contract-name", ["subject"] = "contract-subject", ["message"] = "contract-message", ["message_only"] = true, ["enabled"] = true, ["event_types"] = new string[] { "a" }, ["event_target_ids"] = new Nullable<Int64>[] { 3 } })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "EventSubscription", ("id", "int64"), ("event_channel_id", "int64"), ("workspace_id", "int64"), ("apply_to_all_workspaces", "boolean"), ("name", "string"), ("subject", "string"), ("message", "string"), ("message_only", "boolean"), ("enabled", "boolean"), ("event_types", "array(string)"), ("event_target_ids", "array(int64)"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the EventSubscription the SDK returns gives it.
        private static void AssertModel(JsonObject record, EventSubscription model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no EventSubscription");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "event_channel_id", model.EventChannelId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "apply_to_all_workspaces", model.ApplyToAllWorkspaces, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "subject", model.Subject, what);
            Simulator.Same(record, "message", model.Message, what);
            Simulator.Same(record, "message_only", model.MessageOnly, what);
            Simulator.Same(record, "enabled", model.Enabled, what);
            Simulator.Same(record, "event_types", model.EventTypes, what);
            Simulator.Same(record, "event_target_ids", model.EventTargetIds, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("EventSubscription", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["event_channel_id"] = (Int64)7, ["workspace_id"] = (Int64)7, ["apply_to_all_workspaces"] = true, ["name"] = "contract-name", ["subject"] = "contract-subject", ["message"] = "contract-message", ["message_only"] = true, ["enabled"] = true, ["event_types"] = new string[] { "a" }, ["event_target_ids"] = new Nullable<Int64>[] { 3 } };
            EventSubscription created = await Simulator.Sdk(operation).EventSubscriptions.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["event_channel_id"] = "number", ["workspace_id"] = "number", ["apply_to_all_workspaces"] = "boolean", ["name"] = "string", ["subject"] = "string", ["message"] = "string", ["message_only"] = "boolean", ["enabled"] = "boolean", ["event_types"] = "array", ["event_target_ids"] = "array" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "EventSubscription.create", "event_channel_id", "workspace_id", "apply_to_all_workspaces", "name", "subject", "message", "message_only", "enabled", "event_types", "event_target_ids");
            AssertModel(record, created, "EventSubscription.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("EventSubscription", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EventSubscription[] listed = (await Simulator.Sdk(operation).EventSubscriptions.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"EventSubscription.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("EventSubscription", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EventSubscription found = await Simulator.Sdk(operation).EventSubscriptions.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "EventSubscription.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("EventSubscription", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["event_channel_id"] = (Int64)8;
            sent["workspace_id"] = (Int64)8;
            sent["apply_to_all_workspaces"] = false;
            sent["name"] = "contract-name-2";
            sent["subject"] = "contract-subject-2";
            sent["message"] = "contract-message-2";
            sent["message_only"] = false;
            sent["enabled"] = false;
            sent["event_types"] = new string[] { "b" };
            sent["event_target_ids"] = new Nullable<Int64>[] { 4 };
            EventSubscription updated = await Simulator.Sdk(operation).EventSubscriptions.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["event_channel_id"] = "number", ["workspace_id"] = "number", ["apply_to_all_workspaces"] = "boolean", ["name"] = "string", ["subject"] = "string", ["message"] = "string", ["message_only"] = "boolean", ["enabled"] = "boolean", ["event_types"] = "array", ["event_target_ids"] = "array" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "EventSubscription.update", "event_channel_id", "workspace_id", "apply_to_all_workspaces", "name", "subject", "message", "message_only", "enabled", "event_types", "event_target_ids");
            AssertModel(now, updated, "EventSubscription.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("EventSubscription", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).EventSubscriptions.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"EventSubscription.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}