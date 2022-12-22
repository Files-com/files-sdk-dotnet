# FilesCom.Models.BundleNotification

## Example BundleNotification Object

```
{
  "bundle_id": 1,
  "id": 1,
  "notify_on_registration": true,
  "notify_on_upload": true,
  "user_id": 1
}
```

* `bundle_id` / `BundleId`  (Nullable<Int64>): Bundle ID to notify on
* `id` / `Id`  (Nullable<Int64>): Bundle Notification ID
* `notify_on_registration` / `NotifyOnRegistration`  (bool): Triggers bundle notification when a registration action occurs for it.
* `notify_on_upload` / `NotifyOnUpload`  (bool): Triggers bundle notification when a upload action occurs for it.
* `user_id` / `UserId`  (Nullable<Int64>): The id of the user to notify.


---

## List Bundle Notifications

```
Task<BundleNotification[]> BundleNotification.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `bundle_id` (Nullable<Int64>): Bundle ID to notify on


---

## Show Bundle Notification

```
Task<BundleNotification> BundleNotification.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle Notification ID.


---

## Create Bundle Notification

```
Task<BundleNotification> BundleNotification.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): Required - The id of the user to notify.
* `notify_on_registration` (bool): Triggers bundle notification when a registration action occurs for it.
* `notify_on_upload` (bool): Triggers bundle notification when a upload action occurs for it.
* `bundle_id` (Nullable<Int64>): Required - Bundle ID to notify on


---

## Update Bundle Notification

```
Task<BundleNotification> BundleNotification.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle Notification ID.
* `notify_on_registration` (bool): Triggers bundle notification when a registration action occurs for it.
* `notify_on_upload` (bool): Triggers bundle notification when a upload action occurs for it.


---

## Delete Bundle Notification

```
Task<BundleNotification> BundleNotification.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle Notification ID.


---

## Update Bundle Notification

```
var BundleNotification = BundleNotification.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("notify_on_registration", true);
parameters.Add("notify_on_upload", true);

BundleNotification.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle Notification ID.
* `notify_on_registration` (bool): Triggers bundle notification when a registration action occurs for it.
* `notify_on_upload` (bool): Triggers bundle notification when a upload action occurs for it.


---

## Delete Bundle Notification

```
var BundleNotification = BundleNotification.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


BundleNotification.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Bundle Notification ID.
