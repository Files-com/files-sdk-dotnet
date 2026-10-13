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

    // The record contract of Notification, generated from the schema: each case sends one request through the
    // SDK's Notifications operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class NotificationRecordContractTest : ContractCase
    {
        private const string ListPath = "/notifications";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Notification", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["user_id"] = (Int64)7, ["notify_on_copy"] = true, ["notify_on_delete"] = true, ["notify_on_download"] = true, ["notify_on_move"] = true, ["notify_on_upload"] = true, ["notify_user_actions"] = true, ["recursive"] = true, ["send_interval"] = "contract-send_interval", ["subject"] = "contract-subject", ["message"] = "contract-message", ["triggering_filenames"] = new string[] { "a" }, ["triggering_group_ids"] = new Nullable<Int64>[] { 3 }, ["triggering_user_ids"] = new Nullable<Int64>[] { 3 }, ["trigger_by_share_recipients"] = true, ["workspace_id"] = (Int64)7, ["group_id"] = (Int64)7, ["path"] = "contract-path", ["username"] = "contract-username" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Notification", ("id", "int64"), ("path", "string"), ("group_id", "int64"), ("group_name", "string"), ("group_ids", "array(int64)"), ("group_names", "array(string)"), ("triggering_group_ids", "array(int64)"), ("triggering_user_ids", "array(int64)"), ("trigger_by_share_recipients", "boolean"), ("notify_user_actions", "boolean"), ("notify_on_copy", "boolean"), ("notify_on_delete", "boolean"), ("notify_on_download", "boolean"), ("notify_on_move", "boolean"), ("notify_on_upload", "boolean"), ("recursive", "boolean"), ("send_interval", "string"), ("subject", "string"), ("message", "string"), ("triggering_filenames", "array(string)"), ("workspace_id", "int64"), ("unsubscribed", "boolean"), ("unsubscribed_reason", "string"), ("user_id", "int64"), ("username", "string"), ("suppressed_email", "boolean"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Notification the SDK returns gives it.
        private static void AssertModel(JsonObject record, Notification model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Notification");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "path", model.Path, what);
            Simulator.Same(record, "group_id", model.GroupId, what);
            Simulator.Same(record, "group_name", model.GroupName, what);
            Simulator.Same(record, "group_ids", model.GroupIds, what);
            Simulator.Same(record, "group_names", model.GroupNames, what);
            Simulator.Same(record, "triggering_group_ids", model.TriggeringGroupIds, what);
            Simulator.Same(record, "triggering_user_ids", model.TriggeringUserIds, what);
            Simulator.Same(record, "trigger_by_share_recipients", model.TriggerByShareRecipients, what);
            Simulator.Same(record, "notify_user_actions", model.NotifyUserActions, what);
            Simulator.Same(record, "notify_on_copy", model.NotifyOnCopy, what);
            Simulator.Same(record, "notify_on_delete", model.NotifyOnDelete, what);
            Simulator.Same(record, "notify_on_download", model.NotifyOnDownload, what);
            Simulator.Same(record, "notify_on_move", model.NotifyOnMove, what);
            Simulator.Same(record, "notify_on_upload", model.NotifyOnUpload, what);
            Simulator.Same(record, "recursive", model.Recursive, what);
            Simulator.Same(record, "send_interval", model.SendInterval, what);
            Simulator.Same(record, "subject", model.Subject, what);
            Simulator.Same(record, "message", model.Message, what);
            Simulator.Same(record, "triggering_filenames", model.TriggeringFilenames, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "unsubscribed", model.Unsubscribed, what);
            Simulator.Same(record, "unsubscribed_reason", model.UnsubscribedReason, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "suppressed_email", model.SuppressedEmail, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Notification", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["user_id"] = (Int64)7, ["notify_on_copy"] = true, ["notify_on_delete"] = true, ["notify_on_download"] = true, ["notify_on_move"] = true, ["notify_on_upload"] = true, ["notify_user_actions"] = true, ["recursive"] = true, ["send_interval"] = "contract-send_interval", ["subject"] = "contract-subject", ["message"] = "contract-message", ["triggering_filenames"] = new string[] { "a" }, ["triggering_group_ids"] = new Nullable<Int64>[] { 3 }, ["triggering_user_ids"] = new Nullable<Int64>[] { 3 }, ["trigger_by_share_recipients"] = true, ["workspace_id"] = (Int64)7, ["group_id"] = (Int64)7, ["path"] = "contract-path", ["username"] = "contract-username" };
            Notification created = await Simulator.Sdk(operation).Notifications.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["user_id"] = "number", ["notify_on_copy"] = "boolean", ["notify_on_delete"] = "boolean", ["notify_on_download"] = "boolean", ["notify_on_move"] = "boolean", ["notify_on_upload"] = "boolean", ["notify_user_actions"] = "boolean", ["recursive"] = "boolean", ["send_interval"] = "string", ["subject"] = "string", ["message"] = "string", ["triggering_filenames"] = "array", ["triggering_group_ids"] = "array", ["triggering_user_ids"] = "array", ["trigger_by_share_recipients"] = "boolean", ["workspace_id"] = "number", ["group_id"] = "number", ["path"] = "string", ["username"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Notification.create", "user_id", "notify_on_copy", "notify_on_delete", "notify_on_download", "notify_on_move", "notify_on_upload", "notify_user_actions", "recursive", "send_interval", "subject", "message", "triggering_filenames", "triggering_group_ids", "triggering_user_ids", "trigger_by_share_recipients", "workspace_id", "group_id", "path", "username");
            AssertModel(record, created, "Notification.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Notification", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Notification[] listed = (await Simulator.Sdk(operation).Notifications.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Notification.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Notification", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Notification found = await Simulator.Sdk(operation).Notifications.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Notification.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Notification", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["notify_on_copy"] = false;
            sent["notify_on_delete"] = false;
            sent["notify_on_download"] = false;
            sent["notify_on_move"] = false;
            sent["notify_on_upload"] = false;
            sent["notify_user_actions"] = false;
            sent["recursive"] = false;
            sent["send_interval"] = "contract-send_interval-2";
            sent["subject"] = "contract-subject-2";
            sent["message"] = "contract-message-2";
            sent["triggering_filenames"] = new string[] { "b" };
            sent["triggering_group_ids"] = new Nullable<Int64>[] { 4 };
            sent["triggering_user_ids"] = new Nullable<Int64>[] { 4 };
            sent["trigger_by_share_recipients"] = false;
            sent["workspace_id"] = (Int64)8;
            Notification updated = await Simulator.Sdk(operation).Notifications.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["notify_on_copy"] = "boolean", ["notify_on_delete"] = "boolean", ["notify_on_download"] = "boolean", ["notify_on_move"] = "boolean", ["notify_on_upload"] = "boolean", ["notify_user_actions"] = "boolean", ["recursive"] = "boolean", ["send_interval"] = "string", ["subject"] = "string", ["message"] = "string", ["triggering_filenames"] = "array", ["triggering_group_ids"] = "array", ["triggering_user_ids"] = "array", ["trigger_by_share_recipients"] = "boolean", ["workspace_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Notification.update", "notify_on_copy", "notify_on_delete", "notify_on_download", "notify_on_move", "notify_on_upload", "notify_user_actions", "recursive", "send_interval", "subject", "message", "triggering_filenames", "triggering_group_ids", "triggering_user_ids", "trigger_by_share_recipients", "workspace_id");
            AssertModel(now, updated, "Notification.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Notification", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Notifications.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Notification.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}