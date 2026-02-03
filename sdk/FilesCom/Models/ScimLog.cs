using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class ScimLog
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public ScimLog() : this(null, null) { }

        public ScimLog(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("request_path"))
            {
                this.attributes.Add("request_path", null);
            }
            if (!this.attributes.ContainsKey("request_method"))
            {
                this.attributes.Add("request_method", null);
            }
            if (!this.attributes.ContainsKey("http_response_code"))
            {
                this.attributes.Add("http_response_code", null);
            }
            if (!this.attributes.ContainsKey("user_agent"))
            {
                this.attributes.Add("user_agent", null);
            }
            if (!this.attributes.ContainsKey("request_json"))
            {
                this.attributes.Add("request_json", null);
            }
            if (!this.attributes.ContainsKey("response_json"))
            {
                this.attributes.Add("response_json", null);
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
        /// The unique ID of this SCIM request.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// The date and time when this SCIM request occurred.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public string CreatedAt
        {
            get { return (string)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// The path portion of the URL requested.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_path")]
        public string RequestPath
        {
            get { return (string)attributes["request_path"]; }
            private set { attributes["request_path"] = value; }
        }

        /// <summary>
        /// The HTTP method used for this request.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_method")]
        public string RequestMethod
        {
            get { return (string)attributes["request_method"]; }
            private set { attributes["request_method"] = value; }
        }

        /// <summary>
        /// The HTTP response code returned for this request.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("http_response_code")]
        public string HttpResponseCode
        {
            get { return (string)attributes["http_response_code"]; }
            private set { attributes["http_response_code"] = value; }
        }

        /// <summary>
        /// The User-Agent header sent with the request.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_agent")]
        public string UserAgent
        {
            get { return (string)attributes["user_agent"]; }
            private set { attributes["user_agent"] = value; }
        }

        /// <summary>
        /// The JSON payload sent with the request.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("request_json")]
        public string RequestJson
        {
            get { return (string)attributes["request_json"]; }
            private set { attributes["request_json"] = value; }
        }

        /// <summary>
        /// The JSON payload returned in the response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("response_json")]
        public string ResponseJson
        {
            get { return (string)attributes["response_json"]; }
            private set { attributes["response_json"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at`.
        /// </summary>
        public static FilesList<ScimLog> List(

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

            return new FilesList<ScimLog>($"/scim_logs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<ScimLog> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Scim Log ID.
        /// </summary>
        public static async Task<ScimLog> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/scim_logs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ScimLog>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<ScimLog> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

    }
}