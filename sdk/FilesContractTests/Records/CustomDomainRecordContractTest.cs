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

    // The record contract of CustomDomain, generated from the schema: each case sends one request through the
    // SDK's CustomDomains operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class CustomDomainRecordContractTest : ContractCase
    {
        private const string ListPath = "/custom_domains";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("CustomDomain", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["available_to_all_workspaces"] = true, ["workspace_id"] = (Int64)7, ["destination"] = "site_alias", ["folder_behavior_id"] = (Int64)7, ["ssl_certificate_id"] = (Int64)7, ["domain"] = "contract-domain" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "CustomDomain", ("id", "int64"), ("workspace_id", "int64"), ("available_to_all_workspaces", "boolean"), ("outbound_ip_addresses", "array(string)"), ("domain", "string"), ("destination", "string"), ("dns_status", "string"), ("ssl_certificate_id", "int64"), ("brick_managed", "boolean"), ("folder_behavior_id", "int64"), ("ip_addresses", "array(string)"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the CustomDomain the SDK returns gives it.
        private static void AssertModel(JsonObject record, CustomDomain model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no CustomDomain");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "available_to_all_workspaces", model.AvailableToAllWorkspaces, what);
            Simulator.Same(record, "outbound_ip_addresses", model.OutboundIpAddresses, what);
            Simulator.Same(record, "domain", model.Domain, what);
            Simulator.Same(record, "destination", model.Destination, what);
            Simulator.Same(record, "dns_status", model.DnsStatus, what);
            Simulator.Same(record, "ssl_certificate_id", model.SslCertificateId, what);
            Simulator.Same(record, "brick_managed", model.BrickManaged, what);
            Simulator.Same(record, "folder_behavior_id", model.FolderBehaviorId, what);
            Simulator.Same(record, "ip_addresses", model.IpAddresses, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("CustomDomain", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["available_to_all_workspaces"] = true, ["workspace_id"] = (Int64)7, ["destination"] = "site_alias", ["folder_behavior_id"] = (Int64)7, ["ssl_certificate_id"] = (Int64)7, ["domain"] = "contract-domain" };
            CustomDomain created = await Simulator.Sdk(operation).CustomDomains.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["available_to_all_workspaces"] = "boolean", ["workspace_id"] = "number", ["destination"] = "string", ["folder_behavior_id"] = "number", ["ssl_certificate_id"] = "number", ["domain"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "CustomDomain.create", "available_to_all_workspaces", "workspace_id", "destination", "folder_behavior_id", "ssl_certificate_id", "domain");
            AssertModel(record, created, "CustomDomain.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("CustomDomain", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            CustomDomain[] listed = (await Simulator.Sdk(operation).CustomDomains.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"CustomDomain.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("CustomDomain", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            CustomDomain found = await Simulator.Sdk(operation).CustomDomains.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "CustomDomain.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("CustomDomain", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["available_to_all_workspaces"] = false;
            sent["workspace_id"] = (Int64)8;
            sent["destination"] = "public_hosting";
            sent["folder_behavior_id"] = (Int64)8;
            sent["ssl_certificate_id"] = (Int64)8;
            sent["domain"] = "contract-domain-2";
            CustomDomain updated = await Simulator.Sdk(operation).CustomDomains.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["available_to_all_workspaces"] = "boolean", ["workspace_id"] = "number", ["destination"] = "string", ["folder_behavior_id"] = "number", ["ssl_certificate_id"] = "number", ["domain"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "CustomDomain.update", "available_to_all_workspaces", "workspace_id", "destination", "folder_behavior_id", "ssl_certificate_id", "domain");
            AssertModel(now, updated, "CustomDomain.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("CustomDomain", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).CustomDomains.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"CustomDomain.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}