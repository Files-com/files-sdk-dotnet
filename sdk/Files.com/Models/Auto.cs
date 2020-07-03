using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Auto
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Auto()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("dynamic", null);
        }

        public Auto(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// </summary>
        [JsonPropertyName("dynamic")]
        public object Dynamic
        {
            get { return (object) attributes["dynamic"]; }
        }



    }
}

