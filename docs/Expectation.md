# FilesCom.Models.Expectation

## Example Expectation Object

```
{
  "id": 1,
  "workspace_id": 1,
  "name": "Daily Vendor Feed",
  "description": "Wait for the vendor CSV every morning.",
  "path": "incoming/vendor_a",
  "source": "*.csv",
  "exclude_pattern": "*.tmp",
  "disabled": true,
  "expectations_version": 1,
  "trigger": "manual",
  "interval": "day",
  "recurring_day": 3,
  "schedule_days_of_week": [
    1,
    3,
    5
  ],
  "schedule_times_of_day": [
    "06:00"
  ],
  "schedule_time_zone": "UTC",
  "holiday_region": "us",
  "lookback_interval": 3600,
  "late_acceptance_interval": 900,
  "inactivity_interval": 300,
  "max_open_interval": 43200,
  "criteria": {
    "count": {
      "exact": 1
    },
    "extensions": [
      "csv"
    ]
  },
  "last_evaluated_at": "2000-01-01T01:00:00Z",
  "last_success_at": "2000-01-01T01:00:00Z",
  "last_failure_at": "2000-01-01T01:00:00Z",
  "last_result": "success",
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Expectation ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. `0` means the default workspace.
* `name` / `Name`  (string): Expectation name.
* `description` / `Description`  (string): Expectation description.
* `path` / `Path`  (string): Path scope for the expectation. Supports workspace-relative presentation. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `source` / `Source`  (string): Source glob used to select candidate files.
* `exclude_pattern` / `ExcludePattern`  (string): Optional source exclusion glob.
* `disabled` / `Disabled`  (bool): If true, the expectation is disabled.
* `expectations_version` / `ExpectationsVersion`  (Nullable<Int64>): Criteria schema version for this expectation.
* `trigger` / `Trigger`  (string): How this expectation opens windows.
* `interval` / `Interval`  (string): If trigger is `daily`, this specifies how often to run the expectation.
* `recurring_day` / `RecurringDay`  (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` / `ScheduleDaysOfWeek`  (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` / `ScheduleTimesOfDay`  (string[]): Times of day in HH:MM format for schedule-driven expectations.
* `schedule_time_zone` / `ScheduleTimeZone`  (string): Time zone used by the expectation schedule.
* `holiday_region` / `HolidayRegion`  (string): Optional holiday region used by schedule-driven expectations.
* `lookback_interval` / `LookbackInterval`  (Nullable<Int64>): How many seconds before the due boundary the window starts.
* `late_acceptance_interval` / `LateAcceptanceInterval`  (Nullable<Int64>): How many seconds a schedule-driven window may remain eligible to close as late.
* `inactivity_interval` / `InactivityInterval`  (Nullable<Int64>): How many quiet seconds are required before final closure.
* `max_open_interval` / `MaxOpenInterval`  (Nullable<Int64>): Hard-stop duration in seconds for unscheduled expectations.
* `criteria` / `Criteria`  (object): Structured criteria v1 definition for the expectation.
* `last_evaluated_at` / `LastEvaluatedAt`  (Nullable<DateTime>): Last time this expectation was evaluated.
* `last_success_at` / `LastSuccessAt`  (Nullable<DateTime>): Last time this expectation closed successfully.
* `last_failure_at` / `LastFailureAt`  (Nullable<DateTime>): Last time this expectation closed with a failure result.
* `last_result` / `LastResult`  (string): Most recent terminal result for this expectation.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Creation time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last update time.


---

## List Expectations

```
Task<FilesList<Expectation>> Expectation.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `name` or `disabled`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `workspace_id`. Valid field combinations are `[ workspace_id, disabled ]`.


---

## Show Expectation

```
Task<Expectation> Expectation.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation ID.


---

## Create Expectation

```
Task<Expectation> Expectation.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Expectation name.
* `description` (string): Expectation description.
* `path` (string): Path scope for the expectation. Supports workspace-relative presentation.
* `source` (string): Source glob used to select candidate files.
* `exclude_pattern` (string): Optional source exclusion glob.
* `disabled` (bool): If true, the expectation is disabled.
* `trigger` (string): How this expectation opens windows.
* `interval` (string): If trigger is `daily`, this specifies how often to run the expectation.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for schedule-driven expectations.
* `schedule_time_zone` (string): Time zone used by the expectation schedule.
* `holiday_region` (string): Optional holiday region used by schedule-driven expectations.
* `lookback_interval` (Nullable<Int64>): How many seconds before the due boundary the window starts.
* `late_acceptance_interval` (Nullable<Int64>): How many seconds a schedule-driven window may remain eligible to close as late.
* `inactivity_interval` (Nullable<Int64>): How many quiet seconds are required before final closure.
* `max_open_interval` (Nullable<Int64>): Hard-stop duration in seconds for unscheduled expectations.
* `criteria` (object): Structured criteria v1 definition for the expectation.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Manually open an Expectation window

```
Task<ExpectationEvaluation> Expectation.TriggerEvaluation(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation ID.


---

## Update Expectation

```
Task<Expectation> Expectation.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation ID.
* `name` (string): Expectation name.
* `description` (string): Expectation description.
* `path` (string): Path scope for the expectation. Supports workspace-relative presentation.
* `source` (string): Source glob used to select candidate files.
* `exclude_pattern` (string): Optional source exclusion glob.
* `disabled` (bool): If true, the expectation is disabled.
* `trigger` (string): How this expectation opens windows.
* `interval` (string): If trigger is `daily`, this specifies how often to run the expectation.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for schedule-driven expectations.
* `schedule_time_zone` (string): Time zone used by the expectation schedule.
* `holiday_region` (string): Optional holiday region used by schedule-driven expectations.
* `lookback_interval` (Nullable<Int64>): How many seconds before the due boundary the window starts.
* `late_acceptance_interval` (Nullable<Int64>): How many seconds a schedule-driven window may remain eligible to close as late.
* `inactivity_interval` (Nullable<Int64>): How many quiet seconds are required before final closure.
* `max_open_interval` (Nullable<Int64>): Hard-stop duration in seconds for unscheduled expectations.
* `criteria` (object): Structured criteria v1 definition for the expectation.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Delete Expectation

```
Task Expectation.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation ID.


---

## Manually open an Expectation window

```
var Expectation = Expectation.Find(1);

var parameters = new Dictionary<string, object>();


Expectation.TriggerEvaluation
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation ID.


---

## Update Expectation

```
var Expectation = Expectation.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Daily Vendor Feed");
parameters.Add("description", "Wait for the vendor CSV every morning.");
parameters.Add("path", "incoming/vendor_a");
parameters.Add("source", "*.csv");
parameters.Add("exclude_pattern", "*.tmp");
parameters.Add("disabled", true);
parameters.Add("trigger", "manual");
parameters.Add("interval", "day");
parameters.Add("recurring_day", 3);
parameters.Add("schedule_days_of_week", [1,3,5]);
parameters.Add("schedule_times_of_day", ["06:00"]);
parameters.Add("schedule_time_zone", "UTC");
parameters.Add("holiday_region", "us");
parameters.Add("lookback_interval", 3600);
parameters.Add("late_acceptance_interval", 900);
parameters.Add("inactivity_interval", 300);
parameters.Add("max_open_interval", 43200);
parameters.Add("criteria", {"count":{"exact":1},"extensions":["csv"]});
parameters.Add("workspace_id", 0);

Expectation.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation ID.
* `name` (string): Expectation name.
* `description` (string): Expectation description.
* `path` (string): Path scope for the expectation. Supports workspace-relative presentation.
* `source` (string): Source glob used to select candidate files.
* `exclude_pattern` (string): Optional source exclusion glob.
* `disabled` (bool): If true, the expectation is disabled.
* `trigger` (string): How this expectation opens windows.
* `interval` (string): If trigger is `daily`, this specifies how often to run the expectation.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for schedule-driven expectations.
* `schedule_time_zone` (string): Time zone used by the expectation schedule.
* `holiday_region` (string): Optional holiday region used by schedule-driven expectations.
* `lookback_interval` (Nullable<Int64>): How many seconds before the due boundary the window starts.
* `late_acceptance_interval` (Nullable<Int64>): How many seconds a schedule-driven window may remain eligible to close as late.
* `inactivity_interval` (Nullable<Int64>): How many quiet seconds are required before final closure.
* `max_open_interval` (Nullable<Int64>): Hard-stop duration in seconds for unscheduled expectations.
* `criteria` (object): Structured criteria v1 definition for the expectation.
* `workspace_id` (Nullable<Int64>): Workspace ID. `0` means the default workspace.


---

## Delete Expectation

```
var Expectation = Expectation.Find(1);

var parameters = new Dictionary<string, object>();


Expectation.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Expectation ID.
