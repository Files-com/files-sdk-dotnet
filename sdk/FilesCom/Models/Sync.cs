using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Sync
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Sync() : this(null, null) { }

        public Sync(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
        /// Sync ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Name for this sync job
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Description for this sync job
        /// </summary>
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            set { attributes["description"] = value; }
        }

        /// <summary>
        /// Site ID this sync belongs to
        /// </summary>
        [JsonPropertyName("site_id")]
        public Nullable<Int64> SiteId
        {
            get { return (Nullable<Int64>)attributes["site_id"]; }
            set { attributes["site_id"] = value; }
        }

        /// <summary>
        /// User who created or owns this sync
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Absolute source path for the sync
        /// </summary>
        [JsonPropertyName("src_path")]
        public string SrcPath
        {
            get { return (string)attributes["src_path"]; }
            set { attributes["src_path"] = value; }
        }

        /// <summary>
        /// Absolute destination path for the sync
        /// </summary>
        [JsonPropertyName("dest_path")]
        public string DestPath
        {
            get { return (string)attributes["dest_path"]; }
            set { attributes["dest_path"] = value; }
        }

        /// <summary>
        /// Remote server ID for the source (if remote)
        /// </summary>
        [JsonPropertyName("src_remote_server_id")]
        public Nullable<Int64> SrcRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["src_remote_server_id"]; }
            set { attributes["src_remote_server_id"] = value; }
        }

        /// <summary>
        /// Remote server ID for the destination (if remote)
        /// </summary>
        [JsonPropertyName("dest_remote_server_id")]
        public Nullable<Int64> DestRemoteServerId
        {
            get { return (Nullable<Int64>)attributes["dest_remote_server_id"]; }
            set { attributes["dest_remote_server_id"] = value; }
        }

        /// <summary>
        /// Is this a two-way sync?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("two_way")]
        public bool TwoWay
        {
            get { return attributes["two_way"] == null ? false : (bool)attributes["two_way"]; }
            set { attributes["two_way"] = value; }
        }

        /// <summary>
        /// Keep files after copying?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("keep_after_copy")]
        public bool KeepAfterCopy
        {
            get { return attributes["keep_after_copy"] == null ? false : (bool)attributes["keep_after_copy"]; }
            set { attributes["keep_after_copy"] = value; }
        }

        /// <summary>
        /// Delete empty folders after sync?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("delete_empty_folders")]
        public bool DeleteEmptyFolders
        {
            get { return attributes["delete_empty_folders"] == null ? false : (bool)attributes["delete_empty_folders"]; }
            set { attributes["delete_empty_folders"] = value; }
        }

        /// <summary>
        /// Is this sync disabled?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("disabled")]
        public bool Disabled
        {
            get { return attributes["disabled"] == null ? false : (bool)attributes["disabled"]; }
            set { attributes["disabled"] = value; }
        }

        /// <summary>
        /// Trigger type: daily, custom_schedule, or manual
        /// </summary>
        [JsonPropertyName("trigger")]
        public string Trigger
        {
            get { return (string)attributes["trigger"]; }
            set { attributes["trigger"] = value; }
        }

        /// <summary>
        /// Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        /// </summary>
        [JsonPropertyName("trigger_file")]
        public string TriggerFile
        {
            get { return (string)attributes["trigger_file"]; }
            set { attributes["trigger_file"] = value; }
        }

        /// <summary>
        /// Array of glob patterns to include
        /// </summary>
        [JsonPropertyName("include_patterns")]
        public string[] IncludePatterns
        {
            get { return (string[])attributes["include_patterns"]; }
            set { attributes["include_patterns"] = value; }
        }

        /// <summary>
        /// Array of glob patterns to exclude
        /// </summary>
        [JsonPropertyName("exclude_patterns")]
        public string[] ExcludePatterns
        {
            get { return (string[])attributes["exclude_patterns"]; }
            set { attributes["exclude_patterns"] = value; }
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
            set { attributes["sync_interval_minutes"] = value; }
        }

        /// <summary>
        /// If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        /// </summary>
        [JsonPropertyName("interval")]
        public string Interval
        {
            get { return (string)attributes["interval"]; }
            set { attributes["interval"] = value; }
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
        /// If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        /// </summary>
        [JsonPropertyName("schedule_days_of_week")]
        public Nullable<Int64>[] ScheduleDaysOfWeek
        {
            get { return (Nullable<Int64>[])attributes["schedule_days_of_week"]; }
            set { attributes["schedule_days_of_week"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. Times of day in HH:MM format.
        /// </summary>
        [JsonPropertyName("schedule_times_of_day")]
        public string[] ScheduleTimesOfDay
        {
            get { return (string[])attributes["schedule_times_of_day"]; }
            set { attributes["schedule_times_of_day"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, Custom schedule Time Zone for when the sync should be run.
        /// </summary>
        [JsonPropertyName("schedule_time_zone")]
        public string ScheduleTimeZone
        {
            get { return (string)attributes["schedule_time_zone"]; }
            set { attributes["schedule_time_zone"] = value; }
        }

        /// <summary>
        /// If trigger is `custom_schedule`, the sync will check if there is a formal, observed holiday for the region, and if so, it will not run.
        /// </summary>
        [JsonPropertyName("holiday_region")]
        public string HolidayRegion
        {
            get { return (string)attributes["holiday_region"]; }
            set { attributes["holiday_region"] = value; }
        }

        /// <summary>
        /// The latest run of this sync
        /// </summary>
        [JsonPropertyName("latest_sync_run")]
        public SyncRun LatestSyncRun
        {
            get { return (SyncRun)attributes["latest_sync_run"]; }
            set { attributes["latest_sync_run"] = value; }
        }

        /// <summary>
        /// Dry Run Sync
        /// </summary>
        public async Task DryRun(Dictionary<string, object> parameters)
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

            await FilesClient.SendRequest($"/syncs/{System.Uri.EscapeDataString(attributes["id"].ToString())}/dry_run", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Manually Run Sync
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

            await FilesClient.SendRequest($"/syncs/{System.Uri.EscapeDataString(attributes["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Name for this sync job
        ///   description - string - Description for this sync job
        ///   src_path - string - Absolute source path
        ///   dest_path - string - Absolute destination path
        ///   src_remote_server_id - int64 - Remote server ID for the source
        ///   dest_remote_server_id - int64 - Remote server ID for the destination
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   disabled - boolean - Is this sync disabled?
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   holiday_region - string - If trigger is `custom_schedule`, the sync will check if there is a formal, observed holiday for the region, and if so, it will not run.
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_time_zone - string - If trigger is `custom_schedule`, Custom schedule Time Zone for when the sync should be run.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. Times of day in HH:MM format.
        /// </summary>
        public async Task<Sync> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("src_path") && !(parameters["src_path"] is string))
            {
                throw new ArgumentException("Bad parameter: src_path must be of type string", "parameters[\"src_path\"]");
            }
            if (parameters.ContainsKey("dest_path") && !(parameters["dest_path"] is string))
            {
                throw new ArgumentException("Bad parameter: dest_path must be of type string", "parameters[\"dest_path\"]");
            }
            if (parameters.ContainsKey("src_remote_server_id") && !(parameters["src_remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: src_remote_server_id must be of type Nullable<Int64>", "parameters[\"src_remote_server_id\"]");
            }
            if (parameters.ContainsKey("dest_remote_server_id") && !(parameters["dest_remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: dest_remote_server_id must be of type Nullable<Int64>", "parameters[\"dest_remote_server_id\"]");
            }
            if (parameters.ContainsKey("keep_after_copy") && !(parameters["keep_after_copy"] is bool))
            {
                throw new ArgumentException("Bad parameter: keep_after_copy must be of type bool", "parameters[\"keep_after_copy\"]");
            }
            if (parameters.ContainsKey("delete_empty_folders") && !(parameters["delete_empty_folders"] is bool))
            {
                throw new ArgumentException("Bad parameter: delete_empty_folders must be of type bool", "parameters[\"delete_empty_folders\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_file") && !(parameters["trigger_file"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger_file must be of type string", "parameters[\"trigger_file\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("sync_interval_minutes") && !(parameters["sync_interval_minutes"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: sync_interval_minutes must be of type Nullable<Int64>", "parameters[\"sync_interval_minutes\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/syncs/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Sync>(responseJson);
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

            await FilesClient.SendRequest($"/syncs/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await Sync.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<Sync> List(

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

            return new FilesList<Sync>($"/syncs", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Sync> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Sync ID.
        /// </summary>
        public static async Task<Sync> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Sync>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<Sync> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name - string - Name for this sync job
        ///   description - string - Description for this sync job
        ///   src_path - string - Absolute source path
        ///   dest_path - string - Absolute destination path
        ///   src_remote_server_id - int64 - Remote server ID for the source
        ///   dest_remote_server_id - int64 - Remote server ID for the destination
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   disabled - boolean - Is this sync disabled?
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   holiday_region - string - If trigger is `custom_schedule`, the sync will check if there is a formal, observed holiday for the region, and if so, it will not run.
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_time_zone - string - If trigger is `custom_schedule`, Custom schedule Time Zone for when the sync should be run.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. Times of day in HH:MM format.
        /// </summary>
        public static async Task<Sync> Create(

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
            if (parameters.ContainsKey("src_path") && !(parameters["src_path"] is string))
            {
                throw new ArgumentException("Bad parameter: src_path must be of type string", "parameters[\"src_path\"]");
            }
            if (parameters.ContainsKey("dest_path") && !(parameters["dest_path"] is string))
            {
                throw new ArgumentException("Bad parameter: dest_path must be of type string", "parameters[\"dest_path\"]");
            }
            if (parameters.ContainsKey("src_remote_server_id") && !(parameters["src_remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: src_remote_server_id must be of type Nullable<Int64>", "parameters[\"src_remote_server_id\"]");
            }
            if (parameters.ContainsKey("dest_remote_server_id") && !(parameters["dest_remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: dest_remote_server_id must be of type Nullable<Int64>", "parameters[\"dest_remote_server_id\"]");
            }
            if (parameters.ContainsKey("keep_after_copy") && !(parameters["keep_after_copy"] is bool))
            {
                throw new ArgumentException("Bad parameter: keep_after_copy must be of type bool", "parameters[\"keep_after_copy\"]");
            }
            if (parameters.ContainsKey("delete_empty_folders") && !(parameters["delete_empty_folders"] is bool))
            {
                throw new ArgumentException("Bad parameter: delete_empty_folders must be of type bool", "parameters[\"delete_empty_folders\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_file") && !(parameters["trigger_file"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger_file must be of type string", "parameters[\"trigger_file\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("sync_interval_minutes") && !(parameters["sync_interval_minutes"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: sync_interval_minutes must be of type Nullable<Int64>", "parameters[\"sync_interval_minutes\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/syncs", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Sync>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Dry Run Sync
        /// </summary>
        public static async Task DryRun(
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

            await FilesClient.SendRequest($"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}/dry_run", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Manually Run Sync
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

            await FilesClient.SendRequest($"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}/manual_run", System.Net.Http.HttpMethod.Post, parameters, options);
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Name for this sync job
        ///   description - string - Description for this sync job
        ///   src_path - string - Absolute source path
        ///   dest_path - string - Absolute destination path
        ///   src_remote_server_id - int64 - Remote server ID for the source
        ///   dest_remote_server_id - int64 - Remote server ID for the destination
        ///   keep_after_copy - boolean - Keep files after copying?
        ///   delete_empty_folders - boolean - Delete empty folders after sync?
        ///   disabled - boolean - Is this sync disabled?
        ///   interval - string - If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
        ///   trigger - string - Trigger type: daily, custom_schedule, or manual
        ///   trigger_file - string - Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
        ///   holiday_region - string - If trigger is `custom_schedule`, the sync will check if there is a formal, observed holiday for the region, and if so, it will not run.
        ///   sync_interval_minutes - int64 - Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
        ///   recurring_day - int64 - If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
        ///   schedule_time_zone - string - If trigger is `custom_schedule`, Custom schedule Time Zone for when the sync should be run.
        ///   schedule_days_of_week - array(int64) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
        ///   schedule_times_of_day - array(string) - If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. Times of day in HH:MM format.
        /// </summary>
        public static async Task<Sync> Update(
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
            if (parameters.ContainsKey("src_path") && !(parameters["src_path"] is string))
            {
                throw new ArgumentException("Bad parameter: src_path must be of type string", "parameters[\"src_path\"]");
            }
            if (parameters.ContainsKey("dest_path") && !(parameters["dest_path"] is string))
            {
                throw new ArgumentException("Bad parameter: dest_path must be of type string", "parameters[\"dest_path\"]");
            }
            if (parameters.ContainsKey("src_remote_server_id") && !(parameters["src_remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: src_remote_server_id must be of type Nullable<Int64>", "parameters[\"src_remote_server_id\"]");
            }
            if (parameters.ContainsKey("dest_remote_server_id") && !(parameters["dest_remote_server_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: dest_remote_server_id must be of type Nullable<Int64>", "parameters[\"dest_remote_server_id\"]");
            }
            if (parameters.ContainsKey("keep_after_copy") && !(parameters["keep_after_copy"] is bool))
            {
                throw new ArgumentException("Bad parameter: keep_after_copy must be of type bool", "parameters[\"keep_after_copy\"]");
            }
            if (parameters.ContainsKey("delete_empty_folders") && !(parameters["delete_empty_folders"] is bool))
            {
                throw new ArgumentException("Bad parameter: delete_empty_folders must be of type bool", "parameters[\"delete_empty_folders\"]");
            }
            if (parameters.ContainsKey("disabled") && !(parameters["disabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: disabled must be of type bool", "parameters[\"disabled\"]");
            }
            if (parameters.ContainsKey("interval") && !(parameters["interval"] is string))
            {
                throw new ArgumentException("Bad parameter: interval must be of type string", "parameters[\"interval\"]");
            }
            if (parameters.ContainsKey("trigger") && !(parameters["trigger"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger must be of type string", "parameters[\"trigger\"]");
            }
            if (parameters.ContainsKey("trigger_file") && !(parameters["trigger_file"] is string))
            {
                throw new ArgumentException("Bad parameter: trigger_file must be of type string", "parameters[\"trigger_file\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }
            if (parameters.ContainsKey("sync_interval_minutes") && !(parameters["sync_interval_minutes"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: sync_interval_minutes must be of type Nullable<Int64>", "parameters[\"sync_interval_minutes\"]");
            }
            if (parameters.ContainsKey("recurring_day") && !(parameters["recurring_day"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: recurring_day must be of type Nullable<Int64>", "parameters[\"recurring_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Sync>(responseJson);
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

            await FilesClient.SendRequest($"/syncs/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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