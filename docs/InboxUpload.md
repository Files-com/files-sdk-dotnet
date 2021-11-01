# FilesCom.Models.InboxUpload

## Example InboxUpload Object

```
{
  "inbox_registration": "",
  "path": "a/b/test.txt",
  "created_at": "2020-01-01 00:00:00"
}
```

* `inbox_registration` / `InboxRegistration`  (object): 
* `path` / `Path`  (string): Upload path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Upload date/time


---

## List Inbox Uploads

```
Task<InboxUpload[]> InboxUpload.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `created_at`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal to the supplied value. Valid fields are `created_at`.
* `filter_like` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal to the supplied value. Valid fields are `created_at`.
* `inbox_registration_id` (Nullable<Int64>): InboxRegistration ID
* `inbox_id` (Nullable<Int64>): Inbox ID
