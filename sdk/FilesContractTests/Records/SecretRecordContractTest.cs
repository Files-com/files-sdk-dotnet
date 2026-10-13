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

    // The record contract of Secret, generated from the schema: each case sends one request through the
    // SDK's Secrets operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SecretRecordContractTest : ContractCase
    {
        private const string ListPath = "/secrets";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Secret", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["description"] = "contract-description", ["secret_type"] = "basic", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Secret", ("id", "int64"), ("workspace_id", "int64"), ("name", "string"), ("description", "string"), ("secret_type", "string"), ("value_field_names", "array(string)"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Secret the SDK returns gives it.
        private static void AssertModel(JsonObject record, Secret model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Secret");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "secret_type", model.SecretType, what);
            Simulator.Same(record, "value_field_names", model.ValueFieldNames, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Secret", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["description"] = "contract-description", ["secret_type"] = "basic", ["workspace_id"] = (Int64)7 };
            Secret created = await Simulator.Sdk(operation).Secrets.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["description"] = "string", ["secret_type"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Secret.create", "name", "description", "secret_type", "workspace_id");
            AssertModel(record, created, "Secret.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Secret", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Secret[] listed = (await Simulator.Sdk(operation).Secrets.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Secret.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Secret", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Secret found = await Simulator.Sdk(operation).Secrets.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Secret.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Secret", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["description"] = "contract-description-2";
            sent["secret_type"] = "token";
            Secret updated = await Simulator.Sdk(operation).Secrets.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["description"] = "string", ["secret_type"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Secret.update", "name", "description", "secret_type");
            AssertModel(now, updated, "Secret.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Secret", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Secrets.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Secret.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}