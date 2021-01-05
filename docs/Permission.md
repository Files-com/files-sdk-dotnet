# Files.Models.Permission

## Example Permission Object

```
{
  "id": 1,
  "path": "",
  "user_id": 1,
  "username": "Sser",
  "group_id": 1,
  "group_name": "",
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
Task<Permission[]> Permission.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `group_id`, `path`, `user_id` or `permission`.
* `filter` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `group_id`, `user_id` or `path`.
* `filter_gt` (object): If set, return records where the specifiied field is greater than the supplied value. Valid fields are `group_id`, `user_id` or `path`.
* `filter_gteq` (object): If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `group_id`, `user_id` or `path`.
* `filter_like` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `group_id`, `user_id` or `path`.
* `filter_lt` (object): If set, return records where the specifiied field is less than the supplied value. Valid fields are `group_id`, `user_id` or `path`.
* `filter_lteq` (object): If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `group_id`, `user_id` or `path`.
* `path` (string): DEPRECATED: Permission path.  If provided, will scope permissions to this path. Use `filter[path]` instead.
* `group_id` (string): DEPRECATED: Group ID.  If provided, will scope permissions to this group. Use `filter[group_id]` instead.`
* `user_id` (string): DEPRECATED: User ID.  If provided, will scope permissions to this user. Use `filter[user_id]` instead.`
* `include_groups` (bool): If searching by user or group, also include user's permissions that are inherited from its groups?


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
Task<Permission> Permission.Delete(
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
