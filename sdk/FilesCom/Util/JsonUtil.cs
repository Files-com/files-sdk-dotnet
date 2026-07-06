using FilesCom.Models;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

namespace FilesCom.Util
{
    public static class JsonUtil
    {
        public static readonly JsonSerializerOptions Options = CreateOptions();

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DecimalJsonConverter());
            options.Converters.Add(new BooleanJsonConverter());
            return options;
        }

        public static T DeserializeWithOptions<T>(string json, Dictionary<string, object> requestOptions)
        {
            T value = JsonSerializer.Deserialize<T>(json, Options);
            ApplyOptions(value, requestOptions);
            return value;
        }

        private static void ApplyOptions(object value, Dictionary<string, object> requestOptions)
        {
            if (value == null || requestOptions == null)
            {
                return;
            }

            if (value is IEnumerable enumerable && !(value is string))
            {
                foreach (object item in enumerable)
                {
                    ApplyOptions(item, requestOptions);
                }
                return;
            }

            if (!(value is IModel model))
            {
                return;
            }

            model.SetOptions(requestOptions);
        }
    }
}