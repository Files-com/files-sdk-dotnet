
        public static Request Create(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            return Request.Create((string)attributes["path"], attributes, options);
        }

