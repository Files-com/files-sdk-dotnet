# FilesCom.Models.PartnerSite

## Example PartnerSite Object

```
{
  "host_partner_id": 1,
  "host_partner_name": "Acme Corp",
  "guest_partner_id": 2,
  "guest_partner_name": "Example Corp",
  "host_site_id": 1,
  "host_site_name": "Acme Site",
  "guest_site_id": 2,
  "guest_site_name": "Example Site",
  "workspace_id": 1
}
```

* `host_partner_id` / `HostPartnerId`  (Nullable<Int64>): Host Partner ID
* `host_partner_name` / `HostPartnerName`  (string): Host Partner Name
* `guest_partner_id` / `GuestPartnerId`  (Nullable<Int64>): Guest Partner ID
* `guest_partner_name` / `GuestPartnerName`  (string): Guest Partner Name
* `host_site_id` / `HostSiteId`  (Nullable<Int64>): Host Site ID
* `host_site_name` / `HostSiteName`  (string): Host Site Name
* `guest_site_id` / `GuestSiteId`  (Nullable<Int64>): Guest Site ID
* `guest_site_name` / `GuestSiteName`  (string): Guest Site Name
* `workspace_id` / `WorkspaceId`  (Nullable<Int64>): Workspace ID for the Host Partner


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
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
