# FilesCom.Models.SiemHttpDestination

## Example SiemHttpDestination Object

```
{
  "id": 1,
  "name": "example",
  "destination_type": "example",
  "destination_url": "example",
  "additional_headers": {
    "key": "example value"
  },
  "sending_active": true,
  "generic_payload_type": "example",
  "splunk_token_masked": "example",
  "azure_dcr_immutable_id": "example",
  "azure_stream_name": "example",
  "azure_oauth_client_credentials_tenant_id": "example",
  "azure_oauth_client_credentials_client_id": "example",
  "azure_oauth_client_credentials_client_secret_masked": "example",
  "qradar_username": "example",
  "qradar_password_masked": "example",
  "solar_winds_token_masked": "example",
  "new_relic_api_key_masked": "example",
  "datadog_api_key_masked": "example",
  "sftp_action_send_enabled": true,
  "sftp_action_entries_sent": 1,
  "ftp_action_send_enabled": true,
  "ftp_action_entries_sent": 1,
  "web_dav_action_send_enabled": true,
  "web_dav_action_entries_sent": 1,
  "sync_send_enabled": true,
  "sync_entries_sent": 1,
  "outbound_connection_send_enabled": true,
  "outbound_connection_entries_sent": 1,
  "automation_send_enabled": true,
  "automation_entries_sent": 1,
  "api_request_send_enabled": true,
  "api_request_entries_sent": 1,
  "public_hosting_request_send_enabled": true,
  "public_hosting_request_entries_sent": 1,
  "email_send_enabled": true,
  "email_entries_sent": 1,
  "exavault_api_request_send_enabled": true,
  "exavault_api_request_entries_sent": 1,
  "settings_change_send_enabled": true,
  "settings_change_entries_sent": 1,
  "last_http_call_target_type": "destination_url",
  "last_http_call_success": true,
  "last_http_call_response_code": 1,
  "last_http_call_response_body": "example",
  "last_http_call_error_message": "example",
  "last_http_call_time": "example",
  "last_http_call_duration_ms": 1,
  "most_recent_http_call_success_time": "example",
  "connection_test_entry": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): SIEM HTTP Destination ID
* `name` / `Name`  (string): Name for this Destination
* `destination_type` / `DestinationType`  (string): Destination Type
* `destination_url` / `DestinationUrl`  (string): Destination Url
* `additional_headers` / `AdditionalHeaders`  (object): Additional HTTP Headers included in calls to the destination URL
* `sending_active` / `SendingActive`  (bool): Whether this SIEM HTTP Destination is currently being sent to or not
* `generic_payload_type` / `GenericPayloadType`  (string): Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
* `splunk_token_masked` / `SplunkTokenMasked`  (string): Applicable only for destination type: splunk. Authentication token provided by Splunk.
* `azure_dcr_immutable_id` / `AzureDcrImmutableId`  (string): Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
* `azure_stream_name` / `AzureStreamName`  (string): Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
* `azure_oauth_client_credentials_tenant_id` / `AzureOauthClientCredentialsTenantId`  (string): Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
* `azure_oauth_client_credentials_client_id` / `AzureOauthClientCredentialsClientId`  (string): Applicable only for destination type: azure. Client Credentials OAuth Client ID.
* `azure_oauth_client_credentials_client_secret_masked` / `AzureOauthClientCredentialsClientSecretMasked`  (string): Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
* `qradar_username` / `QradarUsername`  (string): Applicable only for destination type: qradar. Basic auth username provided by QRadar.
* `qradar_password_masked` / `QradarPasswordMasked`  (string): Applicable only for destination type: qradar. Basic auth password provided by QRadar.
* `solar_winds_token_masked` / `SolarWindsTokenMasked`  (string): Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
* `new_relic_api_key_masked` / `NewRelicApiKeyMasked`  (string): Applicable only for destination type: new_relic. API key provided by New Relic.
* `datadog_api_key_masked` / `DatadogApiKeyMasked`  (string): Applicable only for destination type: datadog. API key provided by Datadog.
* `sftp_action_send_enabled` / `SftpActionSendEnabled`  (bool): Whether or not sending is enabled for sftp_action logs.
* `sftp_action_entries_sent` / `SftpActionEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `ftp_action_send_enabled` / `FtpActionSendEnabled`  (bool): Whether or not sending is enabled for ftp_action logs.
* `ftp_action_entries_sent` / `FtpActionEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `web_dav_action_send_enabled` / `WebDavActionSendEnabled`  (bool): Whether or not sending is enabled for web_dav_action logs.
* `web_dav_action_entries_sent` / `WebDavActionEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `sync_send_enabled` / `SyncSendEnabled`  (bool): Whether or not sending is enabled for sync logs.
* `sync_entries_sent` / `SyncEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `outbound_connection_send_enabled` / `OutboundConnectionSendEnabled`  (bool): Whether or not sending is enabled for outbound_connection logs.
* `outbound_connection_entries_sent` / `OutboundConnectionEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `automation_send_enabled` / `AutomationSendEnabled`  (bool): Whether or not sending is enabled for automation logs.
* `automation_entries_sent` / `AutomationEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `api_request_send_enabled` / `ApiRequestSendEnabled`  (bool): Whether or not sending is enabled for api_request logs.
* `api_request_entries_sent` / `ApiRequestEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `public_hosting_request_send_enabled` / `PublicHostingRequestSendEnabled`  (bool): Whether or not sending is enabled for public_hosting_request logs.
* `public_hosting_request_entries_sent` / `PublicHostingRequestEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `email_send_enabled` / `EmailSendEnabled`  (bool): Whether or not sending is enabled for email logs.
* `email_entries_sent` / `EmailEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `exavault_api_request_send_enabled` / `ExavaultApiRequestSendEnabled`  (bool): Whether or not sending is enabled for exavault_api_request logs.
* `exavault_api_request_entries_sent` / `ExavaultApiRequestEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `settings_change_send_enabled` / `SettingsChangeSendEnabled`  (bool): Whether or not sending is enabled for settings_change logs.
* `settings_change_entries_sent` / `SettingsChangeEntriesSent`  (Nullable<Int64>): Number of log entries sent for the lifetime of this destination.
* `last_http_call_target_type` / `LastHttpCallTargetType`  (string): Type of URL that was last called. Can be `destination_url` or `azure_oauth_client_credentials_url`
* `last_http_call_success` / `LastHttpCallSuccess`  (bool): Was the last HTTP call made successful?
* `last_http_call_response_code` / `LastHttpCallResponseCode`  (Nullable<Int64>): Last HTTP Call Response Code
* `last_http_call_response_body` / `LastHttpCallResponseBody`  (string): Last HTTP Call Response Body. Large responses are truncated.
* `last_http_call_error_message` / `LastHttpCallErrorMessage`  (string): Last HTTP Call Error Message if applicable
* `last_http_call_time` / `LastHttpCallTime`  (string): Time of Last HTTP Call
* `last_http_call_duration_ms` / `LastHttpCallDurationMs`  (Nullable<Int64>): Duration of the last HTTP Call in milliseconds
* `most_recent_http_call_success_time` / `MostRecentHttpCallSuccessTime`  (string): Time of Most Recent Successful HTTP Call
* `connection_test_entry` / `ConnectionTestEntry`  (string): Connection Test Entry
* `splunk_token` / `SplunkToken`  (string): Applicable only for destination type: splunk. Authentication token provided by Splunk.
* `azure_oauth_client_credentials_client_secret` / `AzureOauthClientCredentialsClientSecret`  (string): Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
* `qradar_password` / `QradarPassword`  (string): Applicable only for destination type: qradar. Basic auth password provided by QRadar.
* `solar_winds_token` / `SolarWindsToken`  (string): Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
* `new_relic_api_key` / `NewRelicApiKey`  (string): Applicable only for destination type: new_relic. API key provided by New Relic.
* `datadog_api_key` / `DatadogApiKey`  (string): Applicable only for destination type: datadog. API key provided by Datadog.


---

## List SIEM HTTP Destinations

```
Task<FilesList<SiemHttpDestination>> SiemHttpDestination.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show SIEM HTTP Destination

