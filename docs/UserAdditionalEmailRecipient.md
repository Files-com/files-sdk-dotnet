# FilesCom.Models.UserAdditionalEmailRecipient

## Example UserAdditionalEmailRecipient Object

```
{
  "id": 1,
  "user_id": 1,
  "workspace_id": 1,
  "email": "user-copy@example.com",
  "created_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): User additional email recipient ID
* `user_id` / `UserId`  (Nullable<Int64>): User ID
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID (0 for default workspace).
* `email` / `Email`  (string): Additional email recipient address
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Created at date/time


---

## List User Additional Email Recipients

```
Task<FilesList<UserAdditionalEmailRecipient>> UserAdditionalEmailRecipient.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `email`, `user_id` or `workspace_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `email` and `workspace_id`. Valid field combinations are `[ workspace_id, email ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `email`.


---

## Show User Additional Email Recipient

```
Task<UserAdditionalEmailRecipient> UserAdditionalEmailRecipient.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User Additional Email Recipient ID.


---

## Create User Additional Email Recipient

```
Task<UserAdditionalEmailRecipient> UserAdditionalEmailRecipient.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `email` (string): Required - Additional email recipient address


---

## Update User Additional Email Recipient

```
Task<UserAdditionalEmailRecipient> UserAdditionalEmailRecipient.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User Additional Email Recipient ID.
* `email` (string): Additional email recipient address


---

## Delete User Additional Email Recipient

```
Task UserAdditionalEmailRecipient.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User Additional Email Recipient ID.


---

## Update User Additional Email Recipient

```
var UserAdditionalEmailRecipient = UserAdditionalEmailRecipient.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("email", "user-copy@example.com");

UserAdditionalEmailRecipient.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - User Additional Email Recipient ID.
* `email` (string): Additional email recipient address


---

## Delete User Additional Email Recipient

```
var UserAdditionalEmailRecipient = UserAdditionalEmailRecipient.Find(1);

var parameters = new Dictionary<string, object>();


UserAdditionalEmailRecipient.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - User Additional Email Recipient ID.
