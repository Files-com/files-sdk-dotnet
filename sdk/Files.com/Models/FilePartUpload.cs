using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class FilePartUpload
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FilePartUpload()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("send", null);
            this.attributes.Add("action", null);
            this.attributes.Add("ask_about_overwrites", false);
            this.attributes.Add("available_parts", null);
            this.attributes.Add("expires", null);
            this.attributes.Add("headers", null);
            this.attributes.Add("http_method", null);
            this.attributes.Add("next_partsize", null);
            this.attributes.Add("parameters", null);
            this.attributes.Add("part_number", null);
            this.attributes.Add("partsize", null);
            this.attributes.Add("path", null);
            this.attributes.Add("ref", null);
            this.attributes.Add("upload_uri", null);
        }

        public FilePartUpload(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Content-Type and File to send
        /// </summary>
        [JsonPropertyName("send")]
        public object Send
        {
            get { return (object) attributes["send"]; }
        }

        /// <summary>
        /// Type of upload
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string) attributes["action"]; }
        }

        /// <summary>
        /// If false, rename conflicting files instead of asking for overwrite confirmation
        /// </summary>
        [JsonPropertyName("ask_about_overwrites")]
        public bool AskAboutOverwrites
        {
            get { return (bool) attributes["ask_about_overwrites"]; }
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        [JsonPropertyName("available_parts")]
        public string AvailableParts
        {
            get { return (string) attributes["available_parts"]; }
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        [JsonPropertyName("expires")]
        public string Expires
        {
            get { return (string) attributes["expires"]; }
        }

        /// <summary>
        /// Additional upload headers
        /// </summary>
        [JsonPropertyName("headers")]
        public object Headers
        {
            get { return (object) attributes["headers"]; }
        }

        /// <summary>
        /// Upload method, usually POST
        /// </summary>
        [JsonPropertyName("http_method")]
        public string HttpMethod
        {
            get { return (string) attributes["http_method"]; }
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        [JsonPropertyName("next_partsize")]
        public string NextPartsize
        {
            get { return (string) attributes["next_partsize"]; }
        }

        /// <summary>
        /// Additional upload parameters
        /// </summary>
        [JsonPropertyName("parameters")]
        public string Parameters
        {
            get { return (string) attributes["parameters"]; }
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        [JsonPropertyName("part_number")]
        public string PartNumber
        {
            get { return (string) attributes["part_number"]; }
        }

        /// <summary>
        /// Currently unused
        /// </summary>
        [JsonPropertyName("partsize")]
        public string Partsize
        {
            get { return (string) attributes["partsize"]; }
        }

        /// <summary>
        /// Upload path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
        }

        /// <summary>
        /// Reference name for this upload part
        /// </summary>
        [JsonPropertyName("ref")]
        public string Ref
        {
            get { return (string) attributes["ref"]; }
        }

        /// <summary>
        /// URI to upload this part to
        /// </summary>
        [JsonPropertyName("upload_uri")]
        public string UploadUri
        {
            get { return (string) attributes["upload_uri"]; }
        }



    }
}

