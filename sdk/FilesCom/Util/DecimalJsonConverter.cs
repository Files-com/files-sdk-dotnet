using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilesCom.Util
{
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        /// <inheritdoc/>
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Text that is not a decimal, or is too large for one, is a JsonException, so that it is reported as an
            // invalid response like any other. Its message leaves the value out, as it could be anything, even a secret.
            // (A number too large is already one: the serializer rethrows GetDecimal's FormatException as a JsonException.)
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (String.IsNullOrEmpty(s))
                {
                    return 0m;
                }
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsed))
                {
                    return parsed;
                }
                throw new JsonException("The string is not a decimal number, or is too large for one.");
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

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}