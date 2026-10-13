using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Http
{
    // The body of an upload request, written through a stream that times each network write with the attempt's
    // NetworkWatch. Preparing the next bytes, such as reading them from the upload's source, happens between writes and
    // is not timed. Writes are split into chunks, so a large body that keeps moving never looks stalled, and a write
    // that cannot complete within WriteTimeout ends the attempt. It sends inner's bytes and headers unchanged, owns
    // inner, and can be sent again, as a redirect requires.
    internal sealed class WatchedContent : HttpContent
    {
        private const int ChunkSize = 64 * 1024;

        private readonly HttpContent inner;
        private readonly NetworkWatch watch;

        internal WatchedContent(HttpContent inner, NetworkWatch watch)
        {
            this.inner = inner;
            this.watch = watch;
            foreach (var header in inner.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        internal HttpContent Inner => inner;

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            // Also when the body is sent again to follow a redirect, after its first response.
            watch.SendingBody();
            await inner.CopyToAsync(new WatchedStream(stream, watch), context);
            watch.BodySent();
        }

        protected override bool TryComputeLength(out long length)
        {
            long? known = inner.Headers.ContentLength;
            length = known ?? 0;
            return known.HasValue;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
            }
            base.Dispose(disposing);
        }

        private sealed class WatchedStream : Stream
        {
            private readonly Stream network;
            private readonly NetworkWatch watch;

            internal WatchedStream(Stream network, NetworkWatch watch)
            {
                this.network = network;
                this.watch = watch;
            }

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                for (int written = 0; written < count;)
                {
                    int chunk = Math.Min(ChunkSize, count - written);
                    watch.WriteStarted();
                    try
                    {
                        await network.WriteAsync(buffer, offset + written, chunk, cancellationToken);
                    }
                    finally
                    {
                        watch.WriteEnded();
                    }
                    written += chunk;
                }
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                for (int written = 0; written < count;)
                {
                    int chunk = Math.Min(ChunkSize, count - written);
                    watch.WriteStarted();
                    try
                    {
                        network.Write(buffer, offset + written, chunk);
                    }
                    finally
                    {
                        watch.WriteEnded();
                    }
                    written += chunk;
                }
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return network.FlushAsync(cancellationToken);
            }

            public override void Flush()
            {
                network.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();
        }
    }
}