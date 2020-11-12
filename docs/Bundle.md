# Files.Models.Bundle

## Example Bundle Object

```
{
  "code": "abc123",
  "url": "https://subdomain.files.com/f/12345678",
  "description": "The public description of the bundle.",
  "password_protected": true,
  "require_registration": true,
  "require_share_recipient": true,
  "clickwrap_body": "[Legal text]",
  "id": 1,
  "created_at": "2000-01-01T01:00:00Z",
  "expires_at": "2000-01-01T01:00:00Z",
  "max_uses": 1,
  "note": "The internal note on the bundle.",
  "user_id": 1,
  "username": "user",
  "clickwrap_id": 1,
  "inbox_id": 1,
  "has_inbox": true,
  "paths": [

  ]
}
```

* `code` / `Code`  (string): Bundle code.  This code forms the end part of the Public URL.
* `url` / `Url`  (string): Public URL of Share Link
* `description` / `Description`  (string): Public description
* `password_protected` / `PasswordProtected`  (bool): Is this bundle password protected?
* `require_registration` / `RequireRegistration`  (bool): Show a registration page that captures the downloader's name and email address?
* `require_share_recipient` / `RequireShareRecipient`  (bool): Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?
* `clickwrap_body` / `ClickwrapBody`  (string): Legal text that must be agreed to prior to accessing Bundle.
* `id` / `Id`  (Nullable<Int64>): Bundle ID
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Bundle created at date/time
* `expires_at` / `ExpiresAt`  (Nullable<DateTime>): Bundle expiration date/time
* `max_uses` / `MaxUses`  (Nullable<Int64>): Maximum number of times bundle can be accessed
* `note` / `Note`  (string): Bundle internal note
* `user_id` / `UserId`  (Nullable<Int64>): Bundle creator user ID
* `username` / `Username`  (string): Bundle creator username
* `clickwrap_id` / `ClickwrapId`  (Nullable<Int64>): ID of the clickwrap to use with this bundle.
* `inbox_id` / `InboxId`  (Nullable<Int64>): ID of the associated inbox, if available.
* `has_inbox` / `HasInbox`  (bool): Does this bundle have an associated inbox?
* `paths` / `Paths`  (string[]): A list of paths in this bundle
* `password` / `Password`  (string): Password for this bundle.


---

## List Bundles

```
Task<Bundle[]> Bundle.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `site_id`, `created_at` or `code`.
* `filter` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `created_at`.
* `filter_gt` (object): If set, return records where the specifiied field is greater than the supplied value. Valid fields are `created_at`.
* `filter_gteq` (object): If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `created_at`.
* `filter_like` (object): If set, return records where the specifiied field is equal to the supplied value. Valid fields are `created_at`.
* `filter_lt` (object): If set, return records where the specifiied field is less than the supplied value. Valid fields are `created_at`.
* `filter_lteq` (object): If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `created_at`.


---

## Show Bundle

```
Task<Bundle> Bundle.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.


---

## Create Bundle

```
Task<Bundle> Bundle.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `paths` (string[]): Required - A list of paths to include in this bundle.
* `password` (string): Password for this bundle.
* `expires_at` (string): Bundle expiration date/time
* `max_uses` (Nullable<Int64>): Maximum number of times bundle can be accessed
* `description` (string): Public description
* `note` (string): Bundle internal note
* `code` (string): Bundle code.  This code forms the end part of the Public URL.
* `require_registration` (bool): Show a registration page that captures the downloader's name and email address?
* `clickwrap_id` (Nullable<Int64>): ID of the clickwrap to use with this bundle.
* `inbox_id` (Nullable<Int64>): ID of the associated inbox, if available.
* `require_share_recipient` (bool): Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?


---

## Send email(s) with a link to bundle

```
Task<Bundle> Bundle.Share(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
* `to` (string[]): Required - A list of email addresses to share this bundle with.
* `note` (string): Note to include in email.
* `recipients` (object[]): A list of recipients to share this bundle with.


---

## Update Bundle

```
Task<Bundle> Bundle.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
* `password` (string): Password for this bundle.
* `clickwrap_id` (Nullable<Int64>): ID of the clickwrap to use with this bundle.
* `code` (string): Bundle code.  This code forms the end part of the Public URL.
* `description` (string): Public description
* `expires_at` (string): Bundle expiration date/time
* `inbox_id` (Nullable<Int64>): ID of the associated inbox, if available.
* `max_uses` (Nullable<Int64>): Maximum number of times bundle can be accessed
* `note` (string): Bundle internal note
* `require_registration` (bool): Show a registration page that captures the downloader's name and email address?
* `require_share_recipient` (bool): Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?


---

## Delete Bundle

```
Task<Bundle> Bundle.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.


---

## Send email(s) with a link to bundle

```
var Bundle = Bundle.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("to", ["johndoe@gmail.com"]);
parameters.Add("note", "Just a note.");
parameters.Add("recipients", [{"name":"John Doe","company":"Acme Ltd","recipient":"johndoe@gmail.com"}]);

Bundle.Share(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
* `to` (string[]): Required - A list of email addresses to share this bundle with.
* `note` (string): Note to include in email.
* `recipients` (object[]): A list of recipients to share this bundle with.


---

## Update Bundle

```
var Bundle = Bundle.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("password", "Password");
parameters.Add("clickwrap_id", 1);
parameters.Add("code", "abc123");
parameters.Add("description", "The public description of the bundle.");
parameters.Add("expires_at", "2000-01-01T01:00:00Z");
parameters.Add("inbox_id", 1);
parameters.Add("max_uses", 1);
parameters.Add("note", "The internal note on the bundle.");
parameters.Add("require_registration", true);
parameters.Add("require_share_recipient", true);

Bundle.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
* `password` (string): Password for this bundle.
* `clickwrap_id` (Nullable<Int64>): ID of the clickwrap to use with this bundle.
* `code` (string): Bundle code.  This code forms the end part of the Public URL.
* `description` (string): Public description
* `expires_at` (string): Bundle expiration date/time
* `inbox_id` (Nullable<Int64>): ID of the associated inbox, if available.
* `max_uses` (Nullable<Int64>): Maximum number of times bundle can be accessed
* `note` (string): Bundle internal note
* `require_registration` (bool): Show a registration page that captures the downloader's name and email address?
* `require_share_recipient` (bool): Only allow access to recipients who have explicitly received the share via an email sent through the Files.com UI?


---

## Delete Bundle

```
var Bundle = Bundle.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Bundle.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle ID.
