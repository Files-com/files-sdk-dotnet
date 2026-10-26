using FilesCom;
using FilesCom.Transfers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

// Each case runs against a simulator the launcher started and reset for it alone, and the controls share stubs: one test
// at a time.
[assembly: DoNotParallelize]

namespace FilesApiScenarioTests
{
    // The bounds selected for one case: its scenario's work, then a cleanup with a token of its own, both inside the
    // consumer's whole budget, which also covers reading the journal and writing the receipt, and which itself ends inside
    // the launcher's 210 seconds (api-scenarios.rb). They bound this harness; they are no product settings and no
    // measurement.
    internal static class ScenarioBudget
    {
        internal static readonly TimeSpan Work = TimeSpan.FromSeconds(175);
        internal static readonly TimeSpan Cleanup = TimeSpan.FromSeconds(30);
        internal static readonly TimeSpan Consumer = TimeSpan.FromSeconds(205);

        // The lookups that confirm the owned folder absent after an early stop: namespace.close's.
        internal const int AbsenceAttempts = 5;
        internal static readonly TimeSpan AbsenceInterval = TimeSpan.FromSeconds(1);
    }

    // A fixture's bytes, as the definition's recipe gives them: (index * Multiplier + Offset) % Modulus for each index
    // below Length, whose SHA-256 the definition records.
    public sealed class ScenarioRecipe
    {
        public ScenarioRecipe(int number, long multiplier, long offset, long modulus, int length, string sha256)
        {
            Number = number;
            Multiplier = multiplier;
            Offset = offset;
            Modulus = modulus;
            Length = length;
            Sha256 = sha256;
        }

        public int Number { get; }

        public long Multiplier { get; }

        public long Offset { get; }

        public long Modulus { get; }

        public int Length { get; }

        public string Sha256 { get; }

        public byte[] Bytes()
        {
            byte[] bytes = new byte[Length];
            for (int index = 0; index < Length; index++)
            {
                bytes[index] = (byte)((index * Multiplier + Offset) % Modulus);
            }
            return bytes;
        }
    }

    // A path the case owns, relative to its owned folder, whose own path is "".
    public sealed class ScenarioFixture
    {
        public ScenarioFixture(string name, string type, string path, ScenarioRecipe recipe)
        {
            Name = name;
            Type = type;
            Path = path;
            Recipe = recipe;
        }

        public string Name { get; }

        public string Type { get; }

        public string Path { get; }

        public ScenarioRecipe Recipe { get; }

        public bool IsDirectory => Type == "directory";

        public bool IsNamespace => Path.Length == 0;
    }

    // A field of a returned entry and the definition's matcher for it ({"one_of": [...]}, {"includes": [...]} or
    // {"not": value}).
    public sealed class FieldExpectation
    {
        public FieldExpectation(string field, string matcher)
        {
            Field = field;
            Matcher = JsonNode.Parse(matcher).AsObject();
        }

        public string Field { get; }

        public JsonObject Matcher { get; }
    }

    // A returned entry: a fixture's own path, type and size, or a type and the paths allowed, with each field's matcher.
    public sealed class EntryExpectation
    {
        public EntryExpectation(string fixture, string type, string[] pathOneOf, FieldExpectation[] fields)
        {
            Fixture = fixture;
            Type = type;
            PathOneOf = pathOneOf;
            Fields = fields ?? Array.Empty<FieldExpectation>();
        }

        public string Fixture { get; }

        public string Type { get; }

        public string[] PathOneOf { get; }

        public FieldExpectation[] Fields { get; }
    }

    // Every page at most PerPage long, at most MaxPages pages (and MaxRecords records), the last with no next page, holding
    // each of Entries (fixtures) exactly once and nothing else; with no Entries, the pages but not their members.
    public sealed class ListingExpectation
    {
        public ListingExpectation(string[] entries, int perPage, int maxPages, int? maxRecords)
        {
            Entries = entries;
            PerPage = perPage;
            MaxPages = maxPages;
            MaxRecords = maxRecords;
        }

        public string[] Entries { get; }

        public int PerPage { get; }

        public int MaxPages { get; }

        public int? MaxRecords { get; }
    }

    // What an operation step must be answered with: a success status, with its entry or listing, or an error (status and
    // type), looked up Attempts times IntervalSeconds apart until it answers.
    public sealed class ScenarioExpectation
    {
        public ScenarioExpectation(int status, string error, int? attempts, int? intervalSeconds, EntryExpectation entry, ListingExpectation listing)
        {
            Status = status;
            Error = error;
            Attempts = attempts;
            IntervalSeconds = intervalSeconds;
            Entry = entry;
            Listing = listing;
        }

        public int Status { get; }

        public string Error { get; }

        public int? Attempts { get; }

        public int? IntervalSeconds { get; }

        public EntryExpectation Entry { get; }

        public ListingExpectation Listing { get; }
    }

    // An entry the SDK returned, read from its model's properties.
    public sealed class ScenarioEntry
    {
        public ScenarioEntry(object path, object type, object size, params (string Field, object Value)[] fields)
        {
            Path = path as string;
            Type = type as string;
            Size = size == null ? (long?)null : Convert.ToInt64(size, CultureInfo.InvariantCulture);
            Fields = fields.ToDictionary(field => field.Field, field => field.Value);
        }

        public string Path { get; }

        public string Type { get; }

        public long? Size { get; }

        public IReadOnlyDictionary<string, object> Fields { get; }

        // The SDK's own object the entry was read from, kept for a listing's entries: a listed-entry download uses it.
        public object Source { get; internal set; }
    }

    // The pages of a bounded listing, and whether its last page had no next page.
    public sealed class ListedPages
    {
        public ListedPages(IReadOnlyList<IReadOnlyList<ScenarioEntry>> pages, bool complete)
        {
            Pages = pages;
            Complete = complete;
        }

        public IReadOnlyList<IReadOnlyList<ScenarioEntry>> Pages { get; }

        public bool Complete { get; }
    }

    // What an operation step's call returned: an entry, a listing or nothing.
    public sealed class StepResult
    {
        private StepResult(ScenarioEntry entry, ListedPages listing)
        {
            Entry = entry;
            Listing = listing;
        }

        public static StepResult None { get; } = new StepResult(null, null);

        public ScenarioEntry Entry { get; }

        public ListedPages Listing { get; }

        public static StepResult Of(ScenarioEntry entry)
        {
            return new StepResult(entry, null);
        }

        public static StepResult Of(ListedPages listing)
        {
            return new StepResult(null, listing);
        }
    }

    // One expanded position of a scenario.
    public abstract class ScenarioStep
    {
        protected ScenarioStep(int number, string label, string backend)
        {
            Number = number;
            Label = label;
            Backend = backend;
        }

        public int Number { get; }

        public string Label { get; }

        public string Backend { get; }

        public bool AppliesToSimulator => Backend == null || Backend == "simulator";

        // The API routes ("METHOD /path/{name}", as the schema writes them) the position sends: its operation's, or those
        // the definition says its intent relies on. Binding requires the simulator to list each.
        public virtual IReadOnlyList<string> OperationRoutes => Array.Empty<string>();

        internal abstract Task RunAsync(ScenarioRun run, CancellationToken token);
    }

    // A position only a Files.com site can carry out: excluded here, never credited.
    public sealed class ExcludedStep : ScenarioStep
    {
        public ExcludedStep(int number, string label, string backend) : base(number, label, backend)
        {
        }

        internal override Task RunAsync(ScenarioRun run, CancellationToken token)
        {
            throw new InvalidOperationException($"position {Number} applies to {Backend} only");
        }
    }

    // A call of one schema operation through the client, and what it must be answered with. Route is the operation's
    // ("METHOD /path/{name}", as the schema writes it), and PathValues gives, for a run, the values its path parameters
    // must have in every request the call sends; Target is the fixture its path names, if any; CreatesNamespace marks the
    // create that, answered 201 with exactly the owned folder, gives the case its folder.
    public sealed class OperationStep : ScenarioStep
    {
        public OperationStep(int number, string label, string backend, string route, Func<ScenarioRun, IReadOnlyDictionary<string, string>> pathValues, string target, bool createsNamespace,
            ScenarioExpectation expect, Func<ScenarioRun, CancellationToken, Task<StepResult>> call)
            : base(number, label, backend)
        {
            Route = route;
            PathValues = pathValues;
            Target = target;
            CreatesNamespace = createsNamespace;
            Expect = expect;
            Call = call;
        }

        public string Route { get; }

        public Func<ScenarioRun, IReadOnlyDictionary<string, string>> PathValues { get; }

        public string Target { get; }

        public bool CreatesNamespace { get; }

        public ScenarioExpectation Expect { get; }

        public Func<ScenarioRun, CancellationToken, Task<StepResult>> Call { get; }

        public override IReadOnlyList<string> OperationRoutes => new[] { Route };

        internal override Task RunAsync(ScenarioRun run, CancellationToken token)
        {
            return run.RunOperationAsync(this, token);
        }
    }

    // An upload of a fixture's recipe bytes from a local file, a seekable stream or a forward-only stream, completed, or
    // cancelled at its first acknowledged part. BeginRoute and CompletionRoute are the operations the definition says an
    // upload relies on: the begin_upload that starts it and grants its parts, and the request that completes it.
    public sealed class UploadStep : ScenarioStep
    {
        public UploadStep(int number, string label, string backend, string fixture, string source, int? maxRead, bool cancelAfterFirstAcknowledgedPart, string providedMtime, bool finalProgress, int minParts,
            string beginRoute, string completionRoute)
            : base(number, label, backend)
        {
            Fixture = fixture;
            Source = source;
            MaxRead = maxRead;
            CancelAfterFirstAcknowledgedPart = cancelAfterFirstAcknowledgedPart;
            ProvidedMtime = providedMtime;
            FinalProgress = finalProgress;
            MinParts = minParts;
            BeginRoute = beginRoute;
            CompletionRoute = completionRoute;
        }

        public string Fixture { get; }

        public string Source { get; }

        public int? MaxRead { get; }

        public bool CancelAfterFirstAcknowledgedPart { get; }

        public string ProvidedMtime { get; }

        public bool FinalProgress { get; }

        public int MinParts { get; }

        public string BeginRoute { get; }

        public string CompletionRoute { get; }

        public override IReadOnlyList<string> OperationRoutes => new[] { BeginRoute, CompletionRoute };

        internal override Task RunAsync(ScenarioRun run, CancellationToken token)
        {
            return run.UploadAsync(this, token);
        }
    }

    // A download of a fixture, by its path or through the entry a listing returned for it, which must send the first of
    // the operations the definition says a download relies on.
    public sealed class DownloadStep : ScenarioStep
    {
        private readonly string[] routes;

        public DownloadStep(int number, string label, string backend, string fixture, bool listedEntry, string[] operationRoutes) : base(number, label, backend)
        {
            Fixture = fixture;
            ListedEntry = listedEntry;
            routes = operationRoutes;
        }

        public string Fixture { get; }

        public bool ListedEntry { get; }

        public override IReadOnlyList<string> OperationRoutes => routes;

        internal override Task RunAsync(ScenarioRun run, CancellationToken token)
        {
            return run.DownloadAsync(this, token);
        }
    }

    // A copy or move of From to Fixture, started with the schema's operation, the first of those the definition says it
    // relies on, and waited for with FileAction.WaitAsync.
    public sealed class FileActionStep : ScenarioStep
    {
        private readonly string[] routes;

        public FileActionStep(int number, string label, string backend, string action, string from, string fixture, string[] operationRoutes,
            Func<ScenarioRun, CancellationToken, Task<FilesCom.Models.FileAction>> start)
            : base(number, label, backend)
        {
            Action = action;
            From = from;
            Fixture = fixture;
            routes = operationRoutes;
            Start = start;
        }

        public string Action { get; }

        public string From { get; }

        public string Fixture { get; }

        public Func<ScenarioRun, CancellationToken, Task<FilesCom.Models.FileAction>> Start { get; }

        public override IReadOnlyList<string> OperationRoutes => routes;

        internal override Task RunAsync(ScenarioRun run, CancellationToken token)
        {
            return run.FileActionAsync(this, token);
        }
    }

    // A file of a directory transfer's tree: its path relative to the tree's folder, with / between names, and the fixture
    // whose recipe it holds.
    public sealed class TreeFile
    {
        public TreeFile(string relativePath, string fixture)
        {
            RelativePath = relativePath;
            Fixture = fixture;
        }

        public string RelativePath { get; }

        public string Fixture { get; }
    }

    // A local tree uploaded with Transfers.UploadDirectoryAsync to a fixture's folder. It must send the first of the
    // operations the definition says it relies on, within that folder, and complete each file with CompletionRoute.
    public sealed class UploadDirectoryStep : ScenarioStep
    {
        private readonly string[] routes;

        public UploadDirectoryStep(int number, string label, string backend, string fixture, TreeFile[] files, string[] directories, string[] emptyDirectories, string[] completed, int fileCount, int emptyFolderCount, long bytes,
            string[] operationRoutes, string completionRoute)
            : base(number, label, backend)
        {
            Fixture = fixture;
            Files = files;
            Directories = directories;
            EmptyDirectories = emptyDirectories;
            Completed = completed;
            FileCount = fileCount;
            EmptyFolderCount = emptyFolderCount;
            Bytes = bytes;
            routes = operationRoutes;
            CompletionRoute = completionRoute;
        }

        public string Fixture { get; }

        public TreeFile[] Files { get; }

        public string[] Directories { get; }

        public string[] EmptyDirectories { get; }

        public string[] Completed { get; }

        public int FileCount { get; }

        public int EmptyFolderCount { get; }

        public long Bytes { get; }

        public string CompletionRoute { get; }

        public override IReadOnlyList<string> OperationRoutes => routes;

        internal override Task RunAsync(ScenarioRun run, CancellationToken token)
        {
            return run.UploadDirectoryAsync(this, token);
        }
    }

    // A fixture's folder downloaded with Transfers.DownloadDirectoryAsync to a new local directory, which must then hold
    // exactly the folders, empty folders and files the definition's fixtures below that folder are, by paths relative to
    // it. It must send the first of the operations the definition says it relies on, within that folder.
    public sealed class DownloadDirectoryStep : ScenarioStep
    {
        private readonly string[] routes;

        public DownloadDirectoryStep(int number, string label, string backend, string fixture, int fileCount, int emptyFolderCount, long bytes, string[] directories, string[] emptyDirectories, string[] files,
            TreeFile[] bytesMatch, string[] operationRoutes)
            : base(number, label, backend)
        {
            Fixture = fixture;
            FileCount = fileCount;
            EmptyFolderCount = emptyFolderCount;
            Bytes = bytes;
            Directories = directories;
            EmptyDirectories = emptyDirectories;
            Files = files;
            BytesMatch = bytesMatch;
            routes = operationRoutes;
        }

        public string Fixture { get; }

        public int FileCount { get; }

        public int EmptyFolderCount { get; }

        public long Bytes { get; }

        public string[] Directories { get; }

        public string[] EmptyDirectories { get; }

        public string[] Files { get; }

        public TreeFile[] BytesMatch { get; }

        public override IReadOnlyList<string> OperationRoutes => routes;

        internal override Task RunAsync(ScenarioRun run, CancellationToken token)
        {
            return run.DownloadDirectoryAsync(this, token);
        }
    }

    // One scenario of the definition, as generated: its fixtures, every expanded position, and the two calls an early stop
    // cleans up with (a recursive delete of the owned folder and the lookup that confirms it absent), with their routes.
    public sealed class ScenarioCase
    {
        public ScenarioCase(string id, string test, string definitionSha256, ScenarioFixture[] fixtures, ScenarioStep[] steps,
            string deleteRoute, Func<ScenarioRun, CancellationToken, Task> deleteNamespaceRecursively, string lookupRoute, Func<ScenarioRun, CancellationToken, Task> lookUpNamespace)
        {
            Id = id;
            Test = test;
            DefinitionSha256 = definitionSha256;
            Fixtures = fixtures;
            Steps = steps;
            DeleteRoute = deleteRoute;
            DeleteNamespaceRecursively = deleteNamespaceRecursively;
            LookupRoute = lookupRoute;
            LookUpNamespace = lookUpNamespace;
        }

        public string Id { get; }

        public string Test { get; }

        public string DefinitionSha256 { get; }

        public IReadOnlyList<ScenarioFixture> Fixtures { get; }

        public IReadOnlyList<ScenarioStep> Steps { get; }

        public string DeleteRoute { get; }

        public Func<ScenarioRun, CancellationToken, Task> DeleteNamespaceRecursively { get; }

        public string LookupRoute { get; }

        public Func<ScenarioRun, CancellationToken, Task> LookUpNamespace { get; }

        public int SimulatorPositions => Steps.Count(step => step.AppliesToSimulator);
    }

    // Where a case runs: the simulator's address and credential, its control endpoints, its receipts, and the network
    // handler the SDK's own transport ends in.
    public interface IScenarioEnvironment
    {
        string BaseUrl { get; }

        string ApiKey { get; }

        string RunId { get; }

        // The case the runner started this process for, or null for any.
        string ExpectedCase { get; }

        // Where the case's receipt goes, or null for none.
        string ReceiptPath { get; }

        // Everything the case binds itself to except the live readiness and journal.
        BindingInputs ReadReceipts();

        Task<JsonObject> ControlAsync(string name, CancellationToken token);

        HttpMessageHandler CreateNetworkHandler();
    }

    // What a case binds itself to before its first request.
    public sealed class BindingInputs
    {
        // The generated manifest (api-scenarios.json).
        public JsonObject Manifest { get; set; }

        // The readiness the launcher accepted (FILES_MOCK_OUT/ready.json) and the readiness the simulator answers now.
        public JsonObject ReadyReceipt { get; set; }

        public JsonObject Ready { get; set; }

        // The simulator's answer to the launcher's reset (FILES_MOCK_OUT/reset.json), with its epoch.
        public JsonObject Reset { get; set; }

        // The scenario the launcher loaded, by its name and its file.
        public string ScenarioName { get; set; }

        public JsonObject Scenario { get; set; }

        // The simulator's generation receipt (lib/simulation/generation.json).
        public JsonObject Generation { get; set; }

        // The instance the launcher gave the server, and the schema subset it expects.
        public string Instance { get; set; }

        public string SchemaSha256 { get; set; }

        // The journal before the case's first request.
        public JsonObject Journal { get; set; }

        public List<string> Unreadable { get; } = new List<string>();
    }

    // The simulator that ruby with-scenario.rb started for this process, from what it put in the environment
    // (FILES_MOCK_*), and what api-scenarios.rb adds: the case it runs (FILES_API_SCENARIOS_CASE), where its receipt goes
    // (FILES_API_SCENARIOS_RECEIPT) and the simulator's generation receipt (FILES_API_SCENARIOS_GENERATION).
    internal sealed class LauncherEnvironment : IScenarioEnvironment
    {
        private static readonly HttpClient control = new HttpClient(new HttpClientHandler { UseProxy = false, AllowAutoRedirect = false }) { Timeout = TimeSpan.FromSeconds(30) };

        private LauncherEnvironment(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; }

        public string ApiKey => Variable("FILES_MOCK_API_KEY");

        public string RunId => Variable("FILES_MOCK_RUN_ID");

        public string ExpectedCase => Variable("FILES_API_SCENARIOS_CASE");

        public string ReceiptPath => Variable("FILES_API_SCENARIOS_RECEIPT");

        // Null when no launcher started this process.
        internal static LauncherEnvironment FromProcess()
        {
            string url = Variable("FILES_MOCK_SERVER_URL");
            return url == null ? null : new LauncherEnvironment(url.TrimEnd('/'));
        }

        public BindingInputs ReadReceipts()
        {
            var inputs = new BindingInputs();
            string output = Variable("FILES_MOCK_OUT");
            inputs.Manifest = ReadObject(System.IO.Path.Combine(AppContext.BaseDirectory, "api-scenarios.json"), "the generated manifest", inputs.Unreadable);
            inputs.ReadyReceipt = ReadObject(output == null ? null : System.IO.Path.Combine(output, "ready.json"), "the readiness the launcher accepted", inputs.Unreadable);
            inputs.Reset = ReadObject(output == null ? null : System.IO.Path.Combine(output, "reset.json"), "the reset's answer", inputs.Unreadable);
            inputs.ScenarioName = Variable("FILES_MOCK_SCENARIO");
            inputs.Scenario = ReadObject(Variable("FILES_MOCK_SCENARIO_FILE"), "the scenario file", inputs.Unreadable);
            inputs.Generation = ReadObject(Variable("FILES_API_SCENARIOS_GENERATION"), "the simulator's generation receipt", inputs.Unreadable);
            inputs.Instance = Variable("FILES_MOCK_INSTANCE");
            inputs.SchemaSha256 = Variable("FILES_MOCK_SCHEMA_SHA256");
            return inputs;
        }

