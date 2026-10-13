using FilesCom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FilesTests
{
    // The query string of a GET, HEAD or DELETE request: how its parameters are written into the URL.
    [TestClass]
    public class QueryParametersTest
    {
        private static async Task<string> QueryOf(Dictionary<string, object> parameters)
        {
            var handler = new CapturingHttpMessageHandler();
            var service = new FilesApiService(new CapturingHttpClientFactory(handler, "http://example.test"));
            new FilesClient(new FilesConfiguration { ApiKey = "my-key", BaseUrl = "http://example.test" });

            await service.SendRequest("/users", HttpMethod.Get, parameters, new Dictionary<string, object>());

            return Uri.UnescapeDataString(handler.Request.RequestUri.Query);
        }

        // The README shows sort_by and the filters as Dictionary<string, string>; each entry is sent as name[key]=value.
        [TestMethod]
        public async Task ADictionaryOfTextIsSentAsOneQueryEntryPerKey()
        {
            string query = await QueryOf(new Dictionary<string, object>
            {
                { "cursor", "abc" },
                { "sort_by", new Dictionary<string, string> { { "username", "asc" } } },
                { "filter", new Dictionary<string, string> { { "disabled", "false" }, { "company", "Files" } } },
            });

            Assert.AreEqual("?cursor=abc&sort_by[username]=asc&filter[disabled]=false&filter[company]=Files", query);
        }

        // A filter built as a Dictionary<string, object>, as the parameters themselves are, is sent the same way. It
        // used to be sent as the dictionary type's name, so the list came back unfiltered.
        [TestMethod]
        public async Task ADictionaryOfObjectsIsSentTheSameWay()
        {
            string query = await QueryOf(new Dictionary<string, object>
            {
                { "filter", new Dictionary<string, object> { { "disabled", false }, { "group_id", 7L } } },
                { "sort_by", new Dictionary<string, object> { { "username", "desc" } } },
            });

            Assert.AreEqual("?filter[disabled]=False&filter[group_id]=7&sort_by[username]=desc", query);
        }

        // Numbers, booleans and null are written as they always were.
        [TestMethod]
        public async Task ScalarsAreWrittenAsBefore()
        {
            string query = await QueryOf(new Dictionary<string, object>
            {
                { "per_page", 25L },
                { "with_previews", true },
                { "ids", "1,2,3" },
                { "action", null },
            });

            Assert.AreEqual("?per_page=25&with_previews=True&ids=1,2,3&action=", query);
        }

        // A value that cannot be written into a query string in a form Files.com reads is refused, naming the parameter,
        // rather than sent as its type's name.
        [TestMethod]
        public async Task AValueThatCannotBeAQueryEntryIsRefused()
        {
            InvalidParameterException refusal = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => QueryOf(new Dictionary<string, object>
            {
                { "ids", new[] { "1", "2" } },
            }));
            StringAssert.Contains(refusal.Message, "ids");
            StringAssert.Contains(refusal.Message, "cannot be sent in a query string");

            InvalidParameterException nested = await Assert.ThrowsExactlyAsync<InvalidParameterException>(() => QueryOf(new Dictionary<string, object>
            {
                { "filter", new Dictionary<string, object> { { "settings", new Dictionary<string, object>() } } },
            }));
            StringAssert.Contains(nested.Message, "filter[settings]");
        }
    }
}