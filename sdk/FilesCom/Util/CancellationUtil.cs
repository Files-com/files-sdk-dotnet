using System;
using System.Threading;

namespace FilesCom.Util
{
    internal static class CancellationUtil
    {
        // Cancels source, which stops the SDK's own work. The callbacks registered on its token run as it is cancelled,
        // and some can be an application's, registered on a token the SDK passed to its stream. One that throws does not
        // undo the cancellation, which has taken effect, and is not the operation's outcome: it is logged as a cleanup
        // failure, and whatever the work is being stopped for stays what the caller sees.
        internal static void Stop(CancellationTokenSource source, SdkLog log)
        {
            try
            {
                source.Cancel();
            }
            catch (AggregateException e)
            {
                DiagnosticText.WarnCleanupFailed(log, "A callback registered on the token of stopped work failed", e);
            }
        }
    }
}