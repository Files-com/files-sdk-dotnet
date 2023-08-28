using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using FilesCom;
using FilesCom.Models;

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
            Assert.AreEqual(exception.ToString(), testMessage);

            InvalidFileTypeException iftException = new InvalidFileTypeException(message: testMessage);
            Assert.AreEqual(exception.ToString(), testMessage);
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

            Assert.AreEqual(exception.detail, "Error saving model.");
            Assert.AreEqual(exception.error, "Error saving.");
            Assert.AreEqual(exception.errors[0], "Username must not contain multiple spaces together");
            Assert.AreEqual(exception.errors[1], "Username must not begin or end with a space");
            Assert.AreEqual(exception.httpCode, 422);
            Assert.AreEqual(exception.instance, "14846924-ee4f-45ae-836e-406cad383adb");
            Assert.AreEqual(exception.modelErrors["username"][0], "Username must not contain multiple spaces together");
            Assert.AreEqual(exception.modelErrors["username"][1], "Username must not begin or end with a space");
            Assert.AreEqual(exception.title, "Model save error");
            Assert.AreEqual(exception.type, "model-save-error");
            Assert.AreEqual(exception.httpStatus, 400);
            Assert.AreEqual(new List<string>(exception.httpHeaders.GetValues("TestHeader1"))[0], "TestValue1");
            Assert.AreEqual(new List<string>(exception.httpHeaders.GetValues("TestHeader2"))[0], "TestValue2");
            Assert.AreEqual(new List<string>(exception.httpHeaders.GetValues("TestHeader2"))[1], "TestValue3");
        }
    }
}