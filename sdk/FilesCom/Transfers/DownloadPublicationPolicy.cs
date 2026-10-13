namespace FilesCom.Transfers
{
    /// <summary>
    /// How a download to a local file puts the finished file in place, under its name
    /// (<see cref="DownloadOptions.PublicationPolicy"/>). The file is first written whole to a stage beside the
    /// destination, so an interrupted download does not leave a partial destination. The SDK does not switch publication
    /// policies to bypass a refusal. Policy-specific refusals use <see cref="DownloadPublicationException"/>; ordinary
    /// filesystem I/O exceptions may also propagate.
    /// </summary>
    public enum DownloadPublicationPolicy
    {
        /// <summary>
        /// The default: the file is created, or replaces a file of that name. On macOS and Linux a replaced file keeps
        /// exactly who may use it: its owner, group, permissions and access control list go to the new file, and a file
        /// whose owner or group this user cannot give the new file (another user's, or one of a group this user is not in)
        /// is not replaced. On Windows the file is replaced as Windows replaces a file by name: with the net8.0 and
        /// net10.0 builds the new file has the access a new file in the directory gets; with the netstandard2.0 build,
        /// Windows' ReplaceFile gives it the replaced file's access control list and attributes. The directory is used by
        /// path, as it always has been.
        /// </summary>
        ReplaceExisting = 0,

        /// <summary>
        /// The file is created only if nothing is at its name at the moment it is put in place: an existing file, folder or
        /// link, even a link to nothing, is left as it is and the download fails
        /// (<see cref="DownloadPublicationProblem.DestinationExists"/>). The check and the placing are one step where the
        /// file system allows it; nothing is replaced where it does not. On Windows the destination directory, and every
        /// directory above it, is held from the moment the download starts, so that none of them can be renamed or
        /// replaced meanwhile, and the new file gets the access a new file there gets.
        /// </summary>
        CreateNew = 1,

        /// <summary>
        /// The file is created, or replaces a regular file of that name, as PowerShell's <c>-Force</c> does: a folder, a
        /// link or another special file there is never replaced or followed
        /// (<see cref="DownloadPublicationProblem.DestinationNotRegularFile"/>). On macOS and Linux a replaced file's
        /// group, access control list and permissions go to the new file, which belongs to this user: unlike
        /// <see cref="ReplaceExisting"/>, a writable file of another user's is replaced by one of this user's, and its
        /// owner's permissions become this user's, while its group's and others' stay as they were. On Windows the
        /// replaced file's access control list, and its hidden attribute, go to the new file, whose owner is the one this
        /// user's account gives new files; a new file gets the access a new file there gets; and the directory is held as
        /// for <see cref="CreateNew"/>. Checking that the file is a regular one and replacing it are two steps: another
        /// program changing the folder at that moment can put something else at the name in between.
        /// </summary>
        ReplaceRegularFile = 2,
    }
}