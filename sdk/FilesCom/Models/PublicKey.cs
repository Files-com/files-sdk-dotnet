using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class PublicKey
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public PublicKey() : this(null, null) { }

        public PublicKey(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("title"))
            {
                this.attributes.Add("title", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("fingerprint"))
            {
                this.attributes.Add("fingerprint", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("public_key"))
            {
                this.attributes.Add("public_key", null);
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
        /// Public key ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Public key title
        /// </summary>
        [JsonPropertyName("title")]
        public string Title
        {
            
            get { return (string) attributes["title"]; }
            set { attributes["title"] = value; }
        }

        /// <summary>
        /// Public key created at date/time
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            
            get;
            set;
        }

        /// <summary>
        /// Public key fingerprint
        /// </summary>
        [JsonPropertyName("fingerprint")]
        public string Fingerprint
        {
            
            get { return (string) attributes["fingerprint"]; }
            set { attributes["fingerprint"] = value; }
        }

        /// <summary>
        /// User ID.  Provide a value of `0` to operate the current session's user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            
            get { return (Nullable<Int64>) attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Actual contents of SSH key.
        /// </summary>
        [JsonPropertyName("public_key")]
        public string PublicKeyType
        {
            
            get { return (string) attributes["public_key"]; }
            set { attributes["public_key"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   title (required) - string - Internal reference for key.
        /// </summary>
        public async Task<PublicKey> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("title") && !(parameters["title"] is string ))
            {
                throw new ArgumentException("Bad parameter: title must be of type string", "parameters[\"title\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("title") || parameters["title"] == null)
            {
                throw new ArgumentNullException("Parameter missing: title", "parameters[\"title\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/public_keys/{attributes["id"]}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<PublicKey>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<PublicKey> Delete(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendRequest($"/public_keys/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<PublicKey>(responseJson);
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
                var newObj = await PublicKey.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<PublicKey[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/public_keys", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<PublicKey[]>(responseJson);
        }

        public static async Task<PublicKey[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Public Key ID.
        /// </summary>
        public static async Task<PublicKey> Find(
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

            string responseJson = await FilesClient.SendRequest($"/public_keys/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<PublicKey>(responseJson);
        }

        public static async Task<PublicKey> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   title (required) - string - Internal reference for key.
        ///   public_key (required) - string - Actual contents of SSH key.
        /// </summary>
        public static async Task<PublicKey> Create(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("title") && !(parameters["title"] is string ))
            {
                throw new ArgumentException("Bad parameter: title must be of type string", "parameters[\"title\"]");
            }
            if (parameters.ContainsKey("public_key") && !(parameters["public_key"] is string ))
            {
                throw new ArgumentException("Bad parameter: public_key must be of type string", "parameters[\"public_key\"]");
            }
            if (!parameters.ContainsKey("title") || parameters["title"] == null)
            {
                throw new ArgumentNullException("Parameter missing: title", "parameters[\"title\"]");
            }
            if (!parameters.ContainsKey("public_key") || parameters["public_key"] == null)
            {
                throw new ArgumentNullException("Parameter missing: public_key", "parameters[\"public_key\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/public_keys", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<PublicKey>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   title (required) - string - Internal reference for key.
        /// </summary>
        public static async Task<PublicKey> Update(
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
            if (parameters.ContainsKey("title") && !(parameters["title"] is string ))
            {
                throw new ArgumentException("Bad parameter: title must be of type string", "parameters[\"title\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("title") || parameters["title"] == null)
            {
                throw new ArgumentNullException("Parameter missing: title", "parameters[\"title\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/public_keys/{parameters["id"]}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<PublicKey>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<PublicKey> Delete(
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

            string responseJson = await FilesClient.SendRequest($"/public_keys/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<PublicKey>(responseJson);
        }

        public static async Task<PublicKey> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

