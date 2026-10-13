using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace FilesTests
{
    [TestClass]
    public class ExceptionsTest
    {
        [TestMethod]
        public void Exception_ToMessage_ShouldUseMessage()
        {
            const string testMessage = "This is only a test";

            ApiConnectionException exception = new ApiConnectionException(message: testMessage);
            Assert.AreEqual(testMessage, exception.ToString());

            InvalidFileTypeException iftException = new InvalidFileTypeException(message: testMessage);
            Assert.AreEqual(testMessage, exception.ToString());
        }

        [TestMethod]
        public void Exception_WhenConstructed_ShouldHaveAllMembersSet()
        {
            const string body = @"{
                ""detail"": ""Error saving model."",
                ""error"": ""Error saving."",
                ""errors"": [
                    ""Username must not contain multiple spaces together"",
                    ""Username must not begin or end with a space""
                ],
                ""http-code"": 422,
                ""instance"": ""14846924-ee4f-45ae-836e-406cad383adb"",
                ""model_errors"": {
                    ""avatar_file"": [],
                    ""username"": [
                        ""Username must not contain multiple spaces together"",
                        ""Username must not begin or end with a space""
                    ]
                },
                ""title"": ""Model save error"",
                ""type"": ""model-save-error""
            }";
            ResponseError error = JsonSerializer.Deserialize<ResponseError>(body);

            HttpResponseMessage message = new HttpResponseMessage();
            HttpHeaders headers = message.Headers;
            headers.Add("TestHeader1", "TestValue1");
            headers.Add("TestHeader2", new string[] { "TestValue2", "TestValue3" });

            ModelSaveErrorException exception = new ModelSaveErrorException("Model Save Error", 400, error, headers);

            Assert.AreEqual("Error saving model.", exception.detail);
            Assert.AreEqual("Error saving.", exception.error);
            Assert.AreEqual("Username must not contain multiple spaces together", exception.errors[0]);
            Assert.AreEqual("Username must not begin or end with a space", exception.errors[1]);
            Assert.AreEqual(422, exception.httpCode);
            Assert.AreEqual("14846924-ee4f-45ae-836e-406cad383adb", exception.instance);
            Assert.AreEqual("Username must not contain multiple spaces together", exception.modelErrors["username"][0]);
            Assert.AreEqual("Username must not begin or end with a space", exception.modelErrors["username"][1]);
            Assert.AreEqual("Model save error", exception.title);
            Assert.AreEqual("model-save-error", exception.type);
            Assert.AreEqual(400, exception.httpStatus);
            Assert.AreEqual("TestValue1", new List<string>(exception.httpHeaders.GetValues("TestHeader1"))[0]);
            Assert.AreEqual("TestValue2", new List<string>(exception.httpHeaders.GetValues("TestHeader2"))[0]);
            Assert.AreEqual("TestValue3", new List<string>(exception.httpHeaders.GetValues("TestHeader2"))[1]);
        }

        [TestMethod]
        public void Exception_Data_ShouldBePopulatedForRegionMismatch()
        {
            const string body = @"{
                ""error"": ""You have connected to a URL that has different security settings than those required for your site."",
                ""http-code"": 403,
                ""title"": ""Lockout Region Mismatch"",
                ""type"": ""not-authenticated/lockout-region-mismatch"",
                ""data"": {
                    ""host"": ""test.host""
                }
            }";
            ResponseError error = JsonSerializer.Deserialize<ResponseError>(body);

            HttpResponseMessage message = new HttpResponseMessage();
            HttpHeaders headers = message.Headers;
            headers.Add("x-files-host", "test.host");

            LockoutRegionMismatchException exception = new LockoutRegionMismatchException("Lockout Region Mismatch Error", 403, error, headers);

            Assert.AreEqual("You have connected to a URL that has different security settings than those required for your site.", exception.error);
            Assert.AreEqual(403, exception.httpCode);
            Assert.AreEqual("Lockout Region Mismatch", exception.title);
            Assert.AreEqual("not-authenticated/lockout-region-mismatch", exception.type);
            Assert.AreEqual(403, exception.httpStatus);
            Assert.AreEqual("test.host", exception.data["host"].ToString());
            Assert.AreEqual("test.host", new List<string>(exception.httpHeaders.GetValues("x-files-host"))[0]);
        }
    }
}