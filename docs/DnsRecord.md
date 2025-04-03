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

## Show Site DNS Configuration

```
Task<FilesList<DnsRecord>> DnsRecord.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
