using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class SftpActionLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public SftpActionLog() : this(null, null) { }

        public SftpActionLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("user"))
            {
                this.attributes.Add("user", null);
            }
            if (!this.attributes.ContainsKey("session_uid"))
            {
                this.attributes.Add("session_uid", null);
            }
            if (!this.attributes.ContainsKey("seq_id"))
            {
                this.attributes.Add("seq_id", null);
            }
            if (!this.attributes.ContainsKey("auth_method"))
            {
                this.attributes.Add("auth_method", null);
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
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("failure_reason_type"))
            {
                this.attributes.Add("failure_reason_type", null);
            }
            if (!this.attributes.ContainsKey("failure_reason_message"))
            {
                this.attributes.Add("failure_reason_message", null);
            }
            if (!this.attributes.ContainsKey("md5"))
            {
                this.attributes.Add("md5", null);
            }
            if (!this.attributes.ContainsKey("flags"))
            {
                this.attributes.Add("flags", null);
            }
            if (!this.attributes.ContainsKey("handle"))
            {
                this.attributes.Add("handle", null);
            }
            if (!this.attributes.ContainsKey("attrs"))
            {
                this.attributes.Add("attrs", null);
            }
            if (!this.attributes.ContainsKey("size"))
            {
                this.attributes.Add("size", null);
            }
            if (!this.attributes.ContainsKey("offset"))
            {
                this.attributes.Add("offset", null);
            }
            if (!this.attributes.ContainsKey("length"))
            {
                this.attributes.Add("length", null);
            }
            if (!this.attributes.ContainsKey("data_length"))
            {
                this.attributes.Add("data_length", null);
            }
            if (!this.attributes.ContainsKey("success"))
            {
                this.attributes.Add("success", null);
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
        /// IP Address of SFTP Client
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("remote_ip")]
        public string RemoteIp
        {
            get { return (string)attributes["remote_ip"]; }
            private set { attributes["remote_ip"] = value; }
        }

        /// <summary>
        /// Username
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user")]
        public string User
        {
            get { return (string)attributes["user"]; }
            private set { attributes["user"] = value; }
        }

        /// <summary>
        /// Unique ID of the Session
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("session_uid")]
        public string SessionUid
        {
            get { return (string)attributes["session_uid"]; }
            private set { attributes["session_uid"] = value; }
        }

        /// <summary>
        /// SFTP Sequence ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("seq_id")]
        public Nullable<Int64> SeqId
        {
            get { return (Nullable<Int64>)attributes["seq_id"]; }
            private set { attributes["seq_id"] = value; }
        }

        /// <summary>
        /// Authentication Method
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("auth_method")]
        public string AuthMethod
        {
            get { return (string)attributes["auth_method"]; }
            private set { attributes["auth_method"] = value; }
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
        /// Path as sent by the Client (may not match Files.com path due to user root folders for SFTP) This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
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
        /// Log Message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            private set { attributes["message"] = value; }
        }

        /// <summary>
        /// Failure Reason Type
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failure_reason_type")]
        public string FailureReasonType
        {
            get { return (string)attributes["failure_reason_type"]; }
            private set { attributes["failure_reason_type"] = value; }
        }

        /// <summary>
        /// Failure Reason Message
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failure_reason_message")]
        public string FailureReasonMessage
        {
            get { return (string)attributes["failure_reason_message"]; }
            private set { attributes["failure_reason_message"] = value; }
        }

        /// <summary>
        /// MD5 Hash of the file. Not always available.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("md5")]
        public string Md5
        {
            get { return (string)attributes["md5"]; }
            private set { attributes["md5"] = value; }
        }

        /// <summary>
        /// SFTP Command Flags. Present for actions which involve flags.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("flags")]
        public string Flags
        {
            get { return (string)attributes["flags"]; }
            private set { attributes["flags"] = value; }
        }

        /// <summary>
        /// SFTP Command Handle. Present for actions which involve handles.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("handle")]
        public string Handle
        {
            get { return (string)attributes["handle"]; }
            private set { attributes["handle"] = value; }
        }

        /// <summary>
        /// SFTP Command Attrs. Present for action which involve attrs.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("attrs")]
        public string Attrs
        {
            get { return (string)attributes["attrs"]; }
            private set { attributes["attrs"] = value; }
        }

        /// <summary>
        /// Size. Present for action which involve size.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("size")]
        public string Size
        {
            get { return (string)attributes["size"]; }
            private set { attributes["size"] = value; }
        }

        /// <summary>
        /// Offset. Present for actions which involve offset.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("offset")]
        public string Offset
        {
            get { return (string)attributes["offset"]; }
            private set { attributes["offset"] = value; }
        }

        /// <summary>
        /// Length. Present for actions which involve length.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("length")]
        public string Length
        {
            get { return (string)attributes["length"]; }
            private set { attributes["length"] = value; }
        }

        /// <summary>
        /// Data Length in Bytes. Present for actions that transfer data.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("data_length")]
        public string DataLength
        {
            get { return (string)attributes["data_length"]; }
            private set { attributes["data_length"] = value; }
        }

        /// <summary>
        /// Whether SFTP Action was successful.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("success")]
        public string Success
        {
            get { return (string)attributes["success"]; }
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
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date`, `path`, `true_path`, `remote_ip`, `session_uuid`, `success`, `type` or `username`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ path ]`, `[ true_path ]`, `[ remote_ip ]`, `[ session_uuid ]`, `[ success ]`, `[ type ]`, `[ username ]`, `[ start_date, end_date ]`, `[ start_date, path ]`, `[ start_date, true_path ]`, `[ start_date, remote_ip ]`, `[ start_date, session_uuid ]`, `[ start_date, success ]`, `[ start_date, type ]`, `[ start_date, username ]`, `[ end_date, path ]`, `[ end_date, true_path ]`, `[ end_date, remote_ip ]`, `[ end_date, session_uuid ]`, `[ end_date, success ]`, `[ end_date, type ]`, `[ end_date, username ]`, `[ path, true_path ]`, `[ path, remote_ip ]`, `[ path, session_uuid ]`, `[ path, success ]`, `[ path, type ]`, `[ path, username ]`, `[ true_path, remote_ip ]`, `[ true_path, session_uuid ]`, `[ true_path, success ]`, `[ true_path, type ]`, `[ true_path, username ]`, `[ remote_ip, session_uuid ]`, `[ remote_ip, success ]`, `[ remote_ip, type ]`, `[ remote_ip, username ]`, `[ session_uuid, success ]`, `[ session_uuid, type ]`, `[ session_uuid, username ]`, `[ success, type ]`, `[ success, username ]`, `[ type, username ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, true_path ]`, `[ start_date, end_date, remote_ip ]`, `[ start_date, end_date, session_uuid ]`, `[ start_date, end_date, success ]`, `[ start_date, end_date, type ]`, `[ start_date, end_date, username ]`, `[ start_date, path, true_path ]`, `[ start_date, path, remote_ip ]`, `[ start_date, path, session_uuid ]`, `[ start_date, path, success ]`, `[ start_date, path, type ]`, `[ start_date, path, username ]`, `[ start_date, true_path, remote_ip ]`, `[ start_date, true_path, session_uuid ]`, `[ start_date, true_path, success ]`, `[ start_date, true_path, type ]`, `[ start_date, true_path, username ]`, `[ start_date, remote_ip, session_uuid ]`, `[ start_date, remote_ip, success ]`, `[ start_date, remote_ip, type ]`, `[ start_date, remote_ip, username ]`, `[ start_date, session_uuid, success ]`, `[ start_date, session_uuid, type ]`, `[ start_date, session_uuid, username ]`, `[ start_date, success, type ]`, `[ start_date, success, username ]`, `[ start_date, type, username ]`, `[ end_date, path, true_path ]`, `[ end_date, path, remote_ip ]`, `[ end_date, path, session_uuid ]`, `[ end_date, path, success ]`, `[ end_date, path, type ]`, `[ end_date, path, username ]`, `[ end_date, true_path, remote_ip ]`, `[ end_date, true_path, session_uuid ]`, `[ end_date, true_path, success ]`, `[ end_date, true_path, type ]`, `[ end_date, true_path, username ]`, `[ end_date, remote_ip, session_uuid ]`, `[ end_date, remote_ip, success ]`, `[ end_date, remote_ip, type ]`, `[ end_date, remote_ip, username ]`, `[ end_date, session_uuid, success ]`, `[ end_date, session_uuid, type ]`, `[ end_date, session_uuid, username ]`, `[ end_date, success, type ]`, `[ end_date, success, username ]`, `[ end_date, type, username ]`, `[ path, true_path, remote_ip ]`, `[ path, true_path, session_uuid ]`, `[ path, true_path, success ]`, `[ path, true_path, type ]`, `[ path, true_path, username ]`, `[ path, remote_ip, session_uuid ]`, `[ path, remote_ip, success ]`, `[ path, remote_ip, type ]`, `[ path, remote_ip, username ]`, `[ path, session_uuid, success ]`, `[ path, session_uuid, type ]`, `[ path, session_uuid, username ]`, `[ path, success, type ]`, `[ path, success, username ]`, `[ path, type, username ]`, `[ true_path, remote_ip, session_uuid ]`, `[ true_path, remote_ip, success ]`, `[ true_path, remote_ip, type ]`, `[ true_path, remote_ip, username ]`, `[ true_path, session_uuid, success ]`, `[ true_path, session_uuid, type ]`, `[ true_path, session_uuid, username ]`, `[ true_path, success, type ]`, `[ true_path, success, username ]`, `[ true_path, type, username ]`, `[ remote_ip, session_uuid, success ]`, `[ remote_ip, session_uuid, type ]`, `[ remote_ip, session_uuid, username ]`, `[ remote_ip, success, type ]`, `[ remote_ip, success, username ]`, `[ remote_ip, type, username ]`, `[ session_uuid, success, type ]`, `[ session_uuid, success, username ]`, `[ session_uuid, type, username ]`, `[ success, type, username ]`, `[ start_date, end_date, path, true_path ]`, `[ start_date, end_date, path, remote_ip ]`, `[ start_date, end_date, path, session_uuid ]`, `[ start_date, end_date, path, success ]`, `[ start_date, end_date, path, type ]`, `[ start_date, end_date, path, username ]`, `[ start_date, end_date, true_path, remote_ip ]`, `[ start_date, end_date, true_path, session_uuid ]`, `[ start_date, end_date, true_path, success ]`, `[ start_date, end_date, true_path, type ]`, `[ start_date, end_date, true_path, username ]`, `[ start_date, end_date, remote_ip, session_uuid ]`, `[ start_date, end_date, remote_ip, success ]`, `[ start_date, end_date, remote_ip, type ]`, `[ start_date, end_date, remote_ip, username ]`, `[ start_date, end_date, session_uuid, success ]`, `[ start_date, end_date, session_uuid, type ]`, `[ start_date, end_date, session_uuid, username ]`, `[ start_date, end_date, success, type ]`, `[ start_date, end_date, success, username ]`, `[ start_date, end_date, type, username ]`, `[ start_date, path, true_path, remote_ip ]`, `[ start_date, path, true_path, session_uuid ]`, `[ start_date, path, true_path, success ]`, `[ start_date, path, true_path, type ]`, `[ start_date, path, true_path, username ]`, `[ start_date, path, remote_ip, session_uuid ]`, `[ start_date, path, remote_ip, success ]`, `[ start_date, path, remote_ip, type ]`, `[ start_date, path, remote_ip, username ]`, `[ start_date, path, session_uuid, success ]`, `[ start_date, path, session_uuid, type ]`, `[ start_date, path, session_uuid, username ]`, `[ start_date, path, success, type ]`, `[ start_date, path, success, username ]`, `[ start_date, path, type, username ]`, `[ start_date, true_path, remote_ip, session_uuid ]`, `[ start_date, true_path, remote_ip, success ]`, `[ start_date, true_path, remote_ip, type ]`, `[ start_date, true_path, remote_ip, username ]`, `[ start_date, true_path, session_uuid, success ]`, `[ start_date, true_path, session_uuid, type ]`, `[ start_date, true_path, session_uuid, username ]`, `[ start_date, true_path, success, type ]`, `[ start_date, true_path, success, username ]`, `[ start_date, true_path, type, username ]`, `[ start_date, remote_ip, session_uuid, success ]`, `[ start_date, remote_ip, session_uuid, type ]`, `[ start_date, remote_ip, session_uuid, username ]`, `[ start_date, remote_ip, success, type ]`, `[ start_date, remote_ip, success, username ]`, `[ start_date, remote_ip, type, username ]`, `[ start_date, session_uuid, success, type ]`, `[ start_date, session_uuid, success, username ]`, `[ start_date, session_uuid, type, username ]`, `[ start_date, success, type, username ]`, `[ end_date, path, true_path, remote_ip ]`, `[ end_date, path, true_path, session_uuid ]`, `[ end_date, path, true_path, success ]`, `[ end_date, path, true_path, type ]`, `[ end_date, path, true_path, username ]`, `[ end_date, path, remote_ip, session_uuid ]`, `[ end_date, path, remote_ip, success ]`, `[ end_date, path, remote_ip, type ]`, `[ end_date, path, remote_ip, username ]`, `[ end_date, path, session_uuid, success ]`, `[ end_date, path, session_uuid, type ]`, `[ end_date, path, session_uuid, username ]`, `[ end_date, path, success, type ]`, `[ end_date, path, success, username ]`, `[ end_date, path, type, username ]`, `[ end_date, true_path, remote_ip, session_uuid ]`, `[ end_date, true_path, remote_ip, success ]`, `[ end_date, true_path, remote_ip, type ]`, `[ end_date, true_path, remote_ip, username ]`, `[ end_date, true_path, session_uuid, success ]`, `[ end_date, true_path, session_uuid, type ]`, `[ end_date, true_path, session_uuid, username ]`, `[ end_date, true_path, success, type ]`, `[ end_date, true_path, success, username ]`, `[ end_date, true_path, type, username ]`, `[ end_date, remote_ip, session_uuid, success ]`, `[ end_date, remote_ip, session_uuid, type ]`, `[ end_date, remote_ip, session_uuid, username ]`, `[ end_date, remote_ip, success, type ]`, `[ end_date, remote_ip, success, username ]`, `[ end_date, remote_ip, type, username ]`, `[ end_date, session_uuid, success, type ]`, `[ end_date, session_uuid, success, username ]`, `[ end_date, session_uuid, type, username ]`, `[ end_date, success, type, username ]`, `[ path, true_path, remote_ip, session_uuid ]`, `[ path, true_path, remote_ip, success ]`, `[ path, true_path, remote_ip, type ]`, `[ path, true_path, remote_ip, username ]`, `[ path, true_path, session_uuid, success ]`, `[ path, true_path, session_uuid, type ]`, `[ path, true_path, session_uuid, username ]`, `[ path, true_path, success, type ]`, `[ path, true_path, success, username ]`, `[ path, true_path, type, username ]`, `[ path, remote_ip, session_uuid, success ]`, `[ path, remote_ip, session_uuid, type ]`, `[ path, remote_ip, session_uuid, username ]`, `[ path, remote_ip, success, type ]`, `[ path, remote_ip, success, username ]`, `[ path, remote_ip, type, username ]`, `[ path, session_uuid, success, type ]`, `[ path, session_uuid, success, username ]`, `[ path, session_uuid, type, username ]`, `[ path, success, type, username ]`, `[ true_path, remote_ip, session_uuid, success ]`, `[ true_path, remote_ip, session_uuid, type ]`, `[ true_path, remote_ip, session_uuid, username ]`, `[ true_path, remote_ip, success, type ]`, `[ true_path, remote_ip, success, username ]`, `[ true_path, remote_ip, type, username ]`, `[ true_path, session_uuid, success, type ]`, `[ true_path, session_uuid, success, username ]`, `[ true_path, session_uuid, type, username ]`, `[ true_path, success, type, username ]`, `[ remote_ip, session_uuid, success, type ]`, `[ remote_ip, session_uuid, success, username ]`, `[ remote_ip, session_uuid, type, username ]`, `[ remote_ip, success, type, username ]`, `[ session_uuid, success, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip ]`, `[ start_date, end_date, path, true_path, session_uuid ]`, `[ start_date, end_date, path, true_path, success ]`, `[ start_date, end_date, path, true_path, type ]`, `[ start_date, end_date, path, true_path, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid ]`, `[ start_date, end_date, path, remote_ip, success ]`, `[ start_date, end_date, path, remote_ip, type ]`, `[ start_date, end_date, path, remote_ip, username ]`, `[ start_date, end_date, path, session_uuid, success ]`, `[ start_date, end_date, path, session_uuid, type ]`, `[ start_date, end_date, path, session_uuid, username ]`, `[ start_date, end_date, path, success, type ]`, `[ start_date, end_date, path, success, username ]`, `[ start_date, end_date, path, type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid ]`, `[ start_date, end_date, true_path, remote_ip, success ]`, `[ start_date, end_date, true_path, remote_ip, type ]`, `[ start_date, end_date, true_path, remote_ip, username ]`, `[ start_date, end_date, true_path, session_uuid, success ]`, `[ start_date, end_date, true_path, session_uuid, type ]`, `[ start_date, end_date, true_path, session_uuid, username ]`, `[ start_date, end_date, true_path, success, type ]`, `[ start_date, end_date, true_path, success, username ]`, `[ start_date, end_date, true_path, type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success ]`, `[ start_date, end_date, remote_ip, session_uuid, type ]`, `[ start_date, end_date, remote_ip, session_uuid, username ]`, `[ start_date, end_date, remote_ip, success, type ]`, `[ start_date, end_date, remote_ip, success, username ]`, `[ start_date, end_date, remote_ip, type, username ]`, `[ start_date, end_date, session_uuid, success, type ]`, `[ start_date, end_date, session_uuid, success, username ]`, `[ start_date, end_date, session_uuid, type, username ]`, `[ start_date, end_date, success, type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid ]`, `[ start_date, path, true_path, remote_ip, success ]`, `[ start_date, path, true_path, remote_ip, type ]`, `[ start_date, path, true_path, remote_ip, username ]`, `[ start_date, path, true_path, session_uuid, success ]`, `[ start_date, path, true_path, session_uuid, type ]`, `[ start_date, path, true_path, session_uuid, username ]`, `[ start_date, path, true_path, success, type ]`, `[ start_date, path, true_path, success, username ]`, `[ start_date, path, true_path, type, username ]`, `[ start_date, path, remote_ip, session_uuid, success ]`, `[ start_date, path, remote_ip, session_uuid, type ]`, `[ start_date, path, remote_ip, session_uuid, username ]`, `[ start_date, path, remote_ip, success, type ]`, `[ start_date, path, remote_ip, success, username ]`, `[ start_date, path, remote_ip, type, username ]`, `[ start_date, path, session_uuid, success, type ]`, `[ start_date, path, session_uuid, success, username ]`, `[ start_date, path, session_uuid, type, username ]`, `[ start_date, path, success, type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success ]`, `[ start_date, true_path, remote_ip, session_uuid, type ]`, `[ start_date, true_path, remote_ip, session_uuid, username ]`, `[ start_date, true_path, remote_ip, success, type ]`, `[ start_date, true_path, remote_ip, success, username ]`, `[ start_date, true_path, remote_ip, type, username ]`, `[ start_date, true_path, session_uuid, success, type ]`, `[ start_date, true_path, session_uuid, success, username ]`, `[ start_date, true_path, session_uuid, type, username ]`, `[ start_date, true_path, success, type, username ]`, `[ start_date, remote_ip, session_uuid, success, type ]`, `[ start_date, remote_ip, session_uuid, success, username ]`, `[ start_date, remote_ip, session_uuid, type, username ]`, `[ start_date, remote_ip, success, type, username ]`, `[ start_date, session_uuid, success, type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid ]`, `[ end_date, path, true_path, remote_ip, success ]`, `[ end_date, path, true_path, remote_ip, type ]`, `[ end_date, path, true_path, remote_ip, username ]`, `[ end_date, path, true_path, session_uuid, success ]`, `[ end_date, path, true_path, session_uuid, type ]`, `[ end_date, path, true_path, session_uuid, username ]`, `[ end_date, path, true_path, success, type ]`, `[ end_date, path, true_path, success, username ]`, `[ end_date, path, true_path, type, username ]`, `[ end_date, path, remote_ip, session_uuid, success ]`, `[ end_date, path, remote_ip, session_uuid, type ]`, `[ end_date, path, remote_ip, session_uuid, username ]`, `[ end_date, path, remote_ip, success, type ]`, `[ end_date, path, remote_ip, success, username ]`, `[ end_date, path, remote_ip, type, username ]`, `[ end_date, path, session_uuid, success, type ]`, `[ end_date, path, session_uuid, success, username ]`, `[ end_date, path, session_uuid, type, username ]`, `[ end_date, path, success, type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success ]`, `[ end_date, true_path, remote_ip, session_uuid, type ]`, `[ end_date, true_path, remote_ip, session_uuid, username ]`, `[ end_date, true_path, remote_ip, success, type ]`, `[ end_date, true_path, remote_ip, success, username ]`, `[ end_date, true_path, remote_ip, type, username ]`, `[ end_date, true_path, session_uuid, success, type ]`, `[ end_date, true_path, session_uuid, success, username ]`, `[ end_date, true_path, session_uuid, type, username ]`, `[ end_date, true_path, success, type, username ]`, `[ end_date, remote_ip, session_uuid, success, type ]`, `[ end_date, remote_ip, session_uuid, success, username ]`, `[ end_date, remote_ip, session_uuid, type, username ]`, `[ end_date, remote_ip, success, type, username ]`, `[ end_date, session_uuid, success, type, username ]`, `[ path, true_path, remote_ip, session_uuid, success ]`, `[ path, true_path, remote_ip, session_uuid, type ]`, `[ path, true_path, remote_ip, session_uuid, username ]`, `[ path, true_path, remote_ip, success, type ]`, `[ path, true_path, remote_ip, success, username ]`, `[ path, true_path, remote_ip, type, username ]`, `[ path, true_path, session_uuid, success, type ]`, `[ path, true_path, session_uuid, success, username ]`, `[ path, true_path, session_uuid, type, username ]`, `[ path, true_path, success, type, username ]`, `[ path, remote_ip, session_uuid, success, type ]`, `[ path, remote_ip, session_uuid, success, username ]`, `[ path, remote_ip, session_uuid, type, username ]`, `[ path, remote_ip, success, type, username ]`, `[ path, session_uuid, success, type, username ]`, `[ true_path, remote_ip, session_uuid, success, type ]`, `[ true_path, remote_ip, session_uuid, success, username ]`, `[ true_path, remote_ip, session_uuid, type, username ]`, `[ true_path, remote_ip, success, type, username ]`, `[ true_path, session_uuid, success, type, username ]`, `[ remote_ip, session_uuid, success, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid ]`, `[ start_date, end_date, path, true_path, remote_ip, success ]`, `[ start_date, end_date, path, true_path, remote_ip, type ]`, `[ start_date, end_date, path, true_path, remote_ip, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success ]`, `[ start_date, end_date, path, true_path, session_uuid, type ]`, `[ start_date, end_date, path, true_path, session_uuid, username ]`, `[ start_date, end_date, path, true_path, success, type ]`, `[ start_date, end_date, path, true_path, success, username ]`, `[ start_date, end_date, path, true_path, type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, path, remote_ip, session_uuid, type ]`, `[ start_date, end_date, path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, path, remote_ip, success, type ]`, `[ start_date, end_date, path, remote_ip, success, username ]`, `[ start_date, end_date, path, remote_ip, type, username ]`, `[ start_date, end_date, path, session_uuid, success, type ]`, `[ start_date, end_date, path, session_uuid, success, username ]`, `[ start_date, end_date, path, session_uuid, type, username ]`, `[ start_date, end_date, path, success, type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, type ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, true_path, remote_ip, success, type ]`, `[ start_date, end_date, true_path, remote_ip, success, username ]`, `[ start_date, end_date, true_path, remote_ip, type, username ]`, `[ start_date, end_date, true_path, session_uuid, success, type ]`, `[ start_date, end_date, true_path, session_uuid, success, username ]`, `[ start_date, end_date, true_path, session_uuid, type, username ]`, `[ start_date, end_date, true_path, success, type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success, type ]`, `[ start_date, end_date, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, remote_ip, session_uuid, type, username ]`, `[ start_date, end_date, remote_ip, success, type, username ]`, `[ start_date, end_date, session_uuid, success, type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success ]`, `[ start_date, path, true_path, remote_ip, session_uuid, type ]`, `[ start_date, path, true_path, remote_ip, session_uuid, username ]`, `[ start_date, path, true_path, remote_ip, success, type ]`, `[ start_date, path, true_path, remote_ip, success, username ]`, `[ start_date, path, true_path, remote_ip, type, username ]`, `[ start_date, path, true_path, session_uuid, success, type ]`, `[ start_date, path, true_path, session_uuid, success, username ]`, `[ start_date, path, true_path, session_uuid, type, username ]`, `[ start_date, path, true_path, success, type, username ]`, `[ start_date, path, remote_ip, session_uuid, success, type ]`, `[ start_date, path, remote_ip, session_uuid, success, username ]`, `[ start_date, path, remote_ip, session_uuid, type, username ]`, `[ start_date, path, remote_ip, success, type, username ]`, `[ start_date, path, session_uuid, success, type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success, type ]`, `[ start_date, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, true_path, remote_ip, session_uuid, type, username ]`, `[ start_date, true_path, remote_ip, success, type, username ]`, `[ start_date, true_path, session_uuid, success, type, username ]`, `[ start_date, remote_ip, session_uuid, success, type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success ]`, `[ end_date, path, true_path, remote_ip, session_uuid, type ]`, `[ end_date, path, true_path, remote_ip, session_uuid, username ]`, `[ end_date, path, true_path, remote_ip, success, type ]`, `[ end_date, path, true_path, remote_ip, success, username ]`, `[ end_date, path, true_path, remote_ip, type, username ]`, `[ end_date, path, true_path, session_uuid, success, type ]`, `[ end_date, path, true_path, session_uuid, success, username ]`, `[ end_date, path, true_path, session_uuid, type, username ]`, `[ end_date, path, true_path, success, type, username ]`, `[ end_date, path, remote_ip, session_uuid, success, type ]`, `[ end_date, path, remote_ip, session_uuid, success, username ]`, `[ end_date, path, remote_ip, session_uuid, type, username ]`, `[ end_date, path, remote_ip, success, type, username ]`, `[ end_date, path, session_uuid, success, type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success, type ]`, `[ end_date, true_path, remote_ip, session_uuid, success, username ]`, `[ end_date, true_path, remote_ip, session_uuid, type, username ]`, `[ end_date, true_path, remote_ip, success, type, username ]`, `[ end_date, true_path, session_uuid, success, type, username ]`, `[ end_date, remote_ip, session_uuid, success, type, username ]`, `[ path, true_path, remote_ip, session_uuid, success, type ]`, `[ path, true_path, remote_ip, session_uuid, success, username ]`, `[ path, true_path, remote_ip, session_uuid, type, username ]`, `[ path, true_path, remote_ip, success, type, username ]`, `[ path, true_path, session_uuid, success, type, username ]`, `[ path, remote_ip, session_uuid, success, type, username ]`, `[ true_path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, type ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, path, true_path, remote_ip, success, type ]`, `[ start_date, end_date, path, true_path, remote_ip, success, username ]`, `[ start_date, end_date, path, true_path, remote_ip, type, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success, type ]`, `[ start_date, end_date, path, true_path, session_uuid, success, username ]`, `[ start_date, end_date, path, true_path, session_uuid, type, username ]`, `[ start_date, end_date, path, true_path, success, type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, type ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, type, username ]`, `[ start_date, end_date, path, remote_ip, success, type, username ]`, `[ start_date, end_date, path, session_uuid, success, type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, type ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, type, username ]`, `[ start_date, end_date, true_path, remote_ip, success, type, username ]`, `[ start_date, end_date, true_path, session_uuid, success, type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success, type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, type ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, type, username ]`, `[ start_date, path, true_path, remote_ip, success, type, username ]`, `[ start_date, path, true_path, session_uuid, success, type, username ]`, `[ start_date, path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success, type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success, type ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, type, username ]`, `[ end_date, path, true_path, remote_ip, success, type, username ]`, `[ end_date, path, true_path, session_uuid, success, type, username ]`, `[ end_date, path, remote_ip, session_uuid, success, type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success, type, username ]`, `[ path, true_path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success, type ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, success, type, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success, type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, type, username ]` or `[ end_date, path, true_path, remote_ip, session_uuid, success, type, username ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`, `true_path` or `success`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ path ]`, `[ true_path ]`, `[ remote_ip ]`, `[ session_uuid ]`, `[ success ]`, `[ type ]`, `[ username ]`, `[ start_date, end_date ]`, `[ start_date, path ]`, `[ start_date, true_path ]`, `[ start_date, remote_ip ]`, `[ start_date, session_uuid ]`, `[ start_date, success ]`, `[ start_date, type ]`, `[ start_date, username ]`, `[ end_date, path ]`, `[ end_date, true_path ]`, `[ end_date, remote_ip ]`, `[ end_date, session_uuid ]`, `[ end_date, success ]`, `[ end_date, type ]`, `[ end_date, username ]`, `[ path, true_path ]`, `[ path, remote_ip ]`, `[ path, session_uuid ]`, `[ path, success ]`, `[ path, type ]`, `[ path, username ]`, `[ true_path, remote_ip ]`, `[ true_path, session_uuid ]`, `[ true_path, success ]`, `[ true_path, type ]`, `[ true_path, username ]`, `[ remote_ip, session_uuid ]`, `[ remote_ip, success ]`, `[ remote_ip, type ]`, `[ remote_ip, username ]`, `[ session_uuid, success ]`, `[ session_uuid, type ]`, `[ session_uuid, username ]`, `[ success, type ]`, `[ success, username ]`, `[ type, username ]`, `[ start_date, end_date, path ]`, `[ start_date, end_date, true_path ]`, `[ start_date, end_date, remote_ip ]`, `[ start_date, end_date, session_uuid ]`, `[ start_date, end_date, success ]`, `[ start_date, end_date, type ]`, `[ start_date, end_date, username ]`, `[ start_date, path, true_path ]`, `[ start_date, path, remote_ip ]`, `[ start_date, path, session_uuid ]`, `[ start_date, path, success ]`, `[ start_date, path, type ]`, `[ start_date, path, username ]`, `[ start_date, true_path, remote_ip ]`, `[ start_date, true_path, session_uuid ]`, `[ start_date, true_path, success ]`, `[ start_date, true_path, type ]`, `[ start_date, true_path, username ]`, `[ start_date, remote_ip, session_uuid ]`, `[ start_date, remote_ip, success ]`, `[ start_date, remote_ip, type ]`, `[ start_date, remote_ip, username ]`, `[ start_date, session_uuid, success ]`, `[ start_date, session_uuid, type ]`, `[ start_date, session_uuid, username ]`, `[ start_date, success, type ]`, `[ start_date, success, username ]`, `[ start_date, type, username ]`, `[ end_date, path, true_path ]`, `[ end_date, path, remote_ip ]`, `[ end_date, path, session_uuid ]`, `[ end_date, path, success ]`, `[ end_date, path, type ]`, `[ end_date, path, username ]`, `[ end_date, true_path, remote_ip ]`, `[ end_date, true_path, session_uuid ]`, `[ end_date, true_path, success ]`, `[ end_date, true_path, type ]`, `[ end_date, true_path, username ]`, `[ end_date, remote_ip, session_uuid ]`, `[ end_date, remote_ip, success ]`, `[ end_date, remote_ip, type ]`, `[ end_date, remote_ip, username ]`, `[ end_date, session_uuid, success ]`, `[ end_date, session_uuid, type ]`, `[ end_date, session_uuid, username ]`, `[ end_date, success, type ]`, `[ end_date, success, username ]`, `[ end_date, type, username ]`, `[ path, true_path, remote_ip ]`, `[ path, true_path, session_uuid ]`, `[ path, true_path, success ]`, `[ path, true_path, type ]`, `[ path, true_path, username ]`, `[ path, remote_ip, session_uuid ]`, `[ path, remote_ip, success ]`, `[ path, remote_ip, type ]`, `[ path, remote_ip, username ]`, `[ path, session_uuid, success ]`, `[ path, session_uuid, type ]`, `[ path, session_uuid, username ]`, `[ path, success, type ]`, `[ path, success, username ]`, `[ path, type, username ]`, `[ true_path, remote_ip, session_uuid ]`, `[ true_path, remote_ip, success ]`, `[ true_path, remote_ip, type ]`, `[ true_path, remote_ip, username ]`, `[ true_path, session_uuid, success ]`, `[ true_path, session_uuid, type ]`, `[ true_path, session_uuid, username ]`, `[ true_path, success, type ]`, `[ true_path, success, username ]`, `[ true_path, type, username ]`, `[ remote_ip, session_uuid, success ]`, `[ remote_ip, session_uuid, type ]`, `[ remote_ip, session_uuid, username ]`, `[ remote_ip, success, type ]`, `[ remote_ip, success, username ]`, `[ remote_ip, type, username ]`, `[ session_uuid, success, type ]`, `[ session_uuid, success, username ]`, `[ session_uuid, type, username ]`, `[ success, type, username ]`, `[ start_date, end_date, path, true_path ]`, `[ start_date, end_date, path, remote_ip ]`, `[ start_date, end_date, path, session_uuid ]`, `[ start_date, end_date, path, success ]`, `[ start_date, end_date, path, type ]`, `[ start_date, end_date, path, username ]`, `[ start_date, end_date, true_path, remote_ip ]`, `[ start_date, end_date, true_path, session_uuid ]`, `[ start_date, end_date, true_path, success ]`, `[ start_date, end_date, true_path, type ]`, `[ start_date, end_date, true_path, username ]`, `[ start_date, end_date, remote_ip, session_uuid ]`, `[ start_date, end_date, remote_ip, success ]`, `[ start_date, end_date, remote_ip, type ]`, `[ start_date, end_date, remote_ip, username ]`, `[ start_date, end_date, session_uuid, success ]`, `[ start_date, end_date, session_uuid, type ]`, `[ start_date, end_date, session_uuid, username ]`, `[ start_date, end_date, success, type ]`, `[ start_date, end_date, success, username ]`, `[ start_date, end_date, type, username ]`, `[ start_date, path, true_path, remote_ip ]`, `[ start_date, path, true_path, session_uuid ]`, `[ start_date, path, true_path, success ]`, `[ start_date, path, true_path, type ]`, `[ start_date, path, true_path, username ]`, `[ start_date, path, remote_ip, session_uuid ]`, `[ start_date, path, remote_ip, success ]`, `[ start_date, path, remote_ip, type ]`, `[ start_date, path, remote_ip, username ]`, `[ start_date, path, session_uuid, success ]`, `[ start_date, path, session_uuid, type ]`, `[ start_date, path, session_uuid, username ]`, `[ start_date, path, success, type ]`, `[ start_date, path, success, username ]`, `[ start_date, path, type, username ]`, `[ start_date, true_path, remote_ip, session_uuid ]`, `[ start_date, true_path, remote_ip, success ]`, `[ start_date, true_path, remote_ip, type ]`, `[ start_date, true_path, remote_ip, username ]`, `[ start_date, true_path, session_uuid, success ]`, `[ start_date, true_path, session_uuid, type ]`, `[ start_date, true_path, session_uuid, username ]`, `[ start_date, true_path, success, type ]`, `[ start_date, true_path, success, username ]`, `[ start_date, true_path, type, username ]`, `[ start_date, remote_ip, session_uuid, success ]`, `[ start_date, remote_ip, session_uuid, type ]`, `[ start_date, remote_ip, session_uuid, username ]`, `[ start_date, remote_ip, success, type ]`, `[ start_date, remote_ip, success, username ]`, `[ start_date, remote_ip, type, username ]`, `[ start_date, session_uuid, success, type ]`, `[ start_date, session_uuid, success, username ]`, `[ start_date, session_uuid, type, username ]`, `[ start_date, success, type, username ]`, `[ end_date, path, true_path, remote_ip ]`, `[ end_date, path, true_path, session_uuid ]`, `[ end_date, path, true_path, success ]`, `[ end_date, path, true_path, type ]`, `[ end_date, path, true_path, username ]`, `[ end_date, path, remote_ip, session_uuid ]`, `[ end_date, path, remote_ip, success ]`, `[ end_date, path, remote_ip, type ]`, `[ end_date, path, remote_ip, username ]`, `[ end_date, path, session_uuid, success ]`, `[ end_date, path, session_uuid, type ]`, `[ end_date, path, session_uuid, username ]`, `[ end_date, path, success, type ]`, `[ end_date, path, success, username ]`, `[ end_date, path, type, username ]`, `[ end_date, true_path, remote_ip, session_uuid ]`, `[ end_date, true_path, remote_ip, success ]`, `[ end_date, true_path, remote_ip, type ]`, `[ end_date, true_path, remote_ip, username ]`, `[ end_date, true_path, session_uuid, success ]`, `[ end_date, true_path, session_uuid, type ]`, `[ end_date, true_path, session_uuid, username ]`, `[ end_date, true_path, success, type ]`, `[ end_date, true_path, success, username ]`, `[ end_date, true_path, type, username ]`, `[ end_date, remote_ip, session_uuid, success ]`, `[ end_date, remote_ip, session_uuid, type ]`, `[ end_date, remote_ip, session_uuid, username ]`, `[ end_date, remote_ip, success, type ]`, `[ end_date, remote_ip, success, username ]`, `[ end_date, remote_ip, type, username ]`, `[ end_date, session_uuid, success, type ]`, `[ end_date, session_uuid, success, username ]`, `[ end_date, session_uuid, type, username ]`, `[ end_date, success, type, username ]`, `[ path, true_path, remote_ip, session_uuid ]`, `[ path, true_path, remote_ip, success ]`, `[ path, true_path, remote_ip, type ]`, `[ path, true_path, remote_ip, username ]`, `[ path, true_path, session_uuid, success ]`, `[ path, true_path, session_uuid, type ]`, `[ path, true_path, session_uuid, username ]`, `[ path, true_path, success, type ]`, `[ path, true_path, success, username ]`, `[ path, true_path, type, username ]`, `[ path, remote_ip, session_uuid, success ]`, `[ path, remote_ip, session_uuid, type ]`, `[ path, remote_ip, session_uuid, username ]`, `[ path, remote_ip, success, type ]`, `[ path, remote_ip, success, username ]`, `[ path, remote_ip, type, username ]`, `[ path, session_uuid, success, type ]`, `[ path, session_uuid, success, username ]`, `[ path, session_uuid, type, username ]`, `[ path, success, type, username ]`, `[ true_path, remote_ip, session_uuid, success ]`, `[ true_path, remote_ip, session_uuid, type ]`, `[ true_path, remote_ip, session_uuid, username ]`, `[ true_path, remote_ip, success, type ]`, `[ true_path, remote_ip, success, username ]`, `[ true_path, remote_ip, type, username ]`, `[ true_path, session_uuid, success, type ]`, `[ true_path, session_uuid, success, username ]`, `[ true_path, session_uuid, type, username ]`, `[ true_path, success, type, username ]`, `[ remote_ip, session_uuid, success, type ]`, `[ remote_ip, session_uuid, success, username ]`, `[ remote_ip, session_uuid, type, username ]`, `[ remote_ip, success, type, username ]`, `[ session_uuid, success, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip ]`, `[ start_date, end_date, path, true_path, session_uuid ]`, `[ start_date, end_date, path, true_path, success ]`, `[ start_date, end_date, path, true_path, type ]`, `[ start_date, end_date, path, true_path, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid ]`, `[ start_date, end_date, path, remote_ip, success ]`, `[ start_date, end_date, path, remote_ip, type ]`, `[ start_date, end_date, path, remote_ip, username ]`, `[ start_date, end_date, path, session_uuid, success ]`, `[ start_date, end_date, path, session_uuid, type ]`, `[ start_date, end_date, path, session_uuid, username ]`, `[ start_date, end_date, path, success, type ]`, `[ start_date, end_date, path, success, username ]`, `[ start_date, end_date, path, type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid ]`, `[ start_date, end_date, true_path, remote_ip, success ]`, `[ start_date, end_date, true_path, remote_ip, type ]`, `[ start_date, end_date, true_path, remote_ip, username ]`, `[ start_date, end_date, true_path, session_uuid, success ]`, `[ start_date, end_date, true_path, session_uuid, type ]`, `[ start_date, end_date, true_path, session_uuid, username ]`, `[ start_date, end_date, true_path, success, type ]`, `[ start_date, end_date, true_path, success, username ]`, `[ start_date, end_date, true_path, type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success ]`, `[ start_date, end_date, remote_ip, session_uuid, type ]`, `[ start_date, end_date, remote_ip, session_uuid, username ]`, `[ start_date, end_date, remote_ip, success, type ]`, `[ start_date, end_date, remote_ip, success, username ]`, `[ start_date, end_date, remote_ip, type, username ]`, `[ start_date, end_date, session_uuid, success, type ]`, `[ start_date, end_date, session_uuid, success, username ]`, `[ start_date, end_date, session_uuid, type, username ]`, `[ start_date, end_date, success, type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid ]`, `[ start_date, path, true_path, remote_ip, success ]`, `[ start_date, path, true_path, remote_ip, type ]`, `[ start_date, path, true_path, remote_ip, username ]`, `[ start_date, path, true_path, session_uuid, success ]`, `[ start_date, path, true_path, session_uuid, type ]`, `[ start_date, path, true_path, session_uuid, username ]`, `[ start_date, path, true_path, success, type ]`, `[ start_date, path, true_path, success, username ]`, `[ start_date, path, true_path, type, username ]`, `[ start_date, path, remote_ip, session_uuid, success ]`, `[ start_date, path, remote_ip, session_uuid, type ]`, `[ start_date, path, remote_ip, session_uuid, username ]`, `[ start_date, path, remote_ip, success, type ]`, `[ start_date, path, remote_ip, success, username ]`, `[ start_date, path, remote_ip, type, username ]`, `[ start_date, path, session_uuid, success, type ]`, `[ start_date, path, session_uuid, success, username ]`, `[ start_date, path, session_uuid, type, username ]`, `[ start_date, path, success, type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success ]`, `[ start_date, true_path, remote_ip, session_uuid, type ]`, `[ start_date, true_path, remote_ip, session_uuid, username ]`, `[ start_date, true_path, remote_ip, success, type ]`, `[ start_date, true_path, remote_ip, success, username ]`, `[ start_date, true_path, remote_ip, type, username ]`, `[ start_date, true_path, session_uuid, success, type ]`, `[ start_date, true_path, session_uuid, success, username ]`, `[ start_date, true_path, session_uuid, type, username ]`, `[ start_date, true_path, success, type, username ]`, `[ start_date, remote_ip, session_uuid, success, type ]`, `[ start_date, remote_ip, session_uuid, success, username ]`, `[ start_date, remote_ip, session_uuid, type, username ]`, `[ start_date, remote_ip, success, type, username ]`, `[ start_date, session_uuid, success, type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid ]`, `[ end_date, path, true_path, remote_ip, success ]`, `[ end_date, path, true_path, remote_ip, type ]`, `[ end_date, path, true_path, remote_ip, username ]`, `[ end_date, path, true_path, session_uuid, success ]`, `[ end_date, path, true_path, session_uuid, type ]`, `[ end_date, path, true_path, session_uuid, username ]`, `[ end_date, path, true_path, success, type ]`, `[ end_date, path, true_path, success, username ]`, `[ end_date, path, true_path, type, username ]`, `[ end_date, path, remote_ip, session_uuid, success ]`, `[ end_date, path, remote_ip, session_uuid, type ]`, `[ end_date, path, remote_ip, session_uuid, username ]`, `[ end_date, path, remote_ip, success, type ]`, `[ end_date, path, remote_ip, success, username ]`, `[ end_date, path, remote_ip, type, username ]`, `[ end_date, path, session_uuid, success, type ]`, `[ end_date, path, session_uuid, success, username ]`, `[ end_date, path, session_uuid, type, username ]`, `[ end_date, path, success, type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success ]`, `[ end_date, true_path, remote_ip, session_uuid, type ]`, `[ end_date, true_path, remote_ip, session_uuid, username ]`, `[ end_date, true_path, remote_ip, success, type ]`, `[ end_date, true_path, remote_ip, success, username ]`, `[ end_date, true_path, remote_ip, type, username ]`, `[ end_date, true_path, session_uuid, success, type ]`, `[ end_date, true_path, session_uuid, success, username ]`, `[ end_date, true_path, session_uuid, type, username ]`, `[ end_date, true_path, success, type, username ]`, `[ end_date, remote_ip, session_uuid, success, type ]`, `[ end_date, remote_ip, session_uuid, success, username ]`, `[ end_date, remote_ip, session_uuid, type, username ]`, `[ end_date, remote_ip, success, type, username ]`, `[ end_date, session_uuid, success, type, username ]`, `[ path, true_path, remote_ip, session_uuid, success ]`, `[ path, true_path, remote_ip, session_uuid, type ]`, `[ path, true_path, remote_ip, session_uuid, username ]`, `[ path, true_path, remote_ip, success, type ]`, `[ path, true_path, remote_ip, success, username ]`, `[ path, true_path, remote_ip, type, username ]`, `[ path, true_path, session_uuid, success, type ]`, `[ path, true_path, session_uuid, success, username ]`, `[ path, true_path, session_uuid, type, username ]`, `[ path, true_path, success, type, username ]`, `[ path, remote_ip, session_uuid, success, type ]`, `[ path, remote_ip, session_uuid, success, username ]`, `[ path, remote_ip, session_uuid, type, username ]`, `[ path, remote_ip, success, type, username ]`, `[ path, session_uuid, success, type, username ]`, `[ true_path, remote_ip, session_uuid, success, type ]`, `[ true_path, remote_ip, session_uuid, success, username ]`, `[ true_path, remote_ip, session_uuid, type, username ]`, `[ true_path, remote_ip, success, type, username ]`, `[ true_path, session_uuid, success, type, username ]`, `[ remote_ip, session_uuid, success, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid ]`, `[ start_date, end_date, path, true_path, remote_ip, success ]`, `[ start_date, end_date, path, true_path, remote_ip, type ]`, `[ start_date, end_date, path, true_path, remote_ip, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success ]`, `[ start_date, end_date, path, true_path, session_uuid, type ]`, `[ start_date, end_date, path, true_path, session_uuid, username ]`, `[ start_date, end_date, path, true_path, success, type ]`, `[ start_date, end_date, path, true_path, success, username ]`, `[ start_date, end_date, path, true_path, type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, path, remote_ip, session_uuid, type ]`, `[ start_date, end_date, path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, path, remote_ip, success, type ]`, `[ start_date, end_date, path, remote_ip, success, username ]`, `[ start_date, end_date, path, remote_ip, type, username ]`, `[ start_date, end_date, path, session_uuid, success, type ]`, `[ start_date, end_date, path, session_uuid, success, username ]`, `[ start_date, end_date, path, session_uuid, type, username ]`, `[ start_date, end_date, path, success, type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, type ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, true_path, remote_ip, success, type ]`, `[ start_date, end_date, true_path, remote_ip, success, username ]`, `[ start_date, end_date, true_path, remote_ip, type, username ]`, `[ start_date, end_date, true_path, session_uuid, success, type ]`, `[ start_date, end_date, true_path, session_uuid, success, username ]`, `[ start_date, end_date, true_path, session_uuid, type, username ]`, `[ start_date, end_date, true_path, success, type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success, type ]`, `[ start_date, end_date, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, remote_ip, session_uuid, type, username ]`, `[ start_date, end_date, remote_ip, success, type, username ]`, `[ start_date, end_date, session_uuid, success, type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success ]`, `[ start_date, path, true_path, remote_ip, session_uuid, type ]`, `[ start_date, path, true_path, remote_ip, session_uuid, username ]`, `[ start_date, path, true_path, remote_ip, success, type ]`, `[ start_date, path, true_path, remote_ip, success, username ]`, `[ start_date, path, true_path, remote_ip, type, username ]`, `[ start_date, path, true_path, session_uuid, success, type ]`, `[ start_date, path, true_path, session_uuid, success, username ]`, `[ start_date, path, true_path, session_uuid, type, username ]`, `[ start_date, path, true_path, success, type, username ]`, `[ start_date, path, remote_ip, session_uuid, success, type ]`, `[ start_date, path, remote_ip, session_uuid, success, username ]`, `[ start_date, path, remote_ip, session_uuid, type, username ]`, `[ start_date, path, remote_ip, success, type, username ]`, `[ start_date, path, session_uuid, success, type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success, type ]`, `[ start_date, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, true_path, remote_ip, session_uuid, type, username ]`, `[ start_date, true_path, remote_ip, success, type, username ]`, `[ start_date, true_path, session_uuid, success, type, username ]`, `[ start_date, remote_ip, session_uuid, success, type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success ]`, `[ end_date, path, true_path, remote_ip, session_uuid, type ]`, `[ end_date, path, true_path, remote_ip, session_uuid, username ]`, `[ end_date, path, true_path, remote_ip, success, type ]`, `[ end_date, path, true_path, remote_ip, success, username ]`, `[ end_date, path, true_path, remote_ip, type, username ]`, `[ end_date, path, true_path, session_uuid, success, type ]`, `[ end_date, path, true_path, session_uuid, success, username ]`, `[ end_date, path, true_path, session_uuid, type, username ]`, `[ end_date, path, true_path, success, type, username ]`, `[ end_date, path, remote_ip, session_uuid, success, type ]`, `[ end_date, path, remote_ip, session_uuid, success, username ]`, `[ end_date, path, remote_ip, session_uuid, type, username ]`, `[ end_date, path, remote_ip, success, type, username ]`, `[ end_date, path, session_uuid, success, type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success, type ]`, `[ end_date, true_path, remote_ip, session_uuid, success, username ]`, `[ end_date, true_path, remote_ip, session_uuid, type, username ]`, `[ end_date, true_path, remote_ip, success, type, username ]`, `[ end_date, true_path, session_uuid, success, type, username ]`, `[ end_date, remote_ip, session_uuid, success, type, username ]`, `[ path, true_path, remote_ip, session_uuid, success, type ]`, `[ path, true_path, remote_ip, session_uuid, success, username ]`, `[ path, true_path, remote_ip, session_uuid, type, username ]`, `[ path, true_path, remote_ip, success, type, username ]`, `[ path, true_path, session_uuid, success, type, username ]`, `[ path, remote_ip, session_uuid, success, type, username ]`, `[ true_path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, type ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, username ]`, `[ start_date, end_date, path, true_path, remote_ip, success, type ]`, `[ start_date, end_date, path, true_path, remote_ip, success, username ]`, `[ start_date, end_date, path, true_path, remote_ip, type, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success, type ]`, `[ start_date, end_date, path, true_path, session_uuid, success, username ]`, `[ start_date, end_date, path, true_path, session_uuid, type, username ]`, `[ start_date, end_date, path, true_path, success, type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, type ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, type, username ]`, `[ start_date, end_date, path, remote_ip, success, type, username ]`, `[ start_date, end_date, path, session_uuid, success, type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, type ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, type, username ]`, `[ start_date, end_date, true_path, remote_ip, success, type, username ]`, `[ start_date, end_date, true_path, session_uuid, success, type, username ]`, `[ start_date, end_date, remote_ip, session_uuid, success, type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, type ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, type, username ]`, `[ start_date, path, true_path, remote_ip, success, type, username ]`, `[ start_date, path, true_path, session_uuid, success, type, username ]`, `[ start_date, path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, true_path, remote_ip, session_uuid, success, type, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success, type ]`, `[ end_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ end_date, path, true_path, remote_ip, session_uuid, type, username ]`, `[ end_date, path, true_path, remote_ip, success, type, username ]`, `[ end_date, path, true_path, session_uuid, success, type, username ]`, `[ end_date, path, remote_ip, session_uuid, success, type, username ]`, `[ end_date, true_path, remote_ip, session_uuid, success, type, username ]`, `[ path, true_path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success, type ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, success, username ]`, `[ start_date, end_date, path, true_path, remote_ip, session_uuid, type, username ]`, `[ start_date, end_date, path, true_path, remote_ip, success, type, username ]`, `[ start_date, end_date, path, true_path, session_uuid, success, type, username ]`, `[ start_date, end_date, path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, end_date, true_path, remote_ip, session_uuid, success, type, username ]`, `[ start_date, path, true_path, remote_ip, session_uuid, success, type, username ]` or `[ end_date, path, true_path, remote_ip, session_uuid, success, type, username ]`.
        /// </summary>
        public static FilesList<SftpActionLog> List(

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
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }

            return new FilesList<SftpActionLog>($"/sftp_action_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<SftpActionLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}