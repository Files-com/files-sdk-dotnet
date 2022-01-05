# FilesCom.Models.As2Station

## Example As2Station Object

```
{
  "id": 1,
  "name": "AS2 Station Name",
  "uri": "",
  "domain": "domain.test",
  "public_certificate": "",
  "public_certificate_md5": "",
  "private_key_md5": ""
}
```

* `id` / `Id`  (Nullable<Int64>): Id of the AS2 Station.
* `name` / `Name`  (string): The station's formal AS2 name.
* `uri` / `Uri`  (string): Public URI for sending AS2 message to.
* `domain` / `Domain`  (string): The station's AS2 domain name.
* `public_certificate` / `PublicCertificate`  (string): Public certificate used for message security.
* `public_certificate_md5` / `PublicCertificateMd5`  (string): MD5 hash of public certificate used for message security.
* `private_key_md5` / `PrivateKeyMd5`  (string): MD5 hash of private key used for message security.
* `private_key` / `PrivateKey`  (string): 


---

## List As2 Stations

```
Task<As2Station[]> As2Station.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show As2 Station

```
Task<As2Station> As2Station.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Station ID.


---

## Create As2 Station

```
Task<As2Station> As2Station.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - AS2 Name
* `domain` (string): Required - AS2 Domain
* `uri` (string): Required - URL base for AS2 responses
* `public_certificate` (string): Required - 
* `private_key` (string): Required - 


---

## Update As2 Station

```
Task<As2Station> As2Station.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Station ID.
* `name` (string): AS2 Name
* `domain` (string): AS2 Domain
* `uri` (string): URL base for AS2 responses
* `public_certificate` (string): 
* `private_key` (string): 


---

## Delete As2 Station

```
Task<As2Station> As2Station.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Station ID.


---

## Update As2 Station

```
var As2Station = As2Station.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("name", "AS2 Station Name");
parameters.Add("domain", "domain.test");

As2Station.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Station ID.
* `name` (string): AS2 Name
* `domain` (string): AS2 Domain
* `uri` (string): URL base for AS2 responses
* `public_certificate` (string): 
* `private_key` (string): 


---

## Delete As2 Station

```
var As2Station = As2Station.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


As2Station.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Station ID.
