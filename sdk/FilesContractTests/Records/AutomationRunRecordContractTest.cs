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

    // The record contract of AutomationRun, generated from the schema: each case sends one request through the
    // SDK's AutomationRuns operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class AutomationRunRecordContractTest : ContractCase
    {
        private const string ListPath = "/automation_runs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { ["automation_id"] = (Int64)7 });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["automation_id"] = 7, ["automation_version_id"] = 7, ["version"] = 7, ["workspace_id"] = 7, ["cancel_requested_at"] = "2030-01-02T03:04:05Z", ["completed_at"] = "2030-01-02T03:04:05Z", ["created_at"] = "2030-01-02T03:04:05Z", ["retry_at"] = "2030-01-02T03:04:05Z", ["retried_at"] = "2030-01-02T03:04:05Z", ["retried_in_run_id"] = 7, ["retry_of_run_id"] = 7, ["rerun_of_run_id"] = 7, ["rerun_from_node_id"] = "contract-rerun_from_node_id", ["runtime"] = 1.5, ["status"] = "running", ["successful_operations"] = 7, ["failed_operations"] = 7, ["journal_url"] = "contract-journal_url", ["status_messages_url"] = "contract-status_messages_url" };
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
                Simulator.Conforms(record, "AutomationRun", ("id", "int64"), ("automation_id", "int64"), ("automation_version_id", "int64"), ("version", "int64"), ("workspace_id", "int64"), ("cancel_requested_at", "date-time"), ("completed_at", "date-time"), ("created_at", "date-time"), ("retry_at", "date-time"), ("retried_at", "date-time"), ("retried_in_run_id", "int64"), ("retry_of_run_id", "int64"), ("rerun_of_run_id", "int64"), ("rerun_from_node_id", "string"), ("runtime", "double"), ("status", "string"), ("successful_operations", "int64"), ("failed_operations", "int64"), ("journal_url", "string"), ("status_messages_url", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the AutomationRun the SDK returns gives it.
        private static void AssertModel(JsonObject record, AutomationRun model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no AutomationRun");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "automation_id", model.AutomationId, what);
            Simulator.Same(record, "automation_version_id", model.AutomationVersionId, what);
            Simulator.Same(record, "version", model.Version, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "cancel_requested_at", model.CancelRequestedAt, what);
            Simulator.Same(record, "completed_at", model.CompletedAt, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "retry_at", model.RetryAt, what);
            Simulator.Same(record, "retried_at", model.RetriedAt, what);
            Simulator.Same(record, "retried_in_run_id", model.RetriedInRunId, what);
            Simulator.Same(record, "retry_of_run_id", model.RetryOfRunId, what);
            Simulator.Same(record, "rerun_of_run_id", model.RerunOfRunId, what);
            Simulator.Same(record, "rerun_from_node_id", model.RerunFromNodeId, what);
            Simulator.Same(record, "runtime", model.Runtime, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "successful_operations", model.SuccessfulOperations, what);
            Simulator.Same(record, "failed_operations", model.FailedOperations, what);
            Simulator.Same(record, "journal_url", model.JournalUrl, what);
            Simulator.Same(record, "status_messages_url", model.StatusMessagesUrl, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("AutomationRun", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { ["automation_id"] = (Int64)7 };
            AutomationRun[] listed = (await Simulator.Sdk(operation).AutomationRuns.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["automation_id"] = "string" });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"AutomationRun.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("AutomationRun", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            AutomationRun found = await Simulator.Sdk(operation).AutomationRuns.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "AutomationRun.find");
        }
    }
}