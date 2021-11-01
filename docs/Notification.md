# FilesCom.Models.Notification

## Example Notification Object

```
{
  "id": 1,
  "path": "",
  "group_id": 1,
  "group_name": "",
  "notify_user_actions": true,
  "notify_on_copy": true,
  "recursive": true,
  "send_interval": "fifteen_minutes",
  "unsubscribed": true,
  "unsubscribed_reason": "",
  "user_id": 1,
  "username": "User",
  "suppressed_email": true
}
```

* `id` / `Id`  (Nullable<Int64>): Notification ID
* `path` / `Path`  (string): Folder path to notify on This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `group_id` / `GroupId`  (Nullable<Int64>): Notification group id
* `group_name` / `GroupName`  (string): Group name if applicable
* `notify_user_actions` / `NotifyUserActions`  (bool): Trigger notification on notification user actions?
* `notify_on_copy` / `NotifyOnCopy`  (bool): Triggers notification when moving or copying files to this path
* `recursive` / `Recursive`  (bool): Enable notifications for each subfolder in this path
* `send_interval` / `SendInterval`  (string): The time interval that notifications are aggregated to
* `unsubscribed` / `Unsubscribed`  (bool): Is the user unsubscribed from this notification?
* `unsubscribed_reason` / `UnsubscribedReason`  (string): The reason that the user unsubscribed
* `user_id` / `UserId`  (Nullable<Int64>): Notification user ID
* `username` / `Username`  (string): Notification username
* `suppressed_email` / `SuppressedEmail`  (bool): If true, it means that the recipient at this user's email address has manually unsubscribed from all emails, or had their email "hard bounce", which means that we are unable to send mail to this user's current email address. Notifications will resume if the user changes their email address.


---

## List Notifications

```
Task<Notification[]> Notification.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): DEPRECATED: Show notifications for this User ID. Use `filter[user_id]` instead.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `path`, `user_id` or `group_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `user_id`, `group_id` or `path`.
* `filter_gt` (object): If set, return records where the specified field is greater than the supplied value. Valid fields are `user_id`, `group_id` or `path`.
* `filter_gteq` (object): If set, return records where the specified field is greater than or equal to the supplied value. Valid fields are `user_id`, `group_id` or `path`.
* `filter_like` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `user_id`, `group_id` or `path`.
* `filter_lt` (object): If set, return records where the specified field is less than the supplied value. Valid fields are `user_id`, `group_id` or `path`.
* `filter_lteq` (object): If set, return records where the specified field is less than or equal to the supplied value. Valid fields are `user_id`, `group_id` or `path`.
* `group_id` (Nullable<Int64>): DEPRECATED: Show notifications for this Group ID. Use `filter[group_id]` instead.
* `path` (string): Show notifications for this Path.
* `include_ancestors` (bool): If `include_ancestors` is `true` and `path` is specified, include notifications for any parent paths. Ignored if `path` is not specified.


---

## Show Notification

```
Task<Notification> Notification.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Notification ID.


---

## Create Notification

```
Task<Notification> Notification.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): The id of the user to notify. Provide `user_id`, `username` or `group_id`.
* `notify_on_copy` (bool): If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
* `notify_user_actions` (bool): If `true` actions initiated by the user will still result in a notification
* `recursive` (bool): If `true`, enable notifications for each subfolder in this path
* `send_interval` (string): The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
* `group_id` (Nullable<Int64>): The ID of the group to notify.  Provide `user_id`, `username` or `group_id`.
* `path` (string): Path
* `username` (string): The username of the user to notify.  Provide `user_id`, `username` or `group_id`.


---

## Update Notification

```
Task<Notification> Notification.Update(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Notification ID.
* `notify_on_copy` (bool): If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
* `notify_user_actions` (bool): If `true` actions initiated by the user will still result in a notification
* `recursive` (bool): If `true`, enable notifications for each subfolder in this path
* `send_interval` (string): The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.


---

## Delete Notification

```
Task<Notification> Notification.Delete(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Notification ID.


---

## Update Notification

```
var Notification = Notification.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("notify_on_copy", true);
parameters.Add("notify_user_actions", true);
parameters.Add("recursive", true);
parameters.Add("send_interval", "daily");

Notification.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Notification ID.
* `notify_on_copy` (bool): If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
* `notify_user_actions` (bool): If `true` actions initiated by the user will still result in a notification
* `recursive` (bool): If `true`, enable notifications for each subfolder in this path
* `send_interval` (string): The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.


---

## Delete Notification

```
var Notification = Notification.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Notification.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Notification ID.
