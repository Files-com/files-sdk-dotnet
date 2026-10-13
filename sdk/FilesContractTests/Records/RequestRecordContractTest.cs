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

    // The record contract of Request, generated from the schema: each case sends one request through the
    // SDK's Requests operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class RequestRecordContractTest : ContractCase
    {
        private const string ListPath = "/requests";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Request", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["path"] = "contract-path", ["destination"] = "contract-destination" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Request", ("id", "int64"), ("path", "string"), ("source", "string"), ("destination", "string"), ("automation_id", "int64"), ("user_display_name", "string"));
            }
            return records;
        }

        // Every property the record holds, as the Request the SDK returns gives it.
        private static void AssertModel(JsonObject record, Request model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Request");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "source", model.Source, what);
            Simulator.Same(record, "destination", model.Destination, what);
            Simulator.Same(record, "automation_id", model.AutomationId, what);
            Simulator.Same(record, "user_display_name", model.UserDisplayName, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Request", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["path"] = "contract-path", ["destination"] = "contract-destination" };
            Request created = await Simulator.Sdk(operation).Requests.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["path"] = "string", ["destination"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, null, Scope(), candidate => true);
            await Simulator.KeptAsync(sent, record, "Request.create", "path", "destination");
            AssertModel(record, created, "Request.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Request", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Request[] listed = (await Simulator.Sdk(operation).Requests.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Request.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Request", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Requests.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            Assert.AreEqual(0, (await Simulator.ListRawAsync(ListPath, Scope())).Length, "Request.delete: the record is gone");
        }
    }
}