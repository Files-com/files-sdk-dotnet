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

    // The record contract of FileMigration, generated from the schema: each case sends one request through the
    // SDK's FileMigrations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class FileMigrationRecordContractTest : ContractCase
    {
        private const string ListPath = "/file_migrations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            // A FileMigration is the state of a file action the simulator answered pending, never a reset fixture: each
            // record is made by a raw copy of a fixture file under the pending file_actions profile, then read raw until
            // it completes, after which reading it changes nothing.
            JsonNode[] files = Enumerable.Range(1, count).Select(n => (JsonNode)new JsonObject { ["path"] = $"contract-migration-{n}.txt", ["text"] = "contract" }).ToArray();
            await Simulator.ResetAsync(new JsonObject { ["files"] = new JsonArray(files) }, new JsonObject { ["file_actions"] = "pending" });
            List<JsonObject> held = new List<JsonObject>();
            for (int n = 1; n <= count; n++)
            {
                JsonObject action = await Simulator.CreateRawAsync($"/file_actions/copy/contract-migration-{n}.txt", new JsonObject { ["destination"] = $"contract-migration-{n}-copy.txt" });
                Assert.AreEqual("pending", (string)action["status"], $"the raw copy answered a pending FileMigration: {action.ToJsonString()}");
                string member = $"{ListPath}/{(long)action["file_migration_id"]}";
                JsonObject first = await Simulator.ReadRawAsync(ListPath, member, null, null);
                Assert.AreEqual("processing", (string)first["status"], $"the FileMigration's first raw read: {first.ToJsonString()}");
                JsonObject completed = await Simulator.ReadRawAsync(ListPath, member, null, null);
                Assert.AreEqual("completed", (string)completed["status"], $"the FileMigration's second raw read: {completed.ToJsonString()}");
                held.Add(completed);
            }
            return Conform(held.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "FileMigration", ("id", "int64"), ("path", "string"), ("dest_path", "string"), ("failure_message", "string"), ("files_moved", "int64"), ("files_total", "int64"), ("operation", "string"), ("region", "string"), ("status", "string"), ("log_url", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the FileMigration the SDK returns gives it.
        private static void AssertModel(JsonObject record, FileMigration model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no FileMigration");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "dest_path", model.DestPath, what);
            Simulator.Same(record, "failure_message", model.FailureMessage, what);
            Simulator.Same(record, "files_moved", model.FilesMoved, what);
            Simulator.Same(record, "files_total", model.FilesTotal, what);
            Simulator.Same(record, "operation", model.Operation, what);
            Simulator.Same(record, "region", model.Region, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "log_url", model.LogUrl, what);
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("FileMigration", "find", "file_migrations.find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            FileMigration found = await Simulator.Sdk(operation).FileMigrations.FindAsync((long)record["id"], sent);
            JsonObject request = await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            Assert.AreEqual(record["id"]?.ToString(), request["id"]?.ToString(), "FileMigration.find: the migration the SDK's request named");
            Assert.AreEqual("completed", (string)request["migration_status"], "FileMigration.find: the SDK read the completed migration, and its read left it completed");
            AssertModel(record, found, "FileMigration.find");
        }
    }
}