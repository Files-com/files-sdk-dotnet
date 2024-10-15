# FilesCom.Models.ExternalEvent

## Example ExternalEvent Object

```
{
  "id": 1,
  "event_type": "example",
  "status": "example",
  "body": "example",
  "created_at": "2000-01-01T01:00:00Z",
  "body_url": "example",
  "folder_behavior_id": 1,
  "siem_http_destination_id": 1,
  "successful_files": 1,
  "errored_files": 1,
  "bytes_synced": 1,
  "compared_files": 1,
  "compared_folders": 1,
  "remote_server_type": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Event ID
* `event_type` / `EventType`  (string): Type of event being recorded.
* `status` / `Status`  (string): Status of event.
* `body` / `Body`  (string): Event body
* `created_at` / `CreatedAt`  (Nullable<DateTime>): External event create date/time
* `body_url` / `BodyUrl`  (string): Link to log file.
* `folder_behavior_id` / `FolderBehaviorId`  (Nullable<Int64>): Folder Behavior ID
* `siem_http_destination_id` / `SiemHttpDestinationId`  (Nullable<Int64>): SIEM HTTP Destination ID.
* `successful_files` / `SuccessfulFiles`  (Nullable<Int64>): For sync events, the number of files handled successfully.
* `errored_files` / `ErroredFiles`  (Nullable<Int64>): For sync events, the number of files that encountered errors.
* `bytes_synced` / `BytesSynced`  (Nullable<Int64>): For sync events, the total number of bytes synced.
* `compared_files` / `ComparedFiles`  (Nullable<Int64>): For sync events, the number of files considered for the sync.
* `compared_folders` / `ComparedFolders`  (Nullable<Int64>): For sync events, the number of folders listed and considered for the sync.
* `remote_server_type` / `RemoteServerType`  (string): Associated Remote Server type, if any


---

## List External Events

```
Task<FilesList<ExternalEvent>> ExternalEvent.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `remote_server_type`, `site_id`, `folder_behavior_id`, `siem_http_destination_id`, `created_at`, `event_type` or `status`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `event_type`, `remote_server_type`, `status`, `folder_behavior_id` or `siem_http_destination_id`. Valid field combinations are `[ created_at, event_type ]`, `[ created_at, remote_server_type ]`, `[ created_at, status ]`, `[ event_type, status ]` or `[ created_at, event_type, status ]`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `remote_server_type`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.


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
