using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class FileMigration
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FileMigration() : this(null, null) { }

        public FileMigration(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("dest_path"))
            {
                this.attributes.Add("dest_path", null);
            }
            if (!this.attributes.ContainsKey("files_moved"))
            {
                this.attributes.Add("files_moved", null);
            }
            if (!this.attributes.ContainsKey("files_total"))
            {
                this.attributes.Add("files_total", null);
            }
            if (!this.attributes.ContainsKey("operation"))
            {
                this.attributes.Add("operation", null);
            }
            if (!this.attributes.ContainsKey("region"))
            {
                this.attributes.Add("region", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("log_url"))
            {
                this.attributes.Add("log_url", null);
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
        /// File migration ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// Source path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
        }

        /// <summary>
        /// Destination path
        /// </summary>
        [JsonPropertyName("dest_path")]
        public string DestPath
        {
            get { return (string) attributes["dest_path"]; }
        }

        /// <summary>
        /// Number of files processed
        /// </summary>
        [JsonPropertyName("files_moved")]
        public Nullable<Int64> FilesMoved
        {
            get { return (Nullable<Int64>) attributes["files_moved"]; }
        }

        /// <summary>
        /// Total number of files to process
        /// </summary>
        [JsonPropertyName("files_total")]
        public Nullable<Int64> FilesTotal
        {
            get { return (Nullable<Int64>) attributes["files_total"]; }
        }

        /// <summary>
        /// The type of operation
        /// </summary>
        [JsonPropertyName("operation")]
        public string Operation
        {
            get { return (string) attributes["operation"]; }
        }

        /// <summary>
        /// Region
        /// </summary>
        [JsonPropertyName("region")]
        public string Region
        {
            get { return (string) attributes["region"]; }
        }

        /// <summary>
        /// Status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string) attributes["status"]; }
        }

        /// <summary>
        /// Link to download the log file for this migration.
        /// </summary>
        [JsonPropertyName("log_url")]
        public string LogUrl
        {
            get { return (string) attributes["log_url"]; }
        }



        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - File Migration ID.
        /// </summary>
        public static async Task<FileMigration> Find(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/file_migrations/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<FileMigration>(responseJson);
        }

        public static async Task<FileMigration> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

    }
}

