# FilesCom.Models.InvoiceLineItem

## Example InvoiceLineItem Object

```
{
  "amount": 1.0,
  "created_at": "2000-01-01T01:00:00Z",
  "description": "Service from 2019-01-01 through 2019-12-31",
  "type": "invoice",
  "service_end_at": "2000-01-01T01:00:00Z",
  "service_start_at": "2000-01-01T01:00:00Z",
  "plan": "Premier",
  "site": "My site"
}
```

* `amount` / `Amount`  (double): Invoice line item amount
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Invoice line item created at date/time
* `description` / `Description`  (string): Invoice line item description
* `type` / `Type`  (string): Invoice line item type
* `service_end_at` / `ServiceEndAt`  (Nullable<DateTime>): Invoice line item service end date/time
* `service_start_at` / `ServiceStartAt`  (Nullable<DateTime>): Invoice line item service start date/time
* `plan` / `Plan`  (string): Plan name
* `site` / `Site`  (string): Site name
