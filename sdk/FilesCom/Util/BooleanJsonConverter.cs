using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilesCom.Util
{
    public class BooleanJsonConverter : JsonConverter<bool>
    {
        private static readonly SdkLog log = SdkLog.For(typeof(BooleanJsonConverter));
        /// <inheritdoc/>
        public override bool Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {

            // A value that is not a boolean is a JsonException, so that it is reported as an invalid response like any
            // other. Its message leaves the value out, as it could be anything, even a secret.
            if (reader.TokenType == JsonTokenType.String)
            {
                // "true" or "false", in any case and with surrounding white space, as Convert.ToBoolean has always read.
                if (bool.TryParse(reader.GetString(), out bool value))
                {
                    return value;
                }
                throw new JsonException("The string is not a boolean.");
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                throw new JsonException("A number is not a boolean.");
            }
            else if (reader.TokenType == JsonTokenType.False ||
                reader.TokenType == JsonTokenType.True)
            {
                return reader.GetBoolean();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return false;
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                reader.Skip();
                return false;
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                reader.Skip();
                return false;
            }
            else
            {
                log.Error($"Unsupported token type: {reader.TokenType}");
                throw new System.Text.Json.JsonException();
            }
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}