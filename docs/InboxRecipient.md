# FilesCom.Models.InboxRecipient

## Example InboxRecipient Object

```
{
  "company": "Acme Inc.",
  "name": "John Doe",
  "note": "Some note.",
  "recipient": "john.doe@example.com",
  "sent_at": "2000-01-01T01:00:00Z"
}
```

* `company` / `Company`  (string): The recipient's company.
* `name` / `Name`  (string): The recipient's name.
* `note` / `Note`  (string): A note sent to the recipient with the inbox.
* `recipient` / `Recipient`  (string): The recipient's email address.
* `sent_at` / `SentAt`  (Nullable<DateTime>): When the Inbox was shared with this recipient.
* `inbox_id` / `InboxId`  (Nullable<Int64>): Inbox to share.
* `share_after_create` / `ShareAfterCreate`  (bool): Set to true to share the link with the recipient upon creation.


---

## List Inbox Recipients

```
Task<FilesList<InboxRecipient>> InboxRecipient.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): 
* `page` (Nullable<Int64>): 
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[has_registrations]=desc`). Valid fields are `has_registrations`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `has_registrations`.
* `inbox_id` (Nullable<Int64>): Required - List recipients for the inbox with this ID.


---

## Create Inbox Recipient

```
Task<InboxRecipient> InboxRecipient.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `inbox_id` (Nullable<Int64>): Required - Inbox to share.
* `recipient` (string): Required - Email address to share this inbox with.
* `name` (string): Name of recipient.
* `company` (string): Company of recipient.
* `note` (string): Note to include in email.
* `share_after_create` (bool): Set to true to share the link with the recipient upon creation.
