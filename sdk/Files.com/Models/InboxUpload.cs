using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class InboxUpload
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public InboxUpload() : this(null, null) { }

        public InboxUpload(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("inbox_registration"))
            {
                this.attributes.Add("inbox_registration", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
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
        /// </summary>
        [JsonPropertyName("inbox_registration")]
        public object InboxRegistration
        {
            get { return (object) attributes["inbox_registration"]; }
        }

        /// <summary>
        /// Upload path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
        }

        /// <summary>
        /// Upload date/time
        /// </summary>
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>) attributes["created_at"]; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   inbox_registration_id - int64 - InboxRegistration ID
        ///   inbox_id - int64 - Inbox ID
        /// </summary>
        public static async Task<InboxUpload[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("inbox_registration_id") && !(parameters["inbox_registration_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: inbox_registration_id must be of type Nullable<Int64>", "parameters[\"inbox_registration_id\"]");
            }
            if (parameters.ContainsKey("inbox_id") && !(parameters["inbox_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: inbox_id must be of type Nullable<Int64>", "parameters[\"inbox_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/inbox_uploads", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<InboxUpload[]>(responseJson);
        }

        public static async Task<InboxUpload[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

