# FilesCom.Models.PartnerSiteRequest

## Example PartnerSiteRequest Object

```
{
  "id": 1,
  "host_partner_id": 1,
  "guest_site_url": "https://example.files.com",
  "status": "pending",
  "host_site_name": "Acme Site",
  "pairing_key": "abc123xyz",
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Partner Site Request ID
* `host_partner_id` / `HostPartnerId`  (Nullable<Int64>): Host Partner ID
* `guest_site_url` / `GuestSiteUrl`  (string): Guest Site URL
* `status` / `Status`  (string): Request status (pending, approved, rejected)
* `host_site_name` / `HostSiteName`  (string): Host Site Name
* `pairing_key` / `PairingKey`  (string): Pairing key used to approve this request on the Guest Site
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Request creation date/time
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Request last updated date/time


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
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `host_partner_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `host_partner_id`.


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

* `host_partner_id` (Nullable<Int64>): Required - Host Partner ID to link with
* `guest_site_url` (string): Required - Guest Site URL to link to


---

## Reject partner site request

```
Task PartnerSiteRequest.Reject(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `pairing_key` (string): Required - Pairing key for the partner site request


---

## Approve partner site request

```
Task PartnerSiteRequest.Approve(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `pairing_key` (string): Required - Pairing key for the partner site request


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

## Delete Partner Site Request

```
var PartnerSiteRequest = PartnerSiteRequest.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


PartnerSiteRequest.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site Request ID.
