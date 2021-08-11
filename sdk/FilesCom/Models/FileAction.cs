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

        [JsonPropertyName("ref")]
        public string Ref { get; set; }

        [JsonPropertyName("part_number")]
        public int PartNumber { get; set; }

        [JsonPropertyName("partsize")]
        public int Partsize { get; set; }

        [JsonPropertyName("http_method")]
        public string HttpMethod { get; set; }

        [JsonPropertyName("upload_uri")]
        public string UploadUri { get; set; }

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
        /// Status of file operation. Possible values: completed, enqueued.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string) attributes["status"]; }
        }

        /// <summary>
        /// If status is enqueued, this is the id of the FileMigration to check for status updates.
        /// </summary>
        [JsonPropertyName("file_migration_id")]
        public Nullable<Int64> FileMigrationId
        {
            get { return (Nullable<Int64>) attributes["file_migration_id"]; }
        }



    }
}

