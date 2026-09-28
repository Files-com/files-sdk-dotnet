using FilesCom.Models;
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
            if (requestOptions != null)
            {
                ModelContext.SetForResponse(value, null, requestOptions);
            }
            return value;
        }

        // Deserializes the response to a request made with client and requestOptions. The models in it, including
        // nested ones, belong to client and keep a copy of requestOptions, even when requestOptions is null.
        internal static T DeserializeWithOptions<T>(string json, FilesClient client, Dictionary<string, object> requestOptions)
        {
            T value = JsonSerializer.Deserialize<T>(json, Options);
            ModelContext.SetForResponse(value, client, requestOptions);
            return value;
        }
    }
}