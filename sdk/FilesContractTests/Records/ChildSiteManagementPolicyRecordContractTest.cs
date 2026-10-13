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

    // The record contract of ChildSiteManagementPolicy, generated from the schema: each case sends one request through the
    // SDK's ChildSiteManagementPolicies operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ChildSiteManagementPolicyRecordContractTest : ContractCase
    {
        private const string ListPath = "/child_site_management_policies";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("ChildSiteManagementPolicy", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["skip_child_site_ids"] = new Nullable<Int64>[] { 3 }, ["child_site_ids"] = new Nullable<Int64>[] { 3 }, ["default_policy"] = true, ["policy_type"] = "settings", ["name"] = "contract-name", ["description"] = "contract-description" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ChildSiteManagementPolicy", ("id", "int64"), ("policy_type", "string"), ("name", "string"), ("description", "string"), ("applied_child_site_ids", "array(int64)"), ("skip_child_site_ids", "array(int64)"), ("child_site_ids", "array(int64)"), ("default_policy", "boolean"), ("created_at", "date-time"), ("updated_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the ChildSiteManagementPolicy the SDK returns gives it.
        private static void AssertModel(JsonObject record, ChildSiteManagementPolicy model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ChildSiteManagementPolicy");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "policy_type", model.PolicyType, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "applied_child_site_ids", model.AppliedChildSiteIds, what);
            Simulator.Same(record, "skip_child_site_ids", model.SkipChildSiteIds, what);
            Simulator.Same(record, "child_site_ids", model.ChildSiteIds, what);
            Simulator.Same(record, "default_policy", model.DefaultPolicy, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("ChildSiteManagementPolicy", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["skip_child_site_ids"] = new Nullable<Int64>[] { 3 }, ["child_site_ids"] = new Nullable<Int64>[] { 3 }, ["default_policy"] = true, ["policy_type"] = "settings", ["name"] = "contract-name", ["description"] = "contract-description" };
            ChildSiteManagementPolicy created = await Simulator.Sdk(operation).ChildSiteManagementPolicies.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["skip_child_site_ids"] = "array", ["child_site_ids"] = "array", ["default_policy"] = "boolean", ["policy_type"] = "string", ["name"] = "string", ["description"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "ChildSiteManagementPolicy.create", "skip_child_site_ids", "child_site_ids", "default_policy", "policy_type", "name", "description");
            AssertModel(record, created, "ChildSiteManagementPolicy.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ChildSiteManagementPolicy", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ChildSiteManagementPolicy[] listed = (await Simulator.Sdk(operation).ChildSiteManagementPolicies.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ChildSiteManagementPolicy.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("ChildSiteManagementPolicy", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ChildSiteManagementPolicy found = await Simulator.Sdk(operation).ChildSiteManagementPolicies.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "ChildSiteManagementPolicy.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("ChildSiteManagementPolicy", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["skip_child_site_ids"] = new Nullable<Int64>[] { 4 };
            sent["child_site_ids"] = new Nullable<Int64>[] { 4 };
            sent["default_policy"] = false;
            sent["policy_type"] = "settings";
            sent["name"] = "contract-name-2";
            sent["description"] = "contract-description-2";
            ChildSiteManagementPolicy updated = await Simulator.Sdk(operation).ChildSiteManagementPolicies.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["skip_child_site_ids"] = "array", ["child_site_ids"] = "array", ["default_policy"] = "boolean", ["policy_type"] = "string", ["name"] = "string", ["description"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "ChildSiteManagementPolicy.update", "skip_child_site_ids", "child_site_ids", "default_policy", "policy_type", "name", "description");
            AssertModel(now, updated, "ChildSiteManagementPolicy.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("ChildSiteManagementPolicy", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).ChildSiteManagementPolicies.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"ChildSiteManagementPolicy.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}