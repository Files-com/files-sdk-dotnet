using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Expectation
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Expectation() : this(null, null) { }

        public Expectation(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("source"))
            {
                this.attributes.Add("source", null);
            }
            if (!this.attributes.ContainsKey("exclude_pattern"))
            {
                this.attributes.Add("exclude_pattern", null);
            }
            if (!this.attributes.ContainsKey("disabled"))
            {
                this.attributes.Add("disabled", false);
            }
            if (!this.attributes.ContainsKey("expectations_version"))
            {
                this.attributes.Add("expectations_version", null);
            }
            if (!this.attributes.ContainsKey("trigger"))
            {
                this.attributes.Add("trigger", null);
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
            if (!this.attributes.ContainsKey("lookback_interval"))
            {
                this.attributes.Add("lookback_interval", null);
            }
            if (!this.attributes.ContainsKey("late_acceptance_interval"))
            {
                this.attributes.Add("late_acceptance_interval", null);
            }
            if (!this.attributes.ContainsKey("inactivity_interval"))
            {
                this.attributes.Add("inactivity_interval", null);
            }
            if (!this.attributes.ContainsKey("max_open_interval"))
            {
                this.attributes.Add("max_open_interval", null);
            }
            if (!this.attributes.ContainsKey("criteria"))
            {
                this.attributes.Add("criteria", null);
            }
            if (!this.attributes.ContainsKey("last_evaluated_at"))
            {
                this.attributes.Add("last_evaluated_at", null);
            }
            if (!this.attributes.ContainsKey("last_success_at"))
            {
                this.attributes.Add("last_success_at", null);
            }
            if (!this.attributes.ContainsKey("last_failure_at"))
            {
                this.attributes.Add("last_failure_at", null);
            }
            if (!this.attributes.ContainsKey("last_result"))
            {
                this.attributes.Add("last_result", null);
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

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// Expectation ID
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
        /// Expectation name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Expectation description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; }
        }

        /// <summary>
        /// Path scope for the expectation. Supports workspace-relative presentation. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Source glob used to select candidate files.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string)attributes["source"]; }
            set { attributes["source"] = value; }
        }

        /// <summary>
        /// Optional source exclusion glob.
        /// </summary>
        [JsonPropertyName("exclude_pattern")]
        public string ExcludePattern
        {
            get { return (string)attributes["exclude_pattern"]; }
            set { attributes["exclude_pattern"] = value; }
        }

        /// <summary>
        /// If true, the expectation is disabled.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; }
        }

        /// <summary>
        /// Criteria schema version for this expectation.
        /// </summary>
        [JsonPropertyName("expectations_version")]
        public Nullable<Int64> ExpectationsVersion
        {
            get { return (Nullable<Int64>)attributes["expectations_version"]; }
            set { attributes["expectations_version"] = value; }
        }

        /// <summary>
        /// How this expectation opens windows.
        /// </summary>
        [JsonPropertyName("trigger")]
        public string Trigger
        {
            get { return (string)attributes["trigger"]; }
            set { attributes["trigger"] = value; }
        }

        /// <summary>
        /// If trigger is `daily`, this specifies how often to run the expectation.
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
        /// Times of day in HH:MM format for schedule-driven expectations.
        /// </summary>
        [JsonPropertyName("schedule_times_of_day")]
        public string[] ScheduleTimesOfDay
        {
            get { return (string[])attributes["schedule_times_of_day"]; }
            set { attributes["schedule_times_of_day"] = value; }
        }

        /// <summary>
        /// Time zone used by the expectation schedule.
        /// </summary>
        [JsonPropertyName("schedule_time_zone")]
        public string ScheduleTimeZone
        {
            get { return (string)attributes["schedule_time_zone"]; }
            set { attributes["schedule_time_zone"] = value; }
        }

        /// <summary>
        /// Optional holiday region used by schedule-driven expectations.
        /// </summary>
        [JsonPropertyName("holiday_region")]
        public string HolidayRegion
        {
            get { return (string)attributes["holiday_region"]; }
            set { attributes["holiday_region"] = value; }
        }

        /// <summary>
        /// How many seconds before the due boundary the window starts.
        /// </summary>
        [JsonPropertyName("lookback_interval")]
        public Nullable<Int64> LookbackInterval
        {
            get { return (Nullable<Int64>)attributes["lookback_interval"]; }
            set { attributes["lookback_interval"] = value; }
        }

        /// <summary>
        /// How many seconds a schedule-driven window may remain eligible to close as late.
        /// </summary>
        [JsonPropertyName("late_acceptance_interval")]
        public Nullable<Int64> LateAcceptanceInterval
        {
            get { return (Nullable<Int64>)attributes["late_acceptance_interval"]; }
            set { attributes["late_acceptance_interval"] = value; }
        }

        /// <summary>
        /// How many quiet seconds are required before final closure.
        /// </summary>
        [JsonPropertyName("inactivity_interval")]
        public Nullable<Int64> InactivityInterval
        {
            get { return (Nullable<Int64>)attributes["inactivity_interval"]; }
            set { attributes["inactivity_interval"] = value; }
        }

        /// <summary>
        /// Hard-stop duration in seconds for unscheduled expectations.
        /// </summary>
        [JsonPropertyName("max_open_interval")]
        public Nullable<Int64> MaxOpenInterval
        {
            get { return (Nullable<Int64>)attributes["max_open_interval"]; }
            set { attributes["max_open_interval"] = value; }
        }

        /// <summary>
        /// Structured criteria v1 definition for the expectation.
        /// </summary>
        [JsonPropertyName("criteria")]
        public object Criteria
        {
            get { return (object)attributes["criteria"]; }
            set { attributes["criteria"] = value; }
        }

        /// <summary>
        /// Last time this expectation was evaluated.
        /// </summary>
        [JsonPropertyName("last_evaluated_at")]
        public Nullable<DateTime> LastEvaluatedAt
        {
            get { return (Nullable<DateTime>)attributes["last_evaluated_at"]; }
            set { attributes["last_evaluated_at"] = value; }
        }

        /// <summary>
        /// Last time this expectation closed successfully.
        /// </summary>
        [JsonPropertyName("last_success_at")]
        public Nullable<DateTime> LastSuccessAt
        {
            get { return (Nullable<DateTime>)attributes["last_success_at"]; }
            set { attributes["last_success_at"] = value; }
        }

        /// <summary>
        /// Last time this expectation closed with a failure result.
        /// </summary>
        [JsonPropertyName("last_failure_at")]
        public Nullable<DateTime> LastFailureAt
        {
            get { return (Nullable<DateTime>)attributes["last_failure_at"]; }
            set { attributes["last_failure_at"] = value; }
        }

        /// <summary>
        /// Most recent terminal result for this expectation.
        /// </summary>
        [JsonPropertyName("last_result")]
        public string LastResult
        {
            get { return (string)attributes["last_result"]; }
            set { attributes["last_result"] = value; }
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
        /// Manually open an Expectation window
        /// </summary>
        public async Task<ExpectationEvaluation> TriggerEvaluation(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendStringRequest($"/expectations/{System.Uri.EscapeDataString(attributes["id"].ToString())}/trigger_evaluation", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ExpectationEvaluation>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Expectation name.
        ///   description - string - Expectation description.
        ///   path - string - Path scope for the expectation. Supports workspace-relative presentation.
        ///   source - string - Source glob used to select candidate files.
        ///   exclude_pattern - string - Optional source exclusion glob.
        ///   disabled - boolean - If true, the expectation is disabled.
        ///   trigger - string - How this expectation opens windows.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the expectation.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for schedule-driven expectations.
        ///   schedule_time_zone - string - Time zone used by the expectation schedule.
        ///   holiday_region - string - Optional holiday region used by schedule-driven expectations.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Structured criteria v1 definition for the expectation.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public async Task<Expectation> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("exclude_pattern") && !(parameters["exclude_pattern"] is string))
            {
                throw new ArgumentException("Bad parameter: exclude_pattern must be of type string", "parameters[\"exclude_pattern\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("lookback_interval") && !(parameters["lookback_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: lookback_interval must be of type Nullable<Int64>", "parameters[\"lookback_interval\"]");
            }
            if (parameters.ContainsKey("late_acceptance_interval") && !(parameters["late_acceptance_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: late_acceptance_interval must be of type Nullable<Int64>", "parameters[\"late_acceptance_interval\"]");
            }
            if (parameters.ContainsKey("inactivity_interval") && !(parameters["inactivity_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_interval must be of type Nullable<Int64>", "parameters[\"inactivity_interval\"]");
            }
            if (parameters.ContainsKey("max_open_interval") && !(parameters["max_open_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_open_interval must be of type Nullable<Int64>", "parameters[\"max_open_interval\"]");
            }
            if (parameters.ContainsKey("criteria") && !(parameters["criteria"] is object))
            {
                throw new ArgumentException("Bad parameter: criteria must be of type object", "parameters[\"criteria\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/expectations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Expectation>(responseJson, JsonUtil.Options);
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

            await FilesClient.SendRequest($"/expectations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await Expectation.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.
        /// </summary>
        public static FilesList<Expectation> List(

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

            return new FilesList<Expectation>($"/expectations", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Expectation> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation ID.
        /// </summary>
        public static async Task<Expectation> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/expectations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Expectation>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<Expectation> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Expectation name.
        ///   description - string - Expectation description.
        ///   path - string - Path scope for the expectation. Supports workspace-relative presentation.
        ///   source - string - Source glob used to select candidate files.
        ///   exclude_pattern - string - Optional source exclusion glob.
        ///   disabled - boolean - If true, the expectation is disabled.
        ///   trigger - string - How this expectation opens windows.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the expectation.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for schedule-driven expectations.
        ///   schedule_time_zone - string - Time zone used by the expectation schedule.
        ///   holiday_region - string - Optional holiday region used by schedule-driven expectations.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Structured criteria v1 definition for the expectation.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public static async Task<Expectation> Create(

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
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("exclude_pattern") && !(parameters["exclude_pattern"] is string))
            {
                throw new ArgumentException("Bad parameter: exclude_pattern must be of type string", "parameters[\"exclude_pattern\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("lookback_interval") && !(parameters["lookback_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: lookback_interval must be of type Nullable<Int64>", "parameters[\"lookback_interval\"]");
            }
            if (parameters.ContainsKey("late_acceptance_interval") && !(parameters["late_acceptance_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: late_acceptance_interval must be of type Nullable<Int64>", "parameters[\"late_acceptance_interval\"]");
            }
            if (parameters.ContainsKey("inactivity_interval") && !(parameters["inactivity_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_interval must be of type Nullable<Int64>", "parameters[\"inactivity_interval\"]");
            }
            if (parameters.ContainsKey("max_open_interval") && !(parameters["max_open_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_open_interval must be of type Nullable<Int64>", "parameters[\"max_open_interval\"]");
            }
            if (parameters.ContainsKey("criteria") && !(parameters["criteria"] is object))
            {
                throw new ArgumentException("Bad parameter: criteria must be of type object", "parameters[\"criteria\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/expectations", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Expectation>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Manually open an Expectation window
        /// </summary>
        public static async Task<ExpectationEvaluation> TriggerEvaluation(
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

            string responseJson = await FilesClient.SendStringRequest($"/expectations/{System.Uri.EscapeDataString(parameters["id"].ToString())}/trigger_evaluation", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<ExpectationEvaluation>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Expectation name.
        ///   description - string - Expectation description.
        ///   path - string - Path scope for the expectation. Supports workspace-relative presentation.
        ///   source - string - Source glob used to select candidate files.
        ///   exclude_pattern - string - Optional source exclusion glob.
        ///   disabled - boolean - If true, the expectation is disabled.
        ///   trigger - string - How this expectation opens windows.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the expectation.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for schedule-driven expectations.
        ///   schedule_time_zone - string - Time zone used by the expectation schedule.
        ///   holiday_region - string - Optional holiday region used by schedule-driven expectations.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Structured criteria v1 definition for the expectation.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        public static async Task<Expectation> Update(
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("exclude_pattern") && !(parameters["exclude_pattern"] is string))
            {
                throw new ArgumentException("Bad parameter: exclude_pattern must be of type string", "parameters[\"exclude_pattern\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("lookback_interval") && !(parameters["lookback_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: lookback_interval must be of type Nullable<Int64>", "parameters[\"lookback_interval\"]");
            }
            if (parameters.ContainsKey("late_acceptance_interval") && !(parameters["late_acceptance_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: late_acceptance_interval must be of type Nullable<Int64>", "parameters[\"late_acceptance_interval\"]");
            }
            if (parameters.ContainsKey("inactivity_interval") && !(parameters["inactivity_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: inactivity_interval must be of type Nullable<Int64>", "parameters[\"inactivity_interval\"]");
            }
            if (parameters.ContainsKey("max_open_interval") && !(parameters["max_open_interval"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: max_open_interval must be of type Nullable<Int64>", "parameters[\"max_open_interval\"]");
            }
            if (parameters.ContainsKey("criteria") && !(parameters["criteria"] is object))
            {
                throw new ArgumentException("Bad parameter: criteria must be of type object", "parameters[\"criteria\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/expectations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Expectation>(responseJson, JsonUtil.Options);
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

            await FilesClient.SendRequest($"/expectations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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