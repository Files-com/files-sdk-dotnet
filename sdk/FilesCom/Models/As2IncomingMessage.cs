using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class As2IncomingMessage
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public As2IncomingMessage() : this(null, null) { }

        public As2IncomingMessage(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("activity_log"))
            {
                this.attributes.Add("activity_log", null);
            }
            if (!this.attributes.ContainsKey("processing_result"))
            {
                this.attributes.Add("processing_result", null);
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
            get { return (Nullable<Int64>) attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Id of the AS2 Partner associated with this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_partner_id")]
        public Nullable<Int64> As2PartnerId
        {
            get { return (Nullable<Int64>) attributes["as2_partner_id"]; }
            private set { attributes["as2_partner_id"] = value; }
        }

        /// <summary>
        /// UUID assigned to this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("uuid")]
        public string Uuid
        {
            get { return (string) attributes["uuid"]; }
            private set { attributes["uuid"] = value; }
        }

        /// <summary>
        /// Content Type header of the incoming message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content_type")]
        public string ContentType
        {
            get { return (string) attributes["content_type"]; }
            private set { attributes["content_type"] = value; }
        }

        /// <summary>
        /// HTTP Headers sent with this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_headers")]
        public object HttpHeaders
        {
            get { return (object) attributes["http_headers"]; }
            private set { attributes["http_headers"] = value; }
        }

        /// <summary>
        /// JSON Structure of the activity log.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("activity_log")]
        public string ActivityLog
        {
            get { return (string) attributes["activity_log"]; }
            private set { attributes["activity_log"] = value; }
        }

        /// <summary>
        /// Result of processing.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("processing_result")]
        public string ProcessingResult
        {
            get { return (string) attributes["processing_result"]; }
            private set { attributes["processing_result"] = value; }
        }

        /// <summary>
        /// AS2 TO header of message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_to")]
        public string As2To
        {
            get { return (string) attributes["as2_to"]; }
            private set { attributes["as2_to"] = value; }
        }

        /// <summary>
        /// AS2 FROM header of message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("as2_from")]
        public string As2From
        {
            get { return (string) attributes["as2_from"]; }
            private set { attributes["as2_from"] = value; }
        }

        /// <summary>
        /// AS2 Message Id
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message_id")]
        public string MessageId
        {
            get { return (string) attributes["message_id"]; }
            private set { attributes["message_id"] = value; }
        }

        /// <summary>
        /// AS2 Subject Header
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("subject")]
        public string Subject
        {
            get { return (string) attributes["subject"]; }
            private set { attributes["subject"] = value; }
        }

        /// <summary>
        /// Encrypted Payload Body Size
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("body_size")]
        public string BodySize
        {
            get { return (string) attributes["body_size"]; }
            private set { attributes["body_size"] = value; }
        }

        /// <summary>
        /// Filename of the file being received.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("attachment_filename")]
        public string AttachmentFilename
        {
            get { return (string) attributes["attachment_filename"]; }
            private set { attributes["attachment_filename"] = value; }
        }

        /// <summary>
        /// Message creation date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   as2_partner_id - int64 - As2 Partner ID.  If provided, will return message specific to that partner.
        /// </summary>
        public static async Task<As2IncomingMessage[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("as2_partner_id") && !(parameters["as2_partner_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: as2_partner_id must be of type Nullable<Int64>", "parameters[\"as2_partner_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/as2_incoming_messages", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<As2IncomingMessage[]>(responseJson);
        }

        public static async Task<As2IncomingMessage[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

