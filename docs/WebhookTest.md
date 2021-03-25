# Files.Models.WebhookTest

## Example WebhookTest Object

```
{
  "code": 200,
  "message": "",
  "status": "",
  "data": "",
  "success": true
}
```

* `code` / `Code`  (Nullable<Int64>): Status HTTP code
* `message` / `Message`  (string): Error message
* `status` / `Status`  (string): Status message
* `data` / `Data`  (object): Additional data
* `success` / `Success`  (bool): The success status of the webhook test
* `url` / `Url`  (string): URL for testing the webhook.
* `method` / `Method`  (string): HTTP method(GET or POST).
* `encoding` / `Encoding`  (string): HTTP encoding method.  Can be JSON, XML, or RAW (form data).
* `headers` / `Headers`  (object): Additional request headers.
* `body` / `Body`  (object): Additional body parameters.
* `action` / `Action`  (string): action for test body


---

## Create Webhook Test

```
Task<WebhookTest> WebhookTest.Create(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `url` (string): Required - URL for testing the webhook.
* `method` (string): HTTP method(GET or POST).
* `encoding` (string): HTTP encoding method.  Can be JSON, XML, or RAW (form data).
* `headers` (object): Additional request headers.
* `body` (object): Additional body parameters.
* `action` (string): action for test body
