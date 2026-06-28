# FilesCom.Models.AiTask

## Example AiTask Object

```
{
  "id": 1,
  "workspace_id": 1,
  "name": "Summarize daily reports",
  "description": "Summarizes files uploaded by the accounting team.",
  "prompt": "Summarize the uploaded file and identify follow-up actions.",
  "permission_set": "files_only",
  "path": "incoming/reports",
  "source": "*.pdf",
  "disabled": true,
  "trigger": "daily",
  "trigger_actions": [
    "create"
  ],
  "interval": "day",
  "recurring_day": 1,
  "schedule_days_of_week": [
    1,
    3,
    5
  ],
  "schedule_times_of_day": [
    "06:30"
  ],
  "schedule_time_zone": "Eastern Time (US & Canada)",
  "holiday_region": "us",
  "human_readable_schedule": "Runs every day at 06:30 AM UTC TZ.",
  "last_run_at": "2000-01-01T01:00:00Z",
  "master_admin_user_id": 1,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): AI Task ID.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. `0` means the default workspace.
* `name` / `Name`  (string): AI Task name.
* `description` / `Description`  (string): AI Task description.
* `prompt` / `Prompt`  (string): Prompt sent when this AI Task is invoked.
* `permission_set` / `PermissionSet`  (string): Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
* `path` / `Path`  (string): Path scope used for action-triggered AI Tasks. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `source` / `Source`  (string): Source glob used with `path` for action-triggered AI Tasks.
* `disabled` / `Disabled`  (bool): If true, this AI Task will not run.
* `trigger` / `Trigger`  (string): How this AI Task is triggered.
* `trigger_actions` / `TriggerActions`  (string[]): If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
* `interval` / `Interval`  (string): If trigger is `daily`, this specifies how often to run the AI Task.
* `recurring_day` / `RecurringDay`  (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` / `ScheduleDaysOfWeek`  (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` / `ScheduleTimesOfDay`  (string[]): Times of day in HH:MM format for scheduled AI Tasks.
* `schedule_time_zone` / `ScheduleTimeZone`  (string): Time zone used by the AI Task schedule.
* `holiday_region` / `HolidayRegion`  (string): Optional holiday region used by scheduled AI Tasks.
* `human_readable_schedule` / `HumanReadableSchedule`  (string): Human-readable schedule description.
* `last_run_at` / `LastRunAt`  (Nullable<DateTime>): Most recent successful invocation time.
* `master_admin_user_id` / `MasterAdminUserId`  (Nullable<Int64>): Master User ID used for AI Task invocations.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Creation time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last update time.


---

## List Ai Tasks

```
Task<FilesList<AiTask>> AiTask.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `id`, `disabled` or `updated_at`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled`, `trigger` or `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.


---

## Show Ai Task

```
Task<AiTask> AiTask.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Task ID.


---

## Create Ai Task

```
Task<AiTask> AiTask.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `description` (string): AI Task description.
* `disabled` (bool): If true, this AI Task will not run.
* `holiday_region` (string): Optional holiday region used by scheduled AI Tasks.
* `interval` (string): If trigger is `daily`, this specifies how often to run the AI Task.
* `name` (string): Required - AI Task name.
* `path` (string): Path scope used for action-triggered AI Tasks.
* `permission_set` (string): Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
* `prompt` (string): Required - Prompt sent when this AI Task is invoked.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_time_zone` (string): Time zone used by the AI Task schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for scheduled AI Tasks.
* `source` (string): Source glob used with `path` for action-triggered AI Tasks.
* `trigger` (string): How this AI Task is triggered.
* `trigger_actions` (string[]): If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Manually Run AI Task

```
Task AiTask.ManualRun(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Task ID.


---

## Update Ai Task

```
Task<AiTask> AiTask.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Task ID.
* `description` (string): AI Task description.
* `disabled` (bool): If true, this AI Task will not run.
* `holiday_region` (string): Optional holiday region used by scheduled AI Tasks.
* `interval` (string): If trigger is `daily`, this specifies how often to run the AI Task.
* `name` (string): AI Task name.
* `path` (string): Path scope used for action-triggered AI Tasks.
* `permission_set` (string): Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
* `prompt` (string): Prompt sent when this AI Task is invoked.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_time_zone` (string): Time zone used by the AI Task schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for scheduled AI Tasks.
* `source` (string): Source glob used with `path` for action-triggered AI Tasks.
* `trigger` (string): How this AI Task is triggered.
* `trigger_actions` (string[]): If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Delete Ai Task

```
Task AiTask.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Task ID.


---

## Manually Run AI Task

```
var AiTask = AiTask.Find(1);

var parameters = new Dictionary<string, object>();


AiTask.ManualRun
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Task ID.


---

## Update Ai Task

```
var AiTask = AiTask.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("description", "Summarizes files uploaded by the accounting team.");
parameters.Add("disabled", true);
parameters.Add("holiday_region", "us");
parameters.Add("interval", "day");
parameters.Add("name", "Summarize daily reports");
parameters.Add("path", "incoming/reports");
parameters.Add("permission_set", "files_only");
parameters.Add("prompt", "Summarize the uploaded file and identify follow-up actions.");
parameters.Add("recurring_day", 1);
parameters.Add("schedule_days_of_week", [1,3,5]);
parameters.Add("schedule_time_zone", "Eastern Time (US & Canada)");
parameters.Add("schedule_times_of_day", ["06:30"]);
parameters.Add("source", "*.pdf");
parameters.Add("trigger", "daily");
parameters.Add("trigger_actions", ["create"]);
parameters.Add("workspace_id", 0);

AiTask.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Task ID.
* `description` (string): AI Task description.
* `disabled` (bool): If true, this AI Task will not run.
* `holiday_region` (string): Optional holiday region used by scheduled AI Tasks.
* `interval` (string): If trigger is `daily`, this specifies how often to run the AI Task.
* `name` (string): AI Task name.
* `path` (string): Path scope used for action-triggered AI Tasks.
* `permission_set` (string): Permissions used by the internal API key for this AI Task. Valid values are `full` and `files_only`.
* `prompt` (string): Prompt sent when this AI Task is invoked.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_time_zone` (string): Time zone used by the AI Task schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for scheduled AI Tasks.
* `source` (string): Source glob used with `path` for action-triggered AI Tasks.
* `trigger` (string): How this AI Task is triggered.
* `trigger_actions` (string[]): If trigger is `action`, the file action types that invoke this AI Task. Valid actions are create, copy, move, archived_delete, update, read, destroy.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Delete Ai Task

```
var AiTask = AiTask.Find(1);

var parameters = new Dictionary<string, object>();


AiTask.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Ai Task ID.
