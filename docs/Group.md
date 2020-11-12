# Files.Models.Group

## Example Group Object

```
{
  "id": 1,
  "name": "owners",
  "admin_ids": [

  ],
  "notes": "",
  "user_ids": [

  ],
  "usernames": [

  ]
}
```

* `id` / `Id`  (Nullable<Int64>): Group ID
* `name` / `Name`  (string): Group name
* `admin_ids` / `AdminIds`  (string[]): List of user IDs who are group administrators (separated by commas)
* `notes` / `Notes`  (string): Notes about this group
* `user_ids` / `UserIds`  (Nullable<Int64>[]): List of user IDs who belong to this group (separated by commas)
* `usernames` / `Usernames`  (string[]): List of usernames who belong to this group (separated by commas)


---

## List Groups

```
Task<Group[]> Group.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `active`, `site_id` or `name`.
* `filter` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `name`.
* `filter_gt` (object): If set, return records where the specifiied field is greater than the supplied value. Valid fields are `name`.
* `filter_gteq` (object): If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `name`.
* `filter_like` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `name`.
* `filter_lt` (object): If set, return records where the specifiied field is less than the supplied value. Valid fields are `name`.
* `filter_lteq` (object): If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `name`.
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
