using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
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
                this.attributes.Add("targets", new object[0]);
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
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            private set { attributes["path"] = value; }
        }

        /// <summary>
        /// Action occurrence date/time
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("when")]
        public Nullable<DateTime> When
        {
            get { return (Nullable<DateTime>)attributes["when"]; }
            private set { attributes["when"] = value; }
        }

        /// <summary>
        /// The destination path for this action, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string)attributes["destination"]; }
            private set { attributes["destination"] = value; }
        }

        /// <summary>
        /// Friendly displayed output
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("display")]
        public string Display
        {
            get { return (string)attributes["display"]; }
            private set { attributes["display"] = value; }
        }

        /// <summary>
        /// IP Address that performed this action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("ip")]
        public string Ip
        {
            get { return (string)attributes["ip"]; }
            private set { attributes["ip"] = value; }
        }

        /// <summary>
        /// The source path for this action, if applicable
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string)attributes["source"]; }
            private set { attributes["source"] = value; }
        }

        /// <summary>
        /// Targets
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("targets")]
        public object[] Targets
        {
            get { return (object[])attributes["targets"]; }
            private set { attributes["targets"] = value; }
        }

        /// <summary>
        /// User ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            private set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Username
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            private set { attributes["username"] = value; }
        }

        /// <summary>
        /// Type of action
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            private set { attributes["action"] = value; }
        }

        /// <summary>
        /// Failure type.  If action was a user login or session failure, why did it fail?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failure_type")]
        public string FailureType
        {
            get { return (string)attributes["failure_type"]; }
            private set { attributes["failure_type"] = value; }
        }

        /// <summary>
        /// Interface on which this action occurred.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("interface")]
        public string Interface
        {
            get { return (string)attributes["interface"]; }
            private set { attributes["interface"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[user_id]=desc`). Valid fields are `user_id` and `created_at`.
        ///   path (required) - string - Path to operate on.
        /// </summary>
        public static FilesList<Action> ListForFile(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            return new FilesList<Action>($"/history/files/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[user_id]=desc`). Valid fields are `user_id` and `created_at`.
        ///   path (required) - string - Path to operate on.
        /// </summary>
        public static FilesList<Action> ListForFolder(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("path"))
            {
                parameters["path"] = path;
            }
            else
            {
                parameters.Add("path", path);
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }

            return new FilesList<Action>($"/history/folders/{System.Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[user_id]=desc`). Valid fields are `user_id` and `created_at`.
        ///   user_id (required) - int64 - User ID.
        /// </summary>
        public static FilesList<Action> ListForUser(
            Nullable<Int64> user_id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id"))
            {
                parameters["user_id"] = user_id;
            }
            else
            {
                parameters.Add("user_id", user_id);
            }
            if (!parameters.ContainsKey("user_id") || parameters["user_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: user_id", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }

            return new FilesList<Action>($"/history/users/{System.Uri.EscapeDataString(parameters["user_id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[user_id]=desc`). Valid fields are `user_id` and `created_at`.
        /// </summary>
        public static FilesList<Action> ListLogins(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }

            return new FilesList<Action>($"/history/login", System.Net.Http.HttpMethod.Get, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[path]=desc`). Valid fields are `path`, `folder`, `user_id` or `created_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `user_id`, `folder` or `path`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        /// </summary>
        public static FilesList<Action> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("start_at") && !(parameters["start_at"] is string))
            {
                throw new ArgumentException("Bad parameter: start_at must be of type string", "parameters[\"start_at\"]");
            }
            if (parameters.ContainsKey("end_at") && !(parameters["end_at"] is string))
            {
                throw new ArgumentException("Bad parameter: end_at must be of type string", "parameters[\"end_at\"]");
            }
            if (parameters.ContainsKey("display") && !(parameters["display"] is string))
            {
                throw new ArgumentException("Bad parameter: display must be of type string", "parameters[\"display\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
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

            return new FilesList<Action>($"/history", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Action> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}