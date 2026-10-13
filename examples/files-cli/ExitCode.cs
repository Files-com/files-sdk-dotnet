namespace FilesCli
{
    internal static class ExitCode
    {
        public const int Success = 0;

        // The arguments or the sign-in configuration are unusable. Nothing was sent to Files.com.
        public const int Usage = 1;

        // Files.com reported an error, or a local file could not be read or written.
        public const int Failure = 2;

        // Stopped by Ctrl+C. Requests Files.com had already received are not undone.
        public const int Cancelled = 130;
    }
}