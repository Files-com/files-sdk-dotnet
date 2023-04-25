using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Behavior
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Behavior() : this(null, null) { }

        public Behavior(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("attachment_url"))
            {
                this.attributes.Add("attachment_url", null);
            }
            if (!this.attributes.ContainsKey("behavior"))
            {
                this.attributes.Add("behavior", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("value"))
            {
                this.attributes.Add("value", null);
            }
            if (!this.attributes.ContainsKey("attachment_file"))
            {
                this.attributes.Add("attachment_file", null);
            }
            if (!this.attributes.ContainsKey("attachment_delete"))
            {
                this.attributes.Add("attachment_delete", null);
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
        /// Folder behavior ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Folder path.  Note that Behavior paths cannot be updated once initially set.  You will need to remove and re-create the behavior on the new path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// URL for attached file
        /// </summary>
        [JsonPropertyName("attachment_url")]
        public string AttachmentUrl
        {
            get { return (string)attributes["attachment_url"]; }
            set { attributes["attachment_url"] = value; }
        }

        /// <summary>
        /// Behavior type.
        /// </summary>
        [JsonPropertyName("behavior")]
        public string BehaviorType
        {
            get { return (string)attributes["behavior"]; }
            set { attributes["behavior"] = value; }
        }

        /// <summary>
        /// Name for this behavior.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Description for this behavior.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; }
        }

        /// <summary>
        /// Settings for this behavior.  See the section above for an example value to provide here.  Formatting is different for each Behavior type.  May be sent as nested JSON or a single JSON-encoded string.  If using XML encoding for the API call, this data must be sent as a JSON-encoded string.
        /// </summary>
        [JsonPropertyName("value")]
        public object Value
        {
            get { return (object)attributes["value"]; }
            set { attributes["value"] = value; }
        }

        /// <summary>
        /// Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
        /// </summary>
        [JsonPropertyName("attachment_file")]
        public System.Net.Http.ByteArrayContent AttachmentFile
        {
            get { return (System.Net.Http.ByteArrayContent)attributes["attachment_file"]; }
            set { attributes["attachment_file"] = value; }
        }

        /// <summary>
        /// If true, will delete the file stored in attachment
        /// </summary>
        [JsonPropertyName("attachment_delete")]
        public bool AttachmentDelete
        {
            get { return (bool)attributes["attachment_delete"]; }
            set { attributes["attachment_delete"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   value - string - The value of the folder behavior.  Can be a integer, array, or hash depending on the type of folder behavior. See The Behavior Types section for example values for each type of behavior.
        ///   attachment_file - file - Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
        ///   name - string - Name for this behavior.
        ///   description - string - Description for this behavior.
        ///   behavior - string - Behavior type.
        ///   path - string - Folder behaviors path.
        ///   attachment_delete - boolean - If true, will delete the file stored in attachment
        /// </summary>
        public async Task<Behavior> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("value") && !(parameters["value"] is string))
            {
                throw new ArgumentException("Bad parameter: value must be of type string", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("attachment_file") && !(parameters["attachment_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"attachment_file\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("attachment_delete") && !(parameters["attachment_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: attachment_delete must be of type bool", "parameters[\"attachment_delete\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Behavior>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Behavior> Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

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
                var newObj = await Behavior.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[behavior]=desc`). Valid fields are `behavior`.
        ///   behavior - string - If set, return records where the specified field is equal to the supplied value.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `behavior`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `behavior`.
        /// </summary>
        public static async Task<Behavior[]> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
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
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

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
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[behavior]=desc`). Valid fields are `behavior`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `behavior`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `behavior`.
        ///   path (required) - string - Path to operate on.
        ///   recursive - string - Show behaviors above this path?
        ///   behavior - string - DEPRECATED: If set only shows folder behaviors matching this behavior type. Use `filter[behavior]` instead.
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
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is string))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type string", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/folders/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Behavior[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   value - string - The value of the folder behavior.  Can be a integer, array, or hash depending on the type of folder behavior. See The Behavior Types section for example values for each type of behavior.
        ///   attachment_file - file - Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
        ///   name - string - Name for this behavior.
        ///   description - string - Description for this behavior.
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

            if (parameters.ContainsKey("value") && !(parameters["value"] is string))
            {
                throw new ArgumentException("Bad parameter: value must be of type string", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("attachment_file") && !(parameters["attachment_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"attachment_file\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string))
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

            if (parameters.ContainsKey("url") && !(parameters["url"] is string))
            {
                throw new ArgumentException("Bad parameter: url must be of type string", "parameters[\"url\"]");
            }
            if (parameters.ContainsKey("method") && !(parameters["method"] is string))
            {
                throw new ArgumentException("Bad parameter: method must be of type string", "parameters[\"method\"]");
            }
            if (parameters.ContainsKey("encoding") && !(parameters["encoding"] is string))
            {
                throw new ArgumentException("Bad parameter: encoding must be of type string", "parameters[\"encoding\"]");
            }
            if (parameters.ContainsKey("headers") && !(parameters["headers"] is object))
            {
                throw new ArgumentException("Bad parameter: headers must be of type object", "parameters[\"headers\"]");
            }
            if (parameters.ContainsKey("body") && !(parameters["body"] is object))
            {
                throw new ArgumentException("Bad parameter: body must be of type object", "parameters[\"body\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
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
        ///   value - string - The value of the folder behavior.  Can be a integer, array, or hash depending on the type of folder behavior. See The Behavior Types section for example values for each type of behavior.
        ///   attachment_file - file - Certain behaviors may require a file, for instance, the "watermark" behavior requires a watermark image
        ///   name - string - Name for this behavior.
        ///   description - string - Description for this behavior.
        ///   behavior - string - Behavior type.
        ///   path - string - Folder behaviors path.
        ///   attachment_delete - boolean - If true, will delete the file stored in attachment
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
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("value") && !(parameters["value"] is string))
            {
                throw new ArgumentException("Bad parameter: value must be of type string", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("attachment_file") && !(parameters["attachment_file"] is System.Net.Http.ByteArrayContent))
            {
                throw new ArgumentException("Bad parameter: attachment_file must be of type System.Net.Http.ByteArrayContent", "parameters[\"attachment_file\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("behavior") && !(parameters["behavior"] is string))
            {
                throw new ArgumentException("Bad parameter: behavior must be of type string", "parameters[\"behavior\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("attachment_delete") && !(parameters["attachment_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: attachment_delete must be of type bool", "parameters[\"attachment_delete\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

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
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/behaviors/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

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