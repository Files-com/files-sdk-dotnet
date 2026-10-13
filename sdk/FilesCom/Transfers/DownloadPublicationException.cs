using System;
using System.IO;

namespace FilesCom.Transfers
{
    /// <summary>
    /// Why a downloaded file could not be put in place, or was put in place without certainty
    /// (<see cref="DownloadPublicationException.Problem"/>).
    /// </summary>
    public enum DownloadPublicationProblem
    {
        /// <summary>
        /// Something is at the file's name, which <see cref="DownloadPublicationPolicy.CreateNew"/> leaves as it is. Nothing
        /// was replaced.
        /// </summary>
        DestinationExists = 1,

        /// <summary>
        /// A folder, link or other file that is not a regular file is at the file's name, which is never replaced or
        /// followed. Nothing was replaced.
        /// </summary>
        DestinationNotRegularFile = 2,

        /// <summary>
        /// The file at that name cannot be replaced keeping who may use it as the policy says: its owner, group or access
        /// control list cannot be given to the new file, or cannot be read; or, on Windows, the new file cannot be given
        /// the access a new file there gets. Nothing was replaced; the file is as it was.
        /// </summary>
        AccessNotKept = 3,

        /// <summary>
        /// This platform cannot put the file in place as the policy says without a weaker way of doing it, which is not
        /// used, such as on Windows a directory whose folders cannot be held from the root of its drive or share, as on a
        /// volume with no drive letter. Nothing was downloaded or replaced.
        /// </summary>
        NotSupported = 4,

        /// <summary>
        /// The file is in place under its name, complete, but its folder's entry could not be flushed to storage, so it may
        /// not survive a system crash or power loss. It is not removed.
        /// </summary>
        PlacedNotDurable = 5,

        /// <summary>
        /// The file is in place under its name, complete, but recording that in the transfer's checkpoint failed. It is not
        /// removed; running the transfer again with the same checkpoint recognizes it as this transfer's file.
        /// </summary>
        PlacedNotRecorded = 6,

        /// <summary>
        /// The file being downloaded was replaced, where it waited in its private staging folder, by another program
        /// running as this user: on macOS and Linux that folder is private to this user, not to this process, and such a
        /// program can rename the file away and put another at its name. The downloaded file is not in place
        /// (<see cref="DownloadPublicationException.Placed"/> is false) and its bytes are not kept. Found before the file
        /// was to be put in place, nothing was put in place and the file at that name is as it was. Found just after, as
        /// that program did it at the very moment the file was being put in place, what it put at the stage's name was put
        /// there instead, taking the place of any file of that name, and is left as it is: the message says which. Nothing
        /// that program put anywhere is removed.
        /// </summary>
        StageReplaced = 7,

        /// <summary>
        /// The folder the file was to be put in is no longer at the path the download was given: it was renamed, or replaced
        /// by another, while the file was downloaded. On macOS and Linux a download writes into the folder it opened,
        /// wherever that folder is moved, so it puts the file in place only while that path still leads to that folder,
        /// as checked just before and again just after the file is put in place. Found before, nothing was put in place
        /// (<see cref="DownloadPublicationException.Placed"/> is false); with a checkpoint, the downloaded bytes are kept in
        /// the folder the download opened (<see cref="DownloadPublicationException.Retained"/>), where running the download
        /// again finds them only once that folder is at that path again, and otherwise fetches the file anew. Found just
        /// after, the file is in place in the folder the download opened, wherever that folder now is, not at the path
        /// (Placed is true), and is not removed. So too for a file an earlier run put in place, which a later run with the
        /// same checkpoint, or a folder download with the same checkpoint directory, recognizes instead of downloading it
        /// again: found in a folder the path no longer leads to, it is not reported as completed earlier; it is in place
        /// there (Placed is true), and it and its checkpoint are left as they are, for a run that finds that folder at the
        /// path again. A folder download whose folder moves while its remote folder is listed does nothing in it (Placed is
        /// false; its checkpoint directory is left as it is). On Windows the folders are held so that they cannot be renamed for
        /// <see cref="DownloadPublicationPolicy.CreateNew"/> and <see cref="DownloadPublicationPolicy.ReplaceRegularFile"/>,
        /// and the default puts the file in place by its path.
        /// </summary>
        DestinationMoved = 8,
    }

