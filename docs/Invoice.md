# FilesCom.Models.Invoice

## Example Invoice Object

```
{
  "id": 1,
  "amount": 1.0,
  "balance": 1.0,
  "created_at": "2000-01-01T01:00:00Z",
  "currency": "USD",
  "download_uri": "https://url...",
  "invoice_line_items": {
    "amount": 1.0,
    "created_at": "2000-01-01T01:00:00Z",
    "description": "Service from 2019-01-01 through 2019-12-31",
    "type": "invoice",
    "service_end_at": "2000-01-01T01:00:00Z",
    "service_start_at": "2000-01-01T01:00:00Z",
    "updated_at": "2000-01-01T01:00:00Z",
    "plan": "Enterprise",
    "site": "My site"
  },
  "method": "paypal",
  "payment_line_items": {
    "amount": 1.0,
    "created_at": "2000-01-01T01:00:00Z",
    "invoice_id": 1,
    "payment_id": 1,
    "updated_at": "2000-01-01T01:00:00Z"
  },
  "payment_reversed_at": "2000-01-01T01:00:00Z",
  "payment_type": "",
  "site_name": "My Site",
  "type": "invoice",
  "updated_at": "2000-01-01T01:00:00Z"
}
```

* `id` / `Id`  (Nullable<Int64>): Line item Id
* `amount` / `Amount`  (double): Line item amount
* `balance` / `Balance`  (double): Line item balance
* `created_at` / `CreatedAt`  (Nullable<DateTime>): Line item created at
* `currency` / `Currency`  (string): Line item currency
* `download_uri` / `DownloadUri`  (string): Line item download uri
* `invoice_line_items` / `InvoiceLineItems`  (object): Associated invoice line items
* `method` / `Method`  (string): Line item payment method
* `payment_line_items` / `PaymentLineItems`  (object): Associated payment line items
* `payment_reversed_at` / `PaymentReversedAt`  (Nullable<DateTime>): Date/time payment was reversed if applicable
* `payment_type` / `PaymentType`  (string): Type of payment if applicable
* `site_name` / `SiteName`  (string): Site name this line item is for
* `type` / `Type`  (string): Type of line item, either payment or invoice
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Line item updated at


---

## List Invoices

```
Task<AccountLineItem[]> Invoice.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Invoice

```
Task<AccountLineItem> Invoice.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Invoice ID.
