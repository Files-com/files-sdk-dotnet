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

    // The record contract of HistoryExport, generated from the schema: each case sends one request through the
    // SDK's HistoryExports operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class HistoryExportRecordContractTest : ContractCase
    {
        private const string ListPath = "/history_exports";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("HistoryExport", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["start_at"] = "2030-01-02T03:04:05Z", ["end_at"] = "2030-01-02T03:04:05Z", ["query_action"] = "contract-query_action", ["query_interface"] = "contract-query_interface", ["query_user_id"] = "contract-query_user_id", ["query_file_id"] = "contract-query_file_id", ["query_parent_id"] = "contract-query_parent_id", ["query_path"] = "contract-query_path", ["query_folder"] = "contract-query_folder", ["query_src"] = "contract-query_src", ["query_destination"] = "contract-query_destination", ["query_ip"] = "contract-query_ip", ["query_username"] = "contract-query_username", ["query_failure_type"] = "contract-query_failure_type", ["query_target_id"] = "contract-query_target_id", ["query_target_name"] = "contract-query_target_name", ["query_target_permission"] = "contract-query_target_permission", ["query_target_user_id"] = "contract-query_target_user_id", ["query_target_username"] = "contract-query_target_username", ["query_target_platform"] = "contract-query_target_platform", ["query_target_permission_set"] = "contract-query_target_permission_set" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "HistoryExport", ("workspace_id", "int64"), ("id", "int64"), ("history_version", "string"), ("start_at", "date-time"), ("end_at", "date-time"), ("status", "string"), ("query_action", "string"), ("query_interface", "string"), ("query_user_id", "string"), ("query_file_id", "string"), ("query_parent_id", "string"), ("query_path", "string"), ("query_folder", "string"), ("query_src", "string"), ("query_destination", "string"), ("query_ip", "string"), ("query_username", "string"), ("query_failure_type", "string"), ("query_target_id", "string"), ("query_target_name", "string"), ("query_target_permission", "string"), ("query_target_user_id", "string"), ("query_target_username", "string"), ("query_target_platform", "string"), ("query_target_permission_set", "string"), ("results_url", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the HistoryExport the SDK returns gives it.
        private static void AssertModel(JsonObject record, HistoryExport model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no HistoryExport");
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "history_version", model.HistoryVersion, what);
            Simulator.Same(record, "start_at", model.StartAt, what);
            Simulator.Same(record, "end_at", model.EndAt, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "query_action", model.QueryAction, what);
            Simulator.Same(record, "query_interface", model.QueryInterface, what);
            Simulator.Same(record, "query_user_id", model.QueryUserId, what);
            Simulator.Same(record, "query_file_id", model.QueryFileId, what);
            Simulator.Same(record, "query_parent_id", model.QueryParentId, what);
            Simulator.Same(record, "query_path", model.QueryPath, what);
            Simulator.Same(record, "query_folder", model.QueryFolder, what);
            Simulator.Same(record, "query_src", model.QuerySrc, what);
            Simulator.Same(record, "query_destination", model.QueryDestination, what);
            Simulator.Same(record, "query_ip", model.QueryIp, what);
            Simulator.Same(record, "query_username", model.QueryUsername, what);
            Simulator.Same(record, "query_failure_type", model.QueryFailureType, what);
            Simulator.Same(record, "query_target_id", model.QueryTargetId, what);
            Simulator.Same(record, "query_target_name", model.QueryTargetName, what);
            Simulator.Same(record, "query_target_permission", model.QueryTargetPermission, what);
            Simulator.Same(record, "query_target_user_id", model.QueryTargetUserId, what);
            Simulator.Same(record, "query_target_username", model.QueryTargetUsername, what);
            Simulator.Same(record, "query_target_platform", model.QueryTargetPlatform, what);
            Simulator.Same(record, "query_target_permission_set", model.QueryTargetPermissionSet, what);
            Simulator.Same(record, "results_url", model.ResultsUrl, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("HistoryExport", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["start_at"] = "2030-01-02T03:04:05Z", ["end_at"] = "2030-01-02T03:04:05Z", ["query_action"] = "contract-query_action", ["query_interface"] = "contract-query_interface", ["query_user_id"] = "contract-query_user_id", ["query_file_id"] = "contract-query_file_id", ["query_parent_id"] = "contract-query_parent_id", ["query_path"] = "contract-query_path", ["query_folder"] = "contract-query_folder", ["query_src"] = "contract-query_src", ["query_destination"] = "contract-query_destination", ["query_ip"] = "contract-query_ip", ["query_username"] = "contract-query_username", ["query_failure_type"] = "contract-query_failure_type", ["query_target_id"] = "contract-query_target_id", ["query_target_name"] = "contract-query_target_name", ["query_target_permission"] = "contract-query_target_permission", ["query_target_user_id"] = "contract-query_target_user_id", ["query_target_username"] = "contract-query_target_username", ["query_target_platform"] = "contract-query_target_platform", ["query_target_permission_set"] = "contract-query_target_permission_set" };
            HistoryExport created = await Simulator.Sdk(operation).HistoryExports.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["start_at"] = "string", ["end_at"] = "string", ["query_action"] = "string", ["query_interface"] = "string", ["query_user_id"] = "string", ["query_file_id"] = "string", ["query_parent_id"] = "string", ["query_path"] = "string", ["query_folder"] = "string", ["query_src"] = "string", ["query_destination"] = "string", ["query_ip"] = "string", ["query_username"] = "string", ["query_failure_type"] = "string", ["query_target_id"] = "string", ["query_target_name"] = "string", ["query_target_permission"] = "string", ["query_target_user_id"] = "string", ["query_target_username"] = "string", ["query_target_platform"] = "string", ["query_target_permission_set"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "HistoryExport.create", "start_at", "end_at", "query_action", "query_interface", "query_user_id", "query_file_id", "query_parent_id", "query_path", "query_folder", "query_src", "query_destination", "query_ip", "query_username", "query_failure_type", "query_target_id", "query_target_name", "query_target_permission", "query_target_user_id", "query_target_username", "query_target_platform", "query_target_permission_set");
            AssertModel(record, created, "HistoryExport.create");
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("HistoryExport", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            HistoryExport found = await Simulator.Sdk(operation).HistoryExports.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "HistoryExport.find");
        }
    }
}