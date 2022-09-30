# FilesCom.Models.InboxRegistration

## Example InboxRegistration Object

```
{
  "code": "abc123",
  "name": "account",
  "company": "Action Verb",
  "email": "john.doe@files.com",
  "clickwrap_body": "example",
  "form_field_set_id": 1,
  "form_field_data": {
    "key": "example value"
  },
  "inbox_id": 1,
  "inbox_recipient_id": 1,
  "inbox_title": "example"
}
```

* `code` / `Code`  (string): Registration cookie code
* `name` / `Name`  (string): Registrant name
* `company` / `Company`  (string): Registrant company name
* `email` / `Email`  (string): Registrant email address
* `clickwrap_body` / `ClickwrapBody`  (string): Clickwrap text that was shown to the registrant
* `form_field_set_id` / `FormFieldSetId`  (Nullable<Int64>): Id of associated form field set
* `form_field_data` / `FormFieldData`  (object): Data for form field set with form field ids as keys and user data as values
* `inbox_id` / `InboxId`  (Nullable<Int64>): Id of associated inbox
* `inbox_recipient_id` / `InboxRecipientId`  (Nullable<Int64>): Id of associated inbox recipient
* `inbox_title` / `InboxTitle`  (string): Title of associated inbox


---

## List Inbox Registrations

```
Task<InboxRegistration[]> InboxRegistration.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `folder_behavior_id` (Nullable<Int64>): ID of the associated Inbox.
