# FilesCom.Models.PartnerSite



---

## Delete Partner Site

```
Task PartnerSite.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

With a client, which runs it with its own site and credentials and takes a cancellation token:

```
Task client.PartnerSites.DeleteAsync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null,
    CancellationToken cancellationToken = default
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site ID.


---

## Delete Partner Site

```
var partnerSite = new PartnerSite(new Dictionary<string, object> { { "id", 1L } }, null);

var parameters = new Dictionary<string, object>();

await partnerSite.Delete(parameters);
```

`DeleteAsync(parameters, cancellationToken)` does the same and can be cancelled. The object sends it with the client it came from.

### Parameters

* `id` (Nullable<Int64>): Required - Partner Site ID.
