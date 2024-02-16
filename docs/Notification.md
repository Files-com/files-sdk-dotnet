# FilesCom.Models.Notification

## Example Notification Object

```
{
  "id": 1,
  "path": "",
  "group_id": 1,
  "group_name": "example",
  "triggering_group_ids": [
    1
  ],
  "triggering_user_ids": [
    1
  ],
  "trigger_by_share_recipients": true,
  "notify_user_actions": true,
  "notify_on_copy": true,
  "notify_on_delete": true,
  "notify_on_download": true,
  "notify_on_move": true,
  "notify_on_upload": true,
  "recursive": true,
  "send_interval": "fifteen_minutes",
  "message": "custom notification email message",
  "triggering_filenames": [
    "*.jpg",
    "notify_file.txt"
  ],
  "unsubscribed": true,
  "unsubscribed_reason": "example",
  "user_id": 1,
  "username": "User",
  "suppressed_email": true
}
```

* `id` / `Id`  (Nullable<Int64>): Notification ID
* `path` / `Path`  (string): Folder path to notify on This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `group_id` / `GroupId`  (Nullable<Int64>): ID of Group to receive notifications
* `group_name` / `GroupName`  (string): Group name, if a Group ID is set
* `triggering_group_ids` / `TriggeringGroupIds`  (Nullable<Int64>[]): If set, will only notify on actions made by a member of one of the specified groups
* `triggering_user_ids` / `TriggeringUserIds`  (Nullable<Int64>[]): If set, will onlynotify on actions made one of the specified users
* `trigger_by_share_recipients` / `TriggerByShareRecipients`  (bool): Notify when actions are performed by a share recipient?
* `notify_user_actions` / `NotifyUserActions`  (bool): If true, will send notifications about a user's own activity to that user.  If false, only activity performed by other users (or anonymous users) will be sent in notifications.
* `notify_on_copy` / `NotifyOnCopy`  (bool): Trigger on files copied to this path?
* `notify_on_delete` / `NotifyOnDelete`  (bool): Trigger on files deleted in this path?
* `notify_on_download` / `NotifyOnDownload`  (bool): Trigger on files downloaded in this path?
* `notify_on_move` / `NotifyOnMove`  (bool): Trigger on files moved to this path?
* `notify_on_upload` / `NotifyOnUpload`  (bool): Trigger on files created/uploaded/updated/changed in this path?
* `recursive` / `Recursive`  (bool): Apply notification recursively?  This will enable notifications for each subfolder.
* `send_interval` / `SendInterval`  (string): The time interval that notifications are aggregated to
* `message` / `Message`  (string): Custom message to include in notification emails
* `triggering_filenames` / `TriggeringFilenames`  (string[]): Array of filenames (possibly with wildcards) to scope trigger
* `unsubscribed` / `Unsubscribed`  (bool): Is the user unsubscribed from this notification?
* `unsubscribed_reason` / `UnsubscribedReason`  (string): The reason that the user unsubscribed
* `user_id` / `UserId`  (Nullable<Int64>): Notification user ID
* `username` / `Username`  (string): Notification username
* `suppressed_email` / `SuppressedEmail`  (bool): If true, it means that the recipient at this user's email address has manually unsubscribed from all emails, or had their email "hard bounce", which means that we are unable to send mail to this user's current email address. Notifications will resume if the user changes their email address.


---

## List Notifications

```
Task<FilesList<Notification>> Notification.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): DEPRECATED: Show notifications for this User ID. Use `filter[user_id]` instead.
* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `sort_by` (object): If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[path]=desc`). Valid fields are `path`, `user_id` or `group_id`.
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `user_id` or `group_id`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
* `path` (string): Show notifications for this Path.
* `include_ancestors` (bool): If `include_ancestors` is `true` and `path` is specified, include notifications for any parent paths. Ignored if `path` is not specified.
* `group_id` (string): 


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
* `notify_on_delete` (bool): Trigger on files deleted in this path?
* `notify_on_download` (bool): Trigger on files downloaded in this path?
* `notify_on_move` (bool): Trigger on files moved to this path?
* `notify_on_upload` (bool): Trigger on files created/uploaded/updated/changed in this path?
* `notify_user_actions` (bool): If `true` actions initiated by the user will still result in a notification
* `recursive` (bool): If `true`, enable notifications for each subfolder in this path
* `send_interval` (string): The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
* `message` (string): Custom message to include in notification emails
* `triggering_filenames` (string[]): Array of filenames (possibly with wildcards) to scope trigger
* `triggering_group_ids` (Nullable<Int64>[]): If set, will only notify on actions made by a member of one of the specified groups
* `triggering_user_ids` (Nullable<Int64>[]): If set, will onlynotify on actions made one of the specified users
* `trigger_by_share_recipients` (bool): Notify when actions are performed by a share recipient?
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
* `notify_on_delete` (bool): Trigger on files deleted in this path?
* `notify_on_download` (bool): Trigger on files downloaded in this path?
* `notify_on_move` (bool): Trigger on files moved to this path?
* `notify_on_upload` (bool): Trigger on files created/uploaded/updated/changed in this path?
* `notify_user_actions` (bool): If `true` actions initiated by the user will still result in a notification
* `recursive` (bool): If `true`, enable notifications for each subfolder in this path
* `send_interval` (string): The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
* `message` (string): Custom message to include in notification emails
* `triggering_filenames` (string[]): Array of filenames (possibly with wildcards) to scope trigger
* `triggering_group_ids` (Nullable<Int64>[]): If set, will only notify on actions made by a member of one of the specified groups
* `triggering_user_ids` (Nullable<Int64>[]): If set, will onlynotify on actions made one of the specified users
* `trigger_by_share_recipients` (bool): Notify when actions are performed by a share recipient?


---

## Delete Notification

```
Task Notification.Delete(
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
parameters.Add("notify_on_delete", true);
parameters.Add("notify_on_download", true);
parameters.Add("notify_on_move", true);
parameters.Add("notify_on_upload", true);
parameters.Add("notify_user_actions", true);
parameters.Add("recursive", true);
parameters.Add("send_interval", "daily");
parameters.Add("message", "custom notification email message");
parameters.Add("triggering_filenames", ["*.jpg","notify_file.txt"]);
parameters.Add("triggering_group_ids", [1]);
parameters.Add("triggering_user_ids", [1]);
parameters.Add("trigger_by_share_recipients", true);

Notification.Update(parameters);
```

### Parameters

* `id` (Nullable<Int64>): Required - Notification ID.
* `notify_on_copy` (bool): If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
* `notify_on_delete` (bool): Trigger on files deleted in this path?
* `notify_on_download` (bool): Trigger on files downloaded in this path?
* `notify_on_move` (bool): Trigger on files moved to this path?
* `notify_on_upload` (bool): Trigger on files created/uploaded/updated/changed in this path?
* `notify_user_actions` (bool): If `true` actions initiated by the user will still result in a notification
* `recursive` (bool): If `true`, enable notifications for each subfolder in this path
* `send_interval` (string): The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
* `message` (string): Custom message to include in notification emails
* `triggering_filenames` (string[]): Array of filenames (possibly with wildcards) to scope trigger
* `triggering_group_ids` (Nullable<Int64>[]): If set, will only notify on actions made by a member of one of the specified groups
* `triggering_user_ids` (Nullable<Int64>[]): If set, will onlynotify on actions made one of the specified users
* `trigger_by_share_recipients` (bool): Notify when actions are performed by a share recipient?


---

## Delete Notification

```
var Notification = Notification.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


Notification.Delete
```

### Parameters

* `id` (Nullable<Int64>): Required - Notification ID.
