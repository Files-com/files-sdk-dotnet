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

    // The record contract of ActionNotificationExport, generated from the schema: each case sends one request through the
    // SDK's ActionNotificationExports operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ActionNotificationExportRecordContractTest : ContractCase
    {
        private const string ListPath = "/action_notification_exports";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("ActionNotificationExport", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["workspace_id"] = (Int64)7, ["start_at"] = "2030-01-02T03:04:05Z", ["end_at"] = "2030-01-02T03:04:05Z", ["query_message"] = "contract-query_message", ["query_request_method"] = "contract-query_request_method", ["query_request_url"] = "contract-query_request_url", ["query_status"] = "contract-query_status", ["query_success"] = true, ["query_path"] = "contract-query_path", ["query_folder"] = "contract-query_folder" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ActionNotificationExport", ("id", "int64"), ("workspace_id", "int64"), ("export_version", "string"), ("start_at", "date-time"), ("end_at", "date-time"), ("status", "string"), ("query_path", "string"), ("query_folder", "string"), ("query_message", "string"), ("query_request_method", "string"), ("query_request_url", "string"), ("query_status", "string"), ("query_success", "boolean"), ("results_url", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the ActionNotificationExport the SDK returns gives it.
        private static void AssertModel(JsonObject record, ActionNotificationExport model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ActionNotificationExport");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "export_version", model.ExportVersion, what);
            Simulator.Same(record, "start_at", model.StartAt, what);
            Simulator.Same(record, "end_at", model.EndAt, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "query_path", model.QueryPath, what);
            Simulator.Same(record, "query_folder", model.QueryFolder, what);
            Simulator.Same(record, "query_message", model.QueryMessage, what);
            Simulator.Same(record, "query_request_method", model.QueryRequestMethod, what);
            Simulator.Same(record, "query_request_url", model.QueryRequestUrl, what);
            Simulator.Same(record, "query_status", model.QueryStatus, what);
            Simulator.Same(record, "query_success", model.QuerySuccess, what);
            Simulator.Same(record, "results_url", model.ResultsUrl, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("ActionNotificationExport", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["workspace_id"] = (Int64)7, ["start_at"] = "2030-01-02T03:04:05Z", ["end_at"] = "2030-01-02T03:04:05Z", ["query_message"] = "contract-query_message", ["query_request_method"] = "contract-query_request_method", ["query_request_url"] = "contract-query_request_url", ["query_status"] = "contract-query_status", ["query_success"] = true, ["query_path"] = "contract-query_path", ["query_folder"] = "contract-query_folder" };
            ActionNotificationExport created = await Simulator.Sdk(operation).ActionNotificationExports.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["workspace_id"] = "number", ["start_at"] = "string", ["end_at"] = "string", ["query_message"] = "string", ["query_request_method"] = "string", ["query_request_url"] = "string", ["query_status"] = "string", ["query_success"] = "boolean", ["query_path"] = "string", ["query_folder"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "ActionNotificationExport.create", "workspace_id", "start_at", "end_at", "query_message", "query_request_method", "query_request_url", "query_status", "query_success", "query_path", "query_folder");
            AssertModel(record, created, "ActionNotificationExport.create");
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("ActionNotificationExport", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ActionNotificationExport found = await Simulator.Sdk(operation).ActionNotificationExports.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "ActionNotificationExport.find");
        }
    }
}