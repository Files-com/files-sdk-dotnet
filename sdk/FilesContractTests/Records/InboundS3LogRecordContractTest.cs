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

    // The record contract of InboundS3Log, generated from the schema: each case sends one request through the
    // SDK's InboundS3Logs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class InboundS3LogRecordContractTest : ContractCase
    {
        private const string ListPath = "/inbound_s3_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["path"] = "contract-path", ["client_ip"] = "contract-client_ip", ["operation"] = "contract-operation", ["status"] = "contract-status", ["aws_access_key"] = "contract-aws_access_key", ["error_message"] = "contract-error_message", ["error_type"] = "contract-error_type", ["duration_ms"] = 7, ["request_id"] = "contract-request_id", ["user_agent"] = "contract-user_agent", ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "InboundS3Log", ("path", "string"), ("client_ip", "string"), ("operation", "string"), ("status", "string"), ("aws_access_key", "string"), ("error_message", "string"), ("error_type", "string"), ("duration_ms", "int64"), ("request_id", "string"), ("user_agent", "string"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the InboundS3Log the SDK returns gives it.
        private static void AssertModel(JsonObject record, InboundS3Log model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no InboundS3Log");
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "client_ip", model.ClientIp, what);
            Simulator.Same(record, "operation", model.Operation, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "aws_access_key", model.AwsAccessKey, what);
            Simulator.Same(record, "error_message", model.ErrorMessage, what);
            Simulator.Same(record, "error_type", model.ErrorType, what);
            Simulator.Same(record, "duration_ms", model.DurationMs, what);
            Simulator.Same(record, "request_id", model.RequestId, what);
            Simulator.Same(record, "user_agent", model.UserAgent, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("InboundS3Log", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            InboundS3Log[] listed = (await Simulator.Sdk(operation).InboundS3Logs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"InboundS3Log.list[{i}]");
            }
        }
    }
}