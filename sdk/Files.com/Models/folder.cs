
        public static Folder Create(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            return Folder.Create((string)attributes["path"], attributes, options);
        }

