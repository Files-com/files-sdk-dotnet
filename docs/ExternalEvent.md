# FilesCom.Models.ExternalEvent

## Example ExternalEvent Object

```
{
  "id": 1,
  "event_type": "",
  "status": "",
  "body": "",
  "created_at": "2000-01-01T01:00:00Z",
  "body_url": ""
}
```

* `id` / `Id`  (Nullable<Int64>): Event ID
* `event_type` / `EventType`  (string): Type of event being recorded.
* `status` / `Status`  (string): Status of event.
* `body` / `Body`  (string): Event body
* `created_at` / `CreatedAt`  (Nullable<DateTime>): External event create date/time
* `body_url` / `BodyUrl`  (string): Link to log file.


---

## List External Events

```
Task<ExternalEvent[]> ExternalEvent.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `remote_server_type`, `event_type`, `created_at` or `status`.
* `filter` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `created_at`, `event_type`, `remote_server_type` or `status`.
* `filter_gt` (object): If set, return records where the specifiied field is greater than the supplied value. Valid fields are `created_at`, `event_type`, `remote_server_type` or `status`.
* `filter_gteq` (object): If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `created_at`, `event_type`, `remote_server_type` or `status`.
* `filter_like` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `created_at`, `event_type`, `remote_server_type` or `status`.
* `filter_lt` (object): If set, return records where the specifiied field is less than the supplied value. Valid fields are `created_at`, `event_type`, `remote_server_type` or `status`.
* `filter_lteq` (object): If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `created_at`, `event_type`, `remote_server_type` or `status`.


---

## Show External Event

```
Task<ExternalEvent> ExternalEvent.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - External Event ID.


---

## Create External Event

```
Task<ExternalEvent> ExternalEvent.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `status` (string): Required - Status of event.
* `body` (string): Required - Event body
