using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace FilesTests.Models;

[TestClass]
public class ActionTests
{
    private readonly string actionJson = @"
 {
        ""id"": 1,
        ""when"": ""2024-11-14T14:24:29-05:00"",
        ""ip"": ""142.250.113.102"",
        ""targets"": {
            ""user"": {
                ""id"": 1,
                ""username"": ""UserName2""
            }
        },
        ""user_id"": 1,
        ""username"": ""UserName"",
        ""user_is_from_parent_site"": true,
        ""action"": ""user_update"",
        ""interface"": ""web""
    }
";

    [TestMethod]
    public void Assert_DeserializeAction()
    {
        var action = System.Text.Json.JsonSerializer.Deserialize<FilesCom.Models.Action>(actionJson);

        Assert.IsNotNull(action);
    }

}
