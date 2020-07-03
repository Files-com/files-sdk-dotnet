using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Style
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Style()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("path", null);
            this.attributes.Add("logo", null);
            this.attributes.Add("thumbnail", null);
            this.attributes.Add("file", null);
        }

        public Style(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
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

            string responseJson = await FilesClient.SendRequest($"/styles/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Patch, parameters, options);

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

            string responseJson = await FilesClient.SendRequest($"/styles/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

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
        public static async Task<Style> List(
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

            string responseJson = await FilesClient.SendRequest($"/styles/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Style>(responseJson);
        }

        public static async Task<Style> All(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(path, parameters, options);
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

            string responseJson = await FilesClient.SendRequest($"/styles/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Patch, parameters, options);

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

            string responseJson = await FilesClient.SendRequest($"/styles/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

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

