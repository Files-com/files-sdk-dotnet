# FilesCom.Models.AutomationRun

## Example AutomationRun Object

```
{
  "automation_id": 1,
  "status": "success",
  "status_messages_url": "https://www.example.com/log_file.txt"
}
```

* `automation_id` / `AutomationId`  (Nullable<Int64>): ID of the associated Automation.
* `status` / `Status`  (string): The success status of the AutomationRun. One of `running`, `success`, `partial_failure`, or `failure`.
* `status_messages_url` / `StatusMessagesUrl`  (string): Link to status messages log file.


---

## List Automation Runs

```
Task<AutomationRun[]> AutomationRun.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `user_id` (Nullable<Int64>): User ID.  Provide a value of `0` to operate the current session's user.
* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `automation_id` (Nullable<Int64>): Required - ID of the associated Automation.
