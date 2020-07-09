using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class App
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public App()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("name", null);
            this.attributes.Add("extended_description", null);
            this.attributes.Add("documentation_links", null);
            this.attributes.Add("sso_strategy_type", null);
            this.attributes.Add("remote_server_type", null);
            this.attributes.Add("folder_behavior_type", null);
            this.attributes.Add("external_homepage_url", null);
            this.attributes.Add("app_type", null);
            this.attributes.Add("featured", null);
        }

        public App(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Name of the App
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string) attributes["name"]; }
        }

        /// <summary>
        /// Long form description of the App
        /// </summary>
        [JsonPropertyName("extended_description")]
        public string ExtendedDescription
        {
            get { return (string) attributes["extended_description"]; }
        }

        /// <summary>
        /// Collection of named links to documentation
        /// </summary>
        [JsonPropertyName("documentation_links")]
        public string DocumentationLinks
        {
            get { return (string) attributes["documentation_links"]; }
        }

        /// <summary>
        /// Associated SSO Strategy type, if any
        /// </summary>
        [JsonPropertyName("sso_strategy_type")]
        public string SsoStrategyType
        {
            get { return (string) attributes["sso_strategy_type"]; }
        }

        /// <summary>
        /// Associated Remote Server type, if any
        /// </summary>
        [JsonPropertyName("remote_server_type")]
        public string RemoteServerType
        {
            get { return (string) attributes["remote_server_type"]; }
        }

        /// <summary>
        /// Associated Folder Behavior type, if any
        /// </summary>
        [JsonPropertyName("folder_behavior_type")]
        public string FolderBehaviorType
        {
            get { return (string) attributes["folder_behavior_type"]; }
        }

        /// <summary>
        /// Link to external homepage
        /// </summary>
        [JsonPropertyName("external_homepage_url")]
        public string ExternalHomepageUrl
        {
            get { return (string) attributes["external_homepage_url"]; }
        }

        /// <summary>
        /// The type of the App
        /// </summary>
        [JsonPropertyName("app_type")]
        public string AppType
        {
            get { return (string) attributes["app_type"]; }
        }

        /// <summary>
        /// Is featured on the App listing?
        /// </summary>
        [JsonPropertyName("featured")]
        public bool Featured
        {
            get { return (bool) attributes["featured"]; }
        }



        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        /// </summary>
        public static async Task<App[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

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

            string responseJson = await FilesClient.SendRequest($"/apps", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<App[]>(responseJson);
        }

        public static async Task<App[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

    }
}

