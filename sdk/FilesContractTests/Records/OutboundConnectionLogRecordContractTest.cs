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

    // The record contract of OutboundConnectionLog, generated from the schema: each case sends one request through the
    // SDK's OutboundConnectionLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class OutboundConnectionLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/outbound_connection_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["timestamp"] = "2030-01-02T03:04:05Z", ["path"] = "contract-path", ["client_ip"] = "contract-client_ip", ["src_remote_server_id"] = 7, ["src_remote_server_name"] = "contract-src_remote_server_name", ["dest_remote_server_id"] = 7, ["dest_remote_server_name"] = "contract-dest_remote_server_name", ["operation"] = "copy_file", ["error_message"] = "contract-error_message", ["error_operation"] = "contract-error_operation", ["error_type"] = "contract-error_type", ["status"] = "complete", ["duration_ms"] = 7, ["bytes_uploaded"] = 7, ["bytes_downloaded"] = 7, ["list_count"] = 7, ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "OutboundConnectionLog", ("timestamp", "date-time"), ("path", "string"), ("client_ip", "string"), ("src_remote_server_id", "int64"), ("src_remote_server_name", "string"), ("dest_remote_server_id", "int64"), ("dest_remote_server_name", "string"), ("operation", "string"), ("error_message", "string"), ("error_operation", "string"), ("error_type", "string"), ("status", "string"), ("duration_ms", "int64"), ("bytes_uploaded", "int64"), ("bytes_downloaded", "int64"), ("list_count", "int64"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the OutboundConnectionLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, OutboundConnectionLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no OutboundConnectionLog");
            Simulator.Same(record, "timestamp", model.Timestamp, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "client_ip", model.ClientIp, what);
            Simulator.Same(record, "src_remote_server_id", model.SrcRemoteServerId, what);
            Simulator.Same(record, "src_remote_server_name", model.SrcRemoteServerName, what);
            Simulator.Same(record, "dest_remote_server_id", model.DestRemoteServerId, what);
            Simulator.Same(record, "dest_remote_server_name", model.DestRemoteServerName, what);
            Simulator.Same(record, "operation", model.Operation, what);
            Simulator.Same(record, "error_message", model.ErrorMessage, what);
            Simulator.Same(record, "error_operation", model.ErrorOperation, what);
            Simulator.Same(record, "error_type", model.ErrorType, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "duration_ms", model.DurationMs, what);
            Simulator.Same(record, "bytes_uploaded", model.BytesUploaded, what);
            Simulator.Same(record, "bytes_downloaded", model.BytesDownloaded, what);
            Simulator.Same(record, "list_count", model.ListCount, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("OutboundConnectionLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            OutboundConnectionLog[] listed = (await Simulator.Sdk(operation).OutboundConnectionLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"OutboundConnectionLog.list[{i}]");
            }
        }
    }
}