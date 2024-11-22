# FilesCom.Models.EmailLog

## Example EmailLog Object

```
{
  "timestamp": "2000-01-01T01:00:00Z",
  "message": "example",
  "status": "example",
  "subject": "example",
  "to": "example",
  "cc": "example",
  "delivery_method": "example",
  "smtp_hostname": "example",
  "smtp_ip": "example"
}
```

* `timestamp` / `Timestamp`  (Nullable<DateTime>): Start Time of Action
* `message` / `Message`  (string): Log Message
* `status` / `Status`  (string): Status of E-Mail delivery
* `subject` / `Subject`  (string): Subject line of E-Mail
* `to` / `To`  (string): To field of E-Mail
* `cc` / `Cc`  (string): CC field of E-Mail
* `delivery_method` / `DeliveryMethod`  (string): How was the email delivered?  `customer_smtp` or `files.com`
* `smtp_hostname` / `SmtpHostname`  (string): Customer SMTP Hostname used.
* `smtp_ip` / `SmtpIp`  (string): Customer SMTP IP address as resolved for use (useful for troubleshooting DNS issues with customer SMTP).


---

## List Email Logs

```
Task<FilesList<EmailLog>> EmailLog.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date` or `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ status ]`, `[ start_date, end_date ]`, `[ start_date, status ]` or `[ end_date, status ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ status ]`, `[ start_date, end_date ]`, `[ start_date, status ]` or `[ end_date, status ]`.


---

## Create an export CSV of Email Log resources

```
Task<Export> EmailLog.CreateExport(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `filter` (object): If set, return records where the specified field is equal to the supplied value. Valid fields are `start_date`, `end_date` or `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ status ]`, `[ start_date, end_date ]`, `[ start_date, status ]` or `[ end_date, status ]`.
* `filter_prefix` (object): If set, return records where the specified field is prefixed by the supplied value. Valid fields are `status`. Valid field combinations are `[ start_date ]`, `[ end_date ]`, `[ status ]`, `[ start_date, end_date ]`, `[ start_date, status ]` or `[ end_date, status ]`.
