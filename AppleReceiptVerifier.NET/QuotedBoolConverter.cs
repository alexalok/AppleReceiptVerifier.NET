using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppleReceiptVerifier.NET
{
    public class QuotedBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (bool.TryParse(stringValue, out bool value))
                {
                    return value;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}