using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class FileAction
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;
        public FileAction()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

        }

        public FileAction(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// Copy file/folder
        ///
        /// Parameters:
        ///   destination (required) - string - Copy destination path.
        /// </summary>
        public async Task<FileAction> Copy(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path")) {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/file_actions/copy/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<FileAction>(responseJson);
        }


        /// <summary>
        /// Move file/folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        /// </summary>
        public async Task<FileAction> Move(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path")) {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/file_actions/move/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<FileAction>(responseJson);
        }


        /// <summary>
        /// Begin file upload
        ///
        /// Parameters:
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        /// </summary>
        public async Task<FileAction[]> BeginUpload(Dictionary<string, object> parameters)
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            parameters["path"] = attributes["path"];

            if (!attributes.ContainsKey("path")) {
                throw new ArgumentException("Current object doesn't have a path");
            }
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("mkdir_parents") && !(parameters["mkdir_parents"] is bool ))
            {
                throw new ArgumentException("Bad parameter: mkdir_parents must be of type bool", "parameters[\"mkdir_parents\"]");
            }
            if (parameters.ContainsKey("part") && !(parameters["part"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: part must be of type Nullable<Int64>", "parameters[\"part\"]");
            }
            if (parameters.ContainsKey("parts") && !(parameters["parts"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: parts must be of type Nullable<Int64>", "parameters[\"parts\"]");
            }
            if (parameters.ContainsKey("ref") && !(parameters["ref"] is string ))
            {
                throw new ArgumentException("Bad parameter: ref must be of type string", "parameters[\"ref\"]");
            }
            if (parameters.ContainsKey("restart") && !(parameters["restart"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: restart must be of type Nullable<Int64>", "parameters[\"restart\"]");
            }
            if (parameters.ContainsKey("with_rename") && !(parameters["with_rename"] is bool ))
            {
                throw new ArgumentException("Bad parameter: with_rename must be of type bool", "parameters[\"with_rename\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/file_actions/begin_upload/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<FileAction[]>(responseJson);
        }




        /// <summary>
        /// Copy file/folder
        ///
        /// Parameters:
        ///   destination (required) - string - Copy destination path.
        /// </summary>
        public static async Task<FileAction> Copy(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/file_actions/copy/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<FileAction>(responseJson);
        }


        /// <summary>
        /// Move file/folder
        ///
        /// Parameters:
        ///   destination (required) - string - Move destination path.
        /// </summary>
        public static async Task<FileAction> Move(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("destination") && !(parameters["destination"] is string ))
            {
                throw new ArgumentException("Bad parameter: destination must be of type string", "parameters[\"destination\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }
            if (!parameters.ContainsKey("destination") || parameters["destination"] == null)
            {
                throw new ArgumentNullException("Parameter missing: destination", "parameters[\"destination\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/file_actions/move/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<FileAction>(responseJson);
        }


        /// <summary>
        /// Begin file upload
        ///
        /// Parameters:
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        /// </summary>
        public static async Task<FileAction[]> BeginUpload(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            parameters = parameters != null ? parameters : new Dictionary<string, object>();
            options = options != null ? options : new Dictionary<string, object>();

            parameters.Add("path", path);
            if (parameters.ContainsKey("path") && !(parameters["path"] is string ))
            {
                throw new ArgumentException("Bad parameter: path must be of type string", "parameters[\"path\"]");
            }
            if (parameters.ContainsKey("mkdir_parents") && !(parameters["mkdir_parents"] is bool ))
            {
                throw new ArgumentException("Bad parameter: mkdir_parents must be of type bool", "parameters[\"mkdir_parents\"]");
            }
            if (parameters.ContainsKey("part") && !(parameters["part"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: part must be of type Nullable<Int64>", "parameters[\"part\"]");
            }
            if (parameters.ContainsKey("parts") && !(parameters["parts"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: parts must be of type Nullable<Int64>", "parameters[\"parts\"]");
            }
            if (parameters.ContainsKey("ref") && !(parameters["ref"] is string ))
            {
                throw new ArgumentException("Bad parameter: ref must be of type string", "parameters[\"ref\"]");
            }
            if (parameters.ContainsKey("restart") && !(parameters["restart"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: restart must be of type Nullable<Int64>", "parameters[\"restart\"]");
            }
            if (parameters.ContainsKey("with_rename") && !(parameters["with_rename"] is bool ))
            {
                throw new ArgumentException("Bad parameter: with_rename must be of type bool", "parameters[\"with_rename\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/file_actions/begin_upload/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<FileAction[]>(responseJson);
        }


    }
}

