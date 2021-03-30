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
            if (!this.attributes.ContainsKey("notify_user_actions"))
            {
                this.attributes.Add("notify_user_actions", null);
            }
            if (!this.attributes.ContainsKey("notify_on_copy"))
            {
                this.attributes.Add("notify_on_copy", null);
            }
            if (!this.attributes.ContainsKey("recursive"))
            {
                this.attributes.Add("recursive", null);
            }
            if (!this.attributes.ContainsKey("send_interval"))
            {
                this.attributes.Add("send_interval", null);
            }
            if (!this.attributes.ContainsKey("unsubscribed"))
            {
                this.attributes.Add("unsubscribed", null);
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
                this.attributes.Add("suppressed_email", null);
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
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Folder path to notify on This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// Notification group id
        /// </summary>
        [JsonPropertyName("group_id")]
        public Nullable<Int64> GroupId
        {
            get { return (Nullable<Int64>) attributes["group_id"]; }
            set { attributes["group_id"] = value; }
        }

        /// <summary>
        /// Group name if applicable
        /// </summary>
        [JsonPropertyName("group_name")]
        public string GroupName
        {
            get { return (string) attributes["group_name"]; }
            set { attributes["group_name"] = value; }
        }

        /// <summary>
        /// Trigger notification on notification user actions?
        /// </summary>
        [JsonPropertyName("notify_user_actions")]
        public bool NotifyUserActions
        {
            get { return (bool) attributes["notify_user_actions"]; }
            set { attributes["notify_user_actions"] = value; }
        }

        /// <summary>
        /// Triggers notification when moving or copying files to this path
        /// </summary>
        [JsonPropertyName("notify_on_copy")]
        public bool NotifyOnCopy
        {
            get { return (bool) attributes["notify_on_copy"]; }
            set { attributes["notify_on_copy"] = value; }
        }

        /// <summary>
        /// Enable notifications for each subfolder in this path
        /// </summary>
        [JsonPropertyName("recursive")]
        public bool Recursive
        {
            get { return (bool) attributes["recursive"]; }
            set { attributes["recursive"] = value; }
        }

        /// <summary>
        /// The time interval that notifications are aggregated to
        /// </summary>
        [JsonPropertyName("send_interval")]
        public string SendInterval
        {
            get { return (string) attributes["send_interval"]; }
            set { attributes["send_interval"] = value; }
        }

        /// <summary>
        /// Is the user unsubscribed from this notification?
        /// </summary>
        [JsonPropertyName("unsubscribed")]
        public bool Unsubscribed
        {
            get { return (bool) attributes["unsubscribed"]; }
            set { attributes["unsubscribed"] = value; }
        }

        /// <summary>
        /// The reason that the user unsubscribed
        /// </summary>
        [JsonPropertyName("unsubscribed_reason")]
        public string UnsubscribedReason
        {
            get { return (string) attributes["unsubscribed_reason"]; }
            set { attributes["unsubscribed_reason"] = value; }
        }

        /// <summary>
        /// Notification user ID
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Notification username
        /// </summary>
        [JsonPropertyName("username")]
        public string Username
        {
            get { return (string) attributes["username"]; }
            set { attributes["username"] = value; }
        }

        /// <summary>
        /// If true, it means that the recipient at this user's email address has manually unsubscribed from all emails, or had their email "hard bounce", which means that we are unable to send mail to this user's current email address. Notifications will resume if the user changes their email address.
        /// </summary>
        [JsonPropertyName("suppressed_email")]
        public bool SuppressedEmail
        {
            get { return (bool) attributes["suppressed_email"]; }
            set { attributes["suppressed_email"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   notify_on_copy - boolean - If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
        /// </summary>
        public async Task<Notification> Update(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id")) {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("notify_on_copy") && !(parameters["notify_on_copy"] is bool ))
            {
                throw new ArgumentException("Bad parameter: notify_on_copy must be of type bool", "parameters[\"notify_on_copy\"]");
            }
            if (parameters.ContainsKey("notify_user_actions") && !(parameters["notify_user_actions"] is bool ))
            {
                throw new ArgumentException("Bad parameter: notify_user_actions must be of type bool", "parameters[\"notify_user_actions\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool ))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("send_interval") && !(parameters["send_interval"] is string ))
            {
                throw new ArgumentException("Bad parameter: send_interval must be of type string", "parameters[\"send_interval\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/notifications/{attributes["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Notification>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Notification> Delete(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["id"] = attributes["id"];

            if (!attributes.ContainsKey("id")) {
                throw new ArgumentException("Current object doesn't have a id");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/notifications/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Notification>(responseJson);
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
        ///   user_id - int64 - DEPRECATED: Show notifications for this User ID. Use `filter[user_id]` instead.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either 'asc' or 'desc' direction (e.g. sort_by[last_login_at]=desc). Valid fields are `path`, `user_id` or `group_id`.
        ///   filter - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `user_id`, `group_id` or `path`.
        ///   filter_gt - object - If set, return records where the specifiied field is greater than the supplied value. Valid fields are `user_id`, `group_id` or `path`.
        ///   filter_gteq - object - If set, return records where the specifiied field is greater than or equal to the supplied value. Valid fields are `user_id`, `group_id` or `path`.
        ///   filter_like - object - If set, return records where the specifiied field is equal to the supplied value. Valid fields are `user_id`, `group_id` or `path`.
        ///   filter_lt - object - If set, return records where the specifiied field is less than the supplied value. Valid fields are `user_id`, `group_id` or `path`.
        ///   filter_lteq - object - If set, return records where the specifiied field is less than or equal to the supplied value. Valid fields are `user_id`, `group_id` or `path`.
        ///   group_id - int64 - DEPRECATED: Show notifications for this Group ID. Use `filter[group_id]` instead.
        ///   path - string - Show notifications for this Path.
        ///   include_ancestors - boolean - If `include_ancestors` is `true` and `path` is specified, include notifications for any parent paths. Ignored if `path` is not specified.
        /// </summary>
        public static async Task<Notification[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object ))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("filter_gt") && !(parameters["filter_gt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gt must be of type object", "parameters[\"filter_gt\"]");
            }
            if (parameters.ContainsKey("filter_gteq") && !(parameters["filter_gteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_gteq must be of type object", "parameters[\"filter_gteq\"]");
            }
            if (parameters.ContainsKey("filter_like") && !(parameters["filter_like"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_like must be of type object", "parameters[\"filter_like\"]");
            }
            if (parameters.ContainsKey("filter_lt") && !(parameters["filter_lt"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lt must be of type object", "parameters[\"filter_lt\"]");
            }
            if (parameters.ContainsKey("filter_lteq") && !(parameters["filter_lteq"] is object ))
            {
                throw new ArgumentException("Bad parameter: filter_lteq must be of type object", "parameters[\"filter_lteq\"]");
            }
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("include_ancestors") && !(parameters["include_ancestors"] is bool ))
            {
                throw new ArgumentException("Bad parameter: include_ancestors must be of type bool", "parameters[\"include_ancestors\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/notifications", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Notification[]>(responseJson);
        }

        public static async Task<Notification[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
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

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/notifications/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Notification>(responseJson);
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
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
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

            if (parameters.ContainsKey("user_id") && !(parameters["user_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: user_id must be of type Nullable<Int64>", "parameters[\"user_id\"]");
            }
            if (parameters.ContainsKey("notify_on_copy") && !(parameters["notify_on_copy"] is bool ))
            {
                throw new ArgumentException("Bad parameter: notify_on_copy must be of type bool", "parameters[\"notify_on_copy\"]");
            }
            if (parameters.ContainsKey("notify_user_actions") && !(parameters["notify_user_actions"] is bool ))
            {
                throw new ArgumentException("Bad parameter: notify_user_actions must be of type bool", "parameters[\"notify_user_actions\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool ))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("send_interval") && !(parameters["send_interval"] is string ))
            {
                throw new ArgumentException("Bad parameter: send_interval must be of type string", "parameters[\"send_interval\"]");
            }
            if (parameters.ContainsKey("group_id") && !(parameters["group_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: group_id must be of type Nullable<Int64>", "parameters[\"group_id\"]");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("username") && !(parameters["username"] is string ))
            {
                throw new ArgumentException("Bad parameter: username must be of type string", "parameters[\"username\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/notifications", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Notification>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   notify_on_copy - boolean - If `true`, copying or moving resources into this path will trigger a notification, in addition to just uploads.
        ///   notify_user_actions - boolean - If `true` actions initiated by the user will still result in a notification
        ///   recursive - boolean - If `true`, enable notifications for each subfolder in this path
        ///   send_interval - string - The time interval that notifications are aggregated by.  Can be `five_minutes`, `fifteen_minutes`, `hourly`, or `daily`.
        /// </summary>
        public static async Task<Notification> Update(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("notify_on_copy") && !(parameters["notify_on_copy"] is bool ))
            {
                throw new ArgumentException("Bad parameter: notify_on_copy must be of type bool", "parameters[\"notify_on_copy\"]");
            }
            if (parameters.ContainsKey("notify_user_actions") && !(parameters["notify_user_actions"] is bool ))
            {
                throw new ArgumentException("Bad parameter: notify_user_actions must be of type bool", "parameters[\"notify_user_actions\"]");
            }
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool ))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (parameters.ContainsKey("send_interval") && !(parameters["send_interval"] is string ))
            {
                throw new ArgumentException("Bad parameter: send_interval must be of type string", "parameters[\"send_interval\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/notifications/{parameters["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Notification>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Notification> Delete(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("id", id);
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/notifications/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Notification>(responseJson);
        }

        public static async Task<Notification> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

