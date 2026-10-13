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

    // The record contract of BundleNotification, generated from the schema: each case sends one request through the
    // SDK's BundleNotifications operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class BundleNotificationRecordContractTest : ContractCase
    {
        private const string ListPath = "/bundle_notifications";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("BundleNotification", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["bundle_id"] = (Int64)7, ["notify_user_id"] = (Int64)7, ["notify_on_registration"] = true, ["notify_on_upload"] = true })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "BundleNotification", ("bundle_id", "int64"), ("id", "int64"), ("notify_on_registration", "boolean"), ("notify_on_upload", "boolean"), ("notify_current_user", "boolean"), ("notify_user_id", "int64"), ("workspace_id", "int64"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the BundleNotification the SDK returns gives it.
        private static void AssertModel(JsonObject record, BundleNotification model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no BundleNotification");
            Simulator.Same(record, "bundle_id", model.BundleId, what);
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "notify_on_registration", model.NotifyOnRegistration, what);
            Simulator.Same(record, "notify_on_upload", model.NotifyOnUpload, what);
            Simulator.Same(record, "notify_current_user", model.NotifyCurrentUser, what);
            Simulator.Same(record, "notify_user_id", model.NotifyUserId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("BundleNotification", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["bundle_id"] = (Int64)7, ["notify_user_id"] = (Int64)7, ["notify_on_registration"] = true, ["notify_on_upload"] = true };
            BundleNotification created = await Simulator.Sdk(operation).BundleNotifications.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["bundle_id"] = "number", ["notify_user_id"] = "number", ["notify_on_registration"] = "boolean", ["notify_on_upload"] = "boolean" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "BundleNotification.create", "bundle_id", "notify_user_id", "notify_on_registration", "notify_on_upload");
            AssertModel(record, created, "BundleNotification.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("BundleNotification", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            BundleNotification[] listed = (await Simulator.Sdk(operation).BundleNotifications.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"BundleNotification.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("BundleNotification", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            BundleNotification found = await Simulator.Sdk(operation).BundleNotifications.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "BundleNotification.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("BundleNotification", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["notify_on_registration"] = false;
            sent["notify_on_upload"] = false;
            BundleNotification updated = await Simulator.Sdk(operation).BundleNotifications.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["notify_on_registration"] = "boolean", ["notify_on_upload"] = "boolean" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "BundleNotification.update", "notify_on_registration", "notify_on_upload");
            AssertModel(now, updated, "BundleNotification.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("BundleNotification", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).BundleNotifications.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"BundleNotification.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}