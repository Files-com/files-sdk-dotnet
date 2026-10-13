using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// ExpectationEntity model
    /// </summary>
    public class Expectation : IModel, IJsonOnDeserialized
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        // The names of the attributes set on this object, by its constructor or its properties, since it was made
        // or last came from Files.com. SaveAsync sends these, never the defaults of attributes nobody set.
        private readonly HashSet<string> unsaved = new HashSet<string>();
        // The JSON, when handed out since then, of the arrays among them that callers can change in place, directly or
        // through a model in them. SaveAsync compares each with the array's JSON now, and sends the ones that differ.
        private readonly Dictionary<string, string> handedOut = new Dictionary<string, string>();
        private static readonly string[] ArrayAttributeNames = { "recurring_days", "schedule_days_of_week", "schedule_times_of_day" };
        /// <summary>
        /// Makes a Expectation with no attributes set and no options.
        /// </summary>
        public Expectation() : this(null, null) { }

        /// <summary>
        /// Makes a Expectation with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Expectation(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

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
            if (!this.attributes.ContainsKey("recurring_days"))
            {
                this.attributes.Add("recurring_days", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("schedule_id"))
            {
                this.attributes.Add("schedule_id", null);
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

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            // The copy holds this object's own arrays, which the caller can change in place as through its properties.
            foreach (string name in ArrayAttributeNames)
            {
                HandedOut(name);
            }
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.client = client;
            this.origin = origin;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => origin;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[0]; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }

        // What is read from JSON, such as a response from Files.com, is not waiting to be saved.
        void IJsonOnDeserialized.OnDeserialized()
        {
            unsaved.Clear();
            handedOut.Clear();
        }


        /// <summary>
        /// Expectation ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Workspace ID. `0` means the default workspace.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Expectation name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Expectation description.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; unsaved.Add("description"); }
        }

        /// <summary>
        /// Path scope for the expectation. Supports workspace-relative presentation. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; unsaved.Add("path"); }
        }

        /// <summary>
        /// Source glob used to select candidate files.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string)attributes["source"]; }
            set { attributes["source"] = value; unsaved.Add("source"); }
        }

        /// <summary>
        /// Optional source exclusion glob.
        /// </summary>
        [JsonPropertyName("exclude_pattern")]
        public string ExcludePattern
        {
            get { return (string)attributes["exclude_pattern"]; }
            set { attributes["exclude_pattern"] = value; unsaved.Add("exclude_pattern"); }
        }

        /// <summary>
        /// If true, the expectation is disabled.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; unsaved.Add("disabled"); }
        }

        /// <summary>
        /// Criteria schema version for this expectation.
        /// </summary>
        [JsonPropertyName("expectations_version")]
        public Nullable<Int64> ExpectationsVersion
        {
            get { return (Nullable<Int64>)attributes["expectations_version"]; }
            set { attributes["expectations_version"] = value; unsaved.Add("expectations_version"); }
        }

        /// <summary>
        /// How this expectation opens windows.
        /// </summary>
        [JsonPropertyName("trigger")]
        public string Trigger
        {
            get { return (string)attributes["trigger"]; }
            set { attributes["trigger"] = value; unsaved.Add("trigger"); }
        }

        /// <summary>
        /// If trigger is `daily`, this specifies how often to run the expectation.
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval
        {
            get { return (string)attributes["interval"]; }
            set { attributes["interval"] = value; unsaved.Add("interval"); }
        }

        /// <summary>
        /// If trigger is `daily`, this selects the day number inside the chosen interval.
        /// </summary>
        [JsonPropertyName("recurring_day")]
        public Nullable<Int64> RecurringDay
        {
            get { return (Nullable<Int64>)attributes["recurring_day"]; }
            set { attributes["recurring_day"] = value; unsaved.Add("recurring_day"); }
        }

        /// <summary>
        /// If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        /// </summary>
        [JsonPropertyName("recurring_days")]
        public Nullable<Int64>[] RecurringDays
        {
            get { return (Nullable<Int64>[])HandedOut("recurring_days"); }
            set { attributes["recurring_days"] = value; unsaved.Add("recurring_days"); }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, the reusable Schedule used instead of the Expectation's schedule fields.
        /// </summary>
        [JsonPropertyName("schedule_id")]
        public Nullable<Int64> ScheduleId
        {
            get { return (Nullable<Int64>)attributes["schedule_id"]; }
            set { attributes["schedule_id"] = value; unsaved.Add("schedule_id"); }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        /// </summary>
        [JsonPropertyName("schedule_days_of_week")]
        public Nullable<Int64>[] ScheduleDaysOfWeek
        {
            get { return (Nullable<Int64>[])HandedOut("schedule_days_of_week"); }
            set { attributes["schedule_days_of_week"] = value; unsaved.Add("schedule_days_of_week"); }
        }

        /// <summary>
        /// Times of day in HH:MM format for the Expectation schedule.
        /// </summary>
        [JsonPropertyName("schedule_times_of_day")]
        public string[] ScheduleTimesOfDay
        {
            get { return (string[])HandedOut("schedule_times_of_day"); }
            set { attributes["schedule_times_of_day"] = value; unsaved.Add("schedule_times_of_day"); }
        }

        /// <summary>
        /// Time zone used by the Expectation schedule.
        /// </summary>
        [JsonPropertyName("schedule_time_zone")]
        public string ScheduleTimeZone
        {
            get { return (string)attributes["schedule_time_zone"]; }
            set { attributes["schedule_time_zone"] = value; unsaved.Add("schedule_time_zone"); }
        }

        /// <summary>
        /// Optional holiday region used by the Expectation schedule.
        /// </summary>
        [JsonPropertyName("holiday_region")]
        public string HolidayRegion
        {
            get { return (string)attributes["holiday_region"]; }
            set { attributes["holiday_region"] = value; unsaved.Add("holiday_region"); }
        }

        /// <summary>
        /// How many seconds before the due boundary the window starts.
        /// </summary>
        [JsonPropertyName("lookback_interval")]
        public Nullable<Int64> LookbackInterval
        {
            get { return (Nullable<Int64>)attributes["lookback_interval"]; }
            set { attributes["lookback_interval"] = value; unsaved.Add("lookback_interval"); }
        }

        /// <summary>
        /// How many seconds a schedule-driven window may remain eligible to close as late.
        /// </summary>
        [JsonPropertyName("late_acceptance_interval")]
        public Nullable<Int64> LateAcceptanceInterval
        {
            get { return (Nullable<Int64>)attributes["late_acceptance_interval"]; }
            set { attributes["late_acceptance_interval"] = value; unsaved.Add("late_acceptance_interval"); }
        }

        /// <summary>
        /// How many quiet seconds are required before final closure.
        /// </summary>
        [JsonPropertyName("inactivity_interval")]
        public Nullable<Int64> InactivityInterval
        {
            get { return (Nullable<Int64>)attributes["inactivity_interval"]; }
            set { attributes["inactivity_interval"] = value; unsaved.Add("inactivity_interval"); }
        }

        /// <summary>
        /// Hard-stop duration in seconds for unscheduled expectations.
        /// </summary>
        [JsonPropertyName("max_open_interval")]
        public Nullable<Int64> MaxOpenInterval
        {
            get { return (Nullable<Int64>)attributes["max_open_interval"]; }
            set { attributes["max_open_interval"] = value; unsaved.Add("max_open_interval"); }
        }

        /// <summary>
        /// Versioned success criteria definition for the expectation. Criteria v2 supports optional FTS content validation.
        /// </summary>
        [JsonPropertyName("criteria")]
        public object Criteria
        {
            get { return (object)attributes["criteria"]; }
            set { attributes["criteria"] = value; unsaved.Add("criteria"); }
        }

        /// <summary>
        /// Last time this expectation was evaluated.
        /// </summary>
        [JsonPropertyName("last_evaluated_at")]
        public Nullable<DateTime> LastEvaluatedAt
        {
            get { return (Nullable<DateTime>)attributes["last_evaluated_at"]; }
            set { attributes["last_evaluated_at"] = value; unsaved.Add("last_evaluated_at"); }
        }

        /// <summary>
        /// Last time this expectation closed successfully.
        /// </summary>
        [JsonPropertyName("last_success_at")]
        public Nullable<DateTime> LastSuccessAt
        {
            get { return (Nullable<DateTime>)attributes["last_success_at"]; }
            set { attributes["last_success_at"] = value; unsaved.Add("last_success_at"); }
        }

        /// <summary>
        /// Last time this expectation closed with a failure result.
        /// </summary>
        [JsonPropertyName("last_failure_at")]
        public Nullable<DateTime> LastFailureAt
        {
            get { return (Nullable<DateTime>)attributes["last_failure_at"]; }
            set { attributes["last_failure_at"] = value; unsaved.Add("last_failure_at"); }
        }

        /// <summary>
        /// Most recent terminal result for this expectation.
        /// </summary>
        [JsonPropertyName("last_result")]
        public string LastResult
        {
            get { return (string)attributes["last_result"]; }
            set { attributes["last_result"] = value; unsaved.Add("last_result"); }
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
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task<ExpectationEvaluation> TriggerEvaluation(Dictionary<string, object> parameters)
        {
            return TriggerEvaluationCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Manually open an Expectation window
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<ExpectationEvaluation> TriggerEvaluationAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return TriggerEvaluationCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ExpectationEvaluation> TriggerEvaluationCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Expectation has no id, which TriggerEvaluation needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/expectations/{System.Uri.EscapeDataString(attributes["id"].ToString())}/trigger_evaluation", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            ExpectationEvaluation result = JsonUtil.DeserializeModelResponse<ExpectationEvaluation>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
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
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the Expectation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the Expectation schedule.
        ///   schedule_time_zone - string - Time zone used by the Expectation schedule.
        ///   holiday_region - string - Optional holiday region used by the Expectation schedule.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Versioned success criteria definition for the expectation, including optional Files Transform Script content validation in criteria v2.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Expectation it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Expectation> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
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
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the Expectation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the Expectation schedule.
        ///   schedule_time_zone - string - Time zone used by the Expectation schedule.
        ///   holiday_region - string - Optional holiday region used by the Expectation schedule.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Versioned success criteria definition for the expectation, including optional Files Transform Script content validation in criteria v2.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Expectation it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Expectation> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Expectation> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Expectation has no id, which Update needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("source", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: source is null, which cannot be sent" : "Bad parameter: source must be of type string", "source");
            }
            if (parameters.TryGetValue("exclude_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_pattern is null, which cannot be sent" : "Bad parameter: exclude_pattern must be of type string", "exclude_pattern");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("recurring_day", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_day is null, which cannot be sent" : "Bad parameter: recurring_day must be of type Nullable<Int64>", "recurring_day");
            }
            if (parameters.TryGetValue("recurring_days", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_days is null, which cannot be sent" : "Bad parameter: recurring_days must be of type Nullable<Int64>[]", "recurring_days");
            }
            if (parameters.TryGetValue("schedule_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_id is null, which cannot be sent" : "Bad parameter: schedule_id must be of type Nullable<Int64>", "schedule_id");
            }
            if (parameters.TryGetValue("schedule_days_of_week", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_days_of_week is null, which cannot be sent" : "Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "schedule_days_of_week");
            }
            if (parameters.TryGetValue("schedule_times_of_day", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_times_of_day is null, which cannot be sent" : "Bad parameter: schedule_times_of_day must be of type string[]", "schedule_times_of_day");
            }
            if (parameters.TryGetValue("schedule_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_time_zone is null, which cannot be sent" : "Bad parameter: schedule_time_zone must be of type string", "schedule_time_zone");
            }
            if (parameters.TryGetValue("holiday_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: holiday_region is null, which cannot be sent" : "Bad parameter: holiday_region must be of type string", "holiday_region");
            }
            if (parameters.TryGetValue("lookback_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: lookback_interval is null, which cannot be sent" : "Bad parameter: lookback_interval must be of type Nullable<Int64>", "lookback_interval");
            }
            if (parameters.TryGetValue("late_acceptance_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: late_acceptance_interval is null, which cannot be sent" : "Bad parameter: late_acceptance_interval must be of type Nullable<Int64>", "late_acceptance_interval");
            }
            if (parameters.TryGetValue("inactivity_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: inactivity_interval is null, which cannot be sent" : "Bad parameter: inactivity_interval must be of type Nullable<Int64>", "inactivity_interval");
            }
            if (parameters.TryGetValue("max_open_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_open_interval is null, which cannot be sent" : "Bad parameter: max_open_interval must be of type Nullable<Int64>", "max_open_interval");
            }
            if (parameters.TryGetValue("criteria", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: criteria is null, which cannot be sent" : "Bad parameter: criteria must be of type object", "criteria");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/expectations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            Expectation updated = JsonUtil.DeserializeModelResponse<Expectation>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<Expectation>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /expectations/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /expectations/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>, but neither waits for the request nor reports its failure.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        [Obsolete("Destroy neither waits for the request nor reports its failure. Use DestroyAsync instead.")]
        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Expectation has no id, which Delete needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/expectations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "name", "description", "path", "source", "exclude_pattern", "disabled", "trigger", "interval", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region", "lookback_interval", "late_acceptance_interval", "inactivity_interval", "max_open_interval", "criteria", "workspace_id" };
        private static readonly string[] UpdateParameterNames = { "id", "name", "description", "path", "source", "exclude_pattern", "disabled", "trigger", "interval", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region", "lookback_interval", "late_acceptance_interval", "inactivity_interval", "max_open_interval", "criteria", "workspace_id" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Expectation on Files.com, or updates it if it has an ID.
        /// It sends the attributes set on this object, in its constructor or with its properties, or changed in place,
        /// such as an element of an array, since it was made or last came from Files.com, of those that operation
        /// takes; it sends no other attributes, and no defaults. Each goes in the form its parameter takes, such as a
        /// DateTime as ISO 8601 text. When it succeeds, this object takes the attributes Files.com returns.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                await UpdateCore(AttributesToSave(UpdateParameterNames), cancellationToken);
                return;
            }
            Expectation created = await Expectation.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
            Refresh(created.getAttributes());
            origin = ((IModel)created).Origin;
        }

        // The attributes set or changed on this object since it was made or last came from Files.com, of those named
        // in parameterNames: the parameters of the operation that saves it.
        private Dictionary<string, object> AttributesToSave(string[] parameterNames)
        {
            Dictionary<string, object> toSave = new Dictionary<string, object>();
            foreach (string name in parameterNames)
            {
                if (unsaved.Contains(name) || ChangedInPlace(name))
                {
                    toSave[name] = attributes[name];
                }
            }
            return toSave;
        }

        // An array attribute, for a property or getAttributes to hand out. Its JSON is kept the first time, as the caller
        // can change the array, or an array in a model in it, without setting anything.
        private object HandedOut(string name)
        {
            object value = attributes[name];
            if (value != null && !handedOut.ContainsKey(name))
            {
                handedOut[name] = JsonSerializer.Serialize(value, JsonUtil.Options);
            }
            return value;
        }

        // Whether an array handed out has been changed in place since, at any depth.
        private bool ChangedInPlace(string name)
        {
            return handedOut.TryGetValue(name, out string handedOutJson) && JsonSerializer.Serialize(attributes[name], JsonUtil.Options) != handedOutJson;
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
            handedOut.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Expectation> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Expectation> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Expectation> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }

            return new FilesList<Expectation>(client, $"/expectations", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation ID.
        /// </summary>
        /// <param name="id">Expectation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Expectation> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation ID.
        /// </summary>
        /// <param name="id">Expectation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Expectation> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<Expectation> FindCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Expectation>(responseJson, context, options);
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
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the Expectation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the Expectation schedule.
        ///   schedule_time_zone - string - Time zone used by the Expectation schedule.
        ///   holiday_region - string - Optional holiday region used by the Expectation schedule.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Versioned success criteria definition for the expectation, including optional Files Transform Script content validation in criteria v2.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Expectation> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Expectation> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("source", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: source is null, which cannot be sent" : "Bad parameter: source must be of type string", "source");
            }
            if (parameters.TryGetValue("exclude_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_pattern is null, which cannot be sent" : "Bad parameter: exclude_pattern must be of type string", "exclude_pattern");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("recurring_day", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_day is null, which cannot be sent" : "Bad parameter: recurring_day must be of type Nullable<Int64>", "recurring_day");
            }
            if (parameters.TryGetValue("recurring_days", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_days is null, which cannot be sent" : "Bad parameter: recurring_days must be of type Nullable<Int64>[]", "recurring_days");
            }
            if (parameters.TryGetValue("schedule_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_id is null, which cannot be sent" : "Bad parameter: schedule_id must be of type Nullable<Int64>", "schedule_id");
            }
            if (parameters.TryGetValue("schedule_days_of_week", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_days_of_week is null, which cannot be sent" : "Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "schedule_days_of_week");
            }
            if (parameters.TryGetValue("schedule_times_of_day", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_times_of_day is null, which cannot be sent" : "Bad parameter: schedule_times_of_day must be of type string[]", "schedule_times_of_day");
            }
            if (parameters.TryGetValue("schedule_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_time_zone is null, which cannot be sent" : "Bad parameter: schedule_time_zone must be of type string", "schedule_time_zone");
            }
            if (parameters.TryGetValue("holiday_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: holiday_region is null, which cannot be sent" : "Bad parameter: holiday_region must be of type string", "holiday_region");
            }
            if (parameters.TryGetValue("lookback_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: lookback_interval is null, which cannot be sent" : "Bad parameter: lookback_interval must be of type Nullable<Int64>", "lookback_interval");
            }
            if (parameters.TryGetValue("late_acceptance_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: late_acceptance_interval is null, which cannot be sent" : "Bad parameter: late_acceptance_interval must be of type Nullable<Int64>", "late_acceptance_interval");
            }
            if (parameters.TryGetValue("inactivity_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: inactivity_interval is null, which cannot be sent" : "Bad parameter: inactivity_interval must be of type Nullable<Int64>", "inactivity_interval");
            }
            if (parameters.TryGetValue("max_open_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_open_interval is null, which cannot be sent" : "Bad parameter: max_open_interval must be of type Nullable<Int64>", "max_open_interval");
            }
            if (parameters.TryGetValue("criteria", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: criteria is null, which cannot be sent" : "Bad parameter: criteria must be of type object", "criteria");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectations", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Expectation>(responseJson, context, options);
        }

        /// <summary>
        /// Manually open an Expectation window
        /// </summary>
        /// <param name="id">Expectation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ExpectationEvaluation> TriggerEvaluation(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return TriggerEvaluationCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<ExpectationEvaluation> TriggerEvaluationCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectations/{System.Uri.EscapeDataString(parameters["id"].ToString())}/trigger_evaluation", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ExpectationEvaluation>(responseJson, context, options);
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
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the Expectation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the Expectation schedule.
        ///   schedule_time_zone - string - Time zone used by the Expectation schedule.
        ///   holiday_region - string - Optional holiday region used by the Expectation schedule.
        ///   lookback_interval - int64 - How many seconds before the due boundary the window starts.
        ///   late_acceptance_interval - int64 - How many seconds a schedule-driven window may remain eligible to close as late.
        ///   inactivity_interval - int64 - How many quiet seconds are required before final closure.
        ///   max_open_interval - int64 - Hard-stop duration in seconds for unscheduled expectations.
        ///   criteria - object - Versioned success criteria definition for the expectation, including optional Files Transform Script content validation in criteria v2.
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="id">Expectation ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Expectation> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<Expectation> UpdateCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("source", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: source is null, which cannot be sent" : "Bad parameter: source must be of type string", "source");
            }
            if (parameters.TryGetValue("exclude_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_pattern is null, which cannot be sent" : "Bad parameter: exclude_pattern must be of type string", "exclude_pattern");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("recurring_day", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_day is null, which cannot be sent" : "Bad parameter: recurring_day must be of type Nullable<Int64>", "recurring_day");
            }
            if (parameters.TryGetValue("recurring_days", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_days is null, which cannot be sent" : "Bad parameter: recurring_days must be of type Nullable<Int64>[]", "recurring_days");
            }
            if (parameters.TryGetValue("schedule_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_id is null, which cannot be sent" : "Bad parameter: schedule_id must be of type Nullable<Int64>", "schedule_id");
            }
            if (parameters.TryGetValue("schedule_days_of_week", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_days_of_week is null, which cannot be sent" : "Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "schedule_days_of_week");
            }
            if (parameters.TryGetValue("schedule_times_of_day", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_times_of_day is null, which cannot be sent" : "Bad parameter: schedule_times_of_day must be of type string[]", "schedule_times_of_day");
            }
            if (parameters.TryGetValue("schedule_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_time_zone is null, which cannot be sent" : "Bad parameter: schedule_time_zone must be of type string", "schedule_time_zone");
            }
            if (parameters.TryGetValue("holiday_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: holiday_region is null, which cannot be sent" : "Bad parameter: holiday_region must be of type string", "holiday_region");
            }
            if (parameters.TryGetValue("lookback_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: lookback_interval is null, which cannot be sent" : "Bad parameter: lookback_interval must be of type Nullable<Int64>", "lookback_interval");
            }
            if (parameters.TryGetValue("late_acceptance_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: late_acceptance_interval is null, which cannot be sent" : "Bad parameter: late_acceptance_interval must be of type Nullable<Int64>", "late_acceptance_interval");
            }
            if (parameters.TryGetValue("inactivity_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: inactivity_interval is null, which cannot be sent" : "Bad parameter: inactivity_interval must be of type Nullable<Int64>", "inactivity_interval");
            }
            if (parameters.TryGetValue("max_open_interval", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: max_open_interval is null, which cannot be sent" : "Bad parameter: max_open_interval must be of type Nullable<Int64>", "max_open_interval");
            }
            if (parameters.TryGetValue("criteria", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: criteria is null, which cannot be sent" : "Bad parameter: criteria must be of type object", "criteria");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Expectation>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /expectations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Expectation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /expectations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Expectation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(id, parameters, options);
        }

        internal static async Task DeleteCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/expectations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}