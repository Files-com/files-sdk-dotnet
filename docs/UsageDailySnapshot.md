# FilesCom.Models.UsageDailySnapshot

## Example UsageDailySnapshot Object

```
{
  "id": 1,
  "date": "2020-11-21",
  "current_storage": "65536",
  "usage_by_top_level_dir": [

  ]
}
```

* `id` / `Id`  (Nullable<Int64>): ID of the usage record
* `date` / `Date`  (Nullable<DateTime>): The date of this usage record
* `current_storage` / `CurrentStorage`  (Nullable<Int64>): The quantity of storage held for this site
* `usage_by_top_level_dir` / `UsageByTopLevelDir`  (string[]): Usage broken down by each top-level folder


---

## List Usage Daily Snapshots

```
Task<UsageDailySnapshot[]> UsageDailySnapshot.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `date` and `usage_snapshot_id`.
* `filter` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `date` and `usage_snapshot_id`.
* `filter_gt` (object): If set, return records where the specifiied field is greater than the supplied value. Valid fields are `date` and `usage_snapshot_id`.
* `filter_gteq` (object): If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `date` and `usage_snapshot_id`.
* `filter_like` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `date` and `usage_snapshot_id`.
* `filter_lt` (object): If set, return records where the specifiied field is less than the supplied value. Valid fields are `date` and `usage_snapshot_id`.
* `filter_lteq` (object): If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `date` and `usage_snapshot_id`.
