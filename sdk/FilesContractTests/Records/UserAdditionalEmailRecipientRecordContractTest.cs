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

    // The record contract of UserAdditionalEmailRecipient, generated from the schema: each case sends one request through the
    // SDK's UserAdditionalEmailRecipients operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class UserAdditionalEmailRecipientRecordContractTest : ContractCase
    {
        private const string ListPath = "/user_additional_email_recipients";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("UserAdditionalEmailRecipient", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["user_id"] = (Int64)7, ["email"] = "contract-email" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "UserAdditionalEmailRecipient", ("id", "int64"), ("user_id", "int64"), ("workspace_id", "int64"), ("email", "string"), ("created_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the UserAdditionalEmailRecipient the SDK returns gives it.
        private static void AssertModel(JsonObject record, UserAdditionalEmailRecipient model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no UserAdditionalEmailRecipient");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "email", model.Email, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("UserAdditionalEmailRecipient", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["user_id"] = (Int64)7, ["email"] = "contract-email" };
            UserAdditionalEmailRecipient created = await Simulator.Sdk(operation).UserAdditionalEmailRecipients.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["user_id"] = "number", ["email"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "UserAdditionalEmailRecipient.create", "user_id", "email");
            AssertModel(record, created, "UserAdditionalEmailRecipient.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("UserAdditionalEmailRecipient", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UserAdditionalEmailRecipient[] listed = (await Simulator.Sdk(operation).UserAdditionalEmailRecipients.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"UserAdditionalEmailRecipient.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("UserAdditionalEmailRecipient", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UserAdditionalEmailRecipient found = await Simulator.Sdk(operation).UserAdditionalEmailRecipients.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "UserAdditionalEmailRecipient.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("UserAdditionalEmailRecipient", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["email"] = "contract-email-2";
            UserAdditionalEmailRecipient updated = await Simulator.Sdk(operation).UserAdditionalEmailRecipients.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["email"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "UserAdditionalEmailRecipient.update", "email");
            AssertModel(now, updated, "UserAdditionalEmailRecipient.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("UserAdditionalEmailRecipient", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).UserAdditionalEmailRecipients.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"UserAdditionalEmailRecipient.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}