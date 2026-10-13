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

    // The record contract of History, generated from the schema: each case sends one request through the
    // SDK's Histories operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class HistoryRecordContractTest : ContractCase
    {
        private const string ListPath = "/history";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["path"] = "contract-path", ["when"] = "2030-01-02T03:04:05Z", ["destination"] = "contract-destination", ["display"] = "contract-display", ["ip"] = "contract-ip", ["source"] = "contract-source", ["user_id"] = 7, ["username"] = "contract-username", ["user_is_from_parent_site"] = true, ["action"] = "create", ["failure_type"] = "expired_trial", ["interface"] = "web" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "History", ("id", "int64"), ("path", "string"), ("when", "date-time"), ("destination", "string"), ("display", "string"), ("ip", "string"), ("source", "string"), ("user_id", "int64"), ("username", "string"), ("user_is_from_parent_site", "boolean"), ("action", "string"), ("failure_type", "string"), ("interface", "string"));
            }
            return records;
        }

        // Every property the record holds, as the HistoryAction the SDK returns gives it.
        private static void AssertModel(JsonObject record, HistoryAction model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no HistoryAction");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "when", model.When, what);
            Simulator.Same(record, "destination", model.Destination, what);
            Simulator.Same(record, "display", model.Display, what);
            Simulator.Same(record, "ip", model.Ip, what);
            Simulator.Same(record, "source", model.Source, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "user_is_from_parent_site", model.UserIsFromParentSite, what);
            Simulator.Same(record, "action", model.ActionType, what);
            Simulator.Same(record, "failure_type", model.FailureType, what);
            Simulator.Same(record, "interface", model.Interface, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("History", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            HistoryAction[] listed = (await Simulator.Sdk(operation).Histories.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"History.list[{i}]");
            }
        }
    }
}