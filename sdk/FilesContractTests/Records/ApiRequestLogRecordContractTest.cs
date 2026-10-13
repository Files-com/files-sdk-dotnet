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

    // The record contract of ApiRequestLog, generated from the schema: each case sends one request through the
    // SDK's ApiRequestLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class ApiRequestLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/api_request_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["timestamp"] = "2030-01-02T03:04:05Z", ["api_key_id"] = 7, ["api_key_prefix"] = "contract-api_key_prefix", ["user_id"] = 7, ["username"] = "contract-username", ["user_is_from_parent_site"] = true, ["interface"] = "contract-interface", ["request_method"] = "contract-request_method", ["request_path"] = "contract-request_path", ["request_ip"] = "contract-request_ip", ["request_host"] = "contract-request_host", ["request_id"] = "contract-request_id", ["api_name"] = "contract-api_name", ["user_agent"] = "contract-user_agent", ["error_type"] = "contract-error_type", ["error_message"] = "contract-error_message", ["response_code"] = 7, ["success"] = true, ["duration_ms"] = 7, ["impersonator_user_id"] = 7, ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "ApiRequestLog", ("timestamp", "date-time"), ("api_key_id", "int64"), ("api_key_prefix", "string"), ("user_id", "int64"), ("username", "string"), ("user_is_from_parent_site", "boolean"), ("interface", "string"), ("request_method", "string"), ("request_path", "string"), ("request_ip", "string"), ("request_host", "string"), ("request_id", "string"), ("api_name", "string"), ("user_agent", "string"), ("error_type", "string"), ("error_message", "string"), ("response_code", "int64"), ("success", "boolean"), ("duration_ms", "int64"), ("impersonator_user_id", "int64"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the ApiRequestLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, ApiRequestLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no ApiRequestLog");
            Simulator.Same(record, "timestamp", model.Timestamp, what);
            Simulator.Same(record, "api_key_id", model.ApiKeyId, what);
            Simulator.Same(record, "api_key_prefix", model.ApiKeyPrefix, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "user_is_from_parent_site", model.UserIsFromParentSite, what);
            Simulator.Same(record, "interface", model.Interface, what);
            Simulator.Same(record, "request_method", model.RequestMethod, what);
            Simulator.Same(record, "request_path", model.RequestPath, what);
            Simulator.Same(record, "request_ip", model.RequestIp, what);
            Simulator.Same(record, "request_host", model.RequestHost, what);
            Simulator.Same(record, "request_id", model.RequestId, what);
            Simulator.Same(record, "api_name", model.ApiName, what);
            Simulator.Same(record, "user_agent", model.UserAgent, what);
            Simulator.Same(record, "error_type", model.ErrorType, what);
            Simulator.Same(record, "error_message", model.ErrorMessage, what);
            Simulator.Same(record, "response_code", model.ResponseCode, what);
            Simulator.Same(record, "success", model.Success, what);
            Simulator.Same(record, "duration_ms", model.DurationMs, what);
            Simulator.Same(record, "impersonator_user_id", model.ImpersonatorUserId, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("ApiRequestLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            ApiRequestLog[] listed = (await Simulator.Sdk(operation).ApiRequestLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"ApiRequestLog.list[{i}]");
            }
        }
    }
}