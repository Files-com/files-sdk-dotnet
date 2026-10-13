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

    // The record contract of AiTask, generated from the schema: each case sends one request through the
    // SDK's AiTasks operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class AiTaskRecordContractTest : ContractCase
    {
        private const string ListPath = "/ai_tasks";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("AiTask", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["description"] = "contract-description", ["disabled"] = true, ["holiday_region"] = "contract-holiday_region", ["interval"] = "contract-interval", ["name"] = "contract-name", ["path"] = "contract-path", ["permission_set"] = "full", ["prompt"] = "contract-prompt", ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["schedule_times_of_day"] = new string[] { "a" }, ["source"] = "contract-source", ["trigger"] = "manual", ["trigger_actions"] = new string[] { "a" }, ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "AiTask", ("id", "int64"), ("workspace_id", "int64"), ("name", "string"), ("description", "string"), ("prompt", "string"), ("permission_set", "string"), ("path", "string"), ("source", "string"), ("disabled", "boolean"), ("trigger", "string"), ("trigger_actions", "array(string)"), ("interval", "string"), ("recurring_day", "int64"), ("recurring_days", "array(int64)"), ("schedule_id", "int64"), ("schedule_days_of_week", "array(int64)"), ("schedule_times_of_day", "array(string)"), ("schedule_time_zone", "string"), ("holiday_region", "string"), ("human_readable_schedule", "string"), ("last_run_at", "date-time"), ("master_admin_user_id", "int64"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the AiTask the SDK returns gives it.
        private static void AssertModel(JsonObject record, AiTask model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no AiTask");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "prompt", model.Prompt, what);
            Simulator.Same(record, "permission_set", model.PermissionSet, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "source", model.Source, what);
            Simulator.Same(record, "disabled", model.Disabled, what);
            Simulator.Same(record, "trigger", model.Trigger, what);
            Simulator.Same(record, "trigger_actions", model.TriggerActions, what);
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
            Simulator.Same(record, "master_admin_user_id", model.MasterAdminUserId, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("AiTask", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["description"] = "contract-description", ["disabled"] = true, ["holiday_region"] = "contract-holiday_region", ["interval"] = "contract-interval", ["name"] = "contract-name", ["path"] = "contract-path", ["permission_set"] = "full", ["prompt"] = "contract-prompt", ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["schedule_times_of_day"] = new string[] { "a" }, ["source"] = "contract-source", ["trigger"] = "manual", ["trigger_actions"] = new string[] { "a" }, ["workspace_id"] = (Int64)7 };
            AiTask created = await Simulator.Sdk(operation).AiTasks.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["description"] = "string", ["disabled"] = "boolean", ["holiday_region"] = "string", ["interval"] = "string", ["name"] = "string", ["path"] = "string", ["permission_set"] = "string", ["prompt"] = "string", ["recurring_day"] = "number", ["recurring_days"] = "array", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_time_zone"] = "string", ["schedule_times_of_day"] = "array", ["source"] = "string", ["trigger"] = "string", ["trigger_actions"] = "array", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "AiTask.create", "description", "disabled", "holiday_region", "interval", "name", "path", "permission_set", "prompt", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_time_zone", "schedule_times_of_day", "source", "trigger", "trigger_actions", "workspace_id");
            AssertModel(record, created, "AiTask.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("AiTask", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            AiTask[] listed = (await Simulator.Sdk(operation).AiTasks.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"AiTask.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("AiTask", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            AiTask found = await Simulator.Sdk(operation).AiTasks.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "AiTask.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("AiTask", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["description"] = "contract-description-2";
            sent["disabled"] = false;
            sent["holiday_region"] = "contract-holiday_region-2";
            sent["interval"] = "contract-interval-2";
            sent["name"] = "contract-name-2";
            sent["path"] = "contract-path-2";
            sent["permission_set"] = "files_only";
            sent["prompt"] = "contract-prompt-2";
            sent["recurring_day"] = (Int64)8;
            sent["recurring_days"] = new Nullable<Int64>[] { 4 };
            sent["schedule_id"] = (Int64)8;
            sent["schedule_days_of_week"] = new Nullable<Int64>[] { 4 };
            sent["schedule_time_zone"] = "contract-schedule_time_zone-2";
            sent["schedule_times_of_day"] = new string[] { "b" };
            sent["source"] = "contract-source-2";
            sent["trigger"] = "daily";
            sent["trigger_actions"] = new string[] { "b" };
            sent["workspace_id"] = (Int64)8;
            AiTask updated = await Simulator.Sdk(operation).AiTasks.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["description"] = "string", ["disabled"] = "boolean", ["holiday_region"] = "string", ["interval"] = "string", ["name"] = "string", ["path"] = "string", ["permission_set"] = "string", ["prompt"] = "string", ["recurring_day"] = "number", ["recurring_days"] = "array", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_time_zone"] = "string", ["schedule_times_of_day"] = "array", ["source"] = "string", ["trigger"] = "string", ["trigger_actions"] = "array", ["workspace_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "AiTask.update", "description", "disabled", "holiday_region", "interval", "name", "path", "permission_set", "prompt", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_time_zone", "schedule_times_of_day", "source", "trigger", "trigger_actions", "workspace_id");
            AssertModel(now, updated, "AiTask.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("AiTask", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).AiTasks.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"AiTask.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}