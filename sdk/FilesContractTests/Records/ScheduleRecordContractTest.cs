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

    // The record contract of Schedule, generated from the schema: each case sends one request through the
    // SDK's Schedules operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ScheduleRecordContractTest : ContractCase
    {
        private const string ListPath = "/schedules";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Schedule", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_times_of_day"] = new string[] { "a" }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["holiday_region"] = "contract-holiday_region" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Schedule", ("id", "int64"), ("name", "string"), ("schedule_days_of_week", "array(int64)"), ("schedule_times_of_day", "array(string)"), ("schedule_time_zone", "string"), ("holiday_region", "string"), ("human_readable_schedule", "string"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Schedule the SDK returns gives it.
        private static void AssertModel(JsonObject record, Schedule model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Schedule");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "schedule_days_of_week", model.ScheduleDaysOfWeek, what);
            Simulator.Same(record, "schedule_times_of_day", model.ScheduleTimesOfDay, what);
            Simulator.Same(record, "schedule_time_zone", model.ScheduleTimeZone, what);
            Simulator.Same(record, "holiday_region", model.HolidayRegion, what);
            Simulator.Same(record, "human_readable_schedule", model.HumanReadableSchedule, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Schedule", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["schedule_days_of_week"] = new Nullable<Int64>[] { 3 }, ["schedule_times_of_day"] = new string[] { "a" }, ["schedule_time_zone"] = "contract-schedule_time_zone", ["holiday_region"] = "contract-holiday_region" };
            Schedule created = await Simulator.Sdk(operation).Schedules.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["schedule_days_of_week"] = "array", ["schedule_times_of_day"] = "array", ["schedule_time_zone"] = "string", ["holiday_region"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Schedule.create", "name", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region");
            AssertModel(record, created, "Schedule.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Schedule", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Schedule[] listed = (await Simulator.Sdk(operation).Schedules.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Schedule.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Schedule", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Schedule found = await Simulator.Sdk(operation).Schedules.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Schedule.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Schedule", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["schedule_days_of_week"] = new Nullable<Int64>[] { 4 };
            sent["schedule_times_of_day"] = new string[] { "b" };
            sent["schedule_time_zone"] = "contract-schedule_time_zone-2";
            sent["holiday_region"] = "contract-holiday_region-2";
            Schedule updated = await Simulator.Sdk(operation).Schedules.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["schedule_days_of_week"] = "array", ["schedule_times_of_day"] = "array", ["schedule_time_zone"] = "string", ["holiday_region"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Schedule.update", "name", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region");
            AssertModel(now, updated, "Schedule.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Schedule", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Schedules.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Schedule.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}