# Files.Models.IpAddress

## Example IpAddress Object

```
{
  "id": "Site",
  "associated_with": "Site",
  "group_id": 1,
  "ip_addresses": [

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

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `cursor` (string): Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.


---

## List all possible public IP addresses

```
Task<IpAddress[]> IpAddress.GetReserved(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `cursor` (string): Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
