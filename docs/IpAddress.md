# Files.Models.IpAddress

## Example IpAddress Object

```
{
  "id": "Site",
  "associated_with": "Site",
  "group_id": 1,
  "ip_addresses": [
    "127.0.0.1"
  ]
}
```

* `id` / `Id`  (string): Unique label for list; used by Zapier and other integrations.
* `associated_with` / `AssociatedWith`  (string): The object that this public IP address list is associated with.
* `group_id` / `GroupId`  (Nullable<Int64>): Group ID
* `ip_addresses` / `IpAddresses`  (string[]): A list of IP addresses.


---

## List IP Addresses associated with the current site

```
Task<IpAddress[]> IpAddress.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## List all possible public IP addresses

```
Task<IpAddress[]> IpAddress.GetReserved(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
