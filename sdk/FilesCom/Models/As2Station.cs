using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FilesCom.Models
{
    public class As2Station
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public As2Station() : this(null, null) { }

        public As2Station(Dictionary<string, object> attributes, Dictionary<string, object> options)
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
            if (!this.attributes.ContainsKey("uri"))
            {
                this.attributes.Add("uri", null);
            }
            if (!this.attributes.ContainsKey("domain"))
            {
                this.attributes.Add("domain", null);
            }
            if (!this.attributes.ContainsKey("hex_public_certificate_serial"))
            {
                this.attributes.Add("hex_public_certificate_serial", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_md5"))
            {
                this.attributes.Add("public_certificate_md5", null);
            }
            if (!this.attributes.ContainsKey("private_key_md5"))
            {
                this.attributes.Add("private_key_md5", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_subject"))
            {
                this.attributes.Add("public_certificate_subject", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_issuer"))
            {
                this.attributes.Add("public_certificate_issuer", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_serial"))
            {
                this.attributes.Add("public_certificate_serial", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_not_before"))
            {
                this.attributes.Add("public_certificate_not_before", null);
            }
            if (!this.attributes.ContainsKey("public_certificate_not_after"))
            {
                this.attributes.Add("public_certificate_not_after", null);
            }
            if (!this.attributes.ContainsKey("private_key_password_md5"))
            {
                this.attributes.Add("private_key_password_md5", null);
            }
            if (!this.attributes.ContainsKey("public_certificate"))
            {
                this.attributes.Add("public_certificate", null);
            }
            if (!this.attributes.ContainsKey("private_key"))
            {
                this.attributes.Add("private_key", null);
            }
            if (!this.attributes.ContainsKey("private_key_password"))
            {
                this.attributes.Add("private_key_password", null);
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
        /// Id of the AS2 Station.
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// The station's formal AS2 name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name
        {
            get { return (string) attributes["name"]; }
            set { attributes["name"] = value; }
        }

        /// <summary>
        /// Public URI for sending AS2 message to.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri
        {
            get { return (string) attributes["uri"]; }
            set { attributes["uri"] = value; }
        }

        /// <summary>
        /// The station's AS2 domain name.
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain
        {
            get { return (string) attributes["domain"]; }
            set { attributes["domain"] = value; }
        }

        /// <summary>
        /// Serial of public certificate used for message security in hex format.
        /// </summary>
        [JsonPropertyName("hex_public_certificate_serial")]
        public string HexPublicCertificateSerial
        {
            get { return (string) attributes["hex_public_certificate_serial"]; }
            set { attributes["hex_public_certificate_serial"] = value; }
        }

        /// <summary>
        /// MD5 hash of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_md5")]
        public string PublicCertificateMd5
        {
            get { return (string) attributes["public_certificate_md5"]; }
            set { attributes["public_certificate_md5"] = value; }
        }

        /// <summary>
        /// MD5 hash of private key used for message security.
        /// </summary>
        [JsonPropertyName("private_key_md5")]
        public string PrivateKeyMd5
        {
            get { return (string) attributes["private_key_md5"]; }
            set { attributes["private_key_md5"] = value; }
        }

        /// <summary>
        /// Subject of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_subject")]
        public string PublicCertificateSubject
        {
            get { return (string) attributes["public_certificate_subject"]; }
            set { attributes["public_certificate_subject"] = value; }
        }

        /// <summary>
        /// Issuer of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_issuer")]
        public string PublicCertificateIssuer
        {
            get { return (string) attributes["public_certificate_issuer"]; }
            set { attributes["public_certificate_issuer"] = value; }
        }

        /// <summary>
        /// Serial of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_serial")]
        public string PublicCertificateSerial
        {
            get { return (string) attributes["public_certificate_serial"]; }
            set { attributes["public_certificate_serial"] = value; }
        }

        /// <summary>
        /// Not before value of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_not_before")]
        public string PublicCertificateNotBefore
        {
            get { return (string) attributes["public_certificate_not_before"]; }
            set { attributes["public_certificate_not_before"] = value; }
        }

        /// <summary>
        /// Not after value of public certificate used for message security.
        /// </summary>
        [JsonPropertyName("public_certificate_not_after")]
        public string PublicCertificateNotAfter
        {
            get { return (string) attributes["public_certificate_not_after"]; }
            set { attributes["public_certificate_not_after"] = value; }
        }

        /// <summary>
        /// MD5 hash of private key password used for message security.
        /// </summary>
        [JsonPropertyName("private_key_password_md5")]
        public string PrivateKeyPasswordMd5
        {
            get { return (string) attributes["private_key_password_md5"]; }
            set { attributes["private_key_password_md5"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonPropertyName("public_certificate")]
        public string PublicCertificate
        {
            get { return (string) attributes["public_certificate"]; }
            set { attributes["public_certificate"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonPropertyName("private_key")]
        public string PrivateKey
        {
            get { return (string) attributes["private_key"]; }
            set { attributes["private_key"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonPropertyName("private_key_password")]
        public string PrivateKeyPassword
        {
            get { return (string) attributes["private_key_password"]; }
            set { attributes["private_key_password"] = value; }
        }

        /// <summary>
        /// Parameters:
        ///   name - string - AS2 Name
        ///   public_certificate - string
        ///   private_key - string
        ///   private_key_password - string
        /// </summary>
        public async Task<As2Station> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("public_certificate") && !(parameters["public_certificate"] is string ))
            {
                throw new ArgumentException("Bad parameter: public_certificate must be of type string", "parameters[\"public_certificate\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string ))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_password") && !(parameters["private_key_password"] is string ))
            {
                throw new ArgumentException("Bad parameter: private_key_password must be of type string", "parameters[\"private_key_password\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/as2_stations/{Uri.EscapeDataString(attributes["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<As2Station>(responseJson);
        }


        /// <summary>
        /// </summary>
        public async Task<As2Station> Delete(Dictionary<string, object> parameters)
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

            string responseJson = await FilesClient.SendRequest($"/as2_stations/{Uri.EscapeDataString(attributes["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<As2Station>(responseJson);
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
                var newObj = await As2Station.Create(this.attributes, this.options);
                this.attributes = newObj.getAttributes();
            }
        }

        /// <summary>
        /// Parameters:
        ///   cursor - string - Used for pagination.  Send a cursor value to resume an existing list from the point at which you left off.  Get a cursor from an existing list via either the X-Files-Cursor-Next header or the X-Files-Cursor-Prev header.
        ///   per_page - int64 - Number of records to show per page.  (Max: 10,000, 1,000 or less is recommended).
        /// </summary>
        public static async Task<As2Station[]> List(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("cursor") && !(parameters["cursor"] is string ))
            {
                throw new ArgumentException("Bad parameter: cursor must be of type string", "parameters[\"cursor\"]");
            }
            if (parameters.ContainsKey("per_page") && !(parameters["per_page"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: per_page must be of type Nullable<Int64>", "parameters[\"per_page\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/as2_stations", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<As2Station[]>(responseJson);
        }

        public static async Task<As2Station[]> All(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await List(parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   id (required) - int64 - As2 Station ID.
        /// </summary>
        public static async Task<As2Station> Find(
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

            string responseJson = await FilesClient.SendRequest($"/as2_stations/{Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<As2Station>(responseJson);
        }

        public static async Task<As2Station> Get(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Find(id, parameters, options);
        }

        /// <summary>
        /// Parameters:
        ///   name (required) - string - AS2 Name
        ///   public_certificate (required) - string
        ///   private_key (required) - string
        ///   private_key_password - string
        /// </summary>
        public static async Task<As2Station> Create(
            
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("public_certificate") && !(parameters["public_certificate"] is string ))
            {
                throw new ArgumentException("Bad parameter: public_certificate must be of type string", "parameters[\"public_certificate\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string ))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_password") && !(parameters["private_key_password"] is string ))
            {
                throw new ArgumentException("Bad parameter: private_key_password must be of type string", "parameters[\"private_key_password\"]");
            }
            if (!parameters.ContainsKey("name") || parameters["name"] == null)
            {
                throw new ArgumentNullException("Parameter missing: name", "parameters[\"name\"]");
            }
            if (!parameters.ContainsKey("public_certificate") || parameters["public_certificate"] == null)
            {
                throw new ArgumentNullException("Parameter missing: public_certificate", "parameters[\"public_certificate\"]");
            }
            if (!parameters.ContainsKey("private_key") || parameters["private_key"] == null)
            {
                throw new ArgumentNullException("Parameter missing: private_key", "parameters[\"private_key\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/as2_stations", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<As2Station>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   name - string - AS2 Name
        ///   public_certificate - string
        ///   private_key - string
        ///   private_key_password - string
        /// </summary>
        public static async Task<As2Station> Update(
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
            if (parameters.ContainsKey("name") && !(parameters["name"] is string ))
            {
                throw new ArgumentException("Bad parameter: name must be of type string", "parameters[\"name\"]");
            }
            if (parameters.ContainsKey("public_certificate") && !(parameters["public_certificate"] is string ))
            {
                throw new ArgumentException("Bad parameter: public_certificate must be of type string", "parameters[\"public_certificate\"]");
            }
            if (parameters.ContainsKey("private_key") && !(parameters["private_key"] is string ))
            {
                throw new ArgumentException("Bad parameter: private_key must be of type string", "parameters[\"private_key\"]");
            }
            if (parameters.ContainsKey("private_key_password") && !(parameters["private_key_password"] is string ))
            {
                throw new ArgumentException("Bad parameter: private_key_password must be of type string", "parameters[\"private_key_password\"]");
            }
            if (!parameters.ContainsKey("id") || parameters["id"] == null)
            {
                throw new ArgumentNullException("Parameter missing: id", "parameters[\"id\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/as2_stations/{Uri.EscapeDataString(parameters["id"].ToString())}", new HttpMethod("PATCH"), parameters, options);

            return JsonSerializer.Deserialize<As2Station>(responseJson);
        }


        /// <summary>
        /// </summary>
        public static async Task<As2Station> Delete(
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

            string responseJson = await FilesClient.SendRequest($"/as2_stations/{Uri.EscapeDataString(parameters["id"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<As2Station>(responseJson);
        }

        public static async Task<As2Station> Destroy(
            Nullable<Int64> id, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(id, parameters, options);
        }

    }
}

