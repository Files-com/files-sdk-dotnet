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
                this.attributes.Add("ask_about_overwrites", null);
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
                this.attributes.Add("parallel_parts", null);
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
        [JsonPropertyName("send")]
        public object Send
        {
            
            get;
            set;
        }

        /// <summary>
        /// Type of upload
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            
            get;
            set;
        }

        /// <summary>
        /// If `true`, this file exists and you may wish to ask the user for overwrite confirmation
        /// </summary>
        [JsonPropertyName("ask_about_overwrites")]
        public bool AskAboutOverwrites
        {
            
            get;
            set;
        }

        /// <summary>
        /// Number of parts in the upload
        /// </summary>
        [JsonPropertyName("available_parts")]
        public Nullable<Int64> AvailableParts
        {
            
            get;
            set;
        }

        /// <summary>
        /// Date/time of when this Upload part expires and the URL cannot be used any more
        /// </summary>
        [JsonPropertyName("expires")]
        public string Expires
        {
            
            get;
            set;
        }

        /// <summary>
        /// Additional upload headers to provide as part of the upload
        /// </summary>
        [JsonPropertyName("headers")]
        public object Headers
        {
            
            get;
            set;
        }

        /// <summary>
        /// HTTP Method to use for uploading the part, usually `PUT`
        /// </summary>
        [JsonPropertyName("http_method")]
        public string HttpMethod
        {
            
            get;
            set;
        }

        /// <summary>
        /// Size in bytes for this part
        /// </summary>
        [JsonPropertyName("next_partsize")]
        public Nullable<Int64> NextPartsize
        {
            
            get;
            set;
        }

        /// <summary>
        /// If `true`, multiple parts may be uploaded in parallel.  If `false`, be sure to only upload one part at a time, in order.
        /// </summary>
        [JsonPropertyName("parallel_parts")]
        public bool ParallelParts
        {
            
            get;
            set;
        }

        /// <summary>
        /// Additional HTTP parameters to send with the upload
        /// </summary>
        [JsonPropertyName("parameters")]
        public object Parameters
        {
            
            get;
            set;
        }

        /// <summary>
        /// Number of this upload part
        /// </summary>
        [JsonPropertyName("part_number")]
        public Nullable<Int64> PartNumber
        {
            
            get;
            set;
        }

        /// <summary>
        /// Size in bytes for the next upload part
        /// </summary>
        [JsonPropertyName("partsize")]
        public Nullable<Int64> Partsize
        {
            
            get;
            set;
        }

        /// <summary>
        /// New file path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            
            get;
            set;
        }

        /// <summary>
        /// Reference name for this upload part
        /// </summary>
        [JsonPropertyName("ref")]
        public string Ref
        {
            
            get;
            set;
        }

        /// <summary>
        /// URI to upload this part to
        /// </summary>
        [JsonPropertyName("upload_uri")]
        public string UploadUri
        {
            
            get;
            set;
        }



    }
}

