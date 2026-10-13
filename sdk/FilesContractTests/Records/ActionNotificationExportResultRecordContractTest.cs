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

    // The record contract of ActionNotificationExportResult, generated from the schema: each case sends one request through the
    // SDK's ActionNotificationExportResults operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ActionNotificationExportResultRecordContractTest : ContractCase
    {
        private const string ListPath = "/action_notification_export_results";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { ["action_notification_export_id"] = (Int64)7 });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            // Each record holds its list's scope, which only a fixture gives it (_scope).
            JsonObject[] records = Enumerable.Range(1, count).Select(n =>
            {
                JsonObject record = new JsonObject { ["created_at"] = 7, ["status"] = 7, ["message"] = "contract-message", ["success"] = true, ["request_headers"] = "contract-request_headers", ["request_method"] = "contract-request_method", ["request_url"] = "contract-request_url", ["path"] = "contract-path", ["folder"] = "contract-folder" };
                record["_scope"] = Simulator.Json(new Dictionary<string, object> { ["action_notification_export_id"] = (Int64)7 });
                return record;
            }).ToArray();
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), records);
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ActionNotificationExportResult", ("id", "int64"), ("created_at", "int64"), ("status", "int64"), ("message", "string"), ("success", "boolean"), ("request_headers", "string"), ("request_method", "string"), ("request_url", "string"), ("path", "string"), ("folder", "string"));
            }
            return records;
        }

        // Every property the record holds, as the ActionNotificationExportResult the SDK returns gives it.
        private static void AssertModel(JsonObject record, ActionNotificationExportResult model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ActionNotificationExportResult");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "message", model.Message, what);
            Simulator.Same(record, "success", model.Success, what);
            Simulator.Same(record, "request_headers", model.RequestHeaders, what);
            Simulator.Same(record, "request_method", model.RequestMethod, what);
            Simulator.Same(record, "request_url", model.RequestUrl, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "folder", model.Folder, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ActionNotificationExportResult", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { ["action_notification_export_id"] = (Int64)7 };
            ActionNotificationExportResult[] listed = (await Simulator.Sdk(operation).ActionNotificationExportResults.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["action_notification_export_id"] = "string" });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ActionNotificationExportResult.list[{i}]");
            }
        }
    }
}