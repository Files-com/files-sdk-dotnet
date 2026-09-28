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
    public class Style : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
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
            if (!this.attributes.ContainsKey("logo_click_href"))
            {
                this.attributes.Add("logo_click_href", null);
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

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options)
        {
            this.client = client;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[] { Logo, Thumbnail }; }
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
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Folder path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Logo
        /// </summary>
        [JsonPropertyName("logo")]
        public Image Logo
        {
            get { return (Image)attributes["logo"]; }
            set { attributes["logo"] = value; }
        }

        /// <summary>
        /// URL to open when a public visitor clicks the logo
        /// </summary>
        [JsonPropertyName("logo_click_href")]
        public string LogoClickHref
        {
            get { return (string)attributes["logo_click_href"]; }
            set { attributes["logo_click_href"] = value; }
        }

        /// <summary>
        /// Logo thumbnail
        /// </summary>
        [JsonPropertyName("thumbnail")]
        public Image Thumbnail
        {
            get { return (Image)attributes["thumbnail"]; }
            set { attributes["thumbnail"] = value; }
        }

        /// <summary>
        /// Logo for custom branding. Required when creating a new style.
        /// </summary>
        [JsonPropertyName("file")]
        public System.Net.Http.ByteArrayContent File
        {
            get { return (System.Net.Http.ByteArrayContent)attributes["file"]; }
            set { attributes["file"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   file - file - Logo for custom branding. Required when creating a new style.
        ///   logo_click_href - string - URL to open when a public visitor clicks the logo.
        /// </summary>
        public Task<Style> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   file - file - Logo for custom branding. Required when creating a new style.
        ///   logo_click_href - string - URL to open when a public visitor clicks the logo.
        /// </summary>
        public Task<Style> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Style> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("file") && !(parameters["file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: file must be of type System.Net.Http.ByteArrayContent", "parameters[\"file\"]");
            }
            if (parameters.ContainsKey("logo_click_href") && !(parameters["logo_click_href"] is string))
            {
                throw new ArgumentException("Bad parameter: logo_click_href must be of type string", "parameters[\"logo_click_href\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/styles/{System.Uri.EscapeDataString(attributes["path"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<Style>(responseJson, context.Client, requestOptions);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// </summary>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// </summary>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path"))
            {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client));
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/styles/{System.Uri.EscapeDataString(attributes["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            response.Dispose();
        }


        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await UpdateCore(this.attributes, cancellationToken);
        }


        /// <summary>
        /// Parameters:
        ///   path (required) - string - Style path.
        /// </summary>
        public static Task<Style> Find(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        public static Task<Style> Get(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(path, parameters, options);
        }

        internal static async Task<Style> FindCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/styles/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<Style>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// Parameters:
        ///   file - file - Logo for custom branding. Required when creating a new style.
        ///   logo_click_href - string - URL to open when a public visitor clicks the logo.
        /// </summary>
        public static Task<Style> Update(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        internal static async Task<Style> UpdateCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("file") && !(parameters["file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: file must be of type System.Net.Http.ByteArrayContent", "parameters[\"file\"]");
            }
            if (parameters.ContainsKey("logo_click_href") && !(parameters["logo_click_href"] is string))
            {
                throw new ArgumentException("Bad parameter: logo_click_href must be of type string", "parameters[\"logo_click_href\"]");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/styles/{System.Uri.EscapeDataString(parameters["path"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            try
            {
                return JsonUtil.DeserializeWithOptions<Style>(responseJson, context.Client, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        /// <summary>
        /// </summary>
        public static Task Delete(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), path, parameters, options, CancellationToken.None);
        }

        public static Task Destroy(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(path, parameters, options);
        }

        internal static async Task DeleteCore(
            OperationContext context,
            string path,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            HttpResponseMessage response = await FilesClient.SendRequest(context, $"/styles/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
            response.Dispose();
        }

    }
}