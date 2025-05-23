using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class As2OutgoingMessage
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public As2OutgoingMessage() : this(null, null) { }

        public As2OutgoingMessage(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("mic_sha_256"))
            {
                this.attributes.Add("mic_sha_256", null);
            }
            if (!this.attributes.ContainsKey("as2_to"))
            {
                this.attributes.Add("as2_to", null);
            }
            if (!this.attributes.ContainsKey("as2_from"))
            {
                this.attributes.Add("as2_from", null);
            }
            if (!this.attributes.ContainsKey("date"))
            {
                this.attributes.Add("date", null);
            }
            if (!this.attributes.ContainsKey("message_id"))
            {
                this.attributes.Add("message_id", null);
            }
            if (!this.attributes.ContainsKey("body_size"))
            {
                this.attributes.Add("body_size", null);
            }
            if (!this.attributes.ContainsKey("attachment_filename"))
            {
                this.attributes.Add("attachment_filename", null);
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
            if (!this.attributes.ContainsKey("http_transmission_duration"))
            {
                this.attributes.Add("http_transmission_duration", null);
            }
            if (!this.attributes.ContainsKey("mdn_received"))
            {
                this.attributes.Add("mdn_received", false);
            }
            if (!this.attributes.ContainsKey("mdn_valid"))
            {
                this.attributes.Add("mdn_valid", false);
            }
            if (!this.attributes.ContainsKey("mdn_signature_verified"))
            {
                this.attributes.Add("mdn_signature_verified", false);
            }
            if (!this.attributes.ContainsKey("mdn_message_id_matched"))
            {
                this.attributes.Add("mdn_message_id_matched", false);
            }
            if (!this.attributes.ContainsKey("mdn_mic_matched"))
            {
                this.attributes.Add("mdn_mic_matched", false);
            }
            if (!this.attributes.ContainsKey("mdn_processing_success"))
            {
                this.attributes.Add("mdn_processing_success", false);
            }
            if (!this.attributes.ContainsKey("raw_uri"))
            {
                this.attributes.Add("raw_uri", null);
            }
            if (!this.attributes.ContainsKey("smime_uri"))
            {
                this.attributes.Add("smime_uri", null);
            }
            if (!this.attributes.ContainsKey("smime_signed_uri"))
            {
                this.attributes.Add("smime_signed_uri", null);
            }
            if (!this.attributes.ContainsKey("encrypted_uri"))
            {
                this.attributes.Add("encrypted_uri", null);
            }
            if (!this.attributes.ContainsKey("mdn_response_uri"))
            {
                this.attributes.Add("mdn_response_uri", null);
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
        /// Id of the AS2 Partner.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
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
        /// AS2 Message Integrity Check SHA1
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("mic")]
        public string Mic
        {
            get { return (string)attributes["mic"]; }
            private set { attributes["mic"] = value; }
        }

        /// <summary>
        /// AS2 Message Integrity Check SHA256
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("mic_sha_256")]
        public string MicSha256
        {
            get { return (string)attributes["mic_sha_256"]; }
            private set { attributes["mic_sha_256"] = value; }
        }

        /// <summary>
        /// AS2 TO
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_to")]
        public string As2To
        {
            get { return (string)attributes["as2_to"]; }
            private set { attributes["as2_to"] = value; }
        }

        /// <summary>
        /// AS2 FROM
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_from")]
        public string As2From
        {
            get { return (string)attributes["as2_from"]; }
            private set { attributes["as2_from"] = value; }
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
        /// Filename of the file being sent.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("attachment_filename")]
        public string AttachmentFilename
        {
            get { return (string)attributes["attachment_filename"]; }
            private set { attributes["attachment_filename"] = value; }
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
        /// HTTP Response Code received for this message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_response_code")]
        public string HttpResponseCode
        {
            get { return (string)attributes["http_response_code"]; }
            private set { attributes["http_response_code"] = value; }
        }

        /// <summary>
        /// HTTP Headers received for this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_response_headers")]
        public object HttpResponseHeaders
        {
            get { return (object)attributes["http_response_headers"]; }
            private set { attributes["http_response_headers"] = value; }
        }

        /// <summary>
        /// HTTP transmission duration in seceonds
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_transmission_duration")]
        public double HttpTransmissionDuration
        {
            get { return (double)attributes["http_transmission_duration"]; }
            private set { attributes["http_transmission_duration"] = value; }
        }

        /// <summary>
        /// Did the partner give a response body?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mdn_received")]
        public bool MdnReceived
        {
            get { return attributes["mdn_received"] == null ? false : (bool)attributes["mdn_received"]; }
            private set { attributes["mdn_received"] = value; }
        }

        /// <summary>
        /// Is the response in MDN format?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mdn_valid")]
        public bool MdnValid
        {
            get { return attributes["mdn_valid"] == null ? false : (bool)attributes["mdn_valid"]; }
            private set { attributes["mdn_valid"] = value; }
        }

        /// <summary>
        /// MDN signature verified?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mdn_signature_verified")]
        public bool MdnSignatureVerified
        {
            get { return attributes["mdn_signature_verified"] == null ? false : (bool)attributes["mdn_signature_verified"]; }
            private set { attributes["mdn_signature_verified"] = value; }
        }

        /// <summary>
        /// MDN message id matched?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mdn_message_id_matched")]
        public bool MdnMessageIdMatched
        {
            get { return attributes["mdn_message_id_matched"] == null ? false : (bool)attributes["mdn_message_id_matched"]; }
            private set { attributes["mdn_message_id_matched"] = value; }
        }

        /// <summary>
        /// MDN MIC matched?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mdn_mic_matched")]
        public bool MdnMicMatched
        {
            get { return attributes["mdn_mic_matched"] == null ? false : (bool)attributes["mdn_mic_matched"]; }
            private set { attributes["mdn_mic_matched"] = value; }
        }

        /// <summary>
        /// MDN disposition indicate a successful processing?
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("mdn_processing_success")]
        public bool MdnProcessingSuccess
        {
            get { return attributes["mdn_processing_success"] == null ? false : (bool)attributes["mdn_processing_success"]; }
            private set { attributes["mdn_processing_success"] = value; }
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
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at` and `as2_partner_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
        ///   as2_partner_id - int64 - As2 Partner ID.  If provided, will return message specific to that partner.
        /// </summary>
        public static FilesList<As2OutgoingMessage> List(

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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }
            if (parameters.ContainsKey("as2_partner_id") && !(parameters["as2_partner_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: as2_partner_id must be of type Nullable<Int64>", "parameters[\"as2_partner_id\"]");
            }

            return new FilesList<As2OutgoingMessage>($"/as2_outgoing_messages", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<As2OutgoingMessage> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}