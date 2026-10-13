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

    // The record contract of SiemHttpDestination, generated from the schema: each case sends one request through the
    // SDK's SiemHttpDestinations operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SiemHttpDestinationRecordContractTest : ContractCase
    {
        private const string ListPath = "/siem_http_destinations";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            await Simulator.RecordOperationAsync("SiemHttpDestination", "create");
            await Simulator.ResetAsync();
            List<JsonObject> made = new List<JsonObject>();
            for (int i = 0; i < count; i++)
            {
                made.Add(await Simulator.CreateRawAsync(ListPath, Simulator.Json(new Dictionary<string, object> { ["name"] = "contract-name", ["sending_active"] = true, ["generic_payload_type"] = "json_newline", ["file_destination_path"] = "contract-file_destination_path", ["file_format"] = "json", ["file_interval_minutes"] = (Int64)7, ["azure_dcr_immutable_id"] = "contract-azure_dcr_immutable_id", ["azure_stream_name"] = "contract-azure_stream_name", ["azure_oauth_client_credentials_tenant_id"] = "contract-azure_oauth_client_credentials_tenant_id", ["azure_oauth_client_credentials_client_id"] = "contract-azure_oauth_client_credentials_client_id", ["qradar_username"] = "contract-qradar_username", ["action_send_enabled"] = true, ["sftp_action_send_enabled"] = true, ["ftp_action_send_enabled"] = true, ["web_dav_action_send_enabled"] = true, ["sync_send_enabled"] = true, ["outbound_connection_send_enabled"] = true, ["automation_send_enabled"] = true, ["api_request_send_enabled"] = true, ["public_hosting_request_send_enabled"] = true, ["email_send_enabled"] = true, ["exavault_api_request_send_enabled"] = true, ["settings_change_send_enabled"] = true, ["destination_type"] = "generic", ["destination_url"] = "contract-destination_url" })));
            }
            return Conform(made.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "SiemHttpDestination", ("id", "int64"), ("name", "string"), ("destination_type", "string"), ("destination_url", "string"), ("file_destination_path", "string"), ("file_format", "string"), ("file_interval_minutes", "int64"), ("sending_active", "boolean"), ("generic_payload_type", "string"), ("splunk_token_masked", "string"), ("crowdstrike_token_masked", "string"), ("azure_dcr_immutable_id", "string"), ("azure_stream_name", "string"), ("azure_oauth_client_credentials_tenant_id", "string"), ("azure_oauth_client_credentials_client_id", "string"), ("azure_oauth_client_credentials_client_secret_masked", "string"), ("qradar_username", "string"), ("qradar_password_masked", "string"), ("solar_winds_token_masked", "string"), ("new_relic_api_key_masked", "string"), ("datadog_api_key_masked", "string"), ("action_send_enabled", "boolean"), ("action_entries_sent", "int64"), ("sftp_action_send_enabled", "boolean"), ("sftp_action_entries_sent", "int64"), ("ftp_action_send_enabled", "boolean"), ("ftp_action_entries_sent", "int64"), ("web_dav_action_send_enabled", "boolean"), ("web_dav_action_entries_sent", "int64"), ("sync_send_enabled", "boolean"), ("sync_entries_sent", "int64"), ("outbound_connection_send_enabled", "boolean"), ("outbound_connection_entries_sent", "int64"), ("automation_send_enabled", "boolean"), ("automation_entries_sent", "int64"), ("api_request_send_enabled", "boolean"), ("api_request_entries_sent", "int64"), ("public_hosting_request_send_enabled", "boolean"), ("public_hosting_request_entries_sent", "int64"), ("email_send_enabled", "boolean"), ("email_entries_sent", "int64"), ("exavault_api_request_send_enabled", "boolean"), ("exavault_api_request_entries_sent", "int64"), ("settings_change_send_enabled", "boolean"), ("settings_change_entries_sent", "int64"), ("last_http_call_target_type", "string"), ("last_http_call_success", "boolean"), ("last_http_call_response_code", "int64"), ("last_http_call_response_body", "string"), ("last_http_call_error_message", "string"), ("last_http_call_time", "string"), ("last_http_call_duration_ms", "int64"), ("most_recent_http_call_success_time", "string"), ("connection_test_entry", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the SiemHttpDestination the SDK returns gives it.
        private static void AssertModel(JsonObject record, SiemHttpDestination model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no SiemHttpDestination");
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "destination_type", model.DestinationType, what);
            Simulator.Same(record, "destination_url", model.DestinationUrl, what);
            Simulator.Same(record, "file_destination_path", model.FileDestinationPath, what);
            Simulator.Same(record, "file_format", model.FileFormat, what);
            Simulator.Same(record, "file_interval_minutes", model.FileIntervalMinutes, what);
            Simulator.Same(record, "sending_active", model.SendingActive, what);
            Simulator.Same(record, "generic_payload_type", model.GenericPayloadType, what);
            Simulator.Same(record, "splunk_token_masked", model.SplunkTokenMasked, what);
            Simulator.Same(record, "crowdstrike_token_masked", model.CrowdstrikeTokenMasked, what);
            Simulator.Same(record, "azure_dcr_immutable_id", model.AzureDcrImmutableId, what);
            Simulator.Same(record, "azure_stream_name", model.AzureStreamName, what);
            Simulator.Same(record, "azure_oauth_client_credentials_tenant_id", model.AzureOauthClientCredentialsTenantId, what);
            Simulator.Same(record, "azure_oauth_client_credentials_client_id", model.AzureOauthClientCredentialsClientId, what);
            Simulator.Same(record, "azure_oauth_client_credentials_client_secret_masked", model.AzureOauthClientCredentialsClientSecretMasked, what);
            Simulator.Same(record, "qradar_username", model.QradarUsername, what);
            Simulator.Same(record, "qradar_password_masked", model.QradarPasswordMasked, what);
            Simulator.Same(record, "solar_winds_token_masked", model.SolarWindsTokenMasked, what);
            Simulator.Same(record, "new_relic_api_key_masked", model.NewRelicApiKeyMasked, what);
            Simulator.Same(record, "datadog_api_key_masked", model.DatadogApiKeyMasked, what);
            Simulator.Same(record, "action_send_enabled", model.ActionSendEnabled, what);
            Simulator.Same(record, "action_entries_sent", model.ActionEntriesSent, what);
            Simulator.Same(record, "sftp_action_send_enabled", model.SftpActionSendEnabled, what);
            Simulator.Same(record, "sftp_action_entries_sent", model.SftpActionEntriesSent, what);
            Simulator.Same(record, "ftp_action_send_enabled", model.FtpActionSendEnabled, what);
            Simulator.Same(record, "ftp_action_entries_sent", model.FtpActionEntriesSent, what);
            Simulator.Same(record, "web_dav_action_send_enabled", model.WebDavActionSendEnabled, what);
            Simulator.Same(record, "web_dav_action_entries_sent", model.WebDavActionEntriesSent, what);
            Simulator.Same(record, "sync_send_enabled", model.SyncSendEnabled, what);
            Simulator.Same(record, "sync_entries_sent", model.SyncEntriesSent, what);
            Simulator.Same(record, "outbound_connection_send_enabled", model.OutboundConnectionSendEnabled, what);
            Simulator.Same(record, "outbound_connection_entries_sent", model.OutboundConnectionEntriesSent, what);
            Simulator.Same(record, "automation_send_enabled", model.AutomationSendEnabled, what);
            Simulator.Same(record, "automation_entries_sent", model.AutomationEntriesSent, what);
            Simulator.Same(record, "api_request_send_enabled", model.ApiRequestSendEnabled, what);
            Simulator.Same(record, "api_request_entries_sent", model.ApiRequestEntriesSent, what);
            Simulator.Same(record, "public_hosting_request_send_enabled", model.PublicHostingRequestSendEnabled, what);
            Simulator.Same(record, "public_hosting_request_entries_sent", model.PublicHostingRequestEntriesSent, what);
            Simulator.Same(record, "email_send_enabled", model.EmailSendEnabled, what);
            Simulator.Same(record, "email_entries_sent", model.EmailEntriesSent, what);
            Simulator.Same(record, "exavault_api_request_send_enabled", model.ExavaultApiRequestSendEnabled, what);
            Simulator.Same(record, "exavault_api_request_entries_sent", model.ExavaultApiRequestEntriesSent, what);
            Simulator.Same(record, "settings_change_send_enabled", model.SettingsChangeSendEnabled, what);
            Simulator.Same(record, "settings_change_entries_sent", model.SettingsChangeEntriesSent, what);
            Simulator.Same(record, "last_http_call_target_type", model.LastHttpCallTargetType, what);
            Simulator.Same(record, "last_http_call_success", model.LastHttpCallSuccess, what);
            Simulator.Same(record, "last_http_call_response_code", model.LastHttpCallResponseCode, what);
            Simulator.Same(record, "last_http_call_response_body", model.LastHttpCallResponseBody, what);
            Simulator.Same(record, "last_http_call_error_message", model.LastHttpCallErrorMessage, what);
            Simulator.Same(record, "last_http_call_time", model.LastHttpCallTime, what);
            Simulator.Same(record, "last_http_call_duration_ms", model.LastHttpCallDurationMs, what);
            Simulator.Same(record, "most_recent_http_call_success_time", model.MostRecentHttpCallSuccessTime, what);
            Simulator.Same(record, "connection_test_entry", model.ConnectionTestEntry, what);
        }

        [TestMethod]
        public async Task Create()
        {
            string operation = await Simulator.RecordOperationAsync("SiemHttpDestination", "create");
            await Simulator.ResetAsync();
            Dictionary<string, object> sent = new Dictionary<string, object> { ["name"] = "contract-name", ["sending_active"] = true, ["generic_payload_type"] = "json_newline", ["file_destination_path"] = "contract-file_destination_path", ["file_format"] = "json", ["file_interval_minutes"] = (Int64)7, ["azure_dcr_immutable_id"] = "contract-azure_dcr_immutable_id", ["azure_stream_name"] = "contract-azure_stream_name", ["azure_oauth_client_credentials_tenant_id"] = "contract-azure_oauth_client_credentials_tenant_id", ["azure_oauth_client_credentials_client_id"] = "contract-azure_oauth_client_credentials_client_id", ["qradar_username"] = "contract-qradar_username", ["action_send_enabled"] = true, ["sftp_action_send_enabled"] = true, ["ftp_action_send_enabled"] = true, ["web_dav_action_send_enabled"] = true, ["sync_send_enabled"] = true, ["outbound_connection_send_enabled"] = true, ["automation_send_enabled"] = true, ["api_request_send_enabled"] = true, ["public_hosting_request_send_enabled"] = true, ["email_send_enabled"] = true, ["exavault_api_request_send_enabled"] = true, ["settings_change_send_enabled"] = true, ["destination_type"] = "generic", ["destination_url"] = "contract-destination_url" };
            SiemHttpDestination created = await Simulator.Sdk(operation).SiemHttpDestinations.CreateAsync(sent);
            await Simulator.SdkRequestAsync(operation, 201, new JsonObject { ["name"] = "string", ["sending_active"] = "boolean", ["generic_payload_type"] = "string", ["file_destination_path"] = "string", ["file_format"] = "string", ["file_interval_minutes"] = "number", ["azure_dcr_immutable_id"] = "string", ["azure_stream_name"] = "string", ["azure_oauth_client_credentials_tenant_id"] = "string", ["azure_oauth_client_credentials_client_id"] = "string", ["qradar_username"] = "string", ["action_send_enabled"] = "boolean", ["sftp_action_send_enabled"] = "boolean", ["ftp_action_send_enabled"] = "boolean", ["web_dav_action_send_enabled"] = "boolean", ["sync_send_enabled"] = "boolean", ["outbound_connection_send_enabled"] = "boolean", ["automation_send_enabled"] = "boolean", ["api_request_send_enabled"] = "boolean", ["public_hosting_request_send_enabled"] = "boolean", ["email_send_enabled"] = "boolean", ["exavault_api_request_send_enabled"] = "boolean", ["settings_change_send_enabled"] = "boolean", ["destination_type"] = "string", ["destination_url"] = "string" });
            JsonObject record = await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{created.Id}", null, null);
            await Simulator.KeptAsync(sent, record, "SiemHttpDestination.create", "name", "sending_active", "generic_payload_type", "file_destination_path", "file_format", "file_interval_minutes", "azure_dcr_immutable_id", "azure_stream_name", "azure_oauth_client_credentials_tenant_id", "azure_oauth_client_credentials_client_id", "qradar_username", "action_send_enabled", "sftp_action_send_enabled", "ftp_action_send_enabled", "web_dav_action_send_enabled", "sync_send_enabled", "outbound_connection_send_enabled", "automation_send_enabled", "api_request_send_enabled", "public_hosting_request_send_enabled", "email_send_enabled", "exavault_api_request_send_enabled", "settings_change_send_enabled", "destination_type", "destination_url");
            AssertModel(record, created, "SiemHttpDestination.create");
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("SiemHttpDestination", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SiemHttpDestination[] listed = (await Simulator.Sdk(operation).SiemHttpDestinations.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"SiemHttpDestination.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("SiemHttpDestination", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SiemHttpDestination found = await Simulator.Sdk(operation).SiemHttpDestinations.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "SiemHttpDestination.find");
        }

        [TestMethod]
        public async Task Update()
        {
            string operation = await Simulator.RecordOperationAsync("SiemHttpDestination", "update");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            sent["name"] = "contract-name-2";
            sent["sending_active"] = false;
            sent["generic_payload_type"] = "json_array";
            sent["file_destination_path"] = "contract-file_destination_path-2";
            sent["file_format"] = "csv";
            sent["file_interval_minutes"] = (Int64)8;
            sent["azure_dcr_immutable_id"] = "contract-azure_dcr_immutable_id-2";
            sent["azure_stream_name"] = "contract-azure_stream_name-2";
            sent["azure_oauth_client_credentials_tenant_id"] = "contract-azure_oauth_client_credentials_tenant_id-2";
            sent["azure_oauth_client_credentials_client_id"] = "contract-azure_oauth_client_credentials_client_id-2";
            sent["qradar_username"] = "contract-qradar_username-2";
            sent["action_send_enabled"] = false;
            sent["sftp_action_send_enabled"] = false;
            sent["ftp_action_send_enabled"] = false;
            sent["web_dav_action_send_enabled"] = false;
            sent["sync_send_enabled"] = false;
            sent["outbound_connection_send_enabled"] = false;
            sent["automation_send_enabled"] = false;
            sent["api_request_send_enabled"] = false;
            sent["public_hosting_request_send_enabled"] = false;
            sent["email_send_enabled"] = false;
            sent["exavault_api_request_send_enabled"] = false;
            sent["settings_change_send_enabled"] = false;
            sent["destination_type"] = "splunk";
            sent["destination_url"] = "contract-destination_url-2";
            SiemHttpDestination updated = await Simulator.Sdk(operation).SiemHttpDestinations.UpdateAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "number", ["name"] = "string", ["sending_active"] = "boolean", ["generic_payload_type"] = "string", ["file_destination_path"] = "string", ["file_format"] = "string", ["file_interval_minutes"] = "number", ["azure_dcr_immutable_id"] = "string", ["azure_stream_name"] = "string", ["azure_oauth_client_credentials_tenant_id"] = "string", ["azure_oauth_client_credentials_client_id"] = "string", ["qradar_username"] = "string", ["action_send_enabled"] = "boolean", ["sftp_action_send_enabled"] = "boolean", ["ftp_action_send_enabled"] = "boolean", ["web_dav_action_send_enabled"] = "boolean", ["sync_send_enabled"] = "boolean", ["outbound_connection_send_enabled"] = "boolean", ["automation_send_enabled"] = "boolean", ["api_request_send_enabled"] = "boolean", ["public_hosting_request_send_enabled"] = "boolean", ["email_send_enabled"] = "boolean", ["exavault_api_request_send_enabled"] = "boolean", ["settings_change_send_enabled"] = "boolean", ["destination_type"] = "string", ["destination_url"] = "string" });
            JsonObject now = await Simulator.ReadRawAsync(ListPath, MemberPath(record), null, null);
            await Simulator.KeptAsync(sent, now, "SiemHttpDestination.update", "name", "sending_active", "generic_payload_type", "file_destination_path", "file_format", "file_interval_minutes", "azure_dcr_immutable_id", "azure_stream_name", "azure_oauth_client_credentials_tenant_id", "azure_oauth_client_credentials_client_id", "qradar_username", "action_send_enabled", "sftp_action_send_enabled", "ftp_action_send_enabled", "web_dav_action_send_enabled", "sync_send_enabled", "outbound_connection_send_enabled", "automation_send_enabled", "api_request_send_enabled", "public_hosting_request_send_enabled", "email_send_enabled", "exavault_api_request_send_enabled", "settings_change_send_enabled", "destination_type", "destination_url");
            AssertModel(now, updated, "SiemHttpDestination.update");
        }

        [TestMethod]
        public async Task Delete()
        {
            string operation = await Simulator.RecordOperationAsync("SiemHttpDestination", "delete");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            await Simulator.Sdk(operation).SiemHttpDestinations.DeleteAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 204, new JsonObject { ["id"] = "string" });
            (int status, JsonNode body) = await Simulator.ApiAsync(HttpMethod.Get, MemberPath(record));
            Assert.AreEqual(404, status, $"SiemHttpDestination.delete: the record is gone: {body?.ToJsonString()}");
        }
    }
}