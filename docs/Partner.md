# FilesCom.Models.Partner

## Example Partner Object

```
{
  "allow_bypassing_2fa_policies": true,
  "allow_credential_changes": true,
  "allow_providing_gpg_keys": true,
  "allow_user_creation": true,
  "id": 1,
  "name": "Acme Corp",
  "notes": "This is a note about the partner.",
  "root_folder": "/AcmeCorp",
  "tags": "example",
  "user_ids": [
    1,
    2,
    3
  ]
}
```

* `allow_bypassing_2fa_policies` / `AllowBypassing2faPolicies`  (bool): Allow users created under this Partner to bypass Two-Factor Authentication policies.
* `allow_credential_changes` / `AllowCredentialChanges`  (bool): Allow Partner Admins to change or reset credentials for users belonging to this Partner.
* `allow_providing_gpg_keys` / `AllowProvidingGpgKeys`  (bool): Allow Partner Admins to provide GPG keys.
* `allow_user_creation` / `AllowUserCreation`  (bool): Allow Partner Admins to create users.
* `id` / `Id`  (Nullable<Int64>): The unique ID of the Partner.
* `name` / `Name`  (string): The name of the Partner.
* `notes` / `Notes`  (string): Notes about this Partner.
* `root_folder` / `RootFolder`  (string): The root folder path for this Partner.
* `tags` / `Tags`  (string): Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
* `user_ids` / `UserIds`  (Nullable<Int64>[]): Array of User IDs that belong to this Partner.


---

## List Partners

```
Task<FilesList<Partner>> Partner.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`.


---

## Show Partner

```
Task<Partner> Partner.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner ID.


---

## Create Partner

```
Task<Partner> Partner.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): The name of the Partner.
* `allow_bypassing_2fa_policies` (bool): Allow users created under this Partner to bypass Two-Factor Authentication policies.
* `allow_credential_changes` (bool): Allow Partner Admins to change or reset credentials for users belonging to this Partner.
* `allow_providing_gpg_keys` (bool): Allow Partner Admins to provide GPG keys.
* `allow_user_creation` (bool): Allow Partner Admins to create users.
* `notes` (string): Notes about this Partner.
* `root_folder` (string): The root folder path for this Partner.
* `tags` (string): Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.


---

## Update Partner

```
Task<Partner> Partner.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner ID.
* `name` (string): The name of the Partner.
* `allow_bypassing_2fa_policies` (bool): Allow users created under this Partner to bypass Two-Factor Authentication policies.
* `allow_credential_changes` (bool): Allow Partner Admins to change or reset credentials for users belonging to this Partner.
* `allow_providing_gpg_keys` (bool): Allow Partner Admins to provide GPG keys.
* `allow_user_creation` (bool): Allow Partner Admins to create users.
* `notes` (string): Notes about this Partner.
* `root_folder` (string): The root folder path for this Partner.
* `tags` (string): Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.


---

## Delete Partner

```
Task Partner.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner ID.


---

## Update Partner

```
var Partner = Partner.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "Acme Corp");
parameters.Add("allow_bypassing_2fa_policies", false);
parameters.Add("allow_credential_changes", false);
parameters.Add("allow_providing_gpg_keys", false);
parameters.Add("allow_user_creation", false);
parameters.Add("notes", "This is a note about the partner.");
parameters.Add("root_folder", "/AcmeCorp");
parameters.Add("tags", "example");

Partner.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner ID.
* `name` (string): The name of the Partner.
* `allow_bypassing_2fa_policies` (bool): Allow users created under this Partner to bypass Two-Factor Authentication policies.
* `allow_credential_changes` (bool): Allow Partner Admins to change or reset credentials for users belonging to this Partner.
* `allow_providing_gpg_keys` (bool): Allow Partner Admins to provide GPG keys.
* `allow_user_creation` (bool): Allow Partner Admins to create users.
* `notes` (string): Notes about this Partner.
* `root_folder` (string): The root folder path for this Partner.
* `tags` (string): Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.


---

## Delete Partner

```
var Partner = Partner.Find(1);

var parameters = new Dictionary<string, object>();


Partner.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner ID.
