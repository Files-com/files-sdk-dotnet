using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class As2Key
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public As2Key() : this(null, null) { }

        public As2Key(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("as2_partnership_name"))
            {
                this.attributes.Add("as2_partnership_name", null);
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
        /// AS2 Key ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// AS2 Partnership Name
        /// </summary>
        [JsonPropertyName("as2_partnership_name")]
        public string As2PartnershipName
        {
            get { return (string) attributes["as2_partnership_name"]; }
            set { attributes["as2_partnership_name"] = value; }
        }

        /// <summary>
        /// AS2 Key created at date/time
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
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
        /// Actual contents of Public key.
        /// </summary>
        [JsonPropertyName("public_key")]
        public string PublicKey
        {
            get { return (string) attributes["public_key"]; }
            set { attributes["public_key"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   as2_partnership_name (required) - string - AS2 Partnership Name
        /// </summary>
        public async Task<As2Key> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("as2_partnership_name") && !(parameters["as2_partnership_name"] is string ))
            {
                throw new ArgumentException("Bad parameter: as2_partnership_name must be of type string", "parameters[\"as2_partnership_name\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("as2_partnership_name") || parameters["as2_partnership_name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: as2_partnership_name", "parameters[\"as2_partnership_name\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/as2_keys/{attributes["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<As2Key>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<As2Key> Delete(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendRequest($"/as2_keys/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<As2Key>(responseJson);
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
                var newObj = await As2Key.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        /// </summary>
        public static async Task<As2Key[]> List(
            
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

            string responseJson = await FilesClient.SendRequest($"/as2_keys", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<As2Key[]>(responseJson);
        }

        public static async Task<As2Key[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - As2 Key ID.
        /// </summary>
        public static async Task<As2Key> Find(
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

            string responseJson = await FilesClient.SendRequest($"/as2_keys/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<As2Key>(responseJson);
        }

        public static async Task<As2Key> Get(
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
        ///   as2_partnership_name (required) - string - AS2 Partnership Name
        ///   public_key (required) - string - Actual contents of Public key.
        /// </summary>
        public static async Task<As2Key> Create(
            
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
            if (parameters.ContainsKey("as2_partnership_name") && !(parameters["as2_partnership_name"] is string ))
            {
                throw new ArgumentException("Bad parameter: as2_partnership_name must be of type string", "parameters[\"as2_partnership_name\"]");
            }
            if (parameters.ContainsKey("public_key") && !(parameters["public_key"] is string ))
            {
                throw new ArgumentException("Bad parameter: public_key must be of type string", "parameters[\"public_key\"]");
            }
            if (!parameters.ContainsKey("as2_partnership_name") || parameters["as2_partnership_name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: as2_partnership_name", "parameters[\"as2_partnership_name\"]");
            }
            if (!parameters.ContainsKey("public_key") || parameters["public_key"] == null)
            {
                throw new ArgumentNullException("Parameter missing: public_key", "parameters[\"public_key\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/as2_keys", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<As2Key>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   as2_partnership_name (required) - string - AS2 Partnership Name
        /// </summary>
        public static async Task<As2Key> Update(
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
            if (parameters.ContainsKey("as2_partnership_name") && !(parameters["as2_partnership_name"] is string ))
            {
                throw new ArgumentException("Bad parameter: as2_partnership_name must be of type string", "parameters[\"as2_partnership_name\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("as2_partnership_name") || parameters["as2_partnership_name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: as2_partnership_name", "parameters[\"as2_partnership_name\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/as2_keys/{parameters["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<As2Key>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<As2Key> Delete(
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

            string responseJson = await FilesClient.SendRequest($"/as2_keys/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<As2Key>(responseJson);
        }

        public static async Task<As2Key> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

