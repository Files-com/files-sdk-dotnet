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
    /// FileActionEntity model
    /// </summary>
    public class FileAction : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;

        private static readonly TimeSpan DefaultPollInterval = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Waits until Files.com has finished the copy, move or other action this object describes, checking the
        /// <see cref="FileMigration"/> doing it every <paramref name="pollInterval"/>.
        /// </summary>
        /// <param name="pollInterval">How long to wait between checks; one second if null.</param>
        /// <param name="progress">Receives the migration each time it is checked, with its status and the files it has processed.</param>
        /// <param name="cancellationToken">
        /// Stops waiting. To limit the wait, use a token that cancels itself, for example from
        /// <see cref="CancellationTokenSource(TimeSpan)"/>. The action itself goes on at Files.com.
        /// </param>
        /// <returns>The completed migration, or null when the action had no migration: it was complete when Files.com answered.</returns>
        /// <remarks>
        /// Files.com may answer a copy or move before it is done, with <see cref="Status"/> <c>pending</c> and the
        /// <see cref="FileMigrationId"/> of the migration doing it; other actions complete before the answer. Each check is
        /// a request made with the client this action came from, with its credentials, to the address its request went
        /// to, and is retried as that client's requests are. The wait ends when the migration is completed, failed or
        /// cancelled; any other status is checked again.
        /// </remarks>
        /// <exception cref="FileMigrationFailedException">The migration failed or was cancelled, or the action itself failed.</exception>
        /// <exception cref="InvalidResponseException">The action is pending but names no migration to wait for.</exception>
        /// <exception cref="InvalidParameterException"><paramref name="pollInterval"/> is not positive, or more than <see cref="int.MaxValue"/> milliseconds.</exception>
        /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
        public async Task<FileMigration> WaitAsync(TimeSpan? pollInterval = null, IProgress<FileMigration> progress = null, CancellationToken cancellationToken = default)
        {
            TimeSpan interval = CheckedPollInterval(pollInterval);
            // One operation: every check has this action's client settings, origin and request options as they are now.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            using (OperationContext.OperationScope wait = context.Begin(cancellationToken))
            {
                try
                {
                    return await WaitCoreAsync(context, interval, progress, requestOptions, wait.Token);
                }
                finally
                {
                    // This object's later requests go where this one went.
                    origin = context.RelocatedOrigin ?? origin;
                }
            }
        }

        internal static TimeSpan CheckedPollInterval(TimeSpan? pollInterval)
        {
            TimeSpan interval = pollInterval ?? DefaultPollInterval;
            if (interval <= TimeSpan.Zero || interval.TotalMilliseconds > int.MaxValue)
            {
                throw InvalidParameterException.OutOfRange(nameof(pollInterval), interval, "The time between checks must be positive, and at most Int32.MaxValue milliseconds");
            }
            return interval;
        }

        // Waits, as part of the operation of context, for the migration this action started, checking it every interval
        // with requestOptions, the operation's own snapshot. The migration's failure message, which is the server's own
        // text and may quote URLs or credentials, is logged at DEBUG only, and kept on the exception's Migration.
        internal async Task<FileMigration> WaitCoreAsync(OperationContext context, TimeSpan interval, IProgress<FileMigration> progress, Dictionary<string, object> requestOptions, CancellationToken cancellationToken)
        {
            if (FileMigrationId == null)
            {
                if (Status == "failed")
                {
                    throw new FileMigrationFailedException(this, null);
                }
                if (Status == "pending")
                {
                    throw new InvalidResponseException("Files.com described the action as pending without naming the migration doing it, so it cannot be waited for.");
                }
                return null;
            }
            while (true)
            {
                FileMigration migration = await FileMigration.FindCore(context, FileMigrationId, null, DictionaryUtil.Copy(requestOptions), cancellationToken);
                progress?.Report(migration);
                switch (migration.Status)
                {
                    case "completed":
                        return migration;
                    case "failed":
                    case "cancelled":
                        var failed = new FileMigrationFailedException(this, migration);
                        waitLog.Debug($"File migration {migration.Id} {migration.Status}: {migration.FailureMessage}");
                        throw failed;
                }
                await Task.Delay(interval, cancellationToken);
            }
        }

        private static readonly SdkLog waitLog = SdkLog.For(typeof(FileAction));
        /// <summary>
        /// Makes a FileAction with no attributes set and no options.
        /// </summary>
        public FileAction() : this(null, null) { }

        /// <summary>
        /// Makes a FileAction with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public FileAction(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("file_migration_id"))
            {
                this.attributes.Add("file_migration_id", null);
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
            get { return new object[0]; }
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


        /// <summary>
        /// Status of file operation.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// If status is pending, this is the id of the File Migration to check for status updates.  Delete actions include it whenever the deletion runs as a File Migration, including while it is processing.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_migration_id")]
        public Nullable<Int64> FileMigrationId
        {
            get { return (Nullable<Int64>)attributes["file_migration_id"]; }
            private set { attributes["file_migration_id"] = value; }
        }



    }
}