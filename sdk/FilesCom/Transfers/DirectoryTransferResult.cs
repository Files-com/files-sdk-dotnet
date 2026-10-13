using System;
using System.Collections.Generic;

namespace FilesCom.Transfers
{
    /// <summary>
    /// A completed directory upload or download.
    /// </summary>
    public sealed class DirectoryTransferResult
    {
        internal DirectoryTransferResult(IReadOnlyList<DirectoryTransferEntry> files, IReadOnlyList<string> directories, IReadOnlyList<string> excluded, DirectoryTransferProgress finalProgress)
        {
            LocalDirectory = finalProgress.LocalDirectory;
            RemoteDirectory = finalProgress.RemoteDirectory;
            Files = files;
            Directories = directories;
            Excluded = excluded;
            Bytes = finalProgress.BytesTotal;
            Elapsed = finalProgress.Elapsed;
            FinalProgress = finalProgress;
        }

        /// <summary>The local directory, as the caller named it.</summary>
        public string LocalDirectory { get; }

        /// <summary>The remote folder.</summary>
        public string RemoteDirectory { get; }

        /// <summary>Every file transferred, or completed by an earlier run, in the order of their relative paths.</summary>
        public IReadOnlyList<DirectoryTransferEntry> Files { get; }

        /// <summary>
        /// The relative paths, with <c>/</c> between names, of the empty folders created at the destination: folders with
        /// nothing inside to transfer, which no file's transfer created.
        /// </summary>
        public IReadOnlyList<string> Directories { get; }

        /// <summary>
        /// The relative paths of local entries a directory upload did not transfer: symbolic links, and anything else
        /// that is neither a regular file nor a directory. Always empty for a download.
        /// </summary>
        public IReadOnlyList<string> Excluded { get; }

        /// <summary>The bytes of every file in <see cref="Files"/>.</summary>
        public long Bytes { get; }

        /// <summary>The time this run took.</summary>
        public TimeSpan Elapsed { get; }

        /// <summary>The transfer's progress once it was complete, as a progress handler would receive it.</summary>
        public DirectoryTransferProgress FinalProgress { get; }
    }
}