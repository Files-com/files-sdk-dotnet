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
    /// AutomationExecutionNodeEntity model
    /// </summary>
    public class AutomationExecutionNode : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        /// <summary>
        /// Makes a AutomationExecutionNode with no attributes set and no options.
        /// </summary>
        public AutomationExecutionNode() : this(null, null) { }

        /// <summary>
        /// Makes a AutomationExecutionNode with copies of <paramref name="attributes"/>, keyed by their API names, and of
        /// <paramref name="options"/>. Later changes to the dictionaries do not change the object.
        /// </summary>
        /// <param name="attributes">The object's attributes, keyed by their API names. Null for none.</param>
        /// <param name="options">Request options, such as api_key, session_id or workspace_id.</param>
        public AutomationExecutionNode(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes != null ? new Dictionary<string, object>(attributes) : new Dictionary<string, object>();
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();

            if (!this.attributes.ContainsKey("node_id"))
            {
                this.attributes.Add("node_id", null);
            }
            if (!this.attributes.ContainsKey("node_type"))
            {
                this.attributes.Add("node_type", null);
            }
            if (!this.attributes.ContainsKey("status"))
            {
                this.attributes.Add("status", null);
            }
            if (!this.attributes.ContainsKey("run_stage"))
            {
                this.attributes.Add("run_stage", null);
            }
            if (!this.attributes.ContainsKey("reused"))
            {
                this.attributes.Add("reused", false);
            }
            if (!this.attributes.ContainsKey("successful_operations"))
            {
                this.attributes.Add("successful_operations", null);
            }
            if (!this.attributes.ContainsKey("failed_operations"))
            {
                this.attributes.Add("failed_operations", null);
            }
            if (!this.attributes.ContainsKey("started_at"))
            {
                this.attributes.Add("started_at", null);
            }
            if (!this.attributes.ContainsKey("completed_at"))
            {
                this.attributes.Add("completed_at", null);
            }
            if (!this.attributes.ContainsKey("duration_ms"))
            {
                this.attributes.Add("duration_ms", null);
            }
            if (!this.attributes.ContainsKey("inputs"))
            {
                this.attributes.Add("inputs", new object[0]);
            }
            if (!this.attributes.ContainsKey("outputs"))
            {
                this.attributes.Add("outputs", null);
            }
            if (!this.attributes.ContainsKey("input_items"))
            {
                this.attributes.Add("input_items", null);
            }
            if (!this.attributes.ContainsKey("output_items"))
            {
                this.attributes.Add("output_items", null);
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
        /// Node ID from the pinned Automation definition.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("node_id")]
        public string NodeId
        {
            get { return (string)attributes["node_id"]; }
            private set { attributes["node_id"] = value; }
        }

        /// <summary>
        /// Node type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("node_type")]
        public string NodeType
        {
            get { return (string)attributes["node_type"]; }
            private set { attributes["node_type"] = value; }
        }

        /// <summary>
        /// Node status.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public string Status
        {
            get { return (string)attributes["status"]; }
            private set { attributes["status"] = value; }
        }

        /// <summary>
        /// Current node execution stage.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("run_stage")]
        public string RunStage
        {
            get { return (string)attributes["run_stage"]; }
            private set { attributes["run_stage"] = value; }
        }

        /// <summary>
        /// Whether this node reused persisted output from an earlier run.
        /// </summary>
        [JsonInclude]
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("reused")]
        public bool Reused
        {
            get { return attributes["reused"] == null ? false : (bool)attributes["reused"]; }
            private set { attributes["reused"] = value; }
        }

        /// <summary>
        /// Count of successful operations in this node.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("successful_operations")]
        public Nullable<Int64> SuccessfulOperations
        {
            get { return (Nullable<Int64>)attributes["successful_operations"]; }
            private set { attributes["successful_operations"] = value; }
        }

        /// <summary>
        /// Count of failed operations in this node.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("failed_operations")]
        public Nullable<Int64> FailedOperations
        {
            get { return (Nullable<Int64>)attributes["failed_operations"]; }
            private set { attributes["failed_operations"] = value; }
        }

        /// <summary>
        /// When this node started.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("started_at")]
        public Nullable<DateTime> StartedAt
        {
            get { return (Nullable<DateTime>)attributes["started_at"]; }
            private set { attributes["started_at"] = value; }
        }

        /// <summary>
        /// When this node completed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("completed_at")]
        public Nullable<DateTime> CompletedAt
        {
            get { return (Nullable<DateTime>)attributes["completed_at"]; }
            private set { attributes["completed_at"] = value; }
        }

        /// <summary>
        /// Node runtime in milliseconds.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("duration_ms")]
        public Nullable<Int64> DurationMs
        {
            get { return (Nullable<Int64>)attributes["duration_ms"]; }
            private set { attributes["duration_ms"] = value; }
        }

        /// <summary>
        /// Ordered inbound edge references.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("inputs")]
        public object[] Inputs
        {
            get { return (object[])attributes["inputs"]; }
            private set { attributes["inputs"] = value; }
        }

        /// <summary>
        /// Output counts, item kinds, and typed-error summaries by outlet.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("outputs")]
        public object Outputs
        {
            get { return (object)attributes["outputs"]; }
            private set { attributes["outputs"] = value; }
        }

        /// <summary>
        /// Materialized input items currently available, grouped by inlet.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("input_items")]
        public object InputItems
        {
            get { return (object)attributes["input_items"]; }
            private set { attributes["input_items"] = value; }
        }

        /// <summary>
        /// Materialized output items grouped by outlet.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_items")]
        public object OutputItems
        {
            get { return (object)attributes["output_items"]; }
            private set { attributes["output_items"] = value; }
        }



    }
}