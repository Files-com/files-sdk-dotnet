namespace FilesCom
{
    // The client that one logical operation runs with, and that client's settings as they were when the operation
    // started. Every request of the operation, including retries, later pages and the stages of a transfer, uses
    // these same settings, so the operation never mixes endpoints or credentials, even if the configuration of a
    // client made with the FilesClient constructor changes while it runs. The models it returns belong to Client.
    internal sealed class OperationContext
    {
        internal OperationContext(FilesClient client)
        {
            Client = client;
            BaseUrl = client.BaseUrl;
            ApiKey = client.ApiKey;
            SessionId = client.SessionId;
            WorkspaceId = client.WorkspaceId;
            Language = client.Language;
            ReadTimeout = client.ReadTimeout;
        }

        internal FilesClient Client { get; }

        internal string BaseUrl { get; }

        internal string ApiKey { get; }

        internal string SessionId { get; }

        internal string WorkspaceId { get; }

        internal string Language { get; }

        internal int ReadTimeout { get; }

        // The context of the default client, or null when no client has been constructed yet. Operations validate
        // their arguments before sending, so a null context fails only when the request would be sent.
        internal static OperationContext OfDefaultClient()
        {
            FilesClient client = FilesClient.Instance;
            return client == null ? null : new OperationContext(client);
        }
    }
}