# FilesCom.Models.ScheduledExport

## Example ScheduledExport Object

```
{
  "id": 1,
  "name": "Monthly access review",
  "export_type": "permission_audit",
  "report_name": "Permission audit",
  "export_options": {
    "group_by": "user"
  },
  "user_id": 1,
  "disabled": true,
  "trigger": "daily",
  "interval": "month",
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
  "human_readable_schedule": "Runs every month at 06:30 AM UTC TZ.",
  "last_run_at": "2000-01-01T01:00:00Z",
  "last_export_id": 1,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Scheduled Export ID
* `name` / `Name`  (string): Name for this scheduled export.
* `export_type` / `ExportType`  (string): Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
* `report_name` / `ReportName`  (string): Human-readable report name.
* `export_options` / `ExportOptions`  (object): Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
* `user_id` / `UserId`  (Nullable<Int64>): Site Admin user who receives the completed export e-mail.
* `disabled` / `Disabled`  (bool): If true, this scheduled export will not run.
* `trigger` / `Trigger`  (string): Schedule trigger type: `daily` or `custom_schedule`.
* `interval` / `Interval`  (string): If trigger is `daily`, this specifies how often to run the scheduled export.
* `recurring_day` / `RecurringDay`  (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` / `ScheduleDaysOfWeek`  (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` / `ScheduleTimesOfDay`  (string[]): Times of day in HH:MM format for schedule-driven exports.
* `schedule_time_zone` / `ScheduleTimeZone`  (string): Time zone used by the scheduled export.
* `holiday_region` / `HolidayRegion`  (string): Optional holiday region used by schedule-driven exports.
* `human_readable_schedule` / `HumanReadableSchedule`  (string): Human-readable schedule description.
* `last_run_at` / `LastRunAt`  (Nullable<DateTime>): Most recent scheduled run time.
* `last_export_id` / `LastExportId`  (Nullable<Int64>): Most recent Export ID created by this schedule.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Creation time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last update time.


---

## List Scheduled Exports

```
Task<FilesList<ScheduledExport>> ScheduledExport.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `export_type` or `disabled`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `disabled` and `export_type`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `export_type`.


---

## Show Scheduled Export

```
Task<ScheduledExport> ScheduledExport.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Scheduled Export ID.


---

## Create Scheduled Export

```
Task<ScheduledExport> ScheduledExport.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - Name for this scheduled export.
* `export_type` (string): Required - Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
* `export_options` (object): Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
* `user_id` (Nullable<Int64>): Site Admin user who receives the completed export e-mail.
* `disabled` (bool): If true, this scheduled export will not run.
* `trigger` (string): Schedule trigger type: `daily` or `custom_schedule`.
* `interval` (string): If trigger is `daily`, this specifies how often to run the scheduled export.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for schedule-driven exports.
* `schedule_time_zone` (string): Time zone used by the scheduled export.
* `holiday_region` (string): Optional holiday region used by schedule-driven exports.


---

## Update Scheduled Export

```
Task<ScheduledExport> ScheduledExport.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Scheduled Export ID.
* `name` (string): Name for this scheduled export.
* `export_type` (string): Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
* `export_options` (object): Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
* `user_id` (Nullable<Int64>): Site Admin user who receives the completed export e-mail.
* `disabled` (bool): If true, this scheduled export will not run.
* `trigger` (string): Schedule trigger type: `daily` or `custom_schedule`.
* `interval` (string): If trigger is `daily`, this specifies how often to run the scheduled export.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for schedule-driven exports.
* `schedule_time_zone` (string): Time zone used by the scheduled export.
* `holiday_region` (string): Optional holiday region used by schedule-driven exports.


---

## Delete Scheduled Export

```
Task ScheduledExport.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Scheduled Export ID.


---

## Update Scheduled Export

```
var ScheduledExport = ScheduledExport.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Monthly access review");
parameters.Add("export_type", "permission_audit");
parameters.Add("export_options", {"group_by":"user"});
parameters.Add("user_id", 1);
parameters.Add("disabled", true);
parameters.Add("trigger", "daily");
parameters.Add("interval", "month");
parameters.Add("recurring_day", 1);
parameters.Add("schedule_days_of_week", [1,3,5]);
parameters.Add("schedule_times_of_day", ["06:30"]);
parameters.Add("schedule_time_zone", "Eastern Time (US & Canada)");
parameters.Add("holiday_region", "us");

ScheduledExport.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Scheduled Export ID.
* `name` (string): Name for this scheduled export.
* `export_type` (string): Export report type. Valid values: folder_size_audit, group_membership_audit, permission_audit, share_link_audit
* `export_options` (object): Report-specific options. `permission_audit` supports `group_by` with `user` or `path`.
* `user_id` (Nullable<Int64>): Site Admin user who receives the completed export e-mail.
* `disabled` (bool): If true, this scheduled export will not run.
* `trigger` (string): Schedule trigger type: `daily` or `custom_schedule`.
* `interval` (string): If trigger is `daily`, this specifies how often to run the scheduled export.
* `recurring_day` (Nullable<Int64>): If trigger is `daily`, this selects the day number inside the chosen interval.
* `schedule_days_of_week` (Nullable<Int64>[]): If trigger is `custom_schedule`, the 0-based weekdays used by the schedule.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format for schedule-driven exports.
* `schedule_time_zone` (string): Time zone used by the scheduled export.
* `holiday_region` (string): Optional holiday region used by schedule-driven exports.


---

## Delete Scheduled Export

```
var ScheduledExport = ScheduledExport.Find(1);

var parameters = new Dictionary<string, object>();


ScheduledExport.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Scheduled Export ID.
