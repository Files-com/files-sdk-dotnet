using FilesCom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace FilesApiScenarioTests
{
    // One test for each scenario of verification/scenarios/api.json, generated from it and the schema. Each runs only
    // under api-scenarios.rb, whose launcher (the generated mock server's with-scenario.rb) starts a simulator of its own
    // for it under the api-scenarios-canonical preset; anywhere else it is not run (inconclusive). Positions that only a
    // Files.com site can carry out are excluded, never credited.
    [TestClass]
    public class ApiScenarioTests
    {
        [TestMethod]
        public async Task FilesOwnedFolderNamespace()
        {
            await ScenarioRun.ExecuteAsync(ApiScenarioCases.FilesOwnedFolderNamespace());
        }

        [TestMethod]
        public async Task FilesOwnedBinaryRoundtrip()
        {
            await ScenarioRun.ExecuteAsync(ApiScenarioCases.FilesOwnedBinaryRoundtrip());
        }

        [TestMethod]
        public async Task FilesOwnedNamespaceExtended()
        {
            await ScenarioRun.ExecuteAsync(ApiScenarioCases.FilesOwnedNamespaceExtended());
        }

        [TestMethod]
        public async Task FilesOwnedDirectoryHelpers()
        {
            await ScenarioRun.ExecuteAsync(ApiScenarioCases.FilesOwnedDirectoryHelpers());
        }
    }

    // The generated cases, one for each scenario of the definition this SHA-256 names, every position expanded.
    internal static class ApiScenarioCases
    {
        internal const string DefinitionSha256 = "5d9b8eff0ef0724a68a458db77dc85784bb7d24436f104b98c2a2fe67086bbe0";

        internal static ScenarioCase[] All()
        {
            return new[] { FilesOwnedFolderNamespace(), FilesOwnedBinaryRoundtrip(), FilesOwnedNamespaceExtended(), FilesOwnedDirectoryHelpers() };
        }

        internal static ScenarioCase FilesOwnedFolderNamespace()
        {
            return new ScenarioCase(
                "files.owned-folder-namespace",
                "FilesApiScenarioTests.ApiScenarioTests.FilesOwnedFolderNamespace",
                DefinitionSha256,
                new[]
                {
                    new ScenarioFixture("folder", "directory", "", null),
                    new ScenarioFixture("alpha", "directory", "alpha", null),
                    new ScenarioFixture("bravo", "directory", "bravo", null),
                    new ScenarioFixture("charlie", "directory", "charlie", null),
                    new ScenarioFixture("alpha-inner", "directory", "alpha/inner", null),
                },
                new ScenarioStep[]
                {
                    new OperationStep(1, "GET /file_actions/metadata/{path} \"/\": 422 bad-request/request-params-required", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = "/" },
                        null, false,
                        new ScenarioExpectation(422, "bad-request/request-params-required", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync("/", null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(2, "GET /folders/{path} \"/\": 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = "/" },
                        null, false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(null, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor("/", new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new ExcludedStep(3, "GET /behaviors/folders/{path} \"/\": 200", "site"),
                    new OperationStep(4, "GET /file_actions/metadata/{path} folder: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(404, "not-found", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(5, "POST /folders/{path} folder: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", true,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("folder", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("folder"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new ExcludedStep(6, "GET /file_actions/metadata/{path} folder: 200", "site"),
                    new OperationStep(7, "POST /folders/{path} alpha: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha") },
                        "alpha", false,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("alpha", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("alpha"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(8, "POST /folders/{path} bravo: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("bravo") },
                        "bravo", false,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("bravo", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("bravo"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(9, "POST /folders/{path} charlie: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("charlie") },
                        "charlie", false,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("charlie", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("charlie"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(10, "POST /folders/{path} alpha-inner: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha-inner") },
                        "alpha-inner", false,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("alpha-inner", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("alpha-inner"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(11, "POST /folders/{path} alpha: 422 processing-failure/destination-exists", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha") },
                        "alpha", false,
                        new ScenarioExpectation(422, "processing-failure/destination-exists", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.Folders.CreateAsync(run.PathOf("alpha"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(12, "GET /file_actions/metadata/{path} alpha: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha") },
                        "alpha", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("alpha", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("alpha"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(13, "GET /file_actions/metadata/{path} alpha-inner: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha-inner") },
                        "alpha-inner", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("alpha-inner", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("alpha-inner"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(14, "GET /folders/{path} folder: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "alpha", "bravo", "charlie" }, 1, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("folder"), new Dictionary<string, object> { ["per_page"] = 1L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new OperationStep(15, "GET /folders/{path} alpha: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha") },
                        "alpha", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "alpha-inner" }, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("alpha"), new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new OperationStep(16, "DELETE /files/{path} alpha-inner: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha-inner") },
                        "alpha-inner", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("alpha-inner"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(17, "GET /file_actions/metadata/{path} alpha-inner: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha-inner") },
                        "alpha-inner", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("alpha-inner"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(18, "DELETE /files/{path} alpha: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha") },
                        "alpha", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("alpha"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(19, "GET /file_actions/metadata/{path} alpha: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("alpha") },
                        "alpha", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("alpha"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(20, "DELETE /files/{path} bravo: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("bravo") },
                        "bravo", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("bravo"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(21, "GET /file_actions/metadata/{path} bravo: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("bravo") },
                        "bravo", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("bravo"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(22, "DELETE /files/{path} charlie: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("charlie") },
                        "charlie", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("charlie"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(23, "GET /file_actions/metadata/{path} charlie: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("charlie") },
                        "charlie", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("charlie"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(24, "DELETE /files/{path} folder: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("folder"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(25, "GET /file_actions/metadata/{path} folder: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                            return StepResult.None;
                        }),
                },
                "DELETE /files/{path}",
                async (run, token) =>
                {
                    await run.Client.RemoteFiles.DeleteAsync(run.PathOf("folder"), new Dictionary<string, object> { ["recursive"] = true }, null, token);
                },
                "GET /file_actions/metadata/{path}",
                async (run, token) =>
                {
                    await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                });
        }

        internal static ScenarioCase FilesOwnedBinaryRoundtrip()
        {
            return new ScenarioCase(
                "files.owned-binary-roundtrip",
                "FilesApiScenarioTests.ApiScenarioTests.FilesOwnedBinaryRoundtrip",
                DefinitionSha256,
                new[]
                {
                    new ScenarioFixture("folder", "directory", "", null),
                    new ScenarioFixture("binary.bin", "file", "binary.bin", new ScenarioRecipe(1, 131, 17, 251, 65539, "b65094d36ac1009dac9c73c27b48638a47eb7f23d219a80d1b4d05b58f514499")),
                    new ScenarioFixture("empty.bin", "file", "empty.bin", new ScenarioRecipe(2, 1, 0, 256, 0, "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855")),
                    new ScenarioFixture("short-read.bin", "file", "short-read.bin", new ScenarioRecipe(3, 29, 101, 256, 65537, "ed5a86d5b614d5385a85c170c15f25fe75c3faab6a8b29abbb470a6c51faa7f9")),
                },
                new ScenarioStep[]
                {
                    new OperationStep(1, "GET /file_actions/metadata/{path} \"/\": 422 bad-request/request-params-required", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = "/" },
                        null, false,
                        new ScenarioExpectation(422, "bad-request/request-params-required", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync("/", null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(2, "GET /folders/{path} \"/\": 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = "/" },
                        null, false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(null, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor("/", new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new ExcludedStep(3, "GET /behaviors/folders/{path} \"/\": 200", "site"),
                    new OperationStep(4, "GET /file_actions/metadata/{path} folder: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(404, "not-found", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(5, "POST /folders/{path} folder: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", true,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("folder", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("folder"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new ExcludedStep(6, "GET /file_actions/metadata/{path} folder: 200", "site"),
                    new UploadStep(7, "upload binary.bin (local-file)", null, "binary.bin", "local-file", null, false, "2026-09-24T00:00:00Z", false, 0,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(8, "GET /file_actions/metadata/{path} binary.bin: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("binary.bin") },
                        "binary.bin", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("binary.bin", null, null, new FieldExpectation[] { new FieldExpectation("provided_mtime", "{\"one_of\":[\"2026-09-24T00:00:00Z\",null]}") }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("binary.bin"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size, ("provided_mtime", (object)found.ProvidedMtime)));
                        }),
                    new UploadStep(9, "upload empty.bin (seekable-stream)", null, "empty.bin", "seekable-stream", null, false, "2026-09-24T00:00:00Z", false, 0,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(10, "GET /file_actions/metadata/{path} empty.bin: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("empty.bin") },
                        "empty.bin", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("empty.bin", null, null, new FieldExpectation[] { new FieldExpectation("provided_mtime", "{\"one_of\":[\"2026-09-24T00:00:00Z\",null]}") }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("empty.bin"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size, ("provided_mtime", (object)found.ProvidedMtime)));
                        }),
                    new UploadStep(11, "upload short-read.bin (forward-only-stream)", null, "short-read.bin", "forward-only-stream", 3, false, "2026-09-24T00:00:00Z", false, 0,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(12, "GET /file_actions/metadata/{path} short-read.bin: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("short-read.bin") },
                        "short-read.bin", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("short-read.bin", null, null, new FieldExpectation[] { new FieldExpectation("provided_mtime", "{\"one_of\":[\"2026-09-24T00:00:00Z\",null]}") }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("short-read.bin"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size, ("provided_mtime", (object)found.ProvidedMtime)));
                        }),
                    new OperationStep(13, "GET /folders/{path} folder: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "binary.bin", "empty.bin", "short-read.bin" }, 1, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("folder"), new Dictionary<string, object> { ["per_page"] = 1L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new DownloadStep(14, "download binary.bin", null, "binary.bin", false, new[] { "GET /files/{path}" }),
                    new DownloadStep(15, "download empty.bin", null, "empty.bin", false, new[] { "GET /files/{path}" }),
                    new DownloadStep(16, "download short-read.bin", null, "short-read.bin", false, new[] { "GET /files/{path}" }),
                    new OperationStep(17, "DELETE /files/{path} binary.bin: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("binary.bin") },
                        "binary.bin", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("binary.bin"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(18, "GET /file_actions/metadata/{path} binary.bin: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("binary.bin") },
                        "binary.bin", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("binary.bin"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(19, "DELETE /files/{path} empty.bin: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("empty.bin") },
                        "empty.bin", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("empty.bin"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(20, "GET /file_actions/metadata/{path} empty.bin: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("empty.bin") },
                        "empty.bin", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("empty.bin"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(21, "DELETE /files/{path} short-read.bin: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("short-read.bin") },
                        "short-read.bin", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("short-read.bin"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(22, "GET /file_actions/metadata/{path} short-read.bin: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("short-read.bin") },
                        "short-read.bin", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("short-read.bin"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(23, "DELETE /files/{path} folder: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("folder"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(24, "GET /file_actions/metadata/{path} folder: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                            return StepResult.None;
                        }),
                },
                "DELETE /files/{path}",
                async (run, token) =>
                {
                    await run.Client.RemoteFiles.DeleteAsync(run.PathOf("folder"), new Dictionary<string, object> { ["recursive"] = true }, null, token);
                },
                "GET /file_actions/metadata/{path}",
                async (run, token) =>
                {
                    await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                });
        }

        internal static ScenarioCase FilesOwnedNamespaceExtended()
        {
            return new ScenarioCase(
                "files.owned-namespace-extended",
                "FilesApiScenarioTests.ApiScenarioTests.FilesOwnedNamespaceExtended",
                DefinitionSha256,
                new[]
                {
                    new ScenarioFixture("folder", "directory", "", null),
                    new ScenarioFixture("hash-name", "file", "a#b.txt", new ScenarioRecipe(4, 37, 5, 256, 1031, "29f483b8739ae2de994d4794133a7a858a0f0416caa03e5a96165af41ebdf3e8")),
                    new ScenarioFixture("percent-name", "file", "100%.txt", new ScenarioRecipe(5, 41, 11, 256, 2053, "83298104fe01d905ee9ce8fae2f23da08007b5e8ab356d5bfc064b49c2664b98")),
                    new ScenarioFixture("unicode-name", "file", "é +.txt", new ScenarioRecipe(6, 43, 3, 256, 517, "b664357b3151e48ca4c5c6c452dd20d58293897f887536f6e0e0627c0d8cd25b")),
                    new ScenarioFixture("nested", "directory", "nested", null),
                    new ScenarioFixture("inner.bin", "file", "nested/inner.bin", new ScenarioRecipe(7, 47, 19, 256, 4099, "9f251477458622d9da868e31b8708ec638d8ba9a01c9b575bb8c0a0764734521")),
                    new ScenarioFixture("large.bin", "file", "large.bin", new ScenarioRecipe(8, 53, 23, 251, 6291459, "4ab1615925cb4b1fa9b9ddc3345a920200888d8ba7d3ae1b880aa6c0c19f87fa")),
                    new ScenarioFixture("copied.bin", "file", "copied.bin", new ScenarioRecipe(7, 47, 19, 256, 4099, "9f251477458622d9da868e31b8708ec638d8ba9a01c9b575bb8c0a0764734521")),
                    new ScenarioFixture("moved.bin", "file", "nested/moved.bin", new ScenarioRecipe(7, 47, 19, 256, 4099, "9f251477458622d9da868e31b8708ec638d8ba9a01c9b575bb8c0a0764734521")),
                    new ScenarioFixture("cancelled.bin", "file", "cancelled.bin", new ScenarioRecipe(9, 59, 29, 251, 6291461, "d7a71574a24faecb2fea4915b8096a86a43b19db98d440f16730caac0a66cfd4")),
                },
                new ScenarioStep[]
                {
                    new OperationStep(1, "GET /file_actions/metadata/{path} \"/\": 422 bad-request/request-params-required", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = "/" },
                        null, false,
                        new ScenarioExpectation(422, "bad-request/request-params-required", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync("/", null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(2, "GET /folders/{path} \"/\": 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = "/" },
                        null, false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(null, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor("/", new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new ExcludedStep(3, "GET /behaviors/folders/{path} \"/\": 200", "site"),
                    new OperationStep(4, "GET /file_actions/metadata/{path} folder: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(404, "not-found", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(5, "POST /folders/{path} folder: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", true,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("folder", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("folder"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new ExcludedStep(6, "GET /file_actions/metadata/{path} folder: 200", "site"),
                    new UploadStep(7, "upload hash-name (local-file)", null, "hash-name", "local-file", null, false, null, false, 0,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(8, "GET /file_actions/metadata/{path} hash-name: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("hash-name") },
                        "hash-name", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("hash-name", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("hash-name"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new UploadStep(9, "upload percent-name (local-file)", null, "percent-name", "local-file", null, false, null, false, 0,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(10, "GET /file_actions/metadata/{path} percent-name: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("percent-name") },
                        "percent-name", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("percent-name", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("percent-name"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new UploadStep(11, "upload unicode-name (local-file)", null, "unicode-name", "local-file", null, false, null, false, 0,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(12, "GET /file_actions/metadata/{path} unicode-name: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("unicode-name") },
                        "unicode-name", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("unicode-name", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("unicode-name"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(13, "POST /folders/{path} nested: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("nested") },
                        "nested", false,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("nested", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("nested"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new UploadStep(14, "upload inner.bin (seekable-stream)", null, "inner.bin", "seekable-stream", null, false, null, false, 0,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(15, "GET /file_actions/metadata/{path} inner.bin: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("inner.bin") },
                        "inner.bin", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("inner.bin", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("inner.bin"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new UploadStep(16, "upload large.bin (local-file)", null, "large.bin", "local-file", null, false, null, true, 2,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(17, "GET /file_actions/metadata/{path} large.bin: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("large.bin") },
                        "large.bin", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("large.bin", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("large.bin"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new FileActionStep(18, "copy copied.bin from inner.bin", null, "copy", "inner.bin", "copied.bin", new[] { "POST /file_actions/copy/{path}", "GET /file_migrations/{id}" },
                        (run, token) => run.Client.RemoteFiles.CopyAsync(run.PathOf("inner.bin"), new Dictionary<string, object> { ["destination"] = run.PathOf("copied.bin") }, null, token)),
                    new OperationStep(19, "GET /file_actions/metadata/{path} copied.bin: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("copied.bin") },
                        "copied.bin", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("copied.bin", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("copied.bin"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new FileActionStep(20, "move moved.bin from copied.bin", null, "move", "copied.bin", "moved.bin", new[] { "POST /file_actions/move/{path}", "GET /file_migrations/{id}" },
                        (run, token) => run.Client.RemoteFiles.MoveAsync(run.PathOf("copied.bin"), new Dictionary<string, object> { ["destination"] = run.PathOf("moved.bin") }, null, token)),
                    new OperationStep(21, "GET /file_actions/metadata/{path} copied.bin: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("copied.bin") },
                        "copied.bin", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("copied.bin"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(22, "GET /file_actions/metadata/{path} moved.bin: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved.bin") },
                        "moved.bin", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("moved.bin", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("moved.bin"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(23, "GET /folders/{path} folder: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "hash-name", "percent-name", "unicode-name", "nested", "large.bin" }, 1, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("folder"), new Dictionary<string, object> { ["per_page"] = 1L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new DownloadStep(24, "download hash-name (listed-entry)", null, "hash-name", true, new[] { "GET /files/{path}" }),
                    new DownloadStep(25, "download percent-name (listed-entry)", null, "percent-name", true, new[] { "GET /files/{path}" }),
                    new DownloadStep(26, "download unicode-name (listed-entry)", null, "unicode-name", true, new[] { "GET /files/{path}" }),
                    new DownloadStep(27, "download large.bin (listed-entry)", null, "large.bin", true, new[] { "GET /files/{path}" }),
                    new DownloadStep(28, "download inner.bin", null, "inner.bin", false, new[] { "GET /files/{path}" }),
                    new DownloadStep(29, "download moved.bin", null, "moved.bin", false, new[] { "GET /files/{path}" }),
                    new DownloadStep(30, "download large.bin", null, "large.bin", false, new[] { "GET /files/{path}" }),
                    new UploadStep(31, "upload cancelled.bin (local-file), cancelled after its first acknowledged part", null, "cancelled.bin", "local-file", null, true, null, false, 0,
                        "POST /file_actions/begin_upload/{path}", "POST /files/{path}"),
                    new OperationStep(32, "GET /file_actions/metadata/{path} cancelled.bin: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("cancelled.bin") },
                        "cancelled.bin", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("cancelled.bin"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(33, "DELETE /files/{path} moved.bin: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved.bin") },
                        "moved.bin", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("moved.bin"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(34, "GET /file_actions/metadata/{path} moved.bin: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved.bin") },
                        "moved.bin", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("moved.bin"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(35, "DELETE /files/{path} inner.bin: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("inner.bin") },
                        "inner.bin", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("inner.bin"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(36, "GET /file_actions/metadata/{path} inner.bin: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("inner.bin") },
                        "inner.bin", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("inner.bin"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(37, "DELETE /files/{path} nested: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("nested") },
                        "nested", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("nested"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(38, "GET /file_actions/metadata/{path} nested: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("nested") },
                        "nested", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("nested"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(39, "DELETE /files/{path} large.bin: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("large.bin") },
                        "large.bin", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("large.bin"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(40, "GET /file_actions/metadata/{path} large.bin: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("large.bin") },
                        "large.bin", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("large.bin"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(41, "DELETE /files/{path} hash-name: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("hash-name") },
                        "hash-name", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("hash-name"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(42, "GET /file_actions/metadata/{path} hash-name: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("hash-name") },
                        "hash-name", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("hash-name"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(43, "DELETE /files/{path} percent-name: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("percent-name") },
                        "percent-name", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("percent-name"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(44, "GET /file_actions/metadata/{path} percent-name: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("percent-name") },
                        "percent-name", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("percent-name"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(45, "DELETE /files/{path} unicode-name: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("unicode-name") },
                        "unicode-name", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("unicode-name"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(46, "GET /file_actions/metadata/{path} unicode-name: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("unicode-name") },
                        "unicode-name", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("unicode-name"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(47, "DELETE /files/{path} folder: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("folder"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(48, "GET /file_actions/metadata/{path} folder: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                            return StepResult.None;
                        }),
                },
                "DELETE /files/{path}",
                async (run, token) =>
                {
                    await run.Client.RemoteFiles.DeleteAsync(run.PathOf("folder"), new Dictionary<string, object> { ["recursive"] = true }, null, token);
                },
                "GET /file_actions/metadata/{path}",
                async (run, token) =>
                {
                    await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                });
        }

        internal static ScenarioCase FilesOwnedDirectoryHelpers()
        {
            return new ScenarioCase(
                "files.owned-directory-helpers",
                "FilesApiScenarioTests.ApiScenarioTests.FilesOwnedDirectoryHelpers",
                DefinitionSha256,
                new[]
                {
                    new ScenarioFixture("folder", "directory", "", null),
                    new ScenarioFixture("up", "directory", "up", null),
                    new ScenarioFixture("up-a", "file", "up/a.bin", new ScenarioRecipe(10, 61, 7, 256, 1543, "983f31356cea338492c47d6c9c979deb3a45914fe6aa8eafa96cd44e02f08e9d")),
                    new ScenarioFixture("up-sub", "directory", "up/sub", null),
                    new ScenarioFixture("up-b", "file", "up/sub/b.bin", new ScenarioRecipe(11, 67, 13, 256, 2089, "03e528690696066e61d8a65fa0fac38974bb95cb0d31874240287f325484fddb")),
                    new ScenarioFixture("up-empty", "directory", "up/empty", null),
                    new ScenarioFixture("copy", "directory", "copy", null),
                    new ScenarioFixture("copy-b", "file", "copy/b.bin", new ScenarioRecipe(11, 67, 13, 256, 2089, "03e528690696066e61d8a65fa0fac38974bb95cb0d31874240287f325484fddb")),
                    new ScenarioFixture("moved", "directory", "moved", null),
                    new ScenarioFixture("moved-b", "file", "moved/b.bin", new ScenarioRecipe(11, 67, 13, 256, 2089, "03e528690696066e61d8a65fa0fac38974bb95cb0d31874240287f325484fddb")),
                },
                new ScenarioStep[]
                {
                    new OperationStep(1, "GET /file_actions/metadata/{path} \"/\": 422 bad-request/request-params-required", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = "/" },
                        null, false,
                        new ScenarioExpectation(422, "bad-request/request-params-required", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync("/", null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(2, "GET /folders/{path} \"/\": 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = "/" },
                        null, false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(null, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor("/", new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new ExcludedStep(3, "GET /behaviors/folders/{path} \"/\": 200", "site"),
                    new OperationStep(4, "GET /file_actions/metadata/{path} folder: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(404, "not-found", null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(5, "POST /folders/{path} folder: 201", null, "POST /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", true,
                        new ScenarioExpectation(201, null, null, null, new EntryExpectation("folder", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.Folders.CreateAsync(run.PathOf("folder"), new Dictionary<string, object> { ["mkdir_parents"] = false }, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new ExcludedStep(6, "GET /file_actions/metadata/{path} folder: 200", "site"),
                    new UploadDirectoryStep(7, "upload-directory up", null, "up",
                        new TreeFile[] { new TreeFile("a.bin", "up-a"), new TreeFile("sub/b.bin", "up-b") },
                        new string[] { "empty", "sub" },
                        new string[] { "empty" },
                        new string[] { "up-a", "up-b" },
                        2, 1, 3632,
                        new[] { "POST /folders/{path}", "POST /file_actions/begin_upload/{path}", "POST /files/{path}" }, "POST /files/{path}"),
                    new OperationStep(8, "GET /file_actions/metadata/{path} up: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up") },
                        "up", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("up", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("up"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(9, "GET /file_actions/metadata/{path} up-a: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-a") },
                        "up-a", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("up-a", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("up-a"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(10, "GET /file_actions/metadata/{path} up-sub: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-sub") },
                        "up-sub", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("up-sub", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("up-sub"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(11, "GET /file_actions/metadata/{path} up-b: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-b") },
                        "up-b", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("up-b", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("up-b"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(12, "GET /file_actions/metadata/{path} up-empty: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-empty") },
                        "up-empty", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("up-empty", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("up-empty"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(13, "GET /folders/{path} up: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up") },
                        "up", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "up-a", "up-sub", "up-empty" }, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("up"), new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new OperationStep(14, "GET /folders/{path} up-sub: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-sub") },
                        "up-sub", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "up-b" }, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("up-sub"), new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new OperationStep(15, "GET /folders/{path} up-empty: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-empty") },
                        "up-empty", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] {  }, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("up-empty"), new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new DownloadDirectoryStep(16, "download-directory up", null, "up", 2, 1, 3632,
                        new string[] { "empty", "sub" }, new string[] { "empty" }, new string[] { "a.bin", "sub/b.bin" },
                        new TreeFile[] { new TreeFile("a.bin", "up-a"), new TreeFile("sub/b.bin", "up-b") },
                        new[] { "GET /folders/{path}", "GET /files/{path}" }),
                    new FileActionStep(17, "copy copy from up-sub", null, "copy", "up-sub", "copy", new[] { "POST /file_actions/copy/{path}", "GET /file_migrations/{id}" },
                        (run, token) => run.Client.RemoteFiles.CopyAsync(run.PathOf("up-sub"), new Dictionary<string, object> { ["destination"] = run.PathOf("copy") }, null, token)),
                    new OperationStep(18, "GET /file_actions/metadata/{path} copy: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("copy") },
                        "copy", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("copy", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("copy"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(19, "GET /file_actions/metadata/{path} copy-b: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("copy-b") },
                        "copy-b", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("copy-b", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("copy-b"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(20, "GET /folders/{path} copy: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("copy") },
                        "copy", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "copy-b" }, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("copy"), new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new FileActionStep(21, "move moved from copy", null, "move", "copy", "moved", new[] { "POST /file_actions/move/{path}", "GET /file_migrations/{id}" },
                        (run, token) => run.Client.RemoteFiles.MoveAsync(run.PathOf("copy"), new Dictionary<string, object> { ["destination"] = run.PathOf("moved") }, null, token)),
                    new OperationStep(22, "GET /file_actions/metadata/{path} copy-b: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("copy-b") },
                        "copy-b", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("copy-b"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(23, "GET /file_actions/metadata/{path} copy: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("copy") },
                        "copy", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("copy"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(24, "GET /file_actions/metadata/{path} moved: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved") },
                        "moved", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("moved", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("moved"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(25, "GET /file_actions/metadata/{path} moved-b: 200", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved-b") },
                        "moved-b", false,
                        new ScenarioExpectation(200, null, null, null, new EntryExpectation("moved-b", null, null, new FieldExpectation[] {  }), null),
                        async (run, token) =>
                        {
                            var found = await run.Client.RemoteFiles.FindAsync(run.PathOf("moved-b"), null, null, token);
                            return StepResult.Of(new ScenarioEntry(found.Path, found.Type, found.Size));
                        }),
                    new OperationStep(26, "GET /folders/{path} moved: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved") },
                        "moved", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "moved-b" }, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("moved"), new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new OperationStep(27, "GET /folders/{path} folder: 200", null, "GET /folders/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(200, null, null, null, null, new ListingExpectation(new string[] { "up", "moved" }, 1000, 16, null)),
                        async (run, token) =>
                        {
                            return await run.ListAsync(run.Client.Folders.ListFor(run.PathOf("folder"), new Dictionary<string, object> { ["per_page"] = 1000L }, null), item => new ScenarioEntry(item.Path, item.Type, item.Size), 16, token);
                        }),
                    new DownloadStep(28, "download moved-b", null, "moved-b", false, new[] { "GET /files/{path}" }),
                    new OperationStep(29, "DELETE /files/{path} moved-b: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved-b") },
                        "moved-b", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("moved-b"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(30, "GET /file_actions/metadata/{path} moved-b: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved-b") },
                        "moved-b", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("moved-b"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(31, "DELETE /files/{path} moved: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved") },
                        "moved", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("moved"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(32, "GET /file_actions/metadata/{path} moved: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("moved") },
                        "moved", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("moved"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(33, "DELETE /files/{path} up-b: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-b") },
                        "up-b", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("up-b"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(34, "GET /file_actions/metadata/{path} up-b: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-b") },
                        "up-b", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("up-b"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(35, "DELETE /files/{path} up-sub: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-sub") },
                        "up-sub", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("up-sub"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(36, "GET /file_actions/metadata/{path} up-sub: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-sub") },
                        "up-sub", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("up-sub"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(37, "DELETE /files/{path} up-empty: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-empty") },
                        "up-empty", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("up-empty"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(38, "GET /file_actions/metadata/{path} up-empty: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-empty") },
                        "up-empty", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("up-empty"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(39, "DELETE /files/{path} up-a: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-a") },
                        "up-a", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("up-a"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(40, "GET /file_actions/metadata/{path} up-a: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up-a") },
                        "up-a", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("up-a"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(41, "DELETE /files/{path} up: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up") },
                        "up", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("up"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(42, "GET /file_actions/metadata/{path} up: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("up") },
                        "up", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("up"), null, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(43, "DELETE /files/{path} folder: 204", null, "DELETE /files/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(204, null, null, null, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.DeleteAsync(run.PathOf("folder"), new Dictionary<string, object> { ["recursive"] = false }, null, token);
                            return StepResult.None;
                        }),
                    new OperationStep(44, "GET /file_actions/metadata/{path} folder: 404 not-found", null, "GET /file_actions/metadata/{path}", run => new Dictionary<string, string> { ["path"] = run.PathOf("folder") },
                        "folder", false,
                        new ScenarioExpectation(404, "not-found", 5, 1, null, null),
                        async (run, token) =>
                        {
                            await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                            return StepResult.None;
                        }),
                },
                "DELETE /files/{path}",
                async (run, token) =>
                {
                    await run.Client.RemoteFiles.DeleteAsync(run.PathOf("folder"), new Dictionary<string, object> { ["recursive"] = true }, null, token);
                },
                "GET /file_actions/metadata/{path}",
                async (run, token) =>
                {
                    await run.Client.RemoteFiles.FindAsync(run.PathOf("folder"), null, null, token);
                });
        }
    }

    // Controls of the consumer's own decisions, which api-scenarios.rb runs on each framework before the cases: the real
    // binding, request binding, journal join, upload and cancellation logic on data in the simulator's own shapes made
    // here, the real transport observer and progress callback in the order the SDK drives them, and the first generated
    // scenario run by the real SDK against a stub of the Files.com API at the network boundary.
    [TestClass]
    public class ScenarioControls
    {
        private const string Category = "ScenarioControls";
        // The upload the cancellation controls account for, and the routes the definition's uploads rely on.
        private const string CancelledPath = "ns/cancelled.bin";
        private const string BeginRoute = "POST /file_actions/begin_upload/{path}";
        private const string CompletionRoute = "POST /files/{path}";
        // The first scenario's lookup of its owned folder before it creates it.
        private const int Lookup = 4;
        // The begin_upload of CancelledPath as the SDK sends it: RemoteFile.BeginUploadCore escapes its path parameter
        // whole, so the nested path goes as ns%2Fcancelled.bin. The real SDK's requests carry it, and so do the
        // controls' own requests and journal entries; ApiRoutes and the journal join read it as CancelledPath.
        private static readonly string BeginPath = ApiRoutes.ApiPrefix + ApiRoutes.TemplateOf(BeginRoute).Replace("{path}", Uri.EscapeDataString(CancelledPath), StringComparison.Ordinal);
        private static readonly string CompletionPath = ApiRoutes.ApiPrefix + ApiRoutes.TemplateOf(CompletionRoute).Replace("{path}", CancelledPath, StringComparison.Ordinal);

        [TestMethod]
        [TestCategory(Category)]
        [DataRow("none", "")]
        [DataRow("instance", "the instance is not the one the launcher gave its server")]
        [DataRow("contract", "contract_version is 2, not 3")]
        [DataRow("definition", "the preset's definition is")]
        [DataRow("fixed-part-length", "the preset's fixed_part_length is false, not true")]
        [DataRow("raw-schema", "raw schema is")]
        [DataRow("journal", "the journal's dropped is 1, not 0")]
        [DataRow("reset-instance", "the reset's answer: the instance is not the one the launcher gave its server")]
        [DataRow("reset-definition", "the reset's answer: api_scenarios is")]
        [DataRow("live-epoch", "the readiness the simulator answers now is of epoch 2, not the reset's 1")]
        [DataRow("live-upload-mode", "the live profile's upload mode is \"legacy\", not \"serial\"")]
        [DataRow("live-fixed-part-length", "the live profile's fixed_part_length is null, not true")]
        [DataRow("live-definition", "the live profile's api_scenarios is")]
        [DataRow("journal-epoch", "the journal before the case is of epoch 2, not the reset's 1")]
        [DataRow("journal-definition", "the journal before the case: api_scenarios is")]
        [DataRow("unlisted-operation", "the simulator lists no operation")]
        public void BindingRefusesAnythingButTheCanonicalSimulatorAsItsResetLeftIt(string change, string problem)
        {
            ScenarioCase scenarioCase = ApiScenarioCases.All()[0];
            BindingInputs inputs = StubEnvironment.Inputs(scenarioCase);
            switch (change)
            {
                case "instance":
                    inputs.Ready["instance"] = "another-instance";
                    break;
                case "contract":
                    inputs.ReadyReceipt["contract_version"] = 2;
                    break;
                case "definition":
                    inputs.Scenario["reset"]["profile"]["api_scenarios"]["definition_sha256"] = new string('0', 64);
                    break;
                case "fixed-part-length":
                    inputs.Scenario["reset"]["profile"]["upload"]["fixed_part_length"] = false;
                    break;
                case "raw-schema":
                    inputs.Generation["swagger"]["sha256"] = new string('1', 64);
                    break;
                case "journal":
                    inputs.Journal["dropped"] = 1;
                    break;
                case "reset-instance":
                    inputs.Reset["instance"] = "another-instance";
                    break;
                case "reset-definition":
                    inputs.Reset["api_scenarios"]["definition_sha256"] = new string('0', 64);
                    break;
                case "live-epoch":
                    inputs.Ready["epoch"] = 2;
                    break;
                case "live-upload-mode":
                    inputs.Ready["profile"]["upload"]["mode"] = "legacy";
                    break;
                case "live-fixed-part-length":
                    inputs.Ready["profile"]["upload"].AsObject().Remove("fixed_part_length");
                    break;
                case "live-definition":
                    inputs.Ready["profile"]["api_scenarios"]["definition_sha256"] = new string('0', 64);
                    break;
                case "journal-epoch":
                    inputs.Journal["epoch"] = 2;
                    break;
                case "journal-definition":
                    inputs.Journal["api_scenarios"]["format_version"] = 2;
                    break;
                case "unlisted-operation":
                    string create = scenarioCase.Steps.OfType<OperationStep>().Single(step => step.CreatesNamespace).Route;
                    JsonArray operations = inputs.Ready["operations"].AsArray();
                    operations.Remove(operations.First(operation => (string)operation["method"] == ApiRoutes.MethodOf(create) && (string)operation["path"] == ApiRoutes.ApiPrefix + ApiRoutes.TemplateOf(create)));
                    break;
            }

            List<string> problems = ScenarioBinding.Problems(inputs, scenarioCase);

            if (problem.Length == 0)
            {
                Assert.AreEqual(0, problems.Count, string.Join("; ", problems));
            }
            else
            {
                Assert.IsTrue(problems.Any(found => found.Contains(problem, StringComparison.Ordinal)), $"expected a problem with \"{problem}\": {string.Join("; ", problems)}");
            }
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow("none", "")]
        [DataRow("missing", "has no journal entry")]
        [DataRow("extra", "is no request the SDK sent")]
        [DataRow("status", "but the SDK's request 1 received HTTP 404")]
        [DataRow("method", "is no request the SDK sent")]
        [DataRow("epoch", "(request 1, GET /api/rest/v1/file_actions/metadata/ns) is of epoch 2, not the reset's 1")]
        [DataRow("journal-epoch", "the journal is of epoch 2, not the reset's 1")]
        [DataRow("journal-instance", "the journal is of instance \"another-instance\"")]
        [DataRow("journal-definition", "the journal's api_scenarios is")]
        [DataRow("storage-credential", "to storage")]
        [DataRow("dropped", "the journal dropped 1 requests")]
        [DataRow("fault", "records the fault fault_id \"rule-1\"")]
        [DataRow("held", "records the fault fault_id \"rule-2\", fault_kind \"hold\", hold_ms 1000")]
        [DataRow("delayed", "records the fault fault_id \"rule-3\", fault_kind \"delay\"")]
        [DataRow("unavailable", "records the fault fault_unavailable \"rule-4\"")]
        [DataRow("no-fault-field", "names no fault_id")]
        [DataRow("wrong-operation", "is the operation \"files.download\", which is not what the simulator lists")]
        [DataRow("unintended-operation", "not \"folders.list\", the one its position meant to send")]
        [DataRow("unanswered", "but the SDK's request 2 received no answer: HttpRequestException")]
        [DataRow("unanswered-unjournaled", "the SDK's request 2 (PUT /upload/ref-7/1, no answer: HttpRequestException) has no journal entry")]
        public void TheJournalJoinAccountsForEveryRequestExactlyOnce(string change, string problem)
        {
            JournalContract contract = Contract();
            var lookup = new ObservedRequest(1, 4, "GET", "/api/rest/v1/file_actions/metadata/ns", Array.Empty<string>(), "a", api: true, carriesApiKey: true, apiKeyMatches: true, carriesAuthorization: false);
            lookup.Answered(404, "not-found");
            contract.Intended[1] = "files.metadata";
            var part = new ObservedRequest(2, 5, "PUT", "/upload/ref-7/1", new[] { "X-Files-Signature" }, "b", api: false, carriesApiKey: false, apiKeyMatches: false, carriesAuthorization: false);
            if (change == "unanswered" || change == "unanswered-unjournaled")
            {
                part.Failed("HttpRequestException", cancelled: false);
            }
            else
            {
                part.Answered(200, null);
            }
            var entries = new JsonArray
            {
                EntryOf(11, "GET", "/api/rest/v1/file_actions/metadata/ns", "files.metadata", 404),
                EntryOf(12, "PUT", "/upload/ref-7/1", "transfers.upload_part", 200, upload: 7, part: 1),
            };
            JsonObject journal = StubEnvironment.Journal(entries);
            switch (change)
            {
                case "missing":
                case "unanswered-unjournaled":
                    entries.RemoveAt(1);
                    break;
                case "extra":
                    entries.Add(EntryOf(13, "GET", "/api/rest/v1/folders/ns", "folders.list", 200));
                    break;
                case "status":
                    entries[0]["status"] = 200;
                    break;
                case "method":
                    entries[0]["method"] = "POST";
                    break;
                case "epoch":
                    entries[0]["epoch"] = 2;
                    break;
                case "journal-epoch":
                    journal["epoch"] = 2;
                    break;
                case "journal-instance":
                    journal["instance"] = "another-instance";
                    break;
                case "journal-definition":
                    journal["api_scenarios"]["definition_sha256"] = new string('0', 64);
                    break;
                case "storage-credential":
                    entries[1]["credentials"] = new JsonObject { ["api_key"] = 1 };
                    break;
                case "dropped":
                    journal["dropped"] = 1;
                    break;
                case "fault":
                    entries[1]["fault_id"] = "rule-1";
                    break;
                case "held":
                    entries[1]["fault_id"] = "rule-2";
                    entries[1]["fault_kind"] = "hold";
                    entries[1]["hold_ms"] = 1000;
                    entries[1]["held_ms"] = 1000;
                    entries[1]["hold_ended"] = "elapsed";
                    break;
                case "delayed":
                    entries[1]["fault_id"] = "rule-3";
                    entries[1]["fault_kind"] = "delay";
                    break;
                case "unavailable":
                    entries[1]["fault_unavailable"] = "rule-4";
                    break;
                case "no-fault-field":
                    entries[0].AsObject().Remove("fault_id");
                    break;
                case "wrong-operation":
                    entries[0]["operation"] = "files.download";
                    break;
                case "unintended-operation":
                    contract.Intended[1] = "folders.list";
                    break;
            }

            JournalAccount account = JournalJoin.Join(journal, 10, contract, new[] { lookup, part }, final: true);

            if (problem.Length == 0)
            {
                Assert.AreEqual(0, account.Problems.Count, string.Join("; ", account.Problems));
                Assert.AreEqual(2, account.Joined.Count);
            }
            else
            {
                Assert.IsTrue(account.Problems.Any(found => found.Contains(problem, StringComparison.Ordinal)), $"expected a problem with \"{problem}\": {string.Join("; ", account.Problems)}");
            }
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow(true, "")]
        [DataRow(false, "no part of upload 7 had been answered 2xx when the upload was cancelled")]
        public async Task OnlyAPartAnsweredBeforeTheFirstAcknowledgementCountsForTheCancellation(bool answeredFirst, string problem)
        {
            var recorder = new RequestRecorder(StubEnvironment.Url, StubEnvironment.Key);
            var storage = new HeldStorage();
            using var invoker = new HttpMessageInvoker(new ObservingHandler(recorder, storage));
            using var caller = new CancellationTokenSource();
            var gate = new CancellationGate();
            var progress = new SynchronousProgress(recorder, 0, gate, caller);
            var begin = new HttpRequestMessage(HttpMethod.Post, StubEnvironment.Url + BeginPath);
            begin.Headers.Add("X-FilesAPI-Key", StubEnvironment.Key);
            (await invoker.SendAsync(begin, CancellationToken.None)).Dispose();
            Task<HttpResponseMessage> part = invoker.SendAsync(new HttpRequestMessage(HttpMethod.Put, HeldStorage.Url + "/upload/ref-7/1"), CancellationToken.None);

            // The SDK reports a part acknowledged once its answer has come through the transport; here the part's answer
            // comes before the report of the first acknowledged part, or only after it.
            if (answeredFirst)
            {
                storage.Release();
                (await part).Dispose();
            }
            progress.Observe(6291461, true, 5242880, 1);
            if (!answeredFirst)
            {
                storage.Release();
                (await part).Dispose();
            }

            Assert.IsTrue(caller.IsCancellationRequested, "the report of the first acknowledged part cancels");
            Assert.IsTrue(recorder.All[1].Succeeded, "the part was answered 2xx either way");
            Assert.AreEqual(answeredFirst ? "2" : "", string.Join(",", gate.AnsweredStorage));
            JournalContract contract = Contract();
            JsonObject journal = StubEnvironment.Journal(new JsonArray
            {
                EntryOf(11, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
                EntryOf(12, "PUT", "/upload/ref-7/1", "transfers.upload_part", 200, upload: 7, part: 1),
            });
            JournalAccount account = JournalJoin.Join(journal, 10, contract, recorder.All, final: true);
            UploadJournal upload = UploadEvidence.Read(recorder.All, account, contract.Operations, BeginRoute, CompletionRoute, CancelledPath);
            List<string> problems = account.Problems.Concat(UploadEvidence.Cancelled(false, new OperationCanceledException(), gate, upload, recorder.All, CompletionRoute, CancelledPath, 6291461)).ToList();

            if (problem.Length == 0)
            {
                Assert.AreEqual(0, problems.Count, string.Join("; ", problems));
            }
            else
            {
                Assert.IsTrue(problems.Any(found => found.Contains(problem, StringComparison.Ordinal)), $"expected a problem with \"{problem}\": {string.Join("; ", problems)}");
            }
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow("part-cancelled", "")]
        [DataRow("part-journaled", "")]
        [DataRow("part-transport-failure", "")]
        [DataRow("grant-cancelled", "")]
        [DataRow("grant-journaled", "")]
        [DataRow("late-granted-part", "")]
        [DataRow("part-cancelled-starting", "")]
        [DataRow("never-cancelled", "the upload was never cancelled")]
        [DataRow("completed", "the upload completed although it was cancelled")]
        [DataRow("failed", "not as cancelled")]
        [DataRow("unacknowledged", "no part of upload 7 had been answered 2xx when the upload was cancelled")]
        [DataRow("foreign-upload", "not a part of this step's upload 7")]
        [DataRow("finalized", "asked Files.com to complete the cancelled upload")]
        [DataRow("earlier-transport-cancel", "is not one of the cancelled upload's own attempts: it ended before the caller cancelled")]
        [DataRow("earlier-network-failure", "is not one of the cancelled upload's own attempts: it ended before the caller cancelled")]
        [DataRow("ordinary-network-failure", "is not one of the cancelled upload's own attempts: it ended with HttpRequestException while its own token was not cancelled")]
        [DataRow("timeout-before-the-caller-cancels", "is not one of the cancelled upload's own attempts: it ended before the caller cancelled")]
        [DataRow("timeout-then-the-caller-cancels", "while the caller's own token was not yet cancelled, so the caller's cancellation did not end it")]
        [DataRow("transport-ended-before-cancel", "is not one of the cancelled upload's own attempts: its transport had already ended it, with HttpRequestException, when the cancellation of its own token reached the transport")]
        [DataRow("wrong-method", "is not one of the cancelled upload's own attempts: it is POST, not the PUT that stores a part")]
        [DataRow("bad-part", "is not one of the cancelled upload's own attempts: its part 3 is not one of the upload's parts 1 to 2")]
        [DataRow("credentialed", "is not one of the cancelled upload's own attempts: it carried a credential to storage")]
        [DataRow("wrong-origin", "request 3 was cancelled unanswered, and no part of upload 7 the journal shows acknowledged was sent with its ref to its origin and upload path")]
        [DataRow("wrong-upload-path", "request 3 was cancelled unanswered, and no part of upload 7 the journal shows acknowledged was sent with its ref to its origin and upload path")]
        [DataRow("foreign-ref", "request 3 was cancelled unanswered, and no part of upload 7 the journal shows acknowledged was sent with its ref to its origin and upload path")]
        [DataRow("grant-foreign-ref", "request 3 was cancelled unanswered, and no part of upload 7 the journal shows acknowledged was sent with its ref")]
        [DataRow("late-ungranted-part", "is not one of the cancelled upload's own attempts: it began only after the caller cancelled, and no begin_upload of ns/cancelled.bin negotiating its part 2 with its ref was answered before the caller cancelled")]
        [DataRow("late-granted-after-cancel", "is not one of the cancelled upload's own attempts: it began only after the caller cancelled, and no begin_upload of ns/cancelled.bin negotiating its part 2 with its ref was answered before the caller cancelled")]
        [DataRow("late-unlinked-part", "is not one of the cancelled upload's own attempts: it began only after the caller cancelled, with its own token not yet cancelled, so not linked to the caller's")]
        [DataRow("late-grant", "is not one of the cancelled upload's own attempts: it began only after the caller cancelled")]
        [DataRow("unexplained-api", "is not one of the cancelled upload's own attempts: it is neither a part of the upload nor the begin_upload of ns/cancelled.bin that negotiates one")]
        public async Task OnlyACancelledUploadsOwnPendingRequestsMayEndUnanswered(string change, string problem)
        {
            var recorder = new RequestRecorder(StubEnvironment.Url, StubEnvironment.Key);
            using var caller = new CancellationTokenSource();
            using var transport = new CancellationTokenSource();
            // A token linked to the caller's, as the SDK's network wait links each request's (NetworkWatch), which the wait
            // cancels on its own when it runs out.
            using var watch = CancellationTokenSource.CreateLinkedTokenSource(caller.Token);
            recorder.Owner = caller.Token;
            var observing = new ObservingHandler(recorder, new Boundary((request, token) => AnswerAtBoundaryAsync(request, token, caller.Token)));
            using var invoker = new HttpMessageInvoker(observing);
            // Opened once the caller has actually cancelled: until then the handler's observation of a transport that ended
            // the pending request on its own is held (transport-ended-before-cancel), as a continuation the runtime runs late
            // holds it, so that the caller's cancellation reaches the request's token after its transport had ended it.
            var observation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var gate = new CancellationGate();
            var progress = new SynchronousProgress(recorder, 0, gate, caller);
            const string partTwoGrant = "{\"ref\":\"ref-7\",\"part\":2,\"with_direct_connection_info\":true}";
            bool late = change.StartsWith("late-", StringComparison.Ordinal);
            bool timesOut = change.StartsWith("timeout-", StringComparison.Ordinal);

            // The upload's begin_upload and its first part, answered.
            await Settled(invoker.SendAsync(ApiRequest(BeginPath, null, "answer"), CancellationToken.None));
            await Settled(invoker.SendAsync(StorageRequest(HttpMethod.Put, HeldStorage.Url + "/upload/ref-7/1", change == "unacknowledged" ? "answer-500" : "answer"), CancellationToken.None));
            if (change == "late-granted-part")
            {
                // Part 2 granted before the caller cancels, as PartPlanner negotiates the next part while one is being sent.
                await Settled(invoker.SendAsync(ApiRequest(BeginPath, partTwoGrant, "answer"), CancellationToken.None));
            }
            if (late)
            {
                // The caller cancels at the first acknowledged part before the pending request reaches the transport's bottom.
                progress.Observe(6291461, true, 5242880, 1);
            }
            if (change == "late-granted-after-cancel")
            {
                // Part 2 granted only once the caller has cancelled.
                await Settled(invoker.SendAsync(ApiRequest(BeginPath, partTwoGrant, "answer"), CancellationToken.None));
            }
            // The pending request, request 3 (4 after a grant of part 2): pending when the caller cancels at the first
            // acknowledged part, unless it ended before, or reaching the transport's bottom only after (late-).
            HttpRequestMessage pending = change switch
            {
                "grant-cancelled" or "grant-journaled" or "late-grant" => ApiRequest(BeginPath, partTwoGrant, "hold"),
                "grant-foreign-ref" => ApiRequest(BeginPath, "{\"ref\":\"ref-8\",\"part\":2,\"with_direct_connection_info\":true}", "hold"),
                "unexplained-api" => ApiRequest(CompletionPath, "{\"action\":\"end\",\"ref\":\"ref-7\"}", "hold"),
                "part-transport-failure" => StorageRequest(HttpMethod.Put, HeldStorage.Url + "/upload/ref-7/2", "hold-then-transport-failure"),
                "earlier-network-failure" or "transport-ended-before-cancel" => StorageRequest(HttpMethod.Put, HeldStorage.Url + "/upload/ref-7/2", "fail"),
                "ordinary-network-failure" or "timeout-then-the-caller-cancels" => StorageRequest(HttpMethod.Put, HeldStorage.Url + "/upload/ref-7/2", "fail-once-the-caller-cancels"),
                "wrong-method" => StorageRequest(HttpMethod.Post, HeldStorage.Url + "/upload/ref-7/2", "hold"),
                "bad-part" => StorageRequest(HttpMethod.Put, HeldStorage.Url + "/upload/ref-7/3", "hold"),
                "wrong-origin" => StorageRequest(HttpMethod.Put, "http://other-storage.invalid/upload/ref-7/2", "hold"),
                // The same origin, ref and part as the acknowledged part 1 (/upload/ref-7/1), under another upload path.
                "wrong-upload-path" => StorageRequest(HttpMethod.Put, HeldStorage.Url + "/elsewhere/upload/ref-7/2", "hold"),
                "foreign-ref" => StorageRequest(HttpMethod.Put, HeldStorage.Url + "/upload/ref-8/2", "hold"),
                _ => StorageRequest(HttpMethod.Put, HeldStorage.Url + "/upload/ref-7/2", "hold"),
            };
            if (change == "credentialed")
            {
                pending.Headers.Add("X-FilesAPI-Key", StubEnvironment.Key);
            }
            // Set once the transport has ended the pending request on its own and the handler holds its observation of
            // that end (transport-ended-before-cancel).
            var transportEnded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (change == "transport-ended-before-cancel")
            {
                // The transport fails the request at once, on its own; the handler holds its observation of that end
                // until the caller has cancelled, as a continuation the runtime runs late holds it.
                observing.HoldObservation = () =>
                {
                    transportEnded.TrySetResult(true);
                    return observation.Task;
                };
            }
            // Set once the handler holds the pending request's transport as it is about to be started
            // (part-cancelled-starting); opened once the caller has actually cancelled.
            var startHeld = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var start = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (change == "part-cancelled-starting")
            {
                // The handler holds the transport of the pending request, the only one sent from now on, as it is
                // about to be started, until the caller has cancelled, as the runtime can hold a thread there: the
                // cancellation of the request's token, the caller's, reaches the handler before the transport is
                // started, and is relayed at once; the transport is started only then, with the token that relay
                // cancelled, which ends it as it starts ("hold").
                observing.HoldStart = request =>
                {
                    startHeld.TrySetResult(true);
                    return Task.WhenAny(start.Task, Task.Delay(Bound));
                };
            }
            // Sent with the caller's token; with the linked token a network wait runs out on (timeout-); or, where the
            // caller's cancellation must not be what ends it, a token of its own.
            bool callers = change != "earlier-transport-cancel" && change != "ordinary-network-failure" && change != "never-cancelled" && change != "late-unlinked-part";
            Task<Exception> ended = Settled(invoker.SendAsync(pending, timesOut ? watch.Token : callers ? caller.Token : transport.Token));
            if (change == "earlier-transport-cancel" || change == "late-unlinked-part")
            {
                transport.Cancel();
            }
            Exception endedFirst = change == "earlier-transport-cancel" || change == "earlier-network-failure" ? await ended : null;
            if (change == "transport-ended-before-cancel")
            {
                // Its transport failed the request before the caller cancels, and the handler has yet to observe it.
                Assert.IsTrue(await Task.WhenAny(transportEnded.Task, Task.Delay(Bound)) == transportEnded.Task, "the transport ended the pending request on its own before the caller cancelled");
                Assert.IsFalse(ended.IsCompleted, "the handler had not yet observed the end of the pending request");
            }
            if (change == "part-cancelled-starting")
            {
                // The pending request has reached the handler, which holds its transport before it is started.
                Assert.IsTrue(await Task.WhenAny(startHeld.Task, Task.Delay(Bound)) == startHeld.Task, "the handler held the pending request's transport as it was about to be started");
            }
            if (timesOut)
            {
                // Held between the caller's intent and its actual cancellation, the network wait runs out: its token is
                // cancelled while the caller's is not, and the request ends there (timeout-before-the-caller-cancels) or
                // only once the caller has actually cancelled after it (timeout-then-the-caller-cancels).
                progress.BeforeCancel = () =>
                {
                    watch.Cancel();
                    if (change == "timeout-before-the-caller-cancels")
                    {
                        Assert.IsTrue(ended.Wait(Bound), "the request whose wait ran out ended before the caller cancelled");
                    }
                };
            }
            if (change == "never-cancelled")
            {
                transport.Cancel();
            }
            else if (!late)
            {
                // The SDK reports a part acknowledged once its answer has come through the transport.
                progress.Observe(6291461, true, 5242880, 1);
            }
            // The handler observes an end it held only now, after the caller's cancellation.
            observation.TrySetResult(true);
            if (change == "part-cancelled-starting")
            {
                // The caller's cancellation reached the pending request's token while its transport was held, and was
                // relayed at once, before that transport was started.
                ObservedRequest starting = recorder.All[2];
                Assert.IsTrue(!ended.IsCompleted && starting.OwnerCancelledWithToken == true && starting.TransportEndedBeforeToken == false && starting.TokenCancelledAt > gate.OwnerCancelledAt,
                    $"the cancellation of the pending request's token reached the handler at event {Shown.Of(starting.TokenCancelledAt)}, after the caller actually cancelled at event {Shown.Of(gate.OwnerCancelledAt)}, before its transport was started");
            }
            // A transport the handler held (part-cancelled-starting) starts only now, after the caller's cancellation.
            start.TrySetResult(true);
            Assert.IsNotNull(endedFirst ?? await ended, "the pending request ended without an answer");
            Assert.AreEqual(change != "never-cancelled", caller.IsCancellationRequested, "the caller's own token is actually cancelled once the first acknowledged part is reported");
            if (change == "part-cancelled-starting")
            {
                // Its transport, started with the token the relay had cancelled, ended as it started. Its start,
                // tracked only once the relay had been told of it, was not found begun when the caller actually
                // cancelled: nothing of it is taken for a failure from before then.
                ObservedRequest started = recorder.All[2];
                Assert.IsTrue(started.ClientFailure == nameof(TaskCanceledException) && started.FailedBeforeOwner == null && started.FailedAcrossOwner.Count == 0,
                    $"the pending request ended with {Shown.Of(started.ClientFailure)}, {Shown.Of(started.FailedBeforeOwner)} having failed before the caller actually cancelled, and {started.FailedAcrossOwner.Count} of it begun before then found failed after");
            }
            if (change == "finalized")
            {
                await Settled(invoker.SendAsync(ApiRequest(CompletionPath, "{\"action\":\"end\",\"ref\":\"ref-7\"}", "answer-201"), CancellationToken.None));
            }
            var entries = new JsonArray
            {
                EntryOf(11, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
                EntryOf(12, "PUT", "/upload/ref-7/1", "transfers.upload_part", change == "unacknowledged" ? 500 : 200, upload: change == "foreign-upload" ? 8 : 7, part: 1),
            };
            if (change == "part-journaled")
            {
                // The simulator read part 2 before its connection ended, and refused what it read.
                entries.Add(EntryOf(13, "PUT", "/upload/ref-7/2", "transfers.upload_part", 400, upload: 7, part: 2));
            }
            if (change == "grant-journaled")
            {
                // The simulator read the negotiation whole and answered it after its client had gone.
                entries.Add(EntryOf(13, "POST", BeginPath, "files.begin_upload", 200, upload: 7));
            }
            if (change == "late-granted-part" || change == "late-granted-after-cancel")
            {
                // The simulator answered the grant of part 2 that the client received.
                entries.Add(EntryOf(13, "POST", BeginPath, "files.begin_upload", 200, upload: 7));
            }
            if (change == "finalized")
            {
                entries.Add(EntryOf(14, "POST", CompletionPath, "files.finalize_upload", 201, upload: 7));
            }
            bool completed = change == "completed";
            Exception raised = completed ? null : change == "failed" ? new InvalidOperationException("stand-in") : new OperationCanceledException();

            (List<string> problems, UnansweredRequests unanswered, JournalAccount final) = AccountForUpload(Contract(), recorder.All, entries, gate, completed, raised, 6291461);

            if (problem.Length == 0)
            {
                int sequence = change == "late-granted-part" ? 4 : 3;
                Assert.AreEqual(0, problems.Count, string.Join("; ", problems));
                CancelledAttempt attempt = unanswered.Attempts.Single();
                Assert.IsTrue(attempt.Sequence == sequence && attempt.Confirmed && attempt.Part == 2 && attempt.Storage != change.StartsWith("grant-", StringComparison.Ordinal), $"request {sequence} is the upload's own attempt at part 2");
                Assert.AreEqual(late ? 3 : (int?)null, attempt.GrantedBy, "only a part that reached the transport's bottom after the caller cancelled names the grant that shows it was the upload's to send");
                ObservedRequest attempted = recorder.All[sequence - 1];
                Assert.IsNull(attempted.Status, $"request {sequence} keeps the client's missing answer");
                Assert.IsTrue(attempted.OwnerCancelledAtBegin == late && attempted.OwnerCancelledWithToken == true && attempted.TransportEndedBeforeToken == false && attempted.OwnerCancelledAtEnd,
                    $"the caller's own token was actually cancelled when request {sequence}'s token was, with its transport still running, and when it ended");
                JournalEntry entry = final.Joined.Where(pair => pair.Request.Sequence == sequence).Select(pair => pair.Entry).SingleOrDefault();
                Assert.AreEqual(change == "part-journaled" ? 400 : change == "grant-journaled" ? 200 : (int?)null, entry?.Status, "the server's own outcome is kept apart from the client's");
                Assert.AreEqual(entry == null, final.Excused.Any(request => request.Sequence == sequence), "only a confirmed attempt without an entry is excused");
            }
            else
            {
                Assert.IsTrue(problems.Any(found => found.Contains(problem, StringComparison.Ordinal)), $"expected a problem with \"{problem}\": {string.Join("; ", problems)}");
            }
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow(false)]
        [DataRow(true)]
        public async Task TheSdksNextPartNegotiationThatTheCancellationEndsIsTheUploadsOwnAttempt(bool journaled)
        {
            var recorder = new RequestRecorder(StubEnvironment.Url, StubEnvironment.Key);
            var network = new OverlapUpload();
            FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = StubEnvironment.Url, ApiKey = StubEnvironment.Key },
                HttpTransport.FromHandlerFactory(() => new ObservingHandler(recorder, new Boundary(network.AnswerAsync))));
            using var caller = new CancellationTokenSource();
            recorder.Owner = caller.Token;
            var gate = new CancellationGate();
            var progress = new SynchronousProgress(recorder, 0, gate, caller);
            byte[] bytes = { 1, 2, 3, 4, 5, 6 };
            Exception raised = null;
            try
            {
                await client.Transfers.UploadStreamAsync(new System.IO.MemoryStream(bytes, writable: false), bytes.Length, CancelledPath, new FilesCom.Transfers.UploadOptions { Progress = progress }, caller.Token);
            }
            catch (Exception e)
            {
                raised = e;
            }
            finally
            {
                await client.DisposeAsync();
            }
            IReadOnlyList<ObservedRequest> requests = recorder.All;
            string sent = string.Join("; ", requests.Select(request => $"{request.Sequence} {request.Method} {request.Path} {request.Outcome}"));

            // The SDK's own flow: part 2 negotiated while part 1 was being sent, and that negotiation still unanswered when
            // the caller actually cancelled at the first acknowledged part, which is what cancelled its token.
            Assert.IsTrue(raised is OperationCanceledException, $"the upload ended with {(raised == null ? "nothing" : raised.GetType().Name)}, not as cancelled: {sent}");
            Assert.IsTrue(network.PartHeldUntilNextNegotiation, "part 2 was negotiated while part 1 was being sent: " + sent);
            Assert.AreEqual(3, requests.Count, sent);
            ObservedRequest begin = requests.Single(request => request.Api && request.BodyRefSha256 == null);
            ObservedRequest part = requests.Single(request => !request.Api);
            ObservedRequest next = requests.Single(request => request.Api && request.BodyRefSha256 != null);
            Assert.IsTrue(begin.Sequence == 1 && begin.Method == "POST" && begin.Path == BeginPath && begin.Status == 200, sent);
            Assert.IsTrue(part.Method == "PUT" && part.Path == "/upload/ref-7/1" && part.Origin == HeldStorage.Url && part.Status == 200, sent);
            Assert.IsTrue(next.Method == "POST" && next.Path == BeginPath && next.BodyRefSha256 == ScenarioDigest.Sha256("ref-7") && next.BodyPart == 2
                && next.Status == null && next.ClientFailure != null && next.TokenCancelled
                && !next.OwnerCancelledAtBegin && next.OwnerCancelledWithToken == true && next.TransportEndedBeforeToken == false && next.OwnerCancelledAtEnd, sent);
            Assert.IsTrue(next.BegunAt < part.EndedAt && part.EndedAt < gate.CancelEvent && gate.CancelEvent < next.TokenCancelledAt && next.TokenCancelledAt < next.EndedAt,
                $"part 2's negotiation began at event {next.BegunAt}, before part 1 was answered at {Shown.Of(part.EndedAt)}, and its token was cancelled at {Shown.Of(next.TokenCancelledAt)} and it ended at {Shown.Of(next.EndedAt)}, after the caller meant to cancel at {Shown.Of(gate.CancelEvent)}");

            var entries = new JsonArray
            {
                EntryOf(11, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
                EntryOf(12, "PUT", "/upload/ref-7/1", "transfers.upload_part", 200, upload: 7, part: 1),
            };
            if (journaled)
            {
                // The simulator read the negotiation whole and answered it after its client had gone.
                entries.Add(EntryOf(13, "POST", BeginPath, "files.begin_upload", 200, upload: 7));
            }
            (List<string> problems, UnansweredRequests unanswered, JournalAccount final) = AccountForUpload(Contract(3), requests, entries, gate, false, raised, bytes.Length);

            Assert.AreEqual(0, problems.Count, string.Join("; ", problems));
            CancelledAttempt attempt = unanswered.Attempts.Single();
            Assert.IsTrue(attempt.Sequence == next.Sequence && !attempt.Storage && attempt.Part == 2 && attempt.Confirmed && attempt.Upload == 7, "the negotiation is the upload's own attempt at part 2");
            JournalEntry entry = final.Joined.Where(pair => pair.Request == next).Select(pair => pair.Entry).SingleOrDefault();
            Assert.AreEqual(journaled ? 200 : (int?)null, entry?.Status, "the server's own outcome is kept apart from the client's");
            Assert.AreEqual(!journaled, final.Excused.Contains(next), "only a confirmed attempt without an entry is excused");
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow(true, "")]
        [DataRow(false, "the begin_upload that granted its part (request")]
        public async Task TheSdksGrantedPartThatReachesTheTransportOnlyAfterTheCancellationIsTheUploadsOwnAttempt(bool grantJournaled, string problem)
        {
            var recorder = new RequestRecorder(StubEnvironment.Url, StubEnvironment.Key);
            var network = new LateUpload();
            FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = StubEnvironment.Url, ApiKey = StubEnvironment.Key },
                HttpTransport.FromHandlerFactory(() => new LateEntry(network, new ObservingHandler(recorder, new Boundary(network.AnswerAsync)))));
            using var caller = new CancellationTokenSource();
            recorder.Owner = caller.Token;
            var gate = new CancellationGate();
            var progress = new SynchronousProgress(recorder, 0, gate, caller);
            byte[] bytes = { 1, 2, 3, 4, 5, 6 };
            Exception raised = null;
            bool renewedWhilePartOneHeld = false;
            try
            {
                Task upload = client.Transfers.UploadStreamAsync(new System.IO.MemoryStream(bytes, writable: false), bytes.Length, CancelledPath, new FilesCom.Transfers.UploadOptions { Progress = new HeldReport(progress, network) }, caller.Token);
                // Part 1's answer waits at the stub, with the upload's task in hand, until the SDK's dispatcher has
                // renewed part 2's expired URL, which it does only once it has returned from starting part 1
                // (UploadCoordinator.DispatchPartsAsync), so the report of part 1's acknowledgement never runs within
                // the dispatcher's own call, which sends part 2.
                await Task.WhenAny(network.PartTwoRenewed.Task, upload, Task.Delay(Bound));
                renewedWhilePartOneHeld = network.PartTwoRenewed.Task.IsCompleted;
                network.ReleasePartOne();
                await upload;
            }
            catch (Exception e)
            {
                raised = e;
            }
            finally
            {
                network.ReleasePartOne();
                await client.DisposeAsync();
            }
            IReadOnlyList<ObservedRequest> requests = recorder.All;
            string sent = string.Join("; ", requests.Select(request => $"{request.Sequence} {request.Method} {request.Path} {request.Outcome}"));

            // The SDK's own flow: the dispatcher, past starting part 1, renewed part 2's expired URL while part 1 was
            // held unanswered; then part 2 was sent, past FilesApi's own check of its token, while the report of
            // part 1's acknowledgement was on its way to the caller, and reached the transport's bottom only once
            // the caller had actually cancelled, its token already cancelled by the caller's.
            Assert.IsTrue(raised is OperationCanceledException, $"the upload ended with {(raised == null ? "nothing" : raised.GetType().Name)}, not as cancelled: {sent}");
            Assert.IsTrue(renewedWhilePartOneHeld, "the SDK's dispatcher renewed part 2's expired URL, having started part 1, before part 1 was answered: " + sent);
            Assert.IsTrue(network.ReportHeldUntilPartTwoSent, "the SDK sent part 2 before the report of the first acknowledged part reached the caller: " + sent);
            Assert.AreEqual(5, requests.Count, sent);
            ObservedRequest begin = requests.Single(request => request.Api && request.BodyRefSha256 == null);
            ObservedRequest[] grants = requests.Where(request => request.Api && request.BodyRefSha256 != null).ToArray();
            ObservedRequest part = requests.Single(request => !request.Api && request.Path == "/upload/ref-7/1");
            ObservedRequest late = requests.Single(request => !request.Api && request.Path == "/upload/ref-7/2");
            Assert.AreEqual(2, grants.Length, sent);
            // Part 2's first grant, of a URL already expired, then the dispatcher's renewal, which granted the part it sent.
            ObservedRequest renewal = grants[1];
            Assert.IsTrue(begin.Sequence == 1 && begin.Status == 200 && part.Status == 200 && part.Origin == HeldStorage.Url && part.EndedAt < gate.CancelEvent, sent);
            Assert.IsTrue(grants.All(grant => grant.Path == BeginPath && grant.Status == 200 && grant.BodyRefSha256 == ScenarioDigest.Sha256("ref-7") && grant.BodyPart == 2
                && grant.EndedAt < gate.CancelEvent && !grant.OwnerCancelledAtEnd), sent);
            Assert.IsTrue(grants[0].EndedAt < renewal.BegunAt && renewal.BegunAt < part.EndedAt,
                $"part 2's URL was renewed from event {renewal.BegunAt}, after its first grant was answered at {Shown.Of(grants[0].EndedAt)} and before part 1 was answered at {Shown.Of(part.EndedAt)}: {sent}");
            Assert.IsTrue(late.Method == "PUT" && late.Origin == HeldStorage.Url && late.Status == null && late.ClientFailure != null && late.BegunAt > gate.CancelEvent
                && late.OwnerCancelledAtBegin && late.TokenCancelledAtBegin && late.OwnerCancelledWithToken == true && late.TransportEndedBeforeToken == false && late.OwnerCancelledAtEnd,
                $"part 2 reached the transport's bottom at event {late.BegunAt}, after the caller meant to cancel at {Shown.Of(gate.CancelEvent)} and had cancelled, its own token cancelled by the caller's: {sent}");

            var entries = new JsonArray
            {
                EntryOf(11, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
                EntryOf(12, "PUT", "/upload/ref-7/1", "transfers.upload_part", 200, upload: 7, part: 1),
                // Part 2's first grant, whose URL the SDK renewed before sending the part.
                EntryOf(13, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
            };
            if (grantJournaled)
            {
                // The renewal, which granted the part the SDK sent.
                entries.Add(EntryOf(14, "POST", BeginPath, "files.begin_upload", 200, upload: 7));
            }
            (List<string> problems, UnansweredRequests unanswered, JournalAccount final) = AccountForUpload(Contract(3), requests, entries, gate, false, raised, bytes.Length);

            CancelledAttempt attempt = unanswered.Attempts.Single();
            Assert.IsTrue(attempt.Sequence == late.Sequence && attempt.Storage && attempt.Part == 2 && attempt.GrantedBy == renewal.Sequence && attempt.UploadPath == "/upload",
                "part 2 is the upload's attempt that its renewed grant, answered before the caller cancelled, shows was the SDK's to send");
            if (problem.Length == 0)
            {
                Assert.AreEqual(0, problems.Count, string.Join("; ", problems));
                Assert.IsTrue(attempt.Confirmed && attempt.Upload == 7 && final.Excused.Contains(late), "the late part is confirmed as the upload's own attempt and excused without an entry");
            }
            else
            {
                // Without the grant journaled as the upload's, ownership is not shown: the late part stays a failure.
                Assert.IsTrue(attempt.Rejected != null && !final.Excused.Contains(late), "a late part whose grant the journal does not show is not excused");
                Assert.IsTrue(problems.Any(found => found.Contains(problem, StringComparison.Ordinal)), $"expected a problem with \"{problem}\": {string.Join("; ", problems)}");
            }
        }

        // UploadOptions.Progress for TheSdksGrantedPartThatReachesTheTransportOnlyAfterTheCancellationIsTheUploadsOwnAttempt:
        // it passes every report on to PROGRESS, which cancels inside the report of the first acknowledged part, but holds
        // that report until the SDK has sent part 2 toward the transport (LateUpload.PartTwoSent), or Bound has passed.
        // That report follows part 1's answer, which storage holds until the SDK's dispatcher has returned from
        // starting part 1 (LateUpload.PartTwoRenewed), so holding it never holds the dispatcher that sends part 2.
        private sealed class HeldReport : IProgress<FilesCom.Transfers.TransferProgress>
        {
            private readonly IProgress<FilesCom.Transfers.TransferProgress> progress;
            private readonly LateUpload network;

            internal HeldReport(IProgress<FilesCom.Transfers.TransferProgress> progress, LateUpload network)
            {
                this.progress = progress;
                this.network = network;
            }

            public void Report(FilesCom.Transfers.TransferProgress value)
            {
                if (value.PartsAcknowledged >= 1 && !network.ReportHeld)
                {
                    network.ReportHeld = true;
                    network.ReportHeldUntilPartTwoSent = network.PartTwoSent.Task.Wait(Bound);
                }
                progress.Report(value);
            }
        }

        // The client's own transport above the observing handler, for the same control: it passes every request on at once
        // but the SDK's PUT of part 2, which it keeps, once it has signalled that the SDK sent it (LateUpload.PartTwoSent),
        // until the request's own token is cancelled, or Bound has passed. Part 2 thus reaches the observing handler only
        // after the caller has cancelled, as a part UploadCoordinator dispatched before then can.
        private sealed class LateEntry : DelegatingHandler
        {
            private readonly LateUpload network;

            internal LateEntry(LateUpload network, HttpMessageHandler inner) : base(inner)
            {
                this.network = network;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (request.Method == HttpMethod.Put && request.RequestUri.AbsolutePath == "/upload/ref-7/2")
                {
                    network.PartTwoSent.TrySetResult(true);
                    try
                    {
                        await Task.Delay(Bound, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // The caller has cancelled: part 2 goes on to the observing handler now.
                    }
                }
                return await base.SendAsync(request, cancellationToken);
            }
        }

        // The Files.com API and storage at the network boundary for the same control: the SDK's upload of six bytes in parts
        // of three, each part granted alone and at once, as the simulator grants one part unless more are asked for
        // (Files#begin_upload), with a fixed Content-Length. Part 2 is first granted a URL that has already expired,
        // which the SDK's dispatcher renews before admitting the part (UploadCoordinator.RenewIfExpiringAsync), so only
        // once it has returned from starting part 1; the renewal is granted a URL that has not expired
        // (PartTwoRenewed). Storage answers part 1 only once the control releases it (ReleasePartOne), and part 2,
        // which arrives only after the caller cancelled (LateEntry), ends as its own token is cancelled, or fails after
        // Bound. Anything else is answered 501.
        private sealed class LateUpload
        {
            private readonly TaskCompletionSource<bool> partOneAnswer = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private int partTwoGrants;

            // Set once the SDK's dispatcher has asked for part 2's expired URL to be renewed.
            internal TaskCompletionSource<bool> PartTwoRenewed { get; } = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Set once the SDK has sent part 2 toward the transport (LateEntry).
            internal TaskCompletionSource<bool> PartTwoSent { get; } = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Whether the report of the first acknowledged part was held (HeldReport), and passed on only once part 2 had
            // been sent.
            internal bool ReportHeld { get; set; }

            internal bool ReportHeldUntilPartTwoSent { get; set; }

            // Lets storage answer part 1.
            internal void ReleasePartOne()
            {
                partOneAnswer.TrySetResult(true);
            }

            internal async Task<HttpResponseMessage> AnswerAsync(HttpRequestMessage request, CancellationToken token)
            {
                string path = request.RequestUri.AbsolutePath;
                if (request.Method == HttpMethod.Post && path == BeginPath)
                {
                    JsonNode parameters = JsonNode.Parse(await request.Content.ReadAsStringAsync(token));
                    long number = parameters?["ref"] == null ? 1 : parameters["part"].GetValue<long>();
                    // Part 2's first URL has already expired; the one its renewal is granted has not.
                    bool expired = number == 2 && Interlocked.Increment(ref partTwoGrants) == 1;
                    if (number == 2 && !expired)
                    {
                        PartTwoRenewed.TrySetResult(true);
                    }
                    var grant = new JsonArray
                    {
                        new JsonObject
                        {
                            ["ref"] = "ref-7",
                            ["part_number"] = number,
                            ["partsize"] = 3,
                            ["next_partsize"] = 3,
                            ["upload_uri"] = HeldStorage.Url + "/upload/ref-7/" + number.ToString(CultureInfo.InvariantCulture),
                            ["http_method"] = "PUT",
                            ["headers"] = new JsonObject { ["Content-Length"] = "3" },
                            ["parallel_parts"] = false,
                            ["retry_parts"] = true,
                            ["expires"] = expired ? "2000-01-01T00:00:00Z" : "2999-01-01T00:00:00Z",
                            ["path"] = CancelledPath,
                        },
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request, Content = new StringContent(grant.ToJsonString(), Encoding.UTF8, "application/json") };
                }
                bool storage = request.Method == HttpMethod.Put && request.RequestUri.GetLeftPart(UriPartial.Authority) == HeldStorage.Url;
                if (storage && path == "/upload/ref-7/1")
                {
                    await request.Content.ReadAsByteArrayAsync(token);
                    // Answered only once the control releases it.
                    await partOneAnswer.Task.WaitAsync(token);
                    var accepted = new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request };
                    accepted.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"part-1\"");
                    return accepted;
                }
                if (storage && path == "/upload/ref-7/2")
                {
                    await Task.Delay(Bound, token);
                    throw new TimeoutException("the SDK's part 2 was never cancelled");
                }
                return new HttpResponseMessage(HttpStatusCode.NotImplemented) { RequestMessage = request };
            }
        }

        // The SDK's own upload of a local file of six bytes, in parts of three, cancelled at its first acknowledged part
        // while part 2's body, the file's own part content, is being written at the network boundary. The SDK writes every
        // part's body with the upload's own token, not the request's (UploadCoordinator, FilePartContent, WatchedContent):
        // the writes of part 1, acknowledged, and of part 2 show that very token before the caller cancels, and the
        // caller's actual cancellation, taken with whatever of part 2 had already failed before it reaches any token of the
        // SDK's (RequestRecorder.CancellingOwner), is what cancels it. When that token ends part 2's write at the boundary
        // (owner), and so its body and its transport's task, before the cancellation of the request's own token reaches the
        // observing handler, which holds that cancellation back as a callback the runtime runs late holds it
        // (ObservingHandler.HoldRelay), the handler finds the transport had already ended the part, and only the body's end,
        // with the cancellation of that very token, shows that the caller's cancellation ended it: the part is the upload's
        // own attempt. It stays a failure when the network ends the write on its own, with a token of its own, before the
        // caller cancels, the handler observing the transport's end only once the caller has (independent); when the body
        // sees that write's end only after the caller's cancellation has reached part 2's transport, still running
        // (delayed-independent); and when the network ends the write with a token of its own only after the caller has
        // cancelled, the relay held again (foreign-after-owner): every token was cancelled when it ended, as cancelled, and
        // its body's writing ended as cancelled too. Part 2's first grant has already expired, so the SDK's dispatcher
        // renews it, which it does only once it has returned from starting part 1, and storage answers part 1 only after
        // that renewal; the report of part 1's acknowledgement, which cancels, is held until part 2's body is waiting at the
        // boundary (owner, foreign-after-owner), until the handler holds its observation of the end the network gave part 2
        // (independent), or until the body's sight of the end the network gave part 2's write is held (delayed-independent).
        [TestMethod]
        [TestCategory(Category)]
        [DataRow("owner", "", "")]
        [DataRow("independent", "is not one of the cancelled upload's own attempts: its transport had already ended it, with TaskCanceledException, when the cancellation of its own token reached the transport",
            "and its body ended with TaskCanceledException of a token other than the upload's own")]
        [DataRow("delayed-independent", "is not one of the cancelled upload's own attempts: a write of its body had already failed, with TaskCanceledException, when the caller actually cancelled at event", "")]
        [DataRow("foreign-after-owner", "is not one of the cancelled upload's own attempts: its transport had already ended it, with TaskCanceledException, when the cancellation of its own token reached the transport",
            "and its body ended with TaskCanceledException of a token other than the upload's own")]
        public async Task TheSdksPartWhoseBodyTheCancellationEndsBeforeItsTokenIsTheUploadsOwnAttempt(string ending, string problem, string body)
        {
            bool owner = ending == "owner";
            // Where the cancellation of part 2's own token is recorded and relayed only once part 2 has ended (HoldRelay).
            bool relayHeld = owner || ending == "foreign-after-owner";
            var recorder = new RequestRecorder(StubEnvironment.Url, StubEnvironment.Key);
            var network = new BodyUpload(honorsWriteToken: owner);
            // Set once the handler holds its observation of the end the network gave part 2 (independent); opened once the
            // caller has actually cancelled.
            var transportEnded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var observation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Func<Task> holdObservation = ending switch
            {
                "owner" or "foreign-after-owner" => network.HoldObservation,
                "independent" => () =>
                {
                    transportEnded.TrySetResult(true);
                    return observation.Task;
                }
                ,
                _ => null,
            };
            FilesClient client = FilesClient.Create(new FilesConfiguration { BaseUrl = StubEnvironment.Url, ApiKey = StubEnvironment.Key },
                HttpTransport.FromHandlerFactory(() => new ObservingHandler(recorder, new Boundary(network.AnswerAsync))
                {
                    HoldRelay = relayHeld ? new Func<ObservedRequest, Task>(network.HoldRelay) : null,
                    HoldObservation = holdObservation,
                    HoldWrite = ending == "delayed-independent" ? new Func<Task>(network.HoldWrite) : null,
                }));
            using var caller = new CancellationTokenSource();
            recorder.Owner = caller.Token;
            var gate = new CancellationGate();
            Task release = ending switch
            {
                "independent" => transportEnded.Task,
                "delayed-independent" => network.PartTwoWriteHeld.Task,
                _ => network.PartTwoBodyWaiting.Task,
            };
            var held = new HeldAcknowledgement(new SynchronousProgress(recorder, 0, gate, caller), release);
            byte[] bytes = { 1, 2, 3, 4, 5, 6 };
            string folder = System.IO.Directory.CreateTempSubdirectory("files-api-scenarios-body-").FullName;
            string local = System.IO.Path.Combine(folder, "cancelled.bin");
            System.IO.File.WriteAllBytes(local, bytes);
            Exception raised = null;
            try
            {
                Task upload = client.Transfers.UploadFileAsync(local, CancelledPath, new FilesCom.Transfers.UploadOptions { Progress = held }, caller.Token);
                switch (ending)
                {
                    case "independent":
                        // Part 2's body waits at the boundary; the network ends its write on its own before the caller
                        // cancels, and the handler holds its observation of the end of part 2's transport; the report of
                        // part 1's acknowledgement, passed on only then, cancels; and the handler observes that end only once
                        // that is done.
                        await Task.WhenAny(network.PartTwoBodyWaiting.Task, upload, Task.Delay(Bound));
                        network.EndPartTwoBody();
                        await Task.WhenAny(held.Passed.Task, upload, Task.Delay(Bound));
                        observation.TrySetResult(true);
                        break;
                    case "delayed-independent":
                        // The network ends part 2's write on its own before the caller cancels, and the body's sight of that
                        // end is held, so that part 2's body and transport still run; the report, passed on only then,
                        // cancels, and its cancellation reaches part 2's transport while it runs; and the body sees its
                        // write's end only once that is done.
                        await Task.WhenAny(network.PartTwoBodyWaiting.Task, upload, Task.Delay(Bound));
                        network.EndPartTwoBody();
                        await Task.WhenAny(held.Passed.Task, upload, Task.Delay(Bound));
                        network.ReleaseWrite();
                        break;
                    case "foreign-after-owner":
                        // The report, passed on once part 2's body is waiting, cancels; only then does the network end part
                        // 2's write, with a token of its own.
                        await Task.WhenAny(held.Passed.Task, upload, Task.Delay(Bound));
                        network.EndPartTwoBody();
                        break;
                }
                await upload;
            }
            catch (Exception e)
            {
                raised = e;
            }
            finally
            {
                network.EndPartTwoBody();
                network.ReleaseWrite();
                observation.TrySetResult(true);
                await client.DisposeAsync();
                System.IO.Directory.Delete(folder, recursive: true);
            }
            IReadOnlyList<ObservedRequest> requests = recorder.All;
            string sent = string.Join("; ", requests.Select(request => $"{request.Sequence} {request.Method} {request.Path} {request.Outcome}"));

            // The SDK's own flow: the dispatcher, past starting part 1, renewed part 2's expired URL while part 1 was held
            // unanswered; part 2 was then sent, its body reaching the boundary before the report of part 1's
            // acknowledgement cancelled inside it.
            Assert.IsTrue(raised is OperationCanceledException, $"the upload ended with {(raised == null ? "nothing" : raised.GetType().Name)}, not as cancelled: {sent}");
            Assert.IsTrue(network.PartOneHeldUntilRenewal, "the SDK's dispatcher renewed part 2's expired URL, having started part 1, before part 1 was answered: " + sent);
            string releasedBy = ending switch
            {
                "independent" => "the handler held its observation of the end the network gave part 2",
                "delayed-independent" => "the body's sight of the end the network gave part 2's write was held",
                _ => "part 2's body was waiting at the boundary",
            };
            Assert.IsTrue(held.HeldUntilReleased, $"{releasedBy} before the report of the first acknowledged part reached the caller: {sent}");
            Assert.AreEqual(5, requests.Count, sent);
            ObservedRequest begin = requests.Single(request => request.Api && request.BodyRefSha256 == null);
            ObservedRequest[] grants = requests.Where(request => request.Api && request.BodyRefSha256 != null).ToArray();
            ObservedRequest part = requests.Single(request => !request.Api && request.Path == "/upload/ref-7/1");
            ObservedRequest pending = requests.Single(request => !request.Api && request.Path == "/upload/ref-7/2");
            Assert.AreEqual(2, grants.Length, sent);
            Assert.IsTrue(begin.Sequence == 1 && begin.Method == "POST" && begin.Path == BeginPath && begin.Origin == StubEnvironment.Url && begin.Status == 200
                && begin.CarriesApiKey && begin.ApiKeyMatches && !begin.CarriesAuthorization, sent);
            Assert.IsTrue(grants.All(grant => grant.Method == "POST" && grant.Path == BeginPath && grant.Status == 200 && grant.BodyRefSha256 == ScenarioDigest.Sha256("ref-7") && grant.BodyPart == 2
                && grant.CarriesApiKey && grant.ApiKeyMatches && !grant.CarriesAuthorization && grant.EndedAt < gate.CancelEvent && !grant.OwnerCancelledAtEnd), sent);
            Assert.IsTrue(part.Method == "PUT" && part.Origin == HeldStorage.Url && part.Status == 200 && !part.CarriesApiKey && !part.CarriesAuthorization && part.EndedAt < gate.CancelEvent, sent);
            // The first acknowledged part, exactly as the report that cancelled saw it.
            Assert.IsTrue(gate.Cancelled && gate.PartsAcknowledged == 1 && gate.AcknowledgedBytes == 3 && gate.AnsweredStorage.SequenceEqual(new[] { part.Sequence }),
                $"the caller cancelled inside the report of part 1's acknowledgement, with part 1 alone answered: {string.Join(",", gate.AnsweredStorage)}");
            Assert.IsTrue(pending.Method == "PUT" && pending.Origin == HeldStorage.Url && pending.Status == null && pending.ClientFailure == nameof(TaskCanceledException)
                && !pending.CarriesApiKey && !pending.CarriesAuthorization && pending.BegunAt < gate.CancelEvent && !pending.OwnerCancelledAtBegin && !pending.TokenCancelledAtBegin
                && pending.EndedAt > gate.CancelEvent && pending.OwnerCancelledAtEnd, $"part 2 reached the transport's bottom before the caller cancelled, and ended unanswered after it: {sent}");
            // The token the SDK's bodies of the upload wrote with, as the writes of part 1, acknowledged, and of part 2 showed
            // it before the caller actually cancelled, is the very token the SDK wrote part 2's body with at the boundary;
            // the caller's actual cancellation took its place after its intent and before the cancellation of part 2's own
            // token, and is what cancelled that token.
            CancellationToken? own = gate.BodyToken;
            Assert.IsTrue(own != null && network.PartTwoWrittenWith == own && gate.BodyTokenWitnesses.Count == 2 && gate.BodyTokenWitnesses.Contains(part.Sequence)
                && gate.BodyTokenWitnesses.Contains(pending.Sequence) && !gate.BodyTokenCancelledBeforeOwner && gate.BodyTokenCancelledByOwner == true
                && gate.CancelEvent < gate.OwnerCancelledAt && gate.OwnerCancelledAt < pending.TokenCancelledAt,
                $"the writes of requests {string.Join(",", gate.BodyTokenWitnesses)} showed the upload's own token, which the caller's actual cancellation at event {Shown.Of(gate.OwnerCancelledAt)} cancelled: {sent}");
            // What ended part 2's write at the boundary: the cancellation of that very token (owner), or of the network's own.
            Assert.AreEqual(owner ? own : network.NetworkToken, network.PartTwoWriteEndedWith, "the token whose cancellation ended part 2's write at the boundary");
            Assert.IsTrue(owner || network.PartTwoWriteEndedWith != own, "the network's own token is not the upload's");
            Assert.AreEqual(nameof(TaskCanceledException), pending.BodyEndedWith, "what part 2's body's writing ended with");
            string failedBeforeOwner = ending switch
            {
                "independent" => TrackedTask.Transport,
                "delayed-independent" => TrackedTask.Write,
                _ => null,
            };
            Assert.IsTrue(pending.FailedBeforeOwner == failedBeforeOwner && pending.FailedBeforeOwnerWith == (failedBeforeOwner == null ? null : nameof(TaskCanceledException)),
                $"what of part 2 had already failed when the caller actually cancelled: {Shown.Of(pending.FailedBeforeOwner)}, with {Shown.Of(pending.FailedBeforeOwnerWith)}");
            Assert.IsTrue(pending.TokenCancelled && pending.OwnerCancelledWithToken == true && pending.TransportEndedBeforeToken == (ending != "delayed-independent")
                && pending.EndedAt > pending.TokenCancelledAt,
                $"the cancellation of part 2's own token reached the handler at event {Shown.Of(pending.TokenCancelledAt)}, with the caller's cancelled, {(pending.TransportEndedBeforeToken == true ? "after" : "before")} the transport had ended part 2, which ended at {Shown.Of(pending.EndedAt)}");

            var entries = new JsonArray
            {
                EntryOf(11, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
                EntryOf(12, "PUT", "/upload/ref-7/1", "transfers.upload_part", 200, upload: 7, part: 1),
                // Part 2's first grant, whose URL the SDK renewed before sending the part, and the renewal.
                EntryOf(13, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
                EntryOf(14, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
            };
            (List<string> problems, UnansweredRequests unanswered, JournalAccount final) = AccountForUpload(Contract(3), requests, entries, gate, false, raised, bytes.Length);

            if (problem.Length == 0)
            {
                Assert.AreEqual(0, problems.Count, string.Join("; ", problems));
                CancelledAttempt attempt = unanswered.Attempts.Single();
                Assert.IsTrue(attempt.Sequence == pending.Sequence && attempt.Storage && attempt.Part == 2 && attempt.GrantedBy == null && attempt.Origin == HeldStorage.Url && attempt.UploadPath == "/upload"
                    && attempt.RefSha256 == ScenarioDigest.Sha256("ref-7") && attempt.Confirmed && attempt.Upload == 7,
                    "part 2 is the upload's own attempt, which its body's end, with the cancellation of the upload's own token by the caller's, shows the caller's cancellation ended");
                Assert.IsTrue(final.Excused.Contains(pending) && !final.Joined.Any(pair => pair.Request == pending), "the confirmed attempt, without a journal entry, is excused, its missing answer kept");
            }
            else
            {
                Assert.IsFalse(unanswered.Attempts.Any() || final.Excused.Contains(pending), "a part the caller's cancellation is not shown to have ended is no attempt of the upload's, and is not excused");
                Assert.IsTrue(problems.Any(found => found.Contains(problem, StringComparison.Ordinal) && found.Contains(body, StringComparison.Ordinal)),
                    $"expected a problem with \"{problem}\"{(body.Length == 0 ? "" : $" and \"{body}\"")}: {string.Join("; ", problems)}");
            }
        }

        // UploadOptions.Progress for TheSdksPartWhoseBodyTheCancellationEndsBeforeItsTokenIsTheUploadsOwnAttempt: it passes
        // every report on to PROGRESS, which cancels inside the report of the first acknowledged part, but holds that report
        // until RELEASE has ended, or Bound has passed. That report follows part 1's answer, which storage holds until the
        // SDK's dispatcher has returned from starting part 1 (BodyUpload), so holding it never holds the dispatcher that
        // sends part 2.
        private sealed class HeldAcknowledgement : IProgress<FilesCom.Transfers.TransferProgress>
        {
            private readonly IProgress<FilesCom.Transfers.TransferProgress> progress;
            private readonly Task release;
            private bool held;

            internal HeldAcknowledgement(IProgress<FilesCom.Transfers.TransferProgress> progress, Task release)
            {
                this.progress = progress;
                this.release = release;
            }

            // Whether the report of the first acknowledged part was held until RELEASE had ended.
            internal bool HeldUntilReleased { get; private set; }

            // Set once that report has been passed on, with the caller's cancellation made inside it.
            internal TaskCompletionSource<bool> Passed { get; } = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            public void Report(FilesCom.Transfers.TransferProgress value)
            {
                bool first = value.PartsAcknowledged >= 1 && !held;
                if (first)
                {
                    held = true;
                    HeldUntilReleased = release.Wait(Bound);
                }
                progress.Report(value);
                if (first)
                {
                    Passed.TrySetResult(true);
                }
            }
        }

        // The Files.com API and storage at the network boundary for the same control: the SDK's upload of six bytes in parts
        // of three, each part granted alone and at once, as the simulator grants one part unless more are asked for
        // (Files#begin_upload), with a fixed Content-Length. Part 2 is first granted a URL that has already expired, which
        // the SDK's dispatcher renews before admitting the part (UploadCoordinator.RenewIfExpiringAsync), so only once it
        // has returned from starting part 1; storage reads part 1 and answers it only after that renewal, or Bound. Storage
        // reads part 2's body through WaitingWrites: its first write waits until the token it is written with is cancelled
        // (HONORSWRITETOKEN), or else until the network's own token is (EndPartTwoBody), or Bound has passed, and then fails
        // as cancelled by that token, and part 2's request with it. Anything else is answered 501.
        private sealed class BodyUpload
        {
            private readonly bool honorsWriteToken;
            private readonly CancellationTokenSource networkEnds = new CancellationTokenSource();
            private readonly TaskCompletionSource<bool> partTwoRenewed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<bool> partTwoEnded = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<bool> relayHeld = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private readonly TaskCompletionSource<bool> writeRelease = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            private int partTwoGrants;

            internal BodyUpload(bool honorsWriteToken)
            {
                this.honorsWriteToken = honorsWriteToken;
            }

            // Set once part 2's body is waiting at the boundary, its first write begun.
            internal TaskCompletionSource<bool> PartTwoBodyWaiting { get; } = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Set once the body's sight of a failed write of part 2's body is held (HoldWrite).
            internal TaskCompletionSource<bool> PartTwoWriteHeld { get; } = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Whether storage answered part 1 only once part 2's expired URL had been renewed.
            internal bool PartOneHeldUntilRenewal { get; private set; }

            // The token the SDK wrote part 2's body with at the boundary, and the token whose cancellation ended that write;
            // null until then.
            internal CancellationToken? PartTwoWrittenWith { get; private set; }

            internal CancellationToken? PartTwoWriteEndedWith { get; private set; }

            // The network's own token, which ends part 2's write when the network does so on its own (EndPartTwoBody).
            internal CancellationToken NetworkToken => networkEnds.Token;

            // Ends the write of part 2's body on the network's own, as a connection that ends its writes as cancelled.
            internal void EndPartTwoBody()
            {
                networkEnds.Cancel();
            }

            // Lets the body see a failed write of part 2's body that HoldWrite holds.
            internal void ReleaseWrite()
            {
                writeRelease.TrySetResult(true);
            }

            // ObservingHandler.HoldRelay: the cancellation of part 2's own token is recorded and relayed only once part 2's
            // request has ended at the boundary, or Bound has passed; any other request's at once.
            internal Task HoldRelay(ObservedRequest request)
            {
                if (request.Path != "/upload/ref-7/2")
                {
                    return null;
                }
                relayHeld.TrySetResult(true);
                return Task.WhenAny(partTwoEnded.Task, Task.Delay(Bound));
            }

            // ObservingHandler.HoldObservation: part 2's end is observed only once the cancellation of its own token has
            // reached the handler, and been held back, or Bound has passed.
            internal Task HoldObservation()
            {
                return Task.WhenAny(relayHeld.Task, Task.Delay(Bound));
            }

            // ObservingHandler.HoldWrite: the body sees a failed write of part 2's body only once the control lets it
            // (ReleaseWrite), or Bound has passed.
            internal Task HoldWrite()
            {
                PartTwoWriteHeld.TrySetResult(true);
                return Task.WhenAny(writeRelease.Task, Task.Delay(Bound));
            }

            internal Task<HttpResponseMessage> AnswerAsync(HttpRequestMessage request, CancellationToken token)
            {
                if (request.Method == HttpMethod.Put && request.RequestUri.GetLeftPart(UriPartial.Authority) == HeldStorage.Url && request.RequestUri.AbsolutePath == "/upload/ref-7/2")
                {
                    Task<HttpResponseMessage> sending = PartTwoAsync(request, token);
                    sending.ContinueWith(_ => partTwoEnded.TrySetResult(true), CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
                    return sending;
                }
                return AnswerOtherAsync(request, token);
            }

            // A write of part 2's body, made with the token WRITING the body writes with, as WaitingWrites says.
            internal async Task WaitAsync(CancellationToken writing)
            {
                PartTwoWrittenWith = writing;
                PartTwoBodyWaiting.TrySetResult(true);
                try
                {
                    await Task.Delay(Bound, honorsWriteToken ? writing : networkEnds.Token);
                }
                catch (OperationCanceledException e)
                {
                    PartTwoWriteEndedWith = e.CancellationToken;
                    throw;
                }
                throw new TimeoutException("the SDK's part 2 body was never cancelled");
            }

            private async Task<HttpResponseMessage> PartTwoAsync(HttpRequestMessage request, CancellationToken token)
            {
                await request.Content.CopyToAsync(new WaitingWrites(this), token);
                throw new TimeoutException("the SDK's part 2 body was written whole");
            }

            private async Task<HttpResponseMessage> AnswerOtherAsync(HttpRequestMessage request, CancellationToken token)
            {
                string path = request.RequestUri.AbsolutePath;
                if (request.Method == HttpMethod.Post && path == BeginPath)
                {
                    JsonNode parameters = JsonNode.Parse(await request.Content.ReadAsStringAsync(token));
                    long number = parameters?["ref"] == null ? 1 : parameters["part"].GetValue<long>();
                    // Part 2's first URL has already expired; the one its renewal is granted has not.
                    bool expired = number == 2 && Interlocked.Increment(ref partTwoGrants) == 1;
                    if (number == 2 && !expired)
                    {
                        partTwoRenewed.TrySetResult(true);
                    }
                    var grant = new JsonArray
                    {
                        new JsonObject
                        {
                            ["ref"] = "ref-7",
                            ["part_number"] = number,
                            ["partsize"] = 3,
                            ["next_partsize"] = 3,
                            ["upload_uri"] = HeldStorage.Url + "/upload/ref-7/" + number.ToString(CultureInfo.InvariantCulture),
                            ["http_method"] = "PUT",
                            ["headers"] = new JsonObject { ["Content-Length"] = "3" },
                            ["parallel_parts"] = false,
                            ["retry_parts"] = true,
                            ["expires"] = expired ? "2000-01-01T00:00:00Z" : "2999-01-01T00:00:00Z",
                            ["path"] = CancelledPath,
                        },
                    };
                    return new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request, Content = new StringContent(grant.ToJsonString(), Encoding.UTF8, "application/json") };
                }
                if (request.Method == HttpMethod.Put && request.RequestUri.GetLeftPart(UriPartial.Authority) == HeldStorage.Url && path == "/upload/ref-7/1")
                {
                    await request.Content.ReadAsByteArrayAsync(token);
                    // Answered only once the SDK's dispatcher has renewed part 2's expired URL.
                    PartOneHeldUntilRenewal = await Task.WhenAny(partTwoRenewed.Task, Task.Delay(Bound, token)) == partTwoRenewed.Task;
                    var accepted = new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request };
                    accepted.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"part-1\"");
                    return accepted;
                }
                return new HttpResponseMessage(HttpStatusCode.NotImplemented) { RequestMessage = request };
            }
        }

        // The stream storage reads part 2's body through for the same control: each write waits (BodyUpload.WaitAsync),
        // and nothing is kept.
        private sealed class WaitingWrites : System.IO.Stream
        {
            private readonly BodyUpload network;

            internal WaitingWrites(BodyUpload network)
            {
                this.network = network;
            }

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                return network.WaitAsync(cancellationToken);
            }

            public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            {
                return new ValueTask(network.WaitAsync(cancellationToken));
            }

            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, System.IO.SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow("two-parts", "")]
        [DataRow("same-part-renewed-url", "1 distinct parts of upload 7 were stored ([1]), fewer than 2")]
        [DataRow("foreign-part", "request 3 stored part 2 of upload 8, not a part of this step's upload 7")]
        [DataRow("foreign-completion", "request 4 completed upload 8, not this step's upload 7")]
        public async Task ACompletedUploadCountsItsOwnDistinctStoredParts(string change, string problem)
        {
            var recorder = new RequestRecorder(StubEnvironment.Url, StubEnvironment.Key);
            using var invoker = new HttpMessageInvoker(new ObservingHandler(recorder, new Boundary((request, token) => AnswerAtBoundaryAsync(request, token, CancellationToken.None))));
            // Part 1, then part 2, or part 1 again on a renewed URL: the same path, signed again.
            string second = change == "same-part-renewed-url" ? "/upload/ref-7/1?X-Files-Signature=renewed" : "/upload/ref-7/2?X-Files-Signature=second";
            await Settled(invoker.SendAsync(ApiRequest(BeginPath, null, "answer"), CancellationToken.None));
            await Settled(invoker.SendAsync(StorageRequest(HttpMethod.Put, HeldStorage.Url + "/upload/ref-7/1?X-Files-Signature=first", "answer"), CancellationToken.None));
            await Settled(invoker.SendAsync(StorageRequest(HttpMethod.Put, HeldStorage.Url + second, "answer"), CancellationToken.None));
            await Settled(invoker.SendAsync(ApiRequest(CompletionPath, "{\"action\":\"end\",\"ref\":\"ref-7\"}", "answer-201"), CancellationToken.None));
            IReadOnlyList<ObservedRequest> requests = recorder.All;
            var entries = new JsonArray
            {
                EntryOf(11, "POST", BeginPath, "files.begin_upload", 200, upload: 7),
                EntryOf(12, "PUT", "/upload/ref-7/1", "transfers.upload_part", 200, upload: 7, part: 1),
                EntryOf(13, "PUT", requests[2].Path, "transfers.upload_part", 200, upload: change == "foreign-part" ? 8 : 7, part: change == "same-part-renewed-url" ? 1 : 2),
                EntryOf(14, "POST", CompletionPath, "files.finalize_upload", 201, upload: change == "foreign-completion" ? 8 : 7),
            };
            JournalContract contract = Contract();

            // As UploadAsync accounts for an upload that completed, with min_parts 2.
            UnansweredRequests unanswered = UploadEvidence.Settle(requests, contract, null, null, BeginRoute, CompletionRoute, CancelledPath, 6291461);
            JournalAccount account = JournalJoin.Join(StubEnvironment.Journal(entries), 10, contract, requests, final: true);
            UploadJournal upload = UploadEvidence.Read(requests, account, contract.Operations, BeginRoute, CompletionRoute, CancelledPath);
            List<string> problems = account.Problems.Concat(unanswered.Problems).Concat(UploadEvidence.Completed(upload, requests, CompletionRoute, CancelledPath, 2)).ToList();

            Assert.AreNotEqual(requests[1].UriSha256, requests[2].UriSha256, "the two storage requests went to URLs of their own");
            if (problem.Length == 0)
            {
                Assert.AreEqual(0, problems.Count, string.Join("; ", problems));
                Assert.AreEqual("1,2", string.Join(",", upload.StoredParts));
            }
            else
            {
                Assert.IsTrue(problems.Any(found => found.Contains(problem, StringComparison.Ordinal)), $"expected a problem with \"{problem}\": {string.Join("; ", problems)}");
            }
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow("exact", "")]
        [DataRow("wrong-empty", "the download reported the empty folders")]
        [DataRow("filled-empty", "is not an empty local directory")]
        [DataRow("extra-directory", "the local tree holds the folders")]
        [DataRow("changed-bytes", "not recipe")]
        public void ADownloadedTreeIsExactlyTheDefinitionsFolder(string change, string problem)
        {
            (ScenarioCase scenarioCase, DownloadDirectoryStep step) = ApiScenarioCases.All()
                .SelectMany(candidate => candidate.Steps.OfType<DownloadDirectoryStep>().Select(found => (candidate, found))).First();
            Func<string, ScenarioFixture> fixture = name => scenarioCase.Fixtures.Single(candidate => candidate.Name == name);
            // The tree the generated step says the download must make, made for real, then changed as the row says.
            string root = System.IO.Directory.CreateTempSubdirectory("files-api-scenarios-tree-").FullName;
            try
            {
                foreach (string directory in step.Directories)
                {
                    System.IO.Directory.CreateDirectory(ScenarioRun.Local(root, directory));
                }
                foreach (string file in step.Files)
                {
                    TreeFile matched = step.BytesMatch.FirstOrDefault(candidate => candidate.RelativePath == file);
                    System.IO.File.WriteAllBytes(ScenarioRun.Local(root, file), matched == null ? new byte[] { 1 } : ScenarioRun.RecipeBytes(fixture(matched.Fixture)));
                }
                IEnumerable<string> reported = step.EmptyDirectories;
                switch (change)
                {
                    case "wrong-empty":
                        // A folder that holds files, reported as the empty one.
                        reported = step.Directories.Except(step.EmptyDirectories).Take(1);
                        break;
                    case "filled-empty":
                        System.IO.File.WriteAllBytes(ScenarioRun.Local(root, step.EmptyDirectories[0] + "/stray.bin"), new byte[] { 1 });
                        break;
                    case "extra-directory":
                        System.IO.Directory.CreateDirectory(ScenarioRun.Local(root, "extra"));
                        break;
                    case "changed-bytes":
                        string changed = ScenarioRun.Local(root, step.BytesMatch[0].RelativePath);
                        System.IO.File.WriteAllBytes(changed, System.IO.File.ReadAllBytes(changed).Append((byte)0).ToArray());
                        break;
                }

                List<string> problems = TreeEvidence.Downloaded(root, step, reported, fixture);

                if (problem.Length == 0)
                {
                    Assert.AreEqual(0, problems.Count, string.Join("; ", problems));
                }
                else
                {
                    Assert.IsTrue(problems.Any(found => found.Contains(problem, StringComparison.Ordinal)), $"expected a problem with \"{problem}\": {string.Join("; ", problems)}");
                }
            }
            finally
            {
                System.IO.Directory.Delete(root, recursive: true);
            }
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow("absent", "the live profile's upload partsize is null, not one positive partsize for every part")]
        [DataRow("changed", "the live profile's upload partsize is 5242880, changed by its partsize_changes, not one positive partsize for every part")]
        public void BindingRefusesALiveProfileWithoutOnePartsizeForEveryPart(string change, string problem)
        {
            ScenarioCase scenarioCase = ApiScenarioCases.All()[0];
            BindingInputs inputs = StubEnvironment.Inputs(scenarioCase);
            JsonObject upload = inputs.Ready["profile"]["upload"].AsObject();
            if (change == "absent")
            {
                upload.Remove("partsize");
            }
            else
            {
                upload["partsize_changes"] = new JsonArray { new JsonObject { ["from_part"] = 2, ["partsize"] = 1048576 } };
            }

            List<string> problems = ScenarioBinding.Problems(inputs, scenarioCase);

            Assert.IsTrue(problems.Contains(problem), $"expected the problem \"{problem}\": {string.Join("; ", problems)}");
            Assert.IsNull(ScenarioBinding.UploadPartSize(inputs), "no partsize bounds the case's uploads");
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow("refused")]
        [DataRow("another-path")]
        [DataRow("a-file")]
        public async Task OnlyTheExactNewDirectoryMakesTheOwnedFolderTheCases(string answer)
        {
            ScenarioCase scenarioCase = ApiScenarioCases.All()[0];
            var api = new StubFilesApi(answer);

            ScenarioOutcome outcome = await ScenarioRun.RunAsync(scenarioCase, new StubEnvironment(scenarioCase, api));

            int create = scenarioCase.Steps.OfType<OperationStep>().Single(step => step.CreatesNamespace).Number;
            Assert.IsFalse(outcome.Passed, outcome.Summary());
            Assert.AreEqual((int?)create, outcome.FirstFailurePosition, outcome.Summary());
            Assert.IsFalse(outcome.NamespaceOwned, "the owned folder never became the case's");
            Assert.AreEqual(ScenarioOutcome.NothingOwned, outcome.CleanupOutcome);
            Assert.IsFalse(api.Requests.Any(request => request.Method == "DELETE"), "nothing was deleted");
            Assert.IsTrue(outcome.FinalJoinPassed, "every request the SDK sent is in the stub's journal once: " + string.Join("; ", outcome.Journal?.Problems ?? new List<string>()));
            Assert.IsTrue(outcome.Positions.Where(position => position.Number > create).All(position => position.Outcome == "unreached" || position.Outcome == "excluded"), "nothing ran after the create");
        }

        [TestMethod]
        [TestCategory(Category)]
        [DataRow("wrong-path", "not GET /file_actions/metadata/{path} with path")]
        [DataRow("wrong-operation", "not GET /file_actions/metadata/{path} with path")]
        [DataRow("unmapped-404", "received HTTP 404 not-found, as expected, but the SDK raised InvalidResponseException")]
        public async Task AnAbsenceCountsOnlyFromTheStepsOwnLookupAndTheSdksOwnError(string change, string problem)
        {
            ScenarioCase original = ApiScenarioCases.All()[0];
            ScenarioCase scenarioCase = change switch
            {
                "wrong-path" => WithLookup(original, async (run, token) =>
                {
                    await run.Client.RemoteFiles.FindAsync(run.PathOf("elsewhere"), null, null, token);
                    return StepResult.None;
                }),
                "wrong-operation" => WithLookup(original, async (run, token) =>
                {
                    await run.Client.Folders.ListFor(run.PathOf("folder"), null, null).LoadNextPageAsync(token);
                    return StepResult.None;
                }),
                _ => original,
            };
            var api = new StubFilesApi(change == "unmapped-404" ? "unmapped-404" : "exact");

            ScenarioOutcome outcome = await ScenarioRun.RunAsync(scenarioCase, new StubEnvironment(scenarioCase, api));

            Assert.IsFalse(outcome.Passed, outcome.Summary());
            Assert.AreEqual((int?)Lookup, outcome.FirstFailurePosition, outcome.Summary());
            Assert.IsTrue(outcome.FirstFailure.Contains(problem, StringComparison.Ordinal), outcome.Summary());
            Assert.IsFalse(outcome.NamespaceOwned, "the owned folder was never created");
            Assert.AreEqual(ScenarioOutcome.NothingOwned, outcome.CleanupOutcome);
            Assert.IsFalse(api.Requests.Any(request => request.Method == "DELETE"), "nothing was deleted");
            Assert.IsTrue(outcome.FinalJoinPassed, "every request the SDK sent is in the stub's journal once: " + string.Join("; ", outcome.Journal?.Problems ?? new List<string>()));
        }

        [TestMethod]
        [TestCategory(Category)]
        public async Task CleanupKeepsAnAbsenceTheSdkMisreportedApartFromThatFailure()
        {
            ScenarioCase scenarioCase = ApiScenarioCases.All()[0];
            var api = new StubFilesApi("unmapped-absence");

            ScenarioOutcome outcome = await ScenarioRun.RunAsync(scenarioCase, new StubEnvironment(scenarioCase, api));

            int create = scenarioCase.Steps.OfType<OperationStep>().Single(step => step.CreatesNamespace).Number;
            Assert.IsFalse(outcome.Passed, outcome.Summary());
            Assert.IsTrue(outcome.NamespaceOwned && outcome.OwnedAt == create, outcome.Summary());
            Assert.IsTrue(outcome.FirstFailurePosition > create, "the case stopped after the folder became its own: " + outcome.Summary());
            Assert.IsNull(outcome.UnresolvedOwnership, outcome.Summary());
            Assert.IsTrue(outcome.CleanupOutcome.Contains("confirmed absent", StringComparison.Ordinal) && outcome.CleanupOutcome.Contains("InvalidResponseException", StringComparison.Ordinal), outcome.CleanupOutcome);
            Assert.AreEqual(1, api.Requests.Count(request => request.Method == "DELETE"), "the owned folder alone was deleted, once");
            Assert.IsTrue(outcome.FinalJoinPassed, "every request the SDK sent is in the stub's journal once: " + string.Join("; ", outcome.Journal?.Problems ?? new List<string>()));
        }

        // How long a control's held request waits at most, so that no control hangs.
        private static readonly TimeSpan Bound = TimeSpan.FromSeconds(20);

        // How a control's request is answered at the boundary (AnswerAtBoundaryAsync).
        private static readonly HttpRequestOptionsKey<string> Behavior = new HttpRequestOptionsKey<string>("api-scenario-control-boundary");

        // The first scenario with its lookup of its owned folder before creating it calling CALL instead, and the fixture
        // "elsewhere" that call may name.
        private static ScenarioCase WithLookup(ScenarioCase original, Func<ScenarioRun, CancellationToken, Task<StepResult>> call)
        {
            ScenarioStep[] steps = original.Steps.Select(step => step is OperationStep operation && operation.Number == Lookup
                ? new OperationStep(operation.Number, operation.Label, operation.Backend, operation.Route, operation.PathValues, operation.Target, operation.CreatesNamespace, operation.Expect, call)
                : step).ToArray();
            ScenarioFixture[] fixtures = original.Fixtures.Append(new ScenarioFixture("elsewhere", "directory", "elsewhere", null)).ToArray();
            return new ScenarioCase(original.Id, original.Test, original.DefinitionSha256, fixtures, steps, original.DeleteRoute, original.DeleteNamespaceRecursively, original.LookupRoute, original.LookUpNamespace);
        }

        // The journal contract of the controls' simulator, with the partsize its live readiness grants (ScenarioBinding).
        private static JournalContract Contract()
        {
            return Contract(ScenarioBinding.UploadPartSize(new BindingInputs { Ready = StubEnvironment.Readiness() }));
        }

        // The journal contract of the controls' simulator, with an upload's parts granted PARTSIZE bytes.
        private static JournalContract Contract(long? partsize)
        {
            return new JournalContract(1, StubEnvironment.Instance, ApiScenarioCases.DefinitionSha256, SimulatorOperations.From(StubEnvironment.Readiness(), new List<string>()), partsize);
        }

        // An upload's REQUESTS accounted for as UploadAsync accounts for them once the upload has ended (COMPLETED, or with
        // RAISED): settled, joined to a journal of ENTRIES, read as the step's upload, its attempts confirmed, held to what a
        // cancelled upload of LENGTH bytes must show, and joined again as the final join joins them.
        private static (List<string> Problems, UnansweredRequests Unanswered, JournalAccount Final) AccountForUpload(JournalContract contract, IReadOnlyList<ObservedRequest> requests, JsonArray entries,
            CancellationGate gate, bool completed, Exception raised, long length)
        {
            UnansweredRequests unanswered = UploadEvidence.Settle(requests, contract, gate, raised, BeginRoute, CompletionRoute, CancelledPath, length);
            JournalAccount account = JournalJoin.Join(StubEnvironment.Journal(entries.DeepClone().AsArray()), 10, contract, requests, final: false);
            UploadJournal upload = UploadEvidence.Read(requests, account, contract.Operations, BeginRoute, CompletionRoute, CancelledPath);
            var problems = new List<string>(account.Problems);
            problems.AddRange(unanswered.Problems);
            problems.AddRange(UploadEvidence.ConfirmCancelledAttempts(unanswered.Attempts, requests, account, upload, contract.Operations));
            problems.AddRange(UploadEvidence.Cancelled(completed, raised, gate, upload, requests, CompletionRoute, CancelledPath, length));
            JournalAccount final = JournalJoin.Join(StubEnvironment.Journal(entries.DeepClone().AsArray()), 10, contract, requests, final: true);
            problems.AddRange(final.Problems);
            return (problems, unanswered, final);
        }

        // A journal entry as the simulator writes one (simulation.rb#step), with no fault: an API entry with the client's
        // API key, a transfer's with the upload and part it used.
        private static JsonObject EntryOf(long seq, string method, string path, string operation, int status, long? upload = null, long? part = null)
        {
            var entry = new JsonObject
            {
                ["seq"] = seq,
                ["epoch"] = 1,
                ["method"] = method,
                ["path"] = path,
                ["operation"] = operation,
                ["id"] = null,
                ["fault_id"] = null,
                ["status"] = status,
                ["user_agent"] = "Files.com DOTNET SDK v2.0.0",
            };
            if (path.StartsWith(ApiRoutes.ApiPrefix + "/", StringComparison.Ordinal))
            {
                entry["credentials"] = new JsonObject { ["api_key"] = 1 };
            }
            if (upload != null)
            {
                entry["upload"] = upload;
            }
            if (part != null)
            {
                entry["part"] = part;
            }
            return entry;
        }

        // An API request as the SDK sends one: a POST to PATH with the client's API key and, if any, its JSON BODY.
        private static HttpRequestMessage ApiRequest(string path, string body, string behavior)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, StubEnvironment.Url + path);
            request.Headers.Add("X-FilesAPI-Key", StubEnvironment.Key);
            if (body != null)
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            request.Options.Set(Behavior, behavior);
            return request;
        }

        // A storage request with a part's bytes and no credential.
        private static HttpRequestMessage StorageRequest(HttpMethod method, string url, string behavior)
        {
            var request = new HttpRequestMessage(method, url) { Content = new ByteArrayContent(new byte[] { 1, 2, 3 }) };
            request.Options.Set(Behavior, behavior);
            return request;
        }

        // What SENDING ended with: null once it was answered, or the exception it failed with.
        private static async Task<Exception> Settled(Task<HttpResponseMessage> sending)
        {
            try
            {
                (await sending).Dispose();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        // A control's request at the boundary, as the behavior it was sent with says: answered 200, or "answer-201" or
        // "answer-500"; "hold", until its own token is cancelled; "hold-then-transport-failure", until then, and then failed
        // as a connection that ended under the cancelled token; "fail", at once; "fail-once-the-caller-cancels", as a
        // connection that fails on its own when CALLER is cancelled, its own token never cancelled. Nothing is held past
        // Bound.
        private static async Task<HttpResponseMessage> AnswerAtBoundaryAsync(HttpRequestMessage request, CancellationToken token, CancellationToken caller)
        {
            request.Options.TryGetValue(Behavior, out string behavior);
            switch (behavior)
            {
                case "hold":
                    await Task.Delay(Bound, token);
                    throw new TimeoutException("the held request was never cancelled");
                case "hold-then-transport-failure":
                    try
                    {
                        await Task.Delay(Bound, token);
                    }
                    catch (OperationCanceledException)
                    {
                        throw new HttpRequestException("stand-in: the connection ended as its request was cancelled");
                    }
                    throw new TimeoutException("the held request was never cancelled");
                case "fail":
                    throw new HttpRequestException("stand-in: the connection failed");
                case "fail-once-the-caller-cancels":
                    try
                    {
                        await Task.Delay(Bound, caller);
                    }
                    catch (OperationCanceledException)
                    {
                        throw new HttpRequestException("stand-in: the connection failed on its own");
                    }
                    throw new TimeoutException("the caller never cancelled");
                case "answer-201":
                    return new HttpResponseMessage(HttpStatusCode.Created) { RequestMessage = request };
                case "answer-500":
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError) { RequestMessage = request };
                default:
                    return new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request };
            }
        }

        // The network at the boundary of a control's own transport: each request answered as ANSWER answers it.
        private sealed class Boundary : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> answer;

            internal Boundary(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> answer)
            {
                this.answer = answer;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return answer(request, cancellationToken);
            }
        }

        // The Files.com API and storage at the network boundary for the SDK's own upload of six bytes in parts of three:
        // the first begin_upload is granted part 1 alone, as the simulator grants one part unless more are asked for
        // (Files#begin_upload), with a fixed Content-Length; storage holds part 1 until the SDK's begin_upload of part 2
        // has arrived, or Bound has passed, and then accepts it; and that begin_upload is never answered: it ends when its
        // own token is cancelled, or fails after Bound. Anything else is answered 501.
        private sealed class OverlapUpload
        {
            private readonly TaskCompletionSource<bool> nextNegotiation = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Whether part 1 was still unanswered when part 2's negotiation arrived.
            internal bool PartHeldUntilNextNegotiation { get; private set; }

            internal async Task<HttpResponseMessage> AnswerAsync(HttpRequestMessage request, CancellationToken token)
            {
                string path = request.RequestUri.AbsolutePath;
                if (request.Method == HttpMethod.Post && path == BeginPath)
                {
                    JsonNode parameters = JsonNode.Parse(await request.Content.ReadAsStringAsync(token));
                    if (parameters?["ref"] == null)
                    {
                        var grant = new JsonArray
                        {
                            new JsonObject
                            {
                                ["ref"] = "ref-7",
                                ["part_number"] = 1,
                                ["partsize"] = 3,
                                ["next_partsize"] = 3,
                                ["upload_uri"] = HeldStorage.Url + "/upload/ref-7/1",
                                ["http_method"] = "PUT",
                                ["headers"] = new JsonObject { ["Content-Length"] = "3" },
                                ["parallel_parts"] = false,
                                ["retry_parts"] = true,
                                ["expires"] = "2999-01-01T00:00:00Z",
                                ["path"] = CancelledPath,
                            },
                        };
                        return new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request, Content = new StringContent(grant.ToJsonString(), Encoding.UTF8, "application/json") };
                    }
                    nextNegotiation.TrySetResult(true);
                    await Task.Delay(Bound, token);
                    throw new TimeoutException("the SDK's begin_upload of part 2 was never cancelled");
                }
                if (request.Method == HttpMethod.Put && request.RequestUri.GetLeftPart(UriPartial.Authority) == HeldStorage.Url && path == "/upload/ref-7/1")
                {
                    await request.Content.ReadAsByteArrayAsync(token);
                    PartHeldUntilNextNegotiation = await Task.WhenAny(nextNegotiation.Task, Task.Delay(Bound, token)) == nextNegotiation.Task;
                    var accepted = new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request };
                    accepted.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"part-1\"");
                    return accepted;
                }
                return new HttpResponseMessage(HttpStatusCode.NotImplemented) { RequestMessage = request };
            }
        }
    }

    // The Files.com API at the network boundary, for the controls' real runs of the first scenario: it answers what that
    // scenario's start sends, journals each request as the simulator does (simulation.rb#step), with the operation it
    // routes it to, and answers what it does not route 501, as the simulator does. Its mode chooses the answer to the owned
    // folder's create ("refused", "another-path", "a-file", or else exactly that folder, which it then holds until it is
    // deleted), and gives a 404 not-found whose body the SDK cannot read as an error to every lookup of a missing path
    // ("unmapped-404") or to the lookup of the folder it deleted ("unmapped-absence").
    internal sealed class StubFilesApi
    {
        private readonly object gate = new object();
        private readonly string mode;
        private readonly JsonArray journal = new JsonArray();
        private readonly List<(string Method, string Path)> requests = new List<(string Method, string Path)>();
        private string created;
        private string deleted;

        internal StubFilesApi(string mode)
        {
            this.mode = mode;
        }

        internal IReadOnlyList<(string Method, string Path)> Requests
        {
            get { lock (gate) { return requests.ToList(); } }
        }

        internal JsonObject Journal()
        {
            lock (gate)
            {
                return StubEnvironment.Journal(journal.DeepClone().AsArray());
            }
        }

        internal HttpResponseMessage Answer(HttpRequestMessage request)
        {
            string path = request.RequestUri.AbsolutePath;
            string method = request.Method.Method.ToUpperInvariant();
            lock (gate)
            {
                (int status, string body, string operation) = Route(method, path);
                requests.Add((method, path));
                var entry = new JsonObject
                {
                    ["seq"] = journal.Count + 1,
                    ["epoch"] = 1,
                    ["method"] = method,
                    ["path"] = path,
                    ["operation"] = operation,
                    ["id"] = null,
                    ["fault_id"] = null,
                    ["status"] = status,
                };
                if (request.Headers.Contains("X-FilesAPI-Key"))
                {
                    entry["credentials"] = new JsonObject { ["api_key"] = 1 };
                }
                entry["user_agent"] = request.Headers.UserAgent.ToString();
                journal.Add(entry);
                var response = new HttpResponseMessage((HttpStatusCode)status) { RequestMessage = request };
                if (body != null)
                {
                    response.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }
                return response;
            }
        }

        // The answer to METHOD PATH, and the operation the simulator routes it to (null for none).
        private (int Status, string Body, string Operation) Route(string method, string path)
        {
            string metadata = Below(path, "/api/rest/v1/file_actions/metadata");
            string folders = Below(path, "/api/rest/v1/folders");
            string files = Below(path, "/api/rest/v1/files");
            if (method == "GET" && metadata != null)
            {
                if (metadata.Length == 0)
                {
                    return Error(422, "bad-request/request-params-required", "files.metadata");
                }
                if (metadata == created)
                {
                    return (200, Entry(created, "directory"), "files.metadata");
                }
                bool unmapped = mode == "unmapped-404" || (mode == "unmapped-absence" && metadata == deleted);
                return unmapped ? UnmappedNotFound("files.metadata") : Error(404, "not-found", "files.metadata");
            }
            if (method == "GET" && folders != null)
            {
                return folders.Length == 0 ? (200, "[]", "folders.list") : Error(404, "not-found", "folders.list");
            }
            if (method == "POST" && !string.IsNullOrEmpty(folders))
            {
                if (created != null)
                {
                    return Error(422, "processing-failure/destination-exists", "folders.create");
                }
                switch (mode)
                {
                    case "refused":
                        return Error(422, "processing-failure/destination-exists", "folders.create");
                    case "another-path":
                        return (201, Entry(folders + "-elsewhere", "directory"), "folders.create");
                    case "a-file":
                        return (201, Entry(folders, "file"), "folders.create");
                    default:
                        created = folders;
                        return (201, Entry(folders, "directory"), "folders.create");
                }
            }
            if (method == "DELETE" && files != null && files == created)
            {
                deleted = created;
                created = null;
                return (204, null, "files.delete");
            }
            return Error(501, "simulation/not-supported", null);
        }

        private static string Entry(string path, string type)
        {
            var entry = new JsonObject { ["path"] = path, ["type"] = type };
            if (type == "file")
            {
                entry["size"] = 0;
            }
            return entry.ToJsonString();
        }

        // The path below prefix, unescaped and without slashes at either end; null when path is not below it.
        private static string Below(string path, string prefix)
        {
            if (path != prefix && !path.StartsWith(prefix + "/", StringComparison.Ordinal))
            {
                return null;
            }
            return Uri.UnescapeDataString(path.Substring(prefix.Length)).Trim('/');
        }

        private static (int Status, string Body, string Operation) Error(int status, string type, string operation)
        {
            return (status, new JsonObject { ["error"] = type, ["http-code"] = status, ["type"] = type }.ToJsonString(), operation);
        }

        // A 404 not-found whose http-code is no number, so the SDK cannot read it as a Files.com error.
        private static (int Status, string Body, string Operation) UnmappedNotFound(string operation)
        {
            return (404, new JsonObject { ["error"] = "Not Found", ["http-code"] = new JsonObject { ["not"] = "a number" }, ["type"] = "not-found" }.ToJsonString(), operation);
        }
    }

    internal sealed class StubHandler : HttpMessageHandler
    {
        private readonly StubFilesApi api;

        internal StubHandler(StubFilesApi api)
        {
            this.api = api;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(api.Answer(request));
        }
    }

    // Storage at the network boundary for the cancellation control: each storage request is answered 200 only once
    // released, and an API request at once.
    internal sealed class HeldStorage : HttpMessageHandler
    {
        internal const string Url = "http://storage-control.invalid";

        private readonly TaskCompletionSource<bool> released = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        internal void Release()
        {
            released.TrySetResult(true);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri.GetLeftPart(UriPartial.Authority) == Url)
            {
                await released.Task;
            }
            return new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request };
        }
    }

    // A canonical simulator as the controls stand it in, in the simulator's own shapes: the readiness the launcher saved
    // before its reset, the reset's answer and live readiness of epoch 1 under the canonical profile, and the journal; what
    // binding reads agrees with the generated manifest and the case, and the network ends in the stub.
    internal sealed class StubEnvironment : IScenarioEnvironment
    {
        internal const string Url = "http://api-scenarios-control.invalid";
        internal const string Key = "control-key";
        internal const string Instance = "control-instance";
        private const string SchemaSubset = "5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c5c";

        private readonly ScenarioCase scenarioCase;
        private readonly StubFilesApi api;

        internal StubEnvironment(ScenarioCase scenarioCase, StubFilesApi api)
        {
            this.scenarioCase = scenarioCase;
            this.api = api;
        }

        public string BaseUrl => Url;

        public string ApiKey => Key;

        public string RunId => "control";

        public string ExpectedCase => scenarioCase.Id;

        public string ReceiptPath => null;

        public BindingInputs ReadReceipts()
        {
            return Inputs(scenarioCase);
        }

        public Task<JsonObject> ControlAsync(string name, CancellationToken token)
        {
            return Task.FromResult(name == "ready" ? Readiness() : name == "journal" ? api.Journal() : throw new InvalidOperationException($"the stub has no {name}"));
        }

        public HttpMessageHandler CreateNetworkHandler()
        {
            return new StubHandler(api);
        }

        internal static BindingInputs Inputs(ScenarioCase scenarioCase)
        {
            JsonObject manifest = JsonNode.Parse(System.IO.File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "api-scenarios.json"))).AsObject();
            // The readiness the launcher saved precedes its reset: the epoch and the default profile before it.
            JsonObject saved = Readiness();
            saved["epoch"] = 0;
            saved["profile"] = new JsonObject { ["file_actions"] = "completed", ["upload"] = new JsonObject { ["mode"] = "legacy" } };
            return new BindingInputs
            {
                Manifest = manifest,
                ReadyReceipt = saved,
                Ready = Readiness(),
                Reset = new JsonObject { ["epoch"] = 1, ["instance"] = Instance, ["api_scenarios"] = ApiScenarios() },
                ScenarioName = ScenarioBinding.Profile,
                Scenario = new JsonObject
                {
                    ["name"] = ScenarioBinding.Profile,
                    ["reset"] = new JsonObject
                    {
                        ["profile"] = new JsonObject
                        {
                            ["api_scenarios"] = new JsonObject { ["format"] = "files-api-scenarios", ["format_version"] = 1, ["definition_sha256"] = scenarioCase.DefinitionSha256 },
                            ["upload"] = new JsonObject { ["mode"] = "serial", ["fixed_part_length"] = true },
                        },
                    },
                },
                Generation = new JsonObject { ["schema_sha256"] = SchemaSubset, ["swagger"] = new JsonObject { ["sha256"] = JsonRead.Text(manifest, "schema", "sha256") } },
                Instance = Instance,
                SchemaSha256 = SchemaSubset,
                Journal = Journal(new JsonArray()),
            };
        }

        // The api_scenarios identity the reset chose (profile.rb#validated_api_scenarios).
        internal static JsonObject ApiScenarios()
        {
            return new JsonObject { ["format"] = "files-api-scenarios", ["format_version"] = 1, ["definition_sha256"] = ApiScenarioCases.DefinitionSha256 };
        }

        // The simulator's journal answer (GET /__files_mock/v1/journal) around ENTRIES: its epoch, the entries, limit and
        // drops, and the instance and identity it echoes under the api_scenarios profile.
        internal static JsonObject Journal(JsonArray entries)
        {
            return new JsonObject
            {
                ["epoch"] = 1,
                ["entries"] = entries,
                ["limit"] = 10000,
                ["dropped"] = 0,
                ["complete"] = true,
                ["upload_parts"] = new JsonObject { ["in_flight"] = 0, ["most_in_flight"] = 0 },
                ["instance"] = Instance,
                ["api_scenarios"] = ApiScenarios(),
            };
        }

        // Live readiness after the reset (GET /__files_mock/v1/ready): its epoch, the canonical profile, and the operations
        // the simulator routes, by the IDs its journal names them with (simulation.rb's OPERATIONS and TRANSFER_OPERATIONS).
        internal static JsonObject Readiness()
        {
            var operations = new JsonArray();
            foreach ((string id, string method, string path) in new[]
            {
                ("files.begin_upload", "POST", "/file_actions/begin_upload/{path}"),
                ("files.finalize_upload", "POST", "/files/{path}"),
                ("files.download", "GET", "/files/{path}"),
                ("files.metadata", "GET", "/file_actions/metadata/{path}"),
                ("files.delete", "DELETE", "/files/{path}"),
                ("folders.create", "POST", "/folders/{path}"),
                ("folders.list", "GET", "/folders/{path}"),
                ("files.copy", "POST", "/file_actions/copy/{path}"),
                ("files.move", "POST", "/file_actions/move/{path}"),
                ("file_migrations.find", "GET", "/file_migrations/{id}"),
            })
            {
                operations.Add(new JsonObject { ["id"] = id, ["method"] = method, ["path"] = ApiRoutes.ApiPrefix + path });
            }
            return new JsonObject
            {
                ["status"] = "ready",
                ["mode"] = "simulation",
                ["contract_version"] = 3,
                ["instance"] = Instance,
                ["schema_sha256"] = SchemaSubset,
                ["epoch"] = 1,
                ["operations"] = operations,
                ["transfers"] = new JsonObject
                {
                    ["operations"] = new JsonArray
                    {
                        new JsonObject { ["id"] = "transfers.upload_part", ["method"] = "PUT" },
                        new JsonObject { ["id"] = "transfers.download", ["method"] = "GET" },
                    },
                },
                ["profile"] = new JsonObject
                {
                    ["file_actions"] = "completed",
                    ["upload"] = new JsonObject { ["mode"] = "serial", ["retry_parts"] = true, ["partsize"] = 5242880, ["fixed_part_length"] = true },
                    ["list_cursors"] = "opaque",
                    ["api_scenarios"] = ApiScenarios(),
                },
            };
        }
    }
}