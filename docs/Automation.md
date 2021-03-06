# FilesCom.Models.Automation

## Example Automation Object

```
{
  "id": 1,
  "automation": "create_folder",
  "trigger": "realtime",
  "interval": "week",
  "next_process_on": "2020-01-01",
  "schedule": {
    "days_of_week": [
      0,
      2,
      4
    ],
    "times_of_day": [
      "6:30",
      "14:30"
    ],
    "time_zone": "Eastern Time (US & Canada)"
  },
  "source": "",
  "destinations": [
    "destination"
  ],
  "destination_replace_from": "",
  "destination_replace_to": "",
  "path": "",
  "user_id": 1,
  "user_ids": [
    1,
    2
  ],
  "group_ids": [
    1,
    2
  ],
  "webhook_url": "https://app.files.com/api/webhooks/abc123",
  "trigger_actions": "[ \"create\" ]",
  "trigger_action_path": "path/to/file/or/folder",
  "value": "{\"limit\": \"1\"}"
}
```

* `id` / `Id`  (Nullable<Int64>): Automation ID
* `automation` / `AutomationType`  (string): Automation type
* `trigger` / `Trigger`  (string): How this automation is triggered to run. One of: `realtime`, `daily`, `custom_schedule`, `webhook`, `email`, or `action`.
* `interval` / `Interval`  (string): If trigger is `daily`, this specifies how often to run this automation.  One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `next_process_on` / `NextProcessOn`  (string): If trigger is `daily`, date this automation will next run.
* `schedule` / `Schedule`  (object): If trigger is `custom_schedule`, Custom schedule description for when the automation should be run.
* `source` / `Source`  (string): Source Path
* `destinations` / `Destinations`  (string): Destination Path
* `destination_replace_from` / `DestinationReplaceFrom`  (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` / `DestinationReplaceTo`  (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `path` / `Path`  (string): Path on which this Automation runs.  Supports globs. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `user_id` / `UserId`  (Nullable<Int64>): User ID of the Automation's creator.
* `user_ids` / `UserIds`  (Nullable<Int64>[]): IDs of Users for the Automation (i.e. who to Request File from)
* `group_ids` / `GroupIds`  (Nullable<Int64>[]): IDs of Groups for the Automation (i.e. who to Request File from)
* `webhook_url` / `WebhookUrl`  (string): If trigger is `webhook`, this is the URL of the webhook to trigger the Automation.
* `trigger_actions` / `TriggerActions`  (string): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `trigger_action_path` / `TriggerActionPath`  (string): If trigger is `action`, this is the path to watch for the specified trigger actions.
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

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `automation`.
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

* `automation` (string): Required - Automation type
* `source` (string): Source Path
* `destination` (string): DEPRECATED: Destination Path. Use `destinations` instead.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule` (object): Custom schedule for running this automation.
* `trigger` (string): How this automation is triggered to run. One of: `realtime`, `daily`, `custom_schedule`, `webhook`, `email`, or `action`.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `trigger_action_path` (string): If trigger is `action`, this is the path to watch for the specified trigger actions.
* `value` (object): A Hash of attributes specific to the automation type.


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
* `automation` (string): Required - Automation type
* `source` (string): Source Path
* `destination` (string): DEPRECATED: Destination Path. Use `destinations` instead.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule` (object): Custom schedule for running this automation.
* `trigger` (string): How this automation is triggered to run. One of: `realtime`, `daily`, `custom_schedule`, `webhook`, `email`, or `action`.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `trigger_action_path` (string): If trigger is `action`, this is the path to watch for the specified trigger actions.
* `value` (object): A Hash of attributes specific to the automation type.


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
parameters.Add("destinations", "[\"folder_a/file_a.txt\", {\"folder_path\":\"folder_b\", \"file_path\":\"file_b.txt\"}, {\"folder_path\":\"folder_c\"}]");
parameters.Add("interval", "year");
parameters.Add("user_ids", [1,2]);
parameters.Add("group_ids", [1,2]);
parameters.Add("schedule", "{\"days_of_week\": [ 0, 1, 3 ], \"times_of_day\": [ \"7:30\", \"11:30\" ], \"time_zone\": \"Eastern Time (US & Canada)\"}");
parameters.Add("trigger", "realtime");
parameters.Add("trigger_actions", "[ \"create\" ]");
parameters.Add("trigger_action_path", "path/to/file/or/folder");
parameters.Add("value", "{\"limit\": \"1\"}");

Automation.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
* `automation` (string): Required - Automation type
* `source` (string): Source Path
* `destination` (string): DEPRECATED: Destination Path. Use `destinations` instead.
* `destinations` (string[]): A list of String destination paths or Hash of folder_path and optional file_path.
* `destination_replace_from` (string): If set, this string in the destination path will be replaced with the value in `destination_replace_to`.
* `destination_replace_to` (string): If set, this string will replace the value `destination_replace_from` in the destination filename. You can use special patterns here.
* `interval` (string): How often to run this automation? One of: `day`, `week`, `week_end`, `month`, `month_end`, `quarter`, `quarter_end`, `year`, `year_end`
* `path` (string): Path on which this Automation runs.  Supports globs.
* `user_ids` (string): A list of user IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs the automation is associated with. If sent as a string, it should be comma-delimited.
* `schedule` (object): Custom schedule for running this automation.
* `trigger` (string): How this automation is triggered to run. One of: `realtime`, `daily`, `custom_schedule`, `webhook`, `email`, or `action`.
* `trigger_actions` (string[]): If trigger is `action`, this is the list of action types on which to trigger the automation. Valid actions are create, read, update, destroy, move, copy
* `trigger_action_path` (string): If trigger is `action`, this is the path to watch for the specified trigger actions.
* `value` (object): A Hash of attributes specific to the automation type.


---

## Delete Automation

```
var Automation = Automation.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Automation.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Automation ID.
