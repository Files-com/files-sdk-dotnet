using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FilesTests
{
    // An HTTP/1.1 server on a loopback port whose connections a test scripts, for what the WireMock site cannot do:
    // hold its answer, send a body slowly or stop halfway, or stop reading a request body. serve runs once for each
    // connection the client opens, and the server records every connection, so a test can check how many the client
    // opened and see when the client closed one: that is how a test knows a stalled request was really ended, not
    // merely abandoned. With certificateFor, it serves HTTPS, with the certificate it returns for the name the client
    // asks for; a connection whose handshake fails is recorded, and not served.
    //
    // With receiveBuffer, each connection receives into a buffer of about that many bytes, set before the client
    // connects, so that its TCP window stays that small. A connection that stops reading (StallAsync) then takes little
    // of a request body, and the client's send buffer, which the operating system grows only as the connection moves
    // data, stays near its starting size. Without it, a host whose TCP buffers are tuned large can take a whole large
    // body into the connection, so that a client that should be unable to send finishes sending.
    public sealed class ScriptedServer : IDisposable
    {
        private readonly TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource stopping = new CancellationTokenSource();
        private readonly List<ScriptedConnection> connections = new List<ScriptedConnection>();
        private readonly Func<ScriptedConnection, Task> serve;
        private readonly Func<string, X509Certificate2> certificateFor;

        public ScriptedServer(Func<ScriptedConnection, Task> serve, Func<string, X509Certificate2> certificateFor = null, int receiveBuffer = 0)
        {
            this.serve = serve;
            this.certificateFor = certificateFor;
            if (receiveBuffer > 0)
            {
                listener.Server.ReceiveBufferSize = receiveBuffer;
            }
            listener.Start();
            Port = ((IPEndPoint)listener.LocalEndpoint).Port;
            _ = AcceptAsync();
        }

        public int Port { get; }

        // With a stand-in signature, as a signed storage URL has.
        public string Url => $"http://127.0.0.1:{Port}/file?X-Files-Signature=stand-in";

        public IReadOnlyList<ScriptedConnection> Connections
        {
            get
            {
                lock (connections)
                {
                    return connections.ToList();
                }
            }
        }

        // Completes once this server has read and answered all its clients have sent it: no connection waits to be
        // accepted, and every connection is closed, done, or waiting for its next request with nothing more sent on it.
        // A test that has stopped its client waits for it before counting what arrived, so that a request the client
        // sent before it stopped is counted as sent before, however late this server comes to read it. Two checks in a
        // row must find it so, so that a connection accepted between them is seen.
        public async Task SettledAsync()
        {
            for (int settledChecks = 0; settledChecks < 2; settledChecks = Settled() ? settledChecks + 1 : 0)
            {
                await Task.Delay(10, stopping.Token);
            }
        }

        private bool Settled()
        {
            return !listener.Pending() && Connections.All(connection => connection.Settled);
        }

        public void Dispose()
        {
            stopping.Cancel();
            listener.Stop();
            foreach (ScriptedConnection connection in Connections)
            {
                connection.Dispose();
            }
        }

        private async Task AcceptAsync()
        {
            while (!stopping.IsCancellationRequested)
            {
                TcpClient client;
                try
                {
                    client = await listener.AcceptTcpClientAsync();
                }
                catch (Exception) when (stopping.IsCancellationRequested)
                {
                    return;
                }
                var connection = new ScriptedConnection(client, stopping.Token);
                lock (connections)
                {
                    connections.Add(connection);
                }
                _ = RunAsync(connection);
            }
        }

        private async Task RunAsync(ScriptedConnection connection)
        {
            try
            {
                if (certificateFor != null && !await connection.StartTlsAsync(certificateFor, stopping.Token))
                {
                    return;
                }
                await serve(connection);
            }
            catch (Exception) when (stopping.IsCancellationRequested || connection.Closed.IsCompleted)
            {
                // Stopped by the test, or by the client closing the connection.
            }
            catch (IOException)
            {
                // The client closed the connection while the script wrote to it.
            }
            finally
            {
                connection.Done();
            }
        }
    }

    public sealed class ScriptedConnection : IDisposable
    {
        private readonly TcpClient client;
        private readonly CancellationToken stopping;
        private Stream stream;
        private readonly TaskCompletionSource<bool> closed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> stalling = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly byte[] pending = new byte[64 * 1024];
        private int pendingStart;
        private int pendingEnd;
        // Whether a read waits for the client's next bytes, everything received before having been read, and whether the
        // script for the connection has ended.
        private int waiting;
        private int done;

        // Each write is sent at once, as an HTTP server sends its answers, rather than held by Nagle's algorithm until
        // the client acknowledges the write before it: an answer's head and body are separate writes, and a client
        // waiting for the body may delay that acknowledgement (by 40 ms on Linux) on every answer of a connection.
        internal ScriptedConnection(TcpClient client, CancellationToken stopping)
        {
            this.client = client;
            this.stopping = stopping;
            client.NoDelay = true;
            stream = client.GetStream();
        }

        // Completes when the client has closed the connection: a read found its end, or failed.
        public Task Closed => closed.Task;

        // Whether all the client has sent on this connection has been read and answered: it is closed, its script has
        // ended, or a read waits for more and the client has sent nothing more.
        internal bool Settled => Closed.IsCompleted || Volatile.Read(ref done) == 1 || (Volatile.Read(ref waiting) == 1 && NothingUnread());

        internal void Done()
        {
            Volatile.Write(ref done, 1);
        }

        // Whether the client has sent nothing this server has not read; a socket that cannot say, being reset, is not yet
        // settled: the read waiting on it fails, and closes the connection.
        private bool NothingUnread()
        {
            try
            {
                return client.Available == 0;
            }
            catch (Exception e) when (e is SocketException || e is ObjectDisposedException)
            {
                return false;
            }
        }

        // For HTTPS, the server name the client asked for.
        public string ServerName { get; private set; }

        // Serves the connection over TLS, with the certificate for the name the client asks for; false if the client
        // refuses it.
        internal async Task<bool> StartTlsAsync(Func<string, X509Certificate2> certificateFor, CancellationToken cancellationToken)
        {
            var tls = new SslStream(stream);
            var options = new SslServerAuthenticationOptions
            {
                ServerCertificateSelectionCallback = (sender, name) =>
                {
                    ServerName = name;
                    return certificateFor(name);
                },
            };
            try
            {
                await tls.AuthenticateAsServerAsync(options, cancellationToken);
            }
            catch (Exception) when (!cancellationToken.IsCancellationRequested)
            {
                tls.Dispose();
                return false;
            }
            stream = tls;
            return true;
        }

        public List<string> Requests { get; } = new List<string>();

        // Reads one request's line and headers, such as "GET /file HTTP/1.1", and returns its request line. Throws
        // EndOfStreamException if the client closes the connection first.
        public async Task<string> ReadRequestHeadAsync()
        {
            var head = new StringBuilder();
            while (!head.ToString().EndsWith("\r\n\r\n", StringComparison.Ordinal))
            {
                if (pendingStart == pendingEnd && !await FillAsync())
                {
                    throw new EndOfStreamException("The client closed the connection.");
                }
                head.Append((char)pending[pendingStart++]);
            }
            string[] lines = head.ToString().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            HeaderLines = lines.Skip(1).ToArray();
            ContentLength = lines.Skip(1)
                .Where(line => line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                .Select(line => long.Parse(line.Substring("Content-Length:".Length).Trim(), CultureInfo.InvariantCulture))
                .FirstOrDefault();
            Requests.Add(lines[0]);
            return lines[0];
        }

        // The header lines of the request whose head was read last.
        public string[] HeaderLines { get; private set; } = new string[0];

        // The Content-Length of the request whose head was read last.
        public long ContentLength { get; private set; }

        // Reads the request body, at most chunk bytes (and 64 KiB) at a time, waiting pause between reads, and returns it.
        public async Task<byte[]> ReadBodyAsync(int chunk = 64 * 1024, TimeSpan pause = default)
        {
            var body = new MemoryStream();
            while (body.Length < ContentLength)
            {
                if (pendingStart == pendingEnd && !await FillAsync((int)Math.Min(chunk, ContentLength - body.Length)))
                {
                    throw new EndOfStreamException("The client closed the connection.");
                }
                int count = (int)Math.Min(pendingEnd - pendingStart, ContentLength - body.Length);
                body.Write(pending, pendingStart, count);
                pendingStart += count;
                if (pause > TimeSpan.Zero && body.Length < ContentLength)
                {
                    await Task.Delay(pause, stopping);
                }
            }
            return body.ToArray();
        }

        public async Task SendAsync(string text)
        {
            await SendAsync(Encoding.ASCII.GetBytes(text));
        }

        public async Task SendAsync(byte[] bytes)
        {
            await stream.WriteAsync(bytes, 0, bytes.Length, stopping);
            await stream.FlushAsync(stopping);
        }

        // A response's status line and headers, with the Content-Length of a body of length bytes.
        public Task SendHeadAsync(int status, long length, string headers = "")
        {
            return SendAsync($"HTTP/1.1 {status} {(status < 400 ? "OK" : "Failed")}\r\nContent-Length: {length}\r\nContent-Type: application/json\r\n{headers}\r\n");
        }

        // A whole response with a JSON body.
        public async Task RespondAsync(int status, string json, string headers = "")
        {
            byte[] body = Encoding.UTF8.GetBytes(json);
            await SendHeadAsync(status, body.Length, headers);
            await SendAsync(body);
        }

        // Reads nothing more until StopStalling, like a server that has stopped reading, so that a large request body
        // fills the connection's buffers (kept small by the server's receiveBuffer) and the client's writes cannot
        // complete; then reads until the client closes the connection.
        public async Task StallAsync()
        {
            await Task.WhenAny(stalling.Task, Task.Delay(Timeout.Infinite, stopping));
            await HoldAsync();
        }

        public void StopStalling()
        {
            stalling.TrySetResult(true);
        }

        // Waits until the client closes the connection, or the test stops the server; sends nothing meanwhile.
        public async Task HoldAsync()
        {
            while (await FillAsync())
            {
                pendingStart = pendingEnd;
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }

        // Reads more of the request into pending; false once the client has closed the connection.
        private async Task<bool> FillAsync(int most = 64 * 1024)
        {
            pendingStart = 0;
            Volatile.Write(ref waiting, 1);
            try
            {
                pendingEnd = await stream.ReadAsync(pending, 0, Math.Min(most, pending.Length), stopping);
            }
            catch (Exception) when (!stopping.IsCancellationRequested)
            {
                pendingEnd = 0;
            }
            finally
            {
                Volatile.Write(ref waiting, 0);
            }
            if (pendingEnd == 0)
            {
                closed.TrySetResult(true);
                return false;
            }
            return true;
        }
    }
}