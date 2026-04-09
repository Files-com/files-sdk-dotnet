using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class SyncRunLiveTransfer
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public SyncRunLiveTransfer() : this(null, null) { }

        public SyncRunLiveTransfer(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("bytes_copied"))
            {
                this.attributes.Add("bytes_copied", null);
            }
            if (!this.attributes.ContainsKey("bytes_total"))
            {
                this.attributes.Add("bytes_total", null);
            }
            if (!this.attributes.ContainsKey("percentage"))
            {
                this.attributes.Add("percentage", null);
            }
            if (!this.attributes.ContainsKey("eta"))
            {
                this.attributes.Add("eta", null);
            }
            if (!this.attributes.ContainsKey("started_at"))
            {
                this.attributes.Add("started_at", null);
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
        /// The file path being transferred. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// Status of this individual transfer
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Bytes transferred so far
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes_copied")]
        public Nullable<Int64> BytesCopied
        {
            get { return (Nullable<Int64>)attributes["bytes_copied"]; }
            private set { attributes["bytes_copied"] = value; }
        }

        /// <summary>
        /// Total bytes of the file being transferred
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes_total")]
        public Nullable<Int64> BytesTotal
        {
            get { return (Nullable<Int64>)attributes["bytes_total"]; }
            private set { attributes["bytes_total"] = value; }
        }

        /// <summary>
        /// Transfer progress from 0.0 to 1.0
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("percentage")]
        public Nullable<double> Percentage
        {
            get { return (Nullable<double>)attributes["percentage"]; }
            private set { attributes["percentage"] = value; }
        }

        /// <summary>
        /// Estimated time remaining (human-readable)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("eta")]
        public string Eta
        {
            get { return (string)attributes["eta"]; }
            private set { attributes["eta"] = value; }
        }

        /// <summary>
        /// When this individual transfer started
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("started_at")]
        public string StartedAt
        {
            get { return (string)attributes["started_at"]; }
            private set { attributes["started_at"] = value; }
        }



    }
}