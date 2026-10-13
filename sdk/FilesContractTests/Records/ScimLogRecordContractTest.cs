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

    // The record contract of ScimLog, generated from the schema: each case sends one request through the
    // SDK's ScimLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ScimLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/scim_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["created_at"] = "contract-created_at", ["request_path"] = "contract-request_path", ["request_method"] = "contract-request_method", ["http_response_code"] = "contract-http_response_code", ["user_agent"] = "contract-user_agent", ["request_json"] = "contract-request_json", ["response_json"] = "contract-response_json" };
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
                Simulator.Conforms(record, "ScimLog", ("id", "int64"), ("created_at", "string"), ("request_path", "string"), ("request_method", "string"), ("http_response_code", "string"), ("user_agent", "string"), ("request_json", "string"), ("response_json", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the ScimLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, ScimLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ScimLog");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "request_path", model.RequestPath, what);
            Simulator.Same(record, "request_method", model.RequestMethod, what);
            Simulator.Same(record, "http_response_code", model.HttpResponseCode, what);
            Simulator.Same(record, "user_agent", model.UserAgent, what);
            Simulator.Same(record, "request_json", model.RequestJson, what);
            Simulator.Same(record, "response_json", model.ResponseJson, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ScimLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ScimLog[] listed = (await Simulator.Sdk(operation).ScimLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ScimLog.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("ScimLog", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ScimLog found = await Simulator.Sdk(operation).ScimLogs.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "ScimLog.find");
        }
    }
}