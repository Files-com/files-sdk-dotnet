# FilesCom.Models.BandwidthSnapshot

## Example BandwidthSnapshot Object

```
{
  "id": 1,
  "bytes_received": 1.0,
  "bytes_sent": 1.0,
  "sync_bytes_received": 1.0,
  "sync_bytes_sent": 1.0,
  "requests_get": 1.0,
  "requests_put": 1.0,
  "requests_other": 1.0,
  "logged_at": "2000-01-01T01:00:00Z",
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Site bandwidth ID
* `bytes_received` / `BytesReceived`  (double): Site bandwidth report bytes received
* `bytes_sent` / `BytesSent`  (double): Site bandwidth report bytes sent
* `sync_bytes_received` / `SyncBytesReceived`  (double): Site sync bandwidth report bytes received
* `sync_bytes_sent` / `SyncBytesSent`  (double): Site sync bandwidth report bytes sent
* `requests_get` / `RequestsGet`  (double): Site bandwidth report get requests
* `requests_put` / `RequestsPut`  (double): Site bandwidth report put requests
* `requests_other` / `RequestsOther`  (double): Site bandwidth report other requests
* `logged_at` / `LoggedAt`  (Nullable<DateTime>): Time the site bandwidth report was logged
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Site bandwidth report created at date/time
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): The last time this site bandwidth report was updated


---

## List Bandwidth Snapshots

```
Task<BandwidthSnapshot[]> BandwidthSnapshot.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `logged_at`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `logged_at`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `logged_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal to the supplied value. Valid fields are `logged_at`.
* `filter_like` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `logged_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `logged_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal to the supplied value. Valid fields are `logged_at`.
