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

    // The record contract of UserSecurityEvent, generated from the schema: each case sends one request through the
    // SDK's UserSecurityEvents operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class UserSecurityEventRecordContractTest : ContractCase
    {
        private const string ListPath = "/user_security_events";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["event_type"] = "lockout", ["body"] = "contract-body", ["event_errors"] = new JsonArray("a"), ["created_at"] = "2030-01-02T03:04:05Z", ["body_url"] = "contract-body_url", ["user_id"] = 7 };
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
                Simulator.Conforms(record, "UserSecurityEvent", ("id", "int64"), ("event_type", "string"), ("body", "string"), ("event_errors", "array(string)"), ("created_at", "date-time"), ("body_url", "string"), ("user_id", "int64"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the UserSecurityEvent the SDK returns gives it.
        private static void AssertModel(JsonObject record, UserSecurityEvent model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no UserSecurityEvent");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "event_type", model.EventType, what);
            Simulator.Same(record, "body", model.Body, what);
            Simulator.Same(record, "event_errors", model.EventErrors, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "body_url", model.BodyUrl, what);
            Simulator.Same(record, "user_id", model.UserId, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("UserSecurityEvent", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UserSecurityEvent[] listed = (await Simulator.Sdk(operation).UserSecurityEvents.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"UserSecurityEvent.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("UserSecurityEvent", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UserSecurityEvent found = await Simulator.Sdk(operation).UserSecurityEvents.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "UserSecurityEvent.find");
        }
    }
}