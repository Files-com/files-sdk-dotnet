# FilesCom.Models.Folder

## Example Folder Object

```
{
  "path": "path/file.txt",
  "display_name": "file.txt",
  "type": "file",
  "size": 1024,
  "created_at": "2000-01-01T01:00:00Z",
  "mtime": "2000-01-01T01:00:00Z",
  "provided_mtime": "2000-01-01T01:00:00Z",
  "crc32": "70976923",
  "md5": "17c54824e9931a4688ca032d03f6663c",
  "mime_type": "application/octet-stream",
  "region": "us-east-1",
  "permissions": "rwd",
  "subfolders_locked?": true,
  "is_locked": true,
  "download_uri": "https://mysite.files.com/...",
  "priority_color": "red",
  "preview_id": 1,
  "preview": ""
}
```

* `path` / `Path`  (string): File/Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
* `display_name` / `DisplayName`  (string): File/Folder display name
* `type` / `Type`  (string): Type: `directory` or `file`.
* `size` / `Size`  (Nullable<Int64>): File/Folder size
* `created_at` / `CreatedAt`  (Nullable<DateTime>): File created date/time
* `mtime` / `Mtime`  (Nullable<DateTime>): File last modified date/time, according to the server.  This is the timestamp of the last Files.com operation of the file, regardless of what modified timestamp was sent.
* `provided_mtime` / `ProvidedMtime`  (Nullable<DateTime>): File last modified date/time, according to the client who set it.  Files.com allows desktop, FTP, SFTP, and WebDAV clients to set modified at times.  This allows Desktop<->Cloud syncing to preserve modified at times.
* `crc32` / `Crc32`  (string): File CRC32 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
* `md5` / `Md5`  (string): File MD5 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
* `mime_type` / `MimeType`  (string): MIME Type.  This is determined by the filename extension and is not stored separately internally.
* `region` / `Region`  (string): Region location
* `permissions` / `Permissions`  (string): A short string representing the current user's permissions.  Can be `r`,`w`,`d`, `l` or any combination
* `subfolders_locked?` / `SubfoldersLocked`  (bool): Are subfolders locked and unable to be modified?
* `is_locked` / `IsLocked`  (bool): Is this folder locked and unable to be modified?
* `download_uri` / `DownloadUri`  (string): Link to download file. Provided only in response to a download request.
* `priority_color` / `PriorityColor`  (string): Bookmark/priority color of file/folder
* `preview_id` / `PreviewId`  (Nullable<Int64>): File preview ID
* `preview` / `Preview`  (Preview): File preview
* `mkdir_parents` / `MkdirParents`  (bool): Create parent directories if they do not exist?


---

## List Folders by path

```
Task<RemoteFile[]> Folder.ListFor(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `path` (string): Required - Path to operate on.
* `filter` (string): If specified, will filter folders/files list by this string.  Wildcards of `*` and `?` are acceptable here.
* `preview_size` (string): Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
* `search` (string): If `search_all` is `true`, provide the search string here.  Otherwise, this parameter acts like an alias of `filter`.
* `search_all` (bool): Search entire site?  If set, we will ignore the folder path provided and search the entire site.  This is the same API used by the search bar in the UI.  Search results are a best effort, not real time, and not guaranteed to match every file.  This field should only be used for ad-hoc (human) searching, and not as part of an automated process.
* `with_previews` (bool): Include file previews?
* `with_priority_color` (bool): Include file priority color information?


---

## Create folder

```
Task<RemoteFile> Folder.Create(
    string path, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `path` (string): Required - Path to operate on.
* `mkdir_parents` (bool): Create parent directories if they do not exist?
* `provided_mtime` (string): User provided modification time.
