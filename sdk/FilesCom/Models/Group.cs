using FilesCom.Util;
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
                this.attributes.Add("user_ids", null);
            }
            if (!this.attributes.ContainsKey("usernames"))
            {
                this.attributes.Add("usernames", null);
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
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Group name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Comma-delimited list of user IDs who are group administrators (separated by commas)
        /// </summary>
        [JsonPropertyName("admin_ids")]
        public string AdminIds
        {
            get { return (string)attributes["admin_ids"]; }
            set { attributes["admin_ids"] = value; }
        }

        /// <summary>
        /// Notes about this group
        /// </summary>
        [JsonPropertyName("notes")]
        public string Notes
        {
            get { return (string)attributes["notes"]; }
            set { attributes["notes"] = value; }
        }

        /// <summary>
        /// Comma-delimited list of user IDs who belong to this group (separated by commas)
        /// </summary>
        [JsonPropertyName("user_ids")]
        public string UserIds
        {
            get { return (string)attributes["user_ids"]; }
            set { attributes["user_ids"] = value; }
        }

        /// <summary>
        /// Comma-delimited list of usernames who belong to this group (separated by commas)
        /// </summary>
        [JsonPropertyName("usernames")]
        public string Usernames
        {
            get { return (string)attributes["usernames"]; }
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

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/groups/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Group>(responseJson);
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

            await FilesClient.SendRequest($"/groups/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[name]=desc`). Valid fields are `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `name`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
        ///   ids - string - Comma-separated list of group ids to include in results.
        /// </summary>
        public static FilesList<Group> List(

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
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }
            if (parameters.ContainsKey("ids") && !(parameters["ids"] is string))
            {
                throw new ArgumentException("Bad parameter: ids must be of type string", "parameters[\"ids\"]");
            }

            return new FilesList<Group>($"/groups", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Group> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
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
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/groups/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

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

            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/groups", System.Net.Http.HttpMethod.Post, parameters, options);

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
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("admin_ids") && !(parameters["admin_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: admin_ids must be of type string", "parameters[\"admin_ids\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/groups/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Group>(responseJson);
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

            await FilesClient.SendRequest($"/groups/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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