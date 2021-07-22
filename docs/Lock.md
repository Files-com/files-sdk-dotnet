# FilesCom.Models.Lock

## Example Lock Object

```
{
  "path": "locked_file",
  "timeout": 43200,
  "depth": "infinity",
  "recursive": true,
  "owner": "user",
  "scope": "shared",
  "exclusive": true,
  "token": "17c54824e9931a4688ca032d03f6663c",
  "type": "write",
  "allow_access_by_any_user": true,
  "user_id": 1,
  "username": ""
}
```

* `path` / `Path`  (string): Path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `timeout` / `Timeout`  (Nullable<Int64>): Lock timeout in seconds
* `depth` / `Depth`  (string): DEPRECATED: Lock depth
* `recursive` / `Recursive`  (bool): Does lock apply to subfolders?
* `owner` / `Owner`  (string): Owner of the lock.  This can be any arbitrary string.
* `scope` / `Scope`  (string): DEPRECATED: Lock scope
* `exclusive` / `Exclusive`  (bool): Is lock exclusive?
* `token` / `Token`  (string): Lock token.  Use to release lock.
* `type` / `Type`  (string): DEPRECATED: Lock type
* `allow_access_by_any_user` / `AllowAccessByAnyUser`  (bool): Can lock be modified by users other than its creator?
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
* `allow_access_by_any_user` (bool): Allow lock to be updated by any user?
* `exclusive` (bool): Is lock exclusive?
* `recursive` (string): Does lock apply to subfolders?
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
