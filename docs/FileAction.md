# Files.Models.FileAction

## Example FileAction Object

```
{
  "status": "enqueued",
  "file_migration_id": "123"
}
```

* `status` / `Status`  (string): Status of file operation. Possible values: completed, enqueued.
* `file_migration_id` / `FileMigrationId`  (Nullable<Int64>): If status is enqueued, this is the id of the FileMigration to check for status updates.


---

## Copy file/folder

```
Task<FileAction> FileAction.Copy(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Copy destination path.
* `structure` (bool): Copy structure only?


---

## Move file/folder

```
Task<FileAction> FileAction.Move(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Move destination path.


---

## Begin file upload

```
Task<FileAction[]> FileAction.BeginUpload(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `mkdir_parents` (bool): Create parent directories if they do not exist?
* `part` (Nullable<Int64>): Part if uploading a part.
* `parts` (Nullable<Int64>): How many parts to fetch?
* `ref` (string): 
* `restart` (Nullable<Int64>): File byte offset to restart from.
* `with_rename` (bool): Allow file rename instead of overwrite?


---

## Copy file/folder

```
var FileAction = FileAction.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("destination", "destination");
parameters.Add("structure", true);

FileAction.Copy(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Copy destination path.
* `structure` (bool): Copy structure only?


---

## Move file/folder

```
var FileAction = FileAction.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("destination", "destination");

FileAction.Move(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Move destination path.


---

## Begin file upload

```
var FileAction = FileAction.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("mkdir_parents", true);
parameters.Add("part", 1);
parameters.Add("parts", 1);
parameters.Add("ref", "upload-1");
parameters.Add("restart", 1);
parameters.Add("with_rename", true);

FileAction.BeginUpload(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `mkdir_parents` (bool): Create parent directories if they do not exist?
* `part` (Nullable<Int64>): Part if uploading a part.
* `parts` (Nullable<Int64>): How many parts to fetch?
* `ref` (string): 
* `restart` (Nullable<Int64>): File byte offset to restart from.
* `with_rename` (bool): Allow file rename instead of overwrite?
