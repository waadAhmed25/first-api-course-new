using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNAAnalysis.API.Converters
{
    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (DateTime.TryParse(value, out var date))
                return date;

            throw new JsonException("Invalid date format");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }
}