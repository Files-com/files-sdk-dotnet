using FilesCom.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class UserCipherUse
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public UserCipherUse() : this(null, null) { }

        public UserCipherUse(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("protocol_cipher"))
            {
                this.attributes.Add("protocol_cipher", null);
            }
            if (!this.attributes.ContainsKey("created_at"))
            {
                this.attributes.Add("created_at", null);
            }
            if (!this.attributes.ContainsKey("interface"))
            {
                this.attributes.Add("interface", null);
            }
            if (!this.attributes.ContainsKey("updated_at"))
            {
                this.attributes.Add("updated_at", null);
            }
            if (!this.attributes.ContainsKey("user_id"))
            {
                this.attributes.Add("user_id", null);
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
        /// UserCipherUse ID
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>)attributes["id"]; }
            private set { attributes["id"] = value; }
        }

        /// <summary>
        /// The protocol and cipher employed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("protocol_cipher")]
        public string ProtocolCipher
        {
            get { return (string)attributes["protocol_cipher"]; }
            private set { attributes["protocol_cipher"] = value; }
        }

        /// <summary>
        /// The earliest recorded use of this combination of interface and protocol and cipher (for this user)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public Nullable<DateTime> CreatedAt
        {
            get { return (Nullable<DateTime>)attributes["created_at"]; }
            private set { attributes["created_at"] = value; }
        }

        /// <summary>
        /// The interface accessed
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("interface")]
        public string Interface
        {
            get { return (string)attributes["interface"]; }
            private set { attributes["interface"] = value; }
        }

        /// <summary>
        /// The most recent use of this combination of interface and protocol and cipher (for this user)
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("updated_at")]
        public Nullable<DateTime> UpdatedAt
        {
            get { return (Nullable<DateTime>)attributes["updated_at"]; }
            private set { attributes["updated_at"] = value; }
        }

        /// <summary>
        /// ID of the user who performed this access
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("user_id")]
        public Nullable<Int64> UserId
        {
            get { return (Nullable<Int64>)attributes["user_id"]; }
            private set { attributes["user_id"] = value; }
        }



        /// <summary>
        /// Parameters:
        ///   user_id - int64 - User ID.  Provide a value of `0` to operate the current session's user.
        ///   cursor - string - Used for pagination.  When a list request has more records available, cursors are provided in the response headers `X-Files-Cursor-Next` and `X-Files-Cursor-Prev`.  Send one of those cursor value here to resume an existing list from the next available record.  Note: many of our SDKs have iterator methods that will automatically handle cursor-based pagination.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static FilesList<UserCipherUse> List(

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

            return new FilesList<UserCipherUse>($"/user_cipher_uses", System.Net.Http.HttpMethod.Get, parameters, options);
        }

        public static FilesList<UserCipherUse> All(

            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return List(parameters, options);
        }

    }
}