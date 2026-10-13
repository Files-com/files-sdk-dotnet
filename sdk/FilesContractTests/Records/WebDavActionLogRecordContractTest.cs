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

    // The record contract of WebDavActionLog, generated from the schema: each case sends one request through the
    // SDK's WebDavActionLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class WebDavActionLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/web_dav_action_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["timestamp"] = "2030-01-02T03:04:05Z", ["remote_ip"] = "contract-remote_ip", ["server_ip"] = "contract-server_ip", ["username"] = "contract-username", ["auth_ciphers"] = "contract-auth_ciphers", ["action_type"] = "contract-action_type", ["path"] = "contract-path", ["true_path"] = "contract-true_path", ["name"] = "contract-name", ["http_method"] = "contract-http_method", ["http_path"] = "contract-http_path", ["http_response_code"] = 7, ["size"] = 7, ["entries_returned"] = 7, ["success"] = true, ["status"] = "SUCCESS", ["duration_ms"] = 7, ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "WebDavActionLog", ("timestamp", "date-time"), ("remote_ip", "string"), ("server_ip", "string"), ("username", "string"), ("auth_ciphers", "string"), ("action_type", "string"), ("path", "string"), ("true_path", "string"), ("name", "string"), ("http_method", "string"), ("http_path", "string"), ("http_response_code", "int64"), ("size", "int64"), ("entries_returned", "int64"), ("success", "boolean"), ("status", "string"), ("duration_ms", "int64"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the WebDavActionLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, WebDavActionLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no WebDavActionLog");
            Simulator.Same(record, "timestamp", model.Timestamp, what);
            Simulator.Same(record, "remote_ip", model.RemoteIp, what);
            Simulator.Same(record, "server_ip", model.ServerIp, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "auth_ciphers", model.AuthCiphers, what);
            Simulator.Same(record, "action_type", model.ActionType, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "true_path", model.TruePath, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "http_method", model.HttpMethod, what);
            Simulator.Same(record, "http_path", model.HttpPath, what);
            Simulator.Same(record, "http_response_code", model.HttpResponseCode, what);
            Simulator.Same(record, "size", model.Size, what);
            Simulator.Same(record, "entries_returned", model.EntriesReturned, what);
            Simulator.Same(record, "success", model.Success, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "duration_ms", model.DurationMs, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("WebDavActionLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            WebDavActionLog[] listed = (await Simulator.Sdk(operation).WebDavActionLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"WebDavActionLog.list[{i}]");
            }
        }
    }
}