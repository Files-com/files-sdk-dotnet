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

    // The record contract of SyncLog, generated from the schema: each case sends one request through the
    // SDK's SyncLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SyncLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/sync_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["timestamp"] = "2030-01-02T03:04:05Z", ["sync_id"] = 7, ["external_event_id"] = 7, ["sync_run_id"] = 7, ["error_type"] = "contract-error_type", ["message"] = "contract-message", ["operation"] = "delete", ["path"] = "contract-path", ["size"] = 7, ["file_type"] = "contract-file_type", ["status"] = "success", ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "SyncLog", ("timestamp", "date-time"), ("sync_id", "int64"), ("external_event_id", "int64"), ("sync_run_id", "int64"), ("error_type", "string"), ("message", "string"), ("operation", "string"), ("path", "string"), ("size", "int64"), ("file_type", "string"), ("status", "string"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the SyncLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, SyncLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no SyncLog");
            Simulator.Same(record, "timestamp", model.Timestamp, what);
            Simulator.Same(record, "sync_id", model.SyncId, what);
            Simulator.Same(record, "external_event_id", model.ExternalEventId, what);
            Simulator.Same(record, "sync_run_id", model.SyncRunId, what);
            Simulator.Same(record, "error_type", model.ErrorType, what);
            Simulator.Same(record, "message", model.Message, what);
            Simulator.Same(record, "operation", model.Operation, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "size", model.Size, what);
            Simulator.Same(record, "file_type", model.FileType, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("SyncLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SyncLog[] listed = (await Simulator.Sdk(operation).SyncLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"SyncLog.list[{i}]");
            }
        }
    }
}