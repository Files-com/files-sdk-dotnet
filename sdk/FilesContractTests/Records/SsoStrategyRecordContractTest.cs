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

    // The record contract of SsoStrategy, generated from the schema: each case sends one request through the
    // SDK's SsoStrategies operations and checks it against the simulator's journal and the record a raw read finds there.
    [TestClass]
    public class SsoStrategyRecordContractTest : ContractCase
    {
        private const string ListPath = "/sso_strategies";

        // The parameters that select the records of one scope, where the resource has scopes.
        private static JsonObject Scope()
        {
            return Simulator.Json(new Dictionary<string, object> { });
        }

        // The case's starting records: count of them, made by a raw create as the case's create would make them, or
        // by a reset fixture for a resource without one.
        private static async Task<JsonObject[]> RecordsAsync(int count)
        {
            JsonObject record = new JsonObject { ["protocol"] = "oauth2", ["provider"] = "google", ["label"] = "contract-label", ["logo_url"] = "contract-logo_url", ["enabled"] = true, ["user_count"] = 7, ["saml_provider_cert_fingerprint"] = "contract-saml_provider_cert_fingerprint", ["saml_provider_issuer_url"] = "contract-saml_provider_issuer_url", ["saml_provider_metadata_content"] = "contract-saml_provider_metadata_content", ["saml_provider_metadata_url"] = "contract-saml_provider_metadata_url", ["saml_provider_slo_target_url"] = "contract-saml_provider_slo_target_url", ["saml_provider_sso_target_url"] = "contract-saml_provider_sso_target_url", ["scim_authentication_method"] = "none", ["scim_username"] = "contract-scim_username", ["scim_oauth_access_token"] = "contract-scim_oauth_access_token", ["scim_oauth_access_token_expires_at"] = "contract-scim_oauth_access_token_expires_at", ["subdomain"] = "contract-subdomain", ["provision_users"] = true, ["provision_groups"] = true, ["deprovision_users"] = true, ["deprovision_groups"] = true, ["deprovision_behavior"] = "disable", ["provision_group_default"] = "contract-provision_group_default", ["provision_group_exclusion"] = "contract-provision_group_exclusion", ["provision_group_inclusion"] = "contract-provision_group_inclusion", ["provision_group_required"] = "contract-provision_group_required", ["provision_email_signup_groups"] = "contract-provision_email_signup_groups", ["provision_readonly_site_admin_groups"] = "contract-provision_readonly_site_admin_groups", ["provision_site_admin_groups"] = "contract-provision_site_admin_groups", ["provision_group_admin_groups"] = "contract-provision_group_admin_groups", ["provision_attachments_permission"] = true, ["provision_dav_permission"] = true, ["provision_ftp_permission"] = true, ["provision_sftp_permission"] = true, ["provision_time_zone"] = "contract-provision_time_zone", ["provision_company"] = "contract-provision_company", ["provision_require_2fa"] = "use_system_setting", ["provision_filesystem_layout"] = "site_root", ["provider_identifier"] = "contract-provider_identifier", ["ldap_base_dn"] = "contract-ldap_base_dn", ["ldap_domain"] = "contract-ldap_domain", ["display_on_login_page"] = true, ["ldap_host"] = "contract-ldap_host", ["ldap_host_2"] = "contract-ldap_host_2", ["ldap_host_3"] = "contract-ldap_host_3", ["ldap_port"] = 7, ["ldap_provisioning_enabled"] = true, ["ldap_secure"] = true, ["ldap_server_certificate"] = "require_match", ["ldap_type"] = "active_directory", ["ldap_username"] = "contract-ldap_username", ["ldap_username_field"] = "sAMAccountName" };
            await Simulator.ResetWithAsync(ListPath.TrimStart('/'), Enumerable.Repeat(record, count).ToArray());
            // Fixture records get IDs from 1 in the order given.
            List<JsonObject> held = new List<JsonObject>();
            for (int id = 1; id <= count; id++)
            {
                held.Add(await Simulator.ReadRawAsync(ListPath, $"{ListPath}/{id}", null, null));
            }
            return Conform(held.ToArray());
        }

        // The records the case starts from, if they hold the types the schema declares.
        private static JsonObject[] Conform(JsonObject[] records)
        {
            foreach (JsonObject record in records)
            {
                Simulator.Conforms(record, "SsoStrategy", ("protocol", "string"), ("provider", "string"), ("label", "string"), ("logo_url", "string"), ("id", "int64"), ("enabled", "boolean"), ("user_count", "int64"), ("saml_provider_cert_fingerprint", "string"), ("saml_provider_issuer_url", "string"), ("saml_provider_metadata_content", "string"), ("saml_provider_metadata_url", "string"), ("saml_provider_slo_target_url", "string"), ("saml_provider_sso_target_url", "string"), ("scim_authentication_method", "string"), ("scim_username", "string"), ("scim_oauth_access_token", "string"), ("scim_oauth_access_token_expires_at", "string"), ("subdomain", "string"), ("provision_users", "boolean"), ("provision_groups", "boolean"), ("deprovision_users", "boolean"), ("deprovision_groups", "boolean"), ("deprovision_behavior", "string"), ("provision_group_default", "string"), ("provision_group_exclusion", "string"), ("provision_group_inclusion", "string"), ("provision_group_required", "string"), ("provision_email_signup_groups", "string"), ("provision_readonly_site_admin_groups", "string"), ("provision_site_admin_groups", "string"), ("provision_group_admin_groups", "string"), ("provision_attachments_permission", "boolean"), ("provision_dav_permission", "boolean"), ("provision_ftp_permission", "boolean"), ("provision_sftp_permission", "boolean"), ("provision_time_zone", "string"), ("provision_company", "string"), ("provision_require_2fa", "string"), ("provision_filesystem_layout", "string"), ("provider_identifier", "string"), ("ldap_base_dn", "string"), ("ldap_domain", "string"), ("display_on_login_page", "boolean"), ("ldap_host", "string"), ("ldap_host_2", "string"), ("ldap_host_3", "string"), ("ldap_port", "int64"), ("ldap_provisioning_enabled", "boolean"), ("ldap_secure", "boolean"), ("ldap_server_certificate", "string"), ("ldap_type", "string"), ("ldap_username", "string"), ("ldap_username_field", "string"));
            }
            return records;
        }

        private static string MemberPath(JsonObject record)
        {
            return $"{ListPath}/{(long)record["id"]}";
        }

        // Every property the record holds, as the SsoStrategy the SDK returns gives it.
        private static void AssertModel(JsonObject record, SsoStrategy model, string what)
        {
            Assert.IsNotNull(model, $"{what}: the SDK returned no SsoStrategy");
            Simulator.Same(record, "protocol", model.Protocol, what);
            Simulator.Same(record, "provider", model.Provider, what);
            Simulator.Same(record, "label", model.Label, what);
            Simulator.Same(record, "logo_url", model.LogoUrl, what);
            Simulator.Same(record, "id", model.Id, what);
            Simulator.Same(record, "enabled", model.Enabled, what);
            Simulator.Same(record, "user_count", model.UserCount, what);
            Simulator.Same(record, "saml_provider_cert_fingerprint", model.SamlProviderCertFingerprint, what);
            Simulator.Same(record, "saml_provider_issuer_url", model.SamlProviderIssuerUrl, what);
            Simulator.Same(record, "saml_provider_metadata_content", model.SamlProviderMetadataContent, what);
            Simulator.Same(record, "saml_provider_metadata_url", model.SamlProviderMetadataUrl, what);
            Simulator.Same(record, "saml_provider_slo_target_url", model.SamlProviderSloTargetUrl, what);
            Simulator.Same(record, "saml_provider_sso_target_url", model.SamlProviderSsoTargetUrl, what);
            Simulator.Same(record, "scim_authentication_method", model.ScimAuthenticationMethod, what);
            Simulator.Same(record, "scim_username", model.ScimUsername, what);
            Simulator.Same(record, "scim_oauth_access_token", model.ScimOauthAccessToken, what);
            Simulator.Same(record, "scim_oauth_access_token_expires_at", model.ScimOauthAccessTokenExpiresAt, what);
            Simulator.Same(record, "subdomain", model.Subdomain, what);
            Simulator.Same(record, "provision_users", model.ProvisionUsers, what);
            Simulator.Same(record, "provision_groups", model.ProvisionGroups, what);
            Simulator.Same(record, "deprovision_users", model.DeprovisionUsers, what);
            Simulator.Same(record, "deprovision_groups", model.DeprovisionGroups, what);
            Simulator.Same(record, "deprovision_behavior", model.DeprovisionBehavior, what);
            Simulator.Same(record, "provision_group_default", model.ProvisionGroupDefault, what);
            Simulator.Same(record, "provision_group_exclusion", model.ProvisionGroupExclusion, what);
            Simulator.Same(record, "provision_group_inclusion", model.ProvisionGroupInclusion, what);
            Simulator.Same(record, "provision_group_required", model.ProvisionGroupRequired, what);
            Simulator.Same(record, "provision_email_signup_groups", model.ProvisionEmailSignupGroups, what);
            Simulator.Same(record, "provision_readonly_site_admin_groups", model.ProvisionReadonlySiteAdminGroups, what);
            Simulator.Same(record, "provision_site_admin_groups", model.ProvisionSiteAdminGroups, what);
            Simulator.Same(record, "provision_group_admin_groups", model.ProvisionGroupAdminGroups, what);
            Simulator.Same(record, "provision_attachments_permission", model.ProvisionAttachmentsPermission, what);
            Simulator.Same(record, "provision_dav_permission", model.ProvisionDavPermission, what);
            Simulator.Same(record, "provision_ftp_permission", model.ProvisionFtpPermission, what);
            Simulator.Same(record, "provision_sftp_permission", model.ProvisionSftpPermission, what);
            Simulator.Same(record, "provision_time_zone", model.ProvisionTimeZone, what);
            Simulator.Same(record, "provision_company", model.ProvisionCompany, what);
            Simulator.Same(record, "provision_require_2fa", model.ProvisionRequire2fa, what);
            Simulator.Same(record, "provision_filesystem_layout", model.ProvisionFilesystemLayout, what);
            Simulator.Same(record, "provider_identifier", model.ProviderIdentifier, what);
            Simulator.Same(record, "ldap_base_dn", model.LdapBaseDn, what);
            Simulator.Same(record, "ldap_domain", model.LdapDomain, what);
            Simulator.Same(record, "display_on_login_page", model.DisplayOnLoginPage, what);
            Simulator.Same(record, "ldap_host", model.LdapHost, what);
            Simulator.Same(record, "ldap_host_2", model.LdapHost2, what);
            Simulator.Same(record, "ldap_host_3", model.LdapHost3, what);
            Simulator.Same(record, "ldap_port", model.LdapPort, what);
            Simulator.Same(record, "ldap_provisioning_enabled", model.LdapProvisioningEnabled, what);
            Simulator.Same(record, "ldap_secure", model.LdapSecure, what);
            Simulator.Same(record, "ldap_server_certificate", model.LdapServerCertificate, what);
            Simulator.Same(record, "ldap_type", model.LdapType, what);
            Simulator.Same(record, "ldap_username", model.LdapUsername, what);
            Simulator.Same(record, "ldap_username_field", model.LdapUsernameField, what);
        }

        [TestMethod]
        public async Task List()
        {
            string operation = await Simulator.RecordOperationAsync("SsoStrategy", "list");
            await RecordsAsync(2);
            JsonObject[] held = Conform(await Simulator.ListRawAsync(ListPath, Scope()));
            Assert.AreEqual(2, held.Length, "the simulator holds the case's two records");
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SsoStrategy[] listed = (await Simulator.Sdk(operation).SsoStrategies.List(sent).AllAsync()).ToArray();
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { });
            Assert.AreEqual(held.Length, listed.Length, "the SDK lists every record, once");
            for (int i = 0; i < held.Length; i++)
            {
                AssertModel(held[i], listed[i], $"SsoStrategy.list[{i}]");
            }
        }

        [TestMethod]
        public async Task Find()
        {
            string operation = await Simulator.RecordOperationAsync("SsoStrategy", "find");
            JsonObject record = (await RecordsAsync(1))[0];
            Dictionary<string, object> sent = new Dictionary<string, object> { };
            SsoStrategy found = await Simulator.Sdk(operation).SsoStrategies.FindAsync((long)record["id"], sent);
            await Simulator.SdkRequestAsync(operation, 200, new JsonObject { ["id"] = "string" });
            AssertModel(record, found, "SsoStrategy.find");
        }
    }
}