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

    // The record contract of BandwidthSnapshot, generated from the schema: each case sends one request through the
    // SDK's BandwidthSnapshots operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class BandwidthSnapshotRecordContractTest : ContractCase
    {
        private const string ListPath = "/bandwidth_snapshots";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["bytes_received"] = 7, ["bytes_sent"] = 7, ["sync_bytes_received"] = 7, ["sync_bytes_sent"] = 7, ["requests_get"] = 7, ["requests_put"] = 7, ["requests_other"] = 7, ["logged_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "BandwidthSnapshot", ("id", "int64"), ("bytes_received", "int64"), ("bytes_sent", "int64"), ("sync_bytes_received", "int64"), ("sync_bytes_sent", "int64"), ("requests_get", "int64"), ("requests_put", "int64"), ("requests_other", "int64"), ("logged_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the BandwidthSnapshot the SDK returns gives it.
        private static void AssertModel(JsonObject record, BandwidthSnapshot model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no BandwidthSnapshot");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "bytes_received", model.BytesReceived, what);
            Simulator.Same(record, "bytes_sent", model.BytesSent, what);
            Simulator.Same(record, "sync_bytes_received", model.SyncBytesReceived, what);
            Simulator.Same(record, "sync_bytes_sent", model.SyncBytesSent, what);
            Simulator.Same(record, "requests_get", model.RequestsGet, what);
            Simulator.Same(record, "requests_put", model.RequestsPut, what);
            Simulator.Same(record, "requests_other", model.RequestsOther, what);
            Simulator.Same(record, "logged_at", model.LoggedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("BandwidthSnapshot", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            BandwidthSnapshot[] listed = (await Simulator.Sdk(operation).BandwidthSnapshots.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"BandwidthSnapshot.list[{i}]");
            }
        }
    }
}