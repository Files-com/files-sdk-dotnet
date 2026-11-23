# FilesCom.Models.Certificate

## Example Certificate Object

```
{
  "id": 1,
  "name": "My Certificate",
  "custom_domain_id": 1,
  "certificate": "[certificate]",
  "created_at": "2000-01-01T01:00:00Z",
  "display_status": "Available",
  "domains": [

  ],
  "expires_at": "2000-01-01T01:00:00Z",
  "brick_managed": true,
  "intermediates": "[certificates]",
  "ip_addresses": [

  ],
  "issuer": "example",
  "key_type": "RSA-4096",
  "request": "[CSR]",
  "status": "Available",
  "subject": "my-custom-domain.com"
}
```

* `id` / `Id`  (Nullable<Int64>): Certificate ID
* `name` / `Name`  (string): Descriptive name of certificate
* `custom_domain_id` / `CustomDomainId`  (Nullable<Int64>): Custom Domain that owns this certificate, including while the certificate is inactive.
* `certificate` / `CertificateType`  (string): Full text of SSL certificate
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Certificate created at
* `display_status` / `DisplayStatus`  (string): Certificate status. (One of `Request Generated`, `New`, `Active`, `Active/Expired`, `Expired`, or `Available`)
* `domains` / `Domains`  (string[]): Domains on this certificate other than files.com domains
* `expires_at` / `ExpiresAt`  (Nullable<DateTime>): Certificate expiration date/time
* `brick_managed` / `BrickManaged`  (bool): Is this certificate automatically managed and renewed by Files.com?
* `intermediates` / `Intermediates`  (string): Intermediate certificates
* `ip_addresses` / `IpAddresses`  (string[]): A list of IP addresses associated with this SSL Certificate
* `issuer` / `Issuer`  (string): X509 issuer
* `key_type` / `KeyType`  (string): Type of key
* `request` / `Request`  (string): Certificate signing request text
* `status` / `Status`  (string): Certificate status (Request Generated, New, Active, Active/Expired, Expired, or Available)
* `subject` / `Subject`  (string): X509 Subject name
* `certificate_domain` / `CertificateDomain`  (string): Domain for certificate.
* `certificate_extra_domains` / `CertificateExtraDomains`  (string[]): Additional domains for the certificate CSR.
* `certificate_country` / `CertificateCountry`  (string): Country.
* `certificate_state_or_province` / `CertificateStateOrProvince`  (string): State or province.
* `certificate_city_or_locale` / `CertificateCityOrLocale`  (string): City or locale.
* `certificate_company_name` / `CertificateCompanyName`  (string): Company name.
* `csr_ou1` / `CsrOu1`  (string): Department name or organization unit 1
* `csr_ou2` / `CsrOu2`  (string): Department name or organization unit 2
* `csr_ou3` / `CsrOu3`  (string): Department name or organization unit 3
* `certificate_email_address` / `CertificateEmailAddress`  (string): Email address for the certificate owner.
* `private_key` / `PrivateKey`  (string): Certificate private key.
* `password` / `Password`  (string): Certificate password.


---

## List Certificates

```
FilesList<Certificate> Certificate.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials:

```
FilesList<Certificate> client.Certificates.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).


---

## Show Certificate

