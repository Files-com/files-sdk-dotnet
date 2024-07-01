using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class FtpActionLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FtpActionLog() : this(null, null) { }

        public FtpActionLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("timestamp"))
            {
                this.attributes.Add("timestamp", null);
            }
            if (!this.attributes.ContainsKey("remote_ip"))
            {
                this.attributes.Add("remote_ip", null);
            }
            if (!this.attributes.ContainsKey("server_ip"))
            {
                this.attributes.Add("server_ip", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("session_uuid"))
            {
                this.attributes.Add("session_uuid", null);
            }
            if (!this.attributes.ContainsKey("seq_id"))
            {
                this.attributes.Add("seq_id", null);
            }
            if (!this.attributes.ContainsKey("auth_ciphers"))
            {
                this.attributes.Add("auth_ciphers", null);
            }
            if (!this.attributes.ContainsKey("action_type"))
            {
                this.attributes.Add("action_type", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("true_path"))
            {
                this.attributes.Add("true_path", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("cmd"))
            {
                this.attributes.Add("cmd", null);
            }
            if (!this.attributes.ContainsKey("param"))
            {
                this.attributes.Add("param", null);
            }
            if (!this.attributes.ContainsKey("responseCode"))
            {
                this.attributes.Add("responseCode", null);
            }
            if (!this.attributes.ContainsKey("responseMessage"))
            {
                this.attributes.Add("responseMessage", null);
            }
            if (!this.attributes.ContainsKey("entries_returned"))
            {
                this.attributes.Add("entries_returned", null);
            }
            if (!this.attributes.ContainsKey("success"))
            {
                this.attributes.Add("success", false);
            }
            if (!this.attributes.ContainsKey("duration_ms"))
            {
                this.attributes.Add("duration_ms", null);
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
        /// Start Time of Action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("timestamp")]
        public Nullable<DateTime> Timestamp
        {
            get { return (Nullable<DateTime>)attributes["timestamp"]; }
            private set { attributes["timestamp"] = value; }
        }

        /// <summary>
        /// IP Address of FTP Client
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("remote_ip")]
        public string RemoteIp
        {
            get { return (string)attributes["remote_ip"]; }
            private set { attributes["remote_ip"] = value; }
        }

        /// <summary>
        /// IP Address of FTP Server
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_ip")]
        public string ServerIp
        {
            get { return (string)attributes["server_ip"]; }
            private set { attributes["server_ip"] = value; }
        }

        /// <summary>
        /// Username
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            private set { attributes["username"] = value; }
        }

        /// <summary>
        /// Unique ID of the Session
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("session_uuid")]
        public string SessionUuid
        {
            get { return (string)attributes["session_uuid"]; }
            private set { attributes["session_uuid"] = value; }
        }

        /// <summary>
        /// FTP Sequence ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("seq_id")]
        public Nullable<Int64> SeqId
        {
            get { return (Nullable<Int64>)attributes["seq_id"]; }
            private set { attributes["seq_id"] = value; }
        }

        /// <summary>
        /// Authentication Ciphers
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("auth_ciphers")]
        public string AuthCiphers
        {
            get { return (string)attributes["auth_ciphers"]; }
            private set { attributes["auth_ciphers"] = value; }
        }

        /// <summary>
        /// Action Type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("action_type")]
        public string ActionType
        {
            get { return (string)attributes["action_type"]; }
            private set { attributes["action_type"] = value; }
        }

        /// <summary>
        /// Path as sent by the Client (may not match Files.com path due to user root folders for FTP) This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// Path on Files.com
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("true_path")]
        public string TruePath
        {
            get { return (string)attributes["true_path"]; }
            private set { attributes["true_path"] = value; }
        }

        /// <summary>
        /// Name of File
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            private set { attributes["name"] = value; }
        }

        /// <summary>
        /// FTP Command Name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cmd")]
        public string Cmd
        {
            get { return (string)attributes["cmd"]; }
            private set { attributes["cmd"] = value; }
        }

        /// <summary>
        /// FTP Command Param
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("param")]
        public string Param
        {
            get { return (string)attributes["param"]; }
            private set { attributes["param"] = value; }
        }

        /// <summary>
        /// FTP Response Code
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("responseCode")]
        public Nullable<Int64> ResponseCode
        {
            get { return (Nullable<Int64>)attributes["responseCode"]; }
            private set { attributes["responseCode"] = value; }
        }

        /// <summary>
        /// FTP Response Message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("responseMessage")]
        public string ResponseMessage
        {
            get { return (string)attributes["responseMessage"]; }
            private set { attributes["responseMessage"] = value; }
        }

        /// <summary>
        /// Number of entries returned when listing files and folders
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("entries_returned")]
        public Nullable<Int64> EntriesReturned
        {
            get { return (Nullable<Int64>)attributes["entries_returned"]; }
            private set { attributes["entries_returned"] = value; }
        }

        /// <summary>
        /// Whether FTP Action was successful.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("success")]
        public bool Success
        {
            get { return attributes["success"] == null ? false : (bool)attributes["success"]; }
            private set { attributes["success"] = value; }
        }

        /// <summary>
        /// Duration (in milliseconds)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("duration_ms")]
        public Nullable<Int64> DurationMs
        {
            get { return (Nullable<Int64>)attributes["duration_ms"]; }
            private set { attributes["duration_ms"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `path`, `true_path`, `remote_ip`, `session_uuid`, `success`, `action_type` or `username`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ path ]`, `[ true_path ]`, `[ remote_ip ]`, `[ session_uuid ]`, `[ success ]`, `[ action_type ]`, `[ username ]`, `[ start_date, end_date ]`, `[ start_date, path ]`, `[ start_date, true_path ]`, `[ start_date, remote_ip ]`, `[ start_date, session_uuid ]`, `[ start_date, success ]`, `[ start_date, action_type ]`, `[ start_date, username ]`, `[ end_date, path ]`, `[ end_date, true_path ]`, `[ end_date, remote_ip ]`, `[ end_date, session_uuid ]`, `[ end_date, success ]`, `[ end_date, action_type ]`, `[ end_date, username ]`, `[ path, true_path ]`, `[ path, remote_ip ]`, `[ path, session_uuid ]`, `[ path, success ]`, `[ path, action_type ]`, `[ path, username ]`, `[ true_path, remote_ip ]`, `[ true_path, session_uuid ]`, `[ true_path, success ]`, `[ true_path, action_type ]`, `[ true_path, username ]`, `[ remote_ip, session_uuid ]`, `[ remote_ip, success ]`, `[ remote_ip, action_type ]`, `[ remote_ip, username ]`, `[ session_uuid, success ]`, `[ session_uuid, action_type ]`, `[ session_uuid, username ]`, `[ success, action_type ]`, `[ success, username ]`, `[ action_type, username ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, true_path ]`, `[ start_date, end_date, remote_ip ]`, `[ start_date, end_date, session_uuid ]`, `[ start_date, end_date, success ]`, `[ start_date, end_date, action_type ]`, `[ start_date, end_date, username ]`, `[ start_date, path, true_path ]`, `[ start_date, path, remote_ip ]`, `[ start_date, path, session_uuid ]`, `[ start_date, path, success ]`, `[ start_date, path, action_type ]`, `[ start_date, path, username ]`, `[ start_date, true_path, remote_ip ]`, `[ start_date, true_path, session_uuid ]`, `[ start_date, true_path, success ]`, `[ start_date, true_path, action_type ]`, `[ start_date, true_path, username ]`, `[ start_date, remote_ip, session_uuid ]`, `[ start_date, remote_ip, success ]`, `[ start_date, remote_ip, action_type ]`, `[ start_date, remote_ip, username ]`, `[ start_date, session_uuid, success ]`, `[ start_date, session_uuid, action_type ]`, `[ start_date, session_uuid, username ]`, `[ start_date, success, action_type ]`, `[ start_date, success, username ]`, `[ start_date, action_type, username ]`, `[ end_date, path, true_path ]`, `[ end_date, path, remote_ip ]`, `[ end_date, path, session_uuid ]`, `[ end_date, path, success ]`, `[ end_date, path, action_type ]`, `[ end_date, path, username ]`, `[ end_date, true_path, remote_ip ]`, `[ end_date, true_path, session_uuid ]`, `[ end_date, true_path, success ]`, `[ end_date, true_path, action_type ]`, `[ end_date, true_path, username ]`, `[ end_date, remote_ip, session_uuid ]`, `[ end_date, remote_ip, success ]`, `[ end_date, remote_ip, action_type ]`, `[ end_date, remote_ip, username ]`, `[ end_date, session_uuid, success ]`, `[ end_date, session_uuid, action_type ]`, `[ end_date, session_uuid, username ]`, `[ end_date, success, action_type ]`, `[ end_date, success, username ]`, `[ end_date, action_type, username ]`, `[ path, true_path, remote_ip ]`, `[ path, true_path, session_uuid ]`, `[ path, true_path, success ]`, `[ path, true_path, action_type ]`, `[ path, true_path, username ]`, `[ path, remote_ip, session_uuid ]`, `[ path, remote_ip, success ]`, `[ path, remote_ip, action_type ]`, `[ path, remote_ip, username ]`, `[ path, session_uuid, success ]`, `[ path, session_uuid, action_type ]`, `[ path, session_uuid, username ]`, `[ path, success, action_type ]`, `[ path, success, username ]`, `[ path, action_type, username ]`, `[ true_path, remote_ip, session_uuid ]`, `[ true_path, remote_ip, success ]`, `[ true_path, remote_ip, action_type ]`, `[ true_path, remote_ip, username ]`, `[ true_path, session_uuid, success ]`, `[ true_path, session_uuid, action_type ]`, `[ true_path, session_uuid, username ]`, `[ true_path, success, action_type ]`, `[ true_path, success, username ]`, `[ true_path, action_type, username ]`, `[ remote_ip, session_uuid, success ]`, `[ remote_ip, session_uuid, action_type ]`, `[ remote_ip, session_uuid, username ]`, `[ remote_ip, success, action_type ]`, `[ remote_ip, success, username ]`, `[ remote_ip, action_type, username ]`, `[ session_uuid, success, action_type ]`, `[ session_uuid, success, username ]`, `[ session_uuid, action_type, username ]`, `[ success, action_type, username ]`, `[ start_date, end_date, path, true_path ]`, `[ start_date, end_date, path, remote_ip ]`, `[ start_date, end_date, path, session_uuid ]`, `[ start_date, end_date, path, success ]`, `[ start_date, end_date, path, action_type ]`, `[ start_date, end_date, path, username ]`, `[ start_date, end_date, true_path, remote_ip ]`, `[ start_date, end_date, true_path, session_uuid ]`, `[ start_date, end_date, true_path, success ]`, `[ start_date, end_date, true_path, action_type ]`, `[ start_date, end_date, true_path, username ]`, `[ start_date, end_date, remote_ip, session_uuid ]`, `[ start_date, end_date, remote_ip, success ]`, `[ start_date, end_date, remote_ip, action_type ]`, `[ start_date, end_date, remote_ip, username ]`, `[ start_date, end_date, session_uuid, success ]`, `[ start_date, end_date, session_uuid, action_type ]`, `[ start_date, end_date, session_uuid, username ]`, `[ start_date, end_date, success, action_type ]`, `[ start_date, end_date, success, username ]`, `[ start_date, end_date, action_type, username ]`, `[ start_date, path, true_path, remote_ip ]`, `[ start_date, path, true_path, session_uuid ]`, `[ start_date, path, true_path, success ]`, `[ start_date, path, true_path, action_type ]`, `[ start_date, path, true_path, username ]`, `[ start_date, path, remote_ip, session_uuid ]`, `[ start_date, path, remote_ip, success ]`, `[ start_date, path, remote_ip, action_type ]`, `[ start_date, path, remote_ip, username ]`, `[ start_date, path, session_uuid, success ]`, `[ start_date, path, session_uuid, action_type ]`, `[ start_date, path, session_uuid, username ]`, `[ start_date, path, success, action_type ]`, `[ start_date, path, success, username ]`, `[ start_date, path, action_type, username ]`, `[ start_date, true_path, remote_ip, session_uuid ]`, `[ start_date, true_path, remote_ip, success ]`, `[ start_date, true_path, remote_ip, action_type ]`, `[ start_date, true_path, remote_ip, username ]`, `[ start_date, true_path, session_uuid, success ]`, `[ start_date, true_path, session_uuid, action_type ]`, `[ start_date, true_path, session_uuid, username ]`, `[ start_date, true_path, success, action_type ]`, `[ start_date, true_path, success, username ]`, `[ start_date, true_path, action_type, username ]`, `[ start_date, remote_ip, session_uuid, success ]`, `[ start_date, remote_ip, session_uuid, action_type ]`, `[ start_date, remote_ip, session_uuid, username ]`, `[ start_date, remote_ip, success, action_type ]`, `[ start_date, remote_ip, success, username ]`, `[ start_date, remote_ip, action_type, username ]`, `[ start_date, session_uuid, success, action_type ]`, `[ start_date, session_uuid, success, username ]`, `[ start_date, session_uuid, action_type, username ]`, `[ start_date, success, action_type, username ]`, `[ end_date, path, true_path, remote_ip ]`, `[ end_date, path, true_path, session_uuid ]`, `[ end_date, path, true_path, success ]`, `[ end_date, path, true_path, action_type ]`, `[ end_date, path, true_path, username ]`, `[ end_date, path, remote_ip, session_uuid ]`, `[ end_date, path, remote_ip, success ]`, `[ end_date, path, remote_ip, action_type ]`, `[ end_date, path, remote_ip, username ]`, `[ end_date, path, session_uuid, success ]`, `[ end_date, path, session_uuid, action_type ]`, `[ end_date, path, session_uuid, username ]`, `[ end_date, path, success, action_type ]`, `[ end_date, path, success, username ]`, `[ end_date, path, action_type, username ]`, `[ end_date, true_path, remote_ip, session_uuid ]`, `[ end_date, true_path, remote_ip, success ]`, `[ end_date, true_path, remote_ip, action_type ]`, `[ end_date, true_path, remote_ip, username ]`, `[ end_date, true_path, session_uuid, success ]`, `[ end_date, true_path, session_uuid, action_type ]`, `[ end_date, true_path, session_uuid, username ]`, `[ end_date, true_path, success, action_type ]`, `[ end_date, true_path, success, username ]`, `[ end_date, true_path, action_type, username ]`, `[ end_date, remote_ip, session_uuid, success ]`, `[ end_date, remote_ip, session_uuid, action_type ]`, `[ end_date, remote_ip, session_uuid, username ]`, `[ end_date, remote_ip, success, action_type ]`, `[ end_date, remote_ip, success, username ]`, `[ end_date, remote_ip, action_type, username ]`, `[ end_date, session_uuid, success, action_type ]`, `[ end_date, session_uuid, success, username ]`, `[ end_date, session_uuid, action_type, username ]`, `[ end_date, success, action_type, username ]`, `[ path, true_path, remote_ip, session_uuid ]`, `[ path, true_path, remote_ip, success ]`, `[ path, true_path, remote_ip, action_type ]`, `[ path, true_path, remote_ip, username ]`, `[ path, true_path, session_uuid, success ]`, `[ path, true_path, session_uuid, action_type ]`, `[ path, true_path, session_uuid, username ]`, `[ path, true_path, success, action_type ]`, `[ path, true_path, success, username ]`, `[ path, true_path, action_type, username ]`, `[ path, remote_ip, session_uuid, success ]`, `[ path, remote_ip, session_uuid, action_type ]`, `[ path, remote_ip, session_uuid, username ]`, `[ path, remote_ip, success, action_type ]`, `[ path, remote_ip, success, username ]`, `[ path, remote_ip, action_type, username ]`, `[ path, session_uuid, success, action_type ]`, `[ path, session_uuid, success, username ]`, `[ path, session_uuid, action_type, username ]`, `[ path, success, action_type, username ]`, `[ true_path, remote_ip, session_uuid, success ]`, `[ true_path, remote_ip, session_uuid, action_type ]`, `[ true_path, remote_ip, session_uuid, username ]`, `[ true_path, remote_ip, success, action_type ]`, `[ true_path, remote_ip, success, username ]`, `[ true_path, remote_ip, action_type, username ]`, `[ true_path, session_uuid, success, action_type ]`, `[ true_path, session_uuid, success, username ]`, `[ true_path, session_uuid, action_type, username ]`, `[ true_path, success, action_type, username ]`, `[ remote_ip, session_uuid, success, action_type ]`, `[ remote_ip, session_uuid, success, username ]`, `[ remote_ip, session_uuid, action_type, username ]`, `[ remote_ip, success, action_type, username ]`, `[ session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip ]`, `[ start_date, end_date, path, true_path, session_uuid ]`, `[ start_date, end_date, path, true_path, success ]`, `[ start_date, end_date, path, true_path, action_type ]`, `[ start_date, end_date, path, true_path, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid ]`, `[ start_date, end_date, path, remote_ip, success ]`, `[ start_date, end_date, path, remote_ip, action_type ]`, `[ start_date, end_date, path, remote_ip, username ]`, `[ start_date, end_date, path, session_uuid, success ]`, `[ start_date, end_date, path, session_uuid, action_type ]`, `[ start_date, end_date, path, session_uuid, username ]`, `[ start_date, end_date, path, success, action_type ]`, `[ start_date, end_date, path, success, username ]`, `[ start_date, end_date, path, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid ]`, `[ start_date, end_date, true_path, remote_ip, success ]`, `[ start_date, end_date, true_path, remote_ip, action_type ]`, `[ start_date, end_date, true_path, remote_ip, username ]`, `[ start_date, end_date, true_path, session_uuid, success ]`, `[ start_date, end_date, true_path, session_uuid, action_type ]`, `[ start_date, end_date, true_path, session_uuid, username ]`, `[ start_date, end_date, true_path, success, action_type ]`, `[ start_date, end_date, true_path, success, username ]`, `[ start_date, end_date, true_path, action_type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success ]`, `[ start_date, end_date, remote_ip, session_uuid, action_type ]`, `[ start_date, end_date, remote_ip, session_uuid, username ]`, `[ start_date, end_date, remote_ip, success, action_type ]`, `[ start_date, end_date, remote_ip, success, username ]`, `[ start_date, end_date, remote_ip, action_type, username ]`, `[ start_date, end_date, session_uuid, success, action_type ]`, `[ start_date, end_date, session_uuid, success, username ]`, `[ start_date, end_date, session_uuid, action_type, username ]`, `[ start_date, end_date, success, action_type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid ]`, `[ start_date, path, true_path, remote_ip, success ]`, `[ start_date, path, true_path, remote_ip, action_type ]`, `[ start_date, path, true_path, remote_ip, username ]`, `[ start_date, path, true_path, session_uuid, success ]`, `[ start_date, path, true_path, session_uuid, action_type ]`, `[ start_date, path, true_path, session_uuid, username ]`, `[ start_date, path, true_path, success, action_type ]`, `[ start_date, path, true_path, success, username ]`, `[ start_date, path, true_path, action_type, username ]`, `[ start_date, path, remote_ip, session_uuid, success ]`, `[ start_date, path, remote_ip, session_uuid, action_type ]`, `[ start_date, path, remote_ip, session_uuid, username ]`, `[ start_date, path, remote_ip, success, action_type ]`, `[ start_date, path, remote_ip, success, username ]`, `[ start_date, path, remote_ip, action_type, username ]`, `[ start_date, path, session_uuid, success, action_type ]`, `[ start_date, path, session_uuid, success, username ]`, `[ start_date, path, session_uuid, action_type, username ]`, `[ start_date, path, success, action_type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success ]`, `[ start_date, true_path, remote_ip, session_uuid, action_type ]`, `[ start_date, true_path, remote_ip, session_uuid, username ]`, `[ start_date, true_path, remote_ip, success, action_type ]`, `[ start_date, true_path, remote_ip, success, username ]`, `[ start_date, true_path, remote_ip, action_type, username ]`, `[ start_date, true_path, session_uuid, success, action_type ]`, `[ start_date, true_path, session_uuid, success, username ]`, `[ start_date, true_path, session_uuid, action_type, username ]`, `[ start_date, true_path, success, action_type, username ]`, `[ start_date, remote_ip, session_uuid, success, action_type ]`, `[ start_date, remote_ip, session_uuid, success, username ]`, `[ start_date, remote_ip, session_uuid, action_type, username ]`, `[ start_date, remote_ip, success, action_type, username ]`, `[ start_date, session_uuid, success, action_type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid ]`, `[ end_date, path, true_path, remote_ip, success ]`, `[ end_date, path, true_path, remote_ip, action_type ]`, `[ end_date, path, true_path, remote_ip, username ]`, `[ end_date, path, true_path, session_uuid, success ]`, `[ end_date, path, true_path, session_uuid, action_type ]`, `[ end_date, path, true_path, session_uuid, username ]`, `[ end_date, path, true_path, success, action_type ]`, `[ end_date, path, true_path, success, username ]`, `[ end_date, path, true_path, action_type, username ]`, `[ end_date, path, remote_ip, session_uuid, success ]`, `[ end_date, path, remote_ip, session_uuid, action_type ]`, `[ end_date, path, remote_ip, session_uuid, username ]`, `[ end_date, path, remote_ip, success, action_type ]`, `[ end_date, path, remote_ip, success, username ]`, `[ end_date, path, remote_ip, action_type, username ]`, `[ end_date, path, session_uuid, success, action_type ]`, `[ end_date, path, session_uuid, success, username ]`, `[ end_date, path, session_uuid, action_type, username ]`, `[ end_date, path, success, action_type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success ]`, `[ end_date, true_path, remote_ip, session_uuid, action_type ]`, `[ end_date, true_path, remote_ip, session_uuid, username ]`, `[ end_date, true_path, remote_ip, success, action_type ]`, `[ end_date, true_path, remote_ip, success, username ]`, `[ end_date, true_path, remote_ip, action_type, username ]`, `[ end_date, true_path, session_uuid, success, action_type ]`, `[ end_date, true_path, session_uuid, success, username ]`, `[ end_date, true_path, session_uuid, action_type, username ]`, `[ end_date, true_path, success, action_type, username ]`, `[ end_date, remote_ip, session_uuid, success, action_type ]`, `[ end_date, remote_ip, session_uuid, success, username ]`, `[ end_date, remote_ip, session_uuid, action_type, username ]`, `[ end_date, remote_ip, success, action_type, username ]`, `[ end_date, session_uuid, success, action_type, username ]`, `[ path, true_path, remote_ip, session_uuid, success ]`, `[ path, true_path, remote_ip, session_uuid, action_type ]`, `[ path, true_path, remote_ip, session_uuid, username ]`, `[ path, true_path, remote_ip, success, action_type ]`, `[ path, true_path, remote_ip, success, username ]`, `[ path, true_path, remote_ip, action_type, username ]`, `[ path, true_path, session_uuid, success, action_type ]`, `[ path, true_path, session_uuid, success, username ]`, `[ path, true_path, session_uuid, action_type, username ]`, `[ path, true_path, success, action_type, username ]`, `[ path, remote_ip, session_uuid, success, action_type ]`, `[ path, remote_ip, session_uuid, success, username ]`, `[ path, remote_ip, session_uuid, action_type, username ]`, `[ path, remote_ip, success, action_type, username ]`, `[ path, session_uuid, success, action_type, username ]`, `[ true_path, remote_ip, session_uuid, success, action_type ]`, `[ true_path, remote_ip, session_uuid, success, username ]`, `[ true_path, remote_ip, session_uuid, action_type, username ]`, `[ true_path, remote_ip, success, action_type, username ]`, `[ true_path, session_uuid, success, action_type, username ]`, `[ remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid ]`, `[ start_date, end_date, path, true_path, remote_ip, success ]`, `[ start_date, end_date, path, true_path, remote_ip, action_type ]`, `[ start_date, end_date, path, true_path, remote_ip, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success ]`, `[ start_date, end_date, path, true_path, session_uuid, action_type ]`, `[ start_date, end_date, path, true_path, session_uuid, username ]`, `[ start_date, end_date, path, true_path, success, action_type ]`, `[ start_date, end_date, path, true_path, success, username ]`, `[ start_date, end_date, path, true_path, action_type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, path, remote_ip, session_uuid, action_type ]`, `[ start_date, end_date, path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, path, remote_ip, success, action_type ]`, `[ start_date, end_date, path, remote_ip, success, username ]`, `[ start_date, end_date, path, remote_ip, action_type, username ]`, `[ start_date, end_date, path, session_uuid, success, action_type ]`, `[ start_date, end_date, path, session_uuid, success, username ]`, `[ start_date, end_date, path, session_uuid, action_type, username ]`, `[ start_date, end_date, path, success, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, action_type ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, true_path, remote_ip, success, action_type ]`, `[ start_date, end_date, true_path, remote_ip, success, username ]`, `[ start_date, end_date, true_path, remote_ip, action_type, username ]`, `[ start_date, end_date, true_path, session_uuid, success, action_type ]`, `[ start_date, end_date, true_path, session_uuid, success, username ]`, `[ start_date, end_date, true_path, session_uuid, action_type, username ]`, `[ start_date, end_date, true_path, success, action_type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success, action_type ]`, `[ start_date, end_date, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, remote_ip, session_uuid, action_type, username ]`, `[ start_date, end_date, remote_ip, success, action_type, username ]`, `[ start_date, end_date, session_uuid, success, action_type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success ]`, `[ start_date, path, true_path, remote_ip, session_uuid, action_type ]`, `[ start_date, path, true_path, remote_ip, session_uuid, username ]`, `[ start_date, path, true_path, remote_ip, success, action_type ]`, `[ start_date, path, true_path, remote_ip, success, username ]`, `[ start_date, path, true_path, remote_ip, action_type, username ]`, `[ start_date, path, true_path, session_uuid, success, action_type ]`, `[ start_date, path, true_path, session_uuid, success, username ]`, `[ start_date, path, true_path, session_uuid, action_type, username ]`, `[ start_date, path, true_path, success, action_type, username ]`, `[ start_date, path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, path, remote_ip, session_uuid, success, username ]`, `[ start_date, path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, path, remote_ip, success, action_type, username ]`, `[ start_date, path, session_uuid, success, action_type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, true_path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, true_path, remote_ip, success, action_type, username ]`, `[ start_date, true_path, session_uuid, success, action_type, username ]`, `[ start_date, remote_ip, session_uuid, success, action_type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success ]`, `[ end_date, path, true_path, remote_ip, session_uuid, action_type ]`, `[ end_date, path, true_path, remote_ip, session_uuid, username ]`, `[ end_date, path, true_path, remote_ip, success, action_type ]`, `[ end_date, path, true_path, remote_ip, success, username ]`, `[ end_date, path, true_path, remote_ip, action_type, username ]`, `[ end_date, path, true_path, session_uuid, success, action_type ]`, `[ end_date, path, true_path, session_uuid, success, username ]`, `[ end_date, path, true_path, session_uuid, action_type, username ]`, `[ end_date, path, true_path, success, action_type, username ]`, `[ end_date, path, remote_ip, session_uuid, success, action_type ]`, `[ end_date, path, remote_ip, session_uuid, success, username ]`, `[ end_date, path, remote_ip, session_uuid, action_type, username ]`, `[ end_date, path, remote_ip, success, action_type, username ]`, `[ end_date, path, session_uuid, success, action_type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success, action_type ]`, `[ end_date, true_path, remote_ip, session_uuid, success, username ]`, `[ end_date, true_path, remote_ip, session_uuid, action_type, username ]`, `[ end_date, true_path, remote_ip, success, action_type, username ]`, `[ end_date, true_path, session_uuid, success, action_type, username ]`, `[ end_date, remote_ip, session_uuid, success, action_type, username ]`, `[ path, true_path, remote_ip, session_uuid, success, action_type ]`, `[ path, true_path, remote_ip, session_uuid, success, username ]`, `[ path, true_path, remote_ip, session_uuid, action_type, username ]`, `[ path, true_path, remote_ip, success, action_type, username ]`, `[ path, true_path, session_uuid, success, action_type, username ]`, `[ path, remote_ip, session_uuid, success, action_type, username ]`, `[ true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, action_type ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, path, true_path, remote_ip, success, action_type ]`, `[ start_date, end_date, path, true_path, remote_ip, success, username ]`, `[ start_date, end_date, path, true_path, remote_ip, action_type, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success, action_type ]`, `[ start_date, end_date, path, true_path, session_uuid, success, username ]`, `[ start_date, end_date, path, true_path, session_uuid, action_type, username ]`, `[ start_date, end_date, path, true_path, success, action_type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, end_date, path, remote_ip, success, action_type, username ]`, `[ start_date, end_date, path, session_uuid, success, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, success, action_type, username ]`, `[ start_date, end_date, true_path, session_uuid, success, action_type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, path, true_path, remote_ip, success, action_type, username ]`, `[ start_date, path, true_path, session_uuid, success, action_type, username ]`, `[ start_date, path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success, action_type ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, action_type, username ]`, `[ end_date, path, true_path, remote_ip, success, action_type, username ]`, `[ end_date, path, true_path, session_uuid, success, action_type, username ]`, `[ end_date, path, remote_ip, session_uuid, success, action_type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ path, true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, success, action_type, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, action_type, username ]` or `[ end_date, path, true_path, remote_ip, session_uuid, success, action_type, username ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`, `true_path`, `action_type` or `username`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ path ]`, `[ true_path ]`, `[ remote_ip ]`, `[ session_uuid ]`, `[ success ]`, `[ action_type ]`, `[ username ]`, `[ start_date, end_date ]`, `[ start_date, path ]`, `[ start_date, true_path ]`, `[ start_date, remote_ip ]`, `[ start_date, session_uuid ]`, `[ start_date, success ]`, `[ start_date, action_type ]`, `[ start_date, username ]`, `[ end_date, path ]`, `[ end_date, true_path ]`, `[ end_date, remote_ip ]`, `[ end_date, session_uuid ]`, `[ end_date, success ]`, `[ end_date, action_type ]`, `[ end_date, username ]`, `[ path, true_path ]`, `[ path, remote_ip ]`, `[ path, session_uuid ]`, `[ path, success ]`, `[ path, action_type ]`, `[ path, username ]`, `[ true_path, remote_ip ]`, `[ true_path, session_uuid ]`, `[ true_path, success ]`, `[ true_path, action_type ]`, `[ true_path, username ]`, `[ remote_ip, session_uuid ]`, `[ remote_ip, success ]`, `[ remote_ip, action_type ]`, `[ remote_ip, username ]`, `[ session_uuid, success ]`, `[ session_uuid, action_type ]`, `[ session_uuid, username ]`, `[ success, action_type ]`, `[ success, username ]`, `[ action_type, username ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, true_path ]`, `[ start_date, end_date, remote_ip ]`, `[ start_date, end_date, session_uuid ]`, `[ start_date, end_date, success ]`, `[ start_date, end_date, action_type ]`, `[ start_date, end_date, username ]`, `[ start_date, path, true_path ]`, `[ start_date, path, remote_ip ]`, `[ start_date, path, session_uuid ]`, `[ start_date, path, success ]`, `[ start_date, path, action_type ]`, `[ start_date, path, username ]`, `[ start_date, true_path, remote_ip ]`, `[ start_date, true_path, session_uuid ]`, `[ start_date, true_path, success ]`, `[ start_date, true_path, action_type ]`, `[ start_date, true_path, username ]`, `[ start_date, remote_ip, session_uuid ]`, `[ start_date, remote_ip, success ]`, `[ start_date, remote_ip, action_type ]`, `[ start_date, remote_ip, username ]`, `[ start_date, session_uuid, success ]`, `[ start_date, session_uuid, action_type ]`, `[ start_date, session_uuid, username ]`, `[ start_date, success, action_type ]`, `[ start_date, success, username ]`, `[ start_date, action_type, username ]`, `[ end_date, path, true_path ]`, `[ end_date, path, remote_ip ]`, `[ end_date, path, session_uuid ]`, `[ end_date, path, success ]`, `[ end_date, path, action_type ]`, `[ end_date, path, username ]`, `[ end_date, true_path, remote_ip ]`, `[ end_date, true_path, session_uuid ]`, `[ end_date, true_path, success ]`, `[ end_date, true_path, action_type ]`, `[ end_date, true_path, username ]`, `[ end_date, remote_ip, session_uuid ]`, `[ end_date, remote_ip, success ]`, `[ end_date, remote_ip, action_type ]`, `[ end_date, remote_ip, username ]`, `[ end_date, session_uuid, success ]`, `[ end_date, session_uuid, action_type ]`, `[ end_date, session_uuid, username ]`, `[ end_date, success, action_type ]`, `[ end_date, success, username ]`, `[ end_date, action_type, username ]`, `[ path, true_path, remote_ip ]`, `[ path, true_path, session_uuid ]`, `[ path, true_path, success ]`, `[ path, true_path, action_type ]`, `[ path, true_path, username ]`, `[ path, remote_ip, session_uuid ]`, `[ path, remote_ip, success ]`, `[ path, remote_ip, action_type ]`, `[ path, remote_ip, username ]`, `[ path, session_uuid, success ]`, `[ path, session_uuid, action_type ]`, `[ path, session_uuid, username ]`, `[ path, success, action_type ]`, `[ path, success, username ]`, `[ path, action_type, username ]`, `[ true_path, remote_ip, session_uuid ]`, `[ true_path, remote_ip, success ]`, `[ true_path, remote_ip, action_type ]`, `[ true_path, remote_ip, username ]`, `[ true_path, session_uuid, success ]`, `[ true_path, session_uuid, action_type ]`, `[ true_path, session_uuid, username ]`, `[ true_path, success, action_type ]`, `[ true_path, success, username ]`, `[ true_path, action_type, username ]`, `[ remote_ip, session_uuid, success ]`, `[ remote_ip, session_uuid, action_type ]`, `[ remote_ip, session_uuid, username ]`, `[ remote_ip, success, action_type ]`, `[ remote_ip, success, username ]`, `[ remote_ip, action_type, username ]`, `[ session_uuid, success, action_type ]`, `[ session_uuid, success, username ]`, `[ session_uuid, action_type, username ]`, `[ success, action_type, username ]`, `[ start_date, end_date, path, true_path ]`, `[ start_date, end_date, path, remote_ip ]`, `[ start_date, end_date, path, session_uuid ]`, `[ start_date, end_date, path, success ]`, `[ start_date, end_date, path, action_type ]`, `[ start_date, end_date, path, username ]`, `[ start_date, end_date, true_path, remote_ip ]`, `[ start_date, end_date, true_path, session_uuid ]`, `[ start_date, end_date, true_path, success ]`, `[ start_date, end_date, true_path, action_type ]`, `[ start_date, end_date, true_path, username ]`, `[ start_date, end_date, remote_ip, session_uuid ]`, `[ start_date, end_date, remote_ip, success ]`, `[ start_date, end_date, remote_ip, action_type ]`, `[ start_date, end_date, remote_ip, username ]`, `[ start_date, end_date, session_uuid, success ]`, `[ start_date, end_date, session_uuid, action_type ]`, `[ start_date, end_date, session_uuid, username ]`, `[ start_date, end_date, success, action_type ]`, `[ start_date, end_date, success, username ]`, `[ start_date, end_date, action_type, username ]`, `[ start_date, path, true_path, remote_ip ]`, `[ start_date, path, true_path, session_uuid ]`, `[ start_date, path, true_path, success ]`, `[ start_date, path, true_path, action_type ]`, `[ start_date, path, true_path, username ]`, `[ start_date, path, remote_ip, session_uuid ]`, `[ start_date, path, remote_ip, success ]`, `[ start_date, path, remote_ip, action_type ]`, `[ start_date, path, remote_ip, username ]`, `[ start_date, path, session_uuid, success ]`, `[ start_date, path, session_uuid, action_type ]`, `[ start_date, path, session_uuid, username ]`, `[ start_date, path, success, action_type ]`, `[ start_date, path, success, username ]`, `[ start_date, path, action_type, username ]`, `[ start_date, true_path, remote_ip, session_uuid ]`, `[ start_date, true_path, remote_ip, success ]`, `[ start_date, true_path, remote_ip, action_type ]`, `[ start_date, true_path, remote_ip, username ]`, `[ start_date, true_path, session_uuid, success ]`, `[ start_date, true_path, session_uuid, action_type ]`, `[ start_date, true_path, session_uuid, username ]`, `[ start_date, true_path, success, action_type ]`, `[ start_date, true_path, success, username ]`, `[ start_date, true_path, action_type, username ]`, `[ start_date, remote_ip, session_uuid, success ]`, `[ start_date, remote_ip, session_uuid, action_type ]`, `[ start_date, remote_ip, session_uuid, username ]`, `[ start_date, remote_ip, success, action_type ]`, `[ start_date, remote_ip, success, username ]`, `[ start_date, remote_ip, action_type, username ]`, `[ start_date, session_uuid, success, action_type ]`, `[ start_date, session_uuid, success, username ]`, `[ start_date, session_uuid, action_type, username ]`, `[ start_date, success, action_type, username ]`, `[ end_date, path, true_path, remote_ip ]`, `[ end_date, path, true_path, session_uuid ]`, `[ end_date, path, true_path, success ]`, `[ end_date, path, true_path, action_type ]`, `[ end_date, path, true_path, username ]`, `[ end_date, path, remote_ip, session_uuid ]`, `[ end_date, path, remote_ip, success ]`, `[ end_date, path, remote_ip, action_type ]`, `[ end_date, path, remote_ip, username ]`, `[ end_date, path, session_uuid, success ]`, `[ end_date, path, session_uuid, action_type ]`, `[ end_date, path, session_uuid, username ]`, `[ end_date, path, success, action_type ]`, `[ end_date, path, success, username ]`, `[ end_date, path, action_type, username ]`, `[ end_date, true_path, remote_ip, session_uuid ]`, `[ end_date, true_path, remote_ip, success ]`, `[ end_date, true_path, remote_ip, action_type ]`, `[ end_date, true_path, remote_ip, username ]`, `[ end_date, true_path, session_uuid, success ]`, `[ end_date, true_path, session_uuid, action_type ]`, `[ end_date, true_path, session_uuid, username ]`, `[ end_date, true_path, success, action_type ]`, `[ end_date, true_path, success, username ]`, `[ end_date, true_path, action_type, username ]`, `[ end_date, remote_ip, session_uuid, success ]`, `[ end_date, remote_ip, session_uuid, action_type ]`, `[ end_date, remote_ip, session_uuid, username ]`, `[ end_date, remote_ip, success, action_type ]`, `[ end_date, remote_ip, success, username ]`, `[ end_date, remote_ip, action_type, username ]`, `[ end_date, session_uuid, success, action_type ]`, `[ end_date, session_uuid, success, username ]`, `[ end_date, session_uuid, action_type, username ]`, `[ end_date, success, action_type, username ]`, `[ path, true_path, remote_ip, session_uuid ]`, `[ path, true_path, remote_ip, success ]`, `[ path, true_path, remote_ip, action_type ]`, `[ path, true_path, remote_ip, username ]`, `[ path, true_path, session_uuid, success ]`, `[ path, true_path, session_uuid, action_type ]`, `[ path, true_path, session_uuid, username ]`, `[ path, true_path, success, action_type ]`, `[ path, true_path, success, username ]`, `[ path, true_path, action_type, username ]`, `[ path, remote_ip, session_uuid, success ]`, `[ path, remote_ip, session_uuid, action_type ]`, `[ path, remote_ip, session_uuid, username ]`, `[ path, remote_ip, success, action_type ]`, `[ path, remote_ip, success, username ]`, `[ path, remote_ip, action_type, username ]`, `[ path, session_uuid, success, action_type ]`, `[ path, session_uuid, success, username ]`, `[ path, session_uuid, action_type, username ]`, `[ path, success, action_type, username ]`, `[ true_path, remote_ip, session_uuid, success ]`, `[ true_path, remote_ip, session_uuid, action_type ]`, `[ true_path, remote_ip, session_uuid, username ]`, `[ true_path, remote_ip, success, action_type ]`, `[ true_path, remote_ip, success, username ]`, `[ true_path, remote_ip, action_type, username ]`, `[ true_path, session_uuid, success, action_type ]`, `[ true_path, session_uuid, success, username ]`, `[ true_path, session_uuid, action_type, username ]`, `[ true_path, success, action_type, username ]`, `[ remote_ip, session_uuid, success, action_type ]`, `[ remote_ip, session_uuid, success, username ]`, `[ remote_ip, session_uuid, action_type, username ]`, `[ remote_ip, success, action_type, username ]`, `[ session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip ]`, `[ start_date, end_date, path, true_path, session_uuid ]`, `[ start_date, end_date, path, true_path, success ]`, `[ start_date, end_date, path, true_path, action_type ]`, `[ start_date, end_date, path, true_path, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid ]`, `[ start_date, end_date, path, remote_ip, success ]`, `[ start_date, end_date, path, remote_ip, action_type ]`, `[ start_date, end_date, path, remote_ip, username ]`, `[ start_date, end_date, path, session_uuid, success ]`, `[ start_date, end_date, path, session_uuid, action_type ]`, `[ start_date, end_date, path, session_uuid, username ]`, `[ start_date, end_date, path, success, action_type ]`, `[ start_date, end_date, path, success, username ]`, `[ start_date, end_date, path, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid ]`, `[ start_date, end_date, true_path, remote_ip, success ]`, `[ start_date, end_date, true_path, remote_ip, action_type ]`, `[ start_date, end_date, true_path, remote_ip, username ]`, `[ start_date, end_date, true_path, session_uuid, success ]`, `[ start_date, end_date, true_path, session_uuid, action_type ]`, `[ start_date, end_date, true_path, session_uuid, username ]`, `[ start_date, end_date, true_path, success, action_type ]`, `[ start_date, end_date, true_path, success, username ]`, `[ start_date, end_date, true_path, action_type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success ]`, `[ start_date, end_date, remote_ip, session_uuid, action_type ]`, `[ start_date, end_date, remote_ip, session_uuid, username ]`, `[ start_date, end_date, remote_ip, success, action_type ]`, `[ start_date, end_date, remote_ip, success, username ]`, `[ start_date, end_date, remote_ip, action_type, username ]`, `[ start_date, end_date, session_uuid, success, action_type ]`, `[ start_date, end_date, session_uuid, success, username ]`, `[ start_date, end_date, session_uuid, action_type, username ]`, `[ start_date, end_date, success, action_type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid ]`, `[ start_date, path, true_path, remote_ip, success ]`, `[ start_date, path, true_path, remote_ip, action_type ]`, `[ start_date, path, true_path, remote_ip, username ]`, `[ start_date, path, true_path, session_uuid, success ]`, `[ start_date, path, true_path, session_uuid, action_type ]`, `[ start_date, path, true_path, session_uuid, username ]`, `[ start_date, path, true_path, success, action_type ]`, `[ start_date, path, true_path, success, username ]`, `[ start_date, path, true_path, action_type, username ]`, `[ start_date, path, remote_ip, session_uuid, success ]`, `[ start_date, path, remote_ip, session_uuid, action_type ]`, `[ start_date, path, remote_ip, session_uuid, username ]`, `[ start_date, path, remote_ip, success, action_type ]`, `[ start_date, path, remote_ip, success, username ]`, `[ start_date, path, remote_ip, action_type, username ]`, `[ start_date, path, session_uuid, success, action_type ]`, `[ start_date, path, session_uuid, success, username ]`, `[ start_date, path, session_uuid, action_type, username ]`, `[ start_date, path, success, action_type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success ]`, `[ start_date, true_path, remote_ip, session_uuid, action_type ]`, `[ start_date, true_path, remote_ip, session_uuid, username ]`, `[ start_date, true_path, remote_ip, success, action_type ]`, `[ start_date, true_path, remote_ip, success, username ]`, `[ start_date, true_path, remote_ip, action_type, username ]`, `[ start_date, true_path, session_uuid, success, action_type ]`, `[ start_date, true_path, session_uuid, success, username ]`, `[ start_date, true_path, session_uuid, action_type, username ]`, `[ start_date, true_path, success, action_type, username ]`, `[ start_date, remote_ip, session_uuid, success, action_type ]`, `[ start_date, remote_ip, session_uuid, success, username ]`, `[ start_date, remote_ip, session_uuid, action_type, username ]`, `[ start_date, remote_ip, success, action_type, username ]`, `[ start_date, session_uuid, success, action_type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid ]`, `[ end_date, path, true_path, remote_ip, success ]`, `[ end_date, path, true_path, remote_ip, action_type ]`, `[ end_date, path, true_path, remote_ip, username ]`, `[ end_date, path, true_path, session_uuid, success ]`, `[ end_date, path, true_path, session_uuid, action_type ]`, `[ end_date, path, true_path, session_uuid, username ]`, `[ end_date, path, true_path, success, action_type ]`, `[ end_date, path, true_path, success, username ]`, `[ end_date, path, true_path, action_type, username ]`, `[ end_date, path, remote_ip, session_uuid, success ]`, `[ end_date, path, remote_ip, session_uuid, action_type ]`, `[ end_date, path, remote_ip, session_uuid, username ]`, `[ end_date, path, remote_ip, success, action_type ]`, `[ end_date, path, remote_ip, success, username ]`, `[ end_date, path, remote_ip, action_type, username ]`, `[ end_date, path, session_uuid, success, action_type ]`, `[ end_date, path, session_uuid, success, username ]`, `[ end_date, path, session_uuid, action_type, username ]`, `[ end_date, path, success, action_type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success ]`, `[ end_date, true_path, remote_ip, session_uuid, action_type ]`, `[ end_date, true_path, remote_ip, session_uuid, username ]`, `[ end_date, true_path, remote_ip, success, action_type ]`, `[ end_date, true_path, remote_ip, success, username ]`, `[ end_date, true_path, remote_ip, action_type, username ]`, `[ end_date, true_path, session_uuid, success, action_type ]`, `[ end_date, true_path, session_uuid, success, username ]`, `[ end_date, true_path, session_uuid, action_type, username ]`, `[ end_date, true_path, success, action_type, username ]`, `[ end_date, remote_ip, session_uuid, success, action_type ]`, `[ end_date, remote_ip, session_uuid, success, username ]`, `[ end_date, remote_ip, session_uuid, action_type, username ]`, `[ end_date, remote_ip, success, action_type, username ]`, `[ end_date, session_uuid, success, action_type, username ]`, `[ path, true_path, remote_ip, session_uuid, success ]`, `[ path, true_path, remote_ip, session_uuid, action_type ]`, `[ path, true_path, remote_ip, session_uuid, username ]`, `[ path, true_path, remote_ip, success, action_type ]`, `[ path, true_path, remote_ip, success, username ]`, `[ path, true_path, remote_ip, action_type, username ]`, `[ path, true_path, session_uuid, success, action_type ]`, `[ path, true_path, session_uuid, success, username ]`, `[ path, true_path, session_uuid, action_type, username ]`, `[ path, true_path, success, action_type, username ]`, `[ path, remote_ip, session_uuid, success, action_type ]`, `[ path, remote_ip, session_uuid, success, username ]`, `[ path, remote_ip, session_uuid, action_type, username ]`, `[ path, remote_ip, success, action_type, username ]`, `[ path, session_uuid, success, action_type, username ]`, `[ true_path, remote_ip, session_uuid, success, action_type ]`, `[ true_path, remote_ip, session_uuid, success, username ]`, `[ true_path, remote_ip, session_uuid, action_type, username ]`, `[ true_path, remote_ip, success, action_type, username ]`, `[ true_path, session_uuid, success, action_type, username ]`, `[ remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid ]`, `[ start_date, end_date, path, true_path, remote_ip, success ]`, `[ start_date, end_date, path, true_path, remote_ip, action_type ]`, `[ start_date, end_date, path, true_path, remote_ip, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success ]`, `[ start_date, end_date, path, true_path, session_uuid, action_type ]`, `[ start_date, end_date, path, true_path, session_uuid, username ]`, `[ start_date, end_date, path, true_path, success, action_type ]`, `[ start_date, end_date, path, true_path, success, username ]`, `[ start_date, end_date, path, true_path, action_type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, path, remote_ip, session_uuid, action_type ]`, `[ start_date, end_date, path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, path, remote_ip, success, action_type ]`, `[ start_date, end_date, path, remote_ip, success, username ]`, `[ start_date, end_date, path, remote_ip, action_type, username ]`, `[ start_date, end_date, path, session_uuid, success, action_type ]`, `[ start_date, end_date, path, session_uuid, success, username ]`, `[ start_date, end_date, path, session_uuid, action_type, username ]`, `[ start_date, end_date, path, success, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, action_type ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, true_path, remote_ip, success, action_type ]`, `[ start_date, end_date, true_path, remote_ip, success, username ]`, `[ start_date, end_date, true_path, remote_ip, action_type, username ]`, `[ start_date, end_date, true_path, session_uuid, success, action_type ]`, `[ start_date, end_date, true_path, session_uuid, success, username ]`, `[ start_date, end_date, true_path, session_uuid, action_type, username ]`, `[ start_date, end_date, true_path, success, action_type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success, action_type ]`, `[ start_date, end_date, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, remote_ip, session_uuid, action_type, username ]`, `[ start_date, end_date, remote_ip, success, action_type, username ]`, `[ start_date, end_date, session_uuid, success, action_type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success ]`, `[ start_date, path, true_path, remote_ip, session_uuid, action_type ]`, `[ start_date, path, true_path, remote_ip, session_uuid, username ]`, `[ start_date, path, true_path, remote_ip, success, action_type ]`, `[ start_date, path, true_path, remote_ip, success, username ]`, `[ start_date, path, true_path, remote_ip, action_type, username ]`, `[ start_date, path, true_path, session_uuid, success, action_type ]`, `[ start_date, path, true_path, session_uuid, success, username ]`, `[ start_date, path, true_path, session_uuid, action_type, username ]`, `[ start_date, path, true_path, success, action_type, username ]`, `[ start_date, path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, path, remote_ip, session_uuid, success, username ]`, `[ start_date, path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, path, remote_ip, success, action_type, username ]`, `[ start_date, path, session_uuid, success, action_type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, true_path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, true_path, remote_ip, success, action_type, username ]`, `[ start_date, true_path, session_uuid, success, action_type, username ]`, `[ start_date, remote_ip, session_uuid, success, action_type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success ]`, `[ end_date, path, true_path, remote_ip, session_uuid, action_type ]`, `[ end_date, path, true_path, remote_ip, session_uuid, username ]`, `[ end_date, path, true_path, remote_ip, success, action_type ]`, `[ end_date, path, true_path, remote_ip, success, username ]`, `[ end_date, path, true_path, remote_ip, action_type, username ]`, `[ end_date, path, true_path, session_uuid, success, action_type ]`, `[ end_date, path, true_path, session_uuid, success, username ]`, `[ end_date, path, true_path, session_uuid, action_type, username ]`, `[ end_date, path, true_path, success, action_type, username ]`, `[ end_date, path, remote_ip, session_uuid, success, action_type ]`, `[ end_date, path, remote_ip, session_uuid, success, username ]`, `[ end_date, path, remote_ip, session_uuid, action_type, username ]`, `[ end_date, path, remote_ip, success, action_type, username ]`, `[ end_date, path, session_uuid, success, action_type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success, action_type ]`, `[ end_date, true_path, remote_ip, session_uuid, success, username ]`, `[ end_date, true_path, remote_ip, session_uuid, action_type, username ]`, `[ end_date, true_path, remote_ip, success, action_type, username ]`, `[ end_date, true_path, session_uuid, success, action_type, username ]`, `[ end_date, remote_ip, session_uuid, success, action_type, username ]`, `[ path, true_path, remote_ip, session_uuid, success, action_type ]`, `[ path, true_path, remote_ip, session_uuid, success, username ]`, `[ path, true_path, remote_ip, session_uuid, action_type, username ]`, `[ path, true_path, remote_ip, success, action_type, username ]`, `[ path, true_path, session_uuid, success, action_type, username ]`, `[ path, remote_ip, session_uuid, success, action_type, username ]`, `[ true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, action_type ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, path, true_path, remote_ip, success, action_type ]`, `[ start_date, end_date, path, true_path, remote_ip, success, username ]`, `[ start_date, end_date, path, true_path, remote_ip, action_type, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success, action_type ]`, `[ start_date, end_date, path, true_path, session_uuid, success, username ]`, `[ start_date, end_date, path, true_path, session_uuid, action_type, username ]`, `[ start_date, end_date, path, true_path, success, action_type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, end_date, path, remote_ip, success, action_type, username ]`, `[ start_date, end_date, path, session_uuid, success, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, success, action_type, username ]`, `[ start_date, end_date, true_path, session_uuid, success, action_type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, path, true_path, remote_ip, success, action_type, username ]`, `[ start_date, path, true_path, session_uuid, success, action_type, username ]`, `[ start_date, path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success, action_type ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, action_type, username ]`, `[ end_date, path, true_path, remote_ip, success, action_type, username ]`, `[ end_date, path, true_path, session_uuid, success, action_type, username ]`, `[ end_date, path, remote_ip, session_uuid, success, action_type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ path, true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success, action_type ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, action_type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, success, action_type, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success, action_type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, action_type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, action_type, username ]` or `[ end_date, path, true_path, remote_ip, session_uuid, success, action_type, username ]`.
        /// </summary>
        public static FilesList<FtpActionLog> List(

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
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }

            return new FilesList<FtpActionLog>($"/ftp_action_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<FtpActionLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}