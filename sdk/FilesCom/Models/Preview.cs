using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Preview
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Preview() : this(null, null) { }

        public Preview(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("download_uri"))
            {
                this.attributes.Add("download_uri", null);
            }
            if (!this.attributes.ContainsKey("type"))
            {
                this.attributes.Add("type", null);
            }
            if (!this.attributes.ContainsKey("size"))
            {
                this.attributes.Add("size", null);
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
        /// Preview ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            
            get;
            set;
        }

        /// <summary>
        /// Preview status.  Can be invalid, not_generated, generating, complete, or file_too_large
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            
            get;
            set;
        }

        /// <summary>
        /// Link to download preview
        /// </summary>
        [JsonPropertyName("download_uri")]
        public string DownloadUri
        {
            
            get;
            set;
        }

        /// <summary>
        /// Preview status.  Can be invalid, not_generated, generating, complete, or file_too_large
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            
            get;
            set;
        }

        /// <summary>
        /// Preview size
        /// </summary>
        [JsonPropertyName("size")]
        public Nullable<Int64> Size
        {
            
            get;
            set;
        }



    }
}

