# Files.Models.SsoStrategy

## Example SsoStrategy Object

```
{
  "protocol": [

  ],
  "provider": "okta",
  "label": "My Corporate SSO Provider",
  "logo_url": "https://mysite.files.com/.../logo.png",
  "id": 1,
  "saml_provider_cert_fingerprint": "",
  "saml_provider_issuer_url": "",
  "saml_provider_metadata_content": "",
  "saml_provider_metadata_url": "",
  "saml_provider_slo_target_url": "",
  "saml_provider_sso_target_url": "",
  "scim_authentication_method": "",
  "scim_username": "",
  "subdomain": "my-site",
  "provision_users": true,
  "provision_groups": true,
  "deprovision_users": true,
  "deprovision_groups": true,
  "deprovision_behavior": "disable",
  "provision_group_default": "Employees",
  "provision_group_exclusion": "Employees",
  "provision_group_inclusion": "Employees",
  "provision_group_required": "",
  "provision_attachments_permission": true,
  "provision_dav_permission": true,
  "provision_ftp_permission": true,
  "provision_sftp_permission": true,
  "provision_time_zone": "Eastern Time (US & Canada)",
  "ldap_base_dn": "",
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

* `protocol` / `Protocol`  (string[]): SSO Protocol
* `provider` / `Provider`  (string): Provider name
* `label` / `Label`  (string): Custom label for the SSO provider on the login page.
* `logo_url` / `LogoUrl`  (string): URL holding a custom logo for the SSO provider on the login page.
* `id` / `Id`  (Nullable<Int64>): ID
* `saml_provider_cert_fingerprint` / `SamlProviderCertFingerprint`  (string): Identity provider sha256 cert fingerprint if saml_provider_metadata_url is not available.
* `saml_provider_issuer_url` / `SamlProviderIssuerUrl`  (string): Identity provider issuer url
* `saml_provider_metadata_content` / `SamlProviderMetadataContent`  (string): Custom identity provider metadata
* `saml_provider_metadata_url` / `SamlProviderMetadataUrl`  (string): Metadata URL for the SAML identity provider
* `saml_provider_slo_target_url` / `SamlProviderSloTargetUrl`  (string): Identity provider SLO endpoint
* `saml_provider_sso_target_url` / `SamlProviderSsoTargetUrl`  (string): Identity provider SSO endpoint if saml_provider_metadata_url is not available.
* `scim_authentication_method` / `ScimAuthenticationMethod`  (string): SCIM authentication type.
* `scim_username` / `ScimUsername`  (string): SCIM username.
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
* `provision_attachments_permission` / `ProvisionAttachmentsPermission`  (bool): Auto-provisioned users get Sharing permission?
* `provision_dav_permission` / `ProvisionDavPermission`  (bool): Auto-provisioned users get WebDAV permission?
* `provision_ftp_permission` / `ProvisionFtpPermission`  (bool): Auto-provisioned users get FTP permission?
* `provision_sftp_permission` / `ProvisionSftpPermission`  (bool): Auto-provisioned users get SFTP permission?
* `provision_time_zone` / `ProvisionTimeZone`  (string): Default time zone for auto provisioned users.
* `ldap_base_dn` / `LdapBaseDn`  (string): Base DN for looking up users in LDAP server
* `ldap_domain` / `LdapDomain`  (string): Domain name that will be appended to LDAP usernames
* `enabled` / `Enabled`  (bool): Is strategy enabled?
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
Task<SsoStrategy[]> SsoStrategy.List(
    
    Dictionary<string, object> parameters = null,
    Dictionary<string, object> options = null
)
```

### Parameters

* `page` (Nullable<Int64>): Current page number.
* `per_page` (Nullable<Int64>): Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
* `action` (string): Deprecated: If set to `count` returns a count of matching records rather than the records themselves.
* `cursor` (string): Send cursor to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.


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
