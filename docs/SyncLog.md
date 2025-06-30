# FilesCom.Models.SyncLog

## Example SyncLog Object

```
{
  "timestamp": "2000-01-01T01:00:00Z",
  "sync_id": 1,
  "external_event_id": 1,
  "error_type": "example",
  "message": "example",
  "operation": "example",
  "path": "example",
  "size": 1,
  "file_type": "example",
  "status": "example",
  "created_at": "2000-01-01T01:00:00Z"
}
```

* `timestamp` / `Timestamp`  (Nullable<DateTime>): Start Time of Action. Deprecrated: Use created_at.
* `sync_id` / `SyncId`  (Nullable<Int64>): Sync ID
* `external_event_id` / `ExternalEventId`  (Nullable<Int64>): External Event ID
* `error_type` / `ErrorType`  (string): Error type, if applicable
* `message` / `Message`  (string): Message
* `operation` / `Operation`  (string): Operation type
* `path` / `Path`  (string): File path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `size` / `Size`  (Nullable<Int64>): File size
* `file_type` / `FileType`  (string): File type
* `status` / `Status`  (string): Status
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Start Time of Action


---

## List Sync Logs

```
Task<FilesList<SyncLog>> SyncLog.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `external_event_id`, `operation`, `status`, `sync_id` or `created_at`. Valid field combinations are `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ created_at ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ external_event_id, created_at ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ operation, created_at ]`, `[ status, sync_id ]`, `[ status, created_at ]`, `[ sync_id, created_at ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, operation, created_at ]`, `[ external_event_id, status, sync_id ]`, `[ external_event_id, status, created_at ]`, `[ external_event_id, sync_id, created_at ]`, `[ operation, status, sync_id ]`, `[ operation, status, created_at ]`, `[ operation, sync_id, created_at ]`, `[ status, sync_id, created_at ]`, `[ external_event_id, operation, status, sync_id ]`, `[ external_event_id, operation, status, created_at ]`, `[ external_event_id, operation, sync_id, created_at ]`, `[ external_event_id, status, sync_id, created_at ]`, `[ operation, status, sync_id, created_at ]` or `[ external_event_id, operation, status, sync_id, created_at ]`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`. Valid field combinations are `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ created_at ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ external_event_id, created_at ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ operation, created_at ]`, `[ status, sync_id ]`, `[ status, created_at ]`, `[ sync_id, created_at ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, operation, created_at ]`, `[ external_event_id, status, sync_id ]`, `[ external_event_id, status, created_at ]`, `[ external_event_id, sync_id, created_at ]`, `[ operation, status, sync_id ]`, `[ operation, status, created_at ]`, `[ operation, sync_id, created_at ]`, `[ status, sync_id, created_at ]`, `[ external_event_id, operation, status, sync_id ]`, `[ external_event_id, operation, status, created_at ]`, `[ external_event_id, operation, sync_id, created_at ]`, `[ external_event_id, status, sync_id, created_at ]`, `[ operation, status, sync_id, created_at ]` or `[ external_event_id, operation, status, sync_id, created_at ]`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`. Valid field combinations are `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ created_at ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ external_event_id, created_at ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ operation, created_at ]`, `[ status, sync_id ]`, `[ status, created_at ]`, `[ sync_id, created_at ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, operation, created_at ]`, `[ external_event_id, status, sync_id ]`, `[ external_event_id, status, created_at ]`, `[ external_event_id, sync_id, created_at ]`, `[ operation, status, sync_id ]`, `[ operation, status, created_at ]`, `[ operation, sync_id, created_at ]`, `[ status, sync_id, created_at ]`, `[ external_event_id, operation, status, sync_id ]`, `[ external_event_id, operation, status, created_at ]`, `[ external_event_id, operation, sync_id, created_at ]`, `[ external_event_id, status, sync_id, created_at ]`, `[ operation, status, sync_id, created_at ]` or `[ external_event_id, operation, status, sync_id, created_at ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `operation` and `status`. Valid field combinations are `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ created_at ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ external_event_id, created_at ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ operation, created_at ]`, `[ status, sync_id ]`, `[ status, created_at ]`, `[ sync_id, created_at ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, operation, created_at ]`, `[ external_event_id, status, sync_id ]`, `[ external_event_id, status, created_at ]`, `[ external_event_id, sync_id, created_at ]`, `[ operation, status, sync_id ]`, `[ operation, status, created_at ]`, `[ operation, sync_id, created_at ]`, `[ status, sync_id, created_at ]`, `[ external_event_id, operation, status, sync_id ]`, `[ external_event_id, operation, status, created_at ]`, `[ external_event_id, operation, sync_id, created_at ]`, `[ external_event_id, status, sync_id, created_at ]`, `[ operation, status, sync_id, created_at ]` or `[ external_event_id, operation, status, sync_id, created_at ]`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`. Valid field combinations are `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ created_at ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ external_event_id, created_at ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ operation, created_at ]`, `[ status, sync_id ]`, `[ status, created_at ]`, `[ sync_id, created_at ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, operation, created_at ]`, `[ external_event_id, status, sync_id ]`, `[ external_event_id, status, created_at ]`, `[ external_event_id, sync_id, created_at ]`, `[ operation, status, sync_id ]`, `[ operation, status, created_at ]`, `[ operation, sync_id, created_at ]`, `[ status, sync_id, created_at ]`, `[ external_event_id, operation, status, sync_id ]`, `[ external_event_id, operation, status, created_at ]`, `[ external_event_id, operation, sync_id, created_at ]`, `[ external_event_id, status, sync_id, created_at ]`, `[ operation, status, sync_id, created_at ]` or `[ external_event_id, operation, status, sync_id, created_at ]`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`. Valid field combinations are `[ external_event_id ]`, `[ operation ]`, `[ status ]`, `[ sync_id ]`, `[ created_at ]`, `[ external_event_id, operation ]`, `[ external_event_id, status ]`, `[ external_event_id, sync_id ]`, `[ external_event_id, created_at ]`, `[ operation, status ]`, `[ operation, sync_id ]`, `[ operation, created_at ]`, `[ status, sync_id ]`, `[ status, created_at ]`, `[ sync_id, created_at ]`, `[ external_event_id, operation, status ]`, `[ external_event_id, operation, sync_id ]`, `[ external_event_id, operation, created_at ]`, `[ external_event_id, status, sync_id ]`, `[ external_event_id, status, created_at ]`, `[ external_event_id, sync_id, created_at ]`, `[ operation, status, sync_id ]`, `[ operation, status, created_at ]`, `[ operation, sync_id, created_at ]`, `[ status, sync_id, created_at ]`, `[ external_event_id, operation, status, sync_id ]`, `[ external_event_id, operation, status, created_at ]`, `[ external_event_id, operation, sync_id, created_at ]`, `[ external_event_id, status, sync_id, created_at ]`, `[ operation, status, sync_id, created_at ]` or `[ external_event_id, operation, status, sync_id, created_at ]`.
