using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // One part of a download: bytes [Start, End) of the file, the Index-th of its parts. Set once when the download is
    // planned, and never changed: every attempt, to every renewed URL, asks for exactly these bytes.
    internal sealed class ByteRange
    {
        internal ByteRange(long index, long start, long length)
        {
            Index = index;
            Start = start;
            Length = length;
        }

        internal long Index { get; }

        internal long Start { get; }

        internal long Length { get; }

        internal long End => Start + Length;
    }

    // Where a download's bytes go. A part's bytes are received into a PartReceiver, one per attempt, and committed only
    // once the whole part has arrived and been checked; the file is complete once every part has been committed, or its
    // single response has been written in full.
    internal abstract class DownloadSink : IDisposable
    {
        // Whether everything written so far may be discarded to start again from the file's first byte.
        internal abstract bool CanRestart { get; }

        // Discards everything written.
        internal abstract void Restart();

        // The buffer space a part holds from the limiter, from before it is fetched until it has been placed: exactly the
        // bytes its receivers allocate, never more than limit, the limiter's whole buffer space.
        internal abstract long BufferFor(ByteRange part, long limit);

        // Receives one attempt at part, with buffers of bufferBytes (BufferFor). Bytes arrive in order, at positions
        // relative to the part's start.
        internal abstract PartReceiver Receive(ByteRange part, long bufferBytes);

        // part has arrived whole in receiver: it is placed, and reservation, its buffer space, is released once it has.
        internal abstract void Commit(ByteRange part, PartReceiver receiver, IDisposable reservation);

        // Writes bytes of a file that comes in one response, at position, in order.
        internal abstract Task WriteWholeAsync(long position, byte[] buffer, int count, CancellationToken cancellationToken);

        public abstract void Dispose();
    }

    // Where one attempt at a part goes. The body is read into ReadBuffer: in place, when the buffer is the part's own
    // storage (ReadsInPlace), and otherwise a chunk at a time, each then written with WriteAsync. The attempt owns it and
    // disposes it however the attempt ends, after its part is committed when it arrived whole; what a commit handed on,
    // such as a stream's part bytes, is not the receiver's to dispose.
    internal abstract class PartReceiver : IDisposable
    {
        internal abstract byte[] ReadBuffer { get; }

        public virtual void Dispose()
        {
        }

        internal virtual bool ReadsInPlace => false;

        internal virtual Task WriteAsync(long position, byte[] buffer, int count, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("The part is read in place.");
        }
    }

    // A local file, downloaded into a stage (LocalStage) and published under its name once complete. Parts are written at
    // their positions as they arrive, whatever the order, so a part holds only the copy buffer it is read through. A part
    // is recorded as written only once it is whole; a part that failed partway is simply written again. Everything in
    // the stage belongs to this download, so it may start over.
    //
    // Where a later run could not show the stage to be this download's (LocalStage.DigestsRequired), each whole part is
    // also recorded with the SHA-256 of exactly the bytes written for it (Extents), hashed as they are written, so that
    // run can tell which of them the stage still holds (DownloadCheckpoint.Extents).
    internal sealed class FileDownloadSink : DownloadSink
    {
        // The copy buffer a part is read through (ResponseBody's).
        internal const int CopyBufferSize = 81920;

        // For the SDK's own tests: called with 1 as an attempt at a part creates its hash, and with -1 as it disposes it,
        // within the flow of a test that sets it.
        internal static readonly AsyncLocal<Action<int>> HashesForTests = new AsyncLocal<Action<int>>();

        private readonly LocalDirectory ownedDirectory;
        private readonly object recording = new object();
        private List<DigestExtent> extents;
        private IncrementalHash wholeHash;
        private long wholeHashed;

        // ownedDirectory, if not null, is the stage's directory, which the sink closes with the stage. verified are the
        // extents already in the stage, checked against their digests, when the stage requires digests.
        internal FileDownloadSink(LocalDirectory ownedDirectory, LocalStage stage, IntervalSet written, IEnumerable<DigestExtent> verified = null)
        {
            this.ownedDirectory = ownedDirectory;
            Stage = stage;
            Written = written ?? new IntervalSet();
            extents = stage.DigestsRequired ? new List<DigestExtent>(verified ?? Enumerable.Empty<DigestExtent>()) : null;
        }

        internal LocalStage Stage { get; }

        // The parts written to the stage, whole.
        internal IntervalSet Written { get; private set; }

        internal override bool CanRestart => true;

        internal override void Restart()
        {
            Stage.SetLength(0);
            lock (recording)
            {
                Written = new IntervalSet();
                extents = extents == null ? null : new List<DigestExtent>();
                wholeHash?.Dispose();
                wholeHash = null;
                wholeHashed = 0;
            }
        }

        // The stage holds the whole file of size bytes, however it came (in parts, or in one response).
        internal void RecordWhole(long size)
        {
            var whole = new IntervalSet();
            whole.Add(0, size);
            lock (recording)
            {
                Written = whole;
                if (extents != null && wholeHash != null && wholeHashed == size)
                {
                    extents = new List<DigestExtent> { new DigestExtent { Start = 0, End = size, Sha256 = Hex(wholeHash.GetHashAndReset()) } };
                }
            }
        }

        // What a checkpoint records written: the ranges, and, where digests are required, their extents, whose union the
        // ranges then are, so only bytes with a digest are claimed.
        internal (List<long[]> Written, List<DigestExtent> Extents) Recorded()
        {
            lock (recording)
            {
                if (extents == null)
                {
                    return (Written.ToList(), null);
                }
                List<DigestExtent> sorted = extents.OrderBy(extent => extent.Start).Select(extent => new DigestExtent { Start = extent.Start, End = extent.End, Sha256 = extent.Sha256 }).ToList();
                var covered = new IntervalSet();
                foreach (DigestExtent extent in sorted)
                {
                    covered.Add(extent.Start, extent.End);
                }
                return (covered.ToList(), sorted);
            }
        }

        internal static string Hex(byte[] digest)
        {
            return BitConverter.ToString(digest).Replace("-", "").ToLowerInvariant();
        }

        // The copy buffer a part is read through: no larger than the limiter's buffer space, and than the part and one
        // byte more, so that a body longer than the part that arrives in one read is refused before any of it is written.
        // The part's length is bounded by the buffer's before the byte is added, so no part size overflows it.
        internal override long BufferFor(ByteRange part, long limit)
        {
            return Math.Min(Math.Min(CopyBufferSize, limit), Math.Min(part.Length, CopyBufferSize) + 1);
        }

        internal override PartReceiver Receive(ByteRange part, long bufferBytes)
        {
            return new StageReceiver(Stage, part.Start, new byte[bufferBytes], extents != null);
        }

        internal override void Commit(ByteRange part, PartReceiver receiver, IDisposable reservation)
        {
            lock (recording)
            {
                Written.Add(part.Start, part.End);
                // A part whose bytes were not all hashed in order has no digest, so it is not claimed (Recorded).
                string digest = ((StageReceiver)receiver).Digest(part.Length);
                if (extents != null && digest != null)
                {
                    extents.Add(new DigestExtent { Start = part.Start, End = part.End, Sha256 = digest });
                }
            }
            reservation.Dispose();
        }

        internal override async Task WriteWholeAsync(long position, byte[] buffer, int count, CancellationToken cancellationToken)
        {
            await Stage.WriteAsync(position, buffer, count, cancellationToken);
            lock (recording)
            {
                if (extents != null && position == wholeHashed)
                {
                    wholeHash = wholeHash ?? IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
                    wholeHash.AppendData(buffer, 0, count);
                    wholeHashed += count;
                }
            }
        }

        public override void Dispose()
        {
            wholeHash?.Dispose();
            Stage.Dispose();
            ownedDirectory?.Dispose();
        }

        private sealed class StageReceiver : PartReceiver
        {
            private readonly LocalStage stage;
            private readonly long start;
            private readonly IncrementalHash hash;
            private readonly Action<int> counting = HashesForTests.Value;
            private long hashed;
            private bool broken;
            private int disposed;

            internal StageReceiver(LocalStage stage, long start, byte[] buffer, bool hashing)
            {
                this.stage = stage;
                this.start = start;
                ReadBuffer = buffer;
                if (hashing)
                {
                    hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
                    counting?.Invoke(1);
                }
            }

            internal override byte[] ReadBuffer { get; }

            internal override async Task WriteAsync(long position, byte[] buffer, int count, CancellationToken cancellationToken)
            {
                await stage.WriteAsync(start + position, buffer, count, cancellationToken);
                if (hash != null)
                {
                    // The exact bytes just written, in order; any other order leaves the part without a digest.
                    if (position == hashed)
                    {
                        hash.AppendData(buffer, 0, count);
                        hashed += count;
                    }
                    else
                    {
                        broken = true;
                    }
                }
            }

            // The SHA-256 of the part's length bytes written through this receiver, or null when it has none.
            internal string Digest(long length)
            {
                if (hash == null)
                {
                    return null;
                }
                return broken || hashed != length ? null : Hex(hash.GetHashAndReset());
            }

            public override void Dispose()
            {
                if (hash != null && Interlocked.Exchange(ref disposed, 1) == 0)
                {
                    hash.Dispose();
                    counting?.Invoke(-1);
                }
            }
        }
    }

    // A caller's stream, written forward from its position and never seeked, flushed or disposed. Parts may arrive in any
    // order but are written in order: each is read into an array of its own, the buffer space reserved for it, and kept
    // there until the parts before it have been written (DeliverAsync). Only whole, checked parts are written, so after a failure the stream
    // holds whole parts only. A file that comes in one response is written as it arrives; once any of it has been
    // written the download cannot start over, since the stream cannot be taken back.
    internal sealed class StreamDownloadSink : DownloadSink
    {
        private const int WriteChunk = 81920;

        private readonly Stream destination;
        private readonly DownloadProgressTracker progress;
        private readonly Dictionary<long, Arrived> arrived = new Dictionary<long, Arrived>();
        private readonly SemaphoreSlim arrivals = new SemaphoreSlim(0);
        private long delivered;

        internal StreamDownloadSink(Stream destination, DownloadProgressTracker progress)
        {
            this.destination = destination;
            this.progress = progress;
        }

        internal override bool CanRestart => Interlocked.Read(ref delivered) == 0;

        // The bytes written to the stream so far.
        internal long Delivered => Interlocked.Read(ref delivered);

        internal override void Restart()
        {
            if (!CanRestart)
            {
                throw new InvalidOperationException("Bytes already written to the stream cannot be taken back.");
            }
        }

        // The part's own array, which its body is read into: there is no copy buffer. The download plans parts no larger
        // than the limiter's buffer space.
        internal override long BufferFor(ByteRange part, long limit)
        {
            return Math.Max(1, part.Length);
        }

        internal override PartReceiver Receive(ByteRange part, long bufferBytes)
        {
            return new MemoryReceiver(part.Length);
        }

        internal override void Commit(ByteRange part, PartReceiver receiver, IDisposable reservation)
        {
            lock (arrived)
            {
                arrived[part.Index] = new Arrived(((MemoryReceiver)receiver).Bytes, reservation);
            }
            arrivals.Release();
        }

        // Writes the parts in order, from firstIndex until the part after lastIndex, each once the parts before it are
        // written, checking for cancellation between writes. A failure of the stream is thrown as it is.
        internal async Task DeliverAsync(long firstIndex, long lastIndex, CancellationToken cancellationToken)
        {
            for (long index = firstIndex; index <= lastIndex; index++)
            {
                Arrived part;
                while (!TryTake(index, out part))
                {
                    await arrivals.WaitAsync(cancellationToken);
                }
                try
                {
                    for (int offset = 0; offset < part.Bytes.Length; offset += WriteChunk)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        int count = Math.Min(WriteChunk, part.Bytes.Length - offset);
                        await destination.WriteAsync(part.Bytes, offset, count, cancellationToken);
                        Interlocked.Add(ref delivered, count);
                    }
                }
                finally
                {
                    part.Reservation.Dispose();
                }
                progress.PartWritten(part.Bytes.Length);
            }
        }

        internal override async Task WriteWholeAsync(long position, byte[] buffer, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await destination.WriteAsync(buffer, 0, count, cancellationToken);
            Interlocked.Add(ref delivered, count);
        }

        public override void Dispose()
        {
            lock (arrived)
            {
                foreach (Arrived part in arrived.Values)
                {
                    part.Reservation.Dispose();
                }
                arrived.Clear();
            }
            arrivals.Dispose();
        }

        private bool TryTake(long index, out Arrived part)
        {
            lock (arrived)
            {
                if (arrived.TryGetValue(index, out part))
                {
                    arrived.Remove(index);
                    return true;
                }
                return false;
            }
        }

        private sealed class Arrived
        {
            internal Arrived(byte[] bytes, IDisposable reservation)
            {
                Bytes = bytes;
                Reservation = reservation;
            }

            internal byte[] Bytes { get; }

            internal IDisposable Reservation { get; }
        }

        private sealed class MemoryReceiver : PartReceiver
        {
            internal MemoryReceiver(long length)
            {
                Bytes = new byte[length];
            }

            internal byte[] Bytes { get; }

            internal override byte[] ReadBuffer => Bytes;

            internal override bool ReadsInPlace => true;
        }
    }
}