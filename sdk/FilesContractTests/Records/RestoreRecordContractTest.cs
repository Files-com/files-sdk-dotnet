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

    // The record contract of Restore, generated from the schema: each case sends one request through the
    // SDK's Restores operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class RestoreRecordContractTest : ContractCase
    {
        private const string ListPath = "/restores";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Restore", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["earliest_date"] = "2030-01-02T03:04:05Z", ["prefix"] = "contract-prefix", ["restoration_type"] = "files", ["restore_deleted_permissions"] = true, ["restore_in_place"] = true, ["update_timestamps"] = true, ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Restore", ("earliest_date", "date-time"), ("id", "int64"), ("dirs_restored", "int64"), ("dirs_errored", "int64"), ("dirs_total", "int64"), ("files_restored", "int64"), ("files_errored", "int64"), ("files_total", "int64"), ("prefix", "string"), ("restoration_type", "string"), ("restore_in_place", "boolean"), ("restore_deleted_permissions", "boolean"), ("users_restored", "int64"), ("users_errored", "int64"), ("users_total", "int64"), ("api_keys_restored", "int64"), ("public_keys_restored", "int64"), ("two_factor_authentication_methods_restored", "int64"), ("status", "string"), ("update_timestamps", "boolean"), ("workspace_id", "int64"), ("error_messages", "array(string)"));
            }
            return records;
        }

        // Every property the record holds, as the Restore the SDK returns gives it.
        private static void AssertModel(JsonObject record, Restore model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Restore");
            Simulator.Same(record, "earliest_date", model.EarliestDate, what);
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "dirs_restored", model.DirsRestored, what);
            Simulator.Same(record, "dirs_errored", model.DirsErrored, what);
            Simulator.Same(record, "dirs_total", model.DirsTotal, what);
            Simulator.Same(record, "files_restored", model.FilesRestored, what);
            Simulator.Same(record, "files_errored", model.FilesErrored, what);
            Simulator.Same(record, "files_total", model.FilesTotal, what);
            Simulator.Same(record, "prefix", model.Prefix, what);
            Simulator.Same(record, "restoration_type", model.RestorationType, what);
            Simulator.Same(record, "restore_in_place", model.RestoreInPlace, what);
            Simulator.Same(record, "restore_deleted_permissions", model.RestoreDeletedPermissions, what);
            Simulator.Same(record, "users_restored", model.UsersRestored, what);
            Simulator.Same(record, "users_errored", model.UsersErrored, what);
            Simulator.Same(record, "users_total", model.UsersTotal, what);
            Simulator.Same(record, "api_keys_restored", model.ApiKeysRestored, what);
            Simulator.Same(record, "public_keys_restored", model.PublicKeysRestored, what);
            Simulator.Same(record, "two_factor_authentication_methods_restored", model.TwoFactorAuthenticationMethodsRestored, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "update_timestamps", model.UpdateTimestamps, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "error_messages", model.ErrorMessages, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Restore", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["earliest_date"] = "2030-01-02T03:04:05Z", ["prefix"] = "contract-prefix", ["restoration_type"] = "files", ["restore_deleted_permissions"] = true, ["restore_in_place"] = true, ["update_timestamps"] = true, ["workspace_id"] = (Int64)7 };
            Restore created = await Simulator.Sdk(operation).Restores.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["earliest_date"] = "string", ["prefix"] = "string", ["restoration_type"] = "string", ["restore_deleted_permissions"] = "boolean", ["restore_in_place"] = "boolean", ["update_timestamps"] = "boolean", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, null, Scope(), candidate => true);
            await Simulator.KeptAsync(sent, record, "Restore.create", "earliest_date", "prefix", "restoration_type", "restore_deleted_permissions", "restore_in_place", "update_timestamps", "workspace_id");
            AssertModel(record, created, "Restore.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Restore", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Restore[] listed = (await Simulator.Sdk(operation).Restores.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Restore.list[{i}]");
            }
        }
    }
}