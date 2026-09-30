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
    /// Folder operations that run with one client: its endpoint, credentials and connections. Get them from
    /// <see cref="FilesClient.Folders"/>. The objects and lists they return keep using that client.
    /// </summary>
    /// <remarks>
    /// Each method matches the static <see cref="Folder"/> method of the same name. Methods that send a request
    /// are named with Async and take a cancellation token. The dictionaries you pass are copied, never changed.
    /// </remarks>
    public sealed class FolderOperations
    {
        private readonly FilesClient client;

        internal FolderOperations(FilesClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Makes a Folder that belongs to this client, without sending a request. Its methods, such as SaveAsync,
        /// then run with this client.
        /// </summary>
        public Folder New(Dictionary<string, object> attributes = null, Dictionary<string, object> options = null)
        {
            Folder model = new Folder(DictionaryUtil.Copy(attributes), null);
            ((IModel)model).SetContext(client, options);
            return model;
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   path (required) - string - Path to operate on.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   sort_by - object - Search by field and direction. Valid fields are `path`, `size`, `modified_at_datetime`, `provided_modified_at`.  Valid directions are `asc` and `desc`.  Defaults to `{"path":"asc"}`.
        ///   search - string - If specified, will search the folders/files list by name. Ignores text before last `/`. This is the same API used by the search bar in the web UI when running 'Search This Folder'.  Search results are a best effort, not real time, and not guaranteed to perfectly match the latest folder listing.  Results may be truncated if more than 1,000 possible matches exist.  This field should only be used for ad-hoc (human) searching, and not as part of an automated process.
        ///   search_custom_metadata_key - string - If provided, the search string in `search` will search for files where this custom metadata key matches the value sent in `search`.  Set this to `*` to allow any metadata key to match the value sent in `search`.
        ///   search_all - boolean - Search entire site?  If true, we will search the entire site.  Do not provide a path when using this parameter.  This is the same API used by the search bar in the web UI when running 'Search All Files'.  Search results are a best effort, not real time, and not guaranteed to match every file.  This field should only be used for ad-hoc (human) searching, and not as part of an automated process.
        ///   with_previews - boolean - Include file previews?
        ///   with_priority_color - boolean - Include file priority color information?
        ///   type - string - Type of objects to return.  Can be `folder` or `file`.
        ///   modified_at_datetime - string - If provided, will only return files/folders modified after this time. Can be used only in combination with `type` filter.
        /// </summary>
        public FilesList<RemoteFile> ListFor(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Folder.ListForCore(client, path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options));
        }

        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   provided_mtime - string - User provided modification time.
        /// </summary>
        public Task<RemoteFile> CreateAsync(
            string path,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null,
            CancellationToken cancellationToken = default
        )
        {
            return Folder.CreateCore(new OperationContext(client), path, DictionaryUtil.Copy(parameters), DictionaryUtil.Copy(options), cancellationToken);
        }
    }
}