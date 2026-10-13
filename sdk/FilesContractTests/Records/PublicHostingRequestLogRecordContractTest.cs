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

    // The record contract of PublicHostingRequestLog, generated from the schema: each case sends one request through the
    // SDK's PublicHostingRequestLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class PublicHostingRequestLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/public_hosting_request_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["timestamp"] = "2030-01-02T03:04:05Z", ["remote_ip"] = "contract-remote_ip", ["server_ip"] = "contract-server_ip", ["hostname"] = "contract-hostname", ["path"] = "contract-path", ["responseCode"] = 7, ["success"] = true, ["duration_ms"] = 7, ["created_at"] = "2030-01-02T03:04:05Z", ["bytes_transferred"] = 7, ["http_method"] = "contract-http_method" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "PublicHostingRequestLog", ("timestamp", "date-time"), ("remote_ip", "string"), ("server_ip", "string"), ("hostname", "string"), ("path", "string"), ("responseCode", "int64"), ("success", "boolean"), ("duration_ms", "int64"), ("created_at", "date-time"), ("bytes_transferred", "int64"), ("http_method", "string"));
            }
            return records;
        }

        // Every property the record holds, as the PublicHostingRequestLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, PublicHostingRequestLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no PublicHostingRequestLog");
            Simulator.Same(record, "timestamp", model.Timestamp, what);
            Simulator.Same(record, "remote_ip", model.RemoteIp, what);
            Simulator.Same(record, "server_ip", model.ServerIp, what);
            Simulator.Same(record, "hostname", model.Hostname, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "responseCode", model.ResponseCode, what);
            Simulator.Same(record, "success", model.Success, what);
            Simulator.Same(record, "duration_ms", model.DurationMs, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "bytes_transferred", model.BytesTransferred, what);
            Simulator.Same(record, "http_method", model.HttpMethod, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("PublicHostingRequestLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            PublicHostingRequestLog[] listed = (await Simulator.Sdk(operation).PublicHostingRequestLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"PublicHostingRequestLog.list[{i}]");
            }
        }
    }
}