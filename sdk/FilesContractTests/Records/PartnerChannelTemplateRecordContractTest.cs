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

    // The record contract of PartnerChannelTemplate, generated from the schema: each case sends one request through the
    // SDK's PartnerChannelTemplates operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class PartnerChannelTemplateRecordContractTest : ContractCase
    {
        private const string ListPath = "/partner_channel_templates";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("PartnerChannelTemplate", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["direction"] = "two_way", ["use_channel_root"] = true, ["from_partner_folder_name"] = "contract-from_partner_folder_name", ["from_partner_managed_folder_paths"] = new string[] { "a" }, ["from_partner_route_path_pattern"] = "contract-from_partner_route_path_pattern", ["to_partner_folder_name"] = "contract-to_partner_folder_name", ["to_partner_managed_folder_paths"] = new string[] { "a" }, ["to_partner_route_path_pattern"] = "contract-to_partner_route_path_pattern", ["name"] = "contract-name", ["path"] = "contract-path", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "PartnerChannelTemplate", ("id", "int64"), ("workspace_id", "int64"), ("direction", "string"), ("use_channel_root", "boolean"), ("name", "string"), ("path", "string"), ("to_partner_folder_name", "string"), ("from_partner_folder_name", "string"), ("from_partner_route_path_pattern", "string"), ("to_partner_route_path_pattern", "string"), ("to_partner_managed_folder_paths", "array(string)"), ("from_partner_managed_folder_paths", "array(string)"), ("effective_to_partner_folder_name", "string"), ("effective_from_partner_folder_name", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the PartnerChannelTemplate the SDK returns gives it.
        private static void AssertModel(JsonObject record, PartnerChannelTemplate model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no PartnerChannelTemplate");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "direction", model.Direction, what);
            Simulator.Same(record, "use_channel_root", model.UseChannelRoot, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "to_partner_folder_name", model.ToPartnerFolderName, what);
            Simulator.Same(record, "from_partner_folder_name", model.FromPartnerFolderName, what);
            Simulator.Same(record, "from_partner_route_path_pattern", model.FromPartnerRoutePathPattern, what);
            Simulator.Same(record, "to_partner_route_path_pattern", model.ToPartnerRoutePathPattern, what);
            Simulator.Same(record, "to_partner_managed_folder_paths", model.ToPartnerManagedFolderPaths, what);
            Simulator.Same(record, "from_partner_managed_folder_paths", model.FromPartnerManagedFolderPaths, what);
            Simulator.Same(record, "effective_to_partner_folder_name", model.EffectiveToPartnerFolderName, what);
            Simulator.Same(record, "effective_from_partner_folder_name", model.EffectiveFromPartnerFolderName, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("PartnerChannelTemplate", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["direction"] = "two_way", ["use_channel_root"] = true, ["from_partner_folder_name"] = "contract-from_partner_folder_name", ["from_partner_managed_folder_paths"] = new string[] { "a" }, ["from_partner_route_path_pattern"] = "contract-from_partner_route_path_pattern", ["to_partner_folder_name"] = "contract-to_partner_folder_name", ["to_partner_managed_folder_paths"] = new string[] { "a" }, ["to_partner_route_path_pattern"] = "contract-to_partner_route_path_pattern", ["name"] = "contract-name", ["path"] = "contract-path", ["workspace_id"] = (Int64)7 };
            PartnerChannelTemplate created = await Simulator.Sdk(operation).PartnerChannelTemplates.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["direction"] = "string", ["use_channel_root"] = "boolean", ["from_partner_folder_name"] = "string", ["from_partner_managed_folder_paths"] = "array", ["from_partner_route_path_pattern"] = "string", ["to_partner_folder_name"] = "string", ["to_partner_managed_folder_paths"] = "array", ["to_partner_route_path_pattern"] = "string", ["name"] = "string", ["path"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "PartnerChannelTemplate.create", "direction", "use_channel_root", "from_partner_folder_name", "from_partner_managed_folder_paths", "from_partner_route_path_pattern", "to_partner_folder_name", "to_partner_managed_folder_paths", "to_partner_route_path_pattern", "name", "path", "workspace_id");
            AssertModel(record, created, "PartnerChannelTemplate.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("PartnerChannelTemplate", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            PartnerChannelTemplate[] listed = (await Simulator.Sdk(operation).PartnerChannelTemplates.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"PartnerChannelTemplate.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("PartnerChannelTemplate", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            PartnerChannelTemplate found = await Simulator.Sdk(operation).PartnerChannelTemplates.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "PartnerChannelTemplate.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("PartnerChannelTemplate", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["direction"] = "to_partner";
            sent["use_channel_root"] = false;
            sent["from_partner_folder_name"] = "contract-from_partner_folder_name-2";
            sent["from_partner_managed_folder_paths"] = new string[] { "b" };
            sent["from_partner_route_path_pattern"] = "contract-from_partner_route_path_pattern-2";
            sent["to_partner_folder_name"] = "contract-to_partner_folder_name-2";
            sent["to_partner_managed_folder_paths"] = new string[] { "b" };
            sent["to_partner_route_path_pattern"] = "contract-to_partner_route_path_pattern-2";
            sent["name"] = "contract-name-2";
            sent["path"] = "contract-path-2";
            PartnerChannelTemplate updated = await Simulator.Sdk(operation).PartnerChannelTemplates.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["direction"] = "string", ["use_channel_root"] = "boolean", ["from_partner_folder_name"] = "string", ["from_partner_managed_folder_paths"] = "array", ["from_partner_route_path_pattern"] = "string", ["to_partner_folder_name"] = "string", ["to_partner_managed_folder_paths"] = "array", ["to_partner_route_path_pattern"] = "string", ["name"] = "string", ["path"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "PartnerChannelTemplate.update", "direction", "use_channel_root", "from_partner_folder_name", "from_partner_managed_folder_paths", "from_partner_route_path_pattern", "to_partner_folder_name", "to_partner_managed_folder_paths", "to_partner_route_path_pattern", "name", "path");
            AssertModel(now, updated, "PartnerChannelTemplate.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("PartnerChannelTemplate", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).PartnerChannelTemplates.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"PartnerChannelTemplate.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}