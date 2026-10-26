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

    // The record contract of User, generated from the schema: each case sends one request through the
    // SDK's Users operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class UserRecordContractTest : ContractCase
    {
        private const string ListPath = "/users";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("User", "create", "users.create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["email"] = "contract-email", ["group_ids"] = "contract-group_ids", ["ai_assistant_personality_id"] = (Int64)7, ["allowed_ips"] = "contract-allowed_ips", ["attachments_permission"] = true, ["authenticate_until"] = "2030-01-02T03:04:05Z", ["authentication_method"] = "password", ["billing_permission"] = true, ["bypass_user_lifecycle_rules"] = true, ["bypass_site_allowed_ips"] = true, ["dav_permission"] = true, ["desktop_configuration_profile_id"] = (Int64)7, ["default_workspace_id"] = (Int64)7, ["disabled"] = true, ["filesystem_layout"] = "site_root", ["ftp_permission"] = true, ["header_text"] = "contract-header_text", ["integration_centric_profile_id"] = (Int64)7, ["language"] = "contract-language", ["notification_daily_send_time"] = (Int64)7, ["name"] = "contract-name", ["company"] = "contract-company", ["notes"] = "contract-notes", ["office_integration_enabled"] = true, ["partner_admin"] = true, ["partner_id"] = (Int64)7, ["password_validity_days"] = (Int64)7, ["primary_group_id"] = (Int64)7, ["readonly_site_admin"] = true, ["receive_admin_alerts"] = true, ["notify_on_all_site_warnings"] = true, ["notify_on_all_sso_failures"] = true, ["notify_on_all_user_security_events"] = true, ["notify_on_all_pending_work_failures"] = true, ["notify_on_all_siem_http_destination_failures"] = true, ["notify_on_all_sync_failures"] = true, ["notify_on_all_automation_failures"] = true, ["notify_on_all_expectation_failures"] = true, ["require_login_by"] = "2030-01-02T03:04:05Z", ["require_password_change"] = true, ["responsible_group_id"] = (Int64)7, ["responsible_user_id"] = (Int64)7, ["restapi_permission"] = true, ["s3_compatible_endpoint_permission"] = true, ["self_managed"] = true, ["sftp_permission"] = true, ["site_admin"] = true, ["skip_welcome_screen"] = true, ["ssl_required"] = "use_system_setting", ["sso_strategy_id"] = (Int64)7, ["require_2fa"] = "use_system_setting", ["tags"] = "contract-tags", ["time_zone"] = "contract-time_zone", ["user_root"] = "contract-user_root", ["user_home"] = "contract-user_home", ["workspace_admin"] = true, ["username"] = "contract-username", ["workspace_id"] = (Int64)7 })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "User", ("id", "int64"), ("username", "string"), ("admin_group_ids", "array(int64)"), ("allowed_ips", "string"), ("attachments_permission", "boolean"), ("api_keys_count", "int64"), ("authenticate_until", "date-time"), ("authentication_method", "string"), ("avatar_url", "string"), ("billable", "boolean"), ("billing_permission", "boolean"), ("bypass_site_allowed_ips", "boolean"), ("bypass_user_lifecycle_rules", "boolean"), ("created_at", "date-time"), ("dav_permission", "boolean"), ("disabled", "boolean"), ("disabled_expired_or_inactive", "boolean"), ("ai_assistant_personality_id", "int64"), ("desktop_configuration_profile_id", "int64"), ("integration_centric_profile_id", "int64"), ("email", "email"), ("filesystem_layout", "string"), ("first_login_at", "date-time"), ("ftp_permission", "boolean"), ("group_ids", "string"), ("header_text", "string"), ("language", "string"), ("last_login_at", "date-time"), ("last_web_login_at", "date-time"), ("last_ftp_login_at", "date-time"), ("last_sftp_login_at", "date-time"), ("last_dav_login_at", "date-time"), ("last_desktop_login_at", "date-time"), ("last_restapi_login_at", "date-time"), ("last_api_use_at", "date-time"), ("last_active_at", "date-time"), ("last_protocol_cipher", "string"), ("lockout_expires", "date-time"), ("name", "string"), ("company", "string"), ("notes", "string"), ("notification_daily_send_time", "int64"), ("office_integration_enabled", "boolean"), ("partner_admin", "boolean"), ("partner_id", "int64"), ("partner_name", "string"), ("password_set_at", "date-time"), ("password_validity_days", "int64"), ("primary_group_id", "int64"), ("public_keys_count", "int64"), ("receive_admin_alerts", "boolean"), ("notify_on_all_site_warnings", "boolean"), ("notify_on_all_sso_failures", "boolean"), ("notify_on_all_user_security_events", "boolean"), ("notify_on_all_pending_work_failures", "boolean"), ("notify_on_all_siem_http_destination_failures", "boolean"), ("notify_on_all_sync_failures", "boolean"), ("notify_on_all_automation_failures", "boolean"), ("notify_on_all_expectation_failures", "boolean"), ("require_2fa", "string"), ("require_login_by", "date-time"), ("active_2fa", "boolean"), ("require_password_change", "boolean"), ("password_expired", "boolean"), ("responsible_group_id", "int64"), ("responsible_user_id", "int64"), ("readonly_site_admin", "boolean"), ("restapi_permission", "boolean"), ("s3_compatible_endpoint_permission", "boolean"), ("self_managed", "boolean"), ("sftp_permission", "boolean"), ("site_admin", "boolean"), ("workspace_admin", "boolean"), ("site_id", "int64"), ("workspace_id", "int64"), ("default_workspace_id", "int64"), ("skip_welcome_screen", "boolean"), ("ssl_required", "string"), ("sso_strategy_id", "int64"), ("externally_managed", "boolean"), ("tags", "string"), ("time_zone", "string"), ("type_of_2fa", "string"), ("type_of_2fa_for_display", "string"), ("user_root", "string"), ("user_home", "string"), ("days_remaining_until_password_expire", "int64"), ("password_expire_at", "date-time"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the User the SDK returns gives it.
        private static void AssertModel(JsonObject record, User model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no User");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "username", model.Username, what);
            Simulator.Same(record, "admin_group_ids", model.AdminGroupIds, what);
            Simulator.Same(record, "allowed_ips", model.AllowedIps, what);
            Simulator.Same(record, "attachments_permission", model.AttachmentsPermission, what);
            Simulator.Same(record, "api_keys_count", model.ApiKeysCount, what);
            Simulator.Same(record, "authenticate_until", model.AuthenticateUntil, what);
            Simulator.Same(record, "authentication_method", model.AuthenticationMethod, what);
            Simulator.Same(record, "avatar_url", model.AvatarUrl, what);
            Simulator.Same(record, "billable", model.Billable, what);
            Simulator.Same(record, "billing_permission", model.BillingPermission, what);
            Simulator.Same(record, "bypass_site_allowed_ips", model.BypassSiteAllowedIps, what);
            Simulator.Same(record, "bypass_user_lifecycle_rules", model.BypassUserLifecycleRules, what);
            Simulator.Same(record, "created_at", model.CreatedAt, what);
            Simulator.Same(record, "dav_permission", model.DavPermission, what);
            Simulator.Same(record, "disabled", model.Disabled, what);
            Simulator.Same(record, "disabled_expired_or_inactive", model.DisabledExpiredOrInactive, what);
            Simulator.Same(record, "ai_assistant_personality_id", model.AiAssistantPersonalityId, what);
            Simulator.Same(record, "desktop_configuration_profile_id", model.DesktopConfigurationProfileId, what);
            Simulator.Same(record, "integration_centric_profile_id", model.IntegrationCentricProfileId, what);
            Simulator.Same(record, "email", model.Email, what);
            Simulator.Same(record, "filesystem_layout", model.FilesystemLayout, what);
            Simulator.Same(record, "first_login_at", model.FirstLoginAt, what);
            Simulator.Same(record, "ftp_permission", model.FtpPermission, what);
            Simulator.Same(record, "group_ids", model.GroupIds, what);
            Simulator.Same(record, "header_text", model.HeaderText, what);
            Simulator.Same(record, "language", model.Language, what);
            Simulator.Same(record, "last_login_at", model.LastLoginAt, what);
            Simulator.Same(record, "last_web_login_at", model.LastWebLoginAt, what);
            Simulator.Same(record, "last_ftp_login_at", model.LastFtpLoginAt, what);
            Simulator.Same(record, "last_sftp_login_at", model.LastSftpLoginAt, what);
            Simulator.Same(record, "last_dav_login_at", model.LastDavLoginAt, what);
            Simulator.Same(record, "last_desktop_login_at", model.LastDesktopLoginAt, what);
            Simulator.Same(record, "last_restapi_login_at", model.LastRestapiLoginAt, what);
            Simulator.Same(record, "last_api_use_at", model.LastApiUseAt, what);
            Simulator.Same(record, "last_active_at", model.LastActiveAt, what);
            Simulator.Same(record, "last_protocol_cipher", model.LastProtocolCipher, what);
            Simulator.Same(record, "lockout_expires", model.LockoutExpires, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "company", model.Company, what);
            Simulator.Same(record, "notes", model.Notes, what);
            Simulator.Same(record, "notification_daily_send_time", model.NotificationDailySendTime, what);
            Simulator.Same(record, "office_integration_enabled", model.OfficeIntegrationEnabled, what);
            Simulator.Same(record, "partner_admin", model.PartnerAdmin, what);
            Simulator.Same(record, "partner_id", model.PartnerId, what);
            Simulator.Same(record, "partner_name", model.PartnerName, what);
            Simulator.Same(record, "password_set_at", model.PasswordSetAt, what);
            Simulator.Same(record, "password_validity_days", model.PasswordValidityDays, what);
            Simulator.Same(record, "primary_group_id", model.PrimaryGroupId, what);
            Simulator.Same(record, "public_keys_count", model.PublicKeysCount, what);
            Simulator.Same(record, "receive_admin_alerts", model.ReceiveAdminAlerts, what);
            Simulator.Same(record, "notify_on_all_site_warnings", model.NotifyOnAllSiteWarnings, what);
            Simulator.Same(record, "notify_on_all_sso_failures", model.NotifyOnAllSsoFailures, what);
            Simulator.Same(record, "notify_on_all_user_security_events", model.NotifyOnAllUserSecurityEvents, what);
            Simulator.Same(record, "notify_on_all_pending_work_failures", model.NotifyOnAllPendingWorkFailures, what);
            Simulator.Same(record, "notify_on_all_siem_http_destination_failures", model.NotifyOnAllSiemHttpDestinationFailures, what);
            Simulator.Same(record, "notify_on_all_sync_failures", model.NotifyOnAllSyncFailures, what);
            Simulator.Same(record, "notify_on_all_automation_failures", model.NotifyOnAllAutomationFailures, what);
            Simulator.Same(record, "notify_on_all_expectation_failures", model.NotifyOnAllExpectationFailures, what);
            Simulator.Same(record, "require_2fa", model.Require2fa, what);
            Simulator.Same(record, "require_login_by", model.RequireLoginBy, what);
            Simulator.Same(record, "active_2fa", model.Active2fa, what);
            Simulator.Same(record, "require_password_change", model.RequirePasswordChange, what);
            Simulator.Same(record, "password_expired", model.PasswordExpired, what);
            Simulator.Same(record, "responsible_group_id", model.ResponsibleGroupId, what);
            Simulator.Same(record, "responsible_user_id", model.ResponsibleUserId, what);
            Simulator.Same(record, "readonly_site_admin", model.ReadonlySiteAdmin, what);
            Simulator.Same(record, "restapi_permission", model.RestapiPermission, what);
            Simulator.Same(record, "s3_compatible_endpoint_permission", model.S3CompatibleEndpointPermission, what);
            Simulator.Same(record, "self_managed", model.SelfManaged, what);
            Simulator.Same(record, "sftp_permission", model.SftpPermission, what);
            Simulator.Same(record, "site_admin", model.SiteAdmin, what);
            Simulator.Same(record, "workspace_admin", model.WorkspaceAdmin, what);
            Simulator.Same(record, "site_id", model.SiteId, what);
            Simulator.Same(record, "workspace_id", model.WorkspaceId, what);
            Simulator.Same(record, "default_workspace_id", model.DefaultWorkspaceId, what);
            Simulator.Same(record, "skip_welcome_screen", model.SkipWelcomeScreen, what);
            Simulator.Same(record, "ssl_required", model.SslRequired, what);
            Simulator.Same(record, "sso_strategy_id", model.SsoStrategyId, what);
            Simulator.Same(record, "externally_managed", model.ExternallyManaged, what);
            Simulator.Same(record, "tags", model.Tags, what);
            Simulator.Same(record, "time_zone", model.TimeZone, what);
            Simulator.Same(record, "type_of_2fa", model.TypeOf2fa, what);
            Simulator.Same(record, "type_of_2fa_for_display", model.TypeOf2faForDisplay, what);
            Simulator.Same(record, "user_root", model.UserRoot, what);
            Simulator.Same(record, "user_home", model.UserHome, what);
            Simulator.Same(record, "days_remaining_until_password_expire", model.DaysRemainingUntilPasswordExpire, what);
            Simulator.Same(record, "password_expire_at", model.PasswordExpireAt, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("User", "create", "users.create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["email"] = "contract-email", ["group_ids"] = "contract-group_ids", ["ai_assistant_personality_id"] = (Int64)7, ["allowed_ips"] = "contract-allowed_ips", ["attachments_permission"] = true, ["authenticate_until"] = "2030-01-02T03:04:05Z", ["authentication_method"] = "password", ["billing_permission"] = true, ["bypass_user_lifecycle_rules"] = true, ["bypass_site_allowed_ips"] = true, ["dav_permission"] = true, ["desktop_configuration_profile_id"] = (Int64)7, ["default_workspace_id"] = (Int64)7, ["disabled"] = true, ["filesystem_layout"] = "site_root", ["ftp_permission"] = true, ["header_text"] = "contract-header_text", ["integration_centric_profile_id"] = (Int64)7, ["language"] = "contract-language", ["notification_daily_send_time"] = (Int64)7, ["name"] = "contract-name", ["company"] = "contract-company", ["notes"] = "contract-notes", ["office_integration_enabled"] = true, ["partner_admin"] = true, ["partner_id"] = (Int64)7, ["password_validity_days"] = (Int64)7, ["primary_group_id"] = (Int64)7, ["readonly_site_admin"] = true, ["receive_admin_alerts"] = true, ["notify_on_all_site_warnings"] = true, ["notify_on_all_sso_failures"] = true, ["notify_on_all_user_security_events"] = true, ["notify_on_all_pending_work_failures"] = true, ["notify_on_all_siem_http_destination_failures"] = true, ["notify_on_all_sync_failures"] = true, ["notify_on_all_automation_failures"] = true, ["notify_on_all_expectation_failures"] = true, ["require_login_by"] = "2030-01-02T03:04:05Z", ["require_password_change"] = true, ["responsible_group_id"] = (Int64)7, ["responsible_user_id"] = (Int64)7, ["restapi_permission"] = true, ["s3_compatible_endpoint_permission"] = true, ["self_managed"] = true, ["sftp_permission"] = true, ["site_admin"] = true, ["skip_welcome_screen"] = true, ["ssl_required"] = "use_system_setting", ["sso_strategy_id"] = (Int64)7, ["require_2fa"] = "use_system_setting", ["tags"] = "contract-tags", ["time_zone"] = "contract-time_zone", ["user_root"] = "contract-user_root", ["user_home"] = "contract-user_home", ["workspace_admin"] = true, ["username"] = "contract-username", ["workspace_id"] = (Int64)7 };
            User created = await Simulator.Sdk(operation).Users.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["email"] = "string", ["group_ids"] = "string", ["ai_assistant_personality_id"] = "number", ["allowed_ips"] = "string", ["attachments_permission"] = "boolean", ["authenticate_until"] = "string", ["authentication_method"] = "string", ["billing_permission"] = "boolean", ["bypass_user_lifecycle_rules"] = "boolean", ["bypass_site_allowed_ips"] = "boolean", ["dav_permission"] = "boolean", ["desktop_configuration_profile_id"] = "number", ["default_workspace_id"] = "number", ["disabled"] = "boolean", ["filesystem_layout"] = "string", ["ftp_permission"] = "boolean", ["header_text"] = "string", ["integration_centric_profile_id"] = "number", ["language"] = "string", ["notification_daily_send_time"] = "number", ["name"] = "string", ["company"] = "string", ["notes"] = "string", ["office_integration_enabled"] = "boolean", ["partner_admin"] = "boolean", ["partner_id"] = "number", ["password_validity_days"] = "number", ["primary_group_id"] = "number", ["readonly_site_admin"] = "boolean", ["receive_admin_alerts"] = "boolean", ["notify_on_all_site_warnings"] = "boolean", ["notify_on_all_sso_failures"] = "boolean", ["notify_on_all_user_security_events"] = "boolean", ["notify_on_all_pending_work_failures"] = "boolean", ["notify_on_all_siem_http_destination_failures"] = "boolean", ["notify_on_all_sync_failures"] = "boolean", ["notify_on_all_automation_failures"] = "boolean", ["notify_on_all_expectation_failures"] = "boolean", ["require_login_by"] = "string", ["require_password_change"] = "boolean", ["responsible_group_id"] = "number", ["responsible_user_id"] = "number", ["restapi_permission"] = "boolean", ["s3_compatible_endpoint_permission"] = "boolean", ["self_managed"] = "boolean", ["sftp_permission"] = "boolean", ["site_admin"] = "boolean", ["skip_welcome_screen"] = "boolean", ["ssl_required"] = "string", ["sso_strategy_id"] = "number", ["require_2fa"] = "string", ["tags"] = "string", ["time_zone"] = "string", ["user_root"] = "string", ["user_home"] = "string", ["workspace_admin"] = "boolean", ["username"] = "string", ["workspace_id"] = "number" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "User.create", "email", "group_ids", "ai_assistant_personality_id", "allowed_ips", "attachments_permission", "authenticate_until", "authentication_method", "billing_permission", "bypass_user_lifecycle_rules", "bypass_site_allowed_ips", "dav_permission", "desktop_configuration_profile_id", "default_workspace_id", "disabled", "filesystem_layout", "ftp_permission", "header_text", "integration_centric_profile_id", "language", "notification_daily_send_time", "name", "company", "notes", "office_integration_enabled", "partner_admin", "partner_id", "password_validity_days", "primary_group_id", "readonly_site_admin", "receive_admin_alerts", "notify_on_all_site_warnings", "notify_on_all_sso_failures", "notify_on_all_user_security_events", "notify_on_all_pending_work_failures", "notify_on_all_siem_http_destination_failures", "notify_on_all_sync_failures", "notify_on_all_automation_failures", "notify_on_all_expectation_failures", "require_login_by", "require_password_change", "responsible_group_id", "responsible_user_id", "restapi_permission", "s3_compatible_endpoint_permission", "self_managed", "sftp_permission", "site_admin", "skip_welcome_screen", "ssl_required", "sso_strategy_id", "require_2fa", "tags", "time_zone", "user_root", "user_home", "workspace_admin", "username", "workspace_id");
            AssertModel(record, created, "User.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("User", "list", "users.list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            User[] listed = (await Simulator.Sdk(operation).Users.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"User.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("User", "find", "users.find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            User found = await Simulator.Sdk(operation).Users.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "User.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("User", "update", "users.update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["email"] = "contract-email-2";
            sent["group_ids"] = "contract-group_ids-2";
            sent["ai_assistant_personality_id"] = (Int64)8;
            sent["allowed_ips"] = "contract-allowed_ips-2";
            sent["attachments_permission"] = false;
            sent["authenticate_until"] = "2031-02-03T04:05:06Z";
            sent["authentication_method"] = "sso";
            sent["billing_permission"] = false;
            sent["bypass_user_lifecycle_rules"] = false;
            sent["bypass_site_allowed_ips"] = false;
            sent["dav_permission"] = false;
            sent["desktop_configuration_profile_id"] = (Int64)8;
            sent["default_workspace_id"] = (Int64)8;
            sent["disabled"] = false;
            sent["filesystem_layout"] = "user_root";
            sent["ftp_permission"] = false;
            sent["header_text"] = "contract-header_text-2";
            sent["integration_centric_profile_id"] = (Int64)8;
            sent["language"] = "contract-language-2";
            sent["notification_daily_send_time"] = (Int64)8;
            sent["name"] = "contract-name-2";
            sent["company"] = "contract-company-2";
            sent["notes"] = "contract-notes-2";
            sent["office_integration_enabled"] = false;
            sent["partner_admin"] = false;
            sent["partner_id"] = (Int64)8;
            sent["password_validity_days"] = (Int64)8;
            sent["primary_group_id"] = (Int64)8;
            sent["readonly_site_admin"] = false;
            sent["receive_admin_alerts"] = false;
            sent["notify_on_all_site_warnings"] = false;
            sent["notify_on_all_sso_failures"] = false;
            sent["notify_on_all_user_security_events"] = false;
            sent["notify_on_all_pending_work_failures"] = false;
            sent["notify_on_all_siem_http_destination_failures"] = false;
            sent["notify_on_all_sync_failures"] = false;
            sent["notify_on_all_automation_failures"] = false;
            sent["notify_on_all_expectation_failures"] = false;
            sent["require_login_by"] = "2031-02-03T04:05:06Z";
            sent["require_password_change"] = false;
            sent["responsible_group_id"] = (Int64)8;
            sent["responsible_user_id"] = (Int64)8;
            sent["restapi_permission"] = false;
            sent["s3_compatible_endpoint_permission"] = false;
            sent["self_managed"] = false;
            sent["sftp_permission"] = false;
            sent["site_admin"] = false;
            sent["skip_welcome_screen"] = false;
            sent["ssl_required"] = "always_require";
            sent["sso_strategy_id"] = (Int64)8;
            sent["require_2fa"] = "always_require";
            sent["tags"] = "contract-tags-2";
            sent["time_zone"] = "contract-time_zone-2";
            sent["user_root"] = "contract-user_root-2";
            sent["user_home"] = "contract-user_home-2";
            sent["workspace_admin"] = false;
            sent["username"] = "contract-username-2";
            sent["workspace_id"] = (Int64)8;
            User updated = await Simulator.Sdk(operation).Users.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["email"] = "string", ["group_ids"] = "string", ["ai_assistant_personality_id"] = "number", ["allowed_ips"] = "string", ["attachments_permission"] = "boolean", ["authenticate_until"] = "string", ["authentication_method"] = "string", ["billing_permission"] = "boolean", ["bypass_user_lifecycle_rules"] = "boolean", ["bypass_site_allowed_ips"] = "boolean", ["dav_permission"] = "boolean", ["desktop_configuration_profile_id"] = "number", ["default_workspace_id"] = "number", ["disabled"] = "boolean", ["filesystem_layout"] = "string", ["ftp_permission"] = "boolean", ["header_text"] = "string", ["integration_centric_profile_id"] = "number", ["language"] = "string", ["notification_daily_send_time"] = "number", ["name"] = "string", ["company"] = "string", ["notes"] = "string", ["office_integration_enabled"] = "boolean", ["partner_admin"] = "boolean", ["partner_id"] = "number", ["password_validity_days"] = "number", ["primary_group_id"] = "number", ["readonly_site_admin"] = "boolean", ["receive_admin_alerts"] = "boolean", ["notify_on_all_site_warnings"] = "boolean", ["notify_on_all_sso_failures"] = "boolean", ["notify_on_all_user_security_events"] = "boolean", ["notify_on_all_pending_work_failures"] = "boolean", ["notify_on_all_siem_http_destination_failures"] = "boolean", ["notify_on_all_sync_failures"] = "boolean", ["notify_on_all_automation_failures"] = "boolean", ["notify_on_all_expectation_failures"] = "boolean", ["require_login_by"] = "string", ["require_password_change"] = "boolean", ["responsible_group_id"] = "number", ["responsible_user_id"] = "number", ["restapi_permission"] = "boolean", ["s3_compatible_endpoint_permission"] = "boolean", ["self_managed"] = "boolean", ["sftp_permission"] = "boolean", ["site_admin"] = "boolean", ["skip_welcome_screen"] = "boolean", ["ssl_required"] = "string", ["sso_strategy_id"] = "number", ["require_2fa"] = "string", ["tags"] = "string", ["time_zone"] = "string", ["user_root"] = "string", ["user_home"] = "string", ["workspace_admin"] = "boolean", ["username"] = "string", ["workspace_id"] = "number" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "User.update", "email", "group_ids", "ai_assistant_personality_id", "allowed_ips", "attachments_permission", "authenticate_until", "authentication_method", "billing_permission", "bypass_user_lifecycle_rules", "bypass_site_allowed_ips", "dav_permission", "desktop_configuration_profile_id", "default_workspace_id", "disabled", "filesystem_layout", "ftp_permission", "header_text", "integration_centric_profile_id", "language", "notification_daily_send_time", "name", "company", "notes", "office_integration_enabled", "partner_admin", "partner_id", "password_validity_days", "primary_group_id", "readonly_site_admin", "receive_admin_alerts", "notify_on_all_site_warnings", "notify_on_all_sso_failures", "notify_on_all_user_security_events", "notify_on_all_pending_work_failures", "notify_on_all_siem_http_destination_failures", "notify_on_all_sync_failures", "notify_on_all_automation_failures", "notify_on_all_expectation_failures", "require_login_by", "require_password_change", "responsible_group_id", "responsible_user_id", "restapi_permission", "s3_compatible_endpoint_permission", "self_managed", "sftp_permission", "site_admin", "skip_welcome_screen", "ssl_required", "sso_strategy_id", "require_2fa", "tags", "time_zone", "user_root", "user_home", "workspace_admin", "username", "workspace_id");
            AssertModel(now, updated, "User.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("User", "delete", "users.delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).Users.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"User.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}