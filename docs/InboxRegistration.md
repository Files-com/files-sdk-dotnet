# FilesCom.Models.InboxRegistration

## Example InboxRegistration Object

```
{
  "code": "abc123",
  "name": "account",
  "company": "Action Verb",
  "email": "john.doe@files.com",
  "clickwrap_body": "",
  "form_field_set_id": 1,
  "form_field_data": ""
}
```

* `code` / `Code`  (string): Registration cookie code
* `name` / `Name`  (string): Registrant name
* `company` / `Company`  (string): Registrant company name
* `email` / `Email`  (string): Registrant email address
* `clickwrap_body` / `ClickwrapBody`  (string): Clickwrap text that was shown to the registrant
* `form_field_set_id` / `FormFieldSetId`  (Nullable<Int64>): Id of associated form field set
* `form_field_data` / `FormFieldData`  (string): Data for form field set with form field ids as keys and user data as values


---

## List Inbox Registrations

```
Task<InboxRegistration[]> InboxRegistration.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `folder_behavior_id` (Nullable<Int64>): ID of the associated Inbox.
