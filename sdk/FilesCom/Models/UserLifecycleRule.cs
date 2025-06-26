using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class UserLifecycleRule
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public UserLifecycleRule() : this(null, null) { }

        public UserLifecycleRule(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("authentication_method"))
            {
                this.attributes.Add("authentication_method", null);
            }
            if (!this.attributes.ContainsKey("inactivity_days"))
            {
                this.attributes.Add("inactivity_days", null);
            }
            if (!this.attributes.ContainsKey("include_folder_admins"))
            {
                this.attributes.Add("include_folder_admins", false);
            }
            if (!this.attributes.ContainsKey("include_site_admins"))
            {
                this.attributes.Add("include_site_admins", false);
            }
            if (!this.attributes.ContainsKey("action"))
            {
                this.attributes.Add("action", null);
            }
            if (!this.attributes.ContainsKey("user_state"))
            {
                this.attributes.Add("user_state", null);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
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
        /// User Lifecycle Rule ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// User authentication method for the rule
        /// </summary>
        [JsonPropertyName("authentication_method")]
        public string AuthenticationMethod
        {
            get { return (string)attributes["authentication_method"]; }
            set { attributes["authentication_method"] = value; }
        }

        /// <summary>
        /// Number of days of inactivity before the rule applies
        /// </summary>
        [JsonPropertyName("inactivity_days")]
        public Nullable<Int64> InactivityDays
        {
            get { return (Nullable<Int64>)attributes["inactivity_days"]; }
            set { attributes["inactivity_days"] = value; }
        }

        /// <summary>
        /// Include folder admins in the rule
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("include_folder_admins")]
        public bool IncludeFolderAdmins
        {
            get { return attributes["include_folder_admins"] == null ? false : (bool)attributes["include_folder_admins"]; }
            set { attributes["include_folder_admins"] = value; }
        }

        /// <summary>
        /// Include site admins in the rule
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("include_site_admins")]
        public bool IncludeSiteAdmins
        {
            get { return attributes["include_site_admins"] == null ? false : (bool)attributes["include_site_admins"]; }
            set { attributes["include_site_admins"] = value; }
        }

        /// <summary>
        /// Action to take on inactive users (disable or delete)
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string)attributes["action"]; }
            set { attributes["action"] = value; }
        }

        /// <summary>
        /// State of the users to apply the rule to (inactive or disabled)
        /// </summary>
        [JsonPropertyName("user_state")]
        public string UserState
        {
            get { return (string)attributes["user_state"]; }
            set { attributes["user_state"] = value; }
        }

        /// <summary>
        /// Site ID
        /// </summary>
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            set { attributes["site_id"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   action (required) - string - Action to take on inactive users (disable or delete)
        ///   authentication_method (required) - string - User authentication method for the rule
        ///   inactivity_days (required) - int64 - Number of days of inactivity before the rule applies
        ///   include_site_admins - boolean - Include site admins in the rule
        ///   include_folder_admins - boolean - Include folder admins in the rule
        ///   user_state - string - State of the users to apply the rule to (inactive or disabled)
        /// </summary>
        public async Task<UserLifecycleRule> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("action") || parameters["action"] == null)
            {
                throw new ArgumentNullException("Parameter missing: action", "parameters[\"action\"]");
            }
            if (!parameters.ContainsKey("authentication_method") || parameters["authentication_method"] == null)
            {
                throw new ArgumentNullException("Parameter missing: authentication_method", "parameters[\"authentication_method\"]");
            }
            if (!parameters.ContainsKey("inactivity_days") || parameters["inactivity_days"] == null)
            {
                throw new ArgumentNullException("Parameter missing: inactivity_days", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("authentication_method") && !(parameters["authentication_method"] is string))
            {
                throw new ArgumentException("Bad parameter: authentication_method must be of type string", "parameters[\"authentication_method\"]");
            }
            if (parameters.ContainsKey("inactivity_days") && !(parameters["inactivity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_days must be of type Nullable<Int64>", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("include_site_admins") && !(parameters["include_site_admins"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_site_admins must be of type bool", "parameters[\"include_site_admins\"]");
            }
            if (parameters.ContainsKey("include_folder_admins") && !(parameters["include_folder_admins"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_folder_admins must be of type bool", "parameters[\"include_folder_admins\"]");
            }
            if (parameters.ContainsKey("user_state") && !(parameters["user_state"] is string))
            {
                throw new ArgumentException("Bad parameter: user_state must be of type string", "parameters[\"user_state\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/user_lifecycle_rules/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<UserLifecycleRule>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/user_lifecycle_rules/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await UserLifecycleRule.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<UserLifecycleRule> List(

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

            return new FilesList<UserLifecycleRule>($"/user_lifecycle_rules", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<UserLifecycleRule> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - User Lifecycle Rule ID.
        /// </summary>
        public static async Task<UserLifecycleRule> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/user_lifecycle_rules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<UserLifecycleRule>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<UserLifecycleRule> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   action (required) - string - Action to take on inactive users (disable or delete)
        ///   authentication_method (required) - string - User authentication method for the rule
        ///   inactivity_days (required) - int64 - Number of days of inactivity before the rule applies
        ///   include_site_admins - boolean - Include site admins in the rule
        ///   include_folder_admins - boolean - Include folder admins in the rule
        ///   user_state - string - State of the users to apply the rule to (inactive or disabled)
        /// </summary>
        public static async Task<UserLifecycleRule> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("action") || parameters["action"] == null)
            {
                throw new ArgumentNullException("Parameter missing: action", "parameters[\"action\"]");
            }
            if (!parameters.ContainsKey("authentication_method") || parameters["authentication_method"] == null)
            {
                throw new ArgumentNullException("Parameter missing: authentication_method", "parameters[\"authentication_method\"]");
            }
            if (!parameters.ContainsKey("inactivity_days") || parameters["inactivity_days"] == null)
            {
                throw new ArgumentNullException("Parameter missing: inactivity_days", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("authentication_method") && !(parameters["authentication_method"] is string))
            {
                throw new ArgumentException("Bad parameter: authentication_method must be of type string", "parameters[\"authentication_method\"]");
            }
            if (parameters.ContainsKey("inactivity_days") && !(parameters["inactivity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_days must be of type Nullable<Int64>", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("include_site_admins") && !(parameters["include_site_admins"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_site_admins must be of type bool", "parameters[\"include_site_admins\"]");
            }
            if (parameters.ContainsKey("include_folder_admins") && !(parameters["include_folder_admins"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_folder_admins must be of type bool", "parameters[\"include_folder_admins\"]");
            }
            if (parameters.ContainsKey("user_state") && !(parameters["user_state"] is string))
            {
                throw new ArgumentException("Bad parameter: user_state must be of type string", "parameters[\"user_state\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/user_lifecycle_rules", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<UserLifecycleRule>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   action (required) - string - Action to take on inactive users (disable or delete)
        ///   authentication_method (required) - string - User authentication method for the rule
        ///   inactivity_days (required) - int64 - Number of days of inactivity before the rule applies
        ///   include_site_admins - boolean - Include site admins in the rule
        ///   include_folder_admins - boolean - Include folder admins in the rule
        ///   user_state - string - State of the users to apply the rule to (inactive or disabled)
        /// </summary>
        public static async Task<UserLifecycleRule> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("action") || parameters["action"] == null)
            {
                throw new ArgumentNullException("Parameter missing: action", "parameters[\"action\"]");
            }
            if (!parameters.ContainsKey("authentication_method") || parameters["authentication_method"] == null)
            {
                throw new ArgumentNullException("Parameter missing: authentication_method", "parameters[\"authentication_method\"]");
            }
            if (!parameters.ContainsKey("inactivity_days") || parameters["inactivity_days"] == null)
            {
                throw new ArgumentNullException("Parameter missing: inactivity_days", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("authentication_method") && !(parameters["authentication_method"] is string))
            {
                throw new ArgumentException("Bad parameter: authentication_method must be of type string", "parameters[\"authentication_method\"]");
            }
            if (parameters.ContainsKey("inactivity_days") && !(parameters["inactivity_days"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_days must be of type Nullable<Int64>", "parameters[\"inactivity_days\"]");
            }
            if (parameters.ContainsKey("include_site_admins") && !(parameters["include_site_admins"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_site_admins must be of type bool", "parameters[\"include_site_admins\"]");
            }
            if (parameters.ContainsKey("include_folder_admins") && !(parameters["include_folder_admins"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_folder_admins must be of type bool", "parameters[\"include_folder_admins\"]");
            }
            if (parameters.ContainsKey("user_state") && !(parameters["user_state"] is string))
            {
                throw new ArgumentException("Bad parameter: user_state must be of type string", "parameters[\"user_state\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/user_lifecycle_rules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<UserLifecycleRule>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/user_lifecycle_rules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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