using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class EventSubscription
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public EventSubscription() : this(null, null) { }

        public EventSubscription(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("event_channel_id"))
            {
                this.attributes.Add("event_channel_id", null);
            }
            if (!this.attributes.ContainsKey("workspace_id"))
            {
                this.attributes.Add("workspace_id", null);
            }
            if (!this.attributes.ContainsKey("apply_to_all_workspaces"))
            {
                this.attributes.Add("apply_to_all_workspaces", false);
            }
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("enabled"))
            {
                this.attributes.Add("enabled", false);
            }
            if (!this.attributes.ContainsKey("event_types"))
            {
                this.attributes.Add("event_types", new string[0]);
            }
            if (!this.attributes.ContainsKey("filter"))
            {
                this.attributes.Add("filter", null);
            }
            if (!this.attributes.ContainsKey("delivery_policy"))
            {
                this.attributes.Add("delivery_policy", null);
            }
            if (!this.attributes.ContainsKey("event_target_ids"))
            {
                this.attributes.Add("event_target_ids", new Nullable<Int64>[0]);
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
        /// Event Subscription ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Event Channel ID
        /// </summary>
        [JsonPropertyName("event_channel_id")]
        public Nullable<Int64> EventChannelId
        {
            get { return (Nullable<Int64>)attributes["event_channel_id"]; }
            set { attributes["event_channel_id"] = value; }
        }

        /// <summary>
        /// Workspace ID. 0 means the default workspace or site-wide.
        /// </summary>
        [JsonPropertyName("workspace_id")]
        public Nullable<Int64> WorkspaceId
        {
            get { return (Nullable<Int64>)attributes["workspace_id"]; }
            set { attributes["workspace_id"] = value; }
        }

        /// <summary>
        /// If true, this default-workspace subscription applies to events from all workspaces.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("apply_to_all_workspaces")]
        public bool ApplyToAllWorkspaces
        {
            get { return attributes["apply_to_all_workspaces"] == null ? false : (bool)attributes["apply_to_all_workspaces"]; }
            set { attributes["apply_to_all_workspaces"] = value; }
        }

        /// <summary>
        /// Event Subscription name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Whether this Event Subscription can dispatch events.
        /// </summary>
        [JsonConverter(typeof(BooleanJsonConverter))]
        [JsonPropertyName("enabled")]
        public bool Enabled
        {
            get { return attributes["enabled"] == null ? false : (bool)attributes["enabled"]; }
            set { attributes["enabled"] = value; }
        }

        /// <summary>
        /// Event type strings matched by this subscription. Blank means all event types.
        /// </summary>
        [JsonPropertyName("event_types")]
        public string[] EventTypes
        {
            get { return (string[])attributes["event_types"]; }
            set { attributes["event_types"] = value; }
        }

        /// <summary>
        /// Structured event payload filter.
        /// </summary>
        [JsonPropertyName("filter")]
        public object Filter
        {
            get { return (object)attributes["filter"]; }
            set { attributes["filter"] = value; }
        }

        /// <summary>
        /// Event Subscription delivery policy.
        /// </summary>
        [JsonPropertyName("delivery_policy")]
        public object DeliveryPolicy
        {
            get { return (object)attributes["delivery_policy"]; }
            set { attributes["delivery_policy"] = value; }
        }

        /// <summary>
        /// Event Target IDs this subscription sends to.
        /// </summary>
        [JsonPropertyName("event_target_ids")]
        public Nullable<Int64>[] EventTargetIds
        {
            get { return (Nullable<Int64>[])attributes["event_target_ids"]; }
            set { attributes["event_target_ids"] = value; }
        }

        /// <summary>
        /// Event Subscription create date/time.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// Event Subscription update date/time.
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
        ///   event_channel_id - int64 - Event Channel ID
        ///   workspace_id - int64 - Workspace ID. 0 means the default workspace or site-wide.
        ///   apply_to_all_workspaces - boolean - If true, this default-workspace subscription applies to events from all workspaces.
        ///   name - string - Event Subscription name.
        ///   enabled - boolean - Whether this Event Subscription can dispatch events.
        ///   event_types - array(string) - Event type strings matched by this subscription. Blank means all event types.
        ///   filter - object - Structured event payload filter.
        ///   delivery_policy - object - Event Subscription delivery policy.
        ///   event_target_ids - array(int64) - Event Target IDs this subscription sends to.
        /// </summary>
        public async Task<EventSubscription> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("event_channel_id") && !(parameters["event_channel_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: event_channel_id must be of type Nullable<Int64>", "parameters[\"event_channel_id\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("event_types") && !(parameters["event_types"] is string[]))
            {
                throw new ArgumentException("Bad parameter: event_types must be of type string[]", "parameters[\"event_types\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("delivery_policy") && !(parameters["delivery_policy"] is object))
            {
                throw new ArgumentException("Bad parameter: delivery_policy must be of type object", "parameters[\"delivery_policy\"]");
            }
            if (parameters.ContainsKey("event_target_ids") && !(parameters["event_target_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: event_target_ids must be of type Nullable<Int64>[]", "parameters[\"event_target_ids\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/event_subscriptions/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<EventSubscription>(responseJson, JsonUtil.Options);
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

            await FilesClient.SendRequest($"/event_subscriptions/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await EventSubscription.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `enabled`, `event_channel_id` or `workspace_id`.
        ///   filter - object - If set, return records where the specified field is equal to the supplied value. Valid fields are `enabled`, `event_channel_id` or `workspace_id`. Valid field combinations are `[ enabled, event_channel_id ]`, `[ workspace_id, enabled ]` or `[ workspace_id, enabled, event_channel_id ]`.
        /// </summary>
        public static FilesList<EventSubscription> List(

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
            if (parameters.ContainsKey("sort_by") && !(parameters["sort_by"] is object))
            {
                throw new ArgumentException("Bad parameter: sort_by must be of type object", "parameters[\"sort_by\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }

            return new FilesList<EventSubscription>($"/event_subscriptions", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<EventSubscription> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Event Subscription ID.
        /// </summary>
        public static async Task<EventSubscription> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/event_subscriptions/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<EventSubscription>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<EventSubscription> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   event_channel_id - int64 - Event Channel ID
        ///   workspace_id - int64 - Workspace ID. 0 means the default workspace or site-wide.
        ///   apply_to_all_workspaces - boolean - If true, this default-workspace subscription applies to events from all workspaces.
        ///   name (required) - string - Event Subscription name.
        ///   enabled - boolean - Whether this Event Subscription can dispatch events.
        ///   event_types - array(string) - Event type strings matched by this subscription. Blank means all event types.
        ///   filter - object - Structured event payload filter.
        ///   delivery_policy - object - Event Subscription delivery policy.
        ///   event_target_ids - array(int64) - Event Target IDs this subscription sends to.
        /// </summary>
        public static async Task<EventSubscription> Create(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (!parameters.ContainsKey("name") || parameters["name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: name", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("event_channel_id") && !(parameters["event_channel_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: event_channel_id must be of type Nullable<Int64>", "parameters[\"event_channel_id\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("event_types") && !(parameters["event_types"] is string[]))
            {
                throw new ArgumentException("Bad parameter: event_types must be of type string[]", "parameters[\"event_types\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("delivery_policy") && !(parameters["delivery_policy"] is object))
            {
                throw new ArgumentException("Bad parameter: delivery_policy must be of type object", "parameters[\"delivery_policy\"]");
            }
            if (parameters.ContainsKey("event_target_ids") && !(parameters["event_target_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: event_target_ids must be of type Nullable<Int64>[]", "parameters[\"event_target_ids\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/event_subscriptions", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonSerializer.Deserialize<EventSubscription>(responseJson, JsonUtil.Options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   event_channel_id - int64 - Event Channel ID
        ///   workspace_id - int64 - Workspace ID. 0 means the default workspace or site-wide.
        ///   apply_to_all_workspaces - boolean - If true, this default-workspace subscription applies to events from all workspaces.
        ///   name - string - Event Subscription name.
        ///   enabled - boolean - Whether this Event Subscription can dispatch events.
        ///   event_types - array(string) - Event type strings matched by this subscription. Blank means all event types.
        ///   filter - object - Structured event payload filter.
        ///   delivery_policy - object - Event Subscription delivery policy.
        ///   event_target_ids - array(int64) - Event Target IDs this subscription sends to.
        /// </summary>
        public static async Task<EventSubscription> Update(
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
            if (parameters.ContainsKey("event_channel_id") && !(parameters["event_channel_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: event_channel_id must be of type Nullable<Int64>", "parameters[\"event_channel_id\"]");
            }
            if (parameters.ContainsKey("workspace_id") && !(parameters["workspace_id"] is Nullable<Int64>))
            {
                throw new ArgumentException("Bad parameter: workspace_id must be of type Nullable<Int64>", "parameters[\"workspace_id\"]");
            }
            if (parameters.ContainsKey("apply_to_all_workspaces") && !(parameters["apply_to_all_workspaces"] is bool))
            {
                throw new ArgumentException("Bad parameter: apply_to_all_workspaces must be of type bool", "parameters[\"apply_to_all_workspaces\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("enabled") && !(parameters["enabled"] is bool))
            {
                throw new ArgumentException("Bad parameter: enabled must be of type bool", "parameters[\"enabled\"]");
            }
            if (parameters.ContainsKey("event_types") && !(parameters["event_types"] is string[]))
            {
                throw new ArgumentException("Bad parameter: event_types must be of type string[]", "parameters[\"event_types\"]");
            }
            if (parameters.ContainsKey("filter") && !(parameters["filter"] is object))
            {
                throw new ArgumentException("Bad parameter: filter must be of type object", "parameters[\"filter\"]");
            }
            if (parameters.ContainsKey("delivery_policy") && !(parameters["delivery_policy"] is object))
            {
                throw new ArgumentException("Bad parameter: delivery_policy must be of type object", "parameters[\"delivery_policy\"]");
            }
            if (parameters.ContainsKey("event_target_ids") && !(parameters["event_target_ids"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: event_target_ids must be of type Nullable<Int64>[]", "parameters[\"event_target_ids\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/event_subscriptions/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonSerializer.Deserialize<EventSubscription>(responseJson, JsonUtil.Options);
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

            await FilesClient.SendRequest($"/event_subscriptions/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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