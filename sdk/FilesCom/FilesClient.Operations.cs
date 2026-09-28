using FilesCom.Operations;

namespace FilesCom
{
    public partial class FilesClient
    {
        /// <summary>
        /// ActionLog operations that run with this client.
        /// </summary>
        public ActionLogOperations ActionLogs => new ActionLogOperations(this);

        /// <summary>
        /// ActionNotificationExport operations that run with this client.
        /// </summary>
        public ActionNotificationExportOperations ActionNotificationExports => new ActionNotificationExportOperations(this);

        /// <summary>
        /// ActionNotificationExportResult operations that run with this client.
        /// </summary>
        public ActionNotificationExportResultOperations ActionNotificationExportResults => new ActionNotificationExportResultOperations(this);

        /// <summary>
        /// AiAssistantPersonality operations that run with this client.
        /// </summary>
        public AiAssistantPersonalityOperations AiAssistantPersonalities => new AiAssistantPersonalityOperations(this);

        /// <summary>
        /// AiTask operations that run with this client.
        /// </summary>
        public AiTaskOperations AiTasks => new AiTaskOperations(this);

        /// <summary>
        /// ApiKey operations that run with this client.
        /// </summary>
        public ApiKeyOperations ApiKeys => new ApiKeyOperations(this);

        /// <summary>
        /// ApiRequestLog operations that run with this client.
        /// </summary>
        public ApiRequestLogOperations ApiRequestLogs => new ApiRequestLogOperations(this);

        /// <summary>
        /// App operations that run with this client.
        /// </summary>
        public AppOperations Apps => new AppOperations(this);

        /// <summary>
        /// As2IncomingMessage operations that run with this client.
        /// </summary>
        public As2IncomingMessageOperations As2IncomingMessages => new As2IncomingMessageOperations(this);

        /// <summary>
        /// As2OutgoingMessage operations that run with this client.
        /// </summary>
        public As2OutgoingMessageOperations As2OutgoingMessages => new As2OutgoingMessageOperations(this);

        /// <summary>
        /// As2Partner operations that run with this client.
        /// </summary>
        public As2PartnerOperations As2Partners => new As2PartnerOperations(this);

        /// <summary>
        /// As2Station operations that run with this client.
        /// </summary>
        public As2StationOperations As2Stations => new As2StationOperations(this);

        /// <summary>
        /// Automation operations that run with this client.
        /// </summary>
        public AutomationOperations Automations => new AutomationOperations(this);

        /// <summary>
        /// AutomationLog operations that run with this client.
        /// </summary>
        public AutomationLogOperations AutomationLogs => new AutomationLogOperations(this);

        /// <summary>
        /// AutomationRun operations that run with this client.
        /// </summary>
        public AutomationRunOperations AutomationRuns => new AutomationRunOperations(this);

        /// <summary>
        /// BandwidthSnapshot operations that run with this client.
        /// </summary>
        public BandwidthSnapshotOperations BandwidthSnapshots => new BandwidthSnapshotOperations(this);

        /// <summary>
        /// Behavior operations that run with this client.
        /// </summary>
        public BehaviorOperations Behaviors => new BehaviorOperations(this);

        /// <summary>
        /// Bundle operations that run with this client.
        /// </summary>
        public BundleOperations Bundles => new BundleOperations(this);

        /// <summary>
        /// BundleAction operations that run with this client.
        /// </summary>
        public BundleActionOperations BundleActions => new BundleActionOperations(this);

        /// <summary>
        /// BundleDownload operations that run with this client.
        /// </summary>
        public BundleDownloadOperations BundleDownloads => new BundleDownloadOperations(this);

        /// <summary>
        /// BundleNotification operations that run with this client.
        /// </summary>
        public BundleNotificationOperations BundleNotifications => new BundleNotificationOperations(this);

        /// <summary>
        /// BundleRecipient operations that run with this client.
        /// </summary>
        public BundleRecipientOperations BundleRecipients => new BundleRecipientOperations(this);

        /// <summary>
        /// BundleRegistration operations that run with this client.
        /// </summary>
        public BundleRegistrationOperations BundleRegistrations => new BundleRegistrationOperations(this);

