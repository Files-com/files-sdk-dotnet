# Files.Models.BundleDownload

## Example BundleDownload Object

```
{
  "download_method": "file",
  "path": "a/b/test.txt",
  "created_at": "2020-01-01 00:00:00"
}
```

* `download_method` / `DownloadMethod`  (string): Download method (file or full_zip)
* `path` / `Path`  (string): Download path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Download date/time


---

## List Bundle Downloads

```
Task<BundleDownload[]> BundleDownload.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `bundle_registration_id` (Nullable<Int64>): Required - BundleRegistration ID
