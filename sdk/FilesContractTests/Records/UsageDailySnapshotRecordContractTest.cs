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

    // The record contract of UsageDailySnapshot, generated from the schema: each case sends one request through the
    // SDK's UsageDailySnapshots operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class UsageDailySnapshotRecordContractTest : ContractCase
    {
        private const string ListPath = "/usage_daily_snapshots";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["api_usage_available"] = true, ["read_api_usage"] = 7, ["write_api_usage"] = 7, ["transformation_credits_usage"] = 7, ["user_count"] = 7, ["current_storage"] = 7, ["deleted_files_storage"] = 7, ["deleted_files_counted_in_minimum"] = 7, ["root_storage"] = 7 };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "UsageDailySnapshot", ("id", "int64"), ("api_usage_available", "boolean"), ("read_api_usage", "int64"), ("write_api_usage", "int64"), ("transformation_credits_usage", "int64"), ("user_count", "int64"), ("current_storage", "int64"), ("deleted_files_storage", "int64"), ("deleted_files_counted_in_minimum", "int64"), ("root_storage", "int64"));
            }
            return records;
        }

        // Every property the record holds, as the UsageDailySnapshot the SDK returns gives it.
        private static void AssertModel(JsonObject record, UsageDailySnapshot model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no UsageDailySnapshot");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "api_usage_available", model.ApiUsageAvailable, what);
            Simulator.Same(record, "read_api_usage", model.ReadApiUsage, what);
            Simulator.Same(record, "write_api_usage", model.WriteApiUsage, what);
            Simulator.Same(record, "transformation_credits_usage", model.TransformationCreditsUsage, what);
            Simulator.Same(record, "user_count", model.UserCount, what);
            Simulator.Same(record, "current_storage", model.CurrentStorage, what);
            Simulator.Same(record, "deleted_files_storage", model.DeletedFilesStorage, what);
            Simulator.Same(record, "deleted_files_counted_in_minimum", model.DeletedFilesCountedInMinimum, what);
            Simulator.Same(record, "root_storage", model.RootStorage, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("UsageDailySnapshot", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UsageDailySnapshot[] listed = (await Simulator.Sdk(operation).UsageDailySnapshots.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"UsageDailySnapshot.list[{i}]");
            }
        }
    }
}