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
    /// As2IncomingMessageEntity model
    /// </summary>
    public class As2IncomingMessage : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        /// <summary>
        /// Makes a As2IncomingMessage with no attributes set and no options.
        /// </summary>
        public As2IncomingMessage() : this(null, null) { }

        /// <summary>
        /// Makes a As2IncomingMessage with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public As2IncomingMessage(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("replayed_from_message_id"))
            {
                this.attributes.Add("replayed_from_message_id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("as2_partner_id"))
            {
                this.attributes.Add("as2_partner_id", null);
            }
            if (!this.attributes.ContainsKey("as2_station_id"))
            {
                this.attributes.Add("as2_station_id", null);
            }
            if (!this.attributes.ContainsKey("uuid"))
            {
                this.attributes.Add("uuid", null);
            }
            if (!this.attributes.ContainsKey("content_type"))
            {
                this.attributes.Add("content_type", null);
            }
            if (!this.attributes.ContainsKey("http_headers"))
            {
                this.attributes.Add("http_headers", null);
            }
            if (!this.attributes.ContainsKey("processing_result"))
            {
                this.attributes.Add("processing_result", null);
            }
            if (!this.attributes.ContainsKey("processing_result_description"))
            {
                this.attributes.Add("processing_result_description", null);
            }
            if (!this.attributes.ContainsKey("mic"))
            {
                this.attributes.Add("mic", null);
            }
            if (!this.attributes.ContainsKey("mic_algo"))
            {
                this.attributes.Add("mic_algo", null);
            }
            if (!this.attributes.ContainsKey("as2_to"))
            {
                this.attributes.Add("as2_to", null);
            }
            if (!this.attributes.ContainsKey("as2_from"))
            {
                this.attributes.Add("as2_from", null);
            }
            if (!this.attributes.ContainsKey("message_id"))
            {
                this.attributes.Add("message_id", null);
            }
            if (!this.attributes.ContainsKey("subject"))
            {
                this.attributes.Add("subject", null);
            }
            if (!this.attributes.ContainsKey("date"))
            {
                this.attributes.Add("date", null);
            }
            if (!this.attributes.ContainsKey("body_size"))
            {
                this.attributes.Add("body_size", null);
            }
            if (!this.attributes.ContainsKey("attachment_filename"))
            {
                this.attributes.Add("attachment_filename", null);
            }
            if (!this.attributes.ContainsKey("ip"))
            {
                this.attributes.Add("ip", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("http_response_code"))
            {
                this.attributes.Add("http_response_code", null);
            }
            if (!this.attributes.ContainsKey("http_response_headers"))
            {
                this.attributes.Add("http_response_headers", null);
            }
            if (!this.attributes.ContainsKey("recipient_serial"))
            {
                this.attributes.Add("recipient_serial", null);
            }
            if (!this.attributes.ContainsKey("hex_recipient_serial"))
            {
                this.attributes.Add("hex_recipient_serial", null);
            }
            if (!this.attributes.ContainsKey("recipient_issuer"))
            {
                this.attributes.Add("recipient_issuer", null);
            }
            if (!this.attributes.ContainsKey("message_received"))
            {
                this.attributes.Add("message_received", false);
            }
            if (!this.attributes.ContainsKey("message_decrypted"))
            {
                this.attributes.Add("message_decrypted", false);
            }
            if (!this.attributes.ContainsKey("message_signature_verified"))
            {
                this.attributes.Add("message_signature_verified", false);
            }
            if (!this.attributes.ContainsKey("message_processing_success"))
            {
                this.attributes.Add("message_processing_success", false);
            }
            if (!this.attributes.ContainsKey("message_mdn_returned"))
            {
                this.attributes.Add("message_mdn_returned", false);
            }
            if (!this.attributes.ContainsKey("encrypted_uri"))
            {
                this.attributes.Add("encrypted_uri", null);
            }
            if (!this.attributes.ContainsKey("smime_signed_uri"))
            {
                this.attributes.Add("smime_signed_uri", null);
            }
            if (!this.attributes.ContainsKey("smime_uri"))
            {
                this.attributes.Add("smime_uri", null);
            }
            if (!this.attributes.ContainsKey("raw_uri"))
            {
                this.attributes.Add("raw_uri", null);
            }
            if (!this.attributes.ContainsKey("mdn_response_uri"))
            {
                this.attributes.Add("mdn_response_uri", null);
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


        /// <summary>
        /// ID of this AS2 incoming message record.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// ID of the incoming message record this message was replayed from. Empty for messages received directly from a trading partner.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("replayed_from_message_id")]
        public Nullable<Int64> ReplayedFromMessageId
        {
            get { return (Nullable<Int64>)attributes["replayed_from_message_id"]; }
            private set { attributes["replayed_from_message_id"] = value; }
        }

        /// <summary>
        /// ID of the Workspace associated with this AS2 Incoming Message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            private set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// Id of the AS2 Partner associated with this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_partner_id")]
        public Nullable<Int64> As2PartnerId
        {
            get { return (Nullable<Int64>)attributes["as2_partner_id"]; }
            private set { attributes["as2_partner_id"] = value; }
        }

        /// <summary>
        /// Id of the AS2 Station associated with this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_station_id")]
        public Nullable<Int64> As2StationId
        {
            get { return (Nullable<Int64>)attributes["as2_station_id"]; }
            private set { attributes["as2_station_id"] = value; }
        }

        /// <summary>
        /// UUID assigned to this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("uuid")]
        public string Uuid
        {
            get { return (string)attributes["uuid"]; }
            private set { attributes["uuid"] = value; }
        }

        /// <summary>
        /// Content Type header of the incoming message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content_type")]
        public string ContentType
        {
            get { return (string)attributes["content_type"]; }
            private set { attributes["content_type"] = value; }
        }

        /// <summary>
        /// HTTP Headers sent with this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_headers")]
        public object HttpHeaders
        {
            get { return (object)attributes["http_headers"]; }
            private set { attributes["http_headers"] = value; }
        }

        /// <summary>
        /// Result of processing.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("processing_result")]
        public string ProcessingResult
        {
            get { return (string)attributes["processing_result"]; }
            private set { attributes["processing_result"] = value; }
        }

        /// <summary>
        /// Result of processing description.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("processing_result_description")]
        public string ProcessingResultDescription
        {
            get { return (string)attributes["processing_result_description"]; }
            private set { attributes["processing_result_description"] = value; }
        }

        /// <summary>
        /// AS2 Message Integrity Check
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("mic")]
        public string Mic
        {
            get { return (string)attributes["mic"]; }
            private set { attributes["mic"] = value; }
        }

        /// <summary>
        /// AS2 Message Integrity Check Algorithm Used
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("mic_algo")]
        public string MicAlgo
        {
            get { return (string)attributes["mic_algo"]; }
            private set { attributes["mic_algo"] = value; }
        }

        /// <summary>
        /// AS2 TO header of message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_to")]
        public string As2To
        {
            get { return (string)attributes["as2_to"]; }
            private set { attributes["as2_to"] = value; }
        }

        /// <summary>
        /// AS2 FROM header of message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_from")]
        public string As2From
        {
            get { return (string)attributes["as2_from"]; }
            private set { attributes["as2_from"] = value; }
        }

        /// <summary>
        /// AS2 Message Id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message_id")]
        public string MessageId
        {
            get { return (string)attributes["message_id"]; }
            private set { attributes["message_id"] = value; }
        }

        /// <summary>
        /// AS2 Subject Header
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("subject")]
        public string Subject
        {
            get { return (string)attributes["subject"]; }
            private set { attributes["subject"] = value; }
        }

        /// <summary>
        /// Date Header
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("date")]
        public string Date
        {
            get { return (string)attributes["date"]; }
            private set { attributes["date"] = value; }
        }

        /// <summary>
        /// Encrypted Payload Body Size
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("body_size")]
        public string BodySize
        {
            get { return (string)attributes["body_size"]; }
            private set { attributes["body_size"] = value; }
        }

        /// <summary>
        /// Filename of the file being received.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("attachment_filename")]
        public string AttachmentFilename
        {
            get { return (string)attributes["attachment_filename"]; }
            private set { attributes["attachment_filename"] = value; }
        }

        /// <summary>
        /// IP Address of the Sender
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string)attributes["ip"]; }
            private set { attributes["ip"] = value; }
        }

        /// <summary>
        /// Message creation date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// HTTP Response Code sent for this message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_response_code")]
        public string HttpResponseCode
        {
            get { return (string)attributes["http_response_code"]; }
            private set { attributes["http_response_code"] = value; }
        }

        /// <summary>
        /// HTTP Headers sent for this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_response_headers")]
        public object HttpResponseHeaders
        {
            get { return (object)attributes["http_response_headers"]; }
            private set { attributes["http_response_headers"] = value; }
        }

        /// <summary>
        /// Incoming Message Recipient(the Client Cert used to encrypt this message)'s serial
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("recipient_serial")]
        public string RecipientSerial
        {
            get { return (string)attributes["recipient_serial"]; }
            private set { attributes["recipient_serial"] = value; }
        }

        /// <summary>
        /// Incoming Message Recipient(the Client Cert used to encrypt this message)'s serial in hex format.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("hex_recipient_serial")]
        public string HexRecipientSerial
        {
            get { return (string)attributes["hex_recipient_serial"]; }
            private set { attributes["hex_recipient_serial"] = value; }
        }

        /// <summary>
        /// Incoming Message Recipient(the Client Cert used to encrypt this message)'s issuer
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("recipient_issuer")]
        public string RecipientIssuer
        {
            get { return (string)attributes["recipient_issuer"]; }
            private set { attributes["recipient_issuer"] = value; }
        }

        /// <summary>
        /// Message body received?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("message_received")]
        public bool MessageReceived
        {
            get { return attributes["message_received"] == null ? false : (bool)attributes["message_received"]; }
            private set { attributes["message_received"] = value; }
        }

        /// <summary>
        /// Message decrypted successfully?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("message_decrypted")]
        public bool MessageDecrypted
        {
            get { return attributes["message_decrypted"] == null ? false : (bool)attributes["message_decrypted"]; }
            private set { attributes["message_decrypted"] = value; }
        }

        /// <summary>
        /// Message signature verified?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("message_signature_verified")]
        public bool MessageSignatureVerified
        {
            get { return attributes["message_signature_verified"] == null ? false : (bool)attributes["message_signature_verified"]; }
            private set { attributes["message_signature_verified"] = value; }
        }

        /// <summary>
        /// Message processed successfully?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("message_processing_success")]
        public bool MessageProcessingSuccess
        {
            get { return attributes["message_processing_success"] == null ? false : (bool)attributes["message_processing_success"]; }
            private set { attributes["message_processing_success"] = value; }
        }

        /// <summary>
        /// MDN returned?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("message_mdn_returned")]
        public bool MessageMdnReturned
        {
            get { return attributes["message_mdn_returned"] == null ? false : (bool)attributes["message_mdn_returned"]; }
            private set { attributes["message_mdn_returned"] = value; }
        }

        /// <summary>
        /// URL to download the encrypted signed smime that is to sent as AS2 body
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("encrypted_uri")]
        public string EncryptedUri
        {
            get { return (string)attributes["encrypted_uri"]; }
            private set { attributes["encrypted_uri"] = value; }
        }

        /// <summary>
        /// URL to download the file contents as smime with signature
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smime_signed_uri")]
        public string SmimeSignedUri
        {
            get { return (string)attributes["smime_signed_uri"]; }
            private set { attributes["smime_signed_uri"] = value; }
        }

        /// <summary>
        /// URL to download the file contents encoded as smime
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("smime_uri")]
        public string SmimeUri
        {
            get { return (string)attributes["smime_uri"]; }
            private set { attributes["smime_uri"] = value; }
        }

        /// <summary>
        /// URL to download the original file contents
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("raw_uri")]
        public string RawUri
        {
            get { return (string)attributes["raw_uri"]; }
            private set { attributes["raw_uri"] = value; }
        }

        /// <summary>
        /// URL to download the http response body
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("mdn_response_uri")]
        public string MdnResponseUri
        {
            get { return (string)attributes["mdn_response_uri"]; }
            private set { attributes["mdn_response_uri"] = value; }
        }

        /// <summary>
        /// Replay AS2 Incoming Message
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task<As2IncomingMessage> Replay(Dictionary<string, object> parameters)
        {
            return ReplayCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Replay AS2 Incoming Message
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<As2IncomingMessage> ReplayAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ReplayCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<As2IncomingMessage> ReplayCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The As2IncomingMessage has no id, which Replay needs", "id");
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
            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_incoming_messages/{System.Uri.EscapeDataString(attributes["id"].ToString())}/replay", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            As2IncomingMessage result = JsonUtil.DeserializeModelResponse<As2IncomingMessage>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `as2_partner_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `as2_station_id`, `workspace_id`, `replayed_from_message_id` or `as2_partner_id`. Valid field combinations are `[ as2_station_id, created_at ]`, `[ workspace_id, created_at ]`, `[ replayed_from_message_id, created_at ]`, `[ as2_partner_id, created_at ]`, `[ workspace_id, as2_station_id ]`, `[ workspace_id, replayed_from_message_id ]`, `[ workspace_id, as2_partner_id ]`, `[ workspace_id, as2_station_id, created_at ]`, `[ workspace_id, replayed_from_message_id, created_at ]` or `[ workspace_id, as2_partner_id, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<As2IncomingMessage> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `as2_partner_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `as2_station_id`, `workspace_id`, `replayed_from_message_id` or `as2_partner_id`. Valid field combinations are `[ as2_station_id, created_at ]`, `[ workspace_id, created_at ]`, `[ replayed_from_message_id, created_at ]`, `[ as2_partner_id, created_at ]`, `[ workspace_id, as2_station_id ]`, `[ workspace_id, replayed_from_message_id ]`, `[ workspace_id, as2_partner_id ]`, `[ workspace_id, as2_station_id, created_at ]`, `[ workspace_id, replayed_from_message_id, created_at ]` or `[ workspace_id, as2_partner_id, created_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<As2IncomingMessage> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<As2IncomingMessage> ListCore(
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
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }
            if (parameters.TryGetValue("filter_gt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gt is null, which cannot be sent" : "Bad parameter: filter_gt must be of type object", "filter_gt");
            }
            if (parameters.TryGetValue("filter_gteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gteq is null, which cannot be sent" : "Bad parameter: filter_gteq must be of type object", "filter_gteq");
            }
            if (parameters.TryGetValue("filter_lt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lt is null, which cannot be sent" : "Bad parameter: filter_lt must be of type object", "filter_lt");
            }
            if (parameters.TryGetValue("filter_lteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lteq is null, which cannot be sent" : "Bad parameter: filter_lteq must be of type object", "filter_lteq");
            }

            return new FilesList<As2IncomingMessage>(client, $"/as2_incoming_messages", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - As2 Incoming Message ID.
        /// </summary>
        /// <param name="id">As2 Incoming Message ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2IncomingMessage> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - As2 Incoming Message ID.
        /// </summary>
        /// <param name="id">As2 Incoming Message ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2IncomingMessage> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<As2IncomingMessage> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_incoming_messages/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<As2IncomingMessage>(responseJson, context, options);
        }

        /// <summary>
        /// Replay AS2 Incoming Message
        /// </summary>
        /// <param name="id">As2 Incoming Message ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<As2IncomingMessage> Replay(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ReplayCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<As2IncomingMessage> ReplayCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/as2_incoming_messages/{System.Uri.EscapeDataString(parameters["id"].ToString())}/replay", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<As2IncomingMessage>(responseJson, context, options);
        }

    }
}