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

    // The record contract of GpgKey, generated from the schema: each case sends one request through the
    // SDK's GpgKeys operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class GpgKeyRecordContractTest : ContractCase
    {
        private const string ListPath = "/gpg_keys";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("GpgKey", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["user_id"] = (Int64)7, ["partner_id"] = (Int64)7, ["name"] = "contract-name", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "GpgKey", ("id", "int64"), ("workspace_id", "int64"), ("expires_at", "date-time"), ("name", "string"), ("partner_id", "int64"), ("partner_name", "string"), ("user_id", "int64"), ("public_key_md5", "string"), ("private_key_md5", "string"), ("generated_public_key", "string"), ("generated_private_key", "string"), ("private_key_password_md5", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the GpgKey the SDK returns gives it.
        private static void AssertModel(JsonObject record, GpgKey model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no GpgKey");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "expires_at", model.ExpiresAt, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "partner_id", model.PartnerId, what);
            Simulator.Same(record, "partner_name", model.PartnerName, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "public_key_md5", model.PublicKeyMd5, what);
            Simulator.Same(record, "private_key_md5", model.PrivateKeyMd5, what);
            Simulator.Same(record, "generated_public_key", model.GeneratedPublicKey, what);
            Simulator.Same(record, "generated_private_key", model.GeneratedPrivateKey, what);
            Simulator.Same(record, "private_key_password_md5", model.PrivateKeyPasswordMd5, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("GpgKey", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["user_id"] = (Int64)7, ["partner_id"] = (Int64)7, ["name"] = "contract-name", ["workspace_id"] = (Int64)7 };
            GpgKey created = await Simulator.Sdk(operation).GpgKeys.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["user_id"] = "number", ["partner_id"] = "number", ["name"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "GpgKey.create", "user_id", "partner_id", "name", "workspace_id");
            AssertModel(record, created, "GpgKey.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("GpgKey", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            GpgKey[] listed = (await Simulator.Sdk(operation).GpgKeys.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"GpgKey.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("GpgKey", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            GpgKey found = await Simulator.Sdk(operation).GpgKeys.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "GpgKey.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("GpgKey", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["partner_id"] = (Int64)8;
            sent["name"] = "contract-name-2";
            GpgKey updated = await Simulator.Sdk(operation).GpgKeys.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["partner_id"] = "number", ["name"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "GpgKey.update", "partner_id", "name");
            AssertModel(now, updated, "GpgKey.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("GpgKey", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).GpgKeys.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"GpgKey.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}