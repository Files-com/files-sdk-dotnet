# FilesCom.Models.HolidayCalendar

## Example HolidayCalendar Object

```
{
  "id": 1,
  "name": "Company Holidays",
  "definition": "example",
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Holiday Calendar ID. Use `custom_<id>` as a scheduled resource's `holiday_region`.
* `name` / `Name`  (string): Holiday Calendar name.
* `definition` / `Definition`  (object): Holiday rules for the calendar. For more information, refer to the Holiday Calendars section of the Files.com documentation.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Creation time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Last update time.


---

## List Holiday Calendars

```
Task<FilesList<HolidayCalendar>> HolidayCalendar.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .


---

## Show Holiday Calendar

```
Task<HolidayCalendar> HolidayCalendar.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Holiday Calendar ID.


---

## Create Holiday Calendar

```
Task<HolidayCalendar> HolidayCalendar.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - Holiday Calendar name.


---

## Update Holiday Calendar

```
Task<HolidayCalendar> HolidayCalendar.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Holiday Calendar ID.
* `name` (string): Holiday Calendar name.


---

## Delete Holiday Calendar

```
Task HolidayCalendar.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Holiday Calendar ID.


---

## Update Holiday Calendar

```
var HolidayCalendar = HolidayCalendar.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Company Holidays");

HolidayCalendar.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Holiday Calendar ID.
* `name` (string): Holiday Calendar name.


---

## Delete Holiday Calendar

```
var HolidayCalendar = HolidayCalendar.Find(1);

var parameters = new Dictionary<string, object>();


HolidayCalendar.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Holiday Calendar ID.
