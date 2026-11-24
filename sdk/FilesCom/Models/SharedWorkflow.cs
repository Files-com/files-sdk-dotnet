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
    /// SharedWorkflowEntity model
    /// </summary>
    public class SharedWorkflow : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        private FilesClient client;
        // Where this object's operations send their requests when not to its client's BaseUrl: the address of its
        // site's region, where an operation moved. Null for BaseUrl.
        private string origin;
        /// <summary>
        /// Makes a SharedWorkflow with no attributes set and no options.
        /// </summary>
        public SharedWorkflow() : this(null, null) { }

        /// <summary>
        /// Makes a SharedWorkflow with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public SharedWorkflow(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("id"))
            {
                this.attributes.Add("id", null);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("description"))
            {
                this.attributes.Add("description", null);
            }
            if (!this.attributes.ContainsKey("submission_revision"))
            {
                this.attributes.Add("submission_revision", null);
            }
            if (!this.attributes.ContainsKey("form_fields"))
            {
                this.attributes.Add("form_fields", new FormField[0]);
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
            get { return new object[] { FormFields }; }
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
        /// Shared Workflow ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// Name displayed to recipients.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            private set { attributes["name"] = value; }
        }

        /// <summary>
        /// Description displayed to recipients.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("description")]
        public string Description
        {
            get { return (string)attributes["description"]; }
            private set { attributes["description"] = value; }
        }

        /// <summary>
        /// Revision required when submitting a Shared Workflow run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("submission_revision")]
        public string SubmissionRevision
        {
            get { return (string)attributes["submission_revision"]; }
            private set { attributes["submission_revision"] = value; }
        }

        /// <summary>
        /// Current custom questions in display order.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("form_fields")]
        public FormField[] FormFields
        {
            get { return (FormField[])attributes["form_fields"]; }
            private set { attributes["form_fields"] = value; }
        }

        /// <summary>
        /// Run a Shared Workflow
        ///
        /// Parameters:
        ///   submission_revision (required) - string - Revision returned when opening the Shared Workflow.
        ///   submission_id (required) - string - Client-generated UUID used to retry the same submission without starting another run.
        ///   file_path (required) - string - Path of a file the recipient can read, including a file uploaded to a Snapshot created for this workflow.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        public Task<AutomationRun> RunWorkflow(Dictionary<string, object> parameters)
        {
            return RunWorkflowCore(parameters, CancellationToken.None);
        }

        /// <summary>
        /// Run a Shared Workflow
        ///
        /// Parameters:
        ///   submission_revision (required) - string - Revision returned when opening the Shared Workflow.
        ///   submission_id (required) - string - Client-generated UUID used to retry the same submission without starting another run.
        ///   file_path (required) - string - Path of a file the recipient can read, including a file uploaded to a Snapshot created for this workflow.
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="cancellationToken">Stops the request.</param>
        public Task<AutomationRun> RunWorkflowAsync(Dictionary<string, object> parameters = null, CancellationToken cancellationToken = default)
        {
            return RunWorkflowCore(DictionaryUtil.Copy(parameters), cancellationToken);
        }


        private async Task<AutomationRun> RunWorkflowCore(Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            object parameterValue;
            if (!attributes.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("The SharedWorkflow has no id, which RunWorkflow needs", "id");
            }
            parameters["id"] = parameterValue;
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (!parameters.TryGetValue("submission_revision", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: submission_revision", "submission_revision");
            }
            if (!parameters.TryGetValue("submission_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: submission_id", "submission_id");
            }
            if (!parameters.TryGetValue("file_path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: file_path", "file_path");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("submission_revision", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: submission_revision is null, which cannot be sent" : "Bad parameter: submission_revision must be of type string", "submission_revision");
            }
            if (parameters.TryGetValue("submission_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: submission_id is null, which cannot be sent" : "Bad parameter: submission_id must be of type string", "submission_id");
            }
            if (parameters.TryGetValue("file_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_path is null, which cannot be sent" : "Bad parameter: file_path must be of type string", "file_path");
            }

            OperationContext context = new OperationContext(FilesClient.Bind(ref client), origin);
            // This operation's options, for its request and the objects it returns, unaffected by later SetOption calls.
            Dictionary<string, object> requestOptions = DictionaryUtil.Copy(options);
            string responseJson = await FilesClient.SendStringRequest(context, $"/shared_workflows/{System.Uri.EscapeDataString(attributes["id"].ToString())}/run_workflow", System.Net.Http.HttpMethod.Post, parameters, requestOptions, cancellationToken);
            AutomationRun result = JsonUtil.DeserializeModelResponse<AutomationRun>(responseJson, context, requestOptions);
            // This object's later operations go where this one went.
            origin = context.RelocatedOrigin;
            return result;
        }



        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<SharedWorkflow> List(

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
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are .
        /// </summary>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static FilesList<SharedWorkflow> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        internal static FilesList<SharedWorkflow> ListCore(
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

            return new FilesList<SharedWorkflow>(client, $"/shared_workflows", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Shared Workflow ID.
        /// </summary>
        /// <param name="id">Shared Workflow ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<SharedWorkflow> Find(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Shared Workflow ID.
        /// </summary>
        /// <param name="id">Shared Workflow ID.</param>
        /// <param name="parameters">The operation takes no parameters besides id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<SharedWorkflow> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return Find(id, parameters, options);
        }

        internal static async Task<SharedWorkflow> FindCore(
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

            string responseJson = await FilesClient.SendStringRequest(context, $"/shared_workflows/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<SharedWorkflow>(responseJson, context, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Shared Workflow ID.
        ///   run_id (required) - int64 - ID of the run returned by the Shared Workflow submission.
        /// </summary>
        /// <param name="id">Shared Workflow ID.</param>
        /// <param name="run_id">ID of the run returned by the Shared Workflow submission.</param>
        /// <param name="parameters">The operation takes no parameters besides id and run_id.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AutomationRun> FindRun(
            Nullable<Int64> id, Nullable<Int64> run_id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return FindRunCore(OperationContext.OfDefaultClient(), id, run_id, parameters, options, CancellationToken.None);
        }

        internal static async Task<AutomationRun> FindRunCore(
            OperationContext context,
            Nullable<Int64> id, Nullable<Int64> run_id,
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
            if (parameters.ContainsKey("run_id"))
            {
                parameters["run_id"] = run_id;
            }
            else
            {
                parameters.Add("run_id", run_id);
            }
            if (!parameters.TryGetValue("id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: id", "id");
            }
            if (!parameters.TryGetValue("run_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: run_id", "run_id");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("run_id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: run_id is null, which cannot be sent" : "Bad parameter: run_id must be of type Nullable<Int64>", "run_id");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/shared_workflows/{System.Uri.EscapeDataString(parameters["id"].ToString())}/runs/{System.Uri.EscapeDataString(parameters["run_id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AutomationRun>(responseJson, context, options);
        }

        /// <summary>
        /// Run a Shared Workflow
        ///
        /// Parameters:
        ///   submission_revision (required) - string - Revision returned when opening the Shared Workflow.
        ///   submission_id (required) - string - Client-generated UUID used to retry the same submission without starting another run.
        ///   file_path (required) - string - Path of a file the recipient can read, including a file uploaded to a Snapshot created for this workflow.
        /// </summary>
        /// <param name="id">Shared Workflow ID.</param>
        /// <param name="parameters">The operation's parameters listed in the summary, keyed by their API names.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public static Task<AutomationRun> RunWorkflow(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return RunWorkflowCore(OperationContext.OfDefaultClient(), id, parameters, options, CancellationToken.None);
        }

        internal static async Task<AutomationRun> RunWorkflowCore(
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
            if (!parameters.TryGetValue("submission_revision", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: submission_revision", "submission_revision");
            }
            if (!parameters.TryGetValue("submission_id", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: submission_id", "submission_id");
            }
            if (!parameters.TryGetValue("file_path", out parameterValue) || parameterValue == null)
            {
                throw new MissingParameterException("Parameter missing: file_path", "file_path");
            }
            if (parameters.TryGetValue("id", out parameterValue) && !(parameterValue is Nullable<Int64>))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: id is null, which cannot be sent" : "Bad parameter: id must be of type Nullable<Int64>", "id");
            }
            if (parameters.TryGetValue("submission_revision", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: submission_revision is null, which cannot be sent" : "Bad parameter: submission_revision must be of type string", "submission_revision");
            }
            if (parameters.TryGetValue("submission_id", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: submission_id is null, which cannot be sent" : "Bad parameter: submission_id must be of type string", "submission_id");
            }
            if (parameters.TryGetValue("file_path", out parameterValue) && !(parameterValue is string))
            {
                throw new InvalidParameterException(parameterValue == null ? "Bad parameter: file_path is null, which cannot be sent" : "Bad parameter: file_path must be of type string", "file_path");
            }

            string responseJson = await FilesClient.SendStringRequest(context, $"/shared_workflows/{System.Uri.EscapeDataString(parameters["id"].ToString())}/run_workflow", System.Net.Http.HttpMethod.Post, parameters, options, cancellationToken);

            return JsonUtil.DeserializeModelResponse<AutomationRun>(responseJson, context, options);
        }

    }
}