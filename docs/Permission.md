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
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `path` (string): Permission path.  If provided, will scope permissions to this path.
* `group_id` (string): Group ID.  If provided, will scope permissions to this group.
* `user_id` (string): User ID.  If provided, will scope permissions to this user.
* `include_groups` (bool): If searching by user or group, also include user's permissions that are inherited from its groups?


---

## Create Permission

```
Task<Permission> Permission.Create(
    string path, 
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
