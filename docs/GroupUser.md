# FilesCom.Models.GroupUser

## Example GroupUser Object

```
{
  "group_name": "example",
  "group_id": 1,
  "user_id": 1,
  "admin": true,
  "usernames": [
    "user"
  ]
}
```

* `group_name` / `GroupName`  (string): Group name
* `group_id` / `GroupId`  (Nullable<Int64>): Group ID
* `user_id` / `UserId`  (Nullable<Int64>): User ID
* `admin` / `Admin`  (bool): Is this user an administrator of this group?
* `usernames` / `Usernames`  (string[]): A list of usernames for users in this group
* `id` / `Id`  (Nullable<Int64>): Group User ID.


---

## List Group Users

```
Task<FilesList<GroupUser>> GroupUser.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  If provided, will return group_users of this user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `group_id` (Nullable<Int64>): Group ID.  If provided, will return group_users of this group.


---

## Create Group User

```
Task<GroupUser> GroupUser.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `group_id` (Nullable<Int64>): Required - Group ID to add user to.
* `user_id` (Nullable<Int64>): Required - User ID to add to group.
* `admin` (bool): Is the user a group administrator?


---

## Update Group User

```
Task<GroupUser> GroupUser.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Group User ID.
* `group_id` (Nullable<Int64>): Required - Group ID to add user to.
* `user_id` (Nullable<Int64>): Required - User ID to add to group.
* `admin` (bool): Is the user a group administrator?


---

## Delete Group User

```
Task GroupUser.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Group User ID.
* `group_id` (Nullable<Int64>): Required - Group ID from which to remove user.
* `user_id` (Nullable<Int64>): Required - User ID to remove from group.


---

## Update Group User

```
var GroupUser = GroupUser.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("group_id", 1);
parameters.Add("user_id", 1);
parameters.Add("admin", true);

GroupUser.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Group User ID.
* `group_id` (Nullable<Int64>): Required - Group ID to add user to.
* `user_id` (Nullable<Int64>): Required - User ID to add to group.
* `admin` (bool): Is the user a group administrator?


---

## Delete Group User

```
var GroupUser = GroupUser.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("group_id", 1);
parameters.Add("user_id", 1);

GroupUser.Delete(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Group User ID.
* `group_id` (Nullable<Int64>): Required - Group ID from which to remove user.
* `user_id` (Nullable<Int64>): Required - User ID to remove from group.
