using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilesCom.Util
{
    public class BooleanJsonConverter : JsonConverter<bool>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(BooleanJsonConverter));
        public override bool Read(ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {

            if (reader.TokenType == JsonTokenType.String)
            {
                return Convert.ToBoolean(reader.GetString() ?? String.Empty);
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                var stringValue = reader.GetDouble();
                return Convert.ToBoolean(stringValue.ToString());
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

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}