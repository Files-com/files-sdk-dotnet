# FilesCom.Models.RemoteFile

## Example RemoteFile Object

```
{
  "path": "path/file.txt",
  "display_name": "file.txt",
  "type": "file",
  "size": 1024,
  "mtime": "2000-01-01T01:00:00Z",
  "provided_mtime": "2000-01-01T01:00:00Z",
  "crc32": "70976923",
  "md5": "17c54824e9931a4688ca032d03f6663c",
  "mime_type": "application/octet-stream",
  "region": "us-east-1",
  "permissions": "rpw",
  "subfolders_locked?": true,
  "download_uri": "https://mysite.files.com/...",
  "priority_color": "red",
  "preview_id": 1,
  "preview": {
    "id": 1,
    "status": "complete",
    "download_uri": "https://mysite.files.com/...",
    "type": "complete",
    "size": 1024,
    "status_message": ""
  }
}
```

* `path` / `Path`  (string): File/Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `display_name` / `DisplayName`  (string): File/Folder display name
* `type` / `Type`  (string): Type: `directory` or `file`.
* `size` / `Size`  (Nullable<Int64>): File/Folder size
* `mtime` / `Mtime`  (Nullable<DateTime>): File last modified date/time, according to the server.  This is the timestamp of the last Files.com operation of the file, regardless of what modified timestamp was sent.
* `provided_mtime` / `ProvidedMtime`  (Nullable<DateTime>): File last modified date/time, according to the client who set it.  Files.com allows desktop, FTP, SFTP, and WebDAV clients to set modified at times.  This allows Desktop<->Cloud syncing to preserve modified at times.
* `crc32` / `Crc32`  (string): File CRC32 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
* `md5` / `Md5`  (string): File MD5 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
* `mime_type` / `MimeType`  (string): MIME Type.  This is determined by the filename extension and is not stored separately internally.
* `region` / `Region`  (string): Region location
* `permissions` / `Permissions`  (string): A short string representing the current user's permissions.  Can be `r`,`w`,`p`, or any combination
* `subfolders_locked?` / `SubfoldersLocked`  (bool): Are subfolders locked and unable to be modified?
* `download_uri` / `DownloadUri`  (string): Link to download file. Provided only in response to a download request.
* `priority_color` / `PriorityColor`  (string): Bookmark/priority color of file/folder
* `preview_id` / `PreviewId`  (Nullable<Int64>): File preview ID
* `preview` / `Preview`  (object): File preview
* `action` / `Action`  (string): The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
* `length` / `Length`  (Nullable<Int64>): Length of file.
* `mkdir_parents` / `MkdirParents`  (bool): Create parent directories if they do not exist?
* `part` / `Part`  (Nullable<Int64>): Part if uploading a part.
* `parts` / `Parts`  (Nullable<Int64>): How many parts to fetch?
* `ref` / `Ref`  (string): 
* `restart` / `Restart`  (Nullable<Int64>): File byte offset to restart from.
* `structure` / `Structure`  (string): If copying folder, copy just the structure?
* `with_rename` / `WithRename`  (bool): Allow file rename instead of overwrite?


---

## Download file

```
Task<RemoteFile> RemoteFile.Download(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `action` (string): Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
* `preview_size` (string): Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
* `with_previews` (bool): Include file preview information?
* `with_priority_color` (bool): Include file priority color information?


---

## Upload file

```
Task<RemoteFile> RemoteFile.Create(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `action` (string): The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
* `etags[etag]` (string[]): etag identifier.
* `etags[part]` (Nullable<Int64>[]): Part number.
* `length` (Nullable<Int64>): Length of file.
* `mkdir_parents` (bool): Create parent directories if they do not exist?
* `part` (Nullable<Int64>): Part if uploading a part.
* `parts` (Nullable<Int64>): How many parts to fetch?
* `provided_mtime` (string): User provided modification time.
* `ref` (string): 
* `restart` (Nullable<Int64>): File byte offset to restart from.
* `size` (Nullable<Int64>): Size of file.
* `structure` (string): If copying folder, copy just the structure?
* `with_rename` (bool): Allow file rename instead of overwrite?


---

## Update file/folder metadata

```
Task<RemoteFile> RemoteFile.Update(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `provided_mtime` (string): Modified time of file.
* `priority_color` (string): Priority/Bookmark color of file.


---

## Delete file/folder

```
Task<RemoteFile> RemoteFile.Delete(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `recursive` (bool): If true, will recursively delete folers.  Otherwise, will error on non-empty folders.


---

## Find file/folder by path

```
Task<RemoteFile> RemoteFile.Find(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `preview_size` (string): Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
* `with_previews` (bool): Include file preview information?
* `with_priority_color` (bool): Include file priority color information?


---

## Copy file/folder

```
Task<RemoteFile> RemoteFile.Copy(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Copy destination path.
* `structure` (bool): Copy structure only?


---

## Move file/folder

```
Task<RemoteFile> RemoteFile.Move(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Move destination path.


---

## Begin file upload

```
Task<FileUploadPart[]> RemoteFile.BeginUpload(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `mkdir_parents` (bool): Create parent directories if they do not exist?
* `part` (Nullable<Int64>): Part if uploading a part.
* `parts` (Nullable<Int64>): How many parts to fetch?
* `ref` (string): 
* `restart` (Nullable<Int64>): File byte offset to restart from.
* `with_rename` (bool): Allow file rename instead of overwrite?


---

## Download file

```
var File = RemoteFile.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("with_previews", true);
parameters.Add("with_priority_color", true);

File.Download(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `action` (string): Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
* `preview_size` (string): Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
* `with_previews` (bool): Include file preview information?
* `with_priority_color` (bool): Include file priority color information?


---

## Update file/folder metadata

```
var File = RemoteFile.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("provided_mtime", "2000-01-01T01:00:00Z");
parameters.Add("priority_color", "red");

File.Update(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `provided_mtime` (string): Modified time of file.
* `priority_color` (string): Priority/Bookmark color of file.


---

## Delete file/folder

```
var File = RemoteFile.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("recursive", true);

File.Delete(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `recursive` (bool): If true, will recursively delete folers.  Otherwise, will error on non-empty folders.


---

## Copy file/folder

```
var File = RemoteFile.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("destination", "destination");
parameters.Add("structure", true);

File.Copy(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Copy destination path.
* `structure` (bool): Copy structure only?


---

## Move file/folder

```
var File = RemoteFile.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("destination", "destination");

File.Move(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Move destination path.


---

## Begin file upload

```
var File = RemoteFile.ListFor(path)[0];

var parameters = new Dictionary<string, object>();

parameters.Add("mkdir_parents", true);
parameters.Add("part", 1);
parameters.Add("parts", 1);
parameters.Add("ref", "upload-1");
parameters.Add("restart", 1);
parameters.Add("with_rename", true);

File.BeginUpload(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `mkdir_parents` (bool): Create parent directories if they do not exist?
* `part` (Nullable<Int64>): Part if uploading a part.
* `parts` (Nullable<Int64>): How many parts to fetch?
* `ref` (string): 
* `restart` (Nullable<Int64>): File byte offset to restart from.
* `with_rename` (bool): Allow file rename instead of overwrite?
