# Files.Models.UsageDailySnapshot

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

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
