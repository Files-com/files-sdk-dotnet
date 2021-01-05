# Files.Models.BundleDownload

## Example BundleDownload Object

```
{
  "bundle_registration": "",
  "download_method": "file",
  "path": "a/b/test.txt",
  "created_at": "2020-01-01 00:00:00"
}
```

* `bundle_registration` / `BundleRegistration`  (object): 
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

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `bundle_id` (Nullable<Int64>): Bundle ID
* `bundle_registration_id` (Nullable<Int64>): BundleRegistration ID
