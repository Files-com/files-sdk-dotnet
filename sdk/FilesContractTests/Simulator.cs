using FilesCom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Every case resets the one simulator the run shares, so the cases run one at a time.
[assembly: DoNotParallelize]

namespace FilesContractTests
{
    // The Files.com API simulator the generated contract cases run against (the mock-server target in simulation mode,
    // contract version 3), named by FILES_CONTRACT_MOCK_URL. The cases drive the SDK under test and check what it did
    // independently of its answers: the simulator's journal shows each request the SDK sent (operation, status, the
    // JSON type of every value, its credential and User-Agent), and raw HTTP reads, with no SDK involved, show the
    // records the simulator holds. Raw requests carry no credential and their own User-Agent, so the journal tells
    // them apart from the SDK's.
    //
    // A case judges the SDK only against a simulator that holds up its side. Each case has two phases. In its setup,
    // before the SDK is called, a raw request of the case that the simulator answers 501 simulation/not-supported, or a
    // record that contradicts the types the schema declares, ends the case as not applicable, saying so. Once the case
    // calls the SDK (Sdk), its raw requests verify what the SDK did: the same refusal or contradiction then fails the
    // case, naming the SDK operation and the refusal. What the SDK itself sent or returned is never excused.
    internal static class Simulator
    {
        private const string OracleAgent = "files-contract-oracle";
        private static readonly HttpClient http = new HttpClient(new HttpClientHandler { UseProxy = false, AllowAutoRedirect = false });
        private static readonly Lazy<Task<JsonObject>> inventory = new Lazy<Task<JsonObject>>(() => ControlAsync(HttpMethod.Get, "inventory"));
        private static readonly Lazy<Task<JsonObject>> readiness = new Lazy<Task<JsonObject>>(() => ControlAsync(HttpMethod.Get, "ready"));
        // The SDK operation the running case called, or null while the case is in its setup. The cases run one at a time.
        private static string sdkOperation;

        public static string Url
        {
            get
            {
                string url = Environment.GetEnvironmentVariable("FILES_CONTRACT_MOCK_URL");
                if (string.IsNullOrEmpty(url))
                {
                    throw new InvalidOperationException("FILES_CONTRACT_MOCK_URL must name the simulator these cases run against; contract-test.rb starts one.");
                }
                return url.TrimEnd('/');
            }
        }

        // Starts a case in its setup phase (ContractCase calls it before each case).
        public static void BeginCase()
        {
            sdkOperation = null;
        }

        // A client of the SDK under test for the case's operation (as RecordOperationAsync names it), with one
        // credential and no retries, so that each call is exactly one request. The case leaves its setup here: from
        // now on its raw requests verify what the SDK did.
        public static FilesClient Sdk(string operation)
        {
            sdkOperation = operation;
            FilesConfiguration config = new FilesConfiguration { BaseUrl = Url, ApiKey = "contract-key", MaxNetworkRetries = 0 };
            return FilesClient.Create(config, HttpTransport.FromHandlerFactory(() => new HttpClientHandler { UseProxy = false }));
        }

        // Fails the case as not applicable, before anything is sent, unless the simulator's inventory declares the
        // operation a record operation, or exactly the specialized operation the case was generated to bind (such as
        // users.create, whose owner keeps the record contract): then the operation's contract is the record contract
        // these cases check. Returns the simulator's name for the operation, such as groups.create.
        public static async Task<string> RecordOperationAsync(string model, string method, string specialized = null)
        {
            JsonObject entry = (await inventory.Value)["entries"].AsArray().Select(node => node.AsObject())
                .FirstOrDefault(node => (string)node["model"] == model && (string)node["method"] == method);
            if (entry == null)
            {
                Assert.Fail($"The simulator's inventory has no {model}.{method}: it was generated from another schema than the SDK.");
            }
            string disposition = (string)entry["disposition"];
            bool bound = specialized != null && disposition == "specialized" && (string)entry["simulator_operation"] == specialized;
            if (disposition != "record" && !bound)
            {
                string kind = entry["gap_kind"] is JsonNode gap ? $" ({gap})" : "";
                string reason = entry["reason"] is JsonNode why ? $": {why}" : "";
                Assert.Inconclusive($"not-applicable: the simulator declares {model}.{method} {disposition}{kind}{reason}");
            }
            return (string)entry["simulator_operation"];
        }

