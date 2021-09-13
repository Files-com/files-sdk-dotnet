using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Group
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Group() : this(null, null) { }

        public Group(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("admin_ids"))
            {
                this.attributes.Add("admin_ids", null);
            }
            if (!this.attributes.ContainsKey("notes"))
            {
                this.attributes.Add("notes", null);
            }
            if (!this.attributes.ContainsKey("user_ids"))
            {
                this.attributes.Add("user_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("usernames"))
            {
                this.attributes.Add("usernames", new string[0]);
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
        public string AdminIds
        {
            
            get { return (string) attributes["admin_ids"]; }
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
        public Nullable<Int64>[] UserIds
        {
            
            get { return (Nullable<Int64>[]) attributes["user_ids"]; }
            set { attributes["user_ids"] = value; }
        }

        /// <summary>
        /// List of usernames who belong to this group (separated by commas)
        /// </summary>
        [JsonPropertyName("usernames")]
        public string[] Usernames
        {
            
            get { return (string[]) attributes["usernames"]; }
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

            string responseJson = await FilesClient.SendRequest($"/groups/{attributes["id"]}", new HttpMethod("PATCH"), parameters, options);

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
                var newObj = await Group.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `name`.
        ///   filter - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `name`.
        ///   filter_gt - object - If set, return records where the specifiied field is greater than the supplied value. Valid fields are `name`.
        ///   filter_gteq - object - If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `name`.
        ///   filter_like - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `name`.
        ///   filter_lt - object - If set, return records where the specifiied field is less than the supplied value. Valid fields are `name`.
        ///   filter_lteq - object - If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `name`.
        ///   ids - string - Comma-separated list of group ids to include in results.
        /// </summary>
        public static async Task<Group[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object ))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_like") && !(parameters["filter_like"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_like must be of type object", "parameters[\"filter_like\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
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

            string responseJson = await FilesClient.SendRequest($"/groups/{parameters["id"]}", new HttpMethod("PATCH"), parameters, options);

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

