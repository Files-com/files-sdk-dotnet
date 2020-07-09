# Files.Models.FileCommentReaction

## Example FileCommentReaction Object

```
{
  "id": 1,
  "emoji": "👍"
}
```

* `id` / `Id`  (Nullable<Int64>): Reaction ID
* `emoji` / `Emoji`  (string): Emoji used in the reaction.
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `file_comment_id` / `FileCommentId`  (Nullable<Int64>): ID of file comment to attach reaction to.


---

## Create File Comment Reaction

```
Task<FileCommentReaction> FileCommentReaction.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `file_comment_id` (Nullable<Int64>): Required - ID of file comment to attach reaction to.
* `emoji` (string): Required - Emoji to react with.


---

## Delete File Comment Reaction

```
Task<FileCommentReaction> FileCommentReaction.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - File Comment Reaction ID.


---

## Delete File Comment Reaction

```
var FileCommentReaction = FileCommentReaction.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


FileCommentReaction.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - File Comment Reaction ID.
