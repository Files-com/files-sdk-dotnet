# FilesCom.Models.CustomDomain

## Example CustomDomain Object

```
{
  "id": 1,
  "domain": "files.example.com",
  "destination": "site_alias",
  "dns_status": "correct",
  "ssl_certificate_id": 1,
  "brick_managed": true,
  "folder_behavior_id": 1,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Custom Domain ID.
* `domain` / `Domain`  (string): Customer-owned domain name.
* `destination` / `Destination`  (string): Where this custom domain routes. Can be `site_alias`, `public_hosting`, or `s3_endpoint`.
* `dns_status` / `DnsStatus`  (string): Current DNS verification status.
* `ssl_certificate_id` / `SslCertificateId`  (Nullable<Int64>): Current SSL certificate ID.
* `brick_managed` / `BrickManaged`  (bool): Is this domain's SSL certificate automatically managed and renewed by Files.com?
* `folder_behavior_id` / `FolderBehaviorId`  (Nullable<Int64>): Public Hosting behavior ID when this domain routes to a specific Public Hosting behavior.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): When this Custom Domain was created.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): When this Custom Domain was last updated.


---

## List Custom Domains

```
Task<FilesList<CustomDomain>> CustomDomain.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `id`.


---

## Show Custom Domain

```
Task<CustomDomain> CustomDomain.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Custom Domain ID.


---

## Create Custom Domain

```
Task<CustomDomain> CustomDomain.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `destination` (string): Where this custom domain routes. Can be `site_alias`, `public_hosting`, or `s3_endpoint`.
* `folder_behavior_id` (Nullable<Int64>): Public Hosting behavior ID when this domain routes to a specific Public Hosting behavior.
* `ssl_certificate_id` (Nullable<Int64>): Current SSL certificate ID.
* `domain` (string): Required - Customer-owned domain name.


---

## Update Custom Domain

```
Task<CustomDomain> CustomDomain.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Custom Domain ID.
* `destination` (string): Where this custom domain routes. Can be `site_alias`, `public_hosting`, or `s3_endpoint`.
* `folder_behavior_id` (Nullable<Int64>): Public Hosting behavior ID when this domain routes to a specific Public Hosting behavior.
* `ssl_certificate_id` (Nullable<Int64>): Current SSL certificate ID.
* `domain` (string): Customer-owned domain name.


---

## Delete Custom Domain

```
Task CustomDomain.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Custom Domain ID.


---

## Update Custom Domain

```
var CustomDomain = CustomDomain.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("destination", "site_alias");
parameters.Add("folder_behavior_id", 1);
parameters.Add("ssl_certificate_id", 1);
parameters.Add("domain", "files.example.com");

CustomDomain.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Custom Domain ID.
* `destination` (string): Where this custom domain routes. Can be `site_alias`, `public_hosting`, or `s3_endpoint`.
* `folder_behavior_id` (Nullable<Int64>): Public Hosting behavior ID when this domain routes to a specific Public Hosting behavior.
* `ssl_certificate_id` (Nullable<Int64>): Current SSL certificate ID.
* `domain` (string): Customer-owned domain name.


---

## Delete Custom Domain

```
var CustomDomain = CustomDomain.Find(1);

var parameters = new Dictionary<string, object>();


CustomDomain.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Custom Domain ID.
