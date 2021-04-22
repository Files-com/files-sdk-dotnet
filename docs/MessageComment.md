# FilesCom.Models.MessageComment

## Example MessageComment Object

```
{
  "id": 1,
  "body": "What a great idea, thank you!",
  "reactions": {
    "id": 1,
    "emoji": "👍"
  }
}
```

* `id` / `Id`  (Nullable<Int64>): Message Comment ID
* `body` / `Body`  (string): Comment body.
* `reactions` / `Reactions`  (object): Reactions to this comment.
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.


---

## List Message Comments

```
Task<MessageComment[]> MessageComment.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `message_id` (Nullable<Int64>): Required - Message comment to return comments for.


---

## Show Message Comment

```
Task<MessageComment> MessageComment.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Comment ID.


---

## Create Message Comment

```
Task<MessageComment> MessageComment.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `body` (string): Required - Comment body.


---

## Update Message Comment

```
Task<MessageComment> MessageComment.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Comment ID.
* `body` (string): Required - Comment body.


---

## Delete Message Comment

```
Task<MessageComment> MessageComment.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Comment ID.


---

## Update Message Comment

```
var MessageComment = MessageComment.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("body", "body");

MessageComment.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Comment ID.
* `body` (string): Required - Comment body.


---

## Delete Message Comment

```
var MessageComment = MessageComment.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


MessageComment.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Message Comment ID.
