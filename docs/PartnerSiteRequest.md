# FilesCom.Models.PartnerSiteRequest

## Example PartnerSiteRequest Object

```
{
  "id": 1,
  "partner_id": 1,
  "linked_site_id": 1,
  "status": "pending",
  "main_site_name": "Acme Site",
  "pairing_key": "abc123xyz",
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Partner Site Request ID
* `partner_id` / `PartnerId`  (Nullable<Int64>): Partner ID
* `linked_site_id` / `LinkedSiteId`  (Nullable<Int64>): Linked Site ID
* `status` / `Status`  (string): Request status (pending, approved, rejected)
* `main_site_name` / `MainSiteName`  (string): Main Site Name
* `pairing_key` / `PairingKey`  (string): Pairing key used to approve this request on the target site
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Request creation date/time
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Request last updated date/time
* `site_url` / `SiteUrl`  (string): Site URL to link to


---

## List Partner Site Requests

```
Task<FilesList<PartnerSiteRequest>> PartnerSiteRequest.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Find partner site request by pairing key

```
Task PartnerSiteRequest.FindByPairingKey(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `pairing_key` (string): Required - Pairing key for the partner site request


---

## Create Partner Site Request

```
Task<PartnerSiteRequest> PartnerSiteRequest.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `partner_id` (Nullable<Int64>): Required - Partner ID to link with
* `site_url` (string): Required - Site URL to link to


---

## Reject partner site request

```
Task PartnerSiteRequest.Reject(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site Request ID.


---

## Approve partner site request

```
Task PartnerSiteRequest.Approve(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site Request ID.


---

## Delete Partner Site Request

```
Task PartnerSiteRequest.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site Request ID.


---

## Reject partner site request

```
var PartnerSiteRequest = PartnerSiteRequest.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


PartnerSiteRequest.Reject
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site Request ID.


---

## Approve partner site request

```
var PartnerSiteRequest = PartnerSiteRequest.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


PartnerSiteRequest.Approve
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site Request ID.


---

## Delete Partner Site Request

```
var PartnerSiteRequest = PartnerSiteRequest.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


PartnerSiteRequest.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site Request ID.
