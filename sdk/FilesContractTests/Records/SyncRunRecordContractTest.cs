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

    // The record contract of SyncRun, generated from the schema: each case sends one request through the
    // SDK's SyncRuns operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SyncRunRecordContractTest : ContractCase
    {
        private const string ListPath = "/sync_runs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["body"] = "contract-body", ["bytes_synced"] = 7, ["compared_files"] = 7, ["compared_folders"] = 7, ["completed_at"] = "2030-01-02T03:04:05Z", ["created_at"] = "2030-01-02T03:04:05Z", ["dest_remote_server_type"] = "ftp", ["dry_run"] = true, ["errored_files"] = 7, ["estimated_bytes_count"] = 7, ["event_errors"] = new JsonArray("a"), ["log_url"] = "contract-log_url", ["runtime"] = 1.5, ["site_id"] = 7, ["workspace_id"] = 7, ["src_remote_server_type"] = "ftp", ["status"] = "success", ["successful_files"] = 7, ["sync_id"] = 7, ["sync_name"] = "contract-sync_name", ["updated_at"] = "2030-01-02T03:04:05Z" };
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
                Simulator.Conforms(record, "SyncRun", ("id", "int64"), ("body", "string"), ("bytes_synced", "int64"), ("compared_files", "int64"), ("compared_folders", "int64"), ("completed_at", "date-time"), ("created_at", "date-time"), ("dest_remote_server_type", "string"), ("dry_run", "boolean"), ("errored_files", "int64"), ("estimated_bytes_count", "int64"), ("event_errors", "array(string)"), ("log_url", "string"), ("runtime", "double"), ("site_id", "int64"), ("workspace_id", "int64"), ("src_remote_server_type", "string"), ("status", "string"), ("successful_files", "int64"), ("sync_id", "int64"), ("sync_name", "string"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the SyncRun the SDK returns gives it.
        private static void AssertModel(JsonObject record, SyncRun model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no SyncRun");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "body", model.Body, what);
            Simulator.Same(record, "bytes_synced", model.BytesSynced, what);
            Simulator.Same(record, "compared_files", model.ComparedFiles, what);
            Simulator.Same(record, "compared_folders", model.ComparedFolders, what);
            Simulator.Same(record, "completed_at", model.CompletedAt, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "dest_remote_server_type", model.DestRemoteServerType, what);
            Simulator.Same(record, "dry_run", model.DryRun, what);
            Simulator.Same(record, "errored_files", model.ErroredFiles, what);
            Simulator.Same(record, "estimated_bytes_count", model.EstimatedBytesCount, what);
            Simulator.Same(record, "event_errors", model.EventErrors, what);
            Simulator.Same(record, "log_url", model.LogUrl, what);
            Simulator.Same(record, "runtime", model.Runtime, what);
            Simulator.Same(record, "site_id", model.SiteId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "src_remote_server_type", model.SrcRemoteServerType, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "successful_files", model.SuccessfulFiles, what);
            Simulator.Same(record, "sync_id", model.SyncId, what);
            Simulator.Same(record, "sync_name", model.SyncName, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("SyncRun", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SyncRun[] listed = (await Simulator.Sdk(operation).SyncRuns.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"SyncRun.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("SyncRun", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SyncRun found = await Simulator.Sdk(operation).SyncRuns.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "SyncRun.find");
        }
    }
}