using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class BundleDownload
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public BundleDownload() : this(null, null) { }

        public BundleDownload(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("download_method"))
            {
                this.attributes.Add("download_method", null);
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
        /// Download method (file or full_zip)
        /// </summary>
        [JsonPropertyName("download_method")]
        public string DownloadMethod
        {
            get { return (string) attributes["download_method"]; }
        }

        /// <summary>
        /// Download path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
        }

        /// <summary>
        /// Download date/time
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
        ///   bundle_registration_id (required) - int64 - BundleRegistration ID
        /// </summary>
        public static async Task<BundleDownload[]> List(
            
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
            if (parameters.ContainsKey("bundle_registration_id") && !(parameters["bundle_registration_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: bundle_registration_id must be of type Nullable<Int64>", "parameters[\"bundle_registration_id\"]");
            }
            if (!parameters.ContainsKey("bundle_registration_id") || parameters["bundle_registration_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: bundle_registration_id", "parameters[\"bundle_registration_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/bundle_downloads", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<BundleDownload[]>(responseJson);
        }

        public static async Task<BundleDownload[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

