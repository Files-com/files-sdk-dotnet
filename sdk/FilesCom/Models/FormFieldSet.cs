using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class FormFieldSet
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FormFieldSet() : this(null, null) { }

        public FormFieldSet(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("title"))
            {
                this.attributes.Add("title", null);
            }
            if (!this.attributes.ContainsKey("form_layout"))
            {
                this.attributes.Add("form_layout", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("form_fields"))
            {
                this.attributes.Add("form_fields", new string[0]);
            }
            if (!this.attributes.ContainsKey("skip_name"))
            {
                this.attributes.Add("skip_name", false);
            }
            if (!this.attributes.ContainsKey("skip_email"))
            {
                this.attributes.Add("skip_email", false);
            }
            if (!this.attributes.ContainsKey("skip_company"))
            {
                this.attributes.Add("skip_company", false);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
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
        /// Form field set id
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Title to be displayed
        /// </summary>
        [JsonPropertyName("title")]
        public string Title
        {
            get { return (string)attributes["title"]; }
            set { attributes["title"] = value; }
        }

        /// <summary>
        /// Layout of the form
        /// </summary>
        [JsonPropertyName("form_layout")]
        public Nullable<Int64>[] FormLayout
        {
            get { return (Nullable<Int64>[])attributes["form_layout"]; }
            set { attributes["form_layout"] = value; }
        }

        /// <summary>
        /// Associated form fields
        /// </summary>
        [JsonPropertyName("form_fields")]
        public string[] FormFields
        {
            get { return (string[])attributes["form_fields"]; }
            set { attributes["form_fields"] = value; }
        }

        /// <summary>
        /// Any associated InboxRegistrations or BundleRegistrations can be saved without providing name
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_name")]
        public bool SkipName
        {
            get { return attributes["skip_name"] == null ? false : (bool)attributes["skip_name"]; }
            set { attributes["skip_name"] = value; }
        }

        /// <summary>
        /// Any associated InboxRegistrations or BundleRegistrations can be saved without providing email
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_email")]
        public bool SkipEmail
        {
            get { return attributes["skip_email"] == null ? false : (bool)attributes["skip_email"]; }
            set { attributes["skip_email"] = value; }
        }

        /// <summary>
        /// Any associated InboxRegistrations or BundleRegistrations can be saved without providing company
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("skip_company")]
        public bool SkipCompany
        {
            get { return attributes["skip_company"] == null ? false : (bool)attributes["skip_company"]; }
            set { attributes["skip_company"] = value; }
        }

        /// <summary>
        /// User ID.  Provide a value of `0` to operate the current session's user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   title - string - Title to be displayed
        ///   skip_email - boolean - Skip validating form email
        ///   skip_name - boolean - Skip validating form name
        ///   skip_company - boolean - Skip validating company
        ///   form_fields - array(object)
        /// </summary>
        public async Task<FormFieldSet> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("title") && !(parameters["title"] is string))
            {
                throw new ArgumentException("Bad parameter: title must be of type string", "parameters[\"title\"]");
            }
            if (parameters.ContainsKey("skip_email") && !(parameters["skip_email"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_email must be of type bool", "parameters[\"skip_email\"]");
            }
            if (parameters.ContainsKey("skip_name") && !(parameters["skip_name"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_name must be of type bool", "parameters[\"skip_name\"]");
            }
            if (parameters.ContainsKey("skip_company") && !(parameters["skip_company"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_company must be of type bool", "parameters[\"skip_company\"]");
            }
            if (parameters.ContainsKey("form_fields") && !(parameters["form_fields"] is object[]))
            {
                throw new ArgumentException("Bad parameter: form_fields must be of type object[]", "parameters[\"form_fields\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/form_field_sets/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FormFieldSet>(responseJson);
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

            await FilesClient.SendRequest($"/form_field_sets/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await FormFieldSet.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<FormFieldSet> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }

            return new FilesList<FormFieldSet>($"/form_field_sets", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<FormFieldSet> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Form Field Set ID.
        /// </summary>
        public static async Task<FormFieldSet> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/form_field_sets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FormFieldSet>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<FormFieldSet> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   title - string - Title to be displayed
        ///   skip_email - boolean - Skip validating form email
        ///   skip_name - boolean - Skip validating form name
        ///   skip_company - boolean - Skip validating company
        ///   form_fields - array(object)
        /// </summary>
        public static async Task<FormFieldSet> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("title") && !(parameters["title"] is string))
            {
                throw new ArgumentException("Bad parameter: title must be of type string", "parameters[\"title\"]");
            }
            if (parameters.ContainsKey("skip_email") && !(parameters["skip_email"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_email must be of type bool", "parameters[\"skip_email\"]");
            }
            if (parameters.ContainsKey("skip_name") && !(parameters["skip_name"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_name must be of type bool", "parameters[\"skip_name\"]");
            }
            if (parameters.ContainsKey("skip_company") && !(parameters["skip_company"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_company must be of type bool", "parameters[\"skip_company\"]");
            }
            if (parameters.ContainsKey("form_fields") && !(parameters["form_fields"] is object[]))
            {
                throw new ArgumentException("Bad parameter: form_fields must be of type object[]", "parameters[\"form_fields\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/form_field_sets", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FormFieldSet>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   title - string - Title to be displayed
        ///   skip_email - boolean - Skip validating form email
        ///   skip_name - boolean - Skip validating form name
        ///   skip_company - boolean - Skip validating company
        ///   form_fields - array(object)
        /// </summary>
        public static async Task<FormFieldSet> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("title") && !(parameters["title"] is string))
            {
                throw new ArgumentException("Bad parameter: title must be of type string", "parameters[\"title\"]");
            }
            if (parameters.ContainsKey("skip_email") && !(parameters["skip_email"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_email must be of type bool", "parameters[\"skip_email\"]");
            }
            if (parameters.ContainsKey("skip_name") && !(parameters["skip_name"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_name must be of type bool", "parameters[\"skip_name\"]");
            }
            if (parameters.ContainsKey("skip_company") && !(parameters["skip_company"] is bool))
            {
                throw new ArgumentException("Bad parameter: skip_company must be of type bool", "parameters[\"skip_company\"]");
            }
            if (parameters.ContainsKey("form_fields") && !(parameters["form_fields"] is object[]))
            {
                throw new ArgumentException("Bad parameter: form_fields must be of type object[]", "parameters[\"form_fields\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/form_field_sets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<FormFieldSet>(responseJson);
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

            parameters.Add("id", id);
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/form_field_sets/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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