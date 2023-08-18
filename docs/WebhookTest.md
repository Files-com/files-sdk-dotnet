# FilesCom.Models.WebhookTest

## Example WebhookTest Object

```
{
  "code": 200,
  "message": "",
  "status": "",
  "data": "example",
  "success": true
}
```

* `code` / `Code`  (Nullable<Int64>): Status HTTP code
* `message` / `Message`  (string): Error message
* `status` / `Status`  (string): Status message
* `data` / `Data`  (Auto): Additional data
* `success` / `Success`  (bool): The success status of the webhook test
* `url` / `Url`  (string): URL for testing the webhook.
* `method` / `Method`  (string): HTTP method(GET or POST).
* `encoding` / `Encoding`  (string): HTTP encoding method.  Can be JSON, XML, or RAW (form data).
* `headers` / `Headers`  (object): Additional request headers.
* `body` / `Body`  (object): Additional body parameters.
* `raw_body` / `RawBody`  (string): raw body text
* `file_as_body` / `FileAsBody`  (bool): Send the file data as the request body?
* `file_form_field` / `FileFormField`  (string): Send the file data as a named parameter in the request POST body
* `action` / `Action`  (string): action for test body
* `use_dedicated_ips` / `UseDedicatedIps`  (bool): Use dedicated IPs for sending the webhook?


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
* `raw_body` (string): raw body text
* `file_as_body` (bool): Send the file data as the request body?
* `file_form_field` (string): Send the file data as a named parameter in the request POST body
* `action` (string): action for test body
* `use_dedicated_ips` (bool): Use dedicated IPs for sending the webhook?