        public async Task<JsonObject> ControlAsync(string name, CancellationToken token)
        {
            using HttpResponseMessage response = await control.GetAsync($"{BaseUrl}/__files_mock/v1/{name}", token);
            string text = await response.Content.ReadAsStringAsync(token);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"the simulator's {name} answered {(int)response.StatusCode}");
            }
            return JsonNode.Parse(text) as JsonObject ?? throw new InvalidOperationException($"the simulator's {name} is not a JSON object");
        }

        public HttpMessageHandler CreateNetworkHandler()
        {
            return new HttpClientHandler { UseProxy = false, AllowAutoRedirect = false };
        }

        private static string Variable(string name)
        {
            string value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private static JsonObject ReadObject(string path, string what, List<string> unreadable)
        {
            if (path == null)
            {
                unreadable.Add($"{what} is not named");
                return null;
            }
            try
            {
                return JsonNode.Parse(System.IO.File.ReadAllText(path)) as JsonObject ?? throw new JsonException("it is not a JSON object");
            }
            catch (Exception e) when (e is System.IO.IOException || e is UnauthorizedAccessException || e is JsonException)
            {
                unreadable.Add($"{what} ({path}) could not be read: {e.Message}");
                return null;
            }
        }
    }

    // Reads JSON values without throwing on another shape.
    internal static class JsonRead
    {
        internal static JsonNode At(JsonNode node, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (node is not JsonObject json || !json.TryGetPropertyValue(key, out JsonNode value))
                {
                    return null;
                }
                node = value;
            }
            return node;
        }

        internal static string Text(JsonNode node, params string[] keys)
        {
            return At(node, keys) is JsonValue value && value.GetValueKind() == JsonValueKind.String ? value.GetValue<string>() : null;
        }

        // Read from the value's JSON text, which is the same for a parsed number and one a C# integer made.
        internal static long? Integer(JsonNode node, params string[] keys)
        {
            return At(node, keys) is JsonValue value && value.GetValueKind() == JsonValueKind.Number
                && long.TryParse(value.ToJsonString(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long number)
                ? number
                : (long?)null;
        }

        internal static bool? Flag(JsonNode node, params string[] keys)
        {
            JsonValueKind? kind = (At(node, keys) as JsonValue)?.GetValueKind();
            return kind == JsonValueKind.True ? true : kind == JsonValueKind.False ? false : (bool?)null;
        }
    }

    // Whether a case may run against this simulator: the generated manifest and definition, the canonical preset as the
    // launcher loaded it (serial uploads with fixed part lengths), readiness from the launcher's own instance in
    // simulation mode with contract version 3, its schema subset and the raw schema its generation records; then the
    // simulator as the reset left it, before the case changes anything: the reset's epoch, with the launcher's instance
    // and the case's definition echoed, live readiness of that epoch under the canonical profile (one partsize for every
    // part) listing every operation the case sends, and a complete journal of that epoch, instance and definition that has
    // dropped nothing.
    public static class ScenarioBinding
    {
        public const string Profile = "api-scenarios-canonical";

        public static List<string> Problems(BindingInputs inputs, ScenarioCase scenarioCase)
        {
            var problems = new List<string>(inputs.Unreadable);
            JsonObject manifest = inputs.Manifest;
            if (manifest != null)
            {
                Same(problems, "the manifest's definition", JsonRead.Text(manifest, "definition", "sha256"), scenarioCase.DefinitionSha256);
                JsonObject listed = (JsonRead.At(manifest, "cases") as JsonArray)?.OfType<JsonObject>().FirstOrDefault(entry => JsonRead.Text(entry, "id") == scenarioCase.Id);
                if (listed == null)
                {
                    problems.Add($"the manifest lists no case {scenarioCase.Id}");
                }
                else
                {
                    Same(problems, "the manifest's test", JsonRead.Text(listed, "test"), scenarioCase.Test);
                    Same(problems, "the manifest's positions", JsonRead.Integer(listed, "positions"), scenarioCase.Steps.Count);
                    Same(problems, "the manifest's simulator positions", JsonRead.Integer(listed, "simulator"), scenarioCase.SimulatorPositions);
                }
            }
            Same(problems, "the launcher's scenario", inputs.ScenarioName, Profile);
            if (inputs.Scenario != null)
            {
                Same(problems, "the scenario file's name", JsonRead.Text(inputs.Scenario, "name"), Profile);
                JsonNode profile = JsonRead.At(inputs.Scenario, "reset", "profile");
                Same(problems, "the preset's api_scenarios format", JsonRead.Text(profile, "api_scenarios", "format"), "files-api-scenarios");
                Same(problems, "the preset's api_scenarios format_version", JsonRead.Integer(profile, "api_scenarios", "format_version"), 1);
                Same(problems, "the preset's definition", JsonRead.Text(profile, "api_scenarios", "definition_sha256"), scenarioCase.DefinitionSha256);
                Same(problems, "the preset's upload mode", JsonRead.Text(profile, "upload", "mode"), "serial");
                Same(problems, "the preset's fixed_part_length", JsonRead.Flag(profile, "upload", "fixed_part_length"), true);
            }
            foreach ((string what, JsonObject ready) in new[] { ("the readiness the launcher accepted", inputs.ReadyReceipt), ("the readiness the simulator answers now", inputs.Ready) })
            {
                if (ready == null)
                {
                    continue;
                }
                Same(problems, $"{what}: status", JsonRead.Text(ready, "status"), "ready");
                Same(problems, $"{what}: mode", JsonRead.Text(ready, "mode"), "simulation");
                Same(problems, $"{what}: contract_version", JsonRead.Integer(ready, "contract_version"), 3);
                if (JsonRead.Text(ready, "instance") != inputs.Instance || inputs.Instance == null)
                {
                    problems.Add($"{what}: the instance is not the one the launcher gave its server");
                }
                Same(problems, $"{what}: schema subset", JsonRead.Text(ready, "schema_sha256"), inputs.SchemaSha256);
            }
            if (inputs.Generation != null)
            {
                Same(problems, "the simulator's generation: schema subset", JsonRead.Text(inputs.Generation, "schema_sha256"), inputs.SchemaSha256);
                Same(problems, "the simulator's generation: raw schema", JsonRead.Text(inputs.Generation, "swagger", "sha256"), JsonRead.Text(manifest, "schema", "sha256"));
            }
            // The readiness the launcher saved came before its reset, under the profile before it, so only the reset's
            // answer, live readiness and the journal are held to the reset's epoch and the canonical profile.
            long? epoch = null;
            if (inputs.Reset != null)
            {
                epoch = Epoch(inputs.Reset);
                if (epoch == null)
                {
                    problems.Add("the reset's answer names no epoch");
                }
                Echoes(problems, "the reset's answer", inputs.Reset, inputs.Instance, scenarioCase.DefinitionSha256);
            }
            if (inputs.Ready != null)
            {
                OfEpoch(problems, "the readiness the simulator answers now", JsonRead.Integer(inputs.Ready, "epoch"), epoch);
                JsonNode live = JsonRead.At(inputs.Ready, "profile");
                string identity = ApiScenariosMismatch(JsonRead.At(live, "api_scenarios"), scenarioCase.DefinitionSha256);
                if (identity != null)
                {
                    problems.Add($"the live profile's api_scenarios {identity}");
                }
                Same(problems, "the live profile's upload mode", JsonRead.Text(live, "upload", "mode"), "serial");
                Same(problems, "the live profile's fixed_part_length", JsonRead.Flag(live, "upload", "fixed_part_length"), true);
                if (UploadPartSize(inputs) == null)
                {
                    string changed = JsonRead.At(live, "upload", "partsize_changes") != null ? ", changed by its partsize_changes" : "";
                    problems.Add($"the live profile's upload partsize is {Shown.Of(JsonRead.Integer(live, "upload", "partsize"))}{changed}, not one positive partsize for every part");
                }
                SimulatorOperations operations = SimulatorOperations.From(inputs.Ready, problems);
                foreach (string route in RoutesOf(scenarioCase).Where(candidate => operations.Of(candidate) == null))
                {
                    problems.Add($"the simulator lists no operation {route}, which the case sends");
                }
                if (scenarioCase.Steps.Any(step => step.AppliesToSimulator && step is UploadStep) && operations.UploadPart == null)
                {
                    problems.Add("the simulator lists no single transfer operation that stores an upload's parts (PUT)");
                }
            }
            if (inputs.Journal != null)
            {
                Same(problems, "the journal's complete", JsonRead.Flag(inputs.Journal, "complete"), true);
                Same(problems, "the journal's dropped", JsonRead.Integer(inputs.Journal, "dropped"), 0);
                OfEpoch(problems, "the journal before the case", JsonRead.Integer(inputs.Journal, "epoch"), epoch);
                Echoes(problems, "the journal before the case", inputs.Journal, inputs.Instance, scenarioCase.DefinitionSha256);
            }
            return problems;
        }

        public static long? Epoch(JsonObject reset)
        {
            return JsonRead.Integer(reset, "epoch");
        }

        // The operations live readiness lists, which the journal names requests by.
        public static SimulatorOperations Operations(BindingInputs inputs)
        {
            return SimulatorOperations.From(inputs.Ready, new List<string>());
        }

        // The partsize the live profile grants every part of an upload with (Files#upload_part), which bounds an upload's
        // part numbers; null unless it is one positive partsize that no partsize_changes changes.
        public static long? UploadPartSize(BindingInputs inputs)
        {
            long? partsize = JsonRead.Integer(inputs.Ready, "profile", "upload", "partsize");
            return partsize > 0 && JsonRead.At(inputs.Ready, "profile", "upload", "partsize_changes") == null ? partsize : null;
        }

        // Every API route the case's simulator positions and its cleanup send.
        public static IEnumerable<string> RoutesOf(ScenarioCase scenarioCase)
        {
            return scenarioCase.Steps.Where(step => step.AppliesToSimulator).SelectMany(step => step.OperationRoutes)
                .Append(scenarioCase.DeleteRoute).Append(scenarioCase.LookupRoute).Distinct();
        }

        // Null when NODE is exactly the api_scenarios identity of the definition (profile.rb#validated_api_scenarios: its
        // format, format version and SHA-256, and nothing else), or why not.
        internal static string ApiScenariosMismatch(JsonNode node, string definitionSha256)
        {
            if (definitionSha256 != null && node is JsonObject identity && identity.Count == 3 && JsonRead.Text(identity, "format") == "files-api-scenarios"
                && JsonRead.Integer(identity, "format_version") == 1 && JsonRead.Text(identity, "definition_sha256") == definitionSha256)
            {
                return null;
            }
            return $"is {node?.ToJsonString() ?? "null"}, not the definition {definitionSha256}";
        }

        // What the case bound itself to, for its receipt.
        internal static JsonObject Summary(BindingInputs inputs)
        {
            return new JsonObject
            {
                ["scenario"] = inputs.ScenarioName,
                ["definition_sha256"] = JsonRead.Text(inputs.Scenario, "reset", "profile", "api_scenarios", "definition_sha256"),
                ["upload_profile"] = JsonRead.At(inputs.Scenario, "reset", "profile", "upload")?.DeepClone(),
                ["contract_version"] = JsonRead.Integer(inputs.Ready, "contract_version"),
                ["instance"] = JsonRead.Text(inputs.Ready, "instance"),
                ["schema_subset_sha256"] = JsonRead.Text(inputs.Ready, "schema_sha256"),
                ["raw_schema_sha256"] = JsonRead.Text(inputs.Generation, "swagger", "sha256"),
                ["manifest_schema_sha256"] = JsonRead.Text(inputs.Manifest, "schema", "sha256"),
                ["epoch"] = Epoch(inputs.Reset),
                ["live_epoch"] = JsonRead.Integer(inputs.Ready, "epoch"),
                ["live_upload_profile"] = JsonRead.At(inputs.Ready, "profile", "upload")?.DeepClone(),
                ["live_api_scenarios"] = JsonRead.At(inputs.Ready, "profile", "api_scenarios")?.DeepClone(),
                ["journal_epoch"] = JsonRead.Integer(inputs.Journal, "epoch"),
            };
        }

        // The instance and api_scenarios identity a reset's or journal's answer echoes under the api_scenarios profile
        // (simulation.rb#api_scenarios_echo).
        private static void Echoes(List<string> problems, string what, JsonObject answer, string instance, string definitionSha256)
        {
            if (JsonRead.Text(answer, "instance") != instance || instance == null)
            {
                problems.Add($"{what}: the instance is not the one the launcher gave its server");
            }
            string identity = ApiScenariosMismatch(JsonRead.At(answer, "api_scenarios"), definitionSha256);
            if (identity != null)
            {
                problems.Add($"{what}: api_scenarios {identity}");
            }
        }

        private static void OfEpoch(List<string> problems, string what, long? actual, long? reset)
        {
            if (actual == null || actual != reset)
            {
                problems.Add($"{what} is of epoch {Shown.Of(actual)}, not the reset's {Shown.Of(reset)}");
            }
        }

        private static void Same(List<string> problems, string what, string actual, string expected)
        {
            if (actual != expected || expected == null)
            {
                problems.Add($"{what} is {Shown.Of(actual)}, not {Shown.Of(expected)}");
            }
        }

        private static void Same(List<string> problems, string what, long? actual, long expected)
        {
            if (actual != expected)
            {
                problems.Add($"{what} is {Shown.Of(actual)}, not {expected}");
            }
        }

        private static void Same(List<string> problems, string what, bool? actual, bool expected)
        {
            if (actual != expected)
            {
                problems.Add($"{what} is {Shown.Of(actual)}, not {expected.ToString().ToLowerInvariant()}");
            }
        }
    }

    // One request the SDK sent through its transport, as the handler at its bottom saw it: the method, the path without
    // its query (only the query's names are kept, never a signed URL's values), its origin, whether it went to the API or
    // to storage, whether it carried the client's API key or any Authorization, the upload ref (by its SHA-256 only) and
    // part number its JSON API body named, and the answer or the client's own failure. Its start and its end each take
    // their place in the recorder's one order of events (RequestRecorder.NextEvent), as the caller's intent to cancel an
    // upload does (CancellationGate.CancelEvent): which came first is read from that order, never from clocks. Whether
    // the caller's own token (RequestRecorder.Owner) was actually cancelled is read as the request begins, as its own
    // token's cancellation reaches the handler and as it ends, and whether the transport's own task had already ended
    // when that cancellation was relayed to it, so a request ended by something else before the caller cancelled is never
    // taken for one the caller's cancellation ended, however late the handler observes its end. Its transport's task, and
    // for a request to storage each writing of its body and each write of that body to the network, with the token the
    // SDK made the write with, are kept as they begin (RequestRecorder.Tracking and Writing), so that the caller's actual
    // cancellation of an upload, taken before it reaches any token of the SDK's (RequestRecorder.CancellingOwner), reads
    // which of them had already failed then (FailedBeforeOwner), however late their failure is observed after.
    public sealed class ObservedRequest
    {
        private readonly object gate = new object();
        private int? status;
        private string errorType;
        private string clientFailure;
        private bool tokenCancelled;
        private long? endedAt;
        private long? tokenCancelledAt;
        private bool? ownerCancelledWithToken;
        private bool? transportEndedBeforeToken;
        private bool ownerCancelledAtEnd;
        // What of the request is tracked, the tokens its body's writes were made with, before the caller's actual
        // cancellation or after it, and what that cancellation found failed: changed only under the recorder's lock as
        // well as this one (RequestRecorder).
        private readonly List<TrackedTask> tasks = new List<TrackedTask>();
        private readonly List<(CancellationToken Token, bool BeforeOwner)> writtenWith = new List<(CancellationToken Token, bool BeforeOwner)>();
        private readonly List<(string What, Exception Failure)> failedAcrossOwner = new List<(string What, Exception Failure)>();
        private string failedBeforeOwner;
        private string failedBeforeOwnerWith;

        internal ObservedRequest(int sequence, int position, string method, string path, IReadOnlyList<string> queryNames, string uriSha256, bool api, bool carriesApiKey, bool apiKeyMatches, bool carriesAuthorization,
            string origin = null, long begunAt = 0, string bodyRefSha256 = null, long? bodyPart = null, bool ownerCancelledAtBegin = false, bool tokenCancelledAtBegin = false)
        {
            Sequence = sequence;
            Position = position;
            Method = method;
            Path = path;
            QueryNames = queryNames;
            UriSha256 = uriSha256;
            Api = api;
            CarriesApiKey = carriesApiKey;
            ApiKeyMatches = apiKeyMatches;
            CarriesAuthorization = carriesAuthorization;
            Origin = origin;
            BegunAt = begunAt;
            BodyRefSha256 = bodyRefSha256;
            BodyPart = bodyPart;
            OwnerCancelledAtBegin = ownerCancelledAtBegin;
            TokenCancelledAtBegin = tokenCancelledAtBegin;
        }

        public int Sequence { get; }

        public int Position { get; }

        public string Method { get; }

        public string Path { get; }

        public IReadOnlyList<string> QueryNames { get; }

        // The SHA-256 of the whole URI, which tells requests apart without keeping it.
        public string UriSha256 { get; }

        public bool Api { get; }

        public bool CarriesApiKey { get; }

        public bool ApiKeyMatches { get; }

        public bool CarriesAuthorization { get; }

        // The scheme, host and port the request went to.
        public string Origin { get; }

        // The event the request began at.
        public long BegunAt { get; }

        // The SHA-256 of the upload ref a JSON API body named, and the part it named, as a begin_upload that continues an
        // upload names them (UploadSession.ContinueAsync); null for any other request.
        public string BodyRefSha256 { get; }

        public long? BodyPart { get; }

        // Whether the caller's own token (RequestRecorder.Owner) had already been cancelled when the request reached the
        // handler: a request that reaches it only after the caller cancelled.
        public bool OwnerCancelledAtBegin { get; }

        // Whether the request's own token had already been cancelled when it reached the handler.
        public bool TokenCancelledAtBegin { get; }

        public int? Status
        {
            get { lock (gate) { return status; } }
        }

        // The type of an API error answer's body, such as not-found.
        public string ErrorType
        {
            get { lock (gate) { return errorType; } }
        }

        // The exception the request ended with when no answer came, such as TaskCanceledException.
        public string ClientFailure
        {
            get { lock (gate) { return clientFailure; } }
        }

        // Whether the token the request's transport was given was cancelled when it failed. That token is the SDK's,
        // linked to its network wait (NetworkWatch) as well as to the caller's, so it never says by itself who cancelled.
        public bool TokenCancelled
        {
            get { lock (gate) { return tokenCancelled; } }
        }

        // The event at which the cancellation of the request's own token reached the handler, which then, and only then,
        // cancels the token it gave the transport below it (ObservingHandler); null when its token was not cancelled
        // before the request ended.
        public long? TokenCancelledAt
        {
            get { lock (gate) { return tokenCancelledAt; } }
        }

        // Whether the caller's own token had been cancelled at that moment: true when the caller's cancellation reached
        // the request's token, false when its network wait ran out, or anything else cancelled it, before the caller
        // cancelled. A token is cancelled once, so a caller that cancels afterwards never changes this. Null when its
        // token was not cancelled.
        public bool? OwnerCancelledWithToken
        {
            get { lock (gate) { return ownerCancelledWithToken; } }
        }

        // Whether the transport's own task had already ended, answered or failed, when the cancellation of the request's
        // token was relayed to it (ObservingHandler.TokenRelay): an end that relay did not cause, however late the handler
        // then observed it, and that the caller's cancellation caused only where the request's body ended by the upload's
        // own token, which that cancellation is what cancelled (UploadEvidence.Settle). Null when its token was not
        // cancelled.
        public bool? TransportEndedBeforeToken
        {
            get { lock (gate) { return transportEndedBeforeToken; } }
        }

        // What of the request had already failed when the caller actually cancelled the upload, before that cancellation
        // reached any token of the SDK's (CancellationGate.OwnerCancelledAt): its transport's task (TrackedTask.Transport),
        // a writing of its body by the SDK's content (TrackedTask.Body) or one write of that body to the network
        // (TrackedTask.Write), the outermost first; null when nothing of it had. What had failed then did not fail because
        // the caller cancelled, however late its failure was observed or relayed.
        public string FailedBeforeOwner
        {
            get { lock (gate) { return failedBeforeOwner; } }
        }

        // The exception that had failed it then, such as TaskCanceledException; null when nothing had failed.
        public string FailedBeforeOwnerWith
        {
            get { lock (gate) { return failedBeforeOwnerWith; } }
        }

        // The exception the request's body last ended with, as the SDK's content failed to write itself; null when no
        // writing of it failed. It is read from that writing's own task once the request has ended, never as it fails.
        public string BodyEndedWith => BodyFailure?.GetType().Name;

        // That exception itself, whose token is compared with the upload's own (UploadEvidence.Settle).
        internal Exception BodyFailure
        {
            get
            {
                lock (gate)
                {
                    TrackedTask body = tasks.LastOrDefault(task => task.What == TrackedTask.Body && task.Task != null && task.Task.IsCompleted);
                    return body == null || body.Task.IsCompletedSuccessfully ? null : TrackedTask.FailureOf(body.Task);
                }
            }
        }

        // The tokens the writes of the request's body were made with, each once: every write's, and those of the writes
        // that began before the caller actually cancelled.
        internal IReadOnlyList<CancellationToken> WrittenWith
        {
            get { lock (gate) { return writtenWith.Select(write => write.Token).Distinct().ToList(); } }
        }

        internal IReadOnlyList<CancellationToken> WrittenBeforeOwner
        {
            get { lock (gate) { return writtenWith.Where(write => write.BeforeOwner).Select(write => write.Token).Distinct().ToList(); } }
        }

        // What the caller's actual cancellation found begun but whose task was not yet known, and that had already failed
        // once it was, with what it failed with: when it failed, before that cancellation or after it, only its failure
        // can tell (UploadEvidence.Settle).
        internal IReadOnlyList<(string What, Exception Failure)> FailedAcrossOwner
        {
            get { lock (gate) { return failedAcrossOwner.ToList(); } }
        }

        // Whether the caller's own token had been cancelled when the request ended.
        public bool OwnerCancelledAtEnd
        {
            get { lock (gate) { return ownerCancelledAtEnd; } }
        }

        // The event the request ended at, with its answer's headers or its failure; null while it has not ended.
        public long? EndedAt
        {
            get { lock (gate) { return endedAt; } }
        }

        public bool Succeeded => Status >= 200 && Status < 300;

        public string Outcome
        {
            get
            {
                if (Status.HasValue)
                {
                    return $"HTTP {Status}{(ErrorType != null ? " " + ErrorType : "")}";
                }
                return ClientFailure != null ? $"no answer: {ClientFailure}{(TokenCancelled ? " (its token cancelled)" : "")}" : "no answer yet";
            }
        }

        internal void Answered(int answer, string type, long at = 0, bool ownerCancelled = false)
        {
            lock (gate)
            {
                status = answer;
                errorType = type;
                endedAt = at;
                ownerCancelledAtEnd = ownerCancelled;
            }
        }

        internal void Failed(string failure, bool cancelled, long at = 0, bool ownerCancelled = false)
        {
            lock (gate)
            {
                clientFailure = failure;
                tokenCancelled = cancelled;
                endedAt = at;
                ownerCancelledAtEnd = ownerCancelled;
            }
        }

        // The cancellation of the request's own token reached the handler at event at, with the caller's own token
        // cancelled or not, and was relayed to a transport whose own task had or had not already ended. Only the first
        // counts.
        internal void TokenCancelledNow(long at, bool ownerCancelled, bool transportEnded = false)
        {
            lock (gate)
            {
                if (tokenCancelledAt == null)
                {
                    tokenCancelledAt = at;
                    ownerCancelledWithToken = ownerCancelled;
                    transportEndedBeforeToken = transportEnded;
                }
            }
        }

        // The request begins an operation of WHAT (TrackedTask), whose task follows (Tracked); called by the recorder, under
        // its lock (RequestRecorder.Tracking and Writing). Writes that completed are let go as the next begins: only a
        // write's failure is of account.
        internal TrackedTask Tracking(string what)
        {
            lock (gate)
            {
                if (what == TrackedTask.Write)
                {
                    tasks.RemoveAll(task => task.What == TrackedTask.Write && task.Task != null && task.Task.IsCompletedSuccessfully);
                }
                var tracked = new TrackedTask(what);
                tasks.Add(tracked);
                return tracked;
            }
        }

        // TRACKED's TASK is known; called by the recorder, under its lock (RequestRecorder.Tracked). One the caller's actual
        // cancellation found still being started (TrackedTask.Straddled) that has already failed may have failed before
        // that cancellation as well as after it, so its failure is kept, to be read for what it was (FailedAcrossOwner).
        internal void Tracked(TrackedTask tracked, Task task)
        {
            lock (gate)
            {
                tracked.Task = task;
                if (tracked.Straddled && task.IsCompleted && !task.IsCompletedSuccessfully)
                {
                    failedAcrossOwner.Add((tracked.What, TrackedTask.FailureOf(task)));
                }
            }
        }

        // A write of the request's body was made with TOKEN, beginning before the caller's actual cancellation or not
        // (BEFOREOWNER); called by the recorder, under its lock (RequestRecorder.Writing).
        internal void WroteWith(CancellationToken token, bool beforeOwner)
        {
            lock (gate)
            {
                if (!writtenWith.Contains((token, beforeOwner)))
                {
                    writtenWith.Add((token, beforeOwner));
                }
            }
        }

        // The caller actually cancels the upload now, before its cancellation reaches any token of the SDK's; called by the
        // recorder, under its lock (RequestRecorder.CancellingOwner). What of the request had already failed is kept, the
        // outermost first, and what has begun but whose task is not yet known is read once it is (Tracked).
        internal void OwnerCancelling()
        {
            lock (gate)
            {
                foreach (string what in TrackedTask.Outermost)
                {
                    TrackedTask failed = tasks.FirstOrDefault(task => task.What == what && task.Task != null && task.Task.IsCompleted && !task.Task.IsCompletedSuccessfully);
                    if (failed != null && failedBeforeOwner == null)
                    {
                        failedBeforeOwner = what;
                        failedBeforeOwnerWith = TrackedTask.FailureOf(failed.Task).GetType().Name;
                    }
                }
                foreach (TrackedTask starting in tasks.Where(task => task.Task == null))
                {
                    starting.Straddled = true;
                }
            }
        }
    }

    // One operation of a request that the caller's actual cancellation of an upload is read against
    // (RequestRecorder.CancellingOwner): its transport's task (Transport), a writing of its body by the SDK's content
    // (Body), or one write of that body to the network (Write). Its task is known only once the operation has been
    // started; one the cancellation finds still being started is Straddled, and read once its task is known.
    internal sealed class TrackedTask
    {
        internal const string Transport = "its transport";
        internal const string Body = "its body";
        internal const string Write = "a write of its body";

        // The operations by which what had failed is told, the outermost first.
        internal static readonly string[] Outermost = { Transport, Body, Write };

        internal TrackedTask(string what)
        {
            What = what;
        }

        internal string What { get; }

        internal Task Task { get; set; }

        internal bool Straddled { get; set; }

        // The exception TASK, which has completed, ended with, as awaiting it throws it; null when it succeeded.
        internal static Exception FailureOf(Task task)
        {
            try
            {
                task.GetAwaiter().GetResult();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }

    // Every request a client sent, in the order they started, each with the position that was running.
    public sealed class RequestRecorder
    {
        private const string ApiKeyHeader = "X-FilesAPI-Key";

        private readonly object gate = new object();
        private readonly List<ObservedRequest> requests = new List<ObservedRequest>();
        private readonly Uri apiOrigin;
        private readonly string apiKey;
        private int position;
        private long events;
        private CancellationToken owner;
        // The event at which the caller actually cancelled the current upload (CancellingOwner); null before that.
        private long? ownerCancelledAt;

        public RequestRecorder(string baseUrl, string apiKey)
        {
            apiOrigin = new Uri(baseUrl);
            this.apiKey = apiKey;
        }

        // The next event of the run's one order of events: each request's start and end, the cancellation of its own token
        // reaching the handler, and the caller's intent to cancel an upload and its actual cancellation.
        public long NextEvent()
        {
            return Interlocked.Increment(ref events);
        }

        public int Position
        {
            get { lock (gate) { return position; } }
            set { lock (gate) { position = value; } }
        }

        // The caller's own token for the upload whose requests are being sent (ScenarioRun.UploadAsync's caller), which
        // every request reads as it begins, as its own token's cancellation reaches the handler and as it ends: whether
        // the caller had actually cancelled then, apart from any marker of its intent. CancellationToken.None, which is
        // never cancelled, outside an upload. Setting it begins that upload, which the caller has not yet actually
        // cancelled (CancellingOwner).
        public CancellationToken Owner
        {
            get { lock (gate) { return owner; } }
            set
            {
                lock (gate)
                {
                    owner = value;
                    ownerCancelledAt = null;
                }
            }
        }

        public int Count
        {
            get { lock (gate) { return requests.Count; } }
        }

        public IReadOnlyList<ObservedRequest> All => Since(0);

        public IReadOnlyList<ObservedRequest> Since(int index)
        {
            lock (gate)
            {
                return requests.Skip(index).ToList();
            }
        }

        // Records REQUEST as begun, now, with the upload ref (BODYREFSHA256) and part its JSON API body named, whether the
        // caller's own token (OWNER) had been cancelled and whether its own token (TOKENCANCELLED) already was.
        internal ObservedRequest Begin(HttpRequestMessage request, string bodyRefSha256, long? bodyPart, CancellationToken owner = default, bool tokenCancelled = false)
        {
            Uri uri = request.RequestUri;
            bool api = uri != null && SameOrigin(uri) && uri.AbsolutePath.StartsWith("/api/rest/v1/", StringComparison.Ordinal);
            bool carriesApiKey = request.Headers.TryGetValues(ApiKeyHeader, out IEnumerable<string> keys);
            bool apiKeyMatches = carriesApiKey && keys.Count() == 1 && string.Equals(keys.Single(), apiKey, StringComparison.Ordinal);
            string[] queryNames = (uri?.Query ?? "").TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(pair => Uri.UnescapeDataString(pair.Split('=')[0])).ToArray();
            lock (gate)
            {
                // Read before the start takes its place: a caller that cancels after this cancels after the request began.
                bool ownerCancelled = owner.IsCancellationRequested;
                var observed = new ObservedRequest(requests.Count + 1, position, request.Method.Method.ToUpperInvariant(), uri?.AbsolutePath ?? "", queryNames,
                    ScenarioDigest.Sha256(uri?.AbsoluteUri ?? ""), api, carriesApiKey, apiKeyMatches, request.Headers.Authorization != null,
                    uri?.GetLeftPart(UriPartial.Authority), NextEvent(), api ? bodyRefSha256 : null, api ? bodyPart : null, ownerCancelled, tokenCancelled);
                requests.Add(observed);
                return observed;
            }
        }

        // REQUEST begins an operation of WHAT (TrackedTask), whose task follows (Tracked). Under this lock, it begins either
        // before the caller's actual cancellation of the upload (CancellingOwner) or after it.
        internal TrackedTask Tracking(ObservedRequest request, string what)
        {
            lock (gate)
            {
                return request.Tracking(what);
            }
        }

        // The task of REQUEST's operation TRACKED is known.
        internal void Tracked(ObservedRequest request, TrackedTask tracked, Task task)
        {
            lock (gate)
            {
                request.Tracked(tracked, task);
            }
        }

        // A write of REQUEST's body to the network begins, made with TOKEN, the token the SDK's content wrote it with: kept
        // as a write that began before the caller actually cancelled the upload or after it, and tracked as a write.
        internal TrackedTask Writing(ObservedRequest request, CancellationToken token)
        {
            lock (gate)
            {
                request.WroteWith(token, ownerCancelledAt == null);
                return request.Tracking(TrackedTask.Write);
            }
        }

        // The caller actually cancels the upload whose requests are those from FIRST on, before its cancellation reaches
        // the token the SDK was given (SynchronousProgress). Its place among the events is taken now, with what of each of
        // those requests had already failed (ObservedRequest.FailedBeforeOwner) and the token the SDK's bodies of the
        // upload's parts wrote with, as the writes of its storage requests that began before now showed it, the token
        // itself, which CANCELLATION reads the caller's cancellation against (CancellationGate.OwnerCancelling). No
        // operation of those requests begins, and no task of theirs becomes known, while this is taken.
        internal void CancellingOwner(int first, CancellationGate cancellation)
        {
            long at;
            List<(int Sequence, CancellationToken Token)> written;
            lock (gate)
            {
                at = NextEvent();
                ownerCancelledAt = at;
                List<ObservedRequest> upload = requests.Skip(first).ToList();
                foreach (ObservedRequest request in upload)
                {
                    request.OwnerCancelling();
                }
                written = upload.Where(request => !request.Api).SelectMany(request => request.WrittenBeforeOwner.Select(token => (request.Sequence, token))).ToList();
            }
            List<CancellationToken> tokens = written.Select(write => write.Token).Distinct().ToList();
            cancellation.OwnerCancelling(at, tokens.Count == 1 ? tokens[0] : (CancellationToken?)null,
                tokens.Count == 1 ? written.Select(write => write.Sequence).Distinct().ToArray() : Array.Empty<int>());
        }

        private bool SameOrigin(Uri uri)
        {
            return Uri.Compare(uri, apiOrigin, UriComponents.SchemeAndServer, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }

    // The bottom of the client's own transport (HttpTransport.FromHandlerFactory): it records each request and its
    // answer, or its failure, and changes nothing. The SDK's authentication, retries, redirects, timeouts and cancellation
    // stay above it: the transport below is cancelled exactly when the request's own token is, only after the handler has
    // recorded that, with the caller's own state (RequestRecorder.Owner) and whether the transport's own task had already
    // ended (TokenRelay). Only the SDK's own JSON body of an API request (StringContent, already in memory) is read, for the
    // upload ref and part it names, and an API error answer's body, buffered so the SDK reads it as sent, for its type. A
    // request to storage keeps the body the SDK gave it, which the transport writes as it would, through ObservedBody,
    // which only tracks each writing of it and each write of it to the network, with the token the SDK made each write
    // with; the request gets that body back once it has ended. Each request's transport task is tracked too
    // (RequestRecorder.Tracking), for the caller's actual cancellation of an upload to read
    // (RequestRecorder.CancellingOwner).
    internal sealed class ObservingHandler : DelegatingHandler
    {
        private const long ErrorBodyBytes = 1024 * 1024;

        private readonly RequestRecorder recorder;

        internal ObservingHandler(RequestRecorder recorder, HttpMessageHandler inner) : base(inner)
        {
            this.recorder = recorder;
        }

        // Runs once the transport has ended a request without an answer, before the handler observes that end; null in a
        // scenario. A control sets it to hold the observation back, as a continuation the runtime runs late holds it.
        internal Func<Task> HoldObservation { get; set; }

        // Runs as the cancellation of a request's own token reaches the handler, before that cancellation is recorded and
        // relayed; null in a scenario. A control sets it to hold the recording back, as a callback the runtime runs late
        // holds it: the handler records and relays the cancellation once the task it returns has ended, and observes the
        // request's end only after that. A null task holds nothing.
        internal Func<ObservedRequest, Task> HoldRelay { get; set; }

        // Runs once a write of a storage request's body to the network has failed, before the body sees that failure; null
        // in a scenario. A control sets it to hold the body's sight of the failure back, and with it the end of the
        // request's transport, as a continuation the runtime runs late holds it. A null task holds nothing.
        internal Func<Task> HoldWrite { get; set; }

        // Runs as a request's transport is about to be started, before the relay of its token's cancellation is told
        // so and before that start is tracked (TokenRelay.StartingAsync); null in a scenario. A control sets it to
        // hold the start back, as the runtime can hold a thread there, so that a cancellation reaches the request's
        // token there and is relayed at once: the transport then starts with the token that relay cancelled. A null
        // task holds nothing.
        internal Func<ObservedRequest, Task> HoldStart { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            (string refSha256, long? part) = await UploadPartOfAsync(request.Content).ConfigureAwait(false);
            CancellationToken owner = recorder.Owner;
            ObservedRequest observed = recorder.Begin(request, refSha256, part, owner, cancellationToken.IsCancellationRequested);
            // The transport below gets a token of the handler's own, cancelled only once the cancellation of the request's
            // token, whether the caller's own token had been cancelled by then and whether the transport's own task had
            // already ended have been recorded (TokenRelay): a request the transport ends because its token was cancelled
            // ends after that is recorded, however the transport's callbacks and continuations are ordered, and one the
            // transport had already ended keeps that end as its own, however late this handler observes it.
            using var transport = new CancellationTokenSource();
            var relay = new TokenRelay(observed, recorder, owner, transport);
            Func<ObservedRequest, Task> hold = HoldRelay;
            Action onCancelled = hold == null ? relay.Cancelled : () => relay.CancelledAfter(hold(observed));
            CancellationTokenRegistration cancelling = cancellationToken.Register(onCancelled);
            // A storage request's body, the SDK's own, is written by the transport through ObservedBody, which tracks each
            // writing of it and each write of it, with the token the SDK made each write with.
            HttpContent content = request.Content;
            if (!observed.Api && content != null)
            {
                request.Content = new ObservedBody(content, observed, recorder, HoldWrite);
            }
            Task<HttpResponseMessage> sending;
            // The relay is told the transport is about to be started, once any hold a control sets has ended, before
            // the transport's start is tracked. A cancellation it relays at once came before that start: the caller's
            // actual cancellation of an upload, taken before it reaches the request's token
            // (RequestRecorder.CancellingOwner), never finds that start begun, and the transport, started with the
            // token the relay cancelled, ends as that cancellation ends it. One that reaches the relay after waits for
            // the transport's task (TokenRelay).
            Func<ObservedRequest, Task> holdStart = HoldStart;
            await relay.StartingAsync(holdStart?.Invoke(observed)).ConfigureAwait(false);
            TrackedTask tracked = recorder.Tracking(observed, TrackedTask.Transport);
            try
            {
                sending = base.SendAsync(request, transport.Token);
            }
            catch (Exception e)
            {
                sending = Task.FromException<HttpResponseMessage>(e);
            }
            recorder.Tracked(observed, tracked, sending);
            relay.Started(sending);
            HttpResponseMessage response;
            try
            {
                response = await sending.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                request.Content = content;
                if (HoldObservation != null)
                {
                    await HoldObservation().ConfigureAwait(false);
                }
                // Waits for a cancellation another thread is recording, so that it takes its place before this end, and
                // for one held back (HoldRelay) to be recorded and relayed.
                cancelling.Dispose();
                await relay.Relayed().ConfigureAwait(false);
                // Whether its own token, and the caller's, were cancelled is read as it failed, before its end takes its
                // place among the events.
                bool cancelled = cancellationToken.IsCancellationRequested;
                bool ownerCancelled = owner.IsCancellationRequested;
                observed.Failed(e.GetType().Name, cancelled, recorder.NextEvent(), ownerCancelled);
                throw;
            }
            request.Content = content;
            cancelling.Dispose();
            await relay.Relayed().ConfigureAwait(false);
            bool ownerCancelledAtAnswer = owner.IsCancellationRequested;
            long answered = recorder.NextEvent();
            string errorType = null;
            if (observed.Api && (int)response.StatusCode >= 400 && response.Content != null)
            {
                errorType = await ErrorTypeAsync(response.Content).ConfigureAwait(false);
            }
            observed.Answered((int)response.StatusCode, errorType, answered, ownerCancelledAtAnswer);
            return response;
        }

        // The upload ref, as its SHA-256, and the part number a JSON body names ("ref", and "part" as a number or decimal
        // text), as FilesApi sends a begin_upload's parameters (a StringContent of application/json); nulls for any other
        // body, which is never read.
        private static async Task<(string RefSha256, long? Part)> UploadPartOfAsync(HttpContent content)
        {
            if (!(content is ByteArrayContent) || content.Headers.ContentType?.MediaType != "application/json")
            {
                return (null, null);
            }
            try
            {
                using JsonDocument document = JsonDocument.Parse(await content.ReadAsByteArrayAsync().ConfigureAwait(false));
                JsonElement body = document.RootElement;
                if (body.ValueKind != JsonValueKind.Object)
                {
                    return (null, null);
                }
                string reference = body.TryGetProperty("ref", out JsonElement named) && named.ValueKind == JsonValueKind.String ? named.GetString() : null;
                long? part = null;
                if (body.TryGetProperty("part", out JsonElement number))
                {
                    if (number.ValueKind == JsonValueKind.Number && number.TryGetInt64(out long value))
                    {
                        part = value;
                    }
                    else if (number.ValueKind == JsonValueKind.String && long.TryParse(number.GetString(), NumberStyles.None, CultureInfo.InvariantCulture, out long text))
                    {
                        part = text;
                    }
                }
                return (reference == null ? null : ScenarioDigest.Sha256(reference), part);
            }
            catch (JsonException)
            {
                return (null, null);
            }
        }

        private static async Task<string> ErrorTypeAsync(HttpContent content)
        {
            try
            {
                await content.LoadIntoBufferAsync(ErrorBodyBytes).ConfigureAwait(false);
                using JsonDocument document = JsonDocument.Parse(await content.ReadAsStringAsync().ConfigureAwait(false));
                return document.RootElement.ValueKind == JsonValueKind.Object && document.RootElement.TryGetProperty("type", out JsonElement type) && type.ValueKind == JsonValueKind.String
                    ? type.GetString()
                    : null;
            }
            catch (Exception e) when (e is HttpRequestException || e is JsonException || e is InvalidOperationException || e is System.IO.IOException)
            {
                return null;
            }
        }

        // Relays the cancellation of a request's own token to the token its transport was given, having first recorded
        // it (ObservedRequest.TokenCancelledNow): its place among the events, whether the caller's own token had been
        // cancelled by then, and whether the transport's own task had already ended, which a cancellation relayed after it
        // did not cause. One that comes before the transport is started is relayed at once, so the transport never starts
        // with a live token its request's own has outlived; one that comes while the transport is being started, before
        // its task is known, is relayed as soon as it is (Started), so its task's state is always read before the relay.
        // The transport is being started from before its start is tracked (StartingAsync, RequestRecorder.Tracking): a
        // start that the caller's actual cancellation of an upload finds begun (RequestRecorder.CancellingOwner) is
        // never one that cancellation is relayed to at once, with a token it cancelled before the transport started.
        // One a control holds back (ObservingHandler.HoldRelay) is recorded and relayed only once its hold has ended, as a
        // callback the runtime runs late (CancelledAfter), and the handler waits for that (Relayed) before it observes the
        // request's end.
        private sealed class TokenRelay
        {
            private readonly object gate = new object();
            private readonly ObservedRequest observed;
            private readonly RequestRecorder recorder;
            private readonly CancellationToken owner;
            private readonly CancellationTokenSource transport;
            private bool starting;
            private Task sending;
            private bool deferred;
            private long deferredAt;
            private bool deferredOwner;
            // The recording and relay of a cancellation held back, once it is scheduled; null for none.
            private Task late;

            internal TokenRelay(ObservedRequest observed, RequestRecorder recorder, CancellationToken owner, CancellationTokenSource transport)
            {
                this.observed = observed;
                this.recorder = recorder;
                this.owner = owner;
                this.transport = transport;
            }

            // The transport is about to be started, once HOLD has ended (ObservingHandler.HoldStart); its start is
            // tracked only after this. A cancellation relayed before this was relayed at once, and one after it waits
            // for the transport's task (Started). A null HOLD holds nothing.
            internal async Task StartingAsync(Task hold)
            {
                if (hold != null)
                {
                    await hold.ConfigureAwait(false);
                }
                lock (gate)
                {
                    starting = true;
                }
            }

            // The request's own token was cancelled.
            internal void Cancelled()
            {
                long at = recorder.NextEvent();
                bool ownerCancelled = owner.IsCancellationRequested;
                lock (gate)
                {
                    if (starting && sending == null)
                    {
                        deferred = true;
                        deferredAt = at;
                        deferredOwner = ownerCancelled;
                        return;
                    }
                    observed.TokenCancelledNow(at, ownerCancelled, sending != null && sending.IsCompleted);
                }
                transport.Cancel();
            }

            // The transport was started, as TASK.
            internal void Started(Task task)
            {
                lock (gate)
                {
                    sending = task;
                    if (!deferred)
                    {
                        return;
                    }
                    observed.TokenCancelledNow(deferredAt, deferredOwner, task.IsCompleted);
                }
                transport.Cancel();
            }

            // The request's own token was cancelled, and the recording and relay of that are held until HOLD has ended
            // (ObservingHandler.HoldRelay); a null HOLD holds nothing.
            internal void CancelledAfter(Task hold)
            {
                if (hold == null)
                {
                    Cancelled();
                    return;
                }
                Task relayed = hold.ContinueWith(_ => Cancelled(), CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
                lock (gate)
                {
                    late = relayed;
                }
            }

            // Ends once a cancellation held back has been recorded and relayed, or at once when none was.
            internal Task Relayed()
            {
                lock (gate)
                {
                    return late == null ? Task.CompletedTask : late.ContinueWith(_ => { }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                }
            }
        }

        // The body of a request to storage as the transport writes it: the SDK's own content, which writes itself, with its
        // own headers, exactly as it would, to the stream the transport gives it, through WrittenStream. Each writing of it
        // is tracked as it begins (RequestRecorder.Tracking), and so is each write of it to the network, with the token the
        // SDK made the write with (RequestRecorder.Writing); what a writing ended with is read from its own task once the
        // request has ended (ObservedRequest.BodyFailure), never decided as it fails. The SDK's content stays the SDK's: it
        // is never disposed here.
        private sealed class ObservedBody : HttpContent
        {
            private readonly HttpContent content;
            private readonly ObservedRequest observed;
            private readonly RequestRecorder recorder;
            private readonly Func<Task> holdWrite;

            internal ObservedBody(HttpContent content, ObservedRequest observed, RequestRecorder recorder, Func<Task> holdWrite)
            {
                this.content = content;
                this.observed = observed;
                this.recorder = recorder;
                this.holdWrite = holdWrite;
                foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
                {
                    Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            protected override Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context)
            {
                return SerializeToStreamAsync(stream, context, CancellationToken.None);
            }

            protected override async Task SerializeToStreamAsync(System.IO.Stream stream, System.Net.TransportContext context, CancellationToken cancellationToken)
            {
                TrackedTask tracked = recorder.Tracking(observed, TrackedTask.Body);
                Task writing;
                try
                {
                    writing = content.CopyToAsync(new WrittenStream(stream, observed, recorder, holdWrite), context, cancellationToken);
                }
                catch (Exception e)
                {
                    writing = Task.FromException(e);
                }
                recorder.Tracked(observed, tracked, writing);
                await writing.ConfigureAwait(false);
            }

            protected override bool TryComputeLength(out long length)
            {
                long? known = content.Headers.ContentLength;
                length = known ?? 0;
                return known.HasValue;
            }
        }

        // The network stream a storage request's body is written to, as ObservedBody gives it to the SDK's content. Each
        // write goes on to the network unchanged, with the token it was made with, which WatchedContent passes on from
        // the SDK's part content, the upload's own token; that token is kept, and the write's task tracked, as the write
        // begins (RequestRecorder.Writing). A write that fails reaches the body as it failed, once any hold a control sets
        // has ended (ObservingHandler.HoldWrite). Flushes and writes made synchronously go on as they are, untracked; the
        // network stream stays the transport's.
        private sealed class WrittenStream : System.IO.Stream
        {
            private readonly System.IO.Stream network;
            private readonly ObservedRequest observed;
            private readonly RequestRecorder recorder;
            private readonly Func<Task> hold;

            internal WrittenStream(System.IO.Stream network, ObservedRequest observed, RequestRecorder recorder, Func<Task> hold)
            {
                this.network = network;
                this.observed = observed;
                this.recorder = recorder;
                this.hold = hold;
            }

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => network.CanWrite;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return WrittenAsync(cancellationToken, () => network.WriteAsync(buffer, offset, count, cancellationToken));
            }

            public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                return new ValueTask(WrittenAsync(cancellationToken, () => network.WriteAsync(buffer, cancellationToken).AsTask()));
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                network.Write(buffer, offset, count);
            }

            public override void Write(ReadOnlySpan<byte> buffer)
            {
                network.Write(buffer);
            }

            public override Task FlushAsync(CancellationToken cancellationToken)
            {
                return network.FlushAsync(cancellationToken);
            }

            public override void Flush()
            {
                network.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, System.IO.SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            // The write WRITE makes with TOKEN, tracked from before it is made.
            private async Task WrittenAsync(CancellationToken token, Func<Task> write)
            {
                TrackedTask tracked = recorder.Writing(observed, token);
                Task written;
                try
                {
                    written = write();
                }
                catch (Exception e)
                {
                    written = Task.FromException(e);
                }
                recorder.Tracked(observed, tracked, written);
                try
                {
                    await written.ConfigureAwait(false);
                }
                catch (Exception) when (hold != null)
                {
                    Task held = hold();
                    if (held != null)
                    {
                        await held.ConfigureAwait(false);
                    }
                    throw;
                }
            }
        }
    }

    // One request the simulator journaled (simulation.rb#step): its sequence and epoch, method, raw path and the operation
    // it was routed to, its status, the identities it carried, the upload and part a transfer used, and the fault rule
    // that changed it, if any.
    public sealed class JournalEntry
    {
        // What only a fault rule adds to an entry besides its fault_id: fault_kind for any kind but error,
        // fault_unavailable for a rule that could not apply, and a held request's hold_ms, held_ms and hold_ended.
        private static readonly string[] FaultFields = { "fault_kind", "fault_unavailable", "hold_ms", "held_ms", "hold_ended" };

        internal JournalEntry(long seq, string method, string path, string operation, int status, JsonNode credentials, string userAgent, long? epoch, long? upload, long? part, string fault)
        {
            Seq = seq;
            Method = method;
            Path = path;
            Operation = operation;
            Status = status;
            Credentials = credentials;
            UserAgent = userAgent;
            Epoch = epoch;
            Upload = upload;
            Part = part;
            Fault = fault;
        }

        public long Seq { get; }

        public string Method { get; }

        public string Path { get; }

        public string Operation { get; }

        public int Status { get; }

        public JsonNode Credentials { get; }

        public string UserAgent { get; }

        public long? Epoch { get; }

        // The upload a begin_upload, part or completion used, and a part's number.
        public long? Upload { get; }

        public long? Part { get; }

        // The fault the entry records, as its fields show it, or null when no rule changed the request.
        public string Fault { get; }

        internal static JournalEntry Parse(JsonNode node, int index, List<string> problems)
        {
            if (node is not JsonObject raw)
            {
                problems.Add($"journal entry {index} is not an object");
                return null;
            }
            long? seq = JsonRead.Integer(raw, "seq");
            string method = JsonRead.Text(raw, "method");
            string path = JsonRead.Text(raw, "path");
            long? status = JsonRead.Integer(raw, "status");
            if (seq == null || method == null || path == null || status == null)
            {
                problems.Add($"journal entry {index} lacks its seq, method, path or status");
                return null;
            }
            // Every entry names fault_id, null when no rule changed the request.
            var faults = new List<string>();
            if (!raw.TryGetPropertyValue("fault_id", out JsonNode faultId))
            {
                problems.Add($"journal seq {seq} names no fault_id, which every simulator journal entry holds");
            }
            else if (faultId != null)
            {
                faults.Add($"fault_id {faultId.ToJsonString()}");
            }
            foreach (string field in FaultFields)
            {
                if (raw.TryGetPropertyValue(field, out JsonNode value))
                {
                    faults.Add($"{field} {value?.ToJsonString() ?? "null"}");
                }
            }
            raw.TryGetPropertyValue("credentials", out JsonNode credentials);
            return new JournalEntry(seq.Value, method.ToUpperInvariant(), path, JsonRead.Text(raw, "operation"), (int)status.Value, credentials?.DeepClone(), JsonRead.Text(raw, "user_agent"),
                JsonRead.Integer(raw, "epoch"), JsonRead.Integer(raw, "upload"), JsonRead.Integer(raw, "part"), faults.Count == 0 ? null : string.Join(", ", faults));
        }
    }

    // API routes as the schema and the definition write them ("METHOD /path/{name}"), and the request paths they match.
    public static class ApiRoutes
    {
        public const string ApiPrefix = "/api/rest/v1";

        public static string MethodOf(string route)
        {
            return route.Substring(0, route.IndexOf(' '));
        }

        public static string TemplateOf(string route)
        {
            return route.Substring(route.IndexOf(' ') + 1);
        }

        // The values of TEMPLATE's parameters in a request's path, or null when the path is not of the template. The path
        // is unescaped once, as the simulator decodes a route's values; {path} takes the rest of it, slashes included,
        // and any other parameter one segment (simulation.rb's routes).
        public static Dictionary<string, string> ValuesIn(string template, string requestPath)
        {
            var names = new List<string>();
            string pattern = Regex.Replace(Regex.Escape(template), @"\\\{(\w+)}", parameter =>
            {
                names.Add(parameter.Groups[1].Value);
                return parameter.Groups[1].Value == "path" ? "(.*)" : "([^/]+)";
            });
            var found = Regex.Match(Uri.UnescapeDataString(requestPath ?? ""), "^" + pattern + "$", RegexOptions.Singleline);
            if (!found.Success)
            {
                return null;
            }
            var values = new Dictionary<string, string>(StringComparer.Ordinal);
            for (int index = 0; index < names.Count; index++)
            {
                values[names[index]] = found.Groups[index + 1].Value;
            }
            return values;
        }

        // Null when REQUEST is an API request of ROUTE whose path parameters have VALUES, or why not. A path is compared
        // as Files.com names one, without slashes at either end, so "/" names the root, as the simulator's api_scenarios
        // profile takes it (profile.rb).
        public static string Mismatch(ObservedRequest request, string route, IReadOnlyDictionary<string, string> values)
        {
            Dictionary<string, string> actual = request.Api && request.Method == MethodOf(route) ? ValuesIn(ApiPrefix + TemplateOf(route), request.Path) : null;
            if (actual != null && values.All(value => actual.TryGetValue(value.Key, out string found) && SameValue(value.Key, found, value.Value)))
            {
                return null;
            }
            string expected = string.Join(", ", values.Select(value => $"{value.Key} {Shown.Of(value.Value)}"));
            return $"request {request.Sequence} was {request.Method} {(request.Api ? request.Path : "to storage")}, not {route} with {expected}";
        }

        public static bool SameValue(string name, string actual, string expected)
        {
            return name == "path" ? actual.Trim('/') == (expected ?? "").Trim('/') : actual == expected;
        }
    }

    // The operations the simulator's live readiness lists (simulation.rb#readiness): each API operation's ID, method and
    // path template, and each transfer operation's ID and method. Its journal names the operation it routed each request
    // to by these IDs.
    public sealed class SimulatorOperations
    {
        private readonly List<(string Id, string Method, string Template)> api = new List<(string Id, string Method, string Template)>();
        private readonly List<(string Id, string Method)> transfers = new List<(string Id, string Method)>();

        public static SimulatorOperations From(JsonObject ready, List<string> problems)
        {
            var operations = new SimulatorOperations();
            if (ready == null)
            {
                return operations;
            }
            if (JsonRead.At(ready, "operations") is JsonArray listed)
            {
                foreach (JsonNode operation in listed)
                {
                    string id = JsonRead.Text(operation, "id");
                    string method = JsonRead.Text(operation, "method");
                    string path = JsonRead.Text(operation, "path");
                    if (id == null || method == null || path == null)
                    {
                        problems.Add("the readiness the simulator answers now lists an operation without its id, method or path");
                        continue;
                    }
                    operations.api.Add((id, method.ToUpperInvariant(), path));
                }
            }
            else
            {
                problems.Add("the readiness the simulator answers now lists no operations");
            }
            if (JsonRead.At(ready, "transfers", "operations") is JsonArray moved)
            {
                foreach (JsonNode operation in moved)
                {
                    string id = JsonRead.Text(operation, "id");
                    string method = JsonRead.Text(operation, "method");
                    if (id == null || method == null)
                    {
                        problems.Add("the readiness the simulator answers now lists a transfer operation without its id or method");
                        continue;
                    }
                    operations.transfers.Add((id, method.ToUpperInvariant()));
                }
            }
            else
            {
                problems.Add("the readiness the simulator answers now lists no transfer operations");
            }
            return operations;
        }

        // The ID of the API operation ROUTE names, or null unless the simulator lists exactly one.
        public string Of(string route)
        {
            List<string> found = api.Where(operation => operation.Method == ApiRoutes.MethodOf(route) && operation.Template == ApiRoutes.ApiPrefix + ApiRoutes.TemplateOf(route))
                .Select(operation => operation.Id).Distinct().ToList();
            return found.Count == 1 ? found[0] : null;
        }

        // The transfer operation that stores an upload's part, the one that PUTs, or null unless there is exactly one.
        public string UploadPart
        {
            get
            {
                List<string> found = transfers.Where(operation => operation.Method == "PUT").Select(operation => operation.Id).Distinct().ToList();
                return found.Count == 1 ? found[0] : null;
            }
        }

        // The operations REQUEST can have been routed to: the API operations of its method whose path template its path
        // matches, or the transfer operations of its method.
        public IReadOnlyList<string> Candidates(ObservedRequest request)
        {
            return request.Api
                ? api.Where(operation => operation.Method == request.Method && ApiRoutes.ValuesIn(operation.Template, request.Path) != null).Select(operation => operation.Id).Distinct().ToList()
                : transfers.Where(operation => operation.Method == request.Method).Select(operation => operation.Id).Distinct().ToList();
        }

        // REQUEST's {path} value under the API operation ID, or null when that operation's route has none.
        public string PathValue(string id, ObservedRequest request)
        {
            (string Id, string Method, string Template) operation = api.FirstOrDefault(candidate => candidate.Id == id);
            Dictionary<string, string> values = operation.Template == null ? null : ApiRoutes.ValuesIn(operation.Template, request.Path);
            return values != null && values.TryGetValue("path", out string path) ? path : null;
        }
    }

    // What every read of a case's journal is held to: the reset's epoch, the launcher's instance and the case's definition
    // (which the journal echoes under the api_scenarios profile), each request routed to an operation the simulator lists
    // for it, and, for each request a position or the cleanup has bound to its route, the operation it was meant to be.
    // UploadPartSize is the one partsize live readiness grants every part with (ScenarioBinding.UploadPartSize).
    public sealed class JournalContract
    {
        public JournalContract(long epoch, string instance, string definitionSha256, SimulatorOperations operations, long? uploadPartSize)
        {
            Epoch = epoch;
            Instance = instance;
            DefinitionSha256 = definitionSha256;
            Operations = operations;
            UploadPartSize = uploadPartSize;
        }

        public long Epoch { get; }

        public string Instance { get; }

        public string DefinitionSha256 { get; }

        public SimulatorOperations Operations { get; }

        public long? UploadPartSize { get; }

        // The operation each bound request was meant to be, by its sequence.
        public Dictionary<int, string> Intended { get; } = new Dictionary<int, string>();

        // The requests of the case's own uploads that their first-ACK cancellation ended unanswered, by sequence.
        public Dictionary<int, CancelledAttempt> Cancelled { get; } = new Dictionary<int, CancelledAttempt>();
    }

    // A request of an upload the case cancelled at its first acknowledged part, which that cancellation ended before an
    // answer came (UploadEvidence.Settle): a part (Storage, the part operation) or the begin_upload that negotiated a part
    // (the begin_upload operation), with the part number, the upload ref (by its SHA-256) and, for a part, the origin and
    // the upload path (UploadEvidence.UploadPathOf) it was sent to. A part the SDK had already been granted, which reached
    // the transport's bottom only after the caller cancelled, also names that grant (GrantedBy). Its client status stays
    // null, apart from any status the server journaled. It may be joined only to an entry of its own operation (a part's
    // of its own part number), of Upload once the journal has shown which upload the step began, and it may stay without
    // an entry only once Confirmed as that upload's (UploadEvidence.ConfirmCancelledAttempts); once Rejected, it is
    // accounted for as any unanswered request.
    public sealed class CancelledAttempt
    {
        public CancelledAttempt(int sequence, string operation, bool storage, long part, string refSha256, string origin, string uploadPath = null, int? grantedBy = null)
        {
            Sequence = sequence;
            Operation = operation;
            Storage = storage;
            Part = part;
            RefSha256 = refSha256;
            Origin = origin;
            UploadPath = uploadPath;
            GrantedBy = grantedBy;
        }

        public int Sequence { get; }

        public string Operation { get; }

        public bool Storage { get; }

        public long Part { get; }

        public string RefSha256 { get; }

        public string Origin { get; }

        // A part's path without its ref and part number, such as /__files_mock/transfer/upload; null for a begin_upload.
        public string UploadPath { get; }

        // For a part that reached the transport's bottom only after the caller cancelled, the sequence of the
        // begin_upload that negotiated that very part and was answered before the caller cancelled; null otherwise.
        public int? GrantedBy { get; }

        public bool Late => GrantedBy != null;

        public long? Upload { get; internal set; }

        public bool Confirmed { get; internal set; }

        // Why the journal does not show it as the step's upload's, once it does not.
        public string Rejected { get; internal set; }
    }

    // A journal and how the SDK's requests account for its entries.
    public sealed class JournalAccount
    {
        public bool? Complete { get; internal set; }

        public long? Dropped { get; internal set; }

        public long? Epoch { get; internal set; }

        public long BaselineSeq { get; internal set; }

        public bool Final { get; internal set; }

        public IReadOnlyList<JournalEntry> Entries { get; internal set; } = Array.Empty<JournalEntry>();

        public List<(ObservedRequest Request, JournalEntry Entry)> Joined { get; } = new List<(ObservedRequest Request, JournalEntry Entry)>();

        // The confirmed cancelled attempts the final join found without an entry (CancelledAttempt).
        public List<ObservedRequest> Excused { get; } = new List<ObservedRequest>();

        public List<string> Problems { get; } = new List<string>();
    }

    // Joins the SDK's requests to the simulator's journal one to one: each entry the case's requests made (seq above the
    // baseline the case started from) to the earliest request not yet joined with its method and path whose own answer
    // has the entry's status. A request that got no answer is joined only as one of the case's cancelled upload attempts
    // (CancelledAttempt), to an entry of its own operation and upload, and keeps its null status apart from the status
    // the server journaled. The journal must be of the reset's epoch, the launcher's instance and the case's definition; an
    // entry of that epoch, with no fault, of an operation the simulator lists for its request, and of the operation its
    // position meant it to be; an API entry with the client's API key and the SDK's User-Agent, a storage entry with no
    // credential (and its request with neither the key nor any Authorization). Every entry must be joined whenever the
    // journal is read, and every request by the final join, which reads it after the case's last request, except a
    // confirmed cancelled attempt that reached no entry.
    public static class JournalJoin
    {
        private static readonly Regex SdkUserAgent = new Regex(@"^Files\.com DOTNET SDK v\S+$");

        public static long LastSeq(JsonObject journal)
        {
            return (JsonRead.At(journal, "entries") as JsonArray)?.Select(entry => JsonRead.Integer(entry, "seq") ?? 0).DefaultIfEmpty(0).Max() ?? 0;
        }

        public static JournalAccount Join(JsonObject journal, long baselineSeq, JournalContract contract, IReadOnlyList<ObservedRequest> observed, bool final)
        {
            var account = new JournalAccount { BaselineSeq = baselineSeq, Final = final };
            if (journal == null)
            {
                account.Problems.Add("no journal was read");
                return account;
            }
            account.Epoch = JsonRead.Integer(journal, "epoch");
            if (account.Epoch != contract.Epoch)
            {
                account.Problems.Add($"the journal is of epoch {Shown.Of(account.Epoch)}, not the reset's {contract.Epoch}");
            }
            if (JsonRead.Text(journal, "instance") != contract.Instance || contract.Instance == null)
            {
                account.Problems.Add($"the journal is of instance {Shown.Of(JsonRead.Text(journal, "instance"))}, not the launcher's {Shown.Of(contract.Instance)}");
            }
            string identity = ScenarioBinding.ApiScenariosMismatch(JsonRead.At(journal, "api_scenarios"), contract.DefinitionSha256);
            if (identity != null)
            {
                account.Problems.Add($"the journal's api_scenarios {identity}");
            }
            account.Complete = JsonRead.Flag(journal, "complete");
            account.Dropped = JsonRead.Integer(journal, "dropped");
            if (account.Complete != true)
            {
                account.Problems.Add("the journal is not complete");
            }
            if (account.Dropped != 0)
            {
                account.Problems.Add($"the journal dropped {Shown.Of(account.Dropped)} requests");
            }
            if (!(JsonRead.At(journal, "entries") is JsonArray all))
            {
                account.Problems.Add("the journal has no entries list");
                return account;
            }
            var entries = new List<JournalEntry>();
            for (int index = 0; index < all.Count; index++)
            {
                JournalEntry entry = JournalEntry.Parse(all[index], index, account.Problems);
                if (entry != null && entry.Seq > baselineSeq)
                {
                    entries.Add(entry);
                }
            }
            entries.Sort((left, right) => left.Seq.CompareTo(right.Seq));
            for (int index = 1; index < entries.Count; index++)
            {
                if (entries[index].Seq == entries[index - 1].Seq)
                {
                    account.Problems.Add($"journal seq {entries[index].Seq} appears twice");
                }
            }
            account.Entries = entries;

            var unjoined = observed.ToList();
            foreach (JournalEntry entry in entries)
            {
                List<ObservedRequest> candidates = unjoined.Where(request => request.Method == entry.Method && SamePath(request.Path, entry.Path)).ToList();
                ObservedRequest request = candidates.FirstOrDefault(candidate => candidate.Status == entry.Status || (candidate.Status == null && CancelledAttemptOf(contract, candidate, entry)));
                if (request == null)
                {
                    account.Problems.Add(candidates.Count == 0
                        ? $"journal seq {entry.Seq} ({entry.Method} {entry.Path}, {entry.Status}) is no request the SDK sent"
                        : $"journal seq {entry.Seq} ({entry.Method} {entry.Path}) answered {entry.Status}, but the SDK's request {candidates[0].Sequence} received {candidates[0].Outcome}");
                    continue;
                }
                unjoined.Remove(request);
                account.Joined.Add((request, entry));
                string where = $"journal seq {entry.Seq} (request {request.Sequence}, {entry.Method} {entry.Path})";
                if (entry.Epoch != contract.Epoch)
                {
                    account.Problems.Add($"{where} is of epoch {Shown.Of(entry.Epoch)}, not the reset's {contract.Epoch}");
                }
                if (entry.Fault != null)
                {
                    account.Problems.Add($"{where} records the fault {entry.Fault}, which the canonical preset injects none of");
                }
                IReadOnlyList<string> routed = contract.Operations.Candidates(request);
                if (entry.Operation == null || !routed.Contains(entry.Operation))
                {
                    account.Problems.Add($"{where} is the operation {Shown.Of(entry.Operation)}, which is not what the simulator lists for {request.Method} {(request.Api ? request.Path : "to storage")} ({string.Join(", ", routed)})");
                }
                else if (contract.Intended.TryGetValue(request.Sequence, out string intended) && entry.Operation != intended)
                {
                    account.Problems.Add($"{where} is the operation {entry.Operation}, not {Shown.Of(intended)}, the one its position meant to send");
                }
                if (request.Api)
                {
                    if (!JsonNode.DeepEquals(entry.Credentials, new JsonObject { ["api_key"] = 1 }))
                    {
                        account.Problems.Add($"{where} carried the credentials {entry.Credentials?.ToJsonString() ?? "null"}, not the client's API key alone");
                    }
                    if (!request.CarriesApiKey || !request.ApiKeyMatches)
                    {
                        account.Problems.Add($"{where}: the SDK's request did not carry the client's API key");
                    }
                    if (!SdkUserAgent.IsMatch(entry.UserAgent ?? ""))
                    {
                        account.Problems.Add($"{where} carried the User-Agent {Shown.Of(entry.UserAgent)}, not the SDK's");
                    }
                }
                else
                {
                    if (!(entry.Credentials == null || (entry.Credentials is JsonObject credentials && credentials.Count == 0)))
                    {
                        account.Problems.Add($"{where} carried the credentials {entry.Credentials.ToJsonString()} to storage");
                    }
                    if (request.CarriesApiKey || request.CarriesAuthorization)
                    {
                        account.Problems.Add($"{where}: the SDK's request carried a credential to storage");
                    }
                }
            }
            if (final)
            {
                foreach (ObservedRequest request in unjoined)
                {
                    if (request.Status == null && contract.Cancelled.TryGetValue(request.Sequence, out CancelledAttempt attempt) && attempt.Confirmed)
                    {
                        account.Excused.Add(request);
                        continue;
                    }
                    account.Problems.Add($"the SDK's request {request.Sequence} ({request.Method} {request.Path}, {request.Outcome}) has no journal entry");
                }
            }
            return account;
        }

        // Whether ENTRY may stand for REQUEST, which got no answer: only as one of the case's cancelled attempts not
        // rejected, and only an entry of the attempt's own operation (a part's of its own part number), of its upload once
        // that is known.
        private static bool CancelledAttemptOf(JournalContract contract, ObservedRequest request, JournalEntry entry)
        {
            return contract.Cancelled.TryGetValue(request.Sequence, out CancelledAttempt attempt) && attempt.Rejected == null && entry.Operation != null && entry.Operation == attempt.Operation
                && (attempt.Upload == null || entry.Upload == attempt.Upload) && (!attempt.Storage || entry.Part == attempt.Part);
        }

        private static bool SamePath(string observed, string journaled)
        {
            int query = journaled.IndexOf('?');
            if (query >= 0)
            {
                journaled = journaled.Substring(0, query);
            }
            return Uri.UnescapeDataString(observed) == Uri.UnescapeDataString(journaled);
        }
    }

    // The caller's cancellation of an upload at its first acknowledged part: it cancels at the first progress report
    // that shows a part the server accepted, and only once, keeping which of the upload's storage requests had been
    // answered 2xx by then. That list is taken inside the report, before the cancellation: a request answered later is
    // never evidence of an acknowledgement the cancellation came after. CancelEvent marks only the caller's intent, its
    // place in the recorder's order of events taken before the caller cancels its token: a request that ended before it
    // was not ended by it, but one that ended after it was not necessarily ended by the caller either, since the actual
    // cancellation comes later. Whether the caller's own token had actually been cancelled is what each request reads for
    // itself (ObservedRequest.OwnerCancelledAtBegin, OwnerCancelledWithToken and OwnerCancelledAtEnd). OwnerCancelledAt
    // marks the actual cancellation, taken with what of the upload's requests had already failed and the token the
    // upload's bodies wrote with (RequestRecorder.CancellingOwner) before it is carried to the SDK (Forward), and
    // BodyTokenCancelledByOwner whether it is what cancelled that token.
    public sealed class CancellationGate
    {
        private readonly object sync = new object();
        // The thread carrying the caller's cancellation to the SDK, while it does (Forward), and whether it has.
        private int forwarding;
        private bool forwarded;

        public bool Cancelled { get; private set; }

        public long AcknowledgedBytes { get; private set; }

        public int PartsAcknowledged { get; private set; }

        // How many requests the client had started when it cancelled.
        public int RequestsBegun { get; private set; }

        // The upload's storage requests already answered 2xx when it cancelled, by sequence.
        public IReadOnlyList<int> AnsweredStorage { get; private set; } = Array.Empty<int>();

        public long? CancelEvent { get; private set; }

        // The event at which the caller actually cancelled (RequestRecorder.CancellingOwner): after its intent and after
        // anything a control does in between (SynchronousProgress.BeforeCancel), but before its cancellation reaches the
        // token the SDK was given (Forward), and so before it reaches any token of the SDK's. Null until then.
        public long? OwnerCancelledAt { get; private set; }

        // The token the SDK's bodies of the upload's parts wrote with, the token itself, as every write of the upload's
        // storage requests that began before the caller actually cancelled showed it (RequestRecorder.Writing): the
        // upload's own, which UploadCoordinator gives every part's content, PartContent writes with and WatchedContent
        // passes on to the network. Null when those writes showed none, or more than one.
        internal CancellationToken? BodyToken { get; private set; }

        // The requests whose writes showed BodyToken before the caller actually cancelled, by sequence.
        public IReadOnlyList<int> BodyTokenWitnesses { get; private set; } = Array.Empty<int>();

        // Whether BodyToken had been cancelled before the caller's cancellation was carried to the SDK: by something
        // else, such as a failure of the upload.
        public bool BodyTokenCancelledBeforeOwner { get; private set; }

        // Whether the caller's cancellation is what cancelled BodyToken: true when it was cancelled on the thread carrying
        // that cancellation to the SDK, while it did (Forward), as a token linked to the one the SDK was given is, within
        // that token's own cancellation (a CancellationTokenSource runs its token's callbacks, and its linked sources', on
        // the thread that cancels it); false when something else cancelled it after the caller actually cancelled; null
        // while it was not cancelled.
        public bool? BodyTokenCancelledByOwner { get; private set; }

        // Whether to cancel now, as the event CANCELEVENT.
        public bool Observe(long acknowledgedBytes, int partsAcknowledged, int requestsBegun, IReadOnlyList<int> answeredStorage, long cancelEvent)
        {
            if (Cancelled || partsAcknowledged < 1 || acknowledgedBytes <= 0)
            {
                return false;
            }
            Cancelled = true;
            AcknowledgedBytes = acknowledgedBytes;
            PartsAcknowledged = partsAcknowledged;
            RequestsBegun = requestsBegun;
            AnsweredStorage = answeredStorage.ToArray();
            CancelEvent = cancelEvent;
            return true;
        }

        // The caller actually cancels now, as the event AT, before its cancellation is carried to the SDK (Forward). TOKEN
        // is the one the upload's bodies wrote with, which the writes of WITNESSES showed, or null. A token already
        // cancelled is read at once, as cancelled before the caller; its callback ends with its cancellation.
        internal void OwnerCancelling(long at, CancellationToken? token, IReadOnlyList<int> witnesses)
        {
            lock (sync)
            {
                OwnerCancelledAt = at;
                BodyToken = token;
                BodyTokenWitnesses = witnesses;
            }
            if (token is CancellationToken body)
            {
                body.Register(BodyTokenCancelled);
            }
        }

        // Carries the caller's cancellation to the SDK, on this thread: cancels CALLER, whose token the SDK was given.
        internal void Forward(CancellationTokenSource caller)
        {
            lock (sync)
            {
                forwarding = Environment.CurrentManagedThreadId;
            }
            try
            {
                caller.Cancel();
            }
            finally
            {
                lock (sync)
                {
                    forwarding = 0;
                    forwarded = true;
                }
            }
        }

        private void BodyTokenCancelled()
        {
            lock (sync)
            {
                if (forwarding == Environment.CurrentManagedThreadId)
                {
                    BodyTokenCancelledByOwner = true;
                }
                else if (forwarding == 0 && !forwarded)
                {
                    BodyTokenCancelledBeforeOwner = true;
                }
                else
                {
                    BodyTokenCancelledByOwner = false;
                }
            }
        }
    }

    // A progress report's acknowledged fields, and how many requests the client had started then.
    public sealed class ProgressReport
    {
        public ProgressReport(long totalBytes, bool totalBytesKnown, long acknowledgedBytes, int partsAcknowledged, int requestsBegun)
        {
            TotalBytes = totalBytes;
            TotalBytesKnown = totalBytesKnown;
            AcknowledgedBytes = acknowledgedBytes;
            PartsAcknowledged = partsAcknowledged;
            RequestsBegun = requestsBegun;
        }

        public long TotalBytes { get; }

        public bool TotalBytesKnown { get; }

        public long AcknowledgedBytes { get; }

        public int PartsAcknowledged { get; }

        public int RequestsBegun { get; }
    }

    // UploadOptions.Progress that keeps each report and, with a gate, cancels the caller's token inside the report that
    // shows the first acknowledged part, having first read which of the upload's storage requests (those from the
    // recorder's request FIRST on) had already been answered 2xx, and then taken the caller's actual cancellation, with
    // what of those requests had already failed and the token the upload's bodies wrote with, before carrying it to the
    // SDK (RequestRecorder.CancellingOwner, CancellationGate.Forward). The upload calls it on its own threads, one call at a
    // time, after a part's answer has come through the transport (UploadCoordinator.SendPartAsync); it posts nothing
    // elsewhere, as Progress<T> would.
    internal sealed class SynchronousProgress : IProgress<TransferProgress>
    {
        private readonly object gate = new object();
        private readonly List<ProgressReport> reports = new List<ProgressReport>();
        private readonly RequestRecorder recorder;
        private readonly int first;
        private readonly CancellationGate cancellation;
        private readonly CancellationTokenSource caller;

        internal SynchronousProgress(RequestRecorder recorder, int first, CancellationGate cancellation, CancellationTokenSource caller)
        {
            this.recorder = recorder;
            this.first = first;
            this.cancellation = cancellation;
            this.caller = caller;
        }

        internal IReadOnlyList<ProgressReport> Reports
        {
            get { lock (gate) { return reports.ToList(); } }
        }

        // A cancellation callback's own failure while the token was cancelled, if any.
        internal string CancelFailure { get; private set; }

        // Runs once the caller's intent has taken its place (CancellationGate.CancelEvent) and before the caller's token is
        // actually cancelled; null in a scenario. A control sets it to end a request some other way in between, as a network
        // wait running out there would.
        internal Action BeforeCancel { get; set; }

        public void Report(TransferProgress value)
        {
            Observe(value.TotalBytes, value.TotalBytesKnown, value.AcknowledgedBytes, value.PartsAcknowledged);
        }

        // One report's acknowledged fields, as Report receives them.
        internal void Observe(long totalBytes, bool totalBytesKnown, long acknowledgedBytes, int partsAcknowledged)
        {
            var report = new ProgressReport(totalBytes, totalBytesKnown, acknowledgedBytes, partsAcknowledged, recorder.Count);
            lock (gate)
            {
                reports.Add(report);
            }
            if (cancellation == null || cancellation.Cancelled)
            {
                return;
            }
            int[] answered = recorder.Since(first).Where(request => !request.Api && request.Succeeded).Select(request => request.Sequence).ToArray();
            // The caller's intent, taken before the token is cancelled, so every request the cancellation ends ends after
            // it. Whether the token had actually been cancelled is what each request reads for itself, as it ends.
            long cancelling = recorder.NextEvent();
            if (cancellation.Observe(report.AcknowledgedBytes, report.PartsAcknowledged, report.RequestsBegun, answered, cancelling))
            {
                // Between the intent and the actual cancellation, where a control holds the caller (BeforeCancel).
                BeforeCancel?.Invoke();
                // The actual cancellation takes its place first, with what of the upload's requests had already failed and
                // the token the upload's bodies wrote with; only then is it carried to the SDK.
                recorder.CancellingOwner(first, cancellation);
                try
                {
                    cancellation.Forward(caller);
                }
                catch (AggregateException e)
                {
                    CancelFailure = Diagnostics.Describe(e);
                }
            }
        }

        // The last report must acknowledge every byte of a known length, and no report fewer bytes than the one before.
        internal List<string> FinalProblems(long length)
        {
            var problems = new List<string>();
            IReadOnlyList<ProgressReport> all = Reports;
            ProgressReport last = all.LastOrDefault();
            if (last == null)
            {
                problems.Add("no progress was reported");
                return problems;
            }
            if (!last.TotalBytesKnown || last.TotalBytes != length || last.AcknowledgedBytes != length)
            {
                problems.Add($"the final progress acknowledged {last.AcknowledgedBytes} of {(last.TotalBytesKnown ? last.TotalBytes.ToString(CultureInfo.InvariantCulture) : "an unknown number of")} bytes, not {length} of {length}");
            }
            for (int index = 1; index < all.Count; index++)
            {
                if (all[index].AcknowledgedBytes < all[index - 1].AcknowledgedBytes)
                {
                    problems.Add($"progress report {index + 1} acknowledged fewer bytes than the one before");
                }
            }
            return problems;
        }

        internal JsonObject Evidence()
        {
            IReadOnlyList<ProgressReport> all = Reports;
            ProgressReport last = all.LastOrDefault();
            return new JsonObject
            {
                ["reports"] = all.Count,
                ["final"] = last == null ? null : new JsonObject
                {
                    ["total_bytes"] = last.TotalBytes,
                    ["total_bytes_known"] = last.TotalBytesKnown,
                    ["acknowledged_bytes"] = last.AcknowledgedBytes,
                    ["parts_acknowledged"] = last.PartsAcknowledged,
                },
            };
        }
    }

    // What the journal shows of one upload step (UploadEvidence.Read): the upload Files.com numbered for the step's
    // begin_upload, the distinct parts of that upload storage accepted, by number, the step's storage requests answered 2xx
    // that stored them, and whatever in the step is not that upload's.
    public sealed class UploadJournal
    {
        public long? Upload { get; internal set; }

        public SortedSet<long> StoredParts { get; } = new SortedSet<long>();

        // Part numbers by the sequence of the request, answered 2xx, whose entry stored that part of Upload.
        public Dictionary<int, long> PartRequests { get; } = new Dictionary<int, long>();

        public List<string> Problems { get; } = new List<string>();
    }

    // What Settle found of an upload step's requests that got no answer: the attempts its own cancellation ended, and an
    // exact account of every other.
    public sealed class UnansweredRequests
    {
        public List<CancelledAttempt> Attempts { get; } = new List<CancelledAttempt>();

        public List<string> Problems { get; } = new List<string>();
    }

    // What the wire and the journal show of an upload: a completed one's 2xx completion of its own upload and its distinct
    // stored parts; a cancelled one's part of its own upload answered 2xx before the cancellation, no completion request
    // at all, and only its own pending requests ended unanswered by the cancellation.
    public static class UploadEvidence
    {
        // How a request the caller's cancellation ended fails at the bottom of the transport: as the cancellation itself,
        // or as the HttpRequestException a connection ended under a cancelled token can surface as
        // (UploadCoordinator.SendWithRetriesAsync). The name alone never makes an attempt (Settle).
        private static readonly string[] CancelledFailures = { nameof(OperationCanceledException), nameof(TaskCanceledException), nameof(HttpRequestException) };

        public static bool IsCompletion(ObservedRequest request, string completionRoute, string remotePath)
        {
            return ApiRoutes.Mismatch(request, completionRoute, new Dictionary<string, string> { ["path"] = remotePath }) == null;
        }

        // The upload REQUESTS (one step's) made, by the entries ACCOUNT joined them to: the step's begin_upload requests
        // must name one upload, the grant every part and completion of the step must belong to; every storage request must
        // be a part of that upload, and every API request a begin_upload or the completion of REMOTEPATH. Parts are counted
        // by their upload and part numbers, never by their URLs, so a part sent again on a renewed URL counts once.
        public static UploadJournal Read(IReadOnlyList<ObservedRequest> requests, JournalAccount account, SimulatorOperations operations, string beginRoute, string completionRoute, string remotePath)
        {
            var upload = new UploadJournal();
            Dictionary<int, JournalEntry> joined = account.Joined.ToDictionary(pair => pair.Request.Sequence, pair => pair.Entry);
            var path = new Dictionary<string, string> { ["path"] = remotePath };
            var begun = new SortedSet<long>();
            foreach (ObservedRequest request in requests.Where(request => request.Api))
            {
                joined.TryGetValue(request.Sequence, out JournalEntry entry);
                if (ApiRoutes.Mismatch(request, beginRoute, path) == null)
                {
                    if (entry?.Upload is long number)
                    {
                        begun.Add(number);
                    }
                    else if (request.Succeeded)
                    {
                        upload.Problems.Add($"request {request.Sequence} began the upload, but no journal entry names the upload it began");
                    }
                }
                else if (ApiRoutes.Mismatch(request, completionRoute, path) != null)
                {
                    upload.Problems.Add($"request {request.Sequence} ({request.Method} {request.Path}) is neither a begin_upload nor the completion of {remotePath}");
                }
            }
            if (begun.Count != 1)
            {
                upload.Problems.Add(begun.Count == 0
                    ? $"no begin_upload of {remotePath} was journaled with the upload it began"
                    : $"the step's begin_upload requests began the uploads {string.Join(", ", begun)}, not one");
                return upload;
            }
            upload.Upload = begun.Min;
            foreach (ObservedRequest request in requests)
            {
                if (!joined.TryGetValue(request.Sequence, out JournalEntry entry))
                {
                    continue;
                }
                if (!request.Api)
                {
                    if (entry.Operation != operations.UploadPart || entry.Upload != upload.Upload || entry.Part == null)
                    {
                        string stored = entry.Operation == operations.UploadPart ? $"part {Shown.Of(entry.Part)} of upload {Shown.Of(entry.Upload)}" : $"through {Shown.Of(entry.Operation)}";
                        upload.Problems.Add($"request {request.Sequence} stored {stored}, not a part of this step's upload {upload.Upload}");
                    }
                    else if (request.Succeeded && entry.Status >= 200 && entry.Status < 300)
                    {
                        upload.StoredParts.Add(entry.Part.Value);
                        upload.PartRequests[request.Sequence] = entry.Part.Value;
                    }
                }
                else if (ApiRoutes.Mismatch(request, completionRoute, path) == null && entry.Upload != upload.Upload)
                {
                    upload.Problems.Add($"request {request.Sequence} completed upload {Shown.Of(entry.Upload)}, not this step's upload {upload.Upload}");
                }
            }
            return upload;
        }

        public static List<string> Completed(UploadJournal upload, IReadOnlyList<ObservedRequest> requests, string completionRoute, string remotePath, int minParts)
        {
            var problems = new List<string>(upload.Problems);
            if (!requests.Any(request => IsCompletion(request, completionRoute, remotePath) && request.Succeeded))
            {
                problems.Add($"no request completing {remotePath} was answered 2xx");
            }
            if (minParts > 0 && upload.StoredParts.Count < minParts)
            {
                problems.Add($"{upload.StoredParts.Count} distinct parts of upload {Shown.Of(upload.Upload)} were stored ([{string.Join(", ", upload.StoredParts)}]), fewer than {minParts}");
            }
            return problems;
        }

        public static List<string> Cancelled(bool completed, Exception raised, CancellationGate gate, UploadJournal upload, IReadOnlyList<ObservedRequest> requests, string completionRoute, string remotePath, long length)
        {
            var problems = new List<string>(upload.Problems);
            if (!gate.Cancelled)
            {
                problems.Add("no progress report showed an acknowledged part, so the upload was never cancelled");
            }
            else
            {
                if (gate.AcknowledgedBytes >= length)
                {
                    problems.Add("the cancellation came only once every byte was acknowledged");
                }
                if (!gate.AnsweredStorage.Any(upload.PartRequests.ContainsKey))
                {
                    problems.Add($"no part of upload {Shown.Of(upload.Upload)} had been answered 2xx when the upload was cancelled");
                }
            }
            if (completed)
            {
                problems.Add("the upload completed although it was cancelled");
            }
            else if (!(raised is OperationCanceledException))
            {
                problems.Add($"the upload ended with {(raised == null ? "nothing" : Diagnostics.Describe(raised))}, not as cancelled");
            }
            ObservedRequest completion = requests.FirstOrDefault(request => IsCompletion(request, completionRoute, remotePath));
            if (completion != null)
            {
                problems.Add($"request {completion.Sequence} asked Files.com to complete the cancelled upload ({completion.Outcome})");
            }
            return problems;
        }

        // Settles one upload step's requests once its task has ended, and with it every part's and every negotiation's
        // (UploadCoordinator.RunAsync ends them all before it returns): each is meant to be the step's begin_upload, one
        // of its parts or its completion (contract.Intended), and each that got no answer must be one of the attempts the
        // caller's own cancellation ended, or it is a problem, with its exact account. An attempt ended after GATE's
        // CancelEvent, the caller's intent, with the caller's own token actually cancelled by then; its own token, if it was
        // cancelled, was cancelled only once the caller's own token had been (ObservedRequest.OwnerCancelledWithToken); and
        // either that cancellation reached its transport while the transport's own task was still running
        // (ObservedRequest.TransportEndedBeforeToken), or, for a part, its body's writing ended with the cancellation of
        // the upload's own token, that very token (CancellationGate.BodyToken), which the caller's cancellation is what
        // cancelled (CancellationGate.BodyTokenCancelledByOwner), every write of the upload's bodies having been made with
        // it: the caller's cancellation of the upload can end a part's body, and with it the transport's task, before it
        // reaches the request's own token. Nothing of it had failed when the caller actually cancelled, before that
        // cancellation reached any token of the SDK's (CancellationGate.OwnerCancelledAt): not its transport, nor its
        // body's writing, nor a write of it (ObservedRequest.FailedBeforeOwner), even with its transport still running when
        // the cancellation of its own token reached it; and nothing of it begun before then, whose task was known only
        // after, had failed by then otherwise than with the cancellation of that token. So a request its network wait, or
        // anything else, ended while the caller's token was not yet cancelled stays a failure however soon the caller
        // cancels after, and however late its end is observed, and one whose transport had ended it before that relay
        // stays one unless its body shows the caller's cancellation ended it; it failed as a cancelled request fails, in
        // an upload that itself ended as cancelled; and it is either a part (a PUT to storage with no credential, of one of
        // the upload's parts: 1 to LENGTH's parts under the contract's partsize) or the begin_upload of REMOTEPATH that
        // negotiates such a part, with the client's API key and an upload ref, as PartPlanner negotiates the next part
        // while one is being sent. It reached the transport's bottom before the caller cancelled, or it is a part the
        // SDK had been granted before (NotLate): UploadCoordinator dispatches a granted part, past the token check of
        // FilesApi.SendUploadPart, as soon as the part before it ends, before that part's report reaches the caller, so
        // the part can reach the bottom only after the caller has cancelled. A begin_upload that reaches it after is
        // never an attempt. Each attempt is registered in the contract, to be confirmed as the step's upload's
        // (ConfirmCancelledAttempts).
        public static UnansweredRequests Settle(IReadOnlyList<ObservedRequest> requests, JournalContract contract, CancellationGate gate, Exception raised, string beginRoute, string completionRoute, string remotePath, long length)
        {
            var path = new Dictionary<string, string> { ["path"] = remotePath };
            string begin = contract.Operations.Of(beginRoute);
            foreach (ObservedRequest request in requests)
            {
                if (!request.Api)
                {
                    contract.Intended[request.Sequence] = contract.Operations.UploadPart;
                }
                else if (ApiRoutes.Mismatch(request, beginRoute, path) == null)
                {
                    contract.Intended[request.Sequence] = begin;
                }
                else if (IsCompletion(request, completionRoute, remotePath))
                {
                    contract.Intended[request.Sequence] = contract.Operations.Of(completionRoute);
                }
            }
            var unanswered = new UnansweredRequests();
            long? parts = contract.UploadPartSize > 0 ? Math.Max(1, (length + contract.UploadPartSize.Value - 1) / contract.UploadPartSize.Value) : (long?)null;
            foreach (ObservedRequest request in requests.Where(request => request.Status == null))
            {
                string why = NotAnAttempt(request, requests, gate, raised, beginRoute, path, parts, out int? grantedBy);
                if (why != null)
                {
                    unanswered.Problems.Add($"{Account(request, gate)} is not one of the cancelled upload's own attempts: {why}");
                    continue;
                }
                CancelledAttempt attempt = request.Api
                    ? new CancelledAttempt(request.Sequence, begin, false, request.BodyPart.Value, request.BodyRefSha256, null)
                    : new CancelledAttempt(request.Sequence, contract.Operations.UploadPart, true, PartOf(request.Path).Value, ScenarioDigest.Sha256(RefOf(request.Path)), request.Origin,
                        UploadPathOf(request.Path), grantedBy);
                contract.Cancelled[request.Sequence] = attempt;
                unanswered.Attempts.Add(attempt);
            }
            return unanswered;
        }

        // Binds each of ATTEMPTS to the step's upload, as the journal shows it. One joined to an entry must be of its own
        // operation and of that upload (a part's, of its own part number). One without an entry must carry the ref of a
        // part of that upload that storage acknowledged and the journal joined as that upload's (UploadJournal.PartRequests);
        // a part's must also have been sent to that part's origin and to its whole upload path (UploadPathOf), the path
        // under which that request, as it was actually sent, positively reached the simulator's part operation: the
        // simulator gives every part of an upload its URL from the upload's one ref (upload/{ref}/{part}), and a
        // begin_upload continues the upload by that ref. A part that reached the transport's bottom only after the caller
        // cancelled (CancelledAttempt.GrantedBy) also needs the begin_upload that granted it journaled, answered 2xx, as
        // that upload's. Each one so shown is Confirmed; any other is Rejected, and the final join accounts for it as for
        // any unanswered request.
        public static List<string> ConfirmCancelledAttempts(IEnumerable<CancelledAttempt> attempts, IReadOnlyList<ObservedRequest> requests, JournalAccount account, UploadJournal upload, SimulatorOperations operations)
        {
            var problems = new List<string>();
            Dictionary<int, JournalEntry> joined = account.Joined.ToDictionary(pair => pair.Request.Sequence, pair => pair.Entry);
            var shown = requests
                .Where(request => !request.Api && upload.Upload != null && upload.PartRequests.ContainsKey(request.Sequence) && RefOf(request.Path) != null
                    && joined.TryGetValue(request.Sequence, out JournalEntry entry) && entry.Operation == operations.UploadPart && entry.Upload == upload.Upload)
                .Select(request => (Ref: ScenarioDigest.Sha256(RefOf(request.Path)), request.Origin, UploadPath: UploadPathOf(request.Path))).ToList();
            foreach (CancelledAttempt attempt in attempts)
            {
                attempt.Upload = upload.Upload;
                string why = null;
                if (upload.Upload == null)
                {
                    why = "the journal shows no upload it was part of";
                }
                else if (joined.TryGetValue(attempt.Sequence, out JournalEntry entry))
                {
                    if (entry.Operation != attempt.Operation || entry.Upload != upload.Upload || (attempt.Storage && entry.Part != attempt.Part))
                    {
                        string part = attempt.Storage ? $", part {Shown.Of(entry.Part)}" : "";
                        string own = attempt.Storage ? $", part {attempt.Part}" : "";
                        why = $"its journal entry is {Shown.Of(entry.Operation)} of upload {Shown.Of(entry.Upload)}{part}, not {Shown.Of(attempt.Operation)} of upload {upload.Upload}{own}";
                    }
                }
                else if (!attempt.Storage && !shown.Any(part => part.Ref == attempt.RefSha256))
                {
                    why = $"no part of upload {upload.Upload} the journal shows acknowledged was sent with its ref";
                }
                else if (attempt.Storage && !shown.Any(part => part.Ref == attempt.RefSha256 && part.Origin == attempt.Origin && part.UploadPath != null && part.UploadPath == attempt.UploadPath))
                {
                    why = $"no part of upload {upload.Upload} the journal shows acknowledged was sent with its ref to its origin and upload path";
                }
                if (why == null && attempt.GrantedBy is int grant
                    && !(joined.TryGetValue(grant, out JournalEntry granted) && granted.Upload == upload.Upload && granted.Status >= 200 && granted.Status < 300))
                {
                    why = $"the begin_upload that granted its part (request {grant}) is not journaled as answered 2xx for upload {upload.Upload}";
                }
                if (why == null)
                {
                    attempt.Confirmed = true;
                }
                else
                {
                    attempt.Rejected = why;
                    problems.Add($"request {attempt.Sequence} was cancelled unanswered, and {why}");
                }
            }
            return problems;
        }

        // Null when REQUEST, which got no answer, is one of the attempts Settle describes, or why not. GRANTEDBY is set for a
        // part that reached the transport's bottom only after the caller cancelled (NotLate).
        private static string NotAnAttempt(ObservedRequest request, IReadOnlyList<ObservedRequest> requests, CancellationGate gate, Exception raised, string beginRoute, Dictionary<string, string> path, long? parts, out int? grantedBy)
        {
            grantedBy = null;
            if (gate == null)
            {
                return "the step does not cancel its upload";
            }
            if (!gate.Cancelled || gate.CancelEvent == null)
            {
                return "the upload was never cancelled";
            }
            if (!(raised is OperationCanceledException))
            {
                return $"the upload ended with {(raised == null ? "nothing" : Diagnostics.Describe(raised))}, not as cancelled";
            }
            if (request.ClientFailure == null || request.EndedAt == null)
            {
                return "it never ended";
            }
            if (request.EndedAt <= gate.CancelEvent || !request.OwnerCancelledAtEnd)
            {
                return "it ended before the caller cancelled";
            }
            if (request.TokenCancelledAt != null && request.OwnerCancelledWithToken != true)
            {
                return $"its own token was cancelled at event {request.TokenCancelledAt} while the caller's own token was not yet cancelled, so the caller's cancellation did not end it";
            }
            // Whatever of it had already failed when the caller actually cancelled, before that cancellation reached any
            // token of the SDK's, did not fail because the caller cancelled, however its end was relayed or observed after.
            string failedBeforeOwner = FailedBeforeOwner(request, gate);
            // What ended it: the cancellation of its own token, relayed to its transport while the transport was still
            // running; or, for a part, its body's writing, ended with the cancellation of the upload's own token, which the
            // caller's cancellation is what cancelled, before that relay, which then finds the transport already ended.
            string body = BodyAccount(request, requests, gate, out bool bodyEndedByOwner);
            if (bodyEndedByOwner && failedBeforeOwner != null)
            {
                return failedBeforeOwner;
            }
            if (!bodyEndedByOwner)
            {
                if (!request.TokenCancelled || request.TokenCancelledAt == null)
                {
                    return $"it ended with {request.ClientFailure} while its own token was not cancelled{body}";
                }
                if (request.TransportEndedBeforeToken != false)
                {
                    return $"its transport had already ended it, with {request.ClientFailure}, when the cancellation of its own token reached the transport at event {request.TokenCancelledAt}{body}, so the caller's cancellation did not end it";
                }
            }
            if (failedBeforeOwner != null)
            {
                return failedBeforeOwner;
            }
            if (!CancelledFailures.Contains(request.ClientFailure))
            {
                return $"it ended with {request.ClientFailure}, which is not how a cancelled request ends";
            }
            if (request.Api && request.OwnerCancelledAtBegin)
            {
                return "it began only after the caller cancelled";
            }
            if (parts == null)
            {
                return "the live profile gives no one partsize, so the upload's parts are not bounded";
            }
            if (!request.Api)
            {
                if (request.Method != "PUT")
                {
                    return $"it is {request.Method}, not the PUT that stores a part";
                }
                if (request.CarriesApiKey || request.CarriesAuthorization)
                {
                    return "it carried a credential to storage";
                }
                long? part = PartOf(request.Path);
                if (part == null || RefOf(request.Path) == null)
                {
                    return "its URL names no upload ref and part (upload/{ref}/{part})";
                }
                if (part > parts)
                {
                    return $"its part {part} is not one of the upload's parts 1 to {parts}";
                }
                return request.OwnerCancelledAtBegin ? NotLate(request, requests, beginRoute, path, part.Value, out grantedBy) : null;
            }
            if (ApiRoutes.Mismatch(request, beginRoute, path) != null)
            {
                return $"it is neither a part of the upload nor the begin_upload of {path["path"]} that negotiates one";
            }
            if (!request.CarriesApiKey || !request.ApiKeyMatches || request.CarriesAuthorization)
            {
                return "it did not carry the client's API key alone";
            }
            if (request.BodyRefSha256 == null)
            {
                return "it names no upload ref, so it negotiates no part of an upload begun";
            }
            return request.BodyPart >= 1 && request.BodyPart <= parts ? null : $"it negotiates part {Shown.Of(request.BodyPart)}, not one of the upload's parts 1 to {parts}";
        }

        // Null when REQUEST, a part of PART that reached the transport's bottom only after the caller had cancelled, is one
        // the SDK had already been granted, or why not; GRANT is then the begin_upload that granted it. Ownership has to be
        // shown, not assumed: it arrived with its own token already cancelled, as a token linked to the caller's is; it is
        // the only request for that part of its ref, so not one sent again; and a begin_upload of the step's path, with the
        // client's API key alone, negotiated exactly that part of that ref and was answered 2xx before the caller's own
        // token was cancelled (ObservedRequest.OwnerCancelledAtEnd), and before the part arrived. UploadCoordinator sends
        // only parts it has prepared from such a grant, so the part was the SDK's to send before the caller cancelled;
        // ConfirmCancelledAttempts then requires the journal to show that begin_upload as the step's upload's.
        private static string NotLate(ObservedRequest request, IReadOnlyList<ObservedRequest> requests, string beginRoute, Dictionary<string, string> path, long part, out int? grant)
        {
            grant = null;
            const string late = "it began only after the caller cancelled";
            if (!request.TokenCancelledAtBegin)
            {
                return $"{late}, with its own token not yet cancelled, so not linked to the caller's";
            }
            string reference = RefOf(request.Path);
            if (requests.Any(other => other.Sequence != request.Sequence && !other.Api && RefOf(other.Path) == reference && PartOf(other.Path) == part))
            {
                return $"{late}, and its part {part} had been sent before";
            }
            string refSha256 = ScenarioDigest.Sha256(reference);
            ObservedRequest granted = requests.LastOrDefault(other => other.Api && other.Succeeded && !other.OwnerCancelledAtEnd && other.EndedAt < request.BegunAt
                && other.BodyRefSha256 == refSha256 && other.BodyPart == part && ApiRoutes.Mismatch(other, beginRoute, path) == null
                && other.CarriesApiKey && other.ApiKeyMatches && !other.CarriesAuthorization);
            if (granted == null)
            {
                return $"{late}, and no begin_upload of {path["path"]} negotiating its part {part} with its ref was answered before the caller cancelled";
            }
            grant = granted.Sequence;
            return null;
        }

        // Null when nothing of REQUEST had failed when the caller actually cancelled (GATE's OwnerCancelledAt), or why it
        // had: what had already failed then (ObservedRequest.FailedBeforeOwner), or what, begun before then but known only
        // after, had failed by the time it was known otherwise than with the cancellation of the upload's own token, which
        // the caller's cancellation cancelled (ObservedRequest.FailedAcrossOwner), and so may have failed before.
        private static string FailedBeforeOwner(ObservedRequest request, CancellationGate gate)
        {
            if (request.FailedBeforeOwner != null)
            {
                return $"{request.FailedBeforeOwner} had already failed, with {request.FailedBeforeOwnerWith}, when the caller actually cancelled at event {Shown.Of(gate.OwnerCancelledAt)}, so the caller's cancellation did not end it";
            }
            foreach ((string what, Exception failure) in request.FailedAcrossOwner)
            {
                if (!EndedByOwnersBodyToken(failure, gate))
                {
                    return $"{what}, begun before the caller actually cancelled at event {Shown.Of(gate.OwnerCancelledAt)}, had failed, with {failure.GetType().Name}, by the time it was known, and not with the cancellation of the upload's own token that the caller's cancellation cancelled, so the caller's cancellation is not shown to have ended it";
                }
            }
            return null;
        }

        // Whether FAILURE is the cancellation of the upload's own token, that very token (GATE's BodyToken), which the
        // caller's cancellation is what cancelled.
        private static bool EndedByOwnersBodyToken(Exception failure, CancellationGate gate)
        {
            return failure is OperationCanceledException cancelled && gate.BodyToken is CancellationToken own && cancelled.CancellationToken == own && gate.BodyTokenCancelledByOwner == true;
        }

        // How REQUEST's body's writing ended, if it failed, as a clause of why the request is no attempt, and whether it ended
        // with the cancellation of the upload's own token (GATE's BodyToken), compared as the token itself, never by an
        // exception's name or by a token merely cancelled. ENDEDBYOWNER is set when it shows the caller's cancellation ended
        // it: it ended with the cancellation of that very token, which the caller's cancellation is what cancelled, and every
        // write of the upload's bodies (REQUESTS') was made with it. No witness of that token, or any other token, shows
        // nothing.
        private static string BodyAccount(ObservedRequest request, IReadOnlyList<ObservedRequest> requests, CancellationGate gate, out bool endedByOwner)
        {
            endedByOwner = false;
            Exception failure = request.BodyFailure;
            if (failure == null)
            {
                return "";
            }
            string ended = $", and its body ended with {failure.GetType().Name}";
            if (!(failure is OperationCanceledException cancelled))
            {
                return ended;
            }
            if (!(gate.BodyToken is CancellationToken own))
            {
                return $"{ended}, the upload's writes before the caller actually cancelled having shown no one token its bodies write with";
            }
            if (cancelled.CancellationToken != own)
            {
                return $"{ended} of a token other than the upload's own";
            }
            if (gate.BodyTokenCancelledByOwner != true)
            {
                return $"{ended} of the upload's own token, which the caller's cancellation did not cancel";
            }
            if (requests.Any(other => !other.Api && other.WrittenWith.Any(token => token != own)))
            {
                return $"{ended} of the upload's own token, though not every write of the upload's bodies was made with it";
            }
            endedByOwner = true;
            return $"{ended} of the upload's own token, which the caller's cancellation cancelled";
        }

        // REQUEST's exact account: its method, where it went, how it ended and when, beside the caller's intent to cancel and
        // its actual cancellation, whether the caller's own token had actually been cancelled as it began, as its own token
        // was cancelled and as it ended, what its body's writing ended with, and what of it had already failed when the
        // caller actually cancelled.
        private static string Account(ObservedRequest request, CancellationGate gate)
        {
            string where = request.Api ? request.Path : $"to storage {request.Origin}{request.Path}";
            string token = request.TokenCancelledAt is long tokenAt
                ? $", its own token cancelled at event {tokenAt} with the caller's {(request.OwnerCancelledWithToken == true ? "cancelled" : "not")}{(request.TransportEndedBeforeToken == true ? ", after its transport had ended it" : "")}"
                : "";
            string body = request.BodyEndedWith is string bodyFailure ? $", its body ended with {bodyFailure}" : "";
            string failed = request.FailedBeforeOwner != null ? $", {request.FailedBeforeOwner} having failed, with {request.FailedBeforeOwnerWith}, before the caller actually cancelled" : "";
            string cancelled = gate?.CancelEvent is long at
                ? $"; the caller meant to cancel at event {at}{(gate.OwnerCancelledAt is long actual ? $" and actually cancelled at event {actual}" : "")}"
                : "";
            string owner = $"; the caller's own token {(request.OwnerCancelledAtBegin ? "cancelled" : "not")} as it began and {(request.OwnerCancelledAtEnd ? "cancelled" : "not")} as it ended";
            return $"request {request.Sequence} ({request.Method} {where}, {request.Outcome}, begun at event {request.BegunAt}{token}{body}{failed}, ended at event {Shown.Of(request.EndedAt)}{cancelled}{owner})";
        }

        // A storage URL's path without the ref and part it ends with (upload/{ref}/{part}), such as
        // /__files_mock/transfer/upload: where the bound simulator, or a control's transport, issued the upload's parts.
        // Null unless the path names a ref and part.
        private static string UploadPathOf(string path)
        {
            if (PartOf(path) == null || RefOf(path) == null)
            {
                return null;
            }
            string[] names = path.Split('/');
            return string.Join("/", names[..^2]);
        }

        // The part number a storage URL's path names, as the simulator issues them (upload/{ref}/{part}, the part a
        // positive whole number), or null.
        private static long? PartOf(string path)
        {
            string[] names = path.Split('/');
            return names.Length >= 3 && names[^3] == "upload" && Regex.IsMatch(names[^1], @"\A[1-9][0-9]*\z") && long.TryParse(names[^1], NumberStyles.None, CultureInfo.InvariantCulture, out long part)
                ? part
                : (long?)null;
        }

        // The upload ref a storage URL's path names (upload/{ref}/{part}), or null.
        private static string RefOf(string path)
        {
            string[] names = path.Split('/');
            return names.Length >= 3 && names[^3] == "upload" && names[^2].Length > 0 ? Uri.UnescapeDataString(names[^2]) : null;
        }
    }

    // What a directory download made (ScenarioRun.DownloadDirectoryAsync), against its step: the empty folders the
    // download reported, and the folders, empty folders and files below ROOT, exactly as the definition's fixtures below
    // the step's folder are, by paths relative to it, with each file the step matches holding its fixture's recipe bytes.
    public static class TreeEvidence
    {
        public static List<string> Downloaded(string root, DownloadDirectoryStep step, IEnumerable<string> reportedEmpty, Func<string, ScenarioFixture> fixture)
        {
            var problems = new List<string>();
            string[] reported = Sorted(reportedEmpty);
            if (!reported.SequenceEqual(Sorted(step.EmptyDirectories)))
            {
                problems.Add($"the download reported the empty folders [{string.Join(", ", reported)}], not [{string.Join(", ", Sorted(step.EmptyDirectories))}]");
            }
            string[] folders = Folders(root);
            if (!folders.SequenceEqual(Sorted(step.Directories)))
            {
                problems.Add($"the local tree holds the folders [{string.Join(", ", folders)}], not [{string.Join(", ", Sorted(step.Directories))}]");
            }
            string[] local = System.IO.Directory.Exists(root)
                ? Sorted(System.IO.Directory.EnumerateFiles(root, "*", System.IO.SearchOption.AllDirectories).Select(path => Relative(root, path)))
                : Array.Empty<string>();
            if (!local.SequenceEqual(Sorted(step.Files)))
            {
                problems.Add($"the local tree holds the files [{string.Join(", ", local)}], not [{string.Join(", ", Sorted(step.Files))}]");
            }
            foreach (TreeFile file in step.BytesMatch)
            {
                string path = ScenarioRun.Local(root, file.RelativePath);
                if (!System.IO.File.Exists(path))
                {
                    problems.Add($"{file.RelativePath} was not downloaded");
                    continue;
                }
                ScenarioFixture matched = fixture(file.Fixture);
                try
                {
                    ScenarioRun.CheckBytes(matched, System.IO.File.ReadAllBytes(path), ScenarioRun.RecipeBytes(matched), file.RelativePath);
                }
                catch (ScenarioFailure e)
                {
                    problems.Add(e.Message);
                }
            }
            foreach (string directory in step.EmptyDirectories)
            {
                string path = ScenarioRun.Local(root, directory);
                if (!System.IO.Directory.Exists(path) || System.IO.Directory.EnumerateFileSystemEntries(path).Any())
                {
                    problems.Add($"the empty folder {directory} is not an empty local directory");
                }
            }
            return problems;
        }

        // The folders below ROOT, relative to it, in order; none when ROOT was never made.
        public static string[] Folders(string root)
        {
            return System.IO.Directory.Exists(root)
                ? Sorted(System.IO.Directory.EnumerateDirectories(root, "*", System.IO.SearchOption.AllDirectories).Select(path => Relative(root, path)))
                : Array.Empty<string>();
        }

        internal static string[] Sorted(IEnumerable<string> paths)
        {
            return paths.OrderBy(path => path, StringComparer.Ordinal).ToArray();
        }

        // A local path below ROOT, relative to it, with / between names.
        private static string Relative(string root, string path)
        {
            return System.IO.Path.GetRelativePath(root, path).Replace(System.IO.Path.DirectorySeparatorChar, '/');
        }
    }

    // A stream read forward only, at most MaxRead bytes a call, that cannot seek or tell its length or position.
    public sealed class ForwardOnlyStream : System.IO.Stream
    {
        private readonly byte[] bytes;
        private int position;

        public ForwardOnlyStream(byte[] bytes, int maxRead)
        {
            this.bytes = bytes;
            MaxRead = maxRead;
        }

        public int MaxRead { get; }

        public int Reads { get; private set; }

        public int LargestRead { get; private set; }

        public int Consumed => position;

        // Calls a forward-only stream refuses: seeking, length, position or writing.
        public int Refused { get; private set; }

        public bool Disposed { get; private set; }

        public override bool CanRead => !Disposed;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw Refuse("has no length");

        public override long Position
        {
            get => throw Refuse("has no position");
            set => throw Refuse("cannot seek");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan(offset, count));
        }

        public override int Read(Span<byte> buffer)
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(nameof(ForwardOnlyStream));
            }
            int count = Math.Min(Math.Min(buffer.Length, MaxRead), bytes.Length - position);
            bytes.AsSpan(position, count).CopyTo(buffer);
            position += count;
            Reads++;
            LargestRead = Math.Max(LargestRead, count);
            return count;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(Read(buffer.AsSpan(offset, count)));
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new ValueTask<int>(Read(buffer.Span));
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            throw Refuse("cannot seek");
        }

        public override void SetLength(long value)
        {
            throw Refuse("cannot change its length");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw Refuse("cannot be written");
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }

        private NotSupportedException Refuse(string what)
        {
            Refused++;
            return new NotSupportedException($"A forward-only stream {what}.");
        }
    }

    // The definition's matchers for an entry's field: one_of, includes and not.
    internal static class Matchers
    {
        // Null when value satisfies every matcher, or why not.
        internal static string Mismatch(object value, JsonObject matcher)
        {
            foreach (KeyValuePair<string, JsonNode> pair in matcher)
            {
                switch (pair.Key)
                {
                    case "one_of":
                        if (!(pair.Value is JsonArray candidates) || !candidates.Any(candidate => Same(value, candidate)))
                        {
                            return $"is {Shown.Of(value)}, not one of {pair.Value?.ToJsonString()}";
                        }
                        break;
                    case "includes":
                        if (!(pair.Value is JsonArray items) || !items.All(item => Includes(value, item)))
                        {
                            return $"is {Shown.Of(value)}, which does not include all of {pair.Value?.ToJsonString()}";
                        }
                        break;
                    case "not":
                        if (Same(value, pair.Value))
                        {
                            return $"is {Shown.Of(value)}";
                        }
                        break;
                    default:
                        return $"has the matcher {pair.Key}, which this consumer does not know";
                }
            }
            return null;
        }

        private static bool Same(object value, JsonNode expected)
        {
            if (expected == null)
            {
                return value == null;
            }
            JsonValueKind kind = expected.GetValueKind();
            switch (value)
            {
                case null:
                    return false;
                case string text:
                    return kind == JsonValueKind.String && expected.GetValue<string>() == text;
                case bool flag:
                    return (kind == JsonValueKind.True || kind == JsonValueKind.False) && expected.GetValue<bool>() == flag;
                case DateTime time:
                    return kind == JsonValueKind.String
                        && DateTime.TryParse(expected.GetValue<string>(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime instant)
                        && instant.ToUniversalTime() == time.ToUniversalTime();
                case DateTimeOffset offset:
                    return kind == JsonValueKind.String
                        && DateTimeOffset.TryParse(expected.GetValue<string>(), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset moment)
                        && moment == offset;
                case long or int or double or decimal:
                    return kind == JsonValueKind.Number && expected.GetValue<decimal>() == Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                default:
                    return false;
            }
        }

        private static bool Includes(object value, JsonNode item)
        {
            switch (value)
            {
                case string text:
                    return item?.GetValueKind() == JsonValueKind.String && text.Contains(item.GetValue<string>(), StringComparison.Ordinal);
                case IEnumerable items:
                    return items.Cast<object>().Any(element => Same(element, item));
                default:
                    return false;
            }
        }
    }

    // Diagnostics without credentials or signed URLs: an exception as its type and message, with every URL's query and
    // the run's API key removed.
    internal static class Diagnostics
    {
        private static readonly Regex UrlQuery = new Regex(@"(https?://[^\s?#""']+)[?#][^\s""']*", RegexOptions.IgnoreCase);

        internal static string Secret { get; set; }

        internal static string Describe(Exception e)
        {
            return $"{e.GetType().Name}: {Redact(e.Message)}";
        }

        internal static string Redact(string text)
        {
            if (text == null)
            {
                return null;
            }
            text = UrlQuery.Replace(text, "$1?(query removed)");
            return string.IsNullOrEmpty(Secret) ? text : text.Replace(Secret, "(API key)", StringComparison.Ordinal);
        }
    }

    internal static class Shown
    {
        internal static string Of(object value)
        {
            switch (value)
            {
                case null:
                    return "null";
                case string text:
                    return JsonSerializer.Serialize(text);
                case bool flag:
                    return flag ? "true" : "false";
                case DateTime time:
                    return time.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
                case IFormattable formattable:
                    return formattable.ToString(null, CultureInfo.InvariantCulture);
                default:
                    return value.ToString();
            }
        }
    }

    internal static class ScenarioDigest
    {
        internal static string Sha256(byte[] bytes)
        {
            return Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        }

        internal static string Sha256(string text)
        {
            return Sha256(Encoding.UTF8.GetBytes(text));
        }
    }

    // A position's own failure, which stops the case.
    internal sealed class ScenarioFailure : Exception
    {
        internal ScenarioFailure(string message) : base(message)
        {
        }
    }

    // A position's outcome: passed, failed, excluded (site only, never credited) or unreached (after a failure).
    public sealed class PositionRecord
    {
        internal PositionRecord(ScenarioStep step, string outcome, string detail, IReadOnlyList<ObservedRequest> requests)
        {
            Number = step.Number;
            Label = step.Label;
            Backend = step.Backend;
            Outcome = outcome;
            Detail = detail;
            Requests = requests;
        }

        public int Number { get; }

        public string Label { get; }

        public string Backend { get; }

        public string Outcome { get; }

        public string Detail { get; }

        public IReadOnlyList<ObservedRequest> Requests { get; }
    }

    // What one run of a case showed, and its receipt.
    public sealed class ScenarioOutcome
    {
        public const string NothingOwned = "nothing owned: the owned folder was never created as this case's";

        private readonly ScenarioCase scenarioCase;
        private readonly List<PositionRecord> positions = new List<PositionRecord>();
        private readonly Dictionary<int, List<string>> notes = new Dictionary<int, List<string>>();
        private readonly Dictionary<int, JsonObject> transfers = new Dictionary<int, JsonObject>();
        private readonly List<string> laterFailures = new List<string>();
        private IReadOnlyList<ObservedRequest> cleanupRequests = Array.Empty<ObservedRequest>();
        private JsonObject binding;
        private JournalAccount journal;
        private JournalContract contract;

        internal ScenarioOutcome(ScenarioCase scenarioCase)
        {
            this.scenarioCase = scenarioCase;
            CleanupOutcome = "not reached";
        }

        public IReadOnlyList<PositionRecord> Positions => positions;

        public int? FirstFailurePosition { get; private set; }

        public string FirstFailure { get; private set; }

        public string Namespace { get; internal set; }

        public bool NamespaceOwned { get; private set; }

        public int? OwnedAt { get; private set; }

        public int? ReleasedAt { get; private set; }

        public string CleanupOutcome { get; private set; }

        public string UnresolvedOwnership { get; private set; }

        public bool FinalJoinPassed { get; internal set; }

        public JournalAccount Journal => journal;

        public double Seconds { get; internal set; }

        public bool Passed => FirstFailure == null && UnresolvedOwnership == null && FinalJoinPassed
            && positions.Count == scenarioCase.Steps.Count
            && positions.All(position => position.Outcome == "passed" || position.Outcome == "excluded")
            && positions.Count(position => position.Outcome == "passed") == scenarioCase.SimulatorPositions;

        public string Summary()
        {
            if (Passed)
            {
                return $"{scenarioCase.Id}: passed {scenarioCase.SimulatorPositions} positions, {scenarioCase.Steps.Count - scenarioCase.SimulatorPositions} site-only positions excluded";
            }
            string label = FirstFailurePosition is int number ? positions.FirstOrDefault(position => position.Number == number)?.Label : null;
            string where = FirstFailurePosition is int at ? $" at position {at}{(label != null ? $" ({label})" : "")}" : "";
            string cleanup = UnresolvedOwnership != null ? $"; unresolved: {UnresolvedOwnership}" : "";
            return $"{scenarioCase.Id} failed{where}: {FirstFailure ?? "not every position passed"}; cleanup: {CleanupOutcome}{cleanup}";
        }

        internal void Binding(JsonObject summary)
        {
            binding = summary;
        }

        internal void Fail(int? position, string message)
        {
            if (FirstFailure == null)
            {
                FirstFailure = message;
                FirstFailurePosition = position;
            }
            else
            {
                laterFailures.Add(position is int number ? $"position {number}: {message}" : message);
            }
        }

        internal void Note(ScenarioStep step, string text)
        {
            if (!notes.TryGetValue(step.Number, out List<string> stepNotes))
            {
                notes[step.Number] = stepNotes = new List<string>();
            }
            stepNotes.Add(text);
        }

        internal void Transfer(ScenarioStep step, JsonObject evidence)
        {
            transfers[step.Number] = evidence;
        }

        internal void Position(ScenarioStep step, string failure, IReadOnlyList<ObservedRequest> requests)
        {
            notes.TryGetValue(step.Number, out List<string> stepNotes);
            string detail = failure ?? (stepNotes == null ? null : string.Join("; ", stepNotes));
            positions.Add(new PositionRecord(step, failure == null ? "passed" : "failed", detail, requests));
            if (failure != null)
            {
                Fail(step.Number, failure);
            }
        }

        internal void Excluded(ScenarioStep step)
        {
            positions.Add(new PositionRecord(step, "excluded", $"applies to {step.Backend} only: excluded, never credited", Array.Empty<ObservedRequest>()));
        }

        internal void Unreached(ScenarioStep step)
        {
            positions.Add(new PositionRecord(step, "unreached", "not run: an earlier position failed", Array.Empty<ObservedRequest>()));
        }

        internal void Owned(int position)
        {
            NamespaceOwned = true;
            OwnedAt = position;
        }

        internal void Released(int position)
        {
            ReleasedAt = position;
        }

        internal void Cleanup(string outcome, string unresolved, IReadOnlyList<ObservedRequest> requests)
        {
            CleanupOutcome = outcome;
            UnresolvedOwnership = unresolved;
            cleanupRequests = requests;
        }

        internal void Joined(JournalAccount account)
        {
            journal = account;
        }

        // The journal contract of the run, for the operation each request was meant to be and its cancelled attempts.
        internal void Bound(JournalContract runContract)
        {
            contract = runContract;
        }

        internal void Write(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            System.IO.File.WriteAllText(path, Document().ToJsonString(new JsonSerializerOptions { WriteIndented = true }) + "\n");
        }

        internal JsonObject Document()
        {
            Assembly sdk = typeof(FilesClient).Assembly;
            Dictionary<int, JournalEntry> join = journal?.Joined.ToDictionary(pair => pair.Request.Sequence, pair => pair.Entry) ?? new Dictionary<int, JournalEntry>();
            return new JsonObject
            {
                ["format"] = "files-dotnet-api-scenario-receipt",
                ["version"] = 1,
                ["case"] = scenarioCase.Id,
                ["test"] = scenarioCase.Test,
                ["result"] = Passed ? "passed" : "failed",
                ["definition_sha256"] = scenarioCase.DefinitionSha256,
                ["binding"] = binding?.DeepClone(),
                ["budgets"] = new JsonObject
                {
                    ["work_seconds"] = ScenarioBudget.Work.TotalSeconds,
                    ["cleanup_seconds"] = ScenarioBudget.Cleanup.TotalSeconds,
                    ["consumer_seconds"] = ScenarioBudget.Consumer.TotalSeconds,
                },
                ["runtime"] = new JsonObject
                {
                    ["framework"] = RuntimeInformation.FrameworkDescription,
                    ["consumer_target_framework"] = Assembly.GetExecutingAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName,
                    ["sdk_assembly"] = sdk.Location,
                    ["sdk_sha256"] = string.IsNullOrEmpty(sdk.Location) ? null : ScenarioDigest.Sha256(System.IO.File.ReadAllBytes(sdk.Location)),
                    ["sdk_version"] = sdk.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
                    ["sdk_target_framework"] = sdk.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName,
                },
                ["namespace"] = Namespace,
                ["ownership"] = new JsonObject
                {
                    ["acquired"] = NamespaceOwned,
                    ["acquired_at"] = OwnedAt,
                    ["released_at"] = ReleasedAt,
                    ["unresolved"] = UnresolvedOwnership,
                },
                ["counts"] = new JsonObject
                {
                    ["positions"] = scenarioCase.Steps.Count,
                    ["simulator"] = scenarioCase.SimulatorPositions,
                    ["passed"] = positions.Count(position => position.Outcome == "passed"),
                    ["failed"] = positions.Count(position => position.Outcome == "failed"),
                    ["excluded"] = positions.Count(position => position.Outcome == "excluded"),
                    ["unreached"] = positions.Count(position => position.Outcome == "unreached"),
                },
                ["excluded_positions"] = new JsonArray(positions.Where(position => position.Outcome == "excluded").Select(position => (JsonNode)position.Number).ToArray()),
                ["first_failure"] = FirstFailure == null ? null : new JsonObject { ["position"] = FirstFailurePosition, ["message"] = FirstFailure },
                ["later_failures"] = new JsonArray(laterFailures.Select(failure => (JsonNode)failure).ToArray()),
                ["cleanup"] = new JsonObject
                {
                    ["outcome"] = CleanupOutcome,
                    ["requests"] = Requests(cleanupRequests, join),
                },
                ["journal"] = journal == null ? null : new JsonObject
                {
                    ["final"] = journal.Final,
                    ["epoch"] = journal.Epoch,
                    ["complete"] = journal.Complete,
                    ["dropped"] = journal.Dropped,
                    ["baseline_seq"] = journal.BaselineSeq,
                    ["entries"] = journal.Entries.Count,
                    ["joined"] = journal.Joined.Count,
                    ["excused_without_entry"] = new JsonArray(journal.Excused.Select(request => (JsonNode)request.Sequence).ToArray()),
                    ["problems"] = new JsonArray(journal.Problems.Select(problem => (JsonNode)problem).ToArray()),
                },
                ["cancelled_attempts"] = new JsonArray((contract?.Cancelled.Values ?? Enumerable.Empty<CancelledAttempt>()).OrderBy(attempt => attempt.Sequence).Select(attempt => (JsonNode)new JsonObject
                {
                    ["sequence"] = attempt.Sequence,
                    ["operation"] = attempt.Operation,
                    ["part"] = attempt.Part,
                    ["upload_path"] = attempt.UploadPath,
                    ["granted_by"] = attempt.GrantedBy,
                    ["upload"] = attempt.Upload,
                    ["confirmed"] = attempt.Confirmed,
                    ["rejected"] = attempt.Rejected,
                }).ToArray()),
                ["positions"] = new JsonArray(positions.Select(position => (JsonNode)new JsonObject
                {
                    ["number"] = position.Number,
                    ["label"] = position.Label,
                    ["backend"] = position.Backend,
                    ["outcome"] = position.Outcome,
                    ["detail"] = position.Detail,
                    ["transfer"] = transfers.TryGetValue(position.Number, out JsonObject evidence) ? evidence.DeepClone() : null,
                    ["requests"] = Requests(position.Requests, join),
                }).ToArray()),
                ["seconds"] = Math.Round(Seconds, 1),
            };
        }

        private JsonArray Requests(IReadOnlyList<ObservedRequest> requests, Dictionary<int, JournalEntry> join)
        {
            return new JsonArray(requests.Select(request =>
            {
                join.TryGetValue(request.Sequence, out JournalEntry entry);
                string intended = null;
                contract?.Intended.TryGetValue(request.Sequence, out intended);
                return (JsonNode)new JsonObject
                {
                    ["sequence"] = request.Sequence,
                    ["method"] = request.Method,
                    ["path"] = request.Path,
                    ["query_names"] = new JsonArray(request.QueryNames.Select(name => (JsonNode)name).ToArray()),
                    ["origin"] = request.Origin,
                    ["api"] = request.Api,
                    ["body_ref_sha256"] = request.BodyRefSha256,
                    ["body_part"] = request.BodyPart,
                    ["begun_event"] = request.BegunAt,
                    ["ended_event"] = request.EndedAt,
                    ["status"] = request.Status,
                    ["error_type"] = request.ErrorType,
                    ["client_failure"] = request.ClientFailure,
                    ["token_cancelled"] = request.TokenCancelled,
                    ["token_cancelled_at_begin"] = request.TokenCancelledAtBegin,
                    ["token_cancelled_event"] = request.TokenCancelledAt,
                    ["owner_cancelled_at_begin"] = request.OwnerCancelledAtBegin,
                    ["owner_cancelled_with_token"] = request.OwnerCancelledWithToken,
                    ["transport_ended_before_token"] = request.TransportEndedBeforeToken,
                    ["failed_before_owner"] = request.FailedBeforeOwner,
                    ["failed_before_owner_with"] = request.FailedBeforeOwnerWith,
                    ["body_ended_with"] = request.BodyEndedWith,
                    ["owner_cancelled_at_end"] = request.OwnerCancelledAtEnd,
                    ["intended_operation"] = intended,
                    ["journal_seq"] = entry?.Seq,
                    ["journal_status"] = entry?.Status,
                    ["journal_operation"] = entry?.Operation,
                    ["journal_upload"] = entry?.Upload,
                    ["journal_part"] = entry?.Part,
                    ["journal_fault"] = entry?.Fault,
                };
            }).ToArray());
        }
    }

    // One run of a case: the SDK client it drives, through its own observed transport, in a folder the run names on the
    // simulator and owns only once the SDK has created it, with private local scratch space.
    public sealed class ScenarioRun
    {
        private readonly ScenarioCase scenarioCase;
        private readonly IScenarioEnvironment environment;
        private readonly ScenarioOutcome outcome;
        private readonly RequestRecorder recorder;
        private readonly Dictionary<string, ScenarioFixture> fixtures;
        private readonly Dictionary<string, ScenarioEntry> listed = new Dictionary<string, ScenarioEntry>();
        private readonly string scratch;
        private readonly JournalContract contract;
        private readonly long baselineSeq;
        private int scratchNames;
        private bool namespaceOwned;
        private bool namespaceDeleted;
        private bool namespaceGone;

        private ScenarioRun(ScenarioCase scenarioCase, IScenarioEnvironment environment, ScenarioOutcome outcome, JournalContract contract, long baselineSeq)
        {
            this.scenarioCase = scenarioCase;
            this.environment = environment;
            this.outcome = outcome;
            this.contract = contract;
            this.baselineSeq = baselineSeq;
            outcome.Bound(contract);
            fixtures = scenarioCase.Fixtures.ToDictionary(fixture => fixture.Name);
            string runId = Regex.Replace(environment.RunId ?? "run", "[^A-Za-z0-9._-]", "-");
            Namespace = $"api-scenarios-dotnet-{runId.Substring(0, Math.Min(runId.Length, 64))}-{Convert.ToHexString(RandomNumberGenerator.GetBytes(4)).ToLowerInvariant()}";
            outcome.Namespace = Namespace;
            recorder = new RequestRecorder(environment.BaseUrl, environment.ApiKey);
            var configuration = new FilesConfiguration { BaseUrl = environment.BaseUrl, ApiKey = environment.ApiKey };
            Client = FilesClient.Create(configuration, HttpTransport.FromHandlerFactory(() => new ObservingHandler(recorder, environment.CreateNetworkHandler())));
            scratch = System.IO.Directory.CreateTempSubdirectory("files-api-scenarios-").FullName;
        }

        // The client every step uses, made for this run with the simulator's address and credential.
        public FilesClient Client { get; }

        // The run's owned folder, which the definition's namespace fixture names.
        public string Namespace { get; }

        // Runs a case where api-scenarios.rb's launcher started this process, and fails the test unless every position
        // the simulator can carry out passed. Without a launcher, the case is not run.
        public static async Task ExecuteAsync(ScenarioCase scenarioCase)
        {
            IScenarioEnvironment environment = LauncherEnvironment.FromProcess();
            if (environment == null)
            {
                Assert.Inconclusive("not run: the API scenario cases run under api-scenarios.rb, whose launcher starts a simulator for each (FILES_MOCK_SERVER_URL is not set)");
            }
            ScenarioOutcome outcome = await RunAsync(scenarioCase, environment);
            if (!outcome.Passed)
            {
                Assert.Fail(outcome.Summary());
            }
        }

        // Binds the case to its simulator, runs every position until the first failure, cleans up what the case owns,
        // joins its requests to the journal, and writes the receipt, whatever happened.
        public static async Task<ScenarioOutcome> RunAsync(ScenarioCase scenarioCase, IScenarioEnvironment environment)
        {
            Stopwatch clock = Stopwatch.StartNew();
            var outcome = new ScenarioOutcome(scenarioCase);
            using var consumer = new CancellationTokenSource(ScenarioBudget.Consumer);
            Diagnostics.Secret = environment.ApiKey;
            ScenarioRun run = null;
            try
            {
                BindingInputs inputs = environment.ReadReceipts();
                inputs.Ready = await environment.ControlAsync("ready", consumer.Token);
                inputs.Journal = await environment.ControlAsync("journal", consumer.Token);
                List<string> problems = ScenarioBinding.Problems(inputs, scenarioCase);
                if (environment.ExpectedCase != null && environment.ExpectedCase != scenarioCase.Id)
                {
                    problems.Add($"the runner started this process for {environment.ExpectedCase}, not {scenarioCase.Id}");
                }
                outcome.Binding(ScenarioBinding.Summary(inputs));
                if (problems.Count > 0)
                {
                    outcome.Fail(null, "binding: " + string.Join("; ", problems));
                    foreach (ScenarioStep step in scenarioCase.Steps)
                    {
                        if (step.AppliesToSimulator)
                        {
                            outcome.Unreached(step);
                        }
                        else
                        {
                            outcome.Excluded(step);
                        }
                    }
                    return outcome;
                }
                // Bound: the reset's epoch, the launcher's instance, the case's definition, and the operations and partsize
                // live readiness gives hold every later journal read.
                var contract = new JournalContract(ScenarioBinding.Epoch(inputs.Reset).Value, inputs.Instance, scenarioCase.DefinitionSha256, ScenarioBinding.Operations(inputs),
                    ScenarioBinding.UploadPartSize(inputs));
                run = new ScenarioRun(scenarioCase, environment, outcome, contract, JournalJoin.LastSeq(inputs.Journal));
                await run.StepsAsync(consumer.Token);
                await run.CleanupAsync(consumer.Token);
                await run.FinalJoinAsync(consumer.Token);
            }
            catch (Exception e)
            {
                outcome.Fail(null, $"the consumer stopped: {Diagnostics.Describe(e)}");
            }
            finally
            {
                if (run != null)
                {
                    await run.DisposeAsync();
                }
                outcome.Seconds = clock.Elapsed.TotalSeconds;
                outcome.Write(environment.ReceiptPath);
            }
            return outcome;
        }

        // The remote path of a fixture: the owned folder, or a path within it.
        public string PathOf(string fixture)
        {
            ScenarioFixture found = Fixture(fixture);
            return found.IsNamespace ? Namespace : Namespace + "/" + found.Path;
        }

        // Loads a list one page at a time, at most maxPages pages, and returns what each page held and whether the last
        // had no next page. Each page is a request of its own, from the list's client.
        public async Task<StepResult> ListAsync<T>(FilesList<T> list, Func<T, ScenarioEntry> describe, int maxPages, CancellationToken token)
        {
            var pages = new List<IReadOnlyList<ScenarioEntry>>();
            bool more = true;
            while (more && pages.Count < maxPages)
            {
                await list.LoadNextPageAsync(token);
                pages.Add(list.data.Select(item =>
                {
                    ScenarioEntry entry = describe(item);
                    entry.Source = item;
                    return entry;
                }).ToList());
                more = list.HasNextPage;
            }
            return StepResult.Of(new ListedPages(pages, !more));
        }

        private async Task StepsAsync(CancellationToken consumerToken)
        {
            using var work = CancellationTokenSource.CreateLinkedTokenSource(consumerToken);
            work.CancelAfter(ScenarioBudget.Work);
            bool stopped = false;
            foreach (ScenarioStep step in scenarioCase.Steps)
            {
                if (!step.AppliesToSimulator)
                {
                    outcome.Excluded(step);
                    continue;
                }
                if (stopped)
                {
                    outcome.Unreached(step);
                    continue;
                }
                int first = recorder.Count;
                recorder.Position = step.Number;
                string failure = null;
                try
                {
                    await step.RunAsync(this, work.Token);
                    failure = await JoinAsync(final: false, work.Token);
                }
                catch (ScenarioFailure e)
                {
                    failure = e.Message;
                }
                catch (OperationCanceledException) when (work.IsCancellationRequested)
                {
                    failure = consumerToken.IsCancellationRequested
                        ? $"the consumer's {ScenarioBudget.Consumer.TotalSeconds}-second budget ran out"
                        : $"the case's {ScenarioBudget.Work.TotalSeconds}-second work budget ran out";
                }
                catch (Exception e)
                {
                    failure = Diagnostics.Describe(e);
                }
                outcome.Position(step, failure, recorder.Since(first));
                stopped = failure != null;
            }
            recorder.Position = 0;
        }

        // An operation step: its call, looked up again while an expected error has not answered yet, against the status,
        // error, entry or listing it must be answered with. Every request the call sends must be the step's own operation
        // on its own fixture's path; an expected error must come both from the wire and as the SDK's own ApiException of
        // that status and type; and the journal must show each request, as that operation, before the step is credited or
        // changes what the case owns.
        internal async Task RunOperationAsync(OperationStep step, CancellationToken token)
        {
            ScenarioExpectation expect = step.Expect;
            IReadOnlyDictionary<string, string> values = step.PathValues(this);
            int attempts = expect.Attempts ?? 1;
            for (int attempt = 1; ; attempt++)
            {
                int first = recorder.Count;
                StepResult result = null;
                Exception raised = null;
                try
                {
                    result = await step.Call(this, token);
                }
                catch (Exception e) when (!(e is OperationCanceledException && token.IsCancellationRequested))
                {
                    raised = e;
                }
                IReadOnlyList<ObservedRequest> requests = recorder.Since(first);
                string misrouted = Bind(requests, step.Route, values);
                if (misrouted != null)
                {
                    throw new ScenarioFailure(raised == null ? misrouted : $"{misrouted} (the SDK raised {Diagnostics.Describe(raised)})");
                }
                ObservedRequest last = requests.LastOrDefault();
                string answered = last == null ? "no API request" : $"request {last.Sequence} received {last.Outcome}";
                if (expect.Error != null)
                {
                    if (raised == null)
                    {
                        if (attempt < attempts)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(expect.IntervalSeconds ?? 1), token);
                            continue;
                        }
                        throw new ScenarioFailure($"expected {expect.Status} {expect.Error}{(attempts > 1 ? $" within {attempts} lookups" : "")}, but the SDK succeeded after {answered}");
                    }
                    if (last == null || last.Status != expect.Status || last.ErrorType != expect.Error)
                    {
                        throw new ScenarioFailure($"expected {expect.Status} {expect.Error}, but the SDK raised {Diagnostics.Describe(raised)} after {answered}");
                    }
                    string unmapped = Unmapped(raised, expect.Status, expect.Error);
                    if (unmapped != null)
                    {
                        throw new ScenarioFailure($"{answered}, as expected, but {unmapped}");
                    }
                    await ConfirmJournalAsync(requests, token);
                    outcome.Note(step, $"the SDK raised {raised.GetType().Name} ({expect.Status} {expect.Error}) after {answered}{(attempts > 1 ? $", lookup {attempt} of {attempts}" : "")}");
                    AfterError(step);
                    return;
                }
                if (raised != null)
                {
                    throw new ScenarioFailure($"expected {expect.Status}, but the SDK raised {Diagnostics.Describe(raised)} after {answered}");
                }
                if (requests.Count == 0)
                {
                    throw new ScenarioFailure("the SDK sent no API request");
                }
                ObservedRequest other = requests.FirstOrDefault(request => request.Status != expect.Status);
                if (other != null)
                {
                    throw new ScenarioFailure($"expected {expect.Status}, but request {other.Sequence} ({other.Method} {other.Path}) received {other.Outcome}");
                }
                if (expect.Entry != null)
                {
                    CheckEntry(expect.Entry, result?.Entry);
                }
                if (expect.Listing != null)
                {
                    CheckListing(expect.Listing, result?.Listing);
                }
                // The SDK's create, on the owned folder's path, was answered 201 with exactly that folder: the folder is the
                // case's to delete, whatever the journal then shows.
                if (step.CreatesNamespace)
                {
                    namespaceOwned = true;
                    outcome.Owned(step.Number);
                }
                await ConfirmJournalAsync(requests, token);
                outcome.Note(step, $"{requests.Count} API requests, each answered {expect.Status}");
                AfterSuccess(step);
                return;
            }
        }

        // Binds REQUESTS (one call's, or the cleanup's) to ROUTE with VALUES: each must be that operation on that path, and
        // the journal must then show it as the operation the simulator lists for ROUTE. Null when every one is, or why not.
        private string Bind(IReadOnlyList<ObservedRequest> requests, string route, IReadOnlyDictionary<string, string> values)
        {
            string operation = contract.Operations.Of(route);
            foreach (ObservedRequest request in requests)
            {
                string mismatch = ApiRoutes.Mismatch(request, route, values);
                if (mismatch != null)
                {
                    return mismatch;
                }
                contract.Intended[request.Sequence] = operation;
            }
            return null;
        }

        // Null when the SDK raised its own public API error for STATUS and TYPE (an ApiException with that httpStatus and
        // type), or what it raised instead.
        internal static string Unmapped(Exception raised, int status, string type)
        {
            if (raised is ApiException error && error.httpStatus == status && error.type == type)
            {
                return null;
            }
            return raised is ApiException other
                ? $"the SDK raised {other.GetType().Name} with HTTP {other.httpStatus} and type {Shown.Of(other.type)}, not {status} {type}"
                : $"the SDK raised {(raised == null ? "nothing" : Diagnostics.Describe(raised))}, not its ApiException for {status} {type}";
        }

        // Reads the journal and requires each of REQUESTS joined to its entry, with no problem in the account, before a step
        // is credited or ownership changes.
        private async Task ConfirmJournalAsync(IReadOnlyList<ObservedRequest> requests, CancellationToken token)
        {
            JournalAccount account = await ReadJournalAsync(final: false, token);
            if (account.Problems.Count > 0)
            {
                throw new ScenarioFailure(Summarize(account));
            }
            var joined = new HashSet<int>(account.Joined.Select(pair => pair.Request.Sequence));
            ObservedRequest missing = requests.FirstOrDefault(request => !joined.Contains(request.Sequence));
            if (missing != null)
            {
                throw new ScenarioFailure($"journal: the SDK's request {missing.Sequence} ({missing.Method} {missing.Path}, {missing.Outcome}) has no journal entry");
            }
        }

        // A deletion of the owned folder, by a request bound to that operation on its path and shown by the journal, then
        // an answer, so bound and shown, that it is not found, releases it.
        private void AfterSuccess(OperationStep step)
        {
            if (namespaceOwned && IsNamespace(step.Target) && ApiRoutes.MethodOf(step.Route) == "DELETE")
            {
                namespaceDeleted = true;
            }
        }

        private void AfterError(OperationStep step)
        {
            if (namespaceOwned && namespaceDeleted && IsNamespace(step.Target) && step.Expect.Error == "not-found")
            {
                namespaceGone = true;
                outcome.Released(step.Number);
            }
        }

        private bool IsNamespace(string fixture)
        {
            return fixture != null && Fixture(fixture).IsNamespace;
        }

        private void CheckEntry(EntryExpectation expected, ScenarioEntry actual)
        {
            if (actual == null)
            {
                throw new ScenarioFailure("the SDK returned no entry");
            }
            if (expected.Fixture != null)
            {
                CheckFixtureEntry(Fixture(expected.Fixture), actual, "the entry");
            }
            if (expected.Type != null && actual.Type != expected.Type)
            {
                throw new ScenarioFailure($"the entry's type is {Shown.Of(actual.Type)}, not {Shown.Of(expected.Type)}");
            }
            if (expected.PathOneOf != null && !expected.PathOneOf.Select(path => path.Length == 0 ? Namespace : Namespace + "/" + path).Contains(actual.Path))
            {
                throw new ScenarioFailure($"the entry's path is {Shown.Of(actual.Path)}, none of the paths allowed");
            }
            foreach (FieldExpectation field in expected.Fields)
            {
                actual.Fields.TryGetValue(field.Field, out object value);
                string mismatch = Matchers.Mismatch(value, field.Matcher);
                if (mismatch != null)
                {
                    throw new ScenarioFailure($"the entry's {field.Field} {mismatch}");
                }
            }
        }

        private void CheckFixtureEntry(ScenarioFixture fixture, ScenarioEntry actual, string what)
        {
            string path = PathOf(fixture.Name);
            if (actual.Path != path)
            {
                throw new ScenarioFailure($"{what}'s path is {Shown.Of(actual.Path)}, not {Shown.Of(path)}");
            }
            if (actual.Type != fixture.Type)
            {
                throw new ScenarioFailure($"{what} for {fixture.Name} is a {Shown.Of(actual.Type)}, not a {fixture.Type}");
            }
            if (!fixture.IsDirectory && actual.Size != fixture.Recipe.Length)
            {
                throw new ScenarioFailure($"{what} for {fixture.Name} holds {Shown.Of(actual.Size)} bytes, not {fixture.Recipe.Length}");
            }
        }

        private void CheckListing(ListingExpectation expected, ListedPages listing)
        {
            if (listing == null)
            {
                throw new ScenarioFailure("the SDK returned no listing");
            }
            if (!listing.Complete)
            {
                throw new ScenarioFailure($"the listing still had a next page after {listing.Pages.Count} pages, its bound");
            }
            for (int index = 0; index < listing.Pages.Count; index++)
            {
                if (listing.Pages[index].Count > expected.PerPage)
                {
                    throw new ScenarioFailure($"page {index + 1} held {listing.Pages[index].Count} entries, more than per_page {expected.PerPage}");
                }
            }
            List<ScenarioEntry> entries = listing.Pages.SelectMany(page => page).ToList();
            if (expected.MaxRecords is int maxRecords && entries.Count > maxRecords)
            {
                throw new ScenarioFailure($"the listing held {entries.Count} entries, more than {maxRecords}");
            }
            if (expected.Entries == null)
            {
                return;
            }
            var seen = new HashSet<string>();
            foreach (ScenarioEntry entry in entries)
            {
                ScenarioFixture fixture = expected.Entries.Select(Fixture).FirstOrDefault(candidate => PathOf(candidate.Name) == entry.Path);
                if (fixture == null)
                {
                    throw new ScenarioFailure($"the listing returned {Shown.Of(entry.Path)}, which is none of {string.Join(", ", expected.Entries)}");
                }
                if (!seen.Add(fixture.Name))
                {
                    throw new ScenarioFailure($"the listing returned {fixture.Name} more than once");
                }
                CheckFixtureEntry(fixture, entry, "the listed entry");
                listed[fixture.Name] = entry;
            }
            List<string> missing = expected.Entries.Where(name => !seen.Contains(name)).ToList();
            if (missing.Count > 0)
            {
                throw new ScenarioFailure($"the listing did not return {string.Join(", ", missing)}");
            }
        }

        internal async Task UploadAsync(UploadStep step, CancellationToken token)
        {
            ScenarioFixture fixture = Fixture(step.Fixture);
            byte[] bytes = RecipeBytes(fixture);
            string remote = PathOf(fixture.Name);
            CancellationGate gate = step.CancelAfterFirstAcknowledgedPart ? new CancellationGate() : null;
            using var caller = CancellationTokenSource.CreateLinkedTokenSource(token);
            int first = recorder.Count;
            var progress = new SynchronousProgress(recorder, first, gate, caller);
            var options = new UploadOptions { Progress = progress };
            if (step.ProvidedMtime != null)
            {
                options.ModifiedTime = DateTime.Parse(step.ProvidedMtime, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
            }
            bool completed = false;
            Exception raised = null;
            ForwardOnlyStream forward = null;
            System.IO.Stream seekable = null;
            // The caller's own token, whose actual cancellation each of the upload's requests reads (RequestRecorder.Owner).
            recorder.Owner = caller.Token;
            try
            {
                switch (step.Source)
                {
                    case "local-file":
                        string local = ScratchPath("upload");
                        System.IO.File.WriteAllBytes(local, bytes);
                        await Client.Transfers.UploadFileAsync(local, remote, options, caller.Token);
                        break;
                    case "seekable-stream":
                        seekable = new System.IO.MemoryStream(bytes, writable: false);
                        await Client.Transfers.UploadStreamAsync(seekable, bytes.Length, remote, options, caller.Token);
                        break;
                    case "forward-only-stream":
                        forward = new ForwardOnlyStream(bytes, step.MaxRead ?? throw new ScenarioFailure("a forward-only stream upload names no max_read"));
                        await Client.Transfers.UploadStreamAsync(forward, bytes.Length, remote, options, caller.Token);
                        break;
                    default:
                        throw new ScenarioFailure($"this consumer has no upload from {step.Source}");
                }
                completed = true;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (ScenarioFailure)
            {
                throw;
            }
            catch (Exception e)
            {
                raised = e;
            }
            finally
            {
                recorder.Owner = CancellationToken.None;
            }
            // Settled: the upload's task has ended, and with it every part's and negotiation's (UploadCoordinator). Each
            // request is meant to be the upload's begin_upload, one of its parts or its completion; a request its own
            // cancellation ended unanswered may stay so, once shown to be this upload's.
            IReadOnlyList<ObservedRequest> requests = recorder.Since(first);
            UnansweredRequests unanswered = UploadEvidence.Settle(requests, contract, gate, raised, step.BeginRoute, step.CompletionRoute, remote, bytes.Length);
            JournalAccount account = await ReadJournalAsync(final: false, token);
            UploadJournal journaled = UploadEvidence.Read(requests, account, contract.Operations, step.BeginRoute, step.CompletionRoute, remote);
            List<string> unconfirmed = UploadEvidence.ConfirmCancelledAttempts(unanswered.Attempts, requests, account, journaled, contract.Operations);
            var evidence = new JsonObject
            {
                ["source"] = step.Source,
                ["bytes"] = bytes.Length,
                ["sha256"] = fixture.Recipe.Sha256,
                ["provided_mtime"] = step.ProvidedMtime,
                ["progress"] = progress.Evidence(),
                ["upload"] = journaled.Upload,
                ["stored_parts"] = new JsonArray(journaled.StoredParts.Select(part => (JsonNode)part).ToArray()),
            };
            if (gate != null)
            {
                evidence["cancellation"] = new JsonObject
                {
                    ["cancelled"] = gate.Cancelled,
                    ["cancel_event"] = gate.CancelEvent,
                    ["owner_cancelled_event"] = gate.OwnerCancelledAt,
                    ["body_token_witnesses"] = new JsonArray(gate.BodyTokenWitnesses.Select(sequence => (JsonNode)sequence).ToArray()),
                    ["body_token_cancelled_before_owner"] = gate.BodyTokenCancelledBeforeOwner,
                    ["body_token_cancelled_by_owner"] = gate.BodyTokenCancelledByOwner,
                    ["acknowledged_bytes"] = gate.AcknowledgedBytes,
                    ["parts_acknowledged"] = gate.PartsAcknowledged,
                    ["requests_begun"] = gate.RequestsBegun,
                    ["answered_storage_requests"] = new JsonArray(gate.AnsweredStorage.Select(sequence => (JsonNode)sequence).ToArray()),
                    ["acknowledged_parts"] = new JsonArray(gate.AnsweredStorage.Where(journaled.PartRequests.ContainsKey).Select(sequence => (JsonNode)journaled.PartRequests[sequence]).ToArray()),
                    ["unanswered_attempts"] = new JsonArray(unanswered.Attempts.Select(attempt => (JsonNode)attempt.Sequence).ToArray()),
                    ["callback_failure"] = progress.CancelFailure,
                };
            }
            if (forward != null)
            {
                evidence["forward_only"] = new JsonObject { ["reads"] = forward.Reads, ["largest_read"] = forward.LargestRead, ["consumed"] = forward.Consumed, ["refused"] = forward.Refused };
            }
            seekable?.Dispose();
            outcome.Transfer(step, evidence);

            var problems = new List<string>();
            if (account.Problems.Count > 0)
            {
                problems.Add(Summarize(account));
            }
            // Any request that got no answer, other than the cancellation's own attempts, whatever the upload's outcome.
            problems.AddRange(unanswered.Problems);
            if (forward != null)
            {
                if (forward.LargestRead > forward.MaxRead)
                {
                    problems.Add($"a read returned {forward.LargestRead} bytes, more than {forward.MaxRead}");
                }
                if (forward.Refused > 0)
                {
                    problems.Add($"the upload asked the forward-only stream to seek or tell its length {forward.Refused} times");
                }
                if (forward.Disposed)
                {
                    problems.Add("the upload disposed the caller's stream");
                }
                if (completed && forward.Consumed != bytes.Length)
                {
                    problems.Add($"the upload read {forward.Consumed} of the stream's {bytes.Length} bytes");
                }
            }
            if (step.CancelAfterFirstAcknowledgedPart)
            {
                problems.AddRange(UploadEvidence.Cancelled(completed, raised, gate, journaled, requests, step.CompletionRoute, remote, bytes.Length));
                problems.AddRange(unconfirmed);
                if (progress.CancelFailure != null)
                {
                    problems.Add($"cancelling raised {progress.CancelFailure}");
                }
            }
            else if (raised != null)
            {
                problems.Add($"the upload failed: {Diagnostics.Describe(raised)}");
            }
            else
            {
                problems.AddRange(UploadEvidence.Completed(journaled, requests, step.CompletionRoute, remote, step.MinParts));
                if (step.FinalProgress)
                {
                    problems.AddRange(progress.FinalProblems(bytes.Length));
                }
                ProgressReport last = progress.Reports.LastOrDefault();
                if (step.MinParts > 0 && (last == null || last.PartsAcknowledged < step.MinParts))
                {
                    problems.Add($"the final progress acknowledged {last?.PartsAcknowledged ?? 0} parts, fewer than {step.MinParts}");
                }
            }
            if (problems.Count > 0)
            {
                throw new ScenarioFailure(string.Join("; ", problems));
            }
        }

        internal async Task DownloadAsync(DownloadStep step, CancellationToken token)
        {
            ScenarioFixture fixture = Fixture(step.Fixture);
            byte[] expected = RecipeBytes(fixture);
            string remote = PathOf(fixture.Name);
            string local = ScratchPath("download");
            int first = recorder.Count;
            if (step.ListedEntry)
            {
                if (!listed.TryGetValue(fixture.Name, out ScenarioEntry entry))
                {
                    throw new ScenarioFailure($"no listing returned {fixture.Name}, so there is no listed entry to download");
                }
                if (!(entry.Source is FilesCom.Models.RemoteFile file))
                {
                    throw new ScenarioFailure($"the listed {fixture.Name} is a {entry.Source?.GetType().Name ?? "null"}, which does not download itself");
                }
                await file.DownloadFileAsync(local, token);
            }
            else
            {
                await Client.Transfers.DownloadFileAsync(remote, local, null, token);
            }
            BindIntent(recorder.Since(first), step.OperationRoutes[0], path => path == remote.Trim('/'), "the download");
            CheckBytes(fixture, System.IO.File.ReadAllBytes(local), expected, "the downloaded file");
            outcome.Note(step, $"{expected.Length} bytes, SHA-256 {fixture.Recipe.Sha256}{(step.ListedEntry ? ", through the listed entry" : "")}");
        }

        internal async Task FileActionAsync(FileActionStep step, CancellationToken token)
        {
            int first = recorder.Count;
            FilesCom.Models.FileAction action = await step.Start(this, token);
            ObservedRequest[] api = recorder.Since(first).Where(request => request.Api).ToArray();
            ObservedRequest refused = api.FirstOrDefault(request => !request.Succeeded);
            if (api.Length == 0 || refused != null)
            {
                throw new ScenarioFailure($"the {step.Action} was not answered 2xx ({(refused == null ? "no API request" : $"request {refused.Sequence} received {refused.Outcome}")})");
            }
            if (action == null)
            {
                throw new ScenarioFailure($"the {step.Action} returned no action to wait for");
            }
            FilesCom.Models.FileMigration migration = await action.WaitAsync(null, null, token);
            string source = PathOf(step.From);
            BindIntent(recorder.Since(first), step.OperationRoutes[0], path => path == source.Trim('/'), $"the {step.Action}");
            outcome.Note(step, migration == null ? $"the {step.Action} was complete when Files.com answered" : $"the {step.Action}'s migration completed");
        }

        // Binds an intent's API requests (REQUESTS) to what it acts on, OWN: each must be routed to an operation the
        // simulator lists and, when that operation's route has a {path}, be on a path OWN accepts (compared without
        // slashes at either end); and at least one must be the intent's own START route on such a path. Each is then meant
        // to be that operation in the journal.
        private void BindIntent(IReadOnlyList<ObservedRequest> requests, string start, Func<string, bool> own, string what)
        {
            bool started = false;
            foreach (ObservedRequest request in requests.Where(request => request.Api))
            {
                IReadOnlyList<string> routed = contract.Operations.Candidates(request);
                if (routed.Count != 1)
                {
                    throw new ScenarioFailure($"{what}'s request {request.Sequence} ({request.Method} {request.Path}) is {(routed.Count == 0 ? "no operation the simulator lists" : $"each of {string.Join(", ", routed)}")}");
                }
                string path = contract.Operations.PathValue(routed[0], request);
                if (path != null && !own(path.Trim('/')))
                {
                    throw new ScenarioFailure($"{what}'s request {request.Sequence} ({request.Method} {request.Path}) is not on its own paths");
                }
                contract.Intended[request.Sequence] = routed[0];
                Dictionary<string, string> values = request.Method == ApiRoutes.MethodOf(start) ? ApiRoutes.ValuesIn(ApiRoutes.ApiPrefix + ApiRoutes.TemplateOf(start), request.Path) : null;
                started |= values != null && (!values.TryGetValue("path", out string startPath) || own(startPath.Trim('/')));
            }
            if (!started)
            {
                throw new ScenarioFailure($"{what} sent no {start} on its own paths");
            }
        }

        // Whether PATH (without slashes at either end) is FOLDER's own or within it.
        private static bool Within(string folder, string path)
        {
            string root = folder.Trim('/');
            return path == root || path.StartsWith(root + "/", StringComparison.Ordinal);
        }

        internal async Task UploadDirectoryAsync(UploadDirectoryStep step, CancellationToken token)
        {
            string root = ScratchPath("tree-up");
            System.IO.Directory.CreateDirectory(root);
            foreach (string directory in step.Directories)
            {
                System.IO.Directory.CreateDirectory(Local(root, directory));
            }
            long bytes = 0;
            foreach (TreeFile file in step.Files)
            {
                string path = Local(root, file.RelativePath);
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                byte[] content = RecipeBytes(Fixture(file.Fixture));
                System.IO.File.WriteAllBytes(path, content);
                bytes += content.Length;
            }
            string folder = PathOf(step.Fixture);
            int first = recorder.Count;
            DirectoryTransferResult result = await Client.Transfers.UploadDirectoryAsync(root, folder, null, token);
            IReadOnlyList<ObservedRequest> requests = recorder.Since(first);
            BindIntent(requests, step.OperationRoutes[0], path => Within(folder, path), "the directory upload");

            var problems = new List<string>();
            if (bytes != step.Bytes)
            {
                problems.Add($"the tree's files hold {bytes} bytes, not the {step.Bytes} the definition expects");
            }
            if (result.Files.Count != step.FileCount)
            {
                problems.Add($"{result.Files.Count} files were uploaded, not {step.FileCount}");
            }
            if (result.Bytes != step.Bytes)
            {
                problems.Add($"{result.Bytes} bytes were uploaded, not {step.Bytes}");
            }
            string[] empty = result.Directories.OrderBy(directory => directory, StringComparer.Ordinal).ToArray();
            if (empty.Length != step.EmptyFolderCount || !empty.SequenceEqual(step.EmptyDirectories.OrderBy(directory => directory, StringComparer.Ordinal)))
            {
                problems.Add($"the empty folders made were [{string.Join(", ", empty)}], not [{string.Join(", ", step.EmptyDirectories)}]");
            }
            if (result.Excluded.Count > 0)
            {
                problems.Add($"{result.Excluded.Count} local entries were not uploaded");
            }
            foreach (string name in step.Completed)
            {
                if (!requests.Any(request => UploadEvidence.IsCompletion(request, step.CompletionRoute, PathOf(name)) && request.Succeeded))
                {
                    problems.Add($"no 2xx request completed {name}");
                }
            }
            outcome.Note(step, $"{result.Files.Count} files, {result.Bytes} bytes, empty folders [{string.Join(", ", empty)}]");
            if (problems.Count > 0)
            {
                throw new ScenarioFailure(string.Join("; ", problems));
            }
        }

        internal async Task DownloadDirectoryAsync(DownloadDirectoryStep step, CancellationToken token)
        {
            // Not made here: the download creates it.
            string root = ScratchPath("tree-down");
            string folder = PathOf(step.Fixture);
            int first = recorder.Count;
            DirectoryTransferResult result = await Client.Transfers.DownloadDirectoryAsync(folder, root, null, token);
            BindIntent(recorder.Since(first), step.OperationRoutes[0], path => Within(folder, path), "the directory download");

            var problems = new List<string>();
            if (result.Files.Count != step.FileCount)
            {
                problems.Add($"{result.Files.Count} files were downloaded, not {step.FileCount}");
            }
            if (result.Bytes != step.Bytes)
            {
                problems.Add($"{result.Bytes} bytes were downloaded, not {step.Bytes}");
            }
            // The empty folders the download reports, and the folders and files it made, exactly as the definition's
            // fixtures below the folder are.
            problems.AddRange(TreeEvidence.Downloaded(root, step, result.Directories, Fixture));
            outcome.Note(step, $"{result.Files.Count} files, {result.Bytes} bytes, folders [{string.Join(", ", TreeEvidence.Folders(root))}], empty folders [{string.Join(", ", TreeEvidence.Sorted(result.Directories))}]");
            if (problems.Count > 0)
            {
                throw new ScenarioFailure(string.Join("; ", problems));
            }
        }

        // After an early stop, with a token of its own: the owned folder, and only it, deleted recursively and confirmed
        // absent, as namespace.close confirms it. Nothing is deleted that the case does not own.
        private async Task CleanupAsync(CancellationToken consumerToken)
        {
            if (!namespaceOwned)
            {
                outcome.Cleanup(ScenarioOutcome.NothingOwned, null, Array.Empty<ObservedRequest>());
                return;
            }
            if (namespaceGone)
            {
                outcome.Cleanup("not needed: the scenario deleted its owned folder and confirmed it absent", null, Array.Empty<ObservedRequest>());
                return;
            }
            using var cleanup = CancellationTokenSource.CreateLinkedTokenSource(consumerToken);
            cleanup.CancelAfter(ScenarioBudget.Cleanup);
            recorder.Position = 0;
            int first = recorder.Count;
            var owned = new Dictionary<string, string> { ["path"] = Namespace };
            var steps = new List<string>();
            try
            {
                await scenarioCase.DeleteNamespaceRecursively(this, cleanup.Token);
                steps.Add("the recursive delete was answered");
            }
            catch (Exception e)
            {
                steps.Add($"the recursive delete raised {Diagnostics.Describe(e)}");
            }
            string misrouted = Bind(recorder.Since(first), scenarioCase.DeleteRoute, owned);
            if (misrouted != null)
            {
                steps.Add($"the recursive delete: {misrouted}");
            }
            // Absent only when a lookup bound to the owned folder's own path is answered 404 not-found. The SDK must
            // report that answer as its ApiException too; when it does not, the absence still stands, and that failure is
            // kept beside it.
            ObservedRequest witness = null;
            string unmapped = null;
            for (int attempt = 1; attempt <= ScenarioBudget.AbsenceAttempts && witness == null && !cleanup.IsCancellationRequested; attempt++)
            {
                int before = recorder.Count;
                Exception raised = null;
                try
                {
                    await scenarioCase.LookUpNamespace(this, cleanup.Token);
                }
                catch (OperationCanceledException) when (cleanup.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception e)
                {
                    raised = e;
                }
                IReadOnlyList<ObservedRequest> lookups = recorder.Since(before);
                ObservedRequest last = lookups.LastOrDefault();
                string lookupMisrouted = Bind(lookups, scenarioCase.LookupRoute, owned);
                if (lookupMisrouted != null)
                {
                    steps.Add($"lookup {attempt}: {lookupMisrouted}");
                }
                else if (raised == null)
                {
                    steps.Add($"lookup {attempt} still found it");
                }
                else if (last != null && last.Status == 404 && last.ErrorType == "not-found")
                {
                    witness = last;
                    unmapped = Unmapped(raised, 404, "not-found");
                    steps.Add($"lookup {attempt} received {last.Outcome}");
                }
                else
                {
                    steps.Add($"lookup {attempt} raised {Diagnostics.Describe(raised)} ({last?.Outcome ?? "no API request"})");
                }
                if (witness == null && attempt < ScenarioBudget.AbsenceAttempts)
                {
                    try
                    {
                        await Task.Delay(ScenarioBudget.AbsenceInterval, cleanup.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
            // The journal must show the absence too, as the lookup's own operation, before the folder is released.
            string journaled = witness == null ? null : await JournaledAbsenceAsync(witness, scenarioCase.LookupRoute, cleanup.Token);
            IReadOnlyList<ObservedRequest> requests = recorder.Since(first);
            if (witness != null && journaled == null)
            {
                namespaceGone = true;
                outcome.Cleanup($"deleted and confirmed absent ({string.Join("; ", steps)}){(unmapped == null ? "" : $", but {unmapped}")}", null, requests);
                if (unmapped != null)
                {
                    outcome.Fail(null, $"cleanup: the owned folder is absent (request {witness.Sequence} received {witness.Outcome}), but {unmapped}");
                }
            }
            else
            {
                if (journaled != null)
                {
                    steps.Add(journaled);
                }
                outcome.Cleanup($"unresolved ({string.Join("; ", steps)})", $"{Namespace} may still exist and is this case's to remove", requests);
            }
        }

        // Null when the journal shows WITNESS as ROUTE's operation answered 404, of the reset's epoch and with no fault, or
        // why not.
        private async Task<string> JournaledAbsenceAsync(ObservedRequest witness, string route, CancellationToken token)
        {
            try
            {
                JournalAccount account = await ReadJournalAsync(final: false, token);
                JournalEntry entry = account.Joined.Where(pair => pair.Request.Sequence == witness.Sequence).Select(pair => pair.Entry).FirstOrDefault();
                string operation = contract.Operations.Of(route);
                if (entry == null || entry.Operation != operation || entry.Status != 404 || entry.Epoch != contract.Epoch || entry.Fault != null)
                {
                    return $"the journal does not show request {witness.Sequence} as {Shown.Of(operation)} answered 404 ({(entry == null ? "no entry" : $"journal seq {entry.Seq}: {Shown.Of(entry.Operation)}, {entry.Status}, epoch {Shown.Of(entry.Epoch)}{(entry.Fault == null ? "" : $", {entry.Fault}")}")})";
                }
                return null;
            }
            catch (Exception e)
            {
                return $"the journal could not be read: {Diagnostics.Describe(e)}";
            }
        }

        private async Task FinalJoinAsync(CancellationToken consumerToken)
        {
            recorder.Position = 0;
            string problem;
            try
            {
                problem = await JoinAsync(final: true, consumerToken);
            }
            catch (Exception e)
            {
                problem = $"journal: the final journal could not be read: {Diagnostics.Describe(e)}";
            }
            if (problem == null)
            {
                outcome.FinalJoinPassed = true;
            }
            else
            {
                outcome.Fail(null, problem);
            }
        }

        // The journal as it is now, joined to every request the client has sent; null when nothing is wrong.
        private async Task<string> JoinAsync(bool final, CancellationToken token)
        {
            JournalAccount account = await ReadJournalAsync(final, token);
            return account.Problems.Count == 0 ? null : Summarize(account);
        }

        // Reads the journal and joins it to every request the client has sent, under the run's journal contract.
        private async Task<JournalAccount> ReadJournalAsync(bool final, CancellationToken token)
        {
            JsonObject journal = await environment.ControlAsync("journal", token);
            JournalAccount account = JournalJoin.Join(journal, baselineSeq, contract, recorder.All, final);
            outcome.Joined(account);
            return account;
        }

        private static string Summarize(JournalAccount account)
        {
            string more = account.Problems.Count > 5 ? $"; and {account.Problems.Count - 5} more" : "";
            return "journal: " + string.Join("; ", account.Problems.Take(5)) + more;
        }

        private async Task DisposeAsync()
        {
            try
            {
                await Client.DisposeAsync();
            }
            catch (Exception e)
            {
                outcome.Fail(null, $"the client could not be disposed: {Diagnostics.Describe(e)}");
            }
            try
            {
                System.IO.Directory.Delete(scratch, recursive: true);
            }
            catch (Exception e) when (e is System.IO.IOException || e is UnauthorizedAccessException)
            {
                // The scratch directory is private and temporary; failing to remove it changes no outcome.
            }
        }

        private ScenarioFixture Fixture(string name)
        {
            return fixtures.TryGetValue(name, out ScenarioFixture fixture) ? fixture : throw new ScenarioFailure($"the case has no fixture {name}");
        }

        internal static byte[] RecipeBytes(ScenarioFixture fixture)
        {
            if (fixture.Recipe == null)
            {
                throw new ScenarioFailure($"{fixture.Name} has no recipe");
            }
            byte[] bytes = fixture.Recipe.Bytes();
            string sha256 = ScenarioDigest.Sha256(bytes);
            if (sha256 != fixture.Recipe.Sha256)
            {
                throw new ScenarioFailure($"recipe {fixture.Recipe.Number} makes bytes with SHA-256 {sha256}, not the definition's {fixture.Recipe.Sha256}");
            }
            return bytes;
        }

        internal static void CheckBytes(ScenarioFixture fixture, byte[] actual, byte[] expected, string what)
        {
            if (actual.Length != expected.Length || ScenarioDigest.Sha256(actual) != fixture.Recipe.Sha256)
            {
                throw new ScenarioFailure($"{what} holds {actual.Length} bytes with SHA-256 {ScenarioDigest.Sha256(actual)}, not recipe {fixture.Recipe.Number}'s {expected.Length} bytes with {fixture.Recipe.Sha256}");
            }
        }

        private string ScratchPath(string prefix)
        {
            return System.IO.Path.Combine(scratch, $"{prefix}-{++scratchNames}");
        }

        internal static string Local(string root, string relativePath)
        {
            return System.IO.Path.Combine(root, relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
        }
    }
}