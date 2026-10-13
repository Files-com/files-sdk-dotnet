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

    // The record contract of UsageSnapshot, generated from the schema: each case sends one request through the
    // SDK's UsageSnapshots operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class UsageSnapshotRecordContractTest : ContractCase
    {
        private const string ListPath = "/usage_snapshots";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["start_at"] = "2030-01-02T03:04:05Z", ["end_at"] = "2030-01-02T03:04:05Z", ["high_water_user_count"] = 7, ["transformation_credits_usage"] = 7, ["current_storage"] = 7, ["high_water_storage"] = 7, ["root_storage"] = 7, ["deleted_files_counted_in_minimum"] = 7, ["deleted_files_storage"] = 7, ["total_billable_usage"] = 7, ["total_billable_transfer_usage"] = 7, ["bytes_sent"] = 7, ["sync_bytes_received"] = 7, ["sync_bytes_sent"] = 7 };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "UsageSnapshot", ("id", "int64"), ("start_at", "date-time"), ("end_at", "date-time"), ("high_water_user_count", "int64"), ("transformation_credits_usage", "int64"), ("current_storage", "int64"), ("high_water_storage", "int64"), ("root_storage", "int64"), ("deleted_files_counted_in_minimum", "int64"), ("deleted_files_storage", "int64"), ("total_billable_usage", "int64"), ("total_billable_transfer_usage", "int64"), ("bytes_sent", "int64"), ("sync_bytes_received", "int64"), ("sync_bytes_sent", "int64"));
            }
            return records;
        }

        // Every property the record holds, as the UsageSnapshot the SDK returns gives it.
        private static void AssertModel(JsonObject record, UsageSnapshot model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no UsageSnapshot");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "start_at", model.StartAt, what);
            Simulator.Same(record, "end_at", model.EndAt, what);
            Simulator.Same(record, "high_water_user_count", model.HighWaterUserCount, what);
            Simulator.Same(record, "transformation_credits_usage", model.TransformationCreditsUsage, what);
            Simulator.Same(record, "current_storage", model.CurrentStorage, what);
            Simulator.Same(record, "high_water_storage", model.HighWaterStorage, what);
            Simulator.Same(record, "root_storage", model.RootStorage, what);
            Simulator.Same(record, "deleted_files_counted_in_minimum", model.DeletedFilesCountedInMinimum, what);
            Simulator.Same(record, "deleted_files_storage", model.DeletedFilesStorage, what);
            Simulator.Same(record, "total_billable_usage", model.TotalBillableUsage, what);
            Simulator.Same(record, "total_billable_transfer_usage", model.TotalBillableTransferUsage, what);
            Simulator.Same(record, "bytes_sent", model.BytesSent, what);
            Simulator.Same(record, "sync_bytes_received", model.SyncBytesReceived, what);
            Simulator.Same(record, "sync_bytes_sent", model.SyncBytesSent, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("UsageSnapshot", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UsageSnapshot[] listed = (await Simulator.Sdk(operation).UsageSnapshots.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"UsageSnapshot.list[{i}]");
            }
        }
    }
}