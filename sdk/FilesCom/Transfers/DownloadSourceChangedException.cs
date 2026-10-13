using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// The version of the file a download was bound to can no longer be downloaded: the file changed while it was being
    /// downloaded, or since the download it resumes, or Files.com now gives its download URL for other storage.
    /// </summary>
    /// <remarks>
    /// A download that knows its version fetches every part of the file as that version, with the entity tag its
    /// storage first answered with, so bytes of two versions are never put together. When the version changes, the
    /// download stops instead: a file being downloaded is not published, and a stream holds only whole parts of the
    /// version that changed. Downloading again gets the file as it is now. <see cref="Exception.InnerException"/>,
    /// when not null, is the failure that showed the change.
    /// </remarks>
    public sealed class DownloadSourceChangedException : SdkException
    {
        internal DownloadSourceChangedException(string path, string reason, Exception failure = null)
            : base($"The file at {path} changed, or its version being downloaded can no longer be downloaded: {reason}. Download it again to get it as it is now.", failure)
        {
            Path = path;
        }

        /// <summary>The remote path of the download.</summary>
        public string Path { get; }
    }
}