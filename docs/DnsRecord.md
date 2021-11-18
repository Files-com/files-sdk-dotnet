# FilesCom.Models.DnsRecord

## Example DnsRecord Object

```
{
  "id": "customdomain.com-CNAME-site.files.com",
  "domain": "my-custom-domain.com",
  "rrtype": "CNAME",
  "value": "mysite.files.com"
}
```

* `id` / `Id`  (string): Unique label for DNS record; used by Zapier and other integrations.
* `domain` / `Domain`  (string): DNS record domain name
* `rrtype` / `Rrtype`  (string): DNS record type
* `value` / `Value`  (string): DNS record value


---

## Show site DNS configuration

```
Task<DnsRecord[]> DnsRecord.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
