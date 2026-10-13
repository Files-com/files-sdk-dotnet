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

    // The record contract of SftpActionLog, generated from the schema: each case sends one request through the
    // SDK's SftpActionLogs operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SftpActionLogRecordContractTest : ContractCase
    {
        private const string ListPath = "/sftp_action_logs";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["timestamp"] = "2030-01-02T03:04:05Z", ["remote_ip"] = "contract-remote_ip", ["server_ip"] = "contract-server_ip", ["username"] = "contract-username", ["ssh_client_identification"] = "contract-ssh_client_identification", ["session_uuid"] = "contract-session_uuid", ["seq_id"] = 7, ["auth_method"] = "contract-auth_method", ["auth_ciphers"] = "contract-auth_ciphers", ["action_type"] = "contract-action_type", ["path"] = "contract-path", ["true_path"] = "contract-true_path", ["name"] = "contract-name", ["sftp_response_code"] = "contract-sftp_response_code", ["sftp_response_message"] = "contract-sftp_response_message", ["md5"] = "contract-md5", ["size"] = 7, ["data_length"] = 7, ["bytes_transferred"] = 7, ["entries_returned"] = 7, ["success"] = true, ["status"] = "SUCCESS", ["duration_ms"] = 7, ["created_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "SftpActionLog", ("timestamp", "date-time"), ("remote_ip", "string"), ("server_ip", "string"), ("username", "string"), ("ssh_client_identification", "string"), ("session_uuid", "string"), ("seq_id", "int64"), ("auth_method", "string"), ("auth_ciphers", "string"), ("action_type", "string"), ("path", "string"), ("true_path", "string"), ("name", "string"), ("sftp_response_code", "string"), ("sftp_response_message", "string"), ("md5", "string"), ("size", "int64"), ("data_length", "int64"), ("bytes_transferred", "int64"), ("entries_returned", "int64"), ("success", "boolean"), ("status", "string"), ("duration_ms", "int64"), ("created_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the SftpActionLog the SDK returns gives it.
        private static void AssertModel(JsonObject record, SftpActionLog model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no SftpActionLog");
            Simulator.Same(record, "timestamp", model.Timestamp, what);
            Simulator.Same(record, "remote_ip", model.RemoteIp, what);
            Simulator.Same(record, "server_ip", model.ServerIp, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "ssh_client_identification", model.SshClientIdentification, what);
            Simulator.Same(record, "session_uuid", model.SessionUuid, what);
            Simulator.Same(record, "seq_id", model.SeqId, what);
            Simulator.Same(record, "auth_method", model.AuthMethod, what);
            Simulator.Same(record, "auth_ciphers", model.AuthCiphers, what);
            Simulator.Same(record, "action_type", model.ActionType, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "true_path", model.TruePath, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "sftp_response_code", model.SftpResponseCode, what);
            Simulator.Same(record, "sftp_response_message", model.SftpResponseMessage, what);
            Simulator.Same(record, "md5", model.Md5, what);
            Simulator.Same(record, "size", model.Size, what);
            Simulator.Same(record, "data_length", model.DataLength, what);
            Simulator.Same(record, "bytes_transferred", model.BytesTransferred, what);
            Simulator.Same(record, "entries_returned", model.EntriesReturned, what);
            Simulator.Same(record, "success", model.Success, what);
            Simulator.Same(record, "status", model.Status, what);
            Simulator.Same(record, "duration_ms", model.DurationMs, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("SftpActionLog", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SftpActionLog[] listed = (await Simulator.Sdk(operation).SftpActionLogs.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"SftpActionLog.list[{i}]");
            }
        }
    }
}