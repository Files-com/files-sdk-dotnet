# Files.Models.Automation

## Example Automation Object

```
{
  "id": 1,
  "automation": "create_folder",
  "source": "",
  "destination": "",
  "destination_replace_from": "",
  "destination_replace_to": "",
  "interval": "week",
  "next_process_on": "2020-01-01",
  "path": "",
  "realtime": true,
  "user_id": 1,
  "user_ids": [

  ],
  "group_ids": [

  ]
}
```

* `id` / `Id`  (Nullable<Int64>): Automation ID
* `automation` / `AutomationType`  (string): Automation type
* `source` / `Source`  (string): Source Path
* `destination` / `Destination`  (string): Destination Path
* `destination_replace_from` / `DestinationReplaceFrom`  (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` / `DestinationReplaceTo`  (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` / `Interval`  (string): How often to run this automation?  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `next_process_on` / `NextProcessOn`  (string): Date this automation will next run.
* `path` / `Path`  (string): Path on which this Automation runs.  Supports globs. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `realtime` / `Realtime`  (bool): Does this automation run in real time?  This is a read-only property based on automation type.
* `user_id` / `UserId`  (Nullable<Int64>): User ID of the Automation's creator.
* `user_ids` / `UserIds`  (Nullable<Int64>[]): IDs of Users for the Automation (i.e. who to Request File from)
* `group_ids` / `GroupIds`  (Nullable<Int64>[]): IDs of Groups for the Automation (i.e. who to Request File from)


---

## List Automations

```
Task<Automation[]> Automation.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `cursor` (string): Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `site_id` and `automation`.
* `filter` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `automation`.
* `filter_gt` (object): If set, return records where the specifiied field is greater than the supplied value. Valid fields are `automation`.
* `filter_gteq` (object): If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `automation`.
* `filter_like` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `automation`.
* `filter_lt` (object): If set, return records where the specifiied field is less than the supplied value. Valid fields are `automation`.
* `filter_lteq` (object): If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `automation`.
* `automation` (string): DEPRECATED: Type of automation to filter by. Use `filter[automation]` instead.


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

* `automation` (string): Required - Type of automation.  One of: `create_folder`, `request_file`, `request_move`
* `source` (string): Source Path
* `destination` (string): Destination Path
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.


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
* `automation` (string): Required - Type of automation.  One of: `create_folder`, `request_file`, `request_move`
* `source` (string): Source Path
* `destination` (string): Destination Path
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.


---

## Delete Automation

```
Task<Automation> Automation.Delete(
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
var Automation = Automation.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("automation", "create_folder");
parameters.Add("source", "source");
parameters.Add("destination", "destination");
parameters.Add("interval", "year");

Automation.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
* `automation` (string): Required - Type of automation.  One of: `create_folder`, `request_file`, `request_move`
* `source` (string): Source Path
* `destination` (string): Destination Path
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.


---

## Delete Automation

```
var Automation = Automation.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Automation.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
