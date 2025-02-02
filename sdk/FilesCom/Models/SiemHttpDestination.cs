using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class SiemHttpDestination
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public SiemHttpDestination() : this(null, null) { }

        public SiemHttpDestination(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;

            if (this.attributes == null)
            {
                this.attributes = new Dictionary<string, object>();
            }

            if (this.options == null)
            {
                this.options = new Dictionary<string, object>();
            }

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("destination_type"))
            {
                this.attributes.Add("destination_type", null);
            }
            if (!this.attributes.ContainsKey("destination_url"))
            {
                this.attributes.Add("destination_url", null);
            }
            if (!this.attributes.ContainsKey("additional_headers"))
            {
                this.attributes.Add("additional_headers", null);
            }
            if (!this.attributes.ContainsKey("sending_active"))
            {
                this.attributes.Add("sending_active", false);
            }
            if (!this.attributes.ContainsKey("generic_payload_type"))
            {
                this.attributes.Add("generic_payload_type", null);
            }
            if (!this.attributes.ContainsKey("splunk_token_masked"))
            {
                this.attributes.Add("splunk_token_masked", null);
            }
            if (!this.attributes.ContainsKey("azure_dcr_immutable_id"))
            {
                this.attributes.Add("azure_dcr_immutable_id", null);
            }
            if (!this.attributes.ContainsKey("azure_stream_name"))
            {
                this.attributes.Add("azure_stream_name", null);
            }
            if (!this.attributes.ContainsKey("azure_oauth_client_credentials_tenant_id"))
            {
                this.attributes.Add("azure_oauth_client_credentials_tenant_id", null);
            }
            if (!this.attributes.ContainsKey("azure_oauth_client_credentials_client_id"))
            {
                this.attributes.Add("azure_oauth_client_credentials_client_id", null);
            }
            if (!this.attributes.ContainsKey("azure_oauth_client_credentials_client_secret_masked"))
            {
                this.attributes.Add("azure_oauth_client_credentials_client_secret_masked", null);
            }
            if (!this.attributes.ContainsKey("qradar_username"))
            {
                this.attributes.Add("qradar_username", null);
            }
            if (!this.attributes.ContainsKey("qradar_password_masked"))
            {
                this.attributes.Add("qradar_password_masked", null);
            }
            if (!this.attributes.ContainsKey("solar_winds_token_masked"))
            {
                this.attributes.Add("solar_winds_token_masked", null);
            }
            if (!this.attributes.ContainsKey("new_relic_api_key_masked"))
            {
                this.attributes.Add("new_relic_api_key_masked", null);
            }
            if (!this.attributes.ContainsKey("datadog_api_key_masked"))
            {
                this.attributes.Add("datadog_api_key_masked", null);
            }
            if (!this.attributes.ContainsKey("sftp_action_send_enabled"))
            {
                this.attributes.Add("sftp_action_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("sftp_action_entries_sent"))
            {
                this.attributes.Add("sftp_action_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("ftp_action_send_enabled"))
            {
                this.attributes.Add("ftp_action_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("ftp_action_entries_sent"))
            {
                this.attributes.Add("ftp_action_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("web_dav_action_send_enabled"))
            {
                this.attributes.Add("web_dav_action_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("web_dav_action_entries_sent"))
            {
                this.attributes.Add("web_dav_action_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("sync_send_enabled"))
            {
                this.attributes.Add("sync_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("sync_entries_sent"))
            {
                this.attributes.Add("sync_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("outbound_connection_send_enabled"))
            {
                this.attributes.Add("outbound_connection_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("outbound_connection_entries_sent"))
            {
                this.attributes.Add("outbound_connection_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("automation_send_enabled"))
            {
                this.attributes.Add("automation_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("automation_entries_sent"))
            {
                this.attributes.Add("automation_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("api_request_send_enabled"))
            {
                this.attributes.Add("api_request_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("api_request_entries_sent"))
            {
                this.attributes.Add("api_request_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("public_hosting_request_send_enabled"))
            {
                this.attributes.Add("public_hosting_request_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("public_hosting_request_entries_sent"))
            {
                this.attributes.Add("public_hosting_request_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("email_send_enabled"))
            {
                this.attributes.Add("email_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("email_entries_sent"))
            {
                this.attributes.Add("email_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("exavault_api_request_send_enabled"))
            {
                this.attributes.Add("exavault_api_request_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("exavault_api_request_entries_sent"))
            {
                this.attributes.Add("exavault_api_request_entries_sent", null);
            }
            if (!this.attributes.ContainsKey("last_http_call_target_type"))
            {
                this.attributes.Add("last_http_call_target_type", null);
            }
            if (!this.attributes.ContainsKey("last_http_call_success"))
            {
                this.attributes.Add("last_http_call_success", false);
            }
            if (!this.attributes.ContainsKey("last_http_call_response_code"))
            {
                this.attributes.Add("last_http_call_response_code", null);
            }
            if (!this.attributes.ContainsKey("last_http_call_response_body"))
            {
                this.attributes.Add("last_http_call_response_body", null);
            }
            if (!this.attributes.ContainsKey("last_http_call_error_message"))
            {
                this.attributes.Add("last_http_call_error_message", null);
            }
            if (!this.attributes.ContainsKey("last_http_call_time"))
            {
                this.attributes.Add("last_http_call_time", null);
            }
            if (!this.attributes.ContainsKey("last_http_call_duration_ms"))
            {
                this.attributes.Add("last_http_call_duration_ms", null);
            }
            if (!this.attributes.ContainsKey("most_recent_http_call_success_time"))
            {
                this.attributes.Add("most_recent_http_call_success_time", null);
            }
            if (!this.attributes.ContainsKey("connection_test_entry"))
            {
                this.attributes.Add("connection_test_entry", null);
            }
            if (!this.attributes.ContainsKey("splunk_token"))
            {
                this.attributes.Add("splunk_token", null);
            }
            if (!this.attributes.ContainsKey("azure_oauth_client_credentials_client_secret"))
            {
                this.attributes.Add("azure_oauth_client_credentials_client_secret", null);
            }
            if (!this.attributes.ContainsKey("qradar_password"))
            {
                this.attributes.Add("qradar_password", null);
            }
            if (!this.attributes.ContainsKey("solar_winds_token"))
            {
                this.attributes.Add("solar_winds_token", null);
            }
            if (!this.attributes.ContainsKey("new_relic_api_key"))
            {
                this.attributes.Add("new_relic_api_key", null);
            }
            if (!this.attributes.ContainsKey("datadog_api_key"))
            {
                this.attributes.Add("datadog_api_key", null);
            }
        }

        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        public object GetOption(string name)
        {
            return (this.options.ContainsKey(name) ? this.options[name] : null);
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// SIEM HTTP Destination ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Name for this Destination
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Destination Type
        /// </summary>
        [JsonPropertyName("destination_type")]
        public string DestinationType
        {
            get { return (string)attributes["destination_type"]; }
            set { attributes["destination_type"] = value; }
        }

        /// <summary>
        /// Destination Url
        /// </summary>
        [JsonPropertyName("destination_url")]
        public string DestinationUrl
        {
            get { return (string)attributes["destination_url"]; }
            set { attributes["destination_url"] = value; }
        }

        /// <summary>
        /// Additional HTTP Headers included in calls to the destination URL
        /// </summary>
        [JsonPropertyName("additional_headers")]
        public object AdditionalHeaders
        {
            get { return (object)attributes["additional_headers"]; }
            set { attributes["additional_headers"] = value; }
        }

        /// <summary>
        /// Whether this SIEM HTTP Destination is currently being sent to or not
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sending_active")]
        public bool SendingActive
        {
            get { return attributes["sending_active"] == null ? false : (bool)attributes["sending_active"]; }
            set { attributes["sending_active"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        /// </summary>
        [JsonPropertyName("generic_payload_type")]
        public string GenericPayloadType
        {
            get { return (string)attributes["generic_payload_type"]; }
            set { attributes["generic_payload_type"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: splunk. Authentication token provided by Splunk.
        /// </summary>
        [JsonPropertyName("splunk_token_masked")]
        public string SplunkTokenMasked
        {
            get { return (string)attributes["splunk_token_masked"]; }
            set { attributes["splunk_token_masked"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
        /// </summary>
        [JsonPropertyName("azure_dcr_immutable_id")]
        public string AzureDcrImmutableId
        {
            get { return (string)attributes["azure_dcr_immutable_id"]; }
            set { attributes["azure_dcr_immutable_id"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        /// </summary>
        [JsonPropertyName("azure_stream_name")]
        public string AzureStreamName
        {
            get { return (string)attributes["azure_stream_name"]; }
            set { attributes["azure_stream_name"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
        /// </summary>
        [JsonPropertyName("azure_oauth_client_credentials_tenant_id")]
        public string AzureOauthClientCredentialsTenantId
        {
            get { return (string)attributes["azure_oauth_client_credentials_tenant_id"]; }
            set { attributes["azure_oauth_client_credentials_tenant_id"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: azure. Client Credentials OAuth Client ID.
        /// </summary>
        [JsonPropertyName("azure_oauth_client_credentials_client_id")]
        public string AzureOauthClientCredentialsClientId
        {
            get { return (string)attributes["azure_oauth_client_credentials_client_id"]; }
            set { attributes["azure_oauth_client_credentials_client_id"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        /// </summary>
        [JsonPropertyName("azure_oauth_client_credentials_client_secret_masked")]
        public string AzureOauthClientCredentialsClientSecretMasked
        {
            get { return (string)attributes["azure_oauth_client_credentials_client_secret_masked"]; }
            set { attributes["azure_oauth_client_credentials_client_secret_masked"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        /// </summary>
        [JsonPropertyName("qradar_username")]
        public string QradarUsername
        {
            get { return (string)attributes["qradar_username"]; }
            set { attributes["qradar_username"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        /// </summary>
        [JsonPropertyName("qradar_password_masked")]
        public string QradarPasswordMasked
        {
            get { return (string)attributes["qradar_password_masked"]; }
            set { attributes["qradar_password_masked"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        /// </summary>
        [JsonPropertyName("solar_winds_token_masked")]
        public string SolarWindsTokenMasked
        {
            get { return (string)attributes["solar_winds_token_masked"]; }
            set { attributes["solar_winds_token_masked"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: new_relic. API key provided by New Relic.
        /// </summary>
        [JsonPropertyName("new_relic_api_key_masked")]
        public string NewRelicApiKeyMasked
        {
            get { return (string)attributes["new_relic_api_key_masked"]; }
            set { attributes["new_relic_api_key_masked"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: datadog. API key provided by Datadog.
        /// </summary>
        [JsonPropertyName("datadog_api_key_masked")]
        public string DatadogApiKeyMasked
        {
            get { return (string)attributes["datadog_api_key_masked"]; }
            set { attributes["datadog_api_key_masked"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for sftp_action logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_action_send_enabled")]
        public bool SftpActionSendEnabled
        {
            get { return attributes["sftp_action_send_enabled"] == null ? false : (bool)attributes["sftp_action_send_enabled"]; }
            set { attributes["sftp_action_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("sftp_action_entries_sent")]
        public Nullable<Int64> SftpActionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["sftp_action_entries_sent"]; }
            set { attributes["sftp_action_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for ftp_action logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ftp_action_send_enabled")]
        public bool FtpActionSendEnabled
        {
            get { return attributes["ftp_action_send_enabled"] == null ? false : (bool)attributes["ftp_action_send_enabled"]; }
            set { attributes["ftp_action_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("ftp_action_entries_sent")]
        public Nullable<Int64> FtpActionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["ftp_action_entries_sent"]; }
            set { attributes["ftp_action_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for web_dav_action logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("web_dav_action_send_enabled")]
        public bool WebDavActionSendEnabled
        {
            get { return attributes["web_dav_action_send_enabled"] == null ? false : (bool)attributes["web_dav_action_send_enabled"]; }
            set { attributes["web_dav_action_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("web_dav_action_entries_sent")]
        public Nullable<Int64> WebDavActionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["web_dav_action_entries_sent"]; }
            set { attributes["web_dav_action_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for sync logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sync_send_enabled")]
        public bool SyncSendEnabled
        {
            get { return attributes["sync_send_enabled"] == null ? false : (bool)attributes["sync_send_enabled"]; }
            set { attributes["sync_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("sync_entries_sent")]
        public Nullable<Int64> SyncEntriesSent
        {
            get { return (Nullable<Int64>)attributes["sync_entries_sent"]; }
            set { attributes["sync_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for outbound_connection logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("outbound_connection_send_enabled")]
        public bool OutboundConnectionSendEnabled
        {
            get { return attributes["outbound_connection_send_enabled"] == null ? false : (bool)attributes["outbound_connection_send_enabled"]; }
            set { attributes["outbound_connection_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("outbound_connection_entries_sent")]
        public Nullable<Int64> OutboundConnectionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["outbound_connection_entries_sent"]; }
            set { attributes["outbound_connection_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for automation logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("automation_send_enabled")]
        public bool AutomationSendEnabled
        {
            get { return attributes["automation_send_enabled"] == null ? false : (bool)attributes["automation_send_enabled"]; }
            set { attributes["automation_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("automation_entries_sent")]
        public Nullable<Int64> AutomationEntriesSent
        {
            get { return (Nullable<Int64>)attributes["automation_entries_sent"]; }
            set { attributes["automation_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for api_request logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("api_request_send_enabled")]
        public bool ApiRequestSendEnabled
        {
            get { return attributes["api_request_send_enabled"] == null ? false : (bool)attributes["api_request_send_enabled"]; }
            set { attributes["api_request_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("api_request_entries_sent")]
        public Nullable<Int64> ApiRequestEntriesSent
        {
            get { return (Nullable<Int64>)attributes["api_request_entries_sent"]; }
            set { attributes["api_request_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for public_hosting_request logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("public_hosting_request_send_enabled")]
        public bool PublicHostingRequestSendEnabled
        {
            get { return attributes["public_hosting_request_send_enabled"] == null ? false : (bool)attributes["public_hosting_request_send_enabled"]; }
            set { attributes["public_hosting_request_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("public_hosting_request_entries_sent")]
        public Nullable<Int64> PublicHostingRequestEntriesSent
        {
            get { return (Nullable<Int64>)attributes["public_hosting_request_entries_sent"]; }
            set { attributes["public_hosting_request_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for email logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("email_send_enabled")]
        public bool EmailSendEnabled
        {
            get { return attributes["email_send_enabled"] == null ? false : (bool)attributes["email_send_enabled"]; }
            set { attributes["email_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("email_entries_sent")]
        public Nullable<Int64> EmailEntriesSent
        {
            get { return (Nullable<Int64>)attributes["email_entries_sent"]; }
            set { attributes["email_entries_sent"] = value; }
        }

        /// <summary>
        /// Whether or not sending is enabled for exavault_api_request logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("exavault_api_request_send_enabled")]
        public bool ExavaultApiRequestSendEnabled
        {
            get { return attributes["exavault_api_request_send_enabled"] == null ? false : (bool)attributes["exavault_api_request_send_enabled"]; }
            set { attributes["exavault_api_request_send_enabled"] = value; }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("exavault_api_request_entries_sent")]
        public Nullable<Int64> ExavaultApiRequestEntriesSent
        {
            get { return (Nullable<Int64>)attributes["exavault_api_request_entries_sent"]; }
            set { attributes["exavault_api_request_entries_sent"] = value; }
        }

        /// <summary>
        /// Type of URL that was last called. Can be `destination_url` or `azure_oauth_client_credentials_url`
        /// </summary>
        [JsonPropertyName("last_http_call_target_type")]
        public string LastHttpCallTargetType
        {
            get { return (string)attributes["last_http_call_target_type"]; }
            set { attributes["last_http_call_target_type"] = value; }
        }

        /// <summary>
        /// Was the last HTTP call made successful?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("last_http_call_success")]
        public bool LastHttpCallSuccess
        {
            get { return attributes["last_http_call_success"] == null ? false : (bool)attributes["last_http_call_success"]; }
            set { attributes["last_http_call_success"] = value; }
        }

        /// <summary>
        /// Last HTTP Call Response Code
        /// </summary>
        [JsonPropertyName("last_http_call_response_code")]
        public Nullable<Int64> LastHttpCallResponseCode
        {
            get { return (Nullable<Int64>)attributes["last_http_call_response_code"]; }
            set { attributes["last_http_call_response_code"] = value; }
        }

        /// <summary>
        /// Last HTTP Call Response Body. Large responses are truncated.
        /// </summary>
        [JsonPropertyName("last_http_call_response_body")]
        public string LastHttpCallResponseBody
        {
            get { return (string)attributes["last_http_call_response_body"]; }
            set { attributes["last_http_call_response_body"] = value; }
        }

        /// <summary>
        /// Last HTTP Call Error Message if applicable
        /// </summary>
        [JsonPropertyName("last_http_call_error_message")]
        public string LastHttpCallErrorMessage
        {
            get { return (string)attributes["last_http_call_error_message"]; }
            set { attributes["last_http_call_error_message"] = value; }
        }

        /// <summary>
        /// Time of Last HTTP Call
        /// </summary>
        [JsonPropertyName("last_http_call_time")]
        public string LastHttpCallTime
        {
            get { return (string)attributes["last_http_call_time"]; }
            set { attributes["last_http_call_time"] = value; }
        }

        /// <summary>
        /// Duration of the last HTTP Call in milliseconds
        /// </summary>
        [JsonPropertyName("last_http_call_duration_ms")]
        public Nullable<Int64> LastHttpCallDurationMs
        {
            get { return (Nullable<Int64>)attributes["last_http_call_duration_ms"]; }
            set { attributes["last_http_call_duration_ms"] = value; }
        }

        /// <summary>
        /// Time of Most Recent Successful HTTP Call
        /// </summary>
        [JsonPropertyName("most_recent_http_call_success_time")]
        public string MostRecentHttpCallSuccessTime
        {
            get { return (string)attributes["most_recent_http_call_success_time"]; }
            set { attributes["most_recent_http_call_success_time"] = value; }
        }

        /// <summary>
        /// Connection Test Entry
        /// </summary>
        [JsonPropertyName("connection_test_entry")]
        public string ConnectionTestEntry
        {
            get { return (string)attributes["connection_test_entry"]; }
            set { attributes["connection_test_entry"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: splunk. Authentication token provided by Splunk.
        /// </summary>
        [JsonPropertyName("splunk_token")]
        public string SplunkToken
        {
            get { return (string)attributes["splunk_token"]; }
            set { attributes["splunk_token"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        /// </summary>
        [JsonPropertyName("azure_oauth_client_credentials_client_secret")]
        public string AzureOauthClientCredentialsClientSecret
        {
            get { return (string)attributes["azure_oauth_client_credentials_client_secret"]; }
            set { attributes["azure_oauth_client_credentials_client_secret"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        /// </summary>
        [JsonPropertyName("qradar_password")]
        public string QradarPassword
        {
            get { return (string)attributes["qradar_password"]; }
            set { attributes["qradar_password"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        /// </summary>
        [JsonPropertyName("solar_winds_token")]
        public string SolarWindsToken
        {
            get { return (string)attributes["solar_winds_token"]; }
            set { attributes["solar_winds_token"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: new_relic. API key provided by New Relic.
        /// </summary>
        [JsonPropertyName("new_relic_api_key")]
        public string NewRelicApiKey
        {
            get { return (string)attributes["new_relic_api_key"]; }
            set { attributes["new_relic_api_key"] = value; }
        }

        /// <summary>
        /// Applicable only for destination type: datadog. API key provided by Datadog.
        /// </summary>
        [JsonPropertyName("datadog_api_key")]
        public string DatadogApiKey
        {
            get { return (string)attributes["datadog_api_key"]; }
            set { attributes["datadog_api_key"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this Destination
        ///   additional_headers - object - Additional HTTP Headers included in calls to the destination URL
        ///   sending_active - boolean - Whether this SIEM HTTP Destination is currently being sent to or not
        ///   generic_payload_type - string - Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        ///   splunk_token - string - Applicable only for destination type: splunk. Authentication token provided by Splunk.
        ///   azure_dcr_immutable_id - string - Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination type: azure. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   sftp_action_send_enabled - boolean - Whether or not sending is enabled for sftp_action logs.
        ///   ftp_action_send_enabled - boolean - Whether or not sending is enabled for ftp_action logs.
        ///   web_dav_action_send_enabled - boolean - Whether or not sending is enabled for web_dav_action logs.
        ///   sync_send_enabled - boolean - Whether or not sending is enabled for sync logs.
        ///   outbound_connection_send_enabled - boolean - Whether or not sending is enabled for outbound_connection logs.
        ///   automation_send_enabled - boolean - Whether or not sending is enabled for automation logs.
        ///   api_request_send_enabled - boolean - Whether or not sending is enabled for api_request logs.
        ///   public_hosting_request_send_enabled - boolean - Whether or not sending is enabled for public_hosting_request logs.
        ///   email_send_enabled - boolean - Whether or not sending is enabled for email logs.
        ///   exavault_api_request_send_enabled - boolean - Whether or not sending is enabled for exavault_api_request logs.
        ///   destination_type - string - Destination Type
        ///   destination_url - string - Destination Url
        /// </summary>
        public async Task<SiemHttpDestination> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("additional_headers") && !(parameters["additional_headers"] is object))
            {
                throw new ArgumentException("Bad parameter: additional_headers must be of type object", "parameters[\"additional_headers\"]");
            }
            if (parameters.ContainsKey("sending_active") && !(parameters["sending_active"] is bool))
            {
                throw new ArgumentException("Bad parameter: sending_active must be of type bool", "parameters[\"sending_active\"]");
            }
            if (parameters.ContainsKey("generic_payload_type") && !(parameters["generic_payload_type"] is string))
            {
                throw new ArgumentException("Bad parameter: generic_payload_type must be of type string", "parameters[\"generic_payload_type\"]");
            }
            if (parameters.ContainsKey("splunk_token") && !(parameters["splunk_token"] is string))
            {
                throw new ArgumentException("Bad parameter: splunk_token must be of type string", "parameters[\"splunk_token\"]");
            }
            if (parameters.ContainsKey("azure_dcr_immutable_id") && !(parameters["azure_dcr_immutable_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_dcr_immutable_id must be of type string", "parameters[\"azure_dcr_immutable_id\"]");
            }
            if (parameters.ContainsKey("azure_stream_name") && !(parameters["azure_stream_name"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_stream_name must be of type string", "parameters[\"azure_stream_name\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_tenant_id") && !(parameters["azure_oauth_client_credentials_tenant_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_tenant_id must be of type string", "parameters[\"azure_oauth_client_credentials_tenant_id\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_client_id") && !(parameters["azure_oauth_client_credentials_client_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_client_id must be of type string", "parameters[\"azure_oauth_client_credentials_client_id\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_client_secret") && !(parameters["azure_oauth_client_credentials_client_secret"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_client_secret must be of type string", "parameters[\"azure_oauth_client_credentials_client_secret\"]");
            }
            if (parameters.ContainsKey("qradar_username") && !(parameters["qradar_username"] is string))
            {
                throw new ArgumentException("Bad parameter: qradar_username must be of type string", "parameters[\"qradar_username\"]");
            }
            if (parameters.ContainsKey("qradar_password") && !(parameters["qradar_password"] is string))
            {
                throw new ArgumentException("Bad parameter: qradar_password must be of type string", "parameters[\"qradar_password\"]");
            }
            if (parameters.ContainsKey("solar_winds_token") && !(parameters["solar_winds_token"] is string))
            {
                throw new ArgumentException("Bad parameter: solar_winds_token must be of type string", "parameters[\"solar_winds_token\"]");
            }
            if (parameters.ContainsKey("new_relic_api_key") && !(parameters["new_relic_api_key"] is string))
            {
                throw new ArgumentException("Bad parameter: new_relic_api_key must be of type string", "parameters[\"new_relic_api_key\"]");
            }
            if (parameters.ContainsKey("datadog_api_key") && !(parameters["datadog_api_key"] is string))
            {
                throw new ArgumentException("Bad parameter: datadog_api_key must be of type string", "parameters[\"datadog_api_key\"]");
            }
            if (parameters.ContainsKey("sftp_action_send_enabled") && !(parameters["sftp_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_action_send_enabled must be of type bool", "parameters[\"sftp_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("ftp_action_send_enabled") && !(parameters["ftp_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_action_send_enabled must be of type bool", "parameters[\"ftp_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("web_dav_action_send_enabled") && !(parameters["web_dav_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: web_dav_action_send_enabled must be of type bool", "parameters[\"web_dav_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("sync_send_enabled") && !(parameters["sync_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sync_send_enabled must be of type bool", "parameters[\"sync_send_enabled\"]");
            }
            if (parameters.ContainsKey("outbound_connection_send_enabled") && !(parameters["outbound_connection_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: outbound_connection_send_enabled must be of type bool", "parameters[\"outbound_connection_send_enabled\"]");
            }
            if (parameters.ContainsKey("automation_send_enabled") && !(parameters["automation_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: automation_send_enabled must be of type bool", "parameters[\"automation_send_enabled\"]");
            }
            if (parameters.ContainsKey("api_request_send_enabled") && !(parameters["api_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: api_request_send_enabled must be of type bool", "parameters[\"api_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("public_hosting_request_send_enabled") && !(parameters["public_hosting_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: public_hosting_request_send_enabled must be of type bool", "parameters[\"public_hosting_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("email_send_enabled") && !(parameters["email_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: email_send_enabled must be of type bool", "parameters[\"email_send_enabled\"]");
            }
            if (parameters.ContainsKey("exavault_api_request_send_enabled") && !(parameters["exavault_api_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: exavault_api_request_send_enabled must be of type bool", "parameters[\"exavault_api_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("destination_type") && !(parameters["destination_type"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_type must be of type string", "parameters[\"destination_type\"]");
            }
            if (parameters.ContainsKey("destination_url") && !(parameters["destination_url"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_url must be of type string", "parameters[\"destination_url\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/siem_http_destinations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<SiemHttpDestination>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/siem_http_destinations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                await this.Update(this.attributes);
            }
            else
            {
                var newObj = await SiemHttpDestination.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<SiemHttpDestination> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }

            return new FilesList<SiemHttpDestination>($"/siem_http_destinations", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<SiemHttpDestination> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Siem Http Destination ID.
        /// </summary>
        public static async Task<SiemHttpDestination> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/siem_http_destinations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<SiemHttpDestination>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<SiemHttpDestination> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this Destination
        ///   additional_headers - object - Additional HTTP Headers included in calls to the destination URL
        ///   sending_active - boolean - Whether this SIEM HTTP Destination is currently being sent to or not
        ///   generic_payload_type - string - Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        ///   splunk_token - string - Applicable only for destination type: splunk. Authentication token provided by Splunk.
        ///   azure_dcr_immutable_id - string - Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination type: azure. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   sftp_action_send_enabled - boolean - Whether or not sending is enabled for sftp_action logs.
        ///   ftp_action_send_enabled - boolean - Whether or not sending is enabled for ftp_action logs.
        ///   web_dav_action_send_enabled - boolean - Whether or not sending is enabled for web_dav_action logs.
        ///   sync_send_enabled - boolean - Whether or not sending is enabled for sync logs.
        ///   outbound_connection_send_enabled - boolean - Whether or not sending is enabled for outbound_connection logs.
        ///   automation_send_enabled - boolean - Whether or not sending is enabled for automation logs.
        ///   api_request_send_enabled - boolean - Whether or not sending is enabled for api_request logs.
        ///   public_hosting_request_send_enabled - boolean - Whether or not sending is enabled for public_hosting_request logs.
        ///   email_send_enabled - boolean - Whether or not sending is enabled for email logs.
        ///   exavault_api_request_send_enabled - boolean - Whether or not sending is enabled for exavault_api_request logs.
        ///   destination_type (required) - string - Destination Type
        ///   destination_url (required) - string - Destination Url
        /// </summary>
        public static async Task<SiemHttpDestination> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("destination_type") || parameters["destination_type"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination_type", "parameters[\"destination_type\"]");
            }
            if (!parameters.ContainsKey("destination_url") || parameters["destination_url"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination_url", "parameters[\"destination_url\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("additional_headers") && !(parameters["additional_headers"] is object))
            {
                throw new ArgumentException("Bad parameter: additional_headers must be of type object", "parameters[\"additional_headers\"]");
            }
            if (parameters.ContainsKey("sending_active") && !(parameters["sending_active"] is bool))
            {
                throw new ArgumentException("Bad parameter: sending_active must be of type bool", "parameters[\"sending_active\"]");
            }
            if (parameters.ContainsKey("generic_payload_type") && !(parameters["generic_payload_type"] is string))
            {
                throw new ArgumentException("Bad parameter: generic_payload_type must be of type string", "parameters[\"generic_payload_type\"]");
            }
            if (parameters.ContainsKey("splunk_token") && !(parameters["splunk_token"] is string))
            {
                throw new ArgumentException("Bad parameter: splunk_token must be of type string", "parameters[\"splunk_token\"]");
            }
            if (parameters.ContainsKey("azure_dcr_immutable_id") && !(parameters["azure_dcr_immutable_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_dcr_immutable_id must be of type string", "parameters[\"azure_dcr_immutable_id\"]");
            }
            if (parameters.ContainsKey("azure_stream_name") && !(parameters["azure_stream_name"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_stream_name must be of type string", "parameters[\"azure_stream_name\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_tenant_id") && !(parameters["azure_oauth_client_credentials_tenant_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_tenant_id must be of type string", "parameters[\"azure_oauth_client_credentials_tenant_id\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_client_id") && !(parameters["azure_oauth_client_credentials_client_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_client_id must be of type string", "parameters[\"azure_oauth_client_credentials_client_id\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_client_secret") && !(parameters["azure_oauth_client_credentials_client_secret"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_client_secret must be of type string", "parameters[\"azure_oauth_client_credentials_client_secret\"]");
            }
            if (parameters.ContainsKey("qradar_username") && !(parameters["qradar_username"] is string))
            {
                throw new ArgumentException("Bad parameter: qradar_username must be of type string", "parameters[\"qradar_username\"]");
            }
            if (parameters.ContainsKey("qradar_password") && !(parameters["qradar_password"] is string))
            {
                throw new ArgumentException("Bad parameter: qradar_password must be of type string", "parameters[\"qradar_password\"]");
            }
            if (parameters.ContainsKey("solar_winds_token") && !(parameters["solar_winds_token"] is string))
            {
                throw new ArgumentException("Bad parameter: solar_winds_token must be of type string", "parameters[\"solar_winds_token\"]");
            }
            if (parameters.ContainsKey("new_relic_api_key") && !(parameters["new_relic_api_key"] is string))
            {
                throw new ArgumentException("Bad parameter: new_relic_api_key must be of type string", "parameters[\"new_relic_api_key\"]");
            }
            if (parameters.ContainsKey("datadog_api_key") && !(parameters["datadog_api_key"] is string))
            {
                throw new ArgumentException("Bad parameter: datadog_api_key must be of type string", "parameters[\"datadog_api_key\"]");
            }
            if (parameters.ContainsKey("sftp_action_send_enabled") && !(parameters["sftp_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_action_send_enabled must be of type bool", "parameters[\"sftp_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("ftp_action_send_enabled") && !(parameters["ftp_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_action_send_enabled must be of type bool", "parameters[\"ftp_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("web_dav_action_send_enabled") && !(parameters["web_dav_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: web_dav_action_send_enabled must be of type bool", "parameters[\"web_dav_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("sync_send_enabled") && !(parameters["sync_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sync_send_enabled must be of type bool", "parameters[\"sync_send_enabled\"]");
            }
            if (parameters.ContainsKey("outbound_connection_send_enabled") && !(parameters["outbound_connection_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: outbound_connection_send_enabled must be of type bool", "parameters[\"outbound_connection_send_enabled\"]");
            }
            if (parameters.ContainsKey("automation_send_enabled") && !(parameters["automation_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: automation_send_enabled must be of type bool", "parameters[\"automation_send_enabled\"]");
            }
            if (parameters.ContainsKey("api_request_send_enabled") && !(parameters["api_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: api_request_send_enabled must be of type bool", "parameters[\"api_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("public_hosting_request_send_enabled") && !(parameters["public_hosting_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: public_hosting_request_send_enabled must be of type bool", "parameters[\"public_hosting_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("email_send_enabled") && !(parameters["email_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: email_send_enabled must be of type bool", "parameters[\"email_send_enabled\"]");
            }
            if (parameters.ContainsKey("exavault_api_request_send_enabled") && !(parameters["exavault_api_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: exavault_api_request_send_enabled must be of type bool", "parameters[\"exavault_api_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("destination_type") && !(parameters["destination_type"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_type must be of type string", "parameters[\"destination_type\"]");
            }
            if (parameters.ContainsKey("destination_url") && !(parameters["destination_url"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_url must be of type string", "parameters[\"destination_url\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/siem_http_destinations", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<SiemHttpDestination>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   siem_http_destination_id - int64 - SIEM HTTP Destination ID
        ///   destination_type - string - Destination Type
        ///   destination_url - string - Destination Url
        ///   name - string - Name for this Destination
        ///   additional_headers - object - Additional HTTP Headers included in calls to the destination URL
        ///   sending_active - boolean - Whether this SIEM HTTP Destination is currently being sent to or not
        ///   generic_payload_type - string - Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        ///   splunk_token - string - Applicable only for destination type: splunk. Authentication token provided by Splunk.
        ///   azure_dcr_immutable_id - string - Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination type: azure. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   sftp_action_send_enabled - boolean - Whether or not sending is enabled for sftp_action logs.
        ///   ftp_action_send_enabled - boolean - Whether or not sending is enabled for ftp_action logs.
        ///   web_dav_action_send_enabled - boolean - Whether or not sending is enabled for web_dav_action logs.
        ///   sync_send_enabled - boolean - Whether or not sending is enabled for sync logs.
        ///   outbound_connection_send_enabled - boolean - Whether or not sending is enabled for outbound_connection logs.
        ///   automation_send_enabled - boolean - Whether or not sending is enabled for automation logs.
        ///   api_request_send_enabled - boolean - Whether or not sending is enabled for api_request logs.
        ///   public_hosting_request_send_enabled - boolean - Whether or not sending is enabled for public_hosting_request logs.
        ///   email_send_enabled - boolean - Whether or not sending is enabled for email logs.
        ///   exavault_api_request_send_enabled - boolean - Whether or not sending is enabled for exavault_api_request logs.
        /// </summary>
        public static async Task SendTestEntry(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("siem_http_destination_id") && !(parameters["siem_http_destination_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: siem_http_destination_id must be of type Nullable<Int64>", "parameters[\"siem_http_destination_id\"]");
            }
            if (parameters.ContainsKey("destination_type") && !(parameters["destination_type"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_type must be of type string", "parameters[\"destination_type\"]");
            }
            if (parameters.ContainsKey("destination_url") && !(parameters["destination_url"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_url must be of type string", "parameters[\"destination_url\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("additional_headers") && !(parameters["additional_headers"] is object))
            {
                throw new ArgumentException("Bad parameter: additional_headers must be of type object", "parameters[\"additional_headers\"]");
            }
            if (parameters.ContainsKey("sending_active") && !(parameters["sending_active"] is bool))
            {
                throw new ArgumentException("Bad parameter: sending_active must be of type bool", "parameters[\"sending_active\"]");
            }
            if (parameters.ContainsKey("generic_payload_type") && !(parameters["generic_payload_type"] is string))
            {
                throw new ArgumentException("Bad parameter: generic_payload_type must be of type string", "parameters[\"generic_payload_type\"]");
            }
            if (parameters.ContainsKey("splunk_token") && !(parameters["splunk_token"] is string))
            {
                throw new ArgumentException("Bad parameter: splunk_token must be of type string", "parameters[\"splunk_token\"]");
            }
            if (parameters.ContainsKey("azure_dcr_immutable_id") && !(parameters["azure_dcr_immutable_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_dcr_immutable_id must be of type string", "parameters[\"azure_dcr_immutable_id\"]");
            }
            if (parameters.ContainsKey("azure_stream_name") && !(parameters["azure_stream_name"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_stream_name must be of type string", "parameters[\"azure_stream_name\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_tenant_id") && !(parameters["azure_oauth_client_credentials_tenant_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_tenant_id must be of type string", "parameters[\"azure_oauth_client_credentials_tenant_id\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_client_id") && !(parameters["azure_oauth_client_credentials_client_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_client_id must be of type string", "parameters[\"azure_oauth_client_credentials_client_id\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_client_secret") && !(parameters["azure_oauth_client_credentials_client_secret"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_client_secret must be of type string", "parameters[\"azure_oauth_client_credentials_client_secret\"]");
            }
            if (parameters.ContainsKey("qradar_username") && !(parameters["qradar_username"] is string))
            {
                throw new ArgumentException("Bad parameter: qradar_username must be of type string", "parameters[\"qradar_username\"]");
            }
            if (parameters.ContainsKey("qradar_password") && !(parameters["qradar_password"] is string))
            {
                throw new ArgumentException("Bad parameter: qradar_password must be of type string", "parameters[\"qradar_password\"]");
            }
            if (parameters.ContainsKey("solar_winds_token") && !(parameters["solar_winds_token"] is string))
            {
                throw new ArgumentException("Bad parameter: solar_winds_token must be of type string", "parameters[\"solar_winds_token\"]");
            }
            if (parameters.ContainsKey("new_relic_api_key") && !(parameters["new_relic_api_key"] is string))
            {
                throw new ArgumentException("Bad parameter: new_relic_api_key must be of type string", "parameters[\"new_relic_api_key\"]");
            }
            if (parameters.ContainsKey("datadog_api_key") && !(parameters["datadog_api_key"] is string))
            {
                throw new ArgumentException("Bad parameter: datadog_api_key must be of type string", "parameters[\"datadog_api_key\"]");
            }
            if (parameters.ContainsKey("sftp_action_send_enabled") && !(parameters["sftp_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_action_send_enabled must be of type bool", "parameters[\"sftp_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("ftp_action_send_enabled") && !(parameters["ftp_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_action_send_enabled must be of type bool", "parameters[\"ftp_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("web_dav_action_send_enabled") && !(parameters["web_dav_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: web_dav_action_send_enabled must be of type bool", "parameters[\"web_dav_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("sync_send_enabled") && !(parameters["sync_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sync_send_enabled must be of type bool", "parameters[\"sync_send_enabled\"]");
            }
            if (parameters.ContainsKey("outbound_connection_send_enabled") && !(parameters["outbound_connection_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: outbound_connection_send_enabled must be of type bool", "parameters[\"outbound_connection_send_enabled\"]");
            }
            if (parameters.ContainsKey("automation_send_enabled") && !(parameters["automation_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: automation_send_enabled must be of type bool", "parameters[\"automation_send_enabled\"]");
            }
            if (parameters.ContainsKey("api_request_send_enabled") && !(parameters["api_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: api_request_send_enabled must be of type bool", "parameters[\"api_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("public_hosting_request_send_enabled") && !(parameters["public_hosting_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: public_hosting_request_send_enabled must be of type bool", "parameters[\"public_hosting_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("email_send_enabled") && !(parameters["email_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: email_send_enabled must be of type bool", "parameters[\"email_send_enabled\"]");
            }
            if (parameters.ContainsKey("exavault_api_request_send_enabled") && !(parameters["exavault_api_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: exavault_api_request_send_enabled must be of type bool", "parameters[\"exavault_api_request_send_enabled\"]");
            }

            await FilesClient.SendRequest($"/siem_http_destinations/send_test_entry", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Name for this Destination
        ///   additional_headers - object - Additional HTTP Headers included in calls to the destination URL
        ///   sending_active - boolean - Whether this SIEM HTTP Destination is currently being sent to or not
        ///   generic_payload_type - string - Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        ///   splunk_token - string - Applicable only for destination type: splunk. Authentication token provided by Splunk.
        ///   azure_dcr_immutable_id - string - Applicable only for destination type: azure. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination type: azure. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination type: azure. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   sftp_action_send_enabled - boolean - Whether or not sending is enabled for sftp_action logs.
        ///   ftp_action_send_enabled - boolean - Whether or not sending is enabled for ftp_action logs.
        ///   web_dav_action_send_enabled - boolean - Whether or not sending is enabled for web_dav_action logs.
        ///   sync_send_enabled - boolean - Whether or not sending is enabled for sync logs.
        ///   outbound_connection_send_enabled - boolean - Whether or not sending is enabled for outbound_connection logs.
        ///   automation_send_enabled - boolean - Whether or not sending is enabled for automation logs.
        ///   api_request_send_enabled - boolean - Whether or not sending is enabled for api_request logs.
        ///   public_hosting_request_send_enabled - boolean - Whether or not sending is enabled for public_hosting_request logs.
        ///   email_send_enabled - boolean - Whether or not sending is enabled for email logs.
        ///   exavault_api_request_send_enabled - boolean - Whether or not sending is enabled for exavault_api_request logs.
        ///   destination_type - string - Destination Type
        ///   destination_url - string - Destination Url
        /// </summary>
        public static async Task<SiemHttpDestination> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("additional_headers") && !(parameters["additional_headers"] is object))
            {
                throw new ArgumentException("Bad parameter: additional_headers must be of type object", "parameters[\"additional_headers\"]");
            }
            if (parameters.ContainsKey("sending_active") && !(parameters["sending_active"] is bool))
            {
                throw new ArgumentException("Bad parameter: sending_active must be of type bool", "parameters[\"sending_active\"]");
            }
            if (parameters.ContainsKey("generic_payload_type") && !(parameters["generic_payload_type"] is string))
            {
                throw new ArgumentException("Bad parameter: generic_payload_type must be of type string", "parameters[\"generic_payload_type\"]");
            }
            if (parameters.ContainsKey("splunk_token") && !(parameters["splunk_token"] is string))
            {
                throw new ArgumentException("Bad parameter: splunk_token must be of type string", "parameters[\"splunk_token\"]");
            }
            if (parameters.ContainsKey("azure_dcr_immutable_id") && !(parameters["azure_dcr_immutable_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_dcr_immutable_id must be of type string", "parameters[\"azure_dcr_immutable_id\"]");
            }
            if (parameters.ContainsKey("azure_stream_name") && !(parameters["azure_stream_name"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_stream_name must be of type string", "parameters[\"azure_stream_name\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_tenant_id") && !(parameters["azure_oauth_client_credentials_tenant_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_tenant_id must be of type string", "parameters[\"azure_oauth_client_credentials_tenant_id\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_client_id") && !(parameters["azure_oauth_client_credentials_client_id"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_client_id must be of type string", "parameters[\"azure_oauth_client_credentials_client_id\"]");
            }
            if (parameters.ContainsKey("azure_oauth_client_credentials_client_secret") && !(parameters["azure_oauth_client_credentials_client_secret"] is string))
            {
                throw new ArgumentException("Bad parameter: azure_oauth_client_credentials_client_secret must be of type string", "parameters[\"azure_oauth_client_credentials_client_secret\"]");
            }
            if (parameters.ContainsKey("qradar_username") && !(parameters["qradar_username"] is string))
            {
                throw new ArgumentException("Bad parameter: qradar_username must be of type string", "parameters[\"qradar_username\"]");
            }
            if (parameters.ContainsKey("qradar_password") && !(parameters["qradar_password"] is string))
            {
                throw new ArgumentException("Bad parameter: qradar_password must be of type string", "parameters[\"qradar_password\"]");
            }
            if (parameters.ContainsKey("solar_winds_token") && !(parameters["solar_winds_token"] is string))
            {
                throw new ArgumentException("Bad parameter: solar_winds_token must be of type string", "parameters[\"solar_winds_token\"]");
            }
            if (parameters.ContainsKey("new_relic_api_key") && !(parameters["new_relic_api_key"] is string))
            {
                throw new ArgumentException("Bad parameter: new_relic_api_key must be of type string", "parameters[\"new_relic_api_key\"]");
            }
            if (parameters.ContainsKey("datadog_api_key") && !(parameters["datadog_api_key"] is string))
            {
                throw new ArgumentException("Bad parameter: datadog_api_key must be of type string", "parameters[\"datadog_api_key\"]");
            }
            if (parameters.ContainsKey("sftp_action_send_enabled") && !(parameters["sftp_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sftp_action_send_enabled must be of type bool", "parameters[\"sftp_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("ftp_action_send_enabled") && !(parameters["ftp_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: ftp_action_send_enabled must be of type bool", "parameters[\"ftp_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("web_dav_action_send_enabled") && !(parameters["web_dav_action_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: web_dav_action_send_enabled must be of type bool", "parameters[\"web_dav_action_send_enabled\"]");
            }
            if (parameters.ContainsKey("sync_send_enabled") && !(parameters["sync_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: sync_send_enabled must be of type bool", "parameters[\"sync_send_enabled\"]");
            }
            if (parameters.ContainsKey("outbound_connection_send_enabled") && !(parameters["outbound_connection_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: outbound_connection_send_enabled must be of type bool", "parameters[\"outbound_connection_send_enabled\"]");
            }
            if (parameters.ContainsKey("automation_send_enabled") && !(parameters["automation_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: automation_send_enabled must be of type bool", "parameters[\"automation_send_enabled\"]");
            }
            if (parameters.ContainsKey("api_request_send_enabled") && !(parameters["api_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: api_request_send_enabled must be of type bool", "parameters[\"api_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("public_hosting_request_send_enabled") && !(parameters["public_hosting_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: public_hosting_request_send_enabled must be of type bool", "parameters[\"public_hosting_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("email_send_enabled") && !(parameters["email_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: email_send_enabled must be of type bool", "parameters[\"email_send_enabled\"]");
            }
            if (parameters.ContainsKey("exavault_api_request_send_enabled") && !(parameters["exavault_api_request_send_enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: exavault_api_request_send_enabled must be of type bool", "parameters[\"exavault_api_request_send_enabled\"]");
            }
            if (parameters.ContainsKey("destination_type") && !(parameters["destination_type"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_type must be of type string", "parameters[\"destination_type\"]");
            }
            if (parameters.ContainsKey("destination_url") && !(parameters["destination_url"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_url must be of type string", "parameters[\"destination_url\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/siem_http_destinations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<SiemHttpDestination>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public static async Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/siem_http_destinations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(id, parameters, options);
        }

    }
}