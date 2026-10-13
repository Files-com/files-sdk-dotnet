using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// SiemHttpDestinationEntity model
    /// </summary>
    public class SiemHttpDestination : IModel, IJsonOnDeserialized
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        // The names of the attributes set on this object, by its constructor or its properties, since it was made
        // or last came from Files.com. SaveAsync sends these, never the defaults of attributes nobody set.
        private readonly HashSet<string> unsaved = new HashSet<string>();
        /// <summary>
        /// Makes a SiemHttpDestination with no attributes set and no options.
        /// </summary>
        public SiemHttpDestination() : this(null, null) { }

        /// <summary>
        /// Makes a SiemHttpDestination with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public SiemHttpDestination(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

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
            if (!this.attributes.ContainsKey("file_destination_path"))
            {
                this.attributes.Add("file_destination_path", null);
            }
            if (!this.attributes.ContainsKey("file_format"))
            {
                this.attributes.Add("file_format", null);
            }
            if (!this.attributes.ContainsKey("file_interval_minutes"))
            {
                this.attributes.Add("file_interval_minutes", null);
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
            if (!this.attributes.ContainsKey("crowdstrike_token_masked"))
            {
                this.attributes.Add("crowdstrike_token_masked", null);
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
            if (!this.attributes.ContainsKey("action_send_enabled"))
            {
                this.attributes.Add("action_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("action_entries_sent"))
            {
                this.attributes.Add("action_entries_sent", null);
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
            if (!this.attributes.ContainsKey("settings_change_send_enabled"))
            {
                this.attributes.Add("settings_change_send_enabled", false);
            }
            if (!this.attributes.ContainsKey("settings_change_entries_sent"))
            {
                this.attributes.Add("settings_change_entries_sent", null);
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
            if (!this.attributes.ContainsKey("crowdstrike_token"))
            {
                this.attributes.Add("crowdstrike_token", null);
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

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.client = client;
            this.origin = origin;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => origin;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[0]; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }

        // What is read from JSON, such as a response from Files.com, is not waiting to be saved.
        void IJsonOnDeserialized.OnDeserialized()
        {
            unsaved.Clear();
        }


        /// <summary>
        /// SIEM HTTP Destination ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Name for this Destination
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Destination Type
        /// </summary>
        [JsonPropertyName("destination_type")]
        public string DestinationType
        {
            get { return (string)attributes["destination_type"]; }
            set { attributes["destination_type"] = value; unsaved.Add("destination_type"); }
        }

        /// <summary>
        /// Destination Url
        /// </summary>
        [JsonPropertyName("destination_url")]
        public string DestinationUrl
        {
            get { return (string)attributes["destination_url"]; }
            set { attributes["destination_url"] = value; unsaved.Add("destination_url"); }
        }

        /// <summary>
        /// Applicable only for destination type: file. Destination folder path on Files.com.
        /// </summary>
        [JsonPropertyName("file_destination_path")]
        public string FileDestinationPath
        {
            get { return (string)attributes["file_destination_path"]; }
            set { attributes["file_destination_path"] = value; unsaved.Add("file_destination_path"); }
        }

        /// <summary>
        /// Applicable only for destination type: file. Generated file format.
        /// </summary>
        [JsonPropertyName("file_format")]
        public string FileFormat
        {
            get { return (string)attributes["file_format"]; }
            set { attributes["file_format"] = value; unsaved.Add("file_format"); }
        }

        /// <summary>
        /// Applicable only for destination type: file. Interval, in minutes, between file deliveries.
        /// </summary>
        [JsonPropertyName("file_interval_minutes")]
        public Nullable<Int64> FileIntervalMinutes
        {
            get { return (Nullable<Int64>)attributes["file_interval_minutes"]; }
            set { attributes["file_interval_minutes"] = value; unsaved.Add("file_interval_minutes"); }
        }

        /// <summary>
        /// Additional HTTP Headers included in calls to the destination URL
        /// </summary>
        [JsonPropertyName("additional_headers")]
        public object AdditionalHeaders
        {
            get { return (object)attributes["additional_headers"]; }
            set { attributes["additional_headers"] = value; unsaved.Add("additional_headers"); }
        }

        /// <summary>
        /// Whether this SIEM HTTP Destination is currently being sent to or not
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sending_active")]
        public bool SendingActive
        {
            get { return attributes["sending_active"] == null ? false : (bool)attributes["sending_active"]; }
            set { attributes["sending_active"] = value; unsaved.Add("sending_active"); }
        }

        /// <summary>
        /// Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        /// </summary>
        [JsonPropertyName("generic_payload_type")]
        public string GenericPayloadType
        {
            get { return (string)attributes["generic_payload_type"]; }
            set { attributes["generic_payload_type"] = value; unsaved.Add("generic_payload_type"); }
        }

        /// <summary>
        /// Applicable only for destination types: splunk, splunk_compatible. Authentication token for the destination.
        /// </summary>
        [JsonPropertyName("splunk_token_masked")]
        public string SplunkTokenMasked
        {
            get { return (string)attributes["splunk_token_masked"]; }
            set { attributes["splunk_token_masked"] = value; unsaved.Add("splunk_token_masked"); }
        }

        /// <summary>
        /// Applicable only for destination type: crowdstrike. Authentication token provided by Crowdstrike.
        /// </summary>
        [JsonPropertyName("crowdstrike_token_masked")]
        public string CrowdstrikeTokenMasked
        {
            get { return (string)attributes["crowdstrike_token_masked"]; }
            set { attributes["crowdstrike_token_masked"] = value; unsaved.Add("crowdstrike_token_masked"); }
        }

        /// <summary>
        /// Applicable only for destination types: azure, azure_legacy. Immutable ID of the Data Collection Rule.
        /// </summary>
        [JsonPropertyName("azure_dcr_immutable_id")]
        public string AzureDcrImmutableId
        {
            get { return (string)attributes["azure_dcr_immutable_id"]; }
            set { attributes["azure_dcr_immutable_id"] = value; unsaved.Add("azure_dcr_immutable_id"); }
        }

        /// <summary>
        /// Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        /// </summary>
        [JsonPropertyName("azure_stream_name")]
        public string AzureStreamName
        {
            get { return (string)attributes["azure_stream_name"]; }
            set { attributes["azure_stream_name"] = value; unsaved.Add("azure_stream_name"); }
        }

        /// <summary>
        /// Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Tenant ID.
        /// </summary>
        [JsonPropertyName("azure_oauth_client_credentials_tenant_id")]
        public string AzureOauthClientCredentialsTenantId
        {
            get { return (string)attributes["azure_oauth_client_credentials_tenant_id"]; }
            set { attributes["azure_oauth_client_credentials_tenant_id"] = value; unsaved.Add("azure_oauth_client_credentials_tenant_id"); }
        }

        /// <summary>
        /// Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Client ID.
        /// </summary>
        [JsonPropertyName("azure_oauth_client_credentials_client_id")]
        public string AzureOauthClientCredentialsClientId
        {
            get { return (string)attributes["azure_oauth_client_credentials_client_id"]; }
            set { attributes["azure_oauth_client_credentials_client_id"] = value; unsaved.Add("azure_oauth_client_credentials_client_id"); }
        }

        /// <summary>
        /// Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Client Secret.
        /// </summary>
        [JsonPropertyName("azure_oauth_client_credentials_client_secret_masked")]
        public string AzureOauthClientCredentialsClientSecretMasked
        {
            get { return (string)attributes["azure_oauth_client_credentials_client_secret_masked"]; }
            set { attributes["azure_oauth_client_credentials_client_secret_masked"] = value; unsaved.Add("azure_oauth_client_credentials_client_secret_masked"); }
        }

        /// <summary>
        /// Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        /// </summary>
        [JsonPropertyName("qradar_username")]
        public string QradarUsername
        {
            get { return (string)attributes["qradar_username"]; }
            set { attributes["qradar_username"] = value; unsaved.Add("qradar_username"); }
        }

        /// <summary>
        /// Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        /// </summary>
        [JsonPropertyName("qradar_password_masked")]
        public string QradarPasswordMasked
        {
            get { return (string)attributes["qradar_password_masked"]; }
            set { attributes["qradar_password_masked"] = value; unsaved.Add("qradar_password_masked"); }
        }

        /// <summary>
        /// Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        /// </summary>
        [JsonPropertyName("solar_winds_token_masked")]
        public string SolarWindsTokenMasked
        {
            get { return (string)attributes["solar_winds_token_masked"]; }
            set { attributes["solar_winds_token_masked"] = value; unsaved.Add("solar_winds_token_masked"); }
        }

        /// <summary>
        /// Applicable only for destination type: new_relic. API key provided by New Relic.
        /// </summary>
        [JsonPropertyName("new_relic_api_key_masked")]
        public string NewRelicApiKeyMasked
        {
            get { return (string)attributes["new_relic_api_key_masked"]; }
            set { attributes["new_relic_api_key_masked"] = value; unsaved.Add("new_relic_api_key_masked"); }
        }

        /// <summary>
        /// Applicable only for destination type: datadog. API key provided by Datadog.
        /// </summary>
        [JsonPropertyName("datadog_api_key_masked")]
        public string DatadogApiKeyMasked
        {
            get { return (string)attributes["datadog_api_key_masked"]; }
            set { attributes["datadog_api_key_masked"] = value; unsaved.Add("datadog_api_key_masked"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for action logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("action_send_enabled")]
        public bool ActionSendEnabled
        {
            get { return attributes["action_send_enabled"] == null ? false : (bool)attributes["action_send_enabled"]; }
            set { attributes["action_send_enabled"] = value; unsaved.Add("action_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("action_entries_sent")]
        public Nullable<Int64> ActionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["action_entries_sent"]; }
            set { attributes["action_entries_sent"] = value; unsaved.Add("action_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for sftp_action logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sftp_action_send_enabled")]
        public bool SftpActionSendEnabled
        {
            get { return attributes["sftp_action_send_enabled"] == null ? false : (bool)attributes["sftp_action_send_enabled"]; }
            set { attributes["sftp_action_send_enabled"] = value; unsaved.Add("sftp_action_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("sftp_action_entries_sent")]
        public Nullable<Int64> SftpActionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["sftp_action_entries_sent"]; }
            set { attributes["sftp_action_entries_sent"] = value; unsaved.Add("sftp_action_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for ftp_action logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ftp_action_send_enabled")]
        public bool FtpActionSendEnabled
        {
            get { return attributes["ftp_action_send_enabled"] == null ? false : (bool)attributes["ftp_action_send_enabled"]; }
            set { attributes["ftp_action_send_enabled"] = value; unsaved.Add("ftp_action_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("ftp_action_entries_sent")]
        public Nullable<Int64> FtpActionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["ftp_action_entries_sent"]; }
            set { attributes["ftp_action_entries_sent"] = value; unsaved.Add("ftp_action_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for web_dav_action logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("web_dav_action_send_enabled")]
        public bool WebDavActionSendEnabled
        {
            get { return attributes["web_dav_action_send_enabled"] == null ? false : (bool)attributes["web_dav_action_send_enabled"]; }
            set { attributes["web_dav_action_send_enabled"] = value; unsaved.Add("web_dav_action_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("web_dav_action_entries_sent")]
        public Nullable<Int64> WebDavActionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["web_dav_action_entries_sent"]; }
            set { attributes["web_dav_action_entries_sent"] = value; unsaved.Add("web_dav_action_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for sync logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("sync_send_enabled")]
        public bool SyncSendEnabled
        {
            get { return attributes["sync_send_enabled"] == null ? false : (bool)attributes["sync_send_enabled"]; }
            set { attributes["sync_send_enabled"] = value; unsaved.Add("sync_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("sync_entries_sent")]
        public Nullable<Int64> SyncEntriesSent
        {
            get { return (Nullable<Int64>)attributes["sync_entries_sent"]; }
            set { attributes["sync_entries_sent"] = value; unsaved.Add("sync_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for outbound_connection logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("outbound_connection_send_enabled")]
        public bool OutboundConnectionSendEnabled
        {
            get { return attributes["outbound_connection_send_enabled"] == null ? false : (bool)attributes["outbound_connection_send_enabled"]; }
            set { attributes["outbound_connection_send_enabled"] = value; unsaved.Add("outbound_connection_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("outbound_connection_entries_sent")]
        public Nullable<Int64> OutboundConnectionEntriesSent
        {
            get { return (Nullable<Int64>)attributes["outbound_connection_entries_sent"]; }
            set { attributes["outbound_connection_entries_sent"] = value; unsaved.Add("outbound_connection_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for automation logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("automation_send_enabled")]
        public bool AutomationSendEnabled
        {
            get { return attributes["automation_send_enabled"] == null ? false : (bool)attributes["automation_send_enabled"]; }
            set { attributes["automation_send_enabled"] = value; unsaved.Add("automation_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("automation_entries_sent")]
        public Nullable<Int64> AutomationEntriesSent
        {
            get { return (Nullable<Int64>)attributes["automation_entries_sent"]; }
            set { attributes["automation_entries_sent"] = value; unsaved.Add("automation_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for api_request logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("api_request_send_enabled")]
        public bool ApiRequestSendEnabled
        {
            get { return attributes["api_request_send_enabled"] == null ? false : (bool)attributes["api_request_send_enabled"]; }
            set { attributes["api_request_send_enabled"] = value; unsaved.Add("api_request_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("api_request_entries_sent")]
        public Nullable<Int64> ApiRequestEntriesSent
        {
            get { return (Nullable<Int64>)attributes["api_request_entries_sent"]; }
            set { attributes["api_request_entries_sent"] = value; unsaved.Add("api_request_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for public_hosting_request logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("public_hosting_request_send_enabled")]
        public bool PublicHostingRequestSendEnabled
        {
            get { return attributes["public_hosting_request_send_enabled"] == null ? false : (bool)attributes["public_hosting_request_send_enabled"]; }
            set { attributes["public_hosting_request_send_enabled"] = value; unsaved.Add("public_hosting_request_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("public_hosting_request_entries_sent")]
        public Nullable<Int64> PublicHostingRequestEntriesSent
        {
            get { return (Nullable<Int64>)attributes["public_hosting_request_entries_sent"]; }
            set { attributes["public_hosting_request_entries_sent"] = value; unsaved.Add("public_hosting_request_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for email logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("email_send_enabled")]
        public bool EmailSendEnabled
        {
            get { return attributes["email_send_enabled"] == null ? false : (bool)attributes["email_send_enabled"]; }
            set { attributes["email_send_enabled"] = value; unsaved.Add("email_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("email_entries_sent")]
        public Nullable<Int64> EmailEntriesSent
        {
            get { return (Nullable<Int64>)attributes["email_entries_sent"]; }
            set { attributes["email_entries_sent"] = value; unsaved.Add("email_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for exavault_api_request logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("exavault_api_request_send_enabled")]
        public bool ExavaultApiRequestSendEnabled
        {
            get { return attributes["exavault_api_request_send_enabled"] == null ? false : (bool)attributes["exavault_api_request_send_enabled"]; }
            set { attributes["exavault_api_request_send_enabled"] = value; unsaved.Add("exavault_api_request_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("exavault_api_request_entries_sent")]
        public Nullable<Int64> ExavaultApiRequestEntriesSent
        {
            get { return (Nullable<Int64>)attributes["exavault_api_request_entries_sent"]; }
            set { attributes["exavault_api_request_entries_sent"] = value; unsaved.Add("exavault_api_request_entries_sent"); }
        }

        /// <summary>
        /// Whether or not sending is enabled for settings_change logs.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("settings_change_send_enabled")]
        public bool SettingsChangeSendEnabled
        {
            get { return attributes["settings_change_send_enabled"] == null ? false : (bool)attributes["settings_change_send_enabled"]; }
            set { attributes["settings_change_send_enabled"] = value; unsaved.Add("settings_change_send_enabled"); }
        }

        /// <summary>
        /// Number of log entries sent for the lifetime of this destination.
        /// </summary>
        [JsonPropertyName("settings_change_entries_sent")]
        public Nullable<Int64> SettingsChangeEntriesSent
        {
            get { return (Nullable<Int64>)attributes["settings_change_entries_sent"]; }
            set { attributes["settings_change_entries_sent"] = value; unsaved.Add("settings_change_entries_sent"); }
        }

        /// <summary>
        /// Type of URL that was last called. Can be `destination_url` or `azure_oauth_client_credentials_url`
        /// </summary>
        [JsonPropertyName("last_http_call_target_type")]
        public string LastHttpCallTargetType
        {
            get { return (string)attributes["last_http_call_target_type"]; }
            set { attributes["last_http_call_target_type"] = value; unsaved.Add("last_http_call_target_type"); }
        }

        /// <summary>
        /// Was the last HTTP call made successful?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("last_http_call_success")]
        public bool LastHttpCallSuccess
        {
            get { return attributes["last_http_call_success"] == null ? false : (bool)attributes["last_http_call_success"]; }
            set { attributes["last_http_call_success"] = value; unsaved.Add("last_http_call_success"); }
        }

        /// <summary>
        /// Last HTTP Call Response Code
        /// </summary>
        [JsonPropertyName("last_http_call_response_code")]
        public Nullable<Int64> LastHttpCallResponseCode
        {
            get { return (Nullable<Int64>)attributes["last_http_call_response_code"]; }
            set { attributes["last_http_call_response_code"] = value; unsaved.Add("last_http_call_response_code"); }
        }

        /// <summary>
        /// Last HTTP Call Response Body. Large responses are truncated.
        /// </summary>
        [JsonPropertyName("last_http_call_response_body")]
        public string LastHttpCallResponseBody
        {
            get { return (string)attributes["last_http_call_response_body"]; }
            set { attributes["last_http_call_response_body"] = value; unsaved.Add("last_http_call_response_body"); }
        }

        /// <summary>
        /// Last HTTP Call Error Message if applicable
        /// </summary>
        [JsonPropertyName("last_http_call_error_message")]
        public string LastHttpCallErrorMessage
        {
            get { return (string)attributes["last_http_call_error_message"]; }
            set { attributes["last_http_call_error_message"] = value; unsaved.Add("last_http_call_error_message"); }
        }

        /// <summary>
        /// Time of Last HTTP Call
        /// </summary>
        [JsonPropertyName("last_http_call_time")]
        public string LastHttpCallTime
        {
            get { return (string)attributes["last_http_call_time"]; }
            set { attributes["last_http_call_time"] = value; unsaved.Add("last_http_call_time"); }
        }

        /// <summary>
        /// Duration of the last HTTP Call in milliseconds
        /// </summary>
        [JsonPropertyName("last_http_call_duration_ms")]
        public Nullable<Int64> LastHttpCallDurationMs
        {
            get { return (Nullable<Int64>)attributes["last_http_call_duration_ms"]; }
            set { attributes["last_http_call_duration_ms"] = value; unsaved.Add("last_http_call_duration_ms"); }
        }

        /// <summary>
        /// Time of Most Recent Successful HTTP Call
        /// </summary>
        [JsonPropertyName("most_recent_http_call_success_time")]
        public string MostRecentHttpCallSuccessTime
        {
            get { return (string)attributes["most_recent_http_call_success_time"]; }
            set { attributes["most_recent_http_call_success_time"] = value; unsaved.Add("most_recent_http_call_success_time"); }
        }

        /// <summary>
        /// Connection Test Entry
        /// </summary>
        [JsonPropertyName("connection_test_entry")]
        public string ConnectionTestEntry
        {
            get { return (string)attributes["connection_test_entry"]; }
            set { attributes["connection_test_entry"] = value; unsaved.Add("connection_test_entry"); }
        }

        /// <summary>
        /// Applicable only for destination types: splunk, splunk_compatible. Authentication token for the destination.
        /// </summary>
        [JsonPropertyName("splunk_token")]
        public string SplunkToken
        {
            get { return (string)attributes["splunk_token"]; }
            set { attributes["splunk_token"] = value; unsaved.Add("splunk_token"); }
        }

        /// <summary>
        /// Applicable only for destination type: crowdstrike. Authentication token provided by Crowdstrike.
        /// </summary>
        [JsonPropertyName("crowdstrike_token")]
        public string CrowdstrikeToken
        {
            get { return (string)attributes["crowdstrike_token"]; }
            set { attributes["crowdstrike_token"] = value; unsaved.Add("crowdstrike_token"); }
        }

        /// <summary>
        /// Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        /// </summary>
        [JsonPropertyName("azure_oauth_client_credentials_client_secret")]
        public string AzureOauthClientCredentialsClientSecret
        {
            get { return (string)attributes["azure_oauth_client_credentials_client_secret"]; }
            set { attributes["azure_oauth_client_credentials_client_secret"] = value; unsaved.Add("azure_oauth_client_credentials_client_secret"); }
        }

        /// <summary>
        /// Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        /// </summary>
        [JsonPropertyName("qradar_password")]
        public string QradarPassword
        {
            get { return (string)attributes["qradar_password"]; }
            set { attributes["qradar_password"] = value; unsaved.Add("qradar_password"); }
        }

        /// <summary>
        /// Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        /// </summary>
        [JsonPropertyName("solar_winds_token")]
        public string SolarWindsToken
        {
            get { return (string)attributes["solar_winds_token"]; }
            set { attributes["solar_winds_token"] = value; unsaved.Add("solar_winds_token"); }
        }

        /// <summary>
        /// Applicable only for destination type: new_relic. API key provided by New Relic.
        /// </summary>
        [JsonPropertyName("new_relic_api_key")]
        public string NewRelicApiKey
        {
            get { return (string)attributes["new_relic_api_key"]; }
            set { attributes["new_relic_api_key"] = value; unsaved.Add("new_relic_api_key"); }
        }

        /// <summary>
        /// Applicable only for destination type: datadog. API key provided by Datadog.
        /// </summary>
        [JsonPropertyName("datadog_api_key")]
        public string DatadogApiKey
        {
            get { return (string)attributes["datadog_api_key"]; }
            set { attributes["datadog_api_key"] = value; unsaved.Add("datadog_api_key"); }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this Destination
        ///   additional_headers - object - Additional HTTP Headers included in calls to the destination URL
        ///   sending_active - boolean - Whether this SIEM HTTP Destination is currently being sent to or not
        ///   generic_payload_type - string - Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        ///   file_destination_path - string - Applicable only for destination type: file. Destination folder path on Files.com.
        ///   file_format - string - Applicable only for destination type: file. Generated file format.
        ///   file_interval_minutes - int64 - Applicable only for destination type: file. Interval, in minutes, between file deliveries. Valid values are 5, 10, 15, 20, 30, 60, 90, 180, 240, 360.
        ///   splunk_token - string - Applicable only for destination types: splunk, splunk_compatible. Authentication token for the destination.
        ///   crowdstrike_token - string - Applicable only for destination type: crowdstrike. Authentication token provided by Crowdstrike.
        ///   azure_dcr_immutable_id - string - Applicable only for destination types: azure, azure_legacy. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   action_send_enabled - boolean - Whether or not sending is enabled for action logs.
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
        ///   settings_change_send_enabled - boolean - Whether or not sending is enabled for settings_change logs.
        ///   destination_type - string - Destination Type
        ///   destination_url - string - Destination Url
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The SiemHttpDestination it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<SiemHttpDestination> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this Destination
        ///   additional_headers - object - Additional HTTP Headers included in calls to the destination URL
        ///   sending_active - boolean - Whether this SIEM HTTP Destination is currently being sent to or not
        ///   generic_payload_type - string - Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        ///   file_destination_path - string - Applicable only for destination type: file. Destination folder path on Files.com.
        ///   file_format - string - Applicable only for destination type: file. Generated file format.
        ///   file_interval_minutes - int64 - Applicable only for destination type: file. Interval, in minutes, between file deliveries. Valid values are 5, 10, 15, 20, 30, 60, 90, 180, 240, 360.
        ///   splunk_token - string - Applicable only for destination types: splunk, splunk_compatible. Authentication token for the destination.
        ///   crowdstrike_token - string - Applicable only for destination type: crowdstrike. Authentication token provided by Crowdstrike.
        ///   azure_dcr_immutable_id - string - Applicable only for destination types: azure, azure_legacy. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   action_send_enabled - boolean - Whether or not sending is enabled for action logs.
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
        ///   settings_change_send_enabled - boolean - Whether or not sending is enabled for settings_change logs.
        ///   destination_type - string - Destination Type
        ///   destination_url - string - Destination Url
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The SiemHttpDestination it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<SiemHttpDestination> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<SiemHttpDestination> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The SiemHttpDestination has no id, which Update needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("additional_headers", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: additional_headers is null, which cannot be sent" : "Bad parameter: additional_headers must be of type object", "additional_headers");
            }
            if (parameters.TryGetValue("sending_active", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sending_active is null, which cannot be sent" : "Bad parameter: sending_active must be of type bool", "sending_active");
            }
            if (parameters.TryGetValue("generic_payload_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: generic_payload_type is null, which cannot be sent" : "Bad parameter: generic_payload_type must be of type string", "generic_payload_type");
            }
            if (parameters.TryGetValue("file_destination_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_destination_path is null, which cannot be sent" : "Bad parameter: file_destination_path must be of type string", "file_destination_path");
            }
            if (parameters.TryGetValue("file_format", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_format is null, which cannot be sent" : "Bad parameter: file_format must be of type string", "file_format");
            }
            if (parameters.TryGetValue("file_interval_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_interval_minutes is null, which cannot be sent" : "Bad parameter: file_interval_minutes must be of type Nullable<Int64>", "file_interval_minutes");
            }
            if (parameters.TryGetValue("splunk_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: splunk_token is null, which cannot be sent" : "Bad parameter: splunk_token must be of type string", "splunk_token");
            }
            if (parameters.TryGetValue("crowdstrike_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: crowdstrike_token is null, which cannot be sent" : "Bad parameter: crowdstrike_token must be of type string", "crowdstrike_token");
            }
            if (parameters.TryGetValue("azure_dcr_immutable_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_dcr_immutable_id is null, which cannot be sent" : "Bad parameter: azure_dcr_immutable_id must be of type string", "azure_dcr_immutable_id");
            }
            if (parameters.TryGetValue("azure_stream_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_stream_name is null, which cannot be sent" : "Bad parameter: azure_stream_name must be of type string", "azure_stream_name");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_tenant_id is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_tenant_id must be of type string", "azure_oauth_client_credentials_tenant_id");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_client_id is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_client_id must be of type string", "azure_oauth_client_credentials_client_id");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_client_secret is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_client_secret must be of type string", "azure_oauth_client_credentials_client_secret");
            }
            if (parameters.TryGetValue("qradar_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: qradar_username is null, which cannot be sent" : "Bad parameter: qradar_username must be of type string", "qradar_username");
            }
            if (parameters.TryGetValue("qradar_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: qradar_password is null, which cannot be sent" : "Bad parameter: qradar_password must be of type string", "qradar_password");
            }
            if (parameters.TryGetValue("solar_winds_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: solar_winds_token is null, which cannot be sent" : "Bad parameter: solar_winds_token must be of type string", "solar_winds_token");
            }
            if (parameters.TryGetValue("new_relic_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: new_relic_api_key is null, which cannot be sent" : "Bad parameter: new_relic_api_key must be of type string", "new_relic_api_key");
            }
            if (parameters.TryGetValue("datadog_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: datadog_api_key is null, which cannot be sent" : "Bad parameter: datadog_api_key must be of type string", "datadog_api_key");
            }
            if (parameters.TryGetValue("action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action_send_enabled is null, which cannot be sent" : "Bad parameter: action_send_enabled must be of type bool", "action_send_enabled");
            }
            if (parameters.TryGetValue("sftp_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_action_send_enabled is null, which cannot be sent" : "Bad parameter: sftp_action_send_enabled must be of type bool", "sftp_action_send_enabled");
            }
            if (parameters.TryGetValue("ftp_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ftp_action_send_enabled is null, which cannot be sent" : "Bad parameter: ftp_action_send_enabled must be of type bool", "ftp_action_send_enabled");
            }
            if (parameters.TryGetValue("web_dav_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: web_dav_action_send_enabled is null, which cannot be sent" : "Bad parameter: web_dav_action_send_enabled must be of type bool", "web_dav_action_send_enabled");
            }
            if (parameters.TryGetValue("sync_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_send_enabled is null, which cannot be sent" : "Bad parameter: sync_send_enabled must be of type bool", "sync_send_enabled");
            }
            if (parameters.TryGetValue("outbound_connection_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: outbound_connection_send_enabled is null, which cannot be sent" : "Bad parameter: outbound_connection_send_enabled must be of type bool", "outbound_connection_send_enabled");
            }
            if (parameters.TryGetValue("automation_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: automation_send_enabled is null, which cannot be sent" : "Bad parameter: automation_send_enabled must be of type bool", "automation_send_enabled");
            }
            if (parameters.TryGetValue("api_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: api_request_send_enabled is null, which cannot be sent" : "Bad parameter: api_request_send_enabled must be of type bool", "api_request_send_enabled");
            }
            if (parameters.TryGetValue("public_hosting_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_hosting_request_send_enabled is null, which cannot be sent" : "Bad parameter: public_hosting_request_send_enabled must be of type bool", "public_hosting_request_send_enabled");
            }
            if (parameters.TryGetValue("email_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email_send_enabled is null, which cannot be sent" : "Bad parameter: email_send_enabled must be of type bool", "email_send_enabled");
            }
            if (parameters.TryGetValue("exavault_api_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exavault_api_request_send_enabled is null, which cannot be sent" : "Bad parameter: exavault_api_request_send_enabled must be of type bool", "exavault_api_request_send_enabled");
            }
            if (parameters.TryGetValue("settings_change_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: settings_change_send_enabled is null, which cannot be sent" : "Bad parameter: settings_change_send_enabled must be of type bool", "settings_change_send_enabled");
            }
            if (parameters.TryGetValue("destination_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_type is null, which cannot be sent" : "Bad parameter: destination_type must be of type string", "destination_type");
            }
            if (parameters.TryGetValue("destination_url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_url is null, which cannot be sent" : "Bad parameter: destination_url must be of type string", "destination_url");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/siem_http_destinations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            SiemHttpDestination updated = JsonUtil.DeserializeModelResponse<SiemHttpDestination>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<SiemHttpDestination>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /siem_http_destinations/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /siem_http_destinations/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>, but neither waits for the request nor reports its failure.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        [Obsolete("Destroy neither waits for the request nor reports its failure. Use DestroyAsync instead.")]
        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The SiemHttpDestination has no id, which Delete needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/siem_http_destinations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "name", "additional_headers", "sending_active", "generic_payload_type", "file_destination_path", "file_format", "file_interval_minutes", "splunk_token", "crowdstrike_token", "azure_dcr_immutable_id", "azure_stream_name", "azure_oauth_client_credentials_tenant_id", "azure_oauth_client_credentials_client_id", "azure_oauth_client_credentials_client_secret", "qradar_username", "qradar_password", "solar_winds_token", "new_relic_api_key", "datadog_api_key", "action_send_enabled", "sftp_action_send_enabled", "ftp_action_send_enabled", "web_dav_action_send_enabled", "sync_send_enabled", "outbound_connection_send_enabled", "automation_send_enabled", "api_request_send_enabled", "public_hosting_request_send_enabled", "email_send_enabled", "exavault_api_request_send_enabled", "settings_change_send_enabled", "destination_type", "destination_url" };
        private static readonly string[] UpdateParameterNames = { "id", "name", "additional_headers", "sending_active", "generic_payload_type", "file_destination_path", "file_format", "file_interval_minutes", "splunk_token", "crowdstrike_token", "azure_dcr_immutable_id", "azure_stream_name", "azure_oauth_client_credentials_tenant_id", "azure_oauth_client_credentials_client_id", "azure_oauth_client_credentials_client_secret", "qradar_username", "qradar_password", "solar_winds_token", "new_relic_api_key", "datadog_api_key", "action_send_enabled", "sftp_action_send_enabled", "ftp_action_send_enabled", "web_dav_action_send_enabled", "sync_send_enabled", "outbound_connection_send_enabled", "automation_send_enabled", "api_request_send_enabled", "public_hosting_request_send_enabled", "email_send_enabled", "exavault_api_request_send_enabled", "settings_change_send_enabled", "destination_type", "destination_url" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this SiemHttpDestination on Files.com, or updates it if it has an ID.
        /// It sends the attributes set on this object, in its constructor or with its properties, or changed in place,
        /// such as an element of an array, since it was made or last came from Files.com, of those that operation
        /// takes; it sends no other attributes, and no defaults. Each goes in the form its parameter takes, such as a
        /// DateTime as ISO 8601 text. When it succeeds, this object takes the attributes Files.com returns.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                await UpdateCore(AttributesToSave(UpdateParameterNames), cancellationToken);
                return;
            }
            SiemHttpDestination created = await SiemHttpDestination.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
            Refresh(created.getAttributes());
            origin = ((IModel)created).Origin;
        }

        // The attributes set or changed on this object since it was made or last came from Files.com, of those named
        // in parameterNames: the parameters of the operation that saves it.
        private Dictionary<string, object> AttributesToSave(string[] parameterNames)
        {
            Dictionary<string, object> toSave = new Dictionary<string, object>();
            foreach (string name in parameterNames)
            {
                if (unsaved.Contains(name))
                {
                    toSave[name] = attributes[name];
                }
            }
            return toSave;
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<SiemHttpDestination> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<SiemHttpDestination> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<SiemHttpDestination> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }

            return new FilesList<SiemHttpDestination>(client, $"/siem_http_destinations", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Siem Http Destination ID.
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<SiemHttpDestination> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Siem Http Destination ID.
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<SiemHttpDestination> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<SiemHttpDestination> FindCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/siem_http_destinations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<SiemHttpDestination>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this Destination
        ///   additional_headers - object - Additional HTTP Headers included in calls to the destination URL
        ///   sending_active - boolean - Whether this SIEM HTTP Destination is currently being sent to or not
        ///   generic_payload_type - string - Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        ///   file_destination_path - string - Applicable only for destination type: file. Destination folder path on Files.com.
        ///   file_format - string - Applicable only for destination type: file. Generated file format.
        ///   file_interval_minutes - int64 - Applicable only for destination type: file. Interval, in minutes, between file deliveries. Valid values are 5, 10, 15, 20, 30, 60, 90, 180, 240, 360.
        ///   splunk_token - string - Applicable only for destination types: splunk, splunk_compatible. Authentication token for the destination.
        ///   crowdstrike_token - string - Applicable only for destination type: crowdstrike. Authentication token provided by Crowdstrike.
        ///   azure_dcr_immutable_id - string - Applicable only for destination types: azure, azure_legacy. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   action_send_enabled - boolean - Whether or not sending is enabled for action logs.
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
        ///   settings_change_send_enabled - boolean - Whether or not sending is enabled for settings_change logs.
        ///   destination_type (required) - string - Destination Type
        ///   destination_url - string - Destination Url
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<SiemHttpDestination> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<SiemHttpDestination> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("destination_type", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: destination_type", "destination_type");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("additional_headers", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: additional_headers is null, which cannot be sent" : "Bad parameter: additional_headers must be of type object", "additional_headers");
            }
            if (parameters.TryGetValue("sending_active", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sending_active is null, which cannot be sent" : "Bad parameter: sending_active must be of type bool", "sending_active");
            }
            if (parameters.TryGetValue("generic_payload_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: generic_payload_type is null, which cannot be sent" : "Bad parameter: generic_payload_type must be of type string", "generic_payload_type");
            }
            if (parameters.TryGetValue("file_destination_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_destination_path is null, which cannot be sent" : "Bad parameter: file_destination_path must be of type string", "file_destination_path");
            }
            if (parameters.TryGetValue("file_format", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_format is null, which cannot be sent" : "Bad parameter: file_format must be of type string", "file_format");
            }
            if (parameters.TryGetValue("file_interval_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_interval_minutes is null, which cannot be sent" : "Bad parameter: file_interval_minutes must be of type Nullable<Int64>", "file_interval_minutes");
            }
            if (parameters.TryGetValue("splunk_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: splunk_token is null, which cannot be sent" : "Bad parameter: splunk_token must be of type string", "splunk_token");
            }
            if (parameters.TryGetValue("crowdstrike_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: crowdstrike_token is null, which cannot be sent" : "Bad parameter: crowdstrike_token must be of type string", "crowdstrike_token");
            }
            if (parameters.TryGetValue("azure_dcr_immutable_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_dcr_immutable_id is null, which cannot be sent" : "Bad parameter: azure_dcr_immutable_id must be of type string", "azure_dcr_immutable_id");
            }
            if (parameters.TryGetValue("azure_stream_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_stream_name is null, which cannot be sent" : "Bad parameter: azure_stream_name must be of type string", "azure_stream_name");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_tenant_id is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_tenant_id must be of type string", "azure_oauth_client_credentials_tenant_id");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_client_id is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_client_id must be of type string", "azure_oauth_client_credentials_client_id");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_client_secret is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_client_secret must be of type string", "azure_oauth_client_credentials_client_secret");
            }
            if (parameters.TryGetValue("qradar_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: qradar_username is null, which cannot be sent" : "Bad parameter: qradar_username must be of type string", "qradar_username");
            }
            if (parameters.TryGetValue("qradar_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: qradar_password is null, which cannot be sent" : "Bad parameter: qradar_password must be of type string", "qradar_password");
            }
            if (parameters.TryGetValue("solar_winds_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: solar_winds_token is null, which cannot be sent" : "Bad parameter: solar_winds_token must be of type string", "solar_winds_token");
            }
            if (parameters.TryGetValue("new_relic_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: new_relic_api_key is null, which cannot be sent" : "Bad parameter: new_relic_api_key must be of type string", "new_relic_api_key");
            }
            if (parameters.TryGetValue("datadog_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: datadog_api_key is null, which cannot be sent" : "Bad parameter: datadog_api_key must be of type string", "datadog_api_key");
            }
            if (parameters.TryGetValue("action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action_send_enabled is null, which cannot be sent" : "Bad parameter: action_send_enabled must be of type bool", "action_send_enabled");
            }
            if (parameters.TryGetValue("sftp_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_action_send_enabled is null, which cannot be sent" : "Bad parameter: sftp_action_send_enabled must be of type bool", "sftp_action_send_enabled");
            }
            if (parameters.TryGetValue("ftp_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ftp_action_send_enabled is null, which cannot be sent" : "Bad parameter: ftp_action_send_enabled must be of type bool", "ftp_action_send_enabled");
            }
            if (parameters.TryGetValue("web_dav_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: web_dav_action_send_enabled is null, which cannot be sent" : "Bad parameter: web_dav_action_send_enabled must be of type bool", "web_dav_action_send_enabled");
            }
            if (parameters.TryGetValue("sync_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_send_enabled is null, which cannot be sent" : "Bad parameter: sync_send_enabled must be of type bool", "sync_send_enabled");
            }
            if (parameters.TryGetValue("outbound_connection_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: outbound_connection_send_enabled is null, which cannot be sent" : "Bad parameter: outbound_connection_send_enabled must be of type bool", "outbound_connection_send_enabled");
            }
            if (parameters.TryGetValue("automation_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: automation_send_enabled is null, which cannot be sent" : "Bad parameter: automation_send_enabled must be of type bool", "automation_send_enabled");
            }
            if (parameters.TryGetValue("api_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: api_request_send_enabled is null, which cannot be sent" : "Bad parameter: api_request_send_enabled must be of type bool", "api_request_send_enabled");
            }
            if (parameters.TryGetValue("public_hosting_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_hosting_request_send_enabled is null, which cannot be sent" : "Bad parameter: public_hosting_request_send_enabled must be of type bool", "public_hosting_request_send_enabled");
            }
            if (parameters.TryGetValue("email_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email_send_enabled is null, which cannot be sent" : "Bad parameter: email_send_enabled must be of type bool", "email_send_enabled");
            }
            if (parameters.TryGetValue("exavault_api_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exavault_api_request_send_enabled is null, which cannot be sent" : "Bad parameter: exavault_api_request_send_enabled must be of type bool", "exavault_api_request_send_enabled");
            }
            if (parameters.TryGetValue("settings_change_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: settings_change_send_enabled is null, which cannot be sent" : "Bad parameter: settings_change_send_enabled must be of type bool", "settings_change_send_enabled");
            }
            if (parameters.TryGetValue("destination_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_type is null, which cannot be sent" : "Bad parameter: destination_type must be of type string", "destination_type");
            }
            if (parameters.TryGetValue("destination_url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_url is null, which cannot be sent" : "Bad parameter: destination_url must be of type string", "destination_url");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/siem_http_destinations", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<SiemHttpDestination>(responseJson, context, options);
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
        ///   file_destination_path - string - Applicable only for destination type: file. Destination folder path on Files.com.
        ///   file_format - string - Applicable only for destination type: file. Generated file format.
        ///   file_interval_minutes - int64 - Applicable only for destination type: file. Interval, in minutes, between file deliveries. Valid values are 5, 10, 15, 20, 30, 60, 90, 180, 240, 360.
        ///   splunk_token - string - Applicable only for destination types: splunk, splunk_compatible. Authentication token for the destination.
        ///   crowdstrike_token - string - Applicable only for destination type: crowdstrike. Authentication token provided by Crowdstrike.
        ///   azure_dcr_immutable_id - string - Applicable only for destination types: azure, azure_legacy. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   action_send_enabled - boolean - Whether or not sending is enabled for action logs.
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
        ///   settings_change_send_enabled - boolean - Whether or not sending is enabled for settings_change logs.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task SendTestEntry(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return SendTestEntryCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task SendTestEntryCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("siem_http_destination_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: siem_http_destination_id is null, which cannot be sent" : "Bad parameter: siem_http_destination_id must be of type Nullable<Int64>", "siem_http_destination_id");
            }
            if (parameters.TryGetValue("destination_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_type is null, which cannot be sent" : "Bad parameter: destination_type must be of type string", "destination_type");
            }
            if (parameters.TryGetValue("destination_url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_url is null, which cannot be sent" : "Bad parameter: destination_url must be of type string", "destination_url");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("additional_headers", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: additional_headers is null, which cannot be sent" : "Bad parameter: additional_headers must be of type object", "additional_headers");
            }
            if (parameters.TryGetValue("sending_active", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sending_active is null, which cannot be sent" : "Bad parameter: sending_active must be of type bool", "sending_active");
            }
            if (parameters.TryGetValue("generic_payload_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: generic_payload_type is null, which cannot be sent" : "Bad parameter: generic_payload_type must be of type string", "generic_payload_type");
            }
            if (parameters.TryGetValue("file_destination_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_destination_path is null, which cannot be sent" : "Bad parameter: file_destination_path must be of type string", "file_destination_path");
            }
            if (parameters.TryGetValue("file_format", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_format is null, which cannot be sent" : "Bad parameter: file_format must be of type string", "file_format");
            }
            if (parameters.TryGetValue("file_interval_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_interval_minutes is null, which cannot be sent" : "Bad parameter: file_interval_minutes must be of type Nullable<Int64>", "file_interval_minutes");
            }
            if (parameters.TryGetValue("splunk_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: splunk_token is null, which cannot be sent" : "Bad parameter: splunk_token must be of type string", "splunk_token");
            }
            if (parameters.TryGetValue("crowdstrike_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: crowdstrike_token is null, which cannot be sent" : "Bad parameter: crowdstrike_token must be of type string", "crowdstrike_token");
            }
            if (parameters.TryGetValue("azure_dcr_immutable_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_dcr_immutable_id is null, which cannot be sent" : "Bad parameter: azure_dcr_immutable_id must be of type string", "azure_dcr_immutable_id");
            }
            if (parameters.TryGetValue("azure_stream_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_stream_name is null, which cannot be sent" : "Bad parameter: azure_stream_name must be of type string", "azure_stream_name");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_tenant_id is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_tenant_id must be of type string", "azure_oauth_client_credentials_tenant_id");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_client_id is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_client_id must be of type string", "azure_oauth_client_credentials_client_id");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_client_secret is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_client_secret must be of type string", "azure_oauth_client_credentials_client_secret");
            }
            if (parameters.TryGetValue("qradar_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: qradar_username is null, which cannot be sent" : "Bad parameter: qradar_username must be of type string", "qradar_username");
            }
            if (parameters.TryGetValue("qradar_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: qradar_password is null, which cannot be sent" : "Bad parameter: qradar_password must be of type string", "qradar_password");
            }
            if (parameters.TryGetValue("solar_winds_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: solar_winds_token is null, which cannot be sent" : "Bad parameter: solar_winds_token must be of type string", "solar_winds_token");
            }
            if (parameters.TryGetValue("new_relic_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: new_relic_api_key is null, which cannot be sent" : "Bad parameter: new_relic_api_key must be of type string", "new_relic_api_key");
            }
            if (parameters.TryGetValue("datadog_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: datadog_api_key is null, which cannot be sent" : "Bad parameter: datadog_api_key must be of type string", "datadog_api_key");
            }
            if (parameters.TryGetValue("action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action_send_enabled is null, which cannot be sent" : "Bad parameter: action_send_enabled must be of type bool", "action_send_enabled");
            }
            if (parameters.TryGetValue("sftp_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_action_send_enabled is null, which cannot be sent" : "Bad parameter: sftp_action_send_enabled must be of type bool", "sftp_action_send_enabled");
            }
            if (parameters.TryGetValue("ftp_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ftp_action_send_enabled is null, which cannot be sent" : "Bad parameter: ftp_action_send_enabled must be of type bool", "ftp_action_send_enabled");
            }
            if (parameters.TryGetValue("web_dav_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: web_dav_action_send_enabled is null, which cannot be sent" : "Bad parameter: web_dav_action_send_enabled must be of type bool", "web_dav_action_send_enabled");
            }
            if (parameters.TryGetValue("sync_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_send_enabled is null, which cannot be sent" : "Bad parameter: sync_send_enabled must be of type bool", "sync_send_enabled");
            }
            if (parameters.TryGetValue("outbound_connection_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: outbound_connection_send_enabled is null, which cannot be sent" : "Bad parameter: outbound_connection_send_enabled must be of type bool", "outbound_connection_send_enabled");
            }
            if (parameters.TryGetValue("automation_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: automation_send_enabled is null, which cannot be sent" : "Bad parameter: automation_send_enabled must be of type bool", "automation_send_enabled");
            }
            if (parameters.TryGetValue("api_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: api_request_send_enabled is null, which cannot be sent" : "Bad parameter: api_request_send_enabled must be of type bool", "api_request_send_enabled");
            }
            if (parameters.TryGetValue("public_hosting_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_hosting_request_send_enabled is null, which cannot be sent" : "Bad parameter: public_hosting_request_send_enabled must be of type bool", "public_hosting_request_send_enabled");
            }
            if (parameters.TryGetValue("email_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email_send_enabled is null, which cannot be sent" : "Bad parameter: email_send_enabled must be of type bool", "email_send_enabled");
            }
            if (parameters.TryGetValue("exavault_api_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exavault_api_request_send_enabled is null, which cannot be sent" : "Bad parameter: exavault_api_request_send_enabled must be of type bool", "exavault_api_request_send_enabled");
            }
            if (parameters.TryGetValue("settings_change_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: settings_change_send_enabled is null, which cannot be sent" : "Bad parameter: settings_change_send_enabled must be of type bool", "settings_change_send_enabled");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/siem_http_destinations/send_test_entry", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this Destination
        ///   additional_headers - object - Additional HTTP Headers included in calls to the destination URL
        ///   sending_active - boolean - Whether this SIEM HTTP Destination is currently being sent to or not
        ///   generic_payload_type - string - Applicable only for destination type: generic. Indicates the type of HTTP body. Can be json_newline or json_array. json_newline is multiple log entries as JSON separated by newlines. json_array is a single JSON array containing multiple log entries as JSON.
        ///   file_destination_path - string - Applicable only for destination type: file. Destination folder path on Files.com.
        ///   file_format - string - Applicable only for destination type: file. Generated file format.
        ///   file_interval_minutes - int64 - Applicable only for destination type: file. Interval, in minutes, between file deliveries. Valid values are 5, 10, 15, 20, 30, 60, 90, 180, 240, 360.
        ///   splunk_token - string - Applicable only for destination types: splunk, splunk_compatible. Authentication token for the destination.
        ///   crowdstrike_token - string - Applicable only for destination type: crowdstrike. Authentication token provided by Crowdstrike.
        ///   azure_dcr_immutable_id - string - Applicable only for destination types: azure, azure_legacy. Immutable ID of the Data Collection Rule.
        ///   azure_stream_name - string - Applicable only for destination type: azure. Name of the stream in the DCR that represents the destination table.
        ///   azure_oauth_client_credentials_tenant_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Tenant ID.
        ///   azure_oauth_client_credentials_client_id - string - Applicable only for destination types: azure, azure_legacy. Client Credentials OAuth Client ID.
        ///   azure_oauth_client_credentials_client_secret - string - Applicable only for destination type: azure. Client Credentials OAuth Client Secret.
        ///   qradar_username - string - Applicable only for destination type: qradar. Basic auth username provided by QRadar.
        ///   qradar_password - string - Applicable only for destination type: qradar. Basic auth password provided by QRadar.
        ///   solar_winds_token - string - Applicable only for destination type: solar_winds. Authentication token provided by Solar Winds.
        ///   new_relic_api_key - string - Applicable only for destination type: new_relic. API key provided by New Relic.
        ///   datadog_api_key - string - Applicable only for destination type: datadog. API key provided by Datadog.
        ///   action_send_enabled - boolean - Whether or not sending is enabled for action logs.
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
        ///   settings_change_send_enabled - boolean - Whether or not sending is enabled for settings_change logs.
        ///   destination_type - string - Destination Type
        ///   destination_url - string - Destination Url
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<SiemHttpDestination> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<SiemHttpDestination> UpdateCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("additional_headers", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: additional_headers is null, which cannot be sent" : "Bad parameter: additional_headers must be of type object", "additional_headers");
            }
            if (parameters.TryGetValue("sending_active", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sending_active is null, which cannot be sent" : "Bad parameter: sending_active must be of type bool", "sending_active");
            }
            if (parameters.TryGetValue("generic_payload_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: generic_payload_type is null, which cannot be sent" : "Bad parameter: generic_payload_type must be of type string", "generic_payload_type");
            }
            if (parameters.TryGetValue("file_destination_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_destination_path is null, which cannot be sent" : "Bad parameter: file_destination_path must be of type string", "file_destination_path");
            }
            if (parameters.TryGetValue("file_format", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_format is null, which cannot be sent" : "Bad parameter: file_format must be of type string", "file_format");
            }
            if (parameters.TryGetValue("file_interval_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_interval_minutes is null, which cannot be sent" : "Bad parameter: file_interval_minutes must be of type Nullable<Int64>", "file_interval_minutes");
            }
            if (parameters.TryGetValue("splunk_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: splunk_token is null, which cannot be sent" : "Bad parameter: splunk_token must be of type string", "splunk_token");
            }
            if (parameters.TryGetValue("crowdstrike_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: crowdstrike_token is null, which cannot be sent" : "Bad parameter: crowdstrike_token must be of type string", "crowdstrike_token");
            }
            if (parameters.TryGetValue("azure_dcr_immutable_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_dcr_immutable_id is null, which cannot be sent" : "Bad parameter: azure_dcr_immutable_id must be of type string", "azure_dcr_immutable_id");
            }
            if (parameters.TryGetValue("azure_stream_name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_stream_name is null, which cannot be sent" : "Bad parameter: azure_stream_name must be of type string", "azure_stream_name");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_tenant_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_tenant_id is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_tenant_id must be of type string", "azure_oauth_client_credentials_tenant_id");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_client_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_client_id is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_client_id must be of type string", "azure_oauth_client_credentials_client_id");
            }
            if (parameters.TryGetValue("azure_oauth_client_credentials_client_secret", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: azure_oauth_client_credentials_client_secret is null, which cannot be sent" : "Bad parameter: azure_oauth_client_credentials_client_secret must be of type string", "azure_oauth_client_credentials_client_secret");
            }
            if (parameters.TryGetValue("qradar_username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: qradar_username is null, which cannot be sent" : "Bad parameter: qradar_username must be of type string", "qradar_username");
            }
            if (parameters.TryGetValue("qradar_password", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: qradar_password is null, which cannot be sent" : "Bad parameter: qradar_password must be of type string", "qradar_password");
            }
            if (parameters.TryGetValue("solar_winds_token", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: solar_winds_token is null, which cannot be sent" : "Bad parameter: solar_winds_token must be of type string", "solar_winds_token");
            }
            if (parameters.TryGetValue("new_relic_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: new_relic_api_key is null, which cannot be sent" : "Bad parameter: new_relic_api_key must be of type string", "new_relic_api_key");
            }
            if (parameters.TryGetValue("datadog_api_key", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: datadog_api_key is null, which cannot be sent" : "Bad parameter: datadog_api_key must be of type string", "datadog_api_key");
            }
            if (parameters.TryGetValue("action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: action_send_enabled is null, which cannot be sent" : "Bad parameter: action_send_enabled must be of type bool", "action_send_enabled");
            }
            if (parameters.TryGetValue("sftp_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sftp_action_send_enabled is null, which cannot be sent" : "Bad parameter: sftp_action_send_enabled must be of type bool", "sftp_action_send_enabled");
            }
            if (parameters.TryGetValue("ftp_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ftp_action_send_enabled is null, which cannot be sent" : "Bad parameter: ftp_action_send_enabled must be of type bool", "ftp_action_send_enabled");
            }
            if (parameters.TryGetValue("web_dav_action_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: web_dav_action_send_enabled is null, which cannot be sent" : "Bad parameter: web_dav_action_send_enabled must be of type bool", "web_dav_action_send_enabled");
            }
            if (parameters.TryGetValue("sync_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_send_enabled is null, which cannot be sent" : "Bad parameter: sync_send_enabled must be of type bool", "sync_send_enabled");
            }
            if (parameters.TryGetValue("outbound_connection_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: outbound_connection_send_enabled is null, which cannot be sent" : "Bad parameter: outbound_connection_send_enabled must be of type bool", "outbound_connection_send_enabled");
            }
            if (parameters.TryGetValue("automation_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: automation_send_enabled is null, which cannot be sent" : "Bad parameter: automation_send_enabled must be of type bool", "automation_send_enabled");
            }
            if (parameters.TryGetValue("api_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: api_request_send_enabled is null, which cannot be sent" : "Bad parameter: api_request_send_enabled must be of type bool", "api_request_send_enabled");
            }
            if (parameters.TryGetValue("public_hosting_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: public_hosting_request_send_enabled is null, which cannot be sent" : "Bad parameter: public_hosting_request_send_enabled must be of type bool", "public_hosting_request_send_enabled");
            }
            if (parameters.TryGetValue("email_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: email_send_enabled is null, which cannot be sent" : "Bad parameter: email_send_enabled must be of type bool", "email_send_enabled");
            }
            if (parameters.TryGetValue("exavault_api_request_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exavault_api_request_send_enabled is null, which cannot be sent" : "Bad parameter: exavault_api_request_send_enabled must be of type bool", "exavault_api_request_send_enabled");
            }
            if (parameters.TryGetValue("settings_change_send_enabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: settings_change_send_enabled is null, which cannot be sent" : "Bad parameter: settings_change_send_enabled must be of type bool", "settings_change_send_enabled");
            }
            if (parameters.TryGetValue("destination_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_type is null, which cannot be sent" : "Bad parameter: destination_type must be of type string", "destination_type");
            }
            if (parameters.TryGetValue("destination_url", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_url is null, which cannot be sent" : "Bad parameter: destination_url must be of type string", "destination_url");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/siem_http_destinations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<SiemHttpDestination>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /siem_http_destinations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /siem_http_destinations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(id, parameters, options);
        }

        internal static async Task DeleteCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/siem_http_destinations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}