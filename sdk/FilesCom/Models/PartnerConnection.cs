using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// A PartnerConnection object of the Files.com API.
    /// </summary>
    public class PartnerConnection : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a PartnerConnection with no attributes set and no options.
        /// </summary>
        public PartnerConnection() : this(null, null) { }

        /// <summary>
        /// Makes a PartnerConnection with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public PartnerConnection(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

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

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => null;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[0]; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
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