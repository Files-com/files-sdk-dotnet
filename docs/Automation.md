# FilesCom.Models.Automation

## Example Automation Object

```
{
  "id": 1,
  "always_serialize_jobs": true,
  "always_overwrite_size_matching_files": true,
  "automation": "create_folder",
  "deleted": true,
  "description": "example",
  "destination_replace_from": "example",
  "destination_replace_to": "example",
  "destinations": [
    "destination"
  ],
  "disabled": true,
  "exclude_pattern": "path/to/exclude/*",
  "import_urls": [
    {
      "name": "users.json",
      "url": "http://example.com/users",
      "method": "POST",
      "headers": {
        "Content-Type": "application/json"
      },
      "content": {
        "group": "support"
      }
    }
  ],
  "flatten_destination_structure": true,
  "group_ids": [
    1,
    2
  ],
  "ignore_locked_folders": true,
  "interval": "week",
  "last_modified_at": "2000-01-01T01:00:00Z",
  "legacy_folder_matching": true,
  "name": "example",
  "overwrite_files": true,
  "path": "example",
  "path_time_zone": "Eastern Time (US & Canada)",
  "recurring_day": 25,
  "retry_on_failure_interval_in_minutes": 60,
  "retry_on_failure_number_of_attempts": 10,
  "schedule": {
    "days_of_week": [
      0,
      2,
      4
    ],
    "times_of_day": [
      "06:30",
      "14:30"
    ],
    "time_zone": "Eastern Time (US & Canada)"
  },
  "human_readable_schedule": "Triggered every Monday, Wednesday at 6:30 AM,\n  2:30 PM Eastern Time (US & Canada) TZ",
  "schedule_days_of_week": [
    0,
    2,
    4
  ],
  "schedule_times_of_day": [
    "06:30",
    "14:30"
  ],
  "schedule_time_zone": "Eastern Time (US & Canada)",
  "source": "example",
  "sync_ids": [
    1,
    2
  ],
  "trigger_actions": [
    "create"
  ],
  "trigger": "daily",
  "user_id": 1,
  "user_ids": [
    1,
    2
  ],
  "value": {
    "limit": "1"
  },
  "webhook_url": "https://app.files.com/api/webhooks/abc123",
  "holiday_region": "us_dc"
}
```

