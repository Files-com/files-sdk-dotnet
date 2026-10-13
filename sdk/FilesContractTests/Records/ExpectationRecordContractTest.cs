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

    // The record contract of Expectation, generated from the schema: each case sends one request through the
    // SDK's Expectations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ExpectationRecordContractTest : ContractCase
    {
        private const string ListPath = "/expectations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Expectation", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["description"] = "contract-description", ["path"] = "contract-path", ["source"] = "contract-source", ["exclude_pattern"] = "contract-exclude_pattern", ["disabled"] = true, ["trigger"] = "manual", ["interval"] = "contract-interval", ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_times_of_day"] = new string[] { "a" }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["holiday_region"] = "contract-holiday_region", ["lookback_interval"] = (Int64)7, ["late_acceptance_interval"] = (Int64)7, ["inactivity_interval"] = (Int64)7, ["max_open_interval"] = (Int64)7, ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Expectation", ("id", "int64"), ("workspace_id", "int64"), ("name", "string"), ("description", "string"), ("path", "string"), ("source", "string"), ("exclude_pattern", "string"), ("disabled", "boolean"), ("expectations_version", "int64"), ("trigger", "string"), ("interval", "string"), ("recurring_day", "int64"), ("recurring_days", "array(int64)"), ("schedule_id", "int64"), ("schedule_days_of_week", "array(int64)"), ("schedule_times_of_day", "array(string)"), ("schedule_time_zone", "string"), ("holiday_region", "string"), ("lookback_interval", "int64"), ("late_acceptance_interval", "int64"), ("inactivity_interval", "int64"), ("max_open_interval", "int64"), ("last_evaluated_at", "date-time"), ("last_success_at", "date-time"), ("last_failure_at", "date-time"), ("last_result", "string"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Expectation the SDK returns gives it.
        private static void AssertModel(JsonObject record, Expectation model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Expectation");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "source", model.Source, what);
            Simulator.Same(record, "exclude_pattern", model.ExcludePattern, what);
            Simulator.Same(record, "disabled", model.Disabled, what);
            Simulator.Same(record, "expectations_version", model.ExpectationsVersion, what);
            Simulator.Same(record, "trigger", model.Trigger, what);
            Simulator.Same(record, "interval", model.Interval, what);
            Simulator.Same(record, "recurring_day", model.RecurringDay, what);
            Simulator.Same(record, "recurring_days", model.RecurringDays, what);
            Simulator.Same(record, "schedule_id", model.ScheduleId, what);
            Simulator.Same(record, "schedule_days_of_week", model.ScheduleDaysOfWeek, what);
            Simulator.Same(record, "schedule_times_of_day", model.ScheduleTimesOfDay, what);
            Simulator.Same(record, "schedule_time_zone", model.ScheduleTimeZone, what);
            Simulator.Same(record, "holiday_region", model.HolidayRegion, what);
            Simulator.Same(record, "lookback_interval", model.LookbackInterval, what);
            Simulator.Same(record, "late_acceptance_interval", model.LateAcceptanceInterval, what);
            Simulator.Same(record, "inactivity_interval", model.InactivityInterval, what);
            Simulator.Same(record, "max_open_interval", model.MaxOpenInterval, what);
            Simulator.Same(record, "last_evaluated_at", model.LastEvaluatedAt, what);
            Simulator.Same(record, "last_success_at", model.LastSuccessAt, what);
            Simulator.Same(record, "last_failure_at", model.LastFailureAt, what);
            Simulator.Same(record, "last_result", model.LastResult, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Expectation", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["description"] = "contract-description", ["path"] = "contract-path", ["source"] = "contract-source", ["exclude_pattern"] = "contract-exclude_pattern", ["disabled"] = true, ["trigger"] = "manual", ["interval"] = "contract-interval", ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_times_of_day"] = new string[] { "a" }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["holiday_region"] = "contract-holiday_region", ["lookback_interval"] = (Int64)7, ["late_acceptance_interval"] = (Int64)7, ["inactivity_interval"] = (Int64)7, ["max_open_interval"] = (Int64)7, ["workspace_id"] = (Int64)7 };
            Expectation created = await Simulator.Sdk(operation).Expectations.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["description"] = "string", ["path"] = "string", ["source"] = "string", ["exclude_pattern"] = "string", ["disabled"] = "boolean", ["trigger"] = "string", ["interval"] = "string", ["recurring_day"] = "number", ["recurring_days"] = "array", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_times_of_day"] = "array", ["schedule_time_zone"] = "string", ["holiday_region"] = "string", ["lookback_interval"] = "number", ["late_acceptance_interval"] = "number", ["inactivity_interval"] = "number", ["max_open_interval"] = "number", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Expectation.create", "name", "description", "path", "source", "exclude_pattern", "disabled", "trigger", "interval", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region", "lookback_interval", "late_acceptance_interval", "inactivity_interval", "max_open_interval", "workspace_id");
            AssertModel(record, created, "Expectation.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Expectation", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Expectation[] listed = (await Simulator.Sdk(operation).Expectations.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Expectation.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Expectation", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Expectation found = await Simulator.Sdk(operation).Expectations.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Expectation.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Expectation", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["description"] = "contract-description-2";
            sent["path"] = "contract-path-2";
            sent["source"] = "contract-source-2";
            sent["exclude_pattern"] = "contract-exclude_pattern-2";
            sent["disabled"] = false;
            sent["trigger"] = "upload";
            sent["interval"] = "contract-interval-2";
            sent["recurring_day"] = (Int64)8;
            sent["recurring_days"] = new Nullable<Int64>[] { 4 };
            sent["schedule_id"] = (Int64)8;
            sent["schedule_days_of_week"] = new Nullable<Int64>[] { 4 };
            sent["schedule_times_of_day"] = new string[] { "b" };
            sent["schedule_time_zone"] = "contract-schedule_time_zone-2";
            sent["holiday_region"] = "contract-holiday_region-2";
            sent["lookback_interval"] = (Int64)8;
            sent["late_acceptance_interval"] = (Int64)8;
            sent["inactivity_interval"] = (Int64)8;
            sent["max_open_interval"] = (Int64)8;
            sent["workspace_id"] = (Int64)8;
            Expectation updated = await Simulator.Sdk(operation).Expectations.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["description"] = "string", ["path"] = "string", ["source"] = "string", ["exclude_pattern"] = "string", ["disabled"] = "boolean", ["trigger"] = "string", ["interval"] = "string", ["recurring_day"] = "number", ["recurring_days"] = "array", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_times_of_day"] = "array", ["schedule_time_zone"] = "string", ["holiday_region"] = "string", ["lookback_interval"] = "number", ["late_acceptance_interval"] = "number", ["inactivity_interval"] = "number", ["max_open_interval"] = "number", ["workspace_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Expectation.update", "name", "description", "path", "source", "exclude_pattern", "disabled", "trigger", "interval", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region", "lookback_interval", "late_acceptance_interval", "inactivity_interval", "max_open_interval", "workspace_id");
            AssertModel(now, updated, "Expectation.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Expectation", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Expectations.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Expectation.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}