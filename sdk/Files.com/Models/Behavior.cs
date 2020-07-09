using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Behavior
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Behavior()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("path", null);
            this.attributes.Add("attachment_url", null);
            this.attributes.Add("behavior", null);
            this.attributes.Add("value", null);
            this.attributes.Add("attachment_file", null);
        }

        public Behavior(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Folder behavior ID
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
        /// URL for attached file
        /// </summary>
        [JsonPropertyName("attachment_url")]
        public string AttachmentUrl
        {
            get { return (string) attributes["attachment_url"]; }
            set { attributes["attachment_url"] = value; }
        }

        /// <summary>
        /// Behavior type.
        /// </summary>
        [JsonPropertyName("behavior")]
        public string BehaviorType
        {
            get { return (string) attributes["behavior"]; }
            set { attributes["behavior"] = value; }
        }

        /// <summary>
        /// Settings for this behavior.  See the section above for an example value to provide here.  Formatting is different for each Behavior type.  May be sent as nested JSON or a single JSON-encoded string.  If using XML encoding for the API call, this data must be sent as a JSON-encoded string.
        /// </summary>
        [JsonPropertyName("value")]
        public object Value
        {
            get { return (object) attributes["value"]; }
            set { attributes["value"] = value; }
        }

        /// <summary>
        /// Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
        /// </summary>
        [JsonPropertyName("attachment_file")]
        public System.Net.Http.ByteArrayContent AttachmentFile
        {
            get { return (System.Net.Http.ByteArrayContent) attributes["attachment_file"]; }
            set { attributes["attachment_file"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   value - string - The value of the folder behavior.  Can be a integer, array, or hash depending on the type of folder behavior.
        ///   attachment_file - file - Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
        ///   behavior - string - Behavior type.
        ///   path - string - Folder behaviors path.
        /// </summary>
        public async Task<Behavior> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id")) {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("value") && !(parameters["value"] is string ))
            {
                throw new ArgumentException("Bad parameter: value must be of type string", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("attachment_file") && !(parameters["attachment_file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"attachment_file\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string ))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{attributes["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Behavior>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Behavior> Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id")) {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Behavior>(responseJson);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                await this.Update(this.attributes);
            }
            else
            {
                // TODO: Remove this cast after return types are implemented
                var newObj = (Behavior) await Behavior.Create(this.attributes, this.options);
                this.attributes = newObj.attributes;
            }
        }

        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        ///   behavior - string - If set, only shows folder behaviors matching this behavior type.
        /// </summary>
        public static async Task<Behavior[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string ))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Behavior[]>(responseJson);
        }

        public static async Task<Behavior[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        ///   path (required) - string - Path to operate on.
        ///   recursive - string - Show behaviors above this path?
        ///   behavior - string - If set only shows folder behaviors matching this behavior type.
        /// </summary>
        public static async Task<Behavior[]> ListFor(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is string ))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type string", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string ))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/folders/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Behavior[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Behavior ID.
        /// </summary>
        public static async Task<Behavior> Find(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Behavior>(responseJson);
        }

        public static async Task<Behavior> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   value - string - The value of the folder behavior.  Can be a integer, array, or hash depending on the type of folder behavior.
        ///   attachment_file - file - Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
        ///   path (required) - string - Folder behaviors path.
        ///   behavior (required) - string - Behavior type.
        /// </summary>
        public static async Task<Behavior> Create(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("value") && !(parameters["value"] is string ))
            {
                throw new ArgumentException("Bad parameter: value must be of type string", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("attachment_file") && !(parameters["attachment_file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"attachment_file\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string ))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("behavior") || parameters["behavior"] == null)
            {
                throw new ArgumentNullException("Parameter missing: behavior", "parameters[\"behavior\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Behavior>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   url (required) - string - URL for testing the webhook.
        ///   method - string - HTTP method(GET or POST).
        ///   encoding - string - HTTP encoding method.  Can be JSON, XML, or RAW (form data).
        ///   headers - object - Additional request headers.
        ///   body - object - Additional body parameters.
        ///   action - string - action for test body
        /// </summary>
        public static async Task<Behavior> WebhookTest(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("url") && !(parameters["url"] is string ))
            {
                throw new ArgumentException("Bad parameter: url must be of type string", "parameters[\"url\"]");
            }
            if (parameters.ContainsKey("method") && !(parameters["method"] is string ))
            {
                throw new ArgumentException("Bad parameter: method must be of type string", "parameters[\"method\"]");
            }
            if (parameters.ContainsKey("encoding") && !(parameters["encoding"] is string ))
            {
                throw new ArgumentException("Bad parameter: encoding must be of type string", "parameters[\"encoding\"]");
            }
            if (parameters.ContainsKey("headers") && !(parameters["headers"] is object ))
            {
                throw new ArgumentException("Bad parameter: headers must be of type object", "parameters[\"headers\"]");
            }
            if (parameters.ContainsKey("body") && !(parameters["body"] is object ))
            {
                throw new ArgumentException("Bad parameter: body must be of type object", "parameters[\"body\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (!parameters.ContainsKey("url") || parameters["url"] == null)
            {
                throw new ArgumentNullException("Parameter missing: url", "parameters[\"url\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/webhook/test", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Behavior>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   value - string - The value of the folder behavior.  Can be a integer, array, or hash depending on the type of folder behavior.
        ///   attachment_file - file - Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
        ///   behavior - string - Behavior type.
        ///   path - string - Folder behaviors path.
        /// </summary>
        public static async Task<Behavior> Update(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("value") && !(parameters["value"] is string ))
            {
                throw new ArgumentException("Bad parameter: value must be of type string", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("attachment_file") && !(parameters["attachment_file"] is System.Net.Http.ByteArrayContent ))
            {
                throw new ArgumentException("Bad parameter: attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"attachment_file\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string ))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{parameters["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Behavior>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Behavior> Delete(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Behavior>(responseJson);
        }

        public static async Task<Behavior> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

