using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Operations
{
    // Imported inside the namespace so that model names, such as Lock, take precedence over those of the namespaces
    // above, such as System.Threading.Lock. A type of this namespace still takes precedence over a model of the same
    // name (a model UserOperations beside the operations class of User), so code here names models in their own
    // namespace (model_type, qualified: true).
    using FilesCom.Models;

    /// <summary>
    /// SiemHttpDestination operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.SiemHttpDestinations"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="global::FilesCom.Models.SiemHttpDestination"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class SiemHttpDestinationOperations
    {
        private readonly FilesClient client;

        internal SiemHttpDestinationOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a SiemHttpDestination that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public global::FilesCom.Models.SiemHttpDestination New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            global::FilesCom.Models.SiemHttpDestination model = new global::FilesCom.Models.SiemHttpDestination(attributes, null);
            ((IModel)model).SetContext(client, options, null);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList<global::FilesCom.Models.SiemHttpDestination> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FilesList<global::FilesCom.Models.SiemHttpDestination> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.ListCore(client, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Siem Http Destination ID.
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.SiemHttpDestination> FindAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Siem Http Destination ID.
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.SiemHttpDestination> GetAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.FindCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.SiemHttpDestination> CreateAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.CreateCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
        /// <param name="cancellationToken">Stops the request.</param>
        public Task SendTestEntryAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.SendTestEntryCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
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
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<global::FilesCom.Models.SiemHttpDestination> UpdateAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.UpdateCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Sends DELETE /siem_http_destinations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Sends DELETE /siem_http_destinations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Siem Http Destination ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return global::FilesCom.Models.SiemHttpDestination.DeleteCore(new OperationContext(client), id, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}