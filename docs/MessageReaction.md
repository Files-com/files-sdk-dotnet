# FilesCom.Models.MessageReaction

## Example MessageReaction Object

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

## List Message Reactions

```
Task<MessageReaction[]> MessageReaction.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `message_id` (Nullable<Int64>): Required - Message to return reactions for.


---

## Show Message Reaction

```
Task<MessageReaction> MessageReaction.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Reaction ID.


---

## Create Message Reaction

```
Task<MessageReaction> MessageReaction.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `emoji` (string): Required - Emoji to react with.


---

## Delete Message Reaction

```
Task<MessageReaction> MessageReaction.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Reaction ID.


---

## Delete Message Reaction

```
var MessageReaction = MessageReaction.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


MessageReaction.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Reaction ID.
