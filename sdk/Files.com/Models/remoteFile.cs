
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
