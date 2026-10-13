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

    // The record contract of Behavior, generated from the schema: each case sends one request through the
    // SDK's Behaviors operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class BehaviorRecordContractTest : ContractCase
    {
        private const string ListPath = "/behaviors";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Behavior", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["disable_parent_folder_behavior"] = true, ["recursive"] = true, ["name"] = "contract-name", ["description"] = "contract-description", ["path"] = "contract-path", ["behavior"] = "contract-behavior" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Behavior", ("id", "int64"), ("path", "string"), ("attachment_url", "string"), ("behavior", "string"), ("name", "string"), ("description", "string"), ("public_hosting_url", "string"), ("disable_parent_folder_behavior", "boolean"), ("recursive", "boolean"), ("inherited", "boolean"), ("managed", "boolean"), ("root_behavior_site_admin_only", "boolean"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Behavior the SDK returns gives it.
        private static void AssertModel(JsonObject record, Behavior model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Behavior");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "attachment_url", model.AttachmentUrl, what);
            Simulator.Same(record, "behavior", model.BehaviorType, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "public_hosting_url", model.PublicHostingUrl, what);
            Simulator.Same(record, "disable_parent_folder_behavior", model.DisableParentFolderBehavior, what);
            Simulator.Same(record, "recursive", model.Recursive, what);
            Simulator.Same(record, "inherited", model.Inherited, what);
            Simulator.Same(record, "managed", model.Managed, what);
            Simulator.Same(record, "root_behavior_site_admin_only", model.RootBehaviorSiteAdminOnly, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Behavior", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["disable_parent_folder_behavior"] = true, ["recursive"] = true, ["name"] = "contract-name", ["description"] = "contract-description", ["path"] = "contract-path", ["behavior"] = "contract-behavior" };
            Behavior created = await Simulator.Sdk(operation).Behaviors.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["disable_parent_folder_behavior"] = "boolean", ["recursive"] = "boolean", ["name"] = "string", ["description"] = "string", ["path"] = "string", ["behavior"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Behavior.create", "disable_parent_folder_behavior", "recursive", "name", "description", "path", "behavior");
            AssertModel(record, created, "Behavior.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Behavior", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Behavior[] listed = (await Simulator.Sdk(operation).Behaviors.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Behavior.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Behavior", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Behavior found = await Simulator.Sdk(operation).Behaviors.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Behavior.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Behavior", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["disable_parent_folder_behavior"] = false;
            sent["recursive"] = false;
            sent["name"] = "contract-name-2";
            sent["description"] = "contract-description-2";
            Behavior updated = await Simulator.Sdk(operation).Behaviors.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["disable_parent_folder_behavior"] = "boolean", ["recursive"] = "boolean", ["name"] = "string", ["description"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Behavior.update", "disable_parent_folder_behavior", "recursive", "name", "description");
            AssertModel(now, updated, "Behavior.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Behavior", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Behaviors.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Behavior.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}