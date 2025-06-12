# FilesCom.Models.Sync

## Example Sync Object

```
{
  "id": 1,
  "name": "example",
  "description": "example",
  "site_id": 1,
  "user_id": 1,
  "src_path": "example",
  "dest_path": "example",
  "src_remote_server_id": 1,
  "dest_remote_server_id": 1,
  "two_way": true,
  "keep_after_copy": true,
  "delete_empty_folders": true,
  "disabled": true,
  "trigger": "example",
  "trigger_file": "example",
  "include_patterns": [
    "example"
  ],
  "exclude_patterns": [
    "example"
  ],
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z",
  "sync_interval_minutes": 1,
  "interval": "week",
  "recurring_day": 25,
  "schedule_days_of_week": [
    0,
    2,
    4
  ],
  "schedule_times_of_day": [
    "06:30",
    "14:30"
  ],
  "schedule_time_zone": "Eastern Time (US & Canada)"
}
```

* `id` / `Id`  (Nullable<Int64>): Sync ID
* `name` / `Name`  (string): Name for this sync job
* `description` / `Description`  (string): Description for this sync job
* `site_id` / `SiteId`  (Nullable<Int64>): Site ID this sync belongs to
* `user_id` / `UserId`  (Nullable<Int64>): User who created or owns this sync
* `src_path` / `SrcPath`  (string): Absolute source path for the sync
* `dest_path` / `DestPath`  (string): Absolute destination path for the sync
* `src_remote_server_id` / `SrcRemoteServerId`  (Nullable<Int64>): Remote server ID for the source (if remote)
* `dest_remote_server_id` / `DestRemoteServerId`  (Nullable<Int64>): Remote server ID for the destination (if remote)
* `two_way` / `TwoWay`  (bool): Is this a two-way sync?
* `keep_after_copy` / `KeepAfterCopy`  (bool): Keep files after copying?
* `delete_empty_folders` / `DeleteEmptyFolders`  (bool): Delete empty folders after sync?
* `disabled` / `Disabled`  (bool): Is this sync disabled?
* `trigger` / `Trigger`  (string): Trigger type: daily, custom_schedule, or manual
* `trigger_file` / `TriggerFile`  (string): Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
* `include_patterns` / `IncludePatterns`  (string[]): Array of glob patterns to include
* `exclude_patterns` / `ExcludePatterns`  (string[]): Array of glob patterns to exclude
* `created_at` / `CreatedAt`  (Nullable<DateTime>): When this sync was created
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): When this sync was last updated
* `sync_interval_minutes` / `SyncIntervalMinutes`  (Nullable<Int64>): Frequency in minutes between syncs. If set, this value must be greater than or equal to the `remote_sync_interval` value for the site's plan. If left blank, the plan's `remote_sync_interval` will be used. This setting is only used if `trigger` is empty.
* `interval` / `Interval`  (string): If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `recurring_day` / `RecurringDay`  (Nullable<Int64>): If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
* `schedule_days_of_week` / `ScheduleDaysOfWeek`  (Nullable<Int64>[]): If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
* `schedule_times_of_day` / `ScheduleTimesOfDay`  (string[]): If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. Times of day in HH:MM format.
* `schedule_time_zone` / `ScheduleTimeZone`  (string): If trigger is `custom_schedule`, Custom schedule Time Zone for when the sync should be run.


---

## List Syncs

```
Task<FilesList<Sync>> Sync.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Sync

```
Task<Sync> Sync.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sync ID.


---

## Create Sync

