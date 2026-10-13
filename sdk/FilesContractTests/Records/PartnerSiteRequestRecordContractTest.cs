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

    // The record contract of PartnerSiteRequest, generated from the schema: each case sends one request through the
    // SDK's PartnerSiteRequests operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class PartnerSiteRequestRecordContractTest : ContractCase
    {
        private const string ListPath = "/partner_site_requests";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("PartnerSiteRequest", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["host_partner_id"] = (Int64)7, ["guest_site_url"] = "contract-guest_site_url" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "PartnerSiteRequest", ("id", "int64"), ("host_partner_id", "int64"), ("guest_site_url", "string"), ("status", "string"), ("host_site_name", "string"), ("pairing_key", "string"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the PartnerSiteRequest the SDK returns gives it.
        private static void AssertModel(JsonObject record, PartnerSiteRequest model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no PartnerSiteRequest");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "host_partner_id", model.HostPartnerId, what);
            Simulator.Same(record, "guest_site_url", model.GuestSiteUrl, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "host_site_name", model.HostSiteName, what);
            Simulator.Same(record, "pairing_key", model.PairingKey, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("PartnerSiteRequest", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["host_partner_id"] = (Int64)7, ["guest_site_url"] = "contract-guest_site_url" };
            PartnerSiteRequest created = await Simulator.Sdk(operation).PartnerSiteRequests.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["host_partner_id"] = "number", ["guest_site_url"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, null, Scope(), candidate => true);
            await Simulator.KeptAsync(sent, record, "PartnerSiteRequest.create", "host_partner_id", "guest_site_url");
            AssertModel(record, created, "PartnerSiteRequest.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("PartnerSiteRequest", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            PartnerSiteRequest[] listed = (await Simulator.Sdk(operation).PartnerSiteRequests.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"PartnerSiteRequest.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("PartnerSiteRequest", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).PartnerSiteRequests.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            Assert.AreEqual(0, (await Simulator.ListRawAsync(ListPath, Scope())).Length, "PartnerSiteRequest.delete: the record is gone");
        }
    }
}