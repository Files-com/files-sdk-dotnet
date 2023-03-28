using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FilesCom;

namespace FilesTests
{
    public class ExposedFilesApiService : FilesApiService
    {
        private readonly IHttpClientFactory _clientFactory;
        public ExposedFilesApiService(IHttpClientFactory clientFactory) : base (clientFactory)
        {
            // _clientFactory = clientFactory;
        }

        public static string ExposedParsePathParameters(string path, Dictionary<string, object> parameters)
        {
            return ParsePathParameters(path, parameters);
        }
    }

    [TestClass]
    public class FilesApiServiceTest
    {
        [TestMethod]
        public void ParsePathParameters()
        {
            var path = "/users/{id}.json";
            var parameters = new Dictionary<string, object>()
            {
                { "id", 1234 },
            };

            var resultPath = ExposedFilesApiService.ExposedParsePathParameters(path, parameters);

            Assert.AreEqual("/users/1234.json", resultPath);
        }
    }
}
