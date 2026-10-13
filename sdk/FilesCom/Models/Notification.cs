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
    /// NotificationEntity model
    /// </summary>
    public class Notification : IModel, IJsonOnDeserialized
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
        // The JSON, when handed out since then, of the arrays among them that callers can change in place, directly or
        // through a model in them. SaveAsync compares each with the array's JSON now, and sends the ones that differ.
        private readonly Dictionary<string, string> handedOut = new Dictionary<string, string>();
        private static readonly string[] ArrayAttributeNames = { "group_ids", "triggering_group_ids", "triggering_user_ids", "triggering_filenames" };
        /// <summary>
        /// Makes a Notification with no attributes set and no options.
        /// </summary>
        public Notification() : this(null, null) { }

        /// <summary>
        /// Makes a Notification with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public Notification(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
            unsaved.UnionWith(this.attributes.Keys);

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("path"))
            {
                this.attributes.Add("path", null);
            }
            if (!this.attributes.ContainsKey("group_id"))
            {
                this.attributes.Add("group_id", null);
            }
            if (!this.attributes.ContainsKey("group_name"))
            {
                this.attributes.Add("group_name", null);
            }
            if (!this.attributes.ContainsKey("group_ids"))
            {
                this.attributes.Add("group_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("group_names"))
            {
                this.attributes.Add("group_names", new string[0]);
            }
            if (!this.attributes.ContainsKey("triggering_group_ids"))
            {
                this.attributes.Add("triggering_group_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("triggering_user_ids"))
            {
                this.attributes.Add("triggering_user_ids", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("trigger_by_share_recipients"))
            {
                this.attributes.Add("trigger_by_share_recipients", false);
            }
            if (!this.attributes.ContainsKey("notify_user_actions"))
            {
                this.attributes.Add("notify_user_actions", false);
            }
            if (!this.attributes.ContainsKey("notify_on_copy"))
            {
                this.attributes.Add("notify_on_copy", false);
            }
            if (!this.attributes.ContainsKey("notify_on_delete"))
            {
                this.attributes.Add("notify_on_delete", false);
            }
            if (!this.attributes.ContainsKey("notify_on_download"))
            {
                this.attributes.Add("notify_on_download", false);
            }
            if (!this.attributes.ContainsKey("notify_on_move"))
            {
                this.attributes.Add("notify_on_move", false);
            }
            if (!this.attributes.ContainsKey("notify_on_upload"))
            {
                this.attributes.Add("notify_on_upload", false);
            }
            if (!this.attributes.ContainsKey("recursive"))
            {
                this.attributes.Add("recursive", false);
            }
            if (!this.attributes.ContainsKey("send_interval"))
            {
                this.attributes.Add("send_interval", null);
            }
            if (!this.attributes.ContainsKey("subject"))
            {
                this.attributes.Add("subject", null);
            }
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("triggering_filenames"))
            {
                this.attributes.Add("triggering_filenames", new string[0]);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("unsubscribed"))
            {
                this.attributes.Add("unsubscribed", false);
            }
            if (!this.attributes.ContainsKey("unsubscribed_reason"))
            {
                this.attributes.Add("unsubscribed_reason", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("username"))
            {
                this.attributes.Add("username", null);
            }
            if (!this.attributes.ContainsKey("suppressed_email"))
            {
                this.attributes.Add("suppressed_email", false);
            }
        }

        /// <summary>
        /// A copy of this object's attributes, keyed by their API names. Their values, such as arrays, are this
        /// object's own, not copies.
        /// </summary>
        public Dictionary<string, object> getAttributes()
        {
            // The copy holds this object's own arrays, which the caller can change in place as through its properties.
            foreach (string name in ArrayAttributeNames)
            {
                HandedOut(name);
            }
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

        // What is read from JSON, such as a response from Files.com, is not waiting to be saved.
        void IJsonOnDeserialized.OnDeserialized()
        {
            unsaved.Clear();
            handedOut.Clear();
        }


        /// <summary>
        /// Notification ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; unsaved.Add("id"); }
        }

        /// <summary>
        /// Folder path to notify on. This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; unsaved.Add("path"); }
        }

        /// <summary>
        /// ID of Group to receive notifications
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>)attributes["group_id"]; }
            set { attributes["group_id"] = value; unsaved.Add("group_id"); }
        }

        /// <summary>
        /// Group name, if a Group ID is set
        /// </summary>
        [JsonPropertyName("group_name")]
        public string GroupName
        {
            get { return (string)attributes["group_name"]; }
            set { attributes["group_name"] = value; unsaved.Add("group_name"); }
        }

        /// <summary>
        /// Group IDs when the notification requires multiple groups
        /// </summary>
        [JsonPropertyName("group_ids")]
        public Nullable<Int64>[] GroupIds
        {
            get { return (Nullable<Int64>[])HandedOut("group_ids"); }
            set { attributes["group_ids"] = value; unsaved.Add("group_ids"); }
        }

        /// <summary>
        /// Group names when the notification requires multiple groups
        /// </summary>
        [JsonPropertyName("group_names")]
        public string[] GroupNames
        {
            get { return (string[])attributes["group_names"]; }
            set { attributes["group_names"] = value; unsaved.Add("group_names"); }
        }

        /// <summary>
        /// If set, will only notify on actions made by a member of one of the specified groups
        /// </summary>
        [JsonPropertyName("triggering_group_ids")]
        public Nullable<Int64>[] TriggeringGroupIds
        {
            get { return (Nullable<Int64>[])HandedOut("triggering_group_ids"); }
            set { attributes["triggering_group_ids"] = value; unsaved.Add("triggering_group_ids"); }
        }

        /// <summary>
        /// If set, will only notify on actions made one of the specified users
        /// </summary>
        [JsonPropertyName("triggering_user_ids")]
        public Nullable<Int64>[] TriggeringUserIds
        {
            get { return (Nullable<Int64>[])HandedOut("triggering_user_ids"); }
            set { attributes["triggering_user_ids"] = value; unsaved.Add("triggering_user_ids"); }
        }

        /// <summary>
        /// Notify when actions are performed by a share recipient?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("trigger_by_share_recipients")]
        public bool TriggerByShareRecipients
        {
            get { return attributes["trigger_by_share_recipients"] == null ? false : (bool)attributes["trigger_by_share_recipients"]; }
            set { attributes["trigger_by_share_recipients"] = value; unsaved.Add("trigger_by_share_recipients"); }
        }

        /// <summary>
        /// If true, will send notifications about a user's own activity to that user.  If false, only activity performed by other users (or anonymous users) will be sent in notifications.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_user_actions")]
        public bool NotifyUserActions
        {
            get { return attributes["notify_user_actions"] == null ? false : (bool)attributes["notify_user_actions"]; }
            set { attributes["notify_user_actions"] = value; unsaved.Add("notify_user_actions"); }
        }

        /// <summary>
        /// Trigger on files copied to this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_copy")]
        public bool NotifyOnCopy
        {
            get { return attributes["notify_on_copy"] == null ? false : (bool)attributes["notify_on_copy"]; }
            set { attributes["notify_on_copy"] = value; unsaved.Add("notify_on_copy"); }
        }

        /// <summary>
        /// Trigger on files deleted in this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_delete")]
        public bool NotifyOnDelete
        {
            get { return attributes["notify_on_delete"] == null ? false : (bool)attributes["notify_on_delete"]; }
            set { attributes["notify_on_delete"] = value; unsaved.Add("notify_on_delete"); }
        }

        /// <summary>
        /// Trigger on files downloaded in this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_download")]
        public bool NotifyOnDownload
        {
            get { return attributes["notify_on_download"] == null ? false : (bool)attributes["notify_on_download"]; }
            set { attributes["notify_on_download"] = value; unsaved.Add("notify_on_download"); }
        }

        /// <summary>
        /// Trigger on files moved to this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_move")]
        public bool NotifyOnMove
        {
            get { return attributes["notify_on_move"] == null ? false : (bool)attributes["notify_on_move"]; }
            set { attributes["notify_on_move"] = value; unsaved.Add("notify_on_move"); }
        }

        /// <summary>
        /// Trigger on files created/uploaded/updated/changed in this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_upload")]
        public bool NotifyOnUpload
        {
            get { return attributes["notify_on_upload"] == null ? false : (bool)attributes["notify_on_upload"]; }
            set { attributes["notify_on_upload"] = value; unsaved.Add("notify_on_upload"); }
        }

        /// <summary>
        /// Apply notification recursively?  This will enable notifications for each subfolder.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("recursive")]
        public bool Recursive
        {
            get { return attributes["recursive"] == null ? false : (bool)attributes["recursive"]; }
            set { attributes["recursive"] = value; unsaved.Add("recursive"); }
        }

        /// <summary>
        /// The time interval that notifications are aggregated to
        /// </summary>
        [JsonPropertyName("send_interval")]
        public string SendInterval
        {
            get { return (string)attributes["send_interval"]; }
            set { attributes["send_interval"] = value; unsaved.Add("send_interval"); }
        }

        /// <summary>
        /// Custom subject line to use for notification emails
        /// </summary>
        [JsonPropertyName("subject")]
        public string Subject
        {
            get { return (string)attributes["subject"]; }
            set { attributes["subject"] = value; unsaved.Add("subject"); }
        }

        /// <summary>
        /// Custom message to include in notification emails
        /// </summary>
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            set { attributes["message"] = value; unsaved.Add("message"); }
        }

        /// <summary>
        /// Array of filenames (possibly with wildcards) to scope trigger
        /// </summary>
        [JsonPropertyName("triggering_filenames")]
        public string[] TriggeringFilenames
        {
            get { return (string[])HandedOut("triggering_filenames"); }
            set { attributes["triggering_filenames"] = value; unsaved.Add("triggering_filenames"); }
        }

        /// <summary>
        /// Workspace ID. `0` means the default workspace.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; unsaved.Add("workspace_id"); }
        }

        /// <summary>
        /// Is the user unsubscribed from this notification?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("unsubscribed")]
        public bool Unsubscribed
        {
            get { return attributes["unsubscribed"] == null ? false : (bool)attributes["unsubscribed"]; }
            set { attributes["unsubscribed"] = value; unsaved.Add("unsubscribed"); }
        }

        /// <summary>
        /// The reason that the user unsubscribed
        /// </summary>
        [JsonPropertyName("unsubscribed_reason")]
        public string UnsubscribedReason
        {
            get { return (string)attributes["unsubscribed_reason"]; }
            set { attributes["unsubscribed_reason"] = value; unsaved.Add("unsubscribed_reason"); }
        }

        /// <summary>
        /// Notification user ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; unsaved.Add("user_id"); }
        }

        /// <summary>
        /// Notification username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; unsaved.Add("username"); }
        }

        /// <summary>
        /// If true, it means that the recipient at this user's email address has manually unsubscribed from all emails, or had their email "hard bounce", which means that we are unable to send mail to this user's current email address. Notifications will resume if the user changes their email address.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("suppressed_email")]
        public bool SuppressedEmail
        {
            get { return attributes["suppressed_email"] == null ? false : (bool)attributes["suppressed_email"]; }
            set { attributes["suppressed_email"] = value; unsaved.Add("suppressed_email"); }
        }

        /// <summary>
        /// Parameters:
        ///   notify_on_copy - boolean - If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
        ///   notify_on_delete - boolean - Trigger on files deleted in this path?
        ///   notify_on_download - boolean - Trigger on files downloaded in this path?
        ///   notify_on_move - boolean - Trigger on files moved to this path?
        ///   notify_on_upload - boolean - Trigger on files created/uploaded/updated/changed in this path?
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
        ///   subject - string - Custom subject line to use for notification emails
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will only notify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Notification it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Notification> Update(Dictionary<string, object> parameters)
        {
            return UpdateCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   notify_on_copy - boolean - If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
        ///   notify_on_delete - boolean - Trigger on files deleted in this path?
        ///   notify_on_download - boolean - Trigger on files downloaded in this path?
        ///   notify_on_move - boolean - Trigger on files moved to this path?
        ///   notify_on_upload - boolean - Trigger on files created/uploaded/updated/changed in this path?
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
        ///   subject - string - Custom subject line to use for notification emails
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will only notify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        /// <remarks>
        /// When it succeeds, this object takes the attributes Files.com returns. The Notification it returns is a
        /// separate object with those attributes, which keeps the options this request was sent with.
        /// </remarks>
        public Task<Notification> UpdateAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return UpdateCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<Notification> UpdateCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The Notification has no id, which Update needs", "id");
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
            if (parameters.TryGetValue("notify_on_copy", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_copy is null, which cannot be sent" : "Bad parameter: notify_on_copy must be of type bool", "notify_on_copy");
            }
            if (parameters.TryGetValue("notify_on_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_delete is null, which cannot be sent" : "Bad parameter: notify_on_delete must be of type bool", "notify_on_delete");
            }
            if (parameters.TryGetValue("notify_on_download", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_download is null, which cannot be sent" : "Bad parameter: notify_on_download must be of type bool", "notify_on_download");
            }
            if (parameters.TryGetValue("notify_on_move", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_move is null, which cannot be sent" : "Bad parameter: notify_on_move must be of type bool", "notify_on_move");
            }
            if (parameters.TryGetValue("notify_on_upload", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_upload is null, which cannot be sent" : "Bad parameter: notify_on_upload must be of type bool", "notify_on_upload");
            }
            if (parameters.TryGetValue("notify_user_actions", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_user_actions is null, which cannot be sent" : "Bad parameter: notify_user_actions must be of type bool", "notify_user_actions");
            }
            if (parameters.TryGetValue("recursive", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recursive is null, which cannot be sent" : "Bad parameter: recursive must be of type bool", "recursive");
            }
            if (parameters.TryGetValue("send_interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_interval is null, which cannot be sent" : "Bad parameter: send_interval must be of type string", "send_interval");
            }
            if (parameters.TryGetValue("subject", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: subject is null, which cannot be sent" : "Bad parameter: subject must be of type string", "subject");
            }
            if (parameters.TryGetValue("message", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: message is null, which cannot be sent" : "Bad parameter: message must be of type string", "message");
            }
            if (parameters.TryGetValue("triggering_filenames", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_filenames is null, which cannot be sent" : "Bad parameter: triggering_filenames must be of type string[]", "triggering_filenames");
            }
            if (parameters.TryGetValue("triggering_group_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_group_ids is null, which cannot be sent" : "Bad parameter: triggering_group_ids must be of type Nullable<Int64>[]", "triggering_group_ids");
            }
            if (parameters.TryGetValue("triggering_user_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_user_ids is null, which cannot be sent" : "Bad parameter: triggering_user_ids must be of type Nullable<Int64>[]", "triggering_user_ids");
            }
            if (parameters.TryGetValue("trigger_by_share_recipients", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_by_share_recipients is null, which cannot be sent" : "Bad parameter: trigger_by_share_recipients must be of type bool", "trigger_by_share_recipients");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/notifications/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, requestOptions, cancellationToken);
            Notification updated = JsonUtil.DeserializeModelResponse<Notification>(responseJson, context, requestOptions);
            // This object takes its own reading of the response, so that it shares no array or nested model with the
            // one returned, and a change to either leaves the other alone.
            Refresh(JsonUtil.DeserializeModelResponse<Notification>(responseJson, context, requestOptions).attributes);
            // Its later operations go where this one went.
            origin = context.RelocatedOrigin;
            return updated;
        }

        /// <summary>
        /// Sends DELETE /notifications/{id} to the Files.com API.
        /// </summary>
        /// <param name="parameters">The operation takes no parameters besides id, which this object supplies.</param>
        public Task Delete(Dictionary<string, object> parameters)
        {
            return DeleteCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Sends DELETE /notifications/{id} to the Files.com API.
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
                throw new MissingParameterException("The Notification has no id, which Delete needs", "id");
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
            await FilesClient.SendRequestAndDiscard(context, $"/notifications/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, requestOptions, cancellationToken);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
        }


        private static readonly string[] CreateParameterNames = { "user_id", "notify_on_copy", "notify_on_delete", "notify_on_download", "notify_on_move", "notify_on_upload", "notify_user_actions", "recursive", "send_interval", "subject", "message", "triggering_filenames", "triggering_group_ids", "triggering_user_ids", "trigger_by_share_recipients", "workspace_id", "group_id", "group_ids", "path", "username" };
        private static readonly string[] UpdateParameterNames = { "id", "notify_on_copy", "notify_on_delete", "notify_on_download", "notify_on_move", "notify_on_upload", "notify_user_actions", "recursive", "send_interval", "subject", "message", "triggering_filenames", "triggering_group_ids", "triggering_user_ids", "trigger_by_share_recipients", "workspace_id" };

        /// <summary>
        /// Same as <see cref="SaveAsync"/>, without a cancellation token.
        /// </summary>
        public Task Save()
        {
            return SaveAsync(CancellationToken.None);
        }

        /// <summary>
        /// Creates this Notification on Files.com, or updates it if it has an ID.
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
            Notification created = await Notification.CreateCore(new OperationContext(FilesClient.Bind(ref client), origin), AttributesToSave(CreateParameterNames), DictionaryUtil.Copy(this.options), cancellationToken);
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
                if (unsaved.Contains(name) || ChangedInPlace(name))
                {
                    toSave[name] = ToParameter(name, attributes[name]);
                }
            }
            return toSave;
        }

        // An array attribute, for a property or getAttributes to hand out. Its JSON is kept the first time, as the caller
        // can change the array, or an array in a model in it, without setting anything.
        private object HandedOut(string name)
        {
            object value = attributes[name];
            if (value != null && !handedOut.ContainsKey(name))
            {
                handedOut[name] = JsonSerializer.Serialize(value, JsonUtil.Options);
            }
            return value;
        }

        // Whether an array handed out has been changed in place since, at any depth.
        private bool ChangedInPlace(string name)
        {
            return handedOut.TryGetValue(name, out string handedOutJson) && JsonSerializer.Serialize(attributes[name], JsonUtil.Options) != handedOutJson;
        }

        // The value SaveAsync sends for an attribute whose property type is not the one its parameter takes.
        private static object ToParameter(string name, object value)
        {
            switch (name)
            {
                case "group_ids":
                    return ParameterValue.FromIds(value);
                default:
                    return value;
            }
        }

        // Takes the attributes Files.com returned for this object after saving or updating it, which the response's
        // reading has already checked is an object. Its client and options stay as they are.
        private void Refresh(Dictionary<string, object> saved)
        {
            attributes = saved;
            unsaved.Clear();
            handedOut.Clear();
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `path`, `user_id` or `group_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `user_id`, `workspace_id` or `group_id`. Valid field combinations are `[ workspace_id, path ]`, `[ workspace_id, user_id ]`, `[ workspace_id, group_id ]` or `[ workspace_id, user_id, path ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        ///   path - string - Show notifications for this Path.
        ///   include_ancestors - boolean - If `include_ancestors` is `true` and `path` is specified, include notifications for any parent paths. Ignored if `path` is not specified.
        ///   group_id - string
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Notification> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `path`, `user_id` or `group_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `user_id`, `workspace_id` or `group_id`. Valid field combinations are `[ workspace_id, path ]`, `[ workspace_id, user_id ]`, `[ workspace_id, group_id ]` or `[ workspace_id, user_id, path ]`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        ///   path - string - Show notifications for this Path.
        ///   include_ancestors - boolean - If `include_ancestors` is `true` and `path` is specified, include notifications for any parent paths. Ignored if `path` is not specified.
        ///   group_id - string
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<Notification> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<Notification> ListCore(
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
            if (parameters.TryGetValue("filter_prefix", out parameterValue) && !(parameterValue is object))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: filter_prefix is null, which cannot be sent" : "Bad parameter: filter_prefix must be of type object", "filter_prefix");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("include_ancestors", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: include_ancestors is null, which cannot be sent" : "Bad parameter: include_ancestors must be of type bool", "include_ancestors");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type string", "group_id");
            }

            return new FilesList<Notification>(client, $"/notifications", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Notification ID.
        /// </summary>
        /// <param name="id">Notification ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Notification> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Notification ID.
        /// </summary>
        /// <param name="id">Notification ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Notification> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<Notification> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Notification>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - The id of the user to notify. Provide `user_id`, `username` or `group_id`.
        ///   notify_on_copy - boolean - If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
        ///   notify_on_delete - boolean - Trigger on files deleted in this path?
        ///   notify_on_download - boolean - Trigger on files downloaded in this path?
        ///   notify_on_move - boolean - Trigger on files moved to this path?
        ///   notify_on_upload - boolean - Trigger on files created/uploaded/updated/changed in this path?
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
        ///   subject - string - Custom subject line to use for notification emails
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will only notify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        ///   group_id - int64 - The ID of the group to notify.  Provide `user_id`, `username` or `group_id`.
        ///   group_ids - string - Group IDs when the notification requires multiple groups. If sent as a string, it should be comma-delimited.
        ///   path - string - Path
        ///   username - string - The username of the user to notify.  Provide `user_id`, `username` or `group_id`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Notification> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return CreateCore(OperationContext.OfDefaultClient(), parameters, options, CancellationToken.None);
        }

        internal static async Task<Notification> CreateCore(
            OperationContext context,

            Dictionary<string, object> parameters,
            Dictionary<string, object> options,
            CancellationToken cancellationToken
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();
            object parameterValue;

            if (parameters.TryGetValue("user_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: user_id is null, which cannot be sent" : "Bad parameter: user_id must be of type Nullable<Int64>", "user_id");
            }
            if (parameters.TryGetValue("notify_on_copy", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_copy is null, which cannot be sent" : "Bad parameter: notify_on_copy must be of type bool", "notify_on_copy");
            }
            if (parameters.TryGetValue("notify_on_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_delete is null, which cannot be sent" : "Bad parameter: notify_on_delete must be of type bool", "notify_on_delete");
            }
            if (parameters.TryGetValue("notify_on_download", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_download is null, which cannot be sent" : "Bad parameter: notify_on_download must be of type bool", "notify_on_download");
            }
            if (parameters.TryGetValue("notify_on_move", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_move is null, which cannot be sent" : "Bad parameter: notify_on_move must be of type bool", "notify_on_move");
            }
            if (parameters.TryGetValue("notify_on_upload", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_upload is null, which cannot be sent" : "Bad parameter: notify_on_upload must be of type bool", "notify_on_upload");
            }
            if (parameters.TryGetValue("notify_user_actions", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_user_actions is null, which cannot be sent" : "Bad parameter: notify_user_actions must be of type bool", "notify_user_actions");
            }
            if (parameters.TryGetValue("recursive", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recursive is null, which cannot be sent" : "Bad parameter: recursive must be of type bool", "recursive");
            }
            if (parameters.TryGetValue("send_interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_interval is null, which cannot be sent" : "Bad parameter: send_interval must be of type string", "send_interval");
            }
            if (parameters.TryGetValue("subject", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: subject is null, which cannot be sent" : "Bad parameter: subject must be of type string", "subject");
            }
            if (parameters.TryGetValue("message", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: message is null, which cannot be sent" : "Bad parameter: message must be of type string", "message");
            }
            if (parameters.TryGetValue("triggering_filenames", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_filenames is null, which cannot be sent" : "Bad parameter: triggering_filenames must be of type string[]", "triggering_filenames");
            }
            if (parameters.TryGetValue("triggering_group_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_group_ids is null, which cannot be sent" : "Bad parameter: triggering_group_ids must be of type Nullable<Int64>[]", "triggering_group_ids");
            }
            if (parameters.TryGetValue("triggering_user_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_user_ids is null, which cannot be sent" : "Bad parameter: triggering_user_ids must be of type Nullable<Int64>[]", "triggering_user_ids");
            }
            if (parameters.TryGetValue("trigger_by_share_recipients", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_by_share_recipients is null, which cannot be sent" : "Bad parameter: trigger_by_share_recipients must be of type bool", "trigger_by_share_recipients");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }
            if (parameters.TryGetValue("group_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_id is null, which cannot be sent" : "Bad parameter: group_id must be of type Nullable<Int64>", "group_id");
            }
            if (parameters.TryGetValue("group_ids", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: group_ids is null, which cannot be sent" : "Bad parameter: group_ids must be of type string", "group_ids");
            }
            if (parameters.TryGetValue("path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: path is null, which cannot be sent" : "Bad parameter: path must be of type string", "path");
            }
            if (parameters.TryGetValue("username", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: username is null, which cannot be sent" : "Bad parameter: username must be of type string", "username");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/notifications", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Notification>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   notify_on_copy - boolean - If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
        ///   notify_on_delete - boolean - Trigger on files deleted in this path?
        ///   notify_on_download - boolean - Trigger on files downloaded in this path?
        ///   notify_on_move - boolean - Trigger on files moved to this path?
        ///   notify_on_upload - boolean - Trigger on files created/uploaded/updated/changed in this path?
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
        ///   subject - string - Custom subject line to use for notification emails
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will only notify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        ///   workspace_id - int64 - Workspace ID. `0` means the default workspace.
        /// </summary>
        /// <param name="id">Notification ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<Notification> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return UpdateCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<Notification> UpdateCore(
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
            if (parameters.TryGetValue("notify_on_copy", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_copy is null, which cannot be sent" : "Bad parameter: notify_on_copy must be of type bool", "notify_on_copy");
            }
            if (parameters.TryGetValue("notify_on_delete", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_delete is null, which cannot be sent" : "Bad parameter: notify_on_delete must be of type bool", "notify_on_delete");
            }
            if (parameters.TryGetValue("notify_on_download", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_download is null, which cannot be sent" : "Bad parameter: notify_on_download must be of type bool", "notify_on_download");
            }
            if (parameters.TryGetValue("notify_on_move", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_move is null, which cannot be sent" : "Bad parameter: notify_on_move must be of type bool", "notify_on_move");
            }
            if (parameters.TryGetValue("notify_on_upload", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_on_upload is null, which cannot be sent" : "Bad parameter: notify_on_upload must be of type bool", "notify_on_upload");
            }
            if (parameters.TryGetValue("notify_user_actions", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: notify_user_actions is null, which cannot be sent" : "Bad parameter: notify_user_actions must be of type bool", "notify_user_actions");
            }
            if (parameters.TryGetValue("recursive", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: recursive is null, which cannot be sent" : "Bad parameter: recursive must be of type bool", "recursive");
            }
            if (parameters.TryGetValue("send_interval", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: send_interval is null, which cannot be sent" : "Bad parameter: send_interval must be of type string", "send_interval");
            }
            if (parameters.TryGetValue("subject", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: subject is null, which cannot be sent" : "Bad parameter: subject must be of type string", "subject");
            }
            if (parameters.TryGetValue("message", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: message is null, which cannot be sent" : "Bad parameter: message must be of type string", "message");
            }
            if (parameters.TryGetValue("triggering_filenames", out parameterValue) && !(parameterValue is string[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_filenames is null, which cannot be sent" : "Bad parameter: triggering_filenames must be of type string[]", "triggering_filenames");
            }
            if (parameters.TryGetValue("triggering_group_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_group_ids is null, which cannot be sent" : "Bad parameter: triggering_group_ids must be of type Nullable<Int64>[]", "triggering_group_ids");
            }
            if (parameters.TryGetValue("triggering_user_ids", out parameterValue) && !(parameterValue is Nullable<Int64>[]))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: triggering_user_ids is null, which cannot be sent" : "Bad parameter: triggering_user_ids must be of type Nullable<Int64>[]", "triggering_user_ids");
            }
            if (parameters.TryGetValue("trigger_by_share_recipients", out parameterValue) && !(parameterValue is bool))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: trigger_by_share_recipients is null, which cannot be sent" : "Bad parameter: trigger_by_share_recipients must be of type bool", "trigger_by_share_recipients");
            }
            if (parameters.TryGetValue("workspace_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: workspace_id is null, which cannot be sent" : "Bad parameter: workspace_id must be of type Nullable<Int64>", "workspace_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<Notification>(responseJson, context, options);
        }

        /// <summary>
        /// Sends DELETE /notifications/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Notification ID.</param>
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
        /// Sends DELETE /notifications/{id} to the Files.com API.
        /// </summary>
        /// <param name="id">Notification ID.</param>
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

            await FilesClient.SendRequestAndDiscard(context, $"/notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options, cancellationToken);
        }

    }
}