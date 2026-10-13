using FilesCom;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text.Json.Nodes;

namespace FilesContractTests
{
    // Records which FilesCom assembly the cases run, before any case starts, in the file FILES_CONTRACT_IDENTITY_FILE
    // names: contract-test.rb checks that it is the package or project the run is bound to.
    [TestClass]
    public class SdkIdentity
    {
        [AssemblyInitialize]
        public static void Record(TestContext context)
        {
            string file = Environment.GetEnvironmentVariable("FILES_CONTRACT_IDENTITY_FILE");
            if (string.IsNullOrEmpty(file))
            {
                return;
            }
            Assembly sdk = typeof(FilesClient).Assembly;
            JsonObject identity = new JsonObject
            {
                ["assembly"] = sdk.Location,
                ["sha256"] = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(sdk.Location))).ToLowerInvariant(),
                ["version"] = sdk.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
                ["target_framework"] = sdk.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName,
                ["runtime"] = RuntimeInformation.FrameworkDescription,
            };
            File.WriteAllText(file, identity.ToJsonString());
        }
    }
}