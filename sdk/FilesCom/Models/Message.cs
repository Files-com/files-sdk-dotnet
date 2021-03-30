using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class Message
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public Message() : this(null, null) { }

        public Message(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("subject"))
            {
                this.attributes.Add("subject", null);
            }
            if (!this.attributes.ContainsKey("body"))
            {
                this.attributes.Add("body", null);
            }
            if (!this.attributes.ContainsKey("comments"))
            {
                this.attributes.Add("comments", new string[0]);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
            }
            if (!this.attributes.ContainsKey("project_id"))
            {
                this.attributes.Add("project_id", null);
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
        /// Message ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// Message subject.
        /// </summary>
        [JsonPropertyName("subject")]
        public string Subject
        {
            get { return (string) attributes["subject"]; }
            set { attributes["subject"] = value; }
        }

        /// <summary>
        /// Message body.
        /// </summary>
        [JsonPropertyName("body")]
        public string Body
        {
            get { return (string) attributes["body"]; }
            set { attributes["body"] = value; }
        }

        /// <summary>
        /// Comments.
        /// </summary>
        [JsonPropertyName("comments")]
        public string[] Comments
        {
            get { return (string[]) attributes["comments"]; }
            set { attributes["comments"] = value; }
        }

        /// <summary>
        /// User ID.  Provide a value of `0` to operate the current session's user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>) attributes["user_id"]; }
            set { attributes["user_id"] = value; }
        }

        /// <summary>
        /// Project to which the message should be attached.
        /// </summary>
        [JsonPropertyName("project_id")]
        public Nullable<Int64> ProjectId
        {
            get { return (Nullable<Int64>) attributes["project_id"]; }
            set { attributes["project_id"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   project_id (required) - int64 - Project to which the message should be attached.
        ///   subject (required) - string - Message subject.
        ///   body (required) - string - Message body.
        /// </summary>
        public async Task<Message> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("project_id") && !(parameters["project_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: project_id must be of type Nullable<Int64>", "parameters[\"project_id\"]");
            }
            if (parameters.ContainsKey("subject") && !(parameters["subject"] is string ))
            {
                throw new ArgumentException("Bad parameter: subject must be of type string", "parameters[\"subject\"]");
            }
            if (parameters.ContainsKey("body") && !(parameters["body"] is string ))
            {
                throw new ArgumentException("Bad parameter: body must be of type string", "parameters[\"body\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("project_id") || parameters["project_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: project_id", "parameters[\"project_id\"]");
            }
            if (!parameters.ContainsKey("subject") || parameters["subject"] == null)
            {
                throw new ArgumentNullException("Parameter missing: subject", "parameters[\"subject\"]");
            }
            if (!parameters.ContainsKey("body") || parameters["body"] == null)
            {
                throw new ArgumentNullException("Parameter missing: body", "parameters[\"body\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/messages/{attributes["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Message>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<Message> Delete(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendRequest($"/messages/{attributes["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Message>(responseJson);
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
                var newObj = await Message.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via the X-Files-Cursor-Next header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        ///   project_id (required) - int64 - Project for which to return messages.
        /// </summary>
        public static async Task<Message[]> List(
            
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
            if (parameters.ContainsKey("project_id") && !(parameters["project_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: project_id must be of type Nullable<Int64>", "parameters[\"project_id\"]");
            }
            if (!parameters.ContainsKey("project_id") || parameters["project_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: project_id", "parameters[\"project_id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/messages", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Message[]>(responseJson);
        }

        public static async Task<Message[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - Message ID.
        /// </summary>
        public static async Task<Message> Find(
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

            string responseJson = await FilesClient.SendRequest($"/messages/{parameters["id"]}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<Message>(responseJson);
        }

        public static async Task<Message> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   project_id (required) - int64 - Project to which the message should be attached.
        ///   subject (required) - string - Message subject.
        ///   body (required) - string - Message body.
        /// </summary>
        public static async Task<Message> Create(
            
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
            if (parameters.ContainsKey("project_id") && !(parameters["project_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: project_id must be of type Nullable<Int64>", "parameters[\"project_id\"]");
            }
            if (parameters.ContainsKey("subject") && !(parameters["subject"] is string ))
            {
                throw new ArgumentException("Bad parameter: subject must be of type string", "parameters[\"subject\"]");
            }
            if (parameters.ContainsKey("body") && !(parameters["body"] is string ))
            {
                throw new ArgumentException("Bad parameter: body must be of type string", "parameters[\"body\"]");
            }
            if (!parameters.ContainsKey("project_id") || parameters["project_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: project_id", "parameters[\"project_id\"]");
            }
            if (!parameters.ContainsKey("subject") || parameters["subject"] == null)
            {
                throw new ArgumentNullException("Parameter missing: subject", "parameters[\"subject\"]");
            }
            if (!parameters.ContainsKey("body") || parameters["body"] == null)
            {
                throw new ArgumentNullException("Parameter missing: body", "parameters[\"body\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/messages", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<Message>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   project_id (required) - int64 - Project to which the message should be attached.
        ///   subject (required) - string - Message subject.
        ///   body (required) - string - Message body.
        /// </summary>
        public static async Task<Message> Update(
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
            if (parameters.ContainsKey("project_id") && !(parameters["project_id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: project_id must be of type Nullable<Int64>", "parameters[\"project_id\"]");
            }
            if (parameters.ContainsKey("subject") && !(parameters["subject"] is string ))
            {
                throw new ArgumentException("Bad parameter: subject must be of type string", "parameters[\"subject\"]");
            }
            if (parameters.ContainsKey("body") && !(parameters["body"] is string ))
            {
                throw new ArgumentException("Bad parameter: body must be of type string", "parameters[\"body\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }
            if (!parameters.ContainsKey("project_id") || parameters["project_id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: project_id", "parameters[\"project_id\"]");
            }
            if (!parameters.ContainsKey("subject") || parameters["subject"] == null)
            {
                throw new ArgumentNullException("Parameter missing: subject", "parameters[\"subject\"]");
            }
            if (!parameters.ContainsKey("body") || parameters["body"] == null)
            {
                throw new ArgumentNullException("Parameter missing: body", "parameters[\"body\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/messages/{parameters["id"]}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<Message>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<Message> Delete(
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

            string responseJson = await FilesClient.SendRequest($"/messages/{parameters["id"]}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<Message>(responseJson);
        }

        public static async Task<Message> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

