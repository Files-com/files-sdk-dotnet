using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Group
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Group()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("name", null);
            this.attributes.Add("admin_ids", new object[0]);
            this.attributes.Add("notes", null);
            this.attributes.Add("user_ids", new object[0]);
            this.attributes.Add("usernames", new object[0]);
        }

        public Group(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Group ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Group name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string) attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// List of user IDs who are group administrators (separated by commas)
        /// </summary>
        [JsonPropertyName("admin_ids")]
        public object[] AdminIds
        {
            get { return (object[]) attributes["admin_ids"]; }
            set { attributes["admin_ids"] = value; }
        }

        /// <summary>
        /// Notes about this group
        /// </summary>
        [JsonPropertyName("notes")]
        public string Notes
        {
            get { return (string) attributes["notes"]; }
            set { attributes["notes"] = value; }
        }

        /// <summary>
        /// List of user IDs who belong to this group (separated by commas)
        /// </summary>
        [JsonPropertyName("user_ids")]
        public object[] UserIds
        {
            get { return (object[]) attributes["user_ids"]; }
            set { attributes["user_ids"] = value; }
        }

        /// <summary>
        /// List of usernames who belong to this group (separated by commas)
        /// </summary>
        [JsonPropertyName("usernames")]
        public object[] Usernames
        {
            get { return (object[]) attributes["usernames"]; }
            set { attributes["usernames"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Group name.
        ///   notes - string - Group notes.
        ///   user_ids - string - A list of user ids. If sent as a string, should be comma-delimited.
        ///   admin_ids - string - A list of group admin user ids. If sent as a string, should be comma-delimited.
        /// </summary>
        public async Task<Group> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string ))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/groups/{attributes["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Group>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Group> Delete(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendRequest($"/groups/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Group>(responseJson);
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
                var newObj = (Group) await Group.Create(this.attributes, this.options);
                this.attributes = newObj.attributes;
            }
        }

        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        ///   ids - string - Comma-separated list of group ids to include in results.
        /// </summary>
        public static async Task<Group[]> List(
            
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
            if (parameters.ContainsKey("ids") && !(parameters["ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: ids must be of type string", "parameters[\"ids\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/groups", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Group[]>(responseJson);
        }

        public static async Task<Group[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Group ID.
        /// </summary>
        public static async Task<Group> Find(
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

            string responseJson = await FilesClient.SendRequest($"/groups/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Group>(responseJson);
        }

        public static async Task<Group> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Group name.
        ///   notes - string - Group notes.
        ///   user_ids - string - A list of user ids. If sent as a string, should be comma-delimited.
        ///   admin_ids - string - A list of group admin user ids. If sent as a string, should be comma-delimited.
        /// </summary>
        public static async Task<Group> Create(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string ))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/groups", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Group>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Group name.
        ///   notes - string - Group notes.
        ///   user_ids - string - A list of user ids. If sent as a string, should be comma-delimited.
        ///   admin_ids - string - A list of group admin user ids. If sent as a string, should be comma-delimited.
        /// </summary>
        public static async Task<Group> Update(
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string ))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/groups/{parameters["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Group>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Group> Delete(
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

            string responseJson = await FilesClient.SendRequest($"/groups/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Group>(responseJson);
        }

        public static async Task<Group> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

