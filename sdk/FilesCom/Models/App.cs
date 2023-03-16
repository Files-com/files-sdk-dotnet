using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class App
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public App() : this(null, null) { }

        public App(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("extended_description"))
            {
                this.attributes.Add("extended_description", null);
            }
            if (!this.attributes.ContainsKey("short_description"))
            {
                this.attributes.Add("short_description", null);
            }
            if (!this.attributes.ContainsKey("documentation_links"))
            {
                this.attributes.Add("documentation_links", null);
            }
            if (!this.attributes.ContainsKey("icon_url"))
            {
                this.attributes.Add("icon_url", null);
            }
            if (!this.attributes.ContainsKey("logo_url"))
            {
                this.attributes.Add("logo_url", null);
            }
            if (!this.attributes.ContainsKey("screenshot_list_urls"))
            {
                this.attributes.Add("screenshot_list_urls", new string[0]);
            }
            if (!this.attributes.ContainsKey("logo_thumbnail_url"))
            {
                this.attributes.Add("logo_thumbnail_url", null);
            }
            if (!this.attributes.ContainsKey("sso_strategy_type"))
            {
                this.attributes.Add("sso_strategy_type", null);
            }
            if (!this.attributes.ContainsKey("remote_server_type"))
            {
                this.attributes.Add("remote_server_type", null);
            }
            if (!this.attributes.ContainsKey("folder_behavior_type"))
            {
                this.attributes.Add("folder_behavior_type", null);
            }
            if (!this.attributes.ContainsKey("external_homepage_url"))
            {
                this.attributes.Add("external_homepage_url", null);
            }
            if (!this.attributes.ContainsKey("marketing_youtube_url"))
            {
                this.attributes.Add("marketing_youtube_url", null);
            }
            if (!this.attributes.ContainsKey("tutorial_youtube_url"))
            {
                this.attributes.Add("tutorial_youtube_url", null);
            }
            if (!this.attributes.ContainsKey("app_type"))
            {
                this.attributes.Add("app_type", null);
            }
            if (!this.attributes.ContainsKey("featured"))
            {
                this.attributes.Add("featured", null);
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
        /// Name of the App
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            private set { attributes["name"] = value; }
        }

        /// <summary>
        /// Long form description of the App
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("extended_description")]
        public string ExtendedDescription
        {
            get { return (string)attributes["extended_description"]; }
            private set { attributes["extended_description"] = value; }
        }

        /// <summary>
        /// Short description of the App
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("short_description")]
        public string ShortDescription
        {
            get { return (string)attributes["short_description"]; }
            private set { attributes["short_description"] = value; }
        }

        /// <summary>
        /// Collection of named links to documentation
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("documentation_links")]
        public object DocumentationLinks
        {
            get { return (object)attributes["documentation_links"]; }
            private set { attributes["documentation_links"] = value; }
        }

        /// <summary>
        /// App icon
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("icon_url")]
        public string IconUrl
        {
            get { return (string)attributes["icon_url"]; }
            private set { attributes["icon_url"] = value; }
        }

        /// <summary>
        /// Full size logo for the App
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logo_url")]
        public string LogoUrl
        {
            get { return (string)attributes["logo_url"]; }
            private set { attributes["logo_url"] = value; }
        }

        /// <summary>
        /// Screenshots of the App
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("screenshot_list_urls")]
        public string[] ScreenshotListUrls
        {
            get { return (string[])attributes["screenshot_list_urls"]; }
            private set { attributes["screenshot_list_urls"] = value; }
        }

        /// <summary>
        /// Logo thumbnail for the App
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logo_thumbnail_url")]
        public string LogoThumbnailUrl
        {
            get { return (string)attributes["logo_thumbnail_url"]; }
            private set { attributes["logo_thumbnail_url"] = value; }
        }

        /// <summary>
        /// Associated SSO Strategy type, if any
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("sso_strategy_type")]
        public string SsoStrategyType
        {
            get { return (string)attributes["sso_strategy_type"]; }
            private set { attributes["sso_strategy_type"] = value; }
        }

        /// <summary>
        /// Associated Remote Server type, if any
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("remote_server_type")]
        public string RemoteServerType
        {
            get { return (string)attributes["remote_server_type"]; }
            private set { attributes["remote_server_type"] = value; }
        }

        /// <summary>
        /// Associated Folder Behavior type, if any
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("folder_behavior_type")]
        public string FolderBehaviorType
        {
            get { return (string)attributes["folder_behavior_type"]; }
            private set { attributes["folder_behavior_type"] = value; }
        }

        /// <summary>
        /// Link to external homepage
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("external_homepage_url")]
        public string ExternalHomepageUrl
        {
            get { return (string)attributes["external_homepage_url"]; }
            private set { attributes["external_homepage_url"] = value; }
        }

        /// <summary>
        /// Marketing video page
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("marketing_youtube_url")]
        public string MarketingYoutubeUrl
        {
            get { return (string)attributes["marketing_youtube_url"]; }
            private set { attributes["marketing_youtube_url"] = value; }
        }

        /// <summary>
        /// Tutorial video page
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tutorial_youtube_url")]
        public string TutorialYoutubeUrl
        {
            get { return (string)attributes["tutorial_youtube_url"]; }
            private set { attributes["tutorial_youtube_url"] = value; }
        }

        /// <summary>
        /// The type of the App
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("app_type")]
        public string AppType
        {
            get { return (string)attributes["app_type"]; }
            private set { attributes["app_type"] = value; }
        }

        /// <summary>
        /// Is featured on the App listing?
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("featured")]
        public bool Featured
        {
            get { return (bool)attributes["featured"]; }
            private set { attributes["featured"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[name]=desc`). Valid fields are `name` and `app_type`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `name` and `app_type`. Valid field combinations are `[ name, app_type ]` and `[ app_type, name ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
        /// </summary>
        public static async Task<App[]> List(

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
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
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