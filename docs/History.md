# Files.Models.History

## Example History Object

```
{
  "id": 1,
  "path": "path",
  "when": "2000-01-01T01:00:00Z",
  "destination": "/to_path",
  "display": "Actual text of the action here.",
  "ip": "192.283.128.182",
  "source": "/from_path",
  "targets": [

  ],
  "user_id": 1,
  "username": "user",
  "action": "create",
  "failure_type": "none",
  "interface": "web"
}
```

* `id` / `Id`  (Nullable<Int64>): Action ID
* `path` / `Path`  (string): Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `when` / `When`  (Nullable<DateTime>): Action occurrence date/time
* `destination` / `Destination`  (string): The destination path for this action, if applicable
* `display` / `Display`  (string): Friendly displayed output
* `ip` / `Ip`  (string): IP Address that performed this action
* `source` / `Source`  (string): The source path for this action, if applicable
* `targets` / `Targets`  (string[]): Targets
* `user_id` / `UserId`  (Nullable<Int64>): User ID
* `username` / `Username`  (string): Username
* `action` / `Action`  (string): Type of action
* `failure_type` / `FailureType`  (string): Failure type.  If action was a user login or session failure, why did it fail?
* `interface` / `Interface`  (string): Interface on which this action occurred.


---

## List history for specific file

```
Task<History[]> History.ListForFile(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `start_at` (string): Leave blank or set to a date/time to filter earlier entries.
* `end_at` (string): Leave blank or set to a date/time to filter later entries.
* `display` (string): Display format. Leave blank or set to `full` or `parent`.
* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `path` (string): Required - Path to operate on.


---

## List history for specific folder

```
Task<History[]> History.ListForFolder(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `start_at` (string): Leave blank or set to a date/time to filter earlier entries.
* `end_at` (string): Leave blank or set to a date/time to filter later entries.
* `display` (string): Display format. Leave blank or set to `full` or `parent`.
* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `path` (string): Required - Path to operate on.


---

## List history for specific user

```
Task<History[]> History.ListForUser(
    Nullable<Int64> user_id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `start_at` (string): Leave blank or set to a date/time to filter earlier entries.
* `end_at` (string): Leave blank or set to a date/time to filter later entries.
* `display` (string): Display format. Leave blank or set to `full` or `parent`.
* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `user_id` (Nullable<Int64>): Required - User ID.


---

## List site login history

```
Task<History[]> History.ListLogins(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `start_at` (string): Leave blank or set to a date/time to filter earlier entries.
* `end_at` (string): Leave blank or set to a date/time to filter later entries.
* `display` (string): Display format. Leave blank or set to `full` or `parent`.
* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.


---

## List site full action history

```
Task<History[]> History.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `start_at` (string): Leave blank or set to a date/time to filter earlier entries.
* `end_at` (string): Leave blank or set to a date/time to filter later entries.
* `display` (string): Display format. Leave blank or set to `full` or `parent`.
* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
