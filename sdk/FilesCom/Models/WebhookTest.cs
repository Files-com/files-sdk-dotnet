using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class WebhookTest
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public WebhookTest() : this(null, null) { }

        public WebhookTest(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("code"))
            {
                this.attributes.Add("code", null);
            }
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("data"))
            {
                this.attributes.Add("data", null);
            }
            if (!this.attributes.ContainsKey("success"))
            {
                this.attributes.Add("success", null);
            }
            if (!this.attributes.ContainsKey("url"))
            {
                this.attributes.Add("url", null);
            }
            if (!this.attributes.ContainsKey("method"))
            {
                this.attributes.Add("method", null);
            }
            if (!this.attributes.ContainsKey("encoding"))
            {
                this.attributes.Add("encoding", null);
            }
            if (!this.attributes.ContainsKey("headers"))
            {
                this.attributes.Add("headers", null);
            }
            if (!this.attributes.ContainsKey("body"))
            {
                this.attributes.Add("body", null);
            }
            if (!this.attributes.ContainsKey("raw_body"))
            {
                this.attributes.Add("raw_body", null);
            }
            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
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
        /// Status HTTP code
        /// </summary>
        [JsonPropertyName("code")]
        public Nullable<Int64> Code
        {
            get { return (Nullable<Int64>) attributes["code"]; }
            set { attributes["code"] = value; }
        }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string) attributes["message"]; }
            set { attributes["message"] = value; }
        }

        /// <summary>
        /// Status message
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string) attributes["status"]; }
            set { attributes["status"] = value; }
        }

        /// <summary>
        /// Additional data
        /// </summary>
        [JsonPropertyName("data")]
        public object Data
        {
            get { return (object) attributes["data"]; }
            set { attributes["data"] = value; }
        }

        /// <summary>
        /// The success status of the webhook test
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success
        {
            get { return (bool) attributes["success"]; }
            set { attributes["success"] = value; }
        }

        /// <summary>
        /// URL for testing the webhook.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url
        {
            get { return (string) attributes["url"]; }
            set { attributes["url"] = value; }
        }

        /// <summary>
        /// HTTP method(GET or POST).
        /// </summary>
        [JsonPropertyName("method")]
        public string Method
        {
            get { return (string) attributes["method"]; }
            set { attributes["method"] = value; }
        }

        /// <summary>
        /// HTTP encoding method.  Can be JSON, XML, or RAW (form data).
        /// </summary>
        [JsonPropertyName("encoding")]
        public string Encoding
        {
            get { return (string) attributes["encoding"]; }
            set { attributes["encoding"] = value; }
        }

        /// <summary>
        /// Additional request headers.
        /// </summary>
        [JsonPropertyName("headers")]
        public object Headers
        {
            get { return (object) attributes["headers"]; }
            set { attributes["headers"] = value; }
        }

        /// <summary>
        /// Additional body parameters.
        /// </summary>
        [JsonPropertyName("body")]
        public object Body
        {
            get { return (object) attributes["body"]; }
            set { attributes["body"] = value; }
        }

        /// <summary>
        /// raw body text
        /// </summary>
        [JsonPropertyName("raw_body")]
        public string RawBody
        {
            get { return (string) attributes["raw_body"]; }
            set { attributes["raw_body"] = value; }
        }

        /// <summary>
        /// action for test body
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string) attributes["action"]; }
            set { attributes["action"] = value; }
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The WebhookTest object doesn't support updates.");
            }
            else
            {
                var newObj = await WebhookTest.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   url (required) - string - URL for testing the webhook.
        ///   method - string - HTTP method(GET or POST).
        ///   encoding - string - HTTP encoding method.  Can be JSON, XML, or RAW (form data).
        ///   headers - object - Additional request headers.
        ///   body - object - Additional body parameters.
        ///   raw_body - string - raw body text
        ///   action - string - action for test body
        /// </summary>
        public static async Task<WebhookTest> Create(
            
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
            if (parameters.ContainsKey("raw_body") && !(parameters["raw_body"] is string ))
            {
                throw new ArgumentException("Bad parameter: raw_body must be of type string", "parameters[\"raw_body\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (!parameters.ContainsKey("url") || parameters["url"] == null)
            {
                throw new ArgumentNullException("Parameter missing: url", "parameters[\"url\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/webhook_tests", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<WebhookTest>(responseJson);
        }


    }
}

