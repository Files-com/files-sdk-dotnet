# FilesCom.Models.As2Station

## Example As2Station Object

```
{
  "id": 1,
  "name": "AS2 Station Name",
  "uri": "example",
  "domain": "domain.test",
  "hex_public_certificate_serial": "A5:EB:C1:95:DC:D8:2B:E7",
  "public_certificate_md5": "example",
  "private_key_md5": "example",
  "public_certificate_subject": "example",
  "public_certificate_issuer": "example",
  "public_certificate_serial": "example",
  "public_certificate_not_before": "example",
  "public_certificate_not_after": "example",
  "private_key_password_md5": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): Id of the AS2 Station.
* `name` / `Name`  (string): The station's formal AS2 name.
* `uri` / `Uri`  (string): Public URI for sending AS2 message to.
* `domain` / `Domain`  (string): The station's AS2 domain name.
* `hex_public_certificate_serial` / `HexPublicCertificateSerial`  (string): Serial of public certificate used for message security in hex format.
* `public_certificate_md5` / `PublicCertificateMd5`  (string): MD5 hash of public certificate used for message security.
* `private_key_md5` / `PrivateKeyMd5`  (string): MD5 hash of private key used for message security.
* `public_certificate_subject` / `PublicCertificateSubject`  (string): Subject of public certificate used for message security.
* `public_certificate_issuer` / `PublicCertificateIssuer`  (string): Issuer of public certificate used for message security.
* `public_certificate_serial` / `PublicCertificateSerial`  (string): Serial of public certificate used for message security.
* `public_certificate_not_before` / `PublicCertificateNotBefore`  (string): Not before value of public certificate used for message security.
* `public_certificate_not_after` / `PublicCertificateNotAfter`  (string): Not after value of public certificate used for message security.
* `private_key_password_md5` / `PrivateKeyPasswordMd5`  (string): MD5 hash of private key password used for message security.
* `public_certificate` / `PublicCertificate`  (string): 
* `private_key` / `PrivateKey`  (string): 
* `private_key_password` / `PrivateKeyPassword`  (string): 


---

## List As2 Stations

```
Task<FilesList<As2Station>> As2Station.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
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
* `public_certificate` (string): Required - 
* `private_key` (string): Required - 
* `private_key_password` (string): 


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
* `public_certificate` (string): 
* `private_key` (string): 
* `private_key_password` (string): 


---

## Delete As2 Station

```
Task As2Station.Delete(
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
var As2Station = As2Station.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "AS2 Station Name");

As2Station.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Station ID.
* `name` (string): AS2 Name
* `public_certificate` (string): 
* `private_key` (string): 
* `private_key_password` (string): 


---

## Delete As2 Station

```
var As2Station = As2Station.Find(1);

var parameters = new Dictionary<string, object>();


As2Station.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Station ID.
