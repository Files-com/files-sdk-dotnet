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
    /// ScheduledExportEntity model
    /// </summary>
    public class ScheduledExport : IModel, IJsonOnDeserialized
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
        /// Makes a ScheduledExport with no attributes set and no options.
        /// </summary>
        public ScheduledExport() : this(null, null) { }

        /// <summary>
        /// Makes a ScheduledExport with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public ScheduledExport(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("export_type"))
            {
                this.attributes.Add("export_type", null);
            }
            if (!this.attributes.ContainsKey("report_name"))
            {
                this.attributes.Add("report_name", null);
            }
            if (!this.attributes.ContainsKey("export_options"))
            {
                this.attributes.Add("export_options", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("disabled"))
            {
                this.attributes.Add("disabled", false);
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
            if (!this.attributes.ContainsKey("human_readable_schedule"))
            {
                this.attributes.Add("human_readable_schedule", null);
            }
            if (!this.attributes.ContainsKey("last_run_at"))
            {
                this.attributes.Add("last_run_at", null);
            }
            if (!this.attributes.ContainsKey("last_export_id"))
            {
                this.attributes.Add("last_export_id", null);
            }
            if (!this.attributes.ContainsKey("last_error"))
            {
                this.attributes.Add("last_error", null);
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
        /// Scheduled Export ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Name for this scheduled export.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
        /// </summary>
        [JsonPropertyName("export_type")]
        public string ExportType
        {
            get { return (string)attributes["export_type"]; }
            set { attributes["export_type"] = value; unsaved.Add("export_type"); }
        }

        /// <summary>
        /// Human-readable report name.
        /// </summary>
        [JsonPropertyName("report_name")]
        public string ReportName
        {
            get { return (string)attributes["report_name"]; }
            set { attributes["report_name"] = value; unsaved.Add("report_name"); }
        }

        /// <summary>
        /// Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
        /// </summary>
        [JsonPropertyName("export_options")]
        public object ExportOptions
        {
            get { return (object)attributes["export_options"]; }
            set { attributes["export_options"] = value; unsaved.Add("export_options"); }
        }

        /// <summary>
        /// Site Admin user who receives the completed export e-mail.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; unsaved.Add("user_id"); }
        }

        /// <summary>
        /// If true, this scheduled export will not run.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; unsaved.Add("disabled"); }
        }

        /// <summary>
        /// Schedule trigger type: `daily` or `custom_schedule`.
        /// </summary>
        [JsonPropertyName("trigger")]
        public string Trigger
        {
            get { return (string)attributes["trigger"]; }
            set { attributes["trigger"] = value; unsaved.Add("trigger"); }
        }

        /// <summary>
        /// If trigger is `daily`, this specifies how often to run the scheduled export.
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
        /// If trigger is `custom_schedule`, the reusable Schedule used instead of the scheduled export's schedule fields.
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
        /// Times of day in HH:MM format for the scheduled export schedule.
        /// </summary>
        [JsonPropertyName("schedule_times_of_day")]
        public string[] ScheduleTimesOfDay
        {
            get { return (string[])HandedOut("schedule_times_of_day"); }
            set { attributes["schedule_times_of_day"] = value; unsaved.Add("schedule_times_of_day"); }
        }

        /// <summary>
        /// Time zone used by the scheduled export schedule.
        /// </summary>
        [JsonPropertyName("schedule_time_zone")]
        public string ScheduleTimeZone
        {
            get { return (string)attributes["schedule_time_zone"]; }
            set { attributes["schedule_time_zone"] = value; unsaved.Add("schedule_time_zone"); }
        }

        /// <summary>
        /// Optional holiday region used by the scheduled export schedule.
        /// </summary>
        [JsonPropertyName("holiday_region")]
        public string HolidayRegion
        {
            get { return (string)attributes["holiday_region"]; }
            set { attributes["holiday_region"] = value; unsaved.Add("holiday_region"); }
        }

        /// <summary>
        /// Human-readable schedule description.
        /// </summary>
        [JsonPropertyName("human_readable_schedule")]
        public string HumanReadableSchedule
        {
            get { return (string)attributes["human_readable_schedule"]; }
            set { attributes["human_readable_schedule"] = value; unsaved.Add("human_readable_schedule"); }
        }

        /// <summary>
        /// Most recent scheduled attempt time, including attempts that failed validation.
        /// </summary>
        [JsonPropertyName("last_run_at")]
        public Nullable<DateTime> LastRunAt
        {
            get { return (Nullable<DateTime>)attributes["last_run_at"]; }
            set { attributes["last_run_at"] = value; unsaved.Add("last_run_at"); }
        }

        /// <summary>
        /// Most recent Export ID created by this schedule.
        /// </summary>
        [JsonPropertyName("last_export_id")]
        public Nullable<Int64> LastExportId
        {
            get { return (Nullable<Int64>)attributes["last_export_id"]; }
            set { attributes["last_export_id"] = value; unsaved.Add("last_export_id"); }
        }

        /// <summary>
        /// Validation error from the most recent scheduled attempt. The schedule remains enabled and retries at its next scheduled time. Cleared when an export is successfully created; does not describe errors during export generation.
        /// </summary>
        [JsonPropertyName("last_error")]
        public string LastError
        {
            get { return (string)attributes["last_error"]; }
            set { attributes["last_error"] = value; unsaved.Add("last_error"); }
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
        /// Parameters:
        ///   name - string - Name for this scheduled export.
        ///   export_type - string - Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
        ///   export_options - object - Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
        ///   user_id - int64 - Site Admin user who receives the completed export e-mail.
        ///   disabled - boolean - If true, this scheduled export will not run.
        ///   trigger - string - Schedule trigger type: `daily` or `custom_schedule`.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the scheduled export.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the scheduled export's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the scheduled export schedule.
        ///   schedule_time_zone - string - Time zone used by the scheduled export schedule.
        ///   holiday_region - string - Optional holiday region used by the scheduled export schedule.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The ScheduledExport it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<ScheduledExport> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this scheduled export.
        ///   export_type - string - Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
        ///   export_options - object - Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
        ///   user_id - int64 - Site Admin user who receives the completed export e-mail.
        ///   disabled - boolean - If true, this scheduled export will not run.
        ///   trigger - string - Schedule trigger type: `daily` or `custom_schedule`.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the scheduled export.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the scheduled export's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the scheduled export schedule.
        ///   schedule_time_zone - string - Time zone used by the scheduled export schedule.
        ///   holiday_region - string - Optional holiday region used by the scheduled export schedule.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The ScheduledExport it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<ScheduledExport> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<ScheduledExport> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The ScheduledExport has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("export_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: export_type is null, which cannot be sent" : "Bad parameter: export_type must be of type string", "export_type");
            }
            if (parameters.TryGetValue("export_options", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: export_options is null, which cannot be sent" : "Bad parameter: export_options must be of type object", "export_options");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
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

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/scheduled_exports/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            ScheduledExport updated = JsonUtil.DeserializeModelResponse<ScheduledExport>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<ScheduledExport>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /scheduled_exports/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /scheduled_exports/{id} to the Files.com API.
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
                throw new MissingParameterException("The ScheduledExport has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/scheduled_exports/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "name", "export_type", "export_options", "user_id", "disabled", "trigger", "interval", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region" };
        private static readonly string[] UpdateParameterNames = { "id", "name", "export_type", "export_options", "user_id", "disabled", "trigger", "interval", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this ScheduledExport on Files.com, or updates it if it has an ID.
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
            ScheduledExport created = await ScheduledExport.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `export_type` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `export_type`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `export_type`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<ScheduledExport> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `export_type` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `export_type`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `export_type`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<ScheduledExport> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<ScheduledExport> ListCore(
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
            if (parameters.TryGetValue("filter_prefix", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_prefix is null, which cannot be sent" : "Bad parameter: filter_prefix must be of type object", "filter_prefix");
            }

            return new FilesList<ScheduledExport>(client, $"/scheduled_exports", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Scheduled Export ID.
        /// </summary>
        /// <param name="id">Scheduled Export ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ScheduledExport> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Scheduled Export ID.
        /// </summary>
        /// <param name="id">Scheduled Export ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ScheduledExport> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<ScheduledExport> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/scheduled_exports/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ScheduledExport>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name (required) - string - Name for this scheduled export.
        ///   export_type (required) - string - Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
        ///   export_options - object - Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
        ///   user_id - int64 - Site Admin user who receives the completed export e-mail.
        ///   disabled - boolean - If true, this scheduled export will not run.
        ///   trigger - string - Schedule trigger type: `daily` or `custom_schedule`.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the scheduled export.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the scheduled export's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the scheduled export schedule.
        ///   schedule_time_zone - string - Time zone used by the scheduled export schedule.
        ///   holiday_region - string - Optional holiday region used by the scheduled export schedule.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ScheduledExport> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<ScheduledExport> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("name", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: name", "name");
            }
            if (!parameters.TryGetValue("export_type", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: export_type", "export_type");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("export_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: export_type is null, which cannot be sent" : "Bad parameter: export_type must be of type string", "export_type");
            }
            if (parameters.TryGetValue("export_options", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: export_options is null, which cannot be sent" : "Bad parameter: export_options must be of type object", "export_options");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/scheduled_exports", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ScheduledExport>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this scheduled export.
        ///   export_type - string - Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
        ///   export_options - object - Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
        ///   user_id - int64 - Site Admin user who receives the completed export e-mail.
        ///   disabled - boolean - If true, this scheduled export will not run.
        ///   trigger - string - Schedule trigger type: `daily` or `custom_schedule`.
        ///   interval - string - If trigger is `daily`, this specifies how often to run the scheduled export.
        ///   recurring_day - int64 - If trigger is `daily`, this selects the day number inside the chosen interval.
        ///   recurring_days - array(int64) - If trigger is `daily`, this selects one or more day numbers inside a `week`, `month`, `quarter`, or `year` interval.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the scheduled export's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format for the scheduled export schedule.
        ///   schedule_time_zone - string - Time zone used by the scheduled export schedule.
        ///   holiday_region - string - Optional holiday region used by the scheduled export schedule.
        /// </summary>
        /// <param name="id">Scheduled Export ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ScheduledExport> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<ScheduledExport> UpdateCore(
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
            if (parameters.TryGetValue("export_type", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: export_type is null, which cannot be sent" : "Bad parameter: export_type must be of type string", "export_type");
            }
            if (parameters.TryGetValue("export_options", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: export_options is null, which cannot be sent" : "Bad parameter: export_options must be of type object", "export_options");
            }
            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/scheduled_exports/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ScheduledExport>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /scheduled_exports/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Scheduled Export ID.</param>
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
        /// Sends DELETE /scheduled_exports/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Scheduled Export ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/scheduled_exports/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}