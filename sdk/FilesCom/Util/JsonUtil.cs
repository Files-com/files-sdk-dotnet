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
    }
}