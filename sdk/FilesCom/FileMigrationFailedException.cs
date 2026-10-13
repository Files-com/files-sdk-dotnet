using FilesCom.Models;

namespace FilesCom
{
    /// <summary>
    /// A copy, move or other action Files.com finishes in the background failed or was cancelled, as
    /// <see cref="FileAction.WaitAsync"/> found. What it had done before then stays done.
    /// </summary>
    /// <remarks>
    /// The message says only which migration, which kind of operation and how it ended, so that it can be shown or
    /// logged as it is. The migration's own details, such as its paths and <see cref="FileMigration.FailureMessage"/>,
    /// which is Files.com's text and may quote addresses or credentials, are on <see cref="Migration"/>, and logged at the
    /// DEBUG level.
    /// </remarks>
    public sealed class FileMigrationFailedException : SdkException
    {
        // The operations a migration names, which the message may repeat; any other value is not quoted.
        private static readonly string[] Operations = { "delete", "move", "copy", "regional_migration", "organize", "zip", "unzip", "gpg_encrypt", "gpg_decrypt", "transform" };

        internal FileMigrationFailedException(FileAction action, FileMigration migration)
            : base(Describe(migration))
        {
            Action = action;
            Migration = migration;
        }

        /// <summary>The action waited for.</summary>
        public FileAction Action { get; }

        /// <summary>
        /// The migration as last checked, with its <see cref="FileMigration.Status"/>, <see cref="FileMigration.FailureMessage"/>
        /// and how many files it processed; null when the action itself said it failed, without a migration.
        /// </summary>
        public FileMigration Migration { get; }

        private static string Describe(FileMigration migration)
        {
            if (migration == null)
            {
                return "Files.com reported that the action failed.";
            }
            string operation = System.Array.IndexOf(Operations, migration.Operation) >= 0 ? migration.Operation.Replace('_', ' ') : "action";
            string how = migration.Status == "cancelled" ? "was cancelled" : "failed";
            string details = string.IsNullOrEmpty(migration.FailureMessage) ? "" : " Its failure message is in Migration.FailureMessage.";
            return $"File migration {migration.Id} ({operation}) {how}.{details}";
        }
    }
}