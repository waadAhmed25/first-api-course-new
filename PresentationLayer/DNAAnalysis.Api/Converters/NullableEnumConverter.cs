using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNAAnalysis.API.Converters
{
    public class NullableEnumConverter<T> : JsonConverter<T?> where T : struct, Enum
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (Enum.TryParse<T>(value, true, out var result))
                return result;

            throw new JsonException($"Invalid value for enum {typeof(T).Name}");
        }

        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }
    }
}