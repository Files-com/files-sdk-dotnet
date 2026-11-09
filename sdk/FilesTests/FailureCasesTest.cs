using FilesCom;
using FilesCom.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FilesTests
{
    /// <summary>
    /// The shared failure-case matrix (the mock server's scenarios/failure-cases.json) through this SDK's public static
    /// <see cref="User.Find"/> and <see cref="User.Update"/>. test.sh runs it alone on each framework through the mock
    /// server's launcher, which resets the simulator to the scenario's users and adds its fault rules; the ordinary run
    /// leaves out its category. The default client sends only to the launcher's simulator, with the definition's
    /// MaxNetworkRetries. Each call's result or exception is compared with the case's expectation: an
    /// <see cref="ApiException"/>'s status, type and message, an <see cref="ApiConnectionException"/> (a request that
    /// got no answer) once the retries are spent, or the user found. FILES_MOCK_OUT/failure-cases.json records each case's outcome, and the launcher's
    /// failure cases check then compares the journal and the fault rules' decisions with each case's expected wire
    /// outcome.
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class FailureCasesTest
    {
        [TestMethod]
        [TestCategory("SharedSimulatorFailureCases")]
        public async Task TestFailureCases()
        {
            string Required(string name) => Environment.GetEnvironmentVariable(name) is string value && value.Length > 0
                ? value : throw new InvalidOperationException($"Run the failure cases through with-scenario.rb; {name} is missing.");
            string origin = Required("FILES_MOCK_SERVER_URL");
            Assert.IsTrue(origin.StartsWith("http://127.0.0.1:", StringComparison.Ordinal), $"{origin} is not the loopback simulator.");
            JsonNode definition = JsonNode.Parse(File.ReadAllText(Required("FILES_MOCK_SCENARIO_FILE")))["failure_cases"];
            Assert.IsNotNull(definition, "FILES_MOCK_SCENARIO_FILE defines no failure_cases.");

            var results = new JsonArray();
            var failures = new List<string>();
            using (new FilesClient(new FilesConfiguration
            {
                ApiKey = Required("FILES_MOCK_API_KEY"),
                BaseUrl = origin,
                MaxNetworkRetries = definition["max_network_retries"].GetValue<int>(),
            }))
            {
                foreach (JsonNode testCase in definition["cases"].AsArray())
                {
                    var problems = new List<string>();
                    int index = 0;
                    foreach (JsonNode expected in testCase["expect"].AsArray())
                    {
                        index++;
                        // As JSON reads it back, so that numbers compare by value whatever type held them.
                        JsonNode actual = JsonNode.Parse((await Call(testCase)).ToJsonString());
                        if (!JsonNode.DeepEquals(actual, expected))
                        {
                            problems.Add($"call {index} gave {actual.ToJsonString()}, not {expected.ToJsonString()}");
                        }
                    }
                    string id = testCase["id"].GetValue<string>();
                    string failure = problems.Count > 0 ? string.Join("; ", problems) : null;
                    results.Add(new JsonObject { ["id"] = id, ["outcome"] = failure == null ? "passed" : "failed", ["failure"] = failure });
                    if (failure != null)
                    {
                        failures.Add($"{id}: {failure}");
                    }
                }
            }

            var report = new JsonObject
            {
                ["format"] = "files-failure-cases/1",
                ["sdk"] = "dotnet",
                ["runtime"] = Environment.Version.ToString(),
                ["cases"] = results,
            };
            File.WriteAllText(Path.Combine(Required("FILES_MOCK_OUT"), "failure-cases.json"),
                report.ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
            Assert.AreEqual(0, failures.Count, string.Join("\n", failures));
        }

        // What one call did, in the expectation's terms.
        private static async Task<JsonNode> Call(JsonNode testCase)
        {
            long userId = testCase["user_id"].GetValue<long>();
            string call = testCase["call"].GetValue<string>();
            try
            {
                if (call == "users.update")
                {
                    var parameters = testCase["params"].AsObject().ToDictionary(pair => pair.Key, pair => (object)pair.Value.GetValue<string>());
                    await User.Update(userId, parameters);
                    return new JsonObject { ["updated"] = true };
                }
                if (call == "users.find")
                {
                    User user = await User.Find(userId);
                    return new JsonObject { ["user"] = new JsonObject { ["id"] = user.Id, ["username"] = user.Username } };
                }
                return new JsonObject { ["unknown call"] = call };
            }
            catch (ApiException error)
            {
                return new JsonObject { ["error"] = new JsonObject { ["status"] = error.httpCode, ["type"] = error.type, ["message"] = error.error } };
            }
            catch (ApiConnectionException error) when (!(error is InvalidResponseException))
            {
                return new JsonObject { ["error"] = new JsonObject { ["network"] = true } };
            }
            catch (Exception error)
            {
                return new JsonObject { ["unexpected"] = $"{error.GetType().Name}: {error.Message}" };
            }
        }
    }
}