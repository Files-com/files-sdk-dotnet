# FilesCom.Models.Request

## Example Request Object

```
{
  "id": 1,
  "path": "example",
  "source": "example",
  "destination": "example",
  "automation_id": "example",
  "user_display_name": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Request ID
* `path` / `Path`  (string): Folder path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `source` / `Source`  (string): Source filename, if applicable
* `destination` / `Destination`  (string): Destination filename
* `automation_id` / `AutomationId`  (string): ID of automation that created request
* `user_display_name` / `UserDisplayName`  (string): User making the request (if applicable)
* `user_ids` / `UserIds`  (string): A list of user IDs to request the file from. If sent as a string, it should be comma-delimited.
* `group_ids` / `GroupIds`  (string): A list of group IDs to request the file from. If sent as a string, it should be comma-delimited.


---

## List Requests

```
Task<FilesList<Request>> Request.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
* `mine` (bool): Only show requests of the current user?  (Defaults to true if current user is not a site admin.)
* `path` (string): Path to show requests for.  If omitted, shows all paths. Send `/` to represent the root directory.


---

## List Requests

```
Task<FilesList<Request>> Request.GetFolder(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
* `mine` (bool): Only show requests of the current user?  (Defaults to true if current user is not a site admin.)
* `path` (string): Required - Path to show requests for.  If omitted, shows all paths. Send `/` to represent the root directory.


---

## Create Request

```
Task<Request> Request.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Folder path on which to request the file.
* `destination` (string): Required - Destination filename (without extension) to request.
* `user_ids` (string): A list of user IDs to request the file from. If sent as a string, it should be comma-delimited.
* `group_ids` (string): A list of group IDs to request the file from. If sent as a string, it should be comma-delimited.


---

## Delete Request

```
Task Request.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Request ID.


---

## Delete Request

```
var Request = Request.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Request.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Request ID.
