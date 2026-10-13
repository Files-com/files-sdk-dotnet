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

    // The record contract of HolidayCalendar, generated from the schema: each case sends one request through the
    // SDK's HolidayCalendars operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class HolidayCalendarRecordContractTest : ContractCase
    {
        private const string ListPath = "/holiday_calendars";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("HolidayCalendar", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["definition"] = new Dictionary<string, object> { ["k"] = "v" }, ["name"] = "contract-name" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "HolidayCalendar", ("id", "int64"), ("name", "string"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the HolidayCalendar the SDK returns gives it.
        private static void AssertModel(JsonObject record, HolidayCalendar model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no HolidayCalendar");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("HolidayCalendar", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["definition"] = new Dictionary<string, object> { ["k"] = "v" }, ["name"] = "contract-name" };
            HolidayCalendar created = await Simulator.Sdk(operation).HolidayCalendars.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["definition"] = "object", ["name"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "HolidayCalendar.create", "definition", "name");
            AssertModel(record, created, "HolidayCalendar.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("HolidayCalendar", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            HolidayCalendar[] listed = (await Simulator.Sdk(operation).HolidayCalendars.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"HolidayCalendar.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("HolidayCalendar", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            HolidayCalendar found = await Simulator.Sdk(operation).HolidayCalendars.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "HolidayCalendar.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("HolidayCalendar", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            HolidayCalendar updated = await Simulator.Sdk(operation).HolidayCalendars.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "HolidayCalendar.update", "name");
            AssertModel(now, updated, "HolidayCalendar.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("HolidayCalendar", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).HolidayCalendars.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"HolidayCalendar.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}