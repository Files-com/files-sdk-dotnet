# FilesCom.Models.Group

## Example Group Object

```
{
  "id": 1,
  "name": "owners",
  "admin_ids": "1",
  "notes": "example",
  "user_ids": "1",
  "usernames": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Group ID
* `name` / `Name`  (string): Group name
* `admin_ids` / `AdminIds`  (string): Comma-delimited list of user IDs who are group administrators (separated by commas)
* `notes` / `Notes`  (string): Notes about this group
* `user_ids` / `UserIds`  (string): Comma-delimited list of user IDs who belong to this group (separated by commas)
* `usernames` / `Usernames`  (string): Comma-delimited list of usernames who belong to this group (separated by commas)


---

## List Groups

```
Task<Group[]> Group.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[name]=desc`). Valid fields are `name`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `name`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `name`.
* `ids` (string): Comma-separated list of group ids to include in results.


---

## Show Group

```
Task<Group> Group.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Group ID.


---

## Create Group

```
Task<Group> Group.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Group name.
* `notes` (string): Group notes.
* `user_ids` (string): A list of user ids. If sent as a string, should be comma-delimited.
* `admin_ids` (string): A list of group admin user ids. If sent as a string, should be comma-delimited.


---

## Update Group

```
Task<Group> Group.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Group ID.
* `name` (string): Group name.
* `notes` (string): Group notes.
* `user_ids` (string): A list of user ids. If sent as a string, should be comma-delimited.
* `admin_ids` (string): A list of group admin user ids. If sent as a string, should be comma-delimited.


---

## Delete Group

```
Task<Group> Group.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Group ID.


---

## Update Group

```
var Group = Group.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("name", "owners");
parameters.Add("notes", "example");
parameters.Add("user_ids", "1");
parameters.Add("admin_ids", "1");

Group.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Group ID.
* `name` (string): Group name.
* `notes` (string): Group notes.
* `user_ids` (string): A list of user ids. If sent as a string, should be comma-delimited.
* `admin_ids` (string): A list of group admin user ids. If sent as a string, should be comma-delimited.


---

## Delete Group

```
var Group = Group.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Group.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Group ID.
