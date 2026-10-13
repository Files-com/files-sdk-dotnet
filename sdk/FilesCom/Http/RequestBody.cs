using FilesCom.Util;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Http
{
    // The body of an API request whose parameters include files (FileAttachment values). It carries every other
    // parameter exactly as the request's JSON would: as multipart/form-data when form fields can, and otherwise as
    // that JSON, in which each file is the API's file object, {"encoded_content": <Base64>, "filename", "type"}.
    //
    // The Files.com API reads form fields with Rack's bracketed names: "paths[]" appends to a list and "value[gravity]"
    // sets a key of an object. It converts a top-level field's text to that parameter's type, such as a boolean or a
    // number, but keeps the text inside an object as text. Form fields have no way to send null, an empty list or
    // object, or a list of objects; and Rack percent-decodes an uploaded file's name, unescapes backslashes in it and
    // keeps only what follows its last slash. So the body is JSON when a value or name is any of those.
    //
    // The files are opened before anything is sent, and read in chunks as each attempt's body is written, so memory
    // stays bounded whatever their size, and every attempt, like every redirect that repeats the request, has the same
    // length and bytes. A failure of a file's source, as it is opened, checked or read, is a SourceFailedException,
    // which no retry, relocation or network failure handling applies to, until FilesApiService throws the source's own
    // exception as it is. That includes an OperationCanceledException the source throws on its own, such as for a
    // timeout of wherever it reads from: only one while the operation's token is cancelled is the operation's own
    // cancellation. Disposing the body releases what opening the files made or opened; a caller's stream stays open.
    internal sealed class RequestBody : IDisposable
    {
        private static readonly SdkLog log = SdkLog.For(typeof(RequestBody));

        private readonly List<Part> parts;
        private readonly List<byte[]> texts;
        private readonly bool base64;
        private readonly string contentType;
        private readonly string json;

        private RequestBody(List<Part> parts, List<byte[]> texts, bool base64, string contentType, string json)
        {
            this.parts = parts;
            this.texts = texts;
            this.base64 = base64;
            this.contentType = contentType;
            this.json = json;
        }

        internal static bool HasFiles(Dictionary<string, object> parameters)
        {
            foreach (object value in parameters.Values)
            {
                if (value is FileAttachment)
                {
                    return true;
                }
            }
            return false;
        }

        // The body for parameters, which HasFiles, once it has been checked that they can be sent: nothing is opened
        // yet. The same failures as serializing the request's JSON, such as a FileAttachment nested in another value.
        internal static RequestBody For(Dictionary<string, object> parameters)
        {
            var parts = new List<Part>();
            var document = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                if (parameter.Value is FileAttachment file)
                {
                    var part = new Part(parameter.Key, file, "files-sdk-file-" + Guid.NewGuid().ToString("N"));
                    parts.Add(part);
                    document[parameter.Key] = new FileObject(part.Marker, file.FileName, part.ContentType);
                }
                else
                {
                    document[parameter.Key] = parameter.Value;
                }
            }
            string json = JsonSerializer.Serialize(document, JsonUtil.Options);

            // A boundary this random appears in no file.
            string boundary = "files-sdk-" + Guid.NewGuid().ToString("N");
            List<byte[]> fields = FormFields(json, parts, boundary);
            return fields != null
                ? new RequestBody(parts, fields, base64: false, "multipart/form-data; boundary=" + boundary, json)
                : new RequestBody(parts, JsonTexts(json, parts), base64: true, "application/json; charset=utf-8", json);
        }

        // What the body sends, for DEBUG: its type, and the parameters as JSON, where each file is its file object
        // with the file's length in place of its bytes. Complete once the files are open.
        internal string Description
        {
            get
            {
                string text = json;
                foreach (Part part in parts)
                {
                    string length = part.Source == null ? "the file" : $"{part.Source.Length} bytes";
                    text = text.Replace($"\"{part.Marker}\"", $"\"({length}{(base64 ? " as Base64" : ", sent as a form file")})\"");
                }
                return $"{(base64 ? "JSON" : "multipart/form-data")} {text}";
            }
        }

        // Opens each file: the file at a path, a stream's length and position, or a copy of a stream that cannot seek.
        // A failure is a SourceFailedException; disposing the body releases the files opened before it.
        internal async Task OpenAsync(CancellationToken cancellationToken)
        {
            foreach (Part part in parts)
            {
                try
                {
                    part.Source = await part.File.OpenAsync(part.Name, cancellationToken);
                }
                catch (Exception e) when (!FilesClient.IsRequestedCancellation(e, cancellationToken))
                {
                    throw new SourceFailedException(e);
                }
            }
        }

        // A new content for one attempt, of the same bytes as every other. Each file must still have the length it had
        // when opened: a caller's stream or a file that has changed fails, as does a stream that cannot say, with a
        // SourceFailedException, before anything is sent.
        internal AttachmentContent CreateContent(CancellationToken cancellationToken)
        {
            foreach (Part part in parts)
            {
                try
                {
                    part.Source.CheckUnchanged();
                }
                catch (Exception e) when (!FilesClient.IsRequestedCancellation(e, cancellationToken))
                {
                    throw new SourceFailedException(e);
                }
            }
            return new AttachmentContent(this, cancellationToken);
        }

        // A failure to release what a file needed is logged, never thrown over the operation's own outcome.
        public void Dispose()
        {
            foreach (Part part in parts)
            {
                try
                {
                    part.Source?.Dispose();
                }
                catch (Exception e)
                {
                    DiagnosticText.WarnCleanupFailed(log, $"Sending {part.Name}: closing its file failed", e);
                }
                part.Source = null;
            }
        }

        // The form fields of json, the request's parameters, as multipart text around each file in parts, or null if
        // form fields would change a value or name. The texts are written, in order, around each file's bytes.
        private static List<byte[]> FormFields(string json, List<Part> parts, string boundary)
        {
            var body = new StringBuilder();
            var texts = new List<byte[]>();
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                int next = 0;
                foreach (JsonProperty parameter in document.RootElement.EnumerateObject())
                {
                    if (!IsFormName(parameter.Name))
                    {
                        return null;
                    }
                    if (next < parts.Count && parts[next].Name == parameter.Name)
                    {
                        Part part = parts[next++];
                        if (!IsFormFileName(part.File.FileName))
                        {
                            return null;
                        }
                        body.Append("--").Append(boundary).Append("\r\n")
                            .Append("Content-Disposition: form-data; name=\"").Append(parameter.Name).Append("\"; filename=\"").Append(part.File.FileName).Append("\"\r\n")
                            .Append("Content-Type: ").Append(part.ContentType).Append("\r\n\r\n");
                        texts.Add(Encoding.UTF8.GetBytes(body.ToString()));
                        body.Clear();
                        body.Append("\r\n");
                    }
                    else if (!AddFields(body, boundary, parameter.Name, parameter.Value, inObject: false))
                    {
                        return null;
                    }
                }
            }
            body.Append("--").Append(boundary).Append("--\r\n");
            texts.Add(Encoding.UTF8.GetBytes(body.ToString()));
            return texts;
        }

        private static bool AddFields(StringBuilder body, string boundary, string name, JsonElement value, bool inObject)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                    bool any = false;
                    foreach (JsonProperty property in value.EnumerateObject())
                    {
                        if (!IsFormName(property.Name) || !AddFields(body, boundary, name + "[" + property.Name + "]", property.Value, inObject: true))
                        {
                            return false;
                        }
                        any = true;
                    }
                    return any;
                case JsonValueKind.Array:
                    bool items = false;
                    foreach (JsonElement item in value.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.Object || item.ValueKind == JsonValueKind.Array || !AddFields(body, boundary, name + "[]", item, inObject))
                        {
                            return false;
                        }
                        items = true;
                    }
                    return items;
                default:
                    string text = FormText(value, inObject);
                    if (text == null)
                    {
                        return false;
                    }
                    body.Append("--").Append(boundary).Append("\r\n")
                        .Append("Content-Disposition: form-data; name=\"").Append(name).Append("\"\r\n\r\n")
                        .Append(text).Append("\r\n");
                    return true;
            }
        }

        // The form text that carries a JSON scalar exactly, or null. The API converts top-level text to the
        // parameter's type, so there a boolean or a number reads back as itself; inside an object it stays text.
        private static string FormText(JsonElement value, bool inObject)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.String:
                    return value.GetString();
                case JsonValueKind.True:
                    return inObject ? null : "true";
                case JsonValueKind.False:
                    return inObject ? null : "false";
                case JsonValueKind.Number:
                    string number = value.GetRawText();
                    return inObject || !IsPlainNumber(number) ? null : number;
                default:
                    return null;
            }
        }

        // Digits, with a sign and a fraction, as text converted to an integer or decimal reads them.
        private static bool IsPlainNumber(string number)
        {
            foreach (char c in number)
            {
                if (!(c >= '0' && c <= '9') && c != '-' && c != '.')
                {
                    return false;
                }
            }
            return true;
        }

        // Brackets structure form names, and a quote, a control character, a percent sign or a backslash could reach
        // the API as something else.
        private static bool IsFormName(string name)
        {
            if (name.Length == 0)
            {
                return false;
            }
            foreach (char c in name)
            {
                if (c == '[' || c == ']' || c == '"' || c == '%' || c == '\\' || char.IsControl(c))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsFormFileName(string fileName)
        {
            return fileName.IndexOfAny(new[] { '%', '\\', '/', '"' }) < 0;
        }

        // The texts of json around each file's marker, in the order of parts, as its encoded_content string is written.
        private static List<byte[]> JsonTexts(string json, List<Part> parts)
        {
            var texts = new List<byte[]>();
            string rest = json;
            foreach (Part part in parts)
            {
                string marker = $"\"{part.Marker}\"";
                int at = rest.IndexOf(marker, StringComparison.Ordinal);
                texts.Add(Encoding.UTF8.GetBytes(rest.Substring(0, at + 1)));
                rest = rest.Substring(at + marker.Length - 1);
            }
            texts.Add(Encoding.UTF8.GetBytes(rest));
            return texts;
        }

        // The API's JSON file object, as sent for a file in a JSON body, around the marker its Base64 replaces.
        private sealed class FileObject
        {
            internal FileObject(string encodedContent, string fileName, string type)
            {
                EncodedContent = encodedContent;
                FileName = fileName;
                Type = type;
            }

            [JsonPropertyName("encoded_content")]
            public string EncodedContent { get; }

            [JsonPropertyName("filename")]
            public string FileName { get; }

            [JsonPropertyName("type")]
            public string Type { get; }
        }

        // One file parameter.
        private sealed class Part
        {
            internal Part(string name, FileAttachment file, string marker)
            {
                Name = name;
                File = file;
                Marker = marker;
                ContentType = file.ContentType ?? "application/octet-stream";
            }

            internal string Name { get; }

            internal FileAttachment File { get; }

            internal string Marker { get; }

            internal string ContentType { get; }

            internal AttachmentSource Source { get; set; }
        }

        // A file's source failed with Failure, whatever its type, even one a network failure could have, such as the
        // HttpRequestException, or the OperationCanceledException of a timeout, of a stream read from the network: it is
        // the caller's, and the request is not sent again.
        internal sealed class SourceFailedException : Exception
        {
            internal SourceFailedException(Exception failure) : base("A file parameter's source failed.", failure)
            {
                Failure = failure;
            }

            internal Exception Failure { get; }
        }

        // The body of one attempt: the texts, with each file's bytes, or their Base64, between them. Its length is
        // known before it is sent. It reads each file a chunk at a time as it writes, and can be written again, as a
        // redirect that repeats the request requires. Disposing it leaves the files open for the next attempt.
        internal sealed class AttachmentContent : HttpContent
        {
            // A multiple of 3, so that the Base64 of every chunk but the last has no padding, and the chunks join into
            // the Base64 of the whole file.
            private const int ChunkSize = 48 * 1024;

            private readonly RequestBody body;
            private readonly CancellationToken cancellationToken;

            internal AttachmentContent(RequestBody body, CancellationToken cancellationToken)
            {
                this.body = body;
                this.cancellationToken = cancellationToken;
                Headers.ContentType = MediaTypeHeaderValue.Parse(body.contentType);
            }

            // Why reading a file failed, which the HTTP stack reports as a failure to send, or null.
            internal Exception SourceFailure { get; private set; }

            protected override bool TryComputeLength(out long length)
            {
                length = 0;
                foreach (byte[] text in body.texts)
                {
                    length += text.Length;
                }
                foreach (Part part in body.parts)
                {
                    length += body.base64 ? 4 * ((part.Source.Length + 2) / 3) : part.Source.Length;
                }
                return true;
            }

            protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                byte[] chunk = new byte[ChunkSize];
                byte[] encoded = body.base64 ? new byte[ChunkSize / 3 * 4] : null;
                for (int i = 0; i < body.parts.Count; i++)
                {
                    await stream.WriteAsync(body.texts[i], 0, body.texts[i].Length, cancellationToken);
                    Part part = body.parts[i];
                    for (long sent = 0; sent < part.Source.Length;)
                    {
                        int count = await FillAsync(part, sent, chunk);
                        if (encoded == null)
                        {
                            await stream.WriteAsync(chunk, 0, count, cancellationToken);
                        }
                        else
                        {
                            Base64.EncodeToUtf8(new ReadOnlySpan<byte>(chunk, 0, count), encoded, out _, out int written, isFinalBlock: true);
                            await stream.WriteAsync(encoded, 0, written, cancellationToken);
                        }
                        sent += count;
                    }
                }
                byte[] last = body.texts[body.texts.Count - 1];
                await stream.WriteAsync(last, 0, last.Length, cancellationToken);
            }

            // Reads the next chunk of part's file, at position: a whole chunk, unless the file ends first.
            private async Task<int> FillAsync(Part part, long position, byte[] chunk)
            {
                int wanted = (int)Math.Min(chunk.Length, part.Source.Length - position);
                int filled = 0;
                while (filled < wanted)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    int count;
                    try
                    {
                        count = await part.Source.ReadAsync(position + filled, chunk, filled, wanted - filled, cancellationToken);
                    }
                    catch (Exception e) when (!FilesClient.IsRequestedCancellation(e, cancellationToken))
                    {
                        SourceFailure = e;
                        throw;
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    if (count <= 0)
                    {
                        SourceFailure = new EndOfStreamException($"The file for {part.Name} ended after {position + filled} of the {part.Source.Length} bytes it had when the operation started, so its request was not sent.");
                        throw SourceFailure;
                    }
                    filled += count;
                }
                return filled;
            }
        }
    }
}