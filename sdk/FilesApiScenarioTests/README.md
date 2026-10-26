# Generated API scenario tests

These tests run the Files.com API scenarios every client shares (`verification/scenarios/api.json` in the SDK
generator) through this SDK, as an application uses it: an explicit `FilesClient` with its own transport, the generated
operations of `FilesClient`'s operations properties, `FilesList` pages, `client.Transfers` uploads and downloads of files,
streams and directories, and `FileAction.WaitAsync` for copies and moves. They are generated from the definition and the
API schema: `ApiScenarioTests.cs` holds one test for each scenario, with every step bound to the schema's own operation,
parameters and response fields, and `api-scenarios.json` names the definition, the schema and each case. A step the
schema or this SDK cannot carry out fails generation, naming it.

They are separate from the record contract cases (`../FilesContractTests`), which keep their own exact account of the
record operations they check.

## Running them

`test.sh` runs them after the unit tests and the record contract cases, against the package it packed, with the
simulator `FILES_CONTRACT_MOCK_SERVER` names. To run them alone:

    ruby api-scenarios.rb MOCK_SERVER OUT PACKAGE.nupkg PACKAGE_SHA256

`MOCK_SERVER` is the generated mock server (contract version 3) generated from the same schema, with its bundle installed.
`OUT`, outside this SDK (checked, links followed, before it is made) and absent or empty, receives every result. The run
installs the exact archive as an application installs it: FilesCom only from a feed holding that archive, every other
package from the sources `FILES_CONTRACT_NUGET_SOURCES` lists, as the record contract cases take them (nuget.org by
default), with its own package folder.

On each target framework of the project (`net10.0` and `net8.0`, each needing its runtime installed), the run:

1. restores and builds the project once, before any case's time starts;
2. runs the controls (`ScenarioControls`), which check the consumer's own binding, request binding, error, ownership,
   cleanup, journal, upload part counting, cancellation and downloaded tree decisions, with the real SDK where they
   depend on it (among them an upload whose next part's begin_upload is still unanswered when the caller cancels, and
   one whose next part, granted and sent before the caller cancels, reaches the transport's bottom only after, with its
   first part's answer held at the stub until the control has the upload's task and the SDK's dispatcher, past starting
   that part, has renewed the next part's already-expired URL), against stubs at the network and data boundaries and
   real temporary trees;
3. runs each case on its own, one after another, through the mock server's `with-scenario.rb`, which starts a fresh
   simulator for it under the `api-scenarios-canonical` preset (serial uploads, each part granted its exact length) with
   a 16 MiB body limit, so the 6 MiB + 3 byte file goes as a 5 MiB part and a 1 MiB + 3 byte part.

A case has 175 seconds for its scenario and 30 more, with a token of its own, to clean up, within 205 seconds for
everything including its receipt. The launcher gives it 210 seconds, 30 to start the simulator and 5 to stop each process
group. These bound the harness; they are not product settings or measurements. When the run is interrupted (INT, TERM or
HUP), the running case's launcher is sent TERM, so that it stops the case and the simulator and saves the journal, and
the process groups it recorded are checked; the run then records the interruption as a failure, writes its results and
ends as that signal. Those signals are held from before a case's launcher is spawned until its groups are settled, which
happens exactly once however its wait ends, so no signal can leave a launcher this run started unsettled.

Each case binds itself before it sends anything: the generated manifest and definition, the preset the launcher loaded,
readiness from the launcher's own instance (simulation, contract version 3, its schema subset) and the raw schema the
simulator's generation records; then the simulator as the reset left it: the reset's epoch, with the launcher's instance
and the case's definition echoed, live readiness of that epoch under the canonical profile (serial uploads with fixed
part lengths) listing every operation the case sends, and a complete journal of that epoch, instance and definition. The
readiness the launcher saved before its reset is not held to the reset's epoch. The case names a folder of its own, owns
it only once the SDK's create, on that folder's path, answers 201 with exactly that folder as a directory, and stops at
its first failure. It then deletes that folder recursively, and nothing else, with its own cleanup token, and confirms it
absent with a lookup of that folder's own path.

