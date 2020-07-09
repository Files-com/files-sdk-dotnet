using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Files.Models
{
    public class RemoteFile
    {
        private Dictionary<string, object> attributes;
        private Dictionary<string, object> options;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(RemoteFile));

        public static async Task<RemoteFile> Create(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            return (RemoteFile) await RemoteFile.Create((string)attributes["path"], attributes, options);
        }

        public static async Task<RemoteFile> DownloadFile(string path, string localPath = null)
        {
            localPath = localPath != null ? localPath : (System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + path.Substring(path.LastIndexOf('/') + 1));
            RemoteFile f = new RemoteFile();
            f.Path = path;
            await f.DownloadFile(localPath);
            return f;
        }

        private static async Task<Tuple<int, string>> UploadChunk(string path, System.IO.FileStream readStream, string fileRef, Int64 partNumber, int offset, int fileLength, Dictionary<string, object> options = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (fileRef != null) {
                parameters["ref"] = fileRef;
            }
            parameters["part"] = partNumber;
            parameters["mkdir_parents"] = true;
            parameters["mkdir_parents"] = true;

            FileAction[] uploadActions = await FileAction.BeginUpload(path, parameters, options);
            FileAction uploadAction = uploadActions[0];
            int chunkLength = Math.Min(fileLength - offset, uploadAction.Partsize);
            System.Net.Http.HttpMethod httpMethod = new System.Net.Http.HttpMethod(uploadAction.HttpMethod);

            await FilesClient.ChunkUpload(httpMethod, uploadAction.UploadUri, readStream, chunkLength);

            return Tuple.Create(chunkLength, uploadAction.Ref);
        }

        public static async Task<bool> UploadFile(string localPath, string destinationPath = null, Dictionary<string, object> options = null)
        {
            bool success = false;

            using (System.IO.FileStream localFileStream = System.IO.File.OpenRead(localPath))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(localPath);

                if (destinationPath == null) {
                    destinationPath = localPath.Substring(localPath.LastIndexOf('/') + 1);
                }

                // TODO: Remove int restriction on length to allow files > 2GB
                DateTime mtime = fileInfo.LastWriteTimeUtc;
                int fileLength = (int) fileInfo.Length;
                Int64 parts = 0;
                int bytesWritten = 0;
                string fileRef = null;

                // TODO: Set up multiple parallel streams instead of looping serial uploads here.
                while (bytesWritten < fileLength) {
                    parts++;
                    Tuple<int, string> result = await UploadChunk(destinationPath, localFileStream, fileRef, parts, bytesWritten, fileLength, options);
                    bytesWritten += result.Item1;
                    fileRef = result.Item2;
                }

                Dictionary<string, object> createParams = new Dictionary<string, object>();
                createParams["action"] = "end";
                createParams["provided_mtime"] = mtime.ToString("u");
                createParams["provided_mtime"] = mtime.ToString("u");
                createParams["ref"] = fileRef;
                createParams["size"] = (Int64) fileLength;

                await RemoteFile.Create(destinationPath, createParams);
                success = true;
            }
            return success;
        }

        public async Task<string> GetDownloadUriWithLoad()
        {
            if (DownloadUri == null) {
                RemoteFile f = (RemoteFile) await RemoteFile.Download(Path, null, options);
                attributes = f.attributes;
            }
            return DownloadUri;
        }

        public async Task DownloadFile(string outputFile)
        {
            try
            {
                string uri = await GetDownloadUriWithLoad();
                await FilesClient.StreamDownload(uri, outputFile);
            }
            catch (Exception e)
            {
                log.Error($"DownloadFile failed for {Path} to {outputFile}: {e.ToString()}");
            }
        }
        public RemoteFile()
        {
            this.attributes = new Dictionary<string, object>();
            this.options = new Dictionary<string, object>();

            this.attributes.Add("id", null);
            this.attributes.Add("path", null);
            this.attributes.Add("display_name", null);
            this.attributes.Add("type", null);
            this.attributes.Add("size", null);
            this.attributes.Add("mtime", null);
            this.attributes.Add("provided_mtime", null);
            this.attributes.Add("crc32", null);
            this.attributes.Add("md5", null);
            this.attributes.Add("mime_type", null);
            this.attributes.Add("region", null);
            this.attributes.Add("permissions", null);
            this.attributes.Add("subfolders_locked?", null);
            this.attributes.Add("download_uri", null);
            this.attributes.Add("priority_color", null);
            this.attributes.Add("preview_id", null);
            this.attributes.Add("preview", null);
            this.attributes.Add("action", null);
            this.attributes.Add("length", null);
            this.attributes.Add("mkdir_parents", null);
            this.attributes.Add("part", null);
            this.attributes.Add("parts", null);
            this.attributes.Add("ref", null);
            this.attributes.Add("restart", null);
            this.attributes.Add("structure", null);
            this.attributes.Add("with_rename", null);
        }

        public RemoteFile(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            this.attributes = attributes;
            this.options = options;
        }

        /// <summary>
        /// File/Folder ID
        /// </summary>
        [JsonPropertyName("id")]
        public Nullable<Int64> Id
        {
            get { return (Nullable<Int64>) attributes["id"]; }
            set { attributes["id"] = value; }
        }

        /// <summary>
        /// File/Folder path This must be slash-delimited, but it must neither start nor end with a slash. Maximum of 5000 characters.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path
        {
            get { return (string) attributes["path"]; }
            set { attributes["path"] = value; }
        }

        /// <summary>
        /// File/Folder display name
        /// </summary>
        [JsonPropertyName("display_name")]
        public string DisplayName
        {
            get { return (string) attributes["display_name"]; }
            set { attributes["display_name"] = value; }
        }

        /// <summary>
        /// Type: `directory` or `file`.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type
        {
            get { return (string) attributes["type"]; }
            set { attributes["type"] = value; }
        }

        /// <summary>
        /// File/Folder size
        /// </summary>
        [JsonPropertyName("size")]
        public Nullable<Int64> Size
        {
            get { return (Nullable<Int64>) attributes["size"]; }
            set { attributes["size"] = value; }
        }

        /// <summary>
        /// File last modified date/time, according to the server.  This is the timestamp of the last Files.com operation of the file, regardless of what modified timestamp was sent.
        /// </summary>
        [JsonPropertyName("mtime")]
        public Nullable<DateTime> Mtime
        {
            get { return (Nullable<DateTime>) attributes["mtime"]; }
            set { attributes["mtime"] = value; }
        }

        /// <summary>
        /// File last modified date/time, according to the client who set it.  Files.com allows desktop, FTP, SFTP, and WebDAV clients to set modified at times.  This allows Desktop<->Cloud syncing to preserve modified at times.
        /// </summary>
        [JsonPropertyName("provided_mtime")]
        public Nullable<DateTime> ProvidedMtime
        {
            get { return (Nullable<DateTime>) attributes["provided_mtime"]; }
            set { attributes["provided_mtime"] = value; }
        }

        /// <summary>
        /// File CRC32 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("crc32")]
        public string Crc32
        {
            get { return (string) attributes["crc32"]; }
            set { attributes["crc32"] = value; }
        }

        /// <summary>
        /// File MD5 checksum. This is sometimes delayed, so if you get a blank response, wait and try again.
        /// </summary>
        [JsonPropertyName("md5")]
        public string Md5
        {
            get { return (string) attributes["md5"]; }
            set { attributes["md5"] = value; }
        }

        /// <summary>
        /// MIME Type.  This is determined by the filename extension and is not stored separately internally.
        /// </summary>
        [JsonPropertyName("mime_type")]
        public string MimeType
        {
            get { return (string) attributes["mime_type"]; }
            set { attributes["mime_type"] = value; }
        }

        /// <summary>
        /// Region location
        /// </summary>
        [JsonPropertyName("region")]
        public string Region
        {
            get { return (string) attributes["region"]; }
            set { attributes["region"] = value; }
        }

        /// <summary>
        /// A short string representing the current user's permissions.  Can be `r`,`w`,`p`, or any combination
        /// </summary>
        [JsonPropertyName("permissions")]
        public string Permissions
        {
            get { return (string) attributes["permissions"]; }
            set { attributes["permissions"] = value; }
        }

        /// <summary>
        /// Are subfolders locked and unable to be modified?
        /// </summary>
        [JsonPropertyName("subfolders_locked?")]
        public bool SubfoldersLocked
        {
            get { return (bool) attributes["subfolders_locked?"]; }
            set { attributes["subfolders_locked?"] = value; }
        }

        /// <summary>
        /// Link to download file. Provided only in response to a download request.
        /// </summary>
        [JsonPropertyName("download_uri")]
        public string DownloadUri
        {
            get { return (string) attributes["download_uri"]; }
            set { attributes["download_uri"] = value; }
        }

        /// <summary>
        /// Bookmark/priority color of file/folder
        /// </summary>
        [JsonPropertyName("priority_color")]
        public string PriorityColor
        {
            get { return (string) attributes["priority_color"]; }
            set { attributes["priority_color"] = value; }
        }

        /// <summary>
        /// File preview ID
        /// </summary>
        [JsonPropertyName("preview_id")]
        public Nullable<Int64> PreviewId
        {
            get { return (Nullable<Int64>) attributes["preview_id"]; }
            set { attributes["preview_id"] = value; }
        }

        /// <summary>
        /// File preview
        /// </summary>
        [JsonPropertyName("preview")]
        public object Preview
        {
            get { return (object) attributes["preview"]; }
            set { attributes["preview"] = value; }
        }

        /// <summary>
        /// The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
        /// </summary>
        [JsonPropertyName("action")]
        public string Action
        {
            get { return (string) attributes["action"]; }
            set { attributes["action"] = value; }
        }

        /// <summary>
        /// Length of file.
        /// </summary>
        [JsonPropertyName("length")]
        public Nullable<Int64> Length
        {
            get { return (Nullable<Int64>) attributes["length"]; }
            set { attributes["length"] = value; }
        }

        /// <summary>
        /// Create parent directories if they do not exist?
        /// </summary>
        [JsonPropertyName("mkdir_parents")]
        public bool MkdirParents
        {
            get { return (bool) attributes["mkdir_parents"]; }
            set { attributes["mkdir_parents"] = value; }
        }

        /// <summary>
        /// Part if uploading a part.
        /// </summary>
        [JsonPropertyName("part")]
        public Nullable<Int64> Part
        {
            get { return (Nullable<Int64>) attributes["part"]; }
            set { attributes["part"] = value; }
        }

        /// <summary>
        /// How many parts to fetch?
        /// </summary>
        [JsonPropertyName("parts")]
        public Nullable<Int64> Parts
        {
            get { return (Nullable<Int64>) attributes["parts"]; }
            set { attributes["parts"] = value; }
        }

        /// <summary>
        /// </summary>
        [JsonPropertyName("ref")]
        public string Ref
        {
            get { return (string) attributes["ref"]; }
            set { attributes["ref"] = value; }
        }

        /// <summary>
        /// File byte offset to restart from.
        /// </summary>
        [JsonPropertyName("restart")]
        public Nullable<Int64> Restart
        {
            get { return (Nullable<Int64>) attributes["restart"]; }
            set { attributes["restart"] = value; }
        }

        /// <summary>
        /// If copying folder, copy just the structure?
        /// </summary>
        [JsonPropertyName("structure")]
        public string Structure
        {
            get { return (string) attributes["structure"]; }
            set { attributes["structure"] = value; }
        }

        /// <summary>
        /// Allow file rename instead of overwrite?
        /// </summary>
        [JsonPropertyName("with_rename")]
        public bool WithRename
        {
            get { return (bool) attributes["with_rename"]; }
            set { attributes["with_rename"] = value; }
        }

        /// <summary>
        /// Download file
        ///
        /// Parameters:
        ///   action - string - Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
        ///   id - int64 - If provided, lookup the file by id instead of path.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        public async Task<RemoteFile> Download(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("preview_size") && !(parameters["preview_size"] is string ))
            {
                throw new ArgumentException("Bad parameter: preview_size must be of type string", "parameters[\"preview_size\"]");
            }
            if (parameters.ContainsKey("with_previews") && !(parameters["with_previews"] is bool ))
            {
                throw new ArgumentException("Bad parameter: with_previews must be of type bool", "parameters[\"with_previews\"]");
            }
            if (parameters.ContainsKey("with_priority_color") && !(parameters["with_priority_color"] is bool ))
            {
                throw new ArgumentException("Bad parameter: with_priority_color must be of type bool", "parameters[\"with_priority_color\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/files/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<RemoteFile>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   provided_mtime - string - Modified time of file.
        ///   priority_color - string - Priority/Bookmark color of file.
        /// </summary>
        public async Task<RemoteFile> Update(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("provided_mtime") && !(parameters["provided_mtime"] is string ))
            {
                throw new ArgumentException("Bad parameter: provided_mtime must be of type string", "parameters[\"provided_mtime\"]");
            }
            if (parameters.ContainsKey("priority_color") && !(parameters["priority_color"] is string ))
            {
                throw new ArgumentException("Bad parameter: priority_color must be of type string", "parameters[\"priority_color\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/files/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<RemoteFile>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folers.  Otherwise, will error on non-empty folders.  For legacy reasons, this parameter may also be provided as the HTTP header `Depth: Infinity`
        /// </summary>
        public async Task<RemoteFile> Delete(Dictionary<string, object> parameters)
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
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool ))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/files/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<RemoteFile>(responseJson);
        }

        public async void Destroy(Dictionary<string, object> parameters)
        {
            Delete(parameters);
        }


        public async Task Save()
        {
            if (this.attributes["path"] != null)
            {
                await this.Update(this.attributes);
            }
            else
            {
                // TODO: Remove this cast after return types are implemented
                var newObj = (RemoteFile) await RemoteFile.Create(this.attributes, this.options);
                this.attributes = newObj.attributes;
            }
        }

        /// <summary>
        /// Download file
        ///
        /// Parameters:
        ///   action - string - Can be blank, `redirect` or `stat`.  If set to `stat`, we will return file information but without a download URL, and without logging a download.  If set to `redirect` we will serve a 302 redirect directly to the file.  This is used for integrations with Zapier, and is not recommended for most integrations.
        ///   id - int64 - If provided, lookup the file by id instead of path.
        ///   preview_size - string - Request a preview size.  Can be `small` (default), `large`, `xlarge`, or `pdf`.
        ///   with_previews - boolean - Include file preview information?
        ///   with_priority_color - boolean - Include file priority color information?
        /// </summary>
        public static async Task<RemoteFile> Download(
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
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("id") && !(parameters["id"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: id must be of type Nullable<Int64>", "parameters[\"id\"]");
            }
            if (parameters.ContainsKey("preview_size") && !(parameters["preview_size"] is string ))
            {
                throw new ArgumentException("Bad parameter: preview_size must be of type string", "parameters[\"preview_size\"]");
            }
            if (parameters.ContainsKey("with_previews") && !(parameters["with_previews"] is bool ))
            {
                throw new ArgumentException("Bad parameter: with_previews must be of type bool", "parameters[\"with_previews\"]");
            }
            if (parameters.ContainsKey("with_priority_color") && !(parameters["with_priority_color"] is bool ))
            {
                throw new ArgumentException("Bad parameter: with_priority_color must be of type bool", "parameters[\"with_priority_color\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/files/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Get, parameters, options);

            return JsonSerializer.Deserialize<RemoteFile>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   path (required) - string - Path to operate on.
        ///   action - string - The action to perform.  Can be `append`, `attachment`, `end`, `upload`, `put`, or may not exist
        ///   etags[etag] (required) - array(string) - etag identifier.
        ///   etags[part] (required) - array(int64) - Part number.
        ///   length - int64 - Length of file.
        ///   mkdir_parents - boolean - Create parent directories if they do not exist?
        ///   part - int64 - Part if uploading a part.
        ///   parts - int64 - How many parts to fetch?
        ///   provided_mtime - string - User provided modification time.
        ///   ref - string -
        ///   restart - int64 - File byte offset to restart from.
        ///   size - int64 - Size of file.
        ///   structure - string - If copying folder, copy just the structure?
        ///   with_rename - boolean - Allow file rename instead of overwrite?
        /// </summary>
        public static async Task<RemoteFile> Create(
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
            if (parameters.ContainsKey("action") && !(parameters["action"] is string ))
            {
                throw new ArgumentException("Bad parameter: action must be of type string", "parameters[\"action\"]");
            }
            if (parameters.ContainsKey("length") && !(parameters["length"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: length must be of type Nullable<Int64>", "parameters[\"length\"]");
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
            if (parameters.ContainsKey("provided_mtime") && !(parameters["provided_mtime"] is string ))
            {
                throw new ArgumentException("Bad parameter: provided_mtime must be of type string", "parameters[\"provided_mtime\"]");
            }
            if (parameters.ContainsKey("ref") && !(parameters["ref"] is string ))
            {
                throw new ArgumentException("Bad parameter: ref must be of type string", "parameters[\"ref\"]");
            }
            if (parameters.ContainsKey("restart") && !(parameters["restart"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: restart must be of type Nullable<Int64>", "parameters[\"restart\"]");
            }
            if (parameters.ContainsKey("size") && !(parameters["size"] is Nullable<Int64> ))
            {
                throw new ArgumentException("Bad parameter: size must be of type Nullable<Int64>", "parameters[\"size\"]");
            }
            if (parameters.ContainsKey("structure") && !(parameters["structure"] is string ))
            {
                throw new ArgumentException("Bad parameter: structure must be of type string", "parameters[\"structure\"]");
            }
            if (parameters.ContainsKey("with_rename") && !(parameters["with_rename"] is bool ))
            {
                throw new ArgumentException("Bad parameter: with_rename must be of type bool", "parameters[\"with_rename\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/files/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Post, parameters, options);

            return JsonSerializer.Deserialize<RemoteFile>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   provided_mtime - string - Modified time of file.
        ///   priority_color - string - Priority/Bookmark color of file.
        /// </summary>
        public static async Task<RemoteFile> Update(
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
            if (parameters.ContainsKey("provided_mtime") && !(parameters["provided_mtime"] is string ))
            {
                throw new ArgumentException("Bad parameter: provided_mtime must be of type string", "parameters[\"provided_mtime\"]");
            }
            if (parameters.ContainsKey("priority_color") && !(parameters["priority_color"] is string ))
            {
                throw new ArgumentException("Bad parameter: priority_color must be of type string", "parameters[\"priority_color\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/files/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Patch, parameters, options);

            return JsonSerializer.Deserialize<RemoteFile>(responseJson);
        }


        /// <summary>
        /// Parameters:
        ///   recursive - boolean - If true, will recursively delete folers.  Otherwise, will error on non-empty folders.  For legacy reasons, this parameter may also be provided as the HTTP header `Depth: Infinity`
        /// </summary>
        public static async Task<RemoteFile> Delete(
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
            if (parameters.ContainsKey("recursive") && !(parameters["recursive"] is bool ))
            {
                throw new ArgumentException("Bad parameter: recursive must be of type bool", "parameters[\"recursive\"]");
            }
            if (!parameters.ContainsKey("path") || parameters["path"] == null)
            {
                throw new ArgumentNullException("Parameter missing: path", "parameters[\"path\"]");
            }

            string responseJson = await FilesClient.SendRequest($"/files/{Uri.EscapeDataString(parameters["path"].ToString())}", System.Net.Http.HttpMethod.Delete, parameters, options);

            return JsonSerializer.Deserialize<RemoteFile>(responseJson);
        }

        public static async Task<RemoteFile> Destroy(
            string path, 
            Dictionary<string, object> parameters = null,
            Dictionary<string, object> options = null
        )
        {
            return await Delete(path, parameters, options);
        }

    }
}

