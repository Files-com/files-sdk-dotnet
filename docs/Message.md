# FilesCom.Models.Message

## Example Message Object

```
{
  "id": 1,
  "subject": "Files.com Account Upgrade",
  "body": "We should upgrade our Files.com account!",
  "comments": [
    {
      "id": 1,
      "body": "What a great idea, thank you!",
      "reactions": [
        {
          "id": 1,
          "emoji": "👍"
        }
      ]
    }
  ]
}
```

* `id` / `Id`  (Nullable<Int64>): Message ID
* `subject` / `Subject`  (string): Message subject.
* `body` / `Body`  (string): Message body.
* `comments` / `Comments`  (object[]): Comments.
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `project_id` / `ProjectId`  (Nullable<Int64>): Project to which the message should be attached.


---

## List Messages

```
Task<FilesList<Message>> Message.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `project_id` (Nullable<Int64>): Required - Project for which to return messages.


---

## Show Message

```
Task<Message> Message.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message ID.


---

## Create Message

```
Task<Message> Message.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `project_id` (Nullable<Int64>): Required - Project to which the message should be attached.
* `subject` (string): Required - Message subject.
* `body` (string): Required - Message body.


---

## Update Message

```
Task<Message> Message.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message ID.
* `project_id` (Nullable<Int64>): Required - Project to which the message should be attached.
* `subject` (string): Required - Message subject.
* `body` (string): Required - Message body.


---

## Delete Message

```
Task Message.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Message ID.


---

## Update Message

```
var Message = Message.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("project_id", 1);
parameters.Add("subject", "Files.com Account Upgrade");
parameters.Add("body", "We should upgrade our Files.com account!");

Message.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Message ID.
* `project_id` (Nullable<Int64>): Required - Project to which the message should be attached.
* `subject` (string): Required - Message subject.
* `body` (string): Required - Message body.


---

## Delete Message

```
var Message = Message.Find(1);

var parameters = new Dictionary<string, object>();


Message.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Message ID.
