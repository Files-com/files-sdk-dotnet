using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // The stream RemoteFileSystem.OpenReadAsync returns: it reads what a download running behind it writes into a
    // TransferPipe. Disposing it before the end stops the download, and returns once the download has stopped.
    internal sealed class RemoteReadStream : Stream
    {
        private static readonly SdkLog log = SdkLog.For(typeof(RemoteReadStream));

        private readonly TransferPipe pipe;
        private readonly Task download;
        private readonly CancellationTokenSource stop;
        private int disposed;

        internal RemoteReadStream(TransferPipe pipe, Task download, CancellationTokenSource stop)
        {
            this.pipe = pipe;
            this.download = download;
            this.stop = stop;
        }

        public override bool CanRead => Volatile.Read(ref disposed) == 0;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException("A remote file opened to read is read forward only.");

        public override long Position
        {
            get => throw new NotSupportedException("A remote file opened to read is read forward only.");
            set => throw new NotSupportedException("A remote file opened to read is read forward only.");
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            CheckArguments(buffer, offset, count);
            if (Volatile.Read(ref disposed) != 0)
            {
                throw new ObjectDisposedException(nameof(RemoteReadStream));
            }
            return pipe.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException("A remote file opened to read is read forward only.");

        public override void SetLength(long value) => throw new NotSupportedException("A remote file opened to read cannot be changed.");

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException("A remote file opened to read cannot be written.");

        protected override void Dispose(bool disposing)
        {
            if (disposing && Interlocked.Exchange(ref disposed, 1) == 0)
            {
                Stop();
                // The download never throws: its failure went into the pipe.
                download.GetAwaiter().GetResult();
                stop.Dispose();
            }
            base.Dispose(disposing);
        }

#if NETCOREAPP3_0_OR_GREATER
        public override async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref disposed, 1) == 0)
            {
                Stop();
                await download.ConfigureAwait(false);
                stop.Dispose();
            }
            await base.DisposeAsync().ConfigureAwait(false);
        }
#endif

        private void Stop()
        {
            FilesCom.Util.CancellationUtil.Stop(stop, log);
            pipe.Leave();
        }

        internal static void CheckArguments(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0 || count < 0 || count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(offset < 0 ? nameof(offset) : nameof(count));
            }
        }
    }

    // The stream RemoteFileSystem.OpenWriteAsync returns: what is written goes, through a TransferPipe, to an upload of
    // unknown length running behind it. Disposing it ends the bytes and waits for the upload to complete, and throws
    // the upload's failure if it failed, unless it was cancelled by abandon, the caller's token; a write after the upload
    // failed throws that failure.
    internal sealed class RemoteWriteStream : Stream
    {
        private readonly TransferPipe pipe;
        private readonly Task upload;
        private readonly CancellationToken abandon;
        private int disposed;

        internal RemoteWriteStream(TransferPipe pipe, Task upload, CancellationToken abandon)
        {
            this.pipe = pipe;
            this.upload = upload;
            this.abandon = abandon;
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => Volatile.Read(ref disposed) == 0;

        public override long Length => throw new NotSupportedException("A remote file opened to write is written forward only.");

        public override long Position
        {
            get => throw new NotSupportedException("A remote file opened to write is written forward only.");
            set => throw new NotSupportedException("A remote file opened to write is written forward only.");
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            RemoteReadStream.CheckArguments(buffer, offset, count);
            if (Volatile.Read(ref disposed) != 0)
            {
                throw new ObjectDisposedException(nameof(RemoteWriteStream));
            }
            ThrowIfUploadEnded();
            try
            {
                await pipe.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            }
            catch (IOException)
            {
                // The upload stopped reading: its failure says why.
                ThrowIfUploadEnded();
                throw;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException("A remote file opened to write cannot be read.");

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException("A remote file opened to write is written forward only.");

        public override void SetLength(long value) => throw new NotSupportedException("A remote file opened to write is written forward only.");

        protected override void Dispose(bool disposing)
        {
            bool first = disposing && Interlocked.Exchange(ref disposed, 1) == 0;
            base.Dispose(disposing);
            if (first)
            {
                pipe.Complete();
                try
                {
                    upload.GetAwaiter().GetResult();
                }
                catch (OperationCanceledException) when (abandon.IsCancellationRequested)
                {
                    // Abandoned by the caller: not completed, as asked.
                }
            }
        }

#if NETCOREAPP3_0_OR_GREATER
        public override async ValueTask DisposeAsync()
        {
            bool first = Interlocked.Exchange(ref disposed, 1) == 0;
            await base.DisposeAsync().ConfigureAwait(false);
            if (first)
            {
                pipe.Complete();
                try
                {
                    await upload.ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (abandon.IsCancellationRequested)
                {
                    // Abandoned by the caller: not completed, as asked.
                }
            }
        }
#endif

        private void ThrowIfUploadEnded()
        {
            if (upload.IsCompleted)
            {
                // Its failure, as it was thrown; an upload never ends successfully before the bytes do.
                upload.GetAwaiter().GetResult();
                throw new InvalidOperationException("The upload has ended.");
            }
        }
    }

    // What a transfer reads from or writes into to reach a TransferPipe.
    internal sealed class PipeEndStream : Stream
    {
        private readonly TransferPipe pipe;
        private readonly bool reads;

        internal PipeEndStream(TransferPipe pipe, bool reads)
        {
            this.pipe = pipe;
            this.reads = reads;
        }

        public override bool CanRead => reads;

        public override bool CanSeek => false;

        public override bool CanWrite => !reads;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return reads ? pipe.ReadAsync(buffer, offset, count, cancellationToken) : throw new NotSupportedException();
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return reads ? throw new NotSupportedException() : pipe.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WriteAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();
    }
}