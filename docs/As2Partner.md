# FilesCom.Models.As2Partner

## Example As2Partner Object

```
{
  "id": 1,
  "as2_station_id": 1,
  "name": "AS2 Partner Name",
  "uri": "example",
  "server_certificate": "require_match",
  "http_auth_username": "username",
  "additional_http_headers": {
    "key": "example value"
  },
  "default_mime_type": "application/octet-stream",
  "mdn_validation_level": "none",
  "enable_dedicated_ips": true,
  "hex_public_certificate_serial": "A5:EB:C1:95:DC:D8:2B:E7",
  "public_certificate_md5": "example",
  "public_certificate_subject": "example",
  "public_certificate_issuer": "example",
  "public_certificate_serial": "example",
  "public_certificate_not_before": "example",
  "public_certificate_not_after": "example"
}
```

* `id` / `Id`  (Nullable<Int64>): ID of the AS2 Partner.
* `as2_station_id` / `As2StationId`  (Nullable<Int64>): ID of the AS2 Station associated with this partner.
* `name` / `Name`  (string): The partner's formal AS2 name.
* `uri` / `Uri`  (string): Public URI where we will send the AS2 messages (via HTTP/HTTPS).
* `server_certificate` / `ServerCertificate`  (string): Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS?
* `http_auth_username` / `HttpAuthUsername`  (string): Username to send to server for HTTP Authentication.
* `additional_http_headers` / `AdditionalHttpHeaders`  (object): Additional HTTP Headers for outgoing message sent to this partner.
* `default_mime_type` / `DefaultMimeType`  (string): Default mime type of the file attached to the encrypted message
* `mdn_validation_level` / `MdnValidationLevel`  (string): How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates.
* `enable_dedicated_ips` / `EnableDedicatedIps`  (bool): If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
* `hex_public_certificate_serial` / `HexPublicCertificateSerial`  (string): Serial of public certificate used for message security in hex format.
* `public_certificate_md5` / `PublicCertificateMd5`  (string): MD5 hash of public certificate used for message security.
* `public_certificate_subject` / `PublicCertificateSubject`  (string): Subject of public certificate used for message security.
* `public_certificate_issuer` / `PublicCertificateIssuer`  (string): Issuer of public certificate used for message security.
* `public_certificate_serial` / `PublicCertificateSerial`  (string): Serial of public certificate used for message security.
* `public_certificate_not_before` / `PublicCertificateNotBefore`  (string): Not before value of public certificate used for message security.
* `public_certificate_not_after` / `PublicCertificateNotAfter`  (string): Not after value of public certificate used for message security.
* `http_auth_password` / `HttpAuthPassword`  (string): Password to send to server for HTTP Authentication.
* `public_certificate` / `PublicCertificate`  (string): Public certificate for AS2 Partner.  Note: This is the certificate for AS2 message security, not a certificate used for HTTPS authentication.


---

## List AS2 Partners

```
Task<FilesList<As2Partner>> As2Partner.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show AS2 Partner

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

## Create AS2 Partner

```
Task<As2Partner> As2Partner.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `enable_dedicated_ips` (bool): If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
* `http_auth_username` (string): Username to send to server for HTTP Authentication.
* `http_auth_password` (string): Password to send to server for HTTP Authentication.
* `mdn_validation_level` (string): How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates.
* `server_certificate` (string): Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS?
* `default_mime_type` (string): Default mime type of the file attached to the encrypted message
* `additional_http_headers` (object): Additional HTTP Headers for outgoing message sent to this partner.
* `as2_station_id` (Nullable<Int64>): Required - ID of the AS2 Station associated with this partner.
* `name` (string): Required - The partner's formal AS2 name.
* `uri` (string): Required - Public URI where we will send the AS2 messages (via HTTP/HTTPS).
* `public_certificate` (string): Required - Public certificate for AS2 Partner.  Note: This is the certificate for AS2 message security, not a certificate used for HTTPS authentication.


---

## Update AS2 Partner

```
Task<As2Partner> As2Partner.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.
* `enable_dedicated_ips` (bool): If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
* `http_auth_username` (string): Username to send to server for HTTP Authentication.
* `http_auth_password` (string): Password to send to server for HTTP Authentication.
* `mdn_validation_level` (string): How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates.
* `server_certificate` (string): Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS?
* `default_mime_type` (string): Default mime type of the file attached to the encrypted message
* `additional_http_headers` (object): Additional HTTP Headers for outgoing message sent to this partner.
* `name` (string): The partner's formal AS2 name.
* `uri` (string): Public URI where we will send the AS2 messages (via HTTP/HTTPS).
* `public_certificate` (string): Public certificate for AS2 Partner.  Note: This is the certificate for AS2 message security, not a certificate used for HTTPS authentication.


---

## Delete AS2 Partner

```
Task As2Partner.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.


---

## Update AS2 Partner

```
var As2Partner = As2Partner.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("enable_dedicated_ips", true);
parameters.Add("http_auth_username", "username");
parameters.Add("mdn_validation_level", "none");
parameters.Add("server_certificate", "require_match");
parameters.Add("default_mime_type", "application/octet-stream");
parameters.Add("additional_http_headers", {"key":"example value"});
parameters.Add("name", "AS2 Partner Name");
parameters.Add("uri", "example");

As2Partner.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.
* `enable_dedicated_ips` (bool): If `true`, we will use your site's dedicated IPs for all outbound connections to this AS2 Partner.
* `http_auth_username` (string): Username to send to server for HTTP Authentication.
* `http_auth_password` (string): Password to send to server for HTTP Authentication.
* `mdn_validation_level` (string): How should Files.com evaluate message transfer success based on a partner's MDN response?  This setting does not affect MDN storage; all MDNs received from a partner are always stored. `none`: MDN is stored for informational purposes only, a successful HTTPS transfer is a successful AS2 transfer. `weak`: Inspect the MDN for MIC and Disposition only. `normal`: `weak` plus validate MDN signature matches body, `strict`: `normal` but do not allow signatures from self-signed or incorrectly purposed certificates.
* `server_certificate` (string): Should we require that the remote HTTP server have a valid SSL Certificate for HTTPS?
* `default_mime_type` (string): Default mime type of the file attached to the encrypted message
* `additional_http_headers` (object): Additional HTTP Headers for outgoing message sent to this partner.
* `name` (string): The partner's formal AS2 name.
* `uri` (string): Public URI where we will send the AS2 messages (via HTTP/HTTPS).
* `public_certificate` (string): Public certificate for AS2 Partner.  Note: This is the certificate for AS2 message security, not a certificate used for HTTPS authentication.


---

## Delete AS2 Partner

```
var As2Partner = As2Partner.Find(1);

var parameters = new Dictionary<string, object>();


As2Partner.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - As2 Partner ID.
