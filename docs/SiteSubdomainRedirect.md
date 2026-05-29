# FilesCom.Models.SiteSubdomainRedirect

## Example SiteSubdomainRedirect Object

```
{
  "id": 1,
  "subdomain": "old-company",
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Site subdomain redirect ID.
* `subdomain` / `Subdomain`  (string): Files.com subdomain that continues to route to the current site subdomain.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): When this redirect was created.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): When this redirect was last updated.


---

## List Site Subdomain Redirects

```
Task<FilesList<SiteSubdomainRedirect>> SiteSubdomainRedirect.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `id`.


---

## Show Site Subdomain Redirect

```
Task<SiteSubdomainRedirect> SiteSubdomainRedirect.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Site Subdomain Redirect ID.


---

## Delete Site Subdomain Redirect

```
Task SiteSubdomainRedirect.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Site Subdomain Redirect ID.


---

## Delete Site Subdomain Redirect

```
var SiteSubdomainRedirect = SiteSubdomainRedirect.Find(1);

var parameters = new Dictionary<string, object>();


SiteSubdomainRedirect.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Site Subdomain Redirect ID.
