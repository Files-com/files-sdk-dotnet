# FilesCom.Models.UserRequest

## Example UserRequest Object

```
{
  "id": 1,
  "name": "John Doe",
  "email": "example",
  "details": "Changed Departments",
  "company": "Acme Inc."
}
```

* `id` / `Id`  (Nullable<Int64>): ID
* `name` / `Name`  (string): User's full name
* `email` / `Email`  (string): User email address
* `details` / `Details`  (string): Details of the user's request
* `company` / `Company`  (string): User's company name


---

## List User Requests

```
Task<FilesList<UserRequest>> UserRequest.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
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
* `company` (string): Company of the user requested


---

## Create an export CSV of User Request resources

```
Task<Export> UserRequest.CreateExport(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```


---

## Delete User Request

```
Task UserRequest.Delete(
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
var UserRequest = UserRequest.Find(1);

var parameters = new Dictionary<string, object>();


UserRequest.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - User Request ID.