```
Task<SiemHttpDestination> SiemHttpDestination.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Siem Http Destination ID.


---

## Create SIEM HTTP Destination

```
Task<SiemHttpDestination> SiemHttpDestination.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Name for this Destination
* `additional_headers` (object): Additional HTTP Headers included in calls to the destination URL
* `sending_active` (bool): Whether this SIEM HTTP Destination is currently being sent to or not
* `generic_payload_type` (string): Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
* `splunk_token` (string): Applicable only for destination type: splunk. Authentication token provided by Splunk.
* `azure_dcr_immutable_id` (string): Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
* `azure_stream_name` (string): Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
* `azure_oauth_client_credentials_tenant_id` (string): Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
* `azure_oauth_client_credentials_client_id` (string): Applicable only for destination type: azure. Client Credentials OAuth Client ID.
* `azure_oauth_client_credentials_client_secret` (string): Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
* `qradar_username` (string): Applicable only for destination type: qradar. Basic auth username provided by QRadar.
* `qradar_password` (string): Applicable only for destination type: qradar. Basic auth password provided by QRadar.
* `solar_winds_token` (string): Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
* `new_relic_api_key` (string): Applicable only for destination type: new_relic. API key provided by New Relic.
* `datadog_api_key` (string): Applicable only for destination type: datadog. API key provided by Datadog.
* `sftp_action_send_enabled` (bool): Whether or not sending is enabled for sftp_action logs.
* `ftp_action_send_enabled` (bool): Whether or not sending is enabled for ftp_action logs.
* `web_dav_action_send_enabled` (bool): Whether or not sending is enabled for web_dav_action logs.
* `sync_send_enabled` (bool): Whether or not sending is enabled for sync logs.
* `outbound_connection_send_enabled` (bool): Whether or not sending is enabled for outbound_connection logs.
* `automation_send_enabled` (bool): Whether or not sending is enabled for automation logs.
* `api_request_send_enabled` (bool): Whether or not sending is enabled for api_request logs.
* `public_hosting_request_send_enabled` (bool): Whether or not sending is enabled for public_hosting_request logs.
* `email_send_enabled` (bool): Whether or not sending is enabled for email logs.
* `exavault_api_request_send_enabled` (bool): Whether or not sending is enabled for exavault_api_request logs.
* `settings_change_send_enabled` (bool): Whether or not sending is enabled for settings_change logs.
* `destination_type` (string): Required - Destination Type
* `destination_url` (string): Required - Destination Url


