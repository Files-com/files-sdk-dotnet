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
                this.attributes.Add("screenshot_list_urls", null);
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
        /// App icon
        /// </summary>
        [JsonPropertyName("icon_url")]
        public string IconUrl
        {
            get { return (string) attributes["icon_url"]; }
        }

        /// <summary>
        /// Full size logo for the App
        /// </summary>
        [JsonPropertyName("logo_url")]
        public string LogoUrl
        {
            get { return (string) attributes["logo_url"]; }
        }

        /// <summary>
        /// Screenshots of the App
        /// </summary>
        [JsonPropertyName("screenshot_list_urls")]
        public string ScreenshotListUrls
        {
            get { return (string) attributes["screenshot_list_urls"]; }
        }

        /// <summary>
        /// Logo thumbnail for the App
        /// </summary>
        [JsonPropertyName("logo_thumbnail_url")]
        public string LogoThumbnailUrl
        {
            get { return (string) attributes["logo_thumbnail_url"]; }
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
        ///   cursor - string - Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `name` and `app_type`.
        ///   filter - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `name` and `app_type`.
        ///   filter_gt - object - If set, return records where the specifiied field is greater than the supplied value. Valid fields are `name` and `app_type`.
        ///   filter_gteq - object - If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `name` and `app_type`.
        ///   filter_like - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `name` and `app_type`.
        ///   filter_lt - object - If set, return records where the specifiied field is less than the supplied value. Valid fields are `name` and `app_type`.
        ///   filter_lteq - object - If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `name` and `app_type`.
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
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object ))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_like") && !(parameters["filter_like"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_like must be of type object", "parameters[\"filter_like\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
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

