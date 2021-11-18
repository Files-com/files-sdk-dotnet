# FilesCom.Models.UsageSnapshot

## Example UsageSnapshot Object

```
{
  "id": 1,
  "start_at": "2000-01-01T01:00:00Z",
  "end_at": "2000-01-01T01:00:00Z",
  "created_at": "2000-01-01T01:00:00Z",
  "high_water_user_count": 1.0,
  "current_storage": 1.0,
  "high_water_storage": 1.0,
  "total_downloads": 1,
  "total_uploads": 1,
  "updated_at": "2000-01-01T01:00:00Z",
  "usage_by_top_level_dir": "",
  "root_storage": 1.0,
  "deleted_files_counted_in_minimum": 1.0,
  "deleted_files_storage": 1.0
}
```

* `id` / `Id`  (Nullable<Int64>): Site usage ID
* `start_at` / `StartAt`  (Nullable<DateTime>): Site usage report start date/time
* `end_at` / `EndAt`  (Nullable<DateTime>): Site usage report end date/time
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Site usage report created at date/time
* `high_water_user_count` / `HighWaterUserCount`  (double): Site usage report highest usage in time period
* `current_storage` / `CurrentStorage`  (double): Current site usage as of report
* `high_water_storage` / `HighWaterStorage`  (double): Site usage report highest usage in time period
* `total_downloads` / `TotalDownloads`  (Nullable<Int64>): Number of downloads in report time period
* `total_uploads` / `TotalUploads`  (Nullable<Int64>): Number of uploads in time period
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): The last time this site usage report was updated
* `usage_by_top_level_dir` / `UsageByTopLevelDir`  (object): A map of root folders to their total usage
* `root_storage` / `RootStorage`  (double): Usage for root folder
* `deleted_files_counted_in_minimum` / `DeletedFilesCountedInMinimum`  (double): Usage for files that are deleted but uploaded within last 30 days
* `deleted_files_storage` / `DeletedFilesStorage`  (double): Usage for files that are deleted but retained as backups


---

## List Usage Snapshots

```
Task<UsageSnapshot[]> UsageSnapshot.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
