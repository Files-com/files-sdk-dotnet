using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace FilesContractTests.Records
{
    // Imported inside the namespace so that model names, such as Lock, take precedence over those of the namespaces
    // above, such as System.Threading.Lock.
    using FilesCom.Models;

    // The record contract of App, generated from the schema: each case sends one request through the
    // SDK's Apps operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class AppRecordContractTest : ContractCase
    {
        private const string ListPath = "/apps";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["app_type"] = "sdk", ["extended_description"] = "contract-extended_description", ["extended_description_for_marketing_site"] = "contract-extended_description_for_marketing_site", ["external_homepage_url"] = "contract-external_homepage_url", ["featured"] = true, ["folder_behavior_type"] = "webhook", ["icon_url"] = "contract-icon_url", ["logo_thumbnail_url"] = "contract-logo_thumbnail_url", ["logo_url"] = "contract-logo_url", ["marketing_intro"] = "contract-marketing_intro", ["marketing_youtube_url"] = "contract-marketing_youtube_url", ["name"] = "contract-name", ["package_manager_install_command"] = "contract-package_manager_install_command", ["remote_server_type"] = "ftp", ["screenshot_list_urls"] = new JsonArray("a"), ["sdk_installation_instructions_link"] = "contract-sdk_installation_instructions_link", ["short_description"] = "contract-short_description", ["sso_strategy_type"] = "google", ["siem_type"] = "generic", ["tutorial_youtube_url"] = "contract-tutorial_youtube_url" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            return Conform(await Simulator.ListRawAsync(ListPath, Scope()));
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "App", ("app_type", "string"), ("extended_description", "string"), ("extended_description_for_marketing_site", "string"), ("external_homepage_url", "string"), ("featured", "boolean"), ("folder_behavior_type", "string"), ("icon_url", "string"), ("logo_thumbnail_url", "string"), ("logo_url", "string"), ("marketing_intro", "string"), ("marketing_youtube_url", "string"), ("name", "string"), ("package_manager_install_command", "string"), ("remote_server_type", "string"), ("screenshot_list_urls", "array(string)"), ("sdk_installation_instructions_link", "string"), ("short_description", "string"), ("sso_strategy_type", "string"), ("siem_type", "string"), ("tutorial_youtube_url", "string"));
            }
            return records;
        }

        // Every property the record holds, as the App the SDK returns gives it.
        private static void AssertModel(JsonObject record, App model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no App");
            Simulator.Same(record, "app_type", model.AppType, what);
            Simulator.Same(record, "extended_description", model.ExtendedDescription, what);
            Simulator.Same(record, "extended_description_for_marketing_site", model.ExtendedDescriptionForMarketingSite, what);
            Simulator.Same(record, "external_homepage_url", model.ExternalHomepageUrl, what);
            Simulator.Same(record, "featured", model.Featured, what);
            Simulator.Same(record, "folder_behavior_type", model.FolderBehaviorType, what);
            Simulator.Same(record, "icon_url", model.IconUrl, what);
            Simulator.Same(record, "logo_thumbnail_url", model.LogoThumbnailUrl, what);
            Simulator.Same(record, "logo_url", model.LogoUrl, what);
            Simulator.Same(record, "marketing_intro", model.MarketingIntro, what);
            Simulator.Same(record, "marketing_youtube_url", model.MarketingYoutubeUrl, what);
            Simulator.Same(record, "name", model.Name, what);
            Simulator.Same(record, "package_manager_install_command", model.PackageManagerInstallCommand, what);
            Simulator.Same(record, "remote_server_type", model.RemoteServerType, what);
            Simulator.Same(record, "screenshot_list_urls", model.ScreenshotListUrls, what);
            Simulator.Same(record, "sdk_installation_instructions_link", model.SdkInstallationInstructionsLink, what);
            Simulator.Same(record, "short_description", model.ShortDescription, what);
            Simulator.Same(record, "sso_strategy_type", model.SsoStrategyType, what);
            Simulator.Same(record, "siem_type", model.SiemType, what);
            Simulator.Same(record, "tutorial_youtube_url", model.TutorialYoutubeUrl, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("App", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            App[] listed = (await Simulator.Sdk(operation).Apps.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"App.list[{i}]");
            }
        }
    }
}