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
    /// ExpectationEvaluationEntity model
    /// </summary>
    public class ExpectationEvaluation : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a ExpectationEvaluation with no attributes set and no options.
        /// </summary>
        public ExpectationEvaluation() : this(null, null) { }

        /// <summary>
        /// Makes a ExpectationEvaluation with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public ExpectationEvaluation(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("expectation_id"))
            {
                this.attributes.Add("expectation_id", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("opened_via"))
            {
                this.attributes.Add("opened_via", null);
            }
            if (!this.attributes.ContainsKey("opened_at"))
            {
                this.attributes.Add("opened_at", null);
            }
            if (!this.attributes.ContainsKey("window_start_at"))
            {
                this.attributes.Add("window_start_at", null);
            }
            if (!this.attributes.ContainsKey("window_end_at"))
            {
                this.attributes.Add("window_end_at", null);
            }
            if (!this.attributes.ContainsKey("deadline_at"))
            {
                this.attributes.Add("deadline_at", null);
            }
            if (!this.attributes.ContainsKey("late_acceptance_cutoff_at"))
            {
                this.attributes.Add("late_acceptance_cutoff_at", null);
            }
            if (!this.attributes.ContainsKey("hard_close_at"))
            {
                this.attributes.Add("hard_close_at", null);
            }
            if (!this.attributes.ContainsKey("closed_at"))
            {
                this.attributes.Add("closed_at", null);
            }
            if (!this.attributes.ContainsKey("matched_files"))
            {
                this.attributes.Add("matched_files", new object[0]);
            }
            if (!this.attributes.ContainsKey("missing_files"))
            {
                this.attributes.Add("missing_files", new object[0]);
            }
            if (!this.attributes.ContainsKey("criteria_errors"))
            {
                this.attributes.Add("criteria_errors", new string[0]);
            }
            if (!this.attributes.ContainsKey("summary"))
            {
                this.attributes.Add("summary", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
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
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        string IModel.Origin => null;

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
        /// ExpectationEvaluation ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Workspace ID. `0` means the default workspace.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            private set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// Expectation ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("expectation_id")]
        public Nullable<Int64> ExpectationId
        {
            get { return (Nullable<Int64>)attributes["expectation_id"]; }
            private set { attributes["expectation_id"] = value; }
        }

        /// <summary>
        /// Evaluation status.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// How the evaluation window was opened.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("opened_via")]
        public string OpenedVia
        {
            get { return (string)attributes["opened_via"]; }
            private set { attributes["opened_via"] = value; }
        }

        /// <summary>
        /// When the evaluation row was opened.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("opened_at")]
        public Nullable<DateTime> OpenedAt
        {
            get { return (Nullable<DateTime>)attributes["opened_at"]; }
            private set { attributes["opened_at"] = value; }
        }

        /// <summary>
        /// Logical start of the candidate window.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("window_start_at")]
        public Nullable<DateTime> WindowStartAt
        {
            get { return (Nullable<DateTime>)attributes["window_start_at"]; }
            private set { attributes["window_start_at"] = value; }
        }

        /// <summary>
        /// Actual candidate cutoff boundary for the window.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("window_end_at")]
        public Nullable<DateTime> WindowEndAt
        {
            get { return (Nullable<DateTime>)attributes["window_end_at"]; }
            private set { attributes["window_end_at"] = value; }
        }

        /// <summary>
        /// Logical due boundary for schedule-driven windows.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("deadline_at")]
        public Nullable<DateTime> DeadlineAt
        {
            get { return (Nullable<DateTime>)attributes["deadline_at"]; }
            private set { attributes["deadline_at"] = value; }
        }

        /// <summary>
        /// Logical cutoff for late acceptance, when present.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("late_acceptance_cutoff_at")]
        public Nullable<DateTime> LateAcceptanceCutoffAt
        {
            get { return (Nullable<DateTime>)attributes["late_acceptance_cutoff_at"]; }
            private set { attributes["late_acceptance_cutoff_at"] = value; }
        }

        /// <summary>
        /// Hard stop after which the window may not remain open.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("hard_close_at")]
        public Nullable<DateTime> HardCloseAt
        {
            get { return (Nullable<DateTime>)attributes["hard_close_at"]; }
            private set { attributes["hard_close_at"] = value; }
        }

        /// <summary>
        /// When the evaluation row was finalized.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("closed_at")]
        public Nullable<DateTime> ClosedAt
        {
            get { return (Nullable<DateTime>)attributes["closed_at"]; }
            private set { attributes["closed_at"] = value; }
        }

        /// <summary>
        /// Captured evidence for files that matched the window.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("matched_files")]
        public object[] MatchedFiles
        {
            get { return (object[])attributes["matched_files"]; }
            private set { attributes["matched_files"] = value; }
        }

        /// <summary>
        /// Captured evidence for required files that were missing.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("missing_files")]
        public object[] MissingFiles
        {
            get { return (object[])attributes["missing_files"]; }
            private set { attributes["missing_files"] = value; }
        }

        /// <summary>
        /// Captured criteria failures for the window, including file-scoped FTS errors and returned validation details.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("criteria_errors")]
        public string[] CriteriaErrors
        {
            get { return (string[])attributes["criteria_errors"]; }
            private set { attributes["criteria_errors"] = value; }
        }

        /// <summary>
        /// Compact evaluator summary payload.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("summary")]
        public object Summary
        {
            get { return (object)attributes["summary"]; }
            private set { attributes["summary"] = value; }
        }

        /// <summary>
        /// Creation time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Last update time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>)attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `expectation_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `expectation_id` and `workspace_id`. Valid field combinations are `[ workspace_id, expectation_id ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<ExpectationEvaluation> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `workspace_id`, `created_at` or `expectation_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `expectation_id` and `workspace_id`. Valid field combinations are `[ workspace_id, expectation_id ]`.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<ExpectationEvaluation> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<ExpectationEvaluation> ListCore(
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

            return new FilesList<ExpectationEvaluation>(client, $"/expectation_evaluations", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation Evaluation ID.
        /// </summary>
        /// <param name="id">Expectation Evaluation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ExpectationEvaluation> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Expectation Evaluation ID.
        /// </summary>
        /// <param name="id">Expectation Evaluation ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<ExpectationEvaluation> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<ExpectationEvaluation> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/expectation_evaluations/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<ExpectationEvaluation>(responseJson, context, options);
        }

    }
}