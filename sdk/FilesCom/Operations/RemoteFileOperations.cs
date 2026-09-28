using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Operations
{
    // Imported inside the namespace so that model names, such as Action, take precedence over System's.
    using FilesCom.Models;

    /// <summary>
    /// RemoteFile operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.RemoteFiles"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="RemoteFile"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class RemoteFileOperations
    {
        private readonly FilesClient client;

        internal RemoteFileOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a RemoteFile that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public RemoteFile New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            RemoteFile model = new RemoteFile(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Download File
        ///
        /// Parameters:
        ///   action - string - Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        public Task<RemoteFile> DownloadAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.DownloadCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   action - string - The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
        ///   custom_metadata - object - Custom metadata map to save when `action=end` completes the upload.  Replaces existing metadata; an empty map clears it.  No separate metadata-edit permission is required.  Supported on native files and configured remote mounts, excluding remote server automount paths.  Limited to 32 keys, 256 characters per key and 1024 characters per value.
        ///   etags[etag] (required) - array(string) - etag identifier.
        ///   etags[part] (required) - array(int64) - Part number.
        ///   length - int64 - Length of file.
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   provided_mtime - string - User provided modification time.
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Size of file.
        ///   copy_behaviors - boolean - If copying a folder, also copy supported behaviors, email notification subscriptions, and per-folder branding to the destination folder tree?
        ///   structure - string - If copying folder, copy just the structure?
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        ///   buffered_upload - boolean - If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        public Task<RemoteFile> CreateAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.CreateCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   custom_metadata - object - Custom metadata map of keys and values. Limited to 32 keys, 256 characters per key and 1024 characters per value.
        ///   provided_mtime - string - Modified time of file.
        ///   priority_color - string - Priority/Bookmark color of file.
        /// </summary>
        public Task<RemoteFile> UpdateAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.UpdateCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        public Task DeleteAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.DeleteCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folders.  Otherwise, will error on non-empty folders.
        /// </summary>
        public Task DestroyAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.DeleteCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        public Task<RemoteFile> FindAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.FindCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        public Task<RemoteFile> GetAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.FindCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// List the contents of a ZIP file
        /// </summary>
        public Task<ZipListEntry[]> ZipListContentsAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.ZipListContentsCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Copy File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Copy destination path.
        ///   copy_behaviors - boolean - If copying a folder, also copy supported behaviors, email notification subscriptions, and per-folder branding to the destination folder tree?
        ///   structure - boolean - Copy structure only?
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        public Task<FileAction> CopyAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.CopyCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Move File/Folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        ///   overwrite - boolean - Overwrite existing file(s) in the destination?
        /// </summary>
        public Task<FileAction> MoveAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.MoveCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Transform a file and save the output to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the transformed output.
        ///   transform_type (required) - string - Transform type. Supported values are `image_convert`, `document_convert`, and `files_transform_script_execute`.
        ///   target_format (required) - string - Destination format to create.
        ///   script - string - Files TransformScript source. Required when transform_type is `files_transform_script_execute`.
        ///   width - int64 - Maximum output width for image_convert.
        ///   height - int64 - Maximum output height for image_convert.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        public Task<FileAction> TransformAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.TransformCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Decrypt a GPG-encrypted file and save it to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the decrypted file.
        ///   gpg_key_ids - array(int64) - GPG Key IDs to decrypt with. If omitted, every accessible private GPG key in the source workspace is used.
        ///   gpg_key_partner_id - int64 - Partner ID whose GPG keys should be used for decryption.
        ///   use_all_private_keys - boolean - Use every accessible private GPG key in the source workspace for decryption.
        ///   ignore_mdc_error - boolean - Ignore errors from the MDC (modification detection code) check.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        public Task<FileAction> GpgDecryptAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.GpgDecryptCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Encrypt a file with GPG and save it to a destination path
        ///
        /// Parameters:
        ///   destination (required) - string - Destination file path for the encrypted file.
        ///   gpg_key_ids - array(int64) - GPG Key IDs to encrypt with.
        ///   gpg_key_partner_id - int64 - Partner ID whose GPG keys should be used for encryption.
        ///   signing_key_id - int64 - Optional GPG Key ID to sign with.
        ///   armor - boolean - Output ASCII-armored encrypted data.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        public Task<FileAction> GpgEncryptAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.GpgEncryptCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Extract a ZIP file to a destination folder
        ///
        /// Parameters:
        ///   destination (required) - string - Destination folder path for extracted files.
        ///   filename - string - Optional single entry filename to extract.
        ///   overwrite - boolean - Overwrite existing files in the destination?
        /// </summary>
        public Task<FileAction> UnzipAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.UnzipCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Parameters:
        ///   paths (required) - array(string) - Paths to include in the ZIP.
        ///   destination (required) - string - Destination file path for the ZIP.
        ///   overwrite - boolean - Overwrite existing file in the destination?
        /// </summary>
        public Task<FileAction> ZipAsync(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.ZipCore(new OperationContext(client), DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Begin File Upload
        ///
        /// Parameters:
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Total bytes of file being uploaded (include bytes being retained if appending/restarting).
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        ///   buffered_upload - boolean - If true, and the path refers to a destination not stored on Files.com (such as a remote server mount), the upload will be uploaded first to Files.com before being sent to the remote server mount. This can allow clients to upload using parallel parts to a remote server destination that does not offer parallel parts support natively.
        ///   with_direct_connection_info - boolean - Include optional direct connection information for a direct Agent transfer attempt?
        /// </summary>
        public Task<FileUploadPart[]> BeginUploadAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return RemoteFile.BeginUploadCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Downloads the file at <paramref name="path"/> to <paramref name="localPath"/>, creating or replacing it.
        /// </summary>
        /// <param name="path">The remote path of the file.</param>
        /// <param name="localPath">The local file to write. If null, a file with the remote file's name in the current directory.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the download.</param>
        /// <returns>The downloaded file's details.</returns>
        /// <remarks>
        /// The local file is closed when the task ends, whether the download succeeds, fails or is cancelled. After a
        /// failure or cancellation it may be left partly written.
        /// </remarks>
        public Task<RemoteFile> DownloadFileAsync(string path, string localPath = null, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            return RemoteFile.DownloadFileCore(client, path, localPath, DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Downloads the file at <paramref name="path"/> into <paramref name="stream"/>, from the stream's current position.
        /// </summary>
        /// <param name="path">The remote path of the file.</param>
        /// <param name="stream">The stream to write to. It is borrowed: it is left open, including when the download fails or is cancelled.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="cancellationToken">Stops the download.</param>
        /// <returns>The downloaded file's details.</returns>
        public Task<RemoteFile> DownloadFileAsync(string path, System.IO.Stream stream, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            return RemoteFile.DownloadFileCore(client, path, stream, DictionaryUtil.Copy(options), cancellationToken);
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="localPath">The local file to upload. It is opened, and closed when the task ends.</param>
        /// <param name="destinationPath">The remote path to upload to. If null, the local file's name.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        /// <param name="cancellationToken">Stops the upload before its next request. If the final request has been sent, the upload may still complete.</param>
        public Task<bool> UploadFileAsync(string localPath, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return RemoteFile.UploadFileCore(new OperationContext(client), localPath, destinationPath, DictionaryUtil.Copy(options), DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Uploads <paramref name="fileLength"/> bytes from <paramref name="readStream"/> to <paramref name="destinationPath"/>.
        /// </summary>
        /// <param name="destinationPath">The remote path to upload to.</param>
        /// <param name="readStream">
        /// The stream to read from its current position. The upload takes ownership of it and disposes it when the task
        /// ends, whether the upload succeeds, fails or is cancelled.
        /// </param>
        /// <param name="fileLength">The number of bytes to upload. If the stream ends sooner, the upload fails with <see cref="System.IO.EndOfStreamException"/> and is not completed.</param>
        /// <param name="mTime">The file's modification time.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        /// <param name="parameters">Upload parameters, such as mkdir_parents.</param>
        /// <param name="cancellationToken">Stops the upload before its next request. If the final request has been sent, the upload may still complete.</param>
        public Task<bool> UploadFileAsync(string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return RemoteFile.UploadFileCore(new OperationContext(client), destinationPath, readStream, fileLength, mTime, DictionaryUtil.Copy(options), DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to the Remote Server with ID <paramref name="remoteServerId"/>,
        /// as <see cref="UploadFileAsync(string, string, Dictionary{string, object}, Dictionary{string, object}, CancellationToken)"/> does.
        /// </summary>
        public Task<bool> UploadToRemoteServerAsync(string localPath, long remoteServerId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(localPath, RemoteFile.UnderscoreUploadDestinationPath("RemoteServers", remoteServerId, localPath, destinationPath), options, parameters, cancellationToken);
        }

        /// <summary>
        /// Uploads <paramref name="fileLength"/> bytes from <paramref name="readStream"/> to the Remote Server with ID <paramref name="remoteServerId"/>,
        /// as <see cref="UploadFileAsync(string, System.IO.Stream, long, DateTime, Dictionary{string, object}, Dictionary{string, object}, CancellationToken)"/> does.
        /// </summary>
        public Task<bool> UploadToRemoteServerAsync(long remoteServerId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(RemoteFile.UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath), readStream, fileLength, mTime, options, parameters, cancellationToken);
        }

        /// <summary>
        /// Copies the file or folder at <paramref name="path"/> to the Remote Server with ID <paramref name="remoteServerId"/>.
        /// </summary>
        public Task<FileAction> CopyToRemoteServerAsync(string path, long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = RemoteFile.UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return CopyAsync(path, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Moves the file or folder at <paramref name="path"/> to the Remote Server with ID <paramref name="remoteServerId"/>.
        /// </summary>
        public Task<FileAction> MoveToRemoteServerAsync(string path, long remoteServerId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = RemoteFile.UnderscoreDestinationPath("RemoteServers", remoteServerId, destinationPath);
            return MoveAsync(path, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to the Snapshot with ID <paramref name="snapshotId"/>,
        /// as <see cref="UploadFileAsync(string, string, Dictionary{string, object}, Dictionary{string, object}, CancellationToken)"/> does.
        /// </summary>
        public Task<bool> UploadToSnapshotAsync(string localPath, long snapshotId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(localPath, RemoteFile.UnderscoreUploadDestinationPath("Snapshots", snapshotId, localPath, destinationPath), options, parameters, cancellationToken);
        }

        /// <summary>
        /// Uploads <paramref name="fileLength"/> bytes from <paramref name="readStream"/> to the Snapshot with ID <paramref name="snapshotId"/>,
        /// as <see cref="UploadFileAsync(string, System.IO.Stream, long, DateTime, Dictionary{string, object}, Dictionary{string, object}, CancellationToken)"/> does.
        /// </summary>
        public Task<bool> UploadToSnapshotAsync(long snapshotId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(RemoteFile.UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath), readStream, fileLength, mTime, options, parameters, cancellationToken);
        }

        /// <summary>
        /// Copies the file or folder at <paramref name="path"/> to the Snapshot with ID <paramref name="snapshotId"/>.
        /// </summary>
        public Task<FileAction> CopyToSnapshotAsync(string path, long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = RemoteFile.UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return CopyAsync(path, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Moves the file or folder at <paramref name="path"/> to the Snapshot with ID <paramref name="snapshotId"/>.
        /// </summary>
        public Task<FileAction> MoveToSnapshotAsync(string path, long snapshotId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = RemoteFile.UnderscoreDestinationPath("Snapshots", snapshotId, destinationPath);
            return MoveAsync(path, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Uploads the local file at <paramref name="localPath"/> to the Child Site with ID <paramref name="siteId"/>,
        /// as <see cref="UploadFileAsync(string, string, Dictionary{string, object}, Dictionary{string, object}, CancellationToken)"/> does.
        /// </summary>
        public Task<bool> UploadToChildSiteAsync(string localPath, long siteId, string destinationPath = null, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(localPath, RemoteFile.UnderscoreUploadDestinationPath("Sites", siteId, localPath, destinationPath), options, parameters, cancellationToken);
        }

        /// <summary>
        /// Uploads <paramref name="fileLength"/> bytes from <paramref name="readStream"/> to the Child Site with ID <paramref name="siteId"/>,
        /// as <see cref="UploadFileAsync(string, System.IO.Stream, long, DateTime, Dictionary{string, object}, Dictionary{string, object}, CancellationToken)"/> does.
        /// </summary>
        public Task<bool> UploadToChildSiteAsync(long siteId, string destinationPath, System.IO.Stream readStream, Int64 fileLength, DateTime mTime, Dictionary<string, object> options = null, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UploadFileAsync(RemoteFile.UnderscoreDestinationPath("Sites", siteId, destinationPath), readStream, fileLength, mTime, options, parameters, cancellationToken);
        }

        /// <summary>
        /// Copies the file or folder at <paramref name="path"/> to the Child Site with ID <paramref name="siteId"/>.
        /// </summary>
        public Task<FileAction> CopyToChildSiteAsync(string path, long siteId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = RemoteFile.UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return CopyAsync(path, parameters, options, cancellationToken);
        }

        /// <summary>
        /// Moves the file or folder at <paramref name="path"/> to the Child Site with ID <paramref name="siteId"/>.
        /// </summary>
        public Task<FileAction> MoveToChildSiteAsync(string path, long siteId, string destinationPath, Dictionary<string, object> parameters = null, Dictionary<string, object> options = null, CancellationToken cancellationToken = default)
        {
            parameters = parameters != null ? new Dictionary<string, object>(parameters) : new Dictionary<string, object>();
            parameters["destination"] = RemoteFile.UnderscoreDestinationPath("Sites", siteId, destinationPath);
            return MoveAsync(path, parameters, options, cancellationToken);
        }
    }
}