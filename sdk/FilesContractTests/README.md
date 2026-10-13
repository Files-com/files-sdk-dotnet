# Generated contract tests

These cases are generated from the Files.com API schema: one for each record operation (list, create, find, update
and delete) of each model, run against the Files.com API simulator (the SDK generator's `mock-server` target in
simulation mode, contract version 3) generated from the same schema. Each case sends one request through the SDK
and checks it independently of the SDK's answer:

* the simulator's journal shows the operation the request reached, its status, the credential and User-Agent it
  carried, and the JSON type of every value it sent;
* raw reads show the record the simulator keeps, which must hold every value sent, and which the model the SDK
  returns must match property by property.

`contract-cases.json` names the schema and every case. Run the cases with the entrypoint beside the SDK, which binds
the simulator to that schema, starts and stops it, and accounts for every case on each target framework:

    ruby contract-test.rb MOCK_SERVER OUT [PACKAGE.nupkg PACKAGE_SHA256]

`MOCK_SERVER` is the generated simulator's directory, with its bundle installed. Without a package the cases build
against `sdk/FilesCom`; with one, against that exact archive, installed as an application installs it. `test.sh` runs
them after the unit tests, against a package it packs, with `FILES_CONTRACT_MOCK_SERVER` naming that directory; it
refuses to start without it. As the .NET CI job does, generate the simulator from the same schema with the generator's
`--target mock-server`, then run `bundle install` in `generated/mock-server`. The simulator runs in that bundle, not
the caller's: under the generator's `--run-tests`, which runs `test.sh` inside the generator's `bundle exec`, it starts
without any of the caller's `BUNDLE_*` settings, so its gems must be installed where its own Gemfile finds them. If it
cannot start, the failure quotes its output and names the kept `simulator.log`; a failed `test.sh` keeps its results
and prints where. The tests need the .NET SDK that
`global.json` names, with the .NET 8 runtime as well for the `net8.0` framework, Ruby, and access to nuget.org, whose
vulnerability audit every restore runs.

A case whose operation the simulator's inventory declares neither a record operation nor the specialized operation the
case is generated to bind (`specialized` in `contract-cases.json`: users' operations, which the simulator keeps as
records, and a file migration's find, read from a migration the case completes with a pending copy) reports the
simulator's declaration. A case whose own raw requests show, in its setup before the SDK is called, that the simulator cannot serve
it (a request refused as not simulated, or a record contradicting the schema) reports that. Neither is a pass. Once a
case has called the SDK, its raw requests verify what the SDK did, and the same refusal fails the case, naming the SDK
operation.

The run fails unless the project targets one or more distinct frameworks and, on each, `dotnet test` exits 0 and every
generated case is discovered once and reports one result. Any other skipped, missing, extra, repeated or failing case,
or a framework that executes none, fails.