---

## send_test_entry SIEM HTTP Destination

```
Task SiemHttpDestination.SendTestEntry(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `siem_http_destination_id` (Nullable<Int64>): SIEM HTTP Destination ID
* `destination_type` (string): Destination Type
* `destination_url` (string): Destination Url
* `name` (string): Name for this Destination
* `additional_headers` (object): Additional HTTP Headers included in calls to the destination URL
* `sending_active` (bool): Whether this SIEM HTTP Destination is currently being sent to or not
* `generic_payload_type` (string): Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
* `splunk_token` (string): Applicable only for destination type: splunk. Authentication token provided by Splunk.
* `azure_dcr_immutable_id` (string): Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
* `azure_stream_name` (string): Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
* `azure_oauth_client_credentials_tenant_id` (string): Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
* `azure_oauth_client_credentials_client_id` (string): Applicable only for destination type: azure. Client Credentials OAuth Client ID.
* `azure_oauth_client_credentials_client_secret` (string): Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
* `qradar_username` (string): Applicable only for destination type: qradar. Basic auth username provided by QRadar.
* `qradar_password` (string): Applicable only for destination type: qradar. Basic auth password provided by QRadar.
* `solar_winds_token` (string): Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
* `new_relic_api_key` (string): Applicable only for destination type: new_relic. API key provided by New Relic.
* `datadog_api_key` (string): Applicable only for destination type: datadog. API key provided by Datadog.
* `sftp_action_send_enabled` (bool): Whether or not sending is enabled for sftp_action logs.
* `ftp_action_send_enabled` (bool): Whether or not sending is enabled for ftp_action logs.
* `web_dav_action_send_enabled` (bool): Whether or not sending is enabled for web_dav_action logs.
* `sync_send_enabled` (bool): Whether or not sending is enabled for sync logs.
* `outbound_connection_send_enabled` (bool): Whether or not sending is enabled for outbound_connection logs.
* `automation_send_enabled` (bool): Whether or not sending is enabled for automation logs.
* `api_request_send_enabled` (bool): Whether or not sending is enabled for api_request logs.
* `public_hosting_request_send_enabled` (bool): Whether or not sending is enabled for public_hosting_request logs.
* `email_send_enabled` (bool): Whether or not sending is enabled for email logs.
* `exavault_api_request_send_enabled` (bool): Whether or not sending is enabled for exavault_api_request logs.
* `settings_change_send_enabled` (bool): Whether or not sending is enabled for settings_change logs.


---

## Update SIEM HTTP Destination

```
Task<SiemHttpDestination> SiemHttpDestination.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Siem Http Destination ID.
* `name` (string): Name for this Destination
* `additional_headers` (object): Additional HTTP Headers included in calls to the destination URL
* `sending_active` (bool): Whether this SIEM HTTP Destination is currently being sent to or not
* `generic_payload_type` (string): Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
* `splunk_token` (string): Applicable only for destination type: splunk. Authentication token provided by Splunk.
* `azure_dcr_immutable_id` (string): Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
* `azure_stream_name` (string): Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
* `azure_oauth_client_credentials_tenant_id` (string): Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
* `azure_oauth_client_credentials_client_id` (string): Applicable only for destination type: azure. Client Credentials OAuth Client ID.
* `azure_oauth_client_credentials_client_secret` (string): Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
* `qradar_username` (string): Applicable only for destination type: qradar. Basic auth username provided by QRadar.
* `qradar_password` (string): Applicable only for destination type: qradar. Basic auth password provided by QRadar.
* `solar_winds_token` (string): Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
* `new_relic_api_key` (string): Applicable only for destination type: new_relic. API key provided by New Relic.
* `datadog_api_key` (string): Applicable only for destination type: datadog. API key provided by Datadog.
* `sftp_action_send_enabled` (bool): Whether or not sending is enabled for sftp_action logs.
* `ftp_action_send_enabled` (bool): Whether or not sending is enabled for ftp_action logs.
* `web_dav_action_send_enabled` (bool): Whether or not sending is enabled for web_dav_action logs.
* `sync_send_enabled` (bool): Whether or not sending is enabled for sync logs.
* `outbound_connection_send_enabled` (bool): Whether or not sending is enabled for outbound_connection logs.
* `automation_send_enabled` (bool): Whether or not sending is enabled for automation logs.
* `api_request_send_enabled` (bool): Whether or not sending is enabled for api_request logs.
* `public_hosting_request_send_enabled` (bool): Whether or not sending is enabled for public_hosting_request logs.
* `email_send_enabled` (bool): Whether or not sending is enabled for email logs.
* `exavault_api_request_send_enabled` (bool): Whether or not sending is enabled for exavault_api_request logs.
* `settings_change_send_enabled` (bool): Whether or not sending is enabled for settings_change logs.
* `destination_type` (string): Destination Type
* `destination_url` (string): Destination Url


