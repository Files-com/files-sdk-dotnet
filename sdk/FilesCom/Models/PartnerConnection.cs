using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PartnerConnection : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PartnerConnection() : this(null, null) { }

        public PartnerConnection(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("role"))
            {
                this.attributes.Add("role", null);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
            }
            if (!this.attributes.ContainsKey("site_name"))
            {
                this.attributes.Add("site_name", null);
            }
            if (!this.attributes.ContainsKey("mount_path"))
            {
                this.attributes.Add("mount_path", null);
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

        void IModel.SetOptions(Dictionary<string, object> options)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// Relationship ID used with DELETE /partner_sites/:id to disconnect.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// This Partner's role in this connection. A host shares local files with the connected site; a guest accesses files shared by the connected site.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("role")]
        public string Role
        {
            get { return (string)attributes["role"]; }
            private set { attributes["role"] = value; }
        }

        /// <summary>
        /// ID of the connected site.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            private set { attributes["site_id"] = value; }
        }

        /// <summary>
        /// Name of the connected site.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("site_name")]
        public string SiteName
        {
            get { return (string)attributes["site_name"]; }
            private set { attributes["site_name"] = value; }
        }

        /// <summary>
        /// File API path to the connected Host's mount on this site when role is guest. Null when role is host. File access remains subject to the caller's permissions and the Host Partner's grants.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("mount_path")]
        public string MountPath
        {
            get { return (string)attributes["mount_path"]; }
            private set { attributes["mount_path"] = value; }
        }



    }
}