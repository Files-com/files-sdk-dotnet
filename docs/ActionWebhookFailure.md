

---

## retry Action Webhook Failure

```
Task<ActionWebhookFailure> ActionWebhookFailure.Retry(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Action Webhook Failure ID.


---

## retry Action Webhook Failure

```
var ActionWebhookFailure = ActionWebhookFailure.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


ActionWebhookFailure.Retry
```

### Parameters

* `id` (Nullable<Int64>): Required - Action Webhook Failure ID.
