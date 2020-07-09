# Files.Models.Request

## Example Request Object

```
{
  "id": 1,
  "path": "",
  "source": "",
  "destination": "",
  "automation_id": "",
  "user_display_name": ""
}
```

* `id` / `Id`  (Nullable<Int64>): Request ID
* `path` / `Path`  (string): Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `source` / `Source`  (string): Source filename, if applicable
* `destination` / `Destination`  (string): Destination filename
* `automation_id` / `AutomationId`  (string): ID of automation that created request
* `user_display_name` / `UserDisplayName`  (string): User making the request (if applicable)
* `user_ids` / `UserIds`  (string): A list of user IDs to request the file from. If sent as a string, it should be comma-delimited.
* `group_ids` / `GroupIds`  (string): A list of group IDs to request the file from. If sent as a string, it should be comma-delimited.


---

## List Requests

```
Task<Request[]> Request.List(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `mine` (bool): Only show requests of the current user?  (Defaults to true if current user is not a site admin.)
* `path` (string): Path to show requests for.  If omitted, shows all paths. Send `/` to represent the root directory.


---

## List Requests

```
Task<Request[]> Request.Folders(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `mine` (bool): Only show requests of the current user?  (Defaults to true if current user is not a site admin.)
* `path` (string): Path to show requests for.  If omitted, shows all paths. Send `/` to represent the root directory.


---

## Create Request

```
Task<Request> Request.Create(
    string path, 
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
Task<Request> Request.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Request ID.


---

## List Requests

```
var Request = Request.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("page", 1);
parameters.Add("per_page", 1);
parameters.Add("mine", true);

Request.Folders(parameters);
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `mine` (bool): Only show requests of the current user?  (Defaults to true if current user is not a site admin.)
* `path` (string): Path to show requests for.  If omitted, shows all paths. Send `/` to represent the root directory.
