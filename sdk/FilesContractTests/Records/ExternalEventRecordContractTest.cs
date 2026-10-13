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

    // The record contract of ExternalEvent, generated from the schema: each case sends one request through the
    // SDK's ExternalEvents operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ExternalEventRecordContractTest : ContractCase
    {
        private const string ListPath = "/external_events";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("ExternalEvent", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["status"] = "success", ["body"] = "contract-body" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ExternalEvent", ("id", "int64"), ("event_type", "string"), ("status", "string"), ("body", "string"), ("created_at", "date-time"), ("body_url", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the ExternalEvent the SDK returns gives it.
        private static void AssertModel(JsonObject record, ExternalEvent model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ExternalEvent");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "event_type", model.EventType, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "body", model.Body, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "body_url", model.BodyUrl, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("ExternalEvent", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["status"] = "success", ["body"] = "contract-body" };
            ExternalEvent created = await Simulator.Sdk(operation).ExternalEvents.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["status"] = "string", ["body"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "ExternalEvent.create", "status", "body");
            AssertModel(record, created, "ExternalEvent.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ExternalEvent", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ExternalEvent[] listed = (await Simulator.Sdk(operation).ExternalEvents.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ExternalEvent.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("ExternalEvent", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ExternalEvent found = await Simulator.Sdk(operation).ExternalEvents.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "ExternalEvent.find");
        }
    }
}