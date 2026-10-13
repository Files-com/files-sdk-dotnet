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

    // The record contract of ExavaultApiRequestLog, generated from the schema: each case sends one request through the
    // SDK's ExavaultApiRequestLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ExavaultApiRequestLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/exavault_api_request_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["timestamp"] = "2030-01-02T03:04:05Z", ["endpoint"] = "contract-endpoint", ["version"] = 1.5, ["request_ip"] = "contract-request_ip", ["request_method"] = "contract-request_method", ["error_type"] = "contract-error_type", ["error_message"] = "contract-error_message", ["user_agent"] = "contract-user_agent", ["response_code"] = 7, ["success"] = true, ["duration_ms"] = 7, ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ExavaultApiRequestLog", ("timestamp", "date-time"), ("endpoint", "string"), ("version", "double"), ("request_ip", "string"), ("request_method", "string"), ("error_type", "string"), ("error_message", "string"), ("user_agent", "string"), ("response_code", "int64"), ("success", "boolean"), ("duration_ms", "int64"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the ExavaultApiRequestLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, ExavaultApiRequestLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ExavaultApiRequestLog");
            Simulator.Same(record, "timestamp", model.Timestamp, what);
            Simulator.Same(record, "endpoint", model.Endpoint, what);
            Simulator.Same(record, "version", model.Version, what);
            Simulator.Same(record, "request_ip", model.RequestIp, what);
            Simulator.Same(record, "request_method", model.RequestMethod, what);
            Simulator.Same(record, "error_type", model.ErrorType, what);
            Simulator.Same(record, "error_message", model.ErrorMessage, what);
            Simulator.Same(record, "user_agent", model.UserAgent, what);
            Simulator.Same(record, "response_code", model.ResponseCode, what);
            Simulator.Same(record, "success", model.Success, what);
            Simulator.Same(record, "duration_ms", model.DurationMs, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ExavaultApiRequestLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ExavaultApiRequestLog[] listed = (await Simulator.Sdk(operation).ExavaultApiRequestLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ExavaultApiRequestLog.list[{i}]");
            }
        }
    }
}