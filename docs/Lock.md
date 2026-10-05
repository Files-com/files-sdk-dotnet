# FilesCom.Models.Lock

## Example Lock Object

```
{
  "path": "locked_file",
  "timeout": 1,
  "depth": "infinity",
  "recursive": true,
  "owner": "user",
  "scope": "shared",
  "exclusive": false,
  "token": "17c54824e9931a4688ca032d03f6663c",
  "type": "write",
  "allow_access_by_any_user": false,
  "user_id": 1,
  "username": ""
}
```

* `path` / `Path`  (string): Path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `timeout` / `Timeout`  (Nullable<Int64>): Lock timeout in seconds
* `depth` / `Depth`  (string): 
* `recursive` / `Recursive`  (bool): Does lock apply to subfolders?
* `owner` / `Owner`  (string): Arbitrary descriptive label for the lock. Does not change the lock creator or permissions.
* `scope` / `Scope`  (string): 
* `exclusive` / `Exclusive`  (bool): Is lock exclusive?
* `token` / `Token`  (string): Lock token.  Use to release lock.
* `type` / `Type`  (string): 
* `allow_access_by_any_user` / `AllowAccessByAnyUser`  (bool): Can lock be modified by users other than its creator?
* `user_id` / `UserId`  (Nullable<Int64>): Lock creator user ID
* `username` / `Username`  (string): Lock creator username
* `expected_token` / `ExpectedToken`  (string): Require this existing, unexpired token before refreshing or replacing a lock. Set token to the same value to refresh, or a different value to replace.


---

## List Locks by Path

```
Task<FilesList<Lock>> Lock.ListFor(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<Lock> client.Locks.ListFor(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
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

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Lock> client.Locks.CreateAsync(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `path` (string): Required - Path
* `token` (string): Lock token. With expected_token, use the same value to refresh or a different value to replace the existing token.
* `expected_token` (string): Require this existing, unexpired token before refreshing or replacing a lock. Set token to the same value to refresh, or a different value to replace.
* `allow_access_by_any_user` (bool): Can lock be modified by users other than its creator?
* `exclusive` (bool): Is lock exclusive?
* `recursive` (bool): Does lock apply to subfolders?
* `owner` (string): Arbitrary descriptive label for the lock. Does not change the lock creator or permissions.
* `timeout` (Nullable<Int64>): Lock timeout in seconds


---

## Delete Lock

```
Task Lock.Delete(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Locks.DeleteAsync(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
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

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `path` (string): Required - Path
* `token` (string): Required - Lock token
