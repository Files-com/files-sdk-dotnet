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

    // The record contract of FileMigrationLog, generated from the schema: each case sends one request through the
    // SDK's FileMigrationLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class FileMigrationLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/file_migration_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["timestamp"] = "2030-01-02T03:04:05Z", ["file_migration_id"] = 7, ["dest_path"] = "contract-dest_path", ["error_type"] = "contract-error_type", ["message"] = "contract-message", ["operation"] = "copy", ["path"] = "contract-path", ["status"] = "success", ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "FileMigrationLog", ("timestamp", "date-time"), ("file_migration_id", "int64"), ("dest_path", "string"), ("error_type", "string"), ("message", "string"), ("operation", "string"), ("path", "string"), ("status", "string"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the FileMigrationLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, FileMigrationLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no FileMigrationLog");
            Simulator.Same(record, "timestamp", model.Timestamp, what);
            Simulator.Same(record, "file_migration_id", model.FileMigrationId, what);
            Simulator.Same(record, "dest_path", model.DestPath, what);
            Simulator.Same(record, "error_type", model.ErrorType, what);
            Simulator.Same(record, "message", model.Message, what);
            Simulator.Same(record, "operation", model.Operation, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("FileMigrationLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            FileMigrationLog[] listed = (await Simulator.Sdk(operation).FileMigrationLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"FileMigrationLog.list[{i}]");
            }
        }
    }
}