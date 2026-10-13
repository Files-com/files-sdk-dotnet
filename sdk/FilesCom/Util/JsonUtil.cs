using FilesCom.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace FilesCom.Util
{
    public static class JsonUtil
    {
        private static readonly SdkLog log = SdkLog.For(typeof(JsonUtil));

        public static readonly JsonSerializerOptions Options = CreateOptions();

        private static JsonSerializerOptions CreateOptions()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DecimalJsonConverter());
            options.Converters.Add(new BooleanJsonConverter());
            return options;
        }

        /// <summary>
        /// Reads <paramref name="json"/> as a <typeparamref name="T"/>, with the SDK's decimal and boolean readings, and
        /// gives the models in it the request options they were read for.
        /// </summary>
        /// <typeparam name="T">The type to read, such as a model or an array of models.</typeparam>
        /// <param name="json">The JSON text.</param>
        /// <param name="requestOptions">The request options for the models in it, or null to leave them without.</param>
        /// <returns>The value read.</returns>
        public static T DeserializeWithOptions<T>(string json, Dictionary<string, object> requestOptions)
        {
            T value = JsonSerializer.Deserialize<T>(json, Options);
            if (requestOptions != null)
            {
                ModelContext.SetForResponse(value, null, requestOptions, null);
            }
            return value;
        }

        // Deserializes the response to a request of an operation with context, made with requestOptions. The models in
        // it, including nested ones, belong to the operation's client, keep a copy of requestOptions, even when
        // requestOptions is null, and send their requests where the operation's went. A
        // response that is not JSON of type T is an InvalidResponseException. The JsonException saying why is logged
        // at DEBUG rather than kept, since its message can quote the response, such as a key that is a signed URL.
        internal static T DeserializeWithOptions<T>(string json, OperationContext context, Dictionary<string, object> requestOptions)
        {
            T value = Read<T>(json);
            ModelContext.SetForResponse(value, context.Client, requestOptions, context.RelocatedOrigin);
            return value;
        }

        // The same for the response of an operation that returns a model, or an array of models, which must be one:
        // JSON null, or an array holding a null, is an InvalidResponseException too, before any model gets client or
        // requestOptions. Any other JSON that is not an object is one already, and {} is a model like any other.
        internal static T DeserializeModelResponse<T>(string json, OperationContext context, Dictionary<string, object> requestOptions) where T : class
        {
            T value = Read<T>(json);
            if (value == null || (value is Array models && Array.IndexOf(models, null) >= 0))
            {
                throw new InvalidResponseException("Unexpected data received from server: null where the operation returns an object");
            }
            ModelContext.SetForResponse(value, context.Client, requestOptions, context.RelocatedOrigin);
            return value;
        }

        private static T Read<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, Options);
            }
            catch (JsonException e)
            {
                log.Debug("The response is not the data the operation returns", e);
                throw new InvalidResponseException("Unexpected data received from server");
            }
        }
    }
}