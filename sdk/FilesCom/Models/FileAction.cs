using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class FileAction
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FileAction() : this(null, null) { }

        public FileAction(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("file_migration_id"))
            {
                this.attributes.Add("file_migration_id", null);
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
        /// Status of file operation.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// If status is pending, this is the id of the File Migration to check for status updates.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_migration_id")]
        public Nullable<Int64> FileMigrationId
        {
            get { return (Nullable<Int64>)attributes["file_migration_id"]; }
            private set { attributes["file_migration_id"] = value; }
        }



    }
}