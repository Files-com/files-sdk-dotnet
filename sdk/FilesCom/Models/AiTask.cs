using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class AiTask : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public AiTask() : this(null, null) { }

        public AiTask(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("prompt"))
            {
                this.attributes.Add("prompt", null);
            }
            if (!this.attributes.ContainsKey("permission_set"))
            {
                this.attributes.Add("permission_set", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("source"))
            {
                this.attributes.Add("source", null);
            }
            if (!this.attributes.ContainsKey("disabled"))
            {
                this.attributes.Add("disabled", false);
            }
            if (!this.attributes.ContainsKey("trigger"))
            {
                this.attributes.Add("trigger", null);
            }
            if (!this.attributes.ContainsKey("trigger_actions"))
            {
                this.attributes.Add("trigger_actions", new string[0]);
            }
            if (!this.attributes.ContainsKey("interval"))
            {
                this.attributes.Add("interval", null);
            }
            if (!this.attributes.ContainsKey("recurring_day"))
            {
                this.attributes.Add("recurring_day", null);
            }
            if (!this.attributes.ContainsKey("schedule_days_of_week"))
            {
                this.attributes.Add("schedule_days_of_week", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("schedule_times_of_day"))
            {
                this.attributes.Add("schedule_times_of_day", new string[0]);
            }
            if (!this.attributes.ContainsKey("schedule_time_zone"))
            {
                this.attributes.Add("schedule_time_zone", null);
            }
            if (!this.attributes.ContainsKey("holiday_region"))
            {
                this.attributes.Add("holiday_region", null);
            }
            if (!this.attributes.ContainsKey("human_readable_schedule"))
            {
                this.attributes.Add("human_readable_schedule", null);
            }
            if (!this.attributes.ContainsKey("last_run_at"))
            {
                this.attributes.Add("last_run_at", null);
            }
            if (!this.attributes.ContainsKey("master_admin_user_id"))
            {
                this.attributes.Add("master_admin_user_id", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
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

        void IModel.SetOptions(Dictionary<string, object> options)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// AI Task ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Workspace ID. `0` means the default workspace.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// AI Task name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// AI Task description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; }
        }

        /// <summary>
        /// Prompt sent when this AI Task is invoked.
        /// </summary>
        [JsonPropertyName("prompt")]
        public string Prompt
        {
            get { return (string)attributes["prompt"]; }
            set { attributes["prompt"] = value; }
        }

        /// <summary>
        /// Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
        /// </summary>
        [JsonPropertyName("permission_set")]
        public string PermissionSet
        {
            get { return (string)attributes["permission_set"]; }
            set { attributes["permission_set"] = value; }
        }

        /// <summary>
        /// Path scope used for action-triggered AI Tasks. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Source glob used with `path` for action-triggered AI Tasks.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string)attributes["source"]; }
            set { attributes["source"] = value; }
        }

        /// <summary>
        /// If true, this AI Task will not run.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; }
        }

        /// <summary>
        /// How this AI Task is triggered.
        /// </summary>
        [JsonPropertyName("trigger")]
        public string Trigger
        {
            get { return (string)attributes["trigger"]; }
            set { attributes["trigger"] = value; }
        }

        /// <summary>
        /// If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
        /// </summary>
        [JsonPropertyName("trigger_actions")]
        public string[] TriggerActions
        {
            get { return (string[])attributes["trigger_actions"]; }
            set { attributes["trigger_actions"] = value; }
        }

        /// <summary>
        /// If trigger is `daily`, this specifies how often to run the AI Task.
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval
        {
            get { return (string)attributes["interval"]; }
            set { attributes["interval"] = value; }
        }

        /// <summary>
        /// If trigger is `daily`, this selects the day number inside the chosen interval.
        /// </summary>
        [JsonPropertyName("recurring_day")]
        public Nullable<Int64> RecurringDay
        {
            get { return (Nullable<Int64>)attributes["recurring_day"]; }
            set { attributes["recurring_day"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        /// </summary>
        [JsonPropertyName("schedule_days_of_week")]
        public Nullable<Int64>[] ScheduleDaysOfWeek
        {
            get { return (Nullable<Int64>[])attributes["schedule_days_of_week"]; }
            set { attributes["schedule_days_of_week"] = value; }
        }

        /// <summary>
        /// Times of day in HH:MM format for scheduled AI Tasks.
        /// </summary>
        [JsonPropertyName("schedule_times_of_day")]
        public string[] ScheduleTimesOfDay
        {
            get { return (string[])attributes["schedule_times_of_day"]; }
            set { attributes["schedule_times_of_day"] = value; }
        }

        /// <summary>
        /// Time zone used by the AI Task schedule.
        /// </summary>
        [JsonPropertyName("schedule_time_zone")]
        public string ScheduleTimeZone
        {
            get { return (string)attributes["schedule_time_zone"]; }
            set { attributes["schedule_time_zone"] = value; }
        }

        /// <summary>
        /// Optional holiday region used by scheduled AI Tasks.
        /// </summary>
        [JsonPropertyName("holiday_region")]
        public string HolidayRegion
        {
            get { return (string)attributes["holiday_region"]; }
            set { attributes["holiday_region"] = value; }
        }

        /// <summary>
        /// Human-readable schedule description.
        /// </summary>
        [JsonPropertyName("human_readable_schedule")]
        public string HumanReadableSchedule
        {
            get { return (string)attributes["human_readable_schedule"]; }
            set { attributes["human_readable_schedule"] = value; }
        }

        /// <summary>
        /// Most recent successful invocation time.
        /// </summary>
        [JsonPropertyName("last_run_at")]
        public Nullable<DateTime> LastRunAt
        {
            get { return (Nullable<DateTime>)attributes["last_run_at"]; }
            set { attributes["last_run_at"] = value; }
        }

        /// <summary>
        /// Master User ID used for AI Task invocations.
        /// </summary>
        [JsonPropertyName("master_admin_user_id")]
        public Nullable<Int64> MasterAdminUserId
        {
            get { return (Nullable<Int64>)attributes["master_admin_user_id"]; }
            set { attributes["master_admin_user_id"] = value; }
        }

        /// <summary>
        /// Creation time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Last update time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>)attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }

        /// <summary>
        /// Manually Run AI Task
        /// </summary>
        public async Task ManualRun(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/ai_tasks/{System.Uri.EscapeDataString(attributes["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   description - string - AI Task description.
        ///   disabled - boolean - If true, this AI Task will not run.
        ///   holiday_region - string - Optional holiday region used by scheduled AI Tasks.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the AI Task.
        ///   name - string - AI Task name.
        ///   path - string - Path scope used for action-triggered AI Tasks.
        ///   permission_set - string - Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
        ///   prompt - string - Prompt sent when this AI Task is invoked.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_time_zone - string - Time zone used by the AI Task schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for scheduled AI Tasks.
        ///   source - string - Source glob used with `path` for action-triggered AI Tasks.
        ///   trigger - string - How this AI Task is triggered.
        ///   trigger_actions - array(string) - If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public async Task<AiTask> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }
            if (parameters.ContainsKey("prompt") && !(parameters["prompt"] is string))
            {
                throw new ArgumentException("Bad parameter: prompt must be of type string", "parameters[\"prompt\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_actions") && !(parameters["trigger_actions"] is string[]))
            {
                throw new ArgumentException("Bad parameter: trigger_actions must be of type string[]", "parameters[\"trigger_actions\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/ai_tasks/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<AiTask>(responseJson, options);
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

            await FilesClient.SendRequest($"/ai_tasks/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await AiTask.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `id`, `disabled` or `updated_at`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled`, `trigger` or `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.
        /// </summary>
        public static FilesList<AiTask> List(

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

            return new FilesList<AiTask>($"/ai_tasks", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<AiTask> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Ai Task ID.
        /// </summary>
        public static async Task<AiTask> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/ai_tasks/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<AiTask>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<AiTask> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   description - string - AI Task description.
        ///   disabled - boolean - If true, this AI Task will not run.
        ///   holiday_region - string - Optional holiday region used by scheduled AI Tasks.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the AI Task.
        ///   name (required) - string - AI Task name.
        ///   path - string - Path scope used for action-triggered AI Tasks.
        ///   permission_set - string - Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
        ///   prompt (required) - string - Prompt sent when this AI Task is invoked.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_time_zone - string - Time zone used by the AI Task schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for scheduled AI Tasks.
        ///   source - string - Source glob used with `path` for action-triggered AI Tasks.
        ///   trigger - string - How this AI Task is triggered.
        ///   trigger_actions - array(string) - If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public static async Task<AiTask> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("name") || parameters["name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: name", "parameters[\"name\"]");
            }
            if (!parameters.ContainsKey("prompt") || parameters["prompt"] == null)
            {
                throw new ArgumentNullException("Parameter missing: prompt", "parameters[\"prompt\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }
            if (parameters.ContainsKey("prompt") && !(parameters["prompt"] is string))
            {
                throw new ArgumentException("Bad parameter: prompt must be of type string", "parameters[\"prompt\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_actions") && !(parameters["trigger_actions"] is string[]))
            {
                throw new ArgumentException("Bad parameter: trigger_actions must be of type string[]", "parameters[\"trigger_actions\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/ai_tasks", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<AiTask>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Manually Run AI Task
        /// </summary>
        public static async Task ManualRun(
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

            await FilesClient.SendRequest($"/ai_tasks/{System.Uri.EscapeDataString(parameters["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   description - string - AI Task description.
        ///   disabled - boolean - If true, this AI Task will not run.
        ///   holiday_region - string - Optional holiday region used by scheduled AI Tasks.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the AI Task.
        ///   name - string - AI Task name.
        ///   path - string - Path scope used for action-triggered AI Tasks.
        ///   permission_set - string - Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
        ///   prompt - string - Prompt sent when this AI Task is invoked.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_time_zone - string - Time zone used by the AI Task schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for scheduled AI Tasks.
        ///   source - string - Source glob used with `path` for action-triggered AI Tasks.
        ///   trigger - string - How this AI Task is triggered.
        ///   trigger_actions - array(string) - If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public static async Task<AiTask> Update(
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
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("permission_set") && !(parameters["permission_set"] is string))
            {
                throw new ArgumentException("Bad parameter: permission_set must be of type string", "parameters[\"permission_set\"]");
            }
            if (parameters.ContainsKey("prompt") && !(parameters["prompt"] is string))
            {
                throw new ArgumentException("Bad parameter: prompt must be of type string", "parameters[\"prompt\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_actions") && !(parameters["trigger_actions"] is string[]))
            {
                throw new ArgumentException("Bad parameter: trigger_actions must be of type string[]", "parameters[\"trigger_actions\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/ai_tasks/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<AiTask>(responseJson, options);
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

            await FilesClient.SendRequest($"/ai_tasks/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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