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

    // The record contract of UserCipherUse, generated from the schema: each case sends one request through the
    // SDK's UserCipherUses operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class UserCipherUseRecordContractTest : ContractCase
    {
        private const string ListPath = "/user_cipher_uses";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["user_id"] = 7, ["username"] = "contract-username", ["protocol_cipher"] = "contract-protocol_cipher", ["created_at"] = "2030-01-02T03:04:05Z", ["insecure"] = true, ["interface"] = "web", ["updated_at"] = "2030-01-02T03:04:05Z" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "UserCipherUse", ("id", "int64"), ("user_id", "int64"), ("username", "string"), ("protocol_cipher", "string"), ("created_at", "date-time"), ("insecure", "boolean"), ("interface", "string"), ("updated_at", "date-time"));
            }
            return records;
        }

        // Every property the record holds, as the UserCipherUse the SDK returns gives it.
        private static void AssertModel(JsonObject record, UserCipherUse model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no UserCipherUse");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "protocol_cipher", model.ProtocolCipher, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "insecure", model.Insecure, what);
            Simulator.Same(record, "interface", model.Interface, what);
            Simulator.Same(record, "updated_at", model.UpdatedAt, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("UserCipherUse", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UserCipherUse[] listed = (await Simulator.Sdk(operation).UserCipherUses.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"UserCipherUse.list[{i}]");
            }
        }
    }
}