# FilesCom.Models.RemoteFile

## Example RemoteFile Object

```
{
  "path": "path/file.txt",
  "created_by_id": 1,
  "created_by_api_key_id": 1,
  "created_by_as2_incoming_message_id": 1,
  "created_by_automation_id": 1,
  "created_by_bundle_registration_id": 1,
  "created_by_inbox_id": 1,
  "created_by_remote_server_id": 1,
  "created_by_remote_server_sync_id": 1,
  "custom_metadata": {
    "key": "value"
  },
  "display_name": "file.txt",
  "type": "file",
  "size": 1024,
  "created_at": "2000-01-01T01:00:00Z",
  "last_modified_by_id": 1,
  "last_modified_by_api_key_id": 1,
  "last_modified_by_automation_id": 1,
  "last_modified_by_bundle_registration_id": 1,
  "last_modified_by_remote_server_id": 1,
  "last_modified_by_remote_server_sync_id": 1,
  "mtime": "2000-01-01T01:00:00Z",
  "provided_mtime": "2000-01-01T01:00:00Z",
  "crc32": "70976923",
  "md5": "17c54824e9931a4688ca032d03f6663c",
  "sha1": "a94a8fe5ccb19ba61c4c0873d391e987982fbbd3",
  "sha256": "9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08",
  "mime_type": "application/octet-stream",
  "region": "us-east-1",
  "permissions": "rwd",
  "subfolders_locked?": true,
  "is_locked": true,
  "download_uri": "https://mysite.files.com/...",
  "priority_color": "red",
  "preview_id": 1,
  "preview": {
    "id": 1,
    "status": "complete",
    "download_uri": "https://mysite.files.com/...",
    "type": "image",
    "size": "large"
  }
}
```

* `path` / `Path`  (string): File/Folder path. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `created_by_id` / `CreatedById`  (Nullable<Int64>): User ID of the User who created the file/folder
* `created_by_api_key_id` / `CreatedByApiKeyId`  (Nullable<Int64>): ID of the API key that created the file/folder
* `created_by_as2_incoming_message_id` / `CreatedByAs2IncomingMessageId`  (Nullable<Int64>): ID of the AS2 Incoming Message that created the file/folder
* `created_by_automation_id` / `CreatedByAutomationId`  (Nullable<Int64>): ID of the Automation that created the file/folder
* `created_by_bundle_registration_id` / `CreatedByBundleRegistrationId`  (Nullable<Int64>): ID of the Bundle Registration that created the file/folder
* `created_by_inbox_id` / `CreatedByInboxId`  (Nullable<Int64>): ID of the Inbox that created the file/folder
* `created_by_remote_server_id` / `CreatedByRemoteServerId`  (Nullable<Int64>): ID of the Remote Server that created the file/folder
* `created_by_remote_server_sync_id` / `CreatedByRemoteServerSyncId`  (Nullable<Int64>): ID of the Remote Server Sync that created the file/folder
* `custom_metadata` / `CustomMetadata`  (object): Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
* `display_name` / `DisplayName`  (string): File/Folder display name
* `type` / `Type`  (string): Type: `directory` or `file`.
* `size` / `Size`  (Nullable<Int64>): File/Folder size
* `created_at` / `CreatedAt`  (Nullable<DateTime>): File created date/time
* `last_modified_by_id` / `LastModifiedById`  (Nullable<Int64>): User ID of the User who last modified the file/folder
* `last_modified_by_api_key_id` / `LastModifiedByApiKeyId`  (Nullable<Int64>): ID of the API key that last modified the file/folder
* `last_modified_by_automation_id` / `LastModifiedByAutomationId`  (Nullable<Int64>): ID of the Automation that last modified the file/folder
* `last_modified_by_bundle_registration_id` / `LastModifiedByBundleRegistrationId`  (Nullable<Int64>): ID of the Bundle Registration that last modified the file/folder
* `last_modified_by_remote_server_id` / `LastModifiedByRemoteServerId`  (Nullable<Int64>): ID of the Remote Server that last modified the file/folder
* `last_modified_by_remote_server_sync_id` / `LastModifiedByRemoteServerSyncId`  (Nullable<Int64>): ID of the Remote Server Sync that last modified the file/folder
* `mtime` / `Mtime`  (Nullable<DateTime>): File last modified date/time, according to the server.  This is the timestamp of the last Files.com operation of the file, regardless of what modified timestamp was sent.
* `provided_mtime` / `ProvidedMtime`  (Nullable<DateTime>): File last modified date/time, according to the client who set it.  Files.com allows desktop, FTP, SFTP, and WebDAV clients to set modified at times.  This allows Desktop<->Cloud syncing to preserve modified at times.
* `crc32` / `Crc32`  (string): File CRC32 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
* `md5` / `Md5`  (string): File MD5 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
* `sha1` / `Sha1`  (string): File SHA1 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
* `sha256` / `Sha256`  (string): File SHA256 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
* `mime_type` / `MimeType`  (string): MIME Type.  This is determined by the filename extension and is not stored separately internally.
* `region` / `Region`  (string): Region location
* `permissions` / `Permissions`  (string): A short string representing the current user's permissions.  Can be `r` (Read),`w` (Write),`d` (Delete), `l` (List) or any combination
* `subfolders_locked?` / `SubfoldersLocked`  (bool): Are subfolders locked and unable to be modified?
* `is_locked` / `IsLocked`  (bool): Is this folder locked and unable to be modified?
* `download_uri` / `DownloadUri`  (string): Link to download file. Provided only in response to a download request.
* `priority_color` / `PriorityColor`  (string): Bookmark/priority color of file/folder
* `preview_id` / `PreviewId`  (Nullable<Int64>): File preview ID
* `preview` / `Preview`  (Preview): File preview
* `action` / `Action`  (string): The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
* `length` / `Length`  (Nullable<Int64>): Length of file.
* `mkdir_parents` / `MkdirParents`  (bool): Create parent directories if they do not exist?
* `part` / `Part`  (Nullable<Int64>): Part if uploading a part.
* `parts` / `Parts`  (Nullable<Int64>): How many parts to fetch?
* `ref` / `Ref`  (string): 
* `restart` / `Restart`  (Nullable<Int64>): File byte offset to restart from.
* `structure` / `Structure`  (string): If copying folder, copy just the structure?
* `with_rename` / `WithRename`  (bool): Allow file rename instead of overwrite?
* `buffered_upload` / `BufferedUpload`  (bool): If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.


