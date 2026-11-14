using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A source-change precondition failed, a response has a newer modification date, the storage server answered that
    /// the file changed since its download URL was issued (HTTP 409 <c>download_source_changed</c>), or the version a
    /// download was bound to can no longer be downloaded: the file changed while it was being downloaded, or since the
    /// download it resumes, or Files.com now gives its download URL for other storage.
    /// </summary>
    /// <remarks>
    /// A download that knows its version fetches every part of the file as that version, with the entity tag its
    /// storage first answered with, so bytes of two versions are never put together. When the file changes, the
    /// attempt stops, and what it fetched is discarded. A download of <see cref="TransferOperations"/> into a local
    /// file, or into a stream no write has yet begun to, then starts again from the file's first byte, with a new
    /// download URL and size from Files.com, as long as the client's
    /// <see cref="FilesConfiguration.MaxNetworkRetries"/> allow: each start over uses one, however much had been
    /// fetched. This exception is thrown when none is left, at once with none, once a write to a stream has begun,
    /// even one that failed or was cancelled, and by other downloads, which do not start over: a file being downloaded
    /// is not published, and a stream holds only whole parts of the version that changed, and at most some of the part
    /// whose write was under way. Downloading again gets the file as it is now. <see cref="Exception.InnerException"/>,
    /// when not null, is the failure that showed the change. Modification dates provide only best-effort detection at
    /// one-second precision; they do not establish a strong version.
    /// </remarks>
    public sealed class DownloadSourceChangedException : SdkException
    {
        internal DownloadSourceChangedException(string path, string reason, Exception failure = null)
            : base($"{(path == null ? "The file" : "The file at " + path)} changed, or its version being downloaded can no longer be downloaded: {reason}. Download it again to get it as it is now.", failure)
        {
            Path = path;
        }

        /// <summary>The remote path of the download, or null for a download given only a URL.</summary>
        public string Path { get; }
    }
}