        /// <summary>
        /// ChatSession operations that run with this client.
        /// </summary>
        public ChatSessionOperations ChatSessions => new ChatSessionOperations(this);

        /// <summary>
        /// ChildSiteManagementPolicy operations that run with this client.
        /// </summary>
        public ChildSiteManagementPolicyOperations ChildSiteManagementPolicies => new ChildSiteManagementPolicyOperations(this);

        /// <summary>
        /// Clickwrap operations that run with this client.
        /// </summary>
        public ClickwrapOperations Clickwraps => new ClickwrapOperations(this);

        /// <summary>
        /// CustomDomain operations that run with this client.
        /// </summary>
        public CustomDomainOperations CustomDomains => new CustomDomainOperations(this);

        /// <summary>
        /// DesktopConfigurationProfile operations that run with this client.
        /// </summary>
        public DesktopConfigurationProfileOperations DesktopConfigurationProfiles => new DesktopConfigurationProfileOperations(this);

        /// <summary>
        /// DnsRecord operations that run with this client.
        /// </summary>
        public DnsRecordOperations DnsRecords => new DnsRecordOperations(this);

        /// <summary>
        /// EmailIncomingMessage operations that run with this client.
        /// </summary>
        public EmailIncomingMessageOperations EmailIncomingMessages => new EmailIncomingMessageOperations(this);

        /// <summary>
        /// EmailLog operations that run with this client.
        /// </summary>
        public EmailLogOperations EmailLogs => new EmailLogOperations(this);

        /// <summary>
        /// EventChannel operations that run with this client.
        /// </summary>
        public EventChannelOperations EventChannels => new EventChannelOperations(this);

        /// <summary>
        /// EventDeliveryAttempt operations that run with this client.
        /// </summary>
        public EventDeliveryAttemptOperations EventDeliveryAttempts => new EventDeliveryAttemptOperations(this);

        /// <summary>
        /// EventRecord operations that run with this client.
        /// </summary>
        public EventRecordOperations EventRecords => new EventRecordOperations(this);

        /// <summary>
        /// EventSubscription operations that run with this client.
        /// </summary>
        public EventSubscriptionOperations EventSubscriptions => new EventSubscriptionOperations(this);

        /// <summary>
        /// EventTarget operations that run with this client.
        /// </summary>
        public EventTargetOperations EventTargets => new EventTargetOperations(this);

        /// <summary>
        /// ExavaultApiRequestLog operations that run with this client.
        /// </summary>
        public ExavaultApiRequestLogOperations ExavaultApiRequestLogs => new ExavaultApiRequestLogOperations(this);

        /// <summary>
        /// Expectation operations that run with this client.
        /// </summary>
        public ExpectationOperations Expectations => new ExpectationOperations(this);

        /// <summary>
        /// ExpectationEvaluation operations that run with this client.
        /// </summary>
        public ExpectationEvaluationOperations ExpectationEvaluations => new ExpectationEvaluationOperations(this);

        /// <summary>
        /// ExpectationIncident operations that run with this client.
        /// </summary>
        public ExpectationIncidentOperations ExpectationIncidents => new ExpectationIncidentOperations(this);

        /// <summary>
        /// ExternalEvent operations that run with this client.
        /// </summary>
        public ExternalEventOperations ExternalEvents => new ExternalEventOperations(this);

        /// <summary>
        /// RemoteFile operations that run with this client.
        /// </summary>
        public RemoteFileOperations RemoteFiles => new RemoteFileOperations(this);

        /// <summary>
        /// FileComment operations that run with this client.
        /// </summary>
        public FileCommentOperations FileComments => new FileCommentOperations(this);

        /// <summary>
        /// FileCommentReaction operations that run with this client.
        /// </summary>
        public FileCommentReactionOperations FileCommentReactions => new FileCommentReactionOperations(this);

        /// <summary>
        /// FileMigration operations that run with this client.
        /// </summary>
        public FileMigrationOperations FileMigrations => new FileMigrationOperations(this);

        /// <summary>
        /// FileMigrationLog operations that run with this client.
        /// </summary>
        public FileMigrationLogOperations FileMigrationLogs => new FileMigrationLogOperations(this);

