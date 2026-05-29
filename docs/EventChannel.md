# FilesCom.Models.EventChannel

## Example EventChannel Object

```
{
  "id": 1,
  "name": "example",
  "description": "example",
  "enabled": true,
  "default_channel": true,
  "created_at": "2000-01-01T01:00:00Z",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Event Channel ID
* `name` / `Name`  (string): Event Channel name.
* `description` / `Description`  (string): Event Channel description.
* `enabled` / `Enabled`  (bool): Whether this Event Channel can dispatch events.
* `default_channel` / `DefaultChannel`  (bool): Whether this Event Channel is the default destination for newly published events.
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Event Channel create date/time.
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Event Channel update date/time.


---

## List Event Channels

```
Task<FilesList<EventChannel>> EventChannel.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `enabled` and `default_channel`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `enabled` and `default_channel`.


---

## Show Event Channel

```
Task<EventChannel> EventChannel.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Channel ID.


---

## Create Event Channel

```
Task<EventChannel> EventChannel.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `name` (string): Required - Event Channel name.
* `description` (string): Event Channel description.
* `enabled` (bool): Whether this Event Channel can dispatch events.
* `default_channel` (bool): Whether this Event Channel is the default destination for newly published events.


---

## Update Event Channel

```
Task<EventChannel> EventChannel.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Channel ID.
* `name` (string): Event Channel name.
* `description` (string): Event Channel description.
* `enabled` (bool): Whether this Event Channel can dispatch events.
* `default_channel` (bool): Whether this Event Channel is the default destination for newly published events.


---

## Delete Event Channel

```
Task EventChannel.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Channel ID.


---

## Update Event Channel

```
var EventChannel = EventChannel.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("name", "example");
parameters.Add("description", "example");
parameters.Add("enabled", true);
parameters.Add("default_channel", true);

EventChannel.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Channel ID.
* `name` (string): Event Channel name.
* `description` (string): Event Channel description.
* `enabled` (bool): Whether this Event Channel can dispatch events.
* `default_channel` (bool): Whether this Event Channel is the default destination for newly published events.


---

## Delete Event Channel

```
var EventChannel = EventChannel.Find(1);

var parameters = new Dictionary<string, object>();


EventChannel.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Event Channel ID.
