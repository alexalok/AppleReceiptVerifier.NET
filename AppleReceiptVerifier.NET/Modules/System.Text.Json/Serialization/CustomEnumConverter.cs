﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Encodings.Web;
using AppleReceiptVerifierNET.Modules.System.Text.Json;

namespace System.Text.Json.Serialization
{
    // Taken from https://github.com/dotnet/runtime/issues/31619#issuecomment-891994805
    internal sealed class CustomEnumConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            object[]? knownValues = null;

            if (typeToConvert == typeof(BindingFlags))
            {
                knownValues = new object[] {BindingFlags.CreateInstance | BindingFlags.DeclaredOnly};
            }

            return (JsonConverter) Activator.CreateInstance(
                typeof(CustomEnumConverter<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new object?[] {new JsonSnakeCaseNamingPolicy(), options, knownValues},
                null)!;
        }
    }

    internal sealed class CustomEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        private readonly JsonNamingPolicy _namingPolicy;

        private readonly Dictionary<string, T> _readCache = new();
        private readonly Dictionary<T, JsonEncodedText> _writeCache = new();

        // This converter will only support up to 64 enum values (including flags) on serialization and deserialization
        private const int NameCacheLimit = 64;

        private const string ValueSeparator = ", ";

        public CustomEnumConverter(JsonNamingPolicy namingPolicy, JsonSerializerOptions options, object[]? knownValues)
        {
            _namingPolicy = namingPolicy;

            bool continueProcessing = true;
            for (int i = 0; i < knownValues?.Length; i++)
            {
                if (!TryProcessValue((T) knownValues[i]))
                {
                    continueProcessing = false;
                    break;
                }
            }

            if (continueProcessing)
            {
                Array values = Enum.GetValues(typeof(T));

                for (int i = 0; i < values.Length; i++)
                {
                    T value = (T) values.GetValue(i)!;

                    if (!TryProcessValue(value))
                    {
                        break;
                    }
                }
            }

            bool TryProcessValue(T value)
            {
                if (_readCache.Count == NameCacheLimit)
                {
                    Debug.Assert(_writeCache.Count == NameCacheLimit);
                    return false;
                }

                FormatAndAddToCaches(value, options.Encoder);
                return true;
            }
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException(); // All enums sent by Apple are strings.

            string? json = reader.GetString();
            if (json == null)
                return default;
            json = _namingPolicy.ConvertName(reader.GetString());

            if (!_readCache.TryGetValue(json, out T value))
            {
                if (int.TryParse(json, out var jsonInt) && Enum.IsDefined(typeToConvert, jsonInt))
                {
                    return (T) Enum.ToObject(typeToConvert, jsonInt);
                }

                throw new JsonException();
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (!_writeCache.TryGetValue(value, out JsonEncodedText formatted))
            {
                if (_writeCache.Count == NameCacheLimit)
                {
                    Debug.Assert(_readCache.Count == NameCacheLimit);
                    throw new ArgumentOutOfRangeException();
                }

                formatted = FormatAndAddToCaches(value, options.Encoder);
            }

            writer.WriteStringValue(formatted);
        }

        private JsonEncodedText FormatAndAddToCaches(T value, JavaScriptEncoder? encoder)
        {
            (string valueFormattedToStr, JsonEncodedText valueEncoded) =
                FormatEnumValue(value.ToString(), _namingPolicy, encoder);
            _readCache[valueFormattedToStr] = value;
            _writeCache[value] = valueEncoded;
            return valueEncoded;
        }

        private ValueTuple<string, JsonEncodedText> FormatEnumValue(string value, JsonNamingPolicy namingPolicy,
            JavaScriptEncoder? encoder)
        {
            string converted;

            if (!value.Contains(ValueSeparator))
            {
                converted = namingPolicy.ConvertName(value);
            }
            else
            {
#if NET5_0_OR_GREATER
                string[] enumValues = value.Split(ValueSeparator);
#else
                string[] enumValues = value.Split(new[] {ValueSeparator}, StringSplitOptions.None);
#endif

                for (int i = 0; i < enumValues.Length; i++)
                {
                    enumValues[i] = namingPolicy.ConvertName(enumValues[i]);
                }

                converted = string.Join(ValueSeparator, enumValues);
            }

            return (converted, JsonEncodedText.Encode(converted, encoder));
        }
    }
}