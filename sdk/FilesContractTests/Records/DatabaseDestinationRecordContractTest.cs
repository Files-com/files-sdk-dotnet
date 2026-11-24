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

    // The record contract of DatabaseDestination, generated from the schema: each case sends one request through the
    // SDK's DatabaseDestinations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class DatabaseDestinationRecordContractTest : ContractCase
    {
        private const string ListPath = "/database_destinations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("DatabaseDestination", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["provider"] = "snowflake", ["config"] = new Dictionary<string, object> { ["k"] = "v" }, ["secret_id"] = (Int64)7, ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "DatabaseDestination", ("id", "int64"), ("workspace_id", "int64"), ("name", "string"), ("provider", "string"), ("secret_id", "int64"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the DatabaseDestination the SDK returns gives it.
        private static void AssertModel(JsonObject record, DatabaseDestination model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no DatabaseDestination");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "provider", model.Provider, what);
            Simulator.Same(record, "secret_id", model.SecretId, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("DatabaseDestination", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["provider"] = "snowflake", ["config"] = new Dictionary<string, object> { ["k"] = "v" }, ["secret_id"] = (Int64)7, ["workspace_id"] = (Int64)7 };
            DatabaseDestination created = await Simulator.Sdk(operation).DatabaseDestinations.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["provider"] = "string", ["config"] = "object", ["secret_id"] = "number", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "DatabaseDestination.create", "name", "provider", "config", "secret_id", "workspace_id");
            AssertModel(record, created, "DatabaseDestination.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("DatabaseDestination", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            DatabaseDestination[] listed = (await Simulator.Sdk(operation).DatabaseDestinations.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"DatabaseDestination.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("DatabaseDestination", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            DatabaseDestination found = await Simulator.Sdk(operation).DatabaseDestinations.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "DatabaseDestination.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("DatabaseDestination", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["secret_id"] = (Int64)8;
            DatabaseDestination updated = await Simulator.Sdk(operation).DatabaseDestinations.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["secret_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "DatabaseDestination.update", "name", "secret_id");
            AssertModel(now, updated, "DatabaseDestination.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("DatabaseDestination", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).DatabaseDestinations.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"DatabaseDestination.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}