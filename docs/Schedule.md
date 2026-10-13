# FilesCom.Models.Schedule

## Example Schedule Object

```
{
  "id": 1,
  "name": "Weekday overnight",
  "schedule_days_of_week": [
    1,
    2,
    3,
    4,
    5
  ],
  "schedule_times_of_day": [
    "01:00"
  ],
  "schedule_time_zone": "Eastern Time (US & Canada)",
  "holiday_region": "us",
  "human_readable_schedule": "Triggered every Monday, Tuesday, Wednesday, Thursday, Friday at 01:00 AM UTC TZ.",
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Schedule ID.
* `name` / `Name`  (string): Schedule name.
* `schedule_days_of_week` / `ScheduleDaysOfWeek`  (Nullable<Int64>[]): 0-based weekdays used by the Schedule. 0 is Sunday.
* `schedule_times_of_day` / `ScheduleTimesOfDay`  (string[]): Times of day in HH:MM format (24-hour).
* `schedule_time_zone` / `ScheduleTimeZone`  (string): Time zone for scheduled times. If not set, times are interpreted as UTC.
* `holiday_region` / `HolidayRegion`  (string): Optional holiday region on which linked resources do not run.
* `human_readable_schedule` / `HumanReadableSchedule`  (string): Human-readable Schedule description.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Creation time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last update time.


---

## List Schedules

```
FilesList<Schedule> Schedule.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<Schedule> client.Schedules.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`.


---

## Show Schedule

```
Task<Schedule> Schedule.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Schedule> client.Schedules.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Schedule ID.


---

## Create Schedule

```
Task<Schedule> Schedule.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Schedule> client.Schedules.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `name` (string): Required - Schedule name.
* `schedule_days_of_week` (Nullable<Int64>[]): Required - 0-based weekdays used by the Schedule. 0 is Sunday.
* `schedule_times_of_day` (string[]): Required - Times of day in HH:MM format (24-hour).
* `schedule_time_zone` (string): Time zone for scheduled times. If not set, times are interpreted as UTC.
* `holiday_region` (string): Optional holiday region on which linked resources do not run.


---

## Update Schedule

```
Task<Schedule> Schedule.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Schedule> client.Schedules.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Schedule ID.
* `name` (string): Schedule name.
* `schedule_days_of_week` (Nullable<Int64>[]): 0-based weekdays used by the Schedule. 0 is Sunday.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format (24-hour).
* `schedule_time_zone` (string): Time zone for scheduled times. If not set, times are interpreted as UTC.
* `holiday_region` (string): Optional holiday region on which linked resources do not run.


---

## Delete Schedule

```
Task Schedule.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Schedules.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Schedule ID.


---

## Update Schedule

```
var schedule = await Schedule.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Weekday overnight");
parameters.Add("schedule_days_of_week", new Nullable<Int64>[] { 1, 2, 3, 4, 5 });
parameters.Add("schedule_times_of_day", new string[] { "01:00" });
parameters.Add("schedule_time_zone", "Eastern Time (US & Canada)");
parameters.Add("holiday_region", "us");

await schedule.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `Schedule` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Schedule ID.
* `name` (string): Schedule name.
* `schedule_days_of_week` (Nullable<Int64>[]): 0-based weekdays used by the Schedule. 0 is Sunday.
* `schedule_times_of_day` (string[]): Times of day in HH:MM format (24-hour).
* `schedule_time_zone` (string): Time zone for scheduled times. If not set, times are interpreted as UTC.
* `holiday_region` (string): Optional holiday region on which linked resources do not run.


---

## Delete Schedule

```
var schedule = await Schedule.Find(1L);

var parameters = new Dictionary<string, object>();

await schedule.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Schedule ID.


---

## Save

```
await schedule.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
