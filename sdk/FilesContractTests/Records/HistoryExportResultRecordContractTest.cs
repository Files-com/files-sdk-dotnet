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

    // The record contract of HistoryExportResult, generated from the schema: each case sends one request through the
    // SDK's HistoryExportResults operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class HistoryExportResultRecordContractTest : ContractCase
    {
        private const string ListPath = "/history_export_results";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { ["history_export_id"] = (Int64)7 });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            // Each record holds its list's scope, which only a fixture gives it (_scope).
            JsonObject[] records = Enumerable.Range(1, count).Select(n =>
            {
                JsonObject record = new JsonObject { ["created_at"] = 7, ["created_at_iso8601"] = "contract-created_at_iso8601", ["user_id"] = 7, ["file_id"] = 7, ["parent_id"] = 7, ["path"] = "contract-path", ["folder"] = "contract-folder", ["src"] = "contract-src", ["destination"] = "contract-destination", ["ip"] = "contract-ip", ["username"] = "contract-username", ["user_is_from_parent_site"] = true, ["action"] = "contract-action", ["failure_type"] = "contract-failure_type", ["interface"] = "contract-interface", ["target_id"] = 7, ["target_name"] = "contract-target_name", ["target_permission"] = "contract-target_permission", ["target_recursive"] = true, ["target_expires_at"] = 7, ["target_expires_at_iso8601"] = "contract-target_expires_at_iso8601", ["target_permission_set"] = "contract-target_permission_set", ["target_platform"] = "contract-target_platform", ["target_username"] = "contract-target_username", ["target_user_id"] = 7 };
                record["_scope"] = Simulator.Json(new Dictionary<string, object> { ["history_export_id"] = (Int64)7 });
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
                Simulator.Conforms(record, "HistoryExportResult", ("id", "int64"), ("created_at", "int64"), ("created_at_iso8601", "string"), ("user_id", "int64"), ("file_id", "int64"), ("parent_id", "int64"), ("path", "string"), ("folder", "string"), ("src", "string"), ("destination", "string"), ("ip", "string"), ("username", "string"), ("user_is_from_parent_site", "boolean"), ("action", "string"), ("failure_type", "string"), ("interface", "string"), ("target_id", "int64"), ("target_name", "string"), ("target_permission", "string"), ("target_recursive", "boolean"), ("target_expires_at", "int64"), ("target_expires_at_iso8601", "string"), ("target_permission_set", "string"), ("target_platform", "string"), ("target_username", "string"), ("target_user_id", "int64"));
            }
            return records;
        }

        // Every property the record holds, as the HistoryExportResult the SDK returns gives it.
        private static void AssertModel(JsonObject record, HistoryExportResult model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no HistoryExportResult");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "created_at_iso8601", model.CreatedAtIso8601, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "file_id", model.FileId, what);
            Simulator.Same(record, "parent_id", model.ParentId, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "folder", model.Folder, what);
            Simulator.Same(record, "src", model.Src, what);
            Simulator.Same(record, "destination", model.Destination, what);
            Simulator.Same(record, "ip", model.Ip, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "user_is_from_parent_site", model.UserIsFromParentSite, what);
            Simulator.Same(record, "action", model.Action, what);
            Simulator.Same(record, "failure_type", model.FailureType, what);
            Simulator.Same(record, "interface", model.Interface, what);
            Simulator.Same(record, "target_id", model.TargetId, what);
            Simulator.Same(record, "target_name", model.TargetName, what);
            Simulator.Same(record, "target_permission", model.TargetPermission, what);
            Simulator.Same(record, "target_recursive", model.TargetRecursive, what);
            Simulator.Same(record, "target_expires_at", model.TargetExpiresAt, what);
            Simulator.Same(record, "target_expires_at_iso8601", model.TargetExpiresAtIso8601, what);
            Simulator.Same(record, "target_permission_set", model.TargetPermissionSet, what);
            Simulator.Same(record, "target_platform", model.TargetPlatform, what);
            Simulator.Same(record, "target_username", model.TargetUsername, what);
            Simulator.Same(record, "target_user_id", model.TargetUserId, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("HistoryExportResult", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { ["history_export_id"] = (Int64)7 };
            HistoryExportResult[] listed = (await Simulator.Sdk(operation).HistoryExportResults.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["history_export_id"] = "string" });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"HistoryExportResult.list[{i}]");
            }
        }
    }
}