```
Task<Certificate> Certificate.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Certificate> client.Certificates.FindAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.


---

## Create CSR or import existing SSL Certificate

```
Task<Certificate> Certificate.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Certificate> client.Certificates.CreateAsync(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `name` (string): Required - Internal name of the SSL certificate.
* `certificate_domain` (string): Domain for certificate.
* `certificate_extra_domains` (string[]): Additional domains for the certificate CSR.
* `certificate_country` (string): Country.
* `certificate_state_or_province` (string): State or province.
* `certificate_city_or_locale` (string): City or locale.
* `certificate_company_name` (string): Company name.
* `csr_ou1` (string): Department name or organization unit 1
* `csr_ou2` (string): Department name or organization unit 2
* `csr_ou3` (string): Department name or organization unit 3
* `certificate_email_address` (string): Email address for the certificate owner.
* `key_type` (string): Any supported key type.  Defaults to `RSA-4096`.
* `certificate` (string): The certificate.  PEM or PKCS7 formats accepted.
* `private_key` (string): Certificate private key.
* `password` (string): Certificate password.
* `intermediates` (string): Intermediate certificates.  PEM or PKCS7 formats accepted.


---

## Activate SSL Certificate

```
Task Certificate.Activate(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Certificates.ActivateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.
* `replace_cert` (string): Leave blank or set to `any` to replace any currently active certificate, if applicable.  Set to `new_ips` to activate this certificate as an additional certificate on your Site by allocating a new set of Dedicated IPs (may require a Plan upgrade).  Set to the ID of a currently active certificate to replace that certificate on its set of dedicated IPs.  A certificate belonging to a Custom Domain activates only for that domain: leave blank, use `any`, or specify that domain's current certificate ID.


---

## Deactivate SSL Certificate

```
Task Certificate.Deactivate(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Certificates.DeactivateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.


---

## Replace this active custom SSL Certificate for the Primary Custom Domain with a Files.com SSL Certificate.  The Primary Custom Domain's CNAME record must point at the Site's subdomain.  The Files.com certificate is issued and activated in the background

```
Task Certificate.ReplaceWithFiles(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Certificates.ReplaceWithFilesAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.


---

## Update Certificate

```
Task<Certificate> Certificate.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task<Certificate> client.Certificates.UpdateAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.
* `name` (string): Internal certificate name.
* `intermediates` (string): Any intermediate certificates.  PEM or PKCS7 formats accepted.
* `certificate` (string): The certificate.  PEM or PKCS7 formats accepted.


---

## Delete Certificate

```
Task Certificate.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.Certificates.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.


---

## Activate SSL Certificate

```
var certificate = await Certificate.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("replace_cert", "");

await certificate.Activate(parameters);
```

`ActivateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.
* `replace_cert` (string): Leave blank or set to `any` to replace any currently active certificate, if applicable.  Set to `new_ips` to activate this certificate as an additional certificate on your Site by allocating a new set of Dedicated IPs (may require a Plan upgrade).  Set to the ID of a currently active certificate to replace that certificate on its set of dedicated IPs.  A certificate belonging to a Custom Domain activates only for that domain: leave blank, use `any`, or specify that domain's current certificate ID.


---

## Deactivate SSL Certificate

```
var certificate = await Certificate.Find(1L);

var parameters = new Dictionary<string, object>();

await certificate.Deactivate(parameters);
```

`DeactivateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.


---

## Replace this active custom SSL Certificate for the Primary Custom Domain with a Files.com SSL Certificate.  The Primary Custom Domain's CNAME record must point at the Site's subdomain.  The Files.com certificate is issued and activated in the background

```
var certificate = await Certificate.Find(1L);

var parameters = new Dictionary<string, object>();

await certificate.ReplaceWithFiles(parameters);
```

`ReplaceWithFilesAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.


---

## Update Certificate

```
var certificate = await Certificate.Find(1L);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "My Certificate");
parameters.Add("intermediates", "[certificates]");
parameters.Add("certificate", "[certificate]");

await certificate.Update(parameters);
```

`UpdateAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.
When it succeeds, the object takes the attributes Files.com returns. The `Certificate` it returns is a separate object with those attributes.

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.
* `name` (string): Internal certificate name.
* `intermediates` (string): Any intermediate certificates.  PEM or PKCS7 formats accepted.
* `certificate` (string): The certificate.  PEM or PKCS7 formats accepted.


---

## Delete Certificate

```
var certificate = await Certificate.Find(1L);

var parameters = new Dictionary<string, object>();

await certificate.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Certificate ID.


---

## Save

```
await certificate.SaveAsync(cancellationToken);
```

Creates the object on Files.com, or updates it if it has an ID. It sends the attributes set on the object, in its constructor or with its properties, or changed in place, such as an element of an array, since it was made or last came from Files.com, and only those the operation takes, each in the form its parameter takes. When it succeeds, the object takes the attributes Files.com returns. `Save()` does the same without a cancellation token.
