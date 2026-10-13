using System;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A completed download.
    /// </summary>
    public sealed class DownloadResult
    {
        internal DownloadResult(DownloadProgress finalProgress, long parts, DownloadVersion version = null, bool completedEarlier = false, string localIdentity = null)
        {
            CompletedEarlier = completedEarlier;
            LocalIdentity = localIdentity;
            Path = finalProgress.Path;
            LocalPath = finalProgress.LocalPath;
            Size = finalProgress.TotalBytes;
            VersionBound = finalProgress.VersionBound;
            Parts = parts;
            Version = version;
            ResumedBytes = finalProgress.ResumedBytes;
            ReceivedBytes = finalProgress.ReceivedBytes;
            Elapsed = finalProgress.Elapsed;
            FinalProgress = finalProgress;
        }

        /// <summary>The remote path downloaded.</summary>
        public string Path { get; }

        /// <summary>The local file written, or null for a download into a stream.</summary>
        public string LocalPath { get; }

        /// <summary>The number of bytes in the file.</summary>
        public long Size { get; }

        /// <summary>
        /// Whether the download was bound to one version of the file, so every part it wrote is of that version, as its
        /// storage's entity tag shows; false when the file came in one request, which a download not bound to a version
        /// never splits or continues.
        /// </summary>
        public bool VersionBound { get; }

        /// <summary>The number of parts the file was fetched in; 1 for a file that came in one request, 0 for an empty one.</summary>
        public long Parts { get; }

        /// <summary>The bytes a continued download already had from its checkpoint.</summary>
        public long ResumedBytes { get; }

        /// <summary>The bytes received from the network, including parts fetched more than once.</summary>
        public long ReceivedBytes { get; }

        /// <summary>The time the download took.</summary>
        public TimeSpan Elapsed { get; }

        /// <summary>The download's progress once it was complete, as a progress handler would receive it.</summary>
        public DownloadProgress FinalProgress { get; }

        /// <summary>
        /// Whether an earlier run of this download, with the same checkpoint, had already put the file in place, and this
        /// run found it there, the very file that run wrote (by its identity on storage and its length), and finished
        /// what that run did not, without fetching it again. The file is as that run downloaded it; Files.com was not
        /// asked again whether it is still the current version. False for a file this run put in place.
        /// </summary>
        public bool CompletedEarlier { get; }

        // The version downloaded, or null for a download not bound to one.
        internal DownloadVersion Version { get; }

        // The identity on storage of the local file (FileIdentity), or null where the platform has none to give.
        internal string LocalIdentity { get; }
    }
}