using System.Text.Json;
using System.Text.Json.Serialization;

namespace DNAAnalysis.API.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        private const string Format = @"hh\:mm\:ss";

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
{
    var value = reader.GetString();

    // ✅ لو فاضي → سيبيه يفشل validation مش parsing
    if (string.IsNullOrWhiteSpace(value))
        return default;

    if (TimeSpan.TryParse(value, out var time))
        return time;

    throw new JsonException("Time must be in format HH:mm:ss");
}

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}