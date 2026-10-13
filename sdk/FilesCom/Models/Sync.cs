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
    /// SyncEntity model
    /// </summary>
    public class Sync : IModel, IJsonOnDeserialized
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
        private static readonly string[] ArrayAttributeNames = { "include_patterns", "exclude_patterns", "recurring_days", "schedule_days_of_week", "schedule_times_of_day" };
        /// <summary>
        /// Makes a Sync with no attributes set and no options.
        /// </summary>
        public Sync() : this(null, null) { }

        /// <summary>
        /// Makes a Sync with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Sync(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("site_id"))
            {
                this.attributes.Add("site_id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("src_path"))
            {
                this.attributes.Add("src_path", null);
            }
            if (!this.attributes.ContainsKey("dest_path"))
            {
                this.attributes.Add("dest_path", null);
            }
            if (!this.attributes.ContainsKey("src_remote_server_id"))
            {
                this.attributes.Add("src_remote_server_id", null);
            }
            if (!this.attributes.ContainsKey("dest_remote_server_id"))
            {
                this.attributes.Add("dest_remote_server_id", null);
            }
            if (!this.attributes.ContainsKey("src_site_id"))
            {
                this.attributes.Add("src_site_id", null);
            }
            if (!this.attributes.ContainsKey("dest_site_id"))
            {
                this.attributes.Add("dest_site_id", null);
            }
            if (!this.attributes.ContainsKey("two_way"))
            {
                this.attributes.Add("two_way", false);
            }
            if (!this.attributes.ContainsKey("keep_after_copy"))
            {
                this.attributes.Add("keep_after_copy", false);
            }
            if (!this.attributes.ContainsKey("delete_empty_folders"))
            {
                this.attributes.Add("delete_empty_folders", false);
            }
            if (!this.attributes.ContainsKey("disabled"))
            {
                this.attributes.Add("disabled", false);
            }
            if (!this.attributes.ContainsKey("trigger"))
            {
                this.attributes.Add("trigger", null);
            }
            if (!this.attributes.ContainsKey("trigger_file"))
            {
                this.attributes.Add("trigger_file", null);
            }
            if (!this.attributes.ContainsKey("always_write_trigger_file"))
            {
                this.attributes.Add("always_write_trigger_file", false);
            }
            if (!this.attributes.ContainsKey("include_patterns"))
            {
                this.attributes.Add("include_patterns", new string[0]);
            }
            if (!this.attributes.ContainsKey("exclude_patterns"))
            {
                this.attributes.Add("exclude_patterns", new string[0]);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
            }
            if (!this.attributes.ContainsKey("sync_interval_minutes"))
            {
                this.attributes.Add("sync_interval_minutes", null);
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
            if (!this.attributes.ContainsKey("latest_sync_run"))
            {
                this.attributes.Add("latest_sync_run", null);
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
            get { return new object[] { LatestSyncRun }; }
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
        /// Sync ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Name for this sync job
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Description for this sync job
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; unsaved.Add("description"); }
        }

        /// <summary>
        /// Site ID this sync belongs to
        /// </summary>
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            set { attributes["site_id"] = value; unsaved.Add("site_id"); }
        }

        /// <summary>
        /// Workspace ID this sync belongs to
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// User who created or owns this sync
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; unsaved.Add("user_id"); }
        }

        /// <summary>
        /// Absolute source path for the sync
        /// </summary>
        [JsonPropertyName("src_path")]
        public string SrcPath
        {
            get { return (string)attributes["src_path"]; }
            set { attributes["src_path"] = value; unsaved.Add("src_path"); }
        }

        /// <summary>
        /// Absolute destination path for the sync
        /// </summary>
        [JsonPropertyName("dest_path")]
        public string DestPath
        {
            get { return (string)attributes["dest_path"]; }
            set { attributes["dest_path"] = value; unsaved.Add("dest_path"); }
        }

        /// <summary>
        /// Remote server ID for the source (if remote)
        /// </summary>
        [JsonPropertyName("src_remote_server_id")]
        public Nullable<Int64> SrcRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["src_remote_server_id"]; }
            set { attributes["src_remote_server_id"] = value; unsaved.Add("src_remote_server_id"); }
        }

        /// <summary>
        /// Remote server ID for the destination (if remote)
        /// </summary>
        [JsonPropertyName("dest_remote_server_id")]
        public Nullable<Int64> DestRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["dest_remote_server_id"]; }
            set { attributes["dest_remote_server_id"] = value; unsaved.Add("dest_remote_server_id"); }
        }

        /// <summary>
        /// Source site ID if syncing from a child or partner site
        /// </summary>
        [JsonPropertyName("src_site_id")]
        public Nullable<Int64> SrcSiteId
        {
            get { return (Nullable<Int64>)attributes["src_site_id"]; }
            set { attributes["src_site_id"] = value; unsaved.Add("src_site_id"); }
        }

        /// <summary>
        /// Destination site ID if syncing to a child or partner site
        /// </summary>
        [JsonPropertyName("dest_site_id")]
        public Nullable<Int64> DestSiteId
        {
            get { return (Nullable<Int64>)attributes["dest_site_id"]; }
            set { attributes["dest_site_id"] = value; unsaved.Add("dest_site_id"); }
        }

        /// <summary>
        /// Is this a two-way sync?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("two_way")]
        public bool TwoWay
        {
            get { return attributes["two_way"] == null ? false : (bool)attributes["two_way"]; }
            set { attributes["two_way"] = value; unsaved.Add("two_way"); }
        }

        /// <summary>
        /// Keep files after copying?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("keep_after_copy")]
        public bool KeepAfterCopy
        {
            get { return attributes["keep_after_copy"] == null ? false : (bool)attributes["keep_after_copy"]; }
            set { attributes["keep_after_copy"] = value; unsaved.Add("keep_after_copy"); }
        }

        /// <summary>
        /// Delete empty folders after sync?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("delete_empty_folders")]
        public bool DeleteEmptyFolders
        {
            get { return attributes["delete_empty_folders"] == null ? false : (bool)attributes["delete_empty_folders"]; }
            set { attributes["delete_empty_folders"] = value; unsaved.Add("delete_empty_folders"); }
        }

        /// <summary>
        /// Is this sync disabled?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; unsaved.Add("disabled"); }
        }

        /// <summary>
        /// Trigger type: daily, custom_schedule, or manual
        /// </summary>
        [JsonPropertyName("trigger")]
        public string Trigger
        {
            get { return (string)attributes["trigger"]; }
            set { attributes["trigger"] = value; unsaved.Add("trigger"); }
        }

        /// <summary>
        /// Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        /// </summary>
        [JsonPropertyName("trigger_file")]
        public string TriggerFile
        {
            get { return (string)attributes["trigger_file"]; }
            set { attributes["trigger_file"] = value; unsaved.Add("trigger_file"); }
        }

        /// <summary>
        /// If true, the trigger file will be sent at the end of a successful sync even when no files were transferred.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("always_write_trigger_file")]
        public bool AlwaysWriteTriggerFile
        {
            get { return attributes["always_write_trigger_file"] == null ? false : (bool)attributes["always_write_trigger_file"]; }
            set { attributes["always_write_trigger_file"] = value; unsaved.Add("always_write_trigger_file"); }
        }

        /// <summary>
        /// Array of glob patterns to include
        /// </summary>
        [JsonPropertyName("include_patterns")]
        public string[] IncludePatterns
        {
            get { return (string[])HandedOut("include_patterns"); }
            set { attributes["include_patterns"] = value; unsaved.Add("include_patterns"); }
        }

        /// <summary>
        /// Array of glob patterns to exclude
        /// </summary>
        [JsonPropertyName("exclude_patterns")]
        public string[] ExcludePatterns
        {
            get { return (string[])HandedOut("exclude_patterns"); }
            set { attributes["exclude_patterns"] = value; unsaved.Add("exclude_patterns"); }
        }

        /// <summary>
        /// When this sync was created
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// When this sync was last updated
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>)attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }

        /// <summary>
        /// Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        /// </summary>
        [JsonPropertyName("sync_interval_minutes")]
        public Nullable<Int64> SyncIntervalMinutes
        {
            get { return (Nullable<Int64>)attributes["sync_interval_minutes"]; }
            set { attributes["sync_interval_minutes"] = value; unsaved.Add("sync_interval_minutes"); }
        }

        /// <summary>
        /// If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval
        {
            get { return (string)attributes["interval"]; }
            set { attributes["interval"] = value; unsaved.Add("interval"); }
        }

        /// <summary>
        /// If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        /// </summary>
        [JsonPropertyName("recurring_day")]
        public Nullable<Int64> RecurringDay
        {
            get { return (Nullable<Int64>)attributes["recurring_day"]; }
            set { attributes["recurring_day"] = value; unsaved.Add("recurring_day"); }
        }

        /// <summary>
        /// If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        /// </summary>
        [JsonPropertyName("recurring_days")]
        public Nullable<Int64>[] RecurringDays
        {
            get { return (Nullable<Int64>[])HandedOut("recurring_days"); }
            set { attributes["recurring_days"] = value; unsaved.Add("recurring_days"); }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, the reusable Schedule used instead of the sync's schedule fields.
        /// </summary>
        [JsonPropertyName("schedule_id")]
        public Nullable<Int64> ScheduleId
        {
            get { return (Nullable<Int64>)attributes["schedule_id"]; }
            set { attributes["schedule_id"] = value; unsaved.Add("schedule_id"); }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        /// </summary>
        [JsonPropertyName("schedule_days_of_week")]
        public Nullable<Int64>[] ScheduleDaysOfWeek
        {
            get { return (Nullable<Int64>[])HandedOut("schedule_days_of_week"); }
            set { attributes["schedule_days_of_week"] = value; unsaved.Add("schedule_days_of_week"); }
        }

        /// <summary>
        /// Times of day to run in HH:MM format. For `custom_schedule`, run at these times on specified days of week. For `daily`, run at these times on the scheduled interval date.
        /// </summary>
        [JsonPropertyName("schedule_times_of_day")]
        public string[] ScheduleTimesOfDay
        {
            get { return (string[])HandedOut("schedule_times_of_day"); }
            set { attributes["schedule_times_of_day"] = value; unsaved.Add("schedule_times_of_day"); }
        }

        /// <summary>
        /// Time zone for the schedule. If not set, times are interpreted as UTC.
        /// </summary>
        [JsonPropertyName("schedule_time_zone")]
        public string ScheduleTimeZone
        {
            get { return (string)attributes["schedule_time_zone"]; }
            set { attributes["schedule_time_zone"] = value; unsaved.Add("schedule_time_zone"); }
        }

        /// <summary>
        /// Skip the sync if there is a formal, observed holiday for this region.
        /// </summary>
        [JsonPropertyName("holiday_region")]
        public string HolidayRegion
        {
            get { return (string)attributes["holiday_region"]; }
            set { attributes["holiday_region"] = value; unsaved.Add("holiday_region"); }
        }

        /// <summary>
        /// The latest run of this sync
        /// </summary>
        [JsonPropertyName("latest_sync_run")]
        public SyncRun LatestSyncRun
        {
            get { return (SyncRun)attributes["latest_sync_run"]; }
            set { attributes["latest_sync_run"] = value; unsaved.Add("latest_sync_run"); }
        }

        /// <summary>
        /// Dry Run Sync
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task DryRun(Dictionary<string, object> parameters)
        {
            return DryRunCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Dry Run Sync
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DryRunAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DryRunCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task DryRunCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Sync has no id, which DryRun needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/syncs/{System.Uri.EscapeDataString(attributes["id"].ToString())}/dry_run", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Manually Run Sync
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task ManualRun(Dictionary<string, object> parameters)
        {
            return ManualRunCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Manually Run Sync
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task ManualRunAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return ManualRunCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task ManualRunCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Sync has no id, which ManualRun needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/syncs/{System.Uri.EscapeDataString(attributes["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Parameters:
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   description - string - Description for this sync job
        ///   dest_path - string - Absolute destination path for the sync
        ///   dest_remote_server_id - int64 - Remote server ID for the destination (if remote)
        ///   disabled - boolean - Is this sync disabled?
        ///   exclude_patterns - array(string) - Array of glob patterns to exclude
        ///   holiday_region - string - Skip the sync if there is a formal, observed holiday for this region.
        ///   include_patterns - array(string) - Array of glob patterns to include
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   name - string - Name for this sync job
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the sync's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_time_zone - string - Time zone for the schedule. If not set, times are interpreted as UTC.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format. For `custom_schedule`, run at these times on specified days of week. For `daily`, run at these times on the scheduled interval date.
        ///   src_path - string - Absolute source path for the sync
        ///   src_remote_server_id - int64 - Remote server ID for the source (if remote)
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   always_write_trigger_file - boolean - If true, the trigger file will be sent at the end of a successful sync even when no files were transferred.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Sync it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Sync> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   description - string - Description for this sync job
        ///   dest_path - string - Absolute destination path for the sync
        ///   dest_remote_server_id - int64 - Remote server ID for the destination (if remote)
        ///   disabled - boolean - Is this sync disabled?
        ///   exclude_patterns - array(string) - Array of glob patterns to exclude
        ///   holiday_region - string - Skip the sync if there is a formal, observed holiday for this region.
        ///   include_patterns - array(string) - Array of glob patterns to include
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   name - string - Name for this sync job
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the sync's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_time_zone - string - Time zone for the schedule. If not set, times are interpreted as UTC.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format. For `custom_schedule`, run at these times on specified days of week. For `daily`, run at these times on the scheduled interval date.
        ///   src_path - string - Absolute source path for the sync
        ///   src_remote_server_id - int64 - Remote server ID for the source (if remote)
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   always_write_trigger_file - boolean - If true, the trigger file will be sent at the end of a successful sync even when no files were transferred.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Sync it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Sync> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Sync> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Sync has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("delete_empty_folders", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: delete_empty_folders is null, which cannot be sent" : "Bad parameter: delete_empty_folders must be of type bool", "delete_empty_folders");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("dest_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dest_path is null, which cannot be sent" : "Bad parameter: dest_path must be of type string", "dest_path");
            }
            if (parameters.TryGetValue("dest_remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dest_remote_server_id is null, which cannot be sent" : "Bad parameter: dest_remote_server_id must be of type Nullable<Int64>", "dest_remote_server_id");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("exclude_patterns", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_patterns is null, which cannot be sent" : "Bad parameter: exclude_patterns must be of type string[]", "exclude_patterns");
            }
            if (parameters.TryGetValue("holiday_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: holiday_region is null, which cannot be sent" : "Bad parameter: holiday_region must be of type string", "holiday_region");
            }
            if (parameters.TryGetValue("include_patterns", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: include_patterns is null, which cannot be sent" : "Bad parameter: include_patterns must be of type string[]", "include_patterns");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("keep_after_copy", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: keep_after_copy is null, which cannot be sent" : "Bad parameter: keep_after_copy must be of type bool", "keep_after_copy");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
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
            if (parameters.TryGetValue("schedule_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_time_zone is null, which cannot be sent" : "Bad parameter: schedule_time_zone must be of type string", "schedule_time_zone");
            }
            if (parameters.TryGetValue("schedule_times_of_day", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_times_of_day is null, which cannot be sent" : "Bad parameter: schedule_times_of_day must be of type string[]", "schedule_times_of_day");
            }
            if (parameters.TryGetValue("src_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: src_path is null, which cannot be sent" : "Bad parameter: src_path must be of type string", "src_path");
            }
            if (parameters.TryGetValue("src_remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: src_remote_server_id is null, which cannot be sent" : "Bad parameter: src_remote_server_id must be of type Nullable<Int64>", "src_remote_server_id");
            }
            if (parameters.TryGetValue("sync_interval_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_interval_minutes is null, which cannot be sent" : "Bad parameter: sync_interval_minutes must be of type Nullable<Int64>", "sync_interval_minutes");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("trigger_file", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_file is null, which cannot be sent" : "Bad parameter: trigger_file must be of type string", "trigger_file");
            }
            if (parameters.TryGetValue("always_write_trigger_file", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_write_trigger_file is null, which cannot be sent" : "Bad parameter: always_write_trigger_file must be of type bool", "always_write_trigger_file");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/syncs/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            Sync updated = JsonUtil.DeserializeModelResponse<Sync>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<Sync>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /syncs/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /syncs/{id} to the Files.com API.
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
                throw new MissingParameterException("The Sync has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/syncs/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "delete_empty_folders", "description", "dest_path", "dest_remote_server_id", "disabled", "exclude_patterns", "holiday_region", "include_patterns", "interval", "keep_after_copy", "name", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_time_zone", "schedule_times_of_day", "src_path", "src_remote_server_id", "sync_interval_minutes", "trigger", "trigger_file", "always_write_trigger_file", "workspace_id" };
        private static readonly string[] UpdateParameterNames = { "id", "delete_empty_folders", "description", "dest_path", "dest_remote_server_id", "disabled", "exclude_patterns", "holiday_region", "include_patterns", "interval", "keep_after_copy", "name", "recurring_day", "recurring_days", "schedule_id", "schedule_days_of_week", "schedule_time_zone", "schedule_times_of_day", "src_path", "src_remote_server_id", "sync_interval_minutes", "trigger", "trigger_file", "always_write_trigger_file" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Sync on Files.com, or updates it if it has an ID.
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
            Sync created = await Sync.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `disabled`, `src_remote_server_id` or `dest_remote_server_id`. Valid field combinations are `[ workspace_id, disabled ]`, `[ workspace_id, src_remote_server_id ]`, `[ workspace_id, dest_remote_server_id ]`, `[ disabled, src_remote_server_id ]`, `[ disabled, dest_remote_server_id ]`, `[ workspace_id, disabled, src_remote_server_id ]` or `[ workspace_id, disabled, dest_remote_server_id ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Sync> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`, `workspace_id` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`, `disabled`, `src_remote_server_id` or `dest_remote_server_id`. Valid field combinations are `[ workspace_id, disabled ]`, `[ workspace_id, src_remote_server_id ]`, `[ workspace_id, dest_remote_server_id ]`, `[ disabled, src_remote_server_id ]`, `[ disabled, dest_remote_server_id ]`, `[ workspace_id, disabled, src_remote_server_id ]` or `[ workspace_id, disabled, dest_remote_server_id ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Sync> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Sync> ListCore(
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

            return new FilesList<Sync>(client, $"/syncs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sync ID.
        /// </summary>
        /// <param name="id">Sync ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Sync> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sync ID.
        /// </summary>
        /// <param name="id">Sync ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Sync> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<Sync> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Sync>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   description - string - Description for this sync job
        ///   dest_path - string - Absolute destination path for the sync
        ///   dest_remote_server_id - int64 - Remote server ID for the destination (if remote)
        ///   disabled - boolean - Is this sync disabled?
        ///   exclude_patterns - array(string) - Array of glob patterns to exclude
        ///   holiday_region - string - Skip the sync if there is a formal, observed holiday for this region.
        ///   include_patterns - array(string) - Array of glob patterns to include
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   name - string - Name for this sync job
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the sync's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_time_zone - string - Time zone for the schedule. If not set, times are interpreted as UTC.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format. For `custom_schedule`, run at these times on specified days of week. For `daily`, run at these times on the scheduled interval date.
        ///   src_path - string - Absolute source path for the sync
        ///   src_remote_server_id - int64 - Remote server ID for the source (if remote)
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   always_write_trigger_file - boolean - If true, the trigger file will be sent at the end of a successful sync even when no files were transferred.
        ///   workspace_id - int64 - Workspace ID this sync belongs to
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Sync> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Sync> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("delete_empty_folders", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: delete_empty_folders is null, which cannot be sent" : "Bad parameter: delete_empty_folders must be of type bool", "delete_empty_folders");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("dest_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dest_path is null, which cannot be sent" : "Bad parameter: dest_path must be of type string", "dest_path");
            }
            if (parameters.TryGetValue("dest_remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dest_remote_server_id is null, which cannot be sent" : "Bad parameter: dest_remote_server_id must be of type Nullable<Int64>", "dest_remote_server_id");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("exclude_patterns", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_patterns is null, which cannot be sent" : "Bad parameter: exclude_patterns must be of type string[]", "exclude_patterns");
            }
            if (parameters.TryGetValue("holiday_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: holiday_region is null, which cannot be sent" : "Bad parameter: holiday_region must be of type string", "holiday_region");
            }
            if (parameters.TryGetValue("include_patterns", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: include_patterns is null, which cannot be sent" : "Bad parameter: include_patterns must be of type string[]", "include_patterns");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("keep_after_copy", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: keep_after_copy is null, which cannot be sent" : "Bad parameter: keep_after_copy must be of type bool", "keep_after_copy");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
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
            if (parameters.TryGetValue("schedule_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_time_zone is null, which cannot be sent" : "Bad parameter: schedule_time_zone must be of type string", "schedule_time_zone");
            }
            if (parameters.TryGetValue("schedule_times_of_day", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_times_of_day is null, which cannot be sent" : "Bad parameter: schedule_times_of_day must be of type string[]", "schedule_times_of_day");
            }
            if (parameters.TryGetValue("src_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: src_path is null, which cannot be sent" : "Bad parameter: src_path must be of type string", "src_path");
            }
            if (parameters.TryGetValue("src_remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: src_remote_server_id is null, which cannot be sent" : "Bad parameter: src_remote_server_id must be of type Nullable<Int64>", "src_remote_server_id");
            }
            if (parameters.TryGetValue("sync_interval_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_interval_minutes is null, which cannot be sent" : "Bad parameter: sync_interval_minutes must be of type Nullable<Int64>", "sync_interval_minutes");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("trigger_file", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_file is null, which cannot be sent" : "Bad parameter: trigger_file must be of type string", "trigger_file");
            }
            if (parameters.TryGetValue("always_write_trigger_file", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_write_trigger_file is null, which cannot be sent" : "Bad parameter: always_write_trigger_file must be of type bool", "always_write_trigger_file");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/syncs", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Sync>(responseJson, context, options);
        }

        /// <summary>
        /// Dry Run Sync
        /// </summary>
        /// <param name="id">Sync ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task DryRun(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DryRunCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task DryRunCore(
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

            await FilesClient.SendRequestAndDiscard(context, $"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}/dry_run", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Manually Run Sync
        /// </summary>
        /// <param name="id">Sync ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task ManualRun(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ManualRunCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task ManualRunCore(
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

            await FilesClient.SendRequestAndDiscard(context, $"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   description - string - Description for this sync job
        ///   dest_path - string - Absolute destination path for the sync
        ///   dest_remote_server_id - int64 - Remote server ID for the destination (if remote)
        ///   disabled - boolean - Is this sync disabled?
        ///   exclude_patterns - array(string) - Array of glob patterns to exclude
        ///   holiday_region - string - Skip the sync if there is a formal, observed holiday for this region.
        ///   include_patterns - array(string) - Array of glob patterns to include
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   name - string - Name for this sync job
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the sync's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_time_zone - string - Time zone for the schedule. If not set, times are interpreted as UTC.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format. For `custom_schedule`, run at these times on specified days of week. For `daily`, run at these times on the scheduled interval date.
        ///   src_path - string - Absolute source path for the sync
        ///   src_remote_server_id - int64 - Remote server ID for the source (if remote)
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   always_write_trigger_file - boolean - If true, the trigger file will be sent at the end of a successful sync even when no files were transferred.
        /// </summary>
        /// <param name="id">Sync ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Sync> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<Sync> UpdateCore(
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
            if (parameters.TryGetValue("delete_empty_folders", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: delete_empty_folders is null, which cannot be sent" : "Bad parameter: delete_empty_folders must be of type bool", "delete_empty_folders");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("dest_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dest_path is null, which cannot be sent" : "Bad parameter: dest_path must be of type string", "dest_path");
            }
            if (parameters.TryGetValue("dest_remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: dest_remote_server_id is null, which cannot be sent" : "Bad parameter: dest_remote_server_id must be of type Nullable<Int64>", "dest_remote_server_id");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("exclude_patterns", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_patterns is null, which cannot be sent" : "Bad parameter: exclude_patterns must be of type string[]", "exclude_patterns");
            }
            if (parameters.TryGetValue("holiday_region", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: holiday_region is null, which cannot be sent" : "Bad parameter: holiday_region must be of type string", "holiday_region");
            }
            if (parameters.TryGetValue("include_patterns", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: include_patterns is null, which cannot be sent" : "Bad parameter: include_patterns must be of type string[]", "include_patterns");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("keep_after_copy", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: keep_after_copy is null, which cannot be sent" : "Bad parameter: keep_after_copy must be of type bool", "keep_after_copy");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
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
            if (parameters.TryGetValue("schedule_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_time_zone is null, which cannot be sent" : "Bad parameter: schedule_time_zone must be of type string", "schedule_time_zone");
            }
            if (parameters.TryGetValue("schedule_times_of_day", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: schedule_times_of_day is null, which cannot be sent" : "Bad parameter: schedule_times_of_day must be of type string[]", "schedule_times_of_day");
            }
            if (parameters.TryGetValue("src_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: src_path is null, which cannot be sent" : "Bad parameter: src_path must be of type string", "src_path");
            }
            if (parameters.TryGetValue("src_remote_server_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: src_remote_server_id is null, which cannot be sent" : "Bad parameter: src_remote_server_id must be of type Nullable<Int64>", "src_remote_server_id");
            }
            if (parameters.TryGetValue("sync_interval_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_interval_minutes is null, which cannot be sent" : "Bad parameter: sync_interval_minutes must be of type Nullable<Int64>", "sync_interval_minutes");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("trigger_file", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_file is null, which cannot be sent" : "Bad parameter: trigger_file must be of type string", "trigger_file");
            }
            if (parameters.TryGetValue("always_write_trigger_file", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_write_trigger_file is null, which cannot be sent" : "Bad parameter: always_write_trigger_file must be of type bool", "always_write_trigger_file");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Sync>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /syncs/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Sync ID.</param>
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
        /// Sends DELETE /syncs/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Sync ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}