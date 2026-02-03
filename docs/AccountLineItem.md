# FilesCom.Models.AccountLineItem

## Example AccountLineItem Object

```
{
  "id": 1,
  "amount": "1.0",
  "balance": "1.0",
  "created_at": "2000-01-01T01:00:00Z",
  "currency": "USD",
  "download_uri": "https://url...",
  "invoice_line_items": [
    {
      "id": 1,
      "amount": "1.0",
      "created_at": "2000-01-01T01:00:00Z",
      "description": "Service from 2019-01-01 through 2019-12-31",
      "type": "invoice",
      "service_end_at": "2000-01-01T01:00:00Z",
      "service_start_at": "2000-01-01T01:00:00Z",
      "plan": "Premier",
      "site": "My site",
      "prepaid_bytes": 1,
      "prepaid_bytes_expire_at": "2000-01-01T01:00:00Z",
      "prepaid_bytes_used": 1,
      "prepaid_bytes_available": 1
    }
  ],
  "method": "paypal",
  "payment_line_items": [
    {
      "amount": "1.0",
      "created_at": "2000-01-01T01:00:00Z",
      "invoice_id": 1,
      "payment_id": 1
    }
  ],
  "payment_reversed_at": "2000-01-01T01:00:00Z",
  "payment_type": "example",
  "site_name": "My Site",
  "type": "invoice"
}
```

* `id` / `Id`  (Nullable<Int64>): Line item Id
* `amount` / `Amount`  (decimal): Line item amount
* `balance` / `Balance`  (decimal): Line item balance
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Line item created at
* `currency` / `Currency`  (string): Line item currency
* `download_uri` / `DownloadUri`  (string): Line item download uri
* `invoice_line_items` / `InvoiceLineItems`  (object[]): Associated invoice line items
* `method` / `Method`  (string): Line item payment method
* `payment_line_items` / `PaymentLineItems`  (object[]): Associated payment line items
* `payment_reversed_at` / `PaymentReversedAt`  (Nullable<DateTime>): Date/time payment was reversed if applicable
* `payment_type` / `PaymentType`  (string): Type of payment if applicable
* `site_name` / `SiteName`  (string): Site name this line item is for
* `type` / `Type`  (string): Type of line item, either payment or invoice
