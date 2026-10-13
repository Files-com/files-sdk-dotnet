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

    // The record contract of SiemHttpDestinationEvent, generated from the schema: each case sends one request through the
    // SDK's SiemHttpDestinationEvents operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SiemHttpDestinationEventRecordContractTest : ContractCase
    {
        private const string ListPath = "/siem_http_destination_events";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["event_type"] = "contract-event_type", ["status"] = "success", ["body"] = "contract-body", ["event_errors"] = new JsonArray("a"), ["created_at"] = "2030-01-02T03:04:05Z", ["body_url"] = "contract-body_url", ["siem_http_destination_id"] = 7 };
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
                Simulator.Conforms(record, "SiemHttpDestinationEvent", ("id", "int64"), ("event_type", "string"), ("status", "string"), ("body", "string"), ("event_errors", "array(string)"), ("created_at", "date-time"), ("body_url", "string"), ("siem_http_destination_id", "int64"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the SiemHttpDestinationEvent the SDK returns gives it.
        private static void AssertModel(JsonObject record, SiemHttpDestinationEvent model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no SiemHttpDestinationEvent");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "event_type", model.EventType, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "body", model.Body, what);
            Simulator.Same(record, "event_errors", model.EventErrors, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "body_url", model.BodyUrl, what);
            Simulator.Same(record, "siem_http_destination_id", model.SiemHttpDestinationId, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("SiemHttpDestinationEvent", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SiemHttpDestinationEvent[] listed = (await Simulator.Sdk(operation).SiemHttpDestinationEvents.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"SiemHttpDestinationEvent.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("SiemHttpDestinationEvent", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SiemHttpDestinationEvent found = await Simulator.Sdk(operation).SiemHttpDestinationEvents.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "SiemHttpDestinationEvent.find");
        }
    }
}