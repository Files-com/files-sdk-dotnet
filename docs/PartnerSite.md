# FilesCom.Models.PartnerSite

## Example PartnerSite Object

```
{
  "partner_id": 1,
  "partner_name": "Acme Corp",
  "linked_site_id": 1,
  "linked_site_name": "Acme's Partner Site",
  "main_site_id": 2,
  "main_site_name": "Acme Site"
}
```

* `partner_id` / `PartnerId`  (Nullable<Int64>): Partner ID
* `partner_name` / `PartnerName`  (string): Partner Name
* `linked_site_id` / `LinkedSiteId`  (Nullable<Int64>): Linked Site ID
* `linked_site_name` / `LinkedSiteName`  (string): Linked Site Name
* `main_site_id` / `MainSiteId`  (Nullable<Int64>): Main Site ID
* `main_site_name` / `MainSiteName`  (string): Main Site Name


---

## Get Partner Sites linked to the current Site

```
Task<PartnerSite[]> PartnerSite.Linkeds(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```


---

## List Partner Sites

```
Task<FilesList<PartnerSite>> PartnerSite.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
