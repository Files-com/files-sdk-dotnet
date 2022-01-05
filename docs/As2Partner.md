# FilesCom.Models.As2Partner

## Example As2Partner Object

```
{
  "id": 1,
  "as2_station_id": 1,
  "name": "AS2 Partner Name",
  "uri": "",
  "public_certificate_md5": ""
}
```

* `id` / `Id`  (Nullable<Int64>): Id of the AS2 Partner.
* `as2_station_id` / `As2StationId`  (Nullable<Int64>): Id of the AS2 Station associated with this partner.
* `name` / `Name`  (string): The partner's formal AS2 name.
* `uri` / `Uri`  (string): Public URI for sending AS2 message to.
* `public_certificate_md5` / `PublicCertificateMd5`  (string): MD5 hash of public certificate used for message security.
* `public_certificate` / `PublicCertificate`  (string): 


---

## List As2 Partners

```
Task<As2Partner[]> As2Partner.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show As2 Partner

```
Task<As2Partner> As2Partner.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.


---

## Create As2 Partner

```
Task<As2Partner> As2Partner.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - AS2 Name
* `uri` (string): Required - URL base for AS2 responses
* `public_certificate` (string): Required - 
* `as2_station_id` (Nullable<Int64>): Required - Id of As2Station for this partner


---

## Update As2 Partner

```
Task<As2Partner> As2Partner.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.
* `name` (string): AS2 Name
* `uri` (string): URL base for AS2 responses
* `public_certificate` (string): 


---

## Delete As2 Partner

```
Task<As2Partner> As2Partner.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.


---

## Update As2 Partner

```
var As2Partner = As2Partner.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("name", "AS2 Partner Name");

As2Partner.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.
* `name` (string): AS2 Name
* `uri` (string): URL base for AS2 responses
* `public_certificate` (string): 


---

## Delete As2 Partner

```
var As2Partner = As2Partner.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


As2Partner.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.
