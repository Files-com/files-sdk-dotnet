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
        public Automation()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("automation", null);
            this.attributes.Add("source", null);
            this.attributes.Add("destination", null);
            this.attributes.Add("destination_replace_from", null);
            this.attributes.Add("destination_replace_to", null);
            this.attributes.Add("interval", null);
            this.attributes.Add("next_process_on", null);
            this.attributes.Add("path", null);
            this.attributes.Add("realtime", false);
            this.attributes.Add("user_id", null);
            this.attributes.Add("user_ids", new object[0]);
            this.attributes.Add("group_ids", new object[0]);
        }

        public Automation(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
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
        public object[] UserIds
        {
            get { return (object[]) attributes["user_ids"]; }
            set { attributes["user_ids"] = value; }
        }

        /// <summary>
        /// IDs of Groups for the Automation (i.e. who to Request File from)
        /// </summary>
        [JsonPropertyName("group_ids")]
        public object[] GroupIds
        {
            get { return (object[]) attributes["group_ids"]; }
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
                // TODO: Remove this cast after return types are implemented
                var newObj = (Automation) await Automation.Create(this.attributes, this.options);
                this.attributes = newObj.attributes;
            }
        }

        /// <summary>
        /// Parameters:
        ///   page - int64 - Current page number.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string - Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
        ///   automation - string - Type of automation to filter by.
        /// </summary>
        public static async Task<Automation[]> List(
            
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