---

## Delete SIEM HTTP Destination

```
Task SiemHttpDestination.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Siem Http Destination ID.


---

## Update SIEM HTTP Destination

```
var SiemHttpDestination = SiemHttpDestination.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "example");
parameters.Add("additional_headers", {"key":"example value"});
parameters.Add("sending_active", true);
parameters.Add("generic_payload_type", "example");
parameters.Add("azure_dcr_immutable_id", "example");
parameters.Add("azure_stream_name", "example");
parameters.Add("azure_oauth_client_credentials_tenant_id", "example");
parameters.Add("azure_oauth_client_credentials_client_id", "example");
parameters.Add("qradar_username", "example");
parameters.Add("sftp_action_send_enabled", true);
parameters.Add("ftp_action_send_enabled", true);
parameters.Add("web_dav_action_send_enabled", true);
parameters.Add("sync_send_enabled", true);
parameters.Add("outbound_connection_send_enabled", true);
parameters.Add("automation_send_enabled", true);
parameters.Add("api_request_send_enabled", true);
parameters.Add("public_hosting_request_send_enabled", true);
parameters.Add("email_send_enabled", true);
parameters.Add("exavault_api_request_send_enabled", true);
parameters.Add("settings_change_send_enabled", true);
parameters.Add("destination_type", "example");
parameters.Add("destination_url", "example");

