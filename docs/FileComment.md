# FilesCom.Models.FileComment

## Example FileComment Object

```
{
  "id": 1,
  "body": "What a great file!",
  "reactions": [
    {
      "id": 1,
      "emoji": "👍"
    }
  ]
}
```

* `id` / `Id`  (Nullable<Int64>): File Comment ID
* `body` / `Body`  (string): Comment body.
* `reactions` / `Reactions`  (string[]): Reactions to this comment.
* `path` / `Path`  (string): File path.


---

## List File Comments by path

```
Task<FileComment[]> FileComment.ListFor(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `path` (string): Required - Path to operate on.


---

## Create File Comment

```
Task<FileComment> FileComment.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `body` (string): Required - Comment body.
* `path` (string): Required - File path.


---

## Update File Comment

```
Task<FileComment> FileComment.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - File Comment ID.
* `body` (string): Required - Comment body.


---

## Delete File Comment

```
Task<FileComment> FileComment.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - File Comment ID.


---

## Update File Comment

```
var FileComment = FileComment.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("body", "body");

FileComment.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - File Comment ID.
* `body` (string): Required - Comment body.


---

## Delete File Comment

```
var FileComment = FileComment.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


FileComment.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - File Comment ID.
