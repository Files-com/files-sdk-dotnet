using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Http
{
    // Reads the body of a response that was received with HttpCompletionOption.ResponseHeadersRead, timing each read
    // with the attempt's NetworkWatch.
    //
    // A read that is already waiting is not stopped by its token on .NET Framework, so when the watch's token is
    // cancelled the response is disposed as well, which ends the read there. The response belongs to the SDK and to
    // this one request, so that stops nothing else: not the connection pool, and not the caller's stream. The read's
    // IOException is then reported as the cancellation or timeout that caused it; without that cause, a failed read
    // is a connection failure, HttpRequestException, as when HttpClient buffers a body itself.
    //
    // On .NET Framework, a read of a body that is still arriving does not return bytes that came in with the response
    // headers until more arrive; they then come together. A response that stops right after its headers and first
    // bytes therefore times out having delivered nothing; a download is then retried, which is safe because it has
    // written nothing.
    internal static class ResponseBody
    {
        private const int BufferSize = 81920;

        // The rest of the body, in memory.
        internal static async Task<byte[]> ReadAllAsync(HttpResponseMessage response, NetworkWatch watch)
        {
            using (var body = new MemoryStream())
            {
                await CopyToAsync(response, watch, (buffer, count) =>
                {
                    body.Write(buffer, 0, count);
                    return Task.CompletedTask;
                });
                return body.ToArray();
            }
        }

        // At most limit bytes of the rest of the body, in memory, and whether the body held more. Once limit bytes have
        // come, one more is read to find out whether the body goes on, and then nothing more: the rest is never read, and
        // goes with the response when it is disposed. So what is kept, what is read and how long reading takes grow with
        // limit, never with the body, however long it is or however long it keeps arriving. Each read is timed as
        // CopyToAsync times it, and fails as it does.
        internal static async Task<(byte[] Body, bool Truncated)> ReadPrefixAsync(HttpResponseMessage response, NetworkWatch watch, int limit)
        {
            var abort = new Abort(response);
            Stream body = await response.Content.ReadAsStreamAsync();
            byte[] buffer = new byte[Math.Min(BufferSize, limit + 1)];
            bool truncated = false;
            using (var kept = new MemoryStream())
            {
                using (watch.Token.Register(abort.Run))
                {
                    while (true)
                    {
                        int room = limit - (int)kept.Length;
                        int count = await ReadAsync(body, watch, abort, buffer, 0, Math.Min(buffer.Length, room + 1));
                        if (count == 0)
                        {
                            break;
                        }
                        if (count > room)
                        {
                            kept.Write(buffer, 0, room);
                            truncated = true;
                            break;
                        }
                        kept.Write(buffer, 0, count);
                    }
                }
                if (abort.Requested)
                {
                    throw watch.Stopped();
                }
                return (kept.ToArray(), truncated);
            }
        }

        // Passes the rest of the body to write, a chunk at a time. The time write takes is not timed, as it is not the
        // network's, and its failures are thrown as they are.
        internal static Task CopyToAsync(HttpResponseMessage response, NetworkWatch watch, Func<byte[], int, Task> write)
        {
            return CopyToAsync(response, watch, new byte[BufferSize], write);
        }

        // The same, read through buffer, which the caller provides: a transfer allocates what its limiter charged it.
        internal static async Task CopyToAsync(HttpResponseMessage response, NetworkWatch watch, byte[] buffer, Func<byte[], int, Task> write)
        {
            var abort = new Abort(response);
            Stream body = await response.Content.ReadAsStreamAsync();
            using (watch.Token.Register(abort.Run))
            {
                while (true)
                {
                    int count = await ReadAsync(body, watch, abort, buffer, 0, buffer.Length);
                    if (count == 0)
                    {
                        break;
                    }
                    await write(buffer, count);
                }
            }
            // The token was cancelled as the last read completed: the response may already be disposed.
            if (abort.Requested)
            {
                throw watch.Stopped();
            }
        }

        // Passes the rest of the body to write through buffer a buffer at a time: each write has as many bytes as fill
        // buffer, or the last of the body, not what one read happened to return, which over TLS is at most one record of
        // 16 KiB, so a destination takes few large writes. received is called with each read's size as it arrives, before
        // that read's bytes are written; a failure leaves what the buffer holds unwritten. Reads are timed, and fail, as
        // CopyToAsync's do; the time write takes is not timed.
        internal static async Task CopyFilledToAsync(HttpResponseMessage response, NetworkWatch watch, byte[] buffer, Action<int> received, Func<byte[], int, Task> write)
        {
            var abort = new Abort(response);
            Stream body = await response.Content.ReadAsStreamAsync();
            using (watch.Token.Register(abort.Run))
            {
                bool ended = false;
                while (!ended)
                {
                    int filled = 0;
                    while (filled < buffer.Length)
                    {
                        int count = await ReadAsync(body, watch, abort, buffer, filled, buffer.Length - filled);
                        if (count == 0)
                        {
                            ended = true;
                            break;
                        }
                        received(count);
                        filled += count;
                    }
                    if (filled > 0)
                    {
                        await write(buffer, filled);
                    }
                }
            }
            if (abort.Requested)
            {
                throw watch.Stopped();
            }
        }

        // Reads the rest of the body into target itself, from its first byte, until the body ends or target is full;
        // then reads once more, into target's first byte, whose value is put back after, to find where the body ends, so
        // that nothing but target is allocated. Returns the bytes read into target, and whether the body held more than
        // target can: the byte beyond is not kept. onRead receives each read's size.
        internal static async Task<(long Read, bool Overrun)> ReadIntoAsync(HttpResponseMessage response, NetworkWatch watch, byte[] target, Action<int> onRead)
        {
            var abort = new Abort(response);
            Stream body = await response.Content.ReadAsStreamAsync();
            long read = 0;
            bool overrun = false;
            using (watch.Token.Register(abort.Run))
            {
                while (true)
                {
                    if (read == target.Length)
                    {
                        byte kept = target[0];
                        try
                        {
                            overrun = await ReadAsync(body, watch, abort, target, 0, 1) > 0;
                        }
                        finally
                        {
                            target[0] = kept;
                        }
                        break;
                    }
                    int count = await ReadAsync(body, watch, abort, target, (int)read, (int)Math.Min(BufferSize, target.Length - read));
                    if (count == 0)
                    {
                        break;
                    }
                    read += count;
                    onRead(count);
                }
            }
            if (abort.Requested)
            {
                throw watch.Stopped();
            }
            return (read, overrun);
        }

        // One read of the body, timed, with its failures reported as the timeout, stop or connection failure they are.
        private static async Task<int> ReadAsync(Stream body, NetworkWatch watch, Abort abort, byte[] buffer, int offset, int count)
        {
            int read;
            watch.Begin(NetworkWait.ResponseBody);
            try
            {
                read = await body.ReadAsync(buffer, offset, count, watch.Token);
            }
            catch (OperationCanceledException e) when (watch.TimedOut(e))
            {
                throw watch.Timeout();
            }
            catch (Exception e) when (abort.Requested && (e is IOException || e is ObjectDisposedException))
            {
                throw watch.Stopped();
            }
            catch (IOException e)
            {
                throw new HttpRequestException("The connection failed while the response was being received.", e);
            }
            watch.Pause();
            return read;
        }

        // Disposes the response once the watch's token is cancelled. On .NET Framework disposing blocks while it tries
        // to drain the rest of the body, about half a second when the server has stopped, so it runs on a thread of
        // its own: neither the thread that cancelled the token nor a thread pool thread the application needs waits.
        private sealed class Abort
        {
            private readonly HttpResponseMessage response;
            private volatile bool requested;

            internal Abort(HttpResponseMessage response)
            {
                this.response = response;
            }

            internal bool Requested => requested;

            internal void Run()
            {
                requested = true;
                Task.Factory.StartNew(response.Dispose, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }
    }
}