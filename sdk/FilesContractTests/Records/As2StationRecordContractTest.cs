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

    // The record contract of As2Station, generated from the schema: each case sends one request through the
    // SDK's As2Stations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class As2StationRecordContractTest : ContractCase
    {
        private const string ListPath = "/as2_stations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("As2Station", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["workspace_id"] = (Int64)7, ["public_certificate"] = "contract-public_certificate" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "As2Station", ("id", "int64"), ("workspace_id", "int64"), ("name", "string"), ("uri", "string"), ("domain", "string"), ("hex_public_certificate_serial", "string"), ("public_certificate_md5", "string"), ("public_certificate", "string"), ("private_key_md5", "string"), ("public_certificate_subject", "string"), ("public_certificate_issuer", "string"), ("public_certificate_serial", "string"), ("public_certificate_not_before", "string"), ("public_certificate_not_after", "string"), ("private_key_password_md5", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the As2Station the SDK returns gives it.
        private static void AssertModel(JsonObject record, As2Station model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no As2Station");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "uri", model.Uri, what);
            Simulator.Same(record, "domain", model.Domain, what);
            Simulator.Same(record, "hex_public_certificate_serial", model.HexPublicCertificateSerial, what);
            Simulator.Same(record, "public_certificate_md5", model.PublicCertificateMd5, what);
            Simulator.Same(record, "public_certificate", model.PublicCertificate, what);
            Simulator.Same(record, "private_key_md5", model.PrivateKeyMd5, what);
            Simulator.Same(record, "public_certificate_subject", model.PublicCertificateSubject, what);
            Simulator.Same(record, "public_certificate_issuer", model.PublicCertificateIssuer, what);
            Simulator.Same(record, "public_certificate_serial", model.PublicCertificateSerial, what);
            Simulator.Same(record, "public_certificate_not_before", model.PublicCertificateNotBefore, what);
            Simulator.Same(record, "public_certificate_not_after", model.PublicCertificateNotAfter, what);
            Simulator.Same(record, "private_key_password_md5", model.PrivateKeyPasswordMd5, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("As2Station", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["workspace_id"] = (Int64)7, ["public_certificate"] = "contract-public_certificate" };
            As2Station created = await Simulator.Sdk(operation).As2Stations.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["workspace_id"] = "number", ["public_certificate"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "As2Station.create", "name", "workspace_id", "public_certificate");
            AssertModel(record, created, "As2Station.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("As2Station", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            As2Station[] listed = (await Simulator.Sdk(operation).As2Stations.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"As2Station.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("As2Station", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            As2Station found = await Simulator.Sdk(operation).As2Stations.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "As2Station.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("As2Station", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["public_certificate"] = "contract-public_certificate-2";
            As2Station updated = await Simulator.Sdk(operation).As2Stations.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["public_certificate"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "As2Station.update", "name", "public_certificate");
            AssertModel(now, updated, "As2Station.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("As2Station", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).As2Stations.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"As2Station.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}