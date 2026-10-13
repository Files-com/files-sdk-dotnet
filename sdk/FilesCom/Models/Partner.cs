using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    /// <summary>
    /// PartnerEntity model
    /// </summary>
    public class Partner : IModel, IJsonOnDeserialized
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        // The names of the attributes set on this object, by its constructor or its properties, since it was made
        // or last came from Files.com. SaveAsync sends these, never the defaults of attributes nobody set.
        private readonly HashSet<string> unsaved = new HashSet<string>();
        /// <summary>
        /// Makes a Partner with no attributes set and no options.
        /// </summary>
        public Partner() : this(null, null) { }

        /// <summary>
        /// Makes a Partner with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Partner(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("allow_bypassing_2fa_policies"))
            {
                this.attributes.Add("allow_bypassing_2fa_policies", false);
            }
            if (!this.attributes.ContainsKey("allowed_ips"))
            {
                this.attributes.Add("allowed_ips", null);
            }
            if (!this.attributes.ContainsKey("allow_credential_changes"))
            {
                this.attributes.Add("allow_credential_changes", false);
            }
            if (!this.attributes.ContainsKey("allow_providing_gpg_keys"))
            {
                this.attributes.Add("allow_providing_gpg_keys", false);
            }
            if (!this.attributes.ContainsKey("allow_user_creation"))
            {
                this.attributes.Add("allow_user_creation", false);
            }
            if (!this.attributes.ContainsKey("cc_emails_to_responsible_party"))
            {
                this.attributes.Add("cc_emails_to_responsible_party", false);
            }
            if (!this.attributes.ContainsKey("connections"))
            {
                this.attributes.Add("connections", new PartnerConnection[0]);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("ai_assistant_personality_id"))
            {
                this.attributes.Add("ai_assistant_personality_id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("notes"))
            {
                this.attributes.Add("notes", null);
            }
            if (!this.attributes.ContainsKey("partner_admin_ids"))
            {
                this.attributes.Add("partner_admin_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("partner_channel_template_id"))
            {
                this.attributes.Add("partner_channel_template_id", null);
            }
            if (!this.attributes.ContainsKey("partnership_role"))
            {
                this.attributes.Add("partnership_role", null);
            }
            if (!this.attributes.ContainsKey("responsible_group_id"))
            {
                this.attributes.Add("responsible_group_id", null);
            }
            if (!this.attributes.ContainsKey("responsible_user_id"))
            {
                this.attributes.Add("responsible_user_id", null);
            }
            if (!this.attributes.ContainsKey("root_folder"))
            {
                this.attributes.Add("root_folder", null);
            }
            if (!this.attributes.ContainsKey("show_partner_channel_home_page"))
            {
                this.attributes.Add("show_partner_channel_home_page", false);
            }
            if (!this.attributes.ContainsKey("tags"))
            {
                this.attributes.Add("tags", null);
            }
            if (!this.attributes.ContainsKey("user_ids"))
            {
                this.attributes.Add("user_ids", new Nullable<Int64>[0]);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        /// <summary>
        /// The value of one of this object's request options, or null if it has no option of that name.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        public object GetOption(string name)
        {
            return this.options.TryGetValue(name, out object value) ? value : null;
        }

        void IModel.SetContext(FilesClient client, Dictionary<string, object> options, string origin)
        {
            this.client = client;
            this.origin = origin;
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => origin;

        IEnumerable<object> IModel.NestedModels
        {
            get { return new object[] { Connections }; }
        }

        /// <summary>
        /// Sets one of this object's request options.
        /// </summary>
        /// <param name="name">The option's name, such as api_key.</param>
        /// <param name="value">The option's value.</param>
        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }

        // What is read from JSON, such as a response from Files.com, is not waiting to be saved.
        void IJsonOnDeserialized.OnDeserialized()
        {
            unsaved.Clear();
        }


        /// <summary>
        /// Allow Partner Admins to change Two-Factor Authentication requirements for Partner Users.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_bypassing_2fa_policies")]
        public bool AllowBypassing2faPolicies
        {
            get { return attributes["allow_bypassing_2fa_policies"] == null ? false : (bool)attributes["allow_bypassing_2fa_policies"]; }
            set { attributes["allow_bypassing_2fa_policies"] = value; unsaved.Add("allow_bypassing_2fa_policies"); }
        }

        /// <summary>
        /// A list of allowed IPs for this Partner. Newline delimited. Partner User IP access is allowed when the IP matches the Partner, User, or Site allowed IP lists.
        /// </summary>
        [JsonPropertyName("allowed_ips")]
        public string AllowedIps
        {
            get { return (string)attributes["allowed_ips"]; }
            set { attributes["allowed_ips"] = value; unsaved.Add("allowed_ips"); }
        }

        /// <summary>
        /// Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_credential_changes")]
        public bool AllowCredentialChanges
        {
            get { return attributes["allow_credential_changes"] == null ? false : (bool)attributes["allow_credential_changes"]; }
            set { attributes["allow_credential_changes"] = value; unsaved.Add("allow_credential_changes"); }
        }

        /// <summary>
        /// Allow Partner Admins to provide GPG keys.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_providing_gpg_keys")]
        public bool AllowProvidingGpgKeys
        {
            get { return attributes["allow_providing_gpg_keys"] == null ? false : (bool)attributes["allow_providing_gpg_keys"]; }
            set { attributes["allow_providing_gpg_keys"] = value; unsaved.Add("allow_providing_gpg_keys"); }
        }

        /// <summary>
        /// Allow Partner Admins to create users.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("allow_user_creation")]
        public bool AllowUserCreation
        {
            get { return attributes["allow_user_creation"] == null ? false : (bool)attributes["allow_user_creation"]; }
            set { attributes["allow_user_creation"] = value; unsaved.Add("allow_user_creation"); }
        }

        /// <summary>
        /// When `true`, emails sent to Partner users are copied to the responsible User or Group.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("cc_emails_to_responsible_party")]
        public bool CcEmailsToResponsibleParty
        {
            get { return attributes["cc_emails_to_responsible_party"] == null ? false : (bool)attributes["cc_emails_to_responsible_party"]; }
            set { attributes["cc_emails_to_responsible_party"] = value; unsaved.Add("cc_emails_to_responsible_party"); }
        }

        /// <summary>
        /// Approved Connected Sites relationships for this Partner, in both directions. Empty when this Partner has no connections. Read-only.
        /// </summary>
        [JsonPropertyName("connections")]
        public PartnerConnection[] Connections
        {
            get { return (PartnerConnection[])attributes["connections"]; }
            set { attributes["connections"] = value; unsaved.Add("connections"); }
        }

        /// <summary>
        /// The unique ID of the Partner.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// AI Assistant Personality ID assigned to this Partner, if any. Users in the Partner inherit it unless a direct per-user assignment overrides it.
        /// </summary>
        [JsonPropertyName("ai_assistant_personality_id")]
        public Nullable<Int64> AiAssistantPersonalityId
        {
            get { return (Nullable<Int64>)attributes["ai_assistant_personality_id"]; }
            set { attributes["ai_assistant_personality_id"] = value; unsaved.Add("ai_assistant_personality_id"); }
        }

        /// <summary>
        /// ID of the Workspace associated with this Partner.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// The name of the Partner.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; unsaved.Add("name"); }
        }

        /// <summary>
        /// Notes about this Partner.
        /// </summary>
        [JsonPropertyName("notes")]
        public string Notes
        {
            get { return (string)attributes["notes"]; }
            set { attributes["notes"] = value; unsaved.Add("notes"); }
        }

        /// <summary>
        /// Array of User IDs that are Partner Admins for this Partner.
        /// </summary>
        [JsonPropertyName("partner_admin_ids")]
        public Nullable<Int64>[] PartnerAdminIds
        {
            get { return (Nullable<Int64>[])attributes["partner_admin_ids"]; }
            set { attributes["partner_admin_ids"] = value; unsaved.Add("partner_admin_ids"); }
        }

        /// <summary>
        /// ID of the Partner Channel Template assigned to this Partner.
        /// </summary>
        [JsonPropertyName("partner_channel_template_id")]
        public Nullable<Int64> PartnerChannelTemplateId
        {
            get { return (Nullable<Int64>)attributes["partner_channel_template_id"]; }
            set { attributes["partner_channel_template_id"] = value; unsaved.Add("partner_channel_template_id"); }
        }

        /// <summary>
        /// This site's role for this Partner in Connected Sites relationships. `host` is a Partner this site configured. `guest` is a Partner created by approving another site's connection request; it has no root folder and cannot hold users, permissions, or Partner Channels, or host a connection. `host_and_guest` is a configured Partner that is also the guest side of a connection. Promote a `guest` Partner by setting this to `host_and_guest` together with a `root_folder`.
        /// </summary>
        [JsonPropertyName("partnership_role")]
        public string PartnershipRole
        {
            get { return (string)attributes["partnership_role"]; }
            set { attributes["partnership_role"] = value; unsaved.Add("partnership_role"); }
        }

        /// <summary>
        /// ID of the Group responsible for this Partner.
        /// </summary>
        [JsonPropertyName("responsible_group_id")]
        public Nullable<Int64> ResponsibleGroupId
        {
            get { return (Nullable<Int64>)attributes["responsible_group_id"]; }
            set { attributes["responsible_group_id"] = value; unsaved.Add("responsible_group_id"); }
        }

        /// <summary>
        /// ID of the User responsible for this Partner.
        /// </summary>
        [JsonPropertyName("responsible_user_id")]
        public Nullable<Int64> ResponsibleUserId
        {
            get { return (Nullable<Int64>)attributes["responsible_user_id"]; }
            set { attributes["responsible_user_id"] = value; unsaved.Add("responsible_user_id"); }
        }

        /// <summary>
        /// The root folder path for this Partner.
        /// </summary>
        [JsonPropertyName("root_folder")]
        public string RootFolder
        {
            get { return (string)attributes["root_folder"]; }
            set { attributes["root_folder"] = value; unsaved.Add("root_folder"); }
        }

        /// <summary>
        /// Show Partner users a simplified home page built from this Partner's Channels.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("show_partner_channel_home_page")]
        public bool ShowPartnerChannelHomePage
        {
            get { return attributes["show_partner_channel_home_page"] == null ? false : (bool)attributes["show_partner_channel_home_page"]; }
            set { attributes["show_partner_channel_home_page"] = value; unsaved.Add("show_partner_channel_home_page"); }
        }

        /// <summary>
        /// Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        /// </summary>
        [JsonPropertyName("tags")]
        public string Tags
        {
            get { return (string)attributes["tags"]; }
            set { attributes["tags"] = value; unsaved.Add("tags"); }
        }

        /// <summary>
        /// Array of User IDs that belong to this Partner.
        /// </summary>
        [JsonPropertyName("user_ids")]
        public Nullable<Int64>[] UserIds
        {
            get { return (Nullable<Int64>[])attributes["user_ids"]; }
            set { attributes["user_ids"] = value; unsaved.Add("user_ids"); }
        }

        /// <summary>
        /// Parameters:
        ///   ai_assistant_personality_id - int64 - AI Assistant Personality ID assigned to this Partner, if any. Users in the Partner inherit it unless a direct per-user assignment overrides it.
        ///   allowed_ips - string - A list of allowed IPs for this Partner. Newline delimited. Partner User IP access is allowed when the IP matches the Partner, User, or Site allowed IP lists.
        ///   allow_bypassing_2fa_policies - boolean - Allow Partner Admins to change Two-Factor Authentication requirements for Partner Users.
        ///   allow_credential_changes - boolean - Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        ///   allow_providing_gpg_keys - boolean - Allow Partner Admins to provide GPG keys.
        ///   allow_user_creation - boolean - Allow Partner Admins to create users.
        ///   cc_emails_to_responsible_party - boolean - When `true`, emails sent to Partner users are copied to the responsible User or Group.
        ///   notes - string - Notes about this Partner.
        ///   partner_channel_template_id - int64 - ID of the Partner Channel Template assigned to this Partner.
        ///   responsible_group_id - int64 - ID of the Group responsible for this Partner.
        ///   responsible_user_id - int64 - ID of the User responsible for this Partner.
        ///   show_partner_channel_home_page - boolean - Show Partner users a simplified home page built from this Partner's Channels.
        ///   tags - string - Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        ///   name - string - The name of the Partner.
        ///   partnership_role - string - This site's role for this Partner in Connected Sites relationships. `host` is a Partner this site configured. `guest` is a Partner created by approving another site's connection request; it has no root folder and cannot hold users, permissions, or Partner Channels, or host a connection. `host_and_guest` is a configured Partner that is also the guest side of a connection. Promote a `guest` Partner by setting this to `host_and_guest` together with a `root_folder`.
        ///   root_folder - string - The root folder path for this Partner.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Partner it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Partner> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   ai_assistant_personality_id - int64 - AI Assistant Personality ID assigned to this Partner, if any. Users in the Partner inherit it unless a direct per-user assignment overrides it.
        ///   allowed_ips - string - A list of allowed IPs for this Partner. Newline delimited. Partner User IP access is allowed when the IP matches the Partner, User, or Site allowed IP lists.
        ///   allow_bypassing_2fa_policies - boolean - Allow Partner Admins to change Two-Factor Authentication requirements for Partner Users.
        ///   allow_credential_changes - boolean - Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        ///   allow_providing_gpg_keys - boolean - Allow Partner Admins to provide GPG keys.
        ///   allow_user_creation - boolean - Allow Partner Admins to create users.
        ///   cc_emails_to_responsible_party - boolean - When `true`, emails sent to Partner users are copied to the responsible User or Group.
        ///   notes - string - Notes about this Partner.
        ///   partner_channel_template_id - int64 - ID of the Partner Channel Template assigned to this Partner.
        ///   responsible_group_id - int64 - ID of the Group responsible for this Partner.
        ///   responsible_user_id - int64 - ID of the User responsible for this Partner.
        ///   show_partner_channel_home_page - boolean - Show Partner users a simplified home page built from this Partner's Channels.
        ///   tags - string - Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        ///   name - string - The name of the Partner.
        ///   partnership_role - string - This site's role for this Partner in Connected Sites relationships. `host` is a Partner this site configured. `guest` is a Partner created by approving another site's connection request; it has no root folder and cannot hold users, permissions, or Partner Channels, or host a connection. `host_and_guest` is a configured Partner that is also the guest side of a connection. Promote a `guest` Partner by setting this to `host_and_guest` together with a `root_folder`.
        ///   root_folder - string - The root folder path for this Partner.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Partner it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Partner> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Partner> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Partner has no id, which Update needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("ai_assistant_personality_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ai_assistant_personality_id is null, which cannot be sent" : "Bad parameter: ai_assistant_personality_id must be of type Nullable<Int64>", "ai_assistant_personality_id");
            }
            if (parameters.TryGetValue("allowed_ips", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_ips is null, which cannot be sent" : "Bad parameter: allowed_ips must be of type string", "allowed_ips");
            }
            if (parameters.TryGetValue("allow_bypassing_2fa_policies", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_bypassing_2fa_policies is null, which cannot be sent" : "Bad parameter: allow_bypassing_2fa_policies must be of type bool", "allow_bypassing_2fa_policies");
            }
            if (parameters.TryGetValue("allow_credential_changes", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_credential_changes is null, which cannot be sent" : "Bad parameter: allow_credential_changes must be of type bool", "allow_credential_changes");
            }
            if (parameters.TryGetValue("allow_providing_gpg_keys", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_providing_gpg_keys is null, which cannot be sent" : "Bad parameter: allow_providing_gpg_keys must be of type bool", "allow_providing_gpg_keys");
            }
            if (parameters.TryGetValue("allow_user_creation", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_user_creation is null, which cannot be sent" : "Bad parameter: allow_user_creation must be of type bool", "allow_user_creation");
            }
            if (parameters.TryGetValue("cc_emails_to_responsible_party", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cc_emails_to_responsible_party is null, which cannot be sent" : "Bad parameter: cc_emails_to_responsible_party must be of type bool", "cc_emails_to_responsible_party");
            }
            if (parameters.TryGetValue("notes", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notes is null, which cannot be sent" : "Bad parameter: notes must be of type string", "notes");
            }
            if (parameters.TryGetValue("partner_channel_template_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_channel_template_id is null, which cannot be sent" : "Bad parameter: partner_channel_template_id must be of type Nullable<Int64>", "partner_channel_template_id");
            }
            if (parameters.TryGetValue("responsible_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_group_id is null, which cannot be sent" : "Bad parameter: responsible_group_id must be of type Nullable<Int64>", "responsible_group_id");
            }
            if (parameters.TryGetValue("responsible_user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_user_id is null, which cannot be sent" : "Bad parameter: responsible_user_id must be of type Nullable<Int64>", "responsible_user_id");
            }
            if (parameters.TryGetValue("show_partner_channel_home_page", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: show_partner_channel_home_page is null, which cannot be sent" : "Bad parameter: show_partner_channel_home_page must be of type bool", "show_partner_channel_home_page");
            }
            if (parameters.TryGetValue("tags", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: tags is null, which cannot be sent" : "Bad parameter: tags must be of type string", "tags");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("partnership_role", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partnership_role is null, which cannot be sent" : "Bad parameter: partnership_role must be of type string", "partnership_role");
            }
            if (parameters.TryGetValue("root_folder", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: root_folder is null, which cannot be sent" : "Bad parameter: root_folder must be of type string", "root_folder");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/partners/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            Partner updated = JsonUtil.DeserializeModelResponse<Partner>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<Partner>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /partners/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /partners/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DeleteAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>, but neither waits for the request nor reports its failure.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        [Obsolete("Destroy neither waits for the request nor reports its failure. Use DestroyAsync instead.")]
        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }

        /// <summary>
        /// Same as <see cref="DeleteAsync"/>.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task DestroyAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return DeleteAsync(parameters, cancellationToken);
        }

        private async Task DeleteCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Partner has no id, which Delete needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            await FilesClient.SendRequestAndDiscard(context, $"/partners/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "ai_assistant_personality_id", "allowed_ips", "allow_bypassing_2fa_policies", "allow_credential_changes", "allow_providing_gpg_keys", "allow_user_creation", "cc_emails_to_responsible_party", "notes", "partner_channel_template_id", "responsible_group_id", "responsible_user_id", "show_partner_channel_home_page", "tags", "name", "root_folder", "workspace_id" };
        private static readonly string[] UpdateParameterNames = { "id", "ai_assistant_personality_id", "allowed_ips", "allow_bypassing_2fa_policies", "allow_credential_changes", "allow_providing_gpg_keys", "allow_user_creation", "cc_emails_to_responsible_party", "notes", "partner_channel_template_id", "responsible_group_id", "responsible_user_id", "show_partner_channel_home_page", "tags", "name", "partnership_role", "root_folder" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Partner on Files.com, or updates it if it has an ID.
        /// It sends the attributes set on this object, in its constructor or with its properties, or changed in place,
        /// such as an element of an array, since it was made or last came from Files.com, of those that operation
        /// takes; it sends no other attributes, and no defaults. Each goes in the form its parameter takes, such as a
        /// DateTime as ISO 8601 text. When it succeeds, this object takes the attributes Files.com returns.
        /// </summary>
        /// <param name="cancellationToken">Stops the request.</param>
        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            if (this.attributes["id"] != null)
            {
                await UpdateCore(AttributesToSave(UpdateParameterNames), cancellationToken);
                return;
            }
            Partner created = await Partner.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
            Refresh(created.getAttributes());
            origin = ((IModel)created).Origin;
        }

        // The attributes set or changed on this object since it was made or last came from Files.com, of those named
        // in parameterNames: the parameters of the operation that saves it.
        private Dictionary<string, object> AttributesToSave(string[] parameterNames)
        {
            Dictionary<string, object> toSave = new Dictionary<string, object>();
            foreach (string name in parameterNames)
            {
                if (unsaved.Contains(name))
                {
                    toSave[name] = attributes[name];
                }
            }
            return toSave;
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Partner> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return ListCore(FilesClient.Instance, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id` and `name`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `workspace_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Partner> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Partner> ListCore(
            FilesClient client,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("cursor", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cursor is null, which cannot be sent" : "Bad parameter: cursor must be of type string", "cursor");
            }
            if (parameters.TryGetValue("per_page", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: per_page is null, which cannot be sent" : "Bad parameter: per_page must be of type Nullable<Int64>", "per_page");
            }
            if (parameters.TryGetValue("sort_by", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: sort_by is null, which cannot be sent" : "Bad parameter: sort_by must be of type object", "sort_by");
            }
            if (parameters.TryGetValue("filter", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter is null, which cannot be sent" : "Bad parameter: filter must be of type object", "filter");
            }

            return new FilesList<Partner>(client, $"/partners", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner ID.
        /// </summary>
        /// <param name="id">Partner ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Partner> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Partner ID.
        /// </summary>
        /// <param name="id">Partner ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Partner> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<Partner> FindCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Partner>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   ai_assistant_personality_id - int64 - AI Assistant Personality ID assigned to this Partner, if any. Users in the Partner inherit it unless a direct per-user assignment overrides it.
        ///   allowed_ips - string - A list of allowed IPs for this Partner. Newline delimited. Partner User IP access is allowed when the IP matches the Partner, User, or Site allowed IP lists.
        ///   allow_bypassing_2fa_policies - boolean - Allow Partner Admins to change Two-Factor Authentication requirements for Partner Users.
        ///   allow_credential_changes - boolean - Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        ///   allow_providing_gpg_keys - boolean - Allow Partner Admins to provide GPG keys.
        ///   allow_user_creation - boolean - Allow Partner Admins to create users.
        ///   cc_emails_to_responsible_party - boolean - When `true`, emails sent to Partner users are copied to the responsible User or Group.
        ///   notes - string - Notes about this Partner.
        ///   partner_channel_template_id - int64 - ID of the Partner Channel Template assigned to this Partner.
        ///   responsible_group_id - int64 - ID of the Group responsible for this Partner.
        ///   responsible_user_id - int64 - ID of the User responsible for this Partner.
        ///   show_partner_channel_home_page - boolean - Show Partner users a simplified home page built from this Partner's Channels.
        ///   tags - string - Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        ///   name (required) - string - The name of the Partner.
        ///   root_folder (required) - string - The root folder path for this Partner.
        ///   workspace_id - int64 - ID of the Workspace associated with this Partner.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Partner> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Partner> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (!parameters.TryGetValue("name", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: name", "name");
            }
            if (!parameters.TryGetValue("root_folder", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: root_folder", "root_folder");
            }
            if (parameters.TryGetValue("ai_assistant_personality_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ai_assistant_personality_id is null, which cannot be sent" : "Bad parameter: ai_assistant_personality_id must be of type Nullable<Int64>", "ai_assistant_personality_id");
            }
            if (parameters.TryGetValue("allowed_ips", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_ips is null, which cannot be sent" : "Bad parameter: allowed_ips must be of type string", "allowed_ips");
            }
            if (parameters.TryGetValue("allow_bypassing_2fa_policies", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_bypassing_2fa_policies is null, which cannot be sent" : "Bad parameter: allow_bypassing_2fa_policies must be of type bool", "allow_bypassing_2fa_policies");
            }
            if (parameters.TryGetValue("allow_credential_changes", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_credential_changes is null, which cannot be sent" : "Bad parameter: allow_credential_changes must be of type bool", "allow_credential_changes");
            }
            if (parameters.TryGetValue("allow_providing_gpg_keys", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_providing_gpg_keys is null, which cannot be sent" : "Bad parameter: allow_providing_gpg_keys must be of type bool", "allow_providing_gpg_keys");
            }
            if (parameters.TryGetValue("allow_user_creation", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_user_creation is null, which cannot be sent" : "Bad parameter: allow_user_creation must be of type bool", "allow_user_creation");
            }
            if (parameters.TryGetValue("cc_emails_to_responsible_party", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cc_emails_to_responsible_party is null, which cannot be sent" : "Bad parameter: cc_emails_to_responsible_party must be of type bool", "cc_emails_to_responsible_party");
            }
            if (parameters.TryGetValue("notes", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notes is null, which cannot be sent" : "Bad parameter: notes must be of type string", "notes");
            }
            if (parameters.TryGetValue("partner_channel_template_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_channel_template_id is null, which cannot be sent" : "Bad parameter: partner_channel_template_id must be of type Nullable<Int64>", "partner_channel_template_id");
            }
            if (parameters.TryGetValue("responsible_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_group_id is null, which cannot be sent" : "Bad parameter: responsible_group_id must be of type Nullable<Int64>", "responsible_group_id");
            }
            if (parameters.TryGetValue("responsible_user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_user_id is null, which cannot be sent" : "Bad parameter: responsible_user_id must be of type Nullable<Int64>", "responsible_user_id");
            }
            if (parameters.TryGetValue("show_partner_channel_home_page", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: show_partner_channel_home_page is null, which cannot be sent" : "Bad parameter: show_partner_channel_home_page must be of type bool", "show_partner_channel_home_page");
            }
            if (parameters.TryGetValue("tags", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: tags is null, which cannot be sent" : "Bad parameter: tags must be of type string", "tags");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("root_folder", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: root_folder is null, which cannot be sent" : "Bad parameter: root_folder must be of type string", "root_folder");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/partners", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Partner>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   ai_assistant_personality_id - int64 - AI Assistant Personality ID assigned to this Partner, if any. Users in the Partner inherit it unless a direct per-user assignment overrides it.
        ///   allowed_ips - string - A list of allowed IPs for this Partner. Newline delimited. Partner User IP access is allowed when the IP matches the Partner, User, or Site allowed IP lists.
        ///   allow_bypassing_2fa_policies - boolean - Allow Partner Admins to change Two-Factor Authentication requirements for Partner Users.
        ///   allow_credential_changes - boolean - Allow Partner Admins to change or reset credentials for users belonging to this Partner.
        ///   allow_providing_gpg_keys - boolean - Allow Partner Admins to provide GPG keys.
        ///   allow_user_creation - boolean - Allow Partner Admins to create users.
        ///   cc_emails_to_responsible_party - boolean - When `true`, emails sent to Partner users are copied to the responsible User or Group.
        ///   notes - string - Notes about this Partner.
        ///   partner_channel_template_id - int64 - ID of the Partner Channel Template assigned to this Partner.
        ///   responsible_group_id - int64 - ID of the Group responsible for this Partner.
        ///   responsible_user_id - int64 - ID of the User responsible for this Partner.
        ///   show_partner_channel_home_page - boolean - Show Partner users a simplified home page built from this Partner's Channels.
        ///   tags - string - Comma-separated list of Tags for this Partner. Tags are used for other features, such as UserLifecycleRules, which can target specific tags.  Tags must only contain lowercase letters, numbers, and hyphens.
        ///   name - string - The name of the Partner.
        ///   partnership_role - string - This site's role for this Partner in Connected Sites relationships. `host` is a Partner this site configured. `guest` is a Partner created by approving another site's connection request; it has no root folder and cannot hold users, permissions, or Partner Channels, or host a connection. `host_and_guest` is a configured Partner that is also the guest side of a connection. Promote a `guest` Partner by setting this to `host_and_guest` together with a `root_folder`.
        ///   root_folder - string - The root folder path for this Partner.
        /// </summary>
        /// <param name="id">Partner ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Partner> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<Partner> UpdateCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("ai_assistant_personality_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: ai_assistant_personality_id is null, which cannot be sent" : "Bad parameter: ai_assistant_personality_id must be of type Nullable<Int64>", "ai_assistant_personality_id");
            }
            if (parameters.TryGetValue("allowed_ips", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allowed_ips is null, which cannot be sent" : "Bad parameter: allowed_ips must be of type string", "allowed_ips");
            }
            if (parameters.TryGetValue("allow_bypassing_2fa_policies", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_bypassing_2fa_policies is null, which cannot be sent" : "Bad parameter: allow_bypassing_2fa_policies must be of type bool", "allow_bypassing_2fa_policies");
            }
            if (parameters.TryGetValue("allow_credential_changes", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_credential_changes is null, which cannot be sent" : "Bad parameter: allow_credential_changes must be of type bool", "allow_credential_changes");
            }
            if (parameters.TryGetValue("allow_providing_gpg_keys", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_providing_gpg_keys is null, which cannot be sent" : "Bad parameter: allow_providing_gpg_keys must be of type bool", "allow_providing_gpg_keys");
            }
            if (parameters.TryGetValue("allow_user_creation", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: allow_user_creation is null, which cannot be sent" : "Bad parameter: allow_user_creation must be of type bool", "allow_user_creation");
            }
            if (parameters.TryGetValue("cc_emails_to_responsible_party", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: cc_emails_to_responsible_party is null, which cannot be sent" : "Bad parameter: cc_emails_to_responsible_party must be of type bool", "cc_emails_to_responsible_party");
            }
            if (parameters.TryGetValue("notes", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notes is null, which cannot be sent" : "Bad parameter: notes must be of type string", "notes");
            }
            if (parameters.TryGetValue("partner_channel_template_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partner_channel_template_id is null, which cannot be sent" : "Bad parameter: partner_channel_template_id must be of type Nullable<Int64>", "partner_channel_template_id");
            }
            if (parameters.TryGetValue("responsible_group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_group_id is null, which cannot be sent" : "Bad parameter: responsible_group_id must be of type Nullable<Int64>", "responsible_group_id");
            }
            if (parameters.TryGetValue("responsible_user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: responsible_user_id is null, which cannot be sent" : "Bad parameter: responsible_user_id must be of type Nullable<Int64>", "responsible_user_id");
            }
            if (parameters.TryGetValue("show_partner_channel_home_page", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: show_partner_channel_home_page is null, which cannot be sent" : "Bad parameter: show_partner_channel_home_page must be of type bool", "show_partner_channel_home_page");
            }
            if (parameters.TryGetValue("tags", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: tags is null, which cannot be sent" : "Bad parameter: tags must be of type string", "tags");
            }
            if (parameters.TryGetValue("name", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: name is null, which cannot be sent" : "Bad parameter: name must be of type string", "name");
            }
            if (parameters.TryGetValue("partnership_role", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: partnership_role is null, which cannot be sent" : "Bad parameter: partnership_role must be of type string", "partnership_role");
            }
            if (parameters.TryGetValue("root_folder", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: root_folder is null, which cannot be sent" : "Bad parameter: root_folder must be of type string", "root_folder");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Partner>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /partners/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Partner ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return DeleteCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /partners/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Partner ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Delete(id, parameters, options);
        }

        internal static async Task DeleteCore(
            OperationContext context,
            Nullable<Int64> id,
            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }

            await FilesClient.SendRequestAndDiscard(context, $"/partners/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}