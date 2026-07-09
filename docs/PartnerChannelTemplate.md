# FilesCom.Models.PartnerChannelTemplate

## Example PartnerChannelTemplate Object

```
{
  "id": 1,
  "workspace_id": 1,
  "name": "Claims Template",
  "path": "claims/medical",
  "to_partner_folder_name": "outgoing",
  "from_partner_folder_name": "incoming",
  "from_partner_route_path": "processing/from-partner",
  "to_partner_route_path": "delivery/to-partner",
  "to_partner_managed_folder_paths": [
    "reports/monthly"
  ],
  "from_partner_managed_folder_paths": [
    "claims/received"
  ],
  "effective_to_partner_folder_name": "outgoing",
  "effective_from_partner_folder_name": "incoming"
}
```

* `id` / `Id`  (Nullable<Int64>): The unique ID of the Partner Channel Template.
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): ID of the Workspace associated with this Partner Channel Template.
* `name` / `Name`  (string): The name of the Partner Channel Template.
* `path` / `Path`  (string): Channel path relative to the Partner root folder. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `to_partner_folder_name` / `ToPartnerFolderName`  (string): Optional Channel-level to-Partner folder name override.
* `from_partner_folder_name` / `FromPartnerFolderName`  (string): Optional Channel-level from-Partner folder name override.
* `from_partner_route_path` / `FromPartnerRoutePath`  (string): Optional route path for files uploaded by the Partner.
* `to_partner_route_path` / `ToPartnerRoutePath`  (string): Optional route path for files delivered to the Partner.
* `to_partner_managed_folder_paths` / `ToPartnerManagedFolderPaths`  (string[]): Managed folder paths inside the to-Partner folder.
* `from_partner_managed_folder_paths` / `FromPartnerManagedFolderPaths`  (string[]): Managed folder paths inside the from-Partner folder.
* `effective_to_partner_folder_name` / `EffectiveToPartnerFolderName`  (string): Resolved to-Partner folder name after Template override and default.
* `effective_from_partner_folder_name` / `EffectiveFromPartnerFolderName`  (string): Resolved from-Partner folder name after Template override and default.


---

## List Partner Channel Templates

```
Task<FilesList<PartnerChannelTemplate>> PartnerChannelTemplate.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.


---

## Show Partner Channel Template

```
Task<PartnerChannelTemplate> PartnerChannelTemplate.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Channel Template ID.


---

## Create Partner Channel Template

```
Task<PartnerChannelTemplate> PartnerChannelTemplate.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `from_partner_folder_name` (string): Optional Channel-level from-Partner folder name override.
* `from_partner_managed_folder_paths` (string[]): Managed folder paths inside the from-Partner folder.
* `from_partner_route_path` (string): Optional route path for files uploaded by the Partner.
* `to_partner_folder_name` (string): Optional Channel-level to-Partner folder name override.
* `to_partner_managed_folder_paths` (string[]): Managed folder paths inside the to-Partner folder.
* `to_partner_route_path` (string): Optional route path for files delivered to the Partner.
* `name` (string): Required - The name of the Partner Channel Template.
* `path` (string): Required - Channel path relative to the Partner root folder.
* `workspace_id` (Nullable<Int64>): ID of the Workspace associated with this Partner Channel Template.


---

## Update Partner Channel Template

```
Task<PartnerChannelTemplate> PartnerChannelTemplate.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Channel Template ID.
* `from_partner_folder_name` (string): Optional Channel-level from-Partner folder name override.
* `from_partner_managed_folder_paths` (string[]): Managed folder paths inside the from-Partner folder.
* `from_partner_route_path` (string): Optional route path for files uploaded by the Partner.
* `to_partner_folder_name` (string): Optional Channel-level to-Partner folder name override.
* `to_partner_managed_folder_paths` (string[]): Managed folder paths inside the to-Partner folder.
* `to_partner_route_path` (string): Optional route path for files delivered to the Partner.
* `name` (string): The name of the Partner Channel Template.
* `path` (string): Channel path relative to the Partner root folder.


---

## Delete Partner Channel Template

```
Task PartnerChannelTemplate.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Channel Template ID.


---

## Update Partner Channel Template

```
var PartnerChannelTemplate = PartnerChannelTemplate.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("from_partner_folder_name", "incoming");
parameters.Add("from_partner_managed_folder_paths", ["claims/received"]);
parameters.Add("from_partner_route_path", "processing/from-partner");
parameters.Add("to_partner_folder_name", "outgoing");
parameters.Add("to_partner_managed_folder_paths", ["reports/monthly"]);
parameters.Add("to_partner_route_path", "delivery/to-partner");
parameters.Add("name", "Claims Template");
parameters.Add("path", "claims/medical");

PartnerChannelTemplate.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Channel Template ID.
* `from_partner_folder_name` (string): Optional Channel-level from-Partner folder name override.
* `from_partner_managed_folder_paths` (string[]): Managed folder paths inside the from-Partner folder.
* `from_partner_route_path` (string): Optional route path for files uploaded by the Partner.
* `to_partner_folder_name` (string): Optional Channel-level to-Partner folder name override.
* `to_partner_managed_folder_paths` (string[]): Managed folder paths inside the to-Partner folder.
* `to_partner_route_path` (string): Optional route path for files delivered to the Partner.
* `name` (string): The name of the Partner Channel Template.
* `path` (string): Channel path relative to the Partner root folder.


---

## Delete Partner Channel Template

```
var PartnerChannelTemplate = PartnerChannelTemplate.Find(1);

var parameters = new Dictionary<string, object>();


PartnerChannelTemplate.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Channel Template ID.
