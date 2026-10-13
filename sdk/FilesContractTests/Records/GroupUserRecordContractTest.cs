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

    // The record contract of GroupUser, generated from the schema: each case sends one request through the
    // SDK's GroupUsers operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class GroupUserRecordContractTest : ContractCase
    {
        private const string ListPath = "/group_users";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("GroupUser", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["group_id"] = (Int64)7, ["user_id"] = (Int64)7, ["admin"] = true })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "GroupUser", ("group_name", "string"), ("group_id", "int64"), ("user_id", "int64"), ("admin", "boolean"), ("username", "string"));
            }
            return records;
        }

        // Every property the record holds, as the GroupUser the SDK returns gives it.
        private static void AssertModel(JsonObject record, GroupUser model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no GroupUser");
            Simulator.Same(record, "group_name", model.GroupName, what);
            Simulator.Same(record, "group_id", model.GroupId, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "admin", model.Admin, what);
            Simulator.Same(record, "username", model.Username, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("GroupUser", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["group_id"] = (Int64)7, ["user_id"] = (Int64)7, ["admin"] = true };
            GroupUser created = await Simulator.Sdk(operation).GroupUsers.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["group_id"] = "number", ["user_id"] = "number", ["admin"] = "boolean" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, null, Scope(), candidate => true);
            await Simulator.KeptAsync(sent, record, "GroupUser.create", "group_id", "user_id", "admin");
            AssertModel(record, created, "GroupUser.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("GroupUser", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            GroupUser[] listed = (await Simulator.Sdk(operation).GroupUsers.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"GroupUser.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("GroupUser", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { ["group_id"] = (Int64)7, ["user_id"] = (Int64)7 };
            sent["admin"] = false;
            GroupUser updated = await Simulator.Sdk(operation).GroupUsers.UpdateAsync(1, sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["group_id"] = "number", ["user_id"] = "number", ["admin"] = "boolean" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, null, Scope(), candidate => true);
            await Simulator.KeptAsync(sent, now, "GroupUser.update", "group_id", "user_id", "admin");
            AssertModel(now, updated, "GroupUser.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("GroupUser", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { ["group_id"] = (Int64)7, ["user_id"] = (Int64)7 };
            await Simulator.Sdk(operation).GroupUsers.DeleteAsync(1, sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string", ["group_id"] = "string", ["user_id"] = "string" });
            Assert.AreEqual(0, (await Simulator.ListRawAsync(ListPath, Scope())).Length, "GroupUser.delete: the record is gone");
        }
    }
}