using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FilesCom;

namespace FilesTests
{
    [TestClass]
    public class FilesApiTest
    {
        [TestMethod]
        public void ParsePathParameters()
        {
            var path = "/users/{id}.json";
            var parameters = new Dictionary<string, object>()
            {
                { "id", 1234 },
            };

            var resultPath = FilesApi.ParsePathParameters(path, parameters);

            Assert.AreEqual("/users/1234.json", resultPath);
        }
    }
}
