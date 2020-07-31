# Files.Models.BandwidthSnapshot

## Example BandwidthSnapshot Object

```
{
  "id": 1,
  "bytes_received": 1.0,
  "bytes_sent": 1.0,
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

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