        /// <summary>
        /// Folder operations that run with this client.
        /// </summary>
        public FolderOperations Folders => new FolderOperations(this);

        /// <summary>
        /// FormFieldSet operations that run with this client.
        /// </summary>
        public FormFieldSetOperations FormFieldSets => new FormFieldSetOperations(this);

        /// <summary>
        /// FtpActionLog operations that run with this client.
        /// </summary>
        public FtpActionLogOperations FtpActionLogs => new FtpActionLogOperations(this);

        /// <summary>
        /// GpgKey operations that run with this client.
        /// </summary>
        public GpgKeyOperations GpgKeys => new GpgKeyOperations(this);

        /// <summary>
        /// Group operations that run with this client.
        /// </summary>
        public GroupOperations Groups => new GroupOperations(this);

        /// <summary>
        /// GroupUser operations that run with this client.
        /// </summary>
        public GroupUserOperations GroupUsers => new GroupUserOperations(this);

        /// <summary>
        /// History operations that run with this client.
        /// </summary>
        public HistoryOperations Histories => new HistoryOperations(this);

        /// <summary>
        /// HistoryExport operations that run with this client.
        /// </summary>
        public HistoryExportOperations HistoryExports => new HistoryExportOperations(this);

        /// <summary>
        /// HistoryExportResult operations that run with this client.
        /// </summary>
        public HistoryExportResultOperations HistoryExportResults => new HistoryExportResultOperations(this);

        /// <summary>
        /// HolidayCalendar operations that run with this client.
        /// </summary>
        public HolidayCalendarOperations HolidayCalendars => new HolidayCalendarOperations(this);

        /// <summary>
        /// HolidayRegion operations that run with this client.
        /// </summary>
        public HolidayRegionOperations HolidayRegions => new HolidayRegionOperations(this);

        /// <summary>
        /// InboundS3Log operations that run with this client.
        /// </summary>
        public InboundS3LogOperations InboundS3Logs => new InboundS3LogOperations(this);

        /// <summary>
        /// InboxRecipient operations that run with this client.
        /// </summary>
        public InboxRecipientOperations InboxRecipients => new InboxRecipientOperations(this);

        /// <summary>
        /// InboxRegistration operations that run with this client.
        /// </summary>
        public InboxRegistrationOperations InboxRegistrations => new InboxRegistrationOperations(this);

        /// <summary>
        /// InboxUpload operations that run with this client.
        /// </summary>
        public InboxUploadOperations InboxUploads => new InboxUploadOperations(this);

        /// <summary>
        /// IntegrationCentricProfile operations that run with this client.
        /// </summary>
        public IntegrationCentricProfileOperations IntegrationCentricProfiles => new IntegrationCentricProfileOperations(this);

        /// <summary>
        /// Invoice operations that run with this client.
        /// </summary>
        public InvoiceOperations Invoices => new InvoiceOperations(this);

        /// <summary>
        /// IpAddress operations that run with this client.
        /// </summary>
        public IpAddressOperations IpAddresses => new IpAddressOperations(this);

        /// <summary>
        /// KeyLifecycleRule operations that run with this client.
        /// </summary>
        public KeyLifecycleRuleOperations KeyLifecycleRules => new KeyLifecycleRuleOperations(this);

        /// <summary>
        /// Lock operations that run with this client.
        /// </summary>
        public LockOperations Locks => new LockOperations(this);

        /// <summary>
        /// MetadataCategory operations that run with this client.
        /// </summary>
        public MetadataCategoryOperations MetadataCategories => new MetadataCategoryOperations(this);

        /// <summary>
        /// Notification operations that run with this client.
        /// </summary>
        public NotificationOperations Notifications => new NotificationOperations(this);

        /// <summary>
        /// OutboundConnectionLog operations that run with this client.
        /// </summary>
        public OutboundConnectionLogOperations OutboundConnectionLogs => new OutboundConnectionLogOperations(this);

        /// <summary>
        /// Partner operations that run with this client.
        /// </summary>
        public PartnerOperations Partners => new PartnerOperations(this);