```
Task<Sync> Sync.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Name for this sync job
* `description` (string): Description for this sync job
* `src_path` (string): Absolute source path
* `dest_path` (string): Absolute destination path
* `src_remote_server_id` (Nullable<Int64>): Remote server ID for the source
* `dest_remote_server_id` (Nullable<Int64>): Remote server ID for the destination
* `two_way` (bool): Is this a two-way sync?
* `keep_after_copy` (bool): Keep files after copying?
* `delete_empty_folders` (bool): Delete empty folders after sync?
* `disabled` (bool): Is this sync disabled?
* `interval` (string): If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `trigger` (string): Trigger type: daily, custom_schedule, or manual
* `trigger_file` (string): Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
* `recurring_day` (Nullable<Int64>): If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
* `schedule_time_zone` (string): If trigger is `custom_schedule`, Custom schedule Time Zone for when the sync should be run.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
* `schedule_times_of_day` (string[]): If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. Times of day in HH:MM format.


---

## Migrate Legacy Syncs to Syncs

```
Task Sync.CreateMigrateTo(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```


---

## Update Sync

```
Task<Sync> Sync.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sync ID.
* `name` (string): Name for this sync job
* `description` (string): Description for this sync job
* `src_path` (string): Absolute source path
* `dest_path` (string): Absolute destination path
* `src_remote_server_id` (Nullable<Int64>): Remote server ID for the source
* `dest_remote_server_id` (Nullable<Int64>): Remote server ID for the destination
* `two_way` (bool): Is this a two-way sync?
* `keep_after_copy` (bool): Keep files after copying?
* `delete_empty_folders` (bool): Delete empty folders after sync?
* `disabled` (bool): Is this sync disabled?
* `interval` (string): If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `trigger` (string): Trigger type: daily, custom_schedule, or manual
* `trigger_file` (string): Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
* `recurring_day` (Nullable<Int64>): If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
* `schedule_time_zone` (string): If trigger is `custom_schedule`, Custom schedule Time Zone for when the sync should be run.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
* `schedule_times_of_day` (string[]): If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. Times of day in HH:MM format.


---

## Delete Sync

```
Task Sync.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sync ID.


---

## Update Sync

```
var Sync = Sync.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "example");
parameters.Add("description", "example");
parameters.Add("src_path", "example");
parameters.Add("dest_path", "example");
parameters.Add("src_remote_server_id", 1);
parameters.Add("dest_remote_server_id", 1);
parameters.Add("two_way", false);
parameters.Add("keep_after_copy", false);
parameters.Add("delete_empty_folders", false);
parameters.Add("disabled", false);
parameters.Add("interval", "week");
parameters.Add("trigger", "example");
parameters.Add("trigger_file", "example");
parameters.Add("recurring_day", 25);
parameters.Add("schedule_time_zone", "Eastern Time (US & Canada)");
parameters.Add("schedule_days_of_week", [0,2,4]);
parameters.Add("schedule_times_of_day", ["06:30","14:30"]);

Sync.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Sync ID.
* `name` (string): Name for this sync job
* `description` (string): Description for this sync job
* `src_path` (string): Absolute source path
* `dest_path` (string): Absolute destination path
* `src_remote_server_id` (Nullable<Int64>): Remote server ID for the source
* `dest_remote_server_id` (Nullable<Int64>): Remote server ID for the destination
* `two_way` (bool): Is this a two-way sync?
* `keep_after_copy` (bool): Keep files after copying?
* `delete_empty_folders` (bool): Delete empty folders after sync?
* `disabled` (bool): Is this sync disabled?
* `interval` (string): If trigger is `daily`, this specifies how often to run this sync.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `trigger` (string): Trigger type: daily, custom_schedule, or manual
* `trigger_file` (string): Some MFT services request an empty file (known as a trigger file) to signal the sync is complete and they can begin further processing. If trigger_file is set, a zero-byte file will be sent at the end of the sync.
* `recurring_day` (Nullable<Int64>): If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
* `schedule_time_zone` (string): If trigger is `custom_schedule`, Custom schedule Time Zone for when the sync should be run.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
* `schedule_times_of_day` (string[]): If trigger is `custom_schedule`, Custom schedule description for when the sync should be run. Times of day in HH:MM format.


---

## Delete Sync

```
var Sync = Sync.Find(1);

var parameters = new Dictionary<string, object>();


Sync.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Sync ID.
