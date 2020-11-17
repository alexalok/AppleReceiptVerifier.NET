using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppleReceiptVerifier.NET
{
    public class QuotedLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (long.TryParse(stringValue, out long value))
                {
                    return value;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}