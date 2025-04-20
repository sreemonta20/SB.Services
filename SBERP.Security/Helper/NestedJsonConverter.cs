using System.Text.Json;
using System.Text.Json.Serialization;

namespace SBERP.Security.Helper
{
    public class NestedJsonConverter<T> : JsonConverter<List<T>>
    {
        public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                // Parse the nested JSON string
                string nestedJson = reader.GetString()!;
                return JsonSerializer.Deserialize<List<T>>(nestedJson, options);
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                // Parse as a regular JSON array
                return JsonSerializer.Deserialize<List<T>>(ref reader, options);
            }
            throw new JsonException("Unexpected token type");
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            // Serialize the list back as a JSON array
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
