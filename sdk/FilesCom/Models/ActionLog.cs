using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ActionLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ActionLog() : this(null, null) { }

        public ActionLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("destination"))
            {
                this.attributes.Add("destination", null);
            }
            if (!this.attributes.ContainsKey("failure_type"))
            {
                this.attributes.Add("failure_type", null);
            }
            if (!this.attributes.ContainsKey("folder"))
            {
                this.attributes.Add("folder", null);
            }
            if (!this.attributes.ContainsKey("interface"))
            {
                this.attributes.Add("interface", null);
            }
            if (!this.attributes.ContainsKey("ip"))
            {
                this.attributes.Add("ip", null);
            }
            if (!this.attributes.ContainsKey("metadata_dm_id"))
            {
                this.attributes.Add("metadata_dm_id", null);
            }
            if (!this.attributes.ContainsKey("parent_metadata_dm_id"))
            {
                this.attributes.Add("parent_metadata_dm_id", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
            }
            if (!this.attributes.ContainsKey("src"))
            {
                this.attributes.Add("src", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
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
        /// The type of action performed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            private set { attributes["action"] = value; }
        }

        /// <summary>
        /// Time the action was performed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Destination path, for moves and copies
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string)attributes["destination"]; }
            private set { attributes["destination"] = value; }
        }

        /// <summary>
        /// Failure type, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failure_type")]
        public string FailureType
        {
            get { return (string)attributes["failure_type"]; }
            private set { attributes["failure_type"] = value; }
        }

        /// <summary>
        /// Folder involved in the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("folder")]
        public string Folder
        {
            get { return (string)attributes["folder"]; }
            private set { attributes["folder"] = value; }
        }

        /// <summary>
        /// Interface used to perform the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("interface")]
        public string Interface
        {
            get { return (string)attributes["interface"]; }
            private set { attributes["interface"] = value; }
        }

        /// <summary>
        /// IP address from which the action was performed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string)attributes["ip"]; }
            private set { attributes["ip"] = value; }
        }

        /// <summary>
        /// File or Folder ID associated with the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata_dm_id")]
        public Nullable<Int64> MetadataDmId
        {
            get { return (Nullable<Int64>)attributes["metadata_dm_id"]; }
            private set { attributes["metadata_dm_id"] = value; }
        }

        /// <summary>
        /// Parent File or Folder ID associated with the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("parent_metadata_dm_id")]
        public Nullable<Int64> ParentMetadataDmId
        {
            get { return (Nullable<Int64>)attributes["parent_metadata_dm_id"]; }
            private set { attributes["parent_metadata_dm_id"] = value; }
        }

        /// <summary>
        /// File path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// Site ID where the action took place
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            private set { attributes["site_id"] = value; }
        }

        /// <summary>
        /// Source path, for moves and copies
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("src")]
        public string Src
        {
            get { return (string)attributes["src"]; }
            private set { attributes["src"] = value; }
        }

        /// <summary>
        /// User ID who performed the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            private set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Username who performed the action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            private set { attributes["username"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `folder`, `path`, `src`, `destination`, `created_at`, `user_id` or `username`. Valid field combinations are `[ folder ]`, `[ path ]`, `[ src ]`, `[ destination ]`, `[ created_at ]`, `[ user_id ]`, `[ username ]`, `[ folder, path ]`, `[ folder, src ]`, `[ folder, destination ]`, `[ folder, created_at ]`, `[ folder, user_id ]`, `[ folder, username ]`, `[ path, src ]`, `[ path, destination ]`, `[ path, created_at ]`, `[ path, user_id ]`, `[ path, username ]`, `[ src, destination ]`, `[ src, created_at ]`, `[ src, user_id ]`, `[ src, username ]`, `[ destination, created_at ]`, `[ destination, user_id ]`, `[ destination, username ]`, `[ created_at, user_id ]`, `[ created_at, username ]`, `[ user_id, username ]`, `[ folder, path, src ]`, `[ folder, path, destination ]`, `[ folder, path, created_at ]`, `[ folder, path, user_id ]`, `[ folder, path, username ]`, `[ folder, src, destination ]`, `[ folder, src, created_at ]`, `[ folder, src, user_id ]`, `[ folder, src, username ]`, `[ folder, destination, created_at ]`, `[ folder, destination, user_id ]`, `[ folder, destination, username ]`, `[ folder, created_at, user_id ]`, `[ folder, created_at, username ]`, `[ folder, user_id, username ]`, `[ path, src, destination ]`, `[ path, src, created_at ]`, `[ path, src, user_id ]`, `[ path, src, username ]`, `[ path, destination, created_at ]`, `[ path, destination, user_id ]`, `[ path, destination, username ]`, `[ path, created_at, user_id ]`, `[ path, created_at, username ]`, `[ path, user_id, username ]`, `[ src, destination, created_at ]`, `[ src, destination, user_id ]`, `[ src, destination, username ]`, `[ src, created_at, user_id ]`, `[ src, created_at, username ]`, `[ src, user_id, username ]`, `[ destination, created_at, user_id ]`, `[ destination, created_at, username ]`, `[ destination, user_id, username ]`, `[ created_at, user_id, username ]`, `[ folder, path, src, destination ]`, `[ folder, path, src, created_at ]`, `[ folder, path, src, user_id ]`, `[ folder, path, src, username ]`, `[ folder, path, destination, created_at ]`, `[ folder, path, destination, user_id ]`, `[ folder, path, destination, username ]`, `[ folder, path, created_at, user_id ]`, `[ folder, path, created_at, username ]`, `[ folder, path, user_id, username ]`, `[ folder, src, destination, created_at ]`, `[ folder, src, destination, user_id ]`, `[ folder, src, destination, username ]`, `[ folder, src, created_at, user_id ]`, `[ folder, src, created_at, username ]`, `[ folder, src, user_id, username ]`, `[ folder, destination, created_at, user_id ]`, `[ folder, destination, created_at, username ]`, `[ folder, destination, user_id, username ]`, `[ folder, created_at, user_id, username ]`, `[ path, src, destination, created_at ]`, `[ path, src, destination, user_id ]`, `[ path, src, destination, username ]`, `[ path, src, created_at, user_id ]`, `[ path, src, created_at, username ]`, `[ path, src, user_id, username ]`, `[ path, destination, created_at, user_id ]`, `[ path, destination, created_at, username ]`, `[ path, destination, user_id, username ]`, `[ path, created_at, user_id, username ]`, `[ src, destination, created_at, user_id ]`, `[ src, destination, created_at, username ]`, `[ src, destination, user_id, username ]`, `[ src, created_at, user_id, username ]`, `[ destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at ]`, `[ folder, path, src, destination, user_id ]`, `[ folder, path, src, destination, username ]`, `[ folder, path, src, created_at, user_id ]`, `[ folder, path, src, created_at, username ]`, `[ folder, path, src, user_id, username ]`, `[ folder, path, destination, created_at, user_id ]`, `[ folder, path, destination, created_at, username ]`, `[ folder, path, destination, user_id, username ]`, `[ folder, path, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id ]`, `[ folder, src, destination, created_at, username ]`, `[ folder, src, destination, user_id, username ]`, `[ folder, src, created_at, user_id, username ]`, `[ folder, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id ]`, `[ path, src, destination, created_at, username ]`, `[ path, src, destination, user_id, username ]`, `[ path, src, created_at, user_id, username ]`, `[ path, destination, created_at, user_id, username ]`, `[ src, destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at, user_id ]`, `[ folder, path, src, destination, created_at, username ]`, `[ folder, path, src, destination, user_id, username ]`, `[ folder, path, src, created_at, user_id, username ]`, `[ folder, path, destination, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id, username ]` or `[ folder, path, src, destination, created_at, user_id, username ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`. Valid field combinations are `[ folder ]`, `[ path ]`, `[ src ]`, `[ destination ]`, `[ created_at ]`, `[ user_id ]`, `[ username ]`, `[ folder, path ]`, `[ folder, src ]`, `[ folder, destination ]`, `[ folder, created_at ]`, `[ folder, user_id ]`, `[ folder, username ]`, `[ path, src ]`, `[ path, destination ]`, `[ path, created_at ]`, `[ path, user_id ]`, `[ path, username ]`, `[ src, destination ]`, `[ src, created_at ]`, `[ src, user_id ]`, `[ src, username ]`, `[ destination, created_at ]`, `[ destination, user_id ]`, `[ destination, username ]`, `[ created_at, user_id ]`, `[ created_at, username ]`, `[ user_id, username ]`, `[ folder, path, src ]`, `[ folder, path, destination ]`, `[ folder, path, created_at ]`, `[ folder, path, user_id ]`, `[ folder, path, username ]`, `[ folder, src, destination ]`, `[ folder, src, created_at ]`, `[ folder, src, user_id ]`, `[ folder, src, username ]`, `[ folder, destination, created_at ]`, `[ folder, destination, user_id ]`, `[ folder, destination, username ]`, `[ folder, created_at, user_id ]`, `[ folder, created_at, username ]`, `[ folder, user_id, username ]`, `[ path, src, destination ]`, `[ path, src, created_at ]`, `[ path, src, user_id ]`, `[ path, src, username ]`, `[ path, destination, created_at ]`, `[ path, destination, user_id ]`, `[ path, destination, username ]`, `[ path, created_at, user_id ]`, `[ path, created_at, username ]`, `[ path, user_id, username ]`, `[ src, destination, created_at ]`, `[ src, destination, user_id ]`, `[ src, destination, username ]`, `[ src, created_at, user_id ]`, `[ src, created_at, username ]`, `[ src, user_id, username ]`, `[ destination, created_at, user_id ]`, `[ destination, created_at, username ]`, `[ destination, user_id, username ]`, `[ created_at, user_id, username ]`, `[ folder, path, src, destination ]`, `[ folder, path, src, created_at ]`, `[ folder, path, src, user_id ]`, `[ folder, path, src, username ]`, `[ folder, path, destination, created_at ]`, `[ folder, path, destination, user_id ]`, `[ folder, path, destination, username ]`, `[ folder, path, created_at, user_id ]`, `[ folder, path, created_at, username ]`, `[ folder, path, user_id, username ]`, `[ folder, src, destination, created_at ]`, `[ folder, src, destination, user_id ]`, `[ folder, src, destination, username ]`, `[ folder, src, created_at, user_id ]`, `[ folder, src, created_at, username ]`, `[ folder, src, user_id, username ]`, `[ folder, destination, created_at, user_id ]`, `[ folder, destination, created_at, username ]`, `[ folder, destination, user_id, username ]`, `[ folder, created_at, user_id, username ]`, `[ path, src, destination, created_at ]`, `[ path, src, destination, user_id ]`, `[ path, src, destination, username ]`, `[ path, src, created_at, user_id ]`, `[ path, src, created_at, username ]`, `[ path, src, user_id, username ]`, `[ path, destination, created_at, user_id ]`, `[ path, destination, created_at, username ]`, `[ path, destination, user_id, username ]`, `[ path, created_at, user_id, username ]`, `[ src, destination, created_at, user_id ]`, `[ src, destination, created_at, username ]`, `[ src, destination, user_id, username ]`, `[ src, created_at, user_id, username ]`, `[ destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at ]`, `[ folder, path, src, destination, user_id ]`, `[ folder, path, src, destination, username ]`, `[ folder, path, src, created_at, user_id ]`, `[ folder, path, src, created_at, username ]`, `[ folder, path, src, user_id, username ]`, `[ folder, path, destination, created_at, user_id ]`, `[ folder, path, destination, created_at, username ]`, `[ folder, path, destination, user_id, username ]`, `[ folder, path, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id ]`, `[ folder, src, destination, created_at, username ]`, `[ folder, src, destination, user_id, username ]`, `[ folder, src, created_at, user_id, username ]`, `[ folder, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id ]`, `[ path, src, destination, created_at, username ]`, `[ path, src, destination, user_id, username ]`, `[ path, src, created_at, user_id, username ]`, `[ path, destination, created_at, user_id, username ]`, `[ src, destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at, user_id ]`, `[ folder, path, src, destination, created_at, username ]`, `[ folder, path, src, destination, user_id, username ]`, `[ folder, path, src, created_at, user_id, username ]`, `[ folder, path, destination, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id, username ]` or `[ folder, path, src, destination, created_at, user_id, username ]`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`. Valid field combinations are `[ folder ]`, `[ path ]`, `[ src ]`, `[ destination ]`, `[ created_at ]`, `[ user_id ]`, `[ username ]`, `[ folder, path ]`, `[ folder, src ]`, `[ folder, destination ]`, `[ folder, created_at ]`, `[ folder, user_id ]`, `[ folder, username ]`, `[ path, src ]`, `[ path, destination ]`, `[ path, created_at ]`, `[ path, user_id ]`, `[ path, username ]`, `[ src, destination ]`, `[ src, created_at ]`, `[ src, user_id ]`, `[ src, username ]`, `[ destination, created_at ]`, `[ destination, user_id ]`, `[ destination, username ]`, `[ created_at, user_id ]`, `[ created_at, username ]`, `[ user_id, username ]`, `[ folder, path, src ]`, `[ folder, path, destination ]`, `[ folder, path, created_at ]`, `[ folder, path, user_id ]`, `[ folder, path, username ]`, `[ folder, src, destination ]`, `[ folder, src, created_at ]`, `[ folder, src, user_id ]`, `[ folder, src, username ]`, `[ folder, destination, created_at ]`, `[ folder, destination, user_id ]`, `[ folder, destination, username ]`, `[ folder, created_at, user_id ]`, `[ folder, created_at, username ]`, `[ folder, user_id, username ]`, `[ path, src, destination ]`, `[ path, src, created_at ]`, `[ path, src, user_id ]`, `[ path, src, username ]`, `[ path, destination, created_at ]`, `[ path, destination, user_id ]`, `[ path, destination, username ]`, `[ path, created_at, user_id ]`, `[ path, created_at, username ]`, `[ path, user_id, username ]`, `[ src, destination, created_at ]`, `[ src, destination, user_id ]`, `[ src, destination, username ]`, `[ src, created_at, user_id ]`, `[ src, created_at, username ]`, `[ src, user_id, username ]`, `[ destination, created_at, user_id ]`, `[ destination, created_at, username ]`, `[ destination, user_id, username ]`, `[ created_at, user_id, username ]`, `[ folder, path, src, destination ]`, `[ folder, path, src, created_at ]`, `[ folder, path, src, user_id ]`, `[ folder, path, src, username ]`, `[ folder, path, destination, created_at ]`, `[ folder, path, destination, user_id ]`, `[ folder, path, destination, username ]`, `[ folder, path, created_at, user_id ]`, `[ folder, path, created_at, username ]`, `[ folder, path, user_id, username ]`, `[ folder, src, destination, created_at ]`, `[ folder, src, destination, user_id ]`, `[ folder, src, destination, username ]`, `[ folder, src, created_at, user_id ]`, `[ folder, src, created_at, username ]`, `[ folder, src, user_id, username ]`, `[ folder, destination, created_at, user_id ]`, `[ folder, destination, created_at, username ]`, `[ folder, destination, user_id, username ]`, `[ folder, created_at, user_id, username ]`, `[ path, src, destination, created_at ]`, `[ path, src, destination, user_id ]`, `[ path, src, destination, username ]`, `[ path, src, created_at, user_id ]`, `[ path, src, created_at, username ]`, `[ path, src, user_id, username ]`, `[ path, destination, created_at, user_id ]`, `[ path, destination, created_at, username ]`, `[ path, destination, user_id, username ]`, `[ path, created_at, user_id, username ]`, `[ src, destination, created_at, user_id ]`, `[ src, destination, created_at, username ]`, `[ src, destination, user_id, username ]`, `[ src, created_at, user_id, username ]`, `[ destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at ]`, `[ folder, path, src, destination, user_id ]`, `[ folder, path, src, destination, username ]`, `[ folder, path, src, created_at, user_id ]`, `[ folder, path, src, created_at, username ]`, `[ folder, path, src, user_id, username ]`, `[ folder, path, destination, created_at, user_id ]`, `[ folder, path, destination, created_at, username ]`, `[ folder, path, destination, user_id, username ]`, `[ folder, path, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id ]`, `[ folder, src, destination, created_at, username ]`, `[ folder, src, destination, user_id, username ]`, `[ folder, src, created_at, user_id, username ]`, `[ folder, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id ]`, `[ path, src, destination, created_at, username ]`, `[ path, src, destination, user_id, username ]`, `[ path, src, created_at, user_id, username ]`, `[ path, destination, created_at, user_id, username ]`, `[ src, destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at, user_id ]`, `[ folder, path, src, destination, created_at, username ]`, `[ folder, path, src, destination, user_id, username ]`, `[ folder, path, src, created_at, user_id, username ]`, `[ folder, path, destination, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id, username ]` or `[ folder, path, src, destination, created_at, user_id, username ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `folder`, `path`, `src`, `destination` or `username`. Valid field combinations are `[ folder ]`, `[ path ]`, `[ src ]`, `[ destination ]`, `[ created_at ]`, `[ user_id ]`, `[ username ]`, `[ folder, path ]`, `[ folder, src ]`, `[ folder, destination ]`, `[ folder, created_at ]`, `[ folder, user_id ]`, `[ folder, username ]`, `[ path, src ]`, `[ path, destination ]`, `[ path, created_at ]`, `[ path, user_id ]`, `[ path, username ]`, `[ src, destination ]`, `[ src, created_at ]`, `[ src, user_id ]`, `[ src, username ]`, `[ destination, created_at ]`, `[ destination, user_id ]`, `[ destination, username ]`, `[ created_at, user_id ]`, `[ created_at, username ]`, `[ user_id, username ]`, `[ folder, path, src ]`, `[ folder, path, destination ]`, `[ folder, path, created_at ]`, `[ folder, path, user_id ]`, `[ folder, path, username ]`, `[ folder, src, destination ]`, `[ folder, src, created_at ]`, `[ folder, src, user_id ]`, `[ folder, src, username ]`, `[ folder, destination, created_at ]`, `[ folder, destination, user_id ]`, `[ folder, destination, username ]`, `[ folder, created_at, user_id ]`, `[ folder, created_at, username ]`, `[ folder, user_id, username ]`, `[ path, src, destination ]`, `[ path, src, created_at ]`, `[ path, src, user_id ]`, `[ path, src, username ]`, `[ path, destination, created_at ]`, `[ path, destination, user_id ]`, `[ path, destination, username ]`, `[ path, created_at, user_id ]`, `[ path, created_at, username ]`, `[ path, user_id, username ]`, `[ src, destination, created_at ]`, `[ src, destination, user_id ]`, `[ src, destination, username ]`, `[ src, created_at, user_id ]`, `[ src, created_at, username ]`, `[ src, user_id, username ]`, `[ destination, created_at, user_id ]`, `[ destination, created_at, username ]`, `[ destination, user_id, username ]`, `[ created_at, user_id, username ]`, `[ folder, path, src, destination ]`, `[ folder, path, src, created_at ]`, `[ folder, path, src, user_id ]`, `[ folder, path, src, username ]`, `[ folder, path, destination, created_at ]`, `[ folder, path, destination, user_id ]`, `[ folder, path, destination, username ]`, `[ folder, path, created_at, user_id ]`, `[ folder, path, created_at, username ]`, `[ folder, path, user_id, username ]`, `[ folder, src, destination, created_at ]`, `[ folder, src, destination, user_id ]`, `[ folder, src, destination, username ]`, `[ folder, src, created_at, user_id ]`, `[ folder, src, created_at, username ]`, `[ folder, src, user_id, username ]`, `[ folder, destination, created_at, user_id ]`, `[ folder, destination, created_at, username ]`, `[ folder, destination, user_id, username ]`, `[ folder, created_at, user_id, username ]`, `[ path, src, destination, created_at ]`, `[ path, src, destination, user_id ]`, `[ path, src, destination, username ]`, `[ path, src, created_at, user_id ]`, `[ path, src, created_at, username ]`, `[ path, src, user_id, username ]`, `[ path, destination, created_at, user_id ]`, `[ path, destination, created_at, username ]`, `[ path, destination, user_id, username ]`, `[ path, created_at, user_id, username ]`, `[ src, destination, created_at, user_id ]`, `[ src, destination, created_at, username ]`, `[ src, destination, user_id, username ]`, `[ src, created_at, user_id, username ]`, `[ destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at ]`, `[ folder, path, src, destination, user_id ]`, `[ folder, path, src, destination, username ]`, `[ folder, path, src, created_at, user_id ]`, `[ folder, path, src, created_at, username ]`, `[ folder, path, src, user_id, username ]`, `[ folder, path, destination, created_at, user_id ]`, `[ folder, path, destination, created_at, username ]`, `[ folder, path, destination, user_id, username ]`, `[ folder, path, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id ]`, `[ folder, src, destination, created_at, username ]`, `[ folder, src, destination, user_id, username ]`, `[ folder, src, created_at, user_id, username ]`, `[ folder, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id ]`, `[ path, src, destination, created_at, username ]`, `[ path, src, destination, user_id, username ]`, `[ path, src, created_at, user_id, username ]`, `[ path, destination, created_at, user_id, username ]`, `[ src, destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at, user_id ]`, `[ folder, path, src, destination, created_at, username ]`, `[ folder, path, src, destination, user_id, username ]`, `[ folder, path, src, created_at, user_id, username ]`, `[ folder, path, destination, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id, username ]` or `[ folder, path, src, destination, created_at, user_id, username ]`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`. Valid field combinations are `[ folder ]`, `[ path ]`, `[ src ]`, `[ destination ]`, `[ created_at ]`, `[ user_id ]`, `[ username ]`, `[ folder, path ]`, `[ folder, src ]`, `[ folder, destination ]`, `[ folder, created_at ]`, `[ folder, user_id ]`, `[ folder, username ]`, `[ path, src ]`, `[ path, destination ]`, `[ path, created_at ]`, `[ path, user_id ]`, `[ path, username ]`, `[ src, destination ]`, `[ src, created_at ]`, `[ src, user_id ]`, `[ src, username ]`, `[ destination, created_at ]`, `[ destination, user_id ]`, `[ destination, username ]`, `[ created_at, user_id ]`, `[ created_at, username ]`, `[ user_id, username ]`, `[ folder, path, src ]`, `[ folder, path, destination ]`, `[ folder, path, created_at ]`, `[ folder, path, user_id ]`, `[ folder, path, username ]`, `[ folder, src, destination ]`, `[ folder, src, created_at ]`, `[ folder, src, user_id ]`, `[ folder, src, username ]`, `[ folder, destination, created_at ]`, `[ folder, destination, user_id ]`, `[ folder, destination, username ]`, `[ folder, created_at, user_id ]`, `[ folder, created_at, username ]`, `[ folder, user_id, username ]`, `[ path, src, destination ]`, `[ path, src, created_at ]`, `[ path, src, user_id ]`, `[ path, src, username ]`, `[ path, destination, created_at ]`, `[ path, destination, user_id ]`, `[ path, destination, username ]`, `[ path, created_at, user_id ]`, `[ path, created_at, username ]`, `[ path, user_id, username ]`, `[ src, destination, created_at ]`, `[ src, destination, user_id ]`, `[ src, destination, username ]`, `[ src, created_at, user_id ]`, `[ src, created_at, username ]`, `[ src, user_id, username ]`, `[ destination, created_at, user_id ]`, `[ destination, created_at, username ]`, `[ destination, user_id, username ]`, `[ created_at, user_id, username ]`, `[ folder, path, src, destination ]`, `[ folder, path, src, created_at ]`, `[ folder, path, src, user_id ]`, `[ folder, path, src, username ]`, `[ folder, path, destination, created_at ]`, `[ folder, path, destination, user_id ]`, `[ folder, path, destination, username ]`, `[ folder, path, created_at, user_id ]`, `[ folder, path, created_at, username ]`, `[ folder, path, user_id, username ]`, `[ folder, src, destination, created_at ]`, `[ folder, src, destination, user_id ]`, `[ folder, src, destination, username ]`, `[ folder, src, created_at, user_id ]`, `[ folder, src, created_at, username ]`, `[ folder, src, user_id, username ]`, `[ folder, destination, created_at, user_id ]`, `[ folder, destination, created_at, username ]`, `[ folder, destination, user_id, username ]`, `[ folder, created_at, user_id, username ]`, `[ path, src, destination, created_at ]`, `[ path, src, destination, user_id ]`, `[ path, src, destination, username ]`, `[ path, src, created_at, user_id ]`, `[ path, src, created_at, username ]`, `[ path, src, user_id, username ]`, `[ path, destination, created_at, user_id ]`, `[ path, destination, created_at, username ]`, `[ path, destination, user_id, username ]`, `[ path, created_at, user_id, username ]`, `[ src, destination, created_at, user_id ]`, `[ src, destination, created_at, username ]`, `[ src, destination, user_id, username ]`, `[ src, created_at, user_id, username ]`, `[ destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at ]`, `[ folder, path, src, destination, user_id ]`, `[ folder, path, src, destination, username ]`, `[ folder, path, src, created_at, user_id ]`, `[ folder, path, src, created_at, username ]`, `[ folder, path, src, user_id, username ]`, `[ folder, path, destination, created_at, user_id ]`, `[ folder, path, destination, created_at, username ]`, `[ folder, path, destination, user_id, username ]`, `[ folder, path, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id ]`, `[ folder, src, destination, created_at, username ]`, `[ folder, src, destination, user_id, username ]`, `[ folder, src, created_at, user_id, username ]`, `[ folder, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id ]`, `[ path, src, destination, created_at, username ]`, `[ path, src, destination, user_id, username ]`, `[ path, src, created_at, user_id, username ]`, `[ path, destination, created_at, user_id, username ]`, `[ src, destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at, user_id ]`, `[ folder, path, src, destination, created_at, username ]`, `[ folder, path, src, destination, user_id, username ]`, `[ folder, path, src, created_at, user_id, username ]`, `[ folder, path, destination, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id, username ]` or `[ folder, path, src, destination, created_at, user_id, username ]`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`. Valid field combinations are `[ folder ]`, `[ path ]`, `[ src ]`, `[ destination ]`, `[ created_at ]`, `[ user_id ]`, `[ username ]`, `[ folder, path ]`, `[ folder, src ]`, `[ folder, destination ]`, `[ folder, created_at ]`, `[ folder, user_id ]`, `[ folder, username ]`, `[ path, src ]`, `[ path, destination ]`, `[ path, created_at ]`, `[ path, user_id ]`, `[ path, username ]`, `[ src, destination ]`, `[ src, created_at ]`, `[ src, user_id ]`, `[ src, username ]`, `[ destination, created_at ]`, `[ destination, user_id ]`, `[ destination, username ]`, `[ created_at, user_id ]`, `[ created_at, username ]`, `[ user_id, username ]`, `[ folder, path, src ]`, `[ folder, path, destination ]`, `[ folder, path, created_at ]`, `[ folder, path, user_id ]`, `[ folder, path, username ]`, `[ folder, src, destination ]`, `[ folder, src, created_at ]`, `[ folder, src, user_id ]`, `[ folder, src, username ]`, `[ folder, destination, created_at ]`, `[ folder, destination, user_id ]`, `[ folder, destination, username ]`, `[ folder, created_at, user_id ]`, `[ folder, created_at, username ]`, `[ folder, user_id, username ]`, `[ path, src, destination ]`, `[ path, src, created_at ]`, `[ path, src, user_id ]`, `[ path, src, username ]`, `[ path, destination, created_at ]`, `[ path, destination, user_id ]`, `[ path, destination, username ]`, `[ path, created_at, user_id ]`, `[ path, created_at, username ]`, `[ path, user_id, username ]`, `[ src, destination, created_at ]`, `[ src, destination, user_id ]`, `[ src, destination, username ]`, `[ src, created_at, user_id ]`, `[ src, created_at, username ]`, `[ src, user_id, username ]`, `[ destination, created_at, user_id ]`, `[ destination, created_at, username ]`, `[ destination, user_id, username ]`, `[ created_at, user_id, username ]`, `[ folder, path, src, destination ]`, `[ folder, path, src, created_at ]`, `[ folder, path, src, user_id ]`, `[ folder, path, src, username ]`, `[ folder, path, destination, created_at ]`, `[ folder, path, destination, user_id ]`, `[ folder, path, destination, username ]`, `[ folder, path, created_at, user_id ]`, `[ folder, path, created_at, username ]`, `[ folder, path, user_id, username ]`, `[ folder, src, destination, created_at ]`, `[ folder, src, destination, user_id ]`, `[ folder, src, destination, username ]`, `[ folder, src, created_at, user_id ]`, `[ folder, src, created_at, username ]`, `[ folder, src, user_id, username ]`, `[ folder, destination, created_at, user_id ]`, `[ folder, destination, created_at, username ]`, `[ folder, destination, user_id, username ]`, `[ folder, created_at, user_id, username ]`, `[ path, src, destination, created_at ]`, `[ path, src, destination, user_id ]`, `[ path, src, destination, username ]`, `[ path, src, created_at, user_id ]`, `[ path, src, created_at, username ]`, `[ path, src, user_id, username ]`, `[ path, destination, created_at, user_id ]`, `[ path, destination, created_at, username ]`, `[ path, destination, user_id, username ]`, `[ path, created_at, user_id, username ]`, `[ src, destination, created_at, user_id ]`, `[ src, destination, created_at, username ]`, `[ src, destination, user_id, username ]`, `[ src, created_at, user_id, username ]`, `[ destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at ]`, `[ folder, path, src, destination, user_id ]`, `[ folder, path, src, destination, username ]`, `[ folder, path, src, created_at, user_id ]`, `[ folder, path, src, created_at, username ]`, `[ folder, path, src, user_id, username ]`, `[ folder, path, destination, created_at, user_id ]`, `[ folder, path, destination, created_at, username ]`, `[ folder, path, destination, user_id, username ]`, `[ folder, path, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id ]`, `[ folder, src, destination, created_at, username ]`, `[ folder, src, destination, user_id, username ]`, `[ folder, src, created_at, user_id, username ]`, `[ folder, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id ]`, `[ path, src, destination, created_at, username ]`, `[ path, src, destination, user_id, username ]`, `[ path, src, created_at, user_id, username ]`, `[ path, destination, created_at, user_id, username ]`, `[ src, destination, created_at, user_id, username ]`, `[ folder, path, src, destination, created_at, user_id ]`, `[ folder, path, src, destination, created_at, username ]`, `[ folder, path, src, destination, user_id, username ]`, `[ folder, path, src, created_at, user_id, username ]`, `[ folder, path, destination, created_at, user_id, username ]`, `[ folder, src, destination, created_at, user_id, username ]`, `[ path, src, destination, created_at, user_id, username ]` or `[ folder, path, src, destination, created_at, user_id, username ]`.
        /// </summary>
        public static FilesList<ActionLog> List(

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
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            return new FilesList<ActionLog>($"/action_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<ActionLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}