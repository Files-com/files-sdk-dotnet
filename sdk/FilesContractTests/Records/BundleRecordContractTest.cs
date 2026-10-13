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

    // The record contract of Bundle, generated from the schema: each case sends one request through the
    // SDK's Bundles operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class BundleRecordContractTest : ContractCase
    {
        private const string ListPath = "/bundles";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Bundle", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["user_id"] = (Int64)7, ["paths"] = new string[] { "a" }, ["bypasses_site_expiration_rules"] = true, ["dont_separate_submissions_by_folder"] = true, ["expires_at"] = "2030-01-02T03:04:05Z", ["max_uses"] = (Int64)7, ["group_id"] = (Int64)7, ["internal_name"] = "contract-internal_name", ["description"] = "contract-description", ["note"] = "contract-note", ["code"] = "contract-code", ["path_template"] = "contract-path_template", ["path_template_time_zone"] = "contract-path_template_time_zone", ["permissions"] = "read", ["require_registration"] = true, ["clickwrap_id"] = (Int64)7, ["inbox_id"] = (Int64)7, ["require_share_recipient"] = true, ["send_one_time_password_to_recipient_at_registration"] = true, ["send_email_receipt_to_uploader"] = true, ["skip_email"] = true, ["skip_name"] = true, ["skip_company"] = true, ["start_access_on_date"] = "2030-01-02T03:04:05Z", ["snapshot_id"] = (Int64)7, ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Bundle", ("code", "string"), ("color_left", "string"), ("color_link", "string"), ("color_text", "string"), ("color_top", "string"), ("color_top_text", "string"), ("url", "string"), ("description", "string"), ("expires_at", "date-time"), ("password_protected", "boolean"), ("permissions", "string"), ("preview_only", "boolean"), ("require_registration", "boolean"), ("require_share_recipient", "boolean"), ("require_logout", "boolean"), ("clickwrap_body", "string"), ("skip_name", "boolean"), ("skip_email", "boolean"), ("start_access_on_date", "date-time"), ("skip_company", "boolean"), ("id", "int64"), ("bypasses_site_expiration_rules", "boolean"), ("created_at", "date-time"), ("deleted", "boolean"), ("deleted_at", "date-time"), ("dont_separate_submissions_by_folder", "boolean"), ("effective_expires_at", "date-time"), ("max_uses", "int64"), ("internal_name", "string"), ("note", "string"), ("path_template", "string"), ("path_template_time_zone", "string"), ("send_email_receipt_to_uploader", "boolean"), ("snapshot_id", "int64"), ("user_id", "int64"), ("username", "string"), ("group_id", "int64"), ("clickwrap_id", "int64"), ("inbox_id", "int64"), ("send_one_time_password_to_recipient_at_registration", "boolean"), ("workspace_id", "int64"), ("has_inbox", "boolean"), ("dont_allow_folders_in_uploads", "boolean"), ("paths", "array(string)"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Bundle the SDK returns gives it.
        private static void AssertModel(JsonObject record, Bundle model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Bundle");
            Simulator.Same(record, "code", model.Code, what);
            Simulator.Same(record, "color_left", model.ColorLeft, what);
            Simulator.Same(record, "color_link", model.ColorLink, what);
            Simulator.Same(record, "color_text", model.ColorText, what);
            Simulator.Same(record, "color_top", model.ColorTop, what);
            Simulator.Same(record, "color_top_text", model.ColorTopText, what);
            Simulator.Same(record, "url", model.Url, what);
            Simulator.Same(record, "description", model.Description, what);
            Simulator.Same(record, "expires_at", model.ExpiresAt, what);
            Simulator.Same(record, "password_protected", model.PasswordProtected, what);
            Simulator.Same(record, "permissions", model.Permissions, what);
            Simulator.Same(record, "preview_only", model.PreviewOnly, what);
            Simulator.Same(record, "require_registration", model.RequireRegistration, what);
            Simulator.Same(record, "require_share_recipient", model.RequireShareRecipient, what);
            Simulator.Same(record, "require_logout", model.RequireLogout, what);
            Simulator.Same(record, "clickwrap_body", model.ClickwrapBody, what);
            Simulator.Same(record, "skip_name", model.SkipName, what);
            Simulator.Same(record, "skip_email", model.SkipEmail, what);
            Simulator.Same(record, "start_access_on_date", model.StartAccessOnDate, what);
            Simulator.Same(record, "skip_company", model.SkipCompany, what);
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "bypasses_site_expiration_rules", model.BypassesSiteExpirationRules, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "deleted", model.Deleted, what);
            Simulator.Same(record, "deleted_at", model.DeletedAt, what);
            Simulator.Same(record, "dont_separate_submissions_by_folder", model.DontSeparateSubmissionsByFolder, what);
            Simulator.Same(record, "effective_expires_at", model.EffectiveExpiresAt, what);
            Simulator.Same(record, "max_uses", model.MaxUses, what);
            Simulator.Same(record, "internal_name", model.InternalName, what);
            Simulator.Same(record, "note", model.Note, what);
            Simulator.Same(record, "path_template", model.PathTemplate, what);
            Simulator.Same(record, "path_template_time_zone", model.PathTemplateTimeZone, what);
            Simulator.Same(record, "send_email_receipt_to_uploader", model.SendEmailReceiptToUploader, what);
            Simulator.Same(record, "snapshot_id", model.SnapshotId, what);
            Simulator.Same(record, "user_id", model.UserId, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "group_id", model.GroupId, what);
            Simulator.Same(record, "clickwrap_id", model.ClickwrapId, what);
            Simulator.Same(record, "inbox_id", model.InboxId, what);
            Simulator.Same(record, "send_one_time_password_to_recipient_at_registration", model.SendOneTimePasswordToRecipientAtRegistration, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "has_inbox", model.HasInbox, what);
            Simulator.Same(record, "dont_allow_folders_in_uploads", model.DontAllowFoldersInUploads, what);
            Simulator.Same(record, "paths", model.Paths, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Bundle", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["user_id"] = (Int64)7, ["paths"] = new string[] { "a" }, ["bypasses_site_expiration_rules"] = true, ["dont_separate_submissions_by_folder"] = true, ["expires_at"] = "2030-01-02T03:04:05Z", ["max_uses"] = (Int64)7, ["group_id"] = (Int64)7, ["internal_name"] = "contract-internal_name", ["description"] = "contract-description", ["note"] = "contract-note", ["code"] = "contract-code", ["path_template"] = "contract-path_template", ["path_template_time_zone"] = "contract-path_template_time_zone", ["permissions"] = "read", ["require_registration"] = true, ["clickwrap_id"] = (Int64)7, ["inbox_id"] = (Int64)7, ["require_share_recipient"] = true, ["send_one_time_password_to_recipient_at_registration"] = true, ["send_email_receipt_to_uploader"] = true, ["skip_email"] = true, ["skip_name"] = true, ["skip_company"] = true, ["start_access_on_date"] = "2030-01-02T03:04:05Z", ["snapshot_id"] = (Int64)7, ["workspace_id"] = (Int64)7 };
            Bundle created = await Simulator.Sdk(operation).Bundles.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["user_id"] = "number", ["paths"] = "array", ["bypasses_site_expiration_rules"] = "boolean", ["dont_separate_submissions_by_folder"] = "boolean", ["expires_at"] = "string", ["max_uses"] = "number", ["group_id"] = "number", ["internal_name"] = "string", ["description"] = "string", ["note"] = "string", ["code"] = "string", ["path_template"] = "string", ["path_template_time_zone"] = "string", ["permissions"] = "string", ["require_registration"] = "boolean", ["clickwrap_id"] = "number", ["inbox_id"] = "number", ["require_share_recipient"] = "boolean", ["send_one_time_password_to_recipient_at_registration"] = "boolean", ["send_email_receipt_to_uploader"] = "boolean", ["skip_email"] = "boolean", ["skip_name"] = "boolean", ["skip_company"] = "boolean", ["start_access_on_date"] = "string", ["snapshot_id"] = "number", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Bundle.create", "user_id", "paths", "bypasses_site_expiration_rules", "dont_separate_submissions_by_folder", "expires_at", "max_uses", "group_id", "internal_name", "description", "note", "code", "path_template", "path_template_time_zone", "permissions", "require_registration", "clickwrap_id", "inbox_id", "require_share_recipient", "send_one_time_password_to_recipient_at_registration", "send_email_receipt_to_uploader", "skip_email", "skip_name", "skip_company", "start_access_on_date", "snapshot_id", "workspace_id");
            AssertModel(record, created, "Bundle.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Bundle", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Bundle[] listed = (await Simulator.Sdk(operation).Bundles.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Bundle.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Bundle", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Bundle found = await Simulator.Sdk(operation).Bundles.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Bundle.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Bundle", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["paths"] = new string[] { "b" };
            sent["bypasses_site_expiration_rules"] = false;
            sent["clickwrap_id"] = (Int64)8;
            sent["code"] = "contract-code-2";
            sent["description"] = "contract-description-2";
            sent["dont_separate_submissions_by_folder"] = false;
            sent["expires_at"] = "2031-02-03T04:05:06Z";
            sent["inbox_id"] = (Int64)8;
            sent["max_uses"] = (Int64)8;
            sent["group_id"] = (Int64)8;
            sent["internal_name"] = "contract-internal_name-2";
            sent["note"] = "contract-note-2";
            sent["path_template"] = "contract-path_template-2";
            sent["path_template_time_zone"] = "contract-path_template_time_zone-2";
            sent["permissions"] = "write";
            sent["require_registration"] = false;
            sent["require_share_recipient"] = false;
            sent["send_one_time_password_to_recipient_at_registration"] = false;
            sent["send_email_receipt_to_uploader"] = false;
            sent["skip_company"] = false;
            sent["start_access_on_date"] = "2031-02-03T04:05:06Z";
            sent["skip_email"] = false;
            sent["skip_name"] = false;
            sent["user_id"] = (Int64)8;
            sent["workspace_id"] = (Int64)8;
            Bundle updated = await Simulator.Sdk(operation).Bundles.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["paths"] = "array", ["bypasses_site_expiration_rules"] = "boolean", ["clickwrap_id"] = "number", ["code"] = "string", ["description"] = "string", ["dont_separate_submissions_by_folder"] = "boolean", ["expires_at"] = "string", ["inbox_id"] = "number", ["max_uses"] = "number", ["group_id"] = "number", ["internal_name"] = "string", ["note"] = "string", ["path_template"] = "string", ["path_template_time_zone"] = "string", ["permissions"] = "string", ["require_registration"] = "boolean", ["require_share_recipient"] = "boolean", ["send_one_time_password_to_recipient_at_registration"] = "boolean", ["send_email_receipt_to_uploader"] = "boolean", ["skip_company"] = "boolean", ["start_access_on_date"] = "string", ["skip_email"] = "boolean", ["skip_name"] = "boolean", ["user_id"] = "number", ["workspace_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Bundle.update", "paths", "bypasses_site_expiration_rules", "clickwrap_id", "code", "description", "dont_separate_submissions_by_folder", "expires_at", "inbox_id", "max_uses", "group_id", "internal_name", "note", "path_template", "path_template_time_zone", "permissions", "require_registration", "require_share_recipient", "send_one_time_password_to_recipient_at_registration", "send_email_receipt_to_uploader", "skip_company", "start_access_on_date", "skip_email", "skip_name", "user_id", "workspace_id");
            AssertModel(now, updated, "Bundle.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Bundle", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Bundles.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Bundle.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}