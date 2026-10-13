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

    // The record contract of EventDeliveryAttempt, generated from the schema: each case sends one request through the
    // SDK's EventDeliveryAttempts operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class EventDeliveryAttemptRecordContractTest : ContractCase
    {
        private const string ListPath = "/event_delivery_attempts";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["event_record_id"] = 7, ["event_subscription_id"] = 7, ["event_target_id"] = 7, ["activity_notification_batch_id"] = 7, ["workspace_id"] = 7, ["status"] = "queued", ["attempt_number"] = 7, ["response_code"] = 7, ["error_message"] = "contract-error_message", ["response_body"] = "contract-response_body", ["latency_ms"] = 7, ["delivered_at"] = "2030-01-02T03:04:05Z", ["last_attempted_at"] = "2030-01-02T03:04:05Z", ["next_attempt_at"] = "2030-01-02T03:04:05Z", ["created_at"] = "2030-01-02T03:04:05Z" };
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
                Simulator.Conforms(record, "EventDeliveryAttempt", ("id", "int64"), ("event_record_id", "int64"), ("event_subscription_id", "int64"), ("event_target_id", "int64"), ("activity_notification_batch_id", "int64"), ("workspace_id", "int64"), ("status", "string"), ("attempt_number", "int64"), ("response_code", "int64"), ("error_message", "string"), ("response_body", "string"), ("latency_ms", "int64"), ("delivered_at", "date-time"), ("last_attempted_at", "date-time"), ("next_attempt_at", "date-time"), ("created_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the EventDeliveryAttempt the SDK returns gives it.
        private static void AssertModel(JsonObject record, EventDeliveryAttempt model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no EventDeliveryAttempt");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "event_record_id", model.EventRecordId, what);
            Simulator.Same(record, "event_subscription_id", model.EventSubscriptionId, what);
            Simulator.Same(record, "event_target_id", model.EventTargetId, what);
            Simulator.Same(record, "activity_notification_batch_id", model.ActivityNotificationBatchId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "attempt_number", model.AttemptNumber, what);
            Simulator.Same(record, "response_code", model.ResponseCode, what);
            Simulator.Same(record, "error_message", model.ErrorMessage, what);
            Simulator.Same(record, "response_body", model.ResponseBody, what);
            Simulator.Same(record, "latency_ms", model.LatencyMs, what);
            Simulator.Same(record, "delivered_at", model.DeliveredAt, what);
            Simulator.Same(record, "last_attempted_at", model.LastAttemptedAt, what);
            Simulator.Same(record, "next_attempt_at", model.NextAttemptAt, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("EventDeliveryAttempt", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EventDeliveryAttempt[] listed = (await Simulator.Sdk(operation).EventDeliveryAttempts.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"EventDeliveryAttempt.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("EventDeliveryAttempt", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            EventDeliveryAttempt found = await Simulator.Sdk(operation).EventDeliveryAttempts.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "EventDeliveryAttempt.find");
        }
    }
}