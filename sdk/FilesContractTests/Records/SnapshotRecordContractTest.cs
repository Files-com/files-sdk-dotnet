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

    // The record contract of Snapshot, generated from the schema: each case sends one request through the
    // SDK's Snapshots operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SnapshotRecordContractTest : ContractCase
    {
        private const string ListPath = "/snapshots";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Snapshot", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["expires_at"] = "2030-01-02T03:04:05Z", ["name"] = "contract-name", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Snapshot", ("id", "int64"), ("expires_at", "date-time"), ("finalized_at", "date-time"), ("name", "string"), ("user_id", "int64"), ("bundle_id", "int64"), ("workspace_id", "int64"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Snapshot the SDK returns gives it.
        private static void AssertModel(JsonObject record, Snapshot model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Snapshot");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "expires_at", model.ExpiresAt, what);
            Simulator.Same(record, "finalized_at", model.FinalizedAt, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "bundle_id", model.BundleId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Snapshot", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["expires_at"] = "2030-01-02T03:04:05Z", ["name"] = "contract-name", ["workspace_id"] = (Int64)7 };
            Snapshot created = await Simulator.Sdk(operation).Snapshots.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["expires_at"] = "string", ["name"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Snapshot.create", "expires_at", "name", "workspace_id");
            AssertModel(record, created, "Snapshot.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Snapshot", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Snapshot[] listed = (await Simulator.Sdk(operation).Snapshots.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Snapshot.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Snapshot", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Snapshot found = await Simulator.Sdk(operation).Snapshots.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Snapshot.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Snapshot", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["expires_at"] = "2031-02-03T04:05:06Z";
            sent["name"] = "contract-name-2";
            Snapshot updated = await Simulator.Sdk(operation).Snapshots.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["expires_at"] = "string", ["name"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Snapshot.update", "expires_at", "name");
            AssertModel(now, updated, "Snapshot.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Snapshot", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Snapshots.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Snapshot.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}