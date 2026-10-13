# FilesCom.Models.GroupUser

## Example GroupUser Object

```
{
  "group_name": "My Group",
  "group_id": 1,
  "user_id": 1,
  "admin": true,
  "username": "example"
}
```

* `group_name` / `GroupName`  (string): Group name
* `group_id` / `GroupId`  (Nullable<Int64>): Group ID
* `user_id` / `UserId`  (Nullable<Int64>): User ID
* `admin` / `Admin`  (bool): Is this user an administrator of this group?
* `username` / `Username`  (string): Username of the user
* `id` / `Id`  (Nullable<Int64>): Group User ID.


---

## List Group Users

```
FilesList<GroupUser> GroupUser.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<GroupUser> client.GroupUsers.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `group_id` (Nullable<Int64>): Group ID. If provided, returns memberships of this group. Requires a Site Administrator, a Read-only Administrator, a Workspace Administrator for the group's workspace, or a Group Administrator of this group.
* `user_id` (Nullable<Int64>): User ID.  If provided, will return group_users of this user.


---

## Create Group User

```
Task<GroupUser> GroupUser.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<GroupUser> client.GroupUsers.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<GroupUser> client.GroupUsers.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.GroupUsers.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Group User ID.
* `group_id` (Nullable<Int64>): Required - Group ID from which to remove user.
* `user_id` (Nullable<Int64>): Required - User ID to remove from group.


---

## Update Group User

```
var groupUser = (await GroupUser.List().LoadNextPageAsync()).data[0];

var parameters = new Dictionary<string, object>();

parameters.Add("group_id", 1L);
parameters.Add("user_id", 1L);
parameters.Add("admin", false);

await groupUser.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `GroupUser` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Group User ID.
* `group_id` (Nullable<Int64>): Required - Group ID to add user to.
* `user_id` (Nullable<Int64>): Required - User ID to add to group.
* `admin` (bool): Is the user a group administrator?


---

## Delete Group User

```
var groupUser = (await GroupUser.List().LoadNextPageAsync()).data[0];

var parameters = new Dictionary<string, object>();

parameters.Add("group_id", 1L);
parameters.Add("user_id", 1L);

await groupUser.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Group User ID.
* `group_id` (Nullable<Int64>): Required - Group ID from which to remove user.
* `user_id` (Nullable<Int64>): Required - User ID to remove from group.


---

## Save

```
await groupUser.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
