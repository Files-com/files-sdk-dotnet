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

    // The record contract of Certificate, generated from the schema: each case sends one request through the
    // SDK's Certificates operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class CertificateRecordContractTest : ContractCase
    {
        private const string ListPath = "/certificates";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Certificate", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["key_type"] = "contract-key_type", ["certificate"] = "contract-certificate", ["intermediates"] = "contract-intermediates" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Certificate", ("id", "int64"), ("name", "string"), ("custom_domain_id", "int64"), ("certificate", "string"), ("created_at", "date-time"), ("display_status", "string"), ("domains", "array(string)"), ("expires_at", "date-time"), ("brick_managed", "boolean"), ("intermediates", "string"), ("ip_addresses", "array(string)"), ("issuer", "string"), ("key_type", "string"), ("request", "string"), ("status", "string"), ("subject", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Certificate the SDK returns gives it.
        private static void AssertModel(JsonObject record, Certificate model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Certificate");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "custom_domain_id", model.CustomDomainId, what);
            Simulator.Same(record, "certificate", model.CertificateType, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "display_status", model.DisplayStatus, what);
            Simulator.Same(record, "domains", model.Domains, what);
            Simulator.Same(record, "expires_at", model.ExpiresAt, what);
            Simulator.Same(record, "brick_managed", model.BrickManaged, what);
            Simulator.Same(record, "intermediates", model.Intermediates, what);
            Simulator.Same(record, "ip_addresses", model.IpAddresses, what);
            Simulator.Same(record, "issuer", model.Issuer, what);
            Simulator.Same(record, "key_type", model.KeyType, what);
            Simulator.Same(record, "request", model.Request, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "subject", model.Subject, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Certificate", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["key_type"] = "contract-key_type", ["certificate"] = "contract-certificate", ["intermediates"] = "contract-intermediates" };
            Certificate created = await Simulator.Sdk(operation).Certificates.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["key_type"] = "string", ["certificate"] = "string", ["intermediates"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Certificate.create", "name", "key_type", "certificate", "intermediates");
            AssertModel(record, created, "Certificate.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Certificate", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Certificate[] listed = (await Simulator.Sdk(operation).Certificates.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Certificate.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Certificate", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Certificate found = await Simulator.Sdk(operation).Certificates.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Certificate.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Certificate", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["intermediates"] = "contract-intermediates-2";
            sent["certificate"] = "contract-certificate-2";
            Certificate updated = await Simulator.Sdk(operation).Certificates.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["intermediates"] = "string", ["certificate"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Certificate.update", "name", "intermediates", "certificate");
            AssertModel(now, updated, "Certificate.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Certificate", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Certificates.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Certificate.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}