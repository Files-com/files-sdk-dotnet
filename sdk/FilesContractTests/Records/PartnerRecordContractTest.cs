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

    // The record contract of Partner, generated from the schema: each case sends one request through the
    // SDK's Partners operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class PartnerRecordContractTest : ContractCase
    {
        private const string ListPath = "/partners";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("Partner", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["ai_assistant_personality_id"] = (Int64)7, ["allowed_ips"] = "contract-allowed_ips", ["allow_bypassing_2fa_policies"] = true, ["allow_credential_changes"] = true, ["allow_providing_gpg_keys"] = true, ["allow_user_creation"] = true, ["cc_emails_to_responsible_party"] = true, ["notes"] = "contract-notes", ["partner_channel_template_id"] = (Int64)7, ["responsible_group_id"] = (Int64)7, ["responsible_user_id"] = (Int64)7, ["show_partner_channel_home_page"] = true, ["tags"] = "contract-tags", ["name"] = "contract-name", ["root_folder"] = "contract-root_folder", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "Partner", ("allow_bypassing_2fa_policies", "boolean"), ("allowed_ips", "string"), ("allow_credential_changes", "boolean"), ("allow_providing_gpg_keys", "boolean"), ("allow_user_creation", "boolean"), ("cc_emails_to_responsible_party", "boolean"), ("id", "int64"), ("ai_assistant_personality_id", "int64"), ("workspace_id", "int64"), ("name", "string"), ("notes", "string"), ("partner_admin_ids", "array(int64)"), ("partner_channel_template_id", "int64"), ("partnership_role", "string"), ("responsible_group_id", "int64"), ("responsible_user_id", "int64"), ("root_folder", "string"), ("show_partner_channel_home_page", "boolean"), ("tags", "string"), ("user_ids", "array(int64)"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the Partner the SDK returns gives it.
        private static void AssertModel(JsonObject record, Partner model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no Partner");
            Simulator.Same(record, "allow_bypassing_2fa_policies", model.AllowBypassing2faPolicies, what);
            Simulator.Same(record, "allowed_ips", model.AllowedIps, what);
            Simulator.Same(record, "allow_credential_changes", model.AllowCredentialChanges, what);
            Simulator.Same(record, "allow_providing_gpg_keys", model.AllowProvidingGpgKeys, what);
            Simulator.Same(record, "allow_user_creation", model.AllowUserCreation, what);
            Simulator.Same(record, "cc_emails_to_responsible_party", model.CcEmailsToResponsibleParty, what);
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "ai_assistant_personality_id", model.AiAssistantPersonalityId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "notes", model.Notes, what);
            Simulator.Same(record, "partner_admin_ids", model.PartnerAdminIds, what);
            Simulator.Same(record, "partner_channel_template_id", model.PartnerChannelTemplateId, what);
            Simulator.Same(record, "partnership_role", model.PartnershipRole, what);
            Simulator.Same(record, "responsible_group_id", model.ResponsibleGroupId, what);
            Simulator.Same(record, "responsible_user_id", model.ResponsibleUserId, what);
            Simulator.Same(record, "root_folder", model.RootFolder, what);
            Simulator.Same(record, "show_partner_channel_home_page", model.ShowPartnerChannelHomePage, what);
            Simulator.Same(record, "tags", model.Tags, what);
            Simulator.Same(record, "user_ids", model.UserIds, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("Partner", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["ai_assistant_personality_id"] = (Int64)7, ["allowed_ips"] = "contract-allowed_ips", ["allow_bypassing_2fa_policies"] = true, ["allow_credential_changes"] = true, ["allow_providing_gpg_keys"] = true, ["allow_user_creation"] = true, ["cc_emails_to_responsible_party"] = true, ["notes"] = "contract-notes", ["partner_channel_template_id"] = (Int64)7, ["responsible_group_id"] = (Int64)7, ["responsible_user_id"] = (Int64)7, ["show_partner_channel_home_page"] = true, ["tags"] = "contract-tags", ["name"] = "contract-name", ["root_folder"] = "contract-root_folder", ["workspace_id"] = (Int64)7 };
            Partner created = await Simulator.Sdk(operation).Partners.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["ai_assistant_personality_id"] = "number", ["allowed_ips"] = "string", ["allow_bypassing_2fa_policies"] = "boolean", ["allow_credential_changes"] = "boolean", ["allow_providing_gpg_keys"] = "boolean", ["allow_user_creation"] = "boolean", ["cc_emails_to_responsible_party"] = "boolean", ["notes"] = "string", ["partner_channel_template_id"] = "number", ["responsible_group_id"] = "number", ["responsible_user_id"] = "number", ["show_partner_channel_home_page"] = "boolean", ["tags"] = "string", ["name"] = "string", ["root_folder"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "Partner.create", "ai_assistant_personality_id", "allowed_ips", "allow_bypassing_2fa_policies", "allow_credential_changes", "allow_providing_gpg_keys", "allow_user_creation", "cc_emails_to_responsible_party", "notes", "partner_channel_template_id", "responsible_group_id", "responsible_user_id", "show_partner_channel_home_page", "tags", "name", "root_folder", "workspace_id");
            AssertModel(record, created, "Partner.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("Partner", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Partner[] listed = (await Simulator.Sdk(operation).Partners.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"Partner.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("Partner", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            Partner found = await Simulator.Sdk(operation).Partners.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "Partner.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("Partner", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["ai_assistant_personality_id"] = (Int64)8;
            sent["allowed_ips"] = "contract-allowed_ips-2";
            sent["allow_bypassing_2fa_policies"] = false;
            sent["allow_credential_changes"] = false;
            sent["allow_providing_gpg_keys"] = false;
            sent["allow_user_creation"] = false;
            sent["cc_emails_to_responsible_party"] = false;
            sent["notes"] = "contract-notes-2";
            sent["partner_channel_template_id"] = (Int64)8;
            sent["responsible_group_id"] = (Int64)8;
            sent["responsible_user_id"] = (Int64)8;
            sent["show_partner_channel_home_page"] = false;
            sent["tags"] = "contract-tags-2";
            sent["name"] = "contract-name-2";
            sent["partnership_role"] = "guest";
            sent["root_folder"] = "contract-root_folder-2";
            Partner updated = await Simulator.Sdk(operation).Partners.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["ai_assistant_personality_id"] = "number", ["allowed_ips"] = "string", ["allow_bypassing_2fa_policies"] = "boolean", ["allow_credential_changes"] = "boolean", ["allow_providing_gpg_keys"] = "boolean", ["allow_user_creation"] = "boolean", ["cc_emails_to_responsible_party"] = "boolean", ["notes"] = "string", ["partner_channel_template_id"] = "number", ["responsible_group_id"] = "number", ["responsible_user_id"] = "number", ["show_partner_channel_home_page"] = "boolean", ["tags"] = "string", ["name"] = "string", ["partnership_role"] = "string", ["root_folder"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "Partner.update", "ai_assistant_personality_id", "allowed_ips", "allow_bypassing_2fa_policies", "allow_credential_changes", "allow_providing_gpg_keys", "allow_user_creation", "cc_emails_to_responsible_party", "notes", "partner_channel_template_id", "responsible_group_id", "responsible_user_id", "show_partner_channel_home_page", "tags", "name", "partnership_role", "root_folder");
            AssertModel(now, updated, "Partner.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("Partner", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Partners.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"Partner.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}