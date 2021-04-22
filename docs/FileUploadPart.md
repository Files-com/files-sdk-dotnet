# FilesCom.Models.FileUploadPart

## Example FileUploadPart Object

```
{
  "send": "",
  "action": "multipart",
  "ask_about_overwrites": true,
  "available_parts": 1,
  "expires": "",
  "headers": "",
  "http_method": "PUT",
  "next_partsize": 1,
  "parallel_parts": true,
  "parameters": "{}",
  "part_number": 1,
  "partsize": 1,
  "path": "",
  "ref": "upload-1",
  "upload_uri": ""
}
```

* `send` / `Send`  (object): Content-Type and File to send
* `action` / `Action`  (string): Type of upload
* `ask_about_overwrites` / `AskAboutOverwrites`  (bool): If `true`, this file exists and you may wish to ask the user for overwrite confirmation
* `available_parts` / `AvailableParts`  (Nullable<Int64>): Number of parts in the upload
* `expires` / `Expires`  (string): Date/time of when this Upload part expires and the URL cannot be used any more
* `headers` / `Headers`  (object): Additional upload headers to provide as part of the upload
* `http_method` / `HttpMethod`  (string): HTTP Method to use for uploading the part, usually `PUT`
* `next_partsize` / `NextPartsize`  (Nullable<Int64>): Size in bytes for this part
* `parallel_parts` / `ParallelParts`  (bool): If `true`, multiple parts may be uploaded in parallel.  If `false`, be sure to only upload one part at a time, in order.
* `parameters` / `Parameters`  (object): Additional HTTP parameters to send with the upload
* `part_number` / `PartNumber`  (Nullable<Int64>): Number of this upload part
* `partsize` / `Partsize`  (Nullable<Int64>): Size in bytes for the next upload part
* `path` / `Path`  (string): New file path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `ref` / `Ref`  (string): Reference name for this upload part
* `upload_uri` / `UploadUri`  (string): URI to upload this part to
