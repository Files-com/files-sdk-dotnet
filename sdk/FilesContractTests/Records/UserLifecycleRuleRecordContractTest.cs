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

    // The record contract of UserLifecycleRule, generated from the schema: each case sends one request through the
    // SDK's UserLifecycleRules operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class UserLifecycleRuleRecordContractTest : ContractCase
    {
        private const string ListPath = "/user_lifecycle_rules";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("UserLifecycleRule", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["action"] = "disable", ["apply_to_all_workspaces"] = true, ["authentication_method"] = "all", ["group_ids"] = new Nullable<Int64>[] { 3 }, ["inactivity_days"] = (Int64)7, ["include_site_admins"] = true, ["include_folder_admins"] = true, ["name"] = "contract-name", ["notify_users"] = true, ["partner_tag"] = "contract-partner_tag", ["user_state"] = "inactive", ["user_tag"] = "contract-user_tag", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "UserLifecycleRule", ("id", "int64"), ("authentication_method", "string"), ("group_ids", "array(int64)"), ("action", "string"), ("inactivity_days", "int64"), ("include_folder_admins", "boolean"), ("include_site_admins", "boolean"), ("apply_to_all_workspaces", "boolean"), ("name", "string"), ("notify_users", "boolean"), ("partner_tag", "string"), ("site_id", "int64"), ("workspace_id", "int64"), ("user_state", "string"), ("user_tag", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the UserLifecycleRule the SDK returns gives it.
        private static void AssertModel(JsonObject record, UserLifecycleRule model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no UserLifecycleRule");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "authentication_method", model.AuthenticationMethod, what);
            Simulator.Same(record, "group_ids", model.GroupIds, what);
            Simulator.Same(record, "action", model.Action, what);
            Simulator.Same(record, "inactivity_days", model.InactivityDays, what);
            Simulator.Same(record, "include_folder_admins", model.IncludeFolderAdmins, what);
            Simulator.Same(record, "include_site_admins", model.IncludeSiteAdmins, what);
            Simulator.Same(record, "apply_to_all_workspaces", model.ApplyToAllWorkspaces, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "notify_users", model.NotifyUsers, what);
            Simulator.Same(record, "partner_tag", model.PartnerTag, what);
            Simulator.Same(record, "site_id", model.SiteId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "user_state", model.UserState, what);
            Simulator.Same(record, "user_tag", model.UserTag, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("UserLifecycleRule", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["action"] = "disable", ["apply_to_all_workspaces"] = true, ["authentication_method"] = "all", ["group_ids"] = new Nullable<Int64>[] { 3 }, ["inactivity_days"] = (Int64)7, ["include_site_admins"] = true, ["include_folder_admins"] = true, ["name"] = "contract-name", ["notify_users"] = true, ["partner_tag"] = "contract-partner_tag", ["user_state"] = "inactive", ["user_tag"] = "contract-user_tag", ["workspace_id"] = (Int64)7 };
            UserLifecycleRule created = await Simulator.Sdk(operation).UserLifecycleRules.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["action"] = "string", ["apply_to_all_workspaces"] = "boolean", ["authentication_method"] = "string", ["group_ids"] = "array", ["inactivity_days"] = "number", ["include_site_admins"] = "boolean", ["include_folder_admins"] = "boolean", ["name"] = "string", ["notify_users"] = "boolean", ["partner_tag"] = "string", ["user_state"] = "string", ["user_tag"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "UserLifecycleRule.create", "action", "apply_to_all_workspaces", "authentication_method", "group_ids", "inactivity_days", "include_site_admins", "include_folder_admins", "name", "notify_users", "partner_tag", "user_state", "user_tag", "workspace_id");
            AssertModel(record, created, "UserLifecycleRule.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("UserLifecycleRule", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UserLifecycleRule[] listed = (await Simulator.Sdk(operation).UserLifecycleRules.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"UserLifecycleRule.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("UserLifecycleRule", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            UserLifecycleRule found = await Simulator.Sdk(operation).UserLifecycleRules.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "UserLifecycleRule.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("UserLifecycleRule", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["action"] = "delete";
            sent["apply_to_all_workspaces"] = false;
            sent["authentication_method"] = "password";
            sent["group_ids"] = new Nullable<Int64>[] { 4 };
            sent["inactivity_days"] = (Int64)8;
            sent["include_site_admins"] = false;
            sent["include_folder_admins"] = false;
            sent["name"] = "contract-name-2";
            sent["notify_users"] = false;
            sent["partner_tag"] = "contract-partner_tag-2";
            sent["user_state"] = "disabled";
            sent["user_tag"] = "contract-user_tag-2";
            sent["workspace_id"] = (Int64)8;
            UserLifecycleRule updated = await Simulator.Sdk(operation).UserLifecycleRules.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["action"] = "string", ["apply_to_all_workspaces"] = "boolean", ["authentication_method"] = "string", ["group_ids"] = "array", ["inactivity_days"] = "number", ["include_site_admins"] = "boolean", ["include_folder_admins"] = "boolean", ["name"] = "string", ["notify_users"] = "boolean", ["partner_tag"] = "string", ["user_state"] = "string", ["user_tag"] = "string", ["workspace_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "UserLifecycleRule.update", "action", "apply_to_all_workspaces", "authentication_method", "group_ids", "inactivity_days", "include_site_admins", "include_folder_admins", "name", "notify_users", "partner_tag", "user_state", "user_tag", "workspace_id");
            AssertModel(now, updated, "UserLifecycleRule.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("UserLifecycleRule", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).UserLifecycleRules.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"UserLifecycleRule.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}