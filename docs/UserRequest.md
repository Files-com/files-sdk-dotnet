# FilesCom.Models.UserRequest

## Example UserRequest Object

```
{
  "id": 1,
  "name": "John Doe",
  "email": "example",
  "details": "Changed Departments"
}
```

* `id` / `Id`  (Nullable<Int64>): ID
* `name` / `Name`  (string): User's full name
* `email` / `Email`  (string): User email address
* `details` / `Details`  (string): Details of the user's request


---

## List User Requests

```
Task<UserRequest[]> UserRequest.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show User Request

```
Task<UserRequest> UserRequest.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User Request ID.


---

## Create User Request

```
Task<UserRequest> UserRequest.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - Name of user requested
* `email` (string): Required - Email of user requested
* `details` (string): Required - Details of the user request


---

## Delete User Request

```
Task<UserRequest> UserRequest.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User Request ID.


---

## Delete User Request

```
var UserRequest = UserRequest.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


UserRequest.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - User Request ID.
