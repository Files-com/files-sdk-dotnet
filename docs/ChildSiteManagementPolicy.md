# FilesCom.Models.ChildSiteManagementPolicy

## Example ChildSiteManagementPolicy Object

```
{
  "id": 1,
  "site_id": 1,
  "site_setting_name": "color2_left",
  "managed_value": "#FF0000",
  "skip_child_site_ids": [
    1,
    5
  ]
}
```

* `id` / `Id`  (Nullable<Int64>): ChildSiteManagementPolicy ID
* `site_id` / `SiteId`  (Nullable<Int64>): ID of the Site managing the policy
* `site_setting_name` / `SiteSettingName`  (string): The name of the setting that is managed by the policy
* `managed_value` / `ManagedValue`  (string): The value for the setting that will be enforced for all child sites that are not exempt
* `skip_child_site_ids` / `SkipChildSiteIds`  (Nullable<Int64>[]): The list of child site IDs that are exempt from this policy


---

## List Child Site Management Policies

```
Task<FilesList<ChildSiteManagementPolicy>> ChildSiteManagementPolicy.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Child Site Management Policy

```
Task<ChildSiteManagementPolicy> ChildSiteManagementPolicy.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.


---

## Create Child Site Management Policy

```
Task<ChildSiteManagementPolicy> ChildSiteManagementPolicy.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `site_setting_name` (string): Required - The name of the setting that is managed by the policy
* `managed_value` (string): Required - The value for the setting that will be enforced for all child sites that are not exempt
* `skip_child_site_ids` (Nullable<Int64>[]): The list of child site IDs that are exempt from this policy


---

## Update Child Site Management Policy

```
Task<ChildSiteManagementPolicy> ChildSiteManagementPolicy.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.
* `site_setting_name` (string): Required - The name of the setting that is managed by the policy
* `managed_value` (string): Required - The value for the setting that will be enforced for all child sites that are not exempt
* `skip_child_site_ids` (Nullable<Int64>[]): The list of child site IDs that are exempt from this policy


---

## Delete Child Site Management Policy

```
Task ChildSiteManagementPolicy.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.


---

## Update Child Site Management Policy

```
var ChildSiteManagementPolicy = ChildSiteManagementPolicy.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("site_setting_name", "color2_left");
parameters.Add("managed_value", "#FF0000");
parameters.Add("skip_child_site_ids", [1,5]);

ChildSiteManagementPolicy.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.
* `site_setting_name` (string): Required - The name of the setting that is managed by the policy
* `managed_value` (string): Required - The value for the setting that will be enforced for all child sites that are not exempt
* `skip_child_site_ids` (Nullable<Int64>[]): The list of child site IDs that are exempt from this policy


---

## Delete Child Site Management Policy

```
var ChildSiteManagementPolicy = ChildSiteManagementPolicy.Find(1);

var parameters = new Dictionary<string, object>();


ChildSiteManagementPolicy.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Child Site Management Policy ID.
