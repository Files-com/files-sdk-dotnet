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

    // The record contract of ScheduledExport, generated from the schema: each case sends one request through the
    // SDK's ScheduledExports operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ScheduledExportRecordContractTest : ContractCase
    {
        private const string ListPath = "/scheduled_exports";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("ScheduledExport", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["export_type"] = "contract-export_type", ["user_id"] = (Int64)7, ["disabled"] = true, ["trigger"] = "daily", ["interval"] = "contract-interval", ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_times_of_day"] = new string[] { "a" }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["holiday_region"] = "contract-holiday_region" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ScheduledExport", ("id", "int64"), ("name", "string"), ("export_type", "string"), ("report_name", "string"), ("user_id", "int64"), ("disabled", "boolean"), ("trigger", "string"), ("interval", "string"), ("recurring_day", "int64"), ("recurring_days", "array(int64)"), ("schedule_id", "int64"), ("schedule_days_of_week", "array(int64)"), ("schedule_times_of_day", "array(string)"), ("schedule_time_zone", "string"), ("holiday_region", "string"), ("human_readable_schedule", "string"), ("last_run_at", "date-time"), ("last_export_id", "int64"), ("last_error", "string"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the ScheduledExport the SDK returns gives it.
        private static void AssertModel(JsonObject record, ScheduledExport model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ScheduledExport");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "export_type", model.ExportType, what);
            Simulator.Same(record, "report_name", model.ReportName, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "disabled", model.Disabled, what);
            Simulator.Same(record, "trigger", model.Trigger, what);
            Simulator.Same(record, "interval", model.Interval, what);
            Simulator.Same(record, "recurring_day", model.RecurringDay, what);
            Simulator.Same(record, "recurring_days", model.RecurringDays, what);
            Simulator.Same(record, "schedule_id", model.ScheduleId, what);
            Simulator.Same(record, "schedule_days_of_week", model.ScheduleDaysOfWeek, what);
            Simulator.Same(record, "schedule_times_of_day", model.ScheduleTimesOfDay, what);
            Simulator.Same(record, "schedule_time_zone", model.ScheduleTimeZone, what);
            Simulator.Same(record, "holiday_region", model.HolidayRegion, what);
            Simulator.Same(record, "human_readable_schedule", model.HumanReadableSchedule, what);
            Simulator.Same(record, "last_run_at", model.LastRunAt, what);
            Simulator.Same(record, "last_export_id", model.LastExportId, what);
            Simulator.Same(record, "last_error", model.LastError, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("ScheduledExport", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["export_type"] = "contract-export_type", ["user_id"] = (Int64)7, ["disabled"] = true, ["trigger"] = "daily", ["interval"] = "contract-interval", ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_times_of_day"] = new string[] { "a" }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["holiday_region"] = "contract-holiday_region" };
            ScheduledExport created = await Simulator.Sdk(operation).ScheduledExports.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["export_type"] = "string", ["user_id"] = "number", ["disabled"] = "boolean", ["trigger"] = "string", ["interval"] = "string", ["recurring_day"] = "number", ["recurring_days"] = "array", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_times_of_day"] = "array", ["schedule_time_zone"] = "string", ["holiday_region"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "ScheduledExport.create", "name", "export_type", "user_id", "disabled", "trigger", "interval", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region");
            AssertModel(record, created, "ScheduledExport.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ScheduledExport", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ScheduledExport[] listed = (await Simulator.Sdk(operation).ScheduledExports.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ScheduledExport.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("ScheduledExport", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ScheduledExport found = await Simulator.Sdk(operation).ScheduledExports.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "ScheduledExport.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("ScheduledExport", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["export_type"] = "contract-export_type-2";
            sent["user_id"] = (Int64)8;
            sent["disabled"] = false;
            sent["trigger"] = "custom_schedule";
            sent["interval"] = "contract-interval-2";
            sent["recurring_day"] = (Int64)8;
            sent["recurring_days"] = new Nullable<Int64>[] { 4 };
            sent["schedule_id"] = (Int64)8;
            sent["schedule_days_of_week"] = new Nullable<Int64>[] { 4 };
            sent["schedule_times_of_day"] = new string[] { "b" };
            sent["schedule_time_zone"] = "contract-schedule_time_zone-2";
            sent["holiday_region"] = "contract-holiday_region-2";
            ScheduledExport updated = await Simulator.Sdk(operation).ScheduledExports.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["export_type"] = "string", ["user_id"] = "number", ["disabled"] = "boolean", ["trigger"] = "string", ["interval"] = "string", ["recurring_day"] = "number", ["recurring_days"] = "array", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_times_of_day"] = "array", ["schedule_time_zone"] = "string", ["holiday_region"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "ScheduledExport.update", "name", "export_type", "user_id", "disabled", "trigger", "interval", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region");
            AssertModel(now, updated, "ScheduledExport.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("ScheduledExport", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).ScheduledExports.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"ScheduledExport.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}