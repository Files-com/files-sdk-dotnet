using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class FileUploadPart
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FileUploadPart() : this(null, null) { }

        public FileUploadPart(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("send"))
            {
                this.attributes.Add("send", null);
            }
            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("ask_about_overwrites"))
            {
                this.attributes.Add("ask_about_overwrites", false);
            }
            if (!this.attributes.ContainsKey("available_parts"))
            {
                this.attributes.Add("available_parts", null);
            }
            if (!this.attributes.ContainsKey("expires"))
            {
                this.attributes.Add("expires", null);
            }
            if (!this.attributes.ContainsKey("headers"))
            {
                this.attributes.Add("headers", null);
            }
            if (!this.attributes.ContainsKey("http_method"))
            {
                this.attributes.Add("http_method", null);
            }
            if (!this.attributes.ContainsKey("next_partsize"))
            {
                this.attributes.Add("next_partsize", null);
            }
            if (!this.attributes.ContainsKey("parallel_parts"))
            {
                this.attributes.Add("parallel_parts", false);
            }
            if (!this.attributes.ContainsKey("retry_parts"))
            {
                this.attributes.Add("retry_parts", false);
            }
            if (!this.attributes.ContainsKey("parameters"))
            {
                this.attributes.Add("parameters", null);
            }
            if (!this.attributes.ContainsKey("part_number"))
            {
                this.attributes.Add("part_number", null);
            }
            if (!this.attributes.ContainsKey("partsize"))
            {
                this.attributes.Add("partsize", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("ref"))
            {
                this.attributes.Add("ref", null);
            }
            if (!this.attributes.ContainsKey("upload_uri"))
            {
                this.attributes.Add("upload_uri", null);
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
        /// Content-Type and File to send
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("send")]
        public object Send
        {
            get { return (object)attributes["send"]; }
            private set { attributes["send"] = value; }
        }

        /// <summary>
        /// Type of upload
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            private set { attributes["action"] = value; }
        }

        /// <summary>
        /// If `true`, this file exists and you may wish to ask the user for overwrite confirmation
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ask_about_overwrites")]
        public bool AskAboutOverwrites
        {
            get { return attributes["ask_about_overwrites"] == null ? false : (bool)attributes["ask_about_overwrites"]; }
            private set { attributes["ask_about_overwrites"] = value; }
        }

        /// <summary>
        /// Number of parts in the upload
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("available_parts")]
        public Nullable<Int64> AvailableParts
        {
            get { return (Nullable<Int64>)attributes["available_parts"]; }
            private set { attributes["available_parts"] = value; }
        }

        /// <summary>
        /// Date/time of when this Upload part expires and the URL cannot be used any more
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("expires")]
        public string Expires
        {
            get { return (string)attributes["expires"]; }
            private set { attributes["expires"] = value; }
        }

        /// <summary>
        /// Additional upload headers to provide as part of the upload
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("headers")]
        public object Headers
        {
            get { return (object)attributes["headers"]; }
            private set { attributes["headers"] = value; }
        }

        /// <summary>
        /// HTTP Method to use for uploading the part, usually `PUT`
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_method")]
        public string HttpMethod
        {
            get { return (string)attributes["http_method"]; }
            private set { attributes["http_method"] = value; }
        }

        /// <summary>
        /// Size in bytes for this part
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("next_partsize")]
        public Nullable<Int64> NextPartsize
        {
            get { return (Nullable<Int64>)attributes["next_partsize"]; }
            private set { attributes["next_partsize"] = value; }
        }

        /// <summary>
        /// If `true`, multiple parts may be uploaded in parallel.  If `false`, be sure to only upload one part at a time, in order.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("parallel_parts")]
        public bool ParallelParts
        {
            get { return attributes["parallel_parts"] == null ? false : (bool)attributes["parallel_parts"]; }
            private set { attributes["parallel_parts"] = value; }
        }

        /// <summary>
        /// If `true`, parts may be retried. If `false`, a part cannot be retried and the upload should be restarted.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("retry_parts")]
        public bool RetryParts
        {
            get { return attributes["retry_parts"] == null ? false : (bool)attributes["retry_parts"]; }
            private set { attributes["retry_parts"] = value; }
        }

        /// <summary>
        /// Additional HTTP parameters to send with the upload
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("parameters")]
        public object Parameters
        {
            get { return (object)attributes["parameters"]; }
            private set { attributes["parameters"] = value; }
        }

        /// <summary>
        /// Number of this upload part
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("part_number")]
        public Nullable<Int64> PartNumber
        {
            get { return (Nullable<Int64>)attributes["part_number"]; }
            private set { attributes["part_number"] = value; }
        }

        /// <summary>
        /// Size in bytes for the next upload part
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("partsize")]
        public Nullable<Int64> Partsize
        {
            get { return (Nullable<Int64>)attributes["partsize"]; }
            private set { attributes["partsize"] = value; }
        }

        /// <summary>
        /// New file path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// Reference name for this upload part
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ref")]
        public string Ref
        {
            get { return (string)attributes["ref"]; }
            private set { attributes["ref"] = value; }
        }

        /// <summary>
        /// URI to upload this part to
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("upload_uri")]
        public string UploadUri
        {
            get { return (string)attributes["upload_uri"]; }
            private set { attributes["upload_uri"] = value; }
        }



    }
}