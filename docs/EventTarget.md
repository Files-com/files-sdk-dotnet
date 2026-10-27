# FilesCom.Models.EventTarget

## Example EventTarget Object

```
{
  "id": 1,
  "name": "example",
  "target_type": "webhook",
  "workspace_id": 1,
  "apply_to_all_workspaces": true,
  "enabled": true,
  "config": {
    "url": "https://example.com/files-events",
    "headers": {
      "X-Example": "value"
    },
    "use_dedicated_ips": false
  },
  "delivery_policy": {
    "max_attempts": 3,
    "retry_delay_minutes": [
      1,
      10
    ]
  },
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
* `config` / `Config`  (object): Event Target configuration. Fields depend on target_type.
* `delivery_policy` / `DeliveryPolicy`  (object): Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400. Slack and Teams support delivery_mode (immediate or subfolder_summary), summary_path (empty string means root), and summary_delay (15–300 seconds, default 60). The summary applies to matching file activity under summary_path; other matching events retain immediate delivery.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Event Target create date/time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Event Target update date/time.


---

## List Event Targets

```
FilesList<EventTarget> EventTarget.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<EventTarget> client.EventTargets.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `enabled` or `workspace_id`.
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<EventTarget> client.EventTargets.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<EventTarget> client.EventTargets.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `name` (string): Required - Event Target name.
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace target can receive events from all workspaces.
* `enabled` (bool): Whether this Event Target can receive events.
* `config` (object): Required - Event Target configuration. Fields depend on target_type.
* `delivery_policy` (object): Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400. Slack and Teams support delivery_mode (immediate or subfolder_summary), summary_path (empty string means root), and summary_delay (15–300 seconds, default 60). The summary applies to matching file activity under summary_path; other matching events retain immediate delivery.
* `target_type` (string): Required - Event Target type.


---

## Update Event Target

```
Task<EventTarget> EventTarget.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<EventTarget> client.EventTargets.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.
* `name` (string): Event Target name.
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace target can receive events from all workspaces.
* `enabled` (bool): Whether this Event Target can receive events.
* `config` (object): Event Target configuration. Fields depend on target_type.
* `delivery_policy` (object): Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400. Slack and Teams support delivery_mode (immediate or subfolder_summary), summary_path (empty string means root), and summary_delay (15–300 seconds, default 60). The summary applies to matching file activity under summary_path; other matching events retain immediate delivery.


---

## Delete Event Target

```
Task EventTarget.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.EventTargets.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.


---

## Update Event Target

```
var eventTarget = await EventTarget.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "example");
parameters.Add("workspace_id", 1L);
parameters.Add("apply_to_all_workspaces", true);
parameters.Add("enabled", true);
parameters.Add("config", new Dictionary<string, object> { { "url", "https://example.com/files-events" }, { "headers", new Dictionary<string, object> { { "X-Example", "value" } } }, { "use_dedicated_ips", false } });
parameters.Add("delivery_policy", new Dictionary<string, object> { { "max_attempts", 3 }, { "retry_delay_minutes", new object[] { 1, 10 } } });

await eventTarget.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `EventTarget` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.
* `name` (string): Event Target name.
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace target can receive events from all workspaces.
* `enabled` (bool): Whether this Event Target can receive events.
* `config` (object): Event Target configuration. Fields depend on target_type.
* `delivery_policy` (object): Event Target delivery policy. Email and folder targets support batch_interval in seconds, between 600 and 86400. Slack and Teams support delivery_mode (immediate or subfolder_summary), summary_path (empty string means root), and summary_delay (15–300 seconds, default 60). The summary applies to matching file activity under summary_path; other matching events retain immediate delivery.


---

## Delete Event Target

```
var eventTarget = await EventTarget.Find(1L);

var parameters = new Dictionary<string, object>();

await eventTarget.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Event Target ID.


---

## Save

```
await eventTarget.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
