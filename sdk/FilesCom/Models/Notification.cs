using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Notification
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Notification() : this(null, null) { }

        public Notification(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;

            if (this.attributes == null)
            {
                this.attributes = new Dictionary<string, object>();
            }

            if (this.options == null)
            {
                this.options = new Dictionary<string, object>();
            }

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
            if (!this.attributes.ContainsKey("message"))
            {
                this.attributes.Add("message", null);
            }
            if (!this.attributes.ContainsKey("triggering_filenames"))
            {
                this.attributes.Add("triggering_filenames", new string[0]);
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

        public Dictionary<string, object> getAttributes()
        {
            return new Dictionary<string, object>(this.attributes);
        }

        public object GetOption(string name)
        {
            return (this.options.ContainsKey(name) ? this.options[name] : null);
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// Notification ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Folder path to notify on This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string)attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// ID of Group to receive notifications
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>)attributes["group_id"]; }
            set { attributes["group_id"] = value; }
        }

        /// <summary>
        /// Group name, if a Group ID is set
        /// </summary>
        [JsonPropertyName("group_name")]
        public string GroupName
        {
            get { return (string)attributes["group_name"]; }
            set { attributes["group_name"] = value; }
        }

        /// <summary>
        /// If set, will only notify on actions made by a member of one of the specified groups
        /// </summary>
        [JsonPropertyName("triggering_group_ids")]
        public Nullable<Int64>[] TriggeringGroupIds
        {
            get { return (Nullable<Int64>[])attributes["triggering_group_ids"]; }
            set { attributes["triggering_group_ids"] = value; }
        }

        /// <summary>
        /// If set, will onlynotify on actions made one of the specified users
        /// </summary>
        [JsonPropertyName("triggering_user_ids")]
        public Nullable<Int64>[] TriggeringUserIds
        {
            get { return (Nullable<Int64>[])attributes["triggering_user_ids"]; }
            set { attributes["triggering_user_ids"] = value; }
        }

        /// <summary>
        /// Notify when actions are performed by a share recipient?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("trigger_by_share_recipients")]
        public bool TriggerByShareRecipients
        {
            get { return attributes["trigger_by_share_recipients"] == null ? false : (bool)attributes["trigger_by_share_recipients"]; }
            set { attributes["trigger_by_share_recipients"] = value; }
        }

        /// <summary>
        /// If true, will send notifications about a user's own activity to that user.  If false, only activity performed by other users (or anonymous users) will be sent in notifications.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_user_actions")]
        public bool NotifyUserActions
        {
            get { return attributes["notify_user_actions"] == null ? false : (bool)attributes["notify_user_actions"]; }
            set { attributes["notify_user_actions"] = value; }
        }

        /// <summary>
        /// Trigger on files copied to this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_copy")]
        public bool NotifyOnCopy
        {
            get { return attributes["notify_on_copy"] == null ? false : (bool)attributes["notify_on_copy"]; }
            set { attributes["notify_on_copy"] = value; }
        }

        /// <summary>
        /// Trigger on files deleted in this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_delete")]
        public bool NotifyOnDelete
        {
            get { return attributes["notify_on_delete"] == null ? false : (bool)attributes["notify_on_delete"]; }
            set { attributes["notify_on_delete"] = value; }
        }

        /// <summary>
        /// Trigger on files downloaded in this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_download")]
        public bool NotifyOnDownload
        {
            get { return attributes["notify_on_download"] == null ? false : (bool)attributes["notify_on_download"]; }
            set { attributes["notify_on_download"] = value; }
        }

        /// <summary>
        /// Trigger on files moved to this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_move")]
        public bool NotifyOnMove
        {
            get { return attributes["notify_on_move"] == null ? false : (bool)attributes["notify_on_move"]; }
            set { attributes["notify_on_move"] = value; }
        }

        /// <summary>
        /// Trigger on files created/uploaded/updated/changed in this path?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("notify_on_upload")]
        public bool NotifyOnUpload
        {
            get { return attributes["notify_on_upload"] == null ? false : (bool)attributes["notify_on_upload"]; }
            set { attributes["notify_on_upload"] = value; }
        }

        /// <summary>
        /// Apply notification recursively?  This will enable notifications for each subfolder.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("recursive")]
        public bool Recursive
        {
            get { return attributes["recursive"] == null ? false : (bool)attributes["recursive"]; }
            set { attributes["recursive"] = value; }
        }

        /// <summary>
        /// The time interval that notifications are aggregated to
        /// </summary>
        [JsonPropertyName("send_interval")]
        public string SendInterval
        {
            get { return (string)attributes["send_interval"]; }
            set { attributes["send_interval"] = value; }
        }

        /// <summary>
        /// Custom message to include in notification emails
        /// </summary>
        [JsonPropertyName("message")]
        public string Message
        {
            get { return (string)attributes["message"]; }
            set { attributes["message"] = value; }
        }

        /// <summary>
        /// Array of filenames (possibly with wildcards) to scope trigger
        /// </summary>
        [JsonPropertyName("triggering_filenames")]
        public string[] TriggeringFilenames
        {
            get { return (string[])attributes["triggering_filenames"]; }
            set { attributes["triggering_filenames"] = value; }
        }

        /// <summary>
        /// Is the user unsubscribed from this notification?
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("unsubscribed")]
        public bool Unsubscribed
        {
            get { return attributes["unsubscribed"] == null ? false : (bool)attributes["unsubscribed"]; }
            set { attributes["unsubscribed"] = value; }
        }

        /// <summary>
        /// The reason that the user unsubscribed
        /// </summary>
        [JsonPropertyName("unsubscribed_reason")]
        public string UnsubscribedReason
        {
            get { return (string)attributes["unsubscribed_reason"]; }
            set { attributes["unsubscribed_reason"] = value; }
        }

        /// <summary>
        /// Notification user ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Notification username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string)attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// If true, it means that the recipient at this user's email address has manually unsubscribed from all emails, or had their email "hard bounce", which means that we are unable to send mail to this user's current email address. Notifications will resume if the user changes their email address.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("suppressed_email")]
        public bool SuppressedEmail
        {
            get { return attributes["suppressed_email"] == null ? false : (bool)attributes["suppressed_email"]; }
            set { attributes["suppressed_email"] = value; }
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
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will onlynotify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        /// </summary>
        public async Task<Notification> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("notify_on_copy") && !(parameters["notify_on_copy"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_copy must be of type bool", "parameters[\"notify_on_copy\"]");
            }
            if (parameters.ContainsKey("notify_on_delete") && !(parameters["notify_on_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_delete must be of type bool", "parameters[\"notify_on_delete\"]");
            }
            if (parameters.ContainsKey("notify_on_download") && !(parameters["notify_on_download"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_download must be of type bool", "parameters[\"notify_on_download\"]");
            }
            if (parameters.ContainsKey("notify_on_move") && !(parameters["notify_on_move"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_move must be of type bool", "parameters[\"notify_on_move\"]");
            }
            if (parameters.ContainsKey("notify_on_upload") && !(parameters["notify_on_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_upload must be of type bool", "parameters[\"notify_on_upload\"]");
            }
            if (parameters.ContainsKey("notify_user_actions") && !(parameters["notify_user_actions"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_user_actions must be of type bool", "parameters[\"notify_user_actions\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("send_interval") && !(parameters["send_interval"] is string))
            {
                throw new ArgumentException("Bad parameter: send_interval must be of type string", "parameters[\"send_interval\"]");
            }
            if (parameters.ContainsKey("message") && !(parameters["message"] is string))
            {
                throw new ArgumentException("Bad parameter: message must be of type string", "parameters[\"message\"]");
            }
            if (parameters.ContainsKey("triggering_filenames") && !(parameters["triggering_filenames"] is string[]))
            {
                throw new ArgumentException("Bad parameter: triggering_filenames must be of type string[]", "parameters[\"triggering_filenames\"]");
            }
            if (parameters.ContainsKey("triggering_group_ids") && !(parameters["triggering_group_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: triggering_group_ids must be of type Nullable<Int64>[]", "parameters[\"triggering_group_ids\"]");
            }
            if (parameters.ContainsKey("triggering_user_ids") && !(parameters["triggering_user_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: triggering_user_ids must be of type Nullable<Int64>[]", "parameters[\"triggering_user_ids\"]");
            }
            if (parameters.ContainsKey("trigger_by_share_recipients") && !(parameters["trigger_by_share_recipients"] is bool))
            {
                throw new ArgumentException("Bad parameter: trigger_by_share_recipients must be of type bool", "parameters[\"trigger_by_share_recipients\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/notifications/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Notification>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public async Task Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id"))
            {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/notifications/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                await this.Update(this.attributes);
            }
            else
            {
                var newObj = await Notification.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   action - string
        ///   page - int64
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction (e.g. `sort_by[path]=desc`). Valid fields are `path`, `user_id` or `group_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `path`, `user_id` or `group_id`.
        ///   filter_prefix - object - If set, return records where the specified field is prefixed by the supplied value. Valid fields are `path`.
        ///   path - string - Show notifications for this Path.
        ///   include_ancestors - boolean - If `include_ancestors` is `true` and `path` is specified, include notifications for any parent paths. Ignored if `path` is not specified.
        ///   group_id - string
        /// </summary>
        public static FilesList<Notification> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("action") && !(parameters["action"] is string))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("page") && !(parameters["page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: page must be of type Nullable<Int64>", "parameters[\"page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_prefix") && !(parameters["filter_prefix"] is object))
            {
                throw new ArgumentException("Bad parameter: filter_prefix must be of type object", "parameters[\"filter_prefix\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("include_ancestors") && !(parameters["include_ancestors"] is bool))
            {
                throw new ArgumentException("Bad parameter: include_ancestors must be of type bool", "parameters[\"include_ancestors\"]");
            }
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is string))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type string", "parameters[\"group_id\"]");
            }

            return new FilesList<Notification>($"/notifications", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Notification> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Notification ID.
        /// </summary>
        public static async Task<Notification> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Notification>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<Notification> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
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
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will onlynotify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        ///   group_id - int64 - The ID of the group to notify.  Provide `user_id`, `username` or `group_id`.
        ///   path - string - Path
        ///   username - string - The username of the user to notify.  Provide `user_id`, `username` or `group_id`.
        /// </summary>
        public static async Task<Notification> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("notify_on_copy") && !(parameters["notify_on_copy"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_copy must be of type bool", "parameters[\"notify_on_copy\"]");
            }
            if (parameters.ContainsKey("notify_on_delete") && !(parameters["notify_on_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_delete must be of type bool", "parameters[\"notify_on_delete\"]");
            }
            if (parameters.ContainsKey("notify_on_download") && !(parameters["notify_on_download"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_download must be of type bool", "parameters[\"notify_on_download\"]");
            }
            if (parameters.ContainsKey("notify_on_move") && !(parameters["notify_on_move"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_move must be of type bool", "parameters[\"notify_on_move\"]");
            }
            if (parameters.ContainsKey("notify_on_upload") && !(parameters["notify_on_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_upload must be of type bool", "parameters[\"notify_on_upload\"]");
            }
            if (parameters.ContainsKey("notify_user_actions") && !(parameters["notify_user_actions"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_user_actions must be of type bool", "parameters[\"notify_user_actions\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("send_interval") && !(parameters["send_interval"] is string))
            {
                throw new ArgumentException("Bad parameter: send_interval must be of type string", "parameters[\"send_interval\"]");
            }
            if (parameters.ContainsKey("message") && !(parameters["message"] is string))
            {
                throw new ArgumentException("Bad parameter: message must be of type string", "parameters[\"message\"]");
            }
            if (parameters.ContainsKey("triggering_filenames") && !(parameters["triggering_filenames"] is string[]))
            {
                throw new ArgumentException("Bad parameter: triggering_filenames must be of type string[]", "parameters[\"triggering_filenames\"]");
            }
            if (parameters.ContainsKey("triggering_group_ids") && !(parameters["triggering_group_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: triggering_group_ids must be of type Nullable<Int64>[]", "parameters[\"triggering_group_ids\"]");
            }
            if (parameters.ContainsKey("triggering_user_ids") && !(parameters["triggering_user_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: triggering_user_ids must be of type Nullable<Int64>[]", "parameters[\"triggering_user_ids\"]");
            }
            if (parameters.ContainsKey("trigger_by_share_recipients") && !(parameters["trigger_by_share_recipients"] is bool))
            {
                throw new ArgumentException("Bad parameter: trigger_by_share_recipients must be of type bool", "parameters[\"trigger_by_share_recipients\"]");
            }
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/notifications", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Notification>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
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
        ///   message - string - Custom message to include in notification emails
        ///   triggering_filenames - array(string) - Array of filenames (possibly with wildcards) to scope trigger
        ///   triggering_group_ids - array(int64) - If set, will only notify on actions made by a member of one of the specified groups
        ///   triggering_user_ids - array(int64) - If set, will onlynotify on actions made one of the specified users
        ///   trigger_by_share_recipients - boolean - Notify when actions are performed by a share recipient?
        /// </summary>
        public static async Task<Notification> Update(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("notify_on_copy") && !(parameters["notify_on_copy"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_copy must be of type bool", "parameters[\"notify_on_copy\"]");
            }
            if (parameters.ContainsKey("notify_on_delete") && !(parameters["notify_on_delete"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_delete must be of type bool", "parameters[\"notify_on_delete\"]");
            }
            if (parameters.ContainsKey("notify_on_download") && !(parameters["notify_on_download"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_download must be of type bool", "parameters[\"notify_on_download\"]");
            }
            if (parameters.ContainsKey("notify_on_move") && !(parameters["notify_on_move"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_move must be of type bool", "parameters[\"notify_on_move\"]");
            }
            if (parameters.ContainsKey("notify_on_upload") && !(parameters["notify_on_upload"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_on_upload must be of type bool", "parameters[\"notify_on_upload\"]");
            }
            if (parameters.ContainsKey("notify_user_actions") && !(parameters["notify_user_actions"] is bool))
            {
                throw new ArgumentException("Bad parameter: notify_user_actions must be of type bool", "parameters[\"notify_user_actions\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("send_interval") && !(parameters["send_interval"] is string))
            {
                throw new ArgumentException("Bad parameter: send_interval must be of type string", "parameters[\"send_interval\"]");
            }
            if (parameters.ContainsKey("message") && !(parameters["message"] is string))
            {
                throw new ArgumentException("Bad parameter: message must be of type string", "parameters[\"message\"]");
            }
            if (parameters.ContainsKey("triggering_filenames") && !(parameters["triggering_filenames"] is string[]))
            {
                throw new ArgumentException("Bad parameter: triggering_filenames must be of type string[]", "parameters[\"triggering_filenames\"]");
            }
            if (parameters.ContainsKey("triggering_group_ids") && !(parameters["triggering_group_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: triggering_group_ids must be of type Nullable<Int64>[]", "parameters[\"triggering_group_ids\"]");
            }
            if (parameters.ContainsKey("triggering_user_ids") && !(parameters["triggering_user_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: triggering_user_ids must be of type Nullable<Int64>[]", "parameters[\"triggering_user_ids\"]");
            }
            if (parameters.ContainsKey("trigger_by_share_recipients") && !(parameters["trigger_by_share_recipients"] is bool))
            {
                throw new ArgumentException("Bad parameter: trigger_by_share_recipients must be of type bool", "parameters[\"trigger_by_share_recipients\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Notification>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// </summary>
        public static async Task Delete(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("id"))
            {
                parameters["id"] = id;
            }
            else
            {
                parameters.Add("id", id);
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }

            await FilesClient.SendRequest($"/notifications/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
        }

        public static async Task Destroy(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            await Delete(id, parameters, options);
        }

    }
}