* `id` / `Id`  (Nullable<Int64>): Automation ID
* `always_serialize_jobs` / `AlwaysSerializeJobs`  (bool): Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
* `always_overwrite_size_matching_files` / `AlwaysOverwriteSizeMatchingFiles`  (bool): Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
* `automation` / `AutomationType`  (string): Automation type
* `deleted` / `Deleted`  (bool): Indicates if the automation has been deleted.
* `description` / `Description`  (string): Description for the this Automation.
* `destination_replace_from` / `DestinationReplaceFrom`  (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` / `DestinationReplaceTo`  (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `destinations` / `Destinations`  (string[]): Destination Paths
* `disabled` / `Disabled`  (bool): If true, this automation will not run.
* `exclude_pattern` / `ExcludePattern`  (string): If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
* `import_urls` / `ImportUrls`  (object[]): List of URLs to be imported and names to be used.
* `flatten_destination_structure` / `FlattenDestinationStructure`  (bool): Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
* `group_ids` / `GroupIds`  (Nullable<Int64>[]): IDs of Groups for the Automation (i.e. who to Request File from)
* `ignore_locked_folders` / `IgnoreLockedFolders`  (bool): If true, the Lock Folders behavior will be disregarded for automated actions.
* `interval` / `Interval`  (string): If trigger is `daily`, this specifies how often to run this automation.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `last_modified_at` / `LastModifiedAt`  (Nullable<DateTime>): Time when automation was last modified. Does not change for name or description updates.
* `legacy_folder_matching` / `LegacyFolderMatching`  (bool): If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
* `name` / `Name`  (string): Name for this automation.
* `overwrite_files` / `OverwriteFiles`  (bool): If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
* `path` / `Path`  (string): Path on which this Automation runs.  Supports globs, except on remote mounts. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `path_time_zone` / `PathTimeZone`  (string): Timezone to use when rendering timestamps in paths.
* `recurring_day` / `RecurringDay`  (Nullable<Int64>): If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
* `retry_on_failure_interval_in_minutes` / `RetryOnFailureIntervalInMinutes`  (Nullable<Int64>): If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
* `retry_on_failure_number_of_attempts` / `RetryOnFailureNumberOfAttempts`  (Nullable<Int64>): If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
* `schedule` / `Schedule`  (object): If trigger is `custom_schedule`, Custom schedule description for when the automation should be run in json format.
* `human_readable_schedule` / `HumanReadableSchedule`  (string): If trigger is `custom_schedule`, Human readable Custom schedule description for when the automation should be run.
* `schedule_days_of_week` / `ScheduleDaysOfWeek`  (Nullable<Int64>[]): If trigger is `custom_schedule`, Custom schedule description for when the automation should be run. 0-based days of the week. 0 is Sunday, 1 is Monday, etc.
* `schedule_times_of_day` / `ScheduleTimesOfDay`  (string[]): If trigger is `custom_schedule`, Custom schedule description for when the automation should be run. Times of day in HH:MM format.
* `schedule_time_zone` / `ScheduleTimeZone`  (string): If trigger is `custom_schedule`, Custom schedule Time Zone for when the automation should be run.
* `source` / `Source`  (string): Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
* `sync_ids` / `SyncIds`  (Nullable<Int64>[]): IDs of remote sync folder behaviors to run by this Automation
* `trigger_actions` / `TriggerActions`  (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `trigger` / `Trigger`  (string): How this automation is triggered to run.
* `user_id` / `UserId`  (Nullable<Int64>): User ID of the Automation's creator.
* `user_ids` / `UserIds`  (Nullable<Int64>[]): IDs of Users for the Automation (i.e. who to Request File from)
* `value` / `Value`  (object): A Hash of attributes specific to the automation type.
* `webhook_url` / `WebhookUrl`  (string): If trigger is `webhook`, this is the URL of the webhook to trigger the Automation.
* `holiday_region` / `HolidayRegion`  (string): If trigger is `custom_schedule`, the Automation will check if there is a formal, observed holiday for the region, and if so, it will not run.


---

## List Automations

```
Task<FilesList<Automation>> Automation.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `automation`, `disabled`, `last_modified_at` or `name`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled`, `last_modified_at` or `automation`. Valid field combinations are `[ disabled, last_modified_at ]`, `[ automation, disabled ]`, `[ automation, last_modified_at ]` or `[ automation, disabled, last_modified_at ]`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `last_modified_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `last_modified_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `last_modified_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `last_modified_at`.


---

## Show Automation

```
Task<Automation> Automation.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.


---

## Create Automation

```
Task<Automation> Automation.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `source` (string): Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs, except on remote mounts.
* `sync_ids` (string): A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`. A list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
* `schedule_times_of_day` (string[]): If trigger is `custom_schedule`. A list of times of day to run this automation. 24-hour time format.
* `schedule_time_zone` (string): If trigger is `custom_schedule`. Time zone for the schedule.
* `holiday_region` (string): If trigger is `custom_schedule`, the Automation will check if there is a formal, observed holiday for the region, and if so, it will not run.
* `always_overwrite_size_matching_files` (bool): Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
* `always_serialize_jobs` (bool): Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
* `description` (string): Description for the this Automation.
* `disabled` (bool): If true, this automation will not run.
* `exclude_pattern` (string): If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
* `import_urls` (object[]): List of URLs to be imported and names to be used.
* `flatten_destination_structure` (bool): Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
* `ignore_locked_folders` (bool): If true, the Lock Folders behavior will be disregarded for automated actions.
* `legacy_folder_matching` (bool): DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
* `name` (string): Name for this automation.
* `overwrite_files` (bool): If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
* `path_time_zone` (string): Timezone to use when rendering timestamps in paths.
* `retry_on_failure_interval_in_minutes` (Nullable<Int64>): If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
* `retry_on_failure_number_of_attempts` (Nullable<Int64>): If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
* `trigger` (string): How this automation is triggered to run.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `value` (object): A Hash of attributes specific to the automation type.
* `recurring_day` (Nullable<Int64>): If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
* `automation` (string): Required - Automation type


---

## Manually Run Automation

```
Task Automation.ManualRun(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.


---

## Update Automation

```
Task<Automation> Automation.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
* `source` (string): Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs, except on remote mounts.
* `sync_ids` (string): A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`. A list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
* `schedule_times_of_day` (string[]): If trigger is `custom_schedule`. A list of times of day to run this automation. 24-hour time format.
* `schedule_time_zone` (string): If trigger is `custom_schedule`. Time zone for the schedule.
* `holiday_region` (string): If trigger is `custom_schedule`, the Automation will check if there is a formal, observed holiday for the region, and if so, it will not run.
* `always_overwrite_size_matching_files` (bool): Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
* `always_serialize_jobs` (bool): Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
* `description` (string): Description for the this Automation.
* `disabled` (bool): If true, this automation will not run.
* `exclude_pattern` (string): If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
* `import_urls` (object[]): List of URLs to be imported and names to be used.
* `flatten_destination_structure` (bool): Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
* `ignore_locked_folders` (bool): If true, the Lock Folders behavior will be disregarded for automated actions.
* `legacy_folder_matching` (bool): DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
* `name` (string): Name for this automation.
* `overwrite_files` (bool): If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
* `path_time_zone` (string): Timezone to use when rendering timestamps in paths.
* `retry_on_failure_interval_in_minutes` (Nullable<Int64>): If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
* `retry_on_failure_number_of_attempts` (Nullable<Int64>): If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
* `trigger` (string): How this automation is triggered to run.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `value` (object): A Hash of attributes specific to the automation type.
* `recurring_day` (Nullable<Int64>): If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
* `automation` (string): Automation type


---

## Delete Automation

```
Task Automation.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.


---

## Manually Run Automation

```
var Automation = Automation.Find(1);

var parameters = new Dictionary<string, object>();


Automation.ManualRun
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.


---

## Update Automation

```
var Automation = Automation.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("source", "example");
parameters.Add("destinations", ["folder_a/file_a.txt",{"folder_path":"folder_b","file_path":"file_b.txt"},{"folder_path":"folder_c"}]);
parameters.Add("destination_replace_from", "example");
parameters.Add("destination_replace_to", "example");
parameters.Add("interval", "year");
parameters.Add("path", "example");
parameters.Add("sync_ids", [1,2]);
parameters.Add("user_ids", [1,2]);
parameters.Add("group_ids", [1,2]);
parameters.Add("schedule_days_of_week", [0,1,3]);
parameters.Add("schedule_times_of_day", ["7:30","11:30"]);
parameters.Add("schedule_time_zone", "Eastern Time (US & Canada)");
parameters.Add("holiday_region", "us_dc");
parameters.Add("always_overwrite_size_matching_files", true);
parameters.Add("always_serialize_jobs", true);
parameters.Add("description", "example");
parameters.Add("disabled", true);
parameters.Add("exclude_pattern", "path/to/exclude/*");
parameters.Add("import_urls", [{"name":"users.json","url":"http://example.com/users","method":"POST","headers":{"Content-Type":"application/json"},"content":{"group":"support"}}]);
parameters.Add("flatten_destination_structure", true);
parameters.Add("ignore_locked_folders", true);
parameters.Add("legacy_folder_matching", false);
parameters.Add("name", "example");
parameters.Add("overwrite_files", true);
parameters.Add("path_time_zone", "Eastern Time (US & Canada)");
parameters.Add("retry_on_failure_interval_in_minutes", 60);
parameters.Add("retry_on_failure_number_of_attempts", 10);
parameters.Add("trigger", "daily");
parameters.Add("trigger_actions", ["create"]);
parameters.Add("value", {"limit":"1"});
parameters.Add("recurring_day", 25);
parameters.Add("automation", "create_folder");

Automation.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
* `source` (string): Source path/glob.  See Automation docs for exact description, but this is used to filter for files in the `path` to find files to operate on. Supports globs, except on remote mounts.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs, except on remote mounts.
* `sync_ids` (string): A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`. A list of days of the week to run this automation. 0 is Sunday, 1 is Monday, etc.
* `schedule_times_of_day` (string[]): If trigger is `custom_schedule`. A list of times of day to run this automation. 24-hour time format.
* `schedule_time_zone` (string): If trigger is `custom_schedule`. Time zone for the schedule.
* `holiday_region` (string): If trigger is `custom_schedule`, the Automation will check if there is a formal, observed holiday for the region, and if so, it will not run.
* `always_overwrite_size_matching_files` (bool): Ordinarily, files with identical size in the source and destination will be skipped from copy operations to prevent wasted transfer.  If this flag is `true` we will overwrite the destination file always.  Note that this may cause large amounts of wasted transfer usage.  This setting has no effect unless `overwrite_files` is also set to `true`.
* `always_serialize_jobs` (bool): Ordinarily, we will allow automation runs to run in parallel for non-scheduled automations. If this flag is `true` we will force automation runs to be serialized (run one at a time, one after another). This can resolve some issues with race conditions on remote systems at the cost of some performance.
* `description` (string): Description for the this Automation.
* `disabled` (bool): If true, this automation will not run.
* `exclude_pattern` (string): If set, this glob pattern will exclude files from the automation. Supports globs, except on remote mounts.
* `import_urls` (object[]): List of URLs to be imported and names to be used.
* `flatten_destination_structure` (bool): Normally copy and move automations that use globs will implicitly preserve the source folder structure in the destination.  If this flag is `true`, the source folder structure will be flattened in the destination.  This is useful for copying or moving files from multiple folders into a single destination folder.
* `ignore_locked_folders` (bool): If true, the Lock Folders behavior will be disregarded for automated actions.
* `legacy_folder_matching` (bool): DEPRECATED: If `true`, use the legacy behavior for this automation, where it can operate on folders in addition to just files.  This behavior no longer works and should not be used.
* `name` (string): Name for this automation.
* `overwrite_files` (bool): If true, existing files will be overwritten with new files on Move/Copy automations.  Note: by default files will not be overwritten on Copy automations if they appear to be the same file size as the newly incoming file.  Use the `always_overwrite_size_matching_files` option in conjunction with `overwrite_files` to override this behavior and overwrite files no matter what.
* `path_time_zone` (string): Timezone to use when rendering timestamps in paths.
* `retry_on_failure_interval_in_minutes` (Nullable<Int64>): If the Automation fails, retry at this interval (in minutes).  Acceptable values are 5 through 1440 (one day).  Set to null to disable.
* `retry_on_failure_number_of_attempts` (Nullable<Int64>): If the Automation fails, retry at most this many times.  Maximum allowed value: 10.  Set to null to disable.
* `trigger` (string): How this automation is triggered to run.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `value` (object): A Hash of attributes specific to the automation type.
* `recurring_day` (Nullable<Int64>): If trigger type is `daily`, this specifies a day number to run in one of the supported intervals: `week`, `month`, `quarter`, `year`.
* `automation` (string): Automation type


---

## Delete Automation

```
var Automation = Automation.Find(1);

var parameters = new Dictionary<string, object>();


Automation.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
