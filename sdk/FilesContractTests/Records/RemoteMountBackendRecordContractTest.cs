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

    // The record contract of RemoteMountBackend, generated from the schema: each case sends one request through the
    // SDK's RemoteMountBackends operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class RemoteMountBackendRecordContractTest : ContractCase
    {
        private const string ListPath = "/remote_mount_backends";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("RemoteMountBackend", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["enabled"] = true, ["fall"] = (Int64)7, ["health_check_enabled"] = true, ["health_check_type"] = "active", ["interval"] = (Int64)7, ["priority"] = (Int64)7, ["remote_path"] = "contract-remote_path", ["rise"] = (Int64)7, ["canary_file_path"] = "contract-canary_file_path", ["remote_server_mount_id"] = (Int64)7, ["remote_server_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "RemoteMountBackend", ("canary_file_path", "string"), ("enabled", "boolean"), ("fall", "int64"), ("health_check_enabled", "boolean"), ("health_check_type", "string"), ("id", "int64"), ("interval", "int64"), ("min_free_cpu", "decimal"), ("min_free_mem", "decimal"), ("priority", "int64"), ("remote_path", "string"), ("remote_server_id", "int64"), ("remote_server_mount_id", "int64"), ("rise", "int64"), ("status", "string"), ("undergoing_maintenance", "boolean"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the RemoteMountBackend the SDK returns gives it.
        private static void AssertModel(JsonObject record, RemoteMountBackend model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no RemoteMountBackend");
            Simulator.Same(record, "canary_file_path", model.CanaryFilePath, what);
            Simulator.Same(record, "enabled", model.Enabled, what);
            Simulator.Same(record, "fall", model.Fall, what);
            Simulator.Same(record, "health_check_enabled", model.HealthCheckEnabled, what);
            Simulator.Same(record, "health_check_type", model.HealthCheckType, what);
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "interval", model.Interval, what);
            Simulator.Same(record, "min_free_cpu", model.MinFreeCpu, what);
            Simulator.Same(record, "min_free_mem", model.MinFreeMem, what);
            Simulator.Same(record, "priority", model.Priority, what);
            Simulator.Same(record, "remote_path", model.RemotePath, what);
            Simulator.Same(record, "remote_server_id", model.RemoteServerId, what);
            Simulator.Same(record, "remote_server_mount_id", model.RemoteServerMountId, what);
            Simulator.Same(record, "rise", model.Rise, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "undergoing_maintenance", model.UndergoingMaintenance, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteMountBackend", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["enabled"] = true, ["fall"] = (Int64)7, ["health_check_enabled"] = true, ["health_check_type"] = "active", ["interval"] = (Int64)7, ["priority"] = (Int64)7, ["remote_path"] = "contract-remote_path", ["rise"] = (Int64)7, ["canary_file_path"] = "contract-canary_file_path", ["remote_server_mount_id"] = (Int64)7, ["remote_server_id"] = (Int64)7 };
            RemoteMountBackend created = await Simulator.Sdk(operation).RemoteMountBackends.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["enabled"] = "boolean", ["fall"] = "number", ["health_check_enabled"] = "boolean", ["health_check_type"] = "string", ["interval"] = "number", ["priority"] = "number", ["remote_path"] = "string", ["rise"] = "number", ["canary_file_path"] = "string", ["remote_server_mount_id"] = "number", ["remote_server_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "RemoteMountBackend.create", "enabled", "fall", "health_check_enabled", "health_check_type", "interval", "priority", "remote_path", "rise", "canary_file_path", "remote_server_mount_id", "remote_server_id");
            AssertModel(record, created, "RemoteMountBackend.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteMountBackend", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            RemoteMountBackend[] listed = (await Simulator.Sdk(operation).RemoteMountBackends.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"RemoteMountBackend.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteMountBackend", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            RemoteMountBackend found = await Simulator.Sdk(operation).RemoteMountBackends.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "RemoteMountBackend.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteMountBackend", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["enabled"] = false;
            sent["fall"] = (Int64)8;
            sent["health_check_enabled"] = false;
            sent["health_check_type"] = "passive";
            sent["interval"] = (Int64)8;
            sent["priority"] = (Int64)8;
            sent["remote_path"] = "contract-remote_path-2";
            sent["rise"] = (Int64)8;
            sent["canary_file_path"] = "contract-canary_file_path-2";
            sent["remote_server_id"] = (Int64)8;
            RemoteMountBackend updated = await Simulator.Sdk(operation).RemoteMountBackends.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["enabled"] = "boolean", ["fall"] = "number", ["health_check_enabled"] = "boolean", ["health_check_type"] = "string", ["interval"] = "number", ["priority"] = "number", ["remote_path"] = "string", ["rise"] = "number", ["canary_file_path"] = "string", ["remote_server_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "RemoteMountBackend.update", "enabled", "fall", "health_check_enabled", "health_check_type", "interval", "priority", "remote_path", "rise", "canary_file_path", "remote_server_id");
            AssertModel(now, updated, "RemoteMountBackend.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("RemoteMountBackend", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).RemoteMountBackends.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"RemoteMountBackend.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}