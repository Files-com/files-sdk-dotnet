# FilesCom.Models.SyncJob

## Example SyncJob Object

```
{
  "queued_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z",
  "status": "",
  "regional_worker_status": "",
  "uuid": "",
  "folder_behavior_id": 1
}
```

* `queued_at` / `QueuedAt`  (Nullable<DateTime>): Job enqueued at
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Job updated at
* `status` / `Status`  (string): Status of the job
* `regional_worker_status` / `RegionalWorkerStatus`  (string): Most recent reported status of sync worker
* `uuid` / `Uuid`  (string): 
* `folder_behavior_id` / `FolderBehaviorId`  (Nullable<Int64>): 


---

## List Sync Jobs

```
Task<SyncJob[]> SyncJob.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
