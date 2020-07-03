using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Image
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Image()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("name", null);
            this.attributes.Add("uri", null);
        }

        public Image(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Image name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string) attributes["name"]; }
        }

        /// <summary>
        /// Image URI
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri
        {
            get { return (string) attributes["uri"]; }
        }



    }
}

