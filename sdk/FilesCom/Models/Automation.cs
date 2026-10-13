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
    /// AutomationEntity model
    /// </summary>
    public class Automation : IModel, IJsonOnDeserialized
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
        private static readonly string[] ArrayAttributeNames = { "destinations", "import_urls", "group_ids", "recurring_days", "schedule_days_of_week", "schedule_times_of_day", "sync_ids", "trigger_actions", "user_ids" };
        /// <summary>
        /// Makes a Automation with no attributes set and no options.
        /// </summary>
        public Automation() : this(null, null) { }

        /// <summary>
        /// Makes a Automation with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Automation(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("always_serialize_jobs"))
            {
                this.attributes.Add("always_serialize_jobs", false);
            }
            if (!this.attributes.ContainsKey("always_overwrite_size_matching_files"))
            {
                this.attributes.Add("always_overwrite_size_matching_files", false);
            }
            if (!this.attributes.ContainsKey("automation"))
            {
                this.attributes.Add("automation", null);
            }
            if (!this.attributes.ContainsKey("deleted"))
            {
                this.attributes.Add("deleted", false);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("definition"))
            {
                this.attributes.Add("definition", null);
            }
            if (!this.attributes.ContainsKey("destination_replace_from"))
            {
                this.attributes.Add("destination_replace_from", null);
            }
            if (!this.attributes.ContainsKey("destination_replace_to"))
            {
                this.attributes.Add("destination_replace_to", null);
            }
            if (!this.attributes.ContainsKey("destinations"))
            {
                this.attributes.Add("destinations", new string[0]);
            }
            if (!this.attributes.ContainsKey("disabled"))
            {
                this.attributes.Add("disabled", false);
            }
            if (!this.attributes.ContainsKey("exclude_pattern"))
            {
                this.attributes.Add("exclude_pattern", null);
            }
            if (!this.attributes.ContainsKey("import_urls"))
            {
                this.attributes.Add("import_urls", new object[0]);
            }
            if (!this.attributes.ContainsKey("inbound_email_address"))
            {
                this.attributes.Add("inbound_email_address", null);
            }
            if (!this.attributes.ContainsKey("flatten_destination_structure"))
            {
                this.attributes.Add("flatten_destination_structure", false);
            }
            if (!this.attributes.ContainsKey("group_ids"))
            {
                this.attributes.Add("group_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("ignore_locked_folders"))
            {
                this.attributes.Add("ignore_locked_folders", false);
            }
            if (!this.attributes.ContainsKey("interval"))
            {
                this.attributes.Add("interval", null);
            }
            if (!this.attributes.ContainsKey("last_modified_at"))
            {
                this.attributes.Add("last_modified_at", null);
            }
            if (!this.attributes.ContainsKey("legacy_folder_matching"))
            {
                this.attributes.Add("legacy_folder_matching", false);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("overwrite_files"))
            {
                this.attributes.Add("overwrite_files", false);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("path_time_zone"))
            {
                this.attributes.Add("path_time_zone", null);
            }
            if (!this.attributes.ContainsKey("version"))
            {
                this.attributes.Add("version", null);
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
            if (!this.attributes.ContainsKey("retry_on_failure_interval_in_minutes"))
            {
                this.attributes.Add("retry_on_failure_interval_in_minutes", null);
            }
            if (!this.attributes.ContainsKey("retry_on_failure_number_of_attempts"))
            {
                this.attributes.Add("retry_on_failure_number_of_attempts", null);
            }
            if (!this.attributes.ContainsKey("schedule"))
            {
                this.attributes.Add("schedule", null);
            }
            if (!this.attributes.ContainsKey("human_readable_schedule"))
            {
                this.attributes.Add("human_readable_schedule", null);
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
            if (!this.attributes.ContainsKey("source"))
            {
                this.attributes.Add("source", null);
            }
            if (!this.attributes.ContainsKey("sync_ids"))
            {
                this.attributes.Add("sync_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("trigger_actions"))
            {
                this.attributes.Add("trigger_actions", new string[0]);
            }
            if (!this.attributes.ContainsKey("trigger"))
            {
                this.attributes.Add("trigger", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("user_ids"))
            {
                this.attributes.Add("user_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("value"))
            {
                this.attributes.Add("value", null);
            }
            if (!this.attributes.ContainsKey("webhook_url"))
            {
                this.attributes.Add("webhook_url", null);
            }
            if (!this.attributes.ContainsKey("holiday_region"))
            {
                this.attributes.Add("holiday_region", null);
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
        /// Automation ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Workspace ID
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("always_serialize_jobs")]
        public bool AlwaysSerializeJobs
        {
            get { return attributes["always_serialize_jobs"] == null ? false : (bool)attributes["always_serialize_jobs"]; }
            set { attributes["always_serialize_jobs"] = value; unsaved.Add("always_serialize_jobs"); }
        }

        /// <summary>
        /// Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("always_overwrite_size_matching_files")]
        public bool AlwaysOverwriteSizeMatchingFiles
        {
            get { return attributes["always_overwrite_size_matching_files"] == null ? false : (bool)attributes["always_overwrite_size_matching_files"]; }
            set { attributes["always_overwrite_size_matching_files"] = value; unsaved.Add("always_overwrite_size_matching_files"); }
        }

        /// <summary>
        /// Automation type
        /// </summary>
        [JsonPropertyName("automation")]
        public string AutomationType
        {
            get { return (string)attributes["automation"]; }
            set { attributes["automation"] = value; unsaved.Add("automation"); }
        }

        /// <summary>
        /// Indicates if the automation has been deleted.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("deleted")]
        public bool Deleted
        {
            get { return attributes["deleted"] == null ? false : (bool)attributes["deleted"]; }
            set { attributes["deleted"] = value; unsaved.Add("deleted"); }
        }

        /// <summary>
        /// Description for the this Automation.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; unsaved.Add("description"); }
        }

        /// <summary>
        /// Automation v2 graph definition.
        /// </summary>
        [JsonPropertyName("definition")]
        public object Definition
        {
            get { return (object)attributes["definition"]; }
            set { attributes["definition"] = value; unsaved.Add("definition"); }
        }

        /// <summary>
        /// If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        /// </summary>
        [JsonPropertyName("destination_replace_from")]
        public string DestinationReplaceFrom
        {
            get { return (string)attributes["destination_replace_from"]; }
            set { attributes["destination_replace_from"] = value; unsaved.Add("destination_replace_from"); }
        }

        /// <summary>
        /// If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        /// </summary>
        [JsonPropertyName("destination_replace_to")]
        public string DestinationReplaceTo
        {
            get { return (string)attributes["destination_replace_to"]; }
            set { attributes["destination_replace_to"] = value; unsaved.Add("destination_replace_to"); }
        }

        /// <summary>
        /// Destination Paths
        /// </summary>
        [JsonPropertyName("destinations")]
        public string[] Destinations
        {
            get { return (string[])HandedOut("destinations"); }
            set { attributes["destinations"] = value; unsaved.Add("destinations"); }
        }

        /// <summary>
        /// If true, this automation will not run.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; unsaved.Add("disabled"); }
        }

        /// <summary>
        /// If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        /// </summary>
        [JsonPropertyName("exclude_pattern")]
        public string ExcludePattern
        {
            get { return (string)attributes["exclude_pattern"]; }
            set { attributes["exclude_pattern"] = value; unsaved.Add("exclude_pattern"); }
        }

        /// <summary>
        /// List of URLs to be imported and names to be used.
        /// </summary>
        [JsonPropertyName("import_urls")]
        public object[] ImportUrls
        {
            get { return (object[])HandedOut("import_urls"); }
            set { attributes["import_urls"] = value; unsaved.Add("import_urls"); }
        }

        /// <summary>
        /// If trigger is `email`, this is the address that triggers the Automation.
        /// </summary>
        [JsonPropertyName("inbound_email_address")]
        public string InboundEmailAddress
        {
            get { return (string)attributes["inbound_email_address"]; }
            set { attributes["inbound_email_address"] = value; unsaved.Add("inbound_email_address"); }
        }

        /// <summary>
        /// Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("flatten_destination_structure")]
        public bool FlattenDestinationStructure
        {
            get { return attributes["flatten_destination_structure"] == null ? false : (bool)attributes["flatten_destination_structure"]; }
            set { attributes["flatten_destination_structure"] = value; unsaved.Add("flatten_destination_structure"); }
        }

        /// <summary>
        /// IDs of Groups for the Automation (i.e. who to Request File from)
        /// </summary>
        [JsonPropertyName("group_ids")]
        public Nullable<Int64>[] GroupIds
        {
            get { return (Nullable<Int64>[])HandedOut("group_ids"); }
            set { attributes["group_ids"] = value; unsaved.Add("group_ids"); }
        }

        /// <summary>
        /// If true, the Lock Folders behavior will be disregarded for automated actions.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ignore_locked_folders")]
        public bool IgnoreLockedFolders
        {
            get { return attributes["ignore_locked_folders"] == null ? false : (bool)attributes["ignore_locked_folders"]; }
            set { attributes["ignore_locked_folders"] = value; unsaved.Add("ignore_locked_folders"); }
        }

        /// <summary>
        /// If trigger is `daily`, this specifies how often to run this automation.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval
        {
            get { return (string)attributes["interval"]; }
            set { attributes["interval"] = value; unsaved.Add("interval"); }
        }

        /// <summary>
        /// Time when automation was last modified. Does not change for name or description updates.
        /// </summary>
        [JsonPropertyName("last_modified_at")]
        public Nullable<DateTime> LastModifiedAt
        {
            get { return (Nullable<DateTime>)attributes["last_modified_at"]; }
            set { attributes["last_modified_at"] = value; unsaved.Add("last_modified_at"); }
        }

        /// <summary>
        /// If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("legacy_folder_matching")]
        public bool LegacyFolderMatching
        {
            get { return attributes["legacy_folder_matching"] == null ? false : (bool)attributes["legacy_folder_matching"]; }
            set { attributes["legacy_folder_matching"] = value; unsaved.Add("legacy_folder_matching"); }
        }

        /// <summary>
        /// Name for this automation.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("overwrite_files")]
        public bool OverwriteFiles
        {
            get { return attributes["overwrite_files"] == null ? false : (bool)attributes["overwrite_files"]; }
            set { attributes["overwrite_files"] = value; unsaved.Add("overwrite_files"); }
        }

        /// <summary>
        /// Path on which this Automation runs.  Supports globs, except on remote mounts. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; unsaved.Add("path"); }
        }

        /// <summary>
        /// Timezone to use when rendering timestamps in paths.
        /// </summary>
        [JsonPropertyName("path_time_zone")]
        public string PathTimeZone
        {
            get { return (string)attributes["path_time_zone"]; }
            set { attributes["path_time_zone"] = value; unsaved.Add("path_time_zone"); }
        }

        /// <summary>
        /// Current Automation v2 definition version.
        /// </summary>
        [JsonPropertyName("version")]
        public Nullable<Int64> Version
        {
            get { return (Nullable<Int64>)attributes["version"]; }
            set { attributes["version"] = value; unsaved.Add("version"); }
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
        /// If trigger is `custom_schedule`, the reusable Schedule used instead of the automation's schedule fields.
        /// </summary>
        [JsonPropertyName("schedule_id")]
        public Nullable<Int64> ScheduleId
        {
            get { return (Nullable<Int64>)attributes["schedule_id"]; }
            set { attributes["schedule_id"] = value; unsaved.Add("schedule_id"); }
        }

        /// <summary>
        /// If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
        /// </summary>
        [JsonPropertyName("retry_on_failure_interval_in_minutes")]
        public Nullable<Int64> RetryOnFailureIntervalInMinutes
        {
            get { return (Nullable<Int64>)attributes["retry_on_failure_interval_in_minutes"]; }
            set { attributes["retry_on_failure_interval_in_minutes"] = value; unsaved.Add("retry_on_failure_interval_in_minutes"); }
        }

        /// <summary>
        /// If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
        /// </summary>
        [JsonPropertyName("retry_on_failure_number_of_attempts")]
        public Nullable<Int64> RetryOnFailureNumberOfAttempts
        {
            get { return (Nullable<Int64>)attributes["retry_on_failure_number_of_attempts"]; }
            set { attributes["retry_on_failure_number_of_attempts"] = value; unsaved.Add("retry_on_failure_number_of_attempts"); }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule description for when the automation should be run in json format.
        /// </summary>
        [JsonPropertyName("schedule")]
        public object Schedule
        {
            get { return (object)attributes["schedule"]; }
            set { attributes["schedule"] = value; unsaved.Add("schedule"); }
        }

        /// <summary>
        /// If trigger is `custom_schedule` or `daily` with times, Human readable schedule description for when the automation should be run.
        /// </summary>
        [JsonPropertyName("human_readable_schedule")]
        public string HumanReadableSchedule
        {
            get { return (string)attributes["human_readable_schedule"]; }
            set { attributes["human_readable_schedule"] = value; unsaved.Add("human_readable_schedule"); }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule description for when the automation should be run. 0 is Sunday, 1 is Monday, etc.
        /// </summary>
        [JsonPropertyName("schedule_days_of_week")]
        public Nullable<Int64>[] ScheduleDaysOfWeek
        {
            get { return (Nullable<Int64>[])HandedOut("schedule_days_of_week"); }
            set { attributes["schedule_days_of_week"] = value; unsaved.Add("schedule_days_of_week"); }
        }

        /// <summary>
        /// Times of day to run in HH:MM format (24-hour). For `custom_schedule`, run at these times on specified days of week. For `daily`, run at these times on the scheduled interval date.
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
        /// Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string)attributes["source"]; }
            set { attributes["source"] = value; unsaved.Add("source"); }
        }

        /// <summary>
        /// IDs of Syncs to run by this Automation.
        /// </summary>
        [JsonPropertyName("sync_ids")]
        public Nullable<Int64>[] SyncIds
        {
            get { return (Nullable<Int64>[])HandedOut("sync_ids"); }
            set { attributes["sync_ids"] = value; unsaved.Add("sync_ids"); }
        }

        /// <summary>
        /// If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, copy, move, archived_delete, update, read, destroy
        /// </summary>
        [JsonPropertyName("trigger_actions")]
        public string[] TriggerActions
        {
            get { return (string[])HandedOut("trigger_actions"); }
            set { attributes["trigger_actions"] = value; unsaved.Add("trigger_actions"); }
        }

        /// <summary>
        /// How this automation is triggered to run.
        /// </summary>
        [JsonPropertyName("trigger")]
        public string Trigger
        {
            get { return (string)attributes["trigger"]; }
            set { attributes["trigger"] = value; unsaved.Add("trigger"); }
        }

        /// <summary>
        /// User ID of the Automation's creator.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; unsaved.Add("user_id"); }
        }

        /// <summary>
        /// IDs of Users for the Automation (i.e. who to Request File from)
        /// </summary>
        [JsonPropertyName("user_ids")]
        public Nullable<Int64>[] UserIds
        {
            get { return (Nullable<Int64>[])HandedOut("user_ids"); }
            set { attributes["user_ids"] = value; unsaved.Add("user_ids"); }
        }

        /// <summary>
        /// A Hash of attributes specific to the automation type.
        /// </summary>
        [JsonPropertyName("value")]
        public object Value
        {
            get { return (object)attributes["value"]; }
            set { attributes["value"] = value; unsaved.Add("value"); }
        }

        /// <summary>
        /// If trigger is `webhook`, this is the URL of the webhook to trigger the Automation.
        /// </summary>
        [JsonPropertyName("webhook_url")]
        public string WebhookUrl
        {
            get { return (string)attributes["webhook_url"]; }
            set { attributes["webhook_url"] = value; unsaved.Add("webhook_url"); }
        }

        /// <summary>
        /// Skip the automation if there is a formal, observed holiday for this region.
        /// </summary>
        [JsonPropertyName("holiday_region")]
        public string HolidayRegion
        {
            get { return (string)attributes["holiday_region"]; }
            set { attributes["holiday_region"] = value; unsaved.Add("holiday_region"); }
        }

        /// <summary>
        /// Upgrade a legacy Automation to Automation v2
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task<Automation> Upgrade(Dictionary<string, object> parameters)
        {
            return UpgradeCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Upgrade a legacy Automation to Automation v2
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<Automation> UpgradeAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpgradeCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Automation> UpgradeCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Automation has no id, which Upgrade needs", "id");
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
            string responseJson = await FilesClient.SendStringRequest(context, $"/automations/{System.Uri.EscapeDataString(attributes["id"].ToString())}/upgrade", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            Automation result = JsonUtil.DeserializeModelResponse<Automation>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }

        /// <summary>
        /// Manually Run Automation
        ///
        /// Parameters:
        ///   items - array(object) - Initial items for a v2 manual trigger. Each item contains exactly one `file` path or `data` object.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task ManualRun(Dictionary<string, object> parameters)
        {
            return ManualRunCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Manually Run Automation
        ///
        /// Parameters:
        ///   items - array(object) - Initial items for a v2 manual trigger. Each item contains exactly one `file` path or `data` object.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
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
                throw new MissingParameterException("The Automation has no id, which ManualRun needs", "id");
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
            if (parameters.TryGetValue("items", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: items is null, which cannot be sent" : "Bad parameter: items must be of type object[]", "items");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/automations/{System.Uri.EscapeDataString(attributes["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }

        /// <summary>
        /// Parameters:
        ///   source - string - Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        ///   destinations - array(string) - A list of destination paths. Use a trailing slash for folder destinations and omit it for file destinations.
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs, except on remote mounts.
        ///   sync_ids - string - A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the automation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule` without `schedule_id`, a list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format (24-hour). Required for `custom_schedule` without `schedule_id`. Optional for `daily` triggers; if not set, runs at midnight UTC.
        ///   schedule_time_zone - string - Time zone for schedule fields. Optional for `custom_schedule` without `schedule_id` and for `daily`. If not set, times are interpreted as UTC.
        ///   holiday_region - string - Skip automation on holidays in this region. Optional for `custom_schedule` without `schedule_id` and for `daily`.
        ///   always_overwrite_size_matching_files - boolean - Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
        ///   always_serialize_jobs - boolean - Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
        ///   description - string - Description for the this Automation.
        ///   definition - object - Automation v2 graph definition.
        ///   disabled - boolean - If true, this automation will not run.
        ///   exclude_pattern - string - If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        ///   import_urls - array(object) - List of URLs to be imported and names to be used.
        ///   flatten_destination_structure - boolean - Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        ///   ignore_locked_folders - boolean - If true, the Lock Folders behavior will be disregarded for automated actions.
        ///   legacy_folder_matching - boolean - DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        ///   name - string - Name for this automation.
        ///   overwrite_files - boolean - If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
        ///   path_time_zone - string - Timezone to use when rendering timestamps in paths.
        ///   retry_on_failure_interval_in_minutes - int64 - If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
        ///   retry_on_failure_number_of_attempts - int64 - If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
        ///   trigger - string - How this automation is triggered to run.
        ///   trigger_actions - array(string) - If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, copy, move, archived_delete, update, read, destroy
        ///   value - object - A Hash of attributes specific to the automation type.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   automation - string - Automation type
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Automation it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Automation> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   source - string - Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        ///   destinations - array(string) - A list of destination paths. Use a trailing slash for folder destinations and omit it for file destinations.
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs, except on remote mounts.
        ///   sync_ids - string - A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the automation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule` without `schedule_id`, a list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format (24-hour). Required for `custom_schedule` without `schedule_id`. Optional for `daily` triggers; if not set, runs at midnight UTC.
        ///   schedule_time_zone - string - Time zone for schedule fields. Optional for `custom_schedule` without `schedule_id` and for `daily`. If not set, times are interpreted as UTC.
        ///   holiday_region - string - Skip automation on holidays in this region. Optional for `custom_schedule` without `schedule_id` and for `daily`.
        ///   always_overwrite_size_matching_files - boolean - Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
        ///   always_serialize_jobs - boolean - Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
        ///   description - string - Description for the this Automation.
        ///   definition - object - Automation v2 graph definition.
        ///   disabled - boolean - If true, this automation will not run.
        ///   exclude_pattern - string - If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        ///   import_urls - array(object) - List of URLs to be imported and names to be used.
        ///   flatten_destination_structure - boolean - Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        ///   ignore_locked_folders - boolean - If true, the Lock Folders behavior will be disregarded for automated actions.
        ///   legacy_folder_matching - boolean - DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        ///   name - string - Name for this automation.
        ///   overwrite_files - boolean - If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
        ///   path_time_zone - string - Timezone to use when rendering timestamps in paths.
        ///   retry_on_failure_interval_in_minutes - int64 - If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
        ///   retry_on_failure_number_of_attempts - int64 - If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
        ///   trigger - string - How this automation is triggered to run.
        ///   trigger_actions - array(string) - If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, copy, move, archived_delete, update, read, destroy
        ///   value - object - A Hash of attributes specific to the automation type.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   automation - string - Automation type
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Automation it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Automation> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Automation> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Automation has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("source", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: source is null, which cannot be sent" : "Bad parameter: source must be of type string", "source");
            }
            if (parameters.TryGetValue("destinations", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destinations is null, which cannot be sent" : "Bad parameter: destinations must be of type string[]", "destinations");
            }
            if (parameters.TryGetValue("destination_replace_from", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_replace_from is null, which cannot be sent" : "Bad parameter: destination_replace_from must be of type string", "destination_replace_from");
            }
            if (parameters.TryGetValue("destination_replace_to", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_replace_to is null, which cannot be sent" : "Bad parameter: destination_replace_to must be of type string", "destination_replace_to");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("sync_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_ids is null, which cannot be sent" : "Bad parameter: sync_ids must be of type string", "sync_ids");
            }
            if (parameters.TryGetValue("user_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_ids is null, which cannot be sent" : "Bad parameter: user_ids must be of type string", "user_ids");
            }
            if (parameters.TryGetValue("group_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_ids is null, which cannot be sent" : "Bad parameter: group_ids must be of type string", "group_ids");
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
            if (parameters.TryGetValue("always_overwrite_size_matching_files", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_overwrite_size_matching_files is null, which cannot be sent" : "Bad parameter: always_overwrite_size_matching_files must be of type bool", "always_overwrite_size_matching_files");
            }
            if (parameters.TryGetValue("always_serialize_jobs", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_serialize_jobs is null, which cannot be sent" : "Bad parameter: always_serialize_jobs must be of type bool", "always_serialize_jobs");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("definition", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: definition is null, which cannot be sent" : "Bad parameter: definition must be of type object", "definition");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("exclude_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_pattern is null, which cannot be sent" : "Bad parameter: exclude_pattern must be of type string", "exclude_pattern");
            }
            if (parameters.TryGetValue("import_urls", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: import_urls is null, which cannot be sent" : "Bad parameter: import_urls must be of type object[]", "import_urls");
            }
            if (parameters.TryGetValue("flatten_destination_structure", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: flatten_destination_structure is null, which cannot be sent" : "Bad parameter: flatten_destination_structure must be of type bool", "flatten_destination_structure");
            }
            if (parameters.TryGetValue("ignore_locked_folders", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ignore_locked_folders is null, which cannot be sent" : "Bad parameter: ignore_locked_folders must be of type bool", "ignore_locked_folders");
            }
            if (parameters.TryGetValue("legacy_folder_matching", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: legacy_folder_matching is null, which cannot be sent" : "Bad parameter: legacy_folder_matching must be of type bool", "legacy_folder_matching");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("overwrite_files", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite_files is null, which cannot be sent" : "Bad parameter: overwrite_files must be of type bool", "overwrite_files");
            }
            if (parameters.TryGetValue("path_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_time_zone is null, which cannot be sent" : "Bad parameter: path_time_zone must be of type string", "path_time_zone");
            }
            if (parameters.TryGetValue("retry_on_failure_interval_in_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: retry_on_failure_interval_in_minutes is null, which cannot be sent" : "Bad parameter: retry_on_failure_interval_in_minutes must be of type Nullable<Int64>", "retry_on_failure_interval_in_minutes");
            }
            if (parameters.TryGetValue("retry_on_failure_number_of_attempts", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: retry_on_failure_number_of_attempts is null, which cannot be sent" : "Bad parameter: retry_on_failure_number_of_attempts must be of type Nullable<Int64>", "retry_on_failure_number_of_attempts");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("trigger_actions", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_actions is null, which cannot be sent" : "Bad parameter: trigger_actions must be of type string[]", "trigger_actions");
            }
            if (parameters.TryGetValue("value", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: value is null, which cannot be sent" : "Bad parameter: value must be of type object", "value");
            }
            if (parameters.TryGetValue("recurring_day", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_day is null, which cannot be sent" : "Bad parameter: recurring_day must be of type Nullable<Int64>", "recurring_day");
            }
            if (parameters.TryGetValue("recurring_days", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_days is null, which cannot be sent" : "Bad parameter: recurring_days must be of type Nullable<Int64>[]", "recurring_days");
            }
            if (parameters.TryGetValue("automation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: automation is null, which cannot be sent" : "Bad parameter: automation must be of type string", "automation");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/automations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            Automation updated = JsonUtil.DeserializeModelResponse<Automation>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<Automation>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /automations/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /automations/{id} to the Files.com API.
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
                throw new MissingParameterException("The Automation has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/automations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "source", "destinations", "destination_replace_from", "destination_replace_to", "interval", "path", "sync_ids", "user_ids", "group_ids", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region", "always_overwrite_size_matching_files", "always_serialize_jobs", "description", "definition", "disabled", "exclude_pattern", "import_urls", "flatten_destination_structure", "ignore_locked_folders", "legacy_folder_matching", "name", "overwrite_files", "path_time_zone", "retry_on_failure_interval_in_minutes", "retry_on_failure_number_of_attempts", "trigger", "trigger_actions", "value", "recurring_day", "recurring_days", "automation", "workspace_id" };
        private static readonly string[] UpdateParameterNames = { "id", "source", "destinations", "destination_replace_from", "destination_replace_to", "interval", "path", "sync_ids", "user_ids", "group_ids", "schedule_id", "schedule_days_of_week", "schedule_times_of_day", "schedule_time_zone", "holiday_region", "always_overwrite_size_matching_files", "always_serialize_jobs", "description", "definition", "disabled", "exclude_pattern", "import_urls", "flatten_destination_structure", "ignore_locked_folders", "legacy_folder_matching", "name", "overwrite_files", "path_time_zone", "retry_on_failure_interval_in_minutes", "retry_on_failure_number_of_attempts", "trigger", "trigger_actions", "value", "recurring_day", "recurring_days", "automation" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Automation on Files.com, or updates it if it has an ID.
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
            Automation created = await Automation.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
                    toSave[name] = ToParameter(name, attributes[name]);
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

        // The value SaveAsync sends for an attribute whose property type is not the one its parameter takes.
        private static object ToParameter(string name, object value)
        {
            switch (name)
            {
                case "sync_ids":
                case "user_ids":
                case "group_ids":
                    return ParameterValue.FromIds(value);
                default:
                    return value;
            }
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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name`, `automation`, `last_modified_at` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled`, `last_modified_at`, `workspace_id` or `automation`. Valid field combinations are `[ disabled, last_modified_at ]`, `[ workspace_id, disabled ]`, `[ disabled, automation ]`, `[ workspace_id, last_modified_at ]`, `[ automation, last_modified_at ]`, `[ workspace_id, automation ]`, `[ workspace_id, disabled, last_modified_at ]`, `[ disabled, automation, last_modified_at ]`, `[ workspace_id, disabled, automation ]`, `[ workspace_id, automation, last_modified_at ]` or `[ workspace_id, disabled, automation, last_modified_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `last_modified_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `last_modified_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `last_modified_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `last_modified_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Automation> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name`, `automation`, `last_modified_at` or `disabled`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled`, `last_modified_at`, `workspace_id` or `automation`. Valid field combinations are `[ disabled, last_modified_at ]`, `[ workspace_id, disabled ]`, `[ disabled, automation ]`, `[ workspace_id, last_modified_at ]`, `[ automation, last_modified_at ]`, `[ workspace_id, automation ]`, `[ workspace_id, disabled, last_modified_at ]`, `[ disabled, automation, last_modified_at ]`, `[ workspace_id, disabled, automation ]`, `[ workspace_id, automation, last_modified_at ]` or `[ workspace_id, disabled, automation, last_modified_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `last_modified_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `last_modified_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `last_modified_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `last_modified_at`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Automation> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Automation> ListCore(
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
            if (parameters.TryGetValue("filter_gt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gt is null, which cannot be sent" : "Bad parameter: filter_gt must be of type object", "filter_gt");
            }
            if (parameters.TryGetValue("filter_gteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_gteq is null, which cannot be sent" : "Bad parameter: filter_gteq must be of type object", "filter_gteq");
            }
            if (parameters.TryGetValue("filter_lt", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lt is null, which cannot be sent" : "Bad parameter: filter_lt must be of type object", "filter_lt");
            }
            if (parameters.TryGetValue("filter_lteq", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_lteq is null, which cannot be sent" : "Bad parameter: filter_lteq must be of type object", "filter_lteq");
            }

            return new FilesList<Automation>(client, $"/automations", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Automation ID.
        /// </summary>
        /// <param name="id">Automation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Automation> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Automation ID.
        /// </summary>
        /// <param name="id">Automation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Automation> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<Automation> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Automation>(responseJson, context, options);
        }

        /// <summary>
        /// Sends GET /automations/authoring_schema to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AutomationAuthoringSchema> GetAuthoringSchema(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return GetAuthoringSchemaCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<AutomationAuthoringSchema> GetAuthoringSchemaCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();


            string responseJson = await FilesClient.SendStringRequest(context, $"/automations/authoring_schema", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AutomationAuthoringSchema>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   source - string - Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        ///   destinations - array(string) - A list of destination paths. Use a trailing slash for folder destinations and omit it for file destinations.
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs, except on remote mounts.
        ///   sync_ids - string - A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the automation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule` without `schedule_id`, a list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format (24-hour). Required for `custom_schedule` without `schedule_id`. Optional for `daily` triggers; if not set, runs at midnight UTC.
        ///   schedule_time_zone - string - Time zone for schedule fields. Optional for `custom_schedule` without `schedule_id` and for `daily`. If not set, times are interpreted as UTC.
        ///   holiday_region - string - Skip automation on holidays in this region. Optional for `custom_schedule` without `schedule_id` and for `daily`.
        ///   always_overwrite_size_matching_files - boolean - Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
        ///   always_serialize_jobs - boolean - Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
        ///   description - string - Description for the this Automation.
        ///   definition - object - Automation v2 graph definition.
        ///   disabled - boolean - If true, this automation will not run.
        ///   exclude_pattern - string - If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        ///   import_urls - array(object) - List of URLs to be imported and names to be used.
        ///   flatten_destination_structure - boolean - Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        ///   ignore_locked_folders - boolean - If true, the Lock Folders behavior will be disregarded for automated actions.
        ///   legacy_folder_matching - boolean - DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        ///   name - string - Name for this automation.
        ///   overwrite_files - boolean - If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
        ///   path_time_zone - string - Timezone to use when rendering timestamps in paths.
        ///   retry_on_failure_interval_in_minutes - int64 - If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
        ///   retry_on_failure_number_of_attempts - int64 - If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
        ///   trigger - string - How this automation is triggered to run.
        ///   trigger_actions - array(string) - If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, copy, move, archived_delete, update, read, destroy
        ///   value - object - A Hash of attributes specific to the automation type.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   automation (required) - string - Automation type
        ///   workspace_id - int64 - Workspace ID
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Automation> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Automation> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("automation", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: automation", "automation");
            }
            if (parameters.TryGetValue("source", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: source is null, which cannot be sent" : "Bad parameter: source must be of type string", "source");
            }
            if (parameters.TryGetValue("destinations", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destinations is null, which cannot be sent" : "Bad parameter: destinations must be of type string[]", "destinations");
            }
            if (parameters.TryGetValue("destination_replace_from", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_replace_from is null, which cannot be sent" : "Bad parameter: destination_replace_from must be of type string", "destination_replace_from");
            }
            if (parameters.TryGetValue("destination_replace_to", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_replace_to is null, which cannot be sent" : "Bad parameter: destination_replace_to must be of type string", "destination_replace_to");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("sync_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_ids is null, which cannot be sent" : "Bad parameter: sync_ids must be of type string", "sync_ids");
            }
            if (parameters.TryGetValue("user_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_ids is null, which cannot be sent" : "Bad parameter: user_ids must be of type string", "user_ids");
            }
            if (parameters.TryGetValue("group_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_ids is null, which cannot be sent" : "Bad parameter: group_ids must be of type string", "group_ids");
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
            if (parameters.TryGetValue("always_overwrite_size_matching_files", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_overwrite_size_matching_files is null, which cannot be sent" : "Bad parameter: always_overwrite_size_matching_files must be of type bool", "always_overwrite_size_matching_files");
            }
            if (parameters.TryGetValue("always_serialize_jobs", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_serialize_jobs is null, which cannot be sent" : "Bad parameter: always_serialize_jobs must be of type bool", "always_serialize_jobs");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("definition", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: definition is null, which cannot be sent" : "Bad parameter: definition must be of type object", "definition");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("exclude_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_pattern is null, which cannot be sent" : "Bad parameter: exclude_pattern must be of type string", "exclude_pattern");
            }
            if (parameters.TryGetValue("import_urls", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: import_urls is null, which cannot be sent" : "Bad parameter: import_urls must be of type object[]", "import_urls");
            }
            if (parameters.TryGetValue("flatten_destination_structure", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: flatten_destination_structure is null, which cannot be sent" : "Bad parameter: flatten_destination_structure must be of type bool", "flatten_destination_structure");
            }
            if (parameters.TryGetValue("ignore_locked_folders", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ignore_locked_folders is null, which cannot be sent" : "Bad parameter: ignore_locked_folders must be of type bool", "ignore_locked_folders");
            }
            if (parameters.TryGetValue("legacy_folder_matching", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: legacy_folder_matching is null, which cannot be sent" : "Bad parameter: legacy_folder_matching must be of type bool", "legacy_folder_matching");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("overwrite_files", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite_files is null, which cannot be sent" : "Bad parameter: overwrite_files must be of type bool", "overwrite_files");
            }
            if (parameters.TryGetValue("path_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_time_zone is null, which cannot be sent" : "Bad parameter: path_time_zone must be of type string", "path_time_zone");
            }
            if (parameters.TryGetValue("retry_on_failure_interval_in_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: retry_on_failure_interval_in_minutes is null, which cannot be sent" : "Bad parameter: retry_on_failure_interval_in_minutes must be of type Nullable<Int64>", "retry_on_failure_interval_in_minutes");
            }
            if (parameters.TryGetValue("retry_on_failure_number_of_attempts", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: retry_on_failure_number_of_attempts is null, which cannot be sent" : "Bad parameter: retry_on_failure_number_of_attempts must be of type Nullable<Int64>", "retry_on_failure_number_of_attempts");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("trigger_actions", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_actions is null, which cannot be sent" : "Bad parameter: trigger_actions must be of type string[]", "trigger_actions");
            }
            if (parameters.TryGetValue("value", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: value is null, which cannot be sent" : "Bad parameter: value must be of type object", "value");
            }
            if (parameters.TryGetValue("recurring_day", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_day is null, which cannot be sent" : "Bad parameter: recurring_day must be of type Nullable<Int64>", "recurring_day");
            }
            if (parameters.TryGetValue("recurring_days", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_days is null, which cannot be sent" : "Bad parameter: recurring_days must be of type Nullable<Int64>[]", "recurring_days");
            }
            if (parameters.TryGetValue("automation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: automation is null, which cannot be sent" : "Bad parameter: automation must be of type string", "automation");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/automations", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Automation>(responseJson, context, options);
        }

        /// <summary>
        /// Upgrade a legacy Automation to Automation v2
        /// </summary>
        /// <param name="id">Automation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Automation> Upgrade(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpgradeCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<Automation> UpgradeCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}/upgrade", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Automation>(responseJson, context, options);
        }

        /// <summary>
        /// Manually Run Automation
        ///
        /// Parameters:
        ///   items - array(object) - Initial items for a v2 manual trigger. Each item contains exactly one `file` path or `data` object.
        /// </summary>
        /// <param name="id">Automation ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
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
            if (parameters.TryGetValue("items", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: items is null, which cannot be sent" : "Bad parameter: items must be of type object[]", "items");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   source - string - Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        ///   destinations - array(string) - A list of destination paths. Use a trailing slash for folder destinations and omit it for file destinations.
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs, except on remote mounts.
        ///   sync_ids - string - A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   schedule_id - int64 - If trigger is `custom_schedule`, the reusable Schedule used instead of the automation's schedule fields.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule` without `schedule_id`, a list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - Times of day to run in HH:MM format (24-hour). Required for `custom_schedule` without `schedule_id`. Optional for `daily` triggers; if not set, runs at midnight UTC.
        ///   schedule_time_zone - string - Time zone for schedule fields. Optional for `custom_schedule` without `schedule_id` and for `daily`. If not set, times are interpreted as UTC.
        ///   holiday_region - string - Skip automation on holidays in this region. Optional for `custom_schedule` without `schedule_id` and for `daily`.
        ///   always_overwrite_size_matching_files - boolean - Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
        ///   always_serialize_jobs - boolean - Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
        ///   description - string - Description for the this Automation.
        ///   definition - object - Automation v2 graph definition.
        ///   disabled - boolean - If true, this automation will not run.
        ///   exclude_pattern - string - If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        ///   import_urls - array(object) - List of URLs to be imported and names to be used.
        ///   flatten_destination_structure - boolean - Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        ///   ignore_locked_folders - boolean - If true, the Lock Folders behavior will be disregarded for automated actions.
        ///   legacy_folder_matching - boolean - DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        ///   name - string - Name for this automation.
        ///   overwrite_files - boolean - If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
        ///   path_time_zone - string - Timezone to use when rendering timestamps in paths.
        ///   retry_on_failure_interval_in_minutes - int64 - If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
        ///   retry_on_failure_number_of_attempts - int64 - If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
        ///   trigger - string - How this automation is triggered to run.
        ///   trigger_actions - array(string) - If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, copy, move, archived_delete, update, read, destroy
        ///   value - object - A Hash of attributes specific to the automation type.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   recurring_days - array(int64) - If trigger type is `daily`, this specifies one or more day numbers to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   automation - string - Automation type
        /// </summary>
        /// <param name="id">Automation ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Automation> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<Automation> UpdateCore(
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
            if (parameters.TryGetValue("source", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: source is null, which cannot be sent" : "Bad parameter: source must be of type string", "source");
            }
            if (parameters.TryGetValue("destinations", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destinations is null, which cannot be sent" : "Bad parameter: destinations must be of type string[]", "destinations");
            }
            if (parameters.TryGetValue("destination_replace_from", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_replace_from is null, which cannot be sent" : "Bad parameter: destination_replace_from must be of type string", "destination_replace_from");
            }
            if (parameters.TryGetValue("destination_replace_to", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: destination_replace_to is null, which cannot be sent" : "Bad parameter: destination_replace_to must be of type string", "destination_replace_to");
            }
            if (parameters.TryGetValue("interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: interval is null, which cannot be sent" : "Bad parameter: interval must be of type string", "interval");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("sync_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sync_ids is null, which cannot be sent" : "Bad parameter: sync_ids must be of type string", "sync_ids");
            }
            if (parameters.TryGetValue("user_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_ids is null, which cannot be sent" : "Bad parameter: user_ids must be of type string", "user_ids");
            }
            if (parameters.TryGetValue("group_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_ids is null, which cannot be sent" : "Bad parameter: group_ids must be of type string", "group_ids");
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
            if (parameters.TryGetValue("always_overwrite_size_matching_files", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_overwrite_size_matching_files is null, which cannot be sent" : "Bad parameter: always_overwrite_size_matching_files must be of type bool", "always_overwrite_size_matching_files");
            }
            if (parameters.TryGetValue("always_serialize_jobs", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: always_serialize_jobs is null, which cannot be sent" : "Bad parameter: always_serialize_jobs must be of type bool", "always_serialize_jobs");
            }
            if (parameters.TryGetValue("description", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: description is null, which cannot be sent" : "Bad parameter: description must be of type string", "description");
            }
            if (parameters.TryGetValue("definition", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: definition is null, which cannot be sent" : "Bad parameter: definition must be of type object", "definition");
            }
            if (parameters.TryGetValue("disabled", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: disabled is null, which cannot be sent" : "Bad parameter: disabled must be of type bool", "disabled");
            }
            if (parameters.TryGetValue("exclude_pattern", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: exclude_pattern is null, which cannot be sent" : "Bad parameter: exclude_pattern must be of type string", "exclude_pattern");
            }
            if (parameters.TryGetValue("import_urls", out parameterValue) && !(parameterValue is object[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: import_urls is null, which cannot be sent" : "Bad parameter: import_urls must be of type object[]", "import_urls");
            }
            if (parameters.TryGetValue("flatten_destination_structure", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: flatten_destination_structure is null, which cannot be sent" : "Bad parameter: flatten_destination_structure must be of type bool", "flatten_destination_structure");
            }
            if (parameters.TryGetValue("ignore_locked_folders", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ignore_locked_folders is null, which cannot be sent" : "Bad parameter: ignore_locked_folders must be of type bool", "ignore_locked_folders");
            }
            if (parameters.TryGetValue("legacy_folder_matching", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: legacy_folder_matching is null, which cannot be sent" : "Bad parameter: legacy_folder_matching must be of type bool", "legacy_folder_matching");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("overwrite_files", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: overwrite_files is null, which cannot be sent" : "Bad parameter: overwrite_files must be of type bool", "overwrite_files");
            }
            if (parameters.TryGetValue("path_time_zone", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path_time_zone is null, which cannot be sent" : "Bad parameter: path_time_zone must be of type string", "path_time_zone");
            }
            if (parameters.TryGetValue("retry_on_failure_interval_in_minutes", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: retry_on_failure_interval_in_minutes is null, which cannot be sent" : "Bad parameter: retry_on_failure_interval_in_minutes must be of type Nullable<Int64>", "retry_on_failure_interval_in_minutes");
            }
            if (parameters.TryGetValue("retry_on_failure_number_of_attempts", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: retry_on_failure_number_of_attempts is null, which cannot be sent" : "Bad parameter: retry_on_failure_number_of_attempts must be of type Nullable<Int64>", "retry_on_failure_number_of_attempts");
            }
            if (parameters.TryGetValue("trigger", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger is null, which cannot be sent" : "Bad parameter: trigger must be of type string", "trigger");
            }
            if (parameters.TryGetValue("trigger_actions", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_actions is null, which cannot be sent" : "Bad parameter: trigger_actions must be of type string[]", "trigger_actions");
            }
            if (parameters.TryGetValue("value", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: value is null, which cannot be sent" : "Bad parameter: value must be of type object", "value");
            }
            if (parameters.TryGetValue("recurring_day", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_day is null, which cannot be sent" : "Bad parameter: recurring_day must be of type Nullable<Int64>", "recurring_day");
            }
            if (parameters.TryGetValue("recurring_days", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recurring_days is null, which cannot be sent" : "Bad parameter: recurring_days must be of type Nullable<Int64>[]", "recurring_days");
            }
            if (parameters.TryGetValue("automation", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: automation is null, which cannot be sent" : "Bad parameter: automation must be of type string", "automation");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Automation>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /automations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Automation ID.</param>
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
        /// Sends DELETE /automations/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Automation ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}