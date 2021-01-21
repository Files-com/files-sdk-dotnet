# Files.Models.InboxRecipient

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
* `user_id` / `UserId`  (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `inbox_id` / `InboxId`  (Nullable<Int64>): Inbox to share.
* `share_after_create` / `ShareAfterCreate`  (bool): Set to true to share the link with the recipient upon creation.


---

## List Inbox Recipients

```
Task<InboxRecipient[]> InboxRecipient.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `has_registrations`.
* `filter` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `has_registrations`.
* `filter_gt` (object): If set, return records where the specifiied field is greater than the supplied value. Valid fields are `has_registrations`.
* `filter_gteq` (object): If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `has_registrations`.
* `filter_like` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `has_registrations`.
* `filter_lt` (object): If set, return records where the specifiied field is less than the supplied value. Valid fields are `has_registrations`.
* `filter_lteq` (object): If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `has_registrations`.
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

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `inbox_id` (Nullable<Int64>): Required - Inbox to share.
* `recipient` (string): Required - Email addresses to share this inbox with.
* `name` (string): Name of recipient.
* `company` (string): Company of recipient.
* `note` (string): Note to include in email.
* `share_after_create` (bool): Set to true to share the link with the recipient upon creation.
