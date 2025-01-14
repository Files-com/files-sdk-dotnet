using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
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
            if (!this.attributes.ContainsKey("recurring_day"))
            {
                this.attributes.Add("recurring_day", null);
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
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("always_overwrite_size_matching_files")]
        public bool AlwaysOverwriteSizeMatchingFiles
        {
            get { return attributes["always_overwrite_size_matching_files"] == null ? false : (bool)attributes["always_overwrite_size_matching_files"]; }
            set { attributes["always_overwrite_size_matching_files"] = value; }
        }

        /// <summary>
        /// Automation type
        /// </summary>
        [JsonPropertyName("automation")]
        public string AutomationType
        {
            get { return (string)attributes["automation"]; }
            set { attributes["automation"] = value; }
        }

        /// <summary>
        /// Indicates if the automation has been deleted.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("deleted")]
        public bool Deleted
        {
            get { return attributes["deleted"] == null ? false : (bool)attributes["deleted"]; }
            set { attributes["deleted"] = value; }
        }

        /// <summary>
        /// Description for the this Automation.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; }
        }

        /// <summary>
        /// If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        /// </summary>
        [JsonPropertyName("destination_replace_from")]
        public string DestinationReplaceFrom
        {
            get { return (string)attributes["destination_replace_from"]; }
            set { attributes["destination_replace_from"] = value; }
        }

        /// <summary>
        /// If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        /// </summary>
        [JsonPropertyName("destination_replace_to")]
        public string DestinationReplaceTo
        {
            get { return (string)attributes["destination_replace_to"]; }
            set { attributes["destination_replace_to"] = value; }
        }

        /// <summary>
        /// Destination Paths
        /// </summary>
        [JsonPropertyName("destinations")]
        public string[] Destinations
        {
            get { return (string[])attributes["destinations"]; }
            set { attributes["destinations"] = value; }
        }

        /// <summary>
        /// If true, this automation will not run.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; }
        }

        /// <summary>
        /// If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        /// </summary>
        [JsonPropertyName("exclude_pattern")]
        public string ExcludePattern
        {
            get { return (string)attributes["exclude_pattern"]; }
            set { attributes["exclude_pattern"] = value; }
        }

        /// <summary>
        /// List of URLs to be imported and names to be used.
        /// </summary>
        [JsonPropertyName("import_urls")]
        public object[] ImportUrls
        {
            get { return (object[])attributes["import_urls"]; }
            set { attributes["import_urls"] = value; }
        }

        /// <summary>
        /// Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("flatten_destination_structure")]
        public bool FlattenDestinationStructure
        {
            get { return attributes["flatten_destination_structure"] == null ? false : (bool)attributes["flatten_destination_structure"]; }
            set { attributes["flatten_destination_structure"] = value; }
        }

        /// <summary>
        /// IDs of Groups for the Automation (i.e. who to Request File from)
        /// </summary>
        [JsonPropertyName("group_ids")]
        public Nullable<Int64>[] GroupIds
        {
            get { return (Nullable<Int64>[])attributes["group_ids"]; }
            set { attributes["group_ids"] = value; }
        }

        /// <summary>
        /// If true, the Lock Folders behavior will be disregarded for automated actions.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("ignore_locked_folders")]
        public bool IgnoreLockedFolders
        {
            get { return attributes["ignore_locked_folders"] == null ? false : (bool)attributes["ignore_locked_folders"]; }
            set { attributes["ignore_locked_folders"] = value; }
        }

        /// <summary>
        /// If trigger is `daily`, this specifies how often to run this automation.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval
        {
            get { return (string)attributes["interval"]; }
            set { attributes["interval"] = value; }
        }

        /// <summary>
        /// Time when automation was last modified. Does not change for name or description updates.
        /// </summary>
        [JsonPropertyName("last_modified_at")]
        public Nullable<DateTime> LastModifiedAt
        {
            get { return (Nullable<DateTime>)attributes["last_modified_at"]; }
            set { attributes["last_modified_at"] = value; }
        }

        /// <summary>
        /// If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("legacy_folder_matching")]
        public bool LegacyFolderMatching
        {
            get { return attributes["legacy_folder_matching"] == null ? false : (bool)attributes["legacy_folder_matching"]; }
            set { attributes["legacy_folder_matching"] = value; }
        }

        /// <summary>
        /// Name for this automation.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten if they appear to be the same file size as the newly incoming file.  Use the `:always_overwrite_size_matching_files` option to override this.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("overwrite_files")]
        public bool OverwriteFiles
        {
            get { return attributes["overwrite_files"] == null ? false : (bool)attributes["overwrite_files"]; }
            set { attributes["overwrite_files"] = value; }
        }

        /// <summary>
        /// Path on which this Automation runs.  Supports globs, except on remote mounts. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Timezone to use when rendering timestamps in paths.
        /// </summary>
        [JsonPropertyName("path_time_zone")]
        public string PathTimeZone
        {
            get { return (string)attributes["path_time_zone"]; }
            set { attributes["path_time_zone"] = value; }
        }

        /// <summary>
        /// If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        /// </summary>
        [JsonPropertyName("recurring_day")]
        public Nullable<Int64> RecurringDay
        {
            get { return (Nullable<Int64>)attributes["recurring_day"]; }
            set { attributes["recurring_day"] = value; }
        }

        /// <summary>
        /// If the Automation fails, retry at this interval (in minutes).
        /// </summary>
        [JsonPropertyName("retry_on_failure_interval_in_minutes")]
        public Nullable<Int64> RetryOnFailureIntervalInMinutes
        {
            get { return (Nullable<Int64>)attributes["retry_on_failure_interval_in_minutes"]; }
            set { attributes["retry_on_failure_interval_in_minutes"] = value; }
        }

        /// <summary>
        /// If the Automation fails, retry at most this many times.
        /// </summary>
        [JsonPropertyName("retry_on_failure_number_of_attempts")]
        public Nullable<Int64> RetryOnFailureNumberOfAttempts
        {
            get { return (Nullable<Int64>)attributes["retry_on_failure_number_of_attempts"]; }
            set { attributes["retry_on_failure_number_of_attempts"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule description for when the automation should be run in json format.
        /// </summary>
        [JsonPropertyName("schedule")]
        public object Schedule
        {
            get { return (object)attributes["schedule"]; }
            set { attributes["schedule"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Human readable Custom schedule description for when the automation should be run.
        /// </summary>
        [JsonPropertyName("human_readable_schedule")]
        public string HumanReadableSchedule
        {
            get { return (string)attributes["human_readable_schedule"]; }
            set { attributes["human_readable_schedule"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule description for when the automation should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        /// </summary>
        [JsonPropertyName("schedule_days_of_week")]
        public Nullable<Int64>[] ScheduleDaysOfWeek
        {
            get { return (Nullable<Int64>[])attributes["schedule_days_of_week"]; }
            set { attributes["schedule_days_of_week"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule description for when the automation should be run. Times of day in HH:MM format.
        /// </summary>
        [JsonPropertyName("schedule_times_of_day")]
        public string[] ScheduleTimesOfDay
        {
            get { return (string[])attributes["schedule_times_of_day"]; }
            set { attributes["schedule_times_of_day"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule Time Zone for when the automation should be run.
        /// </summary>
        [JsonPropertyName("schedule_time_zone")]
        public string ScheduleTimeZone
        {
            get { return (string)attributes["schedule_time_zone"]; }
            set { attributes["schedule_time_zone"] = value; }
        }

        /// <summary>
        /// Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source
        {
            get { return (string)attributes["source"]; }
            set { attributes["source"] = value; }
        }

        /// <summary>
        /// IDs of remote sync folder behaviors to run by this Automation
        /// </summary>
        [JsonPropertyName("sync_ids")]
        public Nullable<Int64>[] SyncIds
        {
            get { return (Nullable<Int64>[])attributes["sync_ids"]; }
            set { attributes["sync_ids"] = value; }
        }

        /// <summary>
        /// If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
        /// </summary>
        [JsonPropertyName("trigger_actions")]
        public string[] TriggerActions
        {
            get { return (string[])attributes["trigger_actions"]; }
            set { attributes["trigger_actions"] = value; }
        }

        /// <summary>
        /// How this automation is triggered to run.
        /// </summary>
        [JsonPropertyName("trigger")]
        public string Trigger
        {
            get { return (string)attributes["trigger"]; }
            set { attributes["trigger"] = value; }
        }

        /// <summary>
        /// User ID of the Automation's creator.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// IDs of Users for the Automation (i.e. who to Request File from)
        /// </summary>
        [JsonPropertyName("user_ids")]
        public Nullable<Int64>[] UserIds
        {
            get { return (Nullable<Int64>[])attributes["user_ids"]; }
            set { attributes["user_ids"] = value; }
        }

        /// <summary>
        /// A Hash of attributes specific to the automation type.
        /// </summary>
        [JsonPropertyName("value")]
        public object Value
        {
            get { return (object)attributes["value"]; }
            set { attributes["value"] = value; }
        }

        /// <summary>
        /// If trigger is `webhook`, this is the URL of the webhook to trigger the Automation.
        /// </summary>
        [JsonPropertyName("webhook_url")]
        public string WebhookUrl
        {
            get { return (string)attributes["webhook_url"]; }
            set { attributes["webhook_url"] = value; }
        }

        /// <summary>
        /// Manually Run Automation
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

            await FilesClient.SendRequest($"/automations/{System.Uri.EscapeDataString(attributes["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   source - string - Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        ///   destinations - array(string) - A list of String destination paths or Hash of folder_path and optional file_path.
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs, except on remote mounts.
        ///   sync_ids - string - A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`. A list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - If trigger is `custom_schedule`. A list of times of day to run this automation. 24-hour time format.
        ///   schedule_time_zone - string - If trigger is `custom_schedule`. Time zone for the schedule.
        ///   always_overwrite_size_matching_files - boolean - Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.
        ///   description - string - Description for the this Automation.
        ///   disabled - boolean - If true, this automation will not run.
        ///   exclude_pattern - string - If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        ///   import_urls - array(object) - List of URLs to be imported and names to be used.
        ///   flatten_destination_structure - boolean - Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        ///   ignore_locked_folders - boolean - If true, the Lock Folders behavior will be disregarded for automated actions.
        ///   legacy_folder_matching - boolean - DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        ///   name - string - Name for this automation.
        ///   overwrite_files - boolean - If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten if they appear to be the same file size as the newly incoming file.  Use the `:always_overwrite_size_matching_files` option to override this.
        ///   path_time_zone - string - Timezone to use when rendering timestamps in paths.
        ///   retry_on_failure_interval_in_minutes - int64 - If the Automation fails, retry at this interval (in minutes).
        ///   retry_on_failure_number_of_attempts - int64 - If the Automation fails, retry at most this many times.
        ///   trigger - string - How this automation is triggered to run.
        ///   trigger_actions - array(string) - If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
        ///   value - object - A Hash of attributes specific to the automation type.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   automation - string - Automation type
        /// </summary>
        public async Task<Automation> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("destinations") && !(parameters["destinations"] is string[]))
            {
                throw new ArgumentException("Bad parameter: destinations must be of type string[]", "parameters[\"destinations\"]");
            }
            if (parameters.ContainsKey("destination_replace_from") && !(parameters["destination_replace_from"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_replace_from must be of type string", "parameters[\"destination_replace_from\"]");
            }
            if (parameters.ContainsKey("destination_replace_to") && !(parameters["destination_replace_to"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_replace_to must be of type string", "parameters[\"destination_replace_to\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("sync_ids") && !(parameters["sync_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: sync_ids must be of type string", "parameters[\"sync_ids\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
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
            if (parameters.ContainsKey("always_overwrite_size_matching_files") && !(parameters["always_overwrite_size_matching_files"] is bool))
            {
                throw new ArgumentException("Bad parameter: always_overwrite_size_matching_files must be of type bool", "parameters[\"always_overwrite_size_matching_files\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("exclude_pattern") && !(parameters["exclude_pattern"] is string))
            {
                throw new ArgumentException("Bad parameter: exclude_pattern must be of type string", "parameters[\"exclude_pattern\"]");
            }
            if (parameters.ContainsKey("import_urls") && !(parameters["import_urls"] is object[]))
            {
                throw new ArgumentException("Bad parameter: import_urls must be of type object[]", "parameters[\"import_urls\"]");
            }
            if (parameters.ContainsKey("flatten_destination_structure") && !(parameters["flatten_destination_structure"] is bool))
            {
                throw new ArgumentException("Bad parameter: flatten_destination_structure must be of type bool", "parameters[\"flatten_destination_structure\"]");
            }
            if (parameters.ContainsKey("ignore_locked_folders") && !(parameters["ignore_locked_folders"] is bool))
            {
                throw new ArgumentException("Bad parameter: ignore_locked_folders must be of type bool", "parameters[\"ignore_locked_folders\"]");
            }
            if (parameters.ContainsKey("legacy_folder_matching") && !(parameters["legacy_folder_matching"] is bool))
            {
                throw new ArgumentException("Bad parameter: legacy_folder_matching must be of type bool", "parameters[\"legacy_folder_matching\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("overwrite_files") && !(parameters["overwrite_files"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite_files must be of type bool", "parameters[\"overwrite_files\"]");
            }
            if (parameters.ContainsKey("path_time_zone") && !(parameters["path_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: path_time_zone must be of type string", "parameters[\"path_time_zone\"]");
            }
            if (parameters.ContainsKey("retry_on_failure_interval_in_minutes") && !(parameters["retry_on_failure_interval_in_minutes"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: retry_on_failure_interval_in_minutes must be of type Nullable<Int64>", "parameters[\"retry_on_failure_interval_in_minutes\"]");
            }
            if (parameters.ContainsKey("retry_on_failure_number_of_attempts") && !(parameters["retry_on_failure_number_of_attempts"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: retry_on_failure_number_of_attempts must be of type Nullable<Int64>", "parameters[\"retry_on_failure_number_of_attempts\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_actions") && !(parameters["trigger_actions"] is string[]))
            {
                throw new ArgumentException("Bad parameter: trigger_actions must be of type string[]", "parameters[\"trigger_actions\"]");
            }
            if (parameters.ContainsKey("value") && !(parameters["value"] is object))
            {
                throw new ArgumentException("Bad parameter: value must be of type object", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("automation") && !(parameters["automation"] is string))
            {
                throw new ArgumentException("Bad parameter: automation must be of type string", "parameters[\"automation\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/automations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Automation>(responseJson);
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

            await FilesClient.SendRequest($"/automations/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `automation`, `disabled`, `last_modified_at` or `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled`, `last_modified_at` or `automation`. Valid field combinations are `[ disabled, last_modified_at ]`, `[ automation, disabled ]`, `[ automation, last_modified_at ]` or `[ automation, disabled, last_modified_at ]`.
        ///   filter_gt - object - If set, return records where the specified field is greater than the supplied value. Valid fields are `last_modified_at`.
        ///   filter_gteq - object - If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `last_modified_at`.
        ///   filter_lt - object - If set, return records where the specified field is less than the supplied value. Valid fields are `last_modified_at`.
        ///   filter_lteq - object - If set, return records where the specified field is less than or equal the supplied value. Valid fields are `last_modified_at`.
        /// </summary>
        public static FilesList<Automation> List(

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
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }

            return new FilesList<Automation>($"/automations", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Automation> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
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

            string responseJson = await FilesClient.SendStringRequest($"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Automation>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
        ///   source - string - Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        ///   destinations - array(string) - A list of String destination paths or Hash of folder_path and optional file_path.
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs, except on remote mounts.
        ///   sync_ids - string - A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`. A list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - If trigger is `custom_schedule`. A list of times of day to run this automation. 24-hour time format.
        ///   schedule_time_zone - string - If trigger is `custom_schedule`. Time zone for the schedule.
        ///   always_overwrite_size_matching_files - boolean - Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.
        ///   description - string - Description for the this Automation.
        ///   disabled - boolean - If true, this automation will not run.
        ///   exclude_pattern - string - If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        ///   import_urls - array(object) - List of URLs to be imported and names to be used.
        ///   flatten_destination_structure - boolean - Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        ///   ignore_locked_folders - boolean - If true, the Lock Folders behavior will be disregarded for automated actions.
        ///   legacy_folder_matching - boolean - DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        ///   name - string - Name for this automation.
        ///   overwrite_files - boolean - If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten if they appear to be the same file size as the newly incoming file.  Use the `:always_overwrite_size_matching_files` option to override this.
        ///   path_time_zone - string - Timezone to use when rendering timestamps in paths.
        ///   retry_on_failure_interval_in_minutes - int64 - If the Automation fails, retry at this interval (in minutes).
        ///   retry_on_failure_number_of_attempts - int64 - If the Automation fails, retry at most this many times.
        ///   trigger - string - How this automation is triggered to run.
        ///   trigger_actions - array(string) - If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
        ///   value - object - A Hash of attributes specific to the automation type.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   automation (required) - string - Automation type
        /// </summary>
        public static async Task<Automation> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("automation") || parameters["automation"] == null)
            {
                throw new ArgumentNullException("Parameter missing: automation", "parameters[\"automation\"]");
            }
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("destinations") && !(parameters["destinations"] is string[]))
            {
                throw new ArgumentException("Bad parameter: destinations must be of type string[]", "parameters[\"destinations\"]");
            }
            if (parameters.ContainsKey("destination_replace_from") && !(parameters["destination_replace_from"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_replace_from must be of type string", "parameters[\"destination_replace_from\"]");
            }
            if (parameters.ContainsKey("destination_replace_to") && !(parameters["destination_replace_to"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_replace_to must be of type string", "parameters[\"destination_replace_to\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("sync_ids") && !(parameters["sync_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: sync_ids must be of type string", "parameters[\"sync_ids\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
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
            if (parameters.ContainsKey("always_overwrite_size_matching_files") && !(parameters["always_overwrite_size_matching_files"] is bool))
            {
                throw new ArgumentException("Bad parameter: always_overwrite_size_matching_files must be of type bool", "parameters[\"always_overwrite_size_matching_files\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("exclude_pattern") && !(parameters["exclude_pattern"] is string))
            {
                throw new ArgumentException("Bad parameter: exclude_pattern must be of type string", "parameters[\"exclude_pattern\"]");
            }
            if (parameters.ContainsKey("import_urls") && !(parameters["import_urls"] is object[]))
            {
                throw new ArgumentException("Bad parameter: import_urls must be of type object[]", "parameters[\"import_urls\"]");
            }
            if (parameters.ContainsKey("flatten_destination_structure") && !(parameters["flatten_destination_structure"] is bool))
            {
                throw new ArgumentException("Bad parameter: flatten_destination_structure must be of type bool", "parameters[\"flatten_destination_structure\"]");
            }
            if (parameters.ContainsKey("ignore_locked_folders") && !(parameters["ignore_locked_folders"] is bool))
            {
                throw new ArgumentException("Bad parameter: ignore_locked_folders must be of type bool", "parameters[\"ignore_locked_folders\"]");
            }
            if (parameters.ContainsKey("legacy_folder_matching") && !(parameters["legacy_folder_matching"] is bool))
            {
                throw new ArgumentException("Bad parameter: legacy_folder_matching must be of type bool", "parameters[\"legacy_folder_matching\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("overwrite_files") && !(parameters["overwrite_files"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite_files must be of type bool", "parameters[\"overwrite_files\"]");
            }
            if (parameters.ContainsKey("path_time_zone") && !(parameters["path_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: path_time_zone must be of type string", "parameters[\"path_time_zone\"]");
            }
            if (parameters.ContainsKey("retry_on_failure_interval_in_minutes") && !(parameters["retry_on_failure_interval_in_minutes"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: retry_on_failure_interval_in_minutes must be of type Nullable<Int64>", "parameters[\"retry_on_failure_interval_in_minutes\"]");
            }
            if (parameters.ContainsKey("retry_on_failure_number_of_attempts") && !(parameters["retry_on_failure_number_of_attempts"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: retry_on_failure_number_of_attempts must be of type Nullable<Int64>", "parameters[\"retry_on_failure_number_of_attempts\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_actions") && !(parameters["trigger_actions"] is string[]))
            {
                throw new ArgumentException("Bad parameter: trigger_actions must be of type string[]", "parameters[\"trigger_actions\"]");
            }
            if (parameters.ContainsKey("value") && !(parameters["value"] is object))
            {
                throw new ArgumentException("Bad parameter: value must be of type object", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("automation") && !(parameters["automation"] is string))
            {
                throw new ArgumentException("Bad parameter: automation must be of type string", "parameters[\"automation\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/automations", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Automation>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Manually Run Automation
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

            await FilesClient.SendRequest($"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   source - string - Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
        ///   destinations - array(string) - A list of String destination paths or Hash of folder_path and optional file_path.
        ///   destination_replace_from - string - If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
        ///   destination_replace_to - string - If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
        ///   interval - string - How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   path - string - Path on which this Automation runs.  Supports globs, except on remote mounts.
        ///   sync_ids - string - A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   user_ids - string - A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   group_ids - string - A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`. A list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - If trigger is `custom_schedule`. A list of times of day to run this automation. 24-hour time format.
        ///   schedule_time_zone - string - If trigger is `custom_schedule`. Time zone for the schedule.
        ///   always_overwrite_size_matching_files - boolean - Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.
        ///   description - string - Description for the this Automation.
        ///   disabled - boolean - If true, this automation will not run.
        ///   exclude_pattern - string - If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
        ///   import_urls - array(object) - List of URLs to be imported and names to be used.
        ///   flatten_destination_structure - boolean - Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
        ///   ignore_locked_folders - boolean - If true, the Lock Folders behavior will be disregarded for automated actions.
        ///   legacy_folder_matching - boolean - DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
        ///   name - string - Name for this automation.
        ///   overwrite_files - boolean - If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten if they appear to be the same file size as the newly incoming file.  Use the `:always_overwrite_size_matching_files` option to override this.
        ///   path_time_zone - string - Timezone to use when rendering timestamps in paths.
        ///   retry_on_failure_interval_in_minutes - int64 - If the Automation fails, retry at this interval (in minutes).
        ///   retry_on_failure_number_of_attempts - int64 - If the Automation fails, retry at most this many times.
        ///   trigger - string - How this automation is triggered to run.
        ///   trigger_actions - array(string) - If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
        ///   value - object - A Hash of attributes specific to the automation type.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   automation - string - Automation type
        /// </summary>
        public static async Task<Automation> Update(
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
            if (parameters.ContainsKey("source") && !(parameters["source"] is string))
            {
                throw new ArgumentException("Bad parameter: source must be of type string", "parameters[\"source\"]");
            }
            if (parameters.ContainsKey("destinations") && !(parameters["destinations"] is string[]))
            {
                throw new ArgumentException("Bad parameter: destinations must be of type string[]", "parameters[\"destinations\"]");
            }
            if (parameters.ContainsKey("destination_replace_from") && !(parameters["destination_replace_from"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_replace_from must be of type string", "parameters[\"destination_replace_from\"]");
            }
            if (parameters.ContainsKey("destination_replace_to") && !(parameters["destination_replace_to"] is string))
            {
                throw new ArgumentException("Bad parameter: destination_replace_to must be of type string", "parameters[\"destination_replace_to\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("sync_ids") && !(parameters["sync_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: sync_ids must be of type string", "parameters[\"sync_ids\"]");
            }
            if (parameters.ContainsKey("user_ids") && !(parameters["user_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: user_ids must be of type string", "parameters[\"user_ids\"]");
            }
            if (parameters.ContainsKey("group_ids") && !(parameters["group_ids"] is string))
            {
                throw new ArgumentException("Bad parameter: group_ids must be of type string", "parameters[\"group_ids\"]");
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
            if (parameters.ContainsKey("always_overwrite_size_matching_files") && !(parameters["always_overwrite_size_matching_files"] is bool))
            {
                throw new ArgumentException("Bad parameter: always_overwrite_size_matching_files must be of type bool", "parameters[\"always_overwrite_size_matching_files\"]");
            }
            if (parameters.ContainsKey("description") && !(parameters["description"] is string))
            {
                throw new ArgumentException("Bad parameter: description must be of type string", "parameters[\"description\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("exclude_pattern") && !(parameters["exclude_pattern"] is string))
            {
                throw new ArgumentException("Bad parameter: exclude_pattern must be of type string", "parameters[\"exclude_pattern\"]");
            }
            if (parameters.ContainsKey("import_urls") && !(parameters["import_urls"] is object[]))
            {
                throw new ArgumentException("Bad parameter: import_urls must be of type object[]", "parameters[\"import_urls\"]");
            }
            if (parameters.ContainsKey("flatten_destination_structure") && !(parameters["flatten_destination_structure"] is bool))
            {
                throw new ArgumentException("Bad parameter: flatten_destination_structure must be of type bool", "parameters[\"flatten_destination_structure\"]");
            }
            if (parameters.ContainsKey("ignore_locked_folders") && !(parameters["ignore_locked_folders"] is bool))
            {
                throw new ArgumentException("Bad parameter: ignore_locked_folders must be of type bool", "parameters[\"ignore_locked_folders\"]");
            }
            if (parameters.ContainsKey("legacy_folder_matching") && !(parameters["legacy_folder_matching"] is bool))
            {
                throw new ArgumentException("Bad parameter: legacy_folder_matching must be of type bool", "parameters[\"legacy_folder_matching\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("overwrite_files") && !(parameters["overwrite_files"] is bool))
            {
                throw new ArgumentException("Bad parameter: overwrite_files must be of type bool", "parameters[\"overwrite_files\"]");
            }
            if (parameters.ContainsKey("path_time_zone") && !(parameters["path_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: path_time_zone must be of type string", "parameters[\"path_time_zone\"]");
            }
            if (parameters.ContainsKey("retry_on_failure_interval_in_minutes") && !(parameters["retry_on_failure_interval_in_minutes"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: retry_on_failure_interval_in_minutes must be of type Nullable<Int64>", "parameters[\"retry_on_failure_interval_in_minutes\"]");
            }
            if (parameters.ContainsKey("retry_on_failure_number_of_attempts") && !(parameters["retry_on_failure_number_of_attempts"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: retry_on_failure_number_of_attempts must be of type Nullable<Int64>", "parameters[\"retry_on_failure_number_of_attempts\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_actions") && !(parameters["trigger_actions"] is string[]))
            {
                throw new ArgumentException("Bad parameter: trigger_actions must be of type string[]", "parameters[\"trigger_actions\"]");
            }
            if (parameters.ContainsKey("value") && !(parameters["value"] is object))
            {
                throw new ArgumentException("Bad parameter: value must be of type object", "parameters[\"value\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("automation") && !(parameters["automation"] is string))
            {
                throw new ArgumentException("Bad parameter: automation must be of type string", "parameters[\"automation\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Automation>(responseJson);
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

            await FilesClient.SendRequest($"/automations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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