# FilesCom.Models.UserLifecycleRule

## Example UserLifecycleRule Object

```
{
  "id": 1,
  "authentication_method": "password",
  "inactivity_days": 12,
  "include_folder_admins": true,
  "include_site_admins": true,
  "action": "disable",
  "site_id": 1
}
```

* `id` / `Id`  (Nullable<Int64>): User Lifecycle Rule ID
* `authentication_method` / `AuthenticationMethod`  (string): User authentication method for the rule
* `inactivity_days` / `InactivityDays`  (Nullable<Int64>): Number of days of inactivity before the rule applies
* `include_folder_admins` / `IncludeFolderAdmins`  (bool): Include folder admins in the rule
* `include_site_admins` / `IncludeSiteAdmins`  (bool): Include site admins in the rule
* `action` / `Action`  (string): Action to take on inactive users (disable or delete)
* `site_id` / `SiteId`  (Nullable<Int64>): Site ID


---

## List User Lifecycle Rules

```
Task<FilesList<UserLifecycleRule>> UserLifecycleRule.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show User Lifecycle Rule

```
Task<UserLifecycleRule> UserLifecycleRule.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User Lifecycle Rule ID.


---

## Create User Lifecycle Rule

```
Task<UserLifecycleRule> UserLifecycleRule.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `action` (string): Required - Action to take on inactive users (disable or delete)
* `authentication_method` (string): Required - User authentication method for the rule
* `inactivity_days` (Nullable<Int64>): Required - Number of days of inactivity before the rule applies
* `include_site_admins` (bool): Include site admins in the rule
* `include_folder_admins` (bool): Include folder admins in the rule


---

## Update User Lifecycle Rule

```
Task<UserLifecycleRule> UserLifecycleRule.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User Lifecycle Rule ID.
* `action` (string): Required - Action to take on inactive users (disable or delete)
* `authentication_method` (string): Required - User authentication method for the rule
* `inactivity_days` (Nullable<Int64>): Required - Number of days of inactivity before the rule applies
* `include_site_admins` (bool): Include site admins in the rule
* `include_folder_admins` (bool): Include folder admins in the rule


---

## Delete User Lifecycle Rule

```
Task UserLifecycleRule.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - User Lifecycle Rule ID.


---

## Update User Lifecycle Rule

```
var UserLifecycleRule = UserLifecycleRule.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("authentication_method", "password");
parameters.Add("inactivity_days", 12);
parameters.Add("include_site_admins", true);
parameters.Add("include_folder_admins", true);

UserLifecycleRule.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - User Lifecycle Rule ID.
* `action` (string): Required - Action to take on inactive users (disable or delete)
* `authentication_method` (string): Required - User authentication method for the rule
* `inactivity_days` (Nullable<Int64>): Required - Number of days of inactivity before the rule applies
* `include_site_admins` (bool): Include site admins in the rule
* `include_folder_admins` (bool): Include folder admins in the rule


---

## Delete User Lifecycle Rule

```
var UserLifecycleRule = UserLifecycleRule.Find(1);

var parameters = new Dictionary<string, object>();


UserLifecycleRule.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - User Lifecycle Rule ID.
