using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Preview
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Preview()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("status", null);
            this.attributes.Add("download_uri", null);
            this.attributes.Add("type", null);
            this.attributes.Add("size", null);
        }

        public Preview(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Preview ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// Preview status.  Can be invalid, not_generated, generating, complete, or file_too_large
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string) attributes["status"]; }
        }

        /// <summary>
        /// Link to download preview
        /// </summary>
        [JsonPropertyName("download_uri")]
        public string DownloadUri
        {
            get { return (string) attributes["download_uri"]; }
        }

        /// <summary>
        /// Preview status.  Can be invalid, not_generated, generating, complete, or file_too_large
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string) attributes["type"]; }
        }

        /// <summary>
        /// Preview size
        /// </summary>
        [JsonPropertyName("size")]
        public Nullable<Int64> Size
        {
            get { return (Nullable<Int64>) attributes["size"]; }
        }



    }
}