        /// <summary>
        /// PartnerChannel operations that run with this client.
        /// </summary>
        public PartnerChannelOperations PartnerChannels => new PartnerChannelOperations(this);

        /// <summary>
        /// PartnerChannelTemplate operations that run with this client.
        /// </summary>
        public PartnerChannelTemplateOperations PartnerChannelTemplates => new PartnerChannelTemplateOperations(this);

        /// <summary>
        /// PartnerSite operations that run with this client.
        /// </summary>
        public PartnerSiteOperations PartnerSites => new PartnerSiteOperations(this);

        /// <summary>
        /// PartnerSiteRequest operations that run with this client.
        /// </summary>
        public PartnerSiteRequestOperations PartnerSiteRequests => new PartnerSiteRequestOperations(this);

        /// <summary>
        /// Payment operations that run with this client.
        /// </summary>
        public PaymentOperations Payments => new PaymentOperations(this);

        /// <summary>
        /// PendingWorkEvent operations that run with this client.
        /// </summary>
        public PendingWorkEventOperations PendingWorkEvents => new PendingWorkEventOperations(this);

        /// <summary>
        /// Permission operations that run with this client.
        /// </summary>
        public PermissionOperations Permissions => new PermissionOperations(this);

        /// <summary>
        /// PublicHostingRequestLog operations that run with this client.
        /// </summary>
        public PublicHostingRequestLogOperations PublicHostingRequestLogs => new PublicHostingRequestLogOperations(this);

        /// <summary>
        /// PublicKey operations that run with this client.
        /// </summary>
        public PublicKeyOperations PublicKeys => new PublicKeyOperations(this);

        /// <summary>
        /// RemoteBandwidthSnapshot operations that run with this client.
        /// </summary>
        public RemoteBandwidthSnapshotOperations RemoteBandwidthSnapshots => new RemoteBandwidthSnapshotOperations(this);

        /// <summary>
        /// RemoteMountBackend operations that run with this client.
        /// </summary>
        public RemoteMountBackendOperations RemoteMountBackends => new RemoteMountBackendOperations(this);

        /// <summary>
        /// RemoteServer operations that run with this client.
        /// </summary>
        public RemoteServerOperations RemoteServers => new RemoteServerOperations(this);

        /// <summary>
        /// RemoteServerCredential operations that run with this client.
        /// </summary>
        public RemoteServerCredentialOperations RemoteServerCredentials => new RemoteServerCredentialOperations(this);

        /// <summary>
        /// Request operations that run with this client.
        /// </summary>
        public RequestOperations Requests => new RequestOperations(this);

        /// <summary>
        /// Restore operations that run with this client.
        /// </summary>
        public RestoreOperations Restores => new RestoreOperations(this);

        /// <summary>
        /// Schedule operations that run with this client.
        /// </summary>
        public ScheduleOperations Schedules => new ScheduleOperations(this);

        /// <summary>
        /// ScheduledExport operations that run with this client.
        /// </summary>
        public ScheduledExportOperations ScheduledExports => new ScheduledExportOperations(this);

        /// <summary>
        /// ScimLog operations that run with this client.
        /// </summary>
        public ScimLogOperations ScimLogs => new ScimLogOperations(this);

        /// <summary>
        /// Secret operations that run with this client.
        /// </summary>
        public SecretOperations Secrets => new SecretOperations(this);

        /// <summary>
        /// Session operations that run with this client.
        /// </summary>
        public SessionOperations Sessions => new SessionOperations(this);

        /// <summary>
        /// SettingsChange operations that run with this client.
        /// </summary>
        public SettingsChangeOperations SettingsChanges => new SettingsChangeOperations(this);

        /// <summary>
        /// SftpActionLog operations that run with this client.
        /// </summary>
        public SftpActionLogOperations SftpActionLogs => new SftpActionLogOperations(this);

        /// <summary>
        /// SftpHostKey operations that run with this client.
        /// </summary>
        public SftpHostKeyOperations SftpHostKeys => new SftpHostKeyOperations(this);

        /// <summary>
        /// ShareGroup operations that run with this client.
        /// </summary>
        public ShareGroupOperations ShareGroups => new ShareGroupOperations(this);

