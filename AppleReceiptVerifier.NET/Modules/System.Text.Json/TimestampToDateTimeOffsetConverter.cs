using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppleReceiptVerifier.NET.Modules.System.Text.Json
{
    class TimestampToDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (long.TryParse(stringValue, out long timestampMs))
                    return DateTimeOffset.FromUnixTimeMilliseconds(timestampMs);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