Every request the SDK sends, to the API and to storage, is observed at the bottom of the client's transport, its start
and its end each taking a place in one order of events with the case's intent to cancel an upload. As it begins, as the
cancellation of its own token reaches that bottom (recorded before the transport below is told) and as it ends, it also
reads whether the caller's own token for the upload had actually been cancelled, apart from that intent. A position
counts only its own requests: an operation's must each be that operation (method and path template) on the path its
fixture resolves to, and an intent's must be on the paths it acts on and include the operation that starts it. An
expected error must come from the wire and be the SDK's own `ApiException` with that status and type. The requests are
joined one to one to the simulator's journal, which must show each as the operation it was meant to be (by the
operation IDs live readiness lists), with the request's own status, of the reset's epoch, with no fault (`fault_id`,
`fault_kind` or hold fields), the client's API key on API requests and no credential on storage. A request that got no
answer is accepted only as an attempt of the case's own upload that the case cancelled at its first acknowledged part,
in an upload that ended as cancelled. It ended after that intent with the caller's token actually cancelled, and its own
token was cancelled before it ended, only once the caller's had been and while the transport's own task was still
running, as read the moment that cancellation is passed on to the transport: a request that a network wait running out,
or anything else, ended or whose token it cancelled while the caller's token was not yet cancelled, or whose transport
had already ended it on its own, stays a failure, however soon the caller cancels after and however late its end is
observed. It reached the transport's bottom before the caller cancelled, or it is a part the SDK had been granted
before: one that arrived with its own token already cancelled, the only request for that part, after a begin_upload of
the upload's path negotiating exactly that part with the upload's ref was answered 2xx before the caller cancelled and
journaled as that upload's (the SDK sends a granted part as soon as the part before it ends, before that part's
progress report reaches the caller). A begin_upload that arrives only after the caller cancelled never is.
It is either a part (a `PUT` to storage without a credential, of one of the upload's parts under the live profile's one
partsize, with the ref, to the origin and under the whole upload path, everything before `{ref}/{part}`, of a part
storage acknowledged and the journal shows of that upload) or the begin_upload that negotiates such a part, which the
SDK sends while a part is still being sent (with the client's API key and the upload's ref, of a part storage
acknowledged). Its journal entry, if the server made one, must be of its own operation and upload, and the server's
status is kept apart from the client's missing one. Any other request without an answer fails its position, with its
method, path, failure and place in that order. An upload's parts are counted by their upload and part numbers in the
journal, never by their URLs; a cancelled upload's acknowledged part is one storage had already answered 2xx when the
first acknowledged progress report came, taken inside that report before cancelling. A downloaded tree must hold
exactly the folders, empty folders and files the definition's fixtures below it are. No URL query or credential is
written anywhere, and the ref a begin_upload's body names is kept only as its SHA-256.

## Results

`OUT/api-scenarios-results.json` accounts for every framework and case: the package and the loaded FilesCom assembly
(which must be the package's own for that framework), the build, the controls, each case's launcher exit, its TRX, its
receipt and the simulator's journal, and the first failure. For each framework, `OUT/<framework>/<case>/` holds:

* `receipt.json`, the case's own account: what it bound to, every position (passed, failed, excluded because only a
  Files.com site can carry it out, or unreached after a failure) with each request it sent and the journal entry it was
  joined to, upload progress and evidence, ownership, cleanup and the journal's account;
* `launcher/`, the launcher's `ready.json`, `reset.json`, `journal.json`, `run.json` and the simulator's `server.log`;
* `case.trx` and `identity.json`, the loaded FilesCom assembly.

A case passes only if every position the simulator can carry out passed, everything it owned was deleted and confirmed
absent, and its requests and the journal account for each other exactly. The positions only a Files.com site can carry
out (permissions, inherited behaviors) are listed as excluded, never credited: a pass here says nothing about them.
