# FilesCom.Models.Automation

## Example Automation Object

```
{
  "id": 1,
  "automation": "create_folder",
  "deleted": true,
  "disabled": true,
  "trigger": "realtime",
  "interval": "week",
  "last_modified_at": "2000-01-01T01:00:00Z",
  "name": "example",
  "schedule": "example",
  "source": "example",
  "destinations": [
    "destination"
  ],
  "destination_replace_from": "example",
  "destination_replace_to": "example",
  "description": "example",
  "path": "example",
  "user_id": 1,
  "sync_ids": [
    1,
    2
  ],
  "user_ids": [
    1,
    2
  ],
  "group_ids": [
    1,
    2
  ],
  "webhook_url": "https://app.files.com/api/webhooks/abc123",
  "trigger_actions": [
    "create"
  ],
  "value": {
    "limit": "1"
  }
}
```

* `id` / `Id`  (Nullable<Int64>): Automation ID
* `automation` / `AutomationType`  (string): Automation type
* `deleted` / `Deleted`  (bool): Indicates if the automation has been deleted.
* `disabled` / `Disabled`  (bool): If true, this automation will not run.
* `trigger` / `Trigger`  (string): How this automation is triggered to run. One of: `realtime`, `daily`, `custom_schedule`, `webhook`, `email`, or `action`.
* `interval` / `Interval`  (string): If trigger is `daily`, this specifies how often to run this automation.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `last_modified_at` / `LastModifiedAt`  (Nullable<DateTime>): Time when automation was last modified. Does not change for name or description updates.
* `name` / `Name`  (string): Name for this automation.
* `schedule` / `Schedule`  (object): If trigger is `custom_schedule`, Custom schedule description for when the automation should be run.
* `source` / `Source`  (string): Source Path
* `destinations` / `Destinations`  (string[]): Destination Path
* `destination_replace_from` / `DestinationReplaceFrom`  (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` / `DestinationReplaceTo`  (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `description` / `Description`  (string): Description for the this Automation.
* `path` / `Path`  (string): Path on which this Automation runs.  Supports globs. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `user_id` / `UserId`  (Nullable<Int64>): User ID of the Automation's creator.
* `sync_ids` / `SyncIds`  (Nullable<Int64>[]): IDs of remote sync folder behaviors to run by this Automation
* `user_ids` / `UserIds`  (Nullable<Int64>[]): IDs of Users for the Automation (i.e. who to Request File from)
* `group_ids` / `GroupIds`  (Nullable<Int64>[]): IDs of Groups for the Automation (i.e. who to Request File from)
* `webhook_url` / `WebhookUrl`  (string): If trigger is `webhook`, this is the URL of the webhook to trigger the Automation.
* `trigger_actions` / `TriggerActions`  (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `value` / `Value`  (object): A Hash of attributes specific to the automation type.
* `destination` / `Destination`  (string): DEPRECATED: Destination Path. Use `destinations` instead.


---

## List Automations

```
Task<Automation[]> Automation.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[automation]=desc`). Valid fields are `automation`, `disabled`, `last_modified_at` or `name`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `automation`, `last_modified_at` or `disabled`. Valid field combinations are `[ automation, disabled ]` and `[ disabled, automation ]`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `automation`, `last_modified_at` or `disabled`. Valid field combinations are `[ automation, disabled ]` and `[ disabled, automation ]`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal to the supplied value. Valid fields are `automation`, `last_modified_at` or `disabled`. Valid field combinations are `[ automation, disabled ]` and `[ disabled, automation ]`.
* `filter_like` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `automation`, `last_modified_at` or `disabled`. Valid field combinations are `[ automation, disabled ]` and `[ disabled, automation ]`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `automation`, `last_modified_at` or `disabled`. Valid field combinations are `[ automation, disabled ]` and `[ disabled, automation ]`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal to the supplied value. Valid fields are `automation`, `last_modified_at` or `disabled`. Valid field combinations are `[ automation, disabled ]` and `[ disabled, automation ]`.
* `with_deleted` (bool): Set to true to include deleted automations in the results.
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

* `source` (string): Source Path
* `destination` (string): DEPRECATED: Destination Path. Use `destinations` instead.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `sync_ids` (string): A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule` (object): Custom schedule for running this automation.
* `description` (string): Description for the this Automation.
* `disabled` (bool): If true, this automation will not run.
* `name` (string): Name for this automation.
* `trigger` (string): How this automation is triggered to run. One of: `realtime`, `daily`, `custom_schedule`, `webhook`, `email`, or `action`.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `value` (object): A Hash of attributes specific to the automation type.
* `automation` (string): Required - Automation type


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
* `source` (string): Source Path
* `destination` (string): DEPRECATED: Destination Path. Use `destinations` instead.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `sync_ids` (string): A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule` (object): Custom schedule for running this automation.
* `description` (string): Description for the this Automation.
* `disabled` (bool): If true, this automation will not run.
* `name` (string): Name for this automation.
* `trigger` (string): How this automation is triggered to run. One of: `realtime`, `daily`, `custom_schedule`, `webhook`, `email`, or `action`.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `value` (object): A Hash of attributes specific to the automation type.
* `automation` (string): Automation type


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

parameters.Add("source", "source");
parameters.Add("destinations", ["folder_a/file_a.txt",{"folder_path":"folder_b","file_path":"file_b.txt"},{"folder_path":"folder_c"}]);
parameters.Add("destination_replace_from", "example");
parameters.Add("destination_replace_to", "example");
parameters.Add("interval", "year");
parameters.Add("path", "example");
parameters.Add("sync_ids", [1,2]);
parameters.Add("user_ids", [1,2]);
parameters.Add("group_ids", [1,2]);
parameters.Add("schedule", {"days_of_week":[0,1,3],"times_of_day":["7:30","11:30"],"time_zone":"Eastern Time (US & Canada)"});
parameters.Add("description", "example");
parameters.Add("disabled", true);
parameters.Add("name", "example");
parameters.Add("trigger", "realtime");
parameters.Add("trigger_actions", ["create"]);
parameters.Add("value", {"limit":"1"});
parameters.Add("automation", "create_folder");

Automation.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
* `source` (string): Source Path
* `destination` (string): DEPRECATED: Destination Path. Use `destinations` instead.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `sync_ids` (string): A list of sync IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule` (object): Custom schedule for running this automation.
* `description` (string): Description for the this Automation.
* `disabled` (bool): If true, this automation will not run.
* `name` (string): Name for this automation.
* `trigger` (string): How this automation is triggered to run. One of: `realtime`, `daily`, `custom_schedule`, `webhook`, `email`, or `action`.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `value` (object): A Hash of attributes specific to the automation type.
* `automation` (string): Automation type


---

## Delete Automation

```
var Automation = Automation.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Automation.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