        // A reset loading the given fixtures, under the given profile (such as {"file_actions": "pending"}).
        public static async Task ResetAsync(JsonObject fixtures = null, JsonObject profile = null)
        {
            JsonObject body = fixtures == null ? new JsonObject() : new JsonObject { ["fixtures"] = fixtures };
            if (profile != null)
            {
                body["profile"] = profile;
            }
            await ControlAsync(HttpMethod.Post, "reset", body);
        }

        // The records a reset fixture gives a resource, with IDs from 1 in the order given.
        public static Task ResetWithAsync(string resource, params JsonObject[] records)
        {
            return ResetAsync(new JsonObject { [resource] = new JsonArray(records.Select(record => (JsonNode)record.DeepClone()).ToArray()) });
        }

        // Creates a record with a raw request, as the case's fixture, and returns the simulator's answer.
        public static async Task<JsonObject> CreateRawAsync(string path, JsonObject body)
        {
            (int status, JsonNode answer) = await ApiAsync(HttpMethod.Post, path, body);
            if (status / 100 != 2 || answer is not JsonObject record)
            {
                throw new InvalidOperationException($"The fixture's create of {path} got {status}: {answer?.ToJsonString()}");
            }
            return record;
        }

        // A raw API request: a JSON body for POST and PATCH, the query string otherwise.
        public static async Task<(int Status, JsonNode Body)> ApiAsync(HttpMethod method, string path, JsonObject parameters = null)
        {
            string url = $"{Url}/api/rest/v1{path}";
            using HttpRequestMessage request = new HttpRequestMessage(method, url);
            if (parameters != null && (method == HttpMethod.Post || method == HttpMethod.Patch))
            {
                request.Content = new StringContent(parameters.ToJsonString(), Encoding.UTF8, "application/json");
            }
            else if (parameters != null && parameters.Count > 0)
            {
                request.RequestUri = new Uri(url + "?" + string.Join("&", parameters.Select(pair => $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(QueryValue(pair.Value))}")));
            }
            request.Headers.UserAgent.ParseAdd(OracleAgent);
            using HttpResponseMessage response = await http.SendAsync(request);
            string text = await response.Content.ReadAsStringAsync();
            JsonNode body = text.Length == 0 ? null : JsonNode.Parse(text);
            if ((int)response.StatusCode == 501 && body is JsonObject refusal && (string)refusal["type"] == "simulation/not-supported")
            {
                Unserved($"the simulator refuses the case's raw {method} {path} (501 simulation/not-supported): {refusal["error"]}");
            }
            return ((int)response.StatusCode, body);
        }

        // The simulator cannot serve what the case asked of it. In the case's setup, the case does not apply; once the
        // SDK was called, its verification failed.
        private static void Unserved(string why)
        {
            if (sdkOperation != null)
            {
                Assert.Fail($"after the SDK's {sdkOperation}, verification failed: {why}");
            }
            Assert.Inconclusive($"not-applicable: in the case's setup, before the SDK was called, {why}");
        }

        // Ends the case as not applicable, in its setup, when a record the simulator holds contradicts a type the schema
        // declares for it, such as an integer id where the schema declares a string: then no SDK answer could match it.
        // After the SDK was called, the contradiction fails the case.
        public static void Conforms(JsonObject record, string what, params (string Field, string Type)[] declared)
        {
            foreach ((string field, string type) in declared)
            {
                if (!record.TryGetPropertyValue(field, out JsonNode value) || value == null)
                {
                    continue;
                }
                JsonValueKind kind = value.GetValueKind();
                bool conforms = type switch
                {
                    "string" or "email" or "date-time" or "date" => kind == JsonValueKind.String,
                    "int64" or "double" => kind == JsonValueKind.Number,
                    "decimal" => kind is JsonValueKind.String or JsonValueKind.Number,
                    "boolean" => kind is JsonValueKind.True or JsonValueKind.False,
                    _ when type.StartsWith("array(", StringComparison.Ordinal) => kind == JsonValueKind.Array,
                    _ => true,
                };
                if (!conforms)
                {
                    Unserved($"the simulator's {what} record contradicts the schema: {field} is {value.ToJsonString()}, declared {type}");
                }
            }
        }

        // The record a raw read finds: with the member path when the resource has one, else in the raw list.
        public static async Task<JsonObject> ReadRawAsync(string listPath, string memberPath, JsonObject scope, Func<JsonObject, bool> match)
        {
            if (memberPath != null)
            {
                (int status, JsonNode body) = await ApiAsync(HttpMethod.Get, memberPath, scope);
                Assert.AreEqual(200, status, $"raw read of {memberPath}: {body?.ToJsonString()}");
                return body.AsObject();
            }
            JsonObject[] records = await ListRawAsync(listPath, scope);
            JsonObject[] found = records.Where(match).ToArray();
            Assert.AreEqual(1, found.Length, $"raw list of {listPath} holds the record once: {new JsonArray(records.Select(record => record.DeepClone()).ToArray()).ToJsonString()}");
            return found[0];
        }

        public static async Task<JsonObject[]> ListRawAsync(string path, JsonObject scope)
        {
            (int status, JsonNode body) = await ApiAsync(HttpMethod.Get, path, scope);
            Assert.AreEqual(200, status, $"raw list of {path}: {body?.ToJsonString()}");
            return body.AsArray().Select(node => node.AsObject()).ToArray();
        }

        // The one request the SDK sent since the reset (the raw oracle's own requests aside): its operation and
        // status, the JSON type of each value it sent (a query value is always a string), its credential, and the
        // SDK's User-Agent.
        public static async Task<JsonObject> SdkRequestAsync(string operation, int status, JsonObject wire)
        {
            JsonObject journal = await ControlAsync(HttpMethod.Get, "journal");
            Assert.AreEqual(true, (bool)journal["complete"], "the journal kept every request");
            JsonObject[] sent = journal["entries"].AsArray().Select(node => node.AsObject())
                .Where(entry => (string)entry["user_agent"] != OracleAgent).ToArray();
            Assert.AreEqual(1, sent.Length, $"the SDK sent one request: {new JsonArray(sent.Select(entry => entry.DeepClone()).ToArray()).ToJsonString()}");
            JsonObject request = sent[0];
            Assert.AreEqual(operation, (string)request["operation"], "the operation the SDK's request reached");
            Assert.AreEqual(status, (int)request["status"], $"the status of {operation}");
            Assert.IsTrue(JsonNode.DeepEquals(wire, request["wire"] ?? new JsonObject()), $"the JSON type of each value {operation} sent: expected {wire.ToJsonString()}, sent {request["wire"]?.ToJsonString()}");
            Assert.IsTrue(JsonNode.DeepEquals(new JsonObject { ["api_key"] = 1 }, request["credentials"]), $"{operation} carried the client's API key: {request["credentials"]?.ToJsonString()}");
            Assert.IsTrue(Regex.IsMatch((string)request["user_agent"] ?? "", @"^Files\.com DOTNET SDK v\S+$"), $"{operation} carried the SDK's User-Agent: {request["user_agent"]}");
            return request;
        }

        // The values a case sends, as JSON: what the raw oracle sends and what the simulator keeps.
        public static JsonObject Json(Dictionary<string, object> values)
        {
            JsonObject json = new JsonObject();
            foreach (KeyValuePair<string, object> pair in values)
            {
                json[pair.Key] = ToJson(pair.Value);
            }
            return json;
        }

        // Each value a case sent for one of the record's fields, as the simulator keeps it (write-only values, which it
        // never returns, aside).
        public static async Task KeptAsync(Dictionary<string, object> values, JsonObject record, string what, params string[] fields)
        {
            Regex writeOnly = new Regex($"^(?:{(string)(await readiness.Value)["records"]["write_only"]})$");
            JsonObject sent = Json(values);
            foreach (KeyValuePair<string, JsonNode> pair in sent.Where(pair => fields.Contains(pair.Key)))
            {
                if (writeOnly.IsMatch(pair.Key))
                {
                    Assert.IsFalse(record.ContainsKey(pair.Key), $"{what}: the write-only {pair.Key} is never returned");
                    continue;
                }
                Assert.IsTrue(record.ContainsKey(pair.Key) && JsonNode.DeepEquals(pair.Value, record[pair.Key]), $"{what}: {pair.Key} kept as sent, {pair.Value?.ToJsonString()}: {record[pair.Key]?.ToJsonString() ?? "absent"}");
            }
        }

        // A model property's value against the field of the record the raw oracle read: equal when the field is
        // there, and the property's empty value (null, false or an empty array) when it is not.
        public static void Same(JsonObject record, string field, object actual, string what)
        {
            if (!record.TryGetPropertyValue(field, out JsonNode expected))
            {
                bool empty = actual == null || actual is false || (actual is Array array && array.Length == 0);
                Assert.IsTrue(empty, $"{what}: the record has no {field}, but the model has {Show(actual)}");
                return;
            }
            Assert.IsTrue(Equal(expected, actual), $"{what}: {field} is {expected?.ToJsonString() ?? "null"} in the record, {Show(actual)} in the model");
        }

        private static bool Equal(JsonNode expected, object actual)
        {
            if (expected == null)
            {
                return actual == null || actual is false || (actual is Array empty && empty.Length == 0);
            }
            switch (actual)
            {
                case null:
                    return false;
                case string text:
                    return expected.GetValueKind() == JsonValueKind.String && (string)expected == text;
                case bool flag:
                    return expected.GetValueKind() is JsonValueKind.True or JsonValueKind.False && (bool)expected == flag;
                case long number:
                    return expected.GetValueKind() == JsonValueKind.Number && expected.GetValue<decimal>() == number;
                case double number:
                    return expected.GetValueKind() == JsonValueKind.Number && expected.GetValue<double>() == number;
                case decimal number:
                    return decimal.TryParse(expected.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value) && value == number;
                case DateTime time:
                    return expected.GetValueKind() == JsonValueKind.String
                        && DateTime.TryParse((string)expected, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime instant)
                        && instant.ToUniversalTime() == time.ToUniversalTime();
                case Array items:
                    return expected is JsonArray values && values.Count == items.Length && values.Select((value, index) => Equal(value, items.GetValue(index))).All(same => same);
                default:
                    Assert.Fail($"No comparison for a model value of type {actual.GetType()}");
                    return false;
            }
        }

        private static JsonNode ToJson(object value)
        {
            switch (value)
            {
                case null:
                    return null;
                case string text:
                    return JsonValue.Create(text);
                case bool flag:
                    return JsonValue.Create(flag);
                case long number:
                    return JsonValue.Create(number);
                case double number:
                    return JsonValue.Create(number);
                case IDictionary<string, object> map:
                    return Json(new Dictionary<string, object>(map));
                case IEnumerable items:
                    return new JsonArray(items.Cast<object>().Select(ToJson).ToArray());
                default:
                    throw new ArgumentException($"No JSON form for a {value.GetType()}");
            }
        }

        private static string QueryValue(JsonNode value)
        {
            return value is JsonValue scalar && scalar.GetValueKind() == JsonValueKind.String ? (string)scalar : value?.ToJsonString() ?? "";
        }

        private static string Show(object value)
        {
            return value is Array array ? $"[{string.Join(", ", array.Cast<object>())}]" : value?.ToString() ?? "null";
        }

        private static async Task<JsonObject> ControlAsync(HttpMethod method, string name, JsonObject body = null)
        {
            using HttpRequestMessage request = new HttpRequestMessage(method, $"{Url}/__files_mock/v1/{name}");
            if (body != null)
            {
                request.Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json");
            }
            using HttpResponseMessage response = await http.SendAsync(request);
            string text = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"The simulator's {name} answered {(int)response.StatusCode}: {text}");
            }
            return JsonNode.Parse(text).AsObject();
        }
    }

    // The base of every contract case class: MSTest starts each case in its setup phase, whatever ran before it.
    public abstract class ContractCase
    {
        [TestInitialize]
        public void BeginCase()
        {
            Simulator.BeginCase();
        }
    }
}