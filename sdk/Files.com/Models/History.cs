using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class History
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public History() : this(null, null) { }

        public History(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("when"))
            {
                this.attributes.Add("when", null);
            }
            if (!this.attributes.ContainsKey("destination"))
            {
                this.attributes.Add("destination", null);
            }
            if (!this.attributes.ContainsKey("display"))
            {
                this.attributes.Add("display", null);
            }
            if (!this.attributes.ContainsKey("ip"))
            {
                this.attributes.Add("ip", null);
            }
            if (!this.attributes.ContainsKey("source"))
            {
                this.attributes.Add("source", null);
            }
            if (!this.attributes.ContainsKey("targets"))
            {
                this.attributes.Add("targets", new string[0]);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("failure_type"))
            {
                this.attributes.Add("failure_type", null);
            }
            if (!this.attributes.ContainsKey("interface"))
            {
                this.attributes.Add("interface", null);
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
        /// Action ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
        }

        /// <summary>
        /// Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
        }

        /// <summary>
        /// Action occurrence date/time
        /// </summary>
        [JsonPropertyName("when")]
        public Nullable<DateTime> When
        {
            get { return (Nullable<DateTime>) attributes["when"]; }
        }

        /// <summary>
        /// The destination path for this action, if applicable
        /// </summary>
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string) attributes["destination"]; }
        }

        /// <summary>
        /// Friendly displayed output
        /// </summary>
        [JsonPropertyName("display")]
        public string Display
        {
            get { return (string) attributes["display"]; }
        }

        /// <summary>
        /// IP Address that performed this action
        /// </summary>
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string) attributes["ip"]; }
        }

        /// <summary>
        /// The source path for this action, if applicable
        /// </summary>
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string) attributes["source"]; }
        }

        /// <summary>
        /// Targets
        /// </summary>
        [JsonPropertyName("targets")]
        public string[] Targets
        {
            get { return (string[]) attributes["targets"]; }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
        }

        /// <summary>
        /// Username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string) attributes["username"]; }
        }

        /// <summary>
        /// Type of action
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string) attributes["action"]; }
        }

        /// <summary>
        /// Failure type.  If action was a user login or session failure, why did it fail?
        /// </summary>
        [JsonPropertyName("failure_type")]
        public string FailureType
        {
            get { return (string) attributes["failure_type"]; }
        }

        /// <summary>
        /// Interface on which this action occurred.
        /// </summary>
        [JsonPropertyName("interface")]
        public string Interface
        {
            get { return (string) attributes["interface"]; }
        }



        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `user_id` and `created_at`.
        ///   path (required) - string - Path to operate on.
        /// </summary>
        public static async Task<History[]> ListForFile(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string ))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
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
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/history/files/{parameters["path"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<History[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `user_id` and `created_at`.
        ///   path (required) - string - Path to operate on.
        /// </summary>
        public static async Task<History[]> ListForFolder(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string ))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
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
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/history/folders/{parameters["path"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<History[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `user_id` and `created_at`.
        ///   user_id (required) - int64 - User ID.
        /// </summary>
        public static async Task<History[]> ListForUser(
            Nullable<Int64> user_id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("user_id", user_id);
            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string ))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
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
            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (!parameters.ContainsKey("user_id") || parameters["user_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: user_id", "parameters[\"user_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/history/users/{parameters["user_id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<History[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `user_id` and `created_at`.
        /// </summary>
        public static async Task<History[]> ListLogins(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string ))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
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

            string responseJson = await FilesClient.SendRequest($"/history/login", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<History[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `path`, `folder`, `user_id` or `created_at`.
        ///   filter - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `user_id`, `folder` or `path`.
        ///   filter_gt - object - If set, return records where the specifiied field is greater than the supplied value. Valid fields are `user_id`, `folder` or `path`.
        ///   filter_gteq - object - If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `user_id`, `folder` or `path`.
        ///   filter_like - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `user_id`, `folder` or `path`.
        ///   filter_lt - object - If set, return records where the specifiied field is less than the supplied value. Valid fields are `user_id`, `folder` or `path`.
        ///   filter_lteq - object - If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `user_id`, `folder` or `path`.
        /// </summary>
        public static async Task<History[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string ))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string ))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
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

            string responseJson = await FilesClient.SendRequest($"/history", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<History[]>(responseJson);
        }

        public static async Task<History[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