        /// <summary>
        /// SiemHttpDestination operations that run with this client.
        /// </summary>
        public SiemHttpDestinationOperations SiemHttpDestinations => new SiemHttpDestinationOperations(this);

        /// <summary>
        /// SiemHttpDestinationEvent operations that run with this client.
        /// </summary>
        public SiemHttpDestinationEventOperations SiemHttpDestinationEvents => new SiemHttpDestinationEventOperations(this);

        /// <summary>
        /// Site operations that run with this client.
        /// </summary>
        public SiteOperations Sites => new SiteOperations(this);

        /// <summary>
        /// SiteSubdomainRedirect operations that run with this client.
        /// </summary>
        public SiteSubdomainRedirectOperations SiteSubdomainRedirects => new SiteSubdomainRedirectOperations(this);

        /// <summary>
        /// Snapshot operations that run with this client.
        /// </summary>
        public SnapshotOperations Snapshots => new SnapshotOperations(this);

        /// <summary>
        /// SsoEvent operations that run with this client.
        /// </summary>
        public SsoEventOperations SsoEvents => new SsoEventOperations(this);

        /// <summary>
        /// SsoStrategy operations that run with this client.
        /// </summary>
        public SsoStrategyOperations SsoStrategies => new SsoStrategyOperations(this);

        /// <summary>
        /// Style operations that run with this client.
        /// </summary>
        public StyleOperations Styles => new StyleOperations(this);

        /// <summary>
        /// Sync operations that run with this client.
        /// </summary>
        public SyncOperations Syncs => new SyncOperations(this);

        /// <summary>
        /// SyncLog operations that run with this client.
        /// </summary>
        public SyncLogOperations SyncLogs => new SyncLogOperations(this);

        /// <summary>
        /// SyncRun operations that run with this client.
        /// </summary>
        public SyncRunOperations SyncRuns => new SyncRunOperations(this);

        /// <summary>
        /// UsageDailySnapshot operations that run with this client.
        /// </summary>
        public UsageDailySnapshotOperations UsageDailySnapshots => new UsageDailySnapshotOperations(this);

        /// <summary>
        /// UsageSnapshot operations that run with this client.
        /// </summary>
        public UsageSnapshotOperations UsageSnapshots => new UsageSnapshotOperations(this);

        /// <summary>
        /// User operations that run with this client.
        /// </summary>
        public UserOperations Users => new UserOperations(this);

        /// <summary>
        /// UserAdditionalEmailRecipient operations that run with this client.
        /// </summary>
        public UserAdditionalEmailRecipientOperations UserAdditionalEmailRecipients => new UserAdditionalEmailRecipientOperations(this);

        /// <summary>
        /// UserCipherUse operations that run with this client.
        /// </summary>
        public UserCipherUseOperations UserCipherUses => new UserCipherUseOperations(this);

        /// <summary>
        /// UserLifecycleRule operations that run with this client.
        /// </summary>
        public UserLifecycleRuleOperations UserLifecycleRules => new UserLifecycleRuleOperations(this);

        /// <summary>
        /// UserRequest operations that run with this client.
        /// </summary>
        public UserRequestOperations UserRequests => new UserRequestOperations(this);

        /// <summary>
        /// UserSecurityEvent operations that run with this client.
        /// </summary>
        public UserSecurityEventOperations UserSecurityEvents => new UserSecurityEventOperations(this);

        /// <summary>
        /// UserSftpClientUse operations that run with this client.
        /// </summary>
        public UserSftpClientUseOperations UserSftpClientUses => new UserSftpClientUseOperations(this);

        /// <summary>
        /// WebDavActionLog operations that run with this client.
        /// </summary>
        public WebDavActionLogOperations WebDavActionLogs => new WebDavActionLogOperations(this);

        /// <summary>
        /// WebhookTest operations that run with this client.
        /// </summary>
        public WebhookTestOperations WebhookTests => new WebhookTestOperations(this);

        /// <summary>
        /// Workspace operations that run with this client.
        /// </summary>
        public WorkspaceOperations Workspaces => new WorkspaceOperations(this);
    }
}