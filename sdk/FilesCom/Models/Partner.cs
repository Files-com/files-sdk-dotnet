using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Partner
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Partner() : this(null, null) { }

        public Partner(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("allow_bypassing_2fa_policies"))
            {
                this.attributes.Add("allow_bypassing_2fa_policies", false);
            }
            if (!this.attributes.ContainsKey("allow_credential_changes"))
            {
                this.attributes.Add("allow_credential_changes", false);
            }
            if (!this.attributes.ContainsKey("allow_providing_gpg_keys"))
            {
                this.attributes.Add("allow_providing_gpg_keys", false);
            }
            if (!this.attributes.ContainsKey("allow_user_creation"))
            {
                this.attributes.Add("allow_user_creation", false);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("notes"))
            {
                this.attributes.Add("notes", null);
            }
            if (!this.attributes.ContainsKey("partner_admin_ids"))
            {
                this.attributes.Add("partner_admin_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("root_folder"))
            {
                this.attributes.Add("root_folder", null);
            }
            if (!this.attributes.ContainsKey("tags"))
            {
                this.attributes.Add("tags", null);
            }
            if (!this.attributes.ContainsKey("user_ids"))
            {
                this.attributes.Add("user_ids", new Nullable<Int64>[0]);
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
        /// Allow users created under this Partner to bypass Two-Factor Authentication policies.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_bypassing_2fa_policies")]
        public bool AllowBypassing2faPolicies
        {
            get { return attributes["allow_bypassing_2fa_policies"] == null ? false : (bool)attributes["allow_bypassing_2fa_policies"]; }
            set { attributes["allow_bypassing_2fa_policies"] = value; }
        }

        /// <summary>
        /// Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_credential_changes")]
        public bool AllowCredentialChanges
        {
            get { return attributes["allow_credential_changes"] == null ? false : (bool)attributes["allow_credential_changes"]; }
            set { attributes["allow_credential_changes"] = value; }
        }

        /// <summary>
        /// Allow Partner Admins to provide GPG keys.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_providing_gpg_keys")]
        public bool AllowProvidingGpgKeys
        {
            get { return attributes["allow_providing_gpg_keys"] == null ? false : (bool)attributes["allow_providing_gpg_keys"]; }
            set { attributes["allow_providing_gpg_keys"] = value; }
        }

        /// <summary>
        /// Allow Partner Admins to create users.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_user_creation")]
        public bool AllowUserCreation
        {
            get { return attributes["allow_user_creation"] == null ? false : (bool)attributes["allow_user_creation"]; }
            set { attributes["allow_user_creation"] = value; }
        }

        /// <summary>
        /// The unique ID of the Partner.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// The name of the Partner.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Notes about this Partner.
        /// </summary>
        [JsonPropertyName("notes")]
        public string Notes
        {
            get { return (string)attributes["notes"]; }
            set { attributes["notes"] = value; }
        }

        /// <summary>
        /// Array of User IDs that are Partner Admins for this Partner.
        /// </summary>
        [JsonPropertyName("partner_admin_ids")]
        public Nullable<Int64>[] PartnerAdminIds
        {
            get { return (Nullable<Int64>[])attributes["partner_admin_ids"]; }
            set { attributes["partner_admin_ids"] = value; }
        }

        /// <summary>
        /// The root folder path for this Partner.
        /// </summary>
        [JsonPropertyName("root_folder")]
        public string RootFolder
        {
            get { return (string)attributes["root_folder"]; }
            set { attributes["root_folder"] = value; }
        }

        /// <summary>
        /// Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        /// </summary>
        [JsonPropertyName("tags")]
        public string Tags
        {
            get { return (string)attributes["tags"]; }
            set { attributes["tags"] = value; }
        }

        /// <summary>
        /// Array of User IDs that belong to this Partner.
        /// </summary>
        [JsonPropertyName("user_ids")]
        public Nullable<Int64>[] UserIds
        {
            get { return (Nullable<Int64>[])attributes["user_ids"]; }
            set { attributes["user_ids"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - The name of the Partner.
        ///   allow_bypassing_2fa_policies - boolean - Allow users created under this Partner to bypass Two-Factor Authentication policies.
        ///   allow_credential_changes - boolean - Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        ///   allow_providing_gpg_keys - boolean - Allow Partner Admins to provide GPG keys.
        ///   allow_user_creation - boolean - Allow Partner Admins to create users.
        ///   notes - string - Notes about this Partner.
        ///   root_folder - string - The root folder path for this Partner.
        ///   tags - string - Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        /// </summary>
        public async Task<Partner> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("allow_bypassing_2fa_policies") && !(parameters["allow_bypassing_2fa_policies"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_bypassing_2fa_policies must be of type bool", "parameters[\"allow_bypassing_2fa_policies\"]");
            }
            if (parameters.ContainsKey("allow_credential_changes") && !(parameters["allow_credential_changes"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_credential_changes must be of type bool", "parameters[\"allow_credential_changes\"]");
            }
            if (parameters.ContainsKey("allow_providing_gpg_keys") && !(parameters["allow_providing_gpg_keys"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_providing_gpg_keys must be of type bool", "parameters[\"allow_providing_gpg_keys\"]");
            }
            if (parameters.ContainsKey("allow_user_creation") && !(parameters["allow_user_creation"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_user_creation must be of type bool", "parameters[\"allow_user_creation\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("root_folder") && !(parameters["root_folder"] is string))
            {
                throw new ArgumentException("Bad parameter: root_folder must be of type string", "parameters[\"root_folder\"]");
            }
            if (parameters.ContainsKey("tags") && !(parameters["tags"] is string))
            {
                throw new ArgumentException("Bad parameter: tags must be of type string", "parameters[\"tags\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partners/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Partner>(responseJson);
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

            await FilesClient.SendRequest($"/partners/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await Partner.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`.
        /// </summary>
        public static FilesList<Partner> List(

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

            return new FilesList<Partner>($"/partners", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Partner> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner ID.
        /// </summary>
        public static async Task<Partner> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Partner>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<Partner> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - The name of the Partner.
        ///   allow_bypassing_2fa_policies - boolean - Allow users created under this Partner to bypass Two-Factor Authentication policies.
        ///   allow_credential_changes - boolean - Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        ///   allow_providing_gpg_keys - boolean - Allow Partner Admins to provide GPG keys.
        ///   allow_user_creation - boolean - Allow Partner Admins to create users.
        ///   notes - string - Notes about this Partner.
        ///   root_folder - string - The root folder path for this Partner.
        ///   tags - string - Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        /// </summary>
        public static async Task<Partner> Create(

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
            if (parameters.ContainsKey("allow_bypassing_2fa_policies") && !(parameters["allow_bypassing_2fa_policies"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_bypassing_2fa_policies must be of type bool", "parameters[\"allow_bypassing_2fa_policies\"]");
            }
            if (parameters.ContainsKey("allow_credential_changes") && !(parameters["allow_credential_changes"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_credential_changes must be of type bool", "parameters[\"allow_credential_changes\"]");
            }
            if (parameters.ContainsKey("allow_providing_gpg_keys") && !(parameters["allow_providing_gpg_keys"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_providing_gpg_keys must be of type bool", "parameters[\"allow_providing_gpg_keys\"]");
            }
            if (parameters.ContainsKey("allow_user_creation") && !(parameters["allow_user_creation"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_user_creation must be of type bool", "parameters[\"allow_user_creation\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("root_folder") && !(parameters["root_folder"] is string))
            {
                throw new ArgumentException("Bad parameter: root_folder must be of type string", "parameters[\"root_folder\"]");
            }
            if (parameters.ContainsKey("tags") && !(parameters["tags"] is string))
            {
                throw new ArgumentException("Bad parameter: tags must be of type string", "parameters[\"tags\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partners", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Partner>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   name - string - The name of the Partner.
        ///   allow_bypassing_2fa_policies - boolean - Allow users created under this Partner to bypass Two-Factor Authentication policies.
        ///   allow_credential_changes - boolean - Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        ///   allow_providing_gpg_keys - boolean - Allow Partner Admins to provide GPG keys.
        ///   allow_user_creation - boolean - Allow Partner Admins to create users.
        ///   notes - string - Notes about this Partner.
        ///   root_folder - string - The root folder path for this Partner.
        ///   tags - string - Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        /// </summary>
        public static async Task<Partner> Update(
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("allow_bypassing_2fa_policies") && !(parameters["allow_bypassing_2fa_policies"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_bypassing_2fa_policies must be of type bool", "parameters[\"allow_bypassing_2fa_policies\"]");
            }
            if (parameters.ContainsKey("allow_credential_changes") && !(parameters["allow_credential_changes"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_credential_changes must be of type bool", "parameters[\"allow_credential_changes\"]");
            }
            if (parameters.ContainsKey("allow_providing_gpg_keys") && !(parameters["allow_providing_gpg_keys"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_providing_gpg_keys must be of type bool", "parameters[\"allow_providing_gpg_keys\"]");
            }
            if (parameters.ContainsKey("allow_user_creation") && !(parameters["allow_user_creation"] is bool))
            {
                throw new ArgumentException("Bad parameter: allow_user_creation must be of type bool", "parameters[\"allow_user_creation\"]");
            }
            if (parameters.ContainsKey("notes") && !(parameters["notes"] is string))
            {
                throw new ArgumentException("Bad parameter: notes must be of type string", "parameters[\"notes\"]");
            }
            if (parameters.ContainsKey("root_folder") && !(parameters["root_folder"] is string))
            {
                throw new ArgumentException("Bad parameter: root_folder must be of type string", "parameters[\"root_folder\"]");
            }
            if (parameters.ContainsKey("tags") && !(parameters["tags"] is string))
            {
                throw new ArgumentException("Bad parameter: tags must be of type string", "parameters[\"tags\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Partner>(responseJson);
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

            await FilesClient.SendRequest($"/partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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