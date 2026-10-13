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

    // The record contract of ActionLog, generated from the schema: each case sends one request through the
    // SDK's ActionLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ActionLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/action_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["action"] = "contract-action", ["created_at"] = "2030-01-02T03:04:05Z", ["destination"] = "contract-destination", ["failure_type"] = "contract-failure_type", ["folder"] = "contract-folder", ["interface"] = "contract-interface", ["ip"] = "contract-ip", ["metadata_dm_id"] = 7, ["parent_metadata_dm_id"] = 7, ["path"] = "contract-path", ["site_id"] = 7, ["src"] = "contract-src", ["user_id"] = 7, ["username"] = "contract-username" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ActionLog", ("action", "string"), ("created_at", "date-time"), ("destination", "string"), ("failure_type", "string"), ("folder", "string"), ("interface", "string"), ("ip", "string"), ("metadata_dm_id", "int64"), ("parent_metadata_dm_id", "int64"), ("path", "string"), ("site_id", "int64"), ("src", "string"), ("user_id", "int64"), ("username", "string"));
            }
            return records;
        }

        // Every property the record holds, as the ActionLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, ActionLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ActionLog");
            Simulator.Same(record, "action", model.Action, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "destination", model.Destination, what);
            Simulator.Same(record, "failure_type", model.FailureType, what);
            Simulator.Same(record, "folder", model.Folder, what);
            Simulator.Same(record, "interface", model.Interface, what);
            Simulator.Same(record, "ip", model.Ip, what);
            Simulator.Same(record, "metadata_dm_id", model.MetadataDmId, what);
            Simulator.Same(record, "parent_metadata_dm_id", model.ParentMetadataDmId, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "site_id", model.SiteId, what);
            Simulator.Same(record, "src", model.Src, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "username", model.Username, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ActionLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ActionLog[] listed = (await Simulator.Sdk(operation).ActionLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ActionLog.list[{i}]");
            }
        }
    }
}