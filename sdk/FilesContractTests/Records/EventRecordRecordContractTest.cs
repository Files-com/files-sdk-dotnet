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

    // The record contract of EventRecord, generated from the schema: each case sends one request through the
    // SDK's EventRecords operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class EventRecordRecordContractTest : ContractCase
    {
        private const string ListPath = "/event_records";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["workspace_id"] = 7, ["event_uuid"] = "contract-event_uuid", ["event_type"] = "contract-event_type", ["severity"] = "info", ["source_type"] = "contract-source_type", ["source_id"] = 7, ["occurred_at"] = "2030-01-02T03:04:05Z", ["human_title"] = "contract-human_title", ["human_summary"] = "contract-human_summary", ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            // Fixture records get IDs from 1 in the order given.
            List<JsonObject> held = new List<JsonObject>();
            for (int id = 1; id <= count; id++)
            {
                held.Add(await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{id}", null, null));
            }
            return Conform(held.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "EventRecord", ("id", "int64"), ("workspace_id", "int64"), ("event_uuid", "string"), ("event_type", "string"), ("severity", "string"), ("source_type", "string"), ("source_id", "int64"), ("occurred_at", "date-time"), ("human_title", "string"), ("human_summary", "string"), ("created_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the EventRecord the SDK returns gives it.
        private static void AssertModel(JsonObject record, EventRecord model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no EventRecord");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "event_uuid", model.EventUuid, what);
            Simulator.Same(record, "event_type", model.EventType, what);
            Simulator.Same(record, "severity", model.Severity, what);
            Simulator.Same(record, "source_type", model.SourceType, what);
            Simulator.Same(record, "source_id", model.SourceId, what);
            Simulator.Same(record, "occurred_at", model.OccurredAt, what);
            Simulator.Same(record, "human_title", model.HumanTitle, what);
            Simulator.Same(record, "human_summary", model.HumanSummary, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("EventRecord", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EventRecord[] listed = (await Simulator.Sdk(operation).EventRecords.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"EventRecord.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("EventRecord", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EventRecord found = await Simulator.Sdk(operation).EventRecords.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "EventRecord.find");
        }
    }
}