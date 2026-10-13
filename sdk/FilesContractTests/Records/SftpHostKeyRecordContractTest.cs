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

    // The record contract of SftpHostKey, generated from the schema: each case sends one request through the
    // SDK's SftpHostKeys operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SftpHostKeyRecordContractTest : ContractCase
    {
        private const string ListPath = "/sftp_host_keys";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("SftpHostKey", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["active"] = true, ["custom_domain_id"] = (Int64)7, ["name"] = "contract-name" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "SftpHostKey", ("active", "boolean"), ("custom_domain_id", "int64"), ("id", "int64"), ("name", "string"), ("key_type", "string"), ("fingerprint_md5", "string"), ("fingerprint_sha256", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the SftpHostKey the SDK returns gives it.
        private static void AssertModel(JsonObject record, SftpHostKey model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no SftpHostKey");
            Simulator.Same(record, "active", model.Active, what);
            Simulator.Same(record, "custom_domain_id", model.CustomDomainId, what);
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "key_type", model.KeyType, what);
            Simulator.Same(record, "fingerprint_md5", model.FingerprintMd5, what);
            Simulator.Same(record, "fingerprint_sha256", model.FingerprintSha256, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("SftpHostKey", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["active"] = true, ["custom_domain_id"] = (Int64)7, ["name"] = "contract-name" };
            SftpHostKey created = await Simulator.Sdk(operation).SftpHostKeys.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["active"] = "boolean", ["custom_domain_id"] = "number", ["name"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "SftpHostKey.create", "active", "custom_domain_id", "name");
            AssertModel(record, created, "SftpHostKey.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("SftpHostKey", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SftpHostKey[] listed = (await Simulator.Sdk(operation).SftpHostKeys.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"SftpHostKey.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("SftpHostKey", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SftpHostKey found = await Simulator.Sdk(operation).SftpHostKeys.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "SftpHostKey.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("SftpHostKey", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["active"] = false;
            sent["custom_domain_id"] = (Int64)8;
            sent["name"] = "contract-name-2";
            SftpHostKey updated = await Simulator.Sdk(operation).SftpHostKeys.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["active"] = "boolean", ["custom_domain_id"] = "number", ["name"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "SftpHostKey.update", "active", "custom_domain_id", "name");
            AssertModel(now, updated, "SftpHostKey.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("SftpHostKey", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).SftpHostKeys.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"SftpHostKey.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}