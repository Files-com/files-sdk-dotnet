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
        public History()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("path", null);
            this.attributes.Add("when", null);
            this.attributes.Add("destination", null);
            this.attributes.Add("display", null);
            this.attributes.Add("ip", null);
            this.attributes.Add("source", null);
            this.attributes.Add("targets", new string[0]);
            this.attributes.Add("user_id", null);
            this.attributes.Add("username", null);
            this.attributes.Add("action", null);
            this.attributes.Add("failure_type", null);
            this.attributes.Add("interface", null);
        }

        public History(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
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
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
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
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/history/files(/*path)", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<History[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
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
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/history/folders(/*path)", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<History[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
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
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
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

            string responseJson = await FilesClient.SendRequest($"/history/login", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<History[]>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   start_at - string - Leave blank or set to a date/time to filter earlier entries.
        ///   end_at - string - Leave blank or set to a date/time to filter later entries.
        ///   display - string - Display format. Leave blank or set to `full` or `parent`.
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
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

