using FilesCom.Transfers;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom
{
    /// <summary>
    /// A file sent as the value of a file parameter of a Files.com operation, such as a user's <c>avatar_file</c>, a
    /// behavior's <c>attachment_file</c> or a site's <c>logo_file</c>: from a local file, from bytes in memory, or from
    /// a stream of yours.
    /// </summary>
    /// <example>
    /// <code>
    /// await client.Users.UpdateAsync(userId, new Dictionary&lt;string, object&gt;
    /// {
    ///     { "avatar_file", FileAttachment.FromFile("avatar.png") },
    ///     { "notes", "New picture" },
    /// });
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// The file goes in the same request as the operation's other parameters, with each of their values exactly as
    /// it would go without a file: null, false, 0, empty text, lists and objects included. The request is
    /// <c>multipart/form-data</c> when a form can carry every value and the file name unchanged. Otherwise it is JSON
    /// in which the file is the API's file object, with its bytes as Base64 in <c>encoded_content</c>, which makes the
    /// request about a third larger than the file. Either way the file is read in chunks as the request is sent, never
    /// held whole in memory for it, and the request states its exact length.
    /// </para>
    /// <para>
    /// A request that is sent again, after a failure that may be temporary, a redirect that repeats it, or a move to
    /// the site's region, sends the same bytes. A failure to read the file or stream is thrown as it is, and the
    /// request is not sent again.
    /// </para>
    /// <para>
    /// A file parameter also takes the API's own file object, a dictionary with <c>encoded_content</c> (the file's
    /// bytes as Base64), <c>filename</c> and <c>type</c>, which is sent as it is.
    /// </para>
    /// </remarks>
    [JsonConverter(typeof(NotSerialized))]
    public sealed class FileAttachment
    {
        /// <summary>
        /// The most bytes <see cref="FromStream"/> reads from a stream that cannot seek, unless you choose otherwise:
        /// 64 MiB.
        /// </summary>
        public const long DefaultMaxStreamBytes = 64L * 1024 * 1024;

        private readonly string path;
        private readonly byte[] contents;
        private readonly Stream stream;
        private readonly long maxStreamBytes;

        private FileAttachment(string fileName, string contentType, string path, byte[] contents, Stream stream, long maxStreamBytes)
        {
            FileName = fileName;
            ContentType = contentType;
            this.path = path;
            this.contents = contents;
            this.stream = stream;
            this.maxStreamBytes = maxStreamBytes;
        }

        /// <summary>The name the file is sent with, exactly as given.</summary>
        public string FileName { get; }

        /// <summary>The media type the file is sent with, or null to send it as <c>application/octet-stream</c>.</summary>
        public string ContentType { get; }

        /// <summary>
        /// A local file, opened when the operation starts and read through that one handle each time its request is
        /// sent. The SDK closes it when the operation ends. A file whose length changes meanwhile fails the operation
        /// with an <see cref="IOException"/> before its request is sent again.
        /// </summary>
        /// <param name="path">The file to send.</param>
        /// <param name="fileName">The name to send it with. If null, the name of the file in <paramref name="path"/>.</param>
        /// <param name="contentType">Its media type, such as <c>image/png</c>. If null, <c>application/octet-stream</c>.</param>
        /// <exception cref="MissingParameterException"><paramref name="path"/> is null.</exception>
        /// <exception cref="InvalidParameterException">
        /// <paramref name="path"/> names no file, the file name is empty or holds a control character, or
        /// <paramref name="contentType"/> is not a media type.
        /// </exception>
        public static FileAttachment FromFile(string path, string fileName = null, string contentType = null)
        {
            if (path == null)
            {
                throw MissingParameterException.For(nameof(path));
            }
            if (path.Length == 0)
            {
                throw InvalidParameterException.For(nameof(path), "The path is empty");
            }
            string name = fileName ?? Path.GetFileName(path);
            return new FileAttachment(CheckedFileName(name, fileName == null ? nameof(path) : nameof(fileName)), CheckedContentType(contentType), path, null, null, 0);
        }

        /// <summary>
        /// Bytes in memory. The array is not copied: leave it unchanged until the operation ends, so that a request
        /// sent again sends the same bytes.
        /// </summary>
        /// <param name="contents">The bytes of the file.</param>
        /// <param name="fileName">The name to send the file with, such as <c>logo.png</c>.</param>
        /// <param name="contentType">Its media type, such as <c>image/png</c>. If null, <c>application/octet-stream</c>.</param>
        /// <exception cref="MissingParameterException"><paramref name="contents"/> or <paramref name="fileName"/> is null.</exception>
        /// <exception cref="InvalidParameterException">
        /// <paramref name="fileName"/> is empty or holds a control character, or <paramref name="contentType"/> is not a
        /// media type.
        /// </exception>
        public static FileAttachment FromBytes(byte[] contents, string fileName, string contentType = null)
        {
            if (contents == null)
            {
                throw MissingParameterException.For(nameof(contents));
            }
            if (fileName == null)
            {
                throw MissingParameterException.For(nameof(fileName));
            }
            return new FileAttachment(CheckedFileName(fileName, nameof(fileName)), CheckedContentType(contentType), null, contents, null, 0);
        }

        /// <summary>
        /// The rest of a stream of yours, from its position when the operation starts. The stream is borrowed: the SDK
        /// never disposes it.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="fileName">The name to send the file with, such as <c>logo.png</c>.</param>
        /// <param name="contentType">Its media type, such as <c>image/png</c>. If null, <c>application/octet-stream</c>.</param>
        /// <param name="maxBytes">
        /// For a stream that cannot seek, the most bytes it may hold, at least 1. Such a stream is copied, before the
        /// request is sent, to a temporary file of the operation's own, which is deleted when the operation ends, so
        /// that its length is known and a request sent again has the same bytes; a longer stream fails the operation
        /// with <see cref="InvalidParameterException"/> before anything is sent. On macOS and Linux the file is readable
        /// only by its owner from the moment it exists; where the netstandard2.0 build cannot make it so (a runtime that
        /// cannot call the C library), the operation fails with <see cref="System.IO.IOException"/> before the stream is
        /// read. A stream that can seek is read in place,
        /// from its starting position each time the request is sent, and must keep its length meanwhile.
        /// </param>
        /// <exception cref="MissingParameterException"><paramref name="stream"/> or <paramref name="fileName"/> is null.</exception>
        /// <exception cref="InvalidParameterException">
        /// <paramref name="stream"/> cannot be read, <paramref name="fileName"/> is empty or holds a control character,
        /// <paramref name="contentType"/> is not a media type, or <paramref name="maxBytes"/> is less than 1.
        /// </exception>
        public static FileAttachment FromStream(Stream stream, string fileName, string contentType = null, long maxBytes = DefaultMaxStreamBytes)
        {
            if (stream == null)
            {
                throw MissingParameterException.For(nameof(stream));
            }
            if (!stream.CanRead)
            {
                throw InvalidParameterException.For(nameof(stream), "The stream cannot be read");
            }
            if (fileName == null)
            {
                throw MissingParameterException.For(nameof(fileName));
            }
            if (maxBytes < 1)
            {
                throw InvalidParameterException.OutOfRange(nameof(maxBytes), maxBytes, "Must be at least 1");
            }
            return new FileAttachment(CheckedFileName(fileName, nameof(fileName)), CheckedContentType(contentType), null, null, stream, maxBytes);
        }

        // The file's bytes for one operation: opened, measured, or copied as its kind needs, before anything is sent.
        // parameter names the file parameter in messages.
        internal async Task<AttachmentSource> OpenAsync(string parameter, CancellationToken cancellationToken)
        {
            if (path != null)
            {
                return new FileAttachmentSource(FileUploadSource.Open(path), parameter);
            }
            if (contents != null)
            {
                return new BytesAttachmentSource(contents, contents.Length);
            }
            if (stream.CanSeek)
            {
                return StreamAttachmentSource.Of(stream, parameter);
            }
            return await SpooledAttachmentSource.CopyAsync(stream, maxStreamBytes, parameter, cancellationToken);
        }

        private static string CheckedFileName(string name, string argument)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw InvalidParameterException.For(argument, "The file name is empty");
            }
            foreach (char c in name)
            {
                if (char.IsControl(c))
                {
                    throw InvalidParameterException.For(argument, "The file name holds a control character, which cannot be sent");
                }
            }
            return name;
        }

        private static string CheckedContentType(string contentType)
        {
            if (contentType != null && !MediaTypeHeaderValue.TryParse(contentType, out _))
            {
                throw InvalidParameterException.For(nameof(contentType), $"\"{contentType}\" is not a media type, such as image/png");
            }
            return contentType;
        }

        // A file goes only as the value of a file parameter, which the SDK sends itself; anywhere else, such as inside
        // another parameter's object, it would be JSON that says nothing of its bytes.
        private sealed class NotSerialized : JsonConverter<FileAttachment>
        {
            // A response never holds a file's bytes: a model property for a file parameter reads as null.
            public override FileAttachment Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Skip();
                return null;
            }

            public override void Write(Utf8JsonWriter writer, FileAttachment value, JsonSerializerOptions options)
            {
                throw new InvalidParameterException("Bad parameter: a FileAttachment can only be the value of a file parameter, such as avatar_file.");
            }
        }
    }

    // The bytes of one file parameter, for one operation: read at any position, as each attempt of its request
    // needs, by one attempt at a time. Disposing it releases what the operation opened or made for it.
    internal abstract class AttachmentSource : IDisposable
    {
        protected AttachmentSource(long length)
        {
            Length = length;
        }

        internal long Length { get; }

        // Checked as each attempt begins, before any byte of it is sent: a source that has changed length since the
        // operation started would send other bytes than the first attempt did.
        internal virtual void CheckUnchanged()
        {
        }

        // Reads up to count bytes at position into buffer at offset, or fewer, even none, at the end.
        internal abstract Task<int> ReadAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken);

        public virtual void Dispose()
        {
        }
    }

    internal sealed class BytesAttachmentSource : AttachmentSource
    {
        private readonly byte[] contents;

        internal BytesAttachmentSource(byte[] contents, int count) : base(count)
        {
            this.contents = contents;
        }

        internal override Task<int> ReadAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int length = (int)Math.Min(count, Length - position);
            Buffer.BlockCopy(contents, (int)position, buffer, offset, length);
            return Task.FromResult(length);
        }
    }

    // A local file the operation opened, through one handle for all its attempts.
    internal sealed class FileAttachmentSource : AttachmentSource
    {
        private readonly FileUploadSource file;
        private readonly string parameter;

        internal FileAttachmentSource(FileUploadSource file, string parameter) : base(file.Length.Value)
        {
            this.file = file;
            this.parameter = parameter;
        }

        internal override void CheckUnchanged()
        {
            if (file.CurrentLength != Length)
            {
                throw new IOException($"The file for {parameter} changed length after the operation started, so its request was not sent again.");
            }
        }

        internal override Task<int> ReadAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return file.ReadAsync(position, buffer, offset, count, cancellationToken);
        }

        public override void Dispose()
        {
            file.Dispose();
        }
    }

    // A caller's seekable stream, read in place from the position it had when the operation started. It is borrowed:
    // left open, and at a position of the SDK's choosing once the operation ends.
    internal sealed class StreamAttachmentSource : AttachmentSource
    {
        private readonly Stream stream;
        private readonly long start;
        private readonly string parameter;

        private StreamAttachmentSource(Stream stream, long start, long length, string parameter) : base(length)
        {
            this.stream = stream;
            this.start = start;
            this.parameter = parameter;
        }

        internal static StreamAttachmentSource Of(Stream stream, string parameter)
        {
            long start = stream.Position;
            long length = stream.Length - start;
            if (length < 0)
            {
                throw new InvalidParameterException($"Bad parameter: the stream for {parameter} is positioned past its end.");
            }
            return new StreamAttachmentSource(stream, start, length, parameter);
        }

        internal override void CheckUnchanged()
        {
            if (stream.Length - start != Length)
            {
                throw new IOException($"The stream for {parameter} changed length after the operation started, so its request was not sent again.");
            }
        }

        internal override async Task<int> ReadAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            stream.Position = start + position;
            return await stream.ReadAsync(buffer, offset, (int)Math.Min(count, Length - position), cancellationToken);
        }
    }

    // A caller's stream that cannot seek, copied to a temporary file of the operation's own, which disposing deletes.
    // The copy reads the stream from its position, up to maxBytes, and one byte more to find out whether it is longer.
    internal sealed class SpooledAttachmentSource : AttachmentSource
    {
        private const int CopyBufferSize = 64 * 1024;

        private readonly TemporaryPartFile spool;
        private readonly FileUploadSource file;

        private SpooledAttachmentSource(TemporaryPartFile spool, FileUploadSource file) : base(file.Length.Value)
        {
            this.spool = spool;
            this.file = file;
        }

        internal static async Task<SpooledAttachmentSource> CopyAsync(Stream stream, long maxBytes, string parameter, CancellationToken cancellationToken)
        {
            TemporaryPartFile spool = TemporaryPartFile.Create(null, "Sending a stream as " + parameter);
            try
            {
                byte[] buffer = new byte[CopyBufferSize];
                long copied = 0;
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
                        throw new InvalidParameterException($"Bad parameter: the stream for {parameter} holds more than the {maxBytes} bytes FileAttachment.FromStream allows it, so nothing was sent. Pass a larger maxBytes, or a stream that can seek.");
                    }
                    await spool.WriteAsync(buffer, 0, count, cancellationToken);
                    copied += count;
                }
                await spool.FlushAsync(cancellationToken);
                return new SpooledAttachmentSource(spool, spool.Reader());
            }
            catch
            {
                spool.Dispose();
                throw;
            }
        }

        internal override Task<int> ReadAsync(long position, byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return file.ReadAsync(position, buffer, offset, count, cancellationToken);
        }

        public override void Dispose()
        {
            spool.Dispose();
        }
    }
}