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

    // The record contract of ChatSession, generated from the schema: each case sends one request through the
    // SDK's ChatSessions operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ChatSessionRecordContractTest : ContractCase
    {
        private const string ListPath = "/chat_sessions";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            // Each record holds its own string id, which only a fixture supplies.
            JsonObject[] records = Enumerable.Range(1, count).Select(n =>
            {
                JsonObject record = new JsonObject { ["title"] = "contract-title", ["user_id"] = 7, ["ai_task_id"] = 7, ["workspace_id"] = 7, ["last_active_at"] = "2030-01-02T03:04:05Z", ["created_at"] = "2030-01-02T03:04:05Z" };
                record["id"] = $"contract-id-{n}";
                return record;
            }).ToArray();
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), records);
            // A member path names the fixture record holding that string id.
            List<JsonObject> held = new List<JsonObject>();
            for (int id = 1; id <= count; id++)
            {
                held.Add(await Simulator.ReadRawAsync(ListPath, $"{ListPath}/contract-id-{id}", null, null));
            }
            return Conform(held.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ChatSession", ("id", "string"), ("title", "string"), ("user_id", "int64"), ("ai_task_id", "int64"), ("workspace_id", "int64"), ("last_active_at", "date-time"), ("created_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{Uri.EscapeDataString((string)record["id"])}";
        }

        // Every property the record holds, as the ChatSession the SDK returns gives it.
        private static void AssertModel(JsonObject record, ChatSession model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ChatSession");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "title", model.Title, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "ai_task_id", model.AiTaskId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "last_active_at", model.LastActiveAt, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ChatSession", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ChatSession[] listed = (await Simulator.Sdk(operation).ChatSessions.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ChatSession.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("ChatSession", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ChatSession found = await Simulator.Sdk(operation).ChatSessions.FindAsync((string)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "ChatSession.find");
        }
    }
}