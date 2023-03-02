using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Clickwrap
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Clickwrap() : this(null, null) { }

        public Clickwrap(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("body"))
            {
                this.attributes.Add("body", null);
            }
            if (!this.attributes.ContainsKey("use_with_users"))
            {
                this.attributes.Add("use_with_users", null);
            }
            if (!this.attributes.ContainsKey("use_with_bundles"))
            {
                this.attributes.Add("use_with_bundles", null);
            }
            if (!this.attributes.ContainsKey("use_with_inboxes"))
            {
                this.attributes.Add("use_with_inboxes", null);
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
        /// Clickwrap ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Body text of Clickwrap (supports Markdown formatting).
        /// </summary>
        [JsonPropertyName("body")]
        public string Body
        {
            get { return (string)attributes["body"]; }
            set { attributes["body"] = value; }
        }

        /// <summary>
        /// Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.
        /// </summary>
        [JsonPropertyName("use_with_users")]
        public string UseWithUsers
        {
            get { return (string)attributes["use_with_users"]; }
            set { attributes["use_with_users"] = value; }
        }

        /// <summary>
        /// Use this Clickwrap for Bundles?
        /// </summary>
        [JsonPropertyName("use_with_bundles")]
        public string UseWithBundles
        {
            get { return (string)attributes["use_with_bundles"]; }
            set { attributes["use_with_bundles"] = value; }
        }

        /// <summary>
        /// Use this Clickwrap for Inboxes?
        /// </summary>
        [JsonPropertyName("use_with_inboxes")]
        public string UseWithInboxes
        {
            get { return (string)attributes["use_with_inboxes"]; }
            set { attributes["use_with_inboxes"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
        ///   body - string - Body text of Clickwrap (supports Markdown formatting).
        ///   use_with_bundles - string - Use this Clickwrap for Bundles?
        ///   use_with_inboxes - string - Use this Clickwrap for Inboxes?
        ///   use_with_users - string - Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.
        /// </summary>
        public async Task<Clickwrap> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("body") && !(parameters["body"] is string))
            {
                throw new ArgumentException("Bad parameter: body must be of type string", "parameters[\"body\"]");
            }
            if (parameters.ContainsKey("use_with_bundles") && !(parameters["use_with_bundles"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_bundles must be of type string", "parameters[\"use_with_bundles\"]");
            }
            if (parameters.ContainsKey("use_with_inboxes") && !(parameters["use_with_inboxes"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_inboxes must be of type string", "parameters[\"use_with_inboxes\"]");
            }
            if (parameters.ContainsKey("use_with_users") && !(parameters["use_with_users"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_users must be of type string", "parameters[\"use_with_users\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/clickwraps/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Clickwrap>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Clickwrap> Delete(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendRequest($"/clickwraps/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Clickwrap>(responseJson);
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
                var newObj = await Clickwrap.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<Clickwrap[]> List(

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

            string responseJson = await FilesClient.SendRequest($"/clickwraps", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Clickwrap[]>(responseJson);
        }

        public static async Task<Clickwrap[]> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Clickwrap ID.
        /// </summary>
        public static async Task<Clickwrap> Find(
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

            string responseJson = await FilesClient.SendRequest($"/clickwraps/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Clickwrap>(responseJson);
        }

        public static async Task<Clickwrap> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
        ///   body - string - Body text of Clickwrap (supports Markdown formatting).
        ///   use_with_bundles - string - Use this Clickwrap for Bundles?
        ///   use_with_inboxes - string - Use this Clickwrap for Inboxes?
        ///   use_with_users - string - Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.
        /// </summary>
        public static async Task<Clickwrap> Create(

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
            if (parameters.ContainsKey("body") && !(parameters["body"] is string))
            {
                throw new ArgumentException("Bad parameter: body must be of type string", "parameters[\"body\"]");
            }
            if (parameters.ContainsKey("use_with_bundles") && !(parameters["use_with_bundles"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_bundles must be of type string", "parameters[\"use_with_bundles\"]");
            }
            if (parameters.ContainsKey("use_with_inboxes") && !(parameters["use_with_inboxes"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_inboxes must be of type string", "parameters[\"use_with_inboxes\"]");
            }
            if (parameters.ContainsKey("use_with_users") && !(parameters["use_with_users"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_users must be of type string", "parameters[\"use_with_users\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/clickwraps", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Clickwrap>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Name of the Clickwrap agreement (used when selecting from multiple Clickwrap agreements.)
        ///   body - string - Body text of Clickwrap (supports Markdown formatting).
        ///   use_with_bundles - string - Use this Clickwrap for Bundles?
        ///   use_with_inboxes - string - Use this Clickwrap for Inboxes?
        ///   use_with_users - string - Use this Clickwrap for User Registrations?  Note: This only applies to User Registrations where the User is invited to your Files.com site using an E-Mail invitation process where they then set their own password.
        /// </summary>
        public static async Task<Clickwrap> Update(
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
            if (parameters.ContainsKey("body") && !(parameters["body"] is string))
            {
                throw new ArgumentException("Bad parameter: body must be of type string", "parameters[\"body\"]");
            }
            if (parameters.ContainsKey("use_with_bundles") && !(parameters["use_with_bundles"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_bundles must be of type string", "parameters[\"use_with_bundles\"]");
            }
            if (parameters.ContainsKey("use_with_inboxes") && !(parameters["use_with_inboxes"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_inboxes must be of type string", "parameters[\"use_with_inboxes\"]");
            }
            if (parameters.ContainsKey("use_with_users") && !(parameters["use_with_users"] is string))
            {
                throw new ArgumentException("Bad parameter: use_with_users must be of type string", "parameters[\"use_with_users\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/clickwraps/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<Clickwrap>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Clickwrap> Delete(
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

            string responseJson = await FilesClient.SendRequest($"/clickwraps/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Clickwrap>(responseJson);
        }

        public static async Task<Clickwrap> Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}