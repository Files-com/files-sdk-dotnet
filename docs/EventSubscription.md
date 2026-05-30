# FilesCom.Models.EventSubscription

## Example EventSubscription Object

```
{
  "id": 1,
  "event_channel_id": 1,
  "workspace_id": 1,
  "apply_to_all_workspaces": true,
  "name": "example",
  "enabled": true,
  "event_types": [
    "example"
  ],
  "filter": "example",
  "delivery_policy": "example",
  "event_target_ids": [
    1
  ],
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Event Subscription ID
* `event_channel_id` / `EventChannelId`  (Nullable<Int64>): Event Channel ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` / `ApplyToAllWorkspaces`  (bool): If true, this default-workspace subscription applies to events from all workspaces.
* `name` / `Name`  (string): Event Subscription name.
* `enabled` / `Enabled`  (bool): Whether this Event Subscription can dispatch events.
* `event_types` / `EventTypes`  (string[]): Event type strings matched by this subscription. Blank means all event types.
* `filter` / `Filter`  (object): Structured event payload filter.
* `delivery_policy` / `DeliveryPolicy`  (object): Event Subscription delivery policy.
* `event_target_ids` / `EventTargetIds`  (Nullable<Int64>[]): Event Target IDs this subscription sends to.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Event Subscription create date/time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Event Subscription update date/time.


---

## List Event Subscriptions

```
Task<FilesList<EventSubscription>> EventSubscription.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`, `enabled`, `event_channel_id` or `workspace_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `enabled`, `event_channel_id` or `workspace_id`. Valid field combinations are `[ enabled, event_channel_id ]`, `[ workspace_id, enabled ]` or `[ workspace_id, enabled, event_channel_id ]`.


---

## Show Event Subscription

```
Task<EventSubscription> EventSubscription.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Subscription ID.


---

## Create Event Subscription

```
Task<EventSubscription> EventSubscription.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `event_channel_id` (Nullable<Int64>): Event Channel ID
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace subscription applies to events from all workspaces.
* `name` (string): Required - Event Subscription name.
* `enabled` (bool): Whether this Event Subscription can dispatch events.
* `event_types` (string[]): Event type strings matched by this subscription. Blank means all event types.
* `filter` (object): Structured event payload filter.
* `delivery_policy` (object): Event Subscription delivery policy.
* `event_target_ids` (Nullable<Int64>[]): Event Target IDs this subscription sends to.


---

## Update Event Subscription

```
Task<EventSubscription> EventSubscription.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Subscription ID.
* `event_channel_id` (Nullable<Int64>): Event Channel ID
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace subscription applies to events from all workspaces.
* `name` (string): Event Subscription name.
* `enabled` (bool): Whether this Event Subscription can dispatch events.
* `event_types` (string[]): Event type strings matched by this subscription. Blank means all event types.
* `filter` (object): Structured event payload filter.
* `delivery_policy` (object): Event Subscription delivery policy.
* `event_target_ids` (Nullable<Int64>[]): Event Target IDs this subscription sends to.


---

## Delete Event Subscription

```
Task EventSubscription.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Subscription ID.


---

## Update Event Subscription

```
var EventSubscription = EventSubscription.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("event_channel_id", 1);
parameters.Add("workspace_id", 1);
parameters.Add("apply_to_all_workspaces", true);
parameters.Add("name", "example");
parameters.Add("enabled", true);
parameters.Add("event_types", ["example"]);
parameters.Add("delivery_policy", "example");
parameters.Add("event_target_ids", [1]);

EventSubscription.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Subscription ID.
* `event_channel_id` (Nullable<Int64>): Event Channel ID
* `workspace_id` (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `apply_to_all_workspaces` (bool): If true, this default-workspace subscription applies to events from all workspaces.
* `name` (string): Event Subscription name.
* `enabled` (bool): Whether this Event Subscription can dispatch events.
* `event_types` (string[]): Event type strings matched by this subscription. Blank means all event types.
* `filter` (object): Structured event payload filter.
* `delivery_policy` (object): Event Subscription delivery policy.
* `event_target_ids` (Nullable<Int64>[]): Event Target IDs this subscription sends to.


---

## Delete Event Subscription

```
var EventSubscription = EventSubscription.Find(1);

var parameters = new Dictionary<string, object>();


EventSubscription.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Subscription ID.
