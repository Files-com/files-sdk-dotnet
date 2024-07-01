# FilesCom.Models.MessageCommentReaction

## Example MessageCommentReaction Object

```
{
  "id": 1,
  "emoji": "👍"
}
```

* `id` / `Id`  (Nullable<Int64>): Reaction ID
* `emoji` / `Emoji`  (string): Emoji used in the reaction.
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.


---

## List Message Comment Reactions

```
Task<FilesList<MessageCommentReaction>> MessageCommentReaction.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): 
* `page` (Nullable<Int64>): 
* `message_comment_id` (Nullable<Int64>): Required - Message comment to return reactions for.


---

## Show Message Comment Reaction

```
Task<MessageCommentReaction> MessageCommentReaction.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Comment Reaction ID.


---

## Create Message Comment Reaction

```
Task<MessageCommentReaction> MessageCommentReaction.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `emoji` (string): Required - Emoji to react with.


---

## Delete Message Comment Reaction

```
Task MessageCommentReaction.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Comment Reaction ID.


---

## Delete Message Comment Reaction

```
var MessageCommentReaction = MessageCommentReaction.Find(1);

var parameters = new Dictionary<string, object>();


MessageCommentReaction.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Comment Reaction ID.
