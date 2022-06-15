# FilesCom.Models.Preview

## Example Preview Object

```
{
  "id": 1,
  "status": "complete",
  "download_uri": "https://mysite.files.com/...",
  "type": "image",
  "size": "large"
}
```

* `id` / `Id`  (Nullable<Int64>): Preview ID
* `status` / `Status`  (string): Preview status.  Can be invalid, not_generated, generating, complete, or file_too_large
* `download_uri` / `DownloadUri`  (string): Link to download preview
* `type` / `Type`  (string): Preview type. Can be image, pdf, pdf_native, video, or audio
* `size` / `Size`  (string): Preview size
