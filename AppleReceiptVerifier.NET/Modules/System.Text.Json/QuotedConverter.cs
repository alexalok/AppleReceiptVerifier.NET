using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppleReceiptVerifierNET.Modules.System.Text.Json
{
    class QuotedConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(bool)
                || typeToConvert == typeof(long)
                || typeToConvert == typeof(int);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter) Activator.CreateInstance(typeof(QuotedConverterInternal<>).MakeGenericType(typeToConvert))!;
        }

        class QuotedConverterInternal<T> : JsonConverter<T>
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string stringValue = reader.GetString()!; // else how would we know it's a string anyway?
                    if (typeToConvert == typeof(bool) && int.TryParse(stringValue, out int intValue) && (intValue is 0 or 1))
                    {
                        return (T) (object) (intValue == 1);
                    }
                    return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(stringValue);
                }
                if (reader.TokenType == JsonTokenType.Number)
                {
                    if (typeToConvert == typeof(int))
                    {
                        int intValue = reader.GetInt32();
                        return (T) (object) intValue;
                    }
                    else if (typeToConvert == typeof(long))
                    {
                        long longValue = reader.GetInt64();
                        return (T) (object) longValue;
                    }
                }
                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                throw new NotSupportedException();
            }
        }

    }
}
