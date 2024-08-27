# FilesCom.Models.ApiKey

## Example ApiKey Object

```
{
  "id": 1,
  "descriptive_label": "Site-wide API key for https://site.files.com/ (key ID #1)",
  "description": "example",
  "created_at": "2000-01-01T01:00:00Z",
  "expires_at": "2000-01-01T01:00:00Z",
  "key": "[key]",
  "last_use_at": "2000-01-01T01:00:00Z",
  "name": "My Main API Key",
  "permission_set": "full",
  "platform": "win32",
  "url": "example",
  "user_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): API Key ID
* `descriptive_label` / `DescriptiveLabel`  (string): Unique label that describes this API key.  Useful for external systems where you may have API keys from multiple accounts and want a human-readable label for each key.
* `description` / `Description`  (string): User-supplied description of API key.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Time which API Key was created
* `expires_at` / `ExpiresAt`  (Nullable<DateTime>): API Key expiration date
* `key` / `Key`  (string): API Key actual key string
* `last_use_at` / `LastUseAt`  (Nullable<DateTime>): API Key last used - note this value is only updated once per 3 hour period, so the 'actual' time of last use may be up to 3 hours later than this timestamp.
* `name` / `Name`  (string): Internal name for the API Key.  For your use.
* `permission_set` / `PermissionSet`  (string): Permissions for this API Key. It must be full for site-wide API Keys.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations). Keys with the `office_integration` permission set are auto generated, and automatically expire, to allow users to interact with office integration platforms. Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
* `platform` / `Platform`  (string): If this API key represents a Desktop app, what platform was it created on?
* `url` / `Url`  (string): URL for API host.
* `user_id` / `UserId`  (Nullable<Int64>): User ID for the owner of this API Key.  May be blank for Site-wide API Keys.
* `path` / `Path`  (string): Folder path restriction for `office_integration` permission set API keys.


---

## List API Keys

```
Task<FilesList<ApiKey>> ApiKey.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `expires_at`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `expires_at`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `expires_at`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal the supplied value. Valid fields are `expires_at`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `expires_at`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal the supplied value. Valid fields are `expires_at`.


---

## Show information about current API key.  (Requires current API connection to be using an API key.)

```
Task<ApiKey> ApiKey.FindCurrent(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```


---

## Show API Key

```
Task<ApiKey> ApiKey.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Api Key ID.


---

## Create API Key

```
Task<ApiKey> ApiKey.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `description` (string): User-supplied description of API key.
* `expires_at` (string): API Key expiration date
* `permission_set` (string): Permissions for this API Key. It must be full for site-wide API Keys.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations). Keys with the `office_integration` permission set are auto generated, and automatically expire, to allow users to interact with office integration platforms. Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
* `name` (string): Required - Internal name for the API Key.  For your use.
* `path` (string): Folder path restriction for `office_integration` permission set API keys.


---

## Update current API key.  (Requires current API connection to be using an API key.)

```
Task<ApiKey> ApiKey.UpdateCurrent(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `expires_at` (string): API Key expiration date
* `name` (string): Internal name for the API Key.  For your use.
* `permission_set` (string): Permissions for this API Key. It must be full for site-wide API Keys.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations). Keys with the `office_integration` permission set are auto generated, and automatically expire, to allow users to interact with office integration platforms. Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.


---

## Update API Key

```
Task<ApiKey> ApiKey.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Api Key ID.
* `description` (string): User-supplied description of API key.
* `expires_at` (string): API Key expiration date
* `permission_set` (string): Permissions for this API Key. It must be full for site-wide API Keys.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations). Keys with the `office_integration` permission set are auto generated, and automatically expire, to allow users to interact with office integration platforms. Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
* `name` (string): Internal name for the API Key.  For your use.


---

## Delete current API key.  (Requires current API connection to be using an API key.)

```
Task ApiKey.DeleteCurrent(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```


---

## Delete API Key

```
Task ApiKey.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Api Key ID.


---

## Update API Key

```
var ApiKey = ApiKey.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("description", "example");
parameters.Add("expires_at", "2000-01-01T01:00:00Z");
parameters.Add("permission_set", "full");
parameters.Add("name", "My Main API Key");

ApiKey.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Api Key ID.
* `description` (string): User-supplied description of API key.
* `expires_at` (string): API Key expiration date
* `permission_set` (string): Permissions for this API Key. It must be full for site-wide API Keys.  Keys with the `desktop_app` permission set only have the ability to do the functions provided in our Desktop App (File and Share Link operations). Keys with the `office_integration` permission set are auto generated, and automatically expire, to allow users to interact with office integration platforms. Additional permission sets may become available in the future, such as for a Site Admin to give a key with no administrator privileges.  If you have ideas for permission sets, please let us know.
* `name` (string): Internal name for the API Key.  For your use.


---

## Delete API Key

```
var ApiKey = ApiKey.Find(1);

var parameters = new Dictionary<string, object>();


ApiKey.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Api Key ID.
