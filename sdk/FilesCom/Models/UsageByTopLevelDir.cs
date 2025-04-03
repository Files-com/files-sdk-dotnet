using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class UsageByTopLevelDir
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public UsageByTopLevelDir() : this(null, null) { }

        public UsageByTopLevelDir(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("dir"))
            {
                this.attributes.Add("dir", null);
            }
            if (!this.attributes.ContainsKey("size"))
            {
                this.attributes.Add("size", null);
            }
            if (!this.attributes.ContainsKey("count"))
            {
                this.attributes.Add("count", null);
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
        /// Directory name
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("dir")]
        public string Dir
        {
            get { return (string)attributes["dir"]; }
            private set { attributes["dir"] = value; }
        }

        /// <summary>
        /// Usage
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("size")]
        public Nullable<Int64> Size
        {
            get { return (Nullable<Int64>)attributes["size"]; }
            private set { attributes["size"] = value; }
        }

        /// <summary>
        /// File count
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("count")]
        public Nullable<Int64> Count
        {
            get { return (Nullable<Int64>)attributes["count"]; }
            private set { attributes["count"] = value; }
        }



    }
}