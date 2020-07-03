
        public static Lock Create(Dictionary<string, object> attributes, Dictionary<string, object> options)
        {
            return Lock.Create((string)attributes["path"], attributes, options);
        }