SiemHttpDestination.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Siem Http Destination ID.
* `name` (string): Name for this Destination
* `additional_headers` (object): Additional HTTP Headers included in calls to the destination URL
* `sending_active` (bool): Whether this SIEM HTTP Destination is currently being sent to or not
* `generic_payload_type` (string): Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
* `splunk_token` (string): Applicable only for destination type: splunk. Authentication token provided by Splunk.
* `azure_dcr_immutable_id` (string): Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
* `azure_stream_name` (string): Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
* `azure_oauth_client_credentials_tenant_id` (string): Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
* `azure_oauth_client_credentials_client_id` (string): Applicable only for destination type: azure. Client Credentials OAuth Client ID.
* `azure_oauth_client_credentials_client_secret` (string): Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
* `qradar_username` (string): Applicable only for destination type: qradar. Basic auth username provided by QRadar.
* `qradar_password` (string): Applicable only for destination type: qradar. Basic auth password provided by QRadar.
* `solar_winds_token` (string): Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
* `new_relic_api_key` (string): Applicable only for destination type: new_relic. API key provided by New Relic.
* `datadog_api_key` (string): Applicable only for destination type: datadog. API key provided by Datadog.
* `sftp_action_send_enabled` (bool): Whether or not sending is enabled for sftp_action logs.
* `ftp_action_send_enabled` (bool): Whether or not sending is enabled for ftp_action logs.
* `web_dav_action_send_enabled` (bool): Whether or not sending is enabled for web_dav_action logs.
* `sync_send_enabled` (bool): Whether or not sending is enabled for sync logs.
* `outbound_connection_send_enabled` (bool): Whether or not sending is enabled for outbound_connection logs.
* `automation_send_enabled` (bool): Whether or not sending is enabled for automation logs.
* `api_request_send_enabled` (bool): Whether or not sending is enabled for api_request logs.
* `public_hosting_request_send_enabled` (bool): Whether or not sending is enabled for public_hosting_request logs.
* `email_send_enabled` (bool): Whether or not sending is enabled for email logs.
* `exavault_api_request_send_enabled` (bool): Whether or not sending is enabled for exavault_api_request logs.
* `settings_change_send_enabled` (bool): Whether or not sending is enabled for settings_change logs.
* `destination_type` (string): Destination Type
* `destination_url` (string): Destination Url


---

## Delete SIEM HTTP Destination

```
var SiemHttpDestination = SiemHttpDestination.Find(1);

var parameters = new Dictionary<string, object>();


SiemHttpDestination.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Siem Http Destination ID.
