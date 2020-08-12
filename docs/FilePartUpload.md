# Files.Models.FilePartUpload

## Example FilePartUpload Object

```
{
  "send": "",
  "action": "upload/direct",
  "ask_about_overwrites": true,
  "available_parts": "",
  "expires": "",
  "headers": "",
  "http_method": "POST",
  "next_partsize": "",
  "parallel_parts": true,
  "parameters": "",
  "part_number": "",
  "partsize": "",
  "path": "",
  "ref": "upload-1",
  "upload_uri": ""
}
```

* `send` / `Send`  (object): Content-Type and File to send
* `action` / `Action`  (string): Type of upload
* `ask_about_overwrites` / `AskAboutOverwrites`  (bool): If false, rename conflicting files instead of asking for overwrite confirmation
* `available_parts` / `AvailableParts`  (string): Currently unused
* `expires` / `Expires`  (string): Currently unused
* `headers` / `Headers`  (object): Additional upload headers
* `http_method` / `HttpMethod`  (string): Upload method, usually POST
* `next_partsize` / `NextPartsize`  (string): Currently unused
* `parallel_parts` / `ParallelParts`  (bool): If true, parts may be uploaded in parallel
* `parameters` / `Parameters`  (string): Additional upload parameters
* `part_number` / `PartNumber`  (string): Currently unused
* `partsize` / `Partsize`  (string): Currently unused
* `path` / `Path`  (string): Upload path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `ref` / `Ref`  (string): Reference name for this upload part
* `upload_uri` / `UploadUri`  (string): URI to upload this part to
