# Files.Models.DnsRecord

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

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
