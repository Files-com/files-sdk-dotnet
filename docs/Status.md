# Files.Models.Status

## Example Status Object

```
{
  "code": 200,
  "message": "",
  "status": "",
  "data": "",
  "errors": [

  ]
}
```

* `code` / `Code`  (Nullable<Int64>): Status http code
* `message` / `Message`  (string): Error message
* `status` / `StatusType`  (string): Status message
* `data` / `Data`  (object): Additional data
* `errors` / `Errors`  (string[]): A list of api errors
