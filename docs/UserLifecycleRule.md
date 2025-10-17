# FilesCom.Models.UserLifecycleRule

## Example UserLifecycleRule Object

```
{
  "id": 1,
  "authentication_method": "password",
  "group_ids": [
    1,
    2,
    3
  ],
  "action": "disable",
  "inactivity_days": 12,
  "include_folder_admins": true,
  "include_site_admins": true,
  "name": "password specific rules",
  "partner_tag": "guest",
  "site_id": 1,
  "user_state": "inactive",
  "user_tag": "guest"
}
```

* `id` / `Id`  (Nullable<Int64>): User Lifecycle Rule ID
* `authentication_method` / `AuthenticationMethod`  (string): User authentication method for which the rule will apply.
* `group_ids` / `GroupIds`  (Nullable<Int64>[]): Array of Group IDs to which the rule applies. If empty or not set, the rule applies to all users.
* `action` / `Action`  (string): Action to take on inactive users (disable or delete)
* `inactivity_days` / `InactivityDays`  (Nullable<Int64>): Number of days of inactivity before the rule applies
* `include_folder_admins` / `IncludeFolderAdmins`  (bool): If true, the rule will apply to folder admins.
* `include_site_admins` / `IncludeSiteAdmins`  (bool): If true, the rule will apply to site admins.
* `name` / `Name`  (string): User Lifecycle Rule name
* `partner_tag` / `PartnerTag`  (string): If provided, only users belonging to Partners with this tag at the Partner level will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.
* `site_id` / `SiteId`  (Nullable<Int64>): Site ID
* `user_state` / `UserState`  (string): State of the users to apply the rule to (inactive or disabled)
* `user_tag` / `UserTag`  (string): If provided, only users with this tag will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.


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
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `site_id`.


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

* `action` (string): Action to take on inactive users (disable or delete)
* `authentication_method` (string): User authentication method for which the rule will apply.
* `group_ids` (Nullable<Int64>[]): Array of Group IDs to which the rule applies. If empty or not set, the rule applies to all users.
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies
* `include_site_admins` (bool): If true, the rule will apply to site admins.
* `include_folder_admins` (bool): If true, the rule will apply to folder admins.
* `name` (string): User Lifecycle Rule name
* `partner_tag` (string): If provided, only users belonging to Partners with this tag at the Partner level will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.
* `user_state` (string): State of the users to apply the rule to (inactive or disabled)
* `user_tag` (string): If provided, only users with this tag will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.


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
* `action` (string): Action to take on inactive users (disable or delete)
* `authentication_method` (string): User authentication method for which the rule will apply.
* `group_ids` (Nullable<Int64>[]): Array of Group IDs to which the rule applies. If empty or not set, the rule applies to all users.
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies
* `include_site_admins` (bool): If true, the rule will apply to site admins.
* `include_folder_admins` (bool): If true, the rule will apply to folder admins.
* `name` (string): User Lifecycle Rule name
* `partner_tag` (string): If provided, only users belonging to Partners with this tag at the Partner level will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.
* `user_state` (string): State of the users to apply the rule to (inactive or disabled)
* `user_tag` (string): If provided, only users with this tag will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.


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
parameters.Add("group_ids", [1,2,3]);
parameters.Add("inactivity_days", 12);
parameters.Add("include_site_admins", true);
parameters.Add("include_folder_admins", true);
parameters.Add("name", "password specific rules");
parameters.Add("partner_tag", "guest");
parameters.Add("user_state", "inactive");
parameters.Add("user_tag", "guest");

UserLifecycleRule.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - User Lifecycle Rule ID.
* `action` (string): Action to take on inactive users (disable or delete)
* `authentication_method` (string): User authentication method for which the rule will apply.
* `group_ids` (Nullable<Int64>[]): Array of Group IDs to which the rule applies. If empty or not set, the rule applies to all users.
* `inactivity_days` (Nullable<Int64>): Number of days of inactivity before the rule applies
* `include_site_admins` (bool): If true, the rule will apply to site admins.
* `include_folder_admins` (bool): If true, the rule will apply to folder admins.
* `name` (string): User Lifecycle Rule name
* `partner_tag` (string): If provided, only users belonging to Partners with this tag at the Partner level will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.
* `user_state` (string): State of the users to apply the rule to (inactive or disabled)
* `user_tag` (string): If provided, only users with this tag will be affected by the rule. Tags must only contain lowercase letters, numbers, and hyphens.


---

## Delete User Lifecycle Rule

```
var UserLifecycleRule = UserLifecycleRule.Find(1);

var parameters = new Dictionary<string, object>();


UserLifecycleRule.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - User Lifecycle Rule ID.
