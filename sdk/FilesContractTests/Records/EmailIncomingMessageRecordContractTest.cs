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

    // The record contract of EmailIncomingMessage, generated from the schema: each case sends one request through the
    // SDK's EmailIncomingMessages operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class EmailIncomingMessageRecordContractTest : ContractCase
    {
        private const string ListPath = "/email_incoming_messages";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["inbox_id"] = 7, ["sender"] = "contract-sender", ["sender_name"] = "contract-sender_name", ["status"] = "success", ["body"] = "contract-body", ["message"] = "contract-message", ["created_at"] = "2030-01-02T03:04:05Z", ["inbox_title"] = "contract-inbox_title" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "EmailIncomingMessage", ("id", "int64"), ("inbox_id", "int64"), ("sender", "string"), ("sender_name", "string"), ("status", "string"), ("body", "string"), ("message", "string"), ("created_at", "date-time"), ("inbox_title", "string"));
            }
            return records;
        }

        // Every property the record holds, as the EmailIncomingMessage the SDK returns gives it.
        private static void AssertModel(JsonObject record, EmailIncomingMessage model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no EmailIncomingMessage");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "inbox_id", model.InboxId, what);
            Simulator.Same(record, "sender", model.Sender, what);
            Simulator.Same(record, "sender_name", model.SenderName, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "body", model.Body, what);
            Simulator.Same(record, "message", model.Message, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "inbox_title", model.InboxTitle, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("EmailIncomingMessage", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EmailIncomingMessage[] listed = (await Simulator.Sdk(operation).EmailIncomingMessages.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"EmailIncomingMessage.list[{i}]");
            }
        }
    }
}