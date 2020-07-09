# Files.Models.Invoice

## Example Invoice Object

```
{
  "id": 1,
  "amount": "",
  "balance": "",
  "created_at": "2000-01-01T01:00:00Z",
  "currency": "USD",
  "download_uri": "https://url...",
  "invoice_line_items": [

  ],
  "method": "paypal",
  "payment_line_items": [

  ],
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
* `invoice_line_items` / `InvoiceLineItems`  (string[]): Associated invoice line items
* `method` / `Method`  (string): Line item payment method
* `payment_line_items` / `PaymentLineItems`  (string[]): Associated payment line items
* `payment_reversed_at` / `PaymentReversedAt`  (Nullable<DateTime>): Date/time payment was reversed if applicable
* `payment_type` / `PaymentType`  (string): Type of payment if applicable
* `site_name` / `SiteName`  (string): Site name this line item is for
* `type` / `Type`  (string): Type of line item, either payment or invoice
* `updated_at` / `UpdatedAt`  (Nullable<DateTime>): Line item updated at


---

## List Invoices

```
Task<Invoice[]> Invoice.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.


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
