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

    // The record contract of Sync, generated from the schema: each case sends one request through the
    // SDK's Syncs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SyncRecordContractTest : ContractCase
    {
        private const string ListPath = "/syncs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Sync", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["delete_empty_folders"] = true, ["description"] = "contract-description", ["dest_path"] = "contract-dest_path", ["dest_remote_server_id"] = (Int64)7, ["disabled"] = true, ["exclude_patterns"] = new string[] { "a" }, ["holiday_region"] = "contract-holiday_region", ["include_patterns"] = new string[] { "a" }, ["interval"] = "contract-interval", ["keep_after_copy"] = true, ["name"] = "contract-name", ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["schedule_times_of_day"] = new string[] { "a" }, ["src_path"] = "contract-src_path", ["src_remote_server_id"] = (Int64)7, ["sync_interval_minutes"] = (Int64)7, ["trigger"] = "daily", ["trigger_file"] = "contract-trigger_file", ["always_write_trigger_file"] = true, ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Sync", ("id", "int64"), ("name", "string"), ("description", "string"), ("site_id", "int64"), ("workspace_id", "int64"), ("user_id", "int64"), ("src_path", "string"), ("dest_path", "string"), ("src_remote_server_id", "int64"), ("dest_remote_server_id", "int64"), ("src_site_id", "int64"), ("dest_site_id", "int64"), ("two_way", "boolean"), ("keep_after_copy", "boolean"), ("delete_empty_folders", "boolean"), ("disabled", "boolean"), ("trigger", "string"), ("trigger_file", "string"), ("always_write_trigger_file", "boolean"), ("include_patterns", "array(string)"), ("exclude_patterns", "array(string)"), ("created_at", "date-time"), ("updated_at", "date-time"), ("sync_interval_minutes", "int64"), ("interval", "string"), ("recurring_day", "int64"), ("recurring_days", "array(int64)"), ("schedule_id", "int64"), ("schedule_days_of_week", "array(int64)"), ("schedule_times_of_day", "array(string)"), ("schedule_time_zone", "string"), ("holiday_region", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Sync the SDK returns gives it.
        private static void AssertModel(JsonObject record, Sync model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Sync");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "site_id", model.SiteId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "src_path", model.SrcPath, what);
            Simulator.Same(record, "dest_path", model.DestPath, what);
            Simulator.Same(record, "src_remote_server_id", model.SrcRemoteServerId, what);
            Simulator.Same(record, "dest_remote_server_id", model.DestRemoteServerId, what);
            Simulator.Same(record, "src_site_id", model.SrcSiteId, what);
            Simulator.Same(record, "dest_site_id", model.DestSiteId, what);
            Simulator.Same(record, "two_way", model.TwoWay, what);
            Simulator.Same(record, "keep_after_copy", model.KeepAfterCopy, what);
            Simulator.Same(record, "delete_empty_folders", model.DeleteEmptyFolders, what);
            Simulator.Same(record, "disabled", model.Disabled, what);
            Simulator.Same(record, "trigger", model.Trigger, what);
            Simulator.Same(record, "trigger_file", model.TriggerFile, what);
            Simulator.Same(record, "always_write_trigger_file", model.AlwaysWriteTriggerFile, what);
            Simulator.Same(record, "include_patterns", model.IncludePatterns, what);
            Simulator.Same(record, "exclude_patterns", model.ExcludePatterns, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
            Simulator.Same(record, "sync_interval_minutes", model.SyncIntervalMinutes, what);
            Simulator.Same(record, "interval", model.Interval, what);
            Simulator.Same(record, "recurring_day", model.RecurringDay, what);
            Simulator.Same(record, "recurring_days", model.RecurringDays, what);
            Simulator.Same(record, "schedule_id", model.ScheduleId, what);
            Simulator.Same(record, "schedule_days_of_week", model.ScheduleDaysOfWeek, what);
            Simulator.Same(record, "schedule_times_of_day", model.ScheduleTimesOfDay, what);
            Simulator.Same(record, "schedule_time_zone", model.ScheduleTimeZone, what);
            Simulator.Same(record, "holiday_region", model.HolidayRegion, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Sync", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["delete_empty_folders"] = true, ["description"] = "contract-description", ["dest_path"] = "contract-dest_path", ["dest_remote_server_id"] = (Int64)7, ["disabled"] = true, ["exclude_patterns"] = new string[] { "a" }, ["holiday_region"] = "contract-holiday_region", ["include_patterns"] = new string[] { "a" }, ["interval"] = "contract-interval", ["keep_after_copy"] = true, ["name"] = "contract-name", ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["schedule_times_of_day"] = new string[] { "a" }, ["src_path"] = "contract-src_path", ["src_remote_server_id"] = (Int64)7, ["sync_interval_minutes"] = (Int64)7, ["trigger"] = "daily", ["trigger_file"] = "contract-trigger_file", ["always_write_trigger_file"] = true, ["workspace_id"] = (Int64)7 };
            Sync created = await Simulator.Sdk(operation).Syncs.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["delete_empty_folders"] = "boolean", ["description"] = "string", ["dest_path"] = "string", ["dest_remote_server_id"] = "number", ["disabled"] = "boolean", ["exclude_patterns"] = "array", ["holiday_region"] = "string", ["include_patterns"] = "array", ["interval"] = "string", ["keep_after_copy"] = "boolean", ["name"] = "string", ["recurring_day"] = "number", ["recurring_days"] = "array", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_time_zone"] = "string", ["schedule_times_of_day"] = "array", ["src_path"] = "string", ["src_remote_server_id"] = "number", ["sync_interval_minutes"] = "number", ["trigger"] = "string", ["trigger_file"] = "string", ["always_write_trigger_file"] = "boolean", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Sync.create", "delete_empty_folders", "description", "dest_path", "dest_remote_server_id", "disabled", "exclude_patterns", "holiday_region", "include_patterns", "interval", "keep_after_copy", "name", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_time_zone", "schedule_times_of_day", "src_path", "src_remote_server_id", "sync_interval_minutes", "trigger", "trigger_file", "always_write_trigger_file", "workspace_id");
            AssertModel(record, created, "Sync.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Sync", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Sync[] listed = (await Simulator.Sdk(operation).Syncs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Sync.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Sync", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Sync found = await Simulator.Sdk(operation).Syncs.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Sync.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Sync", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["delete_empty_folders"] = false;
            sent["description"] = "contract-description-2";
            sent["dest_path"] = "contract-dest_path-2";
            sent["dest_remote_server_id"] = (Int64)8;
            sent["disabled"] = false;
            sent["exclude_patterns"] = new string[] { "b" };
            sent["holiday_region"] = "contract-holiday_region-2";
            sent["include_patterns"] = new string[] { "b" };
            sent["interval"] = "contract-interval-2";
            sent["keep_after_copy"] = false;
            sent["name"] = "contract-name-2";
            sent["recurring_day"] = (Int64)8;
            sent["recurring_days"] = new Nullable<Int64>[] { 4 };
            sent["schedule_id"] = (Int64)8;
            sent["schedule_days_of_week"] = new Nullable<Int64>[] { 4 };
            sent["schedule_time_zone"] = "contract-schedule_time_zone-2";
            sent["schedule_times_of_day"] = new string[] { "b" };
            sent["src_path"] = "contract-src_path-2";
            sent["src_remote_server_id"] = (Int64)8;
            sent["sync_interval_minutes"] = (Int64)8;
            sent["trigger"] = "custom_schedule";
            sent["trigger_file"] = "contract-trigger_file-2";
            sent["always_write_trigger_file"] = false;
            Sync updated = await Simulator.Sdk(operation).Syncs.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["delete_empty_folders"] = "boolean", ["description"] = "string", ["dest_path"] = "string", ["dest_remote_server_id"] = "number", ["disabled"] = "boolean", ["exclude_patterns"] = "array", ["holiday_region"] = "string", ["include_patterns"] = "array", ["interval"] = "string", ["keep_after_copy"] = "boolean", ["name"] = "string", ["recurring_day"] = "number", ["recurring_days"] = "array", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_time_zone"] = "string", ["schedule_times_of_day"] = "array", ["src_path"] = "string", ["src_remote_server_id"] = "number", ["sync_interval_minutes"] = "number", ["trigger"] = "string", ["trigger_file"] = "string", ["always_write_trigger_file"] = "boolean" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Sync.update", "delete_empty_folders", "description", "dest_path", "dest_remote_server_id", "disabled", "exclude_patterns", "holiday_region", "include_patterns", "interval", "keep_after_copy", "name", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_time_zone", "schedule_times_of_day", "src_path", "src_remote_server_id", "sync_interval_minutes", "trigger", "trigger_file", "always_write_trigger_file");
            AssertModel(now, updated, "Sync.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Sync", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Syncs.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Sync.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}