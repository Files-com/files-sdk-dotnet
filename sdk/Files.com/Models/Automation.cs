using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class Automation
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Automation() : this(null, null) { }

        public Automation(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("automation"))
            {
                this.attributes.Add("automation", null);
            }
            if (!this.attributes.ContainsKey("source"))
            {
                this.attributes.Add("source", null);
            }
            if (!this.attributes.ContainsKey("destination"))
            {
                this.attributes.Add("destination", null);
            }
            if (!this.attributes.ContainsKey("destination_replace_from"))
            {
                this.attributes.Add("destination_replace_from", null);
            }
            if (!this.attributes.ContainsKey("destination_replace_to"))
            {
                this.attributes.Add("destination_replace_to", null);
            }
            if (!this.attributes.ContainsKey("interval"))
            {
                this.attributes.Add("interval", null);
            }
            if (!this.attributes.ContainsKey("next_process_on"))
            {
                this.attributes.Add("next_process_on", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("realtime"))
            {
                this.attributes.Add("realtime", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("user_ids"))
            {
                this.attributes.Add("user_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("group_ids"))
            {
                this.attributes.Add("group_ids", new Nullable<Int64>[0]);
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
        /// Automation ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Automation type
        /// </summary>
        [JsonPropertyName("automation")]
        public string AutomationType
        {
            get { return (string) attributes["automation"]; }
            set { attributes["automation"] = value; }
        }

        /// <summary>
        /// Source Path
        /// </summary>
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string) attributes["source"]; }
            set { attributes["source"] = value; }
        }

        /// <summary>
        /// Destination Path
        /// </summary>
        [JsonPropertyName("destination")]
        public string Destination
        {
            get { return (string) attributes["destination"]; }
            set { attributes["destination"] = value; }
        }

        /// <summary>
        /// If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        /// </summary>
        [JsonPropertyName("destination_replace_from")]
        public string DestinationReplaceFrom
        {
            get { return (string) attributes["destination_replace_from"]; }
            set { attributes["destination_replace_from"] = value; }
        }

        /// <summary>
        /// If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        /// </summary>
        [JsonPropertyName("destination_replace_to")]
        public string DestinationReplaceTo
        {
            get { return (string) attributes["destination_replace_to"]; }
            set { attributes["destination_replace_to"] = value; }
        }

        /// <summary>
        /// How often to run this automation?  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval
        {
            get { return (string) attributes["interval"]; }
            set { attributes["interval"] = value; }
        }

        /// <summary>
        /// Date this automation will next run.
        /// </summary>
        [JsonPropertyName("next_process_on")]
        public string NextProcessOn
        {
            get { return (string) attributes["next_process_on"]; }
            set { attributes["next_process_on"] = value; }
        }

        /// <summary>
        /// Path on which this Automation runs.  Supports globs. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Does this automation run in real time?  This is a read-only property based on automation type.
        /// </summary>
        [JsonPropertyName("realtime")]
        public bool Realtime
        {
            get { return (bool) attributes["realtime"]; }
            set { attributes["realtime"] = value; }
        }

        /// <summary>
        /// User ID of the Automation's creator.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// IDs of Users for the Automation (i.e. who to Request File from)
        /// </summary>
        [JsonPropertyName("user_ids")]
        public Nullable<Int64>[] UserIds
        {
            get { return (Nullable<Int64>[]) attributes["user_ids"]; }
            set { attributes["user_ids"] = value; }
        }

        /// <summary>
        /// IDs of Groups for the Automation (i.e. who to Request File from)
        /// </summary>
        [JsonPropertyName("group_ids")]
        public Nullable<Int64>[] GroupIds
        {
            get { return (Nullable<Int64>[]) attributes["group_ids"]; }
            set { attributes["group_ids"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   automation (required) - string - Type of automation.  One of: `create_folder`, `request_file`, `request_move`
        ///   source - string - Source Path
        ///   destination - string - Destination Path
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        /// </summary>
        public async Task<Automation> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id")) {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("automation") && !(parameters["automation"] is string ))
            {
                throw new ArgumentException("Bad parameter: automation must be of type string", "parameters[\"automation\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string ))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("destination_replace_from") && !(parameters["destination_replace_from"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination_replace_from must be of type string", "parameters[\"destination_replace_from\"]");
            }
            if (parameters.ContainsKey("destination_replace_to") && !(parameters["destination_replace_to"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination_replace_to must be of type string", "parameters[\"destination_replace_to\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string ))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("automation") || parameters["automation"] == null)
            {
                throw new ArgumentNullException("Parameter missing: automation", "parameters[\"automation\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/automations/{attributes["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Automation>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Automation> Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id")) {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/automations/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Automation>(responseJson);
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
                var newObj = await Automation.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `site_id` and `automation`.
        ///   filter - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `automation`.
        ///   filter_gt - object - If set, return records where the specifiied field is greater than the supplied value. Valid fields are `automation`.
        ///   filter_gteq - object - If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `automation`.
        ///   filter_like - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `automation`.
        ///   filter_lt - object - If set, return records where the specifiied field is less than the supplied value. Valid fields are `automation`.
        ///   filter_lteq - object - If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `automation`.
        ///   automation - string - DEPRECATED: Type of automation to filter by. Use `filter[automation]` instead.
        /// </summary>
        public static async Task<Automation[]> List(
            
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
            if (parameters.ContainsKey("automation") && !(parameters["automation"] is string ))
            {
                throw new ArgumentException("Bad parameter: automation must be of type string", "parameters[\"automation\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/automations", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Automation[]>(responseJson);
        }

        public static async Task<Automation[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Automation ID.
        /// </summary>
        public static async Task<Automation> Find(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/automations/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Automation>(responseJson);
        }

        public static async Task<Automation> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   automation (required) - string - Type of automation.  One of: `create_folder`, `request_file`, `request_move`
        ///   source - string - Source Path
        ///   destination - string - Destination Path
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        /// </summary>
        public static async Task<Automation> Create(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("automation") && !(parameters["automation"] is string ))
            {
                throw new ArgumentException("Bad parameter: automation must be of type string", "parameters[\"automation\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string ))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("destination_replace_from") && !(parameters["destination_replace_from"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination_replace_from must be of type string", "parameters[\"destination_replace_from\"]");
            }
            if (parameters.ContainsKey("destination_replace_to") && !(parameters["destination_replace_to"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination_replace_to must be of type string", "parameters[\"destination_replace_to\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string ))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
            }
            if (!parameters.ContainsKey("automation") || parameters["automation"] == null)
            {
                throw new ArgumentNullException("Parameter missing: automation", "parameters[\"automation\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/automations", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Automation>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   automation (required) - string - Type of automation.  One of: `create_folder`, `request_file`, `request_move`
        ///   source - string - Source Path
        ///   destination - string - Destination Path
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        /// </summary>
        public static async Task<Automation> Update(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("automation") && !(parameters["automation"] is string ))
            {
                throw new ArgumentException("Bad parameter: automation must be of type string", "parameters[\"automation\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string ))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (parameters.ContainsKey("destination_replace_from") && !(parameters["destination_replace_from"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination_replace_from must be of type string", "parameters[\"destination_replace_from\"]");
            }
            if (parameters.ContainsKey("destination_replace_to") && !(parameters["destination_replace_to"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination_replace_to must be of type string", "parameters[\"destination_replace_to\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string ))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string ))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("automation") || parameters["automation"] == null)
            {
                throw new ArgumentNullException("Parameter missing: automation", "parameters[\"automation\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/automations/{parameters["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Automation>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Automation> Delete(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/automations/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Automation>(responseJson);
        }

        public static async Task<Automation> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