    /// <summary>
    /// A downloaded file was not put in place under its name as its <see cref="DownloadPublicationPolicy"/> says, or was
    /// put in place without certainty that it will last or was recorded (<see cref="Placed"/>).
    /// </summary>
    /// <remarks>
    /// <see cref="Problem"/> says why, <see cref="LocalPath"/> which file. A destination the policy refuses, or a policy
    /// the platform cannot apply, is refused before anything is fetched when that is known then; otherwise the refusal
    /// comes as the file is put in place, once every byte has been received. Before the file is placed, a download with
    /// a checkpoint keeps what it downloaded where it can (<see cref="Retained"/>): running it again then puts the same
    /// file in place, once its storage confirms that version is still the file's, without fetching it again; a download
    /// not bound to a version, or one without a checkpoint, keeps nothing. After it is placed, the file stays whatever
    /// follows. On macOS and Linux a successful download's file is the one it wrote, at the path it was given, as far as
    /// the checks made just after it was put in place, or as it was recognized as completed earlier, show
    /// (<see cref="DownloadPublicationProblem.StageReplaced"/>, <see cref="DownloadPublicationProblem.DestinationMoved"/>):
    /// checks of names at that moment, which another program running as the same user can change at any time.
    /// <see cref="Exception.InnerException"/>, when not null, is the system's own failure.
    /// </remarks>
    public sealed class DownloadPublicationException : IOException
    {
        internal DownloadPublicationException(DownloadPublicationProblem problem, string localPath, string message, Exception failure = null, bool placed = false)
            : base(message, failure)
        {
            Problem = problem;
            LocalPath = localPath;
            Placed = placed || problem == DownloadPublicationProblem.PlacedNotDurable || problem == DownloadPublicationProblem.PlacedNotRecorded;
        }

        /// <summary>Why the file was not put in place, or what is uncertain about it.</summary>
        public DownloadPublicationProblem Problem { get; }

        /// <summary>The local file the download was to put in place.</summary>
        public string LocalPath { get; }

        /// <summary>
        /// Whether the downloaded file is in place under its name: true for
        /// <see cref="DownloadPublicationProblem.PlacedNotDurable"/> and <see cref="DownloadPublicationProblem.PlacedNotRecorded"/>,
        /// when it is complete but uncertain, and for <see cref="DownloadPublicationProblem.DestinationMoved"/> when its
        /// folder moved just as it was put in place, or before an earlier run's file was recognized in it, so that it is in
        /// place in that folder but not at <see cref="LocalPath"/>; false when the downloaded file was not put in place.
        /// </summary>
        public bool Placed { get; }

        /// <summary>
        /// Whether the downloaded bytes and the checkpoint recording them were kept, so that running the download again
        /// with <see cref="CheckpointPath"/> can put the file in place without fetching it again. They are kept private to
        /// this user: on macOS and Linux owner-only, without an access control list; on Windows with an access control
        /// list of their own that allows only this user, SYSTEM and Administrators, shown to be so before they are kept,
        /// and again before a later run continues them. False when the download has no checkpoint, is not bound to a
        /// version, was placed, lost its staging file to another program (<see cref="DownloadPublicationProblem.StageReplaced"/>),
        /// or its bytes cannot be kept private, as on a Windows volume without access control lists, or keeping them
        /// failed. Where the SDK uses directories by path on Linux (a runtime that cannot call the C
        /// library, or a processor such as ppc64le or s390x) or on another system, it cannot make them private: they are
        /// kept with the permissions new files get there.
        /// </summary>
        public bool Retained { get; internal set; }

        /// <summary>The download's checkpoint, or null when it has none.</summary>
        public string CheckpointPath { get; internal set; }
    }
}