# FilesCom.Models.Permission

## Example Permission Object

```
{
  "id": 1,
  "path": "example",
  "user_id": 1,
  "username": "Sser",
  "group_id": 1,
  "group_name": "example",
  "permission": "full",
  "recursive": true
}
```

* `id` / `Id`  (Nullable<Int64>): Permission ID
* `path` / `Path`  (string): Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `user_id` / `UserId`  (Nullable<Int64>): User ID
* `username` / `Username`  (string): User's username
* `group_id` / `GroupId`  (Nullable<Int64>): Group ID
* `group_name` / `GroupName`  (string): Group name if applicable
* `permission` / `PermissionType`  (string): Permission type
* `recursive` / `Recursive`  (bool): Does this permission apply to subfolders?


---

## List Permissions

```
Task<FilesList<Permission>> Permission.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): 
* `page` (Nullable<Int64>): 
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[group_id]=desc`). Valid fields are `group_id`, `path`, `user_id` or `permission`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `group_id` or `user_id`. Valid field combinations are `[ group_id, path ]`, `[ user_id, path ]` or `[ user_id, group_id, path ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
* `path` (string): Permission path.  If provided, will scope all permissions(including upward) to this path.
* `include_groups` (bool): If searching by user or group, also include user's permissions that are inherited from its groups?
* `group_id` (string): 
* `user_id` (string): 


---

## Create Permission

```
Task<Permission> Permission.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `group_id` (Nullable<Int64>): Group ID
* `path` (string): Folder path
* `permission` (string):  Permission type.  Can be `admin`, `full`, `readonly`, `writeonly`, `list`, or `history`
* `recursive` (bool): Apply to subfolders recursively?
* `user_id` (Nullable<Int64>): User ID.  Provide `username` or `user_id`
* `username` (string): User username.  Provide `username` or `user_id`


---

## Delete Permission

```
Task Permission.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Permission ID.


---

## Delete Permission

```
var Permission = Permission.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Permission.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Permission ID.
