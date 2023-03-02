using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class SettingsChange
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public SettingsChange() : this(null, null) { }

        public SettingsChange(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("changes"))
            {
                this.attributes.Add("changes", new string[0]);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("user_is_files_support"))
            {
                this.attributes.Add("user_is_files_support", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
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
        /// Markdown-formatted change messages.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("changes")]
        public string[] Changes
        {
            get { return (string[])attributes["changes"]; }
            private set { attributes["changes"] = value; }
        }

        /// <summary>
        /// The time this change was made
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// The user id responsible for this change
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            private set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// true if this change was performed by Files.com support.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_is_files_support")]
        public bool UserIsFilesSupport
        {
            get { return (bool)attributes["user_is_files_support"]; }
            private set { attributes["user_is_files_support"] = value; }
        }

        /// <summary>
        /// The username of the user responsible for this change
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            private set { attributes["username"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `api_key_id`, `created_at` or `user_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `api_key_id` and `user_id`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `api_key_id` and `user_id`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal to the supplied value. Valid fields are `api_key_id` and `user_id`.
        ///   filter_like - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `api_key_id` and `user_id`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `api_key_id` and `user_id`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal to the supplied value. Valid fields are `api_key_id` and `user_id`.
        /// </summary>
        public static async Task<SettingsChange[]> List(

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
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_like") && !(parameters["filter_like"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_like must be of type object", "parameters[\"filter_like\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/settings_changes", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<SettingsChange[]>(responseJson);
        }

        public static async Task<SettingsChange[]> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}