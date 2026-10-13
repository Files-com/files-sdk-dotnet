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

    // The record contract of As2Partner, generated from the schema: each case sends one request through the
    // SDK's As2Partners operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class As2PartnerRecordContractTest : ContractCase
    {
        private const string ListPath = "/as2_partners";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("As2Partner", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["enable_dedicated_ips"] = true, ["http_auth_username"] = "contract-http_auth_username", ["mdn_validation_level"] = "none", ["signature_validation_level"] = "normal", ["server_certificate"] = "require_match", ["default_mime_type"] = "contract-default_mime_type", ["as2_station_id"] = (Int64)7, ["name"] = "contract-name", ["uri"] = "contract-uri", ["public_certificate"] = "contract-public_certificate" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "As2Partner", ("id", "int64"), ("workspace_id", "int64"), ("as2_station_id", "int64"), ("name", "string"), ("uri", "string"), ("server_certificate", "string"), ("http_auth_username", "string"), ("default_mime_type", "string"), ("mdn_validation_level", "string"), ("signature_validation_level", "string"), ("enable_dedicated_ips", "boolean"), ("hex_public_certificate_serial", "string"), ("public_certificate", "string"), ("public_certificate_md5", "string"), ("public_certificate_subject", "string"), ("public_certificate_issuer", "string"), ("public_certificate_serial", "string"), ("public_certificate_not_before", "string"), ("public_certificate_not_after", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the As2Partner the SDK returns gives it.
        private static void AssertModel(JsonObject record, As2Partner model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no As2Partner");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "as2_station_id", model.As2StationId, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "uri", model.Uri, what);
            Simulator.Same(record, "server_certificate", model.ServerCertificate, what);
            Simulator.Same(record, "http_auth_username", model.HttpAuthUsername, what);
            Simulator.Same(record, "default_mime_type", model.DefaultMimeType, what);
            Simulator.Same(record, "mdn_validation_level", model.MdnValidationLevel, what);
            Simulator.Same(record, "signature_validation_level", model.SignatureValidationLevel, what);
            Simulator.Same(record, "enable_dedicated_ips", model.EnableDedicatedIps, what);
            Simulator.Same(record, "hex_public_certificate_serial", model.HexPublicCertificateSerial, what);
            Simulator.Same(record, "public_certificate", model.PublicCertificate, what);
            Simulator.Same(record, "public_certificate_md5", model.PublicCertificateMd5, what);
            Simulator.Same(record, "public_certificate_subject", model.PublicCertificateSubject, what);
            Simulator.Same(record, "public_certificate_issuer", model.PublicCertificateIssuer, what);
            Simulator.Same(record, "public_certificate_serial", model.PublicCertificateSerial, what);
            Simulator.Same(record, "public_certificate_not_before", model.PublicCertificateNotBefore, what);
            Simulator.Same(record, "public_certificate_not_after", model.PublicCertificateNotAfter, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("As2Partner", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["enable_dedicated_ips"] = true, ["http_auth_username"] = "contract-http_auth_username", ["mdn_validation_level"] = "none", ["signature_validation_level"] = "normal", ["server_certificate"] = "require_match", ["default_mime_type"] = "contract-default_mime_type", ["as2_station_id"] = (Int64)7, ["name"] = "contract-name", ["uri"] = "contract-uri", ["public_certificate"] = "contract-public_certificate" };
            As2Partner created = await Simulator.Sdk(operation).As2Partners.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["enable_dedicated_ips"] = "boolean", ["http_auth_username"] = "string", ["mdn_validation_level"] = "string", ["signature_validation_level"] = "string", ["server_certificate"] = "string", ["default_mime_type"] = "string", ["as2_station_id"] = "number", ["name"] = "string", ["uri"] = "string", ["public_certificate"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "As2Partner.create", "enable_dedicated_ips", "http_auth_username", "mdn_validation_level", "signature_validation_level", "server_certificate", "default_mime_type", "as2_station_id", "name", "uri", "public_certificate");
            AssertModel(record, created, "As2Partner.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("As2Partner", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            As2Partner[] listed = (await Simulator.Sdk(operation).As2Partners.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"As2Partner.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("As2Partner", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            As2Partner found = await Simulator.Sdk(operation).As2Partners.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "As2Partner.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("As2Partner", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["enable_dedicated_ips"] = false;
            sent["http_auth_username"] = "contract-http_auth_username-2";
            sent["mdn_validation_level"] = "weak";
            sent["signature_validation_level"] = "none";
            sent["server_certificate"] = "allow_any";
            sent["default_mime_type"] = "contract-default_mime_type-2";
            sent["name"] = "contract-name-2";
            sent["uri"] = "contract-uri-2";
            sent["public_certificate"] = "contract-public_certificate-2";
            As2Partner updated = await Simulator.Sdk(operation).As2Partners.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["enable_dedicated_ips"] = "boolean", ["http_auth_username"] = "string", ["mdn_validation_level"] = "string", ["signature_validation_level"] = "string", ["server_certificate"] = "string", ["default_mime_type"] = "string", ["name"] = "string", ["uri"] = "string", ["public_certificate"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "As2Partner.update", "enable_dedicated_ips", "http_auth_username", "mdn_validation_level", "signature_validation_level", "server_certificate", "default_mime_type", "name", "uri", "public_certificate");
            AssertModel(now, updated, "As2Partner.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("As2Partner", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).As2Partners.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"As2Partner.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}