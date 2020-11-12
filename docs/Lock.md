# Files.Models.Lock

## Example Lock Object

```
{
  "path": "locked_file",
  "timeout": 43200,
  "depth": "infinity",
  "owner": "user",
  "scope": "shared",
  "token": "17c54824e9931a4688ca032d03f6663c",
  "type": "write",
  "user_id": 1,
  "username": ""
}
```

* `path` / `Path`  (string): Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `timeout` / `Timeout`  (Nullable<Int64>): Lock timeout
* `depth` / `Depth`  (string): Lock depth (0 or infinity)
* `owner` / `Owner`  (string): Owner of lock.  This can be any arbitrary string.
* `scope` / `Scope`  (string): Lock scope(shared or exclusive)
* `token` / `Token`  (string): Lock token.  Use to release lock.
* `type` / `Type`  (string): Lock type
* `user_id` / `UserId`  (Nullable<Int64>): Lock creator user ID
* `username` / `Username`  (string): Lock creator username


---

## List Locks by path

```
Task<Lock[]> Lock.ListFor(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `path` (string): Required - Path to operate on.
* `include_children` (bool): Include locks from children objects?


---

## Create Lock

```
Task<Lock> Lock.Create(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path
* `timeout` (Nullable<Int64>): Lock timeout length


---

## Delete Lock

```
Task<Lock> Lock.Delete(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path
* `token` (string): Required - Lock token


---

## Delete Lock

```
var Lock = Lock.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("token", "token");

Lock.Delete(parameters);
```

### Parameters

* `path` (string): Required - Path
* `token` (string): Required - Lock token
