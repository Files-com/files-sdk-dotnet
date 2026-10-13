using FilesCom.Util;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Where an upload's bytes come from. The coordinator prepares parts one at a time, in order, and charges the
    // limiter BufferSizeFor(part) bytes for each prepared part before preparing it; for a source of unknown length,
    // part.Length is the most the part may hold.
    internal abstract class UploadSource : IDisposable
    {
        // The bytes to upload, or null while they are not known.
        internal abstract long? Length { get; }

        // Work the source does before anything is sent to Files.com, such as copying a stream to its spool file.
        // limiter is the caller's, if one was given; the upload's own is made only once Files.com has answered.
        internal virtual Task StartAsync(UploadProgressTracker progress, TransferLimiter limiter, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        // The memory a prepared part holds until it has been sent for the last time.
        internal abstract long BufferSizeFor(PartDescriptor part);

        // Whether a prepared part holds all its bytes in memory, which then bounds how large parts can be.
        internal virtual bool HoldsWholeParts => false;

        internal abstract Task<PreparedBody> PrepareAsync(PartDescriptor part, CancellationToken cancellationToken);

        public virtual void Dispose()
        {
        }
    }

    // A prepared part's body, with the bytes it holds and whether the source ended with it.
    internal sealed class PreparedBody
    {
        internal PreparedBody(PartBody body, long length, bool endOfSource)
        {
            Body = body;
            Length = length;
            EndOfSource = endOfSource;
        }

        internal PartBody Body { get; }

        internal long Length { get; }

        internal bool EndOfSource { get; }
    }

    // A caller's stream, read forward from its current position; it is never seeked, measured or disposed. It holds
    // length bytes or, when length is null, whatever it gives until a read returns nothing. Each part is kept until the
    // server accepts it, so a retry resends exactly the bytes read, though the stream cannot be read again: in memory, or
    // in a temporary file of its own, written as the part is read and read back a copy buffer at a time. Short reads are
    // accumulated. A part of a stream of unknown length is shorter than planned only where the stream ends. Nothing is
    // read beyond the part being read, whose buffer the limiter has already been charged for: a part that fills exactly
    // may be the last, which the next part's read, returning nothing, shows (PartPlanner.Freeze). Once cancelled, no
    // further read starts and a read that completes anyway is not used.
    internal sealed class StreamUploadSource : UploadSource
    {
        // The longest array the runtime allows.
        private const long MaxBufferLength = 0x7FFFFFC7;

        private readonly Stream stream;
        private readonly long? length;
        private readonly bool inMemory;
        private readonly string temporaryDirectory;

        internal StreamUploadSource(Stream stream, long? length, StreamPartStorage storage, string temporaryDirectory)
        {
            this.stream = stream;
            this.length = length;
            inMemory = storage == StreamPartStorage.Memory;
            this.temporaryDirectory = temporaryDirectory;
        }

        internal override long? Length => length;

        internal override bool HoldsWholeParts => inMemory;

        internal override long BufferSizeFor(PartDescriptor part)
        {
            return inMemory ? part.Length : Math.Min(FileUploadSource.CopyBufferSize, part.Length);
        }

        internal override async Task<PreparedBody> PrepareAsync(PartDescriptor part, CancellationToken cancellationToken)
        {
            PartBody body;
            long read;
            if (inMemory)
            {
                if (part.Length > MaxBufferLength)
                {
                    throw new UploadLimitException(UploadLimit.MemoryPart, $"Part {part.Number} is {part.Length} bytes, too large to hold in memory. Upload from a file instead, or keep parts in StreamPartStorage.TemporaryFiles.");
                }
                byte[] buffer = new byte[part.Length];
                read = await ReadAsync(buffer, 0, part, (chunk, offset, count) => Task.CompletedTask, cancellationToken);
                body = new BufferedPartBody(buffer, (int)read);
            }
            else
            {
                TemporaryPartFile file = TemporaryPartFile.Create(temporaryDirectory);
                try
                {
                    byte[] copy = new byte[BufferSizeFor(part)];
                    read = await ReadAsync(copy, copy.Length, part, (chunk, offset, count) => file.WriteAsync(chunk, offset, count, cancellationToken), cancellationToken);
                    await file.FlushAsync(cancellationToken);
                    body = file.BodyFor(new PartDescriptor(part.Number, 0, read, part.IsLast), copy);
                }
                catch
                {
                    file.Dispose();
                    throw;
                }
            }
            return new PreparedBody(body, read, length.HasValue ? part.IsLast : read < part.Length);
        }

        // Reads up to the part's length into buffer. With a window of 0, the bytes are read straight into buffer;
        // otherwise buffer is a copy buffer of that size, and write takes each chunk. A stream of known length that ends
        // early fails; one of unknown length ends the part.
        private async Task<long> ReadAsync(byte[] buffer, int window, PartDescriptor part, Func<byte[], int, int, Task> write, CancellationToken cancellationToken)
        {
            long wanted = part.Length;
            long read = 0;
            while (read < wanted)
            {
                int offset = window == 0 ? (int)read : 0;
                int room = (int)Math.Min(window == 0 ? wanted - read : window, wanted - read);
                cancellationToken.ThrowIfCancellationRequested();
                int count = await stream.ReadAsync(buffer, offset, room, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (count == 0)
                {
                    if (length.HasValue)
                    {
                        throw new EndOfStreamException($"The upload stream ended after {part.Offset + read} of the {length} bytes expected.");
                    }
                    break;
                }
                await write(buffer, offset, count);
                read += count;
            }
            return read;
        }
    }

    // A caller's stream of unknown length, copied, before anything is sent to Files.com, to a new temporary file of the
    // upload's own, which is then uploaded like a local file, with its length known. The copy reads a buffer at a time
    // and stops with an error once the stream proves longer than maxBytes: no more than maxBytes + 1 bytes are ever
    // asked for. The copy buffer is charged to the caller's limiter, if one was given, and waits for its turn there like
    // a part's buffer, so uploads sharing the limiter stay within its buffer space; it is released once the copy ends,
    // before any part is read. Without one, the buffer is the upload's alone and within its own limiter's default space.
    // The file is deleted when the upload ends, after every part has settled. The stream is borrowed: it is left open.
    internal sealed class SpooledStreamSource : UploadSource
    {
        // Progress is reported after each this many bytes copied.
        private const long ReportEvery = 16 * 1024 * 1024;

        private readonly Stream stream;
        private readonly long maxBytes;
        private readonly string temporaryDirectory;
        private TemporaryPartFile spool;
        private FileUploadSource file;

        internal SpooledStreamSource(Stream stream, long maxBytes, string temporaryDirectory)
        {
            this.stream = stream;
            this.maxBytes = maxBytes;
            this.temporaryDirectory = temporaryDirectory;
        }

        internal override long? Length => file?.Length;

        internal override async Task StartAsync(UploadProgressTracker progress, TransferLimiter limiter, CancellationToken cancellationToken)
        {
            spool = TemporaryPartFile.Create(temporaryDirectory);
            progress.Spooling();
            int size = limiter == null ? FileUploadSource.CopyBufferSize : (int)Math.Min(FileUploadSource.CopyBufferSize, limiter.MaxBufferedBytes);
            using (limiter == null ? null : await progress.TimeBufferWait(limiter.ReserveBufferAsync(size, cancellationToken)))
            {
                await CopyAsync(progress, new byte[size], cancellationToken);
            }
            await spool.FlushAsync(cancellationToken);
            file = spool.Reader();
            progress.Report();
        }

        private async Task CopyAsync(UploadProgressTracker progress, byte[] buffer, CancellationToken cancellationToken)
        {
            long copied = 0;
            long reported = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                long room = maxBytes - copied;
                int wanted = room >= buffer.Length ? buffer.Length : (int)room + 1;
                int count = await stream.ReadAsync(buffer, 0, wanted, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                if (count == 0)
                {
                    break;
                }
                if (count > room)
                {
                    throw new UploadLimitException(UploadLimit.SpoolLength, $"The stream is longer than the {maxBytes} bytes the upload allows copying to its spool file, so nothing was uploaded.");
                }
                await spool.WriteAsync(buffer, 0, count, cancellationToken);
                copied += count;
                progress.Spooled(count);
                if (copied - reported >= ReportEvery)
                {
                    reported = copied;
                    progress.Report();
                }
            }
        }

        internal override long BufferSizeFor(PartDescriptor part)
        {
            return file.BufferSizeFor(part);
        }

        internal override Task<PreparedBody> PrepareAsync(PartDescriptor part, CancellationToken cancellationToken)
        {
            return file.PrepareAsync(part, cancellationToken);
        }

        public override void Dispose()
        {
            spool?.Dispose();
        }
    }

    // A temporary file an operation creates for itself: an upload's spool file or one part's copy, or the copy of a
    // stream sent as a file parameter. It has a new random name in the chosen folder (the user's temporary folder by
    // default) and is opened for this operation alone. On a system other than Windows it is readable only by its owner
    // from the moment it exists, before a byte is written: .NET 8 and 10 create it with mode 0600; the netstandard2.0
    // build, whose FileStream cannot be given a mode, creates it with mkstemp instead (CreatePrivate), and makes none
    // where it cannot. On Windows it has the folder's permissions. Disposing it deletes it; a failure to delete it is
    // logged, as a failure of purpose, never thrown over the operation's own outcome, and its path is logged at DEBUG only.
    internal sealed class TemporaryPartFile : IDisposable
    {
        private static readonly SdkLog log = SdkLog.For(typeof(TemporaryPartFile));

        // For the SDK's own tests: makes Create use the netstandard2.0 build's creation (CreatePrivate) on .NET 8 and 10,
        // for the operations within the flow of a test that sets it, so that the route that build takes is tested by the
        // operations that take it. Nothing else sets it.
        internal static readonly AsyncLocal<bool> NetstandardCreationForTests = new AsyncLocal<bool>();

        // For the SDK's own tests: makes CreatePrivate act as where the C library cannot be called, for the operations
        // within the flow of a test that sets it. Nothing else sets it.
        internal static readonly AsyncLocal<bool> NoCLibraryForTests = new AsyncLocal<bool>();

        private readonly string path;
        private readonly FileStream handle;
        private readonly string purpose;
        private int disposed;

        private TemporaryPartFile(string path, FileStream handle, string purpose)
        {
            this.path = path;
            this.handle = handle;
            this.purpose = purpose;
        }

        // Reads the file back through its own handle, once it has been written.
        internal FileUploadSource Reader()
        {
            return FileUploadSource.Of(handle);
        }

        internal static TemporaryPartFile Create(string directory, string purpose = "Uploading a stream")
        {
            string folder = directory ?? System.IO.Path.GetTempPath();
#if NET7_0_OR_GREATER
            if (!OperatingSystem.IsWindows() && NetstandardCreationForTests.Value)
            {
                return CreatePrivate(folder, purpose);
            }
            string path = System.IO.Path.Combine(folder, "files-upload-" + Guid.NewGuid().ToString("N") + ".tmp");
            var options = new FileStreamOptions { Mode = FileMode.CreateNew, Access = FileAccess.ReadWrite, Share = FileShare.None, BufferSize = 1 };
            if (!OperatingSystem.IsWindows())
            {
                options.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
            }
            return new TemporaryPartFile(path, new FileStream(path, options), purpose);
#else
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CreatePrivate(folder, purpose);
            }
            string path = System.IO.Path.Combine(folder, "files-upload-" + Guid.NewGuid().ToString("N") + ".tmp");
            return new TemporaryPartFile(path, new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 1), purpose);
#endif
        }

        // A temporary file only its owner can use, for the netstandard2.0 build on a system other than Windows, whose
        // FileStream would create it with the permissions the process's umask leaves (with the common 022, readable by
        // everyone who can name it in a shared folder such as /tmp). mkstemp creates it instead: exclusively, under a name
        // it completes at random, and with mode 0600 whatever the umask, so no one else can open it at any moment. Before
        // a byte is written, the file it opened is shown to be a regular file of this user's, with one name and no
        // permission for anyone else, and any access control list it inherited is removed (Native.RemoveAccessControl),
        // as for checkpoints and download stages. Where the C library cannot be called (Native.Flags is null: .NET Core
        // 2.x on Linux, a Linux processor the SDK has no layouts for, or another system), no file can be made so, and none
        // is made: the operation fails before anything is copied, and the umask is never changed to make one.
        internal static TemporaryPartFile CreatePrivate(string folder, string purpose)
        {
            if (Native.Flags == null || NoCLibraryForTests.Value)
            {
                throw new IOException($"{purpose}: no temporary file can be made in {folder} that only this user can read, because this runtime cannot call the C library, so nothing was copied. Use .NET 8 or later, or the netstandard2.0 build on .NET Core 3.0 or later or on Mono, or give a stream that can seek.");
            }
            byte[] template = Encoding.UTF8.GetBytes(System.IO.Path.Combine(folder, "files-upload-" + Guid.NewGuid().ToString("N") + "-XXXXXX") + "\0");
            int fd = Native.MkStemp(template);
            if (fd < 0)
            {
                throw Native.Failure(Marshal.GetLastWin32Error(), $"{purpose}: a temporary file cannot be created in {folder}");
            }
            string path = Encoding.UTF8.GetString(template, 0, template.Length - 1);
            var file = new SafeFileHandle((IntPtr)fd, ownsHandle: true);
            try
            {
                string what = $"{purpose}: its temporary file";
                Native.FileStatus status = Native.Stat(file, what);
                if (!status.IsRegular || status.Links != 1 || status.Uid != Native.GetEuid() || (status.Mode & 0x3F) != 0)
                {
                    throw new IOException($"{what} is not a private file of this user's, so nothing was copied.");
                }
                Native.RemoveAccessControl(file, what);
                return new TemporaryPartFile(path, new FileStream(file, FileAccess.ReadWrite, 1), purpose);
            }
            catch
            {
                file.Dispose();
                try
                {
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    DiagnosticText.WarnCleanupFailed(log, purpose + ": deleting one of its temporary files failed", e);
                }
                throw;
            }
        }

        internal Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return handle.WriteAsync(buffer, offset, count, cancellationToken);
        }

        internal Task FlushAsync(CancellationToken cancellationToken)
        {
            return handle.FlushAsync(cancellationToken);
        }

        // The body of a part whose bytes are this whole file; disposing the body deletes the file.
        internal PartBody BodyFor(PartDescriptor bytes, byte[] copyBuffer)
        {
            return new TemporaryFilePartBody(this, new FilePartBody(Reader(), bytes, copyBuffer));
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
            {
                return;
            }
            handle.Dispose();
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                DiagnosticText.WarnCleanupFailed(log, purpose + ": deleting one of its temporary files failed", e);
            }
        }

        private sealed class TemporaryFilePartBody : PartBody
        {
            private readonly TemporaryPartFile file;
            private readonly PartBody body;

            internal TemporaryFilePartBody(TemporaryPartFile file, PartBody body)
            {
                this.file = file;
                this.body = body;
            }

            internal override PartContent CreateContent(Action<int> onSent, CancellationToken cancellationToken)
            {
                return body.CreateContent(onSent, cancellationToken);
            }

            internal override void Dispose()
            {
                file.Dispose();
            }
        }
    }

    // A local file, opened once. Every part, on every attempt, is read through that one handle, so what is sent is
    // always the opened file, even if its name comes to refer to another file meanwhile, as renaming or replacing it
    // can on Unix. Each part keeps its own offset; parts read one chunk at a time, in turn, while their sends overlap.
    // A prepared part holds only a copy buffer. On Windows the handle also keeps other programs from changing or
    // deleting the file; elsewhere, a program writing into the file itself changes what later reads return.
    internal sealed class FileUploadSource : UploadSource
    {
        internal const int CopyBufferSize = 128 * 1024;

        private readonly FileStream handle;
        private readonly SemaphoreSlim reading = new SemaphoreSlim(1, 1);

        private FileUploadSource(FileStream handle, bool isLocalFile)
        {
            this.handle = handle;
            Length = handle.Length;
            IsLocalFile = isLocalFile;
        }

        internal static FileUploadSource Open(string localPath)
        {
            return new FileUploadSource(new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read, 1), isLocalFile: true);
        }

        // A source reading through a handle the upload already owns, such as its spool file's.
        internal static FileUploadSource Of(FileStream handle)
        {
            return new FileUploadSource(handle, isLocalFile: false);
        }

        // A source reading a caller's file through a handle already opened to it, as a directory upload opens its files.
        internal static FileUploadSource OfLocal(FileStream handle)
        {
            return new FileUploadSource(handle, isLocalFile: true);
        }

        // Whether the file is the caller's, which a program of theirs can write while it is uploaded, rather than one
        // the upload owns, such as a spool file: the upload checks that the caller's file is as it was before it
        // completes (UploadCoordinator.CheckSourceUnchanged).
        internal bool IsLocalFile { get; }

        internal override long? Length { get; }

        // The opened file's modification time, whatever its name refers to now.
