# FilesCom.Models.BundleAction

## Example BundleAction Object

```
{
  "action": "create",
  "bundle_registration": {
    "code": "abc123",
    "name": "account",
    "company": "Action Verb",
    "email": "john.doe@files.com",
    "ip": "10.1.1.1",
    "inbox_code": "abc123",
    "clickwrap_body": "example",
    "form_field_set_id": 1,
    "form_field_data": {
      "key": "example value"
    },
    "bundle_code": "example",
    "bundle_id": 1,
    "bundle_recipient_id": 1,
    "created_at": "2000-01-01T01:00:00Z"
  },
  "created_at": "2000-01-01T01:00:00Z",
  "destination": "/to_path",
  "path": "",
  "source": "/from_path",
  "bundle_registration_id": 1,
  "bundle_registration_name": "John Doe",
  "bundle_registration_email": "john@example.com",
  "bundle_registration_ip": "127.0.0.1"
}
```

* `action` / `Action`  (string): Type of action
* `bundle_registration` / `BundleRegistration`  (BundleRegistration): Object that contains bundle registration information
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Action occurrence date/time
* `destination` / `Destination`  (string): The destination path for this bundle action, if applicable
* `path` / `Path`  (string): Path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `source` / `Source`  (string): The source path for this bundle action, if applicable
* `bundle_registration_id` / `BundleRegistrationId`  (Nullable<Int64>): Identifier of the associated bundle registration
* `bundle_registration_name` / `BundleRegistrationName`  (string): Name of the registrant who performed the action
* `bundle_registration_email` / `BundleRegistrationEmail`  (string): Email of the registrant
* `bundle_registration_ip` / `BundleRegistrationIp`  (string): IP address of the registrant


---

## List Bundle Actions

```
Task<FilesList<BundleAction>> BundleAction.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `created_at` and `bundle_registration_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `created_at`, `bundle_id` or `bundle_registration_id`. Valid field combinations are `[ bundle_id, created_at ]`, `[ bundle_registration_id, created_at ]`, `[ bundle_id, bundle_registration_id ]` or `[ bundle_id, bundle_registration_id, created_at ]`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `created_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `created_at`.
