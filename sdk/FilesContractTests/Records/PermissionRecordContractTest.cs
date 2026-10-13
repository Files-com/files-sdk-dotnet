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

    // The record contract of Permission, generated from the schema: each case sends one request through the
    // SDK's Permissions operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class PermissionRecordContractTest : ContractCase
    {
        private const string ListPath = "/permissions";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Permission", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["path"] = "contract-path", ["group_id"] = (Int64)7, ["permission"] = "contract-permission", ["recursive"] = true, ["partner_id"] = (Int64)7, ["user_id"] = (Int64)7, ["username"] = "contract-username", ["group_name"] = "contract-group_name", ["site_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Permission", ("id", "int64"), ("path", "string"), ("user_id", "int64"), ("username", "string"), ("group_id", "int64"), ("group_name", "string"), ("group_ids", "array(int64)"), ("group_names", "array(string)"), ("partner_id", "int64"), ("partner_name", "string"), ("permission", "string"), ("recursive", "boolean"), ("site_id", "int64"));
            }
            return records;
        }

        // Every property the record holds, as the Permission the SDK returns gives it.
        private static void AssertModel(JsonObject record, Permission model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Permission");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "group_id", model.GroupId, what);
            Simulator.Same(record, "group_name", model.GroupName, what);
            Simulator.Same(record, "group_ids", model.GroupIds, what);
            Simulator.Same(record, "group_names", model.GroupNames, what);
            Simulator.Same(record, "partner_id", model.PartnerId, what);
            Simulator.Same(record, "partner_name", model.PartnerName, what);
            Simulator.Same(record, "permission", model.PermissionType, what);
            Simulator.Same(record, "recursive", model.Recursive, what);
            Simulator.Same(record, "site_id", model.SiteId, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Permission", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["path"] = "contract-path", ["group_id"] = (Int64)7, ["permission"] = "contract-permission", ["recursive"] = true, ["partner_id"] = (Int64)7, ["user_id"] = (Int64)7, ["username"] = "contract-username", ["group_name"] = "contract-group_name", ["site_id"] = (Int64)7 };
            Permission created = await Simulator.Sdk(operation).Permissions.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["path"] = "string", ["group_id"] = "number", ["permission"] = "string", ["recursive"] = "boolean", ["partner_id"] = "number", ["user_id"] = "number", ["username"] = "string", ["group_name"] = "string", ["site_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, null, Scope(), candidate => true);
            await Simulator.KeptAsync(sent, record, "Permission.create", "path", "group_id", "permission", "recursive", "partner_id", "user_id", "username", "group_name", "site_id");
            AssertModel(record, created, "Permission.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Permission", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Permission[] listed = (await Simulator.Sdk(operation).Permissions.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Permission.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Permission", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Permissions.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            Assert.AreEqual(0, (await Simulator.ListRawAsync(ListPath, Scope())).Length, "Permission.delete: the record is gone");
        }
    }
}