#if NET7_0_OR_GREATER
        internal DateTime LastWriteTimeUtc => File.GetLastWriteTimeUtc(handle.SafeFileHandle);
#else
        internal DateTime LastWriteTimeUtc => File.GetLastWriteTimeUtc(handle.Name);
#endif

        internal override long BufferSizeFor(PartDescriptor part)
        {
            return Math.Min(CopyBufferSize, part.Length);
        }

        internal override Task<PreparedBody> PrepareAsync(PartDescriptor part, CancellationToken cancellationToken)
        {
            return Task.FromResult(new PreparedBody(new FilePartBody(this, part, new byte[BufferSizeFor(part)]), part.Length, part.IsLast));
        }

        // The file's length now, which a program writing into it can change.
        internal long CurrentLength => handle.Length;

        // Reads up to count bytes at position into buffer, from the opened file.
        internal Task<int> ReadAsync(long position, byte[] buffer, int count, CancellationToken cancellationToken)
        {
            return ReadAsync(position, buffer, 0, count, cancellationToken);
        }

        // Reads up to count bytes at position into buffer at offset, from the opened file.
        internal async Task<int> ReadAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await reading.WaitAsync(cancellationToken);
            try
            {
                handle.Position = position;
                return await handle.ReadAsync(buffer, offset, count, cancellationToken);
            }
            finally
            {
                reading.Release();
            }
        }

        public override void Dispose()
        {
            handle.Dispose();
        }
    }

    // A prepared part's contents. Makes the request body for each attempt to send the part; every body has the same
    // bytes. onSent receives each chunk's size as it is written.
    internal abstract class PartBody
    {
        internal abstract PartContent CreateContent(Action<int> onSent, CancellationToken cancellationToken);

        // Releases what the body holds once the part will not be sent again, such as its temporary file.
        internal virtual void Dispose()
        {
        }
    }

    internal sealed class BufferedPartBody : PartBody
    {
        private readonly byte[] contents;
        private readonly int count;

        // The part is the first count bytes of contents.
        internal BufferedPartBody(byte[] contents, int count)
        {
            this.contents = contents;
            this.count = count;
        }

        internal override PartContent CreateContent(Action<int> onSent, CancellationToken cancellationToken)
        {
            return new BufferedPartContent(contents, count, onSent, cancellationToken);
        }
    }

    internal sealed class FilePartBody : PartBody
    {
        private readonly FileUploadSource file;
        private readonly PartDescriptor part;
        private readonly byte[] copyBuffer;

        internal FilePartBody(FileUploadSource file, PartDescriptor part, byte[] copyBuffer)
        {
            this.file = file;
            this.part = part;
            this.copyBuffer = copyBuffer;
        }

        internal override PartContent CreateContent(Action<int> onSent, CancellationToken cancellationToken)
        {
            return new FilePartContent(file, part, copyBuffer, onSent, cancellationToken);
        }
    }

    // The body of one attempt to send a part: exactly the part's length, written in chunks, each counted as sent once
    // it has been written. A failure to read the source is recorded in SourceFailure, because the HTTP stack reports
    // it as a network failure, which would otherwise be retried or mistaken for one.
    internal abstract class PartContent : HttpContent
    {
        private const int ChunkSize = 64 * 1024;

        private readonly long length;
        private readonly Action<int> onSent;
        private long bytesWritten;
        private IncrementalHash hash;
        private long serialized;

        protected PartContent(long length, Action<int> onSent, CancellationToken cancellationToken)
        {
            this.length = length;
            this.onSent = onSent;
            CancellationToken = cancellationToken;
            Headers.ContentLength = length;
        }

        internal Exception SourceFailure { get; private protected set; }

        // Whether to compute the SHA-256 of the bytes sent (Sha256), which a checkpoint records for the accepted part.
        internal bool HashBody { get; set; }

        // The SHA-256 of the body the last time it was written whole: what the server received, even if a redirect had it
        // written more than once. Null until then, or without HashBody.
        internal byte[] Sha256 { get; private set; }

        // The bytes this body has written to the network, including any resent to follow a redirect.
        internal long BytesWritten => Interlocked.Read(ref bytesWritten);

        protected CancellationToken CancellationToken { get; }

        protected override bool TryComputeLength(out long length)
        {
            length = this.length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                hash?.Dispose();
            }
            base.Dispose(disposing);
        }

        // Each writing of the body starts here: a redirect may have it written again.
        protected void StartBody()
        {
            serialized = 0;
            Sha256 = null;
            if (HashBody)
            {
                hash?.Dispose();
                hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            }
        }

        protected async Task WriteAsync(Stream network, byte[] buffer, int count)
        {
            for (int written = 0; written < count;)
            {
                int chunk = Math.Min(ChunkSize, count - written);
                await network.WriteAsync(buffer, written, chunk, CancellationToken);
                hash?.AppendData(buffer, written, chunk);
                serialized += chunk;
                Interlocked.Add(ref bytesWritten, chunk);
                onSent(chunk);
                written += chunk;
            }
            if (hash != null && serialized == length)
            {
                Sha256 = hash.GetHashAndReset();
            }
        }
    }

    internal sealed class BufferedPartContent : PartContent
    {
        private readonly byte[] contents;
        private readonly int count;

        internal BufferedPartContent(byte[] contents, int count, Action<int> onSent, CancellationToken cancellationToken) : base(count, onSent, cancellationToken)
        {
            this.contents = contents;
            this.count = count;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            StartBody();
            await WriteAsync(stream, contents, count);
        }
    }

    internal sealed class FilePartContent : PartContent
    {
        private readonly FileUploadSource file;
        private readonly PartDescriptor part;
        private readonly byte[] copyBuffer;

        internal FilePartContent(FileUploadSource file, PartDescriptor part, byte[] copyBuffer, Action<int> onSent, CancellationToken cancellationToken) : base(part.Length, onSent, cancellationToken)
        {
            this.file = file;
            this.part = part;
            this.copyBuffer = copyBuffer;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            StartBody();
            if (part.Length == 0)
            {
                await WriteAsync(stream, copyBuffer, 0);
                return;
            }
            for (long read = 0; read < part.Length;)
            {
                CancellationToken.ThrowIfCancellationRequested();
                int count = await ReadAsync(part.Offset + read, (int)Math.Min(copyBuffer.Length, part.Length - read));
                await WriteAsync(stream, copyBuffer, count);
                read += count;
            }
        }

        private async Task<int> ReadAsync(long position, int wanted)
        {
            int count;
            try
            {
                count = await file.ReadAsync(position, copyBuffer, wanted, CancellationToken);
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                SourceFailure = e;
                throw;
            }
            if (count == 0)
            {
                SourceFailure = new EndOfStreamException($"The file ended before part {part.Number}, which should end at byte {part.Offset + part.Length}, was read.");
                throw SourceFailure;
            }
            return count;
        }
    }
}