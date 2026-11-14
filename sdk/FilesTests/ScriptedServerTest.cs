using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FilesTests
{
    // The loopback server the transfer tests script (ScriptedServer, and DownloadServer on it) keeps accepting
    // connections whatever its clients do to earlier ones. A client that cancels a request it has just opened a
    // connection for resets that connection, sometimes before the server has set it up; a download that starts over
    // after its file changed then needs new connections, and a server that had stopped accepting would leave them
    // unanswered until the test timed out.
    [TestClass]
    public class ScriptedServerTest
    {
        // Each connection is reset as it closes (linger 0), as a cancelled request's connection is. On macOS, setting
        // up a connection already reset fails, which used to end the server's accept loop.
        [TestMethod]
        [Timeout(30000)]
        public async Task ConnectionsTheClientResetsDoNotStopTheServerAcceptingOthers()
        {
            using (var server = new ScriptedServer(async connection =>
            {
                await connection.ReadRequestHeadAsync();
                await connection.RespondAsync(200, "{}");
            }))
            {
                for (int i = 0; i < 100; i++)
                {
                    using (var client = new TcpClient())
                    {
                        await client.ConnectAsync(IPAddress.Loopback, server.Port);
                        client.LingerState = new LingerOption(true, 0);
                    }
                }

                // A server that stopped accepting would leave this unanswered: the bound turns that into a failure.
                using (var http = new HttpClient(new SocketsHttpHandler { UseProxy = false }) { Timeout = TimeSpan.FromSeconds(10) })
                using (HttpResponseMessage response = await http.GetAsync($"http://127.0.0.1:{server.Port}/"))
                {
                    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }
    }
}