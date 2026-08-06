using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Schedule : IModel
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Schedule() : this(null, null) { }

        public Schedule(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("name"))
            {
                this.attributes.Add("name", null);
            }
            if (!this.attributes.ContainsKey("schedule_days_of_week"))
            {
                this.attributes.Add("schedule_days_of_week", new Nullable<Int64>[0]);
            }
            if (!this.attributes.ContainsKey("schedule_times_of_day"))
            {
                this.attributes.Add("schedule_times_of_day", new string[0]);
            }
            if (!this.attributes.ContainsKey("schedule_time_zone"))
            {
                this.attributes.Add("schedule_time_zone", null);
            }
            if (!this.attributes.ContainsKey("holiday_region"))
            {
                this.attributes.Add("holiday_region", null);
            }
            if (!this.attributes.ContainsKey("human_readable_schedule"))
            {
                this.attributes.Add("human_readable_schedule", null);
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

        void IModel.SetOptions(Dictionary<string, object> options)
        {
            this.options = options != null ? new Dictionary<string, object>(options) : new Dictionary<string, object>();
        }

        public void SetOption(string name, object value)
        {
            this.options[name] = value;
        }


        /// <summary>
        /// Schedule ID.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Schedule name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string)attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// 0-based weekdays used by the Schedule. 0 is Sunday.
        /// </summary>
        [JsonPropertyName("schedule_days_of_week")]
        public Nullable<Int64>[] ScheduleDaysOfWeek
        {
            get { return (Nullable<Int64>[])attributes["schedule_days_of_week"]; }
            set { attributes["schedule_days_of_week"] = value; }
        }

        /// <summary>
        /// Times of day in HH:MM format (24-hour).
        /// </summary>
        [JsonPropertyName("schedule_times_of_day")]
        public string[] ScheduleTimesOfDay
        {
            get { return (string[])attributes["schedule_times_of_day"]; }
            set { attributes["schedule_times_of_day"] = value; }
        }

        /// <summary>
        /// Time zone for scheduled times. If not set, times are interpreted as UTC.
        /// </summary>
        [JsonPropertyName("schedule_time_zone")]
        public string ScheduleTimeZone
        {
            get { return (string)attributes["schedule_time_zone"]; }
            set { attributes["schedule_time_zone"] = value; }
        }

        /// <summary>
        /// Optional holiday region on which linked resources do not run.
        /// </summary>
        [JsonPropertyName("holiday_region")]
        public string HolidayRegion
        {
            get { return (string)attributes["holiday_region"]; }
            set { attributes["holiday_region"] = value; }
        }

        /// <summary>
        /// Human-readable Schedule description.
        /// </summary>
        [JsonPropertyName("human_readable_schedule")]
        public string HumanReadableSchedule
        {
            get { return (string)attributes["human_readable_schedule"]; }
            set { attributes["human_readable_schedule"] = value; }
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
        ///   name - string - Schedule name.
        ///   schedule_days_of_week - array(int64) - 0-based weekdays used by the Schedule. 0 is Sunday.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format (24-hour).
        ///   schedule_time_zone - string - Time zone for scheduled times. If not set, times are interpreted as UTC.
        ///   holiday_region - string - Optional holiday region on which linked resources do not run.
        /// </summary>
        public async Task<Schedule> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/schedules/{System.Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<Schedule>(responseJson, options);
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

            await FilesClient.SendRequest($"/schedules/{System.Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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
                var newObj = await Schedule.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10000, 1,000 or less is recommended).
        ///   sort_by - object - If set, sort records by the specified field in either `asc` or `desc` direction. Valid fields are `name`.
        /// </summary>
        public static FilesList<Schedule> List(

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

            return new FilesList<Schedule>($"/schedules", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<Schedule> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Schedule ID.
        /// </summary>
        public static async Task<Schedule> Find(
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

            string responseJson = await FilesClient.SendStringRequest($"/schedules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<Schedule>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }

        public static async Task<Schedule> Get(
            Nullable<Int64> id,
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name (required) - string - Schedule name.
        ///   schedule_days_of_week (required) - array(int64) - 0-based weekdays used by the Schedule. 0 is Sunday.
        ///   schedule_times_of_day (required) - array(string) - Times of day in HH:MM format (24-hour).
        ///   schedule_time_zone - string - Time zone for scheduled times. If not set, times are interpreted as UTC.
        ///   holiday_region - string - Optional holiday region on which linked resources do not run.
        /// </summary>
        public static async Task<Schedule> Create(

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
            if (!parameters.ContainsKey("schedule_days_of_week") || parameters["schedule_days_of_week"] == null)
            {
                throw new ArgumentNullException("Parameter missing: schedule_days_of_week", "parameters[\"schedule_days_of_week\"]");
            }
            if (!parameters.ContainsKey("schedule_times_of_day") || parameters["schedule_times_of_day"] == null)
            {
                throw new ArgumentNullException("Parameter missing: schedule_times_of_day", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/schedules", System.Net.Http.HttpMethod.Post, parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<Schedule>(responseJson, options);
            }
            catch (JsonException)
            {
                throw new InvalidResponseException("Unexpected data received from server: " + responseJson);
            }
        }


        /// <summary>
        /// Parameters:
        ///   name - string - Schedule name.
        ///   schedule_days_of_week - array(int64) - 0-based weekdays used by the Schedule. 0 is Sunday.
        ///   schedule_times_of_day - array(string) - Times of day in HH:MM format (24-hour).
        ///   schedule_time_zone - string - Time zone for scheduled times. If not set, times are interpreted as UTC.
        ///   holiday_region - string - Optional holiday region on which linked resources do not run.
        /// </summary>
        public static async Task<Schedule> Update(
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("schedule_days_of_week") && !(parameters["schedule_days_of_week"] is Nullable<Int64>[]))
            {
                throw new ArgumentException("Bad parameter: schedule_days_of_week must be of type Nullable<Int64>[]", "parameters[\"schedule_days_of_week\"]");
            }
            if (parameters.ContainsKey("schedule_times_of_day") && !(parameters["schedule_times_of_day"] is string[]))
            {
                throw new ArgumentException("Bad parameter: schedule_times_of_day must be of type string[]", "parameters[\"schedule_times_of_day\"]");
            }
            if (parameters.ContainsKey("schedule_time_zone") && !(parameters["schedule_time_zone"] is string))
            {
                throw new ArgumentException("Bad parameter: schedule_time_zone must be of type string", "parameters[\"schedule_time_zone\"]");
            }
            if (parameters.ContainsKey("holiday_region") && !(parameters["holiday_region"] is string))
            {
                throw new ArgumentException("Bad parameter: holiday_region must be of type string", "parameters[\"holiday_region\"]");
            }

            string responseJson = await FilesClient.SendStringRequest($"/schedules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            try
            {
                return JsonUtil.DeserializeWithOptions<Schedule>(responseJson, options);
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

            await FilesClient.SendRequest($"/schedules/{System.Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);
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