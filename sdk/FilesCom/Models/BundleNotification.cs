using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class BundleNotification
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public BundleNotification() : this(null, null) { }

        public BundleNotification(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("bundle_id"))
            {
                this.attributes.Add("bundle_id", null);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("notify_on_registration"))
            {
                this.attributes.Add("notify_on_registration", false);
            }
            if (!this.attributes.ContainsKey("notify_on_upload"))
            {
                this.attributes.Add("notify_on_upload", false);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
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
        /// Bundle ID to notify on
        /// </summary>
        [JsonPropertyName("bundle_id")]
        public Nullable<Int64> BundleId
        {
            get { return (Nullable<Int64>)attributes["bundle_id"]; }
            set { attributes["bundle_id"] = value; }
        }

        /// <summary>
        /// Bundle Notification ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Triggers bundle notification when a registration action occurs for it.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_registration")]
        public bool NotifyOnRegistration
        {
            get { return attributes["notify_on_registration"] == null ? false : (bool)attributes["notify_on_registration"]; }
            set { attributes["notify_on_registration"] = value; }
        }

        /// <summary>
        /// Triggers bundle notification when a upload action occurs for it.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_upload")]
        public bool NotifyOnUpload
        {
            get { return attributes["notify_on_upload"] == null ? false : (bool)attributes["notify_on_upload"]; }
            set { attributes["notify_on_upload"] = value; }
        }

        /// <summary>
        /// The id of the user to notify.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   notify_on_registration - boolean - Triggers bundle notification when a registration action occurs for it.
        ///   notify_on_upload - boolean - Triggers bundle notification when a upload action occurs for it.
        /// </summary>
        public async Task<BundleNotification> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("notify_on_registration") && !(parameters["notify_on_registration"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_registration must be of type bool", "parameters[\"notify_on_registration\"]");
            }
            if (parameters.ContainsKey("notify_on_upload") && !(parameters["notify_on_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_upload must be of type bool", "parameters[\"notify_on_upload\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/bundle_notifications/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<BundleNotification>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/bundle_notifications/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await BundleNotification.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[bundle_id]=desc`). Valid fields are `bundle_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `bundle_id`.
        /// </summary>
        public static FilesList<BundleNotification> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
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

            return new FilesList<BundleNotification>($"/bundle_notifications", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<BundleNotification> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Bundle Notification ID.
        /// </summary>
        public static async Task<BundleNotification> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/bundle_notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<BundleNotification>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<BundleNotification> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - The id of the user to notify.
        ///   notify_on_registration - boolean - Triggers bundle notification when a registration action occurs for it.
        ///   notify_on_upload - boolean - Triggers bundle notification when a upload action occurs for it.
        ///   bundle_id (required) - int64 - Bundle ID to notify on
        /// </summary>
        public static async Task<BundleNotification> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("notify_on_registration") && !(parameters["notify_on_registration"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_registration must be of type bool", "parameters[\"notify_on_registration\"]");
            }
            if (parameters.ContainsKey("notify_on_upload") && !(parameters["notify_on_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_upload must be of type bool", "parameters[\"notify_on_upload\"]");
            }
            if (parameters.ContainsKey("bundle_id") && !(parameters["bundle_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: bundle_id must be of type Nullable<Int64>", "parameters[\"bundle_id\"]");
            }
            if (!parameters.ContainsKey("bundle_id") || parameters["bundle_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: bundle_id", "parameters[\"bundle_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/bundle_notifications", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<BundleNotification>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   notify_on_registration - boolean - Triggers bundle notification when a registration action occurs for it.
        ///   notify_on_upload - boolean - Triggers bundle notification when a upload action occurs for it.
        /// </summary>
        public static async Task<BundleNotification> Update(
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
            if (parameters.ContainsKey("notify_on_registration") && !(parameters["notify_on_registration"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_registration must be of type bool", "parameters[\"notify_on_registration\"]");
            }
            if (parameters.ContainsKey("notify_on_upload") && !(parameters["notify_on_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_upload must be of type bool", "parameters[\"notify_on_upload\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/bundle_notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<BundleNotification>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public static async Task Delete(
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

            await FilesClient.SendRequest($"/bundle_notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(id, parameters, options);
        }

    }
}