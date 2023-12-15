# FilesCom.Models.SsoStrategy

## Example SsoStrategy Object

```
{
  "protocol": "oauth2",
  "provider": "okta",
  "label": "My Corporate SSO Provider",
  "logo_url": "https://mysite.files.com/.../logo.png",
  "id": 1,
  "user_count": 1,
  "saml_provider_cert_fingerprint": "example",
  "saml_provider_issuer_url": "example",
  "saml_provider_metadata_content": "example",
  "saml_provider_metadata_url": "example",
  "saml_provider_slo_target_url": "example",
  "saml_provider_sso_target_url": "example",
  "scim_authentication_method": "example",
  "scim_username": "example",
  "scim_oauth_access_token": "example",
  "scim_oauth_access_token_expires_at": "example",
  "subdomain": "my-site",
  "provision_users": true,
  "provision_groups": true,
  "deprovision_users": true,
  "deprovision_groups": true,
  "deprovision_behavior": "disable",
  "provision_group_default": "Employees",
  "provision_group_exclusion": "Employees",
  "provision_group_inclusion": "Employees",
  "provision_group_required": "example",
  "provision_email_signup_groups": "Employees",
  "provision_site_admin_groups": "Employees",
  "provision_group_admin_groups": "Employees",
  "provision_attachments_permission": true,
  "provision_dav_permission": true,
  "provision_ftp_permission": true,
  "provision_sftp_permission": true,
  "provision_time_zone": "Eastern Time (US & Canada)",
  "provision_company": "ACME Corp.",
  "ldap_base_dn": "example",
  "ldap_domain": "mysite.com",
  "enabled": true,
  "ldap_host": "ldap.site.com",
  "ldap_host_2": "ldap2.site.com",
  "ldap_host_3": "ldap3.site.com",
  "ldap_port": 1,
  "ldap_secure": true,
  "ldap_username": "[ldap username]",
  "ldap_username_field": "sAMAccountName"
}
```

* `protocol` / `Protocol`  (string): SSO Protocol
* `provider` / `Provider`  (string): Provider name
* `label` / `Label`  (string): Custom label for the SSO provider on the login page.
* `logo_url` / `LogoUrl`  (string): URL holding a custom logo for the SSO provider on the login page.
* `id` / `Id`  (Nullable<Int64>): ID
* `user_count` / `UserCount`  (Nullable<Int64>): Count of users with this SSO Strategy
* `saml_provider_cert_fingerprint` / `SamlProviderCertFingerprint`  (string): Identity provider sha256 cert fingerprint if saml_provider_metadata_url is not available.
* `saml_provider_issuer_url` / `SamlProviderIssuerUrl`  (string): Identity provider issuer url
* `saml_provider_metadata_content` / `SamlProviderMetadataContent`  (string): Custom identity provider metadata
* `saml_provider_metadata_url` / `SamlProviderMetadataUrl`  (string): Metadata URL for the SAML identity provider
* `saml_provider_slo_target_url` / `SamlProviderSloTargetUrl`  (string): Identity provider SLO endpoint
* `saml_provider_sso_target_url` / `SamlProviderSsoTargetUrl`  (string): Identity provider SSO endpoint if saml_provider_metadata_url is not available.
* `scim_authentication_method` / `ScimAuthenticationMethod`  (string): SCIM authentication type.
* `scim_username` / `ScimUsername`  (string): SCIM username.
* `scim_oauth_access_token` / `ScimOauthAccessToken`  (string): SCIM OAuth Access Token.
* `scim_oauth_access_token_expires_at` / `ScimOauthAccessTokenExpiresAt`  (string): SCIM OAuth Access Token Expiration Time.
* `subdomain` / `Subdomain`  (string): Subdomain
* `provision_users` / `ProvisionUsers`  (bool): Auto-provision users?
* `provision_groups` / `ProvisionGroups`  (bool): Auto-provision group membership based on group memberships on the SSO side?
* `deprovision_users` / `DeprovisionUsers`  (bool): Auto-deprovision users?
* `deprovision_groups` / `DeprovisionGroups`  (bool): Auto-deprovision group membership based on group memberships on the SSO side?
* `deprovision_behavior` / `DeprovisionBehavior`  (string): Method used for deprovisioning users.
* `provision_group_default` / `ProvisionGroupDefault`  (string): Comma-separated list of group names for groups to automatically add all auto-provisioned users to.
* `provision_group_exclusion` / `ProvisionGroupExclusion`  (string): Comma-separated list of group names for groups (with optional wildcards) that will be excluded from auto-provisioning.
* `provision_group_inclusion` / `ProvisionGroupInclusion`  (string): Comma-separated list of group names for groups (with optional wildcards) that will be auto-provisioned.
* `provision_group_required` / `ProvisionGroupRequired`  (string): Comma or newline separated list of group names (with optional wildcards) to require membership for user provisioning.
* `provision_email_signup_groups` / `ProvisionEmailSignupGroups`  (string): Comma-separated list of group names whose members will be created with email_signup authentication.
* `provision_site_admin_groups` / `ProvisionSiteAdminGroups`  (string): Comma-separated list of group names whose members will be created as Site Admins.
* `provision_group_admin_groups` / `ProvisionGroupAdminGroups`  (string): Comma-separated list of group names whose members will be provisioned as Group Admins.
* `provision_attachments_permission` / `ProvisionAttachmentsPermission`  (bool): DEPRECATED: Auto-provisioned users get Sharing permission. Use a Group with the Bundle permission instead.
* `provision_dav_permission` / `ProvisionDavPermission`  (bool): Auto-provisioned users get WebDAV permission?
* `provision_ftp_permission` / `ProvisionFtpPermission`  (bool): Auto-provisioned users get FTP permission?
* `provision_sftp_permission` / `ProvisionSftpPermission`  (bool): Auto-provisioned users get SFTP permission?
* `provision_time_zone` / `ProvisionTimeZone`  (string): Default time zone for auto provisioned users.
* `provision_company` / `ProvisionCompany`  (string): Default company for auto provisioned users.
* `ldap_base_dn` / `LdapBaseDn`  (string): Base DN for looking up users in LDAP server
* `ldap_domain` / `LdapDomain`  (string): Domain name that will be appended to LDAP usernames
* `enabled` / `Enabled`  (bool): Is strategy enabled?  This may become automatically set to `false` after a high number and duration of failures.
* `ldap_host` / `LdapHost`  (string): LDAP host
* `ldap_host_2` / `LdapHost2`  (string): LDAP backup host
* `ldap_host_3` / `LdapHost3`  (string): LDAP backup host
* `ldap_port` / `LdapPort`  (Nullable<Int64>): LDAP port
* `ldap_secure` / `LdapSecure`  (bool): Use secure LDAP?
* `ldap_username` / `LdapUsername`  (string): Username for signing in to LDAP server.
* `ldap_username_field` / `LdapUsernameField`  (string): LDAP username field


---

## List Sso Strategies

```
Task<FilesList<SsoStrategy>> SsoStrategy.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `cursor` (string): Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).


---

## Show Sso Strategy

```
Task<SsoStrategy> SsoStrategy.Find(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sso Strategy ID.


---

## Synchronize provisioning data with the SSO remote server

```
Task SsoStrategy.Sync(
    Nullable<Int64> id, 
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `id` (Nullable<Int64>): Required - Sso Strategy ID.


---

## Synchronize provisioning data with the SSO remote server

```
var SsoStrategy = SsoStrategy.ListFor(path)[0];

var parameters = new Dictionary<string, object>();


SsoStrategy.Sync
```

### Parameters

* `id` (Nullable<Int64>): Required - Sso Strategy ID.
