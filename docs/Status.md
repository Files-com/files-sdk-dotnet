# Files.Models.Status

## Example Status Object

```
{
  "code": 200,
  "message": "",
  "status": "",
  "data": "",
  "errors": {
    "fields": [

    ],
    "messages": [

    ]
  },
  "clickwrap_id": 1,
  "clickwrap_body": ""
}
```

* `code` / `Code`  (Nullable<Int64>): Status HTTP code
* `message` / `Message`  (string): Error message
* `status` / `StatusType`  (string): Status message
* `data` / `Data`  (object): Additional data
* `errors` / `Errors`  (object): A list of api errors
* `clickwrap_id` / `ClickwrapId`  (Nullable<Int64>): Required Clickwrap id
* `clickwrap_body` / `ClickwrapBody`  (string): Required Clickwrap body
