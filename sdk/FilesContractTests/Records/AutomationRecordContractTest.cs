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

    // The record contract of Automation, generated from the schema: each case sends one request through the
    // SDK's Automations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class AutomationRecordContractTest : ContractCase
    {
        private const string ListPath = "/automations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Automation", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["source"] = "contract-source", ["destinations"] = new string[] { "a" }, ["destination_replace_from"] = "contract-destination_replace_from", ["destination_replace_to"] = "contract-destination_replace_to", ["interval"] = "contract-interval", ["path"] = "contract-path", ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_times_of_day"] = new string[] { "a" }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["holiday_region"] = "contract-holiday_region", ["always_overwrite_size_matching_files"] = true, ["always_serialize_jobs"] = true, ["description"] = "contract-description", ["disabled"] = true, ["exclude_pattern"] = "contract-exclude_pattern", ["flatten_destination_structure"] = true, ["ignore_locked_folders"] = true, ["legacy_folder_matching"] = true, ["name"] = "contract-name", ["overwrite_files"] = true, ["path_time_zone"] = "contract-path_time_zone", ["retry_on_failure_interval_in_minutes"] = (Int64)7, ["retry_on_failure_number_of_attempts"] = (Int64)7, ["trigger"] = "manual", ["trigger_actions"] = new string[] { "a" }, ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["automation"] = "create_folder", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Automation", ("id", "int64"), ("workspace_id", "int64"), ("always_serialize_jobs", "boolean"), ("always_overwrite_size_matching_files", "boolean"), ("automation", "string"), ("deleted", "boolean"), ("description", "string"), ("destination_replace_from", "string"), ("destination_replace_to", "string"), ("destinations", "array(string)"), ("disabled", "boolean"), ("exclude_pattern", "string"), ("inbound_email_address", "string"), ("flatten_destination_structure", "boolean"), ("group_ids", "array(int64)"), ("ignore_locked_folders", "boolean"), ("interval", "string"), ("last_modified_at", "date-time"), ("legacy_folder_matching", "boolean"), ("name", "string"), ("overwrite_files", "boolean"), ("path", "string"), ("path_time_zone", "string"), ("version", "int64"), ("recurring_day", "int64"), ("recurring_days", "array(int64)"), ("schedule_id", "int64"), ("retry_on_failure_interval_in_minutes", "int64"), ("retry_on_failure_number_of_attempts", "int64"), ("human_readable_schedule", "string"), ("schedule_days_of_week", "array(int64)"), ("schedule_times_of_day", "array(string)"), ("schedule_time_zone", "string"), ("source", "string"), ("sync_ids", "array(int64)"), ("trigger_actions", "array(string)"), ("trigger", "string"), ("user_id", "int64"), ("user_ids", "array(int64)"), ("webhook_url", "string"), ("holiday_region", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Automation the SDK returns gives it.
        private static void AssertModel(JsonObject record, Automation model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Automation");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "always_serialize_jobs", model.AlwaysSerializeJobs, what);
            Simulator.Same(record, "always_overwrite_size_matching_files", model.AlwaysOverwriteSizeMatchingFiles, what);
            Simulator.Same(record, "automation", model.AutomationType, what);
            Simulator.Same(record, "deleted", model.Deleted, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "destination_replace_from", model.DestinationReplaceFrom, what);
            Simulator.Same(record, "destination_replace_to", model.DestinationReplaceTo, what);
            Simulator.Same(record, "destinations", model.Destinations, what);
            Simulator.Same(record, "disabled", model.Disabled, what);
            Simulator.Same(record, "exclude_pattern", model.ExcludePattern, what);
            Simulator.Same(record, "inbound_email_address", model.InboundEmailAddress, what);
            Simulator.Same(record, "flatten_destination_structure", model.FlattenDestinationStructure, what);
            Simulator.Same(record, "group_ids", model.GroupIds, what);
            Simulator.Same(record, "ignore_locked_folders", model.IgnoreLockedFolders, what);
            Simulator.Same(record, "interval", model.Interval, what);
            Simulator.Same(record, "last_modified_at", model.LastModifiedAt, what);
            Simulator.Same(record, "legacy_folder_matching", model.LegacyFolderMatching, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "overwrite_files", model.OverwriteFiles, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "path_time_zone", model.PathTimeZone, what);
            Simulator.Same(record, "version", model.Version, what);
            Simulator.Same(record, "recurring_day", model.RecurringDay, what);
            Simulator.Same(record, "recurring_days", model.RecurringDays, what);
            Simulator.Same(record, "schedule_id", model.ScheduleId, what);
            Simulator.Same(record, "retry_on_failure_interval_in_minutes", model.RetryOnFailureIntervalInMinutes, what);
            Simulator.Same(record, "retry_on_failure_number_of_attempts", model.RetryOnFailureNumberOfAttempts, what);
            Simulator.Same(record, "human_readable_schedule", model.HumanReadableSchedule, what);
            Simulator.Same(record, "schedule_days_of_week", model.ScheduleDaysOfWeek, what);
            Simulator.Same(record, "schedule_times_of_day", model.ScheduleTimesOfDay, what);
            Simulator.Same(record, "schedule_time_zone", model.ScheduleTimeZone, what);
            Simulator.Same(record, "source", model.Source, what);
            Simulator.Same(record, "sync_ids", model.SyncIds, what);
            Simulator.Same(record, "trigger_actions", model.TriggerActions, what);
            Simulator.Same(record, "trigger", model.Trigger, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "user_ids", model.UserIds, what);
            Simulator.Same(record, "webhook_url", model.WebhookUrl, what);
            Simulator.Same(record, "holiday_region", model.HolidayRegion, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Automation", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["source"] = "contract-source", ["destinations"] = new string[] { "a" }, ["destination_replace_from"] = "contract-destination_replace_from", ["destination_replace_to"] = "contract-destination_replace_to", ["interval"] = "contract-interval", ["path"] = "contract-path", ["schedule_id"] = (Int64)7, ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_times_of_day"] = new string[] { "a" }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["holiday_region"] = "contract-holiday_region", ["always_overwrite_size_matching_files"] = true, ["always_serialize_jobs"] = true, ["description"] = "contract-description", ["disabled"] = true, ["exclude_pattern"] = "contract-exclude_pattern", ["flatten_destination_structure"] = true, ["ignore_locked_folders"] = true, ["legacy_folder_matching"] = true, ["name"] = "contract-name", ["overwrite_files"] = true, ["path_time_zone"] = "contract-path_time_zone", ["retry_on_failure_interval_in_minutes"] = (Int64)7, ["retry_on_failure_number_of_attempts"] = (Int64)7, ["trigger"] = "manual", ["trigger_actions"] = new string[] { "a" }, ["recurring_day"] = (Int64)7, ["recurring_days"] = new Nullable<Int64>[] { 3 }, ["automation"] = "create_folder", ["workspace_id"] = (Int64)7 };
            Automation created = await Simulator.Sdk(operation).Automations.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["source"] = "string", ["destinations"] = "array", ["destination_replace_from"] = "string", ["destination_replace_to"] = "string", ["interval"] = "string", ["path"] = "string", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_times_of_day"] = "array", ["schedule_time_zone"] = "string", ["holiday_region"] = "string", ["always_overwrite_size_matching_files"] = "boolean", ["always_serialize_jobs"] = "boolean", ["description"] = "string", ["disabled"] = "boolean", ["exclude_pattern"] = "string", ["flatten_destination_structure"] = "boolean", ["ignore_locked_folders"] = "boolean", ["legacy_folder_matching"] = "boolean", ["name"] = "string", ["overwrite_files"] = "boolean", ["path_time_zone"] = "string", ["retry_on_failure_interval_in_minutes"] = "number", ["retry_on_failure_number_of_attempts"] = "number", ["trigger"] = "string", ["trigger_actions"] = "array", ["recurring_day"] = "number", ["recurring_days"] = "array", ["automation"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Automation.create", "source", "destinations", "destination_replace_from", "destination_replace_to", "interval", "path", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region", "always_overwrite_size_matching_files", "always_serialize_jobs", "description", "disabled", "exclude_pattern", "flatten_destination_structure", "ignore_locked_folders", "legacy_folder_matching", "name", "overwrite_files", "path_time_zone", "retry_on_failure_interval_in_minutes", "retry_on_failure_number_of_attempts", "trigger", "trigger_actions", "recurring_day", "recurring_days", "automation", "workspace_id");
            AssertModel(record, created, "Automation.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Automation", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Automation[] listed = (await Simulator.Sdk(operation).Automations.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Automation.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Automation", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Automation found = await Simulator.Sdk(operation).Automations.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Automation.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Automation", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["source"] = "contract-source-2";
            sent["destinations"] = new string[] { "b" };
            sent["destination_replace_from"] = "contract-destination_replace_from-2";
            sent["destination_replace_to"] = "contract-destination_replace_to-2";
            sent["interval"] = "contract-interval-2";
            sent["path"] = "contract-path-2";
            sent["schedule_id"] = (Int64)8;
            sent["schedule_days_of_week"] = new Nullable<Int64>[] { 4 };
            sent["schedule_times_of_day"] = new string[] { "b" };
            sent["schedule_time_zone"] = "contract-schedule_time_zone-2";
            sent["holiday_region"] = "contract-holiday_region-2";
            sent["always_overwrite_size_matching_files"] = false;
            sent["always_serialize_jobs"] = false;
            sent["description"] = "contract-description-2";
            sent["disabled"] = false;
            sent["exclude_pattern"] = "contract-exclude_pattern-2";
            sent["flatten_destination_structure"] = false;
            sent["ignore_locked_folders"] = false;
            sent["legacy_folder_matching"] = false;
            sent["name"] = "contract-name-2";
            sent["overwrite_files"] = false;
            sent["path_time_zone"] = "contract-path_time_zone-2";
            sent["retry_on_failure_interval_in_minutes"] = (Int64)8;
            sent["retry_on_failure_number_of_attempts"] = (Int64)8;
            sent["trigger"] = "daily";
            sent["trigger_actions"] = new string[] { "b" };
            sent["recurring_day"] = (Int64)8;
            sent["recurring_days"] = new Nullable<Int64>[] { 4 };
            sent["automation"] = "delete_file";
            Automation updated = await Simulator.Sdk(operation).Automations.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["source"] = "string", ["destinations"] = "array", ["destination_replace_from"] = "string", ["destination_replace_to"] = "string", ["interval"] = "string", ["path"] = "string", ["schedule_id"] = "number", ["schedule_days_of_week"] = "array", ["schedule_times_of_day"] = "array", ["schedule_time_zone"] = "string", ["holiday_region"] = "string", ["always_overwrite_size_matching_files"] = "boolean", ["always_serialize_jobs"] = "boolean", ["description"] = "string", ["disabled"] = "boolean", ["exclude_pattern"] = "string", ["flatten_destination_structure"] = "boolean", ["ignore_locked_folders"] = "boolean", ["legacy_folder_matching"] = "boolean", ["name"] = "string", ["overwrite_files"] = "boolean", ["path_time_zone"] = "string", ["retry_on_failure_interval_in_minutes"] = "number", ["retry_on_failure_number_of_attempts"] = "number", ["trigger"] = "string", ["trigger_actions"] = "array", ["recurring_day"] = "number", ["recurring_days"] = "array", ["automation"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Automation.update", "source", "destinations", "destination_replace_from", "destination_replace_to", "interval", "path", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region", "always_overwrite_size_matching_files", "always_serialize_jobs", "description", "disabled", "exclude_pattern", "flatten_destination_structure", "ignore_locked_folders", "legacy_folder_matching", "name", "overwrite_files", "path_time_zone", "retry_on_failure_interval_in_minutes", "retry_on_failure_number_of_attempts", "trigger", "trigger_actions", "recurring_day", "recurring_days", "automation");
            AssertModel(now, updated, "Automation.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Automation", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Automations.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Automation.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}