# FilesCom.Models.FileAction

## Example FileAction Object

```
{
  "status": "pending",
  "file_migration_id": 1
}
```

* `status` / `Status`  (string): Status of file operation.
* `file_migration_id` / `FileMigrationId`  (Nullable<Int64>): If status is pending, this is the id of the File Migration to check for status updates.
