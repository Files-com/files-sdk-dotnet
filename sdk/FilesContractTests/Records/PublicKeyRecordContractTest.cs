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

    // The record contract of PublicKey, generated from the schema: each case sends one request through the
    // SDK's PublicKeys operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class PublicKeyRecordContractTest : ContractCase
    {
        private const string ListPath = "/public_keys";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("PublicKey", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["user_id"] = (Int64)7, ["title"] = "contract-title" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "PublicKey", ("id", "int64"), ("workspace_id", "int64"), ("title", "string"), ("created_at", "date-time"), ("expires_at", "date-time"), ("expired", "boolean"), ("fingerprint", "string"), ("fingerprint_sha256", "string"), ("status", "string"), ("last_login_at", "date-time"), ("generated_private_key", "string"), ("generated_public_key", "string"), ("username", "string"), ("user_id", "int64"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the PublicKey the SDK returns gives it.
        private static void AssertModel(JsonObject record, PublicKey model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no PublicKey");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "title", model.Title, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "expires_at", model.ExpiresAt, what);
            Simulator.Same(record, "expired", model.Expired, what);
            Simulator.Same(record, "fingerprint", model.Fingerprint, what);
            Simulator.Same(record, "fingerprint_sha256", model.FingerprintSha256, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "last_login_at", model.LastLoginAt, what);
            Simulator.Same(record, "generated_private_key", model.GeneratedPrivateKey, what);
            Simulator.Same(record, "generated_public_key", model.GeneratedPublicKey, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "user_id", model.UserId, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("PublicKey", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["user_id"] = (Int64)7, ["title"] = "contract-title" };
            PublicKey created = await Simulator.Sdk(operation).PublicKeys.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["user_id"] = "number", ["title"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "PublicKey.create", "user_id", "title");
            AssertModel(record, created, "PublicKey.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("PublicKey", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            PublicKey[] listed = (await Simulator.Sdk(operation).PublicKeys.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"PublicKey.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("PublicKey", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            PublicKey found = await Simulator.Sdk(operation).PublicKeys.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "PublicKey.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("PublicKey", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { ["title"] = "contract-title" };
            PublicKey updated = await Simulator.Sdk(operation).PublicKeys.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["title"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "PublicKey.update", "title");
            AssertModel(now, updated, "PublicKey.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("PublicKey", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).PublicKeys.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"PublicKey.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}