# FilesCom.Models.ShareGroup

## Example ShareGroup Object

```
{
  "id": 1,
  "name": "Test group 1",
  "notes": "This group is defined for testing purposes",
  "user_id": 1,
  "members": [
    {
      "name": "John Doe",
      "company": "Acme Ltd",
      "email": "johndoe@gmail.com"
    }
  ]
}
```

* `id` / `Id`  (Nullable<Int64>): Share Group ID
* `name` / `Name`  (string): Name of the share group
* `notes` / `Notes`  (string): Additional notes of the share group
* `user_id` / `UserId`  (Nullable<Int64>): Owner User ID
* `members` / `Members`  (string[]): A list of share group members


---

## List Share Groups

```
Task<FilesList<ShareGroup>> ShareGroup.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Share Group

```
Task<ShareGroup> ShareGroup.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Share Group ID.


---

## Create Share Group

```
Task<ShareGroup> ShareGroup.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `notes` (string): Additional notes of the share group
* `name` (string): Required - Name of the share group
* `members` (object[]): Required - A list of share group members.


---

## Update Share Group

```
Task<ShareGroup> ShareGroup.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Share Group ID.
* `notes` (string): Additional notes of the share group
* `name` (string): Name of the share group
* `members` (object[]): A list of share group members.


---

## Delete Share Group

```
Task ShareGroup.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Share Group ID.


---

## Update Share Group

```
var ShareGroup = ShareGroup.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("notes", "This group is defined for testing purposes");
parameters.Add("name", "Test group 1");
parameters.Add("members", [{"name":"John Doe","company":"Acme Ltd","email":"johndoe@gmail.com"}]);

ShareGroup.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Share Group ID.
* `notes` (string): Additional notes of the share group
* `name` (string): Name of the share group
* `members` (object[]): A list of share group members.


---

## Delete Share Group

```
var ShareGroup = ShareGroup.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


ShareGroup.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Share Group ID.
