# FilesCom.Models.EventRecord

## Example EventRecord Object

```
{
  "id": 1,
  "workspace_id": 1,
  "event_uuid": "example",
  "event_type": "example",
  "severity": "example",
  "source_type": "example",
  "source_id": 1,
  "occurred_at": "2000-01-01T01:00:00Z",
  "human_title": "example",
  "human_summary": "example",
  "human_fields": [
    "example"
  ],
  "actor": "example",
  "resources": [
    "example"
  ],
  "payload": "example",
  "created_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Event Record ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID. 0 means the default workspace or site-wide.
* `event_uuid` / `EventUuid`  (string): Stable event UUID.
* `event_type` / `EventType`  (string): Versioned event type string.
* `severity` / `Severity`  (string): Event severity.
* `source_type` / `SourceType`  (string): Source record type.
* `source_id` / `SourceId`  (Nullable<Int64>): Source record ID.
* `occurred_at` / `OccurredAt`  (Nullable<DateTime>): Event occurrence date/time.
* `human_title` / `HumanTitle`  (string): Human-readable event title.
* `human_summary` / `HumanSummary`  (string): Human-readable event summary.
* `human_fields` / `HumanFields`  (object[]): Human-readable event detail fields.
* `actor` / `Actor`  (object): Actor associated with the event.
* `resources` / `Resources`  (object[]): Resources associated with the event.
* `payload` / `Payload`  (object): Event payload.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Event Record create date/time.


---

## List Event Records

```
Task<FilesList<EventRecord>> EventRecord.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `event_type`, `created_at` or `workspace_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `event_type` or `workspace_id`. Valid field combinations are `[ event_type, created_at ]`, `[ workspace_id, created_at ]`, `[ workspace_id, event_type ]` or `[ workspace_id, event_type, created_at ]`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `event_type`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.


---

## Show Event Record

```
Task<EventRecord> EventRecord.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Record ID.
