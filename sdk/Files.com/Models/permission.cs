
        public static Permission Create(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            return Permission.Create((string)attributes["path"], attributes, options);
        }

