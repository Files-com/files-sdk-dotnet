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

    // The record contract of ExpectationEvaluation, generated from the schema: each case sends one request through the
    // SDK's ExpectationEvaluations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ExpectationEvaluationRecordContractTest : ContractCase
    {
        private const string ListPath = "/expectation_evaluations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["workspace_id"] = 7, ["expectation_id"] = 7, ["status"] = "open", ["opened_via"] = "schedule", ["opened_at"] = "2030-01-02T03:04:05Z", ["window_start_at"] = "2030-01-02T03:04:05Z", ["window_end_at"] = "2030-01-02T03:04:05Z", ["deadline_at"] = "2030-01-02T03:04:05Z", ["late_acceptance_cutoff_at"] = "2030-01-02T03:04:05Z", ["hard_close_at"] = "2030-01-02T03:04:05Z", ["closed_at"] = "2030-01-02T03:04:05Z", ["criteria_errors"] = new JsonArray("a"), ["created_at"] = "2030-01-02T03:04:05Z", ["updated_at"] = "2030-01-02T03:04:05Z" };
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
                Simulator.Conforms(record, "ExpectationEvaluation", ("id", "int64"), ("workspace_id", "int64"), ("expectation_id", "int64"), ("status", "string"), ("opened_via", "string"), ("opened_at", "date-time"), ("window_start_at", "date-time"), ("window_end_at", "date-time"), ("deadline_at", "date-time"), ("late_acceptance_cutoff_at", "date-time"), ("hard_close_at", "date-time"), ("closed_at", "date-time"), ("criteria_errors", "array(string)"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the ExpectationEvaluation the SDK returns gives it.
        private static void AssertModel(JsonObject record, ExpectationEvaluation model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ExpectationEvaluation");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "expectation_id", model.ExpectationId, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "opened_via", model.OpenedVia, what);
            Simulator.Same(record, "opened_at", model.OpenedAt, what);
            Simulator.Same(record, "window_start_at", model.WindowStartAt, what);
            Simulator.Same(record, "window_end_at", model.WindowEndAt, what);
            Simulator.Same(record, "deadline_at", model.DeadlineAt, what);
            Simulator.Same(record, "late_acceptance_cutoff_at", model.LateAcceptanceCutoffAt, what);
            Simulator.Same(record, "hard_close_at", model.HardCloseAt, what);
            Simulator.Same(record, "closed_at", model.ClosedAt, what);
            Simulator.Same(record, "criteria_errors", model.CriteriaErrors, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ExpectationEvaluation", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ExpectationEvaluation[] listed = (await Simulator.Sdk(operation).ExpectationEvaluations.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ExpectationEvaluation.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("ExpectationEvaluation", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ExpectationEvaluation found = await Simulator.Sdk(operation).ExpectationEvaluations.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "ExpectationEvaluation.find");
        }
    }
}