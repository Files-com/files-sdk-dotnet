using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Style
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Style() : this(null, null) { }

        public Style(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("logo"))
            {
                this.attributes.Add("logo", null);
            }
            if (!this.attributes.ContainsKey("thumbnail"))
            {
                this.attributes.Add("thumbnail", null);
            }
            if (!this.attributes.ContainsKey("file"))
            {
                this.attributes.Add("file", null);
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
        /// Style ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            
            get { return (string) attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Logo
        /// </summary>
        [JsonPropertyName("logo")]
        public object Logo
        {
            
            get { return (object) attributes["logo"]; }
            set { attributes["logo"] = value; }
        }

        /// <summary>
        /// Logo thumbnail
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public object Thumbnail
        {
            
            get { return (object) attributes["thumbnail"]; }
            set { attributes["thumbnail"] = value; }
        }

        /// <summary>
        /// Logo for custom branding.
        /// </summary>
        [JsonPropertyName("file")]
        public System.Net.Http.ByteArrayContent File
        {
            
            get { return (System.Net.Http.ByteArrayContent) attributes["file"]; }
            set { attributes["file"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   file (required) - file - Logo for custom branding.
        /// </summary>
        public async Task<Style> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path")) {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("file") && !(parameters["file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: file must be of type System.Net.Http.ByteArrayContent", "parameters[\"file\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("file") || parameters["file"] == null)
            {
                throw new ArgumentNullException("Parameter missing: file", "parameters[\"file\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/styles/{attributes["path"]}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Style>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Style> Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path")) {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/styles/{attributes["path"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Style>(responseJson);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            await Update(this.attributes);
        }


        /// <summary>
        /// Parameters:
        ///   path (required) - string - Style path.
        /// </summary>
        public static async Task<Style> Find(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/styles/{parameters["path"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Style>(responseJson);
        }

        public static async Task<Style> Get(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(path, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   file (required) - file - Logo for custom branding.
        /// </summary>
        public static async Task<Style> Update(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("file") && !(parameters["file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: file must be of type System.Net.Http.ByteArrayContent", "parameters[\"file\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("file") || parameters["file"] == null)
            {
                throw new ArgumentNullException("Parameter missing: file", "parameters[\"file\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/styles/{parameters["path"]}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Style>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Style> Delete(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/styles/{parameters["path"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Style>(responseJson);
        }

        public static async Task<Style> Destroy(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(path, parameters, options);
        }

    }
}

