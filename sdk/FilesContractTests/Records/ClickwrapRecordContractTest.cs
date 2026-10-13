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

    // The record contract of Clickwrap, generated from the schema: each case sends one request through the
    // SDK's Clickwraps operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ClickwrapRecordContractTest : ContractCase
    {
        private const string ListPath = "/clickwraps";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Clickwrap", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["body"] = "contract-body", ["use_with_bundles"] = "none", ["use_with_inboxes"] = "none", ["use_with_users"] = "none" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Clickwrap", ("id", "int64"), ("name", "string"), ("body", "string"), ("use_with_users", "string"), ("use_with_bundles", "string"), ("use_with_inboxes", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Clickwrap the SDK returns gives it.
        private static void AssertModel(JsonObject record, Clickwrap model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Clickwrap");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "body", model.Body, what);
            Simulator.Same(record, "use_with_users", model.UseWithUsers, what);
            Simulator.Same(record, "use_with_bundles", model.UseWithBundles, what);
            Simulator.Same(record, "use_with_inboxes", model.UseWithInboxes, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Clickwrap", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["body"] = "contract-body", ["use_with_bundles"] = "none", ["use_with_inboxes"] = "none", ["use_with_users"] = "none" };
            Clickwrap created = await Simulator.Sdk(operation).Clickwraps.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["body"] = "string", ["use_with_bundles"] = "string", ["use_with_inboxes"] = "string", ["use_with_users"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Clickwrap.create", "name", "body", "use_with_bundles", "use_with_inboxes", "use_with_users");
            AssertModel(record, created, "Clickwrap.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Clickwrap", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Clickwrap[] listed = (await Simulator.Sdk(operation).Clickwraps.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Clickwrap.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Clickwrap", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Clickwrap found = await Simulator.Sdk(operation).Clickwraps.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Clickwrap.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Clickwrap", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["body"] = "contract-body-2";
            sent["use_with_bundles"] = "available";
            sent["use_with_inboxes"] = "available";
            sent["use_with_users"] = "require";
            Clickwrap updated = await Simulator.Sdk(operation).Clickwraps.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["body"] = "string", ["use_with_bundles"] = "string", ["use_with_inboxes"] = "string", ["use_with_users"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Clickwrap.update", "name", "body", "use_with_bundles", "use_with_inboxes", "use_with_users");
            AssertModel(now, updated, "Clickwrap.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Clickwrap", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Clickwraps.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Clickwrap.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}