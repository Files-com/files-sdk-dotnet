# FilesCom.Models.Priority

## Example Priority Object

```
{
  "path": "foo/bar",
  "color": "pink"
}
```

* `path` / `Path`  (string): The path corresponding to the priority color This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `color` / `Color`  (string): The priority color


---

## List Priorities

```
Task<Priority[]> Priority.List(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `path` (string): Required - The path to query for priorities
