# FilesCom.Models.EventTarget

## Example EventTarget Object

```
{
  "id": 1,
  "name": "example",
  "target_type": "example",
  "workspace_id": 1,
  "apply_to_all_workspaces": true,
  "enabled": true,
  "config": "example",
  "delivery_policy": "example",
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Event Target ID
* `name` / `Name`  (string): Event Target name.
* `target_type` / `TargetType`  (string): Event Target type.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` / `ApplyToAllWorkspaces`  (bool): If true, this default-workspace target can receive events from all workspaces.
* `enabled` / `Enabled`  (bool): Whether this Event Target can receive events.
* `config` / `Config`  (object): Event Target configuration.
* `delivery_policy` / `DeliveryPolicy`  (object): Event Target delivery policy. Email targets support batch_interval in seconds, between 600 and 86400.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Event Target create date/time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Event Target update date/time.


---

## List Event Targets

```
Task<FilesList<EventTarget>> EventTarget.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `enabled` and `workspace_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `enabled`, `target_type` or `workspace_id`. Valid field combinations are `[ enabled, target_type ]`, `[ workspace_id, enabled ]` or `[ workspace_id, enabled, target_type ]`.


---

## Show Event Target

```
Task<EventTarget> EventTarget.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.


---

## Create Event Target

```
Task<EventTarget> EventTarget.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - Event Target name.
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace target can receive events from all workspaces.
* `target_type` (string): Required - Event Target type.
* `enabled` (bool): Whether this Event Target can receive events.
* `config` (object): Required - Event Target configuration.
* `delivery_policy` (object): Event Target delivery policy. Email targets support batch_interval in seconds, between 600 and 86400.


---

## Update Event Target

```
Task<EventTarget> EventTarget.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.
* `name` (string): Event Target name.
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace target can receive events from all workspaces.
* `target_type` (string): Event Target type.
* `enabled` (bool): Whether this Event Target can receive events.
* `config` (object): Event Target configuration.
* `delivery_policy` (object): Event Target delivery policy. Email targets support batch_interval in seconds, between 600 and 86400.


---

## Delete Event Target

```
Task EventTarget.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.


---

## Update Event Target

```
var EventTarget = EventTarget.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "example");
parameters.Add("workspace_id", 1);
parameters.Add("apply_to_all_workspaces", true);
parameters.Add("target_type", "example");
parameters.Add("enabled", true);
parameters.Add("config", "example");
parameters.Add("delivery_policy", "example");

EventTarget.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.
* `name` (string): Event Target name.
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace target can receive events from all workspaces.
* `target_type` (string): Event Target type.
* `enabled` (bool): Whether this Event Target can receive events.
* `config` (object): Event Target configuration.
* `delivery_policy` (object): Event Target delivery policy. Email targets support batch_interval in seconds, between 600 and 86400.


---

## Delete Event Target

```
var EventTarget = EventTarget.Find(1);

var parameters = new Dictionary<string, object>();


EventTarget.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.
