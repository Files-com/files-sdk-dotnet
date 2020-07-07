using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class GroupUser
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public GroupUser()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("group_name", null);
            this.attributes.Add("group_id", null);
            this.attributes.Add("user_id", null);
            this.attributes.Add("admin", null);
            this.attributes.Add("usernames", new string[0]);
            this.attributes.Add("id", null);
        }

        public GroupUser(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Group name
        /// </summary>
        [JsonPropertyName("group_name")]
        public string GroupName
        {
            get { return (string) attributes["group_name"]; }
            set { attributes["group_name"] = value; }
        }

        /// <summary>
        /// Group ID
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>) attributes["group_id"]; }
            set { attributes["group_id"] = value; }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Is this user an administrator of this group?
        /// </summary>
        [JsonPropertyName("admin")]
        public bool Admin
        {
            get { return (bool) attributes["admin"]; }
            set { attributes["admin"] = value; }
        }

        /// <summary>
        /// A list of usernames for users in this group
        /// </summary>
        [JsonPropertyName("usernames")]
        public string[] Usernames
        {
            get { return (string[]) attributes["usernames"]; }
            set { attributes["usernames"] = value; }
        }

        /// <summary>
        /// Group User ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   group_id (required) - int64 - Group ID to add user to.
        ///   user_id (required) - int64 - User ID to add to group.
        ///   admin - boolean - Is the user a group administrator?
        /// </summary>
        public async Task<GroupUser> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("admin") && !(parameters["admin"] is bool ))
            {
                throw new ArgumentException("Bad parameter: admin must be of type bool", "parameters[\"admin\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("group_id") || parameters["group_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: group_id", "parameters[\"group_id\"]");
            }
            if (!parameters.ContainsKey("user_id") || parameters["user_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: user_id", "parameters[\"user_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/group_users/{attributes["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<GroupUser>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<GroupUser> Delete(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendRequest($"/group_users/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<GroupUser>(responseJson);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            await Update(this.attributes);
        }


        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  If provided, will return group_users of this user.
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        ///   group_id - int64 - Group ID.  If provided, will return group_users of this group.
        /// </summary>
        public static async Task<GroupUser[]> List(
            
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
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/group_users", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<GroupUser[]>(responseJson);
        }

        public static async Task<GroupUser[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   group_id (required) - int64 - Group ID to add user to.
        ///   user_id (required) - int64 - User ID to add to group.
        ///   admin - boolean - Is the user a group administrator?
        /// </summary>
        public static async Task<GroupUser> Update(
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
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("admin") && !(parameters["admin"] is bool ))
            {
                throw new ArgumentException("Bad parameter: admin must be of type bool", "parameters[\"admin\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("group_id") || parameters["group_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: group_id", "parameters[\"group_id\"]");
            }
            if (!parameters.ContainsKey("user_id") || parameters["user_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: user_id", "parameters[\"user_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/group_users/{parameters["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<GroupUser>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<GroupUser> Delete(
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

            string responseJson = await FilesClient.SendRequest($"/group_users/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<GroupUser>(responseJson);
        }

        public static async Task<GroupUser> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

