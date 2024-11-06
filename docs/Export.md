# FilesCom.Models.Export

## Example Export Object

```
{
  "id": 1,
  "export_status": "example",
  "export_type": "example",
  "download_uri": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): ID for this Export
* `export_status` / `ExportStatus`  (string): Status of the Export
* `export_type` / `ExportType`  (string): Type of data being exported
* `download_uri` / `DownloadUri`  (string): Link to download Export file.


---

## List Exports

```
Task<FilesList<Export>> Export.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `export_status` and `export_type`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `export_status` and `export_type`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `export_type`.


---

## Show Export

```
Task<Export> Export.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Export ID.
