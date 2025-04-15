using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Restore
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Restore() : this(null, null) { }

        public Restore(Dictionary<string, object> attributes, Dictionary<string, object> options)
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

            if (!this.attributes.ContainsKey("earliest_date"))
            {
                this.attributes.Add("earliest_date", null);
            }
            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("dirs_restored"))
            {
                this.attributes.Add("dirs_restored", null);
            }
            if (!this.attributes.ContainsKey("dirs_errored"))
            {
                this.attributes.Add("dirs_errored", null);
            }
            if (!this.attributes.ContainsKey("dirs_total"))
            {
                this.attributes.Add("dirs_total", null);
            }
            if (!this.attributes.ContainsKey("files_restored"))
            {
                this.attributes.Add("files_restored", null);
            }
            if (!this.attributes.ContainsKey("files_errored"))
            {
                this.attributes.Add("files_errored", null);
            }
            if (!this.attributes.ContainsKey("files_total"))
            {
                this.attributes.Add("files_total", null);
            }
            if (!this.attributes.ContainsKey("prefix"))
            {
                this.attributes.Add("prefix", null);
            }
            if (!this.attributes.ContainsKey("restore_in_place"))
            {
                this.attributes.Add("restore_in_place", false);
            }
            if (!this.attributes.ContainsKey("restore_deleted_permissions"))
            {
                this.attributes.Add("restore_deleted_permissions", false);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("update_timestamps"))
            {
                this.attributes.Add("update_timestamps", false);
            }
            if (!this.attributes.ContainsKey("error_messages"))
            {
                this.attributes.Add("error_messages", new string[0]);
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
        /// Restore all files deleted after this date/time. Don't set this earlier than you need. Can not be greater than 365 days prior to the restore request.
        /// </summary>
        [JsonPropertyName("earliest_date")]
        public Nullable<DateTime> EarliestDate
        {
            get { return (Nullable<DateTime>)attributes["earliest_date"]; }
            set { attributes["earliest_date"] = value; }
        }

        /// <summary>
        /// Restore Record ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Number of directories that were successfully restored.
        /// </summary>
        [JsonPropertyName("dirs_restored")]
        public Nullable<Int64> DirsRestored
        {
            get { return (Nullable<Int64>)attributes["dirs_restored"]; }
            set { attributes["dirs_restored"] = value; }
        }

        /// <summary>
        /// Number of directories that were not able to be restored.
        /// </summary>
        [JsonPropertyName("dirs_errored")]
        public Nullable<Int64> DirsErrored
        {
            get { return (Nullable<Int64>)attributes["dirs_errored"]; }
            set { attributes["dirs_errored"] = value; }
        }

        /// <summary>
        /// Total number of directories processed.
        /// </summary>
        [JsonPropertyName("dirs_total")]
        public Nullable<Int64> DirsTotal
        {
            get { return (Nullable<Int64>)attributes["dirs_total"]; }
            set { attributes["dirs_total"] = value; }
        }

        /// <summary>
        /// Number of files successfully restored.
        /// </summary>
        [JsonPropertyName("files_restored")]
        public Nullable<Int64> FilesRestored
        {
            get { return (Nullable<Int64>)attributes["files_restored"]; }
            set { attributes["files_restored"] = value; }
        }

        /// <summary>
        /// Number of files that were not able to be restored.
        /// </summary>
        [JsonPropertyName("files_errored")]
        public Nullable<Int64> FilesErrored
        {
            get { return (Nullable<Int64>)attributes["files_errored"]; }
            set { attributes["files_errored"] = value; }
        }

        /// <summary>
        /// Total number of files processed.
        /// </summary>
        [JsonPropertyName("files_total")]
        public Nullable<Int64> FilesTotal
        {
            get { return (Nullable<Int64>)attributes["files_total"]; }
            set { attributes["files_total"] = value; }
        }

        /// <summary>
        /// Prefix of the files/folders to restore. To restore a folder, add a trailing slash to the folder name. Do not use a leading slash. To restore all deleted items, specify an empty string (`''`) in the prefix field or omit the field from the request.
        /// </summary>
        [JsonPropertyName("prefix")]
        public string Prefix
        {
            get { return (string)attributes["prefix"]; }
            set { attributes["prefix"] = value; }
        }

        /// <summary>
        /// If true, we will restore the files in place (into their original paths). If false, we will create a new restoration folder in the root and restore files there.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("restore_in_place")]
        public bool RestoreInPlace
        {
            get { return attributes["restore_in_place"] == null ? false : (bool)attributes["restore_in_place"]; }
            set { attributes["restore_in_place"] = value; }
        }

        /// <summary>
        /// If true, we will also restore any Permissions that match the same path prefix from the same dates.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("restore_deleted_permissions")]
        public bool RestoreDeletedPermissions
        {
            get { return attributes["restore_deleted_permissions"] == null ? false : (bool)attributes["restore_deleted_permissions"]; }
            set { attributes["restore_deleted_permissions"] = value; }
        }

        /// <summary>
        /// Status of the restoration process.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            set { attributes["status"] = value; }
        }

        /// <summary>
        /// If true, we will update the last modified timestamp of restored files to today's date. If false, we might trigger File Expiration to delete the file again.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("update_timestamps")]
        public bool UpdateTimestamps
        {
            get { return attributes["update_timestamps"] == null ? false : (bool)attributes["update_timestamps"]; }
            set { attributes["update_timestamps"] = value; }
        }

        /// <summary>
        /// Error messages received while restoring files and/or directories. Only present if there were errors.
        /// </summary>
        [JsonPropertyName("error_messages")]
        public string[] ErrorMessages
        {
            get { return (string[])attributes["error_messages"]; }
            set { attributes["error_messages"] = value; }
        }


        public async Task Save()
        {
            if (this.attributes["id"] != null)
            {
                throw new NotImplementedException("The Restore object doesn't support updates.");
            }
            else
            {
                var newObj = await Restore.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<Restore> List(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }

            return new FilesList<Restore>($"/restores", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Restore> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   earliest_date (required) - string - Restore all files deleted after this date/time. Don't set this earlier than you need. Can not be greater than 365 days prior to the restore request.
        ///   prefix - string - Prefix of the files/folders to restore. To restore a folder, add a trailing slash to the folder name. Do not use a leading slash. To restore all deleted items, specify an empty string (`''`) in the prefix field or omit the field from the request.
        ///   restore_deleted_permissions - boolean - If true, we will also restore any Permissions that match the same path prefix from the same dates.
        ///   restore_in_place - boolean - If true, we will restore the files in place (into their original paths). If false, we will create a new restoration folder in the root and restore files there.
        ///   update_timestamps - boolean - If true, we will update the last modified timestamp of restored files to today's date. If false, we might trigger File Expiration to delete the file again.
        /// </summary>
        public static async Task<Restore> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("earliest_date") || parameters["earliest_date"] == null)
            {
                throw new ArgumentNullException("Parameter missing: earliest_date", "parameters[\"earliest_date\"]");
            }
            if (parameters.ContainsKey("earliest_date") && !(parameters["earliest_date"] is string))
            {
                throw new ArgumentException("Bad parameter: earliest_date must be of type string", "parameters[\"earliest_date\"]");
            }
            if (parameters.ContainsKey("prefix") && !(parameters["prefix"] is string))
            {
                throw new ArgumentException("Bad parameter: prefix must be of type string", "parameters[\"prefix\"]");
            }
            if (parameters.ContainsKey("restore_deleted_permissions") && !(parameters["restore_deleted_permissions"] is bool))
            {
                throw new ArgumentException("Bad parameter: restore_deleted_permissions must be of type bool", "parameters[\"restore_deleted_permissions\"]");
            }
            if (parameters.ContainsKey("restore_in_place") && !(parameters["restore_in_place"] is bool))
            {
                throw new ArgumentException("Bad parameter: restore_in_place must be of type bool", "parameters[\"restore_in_place\"]");
            }
            if (parameters.ContainsKey("update_timestamps") && !(parameters["update_timestamps"] is bool))
            {
                throw new ArgumentException("Bad parameter: update_timestamps must be of type bool", "parameters[\"update_timestamps\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/restores", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<Restore>(responseJson);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


    }
}