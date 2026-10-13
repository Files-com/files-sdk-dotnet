using FilesCom.Http;
using FilesCom.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Transfers
{
    // Which bytes a part holds. Set once, when the part is planned, and never changed: every attempt to send the part,
    // including one to a renewed URL, sends these bytes as this part number.
    internal sealed class PartDescriptor
    {
        internal PartDescriptor(long number, long offset, long length, bool isLast, int sizeEpoch = 0)
        {
            Number = number;
            Offset = offset;
            Length = length;
            IsLast = isLast;
            SizeEpoch = sizeEpoch;
        }

        internal long Number { get; }

        internal long Offset { get; }

        internal long Length { get; }

        internal bool IsLast { get; }

        // The size epoch of an upload that chooses its part sizes, which measurements are grouped by.
        internal int SizeEpoch { get; }
    }

    // The next part to read. For an upload of known length its descriptor and target are final. For one of unknown
    // length, the descriptor's length is the most the part may hold, and Target is null: the part is frozen by
    // PartPlanner.Freeze once it has been read. An end check is a single byte read where the stream must end, at its
    // bound or after the most parts it may have: it is sent only as the empty first part of an empty stream, and a byte
    // read shows the stream to be longer than the upload allows.
    internal sealed class PartToRead
    {
        internal PartToRead(PartDescriptor part, PartTarget target, FileUploadPart negotiated, bool endCheck = false)
        {
            Part = part;
            Target = target;
            Negotiated = negotiated;
            EndCheck = endCheck;
        }

        internal PartDescriptor Part { get; }

        internal PartTarget Target { get; }

        internal FileUploadPart Negotiated { get; }

        internal bool EndCheck { get; }
    }

    // A planned part: its bytes and where and how the server asked for them to be sent.
    internal sealed class PlannedPart
    {
        internal PlannedPart(PartDescriptor part, PartTarget target)
        {
            Part = part;
            Target = target;
        }

        internal PartDescriptor Part { get; }

        internal PartTarget Target { get; }
    }

    // Where and how to send one part, as a begin_upload response negotiated it. Only instructions this SDK knows how
    // to follow are accepted; anything else fails before the part's bytes are sent rather than being guessed at.
    internal sealed class PartTarget
    {
        private PartTarget(HttpMethod method, string uri, IReadOnlyList<KeyValuePair<string, string>> headers, DateTimeOffset? expiresAt, bool parallelParts, bool retryParts)
        {
            Method = method;
            Uri = uri;
            Headers = headers;
            ExpiresAt = expiresAt;
            ParallelParts = parallelParts;
            RetryParts = retryParts;
        }

        internal HttpMethod Method { get; }

        // Sent exactly as given: signed URLs are never rewritten.
        internal string Uri { get; }

        internal IReadOnlyList<KeyValuePair<string, string>> Headers { get; }

        internal DateTimeOffset? ExpiresAt { get; }

        // Whether this part may be sent while other parts of the upload are being sent.
        internal bool ParallelParts { get; }

        // Whether this part may be sent again after a failed attempt.
        internal bool RetryParts { get; }

        internal bool ExpiresWithin(TimeSpan time)
        {
            return ExpiresAt.HasValue && ExpiresAt.Value <= DateTimeOffset.UtcNow + time;
        }

        // The target of part, as negotiated.
        internal static PartTarget For(FileUploadPart negotiated, PartDescriptor part)
        {
            List<KeyValuePair<string, string>> headers = Check(negotiated, part.Number);
            foreach (KeyValuePair<string, string> header in headers)
            {
                if (string.Equals(header.Key, "Content-Length", StringComparison.OrdinalIgnoreCase) && header.Value.Trim() != part.Length.ToString(CultureInfo.InvariantCulture))
                {
                    throw new InvalidResponseException($"The server asked for part {part.Number} to be sent with Content-Length {header.Value}, but the part is {part.Length} bytes.");
                }
            }
            headers.RemoveAll(header => string.Equals(header.Key, "Content-Length", StringComparison.OrdinalIgnoreCase));

            // The earlier of the expiry Files.com negotiated and the one the URL's signature states.
            DateTimeOffset expires;
            DateTimeOffset? negotiatedExpiry = DateTimeOffset.TryParse(negotiated.Expires, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out expires) ? expires : (DateTimeOffset?)null;
            DateTimeOffset? expiresAt = SignedUrl.Of(negotiated.UploadUri, negotiatedExpiry).ExpiresAt;
            CheckPartOffset(negotiated.UploadUri, part);
            return new PartTarget(new HttpMethod(negotiated.HttpMethod), negotiated.UploadUri, headers.AsReadOnly(), expiresAt, negotiated.ParallelParts, negotiated.RetryParts);
        }

        // Checks what can be checked before the part's length is known, and returns its headers.
        internal static List<KeyValuePair<string, string>> Check(FileUploadPart negotiated, long number)
        {
            if (string.IsNullOrEmpty(negotiated.UploadUri) || string.IsNullOrEmpty(negotiated.HttpMethod))
            {
                throw new InvalidResponseException($"The server did not say where to send part {number}.");
            }
            CheckSendInstructions(negotiated.Send, number);
            if (Strings(negotiated.Parameters, "parameters", number).Count > 0)
            {
                throw new NotImplementedException($"The server asked for part {number} to be sent with parameters, which this version of the SDK cannot send.");
            }
            return Strings(negotiated.Headers, "headers", number);
        }

        // A URL that names the part's offset in part_offset must name the part's own offset, once: one Files.com signed
        // with another, first or on renewal, would write the part elsewhere, so it is refused before the part is sent.
        // The SDK never adds an offset itself: the URL is sent exactly as given.
        private static void CheckPartOffset(string uri, PartDescriptor part)
        {
            string offset = part.Offset.ToString(CultureInfo.InvariantCulture);
            int fragment = uri.IndexOf('#');
            string beforeFragment = fragment < 0 ? uri : uri.Substring(0, fragment);
            int query = beforeFragment.IndexOf('?');
            int named = 0;
            bool sameOffset = false;
            if (query >= 0)
            {
                foreach (string pair in beforeFragment.Substring(query + 1).Split('&'))
                {
                    int equals = pair.IndexOf('=');
                    if (System.Uri.UnescapeDataString(equals < 0 ? pair : pair.Substring(0, equals)) != "part_offset")
                    {
                        continue;
                    }
                    named++;
                    sameOffset = equals >= 0 && System.Uri.UnescapeDataString(pair.Substring(equals + 1)) == offset;
                }
            }
            if (named > 1 || (named == 1 && !sameOffset))
            {
                throw new InvalidResponseException($"The server's URL for part {part.Number} names a part_offset other than the part's offset, {offset}, so the part was not sent.");
            }
        }

        // The only format the server asks for today: the part's size in the Content-Length header, its bytes as the
        // request body.
        private static void CheckSendInstructions(object send, long number)
        {
            foreach (KeyValuePair<string, string> instruction in Strings(send, "send", number))
            {
                bool understood = (instruction.Key == "partsize" && string.Equals(instruction.Value, "required-header Content-Length", StringComparison.OrdinalIgnoreCase))
                    || (instruction.Key == "partdata" && instruction.Value == "body");
                if (!understood)
                {
                    throw new NotImplementedException($"The server asked for part {number} to be sent with {instruction.Key} as \"{instruction.Value}\", which this version of the SDK cannot do.");
                }
            }
        }

        // A JSON object of strings from a FileUploadPart field; empty when the field is missing or null.
        private static List<KeyValuePair<string, string>> Strings(object field, string name, long number)
        {
            var strings = new List<KeyValuePair<string, string>>();
            if (field == null)
            {
                return strings;
            }
            if (!(field is JsonElement json) || (json.ValueKind != JsonValueKind.Object && json.ValueKind != JsonValueKind.Null))
            {
                throw new InvalidResponseException($"The {name} of part {number} are not an object.");
            }
            if (json.ValueKind == JsonValueKind.Null)
            {
                return strings;
            }
            foreach (JsonProperty property in json.EnumerateObject())
            {
                if (property.Value.ValueKind != JsonValueKind.String)
                {
                    throw new InvalidResponseException($"The {name} of part {number} include {property.Name}, which is not a string.");
                }
                strings.Add(new KeyValuePair<string, string>(property.Name, property.Value.GetString()));
            }
            return strings;
        }
    }

    // Divides an upload into parts and gives each its number, offset and length once, in order, as it is planned. How
    // long a part is depends on what Files.com allows:
    //
    //   the server's sizes  part n holds the partsize bytes its own negotiation gave, or what remains of the file if
    //                       that is less, so the server's rules, such as equal part sizes, hold;
    //   chosen sizes        where the parts go straight to Amazon S3 as an ordinary multipart upload (UploadGrant),
    //                       PartSizes chooses each length within the SDK's limits for S3 as the part is planned, from
    //                       the size preferred at that moment;
    //   unknown length      the stream is read one part at a time, in order, up to the size chosen for the part, or
    //                       else the partsize of its own negotiation, and the part is frozen once read, holding what
    //                       the stream gave. A part that fills is not known to be the last until the next part's read
    //                       returns nothing, and that part is then not sent. With chosen sizes, the stream stays within
    //                       its bound, the smaller of MaxStreamLength and the largest file the limits allow, and the
    //                       most parts they allow; with the server's sizes, which the SDK's limits do not bind, within
    //                       MaxStreamLength and available_parts. A stream that gives more fails once that is found,
    //                       without the excess being sent, and is not completed.
    //
    // The first begin_upload answer shows what the upload may do (UploadGrant); every later answer, for new parts or
    // renewed URLs, is checked against what the upload relies on. While every part so far may be sent in parallel, the
    // planner negotiates several parts per request, up to the upload's concurrency and no more than the file still
    // needs; negotiating a URL does not size its part, which happens only when the part is planned.
    internal sealed class PartPlanner
    {
        private readonly UploadSession session;
        private readonly long? totalLength;
        private readonly long? declaredBound;
        private readonly Queue<FileUploadPart> negotiated = new Queue<FileUploadPart>();
        // Parts an earlier run planned but did not get accepted, sent again first, as they were planned.
        private readonly Queue<PartDescriptor> resumed = new Queue<PartDescriptor>();
        private FileUploadPart resumedFirst;
        private PartSizes sizes;
        private int maxPartsPerRequest = 1;
        private long nextNumber = 1;
        private long nextOffset;
        private long nextSizeHint;
        private bool endOfSource;
        // Also cleared by a renewal, which may run while the next parts are being planned.
        private volatile bool everyPartParallel = true;

        // totalLength is null for a stream of unknown length, which may give at most declaredBound bytes, if that is not
        // null.
        internal PartPlanner(UploadSession session, long? totalLength, long? declaredBound)
        {
            this.session = session;
            this.totalLength = totalLength;
            this.declaredBound = declaredBound;
        }

        // What the first answer shows the upload may do. Set by StartAsync or ResumeAsync.
        internal UploadGrant Grant { get; private set; }

        // Whether the first part may be sent alongside others.
        internal bool FirstPartParallel { get; private set; }

        // The number of parts the first answer said the upload may have, if it said.
        internal long? AvailableParts { get; private set; }

        // The size of the largest part planned so far.
        internal long LargestPartSize { get; private set; }

        // The number of parts planned so far.
        internal long PartCount => nextNumber - 1;

        // The bytes of the parts planned so far: the upload's size once every part has been.
        internal long PlannedBytes => nextOffset;

        // Whether every part planned so far, and every renewed URL, allowed sending parts in parallel.
        internal bool EveryPartParallel => everyPartParallel;

        // Whether every part has been planned: for a stream of unknown length, once it has ended.
        internal bool AllPlanned => Planned;

        // For a stream of unknown length, the most bytes it may hold, if anything bounds it: the chosen sizes' bound, or
        // with the server's sizes, the caller's. Null for a file.
        internal long? StreamBound => totalLength.HasValue ? null : sizes != null ? sizes.StreamBound : declaredBound;

        // For a stream of unknown length, the most parts it may have, if anything bounds them: the chosen sizes' count,
        // or with the server's sizes, the parts the first answer said were available.
        private long? PartSlots => sizes != null ? sizes.MaxParts : AvailableParts >= 1 ? AvailableParts : null;

        internal PartSizes Sizes => sizes;

        // Starts the upload: negotiates its first part, or parts, and reads what they grant.
        internal async Task<UploadGrant> StartAsync(CancellationToken cancellationToken)
        {
            FileUploadPart[] parts = await session.BeginAsync(totalLength, cancellationToken);
            CheckNumbering(parts, 1);
            Grant = UploadGrant.Of(parts, 1);
            FirstPartParallel = parts[0].ParallelParts;
            AvailableParts = parts[0].AvailableParts;
            foreach (FileUploadPart part in parts)
            {
                negotiated.Enqueue(part);
            }
            return Grant;
        }

        // Continues an upload an earlier run started with reference and planned as planned, a prefix of its parts, of
        // which those numbered in accepted need not be sent again. The parts not accepted are sent first, each exactly as
        // it was planned; the rest of the file is planned as usual from where the earlier run stopped. The first answer,
        // for the first part still to send, is this run's grant; the caller checks it is the grant the upload started with.
        internal async Task<UploadGrant> ResumeAsync(string reference, IReadOnlyList<PartDescriptor> planned, ISet<long> accepted, CancellationToken cancellationToken)
        {
            session.Resume(reference);
            foreach (PartDescriptor part in planned)
            {
                if (!accepted.Contains(part.Number))
                {
                    resumed.Enqueue(part);
                }
                nextOffset = checked(nextOffset + part.Length);
                LargestPartSize = Math.Max(LargestPartSize, part.Length);
            }
            nextNumber = planned.Count + 1;
            // When every part has been planned and accepted, only the final request remains: the last part is asked for,
            // to learn whether Files.com still knows the upload and what it allows it, and is not sent again.
            bool wholeFilePlanned = planned.Count > 0 && totalLength.HasValue && nextOffset == totalLength.Value;
            long first = resumed.Count > 0 ? resumed.Peek().Number : wholeFilePlanned ? planned.Count : nextNumber;
            FileUploadPart[] parts = CheckNumbering(await session.ContinueAsync(first, 1, cancellationToken), first);
            Grant = UploadGrant.Of(parts, first);
            FirstPartParallel = parts[0].ParallelParts;
            AvailableParts = parts[0].AvailableParts;
            if (resumed.Count > 0)
            {
                resumedFirst = parts[0];
            }
            else if (!wholeFilePlanned)
            {
                negotiated.Enqueue(parts[0]);
            }
            return Grant;
        }

        // How parts are sized, once the upload has chosen: sizes is null for the server's.
        internal void Use(PartSizes sizes, int maxPartsPerRequest)
        {
            this.sizes = sizes;
            this.maxPartsPerRequest = maxPartsPerRequest;
        }

        // The next part to read, or null when the whole file has been planned. An empty file is one empty part. The
        // part's instructions are checked here, before it is read.
        internal async Task<PartToRead> NextAsync(CancellationToken cancellationToken)
        {
            if (resumed.Count > 0)
            {
                return await NextResumedAsync(cancellationToken);
            }
            if (Planned)
            {
                return null;
            }
            if (!totalLength.HasValue)
            {
                return await NextOfStreamAsync(cancellationToken);
            }
            if (negotiated.Count == 0)
            {
                await NegotiateAsync(cancellationToken);
            }
            FileUploadPart next = negotiated.Dequeue();
            long remaining = totalLength.Value - nextOffset;
            PartDescriptor part;
            if (sizes != null)
            {
                long length = sizes.LengthFor(nextNumber, nextOffset, out int sizeEpoch);
                part = new PartDescriptor(nextNumber, nextOffset, length, isLast: length == remaining, sizeEpoch);
            }
            else
            {
                long partsize = next.Partsize ?? 0;
                if (remaining > 0 && partsize <= 0)
                {
                    throw new InvalidResponseException($"The server gave no size for part {nextNumber}.");
                }
                part = new PartDescriptor(nextNumber, nextOffset, Math.Min(remaining, partsize), isLast: remaining <= partsize);
                nextSizeHint = next.NextPartsize ?? partsize;
            }
            PartTarget target = Target(next, part);
            nextNumber++;
            nextOffset = checked(nextOffset + part.Length);
            LargestPartSize = Math.Max(LargestPartSize, part.Length);
            return new PartToRead(part, target, next);
        }

        // The next part an earlier run planned but did not get accepted, with a new URL for it.
        private async Task<PartToRead> NextResumedAsync(CancellationToken cancellationToken)
        {
            PartDescriptor part = resumed.Dequeue();
            FileUploadPart answer = resumedFirst ?? CheckNumbering(await session.ContinueAsync(part.Number, 1, cancellationToken), part.Number)[0];
            resumedFirst = null;
            if (sizes == null)
            {
                long partsize = answer.Partsize ?? 0;
                if (part.IsLast ? partsize < part.Length : partsize != part.Length)
                {
                    throw new InvalidResponseException($"The server now plans part {part.Number} at {partsize} bytes, not the {part.Length} it was planned with, so the upload was stopped.");
                }
            }
            return new PartToRead(part, Target(answer, part), answer);
        }

        // The next part of a stream of unknown length. Once the stream has reached its bound, or has as many parts as it
        // may, the next read is an end check, which negotiates nothing; otherwise the part may hold at most its size and
        // what is left of the bound. The server's sizes are taken as the server plans them: the SDK's limits bind only
        // the sizes it chooses.
        private async Task<PartToRead> NextOfStreamAsync(CancellationToken cancellationToken)
        {
            long? bound = StreamBound;
            long? slots = PartSlots;
            bool atBound = bound.HasValue && nextOffset >= bound.Value;
            if (nextNumber > 1 && (atBound || nextNumber > slots))
            {
                return new PartToRead(new PartDescriptor(nextNumber, nextOffset, 1, isLast: false), null, null, endCheck: true);
            }
            if (negotiated.Count == 0)
            {
                await NegotiateAsync(cancellationToken);
            }
            FileUploadPart next = negotiated.Dequeue();
            PartTarget.Check(next, nextNumber);
            if (atBound)
            {
                // A bound of 0: the stream must be empty, and is sent as one empty part.
                return new PartToRead(new PartDescriptor(nextNumber, nextOffset, 1, isLast: false), null, next, endCheck: true);
            }
            long room = bound.HasValue ? bound.Value - nextOffset : long.MaxValue;
            long most;
            int sizeEpoch = 0;
            if (sizes != null)
            {
                most = sizes.LengthFor(nextNumber, nextOffset, out sizeEpoch);
            }
            else
            {
                most = next.Partsize ?? 0;
                if (most <= 0)
                {
                    throw new InvalidResponseException($"The server gave no size for part {nextNumber}.");
                }
                most = Math.Min(most, room);
            }
            return new PartToRead(new PartDescriptor(nextNumber, nextOffset, most, isLast: false, sizeEpoch), null, next);
        }

        // The part once read: for a stream of unknown length, its length is what was read, and it is the last part if
        // the stream ended there. When it read nothing, the stream ended where the part before did, which was the last:
        // nothing more is sent, and Freeze returns null. Other parts are already final.
        internal PlannedPart Freeze(PartToRead next, long length, bool endOfStream)
        {
            if (totalLength.HasValue)
            {
                return new PlannedPart(next.Part, next.Target);
            }
            if (next.EndCheck && length > 0)
            {
                throw nextOffset >= StreamBound
                    ? new UploadLimitException(UploadLimit.StreamLength, $"The stream is longer than the {StreamBound} bytes the upload allows it (UploadOptions.MaxStreamLength, or the largest file the SDK plans part sizes for), so the upload was stopped and not completed.")
                    : new UploadLimitException(UploadLimit.PartCount, $"The stream is longer than its {PartSlots} parts, the most the upload may have, can hold, so the upload was stopped and not completed.");
            }
            if (length == 0 && next.Part.Number > 1)
            {
                endOfSource = true;
                return null;
            }
            var part = new PartDescriptor(next.Part.Number, next.Part.Offset, length, endOfStream, next.Part.SizeEpoch);
            PartTarget target = Target(next.Negotiated, part);
            nextNumber++;
            nextOffset = checked(nextOffset + length);
            endOfSource = endOfStream;
            LargestPartSize = Math.Max(LargestPartSize, length);
            return new PlannedPart(part, target);
        }

        // A new URL for a planned part, for when its URL is about to expire. The part keeps its number, offset and
        // length: with the server's sizes, the server must still plan the same size for it; with chosen sizes, the URL
        // must be for the same S3 upload and part, whatever size it recommends. Safe to call while planning continues.
        internal async Task<PartTarget> RenewAsync(PartDescriptor part, CancellationToken cancellationToken)
        {
            FileUploadPart renewed = CheckNumbering(await session.ContinueAsync(part.Number, 1, cancellationToken), part.Number)[0];
            if (sizes == null)
            {
                // The last part, or one cut short by the stream's bound, may be smaller than the server's size.
                long partsize = renewed.Partsize ?? 0;
                bool sameSize = part.IsLast || part.Offset + part.Length == StreamBound ? partsize >= part.Length : partsize == part.Length;
                if (!sameSize)
                {
                    throw new InvalidResponseException($"The server changed the size of part {part.Number} from {part.Length} to {partsize} bytes when renewing its URL.");
                }
            }
            PartTarget target = PartTarget.For(renewed, part);
            if (target.ExpiresWithin(TimeSpan.Zero))
            {
                // Renewing it again would get no further.
                throw new InvalidResponseException($"The server renewed the URL of part {part.Number} with one that has already expired.");
            }
            if (!target.ParallelParts)
            {
                everyPartParallel = false;
            }
            return target;
        }

        private bool Planned => totalLength.HasValue ? nextNumber > 1 && nextOffset >= totalLength.Value : endOfSource;

        private PartTarget Target(FileUploadPart negotiatedPart, PartDescriptor part)
        {
            PartTarget target = PartTarget.For(negotiatedPart, part);
            if (!target.ParallelParts)
            {
                everyPartParallel = false;
            }
            return target;
        }

        private async Task NegotiateAsync(CancellationToken cancellationToken)
        {
            FileUploadPart[] parts = await session.ContinueAsync(nextNumber, PartsToRequest(), cancellationToken);
            foreach (FileUploadPart part in CheckNumbering(parts, nextNumber))
            {
                negotiated.Enqueue(part);
            }
        }

        // Parts are negotiated several at a time only while they may be sent in parallel, and a stream of unknown length
        // only when the upload chooses its part sizes, as far as its bound could need.
        private long PartsToRequest()
        {
            if (!everyPartParallel || maxPartsPerRequest <= 1 || (!totalLength.HasValue && sizes == null))
            {
                return 1;
            }
            long perPart = sizes != null ? sizes.PreferredSize : nextSizeHint;
            if (perPart <= 0)
            {
                return 1;
            }
            long remaining = (totalLength ?? sizes.StreamBound.Value) - nextOffset;
            long count = Math.Min(maxPartsPerRequest, PartSizes.CeilingDivide(remaining, perPart));
            if (!totalLength.HasValue)
            {
                // Never beyond the most parts the stream may have.
                count = Math.Min(count, PartSlots.Value - nextNumber + 1);
            }
            return Math.Max(1, count);
        }

        // The parts of one response, which must be numbered consecutively from firstNumber, belong to this upload and,
        // after the first answer, still allow what the upload relies on (UploadGrant.CheckStillAllowed).
        private FileUploadPart[] CheckNumbering(FileUploadPart[] parts, long firstNumber)
        {
            if (parts == null || parts.Length == 0)
            {
                throw new InvalidResponseException($"The server returned no upload part {firstNumber}.");
            }
            for (int i = 0; i < parts.Length; i++)
            {
                long expected = firstNumber + i;
                if (parts[i].PartNumber.HasValue && parts[i].PartNumber.Value != expected)
                {
                    throw new InvalidResponseException($"The server returned upload part {parts[i].PartNumber} instead of part {expected}.");
                }
                if (!string.IsNullOrEmpty(parts[i].Ref) && parts[i].Ref != session.Ref)
                {
                    throw new InvalidResponseException($"The server returned upload part {expected} for another upload.");
                }
                Grant?.CheckStillAllowed(parts[i], expected, sizes != null);
            }
            return parts;
        }
    }
}