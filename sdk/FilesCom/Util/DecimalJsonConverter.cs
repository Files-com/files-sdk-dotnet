using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilesCom.Util
{
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (String.IsNullOrEmpty(s))
                {
                    return 0m;
                }
                return decimal.Parse(s, NumberStyles.Number, CultureInfo.InvariantCulture);
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDecimal();
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return 0m;
            }

            throw new JsonException($"Unsupported token type for decimal: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}