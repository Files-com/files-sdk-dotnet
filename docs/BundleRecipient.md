# Files.Models.BundleRecipient

## Example BundleRecipient Object

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
* `note` / `Note`  (string): A note sent to the recipient with the bundle.
* `recipient` / `Recipient`  (string): The recipient's email address.
* `sent_at` / `SentAt`  (Nullable<DateTime>): When the Bundle was shared with this recipient.


---

## List Bundle Recipients

```
Task<BundleRecipient[]> BundleRecipient.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `bundle_id` (Nullable<Int64>): Required - List recipients for the bundle with this ID.