---

## Download File

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

## Upload File

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
* `buffered_upload` (bool): If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.


---

## Update File/Folder Metadata

```
Task<RemoteFile> RemoteFile.Update(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `custom_metadata` (object): Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
* `provided_mtime` (string): Modified time of file.
* `priority_color` (string): Priority/Bookmark color of file.


---

## Delete File/Folder

```
Task RemoteFile.Delete(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `recursive` (bool): If true, will recursively delete folders.  Otherwise, will error on non-empty folders.


---

## Find File/Folder by Path

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

## Copy File/Folder

```
Task<FileAction> RemoteFile.Copy(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Copy destination path.
* `structure` (bool): Copy structure only?
* `overwrite` (bool): Overwrite existing file(s) in the destination?


---

## Move File/Folder

```
Task<FileAction> RemoteFile.Move(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Move destination path.
* `overwrite` (bool): Overwrite existing file(s) in the destination?


---

## Begin File Upload

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
* `size` (Nullable<Int64>): Total bytes of file being uploaded (include bytes being retained if appending/restarting).
* `with_rename` (bool): Allow file rename instead of overwrite?


---

## Download File

```
var File = RemoteFile.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("with_previews", false);
parameters.Add("with_priority_color", false);

File.Download(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `action` (string): Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
* `preview_size` (string): Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
* `with_previews` (bool): Include file preview information?
* `with_priority_color` (bool): Include file priority color information?


---

## Update File/Folder Metadata

```
var File = RemoteFile.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("custom_metadata", {"key":"value"});
parameters.Add("provided_mtime", "2000-01-01T01:00:00Z");
parameters.Add("priority_color", "red");

File.Update(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `custom_metadata` (object): Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
* `provided_mtime` (string): Modified time of file.
* `priority_color` (string): Priority/Bookmark color of file.


---

## Delete File/Folder

```
var File = RemoteFile.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("recursive", false);

File.Delete(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `recursive` (bool): If true, will recursively delete folders.  Otherwise, will error on non-empty folders.


---

## Copy File/Folder

```
var File = RemoteFile.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("destination", "destination");
parameters.Add("structure", false);
parameters.Add("overwrite", false);

File.Copy(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Copy destination path.
* `structure` (bool): Copy structure only?
* `overwrite` (bool): Overwrite existing file(s) in the destination?


---

## Move File/Folder

```
var File = RemoteFile.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("destination", "destination");
parameters.Add("overwrite", false);

File.Move(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `destination` (string): Required - Move destination path.
* `overwrite` (bool): Overwrite existing file(s) in the destination?


---

## Begin File Upload

```
var File = RemoteFile.Find(1);

var parameters = new Dictionary<string, object>();

parameters.Add("mkdir_parents", false);
parameters.Add("part", 1);
parameters.Add("parts", 1);
parameters.Add("ref", "upload-1");
parameters.Add("restart", 1);
parameters.Add("size", 1);
parameters.Add("with_rename", false);

File.BeginUpload(parameters);
```

### Parameters

* `path` (string): Required - Path to operate on.
* `mkdir_parents` (bool): Create parent directories if they do not exist?
* `part` (Nullable<Int64>): Part if uploading a part.
* `parts` (Nullable<Int64>): How many parts to fetch?
* `ref` (string): 
* `restart` (Nullable<Int64>): File byte offset to restart from.
* `size` (Nullable<Int64>): Total bytes of file being uploaded (include bytes being retained if appending/restarting).
* `with_rename` (bool): Allow file rename instead of overwrite?
