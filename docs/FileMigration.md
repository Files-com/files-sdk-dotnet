# FilesCom.Models.FileMigration

## Example FileMigration Object

```
{
  "id": 1,
  "path": "MyFolder",
  "dest_path": "MyFolder",
  "files_moved": 1,
  "files_total": 1,
  "operation": "move",
  "region": "USA",
  "status": "complete",
  "log_url": "https://www.example.com/log_file"
}
```

* `id` / `Id`  (Nullable<Int64>): File migration ID
* `path` / `Path`  (string): Source path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `dest_path` / `DestPath`  (string): Destination path
* `files_moved` / `FilesMoved`  (Nullable<Int64>): Number of files processed
* `files_total` / `FilesTotal`  (Nullable<Int64>): 
* `operation` / `Operation`  (string): The type of operation
* `region` / `Region`  (string): Region
* `status` / `Status`  (string): Status
* `log_url` / `LogUrl`  (string): Link to download the log file for this migration.


---

## Show File Migration

```
Task<FileMigration> FileMigration.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - File Migration ID.
